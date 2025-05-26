/// <reference path="DrawContext.js" />





var MseButtons = {
    Left: 1,
    Middle: 2,
    Right: 3
};

var Cursors = {
    Default: 'default',
    Pointer: 'pointer',
    Text: 'text',
    Wait: 'wait',
    Help: 'help',
    ResizeN: 'n-resize',
    ResizeE: 'e-resize',
    ResizeNE: 'ne-resize',
    ResizeNW: 'nw-resize',
    ResizeCol: 'col-resize',
    ResizeRow: 'row-resize'
}







/****************************************************************
* Drawn Object
***************************************************************/
function DrawnObject(parent) {
    var _Parent = parent ? parent : null;
    var _Dim = { X: 0, Y: 0, W: 0, H: 0 };
    var _Children = [];
    var _Buffer = null;
    var _MyDrawingCtx = null;
    var _Invalid = true;
    var _Scale = 1;

    if (_Parent != null) {
        _Parent.GetChildren().push(this);
        _Parent.ChildrenChanged();
    }

    var _GetBuffer = function() {
        if (_Buffer == null) {
            var buff = document.createElement('canvas');
            _Buffer = buff;
            _EnsureBuffSize();
        }
        return _Buffer;
    }
    var _Ensured = false;
    var _EnsureBuffSize = function() {
        if (_GetBuffer().width != _Dim.W)
            _GetBuffer().width = _Dim.W;
        if (_GetBuffer().height != _Dim.H)
            _GetBuffer().height = _Dim.H;
    }
    var _GetCtx = function() {
        if (_MyDrawingCtx == null) {
            _MyDrawingCtx = new DrawContext(_GetBuffer());
            _MyDrawingCtx.ctx().save();
        }
        return _MyDrawingCtx;
    }

    this.GetScale = function() { return _Scale; }
    this.SetScale = function(s) {
        _Scale = s;

        for (var i in _Children) {
            _Children[i].SetScale(s);
        }
    }

    this.GetParent = function() {
        return _Parent;
    };
    this.GetChildren = function() {
        return _Children;
    }

    this.TheCanvas = function() {
        return _Parent.TheCanvas();
    }

    this.GetType = function() { return ''; }

    // Get pos on the canvas, not relative pos
    this.GetPos = function() {
        var parPos = { X: 0, Y: 0 };
        if (_Parent != null && _Parent.GetPos) {
            parPos = _Parent.GetPos();
        }

        return { X: _Dim.X + parPos.X, Y: _Dim.Y + parPos.Y };
    };

    this.SetX = function(x) { _Dim.X = x; };
    this.SetY = function(y) { _Dim.Y = y; };
    this.SetW = function(w) {
        if (_Dim.W != w)
            this.Invalidate();
        _Dim.W = w;
    };
    this.SetH = function(h) {
        if (_Dim.H != h)
            this.Invalidate();
        _Dim.H = h;
    };
    this.SetDim = function(dim) {
        this.SetX(dim.X);
        this.SetY(dim.Y);
        this.SetW(dim.W);
        this.SetH(dim.H);
    };
    this.GetX = function() { return _Dim.X; };
    this.GetY = function() { return _Dim.Y; };
    this.GetW = function() { return _Dim.W; };
    this.GetH = function() { return _Dim.H; };
    this.GetDim = function() { return _Dim; }

    this.GetCenter = function() {
        var d = this.GetDim();
        return new Pt(d.X + (d.W / 2), d.Y + (d.H / 2));
    }

    this.Invalidate = function() {
        /// <summary>Flag this object to be redrawn</summary>
        _Invalid = true;
    }

    this.ChildrenChanged = function() { };

    this.Draw = function(ctx) {
        /// <summary>Pass a drawing context onto which this should be drawn</summary>
        /// <param name="ctx">DrawingContext to write to</param>
        //var dim = this.GetDim();
        //if (_Invalid) {
        // ensure buffer width
        //_EnsureBuffSize();

        // clear previously drawn crap
        //_GetCtx().ctx().clearRect(0, 0, dim.W, dim.H);

        // draw self to new canvas
        this.MyDraw(ctx);

        //_Invalid = false;
        //}

        // draw children
        var children = this.GetChildren();
        for (var i in children) {
            children[i].Draw(ctx);
        }

        // copy buffer to passed context
        //ctx.ctx().save();
        //ctx.ctx().scale(1 / _Scale, 1 / _Scale)
        //ctx.DrawImage(_GetBuffer(), { X: dim.X * _Scale, Y: dim.Y * _Scale, W: dim.W * _Scale, H: dim.H * _Scale });
        //ctx.ctx().restore();
        //ctx.DrawImage(_GetBuffer(), dim);
    }
    // override this to draw
    this.MyDraw = function(ctx) {
        /// <summary>Override this in inheriting classes draw self</summary>
    }

    this.MouseDown = function(e) { this.MyMouseDown(e); this.CallEvent('MouseDown', e); }
    this.MouseUp = function(e) { this.MyMouseUp(e); this.CallEvent('MouseUp', e); }
    this.MouseMove = function(e) { this.MyMouseMove(e); this.CallEvent('MouseMove', e); }
    this.SizeChanged = function() { this.MySizeChanged(); this.CallEvent('SizeChanged', {}) }

    this.MyMouseDown = function(e) { };
    this.MyMouseUp = function(e) { };
    this.MyMouseMove = function(e) { };
    this.MySizeChanged = function() { };

    var _Handlers = {};
    this.AddHandler = function(event, fn) {
        if (!_Handlers[event]) {
            _Handlers[event] = [];
        }
        _Handlers[event].push(fn);
    }
    this.RemoveHandler = function(event, fn) {
        _Handlers[event].splice(_Handlers[event].indexOf(fn), 1);
    }
    this.CallEvent = function(event, args) {
        if (_Handlers[event]) {
            for (var i in _Handlers[event]) {
                _Handlers[event][i](args);
            }
        }
    }

    // add every object that collides with the point into the collection
    // add this object first, then its children
    this.AccumCollisions = function(pt, coll) {
        var dim = this.GetDim();
        var collides = pt.X > dim.X && pt.X < dim.X + dim.W
                    && pt.Y > dim.Y && pt.Y < dim.Y + dim.H;

        if (collides) {
            coll.push(this);
            for (var i in _Children) {
                _Children[i].AccumCollisions({ X: pt.X - _Dim.X, Y: pt.Y - _Dim.Y }, coll);
            }
        }
    }
}

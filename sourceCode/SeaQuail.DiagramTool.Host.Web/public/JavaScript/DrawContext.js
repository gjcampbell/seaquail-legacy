/**************************************************************
* Enumerations
*
* These are the string settings used by the canvas
* drawing context
***************************************************************/
var LineCaps = {
    Butt: 'butt',
    Round: 'round',
    Square: 'square'
}

var LineJoins = {
    Round: 'round',
    Bevel: 'bevel',
    Miter: 'miter'
}

var TextAligns = {
    Start: 'start',
    End: 'end',
    Left: 'left',
    Right: 'right',
    Center: 'center'
}

var TextBaselines = {
    Top: 'top',
    Hanging: 'hanging',
    Middle: 'middle',
    Alphabetic: 'alphabetic',
    Ideographic: 'ideographic',
    Start: 'start'
}

var DrawModes = {
    Performance: 1,
    Normal: 2
}

function Pt(x, y) {
    this.X = x;
    this.Y = y;
    this.InRect = function(rect) {
        return this.X >= rect.X && this.X <= rect.X + rect.W
            && this.Y >= rect.Y && this.Y <= rect.Y + rect.H;
    }
    this.Distance = function(pt) {
        return Math.sqrt(Math.pow(pt.X - this.X, 2) + Math.pow(pt.Y - this.Y, 2));
    }
}
Pt.prototype.constructor = Pt;
Pt.prototype.Equal = function(pt) {
    return this.X == pt.X && this.Y == pt.Y;
}

function Rect(x, y, w, h) {
    if (x.X)
        h = x.H, w = x.W, y = x.Y, x = x.X
    this.X = x;
    this.Y = y;
    this.W = w;
    this.H = h;
}
Rect.prototype.constructor = Rect;
Rect.prototype.GetPoints = function() {
    return [
        new Pt(this.X, this.Y),
        new Pt(this.X, this.Y + this.H),
        new Pt(this.X + this.W, this.Y + this.H),
        new Pt(this.X + this.W, this.Y)
    ];
}
Rect.prototype.InsidePts = function(rect) {
    var myPoints = this.GetPoints();
    var insidePts = [];
    for (var i in myPoints)
        if (myPoints[i].InRect(rect))
        insidePts.push(myPoints[i]);

    return insidePts;
}
Rect.prototype.Crosses = function(rect) {
    // does the passed rect cross horizontally over this rect
    return rect.X < this.X && rect.X + rect.W > this.X + this.W
        && rect.Y > this.Y && rect.Y + rect.H < this.Y + this.H;
}
Rect.prototype.Overlaps = function(rect) {
    // does this or the passed rect cross the other
    //return this.Crosses(rect) || rect.Crosses(this);
    if (this.X > rect.X + rect.W || rect.X > this.X > this.W) {
        return false;
    }

    if (this.Y > rect.Y + rect.H || rect.Y > this.Y + this.H) {
        return false;
    }

    return true;
}
Rect.prototype.IntersectPts = function(rect) {
    // TODO: finish
    var insidePts = this.InsidePts(rect).concat(rect.InsidePts(this));
    if (insidePts.length == 0) {

    }
}
Rect.prototype.GetCenter = function() {
    return new Pt(this.X + (this.W / 2), this.Y + (this.H / 2));
}

/**************************************************************
* Pen
*
* Wrapper for the settings related to line drawing
***************************************************************/
Pen.prototype.constructor = Pen;
function Pen(color, width, cap, join, miter) {
    this.LineWidth = width ? width : 1;
    this.LineCap = cap ? cap : LineCaps.Butt;
    this.LineJoin = join ? join : LineJoins.Miter;
    this.MiterLimit = miter ? miter : 10;
    this.Color = color ? color : '#000';
}

/**************************************************************
* Glyph
*
* Wrapper for the settings related to text writing
***************************************************************/
function Glyph(font, align, baseline, width, wrap) {
    this.Font = font;
    this.Align = align ? align : TextAligns.Start;
    this.Baseline = baseline ? baseline : TextBaselines.Alphabetic;
    this.Width = width ? width : null;
    // not implemented yet
    this.Wrap = wrap ? true : false;
}
Glyph.prototype.constructor = Glyph;

/**************************************************************
* ColorStop
*
* Unit used in creating a Gradient
***************************************************************/
function ColorStop(offset, color) {
    this.Offset = offset;
    this.Color = color;
}
ColorStop.prototype.constructor = ColorStop;

/**************************************************************
* LinearGradient
*
* Unit used in creating a Gradient
***************************************************************/
function LinearGrad(x1, y1, x2, y2, stops) {
    this.X1 = x1;
    this.Y1 = y1;
    this.X2 = x2;
    this.Y2 = y2;
    this.ColorStops = stops ? stops : [];
}
LinearGrad.prototype.constructor = LinearGrad;

/**************************************************************
* DrawContext
*
* The drawing context is a wrapper for a 2d canvas drawing 
* context. It provides methods for drawing which do not
* require setting/unsetting context settings and allow quick
* style setup.
***************************************************************/
DrawContext.prototype.constructor = DrawContext;
function DrawContext(canvasEl) {
    var _Cvs = canvasEl;
    var _Ctx = _Cvs.getContext('2d');

    // these are the names of the drawing context style properties
    var Styles = {
        Stroke: 'strokeStyle',
        Fill: 'fillStyle',
        Font: 'font',
        Align: 'textAlign',
        Baseline: 'textBaseline',
        LineWidth: 'lineWidth',
        LineCap: 'lineCap',
        LineJoin: 'lineJoin',
        MiterLimit: 'miterLimit',
        ShadowColor: 'shadowColor',
        ShadowBlur: 'shadowBlur'
    };

    /* Style Preservation
    ********************************************************/
    // default styles are stored here
    var _Style = {};

    var _HoldStyle = function(styleName, newValue) {
        if (newValue) {
            _Style[styleName] = _Ctx[styleName];
            _Ctx[styleName] = newValue;
        }
    }
    var _RestoreStyle = function(styleName) {
        _Ctx[styleName] = _Style[styleName];
    }
    var _HoldPen = function(pen) {
        if (pen) {
            _HoldStyle(Styles.LineWidth, pen.LineWidth);
            _HoldStyle(Styles.LineCap, pen.LineCap);
            _HoldStyle(Styles.LineJoin, pen.LineJoin);
            _HoldStyle(Styles.MiterLimit, pen.MiterLimit);
            _HoldStyle(Styles.Stroke, pen.Color);
        }
    }
    var _RestorePen = function() {
        _RestoreStyle(Styles.LineWidth);
        _RestoreStyle(Styles.LineCap);
        _RestoreStyle(Styles.LineJoin);
        _RestoreStyle(Styles.MiterLimit);
        _RestoreStyle(Styles.Stroke);
    }
    var _HoldGlyph = function(glyph) {
        if (glyph) {
            _HoldStyle(Styles.Font, glyph.Font);
            _HoldStyle(Styles.Align, glyph.Align);
            _HoldStyle(Styles.Baseline, glyph.Baseline);
        }
    }
    var _RestoreGlyph = function() {
        _RestoreStyle(Styles.Font);
        _RestoreStyle(Styles.Align);
        _RestoreStyle(Styles.Baseline);
    }

    var _DrawMode = DrawModes.Normal;

    /* Execute context calls
    ********************************************************/
    var _DoRect = function(rect, actionName) {
        _Ctx[actionName](rect.X, rect.Y, rect.W, rect.H);
    },
    _DoEllipse = function(rect, actionName) {
        _Ctx.beginPath();

        var kappa = .5522848,
            x = rect.X, y = rect.Y, h = rect.H, w = rect.W,
            ox = (w / 2) * kappa,
            oy = (h / 2) * kappa,
            xe = x + w,
            ye = y + h,
            xm = x + w / 2,
            ym = y + h / 2;

        _Ctx.moveTo(x, ym);
        _Ctx.bezierCurveTo(x, ym - oy, xm - ox, y, xm, y);
        _Ctx.bezierCurveTo(xm + ox, y, xe, ym - oy, xe, ym);
        _Ctx.bezierCurveTo(xe, ym + oy, xm + ox, ye, xm, ye);
        _Ctx.bezierCurveTo(xm - ox, ye, x, ym + oy, x, ym);

        _Ctx.closePath();
        _Ctx[actionName]();
    },
    _DoText = function(text, pt, actionName, glyph) {
        _HoldGlyph(glyph);
        /* 2012-02-25 GC Removed due to performance
        if (glyph.Width != null) {
            var widths = {};
            var newText = '';
            var w = 0;
            for (var i = 0; i < text.length; i++) {
                var t = text[i];
                w += widths[t] ? widths[t] : widths[t] = _Ctx.measureText(t).width;
                if (w > glyph.Width)
                    break;
                newText += t;
            }
            text = newText;
        }*/
        _Ctx[actionName](text, pt.X, pt.Y);
        _RestoreGlyph();
    };


    /* Public Methods
    ********************************************************/

    // this provides access to the underlying drawing context
    this.ctx = function() { return _Ctx; };

    this.DrawRect = function(rect, pen) {
        _HoldPen(pen);
        _DoRect(rect, 'strokeRect');
        _RestorePen();
    }
    this.FillRect = function(rect, fillStyle) {
        _HoldStyle(Styles.Fill, fillStyle);
        _DoRect(rect, 'fillRect');
        _RestoreStyle(Styles.Fill);
    }

    this.DrawEllipse = function(rect, pen) {
        _HoldPen(pen);
        _DoEllipse(rect, 'stroke');
        _RestorePen();
    }
    this.FillEllipse = function(rect, fillStyle) {
        _HoldStyle(Styles.Fill, fillStyle);
        _DoEllipse(rect, 'fill');
        _RestoreStyle(Styles.Fill);
    }

    this.DrawText = function(text, pt, pen, glyph) {
        _HoldPen(pen);
        _DoText(text, pt, 'strokeText', glyph);
        _RestorePen();
    }
    this.FillText = function(text, pt, style, glyph) {
        _HoldStyle(Styles.Fill, style);
        _DoText(text, pt, 'fillText', glyph);
        _RestoreStyle(Styles.Fill);
    }
    this.MeasureText = function(text, glyph) {
        _HoldGlyph(glyph);
        var res = _Ctx.measureText(text).width;
        _RestoreGlyph();

        return res;
    }

    this.RoundedRect = function(rect, radius) {
        var x = rect.X;
        var y = rect.Y;
        var height = rect.H;
        var width = rect.W;
        _Ctx.beginPath();
        _Ctx.moveTo(x, y + radius);
        _Ctx.lineTo(x, y + height - radius);
        _Ctx.quadraticCurveTo(x, y + height, x + radius, y + height);
        _Ctx.lineTo(x + width - radius, y + height);
        _Ctx.quadraticCurveTo(x + width, y + height, x + width, y + height - radius);
        _Ctx.lineTo(x + width, y + radius);
        _Ctx.quadraticCurveTo(x + width, y, x + width - radius, y);
        _Ctx.lineTo(x + radius, y);
        _Ctx.quadraticCurveTo(x, y, x, y + radius);
        _Ctx.closePath();
    }

    this.DrawRoundedRect = function(rect, radius, pen) {
        _HoldPen(pen);
        this.RoundedRect(rect, radius);
        _Ctx.stroke();
        _RestorePen();
    }

    this.FillRoundedRect = function(rect, radius, fillStyle) {
        _HoldStyle(Styles.Fill, fillStyle);
        this.RoundedRect(rect, radius);
        _Ctx.fill();
        _RestoreStyle(Styles.Fill);
    }

    this.DrawImage = function(img, box) {
        _Ctx.drawImage(img, box.X, box.Y, box.W, box.H);
    }

    this.DrawPath = function(pen) {
        _HoldPen(pen);
        _Ctx.stroke();
        _RestorePen();
    }

    this.FillPath = function(style) {
        _HoldStyle(Styles.Fill, style);
        _Ctx.fill();
        _RestoreStyle(Styles.Fill);
    }

    this.Highlight = function(mode) {
        _Ctx.shadowBlur = mode ? 10 : 0;
        _Ctx.shadowColor = mode ? 'Orange' : null;
    }

    this.SetDrawMode = function(mode) {
        _DrawMode = mode;
    }
    this.GetDrawMode = function() {
        return _DrawMode;
    }

    this.CreateLinearGrad = function(linearGrad) {
        var res = _Ctx.createLinearGradient(linearGrad.X1, linearGrad.Y1, linearGrad.X2, linearGrad.Y2);
        for (var i in linearGrad.ColorStops) {
            var cs = linearGrad.ColorStops[i];
            res.addColorStop(cs.Offset, cs.Color);
        }

        return res;
    }
}
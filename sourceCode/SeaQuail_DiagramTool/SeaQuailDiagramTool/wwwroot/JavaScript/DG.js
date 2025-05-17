/// <reference path="DrawContext.js" />
/// <reference path="DrawnObject.js" />
/// <reference path="ContextMenu.js" />

function S4() {
  return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}
function guid() {
  return (
    S4() +
    S4() +
    "-" +
    S4() +
    "-" +
    S4() +
    "-" +
    S4() +
    "-" +
    S4() +
    S4() +
    S4()
  );
}

var GroupColors = [
  // Blues
  { F: "#2B4C71", B: "#DCE6F1" },
  { F: "#2B4C71", B: "#B8CCE4" },
  // Reds
  { F: "#732929", B: "#F2DCDB" },
  { F: "#732929", B: "#E6B8B7" },
  // Greens
  { F: "#5D702E", B: "#EBF1DE" },
  { F: "#5D702E", B: "#D8E4BC" },
  // Violets
  { F: "#48395D", B: "#E4DFEC" },
  { F: "#48395D", B: "#CCC0DA" },
  // Cyans
  { F: "#1F5461", B: "#DAEEF3" },
  { F: "#1F5461", B: "#B7DEE8" },
  // Oranges
  { F: "#7C3806", B: "#FDE9D9" },
  { F: "#7C3806", B: "#FCD5B4" },
];

var Moves = {
  Up: "up",
  Down: "down",
  Top: "top",
  Bottom: "bottom",
};

var SSMSIcons = {
  ColumnAdd: function () {
    return this.GetURL("column_add");
  },
  ColumnInsert: function () {
    return this.GetURL("column_insert");
  },
  ColumnDelete: function () {
    return this.GetURL("column_delete");
  },
  GetURL: function (ssmsicon) {
    return "/Media/Images/Icons/SSMS/" + ssmsicon + ".png";
  },
};
var FFFIcons = {
  Relationship: function () {
    return this.GetURL("table_relationship");
  },
  GetURL: function (ssmsicon) {
    return "/Media/Images/Icons/" + ssmsicon + ".png";
  },
};

var ZoomLevels = [0.25, 0.33, 0.5, 0.75, 0.87, 1, 1.25, 1.5, 2];

var _DataTypes = {
  Boolean: {
    Name: "Boolean",
    HasLength: false,
    HasPrecision: false,
    IDOK: false,
    PKOK: false,
    FKOK: false,
  },
  SmallInt: {
    Name: "SmallInt",
    HasLength: false,
    HasPrecision: false,
    IDOK: true,
    PKOK: true,
    FKOK: true,
  },
  Int: {
    Name: "Int",
    HasLength: false,
    HasPrecision: false,
    IDOK: true,
    PKOK: true,
    FKOK: true,
  },
  BigInt: {
    Name: "BigInt",
    HasLength: false,
    HasPrecision: false,
    IDOK: true,
    PKOK: true,
    FKOK: true,
  },
  Decimal: {
    Name: "Decimal",
    HasLength: false,
    HasPrecision: true,
    IDOK: false,
    PKOK: true,
    FKOK: true,
  },
  DateTime: {
    Name: "DateTime",
    HasLength: false,
    HasPrecision: false,
    IDOK: false,
    PKOK: false,
    FKOK: false,
  },
  String: {
    Name: "String",
    HasLength: true,
    HasPrecision: false,
    IDOK: false,
    PKOK: true,
    FKOK: true,
  },
  Bytes: {
    Name: "Bytes",
    HasLength: true,
    HasPrecision: false,
    IDOK: false,
    PKOK: false,
    FKOK: false,
  },
  GetDataTypeName: function (col) {
    return (
      col.DataType.Name +
      (col.DataType.HasPrecision
        ? "(" + col.Precision + ", " + col.Scale + ")"
        : col.DataType.HasLength
        ? "(" + (col.Length == 0 ? "MAX" : col.Length) + ")"
        : "")
    );
  },
  Validation: new RegExp(
    /^Boolean$|^SmallInt$|^Int$|^BigInt$|^Decimal\(\d+,[ ]?\d+\)$|^DateTime$|^(String|Bytes)\((\d+|max)\)$/i
  ),
  ValidateDataTypeName: function (name) {
    return this.Validation.test(name);
  },
  SetColumnSettings: function (datatypeText, column) {
    var datatypeTextParts = datatypeText.replace(")", "").split("(");
    var typeName = datatypeTextParts[0].toLowerCase();
    for (var i in _DataTypes) {
      var type = _DataTypes[i];
      if (type.Name && type.Name.toLowerCase() == typeName) {
        column.DataType = type;
        if (type.HasLength && datatypeTextParts[1]) {
          column.Length =
            datatypeTextParts[1].toLowerCase() == "max"
              ? 0
              : parseInt(datatypeTextParts[1]);
        } else if (type.HasPrecision && datatypeTextParts[1]) {
          var precParts = datatypeTextParts[1].replace(" ", "").split(",");
          column.Precision = parseInt(precParts[0]);
          column.Scale = parseInt(precParts[1]);
        }
      }
    }
  },
};

var CanvasTextBox = {
  TextBox: null,
  Close: null,
  Show: function (text, rect, font, onblur, keydown, keypress, keyup) {
    if (this.TextBox != null) {
      this.Close();
    }
    var txt = $('<input type="textbox" class="canvas-TextBox" />');
    var origText = text;
    txt.css({
      width: rect.W + "px",
      height: rect.H + "px",
      left: rect.X + "px",
      top: rect.Y + "px",
    });
    txt.css(font);
    txt.val(text);

    $("body").append(txt);

    var close = function () {
      onblur(txt);
      txt.remove();
      CanvasTextBox.TextBox = null;
    };
    this.Close = close;

    txt.focus(function () {
      txt.blur(close);
    });
    txt.keyup(function (e) {
      if (e.keyCode == 13) {
        close();
      } else if (e.keyCode == 27) {
        // escape key, cancel text changes
        txt.val(origText);
        close();
      }
    });

    txt.keydown(keydown).keypress(keypress).keyup(keyup);

    this.TextBox = txt;
    txt.focus();
  },
};

DGDiagramObject.prototype = new DrawnObject();
DGDiagramObject.prototype.constructor = DGDiagramObject;
function DGDiagramObject(parent) {
  DrawnObject.call(this, parent);

  this.DG = function () {
    return this.GetParent().DG();
  };
}

/****************************************************************
 * Table
 ***************************************************************/
DGTable.prototype = new DGDiagramObject();
DGTable.prototype.constructor = DGTable;
function DGTable(parent) {
  DGDiagramObject.call(this, parent);
  this.GetType = function () {
    return "DGTable";
  };
  var _Parent = parent,
    self = this,
    _Highlight = false;

  var _Guid = guid();

  var dragging = null;

  var dg_MouseMove = function (e) {
    if (dragging != null) {
      var newX = dragging.MyStartPos.X + (e.Pos.X - dragging.MseStartPos.X);
      var newY = dragging.MyStartPos.Y + (e.Pos.Y - dragging.MseStartPos.Y);
      self.SetX(newX);
      self.SetY(newY);
      self.DG().SetDragMode(true);
      self.DG().UpdateTableGroups(null, [self]);
    }

    if (!_MouseOver) {
      self.TheCanvas().style.cursor = Cursors.Default;
    }
    _MouseOver = false;
  };

  var _MouseOver = false;
  var _ResizeMode = null;

  this.MyMouseMove = function (e) {
    _MouseOver = true;

    //        // resizing
    //        if (_ResizeMode == null) {
    //            var pt = new Pt(e.Pos.X - this.GetX(), e.Pos.Y - this.GetY());

    //            var dim = this.GetDim();
    //            if (pt.InRect(new Rect(dim.W - 7, dim.H - 7, 7, 7))) {
    //                this.TheCanvas().style.cursor = Cursors.ResizeNW;
    //            } else if (pt.InRect(new Rect(dim.W - 7, 0, 7, dim.H))) {
    //                this.TheCanvas().style.cursor = Cursors.ResizeE;
    //            } else if (pt.InRect(new Rect(0, dim.H - 7, dim.W, 7))) {
    //                this.TheCanvas().style.cursor = Cursors.ResizeN;
    //            } else {
    //                this.TheCanvas().style.cursor = Cursors.Default;
    //            }
    //        }
  };

  this.MyMouseUp = function (e) {
    dragging = null;
    self.DG().SetDragMode(false);
  };

  this.Select = function () {
    this.Invalidate();
    _Parent.SelectedTable = this;
  };
  this.Deselect = function () {
    this.Invalidate();
    this.SelectedColumn = null;
    this.DG().UpdateTableGroups();
    _ColList.Invalidate();
  };

  this.MyMouseDown = function (e) {
    if (e.Button == MseButtons.Left) {
      dragging = {
        MyStartPos: { X: this.GetX(), Y: this.GetY() },
        MseStartPos: e.Pos,
      };
      e.Cancel = true;
      this.Select();
    }
  };

  this.GetHighlight = function () {
    return _Highlight;
  };
  this.SetHighlight = function (on) {
    _Highlight = on;
  };
  this.HasHighlights = function () {
    return _Highlight || this.GetColList().GetHighlights().length > 0;
  };

  this.Group = null;

  _Parent.AddHandler("MouseMove", dg_MouseMove);

  this.MyDraw = function (ctx) {
    var dim = this.GetDim();
    updateColListSize();
    var fillColor = this.Group ? this.Group.GetColor().B : "#efefef",
      penColor = this.Group ? this.Group.GetColor().F : "#404040";
    fillColor =
      this.DG().SelectedTable != this
        ? fillColor
        : this.Group
        ? penColor
        : "#1C4B7F";

    //ctx.FillRect(dim, fillColor);
    //ctx.DrawRect(dim, new Pen('#404040', 1));
    ctx.FillRoundedRect(new Rect(dim.X, dim.Y, dim.W, 31), 7, fillColor);
    if (_Highlight) ctx.Highlight(true);
    ctx.DrawRoundedRect(
      new Rect(dim.X, dim.Y, dim.W, 31),
      7,
      new Pen(penColor, 1)
    );
    ctx.Highlight(false);
    ctx.FillText(
      _Name,
      { X: dim.X + 12, Y: dim.Y + 20 },
      _Parent.SelectedTable == this
        ? "#ffffff"
        : this.Group
        ? penColor
        : "#000000",
      new Glyph("bold 9pt Arial", null, null, dim.W - 12)
    );
  };

  var _Name = "";
  var _Columns = [
    {
      Name: "",
      DataType: _DataTypes.String,
      Length: 250,
      Nullable: true,
      GUID: guid(),
    },
  ];

  this.SetName = function (n) {
    _Name = n;
  };
  this.GetName = function () {
    return _Name;
  };
  this.GetGUID = function () {
    return _Guid;
  };
  this.SetGUID = function (guid) {
    _Guid = guid;
  };
  this.GetColumns = function () {
    return _Columns;
  };
  this.GetColList = function () {
    return _ColList;
  };

  this.GetColumnByGUID = function (guid) {
    for (var i in _Columns) {
      if (_Columns[i].GUID == guid) {
        return _Columns[i];
      }
    }

    return null;
  };

  this.Delete = function () {
    if (this.DG().SelectedTable == this) this.DG().SelectedTable = null;

    _Parent.GetChildren().splice(_Parent.GetChildren().indexOf(this), 1);
    _Parent.RemoveHandler("MouseMove", dg_MouseMove);
  };

  var children = this.GetChildren();

  var _ColList = new DGColList(this);
  _ColList.SetX(7);
  _ColList.SetY(32);

  var updateColListSize = function () {
    var dim = self.GetDim();
    _ColList.SetW(dim.W - 14);
    _ColList.SetH(dim.H - 37);
  };
  // need size change event
  updateColListSize();
}

/****************************************************************
 * Column List
 ***************************************************************/
DGColList.prototype = new DGDiagramObject();
DGColList.prototype.constructor = DGColList;
function DGColList(parent) {
  DGDiagramObject.call(this, parent);
  this.GetType = function () {
    return "DGColList";
  };
  var _Parent = parent,
    self = this,
    _initialDraw = true;

  var _Columns = function () {
    return _Parent.GetColumns();
  };

  var _RowHeight = 24,
    _BorderPen = new Pen("#cfcfcf", 0.5),
    _CellGlyph = new Glyph("8pt Arial", null, TextBaselines.Middle),
    _HeaderCellGlyph = new Glyph(
      "8pt Arial",
      TextAligns.Center,
      TextBaselines.Middle
    ),
    _CellGrad = null,
    _HighlightIndexes = [];

  // Record the down position to detect full up-down click
  var _DownPos = null;

  var _ColStyles = null;

  var _CreateColumn = function () {
    return {
      Name: "",
      DataType: _DataTypes.String,
      Length: 250,
      Nullable: true,
      GUID: guid(),
    };
  };

  var _CreateRowHighlight = function (ctx, x, y) {
    return ctx.CreateLinearGrad(
      new LinearGrad(x, y, x, y + _RowHeight, [
        new ColorStop(0, "#729844"),
        new ColorStop(0.2, "#BAD698"),
        new ColorStop(0.8, "#BAD698"),
        new ColorStop(1, "#CAE1AF"),
      ])
    );
  };
  var _CreateRowSelHighlight = function (ctx, x, y) {
    return ctx.CreateLinearGrad(
      new LinearGrad(x, y, x, y + _RowHeight, [
        new ColorStop(0, "#4C6E90"),
        new ColorStop(0.2, "#9DB7D1"),
        new ColorStop(0.8, "#9DB7D1"),
        new ColorStop(1, "#B3C8DD"),
      ])
    );
  };

  var _DrawRow = function (ctx, data, rowidx, x, y) {
    var overrideBG = null;
    if (self.DG().GetCurrentFK() != null) {
      var thecol = _Columns()[rowidx];
      var fromcol = self.DG().GetCurrentFK().GetFromColumn();
      overrideBG =
        thecol.DataType == fromcol.DataType && thecol != fromcol
          ? _CreateRowHighlight(ctx, x, y)
          : "#F1F2F4";
    }
    for (var i in _ColStyles) {
      var col = _ColStyles[i];
      var rect = new Rect(x, y, col.Width, _RowHeight);
      ctx.FillRect(
        rect,
        i == 0
          ? col.BG
          : _Parent.SelectedColumn == rowidx
          ? _CreateRowSelHighlight(ctx, x, y)
          : overrideBG
          ? overrideBG
          : col.BG
      );
      if (i == 1 && _HighlightIndexes.indexOf(rowidx) > -1) ctx.Highlight(true);
      ctx.DrawRect(rect, _BorderPen);
      ctx.Highlight(false);
      _CellGlyph.Align = col.Align;
      var textX =
        _CellGlyph.Align == TextAligns.Left
          ? x + 3
          : _CellGlyph.Align == TextAligns.Center
          ? x + col.Width / 2
          : col.Width - 3 + x;
      _CellGlyph.Width = col.Width - 3;
      ctx.FillText(
        data[i],
        new Pt(textX, y + _RowHeight / 2),
        "#000000",
        _CellGlyph
      );

      x += col.Width;
    }
  };

  var _DrawHeader = function (ctx, x, y) {
    for (var i in _ColStyles) {
      var col = _ColStyles[i];
      var rect = new Rect(x, y, col.Width, _RowHeight);
      ctx.FillRect(rect, _CellGrad);
      ctx.DrawRect(rect, _BorderPen);
      var textX = x + col.Width / 2;
      ctx.FillText(
        col.Name,
        new Pt(textX, y + _RowHeight / 2),
        "#000000",
        _HeaderCellGlyph
      );

      x += col.Width;
    }
  };

  var EditTypes = { Text: "text", Toggle: "toggle" };

  this.MyDraw = function (ctx) {
    if (
      ctx.GetDrawMode() == DrawModes.Performance &&
      !_initialDraw &&
      new Rect(this.GetParent().GetDim()).InsidePts(this.DG().GetView())
        .length < 1
    )
      return;

    _initialDraw = false;

    var pos = this.GetPos();

    // initialize the gradient used for the header row
    _CellGrad = ctx.CreateLinearGrad(
      new LinearGrad(pos.X, pos.Y, pos.X, pos.Y + _RowHeight, [
        new ColorStop(0, "#ffffff"),
        new ColorStop(0.4, "#ffffff"),
        new ColorStop(0.41, "#F7F8FA"),
        new ColorStop(1, "#F1F2F4"),
      ])
    );
    // initialize the column styles
    if (_ColStyles == null) {
      _ColStyles = [
        {
          Width: 24,
          Name: "",
          GetValue: function (col) {
            return "";
          },
          SetValue: function (col, val) {},
          BG: "#F1F2F4",
          Align: TextAligns.Center,
        },
        {
          Width: 160,
          Name: "Name",
          GetValue: function (col) {
            return col.Name;
          },
          SetValue: function (col, val) {
            col.Name = val;
          },
          BG: "#ffffff",
          Align: TextAligns.Left,
          EditType: EditTypes.Text,
        },
        {
          Width: 80,
          Name: "Type",
          GetValue: function (col) {
            return _DataTypes.GetDataTypeName(col);
          },
          SetValue: function (col, val) {
            if (_DataTypes.ValidateDataTypeName(val)) {
              _DataTypes.SetColumnSettings(val, col);
            }
          },
          BG: "#ffffff",
          Align: TextAligns.Left,
          EditType: EditTypes.Text,
        },
        {
          Width: 52,
          Name: "Nullable",
          GetValue: function (col) {
            return col.Nullable ? "Yes" : "No";
          },
          SetValue: function (col, val) {},
          BG: "#ffffff",
          Align: TextAligns.Center,
          EditType: EditTypes.Toggle,
        },
      ];
    }

    var dim = new Rect(pos.X, pos.Y, this.GetW(), this.GetH() - 0.5);
    ctx.FillRect(dim, "#ffffff");
    ctx.DrawRect(dim, new Pen("#000000", 0.5));

    var rowY = pos.Y;
    _DrawHeader(ctx, pos.X + 0.5, rowY + 0.5);
    rowY += _RowHeight;
    var cols = _Columns();
    for (var i = 0; i < cols.length; i++) {
      var col = cols[i];
      var columnText = [];
      for (var ii = 0; ii < _ColStyles.length; ii++) {
        if (i == _EditIndex.Row && ii == _EditIndex.Col) columnText.push("");
        else columnText.push(_ColStyles[ii].GetValue(col));
      }

      _DrawRow(ctx, columnText, i, pos.X + 0.5, rowY + 0.5);
      rowY += _RowHeight;
    }

    _Parent.SetH(
      _Parent.GetH() - this.GetH() + _RowHeight * (_Columns().length + 1) + 2
    );
    _Parent.SetW(
      _Parent.GetW() - this.GetW() + _SumColWidth(_ColStyles.length - 1) + 1
    );
  };

  this.Select = function (index) {
    _Parent.SelectedColumn = index;
  };
  this.Deselect = function () {
    _Parent.SelectedColumn = null;
    this.Invalidate();
  };

  this.SetHighlights = function (highlightIndexes) {
    _HighlightIndexes = highlightIndexes;
  };
  this.GetHighlights = function () {
    return _HighlightIndexes;
  };

  this.DeleteColumn = function (index) {
    _Columns().splice(index, 1);
  };

  this.GetRowHeight = function () {
    return _RowHeight;
  };

  _DG_MouseUp = function (e) {
    _DownPos = null;
  };

  this.DG().AddHandler("MouseUp", _DG_MouseUp);

  this.MyMouseDown = function (e) {
    if (e.Button == MseButtons.Left) {
      _DownPos = e.Pos;
      _Parent.Select();
    }
    e.Cancel = true;
    if (e.Button == MseButtons.Right) {
      _DownPos = e.Pos;
      e.DisableContextMenu();
      _Parent.Select();
    }
  };

  this.MyMouseMove = function (e) {
    if (_DownPos != null) {
      if (
        !_DownPos.Equal(e.Pos) &&
        _Columns().indexOf(this.GetColumnAtPoint(_DownPos)) >= 0 &&
        _DownPos.Distance(e.Pos) > 8 &&
        this.DG().GetCurrentFK() == null
      ) {
        _Parent.SelectedColumn = this.GetRowAtPoint(_DownPos);
        var col = _Columns()[_Parent.SelectedColumn];
        if (col.DataType.FKOK) this.DG().AddRelationship(_Parent, col);
      }
    }
  };

  var ContextMenu_Click = function (args) {
    if (args.Text == "Insert Column") {
      _Columns().splice(_Parent.SelectedColumn, 0, _CreateColumn());
    } else if (args.Text == "Delete Column") {
      _Columns().splice(_Parent.SelectedColumn, 1);
    } else if (args.Text == "Add Column") {
      _Columns().push(_CreateColumn());
    } else if (args.Text == "Set Foreign Key") {
      _Parent
        .GetParent()
        .AddRelationship(_Parent, _Columns()[_Parent.SelectedColumn]);
    } else if (args.Text == "Remove Foreign Key") {
      _Parent
        .GetParent()
        .RemoveRelationship(_Columns()[_Parent.SelectedColumn]);
    }
    _Parent.SelectedColumn = null;
    self.Invalidate();
  };

  var _SumColWidth = function (endIndex) {
    var res = 0;
    for (var i = 0; i <= endIndex; i++) {
      res += _ColStyles[i].Width;
    }

    return res;
  };

  var _EditIndex = { Row: -1, Col: -1 };

  var _GetNextEditCell = function (dir) {
    var startCol = parseInt(_EditIndex.Col) + dir;
    for (
      var row = _EditIndex.Row;
      row < _Columns().length && row >= 0;
      row += dir
    ) {
      for (
        var col = startCol;
        col < _ColStyles.length && col >= 0;
        col += dir
      ) {
        if (_ColStyles[col].EditType == EditTypes.Text) {
          return { Row: row, Col: col };
        }
      }
      startCol = dir < 0 ? _ColStyles.length - 1 : 0;
    }

    return null;
  };

  var _EditColumnText = function (colIndex, rowIndex, suggest) {
    var rowIndex = rowIndex;
    var colIndex = colIndex;
    var pos = self.GetPos();
    var sugg = suggest;
    var colStyle = _ColStyles[colIndex];
    var textX = _SumColWidth(colIndex) - colStyle.Width;
    var column = _Columns()[rowIndex];

    var dg = _Parent.GetParent();
    var zoom = _Parent.GetParent().GetZoom();

    // TODO: need to adjust position for firefox
    CanvasTextBox.Show(
      // text
      colStyle.GetValue(column),
      // position
      new Rect(
        pos.X + textX + 1 + dg.GetOffsetX(),
        pos.Y +
          (rowIndex + 1) * _RowHeight +
          1 +
          dg.GetOffsetY() +
          $(self.DG().TheCanvas()).offset().top,
        colStyle.Width,
        _RowHeight
      ),
      // styles
      { zoom: zoom, "-moz-transform": "scale(" + zoom + ")" },
      // on textbox close event
      function (txt) {
        _EditIndex = { Row: -1, Col: -1 };
        colStyle.SetValue(column, txt.val());
        self.Invalidate();
      },
      // on textbox keydown event
      function (e) {
        if (e.keyCode == 38) {
          if (rowIndex - 1 >= 0) {
            // up pressed
            _EditColumnText(colIndex, --rowIndex, sugg);
          }
        } else if (e.keyCode == 40) {
          // down pressed
          if (rowIndex + 1 < _Columns().length) {
            _EditColumnText(colIndex, ++rowIndex, sugg);
          }
        } else if (e.keyCode == 9) {
          // tab key
          var nextEditCell = _GetNextEditCell(e.shiftKey ? -1 : 1);

          if (nextEditCell != null) {
            _EditColumnText(nextEditCell.Col, nextEditCell.Row, sugg);
          }

          // if tab was pressed on the last column, create a new column
          // and focus on it
          if (!e.shiftKey && nextEditCell == null) {
            _Columns().push(_CreateColumn());
            var nextEditCell = _GetNextEditCell(1);
            if (nextEditCell != null) {
              _EditColumnText(nextEditCell.Col, nextEditCell.Row, sugg);
            }
          }

          return false;
        }
      },
      // on textbox keypress event
      function (e) {
        if (sugg) {
          sugg(e);
        }
      }
    );

    _EditIndex = { Row: rowIndex, Col: colIndex };
    self.Invalidate();
    self.Select(rowIndex);
  };

  this.GetColumnAtPoint = function (point) {
    var pos = this.GetPos();
    var pt = new Pt(point.X - pos.X, point.Y - pos.Y);

    var index = Math.floor(pt.Y / _RowHeight) - 1;
    if (_Columns().length > index && index >= 0) {
      return _Columns()[index];
    }

    return null;
  };

  this.GetRowAtPoint = function (point) {
    var pos = this.GetPos();
    var pt = new Pt(point.X - pos.X, point.Y - pos.Y);
    var index = Math.floor(pt.Y / _RowHeight) - 1;
    if (_Columns().length > index && index >= 0) {
      return index;
    }

    return null;
  };

  this.GetEditIndex = function () {
    return _EditIndex;
  };
  this.GetColStyles = function () {
    return _ColStyles;
  };

  this.MyMouseUp = function (e) {
    var fullClick =
      _DownPos != null && _DownPos.X == e.Pos.X && _DownPos.Y == e.Pos.Y;

    if (
      fullClick &&
      (e.Button == MseButtons.Left || e.Button == MseButtons.Right)
    ) {
      var pos = this.GetPos();
      var pt = new Pt(e.Pos.X - pos.X, e.Pos.Y - pos.Y);

      var index = Math.floor(pt.Y / _RowHeight) - 1;
      if (_Columns().length > index && index >= 0) {
        this.Select(index);
        this.Invalidate();
      } else {
        this.Deselect();
      }

      if (e.Button == MseButtons.Left) {
        var cellIdx = 0;

        // get selected column
        var cellX = 0;
        for (var i in _ColStyles) {
          cellX += _ColStyles[i].Width;
          if (pt.X < cellX) {
            cellIdx = i;
            break;
          }
        }

        var colStyle = _ColStyles[cellIdx];

        if (_Parent.SelectedColumn != null) {
          if (colStyle.Name == "Name") {
            _EditColumnText(cellIdx, index);
          } else if (colStyle.Name == "Type") {
            var column = _Columns()[index];
            _EditColumnText(
              cellIdx,
              index,
              // datatype text suggestion
              function (e) {}
            );
          } /* else if (colStyle.Name == 'Nullable') {
                        var column = _Columns()[index];
                        column.Nullable = !column.Nullable;
                    }*/ // todo: toggle nullable from click
        }
      } else if (e.Button == MseButtons.Right) {
        e.DisableContextMenu();
        var items = [];
        items.push(new MenuItem("plus", "Add Column"));
        if (_Parent.SelectedColumn != null) {
          items.push(new MenuItem("plus-square-o", "Insert Column"));
          items.push(new MenuItem("times", "Delete Column"));
          if (_Columns()[_Parent.SelectedColumn].DataType.FKOK) {
            items.push(new MenuItem("link", "Set Foreign Key"));
            if (
              _Parent
                .GetParent()
                .GetRelationshipByFromColumn(_Columns()[_Parent.SelectedColumn])
            ) {
              items.push(new MenuItem("unlink", "Remove Foreign Key"));
            }
          }
        }

        ContextMenu.Show(
          new Pt(e.Event.pageX, e.Event.pageY),
          items,
          ContextMenu_Click
        );
      }
      e.Cancel = true;
    }

    _DownPos = null;
  };
}

/****************************************************************
 * Table Group
 ***************************************************************/
DGGroup.prototype = new DGDiagramObject();
DGFKey.prototype.constructor = DGGroup;
function DGGroup(parent) {
  DGDiagramObject.call(this, parent);
  this.GetType = function () {
    return "DGGroup";
  };

  var _Parent = parent,
    self = this,
    _Color = GroupColors[0],
    _Label = "",
    _Glyph = new Glyph("9pt Arial", TextAligns.Center, TextBaselines.Middle),
    _Dragging = null,
    _DragHandleRadius = 5,
    _Handles = ["Right", "Top", "Left", "Bottom"],
    _MoveToTop = function () {
      self.DG().MoveChildTo(self, self.DG().GetLastGroupIndex());
    },
    _GetDetachHandlePt = function () {
      return new Pt(self.GetX() + self.GetW() / 2, self.GetY() + 40);
    },
    _PtInEllipse = function (pt) {
      var c = self.GetCenter(),
        rx = self.GetW() / 2,
        ry = self.GetH() / 2,
        dist =
          Math.pow(pt.X - c.X, 2) / Math.pow(rx, 2) +
          Math.pow(pt.Y - c.Y, 2) / Math.pow(ry, 2);
      return dist <= 1;
    },
    _GetDragHandlePts = function () {
      var d = self.GetDim(),
        dhr = _DragHandleRadius;
      return [
        new Pt(d.X + d.W - dhr, d.Y + d.H / 2),
        new Pt(d.X + d.W / 2, d.Y + dhr),
        new Pt(d.X + dhr, d.Y + d.H / 2),
        new Pt(d.X + d.W / 2, d.Y + d.H - dhr),
      ];
    },
    _StartDragging = function (pos, handle) {
      _Dragging = {
        Handle: handle,
        MyStartSize: { W: self.GetW(), H: self.GetH() },
        MyStartPos: { X: self.GetX(), Y: self.GetY() },
        MseStartPos: pos,
      };
      // if this is not a handle being dragged, drag tables in group
      if (handle == null) {
        if (_GetDetachHandlePt().Distance(pos) > 15) {
          var tables = self.DG().FindChildren(function (o) {
            return o.GetType() == "DGTable" && _PtInEllipse(o.GetCenter());
          });
          _Dragging.Tables = [];
          for (var i in tables) {
            _Dragging.Tables.push({
              Table: tables[i],
              StartPos: new Pt(tables[i].GetX(), tables[i].GetY()),
            });
          }
        }
      }
    },
    _StopDragging = function () {
      _Dragging = null;
    },
    dg_MouseMove = function (e) {
      if (_Dragging != null) {
        var deltX = e.Pos.X - _Dragging.MseStartPos.X,
          deltY = e.Pos.Y - _Dragging.MseStartPos.Y,
          newX = _Dragging.MyStartPos.X + deltX,
          newY = _Dragging.MyStartPos.Y + deltY;
        if (_Dragging.Handle == null) {
          self.SetX(newX);
          self.SetY(newY);
          for (var i in _Dragging.Tables) {
            var t = _Dragging.Tables[i];
            t.Table.SetX(t.StartPos.X + deltX);
            t.Table.SetY(t.StartPos.Y + deltY);
          }
        } else {
          switch (_Handles[_Dragging.Handle]) {
            case "Right":
              self.SetW(_Dragging.MyStartSize.W + (newX - self.GetX()));
              break;
            case "Left":
              self.SetW(
                _Dragging.MyStartSize.W - (e.Pos.X - _Dragging.MseStartPos.X)
              );
              self.SetX(newX);
              break;
            case "Top":
              self.SetH(
                _Dragging.MyStartSize.H - (e.Pos.Y - _Dragging.MseStartPos.Y)
              );
              self.SetY(newY);
              break;
            case "Bottom":
              self.SetH(_Dragging.MyStartSize.H + (newY - self.GetY()));
              break;
          }
        }
        self.DG().SetDragMode(true);

        self.DG().UpdateTableGroups([self]);
      }
    };

  _Parent.AddHandler("MouseMove", dg_MouseMove);
  _MoveToTop();

  this.Draw = function (ctx) {
    var children = this.DG().GetChildren(),
      dhr = _DragHandleRadius,
      r = new Rect(
        this.GetX() + dhr,
        this.GetY() + dhr,
        this.GetW() - 2 * dhr,
        this.GetH() - 2 * dhr
      );
    ctx.ctx().save();

    ctx.FillEllipse(r, _Color.B);

    // Draw detach handle
    if (this.DG().SelectedGroup == this) {
      var pt = _GetDetachHandlePt(),
        pen = new Pen(_Color.F, 2),
        rect = new Rect(pt.X - 15, pt.Y - 15, 30, 30);

      ctx.FillEllipse(rect, "#ffffff");
      ctx.DrawEllipse(rect, pen);
    }

    // Draw label
    if (_Label != null && _Label.length > 0) {
      var c = self.GetCenter(),
        h = 31,
        rad = 7,
        w = ctx.MeasureText(_Label, _Glyph) + 24,
        x = c.X - w / 2,
        y = c.Y - h / 2,
        rect = new Rect(x, y, w, h);

      ctx.FillRoundedRect(rect, rad, "#ffffff");
      ctx.DrawRoundedRect(rect, rad, new Pen(_Color.F, 1));
      ctx.FillText(_Label, new Pt(c.X, c.Y), _Color.F, _Glyph);
    }

    // Draw sizing handles
    ctx.DrawEllipse(r, new Pen(_Color.F, 1));
    if (this.DG().SelectedGroup == this) {
      var pen = new Pen(_Color.F, 2);

      ctx.DrawEllipse(r, pen);
      var dhs = _GetDragHandlePts();
      for (var i = 0, h = null; i < dhs.length; i++) {
        h = dhs[i];
        ctx.ctx().beginPath();
        ctx.ctx().arc(h.X, h.Y, dhr, 0, 2 * Math.PI);
        ctx.ctx().closePath();
        ctx.FillPath("#ffffff");
        ctx.DrawPath(pen);
      }
    }

    ctx.ctx().restore();
  };

  this.MyMouseMove = function (e) {};

  this.MyMouseUp = function (e) {
    _StopDragging();
    this.DG().SetDragMode(false);
    this.DG().UpdateTableGroups();
  };

  this.Select = function () {
    this.Invalidate();
    _MoveToTop();
    _Parent.SelectedGroup = this;
  };
  this.Deselect = function () {
    this.Invalidate();
  };

  this.Delete = function () {
    if (this.DG().SelectedGroup == this) this.DG().SelectedGroup = null;

    _Parent.GetChildren().splice(_Parent.GetChildren().indexOf(this), 1);
    _Parent.RemoveHandler("MouseMove", dg_MouseMove);
  };

  this.ContainsPt = function (pt) {
    return _PtInEllipse(pt);
  };

  this.MyMouseDown = function (e) {
    if (e.Button == MseButtons.Left) {
      var dhs = _GetDragHandlePts(),
        c = this.GetCenter();
      // check mousedown on a drag handle
      for (var i = 0; i < dhs.length; i++) {
        if (
          Math.sqrt(
            Math.pow(dhs[i].Y - e.Pos.Y, 2) + Math.pow(dhs[i].X - e.Pos.X, 2)
          ) < _DragHandleRadius
        ) {
          _StartDragging(e.Pos, i);
          break;
        }
      }

      // check mousedown on the group
      if (_Dragging == null && _PtInEllipse(e.Pos)) _StartDragging(e.Pos, null);

      if (_Dragging != null) {
        e.Cancel = true;
        this.Select();
      }
    }
  };

  this.SetLabel = function (label) {
    _Label = label;
  };

  this.GetLabel = function () {
    return _Label;
  };

  this.SetColor = function (color) {
    _Color = color;
  };

  this.GetColor = function () {
    return _Color;
  };
}

/****************************************************************
 * Foreign Key
 ***************************************************************/
DGFKey.prototype = new DGDiagramObject();
DGFKey.prototype.constructor = DGFKey;
function DGFKey(parent) {
  DGDiagramObject.call(this, parent);
  this.GetType = function () {
    return "DGFKey";
  };
  var _Parent = parent;
  var self = this;

  var _Points = [];
  var _FromTable = null;
  var _FromCol = null;
  var _ToTable = null;
  var _ToCol = null;
  var _OverrideEndPoint = _Parent.GetMousePos();
  var _MoveToTop = function () {
    self.DG().MoveChildTo(self, self.DG().GetLastFKeyIndex());
  };

  var _GetStartPos = function () {
    return _GetColPos(_FromTable, _FromCol);
  };

  var _GetEndPos = function () {
    if (_OverrideEndPoint == null) {
      return _GetColPos(_ToTable, _ToCol);
    }
    return _OverrideEndPoint;
  };

  var _GetColPos = function (tbl, col) {
    if (tbl != null) {
      var tPos = tbl.GetPos();
      tPos.Y +=
        (tbl.GetColumns().indexOf(col) + 1) * tbl.GetColList().GetRowHeight() +
        tbl.GetColList().GetY() +
        tbl.GetColList().GetRowHeight() / 2;
      tPos.X += 7;
      return new Pt(tPos.X, tPos.Y);
    }
    return new Pt(0, 0);
  };

  this.SetFrom = function (table, column) {
    _FromTable = table;
    _FromCol = column;
  };
  this.SetTo = function (table, column) {
    _ToTable = table;
    _ToCol = column;
    _MoveToTop();
  };
  this.GetFromTable = function () {
    return _FromTable;
  };
  this.GetFromColumn = function () {
    return _FromCol;
  };
  this.GetToTable = function () {
    return _ToTable;
  };
  this.GetToColumn = function () {
    return _ToCol;
  };

  this.OverrideEndPoint = function (point) {
    _OverrideEndPoint = point;
  };

  this.GetPoints = function () {
    return _Points;
  };

  var _SortPointPairs = function (a, b) {
    return a[0].Distance(a[1]) - b[0].Distance(b[1]);
  };

  this.Draw = function (ctx) {
    if (this.DG().GetCurrentFK() != this) {
      if (
        this.DG().GetChildren().indexOf(this.GetFromTable()) < 0 ||
        this.DG().GetChildren().indexOf(this.GetToTable()) < 0 ||
        this.GetFromColumn() == null ||
        this.GetFromTable().GetColumns().indexOf(this.GetFromColumn()) < 0 ||
        this.GetToColumn() == null ||
        this.GetToTable().GetColumns().indexOf(this.GetToColumn()) < 0
      )
        this.DG().GetChildren().splice(_Parent.GetChildren().indexOf(this), 1);
    }

    var c = ctx.ctx();

    var startLeft = _GetStartPos();
    var endLeft = _GetEndPos();
    var startRight = new Pt(startLeft.X + _FromTable.GetW() - 14, startLeft.Y);
    var endRight =
      _ToTable != null
        ? new Pt(endLeft.X + _ToTable.GetW() - 14, endLeft.Y)
        : endLeft;
    var closest = [
      [startLeft, endLeft],
      [startLeft, endRight],
      [startRight, endLeft],
      [startRight, endRight],
    ];
    closest.sort(_SortPointPairs);
    var startPos = closest[0][0];
    var endPos = closest[0][1];

    var startOff = startLeft == startPos ? -1 : 1;
    var endOff = _ToTable == null ? startOff * -1 : endLeft == endPos ? -1 : 1;

    var rowHeight = _FromTable.GetColList().GetRowHeight();

    c.save();

    c.lineJoin = LineJoins.Round;
    c.lineCap = LineCaps.Round;

    c.beginPath(); // draw cord begin ---------
    c.moveTo(startPos.X, startPos.Y);
    c.lineTo(startPos.X + startOff * 7, startPos.Y);
    c.moveTo(startPos.X + startOff * 7, startPos.Y);

    var bezPt1 = new Pt(endPos.X + 5 * endOff, startPos.Y);
    var bezPt2 = new Pt(startPos.X + 5 * startOff, endPos.Y);
    var bezEnd = new Pt(endPos.X + endOff * 14, endPos.Y);

    if (_ToTable != null) {
      // Adjust the curve depending on the orientation from one table to the other
      if (startOff == endOff)
        bezPt1.X =
          startPos.X + (Math.abs(startPos.X - endPos.X) + 30) * startOff;
      if (startOff == endOff)
        bezPt2.X = endPos.X + (Math.abs(startPos.X - endPos.X) + 30) * startOff;

      if (
        startOff != endOff &&
        ((startOff == 1 && startPos.X > endPos.X) ||
          (startOff == -1 && startPos.X < endPos.X))
      ) {
        var tmp = bezPt1.X;
        bezPt1.X = bezPt2.X + startOff * 50;
        bezPt2.X = tmp + endOff * 50;
        tmp = bezPt1.Y;
        bezPt1.Y = bezPt2.Y;
        bezPt2.Y = tmp;
      }
    }

    c.bezierCurveTo(bezPt1.X, bezPt1.Y, bezPt2.X, bezPt2.Y, bezEnd.X, bezEnd.Y);

    c.moveTo(endPos.X + endOff * 14, endPos.Y);
    c.lineTo(endPos.X + endOff * 3, endPos.Y);
    c.strokeStyle = "#9A9A9A";
    c.lineWidth = 4;
    c.stroke();
    c.strokeStyle = "#e6e6e6";
    c.lineWidth = 3;
    c.stroke();
    c.closePath(); // end ---------

    c.beginPath(); // draw start begin ---------
    c.rect(
      startOff == -1 ? startPos.X - 7 : startPos.X - 1,
      startPos.Y - 3,
      8,
      6
    );
    c.strokeStyle = "#9A9A9A";
    c.lineWidth = 1;
    c.stroke();
    c.fillStyle = "#e6e6e6";
    c.fill();
    c.closePath(); // end ---------

    c.beginPath(); // draw end begin ---------
    c.arc(endPos.X, endPos.Y, 6, Math.PI * 0.5, Math.PI * 1.5, endOff == 1);
    c.fillStyle = "#9A9A9A";
    c.fill();
    c.closePath(); // end ---------

    c.restore();
  };

  this.AccumCollisions = function (pt, coll) {};
}

/****************************************************************
 * Diagram
 ***************************************************************/
DGDiagram.prototype = new DGDiagramObject();
DGDiagram.prototype.constructor = DGDiagram;
function DGDiagram(canvas) {
  DGDiagramObject.call(this, null);
  var _Cvs = canvas;
  var _Ctx = new DrawContext(_Cvs);
  _Ctx.ctx().save();

  var self = this;
  var _YOffset = $(_Cvs).offset().top;
  var _ChildZOrder = { DGGroup: 0, DGFKey: 1, DGTable: 2 };
  var _NeedSort = false;

  var _ZoomIndex = 5;
  var _Zoom = 1;
  var _Offset = new Pt(0, 0);
  var _DragMode = false;
  var _ScrollVel = new Pt(0, 0);
  // middle mouse button scrolling
  var _MScroll = null;
  var _Name = "Untitled Diagram 1";
  var _CurrentZoom = { zoom: _Zoom };

  var _ChildrenSort = function (a, b) {
    return b.GetType() == a.GetType()
      ? 0
      : _ChildZOrder[a.GetType()] > _ChildZOrder[b.GetType()]
      ? 1
      : -1;
  };
  var _SortChildren = function () {
    if (_NeedSort) {
      self.GetChildren().sort(_ChildrenSort);
      _NeedSort = false;
    }
  };

  var _ApplyContextSettings = function () {
    _Ctx.ctx().restore();
    _Ctx.ctx().save();
    _Ctx.ctx().scale(_Zoom, _Zoom);
    _Ctx.ctx().translate(_Offset.X, _Offset.Y);
  };

  this.DG = function () {
    return this;
  };
  this.SetScale = function (s) {
    _Scale = s;

    for (var i in this.GetChildren()) {
      this.GetChildren()[i].SetScale(s);
    }
  };

  this.SortChildren = function () {
    _NeedSort = true;
    _SortChildren();
  };

  this.FindChildren = function (lambda) {
    var res = [],
      children = this.GetChildren();
    for (var i in children) {
      if (lambda(children[i])) {
        res.push(children[i]);
      }
    }

    return res;
  };

  this.SetCenter = function (pt) {
    var v = this.GetView();
    this.SetOffset(new Pt(-pt.X - v.W / -2, -pt.Y - v.H / -2));
  };

  this.SetDragMode = function (ison) {
    _DragMode = ison;
  };
  this.GetDragMode = function (ison) {
    return _DragMode;
  };

  this.GetZoom = function () {
    return _Zoom;
  };
  this.GetZoomIdx = function () {
    return _ZoomIndex;
  };
  this.SetZoomIdx = function (idx) {
    _ZoomIndex = idx;
    $(_CurrentZoom)
      .stop()
      .animate(
        { zoom: ZoomLevels[idx] },
        {
          duration: 600,
          step: function () {
            _Zoom = _CurrentZoom.zoom;
            self.SetScale(_CurrentZoom.zoom);
            _ApplyContextSettings();
          },
        }
      );
  };
  this.ZoomOut = function () {
    this.SetZoomIdx(_ZoomIndex - 1 >= 0 ? _ZoomIndex - 1 : 0);
  };
  this.ZoomIn = function () {
    this.SetZoomIdx(
      _ZoomIndex + 1 < ZoomLevels.length ? _ZoomIndex + 1 : _ZoomIndex
    );
  };
  this.Reset = function () {
    this.SetZoomIdx(5);
    this.SetOffset(new Pt(0, 0));
    this.GetChildren().splice(0, this.GetChildren().length);
    this.SelectedTable = null;
    this.SelectedGroup = null;
  };

  this.GetOffsetX = function () {
    return _Offset.X;
  };
  this.GetOffsetY = function () {
    return _Offset.Y;
  };
  this.SetOffset = function (pt) {
    _Offset.X = pt.X;
    _Offset.Y = pt.Y;
    _ApplyContextSettings();
  };

  this.GetView = function () {
    return new Rect(
      -this.GetOffsetX(),
      -this.GetOffsetY(),
      this.GetW() / this.GetZoom(),
      this.GetH() / this.GetZoom()
    );
  };

  this.GetName = function () {
    return _Name;
  };
  this.SetName = function (name) {
    _Name = name;
  };

  this.TheCanvas = function () {
    return _Cvs;
  };

  var _MsePos = null;

  this.GetMousePos = function () {
    return _MsePos;
  };

  this.ChildrenChanged = function () {
    //_NeedSort = true;
  };

  this.UpdateTableGroups = function (groups, tables) {
    var groups =
        groups ||
        this.FindChildren(function (o) {
          return o.GetType() == "DGGroup";
        }),
      tables =
        tables ||
        this.FindChildren(function (o) {
          return o.GetType() == "DGTable";
        });
    for (var i in tables) {
      var t = tables[i],
        tc = t.GetCenter(),
        found = false;
      if (t.GetType() == "DGTable") {
        for (var j = groups.length - 1; j >= 0; j--) {
          if ((found = groups[j].ContainsPt(tc))) {
            t.Group = groups[j];
            break;
          }
        }
        if (!found) t.Group = null;
      }
    }
  };

  this.MoveChild = function (child, move) {
    var children = this.GetChildren(),
      index = children.indexOf(child);
    switch (move) {
      case Moves.Up:
        this.MoveChildTo(child, children.length == index ? index : index + 1);
        break;
      case Moves.Down:
        this.MoveChildTo(child, index == 0 ? 0 : index - 1);
        break;
      case Moves.Top:
        this.MoveChildTo(child, children.length);
        break;
      case Moves.Bottom:
        this.MoveChildTo(child, 0);
        break;
    }
  };
  this.MoveChildTo = function (child, index) {
    var children = this.GetChildren();
    children.splice(children.indexOf(child), 1);
    children.splice(index, 0, child);
  };

  this.GetLastGroupIndex = function () {
    var grps = this.FindChildren(function (o) {
      return o.GetType() == "DGGroup";
    });
    return grps.length - 1;
  };

  this.GetLastFKeyIndex = function () {
    var fkeys = this.FindChildren(function (o) {
      return o.GetType() == "DGFKey";
    });
    return fkeys.length - 1 + this.GetLastGroupIndex();
  };

  this.GetCurrentFK = function () {
    return _CurrentFK;
  };

  // Canvas event handlers
  var _DoPreventContextMenu = function () {
    _PreventContextMenu = true;
  };
  var _PreventContextMenu = false;

  this.StartMScroll = function (pt) {
    _MScroll = { StartPos: pt };
    this.SetDragMode(true);
  };
  this.EndMScroll = function () {
    _MScroll = null;
    this.SetDragMode(false);
  };

  var _CreateMouseEvent = function (e) {
    const button = !!e.which
      ? e.which
      : typeof e.button === "number"
      ? e.button + 1
      : 1;
    return {
      Event: e,
      Pos: _MsePos,
      Button: button,
      DisableContextMenu: _DoPreventContextMenu,
    };
  };

  var doc_MouseMove = function (e) {
    if (_DragMode) {
      var w = self.GetW() / 2,
        xtoC = Math.abs(e.pageX - w),
        xDir = ((e.pageX - w) / Math.abs(e.pageX - w)) * -1,
        h = self.GetH() / 2,
        ytoC = Math.abs(e.pageY - h),
        yDir = ((e.pageY - h) / Math.abs(e.pageY - h)) * -1;

      _ScrollVel.X = w - 35 < xtoC ? ((xtoC - (w - 35)) / 35) * xDir : 0;
      _ScrollVel.Y = h - 35 < ytoC ? ((ytoC - (h - 35)) / 35) * yDir : 0;
    }

    if (_MScroll != null) {
      _ScrollVel.X = Math.max(
        Math.min((_MScroll.StartPos.X - e.pageX) / 60, 2),
        -2
      );
      _ScrollVel.Y = Math.max(
        Math.min((_MScroll.StartPos.Y - e.pageY) / 60, 2),
        -2
      );
    }
  };
  $(document).mousemove(doc_MouseMove);

  var cvs_MouseMove = function (e) {
    _MsePos = new Pt(
      e.pageX / _Zoom - _Offset.X,
      (e.pageY - _YOffset) / _Zoom - _Offset.Y
    );

    if (_CurrentFK != null) {
      _CurrentFK.OverrideEndPoint(_MsePos);
    }

    self.TriggerMouseEvent("MouseMove", e);
    self.MouseMove({ Event: e, Pos: _MsePos });
  };
  var cvs_MouseUp = function (e) {
    self.TriggerMouseEvent("MouseUp", e);
    self.MouseUp(_CreateMouseEvent(e));

    self.EndMScroll();
  };
  var cvs_MouseDown = function (e) {
    if (_CurrentFK != null) {
      if (e.which == MseButtons.Left) {
        // get all items colliding with the mouse down point
        var collisions = [];
        for (var i = 0; i < self.GetChildren().length; i++) {
          self.GetChildren()[i].AccumCollisions(_MsePos, collisions);
        }

        // if the mouse was dropped on a column while creating a
        // foreign key, then make that column the to column for
        // the key
        if (collisions.length > 0) {
          var colList = collisions[collisions.length - 1];
          var column =
            colList.GetType() == "DGColList"
              ? colList.GetColumnAtPoint(_MsePos)
              : null;
          if (
            column != null &&
            column.DataType == _CurrentFK.GetFromColumn().DataType &&
            column != _CurrentFK.GetFromColumn()
          ) {
            _CurrentFK.SetTo(colList.GetParent(), column);
          } else {
            // cancel adding the foreign key if no column was
            // clicked
            self
              .GetChildren()
              .splice(self.GetChildren().indexOf(_CurrentFK), 1);
          }
        }
        _CurrentFK.OverrideEndPoint(null);
        self.SetDragMode(false);
        _CurrentFK = null;
        return false;
      }
    }

    var prevSelected = self.SelectedTable;
    self.SelectedTable = null;
    self.SelectedGroup = null;
    if (prevSelected) prevSelected.Invalidate();

    self.TriggerMouseEvent("MouseDown", e);
    self.MouseDown(_CreateMouseEvent(e));

    // move the selected table to the end of the child list
    // so that it is drawn above the rest
    var newSelected = null;
    for (var i in self.GetChildren()) {
      var t = self.GetChildren()[i];
      if (t == self.SelectedTable) {
        newSelected = t;
        self.GetChildren().splice(i, 1);
        self.GetChildren().push(t);
        break;
      }
    }
    if (prevSelected && prevSelected != newSelected) {
      prevSelected.Deselect();
    }

    // start the middle mouse scrolling
    if (e.which == MseButtons.Middle) {
      self.StartMScroll(new Pt(e.pageX, e.pageY));
    }
  };
  $(_Cvs)
    .mousemove(cvs_MouseMove)
    .mouseup(cvs_MouseUp)
    .mousedown(cvs_MouseDown);

  // two wheel events fire simultaneously. only need one
  var _DampenWheel = false;
  var cvs_MouseWheel = function (e, delta) {
    if (!_DampenWheel) {
      if (delta < 0) {
        self.ZoomOut();
      } else if (delta > 0) {
        self.ZoomIn();
      }
      $(_Offset).animate(
        new Pt(
          -_MsePos.X + self.GetW() / 2 / self.GetZoom(),
          -_MsePos.Y + self.GetH() / 2 / self.GetZoom()
        ),
        {
          duration: 600,
          step: function () {
            self.SetOffset(_Offset);
          },
        }
      );
      _DampenWheel = true;
      setTimeout(function () {
        _DampenWheel = false;
      }, 100);
    }
  };
  $(_Cvs).mousewheel(cvs_MouseWheel);

  var doc_ContextMenu = function (e) {
    var preventNow = _PreventContextMenu;
    _PreventContextMenu = false;
    if (preventNow) {
      return false;
    }
  };
  $(document).bind("contextmenu", doc_ContextMenu);

  var _ExecuteMouseEvent = function (child, type, args) {
    child[type](args);
  };

  this.TriggerMouseEvent = function (type, e) {
    for (var i = this.GetChildren().length - 1; i >= 0; i--) {
      var children = [];
      this.GetChildren()[i].AccumCollisions(_MsePos, children);
      for (var ii = children.length - 1; ii >= 0; ii--) {
        var args = _CreateMouseEvent(e);
        children[ii][type](args);
        if (args.Cancel) {
          return;
        }
      }
    }
  };

  this.RemoveRelationship = function (fromCol) {
    var fk = self.GetRelationshipByFromColumn(fromCol);
    if (fk != null) {
      self.GetChildren().splice(self.GetChildren().indexOf(fk), 1);
    }
  };

  this.GetRelationshipByFromColumn = function (fromCol) {
    for (var i in this.GetChildren()) {
      var child = this.GetChildren()[i];
      if (child.GetType() == "DGFKey") {
        if (child.GetFromColumn() == fromCol) {
          return child;
        }
      }
    }

    return null;
  };

  var _CurrentFK = null;
  this.AddRelationship = function (fromTable, fromCol) {
    // if the passed from column already has a fk, then delete that
    // one before creating a new one
    self.RemoveRelationship(fromCol);
    _CurrentFK = new DGFKey(this);
    _CurrentFK.SetFrom(fromTable, fromCol);
    _CurrentFK.OverrideEndPoint(_MsePos);
    this.SetDragMode(true);
  };

  this.Draw = function (ctx) {
    if (_DragMode) {
      this.SetOffset(
        new Pt(_Offset.X + _ScrollVel.X * 30, _Offset.Y + _ScrollVel.Y * 30)
      );
    }

    _Ctx
      .ctx()
      .clearRect(
        0 - _Offset.X,
        0 - _Offset.Y,
        this.GetW() / _Zoom,
        this.GetH() / _Zoom
      );

    // TODO: draw grid lines

    _SortChildren();
    var children = this.GetChildren();
    for (var i in children) {
      children[i].Draw(_Ctx);
    }
  };
}

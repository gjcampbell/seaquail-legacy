<%@ Page Title="Print Sea Quail Diagram" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DiagramPrint.aspx.cs" Inherits="SeaQuail_DiagramTool.DiagramPrint" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Media/Stylesheets/DiagramPrint.css" rel="Stylesheet" type="text/css" />
    <style type="text/css" media="print">
        #header
        {
        	display: none;
        }
        
        body
        {
        	margin: 0px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="header">
        <div style="float: left; ">
            <div id="btnScale-outer"><div><a href="javascript:" title="Click to reset scale.">Scale</a>: </div><div id="btnScale"><div>&nbsp;</div></div><div id="btnScaleRes"></div></div>
        </div>
        <div style="float: right; ">
            <input type="button" id="btnGetImage" value="Get Image" />
            <input type="button" id="btnPrint" value="Print" />
        </div>
    </div>
    <canvas id="cvs"></canvas>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cntFooter" runat="server">
    <script type="text/javascript" src="JavaScript/jquery-1.4.2.min.js"></script>    <script type="text/javascript" src="JavaScript/DrawContext.js"></script>    <script type="text/javascript" src="JavaScript/DrawnObject.js"></script>    <script type="text/javascript">
        var Ctx = null,
            OrigSize = { W: 0, H: 0 },
            Scale = 1,
            Offset = null,
            NeedRedraw = true,
            Redraw = function() {
                NeedRedraw = true;
            },
            GetSize = function() {
                return { W: OrigSize.W * Scale + 160, H: OrigSize.H * Scale + 160 };
            },
            SetCanvasSize = function(size) {
                $('#cvs').attr({
                    width: size.W,
                    height: size.H
                });
                $('#btnScaleRes').text(parseInt(size.W) + ' x ' + parseInt(size.H));
            },
            ScaleCanvas = function(factor) {
                if (Ctx) {
                    SetCanvasSize(GetSize());
                    Ctx.ctx().restore();
                    Ctx.ctx().save();
                    Ctx.ctx().scale(factor, factor);
                    Ctx.ctx().translate(Offset.X, Offset.Y);
                    Redraw();
                }
            };

        $('#btnPrint').click(function() {
            window.print();
        });

        $('#btnGetImage').click(function() {
            var src = $('#cvs').get(0).toDataURL("image/png");
            window.open(src, 'Diagram Image');
        });

        $('#btnScale').scroll(function() {
            ScaleCanvas(Scale = Math.pow(2, (($('#btnScale').scrollLeft() - 750) / 750) * 3));
        });

        $('#btnScale-outer a').click(function() {
            $('#btnScale').scrollLeft(750);
        });

        window.Load = function(dg) {
            window.Loaded = true;
            var minPt = new Pt(0, 0),
                maxPt = new Pt(0, 0),
                dgitems = dg.GetChildren();

            // get the list of tables and the min and max points to 
            // show in the overview
            for (var i in dgitems) {
                if (dgitems[i].GetType() == 'DGTable') {
                    minPt.X = Math.min(minPt.X, dgitems[i].GetX());
                    minPt.Y = Math.min(minPt.Y, dgitems[i].GetY());
                    maxPt.X = Math.max(maxPt.X, dgitems[i].GetX() + dgitems[i].GetW());
                    maxPt.Y = Math.max(maxPt.Y, dgitems[i].GetY() + dgitems[i].GetH());
                }
            }

            SetCanvasSize(OrigSize = { W: maxPt.X - minPt.X, H: maxPt.Y - minPt.Y });
            Offset = { X: -minPt.X + 80, Y: -minPt.Y + 80 };

            Ctx = new DrawContext($('#cvs').get(0));
            Ctx.SetDrawMode(DrawModes.Normal);
            Ctx.ctx().save();
            $('#btnScale').scrollLeft(750);

            var draw = function() {
                if (NeedRedraw) {
                    var dim = GetSize();
                    Ctx.ctx().clearRect(minPt.X, minPt.Y, maxPt.X, maxPt.Y);
                    for (var i in dgitems) {
                        dgitems[i].Draw(Ctx);
                    }

                    NeedRedraw = false;
                }
                setTimeout(draw, 50);
            }
            draw();
        }
    </script>
</asp:Content>

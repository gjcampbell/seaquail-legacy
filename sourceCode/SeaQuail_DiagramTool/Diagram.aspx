<%@ Page Title="Sea Quail Database Diagram Tool" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Diagram.aspx.cs" Inherits="SeaQuail_DiagramTool.Diagram" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link media="screen" rel="Stylesheet" href="/Media/Stylesheets/Diagram.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page">
        <div id="header">
            <div class="dropdownmenu">
                <div class="dropdownmenu-inner">
                    <div id="divWelcome"></div>
                    <a href="#"><img src="/Media/Images/Icons/sitemap.png" alt="New Diagram" />New Diagram</a>
                    <a href="#" data-shortcut="ctrl|S" class="disable can-disable no-readonly" title="Save Diagram (Ctrl + S)"><img src="/Media/Images/Icons/disk.png" alt="Save Diagram" />Save Diagram</a>
                    <a href="#"><img src="/Media/Images/Icons/script_save.png" alt="Create Script" />Generate Create Script</a>
                    <a href="#" data-shortcut="ctrl|O" class="disable can-disable" title="My Diagram (Ctrl + O)"><img src="/Media/Images/Icons/folder.png" alt="My Diagrams" />My Diagrams</a>
                    <a href="#" data-shortcut="ctrl|P" title="Print Diagram (Ctrl + P)"><img src="/Media/Images/Icons/printer.png" alt="Print Diagram" />Print Diagram</a>
                    <div class="divider">&nbsp;</div>
                    <a href="#" class="disable can-disable"><img src="/Media/Images/Icons/folder_camera.png" alt="Diagram Snapshots" />Diagram Snapshots</a>
                    <a href="#" data-shortcut="ctrl|shift|S" class="disable can-disable no-readonly" title="Create Snapshot (Ctrl + Shift + S)"><img src="/Media/Images/Icons/camera.png" alt="Create Snapshot" />Create Snapshot</a>
                    <a href="#" class="disable always-disable can-disable no-readonly" title="Not Yet Available"><img src="/Media/Images/Icons/script_camera.png" alt="Generate Change Script" />Generate Change Script</a>
                    <div class="divider">&nbsp;</div>
                    <a href="#" class="disable can-disable no-readonly"><img src="/Media/Images/Icons/group.png" alt="Share Diagram" />Share Diagram</a>
                    <div class="divider">&nbsp;</div>
                    <a href="#" id="btnSignIn"><img src="http://www.google.com/favicon.ico" alt="Sign In" />Sign In</a>
                    <a href="#"><img src="/Media/Images/Icons/help.png" alt="Help" />Help</a>
                    <a href="http://seaquail.codeplex.com/"><img src="/Media/Images/Icons/house.png" alt="Home" />Sea Quail Home</a>
                </div>
                <div class="title">Sea Quail Database Diagram Tool</div>
            </div>
            <div class="toolstrip">
                <div class="toolstrip-inner" style="float: left; margin-left: 195px; ">
                    <a href="#" class="no-readonly" title="Add Table"><img src="/Media/Images/Icons/table_add.png" alt="Add Table" />Add Table</a>
                    <a href="#" class="no-readonly" title="Add Tables"><img src="/Media/Images/Icons/table_multiple_add.png" alt="Add Tables" /></a>
                    <a href="#" class="no-readonly" title="Add Group"><img src="/Media/Images/Icons/layer-shape-ellipse-add.png" alt="Add Group" /></a>
                    <div class="divider no-readonly">&nbsp;</div>
                    <a href="#" data-shortcut="ctrl|C" class="no-readonly disable enable-TableSelected" title="Copy Selected Table (Ctrl + C)"><img src="/Media/Images/Icons/table_copy.png" alt="Copy Table" /></a>
                    <a href="#" data-shortcut="ctrl|V" class="no-readonly disable enable-TableCopied" title="Paste Table (Ctrl + V)"><img src="/Media/Images/Icons/paste_table.png" alt="Paste Table" /></a>
                    <a href="#" data-shortcut="delete" class="no-readonly disable enable-ItemSelected" title="Delete Selected Item (Delete)"><img src="/Media/Images/Icons/delete.png" alt="Delete Item" />&nbsp;</a>
                    <!--<a href="#"><img src="/Media/Images/Icons/table_multiple.png" alt="Add Table Group" title="Add Table Group" />Add Table Group</a>-->
                    <a href="#" class="for-readonly" title="Save a Copy"><img src="/Media/Images/Icons/disk.png" alt="Save a Copy" /></a>
                    <div class="divider">&nbsp;</div>
                    <span id="diagram-name">Untitled Diagram 1</span><a href="#" class="no-readonly" title="Rename Diagram"><img src="/Media/Images/Icons/textfield_rename.png" alt="Rename Diagram" />&nbsp;</a>
                </div>
                <div class="toolstrip-inner">
                    <a href="#" data-shortcut="ctrl|F" title="Find Text (Ctrl + F)"><img src="/Media/Images/Icons/binocular.png" alt="Find Text" />Search</a>
                    <a href="#"><img src="/Media/Images/Icons/magnifier.png" alt="Overview" />Overview</a>
                    <a href="#" class="hover no-readonly"><img src="/Media/Images/Icons/application_side_list.png" alt="Properties" />Properties</a>
                </div>
            </div>
            <div id="show-snapshots" class="tool-strip-dropdown" style="display: none; ">
                <div class="tool-panel-item">
                    <div class="head">Snapshots</div>
                    <div id="snapshot-message"></div>
                    <div id="snapshot-list" class="dbitem-list">
                    </div>
                </div>
            </div>
            <div id="tool-panel" class="tool-strip-dropdown">
                <div id="overview" class="tool-panel-item" style="display: none; ">
                    <div class="head">Overview</div>
                    <div class="canvas-cont">
                        <canvas id="diagram-overview" width="220" height="220"></canvas>
                    </div>
                    <div class="toolstrip">
                        <div class="toolstrip-inner">
                            <a href="#" title="Zoom In"><img src="/Media/Images/Icons/magnifier_zoom_in.png" alt="Zoom In" />&nbsp;</a>
                            <a href="#" title="Zoom Out"><img src="/Media/Images/Icons/magnifier_zoom_out.png" alt="Zoom Out" />&nbsp;</a>
                        </div>
                    </div>
                </div>
                <div id="find" class="tool-panel-item" style="display: none; ">
                    <div class="head">Search Diagram</div>
                    <div class="content">
                        Search by column or table name.<br />
                        <input type="text" id="txtFind" />
                    </div>
                    <div class="toolstrip">
                        <div class="toolstrip-inner">
                            <a href="#" title="Find Next"><img src="/Media/Images/Icons/binocular--arrow.png" alt="Find Next" /> Find Next</a>
                        </div>
                    </div>
                </div>
                <div id="edit-table" class="tool-panel-item">
                    <div class="head">Properties</div>
                    <strong>Selected Table</strong>
                    <div class="propgrid">
                        <table>
                            <tr>
                                <td class="prop">Name</td>
                                <td class="value"><input type="text" id="tbl-Name" /></td>
                            </tr>
                        </table>
                    </div>
                    <div id="edit-column">
                        <strong>Selected Column</strong>
                        <div class="propgrid">
                            <table>
                                <tr>
                                    <td class="prop">Name</td>
                                    <td class="value"><input type="text" id="col-Name" /></td>
                                </tr>
                                <tr>
                                    <td class="prop">Data Type</td>
                                    <td class="value">
                                        <select id="col-DataType">
                                            <option value="BigInt">BigInt</option>
                                            <option value="Int">Int</option>
                                            <option value="SmallInt">SmallInt</option>
                                            <option value="Boolean">Boolean</option>
                                            <option value="Decimal">Decimal</option>
                                            <option value="DateTime">DateTime</option>
                                            <option value="String">String</option>
                                            <option value="Bytes">Bytes</option>
                                        </select>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="prop">Nullable</td>
                                    <td class="value"><input type="checkbox" id="col-Nullable" /></td>
                                </tr>
                                <tr class="col-cond col-cond-HasLength">
                                    <td class="prop">Length</td>
                                    <td class="value"><input type="text" id="col-Length" /></td>
                                </tr>
                                <tr class="col-cond col-cond-HasPrecision">
                                    <td class="prop">Precision</td>
                                    <td class="value"><input type="text" id="col-Precision" /></td>
                                </tr>
                                <tr class="col-cond col-cond-HasPrecision">
                                    <td class="prop">Scale</td>
                                    <td class="value"><input type="text" id="col-Scale" /></td>
                                </tr>
                                <tr class="col-cond col-cond-PKOK">
                                    <td class="prop">Primary Key</td>
                                    <td class="value"><input type="checkbox" id="col-IsPrimary" /></td>
                                </tr>
                                <tr class="col-cond col-cond-IDOK">
                                    <td class="prop">Identity</td>
                                    <td class="value"><input type="checkbox" id="col-IsIdentity" /></td>
                                </tr>
                                <tr>
                                    <td class="prop">Default Value</td>
                                    <td class="value"><input type="text" id="col-DefaultValue" /></td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
                <div id="edit-group" class="tool-panel-item">
                    <div class="head">Properties</div>
                    <strong>Selected Group</strong>
                    <div class="propgrid">
                        <table>
                            <tr>
                                <td class="prop">Label</td>
                                <td class="value"><input type="text" id="grp-Label" /></td>
                            </tr>
                            <tr>
                                <td class="prop">Color</td>
                                <td class="value">
                                    <select id="grp-Color"></select>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>                
            </div>
            <div id="about-cont" style="display: none; ">
                <div class="toolstrip">
                    <div class="toolstrip-inner">
                        <a href="#"><img src="/Media/Images/Icons/cross.png" alt="Close About" />&nbsp;</a>
                    </div>
                </div>
                <div class="about-inner">
                    <p>
                        This is a proof of concept database diagram tool written for Chrome and the HTML 5 canvas.
                        It makes use of the <a href="http://seaquail.codeplex.com/">Sea Quail SQL Writing Library</a> for MySQL SQL generation.
                    </p>
                    <p>
                        This should not be expected to work in any browser other than chrome, and even in Chrome
                        it has plenty of bugs, however it's fun to play with.
                    </p>
                    <p>
                        Thanks for checking it out!<br />
                        <a href="http://www.codeplex.com/site/users/view/sledgebox">Gabriel</a>
                    </p>
                </div>
            </div>
        </div>
        <div id="body">
            <canvas id="diagram">
            </canvas>
        </div>
        
        <div id="popupCont" style="display: none; ">
            <div id="popupBG">
            </div>
            <div id="popup">
                <div id="popupTitle"></div>
                <div id="popup-inner">
                    <div id="MyDiagrams" style="display: none; ">
                        <div class="popupMenu">
                            <div class="toolstrip">
                                <div class="toolstrip-inner"></div>
                            </div>
                        </div>
                        <div class="popup-grid">
                            <ul>
                            </ul>
                        </div>
                    </div>
                    <div id="Sharing" style="display: none">
                        <div class="popupMenu">
                            <div class="toolstrip">
                                <div class="toolstrip-inner"></div>
                            </div>
                        </div>
                        <div class="instructions">
                            <div id="divAllowPublicAccess">
                                Public Access: <span id="lblAllowPublicAccess">Disallowed</span> (<a id="btnAllowPublicAccess" href="#" onclick="TogglePublicAccess(); ">Allow</a>)
                                <a target="_blank" id="lblPublicAccessLink"></a>
                            </div>
                            You are sharing this diagram with following users:
                        </div>
                        <div class="popup-grid">
                            <ul></ul>
                        </div>
                    </div>
                    <div id="Snapshots" style="display: none; ">
                        <div class="popup-grid">
                            <ul>
                            </ul>
                        </div>
                        <div class="instructions">
                            Click the name of a snapshot to load it. <br />
                            Note: unsaved changes to the current diagram will be lost. Even when a snapshot is loaded it can not be modified, so when you save after loading a snapshot you will be saving over the diagram. 
                        </div>
                    </div>
                    <div id="Script" style="display: none;">
                        <div class="popupMenu">
                            <div class="toolstrip">
                                <div class="toolstrip-inner">
                                    <a href="#" onclick="CreateScript('MySQL')">MySQL</a>
                                    <a href="#" onclick="CreateScript('PostgreSQL')">PostgreSQL</a>
                                    <a href="#" onclick="CreateScript('SQL Server')">SQL Server</a>
                                </div>
                            </div>
                        </div>
                        <textarea id="txtScript"></textarea>
                    </div>
                </div>
            </div>
        </div>
        
        <div id="readonly-overlay">&nbsp;</div>
        <div id="loading-overlay"><div>&nbsp;</div></div>
    </div>
</asp:Content>
<asp:Content runat="server" id="cntFooter" ContentPlaceHolderID="cntFooter">
    <script type="text/javascript" src="JavaScript/jquery-1.4.2.min.js"></script>    <script type="text/javascript" src="JavaScript/jquery.json-1.3.min.js"></script>    <script type="text/javascript" src="JavaScript/jquery.mousewheel.js"></script>    <script type="text/javascript" src="JavaScript/OpenIDPopup.js"></script>    <script type="text/javascript" src="JavaScript/Dateformat.js"></script>    <script type="text/javascript" src="JavaScript/DrawContext.js"></script>    <script type="text/javascript" src="JavaScript/ContextMenu.js"></script>    <script type="text/javascript" src="JavaScript/DrawnObject.js"></script>    <script type="text/javascript" src="JavaScript/DG.js"></script>    <script type="text/javascript">
        var userID = 0;
    
        var TrackEvent = function(cate, action, label, value) {
            var req = ['_trackEvent', cate, action];
            if (label)
                req.push(label);
            if (value)
                req.push(value);
            if (window._gaq)
                _gaq.push(req);
        }

        var showLoading = function(target) {
            var ol = $('#loading-overlay'),
                oli = $('div', ol);

            ol.show().css({ top: target.offset().top + 'px', left: target.offset().left + 'px' });
            oli.width(target.width()).height(target.parent().height());
        }, closeLoading = function() {
            $('#loading-overlay').hide();
        }
        
    
        var GetSelectedTable = function() {
            return DG.SelectedTable;
        }
        var GetSelectedColIndex = function() {
            var table = GetSelectedTable();
            if (table) {
                if (table.SelectedColumn != null) {
                    return table.SelectedColumn;
                }
            }
            return null;
        }
        var GetSelectedColumn = function(idx) {
            var idx = typeof (idx) == 'undefined' ? GetSelectedColIndex() : idx;
            if (idx != null && DG.SelectedTable) {
                return DG.SelectedTable.GetColumns()[idx];
            }

            return null;
        }

        var GetJSONDG = function() {
            var dg = {
                Name: DG.GetName(),
                ID: DG.ID || -1,
                Tables: [],
                FKeys: [],
                Groups: []
            };

            var children = DG.GetChildren();
            for (var i in children) {
                var child = children[i];
                if (child.GetType() == 'DGTable') {
                    var t = {
                        Name: child.GetName(),
                        GUID: child.GetGUID(),
                        X: child.GetX(),
                        Y: child.GetY(),
                        Columns: child.GetColumns()
                    };
                    dg.Tables.push(t);
                } else if (child.GetType() == 'DGFKey') {
                    var fk = {
                        From: { Table: child.GetFromTable().GetGUID(), Column: child.GetFromColumn().GUID },
                        To: { Table: child.GetToTable().GetGUID(), Column: child.GetToColumn().GUID }
                    };
                    dg.FKeys.push(fk);
                } else if (child.GetType() == 'DGGroup') {
                    var grp = {
                        Label: child.GetLabel(),
                        Color: child.GetColor(),
                        X: child.GetX(),
                        Y: child.GetY(),
                        W: child.GetW(),
                        H: child.GetH()
                    };
                    dg.Groups.push(grp);
                }
            }

            return dg;
        }

        var UpdateDGName = function() {
            $('#diagram-name').text(DG.GetName());
        }

        var LoadDiagramDef = function(def) {
            DG.Reset();
            for (var i in def.Groups) {
                var jgrp = def.Groups[i],
                    grp = new DGGroup(DG);
                grp.SetX(jgrp.X);
                grp.SetY(jgrp.Y);
                grp.SetW(jgrp.W);
                grp.SetH(jgrp.H);
                grp.SetLabel(jgrp.Label);
                grp.SetColor(jgrp.Color);
            }
            for (var i in def.Tables) {
                var t = new DGTable(DG);
                t.SetGUID(def.Tables[i].GUID);
                t.SetName(def.Tables[i].Name);
                t.SetX(def.Tables[i].X);
                t.SetY(def.Tables[i].Y);
                t.SetW(50);
                t.SetH(50);
                t.GetColumns().splice(0, 1);
                var cols = def.Tables[i].Columns;
                for (var i in cols) {
                    var col = cols[i];
                    t.GetColumns().push(col);
                    col.DataType = _DataTypes[col.DataType.Name];
                }
            }
            for (var i in def.FKeys) {
                var fk = def.FKeys[i];
                var from = { Table: null, Column: null };
                var to = { Table: null, Column: null };
                var children = DG.GetChildren();
                for (var i in children) {
                    if (children[i].GetType() == 'DGTable') {
                        if (children[i].GetGUID() == fk.From.Table) {
                            from.Table = children[i];
                            from.Column = children[i].GetColumnByGUID(fk.From.Column);
                        }
                        if (children[i].GetGUID() == fk.To.Table) {
                            to.Table = children[i];
                            to.Column = children[i].GetColumnByGUID(fk.To.Column);
                        }
                    }
                }
                var f = new DGFKey(DG);
                DG.MoveChild(f, Moves.Bottom);
                f.OverrideEndPoint(null);
                f.SetFrom(from.Table, from.Column);
                f.SetTo(to.Table, to.Column);
            }
            DG.SortChildren();
            DG.UpdateTableGroups();
        }

        var LoadDiagram = function(id) {
            $.ajax({
                url: '/GetDiagram.dg',
                cache: false,
                type: 'GET',
                data: { ID: id },
                success: function(res) {
                    var dg = JSON.parse(res),
                        def = JSON.parse(dg.PrimarySnapshot.DiagramData);

                    SetReadonly(false);
                    if (dg.UserID != userID && id != 'WelcomeDiagram') {
                        SetReadonly(true);
                    }

                    var cvs = document.getElementById('diagram');
                    DG.AllowPublicAccess = dg.AllowPublicAccess;
                    DG.SetName(dg.Name);
                    DG.ID = dg.ID;
                    UpdateDGName();
                    LoadDiagramDef(def);
                }
            });
        }

        var CreateScript = function(lang) {
            var dg = GetJSONDG();
            showLoading($('#txtScript'));
            $.post('/CreateScript.dg', { Diagram: JSON.stringify(dg), Lang: lang }, function(res) {
                closeLoading();
                $('#txtScript').val(res);
            });
        }
        
        var SetUpPublicAccess = function() {
            $('#lblPublicAccessLink').text('Link: http://diagrams.seaquail.net/Diagram.aspx?ID=' + DG.ID).attr('href', 'http://diagrams.seaquail.net/Diagram.aspx?ID=' + DG.ID)[DG.AllowPublicAccess ? 'show' : 'hide']();
            $('#btnAllowPublicAccess').text((DG.AllowPublicAccess ? 'Disa' : 'A') + 'llow');
            $('#lblAllowPublicAccess').text((DG.AllowPublicAccess ? 'A' : 'Disa') + 'llowed');
        }

        var TogglePublicAccess = function() {
            $.post('/TogglePublicAccess.dg', { ID: DG.ID }, function(res) {
                if (res != 'ERROR:Permission Denied') {
                    DG.AllowPublicAccess = JSON.parse(res);
                    TrackEvent('Diagram', 'Public Access Toggled', 'Diagram ID:' + DG.ID + ":" + (DG.AllowPublicAccess ? 'Yes' : 'No'));
                    SetUpPublicAccess();
                } else {
                    
                }
            });
        }

        var SetReadonly = function(readonly) {
            $('#page')[readonly ? 'addClass' : 'removeClass']('readonly');
        }

        var showProperties = true;
        var DG, CurrentSearch = null;
        $(function() {
            var hasSignedIn = false;

            var cvs = document.getElementById('diagram');
            DG = new DGDiagram(cvs);
            DG.ID = -1;
            var updateCanvasSize = function() {
                var canvas = $(cvs);
                var width = $(window).width();
                var height = $(window).height() - $('#header').height();
                canvas.attr({
                    'width': width,
                    'height': height
                });
                DG.SetW(width);
                DG.SetH(height);

                $('#tool-panel').css('left', (width - 310) + 'px');
            }
            $(window).bind('resize', function() { setTimeout(function() { updateCanvasSize(); }, 250); })
                .bind('beforeunload', function() { return "Are you sure you want to leave this page? Be sure that you've saved your changes! "; });
            updateCanvasSize();

            var printWindow = null;

            var GetJSONDate = function(d) {
                return new Date(parseInt(d.replace(/\/Date\(|\/|\)/gi, '')));
            }

            $('.dropdownmenu').mouseenter(function() {
                $(this).stop().animate({ 'margin-top': '-2px' });
            }).mouseleave(function() {
                $(this).stop().animate({ 'margin-top': -($('.dropdownmenu-inner').outerHeight()) + 'px' });
            }).css('margin-top', -($('.dropdownmenu-inner').outerHeight()) + 'px');

            var frmEditTable = $('#edit-table');
            var frmEditCol = $('#edit-column');
            var frmEditGroup = $('#edit-group');

            for (var i in GroupColors) {
                var color = GroupColors[i];
                $('#grp-Color').append('<option value=' + color.B + ' style="background-color: ' + color.B + '; color: ' + color.F + '; ">Sample Text</option>');
            }

            var GetDataTypeByName = function(name) {
                for (var i in _DataTypes) {
                    if (_DataTypes[i].Name && _DataTypes[i].Name == name) {
                        return _DataTypes[i];
                    }
                }
            }

            var colDataType_Change = function() {
                DataTypeUpdated();
                SaveColumnDataType(GetSelectedColumn());
            }

            var DataTypeUpdated = function(ddl) {
                var datatype = GetDataTypeByName($('#col-DataType').val());
                $('.col-cond').hide();
                for (var i in datatype) {
                    if (datatype[i]) {
                        $('.col-cond-' + i).show();
                    }
                }
            }

            $('#col-DataType').click(colDataType_Change).change(colDataType_Change);
            $('#tbl-Name').keyup(function() { _EditTable.SetName($('#tbl-Name').val()); _EditTable.Invalidate(); });

            $('#col-Nullable').click(function() { SaveColumnNullable(GetSelectedColumn()); });
            $('#col-IsPrimary,#col-IsIdentity').click(function() { SaveColumnDataType(GetSelectedColumn()); });
            $('#col-Length,#col-Precision,#col-Scale').change(function() { SaveColumnDataType(GetSelectedColumn()); });
            $('#col-Name').bind('change keyup', function() { SaveColumnName(GetSelectedColumn()); });
            $('#grp-Label').bind('change keyup', function() { SaveGroupLabel(DG.SelectedGroup); });
            $('#grp-Color').bind('click', function() { SaveGroupColor(DG.SelectedGroup); });

            var InvalidateColList = function() {
                var t = GetSelectedTable();
                if (t) {
                    t.GetColList().Invalidate();
                }
            }

            var BindTableEdit = function() {
                var t = GetSelectedTable();
                $('#tbl-Name').val(t.GetName());
            };
            var BindGroupEdit = function() {
                var g = DG.SelectedGroup;
                $('#grp-Label').val(g.GetLabel());
                $('#grp-Color').val(g.GetColor().B);
                $('#grp-Color').css('background-color', g.GetColor().B);
            };
            var BindColumnDataType = function(c) {
                $('#col-DataType').val(c.DataType.Name);
                $('#col-IsPrimary').attr('checked', c.IsPrimary);
                $('#col-IsIdentity').attr('checked', c.IsIdentity);
                $('#col-Scale').val(c.Scale);
                $('#col-Precision').val(c.Precision);
                $('#col-Length').val(c.Length);
                DataTypeUpdated();
            };
            var BindColumnEdit = function() {
                var c = GetSelectedColumn();
                $('#col-Name').val(c.Name);
                $('#col-Nullable').attr('checked', c.Nullable);
                $('#col-DefaultValue').val(c.DefaultValue);
                BindColumnDataType(c);
            };
            var SaveColumnName = function(col) {
                if (col) {
                    col.Name = $('#col-Name').val();
                    InvalidateColList();
                }
            };
            var SaveColumnNullable = function(col) {
                col.Nullable = $('#col-Nullable').is(':checked');
                InvalidateColList();
            };
            var SaveColumnDataType = function(col) {
                if (col != null) {
                    var type = GetDataTypeByName($('#col-DataType').val());
                    col.DataType = type;
                    if (type.HasLength)
                        col.Length = $('#col-Length').val();
                    if (type.HasPrecision) {
                        col.Precision = $('#col-Precision').val();
                        col.Scale = $('#col-Scale').val();
                    }
                    if (type.IDOK)
                        col.IsIdentity = $('#col-IsIdentity').is(':checked')
                    if (type.PKOK)
                        col.IsPrimary = $('#col-IsPrimary').is(':checked');
                    InvalidateColList();
                }
            };
            var SaveColumnEdit = function(colIdx) {
                var c = GetSelectedColumn(colIdx);
                SaveColumnDataType(c);
                SaveColumnName(c);
                SaveColumnNullable(c);
                c.DefaultValue = $('#col-DefaultValue').val();
                InvalidateColList();
            };
            var SaveGroupLabel = function(grp) {
                if (grp != null) {
                    grp.SetLabel($('#grp-Label').val());
                }
            }
            var SaveGroupColor = function(grp) {
                if (grp != null) {
                    grp.SetColor(GroupColors[$('#grp-Color option:selected').prevAll().size()]);
                    $('#grp-Color').css('background-color', $('#grp-Color option:selected').val());
                }
            }

            var ShowProperties = function(editForm) {
                if (showProperties) {
                    editForm.show();
                }
            };



            var _EditTable = null;
            var _EditColIndex = null;
            var _EditGroup = null;
            var _ColCount = 0;
            var _TextEditIndex = { Row: -1, Col: -1 };
            var _EditSelections = function() {
                var editTable = GetSelectedTable();
                var tableChanged = editTable != _EditTable;
                if (tableChanged) {
                    if (_EditTable == null)
                        ShowProperties(frmEditTable);
                    else if (editTable == null)
                        frmEditTable.hide();

                    _EditTable = editTable;
                    if (editTable != null)
                        BindTableEdit();
                }

                var editGroup = DG.SelectedGroup,
                    groupChanged = editGroup != _EditGroup;
                if (groupChanged) {
                    if (_EditGroup == null)
                        ShowProperties(frmEditGroup);
                    else if (editGroup == null)
                        frmEditGroup.hide();

                    _EditGroup = editGroup;
                    if (editGroup != null)
                        BindGroupEdit();
                }

                if (editTable != null) {
                    var editindex = editTable.GetColList().GetEditIndex();
                    if (_TextEditIndex != editindex) {
                        if (_TextEditIndex.Col != -1) {
                            var colStyle = editTable.GetColList().GetColStyles()[_TextEditIndex.Col];
                            var col = GetSelectedColumn(_TextEditIndex.Row);
                            if (colStyle.Name == 'Name') {
                                $('#col-Name').val(col.Name);
                            } else if (colStyle.Name == 'Type') {
                                BindColumnDataType(col);
                            }
                        }
                        _TextEditIndex = editindex;
                    }
                }

                if (editTable != null) {
                    var editCol = GetSelectedColIndex();
                    if (editCol != _EditColIndex) {
                        if (_EditColIndex == null)
                            frmEditCol.show();
                        else if (editCol == null)
                            frmEditCol.hide();

                        if (_EditColIndex != null
                            && _ColCount == editTable.GetColumns().length
                            && !tableChanged) {
                            SaveColumnEdit(_EditColIndex);
                        }

                        _ColCount = editTable.GetColumns().length;
                        _EditColIndex = editCol;
                        if (editCol != null)
                            BindColumnEdit();
                    }
                }
            }

            var saveSnapshot = function() {
                if (DG.ID > 0) {
                    var dg = GetJSONDG();

                    var name = prompt('Enter a description of this snapshot.');
                    if (name) {
                        $.ajax({
                            url: '/AddSnapshot.dg',
                            cache: false,
                            type: 'POST',
                            data: { Snapshot: JSON.stringify(dg), Name: name, DGID: DG.ID },
                            success: function(id) {
                                DG.ID = id;
                                alert('Snapshot Saved');
                            },
                            error: function(req, status, err) {
                                alert('An error occured: ' + err);
                            }
                        });
                    } else if (name == '') {
                        alert('You must enter a name for the snapshot!');
                        saveSnapshot();
                    }
                } else {
                    saveDiagram();
                }
            }

            var saveDiagram = function() {
                var dg = GetJSONDG();

                $.ajax({
                    url: '/SaveDiagram.dg',
                    cache: false,
                    type: 'POST',
                    data: { Diagram: JSON.stringify(dg) },
                    success: function(id) {
                        if (id == 'ERROR:Permission Denied')
                            alert('This diagram was shared with you for viewing. You are not allowed to modify it. ');
                        else {
                            DG.ID = id;
                            SetReadonly(false);
                            alert('Diagram Saved');
                        }
                    },
                    error: function(req, status, err) {
                        alert('An error occured: ' + err);
                    }
                });
            }

            var showPopup = function(popup, title, menuitems) {
                closeLoading();
                $('#popup-inner > div').hide();
                $('#' + popup).show();
                $('#popupCont').show();
                $('#popupTitle').text(title);
                if (menuitems) {
                    $('#' + popup + ' .toolstrip-inner').empty();
                    for (var i in menuitems)
                        $('#' + popup + ' .toolstrip-inner').append($('<a href="#">' + menuitems[i].Name + '</a>').click(menuitems[i].Click));
                }
            }
            var hidePopup = function() {
                $('#popupCont').hide();
            }

            $('#popupBG').click(function() {
                hidePopup();
            });


            var newTCount = 0;
            var _Clipboard = null;

            // Toolstrip menu item click and setup for shortcut keys
            var _ShortCuts = [];
            $('.toolstrip a, .dropdownmenu a').click(function() {
                if ($(this).hasClass('disable')
                    || ($(this).hasClass('no-readonly') && $('#page').hasClass('readonly')))
                    return;

                var menuitem = $('img', this).attr('alt') || $(this).text();
                _ExecuteMenuCommand(menuitem, this);
            }).each(function() {
                if ($(this).attr('data-shortcut')) {
                    var parts = $(this).attr('data-shortcut').split(/\|/);
                    var shortcut = {
                        MenuItem: $(this),
                        Shift: false,
                        Ctrl: false
                    };
                    for (var i in parts) {
                        switch (parts[i]) {
                            case 'ctrl': shortcut.Ctrl = true; break;
                            case 'shift': shortcut.Shift = true; break;
                            case 'delete': shortcut.Key = 46; break;
                            default: shortcut.Key = parts[i].charCodeAt(0); break;
                        }
                    }

                    _ShortCuts.push(shortcut);
                }
            });

            $(document).keydown(function(e) {
                for (var i in _ShortCuts) {
                    if (_ShortCuts[i].Shift == e.shiftKey
                        && _ShortCuts[i].Ctrl == e.ctrlKey
                        && _ShortCuts[i].Key == e.which) {
                        if ($('input:focus').size() == 0) {
                            _ShortCuts[i].MenuItem.click();
                            e.preventDefault();
                        }
                    }
                }
            });

            $('#txtFind').keydown(function(e) {
                var findNext = e.which == 13 || e.which == 27;

                if (e.which == 27) {
                    $(this).val('');
                    $('a').focus();
                    _ExecuteMenuCommand('Find Text', $('img[alt="Find Text"]').parent());
                }

                if (findNext)
                    _ExecuteMenuCommand('Find Next', $('#find a'));
            });

            var _ExecuteMenuCommand = function(menuitem, item) {
                switch (menuitem) {
                    case "Sign In":
                        {
                            var extensions = {
                                'openid.ns.ax': 'http://openid.net/srv/ax/1.0',
                                'openid.ax.mode': 'fetch_request',
                                'openid.ax.type.email': 'http://axschema.org/contact/email',
                                'openid.ax.required': 'email',
                                //'openid.ns.oauth': 'http://specs.openid.net/extensions/oauth/1.0',
                                //'openid.oauth.consumer': 'seaquail.cz.cc',
                                //'openid.oauth.scope': 'http://www.google.com/m8/feeds/',
                                'openid.ui.icon': 'true'
                            };
                            var googleOpener = popupManager.createPopupOpener({
                                'realm': '<%= Request.Url.GetRoot() %>',
                                'opEndpoint': 'https://www.google.com/accounts/o8/ud',
                                'returnToUrl': '<%= Request.Url.GetRoot() %>SignIn.aspx',
                                'onCloseHandler': function() {
                                    // when the login dialog is closed, retrieve the
                                    // logged in user details
                                    $.get('/GetCurrentUser.dg', function(res) {
                                        var user = JSON.parse(res);
                                        if (user) {
                                            TrackEvent('Diagram', 'Sign In', 'UserID:' + user.ID);
                                            $('#divWelcome').text('Welcome, ' + user.Name);
                                            $('.dropdownmenu a.disable').removeClass('disable');
                                            $('.dropdownmenu').css('margin-top', '-1px');
                                            hasSignedIn = true;
                                            userID = user.ID;
                                        }
                                    });
                                },
                                'shouldEncodeUrls': true,
                                'extensions': extensions
                            });
                            googleOpener.popup(450, 500);
                        }
                        break;
                    case "Help":
                        {
                            window.open('DiagramHelp.aspx', 'Sea Quail Help', 'width=420,height=500');
                        }
                        break;
                    case "Properties":
                        {
                            if (showProperties = !showProperties) {
                                $(item).addClass('hover');
                                if (DG.SelectedTable)
                                    $('#edit-table').show();
                            } else {
                                $('#edit-table').hide();
                                $(item).removeClass('hover');
                            }
                        }
                        break;
                    case "Overview":
                        {
                            TrackEvent('Diagram', 'Used Overview');
                            if ($('#overview').is(':visible')) {
                                $('#overview').hide();
                                $(item).removeClass('hover');
                            } else {
                                $('#overview').show();
                                $(item).addClass('hover');
                            }
                        }
                        break;
                    case "New Diagram":
                        if (DG.GetChildren().length < 1 || confirm('This will clear the current diagram. Are you sure you want to create a new one? ')) {
                            TrackEvent('Diagram', 'Cleared Diagram');
                            DG.Reset();
                            SetReadonly(false);
                            DG.SetName('Untitled Diagram 1');
                            DG.ID = -1;
                            newTCount = 0;
                            UpdateDGName();
                        }
                        break;
                    case "Share Diagram":
                        {
                            if (DG.ID < 1) {
                                alert('The diagram must be saved before it can be shared. ');
                                return;
                            }

                            SetUpPublicAccess();

                            var showSharing = function(shares) {
                                $('#Sharing ul').empty();
                                for (var i in shares) {
                                    var sh = shares[i];
                                    $('#Sharing ul').append('<li><span class="desc">' + sh.Email + '</span><span class="owner">&nbsp;</span><span class="options"><a href="#" class="delete" title="Unshare"><img src="/Media/Images/Icons/cross.png" alt="Unshare" title="Unshare" /></a></span></li>');
                                }

                                $('#Sharing a.delete').click(function() {
                                    var self = this;
                                    if (confirm('Are you sure you want to stop sharing with this person?')) {
                                        $.get('/UnshareDiagram.dg', { Email: $('.desc', $(this).parents('li:first')).text(), DiagramID: DG.ID }, function(res) {
                                            TrackEvent('Diagram', 'Unshare');
                                            showSharing(JSON.parse(res));
                                        });
                                    }
                                });
                            };

                            $.get('/GetSharing.dg', function(res) {
                                showSharing(JSON.parse(res));
                            });
                            showPopup('Sharing', 'Share Diagram', [
                                {
                                    Name: 'Share with Another',
                                    Click: function() {
                                        var email = prompt('Enter a Google e-mail address for the person with whom you\'d like to share this diagram.\r\nLet this person know that you\'ve shared the diagram with them. They\'ll be able to view your diagram by opening it from the \'My Diagrams\' panel. ');
                                        if (email) {
                                            $.get('ShareDiagram.dg', { Email: email, DiagramID: DG.ID }, function(res) {
                                                if (res == 'ERROR:Permission Denied')
                                                    alert('You can not share this diagram, because you are not the owner. ');
                                                else {
                                                    TrackEvent('Diagram', 'Share');
                                                    showSharing(JSON.parse(res));
                                                }
                                            });
                                        }
                                    }
                                }
                            ]);
                        }
                        break;
                    case "My Diagrams":
                        {
                            var showMyDiagrams = function() {
                                $('#MyDiagrams .popup-grid ul').empty();
                                showLoading($('#MyDiagrams ul'));
                                $.get('/GetDiagrams.dg', function(res) {
                                    closeLoading();
                                    var dgs = JSON.parse(res);
                                    for (var i in dgs) {
                                        var dg = dgs[i];
                                        var name = dg.Name == null || dg.Name.length == 0 ? '[Unnamed Diagram]' : dg.Name;
                                        $('#MyDiagrams ul').append('<li data-key="' + dgs[i].ID + '"><span class="desc">' + name + '</span><span class="moddate">' + GetJSONDate(dgs[i].ModifyDate).format('m/d/yyyy h:mmTT') + '</span><span class="options"><a href="#" class="delete" title="Delete Diagram"><img src="/Media/Images/Icons/cross.png" alt="Delete" title="Delete" /></a></span></li>');
                                    }
                                    $('#MyDiagrams .desc').click(function() {
                                        hidePopup();
                                        TrackEvent('Diagram', 'Loaded Diagram');
                                        LoadDiagram($(this).parents('li:first').attr('data-key'));
                                    });
                                    $('#MyDiagrams a.delete').click(function() {
                                        var self = this;
                                        if (confirm('Are you sure you want to delete this diagram?')) {
                                            $.get('/DeleteDiagram.dg', { ID: $(this).parents('li:first').attr('data-key') }, function(res) {
                                                if (res == 'OK') {
                                                    $(self).parent().remove();
                                                } else {
                                                    alert('Error: ' + res);
                                                }
                                            });
                                        }
                                    });
                                });
                            }, showSharedDiagrams = function() {
                                $('#MyDiagrams .popup-grid ul').empty();
                                showLoading($('#MyDiagrams ul'));
                                $.get('/GetSharedDiagrams.dg', function(res) {
                                    closeLoading();
                                    var dgs = JSON.parse(res);
                                    for (var i in dgs) {
                                        var dg = dgs[i], name = dg.Diagram == null || dg.Diagram.length == 0 ? '[Unnamed Diagram]' : dg.Diagram;
                                        $('#MyDiagrams ul').append('<li data-key="' + dgs[i].DiagramID + '"><span class="desc">' + name + '</span><span class="owner">Owner: ' + dg.Owner + '</span></li>');
                                    }
                                    $('#MyDiagrams .desc').click(function() {
                                        hidePopup();
                                        TrackEvent('Diagram', 'Loaded Shared Diagram');
                                        LoadDiagram($(this).parents('li:first').attr('data-key'));
                                    });
                                });
                            };

                            showPopup('MyDiagrams', 'My Diagrams',
                            [
                                { Name: 'My Diagrams', Click: function() { showMyDiagrams(); } },
                                { Name: 'Other\'s Diagrams', Click: function() { showSharedDiagrams(); } }
                            ]);

                            showMyDiagrams();
                        }
                        break;
                    case "Add Group":
                        {
                            var g = new DGGroup(DG),
                                cx = -DG.GetOffsetX() + $(window).width() / 2,
                                cy = -DG.GetOffsetY() + $(window).height() / 2;

                            g.SetLabel('New Group');
                            g.SetDim({
                                X: cx - 300,
                                Y: cy - 300,
                                W: 600,
                                H: 600
                            });

                            DG.SelectedGroup = g;
                            DG.UpdateTableGroups();
                            TrackEvent('Diagram', 'Group Added');
                        }
                        break;
                    case "Add Table":
                        {
                            var name = prompt('Enter a name for the new table.', 'Table_' + newTCount++);
                            if (name) {
                                var t = new DGTable(DG);
                                t.SetDim({
                                    X: -DG.GetOffsetX() + $(window).width() / 2 - 166,
                                    Y: -DG.GetOffsetY() + $(window).height() / 2 - 100,
                                    W: 332,
                                    H: 200
                                });
                                t.SetName(name);
                                if (DG.SelectedTable)
                                    DG.SelectedTable.Deselect();
                                DG.SelectedTable = t;
                                t.SelectedColumn = 0;
                            } else {
                                newTCount--;
                            }
                        }
                        break;
                    case "Add Tables":
                        {
                            var count = parseInt(prompt('Enter number of tables to add')) || 0;
                            for (var i = 0; i < count; i++) {
                                if (DG.SelectedTable)
                                    DG.SelectedTable.Deselect();
                                var t = new DGTable(DG);
                                t.SetDim({
                                    X: -DG.GetOffsetX() + ($(window).width() / 2) - 166 + (i * 20),
                                    Y: -DG.GetOffsetY() + ($(window).height() / 2) - 100 + (i * 20),
                                    W: 332,
                                    H: 200
                                });
                                t.SetName('Table_' + newTCount++);

                                if (i == count - 1) {
                                    DG.SelectedTable = t;
                                    t.SelectedColumn = 0;
                                }
                            }
                        }
                        break;
                    case "Create Script":
                        {
                            TrackEvent('Diagram', 'Generated Create Script');
                            $('#txtScript').val('');
                            showPopup('Script', 'Generate Create Script');
                        }
                        break;
                    case "Save Diagram":
                        {
                            TrackEvent('Diagram', 'Saved Diagram');
                            saveDiagram();
                        }
                        break;
                    case "Save a Copy":
                        {
                            if (confirm('Do you want to make your own editable copy of this diagram?')) {
                                DG.ID = 0;
                                saveDiagram();
                            }
                        }
                        break;
                    case "Print Diagram":
                        {
                            TrackEvent('Diagram', 'Printed Diagram');
                            DG.SelectedTable = null;
                            printWindow = window.open('DiagramPrint.aspx', null);
                            var tries = 0;
                            var onload = function() {
                                if (printWindow.Load && !printWindow.Loaded) {
                                    printWindow.Load(DG);
                                } else if (tries < 500) {
                                    tries++;
                                    setTimeout(onload, 50);
                                }
                            }
                            onload();
                        }
                        break;
                    case "Create Snapshot":
                        {
                            TrackEvent('Diagram', 'Created Snapshot');
                            saveSnapshot();
                        }
                        break;
                    case "Create Change Script":
                        {
                            var btn = item;
                            showSnapshots(item, function() {
                                $('#snapshot-list a').click(function() {
                                    var id = $(this).attr('data-key');
                                    $.ajax({
                                        url: '/DoChangeScript.dg',
                                        cache: false,
                                        type: 'POST',
                                        data: { SnapShotID: id, Diagram: JSON.stringify(GetJSONDG()) },
                                        success: function(res) {
                                            $('#show-snapshots').hide();
                                            $(btn).removeClass('hover');

                                            $('#txtScript').show();
                                            $('#txtScript').val(res);
                                            $('#script-cont').show();
                                        }
                                    });
                                });
                            });
                            $('#snapshot-message').html('A script will be generated which would alter the schema represented by the snapshot selected below to reflect the current schema. ');
                        }
                        break;
                    case "Diagram Snapshots":
                        {
                            showPopup('Snapshots', "Snapshots for " + DG.GetName());
                            showLoading($('#Snapshots ul'));
                            $('#Snapshots ul').empty();
                            $.get('/GetSnapshots.dg', { DGID: DG.ID }, function(res) {
                                closeLoading();
                                var dgs = JSON.parse(res);
                                for (var i in dgs) {
                                    var dg = dgs[i];
                                    var name = dg.Name == null || dg.Name.length == 0 ? '[Unnamed Diagram]' : dg.Name;
                                    $('#Snapshots ul').append('<li data-key="' + dgs[i].ID + '"><span class="desc">' + name + '</span><span class="moddate">' + GetJSONDate(dgs[i].ModifyDate).format('yyyy-mm-dd h:MMTT') + '</span><span class="options"><a href="#" class="delete" title="Delete Snapshot"><img src="/Media/Images/Icons/cross.png" alt="Delete" title="Delete" /></a></span></li>');
                                }
                                $('#Snapshots .desc').click(function() {
                                    var id = $(this).parents('li:first').attr('data-key');
                                    $.get('/LoadSnapshot.dg', { ID: id }, function(res) {
                                        var snap = JSON.parse(res);
                                        var def = JSON.parse(snap.DiagramData);
                                        hidePopup();
                                        LoadDiagramDef(def);
                                    });
                                });
                                $('#Snapshots a.delete').click(function() {
                                    var self = this;
                                    if (confirm('Are you sure you want to delete this snapshot?')) {
                                        $.get('/DeleteSnapshot.dg', { ID: $(this).parents('li:first').attr('data-key') }, function(res) {
                                            if (res == 'OK') {
                                                $(self).parents('li:first').remove();
                                            } else {
                                                alert(res);
                                            }
                                        });
                                    }
                                });
                            });
                        }
                        break;
                    case "Copy Table":
                        {
                            _Clipboard = DG.SelectedTable;
                            $('.enable-TableCopied').removeClass('disable');
                        }
                        break;
                    case "Paste Table":
                        {
                            if (_Clipboard != null && _Clipboard.GetType() == 'DGTable') {
                                var t = new DGTable(DG);
                                t.SetDim({
                                    X: -DG.GetOffsetX() + ($(window).width() / 2) - 166,
                                    Y: -DG.GetOffsetY() + ($(window).height() / 2) - 100,
                                    W: 332,
                                    H: 200
                                });
                                t.SetName(_Clipboard.GetName());
                                t.GetColumns().splice(0, 1);
                                var cols = _Clipboard.GetColumns();
                                for (var i in cols) {
                                    var col = {};
                                    for (var k in cols[i]) {
                                        col[k] = cols[i][k];
                                    }
                                    col.GUID = guid();

                                    t.GetColumns().push(col);
                                }

                                DG.SelectedTable = t;
                                t.SelectedColumn = null;
                            }
                        }
                        break;
                    case "Delete Item":
                        {
                            var selectedItem = DG.SelectedGroup ? DG.SelectedGroup
                                : DG.SelectedTable ? (DG.SelectedTable.SelectedColumn != null ? { GetType: function() { return 'DGColumn'; } } : DG.SelectedTable)
                                : null;
                            if (selectedItem != null) {
                                if (confirm('Are you sure you want to delete the selected ' + selectedItem.GetType().replace('DG', '') + '?')) {
                                    switch (selectedItem.GetType()) {
                                        case 'DGTable':
                                            selectedItem.Delete();
                                            break;
                                        case 'DGColumn':
                                            DG.SelectedTable.GetColList().DeleteColumn(DG.SelectedTable.SelectedColumn);
                                            break;
                                        case 'DGGroup':
                                            DG.SelectedGroup.Delete();
                                            break;
                                    }
                                    DG.UpdateTableGroups();
                                }
                            }
                        }
                        break;
                    case "Rename Diagram":
                        DG.SetName(prompt('Enter Diagram Name', DG.GetName()));
                        UpdateDGName();
                        break;
                    case "Zoom In":
                        DG.ZoomIn();
                        break;
                    case "Zoom Out":
                        DG.ZoomOut();
                        break;
                    case "Find Next":
                        {
                            var text = $('#txtFind').val();
                            if (!CurrentSearch || CurrentSearch.Text != text) {
                                CurrentSearch = {
                                    Text: text,
                                    Results: [],
                                    Index: -1
                                };

                                var items = DG.GetChildren(),
                                    rg = new RegExp(text, 'i');
                                for (var i in items) {
                                    var table = items[i];
                                    if (table.GetType() == 'DGTable') {
                                        table.SetHighlight(text != '' && table.GetName().search(rg) >= 0 ? true : false);
                                        var cols = table.GetColumns(),
                                            hidxs = [];
                                        for (var j = 0; j < cols.length; j++) {
                                            var col = cols[j];
                                            if (text != '' && col.Name.search(rg) >= 0)
                                                hidxs.push(j);
                                        }
                                        table.GetColList().SetHighlights(hidxs);

                                        if (table.HasHighlights())
                                            CurrentSearch.Results.push(table);
                                    }
                                }
                            }

                            if (CurrentSearch.Text == text) {
                                CurrentSearch.Index = (CurrentSearch.Index + 1) % CurrentSearch.Results.length;
                                DG.SetCenter(CurrentSearch.Results[CurrentSearch.Index].GetCenter());
                            }
                        }
                        break;
                    case "Find Text":
                        {
                            if ($('#find').is(':visible')) {
                                $('#find').hide();
                                $(item).removeClass('hover');
                            } else {
                                setTimeout(function() { $('#txtFind').focus(); }, 10);
                                $('#find').show();
                                $(item).addClass('hover');
                            }
                        }
                        break;
                    //                    case "Scroll Right":                                                                                                                                                                                         
                    //                        DG.SetOffset(new Pt(DG.GetOffsetX() - 50, DG.GetOffsetY()));                                                                                                                                                                                         
                    //                        break;                                                                                                                                                                                         
                    //                    case "Scroll Left":                                                                                                                                                                                         
                    //                        DG.SetOffset(new Pt(DG.GetOffsetX() + 50, DG.GetOffsetY()));                                                                                                                                                                                         
                    //                        break;                                                                                                                                                                                         
                    //                    case "Add Table Group":                                                                                                                                                                                         
                    //                        new DGGroup(DG);                                                                                                                                                                                         
                    //                        break;                                                                                                                                                                                         
                    //                    case "Close":                                                                                                                                                                                         
                    //                        $(this).parents('.tool-strip-dropdown:first').hide();                                                                                                                                                                                         
                    //                        break;                                                                                                                                                                                         
                    //                    case "About":                                                                                                                                                                                         
                    //                        $('#about-cont').show();                                                                                                                                                                                         
                    //                        break;                                                                                                                                                                                         
                    //                    case "Close About":                                                                                                                                                                                         
                    //                        $('#about-cont').hide();                                                                                                                                                                                         
                    //                        break;                                                                                                                                                                                         
                }
            }

            _UpdateToolstrip = function() {
                if (_EditTable != GetSelectedTable())
                    $('.enable-TableSelected')[(DG.SelectedTable ? 'remove' : 'add') + 'Class']('disable');

                $('.enable-ItemSelected')[(DG.SelectedTable || DG.SelectedGroup ? 'remove' : 'add') + 'Class']('disable');
            }



            // OVERVIEW =======================================================
            //(function() {
            var _Ovr = document.getElementById('diagram-overview');
            var _OvrCtx = new DrawContext(_Ovr);
            _OvrCtx.ctx().save();
            _Ovr.Mse = { Pos: new Pt(0, 0), Down: null };
            var _Ow = 220;
            var _Oh = 220;

            var doc_MouseUp = function(e) {
                _Ovr.Mse.Dragging = false;
            }
            $(document).mouseup(doc_MouseUp);

            var _Ovr_MouseDown = function(e) {
                var ovrOff = $(this).offset();
                _Ovr.Mse.Down = { Button: e.which, Pos: new Pt(_Ovr.Mse.Pos.X - ovrOff.Left, _Ovr.Mse.Pos.Y - ovrOff.Top) };
                _Ovr.Mse.Dragging = true;
                _Ovr.NeedReposition = true;
            }
            var _Ovr_MouseMove = function(e) {
                _Ovr.Mse.Pos.X = e.pageX;
                _Ovr.Mse.Pos.Y = e.pageY;
            }
            $(_Ovr).mousedown(_Ovr_MouseDown).mousemove(_Ovr_MouseMove);

            var _DrawOverview = function() {
                var items = [];
                var minPt = new Pt(0, 0);
                var maxPt = new Pt(DG.GetW(), DG.GetH());
                var ti = 0;
                var dgchildren = DG.GetChildren();

                // get the list of tables and groups and the min and max points to 
                // show in the overview
                for (var i in dgchildren) {
                    if (dgchildren[i].GetType() == 'DGTable'
                        || dgchildren[i].GetType() == 'DGGroup') {
                        items.push(dgchildren[i]);

                        minPt.X = Math.min(minPt.X, items[ti].GetX());
                        minPt.Y = Math.min(minPt.Y, items[ti].GetY());
                        maxPt.X = Math.max(maxPt.X, items[ti].GetX() + items[ti].GetW());
                        maxPt.Y = Math.max(maxPt.Y, items[ti].GetY() + items[ti].GetH());
                        ti++;
                    }
                }

                _OvrCtx.ctx().restore();
                _OvrCtx.ctx().save();

                _OvrCtx.ctx().clearRect(0, 0, _Ow, _Oh);
                _OvrCtx.FillRect(new Rect(0, 0, _Ow, _Oh), '#ffffff');
                var totalW = Math.max(maxPt.X - minPt.X, maxPt.Y - minPt.Y);
                var scale = (_Ow / totalW);

                _OvrCtx.ctx().scale(scale, scale);

                if (_Ovr.NeedReposition || _Ovr.Mse.Dragging) {
                    _Ovr.NeedReposition = false;
                    var ovrOff = $(_Ovr).offset();
                    var myPt = new Pt(_Ovr.Mse.Pos.X - ovrOff.left, _Ovr.Mse.Pos.Y - ovrOff.top);
                    var px = (myPt.X / _Ow);
                    var py = (myPt.Y / _Oh);

                    DG.SetOffset(new Pt(
                        (px * totalW + minPt.X) * -1 + (DG.GetW() / 2) / DG.GetZoom(),
                        (py * totalW + minPt.Y) * -1 + (DG.GetH() / 2) / DG.GetZoom()
                    ));
                }

                var tableborderPen = new Pen('#808080', .5 / scale);
                for (var i in items) {
                    var rect = new Rect(items[i].GetX() - minPt.X, items[i].GetY() - minPt.Y, items[i].GetW(), items[i].GetH());
                    if (items[i].GetType() == 'DGGroup') {
                        var color = items[i].GetColor();
                        _OvrCtx.FillEllipse(rect, color.B);
                        _OvrCtx.DrawEllipse(rect, new Pen(color.F, .5 / scale));
                    } else if (items[i].GetType() == 'DGTable') {
                        var color = items[i].HasHighlights() ? 'Orange' : '#dfdfdf';
                        _OvrCtx.FillRect(rect, color);
                        _OvrCtx.DrawRect(rect, tableborderPen);
                    }
                }
                var cvsRect = new Rect(0, 0, _Ow / scale, _Oh / scale)
                var viewArea = new Rect(Math.round(-DG.GetOffsetX()) - minPt.X, Math.round(-DG.GetOffsetY()) - minPt.Y, DG.GetW() / DG.GetZoom(), DG.GetH() / DG.GetZoom())
                _OvrCtx.DrawRect(viewArea, new Pen('#808080', 1.5 / scale));

                var fill = 'rgba(0, 0, 0, 0.25)';
                _OvrCtx.FillRect(new Rect(0, 0.5, cvsRect.W, viewArea.Y), fill);
                _OvrCtx.FillRect(new Rect(0 + minPt.X, viewArea.Y, viewArea.X - minPt.X, viewArea.H), fill);
                _OvrCtx.FillRect(new Rect(viewArea.X + viewArea.W, viewArea.Y, cvsRect.W - (viewArea.X + viewArea.W), viewArea.H), fill);
                _OvrCtx.FillRect(new Rect(0, viewArea.Y + viewArea.H, cvsRect.W, cvsRect.H - (viewArea.Y + viewArea.H)), fill);
            }
            //})();
            // END OVERVIEW =====================================================

            // Do draw loop
            o:-function drawLoop() {
				window.requestAnimationFrame(drawLoop);
                try {
                    DG.Draw();
                } catch (err) {
                }

                try {
					_UpdateToolstrip();
					_EditSelections();
                } catch (err) {
                }

                try {
                    _DrawOverview();
                } catch (err) {
                }
            }();

            var preserveSession;
            (preserveSession = function() {
                if (hasSignedIn) {
                    $.get('PreserveSession.dg', function(res) {
                        if (res != 'true') {
                            hasSignedIn = false;
                            $('.can-disable').addClass('disable');
                            if (confirm('Your session has timed out, but don\'t worry. You won\'t lose your work if you sign in again and save. Would you like to sign in now?')) {
                                _ExecuteMenuCommand('Sign In', null);
                            }
                        }
                    });
                } else {

                }
                setTimeout(preserveSession, 1000 * 60 * 5);
            })();

            LoadDiagram('<%= _OnLoadDiagram %>');
        });
    </script>
</asp:Content>

<script type="text/C#" runat="server">
    string _OnLoadDiagram = "WelcomeDiagram";
    
    protected override void OnLoad(EventArgs e)
    {
        if (Request.Url.GetRoot().StartsWith("http://www."))
        {
            Response.Redirect(Request.Url.ToString().Replace("www.", ""));
        }

        if (!IsPostBack)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["ID"]))
            {
                Int64 diagramID;
                if (Int64.TryParse(Request.QueryString["ID"], out diagramID))
                {
                    try
                    {
                        DGDiagram dg = DGDiagram.ByID(diagramID);
                        if (dg.AllowPublicAccess)
                        {
                            _OnLoadDiagram = diagramID.ToString();
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    catch
                    {
                        Response.Redirect("Diagram.aspx");
                    }
                }
            }
        }
       
        base.OnLoad(e);
    }
</script>

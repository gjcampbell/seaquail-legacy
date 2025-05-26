export function DiagramEditorTool() {
    return (
<div id="page">
    <div id="header">
        <div class="dropdownmenu">
            <div class="dropdownmenu-inner">
                <div id="divWelcome"></div>
                <a href="#"><i class="fa fa-sitemap" data-action="New Diagram"></i> New Diagram</a>
                <a href="#" data-shortcut="ctrl|S" class="disable can-disable no-readonly" title="Save Diagram (Ctrl + S)"><i class="fa fa-save" data-action="Save Diagram"></i> Save Diagram</a>
                <a href="#"><i class="fa fa-file-code-o" data-action="Create Script"></i> Generate Create Script</a>
                <a href="#" data-shortcut="ctrl|O" class="disable can-disable" title="My Diagram (Ctrl + O)"><i class="fa fa-folder-open-o" data-action="My Diagrams"></i> My Diagrams</a>
                <a href="#" data-shortcut="ctrl|P" title="Print Diagram (Ctrl + P)"><i class="fa fa-print" data-action="Print Diagram"></i> Print Diagram</a>
                <div class="divider">&nbsp;</div>
                <a href="#" class="disable can-disable"><i class="fa fa-history" data-action="Diagram Snapshots"></i> Diagram Snapshots</a>
                <a href="#" data-shortcut="ctrl|shift|S" class="disable can-disable no-readonly" title="Create Snapshot (Ctrl + Shift + S)"><i class="fa fa-camera" data-action="Create Snapshot"></i> Create Snapshot</a>
                <a href="#" class="disable always-disable can-disable no-readonly" title="Not Yet Available"><i class="fa fa-file-o" data-action="Generate Change Script"></i> Generate Change Script</a>
                <div class="divider">&nbsp;</div>
                <a href="#" class="disable can-disable no-readonly"><i class="fa fa-users" data-action="Share Diagram"></i> Share Diagram</a>
                <div class="divider">&nbsp;</div>
                <a href="#" id="btnSignIn"><i class="fa fa-google-plus-square" data-action="Sign In"></i> <span>Sign In</span></a>
                <a href="#"><i class="fa fa-question-circle" data-action="Help"></i> Help</a>
                <a href="https://github.com/gjcampbell/seaquail-legacy"><i class="fa fa-home" data-action="Home"></i> Sea Quail Home</a>
            </div>
            <div class="title">Sea Quail<br />Database Diagram Tool</div>
        </div>
        <div class="toolstrip toolstrip-main">
            <div class="toolstrip-inner" style="float: left; margin-left: 195px; ">
                <a href="#" class="no-readonly" title="Add Table"><i class="fa fa-table" data-action="Add Table"></i> Add Table</a>
                <a href="#" class="no-readonly" title="Add Many Tables"><i class="fa fa-plus-circle" data-action="Add Tables"></i></a>
                <a href="#" class="no-readonly" title="Add Group"><i class="fa fa-circle-thin" data-action="Add Group"></i></a>
                <div class="divider no-readonly">&nbsp;</div>
                <a href="#" data-shortcut="ctrl|C" class="no-readonly disable enable-TableSelected" title="Copy Selected Table (Ctrl + C)"><i class="fa fa-copy" data-action="Copy Table"></i></a>
                <a href="#" data-shortcut="ctrl|V" class="no-readonly disable enable-TableCopied" title="Paste Table (Ctrl + V)"><i class="fa fa-paste" data-action="Paste Table"></i></a>
                <a href="#" data-shortcut="delete" class="no-readonly disable enable-ItemSelected" title="Delete Selected Item (Delete)"><i data-action="Delete Item" class="fa fa-minus-circle"></i>&nbsp;</a>
                {/*<a href="#"><img src="/Media/Images/Icons/table_multiple.png" alt="Add Table Group" title="Add Table Group" />Add Table Group</a>-->*/}
                <a href="#" class="for-readonly" title="Save a Copy"><i class="fa fa-save" data-action="Save a Copy"></i></a>
                <div class="divider">&nbsp;</div>
                <span id="diagram-name">Untitled Diagram 1</span><a href="#" class="no-readonly" title="Rename Diagram"><i class="fa fa-pencil" data-action="Rename Diagram"></i>&nbsp;</a>
            </div>
            <div class="toolstrip-inner">
                <a href="#" data-shortcut="ctrl|F" title="Find Text (Ctrl + F)"><i class="fa fa-binoculars" data-action="Find Text"></i> Search</a>
                <a href="#"><i class="fa fa-search" data-action="Overview"></i> Overview</a>
                <a href="#" class="hover no-readonly"><i class="fa fa-bars" data-action="Properties"></i> Properties</a>
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
                        <a href="#" title="Zoom In"><i class="fa fa-search-plus" data-action="Zoom In"></i>&nbsp;</a>
                        <a href="#" title="Zoom Out"><i class="fa fa-search-minus" data-action="Zoom Out"></i>&nbsp;</a>
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
                        <a href="#" title="Find Next"><i class="fa fa-arrow-right" data-action="Find Next"></i> Find Next</a>
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
                    It makes use of the <a href="https://github.com/gjcampbell/seaquail-legacy">Sea Quail SQL Writing Library</a> for MySQL SQL generation.
                </p>
                <p>
                    This should not be expected to work in any browser other than chrome, and even in Chrome
                    it has plenty of bugs, however it's fun to play with.
                </p>
                <p>
                    Thanks for checking it out!<br />
                    <a href="https://github.com/gjcampbell">Gabriel</a>
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
    );
}
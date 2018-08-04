<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DiagramHelp.aspx.cs" Inherits="SeaQuail_DiagramTool.DiagramHelp" %>
<%@ Import Namespace="System.Net.Mail" %>
<%@ Import Namespace="System.Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        body
        {
        	margin: 0px;
        	background-color: #efefef;        	
        }
        
        #header
        {
        	padding: 0px 20px;
        }
        
        #body
        {
        	background-color: #fff;
        }
        
        h1
        {
        	font-size: 1.5em;
        	padding: 10px 0px;
        	color: #66aaff;
        	background-color: #efefef;
        }
        
        h2
        {
        	font-size: 1.2em;
        	border-bottom: solid 1px Gray;
        	background-color: #efefef;
        	color: Gray;
        	padding: 4px 9px;
        }
        
        h3
        {
        	font-size: 1.1em;
        	color: #3366dd;
        }
        
        .section
        {
        	padding: 0px 20px 25px 20px;
        }
    </style>
    <script type="text/C#" runat="server">
        protected void btnSendMessage_Click(object sender, EventArgs e)
        {
            SmtpClient client = new SmtpClient("75.126.150.160");
            client.Credentials = new NetworkCredential("support_request@seaquail.cz.cc", "password");
            try
            {
                client.Send("support_request@seaquail.cz.cc", "sledgebox@gmail.com", "Sea Quail Help Request",
                    "Email: " + txtEmail.Text + "\r\n\r\nMessage: \r\n" + txtMessage.Text);
                txtEmail.Text = "";
                txtMessage.Text = "";
                lblResponse.Text = "Your message has been sent. You can expect an email reply in the next 24 hours. ";
            }
            catch(Exception ex)
            {
                lblResponse.Text = "An error occurred while sending the message! You may email your issue directly to sledgebox@gmail.com. " + ex;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            lblResponse.Text = "";
            
            base.OnLoad(e);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="header">
        <h1><img src="/Media/Images/Icons/help.png" alt="Help" />Sea Quail Database Diagram Tool Help</h1>
        Jump to: <a href="#DiagramTopics">Diagram Topics</a> | <a href="#MouseControls">Mouse Controls</a> | <a href="#KeyboardShortcuts">Keyboard Shortcuts</a>
    </div>
    <div id="body">
        <h2>Request Help</h2>
        <div class="section">
            <p>
                <div>Your Email Address: </div>
                <asp:TextBox runat="server" ID="txtEmail" Columns="66"></asp:TextBox>
            </p>
            <p>
                <div>Message: </div>
                <asp:TextBox runat="server" ID="txtMessage" TextMode="MultiLine" Rows="5" Columns="66"></asp:TextBox>
            </p>
            <p style="text-align: center; ">
                <asp:Label runat="server" ID="lblResponse" ForeColor="Red" Font-Size="Medium"></asp:Label>
            </p>
            <p style="text-align: right; ">
                <asp:Button runat="server" ID="btnSendMessage" Text="Submit" OnClick="btnSendMessage_Click" Width="90" Height="26" />
            </p>
        </div>
        <a name="DiagramTopics"><h2>Diagram Topics</h2></a>
        <div class="section">
            <div>
                <h3>Saving and Loading</h3>
                <p>You must be signed in with a google account in order to save or load diagrams. If you've started a diagram, and want to save, but you've not yet signed in, your changes will not be lost by clicking sign in.</p>
                <p>Name your diagram so that you may identify it easily when you need to load it. The "My Diagrams" dialog will allow you to load previously saved diagrams. </p>
            </div>
            <div>
                <h3>Snapshots</h3>
                <p>Use snapshots for diagram versioning(, and later, when it is available, for generating change scripts). For example, once you've created a diagram, you may want to keep a copy of its current state. This would allow you revert unwanted changes or simply to view a past state of the database. </p>
                <p>Each diagram has its own set of snapshots separate from other diagrams. </p>
                <p>Snapshots are not editable, they may only be created or reloaded. Once you've loaded a snapshot, executing a save will overwrite the current diagram, it will not update the snapshot! </p>
            </div>
            <div>
                <h3>Tables</h3>
                <p>Add a single table to the diagram with the Add Table button in the toolbar. You can also add multiple tables at once, or copy and paste a table. </p>
                <p>Reposition a table by dragging it by its title bar. Rename a table by selecting it, then changing the name field in the properties box. To remove a table, select it, then press delete or click the delete toolbar button. </p>
                <p>Use the overview tool to help move tables over long distances. </p>
            </div>
            <div>
                <h3>Columns</h3>
                <p>Right click the column area of a table to add or insert columns, or while editing a column, tabbing will create new columns quickly. Use the right-click context menu on a column to insert or delete columns. </p>
                <p>Column settings are set in the properties box which appears (if you've not disabled it) when a column is selected. Many settings apply only to certain data types. For example, strings have a length, integers have no length but can be set as an identity(auto increment) column. </p>
                <p>The data types are meant to be database agnostic. This is why they are limited to a few common types. The advantage is that scripts may be generated off of the same diagram for different dbms's. Currently, SQL Server and MySQL are supported. </p>
            </div>
            <div>
                <h3>Relationships</h3>
                <p>Set foreign key relationships between columns by dragging a column or through the context menu of a column. When you've begun setting a foreign key, compatible columns will be highlighted green, all others will be grayed; click a green highlighted column to create the relationship. </p>
                <p>While setting a foreign key, use the overview tool quickly set a relationship to a table across a long distance. </p>
            </div>
        </div>

        <a name="MouseControls"><h2>Mouse Controls</h2></a>
        <div class="section">
            <div>
                <h3>Left Click</h3>
                <p>Click a table or column to select it. Click the background to deselect tables and columns. </p>
                <p>While setting a foreign key, click a green highlighted column to set the foreign key or click elsewhere to cancel setting the foreign key. </p>
            </div>
            <div>
                <h3>Right Click</h3>
                <p>Right click a column to open the column context menu. </p>
            </div>
            <div>
                <h3>Left Mouse Drag</h3>
                <p>Drag the title bar of a table in order to move it. </p>
                <p>Drag a column to begin set a foreign key. You do not need to continue to hold the mouse button once you've begun to set a foreign key. </p>
                <p>Dragging a table near the edge of the diagram will pan your view. </p>
            </div>
            <div>
                <h3>Middle Mouse Drag</h3>
                <p>Drag with the middle mouse button move about the diagram. </p>
            </div>
            <div>
                <h3>Mouse Wheel</h3>
                <p>Mouse wheel up to zoom in, down to zoom out. </p>
            </div>
        </div>
        
        <a name="KeyboardShortcuts"><h2>Keyboard Shortcuts</h2></a>
        <div class="section">
            <div>
                <h3>Save Diagram (Ctrl + S)</h3>
                <p>Save the diagram. </p>
            </div>
            <div>
                <h3>Open Diagram (Ctrl + O)</h3>
                <p>Open the "My Diagrams" dialog to load a previously saved diagram. </p>
            </div>
            <div>
                <h3>Create a Snapshot (Ctrl + Shift + S)</h3>
                <p>Create a new snapshot for the current diagram. </p>
            </div>
            <div>
                <h3>Copy Table (Ctrl + C)</h3>
                <p>Copy the selected table. </p>
            </div>
            <div>
                <h3>Paste Table (Ctrl + V)</h3>
                <p>Paste the last copied table. </p>
            </div>
            <div>
                <h3>Delete Table (Delete)</h3>
                <p>The delete key removes the selected table. </p>
            </div>
            <div>
                <h3>Print Diagram (Ctrl + P)</h3>
                <p>Open a printable view of the diagram. </p>
            </div>
        </div>
    </div>    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cntFooter" runat="server">
</asp:Content>

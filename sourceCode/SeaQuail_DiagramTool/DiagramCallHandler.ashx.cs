using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;
using SeaQuail_DiagramTool.Model;
using System.Web.Script.Serialization;
using SeaQuail_MySQL;
using SeaQuail.Schema;
using System.Text;
using MSBDataSlave.Data;
using SeaQuail;
using SeaQuail_SQLite;
using SeaQuail_SQLServer;
using SeaQuail.Data;
using System.Configuration;
using SeaQuail_PostgreSQL;

namespace SeaQuail_DiagramTool
{
    public class DiagramCallHandler : IHttpHandler, IReadOnlySessionState
    {
        private HttpContext _Ctx = null;

        private DGUser User
        {
            get { return _Ctx.Session.GetUser(); }
        }

        private Int64 UserID
        {
            get { return User == null ? 0 : User.ID; }
        }

        public void ProcessRequest(HttpContext context)
        {
            _Ctx = context;
            string path = context.Request.Path;
            string command = path.Split(new string[] { ".dg" }, StringSplitOptions.None)[0].Replace("/", "");
            HandleRequest(command);
        }

        private void HandleRequest(string command)
        {
            switch (command)
            {
                case "PreserveSession":
                    {
                        Respond(User != null ? "true" : "false");
                    }
                    break;
                case "GetCurrentUser":
                    {
                        Respond(User);
                    }
                    break;
                case "GetDiagrams":
                    {
                        List<DGDiagram> dgs = DGDiagram.ByUserID(UserID);
                        Respond(dgs);
                    }
                    break;
                case "GetSharedDiagrams":
                    {
                        if (UserID != 0)
                        {
                            Respond(DGVSharedDiagram.ByEmail(User.Name));
                        }
                        else
                        {
                            Respond("ERROR:Not Logged In");
                        }
                    }
                    break;
                case "GetSharing":
                    {
                        if (UserID != 0)
                        {
                            Int64 diagramID = Convert.ToInt64(_Ctx.Request["DiagramID"]);

                            DGDiagram dg = DGDiagram.ByID(diagramID);
                            if (dg != null && dg.UserID == UserID)
                            {
                                Respond(DGShare.ByDiagram(diagramID));
                            }
                        }
                    }
                    break;
                case "ShareDiagram":
                    {
                        if (UserID != 0)
                        {
                            string email = _Ctx.Request["Email"];
                            Int64 diagramID = Convert.ToInt64(_Ctx.Request["DiagramID"]);

                            DGDiagram dg = DGDiagram.ByID(diagramID);
                            if (dg.UserID == UserID)
                            {
                                dg.Share(email, DGSharePermisson.View);
                                Respond(DGShare.ByDiagram(diagramID));
                            }
                            else
                            {
                                Respond("ERROR:Permission Denied");
                            }
                        }
                    }
                    break;
                case "UnshareDiagram":
                    {
                        if (UserID != 0)
                        {
                            string email = _Ctx.Request["Email"];
                            Int64 diagramID = Convert.ToInt64(_Ctx.Request["DiagramID"]);

                            DGDiagram dg = DGDiagram.ByID(diagramID);
                            if (dg.UserID == UserID)
                            {
                                dg.Unshare(email);
                                Respond(DGShare.ByDiagram(diagramID));
                            }
                        }
                    }
                    break;
                case "SaveDiagram":
                    {
                        string def = _Ctx.Request["Diagram"];

                        JavaScriptSerializer ser = new JavaScriptSerializer();
                        JSDiagram dg = ser.Deserialize<JSDiagram>(def);

                        DGDiagram diagram = DGDiagram.ByUserID(UserID).ByID(dg.ID);
                        if (diagram == null)
                        {
                            diagram = new DGDiagram();
                            diagram.UserID = UserID;
                        }

                        if (diagram.UserID != UserID)
                        {
                            Respond("ERROR:Permission Denied");
                        }
                        else
                        {
                            diagram.Name = dg.Name;
                            diagram.PrimarySnapshot.DiagramData = def;
                            diagram.Save();

                            Respond(diagram.ID);
                        }
                    }
                    break;
                case "TogglePublicAccess":
                    {
                        DGDiagram dg = DGDiagram.ByID(Convert.ToInt64(_Ctx.Request["ID"]));
                        if (dg != null)
                        {
                            if (dg.UserID == UserID)
                            {
                                dg.AllowPublicAccess = !dg.AllowPublicAccess;
                                dg.Save();
                                Respond(dg.AllowPublicAccess);
                            }
                            else
                            {
                                Respond("ERROR:Permission Denied");
                            }
                        }
                    }
                    break;
                case "GetDiagram":
                    {
                        if (_Ctx.Request["ID"] == "WelcomeDiagram")
                        {
                            string email = ConfigurationManager.AppSettings["WelcomeDiagram Email"];
                            DGUser user = DGUser.ByName(email);
                            if (user != null)
                            {
                                DGDiagram dg = DGDiagram.ByUserID(user.ID).ByName(ConfigurationManager.AppSettings["WelcomeDiagram Name"]);
                                Respond(dg);
                            }
                        }
                        else
                        {
                            DGDiagram dg = DGDiagram.ByID(Convert.ToInt64(_Ctx.Request["ID"]));
                            if (dg != null)
                            {
                                if (dg.UserID == UserID
                                    || dg.AllowPublicAccess
                                    || DGShare.ByEmailAndDiagram(User.Name, dg.ID).Count > 0)
                                {
                                    Respond(dg);
                                }
                            }
                        }
                    }
                    break;
                case "DeleteDiagram":
                    {
                        DGDiagram dg = DGDiagram.ByUserID(UserID).ByID(Convert.ToInt64(_Ctx.Request["ID"]));
                        dg.Delete();
                        Respond("OK");
                    }
                    break;
                case "AddSnapshot":
                    {
                        string def = _Ctx.Request["Snapshot"];

                        JavaScriptSerializer ser = new JavaScriptSerializer();
                        JSDiagram dg = ser.Deserialize<JSDiagram>(def);

                        // TODO: check user
                        DGDiagram diagram = DGDiagram.ByUserID(UserID).ByID(Convert.ToInt64(_Ctx.Request["DGID"]));
                        if (diagram != null)
                        {
                            DGSnapshot snapshot = new DGSnapshot()
                            {
                                Name = _Ctx.Request["Name"],
                                DiagramID = diagram.ID,
                                IsDefault = false,
                                DiagramData = def
                            };
                            snapshot.Save();

                            Respond(diagram.ID);
                        }
                    }
                    break;
                case "GetSnapshots":
                    {
                        DGDiagram diagram = DGDiagram.ByID(Convert.ToInt64(_Ctx.Request["DGID"])) ?? new DGDiagram();

                        if (diagram.UserID == UserID
                            || DGShare.ByEmailAndDiagram(User.Name, diagram.ID).Count > 0)
                        {
                            Respond(DGSnapshot.ByDiagramID(diagram.ID));
                        }
                    }
                    break;
                case "DeleteSnapshot":
                    {
                        DGSnapshot snapshot = DGSnapshot.ByID(Convert.ToInt64(_Ctx.Request["ID"]));
                        if (snapshot != null)
                        {
                            if (snapshot.Diagram.UserID == User.ID)
                            {
                                snapshot.Delete();
                                Respond("OK");
                            }
                            else
                            {
                                Respond("ERROR:Permission Denied");
                            }
                        }
                        else
                        {
                            Respond("ERROR:Bad Input ID");
                        }
                    }
                    break;
                case "LoadSnapshot":
                    {
                        DGSnapshot snapshot = DGSnapshot.ByID(Convert.ToInt64(_Ctx.Request["ID"]));
                        if (snapshot.Diagram.UserID == UserID
                            || DGShare.ByEmailAndDiagram(User.Name, snapshot.Diagram.ID).Count > 0)
                        {
                            Respond(snapshot);
                        }
                        else
                        {
                            Respond("ERROR:Permission Denied");
                        }
                    }
                    break;
                case "CreateScript":
                    {
                        string def = _Ctx.Request["Diagram"];

                        JavaScriptSerializer ser = new JavaScriptSerializer();
                        JSDiagram dg = ser.Deserialize<JSDiagram>(def);
                        
                        dg.Relate();
                        SQAdapter adp = (_Ctx.Request["Lang"] == "MySQL") ? (SQAdapter)new MySQLAdapter()
                            : (_Ctx.Request["Lang"] == "SQL Server") ? (SQAdapter)new SQLServerAdapter()
                            : (_Ctx.Request["Lang"] == "PostgreSQL") ? (SQAdapter)new PostgreSQLAdapter()
                            : (SQAdapter)new SQLiteAdapter();

                        StringBuilder sb = new StringBuilder();

                        foreach (JSTable table in dg.Tables)
                        {
                            SQTable t = table.GetTable();

                            sb.AppendLine("-- Create Table: " + t.Name);
                            sb.AppendLine("--------------------------------------------------------------------------------");
                            sb.Append(adp.WriteCreateTable(t));
                            sb.AppendLine("");
                            sb.AppendLine("");
                            sb.AppendLine("");
                        }

                        foreach (JSFKey fk in dg.FKeys)
                        {
                            sb.AppendLine(string.Format("-- Create Foreign Key: {0}.{1} -> {2}.{3}", fk.From.GetTable().Name, fk.From.GetColumn().Name, fk.To.GetTable().Name, fk.To.GetColumn().Name));
                            sb.Append(adp.WriteAddForeignKey(fk.From.GetColumn().GetColumn(), fk.To.GetColumn().GetColumn()));
                            sb.AppendLine("");
                            sb.AppendLine("");
                            sb.AppendLine("");
                        }

                        Respond(sb.ToString());
                    }
                    break;
                case "DoChangeScript":
                    {

                    }
                    break;
            }
        }

        private void Respond(object response)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            string resText = ser.Serialize(response);
            Respond(resText);            
        }
        private void Respond(string response)
        {
            _Ctx.Response.Write(response);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    #region JSON Diagram Objects
    public class JSDiagram
    {
        public string Name { get; set; }
        public Int64 ID { get; set; }
        public List<JSTable> Tables { get; set; }
        public List<JSFKey> FKeys { get; set; }

        internal void Relate()
        {
            Tables.ForEach(x => x.Relate(this));
            FKeys.ForEach(x => x.Relate(this));
        }
    }

    public class JSTable
    {
        private SQTable _Table = null;

        public string Name { get; set; }
        public string GUID { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public List<JSColumn> Columns { get; set; }

        internal JSDiagram Diagram { get; set; }

        internal void Relate(JSDiagram dg)
        {
            Diagram = dg;
            Columns.ForEach(x => x.Table = this);
        }

        internal SQTable GetTable()
        {
            if (_Table == null)
            {
                _Table = GetTable(true);
            }

            return _Table;
        }
        internal SQTable GetTable(bool refresh)
        {
            if (!refresh)
            {
                GetTable();
            }
            
            _Table = new SQTable();
            _Table.Name = Name;
            foreach (JSColumn col in Columns)
            {
                _Table.Columns.Add(col.GetColumn());
            }

            return _Table;
        }
    }

    public class JSColumn
    {
        private SQColumn _Column = null;

        public string Name { get; set; }
        public JSDataType DataType { get; set; }
        public string GUID { get; set; }
        public string Length { get; set; }
        public bool Nullable { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsIdentity { get; set; }
        public string Precision { get; set; }
        public string Scale { get; set; }
        public string DefaultValue { get; set; }

        internal JSTable Table { get; set; }

        internal SQColumn GetColumn()
        {
            if (_Column == null)
            {
                _Column = GetColumn(true);
            }

            return _Column;
        }
        internal SQColumn GetColumn(bool refresh)
        {
            if (!refresh)
            {
                return GetColumn();
            }

            _Column = new SQColumn();
            _Column.Name = Name;
            _Column.DataType = (SQDataTypes)Enum.Parse(typeof(SQDataTypes), 
                DataType.Name == "BigInt" ? SQDataTypes.Int64.ToString()
                : DataType.Name == "Int" ? SQDataTypes.Int32.ToString()
                : DataType.Name == "SmallInt" ? SQDataTypes.Int16.ToString()
                : DataType.Name);

            int len = 0;
            if (DataType.HasLength && Int32.TryParse(Length, out len))
            {
                _Column.Length = len;
            }

            int precision = 0;
            if (DataType.HasPrecision && Int32.TryParse(Precision, out precision))
            {
                _Column.Precision = precision;
            }

            int scale = 0;
            if (DataType.HasPrecision && Int32.TryParse(Scale, out scale))
            {
                _Column.Scale = scale;
            }

            if (DataType.PKOK)
            {
                _Column.IsPrimary = IsPrimary;
            }
            if (DataType.IDOK)
            {
                _Column.IsIdentity = IsIdentity;
            }

            _Column.Nullable = Nullable;

            _Column.DefaultValue = DefaultValue;

            return _Column;
        }
    }

    public class JSDataType
    {
        public string Name { get; set; }
        public bool HasLength { get; set; }
        public bool HasPrecision { get; set; }
        public bool IDOK { get; set; }
        public bool PKOK { get; set; }
        public bool FKOK { get; set; }
    }

    public class JSFKey
    {
        public JSFKeyPair From { get; set; }
        public JSFKeyPair To { get; set; }

        internal JSDiagram Diagram { get; set; }

        internal void Relate(JSDiagram dg)
        {
            Diagram = dg;
            From.Relate(this);
            To.Relate(this);
        }
    }

    public class JSFKeyPair
    {
        private JSTable _TheTable = null;
        private JSColumn _TheColumn = null;

        public string Table { get; set; }
        public string Column { get; set; }

        internal JSFKey _FKey { get; set; }

        public JSTable GetTable()
        {
            return _TheTable ?? (_TheTable = _FKey.Diagram.Tables.FirstOrDefault(x => x.GUID == Table));
        }

        public JSColumn GetColumn()
        {
            return _TheColumn ?? (_TheColumn = GetTable().Columns.FirstOrDefault(x => x.GUID == Column));
        }

        internal void Relate(JSFKey fk)
        {
            _FKey = fk;
        }
    }
    #endregion
}

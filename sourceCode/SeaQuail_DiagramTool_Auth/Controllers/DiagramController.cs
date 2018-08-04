using SeaQuail;
using SeaQuail.Schema;
using SeaQuail_DiagramTool;
using SeaQuail_DiagramTool.Model;
using SeaQuail_DiagramTool.Models;
using SeaQuail_MySQL;
using SeaQuail_PostgreSQL;
using SeaQuail_SQLite;
using SeaQuail_SQLServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SeaQuail_DiagramTool.Controllers
{
    [Authorize]
    [RequireHttps]
    public class DiagramController : Controller
    {
        private DGUser User
        {
            get { return DGUser.ByID(UserID); }
        }

        private Int64 UserID
        {
            get 
            {
                if (Session["Id"] == null)
                {
                    if (Request.IsAuthenticated)
                    {
                        var email = HttpContext.User.Identity.Name;
                        if (!string.IsNullOrEmpty(email))
                        {
                            var user = DGUser.ByName(email);
                            if (user != null)
                            {
                                Session["Id"] = user.ID;
                            }
                        }
                    }
                }

                return Convert.ToInt32(Session["Id"] ?? 0);
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult PreserveSession()
        {
            return this.Content(User != null ? "true" : "false");
        }

        public ActionResult GetCurrentUser()
        {
            return Json(User, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDiagrams()
        {
            List<DGDiagram> dgs = DGDiagram.ByUserID(UserID);
            return Json(dgs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSharedDiagrams()
        {
            if (UserID != 0)
            {
                return Json(DGVSharedDiagram.ByEmail(User.Name), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Content("ERROR:Not Logged In");
            }
        }

        public ActionResult GetSharing()
        {
            if (UserID != 0)
            {
                Int64 diagramID = Convert.ToInt64(Request["DiagramID"]);

                DGDiagram dg = DGDiagram.ByID(diagramID);
                if (dg != null && dg.UserID == UserID)
                {
                    return Json(DGShare.ByDiagram(diagramID), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new string[0], JsonRequestBehavior.AllowGet);
                }
            }
            return PermissionDenied();
        }

        public ActionResult ShareDiagram()
        {
            if (UserID != 0)
            {
                string email = Request["Email"];
                Int64 diagramID = Convert.ToInt64(Request["DiagramID"]);

                DGDiagram dg = DGDiagram.ByID(diagramID);
                if (dg.UserID == UserID)
                {
                    dg.Share(email, DGSharePermisson.View);
                    return Json(DGShare.ByDiagram(diagramID), JsonRequestBehavior.AllowGet);
                }
            }

            return PermissionDenied();
        }

        public ActionResult UnshareDiagram()
        {
            if (UserID != 0)
            {
                string email = Request["Email"];
                Int64 diagramID = Convert.ToInt64(Request["DiagramID"]);

                DGDiagram dg = DGDiagram.ByID(diagramID);
                if (dg.UserID == UserID)
                {
                    dg.Unshare(email);
                    return Json(DGShare.ByDiagram(diagramID), JsonRequestBehavior.AllowGet);
                }
            }

            return PermissionDenied();
        }

        [HttpPost]
        public ActionResult SaveDiagram()
        {
            string def = Request["Diagram"];

            JavaScriptSerializer ser = new JavaScriptSerializer();
            JSDiagram dg = ser.Deserialize<JSDiagram>(def);

            DGDiagram diagram = DGDiagram.ByUserID(UserID).ByID(dg.ID);
            if (diagram == null)
            {
                diagram = new DGDiagram();
                diagram.UserID = UserID;
            }

            if (diagram.UserID == UserID)
            {
                diagram.Name = dg.Name;
                diagram.PrimarySnapshot.DiagramData = def;
                diagram.Save();

                return Json(diagram.ID, JsonRequestBehavior.AllowGet);
            }

            return PermissionDenied();
        }

        [HttpPost]
        public ActionResult TogglePublicAccess()
        {
            DGDiagram dg = DGDiagram.ByID(Convert.ToInt64(Request["ID"]));
            if (dg != null)
            {
                if (dg.UserID == UserID)
                {
                    dg.AllowPublicAccess = !dg.AllowPublicAccess;
                    dg.Save();
                    return Json(dg.AllowPublicAccess, JsonRequestBehavior.AllowGet);
                }
            }

            return PermissionDenied();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetDiagram()
        {
            if (Request["ID"] == "WelcomeDiagram")
            {
                string email = ConfigurationManager.AppSettings["WelcomeDiagram Email"];
                DGUser user = DGUser.ByName(email);
                if (user != null)
                {
                    DGDiagram dg = DGDiagram.ByUserID(user.ID).ByName(ConfigurationManager.AppSettings["WelcomeDiagram Name"]);
                    return Json(dg, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                DGDiagram dg = DGDiagram.ByID(Convert.ToInt64(Request["ID"]));
                if (dg != null)
                {
                    if (dg.UserID == UserID
                        || dg.AllowPublicAccess
                        || DGShare.ByEmailAndDiagram(User.Name, dg.ID).Count > 0)
                    {
                        return Json(dg, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return PermissionDenied();
        }

        public ActionResult DeleteDiagram()
        {
            DGDiagram dg = DGDiagram.ByUserID(UserID).ByID(Convert.ToInt64(Request["ID"]));
            if (dg != null)
            {
                dg.Delete();
                return Content("OK");
            }

            return PermissionDenied();
        }

        [HttpPost]
        public ActionResult AddSnapshot()
        {
            string def = Request["Snapshot"];

            JavaScriptSerializer ser = new JavaScriptSerializer();
            JSDiagram dg = ser.Deserialize<JSDiagram>(def);

            DGDiagram diagram = DGDiagram.ByUserID(UserID).ByID(Convert.ToInt64(Request["DGID"]));
            if (diagram != null)
            {
                DGSnapshot snapshot = new DGSnapshot()
                {
                    Name = Request["Name"],
                    DiagramID = diagram.ID,
                    IsDefault = false,
                    DiagramData = def
                };
                snapshot.Save();

                return Json(diagram.ID, JsonRequestBehavior.AllowGet);
            }

            return PermissionDenied();
        }

        public ActionResult GetSnapshots()
        {
            DGDiagram diagram = DGDiagram.ByID(Convert.ToInt64(Request["DGID"])) ?? new DGDiagram();

            if (diagram.UserID == UserID
                || DGShare.ByEmailAndDiagram(User.Name, diagram.ID).Count > 0)
            {
                return Json(DGSnapshot.ByDiagramID(diagram.ID), JsonRequestBehavior.AllowGet);
            }

            return PermissionDenied();
        }

        public ActionResult DeleteSnapshot()
        {
            DGSnapshot snapshot = DGSnapshot.ByID(Convert.ToInt64(Request["ID"]));
            if (snapshot != null)
            {
                if (snapshot.Diagram.UserID == User.ID)
                {
                    snapshot.Delete();
                    return Content("OK");
                }
                else
                {
                    return Content("ERROR:Permission Denied");
                }
            }
            else
            {
                return Content("ERROR:Bad Input ID");
            }
        }

        public ActionResult LoadSnapshot()
        {
            DGSnapshot snapshot = DGSnapshot.ByID(Convert.ToInt64(Request["ID"]));
            if (snapshot.Diagram.UserID == UserID
                || DGShare.ByEmailAndDiagram(User.Name, snapshot.Diagram.ID).Count > 0)
            {
                return Json(snapshot, JsonRequestBehavior.AllowGet);
            }

            return PermissionDenied();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult CreateScript()
        {
            string def = Request["Diagram"];

            JavaScriptSerializer ser = new JavaScriptSerializer();
            JSDiagram dg = ser.Deserialize<JSDiagram>(def);

            dg.Relate();
            SQAdapter adp = (Request["Lang"] == "MySQL") ? (SQAdapter)new MySQLAdapter()
                : (Request["Lang"] == "SQL Server") ? (SQAdapter)new SQLServerAdapter()
                : (Request["Lang"] == "PostgreSQL") ? (SQAdapter)new PostgreSQLAdapter()
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

            return Content(sb.ToString());
        }

        private ActionResult PermissionDenied()
        {
            return Content("ERROR:Permission Denied");
        }
    }
}
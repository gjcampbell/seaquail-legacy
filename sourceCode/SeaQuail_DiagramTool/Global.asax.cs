using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using SeaQuail_SQLite;
using System.Configuration;
using SeaQuail;
using SeaQuail_DiagramTool.Model;
using SeaQuail_SQLServer;

namespace SeaQuail_DiagramTool
{
    public class Global : System.Web.HttpApplication
    {
        private SQAdapter _Adp = null;

        private SQAdapter ProvideAdapter()
        {            
            return _Adp ?? (_Adp = new SQLServerAdapter(ConfigurationManager.AppSettings["SqlServerConn"]));
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            DGBase.AdapterProvider = new DGBase.ProvideAdapter(ProvideAdapter);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }

    public static class SessionStateExtensions
    {
        public static DGUser GetUser(this HttpSessionState sess)
        {
            return (DGUser)sess["User"];
        }

        public static void SetUser(this HttpSessionState sess, DGUser user)
        {
            sess["User"] = user;
        }
    }

    public static class UriExtensions
    {
        public static string GetRoot(this Uri uri)
        {
            return uri.ToString().Replace(uri.PathAndQuery, "") + "/";
        }
    }
}
using SeaQuail;
using SeaQuail_DiagramTool.Model;
using SeaQuail_SQLServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SeaQuail_DiagramTool
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private SQAdapter _Adp = null;

        private SQAdapter ProvideAdapter()
        {
            return _Adp ?? (_Adp = new SQLServerAdapter(ConfigurationManager.AppSettings["SqlServerConn"]));
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DGBase.AdapterProvider = new DGBase.ProvideAdapter(ProvideAdapter);
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

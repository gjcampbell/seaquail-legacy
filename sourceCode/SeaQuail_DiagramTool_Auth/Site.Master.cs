using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SeaQuail_DiagramTool
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected override void OnInit(EventArgs e)
        {
            var scheme = Request.Url.Scheme;
            if (scheme.ToLower() != "https")
            {
                var redirectUrl = Request.Url.ToString();
                redirectUrl = redirectUrl.Replace("http://", "https://");
                Response.Redirect(redirectUrl);
            }
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}

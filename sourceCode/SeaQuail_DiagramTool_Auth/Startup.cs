using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SeaQuail_DiagramTool.Startup))]
namespace SeaQuail_DiagramTool
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GlobalSite.Startup))]
namespace GlobalSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

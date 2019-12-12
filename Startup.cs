using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(dc_portal.Startup))]
namespace dc_portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

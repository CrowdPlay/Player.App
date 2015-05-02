using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CrowdPlay.Startup))]
namespace CrowdPlay
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

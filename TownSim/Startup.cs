using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TownSim.Startup))]
namespace TownSim
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

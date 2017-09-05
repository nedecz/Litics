using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Litics.Controller.Startup))]

namespace Litics.Controller
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

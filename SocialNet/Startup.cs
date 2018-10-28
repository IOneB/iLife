using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SocialNet.Startup))]
namespace SocialNet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}

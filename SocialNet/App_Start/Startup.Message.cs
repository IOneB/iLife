using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

namespace SocialNet.Models
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}

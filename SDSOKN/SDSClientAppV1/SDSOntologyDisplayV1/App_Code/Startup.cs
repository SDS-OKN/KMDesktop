using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SDSOntologyDisplayV1.Startup))]

// Files related to ASP.NET Identity duplicate the Microsoft ASP.NET Identity file structure and contain initial Microsoft comments.

namespace SDSOntologyDisplayV1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
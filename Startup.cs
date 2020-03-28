using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AidatPro.Startup))]
namespace AidatPro
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

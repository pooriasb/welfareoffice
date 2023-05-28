using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BehzistiMaskan.Startup))]
namespace BehzistiMaskan
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

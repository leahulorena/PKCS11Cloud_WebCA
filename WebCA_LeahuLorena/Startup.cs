using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebCA_LeahuLorena.Startup))]
namespace WebCA_LeahuLorena
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

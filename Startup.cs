using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebAppInvoiceSystem.Startup))]
namespace WebAppInvoiceSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

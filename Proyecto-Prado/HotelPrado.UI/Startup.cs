using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HotelPrado.UI.Startup))]
namespace HotelPrado.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

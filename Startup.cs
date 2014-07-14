using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TwitterSearch.Startup))]
namespace TwitterSearch
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
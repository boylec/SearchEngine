using System.Web.Http;

namespace SearchEngine
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(Startup.Register);
        }
    }
}

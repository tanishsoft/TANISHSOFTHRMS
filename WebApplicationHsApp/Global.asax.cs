using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace WebApplicationHsApp
{
    public class MvcApplication : System.Web.HttpApplication
    {       
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(object));      
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Utilities.Network.NetworkDrive nd = new Utilities.Network.NetworkDrive();
            nd.MapNetworkDrive(@"\\172.16.0.67\dms", "Z:", "administrator", "Fh@xp@rt01");
        }       
    }
}

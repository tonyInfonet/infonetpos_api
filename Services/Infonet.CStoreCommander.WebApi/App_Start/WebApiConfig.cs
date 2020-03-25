using System.Web.Http;

namespace Infonet.CStoreCommander.WebApi
{
    /// <summary>
    /// Web API Configuration class 
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            //log4net.Config.XmlConfigurator.Configure();
           
        }
    }
}

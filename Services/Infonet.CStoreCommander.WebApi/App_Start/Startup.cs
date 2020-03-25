using Infonet.CStoreCommander.WebApi.Filters;
using Microsoft.Owin;
using Newtonsoft.Json.Serialization;
using Owin;
using Swashbuckle.Application;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

[assembly: OwinStartup(typeof(Infonet.CStoreCommander.WebApi.Startup))]
namespace Infonet.CStoreCommander.WebApi
{
    /// <summary>
    /// Startup Class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configuration
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            // Any additional configuration: filters, services, etc.


            // Adding custom filters for logging
            config.Filters.Add(new LoggingFilterAttribute());
            config.Filters.Add(new GlobalExceptionAttribute());

            // Registering and resolving the dependencies
            var dependencyResolver = new DependencyResolver();
            var resolver = dependencyResolver.RegisterDependenciesAndGetResolver();

            config.DependencyResolver = resolver;

            config.EnableSwagger(x =>
            {
                x.SingleApiVersion("v1", "Infonet C Store Commander API");
                x.OperationFilter<SwaggerHeaderParameter>();
                x.OperationFilter<AddFileUploadParams>();
                x.IncludeXmlComments($@"{AppDomain.CurrentDomain.BaseDirectory}\bin\Infonet.CStoreCommander.WebApi.XML");
            }).EnableSwaggerUi();

            //Swashbuckle.Bootstrapper.Init(config);

            app.UseWebApi(config);
            app.MapSignalR();
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

        }
    }
}
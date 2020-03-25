using System;
using Infonet.CStoreCommander.WebApi;
using Infonet.CStoreCommander.WebApi.Filters;
using Swashbuckle.Application;
using System.Web.Http;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]
namespace Infonet.CStoreCommander.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
            .EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "Infonet C Store Commander API");
                c.OperationFilter<SwaggerHeaderParameter>();
                c.OperationFilter<AddFileUploadParams>();
                c.IncludeXmlComments($@"{AppDomain.CurrentDomain.BaseDirectory}\bin\Infonet.CStoreCommander.WebApi.XML");
            })
            .EnableSwaggerUi(c =>
            {

            });
        }
    }
}

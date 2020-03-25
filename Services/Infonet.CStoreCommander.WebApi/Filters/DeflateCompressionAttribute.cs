using System.Net.Http;
using System.Web.Http.Filters;
using Infonet.CStoreCommander.WebApi.Helpers;

namespace Infonet.CStoreCommander.WebApi.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class DeflateCompressionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext actContext)
        {
            var content = actContext.Response.Content;
            var bytes = content?.ReadAsByteArrayAsync().Result;
            var zlibbedContent = bytes == null ? new byte[0] :
            CompressionHelper.DeflateByte(bytes);
            actContext.Response.Content = new ByteArrayContent(zlibbedContent);
            actContext.Response.Content.Headers.Remove("Content-Type");
            actContext.Response.Content.Headers.Add("Content-encoding", "deflate");
            actContext.Response.Content.Headers.Add("Content-Type", "application/json");
            base.OnActionExecuted(actContext);
        }
    }
}
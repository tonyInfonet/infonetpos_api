using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.WebApi.Filters;
using Swashbuckle.Swagger.Annotations;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// car wash controller
    /// </summary>
    [RoutePrefix("api/v1/Carwash")]
    [ApiAuthorization]
    public class CarwashController : ApiController
    {
        private readonly ICarwashManager _carwashManager;
        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="carwashManager"></param>
        public CarwashController(ICarwashManager carwashManager)
        {
            _carwashManager = carwashManager;
        }

        /// <summary>
        /// Method to validate the carwash code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("verifyCarwash")]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [SwaggerResponse(HttpStatusCode.OK)]
        public HttpResponseMessage VerifyCarwashCode(string code)
        {

            try
            {
                var isValid = _carwashManager.ValidateCarwash(code);
                return Request.CreateResponse(HttpStatusCode.OK, isValid);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Request can't be processed");
            }
        }

        /// <summary>
        /// Method to get the status of the server
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getServerStatus")]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [SwaggerResponse(HttpStatusCode.OK)]
        public HttpResponseMessage GetCarwashServerStatus()
        {
            try
            {
                var isActive = _carwashManager.GetCarwashServerStatus();
                return Request.CreateResponse(HttpStatusCode.OK, isActive);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Request can't be processed");
            }
        }
    }
}
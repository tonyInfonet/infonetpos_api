using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Resources;
using Infonet.CStoreCommander.WebApi.Utilities;
using log4net;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Tax;
using Swashbuckle.Swagger.Annotations;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for tax
    /// </summary>
    [RoutePrefix("api/v1/tax")]
    [ApiAuthorization]
    public class TaxController : ApiController
    {
        private readonly ITaxManager _taxManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="taxManager"></param>
        public TaxController(ITaxManager taxManager)
        {
            _taxManager = taxManager;
        }

        /// <summary>
        /// Method to get all taxes
        /// </summary>
        /// <returns></returns>
        [Route("getTaxes")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(Taxes))]
        public HttpResponseMessage GetAllTaxes()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxV1Controller,GetAllTaxes,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var accessToken = Request.GetFirstHeaderValueOrDefault<string>("authToken");
            if (accessToken == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }

                    });
            }

            var taxes = _taxManager.GetTaxes();
            _performancelog.Debug($"End,TaxV1Controller,GetAllTaxes,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, new Taxes
            {
                TaxCodes = taxes
            });
        }
    }
}

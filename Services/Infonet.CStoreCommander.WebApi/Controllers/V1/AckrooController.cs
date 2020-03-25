using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Models.Common;
using log4net;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for Ackroo integration
    /// </summary>
    [RoutePrefix("api/v1/ackroo")]
    [ApiAuthorization]
    public class AckrooController : ApiController
    {
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;
        private readonly IAckrooManager _IAckrooManager;
        public AckrooController(IAckrooManager IAckrooManager)
        {
            _IAckrooManager = IAckrooManager;
        }
        /// <summary>
        /// Returns an Ackroo Loyalty Number
        /// </summary>
        [Route("getLoyaltyNo")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]

        public HttpResponseMessage GetLoyaltyNo(int Sale_No)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,AckrooController,GetLoyaltyNo,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");


            string sVal = _IAckrooManager.GetLoyaltyNo(Sale_No);



            _performancelog.Debug($"End,AckrooController,GetLoyaltyNo,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, sVal);
        }
        /// <summary>
        /// Returns a valid ackroo stock code.
        /// </summary>
        [Route("getAValidAckrooStock")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]

        public HttpResponseMessage GetAckrooStockCode()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,AckrooController,GetAckrooStockCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");


            string sVal = _IAckrooManager.GetValidAckrooStock();



            _performancelog.Debug($"End,AckrooController,GetAckrooStockCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, sVal);
        }
        /// <summary>
        /// Returns an Ackroo Carwash stock code.
        /// </summary>
        [Route("getAckrooCarwashStockCode")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]

        public HttpResponseMessage GetAckrooCarwashStockCode(string sDesc)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,AckrooController,GetAckrooCarwashStockCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");


            string sVal = _IAckrooManager.GetAckrooCarwashStockCode(sDesc);



            _performancelog.Debug($"End,AckrooController,GetAckrooCarwashStockCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, sVal);
        }
        /// <summary>
        /// Returns Ackroo carwash categories
        /// </summary>
        [Route("getCarwashCategories")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Carwash>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]

        public HttpResponseMessage GetCarwashCategories()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,AckrooController,GetCarwashCategories,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");


            var olist = _IAckrooManager.GetCarwashCategories();



            _performancelog.Debug($"End,AckrooController,GetCarwashCategories,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, olist);
        }
    }
}

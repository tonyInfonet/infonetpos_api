using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Maintenance;
using Infonet.CStoreCommander.WebApi.Resources;
using log4net;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Maintenance Controller
    /// </summary>
    [RoutePrefix("api/v1/maintenance")]
    [ApiAuthorization]
    public class MaintenanceController : ApiController
    {
        private readonly IMaintenanceManager _maintenanceManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="maintenanceManager"></param>
        public MaintenanceController(IMaintenanceManager maintenanceManager)
        {
            _maintenanceManager = maintenanceManager;
        }

        /// <summary>
        /// Close Batch for all transactions
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("closeBatch")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Report>))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "", Type = typeof(ErrorResponse))]
        public HttpResponseMessage CloseBatch([FromBody] CloseBatchModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,MaintenanceController,CloseBatch,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            ErrorMessage error;
            var reports = _maintenanceManager.CloseBatch(model.PosId, model.TillNumber, model.SaleNumber, model.RegisterNumber, out error);
            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            _performancelog.Debug($"End,MaintenanceController,CloseBatch,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, reports);

        }

        /// <summary>
        /// Initialize bank 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("initialize")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Type = typeof(ErrorResponse))]
        public HttpResponseMessage Initialize([FromBody] CloseBatchModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,MaintenanceController,CloseBatch,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            ErrorMessage error;
            _maintenanceManager.Initialize(out error);
            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return Request.CreateResponse(error.StatusCode,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            _performancelog.Debug($"End,MaintenanceController,CloseBatch,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK,
                   new ErrorResponse
                   {
                       Error = new MessageStyle
                       {
                           Message = Resource.Error,
                           MessageType = 0
                       }
                   });
        }

        /// <summary>
        /// Method to change post pay
        /// </summary>
        /// <returns></returns>
        [Route("postPay")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SuccessReponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Forbidden, Type = typeof(ErrorResponse))]
        public HttpResponseMessage UpdatePostPay(bool newState)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,MaintenanceController,UpdatePostPay,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            ErrorMessage error;
            _maintenanceManager.UpdatePostPay(newState,out error);
            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return Request.CreateResponse(error.StatusCode,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle
                    });
            }
            _performancelog.Debug($"End,MaintenanceController,UpdatePostPay,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK,
                   new SuccessReponse
                   {
                       Success = true
                   });
        }

        /// <summary>
        /// Methgod to change prepay
        /// </summary>
        /// <returns></returns>
        [Route("prepay")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SuccessReponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage UpdatePrepay(bool newState)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,MaintenanceController,UpdatePrepay,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            ErrorMessage error;
            _maintenanceManager.UpdatePrepay(newState,out error);
            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return Request.CreateResponse(error.StatusCode,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle
                    });
            }
            _performancelog.Debug($"End,MaintenanceController,UpdatePrepay,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK,
                   new SuccessReponse
                   {
                       Success = true
                   });
        }

    }
}

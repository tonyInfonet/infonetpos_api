using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.WebApi.Filters;
using log4net;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infonet.CStoreCommander.WebApi.Models.Reason;
using Swashbuckle.Swagger.Annotations;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for reason
    /// </summary>
    [RoutePrefix("api/v1/reason")]
    [ApiAuthorization]
    public class ReasonController : ApiController
    {
        private readonly IReasonManager _reasonManager;

        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="reasonManager"></param>
        public ReasonController(IReasonManager reasonManager)
        {
            _reasonManager = reasonManager;
        }

        /// <summary>
        /// Method to get list of reasons according to reason type
        /// </summary>
        /// <param name="reason">Reason type</param>
        /// <returns>Reasons</returns>
        [Route("getReason")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ReasonModel))]
        public HttpResponseMessage GetReasons(string reason)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReasonController,GetReasons,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            if (!string.IsNullOrEmpty(reason))
            {
                reason = reason.Replace(" ", "");
            }
            ReasonType reasonType;
            Enum.TryParse(reason, true, out reasonType);
            var reasons = _reasonManager.GetReasons(reasonType);
            var reasonName = _reasonManager.GetReasonType(reasonType);

            _performancelog.Debug($"End,ReasonController,GetReasons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            var response = new ReasonModel
            {
                ReasonTitle = reasonName,
                Reasons = reasons.Select(r => new Reason
                {
                    Code = r.Reason,
                    Description = r.Description
                }).ToList()
            };
            return Request.CreateResponse(HttpStatusCode.OK,response);
        }

    }
}

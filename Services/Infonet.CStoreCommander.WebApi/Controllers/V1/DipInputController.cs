using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Resources;
using Infonet.CStoreCommander.WebApi.Utilities;
using log4net;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.DipInput;
using Infonet.CStoreCommander.WebApi.Models.Report;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Dip Input Controller
    /// </summary>
    [RoutePrefix("api/v1/dipInput")]
    [ApiAuthorization]
    public class DipInputController : ApiController
    {
        private readonly IDipInputManager _dipInputManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="dipInputManager"></param>
        public DipInputController(IDipInputManager dipInputManager)
        {
            _dipInputManager = dipInputManager;
        }

        /// <summary>
        /// Get All Dip Input Values
        /// </summary>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<DipInputModel>))]
        public HttpResponseMessage Index()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,DipInputController,Index,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var dipInputs = _dipInputManager.GetDipInputValues();

            List<DipInputModel> response = GerDipInputResponse(dipInputs);

            _performancelog.Debug($"End,DipInputController,Index,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// Save All Dip Input Values
        /// </summary>
        /// <returns></returns>
        [Route("save")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<DipInputModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public HttpResponseMessage Save([FromBody] List<DipInputModel> model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,DipInputController,Save,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            ErrorMessage error;

            var dipInputModel = (from dpInput in model
                                 select new DipInput
                                 {
                                     TankId = dpInput.TankId,
                                     GradeId = dpInput.GradeId,
                                     Grade = dpInput.Grade,
                                     DipValue = dpInput.DipValue
                                 }).ToList();

            var dipInputs = _dipInputManager.SaveDipInputs(dipInputModel, out error);

            _performancelog.Debug($"End,DipInputController,Save,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(error.StatusCode, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }

            var result = GerDipInputResponse(dipInputs);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Get report for All Dip Values
        /// </summary>
        /// <returns></returns>
        [Route("print")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ReportModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public HttpResponseMessage Print(int tillNumber, int shiftNumber, int registerNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,DipInputController,Save,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            ErrorMessage error;
            var accessToken = Request.GetFirstHeaderValueOrDefault<string>("authToken");
            if (accessToken == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                    });
            }

            var userCode = TokenValidator.GetUserCode(accessToken);

            var report = _dipInputManager.PrintDipReport(tillNumber, shiftNumber, registerNumber, userCode, out error);

            _performancelog.Debug($"End,DipInputController,Save,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(error.StatusCode, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }

            var result = new ReportModel
            {
                ReportName = report.ReportName,
                ReportContent = report.ReportContent,
                Copies = report.Copies == 0 ? 1 : report.Copies
            };
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        #region Private methods
        
        /// <summary>
        /// Method to create dip input response model
        /// </summary>
        /// <param name="dipInputs"></param>
        /// <returns></returns>
        private static List<DipInputModel> GerDipInputResponse(List<DipInput> dipInputs)
        {
            return (from dpInp in dipInputs
                    select new DipInputModel
                    {
                        TankId = dpInp.TankId,
                        GradeId = dpInp.GradeId,
                        Grade = dpInp.Grade,
                        DipValue = dpInp.DipValue
                    }).ToList();
        }

        #endregion
    }
}

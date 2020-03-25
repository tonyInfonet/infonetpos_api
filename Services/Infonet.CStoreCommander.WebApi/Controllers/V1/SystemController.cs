using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Models.Common;
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
    /// Controller for sound
    /// </summary>
    [RoutePrefix("api/v1/system")]
    [ApiAuthorization]
    public class SystemController : ApiController
    {
        private readonly IMainManager _mainManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="mainManager">Main manager</param>
        public SystemController(IMainManager mainManager)
        {
            _mainManager = mainManager;
        }

        /// <summary>
        /// Get list of all sounds
        ///</summary>
        /// <returns>List of sounds</returns>
        [Route("getSounds")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Sound>))]
        public HttpResponseMessage GetSounds()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SystemController,GetSounds,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var sounds = _mainManager.GetSoundFiles();
            _performancelog.Debug($"End,SystemController,GetSounds,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, sounds);
        }

        /// <summary>
        /// Get list of all sounds
        ///</summary>
        /// <returns>List of sounds</returns>
        [Route("getDeviceInfo")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(DeviceSetting))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetDevicesByRegisterId(int registerNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SystemController,GetDevicesByRegisterId,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            ErrorMessage error;
            var devices = _mainManager.GetDeviceSetting(registerNumber, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                _performancelog.Debug($"End,SystemController,GetDevicesByRegisterId,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(error.StatusCode, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }
            _performancelog.Debug($"End,SystemController,GetDevicesByRegisterId,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, devices);
        }

        /// <summary>
        /// Get error file content
        /// </summary>
        /// <returns></returns>
        [Route("getError")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(string))]
        public HttpResponseMessage GetFuelError()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SystemController,GetFuelError,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var content = string.Empty;
            var fs = _mainManager.GetErrorLog();
            if (fs != null)
            {
                content = Helper.CreateBytes(fs);
            }
            _performancelog.Debug($"End,SystemController,GetFuelError,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, content);
        }

        /// <summary>
        /// Clear the error file content
        /// </summary>
        /// <returns></returns>
        [Route("clearError")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(bool))]
        public HttpResponseMessage ClearErrorLog()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SystemController,ClearErrorLog,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var response = _mainManager.ClearErrorLog();

            _performancelog.Debug($"End,SystemController,ClearErrorLog,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// Checks whether any error is logged
        /// </summary>
        /// <returns></returns>
        [Route("checkError")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(bool))]
        public HttpResponseMessage CheckError()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SystemController,ClearErrorLog,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var response = _mainManager.CheckErrorLog();

            _performancelog.Debug($"End,SystemController,ClearErrorLog,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}

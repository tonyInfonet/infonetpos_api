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
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Message;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for message
    /// </summary>
    [RoutePrefix("api/v1/message")]
    [ApiAuthorization]
    public class MessageController : ApiController
    {
        private readonly IMessageManager _messageManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Ctor
        /// </summary>
        public MessageController(IMessageManager messageManager)
        {
            _messageManager = messageManager;
        }

        /// <summary>
        /// Get list of all messages
        /// </summary>
        /// <returns>List of messages</returns>
        [Route("")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<MessageButton>))]
        public HttpResponseMessage Index()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,MessageController,Index,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var messages = _messageManager.GetMessages();
            _performancelog.Debug($"End,MessageController,Index,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, messages);
        }

        /// <summary>
        /// Save message
        /// </summary>
        /// <param name="messageModel">Message model</param>
        /// <returns>Success/Failure</returns>
        [Route("add")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SuccessReponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type =typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type =typeof(ErrorResponse))]
        public HttpResponseMessage SaveMessage([FromBody]MessageModel messageModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,MessageController,SaveMessage,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
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
            if(messageModel == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = new MessageStyle
                    {
                        Message = Resource.InvalidInformation
                    }
                });
            }
            var userCode = TokenValidator.GetUserCode(accessToken);
            var message = new MessageButton
            {
                Message = messageModel.Message,
                Index = messageModel.Index
            };
            ErrorMessage error;
             _messageManager.SaveMessage(message,messageModel.TillNumber,userCode,out error);

            _performancelog.Debug($"End,MessageController,SaveMessage,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            var response = new SuccessReponse {Success = true};
            if (string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }

            return Request.CreateResponse(error.StatusCode,
             new ErrorResponse
             {
                 Error = error.MessageStyle,
             });
        }
    }
}

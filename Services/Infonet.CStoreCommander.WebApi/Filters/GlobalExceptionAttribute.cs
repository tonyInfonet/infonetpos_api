using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Exceptions;
using Infonet.CStoreCommander.WebApi.Helpers;
using Infonet.CStoreCommander.WebApi.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Tracing;

namespace Infonet.CStoreCommander.WebApi.Filters
{
    /// <summary>
    /// Class for Global Exception
    /// </summary>
    public class GlobalExceptionAttribute : ExceptionFilterAttribute
    {
        /// <summary> 
        /// On Exception filter
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(HttpActionExecutedContext context)
        {
            GlobalConfiguration.Configuration.Services.Replace(typeof(ITraceWriter), new NLogger());
            var trace = GlobalConfiguration.Configuration.Services.GetTraceWriter();
            trace.Error(context.Request, "Controller : " + context.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine + "Action : " + context.ActionContext.ActionDescriptor.ActionName, context.Exception.ToString());

            var exceptionType = context.Exception.GetType();

            if (exceptionType == typeof(ValidationException))
            {                
                throw new HttpResponseException(context.Request.CreateResponse(HttpStatusCode.BadRequest,
                    new
                    {
                        error = new
                        {
                            Message = new StringContent(context.Exception.Message),
                            MessageType = MessageType.OkOnly
                        }
                    }));

            }
            else if (exceptionType == typeof(UnauthorizedAccessException))
            {
                throw new HttpResponseException(context.Request.CreateResponse(HttpStatusCode.Unauthorized,
                    new
                    {
                        error = new
                        {
                            Message = "UnAuthorized",
                            MessageType = MessageType.OkOnly
                        }
                    }));
            }
            else if (exceptionType == typeof(ApiException))
            {
                var webapiException = context.Exception as ApiException;
                if (webapiException != null)
                    throw new HttpResponseException(context.Request.CreateResponse(webapiException.HttpStatus,
                        new
                        {
                            error = new
                            {
                                Message = webapiException.ErrorDescription,
                                MessageType = MessageType.OkOnly
                            }
                        }));
            }
            else if (exceptionType == typeof(ApiBusinessException))
            {
                var businessException = context.Exception as ApiBusinessException;
                if (businessException != null)
                    throw new HttpResponseException(context.Request.CreateResponse(businessException.HttpStatus,
                        new
                        {
                            error = new
                            {
                                Message = businessException.ErrorDescription,
                                MessageType = MessageType.OkOnly
                            }
                        }));
            }
            else if (exceptionType == typeof(ApiDataException))
            {
                var dataException = context.Exception as ApiDataException;
                if (dataException != null)
                    throw new HttpResponseException(context.Request.CreateResponse(dataException.HttpStatus,
                        new
                        {
                            error = new
                            {
                                Message = dataException.ErrorDescription,
                                MessageType = MessageType.OkOnly
                            }
                        }));
            }
            else if (exceptionType == typeof(InvalidTokenException))
            {
                var dataException = context.Exception as InvalidTokenException;
                if (dataException != null)
                    throw new HttpResponseException(context.Request.CreateResponse(dataException.HttpStatus,
                         new
                         {
                             error = new
                             {
                                 Message = dataException.ErrorDescription,
                                 MessageType = MessageType.OkOnly
                             }
                         }));
            }
            else if (exceptionType == typeof(SocketException))
            {
                var dataException = context.Exception as SocketException;
                if (dataException != null)
                    throw new HttpResponseException(context.Request.CreateResponse(HttpStatusCode.InternalServerError,
                   new
                   {
                       error = new
                       {
                           Message = Resource.PortAlreadyOpenMessage,
                           MessageType = MessageType.OkOnly
                       }
                   }));
            }
            else
            {
                 throw new HttpResponseException(context.Request.CreateResponse(HttpStatusCode.InternalServerError,
                   new
                   {
                       error = new
                       {
                           Message = Resource.Error,
                           MessageType = MessageType.OkOnly
                       }
                   }));
            }
        }
    }
}
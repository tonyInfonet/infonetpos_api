using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Utilities;
using log4net;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infonet.CStoreCommander.WebApi.Models.Login;
using Infonet.CStoreCommander.WebApi.Models.Till;
using Infonet.CStoreCommander.WebApi.Resources;
using Swashbuckle.Swagger.Annotations;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Login Controller
    /// </summary>
    [RoutePrefix("api/v1/login")]
    public class LoginController : ApiController
    {
        private readonly ITillManager _tillManager;
        private readonly ILoginManager _loginManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tillManager"></param>
        /// <param name="loginManager"></param>
        /// <param name="resourceManager"></param>
        public LoginController(ITillManager tillManager, ILoginManager loginManager, IApiResourceManager resourceManager)
        {
            _tillManager = tillManager;
            _loginManager = loginManager;
            _resourceManager = resourceManager;
        }

        /// <summary>
        /// Login user 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LoginResponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(InvalidLoginReponseModel))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(InvalidLoginReponseModel))]
        public HttpResponseMessage LoginUser([FromBody]UserModel user)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,LoginController,LoginUser,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (user != null)
            {
                ErrorMessage errorMessage;
                var trainerCaption = _tillManager.UpdateTillInformation(user.TillNumber, user.ShiftNumber, user.ShiftDate, user.UserName, user.Password, user.PosId, user.FloatAmount, out errorMessage);
                if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                {
                    var ticks = DateTime.Now.Ticks;
                    var token = TokenGenerator.GenerateToken(user.UserName, ticks, user.PosId);
                    _performancelog.Debug($"End,LoginController,LoginUser,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                    var login = new LoginResponseModel
                    {
                        AuthToken = token,
                        TrainerCaption = trainerCaption
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, login);
                }
                _performancelog.Debug($"End,LoginController,LoginUser,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                var errorMsg = new InvalidLoginReponseModel
                {
                    Error = errorMessage.MessageStyle,
                    ShutDownPOS = errorMessage.ShutDownPos
                };
                return Request.CreateResponse(errorMessage.StatusCode, errorMsg);
            }
            _performancelog.Debug($"End,LoginController,LoginUser,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            var error = new InvalidLoginReponseModel
            {
                Error = new MessageStyle
                {
                    Message = Resource.InvalidInformation,
                    MessageType = MessageType.OkOnly
                },
                ShutDownPOS = true
            };
            return Request.CreateResponse(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Login user 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [Route("getPassword")]
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(InvalidLoginReponseModel))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(InvalidLoginReponseModel))]
        public HttpResponseMessage GetPassword(string userName)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,LoginController,GetPassword,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!string.IsNullOrEmpty(userName))
            {
                ErrorMessage errorMessage;
                var password = _loginManager.GetPassword(userName, out errorMessage);
                _performancelog.Debug($"End,LoginController,GetPassword,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                {
                    //if there is any message
                    return Request.CreateResponse(errorMessage.StatusCode,
                        new InvalidLoginReponseModel
                        {
                            Error = errorMessage.MessageStyle,
                            ShutDownPOS = errorMessage.ShutDownPos
                        });
                }

                return Request.CreateResponse(HttpStatusCode.OK, password);
            }
            _performancelog.Debug($"End,LoginController,GetPassword,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            var error = new InvalidLoginReponseModel
            {
                Error = new MessageStyle
                {
                    Message = Resource.InvalidInformation,
                    MessageType = MessageType.OkOnly
                },
                ShutDownPOS = true
            };
            return Request.CreateResponse(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("changePassword")]
        [HttpPost]
        [ApiAuthorization]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ChangePasswordReponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(InvalidLoginReponseModel))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(InvalidLoginReponseModel))]
        public HttpResponseMessage ChangePassword([FromBody]ChangePasswordModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,LoginController,ChangePassword,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (model != null)
            {
                ErrorMessage errorMessage;
                const MessageType messageType = (int)MessageType.Critical + MessageType.OkOnly;
                if (string.IsNullOrEmpty(model.Password))
                {
                    _performancelog.Debug($"End,LoginController,ChangePassword,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                    //MsgBox "Password Can not be Empty", vbcritical + vbOKOnly, "Empty Password"
                    var store = CacheManager.GetStoreInfo();
                    var offSet = store?.OffSet ?? 0;
                    var errorMsg = new InvalidLoginReponseModel
                    {
                        Error = _resourceManager.CreateMessage(offSet,39, 64, null, messageType),
                        ShutDownPOS = false
                    };
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, errorMsg);
                }
                if (model.Password != model.ConfirmPassword)
                {
                    var store = CacheManager.GetStoreInfo();
                    var offSet = store?.OffSet ?? 0;
                    // MsgBox "Re-entry Password is not matching with the Password Entry", vbCritical + vbOKOnly, "Password Entry"
                    _performancelog.Debug($"End,LoginController,ChangePassword,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                    var errorMsg = new InvalidLoginReponseModel
                    {
                        Error = _resourceManager.CreateMessage(offSet,39, 65, null, messageType),
                        ShutDownPOS = false
                    };

                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, errorMsg);
                }
                var result = _loginManager.ChangePassword(model.UserName, model.Password, out errorMessage);
                _performancelog.Debug($"End,LoginController,ChangePassword,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                var response = new ChangePasswordReponseModel
                {
                    Error = errorMessage.MessageStyle,
                    Success = result
                };
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            _performancelog.Debug($"End,LoginController,ChangePassword,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            var error = new InvalidLoginReponseModel
            {
                Error = new MessageStyle
                {
                    Message = Resource.InvalidInformation,
                    MessageType = MessageType.OkOnly
                },
                ShutDownPOS = true
            };
            return Request.CreateResponse(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Change user 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("changeUser")]
        [HttpPost]
        [ApiAuthorization]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LoginResponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(InvalidLoginReponseModel))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(InvalidLoginReponseModel))]
        public HttpResponseMessage ChangeUser([FromBody]UserModel user)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,LoginController,ChangeUser,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var accessToken = Request.GetFirstHeaderValueOrDefault<string>("authToken");
            var userCode = TokenValidator.GetUserCode(accessToken);

            if (user != null)
            {
                ErrorMessage errorMessage;
                string userName;
                var result = _loginManager.ChangeUser(userCode, user.UserName,
                    user.Password, user.TillNumber, user.ShiftNumber, user.ShiftDate,
                    user.PosId, user.UnauthorizedAccess, out errorMessage, out userName);
                if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message) || !result)
                {
                    var response = new InvalidLoginReponseModel
                    {
                        Error = errorMessage.MessageStyle,
                        ShutDownPOS = false
                    };
                    return Request.CreateResponse(HttpStatusCode.Conflict, response);
                }
                var ticks = DateTime.Now.Ticks;
                var token = TokenGenerator.GenerateToken(user.UserName, ticks, user.PosId);
                var loginResponse = new LoginResponseModel
                {
                    AuthToken = token ,
                    TrainerCaption = userName
                };
                return Request.CreateResponse(HttpStatusCode.OK, loginResponse);
            }

            _performancelog.Debug($"End,LoginController,ChangeUser,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            var error = new InvalidLoginReponseModel
            {
                Error = new MessageStyle
                {
                    Message = Resource.InvalidInformation,
                    MessageType = MessageType.OkOnly
                },
                ShutDownPOS = true
            };
            return Request.CreateResponse(HttpStatusCode.BadRequest, error);
        }
    }
}

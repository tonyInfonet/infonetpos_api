using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Resources;
using Infonet.CStoreCommander.WebApi.Utilities;
using log4net;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Login;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for Policy 
    /// </summary>
    [RoutePrefix("api/v1/policy")]
    public class PolicyController : ApiController
    {
        private readonly ILoginManager _loginManager;
        private readonly IPolicyManager _policyManager;
        private readonly ITillManager _tillManager;
        private readonly ISaleManager _saleManager;
        private readonly IApiResourceManager _resourceManager;

        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="policyManager"></param>
        /// <param name="loginManager"></param>
        /// <param name="tillManager"></param>
        /// <param name="resourcemanager"></param>
        /// <param name="saleManager"></param>
        public PolicyController(IPolicyManager policyManager,
            ILoginManager loginManager,
            ITillManager tillManager,
            IApiResourceManager resourcemanager,
            ISaleManager saleManager)
        {
            _policyManager = policyManager;
            _loginManager = loginManager;
            _tillManager = tillManager;
            _resourceManager = resourcemanager;
            _saleManager = saleManager;
            GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;

        }


        /// <summary>
        /// Get login Policies
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        [Route("login")]
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(InvalidLoginReponseModel))]
        [SwaggerResponse(HttpStatusCode.NotAcceptable, Type = typeof(InvalidLoginReponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(InvalidLoginReponseModel))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(InvalidLoginReponseModel))]
        public HttpResponseMessage GetLoginPolicies(string ipAddress)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PolicyController,GetLoginPolicies,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string message;
            var posId = _loginManager.Authenticate(ipAddress, out message, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                _performancelog.Debug($"End,PolicyController,GetLoginPolicies,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return Request.CreateResponse(
                                    error.StatusCode,
                                    new InvalidLoginReponseModel
                                    {
                                        Error = error.MessageStyle,
                                        ShutDownPOS = error.ShutDownPos
                                    });
            }

            if (posId == 0)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                _performancelog.Debug($"End,PolicyController,GetLoginPolicies,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new InvalidLoginReponseModel
                    {
                        Error = new MessageStyle { Message = _resourceManager.GetResString(8198, offSet) },
                        ShutDownPOS = error.ShutDownPos
                    });
            }

            if (!_tillManager.IsActiveTillAvailable(posId, out error))
            {
                _performancelog.Debug($"End,PolicyController,GetLoginPolicies,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return Request.CreateResponse(
                                   error.StatusCode,
                                   new InvalidLoginReponseModel
                                   {
                                       Error = error.MessageStyle,
                                       ShutDownPOS = error.ShutDownPos
                                   });
            }

            var policies = _policyManager.GetLoginPolicies(ipAddress, posId);

            _performancelog.Debug($"End,PolicyController,GetLoginPolicies,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                policies,
                message
            });
        }

        /// <summary>
        /// Get All Policies
        /// </summary>
        /// <returns></returns>
        [Route("getAll")]
        [HttpGet]
        [ApiAuthorization]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetAllPolicies(int registerNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PolicyController,GetAllPolicies,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var accessToken = Request.GetFirstHeaderValueOrDefault<string>("authToken");
            if (accessToken == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle
                        {
                            Message = Resource.Error,
                            MessageType = MessageType.OkOnly
                        }
                    });
            }

            var userCode = TokenValidator.GetUserCode(accessToken);
            var posId = TokenValidator.GetPosId(accessToken);
            //delete and initialise a new register by sale initialisation
            CacheManager.DeleteRegister(posId);
            var policies = _policyManager.GetAllPolicies(userCode);
            if (policies == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new ErrorResponse
                    {
                        Error = new MessageStyle
                        {
                            Message = Resource.Error,
                            MessageType = MessageType.OkOnly
                        }
                    });
            }
            var allPolicies = (object)policies;
            _performancelog.Debug($"End,PolicyController,GetAllPolicies,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, allPolicies);
        }

        /// <summary>
        /// Refresh All Policies
        /// </summary>
        /// <returns></returns>
        [Route("refresh")]
        [HttpGet]
        [ApiAuthorization]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
         public HttpResponseMessage RefreshPolicies(int tillNumber, int saleNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PolicyController,RefreshPolicies,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var accessToken = Request.GetFirstHeaderValueOrDefault<string>("authToken");
            if (accessToken == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle
                        {
                            Message = Resource.Error,
                            MessageType = MessageType.OkOnly
                        }
                    });
            }

            var userCode = TokenValidator.GetUserCode(accessToken);
            var policies = _policyManager.RefreshPolicies(userCode);
            if (policies == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new ErrorResponse
                    {
                        Error = new MessageStyle
                        {
                            Message = Resource.Error,
                            MessageType = MessageType.OkOnly
                        }
                    });
            }
            var error = new ErrorMessage();
            var sale = CacheManager.GetCurrentSaleForTill(tillNumber, saleNumber);
            if (string.IsNullOrEmpty(error.MessageStyle.Message) && sale != null)
            {
                policies.EnableExactChange = _saleManager.EnableCashButton(sale, userCode);
            }
            var allPolicies = (object)policies;
            _performancelog.Debug($"End,PolicyController,RefreshPolicies,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, allPolicies);
        }
    }
}

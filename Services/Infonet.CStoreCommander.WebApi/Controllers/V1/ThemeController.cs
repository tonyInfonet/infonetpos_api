using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using log4net;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq;
using Swashbuckle.Swagger.Annotations;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Login;
using Infonet.CStoreCommander.WebApi.Models.Theme;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Themes Controller
    /// </summary>
    [RoutePrefix("api/v1/themes")]
    public class ThemeController : ApiController
    {
        private readonly ILoginManager _loginManager;
        private readonly IThemeManager _themeManager;
        private readonly IApiResourceManager _resourceManager;

        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="themeManager"></param>
        /// <param name="resourcemanager"></param>
        /// <param name="loginManager"></param>
        public ThemeController(ILoginManager loginManager,
            IThemeManager themeManager,
            IApiResourceManager resourcemanager)
        {
            _loginManager = loginManager;
            _themeManager = themeManager;
            _resourceManager = resourcemanager;
        }

        /// <summary>
        /// Get ACtive theme Data
        /// </summary>
        /// <param name="ipAddress">IP Address of the POS accessing the API</param>
        /// <returns></returns>
        [Route("active")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(InvalidLoginReponseModel))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ThemeModel))]
        [AllowAnonymous]
        public HttpResponseMessage GetActiveTheme(string ipAddress)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ThemeV1Controller,GetActiveTheme,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string message;

            var posId = _loginManager.Authenticate(ipAddress, out message, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                _performancelog.Debug($"End,ThemeV1Controller,GetActiveTheme,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
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
                _performancelog.Debug($"End,ThemeV1Controller,GetActiveTheme,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                var store = CacheManager.GetStoreInfo();
                var offSet = store?.OffSet ?? 0;
                return Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new InvalidLoginReponseModel
                    {
                        Error = new MessageStyle { Message = _resourceManager.GetResString(8198, offSet)},
                        ShutDownPOS = error.ShutDownPos
                    });
            }

            var theme = _themeManager.GetActiveTheme();

            if (theme != null)
            {
                return Request.CreateResponse
                    (HttpStatusCode.OK,
                    new ThemeModel
                    {
                        BackgroundColor1Light = theme.Data.FirstOrDefault(x => x.Name == "BackgroundColor1Light")?.ColorCode,
                        BackgroundColor1Dark = theme.Data.FirstOrDefault(x => x.Name == "BackgroundColor1Dark")?.ColorCode,
                        BackgroundColor2 = theme.Data.FirstOrDefault(x => x.Name == "BackgroundColor2")?.ColorCode,
                        ButtonFooterConfirmationColor = theme.Data.FirstOrDefault(x => x.Name == "ButtonFooterConfirmationColor")?.ColorCode,
                        ButtonFooterWarningColor = theme.Data.FirstOrDefault(x => x.Name == "ButtonFooterWarningColor")?.ColorCode,
                        HeaderBackgroundColor = theme.Data.FirstOrDefault(x => x.Name == "ButtonBackgroundColor")?.ColorCode,
                        HeaderForegroundColor = theme.Data.FirstOrDefault(x => x.Name == "ButtonForegroundColor")?.ColorCode,
                         ButtonFooterColor = theme.Data.FirstOrDefault(x => x.Name == "ButtonFooterColor")?.ColorCode,
                        LabelTextForegroundColor = theme.Data.FirstOrDefault(x => x.Name == "LabelTextForegroundColor")?.ColorCode
                    });
            }

            return Request.CreateResponse(
                HttpStatusCode.NotFound,
                new ErrorResponse
                {
                    Error = new MessageStyle { Message =  "There are no themes defined!"}
                });
        }
    }
}

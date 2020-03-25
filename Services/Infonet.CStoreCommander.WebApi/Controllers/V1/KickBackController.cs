using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Resources;
using Infonet.CStoreCommander.WebApi.Utilities;
using log4net;
using System;
using System.Net;
using System.Net.Http;

using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Tax;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http;
using System.Web;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.WebApi.Models.Tender;
using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.WebApi.Models.GasKing;
using Infonet.CStoreCommander.BusinessLayer.Entities;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{

    /// <summary>
    /// Controller for KickBack
    /// </summary>
    [RoutePrefix("api/v1/kickback")]
    public class KickBackController : ApiController
    {

        private readonly IKickBackManager _kickBackManager;
        private readonly ITenderManager _tenderManager;
        private readonly ISaleManager _saleManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        public static Sale sale;
        ///// <summary>
        ///// Ctor
        ///// </summary>
        ///// <param name="kickBackManager"></param>
        public KickBackController(IKickBackManager kickBackManager,ITenderManager tenderManager, ISaleManager saleManager)
        {
            _kickBackManager = kickBackManager;
            _tenderManager = tenderManager;
            _saleManager = saleManager;
        }

        [Route("checkKickBackResponse")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SuccessReponse))]
        public HttpResponseMessage CheckResponse(bool response, int tillNumber,byte registerNumber, int saleNumber)
        {
            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,KickBackController,CheckResponse,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = false;
            //var sale = CacheManager.GetCurrentSaleForTill(tillNumber, saleNumber);
            sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out errorMessage);
            var points= _kickBackManager.CheckKickbackResponse(response, tillNumber, saleNumber, userCode, out errorMessage,ref sale);
            if (points != 0)
            {
                sale.Customer.Points_Redeemed = points;
                _performancelog.Debug($"End,KickBackController,CheckResponse,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                result = true;
            }
            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle?.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                }

                    );
            }
            _performancelog.Debug($"End,KickBackController,CheckResponse,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");


            return Request.CreateResponse(HttpStatusCode.OK, new SuccessReponse
            {
                Success = result
            });
        }



        /// <summary>
        /// Method to get user code 
        /// </summary>
        /// <param name="userCode">Usercode</param>
        /// <param name="httpResponseMessage">HttpResponse</param>
        /// <returns>True or false</returns>
        private bool GetUserCode(out string userCode, out HttpResponseMessage httpResponseMessage)
        {
            var dateStart = DateTime.Now;
            userCode = string.Empty;
            httpResponseMessage = null;
            _performancelog.Debug($"Start,KickBackController,GetUserCode,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            var accessToken = Request.GetFirstHeaderValueOrDefault<string>("authToken");
            if (accessToken == null)
            {
                {
                    httpResponseMessage = Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        new ErrorResponse
                        {
                            Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                        });
                    return true;
                }
            }

            userCode = TokenValidator.GetUserCode(accessToken);
            _performancelog.Debug($"End,KickBackController,GetUserCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return false;
        }


        // GET: KickBack
        //public ActionResult Index()
        //{

        //    return View();
        //}


        [Route("checkKickBackBalance")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(KickbackPointsModel))]
        public HttpResponseMessage CheckBalance(int tillNumber, int saleNumber,string pointCardNum)
        {
            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
           
            var points = _kickBackManager.CheckBalance(pointCardNum, saleNumber, tillNumber,userCode, out errorMessage);

            if (points == -1)
            {
                if (!string.IsNullOrEmpty(errorMessage?.MessageStyle?.Message))
                {
                    return Request.CreateResponse(errorMessage.StatusCode,new ErrorResponse
                    {
                        Error=errorMessage.MessageStyle
                    }
                        
                        );
                }
            }
            var kickBackPoints = new KickbackPointsModel
            {
                BalancePoints = points
            };

            //   CacheManager.AddCurrentSaleForTill(tillNumber,saleNumber,);
            var s = _saleManager.GetCurrentSale(saleNumber, tillNumber, 1, userCode, out errorMessage);
            return Request.CreateResponse(HttpStatusCode.OK, kickBackPoints);
        }


       
        [Route("validateGasKing")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(GaskingKickback))]
        public HttpResponseMessage ValidateGasKing(int tillNumber, int saleNumber,byte registerNumber)
        {
            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;

        var response=_kickBackManager.ValidateGasKing(tillNumber, saleNumber, registerNumber, userCode, out errorMessage);
            //var kickBackPoints = new KickbackPointsModel
            //{
            //    BalancePoints = points
            //};
            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle?.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                }
                   
                    );
            }

            return Request.CreateResponse(HttpStatusCode.OK,response);
        }
    }
}
using Infonet.CStoreCommander.BusinessLayer.Entities;
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
using System.Linq;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Payment;
using Infonet.CStoreCommander.WebApi.Models.Payout;
using Infonet.CStoreCommander.WebApi.Models.Report;
using Infonet.CStoreCommander.WebApi.Models.Sale;
using Infonet.CStoreCommander.WebApi.Models.Tender;
using Infonet.CStoreCommander.WebApi.Mapper;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for payout
    /// </summary>
    [RoutePrefix("api/v1/payout")]
    [ApiAuthorization]
    public class PayoutController : ApiController
    {
        private readonly IPayoutManager _payoutManager;
        private readonly ISaleManager _saleManager;
        private readonly ITenderManager _tenderManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="payoutManager"></param>
        /// <param name="saleManager"></param>
        /// <param name="tenderManager"></param>
        public PayoutController(IPayoutManager payoutManager, ISaleManager saleManager,
            ITenderManager tenderManager)
        {
            _payoutManager = payoutManager;
            _saleManager = saleManager;
            _tenderManager = tenderManager;
        }

        /// <summary>
        /// Get the payout information
        /// </summary>
        /// <returns>Vendor payout</returns>
        [Route("getVendorPayout")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VendorPayout))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetVendorPayout()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PayoutController,GetVendorPayout,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            ErrorMessage error;
            var vendorPayout = _payoutManager.GetPayoutVendor(out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound,
              new ErrorResponse
              {
                  Error = error.MessageStyle,
              });
            }
            _performancelog.Debug($"End,PayoutController,GetVendorPayout,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, vendorPayout);
        }

        /// <summary>
        /// Complete payout
        /// </summary>
        /// <param name="payoutModel">Message model</param>
        /// <returns>Success/Failure</returns>
        [Route("complete")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CompletePaymentResponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage CompletePayout([FromBody]PayoutModel payoutModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PayoutController,CompletePayout,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (payoutModel == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = new MessageStyle
                    {
                        Message = Resource.InvalidInformation,
                        MessageType = MessageType.OkOnly
                    }
                });
            }
            var po = new Payout
            {
                Sale_Num = payoutModel.SaleNumber,
                Gross = payoutModel.Amount,
                Return_Reason = new Return_Reason { Reason = payoutModel.ReasonCode },
                Vendor = new Vendor { Code = payoutModel.VendorCode },

            };
            ErrorMessage error;
            bool openDrawer;
            var fs = _payoutManager.SaveVendorPayout(po, payoutModel.TillNumber, userCode,
                payoutModel.RegisterNumber, payoutModel.Taxes, out openDrawer, out error);
            _performancelog.Debug($"End,PayoutController,CompletePayout,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound,
              new ErrorResponse
              {
                  Error = error.MessageStyle,
              });
            }
            var newSale = _saleManager.InitializeSale(payoutModel.TillNumber,payoutModel.RegisterNumber, userCode, out error);
            var completePaymentResponseModel = new CompletePaymentResponseModel
            {
                NewSale = new NewSale
                {
                    SaleNumber = newSale.Sale_Num,
                    TillNumber = newSale.TillNumber,
                    Customer = newSale.Customer.Name
                },
                OpenCashDrawer = openDrawer,
                CustomerDisplays = new List<CustomerDisplay> { newSale.CustomerDisplay}
            };
            try
            {
                completePaymentResponseModel.PaymentReceipt = new ReportModel
                {
                    ReportName = fs.ReportName,
                    ReportContent = fs.ReportContent,
                    Copies = fs.Copies
                };
                _performancelog.Debug($"End,PayoutController,CompletePayout,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return Request.CreateResponse(HttpStatusCode.OK, completePaymentResponseModel);
            }
            catch
            {
                _performancelog.Debug($"End,PayoutController,CompletePayout,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                });
            }
        }

        /// <summary>
        /// Get the payout information
        /// </summary>
        /// <returns>Vendor payout</returns>
        [Route("validateFleet")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ValidateFleetResponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public HttpResponseMessage ValidateFleet()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PayoutController,ValidateFleet,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            ErrorMessage error;
            bool allowSwipe;
            var caption = _payoutManager.ValidateFleetPayout(out allowSwipe, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message) && error.StatusCode != HttpStatusCode.OK)
            {
                return Request.CreateResponse(error.StatusCode,
              new ErrorResponse
              {
                  Error = error.MessageStyle,
              });
            }
            var response = new ValidateFleetResponseModel
            {
                Message = error.MessageStyle,
                Caption = caption,
                AllowSwipe = allowSwipe
            };
            _performancelog.Debug($"End,PayoutController,ValidateFleet,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// Get Sale Summary
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("fleetPayment")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SaleSummaryResponseModel))]
        public HttpResponseMessage SaleSummaryForArPayment([FromBody] FleetPaymentInputModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderV1Controller,SaleSummaryForARPayment,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (model == null)
            {
                return Request.CreateResponse(
                  HttpStatusCode.BadRequest,
                  new ErrorResponse
                  {
                      Error = new MessageStyle { Message = Constants.InvalidRequest }
                  });
            }
            var result = _tenderManager.SaleSummaryForFleetPayment(model.CardNumber, model.Amount, model.IsSwiped,
                userCode, model.TillNumber, model.SaleNumber, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle?.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }

            var response = new SaleSummaryResponseModel
            {
                SaleSummary = result.SaleSummary != null ? (from saleSum in result.SaleSummary
                                                            select new NameValuePair
                                                            {
                                                                Key = saleSum.Key,
                                                                Value = saleSum.Value
                                                            }).ToList() : null,
                TenderSummary = result.Tenders != null ? TenderMapper.GetTenderSummaryModel(result.Tenders,
                "", result.Tenders) : null
            };

            _performancelog.Debug($"End,TenderV1Controller,SaleSummaryForARPayment,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        
        #region Private methods

        /// <summary>
        /// Method to get user code 
        /// </summary>
        /// <param name="userCode">Usercode</param>
        /// <param name="httpResponseMessage">HttpResponse</param>
        /// <returns>True or false</returns>
        private bool GetUserCode(out string userCode, out HttpResponseMessage httpResponseMessage)
        {
            userCode = string.Empty;
            httpResponseMessage = null;
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
            return false;
        }

        #endregion
    }
}

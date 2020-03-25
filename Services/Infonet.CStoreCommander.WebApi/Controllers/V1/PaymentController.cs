using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Resources;
using Infonet.CStoreCommander.WebApi.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Mapper;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Payment;
using Infonet.CStoreCommander.WebApi.Models.Report;
using Infonet.CStoreCommander.WebApi.Models.Sale;
using Infonet.CStoreCommander.WebApi.Models.Tender;
using Swashbuckle.Swagger.Annotations;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Payment controller
    /// </summary>
    [RoutePrefix("api/v1/payment")]
    [ApiAuthorization]
    public class PaymentController : ApiController
    {
        private readonly IPaymentManager _paymentManager;
        private readonly ISaleManager _saleManager;
        private readonly ITenderManager _tenderManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;
        private readonly IApiResourceManager _resourceManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentManager"></param>
        /// <param name="saleManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="tenderManager">tender manager</param>
        public PaymentController(IPaymentManager paymentManager,
            ISaleManager saleManager, IApiResourceManager resourceManager,
            ITenderManager tenderManager)
        {
            _paymentManager = paymentManager;
            _saleManager = saleManager;
            _resourceManager = resourceManager;
            _tenderManager = tenderManager;
        }

        /// <summary>
        /// Method to complete payment by exact cash
        /// </summary>
        /// <param name="paymentModel">Payment model</param>
        /// <returns></returns>
        [Route("byExactCash")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CompletePaymentResponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage PaymentByExactCash([FromBody] PaymentModel paymentModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug(
                $"Start,PaymentController,PaymentByExactCash,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            string fileName = string.Empty;
            Report receipt = null;
            ErrorMessage errorMessage;
            CustomerDisplay lcdMsg;
            var sale = _paymentManager.ByCashExact(paymentModel.TillNumber,
                paymentModel.SaleNumber,
                userCode, ref receipt, ref fileName,
                out errorMessage, out lcdMsg);


            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle?.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }

            var recieptModel = new ReportModel
            {
                ReportName = receipt.ReportName,
                ReportContent = receipt.ReportContent,
                Copies = 1
            };
            var tillLimitExeceeded = _paymentManager.CheckTillLimit(sale.TillNumber);
            var completePaymentResponseModel = new CompletePaymentResponseModel
            {
                NewSale = new NewSale
                {
                    SaleNumber = sale.Sale_Num,
                    TillNumber = sale.TillNumber,
                    Customer = sale.Customer.Name
                },
                LimitExceedMessage = tillLimitExeceeded,
                PaymentReceipt = recieptModel,
                CustomerDisplays = new List<CustomerDisplay> { lcdMsg, sale.CustomerDisplay }
            };
            _performancelog.Debug(
                $"End,PaymentController,PaymentByExactCash,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, completePaymentResponseModel);
        }

        /// <summary>
        /// Complete a transaction as run away
        /// </summary>
        /// <param name="paymentModel">Payment model</param>
        /// <returns></returns>
        [Route("runAway")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CompletePaymentResponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public HttpResponseMessage RunAway([FromBody] TransactionCompleteModel paymentModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentController,RnAway,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            Report receipt = null;

            var newSale = _paymentManager.RunAway(paymentModel.SaleNumber,
                                    paymentModel.TillNumber,
                                    userCode, ref receipt,
                                    out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }
            CompletePaymentResponseModel completePaymentResponseModel = null;
            if (receipt != null)
            {
                completePaymentResponseModel = new CompletePaymentResponseModel
                {
                    NewSale = new NewSale
                    {
                        SaleNumber = newSale.Sale_Num,
                        TillNumber = newSale.TillNumber,
                        Customer = newSale.Customer.Name
                    },
                    PaymentReceipt = new ReportModel
                    {
                        ReportName = receipt.ReportName,
                        ReportContent = receipt.ReportContent,
                        Copies = 1
                    },
                    CustomerDisplays = new List<CustomerDisplay> { newSale.CustomerDisplay }
                };
            }

            _performancelog.Debug($"End,PaymentController,RunAway,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, completePaymentResponseModel);

        }

        /// <summary>
        /// Complete a transaction as pump test
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        [Route("pumpTest")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CompletePaymentResponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public HttpResponseMessage PumpTest([FromBody] TransactionCompleteModel paymentModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentController,PumpTest,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            Sale newSale;
            var report = _paymentManager.CompletePumpTest(paymentModel.SaleNumber,
                                    paymentModel.TillNumber,
                                    userCode, out newSale,
                                    out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }
            CompletePaymentResponseModel completePaymentResponseModel = null;
            if (report != null)
            {

                completePaymentResponseModel = new CompletePaymentResponseModel
                {
                    NewSale = new NewSale
                    {
                        SaleNumber = newSale.Sale_Num,
                        TillNumber = newSale.TillNumber,
                        Customer = newSale.Customer.Name
                    },
                    PaymentReceipt = new ReportModel
                    {
                        ReportName = report.ReportName,
                        ReportContent = report.ReportContent,
                        Copies = 1
                    },
                    CustomerDisplays = new List<CustomerDisplay> { newSale.CustomerDisplay }
                };
            }

            _performancelog.Debug($"End,PaymentController,PumpTest,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, completePaymentResponseModel);

        }

        /// <summary>
        /// Method to complete payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("complete")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FinishPaymentResponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage CompletePayment([FromBody] CompletePaymentInputModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentController,CompletePayment,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
           
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
                       Error = new MessageStyle { Message = Constants.InvalidRequest, MessageType = 0 }
                   });
            }

            bool openCashDrawer;
            bool isRefund;
            string changeDue;
            CustomerDisplay lcdMsg;
            var receipts = _paymentManager.CompletePayment(model.SaleNumber, model.TillNumber,
                model.TransactionType, userCode, model.IssueStoreCredit, out errorMessage, out openCashDrawer,
                out changeDue, out isRefund, out lcdMsg);

            var kickbackCommunicationMsg = _paymentManager.KickbackCommunicationError();
            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle?.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }

            var newSale = _saleManager.InitializeSale(model.TillNumber, model.RegisterNumber, userCode, out errorMessage);
            var tillLimitExeceeded = _paymentManager.CheckTillLimit(model.TillNumber);
            var completePaymentResponseModel = new FinishPaymentResponseModel
            {
                NewSale = new NewSale
                {
                    SaleNumber = newSale.Sale_Num,
                    TillNumber = newSale.TillNumber,
                    Customer = newSale.Customer.Name
                },
                OpenCashDrawer = openCashDrawer,
                LimitExceedMessage = tillLimitExeceeded,
                ChangeDue = changeDue,
                CustomerDisplays = new List<CustomerDisplay> { lcdMsg, newSale.CustomerDisplay },
                Receipts = receipts,
                kickabckServerError= kickbackCommunicationMsg

            };

            KickBackManager.ExchangeRate = 0;
            return Request.CreateResponse(HttpStatusCode.OK, completePaymentResponseModel);

        }

        /// <summary>
        /// Method to perform a transaction by card
        /// </summary>
        /// <param name="paymentModel">Payment model</param>
        /// <returns>tender summary</returns>
        [Route("byCard")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TenderSummaryModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public HttpResponseMessage PaymentByCard([FromBody]PaymentModel paymentModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentController,PaymentByCard,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            Report merchantStream = null;
            Report customerStream = null;
            var updatedTenders = _paymentManager.ByCard(paymentModel.SaleNumber,
                                    paymentModel.TillNumber,
                                    paymentModel.CardNumber,
                                    userCode,
                                    paymentModel.TransactionType,
                                    paymentModel.PONumber,
                                    paymentModel.AmountUsed,
                                    ref merchantStream,
                                    ref customerStream,
                                    out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }

            TenderSummaryModel tenderSummary = GetTenderSummary(updatedTenders);
            if (merchantStream != null || customerStream != null)
            {
                tenderSummary.Receipts = new List<ReportModel>();
                if (merchantStream != null)
                {
                    tenderSummary.Receipts.Add(new ReportModel
                    {
                        ReportName = merchantStream.ReportName,
                        ReportContent = merchantStream.ReportContent,
                        Copies = 1
                    });
                }
                if (customerStream != null)
                {
                    tenderSummary.Receipts.Add(new ReportModel
                    {
                        ReportName = customerStream.ReportName,
                        ReportContent = customerStream.ReportContent,
                        Copies = 1
                    });
                }
            }
            _performancelog.Debug($"End,PaymentController,PaymentByCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, tenderSummary);
        }

        /// <summary>
        /// Method to complete transaction by fuel coupon
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        [Route("byCoupon")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TenderSummaryModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public HttpResponseMessage PaymentByCoupon([FromBody]PaymentModel paymentModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentController,PaymentByCoupon,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var updatedTenders = _paymentManager.ByCoupon(paymentModel.SaleNumber,
                                    paymentModel.TillNumber, paymentModel.TransactionType, paymentModel.CouponNumber,
                                    paymentModel.TillClose, paymentModel.TenderCode, userCode,
                                    out errorMessage);

            //Added scenario when "Amount Cannot Exceed value of sale." 
            //should be displayed and countinue to use the coupon amount
            var store = CacheManager.GetStoreInfo();
            var offSet = store?.OffSet ?? 0;
            var messageStyle = _resourceManager.CreateMessage(offSet, 14, 88, null, MessageType.OkOnly);
            var messages = new List<object>();
            if (messageStyle.Message.Equals(errorMessage.MessageStyle.Message))
            {
                var msg = new
                {
                    error = messageStyle
                };
                messages.Add(msg);
            }
            else
            {
                if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                {
                    return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
                }
            }
            TenderSummaryModel tenderSummary = GetTenderSummary(updatedTenders);
            tenderSummary.Messages = messages;
            _performancelog.Debug($"End,PaymentController,PaymentByCoupon,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, tenderSummary);
        }

        /// <summary>
        /// Method to get all selected sale vendor coupons
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderCode">Tender code</param>
        /// <returns>List of vendor coupons</returns>
        [Route("getVendorCoupon")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AddVendorCouponResposeModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetSaleVendorCoupons(int saleNumber, int tillNumber,
            string tenderCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentController,GetSaleVendorCoupons,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string coupon = string.Empty;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            AddVendorCouponResposeModel model = new AddVendorCouponResposeModel
            {
                SaleVendorCoupons = _paymentManager.GetSaleVendorCoupon(saleNumber,
                    tillNumber, userCode, tenderCode, ref coupon, out errorMessage)
            };
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }
            if (model.SaleVendorCoupons.Count == 0 && !string.IsNullOrEmpty(coupon))
            {
                model.DefaultCoupon = coupon;
            }
            _performancelog.Debug($"End,PaymentController,GetSaleVendorCoupons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        /// <summary>
        /// Method to add vendor coupons
        /// </summary>
        /// <param name="vendorCouponModel">Vendor coupon model</param>
        /// <returns>List of updated tenders</returns>
        [Route("addVendorCoupon")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TenderSummaryModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public HttpResponseMessage AddSaleVendorCoupon([FromBody]AddVendorCouponInputModel vendorCouponModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentController,AddSaleVendorCoupon,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var model = new AddVendorCouponResposeModel
            {
                SaleVendorCoupons = _paymentManager.AddSaleVendorCoupon(vendorCouponModel.SaleNumber,
                    vendorCouponModel.TillNumber, userCode, vendorCouponModel.TenderCode, vendorCouponModel.CouponNumber,
                    vendorCouponModel.SerialNumber, out errorMessage)
            };
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }
            var updatedTenders = CacheManager.GetTenderForSale(vendorCouponModel.SaleNumber, vendorCouponModel.TillNumber);
            TenderSummaryModel tenderSummary = GetTenderSummary(updatedTenders);
            tenderSummary.VendorCoupons = model.SaleVendorCoupons;
            _performancelog.Debug($"End,PaymentController,AddSaleVendorCoupon,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, tenderSummary);
        }

        /// <summary>
        /// Method to add vendor coupons
        /// </summary>
        /// <param name="vendorCouponModel">Vendor coupon model</param>
        /// <returns>List of updated tenders</returns>
        [Route("removeVendorCoupon")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TenderSummaryModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public HttpResponseMessage RemoveSaleVendorCoupon([FromBody]RemoveVendorCouponInputModel vendorCouponModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentController,RemoveSaleVendorCoupon,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var model = new AddVendorCouponResposeModel
            {
                SaleVendorCoupons = _paymentManager.RemoveSaleVendorCoupon(vendorCouponModel.SaleNumber,
                    vendorCouponModel.TillNumber, userCode, vendorCouponModel.TenderCode, vendorCouponModel.CouponNumber,
                    out errorMessage)
            };
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }
            var updatedTenders = CacheManager.GetTenderForSale(vendorCouponModel.SaleNumber, vendorCouponModel.TillNumber);
            TenderSummaryModel tenderSummary = GetTenderSummary(updatedTenders);
            tenderSummary.VendorCoupons = model.SaleVendorCoupons;
            _performancelog.Debug($"End,PaymentController,RemoveSaleVendorCoupon,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, tenderSummary);
        }


        /// <summary>
        /// Method to complete payment by sale vendor coupon
        /// </summary>
        /// <param name="vendorCouponModel"></param>
        /// <returns></returns>
        [Route("byVendorCoupon")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TenderSummaryModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public HttpResponseMessage PaymentByVendorCoupon([FromBody]PaymentByVCouponModel vendorCouponModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentController,PaymentByVCoupon,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var updatedTenders = _paymentManager.PaymentByVCoupon(vendorCouponModel.SaleNumber,
                vendorCouponModel.TillNumber, vendorCouponModel.TenderCode, userCode, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }
            TenderSummaryModel tenderSummary = GetTenderSummary(updatedTenders);
            _performancelog.Debug($"End,PaymentController,PaymentByVCoupon,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, tenderSummary);
        }


        /// <summary>
        /// Verifies the status of payment for the current sale using Account Tender
        /// </summary>
        /// <param name="tenderModel">Tender model</param>
        /// <returns></returns>
        [Route("verifyByAccount")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VerifyAccountPaymentModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Forbidden, Type = typeof(ErrorResponse))]
        [HttpPost]
        public HttpResponseMessage VerifyPaymentByAccount(UpdateTenderModel tenderModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentController,VerifyPaymentByAccount,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var verifyAccount = _paymentManager.VerifyPaymentByAccount(tenderModel.SaleNumber,
                                    tenderModel.TillNumber, tenderModel.TransactionType,
                                    tenderModel.Tender.AmountEntered.HasValue ? tenderModel.Tender.AmountEntered.ToString() : null,
                                    tenderModel.TillClose, tenderModel.Tender.TenderCode, userCode,
                                    out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }

            var verifyAccountModel = new VerifyAccountPaymentModel
            {
                CreditMessage = !string.IsNullOrEmpty(verifyAccount.CreditMessage) ?
                new MessageStyle { Message = verifyAccount.CreditMessage, MessageType = 0 } : null,
                OverrideArLimitMessage = !string.IsNullOrEmpty(verifyAccount.OverrideARLimitMessage) ?
                new MessageStyle { Message = verifyAccount.OverrideARLimitMessage, MessageType = 0 } : null,
                IsPurchaseOrderRequired = verifyAccount.IsPurchaseOrderRequired,
                IsMutiliPO = verifyAccount.UseMultiPO,
                CardSummary = verifyAccount.CardSummary == null ? null : new CardInformationResponseModel
                {
                    Amount = verifyAccount.CardSummary.Amount,
                    AskPin = verifyAccount.CardSummary.AskPin,
                    Caption = verifyAccount.CardSummary.Caption,
                    CardNumber = verifyAccount.CardSummary.CardNumber,
                    CardType = verifyAccount.CardSummary.SelectedCard,
                    IsArCustomer = verifyAccount.CardSummary.IsArCustomer,
                    Pin = verifyAccount.CardSummary.Pin,
                    POMessage = verifyAccount.CardSummary.PONumber,
                    ProfileId = verifyAccount.CardSummary.ProfileId,
                    ProfileValidations = verifyAccount.CardSummary.ValidationMessages,
                    PromptMessages = verifyAccount.CardSummary.PromptMessages,
                    TenderClass = verifyAccount.CardSummary.TenderClass,
                    TenderCode = verifyAccount.CardSummary.TenderCode
                },
                UnauthorizedMessage = !string.IsNullOrEmpty(verifyAccount.UnauthorizedMessage) ?
                new MessageStyle { Message = verifyAccount.UnauthorizedMessage, MessageType = MessageType.YesNo } : null
            };

            _performancelog.Debug($"End,PaymentController,VerifyPaymentByAccount,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, verifyAccountModel);
        }

        /// <summary>
        /// Verifies if entered PO is already entered
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="purchaseOrder">purchase order</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns></returns>
        [Route("validatePO")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SuccessReponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Forbidden, Type = typeof(ErrorResponse))]
        [HttpGet]
        public HttpResponseMessage ValidatePo(int saleNumber, int tillNumber, string purchaseOrder)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentController,ValidatePO,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var verifyPo = _tenderManager.ValidateMuliPo(saleNumber, tillNumber, purchaseOrder, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }
            _performancelog.Debug($"End,PaymentController,ValidatePO,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, new SuccessReponse
            {
                Success = verifyPo
            });
        }


        /// <summary>
        /// Completes the payment of the current sale using Account Tender
        /// </summary>
        /// <param name="tenderModel">tender model</param>
        /// <returns></returns>
        [Route("byAccount")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TenderSummaryModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Forbidden, Type = typeof(ErrorResponse))]
        [HttpPost]
        public HttpResponseMessage PaymentByAccount(UpdateAccountTenderModel tenderModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentController,PaymentByAccount,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var updatedTenders = _paymentManager.ByAccount(tenderModel.SaleNumber,
                                    tenderModel.TillNumber, tenderModel.TransactionType,
                                    tenderModel.Tender.AmountEntered.HasValue ? tenderModel.Tender.AmountEntered.ToString() : null,
                                    tenderModel.TillClose, tenderModel.Tender.TenderCode, userCode, tenderModel.PurchaseOrder,
                                    tenderModel.OverrideArLimit, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }

            TenderSummaryModel tenderSummary = GetTenderSummary(updatedTenders);
            _performancelog.Debug($"End,PaymentController,PaymentByAccount,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, tenderSummary);
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

        /// <summary>
        /// Method to get tender summary
        /// </summary>
        /// <param name="updatedTenders">Updated summary</param>
        /// <returns>Tender summary model</returns>
        private TenderSummaryModel GetTenderSummary(Tenders updatedTenders)
        {
            var selectedTenders = updatedTenders.Where(t => t.Amount_Entered != 0);
            string issueStoreCreditMessage = string.Empty;

            if (updatedTenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(updatedTenders);
            }
            var tenderSummary = TenderMapper.GetTenderSummaryModel(updatedTenders, issueStoreCreditMessage, selectedTenders);
            return tenderSummary;
        }

        #endregion

    }//end class    
}//end namespace

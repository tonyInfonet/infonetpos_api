using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Mapper;
using Infonet.CStoreCommander.WebApi.Models;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Givex;
using Infonet.CStoreCommander.WebApi.Models.Report;
using Infonet.CStoreCommander.WebApi.Models.Sale;
using Infonet.CStoreCommander.WebApi.Models.Tender;
using Infonet.CStoreCommander.WebApi.Resources;
using Infonet.CStoreCommander.WebApi.Utilities;
using log4net;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Tender controller
    /// </summary>
    [RoutePrefix("api/v1/tenders")]
    [ApiAuthorization]
    public class TenderController : ApiController
    {
        private readonly ITenderManager _tenderManager;
        private readonly IOverLimitManager _overLimitManager;
        private readonly IOverrideLimitManager _overrideLimitManager;
        private readonly ITaxManager _taxManager;
        private readonly ISaleManager _saleManager;
        private readonly IPolicyManager _policyManager;
        private readonly IKickBackManager _kickBackManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;
        private readonly IWexManager _wexManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="tenderManager"></param>
        /// <param name="overLimitManager"></param>
        /// <param name="overrideLimitManager"></param>
        /// <param name="taxManager"></param>
        public TenderController(
            ITenderManager tenderManager,
            IOverLimitManager overLimitManager,
            IOverrideLimitManager overrideLimitManager,
            ITaxManager taxManager,
            ISaleManager saleManager,
            IPolicyManager policyManager,
            IKickBackManager kickBackManager,
            IWexManager wexManager)
        {
            _tenderManager = tenderManager;
            _overLimitManager = overLimitManager;
            _overrideLimitManager = overrideLimitManager;
            _taxManager = taxManager;
            _saleManager = saleManager;
            _policyManager = policyManager;
            _kickBackManager = kickBackManager;
            _wexManager = wexManager;
        }



        /*Added by sonali 27 Dec 2017 */

        /// <summary>
        /// Verify Tax Exampt
        /// </summary>
        ///  <param name="PointCardNumber"></param>
        ///  <param name="PhoneNumber"></param>
        ///  <param name="tillNumber"></param>
        /// <param name="saleNumber"></paramm>
      
        /// <returns></returns>
        [Route("verifyKickBack")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VerifyKickBackModel))]
        public HttpResponseMessage VerifyKickBack(string PointCardNumber,string PhoneNumber,byte registerNumber, int tillNumber, int saleNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,VerifyKickBack,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage=null;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            Sale sale;

            bool usePhoneNumber = false;

            var result= _kickBackManager.VerifyKickBack(PointCardNumber,
                PhoneNumber,tillNumber,saleNumber,registerNumber, userCode,out errorMessage,out sale, ref usePhoneNumber);
            if (errorMessage != null)
            {
                if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                {
                    return Request.CreateResponse(
                   errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = errorMessage.MessageStyle.Message }
                    });
                    //add response
                }
            }
            if (result == -1)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = Resource.InvalidRequest, MessageType = 0 }
                    });
                
            }
            var resp = (result == -2) ? "false" : "true";
            if (usePhoneNumber)
            {
                resp = "false";                
            }

            var response = new
            {
                verify = resp,
                BalancePoints = result,
                value= (result * sale.Customer.Points_ExchangeRate).ToString("0.00"),
              
        };

            var s = sale;
            _performancelog.Debug($"End,TenderController,VerifyKickBack,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");



            return Request.CreateResponse(HttpStatusCode.OK,response);
        }



        /*    Ended By Sonali 27 Dec 2017                            */



        /// <summary>
        /// Get All Tenders
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <param name="transactionType"></param>
        /// <param name="billTillClose"></param>
        /// <param name="dropReason"></param>
        /// <returns></returns>
        [Route("getTenders")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<TenderSummaryModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetAllTenders(int saleNumber, int tillNumber, string transactionType,
            bool billTillClose, string dropReason)
        {

            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,GetAllTenders,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var tenders = _tenderManager.GetAllTender(saleNumber, tillNumber, transactionType, userCode, billTillClose,
                dropReason, out error);
            _performancelog.Debug($"End,TenderController,GetAllTenders,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(error.StatusCode, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }
            string issueStoreCreditMessage = string.Empty;

            if (tenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(tenders);
            }
            List<Report> transactReports = null;
            //if (!string.IsNullOrEmpty(kickBackAmount))
            //{
            //   tenders = _tenderManager.UpdateTenders(saleNumber,tillNumber,transactionType, userCode, billTillClose,"KICKBACK", kickBackAmount,out transactReports, out error);
            //}
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(error.StatusCode, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }
            var tenderSummary = TenderMapper.GetTenderSummaryModel(tenders, issueStoreCreditMessage, tenders);
            return Request.CreateResponse(HttpStatusCode.OK, tenderSummary);
        }

        /// <summary>
        /// Update tender values
        /// </summary>
        ///<param name="tenderModel">Tender model</param>
        /// <returns></returns>
        [Route("updateTenders")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<TenderSummaryModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage UpdateTenders(UpdateTenderModel tenderModel, bool isAmountEnteredManually)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,UpdateTenders,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (tenderModel == null)
            {
                return Request.CreateResponse(
                   HttpStatusCode.BadRequest,
                   new ErrorResponse
                   {
                       Error = new MessageStyle { Message = Constants.InvalidRequest, MessageType = 0 }
                   });
            }

            var amountEntered = tenderModel.Tender.AmountEntered.HasValue ? tenderModel.Tender.AmountEntered.ToString() : string.Empty;
            List<Report> transactionReports;
            var updatedTenders = _tenderManager.UpdateTenders(tenderModel.SaleNumber, tenderModel.TillNumber,
                tenderModel.TransactionType, userCode, tenderModel.TillClose, tenderModel.Tender.TenderCode, amountEntered,
                out transactionReports, out error, true, isAmountEnteredManually);
            _performancelog.Debug($"End,TenderController,UpdateTenders,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            List<object> messages = null;
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                if (error.StatusCode != HttpStatusCode.OK)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                    {
                        Error = error.MessageStyle
                    });
                }
                messages = new List<object>();
                var msg = new
                {
                    error = error.MessageStyle
                };
                messages.Add(msg);
            }
            var selectedTenders = updatedTenders.Where(t => t.Amount_Entered != 0);
            string issueStoreCreditMessage = string.Empty;

            if (updatedTenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(updatedTenders);
            }
            var tenderSummary = TenderMapper.GetTenderSummaryModel(updatedTenders, issueStoreCreditMessage,
                selectedTenders);

            //if(tenderSummary.Tenders.Count==1)
            //{
            //    if (tenderSummary.Tenders[0].TenderCode == "KICKBACK")
            //    {
            //        error.MessageStyle.Message = "Cannot use this tender";
            //        error.StatusCode = HttpStatusCode.BadRequest;
            //        return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
            //        {
            //            Error = error.MessageStyle
            //        });
            //    }
            //}
            if (transactionReports != null)
            {
                tenderSummary.Receipts = new List<ReportModel>();
                foreach (var report in transactionReports)
                {
                    if (report != null)
                    {
                        tenderSummary.Receipts.Add(new ReportModel
                        {
                            ReportName = report.ReportName,
                            ReportContent = report.ReportContent,
                            Copies = report.Copies == 0 ? 1 : report.Copies
                        });
                    }
                }
            }
            if (messages != null)
                tenderSummary.Messages = messages;
            return Request.CreateResponse(HttpStatusCode.OK, tenderSummary);
        }

        /// <summary>
        /// Get Information about tender using card number
        /// </summary>
        ///<param name="cardModel">Card model</param>
        /// <returns></returns>
        [Route("getCardInformation")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<CardInformationResponseModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetCardInformation(CardModel cardModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,GetCardInformation,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (cardModel == null)
            {
                return Request.CreateResponse(
                   HttpStatusCode.BadRequest,
                   new ErrorResponse
                   {
                       Error = new MessageStyle { Message = Constants.InvalidRequest, MessageType = 0 }
                   });
            }
            Credit_Card cc = new Credit_Card();
            var cardSummary = _tenderManager.FindCardTender(cardModel.CardNumber, cardModel.SaleNumber,
               cardModel.TillNumber, cardModel.TransactionType, userCode, out error, ref cc);
            _performancelog.Debug($"End,TenderController,GetCardInformation,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            if (cardSummary != null)
            {
                if (cardSummary.SelectedCard.Equals("Fleet"))
                {
                    cardSummary.IsFleet = true;
                }
                else
                {
                    cardSummary.IsFleet = false;
                }
                var gasking = _kickBackManager.ValidateGasKing(cardModel.TillNumber, cardModel.SaleNumber, 1,
                        userCode, out error, true);
                if (gasking == null)
                {
                    cardSummary.IsKickBackLinked = false;
                    cardSummary.KickbackPoints = 0;
                    cardSummary.KickBackValue = null;

                }
                else
                {
                    cardSummary.IsKickBackLinked = gasking.IsKickBackLinked;
                    cardSummary.KickbackPoints = gasking.PointsReedemed;
                    cardSummary.KickBackValue = gasking.Value;
                }

            }
            if (!string.IsNullOrEmpty(error.MessageStyle.Message) &&
                 !error.MessageStyle.Message.Equals("Invalid loyalty card"))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }

            var model = new CardInformationResponseModel
            {
                IsInvalidLoyaltyCard = !string.IsNullOrEmpty(error.MessageStyle.Message) &&  
                                            error.MessageStyle.Message.Equals("Invalid loyalty card"),
                CardType = cardSummary.SelectedCard,
                Amount = cardSummary.Amount,
                Caption = cardSummary.Caption,
                CardNumber = cardSummary.CardNumber,
                TenderCode = cardSummary.TenderCode,
                AskPin = cardSummary.AskPin,
                Pin = cardSummary.Pin,
                PromptMessages = cardSummary.PromptMessages,
                ProfileId = cardSummary.ProfileId,
                POMessage = cardSummary.PONumber,
                ProfileValidations = cardSummary.ValidationMessages,
                IsArCustomer = cardSummary.IsArCustomer,
                TenderClass = cardSummary.TenderClass,
                IsKickBackLinked = cardSummary.IsKickBackLinked,
                KickbackPoints=cardSummary.KickbackPoints,
                KickBackValue=cardSummary.KickBackValue,
                IsFleet=cardSummary.IsFleet

            };
            var s = _saleManager.GetCurrentSale(cardModel.SaleNumber, cardModel.TillNumber, 1, userCode, out error);
         
            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        /// <summary>
        /// Get list of gift certificates
        /// <param name="tenderModel">Tender model</param>
        /// </summary>
        /// <returns></returns>
        [Route("getGiftCerts")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<GiftCertificateResponseModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetGiftCertificates(UpdateTenderModel tenderModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,GetGiftCertificates,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (tenderModel == null)
            {
                return Request.CreateResponse(
                   HttpStatusCode.BadRequest,
                   new ErrorResponse
                   {
                       Error = new MessageStyle { Message = Resource.InvalidRequest, MessageType = 0 }
                   });
            }

            var amountEntered = tenderModel.Tender.AmountEntered.HasValue ? tenderModel.Tender.AmountEntered.ToString() : string.Empty;

            var giftCertificates = _tenderManager.GetGiftCertificates(tenderModel.SaleNumber, tenderModel.TillNumber,
                userCode, tenderModel.Tender.TenderCode, amountEntered, tenderModel.TransactionType, out error);

            _performancelog.Debug($"End,TenderController,GetGiftCertificates,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }
            if (giftCertificates == null || giftCertificates.Count == 0)
                giftCertificates = new List<GiftCert>();
            var giftCerts = (from gc in giftCertificates
                             select new GiftCertificateResponseModel
                             {
                                 Number = gc.GcNumber,
                                 SoldOn = gc.GcDate.ToString("MM/dd/yyyy"),
                                 Amount = Math.Round(gc.GcAmount, 2),
                                 ExpiresOn = gc.GcExpiresOn.ToString("dd-MMM-yyyy"),
                                 IsExpired = gc.GcExpiresOn != DateTime.MinValue
                             }).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, giftCerts);
        }

        /// <summary>
        /// save selected gift certificates
        /// <param name="giftCertificateModel">Gift certificate model</param>
        /// </summary>
        /// <returns></returns>
        [Route("saveGiftCertificates")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<TenderSummaryModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage SaveGiftCertificates(GiftCertificateModel giftCertificateModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,GetGiftCertificates,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (giftCertificateModel == null)
            {
                return Request.CreateResponse(
                   HttpStatusCode.BadRequest,
                   new ErrorResponse
                   {
                       Error = new MessageStyle { Message = Constants.InvalidRequest, MessageType = 0 }
                   });
            }
            var giftCerts = new List<GiftCert>();
            foreach (var giftcert in giftCertificateModel.GiftCerts)
            {
                giftCerts.Add(new GiftCert
                {
                    GcNumber = giftcert.Number,
                    GcAmount = giftcert.Amount
                });
            }
            var updatedTenders = _tenderManager.SaveGiftCertificate(giftCertificateModel.SaleNumber,
                giftCertificateModel.TillNumber, giftCerts, giftCertificateModel.TransactionType,
                userCode, giftCertificateModel.TenderCode, out error);

            _performancelog.Debug($"End,TenderController,GetGiftCertificates,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }
            var selectedTenders = updatedTenders.Where(t => t.Amount_Entered != 0);
            string issueStoreCreditMessage = string.Empty;

            if (updatedTenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(updatedTenders);
            }
            var tenderSummary = TenderMapper.GetTenderSummaryModel(updatedTenders, issueStoreCreditMessage,
                selectedTenders);
            return Request.CreateResponse(HttpStatusCode.OK, tenderSummary);
        }

        /// <summary>
        /// Get list of Store Credits
        /// <param name="tenderModel">Tender model</param>
        /// </summary>
        /// <returns></returns>
        [Route("getStoreCredits")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<StoreCreditResponseModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetStoreCredits(UpdateTenderModel tenderModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,GetStoreCredits,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (tenderModel == null)
            {
                return Request.CreateResponse(
                   HttpStatusCode.BadRequest,
                   new ErrorResponse
                   {
                       Error = new MessageStyle { Message = Resource.InvalidRequest, MessageType = 0 }
                   });
            }

            var amountEntered = tenderModel.Tender.AmountEntered.HasValue ? tenderModel.Tender.AmountEntered.ToString() : string.Empty;

            var storeCredits = _tenderManager.GetStoreCredits(tenderModel.SaleNumber, tenderModel.TillNumber,
                userCode, tenderModel.Tender.TenderCode, amountEntered, tenderModel.TransactionType, out error);

            _performancelog.Debug($"End,TenderController,GetStoreCredits,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }
            if (storeCredits == null || storeCredits.Count == 0)
                storeCredits = new List<Store_Credit>();
            var storeCreditsModel = (from sc in storeCredits
                                     select new StoreCreditResponseModel
                                     {
                                         Number = sc.Number.ToString(),
                                         SoldOn = sc.SC_Date.ToString("MM/dd/yyyy"),
                                         Amount = Math.Round(sc.Amount, 2),
                                         ExpiresOn = sc.Expires_On.ToString("dd-MMM-yyyy"),
                                         IsExpired = sc.Expires_On != DateTime.MinValue
                                     }).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, storeCreditsModel);
        }

        /// <summary>
        /// save selected store credits
        /// <param name="storeCreditsModel">Store credits model</param>
        /// </summary>
        /// <returns></returns>
        [Route("saveStoreCredits")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<TenderSummaryModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage SaveStoreCredits(StoreCreditModel storeCreditsModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,SaveStoreCredits,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (storeCreditsModel == null)
            {
                return Request.CreateResponse(
                   HttpStatusCode.BadRequest,
                   new ErrorResponse
                   {
                       Error = new MessageStyle { Message = Resource.InvalidRequest, MessageType = 0 }
                   });
            }
            var storeCredits = new List<Store_Credit>();
            foreach (var storeCredit in storeCreditsModel.StoreCredits)
            {
                storeCredits.Add(new Store_Credit
                {
                    Number = string.IsNullOrEmpty(storeCredit.Number) ? 0 : Convert.ToInt32(storeCredit.Number),
                    Amount = storeCredit.AmountEntered
                });
            }
            var updatedTenders = _tenderManager.SaveStoreCredits(storeCreditsModel.SaleNumber,
                storeCreditsModel.TillNumber, storeCredits, storeCreditsModel.TransactionType,
                userCode, storeCreditsModel.TenderCode, out error);

            _performancelog.Debug($"End,TenderController,SaveStoreCredits,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }
            var selectedTenders = updatedTenders.Where(t => t.Amount_Entered != 0);
            string issueStoreCreditMessage = string.Empty;

            if (updatedTenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(updatedTenders);
            }
            var tenderSummary = TenderMapper.GetTenderSummaryModel(updatedTenders, issueStoreCreditMessage,
               selectedTenders);
            return Request.CreateResponse(HttpStatusCode.OK, tenderSummary);
        }

        /// <summary>
        /// save selected gift certificates
        /// <param name="givexModel">Gift certificate model</param>
        /// </summary>
        /// <returns></returns>
        [Route("saveGivex")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<TenderSummaryModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage SaveGivex(GivexCardModel givexModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,GetGiftCertificates,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (givexModel == null)
            {
                return Request.CreateResponse(
                   HttpStatusCode.BadRequest,
                   new ErrorResponse
                   {
                       Error = new MessageStyle { Message = Resource.InvalidRequest, MessageType = 0 }
                   });
            }
            var amount = givexModel.Amount == null ? "" : givexModel.Amount.ToString();
            int givexCopies;
            Report report;
            var updatedTenders = _tenderManager.SaveGivexSale(givexModel.SaleNumber,
                givexModel.TillNumber, givexModel.GivexCardNumber, givexModel.TransactionType, givexModel.TenderCode,
                userCode, amount, out error, out report, out givexCopies);

            _performancelog.Debug($"End,TenderController,SaveGivex,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }
            var selectedTenders = updatedTenders.Where(t => t.Amount_Entered != 0);
            string issueStoreCreditMessage = string.Empty;

            if (updatedTenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(updatedTenders);
            }
            try
            {
                var givexReceipt = new ReportModel
                {
                    ReportName = report.ReportName,
                    ReportContent = report.ReportContent,
                    Copies = givexCopies == 0 ? 1 : givexCopies
                };
                var tenderSummary = TenderMapper.GetTenderSummaryModel(updatedTenders, issueStoreCreditMessage,
              selectedTenders);
                tenderSummary.Receipts = new List<ReportModel> { givexReceipt };
                return Request.CreateResponse(HttpStatusCode.OK, tenderSummary);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                });
            }

        }

        /// <summary>
        /// Update tender values
        /// </summary>
        ///<param name="saleNumber">SaleNumber</param>
        ///<param name="tillNumber">Till Number</param>
        ///<param name="transactionType">Transaction type</param>
        /// <returns></returns>
        [Route("cancelTenders")]
        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage CancelTenders(int saleNumber, int tillNumber, string transactionType)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,CancelTenders,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var sale = _tenderManager.CancelTender(saleNumber, tillNumber, userCode, transactionType, out error);
            _performancelog.Debug($"End,TenderController,CancelTenders,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }
            if (sale != null)
            {
                var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                var enableButton = _saleManager.EnableCashButton(sale, userCode);
                var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
                var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButton, userCanWriteOff);
                _performancelog.Debug($"End,SaleController,CancelTenders,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.OK, saleModel);
            }
            return null;

        }

        /// <summary>
        /// Verify Tax Exampt
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <param name="registerNumber"></param>
        /// <returns></returns>
        [Route("verifyTaxExempt")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VerifyTaxExemptModel))]
        public HttpResponseMessage VerifyTaxExempt(int saleNumber, int tillNumber, int registerNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,GetAllTenders,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = _taxManager.VerifyTaxExampt(saleNumber, tillNumber, registerNumber, userCode, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            VerifyTaxExemptModel model = new VerifyTaxExemptModel
            {
                ProcessAite = result.ProcessAite,
                ProcessSiteSale = result.ProcessSiteSale,
                ProcessSiteSaleRemoveTax = result.ProcessSiteSaleReturnTax,
                ProcessSiteReturn = result.ProcessSiteReturn,
                ConfirmMessage = result.ConfirmMessage.MessageStyle,
                ProcessQite = result.ProcessQite,
                TreatyNumber = result.TreatyNumber,
                TreatyName = result.TreatyName
            };
            _performancelog.Debug($"End,TenderController,GetAllTenders,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }


        /// <summary>
        /// Remove Tax
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("site/removeTax")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TreatyNumberResponseModel))]
        public HttpResponseMessage RemoveTax([FromBody] TreatyNumberModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,RemoveTax,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = _taxManager.RemoveTax(model.SaleNumber, model.TillNumber, model.TreatyNumber, model.RegisterNumber, userCode, model.CaptureMethod, model.PermitNumber, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            string issueStoreCreditMessage = string.Empty;

            if (result.Tenders != null && result.Tenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(result.Tenders);
            }
            _performancelog.Debug($"End,TenderController,RemoveTax,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            var treatyNumberResult = new TreatyNumberResponseModel
            {
                TreatyNumber = result.TreatyNumber,
                TreatyCustomerName = result.TreatyCustomerName,
                IsFrmOverrideLimit = result.IsFrmOverrideLimit,
                PermitNumber = result.PermitNumber,
                IsFngtr = result.IsFngtr,
                FngtrMessage = result.FngtrMessage,
                SaleSummary = result.SaleSummary != null ? (from taxSumm in result.SaleSummary
                                                            select new NameValuePair
                                                            {
                                                                Key = taxSumm.Key,
                                                                Value = taxSumm.Value
                                                            }).ToList() : null,
                TenderSummary = result.Tenders != null ? TenderMapper.GetTenderSummaryModel(result.Tenders,
                issueStoreCreditMessage, result.Tenders) : null
            };
            return Request.CreateResponse(HttpStatusCode.OK, treatyNumberResult);
        }


        /// <summary>
        /// Validate Treaty Number
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("site/validate")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TreatyNumberResponseModel))]
        public HttpResponseMessage ValidateTreatyNumber([FromBody]TreatyNumberModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,ValidateTreatyNumber,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = _taxManager.Validate(model.SaleNumber, model.TillNumber, model.TreatyNumber,
                model.TreatyName, model.PermitNumber, model.RegisterNumber, userCode, model.CaptureMethod, model.IsEnterPress, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            string issueStoreCreditMessage = string.Empty;

            if (result.Tenders != null && result.Tenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(result.Tenders);
            }
            var treatyNumberResult = new TreatyNumberResponseModel
            {
                TreatyNumber = result.TreatyNumber,
                TreatyCustomerName = result.TreatyCustomerName,
                IsFrmOverrideLimit = result.IsFrmOverrideLimit,
                PermitNumber = result.PermitNumber,
                IsFngtr = result.IsFngtr,
                FngtrMessage = result.FngtrMessage,
                SaleSummary = result.SaleSummary != null ? (from taxSumm in result.SaleSummary
                                                            select new NameValuePair
                                                            {
                                                                Key = taxSumm.Key,
                                                                Value = taxSumm.Value
                                                            }).ToList() : null,
                TenderSummary = result.Tenders != null ? TenderMapper.GetTenderSummaryModel(result.Tenders,
                issueStoreCreditMessage, result.Tenders) : null,
                RequireSignature = _policyManager.TE_SIGNATURE && !_policyManager.TE_ByRate
            };
            _performancelog.Debug($"End,TenderController,ValidateTreatyNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, treatyNumberResult);
        }


        /// <summary>
        /// Validate Treaty Number
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("site/fngtr")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TreatyNumberResponseModel))]
        public HttpResponseMessage ValidateFngtr([FromBody]FngtrModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,ValidateFngtr,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = _taxManager.ProcessFngtrSale(model.TillNumber, model.SaleNumber, model.RegisterNumber, model.PhoneNumber, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            string issueStoreCreditMessage = string.Empty;

            if (result.Tenders != null && result.Tenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(result.Tenders);
            }
            var treatyNumberResult = new TreatyNumberResponseModel
            {
                TreatyNumber = result.TreatyNumber,
                TreatyCustomerName = result.TreatyCustomerName,
                IsFrmOverrideLimit = result.IsFrmOverrideLimit,
                PermitNumber = result.PermitNumber,
                IsFngtr = result.IsFngtr,
                FngtrMessage = result.FngtrMessage,
                SaleSummary = result.SaleSummary != null ? (from taxSumm in result.SaleSummary
                                                            select new NameValuePair
                                                            {
                                                                Key = taxSumm.Key,
                                                                Value = taxSumm.Value
                                                            }).ToList() : null,
                TenderSummary = result.Tenders != null ? TenderMapper.GetTenderSummaryModel(result.Tenders,
                issueStoreCreditMessage, result.Tenders) : null
            };
            _performancelog.Debug($"End,TenderController,ValidateFngtr,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, treatyNumberResult);
        }



        /// <summary>
        /// Validate AITE CARD Number
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("aite/validate")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AiteCardResponseModel))]
        public HttpResponseMessage ValidateAiteCard([FromBody]AiteCardModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,ValidateAiteCard,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = _taxManager.ValidateAiteCard(model.SaleNumber, model.TillNumber, model.ShiftNumber, model.CardNumber,
                model.BarCode, model.RegisterNumber, model.CheckMode, userCode, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }

            _performancelog.Debug($"End,TenderController,ValidateAiteCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            string issueStoreCreditMessage = string.Empty;

            if (result.Tenders != null && result.Tenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(result.Tenders);
            }
            var aiteCardResponse = new AiteCardResponseModel
            {
                AiteCardNumber = result.AiteCardNumber,
                AiteCardHolderName = result.AiteCardHolderName,
                BarCode = result.BarCode,
                IsFrmOverLimit = result.IsFrmOverLimit,
                SaleSummary = result.SaleSummary != null ? (from taxSumm in result.SaleSummary
                                                            select new NameValuePair
                                                            {
                                                                Key = taxSumm.Key,
                                                                Value = taxSumm.Value
                                                            }).ToList() : null,
                TenderSummary = result.Tenders != null ? TenderMapper.GetTenderSummaryModel(result.Tenders,
                issueStoreCreditMessage, result.Tenders) : null
            };
            return Request.CreateResponse(HttpStatusCode.OK, aiteCardResponse);
        }

        /// <summary>
        /// Affix AITE Bar Code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("aite/affixBarCode")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SuccessReponse))]
        public HttpResponseMessage AffixAiteBarCode([FromBody] AffixBarCodeModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,AffixAiteBarCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            var result = _taxManager.AffixBarCode(model.CardNumber, model.BarCode, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }

            _performancelog.Debug($"End,TenderController,AffixAiteBarCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, new SuccessReponse
            {
                Success = result
            });
        }

        /// <summary>
        /// GST/PST Tax Exempt
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("aite/gstPstExempt")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AiteCardResponseModel))]
        public HttpResponseMessage AiteGstPstExempt([FromBody] AiteGstPstCardModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,AiteGstPstExempt,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            var accessToken = Request.GetFirstHeaderValueOrDefault<string>("authToken");
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = _taxManager.AiteGstPstExempt(model.SaleNumber, model.TillNumber, model.ShiftNumber, model.TreatyNumber,
                model.RegisterNumber, userCode, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            string issueStoreCreditMessage = string.Empty;

            if (result.Tenders != null && result.Tenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(result.Tenders);
            }
            var aiteCardResponse = new AiteCardResponseModel
            {
                AiteCardNumber = result.AiteCardNumber,
                AiteCardHolderName = result.AiteCardHolderName,
                BarCode = result.BarCode,
                IsFrmOverLimit = result.IsFrmOverLimit,
                SaleSummary = result.SaleSummary != null ? (from taxSumm in result.SaleSummary
                                                            select new NameValuePair
                                                            {
                                                                Key = taxSumm.Key,
                                                                Value = taxSumm.Value
                                                            }).ToList() : null,
                TenderSummary = result.Tenders != null ? TenderMapper.GetTenderSummaryModel(result.Tenders,
                issueStoreCreditMessage, result.Tenders) : null
            };
            _performancelog.Debug($"End,TenderController,AiteGstPstExempt,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, aiteCardResponse);
        }

        /// <summary>
        /// Validate QITE Band Member
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("qite/validate")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(QitecardResponseModel))]
        public HttpResponseMessage ValidateQiteBandMember([FromBody] QiteCardModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,ValidateTreatyNumber,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = _taxManager.ValidateQiteBandMember(model.SaleNumber, model.TillNumber, model.ShiftNumber, model.RegisterNumber, model.BandMember, userCode, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            string issueStoreCreditMessage = string.Empty;

            if (result.Tenders != null && result.Tenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(result.Tenders);
            }
            var cardResponse = new QitecardResponseModel
            {
                BandMember = result.QiteBandMember,
                BandMemberName = result.BandMemberName,
                SaleSummary = result.SaleSummary != null ? (from taxSumm in result.SaleSummary
                                                            select new NameValuePair
                                                            {
                                                                Key = taxSumm.Key,
                                                                Value = taxSumm.Value
                                                            }).ToList() : null,
                TenderSummary = result.Tenders != null ? TenderMapper.GetTenderSummaryModel(result.Tenders,
                issueStoreCreditMessage, result.Tenders) : null
            };

            _performancelog.Debug($"End,TenderController,ValidateTreatyNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, cardResponse);
        }


        /// <summary>
        /// Get Sale Summary
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("salesummary")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SaleSummaryResponseModel))]
        public HttpResponseMessage SaleSummary([FromBody]SaleSummaryInputModel model,string kickBackAmount = null)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,SaleSummary,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var sam1 = _saleManager.GetCurrentSale(model.SaleNumber, model.TillNumber, 1, userCode, out errorMessage);
            SaleSummaryInput obj = new SaleSummaryInput
            {
                SaleNumber = model.SaleNumber,
                TillNumber = model.TillNumber,
                RegisterNumber = model.RegisterNumber,
                IsAiteValidated = model.IsAiteValidated,
                IsSiteValidated = model.IsSiteValidated,
                UserCode = userCode
            };
            var result = _taxManager.GetSaleSummary(obj, out errorMessage);
           
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            string issueStoreCreditMessage = string.Empty;

            if (result.Tenders != null && result.Tenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(result.Tenders);
            }

            if (result.Tenders.Count > 0)
            {
                foreach (Tender tender in result.Tenders)
                {
                    tender.Image = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, "/images/" + tender.Image);
                }
            }

            List<Report> transactReports = null;
            if (!string.IsNullOrEmpty(kickBackAmount))
            {
                result.Tenders = _tenderManager.UpdateTenders(model.SaleNumber, model.TillNumber, "Sale", userCode, false, "KICKBACK", kickBackAmount, out transactReports, out errorMessage);
            }
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            var cardResponse = new SaleSummaryResponseModel
            {
                SaleSummary = result.SaleSummary != null ? (from taxSumm in result.SaleSummary
                                                            select new NameValuePair
                                                            {
                                                                Key = taxSumm.Key,
                                                                Value = taxSumm.Value
                                                            }).ToList() : null,
                TenderSummary = result.Tenders != null ? TenderMapper.GetTenderSummaryModel(result.Tenders,
                issueStoreCreditMessage, result.Tenders) : null
            };

            var sam = _saleManager.GetCurrentSale(model.SaleNumber,model.TillNumber,1,userCode,out errorMessage);
            WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside sale summary 1194 cardno value" + sam.Customer.PointCardNum);
            _performancelog.Debug($"End,TenderController,SaleSummary,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, cardResponse);
        }

        /// <summary>
        /// Get OverLimit Detail
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        [Route("overLimitDetails")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(OverLimitResponseModel))]
        public HttpResponseMessage GetOverLimitDetail(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,GetOverLimitDetail,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            ErrorMessage errorMessage;

            var result = _overLimitManager.GetOverLimitDetails(tillNumber, saleNumber, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            var model = new OverLimitResponseModel
            {
                IsGasReasons = result.IsGasReasons,
                IsTobaccoReasons = result.IsTobaccoReasons,
                IsPropaneReasons = result.IsPropaneReasons,
                GasReasons = result.GasReasons != null ? (from rsn in result.GasReasons
                                                          select new TaxExemptReasonModel
                                                          {
                                                              Reason = rsn.Reason,
                                                              ExplanationCode = rsn.ExplanationCode
                                                          }).ToList() : null,
                PropaneReasons = result.PropaneReasons != null ? (from rsn in result.PropaneReasons
                                                                  select new TaxExemptReasonModel
                                                                  {
                                                                      Reason = rsn.Reason,
                                                                      ExplanationCode = rsn.ExplanationCode
                                                                  }).ToList() : null,
                TobaccoReasons = result.TobaccoReasons != null ? (from rsn in result.TobaccoReasons
                                                                  select new TaxExemptReasonModel
                                                                  {
                                                                      Reason = rsn.Reason,
                                                                      ExplanationCode = rsn.ExplanationCode
                                                                  }).ToList() : null,
                TaxExemptSale = result.TaxExemptSale != null ? (from teSale in result.TaxExemptSale
                                                                select new TaxExemptSaleModel
                                                                {
                                                                    Type = teSale.Type,
                                                                    Product = teSale.Product,
                                                                    Quantity = teSale.Quantity,
                                                                    RegularPrice = teSale.RegularPrice,
                                                                    TaxFreePrice = teSale.TaxFreePrice,
                                                                    ExemptedTax = teSale.ExemptedTax,
                                                                    QuotaUsed = teSale.QuotaUsed,
                                                                    QuotaLimit = teSale.QuotaLimit
                                                                }).ToList() : null
            };

            _performancelog.Debug($"End,TenderController,GetOverLimitDetail,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }


        /// <summary>
        /// Get OverLimit Detail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("overLimit/complete")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SaleSummaryResponseModel))]
        public HttpResponseMessage DoneOverLimit([FromBody] OverLimitDoneModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,DoneOverLimit,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = _overLimitManager.DoneOverLimit(model.TillNumber, model.SaleNumber, userCode, model.Reason
                , model.Explanation, model.Location, model.Date, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            string issueStoreCreditMessage = string.Empty;

            if (result.Tenders != null && result.Tenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(result.Tenders);
            }
            var saleSummary = new SaleSummaryResponseModel
            {
                SaleSummary = result.SaleSummary != null ? (from taxSumm in result.SaleSummary
                                                            select new NameValuePair
                                                            {
                                                                Key = taxSumm.Key,
                                                                Value = taxSumm.Value
                                                            }).ToList() : null,
                TenderSummary = result.Tenders != null ? TenderMapper.GetTenderSummaryModel(result.Tenders,
                issueStoreCreditMessage, result.Tenders) : null
            };

            _performancelog.Debug($"End,TenderController,DoneOverLimit,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, saleSummary);
        }

        /// <summary>
        /// Get OverrideLimit Detail
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <param name="treatyNumber"></param>
        /// <param name="treatyName"></param>
        /// <returns></returns>
        [Route("overrideLimitDetails")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(OverrideLimitResponseModel))]
        public HttpResponseMessage GetOverrideLimitDetail(int saleNumber, int tillNumber , string treatyNumber , string treatyName)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,GetOverrideLimitDetail,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = _overrideLimitManager.LoadOverrideLimitDetails(saleNumber, tillNumber, userCode,  treatyNumber,  treatyName, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            var model = new OverrideLimitResponseModel
            {
                Caption = result.Caption,
                IsDocumentNoEnabled = result.IsDocumentNoEnabled,
                IsOverrideCodeEnabled = result.IsOverrideCodeEnabled,
                IsRtvpValidationEnabled = result.IsRtvpValidationEnabled,
                OverrideCodes = result.OverrideCodes != null ? (from codes in result.OverrideCodes
                                                                select new ComboOverrideCodesModel
                                                                {
                                                                    RowId = codes.RowId,
                                                                    Codes = codes.Codes
                                                                }).ToList() : null,
                PurchaseItems = result.OverrideCodes != null ? (from purchaseItem in result.PurchaseItems
                                                                select new PurchaseItemResponseModel
                                                                {
                                                                    ProductTypeId = (int)purchaseItem.ProductTypeId,
                                                                    Amount = purchaseItem.Amount,
                                                                    Price = purchaseItem.Price,
                                                                    EquivalentQuantity = purchaseItem.EquivalentQuantity,
                                                                    QuotaLimit = purchaseItem.QuotaLimit,
                                                                    Quantity = purchaseItem.Quantity,
                                                                    QuotaUsed = purchaseItem.QuotaUsed,
                                                                    ProductId = purchaseItem.ProductId,
                                                                    DisplayQuota = purchaseItem.DisplayQuota,
                                                                    FuelOverLimitText = purchaseItem.FuelOverLimitText
                                                                }).ToList() : null
            };

            _performancelog.Debug($"End,TenderController,GetOverrideLimitDetail,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }


        /// <summary>
        /// Get OverrideLimit Detail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("overrideLimit/override")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SuccessReponse))]
        public HttpResponseMessage DoneOverRideLimit([FromBody] OverrideLimitDoneModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,GetOverrideLimitDetail,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = _overrideLimitManager.DoneOverRideLimit(model.SaleNumber, model.TillNumber, userCode, model.ItemNumber, model.DocumentNumber, model.OverrideCode, model.DocumentDetail, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            var res = new SuccessReponse
            {
                Success = result
            };

            _performancelog.Debug($"End,TenderController,GetOverrideLimitDetail,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// Get OverrideLimit Detail
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <param name="registerNumber"></param>
        /// <returns></returns>
        [Route("overrideLimit/complete")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SaleSummaryResponseModel))]
        public HttpResponseMessage CompleteOverrideLimit(int saleNumber, int tillNumber, byte registerNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,CompleteOverrideLimit,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = _overrideLimitManager.CompleteOverrideLimit(tillNumber, saleNumber, registerNumber, userCode, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            string issueStoreCreditMessage = string.Empty;

            if (result.Tenders != null && result.Tenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(result.Tenders);
            }
            var saleSummary = new SaleSummaryResponseModel
            {
                SaleSummary = result.SaleSummary != null ? (from taxSumm in result.SaleSummary
                                                            select new NameValuePair
                                                            {
                                                                Key = taxSumm.Key,
                                                                Value = taxSumm.Value
                                                            }).ToList() : null,
                TenderSummary = result.Tenders != null ? TenderMapper.GetTenderSummaryModel(result.Tenders,
                issueStoreCreditMessage, result.Tenders) : null
            };

            _performancelog.Debug($"End,TenderController,CompleteOverrideLimit,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, saleSummary);
        }


        /// <summary>
        /// Get Sale Summary
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("arPayment")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SaleSummaryResponseModel))]
        public HttpResponseMessage SaleSummaryForArPayment([FromBody] ArPaymentInputModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,SaleSummaryForARPayment,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

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
            var arPayment = new ArPaymentRequest
            {
                Amount = model.Amount,
                CustomerCode = model.CustomerCode,
                IsReturnMode = model.IsReturnMode,
                RegisterNumber = model.RegisterNumber,
                SaleNumber = model.SaleNumber,
                TillNumber = model.TillNumber
            };
            var result = _tenderManager.SaleSummaryForArPayment(arPayment, userCode, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle?.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            string issueStoreCreditMessage = string.Empty;

            if (result.Tenders != null && result.Tenders.EnableCompletePayment)
            {
                issueStoreCreditMessage = _tenderManager.IssueStoreCredit(result.Tenders);
            }

            if (result.Tenders.Count > 0)
            {
                foreach (Tender tender in result.Tenders)
                {
                    tender.Image = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, "/images/" + tender.Image);
                }
            }

            var cardResponse = new SaleSummaryResponseModel
            {
                SaleSummary = result.SaleSummary != null ? (from taxSumm in result.SaleSummary
                                                            select new NameValuePair
                                                            {
                                                                Key = taxSumm.Key,
                                                                Value = taxSumm.Value
                                                            }).ToList() : null,
                TenderSummary = result.Tenders != null ? TenderMapper.GetTenderSummaryModel(result.Tenders,
                issueStoreCreditMessage, result.Tenders) : null
            };

            _performancelog.Debug($"End,TenderController,SaleSummaryForARPayment,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, cardResponse);
        }


        /// <summary>
        /// Get treaty Name for treaty Number
        /// </summary>
        /// <param name="treatyNumber"></param>
        /// <param name="captureMethod"></param>
        /// <returns></returns>
        [Route("getTreatyName")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        public HttpResponseMessage GetTreatyName(string treatyNumber, short captureMethod)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,GetTreatyName,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = _taxManager.GetTreatyName(treatyNumber, captureMethod, userCode, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }

            _performancelog.Debug($"End,TenderController,GetTreatyName,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Update tender values
        /// </summary>
        ///<param name="profilePrompts">Profile prompts</param>
        /// <returns></returns>
        [Route("saveProfilePrompts")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage SaveProfilePrompt([FromBody] ProfilePromptModel profilePrompts)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderController,CancelTenders,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error = new ErrorMessage();
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (profilePrompts == null)
            {
                error.MessageStyle.Message = Constants.InvalidRequest;
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }
            var cardPrompts = new List<CardPrompt>();
            if (profilePrompts.Prompts == null || profilePrompts.Prompts.Count == 0)
                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            foreach (var prompts in profilePrompts.Prompts)
            {
                cardPrompts.Add(new CardPrompt
                {
                    PromptAnswer = prompts.PromptAnswer,
                    PromptMessage = prompts.PromptMessage
                });
            }
            var poNumber = _tenderManager.SaveProfilePrompt(profilePrompts.SaleNumber, profilePrompts.TillNumber,
                 profilePrompts.CardNumber, profilePrompts.ProfileId, cardPrompts, userCode, out error);
            _performancelog.Debug($"End,TenderController,CancelTenders,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, poNumber);
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
        public void WriteUDPData(string msgStr)
        {
            try
            {
                var logPath = @"C:\APILog\";
                var fileName = logPath + "PosLog_" + DateTime.Today.ToString("MM/dd/yyyy") + ".txt";

                using (StreamWriter fileWriter = new StreamWriter(fileName, true))
                {
                    fileWriter.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + Microsoft.VisualBasic.Strings.Space(3) + msgStr);
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}

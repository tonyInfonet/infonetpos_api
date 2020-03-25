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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infonet.CStoreCommander.WebApi.Models.Cash;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Report;
using Infonet.CStoreCommander.WebApi.Models.Tender;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Cash controller
    /// </summary>
    [RoutePrefix("api/v1/cash")]
    [ApiAuthorization]
    public class CashController : ApiController
    {
        private readonly ICashManager _cashManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cashManager"></param>
        public CashController(ICashManager cashManager)
        {
            _cashManager = cashManager;
        }

        /// <summary>
        /// Method to get cash draw types
        /// </summary>
        /// <returns>Csh draw button</returns>
        [Route("getDrawTypes")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Forbidden, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CashDrawButtonsResponseModel))]
        public HttpResponseMessage GetCashDrawButtons()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashController,GetCashDrawButtons,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            ErrorMessage error;

            var cashDrawButtons = _cashManager.GetCashDrawButtons(userCode, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                _performancelog.Debug(
                    $"End,CashController,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            CashDrawButtonsResponseModel response = GetCashDrawModel(cashDrawButtons);
            _performancelog.Debug(
                $"End,CashController,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// Method to complete cash draw
        /// </summary>
        /// <param name="cashDraw">Cash draw</param>
        /// <returns>Receipt</returns>
        [Route("completeDraw")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Forbidden, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ReportModel))]
        public HttpResponseMessage CompleteCashDraw([FromBody] CashDrawButton cashDraw)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashController,CompleteCashDraw,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (cashDraw == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = Constants.InvalidRequest, MessageType = 0 }
                    });
            }
            ErrorMessage error;
            int copies;
            var fs = _cashManager.CompleteCashDraw(cashDraw, userCode, out copies, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                _performancelog.Debug(
                    $"End,CashController,CompleteCashDraw,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            try
            {
                var content = Helper.CreateBytes(fs);
                var cashDrawReceipt = new ReportModel
                {
                    ReportName = Constants.CashDrawFile,
                    ReportContent = content,
                    Copies = copies
                };
                _performancelog.Debug(
                    $"End,CashController,CompleteCashDraw,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.OK, cashDrawReceipt);
            }
            catch
            {
                _performancelog.Debug(
                    $"End,CashController,CompleteCashDraw,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                });
            }
        }

        /// <summary>
        /// Method to get cash buttons
        /// </summary>
        /// <returns>Cash buttons</returns>
        [Route("getCashButtons")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<CashButton>))]
        public HttpResponseMessage GetCashButtons()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashController,GetCashButtons,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var cashDrawButtons = _cashManager.GetCashButtons();

            _performancelog.Debug(
                $"End,CashController,GetCashButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, cashDrawButtons);
        }

        /// <summary>
        /// Method to update cash drop tenders
        /// </summary>
        /// <param name="cashTender">Cash tenders</param>
        /// <returns>Tenders</returns>
        [Route("updateTenders")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UpdateCashDropResponseModel))]
        public HttpResponseMessage UpdateCashDropTenders([FromBody] CashTenderModel cashTender)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug(
                $"Start,CashController,UpdateCashDropTenders,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            if (cashTender == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = Resource.InvalidRequest, MessageType = 0 }
                    });
            }

            ErrorMessage error;
            var tenders = cashTender.Tenders.Select(tender => new Tender
            {
                Tender_Code = tender.TenderCode,
                Amount_Entered = tender.AmountEntered ?? 0
            }).ToList();

            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;

            var updatedTenders = _cashManager.UpdateCashDropTendered(tenders, cashTender.DropReason,
                cashTender.SaleNumber, cashTender.TillNumber, userCode, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                _performancelog.Debug(
                    $"End,CashController,UpdateCashDropTenders,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            UpdateCashDropResponseModel response = GetUpdatedTenders(updatedTenders);

            _performancelog.Debug(
                $"End,CashController,UpdateCashDropTenders,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// Method to complete cash drop
        /// </summary>
        /// <param name="cashDrop">Cash drop</param>
        /// <returns>Receipt</returns>
        [Route("completeDrop")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Forbidden, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ReportModel))]
        public HttpResponseMessage CompleteCashDrop([FromBody] CashDropModel cashDrop)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashController,CompleteCashDrop,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (cashDrop == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = "Request is Invalid", MessageType = 0 }
                    });
            }
            ErrorMessage error;
            var tenders = cashDrop.Tenders.Select(tender => new Tender
            {
                Tender_Code = tender.TenderCode,
                Amount_Entered = tender.AmountEntered
            }).ToList();
            int copies;
            var fs = _cashManager.CompleteCashDrop(tenders, cashDrop.TillNumber, userCode,
                cashDrop.RegisterNumber, cashDrop.DropReason, cashDrop.EnvelopeNumber, out copies, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                _performancelog.Debug(
                    $"End,CashController,CompleteCashDrop,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            try
            {
                var content = Helper.CreateBytes(fs);

                var cashDrawReceipt = new ReportModel
                {
                    ReportName = Constants.CashDropFile,
                    ReportContent = content,
                    Copies = copies
                };

                _performancelog.Debug(
                    $"End,CashController,CompleteCashDrop,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return Request.CreateResponse(HttpStatusCode.OK, cashDrawReceipt);
            }
            catch
            {
                _performancelog.Debug(
                    $"End,CashController,CompleteCashDrop,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                });
            }
        }

        /// <summary>
        /// Method to save cash draw open reason
        /// </summary>
        /// <param name="cashDrop">Cash drop</param>
        /// <returns>Success Response</returns>
        [Route("openCashDrawer")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SuccessReponse))]
        public HttpResponseMessage OpenCashDrawer([FromBody] CashDropModel cashDrop)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashController,OpenCashDrawer,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (cashDrop == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = Constants.InvalidRequest, MessageType = 0 }
                    });
            }
            ErrorMessage error;
            _cashManager.OpenCashDrawer(userCode, cashDrop.DropReason, cashDrop.TillNumber, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(error.StatusCode,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            var success = new SuccessReponse
            {
                Success = true
            };
            _performancelog.Debug(
                $"End,CashController,OpenCashDrawer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, success);
        }

        #region Private methods

        /// <summary>
        /// Method to create cash draw model
        /// </summary>
        /// <param name="cashDrawButtons">Cash draw buttons</param>
        /// <returns>Cash draw button response</returns>
        private CashDrawButtonsResponseModel GetCashDrawModel(CashDrawButton cashDrawButtons)
        {
            var response = new CashDrawButtonsResponseModel();
            foreach (var coin in cashDrawButtons.Coins)
            {
                response.Coins.Add(new CashModel
                {
                    CurrencyName = coin.CurrencyName,
                    Value = coin.Value,
                    Image = coin.Image
                });
            }

            foreach (var bill in cashDrawButtons.Bills)
            {
                response.Bills.Add(new CashModel
                {
                    CurrencyName = bill.CurrencyName,
                    Value = bill.Value,
                    Image = bill.Image
                });
            }

            return response;
        }

        /// <summary>
        /// Method to create update cash drop model
        /// </summary>
        /// <param name="updatedTenders">Updated tenders</param>
        /// <returns>Update cash drop model</returns>
        private UpdateCashDropResponseModel GetUpdatedTenders(Tenders updatedTenders)
        {
            var newTenders = updatedTenders.Where(u => u.Amount_Entered != 0);

            var response = new UpdateCashDropResponseModel
            {
                Tenders = newTenders.Select(tender => new TenderResponseModel
                {
                    TenderCode = tender.Tender_Code,
                    TenderName = tender.Tender_Name,
                    AmountEntered =
                        tender.Amount_Entered == 0 ? "" : tender.Amount_Entered.ToString(Constants.CurrencyFormat),
                    AmountValue = tender.Amount_Used == 0 ? "" : tender.Amount_Used.ToString(Constants.CurrencyFormat),
                    MaximumValue = tender.MaxAmount,
                    MinimumValue = tender.MinAmount,
                    Image = tender.Image
                }).ToList(),
                TenderedAmount = updatedTenders.Tend_Totals.Tend_Used.ToString(Constants.CurrencyFormat)
            };
            return response;
        }


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
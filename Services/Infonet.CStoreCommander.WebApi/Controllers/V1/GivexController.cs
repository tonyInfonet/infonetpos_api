using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.VisualBasic;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Resources;
using Infonet.CStoreCommander.WebApi.Utilities;
using log4net;
using Infonet.CStoreCommander.Logging;
using System;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.WebApi.Mapper;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Givex;
using Swashbuckle.Swagger.Annotations;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Ctor
    /// </summary>
    [RoutePrefix("api/v1/givex")]
    [ApiAuthorization]
    public class GivexController : ApiController
    {
        private readonly ISaleManager _saleManager;
        private readonly IGivexManager _givexManager;
        private readonly IPolicyManager _policyManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="givexManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="policyManager"></param>
        /// <param name="saleManager"></param>
        public GivexController(IGivexManager givexManager, IApiResourceManager resourceManager,
            IPolicyManager policyManager, ISaleManager saleManager)
        {
            _givexManager = givexManager;
            _resourceManager = resourceManager;
            _policyManager = policyManager;
            _saleManager = saleManager;
        }

        /// <summary>
        /// Get Givex Stock Code
        /// </summary>
        /// <returns>Stock code</returns>
        [Route("getStockCode")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(GivexStockCode))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetStockCode()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,GivexController,GetStockCode,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;

            var strGiveStock = _givexManager.GetValidGiveXStock(out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }
            var givexStockCode = new GivexStockCode { StockCode = strGiveStock };
            _performancelog.Debug($"End,GivexController,GetStockCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, givexStockCode);
        }

        /// <summary>
        /// Gets Givex Card balance
        /// </summary>
        /// <param name="givexCardNumber">Givex card number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Givex card</returns>
        [Route("balance")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(GivexCardBalanceModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GivexCardBalance(string givexCardNumber, int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,GivexController,GivexCardBalance,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            HttpResponseMessage responseMessage;

            if (!ValidateGivexCard(ref givexCardNumber, out responseMessage))
            {
                return responseMessage;
            }

            string userCode;
            HttpResponseMessage httpResponseMessage;
            Report giveReceipt;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var balance = _givexManager.GetCardBalance(givexCardNumber, saleNumber, tillNumber,
                                        userCode, out giveReceipt, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }
            _performancelog.Debug($"End,GivexController,GivexCardBalance,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            var balanceModel = new GivexCardBalanceModel
            {
                CardNumber = givexCardNumber,
                Balance = balance,
                Receipt = giveReceipt
            };
            return Request.CreateResponse(HttpStatusCode.OK, balanceModel);
        }

        /// <summary>
        /// Adjust Givex Card 
        /// </summary>
        /// <param name="givexModel">Givex input model</param>
        /// <returns>Sale</returns>
        [Route("adjust")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(GivexResponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotAcceptable, Type = typeof(ErrorResponse))]
        public HttpResponseMessage AdjustGivexCard([FromBody]AdjustGivexModel givexModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,GivexController,AdjustGivexCard,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            HttpResponseMessage responseMessage;

            var cardNumber = givexModel.GivexCardNumber;
            if (!ValidateGivexCard(ref cardNumber, out responseMessage))
            {
                return responseMessage;
            }
            givexModel.GivexCardNumber = cardNumber;

            if (!ValidateStockCode(givexModel.StockCode, out responseMessage))
            {
                return responseMessage;
            }

            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (Conversion.Val(givexModel.Amount) <= 0)
            {

                const MessageType messageType = (int)MessageType.OkOnly + MessageType.Critical;
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                return Request.CreateResponse(HttpStatusCode.NotAcceptable,
                        new ErrorResponse
                        {
                            Error = _resourceManager.CreateMessage(offSet, 32, 94, null, messageType),
                        });
            }
            Report giveReceipt;
            var sale = _givexManager.AdjustGivexCard(givexModel.GivexCardNumber,
                                        givexModel.Amount,
                                        userCode,
                                        givexModel.TillNumber,
                                        givexModel.SaleNumber,
                                        givexModel.StockCode,
                                        out giveReceipt,
                                        out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }
            var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
            var enableButtons = _saleManager.EnableCashButton(sale, userCode);
            var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
            var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
            var response = new GivexResponseModel
            {
                Sale = saleModel,
                Receipt = giveReceipt
            };
            _performancelog.Debug($"End,GivexController,AdjustGivexCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// Activate Givex Card
        /// </summary>
        /// <param name="givexModel">Givex  model</param>
        /// <returns>Sale</returns>
        [Route("activate")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(GivexResponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotAcceptable, Type = typeof(ErrorResponse))]
        public HttpResponseMessage ActivateGivexCard([FromBody]ActivateGivexModel givexModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,GivexController,ActivateGivexCard,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            HttpResponseMessage responseMessage;

            var cardNumber = givexModel.GivexCardNumber;
            if (!ValidateGivexCard(ref cardNumber, out responseMessage))
            {
                return responseMessage;
            }
            givexModel.GivexCardNumber = cardNumber;


            if (!ValidateStockCode(givexModel.StockCode, out responseMessage))
            {
                return responseMessage;
            }
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (Conversion.Val(givexModel.GivexPrice) <= 0)
            {

                var messageType = (int)MessageType.OkOnly + MessageType.Critical;
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                return Request.CreateResponse(HttpStatusCode.NotAcceptable,
                        new ErrorResponse
                        {
                            Error = _resourceManager.CreateMessage(offSet, 32, 92, null, messageType),
                        });
            }
            Report giveReceipt;
            var sale = _givexManager.ActivateGivexCard(givexModel.GivexCardNumber,
                                        givexModel.GivexPrice,
                                        userCode,
                                        givexModel.TillNumber,
                                        givexModel.SaleNumber,
                                        givexModel.StockCode,
                                        out giveReceipt,
                                        out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle?.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }

            var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
            var enableButtons = _saleManager.EnableCashButton(sale, userCode);
            var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
            var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
            var response = new GivexResponseModel
            {
                Sale = saleModel,
                Receipt = giveReceipt
            };
            _performancelog.Debug($"End,GivexController,ActivateGivexCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// DeActivate Givex Card
        /// </summary>
        /// <param name="givexModel">Givex model</param>
        /// <returns>Sale</returns>
        [Route("deactivate")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(GivexResponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage DeactivateGivexCard([FromBody]ActivateGivexModel givexModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,GivexController,DeactivateGivexCard,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            HttpResponseMessage responseMessage;

            var cardNumber = givexModel.GivexCardNumber;
            if (!ValidateGivexCard(ref cardNumber, out responseMessage))
            {
                return responseMessage;
            }
            givexModel.GivexCardNumber = cardNumber;

            if (!ValidateStockCode(givexModel.StockCode, out responseMessage))
            {
                return responseMessage;
            }

            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            Report giveReceipt;
            var sale = _givexManager.DeactivateGivexCard(givexModel.GivexCardNumber,
                                        givexModel.GivexPrice,
                                        userCode,
                                        givexModel.TillNumber,
                                        givexModel.SaleNumber,
                                        givexModel.StockCode,
                                        out giveReceipt,
                                        out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }

            var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
            var enableButtons = _saleManager.EnableCashButton(sale, userCode);
            var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
            var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
            var response = new GivexResponseModel
            {
                Sale = saleModel,
                Receipt = giveReceipt
            };
            _performancelog.Debug($"End,GivexController,DeactivateGivexCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// Increase Givex card amount
        /// </summary>
        /// <param name="givexModel">Givex modle</param>
        /// <returns>Sale</returns>
        [Route("increase")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(GivexResponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage IncreaseGivexCard([FromBody]ActivateGivexModel givexModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,GivexController,IncreaseGivexCard,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            HttpResponseMessage responseMessage;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var cardNumber = givexModel.GivexCardNumber;
            if (!ValidateGivexCard(ref cardNumber, out responseMessage))
            {
                return responseMessage;
            }
            givexModel.GivexCardNumber = cardNumber;

            if (!ValidateStockCode(givexModel.StockCode, out responseMessage))
            {
                return responseMessage;
            }
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (Conversion.Val(givexModel.GivexPrice) <= 0)
            {

                var messageType = (int)MessageType.OkOnly + MessageType.Critical;

                return Request.CreateResponse(HttpStatusCode.NotAcceptable,
                        new ErrorResponse
                        {
                            Error = _resourceManager.CreateMessage(offSet, 32, 95, null, messageType),
                        });
            }
            Report giveReceipt;
            var sale = _givexManager.IncreaseGivexCard(givexModel.GivexCardNumber,
                                        givexModel.GivexPrice,
                                        userCode,
                                        givexModel.TillNumber,
                                        givexModel.SaleNumber,
                                        givexModel.StockCode,
                                        out giveReceipt,
                                        out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }

            var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
            var enableButtons = _saleManager.EnableCashButton(sale, userCode);
            var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
            var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
            var response = new GivexResponseModel
            {
                Sale = saleModel,
                Receipt = giveReceipt
            };
            _performancelog.Debug($"End,GivexController,IncreaseGivexCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// Close batch Givex card
        /// </summary>
        /// <param name="givexModel">Givex model</param>
        /// <returns>Receipt</returns>
        [Route("closeBatch")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Report))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage CloseBatch([FromBody]GivexCloseBatchModel givexModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,GivexController,CloseBatch,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            Report giveReceipt;
            _givexManager.CloseBatch(givexModel.SaleNumber, givexModel.TillNumber,
                userCode, out giveReceipt, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }
            _performancelog.Debug($"End,GivexController,CloseBatch,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, giveReceipt);
        }


        /// <summary>
        /// Close batch Givex report
        /// </summary>
        /// <param name="reportDate">Report Date</param>
        /// <returns>Receipt</returns>
        [Route("report")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(GivexReport))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GivexReport(string reportDate)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,GivexController,GivexReport,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            DateTime reptDate;

            if (string.IsNullOrEmpty(reportDate))
            {
                reptDate = DateTime.Now;
            }
            else
            {
                try
                {
                    reptDate = Convert.ToDateTime(reportDate);
                }
                catch
                {
                    reptDate = DateTime.Now;
                }
            }
            var givexReport = _givexManager.GetGivexReport(reptDate);


            _performancelog.Debug($"End,GivexController,GivexReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, givexReport);
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
        /// Validate the givexcard number
        /// </summary>
        /// <param name="givexCardNumber"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        private bool ValidateGivexCard(ref string givexCardNumber, out HttpResponseMessage responseMessage)
        {
            var returnValue = true;
            responseMessage = new HttpResponseMessage();

            //const MessageType messageType = (int)MessageType.OkOnly + MessageType.Information;
            var offSet = _policyManager.LoadStoreInfo().OffSet;

            var start = givexCardNumber.IndexOf(';');
            var end = givexCardNumber.IndexOf('?');

            if (start > -1 && end > -1)
            {
                givexCardNumber = givexCardNumber.Substring(start + 1, end - start - 1);
            }

            if (!string.IsNullOrEmpty(givexCardNumber))
            {
                if (!Information.IsNumeric(givexCardNumber.Trim()) || Conversion.Val(givexCardNumber.Trim()) <= 0)
                {
                    if (_policyManager.ScanGiftCard)
                    {

                        responseMessage = Request.CreateResponse(HttpStatusCode.NotAcceptable,
                            new ErrorResponse
                            {
                                Error = _resourceManager.CreateMessage(offSet, 32, 90, null),
                            });
                    }
                    else
                    {

                        responseMessage = Request.CreateResponse(HttpStatusCode.NotAcceptable,
                            new ErrorResponse
                            {
                                Error = _resourceManager.CreateMessage(offSet, 32, 91, null),
                            });
                    }
                    returnValue = false;
                }
            }
            else
            {
                if (_policyManager.ScanGiftCard)
                {

                    responseMessage = Request.CreateResponse(HttpStatusCode.NotAcceptable,
                        new ErrorResponse
                        {
                            Error = _resourceManager.CreateMessage(offSet, 32, 90, null),
                        });
                }
                else
                {

                    responseMessage = Request.CreateResponse(HttpStatusCode.NotAcceptable,
                        new ErrorResponse
                        {
                            Error = _resourceManager.CreateMessage(offSet, 32, 91, null),
                        });
                }
                returnValue = false;
            }
            return returnValue;
        }

        /// <summary>
        /// Validate Stock Code
        /// </summary>
        /// <param name="stockCode"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        private bool ValidateStockCode(string stockCode, out HttpResponseMessage responseMessage)
        {
            var returnValue = true;
            responseMessage = new HttpResponseMessage();
            ErrorMessage error;
            var strGiveStock = _givexManager.GetValidGiveXStock(out error);
            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.NotAcceptable, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
                returnValue = false;
            }
            if (string.IsNullOrEmpty(stockCode) || !stockCode.ToUpper().Equals(strGiveStock.ToUpper()))
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.NotAcceptable, new ErrorResponse
                {
                    Error = new MessageStyle { Message = Resource.InvalidStockCode, MessageType = 0 }
                });
                returnValue = false;
            }
            return returnValue;
        }


        #endregion

    }//end class
}//end namespace

using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Mapper;
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
using Infonet.CStoreCommander.WebApi.Models.Payment;
using Infonet.CStoreCommander.WebApi.Models.Report;
using Infonet.CStoreCommander.WebApi.Models.Sale;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.WebApi.Models.Common;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for sale
    /// </summary>
    [RoutePrefix("api/v1/sale")]
    [ApiAuthorization]
    public class SaleController : ApiController
    {
        private readonly ISaleManager _saleManager;
        private readonly ISuspendedSaleManger _suspendedSaleManger;
        private readonly ISaleLineManager _saleLineManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="saleManager"></param>
        /// <param name="suspendedSaleManger"></param>
        /// <param name="saleLineManager"></param>
        public SaleController(ISaleManager saleManager,
            ISuspendedSaleManger suspendedSaleManger,
            ISaleLineManager saleLineManager)
        {
            _saleManager = saleManager;
            _suspendedSaleManger = suspendedSaleManger;
            _saleLineManager = saleLineManager;
        }


        /// <summary>
        /// Method to initialise a sale
        /// </summary>
        /// <param name="tillNumber">TillNumber</param>
        /// <param name="registerNumber"></param>
        /// <returns>Sale</returns>
        [Route("new")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ErrorResponse))]
        public HttpResponseMessage InitializeSale(int tillNumber, int registerNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleController,InitializeSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage message;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var sale = _saleManager.InitializeSale(tillNumber, registerNumber, userCode, out message);

            if (message?.MessageStyle?.Message != null)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, message);
            }
            if (sale != null)
            {
                var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                var enableButton = _saleManager.EnableCashButton(sale, userCode);
                var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
                var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButton, userCanWriteOff);
                _performancelog.Debug($"End,SaleController,InitializeSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.OK, saleModel);
            }


            return null;
        }

        /// <summary>
        /// Method to check various conditions before adding a stock
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="stockCode">Stock code</param>
        /// <param name="quantity"></param>
        /// <param name="isReturnMode">Return mode or not</param>
        /// <param name="saleNumber"></param>
        /// <returns>Stock constraints</returns>
        [Route("items/verifyStock")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(StockMessage))]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ErrorResponse))]
        public HttpResponseMessage VerifyStockConstraints(int saleNumber, int tillNumber, string stockCode,
            float quantity, bool isReturnMode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleController,VerifyStockConstraints,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var stockMessage = _saleLineManager.CheckStockConditions(saleNumber, tillNumber, stockCode, userCode,
                isReturnMode, quantity, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(error.StatusCode,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            _performancelog.Debug($"End,SaleController,VerifyStockConstraints,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, stockMessage);
        }

        /// <summary>
        /// Verify and Add Sale Line Item
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Sale</returns>
        [Route("items/verifyAdd")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.PartialContent, Type = typeof(StockMessage))]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ErrorResponse))]
        public HttpResponseMessage VerifyAndAddSaleLineItem([FromBody] AddSaleLineModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleController,VerifyAndAddSaleLineItem,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
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

            var stockMessage = _saleLineManager.CheckStockConditions(model.SaleNumber, model.TillNumber, model.StockCode, userCode,
               model.IsReturnMode, (float)model.Quantity, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(error.StatusCode,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }

            //if (stockMessage.AddStockPage.OpenAddStockPage || stockMessage.GiftCertPage.OpenGiftCertPage || stockMessage.GivexPage.OpenGivexPage || stockMessage.RestrictionPage.OpenRestrictionPage || !string.IsNullOrEmpty(stockMessage.QuantityMessage.Message) || !string.IsNullOrEmpty(stockMessage.RegularPriceMessage.Message))
            if (stockMessage.AddStockPage.OpenAddStockPage
                || stockMessage.GiftCertPage.OpenGiftCertPage
                || stockMessage.GivexPage.OpenGivexPage
                || stockMessage.RestrictionPage.OpenRestrictionPage
                || stockMessage.PSInetPage.OpenPSInetPage
                || !string.IsNullOrEmpty(stockMessage.QuantityMessage.Message)
                || !string.IsNullOrEmpty(stockMessage.RegularPriceMessage.Message))
            {
                return Request.CreateResponse(HttpStatusCode.PartialContent, stockMessage);
            }

            if (!stockMessage.CanManuallyEnterProduct && model.IsManuallyAdded)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = new MessageStyle
                        {
                            Message = stockMessage.ManuallyEnterMessage,
                            MessageType = 0
                        }
                    });
            }

            var sale = _saleManager.VerifyAddSaleLineItem(userCode, model.TillNumber,
                model.SaleNumber, model.RegisterNumber, model.StockCode,
                model.Quantity, model.IsReturnMode, model.GiftCard, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message) && error.StatusCode != HttpStatusCode.OK)
            {
                return Request.CreateResponse(error.StatusCode,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            if (sale != null)
            {
                object saleModel;
                var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                var enableButtons = _saleManager.EnableCashButton(sale, userCode);
                var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);

                if (!string.IsNullOrEmpty(error.MessageStyle.Message))
                {
                    var message = new
                    {
                        error = error.MessageStyle,
                    };
                    var messages = new List<object> { message };

                    saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff, messages);
                }
                else
                {
                    saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
                }
                _performancelog.Debug($"End,SaleController,VerifyAndAddSaleLineItem,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.OK, saleModel);
            }
            return null;
        }

        /// <summary>
        /// Add Sale Line Item
        /// </summary>
        /// <param name="saleLineModel"></param>
        /// <returns>Sale</returns>
        [Route("items/add")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ErrorResponse))]
        public HttpResponseMessage AddSaleLineItem([FromBody] AddSaleLineModel saleLineModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleController,AddSaleLineItem,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (saleLineModel == null)
            {
                return Request.CreateResponse(
                  HttpStatusCode.BadRequest,
                  new ErrorResponse
                  {
                      Error = new MessageStyle { Message = Constants.InvalidRequest }
                  });
            }

            var sale = _saleManager.AddSaleLineItem(userCode, saleLineModel.TillNumber,
                saleLineModel.SaleNumber, saleLineModel.RegisterNumber, saleLineModel.StockCode,
                saleLineModel.Quantity, saleLineModel.IsReturnMode, saleLineModel.GiftCard, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message) && error.StatusCode != HttpStatusCode.OK)
            {
                return Request.CreateResponse(error.StatusCode,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            if (sale != null)
            {
                object saleModel;
                var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                var enableButtons = _saleManager.EnableCashButton(sale, userCode);
                var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);

                if (!string.IsNullOrEmpty(error.MessageStyle.Message))
                {
                    var message = new
                    {
                        error = error.MessageStyle,
                    };
                    var messages = new List<object> { message };

                    saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff, messages);
                }
                else
                {
                    saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
                }
                _performancelog.Debug($"End,SaleController,AddSaleLineItem,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.OK, saleModel);
            }
            return null;
        }

        /// <summary>
        /// Remove Sale Line Item
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        [Route("items/remove")]
        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ErrorResponse))]
        public HttpResponseMessage RemoveSaleLineItem(int tillNumber, int saleNumber, int lineNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleController,RemoveSaleLineItem,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var sale = _saleManager.RemoveSaleLineItem(userCode, tillNumber, saleNumber, lineNumber, out error, true,
                true);
            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                var statusCode = error.StatusCode == 0 ? HttpStatusCode.BadRequest : error.StatusCode;
                return Request.CreateResponse(
                    statusCode,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }

            var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
            var enableButtons = _saleManager.EnableCashButton(sale, userCode);
            var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
            var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
            _performancelog.Debug($"End,SaleController,RemoveSaleLineItem,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, saleModel);
        }

        /// <summary>
        /// Add Sale Line Item
        /// </summary>
        /// <param name="saleLineModel"></param>
        /// <returns>Sale</returns>
        [Route("items/update")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ErrorResponse))]
        public HttpResponseMessage UpdateSaleLineItem([FromBody] UpdateSaleLineModel saleLineModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleController,UpdateSaleLineItem,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (saleLineModel == null)
            {
                return Request.CreateResponse(
                  HttpStatusCode.BadRequest,
                  new ErrorResponse
                  {
                      Error = new MessageStyle { Message = Resource.InvalidDataEntered, MessageType = 0 }
                  });
            }

            // var sale = new Sale();
            var sale = _saleManager.UpdateSaleLine(saleLineModel.SaleNumber, saleLineModel.TillNumber,
                saleLineModel.LineNumber, userCode, saleLineModel.Discount,
                saleLineModel.DiscountType, saleLineModel.Quantity, saleLineModel.Price, saleLineModel.ReasonCode,
                saleLineModel.ReasonType, saleLineModel.RegisterNumber,
                out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message) && error.StatusCode != HttpStatusCode.OK)
            {
                return Request.CreateResponse(error.StatusCode,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            if (sale != null)
            {
                object saleModel;
                if (!string.IsNullOrEmpty(error.MessageStyle.Message))
                {
                    var message = new
                    {
                        error = error.MessageStyle,
                    };
                    var messages = new List<object> { message };
                    var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                    var enableButtons = _saleManager.EnableCashButton(sale, userCode);
                    var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
                    saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff, messages);
                }
                else
                {
                    var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                    var enableButtons = _saleManager.EnableCashButton(sale, userCode);
                    var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
                    saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
                }
                _performancelog.Debug($"End,SaleController,UpdateSaleLineItem,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.OK, saleModel);
            }
            return null;
        }

        /// <summary>
        /// Method to Set Loyalty customer
        /// </summary>
        /// <param name="customerCode">Search term</param>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        /// <param name="registerNumber"></param>
        /// <returns>List of customers</returns>
        [Route("setCustomer")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ErrorResponse))]
        public HttpResponseMessage SetCustomer(string customerCode, int tillNumber, int saleNumber, byte registerNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleController,UpdateSaleLineItem,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var sale = _saleManager.SetCustomer(customerCode, saleNumber, tillNumber, userCode, registerNumber,
                string.Empty, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message) && sale == null)
            {
                return Request.CreateResponse(error.StatusCode,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            else if (!string.IsNullOrEmpty(error.MessageStyle.Message) && sale != null)
            {
                var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                var enableButtons = _saleManager.EnableCashButton(sale, userCode);
                var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
                var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
                if (saleModel.SaleLineErrors == null)
                {
                    saleModel.SaleLineErrors = new List<object>();
                    saleModel.SaleLineErrors.Add(new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
                }
                else
                {
                    saleModel.SaleLineErrors.Add(new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
                }
                _performancelog.Debug($"End,SaleController,SetCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.OK, saleModel);
            }

            if (error.MessageStyle != null && string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                var enableButtons = _saleManager.EnableCashButton(sale, userCode);
                var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
                var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
                _performancelog.Debug($"End,SaleController,SetCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.OK, saleModel);
            }
            return null;
        }


        /// <summary>
        /// Suspend Sales
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        [Route("suspendedSales")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<SuspendSale>))]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetAllSuspendedSale(int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug(
                $"Start,SaleController,GetAllSuspendedSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage errorMessage;
            var allSuspendedSale = _suspendedSaleManger.GetSuspendedSale(tillNumber, out errorMessage);


            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle,
                    });
            }

            if (allSuspendedSale?.Count > 0)
            {
                var suspendedSaleModel = new
                {
                    SuspendedSale = from susSale in allSuspendedSale
                                    select new SuspendSale
                                    {
                                        SaleNumber = susSale.SaleNumber,
                                        TillNumber = susSale.TillNumber,
                                        Customer = susSale.Client
                                    }
                };
                _performancelog.Debug(
                $"End,SaleController,GetAllSuspendedSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.OK, suspendedSaleModel);
            }
            return null;

        }


        /// <summary>
        /// Suspend
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        [Route("suspend")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ErrorResponse))]
        public HttpResponseMessage SuspendSale(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleController,SuspendSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            ErrorMessage errorMessage;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var suspendSale = _suspendedSaleManger.SuspendSale(tillNumber, saleNumber, userCode, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle?.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }

            var newSale = new SaleModel
            {
                TillNumber = suspendSale.TillNumber,
                SaleNumber = suspendSale.Sale_Num,
                Customer = suspendSale.Customer.Name,
                CustomerDisplayText = suspendSale.CustomerDisplay
            };

            _performancelog.Debug($"End,SaleController,SuspendSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, newSale);

        }


        /// <summary>
        /// Unsuspend
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        [Route("unsuspend")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ErrorResponse))]
        public HttpResponseMessage UnSuspendSale(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;

            _performancelog.Debug($"Start,SaleController,UnSuspendSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            if (saleNumber > 0)
            {
                ErrorMessage errorMessage;
                var sale = _suspendedSaleManger.UnsuspendSale(saleNumber, tillNumber, userCode, out errorMessage);

                if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                {
                    return Request.CreateResponse(
                        HttpStatusCode.NotFound,
                        new ErrorResponse
                        {
                            Error = errorMessage.MessageStyle
                        });
                }
                if (sale != null)
                {
                    var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                    var enableButtons = _saleManager.EnableCashButton(sale, userCode);
                    var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
                    var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
                    _performancelog.Debug(
                        $"End,SaleController,UnSuspendSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                    return Request.CreateResponse(HttpStatusCode.OK, saleModel);

                }

            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
            {
                Error = new MessageStyle { Message = Resource.SaleNotExist, MessageType = (MessageType)16 }
            });
        }

        /// <summary>
        /// Void Sale
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        [Route("validateVoid")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SuccessReponse))]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ErrorResponse))]
        public HttpResponseMessage VerifyVoid(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleController,VoidSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var response = _suspendedSaleManger.VerifyVoidSale(userCode, saleNumber, tillNumber, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(error.StatusCode,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }

            _performancelog.Debug($"End,SaleController,VoidSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessReponse
            {
                Success = response
            });

        }

        /// <summary>
        /// Void Sale
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("void")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CompletePaymentResponseModel))]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ErrorResponse))]
        public HttpResponseMessage VoidSale([FromBody]VoidSaleModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleController,VoidSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            Report fs;
            var sale = _suspendedSaleManger.VoidSale(userCode, model.SaleNumber, model.TillNumber,
                model.VoidReason, out error, out fs);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(error.StatusCode,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }

            var completePaymentResponseModel = new CompletePaymentResponseModel
            {
                NewSale = new NewSale
                {
                    SaleNumber = sale.Sale_Num,
                    TillNumber = sale.TillNumber,
                    Customer = sale.Customer.Name
                },
                CustomerDisplays = new List<CustomerDisplay> { sale.CustomerDisplay }
            };
            try
            {
                if (fs != null)
                {
                    completePaymentResponseModel.PaymentReceipt = new ReportModel
                    {
                        ReportName = fs.ReportName,
                        ReportContent = fs.ReportContent
                    };
                }
                _performancelog.Debug($"End,SaleController,VoidSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return Request.CreateResponse(HttpStatusCode.OK, completePaymentResponseModel);
            }
            catch
            {
                _performancelog.Debug($"End,SaleController,VoidSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                });
            }
        }

        /// <summary>
        /// Method to write off a sale
        /// </summary>
        /// <param name="model"></param>
        /// <returns>New Sale</returns>
        [Route("writeoff")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(WriteOffResponseModel))]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ErrorResponse))]
        public HttpResponseMessage WriteOff([FromBody]WriteOffModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleController,WriteOff,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            int registerNumber;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var fs = _suspendedSaleManger.WriteOff(userCode, model.SaleNumber, model.TillNumber,
                model.WriteOffReason, out error, out registerNumber);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(error.StatusCode,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            var newSale = _saleManager.InitializeSale(model.TillNumber, registerNumber, userCode, out error);
            var saleModel = new NewSale
            {
                TillNumber = newSale.TillNumber,
                SaleNumber = newSale.Sale_Num,
                Customer = newSale.Customer.Name
            };
            _performancelog.Debug($"End,SaleController,WriteOff,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            try
            {
                var writeOffReceipt = new ReportModel
                {
                    ReportName = fs.ReportName,
                    ReportContent = fs.ReportContent
                };
                return Request.CreateResponse(HttpStatusCode.OK, new WriteOffResponseModel
                {
                    NewSale = saleModel,
                    WriteOffReceipt = writeOffReceipt,
                    CustomerDisplay = newSale.CustomerDisplay
                });
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
        /// Set tax exemption refrence number for sale
        /// </summary>
        /// <param name="model">Tax exemtion model</param>
        /// <returns>Success/Failure</returns>
        [Route("setTaxExemption")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ErrorResponse))]
        public HttpResponseMessage SetTaxExemption([FromBody]TaxExemptionModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleController,SetTaxExemption,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            ErrorMessage error;
            var sale = _saleManager.SetTaxExemptionCode(model.SaleNumber, model.TillNumber,
                userCode, model.TaxExemptionCode, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(
                    HttpStatusCode.NotFound,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle
                    });
            }
            if (sale != null)
            {
                var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                var enableButtons = _saleManager.EnableCashButton(sale, userCode);
                var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
                var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
                _performancelog.Debug(
                    $"End,SaleController,SetTaxExemption,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.OK, saleModel);

            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
            {
                Error = new MessageStyle { Message = Resource.SaleNotExist, MessageType = (MessageType)16 }
            });
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

    }//end class
}//end namespace

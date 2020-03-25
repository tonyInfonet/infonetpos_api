using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Mapper;
using Infonet.CStoreCommander.WebApi.Resources;
using Infonet.CStoreCommander.WebApi.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Report;
using Infonet.CStoreCommander.WebApi.Models.ReturnSale;
using Infonet.CStoreCommander.WebApi.Models.Sale;
using Swashbuckle.Swagger.Annotations;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Return sale controller
    /// </summary>
    [RoutePrefix("api/v1/returnSale")]
    [ApiAuthorization]
    public class ReturnSaleController : ApiController
    {
        private readonly IReturnSaleManager _returnSaleManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ISaleManager _saleManager;
        private readonly ILoginManager _loginManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="returnSaleManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="loginManager"></param>
        /// <param name="saleManager"></param>
        public ReturnSaleController(IReturnSaleManager returnSaleManager,
            IApiResourceManager resourceManager,
            ILoginManager loginManager,
            ISaleManager saleManager)
        {
            _returnSaleManager = returnSaleManager;
            _resourceManager = resourceManager;
            _loginManager = loginManager;
            _saleManager = saleManager;
        }


        /// <summary>
        /// Method to get list of all sales
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of customers</returns>
        [Route("getAllSales")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(List<SaleHeadModel>))]
        public HttpResponseMessage Index(int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleController,Index,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var sales = _returnSaleManager.GetAllSales(pageIndex, pageSize);

            if (sales.Count == 0 && (pageIndex == 1 || pageIndex == 0))
            {
                var store = CacheManager.GetStoreInfo();
                var offSet = store?.OffSet ?? 0;
                var type = (int)MessageType.Exclamation + MessageType.OkOnly;
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = _resourceManager.CreateMessage(offSet,40, 97, null, type)
                });
            }

            var saleHeadModel = (from sale in sales
                                 select new SaleHeadModel
                                 {
                                     SaleNumber = sale.SaleNumber,
                                     TillNumber = sale.TillNumber,
                                     Date = sale.SaleDate,
                                     Time = sale.SaleTime,
                                     TotalAmount = Math.Round(sale.SaleAmount, 2),
                                     AllowCorrection = _returnSaleManager.IsAllowCorrection(sale.SaleNumber),
                                     AllowReason = _returnSaleManager.IsReasonAllowed(sale.SaleNumber)
                                 }).ToList();

            _performancelog.Debug($"End,ReturnSaleController,Index,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, saleHeadModel);
        }

        /// <summary>
        /// Method to get a sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Sale model</returns>
        [Route("getSale")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SaleModel))]
        public HttpResponseMessage GetSale(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleController,GetSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            ErrorMessage message;
            var sale = _returnSaleManager.GetSale(saleNumber, tillNumber, out message);

            if (!string.IsNullOrEmpty(message.MessageStyle.Message))
            {
                _performancelog.Debug($"End,ReturnSaleController,GetSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = message.MessageStyle
                });
            }

            var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
            var enableButtons = _saleManager.EnableCashButton(sale, userCode);
            var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
            var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);

            _performancelog.Debug($"End,ReturnSaleController,GetSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, saleModel);
        }

        /// <summary>
        /// Method to search a sale
        /// </summary>
        /// <param name="searchTerm">search term</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        [Route("searchSale")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(List<SaleHeadModel>))]
        public HttpResponseMessage SearchSale(int? searchTerm, DateTime? saleDate, int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleController,searchSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            ErrorMessage message;

            if (searchTerm <= 0)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = Resource.InvalidRequest, MessageType = MessageType.OkOnly }
                    });
            }

            var sales = _returnSaleManager.SearchSale(saleDate, searchTerm, pageIndex, pageSize, out message);

            if (!string.IsNullOrEmpty(message.MessageStyle.Message))
            {
                _performancelog.Debug(
                    $"End,ReturnSaleController,SearchSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = message.MessageStyle
                });
            }
            var saleHeadModel = from sale in sales
                                select new SaleHeadModel
                                {
                                    SaleNumber = sale.SaleNumber,
                                    TillNumber = sale.TillNumber,
                                    Date = sale.SaleDate,
                                    Time = sale.SaleTime,
                                    TotalAmount = sale.SaleAmount,
                                    AllowCorrection = _returnSaleManager.IsAllowCorrection(sale.SaleNumber),
                                    AllowReason = _returnSaleManager.IsReasonAllowed(sale.SaleNumber)
                                };

            _performancelog.Debug($"End,ReturnSaleController,searchSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, saleHeadModel);
        }

        /// <summary>
        /// Method to return a sale
        /// </summary>
        /// <param name="model">Return sale model</param>
        /// <returns>Sale</returns>
        [Route("return")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(ReturnSaleResponseModel))]
        public HttpResponseMessage ReturnSale([FromBody] ReturnSaleModel model)
        {
            var dateStart = DateTime.Now;
            ErrorMessage message;
            List<ErrorMessage> saleLineMessages;
            _performancelog.Debug($"Start,ReturnSaleController,return,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var user = CacheManager.GetUser(userCode) ?? _loginManager.GetUser(userCode);
            Report fs;
            string fileName;
            var sale = _returnSaleManager.ReturnSale(user, model.SaleNumber, model.TillNumber, model.SaleTillNumber, model.IsCorrection,
                model.ReasonType, model.ReasonCode, out message, out saleLineMessages, out fs, out fileName);

            if (!string.IsNullOrEmpty(message.MessageStyle.Message))
            {
                _performancelog.Debug(
                    $"End,ReturnSaleController,return,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = message.MessageStyle
                });
            }

            var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
            var messages = new List<object>();
            if (saleLineMessages.Count > 0)
            {
                messages.AddRange(saleLineMessages.Select(saleLineMessage => new
                {
                    error = saleLineMessage.MessageStyle,
                }));
            }
            var enableButtons = _saleManager.EnableCashButton(sale, userCode);
            var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
            var saleModel = messages.Count > 0 ? SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff, messages) :
                SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
            var returnSale = new ReturnSaleResponseModel
            {
                Sale = saleModel
            };
            if (fs != null)
            {
                returnSale.Receipt = new ReportModel
                {
                    ReportContent = fs.ReportName,
                    ReportName = fs.ReportContent,
                    Copies = 1
                };
            }
            _performancelog.Debug($"End,ReturnSaleController,return,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, returnSale);
        }


        /// <summary>
        /// Method to return sale items
        /// </summary>
        /// <param name="model">Return sale model</param>
        /// <returns>Sale model</returns>
        [Route("returnItems")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SaleModel))]
        public HttpResponseMessage ReturnSaleItems([FromBody]ReturnSaleModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleController,returnItems,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            ErrorMessage message;
            List<ErrorMessage> saleLineMessages;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var user = CacheManager.GetUser(userCode) ?? _loginManager.GetUser(userCode);

            if (model.SaleLines == null || model.SaleLines.Length <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = new MessageStyle { Message = Resource.LineRequired, MessageType = MessageType.OkOnly }
                });
            }

            var sale = _returnSaleManager.ReturnSaleItems(user, model.SaleTillNumber, model.TillNumber, model.SaleNumber,
                model.SaleLines, model.IsCorrection, model.ReasonType, model.ReasonCode, out message,
                out saleLineMessages);


            if (!string.IsNullOrEmpty(message.MessageStyle.Message))
            {
                _performancelog.Debug($"End,ReturnSaleController,returnItems,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = message.MessageStyle
                });
            }

            var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);

            var messages = new List<object>();
            if (saleLineMessages.Count > 0)
            {
                messages.AddRange(saleLineMessages.Select(saleLineMessage => new
                {
                    error = saleLineMessage.MessageStyle,
                }));
            }
            var enableButtons = _saleManager.EnableCashButton(sale, userCode);
            var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
            var saleModel = messages.Count > 0 ? SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff, messages) :
                SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
            _performancelog.Debug($"End,ReturnSaleController,returnItems,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, saleModel);
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

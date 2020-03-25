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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.PriceCheck;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Price Check Controller
    /// </summary>
    [RoutePrefix("api/v1/priceCheck")]
    [ApiAuthorization]
    public class PriceCheckController : ApiController
    {
        private readonly IPriceCheckManager _priceCheckManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="priceCheckManager"></param>
        public PriceCheckController(IPriceCheckManager priceCheckManager)
        {
            _priceCheckManager = priceCheckManager;
        }

        /// <summary>
        /// Check Price Types for stock Code
        /// </summary>
        /// <returns></returns>
        [Route("getStockPrices")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(PriceCheckModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetStockPrices(string stockCode, int tillNumber,
            int saleNumber, byte registerNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PriceCheckController,GetStockPrices,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;

            var result = _priceCheckManager.GetStockPriceDetails(stockCode, tillNumber, saleNumber, registerNumber, userCode, out error);

            _performancelog.Debug($"End,PriceCheckController,GetStockPrices,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
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
            return Request.CreateResponse(HttpStatusCode.OK, result);

        }

        /// <summary>
        /// Updates Regular Price
        /// </summary>
        /// <returns></returns>
        [Route("updateRegularPrice")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(PriceCheckModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage ApplyRegularPrice([FromBody] RegularPriceCheckModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PriceCheckController,ApplyRegularPrice,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;

            var regularPrice = new RegularPriceCheck
            {
                RegularPrice = model.RegularPrice,
                RegisterNumber = model.RegisterNumber,
                StockCode = model.StockCode,
                TillNumber = model.TillNumber,
                SaleNumber = model.SaleNumber
            };

            var result = _priceCheckManager.ApplyRegularPrice(regularPrice, userCode, out error);

            _performancelog.Debug($"End,PriceCheckController,ApplyRegularPrice,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
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
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        /// <summary>
        /// Updates Special Price
        /// </summary>
        /// <returns></returns>
        [Route("updateSpecialPrice")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(PriceCheckModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage ApplySpecialPrice([FromBody] SpecialPriceCheckModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PriceCheckController,ApplySpecialPrice,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;

            var specialPrice = new SpecialPriceCheck
            {
                RegularPrice = model.RegularPrice,
                RegisterNumber = model.RegisterNumber,
                StockCode = model.StockCode,
                TillNumber = model.TillNumber,
                SaleNumber = model.SaleNumber,
                Fromdate = model.Fromdate,
                IsEndDate = model.IsEndDate,
                PerDollarChecked = model.PerDollarChecked,
                PriceType = model.PriceType,
                Todate = model.Todate,
                GridPrices = (from priceGrd in model.GridPrices
                              select new Entities.PriceGrid
                              {
                                  Column1 = priceGrd.Column1,
                                  Column2 = priceGrd.Column2,
                                  Column3 = priceGrd.Column3
                              }).ToList()
            };

            var result = _priceCheckManager.ApplySpecialPrice(specialPrice, userCode, out error);

            _performancelog.Debug($"End,PriceCheckController,ApplySpecialPrice,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
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
            return Request.CreateResponse(HttpStatusCode.OK, result);

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

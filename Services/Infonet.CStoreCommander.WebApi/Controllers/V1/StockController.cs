using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
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
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Stock;
using Swashbuckle.Swagger.Annotations;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for stock
    /// </summary>
    [RoutePrefix("api/v1/stock")]
    [ApiAuthorization]
    public class StockController : ApiController
    {
        private readonly IStockManager _stockManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Stock Controller
        /// </summary>
        /// <param name="stockManager"></param>
        public StockController(IStockManager stockManager)
        {
            _stockManager = stockManager;
        }

        /// <summary>
        /// Method to get list of stockitems
        /// </summary>
        /// <returns>List of stock items</returns>
        [Route("items")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(List<StockItemModel>))]
        public HttpResponseMessage Index(int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockV1Controller,Index,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var stockItems = _stockManager.GetStockItems(pageIndex, pageSize);

            var listStockItems = stockItems.Select(stockItem => new StockItemModel
            {
                StockCode = stockItem.StockCode,
                Description = stockItem.Description,
                AlternateCode = stockItem.AlternateCode
            }).ToList();

            _performancelog.Debug($"End,StockV1Controller,Index,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, listStockItems);
        }

        /// <summary>
        /// Method to get a stock by code
        /// </summary>
        /// <param name="code">Stock code</param>
        /// <returns>Response message</returns>
        [Route("")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(StockItemModel))]
        public HttpResponseMessage GetStockItemCode(string code)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockV1Controller,GetStockItemCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            var stockItem = _stockManager.GetStockByCode(code, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                _performancelog.Debug($"End,StockV1Controller,GetStockItemCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return Request.CreateResponse(error.StatusCode, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }
            _performancelog.Debug($"End,StockV1Controller,GetStockItemCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, new StockItemModel
            {
                StockCode = stockItem.StockCode,
                Description = stockItem.Description,
                AlternateCode = stockItem.AlternateCode
            });

        }

        /// <summary>
        /// Method to search stock items
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of stock items</returns>
        [Route("search")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(List<StockItemModel>))]
        public HttpResponseMessage SearchStock(string searchTerm, int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockV1Controller,SearchStock,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (string.IsNullOrEmpty(searchTerm))
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = Resource.InvalidRequest, MessageType = 0 }
                    });
            }

            var stockItems = _stockManager.SearchStockItems(searchTerm, pageIndex, pageSize);
            if (stockItems.Count == 0 && (pageIndex == 1 || pageIndex == 0))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = new MessageStyle
                    {
                        Message = Resource.StockNotExists,
                        MessageType = MessageType.OkOnly
                    }
                });
            }


            var listStockItems = stockItems.Select(stockItem => new StockItemModel
            {
                StockCode = stockItem.StockCode,
                Description = stockItem.Description,
                AlternateCode = stockItem.AlternateCode
            }).ToList();
            _performancelog.Debug($"End,StockV1Controller,SearchStock,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, listStockItems);
        }

        /// <summary>
        /// Method to add a stock
        /// </summary>
        /// <param name="stock">Stock</param>
        /// <returns>Response message</returns>
        [Route("add")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SuccessReponse))]
        public HttpResponseMessage AddStock([FromBody]StockModel stock)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockV1Controller,AddStock,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            var accessToken = Request.GetFirstHeaderValueOrDefault<string>("authToken");
            if (accessToken == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                    });
            }
            if (stock == null)
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
            var userCode = TokenValidator.GetUserCode(accessToken);
            var stockItem = new StockItem
            {
                Description = stock.Description,
                Price = stock.RegularPrice,
                StockCode = stock.StockCode
            };
            _stockManager.AddStockItem(userCode, stockItem, stock.TaxCodes, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                _performancelog.Debug($"End,StockV1Controller,AddStock,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return Request.CreateResponse(error.StatusCode, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }
            _performancelog.Debug($"End,StockV1Controller,AddStock,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessReponse
            {
                Success = true

            });
        }


        /// <summary>
        /// Get Hot Product Pages
        /// </summary>
        /// <returns></returns>
        [Route("getHotProductPages")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(List<HotButtonPageModel>))]
        public HttpResponseMessage GetHotButtonpages()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockV1Controller,GetHotButtonpages,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var hotButtonPages = _stockManager.GetHotButonPages();

            var listHotButtonpages = hotButtonPages.Select(hotButton => new HotButtonPageModel
            {
                PageId = hotButton.Key,
                PageName = hotButton.Value
            }).ToList();

            _performancelog.Debug($"End,StockV1Controller,GetHotButtonpages,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, listHotButtonpages);
        }


        /// <summary>
        /// Get Hot Products
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [Route("getHotProducts")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(List<HotButtonModel>))]
        public HttpResponseMessage GetHotButtons(int pageId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockV1Controller,GetHotButtons,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var hotbuttons = _stockManager.GetHotButons(pageId);

            var listHotButtons = from hotButton in hotbuttons
                                 select new HotButtonModel
                                 {
                                     ButtonId = hotButton.Button_Number,
                                     DefaultQuantity = hotButton.DefaultQuantity,
                                     StockCode = hotButton.StockCode,
                                     ImageUrl = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, "/images/" + hotButton.ImageUrl),
                                     Description = hotButton.Button_Product
                                 };

            _performancelog.Debug($"End,StockV1Controller,GetHotButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, listHotButtons);
        }

    }
}

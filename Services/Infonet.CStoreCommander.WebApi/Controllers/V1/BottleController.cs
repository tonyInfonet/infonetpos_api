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
using Infonet.CStoreCommander.WebApi.Models.Bottle;
using Infonet.CStoreCommander.WebApi.Models.Report;
using Infonet.CStoreCommander.WebApi.Models.Sale;
using Infonet.CStoreCommander.WebApi.Models.Common;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Bottle Controller
    /// </summary>
    [RoutePrefix("api/v1/bottle")]
    [ApiAuthorization]
    public class BottleController : ApiController
    {
        private readonly IBottleManager _bottleManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        #region Constructor 

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bottleManager"></param>
        public BottleController(IBottleManager bottleManager)
        {
            _bottleManager = bottleManager;
        }

        #endregion

        #region Controller methods

        /// <summary>
        /// Get all bottles
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns>List of bottles</returns>
        [Route("getBottles")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<BottleReturnModel>))]
        public HttpResponseMessage Index(int pageId = 1)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,BottleController,Index,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var bottles = _bottleManager.GetBottles(pageId);

            List<BottleReturnModel> response = GetBottleReturn(bottles);

            _performancelog.Debug($"End,BottleController,Index,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }



        /// <summary>
        /// Return bottle
        /// </summary>
        /// <param name="bottleReturn"></param>
        /// <returns>HttpResponse message</returns>
        [Route("returns")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CompleteBottleReturnResponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage SaveBottleReturns([FromBody]BottleReturnInputModel bottleReturn)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,BottleController,SaveBottleReturns,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            BR_Payment brPayments = GetBrPayment(bottleReturn);
            ErrorMessage errorMessage;

            bool openDrawer;
            Report fs;
            var sale = _bottleManager.SaveBottleReturn(brPayments, out errorMessage, out fs, out openDrawer);

            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }

            var completePaymentResponseModel = new CompleteBottleReturnResponseModel
            {
                NewSale = new NewSale
                {
                    SaleNumber = sale.Sale_Num,
                    TillNumber = sale.TillNumber,
                    Customer = sale.Customer.Name
                },
                OpenCashDrawer = openDrawer,
                CustomerDisplay = sale.CustomerDisplay

            };
            if (fs != null)
            {
                completePaymentResponseModel.PaymentReceipt = new ReportModel
                {
                    ReportName = fs.ReportName,
                    ReportContent = fs.ReportContent,
                    Copies = fs.Copies
                };
            }
            _performancelog.Debug($"End,BottleController,SaveBottleReturns,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, completePaymentResponseModel);
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Method to convert bottle return to Br payment model
        /// </summary>
        /// <param name="bottleReturn">Bottle return model</param>
        /// <returns>BR payment</returns>
        private BR_Payment GetBrPayment(BottleReturnInputModel bottleReturn)
        {
            var brPayments = new BR_Payment
            {
                Sale_Num = bottleReturn.SaleNumber,
                TillNumber = bottleReturn.TillNumber,
                Amount = bottleReturn.Amount,
                RegisterNumber = bottleReturn.RegisterNumber
            };

            foreach (var bottle in bottleReturn.Bottles)
            {
                brPayments.Br_Lines.AddLine(bottle.LineNumber, bottle, "");
            }

            return brPayments;
        }

        /// <summary>
        /// Method to get list of bottle return
        /// </summary>
        /// <param name="bottles">List of bottles</param>
        /// <returns></returns>
        private List<BottleReturnModel> GetBottleReturn(List<BottleReturn> bottles)
        {
            return bottles.Select(bottle => new BottleReturnModel
            {
                Description = bottle.Description,
                ImageUrl =
                     Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery,
                         "/images/" + bottle.Image_Url),
                Product = bottle.Product,
                Price = bottle.Price,
                DefaultQuantity = bottle.Quantity,
                Amount = bottle.Amount
            }).ToList();
        }

        #endregion
    }
}

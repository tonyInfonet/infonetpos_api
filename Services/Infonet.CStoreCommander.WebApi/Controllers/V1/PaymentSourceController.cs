using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Resources;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Customer;
using Swashbuckle.Swagger.Annotations;

using Infonet.CStoreCommander.WebApi.Utilities;
using Infonet.CStoreCommander.WebApi.Mapper;
using Infonet.CStoreCommander.BusinessLayer.Entities;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for payment source integration
    /// </summary>
    [RoutePrefix("api/v1/paymentsource")]
    [ApiAuthorization]
    public class PaymentSourceController : ApiController
    {
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;
        private readonly IPaymentSourceManager _PaymentSourceManager;
        /// <summary>
        
        /// </summary>
        public PaymentSourceController(IPaymentSourceManager PaymentSourceManager)
        {
            _PaymentSourceManager = PaymentSourceManager;
        }
        /// <summary>
        /// Try to get refund ps sale line infomation stockcode, sale amount,product name
        /// Return a PSRefund instance if finds the refund by transid.
        /// return null if not find.
        /// </summary>
        [Route("getRefundInfo")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(PSRefund))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]

        public HttpResponseMessage GetRefundInfo(string TransID, int SALE_NO, int TILL_NUM)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentSourceController,GetRefundInfo,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");


            PSRefund psr = _PaymentSourceManager.GetPSRefund(TILL_NUM, SALE_NO, TransID);



            _performancelog.Debug($"End,PaymentSourceController,GetRefundInfo,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, psr);
        }
        /// <summary>
        /// Try to download new product file
        /// If a new file is downloaded, return true.
        /// return false if downloading fails
        /// </summary>
        [Route("getNewFile")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        
        public HttpResponseMessage GetNewFile()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentSourceController,GetNewFile,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            
            bool bDownload = _PaymentSourceManager.DownloadFile();



            _performancelog.Debug($"End,PaymentSourceController,GetNewFile,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, bDownload);
        }
        /// <summary>
        /// Get all payment source logos

        /// </summary>
        [Route("getPSLogos")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<PSLogo>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]

        public HttpResponseMessage GetPSLogos()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentSourceController,GetPSProducts,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            List<PSLogo> logos = _PaymentSourceManager.GetPSLogos();
            _performancelog.Debug($"End,PaymentSourceController,GetPSProducts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, logos);
        }
        /// <summary>
        /// Get all Payment Source products

        /// </summary>
        [Route("getPSProducts")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<PSProduct>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]

        public HttpResponseMessage GetPSProducts()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentSourceController,GetPSProducts,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            List<PSProduct> products = _PaymentSourceManager.GetPSProducts();
            _performancelog.Debug($"End,PaymentSourceController,GetPSProducts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, products);
        }
        /// <summary>
        /// Get Payment source transactions for the number of past days

        /// </summary>
        [Route("getPSTransactions")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<PSTransaction>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]

        public HttpResponseMessage GetPSTransactions(int TILL_NUM, int SALE_NO, int PastDays)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentSourceController,GetPSTransactions,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            List<PSTransaction> trans = _PaymentSourceManager.GetPSTransactions(TILL_NUM, SALE_NO, PastDays);
            _performancelog.Debug($"End,PaymentSourceController,GetPSTransactions,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, trans);
        }
        /// <summary>
        /// Get payment source transactionid

        /// </summary>
        [Route("getPSTransactionID")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]

        public HttpResponseMessage GetPSTranactionID()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentSourceController,GetPSTranactionID,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string transid = _PaymentSourceManager.GetPSTransactionID();
            _performancelog.Debug($"End,PaymentSourceController,GetPSTranactionID,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, transid);
        }
        /// <summary>
        /// Get payment source profile

        /// </summary>
        [Route("getPSProfile")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(PSProfile))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]

        public HttpResponseMessage GetPSProfile()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentSourceController,GetPSProfile,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            PSProfile pspf = _PaymentSourceManager.GetPSProfile();
            _performancelog.Debug($"End,PaymentSourceController,GetPSProfile,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, pspf);
        }
        /// <summary>
        /// Get payment source profile

        /// </summary>
        [Route("getPSVoucherInfo")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(PSVoucherInfo))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]

        public HttpResponseMessage GetPSVoucherInfo(string ProdName)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentSourceController,getPSVoucherInfo,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            PSVoucherInfo psvcinfo = _PaymentSourceManager.GetPSVoucherInfo(ProdName);
            
            _performancelog.Debug($"End,PaymentSourceController,getPSVoucherInfo,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, psvcinfo);
        }
        /// <summary>
        /// Try to download new product file
        /// If a new file is downloaded, return true.
        /// return false if downloading fails
        /// </summary>
        [Route("savePSTansID")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]

        public HttpResponseMessage SavePSTransactionID(int TILL_NUM, int SALE_NO, int LINE_NUM, string TransID)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,PaymentSourceController,SavePSTransactionID,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");


            bool bSaved = _PaymentSourceManager.SavePSTransactionID(TILL_NUM, SALE_NO, LINE_NUM, TransID);



            _performancelog.Debug($"End,PaymentSourceController,SavePSTransactionID,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, bSaved);
        }
    }
}

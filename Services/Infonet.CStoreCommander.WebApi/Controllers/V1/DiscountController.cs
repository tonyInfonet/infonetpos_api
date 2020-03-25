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

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for Discount
    /// </summary>
    [RoutePrefix("api/v1/discount")]
    [ApiAuthorization]
    public class DiscountController : ApiController
    {
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;
        //private readonly ICustomerManager _customerManager;
        //private readonly IGivexManager _givexManager;
        private readonly IFuelDiscountManager _fuelDiscountManager;
        /// <summary>
        /// Controller for Discount
        /// </summary>
        public DiscountController(IFuelDiscountManager fuelDiscountManager )
        {
            //_customerManager = customerManager;
            //_givexManager = givexManager;
            _fuelDiscountManager = fuelDiscountManager;
        }
        /// <summary>
        /// Get all discouts
        /// </summary>
        [Route("getDiscounts")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ClientGroup>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetAllDiscount()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,DiscountController,GetAllDiscount,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            //List<ClientGroup> olist = new List<ClientGroup>()
            //{
            //    new ClientGroup() {GroupId="1",GroupName="Cash/Debit discount",DiscountType="$",DiscountRate=0.02f,Footer="",DiscountName="" },
            //    new ClientGroup() {GroupId="2",GroupName="5 for 5 Discount",DiscountType="$",DiscountRate=0.05f,Footer="",DiscountName="" },
            //    new ClientGroup() {GroupId="3",GroupName="8.5 CPL (Cash/Debit + 5 for 5)",DiscountType="$",DiscountRate=0.085f,Footer="",DiscountName="" },
            //    new ClientGroup() {GroupId="4",GroupName="test",DiscountType="%",DiscountRate=10f,Footer="",DiscountName="" }
            //}
            //    ;
            List<ClientGroup> olist = _fuelDiscountManager.GetClientGroups();



            _performancelog.Debug($"End,DiscountController,Index,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, olist);
        }

        /// <summary>
        /// Get all fuel codes
        /// </summary>
        [Route("getFuelCodes")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetFuelCodes()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,DiscountController,GetFuelCodes,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            //string sCodes = "DIESEL,MIDGRADE,PREMIUM,REGULAR";
            string sCodes = _fuelDiscountManager.GetFuelCodes();



            _performancelog.Debug($"End,DiscountController,Index,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, sCodes);
        }
    }
}

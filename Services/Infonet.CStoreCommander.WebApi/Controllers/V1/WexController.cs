using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.WebApi.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for payment using wex fleet card
    /// </summary>
    public class WexController : ApiController
    {
        private readonly IWexManager _wexManager;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="wexManager"></param>
        public WexController(IWexManager wexManager)
        {
            _wexManager = wexManager;
        }

        /// <summary>
        /// Controller to return the profile prompt assosiated with the wex card
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorization]
        public HttpResponseMessage GetWexProfilePrompts()
        {
            try
            {
                var prompts = _wexManager.GetWexProfilePrompts();
                return Request.CreateResponse(HttpStatusCode.OK, prompts);
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}

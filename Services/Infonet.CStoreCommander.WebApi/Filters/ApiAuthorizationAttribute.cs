using Infonet.CStoreCommander.WebApi.Exceptions;
using Infonet.CStoreCommander.WebApi.Utilities;
using System;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.WebApi.Resources;

namespace Infonet.CStoreCommander.WebApi.Filters
{
    /// <summary>
    /// Filter class for API Authorization
    /// </summary>
    public class ApiAuthorizationAttribute : AuthorizationFilterAttribute
    {
        private readonly string _securityToken = Resource.AuthToken; // Name of the url parameter.

        /// <summary>
        /// override of OnAuthorization called when filter is applied
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (AuthorizeRequest(actionContext))
            {
                return;
            }
            throw new InvalidTokenException();
        }

        /// <summary>
        /// Authorize request
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private bool AuthorizeRequest(HttpActionContext actionContext)
        {
            try
            {
                var token = actionContext.Request.Headers.FirstOrDefault(x => x.Key == _securityToken).Value;
                return TokenValidator.IsValidToken(token.First());
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Infonet.CStoreCommander.WebApi.Utilities
{
    /// <summary>
    /// Http request message extension class
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Gets the Header values from request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="headerKey"></param>
        /// <returns></returns>
        public static T GetFirstHeaderValueOrDefault<T>(
        this HttpRequestMessage request,
        string headerKey)
        {
            var toReturn = default(T);

            IEnumerable<string> headerValues;

            if (request.Headers.TryGetValues(headerKey, out headerValues))
            {
                var valueString = headerValues.FirstOrDefault();
                if (valueString != null)
                {
                    return (T)Convert.ChangeType(valueString, typeof(T));
                }
            }

            return toReturn;
        }
    }
}
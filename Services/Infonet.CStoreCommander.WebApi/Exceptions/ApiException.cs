using System;
using System.Net;
using System.Runtime.Serialization;

namespace Infonet.CStoreCommander.WebApi.Exceptions
{
    /// <summary>
    /// Class for API exception
    /// </summary>
    [Serializable]
    [DataContract]
    public class ApiException : Exception, IApiException
    {
        #region Public Serializable properties. 
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public HttpStatusCode HttpStatus { get; set; }

        string _reasonPhrase = "ApiException";
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ReasonPhrase
        {
            get { return _reasonPhrase; }

            set { _reasonPhrase = value; }
        }
        #endregion
    }
}
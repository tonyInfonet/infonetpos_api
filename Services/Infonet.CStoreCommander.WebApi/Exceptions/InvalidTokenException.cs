using Infonet.CStoreCommander.WebApi.Resources;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Infonet.CStoreCommander.WebApi.Exceptions
{
    /// <summary>
    /// Class for Invalid Token Exception
    /// </summary>
    [Serializable]
    [DataContract]
    public class InvalidTokenException : Exception, IApiException
    {
        #region Public Serializable properties.  

        private string _reasonPhrase = Resource.InvalidAuthToken;

        private string _errorDescription = Resource.InvalidAuthToken;

        private HttpStatusCode _status = HttpStatusCode.Unauthorized;
        /// <summary>
        /// Error Description
        /// </summary>
        [DataMember]
        public string ErrorDescription
        {
            get
            {
                return _errorDescription;
            }
            set
            {
                _errorDescription = value;
            }
        }
        /// <summary>
        /// Http Status
        /// </summary>
        [DataMember]
        public HttpStatusCode HttpStatus
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }
        /// <summary>
        /// Reason Phrase
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
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Infonet.CStoreCommander.WebApi.Exceptions
{
    /// <summary>
    /// Class for API Business exception
    /// </summary>
    [Serializable]
    [DataContract]
    public class ApiBusinessException : Exception, IApiException
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

        string _reasonPhrase = "ApiBusinessException";
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

        #region Public Constructor.  

        /// <summary>  
        /// Public constructor for Api Business Exception  
        /// </summary>
        /// <param name="errorDescription"></param>  
        /// <param name="httpStatus"></param>  
        public ApiBusinessException(string errorDescription, HttpStatusCode httpStatus)
        {
            ErrorDescription = errorDescription;
            HttpStatus = httpStatus;
        }
        #endregion

    }
}
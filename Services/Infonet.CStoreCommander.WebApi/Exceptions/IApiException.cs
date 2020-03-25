using System.Net;

namespace Infonet.CStoreCommander.WebApi.Exceptions
{
    /// <summary>  
    /// IApiException Interface  
    /// </summary>  
    public interface IApiException
    {
        /// <summary>  
        /// Error Description  
        /// </summary>  
        string ErrorDescription { get; set; }

        /// <summary>  
        /// Http Status  
        /// </summary>  
        HttpStatusCode HttpStatus { get; set; }

        /// <summary>  
        /// Reason Phrase  
        /// </summary>  
        string ReasonPhrase { get; set; }
    }
}
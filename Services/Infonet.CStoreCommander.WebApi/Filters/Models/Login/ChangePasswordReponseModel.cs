using Infonet.CStoreCommander.Resources;

namespace Infonet.CStoreCommander.WebApi.Models.Login
{
    /// <summary>
    /// Change password reponse
    /// </summary>
    public class ChangePasswordReponseModel
    {
        /// <summary>
        /// Error
        /// </summary>
        public MessageStyle Error { get; set; }

        /// <summary>
        /// Success
        /// </summary>
        public  bool Success { get; set; }
    }
}
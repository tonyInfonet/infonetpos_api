using Infonet.CStoreCommander.Resources;

namespace Infonet.CStoreCommander.WebApi.Models.Login
{
    /// <summary>
    /// Invalid login response model
    /// </summary>
    public class InvalidLoginReponseModel
    {
        /// <summary>
        /// Error
        /// </summary>
        public MessageStyle Error { get; set; }

        /// <summary>
        /// Shut down POS
        /// </summary>
        public  bool ShutDownPOS { get; set; }
    }
}
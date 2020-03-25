using Infonet.CStoreCommander.Resources;

namespace Infonet.CStoreCommander.WebApi.Models.Payout
{
    /// <summary>
    /// Validate fleet response model
    /// </summary>
    public class ValidateFleetResponseModel
    {
        /// <summary>
        /// Message
        /// </summary>
        public MessageStyle Message { get; set; }

        /// <summary>
        /// Caption
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Allow swipe
        /// </summary>
        public bool AllowSwipe { get; set; }
    }
}
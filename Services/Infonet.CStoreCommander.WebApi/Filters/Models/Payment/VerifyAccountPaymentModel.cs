using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Models.Tender;

namespace Infonet.CStoreCommander.WebApi.Models.Payment
{
    /// <summary>
    /// Verify account payment model
    /// </summary>
    public class VerifyAccountPaymentModel
    {
        /// <summary>
        /// Is PO required
        /// </summary>
        public bool IsPurchaseOrderRequired { get; set; }

        /// <summary>
        /// Override AR limit message
        /// </summary>
        public MessageStyle OverrideArLimitMessage { get; set; }

        /// <summary>
        /// Credit message
        /// </summary>
        public MessageStyle CreditMessage { get; set; }

        /// <summary>
        /// Unauthorised message
        /// </summary>
        public MessageStyle UnauthorizedMessage { get; set; }

        /// <summary>
        /// Multi PO use
        /// </summary>
        public bool IsMutiliPO { get; set; }

        public CardInformationResponseModel CardSummary { get; set; }
    }
}
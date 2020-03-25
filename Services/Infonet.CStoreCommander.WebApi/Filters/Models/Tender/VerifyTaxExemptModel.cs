using Infonet.CStoreCommander.Resources;

namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Verify Tax Exempt Model
    /// </summary>
    public class VerifyTaxExemptModel
    {
        /// <summary>
        /// Is ProcessSite Return
        /// </summary>
        public bool ProcessSiteReturn { get; set; }

        /// <summary>
        /// Is Process Site Sale
        /// </summary>
        public bool ProcessSiteSale { get; set; }

        /// <summary>
        /// Process Site Sale Return Button Enable
        /// </summary>
        public bool ProcessSiteSaleRemoveTax { get; set; }

        /// <summary>
        /// Process AITE
        /// </summary>
        public bool ProcessAite { get; set; }

        /// <summary>
        /// Process QITE
        /// </summary>
        public bool ProcessQite { get; set; }

        /// <summary>
        /// Confirm Message
        /// </summary>
        public MessageStyle ConfirmMessage { get; set; }

        /// <summary>
        /// Treaty name
        /// </summary>
        public string TreatyName { get; set; }

        /// <summary>
        /// Treaty number
        /// </summary>
        public string TreatyNumber { get; set; }
    }
}
namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Gift certificate response model
    /// </summary>
    public class GiftCertificateResponseModel
    {
        /// <summary>
        /// Gift cert number
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Sold date
        /// </summary>
        public string SoldOn { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Expires on
        /// </summary>
        public string ExpiresOn { get; set; }

        /// <summary>
        /// Is expired
        /// </summary>
        public bool IsExpired { get; set; }
    }
}
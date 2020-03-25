namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Store credit response model
    /// </summary>
    public class StoreCreditResponseModel
    {
        /// <summary>
        /// Number
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
        /// Expires On
        /// </summary>
        public string ExpiresOn { get; set; }

        /// <summary>
        /// Is expired
        /// </summary>
        public bool IsExpired { get; set; }
    }
}
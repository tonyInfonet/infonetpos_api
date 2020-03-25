namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Tender model
    /// </summary>
    public class TenderModel
    {
        /// <summary>
        /// Tender code
        /// </summary>
        public string TenderCode { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal AmountEntered { get; set; }
    }
}
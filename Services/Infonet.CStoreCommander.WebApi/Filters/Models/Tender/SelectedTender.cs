namespace Infonet.CStoreCommander.WebApi.Models.Tender
{   
    /// <summary>
    /// Selected tender
    /// </summary>
    public class SelectedTender
    {
        /// <summary>
        /// Tender code
        /// </summary>
        public string TenderCode { get; set; }

        /// <summary>
        /// Amount entered
        /// </summary>
        public decimal? AmountEntered { get; set; }
    }
}
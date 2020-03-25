namespace Infonet.CStoreCommander.WebApi.Models.Givex
{
    /// <summary>
    /// Givex card model
    /// </summary>
    public class GivexCardModel
    {
        /// <summary>
        /// Card number
        /// </summary>
        public string GivexCardNumber { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Transaction type
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Tender code
        /// </summary>
        public string TenderCode { get; set; }
    }
}
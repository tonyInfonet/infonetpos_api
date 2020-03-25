namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Card model
    /// </summary>
    public class CardModel
    {
        /// <summary>
        /// Card number
        /// </summary>
        public string CardNumber { get; set; }

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
        
    }
}
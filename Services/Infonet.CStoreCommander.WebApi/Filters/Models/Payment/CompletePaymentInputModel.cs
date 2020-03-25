namespace Infonet.CStoreCommander.WebApi.Models.Payment
{
    /// <summary>
    /// Complete payment input model
    /// </summary>
    public class CompletePaymentInputModel
    {

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Register number
        /// </summary>
        public int RegisterNumber { get; set; }

        /// <summary>
        /// Transaction type
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Issue store credit
        /// </summary>
        public bool IssueStoreCredit { get; set; }
    }
}
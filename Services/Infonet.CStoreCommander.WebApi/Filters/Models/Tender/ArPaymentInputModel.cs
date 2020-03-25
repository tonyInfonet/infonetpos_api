namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// AR payment input model
    /// </summary>
    public class ArPaymentInputModel
    {
        /// <summary>
        /// Customer code
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }

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
        public byte RegisterNumber { get; set; }

        /// <summary>
        /// Return mode
        /// </summary>
        public bool IsReturnMode { get; set; }
    }
}
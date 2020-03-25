namespace Infonet.CStoreCommander.WebApi.Models.Payment
{
    /// <summary>
    /// Payment model
    /// </summary>
    public class PaymentModel
    {
        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Tender code
        /// </summary>
        public string TenderCode { get; set; }

        /// <summary>
        /// Amount used
        /// </summary>
        public string  AmountUsed { get; set; }

        /// <summary>
        /// Transaction type
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Card number
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Coupon number
        /// </summary>
        public string CouponNumber { get; set; }

        /// <summary>
        /// Till close
        /// </summary>
        public bool TillClose { get; set; }

        /// <summary>
        /// PO number
        /// </summary>
        public string PONumber { get; set; }

    }
}
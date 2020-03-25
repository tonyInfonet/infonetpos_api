namespace Infonet.CStoreCommander.WebApi.Models.Payment
{
    /// <summary>
    /// Payment by sale vendor coupon model
    /// </summary>
    public class PaymentByVCouponModel
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
        /// Tender code
        /// </summary>
        public string TenderCode { get; set; }
    }
}
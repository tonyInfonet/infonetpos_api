
namespace Infonet.CStoreCommander.WebApi.Models.Payment
{
    /// <summary>
    /// Add vendoor coupon input model
    /// </summary>
    public class AddVendorCouponInputModel
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

        /// <summary>
        /// Coupon number
        /// </summary>
        public string CouponNumber { get; set; }

        /// <summary>
        /// Serial number
        /// </summary>
        public string SerialNumber { get; set; }


    }
}
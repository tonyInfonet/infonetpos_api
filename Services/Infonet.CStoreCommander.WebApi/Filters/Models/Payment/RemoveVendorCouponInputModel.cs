

namespace Infonet.CStoreCommander.WebApi.Models.Payment
{
    /// <summary>
    /// Remove vendor coupon input model
    /// </summary>
    public class RemoveVendorCouponInputModel
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

    }
}
using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Payment
{
    /// <summary>
    /// Sale vendor coupon input model
    /// </summary>
    public class SaleVendorCouponInputModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public SaleVendorCouponInputModel()
        {
           Coupons = new List<CouponModel>();
        }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Tender class
        /// </summary>
        public string TenderClass { get; set; }

        /// <summary>
        /// Tender code
        /// </summary>
        public string TenderCode { get; set; }

        /// <summary>
        /// Transaction type
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Coupons
        /// </summary>
        public List<CouponModel> Coupons { get; set; }

    }

    /// <summary>
    /// Coupon model
    /// </summary>
    public class CouponModel
    {
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
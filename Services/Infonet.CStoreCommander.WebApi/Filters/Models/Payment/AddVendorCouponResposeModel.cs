using Infonet.CStoreCommander.BusinessLayer.Entities;
using System.Collections.Generic;


namespace Infonet.CStoreCommander.WebApi.Models.Payment
{
    /// <summary>
    /// Add vendor coupon response model
    /// </summary>
    public class AddVendorCouponResposeModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public AddVendorCouponResposeModel()
        {
            SaleVendorCoupons = new List<VCoupon>();
        }

        /// <summary>
        /// Default coupon
        /// </summary>
        public string DefaultCoupon { get; set; }

        /// <summary>
        /// Sale vendor coupons
        /// </summary>
        public List<VCoupon> SaleVendorCoupons { get; set; }
    }
}
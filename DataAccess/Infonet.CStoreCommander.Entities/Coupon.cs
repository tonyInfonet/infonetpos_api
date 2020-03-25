using System;

namespace Infonet.CStoreCommander.Entities
{
    public class Coupon
    {
        public string CouponId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool Used { get; set; }
        public bool Void { get; set; }
    }
}

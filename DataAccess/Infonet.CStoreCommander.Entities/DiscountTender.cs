using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.Entities
{
    public class DiscountTender
    {
        public int SaleNumber { get; set; }
        public int TillNumber { get; set; }
        public string CardNumber { get; set; }
        public string ClCode { get; set; }
        public string DiscountType { get; set; }
        public float DiscountRate { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string CouponId { get; set; }
    }
}

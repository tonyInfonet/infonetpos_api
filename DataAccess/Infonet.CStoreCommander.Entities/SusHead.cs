using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.Entities
{
   public  class SusHead
    {
        public int SaleNumber{ get; set; }
        public int TillNumber{ get; set; }
        public int Regist{ get; set; }
        public string Store{ get; set; }
        public string User{ get; set; }
        public string Client{ get; set; }
        public string T_TyPe { get; set; }
        public string Reason { get; set; }
        public string ReasonType { get; set; }
        public string DiscType { get; set; }
        public decimal InvcDisc { get; set; }
        public decimal DiscountPercent { get; set; }
        public int  VoidNumber { get; set; }
        public string LoyaltyCard { get; set; }
        public string CouponId { get; set; }
        public string LoyaltyExpiaryDate { get; set; }
        public bool Upsell { get; set; }
    }
}

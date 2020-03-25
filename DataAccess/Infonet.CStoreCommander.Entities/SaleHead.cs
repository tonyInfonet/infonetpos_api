using System;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class SaleHead
    {
        public int SaleNumber { get; set; }

        public int TillNumber { get; set; }

        public int Shift { get; set; }

        public int Register { get; set; }

        public string User { get; set; }
        public string Client { get; set; }
        public DateTime SD { get; set; }
        public DateTime SaleDate { get; set; }
        public DateTime SaleTime { get; set; }
        public DateTime StoreTime { get; set; }
        public DateTime ShiftDate { get; set; }
        public decimal AssociatedAmount { get; set; }
        public decimal LineDiscount { get; set; }
        public decimal INVCDiscount { get; set; }
        public string DiscountType { get; set; }
        public decimal TenderAmount { get; set; }
        public decimal Deposit { get; set; }
        public decimal Payment { get; set; }
        public decimal Change { get; set; }
        public decimal Credits { get; set; }
        public string TType { get; set; }
        public int SaleLine { get; set; }
        public int TendLine { get; set; }
        public string POCode { get; set; }
        public decimal LoyalPoint { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal PayOut { get; set; }
        public int CloseNum { get; set; }
        public string Reason { get; set; }
        public string ReasonType { get; set; }
        public decimal OverPayment { get; set; }
        public bool Upsell { get; set; }
        public string TreatyNumber { get; set; }
        public string LoyaltyCard { get; set; }
        public string RefernceNum { get; set; }
        public string MainTillCloseNum { get; set; }
        public string Store { get; set; }
        public decimal PennyAdjust { get; set; }
        public decimal LoyaltyBalance { get; set; }

        public int VoidNumber { get; set; }





    }
}

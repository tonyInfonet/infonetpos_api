using System;

namespace Infonet.CStoreCommander.Entities
{
    public class CardSale
    {
        public int TillNumber { get; set; }
        public int SaleNumber { get; set; }
        public int LineNumber { get; set; }
        public string CardType { get; set; }
        public string CardNum { get; set; }
        public decimal? CardBalance { get; set; }
        public decimal? PointBalance { get; set; }
        public decimal? SaleAmount { get; set; }
        public decimal? Amount { get; set; }
        public string SaleType { get; set; }
        public string Language { get; set; }
        public string CardUsage { get; set; }
        public bool StoreForward { get; set; }
        public string ExpiryDate { get; set; }
        public string ReferenceNumber { get; set; }
        public string ApprovalCode { get; set; }
        public string SequenceNumber { get; set; }
        public int AllowMulticard { get; set; }
        public bool Swiped { get; set; }
        public string TenderName { get; set; }
        public string CardName { get; set; }
        public string DeclineReason { get; set; }
        public string Result { get; set; }
        public string TerminalID { get; set; }
        public string DebitAccount { get; set; }
        public string ResponseCode { get; set; }
        public string ISOCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime TransactionTime { get; set; }
        public string ReceiptDisplay { get; set; }
        public string CustomerName { get; set; }
        public bool CallTheBank { get; set; }
        public string VechicleNo { get; set; }
        public string DriverNo { get; set; }
        public string IdentificationNo { get; set; }
        public string Odometer { get; set; }
        public bool PrintVechicleNo { get; set; }
        public bool PrintDriverNo { get; set; }
        public bool PrintIdentificationNo { get; set; }
        public bool PrintUsage { get; set; }
        public bool PrintOdometer { get; set; }
        public decimal Balance { get; set; }
        public decimal Quantity { get; set; }
        public string Message { get; set; }
        public string CardProfileID { get; set; }
        public string PONumber { get; set; }
        public int  Sequence { get; set; }
    }
}

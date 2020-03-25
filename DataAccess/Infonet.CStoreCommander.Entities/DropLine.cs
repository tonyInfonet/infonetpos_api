using System;

namespace Infonet.CStoreCommander.Entities
{
    public class DropLine
    {
        public short TillNumber { get; set; }
        public DateTime DropDate { get; set; }
        public string TenderName { get; set; }
        public double ExchangeRate { get; set; }
        public decimal Amount { get; set; }
        public decimal ConvAmount { get; set; }
        public short CloseNum { get; set; }
        public int DropID { get; set; }
    }
}

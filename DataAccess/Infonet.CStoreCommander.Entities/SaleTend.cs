namespace Infonet.CStoreCommander.Entities
{
    public class SaleTend
    {
        public int TillNumber { get; set; }
        public int SaleNumber { get; set; }
        public int SequenceNumber { get; set; }
        public string TenderName { get; set; }
        public string TenderClass { get; set; }
        public decimal? AmountTend { get; set; }
        public decimal? AmountUsed { get; set; }
        public decimal? Exchange { get; set; }
        public string SerialNumber { get; set; }
        public string CCardNumber { get; set; }
        public string CCardAPRV { get; set; }
        public string AuthUser { get; set; }

        //Added new property 
        public string ClassDescription { get; set; }
    }
}

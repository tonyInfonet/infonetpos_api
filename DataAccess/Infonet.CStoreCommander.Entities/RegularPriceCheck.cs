namespace Infonet.CStoreCommander.Entities
{
    public class RegularPriceCheck
    {
        public string StockCode { get; set; }

        public int TillNumber { get; set; }

        public int SaleNumber { get; set; }

        public byte RegisterNumber { get; set; }

        public double RegularPrice { get; set; }

        public string VendorId { get; set; }
    }
}

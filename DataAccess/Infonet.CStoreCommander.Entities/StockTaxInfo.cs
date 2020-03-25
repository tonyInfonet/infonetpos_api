
namespace Infonet.CStoreCommander.Entities
{
   public class StockTaxInfo
    {
        public string TaxName { get; set; }
        public string TaxCode { get; set; }
        public float Rate { get; set; }
        public bool Included { get; set; }
        public float Rebate { get; set; }
    }
}

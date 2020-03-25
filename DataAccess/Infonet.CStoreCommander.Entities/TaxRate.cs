namespace Infonet.CStoreCommander.Entities
{
    public class TaxRate
    {
        public float? Rate { get; set; }
        public bool? Included { get; set; }
        public float? Rebate { get; set; }
        public string TaxName{ get; set; }
        public string TaxCode { get; set; }
        public string TaxDescription { get; set; }
    }
}

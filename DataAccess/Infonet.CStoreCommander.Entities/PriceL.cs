using System;

namespace Infonet.CStoreCommander.Entities
{
    public class PriceL
    {
        public char PriceType { get; set; }
        public char PriceUnit { get; set; }
        public float FQuantity {get;set;}
        public float? TQuantity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public float? Price { get; set; }
        public string Vendor { get; set; }
    }
}

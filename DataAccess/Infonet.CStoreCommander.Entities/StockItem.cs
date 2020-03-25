using System;

namespace Infonet.CStoreCommander.Entities
{
    public class StockItem
    {
        public string StockCode { get; set; }
        public string Description { get; set; }
        public char StockType { get; set; }
        public short? ProductDescription { get; set; }
        public decimal? StandardCost { get; set; }
        public decimal? AverageCost { get; set; }
        public decimal? AverageUnit { get; set; }
        public decimal? LateCost { get; set; }
        public string Department { get; set; }
        public string SubDepartment { get; set; }
        public string SubDetail { get; set; }
        public bool? Serial { get; set; }
        public bool? SByWeight { get; set; }
        public string Vendor { get; set; }
        public char PRType { get; set; }
        public char PRUnit { get; set; }
        public DateTime? PRFrom { get; set; }
        public DateTime? PRTo { get; set; }
        public string VendorNumber { get; set; }
        public bool? Availability { get; set; }
        public string Brand { get; set; }
        public string Characteristic { get; set; }
        public DateTime? CreationDateTime { get; set; }
        public string Family { get; set; }
        public string Format { get; set; }
        public string Generic { get; set; }
        public bool? Label { get; set; }
        public bool? SaleCode { get; set; }
        public bool? ShelfLabel { get; set; }
        public string UM { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string OrderUnit { get; set; }
        public short? Packaging { get; set; }
        public bool? Transfer { get; set; }
        public bool? EligibleLoyalty { get; set; }
        public bool? EligibleFuelRebate { get; set; }
        public decimal? FuelRebate { get; set; }
        public bool? QualtaxRebate { get; set; }
        public bool? EligibletaxRebate { get; set; }
        public bool? EligibleTaxExemption { get; set; }
        //Added new property
        public decimal Price { get; set; }
        public string AlternateCode { get; set; }
    }
}

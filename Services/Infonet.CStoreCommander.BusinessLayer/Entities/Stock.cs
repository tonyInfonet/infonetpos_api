
using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class Stock
    {
        public string PLUPrim { get; set; }

        public char PLUType { get; set; }

        public bool ProductIsFuel { get; set; }

        public bool ProductIsPropane { get; set; }

        public bool ActiveStock { get; set; }

        public int AvailableItems { get; set; }

        public bool CheckGroupButton { get; set; }

        public decimal Rebate { get; set; }

        public short TECategory { get; set; }

        public string TEVendor { get; set; }

        public double? Price { get; set; }

        public char PriceLType { get; set; } 

        public char PriceLUnit { get; set; }

        public string StockCode { get; set; }

        public string Description { get; set; }

        public char StockType { get; set; }

        public char PRType { get; set; }

        public char PRUnit { get; set; }

        public int LoyaltySave { get; set; }

        public string Vendor { get; set; }

        public short ProductDescription { get; set; }

        public bool SByWeight { get; set; }

        public string UM { get; set; }

        public double StandardCost { get; set; }

        public bool EligibleLoyalty { get; set; }

        public bool EligibleFuelRebate { get; set; }

        public decimal FuelRebate { get; set; }

        public bool EligibletaxRebate { get; set; }

        public bool EligibleTaxExemption { get; set; }

        public bool AvailableByDay { get; set; }

        public string Department { get; set; }

        public string SubDepartment { get; set; }

        public string SubDetail { get; set; }

        public double AverageCost { get; set; }

        public Charges Charges { get; set; }
        
        public Line_Taxes LineTaxes { get; set; }
        
        public Line_Kits LineKits { get; set; } 

        public List<PriceL> PriceL { get; set;}

        public List<Promo> PromoByStockCode { get; set; }

        public List<Promo> PromoByDept { get; set; }

        public List<Promo> PromoBySubDept { get; set; }

        public List<Promo> PromoBySubDetail { get; set; }

        //public decimal? AverageUnit { get; set; }
        //public decimal? LateCost { get; set; }
       
        //public bool? Serial { get; set; }
    
        //public DateTime? PRFrom { get; set; }
        //public DateTime? PRTo { get; set; }
        //public string VendorNumber { get; set; }
        //public bool? Availability { get; set; }
        //public string Brand { get; set; }
        //public string Characteristic { get; set; }
        //public DateTime? CreationDateTime { get; set; }
        //public string Family { get; set; }
        //public string Format { get; set; }
        //public string Generic { get; set; }
        //public bool? Label { get; set; }
        //public bool? SaleCode { get; set; }
        //public bool? ShelfLabel { get; set; }
        //public DateTime? UpdateDate { get; set; }
        //public string OrderUnit { get; set; }
        //public short? Packaging { get; set; }
        //public bool? Transfer { get; set; }
       
        public bool QualtaxRebate { get; set; }
       
        ////Added new property
        //public decimal Price { get; set; }
        //public string AlternateCode { get; set; }
    }
}

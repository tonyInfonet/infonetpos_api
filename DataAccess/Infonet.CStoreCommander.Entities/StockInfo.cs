
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class StockInfo
    {
        public string PLUPrim { get; set; }

        public string PluCode { get; set; }

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

        public bool QualtaxRebate { get; set; }

        public DateTime PRFrom { get; set; }

        public DateTime PRTo { get; set; }

        public string VendorNumber { get; set; }

        public bool Availability { get; set; }

    }
}

using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class StockPriceCheck
    {
        public string StockCode { get; set; }

        public string Description { get; set; }

        public string VendorId { get; set; }

        public List<string> SpecialPriceTypes { get; set; }

        public string RegularPriceText { get; set; }

        public string PriceTypeText { get; set; }

        public bool IsPriceVisible { get; set; }

        public bool IsAvQtyVisible { get; set; }

        public string AvailableQuantity { get; set; }

        public bool IsTaxExemptVisible { get; set; }

        public string TaxExemptPrice { get; set; }

        public string TaxExemptAvailable { get; set; }

        public bool IsSpecialPricingVisible { get; set; }

        public string FromDate { get; set; }

        public bool IsToDateVisible { get; set; }

        public string ToDate { get; set; }

        public bool IsEndDateChecked { get; set; }

        public bool IsActiveVendorPrice { get; set; }

        public bool IsPerDollarChecked { get; set; }

        public bool IsPerPercentageChecked { get; set; }

        public bool IsAddButtonVisible { get; set; }

        public bool IsRemoveButtonVisible { get; set; }

        public bool IsChangePriceEnable { get; set; }

        public SalePriceType SalePrice { get; set; }

        public FirstUnitPriceType FirstUnitPrice { get; set; }

        public IncrementalPriceType IncrementalPrice { get; set; }

        public XForPriceType XForPrice { get; set; }

        public CustomerDisplay CustomerDisplay { get; set; }

        public string Message { get; set; }

    }

    public class SalePriceType
    {
        public int Columns { get; set; }

        public string ColumnText { get; set; }

        public List<PriceGrid> SalePrices { get; set; }
    }

    public class FirstUnitPriceType
    {
        public int Columns { get; set; }

        public string ColumnText { get; set; }

        public string ColumnText2 { get; set; }

        public List<PriceGrid> FirstUnitPriceGrids { get; set; }
    }


    public class IncrementalPriceType
    {
        public int Columns { get; set; }

        public string ColumnText { get; set; }

        public string ColumnText2 { get; set; }

        public string ColumnText3 { get; set; }

        public List<PriceGrid> IncrementalPriceGrids { get; set; }
    }

    public class XForPriceType
    {
        public int Columns { get; set; }

        public string ColumnText { get; set; }

        public string ColumnText2 { get; set; }

        public List<PriceGrid> XForPriceGrids { get; set; }
    }

    public class PriceGrid
    {
        public string Column1 { get; set; }

        public string Column2 { get; set; }

        public string Column3 { get; set; }
    }

}

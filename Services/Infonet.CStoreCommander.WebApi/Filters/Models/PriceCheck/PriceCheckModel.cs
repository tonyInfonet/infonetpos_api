using System.Collections.Generic;
using NLog;

namespace Infonet.CStoreCommander.WebApi.Models.PriceCheck
{
    /// <summary>
    /// Price Check Model
    /// </summary>
    public class PriceCheckModel
    {
        /// <summary>
        /// Stock Code
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// Stock Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Vendor Id
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// Is Active Vendor Price
        /// </summary>
        public bool IsActiveVendorPrice { get; set; }


        /// <summary>
        /// Is Change Price Enables
        /// </summary>
        public bool IsChangePriceEnable { get; set; }

        /// <summary>
        /// Special Price Types
        /// </summary>
        public List<string> SpecialPriceTypes { get; set; }

        /// <summary>
        /// Is Price Visible
        /// </summary>
        public bool IsPriceVisible { get; set; }

        /// <summary>
        /// Regular Price Text
        /// </summary>
        public string RegularPriceText { get; set; }
        
        /// <summary>
        /// Price Type Text
        /// </summary>
        public string PriceTypeText { get; set; }

        /// <summary>
        /// Is Available Quantity Visible
        /// </summary>
        public bool IsAvQtyVisible { get; set; }

        /// <summary>
        /// Available Quantity
        /// </summary>
        public string AvailableQuantity { get; set; }

        /// <summary>
        /// Is Tax Exempt Visible
        /// </summary>
        public bool IsTaxExemptVisible { get; set; }

        /// <summary>
        /// Tax Exempt Price
        /// </summary>
        public string TaxExemptPrice { get; set; }

        /// <summary>
        /// Tax Exempt Available
        /// </summary>
        public string TaxExemptAvailable { get; set; }

        /// <summary>
        /// Is Special Pricing visible
        /// </summary>
        public bool IsSpecialPricingVisible { get; set; }

        /// <summary>
        /// From Date
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        ///  IS To date Visible
        /// </summary>
        public bool IsToDateVisible { get; set; }

        /// <summary>
        /// To Date
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Is End date Checked
        /// </summary>
        public bool IsEndDateChecked { get; set; }

        /// <summary>
        /// Is Per dollar checked
        /// </summary>
        public bool IsPerDollarChecked { get; set; }

        /// <summary>
        /// Is Per percentage checked
        /// </summary>
        public bool IsPerPercentageChecked { get; set; }

        /// <summary>
        /// Is Add button Visible
        /// </summary>
        public bool IsAddButtonVisible { get; set; }

        /// <summary>
        /// Is Remove Button Visible
        /// </summary>
        public bool IsRemoveButtonVisible { get; set; }

        /// <summary>
        /// Sale Price
        /// </summary>
        public SalePriceTypeModel SalePrice { get; set; }

        /// <summary>
        /// First Unit Price
        /// </summary>
        public FirstUnitPriceTypeModel FirstUnitPrice { get; set; }

        /// <summary>
        /// Incremental Price
        /// </summary>
        public IncrementalPriceTypeModel IncrementalPrice { get; set; }

        /// <summary>
        /// X For Price
        /// </summary>
        public XForPriceTypeModel XForPrice { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

    }
}
namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Tax Exempt Sale Model
    /// </summary>
    public class TaxExemptSaleModel
    {
        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Product Name
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// Quantity
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        /// Regular Price
        /// </summary>
        public string RegularPrice { get; set; }

        /// <summary>
        /// Tax Free Price
        /// </summary>
        public string TaxFreePrice { get; set; }

        /// <summary>
        /// Exempted Tax
        /// </summary>
        public string ExemptedTax { get; set; }

        /// <summary>
        /// Quota Used
        /// </summary>
        public string QuotaUsed { get; set; }

        /// <summary>
        /// Quota Limit
        /// </summary>
        public string QuotaLimit { get; set; }
    }
}
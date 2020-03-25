using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Stock
{
    /// <summary>
    /// Stock model
    /// </summary>
    public class StockModel
    {
        /// <summary>
        /// Stock code
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Regular price
        /// </summary>
        public decimal RegularPrice { get; set; } 

        /// <summary>
        /// Tax codes
        /// </summary>
        public List<string> TaxCodes { get; set; }
    }
}
using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.PriceCheck
{
    /// <summary>
    /// Sale Price Type Model
    /// </summary>
    public class SalePriceTypeModel
    {
        /// <summary>
        /// Columns
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// Column Text
        /// </summary>
        public string ColumnText { get; set; }


        /// <summary>
        /// Sale Prices
        /// </summary>
        public List<PriceGrid> SalePrices { get; set; }
    }
}
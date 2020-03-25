using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.PriceCheck
{
    /// <summary>
    /// XFOR Price Model
    /// </summary>
    public class XForPriceTypeModel
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
        /// Column Text
        /// </summary>
        public string ColumnText2 { get; set; }

        /// <summary>
        /// X For Prices
        /// </summary>
        public List<PriceGrid> XForPriceGrids { get; set; }
    }
}
using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.PriceCheck
{
    /// <summary>
    /// First Unit Price Model
    /// </summary>
    public class FirstUnitPriceTypeModel
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
        /// Grids
        /// </summary>
        public List<PriceGrid> FirstUnitPriceGrids { get; set; }

    }
}
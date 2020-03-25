﻿using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.PriceCheck
{
    /// <summary>
    /// Model
    /// </summary>
    public class IncrementalPriceTypeModel
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
        /// Column Text2
        /// </summary>
        public string ColumnText2 { get; set; }

        /// <summary>
        /// Column Text2
        /// </summary>
        public string ColumnText3 { get; set; }

        /// <summary>
        /// Grids
        /// </summary>
        public List<PriceGrid> IncrementalPriceGrids { get; set; }

    }
}
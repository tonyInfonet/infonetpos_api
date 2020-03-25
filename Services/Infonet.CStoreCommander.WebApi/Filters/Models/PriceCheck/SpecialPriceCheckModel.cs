using System;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.PriceCheck
{
    /// <summary>
    /// Special Price Check Model
    /// </summary>
    public class SpecialPriceCheckModel
    {
        /// <summary>
        /// Stock Code
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// TillNumber
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Sale Number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// RegisterNumber
        /// </summary>
        public byte RegisterNumber { get; set; }

        /// <summary>
        /// Regular Price
        /// </summary>
        public double RegularPrice { get; set; }


        /// <summary>
        /// Price Type
        /// </summary>
        public string PriceType { get; set; }

        /// <summary>
        /// Grid Prices
        /// </summary>
        public List<PriceGrid> GridPrices { get; set; }

        /// <summary>
        /// From date
        /// </summary>
        public DateTime Fromdate { get; set; }

        /// <summary>
        /// To Date
        /// </summary>
        public DateTime Todate { get; set; }

        /// <summary>
        /// Per dollar checked
        /// </summary>
        public bool PerDollarChecked { get; set; }

        /// <summary>
        /// Is EndDate
        /// </summary>
        public bool IsEndDate { get; set; }
    }
}
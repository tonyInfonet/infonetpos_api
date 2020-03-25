using System;

namespace Infonet.CStoreCommander.WebApi.Models.ReturnSale
{
    /// <summary>
    /// Sale head model
    /// </summary>
    public class SaleHeadModel
    {
        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Time
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Total amount
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Allow correction
        /// </summary>
        public bool AllowCorrection { get; set; }

        /// <summary>
        /// Allow reason
        /// </summary>
        public bool AllowReason { get; set; }
    }
}
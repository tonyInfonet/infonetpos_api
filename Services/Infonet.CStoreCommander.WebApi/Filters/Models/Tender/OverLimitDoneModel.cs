using System;

namespace Infonet.CStoreCommander.WebApi.Models
{
    /// <summary>
    /// OverLimit Done Model
    /// </summary>
    public class OverLimitDoneModel
    {
        /// <summary>
        /// Till Number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Sale Number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Reason
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Explanation
        /// </summary>
        public string Explanation { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; set; }
    }
}
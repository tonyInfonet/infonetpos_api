using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Bottle
{
    /// <summary>
    /// Return bottle model
    /// </summary>
    public class BottleReturnInputModel
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
        /// Register number
        /// </summary>
        public byte RegisterNumber { get; set; }

        /// <summary>
        /// Bottles
        /// </summary>
        public List<BottleReturn> Bottles { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }

    }
}
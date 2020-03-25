using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infonet.CStoreCommander.WebApi.Models.GasKing
{
    /// <summary>
    /// GasKing Kickback card linked
    /// </summary>
    public class GasKingKickbackResponseModel
    {
        /// <summary>
        /// GasKing card is linked or not
        /// </summary>
        public bool IsKickBackLinked{get;set;}

        /// <summary>
        /// Kickback points
        /// </summary>
        public double PointsReedemed { get; set; }

        /// <summary>
        /// Kickback value
        /// </summary>
        public double Value { get; set; }
    }
}
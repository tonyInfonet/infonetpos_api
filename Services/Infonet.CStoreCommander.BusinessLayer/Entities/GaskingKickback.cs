using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{

    /// <summary>
    /// GasKing Kickback card linked
    /// </summary>
    public class GaskingKickback
    {
        /// <summary>
        /// GasKing card is linked or not
        /// </summary>
        public bool IsKickBackLinked { get; set; }

        /// <summary>
        /// Kickback points
        /// </summary>
        public double PointsReedemed { get; set; }

        /// <summary>
        /// Kickback value
        /// </summary>
        public string Value { get; set; }
    }
}

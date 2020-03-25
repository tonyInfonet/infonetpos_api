using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    /// <summary>
    /// Save group price model
    /// </summary>
    public class SaveGroupPriceInputModel
    {
        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }
        public bool IsReadTotalizerChecked { get; set; }
        public bool IsPricesToDisplayChecked { get; set; }
        public bool IsReadTankDipChecked { get; set; }

        /// <summary>
        /// Group prices
        /// </summary>
        public List<GroupFuelPriceModel> GroupFuelPrices { get; set; }
    }
}
using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    public class BasePricesSaveModel
    {/// <summary>
     /// Fuel prices
     /// </summary>
        public List<FuelPriceModel> FuelPrices { get; set; }

        public bool IsReadTotalizerChecked { get; set; }
        public bool IsPricesToDisplayChecked { get; set; }
        public bool IsReadTankDipChecked { get; set; }

        public int TillNumber { get; set; }
    }
}
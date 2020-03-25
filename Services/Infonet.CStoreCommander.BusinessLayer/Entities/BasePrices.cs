using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class BasePrices
    {
        public List<FuelPrice> FuelPrices { get; set; } = new List<FuelPrice>();

        public bool IsTaxExemptionVisible { get; set; }
        public bool IsReadTotalizerEnabled { get; set; } = true;
        public bool IsReadTotalizerChecked { get; set; } = true;
        public bool IsPricesToDisplayEnabled { get; set; } = true;
        public bool IsPricesToDisplayChecked { get; set; }
        public bool IsReadTankDipEnabled { get; set; } = true;
        public bool IsReadTankDipChecked { get; set; } = true;
        public bool CanReadTotalizer { get; set; } = true;
        public bool CanSelectPricesToDisplay { get; set; }
        public bool IsExitEnabled { get; set; } = true;
        public bool IsErrorEnabled { get; set; } = true;
        public string Caption { get; set; }
        public bool IsCashPriceEnabled { get; set; } = true;
        public bool IsCreditPriceEnabled { get; set; } = true;
        public bool IsTaxExemptedCashPriceEnabled { get; set; } = true;
        public bool IsTaxExemptedCreditPriceEnabled { get; set; } = true;
        public bool IsIncrementEnabled { get; set; }
    }
}

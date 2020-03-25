using Infonet.CStoreCommander.WebApi.Models.Report;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    /// <summary>
    /// Base price model
    /// </summary>
    public class BasePricesModel
    {
        /// <summary>
        /// Fuel prices
        /// </summary>
        public List<FuelPriceModel> FuelPrices { get; set; }

        public ReportModel Report { get; set; }

        /// <summary>
        /// Is tax exempt visible
        /// </summary>
        public bool IsTaxExemptionVisible { get; set; }
        public bool IsReadTotalizerEnabled { get; set; }
        public bool IsReadTotalizerChecked { get; set; }
        public bool IsPricesToDisplayEnabled { get; set; }
        public bool IsPricesToDisplayChecked { get; set; }
        public bool IsReadTankDipEnabled { get; set; }
        public bool IsReadTankDipChecked { get; set; }
        public bool CanReadTotalizer { get; set; }
        public bool CanSelectPricesToDisplay { get; set; }
        public bool IsExitEnabled { get; set; }
        public bool IsErrorEnabled { get; set; }
        public string Caption { get; set; }
        public bool IsCashPriceEnabled { get; set; }
        public bool IsCreditPriceEnabled { get; set; }
        public bool IsTaxExemptedCashPriceEnabled { get; set; }
        public bool IsTaxExemptedCreditPriceEnabled { get; set; }
    }
}
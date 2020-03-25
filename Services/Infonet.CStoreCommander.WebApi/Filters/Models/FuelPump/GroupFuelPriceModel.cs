namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    /// <summary>
    /// Fuel price model
    /// </summary>
    public class GroupFuelPriceModel
    {
        /// <summary>
        /// Row
        /// </summary>
        public short Row { get; set; }

        /// <summary>
        /// Grade
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// Cash price
        /// </summary>
        public string CashPrice { get; set; }

        /// <summary>
        /// Credit price
        /// </summary>
        public string CreditPrice { get; set; }

        /// <summary>
        /// Tax exempted cash price
        /// </summary>
        public string TaxExemptedCashPrice { get; set; }

        /// <summary>
        /// Tax exempted credit price
        /// </summary>
        public string TaxExemptedCreditPrice { get; set; }
    }
}
namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    /// <summary>
    /// Manual fuel model
    /// </summary>
    public class ManualFuelModel
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
        /// Pump Id
        /// </summary>
        public short PumpId { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public float Amount { get; set; }
        
        /// <summary>
        /// Is cash selected
        /// </summary>
        public bool IsCashSelected { get; set; }

        /// <summary>
        /// Grade
        /// </summary>
        public string Grade { get; set; }
    }
}
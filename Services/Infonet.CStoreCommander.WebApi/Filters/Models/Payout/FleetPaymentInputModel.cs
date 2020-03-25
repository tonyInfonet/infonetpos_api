namespace Infonet.CStoreCommander.WebApi.Models.Payout
{
    /// <summary>
    /// Fleet payment input model
    /// </summary>
    public class FleetPaymentInputModel
    {
        /// <summary>
        /// Card number
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Is swiped or not
        /// </summary>
        public bool IsSwiped { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }
    }
}
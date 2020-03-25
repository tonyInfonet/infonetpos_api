 namespace Infonet.CStoreCommander.WebApi.Models.Till
{
    /// <summary>
    /// Update till close
    /// </summary>
    public class UpdateTillCloseInputModel
    {
        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Updated tender
        /// </summary>
        public UpdateTender UpdatedTender { get; set; }

        /// <summary>
        /// Updated bill coin
        /// </summary>
        public UpdateBillCoin UpdatedBillCoin { get; set; }
    }

    /// <summary>
    /// Tender
    /// </summary>
    public class UpdateTender
    {
        /// <summary>
        /// Tender name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Amount entered
        /// </summary>
        public decimal Entered { get; set; }
    }

    /// <summary>
    /// Bill coin
    /// </summary>
    public class UpdateBillCoin
    {
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }
    }
}
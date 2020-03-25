namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Store credit input model
    /// </summary>
    public class StoreCreditInput
    {
        /// <summary>
        /// Number
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal AmountEntered { get; set; }
    }
}
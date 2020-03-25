namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Gift certificate input model
    /// </summary>
    public class GiftCertInput
    {
        /// <summary>
        /// Gift certificate number
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }
    }
}
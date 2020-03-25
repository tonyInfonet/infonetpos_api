namespace Infonet.CStoreCommander.WebApi.Models.Customer
{

    /// <summary>
    /// Search by card model
    /// </summary>
    public class SearchBycardModel
    {
        /// <summary>
        /// Card number
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Is loyalty card
        /// </summary>
        public bool IsLoyaltycard { get; set; }

        public int SaleNumber { get; set; }

        public int TillNumber { get; set; }
    }
}
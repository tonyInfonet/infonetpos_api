namespace Infonet.CStoreCommander.WebApi.Models.Customer
{
    /// <summary>
    /// Loyalty customer model
    /// </summary>
    public class LoyaltyCustomerResponseModel
    {

        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Loyalty number
        /// </summary>
        public  string LoyaltyNumber { get; set; }

        /// <summary>
        /// Loaylty points
        /// </summary>
        public  string LoyaltyPoints { get; set; }
    }
}
namespace Infonet.CStoreCommander.WebApi.Models.Customer
{
    /// <summary>
    /// AR customer model
    /// </summary>
    public class ArCustomerModel
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
        /// Balance
        /// </summary>
        public string Balance { get; set; }

        /// <summary>
        /// Credit limit
        /// </summary>
        public string CreditLimit { get; set; }
    }
}
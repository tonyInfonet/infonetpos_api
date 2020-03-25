namespace Infonet.CStoreCommander.WebApi.Models.Sale
{
    /// <summary>
    /// New sale
    /// </summary>
    public class NewSale
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
        /// Customer
        /// </summary>
        public string Customer { get; set; }
    }
}
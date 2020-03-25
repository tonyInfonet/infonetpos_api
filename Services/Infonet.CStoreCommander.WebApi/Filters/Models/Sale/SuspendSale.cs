namespace Infonet.CStoreCommander.WebApi.Models.Sale
{
    /// <summary>
    /// Suspend sale
    /// </summary>
    public class SuspendSale
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
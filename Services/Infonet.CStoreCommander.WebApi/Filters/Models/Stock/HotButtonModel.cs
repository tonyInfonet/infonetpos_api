namespace Infonet.CStoreCommander.WebApi.Models.Stock
{
    /// <summary>
    /// Hot button model
    /// </summary>
    public class HotButtonModel
    {
        /// <summary>
        /// Button Id
        /// </summary>
        public int ButtonId { get; set; }

        /// <summary>
        /// Stock code
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Default quantity
        /// </summary>
        public int DefaultQuantity { get; set; }

        /// <summary>
        /// Image url
        /// </summary>
        public string ImageUrl { get; set; }
    }
}
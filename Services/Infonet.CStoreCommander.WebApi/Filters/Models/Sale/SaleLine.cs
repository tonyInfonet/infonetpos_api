namespace Infonet.CStoreCommander.WebApi.Models.Sale
{
    /// <summary>
    /// Sale line model
    /// </summary>
    public class SaleLine
    {
        /// <summary>
        /// Line number
        /// </summary>
        public short LineNumber { get; set; }

        /// <summary>
        /// Stock code
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Quantity
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// Discount rate
        /// </summary>
        public string DiscountRate { get; set; }

        /// <summary>
        /// Dsicount type
        /// </summary>
        public string DiscountType { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// Allow price change
        /// </summary>
        public bool AllowPriceChange { get; set; }

        /// <summary>
        /// Allow quantity change
        /// </summary>
        public bool AllowQuantityChange { get; set; }

        /// <summary>
        /// Allow discount change
        /// </summary>
        public bool AllowDiscountChange { get; set; }

        /// <summary>
        /// Allow discount reason
        /// </summary>
        public bool AllowDiscountReason { get; set; }

        /// <summary>
        /// Allow price reason
        /// </summary>
        public bool AllowPriceReason { get; set; }

        /// <summary>
        /// Allow return reason
        /// </summary>
        public bool AllowReturnReason { get; set; }

        /// <summary>
        /// Confirm delete
        /// </summary>
        public bool ConfirmDelete { get; set; }
        public string Dept { get; set; }
    }
}
namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Purchase item response model
    /// </summary>
    public class PurchaseItemResponseModel
    {
        /// <summary>
        /// Product type Id
        /// </summary>
        public int ProductTypeId { get; set; }

        /// <summary>
        /// Product Id
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Quantity
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// Equivalent quantity
        /// </summary>
        public string EquivalentQuantity { get; set; }

        /// <summary>
        /// Quota used
        /// </summary>
        public string QuotaUsed { get; set; }

        /// <summary>
        /// Quota limit
        /// </summary>
        public string QuotaLimit { get; set; }

        /// <summary>
        /// Display quota
        /// </summary>
        public string DisplayQuota { get; set; }

        /// <summary>
        /// Fuel Over limit text
        /// </summary>
        public string FuelOverLimitText { get; set; }
    }
}
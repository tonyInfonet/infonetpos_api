namespace Infonet.CStoreCommander.WebApi.Models.Bottle
{
    /// <summary>
    /// Bottle return model
    /// </summary>
    public class BottleReturnModel
    {
        /// <summary>
        /// Description
        /// </summary>
        public  string Description { get; set; }

        /// <summary>
        /// Image url
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Product
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public  float Price { get; set; }

        /// <summary>
        /// Default Quantity
        /// </summary>
        public float DefaultQuantity { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }
    }
}
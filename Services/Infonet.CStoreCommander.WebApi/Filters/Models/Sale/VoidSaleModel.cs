namespace Infonet.CStoreCommander.WebApi.Models.Sale
{
    /// <summary>
    /// VoidSaleModel
    /// </summary>
    public class VoidSaleModel
    {
        /// <summary>
        /// Sale Number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till Number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Void Reason
        /// </summary>
        public string VoidReason { get; set; }
    }
}
namespace Infonet.CStoreCommander.WebApi.Models.ReturnSale
{
    /// <summary>
    /// Return sale model
    /// </summary>
    public class ReturnSaleModel
    {
        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Sale till number
        /// </summary>
        public int SaleTillNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Correction sale
        /// </summary>
        public bool IsCorrection { get; set; }

        /// <summary>
        /// Sale lines
        /// </summary>
        public int[] SaleLines { get; set; }

        /// <summary>
        /// Reason type
        /// </summary>
        public string ReasonType { get; set; }

        /// <summary>
        /// Reason code
        /// </summary>
        public string ReasonCode { get; set; }
    }
}
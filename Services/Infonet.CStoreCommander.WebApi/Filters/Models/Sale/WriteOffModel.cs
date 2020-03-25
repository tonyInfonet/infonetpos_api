namespace Infonet.CStoreCommander.WebApi.Models.Sale
{
    /// <summary>
    /// WriteOffModel
    /// </summary>
    public class WriteOffModel
    {
        /// <summary>
        /// SaleNumber
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// TillNumber
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// WriteOffReason
        /// </summary>
        public string WriteOffReason { get; set; }
    }
}
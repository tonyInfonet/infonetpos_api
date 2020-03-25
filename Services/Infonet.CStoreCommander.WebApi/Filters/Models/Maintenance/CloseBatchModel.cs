namespace Infonet.CStoreCommander.WebApi.Models.Maintenance
{
    /// <summary>
    /// Close Batch Model
    /// </summary>
    public class CloseBatchModel
    {
        /// <summary>
        /// POS ID
        /// </summary>
        public byte PosId { get; set; }

        /// <summary>
        /// Till Number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Sale Number
        /// </summary>
        public int SaleNumber { get; set; }
        
        /// <summary>
        /// Register Number
        /// </summary>
        public byte RegisterNumber { get; set; }

    }
}
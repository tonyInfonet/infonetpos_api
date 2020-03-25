namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Override Limit Done Model
    /// </summary>
    public class OverrideLimitDoneModel
    {
        /// <summary>
        /// Till Number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Sale Number
        /// </summary>
        public int SaleNumber { get; set; }


        /// <summary>
        /// Purchase Item Number
        /// </summary>
        public short ItemNumber { get; set; }

        /// <summary>
        /// Reason
        /// </summary>
        public string OverrideCode { get; set; }

        /// <summary>
        /// Explanation
        /// </summary>
        public string DocumentNumber { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public string DocumentDetail { get; set; }

    }
}
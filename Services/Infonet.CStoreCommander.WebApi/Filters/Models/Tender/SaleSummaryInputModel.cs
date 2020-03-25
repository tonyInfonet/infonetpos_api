namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Sale Summary Input Model
    /// </summary>
    public class SaleSummaryInputModel
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
        /// Register Number
        /// </summary>
        public byte RegisterNumber { get; set; }


        /// <summary>
        /// Checks Whether SITE is Validated
        /// </summary>
        public bool IsSiteValidated { get; set; }


        /// <summary>
        /// Checks Whether AITE is Validated
        /// </summary>
        public bool IsAiteValidated { get; set; }

    }
}
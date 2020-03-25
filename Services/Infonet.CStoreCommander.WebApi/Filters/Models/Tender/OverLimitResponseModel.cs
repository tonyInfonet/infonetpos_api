using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// OverLimit Response Model
    /// </summary>
    public class OverLimitResponseModel
    {
        /// <summary>
        /// Is Gas Reasons
        /// </summary>
        public bool IsGasReasons { get; set; }

        /// <summary>
        /// Is Tobacco Reasons
        /// </summary>
        public bool IsTobaccoReasons { get; set; }

        /// <summary>
        /// Is Propane Reasons
        /// </summary>
        public bool IsPropaneReasons { get; set; }

        /// <summary>
        /// Gas Reasons
        /// </summary>
        public List<TaxExemptReasonModel> GasReasons { get; set; }

        /// <summary>
        /// Tobacco Reasons
        /// </summary>
        public List<TaxExemptReasonModel> TobaccoReasons { get; set; }

        /// <summary>
        /// Propane Reasons
        /// </summary>
        public List<TaxExemptReasonModel> PropaneReasons { get; set; }

        /// <summary>
        /// Tax Exempt Sale Model
        /// </summary>
        public List<TaxExemptSaleModel> TaxExemptSale { get; set; }
    }
}
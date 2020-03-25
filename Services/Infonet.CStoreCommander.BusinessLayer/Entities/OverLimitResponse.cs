using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class OverLimitResponse
    {
        public bool IsGasReasons { get; set; }

        public bool IsTobaccoReasons { get; set; }

        public bool IsPropaneReasons { get; set; }

        public List<TaxExemptReasonResponse> GasReasons { get; set; }

        public List<TaxExemptReasonResponse> TobaccoReasons { get; set; }

        public List<TaxExemptReasonResponse> PropaneReasons { get; set; }

        public List<TaxExemptSaleResponse> TaxExemptSale { get; set; }
    }


    public class TaxExemptReasonResponse
    {
        public string Reason { get; set; }

        public short ExplanationCode { get; set; }
    }


    /// <summary>
    /// Tax Exempt Sale Model
    /// </summary>
    public class TaxExemptSaleResponse
    {
        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Product Name
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// Quantity
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        /// Regular Price
        /// </summary>
        public string RegularPrice { get; set; }

        /// <summary>
        /// Tax Free Price
        /// </summary>
        public string TaxFreePrice { get; set; }

        /// <summary>
        /// Exempted Tax
        /// </summary>
        public string ExemptedTax { get; set; }

        /// <summary>
        /// Quota Used
        /// </summary>
        public string QuotaUsed { get; set; }

        /// <summary>
        /// Quota Limit
        /// </summary>
        public string QuotaLimit { get; set; }
    }
}

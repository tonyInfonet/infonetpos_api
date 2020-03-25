using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// AITE Card Response Model
    /// </summary>
    public class AiteCardResponseModel
    {
        /// <summary>
        /// AITE Card Number
        /// </summary>
        public string AiteCardNumber { get; set; }

        /// <summary>
        /// AITE Card Holder Name
        /// </summary>
        public string AiteCardHolderName { get; set; }

        /// <summary>
        /// Bar Code
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// Is OverLimit form To be opened
        /// </summary>
        public bool IsFrmOverLimit { get; set; }

        /// <summary>
        /// Sale Summary Model
        /// </summary>
        public List<NameValuePair> SaleSummary { get; set; }

        /// <summary>
        /// Tenders
        /// </summary>
        public TenderSummaryModel TenderSummary { get; set; }

    }
}
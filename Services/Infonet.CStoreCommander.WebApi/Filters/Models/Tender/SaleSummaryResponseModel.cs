using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Sale Summary response Model
    /// </summary>
    public class SaleSummaryResponseModel
    {
        /// <summary>
        /// Sale Summary
        /// </summary>
        public List<NameValuePair> SaleSummary { get; set; }

        /// <summary>
        /// Tenders
        /// </summary>
        public TenderSummaryModel TenderSummary { get; set; }
    }
}
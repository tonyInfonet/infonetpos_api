using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Qite card Response Model
    /// </summary>
    public class QitecardResponseModel
    {
        /// <summary>
        /// AITE Card Number
        /// </summary>
        public string BandMember { get; set; }

        /// <summary>
        /// BandMemberName
        /// </summary>
        public string BandMemberName { get; set; }

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
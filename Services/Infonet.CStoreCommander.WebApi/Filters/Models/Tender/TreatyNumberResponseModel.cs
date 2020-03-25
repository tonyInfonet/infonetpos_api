using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Treaty Number Response Model
    /// </summary>
    public class TreatyNumberResponseModel
    {
        /// <summary>
        /// Treaty Number
        /// </summary>
        public string TreatyNumber { get; set; }

        /// <summary>
        /// Treaty Name (Treaty Customer Name)
        /// </summary>
        public string TreatyCustomerName { get; set; }

        /// <summary>
        /// Permit Number
        /// </summary>
        public string PermitNumber { get; set; }

        /// <summary>
        /// IS Frm Override Limit
        /// </summary>
        public bool IsFrmOverrideLimit { get; set; }

        /// <summary>
        /// Is FNGTR
        /// </summary>
        public bool IsFngtr { get; set; }

        /// <summary>
        /// FNGTR Message
        /// </summary>
        public string FngtrMessage { get; set; }

        /// <summary>
        /// Sale Summary
        /// </summary>
        public List<NameValuePair> SaleSummary { get; set; }

        /// <summary>
        /// Tenders
        /// </summary>
        public TenderSummaryModel TenderSummary { get; set; }

        /// <summary>
        /// Requires Signature
        /// </summary>
        public bool RequireSignature { get; set; }
    }

    /// <summary>
    /// Tender Response Model
    /// </summary>
    public class TenderResponseModel
    {
        /// <summary>
        /// Tender Code
        /// </summary>
        public string TenderCode { get; set; }

        /// <summary>
        /// Tender name
        /// </summary>
        public string TenderName { get; set; }

        /// <summary>
        /// Tender Class
        /// </summary>
        public string TenderClass { get; set; }

        /// <summary>
        /// Amount Entered
        /// </summary>
        public string AmountEntered { get; set; }

        /// <summary>
        /// Amount Value
        /// </summary>
        public string AmountValue { get; set; }

        /// <summary>
        /// Text field enabled
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Maximum value
        /// </summary>
        public double MaximumValue { get; set; }

        /// <summary>
        /// MinimumValue
        /// </summary>
        public double MinimumValue { get; set; }

        /// <summary>
        /// Image
        /// </summary>
        public string Image { get; set; }
    }

    /// <summary>
    /// NameValue Pair
    /// </summary>
    public class NameValuePair
    {
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }
    }
}
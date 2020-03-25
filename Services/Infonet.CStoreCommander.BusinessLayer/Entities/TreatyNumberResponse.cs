using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class TreatyNumberResponse
    {
        public string TreatyNumber { get; set; }

        public string TreatyCustomerName { get; set; }

        public string PermitNumber { get; set; }

        public Dictionary<string, string> SaleSummary { get; set; }

        public Tenders Tenders { get; set; }

        public bool IsFrmOverrideLimit { get; set; }

        public bool IsFngtr { get; set; }

        public string FngtrMessage { get; set; }
    }

    public class TenderResponse
    {
        public string TenderCode { get; set; }
        public string TenderName { get; set; }
        public string TenderClass { get; set; }
        public string AmountEntered { get; set; }
        public string AmountValue { get; set; }
        public double MaximumValue { get; set; }
        public double MinimumValue { get; set; }
    }
}

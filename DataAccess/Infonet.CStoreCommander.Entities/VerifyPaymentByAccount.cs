using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class VerifyPaymentByAccount
    {
        public bool IsPurchaseOrderRequired { get; set; }

        public string OverrideARLimitMessage { get; set; }

        public string CreditMessage { get; set; }

        public string UnauthorizedMessage { get; set; }

        public List<string> UsedPO { get; set; }

        public bool UseMultiPO { get; set; }

        public CardSummary CardSummary { get; set; }
    }
}

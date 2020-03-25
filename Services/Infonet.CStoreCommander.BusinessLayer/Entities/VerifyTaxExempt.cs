using Infonet.CStoreCommander.BusinessLayer.Utilities;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class VerifyTaxExempt
    {
        public bool ProcessSiteReturn { get; set; }

        public bool ProcessSiteSale { get; set; }

        public bool ProcessSiteSaleReturnTax { get; set; }

        public bool ProcessAite { get; set; }

        public bool ProcessQite { get; set; }

        public ErrorMessage ConfirmMessage { get; set; }

        public string TreatyName { get; set; }

        public  string TreatyNumber { get; set; }

    }
}

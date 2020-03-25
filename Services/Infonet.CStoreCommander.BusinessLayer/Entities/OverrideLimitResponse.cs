using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class OverrideLimitResponse
    {
        public string Caption { get; set; }

        public List<PurchaseItemResponse> PurchaseItems { get; set; }

        public List<ComboOverrideCodes> OverrideCodes { get; set; }

        public bool IsOverrideCodeEnabled { get; set; }

        public bool IsDocumentNoEnabled { get; set; }

        public bool IsRtvpValidationEnabled { get; set; }
    }

    public class PurchaseItemResponse
    {
        public mPrivateGlobals.teProductEnum ProductTypeId { get; set; }

        public string ProductId { get; set; }

        public string Quantity { get; set; }

        public string Price { get; set; }

        public string Amount { get; set; }

        public string EquivalentQuantity { get; set; }

        public string QuotaUsed { get; set; }

        public string QuotaLimit { get; set; }

        public string DisplayQuota { get; set; }
        
        public string FuelOverLimitText { get; set; }
    }


}

using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Override limit repsonse model
    /// </summary>
    public class OverrideLimitResponseModel
    {
        /// <summary>
        /// Caption
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Purchase items
        /// </summary>
        public List<PurchaseItemResponseModel> PurchaseItems { get; set; }

        /// <summary>
        /// Override codes
        /// </summary>
        public List<ComboOverrideCodesModel> OverrideCodes { get; set; }

        /// <summary>
        ///Is override enabled
        /// </summary>
        public bool IsOverrideCodeEnabled { get; set; }

        /// <summary>
        /// Is document enabled
        /// </summary>
        public bool IsDocumentNoEnabled { get; set; }

        /// <summary>
        /// Is rtvp validation enabled
        /// </summary>
        public bool IsRtvpValidationEnabled { get; set; }
    }
}
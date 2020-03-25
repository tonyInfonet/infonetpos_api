using Infonet.CStoreCommander.WebApi.Models.Tender;

namespace Infonet.CStoreCommander.WebApi.Models.Payment
{
    /// <summary>
    /// Update account tender model
    /// </summary>
    public class UpdateAccountTenderModel : UpdateTenderModel
    {
        /// <summary>
        /// Purchase model
        /// </summary>
        public string PurchaseOrder { get; set; }

        /// <summary>
        /// Override AR limit
        /// </summary>
        public bool OverrideArLimit { get; set; }
    }
}
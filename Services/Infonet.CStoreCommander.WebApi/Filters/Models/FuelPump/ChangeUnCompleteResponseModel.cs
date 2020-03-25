using Infonet.CStoreCommander.WebApi.Models.Report;

namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    /// <summary>
    /// Change uncomplete response model
    /// </summary>
    public class ChangeUnCompleteResponseModel
    {
        /// <summary>
        /// Change due
        /// </summary>
        public string ChangeDue { get; set; }

        /// <summary>
        /// Open drawer
        /// </summary>
        public bool OpenDrawer { get; set; }

        /// <summary>
        /// tax exempt receipt
        /// </summary>
        public ReportModel TaxExemptReceipt { get; set; }
    }
}
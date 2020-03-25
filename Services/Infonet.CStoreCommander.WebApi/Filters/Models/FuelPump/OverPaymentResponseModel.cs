using Infonet.CStoreCommander.WebApi.Models.Report;

namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    /// <summary>
    /// Over payment response model
    /// </summary>
    public class OverPaymentResponseModel
    {
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
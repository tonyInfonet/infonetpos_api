using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.WebApi.Models.Report;

namespace Infonet.CStoreCommander.WebApi.Models.Sale
{
    /// <summary>
    /// Write off response model
    /// </summary>
    public class WriteOffResponseModel
    {
        /// <summary>
        /// New sale
        /// </summary>
        public NewSale NewSale { get; set; }

        /// <summary>
        /// Write off receipt
        /// </summary>
        public ReportModel WriteOffReceipt { get; set; }

        /// <summary>
        /// Customer display
        /// </summary>
        public CustomerDisplay CustomerDisplay { get; set; }

    }
}
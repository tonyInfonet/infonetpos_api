using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.WebApi.Models.Report;
using Infonet.CStoreCommander.WebApi.Models.Sale;

namespace Infonet.CStoreCommander.WebApi.Models.Bottle
{
    /// <summary>
    /// Complete bottle return response model
    /// </summary>
    public class CompleteBottleReturnResponseModel
    {
        /// <summary>
        /// New sale
        /// </summary>
        public NewSale NewSale { get; set; }

        /// <summary>
        /// Open cash drawer
        /// </summary>
        public bool OpenCashDrawer { get; set; }

        /// <summary>
        /// Payment receipt
        /// </summary>
        public ReportModel PaymentReceipt { get; set; }

        /// <summary>
        /// Customer display
        /// </summary>
        public CustomerDisplay CustomerDisplay { get; set; }

    }
}
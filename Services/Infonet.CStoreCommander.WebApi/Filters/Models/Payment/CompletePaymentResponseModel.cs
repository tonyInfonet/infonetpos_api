using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.WebApi.Models.Report;
using Infonet.CStoreCommander.WebApi.Models.Sale;

namespace Infonet.CStoreCommander.WebApi.Models.Payment
{
    /// <summary>
    /// Complete payment response model
    /// </summary>
    public class CompletePaymentResponseModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CompletePaymentResponseModel()
        {
            NewSale = new NewSale();
            PaymentReceipt = new ReportModel();
            CustomerDisplays = new List<CustomerDisplay>();
        }

        /// <summary>
        /// New Sale
        /// </summary>
        public NewSale NewSale { get; set; }

        /// <summary>
        /// Report
        /// </summary>
        public ReportModel PaymentReceipt { get; set; }

        /// <summary>
        /// Open cash drawer
        /// </summary>
        public bool OpenCashDrawer { get; set; }

        /// <summary>
        /// Change due
        /// </summary>
        public string ChangeDue { get; set; }

        /// <summary>
        /// Is refund
        /// </summary>
        public bool IsRefund { get; set; }

        /// <summary>
        /// Limit exceed message
        /// </summary>
        public string LimitExceedMessage { get; set; }

        /// <summary>
        /// Customer display
        /// </summary>
        public List<CustomerDisplay> CustomerDisplays { get; set; }

    }
}
using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.WebApi.Models.Sale;

namespace Infonet.CStoreCommander.WebApi.Models.Payment
{
    /// <summary>
    /// Finish payment response model
    /// </summary>
    public class FinishPaymentResponseModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public FinishPaymentResponseModel()
        {
            NewSale = new NewSale();
            Receipts = new List<Entities.Report>();
            CustomerDisplays = new List<CustomerDisplay>();
        }

        /// <summary>
        /// New Sale
        /// </summary>
        public NewSale NewSale { get; set; }

        /// <summary>
        /// Report
        /// </summary>
        public List<Entities.Report> Receipts { get; set; }

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

        public string kickabckServerError { get; set; }
    }
}
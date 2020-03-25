using Infonet.CStoreCommander.BusinessLayer.Entities;
using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.WebApi.Models.Report;

namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Tender summary model
    /// </summary>
    public class TenderSummaryModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public TenderSummaryModel()
        {
            Tenders = new List<TenderResponseModel>();
            Messages = new List<object>();
            VendorCoupons = new List<VCoupon>();
        }

        /// <summary>
        /// Summary 1
        /// </summary>
        public string Summary1 { get; set; }

        /// <summary>
        /// Summary 2
        /// </summary>
        public string Summary2 { get; set; }

        /// <summary>
        /// Enable complete payment
        /// </summary>
        public bool EnableCompletePayment { get; set; }

        /// <summary>
        /// Display no receipt toggle
        /// </summary>
        public bool DisplayNoReceiptButton { get; set; }

        /// <summary>
        /// Is run away
        /// </summary>
        public bool EnableRunAway { get; set; }

        /// <summary>
        /// Is pump test
        /// </summary>
        public bool EnablePumpTest { get; set; } 

        /// <summary>
        /// Outstanding amount
        /// </summary>
        public string OutstandingAmount { get; set; }

        /// <summary>
        /// Issue store credit message
        /// </summary>
        public string IssueStoreCreditMessage { get; set; }

        /// <summary>
        /// Tenders
        /// </summary>
        public List<TenderResponseModel> Tenders { get; set; }

        /// <summary>
        /// Givex receipt
        /// </summary>
        public List<ReportModel> Receipts { get; set; }

        /// <summary>
        /// Messages
        /// </summary>
        public List<object> Messages { get; set; }

        /// <summary>
        /// Vendor coupons
        /// </summary>
        public List<VCoupon> VendorCoupons { get; set; }

        /// <summary>
        /// Customer display
        /// </summary>
        public CustomerDisplay CustomerDisplay { get; set; }
    }
}
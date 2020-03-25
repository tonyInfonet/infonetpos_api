namespace Infonet.CStoreCommander.WebApi.Models.Report
{
    /// <summary>
    /// Flash totals model
    /// </summary>
    public class Totals
    {
        /// <summary>
        /// Product sales
        /// </summary>
        public string ProductSales { get; set; }

        /// <summary>
        /// Line discount
        /// </summary>
        public string LineDiscount { get; set; }

        /// <summary>
        /// Invoice discount
        /// </summary>
        public string InvoiceDiscount { get; set; }

        /// <summary>
        /// Sale after discount
        /// </summary>
        public string SalesAfterDiscount { get; set; }

        /// <summary>
        /// Taxes
        /// </summary>
        public string Taxes { get; set; }

        /// <summary>
        /// Charges
        /// </summary>
        public string Charges { get; set; }

        /// <summary>
        /// Refunded
        /// </summary>
        public string Refunded { get; set; }

        /// <summary>
        /// Total receipts
        /// </summary>
        public string TotalsReceipts { get; set; }
    }
}
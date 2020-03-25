using Infonet.CStoreCommander.WebApi.Models.Report;
using Infonet.CStoreCommander.WebApi.Models.Sale;

namespace Infonet.CStoreCommander.WebApi.Models.ReturnSale
{
    /// <summary>
    /// Return sale response model
    /// </summary>
    public class ReturnSaleResponseModel
    {
        /// <summary>
        /// Sale model
        /// </summary>
        public SaleModel Sale { get; set; }

        /// <summary>
        /// Receipt
        /// </summary>
        public ReportModel Receipt { get; set; }
    }
}
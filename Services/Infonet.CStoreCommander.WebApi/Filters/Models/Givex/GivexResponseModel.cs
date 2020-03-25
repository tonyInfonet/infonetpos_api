using Infonet.CStoreCommander.WebApi.Models.Sale;

namespace Infonet.CStoreCommander.WebApi.Models.Givex
{
    /// <summary>
    /// Givex response model
    /// </summary>
    public class GivexResponseModel
    {
        /// <summary>
        /// Sale
        /// </summary>
        public SaleModel Sale { get; set; }

        /// <summary>
        /// Receipt
        /// </summary>
        public Entities.Report Receipt { get; set; }
    }
}
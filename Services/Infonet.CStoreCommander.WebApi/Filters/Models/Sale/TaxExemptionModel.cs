
namespace Infonet.CStoreCommander.WebApi.Models.Sale
{
    /// <summary>
    /// Tax exemption model
    /// </summary>
    public class TaxExemptionModel
    {
        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Tax exemption code
        /// </summary>
        public string TaxExemptionCode { get; set; }
    }
}
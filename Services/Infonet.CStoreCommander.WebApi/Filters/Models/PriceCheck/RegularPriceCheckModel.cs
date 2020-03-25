namespace Infonet.CStoreCommander.WebApi.Models.PriceCheck
{
    /// <summary>
    /// Regular Price Check Model
    /// </summary>
    public class RegularPriceCheckModel
    {
        /// <summary>
        /// Stock Code
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// TillNumber
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Sale Number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// RegisterNumber
        /// </summary>
        public byte RegisterNumber { get; set; }

        /// <summary>
        /// Regular Price
        /// </summary>
        public double RegularPrice { get; set; }

    }
}
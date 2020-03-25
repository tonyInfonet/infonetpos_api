namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// AITE Card Model
    /// </summary>
    public class AiteCardModel
    {
        /// <summary>
        /// Sale Number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till Number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Till Number
        /// </summary>
        public int ShiftNumber { get; set; }

        /// <summary>
        /// Register Number
        /// </summary>
        public byte RegisterNumber { get; set; }

        /// <summary>
        /// Card Number
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Bar Code
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 1 - By Bar Code 2 - By Card Number
        /// </summary>
        public byte CheckMode { get; set; }

    }
}
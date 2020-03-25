namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// QITE Card Model
    /// </summary>
    public class QiteCardModel
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
        /// Shift Number
        /// </summary>
        public int ShiftNumber { get; set; }


        /// <summary>
        /// Register Number
        /// </summary>
        public byte RegisterNumber { get; set; }

        /// <summary>
        /// Card Number
        /// </summary>
        public string BandMember { get; set; }

        
    }
}
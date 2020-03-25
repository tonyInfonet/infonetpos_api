namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// AITE GST PST Card Model
    /// </summary>
    public class AiteGstPstCardModel
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
        /// Treaty Number
        /// </summary>
        public string TreatyNumber { get; set; }

    }
}
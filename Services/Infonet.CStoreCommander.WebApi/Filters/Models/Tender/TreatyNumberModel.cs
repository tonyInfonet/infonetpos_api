namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Treaty Number Model
    /// </summary>
    public class TreatyNumberModel
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
        /// Register Number
        /// </summary>
        public byte RegisterNumber { get; set; }

        /// <summary>
        /// Treaty Number
        /// </summary>
        public string TreatyNumber { get; set; }

        /// <summary>
        /// Capture Method
        /// </summary>
        public short CaptureMethod { get; set; }

        /// <summary>
        /// Treaty Name (Treaty Customer Name)
        /// </summary>
        public string TreatyName { get; set; }

        /// <summary>
        /// Document Number (Permit Number)
        /// </summary>
        public string PermitNumber { get; set; }


        /// <summary>
        /// is Enter Pressed
        /// </summary>
        public bool IsEnterPress { get; set; }

    }
}
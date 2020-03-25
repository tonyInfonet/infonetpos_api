namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// FNGTR Model
    /// </summary>
    public class FngtrModel
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
        public string PhoneNumber { get; set; }
        
    }
}
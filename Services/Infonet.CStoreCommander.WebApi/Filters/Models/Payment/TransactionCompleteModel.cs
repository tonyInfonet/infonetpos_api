namespace Infonet.CStoreCommander.WebApi.Models.Payment
{
    /// <summary>
    /// Transaction complete model
    /// </summary>
    public class TransactionCompleteModel
    {
        /// <summary>
        /// Sale number
        /// </summary>
        public  int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }
    }
}
namespace Infonet.CStoreCommander.WebApi.Models.Givex
{
    /// <summary>
    /// Givex card balance model
    /// </summary>
    public class GivexCardBalanceModel
    {
        /// <summary>
        /// Card number
        /// </summary>
        public  string CardNumber { get; set; }

        /// <summary>
        /// Balance
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Receipt
        /// </summary>
        public Entities.Report Receipt { get; set; }

    }
}
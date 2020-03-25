using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Store credit model
    /// </summary>
    public class StoreCreditModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public StoreCreditModel()
        {
            StoreCredits = new List<StoreCreditInput>();
        }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Transaction type
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Tender code
        /// </summary>
        public string TenderCode { get; set; }

        /// <summary>
        /// Store credits
        /// </summary>
        public List<StoreCreditInput> StoreCredits { get; set; }
    }
}
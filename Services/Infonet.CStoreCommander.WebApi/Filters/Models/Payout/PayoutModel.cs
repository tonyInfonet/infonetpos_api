using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Payout
{
    /// <summary>
    /// Payout model
    /// </summary>
    public class PayoutModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public PayoutModel()
        {
            Taxes = new List<BusinessLayer.Entities.Tax>();
        }

        /// <summary>
        /// Register number
        /// </summary>
        public byte RegisterNumber { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Reason code
        /// </summary>
        public string ReasonCode { get; set; }

        /// <summary>
        /// Vendor code
        /// </summary>
        public string VendorCode { get; set; }

        /// <summary>
        /// Taxes
        /// </summary>
        public List<BusinessLayer.Entities.Tax> Taxes { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }
    }
}
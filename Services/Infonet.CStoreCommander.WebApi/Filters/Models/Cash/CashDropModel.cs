using System.Collections.Generic;
using Infonet.CStoreCommander.WebApi.Models.Tender;

namespace Infonet.CStoreCommander.WebApi.Models.Cash
{
    /// <summary>
    /// Cash drop model
    /// </summary>
    public class CashDropModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CashDropModel()
        {
            Tenders = new List<TenderModel>();
        }

        /// <summary>
        /// Tenders
        /// </summary>
        public List<TenderModel> Tenders { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Register number
        /// </summary>
        public byte RegisterNumber { get; set; }

        /// <summary>
        /// Envelope number
        /// </summary>
        public string EnvelopeNumber { get; set; }

        /// <summary>
        /// Drop reason
        /// </summary>
        public string DropReason { get; set; }
    }
}
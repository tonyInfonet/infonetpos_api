using System.Collections.Generic;
using Infonet.CStoreCommander.WebApi.Models.Tender;

namespace Infonet.CStoreCommander.WebApi.Models.Cash
{
    /// <summary>
    /// Cash tender model
    /// </summary>
    public class CashTenderModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CashTenderModel()
        {
            Tenders = new List<SelectedTender>();
        }

        /// <summary>
        /// Tenders
        /// </summary>
        public List<SelectedTender> Tenders { get; set; }

        /// <summary>
        /// Sale number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till number
        /// </summary>
        public int TillNumber { get; set; }

        /// <summary>
        /// Drop reason
        /// </summary>
        public string DropReason { get; set; }
    }
}
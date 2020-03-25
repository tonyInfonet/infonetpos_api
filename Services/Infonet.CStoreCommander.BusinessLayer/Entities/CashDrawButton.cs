using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class CashDrawButton
    {

        public CashDrawButton()
        {
            Coins = new List<Cash>();
            Bills = new List<Cash>();
        }

        public List<Cash> Coins { get; set; }

        public List<Cash> Bills { get; set; }

        public decimal Amount { get; set; }

        public int TillNumber { get; set; }

        public short RegisterNumber { get; set; }

        public string DrawReason {get;set;}
    }
}

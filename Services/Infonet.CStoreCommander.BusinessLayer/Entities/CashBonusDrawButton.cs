using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
   public class CashBonusDrawButton
    {
        public CashBonusDrawButton()
        {
            Coins = new List<CashBonus>();
          
        }

        public List<CashBonus> Coins { get; set; }

      

        public decimal Amount { get; set; }

        public int TillNumber { get; set; }

        public short RegisterNumber { get; set; }

        public string DrawReason { get; set; }
    }
}

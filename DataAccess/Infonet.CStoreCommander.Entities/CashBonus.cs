using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.Entities
{
    public class CashBonus
    {
        public string CurrencyName { get; set; }

        public decimal Value { get; set; }

        public int ButtonNumber { get; set; }

        public int Quantity { get; set; }
    }
}

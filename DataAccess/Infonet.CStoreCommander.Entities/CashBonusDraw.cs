using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.Entities
{
    public class CashBonusDraw
    {

        public DateTime DrawDate { get; set; }

        public string User { get; set; }

        public int TillNumber { get; set; }

        //public float TotalValue { get; set; }

        public string Reason { get; set; }

        public decimal CashBonus { get; set; }
    }
}

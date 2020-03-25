using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.Entities
{
    public class CardProfileTimeLimit
    {

        public bool AllowPurchase { get; set; }

        public bool TimeRestriction { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime StartTime { get; set; }
    }
}

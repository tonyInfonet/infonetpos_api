using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.Entities
{
    public class ShiftStore
    {
        public int ShiftNumber { get; set; }

        public DateTime StartTime { get; set; }

        public byte Active { get; set; }

        public byte CurrentDay { get; set; }
    }
}

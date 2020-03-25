using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.Entities
{
    public class GiftCert
    {
        public int SaleNumber { get; set; }
        public int LineNumber { get; set; }
        public decimal GcAmount { get; set; }
        public string GcCust { get; set; }
        public string GcStore { get; set; }
        public DateTime GcDate { get; set; }
        public string GcUser { get; set; }
        public decimal GcRegister { get; set; }
        public DateTime GcExpiresOn { get; set; }
        public string GcNumber { get; set; }
        public int GcExpiryDays { get; set; }

    }
}

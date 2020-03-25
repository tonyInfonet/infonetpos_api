using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class PSVoucherInfo
    {
        public PSVoucher Voucher { get; set; }
        public List<PSLogo> Logos { get; set; }
    }
}

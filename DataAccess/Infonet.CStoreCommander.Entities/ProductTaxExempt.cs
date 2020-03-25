using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.Entities
{
    public class ProductTaxExempt
    {
        public short? CategoryFK { get; set; }
        public string TEVendor { get; set; }
        public float TaxFreePrice { get; set; }
        public short TaxCode { get; set; }

        public int Available { get; set; }
    }
}

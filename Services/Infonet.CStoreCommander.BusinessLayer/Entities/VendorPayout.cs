using Infonet.CStoreCommander.Resources;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class VendorPayout
    {
        public VendorPayout()
        {
            Reasons = new List<VendorReason>();
            Vendors = new List<PayoutVendor>();
            Taxes = new List<Tax>();
            Message = new MessageStyle();
        }

        public List<VendorReason> Reasons { get; set; }

        public List<PayoutVendor> Vendors { get; set; }

        public List<Tax> Taxes{ get; set; }

        public MessageStyle Message { get; set; }


    }

    public class PayoutVendor
    {
        public string Code { get; set; }

        public string Name { get; set; }
    }


    public class Tax
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }
    }

    public class VendorReason

    {
        public string Code { get; set; }

        public string Description { get; set; }
    }
}

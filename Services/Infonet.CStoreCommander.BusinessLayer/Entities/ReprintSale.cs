
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class ReprintSale
    {
        public bool IsPayInsideSale { get; set; }

        public bool IsPayAtPumpSale { get; set; }

        public bool IsPaymentSale { get; set; }

        public bool IsCloseBatchSale { get; set; }

        public List<PayInsideSale> PayInsideSales { get; set; }

        public List<PayAtPumpSale> PayAtPumpSales { get; set; }

        public List<PaymentSale> PaymentSales { get; set; }

        public List<CloseBatchSale> CloseBatchSales { get; set; }
    }
}

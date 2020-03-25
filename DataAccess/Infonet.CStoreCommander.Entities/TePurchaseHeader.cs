using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.Entities
{
    public class TePurchaseHeader
    {

        public int PurchaseNumber { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string WholeSaleNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime TransDate { get; set; }

        public bool Finalised { get; set; }
    }

    public class TePurchaseDetail
    {
        public int PurchaseNumber { get; set; }
        public string PurchaseItem { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public decimal PurchaseQuantity { get; set; }
        public short ProductType { get; set; }
        public string CurrentStock { get; set; }
    }
}

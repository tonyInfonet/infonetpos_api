using System;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class SpecialPriceCheck
    {
        public string StockCode { get; set; }

        public int TillNumber { get; set; }

        public int SaleNumber { get; set; }

        public byte RegisterNumber { get; set; }

        public bool ActiveVendorPrice { get; set; }

        public double RegularPrice { get; set; }

        public string VendorId { get; set; }

        public string PriceType { get; set; }

        public List<PriceGrid> GridPrices { get; set; }

        public DateTime Fromdate { get; set; }

        public DateTime Todate { get; set; }

        public bool PerDollarChecked { get; set; }

        public bool IsEndDate { get; set; }
    }
}

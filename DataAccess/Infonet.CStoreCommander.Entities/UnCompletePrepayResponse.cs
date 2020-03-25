using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class UnCompletePrepayResponse
    {
        public bool IsDeleteEnabled { get; set; }

        public bool IsChangeEnabled { get; set; }

        public bool IsOverPaymentEnabled { get; set; }

        public bool IsDeleteVisible { get; set; }

        public string Caption { get; set; }

        public List<UnCompleteSale> UnCompleteSale { get; set; }
    }

    public class UnCompleteSale
    {
        public short PumpId { get; set; }

        public short PositionId { get; set; }

        public int SaleNumber { get; set; }

        public double PrepayAmount { get; set; }


        public float PrepayVolume { get; set; }


        public float UsedAmount { get; set; }


        public float UsedVolume { get; set; }


        public int Grade { get; set; }

        public float UnitPrice { get; set; }


        public int SalePosition { get; set; }


        public int SaleGrade { get; set; }

        public double RegPrice { get; set; }

        public byte Mop { get; set; }
    }
}

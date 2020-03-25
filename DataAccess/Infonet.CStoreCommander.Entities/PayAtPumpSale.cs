using System;

namespace Infonet.CStoreCommander.Entities
{
    public class PayAtPumpSale
    {
        public int SaleNumber { get; set; }

        public DateTime SaleDate { get; set; }

        public DateTime SaleTime { get; set; }

        public decimal SaleAmount { get; set; }

        public float Volume { get; set; }

        public byte GradeId { get; set; }

        public byte PumpId { get; set; }
    }
}

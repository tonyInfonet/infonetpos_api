using System;

namespace Infonet.CStoreCommander.Entities
{
    public class CashDraw
    {

        public DateTime DrawDate { get; set; }

        public string User { get; set; }

        public int TillNumber { get; set; }

        public float TotalValue { get; set; }

        public string Reason { get; set; }
    }
}

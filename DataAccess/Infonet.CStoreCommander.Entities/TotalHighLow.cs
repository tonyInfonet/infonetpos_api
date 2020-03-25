using System;

namespace Infonet.CStoreCommander.Entities
{
    public class TotalHighLow
    {
        public int PumpId { get; set; }

        public double HighVolume { get; set; }

        public double LowVolume { get; set; }

        public DateTime Date { get; set; }
    }
}

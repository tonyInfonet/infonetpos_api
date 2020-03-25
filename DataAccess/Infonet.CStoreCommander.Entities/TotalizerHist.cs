using System;

namespace Infonet.CStoreCommander.Entities
{
    public class TotalizerHist
    {
        public int PumpId { get; set; }
        public int Grade { get; set; }
        public decimal Dollars { get; set; }
        public int GroupNumber { get; set; }
        public decimal Volume { get; set; }
        public DateTime Date { get; set; }
        public short PositionId { get; set; }
    }
}

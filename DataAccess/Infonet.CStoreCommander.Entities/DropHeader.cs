
using System;

namespace Infonet.CStoreCommander.Entities
{
    public class DropHeader
    {
        public DateTime DropDate { get; set; }
        public string UserCode { get; set; }
        public int TillNumber { get; set; }
        public int CloseNumber { get; set; }
        public short DropCount { get; set; }
        public short ShiftId { get; set; }
        public DateTime ShiftDate { get; set; }
        public string EnvelopeNo { get; set; }
        public string ReasonCode { get; set; }
        public int DropId { get; set; }
    }
}

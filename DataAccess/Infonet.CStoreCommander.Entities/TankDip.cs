using System;

namespace Infonet.CStoreCommander.Entities
{
    public class TankDip
    {
        public short DipNumber { get; set; }

        public short TankId { get; set; }

        public float FuelDip { get; set; }

        public float WaterDip { get; set; }

        public float Temperature { get; set; }

        public DateTime Date { get; set; }

        public DateTime ShiftDate { get; set; }

        public DateTime ReadTime { get; set; }

        public int GradeId { get; set; }

        public float Volume { get; set; }

        public float Vllage { get; set; }
    }

    public class TankGaugeSetup
    {
        public string DipUM { get; set; }

        public string TempUM { get; set; }
    }
}

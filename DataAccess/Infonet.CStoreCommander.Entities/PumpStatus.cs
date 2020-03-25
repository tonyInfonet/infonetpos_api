using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class PumpStatus
    {

        public bool IsPrepayEnabled { get; set; }

        public bool IsFinishEnabled { get; set; }

        public bool IsManualEnabled { get; set; }

        public bool IsCurrentEnabled { get; set; }

        public bool IsFuelPriceEnabled { get; set; }

        public bool IsTierLevelEnabled { get; set; }

        public bool IsPropaneEnabled { get; set; }

        public bool IsStopButtonEnabled { get; set; }

        public bool IsResumeButtonEnabled { get; set; }


        public bool IsErrorEnabled { get; set; }

        public List<BigPump> BigPumps { get; set; }

        public List<PumpControl> Pumps { get; set; }
    }


    public class BigPump
    {
        public string PumpId { get; set; }

        public bool IsPumpVisible { get; set; }

        public string PumpLabel { get; set; }

        public string PumpMessage { get; set; }

        public string Amount { get; set; }
    }
}

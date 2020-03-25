using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    public class TierLevelModel
    {
        public List<int> PumpIds { get; set; }

        public int TierId { get; set; }

        public int LevelId { get; set; }
    }
}
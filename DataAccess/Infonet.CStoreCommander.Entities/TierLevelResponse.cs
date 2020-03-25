using Infonet.CStoreCommander.Resources;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class TierLevelResponse
    {
        public string PageCaption { get; set; }


        public List<PumpTierLevel> PumpTierLevels { get; set; }


        public List<Tier> Tiers { get; set; }


        public List<Level> Levels { get; set; }

        public MessageStyle Message { get; set; }
    }



    public class PumpTierLevel
    {
        public int PumpId { get; set; }

        public byte TierId { get; set; }

        public string TierName { get; set; }

        public byte LevelId { get; set; }

        public string LevelName { get; set; }
    }

    public class Tier
    {
        public byte TierId { get; set; }

        public string TierName { get; set; }
    }


    public class Level
    {
        public byte LevelId { get; set; }

        public string LevelName { get; set; }
    }
}

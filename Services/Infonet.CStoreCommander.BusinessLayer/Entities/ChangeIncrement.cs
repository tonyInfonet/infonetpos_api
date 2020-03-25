using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class ChangeIncrement
    {
        public List<PriceIncrement> PriceIncrements { get; set; } = new List<PriceIncrement>();
        public List<PriceDecrement> PriceDecrements { get; set; } = new List<PriceDecrement>();
        public bool IsCreditEnabled { get; set; }
    }

    public class PriceIncrement
    {
        public short Row { get; set; }
        public short GradeId { get; set; }
        public string Grade { get; set; }
        public string Cash { get; set; }
        public string Credit { get; set; }
    }

    public class PriceDecrement
    {
        public short Row { get; set; }
        public short TierId { get; set; }
        public short LevelId { get; set; }
        public string TierLevel { get; set; }
        public string Cash { get; set; }
        public string Credit { get; set; }
    }
}

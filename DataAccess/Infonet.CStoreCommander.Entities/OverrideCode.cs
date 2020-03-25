using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class OverrideCode
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string ExtraText { get; set; }

        public string IsTobacco { get; set; }

        public string CanBeUsedForTobaccoMaxThreshold { get; set; }
    }


    public class ComboOverrideCodes
    {
        public int RowId { get; set; }

        public List<string> Codes { get; set; }
    }
}

using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class QiteCardResponse
    {
        public string QiteBandMember { get; set; }

        public string BandMemberName { get; set; }

        public Dictionary<string, string> SaleSummary { get; set; }

        public Tenders Tenders { get; set; }

        public bool IsFrmOverLimit { get; set; }
    }
}

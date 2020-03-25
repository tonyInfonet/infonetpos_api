using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class AiteCardResponse
    {

        public string AiteCardNumber { get; set; }

        public string AiteCardHolderName { get; set; }

        public string BarCode { get; set; }

        public Dictionary<string, string> SaleSummary { get; set; }

        public Tenders Tenders { get; set; }

        public bool IsFrmOverLimit { get; set; }

    }
}

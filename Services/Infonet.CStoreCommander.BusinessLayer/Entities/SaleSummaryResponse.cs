using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class SaleSummaryResponse
    {
        public SaleSummaryResponse()
        {
            SaleSummary = new Dictionary<string, string>();
            Tenders = new Tenders();
        }
        public Dictionary<string, string> SaleSummary { get; set; }

        public Tenders Tenders { get; set; }
      
    }


    public class SaleSummaryInput
    {
        /// <summary>
        /// Sale Number
        /// </summary>
        public int SaleNumber { get; set; }

        /// <summary>
        /// Till Number
        /// </summary>
        public int TillNumber { get; set; }


        /// <summary>
        /// Register Number
        /// </summary>
        public byte RegisterNumber { get; set; }


        /// <summary>
        /// Checks Whether SITE is Validated
        /// </summary>
        public bool IsSiteValidated { get; set; }


        /// <summary>
        /// Checks Whether AITE is Validated
        /// </summary>
        public bool IsAiteValidated { get; set; }

        /// <summary>
        /// User Code
        /// </summary>
        public string UserCode { get; set; }
    }
}

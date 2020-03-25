using System;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    /// <summary>
    /// Givex Report model
    /// </summary>
    public class GivexReport
    {
        /// <summary>
        /// Givex Details
        /// </summary>
        public List<GivexDetails> ReportDetails { get; set; }

        /// <summary>
        /// Stock code
        /// </summary>
        public Report CloseBatchReport { get; set; }

    }

    /// <summary>
    /// Givex Details
    /// </summary>
    public class GivexDetails
    {
        public int Id { get; set; }

        public string CashOut { get; set; }

        public string BatchDate { get; set; }

        public string BatchTime { get; set; }

        public string Report { get; set; }
    }
}

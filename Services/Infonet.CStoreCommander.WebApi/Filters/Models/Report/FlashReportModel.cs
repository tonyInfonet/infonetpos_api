using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Report
{
    /// <summary>
    /// Flash report model
    /// </summary>
    public class FlashReportModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public FlashReportModel()
        {
            Totals = new Totals();
            Departments = new List<Dept>();
            Report = new ReportModel();
        }

        /// <summary>
        /// Totals
        /// </summary>
        public Totals Totals { get; set; }

        /// <summary>
        /// Departments
        /// </summary>
        public List<Dept> Departments { get; set; }

        /// <summary>
        /// Report
        /// </summary>
        public ReportModel Report { get; set; }
    }
}
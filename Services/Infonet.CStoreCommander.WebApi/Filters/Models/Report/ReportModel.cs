namespace Infonet.CStoreCommander.WebApi.Models.Report
{
    /// <summary>
    /// Report model
    /// </summary>
    public class ReportModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ReportModel()
        {
            Copies = 1;
        }

        /// <summary>
        /// Report name
        /// </summary>
        public string ReportName { get; set; }

        /// <summary>
        /// Report content
        /// </summary>
        public string ReportContent { get; set; }

        /// <summary>
        /// Copies
        /// </summary>
        public int Copies { get; set; }
    }
}
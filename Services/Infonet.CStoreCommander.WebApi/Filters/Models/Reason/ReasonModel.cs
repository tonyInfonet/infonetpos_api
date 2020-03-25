using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Reason
{
    /// <summary>
    /// Reason Model
    /// </summary>
    public class ReasonModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ReasonModel()
        {
            Reasons = new List<Reason>();
        }

        /// <summary>
        /// Reason title
        /// </summary>
        public string ReasonTitle { get; set; }

        /// <summary>
        /// Reasons
        /// </summary>
        public List<Reason> Reasons { get; set; }

    }

    /// <summary>
    /// Reason
    /// </summary>
    public class Reason
    {
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
    }
}
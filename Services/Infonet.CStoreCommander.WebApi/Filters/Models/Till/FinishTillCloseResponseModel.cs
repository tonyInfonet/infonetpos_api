using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;

namespace Infonet.CStoreCommander.WebApi.Models.Till
{
    /// <summary>
    /// Finish till close response model
    /// </summary>
    public class FinishTillCloseResponseModel
    {
        /// <summary>
        /// Reports
        /// </summary>
        public List<Entities.Report> Reports { get; set; }

        /// <summary>
        /// lcd message
        /// </summary>
        public  CustomerDisplay LcdMessage { get; set; }

        /// <summary>
        /// Message to display
        /// </summary>
        public MessageStyle Message { get; set; }
    }
}
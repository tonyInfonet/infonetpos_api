using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Combo override codes model
    /// </summary>
    public class ComboOverrideCodesModel
    {
        /// <summary>
        /// Row id
        /// </summary>
        public int RowId { get; set; }

        /// <summary>
        /// Codes
        /// </summary>
        public List<string> Codes { get; set; }
    }
}
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.WebApi.Models.Tender
{
    /// <summary>
    /// Cancel tender response model
    /// </summary>
    public class CancelTenderResponseModel
    {
        /// <summary>
        /// Success
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Customer display
        /// </summary>
        public CustomerDisplay CustomerDisplay { get; set; }
    }
}
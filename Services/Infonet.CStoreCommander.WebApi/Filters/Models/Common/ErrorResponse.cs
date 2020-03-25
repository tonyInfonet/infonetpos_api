using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Models.Report;

namespace Infonet.CStoreCommander.WebApi.Models.Common
{
    /// <summary>
    /// Error response
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Error
        /// </summary>
        public MessageStyle Error { get; set; }

    }

    public class ErrorResponseWithCaption
    {
        public MessageStyle Error { get; set; }
        public string Caption { get; set; }
    }



    public class FuelPriceResponse
    {
        public MessageStyle Error { get; set; }
        public string Caption { get; set; }

        public ReportModel PriceReport { get; set; }


        public ReportModel FuelPriceReport { get; set; }
    }
}
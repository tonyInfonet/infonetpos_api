using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.WebApi.Models.Report;

namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    public class SetPriceIncrementModel
    {
        public PriceIncrement Price { get; set; }
        public ReportModel Report { get; set; }
    }

    public class SetPriceDecrementModel
    {
        public PriceDecrement Price { get; set; }
        public ReportModel Report { get; set; }
    }
}
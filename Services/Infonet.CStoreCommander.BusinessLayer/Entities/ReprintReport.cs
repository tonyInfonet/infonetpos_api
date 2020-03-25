

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class ReprintReport
    {
        public string ReportType { get; set; }

        public bool IsEnabled { get; set; }

        public bool DateEnabled { get; set; }

        public string ReportName { get; set; }
    }

    public enum ReportType
    {
        PayInside_CurrentSale,
        PayInside_HistoricalSale,
        PayAtPump_CurrentSale,
        PayAtPump_HistoricalSale,
        Payments_ArPay,
        Payments_FleetCard,
        Payments_Payout,
        Payments_BottleReturn,
        CloseBatch
    }
}

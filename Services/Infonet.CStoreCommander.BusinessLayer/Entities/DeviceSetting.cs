namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class DeviceSetting
    {
        public DeviceSetting()
        {
            Scanner = new ScannerSetting();
            CashDrawer = new CashDrawerSetting();
            CustomerDisplay = new CustomerDisplaySetting();
            Report = new ReportSetting();
            Receipt = new ReceiptSetting();
            Msr = new MsrSetting();
        }

        public ScannerSetting Scanner { get; set; }

        public CashDrawerSetting CashDrawer { get; set; }

        public CustomerDisplaySetting CustomerDisplay { get; set; }

        public ReportSetting Report { get; set; }

        public ReceiptSetting Receipt { get; set; }

        public MsrSetting Msr { get; set; }
    }

    public class ScannerSetting
    {
        public bool UseScanner { get; set; }

        public bool UseOposScanner { get; set; }

        public string Port { get; set; }

        public string Setting { get; set; }

        public string Name { get; set; }

    }

    public class CustomerDisplaySetting
    {
        public byte Port { get; set; }

        public string Name { get; set; }

        public byte DisplayCode { get; set; }

        public byte DisplayLen { get; set; }

        public bool UseCustomerDisplay { get; set; }

        public bool UseOposCustomerDisplay { get; set; }
    }

    public class ReportSetting
    {
        public bool UseReportPrinter { get; set; }     

        public bool UseOposReportPrinter { get; set; }

        public string Name { get; set; }

        public string Driver { get; set; }

        public string Font { get; set; }

        public int FontSize { get; set; }
    }

    public class CashDrawerSetting
    {
        public bool UseCashDrawer { get; set; }


        public bool UseOposCashDrawer { get; set; }


        public string Name { get; set; }


        public short OpenCode { get; set; }

    }

    public class ReceiptSetting
    {

        public bool UseReceiptPrinter { get; set; }

        public bool UseOposReceiptPrinter { get; set; }

        public string Name { get; set; }

        public string ReceiptDriver { get; set; }

    }

    public class MsrSetting
    {
        public bool UseMsr { get; set; }

        public bool UseOposMsr { get; set; }

        public string Name { get; set; }

    }
}

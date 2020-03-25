using Microsoft.VisualBasic;

namespace Infonet.CStoreCommander.Entities
{
    public class Register
    {
        

        private short _mvarRegisterNum;

        private bool _mvarReportPrinter;
        private bool _mvarOposReportPrinter;
        private string _mvarReportPrinterName;
        private string _mvarReportPrinterDriver;
        private string _mvarReportPrinterFont;
        private string _mvarReportPrinterFontSize;

        private bool _mvarCustomerDisplay;
        private bool _mvarOposCustomerDisplay;
        private byte _mvarCustomerDisplayPort;
        private string _mvarCustomerDisplayName;
        private byte _mvarCustomerDisplayCode;
        private byte _mvarCustomerDisplayLen;

        private bool _mvarScanner;
        private bool _mvarOposScanner;
        private string _mvarScannerPort;
        private string _mvarScannerSetting;
        private string _mvarScannerName;

        private bool _mvarCashDrawer;
        private bool _mvarOposCashDrawer;
        private string _mvarCashDrawerName;

        private bool _mvarMsr;
        private bool _mvarOposMsr;
        private string _mvarMsrName;

        private bool _mvarReceiptPrinter;
        private bool _mvarOposReceiptPrinter;
        private string _mvarReceiptPrinterName;
        private string _mvarReceiptDriver;

        private bool _mvarWritePosLog;
        private short _mvarCashDrawerOpenCode;

        // 
        private bool _mvarScale;
        private bool _mvarOposScale;
        private string _mvarScaleName;
        //shiny end



        public bool Scanner
        {
            get
            {
                bool returnValue = false;
                returnValue = _mvarScanner;
                return returnValue;
            }
            set
            {
                _mvarScanner = value;
            }
        }


        public bool Opos_Scanner
        {
            get
            {
                bool returnValue = false;
                returnValue = _mvarOposScanner;
                return returnValue;
            }
            set
            {
                _mvarOposScanner = value;
            }
        }


        public string MSR_Name
        {
            get
            {
                string returnValue = "";
                returnValue = _mvarMsrName;
                return returnValue;
            }
            set
            {
                _mvarMsrName = value;
            }
        }


        public string Scanner_Port
        {
            get
            {
                string returnValue = "";
                returnValue = _mvarScannerPort;
                return returnValue;
            }
            set
            {
                _mvarScannerPort = value;
            }
        }


        public string Scanner_Setting
        {
            get
            {
                string returnValue = "";
                returnValue = _mvarScannerSetting;
                return returnValue;
            }
            set
            {
                _mvarScannerSetting = value;
            }
        }


        public string Scanner_Name
        {
            get
            {
                string returnValue = "";
                returnValue = _mvarScannerName;
                return returnValue;
            }
            set
            {
                _mvarScannerName = value;
            }
        }



        public byte Customer_Display_Port
        {
            get
            {
                byte returnValue = 0;
                returnValue = _mvarCustomerDisplayPort;
                return returnValue;
            }
            set
            {
                _mvarCustomerDisplayPort = value;
            }
        }


        public string Customer_Display_Name
        {
            get
            {
                string returnValue = "";
                returnValue = _mvarCustomerDisplayName;
                return returnValue;
            }
            set
            {
                _mvarCustomerDisplayName = value;
            }
        }


        public byte Customer_Display_Code
        {
            get
            {
                byte returnValue = 0;
                returnValue = _mvarCustomerDisplayCode;
                return returnValue;
            }
            set
            {
                _mvarCustomerDisplayCode = value;
                if ((this.Customer_Display_Code == ((byte)1)) || (this.Customer_Display_Code == ((byte)3)))
                {
                    this.Customer_Display_Len = (byte)20;
                }
                else if (this.Customer_Display_Code == ((byte)2))
                {
                    this.Customer_Display_Len = (byte)18;
                }
                else
                {
                    this.Customer_Display_Len = (byte)0;
                }
            }
        }


        public byte Customer_Display_Len
        {
            get
            {
                byte returnValue = 0;
                returnValue = _mvarCustomerDisplayLen;
                return returnValue;
            }
            set
            {
                _mvarCustomerDisplayLen = value;
            }
        }


        public bool MSR
        {
            get
            {
                bool returnValue = false;
                returnValue = _mvarMsr;
                return returnValue;
            }
            set
            {
                _mvarMsr = value;
            }
        }


        public bool Opos_MSR
        {
            get
            {
                bool returnValue = false;
                returnValue = _mvarOposMsr;
                return returnValue;
            }
            set
            {
                _mvarOposMsr = value;
            }
        }


        public bool Customer_Display
        {
            get
            {
                bool returnValue = false;
                returnValue = _mvarCustomerDisplay;
                return returnValue;
            }
            set
            {
                _mvarCustomerDisplay = value;
            }
        }


        public bool Opos_Customer_Display
        {
            get
            {
                bool returnValue = false;
                returnValue = _mvarOposCustomerDisplay;
                return returnValue;
            }
            set
            {
                _mvarOposCustomerDisplay = value;
            }
        }


        public bool Report_Printer
        {
            get
            {
                bool returnValue = false;
                returnValue = _mvarReportPrinter;
                return returnValue;
            }
            set
            {
                _mvarReportPrinter = value;
            }
        }


        public string Report_Printer_Name
        {
            get
            {
                string returnValue = "";
                returnValue = _mvarReportPrinterName;
                return returnValue;
            }
            set
            {
                _mvarReportPrinterName = value;
            }
        }


        public string Report_Printer_Driver
        {
            get
            {
                string returnValue = "";
                returnValue = _mvarReportPrinterDriver;
                return returnValue;
            }
            set
            {
                _mvarReportPrinterDriver = value;
            }
        }


        public bool Opos_Report_Printer
        {
            get
            {
                bool returnValue = false;
                returnValue = _mvarOposReportPrinter;
                return returnValue;
            }
            set
            {
                _mvarOposReportPrinter = value;
            }
        }


        public string ReceiptPrinterName
        {
            get
            {
                string returnValue = "";
                returnValue = _mvarReceiptPrinterName;
                return returnValue;
            }
            set
            {
                _mvarReceiptPrinterName = value;
            }
        }


        public bool Cash_Drawer
        {
            get
            {
                bool returnValue = false;
                returnValue = _mvarCashDrawer;
                return returnValue;
            }
            set
            {
                _mvarCashDrawer = value;
            }
        }


        public bool Opos_Cash_Drawer
        {
            get
            {
                bool returnValue = false;
                returnValue = _mvarOposCashDrawer;
                return returnValue;
            }
            set
            {
                _mvarOposCashDrawer = value;
            }
        }


        public string Cash_Drawer_Name
        {
            get
            {
                string returnValue = "";
                returnValue = _mvarCashDrawerName;
                return returnValue;
            }
            set
            {
                _mvarCashDrawerName = value;
            }
        }


        public short Cash_Drawer_Open_Code
        {
            get
            {
                short returnValue = 0;
                returnValue = _mvarCashDrawerOpenCode;
                return returnValue;
            }
            set
            {
                _mvarCashDrawerOpenCode = value;
            }
        }



        public bool Receipt_Printer
        {
            get
            {
                bool returnValue = false;
                returnValue = _mvarReceiptPrinter;
                return returnValue;
            }
            set
            {
                _mvarReceiptPrinter = value;
            }
        }


        public bool Opos_Receipt_Printer
        {
            get
            {
                bool returnValue = false;
                returnValue = _mvarOposReceiptPrinter;
                return returnValue;
            }
            set
            {
                _mvarOposReceiptPrinter = value;
            }
        }


        public string ReceiptDriver
        {
            get
            {
                string returnValue = "";
                returnValue = _mvarReceiptDriver;
                return returnValue;
            }
            set
            {
                _mvarReceiptDriver = value;
            }
        }


        public string Report_Printer_font
        {
            get
            {
                string returnValue = "";
                returnValue = _mvarReportPrinterFont;
                return returnValue;
            }
            set
            {
                _mvarReportPrinterFont = value;
            }
        }


        public int Report_Printer_font_size
        {
            get
            {
                int returnValue = 0;
                returnValue = int.Parse(_mvarReportPrinterFontSize);
                return returnValue;
            }
            set
            {
                _mvarReportPrinterFontSize = (value).ToString();
            }
        }


        public short Register_Num
        {
            get
            {
                short returnValue = 0;
                returnValue = _mvarRegisterNum;
                return returnValue;
            }
           set
            {
                //ADODB.Recordset rs = default(ADODB.Recordset);

                _mvarRegisterNum = value;

                //rs = Chaps_Main.Get_Records("Select *  FROM Register  WHERE Register.Reg_No = " + System.Convert.ToString(value) + " ", Chaps_Main.dbAdmin, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

                //while (!rs.EOF)
                //{
                //    if (rs.Fields["DeviceID"].Value == Chaps_Main.Device_Renamed.ScannerID)
                //    {
                //        this.Scanner = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["Active"].Value)) ? false : (rs.Fields["Active"].Value));
                //        this.Opos_Scanner = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["UseOPOS"].Value)) ? false : (rs.Fields["UseOPOS"].Value));
                //        this.Scanner_Name = System.Convert.ToString((Information.IsDBNull(rs.Fields["DeviceName"].Value)) ? "" : (rs.Fields["DeviceName"].Value));
                //        this.Scanner_Port = System.Convert.ToString((Information.IsDBNull(rs.Fields["PortNum"].Value)) ? 0 : (rs.Fields["PortNum"].Value));
                //        this.Scanner_Setting = System.Convert.ToString((Information.IsDBNull(rs.Fields["PortSetting"].Value)) ? "" : (rs.Fields["PortSetting"].Value));
                //    }
                //    else if (rs.Fields["DeviceID"].Value == Chaps_Main.Device_Renamed.CashDrawerID)
                //    {
                //        this.Cash_Drawer = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["Active"].Value)) ? false : (rs.Fields["Active"].Value));
                //        this.Opos_Cash_Drawer = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["UseOPOS"].Value)) ? false : (rs.Fields["UseOPOS"].Value));
                //        this.Cash_Drawer_Name = System.Convert.ToString((Information.IsDBNull(rs.Fields["DeviceName"].Value)) ? "" : (rs.Fields["DeviceName"].Value));
                //        this.Cash_Drawer_Open_Code = System.Convert.ToInt16(rs.Fields["PortNum"].Value); 
                //    }
                //    else if (rs.Fields["DeviceID"].Value == Chaps_Main.Device_Renamed.CustomerDisplayID)
                //    {
                //        this.Customer_Display = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["Active"].Value)) ? false : (rs.Fields["Active"].Value));
                //        this.Opos_Customer_Display = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["UseOPOS"].Value)) ? false : (rs.Fields["UseOPOS"].Value));
                //        this.Customer_Display_Name = System.Convert.ToString((Information.IsDBNull(rs.Fields["DeviceName"].Value)) ? "" : (rs.Fields["DeviceName"].Value));
                //        if (Information.IsDBNull(rs.Fields["DriverName"].Value))
                //        {
                //            this.Customer_Display_Code = (byte)0;
                //        }
                //        else if (Strings.Len(rs.Fields["DriverName"].Value) == 0)
                //        {
                //            this.Customer_Display_Code = (byte)0;
                //        }
                //        else
                //        {
                //            this.Customer_Display_Code = (byte)(Conversion.Val(Strings.Left(System.Convert.ToString(rs.Fields["DriverName"].Value), 1)));
                //        }
                //        this.Customer_Display_Port = System.Convert.ToByte((Information.IsDBNull(rs.Fields["PortNum"].Value)) ? 0 : (rs.Fields["PortNum"].Value));

                //        
                //        if (rs.Fields["UseOPOS"].Value)
                //        {
                //            this.Customer_Display_Len = (byte)20;
                //        }
                //    }
                //    else if (rs.Fields["DeviceID"].Value == Chaps_Main.Device_Renamed.ReceiptPrinterID)
                //    {
                //        this.Receipt_Printer = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["Active"].Value)) ? false : (rs.Fields["Active"].Value));
                //        this.Opos_Receipt_Printer = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["UseOPOS"].Value)) ? false : (rs.Fields["UseOPOS"].Value));
                //        this.ReceiptPrinterName = System.Convert.ToString((Information.IsDBNull(rs.Fields["DeviceName"].Value)) ? "" : (rs.Fields["DeviceName"].Value));
                //        this.ReceiptDriver = System.Convert.ToString((Information.IsDBNull(rs.Fields["DriverName"].Value)) ? "" : (rs.Fields["DriverName"].Value));
                //    }
                //    else if (rs.Fields["DeviceID"].Value == Chaps_Main.Device_Renamed.ReportPrinterID)
                //    {
                //        this.Report_Printer = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["Active"].Value)) ? false : (rs.Fields["Active"].Value));
                //        this.Opos_Report_Printer = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["UseOPOS"].Value)) ? false : (rs.Fields["UseOPOS"].Value));
                //        this.Report_Printer_Name = System.Convert.ToString((Information.IsDBNull(rs.Fields["DeviceName"].Value)) ? "" : (rs.Fields["DeviceName"].Value));
                //        this.Report_Printer_Driver = System.Convert.ToString((Information.IsDBNull(rs.Fields["DriverName"].Value)) ? "" : (rs.Fields["DriverName"].Value));
                //        this.Report_Printer_font = System.Convert.ToString((Information.IsDBNull(rs.Fields["FontName"].Value)) ? "" : (rs.Fields["FontName"].Value));
                //        this.Report_Printer_font_size = System.Convert.ToInt32((Information.IsDBNull(rs.Fields["FontSize"].Value)) ? 0 : (rs.Fields["FontSize"].Value));
                //    }
                //    else if (rs.Fields["DeviceID"].Value == Chaps_Main.Device_Renamed.MSRID)
                //    {
                //        this.MSR = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["Active"].Value)) ? false : (rs.Fields["Active"].Value));
                //        this.Opos_MSR = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["UseOPOS"].Value)) ? false : (rs.Fields["UseOPOS"].Value));
                //        this.MSR_Name = System.Convert.ToString((Information.IsDBNull(rs.Fields["DeviceName"].Value)) ? "" : (rs.Fields["DeviceName"].Value));
                //    } //  - scale integration
                //    else if (rs.Fields["DeviceID"].Value == Chaps_Main.Device_Renamed.ScaleID)
                //    {
                //        this.UseScale = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["Active"].Value)) ? false : (rs.Fields["Active"].Value));
                //        this.OPOS_Scale = System.Convert.ToBoolean((Information.IsDBNull(rs.Fields["UseOPOS"].Value)) ? false : (rs.Fields["UseOPOS"].Value));
                //        this.SCALE_Name = System.Convert.ToString((Information.IsDBNull(rs.Fields["DeviceName"].Value)) ? "" : (rs.Fields["DeviceName"].Value));
                //        //Shiny end
                //    }
                //    rs.MoveNext();
                //}
                //rs = null;

            }
        }


        public bool WritePosLog
        {
            get
            {
                bool returnValue = false;
                returnValue = _mvarWritePosLog;
                return returnValue;
            }
            set
            {
                _mvarWritePosLog = value;
            }
        }

        //Shiny dec5, 2008 - Scale integration

        public bool UseScale
        {
            get
            {
                bool returnValue = false;
                returnValue = _mvarScale;
                return returnValue;
            }
            set
            {
                _mvarScale = value;
            }
        }


        public bool OPOS_Scale
        {
            get
            {
                bool returnValue = false;
                returnValue = _mvarOposScale;
                return returnValue;
            }
            set
            {
                _mvarOposScale = value;
            }
        }


        public string SCALE_Name
        {
            get
            {
                string returnValue = "";
                returnValue = _mvarScaleName;
                return returnValue;
            }
            set
            {
                _mvarScaleName = value;
            }
        }
        //shiny end
    }
}

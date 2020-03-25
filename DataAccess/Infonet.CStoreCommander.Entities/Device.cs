// VBConversions Note: VB project level imports
//using AxccrpMonthcal6;
//using AxCCRPDTP6;
using Microsoft.VisualBasic;
// End of VB project level imports


namespace Infonet.CStoreCommander.Entities
{
    public class Device
    {
        private string[] myDeviceName;
        private byte myScannerID;
        private byte myReceiptPrinterID;
        private byte myMSRID;
        private byte myCashDrawerID;
        private byte myCustomerDisplayID;
        private byte myReportPrinterID;
        private byte myScaleID;

        public string[] DeviceName { get; set; }

        public string get_DeviceName(byte DeviceID)
        {
            string returnValue = "";
            if (DeviceID != 0)
            {
                if (DeviceID >= 0 && DeviceID <= (myDeviceName.Length - 1))
                {
                    returnValue = myDeviceName[DeviceID];
                }
                else
                {
                    returnValue = "";
                }
            }
            else
            {
                returnValue = "";
            }
            return returnValue;
        }


        public byte ScannerID
        {
            get
            {
                byte returnValue = 0;
                returnValue = myScannerID;
                return returnValue;
            }
            set
            {
                myScannerID = value;
            }
        }


        public byte ReceiptPrinterID
        {
            get
            {
                byte returnValue = 0;
                returnValue = myReceiptPrinterID;
                return returnValue;
            }
            set
            {
                myReceiptPrinterID = value;
            }
        }

        public byte MSRID
        {
            get
            {
                byte returnValue = 0;
                returnValue = myMSRID;
                return returnValue;
            }
            set
            {
                myMSRID = value;
            }
        }

        public byte CashDrawerID
        {
            get
            {
                byte returnValue = 0;
                returnValue = myCashDrawerID;
                return returnValue;
            }
            set
            {
                myCashDrawerID = value;
            }
        }

        public byte CustomerDisplayID
        {
            get
            {
                byte returnValue = 0;
                returnValue = myCustomerDisplayID;
                return returnValue;
            }
            set
            {
                myCustomerDisplayID = value;
            }
        }

        public byte ReportPrinterID
        {
            get
            {
                byte returnValue = 0;
                returnValue = myReportPrinterID;
                return returnValue;
            }
            set
            {
                myReportPrinterID = value;
            }
        }
        //  - scale integration

        public byte ScaleID
        {
            get
            {
                byte returnValue = 0;
                returnValue = myScaleID;
                return returnValue;
            }
            set
            {
                myScaleID = value;
            }
        }

        private void Class_Initialize_Renamed()
        {
           
        }
        public Device()
        {
            Class_Initialize_Renamed();
        }

        private void Class_Terminate_Renamed()
        {
            //rsDevice = null;
        }
        ~Device()
        {
            Class_Terminate_Renamed();
            //base.Finalize();
        }
        //shiny end
    }
}

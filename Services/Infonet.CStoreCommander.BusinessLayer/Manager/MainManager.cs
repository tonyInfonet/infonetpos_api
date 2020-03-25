using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;
using System.Linq;
using System.Net;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using Constants = Infonet.CStoreCommander.BusinessLayer.Utilities.Constants;
using System.IO;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class MainManager : ManagerBase, IMainManager
    {
        private readonly IUtilityService _utilityService;
        private readonly IPolicyManager _policyManager;

        public MainManager(IUtilityService utilityService,
            IPolicyManager policyManager)
        {
            _utilityService = utilityService;
            _policyManager = policyManager;
        }

        /// <summary>
        /// Method to load device
        /// </summary>
        /// <param name="device">Device</param>
        public void LoadDevice(ref Device device)
        {
            var newDevice = _utilityService.LoadDevice();
            if (newDevice != null)
            {
                device = newDevice;
            }
            else
            {
                device.ScannerID = 0;
                device.ReceiptPrinterID = 0;
                device.MSRID = 0;
                device.CashDrawerID = 0;
                device.CustomerDisplayID = 0;
                device.ReportPrinterID = 0;
                device.ScaleID = 0;
            }
        }

        /// <summary>
        /// Method to get register according to register number
        /// </summary>
        /// <param name="register">Register</param>
        /// <param name="registerNumber">Register</param>
        public void SetRegister(ref Register register, short registerNumber)
        {
            var existingRegister = CacheManager.GetRegister(PosId);
            if (existingRegister != null)
            {
                register = existingRegister;
                return;
            }
            var device = new Device();
            LoadDevice(ref device);
            register = _utilityService.SetRegisterInfo(registerNumber, device);
            if (register == null)
                return;
            register.WritePosLog = _utilityService.CanWritePosLog(PosId);
            CacheManager.AddRegister(register, PosId);
        }

        /// <summary>
        /// Method to get sound
        /// </summary>
        /// <returns>Sounds</returns>
        public Entities.Sound GetSoundFiles()
        {
            var sound = new Entities.Sound();
            var dbSounds = _utilityService.GetAllSounds();
            var pumpSounds = dbSounds.Where(s => s.SoundType == "Pump");
            var deviceSounds = dbSounds.Where(s => s.SoundType == "Device");
            var systemSounds = dbSounds.Where(s => s.SoundType == "System");
            foreach (var pumpSound in pumpSounds)
            {
                var pSound = new SoundInfo();
                if (pumpSound.ID != 1 || pumpSound.ID != 9)
                    pSound.File = (pumpSound.Active) ? (pumpSound.SoundName) + ".wav" : "";
                else
                    pSound.File = (pumpSound.Active) ? (pumpSound.SoundName) : "";
                switch (pumpSound.ID)
                {
                    case 1: pSound.Name = "Calling"; break;
                    case 2: pSound.Name = "PumpAuthorized"; break;
                    case 3: pSound.Name = "Stopped"; break;
                    case 4: pSound.Name = "Driveoff"; break;
                    case 5: pSound.Name = "Finished"; break;
                    case 6: pSound.Name = "PayatPumpStarted"; break;
                    case 7: pSound.Name = "PayatPumpDone"; break;
                    case 8: pSound.Name = "PumpError"; break;
                    case 9: pSound.Name = "Help"; break;
                }
                sound.PumpSounds.Add(pSound);
            }
            foreach (var deviceSound in deviceSounds)
            {
                var dSound = new SoundInfo
                {
                    File = (deviceSound.Active) ? (deviceSound.SoundName) + ".wav" : ""
                };
                switch (deviceSound.ID)
                {
                    case 1: dSound.Name = "DrawerOpened"; break;
                    case 2: dSound.Name = "PrintDone"; break;
                }
                sound.DeviceSounds.Add(dSound);
            }
            foreach (var systemSound in systemSounds)
            {
                var sSound = new SoundInfo
                {
                    File = (systemSound.Active) ? (systemSound.SoundName) + ".wav" : ""
                };
                switch (systemSound.ID)
                {
                    case 1: sSound.Name = "ItemDeleted"; break;
                    case 2: sSound.Name = "Error"; break;
                    case 3: sSound.Name = "StockFound"; break;
                    case 4: sSound.Name = "StocknotFound"; break;
                }
                sound.SystemSounds.Add(sSound);
            }

            return sound;
        }

        /// <summary>
        /// Method to get device setting using register number
        /// </summary>
        /// <param name="registerNumber">Register number</param>
        /// <param name="error">Error message</param>
        /// <returns>Device setting</returns>
        public DeviceSetting GetDeviceSetting(int registerNumber, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var device = new Device();
            LoadDevice(ref device);
            var register = _utilityService.SetRegisterInfo((short)registerNumber, device);
            if (register == null)
            {
                error.MessageStyle = new MessageStyle
                {
                    Message = Constants.InvalidRegister
                };
                error.StatusCode = HttpStatusCode.NotFound;
                return null;
            }
            return MapDeviceSetting(register);
        }

        
        /// <summary>
        /// Method to format display message
        /// </summary>
        /// <param name="register">Register</param>
        /// <param name="st1">Message first part</param>
        /// <param name="st2">Message second part</param>
        /// <returns></returns>
        public CustomerDisplay DisplayMsgLcd(Register register, string st1, string st2)
        {
            var store = _policyManager.LoadStoreInfo();
            var customerDisplay = new CustomerDisplay();
            if (register.Opos_Customer_Display) 
            {
                customerDisplay.OposText1 = st1;
                customerDisplay.OposText2 = st2;
            }
            else 
            {
                
                if (register.Customer_Display_Code == 1) //  Champion customer display
                {
                    short tempEd = 40;
                    ClearDisplayLcd(1, ref tempEd, ref customerDisplay);
                    short tempPo = 1;
                    SetCursor(ref tempPo, ref customerDisplay);

                    //PComLCD.Output = Chr(&H4) & Chr(&H1) & "T" & Chr(&H17) & ST1 & ST2
                    customerDisplay.NonOposTexts.Add('\u0004' + "\u0001" + "T" + "\u0017" + st1 + st2);
                } // Logic Controls customer display
                else if (register.Customer_Display_Code == 2)
                {

                    //PComLCD.Output = Chr(&H1F)
                    customerDisplay.NonOposTexts.Add(System.Convert.ToString('\u001F'));

                    customerDisplay.NonOposTexts.Add(st1 + "\r\n" + st2);

                    customerDisplay.NonOposTexts.Add(System.Convert.ToString('\u0014'));
                } // IEE customer display
                else if (register.Customer_Display_Code == 3)
                {
                    //        Chr (27) + "@" ' reset command
                    

                    if (Strings.UCase(System.Convert.ToString(store.Language)) == "ENGLISH")
                    {
                        customerDisplay.NonOposTexts.Add(System.Convert.ToString('\f')); // clear display
                        customerDisplay.NonOposTexts.Add(st1);
                        customerDisplay.NonOposTexts.Add(st2);
                    }
                    else
                    {
                        

                        customerDisplay.NonOposTexts.Add('\u001B' + "\u0074" + "\u0002");
                        customerDisplay.NonOposTexts.Add(System.Convert.ToString('\f')); // clear display
                        customerDisplay.NonOposTexts.Add(IEE_P322_String(st1));
                        customerDisplay.NonOposTexts.Add(IEE_P322_String(st2));
                    }
                    
                }
                else
                {

                    customerDisplay.NonOposTexts.Add(System.Convert.ToString('\f')); // clear display

                    customerDisplay.NonOposTexts.Add(st1);

                    customerDisplay.NonOposTexts.Add(st2);
                }
            } 
            return customerDisplay;
        }

        /// <summary>
        /// Method to format message entyered
        /// </summary>
        /// <param name="register">Register</param>
        /// <param name="strValNo1">Value 1</param>
        /// <param name="strValNo2">Value 2</param>
        /// <returns></returns>
        public string FormatLcdString(Register register, string strValNo1, string strValNo2)
        {
            string returnValue;
            strValNo1 = strValNo1?.Trim() ?? "";
            strValNo2 = strValNo2?.Trim() ?? "";

            var avLen = System.Convert.ToByte(register.Customer_Display_Len);
            var valNo1Len = (byte)strValNo1.Length;
            var valNo2Len = (byte)strValNo2.Length;

            if (avLen > valNo1Len + valNo2Len)
            {
                returnValue = strValNo1 + Strings.Space(avLen - valNo1Len - valNo2Len) + strValNo2;
            }
            else if (avLen > valNo2Len + 1)
            {
                returnValue = strValNo1.Substring(0, avLen - (valNo2Len + 1)) + " " + strValNo2;
            }
            else if (avLen > valNo2Len)
            {
                returnValue = strValNo2;
            }
            else
            {
                returnValue = strValNo2.Substring(strValNo2.Length - avLen, avLen);
            }

            return returnValue;
        }

        /// <summary>
        /// Method to get error log
        /// </summary>
        public FileStream GetErrorLog()
        {
            var folderPath = @"C:\APILog\";
            var filePath = folderPath + "ErrorLog_" + PosId + ".txt";
            if (FileSystem.Dir(filePath) != "")
            {
                var nH = (short)FileSystem.FreeFile();
                var fs = File.OpenRead(filePath);
                FileSystem.FileClose(nH);
                return fs;
            }
            return null;
        }

        /// <summary>
        /// Method to clear error log
        /// </summary>
        public bool ClearErrorLog()
        {
            var folderPath = @"C:\APILog\";
            var filePath = folderPath + "ErrorLog_" + PosId + ".txt";
            if (FileSystem.Dir(filePath) != "")
            {
                if (FileSystem.Dir(filePath) != "")
                {
                    FileSystem.Kill(filePath);
                }
            }
            return true;
        }

        /// <summary>
        /// Method to check whether error logs present
        /// </summary>
        public bool CheckErrorLog()
        {
            var folderPath = @"C:\APILog\";
            var filePath = folderPath + "ErrorLog_" + PosId + ".txt";
            if (File.Exists(filePath))
            {
                if (new FileInfo(filePath).Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        #region Private methods


        
        /// <summary>
        /// Methdo to set cursor
        /// </summary>
        /// <param name="po">PO </param>
        /// <param name="customerDisplay">Customer dispaly</param>
        private void SetCursor(ref short po, ref CustomerDisplay customerDisplay)
        {
            po = (short)(po < 1 | po > 40 ? 0x31 : po + 0x30);
            var strCommand = "P" + System.Convert.ToString(Strings.Chr(po));
            strCommand = '\u0004' + "\u0001" + strCommand + "\u0017";

            customerDisplay.NonOposTexts.Add(strCommand);
        }


        
        /// <summary>
        /// Method to format string message
        /// </summary>
        /// <param name="strSource">Source</param>
        /// <returns></returns>
        private string IEE_P322_String(string strSource)
        {
            string returnValue;
            var store = _policyManager.LoadStoreInfo();


            if (strSource.Trim() == "" || Strings.UCase(System.Convert.ToString(store.Language)) == "ENGLISH")
            {
                returnValue = strSource;
                return returnValue;
            }

            short i;
            var strTmp = "";
            for (i = 1; i <= strSource.Length; i++)
            {
                var strChr = strSource.Substring(i - 1, 1);
                switch (strChr)
                {
                    case "ç":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0x87));
                        break;
                    case "à":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0x85));
                        break;
                    case "è":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0x8A));
                        break;
                    case "ì":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0x8D));
                        break;
                    case "ò":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0x95));
                        break;
                    case "ù":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0x97));
                        break;
                    case "À":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xB7));
                        break;
                    case "È":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xD4));
                        break;
                    case "Ì":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xDE));
                        break;
                    case "Ò":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xE3));
                        break;
                    case "Ù":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xEB));
                        break;
                    case "á":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xA0));
                        break;
                    case "é":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0x82));
                        break;
                    case "í":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xA1));
                        break;
                    case "ó":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xA2));
                        break;
                    case "ú":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xA3));
                        break;
                    case "Á":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xB5));
                        break;
                    case "É":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0x90));
                        break;
                    case "Í":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xD6));
                        break;
                    case "Ó":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xE0));
                        break;
                    case "Ú":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xE9));
                        break;
                    case "Ç":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0x80));
                        break;
                    case "â":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0x87));
                        break;
                    case "ê":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0x88));
                        break;
                    case "î":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0x8C));
                        break;
                    case "ô":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0x93));
                        break;
                    case "û":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0x96));
                        break;
                    case "Â":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xB6));
                        break;
                    case "Ê":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xD2));
                        break;
                    case "Î":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xD7));
                        break;
                    case "Ô":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xE2));
                        break;
                    case "Û":
                        strTmp = strTmp + System.Convert.ToString(Strings.Chr(0xEA));
                        break;
                    default:
                        strTmp = strTmp + strChr;
                        break;
                }
            }
            returnValue = strTmp;
            return returnValue;
        }

        // Clear Display Range
        /// <summary>
        /// Method to clear display message
        /// </summary>
        /// <param name="st">Start index</param>
        /// <param name="ed">End index</param>
        /// <param name="customerDisplay">Customer display</param>
        private void ClearDisplayLcd(short st, ref short ed, ref CustomerDisplay customerDisplay)
        {
            st = (short)((st < 1) | st > 40 ? 0x31 : st + 0x30);
            ed = (short)(ed < 1 | ed > 40 ? 0x31 : ed + 0x30);
            var strCommand = "C" + System.Convert.ToString(Strings.Chr(st)) + System.Convert.ToString(Strings.Chr(ed));
            strCommand = '\u0004' + "\u0001" + strCommand + "\u0017";


            //PComLCD.Output = StrCommand'
            customerDisplay.NonOposTexts.Add(strCommand);
        }

        /// <summary>
        /// Method to map device setting
        /// </summary>
        /// <param name="register">Register</param>
        /// <returns></returns>
        private DeviceSetting MapDeviceSetting(Register register)
        {
            var device = new DeviceSetting
            {
                Scanner = new ScannerSetting
                {
                    UseScanner = register.Scanner,
                    UseOposScanner = register.Opos_Scanner,
                    Name = register.Scanner_Name,
                    Port = register.Scanner_Port,
                    Setting = register.Scanner_Setting
                },
                CashDrawer = new CashDrawerSetting
                {
                    UseCashDrawer = register.Cash_Drawer,
                    UseOposCashDrawer = register.Opos_Cash_Drawer,
                    Name = register.Cash_Drawer_Name,
                    OpenCode = register.Cash_Drawer_Open_Code
                },
                CustomerDisplay = new CustomerDisplaySetting
                {
                    UseCustomerDisplay = register.Customer_Display,
                    UseOposCustomerDisplay = register.Opos_Customer_Display,
                    Name = register.Customer_Display_Name,
                    DisplayCode = register.Customer_Display_Code,
                    Port = register.Customer_Display_Port,
                    DisplayLen = register.Customer_Display_Len
                },
                Receipt = new ReceiptSetting
                {
                    UseReceiptPrinter = register.Receipt_Printer,
                    UseOposReceiptPrinter = register.Opos_Receipt_Printer,
                    ReceiptDriver = register.ReceiptDriver,
                    Name = register.ReceiptPrinterName
                },
                Report = new ReportSetting
                {
                    UseReportPrinter = register.Report_Printer,
                    UseOposReportPrinter = register.Opos_Report_Printer,
                    Name = register.Report_Printer_Name,
                    Driver = register.Report_Printer_Driver,
                    Font = register.Report_Printer_font,
                    FontSize = register.Report_Printer_font_size
                },
                Msr = new MsrSetting
                {
                    UseMsr = register.MSR,
                    UseOposMsr = register.Opos_MSR,
                    Name = register.MSR_Name
                }
            };




            return device;
        }
        #endregion
    }
}

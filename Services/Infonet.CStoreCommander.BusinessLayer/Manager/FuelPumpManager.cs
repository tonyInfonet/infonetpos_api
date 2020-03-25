using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Windows.Forms;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{

    public class FuelPumpManager : ManagerBase, IFuelPumpManager
    {
        private static int _pumpCount;
        private byte _pumpiSpace;
        private float _baskDimInterval;
        private byte _lineNum;
        private bool _pumpingBroadcast;
        private short _intNumberPerLine;
        private int _longHeight;
        private int _longWidth;
        private string _readUdpStrOldPrepayStatus = "";
        private bool _displayReader;
        private float[] _curBaskSit = new float[33];
        private short _putErrorLogFl = 0;
        private const short SCalling = 1;
        private const short SStopped = 2;
        private bool[] _payAtPumpRenamed = new bool[33];
        private float[] _stackBaskSit = new float[33];
        private Comm _xmlComm;
        private string _responseBuffer = string.Empty;


        private readonly IGetPropertyManager _getPropertyManager;
        private readonly IFuelPumpService _fuelPumpService;
        private readonly IPolicyManager _policyManager;
        private readonly ISaleManager _saleManager;
        private readonly ISaleLineManager _saleLineManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly IFuelService _fuelService;
        private readonly ITeSystemManager _teSystemManager;
        private readonly IPrepayManager _prepayManager;
        private readonly ITillService _tillService;
        private readonly ITillCloseService _tillCloseService;
      //  private readonly ICashBonusManager _cashBonusManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="getPropertyManager"></param>
        /// <param name="fuelPumpService"></param>
        /// <param name="policyManager"></param>
        /// <param name="saleManager"></param>
        /// <param name="saleLineManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="teSystemManager"></param>
        /// <param name="fuelService"></param>
        /// <param name="modPrepay"></param>
        /// <param name="tillService"></param>
        /// <param name="tillCloseService"></param>
        public FuelPumpManager(
            IGetPropertyManager getPropertyManager,
            IFuelPumpService fuelPumpService,
            IPolicyManager policyManager,
            ISaleManager saleManager,
            ISaleLineManager saleLineManager,
            IApiResourceManager resourceManager,
            ITeSystemManager teSystemManager,
            IFuelService fuelService,
            IPrepayManager modPrepay,
            ITillService tillService,
            ITillCloseService tillCloseService)
        {
            _getPropertyManager = getPropertyManager;
            _fuelPumpService = fuelPumpService;
            _saleLineManager = saleLineManager;
            _saleManager = saleManager;
            _policyManager = policyManager;
            _resourceManager = resourceManager;
            _teSystemManager = teSystemManager;
            _fuelService = fuelService;
            _prepayManager = modPrepay;
            _tillService = tillService;
            _tillCloseService = tillCloseService;
           // _cashBonusManager = cashBonusManager;
        }

        /// <summary>
        /// Method to load pumps
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Pump status</returns>
        public PumpStatus LoadPumps(int tillNumber)
        {
            var result = new PumpStatus();
            if (!_policyManager.USE_FUEL)
            {
                result.IsPrepayEnabled = false;
                result.IsFinishEnabled = false;
                result.IsManualEnabled = false;
                result.IsCurrentEnabled = false;
                result.IsFuelPriceEnabled = false;
                result.IsTierLevelEnabled = false;
                result.IsPropaneEnabled = false;
                result.IsStopButtonEnabled = false;
                result.IsResumeButtonEnabled = false;
                result.IsErrorEnabled = false;
            }
            else if (_policyManager.FUELONLY)
            {
                Enable_FuelOnly_Buttons(ref result, false);
                result.Pumps = Setup_Pump_System();
                Enable_FuelOnly_Buttons(ref result, true);
                Variables.IsChildForm = true;
            }
            else
            {
                Variables.LockUdp = false;
                var service = _fuelService.LoadService();

                result.Pumps = Setup_Pump_System();
                result.IsFinishEnabled = ExistPrepay(tillNumber);
                result.IsCurrentEnabled = false;
                result.IsFuelPriceEnabled = true;
                result.IsTierLevelEnabled = UDPAgent.Instance.IsConnected && TCPAgent.Instance.IsConnected && TCPAgent.Instance.SocketConnected;
                result.IsStopButtonEnabled = UDPAgent.Instance.IsConnected && TCPAgent.Instance.IsConnected && TCPAgent.Instance.SocketConnected && result.Pumps.Any(p => p.Status != "Stopped");
                result.IsResumeButtonEnabled = UDPAgent.Instance.IsConnected && TCPAgent.Instance.IsConnected && TCPAgent.Instance.SocketConnected && result.Pumps.Any(p => p.Status == "Stopped");
                if (TCPAgent.Instance.IsConnected)
                {
                    if (_policyManager.AllowPrepay && service.PrepayEnabled)
                    {
                        result.IsPrepayEnabled = !Variables.Return_Mode;
                    }
                    else
                    {
                        result.IsPrepayEnabled = false;
                    }
                }
                else
                {
                    result.IsPrepayEnabled = false;
                }
                result.IsErrorEnabled = false;

                result.IsManualEnabled = _policyManager.AllowManual && !Variables.Return_Mode;
                result.IsPropaneEnabled = _policyManager.USE_PROPANE && !Variables.Return_Mode;
                if (_policyManager.AllowPrepay && service.PrepayEnabled)
                {
                    result.IsFinishEnabled = ExistPrepay(tillNumber);
                }
                if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/Logs/")))
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Logs/"));
                }
                if (File.Exists(HttpContext.Current.Server.MapPath("~/Logs/") + Variables.ErrorFile))
                {
                    EnableErrorButton(ref result);
                }
            }
            return result;
        }

        /// <summary>
        /// Method to read UDP data
        /// </summary>
        /// <returns>Pump status</returns>
        public PumpStatus ReadUdpData(int tillNumber)
        {
            PumpStatus status = new PumpStatus();
            if (!TCPAgent.Instance.IsConnected || !TCPAgent.Instance.SocketConnected)
            {
                status = GetNoConnectionPumpStatus();
            }
            else
            {
                var service = _fuelService.LoadService();
                status.IsFinishEnabled = ExistPrepay(tillNumber);
                status.IsCurrentEnabled = false;
                status.IsFuelPriceEnabled = true;
                status.Pumps = CacheManager.GetAllPumps();

                if (status.Pumps == null)
                {
                    status.Pumps = Setup_Pump_System();
                    CacheManager.AddAllPumps(status.Pumps);
                }

                status.IsTierLevelEnabled = UDPAgent.Instance.IsConnected && TCPAgent.Instance.IsConnected && TCPAgent.Instance.SocketConnected;
                status.IsStopButtonEnabled = UDPAgent.Instance.IsConnected && TCPAgent.Instance.IsConnected && TCPAgent.Instance.SocketConnected && status.Pumps.Any(p => p.Status != "Stopped");
                status.IsResumeButtonEnabled = UDPAgent.Instance.IsConnected && TCPAgent.Instance.IsConnected && TCPAgent.Instance.SocketConnected && status.Pumps.Any(p => p.Status == "Stopped");
                if (TCPAgent.Instance.IsConnected)
                {
                    if (_policyManager.AllowPrepay && service.PrepayEnabled)
                    {
                        status.IsPrepayEnabled = !Variables.Return_Mode;
                    }
                    else
                    {
                        status.IsPrepayEnabled = false;
                    }
                }
                else
                {
                    status.IsPrepayEnabled = false;
                }
                status.IsErrorEnabled = false;

                status.IsManualEnabled = _policyManager.AllowManual && !Variables.Return_Mode;
                status.IsPropaneEnabled = _policyManager.USE_PROPANE && !Variables.Return_Mode;

            }
            return status;
        }

        private PumpStatus GetNoConnectionPumpStatus()
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            PumpStatus result = new PumpStatus();
            result.IsPrepayEnabled = false;
            result.IsFuelPriceEnabled = true;
            result.IsTierLevelEnabled = false;
            result.IsStopButtonEnabled = false;
            result.IsResumeButtonEnabled = false;
            result.IsErrorEnabled = false;
            result.IsManualEnabled = _policyManager.AllowManual && !Variables.Return_Mode;
            result.IsPropaneEnabled = _policyManager.USE_PROPANE && !Variables.Return_Mode;

            var pumps = CacheManager.GetAllPumps();

            if (Variables.gPumps != null)
            {
                _pumpCount = Variables.gPumps.PumpsCount;
            }

            for (int i = 1; i <= _pumpCount; i++)
            {
                pumps[i - 1].PumpButtonCaption = _resourceManager.CreateCaption(offSet, (short)68, Convert.ToInt16(38), null, (short)0); ;
                pumps[i - 1].SetPumpStatus((byte)i, "7");
                pumps[i - 1].BasketButtonCaption = "";
                pumps[i - 1].BasketButtonVisible = 1;
                pumps[i - 1].BasketLabelCaption = "";
                pumps[i - 1].EnableBasketBotton = true;
                pumps[i - 1].EnableStackBasketBotton = false;
                pumps[i - 1].PayPumporPrepay = false;
                pumps[i - 1].PrepayText = "";
                pumps[i - 1].CanCashierAuthorize = Variables.Cashier_Auth[i];
            }
            result.Pumps = pumps;
            return result;
        }


        /// <summary>
        /// Method to read price change notification
        /// </summary>
        /// <returns>Messge</returns>
        public MessageStyle ReadPricheChangeNotificationHo()
        {
            MessageStyle msg = null;
            if (_policyManager.FUELPR_HO)
            {
                if (modGlobalFunctions.BoolFuelPriceApplied)
                {
                    WriteToLogFile("Fuel price change from HeadOffice: boolFuelPriceApplied is " + Convert.ToString(modGlobalFunctions.BoolFuelPriceApplied) + " counter was reset to 0.");
                }
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                modGlobalFunctions.BoolFuelPriceApplied = false;
                if (_fuelPumpService.IsPriceChangeFromHo())
                {
                    if (_policyManager.FPR_USER)
                    {
                        MessageType msgType = (int)MessageType.Question + MessageType.YesNoCancel;
                        msg = _resourceManager.CreateMessage(offSet, 38, 71, null, msgType);
                    }
                    else
                    {
                        MessageType msgType = (int)MessageType.Question + MessageType.YesNo;
                        msg = _resourceManager.CreateMessage(offSet, 38, 71, null, msgType);
                    }
                }
            }
            return msg;
        }

        /// <summary>
        /// Method to update price chage
        /// </summary>
        /// <param name="ans">Selected option</param>
        /// <param name="counter"></param>
        /// <param name="error">Error message</param>
        /// <returns>Page name </returns>
        public string UpdatePriceChange(int ans, int counter, out ErrorMessage error)
        {
            string pageName = "";
            error = new ErrorMessage();
            var user = CacheManager.GetUser(UserCode);
            if (ans == 1)
            {
                WriteToLogFile("Fuel price change from HeadOffice: user accepted the fuel price change. Counter is " + Convert.ToString(0));
                Chaps_Main.boolFP_HO = true;
                pageName = Command1_ClickEvent(out error);
            }
            else if (ans == 3)
            {
                _fuelPumpService.DeleteFuelPrice();
                WriteToLogFile("Fuel price change from HeadOffice: user canceled the fuel price change.");
                pageName = "ResetCounter";
            }
            else
            {
                WriteToLogFile("Fuel price change from HeadOffice: user did not accept the fuel price change. Counter is " + Convert.ToString(0) + " User setting to reject the fule price change is " + _policyManager.FPR_USER);
                if (!_policyManager.GetPol("FPR_USER", user) && counter == _policyManager.FPR_NOTE_CNT)
                {
                    WriteToLogFile("Fuel price change from HeadOffice: user did not accept the fuel price change. " + "Fuel price screen will be displayed. User setting to reject the fule price change is " + _policyManager.FPR_USER);
                    Chaps_Main.boolFP_HO = true;
                    pageName = Command1_ClickEvent(out error);
                    if (string.IsNullOrEmpty(pageName))
                        pageName = "ResetCounter";
                    WriteToLogFile("Fuel price change from HeadOffice: counter was reset to 0.");
                }
                else if (_policyManager.GetPol("FPR_USER", user))
                {
                    WriteToLogFile("Fuel price change from HeadOffice: user can reject fuel price changes. Prices were rejected " + Convert.ToString(0) + " times. No message displayed until HeadOffice send another set of fuel prices.");
                    _fuelPumpService.DeleteFuelPrice();
                    pageName = "ResetCounter";
                }
            }
            return pageName;
        }

        /// <summary>
        /// Method to read UDP
        /// </summary>
        /// <param name="udpReading">UDP reading</param>
        /// <returns>Pump status</returns>
        public PumpStatus ReadUdp(string udpReading)
        {
            PumpStatus result = new PumpStatus();
            Service serviceRenamed = _fuelService.LoadService();
            var pumps = CacheManager.GetAllPumps();
            int tillNumber = 1;
            short iStrPoint = 0;
            bool fueling = false;
            bool hasBasket = false;
            string statusStr = "";
            short ifinPoint = 0;
            string strPrepayStatus = "";
            string ubasking = "";
            short uLen;
            short ifpoint1 = 0;
            short ifpoint2 = 0;
            short ifpoint3 = 0;
            short ifpoint4 = 0;
            bool refreshSuccess = false;
            short refreshCount = 0;
            short i = 0;
            short j = 0;

            Variables.UDPonLock = udpReading;

            WriteToLogFile("UDP received from " + Convert.ToString(Variables.gPumps.IP.FC_IP) + " " + Convert.ToString(Variables.gPumps.IP.FC_UDP_Port) + ": " + udpReading);

            uLen = (short)udpReading.Length;

            if (uLen == 0)
            {
                Variables.LockWindowUpdate(Convert.ToInt32(false));
                return result;
            }

            float tm = 0;

            if (TCPAgent.Instance.IsConnected)
            {
                if (_policyManager.AllowPrepay && serviceRenamed.PrepayEnabled)
                {
                    result.IsPrepayEnabled = !Variables.Return_Mode;
                }
                else
                {
                    result.IsPrepayEnabled = false;
                }
            }
            else
            {
                result.IsPrepayEnabled = false;
            }

            result.IsFinishEnabled = !string.IsNullOrEmpty(Variables.MyPrepayBaskets);

            result.IsFuelPriceEnabled = true;
            result.IsTierLevelEnabled = UDPAgent.Instance.IsConnected && TCPAgent.Instance.SocketConnected;
            result.IsStopButtonEnabled = UDPAgent.Instance.IsConnected && TCPAgent.Instance.SocketConnected;
            result.IsResumeButtonEnabled = UDPAgent.Instance.IsConnected && TCPAgent.Instance.SocketConnected;
            result.IsErrorEnabled = false;
            result.IsManualEnabled = _policyManager.AllowManual && !Variables.Return_Mode;
            result.IsPropaneEnabled = _policyManager.USE_PROPANE && !Variables.Return_Mode;


            if (udpReading.Substring(0, 7).ToUpper() == "FCSTART") // reset sockets on FC restart
            {
                UDPAgent.Instance.ClosePort();
                UDPAgent.Instance.OpenPort();
                TCPAgent.Instance.ClosePort();
                TCPAgent.Instance.OpenPort(Convert.ToString(Variables.gPumps.IP.FC_IP), Convert.ToInt16(Variables.gPumps.IP.FC_TCP_Port));
                if (TCPAgent.Instance.IsConnected)
                {
                    if (_policyManager.AllowPrepay && serviceRenamed.PrepayEnabled)
                    {
                        result.IsPrepayEnabled = !Variables.Return_Mode;
                    }
                    else
                    {
                        result.IsPrepayEnabled = false;
                    }
                }
                else
                {
                    result.IsPrepayEnabled = false;
                }
                result.IsFinishEnabled = !string.IsNullOrEmpty(Variables.MyPrepayBaskets);
                result.IsFuelPriceEnabled = true;
                result.IsTierLevelEnabled = UDPAgent.Instance.IsConnected && TCPAgent.Instance.SocketConnected;
                result.IsStopButtonEnabled = UDPAgent.Instance.IsConnected && TCPAgent.Instance.SocketConnected && pumps.Any(p => p.Status != "Stopped");
                result.IsResumeButtonEnabled = UDPAgent.Instance.IsConnected && TCPAgent.Instance.SocketConnected && pumps.Any(p => p.Status == "Stopped");
                result.IsErrorEnabled = false;
                result.IsManualEnabled = _policyManager.AllowManual && !Variables.Return_Mode;
                result.IsPropaneEnabled = _policyManager.USE_PROPANE && !Variables.Return_Mode;
                result.Pumps = pumps;
                CacheManager.AddAllPumps(pumps);
                return result;
            }

            if (Strings.Left(udpReading, 11).ToUpper() == "PRICECHANGE")
            {
                Variables.NeedToRefreshPumpPrice = true;
                return result;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            short pumpId = 0;
            if (Strings.Left(udpReading, 1) == "P" && udpReading.Substring(3, 5).ToUpper() == "RATIO")
            {
                pumpId = (short)Conversion.Val(udpReading.Substring(1, 2));
                if (udpReading.Substring(8, 1) == "N") //RatioNotMatch
                {
                    EnableErrorButton(ref result);
                    PutErrorLog(_resourceManager.CreateCaption(offSet, (short)95, Convert.ToInt16(38), pumpId, (short)0));
                }
                else if (udpReading.Substring(8, 1) == "C") //RatioChanged
                {
                    EnableErrorButton(ref result);
                    PutErrorLog(_resourceManager.CreateCaption(offSet, (short)96, Convert.ToInt16(38), pumpId, (short)0));

                }
                Variables.LockWindowUpdate(Convert.ToInt32(false));
                return result;
            }

            if (Strings.Left(udpReading, 4) == "Pump")
            {
                switch (udpReading.Substring(4, 1))
                {
                    case "O": //PumpOutOfService
                        EnableErrorButton(ref result);
                        PutErrorLog(_resourceManager.CreateCaption(offSet, (short)98, Convert.ToInt16(38), null, (short)0));
                        break;
                    case "I": //PumpInService
                        EnableErrorButton(ref result);
                        PutErrorLog(_resourceManager.CreateCaption(offSet, (short)99, Convert.ToInt16(38), null, (short)0));
                        break;
                }
                Variables.LockWindowUpdate(Convert.ToInt32(false));
                result.Pumps = pumps;
                return result;
            }

            if (Strings.Left(udpReading, 3) == "Isd")
            {
                switch (udpReading.Substring(3, 1))
                {
                    case "0":
                        serviceRenamed.PostPayEnabled = false;
                        break;
                    case "1":
                        serviceRenamed.PostPayEnabled = true;
                        break;
                }
                switch (udpReading.Substring(7, 1))
                {
                    case "0":
                        serviceRenamed.PrepayEnabled = false;
                        break;
                    case "1":
                        serviceRenamed.PrepayEnabled = true;
                        break;
                }
                return result;
            }

            var status = string.Empty;
            switch (Strings.Left(udpReading, 1))
            {
                case "E":
                    statusStr = udpReading.Substring(0, Variables.iPumpCount);
                    status = "E";
                    break;
                case "R":
                    var readerId = (short)Conversion.Val(udpReading.Substring(1, 2));
                    switch (udpReading.Substring(3, 1))
                    {
                        case "O": //Out of service (Reader)
                            EnableErrorButton(ref result);
                            PutErrorLog(_resourceManager.CreateCaption(offSet, (short)93, Convert.ToInt16(38), readerId, (short)0));
                            break;
                        case "P": //Paper out (Reader)
                            EnableErrorButton(ref result);
                            PutErrorLog(_resourceManager.CreateCaption(offSet, (short)94, Convert.ToInt16(38), readerId, (short)0));
                            break;
                        case "H": //HELP button was pressed (Reader)
                            Variables.LockWindowUpdate(Convert.ToInt32(false));
                            var pmp = pumps[readerId - 1];
                            SetTopSign(ref pmp, 5);
                            CacheManager.AddAllPumps(pumps);

                            break;
                        case "C": //Desjardins sends CLOSEBATCH (150) action code
                            EnableErrorButton(ref result);
                            PutErrorLog(_resourceManager.CreateCaption(offSet, (short)52, Convert.ToInt16(38), null, (short)0));
                            break;
                    }
                    Variables.LockWindowUpdate(Convert.ToInt32(false));
                    return result;
                default:
                    status = statusStr = udpReading.Substring(0, Variables.iPumpCount);
                    strPrepayStatus = udpReading.Substring(Variables.iPumpCount + 2 - 1, Variables.iPumpCount);
                    break;
            }

            ifinPoint = (short)(udpReading.IndexOf(";") + 1);
            ifinPoint = (short)(udpReading.IndexOf(";", ifinPoint + 1 - 1) + 1);

            ifinPoint = (short)(udpReading.IndexOf(";", ifinPoint + 1 - 1) + 1);
            ifpoint1 = (short)(udpReading.IndexOf(";", ifinPoint + 1 - 1) + 1); //each basket should have a ";"
            ifpoint2 = (short)(udpReading.IndexOf("@", ifinPoint + 1 - 1) + 1); //Reader status string start with "@"

            if (!string.IsNullOrEmpty(strPrepayStatus))
            {
                if (strPrepayStatus != _readUdpStrOldPrepayStatus)
                {
                    for (i = 1; i <= strPrepayStatus.Length; i++)
                    {
                        if (string.IsNullOrEmpty(_readUdpStrOldPrepayStatus) || Conversion.Val(strPrepayStatus.Substring(i - 1, 1)) > 0)
                        {
                            refreshSuccess = false;
                            refreshCount = (short)0;

                            while (!refreshSuccess && (refreshCount < 10))
                            {
                                refreshSuccess = _prepayManager.RefreshPrepay(i, (short)Conversion.Val(strPrepayStatus.Substring(i - 1, 1)));
                                refreshCount++;
                            }
                        }
                    }
                    _readUdpStrOldPrepayStatus = _prepayManager.PrepayStatusString();
                }
            }

            Status_Display(ref pumps, statusStr, status, false);
            if (statusStr.IndexOf("4", StringComparison.Ordinal) + 1 != 0 || statusStr.IndexOf("P", StringComparison.Ordinal) + 1 != 0)
            {
                fueling = true;
            }

            string basketStr = "";
            if (udpReading.Substring(ifinPoint - 1).Length > 22 & ifpoint1 != 0)
            {
                hasBasket = true;
                if (ifpoint2 == 0)
                {
                    basketStr = udpReading.Substring(ifinPoint + 1 - 1);
                }
                else if (ifpoint2 > ifpoint1) //if has ReaderStatusString, cut it
                {
                    basketStr = udpReading.Substring(ifinPoint + 1 - 1, ifpoint2 - ifinPoint - 1);
                }
            }
            BasketCleanUp(ref pumps);

            Variables.MyPrepayBaskets = "";
            Variables.SwitchPrepayBaskets = "";
            if (hasBasket)
            {
                while (basketStr.Length != 0)
                {
                    ifpoint4 = (short)(basketStr.IndexOf(";", StringComparison.Ordinal) + 1);
                    ubasking = basketStr.Substring(0, ifpoint4);

                    //Added "D" Intro for full completion with different grade
                    if (Strings.Left(ubasking, 1) == "P" || Strings.Left(ubasking, 1) == "D") //nancy changed
                    {
                        if (Convert.ToBoolean(_prepayManager.IsMyPrepayBasket(ubasking, tillNumber)))
                        {
                            if (Strings.Left(ubasking, 1) == "D")
                            {
                                Variables.SwitchPrepayBaskets = Variables.SwitchPrepayBaskets + ubasking; // here we are keeping only 'D' type prepay basket created because of switching the grade
                            }
                            else
                            {
                                Variables.MyPrepayBaskets = Variables.MyPrepayBaskets + ubasking;
                                result.IsFinishEnabled = true;
                            }
                        }
                    }
                    else
                    {
                        Make_Basket(ref pumps, ubasking);
                    }

                    basketStr = basketStr.Substring(ifpoint4 + 1 - 1);
                }
            }



            //nancy changed for displaying sale value for specified pumps, Jul.31th,2002
            //If fueling And DisplayFueling Then
            if (fueling || _displayReader)
            {
                ifpoint1 = (short)(udpReading.IndexOf(";", StringComparison.Ordinal) + 1);
                ifpoint1 = (short)(udpReading.IndexOf(";", ifpoint1 + 1 - 1, StringComparison.Ordinal) + 1);
                ifpoint2 = (short)(udpReading.IndexOf(";", ifpoint1 + 1 - 1, StringComparison.Ordinal) + 1);
                ifpoint3 = (short)(udpReading.IndexOf("@", StringComparison.Ordinal) + 1);

                ifpoint1++;
                ifpoint2 = (short)(ifpoint2 - ifpoint1);

                if (ifpoint3 != 0)
                {
                    result.BigPumps = ShowFueling(ref pumps, udpReading.Substring(ifpoint1 - 1, ifpoint2), udpReading.Substring(ifpoint3 + 1 - 1));
                }
                else
                {
                    result.BigPumps = ShowFueling(ref pumps, udpReading.Substring(ifpoint1 - 1, ifpoint2));
                }
            }
            iStrPoint = (short)((Variables.iPumpCount + 1).ToString().IndexOf(udpReading) + 1);
            udpReading = udpReading.Substring(iStrPoint + 1 - 1);

            Variables.LockWindowUpdate(Convert.ToInt32(false));
            result.Pumps = pumps;
            result.IsStopButtonEnabled = UDPAgent.Instance.IsConnected && pumps.Any(p => p.Status != "Stopped");
            result.IsResumeButtonEnabled = UDPAgent.Instance.IsConnected && pumps.Any(p => p.Status == "Stopped");
            CacheManager.AddAllPumps(pumps);
            CacheManager.AddAllVariablePumps(Variables.Pump);
            return result;
        }

        /// <summary>
        /// Method to show big pump action
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="stopPressed">Stop pressed</param>
        /// <param name="resumePressed">Resume pressed</param>
        /// <param name="error">Error message</param>
        /// <returns>Big pump</returns>
        public BigPump PumpAction(short pumpId, bool stopPressed, bool resumePressed, out ErrorMessage error)
        {
            BigPump bigPump = new BigPump();
            error = new ErrorMessage();
            short index = pumpId;
            var service = _fuelService.LoadService();

            string strBuffer = "";
            string strRemain = "";
            float timeIn = 0;
            string response = "";
            short i = 0;
            short ans = 0;

            if (!TCPAgent.Instance.IsConnected)
            {
                //        MsgBox ("TCP connection between FC and POS lost!")
                //Chaps_Main.DisplayMsgForm((System.Convert.ToDouble(this.Tag)) * 100 + 92, (short)99, null, (byte)0, (byte)0, "", "", "", "");
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 38, 92, null, OkMessageType)
                };
                return bigPump;
            }
            //nancy end

            //if ((VB.DateAndTime.Timer - Variables.basketClick < Variables.basket_click_delay) && (VB.DateAndTime.Timer - Variables.basketClick) > 0)
            //{
            //    // SaleMain
            //    if (Application.OpenForms.Count == 2)
            //    {
            //        SaleMain.Default.Refresh_Lines();
            //        System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            //    }
            //    //   end
            //    return;
            //}

            if (Variables.basketClick > DateAndTime.Timer)
            {
                Variables.basketClick = 0;
            }
            else
            {
                Variables.basketClick = (float)DateAndTime.Timer;
            }

            if (timeIn > DateAndTime.Timer)
            {
                timeIn = 0; //reset on midnight
            }
            else
            {
                timeIn = (float)DateAndTime.Timer;
            }

            if (stopPressed)
            {


                //if (_policyManager.StopMsg)
                //{
                //    error = new ErrorMessage
                //    {
                //        MessageStyle = _resourceManager.CreateMessage(offSet,38, (short)66, Index, OkMessageType)
                //    };
                //    if (ans != (short)MsgBoxResult.Yes)
                //    {
                //        StopPressed = false;
                //        return bigPump;
                //    }
                //}
                //

                Variables.IsWaiting = true;
                string tempCommandRenamed = "Stp" + Strings.Right("0" + Convert.ToString(index), 2);
                TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);
                response = "";
                strRemain = "";
                while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
                {
                    TCPAgent.Instance.Read_Port(false);
                    strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                    if (!string.IsNullOrEmpty(strBuffer))
                    {
                        modStringPad.SplitResponse(strBuffer, "Stp" + Strings.Right("0" + Convert.ToString(index), 2), ref response, ref strRemain);
                        if (!string.IsNullOrEmpty(response)) //got what we are waiting
                        {
                            TCPAgent.Instance.PortReading = strRemain;
                            break;
                        }
                    }
                    if (DateAndTime.Timer < timeIn)
                    {
                        timeIn = (float)DateAndTime.Timer;
                    }
                }
                Variables.IsWaiting = false;
                stopPressed = false;
                return bigPump;
            }

            var pumpStop = (byte)0;
            if ((Variables.PumpStatus[index] == (byte)1) || (Variables.PumpStatus[index] == (byte)13)) //idle, Preset idle
            {


                if (Variables.PrepayIsPressed && Variables.PumpStatus[index] == 1)
                {
                    for (i = 0; i <= Application.OpenForms.Count - 1; i++)
                    {
                        if (Application.OpenForms[i].Name == "frmPrepay")
                        {
                            //frmPrepay.Default.EnablePrepayButtons(Index);
                            break;
                        }
                    }
                }
                else
                {




                    if (!Variables.Pump[index].IsHoldPrepay && !Variables.Pump[index].IsPrepay)
                    {
                        if (_policyManager.AllowPostPay && service.PostPayEnabled && Variables.gPumps.get_Pump((byte)index).AllowPostPay && Variables.gPumps.get_Pump((byte)index).AllowPreauth && (_policyManager.AllowStack || (!_policyManager.AllowStack && !Variables.gBasket[index].CurrentFilled && !Variables.gBasket[index].StackFilled)))
                        {
                            if (Variables.Cashier_Auth[index] && !Variables.gBasket[index].StackFilled)
                            {
                                ////var tcpAgent = new TCPAgent();
                                // Check if that till can authorize the pump
                                if (_policyManager.AuthPumpPOS)
                                {
                                    if (Variables.gPumps.get_Pump((byte)index).AuthorizeFromTill == true)
                                    {
                                        string tempCommandRenamed2 = "Ath" + Strings.Right("0" + Convert.ToString(index), 2) + "0F" + "";
                                        TCPAgent.Instance.Send_TCP(ref tempCommandRenamed2, false);
                                        ////server.Send(Encoding.ASCII.GetBytes(temp_Command_Renamed2));

                                    }
                                }
                                else
                                {
                                    string tempCommandRenamed3 = "Ath" + Strings.Right("0" + Convert.ToString(index), 2) + "0F" + "";
                                    TCPAgent.Instance.Send_TCP(ref tempCommandRenamed3, false);
                                    ////server.Send(Encoding.ASCII.GetBytes(temp_Command_Renamed3));

                                }
                            }

                        }
                    }

                }
            } //calling, Preset calling
            else if ((Variables.PumpStatus[index] == (byte)2) || (Variables.PumpStatus[index] == (byte)14))
            {


                if (Variables.PrepayIsPressed && Variables.PumpStatus[index] == 2)
                {
                    for (i = 0; i <= Application.OpenForms.Count - 1; i++)
                    {
                        if (Application.OpenForms[i].Name == "frmPrepay")
                        {
                            //frmPrepay.Default.EnablePrepayButtons(Index);
                            break;
                        }
                    }
                }
                else
                {




                    if (!Variables.Pump[index].IsHoldPrepay && !Variables.Pump[index].IsPrepay)
                    {

                        if (_policyManager.AllowPostPay && service.PostPayEnabled && Variables.gPumps.get_Pump((byte)index).AllowPostPay && (_policyManager.AllowStack || (!_policyManager.AllowStack && !Variables.gBasket[index].CurrentFilled && !Variables.gBasket[index].StackFilled)))
                        {

                            //Nancy 06/13/03,add Auth for Not Variables.Cashier_auth but NotActivatedPrepay,don't auth it in FC automatically
                            if ((Variables.Cashier_Auth[index] && !Variables.gBasket[index].StackFilled) || (!Variables.Cashier_Auth[index] && !Variables.gBasket[index].StackFilled))
                            {
                                if (_policyManager.AuthPumpPOS)
                                {

                                    if (Variables.gPumps.get_Pump((byte)index).AuthorizeFromTill == true)
                                    {
                                        string tempCommandRenamed4 = "Ath" + Strings.Right("0" + Convert.ToString(index), 2) + "0F" + "";
                                        TCPAgent.Instance.Send_TCP(ref tempCommandRenamed4, false);
                                    }
                                }
                                else
                                {
                                    string tempCommandRenamed5 = "Ath" + Strings.Right("0" + Convert.ToString(index), 2) + "0F" + "";
                                    TCPAgent.Instance.Send_TCP(ref tempCommandRenamed5, false);
                                }
                                //End - Svetlana
                            }

                        }

                    }

                }
            } //authorized
            else if (Variables.PumpStatus[index] == (byte)3)
            {
                if (Variables.Cashier_Auth[index])
                {
                    string tempCommandRenamed6 = "Dau" + Strings.Right("0" + Convert.ToString(index), 2);
                    TCPAgent.Instance.Send_TCP(ref tempCommandRenamed6, false);

                }
            } //pumping, paypump pumping
            else if ((Variables.PumpStatus[index] == (byte)4) || (Variables.PumpStatus[index] == (byte)11))
            {
                pumpStop = (byte)index;
                //click pump control while pump pumping,means display this pump's sale value
                ///09/24/02,comment next line for allowing pop up big pump even when DisplayFueling=true
                //        If Not DisplayFueling Then 'if DisplayFueling=true, will display all pumps' sale value

                if (Variables.DisplayPumpID == 0)
                {
                    Variables.DisplayPumpID = (byte)index;
                    string tempCommandRenamed7 = "Brd" + Strings.Right("0" + Convert.ToString(index), 2) + "1";
                    TCPAgent.Instance.Send_TCP(ref tempCommandRenamed7, false);
                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    bigPump.IsPumpVisible = true;
                    bigPump.PumpMessage = "";
                    bigPump.Amount = _resourceManager.GetResString(offSet, (short)288);
                    bigPump.PumpId = pumpId.ToString();
                    bigPump.PumpLabel = _resourceManager.GetResString(offSet, (short)333) + " " + Strings.Right("00" + Convert.ToString(index), 2);
                }
            } //stopped
            else if (Variables.PumpStatus[index] == (byte)5)
            {
                if (resumePressed)
                {
                    Variables.IsWaiting = true;
                    string tempCommandRenamed8 = "Rsm" + Strings.Right("0" + Convert.ToString(index), 2);
                    TCPAgent.Instance.Send_TCP(ref tempCommandRenamed8, true);
                    resumePressed = false;
                    response = "";
                    strRemain = "";
                    while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
                    {
                        strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                        if (!string.IsNullOrEmpty(strBuffer))
                        {
                            modStringPad.SplitResponse(strBuffer, "Rsm" + Strings.Right("0" + Convert.ToString(index), 2), ref response, ref strRemain);
                            if (!string.IsNullOrEmpty(response)) //got what we are waiting
                            {
                                TCPAgent.Instance.PortReading = strRemain;
                                break;
                            }
                        }
                        Variables.IsWaiting = false;
                        if (DateAndTime.Timer < timeIn)
                        {
                            timeIn = (float)DateAndTime.Timer;
                        }
                        Variables.Sleep(100);
                    }
                }
            } //runaway
            else if (Variables.PumpStatus[index] == (byte)6)
            {
            } //inactive
            else if (Variables.PumpStatus[index] == (byte)7)
            {
            } //finished, paypump finished
            else if ((Variables.PumpStatus[index] == (byte)8) || (Variables.PumpStatus[index] == (byte)12))
            {
            } //Paypump Holding, Paypump Calling
            else if ((Variables.PumpStatus[index] == (byte)9) || (Variables.PumpStatus[index] == (byte)10))
            {

                if (_policyManager.CashAuthPP && Variables.Cashier_Auth[index])
                {



                    //
                    ////var tcpAgent = new TCPAgent();
                    if (_policyManager.AuthPumpPOS)
                    {
                        if (Variables.gPumps.get_Pump((byte)index).AuthorizeFromTill == true)
                        {
                            string tempCommandRenamed9 = "Ath" + Strings.Right("0" + Convert.ToString(index), 2) + "0F" + "";
                            TCPAgent.Instance.Send_TCP(ref tempCommandRenamed9, false);
                            //server.Send(Encoding.ASCII.GetBytes(tempCommandRenamed9));

                        }
                    }
                    else
                    {
                        string tempCommandRenamed10 = "Ath" + Strings.Right("0" + Convert.ToString(index), 2) + "0F" + "";
                        TCPAgent.Instance.Send_TCP(ref tempCommandRenamed10, false);
                    }
                }
                else
                {
                    _displayReader = true;
                    //click pump control while pump pumping,means display this pump's sale value

                    if (Variables.DisplayPumpID == 0)
                    {
                        var offSet = _policyManager.LoadStoreInfo().OffSet;
                        Variables.DisplayPumpID = (byte)index; //to display this pump's value in big pump
                                                               //FC is not broadcasting sale value,click this pump means want to turn it on
                        string tempCommandRenamed11 = "Brd" + Strings.Right("0" + Convert.ToString(index), 2) + "1";
                        TCPAgent.Instance.Send_TCP(ref tempCommandRenamed11, false);

                        //nancy add display pump frame initialize ,08/21/2002
                        bigPump.IsPumpVisible = true;
                        bigPump.PumpMessage = "";
                        bigPump.Amount = _resourceManager.GetResString(offSet, (short)288);
                        bigPump.PumpId = _resourceManager.GetResString(offSet, (short)333) + " " +
                                         Strings.Right("00" + Convert.ToString(index), 2);
                    }
                }
            }
            return bigPump;
        }

        /// <summary>
        /// Method get list of pump grades
        /// </summary>
        /// <param name="pumpId">Pump id</param>
        /// <returns>List of pump grades</returns>
        public List<string> LoadPumpGrades(short pumpId)
        {
            return ctlKeyPadV1_EnterPressed(pumpId);
        }

        /// <summary>
        /// Method to add fuel manually
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <param name="amountOn">Amount</param>
        /// <param name="cobGrade">Grade</param>
        /// <param name="activePump">Active pump id</param>
        /// <param name="isCashSelected">Cash selected or not</param>
        /// <returns>Sale</returns>
        public Sale AddFuelManually(int saleNumber, int tillNumber, byte registerNumber,
            string userCode, out ErrorMessage error, float amountOn, string cobGrade,
            short activePump, bool isCashSelected)
        {
            return cmdSet_ClickEvent(saleNumber, tillNumber, registerNumber,
            userCode, out error, amountOn, cobGrade, activePump, isCashSelected);
        }

        /// <summary>
        /// Method to set fuel price manually
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <param name="amountOn">Amount</param>
        /// <param name="cobGrade">Grade</param>
        /// <param name="activePump">Active pump or not</param>
        /// <param name="isCashSelected">Cash selcted or not</param>
        /// <returns>Sale</returns>
        public Sale cmdSet_ClickEvent(int saleNumber, int tillNumber, byte registerNumber,
            string userCode, out ErrorMessage error, float amountOn,
            string cobGrade, short activePump, bool isCashSelected)
        {
            Sale_Line sl = default(Sale_Line);
            error = new ErrorMessage();
            short i = 0;
            short positionId = 0;
            short gradeId = 0;
            float price = 0;
            double dblPrice = 0;
            float quantity = 0;

            if (!isCashSelected && !_policyManager.FUEL_CP)
            {
                error.MessageStyle = new MessageStyle
                {
                    Message = Utilities.Constants.InvalidRequest,
                    MessageType = MessageType.OkOnly
                };
                error.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (amountOn == 0)
            {
                //Enter Manual Fuel Amount first !
                ////Chaps_Main.DisplayMessage(this, (short)2, MsgBoxStyle.Critical, null, (byte)0);
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 55, 2, null, MessageType.Critical);
                error.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }

            if (amountOn > 999.99)
            {
                //Invalid Manual Fuel Amount!
                //Chaps_Main.DisplayMessage(this, (short)7, MsgBoxStyle.Critical, null, (byte)0);
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 55, 7, null, MessageType.Critical);
                error.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }

            if (cobGrade.Trim() == "")
            {

                //Chaps_Main.DisplayMessage(this, (short)3, MsgBoxStyle.Critical, null, (byte)0);
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 55, 3, null, MessageType.Critical);
                error.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }
            else
            {
                i = (short)(cobGrade.Trim().IndexOf("-") + 1);
                if (i > 1)
                {
                    positionId = (short)Conversion.Val(Strings.Left(cobGrade.Trim(), i - 1));
                    var pump = Variables.gPumps.get_Assignment((byte)activePump, (byte)positionId);
                    if (pump == null)
                    {
                        error.MessageStyle = new MessageStyle
                        {
                            Message = "Invalid Pump Id!",
                            MessageType = MessageType.OkOnly
                        };
                        error.StatusCode = HttpStatusCode.NotFound;
                        return null;
                    }
                    gradeId = Convert.ToInt16(pump.GradeID);
                }
                else
                {

                    //Chaps_Main.DisplayMessage(this, (short)4, MsgBoxStyle.Critical, null, (byte)0);
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 55, 4, null, MessageType.Critical);
                    error.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
            }

            if (isCashSelected)
            {
                if (Variables.gPumps.get_FuelPrice((byte)gradeId, Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).TierID), Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).LevelID)).CashPrice == 0)
                {

                    //Chaps_Main.DisplayMessage(this, (short)5, MsgBoxStyle.Critical, null, (byte)0);
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 55, 5, null, MessageType.Critical);
                    error.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
            }
            else
            {
                if (Variables.gPumps.get_FuelPrice((byte)gradeId, Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).TierID), Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).LevelID)).CreditPrice == 0)
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 55, 5, null, MessageType.Critical);
                    error.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
            }

            //this.Enabled = false;

            Chaps_Main.Transaction_Type = "Manual";

            sl = new Sale_Line();

            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return null;
            }
            //sl.PLU_Code = System.Convert.ToString(Variables.gPumps.get_Grade((byte)GradeID).Stock_Code);
            var pluCode = Convert.ToString(Variables.gPumps.get_Grade((byte)gradeId).Stock_Code);
            error = new ErrorMessage();
            _saleLineManager.SetPluCode(ref sale, ref sl, pluCode, out error);
            if (isCashSelected)
            {
                //TODO: Smriti_15 updated float parsing
                price = float.Parse(Variables.gPumps.get_FuelPrice((byte)gradeId, Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).TierID), Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).LevelID)).CashPrice.ToString("n3"));
            }
            else
            {
                //float.Parse(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Variables.gPumps.get_FuelPrice((byte)GradeID, System.Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).TierID), System.Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).LevelID)).CreditPrice, "##0.000"));
                price = float.Parse(Variables.gPumps.get_FuelPrice((byte)gradeId, Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).TierID), Convert.ToByte(Variables.gPumps.get_Pump((byte)activePump).LevelID)).CreditPrice.ToString("n3"));
            }
            //TODO: Smriti_17 updated parsing
            //dblPrice = double.Parse(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(price, "##0.000"));
            //Quantity = float.Parse(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(amountON / price, "##0.000"));
            //sl.Quantity = float.Parse(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Quantity, "##0.000"));
            //sl.Regular_Price = double.Parse(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(price, "##0.000"));
            dblPrice = double.Parse(price.ToString("n3"));
            quantity = float.Parse((amountOn / price).ToString("n3"));
            //sl.Quantity = float.Parse(Quantity.ToString("n3"));
            _saleLineManager.SetQuantity(ref sl, float.Parse(quantity.ToString("n3")));
            sl.Regular_Price = double.Parse(price.ToString("n3"));
            sl.pumpID = (byte)activePump;
            sl.PositionID = (byte)positionId;
            sl.GradeID = (byte)gradeId;
            sl.MOP = Convert.ToByte(isCashSelected ? 1 : 2);
            sl.Prepay = false;
            sl.ManualFuel = true;
            _saleManager.Line_Price(ref sale, ref sl, dblPrice);
            //sale.Line_Price(sl, dblPrice); 
            _saleManager.Add_a_Line(ref sale, sl, userCode, sale.TillNumber, out error, true);
            //     SA.Line_Price sl, dblPrice '  - moved above
            Chaps_Main.SC = sl.Stock_Code;

            sl = null;

            return sale;
        }

        /// <summary>
        /// Method to change fuel price
        /// </summary>
        public void ChangeFuelPrice()
        {

        }

        /// <summary>
        /// Method to read totalizer
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        public void ReadTotalizer(int tillNumber, ref ErrorMessage errorMessage)
        {
            cmdTot_Click(tillNumber, ref errorMessage);
        }

        /// <summary>
        /// Method to get totalizer reading
        /// </summary>
        /// <param name="errorMessage">Erroe message</param>
        public void cmdTot_Click(int tillNumber, ref ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();

            float timeIn = 0;
            string response = "";
            string strBuffer = "";
            string strRemain = "";
            string amount = "";
            string volume = "";
            short j = 0;
            short posId = 0;
            const short iMaxPos = 9;
            string highVolume = "";
            string lowVolume = "";

            short totMg = (short)(_fuelPumpService.GetMaxGroupNumberofTotalizerHistory() + 1);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            short tothlMg = _fuelPumpService.GetMaxGroupNumberofTotalHighLow();

            for (j = 1; j <= Variables.gPumps.PumpsCount; j++) //loop through all gPumps that have such grade/position
            {
                if (Variables.PumpStatus[j] != 7) //if pump is "Inactive",can't get Totalizer successful
                {
                    for (posId = 1; posId <= iMaxPos; posId++)
                    {
                        if (Variables.gPumps.get_Assignment((byte)j, (byte)posId).GradeID != 0 && Variables.gPumps.get_Assignment((byte)j, (byte)posId).GradeID != null)
                        {
                            if (TCPAgent.Instance.IsConnected)
                            {
                                response = "";
                                strRemain = "";
                                string tempCommandRenamed = "Tot" + Strings.Right("0" + Convert.ToString(j), 2) + Convert.ToString(posId);
                                TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);
                                if (timeIn > DateAndTime.Timer)
                                {
                                    timeIn = 0; //reset on midnight
                                }
                                else
                                {
                                    timeIn = (float)DateAndTime.Timer;
                                }

                                while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
                                {
                                    strBuffer = TCPAgent.Instance.NewPortReading;

                                    WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting Tot" + Strings.Right("0" + Convert.ToString(j), 2) + Convert.ToString(posId));
                                    if (!string.IsNullOrEmpty(strBuffer))
                                    {
                                        modStringPad.SplitResponse(strBuffer, "Tot" + Strings.Right("0" + Convert.ToString(j), 2) + Convert.ToString(posId), ref response, ref strRemain); //strBuffer<>""
                                        if (!string.IsNullOrEmpty(response)) //got what we are waiting
                                        {
                                            strRemain = TCPAgent.Instance.PortReading;
                                            WriteToLogFile("modify TCPAgent.PortReading from Get Totalizer: " + strRemain);
                                            break;
                                        }
                                    }
                                    Variables.Sleep(100);
                                    if (DateAndTime.Timer < timeIn)
                                    {
                                        timeIn = (float)DateAndTime.Timer;
                                    }
                                }

                                if (!(response.Length == 33) || !(Strings.Left(response, 3) == "Tot"))
                                {
                                    GetFromLastReading(j, Convert.ToInt16(Variables.gPumps.get_Assignment((byte)j, (byte)posId).GradeID), volume, ref amount);
                                }
                                else
                                {
                                    //write to database
                                    volume = response.Substring(6, 10);
                                    amount = response.Substring(16, 10);
                                }
                                _getPropertyManager.WriteTotalizer(_tillService.GetTill(tillNumber), j, posId, Convert.ToByte(Variables.gPumps.get_Assignment((byte)j, (byte)posId).GradeID), volume, amount, totMg);
                            }
                            else
                            {
                                errorMessage.MessageStyle = new MessageStyle
                                {
                                    Message = _resourceManager.GetResString(offSet, 3892),
                                    MessageType = MessageType.OkOnly
                                };
                                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                                return;
                            }

                        }
                    }
                    response = "";
                    strRemain = "";
                    string tempCommandRenamed2 = "THL" + Strings.Right("0" + Convert.ToString(j), 2);
                    TCPAgent.Instance.Send_TCP(ref tempCommandRenamed2, true);

                    if (timeIn > DateAndTime.Timer)
                    {
                        timeIn = 0; //reset on midnight
                    }
                    else
                    {
                        timeIn = (float)DateAndTime.Timer;
                    }

                    while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
                    {
                        strBuffer = TCPAgent.Instance.NewPortReading;

                        WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting THL" + Strings.Right("0" + Convert.ToString(j), 2));
                        if (!string.IsNullOrEmpty(strBuffer))
                        {
                            modStringPad.SplitResponse(strBuffer, "THL" + Strings.Right("0" + Convert.ToString(j), 2), ref response, ref strRemain); //strBuffer<>""
                            if (!string.IsNullOrEmpty(response)) //got what we are waiting
                            {
                                strRemain = TCPAgent.Instance.PortReading;
                                WriteToLogFile("modify TCPAgent.PortReading from Get Totalizer HighLow: " + strRemain);
                                break;
                            }
                        }
                        Variables.Sleep(100);
                        if (DateAndTime.Timer < timeIn)
                        {
                            timeIn = (float)DateAndTime.Timer;
                        }
                    }

                    if (!(response.Length == 27) || !(Strings.Left(response, 3) == "THL") || !(Strings.Right(response, 2) == "OK") || !(response.Substring(3, 2) == Strings.Right("0" + Convert.ToString(j), 2)))
                    {
                        highVolume = "0000000000";
                        lowVolume = "0000000000";
                    }
                    else
                    {
                        //write to database
                        highVolume = response.Substring(5, 10);
                        lowVolume = response.Substring(15, 10);
                    }
                    _getPropertyManager.WriteTotalHighLow(j, highVolume, lowVolume, tothlMg);
                }
            }
        }

        /// <summary>
        /// Method to load list of fuel prices
        /// </summary>
        /// <param name="report">Report</param>
        /// <param name="isTaxExemptionVisible">Tax exempt visible or not</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>List of fuel price</returns>
        public BasePrices LoadGroupedBasePrices(ref string report, out ErrorMessage errorMessage)
        {
            errorMessage = default(ErrorMessage);
            BasePrices prices = new BasePrices();
            GradeType[] myPrice = default(GradeType[]);
            MyGradeType[] myBaseGrade = default(MyGradeType[]);
            MyGradeType[] myLines;
            MyGradeType regularGrade = new MyGradeType();

            frmPumpGroupPrice_Load(ref myPrice, ref myBaseGrade, ref regularGrade, ref prices, ref report, out errorMessage);

            return prices;
        }

        ///Method to save base prices      
        public string SaveGroupedBasePrices(int tillNumber, List<FuelPrice> updatedPrices, bool totalizer, bool tankDip, bool priceSign, bool blTot,
            out string priceReport,
            out string fuelPriceReport,
            out ErrorMessage errorMessage,
            ref List<MessageStyle> messages)
        {
            GradeType[] myPrice = default(GradeType[]);
            errorMessage = new ErrorMessage();
            priceReport = string.Empty;
            fuelPriceReport = string.Empty;
            var caption2 = string.Empty;
            messages = new List<MessageStyle>();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            short oldReportId = 0;
            short yesNo;
            short j = 0;
            short T = 0;
            short l = 0;
            short i = 0;
            string cashPrice = "";
            string creditPrice = "";
            short posId = 0;
            byte tierLevel = 0;
            string amount = "";
            string volume = "";
            string highVolume = "";
            string lowVolume = "";
            short gradeId = 0;
            const short iMaxPos = 9;
            string response = "";
            string strBuffer = "";
            string strRemain = "";
            bool blPriceUp = false;
            User currentUser = default(User);
            bool changeFuelPriceManually = false;
            short ans = 0;
            int handleRenamed = 0;
            string strDipMessage = "";
            strDipMessage = "";
            short Index = 0;
            int reportId = 0;
            DateTime dateTime = default(DateTime);
            bool boolPrevBoolFpHo = false;
            short baseGrades = default(short);
            BasePrices prices = new BasePrices();
            MyGradeType[] myBaseGrade = default(MyGradeType[]);
            MyGradeType[] myLines = default(MyGradeType[]);
            MyGradeType regularGrade = default(MyGradeType);
            double teCashPrice = default(double);
            double teCreditPrice = default(double);


            LoadGroupBasePrices(ref prices, ref errorMessage, ref baseGrades, ref myPrice, ref myBaseGrade, ref myLines,
                ref regularGrade, ref teCashPrice, ref teCreditPrice, true);

            if (errorMessage != null && errorMessage.MessageStyle != null && !string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }

            errorMessage = new ErrorMessage();

            for (short index = 0; index < prices.FuelPrices.Count; index++)
            {
                var report = string.Empty;
                UpdatePrice(index, ref prices, ref updatedPrices, ref myPrice, ref myBaseGrade,
                    ref regularGrade, ref report, ref errorMessage);
            }

            if (errorMessage != null && errorMessage.MessageStyle != null && !string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }

            errorMessage = new ErrorMessage();
            //if Cash price is greater than Credit Price, can't set price.
            if (PriceError(myPrice))
            {
                var eMessage = _resourceManager.CreateMessage(offSet, 42, 18, null, MessageType.OkOnly);
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                errorMessage.MessageStyle = eMessage;
                return null;
            }

            if (_policyManager.TAX_EXEMPT)
            {
                if (!HaveSetAllTeCategoryForGroupPrices(myPrice))
                {
                    var eMessage = _resourceManager.CreateMessage(offSet, 42, 25, null, MessageType.OkOnly);
                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                    errorMessage.MessageStyle = eMessage;
                    return null;
                }
            }

            changeFuelPriceManually = false;

            if (!TCPAgent.Instance.IsConnected)
            {
                currentUser = Chaps_Main.User_Renamed;
                if (_policyManager.U_ManuFPrice)
                {

                    if (_policyManager.USE_FUEL)
                    {
                        currentUser = null;

                    } //###FUELPRICE
                }
                else
                {
                    var eMessage = _resourceManager.CreateMessage(offSet, 42, !_policyManager.USE_FUEL ? 69 : 64, null, MessageType.YesNo);
                    errorMessage.MessageStyle = eMessage;
                    errorMessage.StatusCode = HttpStatusCode.Conflict;
                    return null;
                }

                if (_policyManager.U_ManuFPrice)
                {
                    changeFuelPriceManually = true;
                }
                else
                {
                    var eMessage = _resourceManager.CreateMessage(offSet, 42, !_policyManager.USE_FUEL ? 70 : 65, null, MessageType.OkOnly);
                    errorMessage.MessageStyle = eMessage;
                    errorMessage.StatusCode = HttpStatusCode.Unauthorized;
                    return null;
                }
            }
            var totalizerValue = totalizer; var tankDipValue = tankDip; var PriceSign = priceSign;

            if (!_policyManager.PRICEDISPLAY)
            {
                //Go head and set up the price right now; the same as blPriceUp=False
                blPriceUp = false;
            }
            else
            {
                //Policy is true but price sign is not checked; same as blPriceUp=False
                if (!PriceSign)
                {
                    blPriceUp = false;
                }
            }

            if (_policyManager.FuelPriceChg) //Do you want to stay in Fuel Price Change Screen
            {
            }
            else
            {
                if (Chaps_Main.boolFP_HO)
                {

                }
                boolPrevBoolFpHo = Chaps_Main.boolFP_HO;
                Chaps_Main.boolFP_HO = boolPrevBoolFpHo;
            }

            dateTime = DateTime.Now;
            reportId = _getPropertyManager.Get_ReportID();

            Index = (short)0;
            float timeIn = 0;
            string[] strPrice = new string[5];
            do
            {

                for (i = 1; i <= myPrice.Length - 1; i++)
                {
                    if ((int)myPrice[i].Grade != 0 && Variables.gPumps.get_Grade(Convert.ToByte(myPrice[i].Grade)).FuelType != "O")
                    {
                        for (T = 1; T <= 2; T++)
                        {
                            for (l = 1; l <= 2; l++)
                            {
                                switch (T + Convert.ToString(l))
                                {
                                    case "11":
                                        tierLevel = (byte)0;
                                        break;
                                    case "12":
                                        tierLevel = (byte)1;
                                        break;
                                    case "21":
                                        tierLevel = (byte)2;
                                        break;
                                    case "22":
                                        tierLevel = (byte)3;
                                        break;
                                }
                                gradeId = myPrice[i].Grade;
                                cashPrice = Convert.ToString(Information.IsDBNull(myPrice[i].Price[T, l].CashP) ? 0 : myPrice[i].Price[T, l].CashP);
                                creditPrice = Convert.ToString(Information.IsDBNull(myPrice[i].Price[T, l].CreditP) ? 0 : myPrice[i].Price[T, l].CreditP);
                                if (!string.IsNullOrEmpty(cashPrice) && !string.IsNullOrEmpty(creditPrice) && double.Parse(cashPrice) != 0 && double.Parse(creditPrice) != 0)
                                {
                                    cashPrice = Strings.Right("00000" + Convert.ToString(CommonUtility.GetDoubleValue(cashPrice) * 1000), 5);
                                    creditPrice = Strings.Right("00000" + Convert.ToString(CommonUtility.GetDoubleValue(creditPrice) * 1000), 5);

                                    if (changeFuelPriceManually)
                                    {
                                        Save_GroupPrice(gradeId, T, l, reportId, dateTime, ref myPrice, ref messages);
                                    }
                                    else
                                    {
                                        if (blPriceUp == false || Index == 1) //
                                        {

                                            if (TCPAgent.Instance.IsConnected)
                                            {
                                                response = "";
                                                strRemain = "";
                                                string tempCommandRenamed = "Set" + Convert.ToString(gradeId) + Convert.ToString(tierLevel) + cashPrice + creditPrice;
                                                TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);

                                                if (timeIn > DateAndTime.Timer)
                                                {
                                                    timeIn = 0; //reset on midnight
                                                }
                                                else
                                                {
                                                    timeIn = (float)DateAndTime.Timer;
                                                }
                                                while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
                                                {
                                                    strBuffer = TCPAgent.Instance.NewPortReading;
                                                    WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting Set" + Convert.ToString(gradeId) + Convert.ToString(tierLevel));
                                                    if (!string.IsNullOrEmpty(strBuffer))
                                                    {
                                                        modStringPad.SplitResponse(strBuffer, "Set" + Convert.ToString(gradeId) + Convert.ToString(tierLevel), ref response, ref strRemain); //strBuffer<>""
                                                        if (!string.IsNullOrEmpty(response)) //got what we are waiting
                                                        {
                                                            TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                                                            WriteToLogFile("modify TCPAgent.PortReading from Set Price: " + strRemain);
                                                            break;
                                                        }
                                                    }
                                                    if (DateAndTime.Timer < timeIn)
                                                    {
                                                        timeIn = (float)DateAndTime.Timer;
                                                    }

                                                }

                                                if (!(Strings.Left(response, 7) == "Set" + Convert.ToString(gradeId) + Convert.ToString(tierLevel) + "OK"))
                                                {

                                                    goto Error1;
                                                }

                                                if (blPriceUp == false)
                                                {
                                                    Save_GroupPrice(gradeId, T, l, reportId, dateTime, ref myPrice,
                                                        ref messages); //need to save prices as it was not saved yet
                                                }
                                            }
                                            else
                                            {
                                                if (_policyManager.FuelPriceChg)
                                                {
                                                    var eMessage = _resourceManager.CreateMessage(offSet, 42, 99, null, MessageType.OkCancel);
                                                    errorMessage.MessageStyle = eMessage;
                                                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                                                    return null;

                                                }
                                                else
                                                {
                                                    fuelPriceReport = SendToPrinter(_resourceManager.GetResString(offSet, 3892));
                                                }
                                                return null;
                                            }
                                        }
                                        else
                                        {
                                            Save_GroupPrice(gradeId, T, l, reportId, dateTime, ref myPrice,
                                                ref messages);
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                if (Index == 0)
                {
                    if (_policyManager.PRICEDISPLAY)
                    {
                        if (PriceSign)
                        {
                            for (i = 1; i <= Variables.gPumps.PriceDisplayRows; i++)
                            {
                                strPrice[i] = Strings.Right("0000" + Variables.gPumps.get_FuelPrice(_getPropertyManager.get_PricesToDisplay((byte)i).GradeID, _getPropertyManager.get_PricesToDisplay((byte)i).TierID, _getPropertyManager.get_PricesToDisplay((byte)i).LevelID).CashPrice * 1000, 4);
                            }

                            caption2 = DisplayPriceViaFc(strPrice[1], strPrice[2], strPrice[3], strPrice[4]);
                        }
                    }
                }
                Index++;

                if (blPriceUp == false)
                {
                    //we have already processed gas prices and sign, so we can exit
                    Index = (short)2;
                }
            } while (Index < 2);

            if (!changeFuelPriceManually)
            {
                if (totalizerValue)
                {
                    timeIn = (float)DateAndTime.Timer;
                    while (DateAndTime.Timer - timeIn < Variables.gPumps.ReadTotDelay)
                    {

                    }

                    short totMg = (short)0;
                    if (_fuelPumpService.GetMaxGroupNumberofTotalizerHistory() != 0)
                    {
                        totMg = (short)(_fuelPumpService.GetMaxGroupNumberofTotalizerHistory() + 1);
                    }
                    short tothlMg = (short)0;
                    if (_fuelPumpService.GetMaxGroupNumberofTotalHighLow() != 0)
                    {
                        totMg = (short)(_fuelPumpService.GetMaxGroupNumberofTotalHighLow() + 1);
                    }

                    for (j = 1; j <= Variables.gPumps.PumpsCount; j++) //loop through all pumps that have such grade/position
                    {
                        if (Variables.PumpStatus[j] != 7) //if pump is "Inactive",can't get Totalizer successful
                        {
                            for (posId = 1; posId <= iMaxPos; posId++)
                            {
                                if (Variables.gPumps.get_Assignment((byte)j, (byte)posId).GradeID != 0 &&
                                    Variables.gPumps.get_Assignment((byte)j, (byte)posId).GradeID != null)
                                {
                                    response = "";
                                    strRemain = "";
                                    string tempCommandRenamed3 = "Tot" + Strings.Right("0" + Convert.ToString(j), 2) + Convert.ToString(posId);
                                    TCPAgent.Instance.Send_TCP(ref tempCommandRenamed3, true);

                                    if (timeIn > DateAndTime.Timer)
                                    {
                                        timeIn = 0; //reset on midnight
                                    }
                                    else
                                    {
                                        timeIn = (float)DateAndTime.Timer;
                                    }
                                    while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
                                    {
                                        strBuffer = TCPAgent.Instance.NewPortReading;
                                        WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting Tot" + Strings.Right("0" + Convert.ToString(j), 2) + Convert.ToString(posId));
                                        if (!string.IsNullOrEmpty(strBuffer))
                                        {
                                            modStringPad.SplitResponse(strBuffer, "Tot" + Strings.Right("0" + Convert.ToString(j), 2) + Convert.ToString(posId), ref response, ref strRemain); //strBuffer<>""
                                            if (!string.IsNullOrEmpty(response)) //got what we are waiting
                                            {
                                                TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                                                WriteToLogFile("modify TCPAgent.PortReading from Get Totalizer: " + strRemain);
                                                break;
                                            }
                                        }
                                        Variables.Sleep(100);
                                        if (DateAndTime.Timer < timeIn)
                                        {
                                            timeIn = (float)DateAndTime.Timer;
                                        }
                                    }

                                    if (!(response.Length == 33) || !(Strings.Left(response, 3) == "Tot"))
                                    {
                                        GetFromLastReading(j, Convert.ToInt16(Variables.gPumps.get_Assignment((byte)j, (byte)posId).GradeID), volume, ref amount);
                                    }
                                    else
                                    {
                                        //write to database
                                        volume = response.Substring(6, 10);
                                        amount = response.Substring(16, 10);
                                    }
                                    _getPropertyManager.WriteTotalizer(_tillService.GetTill(tillNumber), j, posId, Convert.ToByte(Variables.gPumps.get_Assignment((byte)j, (byte)posId).GradeID), volume, amount, totMg);
                                }
                            }
                            response = "";
                            strRemain = "";
                            string tempCommandRenamed4 = "THL" + Strings.Right("0" + Convert.ToString(j), 2);
                            TCPAgent.Instance.Send_TCP(ref tempCommandRenamed4, true);

                            if (timeIn > DateAndTime.Timer)
                            {
                                timeIn = 0; //reset on midnight
                            }
                            else
                            {
                                timeIn = (float)DateAndTime.Timer;
                            }
                            while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
                            {
                                strBuffer = TCPAgent.Instance.NewPortReading;
                                WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting THL" + Strings.Right("0" + Convert.ToString(j), 2));
                                if (!string.IsNullOrEmpty(strBuffer))
                                {
                                    modStringPad.SplitResponse(strBuffer, "THL" + Strings.Right("0" + Convert.ToString(j), 2), ref response, ref strRemain); //strBuffer<>""
                                    if (!string.IsNullOrEmpty(response)) //got what we are waiting
                                    {
                                        TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                                        WriteToLogFile("modify TCPAgent.PortReading from Get Totalizer HighLow: " + strRemain);
                                    }
                                }
                                Variables.Sleep(100);

                                if (DateAndTime.Timer < timeIn)
                                {
                                    timeIn = (float)DateAndTime.Timer;
                                }
                            }

                            if (!(response.Length == 27) || !(Strings.Left(response, 3) == "THL") || !(Strings.Right(response, 2) == "OK") || !(response.Substring(3, 2) == Strings.Right("0" + Convert.ToString(j), 2)))
                            {
                                highVolume = "0000000000";
                                lowVolume = "0000000000";
                            }
                            else
                            {
                                //write to database
                                highVolume = response.Substring(5, 10);
                                lowVolume = response.Substring(15, 10);
                            }
                            _getPropertyManager.WriteTotalHighLow(j, highVolume, lowVolume, tothlMg);
                        }
                    }
                }

                if (tankDipValue) //
                {
                    Variables.ReadTankDipSuccess = ReadTankDip(tillNumber, out errorMessage);
                    if (!Variables.ReadTankDipSuccess)
                    {
                        strDipMessage = _resourceManager.GetResString(offSet, 8398);
                    }
                }

            }

            prices.Caption = _resourceManager.CreateCaption(offSet, 14, 42, null, 2);
            if (_policyManager.FuelPriceChg)
            {
                prices.Caption = _resourceManager.CreateCaption(offSet, 14, 42, null, (short)2) + " " + strDipMessage;
            }
            else
            {
                //TODO:Udham
                prices.FuelPrices = prices.FuelPrices.Where(x => x.Row != 0).ToList();
                var reptId = (short)Variables.gPumps.get_FuelPrice((byte)prices.FuelPrices.FirstOrDefault().GradeId, (byte)prices.FuelPrices.FirstOrDefault()?.TierId, (byte)prices.FuelPrices.FirstOrDefault()?.LevelId).ReportID;
                priceReport = cmdPrint_Click(ref myPrice, prices.IsReadTotalizerChecked, prices.IsReadTankDipChecked, reptId);

                //Done successfully, send the price change to the printer
                if (!string.IsNullOrEmpty(strDipMessage))
                {

                    fuelPriceReport = SendToPrinter(strDipMessage);
                }
            }
            if (changeFuelPriceManually)
            {
                if (_policyManager.FuelPriceChg)
                {
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 42, 66, null, MessageType.OkOnly);
                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                }
                else
                {
                    //Send to printer
                    fuelPriceReport = SendToPrinter(_resourceManager.GetResString(offSet, 4266));
                }
            }
            object dblRecNo = 0;
            if (Chaps_Main.boolFP_HO)
            {
                if (_policyManager.FuelPriceChg)
                {
                    prices.IsErrorEnabled = true;
                }
                _fuelPumpService.DeleteFuelPrice();
                modGlobalFunctions.BoolFuelPriceApplied = true;
                WriteToLogFile(dblRecNo + " records deleted from FuelPrice_HO table");
            }

            return string.Format("{0};{1}", prices.Caption, caption2);

            Error1:
            if (_policyManager.FuelPriceChg)
            {
                prices.Caption = _resourceManager.CreateCaption(offSet, 14, 42, null, 3);
                return string.Format("{0};{1}", prices.Caption, caption2);
            }
            else
            {

            }

            prices.IsExitEnabled = true;
            prices.Caption = _resourceManager.CreateCaption(offSet, 14, 42, null, 3);
            return string.Format("{0};{1}", prices.Caption, caption2);
        }

        /// <summary>
        /// Method to update price
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="prices">Fuel price</param>
        /// <param name="updatedPrices">Updated fuel prices</param>
        /// <param name="myPrice">Grade type</param>
        /// <param name="myBaseGrade">My grade type</param>
        /// <param name="regularGrade">Regular grade</param>
        /// <param name="errorMessage">Error message</param>
        public void UpdatePrice(short index, ref BasePrices prices, ref List<FuelPrice> updatedPrices, ref GradeType[] myPrice,
            ref MyGradeType[] myBaseGrade, ref MyGradeType regularGrade, ref string report, ref ErrorMessage errorMessage)
        {
            var price = prices.FuelPrices[index];
            var updatedPrice = updatedPrices.FirstOrDefault(x => x.Grade == price.Grade);
            if (updatedPrice != null)
            {

                if (price.CashPrice != updatedPrice.CashPrice)
                {
                    ctlKeyPadV1_EnterPressed(ref myPrice, ref myBaseGrade, ref regularGrade,
                    ref prices, updatedPrice.CashPrice, (byte)1, (short)(index + 1), ref report, ref errorMessage);
                    if (errorMessage != null && errorMessage.MessageStyle != null && !string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                    {
                        return;
                    }
                }
                else if (price.CreditPrice != updatedPrice.CreditPrice)
                {
                    ctlKeyPadV1_EnterPressed(ref myPrice, ref myBaseGrade, ref regularGrade,
                    ref prices, updatedPrice.CreditPrice, (byte)2, (short)(index + 1), ref report, ref errorMessage);
                    if (errorMessage != null && errorMessage.MessageStyle != null && !string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                    {
                        return;
                    }
                }
                GetTaxExemption(ref prices);
                if (prices.IsTaxExemptionVisible)
                {

                    if (price.TaxExemptedCashPrice != updatedPrice.TaxExemptedCashPrice)
                    {
                        ctlKeyPadV1_EnterPressed(ref myPrice, ref myBaseGrade, ref regularGrade,
                        ref prices, updatedPrice.TaxExemptedCashPrice, (byte)3, (short)(index + 1), ref report, ref errorMessage);
                        if (errorMessage != null && errorMessage.MessageStyle != null && !string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                        {
                            return;
                        }
                    }
                    else if (price.TaxExemptedCreditPrice != updatedPrice.TaxExemptedCreditPrice)
                    {
                        ctlKeyPadV1_EnterPressed(ref myPrice, ref myBaseGrade, ref regularGrade,
                         ref prices, updatedPrice.TaxExemptedCreditPrice, (byte)4, (short)(index + 1), ref report, ref errorMessage);
                        if (errorMessage != null && errorMessage.MessageStyle != null && !string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                        {
                            return;
                        }
                    }
                }
            }
        }

        public string cmdPrint_Click(ref GradeType[] myPrice, bool isReadTotalizer, bool isReadTankDip, int oldReportId)
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            short iGradeId = 0;
            string fileName = "";
            short i = 0;
            short j = 0;
            object oldPrice;
            object newPrice;
            //On Error GoTo err1
            short m = 0;
            string tmpstr = "";

            tmpstr = "";
            tmpstr = "\r\n" + _resourceManager.GetResString(offSet, (short)358) + "\r\n" + "\r\n";
            for (i = 1; i <= 2; i++)
            {
                for (j = 1; j <= 2; j++)
                {
                    tmpstr = tmpstr + Convert.ToString(Variables.gPumps.get_Tier((byte)i)) + " & " + Convert.ToString(Variables.gPumps.get_Level((byte)j)) + " :" + "\r\n";
                    tmpstr = tmpstr + "  " + _resourceManager.GetResString(offSet, (short)359) + "\r\n";

                    if (_policyManager.TAX_EXEMPT)
                    {
                        tmpstr = tmpstr + "   " + _resourceManager.GetResString(offSet, (short)375) + "\r\n";
                    }

                    for (m = 1; m <= Variables.gPumps.GradesCount; m++)
                    {
                        if (Variables.gPumps.get_Grade((byte)m).FuelType != "O" && Variables.gPumps.get_GradeIsExist((byte)m))
                        {
                            if (!_policyManager.TAX_EXEMPT)
                            {


                                tmpstr = tmpstr + " " + Strings.Left(Variables.gPumps.get_Grade((byte)m).FullName + ":             ", 15) + CommonUtility.GetDoubleValue(myPrice[m].Price[i, j].CashP).ToString("0.000") + "       " + CommonUtility.GetDoubleValue(myPrice[m].Price[i, j].CreditP).ToString("0.000") + "\r\n";
                            }
                            else
                            {
                                tmpstr = tmpstr + Strings.Left(Variables.gPumps.get_Grade((byte)m).FullName + ":        ", 9) + "   " + CommonUtility.GetDoubleValue(myPrice[m].Price[i, j].CashP).ToString("0.000") + "/" + CommonUtility.GetDoubleValue(myPrice[m].Price[i, j].TECashP).ToString("0.000") + "    " + CommonUtility.GetDoubleValue(myPrice[m].Price[i, j].CreditP).ToString("0.000") + "/" + CommonUtility.GetDoubleValue(myPrice[m].Price[i, j].TECreditP).ToString("0.000") + "\r\n";
                            }
                        }
                    }
                    tmpstr = tmpstr + "\r\n";
                }
            }

            string timeFormat;
            string timeFormats;
            if (_policyManager.TIMEFORMAT == "24 HOURS")
            {
                timeFormat = "hh:mm";
                timeFormats = "hh:mm:ss";
            }
            else
            {
                timeFormat = "hh:mm tt";
                timeFormats = "hh:mm:ss tt";
            }

            if (isReadTotalizer)
            {
                tmpstr = tmpstr + "\r\n" + modStringPad.PadC(_resourceManager.GetResString(offSet, (short)372), modPrint.PRINT_WIDTH) + "\r\n" + "\r\n";
                tmpstr = tmpstr + modStringPad.PadC(DateAndTime.Today.Date.ToString("dd-MMM-yyyy") + " " + DateAndTime.TimeOfDay.ToString(timeFormat), modPrint.PRINT_WIDTH) + "\r\n" + "\r\n"; //  
                tmpstr = tmpstr + modStringPad.PadC("Pump/Grade", (short)12) + modStringPad.PadC("Volume", (short)14) + modStringPad.PadC("Amount", (short)14) + "\r\n"; //& vbCrLf
                tmpstr = tmpstr + modStringPad.PadC("-", modPrint.PRINT_WIDTH, "-") + "\r\n";
                var totMg = _fuelPumpService.GetMaxGroupNumberofTotalizerHistory();
                var rs = _fuelPumpService.GetTotalizerHist(totMg, 0, 0);
                foreach (var record in rs)
                {
                    tmpstr = tmpstr + modStringPad.PadC(record.PumpId + "/" +
                        Convert.ToString(Variables.gPumps.get_Grade(Convert.ToByte(record.Grade)).FullName), (short)12) + modStringPad.PadL(record.Volume.ToString("#0.000"), (short)14) + modStringPad.PadL(record.Dollars.ToString("#0.000"), (short)14) + "\r\n";
                }

                tmpstr = tmpstr + "\r\n";


                var totalHist = _tillCloseService.GetTotalReading(totMg);
                var groupedResults = rs.GroupBy(x => x.Grade,
                    (key, g) => new TotalizerHist
                    {
                        Grade = key,
                        Dollars = g.Sum(y => y.Dollars),
                        Volume = g.Sum(y => y.Volume)
                    });
                tmpstr = tmpstr + modStringPad.PadC(_resourceManager.GetResString(offSet, (short)332), (short)12) + modStringPad.PadL(_resourceManager.CreateCaption(offSet, (short)64, (short)46, null, (short)0), (short)14) + modStringPad.PadL(_resourceManager.CreateCaption(offSet, (short)65, (short)46, null, (short)0), (short)14) + "\r\n";
                tmpstr = tmpstr + modStringPad.PadC("-", modPrint.PRINT_WIDTH, "-") + "\r\n";
                foreach (var groupedResult in groupedResults)
                {

                    iGradeId = Convert.ToInt16(Information.IsDBNull(groupedResult.Grade) ? 0 : groupedResult.Grade);
                    if (iGradeId > 0 && !string.IsNullOrEmpty(Variables.gPumps.get_Grade((byte)iGradeId).FullName))
                    {
                        tmpstr = tmpstr + modStringPad.PadC(Convert.ToString(Variables.gPumps.get_Grade((byte)iGradeId).FullName), (short)12) + modStringPad.PadL(groupedResult.Volume.ToString("0.000"), (short)14) + modStringPad.PadL(groupedResult.Dollars.ToString("0.000"), (short)14) + "\r\n";
                    }
                }

            }

            if (isReadTankDip)
            {
                tmpstr = tmpstr + GetTankDipReport(Variables.ReadTankDipSuccess);
            }

            fileName = new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase().Info.DirectoryPath + "/Price.txt";
            Variables.DeleteFile(fileName);

            var nH = string.Empty;

            nH += "\n";
            nH += "\n" + _resourceManager.GetResString(offSet, (short)289) + Convert.ToString(oldReportId + 1); //  "        Fuel Price Change Report:"
            nH += "\n" + new string('-', 40);
            nH += "\n";
            nH += "\n" + _resourceManager.GetResString(offSet, (short)290) + Convert.ToString(DateAndTime.Today.ToString("dd-MM-yyyy")) + " " + DateAndTime.TimeOfDay.ToString(timeFormats); //  Date & Time:   '  
            nH += "\n";
            nH += "\n" + _resourceManager.GetResString(offSet, (short)291) + UserCode; // Employee.EmployeeID ' Employee #:
            nH += "\n";

            nH += "\n" + tmpstr;
            nH += "\n";
            nH += "\n";
            nH += "\n";
            nH += "\n";
            nH += "\n";

            return nH;
        }


        /// <summary>
        /// Method to verify base prices
        /// </summary>
        /// <param name="updatedPrices">Updated prices</param>
        /// <param name="errorMessage">Error message</param>
        public void VerifyGroupedBasePrices(List<FuelPrice> updatedPrices, ref ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();
            GradeType[] myPrice = default(GradeType[]);
            var prices = new BasePrices();
            short baseGrades = default(short);
            MyGradeType[] myBaseGrade = default(MyGradeType[]);
            MyGradeType[] myLines = default(MyGradeType[]);
            MyGradeType regularGrade = default(MyGradeType);
            double teCashPrice = default(double);
            double teCreditPrice = default(double);

            LoadGroupBasePrices(ref prices, ref errorMessage,
                ref baseGrades, ref myPrice, ref myBaseGrade, ref myLines,
                ref regularGrade, ref teCashPrice, ref teCreditPrice, true);

            if (errorMessage != null && errorMessage.MessageStyle != null && !string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return;
            }

            var report = string.Empty;
            for (short index = 0; index < prices.FuelPrices.Count; index++)
            {
                var price = prices.FuelPrices[index];
                var updatedPrice = updatedPrices.FirstOrDefault(x => x.Grade == price.Grade);

                if (updatedPrice != null)
                {
                    if (price.CashPrice != updatedPrice.CashPrice)
                    {
                        ctlKeyPadV1_EnterPressed(ref myPrice, ref myBaseGrade, ref regularGrade,
                        ref prices, updatedPrice.CashPrice, (byte)1, (short)(index + 1), ref report, ref errorMessage);
                        if (errorMessage != null && errorMessage.MessageStyle != null && !string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                        {
                            return;
                        }
                    }
                    if (price.CreditPrice != updatedPrice.CreditPrice)
                    {
                        ctlKeyPadV1_EnterPressed(ref myPrice, ref myBaseGrade, ref regularGrade,
                        ref prices, updatedPrice.CreditPrice, (byte)2, (short)(index + 1), ref report, ref errorMessage);
                        if (errorMessage != null && errorMessage.MessageStyle != null && !string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                        {
                            return;
                        }
                    }
                    bool isTaxExempted = false;
                    GetTaxExemption(ref prices);
                    if (isTaxExempted)
                    {
                        if (price.TaxExemptedCashPrice != updatedPrice.TaxExemptedCashPrice)
                        {
                            ctlKeyPadV1_EnterPressed(ref myPrice, ref myBaseGrade, ref regularGrade,
                            ref prices, updatedPrice.TaxExemptedCashPrice, (byte)3, (short)(index + 1), ref report, ref errorMessage);
                            if (errorMessage != null && errorMessage.MessageStyle != null && !string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                            {
                                return;
                            }
                        }
                        if (price.TaxExemptedCreditPrice != updatedPrice.TaxExemptedCreditPrice)
                        {
                            ctlKeyPadV1_EnterPressed(ref myPrice, ref myBaseGrade, ref regularGrade,
                             ref prices, updatedPrice.TaxExemptedCreditPrice, (byte)4, (short)(index + 1), ref report, ref errorMessage);
                            if (errorMessage != null && errorMessage.MessageStyle != null && !string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                            {
                                return;
                            }
                        }
                    }
                }
            }

            if (PriceError(myPrice))
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                var message = _resourceManager.CreateMessage(offSet, 42, 18, null, MessageType.OkOnly);
                errorMessage = new ErrorMessage();
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                errorMessage.MessageStyle = message;

                return;
            }

            if (_policyManager.TAX_EXEMPT)
            {
                if (!HaveSetAllTeCategoryForGroupPrices(myPrice))
                {

                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    var message = _resourceManager.CreateMessage(offSet, 42, 25, null, MessageType.OkOnly);
                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                    errorMessage.MessageStyle = message;

                    return;
                }
            }
            if (!TCPAgent.Instance.IsConnected)
            {
                if (_policyManager.U_ManuFPrice)
                {
                    if (_policyManager.USE_FUEL) // ###FUELPRICE
                    {
                        var offSet = _policyManager.LoadStoreInfo().OffSet;
                        var message = _resourceManager.CreateMessage(offSet, 42, 63, null, MessageType.YesNo);
                        errorMessage.MessageStyle = message;
                        errorMessage.StatusCode = HttpStatusCode.Conflict;

                        return;
                    } //###FUELPRICE
                }
                else
                {
                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    var message = _resourceManager.CreateMessage(offSet, 42, !_policyManager.USE_FUEL ? 69 : 64, null, MessageType.YesNo);
                    errorMessage.MessageStyle = message;
                    errorMessage.StatusCode = HttpStatusCode.Conflict;
                    return;
                }

                if (_policyManager.U_ManuFPrice)
                {
                    //ChangeFuelPriceManually = true;
                }
                else
                {
                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    var message = _resourceManager.CreateMessage(offSet, 42, !_policyManager.USE_FUEL ? 70 : 65, null, MessageType.OkOnly);
                    errorMessage.MessageStyle = message;
                    errorMessage.StatusCode = HttpStatusCode.Unauthorized;
                }
            }
        }

        /// <summary>
        /// Method to upadte pump Info
        /// </summary>
        /// <param name="myPrice">Orignal price</param>
        /// <param name="myBaseGrade">Base grade</param>
        /// <param name="regularGrade">Regular grade</param>
        /// <param name="prices">Fuel Prices</param>
        /// <param name="amount">Amount</param>
        /// <param name="txtPrice">Entered price</param>
        /// <param name="curLine">Current line</param>
        /// <param name="errorMessage">Error</param>
        public void ctlKeyPadV1_EnterPressed(ref GradeType[] myPrice, ref MyGradeType[] myBaseGrade, ref MyGradeType regularGrade,
            ref BasePrices prices, string amount, byte txtPrice, short curLine, ref string report, ref ErrorMessage errorMessage)
        {
            string keyEntry = amount;
            errorMessage = new ErrorMessage();

            try
            {
                Convert.ToDecimal(keyEntry);
            }
            catch (Exception)
            {
                errorMessage.MessageStyle = new MessageStyle { Message = Utilities.Constants.InvalidRequest, MessageType = 0 };
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                return;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (Conversion.Val((string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000")) > 9.999)
            {
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 42, 19, null, MessageType.OkOnly);
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                return;
            }

            if (Conversion.Val((string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000")) <= 0)
            {
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 42, 22, null, MessageType.OkOnly);
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                //
                return;
            }

            if (txtPrice == (byte)1)
            {
                if (!(CommonUtility.GetDoubleValue(myPrice[myBaseGrade[curLine].GradeId].Price[1, 1].CashP) - _policyManager.FUEL_MAXTH <= CommonUtility.GetDoubleValue(keyEntry)
                     && CommonUtility.GetDoubleValue(myPrice[myBaseGrade[curLine].GradeId].Price[1, 1].CashP) + _policyManager.FUEL_MAXTH >= CommonUtility.GetDoubleValue(keyEntry)))
                {
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 2193, _policyManager.FUEL_MAXTH.ToString("$#0.000"), MessageType.OkOnly);
                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                    return;
                }
                prices.FuelPrices[curLine - 1].CashPrice = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000");
                myPrice[myBaseGrade[curLine].GradeId].Price[1, 1].CashP = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000");
                myBaseGrade[curLine].Price[1, 1].CashP = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000");
                if (myBaseGrade[curLine].GradeId == regularGrade.GradeId)
                {
                    regularGrade.Price[1, 1].CashP = myBaseGrade[curLine].Price[1, 1].CashP;
                }

                if (!_policyManager.FUEL_CP)
                {
                    prices.FuelPrices[curLine - 1].CreditPrice = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000");
                    myPrice[myBaseGrade[curLine].GradeId].Price[1, 1].CreditP = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000");
                    myBaseGrade[curLine].Price[1, 1].CreditP = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000");
                    if (myBaseGrade[curLine].GradeId == regularGrade.GradeId)
                    {
                        regularGrade.Price[1, 1].CreditP = myBaseGrade[curLine].Price[1, 1].CreditP;
                    }
                }
            }
            else if (txtPrice == (byte)2)
            {
                if (!(CommonUtility.GetDoubleValue(myPrice[myBaseGrade[curLine].GradeId].Price[1, 1].CreditP) - _policyManager.FUEL_MAXTH <= CommonUtility.GetDoubleValue(keyEntry)
                   && CommonUtility.GetDoubleValue(myPrice[myBaseGrade[curLine].GradeId].Price[1, 1].CreditP) + _policyManager.FUEL_MAXTH >= CommonUtility.GetDoubleValue(keyEntry)))
                {
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 2193, _policyManager.FUEL_MAXTH.ToString("$#0.000"), MessageType.OkOnly);
                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                    return;
                }
                prices.FuelPrices[curLine - 1].CreditPrice = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000");
                myPrice[myBaseGrade[curLine].GradeId].Price[1, 1].CreditP = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000");
                myBaseGrade[curLine].Price[1, 1].CreditP = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000");
                if (myBaseGrade[curLine].GradeId == regularGrade.GradeId)
                {
                    regularGrade.Price[1, 1].CreditP = myBaseGrade[curLine].Price[1, 1].CreditP;
                }
            }
            else if (txtPrice == (byte)3)
            {
                if (!(myPrice[myBaseGrade[curLine].GradeId].Price[1, 1].TECashP - _policyManager.FUEL_MAXTH <= double.Parse(keyEntry) && myPrice[myBaseGrade[curLine].GradeId].Price[1, 1].TECashP + _policyManager.FUEL_MAXTH >= double.Parse(keyEntry)))
                {
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 2193, _policyManager.FUEL_MAXTH.ToString("$#0.000"), MessageType.OkOnly);
                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                    return;
                }
                prices.FuelPrices[curLine - 1].TaxExemptedCashPrice = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000");
                myPrice[myBaseGrade[curLine].GradeId].Price[1, 1].TECashP = Convert.ToDouble(Convert.ToDouble((string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000")));
                myBaseGrade[curLine].Price[1, 1].TECashP = Convert.ToDouble(Convert.ToDouble((string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000")));
                if (myBaseGrade[curLine].GradeId == regularGrade.GradeId)
                {
                    regularGrade.Price[1, 1].TECashP = myBaseGrade[curLine].Price[1, 1].TECashP;
                }

                if (!_policyManager.FUEL_CP)
                {
                    prices.FuelPrices[curLine - 1].TaxExemptedCreditPrice = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000");
                    myPrice[myBaseGrade[curLine].GradeId].Price[1, 1].TECreditP = Convert.ToDouble(Convert.ToDouble((string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000")));
                    myBaseGrade[curLine].Price[1, 1].TECreditP = Convert.ToDouble((string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000"));
                    if (myBaseGrade[curLine].GradeId == regularGrade.GradeId)
                    {
                        regularGrade.Price[1, 1].TECreditP = myBaseGrade[curLine].Price[1, 1].TECreditP;
                    }
                }
            }
            else if (txtPrice == (byte)4)
            {
                if (!(myPrice[myBaseGrade[curLine].GradeId].Price[1, 1].TECreditP - _policyManager.FUEL_MAXTH <= double.Parse(keyEntry) && myPrice[myBaseGrade[curLine].GradeId].Price[1, 1].TECreditP + _policyManager.FUEL_MAXTH >= double.Parse(keyEntry)))
                {
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 2193, _policyManager.FUEL_MAXTH.ToString("$#0.000"), MessageType.OkOnly);
                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                    return;
                }
                prices.FuelPrices[curLine - 1].TaxExemptedCreditPrice = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000");
                myPrice[myBaseGrade[curLine].GradeId].Price[1, 1].TECreditP = Convert.ToDouble(Convert.ToDouble((string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000")));
                myBaseGrade[curLine].Price[1, 1].TECreditP = Convert.ToDouble(Convert.ToDouble((string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDecimalValue(keyEntry)).ToString("0.000")));
                if (myBaseGrade[curLine].GradeId == regularGrade.GradeId)
                {
                    regularGrade.Price[1, 1].TECreditP = myBaseGrade[curLine].Price[1, 1].TECreditP;
                }
            }

            CalculatePrice(ref regularGrade, ref myPrice, ref errorMessage);
            DisplayReport(ref myPrice, ref report);
            prices.IsExitEnabled = true;
        }

        /// <summary>
        /// Method to stop all pumps
        /// </summary>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        public bool StopAllPumps(out ErrorMessage error)
        {
            error = new ErrorMessage();
            short i = 0;
            string response = "";
            string strBuffer = "";
            string strRemain = "";
            short ans = 0;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!TCPAgent.Instance.IsConnected)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = _resourceManager.GetResString(offSet, 3892),
                        MessageType = OkMessageType
                    }
                };
                return false;
            }
            for (i = 1; i <= Variables.gPumps.PumpsCount; i++)
            {
                response = "";
                strRemain = "";
                Variables.IsWaiting = true;
                string tempCommandRenamed = "Stp" + Strings.Right("0" + Convert.ToString(i), 2);
                TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);
                var timeIn = (float)DateAndTime.Timer;

                while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
                {
                    strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                    if (!string.IsNullOrEmpty(strBuffer))
                    {
                        modStringPad.SplitResponse(strBuffer, "Stp" + Strings.Right("0" + Convert.ToString(i), 2), ref response, ref strRemain);
                        if (!string.IsNullOrEmpty(response)) //got what we are waiting
                        {
                            TCPAgent.Instance.PortReading = strRemain;
                            break;
                        }
                    }
                    Variables.Sleep(100);
                    if (DateAndTime.Timer < timeIn)
                    {
                        timeIn = (float)DateAndTime.Timer;
                    }
                }
                Variables.IsWaiting = false;
            }

            return true;
        }

        /// <summary>
        /// Method to resume all pumps
        /// </summary>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        public bool ResumeAllPumps(out ErrorMessage error)
        {
            error = new ErrorMessage();
            short i = 0;
            bool needResume = false;
            string response = "";
            string strBuffer = "";
            string strRemain = "";
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!TCPAgent.Instance.IsConnected)
            {
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 38, 92, null, OkMessageType)
                };
                return false;
            }

            if (Variables.basketClick > DateAndTime.Timer)
            {
                Variables.basketClick = 0;
            }
            else
            {
                Variables.basketClick = (float)DateAndTime.Timer;
            }

            for (i = 1; i <= Variables.gPumps.PumpsCount; i++)
            {
                if (Variables.PumpStatus[i] == double.Parse("5") || Variables.PumpStatus[i] == double.Parse("7"))
                {
                    needResume = true;
                }
            }
            if (!needResume)
            {
                return false;
            }
            float timeIn = 0;
            for (i = 1; i <= Variables.gPumps.PumpsCount; i++)
            {
                response = "";
                strRemain = "";

                Variables.IsWaiting = true;
                string tempCommandRenamed = "Rsm" + Strings.Right("0" + Convert.ToString(i), 2);
                //var tcpAgent = new TCPAgent();
                TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);
                if (timeIn > DateAndTime.Timer)
                {
                    timeIn = 0; //reset on midnight
                }
                else
                {
                    timeIn = (float)DateAndTime.Timer;
                }

                while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
                {
                    strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                    if (!string.IsNullOrEmpty(strBuffer))
                    {
                        modStringPad.SplitResponse(strBuffer, "Rsm" + Strings.Right("0" + Convert.ToString(i), 2), ref response, ref strRemain);
                        if (!string.IsNullOrEmpty(response)) //got what we are waiting
                        {
                            TCPAgent.Instance.PortReading = strRemain;
                            break;
                        }
                    }
                    Variables.Sleep(100);
                    if (DateAndTime.Timer < timeIn)
                    {
                        timeIn = (float)DateAndTime.Timer;
                    }
                }
                Variables.IsWaiting = false;
            }
            return true;
        }

        /// <summary>
        /// Method to get list of fuel prices
        /// </summary>
        /// <param name="fuelPrices">Fuel prices</param>
        /// <param name="report">Report</param>
        /// <param name="isTaxExemptionVisible">Tax exempt visible or not</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>List of fuel prices</returns>
        public List<FuelPrice> SetGroupedBasePrice(List<FuelPrice> fuelPrices, int row, ref string report, out ErrorMessage errorMessage)
        {
            errorMessage = default(ErrorMessage);
            var prices = new BasePrices();
            GradeType[] myPrice = default(GradeType[]);
            MyGradeType[] myBaseGrade = default(MyGradeType[]);
            MyGradeType[] myLines;
            MyGradeType regularGrade = new MyGradeType();

            frmPumpGroupPrice_Load(ref myPrice, ref myBaseGrade, ref regularGrade,
                ref prices, ref report, out errorMessage);

            var updatedFuelPrices = fuelPrices;

            var price = prices.FuelPrices.FirstOrDefault(x => x.Grade == fuelPrices.FirstOrDefault(y => y.Row == row).Grade);
            UpdatePrice((short)prices.FuelPrices.IndexOf(price), ref prices, ref updatedFuelPrices, ref myPrice,
                ref myBaseGrade, ref regularGrade, ref report, ref errorMessage);

            return prices.FuelPrices;
        }

        /// <summary>
        /// Method to add fuel sale from basket
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="pumpId">Pump id</param>
        /// <param name="basketValue">Basket value</param>
        /// <param name="error">Error</param>
        /// <returns>Sale</returns>
        public Sale AddFuelSaleFromBasket(int saleNumber, int tillNumber, byte registerNumber, short pumpId, float basketValue, out ErrorMessage error)
        {
            WriteToLogFile(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "AddFuelSaleFromBasket METHOD INVOKED");
           // WriteToLogFile("AddFuelSaleFromBasket invoked");
            short index = (short)pumpId;

            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, UserCode, out error);
            error = new ErrorMessage();
            var pumps = CacheManager.GetAllPumps();

            var offSet = _policyManager.LoadStoreInfo().OffSet;
            Sale_Line sl = new Sale_Line();
            double dblPrice = 0;
            float timeIn = 0;
            char[] baskId = new char[3];
            string response = "";
            string strBuffer = "";
            string strRemain = "";
            short i;
            bool boolSent = false; //   to check if the basket was sent to BreakPoint

            if (timeIn > DateAndTime.Timer)
            {
                timeIn = 0; //reset on midnight
            }
            else
            {
                timeIn = (float)DateAndTime.Timer;
            }

            if (Variables.basketClick > DateAndTime.Timer)
            {
                Variables.basketClick = 0;
            }
            else
            {
                Variables.basketClick = (float)DateAndTime.Timer;
            }

            if (!_policyManager.FUELONLY) //   added Not Policy.FUELONLY; there is no prepay for FuelOnly
            {
                if (sale.DeletePrepay)
                {
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)1150, null, OkMessageType)
                    };
                    return null;
                }
            }

            Variables.LockUdp = true;
            Variables.LockUdpDate = DateTime.Now;
            if (!TCPAgent.Instance.IsConnected)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = _resourceManager.GetResString(offSet, 3892),
                        MessageType = OkMessageType
                    }
                };
                Variables.LockUdp = false; //to unlock UDP, otherwise next time when connection comes again, can't get "FCSTART"
                return null;
            }

            if (Variables.Return_Mode)
            {
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)1155, null, OkMessageType)
                };

                Variables.LockUdp = false; //to unlock UDP, otherwise next time when connection comes again, can't get "FCSTART"
                return null;
            }

            if (Variables.gBasket[index].AmountCurrent == basketValue)
            {

                pumps[index - 1].EnableBasketBotton = false;

                if (_policyManager.FUELONLY && Variables.gBasket[index].AmountCurrent > 0)
                {
                    boolSent = Send_Sale(index, false, out error);
                    if (string.IsNullOrEmpty(error?.MessageStyle?.Message))
                    {
                        return null;
                    }
                    if (!boolSent)
                    {
                        if (!string.IsNullOrEmpty(Variables.UDPonLock))
                        {
                            ReadUdp(Variables.UDPonLock); //force to refresh UDP due to Doevents problem
                        }
                        Variables.LockUdp = false;
                        Variables.IsWaiting = false;
                        pumps[index - 1].EnableBasketBotton = true;
                        return null;
                    }
                }

                if (Variables.gBasket[index].currBaskID == "0")
                {
                    return sale;
                }

                Variables.IsWaiting = true;
                string tempCommandRenamed = "Rmv" + Convert.ToString(Variables.gBasket[index].currBaskID);
                TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);
                baskId = Variables.gBasket[index].currBaskID.ToString().ToCharArray();

                response = "";
                strRemain = "";
                WriteToLogFile("Time in " + timeIn);
                while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
                {
                    strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                    //WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting Rmv" + new string(baskId));
                    if (!string.IsNullOrEmpty(strBuffer))
                    {
                       
                        
                        modStringPad.SplitResponse(strBuffer, "Rmv" + new string(baskId), ref response, ref strRemain); //strBuffer<>""

                        WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 2663"+ Variables.gPumps.CommunicationTimeOut.ToString());
                        if (!string.IsNullOrEmpty(response)) //got what we are waiting
                        {
                            TCPAgent.Instance.PortReading = strRemain;
                            //WriteToLogFile("modify PortReading from remove Basket: " + strRemain);
                            break;
                        }
                        WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 2670");
                 
                    }
                    if (DateAndTime.Timer < timeIn)
                    {
                        timeIn = (float)DateAndTime.Timer;
                    }
                    Variables.Sleep(100);
                    break;
                }


                //don't allow user click this button before getting response
                pumps[index - 1].EnableBasketBotton = true;
                WriteToLogFile("Strings.Left(response, 9) =" + Strings.Left(response, 9) + "\n baskId =" + baskId);

                if (Strings.Left(response, 9) == "Rmv" + new string(baskId) + "ERR")
                {
                    if (!string.IsNullOrEmpty(Variables.UDPonLock))
                    {
                        ReadUdp(Variables.UDPonLock); //force to refresh UDP due to Doevents problem
                    }
                    Variables.LockUdp = false;
                    Variables.IsWaiting = false;
                    return null;
                }

                if (!_policyManager.FUELONLY)
                {
                   // WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 2693");
                    if (Strings.Left(response, 8) != "Rmv" + new string(baskId) + "OK")
                    {
                        //WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 2696");
                        string tempCommandRenamed2 = "ENDPOS";
                        // WriteUDPData("Satrting "+DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 2701");
                        TCPAgent.Instance.Send_TCP(ref tempCommandRenamed2, true);
                        //WriteUDPData("Ending "+DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 2703");
                        if (!string.IsNullOrEmpty(Variables.UDPonLock))
                        {
                            ReadUdp(Variables.UDPonLock); //force to refresh UDP due to Doevents problem
                        }
                        //WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 2705");
                        Variables.LockUdp = false;
                        Variables.IsWaiting = false;
                        // WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 2713");
                        WriteToLogFile(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 2716");

                      // return sale;
                    }
                }

                if (Variables.Pump[index].Stock_Code == null)
                {
                    return sale;
                }

                WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 2722");


                pumps[index - 1].BasketButtonCaption = "";
                _saleLineManager.SetPluCode(ref sale, ref sl, Strings.Trim(Convert.ToString(Variables.Pump[index].Stock_Code[Variables.gBasket[index].PosIDCurr])), out error); //gPumps.grade(gBasket(Index).gradeIDCurr).Stock_Code);
                sl.BasketId = Variables.gBasket[index].currBaskID;
                Variables.gBasket[index].currBaskID = 0.ToString();
                dblPrice = double.Parse(Variables.gBasket[index].UPCurrent.ToString("#,##0.000"));
                sl.Regular_Price = dblPrice;
                sl.Quantity = Variables.gBasket[index].VolumeCurrent;
                sl.pumpID = (byte)index;
                sl.PositionID = (byte)Variables.gBasket[index].PosIDCurr;
                sl.GradeID = (byte)Variables.gBasket[index].gradeIDCurr;
                sl.MOP = (byte)Variables.gBasket[index].currMOP;

                sl.Amount = decimal.Parse(Variables.gBasket[index].AmountCurrent.ToString("#0.00"));

                sl.LoadFuelAmount = true;

                _saleManager.Add_a_Line(ref sale, sl, UserCode, sale.TillNumber, out error, true);

                _saleManager.Line_Price(ref sale, ref sl, dblPrice);

              //  WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 2743");

                if (_policyManager.FUELONLY)
                {
                    //Save_FuelOnly_Sale();
                }

                //convert stack sale to current if such exists
                if (Variables.gBasket[index].StackFilled)
                {
                    Variables.gBasket[index].currBaskID = Variables.gBasket[index].stackBaskID;
                    _curBaskSit[index] = _stackBaskSit[index];
                    Variables.gBasket[index].AmountCurrent = Variables.gBasket[index].AmountStack;
                    Variables.gBasket[index].VolumeCurrent = Variables.gBasket[index].VolumeStack;
                    Variables.gBasket[index].UPCurrent = Variables.gBasket[index].UPStack;
                    Variables.gBasket[index].DescCurrent = Variables.gBasket[index].DescStack;
                    pumps[index - 1].BasketButtonCaption = Variables.gBasket[index].AmountCurrent.ToString("##0.00"); //& vbCrLf & Format(gBasket(Index).VolumeCurrent, "#####.000")
                    Variables.gBasket[index].StackFilled = false;
                    MakeZero(ref pumps, false, index);
                    Variables.gBasket[index].stackBaskID = 0.ToString();
                    if (!string.IsNullOrEmpty(Variables.UDPonLock))
                    {
                        ReadUdp(Variables.UDPonLock); //force to refresh UDP due to Doevents problem
                    }
                    Variables.LockUdp = false;
                    Variables.IsWaiting = false;
                    return sale;
                }
                _curBaskSit[index] = 0;
                Variables.gBasket[index].CurrentFilled = false;
                if (!string.IsNullOrEmpty(Variables.UDPonLock))
                {
                    ReadUdp(Variables.UDPonLock); //force to refresh UDP due to Doevents problem
                }
                Variables.LockUdp = false;
                Variables.IsWaiting = false;
            }
            else
            {
                //don't allow user click this button before getting response
                pumps[index - 1].EnableBasketBotton = false;
                baskId = Variables.gBasket[index].stackBaskID.ToString().ToCharArray();
                if (_policyManager.FUELONLY)
                {
                    boolSent = Send_Sale(index, true, out error);
                    if (!boolSent)
                    {
                        if (!string.IsNullOrEmpty(Variables.UDPonLock))
                        {
                            ReadUdp(Variables.UDPonLock); //force to refresh UDP due to Doevents problem
                        }
                        Variables.LockUdp = false;
                        Variables.IsWaiting = false;
                        pumps[index - 1].EnableBasketBotton = true;
                        return null;
                    }
                }

                if (Variables.gBasket[index].stackBaskID == "0")
                {
                    return sale;
                }

                Variables.IsWaiting = true;
                string temp_Command_Renamed = "Rmv" + Convert.ToString(Variables.gBasket[index].stackBaskID);
                TCPAgent.Instance.Send_TCP(ref temp_Command_Renamed, true);


                string Response = "";
                strRemain = "";
                timeIn = (float)DateAndTime.Timer;
                while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
                {
                    strBuffer = System.Convert.ToString(TCPAgent.Instance.NewPortReading);
                    WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting Rmv" + new string(baskId));
                    if (!string.IsNullOrEmpty(strBuffer))
                    {
                        modStringPad.SplitResponse(strBuffer, "Rmv" + new string(baskId), ref Response, ref strRemain);
                        if (!string.IsNullOrEmpty(Response)) //got what we are waiting
                        {
                            TCPAgent.Instance.PortReading = strRemain;
                            WriteToLogFile("modify PortReading from remove Basket: " + strRemain);
                            break;
                        }
                    }
                    Variables.Sleep(100);
                    if (DateAndTime.Timer < timeIn)
                    {
                        timeIn = (float)DateAndTime.Timer;
                    }
                    break;
                }

                pumps[index - 1].EnableStackBasketBotton = true;

                if (Strings.Left(Response, 9) == "Rmv" + new string(baskId) + "ERR")
                {
                    if (!string.IsNullOrEmpty(Variables.UDPonLock))
                    {
                        ReadUdp(Variables.UDPonLock); //force to refresh UDP due to Doevents problem
                    }
                    Variables.LockUdp = false;
                    Variables.IsWaiting = false;
                    return null;
                }

                if (!_policyManager.FUELONLY)
                {
                    if (Strings.Left(Response, 8) != "Rmv" + new string(baskId) + "OK")
                    {
                        string temp_Command_Renamed2 = "ENDPOS";
                        WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 2844");

                        TCPAgent.Instance.Send_TCP(ref temp_Command_Renamed2, true);
                        WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 2847");

                        if (!string.IsNullOrEmpty(Variables.UDPonLock))
                        {
                            ReadUdp(Variables.UDPonLock); //force to refresh UDP due to Doevents problem
                        }
                        Variables.LockUdp = false;
                        Variables.IsWaiting = false;
                        
                      // return null;
                    }
                }

                var lastBasket = (Variables.gBasket[index].stackBaskID + "," + Strings.Right("00" + System.Convert.ToString(index), 2) + System.Convert.ToString(Variables.gBasket[index].stackMOP) + System.Convert.ToString(Variables.gBasket[index].posIDStack) + Strings.Right("0000000" + System.Convert.ToString(Variables.gBasket[index].AmountStack * 1000), 8) + Strings.Right("00000000" + System.Convert.ToString(Variables.gBasket[index].VolumeStack * 1000), 8)).ToCharArray();

                if (Variables.Pump[index].Stock_Code == null)
                {
                    return sale;
                }

                _stackBaskSit[index] = 0;
                _saleLineManager.SetPluCode(ref sale, ref sl, Convert.ToString(Variables.Pump[index].Stock_Code[Variables.gBasket[index].posIDStack]), out error);
                sl.BasketId = Variables.gBasket[index].stackBaskID;
                Variables.gBasket[index].stackBaskID = (0).ToString();
                dblPrice = double.Parse(Variables.gBasket[index].UPStack.ToString("#,##0.000"));
                sl.Regular_Price = dblPrice;
                sl.Quantity = Variables.gBasket[index].VolumeStack;
                sl.pumpID = (byte)index;
                sl.PositionID = (byte)(Variables.gBasket[index].posIDStack);
                sl.GradeID = (byte)(Variables.gBasket[index].gradeIDStack);
                sl.MOP = (byte)(Variables.gBasket[index].stackMOP);
                sl.Amount = decimal.Parse(Variables.gBasket[index].AmountStack.ToString("#0.00"));
                sl.LoadFuelAmount = true;
                _saleManager.Add_a_Line(ref sale, sl, UserCode, sale.TillNumber, out error, true);
                _saleManager.Line_Price(ref sale, ref sl, dblPrice);

                Chaps_Main.SC = sl.Stock_Code;

                // There is no SaleMain screen, so don't send it to the screen at all
                if (_policyManager.FUELONLY)
                {
                    //Save_FuelOnly_Sale();
                }
                else
                {

                }


                Variables.gBasket[index].StackFilled = false;
                //makeZero(false, Index);

                if (!string.IsNullOrEmpty(Variables.UDPonLock))
                {
                    ReadUdp(Variables.UDPonLock); //force to refresh UDP due to Doevents problem
                }
                Variables.LockUdp = false;
                Variables.IsWaiting = false;
            }
            // WriteUDPData("Inside Fuel PumpManager "+ pumps[2].BasketButtonCaption);
            return sale;
        }



        /// <summary>
        /// Method to set all pump amount to 0
        /// </summary>
        /// <param name="pumps">List of pumps</param>
        /// <param name="current">Current or not</param>
        /// <param name="pumpId">Pump id</param>
        public void MakeZero(ref List<PumpControl> pumps, bool current, short pumpId)
        {
            pumps[pumpId - 1].BasketButtonCaption = "";
            Variables.gBasket[pumpId].AmountCurrent = 0;
            Variables.gBasket[pumpId].VolumeCurrent = 0;
            Variables.gBasket[pumpId].UPCurrent = 0;
            Variables.gBasket[pumpId].DescCurrent = new char[100].ToString();
        }

        /// <summary>
        /// Method to load base prices
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public BasePrices LoadBasePrices(ref string report, out ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();
            return frmPumpPrice_Load(ref report, ref errorMessage);
        }

        /// <summary>
        /// Method to initialise pump manager
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public BasePrices frmPumpPrice_Load(ref string report, ref ErrorMessage errorMessage)
        {
            var fuelPrices = new BasePrices();

            Store storeRenamed = _policyManager.LoadStoreInfo();
            GradeType[] myPrice = default(GradeType[]);
            short[] iGradeLt = default(short[]);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!_policyManager.U_CHGFPRICE && !_policyManager.FUELPR_HO) //   added And Not Policy.FUELPR_HO, if fuel prices are controlled from HeadOffice the policy U_CHGFPRICE should not matter, as the user cannot edit the prices in the fuel price form. This is by design.
            {
                //MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                //Chaps_Main.DisplayMessage(this, (short)75, temp_VbStyle, null, (byte)0);
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 38, 75, null, MessageType.OkOnly);
                errorMessage.StatusCode = HttpStatusCode.Forbidden;
                return null;
            }

            if (_policyManager.U_CHGFPRICE || (_policyManager.FUELPR_HO && Chaps_Main.boolFP_HO)) //   added Or (Policy.FUELPR_HO And boolFP_HO), if fuel prices are controlled from HeadOffice the policy U_CHGFPRICE should not matter, as the user cannot edit the prices in the fuel price form. This is by design.
            {

            }
            else
            {
                //MsgBoxStyle temp_VbStyle2 = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                //Chaps_Main.DisplayMessage(this, (short)75, temp_VbStyle2, null, (byte)0);
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 38, 75, null, MessageType.OkOnly);
                errorMessage.StatusCode = HttpStatusCode.Forbidden;
                return null;
            }

            LoadMyPrices(ref myPrice, ref iGradeLt, ref fuelPrices);

            fuelPrices.IsCreditPriceEnabled = Convert.ToBoolean(_policyManager.FUEL_CP);


            //lblinformation.Visible = true;
            fuelPrices.Caption = _resourceManager.GetResString(offSet, (short)287);

            // Will be calculated once user selects a grade
            //old_reportID = (short)(Variables.gPumps.get_FuelPrice(byte.Parse(txtGrade.Text), byte.Parse(txtTier.Text), byte.Parse(txtLevel.Text)).ReportID);

            //ckTot.CaptionBold = true;


            //if (VB.Strings.Left(System.Convert.ToString(Store_Renamed.Language), 1).ToUpper() == "F")
            //{
            //    ckTot.CaptionSize = 10;
            //    ckTot.CaptionTop = 180;
            //}
            //else
            //{
            //    
            //    ckTot.CaptionSize = 12;
            //    ckTot.CaptionTop = 120;
            //}
            //ckTot.CheckBoxTop = 130;
            //ckTot.CheckSize = 10;
            //ckPriceDisplay.CaptionBold = true;


            //if (VB.Strings.Left(System.Convert.ToString(Store_Renamed.Language), 1).ToUpper() == "F")
            //{
            //    ckPriceDisplay.CaptionSize = 10;
            //    ckPriceDisplay.CaptionTop = 180;
            //}
            //else
            //{
            //    
            //    ckPriceDisplay.CaptionSize = 12;
            //    ckPriceDisplay.CaptionTop = 120;
            //}
            //ckPriceDisplay.CheckBoxTop = 130;
            //ckPriceDisplay.CheckSize = 10;
            //    ckPriceDisplay.Caption = Resource.DisplayCaption(ckPriceDisplay.Tag, Me.Tag)

            //ckPriceDisplay.Enabled = _policyManager.PRICEDISPLAY;
            fuelPrices.IsPricesToDisplayEnabled = _policyManager.PRICEDISPLAY;



            if (_policyManager.PRICEDISPLAY)
            {
                //ckPriceDisplay.Value = true;
                fuelPrices.IsPricesToDisplayChecked = true;
            }


            //cmdSelectPrice.Enabled = System.Convert.ToBoolean(ckPriceDisplay.Enabled);
            fuelPrices.IsPricesToDisplayEnabled = Convert.ToBoolean(fuelPrices.IsPricesToDisplayEnabled);


            //ckTankDip.CaptionBold = true;

            //if (VB.Strings.Left(System.Convert.ToString(Store_Renamed.Language), 1).ToUpper() == "F")
            //{
            //    ckTankDip.CaptionSize = 10;
            //    ckTankDip.CaptionTop = 180;
            //}
            //else
            //{
            //    ckTankDip.CaptionSize = 12;
            //    ckTankDip.CaptionTop = 100;
            //}

            //ckTankDip.Enabled = _policyManager.TankGauge && _policyManager.U_DipRead;
            fuelPrices.IsReadTankDipEnabled = _policyManager.TankGauge && _policyManager.U_DipRead;

            //ckTankDip.Value = _policyManager.TankGauge && _policyManager.DftRdTankDip && _policyManager.U_DipRead;
            fuelPrices.IsReadTankDipChecked = _policyManager.TankGauge && _policyManager.DftRdTankDip && _policyManager.U_DipRead;
            //ckTankDip.CheckBoxTop = 130;
            //ckTankDip.CheckSize = 10;




            if (Strings.Left(Convert.ToString(storeRenamed.Language), 1).ToUpper() == "F")
            {
                //cmdTot.TopPosition = (short)2;
            }




            if (_policyManager.TAX_EXEMPT)
            {
                //txtTECashPrice.Visible = true;



                fuelPrices.IsTaxExemptedCreditPriceEnabled = _policyManager.FUEL_CP && (_policyManager.TE_ByRate == false || (_policyManager.TE_ByRate == true && _policyManager.TE_Type == "SITE"));

                fuelPrices.IsTaxExemptedCashPriceEnabled = _policyManager.TE_ByRate == false || (_policyManager.TE_ByRate == true && _policyManager.TE_Type == "SITE");

                //txtTECreditPrice.Visible = true;

                fuelPrices.IsTaxExemptionVisible = true;
                //Label2.Visible = true;
            }
            else
            {
                //txtTECashPrice.Visible = false;
                //txtTECreditPrice.Visible = false;
                fuelPrices.IsTaxExemptionVisible = false;
                //Label2.Visible = false;
            }


            if (_policyManager.FUELPR_HO && Chaps_Main.boolFP_HO)
            {
                fuelPrices.IsCashPriceEnabled = false;
                fuelPrices.IsCreditPriceEnabled = false;
                fuelPrices.IsTaxExemptedCashPriceEnabled = false;
                fuelPrices.IsTaxExemptedCreditPriceEnabled = false;
            }




            //ckTot.Value = _policyManager.DftRdTotal;
            fuelPrices.IsReadTankDipChecked = _policyManager.DftRdTotal;



            if (!TCPAgent.Instance.PortOpened)
            {
                fuelPrices.CanReadTotalizer = false;
                fuelPrices.IsReadTotalizerEnabled = false;
                fuelPrices.IsReadTotalizerChecked = false;
                fuelPrices.IsReadTankDipEnabled = false;
                fuelPrices.IsReadTankDipChecked = false;
            }
            fuelPrices.FuelPrices = fuelPrices.FuelPrices.Where(x => x.Row != 0).ToList();
            var oldReportId = (short)Variables.gPumps.get_FuelPrice((byte)fuelPrices.FuelPrices.FirstOrDefault().GradeId, (byte)fuelPrices.FuelPrices.FirstOrDefault()?.TierId, (byte)fuelPrices.FuelPrices.FirstOrDefault()?.LevelId).ReportID;
            report = cmdPrint_Click(ref myPrice, fuelPrices.IsReadTotalizerChecked, fuelPrices.IsReadTankDipChecked, oldReportId);
            return fuelPrices;

            error_end:
            1.GetHashCode(); //VBConversions note: C# requires an executable line here, so a dummy line was added.
        }

        /// <summary>
        /// Method to set base prices
        /// </summary>
        /// <param name="updatedPrice"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public FuelPrice SetBasePrice(ref FuelPrice updatedPrice, out ErrorMessage message)
        {
            message = new ErrorMessage();

            var fuelPrices = new BasePrices();

            GradeType[] myPrice = default(GradeType[]);
            short[] iGradeLt = default(short[]);

            LoadMyPrices(ref myPrice, ref iGradeLt, ref fuelPrices);

            GetTaxExemption(ref fuelPrices);

            ctlKeyPadV1_EnterPressed(ref myPrice, ref fuelPrices, ref updatedPrice, ref message);

            return updatedPrice;
        }

        /// <summary>
        /// Method to update on key pad enter event
        /// </summary>
        /// <param name="myPrice"></param>
        /// <param name="prices"></param>
        /// <param name="updatedPrice"></param>
        /// <param name="message"></param>
        public void ctlKeyPadV1_EnterPressed(ref GradeType[] myPrice, ref BasePrices prices,
            ref FuelPrice updatedPrice, ref ErrorMessage message)
        {
            short mGrade = updatedPrice.GradeId;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            try
            {

                Convert.ToDecimal(updatedPrice.CashPrice);
                Convert.ToDecimal(updatedPrice.CreditPrice);
                if (prices.IsTaxExemptionVisible)
                {
                    if (updatedPrice.TaxExemptedCashPrice != _resourceManager.GetResString(offSet, 288))
                    {
                        Convert.ToDecimal(updatedPrice.TaxExemptedCashPrice);
                    }
                    if (updatedPrice.TaxExemptedCreditPrice != _resourceManager.GetResString(offSet, 288))
                    {
                        Convert.ToDecimal(updatedPrice.TaxExemptedCreditPrice);
                    }
                }
            }
            catch (Exception)
            {
                message.MessageStyle = new MessageStyle { Message = Utilities.Constants.InvalidRequest, MessageType = 0 };
                message.StatusCode = HttpStatusCode.BadRequest;
                return;
            }

            updatedPrice.CashPrice = CommonUtility.GetDoubleValue(updatedPrice.CashPrice).ToString("0.000");
            updatedPrice.CreditPrice = CommonUtility.GetDoubleValue(updatedPrice.CreditPrice).ToString("0.000");
            if (prices.IsTaxExemptionVisible)
            {
                if (updatedPrice.TaxExemptedCashPrice != _resourceManager.GetResString(offSet, 288))
                {
                    updatedPrice.TaxExemptedCashPrice = CommonUtility.GetDoubleValue(updatedPrice.TaxExemptedCashPrice).ToString("0.000");
                }
                if (updatedPrice.TaxExemptedCreditPrice != _resourceManager.GetResString(offSet, 288))
                {
                    updatedPrice.TaxExemptedCreditPrice = CommonUtility.GetDoubleValue(updatedPrice.TaxExemptedCreditPrice).ToString("0.000");
                }
            }

            if (updatedPrice.CashPrice != myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].CashP.ToString())
            {
                if (!ValidatePrice(updatedPrice.CashPrice, ref message))
                {
                    return;
                }

                mGrade = updatedPrice.GradeId;
                // change


                if (!(CommonUtility.GetDoubleValue(myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].CashP) - _policyManager.FUEL_MAXTH <= CommonUtility.GetDoubleValue(updatedPrice.CashPrice)
                    && CommonUtility.GetDoubleValue(myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].CashP) + _policyManager.FUEL_MAXTH >= CommonUtility.GetDoubleValue(updatedPrice.CashPrice)))
                {

                    //MsgBoxStyle temp_VbStyle3 = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                    //Chaps_Main.DisplayMessage(this, (short)93, temp_VbStyle3, _policyManager.FUEL_MAXTH.ToString("$#0.000"), (byte)0);
                    message.MessageStyle = _resourceManager.CreateMessage(offSet, 21, 93, _policyManager.FUEL_MAXTH.ToString("$#0.000"), MessageType.OkOnly);
                    message.StatusCode = HttpStatusCode.BadRequest;
                    return;
                }
                //   end
                updatedPrice.CashPrice = CommonUtility.GetDoubleValue(updatedPrice.CashPrice).ToString("0.000");
                //            mGrade = GetGradeID

                myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].CashP = CommonUtility.GetDoubleValue(updatedPrice.CashPrice).ToString("0.000");

                if (!_policyManager.FUEL_CP)
                {
                    updatedPrice.CreditPrice = CommonUtility.GetDoubleValue(updatedPrice.CashPrice).ToString("0.000");

                    myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].CreditP = CommonUtility.GetDoubleValue(updatedPrice.CashPrice).ToString("0.000");
                }





                if (_policyManager.TAX_EXEMPT && _policyManager.TE_ByRate && _policyManager.TE_Type != "SITE") //  
                {

                    myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].TECashP = Convert.ToDouble(_fuelPumpService.GetTePriceByRate(mGrade, float.Parse(CommonUtility.GetDoubleValue(updatedPrice.CashPrice).ToString("0.000")), Variables.gPumps.get_Grade((byte)Convert.ToInt16(mGrade)).Stock_Code));
                    if (updatedPrice.TaxExemptedCashPrice != _resourceManager.GetResString(offSet, 288))
                    {
                        updatedPrice.TaxExemptedCashPrice = myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].TECashP.ToString("0.000");
                    }
                    if (!_policyManager.FUEL_CP)
                    {
                        myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].TECreditP = myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].TECashP;
                        if (updatedPrice.TaxExemptedCreditPrice != _resourceManager.GetResString(offSet, 288))
                        {
                            updatedPrice.TaxExemptedCreditPrice = updatedPrice.TaxExemptedCashPrice;
                        }
                    }
                }
            }
            if (updatedPrice.CreditPrice != myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].CreditP.ToString())
            {
                if (!ValidatePrice(updatedPrice.CreditPrice, ref message))
                {
                    return;
                }
                mGrade = updatedPrice.GradeId;
                // change


                if (!(CommonUtility.GetDoubleValue(myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].CreditP) - _policyManager.FUEL_MAXTH <= CommonUtility.GetDoubleValue(updatedPrice.CreditPrice)
                    && CommonUtility.GetDoubleValue(myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].CreditP) + _policyManager.FUEL_MAXTH >= CommonUtility.GetDoubleValue(updatedPrice.CreditPrice)))
                {

                    //MsgBoxStyle temp_VbStyle4 = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                    //Chaps_Main.DisplayMessage(this, (short)93, temp_VbStyle4, _policyManager.FUEL_MAXTH.ToString("$#0.000"), (byte)0);
                    message.MessageStyle = _resourceManager.CreateMessage(offSet, 21, 93, _policyManager.FUEL_MAXTH.ToString("$#0.000"), MessageType.OkOnly);
                    message.StatusCode = HttpStatusCode.BadRequest;
                    return;
                }
                //   end
                updatedPrice.CreditPrice = CommonUtility.GetDoubleValue(updatedPrice.CreditPrice).ToString("0.000");
                //            mGrade = GetGradeID

                myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].CreditP = CommonUtility.GetDoubleValue(updatedPrice.CreditPrice).ToString("0.000");





                if (_policyManager.TAX_EXEMPT && _policyManager.TE_ByRate && _policyManager.TE_Type != "SITE") //  
                {

                    myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].TECreditP = Convert.ToDouble(_fuelPumpService.GetTePriceByRate(mGrade, float.Parse(CommonUtility.GetDoubleValue(updatedPrice.CreditPrice).ToString("0.000")), Variables.gPumps.get_Grade((byte)Convert.ToInt16(mGrade)).Stock_Code));
                    updatedPrice.TaxExemptedCreditPrice = myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].TECreditP.ToString("0.000");
                }


            }
            if (updatedPrice.TaxExemptedCashPrice != _resourceManager.GetResString(offSet, 288) &&
                updatedPrice.TaxExemptedCashPrice != myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].TECashP.ToString())
            {
                if (!ValidatePrice(updatedPrice.TaxExemptedCashPrice, ref message))
                {
                    return;
                }
                mGrade = updatedPrice.GradeId;
                // change

                if (!(myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].TECashP - _policyManager.FUEL_MAXTH <= double.Parse(updatedPrice.TaxExemptedCashPrice) && myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].TECashP + _policyManager.FUEL_MAXTH >= double.Parse(updatedPrice.TaxExemptedCashPrice)))
                {

                    //MsgBoxStyle temp_VbStyle5 = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                    //Chaps_Main.DisplayMessage(this, (short)93, temp_VbStyle5, _policyManager.FUEL_MAXTH.ToString("$#0.000"), (byte)0);
                    message.MessageStyle = _resourceManager.CreateMessage(offSet, 21, 93, _policyManager.FUEL_MAXTH.ToString("$#0.000"), MessageType.OkOnly);
                    message.StatusCode = HttpStatusCode.BadRequest;
                    return;
                }
                //   end
                updatedPrice.TaxExemptedCashPrice = CommonUtility.GetDoubleValue(updatedPrice.TaxExemptedCashPrice).ToString("0.000");
                myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].TECashP = double.Parse(CommonUtility.GetDoubleValue(updatedPrice.TaxExemptedCashPrice).ToString("0.000"));

                if (!_policyManager.FUEL_CP)
                {
                    updatedPrice.TaxExemptedCreditPrice = CommonUtility.GetDoubleValue(updatedPrice.TaxExemptedCashPrice).ToString("0.000");
                    myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].TECreditP = double.Parse(CommonUtility.GetDoubleValue(updatedPrice.TaxExemptedCashPrice).ToString("0.000"));
                }
            }
            if (updatedPrice.TaxExemptedCreditPrice != _resourceManager.GetResString(offSet, 288) &&
                updatedPrice.TaxExemptedCreditPrice != myPrice[updatedPrice.GradeId].Price[updatedPrice.TierId, updatedPrice.LevelId].TECreditP.ToString())
            {
                if (!ValidatePrice(updatedPrice.TaxExemptedCreditPrice, ref message))
                {
                    return;
                }
                mGrade = updatedPrice.GradeId;
                // change

                if (!(myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].TECreditP - _policyManager.FUEL_MAXTH <= double.Parse(updatedPrice.TaxExemptedCreditPrice) && myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].TECreditP + _policyManager.FUEL_MAXTH >= double.Parse(updatedPrice.TaxExemptedCreditPrice)))
                {

                    //MsgBoxStyle temp_VbStyle6 = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                    //Chaps_Main.DisplayMessage(this, (short)93, temp_VbStyle6, _policyManager.FUEL_MAXTH.ToString("$#0.000"), (byte)0);
                    message.MessageStyle = _resourceManager.CreateMessage(offSet, 21, 93, _policyManager.FUEL_MAXTH.ToString("$#0.000"), MessageType.OkOnly);
                    message.StatusCode = HttpStatusCode.BadRequest;
                    return;
                }
                //   end
                updatedPrice.TaxExemptedCreditPrice = CommonUtility.GetDoubleValue(updatedPrice.TaxExemptedCreditPrice).ToString("0.000");
                myPrice[mGrade].Price[updatedPrice.TierId, updatedPrice.LevelId].TECreditP = double.Parse(CommonUtility.GetDoubleValue(updatedPrice.TaxExemptedCreditPrice).ToString("0.000"));

            }


            //txtTECashPrice.Visible = System.Convert.ToBoolean(_policyManager.TAX_EXEMPT);

            //txtTECreditPrice.Visible = System.Convert.ToBoolean(_policyManager.TAX_EXEMPT);
            prices.IsTaxExemptionVisible = Convert.ToBoolean(_policyManager.TAX_EXEMPT);

            //Label2.Visible = System.Convert.ToBoolean(_policyManager.TAX_EXEMPT);


        }

        /// <summary>
        /// Method to verify base price
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="updatedPrices"></param>
        /// <param name="isPricesToDisplayChecked"></param>
        /// <param name="isTankDipChecked"></param>
        /// <param name="isTotalizerChecked"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string VerifyBasePrices(int tillNumber, List<FuelPrice> updatedPrices, bool isPricesToDisplayChecked,
            bool isTankDipChecked, bool isTotalizerChecked, out ErrorMessage error, ref string caption2)
        {
            error = new ErrorMessage();
            return cmdFinish_Click(tillNumber, updatedPrices, isPricesToDisplayChecked, isTankDipChecked, isTotalizerChecked,
                true,
                ref error, ref caption2);
        }

        /// <summary>
        /// Method to save base price
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="updatedPrices"></param>
        /// <param name="isPricesToDisplayChecked"></param>
        /// <param name="isTankDipChecked"></param>
        /// <param name="isTotalizerChecked"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string SaveBasePrices(int tillNumber, List<FuelPrice> updatedPrices, bool isPricesToDisplayChecked,
            bool isTankDipChecked, bool isTotalizerChecked, out ErrorMessage error, ref string caption2)
        {
            error = new ErrorMessage();
            return cmdFinish_Click(tillNumber, updatedPrices, isPricesToDisplayChecked, isTankDipChecked, isTotalizerChecked,
                false,
                ref error, ref caption2);
        }

        /// <summary>
        /// Method to finish
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="updatedPrices"></param>
        /// <param name="isPricesToDisplayChecked"></param>
        /// <param name="isTankDipChecked"></param>
        /// <param name="isTotalizerChecked"></param>
        /// <param name="isCallForVerify"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string cmdFinish_Click(int tillNumber, List<FuelPrice> updatedPrices, bool isPricesToDisplayChecked,
            bool isTankDipChecked, bool isTotalizerChecked, bool isCallForVerify, ref ErrorMessage error, ref string caption2)
        {
            var caption = string.Empty;

            string displayInfo = string.Empty;
            MessageStyle success = new MessageStyle();

            var fuelPrices = new BasePrices();
            GradeType[] myPrice = default(GradeType[]);
            short iPointer;
            short[] iGradeLt = default(short[]);
            short iPointerMax;
            byte dummyGradeQty = default(byte);
            var tothlMg = default(short);
            var totMg = default(short);

            short oldReportId;
            double teCashPrice = 0;
            double teCreditPrice = 0;
            double teCashPriceHo = 0;
            double teCreditPriceHo = 0;
            bool teFound = false;
            short gradeId;
            short j = 0;
            short i = 0;
            short ii = 0;
            short T = 0;
            short l = 0;
            bool firsttime = false;
            List<FuelPrice> rsFuelPriceHo = null; //  

            LoadMyPrices(ref myPrice, ref iGradeLt, ref fuelPrices);

            GetTaxExemption(ref fuelPrices);

            for (var index = 0; index < updatedPrices.Count; index++)
            {
                var updatedPrice = updatedPrices[index];
                ctlKeyPadV1_EnterPressed(ref myPrice, ref fuelPrices, ref updatedPrice, ref error);

                if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
                {
                    return null;
                }
            }

            short yesNo;
            string cashPrice = "";
            string creditPrice = "";
            short posId = 0;
            byte tierLevel = 0;
            string amount = "";
            string volume = "";
            string highVolume = "";
            string lowVolume = "";
            const short iMaxPos = 9;
            string response = "";
            string strBuffer = "";
            string strRemain = "";
            User currentUser = default(User);
            bool changeFuelPriceManually = false;
            short ans = 0;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            int handleRenamed = 0;
            string strDipMessage = "";
            // Oct 27, 2009 Nicolette added to change the way system manages ReportID and DateTime stamp in FuelPrice change
            // these two values should be unique by batch and not by grade, tier, level
            int reportId = 0;
            DateTime dateTime = default(DateTime);
            // Oct 27, 2009 Nicolette end

            strDipMessage = "";
            //lblDisplaynfo.Visible = false; 
            displayInfo = string.Empty;

            float timeIn = 0;

            string[] strPrice = new string[5];
            //if (txtCashPrice.Text != _resourceManager.GetResString(offSet,(short)288) && txtCreditPrice.Text != _resourceManager.GetResString(offSet,(short)288))
            if (true)
            {



                ValidateBasePrices(myPrice, ref fuelPrices, ref error, ref changeFuelPriceManually, ref currentUser, isCallForVerify);

                if (isCallForVerify)
                {
                    return null;
                }

                if (error != null && error.MessageStyle != null && !string.IsNullOrEmpty(error.MessageStyle.Message))
                {
                    return null;
                }



                //EnableButtons(false);
                //this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                //lblinformation.Visible = true;
                caption = _resourceManager.CreateCaption(offSet, Convert.ToInt16(14), Convert.ToInt16(21), null, (short)1); //   "Setting in Process... Please Wait..."
                //this.Refresh();
                // Oct 27, 2009 Nicolette, to have unique ReportID and DateTime by batch, see comment in GetProperty object
                dateTime = DateTime.Now;
                reportId = _getPropertyManager.Get_ReportID();
                // Oct 27, 2009 Nicolette end

                for (i = 1; i <= myPrice.Length - 1; i++)
                {


                    if ((int)myPrice[i].Grade != 0 && Variables.gPumps.get_Grade(Convert.ToByte(myPrice[i].Grade)).FuelType != "O")
                    {
                        //06/17/03,if we don't have this grade in the Grade table, don't do anything
                        //to fix the problem when we skip some grade in the Grade table


                        //rsGrd = Chaps_Main.Get_Records("select * from Grade where ID=" + System.Convert.ToString(MyPrice[i].Grade), Chaps_Main.dbPump, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
                        var rsGrd = Variables.gPumps.get_Grade((byte)myPrice[i].Grade);
                        if (rsGrd != null)
                        {
                            //06/17/03 end
                            for (T = 1; T <= 2; T++)
                            {
                                for (l = 1; l <= 2; l++)
                                {
                                    switch (T + Convert.ToString(l))
                                    {
                                        case "11":
                                            tierLevel = (byte)0;
                                            break;
                                        case "12":
                                            tierLevel = (byte)1;
                                            break;
                                        case "21":
                                            tierLevel = (byte)2;
                                            break;
                                        case "22":
                                            tierLevel = (byte)3;
                                            break;
                                    }

                                    gradeId = Convert.ToInt16(myPrice[i].Grade);

                                    cashPrice = Convert.ToString(Information.IsDBNull(myPrice[i].Price[T, l].CashP) ? 0 : myPrice[i].Price[T, l].CashP);

                                    creditPrice = Convert.ToString(Information.IsDBNull(myPrice[i].Price[T, l].CreditP) ? 0 : myPrice[i].Price[T, l].CreditP);
                                    if (!string.IsNullOrEmpty(cashPrice) && !string.IsNullOrEmpty(creditPrice))
                                    {
                                        cashPrice = Strings.Right("00000" + Convert.ToString(CommonUtility.GetDoubleValue(cashPrice) * 1000), 5);
                                        creditPrice = Strings.Right("00000" + Convert.ToString(CommonUtility.GetDoubleValue(creditPrice) * 1000), 5);

                                        if (changeFuelPriceManually)
                                        {
                                            Save_Price(myPrice, gradeId, T, l, reportId, dateTime);
                                        }
                                        else
                                        {


                                            if (TCPAgent.Instance.PortOpened)
                                            {
                                                response = "";
                                                strRemain = "";
                                                var command = "Set" + Convert.ToString(gradeId) + Convert.ToString(tierLevel) + cashPrice + creditPrice;
                                                TCPAgent.Instance.Send_TCP(ref command, true);

                                                if (timeIn > DateAndTime.Timer)
                                                {
                                                    timeIn = 0; //reset on midnight
                                                }
                                                else
                                                {
                                                    timeIn = (float)DateAndTime.Timer;
                                                }

                                                while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
                                                {

                                                    strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                                                    WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting Set" + Convert.ToString(gradeId) + Convert.ToString(tierLevel));
                                                    if (!string.IsNullOrEmpty(strBuffer))
                                                    {
                                                        modStringPad.SplitResponse(strBuffer, "Set" + Convert.ToString(gradeId) + Convert.ToString(tierLevel), ref response, ref strRemain); //strBuffer<>""
                                                        if (!string.IsNullOrEmpty(response)) //got what we are waiting
                                                        {

                                                            TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                                                            WriteToLogFile("modify TCPAgent.PortReading from Set Price: " + strRemain);
                                                        }
                                                    }
                                                    Variables.Sleep(100);
                                                    if (DateAndTime.Timer < timeIn)
                                                    {
                                                        timeIn = (float)DateAndTime.Timer;
                                                    }
                                                }

                                                if (!(Strings.Left(response, 7) == "Set" + Convert.ToString(gradeId) + Convert.ToString(tierLevel) + "OK"))
                                                {
                                                    //command = "ENDPOS";
                                                    //TCPAgent.Instance.Send_TCP(ref command, true);
                                                    goto Error1;
                                                }
                                                Save_Price(myPrice, gradeId, T, l, reportId, dateTime);
                                                ///                            'Nancy,11/03/03,logic changed to if fail for one, continue the rest
                                                ///                            If Left(TCPAgent.PortReading, 7) = "Set" & GradeID & TierLevel & "OK" Then
                                                ///                                Save_Price GradeID, T, L
                                                ///                            End If
                                            }
                                            else
                                            {
                                                //                     MsgBox "No TCP connection to Host !!!", vbCritical
                                                //EnableButtons(true);
                                                //Chaps_Main.DisplayMsgForm(3892, (short)99, null, (byte)0, (byte)0, "", "", "", "");
                                                error.MessageStyle = _resourceManager.CreateMessage(offSet, 3892, 99, null, MessageType.OkOnly);
                                                error.StatusCode = HttpStatusCode.BadRequest;
                                                //this.Cursor = System.Windows.Forms.Cursors.Arrow;

                                                rsGrd = null;
                                                //if (!cmdExit_.Enabled)
                                                //{
                                                //    cmdExit_.Enabled = true;
                                                //}
                                                fuelPrices.IsExitEnabled = true;
                                                return null;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (_policyManager.PRICEDISPLAY)
                {

                    if (isPricesToDisplayChecked)
                    {
                        for (i = 1; i <= Variables.gPumps.PriceDisplayRows; i++)
                        {
                            strPrice[i] = Strings.Right("0000" + Variables.gPumps.get_FuelPrice(_getPropertyManager.get_PricesToDisplay((byte)i).GradeID, _getPropertyManager.get_PricesToDisplay((byte)i).TierID, _getPropertyManager.get_PricesToDisplay((byte)i).LevelID).CashPrice * 1000, 4);
                        }

                        caption2 = DisplayPriceViaFc(strPrice[1], strPrice[2], strPrice[3], strPrice[4]);

                    }
                }


                if (!changeFuelPriceManually)
                {

                    if (isTotalizerChecked)
                    {

                        timeIn = (float)DateAndTime.Timer;
                        while (DateAndTime.Timer - timeIn < Variables.gPumps.ReadTotDelay)
                        {
                            //Variables.Sleep(100);
                            Application.DoEvents();
                        }


                        totMg = (short)(_fuelPumpService.GetMaxGroupNumberofTotalizerHistory() + 1);

                        tothlMg = _fuelPumpService.GetMaxGroupNumberofTotalHighLow();
                        //02/17/03 end

                        for (j = 1; j <= Variables.gPumps.PumpsCount; j++) //loop through all pumps that have such grade/position
                        {
                            if (Variables.PumpStatus[j] != 7) //if pump is "Inactive",can't get Totalizer successful
                            {
                                for (posId = 1; posId <= iMaxPos; posId++)
                                {
                                    if (Variables.gPumps.get_Assignment((byte)j, (byte)posId).GradeID != 0 && Variables.gPumps.get_Assignment((byte)i, (byte)j).GradeID != null)
                                    {
                                        ///                        TCPAgent.PortReading = ""
                                        response = "";
                                        strRemain = "";

                                        var data = "Tot" + Strings.Right("0" + Convert.ToString(j), 2) + Convert.ToString(posId);
                                        TCPAgent.Instance.Send_TCP(ref data, true);

                                        //                        Dim timeIN           As Single
                                        if (timeIn > DateAndTime.Timer)
                                        {
                                            timeIn = 0; //reset on midnight
                                        }
                                        else
                                        {
                                            timeIn = (float)DateAndTime.Timer;
                                        }
                                        ///                        Do Until Left(TCPAgent.PortReading, 3) = "Tot" Or Timer - timeIN > gPumps.CommunicationTimeOut
                                        ///                            DoEvents
                                        ///                        Loop
                                        while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
                                        {

                                            strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                                            WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting Tot" + Strings.Right("0" + Convert.ToString(j), 2) + Convert.ToString(posId));
                                            if (!string.IsNullOrEmpty(strBuffer))
                                            {
                                                modStringPad.SplitResponse(strBuffer, "Tot" + Strings.Right("0" + Convert.ToString(j), 2) + Convert.ToString(posId), ref response, ref strRemain); //strBuffer<>""
                                                if (!string.IsNullOrEmpty(response)) //got what we are waiting
                                                {

                                                    TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                                                    WriteToLogFile("modify TCPAgent.PortReading from Get Totalizer: " + strRemain);
                                                    break;
                                                }
                                            }
                                            //Variables.Sleep(100);
                                            Application.DoEvents();
                                            if (DateAndTime.Timer < timeIn)
                                            {
                                                timeIn = (float)DateAndTime.Timer;
                                            }
                                        }

                                        if (!(response.Length == 33) || !(Strings.Left(response, 3) == "Tot"))
                                        {
                                            //if got Totalizer reading Error, save 0 to database
                                            //Nancy,11/03/03,logic changed to if fail for one, continue the rest
                                            GetFromLastReading(j, Convert.ToInt16(Variables.gPumps.get_Assignment((byte)j, (byte)posId).GradeID), volume, ref amount);
                                        }
                                        else
                                        {
                                            //write to database
                                            volume = response.Substring(6, 10);
                                            amount = response.Substring(16, 10);
                                        }
                                        _getPropertyManager.WriteTotalizer(_tillService.GetTill(tillNumber), j, posId, Convert.ToByte(Variables.gPumps.get_Assignment((byte)j, (byte)posId).GradeID), volume, amount, totMg);
                                    }
                                }


                                response = "";
                                strRemain = "";

                                var command = "THL" + Strings.Right("0" + Convert.ToString(j), 2);
                                TCPAgent.Instance.Send_TCP(ref command, true);

                                if (timeIn > DateAndTime.Timer)
                                {
                                    timeIn = 0; //reset on midnight
                                }
                                else
                                {
                                    timeIn = (float)DateAndTime.Timer;
                                }

                                while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
                                {

                                    strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                                    WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting THL" + Strings.Right("0" + Convert.ToString(j), 2));
                                    if (!string.IsNullOrEmpty(strBuffer))
                                    {
                                        modStringPad.SplitResponse(strBuffer, "THL" + Strings.Right("0" + Convert.ToString(j), 2), ref response, ref strRemain); //strBuffer<>""
                                        if (!string.IsNullOrEmpty(response)) //got what we are waiting
                                        {

                                            TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                                            WriteToLogFile("modify TCPAgent.PortReading from Get Totalizer HighLow: " + strRemain);
                                            break;
                                        }
                                    }
                                    Variables.Sleep(100);
                                    if (DateAndTime.Timer < timeIn)
                                    {
                                        timeIn = (float)DateAndTime.Timer;
                                    }
                                }

                                if (!(response.Length == 27) || !(Strings.Left(response, 3) == "THL") || !(Strings.Right(response, 2) == "OK") || !(response.Substring(3, 2) == Strings.Right("0" + Convert.ToString(j), 2)))
                                {
                                    //Nancy,11/03/03,logic changed to if fail for one, continue the rest
                                    highVolume = "0000000000";
                                    lowVolume = "0000000000";
                                }
                                else
                                {
                                    //write to database
                                    highVolume = response.Substring(5, 10);
                                    lowVolume = response.Substring(15, 10);
                                }

                                _getPropertyManager.WriteTotalHighLow(j, highVolume, lowVolume, tothlMg);
                            }
                        }
                    }
                }


                if (isTankDipChecked)
                {
                    Variables.ReadTankDipSuccess = ReadTankDip(tillNumber, out error);
                    if (!Variables.ReadTankDipSuccess)
                    {

                        strDipMessage = _resourceManager.GetResString(offSet, (short)8398);
                    }
                }


                // SaveInHist Val(Me.txtGrade), Val(txtTier), Val(txtLevel)


                caption = _resourceManager.CreateCaption(offSet, Convert.ToInt16(14), Convert.ToInt16(21), null, (short)2) + " " + strDipMessage;


                //this.Cursor = System.Windows.Forms.Cursors.Default;

            }
            //cmdExit_.Visible = True
            //EnableButtons(true);

            //  
            object dblRecNo = 0;
            if (Chaps_Main.boolFP_HO)
            {
                //if (!cmdExit_.Enabled)
                //{
                //    cmdExit_.Enabled = true;
                //}
                fuelPrices.IsExitEnabled = true;
                //Chaps_Main.dbPump.Execute("DELETE FROM FuelPrice_HO", out dblRecNo, (System.Int32)((int)ADODB.ExecuteOptionEnum.adExecuteNoRecords + ADODB.CommandTypeEnum.adCmdText));
                _fuelPumpService.DeleteFuelPrice();
                modGlobalFunctions.BoolFuelPriceApplied = true;
                WriteToLogFile(dblRecNo + " records deleted from FuelPrice_HO table");
            }
            //   end


            if (changeFuelPriceManually)
            {

                //MsgBoxStyle temp_VbStyle5 = (int)MsgBoxStyle.Information + MsgBoxStyle.OkOnly;
                //Chaps_Main.DisplayMessage(0, (short)4266, temp_VbStyle5, null, (byte)0);
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)4266, null, MessageType.OkOnly);
                error.MessageStyle.MessageType = MessageType.Information;
                error.StatusCode = HttpStatusCode.OK;
                return caption;
            }

            return caption;


            Error1:

            //this.Cursor = System.Windows.Forms.Cursors.Default;

            //EnableButtons(true);
            //if (!cmdExit_.Enabled)

            //{
            //    cmdExit_.Enabled = true;

            //    rsGrd = null;

            //}
            fuelPrices.IsExitEnabled = true;
            caption = _resourceManager.CreateCaption(offSet, 14, 42, null, 3);
            return caption;
        }

        /// <summary>
        /// Method to read tank dip
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ReadTankDip(int tillNumber, out ErrorMessage error)
        {
            error = new ErrorMessage();
            bool returnValue = false;
            var till = _tillService.GetTill(tillNumber);
            if (till == null)
            {
                error.MessageStyle = new MessageStyle { Message = "Till does not exists" };
                return false;
            }
            float timeIn = 0;
            string strReading = "";
            string originalResponse = "";
            int volume = 0;
            float fuelDip = 0;
            float waterDip = 0;
            float temp = 0;
            int vllage = 0;
            short totMg = 0;
            string response = "";
            string strBuffer = "";
            string strRemain = "";
            DateTime readTime = default(DateTime);
            short j = 0;
            short i = 0;
            short k = 0;
            short pos = 0;
            short posStart = 0;
            short numchars = 0; //added by Dmitry to save number of bytes for TankDIP device response
            short[] arrMinus = null; //   to consider negative amounts for temperature
            short[] arrTanks = null; // positive
            bool boolNegTemp = false;
            short intBlockSize = 0;
            returnValue = false;

            response = "";
            strRemain = "";
            boolNegTemp = false;

            string tempCommandRenamed = "DIP";
            try
            {
                LoadPumps(tillNumber);
            }
            catch
            {
                error.MessageStyle = new MessageStyle { Message = "Unable to connect to server" };
                return false;
            }
            TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);

            if (timeIn > DateAndTime.Timer)
            {
                timeIn = 0; //reset on midnight
            }
            else
            {
                timeIn = (float)DateAndTime.Timer;
            }
            while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
            {

                strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting DIP");
                if (!string.IsNullOrEmpty(strBuffer))
                {
                    modStringPad.SplitResponse(strBuffer, "DIP", ref response, ref strRemain); //strBuffer<>""
                    if (!string.IsNullOrEmpty(response)) //got what we are waiting
                    {

                        TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                        WriteToLogFile("modify TCPAgent.PortReading from reading tank dip: " + strRemain);
                        break;
                    }
                }
                Variables.Sleep(100);
                if (DateAndTime.Timer < timeIn)
                {
                    timeIn = (float)DateAndTime.Timer;
                }
            }

            if ((response.Length <= 16) || !(response.Substring(0, 3) == "DIP") || !(response.Substring(response.Length - 2, 2) == "OK"))
            {
                goto Error1;
            }
            else
            {


                response = response.Substring(3, response.Length - 5);
                readTime = DateTime.Parse(response.Substring(0, 2) + "/" + response.Substring(2, 2) + "/" + Convert.ToString(DateAndTime.Year(DateAndTime.Today)) + " " + response.Substring(4, 2) + ":" + response.Substring(6, 2) + ":00");
                response = response.Substring(11);

                //   to consider negative temperatures
                boolNegTemp = response.IndexOf("-") + 1 > 0;
                arrMinus = new short[1];
                arrTanks = new short[1];
                originalResponse = response;
                if (boolNegTemp)
                {
                    k = (short)1;
                    while (k <= response.Length)
                    {
                        pos = (short)(k.ToString().IndexOf(response) + 1);
                        if (pos > 0)
                        {
                            j = (short)(arrMinus.Length - 1 + 1);
                            Array.Resize(ref arrMinus, j + 1);
                            arrMinus[j] = pos;
                            k = (short)(pos + 1);
                        }
                        else
                        {
                            k++;
                        }
                    }
                    //            Response = Replace(Response, "-", "", 1)    ' eliminate the minus sign, Arr_Minus contains the positions of minus sign in the original string
                    response = Strings.Replace(response, "-", "0", 1); // true
                }
                //   end




                //------------------------------------------------
                //Added by Dmitry to diffenciate between Incon and Veeder-Root
                var systemType = _tillCloseService.GetSystemType();
                if (systemType == "Incon")
                {
                    if (response.Length % 31 != 0)
                    {
                        goto Error1;
                    }
                    else
                    {
                        numchars = (short)31;
                        intBlockSize = (short)31;
                    }
                }
                else
                {
                    if (response.Length % 32 != 0)
                    {
                        goto Error1;
                    }
                    else
                    {
                        numchars = (short)32;
                        intBlockSize = (short)32;
                    }
                }

                //End Dmitry

                //   for negative temperatures; set the negative tanks sign
                if (boolNegTemp)
                {
                    arrTanks = new short[originalResponse.Length / intBlockSize + 1];
                    for (k = 1; k <= (double)originalResponse.Length / intBlockSize; k++)
                    {
                        if (k <= arrTanks.Length - 1 && k <= arrMinus.Length - 1)
                        {
                            arrTanks[k] = (short)((arrMinus[k] > (short)(k - 1) * numchars) && (arrMinus[k] < k * numchars) ? 1 : (short)0);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                //   end


                totMg = (short)_tillCloseService.GetMaximumDipNumber();
                //Added by Dmitry to save data in DipEvents table
                _tillCloseService.AddDipEvent(totMg, till.ShiftDate);
                //End Dmitry
                posStart = (short)1;
                k = (short)0; // data
                while (posStart < response.Length)
                {
                    strReading = response.Substring(posStart - 1, numchars);
                    k++; // data

                    i = (short)Conversion.Val(strReading.Substring(0, 1));
                    fuelDip = float.Parse((Conversion.Val(strReading.Substring(6, 5)) / 100).ToString("#0.00"));
                    volume = int.Parse(strReading.Substring(11, 6));
                    temp = float.Parse((Conversion.Val(strReading.Substring(17, 5)) / 10).ToString("#0.0"));
                    // temperature
                    if (boolNegTemp)
                    {
                        if (k <= arrTanks.Length - 1)
                        {
                            if (arrTanks[k] == 1)
                            {
                                temp = -1 * temp;
                            }
                        }
                    }
                    //   end
                    vllage = int.Parse(strReading.Substring(22, 6));
                    waterDip = float.Parse((Conversion.Val(strReading.Substring(28, 3)) / 10).ToString("#0.0"));
                    var tankDip = new TankDip
                    {
                        DipNumber = totMg,
                        TankId = i,
                        FuelDip = fuelDip,
                        WaterDip = waterDip,
                        Temperature = temp,
                        Date = DateTime.Today,
                        ShiftDate = till.ShiftDate,
                        ReadTime = readTime,
                        GradeId = _getPropertyManager.get_TankInfo((byte)i).GradeID,
                        Volume = volume,
                        Vllage = vllage
                    };

                    _tillCloseService.SaveTankDip(tankDip);


                    posStart = (short)(posStart + numchars);
                }
            }
            returnValue = true;
            arrMinus = null;
            arrTanks = null;
            return returnValue;

            Error1:
            returnValue = false;
            return returnValue;
        }


        /// <summary>
        /// Method to get tank dip report
        /// </summary>
        /// <param name="readTankDipSuccess"></param>
        /// <returns></returns>
        public string GetTankDipReport(bool? readTankDipSuccess)
        {
            string returnValue = "";
            string strTmp = "";
            string umDip = "";
            string umTemp = "";
            DateTime lastDate = default(DateTime);
            short dn = 0;

            returnValue = "";
            strTmp = "";
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (readTankDipSuccess != null && readTankDipSuccess.Value)
            {
                // 'LastDate' is the date of the last totalizer readings that were used.
                dn = _tillCloseService.GetMaxDipNumber();
                if (dn == -1)
                {
                    //        "Reading Tank Dip failed."
                    returnValue = "\r\n" + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8399), (short)40);
                    return returnValue;
                }

                var tankDips = _tillCloseService.GetTankDipByDipNumber(dn);
                lastDate = tankDips.FirstOrDefault().Date;
                string timeFormat;
                if (_policyManager.TIMEFORMAT == "24 HOURS")
                {
                    timeFormat = "hh:mm";
                }
                else
                {
                    timeFormat = "hh:mm tt";
                }
                strTmp = "\r\n";
                //    "TANK DIP REPORT"
                strTmp = strTmp + modStringPad.PadC(_resourceManager.GetResString(offSet, (short)8400), modPrint.PRINT_WIDTH) + "\r\n";
                strTmp = strTmp + modStringPad.PadC(DateAndTime.Today.ToString("dd-MMM-yyyy") + " " + DateAndTime.TimeOfDay.ToString(timeFormat), modPrint.PRINT_WIDTH) + "\r\n";
                strTmp = strTmp + modStringPad.PadC(_resourceManager.GetResString(offSet, (short)4662) + " " + lastDate.ToString("dd-MMM-yyyy") + " " + lastDate.ToString(timeFormat), (short)40) + "\r\n" + "\r\n"; //Last Readings:

                //    strTmp = strTmp & PadC("Tank", 10) & _



                // 

                ///               PadL(GetResString(8402), 12) & _
                ///               PadL(GetResString(8403), 8) & _
                ///               PadL(GetResString(8404), 10) & vbCrLf
                strTmp = strTmp + modStringPad.PadC(_resourceManager.GetResString(offSet, (short)8401), (short)5) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8402), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8403), (short)7) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8404), (short)10) + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)334), (short)8) + "\r\n"; // 
                var tankGauge = _tillCloseService.GetTankGaugeSetUp();                                                                                                                                                                                                                                                                                                                                                                                                             //shiny end
                if (tankGauge != null)
                {

                    umDip = " " + Strings.Trim(tankGauge.DipUM);

                    umTemp = " " + Strings.Trim(tankGauge.TempUM);
                }
                else
                {
                    umDip = "";
                    umTemp = "";
                }

                strTmp = strTmp + modStringPad.PadC("-", modPrint.PRINT_WIDTH, "-") + "\r\n";

                foreach (var tankDip in tankDips)
                {
                    // 

                    ///                   PadL(Format(rs![Dip_Fuel], "0.00") & UM_DIP, 12) & _
                    ///                   PadL(Format(rs![Dip_Water], "0.0"), 8) & _
                    ///                   PadL(Format(rs![TemperatureC], "0.0") & UM_Temp, 10) & vbCrLf

                    strTmp = strTmp + modStringPad.PadC(Convert.ToString(tankDip.TankId), (short)5) + modStringPad.PadL(tankDip.FuelDip.ToString("0.00") + umDip, (short)10) + modStringPad.PadL(tankDip.WaterDip.ToString("0.0"), (short)7) + modStringPad.PadL(tankDip.Temperature.ToString("0.0") + umTemp, (short)10) + modStringPad.PadL(tankDip.Volume.ToString("#####0"), (short)8) + "\r\n";
                    // 
                }

                strTmp = strTmp + "\r\n";
                returnValue = strTmp;

            }
            else
            {
                //    "Reading Tank Dip failed."
                returnValue = "\r\n" + modStringPad.PadL(_resourceManager.GetResString(offSet, (short)8399), (short)40);
            }
            return returnValue;
        }

        /// <summary>
        /// Method to load prices to display
        /// </summary>
        /// <returns></returns>
        public PriceToDisplay LoadPricesToDisplay()
        {
            return frmSelectPrice_Load();
        }

        /// <summary>
        /// Method to load on select
        /// </summary>
        /// <returns></returns>
        public PriceToDisplay frmSelectPrice_Load()
        {
            var pricesToDisplay = new PriceToDisplay();
            short i = 0;
            var rsGrade = _fuelPumpService.GetGradesForPriceToDisplay();
            var rsTier = _fuelPumpService.GetTiersForPriceToDisplay();
            var rsLevel = _fuelPumpService.GetLevelsForPriceToDisplay();

            pricesToDisplay.Grades = rsGrade?.ToList();
            pricesToDisplay.Tiers = rsTier?.ToList();
            pricesToDisplay.Levels = rsLevel?.ToList();

            for (i = 0; i < 4; i++)
            {
                pricesToDisplay.GradesState.Add(new PriceToDisplayComboBox());
                pricesToDisplay.TiersState.Add(new PriceToDisplayComboBox());
                pricesToDisplay.LevelsState.Add(new PriceToDisplayComboBox());
            }


            for (i = 1; i <= Variables.gPumps.PriceDisplayRows; i++)
            {
                //lbRowID[i].Enabled = true;
                pricesToDisplay.GradesState[i - 1].IsEnabled = true;
                pricesToDisplay.LevelsState[i - 1].IsEnabled = true;
                pricesToDisplay.TiersState[i - 1].IsEnabled = true;
                var rstPriceDisplay = _fuelPumpService.get_PricesToDisplay((byte)i);
                if (rstPriceDisplay != null)
                {
                    pricesToDisplay.GradesState[i - 1].SelectedValue = Information.IsDBNull(rstPriceDisplay.GradeID) ? "" : rstPriceDisplay.GradeID + " - " + Convert.ToString(Variables.gPumps.get_Grade(Convert.ToByte(rstPriceDisplay.GradeID))?.ShortName);
                    pricesToDisplay.LevelsState[i - 1].SelectedValue = Information.IsDBNull(rstPriceDisplay.LevelID) ? "" : rstPriceDisplay.LevelID + " - " + Convert.ToString(Variables.gPumps.get_Level(Convert.ToByte(rstPriceDisplay.LevelID)));
                    pricesToDisplay.TiersState[i - 1].SelectedValue = Information.IsDBNull(rstPriceDisplay.TierID) ? "" : rstPriceDisplay.TierID + " - " + Convert.ToString(Variables.gPumps.get_Tier(Convert.ToByte(rstPriceDisplay.TierID)));
                }
                else
                {
                    pricesToDisplay.GradesState[i - 1].SelectedValue = "";
                    pricesToDisplay.LevelsState[i - 1].SelectedValue = "";
                    pricesToDisplay.TiersState[i - 1].SelectedValue = "";
                }
            }

            //this.Cursor = System.Windows.Forms.Cursors.Arrow;
            return pricesToDisplay;
        }

        /// <summary>
        /// Method to save prices to display
        /// </summary>
        /// <param name="selectedGrades"></param>
        /// <param name="selectedTiers"></param>
        /// <param name="selectedLevels"></param>
        /// <returns></returns>
        public bool SavePricesToDisplay(List<string> selectedGrades, List<string> selectedTiers,
            List<string> selectedLevels)
        {
            return cmdSave_ClickEvent(selectedGrades, selectedTiers, selectedLevels);
        }

        /// <summary>
        /// Method to save grades
        /// </summary>
        /// <param name="selectedGrades"></param>
        /// <param name="selectedTiers"></param>
        /// <param name="selectedLevels"></param>
        /// <returns></returns>
        public bool cmdSave_ClickEvent(List<string> selectedGrades, List<string> selectedTiers,
            List<string> selectedLevels)
        {
            short i = 0;
            //cmdSave.Enabled = false;
            //cmdCancel.Enabled = false;
            for (i = 1; i <= Variables.gPumps.PriceDisplayRows; i++)
            {

                if (!_fuelPumpService.SavePriceToDisplay((byte)i,
                   Strings.Left(Convert.ToString(selectedGrades[i - 1]), 2),
                   Strings.Left(Convert.ToString(selectedLevels[i - 1]), 2),
                    Strings.Left(Convert.ToString(selectedTiers[i - 1]), 2)))
                {
                    return false;
                }
            }

            return true;
            //    cmdSave.Enabled = True
        }

        /// <summary>
        /// Method to load change increment
        /// </summary>
        /// <param name="taxExempt"></param>
        /// <returns></returns>
        public ChangeIncrement LoadChangeIncrement(bool taxExempt)
        {
            if (!taxExempt)
            {
                return frmChangeIncrement_Load();
            }
            else
            {
                return frmChangeTaxExemption_Load();
            }
        }

        /// <summary>
        /// Method to load change increment file
        /// </summary>
        /// <param name="myTierLevel"></param>
        /// <param name="myUnBaseGrade"></param>
        /// <returns></returns>
        public ChangeIncrement frmChangeIncrement_Load()
        {
            var changeIncrement = new ChangeIncrement();
            short j = 0;
            short i = 0;
            short ii = 0;

            object[] iGradeLt = new object[(int)Variables.gPumps.GradesCount + 1];

            object[] myPrice = new object[(int)Variables.gPumps.GradesCount + 1];
            var unBaseGrades = Variables.gPumps.UnBaseGasGradesCount;
            var lineCount = unBaseGrades;

            var myLines = new MyGradeType2[lineCount + 1];
            i = (short)1;
            var rstGradeIncre = _fuelPumpService.GetGradePriceIncrementIds();

            if (Variables.gPumps.myUnBaseGrade == null)
            {
                Variables.gPumps.myUnBaseGrade = new MyGradeType2[unBaseGrades + 1];
                foreach (var gradeId in rstGradeIncre)
                {
                    Variables.gPumps.myUnBaseGrade[i].GradeId = Convert.ToInt16(gradeId);
                    Variables.gPumps.myUnBaseGrade[i].GradeDesp = Convert.ToString(Variables.gPumps.get_Grade((byte)Variables.gPumps.myUnBaseGrade[i].GradeId).FullName);
                    Variables.gPumps.myUnBaseGrade[i].Price.CashP = _getPropertyManager.get_GradePriceIncrement((byte)Variables.gPumps.myUnBaseGrade[i].GradeId).CashPriceIncre;
                    Variables.gPumps.myUnBaseGrade[i].Price.CreditP = _getPropertyManager.get_GradePriceIncrement((byte)Variables.gPumps.myUnBaseGrade[i].GradeId).CreditPriceIncre;
                    i++;
                }
            }

            i = (short)1;
            foreach (var gradeId in rstGradeIncre)
            {
                if (i <= lineCount)
                {
                    myLines[i].GradeId = Variables.gPumps.myUnBaseGrade[i].GradeId;
                    myLines[i].GradeDesp = Variables.gPumps.myUnBaseGrade[i].GradeDesp;
                    myLines[i].Price.CashP = Variables.gPumps.myUnBaseGrade[i].Price.CashP;
                    myLines[i].Price.CreditP = Variables.gPumps.myUnBaseGrade[i].Price.CreditP;

                    changeIncrement.PriceIncrements.Add(new PriceIncrement
                    {
                        Row = i,
                        Cash = CommonUtility.GetDoubleValue(myLines[i].Price.CashP).ToString("0.000"),
                        Credit = CommonUtility.GetDoubleValue(myLines[i].Price.CreditP).ToString("0.000"),
                        Grade = myLines[i].GradeDesp,
                        GradeId = myLines[i].GradeId
                    });
                }
                i++;
            }

            changeIncrement.IsCreditEnabled = _policyManager.FUEL_CP;

            ii = (short)1;

            if (Variables.gPumps.myTierLevel == null)
            {
                Variables.gPumps.myTierLevel = new TierLevelType[4];
                for (i = 1; i <= 2; i++)
                {
                    for (j = 1; j <= 2; j++)
                    {
                        if (i != 1 | j != 1)
                        {
                            Variables.gPumps.myTierLevel[ii].Tier = i;
                            Variables.gPumps.myTierLevel[ii].Level = j;
                            Variables.gPumps.myTierLevel[ii].CashP = Information.IsDBNull(_getPropertyManager.get_TierLevelPriceDiff((byte)i, (byte)j).CashDiff) ? 0 : _getPropertyManager.get_TierLevelPriceDiff((byte)i, (byte)j).CashDiff;
                            Variables.gPumps.myTierLevel[ii].CreditP = Information.IsDBNull(_getPropertyManager.get_TierLevelPriceDiff((byte)i, (byte)j).CreditDiff) ? 0 : _getPropertyManager.get_TierLevelPriceDiff((byte)i, (byte)j).CreditDiff;
                            ii++;
                        }
                    }
                }
            }

            ii = (short)1;
            for (i = 1; i <= 2; i++)
            {
                for (j = 1; j <= 2; j++)
                {
                    if (i != 1 | j != 1)
                    {
                        changeIncrement.PriceDecrements.Add(new PriceDecrement
                        {
                            Row = ii,
                            Cash = CommonUtility.GetDoubleValue(Variables.gPumps.myTierLevel[ii].CashP).ToString("0.000"),
                            Credit = CommonUtility.GetDoubleValue(Variables.gPumps.myTierLevel[ii].CreditP).ToString("0.000"),
                            LevelId = j,
                            TierId = i,
                            TierLevel = Variables.gPumps.get_Tier((byte)i) + "/" + Convert.ToString(Variables.gPumps.get_Level((byte)j))
                        });
                        ii++;
                    }
                }
            }
            return changeIncrement;
        }

        /// <summary>
        /// Method to load tax exemption
        /// </summary>
        /// <param name="myTierLevel"></param>
        /// <param name="myUnBaseGrade"></param>
        /// <returns></returns>
        public ChangeIncrement frmChangeTaxExemption_Load()
        {
            var changeIncrement = new ChangeIncrement();

            double teCashPrice = 0;
            double teCreditPrice = 0;
            bool teFound = false;

            short gradeId;
            short j = 0;
            short i = 0;
            short ii = 0;
            short T;
            short l;
            bool firsttime;

            object[] iGradeLt = new object[(int)Variables.gPumps.GradesCount + 1];

            object[] myPrice = new object[(int)Variables.gPumps.GradesCount + 1];

            var unBaseGrades = Variables.gPumps.UnBaseGasGradesCount;

            var lineCount = unBaseGrades;

            var myLines = new MyGradeType2[lineCount + 1];

            i = (short)1;

            if (Variables.gPumps.myUnBaseGradeTaxExemption == null)
            {
                Variables.gPumps.myUnBaseGradeTaxExemption = new MyGradeType2[Variables.gPumps.UnBaseGasGradesCount + 1];
                for (j = 1; j <= Variables.gPumps.GradesCount; j++)
                {
                    if (Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade((byte)j).FuelType)).Trim() == "G" && Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade((byte)j).FullName)).Trim() != "REGULAR")
                    {
                        _teSystemManager.TeGetTaxFreeGradePriceIncrement(j, ref teCashPrice, ref teCreditPrice, ref teFound);
                        if (teFound)
                        {
                            Variables.gPumps.myUnBaseGradeTaxExemption[i].Price.TECashP = teCashPrice;
                            Variables.gPumps.myUnBaseGradeTaxExemption[i].Price.TECreditP = teCreditPrice;
                        }
                        else
                        {
                            Variables.gPumps.myUnBaseGradeTaxExemption[i].Price.TECashP = 0;
                            Variables.gPumps.myUnBaseGradeTaxExemption[i].Price.TECreditP = 0;
                        }
                        Variables.gPumps.myUnBaseGradeTaxExemption[i].GradeId = j;

                        Variables.gPumps.myUnBaseGradeTaxExemption[i].GradeDesp = Convert.ToString(Variables.gPumps.get_Grade((byte)j).FullName);
                        i++;
                    }
                }
            }

            i = (short)1;
            for (j = 1; j <= Variables.gPumps.GradesCount; j++)
            {
                if (Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade((byte)j).FuelType)).Trim() == "G" && Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade((byte)j).FullName)).Trim() != "REGULAR")
                {
                    if (i <= lineCount)
                    {
                        myLines[i].GradeId = Variables.gPumps.myUnBaseGradeTaxExemption[i].GradeId;
                        myLines[i].GradeDesp = Variables.gPumps.myUnBaseGradeTaxExemption[i].GradeDesp;
                        myLines[i].Price.TECashP = Variables.gPumps.myUnBaseGradeTaxExemption[i].Price.TECashP;
                        myLines[i].Price.TECreditP = Variables.gPumps.myUnBaseGradeTaxExemption[i].Price.TECreditP;

                        changeIncrement.PriceIncrements.Add(new PriceIncrement
                        {
                            Row = i,
                            Cash = CommonUtility.GetDoubleValue(myLines[i].Price.TECashP).ToString("0.000"),
                            Credit = CommonUtility.GetDoubleValue(myLines[i].Price.TECreditP).ToString("0.000"),
                            GradeId = Variables.gPumps.myUnBaseGradeTaxExemption[i].GradeId,
                            Grade = myLines[i].GradeDesp
                        });
                    }
                    i++;
                }
            }

            changeIncrement.IsCreditEnabled = _policyManager.FUEL_CP;
            ii = (short)1;
            if (Variables.gPumps.myTierLevelTaxExemption == null)
            {
                Variables.gPumps.myTierLevelTaxExemption = new TierLevelType[4];
                for (i = 1; i <= 2; i++)
                {
                    for (j = 1; j <= 2; j++)
                    {
                        if (i != 1 | j != 1)
                        {
                            Variables.gPumps.myTierLevelTaxExemption[ii].Tier = i;
                            Variables.gPumps.myTierLevelTaxExemption[ii].Level = j;
                            _teSystemManager.TeGetTaxFreeTierLevelPriceDiff(i, j, ref teCashPrice, ref teCreditPrice, ref teFound);
                            if (teFound)
                            {
                                Variables.gPumps.myTierLevelTaxExemption[ii].CashP = teCashPrice;
                                Variables.gPumps.myTierLevelTaxExemption[ii].CreditP = teCreditPrice;
                            }
                            else
                            {
                                Variables.gPumps.myTierLevelTaxExemption[ii].CashP = 0;
                                Variables.gPumps.myTierLevelTaxExemption[ii].CreditP = teCreditPrice;
                            }
                            ii++;
                        }
                    }
                }
            }

            ii = (short)1;
            for (i = 1; i <= 2; i++)
            {
                for (j = 1; j <= 2; j++)
                {
                    if (i != 1 | j != 1)
                    {
                        changeIncrement.PriceDecrements.Add(new PriceDecrement
                        {
                            Row = ii,
                            Cash = CommonUtility.GetDoubleValue(Variables.gPumps.myTierLevelTaxExemption[ii].CashP).ToString("0.000"),
                            Credit = CommonUtility.GetDoubleValue(Variables.gPumps.myTierLevelTaxExemption[ii].CreditP).ToString("0.000"),
                            LevelId = j,
                            TierId = i,
                            TierLevel = Variables.gPumps.get_Tier((byte)i) + "/" + Convert.ToString(Variables.gPumps.get_Level((byte)j))
                        });

                        ii++;
                    }
                }
            }
            return changeIncrement;
        }

        /// <summary>
        /// Method to set price increment
        /// </summary>
        /// <param name="price"></param>
        /// <param name="taxExempt"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public PriceIncrement SetPriceIncrement(PriceIncrement price, bool taxExempt, ref ErrorMessage error, ref string report)
        {
            if (!taxExempt)
            {
                frmChangeIncrement_Load();
                ctlKeyPadV1_EnterPressed(price, null, price.Cash, true, true, taxExempt, ref error);
                if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
                {
                    return null;
                }

                ctlKeyPadV1_EnterPressed(price, null, price.Credit, true, false, taxExempt, ref error);
                if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
                {
                    return null;
                }
                report = RefreshIncrement(ref error);
            }
            else
            {
                frmChangeTaxExemption_Load();
                ctlKeyPadV1_EnterPressed(price, null, price.Cash, true, true, taxExempt, ref error);
                if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
                {
                    return null;
                }
                ctlKeyPadV1_EnterPressed(price, null, price.Credit, true, false, taxExempt, ref error);
                if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
                {
                    return null;
                }
                report = RefreshIncrementForTaxExemption(ref error);
            }

            return price;
        }

        /// <summary>
        /// Method to enter value
        /// </summary>
        /// <param name="myTierLevel"></param>
        /// <param name="myUnBaseGrade"></param>
        /// <param name="priceIncrement"></param>
        /// <param name="priceDecrement"></param>
        /// <param name="keyEntry"></param>
        /// <param name="isPriceIncrement"></param>
        /// <param name="updateCashPrice"></param>
        /// <param name="taxExempt"></param>
        /// <param name="error"></param>
        public void ctlKeyPadV1_EnterPressed(
            PriceIncrement priceIncrement,
            PriceDecrement priceDecrement, string keyEntry, bool isPriceIncrement, bool updateCashPrice,
            bool taxExempt,
            ref ErrorMessage error)
        {
            error = new ErrorMessage();

            if (Conversion.Val((string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000")) > 9.999)

            {
                //MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                //Chaps_Main.DisplayMessage(this, (short)8, temp_VbStyle, null, (byte)0); 
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 43, 8, null, MessageType.OkOnly);
                error.StatusCode = HttpStatusCode.BadRequest;
                return;
            }
            if (isPriceIncrement)
            {
                if (updateCashPrice)
                {
                    priceIncrement.Cash = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000");

                    if (!taxExempt)
                    {
                        Variables.gPumps.myUnBaseGrade[priceIncrement.Row].Price.CashP = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000");
                    }
                    else
                    {
                        Variables.gPumps.myUnBaseGradeTaxExemption[priceIncrement.Row].Price.TECashP = string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry);
                    }
                    //02/11/03,if not allowing credit pricing,set credit price same as cash price


                    if (!_policyManager.FUEL_CP)
                    {

                        priceIncrement.Credit = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000");

                        if (!taxExempt)
                        {
                            Variables.gPumps.myUnBaseGrade[priceIncrement.Row].Price.CreditP = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000");
                        }
                        else
                        {
                            Variables.gPumps.myUnBaseGradeTaxExemption[priceIncrement.Row].Price.TECreditP = string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry);
                        }
                    }
                }
                else
                {
                    priceIncrement.Credit = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000");

                    if (!taxExempt)
                    {
                        Variables.gPumps.myUnBaseGrade[priceIncrement.Row].Price.CreditP = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000");
                    }
                    else
                    {
                        Variables.gPumps.myUnBaseGradeTaxExemption[priceIncrement.Row].Price.TECreditP = string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry);
                    }
                }
            }
            else
            {
                if (updateCashPrice)
                {
                    priceDecrement.Cash = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000");

                    if (!taxExempt)
                    {
                        Variables.gPumps.myTierLevel[priceDecrement.Row].CashP = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000");
                    }
                    else
                    {
                        Variables.gPumps.myTierLevelTaxExemption[priceDecrement.Row].CashP = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000");
                    }
                    //02/11/03,if not allowing credit pricing,set credit price same as cash price


                    if (!_policyManager.FUEL_CP)
                    {

                        priceDecrement.Credit = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000");

                        if (!taxExempt)
                        {
                            Variables.gPumps.myTierLevel[priceDecrement.Row].CreditP = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000");
                        }
                        else
                        {
                            Variables.gPumps.myTierLevelTaxExemption[priceDecrement.Row].CreditP = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000");
                        }
                    }
                }
                else
                {
                    priceDecrement.Credit = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000");

                    if (!taxExempt)
                    {
                        Variables.gPumps.myTierLevel[priceDecrement.Row].CreditP = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000");
                    }
                    else
                    {
                        Variables.gPumps.myTierLevelTaxExemption[priceDecrement.Row].CreditP = (string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000");
                    }
                }
            }
        }

        /// <summary>
        /// Method to set price decrement
        /// </summary>
        /// <param name="price"></param>
        /// <param name="taxExempt"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public PriceDecrement SetPriceDecrement(PriceDecrement price, bool taxExempt, ref ErrorMessage error, ref string report)
        {
            if (!taxExempt)
            {
                frmChangeIncrement_Load();
                ctlKeyPadV1_EnterPressed(null, price, price.Cash, false, true, taxExempt, ref error);
                if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
                {
                    return null;
                }

                ctlKeyPadV1_EnterPressed(null, price, price.Credit, false, false, taxExempt, ref error);
                if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
                {
                    return null;
                }

                report = RefreshIncrement(ref error);
            }
            else
            {
                frmChangeTaxExemption_Load();
                ctlKeyPadV1_EnterPressed(null, price, price.Cash, false, true, taxExempt, ref error);
                if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
                {
                    return null;
                }
                ctlKeyPadV1_EnterPressed(null, price, price.Credit, false, false, taxExempt, ref error);
                if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
                {
                    return null;
                }
                report = RefreshIncrementForTaxExemption(ref error);
            }
            return price;
        }

        public string RefreshReport(ref ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();
            MyGradeType[] myLines = default(MyGradeType[]);
            BasePrices prices = new BasePrices();
            GradeType[] myPrice = default(GradeType[]);
            MyGradeType[] myBaseGrade = default(MyGradeType[]);
            MyGradeType regularGrade = new MyGradeType();
            short baseGrades = 0;

            double teCashPrice = 0;
            double teCreditPrice = 0;

            LoadGroupBasePrices(ref prices, ref errorMessage,
                ref baseGrades, ref myPrice, ref myBaseGrade, ref myLines,
                ref regularGrade, ref teCashPrice, ref teCreditPrice, true);

            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle?.Message))
            {
                return null;
            }

            var report = string.Empty;
            if (!Chaps_Main.boolFP_HO)
            {
                CalculatePrice(ref regularGrade, ref myPrice, ref errorMessage);
            }
            DisplayReport(ref myPrice, ref report);

            return report;
        }

        /// <summary>
        /// Stop Pump Broadcasting
        /// </summary>
        /// <returns></returns>
        public bool DisableFramePump()
        {
            if (DateAndTime.Timer - Variables.basketClick < Variables.basket_click_delay && DateAndTime.Timer - Variables.basketClick > 0)
            {
                return false;
            }

            if (Variables.basketClick > DateAndTime.Timer)
            {
                Variables.basketClick = 0;
            }
            else
            {
                Variables.basketClick = (float)DateAndTime.Timer;
            }

            _displayReader = false;
            //send TCP command to notify FC don't broadcast value of this pump
            if (Variables.DisplayPumpID != 0)
            {
                string tempCommandRenamed = "Brd" + Strings.Right("0" + Convert.ToString(Variables.DisplayPumpID), 2) + "0";
                //var tcpAgent = new TCPAgent();
                TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, false);
                Variables.DisplayPumpID = (byte)0; //don't display this pump frame any more
            }
            return true;
        }

        #region Private methods

        /// <summary>
        /// Method to refresh increment
        /// </summary>
        /// <param name="myTierLevel"></param>
        /// <param name="myUnBaseGrade"></param>
        private string RefreshIncrement(ref ErrorMessage errorMessage)
        {
            short i = 0;
            short j;
            for (i = 1; i <= Variables.gPumps.UnBaseGasGradesCount; i++)
            {
                Variables.MyGradeIncre[Variables.gPumps.myUnBaseGrade[i].GradeId].CashP = Conversion.Val(Variables.gPumps.myUnBaseGrade[i].Price.CashP);
                Variables.MyGradeIncre[Variables.gPumps.myUnBaseGrade[i].GradeId].CreditP = Conversion.Val(Variables.gPumps.myUnBaseGrade[i].Price.CreditP);
            }
            for (i = 1; i <= 3; i++)
            {
                Variables.MyTLDiff[Variables.gPumps.myTierLevel[i].Tier, Variables.gPumps.myTierLevel[i].Level].CashP = Conversion.Val(Variables.gPumps.myTierLevel[i].CashP);
                Variables.MyTLDiff[Variables.gPumps.myTierLevel[i].Tier, Variables.gPumps.myTierLevel[i].Level].CreditP = Conversion.Val(Variables.gPumps.myTierLevel[i].CreditP);
            }
            return RefreshReport(ref errorMessage);
        }

        /// <summary>
        /// Method to refresh increment for tax exemption
        /// </summary>
        /// <param name="myTierLevel"></param>
        /// <param name="myUnBaseGrade"></param>
        private string RefreshIncrementForTaxExemption(ref ErrorMessage errorMessage)
        {
            short i = 0;
            short j;
            for (i = 1; i <= Variables.gPumps.UnBaseGasGradesCount; i++)
            {
                if (Variables.gPumps.myUnBaseGradeTaxExemption[i].GradeId != 0)
                {
                    Variables.MyGradeIncre[Variables.gPumps.myUnBaseGradeTaxExemption[i].GradeId].TECashP = Conversion.Val(Variables.gPumps.myUnBaseGradeTaxExemption[i].Price.TECashP.ToString());
                    Variables.MyGradeIncre[Variables.gPumps.myUnBaseGradeTaxExemption[i].GradeId].TECreditP = Conversion.Val(Variables.gPumps.myUnBaseGradeTaxExemption[i].Price.TECreditP.ToString());
                }
            }
            for (i = 1; i <= 3; i++)
            {
                Variables.MyTLDiff[Variables.gPumps.myTierLevelTaxExemption[i].Tier, Variables.gPumps.myTierLevelTaxExemption[i].Level].TECashP = Conversion.Val(Variables.gPumps.myTierLevelTaxExemption[i].CashP);
                Variables.MyTLDiff[Variables.gPumps.myTierLevelTaxExemption[i].Tier, Variables.gPumps.myTierLevelTaxExemption[i].Level].TECreditP = Conversion.Val(Variables.gPumps.myTierLevelTaxExemption[i].CreditP);
            }

            return RefreshReport(ref errorMessage);
        }

        /// <summary>
        /// Method to validate base price
        /// </summary>
        /// <param name="myPrice"></param>
        /// <param name="prices"></param>
        /// <param name="error"></param>
        /// <param name="changeFuelPriceManually"></param>
        /// <param name="currentUser"></param>
        /// <param name="isCallForVerify"></param>
        private void ValidateBasePrices(GradeType[] myPrice, ref BasePrices prices, ref ErrorMessage error, ref bool changeFuelPriceManually,
            ref User currentUser, bool isCallForVerify)
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (_policyManager.TAX_EXEMPT)
            {
                if (!HaveSetAllTeCategory(myPrice))
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 21, 92, null, MessageType.OkOnly);
                    error.StatusCode = HttpStatusCode.BadRequest;

                    prices.IsExitEnabled = true;
                    return;
                }
            }

            changeFuelPriceManually = false;
            if (!TCPAgent.Instance.PortOpened)
            {
                currentUser = Chaps_Main.User_Renamed;
                if (_policyManager.U_ManuFPrice)
                {
                    if (_policyManager.USE_FUEL && isCallForVerify) // ###FUELPRICE
                    {
                        error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 4263, null, MessageType.YesNo);
                        error.StatusCode = HttpStatusCode.Conflict;

                        return;

                    } //-###FUELPRICE
                }
                else
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, Convert.ToInt16(!_policyManager.USE_FUEL ? 4269 : 4264), null, MessageType.YesNo); //###FUELPRICE'shiny added posonly resource string
                    error.StatusCode = HttpStatusCode.Conflict;

                }

                if (_policyManager.U_ManuFPrice)
                {
                    changeFuelPriceManually = true;
                }
                else
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 21, Convert.ToInt16(!_policyManager.USE_FUEL ? 4270 : 4265), null, MessageType.YesNo); //###FUELPRICE'shiny added posonly resource string
                    error.StatusCode = HttpStatusCode.Conflict;

                    currentUser = null;

                    prices.IsExitEnabled = true;
                    return;
                }
                Chaps_Main.User_Renamed = currentUser;

                currentUser = null;
            }
        }

        /// <summary>
        /// Method to save price
        /// </summary>
        /// <param name="myPrice"></param>
        /// <param name="g"></param>
        /// <param name="T"></param>
        /// <param name="l"></param>
        /// <param name="reportId"></param>
        /// <param name="dateTime"></param>
        private void Save_Price(GradeType[] myPrice, short g, short T, short l, int reportId, DateTime dateTime)
        {
            CFuelPrice gFuelStruct = default(CFuelPrice);
            short i = 0;
            short j = 0;
            //Dim T As Integer
            //Dim L As Integer

            //   For T = 1 To 2
            //       For L = 1 To 2

            gFuelStruct = new CFuelPrice();

            gFuelStruct.CashPrice = Convert.ToSingle(myPrice[g].Price[T, l].CashP);

            gFuelStruct.CreditPrice = Convert.ToSingle(myPrice[g].Price[T, l].CreditP);


            gFuelStruct.teCashPrice = (float)myPrice[g].Price[T, l].TECashP;
            gFuelStruct.teCreditPrice = (float)myPrice[g].Price[T, l].TECreditP;



            gFuelStruct.EmplID = UserCode; //Employee.EmployeeID
            gFuelStruct.Date_Time = (int)dateTime.ToOADate(); //  Now()
            gFuelStruct.ReportID = reportId; // gPumps.FuelPrice(G, T, L).ReportID + 1

            _getPropertyManager.set_FuelPrice(ref Variables.gPumps, (byte)g, (byte)T, (byte)l, gFuelStruct);


            if (_policyManager.TAX_EXEMPT)
            {



                string tempSProductKey = _teSystemManager.TeMakeFuelKey(g, T, l);
                _teSystemManager.SetTaxFreeFuelPrice(ref tempSProductKey, 0, myPrice[g].Price[T, l].TECashP, myPrice[g].Price[T, l].TECreditP, UserCode);

            }

            SaveInHist(myPrice, (byte)g, (byte)T, (byte)l);
            for (i = 1; i <= Variables.gPumps.PumpsCount; i++)
            {

                for (j = 1; j <= Variables.gPumps.get_PositionsCount((byte)i); j++)
                {
                    if (CommonUtility.GetIntergerValue(Variables.gPumps.get_Assignment((byte)i, (byte)j).GradeID) == g)
                    {
                        if (CommonUtility.GetIntergerValue(Variables.gPumps.get_Pump((byte)i).TierID) == T && CommonUtility.GetIntergerValue(Variables.gPumps.get_Pump((byte)i).LevelID) == l)
                        {

                            Variables.Pump[i].cashUP[j] = CommonUtility.GetFloatValue(myPrice[g].Price[T, l].CashP);

                            Variables.Pump[i].creditUP[j] = CommonUtility.GetFloatValue(myPrice[g].Price[T, l].CreditP);
                        }
                    }
                }
            }
            //           SaveInHist G, T, L

            gFuelStruct = null;
            //       Next L
            //   Next T

        }

        /// <summary>
        /// Method to validate price
        /// </summary>
        /// <param name="keyEntry"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool ValidatePrice(string keyEntry, ref ErrorMessage message)
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (Conversion.Val((string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000")) > 9.999)
            {
                //MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                //Chaps_Main.DisplayMessage(this, (short)22, temp_VbStyle, null, (byte)0); 
                message.MessageStyle = _resourceManager.CreateMessage(offSet, 21, 22, null, MessageType.OkOnly);
                message.StatusCode = HttpStatusCode.BadRequest;
                return false;
            }

            if (Conversion.Val((string.IsNullOrEmpty(keyEntry) ? 0 : CommonUtility.GetDoubleValue(keyEntry)).ToString("0.000")) <= 0)
            {
                //MsgBoxStyle temp_VbStyle2 = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                //Chaps_Main.DisplayMessage(this, (short)23, temp_VbStyle2, null, (byte)0); 
                message.MessageStyle = _resourceManager.CreateMessage(offSet, 21, 23, null, MessageType.OkOnly);
                message.StatusCode = HttpStatusCode.BadRequest;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method to fill up prices
        /// </summary>
        /// <param name="myPrice"></param>
        /// <param name="fuelPrices"></param>
        /// <param name="gradeId"></param>
        /// <param name="tierId"></param>
        /// <param name="levelId"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool FillUp_Prices(ref GradeType[] myPrice, ref BasePrices fuelPrices, short gradeId, short tierId, short levelId, short row)
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;

            var fuelPrice = new FuelPrice();
            fuelPrice.Row = row;
            if (tierId == 1 && levelId == 1)
            {
                fuelPrice.GradeId = gradeId;
                fuelPrice.Grade = Convert.ToString(Variables.gPumps.get_Grade((byte)gradeId).FullName);
                fuelPrice.TierId = 1;

                fuelPrice.Tier = Convert.ToString(Variables.gPumps.get_Tier((byte)1));
                fuelPrice.LevelId = 1;

                fuelPrice.Level = Convert.ToString(Variables.gPumps.get_Level((byte)1));

                fuelPrice.CashPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[1, 1].CashP).ToString("0.000");
                if (fuelPrice.CashPrice == "")
                {
                    fuelPrice.CashPrice = _resourceManager.GetResString(offSet, (short)288);
                }

                fuelPrice.CreditPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[1, 1].CreditP).ToString("0.000");
                if (fuelPrice.CreditPrice == "")
                {
                    fuelPrice.CreditPrice = _resourceManager.GetResString(offSet, (short)288);
                }

                fuelPrice.TaxExemptedCashPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[1, 1].TECashP).ToString("0.000");
                fuelPrice.TaxExemptedCreditPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[1, 1].TECreditP).ToString("0.000");
                if (fuelPrice.TaxExemptedCashPrice == "" || fuelPrice.TaxExemptedCashPrice == "0.000")
                {
                    fuelPrice.TaxExemptedCashPrice = _resourceManager.GetResString(offSet, (short)288);
                }
                if (fuelPrice.TaxExemptedCreditPrice == "" || fuelPrice.TaxExemptedCreditPrice == "0.000")
                {
                    fuelPrice.TaxExemptedCreditPrice = _resourceManager.GetResString(offSet, (short)288);
                }

                fuelPrices.FuelPrices.Add(fuelPrice);
            }

            else if (tierId == 1 && levelId == 2)
            {


                fuelPrice.GradeId = gradeId;
                fuelPrice.Grade = Convert.ToString(Variables.gPumps.get_Grade((byte)gradeId).FullName);
                fuelPrice.TierId = 1;

                fuelPrice.Tier = Convert.ToString(Variables.gPumps.get_Tier((byte)1));
                fuelPrice.LevelId = 2;

                fuelPrice.Level = Convert.ToString(Variables.gPumps.get_Level((byte)2));

                fuelPrice.CashPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[1, 2].CashP).ToString("0.000");
                if (fuelPrice.CashPrice == "")
                {
                    fuelPrice.CashPrice = _resourceManager.GetResString(offSet, (short)288); //"n/a"
                }

                fuelPrice.CreditPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[1, 2].CreditP).ToString("0.000");
                if (fuelPrice.CreditPrice == "")
                {
                    fuelPrice.CreditPrice = _resourceManager.GetResString(offSet, (short)288);
                }

                fuelPrice.TaxExemptedCashPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[1, 2].TECashP).ToString("0.000");
                fuelPrice.TaxExemptedCreditPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[1, 2].TECreditP).ToString("0.000");
                if (fuelPrice.TaxExemptedCashPrice == "" || fuelPrice.TaxExemptedCashPrice == "0.000")
                {
                    fuelPrice.TaxExemptedCashPrice = _resourceManager.GetResString(offSet, (short)288);
                }
                if (fuelPrice.TaxExemptedCreditPrice == "" || fuelPrice.TaxExemptedCreditPrice == "0.000")
                {
                    fuelPrice.TaxExemptedCreditPrice = _resourceManager.GetResString(offSet, (short)288);
                }
                fuelPrices.FuelPrices.Add(fuelPrice);
            }

            else if (tierId == 2 && levelId == 1)
            {


                fuelPrice.GradeId = gradeId;
                fuelPrice.Grade = Convert.ToString(Variables.gPumps.get_Grade((byte)gradeId).FullName);
                fuelPrice.TierId = 2;

                fuelPrice.Tier = Convert.ToString(Variables.gPumps.get_Tier((byte)2));
                fuelPrice.LevelId = 1;

                fuelPrice.Level = Convert.ToString(Variables.gPumps.get_Level((byte)1));
                fuelPrice.CashPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[2, 1].CashP).ToString("0.000");
                if (fuelPrice.CashPrice == "")
                {
                    fuelPrice.CashPrice = _resourceManager.GetResString(offSet, (short)288);
                }
                fuelPrice.CreditPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[2, 1].CreditP).ToString("0.000");
                if (fuelPrice.CreditPrice == "")
                {
                    fuelPrice.CreditPrice = _resourceManager.GetResString(offSet, (short)288);
                }

                fuelPrice.TaxExemptedCashPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[2, 1].TECashP).ToString("0.000");
                fuelPrice.TaxExemptedCreditPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[2, 1].TECreditP).ToString("0.000");
                if (fuelPrice.TaxExemptedCashPrice == "" || fuelPrice.TaxExemptedCashPrice == "0.000")
                {
                    fuelPrice.TaxExemptedCashPrice = _resourceManager.GetResString(offSet, (short)288);
                }
                if (fuelPrice.TaxExemptedCreditPrice == "" || fuelPrice.TaxExemptedCreditPrice == "0.000")
                {
                    fuelPrice.TaxExemptedCreditPrice = _resourceManager.GetResString(offSet, (short)288);
                }
                fuelPrices.FuelPrices.Add(fuelPrice);
            }

            else if (tierId == 2 && levelId == 2)
            {

                fuelPrice.GradeId = gradeId;
                fuelPrice.Grade = Convert.ToString(Variables.gPumps.get_Grade((byte)gradeId).FullName);
                fuelPrice.TierId = 2;

                fuelPrice.Tier = Convert.ToString(Variables.gPumps.get_Tier((byte)2));
                fuelPrice.LevelId = 2;

                fuelPrice.Level = Convert.ToString(Variables.gPumps.get_Level((byte)2));

                fuelPrice.CashPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[2, 2].CashP).ToString("0.000");
                if (fuelPrice.CashPrice == "")
                {
                    fuelPrice.CashPrice = _resourceManager.GetResString(offSet, (short)288);
                }
                fuelPrice.CreditPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[2, 2].CreditP).ToString("0.000");
                if (fuelPrice.CreditPrice == "")
                {
                    fuelPrice.CreditPrice = _resourceManager.GetResString(offSet, (short)288);
                }
                fuelPrice.TaxExemptedCashPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[2, 2].TECashP).ToString("0.000");
                fuelPrice.TaxExemptedCreditPrice = CommonUtility.GetDoubleValue(myPrice[gradeId].Price[2, 2].TECreditP).ToString("0.000");
                if (fuelPrice.TaxExemptedCashPrice == "" || fuelPrice.TaxExemptedCashPrice == "0.000")
                {
                    fuelPrice.TaxExemptedCashPrice = _resourceManager.GetResString(offSet, (short)288);
                }
                if (fuelPrice.TaxExemptedCreditPrice == "" || fuelPrice.TaxExemptedCreditPrice == "0.000")
                {
                    fuelPrice.TaxExemptedCreditPrice = _resourceManager.GetResString(offSet, (short)288);
                }
                fuelPrices.FuelPrices.Add(fuelPrice);
            }
            return true;
        }

        /// <summary>
        /// Method to send sale
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="boolStackSale">Stack sale</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        private bool Send_Sale(short index, bool boolStackSale, out ErrorMessage error)
        {
            bool returnValue = false;
            error = new ErrorMessage();
            XML xmlRenamed = default(XML);
            bool gotResponse = false;
            short timeOut = 0;
            float timeIn = 0;

            xmlRenamed = new XML(_policyManager);
            if (!xmlRenamed.Create_Basket_XML(index, boolStackSale))
            {
                return false;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            _xmlComm = new Comm();
            _xmlComm.CommData += XMLComm_CommData;
            if (!_xmlComm.Initialize(Convert.ToString(_policyManager.FUELONLY_IP), (short)Conversion.Val(_policyManager.FUELONLY_PRT), (short)1))
            {
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 38, 73, null, CriticalOkMessageType)
                };
                return false;
            }

            if (!_xmlComm.IsConnected)
            {
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 38, 73, null, CriticalOkMessageType)
                };
                return false;
            }

            _responseBuffer = ""; //clear the response buffer

            if (!_xmlComm.SendData(Convert.ToString(xmlRenamed.xmlString)))
            {
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 38, 73, null, CriticalOkMessageType)
                };
                return false;
            }

            // Got Response from the Loyalty Server
            timeOut = Convert.ToInt16(_policyManager.FUELONLY_TMT);
            timeIn = (float)DateAndTime.Timer;
            while (!(DateAndTime.Timer - timeIn > timeOut))
            {
                if (!string.IsNullOrEmpty(_responseBuffer))
                {
                    gotResponse = true;
                    break;
                }
                if (DateAndTime.Timer < timeIn)
                {
                    timeIn = (float)DateAndTime.Timer;
                }
            }

            if (!gotResponse)
            {
                _xmlComm.WriteToLog("No response from Server.");
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 38, 73, null, CriticalOkMessageType)
                };
                return false;
            }
            if (Convert.ToBoolean(_responseBuffer.IndexOf("OK", StringComparison.Ordinal) + 1))
            {
                returnValue = true;
            }

            return returnValue;
        }

        /// <summary>
        /// Method to save xml communication data
        /// </summary>
        /// <param name="data">Data</param>
        private void XMLComm_CommData(string data)
        {
            _xmlComm.WriteToLog("Received from server " + data);
            _responseBuffer = data;
        }

        /// <summary>
        /// Method to load my prices
        /// </summary>
        /// <param name="myPrice"></param>
        /// <param name="iGradeLt"></param>
        /// <param name="fuelPrices"></param>
        private void LoadMyPrices(ref GradeType[] myPrice,
            ref short[] iGradeLt, ref BasePrices fuelPrices)
        {
            short i = default(short);
            short j = default(short);
            short ii = default(short);
            short iPointerMax = default(short);
            short T = default(short);
            short l = default(short);
            bool firsttime = default(bool);
            byte dummyGradeQty = default(byte);
            List<FuelPrice> rsFuelPriceHo;
            double teCashPriceHo = default(double);
            double teCreditPriceHo = default(double);
            double teCashPrice = default(double);
            double teCreditPrice = default(double);
            bool teFound = default(bool);
            short iPointer = default(short);

            fuelPrices.IsErrorEnabled = false;

            if (_policyManager.FUELPR_HO && Chaps_Main.boolFP_HO)
            {
                fuelPrices.IsExitEnabled = Convert.ToBoolean(_policyManager.FPR_USER); // if the user cannot reject fuel price change from HO, then disable the exit button
            }

            _getPropertyManager.Read_FuelPrice(ref Variables.gPumps, _policyManager.TAX_EXEMPT, _policyManager.TE_ByRate, _policyManager.TE_Type);
            iGradeLt = new short[(int)Variables.gPumps.GradesCount + 1];

            myPrice = new GradeType[(int)Variables.gPumps.GradesCount + 1];
            for (int x = 0; x < myPrice.Length; x++)
            {
                myPrice[x].Price = new PriceType[3, 3];
            }
            for (i = 1; i <= Variables.gPumps.GradesCount; i++)
            {
                ii++;
                if (Variables.gPumps.get_Grade((byte)i).ShortName != "" && Variables.gPumps.get_Grade((byte)i).FuelType != "O")
                {

                    if (!firsttime)
                    {
                        dummyGradeQty = (byte)(ii - 1);
                    }
                    if (!firsttime)
                    {
                        firsttime = !firsttime;
                    }
                    if (j == 0)
                    {
                        j = i;
                    }
                    iPointerMax = (short)(ii * 4);
                    iGradeLt[ii] = i;

                    myPrice[i].Grade = i;
                }
            }

            for (i = 1; i <= myPrice.Length - 1; i++)
            {

                if ((int)myPrice[i].Grade != 0)
                {
                    for (T = 1; T <= 2; T++)
                    {
                        for (l = 1; l <= 2; l++)
                        {
                            myPrice[i].Price[T, l] = new PriceType();
                            // HO
                            if (Chaps_Main.boolFP_HO)
                            {

                                rsFuelPriceHo = _fuelPumpService.GetHeadOfficeFuelPrices(myPrice[i].Grade, T, l);
                                teCashPriceHo = 0;
                                teCreditPriceHo = 0;
                                if (rsFuelPriceHo != null && rsFuelPriceHo.Count > 0)
                                {
                                    myPrice[i].Price[T, l].CashP = Information.IsDBNull(rsFuelPriceHo[0].CashPrice) ? "0" : rsFuelPriceHo[0].CashPrice;
                                    myPrice[i].Price[T, l].CreditP = Information.IsDBNull(rsFuelPriceHo[0].CreditPrice) ? "0" : rsFuelPriceHo[0].CreditPrice;
                                    if (_policyManager.TAX_EXEMPT)
                                    {
                                        teCashPriceHo = Convert.ToDouble(Information.IsDBNull(rsFuelPriceHo[0].TaxExemptedCashPrice) ? "0" : rsFuelPriceHo[0].TaxExemptedCashPrice);
                                        teCreditPriceHo = Convert.ToDouble(Information.IsDBNull(rsFuelPriceHo[0].TaxExemptedCreditPrice) ? "0" : rsFuelPriceHo[0].TaxExemptedCreditPrice);
                                    }
                                }

                                if (CommonUtility.GetDoubleValue(myPrice[i].Price[T, l].CashP) == 0)
                                {
                                    myPrice[i].Price[T, l].CashP = Variables.gPumps.get_FuelPrice((byte)i, (byte)T, (byte)l).CashPrice;
                                }

                                if (CommonUtility.GetDoubleValue(myPrice[i].Price[T, l].CreditP) == 0)
                                {
                                    myPrice[i].Price[T, l].CreditP = Variables.gPumps.get_FuelPrice((byte)i, (byte)T, (byte)l).CreditPrice;
                                }
                            }
                            else
                            {
                                myPrice[i].Price[T, l].CashP = Variables.gPumps.get_FuelPrice((byte)i, (byte)T, (byte)l).CashPrice;
                                myPrice[i].Price[T, l].CreditP = Variables.gPumps.get_FuelPrice((byte)i, (byte)T, (byte)l).CreditPrice;

                            }
                            if (_policyManager.TAX_EXEMPT)
                            {
                                if (Chaps_Main.boolFP_HO)
                                {
                                    myPrice[i].Price[T, l].TECashP = teCashPriceHo;
                                    myPrice[i].Price[T, l].TECreditP = teCreditPriceHo;
                                    if (teCashPriceHo == 0 | teCreditPriceHo == 0)
                                    {
                                        string tempSProductKey = _teSystemManager.TeMakeFuelKey(i, (short)1, (short)1);
                                        _teSystemManager.TeGetTaxFreeFuelPrice(ref tempSProductKey, ref teCashPrice, ref teCreditPrice, ref teFound);
                                        if (teFound)
                                        {
                                            if (teCashPriceHo == 0)
                                            {
                                                myPrice[i].Price[T, l].TECashP = teCashPrice;
                                            }
                                            if (teCreditPriceHo == 0)
                                            {
                                                myPrice[i].Price[T, l].TECreditP = teCreditPrice;
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    //   end
                                    string tempSProductKey2 = _teSystemManager.TeMakeFuelKey(i, (short)1, (short)1);
                                    _teSystemManager.TeGetTaxFreeFuelPrice(ref tempSProductKey2, ref teCashPrice, ref teCreditPrice, ref teFound);
                                    if (teFound)
                                    {
                                        myPrice[i].Price[T, l].TECashP = teCashPrice;
                                        myPrice[i].Price[T, l].TECreditP = teCreditPrice;
                                    }
                                }
                            }

                            iPointer = (short)(dummyGradeQty * 4 + 1);
                            if (!string.IsNullOrEmpty(Variables.gPumps.get_Grade((byte)i).ShortName) && Variables.gPumps.get_Grade((byte)i).FuelType != "O")
                            {
                                FillUp_Prices(ref myPrice, ref fuelPrices, (short)myPrice[i].Grade, T, l, i);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method to check if price error or not
        /// </summary>
        /// <param name="myPrice">Grade types</param>
        /// <returns>True or false</returns>
        private bool PriceError(GradeType[] myPrice)
        {
            short m = 0;
            short i = 0;
            short n = 0;
            for (i = 1; i <= Variables.gPumps.GradesCount; i++)
            {
                for (m = 1; m <= 2; m++)
                {
                    for (n = 1; n <= 2; n++)
                    {
                        if (Convert.ToDouble(myPrice[i].Price[m, n].CashP) > Convert.ToDouble(myPrice[i].Price[m, n].CreditP))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Method to check if all tax exempt category is set
        /// </summary>
        /// <param name="myPrice">Grade types</param>
        /// <returns>True or false</returns>
        private bool HaveSetAllTeCategoryForGroupPrices(GradeType[] myPrice)
        {
            short i = 0;
            mPrivateGlobals.teProductEnum vCatg = default(mPrivateGlobals.teProductEnum);

            if (_policyManager.TE_Type != "QITE")
            {
                for (i = 1; i <= myPrice.Length - 1; i++)
                {
                    if ((int)myPrice[i].Grade != 0 && Variables.gPumps.get_Grade((byte)i).FullName != "" && Variables.gPumps.get_Grade(Convert.ToByte(myPrice[i].Grade)).FuelType != "O")
                    {
                        if (_teSystemManager.TeGetCategory(Convert.ToString(Variables.gPumps.get_Grade(Convert.ToByte(myPrice[i].Grade)).Stock_Code), ref vCatg))
                        {
                            if (vCatg == mPrivateGlobals.teProductEnum.eNone)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Method to have all te category
        /// </summary>
        /// <param name="myPrice">Prices</param>
        /// <returns></returns>
        private bool HaveSetAllTeCategory(GradeType[] myPrice)
        {
            short i = 0;
            mPrivateGlobals.teProductEnum vCatg = default(mPrivateGlobals.teProductEnum);

            for (i = 1; i <= myPrice.Length - 1; i++)
            {
                if ((int)myPrice[i].Grade != 0 && Variables.gPumps.get_Grade(Convert.ToByte(myPrice[i].Grade)).FuelType != "O")
                {
                    if (_teSystemManager.TeGetCategory(Convert.ToString(Variables.gPumps.get_Grade(Convert.ToByte(myPrice[i].Grade)).Stock_Code), ref vCatg))
                    {
                        if (vCatg == mPrivateGlobals.teProductEnum.eNone)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Method to send to printer
        /// </summary>
        /// <param name="tmpstr">String</param>
        private string SendToPrinter(string tmpstr)
        {
            string fileName = "";
            short nH = 0;
            bool blPrintHeader = false;
            short oldReportId = (short)0;

            fileName = Path.GetTempPath() + "/FuelPrice.txt";
            if (blPrintHeader == false)
            {
                Variables.DeleteFile(fileName);
            }
            string timeFormat;
            if (_policyManager.TIMEFORMAT == "24 HOURS")
            {
                timeFormat = "hh:mm:ss";
            }
            else
            {
                timeFormat = "hh:mm:ss tt";
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;

            nH = (short)FileSystem.FreeFile();
            FileSystem.FileOpen(nH, fileName, OpenMode.Append);

            if (blPrintHeader == false)
            {
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)289) + Convert.ToString(oldReportId + 1)); //  "        Fuel Price Change Report:"
                FileSystem.PrintLine(nH, new string('-', 40));
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)290) + Convert.ToString(DateAndTime.Today.ToString("dd-MM-yyy")) + " " + DateAndTime.TimeOfDay.ToString(timeFormat)); //  Date & Time:   '  
                FileSystem.PrintLine(nH);
                FileSystem.PrintLine(nH, _resourceManager.GetResString(offSet, (short)291) + UserCode); // Employee.EmployeeID ' Employee #:
                FileSystem.PrintLine(nH);
            }
            FileSystem.PrintLine(nH);
            FileSystem.PrintLine(nH, "!!!ATTENTION PLEASE!!!");
            FileSystem.PrintLine(nH, tmpstr);
            FileSystem.FileClose(nH);

            var fileSTr = File.ReadAllText(fileName);
            return fileSTr;
        }

        /// <summary>
        /// Method to display price through Fuel change
        /// </summary>
        /// <param name="price1">First price</param>
        /// <param name="price2">Second price</param>
        /// <param name="price3">Third price</param>
        /// <param name="price4">Fourth prices</param>
        private string DisplayPriceViaFc(string price1, string price2, string price3, string price4)
        {
            var caption2 = string.Empty;
            float timeIn = 0;
            string strBuffer = "";
            string response = "";
            string strRemain = "";
            string strSend = "";
            var offSet = _policyManager.LoadStoreInfo().OffSet;

            if (!TCPAgent.Instance.IsConnected)
            {
                if (_policyManager.FuelPriceChg)
                {
                    caption2 = _resourceManager.CreateCaption(offSet, (short)67, 42, null, (short)0);
                }
                else
                {
                    SendToPrinter(_resourceManager.GetResString(offSet, (short)4267));
                }
                return caption2;
            }
            else
            {
                if (_policyManager.FuelPriceChg)
                {
                    //lblDisplaynfo.Visible = false;
                }
            }

            strSend = "DPS";
            strSend = Convert.ToString(strSend + Convert.ToString(string.IsNullOrEmpty(price1) ? "0000" : price1));
            strSend = Convert.ToString(strSend + Convert.ToString(string.IsNullOrEmpty(price2) ? "0000" : price2));
            strSend = Convert.ToString(strSend + Convert.ToString(string.IsNullOrEmpty(price3) ? "0000" : price3));
            strSend = Convert.ToString(strSend + Convert.ToString(string.IsNullOrEmpty(price4) ? "0000" : price4));

            response = "";
            strRemain = "";
            TCPAgent.Instance.Send_TCP(ref strSend, false);

            if (timeIn > DateAndTime.Timer)
            {
                timeIn = 0; //reset on midnight
            }
            else
            {
                timeIn = (float)DateAndTime.Timer;
            }

            strBuffer = TCPAgent.Instance.PortReading;
            WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting " + strSend);

            if (!string.IsNullOrEmpty(strBuffer))
            {

                modStringPad.SplitResponse(strBuffer, "DPS", ref response, ref strRemain); //strBuffer<>""

                if (!string.IsNullOrEmpty(response)) //got what we are waiting
                {
                    TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                    WriteToLogFile("modify TCPAgent.PortReading from Display Price: " + strRemain);
                }
            }
            if (response != "DPSOK")
            {
                if (_policyManager.FuelPriceChg)
                {
                    caption2 = _resourceManager.CreateCaption(offSet, (short)68, 42, null, (short)0);
                }
                else
                {
                    //Send to printer
                    SendToPrinter(_resourceManager.GetResString(offSet, 4268));
                }
            }
            else
            {
                if (_policyManager.FuelPriceChg)
                {
                    //lblDisplaynfo.Visible = false;
                }
            }
            return caption2;
        }

        /// <summary>
        /// Method to save price increment
        /// </summary>
        /// <param name="g">Grade id</param>
        /// <param name="T">Tier id</param>
        /// <param name="l">Level id</param>
        /// <param name="reportId">Report time</param>
        /// <param name="dateTime">Date</param>
        /// <param name="myPrice">New price</param>
        /// <param name="messages">Messages</param>
        private string Save_GroupPrice(short g, short T, short l, int reportId, DateTime dateTime, ref GradeType[] myPrice,
            ref List<MessageStyle> messages)
        {
            string sendToPrinter = string.Empty;
            CTierLevelPriceDiff tmpTierLevelDiff = default(CTierLevelPriceDiff);
            CGradePriceIncrement tmpGradePriceIncrement = default(CGradePriceIncrement);
            CFuelPrice gFuelStruct = default(CFuelPrice);
            short i = 0;
            short j = 0;

            var offSet = _policyManager.LoadStoreInfo().OffSet;
            gFuelStruct = new CFuelPrice();
            gFuelStruct.CashPrice = Convert.ToSingle(myPrice[g].Price[T, l].CashP);
            gFuelStruct.CreditPrice = Convert.ToSingle(myPrice[g].Price[T, l].CreditP);


            gFuelStruct.teCashPrice = (float)myPrice[g].Price[T, l].TECashP;
            gFuelStruct.teCreditPrice = (float)myPrice[g].Price[T, l].TECreditP;
            gFuelStruct.EmplID = UserCode;
            gFuelStruct.Date_Time = (int)dateTime.ToOADate();
            gFuelStruct.ReportID = reportId; _getPropertyManager.set_FuelPrice(ref Variables.gPumps, (byte)g, (byte)T, (byte)l, gFuelStruct);

            if (_policyManager.TAX_EXEMPT)
            {
                string tempSProductKey = _teSystemManager.TeMakeFuelKey(g, T, l);
                if (!_teSystemManager.SetTaxFreeFuelPrice(ref tempSProductKey, 0, myPrice[g].Price[T, l].TECashP, myPrice[g].Price[T, l].TECreditP, Convert.ToString(UserCode)))
                {
                    if (_policyManager.FuelPriceChg)
                    {
                        messages.Add(_resourceManager.CreateMessage(offSet, 42, 60, g + "/" + Convert.ToString(T) + "/", MessageType.OkOnly));
                    }
                    else
                    {
                        //Send message to the printer
                        sendToPrinter = SendToPrinter(Strings.Left(_resourceManager.GetResString(offSet, 4260), 52));
                    }
                }
            }

            SaveInHist(myPrice, (byte)g, (byte)T, (byte)l);
            for (i = 1; i <= Variables.gPumps.PumpsCount; i++)
            {
                for (j = 1; j <= Variables.gPumps.get_PositionsCount((byte)i); j++)
                {
                    if (CommonUtility.GetIntergerValue(Variables.gPumps.get_Assignment((byte)i, (byte)j).GradeID) == g)
                    {
                        if (CommonUtility.GetIntergerValue(Variables.gPumps.get_Pump((byte)i).TierID) == T &&
                            CommonUtility.GetIntergerValue(Variables.gPumps.get_Pump((byte)i).LevelID) == l)
                        {
                            Variables.Pump[i].cashUP[j] = CommonUtility.GetFloatValue(myPrice[g].Price[T, l].CashP);
                            Variables.Pump[i].creditUP[j] = CommonUtility.GetFloatValue(myPrice[g].Price[T, l].CreditP);
                        }
                    }
                }
            }

            //save to GradePriceIncrement table
            tmpGradePriceIncrement = new CGradePriceIncrement();
            if (Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade((byte)g).FuelType)).Trim() == "G" && Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade((byte)g).FullName)).Trim() != "REGULAR")
            {
                tmpGradePriceIncrement.CashPriceIncre = Variables.MyGradeIncre[g].CashP;
                tmpGradePriceIncrement.CreditPriceIncre = Variables.MyGradeIncre[g].CreditP;
                _getPropertyManager.set_GradePriceIncrement((byte)g, tmpGradePriceIncrement);
            }

            if (_policyManager.TAX_EXEMPT)
            {
                if (Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade((byte)g).FuelType)).Trim() == "G" && Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade((byte)g).FullName)).Trim() != "REGULAR")
                {
                    if (!_teSystemManager.TeSetTaxFreeGradePriceIncrement(g, Variables.MyGradeIncre[g].TECashP, Variables.MyGradeIncre[g].TECreditP))
                    {
                        if (_policyManager.FuelPriceChg)
                        {
                            messages.Add(_resourceManager.CreateMessage(offSet, 42, 61, g, MessageType.OkOnly));
                        }
                        else
                        {
                            //Send message tot the printer
                            sendToPrinter = SendToPrinter(Strings.Left(_resourceManager.GetResString(offSet, 4261), 53));
                        }
                    }
                }
            }

            //save to TierLevelPriceDiff table
            tmpTierLevelDiff = new CTierLevelPriceDiff();
            if (T != 1 | l != 1)
            {
                tmpTierLevelDiff.CashDiff = Conversion.Val(Variables.MyTLDiff[T, l].CashP);
                tmpTierLevelDiff.CreditDiff = Conversion.Val(Variables.MyTLDiff[T, l].CreditP);
                _getPropertyManager.set_TierLevelPriceDiff((byte)T, (byte)l, tmpTierLevelDiff);
            }

            if (_policyManager.TAX_EXEMPT)
            {
                if (T != 1 | l != 1)
                {
                    if (!_teSystemManager.TeSetTaxFreeTierLevelPriceDiff(T, l, Variables.MyTLDiff[T, l].TECashP, Variables.MyTLDiff[T, l].TECreditP))
                    {
                        if (_policyManager.FuelPriceChg)
                        {
                            messages.Add(_resourceManager.CreateMessage(offSet, 42, 62, T + "/" + Convert.ToString(l), MessageType.OkOnly));
                        }
                        else
                        {
                            sendToPrinter = SendToPrinter(Strings.Left(_resourceManager.GetResString(offSet, 4262), 63));
                        }
                    }
                }
            }
            return sendToPrinter;
        }

        /// <summary>
        /// Method to save in history
        /// </summary>
        /// <param name="myPrice">New price</param>
        /// <param name="gradeId">Grade id</param>
        /// <param name="tierId">Tier id</param>
        /// <param name="levelId">Level id</param>
        private void SaveInHist(GradeType[] myPrice, byte gradeId, byte tierId, byte levelId)
        {
            CFuelPrice gFuelStruct = default(CFuelPrice);
            gFuelStruct = new CFuelPrice();

            gFuelStruct.CashPrice = Convert.ToSingle(myPrice[gradeId].Price[tierId, levelId].CashP);
            gFuelStruct.CreditPrice = Convert.ToSingle(myPrice[gradeId].Price[tierId, levelId].CreditP);
            gFuelStruct.EmplID = UserCode; //Employee.EmployeeID
            gFuelStruct.Date_Time = (int)DateTime.Now.ToOADate();
            gFuelStruct.ReportID = Variables.gPumps.get_FuelPrice(gradeId, tierId, levelId).ReportID;
            _getPropertyManager.set_PutPriceinHist(gradeId, tierId, levelId, gFuelStruct);
        }

        /// <summary>
        /// Method to get last reading for pump
        /// </summary>
        /// <param name="pumpId">Pump id</param>
        /// <param name="gradeId">Grade id</param>
        /// <param name="volume">Volume</param>
        /// <param name="amount">Amount</param>
        private void GetFromLastReading(short pumpId, short gradeId, string volume, ref string amount)
        {
            var rsTotLastReading = _fuelPumpService.GetTotalizerHist(0, pumpId, gradeId);
            if (rsTotLastReading != null && rsTotLastReading.Count > 0)
            {
                rsTotLastReading = rsTotLastReading.OrderByDescending(x => x.GroupNumber).ToList();

                volume = Convert.ToInt32(rsTotLastReading.FirstOrDefault().Volume * 1000).ToString("0000000000");
                amount = (rsTotLastReading.FirstOrDefault().Dollars * 100).ToString("0000000000");
            }
        }

        /// <summary>
        /// Method to laod pump group price
        /// </summary>
        /// <param name="myPrice">Grade type</param>
        /// <param name="myBaseGrade">Base grade</param>
        /// <param name="regularGrade">Regular grade</param>
        /// <param name="prices">Prices</param>
        /// <param name="report">Report</param>
        /// <param name="errorMessage">Error message</param>
        private void frmPumpGroupPrice_Load(ref GradeType[] myPrice, ref MyGradeType[] myBaseGrade, ref MyGradeType regularGrade,
            ref BasePrices prices, ref string report,
            out ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();
            MyGradeType[] myLines = default(MyGradeType[]);
            dynamic lbTierLevel = new object();
            short baseGrades = 0;
            double teCashPrice = 0;
            double teCreditPrice = 0;
            short gradeId;
            bool firsttime;

            //check User's policy if this user can change Price Increment or not
            if (_policyManager.U_FUELGP)
            {
                prices.IsIncrementEnabled = true;
            }
            else
            {
                prices.IsCreditPriceEnabled = false;
            }

            LoadGroupBasePrices(ref prices, ref errorMessage,
                ref baseGrades, ref myPrice, ref myBaseGrade, ref myLines,
                ref regularGrade, ref teCashPrice, ref teCreditPrice, false);

            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle?.Message))
            {
                return;
            }

            DisplayReport(ref myPrice, ref report);
            prices.IsPricesToDisplayEnabled = false;

            if (_policyManager.PRICEDISPLAY)
            {
                prices.IsPricesToDisplayChecked = true;
            }

            prices.IsPricesToDisplayEnabled = Convert.ToBoolean(prices.IsPricesToDisplayEnabled);



            GetTaxExemption(ref prices);
            SetPriceFrame(prices.IsTaxExemptionVisible);
            prices.IsReadTotalizerChecked = _policyManager.DftRdTotal;
            prices.IsReadTankDipEnabled = _policyManager.TankGauge && _policyManager.U_DipRead;
            prices.IsReadTankDipChecked = _policyManager.TankGauge && _policyManager.DftRdTankDip && _policyManager.U_DipRead;
            if (!TCPAgent.Instance.IsConnected)
            {
                prices.CanReadTotalizer = false;
                prices.IsReadTotalizerEnabled = false;
                prices.IsReadTotalizerChecked = false;
                prices.IsReadTankDipChecked = false;
                prices.IsReadTankDipEnabled = false;
            }

        }

        /// <summary>
        /// Method to get tax exemption
        /// </summary>
        /// <param name="isTaxExemptionVisible">Tax exemption visible or not</param>
        private void GetTaxExemption(ref BasePrices basePrices)
        {
            if (_policyManager.TAX_EXEMPT)
            {
                if (_policyManager.TE_ByRate == false) //SITE & TE-By Rate
                {
                    basePrices.IsTaxExemptionVisible = true;
                }
                else if (_policyManager.TE_ByRate == true)
                {
                    if (_policyManager.TE_Type == "SITE")
                    {
                        basePrices.IsTaxExemptionVisible = true;
                    }
                    else
                    {
                        basePrices.IsTaxExemptionVisible = false;
                    }
                }
            }
            else
            {
                basePrices.IsTaxExemptionVisible = false;
            }
        }

        /// <summary>
        /// Method to load base prices
        /// </summary>
        /// <param name="prices">Fuel prices</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="isExitEnabled">Exit enabled or not</param>
        /// <param name="baseGrades">Base grades</param>
        /// <param name="myPrice">New grade price</param>
        /// <param name="myBaseGrade">New base grade</param>
        /// <param name="myLines">Grade typee</param>
        /// <param name="regularGrade">Regular grade</param>
        /// <param name="teCashPrice">Tax exempt cash price</param>
        /// <param name="teCreditPrice">Tax exempt credit price</param>
        private void LoadGroupBasePrices(ref BasePrices prices,
            ref ErrorMessage errorMessage,
            ref short baseGrades, ref GradeType[] myPrice,
            ref MyGradeType[] myBaseGrade, ref MyGradeType[] myLines, ref MyGradeType regularGrade,
            ref double teCashPrice, ref double teCreditPrice, bool loadingForSave)
        {
            short j = 0;
            short i = 0;
            short ii = 0;
            short T = 0;
            short l = 0;
            short m = 0;
            short uBnd = 0;
            double teCashPriceHo = default(double);
            double teCreditPriceHo = default(double);

            bool teFound = false;

            short[] arrGrade = new short[1];

            if (!_policyManager.U_CHGFPRICE && !_policyManager.FUELPR_HO) //   added And Not Policy.FUELPR_HO, if fuel prices are controlled from HeadOffice the policy U_CHGFPRICE should not matter, as the user cannot edit the prices in the fuel price form. This is by design.
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 38, 75, null, MessageType.OkOnly);
                errorMessage.StatusCode = HttpStatusCode.Forbidden;
                return;
            }

            if (_policyManager.U_CHGFPRICE || (_policyManager.FUELPR_HO && Chaps_Main.boolFP_HO)) //   added Or (Policy.FUELPR_HO And boolFP_HO), if fuel prices are controlled from HeadOffice the policy U_CHGFPRICE should not matter, as the user cannot edit the prices in the fuel price form. This is by design.
            {

            }
            else
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 38, 75, null, MessageType.OkOnly);
                errorMessage.StatusCode = HttpStatusCode.Forbidden;
                return;
            }

            for (var index = 0; index < Variables.gPumps.GradesCount + 1; index++)
            {
                prices.FuelPrices.Add(new FuelPrice());
            }

            if (_policyManager.FUELPR_HO && Chaps_Main.boolFP_HO)
            {
                prices.IsCashPriceEnabled = false;
                prices.IsCreditPriceEnabled = false;
                prices.IsTaxExemptedCashPriceEnabled = false;
                prices.IsExitEnabled = Convert.ToBoolean(_policyManager.FPR_USER);
                i = (short)0;
            }

            _getPropertyManager.Read_FuelPrice(ref Variables.gPumps, _policyManager.TAX_EXEMPT, _policyManager.TE_ByRate, _policyManager.TE_Type);

            short[] iGradeLt = new short[(int)Variables.gPumps.GradesCount + 1];
            myPrice = new GradeType[(int)Variables.gPumps.GradesCount + 1];
            for (int x = 0; x < myPrice.Length; x++)
            {
                myPrice[x].Price = new PriceType[3, 3];
            }
            short topLine = (short)1;
            int lineCount = Variables.gPumps.GradesCount;

            ii = (short)0;
            for (i = 1; i <= Variables.gPumps.GradesCount; i++)
            {

                if ((Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade((byte)i).FuelType)).Trim() == "D") || (Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade((byte)i).FuelType)).Trim() == "G" && Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade((byte)i).FullName)).Trim() == "REGULAR"))
                {
                    ii++;
                }
            }
            baseGrades = ii;

            if (!_policyManager.FUEL_CP)
            {
                prices.IsCreditPriceEnabled = false;
                prices.IsTaxExemptedCreditPriceEnabled = false;
            }

            if (!loadingForSave)
            {
                for (i = 1; i <= 2; i++)
                {
                    for (j = 1; j <= 2; j++)
                    {
                        Variables.MyTLDiff[i, j].CashP = Information.IsDBNull(_getPropertyManager.get_TierLevelPriceDiff((byte)i, (byte)j).CashDiff) ? 0 : _getPropertyManager.get_TierLevelPriceDiff((byte)i, (byte)j).CashDiff;
                        Variables.MyTLDiff[i, j].CreditP = Information.IsDBNull(_getPropertyManager.get_TierLevelPriceDiff((byte)i, (byte)j).CreditDiff) ? 0 : _getPropertyManager.get_TierLevelPriceDiff((byte)i, (byte)j).CreditDiff;
                        if (_policyManager.TAX_EXEMPT)
                        {
                            _teSystemManager.TeGetTaxFreeTierLevelPriceDiff(i, j, ref teCashPrice, ref teCreditPrice, ref teFound);
                            if (teFound)
                            {
                                Variables.MyTLDiff[i, j].TECashP = teCashPrice;
                                Variables.MyTLDiff[i, j].TECreditP = teCreditPrice;
                            }
                        }
                    }
                }
            }

            myBaseGrade = new MyGradeType[baseGrades + 1];
            for (var x = 0; x < myBaseGrade.Length; x++)
            {
                myBaseGrade[x].Price = new PriceType[3, 3];
            }
            myLines = new MyGradeType[lineCount + 1];
            for (var x = 0; x < myLines.Length; x++)
            {
                myLines[x].Price = new PriceType[3, 3];
            }
            if (!loadingForSave)
            {
                Variables.MyGradeIncre = new PriceType[(int)Variables.gPumps.GradesCount + 1];
            }

            ii = (short)0;
            for (i = 1; i <= Variables.gPumps.GradesCount; i++)
            {
                myPrice[i].Grade = i;
                if (Chaps_Main.boolFP_HO)
                {
                    if (!(Variables.gPumps.get_Grade((byte)i).FullName == null) && Variables.gPumps.get_Grade((byte)i).FuelType != "O")
                    {
                        uBnd = (short)(arrGrade.Length - 1);
                        Array.Resize(ref arrGrade, uBnd + 1 + 1);
                        arrGrade[uBnd + 1] = i;
                    }

                    var rsFuelPriceHo = _fuelPumpService.GetHeadOfficeFuelPrices(i, 1, 1);
                    if (rsFuelPriceHo != null && rsFuelPriceHo.Count > 0)
                    {
                        myPrice[i].Price[1, 1].CashP = rsFuelPriceHo[0].CashPrice;
                        myPrice[i].Price[1, 1].CreditP = rsFuelPriceHo[0].CreditPrice;
                        if (_policyManager.TAX_EXEMPT)
                        {
                            myPrice[i].Price[1, 1].TECashP = CommonUtility.GetDoubleValue(rsFuelPriceHo[0].TaxExemptedCashPrice);
                            myPrice[i].Price[1, 1].TECreditP = CommonUtility.GetDoubleValue(rsFuelPriceHo[0].TaxExemptedCreditPrice);
                        }
                    }
                    if (CommonUtility.GetDoubleValue(myPrice[i].Price[1, 1].CashP) == 0)
                    {
                        myPrice[i].Price[1, 1].CashP = Variables.gPumps.get_FuelPrice((byte)i, (byte)1, (byte)1).CashPrice;
                    }
                    if (CommonUtility.GetDoubleValue(myPrice[i].Price[1, 1].CreditP) == 0)
                    {
                        myPrice[i].Price[1, 1].CreditP = Variables.gPumps.get_FuelPrice((byte)i, (byte)1, (byte)1).CreditPrice;
                    }
                    if (_policyManager.TAX_EXEMPT)
                    {
                        if (myPrice[i].Price[1, 1].TECashP == 0)
                        {
                            myPrice[i].Price[1, 1].TECashP = Variables.gPumps.get_FuelPrice((byte)i, (byte)1, (byte)1).teCashPrice;
                        }
                        if (myPrice[i].Price[1, 1].TECreditP == 0)
                        {
                            myPrice[i].Price[1, 1].TECreditP = Variables.gPumps.get_FuelPrice((byte)i, (byte)1, (byte)1).teCashPrice;
                        }
                    }
                }
                else
                {
                    myPrice[i].Price[1, 1].CashP = Variables.gPumps.get_FuelPrice((byte)i, (byte)1, (byte)1).CashPrice; //IIf(gPumps.FuelPrice(i, 1, 1).cashPrice = "", 0, gPumps.FuelPrice(i, 1, 1).cashPrice)
                    myPrice[i].Price[1, 1].CreditP = Variables.gPumps.get_FuelPrice((byte)i, (byte)1, (byte)1).CreditPrice; //IIf(gPumps.FuelPrice(i, 1, 1).creditPrice = "", 0, gPumps.FuelPrice(i, 1, 1).creditPrice)
                }
                if (_policyManager.TAX_EXEMPT)
                {
                    if (_policyManager.TE_ByRate == false || (_policyManager.TE_ByRate && _policyManager.TE_Type == "SITE"))
                    {
                        string tempSProductKey = _teSystemManager.TeMakeFuelKey(i, (short)1, (short)1);
                        _teSystemManager.TeGetTaxFreeFuelPrice(ref tempSProductKey, ref teCashPrice, ref teCreditPrice, ref teFound);
                        if (teFound)
                        {
                            myPrice[i].Price[1, 1].TECashP = teCashPrice;
                            myPrice[i].Price[1, 1].TECreditP = teCreditPrice;
                        }
                    }
                }

                if ((Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade((byte)i).FuelType)).Trim() == "D") || (Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade((byte)i).FuelType)).Trim() == "G" && Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade((byte)i).FullName)).Trim() == "REGULAR"))
                {

                    ii++;
                    myBaseGrade[ii].GradeId = i;
                    myBaseGrade[ii].GradeDesp = Convert.ToString(Variables.gPumps.get_Grade((byte)i).FullName);
                    myBaseGrade[ii].Price[1, 1].CashP = myPrice[i].Price[1, 1].CashP;
                    myBaseGrade[ii].Price[1, 1].CreditP = myPrice[i].Price[1, 1].CreditP;

                    myBaseGrade[ii].Price[1, 1].TECashP = myPrice[i].Price[1, 1].TECashP;
                    myBaseGrade[ii].Price[1, 1].TECreditP = myPrice[i].Price[1, 1].TECreditP;
                    if (ii <= lineCount)
                    {
                        myLines[ii].GradeId = i;
                        myLines[ii].GradeDesp = myBaseGrade[ii].GradeDesp;
                        myLines[ii].Price[1, 1].CashP = myBaseGrade[ii].Price[1, 1].CashP;
                        myLines[ii].Price[1, 1].CreditP = myBaseGrade[ii].Price[1, 1].CreditP;
                        myLines[ii].Price[1, 1].TECashP = myBaseGrade[ii].Price[1, 1].TECashP;
                        myLines[ii].Price[1, 1].TECreditP = myBaseGrade[ii].Price[1, 1].TECreditP;
                        prices.FuelPrices[ii].Row = ii;
                        prices.FuelPrices[ii].Grade = Variables.gPumps.get_Grade((byte)i).FullName;
                        prices.FuelPrices[ii].CashPrice = String.Format(CommonUtility.GetDecimalValue(myPrice[i].Price[1, 1].CashP).ToString("0.000"));
                        prices.FuelPrices[ii].CreditPrice = String.Format(CommonUtility.GetDecimalValue(myPrice[i].Price[1, 1].CreditP).ToString("0.000"));
                        prices.FuelPrices[ii].TaxExemptedCashPrice = String.Format(CommonUtility.GetDecimalValue(myPrice[i].Price[1, 1].TECashP).ToString("0.000"));
                        prices.FuelPrices[ii].TaxExemptedCreditPrice = String.Format(CommonUtility.GetDecimalValue(myPrice[i].Price[1, 1].TECreditP).ToString("0.000"));

                    }

                    if (!loadingForSave)
                    {
                        Variables.MyGradeIncre[i].CashP = 0;
                        Variables.MyGradeIncre[i].CreditP = 0;
                        Variables.MyGradeIncre[i].TECashP = 0;
                        Variables.MyGradeIncre[i].TECreditP = 0;
                    }

                    if (Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade((byte)i).FuelType)).Trim() == "G")
                    {
                        regularGrade.GradeId = i;
                        regularGrade.GradeDesp = Convert.ToString(Variables.gPumps.get_Grade((byte)i).FullName);
                        regularGrade.Price = new PriceType[3, 3];
                        regularGrade.Price[1, 1].CashP = myPrice[i].Price[1, 1].CashP;
                        regularGrade.Price[1, 1].CreditP = myPrice[i].Price[1, 1].CreditP;
                        regularGrade.Price[1, 1].TECashP = myPrice[i].Price[1, 1].TECashP;
                        regularGrade.Price[1, 1].TECreditP = myPrice[i].Price[1, 1].TECreditP;
                    }
                }
                else
                {
                    if (!loadingForSave)
                    {
                        Variables.MyGradeIncre[i].CashP = Information.IsDBNull(_getPropertyManager.get_GradePriceIncrement((byte)i).CashPriceIncre) ? 0 : _getPropertyManager.get_GradePriceIncrement((byte)i).CashPriceIncre;
                        Variables.MyGradeIncre[i].CreditP = Information.IsDBNull(_getPropertyManager.get_GradePriceIncrement((byte)i).CreditPriceIncre) ? 0 : _getPropertyManager.get_GradePriceIncrement((byte)i).CreditPriceIncre;
                        if (_policyManager.TAX_EXEMPT)
                        {
                            _teSystemManager.TeGetTaxFreeGradePriceIncrement(i, ref teCashPrice, ref teCreditPrice, ref teFound);
                            if (teFound)
                            {
                                Variables.MyGradeIncre[i].TECashP = teCashPrice;
                                Variables.MyGradeIncre[i].TECreditP = teCreditPrice;
                            }
                        }
                    }
                }
            }

            if (Chaps_Main.boolFP_HO)
            {
                for (m = 1; m <= arrGrade.Length - 1; m++)
                {
                    i = arrGrade[m];
                    for (T = 1; T <= 2; T++)
                    {
                        for (l = 1; l <= 2; l++)
                        {
                            var rsFuelPriceHo = _fuelPumpService.GetHeadOfficeFuelPrices(i, T, l);
                            if (rsFuelPriceHo != null && rsFuelPriceHo.Count > 0)
                            {
                                myPrice[i].Price[T, l].CashP = rsFuelPriceHo[0].CashPrice;
                                myPrice[i].Price[T, l].CreditP = rsFuelPriceHo[0].CreditPrice;
                                if (_policyManager.TAX_EXEMPT)
                                {
                                    myPrice[i].Price[T, l].TECashP = CommonUtility.GetDoubleValue(rsFuelPriceHo[0].TaxExemptedCashPrice);
                                    myPrice[i].Price[T, l].TECreditP = CommonUtility.GetDoubleValue(rsFuelPriceHo[0].TaxExemptedCreditPrice);
                                }
                            }
                            if (CommonUtility.GetDoubleValue(myPrice[i].Price[T, l].CashP) == 0)
                            {
                                myPrice[i].Price[T, l].CashP = Variables.gPumps.get_FuelPrice((byte)i, (byte)T, (byte)l).CashPrice;
                            }
                            if (CommonUtility.GetDoubleValue(myPrice[i].Price[T, l].CreditP) == 0)
                            {
                                myPrice[i].Price[T, l].CreditP = Variables.gPumps.get_FuelPrice((byte)i, (byte)T, (byte)l).CreditPrice;
                            }
                            if (_policyManager.TAX_EXEMPT)
                            {
                                if (myPrice[i].Price[T, l].TECashP == 0)
                                {
                                    myPrice[i].Price[T, l].TECashP = Variables.gPumps.get_FuelPrice((byte)i, (byte)T, (byte)l).teCashPrice;
                                }
                                if (myPrice[i].Price[T, l].TECreditP == 0)
                                {
                                    myPrice[i].Price[T, l].TECreditP = Variables.gPumps.get_FuelPrice((byte)i, (byte)T, (byte)l).teCashPrice;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                CalculatePrice(ref regularGrade, ref myPrice, ref errorMessage);
            }

            prices.FuelPrices = prices.FuelPrices.Where(x => x.Row != 0).ToList();
        }

        /// <summary>
        /// Method to calculate price
        /// </summary>
        /// <param name="regularGrade">Regular grade</param>
        /// <param name="myPrice">My price</param>
        /// <param name="errorMessage">Error message</param>
        private void CalculatePrice(ref MyGradeType regularGrade, ref GradeType[] myPrice, ref ErrorMessage errorMessage)
        {
            dynamic blPriceUp;

            short i = 0;
            object m = null;
            short n = 0;
            bool priceTooBig = false;

            if (Chaps_Main.boolFP_HO)
            {
                return;
            }

            for (i = 1; i <= (int)Variables.gPumps.GradesCount; i = (short)(i + 1))
            {
                myPrice[(int)i].Grade = i;
                if (Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade(Convert.ToByte(i)).FuelType)).Trim() == "G" && Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade(Convert.ToByte(i)).FullName)).Trim() != "REGULAR")
                {
                    myPrice[(int)i].Price[1, 1].CashP = Conversion.Val(regularGrade.Price[1, 1].CashP) + Conversion.Val(Variables.MyGradeIncre[(int)i].CashP);
                    myPrice[(int)i].Price[1, 1].CreditP = Conversion.Val(regularGrade.Price[1, 1].CreditP) + Conversion.Val(Variables.MyGradeIncre[(int)i].CreditP);
                    if ((Conversion.Val(myPrice[(int)i].Price[1, 1].CashP) > 9.999) || (Conversion.Val(myPrice[(int)i].Price[1, 1].CreditP) > 9.999))
                    {
                        priceTooBig = true;
                        if (Conversion.Val(myPrice[(int)i].Price[1, 1].CashP) > 9.999)
                        {
                            myPrice[(int)i].Price[1, 1].CashP = 9.999;
                        }
                        if (Conversion.Val(myPrice[(int)i].Price[1, 1].CreditP) > 9.999)
                        {
                            myPrice[(int)i].Price[1, 1].CreditP = 9.999;
                        }
                    }

                    if (_policyManager.TAX_EXEMPT && _policyManager.TE_ByRate && _policyManager.TE_Type != "SITE") //  
                    {
                        myPrice[(int)i].Price[1, 1].TECashP = Convert.ToDouble(_fuelPumpService.GetTePriceByRate(Convert.ToInt16(i), Convert.ToSingle(myPrice[(int)i].Price[1, 1].CashP), Variables.gPumps.get_Grade((byte)Convert.ToInt16(i)).Stock_Code));
                        myPrice[(int)i].Price[1, 1].TECreditP = Convert.ToDouble(_fuelPumpService.GetTePriceByRate(Convert.ToInt16(i), Convert.ToSingle(myPrice[(int)i].Price[1, 1].CreditP), Variables.gPumps.get_Grade((byte)Convert.ToInt16(i)).Stock_Code));
                    }
                    else
                    {
                        myPrice[(int)i].Price[1, 1].TECashP = Conversion.Val(regularGrade.Price[1, 1].TECashP.ToString()) + Conversion.Val(Variables.MyGradeIncre[(int)i].TECashP.ToString());
                        myPrice[(int)i].Price[1, 1].TECreditP = Conversion.Val(regularGrade.Price[1, 1].TECreditP.ToString()) + Conversion.Val(Variables.MyGradeIncre[(int)i].TECreditP.ToString());

                    }
                    if ((Conversion.Val(myPrice[(int)i].Price[1, 1].TECashP.ToString()) > 9.999) || (Conversion.Val(myPrice[(int)i].Price[1, 1].TECreditP.ToString()) > 9.999))
                    {
                        priceTooBig = true;
                        if (Conversion.Val(myPrice[(int)i].Price[1, 1].TECashP.ToString()) > 9.999)
                        {
                            myPrice[(int)i].Price[1, 1].TECashP = 9.999;
                        }
                        if (Conversion.Val(myPrice[(int)i].Price[1, 1].TECreditP.ToString()) > 9.999)
                        {
                            myPrice[(int)i].Price[1, 1].TECreditP = 9.999;
                        }
                    }
                }
                else if (Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade(Convert.ToByte(i)).FuelType)).Trim() == "D" || (Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade(Convert.ToByte(i)).FuelType)).Trim() == "G" && Strings.UCase(Convert.ToString(Variables.gPumps.get_Grade(Convert.ToByte(i)).FullName)).Trim() == "REGULAR"))
                {
                    if (_policyManager.TAX_EXEMPT && _policyManager.TE_ByRate && _policyManager.TE_Type != "SITE") //  
                    {
                        myPrice[(int)i].Price[1, 1].TECashP = Convert.ToDouble(_fuelPumpService.GetTePriceByRate(Convert.ToInt16(i), Convert.ToSingle(myPrice[(int)i].Price[1, 1].CashP), Variables.gPumps.get_Grade((byte)Convert.ToInt16(i)).Stock_Code));
                        myPrice[(int)i].Price[1, 1].TECreditP = Convert.ToDouble(_fuelPumpService.GetTePriceByRate(Convert.ToInt16(i), Convert.ToSingle(myPrice[(int)i].Price[1, 1].CreditP), Variables.gPumps.get_Grade((byte)Convert.ToInt16(i)).Stock_Code));
                    }

                }

                for (m = 1; (int)m <= 2; m = (int)m + 1)
                {
                    for (n = 1; n <= 2; n++)
                    {
                        if ((int)m != 1 | n != 1) //if not (Tier 1 & Level 1)
                        {
                            if (!(myPrice[(int)i].Price[(int)m, n].CashP == null))
                            {
                                if (Convert.ToDouble(myPrice[(int)i].Price[(int)m, n].CashP) < Conversion.Val(myPrice[(int)i].Price[1, 1].CashP) + Conversion.Val(Variables.MyTLDiff[(int)m, n].CashP))
                                {
                                    blPriceUp = true;
                                }
                                else
                                {
                                    blPriceUp = false;
                                }
                            }
                            myPrice[(int)i].Price[(int)m, n].CashP = Conversion.Val(myPrice[(int)i].Price[1, 1].CashP) + Conversion.Val(Variables.MyTLDiff[(int)m, n].CashP);
                            myPrice[(int)i].Price[(int)m, n].CreditP = Conversion.Val(myPrice[(int)i].Price[1, 1].CreditP) + Conversion.Val(Variables.MyTLDiff[(int)m, n].CreditP);
                            if ((Conversion.Val(myPrice[(int)i].Price[(int)m, n].CashP) > 9.999) || (Conversion.Val(myPrice[(int)i].Price[(int)m, n].CreditP) > 9.999))
                            {
                                priceTooBig = true;
                                if (Conversion.Val(myPrice[(int)i].Price[(int)m, n].CashP) > 9.999)
                                {
                                    myPrice[(int)i].Price[(int)m, n].CashP = 9.999;
                                }
                                if (Conversion.Val(myPrice[(int)i].Price[(int)m, n].CreditP) > 9.999)
                                {
                                    myPrice[(int)i].Price[(int)m, n].CreditP = 9.999;
                                }
                            }

                            if (_policyManager.TAX_EXEMPT && _policyManager.TE_ByRate && _policyManager.TE_Type != "SITE") //  
                            {
                                myPrice[(int)i].Price[(int)m, n].TECashP = Convert.ToDouble(_fuelPumpService.GetTePriceByRate(Convert.ToInt16(i), Convert.ToSingle(myPrice[(int)i].Price[(int)m, n].CashP), Variables.gPumps.get_Grade((byte)Convert.ToInt16(i)).Stock_Code));
                                myPrice[(int)i].Price[(int)m, n].TECreditP = Convert.ToDouble(_fuelPumpService.GetTePriceByRate(Convert.ToInt16(i), Convert.ToSingle(myPrice[(int)i].Price[(int)m, n].CreditP), Variables.gPumps.get_Grade((byte)Convert.ToInt16(i)).Stock_Code));
                            }
                            else
                            {
                                myPrice[(int)i].Price[(int)m, n].TECashP = Conversion.Val(myPrice[(int)i].Price[1, 1].TECashP.ToString()) + Conversion.Val(Variables.MyTLDiff[(int)m, n].TECashP.ToString());
                                myPrice[(int)i].Price[(int)m, n].TECreditP = Conversion.Val(myPrice[(int)i].Price[1, 1].TECreditP.ToString()) + Conversion.Val(Variables.MyTLDiff[(int)m, n].TECreditP.ToString());
                            }

                            if ((Conversion.Val(myPrice[(int)i].Price[(int)m, n].TECashP.ToString()) > 9.999) || (Conversion.Val(myPrice[(int)i].Price[(int)m, n].TECreditP.ToString()) > 9.999))
                            {
                                priceTooBig = true;
                                if (Conversion.Val(myPrice[(int)i].Price[(int)m, n].TECashP.ToString()) > 9.999)
                                {
                                    myPrice[(int)i].Price[(int)m, n].TECashP = 9.999;
                                }
                                if (Conversion.Val(myPrice[(int)i].Price[(int)m, n].TECreditP.ToString()) > 9.999)
                                {
                                    myPrice[(int)i].Price[(int)m, n].TECreditP = 9.999;
                                }
                            }
                        }
                    }
                }
            }

            if (priceTooBig)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 42, 19, null, MessageType.OkOnly);
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
            }
        }

        /// <summary>
        /// Method to display report
        /// </summary>
        /// <param name="myPrice">Grade type</param>
        /// <param name="tmpstr">Report</param>
        private void DisplayReport(ref GradeType[] myPrice, ref string tmpstr)
        {
            short j = 0;
            short i = 0;
            short m = 0;
            tmpstr = "";
            bool toDisplay = false;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            tmpstr = "";
            tmpstr = "\r\n" + _resourceManager.GetResString(offSet, (short)358) + "\r\n" + "\r\n";

            for (i = 1; i <= 2; i++)
            {
                for (j = 1; j <= 2; j++)
                {
                    toDisplay = true;
                    if (!(i == 1 & j == 1))
                    {
                        if (_policyManager.TAX_EXEMPT)
                        {
                            if (CommonUtility.GetDoubleValue(Variables.MyTLDiff[i, j].CashP) == 0 && CommonUtility.GetDoubleValue(Variables.MyTLDiff[i, j].CreditP) == 0 && CommonUtility.GetDoubleValue(Variables.MyTLDiff[i, j].TECashP) == 0 && CommonUtility.GetDoubleValue(Variables.MyTLDiff[i, j].TECreditP) == 0)
                            {
                                toDisplay = false; // Exit For
                            }
                        }
                        else
                        {
                            if (CommonUtility.GetDoubleValue(Variables.MyTLDiff[i, j].CashP) == 0 && CommonUtility.GetDoubleValue(Variables.MyTLDiff[i, j].CreditP) == 0)
                            {
                                toDisplay = false; //Exit For
                            }
                        }
                    }
                    if (toDisplay)
                    {
                        tmpstr = tmpstr + Convert.ToString(Variables.gPumps.get_Tier((byte)i)) + " & " + Convert.ToString(Variables.gPumps.get_Level((byte)j)) + " :" + "\r\n";
                        tmpstr = tmpstr + _resourceManager.GetResString(offSet, (short)359) + "\r\n";

                        if (_policyManager.TAX_EXEMPT)
                        {
                            tmpstr = tmpstr + _resourceManager.GetResString(offSet, (short)375) + "\r\n";
                        }
                        for (m = 1; m <= Variables.gPumps.GradesCount; m++)
                        {
                            if (Variables.gPumps.get_Grade((byte)m).FuelType != "O" && Variables.gPumps.get_GradeIsExist((byte)m))
                            {
                                if (!_policyManager.TAX_EXEMPT)
                                {
                                    tmpstr = tmpstr + " " + Strings.Left(Variables.gPumps.get_Grade((byte)m).FullName + ":             ", 15) + String.Format(CommonUtility.GetDecimalValue(myPrice[m].Price[i, j].CashP).ToString("0.000")) + "       " + String.Format(CommonUtility.GetDecimalValue(myPrice[m].Price[i, j].CreditP).ToString("0.000")) + "\r\n";
                                }
                                else
                                {
                                    tmpstr = tmpstr + Strings.Left(Variables.gPumps.get_Grade((byte)m).FullName + ":        ", 9) + " " + String.Format(CommonUtility.GetDecimalValue(myPrice[m].Price[i, j].CashP).ToString("0.000")) + "/" + String.Format(CommonUtility.GetDecimalValue(myPrice[m].Price[i, j].TECashP).ToString("0.000")) + "  " + String.Format(CommonUtility.GetDecimalValue(myPrice[m].Price[i, j].CreditP).ToString("0.000")) + "/" + String.Format(CommonUtility.GetDecimalValue(myPrice[m].Price[i, j].TECreditP).ToString("0.000")) + "\r\n";

                                }
                            }
                        }
                        tmpstr = tmpstr + "\r\n";
                    }
                }
            }
        }

        /// <summary>
        /// Method to set price frame
        /// </summary>
        /// <param name="withTaxExempt">Tax exempt added or not</param>
        private void SetPriceFrame(bool withTaxExempt)
        {
            dynamic lblCashOrg = null;
            dynamic lblCashTe = null;
            bool isTaxExemptionVisible = false;
            dynamic lblCreditOrg = null;
            dynamic lblCreditTe = null;

            if (withTaxExempt)
            {
                isTaxExemptionVisible = true;
            }
            else
            {
                isTaxExemptionVisible = false;
            }
        }


        /// <summary>
        /// Method to load grade for pump
        /// </summary>
        /// <param name="pumpId">Pump id</param>
        /// <returns>List of grades</returns>
        private List<string> ctlKeyPadV1_EnterPressed(short pumpId)
        {
            if (pumpId == 0)
            {
                return null;
            }
            return LoadGradeForPump(pumpId);
        }

        /// <summary>
        /// Method to get list of grade for pump
        /// </summary>
        /// <param name="pumpId">Pump id</param>
        /// <returns>List of grade pumps</returns>
        private List<string> LoadGradeForPump(short pumpId)
        {
            short i = 0;
            short j = 0;

            var pumpGrades = new List<string>();

            if (Variables.gPumps == null)
            {
                Variables.gPumps = _fuelPumpService.InitializeGetProperty(PosId, _policyManager.TAX_EXEMPT, _policyManager.TE_ByRate, _policyManager.TE_Type, _policyManager.AuthPumpPOS);
            }
            if (Variables.gPumps.get_Pump((byte)pumpId).MaxPositionID > 0)
            {

                for (j = 1; j <= Variables.gPumps.GradesCount; j++)
                {
                    for (i = 1; i <= Variables.gPumps.get_Pump((byte)pumpId).MaxPositionID; i++)
                    {
                        var assignment = Variables.gPumps.get_Assignment((byte)pumpId, (byte)i);
                        if (assignment != null && assignment.GradeID != null &&
                            (int)assignment.GradeID == j)
                        {

                            pumpGrades.Add(i + "-" + Convert.ToString(Variables.gPumps.get_Grade(Convert.ToByte(assignment.GradeID)).FullName));
                            break;
                        }
                    }
                }
            }
            return pumpGrades;
        }

        /// <summary>
        /// Method to display status
        /// </summary>
        /// <param name="statusStr">Status</param>
        /// <param name="hold">Hold or not</param>
        private void Status_Display(ref List<PumpControl> pumps, string statusStr, string status, bool hold)
        {
            byte i = 0;
            char iStatus;
            byte oldStatus = 0;
            for (i = 1; i <= Variables.iPumpCount; i++)
            {
                var pmp = pumps[i - 1];

                if (_payAtPumpRenamed[i])
                {
                    SetTopSign(ref pmp, 2); //paypump
                }
                else
                {
                    if (Variables.Pump[i].IsPrepay)
                    {
                        SetTopSign(ref pmp, 1, Variables.Pump[i].PrepayAmount);
                    }
                    else
                    {
                        if (statusStr.Substring(i - 1, 1) == "S" || statusStr.Substring(i - 1, 1) == "T")
                        {
                            SetTopSign(ref pmp, 4); //display "Preset"
                        }
                        else
                        {
                            SetTopSign(ref pmp, 3); // = False
                        }
                    }
                }

                if (status != "E")
                {
                    iStatus = Convert.ToChar(statusStr.Substring(i - 1, 1));
                }
                else
                {
                    iStatus = '0';
                }
                if (Variables.gBasket[i - 1].CurrentFilled)
                {
                    if (_curBaskSit[i] == 0)
                    {
                        _curBaskSit[i] = (float)DateAndTime.Timer;
                    }
                }
                else
                {
                    _curBaskSit[i] = 0;
                }
                if (Variables.gBasket[i].StackFilled && _stackBaskSit[i] == 0)
                {
                    _stackBaskSit[i] = (float)DateAndTime.Timer;
                }

                oldStatus = Variables.PumpStatus[i]; //save the old status to change if status is changed or not
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                switch (iStatus)
                {
                    case '0': // connection error
                        Variables.PumpStatus[i] = (byte)0;
                        pumps[i - 1].PumpButtonCaption = _resourceManager.CreateCaption(offSet, (short)61, Convert.ToInt16(38), null, (short)0);

                        pumps[i - 1].SetPumpStatus(i, "7");

                        break;
                    case '1': // Idle
                        _payAtPumpRenamed[i] = false;
                        Variables.PumpStatus[i] = (byte)1;
                        pumps[i - 1].PumpButtonCaption = _resourceManager.GetResString(offSet, (short)331);

                        if (_payAtPumpRenamed[i])
                        {
                            SetTopSign(ref pmp, 2); //paypump
                            pumps[i - 1].SetPumpStatus(i, "H");
                        }
                        else
                        {
                            if (Variables.Pump[i].IsHoldPrepay)
                            {
                                SetTopSign(ref pmp, 1);
                            }
                            else if (Variables.Pump[i].IsPrepay)
                            {
                                SetTopSign(ref pmp, 1, Variables.Pump[i].PrepayAmount);
                            }
                            else
                            {
                                SetTopSign(ref pmp, 3);
                            }
                            pumps[i - 1].SetPumpStatus(i, "1");
                        }

                        //nancy add Diable Big Pump display, when Reader status was TimeOut and no pump Finished status,we need to disable the Pump frame after Idle
                        if (Variables.DisplayPumpID == i)
                        {
                            Variables.DisplayPumpID = (byte)0;

                        }

                        break;
                    case '2': // calling
                        Variables.PumpStatus[i] = (byte)2;
                        pumps[i - 1].PumpButtonCaption = _resourceManager.GetResString(offSet, (short)324);
                        if (_payAtPumpRenamed[i])
                        {
                            SetTopSign(ref pmp, 2); //paypump
                            pumps[i - 1].SetPumpStatus(i, "C");
                        }
                        else
                        {
                            if (Variables.Pump[i].IsPrepay) //prepay
                            {
                                SetTopSign(ref pmp, 1, Variables.Pump[i].PrepayAmount);
                                pumps[i - 1].SetPumpStatus(i, "L");
                            }
                            else
                            {
                                SetTopSign(ref pmp, 3); // = False
                                pumps[i - 1].SetPumpStatus(i, "2");
                            }
                        }

                        break;

                    case '3': // authorized
                    case 'L':
                        Variables.PumpStatus[i] = (byte)3;
                        pumps[i - 1].PumpButtonCaption = _resourceManager.GetResString(offSet, (short)325);
                        if (_payAtPumpRenamed[i])
                        {
                            SetTopSign(ref pmp, 2); //paypump
                            pumps[i - 1].SetPumpStatus(i, "U");
                        }
                        else
                        {
                            if (Variables.Pump[i].IsPrepay) //prepay
                            {
                                SetTopSign(ref pmp, 1, Variables.Pump[i].PrepayAmount);
                                pumps[i - 1].SetPumpStatus(i, "O");
                            }
                            else
                            {
                                SetTopSign(ref pmp, 3); // = False
                                pumps[i - 1].SetPumpStatus(i, "3");
                            }
                        }

                        break;
                    case '4': // in use /pumping
                        Variables.PumpStatus[i] = (byte)4;
                        if (!_pumpingBroadcast)
                        {
                            //          Sleep 500 'wait for globalPrepay to be updated
                            //Variables.Sleep(50); //for testing,Nancy changed to 50, 03/10/03, it wasted long time
                        }

                        if (Variables.Pump[i].IsPrepay && Variables.Pump[i].IsPrepayLocked) //current pumping tran is prepay
                        {
                            pumps[i - 1].SetPumpStatus(i, "A");
                            SetTopSign(ref pmp, 1, Variables.Pump[i].PrepayAmount);
                        }
                        else
                        {
                            pumps[i - 1].SetPumpStatus(i, "4");
                            SetTopSign(ref pmp, 3);
                        }

                        break;
                    case '5': //stopped
                        Variables.PumpStatus[i] = (byte)5;
                        if (!hold)
                        {
                            pumps[i - 1].PumpButtonCaption = _resourceManager.GetResString(offSet, (short)326); //GetResString(323) & i & vbCrLf &
                                                                                                                //        ctlPump(i).PumpButtonColor = vbRed
                        }
                        else
                        {
                            pumps[i - 1].PumpButtonCaption = _resourceManager.GetResString(offSet, (short)327); //GetResString(323) & i & vbCrLf &
                                                                                                                //        ctlPump(i).PumpButtonColor = &H80C0FF
                        }
                        pumps[i - 1].SetPumpStatus(i, "5");

                        break;

                    case '6': //runaway
                        Variables.PumpStatus[i] = (byte)6;
                        pumps[i - 1].PumpButtonCaption = _resourceManager.GetResString(offSet, (short)328); //GetResString(323) & i & vbCrLf &
                        pumps[i - 1].SetPumpStatus(i, "6");

                        break;

                    case '7': //inactive communication
                        Variables.PumpStatus[i] = (byte)7;
                        pumps[i - 1].PumpButtonCaption = _resourceManager.GetResString(offSet, (short)329); // GetResString(323) & i & vbCrLf &
                                                                                                            //      ctlPump(i).PumpButtonColor = vbBlue
                        pumps[i - 1].SetPumpStatus(i, "7");

                        break;
                    case '8': // finished
                        Variables.PumpStatus[i] = (byte)8;
                        Variables.Pump[i].Amount = 0; //reset amount for pumping
                        pumps[i - 1].PumpButtonCaption = _resourceManager.GetResString(offSet, (short)330); //i & vbCrLf &

                        if (Variables.Pump[i].IsPrepay && Variables.Pump[i].IsPrepayLocked)
                        {
                            pumps[i - 1].SetPumpStatus(i, "R");
                            SetTopSign(ref pmp, 1, Variables.Pump[i].PrepayAmount);
                        }
                        else
                        {
                            pumps[i - 1].SetPumpStatus(i, "8");
                            SetTopSign(ref pmp, 3); //10/01/02, add for Reader breaking while paypump pumping, status should change to PayInside pumping
                        }
                        _payAtPumpRenamed[i] = false;

                        //nancy add Diable Big Pump display
                        if (Variables.DisplayPumpID == i)
                        {
                            Variables.DisplayPumpID = (byte)0;
                        }

                        break;
                    case 'H': //paypump on hold
                        Variables.PumpStatus[i] = (byte)9;
                        pumps[i - 1].PumpButtonCaption = _resourceManager.CreateCaption(offSet, (short)62, Convert.ToInt16(38), null, (short)0); //GetResString(323) & i & vbCrLf &
                        pumps[i - 1].SetPumpStatus(i, "1");
                        SetTopSign(ref pmp, 2);

                        break;

                    case 'C': //paypump calling
                        Variables.PumpStatus[i] = (byte)10;
                        _payAtPumpRenamed[i] = true;
                        pumps[i - 1].PumpButtonCaption = _resourceManager.GetResString(offSet, (short)324); //GetResString(323) & i & vbCrLf &
                        pumps[i - 1].SetPumpStatus(i, "H");
                        SetTopSign(ref pmp, 2); // True

                        break;
                    case 'P': //paypump pumping
                        Variables.PumpStatus[i] = (byte)11;
                        _payAtPumpRenamed[i] = true;
                        SetTopSign(ref pmp, 2); // True
                        pumps[i - 1].SetPumpStatus(i, "P");

                        break;

                    case 'F': // paypump finished
                        Variables.PumpStatus[i] = (byte)12;
                        Variables.Pump[i].Amount = 0; //reset amount for pumping
                        pumps[i - 1].PumpButtonCaption = _resourceManager.GetResString(offSet, (short)330); //i & vbCrLf &
                        pumps[i - 1].SetPumpStatus(i, "F");
                        _payAtPumpRenamed[i] = true;

                        //nancy add Disable Big Pump display
                        if (Variables.DisplayPumpID == i)
                        {
                            Variables.DisplayPumpID = (byte)0;
                            //fmePump.Enabled = false;
                            //fmePump.Visible = false;
                            if (_displayReader)
                            {
                                _displayReader = false;
                            }
                        }

                        break;
                    case 'S': //Preset Idle
                        Variables.PumpStatus[i] = (byte)13;
                        pumps[i - 1].PumpButtonCaption = _resourceManager.GetResString(offSet, (short)331); //idle
                        pumps[i - 1].SetPumpStatus(i, "S");
                        SetTopSign(ref pmp, 4); //preset

                        break;
                    case 'T': //Preset Calling
                        Variables.PumpStatus[i] = (byte)14;
                        pumps[i - 1].PumpButtonCaption = _resourceManager.GetResString(offSet, (short)324); //calling
                        pumps[i - 1].SetPumpStatus(i, "T");
                        SetTopSign(ref pmp, 4); //preset

                        break;
                    case '\0':
                        break;
                }
            }
            CacheManager.AddAllPumps(pumps);
        }

        /// <summary>
        /// Method to make basket
        /// </summary>
        /// <param name="strFinish">Finish string</param>
        /// <returns>True or false</returns>
        private bool Make_Basket(ref List<PumpControl> pumps, string strFinish)
        {
            bool returnValue = false;

            // static int sLastSoundTime; VBConversions Note: Static variable moved to class level and renamed Make_Basket_sLastSoundTime. Local static variables are not supported in C#.
            string fileName;
            char[] baskId = new char[3];
            short positionId = 0;
            short gradeId = 0;
            short mop = 0;
            byte pumpId = 0;
            string strBuffer;
            string strRemain;
            string response;



            //Variables.Pump = CacheManager.GetAllVariablePumps();
            pumpId = (byte)Conversion.Val(strFinish.Substring(4, 2));

            returnValue = false;
            baskId = Strings.Left(strFinish, 3).ToCharArray();

            if (Strings.Left(baskId.ToString(), 1) == "P")
            {
                return returnValue; //remove after test
            }

            strFinish = strFinish.Substring(4);
            mop = short.Parse(strFinish.Substring(2, 1));
            positionId = short.Parse(strFinish.Substring(3, 1));

            Variables.Pump[pumpId].Volume = (float)(Conversion.Val(strFinish.Substring(12, 8)) / 1000);
            Variables.Pump[pumpId].Amount = (float)(Conversion.Val(strFinish.Substring(4, 8)) / 1000);
            Variables.Pump[pumpId].Position = positionId;
            gradeId = Variables.gPumpPositionGrade[pumpId, positionId];

            //nancy changed,02/10/03,only if (the price from database) * Volume <> Amount, then we need to get the price from Amount/Volume
            if (mop == 2) //credit price in effect
            {
                Variables.Pump[pumpId].UnitPrice = Variables.Pump[pumpId].creditUP == null ? 0 : Convert.ToSingle(Variables.Pump[pumpId].creditUP[positionId]); //gPumps.FuelPrice(gradeID, gPumps.pump(pumpID).TierID, gPumps.pump(pumpID).LevelID).creditPrice
            }
            else if (mop == 1) //cash pricing
            {
                Variables.Pump[pumpId].UnitPrice = Variables.Pump[pumpId].cashUP == null ? 0 : Convert.ToSingle(Variables.Pump[pumpId].cashUP[positionId]); //gPumps.FuelPrice(gradeID, gPumps.pump(pumpID).TierID, gPumps.pump(pumpID).LevelID).cashPrice
            }
            if (modGlobalFunctions.Round(Variables.Pump[pumpId].UnitPrice * Variables.Pump[pumpId].Volume, 2) != modGlobalFunctions.Round(Variables.Pump[pumpId].Amount, 2))
            {
                Variables.Pump[pumpId].UnitPrice = (float)modGlobalFunctions.Round(Conversion.Val(strFinish.Substring(4, 8)) / Conversion.Val(strFinish.Substring(12, 8)), 3);
            }


            if (!Variables.gBasket[pumpId].CurrentFilled)
            {
                Variables.gBasket[pumpId].CurrentFilled = true;
                Variables.gBasket[pumpId].currMOP = mop;
                Variables.gBasket[pumpId].AmountCurrent = Variables.Pump[pumpId].Amount;
                Variables.gBasket[pumpId].VolumeCurrent = Variables.Pump[pumpId].Volume;
                Variables.gBasket[pumpId].UPCurrent = Variables.Pump[pumpId].UnitPrice;
                Variables.gBasket[pumpId].DescCurrent = Variables.gGradeDescription[gradeId];
                Variables.gBasket[pumpId].gradeIDCurr = gradeId;
                Variables.gBasket[pumpId].currBaskID = new string(baskId);
                Variables.gBasket[pumpId].PosIDCurr = Variables.Pump[pumpId].Position;
                //Nancy took out "$"
                //        ctlPump(pumpID).BasketButtonCaption = "$ " & Format(gBasket(pumpID).AmountCurrent, "###0.00") '  & vbCrLf & Format(gBasket(pumpID).VolumeCurrent, "####0.000")
                pumps[pumpId - 1].BasketButtonCaption = Variables.gBasket[pumpId].AmountCurrent.ToString("##0.00"); //  & vbCrLf & Format(gBasket(pumpID).VolumeCurrent, "####0.000")
            }
            else
            {
                //  ElseIf Not gBasket(pumpID).StackFilled Then  '10/03/02,Nancy changed to not displaying the 3nd basket

                Variables.gBasket[pumpId].StackFilled = true;
                Variables.gBasket[pumpId].stackMOP = mop;
                Variables.gBasket[pumpId].AmountStack = Variables.Pump[pumpId].Amount;
                Variables.gBasket[pumpId].VolumeStack = Variables.Pump[pumpId].Volume;
                Variables.gBasket[pumpId].UPStack = Variables.Pump[pumpId].UnitPrice;
                Variables.gBasket[pumpId].DescStack = Variables.gGradeDescription[gradeId];
                Variables.gBasket[pumpId].gradeIDStack = gradeId;
                Variables.gBasket[pumpId].stackBaskID = new string(baskId);
                Variables.gBasket[pumpId].posIDStack = Variables.Pump[pumpId].Position;
                //Nancy took out "$"
                //        ctlPump(pumpID).BasketLabelCaption = "$ " & Format(gBasket(pumpID).AmountStack, "###0.00") ' & vbCrLf & Format(gBasket(pumpID).VolumeStack, "####0.000")
                pumps[pumpId - 1].BasketLabelCaption = Variables.gBasket[pumpId].AmountStack.ToString("##0.00"); // & vbCrLf & Format(gBasket(pumpID).VolumeStack, "####0.000")
                pumps[pumpId - 1].EnableStackBasketBotton = true;
            }



            if (!_payAtPumpRenamed[pumpId] && !Variables.Pump[pumpId].IsHoldPrepay)// && (!preset[pumpID]))
            {
                var pmp = pumps[pumpId - 1];
                SetTopSign(ref pmp, 3); // False
            }

            CacheManager.AddAllPumps(pumps);
            return returnValue;
        }

        /// <summary>
        /// Method to show reader
        /// </summary>
        /// <param name="strReader">Reader</param>
        /// <param name="pumpId">Pump Id</param>
        /// <returns>Big pump</returns>
        private BigPump ShowReader(string strReader, short pumpId)
        {
            char stat;
            BigPump bigPump = new BigPump();
            if (strReader.Length > 0)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                bigPump.IsPumpVisible = true;
                bigPump.PumpId = pumpId.ToString();
                bigPump.PumpLabel = _resourceManager.GetResString(offSet, (short)333) + " " + Strings.Right("00" + Convert.ToString(pumpId), 2);
                stat = Convert.ToChar(strReader.Substring(pumpId - 1, 1));
                switch (stat)
                {
                    case ' ': // "0" means FC didn't get the real status for this Reader
                    case '0':
                    case 'B':
                        // "B" means "Reader is out of Service"
                        // "" means no such reader in Database


                        if (Variables.Pump[pumpId].IsPrepay)
                        {

                            bigPump.PumpMessage = _resourceManager.CreateCaption(offSet, (short)82, Convert.ToInt16(38), null, (short)0); // GetResString(Me.Tag * 100 + 82) 'Prepay - Pumping..
                        }
                        else
                        {
                            bigPump.PumpMessage = _resourceManager.CreateCaption(offSet, (short)81, Convert.ToInt16(38), null, (short)0); //GetResString(Me.Tag * 100 + 81) 'Pay inside - Pumping..
                        }
                        break;
                    case 'A':
                        bigPump.PumpMessage = _resourceManager.CreateCaption(offSet, (short)80, Convert.ToInt16(38), null, (short)0); //GetResString(Me.Tag * 100 + 80) 'Card inserted at wrong side.
                        break;
                    case 'F':
                        bigPump.PumpMessage = _resourceManager.CreateCaption(offSet, (short)83, Convert.ToInt16(38), null, (short)0); //GetResString(Me.Tag * 100 + 83) 'Bank processing
                        break;
                    case 'G':
                        bigPump.PumpMessage = _resourceManager.CreateCaption(offSet, (short)84, Convert.ToInt16(38), null, (short)0); //GetResString(Me.Tag * 100 + 84) 'Invalid card.
                        break;
                    case 'H':
                        if (Variables.PumpStatus[pumpId] == 11)
                        {
                            bigPump.PumpMessage = _resourceManager.CreateCaption(offSet, (short)90, Convert.ToInt16(38), null, (short)0); //GetResString(Me.Tag * 100 + 90) 'Pay@Pump - Pumping..
                        }
                        else
                        {
                            bigPump.PumpMessage = _resourceManager.CreateCaption(offSet, (short)85, Convert.ToInt16(38), null, (short)0); //GetResString(Me.Tag * 100 + 85) 'Card Approved.
                        }
                        break;
                    case 'I':
                        bigPump.PumpMessage = _resourceManager.CreateCaption(offSet, (short)86, Convert.ToInt16(38), null, (short)0); //GetResString(Me.Tag * 100 + 86) 'Card Not Approved.
                        break;
                    case 'J':
                        bigPump.PumpMessage = _resourceManager.CreateCaption(offSet, (short)87, Convert.ToInt16(38), null, (short)0); //GetResString(Me.Tag * 100 + 87) 'Bank Timed Out.
                        break;
                    case 'K':
                        bigPump.PumpMessage = _resourceManager.CreateCaption(offSet, (short)89, Convert.ToInt16(38), null, (short)0); //GetResString(Me.Tag * 100 + 89) 'Transaction Canceled.
                        break;
                    case 'L':
                        bigPump.PumpMessage = _resourceManager.CreateCaption(offSet, (short)88, Convert.ToInt16(38), null, (short)0); //GetResString(Me.Tag * 100 + 88) 'User Timed Out.
                        break;
                    case 'C':
                        bigPump.PumpMessage = _resourceManager.CreateCaption(offSet, (short)62, Convert.ToInt16(38), null, (short)0) + "..";
                        break;
                    case 'P': //for preset transaction, only has one status from beginning to the end
                        bigPump.PumpMessage = _resourceManager.CreateCaption(offSet, (short)63, Convert.ToInt16(38), null, (short)0);
                        break;
                }
            }
            else
            {
                bigPump.PumpMessage = "";
            }
            return bigPump;
        }

        /// <summary>
        /// Method to show fueling
        /// </summary>
        /// <param name="strSource">Source</param>
        /// <param name="strReader">Reader</param>
        /// <returns>List of big pump</returns>
        private List<BigPump> ShowFueling(ref List<PumpControl> pumps, string strSource, string strReader = "")
        {
            List<BigPump> bigPumps = new List<BigPump>();

            byte pumpId = 0;
            string gradeDesp;
            string[] strFuel = null;
            short p = 0;
            string strTmp = "";
            //var pumps = CacheManager.GetAllPumps();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            strFuel = new string[Variables.iPumpCount + 1];
            strTmp = strSource;
            if (strSource.Length == 0)
            {
                for (pumpId = 1; pumpId <= Variables.iPumpCount; pumpId++)
                {
                    strFuel[pumpId] = "";
                    if (Variables.PumpStatus[pumpId] == 4 || Variables.PumpStatus[pumpId] == 11)
                    {
                        //            ctlPump(pumpID).PumpButtonCaption = "Pumping"
                        pumps[pumpId - 1].PumpButtonCaption = _resourceManager.CreateCaption(offSet, (short)64, Convert.ToInt16(38), null, (short)0);
                    }
                }
            }
            else
            {
                for (pumpId = 1; pumpId <= Variables.iPumpCount; pumpId++)
                {
                    p = (short)(strTmp.IndexOf(",") + 1);
                    if (p >= 1)
                    {
                        ///if BroadcastValue=false and pressed big pump to disable broadcasting for this pump, then display "Pumping" rightaway
                        if (Variables.DisplayPumpID != pumpId && !Variables.DisplayFueling)
                        {
                            if (Variables.PumpStatus[pumpId] == 4 || Variables.PumpStatus[pumpId] == 11)
                            {
                                //                    ctlPump(pumpID).PumpButtonCaption = "Pumping"
                                pumps[pumpId - 1].PumpButtonCaption = _resourceManager.CreateCaption(offSet, (short)64, Convert.ToInt16(38), null, (short)0);
                            }
                            Variables.DisplayValue[pumpId] = false; //FC is not broadcasting sale value for this pump
                            strFuel[pumpId] = "";
                            strTmp = strTmp.Substring(p + 1 - 1);
                        }
                        else
                        {
                            if (strTmp.IndexOf("0,") + 1 == 1 || strTmp.IndexOf("E,") + 1 == 1)
                            {
                                if (Variables.PumpStatus[pumpId] == 4 || Variables.PumpStatus[pumpId] == 11)
                                {
                                    //                        ctlPump(pumpID).PumpButtonCaption = "Pumping"
                                    pumps[pumpId - 1].PumpButtonCaption = _resourceManager.CreateCaption(offSet, (short)64, Convert.ToInt16(38), null, (short)0);
                                }
                                Variables.DisplayValue[pumpId] = false; //FC is not broadcasting sale value for this pump
                                strFuel[pumpId] = "";
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(strTmp))
                                {
                                    Variables.DisplayValue[pumpId] = false;
                                }
                                else
                                {
                                    Variables.DisplayValue[pumpId] = true; //FC is broadcasting sale value for this pump
                                }
                                strFuel[pumpId] = Strings.Left(strTmp, p - 1);
                            }
                            strTmp = strTmp.Substring(p + 1 - 1);
                        }
                    }
                    else //if there's no "," at all
                    {
                        Variables.DisplayValue[pumpId] = false;
                        strTmp = "";
                        strFuel[pumpId] = "";
                    }
                }
            }
            for (pumpId = 1; pumpId <= Variables.iPumpCount; pumpId++)
            {
                if (Variables.PumpStatus[pumpId] == 4 || Variables.PumpStatus[pumpId] == 11 || _displayReader)
                {
                    BigPump bigPump = null;//        If DisplayReader Then
                                           //if this is the pump, display reader status in the text box
                    if (Variables.DisplayPumpID == pumpId)
                    {
                        bigPump = ShowReader(strReader, pumpId);
                    }
                    //        End If
                    if (strFuel[pumpId].Length == 23)
                    {
                        Variables.DisplayValue[pumpId] = true;
                        Variables.Pump[pumpId].Amount = (float)(Conversion.Val(strFuel[pumpId].Substring(10, 8)) / 1000);
                        pumps[pumpId - 1].PumpButtonCaption = Variables.Pump[pumpId].Amount.ToString("##0.00"); // GetResString(323) & pumpID
                        if (Variables.PumpStatus[pumpId] == 11)
                        {
                            var pmp = pumps[pumpId - 1];
                            SetTopSign(ref pmp, 2); // True
                        }
                        //nancy add display pumping value in big pump frame,08/21/2002
                        //if we need to display the big frame for this pump
                        if (Variables.DisplayPumpID == pumpId)
                        {
                            Variables.Pump[pumpId].Volume = (float)(Conversion.Val(strFuel[pumpId].Substring(2, 8)) / 1000);
                            Variables.Pump[pumpId].price = (float)(Conversion.Val(strFuel[pumpId].Substring(18, 5)) / 10);
                            Variables.Pump[pumpId].Position = (short)Conversion.Val(Strings.Left(strFuel[pumpId], 1));
                            if (bigPump == null)
                            {
                                bigPump = new BigPump();
                            }
                            bigPump.IsPumpVisible = true;
                            bigPump.PumpLabel = _resourceManager.GetResString(offSet, (short)333) + " " + Strings.Right("00" + Convert.ToString(pumpId), 2);
                            bigPump.Amount = Variables.Pump[pumpId].Amount.ToString("#0.00");
                        }
                    }
                    else if (Variables.PumpStatus[pumpId] == 4 || Variables.PumpStatus[pumpId] == 11)
                    {
                        //            ctlPump(pumpID).PumpButtonCaption = "Pumping"
                        pumps[pumpId - 1].PumpButtonCaption = _resourceManager.CreateCaption(offSet, (short)64, Convert.ToInt16(38), null, (short)0);
                    }
                    //        strSource = Mid(strSource, 25)


                    if (Variables.PumpStatus[pumpId] == 4 || Variables.PumpStatus[pumpId] == 11)
                    {
                        var pmp = pumps[pumpId - 1];
                        if (_payAtPumpRenamed[pumpId])
                        {
                            SetTopSign(ref pmp, 2); //paypump
                            pumps[pumpId - 1].SetPumpStatus(pumpId, "P");
                        }
                        else
                        {
                            if (Variables.Pump[pumpId].IsPrepay) //prepay
                            {
                                SetTopSign(ref pmp, 1, Variables.Pump[pumpId].PrepayAmount);
                                pumps[pumpId - 1].SetPumpStatus(pumpId, "A");
                            }
                            else
                            {
                                SetTopSign(ref pmp, 3); // = False
                                pumps[pumpId - 1].SetPumpStatus(pumpId, "4");
                            }
                        }
                    }
                    if (bigPump != null)
                    {
                        bigPumps.Add(bigPump);
                    }
                }
            }
            CacheManager.AddAllPumps(pumps);
            return bigPumps;
        }

        /// <summary>
        /// Method to clean up basket
        /// </summary>
        private void BasketCleanUp(ref List<PumpControl> pumps)
        {
            byte i = 0;
            //var pumps = CacheManager.GetAllPumps();
            for (i = 1; i <= Variables.iPumpCount; i++)
            {
                Variables.gBasket[i].CurrentFilled = false;
                Variables.gBasket[i].StackFilled = false;
                if (Strings.Len(pumps[i - 1].BasketLabelCaption) > 1)
                {
                    pumps[i - 1].BasketLabelCaption = "";
                    pumps[i - 1].EnableStackBasketBotton = false;
                    Variables.gBasket[i].stackBaskID = 0.ToString();
                }
                if (Strings.Len(pumps[i - 1].BasketButtonCaption) > 1)
                {
                    pumps[i - 1].BasketButtonCaption = "";
                    Variables.gBasket[i].currBaskID = 0.ToString();
                }
            }
            CacheManager.AddAllPumps(pumps);
        }

        /// <summary>
        /// Method to set command
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Command</returns>
        private string Command1_ClickEvent(out ErrorMessage message)
        {
            message = new ErrorMessage();
            string pageName = string.Empty;
            User currentUser = CacheManager.GetUser(UserCode);
            if (Variables.IsWaiting)
            {
                return string.Empty;
            }
            if (!_policyManager.FUELONLY)
            {
                //result.IsCommandButtonEnabled = false;
            }

            if (!_policyManager.U_CHGFPRICE && !_policyManager.FUELPR_HO) //   added And Not Policy.FUELPR_HO, if fuel prices are controlled from HeadOffice the policy U_CHGFPRICE should not matter, as the user cannot edit the prices in the fuel price form. This is by design.
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                message = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 38, 75, null, CriticalOkMessageType)
                };
                // Get a user who is authorized to perform changing price
                pageName = "UserChange";
            }

            if (_policyManager.U_CHGFPRICE || (_policyManager.FUELPR_HO && Chaps_Main.boolFP_HO)) //   added Or (Policy.FUELPR_HO And boolFP_HO), if fuel prices are controlled from HeadOffice the policy U_CHGFPRICE should not matter, as the user cannot edit the prices in the fuel price form. This is by design.
            {
                // user can perform changing price, check GroupPricing or not
                if (_policyManager.FUEL_GP)
                {
                    pageName = "PumpGroupPrice";
                }
                else
                {
                    pageName = "PumpPrice";
                }
            }
            else
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                message = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 38, 75, null, CriticalOkMessageType)
                };
            }

            return pageName;
        }

        /// <summary>
        /// Method to put error log
        /// </summary>
        /// <param name="strError">Error message</param>
        private void PutErrorLog(string strError)
        {
            short nH = 0;
            _putErrorLogFl++;
            if (_putErrorLogFl > 100)
            {
                _putErrorLogFl = (short)0;
                var folderPath = @"C:\APILog\";
                var filePath = folderPath + "ErrorLog_" + PosId + ".txt";

                if (FileSystem.Dir(filePath) != "")
                {
                    if (FileSystem.FileLen(filePath) > 500000)
                    {
                        Variables.CopyFile(Variables.ErrorFile, folderPath + "ErrorLog_" + PosId + DateAndTime.Day(DateAndTime.Today).ToString("00") + DateAndTime.Hour(DateAndTime.TimeOfDay).ToString("00") + ".txt", 0);
                        Variables.DeleteFile(Variables.ErrorFile);
                    }
                }
            }

            nH = (short)FileSystem.FreeFile();
            FileSystem.FileOpen(nH, Variables.ErrorFile, OpenMode.Append);
            FileSystem.PrintLine(nH, DateTime.Now.ToString("hh:mm tt") + " - " + strError);
            FileSystem.FileClose(nH);

        }



        /// <summary>
        /// Method to enable error button
        /// </summary>
        /// <param name="result">Pump status</param>
        private void EnableErrorButton(ref PumpStatus result)
        {
            short i = 0;

            for (i = 1; i <= 5; i++)
            {
                result.IsErrorEnabled = true;
            }
        }

        /// <summary>
        /// Method to test if prepay exists
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>True or false</returns>
        private bool ExistPrepay(int tillNumber)
        {
            var returnValue = !string.IsNullOrEmpty(Variables.MyPrepayBaskets);

            if (UDPAgent.Instance.IsConnected)
            {
                _fuelPumpService.IsPrepaySet(tillNumber);
            }
            return returnValue;
        }

        /// <summary>
        /// Method to enable fuel only buttons
        /// </summary>
        /// <param name="result">Pump status</param>
        /// <param name="boolValue">Value</param>
        private void Enable_FuelOnly_Buttons(ref PumpStatus result, bool boolValue)
        {
            result.IsPrepayEnabled = false;
            result.IsFinishEnabled = false;
            result.IsManualEnabled = false;
            result.IsCurrentEnabled = boolValue; //cmdSwitch
            result.IsFuelPriceEnabled = boolValue; //command1
            result.IsTierLevelEnabled = boolValue;
            result.IsPropaneEnabled = false;
            result.IsErrorEnabled = boolValue;
            result.IsStopButtonEnabled = boolValue;
            result.IsResumeButtonEnabled = boolValue;
        }

        /// <summary>
        /// Method to set up pump status
        /// </summary>
        /// <returns>List of pump control</returns>
        private List<PumpControl> Setup_Pump_System()
        {
            List<PumpControl> result = new List<PumpControl>();
            var pumps = CacheManager.GetAllPumps();

            short ii;
            short i = 0;
            short j = 0;
            float tm = 0;

            var prepayRenamed = new Prepay();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            Variables.gPumps = _fuelPumpService.InitializeGetProperty(PosId, _policyManager.TAX_EXEMPT, _policyManager.TE_ByRate, _policyManager.TE_Type, _policyManager.AuthPumpPOS);
            Variables.Pump.Initialize();
            _baskDimInterval = Variables.gPumps.BasketInterval;
            for (i = 1; i <= Variables.gPumps.GradesCount; i++)
            {
                if (Variables.gPumps.get_Grade((byte)i).ShortName != "")
                {
                    Variables.gGradeDescription[i] = Convert.ToString(Variables.gPumps.get_Grade((byte)i).ShortName);
                }
            }
            Variables.UnitMeasurement = Variables.gPumps.UnitMeasurement;
            Variables.DisplayFueling = Variables.gPumps.BrdCst_Value;
            Variables.iPumpCount = Convert.ToByte(Variables.gPumps.PumpsCount);
            Variables.DisplayValue = new bool[(int)Variables.gPumps.PumpsCount + 1];
            _pumpiSpace = (byte)(double.Parse(Variables.gPumps.SpacesCount) + Variables.iPumpCount);
            if (Conversion.Int(_pumpiSpace / 2) != _pumpiSpace / 2)
            {
                _pumpiSpace++;
            }
            i = 0;
            j = 0;
            if (_pumpiSpace <= 8)
            {
                _lineNum = 1;
            }
            else if (_pumpiSpace <= 16)
            {
                _lineNum = 2;
            }
            else if (_pumpiSpace <= 32)
            {
                _lineNum = 3;
            }
            for (i = 1; i <= Variables.iPumpCount; i++)
            {
                Variables.Pump[i].Initialize();
                Variables.Cashier_Auth[i] = Convert.ToBoolean(Variables.gPumps.get_Pump((byte)i).CashierAuthorize);
                if (!Variables.gPumps.get_Pump((byte)i).ManualPump)
                {
                    recPump with1 = Variables.Pump[i];
                    with1.Initialize();
                    with1.Button = Convert.ToInt16(Variables.gPumps.get_Pump((byte)i).PlaceOnScreen);
                    with1.Title = Convert.ToString(Variables.gPumps.get_Pump((byte)i).Title);
                    with1.Tier = Convert.ToInt16(Variables.gPumps.get_Pump((byte)i).TierID);
                    with1.Level = Convert.ToInt16(Variables.gPumps.get_Pump((byte)i).LevelID);

                    for (j = 1; j <= Variables.gPumps.get_PositionsCount((byte)i); j++)
                    {
                        if (Variables.gPumps.get_Assignment((byte)i, (byte)j).GradeID != 0 &&
                            Variables.gPumps.get_Assignment((byte)i, (byte)j).GradeID != null)
                        {
                            Variables.Pump[i].cashUP[j] = Variables.gPumps.get_FuelPrice(Convert.ToByte(Variables.gPumps.get_Assignment((byte)i, (byte)j).GradeID), Convert.ToByte(Variables.gPumps.get_Pump((byte)i).TierID), Convert.ToByte(Variables.gPumps.get_Pump((byte)i).LevelID)).CashPrice;
                            Variables.Pump[i].creditUP[j] = Variables.gPumps.get_FuelPrice(Convert.ToByte(Variables.gPumps.get_Assignment((byte)i, (byte)j).GradeID), Convert.ToByte(Variables.gPumps.get_Pump((byte)i).TierID), Convert.ToByte(Variables.gPumps.get_Pump((byte)i).LevelID)).CreditPrice;
                            Variables.Pump[i].Stock_Code[j] = Strings.Trim(Convert.ToString(Variables.gPumps.get_Grade(Convert.ToByte(Variables.gPumps.get_Assignment((byte)i, (byte)j).GradeID)).Stock_Code));
                            Variables.gPumpPositionGrade[i, j] = Convert.ToInt16(Variables.gPumps.get_Assignment((byte)i, (byte)j).GradeID);
                        }
                    }
                }

                Variables.Pump[i].CallingSound.NeedPlay = false;
                Variables.Pump[i].PayPumpCallingSound.NeedPlay = false;
                Variables.Pump[i].StopSound.NeedPlay = false;
                Variables.Pump[i].CallingSound.ListName = (short)((i - 1) * 4);
                Variables.Pump[i].PayPumpCallingSound.ListName = (short)((i - 1) * 4 + 3);
            }

            if (pumps == null || pumps.Count != Variables.iPumpCount || !TCPAgent.Instance.SocketConnected || !UDPAgent.Instance.IsConnected)
            {
                for (i = 1; i <= Variables.iPumpCount; i++)
                {
                    var pump = new PumpControl
                    {
                        PumpButtonCaption = _resourceManager.CreateCaption(offSet, (short)68, Convert.ToInt16(38), null, (short)0),
                        BasketButtonCaption = "",
                        BasketButtonVisible = 1,
                        BasketLabelCaption = "",
                        EnableBasketBotton = true,
                        EnableStackBasketBotton = false,
                        PayPumporPrepay = false,
                        PrepayText = "",
                        PumpId = i,
                        Status = "Inactive",
                        CanCashierAuthorize = Variables.Cashier_Auth[i]
                    };
                    if (Variables.Pump[i].IsPrepay)
                    {
                        SetTopSign(ref pump, 1, Variables.Pump[i].PrepayAmount);
                    }
                    result.Add(pump);
                }
            }
            else
            {
                result = pumps;
            }
            _pumpingBroadcast = Variables.gPumps.BrdCst_Value;
            Variables.LockUdp = false;

            try
            {
                UDPAgent.Instance.OpenPort();
            }
            catch (Exception ex)
            {
            }

            TCPAgent.Instance.OpenPort(Convert.ToString(Variables.gPumps.IP.FC_IP), Convert.ToInt16(Variables.gPumps.IP.FC_TCP_Port));
            //Variables.Sleep(200);
            Variables.NeedToShowManualButton = Convert.ToBoolean(_policyManager.AllowManual);
            CacheManager.AddAllPumps(result);

            return result;
        }


        private void SetTopSign(ref PumpControl pump, byte signCode, float PrepAmount = 0)
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (signCode == ((byte)1)) //prepay
            {
                pump.PayPumporPrepay = true;
                pump.PrepayText = _resourceManager.GetResString(offSet, (short)601);

                if (PrepAmount > 0)
                {
                    pump.PrepayText = "$" + PrepAmount.ToString("#0.00");
                }
            }
            else if (signCode == ((byte)2))
            {
                pump.PayPumporPrepay = true;
                pump.PrepayText = _resourceManager.GetResString(offSet, (short)403);
            }
            else if (signCode == ((byte)3))
            {
                pump.PayPumporPrepay = false;
                if (PrepAmount == 0)
                {
                    pump.PrepayText = "";
                }
            }
            else if (signCode == ((byte)4))
            {
                pump.PayPumporPrepay = true;
                pump.PrepayText = "Preset";
            }
            else if (signCode == ((byte)5))
            {
                pump.PayPumporPrepay = true;
                pump.PrepayText = "Help";
            }
            else
            {
                pump.PrepayText = "";
            }

        }


        #endregion
    }

    public struct GradeType
    {
        public short Grade;
        [VBFixedArray(2, 2)]
        public PriceType[,] Price;

        public GradeType(short grade, PriceType[,] price)
        {
            Grade = 0;
            Price = new PriceType[3, 3];
        }
    }

    public struct MyGradeType
    {
        public short GradeId;
        public string GradeDesp;
        [VBFixedArray(2, 2)]
        public PriceType[,] Price;


        public MyGradeType(short gradeId, string gradeDesp, PriceType[,] price)
        {
            GradeId = gradeId;
            GradeDesp = gradeDesp;
            Price = new PriceType[3, 3];
        }

        /// <summary>
        /// Intialize grade type
        /// </summary>
        public void Initialize()
        {
            Price = new PriceType[3, 3];
        }
    }
}

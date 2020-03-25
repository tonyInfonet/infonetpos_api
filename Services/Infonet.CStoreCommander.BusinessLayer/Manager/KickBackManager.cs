using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.ADOData;
using Microsoft.VisualBasic;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using System.Net;
using System.IO;
using System.Threading;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{


    public class KickBackManager : ManagerBase, IKickBackManager
    {
        //private readonly IKickBackManager _
        private readonly IPolicyManager _policyManager;
        private readonly IKickBackService _kickBackService;
        private readonly ISaleManager _saleManager;
        private readonly ISaleLineManager _saleLineManager;
        //   private readonly ICommManager _commManager;
        private readonly IXMLManager _xmlManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ITenderManager _tenderManager;
        private readonly ICustomerService _customerService;
        public static double ExchangeRate;
        SocketWrench socket = new SocketWrench();
        private Sale saleObj;
        private string KickBackBuffer;
        public KickBackManager(IPolicyManager policyManager, IKickBackService kickBackService, ISaleManager saleManager, ISaleLineManager saleLineManagaer, IXMLManager xmlManager, IApiResourceManager resourceManager, ITenderManager tenderManager, ICustomerService customerService)
        {

            _policyManager = policyManager;
            _kickBackService = kickBackService;
            _saleManager = saleManager;
            _saleLineManager = saleLineManagaer;
            //  _commManager = commManager;
            _xmlManager = xmlManager;
            _resourceManager = resourceManager;
            _tenderManager = tenderManager;
            _customerService = customerService;
        }


        public double CheckBalance(string pointCardNum, int saleNumber, int tillNumber, string userCode,
            out ErrorMessage errorMessage)
        {
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 1, userCode, out errorMessage);
            short pos = 0;
            short posqm = 0;
            bool boolIsPhoneNumber = false;
            string PointCardNumber = "";

            if (pointCardNum.Trim().Length > 0)
            {

                sale.STFDNumber = pointCardNum;

                // Chaps_Main.DisplayMsgForm(Policy_Renamed.LoyaltyMesg, (short)15, null, (byte)0, (byte)0, "", "", "", "");
                if (!string.IsNullOrEmpty(sale.STFDNumber))
                {
                    if (string.IsNullOrEmpty(sale.STFDNumber))
                    {
                        return 0;
                    }

                    boolIsPhoneNumber = true;
                    // May 26, 2009: Nicolette changed to fix the crash for track1 being enabled
                    // look for "?" after the ";" to consider only track2. If any of ";" or "?"
                    // are not found, system assumes that is a phone number
                    pos = (short)(sale.STFDNumber.IndexOf(";") + 1);
                    //    posqm = InStr(1, InputValue, "?") ' May 26, 2009: Nicolette see comment
                    if (pos > 0)
                    {
                        //posqm = (short)(pos.ToString().IndexOf(sale.STFDNumber) + 1);
                        posqm = (short)(sale.STFDNumber.ToString().IndexOf("?") + 1);
                    }
                    else
                    {
                        posqm = (short)0;
                        PointCardNumber = sale.STFDNumber;
                    }
                    if (posqm > 0 & pos > 0)
                    {
                        boolIsPhoneNumber = false;
                        pos = (short)(sale.STFDNumber.IndexOf(";") + 1);
                        //        If pos < 0 Then Exit Sub   ' May 26, 2009: Nicolette see comment
                        PointCardNumber = sale.STFDNumber.Substring(pos + 1 - 1, posqm - pos - 1);
                    }

                    // CacheManager.GetCurrentSaleForTill.mvrCustomer.Set_Customer_KickBack_Data(Variables.STFDNumber);
                    _kickBackService.Set_Customer_KickBack_Data(sale.STFDNumber, boolIsPhoneNumber,
                        PointCardNumber, ref sale);

                    if (string.IsNullOrEmpty(sale.Customer.CustomerCardNum))
                    {
                        _kickBackService.Set_Customer_KickBack_Data(sale.STFDNumber, !boolIsPhoneNumber,
                            PointCardNumber, ref sale);
                    }

                    sale.STFDNumber = ""; //reset STFDNumber


                }
                else
                {
                    pointCardNum = "";
                    errorMessage.MessageStyle = new MessageStyle
                    {
                        Message = Utilities.Constants.CheckKickbackBalance,

                    };
                    errorMessage.StatusCode = HttpStatusCode.NotFound;
                    return -1;
                }
            }


            if (sale.Customer.PointCardNum != "")
            {
                var kickBackResponse = ProcessKickBack((short)2, userCode, ref sale, out errorMessage);

                if (!kickBackResponse)
                {
                    if (!string.IsNullOrEmpty(errorMessage?.MessageStyle?.Message))
                    {
                        errorMessage.MessageStyle = new MessageStyle
                        {
                            Message = errorMessage.MessageStyle.Message,

                        };
                        errorMessage.StatusCode = HttpStatusCode.NotFound;

                    }
                    return -1;
                }
            }
            WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside checkbalance 132 cardno value" + sale.Customer.PointCardNum);
            CacheManager.AddCurrentSaleForTill(saleNumber, tillNumber, sale);
            return sale.Customer.Balance_Points;
        }


        public double CheckKickbackResponse(bool response, int tillNumber, int saleNumber, string userCode, out ErrorMessage errorMessage, ref Sale sale)
        {
            object[] CapValue = new object[3];
            SaleSummaryResponse result = null;
            List<Report> transactReports = null;
            if (_policyManager.Use_KickBack && sale.Customer.PointCard_Registered && sale.Customer.Balance_Points >= _policyManager.L_RedeemPnts && sale.Sale_Totals.Gross > 0 && sale.Customer.PointsCard_AllowRedemption)
            {
                if (sale.Customer.Points_ExchangeRate != 0)
                {
                    //You have this many points. Redeem Now

                    CapValue[1] = sale.Customer.Balance_Points; // points balance

                    CapValue[2] = (sale.Customer.Balance_Points * sale.Customer.Points_ExchangeRate).ToString("0.00");
                }
                else
                {

                    CapValue[1] = sale.Customer.Balance_Points;

                    CapValue[2] = "?";
                }

                if (response == true)
                {
                    sale.Customer.Points_Redeemed = double.Parse(sale.Customer.Balance_Points.ToString("0.00"));
                    var amount = (sale.Customer.Balance_Points * sale.Customer.Points_ExchangeRate).ToString("0.00");


                    // result.Tenders = _tenderManager.UpdateTenders(sale.Sale_Num, sale.TillNumber, "Sale", userCode, false, "KICKBACK", amount, out transactReports, out errorMessage);
                }
                else
                {
                    sale.Customer.Points_Redeemed = 0;
                }
            }
            CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);
            errorMessage = null;

            return sale.Customer.Points_Redeemed;
        }


        public double VerifyKickBack(string pointCardNumber, string phoneNumber,
            int tillNumber, int saleNumber, byte registerNumber, string userCode, out ErrorMessage errorMessage,
            out Sale sale, ref bool usePhoneNumber)
        {
            short pos = 0;
            short posqm = 0;
            bool boolIsPhoneNumber = false;
            string PointCardNumber = "";
            string Cardnumber;
            string Code;
            Tender tender = null;
            bool res = false;
            sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out errorMessage);//all required)
            if (sale == null)
            {
                return 0;
            }
            sale.Customer.PointCardNum = null;
            sale.Customer.Balance_Points = 0;
            if (_policyManager.Use_KickBack)
            {

                //sale= _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out errorMessage);
                if (string.IsNullOrEmpty(sale.Customer.PointCardNum))
                {
                    sale.STFDNumber = !(string.IsNullOrEmpty(pointCardNumber)) ? pointCardNumber : phoneNumber;

                    // Chaps_Main.DisplayMsgForm(Policy_Renamed.LoyaltyMesg, (short)15, null, (byte)0, (byte)0, "", "", "", "");
                    if (!string.IsNullOrEmpty(sale.STFDNumber))
                    {
                        if (string.IsNullOrEmpty(sale.STFDNumber))
                        {
                            errorMessage = null;
                            return 0;
                        }
                        boolIsPhoneNumber = true;

                        // May 26, 2009: Nicolette changed to fix the crash for track1 being enabled
                        // look for "?" after the ";" to consider only track2. If any of ";" or "?"
                        // are not found, system assumes that is a phone number
                        pos = (short)(sale.STFDNumber.IndexOf(";") + 1);
                        //    posqm = InStr(1, InputValue, "?") ' May 26, 2009: Nicolette see comment
                        if (pos > 0)
                        {
                            //posqm = (short)(pos.ToString().IndexOf(sale.STFDNumber) + 1);
                            posqm = (short)(sale.STFDNumber.ToString().IndexOf("?") + 1);
                        }
                        else
                        {
                            posqm = (short)0;
                        }
                        if (posqm > 0 & pos > 0)
                        {
                            boolIsPhoneNumber = false;
                            pos = (short)(sale.STFDNumber.IndexOf(";") + 1);
                            //        If pos < 0 Then Exit Sub   ' May 26, 2009: Nicolette see comment
                            PointCardNumber = sale.STFDNumber.Substring(pos + 1 - 1, posqm - pos - 1);
                        }

                        // CacheManager.GetCurrentSaleForTill.mvrCustomer.Set_Customer_KickBack_Data(Variables.STFDNumber);
                        _kickBackService.Set_Customer_KickBack_Data(sale.STFDNumber, boolIsPhoneNumber,
                            PointCardNumber, ref sale, true);

                        usePhoneNumber = boolIsPhoneNumber;

                        if (string.IsNullOrEmpty(sale.Customer.CustomerCardNum))
                        {
                            _kickBackService.Set_Customer_KickBack_Data(sale.STFDNumber, !boolIsPhoneNumber,
                               string.IsNullOrEmpty(PointCardNumber) ? sale.STFDNumber : PointCardNumber, ref sale);

                            // usePhoneNumber = !boolIsPhoneNumber;
                        }

                        var a = sale;
                        sale.STFDNumber = ""; //reset STFDNumber
                    }

                }

                //SaveMousePointer = Cursor.Current;
                //this.Enabled = false;
                var allowredemption = sale.Customer.PointsCard_AllowRedemption;

                // System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                if (sale.Customer.PointCardNum != "")
                {
                    res = ProcessKickBack((short)(0), userCode, ref sale, out errorMessage);
                    if (res == false)
                    {
                        return -2;
                    }
                    sale.Customer.PointCardNum = (PointCardNumber == "") ? sale.Customer.PointCardNum : PointCardNumber;
                    sale.Customer.PointsCard_AllowRedemption = (allowredemption == false) ? sale.Customer.PointsCard_AllowRedemption : allowredemption;

                }

                CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);
                var samp = CacheManager.GetCurrentSaleForTill(tillNumber, saleNumber);
                //why samp?
                // System.Windows.Forms.Cursor.Current = SaveMousePointer;
                //Shiny Took it out(Discussion with Nicolette)-Aug27, 2009 - for testing for Flipping to Salemain .Me.enabled = True
            }

            //throw new NotImplementedException();
            errorMessage = null;
            if (res == true)
            {
                return sale.Customer.Balance_Points;
            }
            return 0;
        }





        public void KBComm_CommData(string Data)
        {
            KickBackBuffer = Data;
        }
        // Nicolette, July 23, 2010

        private void KBComm_CommError(string ErrorNum, string ErrData)
        {
            //    DisplayMessage Me, 42, vbInformation + vbOKOnly  ' June 25, 2009 Nicolette commented because it displays the message too many times in the POS. We already have the message displayed from ProcessKickBack procedure.
            // write in log file is done by KickBackComm class
            ///    KBComm.WriteToLog ("Error in KickBackComm class. Error is " & ErrorNum & " " & ErrData)
        }





        public bool ProcessKickBack(short command_Renamed, string userCode, ref Sale sale, out ErrorMessage errorMessage)
        {
            //  var tender = new Tender();
            bool result = true;
            var kickback = new XML(_policyManager);
            var store = _policyManager.LoadStoreInfo();

            float timeIN = 0;
            short TimeOut = 0;
            bool GotResponse = false;
            short Answer;
            var KBComm = new Comm();
            object[] CapValue = new object[3];
            var tender = _tenderManager.GetAllTender(sale.Sale_Num, sale.TillNumber, "Sale", userCode, false,
                   "", out errorMessage);

            //   command 0 - GetPointStatus Request
            //   command 1 - FinalizeRewards Request - only from Exact Change button
            //   command 2 - Check Balance
            if (command_Renamed < 0 | command_Renamed > 2)
            {
                errorMessage.StatusCode = HttpStatusCode.BadGateway;
                errorMessage.MessageStyle.Message = "Invalid loyalty card";
                return false;
            }

            KBComm = new Comm(); //'' KickBackComm
            KBComm.CommData += KBComm_CommData;
            KBComm.CommError += KBComm_CommError;



            Comm.KBComm = KBComm;

            var xml = new XML(_policyManager);
            var saleLine = new Sale_Line();
            var s = new Sale();


            //  var sale = _saleManager.GetCurrentSale();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            // Write to log is done by KickBackXML class, POS only displays messages for the cashier

            if (!_xmlManager.Initialize("", "", "", (sale.Sale_Num).ToString(), System.Convert.ToString(store.Code), "", ref xml))
            {

                // Display the same error message on the screen because for cashier doesn't matter
                // if the connection cannot be made or the XML cannot be created.
                // But KickBackXML class writes the error in the log for debugging purposes

                _resourceManager.CreateMessage(offSet, 11, 42, null, MessageType.Exclamation);//Cannot communicate with Loyalty Server
                errorMessage.MessageStyle.Message = "Invalid loyalty card";
                errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                return false;
            }
            else
            {

                kickback.Sale = this.saleObj;

                if (!(tender == null))
                {
                    kickback.Tenders = tender;
                }
            }

            if (command_Renamed == 0 | command_Renamed == 2)
            {
                var a = xml.PosSequenceID;

                // if (!_xmlManager.FormatFinalizeRewardsRequestXML(sale.Customer.PointCardNum, System.Convert.ToString(sale.Customer.PointCardSwipe == "2" ? "SWIPE" : "MANUAL"), (sale.Customer.Points_Redeemed).ToString(), (sale.Sale_Totals.Gross).ToString(), "", "", "", System.Convert.ToString(store.Code), System.Convert.ToString(userCode), System.Convert.ToString(sale.TillNumber), "", "", "", "",ref xml,ref sale))
                if (!_xmlManager.FormatGetPointStatusRequestXML(sale.Customer.PointCardNum, System.Convert.ToString(sale.Customer.PointCardSwipe == "2" ? "SWIPE" : "MANUAL"), ref xml))
                {
                    // Display the same error massage on the screen because for cashier doesn't matter
                    // if the connection cannot be made or the XML cannot be created.
                    // But write the correct reason in the log for debugging purposes

                    _resourceManager.CreateMessage(offSet, 11, 42, null, MessageType.Information); //Cannot communicate to Loyalty TPS

                    return false;
                }
                var sam1 = xml;
            }


            else if (command_Renamed == 1) // send finalize rewards only from Exact_Change Button
            {
                var points = (sale.Customer.Points_Redeemed == 0) ? 0 : (sale.Customer.Points_Redeemed / ExchangeRate);
                if (!_xmlManager.FormatFinalizeRewardsRequestXML(sale.Customer.PointCardNum, System.Convert.ToString(sale.Customer.PointCardSwipe == "2" ? "SWIPE" : "MANUAL"), points.ToString(), (sale.Sale_Totals.Gross).ToString(), "", "", "", System.Convert.ToString(store.Code), System.Convert.ToString(userCode), System.Convert.ToString(sale.TillNumber), "", "", "", "", ref xml, ref sale))
                // if (!_xmlManager.FormatFinalizeRewardsRequestXML(sale.Customer.PointCardNum, "SWIPE", (sale.Customer.Points_Redeemed * 100).ToString(), (sale.Sale_Totals.Gross).ToString(), "", "", "", System.Convert.ToString(store.Code), System.Convert.ToString(userCode), System.Convert.ToString(sale.TillNumber), "", "", "", "", ref xml, ref sale))
                {
                    //shiny end
                    // Display the same error massage on the screen because for cashier doesn't matter
                    // if the connection cannot be made or the XML cannot be created.
                    // But write the correct reason in the log for debugging purposes
                    //Chaps_Main.DisplayMessage(this, (short)42, MsgBoxStyle.Information, null, (byte)0); //Cannot communicate to Loyalty TPS
                    //shiny sept 14, 2009 - changed to use the common function, so apperstophe fix can do in one place
                    //             dbMaster.Execute "INSERT INTO KickBackQueue VALUES ('" & KickBack.GetRequestXMLstring & "')"
                    // modGlobalFunctions.InsertTo_KickBackQueue(Variables.KickBack.GetRequestXMLstring);
                    InsertTo_KickBackQueue(xml.GetRequestXMLstring);
                    //shiny end - sept14, 2009
                    return false;
                }
            }


            KickBackBuffer = ""; //clear the response buffer

            Thread.Sleep(500);
            var timeout = _policyManager.KICKBACK_TMT;

            KickBackBuffer = socket.Connect(xml.GetRequestXMLstring, _policyManager.KICKBACK_IP, (short)Conversion.Val(_policyManager.KICKBACK_PRT), (short)Conversion.Val(timeout), out errorMessage);




            if (KickBackBuffer == "failed")
            {
                WriteTokickBackLogFile(" Cannot Send to Server: " + xml.GetRequestXMLstring);
                if (command_Renamed == 1)
                {
                    InsertTo_KickBackQueue(xml.GetRequestXMLstring);
                }
                return false;

            }

            WriteTokickBackLogFile(" Connected to KickBack Server: " + $"{_policyManager.KICKBACK_IP}:{_policyManager.KICKBACK_PRT}");
            string kickBackTrimmedResponse = KickBackBuffer.Replace("\0", string.Empty);

            WriteTokickBackLogFile(" Received from Kickback Server:: " + kickBackTrimmedResponse.TrimEnd());
            var sam3 = xml;
            string RespStr = "";
            Byte[] buff = new Byte[1000];

            GotResponse = false;

            TimeOut = System.Convert.ToInt16(_policyManager.KICKBACK_TMT);
            timeIN = (float)DateAndTime.Timer;
            //    Debug.Print TimeOut & " " & timeIN
            while (!(DateAndTime.Timer - timeIN > TimeOut))
            {
                //        Debug.Print Timer
                System.Windows.Forms.Application.DoEvents();
                if (!string.IsNullOrEmpty(KickBackBuffer))
                {
                    GotResponse = true;
                    break;
                }
                if (DateAndTime.Timer < timeIN)
                {
                    timeIN = (float)DateAndTime.Timer;
                }
            }


            if (!GotResponse)
            {
                WriteTokickBackLogFile("No response from Loyalty Server.");
                if (command_Renamed == 1)
                {
                    InsertTo_KickBackQueue(xml.GetRequestXMLstring);
                }
                MessageType temp_VbStyle2 = (int)MessageType.Critical + MessageType.OkOnly;
                _resourceManager.CreateMessage(offSet, 11, 42, null, temp_VbStyle2);

                return false;
            }
            _xmlManager.AnalyseKickBackResponse(KickBackBuffer, ref xml);

            if (command_Renamed == 0 | command_Renamed == 2)
            {
                //Analyze the response
                // Modified on Apr 03, 2009 to use two flags for invalid and non registered cards
                // If the card is not valid processing should continue without any KickBack redemtion or acumulation
                // If the card is not registered processing should continue with accumulation but without redemption
                if (!xml.GetLoyaltyIDValid)
                {
                    //Invalid Loyalty Card
                    _resourceManager.CreateMessage(offSet, 11, 45, null, MessageType.OkOnly);
                    // was 1145
                    sale.Customer.PointCardNum = "";
                    sale.Customer.PointCardPhone = "";
                    sale.Customer.PointCardSwipe = "";
                    sale.Customer.Points_ExchangeRate = 0;
                    sale.Customer.Points_Redeemed = 0;
                    sale.Customer.PointsAwarded = 0;
                    sale.Customer.PointCard_Registered = false;

                    errorMessage = new ErrorMessage();
                    errorMessage.MessageStyle = new MessageStyle
                    {
                        Message = "Invalid loyalty card"

                    };
                    errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                    result = false;
                }
                else if (command_Renamed == 0)
                {
                    if (xml.LoyaltyIDRegistered)
                    {
                        var allowredeem = sale.Customer.PointsCard_AllowRedemption;
                        var balance = xml.Sale.Customer.Balance_Points;
                        //var saleline = sale.Sale_Lines;
                        //var saletotal = sale.Sale_Totals;
                        //var gross = sale.Sale_Totals.Gross;
                        //var till = sale.TillNumber;
                        //var cardno = sale.Customer.PointCardNum;
                        //var salenum = sale.Sale_Num;
                        //var entrymethod = sale.Customer.PointCardSwipe;
                        //var selectedCustomer = sale.Customer;
                        // sale = xml.Sale;
                        sale.Customer.PointCard_Registered = true;
                        // sale.Sale_Totals = saletotal;
                        // sale.Sale_Num = salenum;
                        // sale.TillNumber = till;
                        // sale.Sale_Lines = saleline;
                        //sale.Customer = selectedCustomer;
                        sale.Customer.PointsCard_AllowRedemption = allowredeem;
                        //sale.Customer.PointCardNum = cardno;
                        //sale.Customer.PointCardSwipe = entrymethod;
                        sale.Customer.Balance_Points = xml.Sale.Customer.Balance_Points;
                        result = true;
                    }
                    else
                    {
                        _resourceManager.CreateMessage(offSet, 11, 46, null, MessageType.OkOnly);

                        sale.Customer.PointCard_Registered = false;
                        sale.Customer.Points_Redeemed = 0;
                    }
                }
                sale.Customer.Balance_Points = xml.Sale.Customer.Balance_Points;
                ExchangeRate = sale.Customer.Points_ExchangeRate = xml.Sale.Customer.Points_ExchangeRate;
                WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside processkickback 483 cardno value" + sale.Customer.PointCardNum);
            }
            var a10 = xml;
            CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
            // _commManager.EndClass();

            //if (((string.IsNullOrEmpty(sale.Customer.CustomerCardNum) && string.IsNullOrEmpty(sale.Customer.PointCardNum))
            //    || (string.IsNullOrEmpty(sale.Customer.CustomerCardNum) && string.IsNullOrEmpty(sale.Customer.PointCardPhone)))
            //    && (string.IsNullOrEmpty(sale.Customer.PointCardNum) && string.IsNullOrEmpty(sale.Customer.LoyaltyCard)))
            //{
            //    errorMessage = new ErrorMessage();
            //    errorMessage.MessageStyle = new MessageStyle
            //    {
            //        Message = "Invalid loyalty card"

            //    };
            //    errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
            //    result = false;
            //}

            //if (sale.Customer.Balance_Points == 0)
            //{
            //    errorMessage = new ErrorMessage();
            //    errorMessage.MessageStyle = new MessageStyle
            //    {
            //        Message = "Invalid loyalty card"

            //    };
            //    errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
            //    result = false;
            //}
            return result;
        }

        public GaskingKickback ValidateGasKing(int tillNumber, int saleNumber, byte registerNumber,
            string userCode, out ErrorMessage errorMessage, bool isCardSwipedInTenderScreen = false)
        {
            errorMessage = new ErrorMessage();
            saleObj = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out errorMessage);
            var customer = saleObj.Customer.LoyaltyCardSwiped ?
                     _customerService.GetClientCardForGasKingCustomer(saleObj.Customer.LoyaltyCard) : null;

            if (customer == null)
            {
                // errorMessage.MessageStyle.Message = "No customer is selected";
                //errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                return new GaskingKickback
                {
                    IsKickBackLinked = false,
                    PointsReedemed = saleObj.Customer.Balance_Points,
                    Value = (saleObj.Customer.Balance_Points * saleObj.Customer.Points_ExchangeRate).ToString("0.00")
                };
            }
            saleObj.Customer.Loyalty_Code = customer.CardNumber;
            saleObj.Customer.PointsCard_AllowRedemption = customer.AllowRedemption;
            var arcust = saleObj.Customer.AR_Customer;
            bool isKickBackLinked = false;

            if (_policyManager.Use_KickBack)
            {
                // if (!customer.AllowRedemption && isCardSwipedInTenderScreen)
                //if (isCardSwipedInTenderScreen)
                //{
                //    return new GaskingKickback
                //    {
                //        IsKickBackLinked = false,
                //        PointsReedemed = saleObj.Customer.Balance_Points,
                //        Value = (saleObj.Customer.Balance_Points * saleObj.Customer.Points_ExchangeRate).ToString("0.00")
                //    };
                //}

                // Shiny -This Pointcard loading should have done as part of Customer loading - Since Previous code is not done properly at Customer class I can't change it- I am forced to do this here
                var data = _kickBackService.GaskingKickback(saleObj);
                if (data.Rows.Count > 0)
                {
                    var row = data.Rows[0];
                    saleObj.Customer.PointCardNum = CommonUtility.GetStringValue(row["PointCardNum"]);
                    saleObj.Customer.PointCardPhone = String.IsNullOrEmpty(CommonUtility.GetStringValue(row["phonenum"])) ? ""
                                    : CommonUtility.GetStringValue(row["phonenum"]);
                    saleObj.Customer.PointCardSwipe = "0"; // 0-from database based on GK card swiped, 1-from phone number, 2-swiped
                                                           //  '                            SA.Customer.PointsCard_AllowRedemption = True- we need to check the Gasking card setting
                    saleObj.Customer.PointsCard_AllowRedemption = _customerService.
                                            Check_Allowredemption(saleObj.Customer.LoyaltyCard);

                    //if (isCardSwipedInTenderScreen)
                    // {

                    //if (!string.IsNullOrEmpty(saleObj.Customer.PointCardNum) && !customer.AllowRedemption)
                    //{
                    //  //  ProcessKickBack((short)(1), userCode, ref saleObj, out errorMessage);

                    //    if (!isCardSwipedInTenderScreen)
                    //    {
                    //        return new GaskingKickback
                    //        {
                    //            IsKickBackLinked = true,
                    //            PointsReedemed = _policyManager.L_RedeemPnts - 1,
                    //            Value = (saleObj.Customer.Balance_Points * saleObj.Customer.Points_ExchangeRate).ToString("0.00")
                    //        };
                    //    }
                    //}
                    //else 
                    if (!string.IsNullOrEmpty(saleObj.Customer.PointCardNum))
                        //&& ((saleObj.Customer.PointsCard_AllowRedemption && isCardSwipedInTenderScreen) ||
                        //(!isCardSwipedInTenderScreen)))
                    {
                        isKickBackLinked = ProcessKickBack((short)(0), userCode, ref saleObj, out errorMessage);

                        if (!isCardSwipedInTenderScreen && !customer.AllowRedemption)
                        {
                            return new GaskingKickback
                            {
                                IsKickBackLinked = true,
                                PointsReedemed = _policyManager.L_RedeemPnts - 1,
                                Value = (saleObj.Customer.Balance_Points * saleObj.Customer.Points_ExchangeRate).ToString("0.00")
                            };
                        }

                        if(isCardSwipedInTenderScreen)
                        {
                            return new GaskingKickback
                            {
                                IsKickBackLinked = isKickBackLinked && customer.AllowRedemption,
                                PointsReedemed = saleObj.Customer.Balance_Points,
                                Value = (saleObj.Customer.Balance_Points * saleObj.Customer.Points_ExchangeRate).ToString("0.00")
                            };
                        }
                    }
                    else
                    {
                        return new GaskingKickback
                        {
                            IsKickBackLinked = false,
                            PointsReedemed = saleObj.Customer.Balance_Points,
                            Value = (saleObj.Customer.Balance_Points * saleObj.Customer.Points_ExchangeRate).ToString("0.00")
                        };
                    }
                    // }
                    //else
                    //{
                    //    if (!string.IsNullOrEmpty(saleObj.Customer.PointCardNum))
                    //    {
                    //        isKickBackLinked = ProcessKickBack((short)(0), userCode, ref saleObj, out errorMessage);
                    //    }
                    //}
                }

                data = null;
            }

            else
            {
                return new GaskingKickback
                {
                    IsKickBackLinked = true,
                    PointsReedemed = _policyManager.L_RedeemPnts - 1,
                    Value = (saleObj.Customer.Balance_Points * saleObj.Customer.Points_ExchangeRate).ToString("0.00")
                };
            }
            saleObj.Customer.AR_Customer = arcust;
            CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, saleObj);

            var resp = new GaskingKickback
            {
                IsKickBackLinked = isKickBackLinked,
                PointsReedemed = saleObj.Customer.Balance_Points,
                Value = (saleObj.Customer.Balance_Points * saleObj.Customer.Points_ExchangeRate).ToString("0.00")
            };
            return resp;
        }

        public void InsertTo_KickBackQueue(string SourceString)
        {
            SourceString = SourceString.Replace("\'", "\'\'"); //shiny added on sept14, 2009 - to solve apperstiophe issue in request string. It was crashing
            object null_object = null;
            _kickBackService.InsertKickbackQueue(SourceString);

        }

        public void WriteUDPData(string msgStr)
        {
            try
            {
                var logPath = @"C:\APILog\";
                var fileName = logPath + "PosLog_" + DateTime.Today.ToString("MM/dd/yyyy") + ".txt";

                using (StreamWriter fileWriter = new StreamWriter(fileName, true))
                {
                    fileWriter.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + Microsoft.VisualBasic.Strings.Space(3) + msgStr);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}

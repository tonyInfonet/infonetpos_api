using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class MaintenanceManager : ManagerBase, IMaintenanceManager
    {
        private readonly ICardManager _cardManager;
        private readonly IPolicyManager _policyManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ISaleManager _saleManager;
        private readonly IMaintenanceService _maintenanceService;
        private readonly ICreditCardManager _creditCardManager;
        private readonly ITillCloseManager _tillCloseManager;
        private readonly IUtilityService _utilityService;
        private readonly ILoginManager _loginManager;
        private readonly IFuelService _fuelService;
        private Socket _client;


        public MaintenanceManager(ICardManager cardManager,
            IPolicyManager policyManager,
            IApiResourceManager resourceManager,
            ISaleManager saleManager,
            IMaintenanceService maintenanceService,
            ICreditCardManager creditCardManager,
            ITillCloseManager tillCloseManager,
            IUtilityService utilityService,
            ILoginManager loginManager,
            IFuelService fuelService)
        {
            _cardManager = cardManager;
            _policyManager = policyManager;
            _resourceManager = resourceManager;
            _saleManager = saleManager;
            _maintenanceService = maintenanceService;
            _creditCardManager = creditCardManager;
            _tillCloseManager = tillCloseManager;
            _utilityService = utilityService;
            _loginManager = loginManager;
            _fuelService = fuelService;
           
        }

        /// <summary>
        /// Method to close batch
        /// </summary>
        /// <param name="posId">Pos Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="error">Error message</param>
        /// <returns>List of report</returns>
        public List<Report> CloseBatch(byte posId, int tillNumber, int saleNumber, byte registerNumber, out ErrorMessage error)
        {
            var reports = new List<Report>();
            short termNos = 0;
            string[,] Terminals = new string[3, 3];
            short i = 0;
            string rFileName = "";
            DateTime batchDate = default(DateTime);
            bool blnNoDebitTrans = false;
            var emvProcess = _policyManager.EMVVersion;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, UserCode, out error);

            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return null;
            }
            var rFileNumber = (short)(FileSystem.FreeFile());
            try
            {
                if (GetConnection())
                {
                    var terminals = _maintenanceService.GetTerminalIds(posId);
                    if (terminals == null)
                    {
                        
                        WriteToLogFile("***PROBLEM WITH CLOSE BATCH*** " + " " + _resourceManager.GetResString(offSet, 1287)); // Shiny Mar6, 2008 -EKO
                                                                                                                               //MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                                                                                                                               //Chaps_Main.DisplayMessage(12, (short)87, temp_VbStyle, null, (byte)0);
                        error = new ErrorMessage
                        {
                            MessageStyle = _resourceManager.CreateMessage(offSet, 12, 87, null, CriticalOkMessageType)
                        };
                        return null;
                    }
                    //ButtonsEnabled(false);
                    rFileName = Path.GetTempPath() + "BankEod_ " + PosId + ".txt";
                    FileSystem.FileOpen(rFileNumber, rFileName, OpenMode.Append);
                    termNos = 0;
                    foreach (Terminal terminal in terminals)
                    {
                        if (string.IsNullOrEmpty(terminal.TerminalType) || string.IsNullOrEmpty(terminal.TerminalId))
                        {
                            
                            WriteToLogFile("***PROBLEM WITH CLOSE BATCH***" + " " + _resourceManager.GetResString(offSet, 1287)); // Shiny Mar6, 2008 -EKO
                                                                                                                                  //MsgBoxStyle temp_VbStyle2 = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                                                                                                                                  //Chaps_Main.DisplayMessage(this, (short)87, temp_VbStyle2, null, (byte)0);
                            error = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 12, 87, null, CriticalOkMessageType)
                            };
                            FileSystem.FileClose(rFileNumber);
                            return null;
                        }

                        modTPS.cc = new Credit_Card();

                        WriteToLogFile("EMVprocess:" + Convert.ToString(emvProcess));
                        SendToTps(_cardManager.GetRequestString(ref modTPS.cc, sale, "EODTerminal", terminal.TerminalType, 0, terminal.TerminalId), ref modTPS.cc, emvProcess);
                        var eodTimer = DateAndTime.Timer;
                        while (Convert.ToInt32(DateAndTime.Timer - Convert.ToDouble(eodTimer)) < 120) // shiny changed this waiting to 2 minutes- 4 is too much 240)
                        {
                            if (modTPS.cc.Response.Length > 0)
                            {
                                break;
                            }
                        }

                        if (modTPS.cc.Response.ToUpper() == "APPROVED")
                        {
                            termNos++;
                            Terminals[termNos, 1] = modTPS.cc.TerminalID;
                            Terminals[termNos, 2] = modTPS.cc.Sequence_Number;
                            FileSystem.PrintLine(rFileNumber, modTPS.cc.Report);
                            batchDate = modTPS.cc.Trans_Date;
                            WriteToLogFile("SUCCESS WITH CLOSE BATCH"); // Shiny Mar6, 2008 -EKO

                            //  - added as part of Datawire Integeration (added by Mina)
                            //#:5:TPS does handshake after sending EOD for debit. If there weren’t any debit transactions between closes batches, POS has to send special indication that there weren’t any debit transactions. Having gotten that flag, TPS doesn’t do handshake.
                            if (Strings.UCase(Convert.ToString(_policyManager.BankSystem)) == "GLOBAL")
                            {
                                blnNoDebitTrans = modTPS.cc.Report.IndexOf("No Transactions", StringComparison.Ordinal) + 1 > 0;
                            }
                            _maintenanceService.SetCloseBatchNumber(modTPS.cc);
                        }
                        else // Behrooz Jan-09
                        {
                            if (modTPS.cc.Receipt_Display.Length > 0)
                            {
                                WriteToLogFile("***PROBLEM WITH CLOSE BATCH***" + " " + modTPS.cc.Receipt_Display); // Shiny Mar6, 2008 -EKO
                                                                                                                    //Chaps_Main.DisplayMsgForm(modTPS.cc.Receipt_Display, (short)100, null, (byte)0, (byte)0, "", "", "", "");
                                error = new ErrorMessage
                                {
                                    MessageStyle = new MessageStyle
                                    {
                                        Message = modTPS.cc.Receipt_Display,
                                        MessageType = MessageType.OkOnly
                                    }
                                };
                                FileSystem.FileClose(rFileNumber);
                                return null;
                            }
                            else
                            {
                                WriteToLogFile("***PROBLEM WITH CLOSE BATCH***" + " " + _resourceManager.GetResString(offSet, 1284)); // Shiny Mar6, 2008 -EKO
                                                                                                                                      //Chaps_Main.DisplayMsgForm((Convert.ToDouble(12)) * 100 + 84, (short)99, null, (byte)0, (byte)0, "", "", "", "");
                                error = new ErrorMessage
                                {
                                    MessageStyle = new MessageStyle
                                    {
                                        Message = _resourceManager.GetResString(offSet, 1284),
                                        MessageType = MessageType.OkOnly
                                    }
                                };
                                FileSystem.FileClose(rFileNumber);
                                return null;
                            }
                        }
                        modTPS.cc = null;
                        i++;
                    }
                }
                else
                {
                    WriteToLogFile("***PROBLEM WITH CLOSE BATCH***" + " " + _resourceManager.GetResString(offSet, 1296)); // Shiny Mar6, 2008 -EKO
                                                                                                                          //Chaps_Main.DisplayMsgForm((Convert.ToDouble(12)) * 100 + 96, (short)99, null, (byte)0, (byte)0, "", "", "", "");
                    error = new ErrorMessage
                    {
                        MessageStyle = new MessageStyle
                        {
                            Message = _resourceManager.GetResString(offSet, 1296),
                            MessageType = MessageType.OkOnly
                        }
                    };
                    //return;
                }

                FileSystem.FileClose(rFileNumber);
                if (!string.IsNullOrEmpty(rFileName))
                {
                    var stream = File.OpenRead(rFileName);
                    FileSystem.FileClose(rFileNumber);
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, Convert.ToInt32(stream.Length));
                    stream.Close();
                    var content = Convert.ToBase64String(bytes);
                    reports.Add(new Report
                    {
                        ReportName = "BankEod.txt",
                        ReportContent = content,
                        Copies = 1
                    });
                }
                if (termNos == i)
                {
                    if (GetConnection()) //Nancy
                    {
                        //  - Datawire Integration (Added by Mina)
                        //#5: TPS does handshake after sending EOD for debit. If there weren’t any debit transactions between closes batches, POS has to send special indication that there weren’t any debit transactions. Having gotten that flag, TPS doesn’t do handshake.
                        if (Strings.UCase(Convert.ToString(_policyManager.BankSystem)) == "GLOBAL")
                        {
                            if (blnNoDebitTrans) //Added by Mina
                            {
                                SendToTps(_cardManager.GetRequestString(ref modTPS.cc, sale, "CloseBatchInside", "NoDebit", 0, ""), ref modTPS.cc, emvProcess);
                            }
                            else
                            {
                                SendToTps(_cardManager.GetRequestString(ref modTPS.cc, sale, "CloseBatchInside", "Credit", 0, ""), ref modTPS.cc, emvProcess);
                            }
                        }
                        else
                        {
                            SendToTps(_cardManager.GetRequestString(ref modTPS.cc, sale, "CloseBatchInside", "Credit", 0, ""), ref modTPS.cc, emvProcess);
                        }
                    }
                    else
                    {
                        return null; //Call DisplayMsgForm(Me.Tag * 100 + 96, 99)
                    }
                }
                if (termNos > 0)
                {
                    //modPrint.Dump_To_Printer(RFileName, (short)1, true, true, false);
                    //FileStream fs = new FileStream(RFileName, FileMode.Open, FileAccess.Read);
                    var date = DateTime.Parse("12:00:00 AM");
                    var eodReport = _tillCloseManager.PrintEodDetails(tillNumber, ref date, ref date, Terminals[1, 1],
                          Terminals[1, 2], Terminals[2, 1], Terminals[2, 2], false, batchDate);
                    reports.Add(eodReport);
                }
                return reports;
            }
            finally
            {
                if (!string.IsNullOrEmpty(rFileName))
                    FileSystem.Kill(rFileName);
            }
        }

        /// <summary>
        /// Method to update post pay
        /// </summary>
        /// <param name="newStatus">New status</param>
        /// <param name="error">Error message</param>
        public void UpdatePostPay(bool newStatus, out ErrorMessage error)
        {
            var user = _loginManager.GetExistingUser(UserCode);
            error = new ErrorMessage();
            var blEnabled = !newStatus;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            bool stateOn;
            try
            {
                stateOn = TCPAgent.Instance.IsConnected;
            }
            catch
            {
                stateOn = false;
            }
            if (!stateOn)
            {
                if (blEnabled) 
                {
                    //        MsgBox ("Communication problem, Cannot turn off PostPay!~Turn Off PostPay Error!")
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 12, 60, null, MessageType.OkOnly);
                    error.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 12, 61, null, MessageType.OkOnly);
                    error.StatusCode = HttpStatusCode.NotFound;
                }
                return;
            }
            if (!_policyManager.GetPol("U_AD_PI", user))
            {
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 12, 92, null, MessageType.OkOnly);
                error.StatusCode = HttpStatusCode.Forbidden;
                return;
                // Get a user who is authorized to turn on/off post pay
            }

            //cmdPostPay.Enabled = false;
            var strSend = blEnabled ? "Isd0" : "Isd1";

            var response = "";

            var timeIn = (float)DateAndTime.Timer;
            TCPAgent.Instance.Send_TCP(ref strSend, true);

            while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
            {
                var strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting " + strSend);
                if (!string.IsNullOrEmpty(strBuffer))
                {
                    var strRemain = "";
                    modStringPad.SplitResponse(strBuffer, strSend, ref response, ref strRemain); //strBuffer<>""
                    if (!string.IsNullOrEmpty(response)) //got what we are waiting
                    {
                        TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                        WriteToLogFile("modify PortReading from PostPay On/Off: " + strRemain);
                        break;
                    }
                }
                if (DateAndTime.Timer < timeIn)
                {
                    timeIn = (float)DateAndTime.Timer;
                }
                Variables.Sleep(100);
            }

            if (Strings.Left(response, 6) != strSend + "OK") //response is not Isd0OK / Isd1OK
            {
                if (blEnabled) 
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 12, 60, null, MessageType.OkOnly);
                    error.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 12, 61, null, MessageType.OkOnly);
                    error.StatusCode = HttpStatusCode.NotFound;
                }
                // Return to current user
                return;
            }
            _fuelService.SavePostPayEnabled(newStatus);
            _fuelService.Save_PostPaySetManually(true);

            // cmdPostPay.Enabled = true;
            WriteToLogFile("PostPay was set to " + newStatus + " by the user " + user.Code + " " + user.Name + ". Return to current user " + user.Code + " " + user.Name);
            // Return to current user

        }

        /// <summary>
        /// Methos to update prepay
        /// </summary>
        /// <param name="newStatus">New status</param>
        /// <param name="error">Error message</param>
        public void UpdatePrepay(bool newStatus, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var blEnabled = !newStatus;
            bool stateOn;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            try
            {
                stateOn = TCPAgent.Instance.IsConnected;
            }
            catch
            {
                stateOn = false;
            }
            if (!stateOn)
            {
                if (blEnabled) 
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 12, 62, null, MessageType.OkOnly);
                    error.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 12, 63, null, MessageType.OkOnly);
                    error.StatusCode = HttpStatusCode.NotFound;
                }
                return;
            }

            var strSend = blEnabled ? "Ppy0" : "Ppy1";

            var response = "";
            var strRemain = "";

            var timeIn = (float)DateAndTime.Timer;
            TCPAgent.Instance.Send_TCP(ref strSend, true);

            while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
            {
                var strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                modStringPad.WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting " + strSend);
                if (!string.IsNullOrEmpty(strBuffer))
                {
                    modStringPad.SplitResponse(strBuffer, strSend, ref response, ref strRemain); //strBuffer<>""
                    if (!string.IsNullOrEmpty(response)) //got what we are waiting
                    {
                        TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                        modStringPad.WriteToLogFile("modify PortReading from Prepay On/Off: " + strRemain);
                        break;
                    }
                }
                if (DateAndTime.Timer < timeIn)
                {
                    timeIn = (float)DateAndTime.Timer;
                }
                Variables.Sleep(100);
            }

            if (Strings.Left(response, 6) != strSend + "OK") //response is not Ppy0OK/Ppy1OK
            {
                if (blEnabled) 
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 12, 62, null, MessageType.OkOnly);
                    error.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 12, 63, null, MessageType.OkOnly);
                    error.StatusCode = HttpStatusCode.NotFound;
                }
                return;
            }
            _fuelService.SavePrepayEnabled(newStatus);
        }

        /// <summary>
        /// Method to initiaise close batch
        /// </summary>
        /// <param name="error">Error message</param>
        /// <returns></returns>
        public bool Initialize(out ErrorMessage error)
        {
            error = new ErrorMessage();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (GetConnection()) //Nancy
            {
                var emvProcess = _policyManager.EMVVersion;
                modTPS.cc = new Credit_Card();
                if (InitializePinpad(ref modTPS.cc, emvProcess, out error))
                {
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 1294, null, CriticalOkMessageType)
                    };
                    error.StatusCode = HttpStatusCode.OK;
                    //Chaps_Main.DisplayMsgForm((System.Convert.ToDouble(12)) * 100 + 94, (short)99, null, (byte)0, (byte)0, "", "", "", "");
                }
                else
                {
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 1295, null, CriticalOkMessageType)
                    };
                    error.StatusCode = HttpStatusCode.OK;
                    //Chaps_Main.DisplayMsgForm((System.Convert.ToDouble(12)) * 100 + 95, (short)99, null, (byte)0, (byte)0, "", "", "", "");
                }
            }
            else
            {
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 1296, null, CriticalOkMessageType)
                };
                error.StatusCode = HttpStatusCode.Conflict;

                //Chaps_Main.DisplayMsgForm((System.Convert.ToDouble(12)) * 100 + 96, (short)99, null, (byte)0, (byte)0, "", "", "", "");
            }
            modTPS.cc = null;
            WriteToLogFile("CC object set to nothing in cmdInitPinPad - frmMaintenance"); // TEST ONLY - to be removed after we fix the CC issue
            return true;
        }

        #region Private methods

        /// <summary>
        /// Method to get connection
        /// </summary>
        /// <returns></returns>
        private bool GetConnection()
        {
            _client?.Close();
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ip = _utilityService.GetPosAddress((byte)PosId);
            var ipAddress = IPAddress.Parse(ip);
            var remoteEndPoint = new IPEndPoint(ipAddress, 8888);
            try
            {
                _client.Connect(remoteEndPoint);
            }
            catch (Exception)
            {
                // ignored
            }
            return _client.Connected;
        }

        /// <summary>
        /// Method to send to TPS
        /// </summary>
        /// <param name="strRequest"></param>
        /// <param name="cc"></param>
        /// <param name="emvProcess"></param>
        /// <returns></returns>
        private bool SendToTps(string strRequest, ref Credit_Card cc, bool emvProcess)
        {
            short retry = 0;
            bool isWex = false;

            while (retry < 3) //From Table
            {
                if (_client.Connected)
                {
                    object sendStringSuf = "," + "END-DATA";
                    if (cc != null)
                    {
                        if ((cc.Crd_Type == "F" && cc.GiftType.ToUpper() == "W") || cc.Crd_Type == "WEX")
                        {
                            isWex = true;
                        }
                    }

                    if (isWex)
                    {

                        object startHeader = 0x1;

                        object sequenceNumber = 0x1;
                        var endTransmit = (byte)0x4;


                        strRequest = startHeader + System.Convert.ToString(sequenceNumber) + (strRequest.Length.ToString("0000") + strRequest); // For WEX TPS Specific
                        sendStringSuf = endTransmit;
                    }
                    WriteToLogFile("Send to STPS: " + strRequest + Convert.ToString(sendStringSuf));

                    _client.Send(Encoding.ASCII.GetBytes(strRequest + Convert.ToString(sendStringSuf)));
                    Variables.Sleep(200);
                    var response = string.Empty;
                    if (_client.Available > 0)
                    {
                        byte[] data = new byte[2048];
                        _client.Receive(data);
                        response = Encoding.UTF8.GetString(data);
                    }
                    if (!string.IsNullOrEmpty(response))
                    {
                        WriteToLogFile("Received from STPS: " + response);
                        GetResponse(response, ref cc, emvProcess);
                    }
                    return true;
                }
                GetConnection();
                retry++;
            }
            return false;
        }

        /// <summary>
        /// Method to get response
        /// </summary>
        /// <param name="strResponse"></param>
        /// <param name="cc"></param>
        /// <param name="emvProcess"></param>
        /// <returns></returns>
        private string GetResponse(string strResponse, ref Credit_Card cc, bool emvProcess)
        {
            string returnValue = string.Empty;

            WriteToLogFile("GetResponse procedure response is " + cc.Response);
            cc.Response = GetStrPosition(strResponse, (short)15).Trim().ToUpper();
            if (string.IsNullOrEmpty(strResponse))
            {
                return returnValue;
            }
            if (emvProcess) //EMVVERSION 'Added May4,2010
            {
                cc.Card_Swiped = cc.ManualCardProcess == false;
            }
            cc.Result = GetStrPosition(strResponse, 16).Trim();
            cc.Authorization_Number = GetStrPosition(strResponse, 17).Trim().ToUpper();
            cc.ResponseCode = GetStrPosition(strResponse, 29).Trim().ToUpper();
            //  EMVVERSION
            if (emvProcess) //EMVVERSION
            {
                cc.Crd_Type = GetStrPosition(strResponse, 2).Trim().Substring(0, 1);
                _creditCardManager.SetTrack2(ref cc, GetStrPosition(strResponse, 12).Trim().ToUpper());
                cc.Swipe_String = cc.Track2;
            }
            //shiny end-EMVVERSION

            
            
            var strSeq = GetStrPosition(strResponse, 5).Trim();
            if (_policyManager.BankSystem != "Moneris")
            {
                cc.Sequence_Number = string.IsNullOrEmpty(strSeq) ? "" : strSeq.Substring(0, strSeq.Length - 1);
            }
            else //Moneris
            {
                cc.Sequence_Number = strSeq;
            }
            

            cc.TerminalID = GetStrPosition(strResponse, 8).Trim();
            cc.DebitAccount = GetStrPosition(strResponse, 11).Trim();
            var strDate = GetStrPosition(strResponse, 21).Trim();
            //Nancy changed,10/21/02
            if (string.IsNullOrEmpty(strDate))
            {
                cc.Trans_Date = DateTime.Now;
            }
            else
            {
                try
                {
                    cc.Trans_Date = DateTime.Parse(strDate);
                }
                catch (Exception)
                {
                    cc.Trans_Date = DateTime.Now;
                }
            }
            var strTime = GetStrPosition(strResponse, (short)22).Trim();
            if (string.IsNullOrEmpty(strTime))
            {
                cc.Trans_Time = DateTime.Parse(DateTime.Now.ToString("hhmmss"));
            }
            else
            {
                try
                {
                    cc.Trans_Time = DateTime.Parse(strTime);
                }
                catch (Exception)
                {
                    cc.Trans_Time = DateTime.Parse(DateTime.Now.ToString("hhmmss"));
                }
            }
            //    cc.Trans_Date = Trim(GetStrPosition(strResponse, 21))
            //    cc.Trans_Time = Trim(GetStrPosition(strResponse, 22))
            cc.ApprovalCode = GetStrPosition(strResponse, 18).Trim();
            cc.Receipt_Display = GetStrPosition(strResponse, 23).Trim();

            //    cc.Report = Trim(GetStrPosition(strResponse, 30))
            if (emvProcess) //EMVVERSION
            {
                cc.Report = GetStrPosition(strResponse, 31).Trim();
                cc.BankMessage = GetStrPosition(strResponse, 30).Trim();
            }
            else
            {
                cc.Report = GetStrPosition(strResponse, 30).Trim();
            }

            // Nicolette added next lines
            if (cc.AskVechicle)
            {
                _creditCardManager.SetVehicleNumber(ref cc, GetStrPosition(strResponse, 33).Trim());
            }
            if (cc.AskIdentificationNo)
            {
                _creditCardManager.SetIdNumber(ref cc, GetStrPosition(strResponse, 34).Trim());
            }
            if (cc.AskDriverNo)
            {
                _creditCardManager.SetDriverNumber(ref cc, GetStrPosition(strResponse, 34).Trim());
            }
            if (cc.AskOdometer)
            {
                _creditCardManager.SetOdoMeter(ref cc, GetStrPosition(strResponse, 35).Trim());
            }
            
            if (!_policyManager.EMVVersion) //  this is for pinpad swipe
            {
                if (cc.Track2 == "" && GetStrPosition(strResponse, 12) != "")
                {
                    //        12/20/06 end
                    _creditCardManager.SetTrack2(ref cc, GetStrPosition(strResponse, 12).Trim());
                }
            }
            
            _creditCardManager.SetIdNumber(ref cc, GetStrPosition(strResponse, 3).Trim());
            if (emvProcess == false) //EMVVERSION ' 
            {
                if (GetStrPosition(strResponse, 1).Trim().ToUpper() == "SWIPEINSIDE")
                {
                    cc.Card_Swiped = (GetStrPosition(strResponse, 15).Trim().ToUpper() == "SWIPED");
                }
            }
            if (emvProcess) // 31 position is card name
            {
                cc.Name = GetStrPosition(strResponse, 33).Trim(); // Trim(GetStrPosition(strResponse, 32))
            }

            return returnValue;
        }

        /// <summary>
        /// Method to get position
        /// </summary>
        /// <param name="strRenamed"></param>
        /// <param name="lo"></param>
        /// <returns></returns>
        private string GetStrPosition(string strRenamed, short lo)
        {
            string strTemp = "";

            var strT = strRenamed;
            var returnValue = "";
            var i = (short)0;
            if (strRenamed.Length > 0)
            {
                while (i < lo)
                {
                    var intTemp = (short)(strT.IndexOf(",") + 1);
                    if ((intTemp == 0) && (!string.IsNullOrEmpty(strT)))
                    {
                        strTemp = strT;
                        strT = "";
                    }
                    else
                    {
                        if (intTemp > 0) //  added to prevent from occurring runtime error
                        {
                            strTemp = strT.Substring(0, intTemp - 1);
                            strT = strT.Substring(intTemp + 1 - 1);
                        }
                    }
                    i++;
                }
                returnValue = strTemp;

            }
            return returnValue;
        }

        /// <summary>
        /// Method to initialise pin pad
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="emvProcess"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private bool InitializePinpad(ref Credit_Card cc, bool emvProcess, out ErrorMessage error)
        {
            bool returnValue = false;
            object processTimer = null;
            SendToTps("InitInside" + "," + "Debit" + ",1,,,,,,,,,,,,,,,,", ref cc, emvProcess);

            processTimer = DateAndTime.Timer;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            error = new ErrorMessage
            {
                MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8139, null, CriticalOkMessageType)
            };

            while (Convert.ToInt32(DateAndTime.Timer - Convert.ToDouble(processTimer)) < 90)
            {
                if (cc.Response.Length > 0)
                {
                    break;
                }
            }
            returnValue = cc.Response.ToUpper() == "APPROVED";
            return returnValue;
        }

        #endregion

    }
}

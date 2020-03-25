using System;
using System.IO;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.Entities;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using log4net;
using Infonet.CStoreCommander.Logging;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    /// <summary>
    /// Manager to perform cash draw and drop operations
    /// </summary>
    public class CashManager : ICashManager
    {
        private readonly ICashService _cashService;
        private readonly IReceiptManager _receiptManager;
        private readonly ITillService _tillService;
        private readonly IReasonService _reasonService;
        private readonly IPolicyManager _policyManager;
        private readonly ILoginManager _loginManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ITenderManager _tenderManager;
        private readonly ISaleManager _saleManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cashService"></param>
        /// <param name="receiptManager"></param>
        /// <param name="tillService"></param>
        /// <param name="reasonService"></param>
        /// <param name="policyManager"></param>
        /// <param name="loginManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="tenderManager"></param>
        /// <param name="saleManager"></param>
        public CashManager(ICashService cashService, IReceiptManager receiptManager, ITillService tillService,
            IReasonService reasonService, IPolicyManager policyManager,
            ILoginManager loginManager, IApiResourceManager resourceManager, ITenderManager tenderManager,
            ISaleManager saleManager)
        {
            _cashService = cashService;
            _receiptManager = receiptManager;
            _reasonService = reasonService;
            _policyManager = policyManager;
            _loginManager = loginManager;
            _resourceManager = resourceManager;
            _tillService = tillService;
            _tenderManager = tenderManager;
            _saleManager = saleManager;
        }

        /// <summary>
        /// Method to get cash draw buttons
        /// </summary>
        /// <returns>Cash draw buttons</returns>
        public CashDrawButton GetCashDrawButtons(string userCode, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,GetCashDrawButtons,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            error = new ErrorMessage();
            var user = _loginManager.GetExistingUser(userCode);
            if (!Convert.ToBoolean(_policyManager.GetPol("U_TILLDRAW", user)))
            {
              MessageType temp_VbStyle3 = (int)MessageType.Exclamation + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet,38, 56, null, temp_VbStyle3);
                error.StatusCode = System.Net.HttpStatusCode.Forbidden;
                _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return null;
            }

            var cashDrawButton = new CashDrawButton
            {
                Coins = _cashService.GetCoins(),
                Bills = _cashService.GetBills()
            };
            _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return cashDrawButton;
        }

        // code moved to cash bonus controller
        ///// <summary>
        ///// Method to get cash draw buttons
        ///// </summary>
        ///// <returns>Cash draw buttons</returns>
        //public CashDrawButton GetCashBonusDrawButtons(string userCode, out ErrorMessage error)
        //{
        //    var dateStart = DateTime.Now;
        //    _performancelog.Debug($"Start,CashManager,GetCashDrawButtons,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
        //    var offSet = _policyManager.LoadStoreInfo().OffSet;
        //    error = new ErrorMessage();
        //    var user = _loginManager.GetExistingUser(userCode);
        //    if (!Convert.ToBoolean(_policyManager.GetPol("U_TILLDRAW", user)))
        //    {
        //        MessageType temp_VbStyle3 = (int)MessageType.Exclamation + MessageType.OkOnly;
        //        error.MessageStyle = _resourceManager.CreateMessage(offSet, 38, 56, null, temp_VbStyle3);
        //        error.StatusCode = System.Net.HttpStatusCode.Forbidden;
        //        _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        //        return null;
        //    }

        //    var cashDrawButton = new CashDrawButton
        //    {
        //        Coins = _cashService.GetCoins(),
        //        Bills = _cashService.GetBills()
        //    };
        //    _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        //    return cashDrawButton;
        //}


        /// <summary>
        /// Print cash draw 
        /// </summary>
        /// <param name="cashDraw">Cash draw</param>
        /// <param name="userCode"></param>
        /// <param name="copies">Copies</param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public FileStream CompleteCashDraw(CashDrawButton cashDraw, string userCode,out int copies,
            out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,GetCashDrawButtons,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            copies = _policyManager.CashDrawReceiptCopies;
            errorMessage = new ErrorMessage();
            var message = string.Empty;
            var till = _tillService.GetTill(cashDraw.TillNumber);
            if (till == null)
            {
                message = "Till does not exists";
            }

            if (cashDraw.Amount <= 0)
            {
                message = "Invalid Cash Drop Amount";
            }

            if (!string.IsNullOrEmpty(message))
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = message
                };
                errorMessage.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return null;
            }
            var isInvalidCash = false;
            var isInvalidQuantity = false;

            var cashDrawButtons = GetCashDrawButtons(userCode, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return null;
            }

            ValidateCoins(cashDraw, ref isInvalidCash, ref isInvalidQuantity, cashDrawButtons);
            ValidateBills(cashDraw, ref isInvalidCash, ref isInvalidQuantity, cashDrawButtons);

            if (isInvalidCash)
            {
                message = "Invalid Cash or Bill";
            }

            if (isInvalidQuantity)
            {
                message = "Invalid Cash or Bill Quantity";
            }

            if (!string.IsNullOrEmpty(message))
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = message
                };
                errorMessage.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return null;
            }

            //print receipt
            var reasonType = ReasonType.CashDraw;
            var reason = _reasonService.GetReturnReason(cashDraw.DrawReason, (char)reasonType);
            if (_policyManager.DRAW_REASON && reason == null)
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = "Invalid Reason"
                };
                errorMessage.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return null;
            }

            var draw = new CashDraw
            {
                TillNumber = cashDraw.TillNumber,
                DrawDate = DateTime.Now,
                TotalValue = (float)cashDraw.Amount,
                User = userCode.ToUpper(),
                Reason = cashDraw.DrawReason
            };

            // add values to cash draw
            _cashService.AddCashDraw(draw);

            //update till
            if (till != null)
            {
                till.Cash = till.Cash + cashDraw.Amount;
                _tillService.UpdateTill(till);
                _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return _receiptManager.Print_Draw(till, cashDraw.RegisterNumber, userCode, cashDraw.Coins,
                    cashDraw.Bills, reason, cashDraw.Amount);
            }
            return null;
        }

        /// <summary>
        /// Method to get cash buttons
        /// </summary>
        /// <returns>List of cash buttons</returns>
        public List<CashButton> GetCashButtons()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,GetCashDrawButtons,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var cashButtons = _cashService.GetCashButtons();
            var cash = new List<CashButton>();
            foreach (var cashButton in cashButtons)
            {
                cash.Add(new CashButton
                {
                    Button = $"${cashButton.Value}",
                    Value = cashButton.Value
                });
            }
            _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return cash;

        }

        /// <summary>
        ///  Method to complete cash drop
        ///  </summary>
        /// <param name="selectedTenders">Selected tenders</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="reason">Reason code</param>
        /// <param name="envelopeNumber">Envelope number</param>
        /// <param name="copies">Copies</param>
        /// <param name="error">Error</param>
        /// <returns>Stream</returns>
        public FileStream CompleteCashDrop(List<Tender> selectedTenders, int tillNumber, string userCode,
          byte registerNumber, string reason, string envelopeNumber,out int copies, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,CompleteCashDrop,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            copies = _policyManager.CashDropReceiptCopies;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            error = new ErrorMessage();
            Till till = _tillService.GetTill(tillNumber);
            if (till == null)
            {
                error.MessageStyle = new MessageStyle
                {
                    Message = "Till does not exists"
                };
                error.StatusCode = System.Net.HttpStatusCode.NotFound;
                return null;
            }
            User user = _loginManager.GetExistingUser(userCode);
            if (!Convert.ToBoolean(_policyManager.GetPol("U_TILLDROP", user)))
            {
                MessageType temp_VbStyle3 = (int)MessageType.Exclamation + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet,38, 55, null, temp_VbStyle3);
                error.StatusCode = System.Net.HttpStatusCode.Forbidden;
                return null;

            }

            var dropDate = DateTime.Now;

            if (!IsValidReason(reason))
            {
                error.MessageStyle = new MessageStyle
                {
                    Message = "Invalid reason"
                };
                error.StatusCode = System.Net.HttpStatusCode.NotFound;
                return null;
            }


            if (_policyManager.DropEnv && string.IsNullOrEmpty(envelopeNumber))
            {
                error.MessageStyle = _resourceManager.CreateMessage(offSet,12, 65, null, MessageType.OkOnly);
                error.StatusCode = System.Net.HttpStatusCode.NotFound;
                return null;
            }

            
            
            var bt = Convert.ToString(_policyManager.BASECURR);
            
            var cntDrop = _cashService.GetMaxCashDrop(till.Number, till.ShiftDate, till.Shift);

            var tenders = _tenderManager.Load(null, "CashDrop", false, reason, out error);

            var cashDropTenders = GetCashDropTenders(selectedTenders, tenders, 0, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return null;
            }
            CashDrop cashDrop = new CashDrop
            {
                Envelope_No = envelopeNumber,
                ReasonCode = reason,
                DropID = _cashService.GetMaxDropId(DataSource.CSCTills)
            };

            //  -   adding the unique dropid (max drop number)
            if (cashDrop.DropID == 0)
            {
                var dropId = _cashService.GetMaxDropId(DataSource.CSCTrans);
                cashDrop.DropID = dropId == 0 ? 1 : dropId;
            }
            // 
            var fs = _receiptManager.PrintDrop(cashDropTenders, till, user, registerNumber, cashDrop, cntDrop);
            AddDropHeader(tillNumber, userCode, till, dropDate, cntDrop, cashDrop);

            AddDropLines(till, bt, dropDate, cashDropTenders, cashDrop);
            CacheManager.DeleteTendersForCashDrop("CashDrop", reason);
            _performancelog.Debug($"End,CashManager,CompleteCashDrop,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return fs;
        }

        /// <summary>
        /// Method to update the tenders selected
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="reason">Reason code</param>
        /// <param name="errorMessage">Error Message</param>
        /// <returns></returns>
        public Tenders UpdateCashDropTendered(List<Tender> tenders, string reason, int saleNumber,
           int tillNumber, string userCode, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,UpdateCashDropTendered,{string.Empty},{dateStart:hh.mm.ss.ffffff}");


            errorMessage = new ErrorMessage();

            if (!IsValidReason(reason))
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = "Invalid reason"
                };
                errorMessage.StatusCode = System.Net.HttpStatusCode.NotFound;
                return null;
            }

            var allTenders = _tenderManager.Load(null, "CashDrop", false, reason, out errorMessage);
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            decimal saleTotals = 0;
            if (sale != null)
            {
                saleTotals = sale.Sale_Totals.Gross;
            }
            var cashDropTenders = GetCashDropTenders(tenders, allTenders, saleTotals, out errorMessage);
            _performancelog.Debug($"End,CashManager,UpdateCashDropTendered,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return cashDropTenders;
        }
  
        // ==================================================================================
        // Open the cash drawer
        // ==================================================================================
        /// <summary>
        /// Method to save the cash drawer reason
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="reasonCode">Reason code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="error">Error message</param>
        public void OpenCashDrawer(string userCode, string reasonCode, int tillNumber, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,OpenCashDrawer,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            User currentUser = _loginManager.GetExistingUser(userCode);
            error = new ErrorMessage();

            var till = _tillService.GetTill(tillNumber);
            if (till == null)
            {
                error.MessageStyle = new MessageStyle
                {
                    Message = "Till does not exists"
                };
                error.StatusCode = System.Net.HttpStatusCode.NotFound;
                return;
            }


            if (!_policyManager.GetPol("U_OPENDRW", currentUser))
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                
                MessageType temp_VbStyle = (int)MessageType.Exclamation + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet,11, 43, null, temp_VbStyle);
                // Get a user who is authorized to open cash drawer
                error.StatusCode = System.Net.HttpStatusCode.Forbidden;
                return;
            }

            
            
            // code moved to App
            // if (modPrint.Open_Cash_Drawer())
            // {
            // Nicolette added to implement reasons for open cash drawer

            
            
            if (_policyManager.OCD_REASON)
            {
                
                var cd = new CashDrawer();
                var rType = ReasonType.OpenCashDrawer;
                var returnReason = _reasonService.GetReturnReason(reasonCode, (char)rType);
                if (returnReason != null)
                {
                    cd.Return_Reason = returnReason;
                    Save_CashDrawer(cd.Return_Reason, currentUser, tillNumber);
                }
                else
                {
                    error.MessageStyle = new MessageStyle
                    {
                        Message = "Invalid reason"
                    };
                    // Get a user who is authorized to open cash drawer
                    error.StatusCode = System.Net.HttpStatusCode.NotFound;
                    return;
                }

            }
            // Nicolette end
            _performancelog.Debug($"End,CashManager,OpenCashDrawer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        #region Private Methods

        /// <summary>
        /// Method to verify tenders
        /// </summary>
        /// <param name="selectedTender">Selected tender</param>
        /// <param name="amountEntered">Amount entered</param>
        /// <param name="saleTotals">Sale totals</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns></returns>
        private decimal VerifyTenderInformation(Tender selectedTender, decimal amountEntered,
           decimal saleTotals, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,VerifyTenderInformation,{string.Empty},{dateStart:hh.mm.ss.ffffff}");


            errorMessage = new ErrorMessage();
            decimal tenderedAmount = amountEntered;
            string[] capValue = new string[3];
            // Doing a Cash Drop
            var grossTotal = (decimal)9999999999.0D;

            if (Conversion.Val(amountEntered) > 99999.99)
            {
                tenderedAmount = 0;
            }
            if (selectedTender.MaxAmount != 0) // need to check only if it is not zero- otherwise no restriction
            {
                if (Convert.ToDecimal(amountEntered) > Convert.ToDecimal(selectedTender.MaxAmount))
                {
                    //MsgBox " Maximum amount allowed for the tender" & TenderName & " is " & TenderMax. Do you want to continue(Y/N)
                    //CapValue[1] = TenderName;
                    //CapValue[2] = TenderMax;
                    //ans = (short)(_resourceManager.CreateMessage(offSet,offSet,0, (short)449, MessageType.YesNo, CapValue, (byte)0));
                    //if (ans == (int)MsgBoxResult.Yes)
                    //{
                    tenderedAmount = Convert.ToDecimal(selectedTender.MaxAmount);
                    //}
                    //else
                    //{
                    //   amountEntered = "";
                    //    lblUsed[Active_Line].Text = "";
                    //    txtAmount[Active_Line].Enabled = true;
                    //    if (txtAmount[Active_Line].Enabled)
                    //    {
                    //        txtAmount[Active_Line].Focus();
                    //    }
                    //    IsEnter = false;
                    //    return;
                    //}
                }
            }
            if (selectedTender.MinAmount != 0 & saleTotals > 0) //Then ' shouldn't < minamount if there is a setting for tender
            {
                if (Convert.ToDecimal(amountEntered) < Convert.ToDecimal(selectedTender.MinAmount))
                {
                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    //MsgBox " Minimum amount required for the tender" & TenderName & " is " & TenderMin
                    capValue[1] = selectedTender.Tender_Name;
                    capValue[2] = selectedTender.MinAmount.ToString();
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet,0, (short)450, capValue, MessageType.OkOnly);
                    //lblUsed[Active_Line].Text = "";
                    //txtAmount[Active_Line].Enabled = true;
                    //if (txtAmount[Active_Line].Enabled)
                    //{
                    //    txtAmount[Active_Line].Focus();
                    //}
                    return 0;
                }
            }
            // 

            if (grossTotal > 0 & tenderedAmount < 0)
            {
                tenderedAmount = 0;
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                // MsgBox "Negative Amounts are NOT Allowed for a Sale"
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet,14, 75, null, MessageType.OkOnly);
                errorMessage.StatusCode = System.Net.HttpStatusCode.NotFound;
            }
            _performancelog.Debug($"End,CashManager,VerifyTenderInformations,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Math.Round(tenderedAmount, 2);
        }

        /// <summary>
        /// Method to save cash drawer
        /// </summary>
        /// <param name="rr">Return reason</param>
        /// <param name="user">User</param>
        /// <param name="tillNumber">Till number</param>
        private void Save_CashDrawer(Return_Reason rr, User user, int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,Save_CashDrawer,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            _cashService.SaveCashDraw(tillNumber, user.Code, rr.Reason);
            _performancelog.Debug($"End,CashManager,Save_CashDrawer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }


        /// <summary>
        /// Method to validate if cash drop reason is valid or not
        /// </summary>
        /// <param name="reason">Reason</param>
        /// <returns>True or false</returns>
        private bool IsValidReason(string reason)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,IsValidReason,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var flag = true;
            if (_policyManager.SAFEATMDROP && (reason != "ATM" && reason != "SAFE"))
            {
                flag = false;
            }
            else if (!_policyManager.SAFEATMDROP && reason != "SAFE")
            {
                flag = false;

            }
            _performancelog.Debug($"End,CashManager,IsValidReason,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return flag;
        }

        /// <summary>
        /// Method to get cash drop tenders
        /// </summary>
        /// <param name="selectedTenders">Selected tenders</param>
        /// <param name="tenders">All Tenders</param>
        /// <param name="totals">totals</param>
        /// <param name="error">Error message</param>
        /// <returns>Tenders</returns>
        private Tenders GetCashDropTenders(List<Tender> selectedTenders, Tenders tenders, decimal
            totals, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,GetCashDropTenders,{string.Empty},{dateStart:hh.mm.ss.ffffff}");


            error = new ErrorMessage();
            Sale sale = null;
            var isInvalidTender = false;
            Tenders cashDropTenders = tenders;
            foreach (var tender in selectedTenders)
            {
                var selectedTender = tenders.FirstOrDefault(t => t.Tender_Code == tender.Tender_Code);
                if (selectedTender == null)
                {
                    isInvalidTender = true;
                    break;
                }
                tender.MinAmount = selectedTender.MinAmount;
                tender.MaxAmount = selectedTender.MaxAmount;
                decimal tenderedAmount = VerifyTenderInformation(tender, tender.Amount_Entered,
                    totals, out error);
                if (string.IsNullOrEmpty(error.MessageStyle.Message))
                {
                    //selectedTender = cashDropTenders.Add(selectedTender.Tender_Name, selectedTender.Tender_Class, selectedTender.Exchange_Rate,
                    //    selectedTender.Give_Change, selectedTender.Give_As_Refund, selectedTender.System_Can_Adjust,
                    //    selectedTender.Sequence_Number, selectedTender.Tender_Code, selectedTender.Exact_Change,
                    //    selectedTender.MaxAmount, selectedTender.MinAmount, selectedTender.Smallest_Unit,
                    //    selectedTender.Open_Drawer, Convert.ToDouble(tenderedAmount), selectedTender.PrintCopies,
                    //    selectedTender.AcceptAspayment, selectedTender.SignatureLine,selectedTender.Image, selectedTender.Tender_Code);
                    _tenderManager.Set_Amount_Entered(ref cashDropTenders, ref sale, selectedTender, tenderedAmount, -1);
                }
                else
                {
                    return null;
                }
            }

            if (isInvalidTender)
            {
                error.MessageStyle = new MessageStyle
                {
                    Message = "Invalid Tender"
                };
                error.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return null;
            }
            _performancelog.Debug($"End,CashManager,GetCashDropTenders,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return cashDropTenders;
        }

        /// <summary>
        /// Method to add all drop lines
        /// </summary>
        /// <param name="till">Till</param>
        /// <param name="bt">Base currency</param>
        /// <param name="dropDate">Drop date</param>
        /// <param name="cashDropTenders">Cash drop tenders</param>
        /// <param name="cashDrop">Cash drop</param>
        private void AddDropLines(Till till, string bt, DateTime dropDate, Tenders cashDropTenders, CashDrop cashDrop)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,AddDropLines,{string.Empty},{dateStart:hh.mm.ss.ffffff}");


            foreach (Tender tempLoopVarT in cashDropTenders)
            {
                var T = tempLoopVarT;
                if (T.Amount_Entered > 0)
                {
                    var dropLine = new DropLine
                    {
                        TillNumber = till.Number,
                        DropDate = dropDate,
                        TenderName = T.Tender_Name,
                        ExchangeRate = T.Exchange_Rate,
                        Amount = T.Amount_Entered,
                        ConvAmount = T.Amount_Used,
                        DropID = cashDrop.DropID

                    };
                    _cashService.AddDropLine(dropLine);
                    if (T.Tender_Name.ToUpper() == bt.ToUpper())
                    {
                        till.Cash = till.Cash - T.Amount_Entered;
                        _tillService.UpdateTill(till);
                    }
                    //Cash bonus out of scope
                    //  track the cash bonus
                    //if (Policy_Renamed.Use_CashBonus)
                    //{
                    //    if (T.Tender_Name.ToUpper() == modGlobalFunctions.Get_TenderName(System.Convert.ToString(Policy_Renamed.CBonusTend)).ToUpper())
                    //    {
                    //        rs = _dbService.GetRecords("Select *  FROM   Tills  WHERE  Tills.Till_Num = " + till.Number + " ", DataSource.CSCMaster, ADODB.CursorTypeEnum.adOpenForwardOnly);
                    //        rs.Fields["CashBonus"].Value = rs.Fields["CashBonus"].Value - T.Amount_Entered;
                    //        rs.Update();
                    //        rs = null;
                    //    }
                    //}
                    //Shiny end
                }
                _performancelog.Debug($"End,CashManager,AddDropLines,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            }
        }

        /// <summary>
        /// Method to add drop header
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="till">Till</param>
        /// <param name="dropDate">Drop date</param>
        /// <param name="cntDrop">Drop count</param>
        /// <param name="cashDrop">Cash drop</param>
        private void AddDropHeader(int tillNumber, string userCode, Till till, DateTime dropDate, short cntDrop,
            CashDrop cashDrop)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,AddDropHeader,{string.Empty},{dateStart:hh.mm.ss.ffffff}");


            var dropHeader = new DropHeader
            {
                DropDate = dropDate,
                UserCode = userCode.ToUpper(),
                TillNumber = tillNumber,
                DropCount = cntDrop,
                ShiftId = till.Shift,
                ShiftDate = till.ShiftDate,
                EnvelopeNo = cashDrop.Envelope_No,
                ReasonCode = string.IsNullOrEmpty(cashDrop.ReasonCode) ? "SAFE" : cashDrop.ReasonCode, // 
                DropId = cashDrop.DropID
            };
            _cashService.AddDropHeader(dropHeader);
            _performancelog.Debug($"End,CashManager,AddDropHeader,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to validate bill
        /// </summary>
        /// <param name="cashDraw">Cash draw</param>
        /// <param name="isInvalidCash">Is valid cash or not</param>
        /// <param name="isInvalidQuantity">Is valid quantity</param>
        /// <param name="cashDrawButtons">cash drae buttons</param>
        private void ValidateBills(CashDrawButton cashDraw, ref bool isInvalidCash, ref bool
            isInvalidQuantity, CashDrawButton cashDrawButtons)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,GetCashDrawButtons,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            foreach (var bill in cashDraw.Bills)
            {
                if (!cashDrawButtons.Bills.Any(c => c.CurrencyName == bill.CurrencyName
                && c.Value == bill.Value))
                {
                    isInvalidCash = true;
                    break;
                }
                if (bill.Quantity < 1 && bill.Quantity > 99)
                {
                    isInvalidQuantity = true;
                    break;
                }
            }
            _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to validate coins
        /// </summary>
        /// <param name="cashDraw">Cash draw</param>
        /// <param name="isInvalidCash">Is valid cash or not</param>
        /// <param name="isInvalidQuantity">Is valid quantity</param>
        /// <param name="cashDrawButtons">cash drae buttons</param>
        private void ValidateCoins(CashDrawButton cashDraw, ref bool isInvalidCash, ref bool
            isInvalidQuantity, CashDrawButton cashDrawButtons)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,GetCashDrawButtons,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            foreach (var coin in cashDraw.Coins)
            {
                if (!cashDrawButtons.Coins.Any(c => c.CurrencyName == coin.CurrencyName
                && c.Value == coin.Value))
                {
                    isInvalidCash = true;
                    break;
                }
                if (coin.Quantity < 1 && coin.Quantity > 99)
                {
                    isInvalidQuantity = true;
                    break;
                }
            }
            _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }
        #endregion
    }
}

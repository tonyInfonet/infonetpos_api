using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using User = Infonet.CStoreCommander.Entities.User;
using Microsoft.VisualBasic;
using System.Threading;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class PaymentManager : ManagerBase, IPaymentManager
    {
        private readonly IApiResourceManager _resourceManager;
        private readonly ISaleService _saleService;
        private readonly ISaleLineManager _saleLineManager;
        private readonly IPolicyManager _policyManager;
        private readonly ISaleManager _saleManager;
        private readonly ITillService _tillService;
        private readonly IReceiptManager _receiptManager;
        private readonly ITenderManager _tenderManager;
        private readonly ILoginManager _loginManager;
        private readonly ITenderService _tenderService;
        private readonly ICreditCardManager _creditCardManager;
        private readonly ICardManager _cardManager;
        private readonly ICustomerManager _customerManager;
        private readonly IUtilityService _utilityService;
        private readonly ISaleVendorCouponManager _svcManager;
        private readonly IPrepayManager _prepayManager;
        private readonly IMainManager _mainManager;
        private readonly IKickBackManager _kickBackManager;
        private readonly IWexManager _wexManager;
        public static string kickBackError = null;
        
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="saleService"></param>
        /// <param name="saleLineManager"></param>
        /// <param name="utilityService"></param>
        /// <param name="policyManager"></param>
        /// <param name="saleManager"></param>
        /// <param name="tillService"></param>
        /// <param name="receiptManager"></param>
        /// <param name="tenderManager"></param>
        /// <param name="loginManager"></param>
        /// <param name="tenderService"></param>
        /// <param name="creditCardManager"></param>
        /// <param name="cardManager"></param>
        /// <param name="customerManager"></param>
        /// <param name="svcManager"></param>
        /// <param name="prepayManager"></param>
        /// <param name="mainManager"></param>
        public PaymentManager(IApiResourceManager resourceManager,
            ISaleService saleService,
            ISaleLineManager saleLineManager,
            IPolicyManager policyManager,
            ISaleManager saleManager,
            ITillService tillService,
            IReceiptManager receiptManager,
            ITenderManager tenderManager,
            ILoginManager loginManager,
            ITenderService tenderService,
            ICreditCardManager creditCardManager,
            ICardManager cardManager,
            ICustomerManager customerManager,
            IUtilityService utilityService,
            ISaleVendorCouponManager svcManager,
            IPrepayManager prepayManager,
            IMainManager mainManager,
            IKickBackManager kickBackManager,
            IWexManager wexManager)
        {
            _resourceManager = resourceManager;
            _saleService = saleService;
            _saleLineManager = saleLineManager;
            _policyManager = policyManager;
            _saleManager = saleManager;
            _tillService = tillService;
            _receiptManager = receiptManager;
            _tenderManager = tenderManager;
            _loginManager = loginManager;
            _tenderService = tenderService;
            _creditCardManager = creditCardManager;
            _cardManager = cardManager;
            _customerManager = customerManager;
            _utilityService = utilityService;
            _svcManager = svcManager;
            _prepayManager = prepayManager;
            _mainManager = mainManager;
            _kickBackManager = kickBackManager;
            _wexManager = wexManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        /// <param name="userCode"></param>
        /// <param name="receipt"></param>
        /// <param name="fileName"></param>
        /// <param name="errorMessage"></param>
        /// <param name="lcdMsg"></param>
        public Sale ByCashExact(int tillNumber, int saleNumber, string userCode,
            ref Report receipt, ref string fileName, out ErrorMessage errorMessage,
            out CustomerDisplay lcdMsg)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,PaymentManager,ByCashExact,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var offSet = _policyManager.LoadStoreInfo().OffSet;

            bool hasFuelSale = false;
            lcdMsg = null;
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle?.Message))
            {
                return null;
            }
            errorMessage = new ErrorMessage();

            if (sale.DeletePrepay)
            {
                //Please complete delete prepay first!~Complete current transaction.
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 50, null);
                errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                return null;
            }


            var security = _policyManager.LoadSecurityInfo();
            //   checking the expire date to prevent users from changing the system date and cheat the security system
            _loginManager.GetInstallDate(ref security);
            security.ExpireDate = DateTime.FromOADate(security.Install_Date.ToOADate() + 50 * 365); // 50 Year
            if (security.ExpireDate < DateAndTime.Today)
            {
                // "Transactions are not allowed beyond expiry date. Please check the date and restart POS", vbOKOnly, "Software License Expired"
                //Chaps_Main.DisplayMessage(0, (short)8194, MsgBoxStyle.OkOnly, null, (byte)0);
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8194, null);
                errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                return null;
            }
            //Shiny end
            //  - crash recovery ' if there is partial/ full payment donot allow exact change
            if (sale.Payment)
            {
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 15, null, CriticalOkMessageType);
                errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                return null;
            }
            //shiny end
            Sale_Line saleLine;
            var user = _loginManager.GetExistingUser(userCode);

            if (user.User_Group.Code == Entities.Constants.Trainer) //Behrooz Jan-12-06
            {
                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    saleLine = tempLoopVarSl;
                    if (saleLine.ProductIsFuel && (!saleLine.IsPropane))
                    {
                        errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 56, null, CriticalOkMessageType);
                        errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                        return null;
                    }
                }
            }

            if (_policyManager.CouponMSG)
            {
                if (sale.Sale_Totals.Gross >= _policyManager.CupnThrehld)
                {
                    float fuelSaleAmount = 0;
                    foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                    {
                        saleLine = tempLoopVarSl;
                        if (saleLine.ProductIsFuel && (!saleLine.IsPropane))
                        {
                            hasFuelSale = true;
                            fuelSaleAmount = fuelSaleAmount + (float)saleLine.Amount;
                        }
                    }

                    if (hasFuelSale && (fuelSaleAmount >= _policyManager.CupnThrehld))
                    {
                        errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 58,
                            _policyManager.CouponType);
                        errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                        return null;
                    }
                }
            }



            var prepayItem = PrepayItemID(sale);
            if (prepayItem != 0)
            {
                short prepayPumpId = sale.Sale_Lines[prepayItem].pumpID;




                char PrepayPriceType = '1'; //Hardcoded to 1 due to cash payment
                float prepayAmount = Convert.ToSingle(Math.Round(Convert.ToDouble(sale.Sale_Totals.Gross), 2));
                byte prepayPosition = 7;
                if (SetPrepayFromFc(prepayPumpId, (byte)tillNumber, out errorMessage))
                {
                    SetPrepayment(sale.Sale_Num, prepayPumpId, prepayAmount, prepayPosition);
                }
                else
                {
                    SetPrepaymentFromPos(sale.Sale_Num, sale.TillNumber, prepayPumpId, prepayAmount,
                       (byte)(Conversion.Val(PrepayPriceType)), prepayPosition);
                }
            }

            //Transaction_Type = "Sale";


            //var MillExchangeAmount = 0;


            // var AccumulateAmount = 0;
            // var AccumulateCard = "";
            // var MillExchangeCard = "";
            //Out of scope kickback
            //   to ask for phone number or card if it was not entered from customer screen
            //if (_policyManager.Use_KickBack && sale.Customer.PointCardNum == "")
            //{
            //    var STFDNumber = "";

            //    //Chaps_Main.DisplayMsgForm(_policyManager.LoyaltyMesg, (short)15, null, (byte)0, (byte)0, "", "", "", "");
            //    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(offSet,_policyManager.LoyaltyMesg, 15, null);

            //    if (!string.IsNullOrEmpty(STFDNumber))
            //    {
            //        STFDNumber = "";
            //    }
            //}
            Register register = new Register();
            _mainManager.SetRegister(ref register, sale.Register);

            if (register.Customer_Display)
            {
                lcdMsg = _mainManager.DisplayMsgLcd(register, _mainManager.FormatLcdString(register, _resourceManager.GetResString(offSet, 165), ""), "");
            }

            _saleManager.ReCompute_CashBonus(ref sale);
            //Shiny mar1,2009'- only we need cash bonus calculation only at the end to print or to save in the table


            //Shiny Mar1,2009- - need to do it here- After the receipt all sa is cleared-If there is Cash bonus - Need to show in the screen
            Stream signature;
            if (_policyManager.PRINT_REC || (sale.Sale_Totals.Gross < 0 && _policyManager.PRINT_VOID))
            {


                receipt = ExactChange_Receipt(sale, user, tillNumber, out errorMessage, out signature, ref fileName);
            }
            else
            {
                receipt = ExactChange_NoReceipt(sale, user, tillNumber, out errorMessage, out signature, ref fileName);
            }
            sale = _saleManager.InitializeSale(tillNumber, sale.Register, userCode, out errorMessage);

            Performancelog.Debug($"End,PaymentManager,ByCashExact,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return sale;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <param name="userCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="receipt"></param>
        /// <returns></returns>
        public Sale RunAway(int saleNumber, int tillNumber, string userCode, ref Report receipt, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,PaymentManager,RunAway,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            errorMessage = new ErrorMessage();

            var till = _tillService.GetTill(tillNumber);
            if (till == null)
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = "Till does not exists"
                };
                errorMessage.StatusCode = HttpStatusCode.NotFound;
                return null;
            }

            var user = _loginManager.GetExistingUser(userCode);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            object intCntNoFuelLines = null;

            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {

                if (sale.DeletePrepay)
                {
                    //Please complete delete prepay first!~Comlete current transaction.
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 50, null);
                    errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                    return null;
                }


                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    var saleLine = tempLoopVarSl;
                    if (!saleLine.ProductIsFuel)
                    {
                        intCntNoFuelLines = Convert.ToInt32(intCntNoFuelLines) + 1;
                    }
                }
                CustomerChange(true, false, ref sale, tillNumber, out errorMessage);

                if (Convert.ToInt32(intCntNoFuelLines) != 0)
                {
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 78, null);
                    errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                    return null;
                }



                //  - If reversal runaway automatically finish it as runawy -ve
                if (sale.ReverseRunaway)
                {
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 25, null);
                    errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                    return null;
                }
                Tenders nullTenders = null;
                var fileName = "RunAway";
                var rePrint = false;
                Stream signature;
                sale.Sale_Type = "RUNAWAY";
                receipt = _receiptManager.Print_Receipt(tillNumber, null, ref sale, ref nullTenders, true, ref fileName, ref rePrint, out signature, user.Code);
                _saleManager.Clear_Sale(sale, saleNumber, tillNumber, userCode, "RUNAWAY", null, true, true, false,
                    out errorMessage);

                sale = _saleManager.InitializeSale(tillNumber, sale.Register, userCode, out errorMessage);
            }
            Performancelog.Debug($"End,PaymentManager,RunAway,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return sale;
        }

        /// <summary>
        /// Method to complete a transaction as pump test
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="newSale">New Sale</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Report content</returns>
        public Report CompletePumpTest(int saleNumber, int tillNumber, string userCode,
            out Sale newSale, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,PaymentManager,CompletePumpTest,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            newSale = new Sale();
            errorMessage = new ErrorMessage();

            var user = _loginManager.GetExistingUser(userCode);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            int intCntNoFuelLines = 0;

            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode,
                out errorMessage);
            if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                // checking delete prepay crash recovery
                if (sale.DeletePrepay)
                {
                    //Please complete delete prepay first!~Comlete current transaction.
                    MessageType temp_VbStyle = (int)MessageType.OkOnly + MessageType.Information;
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, (short)50, null, temp_VbStyle);
                    return null;
                }
                Sale_Line SL;
                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    SL = tempLoopVarSl;
                    if (!SL.ProductIsFuel)
                    {

                        intCntNoFuelLines = System.Convert.ToInt32(intCntNoFuelLines) + 1;
                    }
                }
                _saleManager.SetCustomer(Utilities.Constants.CashSaleClient, saleNumber, tillNumber, userCode,
                    sale.Register, string.Empty, out errorMessage);

                Chaps_Main.LoyCard = "";
                Chaps_Main.LoyExpDate = "";
                Chaps_Main.LoyCrdSwiped = false;


                if (Convert.ToInt32(intCntNoFuelLines) != 0)
                {
                    MessageType tempVbStyle2 = (int)MessageType.OkOnly;
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, (short)78, null, tempVbStyle2);
                    return null;
                }



                //  - If reversal pump test automatically finish it as pumptest -ve
                if (sale.ReversePumpTest)
                {
                    errorMessage.MessageStyle = new MessageStyle { Message = Utilities.Constants.PumpTestNotAllowed };
                    return null;
                }

                sale.Sale_Type = "PUMPTEST";
                Tenders nullTenders = null;
                var fileName = "PumpTest";
                var rePrint = false;
                Stream signature;
                var receipt = _receiptManager.Print_Receipt(tillNumber, null, ref sale, ref nullTenders, true, ref fileName, ref rePrint, out signature, user.Code);
                _saleManager.Clear_Sale(sale, saleNumber, tillNumber, userCode, "PUMPTEST", null,
                    false, true, false, out errorMessage);
                newSale = _saleManager.InitializeSale(tillNumber, sale.Register,
                    userCode, out errorMessage);
                return receipt;
            }

            Performancelog.Debug($"End,PaymentManager,RunAway,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return null;
        }

        /// <summary>
        /// Method to create exact change with receipt
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="user">User</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="errorMessage">Error</param>
        /// <param name="signature">Signature</param>
        /// <param name="fileName">File name</param>
        /// <returns>Report content</returns>
        public Report ExactChange_Receipt(Sale sale, User user, int tillNumber,
            out ErrorMessage errorMessage, out Stream signature, ref string fileName)
        {
            Report receipt;
            errorMessage = new ErrorMessage();
            if (sale.Sale_Type == "MARKDOWN")
            {
                Tenders nullTenders = null;
                //var fileName = string.Empty;
                var rePrint = false;
                receipt = _receiptManager.Print_Receipt(tillNumber, null, ref sale, ref nullTenders, true, ref fileName, ref rePrint, out signature, user.Code);
                _saleManager.SaveSale(sale, user.Code, ref nullTenders, null);
                _saleManager.Clear_Sale(sale, sale.Sale_Num, tillNumber, user.Code, "Initial", null, false, true, false, out errorMessage);

            }
            else
            {
                var tendersRenamed = Exact_Change(sale, tillNumber, out errorMessage);
                //var fileName = string.Empty;
                var rePrint = false;
                receipt = _receiptManager.Print_Receipt(tillNumber, null, ref sale, ref tendersRenamed, true, ref fileName, ref rePrint, out signature, user.Code);
                if (sale.Sale_Totals.Gross < 0)
                {
                    _saleManager.Clear_Sale(sale, sale.Sale_Num, tillNumber, user.Code, "REFUND", tendersRenamed, false, true, false, out errorMessage);
                }
                else
                {
                    _saleManager.Clear_Sale(sale, sale.Sale_Num, tillNumber, user.Code, "SALE", tendersRenamed, false, true, false, out errorMessage);
                }
            }
            return receipt;
        }

        /// <summary>
        /// Method to complete payment
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="userCode">User code</param>
        /// <param name="issueSc">Issue store credit</param>
        /// <param name="error">Error</param>
        /// <param name="openCashDrawer">Open cash drawer</param>
        /// <param name="changeDue">Change due</param>
        /// <param name="isRefund">Refund sale</param>
        /// <param name="lcdDisplay"></param>
        /// <returns>Report content</returns>
        public List<Report> CompletePayment(int saleNumber, int tillNumber, string transactionType,
            string userCode, bool issueSc, out ErrorMessage error, out bool openCashDrawer,
            out string changeDue, out bool isRefund, out CustomerDisplay lcdDisplay)
        {
            //var sample = CacheManager.GetCurrentSaleForTill(tillNumber, saleNumber);
            openCashDrawer = false;
            changeDue = "0.00";
            isRefund = false;
            lcdDisplay = null;
            var till = _tillService.GetTill(tillNumber);
         
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 1, userCode, out error);
            if (sale == null)
            {
                return null;
            }
            var trnType = "";
            if (transactionType == "Delete Prepay")
            {
                trnType = transactionType;
                transactionType = "Sale";
            }
            var tenders = _tenderManager.GetAllTender(saleNumber, tillNumber,
                 transactionType, userCode, false, string.Empty, out error);

            if (error?.MessageStyle?.Message != null)
            {
                if (error.MessageStyle.MessageType != MessageType.YesNo)
                    return null;
                tenders = new Tenders();
            }

            if (!string.IsNullOrEmpty(trnType))
            {
                transactionType = trnType;
            }

            var prepayItem = _prepayManager.PrepayItemId(ref sale);
            byte prepayPumpId = 0;
            if (prepayItem != 0)
            {
                prepayPumpId = sale.Sale_Lines[prepayItem].pumpID;
            }

            string PrepayPriceType = "1";



            if (prepayItem != 0 && transactionType != "Delete Prepay")
            {



                if (_prepayManager.SetPrepayFromFc(prepayPumpId, tillNumber, sale.Sale_Num, PrepayPriceType, out error))
                {
                    _prepayManager.SetPrepayment(sale.Sale_Num, prepayPumpId, Variables.Pump[prepayPumpId].PrepayAmount, (byte)(Variables.Pump[prepayPumpId].PrepayPosition));
                }
                else
                {
                    _prepayManager.SetPrepaymentFromPos(sale.Sale_Num, prepayPumpId,
                        Variables.Pump[prepayPumpId].PrepayAmount, (byte)(Conversion.Val(PrepayPriceType)),
                        (byte)(Variables.Pump[prepayPumpId].PrepayPosition), tillNumber);
                }
                transactionType = "Sale";
            }
            else if (transactionType == "Delete Prepay")
            {


                var deletePrepayPumpId = sale.Sale_Lines[1].pumpID;

                if (!_prepayManager.DeletePrepayFromFc(deletePrepayPumpId, false, out error))
                {
                    _prepayManager.DeletePrepaymentFromPos(deletePrepayPumpId);
                }
                else
                {
                    _prepayManager.DeletePrepaymentFromPos(deletePrepayPumpId);
                }
                if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
                {
                    return null;
                }
                transactionType = "Sale";
            }

            var reports = new List<Report>();
          
            switch (transactionType)
            {
                case "Sale":
                    {
                        WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside payment 593 cardno value" + sale.Customer.PointCardNum);
                        //sale.Customer.PointCardNum = sample.Customer.PointCardNum;
                        //sale.Customer.Balance_Points = sample.Customer.Balance_Points;
                        if (_policyManager.Use_KickBack && sale.Customer.PointCardNum != "")
                        {
                            WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside payment 598 cardno value" + sale.Customer.PointCardNum);
                            _kickBackManager.ProcessKickBack((short)1,UserCode,ref sale,out error);
                            kickBackError = error.MessageStyle.Message;
                            
                          
                       
                        }

                        reports = _tenderManager.Finishing_Sale(tenders, transactionType, saleNumber, userCode, till,
                         issueSc, out error, out openCashDrawer, out changeDue, out isRefund);
                        break;
                    }
                case "ARPay":
                    {
                        reports = _tenderManager.Finishing_ARPay(tenders, saleNumber, userCode, till, issueSc,
                            out openCashDrawer, out changeDue, out error);
                        break;
                    }
                case "Payment":
                    {
                        reports = _tenderManager.Finishing_Payment(tenders, saleNumber, userCode, till, issueSc,
                            out openCashDrawer, out changeDue, out error);
                        break;
                    }
            }
            var chDue = changeDue;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            changeDue = string.Format(Utilities.Constants.ChangeDue, changeDue, _resourceManager.GetResString(offSet, 166));
            Register register = new Register();
            _mainManager.SetRegister(ref register, sale.Register);
            if (register.Customer_Display)
            {
                lcdDisplay = _mainManager.DisplayMsgLcd(register, _mainManager.FormatLcdString(register, _resourceManager.GetResString(offSet, (short)166), chDue), "");
            }

            //if (kickBackError != null)
            //{
            //    error.MessageStyle.Message = kickBackError;
            //    error.StatusCode = HttpStatusCode.NotAcceptable;
               
            //}

            return reports;
        }


        public string KickbackCommunicationError()
        {
            if (kickBackError == null)
            {
                return null;
            }
            return kickBackError;
        }


        /// <summary>
        /// Method to perform payment by credit/debit/fleet
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="cardNumber">Card number</param>
        /// <param name="userCode">User code</param>
        /// <param name="amountUsed">Amount used</param>
        /// <param name="merchantFileStream">Merchant file</param>
        /// <param name="customerFileStream">Customer file</param>
        /// <param name="errorMessage">Error</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="poNumber">PO number</param>
        /// <returns>Tenders</returns>
        public Tenders ByCard(int saleNumber, int tillNumber, string cardNumber, string userCode, string transactionType,
           string poNumber, string amountUsed, ref Report merchantFileStream, ref Report customerFileStream, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            string ip;
            int port;
            Performancelog.Debug($"Start,PaymentManager,ByCard,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            errorMessage = new ErrorMessage();

            Tenders updateTenders = null;

            //Validation of tillnumber
            var till = _tillService.GetTill(tillNumber);
            if (till == null)
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = "Till does not exists"
                };
                errorMessage.StatusCode = HttpStatusCode.NotFound;
                return null;
            }
            var tenders = _tenderManager.GetAllTender(saleNumber, till.Number, transactionType,
                userCode, false, string.Empty, out errorMessage);
            var user = _loginManager.GetExistingUser(UserCode);
            if (user.User_Group.Code == "Trainer") return tenders;
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return null;
            }
            //Validate Sale
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                var cc = new Credit_Card();
                var cardInfo = _tenderManager.FindCardTender(cardNumber, saleNumber, tillNumber, transactionType, userCode, out errorMessage,
                    ref cc, false);

                if (cc.Crd_Type == "F" && cc.GiftType == "W")
                {
                    int msgNum = 0;
                    if (modTPS.cc == null)
                    {
                        modTPS.cc = cc;
                    }
                    if (!_wexManager.ValidWexTransaction(sale.Sale_Lines.Count, float.Parse(amountUsed,CultureInfo.InvariantCulture), ref msgNum)) // to validate the wex transaction
                    {
                        errorMessage.MessageStyle.Message = _resourceManager.GetResString(offSet, (short)msgNum);
                        Variables.blChargeAcct = false;
                        return null;
                    }
                }


                if (modTPS.cc != null && cc.GiftType.ToUpper() == "W")
                {
                    var prompts = modTPS.cc.CardPrompts;
                    modTPS.cc = cc;

                    for (int i = prompts.Count; i > 0; i--)
                    {
                        foreach (CardPrompt prompt in prompts)
                        {
                            if (modTPS.cc.CardPrompts[i].PromptMessage.Equals(prompt.PromptMessage))
                            {
                                modTPS.cc.CardPrompts[i].PromptAnswer = prompt.PromptAnswer;
                            }
                        }
                    }
                }
                else
                {
                    modTPS.cc = cc;
                }
               // modTPS.cc.CardPrompts = prompts;
                if (errorMessage?.MessageStyle?.Message != null)
                {
                    return null;
                }
                var tenderCode = cardInfo.TenderCode;
                if (cardInfo.SelectedCard == CardForm.None)
                {
                    return tenders;
                }
                string amountEntered = string.IsNullOrEmpty(amountUsed) ? cardInfo.Amount : amountUsed;
                if (!string.IsNullOrEmpty(poNumber))
                {
                    cc.PONumber = poNumber;
                }
                cc.Trans_Amount = (float)(Conversion.Val(string.Format(amountEntered, "##0.00")));
                cc.Trans_Number = (sale.Sale_Num).ToString();
                cc.Void_Num = sale.Void_Num;

                if (sale.Sale_Totals.Gross < 0)
                {
                    if (!cc.RefundAllowed)
                    {
                        //Refund is not allowed with this card.
                        errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 82, null, CriticalOkMessageType);
                        errorMessage.StatusCode = HttpStatusCode.BadRequest;
                        return null;
                    }
                    cc.Trans_Type = "RefundInside";
                }
                else
                {
                    cc.Trans_Type = sale.Void_Num > 0 ? "RefundInside" : "SaleInside";
                }

                if (cc.Trans_Type == "RefundInside" && cc.Trans_Amount > 0)
                {
                    cc.Trans_Amount = -cc.Trans_Amount;
                    amountEntered = "-"+ amountEntered;
                }

                if (_creditCardManager.Call_The_Bank(ref cc)) //Card_Authorized = True
                {
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    

                    if (cc.GiftType.ToUpper() != "W")
                    {
                        ip = _utilityService.GetPosAddress((byte)PosId);
                        port = 8888;
                    }
                    else
                    {
                        ip = _policyManager.WexIp;
                        port = _policyManager.WexPort;
                    }

                    if (ip != null)
                    {
                        var ipAddress = IPAddress.Parse(ip);
                        var remoteEndPoint = new IPEndPoint(ipAddress, port);
                        try
                        {
                            socket.Connect(remoteEndPoint);

                            if (socket.Connected)
                            {
                                //Nancy add the following line to clean up the response at first
                                cc.Response = "";
                                switch (cc.Crd_Type)
                                {
                                    case "D":
                                        SendToTPS(_cardManager.GetRequestString(ref cc, sale, cc.Trans_Type, "Debit", cc.Trans_Amount, ""), ref socket, ref cc);
                                        break;
                                    case "C":
                                        SendToTPS(_cardManager.GetRequestString(ref cc, sale, cc.Trans_Type, "Credit", cc.Trans_Amount, ""), ref socket, ref cc);
                                        break;
                                    case "F":
                                        if (cc.GiftType.ToUpper() == "W")
                                        {
                                           SendToTPS(_wexManager.GetWexString(sale.Sale_Lines.Count, cc.Trans_Amount), ref socket, ref cc);
                                        }
                                        else
                                        {
                                            SendToTPS(_cardManager.GetRequestString(ref cc, sale, cc.Trans_Type, "Fleet", cc.Trans_Amount, ""), ref socket, ref cc);
                                        }
                                        break;
                                }
                                double processTimer = (float)DateAndTime.Timer;
                                var timeout = _policyManager.BankTimeOutSec;
                                if (timeout == 0)
                                {
                                    timeout = 10;
                                }
                                while (DateAndTime.Timer - processTimer < timeout) // 2013 10 17 - Reji Added polity for bank transaction timeout in seconds
                                {
                                    // Data buffer for incoming data.
                                    var bytes = new byte[2048];
                                    var bytesRec = socket.Receive(bytes);
                                    var strBuffer = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                                    WriteToLogFile("Received from STPS: " + strBuffer);
                                    if (cc.GiftType.ToUpper() != "W")
                                    {
                                        GetResponse(strBuffer, ref cc);
                                    }
                                    else
                                    {
                                        _wexManager.AnalyseWexResponse(strBuffer, ref cc);
                                    }
                                    if (cc.Response.Length > 0)
                                    {
                                        break;
                                    }
                                    if (DateAndTime.Timer < processTimer)
                                    {
                                        processTimer = (float)DateAndTime.Timer;
                                    }
                                }

                                if (cc.Response.Trim() == "")
                                {
                                    cc.Response = "NotCompleted";
                                }

                                if (!(cc.Crd_Type == "F" && cc.GiftType == "W"))
                                {

                                    GetTransactionResult(ref tenders, ref cc, tenderCode, ref sale, ref socket,
                                                        ref merchantFileStream, ref customerFileStream, out errorMessage);
                                }
                                else
                                {
                                    if (cc.Response != "APPROVED")
                                    {
                                        if (string.IsNullOrEmpty(cc.Response))
                                        {
                                            errorMessage.MessageStyle.Message = "Payment not Completed";
                                        }
                                        else
                                        {
                                            errorMessage.MessageStyle.Message = cc.Response;
                                        }
                                        return null;
                                    }
                                }

                                if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                                {
                                    var selectedTender = GetSelectedTender(tenders, tenderCode);
                                    selectedTender.Credit_Card = cc;
                                    updateTenders = _tenderManager.UpdateTender(ref tenders, sale, till, transactionType, userCode,
                                         false, tenderCode, amountEntered, out errorMessage);
                                    tenders.Card_Authorized = true;

                                    switch (transactionType)
                                    {
                                        case "Sale":
                                            CacheManager.AddTendersForSale(saleNumber, till.Number, tenders);
                                            break;
                                        case "ARPay":
                                            CacheManager.AddTendersForArPay(saleNumber, tenders);
                                            break;
                                        case "Payment":
                                            CacheManager.AddTendersForPayment(saleNumber, tenders);
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)84, null,
                                    CriticalOkMessageType);
                                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                                return null;
                            }
                        }
                        catch(Exception ex)
                        {
                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)84, null,
                                  CriticalOkMessageType);
                            errorMessage.StatusCode = HttpStatusCode.BadRequest;
                            return null;
                        }
                    }
                }
                else
                {


                    if (!cc.AutoRecognition && amountEntered.Equals("0"))
                    {
                        tenders.Card_Authorized = false;
                    }
                    else
                    {
                        _creditCardManager.Authorize_Card(ref cc);
                        cc.Trans_Date = DateAndTime.Today;
                        cc.Trans_Time = DateAndTime.TimeOfDay;
                        cc.Result = "0";
                        var selectedTender = GetSelectedTender(tenders, tenderCode);
                        selectedTender.Credit_Card = cc;

                        tenders.Card_Authorized = true;
                    }

                    updateTenders = _tenderManager.UpdateTender(ref tenders, sale, till, transactionType, userCode, false, tenderCode,
                        amountEntered, out errorMessage);

                    switch (transactionType)
                    {
                        case "Sale":
                            CacheManager.AddTendersForSale(saleNumber, till.Number, tenders);
                            break;
                        case "ARPay":
                            CacheManager.AddTendersForArPay(saleNumber, tenders);
                            break;
                        case "Payment":
                            CacheManager.AddTendersForPayment(saleNumber, tenders);
                            break;
                    }
                }

                if (cc.Crd_Type == "F" && cc.GiftType == "W")
                {
                    var x = (short)50;
                    modTPS.cc = cc; 
                    modTPS.cc.Report = _wexManager.GetWexRecieptString(ref x, sale, ref tenders);
                }


            }

            Performancelog.Debug(
                $"End,PaymentManager,ByCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return updateTenders;
        }

        /// <summary>
        /// Method to perform payment by coupon tender
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="couponId">Coupon Id</param>
        /// <param name="blTillClose">Till close</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Tenders</returns>
        public Tenders ByCoupon(int saleNumber, int tillNumber, string transactionType, string couponId,
            bool blTillClose, string tenderCode, string userCode,
            out ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();
            var till = _tillService.GetTill(tillNumber);
            if (till == null)
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = "Till does not exists"
                };
                errorMessage.StatusCode = HttpStatusCode.NotFound;
                return null;
            }
            var tenders = _tenderManager.GetAllTender(saleNumber, till.Number, transactionType, userCode, blTillClose,
                string.Empty, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return null;
            }

            var couponValue = ValidateCoupon(couponId, out errorMessage);
            var amountEntered = couponValue.ToString("##0.00");

            if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                var sale = _saleManager.GetCurrentSale(saleNumber, till.Number, 0, userCode, out errorMessage);
                _tenderManager.Load_Temp_Tenders(ref tenders, till, ref sale);
                var isInvalidTender = false;
                AR_Payment arPay = null;
                Payment payment = null;
                if (transactionType == "ARPay")
                {
                    arPay = CacheManager.GetArPayment(saleNumber);
                }
                if (transactionType == "Payment")
                {
                    payment = CacheManager.GetFleetPayment(saleNumber);
                }
                bool displayNoReceiptButton;
                var gross = _tenderManager.GetGrossTotal(transactionType, sale, payment, arPay, -1, out displayNoReceiptButton);

                if (!tenders.Any(t => t.Tender_Code == tenderCode || t.Tender_Class == tenderCode))
                {
                    isInvalidTender = true;
                }
                else
                {
                    List<Report> transactReports = null;
                    _tenderManager.SaleTend_Keydown(ref tenders, sale, userCode, tenderCode, ref amountEntered,
                        transactionType, gross, null, out transactReports, out errorMessage);
                    _tenderManager.UpdateTenders(saleNumber, tillNumber, transactionType, userCode, blTillClose, tenderCode, amountEntered, out transactReports, out errorMessage);
                    tenders.Coupon = couponId;
                }
                if (isInvalidTender)
                {
                    errorMessage.MessageStyle = new MessageStyle
                    {
                        Message = "Invalid Tender"
                    };
                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                CacheManager.AddTendersForSale(saleNumber, till.Number, tenders);
            }
            return tenders;
        }


        /// <summary>
        /// Method to perform a transaction by account
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="amountTend">Amount tendered</param>
        /// <param name="tillClose">Close till</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="userCode">User code</param>
        /// <param name="purchaseOrder">Purchase order</param>
        /// <param name="overrideArLimit">Override ar limit</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Tenders</returns>
        public Tenders ByAccount(int saleNumber, int tillNumber, string transactionType, string amountTend,
       bool tillClose, string tenderCode, string userCode, string purchaseOrder, bool overrideArLimit,
       out ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();
            var till = _tillService.GetTill(tillNumber);
            if (till == null)
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = "Till does not exists"
                };
                errorMessage.StatusCode = HttpStatusCode.NotFound;
                return null;
            }
            var tenders = _tenderManager.GetAllTender(saleNumber, till.Number, transactionType, userCode, tillClose,
                string.Empty, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                var sale = _saleManager.GetCurrentSale(saleNumber, till.Number, 0, userCode, out errorMessage);
                _tenderManager.Load_Temp_Tenders(ref tenders, till, ref sale);
                var isInvalidTender = false;
                AR_Payment arPay = null;
                Payment payment = null;
                if (transactionType == "ARPay")
                {
                    arPay = CacheManager.GetArPayment(saleNumber);
                }
                if (transactionType == "Payment")
                {
                    payment = CacheManager.GetFleetPayment(saleNumber);
                }
                bool displayNoReceiptButton;
                var gross = _tenderManager.GetGrossTotal(transactionType, sale, payment, arPay, -1, out displayNoReceiptButton);

                if (!tenders.Any(t => t.Tender_Code == tenderCode || t.Tender_Class == tenderCode))
                {
                    isInvalidTender = true;
                }
                else
                {
                    List<Report> transactReports = null;
                    _tenderManager.SaleTend_Keydown(ref tenders, sale, userCode, tenderCode, ref amountTend,
                        transactionType, gross, null, out transactReports, out errorMessage, true,
                        purchaseOrder, overrideArLimit);

                    if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                    {
                        return null;
                    }

                    var card = CacheManager.GetCreditCard(tillNumber, saleNumber);
                    var selectedTender = GetSelectedTender(tenders, tenderCode);
                    //if (card != null)
                    //{
                    //    card.Trans_Date = DateTime.Now;
                    //    card.Trans_Time = DateTime.Now;
                    //    card.Result = "0";
                    //    selectedTender.Credit_Card = card;
                    //}
                    //CacheManager.AddTendersForSale(saleNumber, till.Number, tenders);

                    _tenderManager.UpdateTender(ref tenders, sale, till, transactionType, userCode, tillClose,
                    tenderCode, amountTend, out errorMessage);
                }
                if (isInvalidTender)
                {
                    errorMessage.MessageStyle = new MessageStyle
                    {
                        Message = "Invalid Tender"
                    };
                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
            }
            return tenders;
        }


        /// <summary>
        /// Method to verify payment by account
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="amountEntered">Amount entered</param>
        /// <param name="tillClose">Close till</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Payment by account messages</returns>
        public VerifyPaymentByAccount VerifyPaymentByAccount(int saleNumber, int tillNumber, string transactionType,
           string amountEntered, bool tillClose, string tenderCode, string userCode,
           out ErrorMessage errorMessage)
        {
            VerifyPaymentByAccount verifyAccount = null;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            errorMessage = new ErrorMessage();
            var till = _tillService.GetTill(tillNumber);
            if (till == null)
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = "Till does not exists"
                };
                errorMessage.StatusCode = HttpStatusCode.NotFound;
                return null;
            }
            var tenders = _tenderManager.GetAllTender(saleNumber, till.Number, transactionType, userCode, tillClose,
                string.Empty, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                var sale = _saleManager.GetCurrentSale(saleNumber, till.Number, 0, userCode, out errorMessage);
                _tenderManager.Load_Temp_Tenders(ref tenders, till, ref sale);
                var isInvalidTender = false;
                AR_Payment arPay = null;
                Payment payment = null;
                if (transactionType == "ARPay")
                {
                    arPay = CacheManager.GetArPayment(saleNumber);
                }
                if (transactionType == "Payment")
                {
                    payment = CacheManager.GetFleetPayment(saleNumber);
                }
                bool displayNoReceiptButton;
                var gross = _tenderManager.GetGrossTotal(transactionType, sale, payment, arPay, -1, out displayNoReceiptButton);

                if (!tenders.Any(t => t.Tender_Code == tenderCode || t.Tender_Class == tenderCode))
                {
                    isInvalidTender = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(amountEntered))
                    {
                        List<Report> transactReports;
                        _tenderManager.SaleTend_Keydown(ref tenders, sale, userCode, tenderCode, ref amountEntered,
                              transactionType, gross, new Credit_Card(), out transactReports, out errorMessage);
                    }
                    if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                    {
                        return null;
                    }
                    if (_policyManager.Charge_Acct)
                    {
                        if (sale.Customer.AR_Customer == false)
                        {
                            var temp_VbStyle4 = (int)MessageType.OkOnly + MessageType.Information;
                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 4199, sale.Customer.Name, temp_VbStyle4);
                            errorMessage.StatusCode = HttpStatusCode.BadRequest;
                            return null;
                        }
                    }
                    verifyAccount = VerifyAccount(sale, userCode, amountEntered,
                        out errorMessage);

                    if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                    {
                        return null;
                    }
                }
                if (isInvalidTender)
                {
                    errorMessage.MessageStyle = new MessageStyle
                    {
                        Message = "Invalid Tender"
                    };
                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
            }
            return verifyAccount;
        }

        /// <summary>
        /// Method to get sale vendor coupon 
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="coupon">Coupon</param>
        /// <param name="error">Error</param>
        /// <returns>List of sale vendor coupon</returns>
        public List<VCoupon> GetSaleVendorCoupon(int saleNumber, int tillNumber, string userCode,
           string tenderCode, ref string coupon, out ErrorMessage error)
        {
            var lstSerialNum = new List<VCoupon>();
            string transactionType = "Sale";
            var allTenders = _tenderManager.GetAllTender(saleNumber, tillNumber, transactionType,
                 userCode, false, string.Empty, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return null;
            }
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode,
                out error);
            var till = _tillService.GetTill(tillNumber);
            bool displayNoReceipt;
            var gross = _tenderManager.GetGrossTotal(transactionType, sale, null, null, -1,
                out displayNoReceipt);
            var selectedTender = GetSelectedTender(allTenders, tenderCode);
            if (selectedTender == null)
            {
                error.MessageStyle.Message = "Invalid tender";
                return null;
            }
            if (selectedTender.Tender_Class != "COUPON")
            {
                error.MessageStyle.Message = "Invalid tender";
                return null;
            }
            var svc = CacheManager.GetSaleVendorCoupon(saleNumber, selectedTender.Tender_Code) ??
                                   new SaleVendorCoupon
                                   {
                                       Sale_Num = saleNumber,
                                       Till_Num = (byte)tillNumber
                                   };
            var tenderName = selectedTender.Tender_Name;
            coupon = GetDefaultCoupon(tenderName);
            if (!string.IsNullOrEmpty(coupon) && svc.SVC_Lines.Count > 0)
            {
                return cmdSerAdd_Click(ref allTenders, coupon, string.Empty, tenderCode, svc, sale, till,
                   gross, out error);
            }
            short seqNum = 0;
            short lineNum = 0;
            if (!string.IsNullOrEmpty(coupon))
            {
                if (!ValidToAddVendorCoupon(sale, till, svc, coupon, tenderName,
                    ref seqNum, ref lineNum, out error))
                    return null;
            }
            foreach (SaleVendorCouponLine tempLoopVarSvcLine in svc.SVC_Lines)
            {
                var svcLine = tempLoopVarSvcLine;

                var newVCoupon = new VCoupon
                {
                    Coupon = Convert.ToInt32(svcLine.SeqNum),
                };
                if (!string.IsNullOrEmpty(svcLine.SerialNumber))
                {
                    newVCoupon.SerialNumber = svcLine.ItemNum + " - " + svcLine.SerialNumber;
                }
                else
                {
                    newVCoupon.SerialNumber = svcLine.ItemNum.ToString();
                }
                lstSerialNum.Add(newVCoupon);
            }
            return lstSerialNum;
        }

        /// <summary>
        /// Method to add sale vendor coupon
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="coupon">Coupon</param>
        /// <param name="serialNumber">Serial number</param>
        /// <param name="error">Error</param>
        /// <returns>List of sale vendor coupon</returns>
        public List<VCoupon> AddSaleVendorCoupon(int saleNumber, int tillNumber, string userCode,
          string tenderCode, string coupon, string serialNumber, out ErrorMessage error)
        {
            var lstSerialNum = new List<VCoupon>();
            var transactionType = "Sale";
            var allTenders = _tenderManager.GetAllTender(saleNumber, tillNumber, transactionType,
                 userCode, false, string.Empty, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return null;
            }
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode,
                out error);
            var till = _tillService.GetTill(tillNumber);
            bool displayNoReceipt;
            var gross = _tenderManager.GetGrossTotal(transactionType, sale, null, null, -1,
                out displayNoReceipt);
            var selectedTender = GetSelectedTender(allTenders, tenderCode);
            if (selectedTender == null)
            {
                error.MessageStyle.Message = "Invalid tender";
                return null;
            }
            if (selectedTender.Tender_Class != "COUPON")
            {
                error.MessageStyle.Message = "Invalid tender";
                return null;
            }
            var svc = CacheManager.GetSaleVendorCoupon(saleNumber, selectedTender.Tender_Code) ?? new SaleVendorCoupon
            {
                Sale_Num = saleNumber,
                Till_Num = (byte)tillNumber
            };

            var tenderName = selectedTender.Tender_Name;
            var defaultCoupon = GetDefaultCoupon(tenderName);
            if ((!string.IsNullOrEmpty(defaultCoupon) && defaultCoupon != coupon) || string.IsNullOrEmpty(coupon))
            {
                error.MessageStyle.Message = "Invalid coupon entered";
            }
            if (!string.IsNullOrEmpty(coupon))
            {
                return cmdSerAdd_Click(ref allTenders, coupon, serialNumber, tenderCode, svc, sale, till,
                   gross, out error);
            }
            return RefreshSerialNumbers(svc, tenderName);

        }

        /// <summary>
        /// Method to remove sale vendor coupon
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="coupon">Coupon</param>
        /// <param name="error">Error</param>
        /// <returns>List of sale vendor coupon</returns>
        public List<VCoupon> RemoveSaleVendorCoupon(int saleNumber, int tillNumber, string userCode,
          string tenderCode, string coupon, out ErrorMessage error)
        {
            var lstSerialNum = new List<VCoupon>();
            var transactionType = "Sale";
            var allTenders = _tenderManager.GetAllTender(saleNumber, tillNumber, transactionType,
                 userCode, false, string.Empty, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return null;
            }
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode,
                out error);
            bool displayNoReceipt;
            var gross = _tenderManager.GetGrossTotal(transactionType, sale, null, null, -1,
                out displayNoReceipt);
            var selectedTender = GetSelectedTender(allTenders, tenderCode);
            if (selectedTender == null)
            {
                error.MessageStyle.Message = "Invalid tender";
                return null;
            }
            if (selectedTender.Tender_Class != "COUPON")
            {
                error.MessageStyle.Message = "Invalid tender";
                return null;
            }
            var svc = CacheManager.GetSaleVendorCoupon(saleNumber, selectedTender.Tender_Code) ??
                                   new SaleVendorCoupon
                                   {
                                       Sale_Num = saleNumber,
                                       Till_Num = (byte)tillNumber
                                   };

            var tenderName = selectedTender.Tender_Name;
            var item = svc.SVC_Lines.FirstOrDefault(l => l.SeqNum == Convert.ToInt16(coupon));
            if (item != null)
                return RemoveVendorCouponForTender(ref allTenders, tenderCode, svc, saleNumber, tillNumber, gross,
                    tenderName, coupon, item.ItemNum);
            error.MessageStyle.Message = "No sale vendor coupons found";
            return null;
        }

        /// <summary>
        /// Method to perform transaction by sale vendor coupon
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>Tenders</returns>
        public Tenders PaymentByVCoupon(int saleNumber, int tillNumber, string tenderCode,
            string userCode, out ErrorMessage error)
        {
            var transactionType = "Sale";
            var allTenders = _tenderManager.GetAllTender(saleNumber, tillNumber, transactionType,
                 userCode, false, string.Empty, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return null;
            }
            var selectedTender = GetSelectedTender(allTenders, tenderCode);
            if (selectedTender == null)
            {
                error.MessageStyle.Message = "Invalid tender";
                return null;
            }
            if (selectedTender.Tender_Class != "COUPON")
            {
                error.MessageStyle.Message = "Invalid tender";
                return null;
            }
            var svc = CacheManager.GetSaleVendorCoupon(saleNumber, selectedTender.Tender_Code);
            if (svc != null)
            {
                List<Report> transactionReports;
                return _tenderManager.UpdateTenders(saleNumber, tillNumber, transactionType,
                      userCode, false, tenderCode, svc.Amount.ToString(), out transactionReports, out error);
            }
            return allTenders;
        }

        /// <summary>
        /// Method to check till limit and verify exceed limit message
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Till limit message</returns>
        public string CheckTillLimit(int tillNumber)
        {
            var limitExeceededMsg = string.Empty;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (_policyManager.USE_CL_TILL)
            {
                var limit = Convert.ToInt16(_policyManager.CLEAR_TILL);
                var updatedCash = _tillService.GetTill(tillNumber).Cash;
                if (Convert.ToInt32(updatedCash) > limit)
                {
                    limitExeceededMsg = _resourceManager.CreateMessage(offSet, 0, 8309, limit).Message;


                }
            }
            return limitExeceededMsg;
        }

        #region Private methods

        /// <summary>
        /// Method to validate coupons
        /// </summary>
        /// <param name="couponId">Coupon Id</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Coupon value</returns>
        private float ValidateCoupon(string couponId, out ErrorMessage errorMessage)
        {
            var coupon = _saleService.GetCoupon(couponId);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            errorMessage = new ErrorMessage();
            if (coupon == null)
            {

                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 57, null, CriticalOkMessageType);
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                return 0;
            }

            if (coupon.ExpiryDate != null && (coupon.Used || coupon.Void || DateAndTime.Today > coupon.ExpiryDate.Value))
            {
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 57, null, CriticalOkMessageType);
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                return 0;
            }


            var returnValue = Convert.ToSingle(coupon.Amount);

            return returnValue;
        }

        /// <summary>
        /// Method to set prepay from fuel control
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>True ro false</returns>
        private bool SetPrepayFromFc(short pumpId, byte tillNumber, out ErrorMessage
            errorMessage)
        {
            errorMessage = new ErrorMessage();
            bool returnValue = false;
            string response = "";
            string strBuffer = "";
            string strRemain = "";

            returnValue = false;


            response = "";
            strRemain = "";
            string tempCommandRenamed = "Prp" + Strings.Right("0" + Convert.ToString(pumpId), 2) + Strings.Right("00000000" + Convert.ToString(Variables.Pump[pumpId].PrepayInvoiceID), 8) + '1' +
                (Variables.Pump[pumpId].PrepayAmount * 100).ToString("000000") + Strings.Right("000" + Convert.ToString(tillNumber), 3) + Strings.Right("0" + Convert.ToString(Variables.Pump[pumpId].PrepayPosition), 1);

            // //var tcpAgent = new TCPAgent();
            TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);

            var timeIN = (float)DateAndTime.Timer;
            while (!(DateAndTime.Timer - timeIN > Variables.gPumps.CommunicationTimeOut))
            {
                strBuffer = System.Convert.ToString(TCPAgent.Instance.NewPortReading);
                WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting Prp" + Strings.Right("0" + Convert.ToString(pumpId), 2));

                if (!string.IsNullOrEmpty(strBuffer))
                {
                    modStringPad.SplitResponse(strBuffer, "Prp" + Strings.Right("0" + Convert.ToString(pumpId), 2), ref response, ref strRemain); //strBuffer<>""
                    if (!string.IsNullOrEmpty(response)) //got what we are waiting
                    {
                        TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                        WriteToLogFile("modify TCPAgent.PortReading from set Prepayment: " + strRemain);
                        break;
                    }
                }
                if (DateAndTime.Timer < timeIN)
                {
                    timeIN = (float)DateAndTime.Timer;
                }
                Variables.Sleep(100);
            }

            if (Strings.Left(response, 7) != "Prp" + Strings.Right("0" + System.Convert.ToString(pumpId), 2) + "OK")
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                //MsgBox ("Communication problem, restart FuelControl to process prepay or use the delete prepay option to remove prepay!")
                var messageType = (int)MessageType.Information + MessageType.OkOnly;
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, (short)60, null, messageType);

                return false;
            }

            return true;
        }


        /// <summary>
        /// Method to complete payment by exact change
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Tenders</returns>
        private Tenders Exact_Change(Sale sale, int tillNumber, out ErrorMessage
            errorMessage)
        {
            var Transaction_Type = "Sale";
            var tendersRenamed = _tenderManager.Load(sale, Transaction_Type, false, "", out errorMessage);
            _tenderManager.Zero_Tenders(ref tendersRenamed, ref sale);


            //TODO:
            //modPrint.Open_Cash_Drawer();

            foreach (Tender tempLoopVarT in tendersRenamed)
            {
                var T = tempLoopVarT;
                if (T.Tender_Name.ToUpper() == _policyManager.BASECURR.ToUpper())
                {
                    _tenderManager.Set_Amount_Entered(ref tendersRenamed, ref sale, T, sale.Sale_Totals.Gross, -1);
                    T.Amount_Used = (decimal)(Helper.Round((double)sale.Sale_Totals.Gross, 2));
                    break;
                }
            }

            tendersRenamed.Tend_Totals.Change = 0;
            tendersRenamed.Tend_Totals.Tend_Amount = Math.Abs(sale.Sale_Totals.Gross);
            tendersRenamed.Tend_Totals.Gross = Math.Abs(sale.Sale_Totals.Gross);
            tendersRenamed.Tend_Totals.Tend_Used = Math.Abs(sale.Sale_Totals.Gross);
            _tillService.UpdateCash(tillNumber, tendersRenamed[_policyManager.BASECURR].Amount_Used);

            var returnValue = tendersRenamed;

            return returnValue;
        }

        /// <summary>
        /// Method to get prepay item ID
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <returns>Prepay Id</returns>
        private short PrepayItemID(Sale sale)
        {
            short returnValue = 0;
            short i = 0;


            returnValue = (short)0;
            if (sale == null)
            {
                return returnValue;
            }
            if (sale.Sale_Lines.Count < 1)
            {
                return returnValue;
            }


            for (i = 1; i <= sale.Sale_Lines.Count; i++)
            {
                if (sale.Sale_Lines[i].Prepay)
                {
                    returnValue = i;
                    break;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Method to set prepayment
        /// </summary>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="amount">Amount</param>
        /// <param name="position">Position</param>
        private void SetPrepayment(int invoiceId, short pumpId, float amount, byte position)
        {
            bool returnValue = false;
            //reset prepay variables
            Variables.Pump[pumpId].IsPrepay = true;
            //Pump(PumpID).IsMyPrepay = True
            Variables.Pump[pumpId].PrepayAmount = amount;
            Variables.Pump[pumpId].IsPrepayLocked = false;
            //Pump(PumpID).IsPrepayActivated = False
            Variables.Pump[pumpId].PrepayInvoiceID = invoiceId;
            Variables.Pump[pumpId].PrepayPosition = position;
            returnValue = true;
        }

        /// <summary>
        /// Method to set prepayment from POS
        /// </summary>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="amount">Amount</param>
        /// <param name="mop">MOP</param>
        /// <param name="positionId">Position Id</param>
        private void SetPrepaymentFromPos(int invoiceId, int tillNumber, short pumpId, float amount, byte mop, byte positionId)
        {
            bool returnValue = _saleService.SetPrepaymentFromPos(invoiceId, tillNumber, pumpId, amount, mop, positionId);

            //and reset prepay variables
            Variables.Pump[pumpId].IsPrepay = true;
            Variables.Pump[pumpId].PrepayAmount = amount;
            Variables.Pump[pumpId].IsPrepayLocked = false;
            Variables.Pump[pumpId].PrepayInvoiceID = invoiceId;
            Variables.Pump[pumpId].PrepayPosition = positionId;
            returnValue = true;
        }

        /// <summary>
        /// Method to change customer
        /// </summary>
        /// <param name="loadCustomer">Load customer</param>
        /// <param name="refund">Refund or not</param>
        /// <param name="sale">Sale</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="errorMessage">Error</param>
        private void CustomerChange(bool loadCustomer, bool refund, ref Sale sale,
            int tillNumber, out ErrorMessage errorMessage)
        {
            double getPrice = 0;
            Sale_Line SL = new Sale_Line();
            bool cd = false;
            bool Pd = false;
            short loyalPricecode = 0;
            errorMessage = new ErrorMessage();
            if (sale != null && !refund && !sale.EligibleTaxEx && !sale.Apply_CustomerChange) //   added And Not SA.EligibleTaxEx to reevaluate the sale if tax exemption button was clicked in Customer form
            {
                if (sale.Customer.Code == Utilities.Constants.CashSaleClient)
                {
                    return; //   if is the same code don't reevaluate the sale
                }
            }

            //   Don't check if CL is "" or "*", if it is a cash sale, load the cash customer for the sale
            // This works also if the sale has been reset to "Cash Sale" using Customer form "Cash Sale" button
            // Handles the case when the customer is set by policy to a default customer
            //    If CL <> "" And CL <> "*" Then
            WriteToLogFile("Changing customer to " + Utilities.Constants.CashSaleClient);
            if (sale != null)
            {
                sale.Customer.Code = Utilities.Constants.CashSaleClient;

                if (loadCustomer)
                {
                    sale.Customer = _customerManager.LoadCustomer(sale.Customer.Code);
                }


                if (!string.IsNullOrEmpty(Chaps_Main.LoyCard))
                {
                    //Chaps_Main.SA.Customer.LoyaltyCard = Chaps_Main.LoyCard;
                    //Chaps_Main.SA.Customer.LoyaltyExpDate = Chaps_Main.LoyExpDate;
                    //Chaps_Main.SA.Customer.LoyaltyCardSwiped = Chaps_Main.LoyCrdSwiped;
                    /*
                        //
                        if (_policyManager.Use_KickBack)
                        {
                            rsTemp = Chaps_Main.Get_Records("SELECT * FROM Kickback " + " WHERE CustomerCardNum=\'" + Chaps_Main.LoyCard + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
                            if (!rsTemp.EOF)
                            {
                                Chaps_Main.SA.Customer.PointCardNum = System.Convert.ToString(rsTemp.Fields["PointCardNum"].Value);
                                Chaps_Main.SA.Customer.PointCardPhone = System.Convert.ToString(rsTemp.Fields["phonenum"].Value);
                                //                If SA.Customer.PointCardSwipe = "" Then SA.Customer.PointCardSwipe = "0" '0-from database, 1-from phone number, 2-swiped
                                Chaps_Main.SA.Customer.PointCardSwipe = "0"; // 0-from database based on GK card swiped, 1-from phone number, 2-swiped
                            }
                            else
                            {
                                sale.Customer.PointCardNum = "";
                                sale.Customer.PointCardPhone = "";
                                sale.Customer.PointCardSwipe = "";
                            }

                            rsTemp = null;

                        }
                        */
                }
                else
                {
                    sale.Customer.LoyaltyCard = "";
                    sale.Customer.LoyaltyExpDate = "";
                    sale.Customer.LoyaltyCardSwiped = false;
                    sale.Customer.PointCardNum = "";
                    sale.Customer.PointCardPhone = "";
                    sale.Customer.PointCardSwipe = "";
                }


                //    lblCustName = SA.Customer.Name '  - screen flip for SITE
                sale.Sale_Client = sale.Customer.Name;
                //    End If

                // 


                if (_policyManager.USE_LOYALTY && Strings.UCase(Convert.ToString(_policyManager.LOYAL_TYPE)) == "PRICES"
                    && !string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "A")
                {

                    loyalPricecode = Convert.ToInt16(_policyManager.LOYAL_PRICE);
                    getPrice = loyalPricecode; //  if selecting the customer after entering the product not giving loyalty
                }
                else
                {

                    if (sale.Customer.Price_Code >= 1 & sale.Customer.Price_Code <= _policyManager.NUM_PRICE)
                    {
                        getPrice = sale.Customer.Price_Code;
                    }
                    else
                    {
                        getPrice = 1;
                    }
                }


                cd = Convert.ToBoolean(_policyManager.CUST_DISC); // Use customer discount codes

                Pd = Convert.ToBoolean(_policyManager.PROD_DISC); // Use product discount codes

                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    SL = tempLoopVarSl;


                    //  - For fule we shouldn't look for different price_number
                    //If Not sl.Gift_Certificate then
                    if (SL.Gift_Certificate == false && SL.ProductIsFuel == false)
                    {
                        // 


                        if (_policyManager.USE_LOYALTY && Strings.UCase(Convert.ToString(_policyManager.LOYAL_TYPE)) == "PRICES" && string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "A")
                        {
                            if (getPrice != SL.Price_Number)
                            {
                                if (!SL.LOY_EXCLUDE)
                                {
                                    _saleManager.Line_Price_Number(ref sale, ref SL, loyalPricecode);
                                    // Chaps_Main.SA.Line_Price_Number(ref SL, Loyal_pricecode);
                                }
                                else
                                {
                                    _saleManager.Line_Price_Number(ref sale, ref SL, (short)getPrice);
                                    // Chaps_Main.SA.Line_Price_Number(ref SL, (short)Get_Price);
                                }
                            }
                        }
                        else
                        {
                            if (getPrice != SL.Price_Number)
                            {
                                _saleManager.Line_Price_Number(ref sale, ref SL, (short)getPrice);
                                //Chaps_Main.SA.Line_Price_Number(ref SL, (short)Get_Price);
                            }
                        }
                    }
                    //  For using Loyalty discount



                    if (_policyManager.USE_LOYALTY && Strings.UCase(Convert.ToString(_policyManager.LOYAL_TYPE)) == "DISCOUNTS" && string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "A")
                    {
                        if (!SL.LOY_EXCLUDE)
                        {

                            var loydiscode = Convert.ToInt16(_policyManager.LOYAL_DISC);
                            if (cd || Pd)
                            {
                                if (cd && Pd)
                                {
                                    _saleLineManager.Apply_Table_Discount(ref SL, SL.Prod_Discount_Code, loydiscode, out errorMessage);
                                }
                                else if (cd)
                                {
                                    _saleLineManager.Apply_Table_Discount(ref SL, 0, loydiscode, out errorMessage);
                                }
                                else if (Pd)
                                {
                                    _saleLineManager.Apply_Table_Discount(ref SL, SL.Prod_Discount_Code, 0, out errorMessage);
                                }
                            }
                            else
                            {
                                if (cd && Pd)
                                {
                                    _saleLineManager.Apply_Table_Discount(ref SL, SL.Prod_Discount_Code, sale.Customer.Discount_Code, out errorMessage);
                                }
                                else if (cd)
                                {
                                    _saleLineManager.Apply_Table_Discount(ref SL, 0, sale.Customer.Discount_Code, out errorMessage);
                                }
                                else if (Pd)
                                {
                                    _saleLineManager.Apply_Table_Discount(ref SL, SL.Prod_Discount_Code, 0, out errorMessage);
                                }
                            }
                            _saleManager.Line_Discount_Type(ref SL, SL.Discount_Type);
                            _saleManager.Line_Discount_Rate(ref sale, ref SL, SL.Discount_Rate);
                            //Chaps_Main.SA.Line_Discount_Rate(SL, SL.Discount_Rate);
                        }
                    }
                    else
                    {
                        //Shiny end

                        if (cd || Pd)
                        {
                            if (cd && Pd) // Use both customer & product discounts
                            {
                                _saleLineManager.Apply_Table_Discount(ref SL, SL.Prod_Discount_Code, sale.Customer.Discount_Code, out errorMessage);
                            }
                            else if (cd) // Use customer but not product
                            {
                                _saleLineManager.Apply_Table_Discount(ref SL, 0, sale.Customer.Discount_Code, out errorMessage);
                            }
                            else if (Pd) // Use product but not customer
                            {
                                _saleLineManager.Apply_Table_Discount(ref SL, SL.Prod_Discount_Code, 0, out errorMessage);
                            }
                            _saleManager.Line_Discount_Type(ref SL, SL.Discount_Type);
                            // Chaps_Main.SA.Line_Discount_Type(SL, SL.Discount_Type);
                            _saleManager.Line_Discount_Rate(ref sale, ref SL, SL.Discount_Rate);

                            //Chaps_Main.SA.Line_Discount_Rate(SL, SL.Discount_Rate);
                        }
                        WriteToLogFile(" Applied customer discounts from Customer change");

                        if (SL.FuelRebateEligible && SL.FuelRebate > 0 && sale.Customer.UseFuelRebate && sale.Customer.UseFuelRebateDiscount)
                        {
                            _saleLineManager.ApplyFuelRebate(ref SL);
                        }
                        else
                        {



                            if (SL.ProductIsFuel && _policyManager.FuelLoyalty)
                            {
                                if (sale.Customer.GroupID != "")
                                {
                                    if (sale.Customer.DiscountType != "")
                                    {

                                        string temp_Policy_Name = "CL_DISCOUNTS";
                                        if (!_policyManager.GetPol(temp_Policy_Name, SL))
                                        {
                                            //VB.MsgBoxStyle temp_VbStyle = (int)VB.MsgBoxStyle.Critical + VB.MsgBoxStyle.OkOnly;
                                            //Chaps_Main.DisplayMessage(this, (short)81, temp_VbStyle, null, (byte)0);
                                            var offSet = _policyManager.LoadStoreInfo().OffSet;
                                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 81, null,
                                                CriticalOkMessageType);
                                        }
                                        else
                                        {

                                            //  Discountchart loyalty
                                            //same as $discount by litre- only difference is discount rate should be based on grade
                                            if (sale.Customer.DiscountType == "D")
                                            {
                                                _saleLineManager.ApplyFuelLoyalty(ref SL, sale.Customer.DiscountType, _saleLineManager.GetFuelDiscountChartRate(ref SL, sale.Customer.GroupID, SL.GradeID), sale.Customer.DiscountName); // this will bring the discount rate based on customer group id and fuel grade
                                            }
                                            else
                                            {
                                                // 
                                                _saleLineManager.ApplyFuelLoyalty(ref SL, sale.Customer.DiscountType, sale.Customer.DiscountRate, sale.Customer.DiscountName);
                                                WriteToLogFile("Apply FuelLoyalty from Customer change");
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    } //Shiny
                }
                _saleManager.ReCompute_Coupon(ref sale);
                //Chaps_Main.SA.ReCompute_Coupon(); //05/17/06 Nancy added for Fuel Loyalty of Coupon type
                _saleManager.ReCompute_Totals(ref sale);

                //Chaps_Main.SA.ReCompute_Totals();
                WriteToLogFile(" Finished Recompute from Customer change");
                _saleManager.SaveTemp(ref sale, tillNumber);
            }
            // Chaps_Main.SA.SaveTemp(); //  If taking customer after adding all items and if there is no discount system was not saving the customer info.
            WriteToLogFile("SaveTemp from Customer change");
            //if (Refund)
            //{
            //    Refresh_Lines(); //  Tookout refreshlines from customer_change( because it is causing screen flip & freezing in SITE screen)
            //}
        }


        /// <summary>
        /// Method to send request to TPS
        /// </summary>
        /// <param name="strRequest">Request</param>
        /// <param name="socket">Socket</param>
        /// <param name="cc">Credit card</param>
        private void SendToTPS(string strRequest, ref Socket socket, ref Credit_Card cc)
        {
            short retry = 0;
            var isWex = false;

            while (retry < 3) //From Table
            {
                try
                {
                    if (socket.Connected)
                    {
                        object sendStringSuf = "," + "END-DATA";
                        if (!(cc == null))
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
                            const byte endTransmit = (byte)(0x4);

                            strRequest = startHeader + Convert.ToString(sequenceNumber) + (strRequest.Length.ToString("0000") + strRequest); // For WEX TPS Specific
                            sendStringSuf = endTransmit;
                        }
                        WriteToLogFile("Send to STPS: " + strRequest + Convert.ToString(sendStringSuf));

                        var msg = Encoding.ASCII.GetBytes(strRequest + Convert.ToString(sendStringSuf));
                        socket.Send(msg);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Thread.Sleep(200);
                    retry++;
                }
            }
        }

        /// <summary>
        /// MEthod to get transaction result
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="cc">Credit card</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="sale">Sale</param>
        /// <param name="socket">Socket</param>
        /// <param name="customerFileStream">Customer file</param>
        /// <param name="errorMessage">Error</param>
        /// <param name="merchantFileStream">Merchant file</param>
        private void GetTransactionResult(ref Tenders tenders, ref Credit_Card cc, string tenderCode, ref Sale sale,
            ref Socket socket, ref Report merchantFileStream, ref Report customerFileStream, out ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();
            float processTimer;
            bool Mask_Card = _policyManager.MASK_CARDNO;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            //  - This shouldn't happen, but in case exit
            if (cc == null)
            {
                WriteToLogFile(" GetTransactionResult  with CC = Nothing");
                return;
            }
            //shiny end
            WriteToLogFile("Response property for cc card is " + cc.Response);
            switch (cc.Response.ToUpper())
            {
                case "APPROVED":
                    //Shiny Nov9, 2009 -EMVVERSION
                    if (Strings.UCase(Convert.ToString(_policyManager.BankSystem)) != "TD")
                    {
                        if (_policyManager.EMVVersion)
                        {
                            if (cc.Report.Length > 0)
                            {
                                if (_creditCardManager.Language(ref cc) == "French")
                                {
                                    //Receipts.PrintTransRecordFrench(cc, Mask_Card, true, false, true); //To Print Merchant copy

                                    merchantFileStream = _receiptManager.PrintTransRecordFrench(cc, Mask_Card, true, false, true); // To print Customer copy
                                    customerFileStream = _receiptManager.PrintTransRecordFrench(cc, Mask_Card, true, false, false); // To print Customer copy
                                }
                                else
                                {
                                    merchantFileStream = _receiptManager.PrintTransRecordEnglish(cc, Mask_Card, true, false, true); // To print Customer copy
                                    customerFileStream = _receiptManager.PrintTransRecordEnglish(cc, Mask_Card, true, false, false); // To print Customer copy
                                }
                            }
                        }
                        else
                        {
                            //End -EMV VERSION
                            customerFileStream = _creditCardManager.Language(ref cc) == "French" ? _receiptManager.PrintTransRecordFrench(cc, Mask_Card, true, false, false) : _receiptManager.PrintTransRecordEnglish(cc, Mask_Card, true, false, false);
                        }
                    }
                    //lblResponse.Text = modTPS.cc.Response;

                    break;
                //Call DisplayMessage(Me, 93, vbInformation + vbOKOnly, CC.Authorization_Number)
                case "NOTAPPROVED":
                case "NOTFOUND":
                    //lblResponse.Text = modTPS.cc.Receipt_Display;
                    //                tmrResponse.Enabled = True
                    DisplayBankMessage(cc, out errorMessage);
                    NotApproved(ref tenders, tenderCode, sale, ref cc, ref merchantFileStream, ref customerFileStream);
                    break;

                //Behrooz Nov-14
                case "CALLBANK":
                    if (cc.Crd_Type != "C")
                    {
                        //lblResponse.Text = modTPS.cc.Receipt_Display;
                        //                    tmrResponse.Enabled = True
                        DisplayBankMessage(cc, out errorMessage);
                        NotApproved(ref tenders, tenderCode, sale, ref cc, ref merchantFileStream, ref customerFileStream);
                        return;
                    }
                    cc.StoreAndForward = true;
                    Variables.STFDNumber = "";
                    //01/10/03,Nancy comment out the following checking
                    //because if we get CALLBANK,even the amount is less than FloorLimit
                    //we should display "Call..." message
                    //                If Abs(CC.Trans_Amount) > CC.FloorLimit Then
                    //Nancy took out Floor limit message, it should only display in TimeOut,01/27/03
                    //                     DisplayMsgForm "Purchase amount is above the floor limit. You have to call the bank for approval No.", 10
                    //                Ans = DisplayMsgForm("You have to call the bank for approval No.", 11)

                    /*ans = Chaps_Main.DisplayMsgForm(Chaps_Main.Resource.DisplayCaption((short)63, System.Convert.ToInt16(this.Tag), null, (short)0), (short)11, null, (byte)0, (byte)0, "", "", "", "");
                    //                 End If
                    if (ans == (int)VB.MsgBoxResult.Cancel)
                    {
                        //EMVVERSION-   send word Cancel if we didn't get callbank auth number if florlimit > 0
                        if (_policyManager.EMVVersion)
                        {
                            //MakeSafCancel();
                        }
                        //EMVVERSION- end

                        return;
                    }*/
                    switch (cc.Trans_Type.ToUpper())
                    {
                        case "SALEINSIDE":
                            cc.Trans_Type = "SAFSaleInside";
                            break;
                        case "REFUNDINSIDE":
                            cc.Trans_Type = "SAFRefundInside";
                            break;
                        case "VOIDINSIDE":
                            cc.Trans_Type = "SAFVoidInside";
                            break;
                    }
                    //EMVVERSION-   send word Cancel if we didn't get callbank auth number if florlimit > 0
                    if (_policyManager.EMVVersion && string.IsNullOrEmpty(Variables.STFDNumber))
                    {
                        Variables.STFDNumber = "CANCEL";
                    }
                    //EMVVERSION- end

                    SendToTPS(_cardManager.GetRequestString(ref cc, sale, cc.Trans_Type, "Credit", cc.Trans_Amount, Variables.STFDNumber), ref socket, ref cc);
                    //lblResponse.Text = Chaps_Main.Resource.DisplayCaption(System.Convert.ToInt16(lblResponse.Tag), System.Convert.ToInt16(this.Tag), null, (short)3); //"Processing. . . . . ."
                    //lblResponse.Refresh();
                    processTimer = (float)DateAndTime.Timer;
                    cc.Response = "";
                    while ((DateAndTime.Timer - processTimer) < _policyManager.BankTimeOutSec) // 2013 10 17 - Reji Added polity for bank transaction timeout in seconds
                    {
                        if (cc.Response.Length > 0)
                        {
                            //Me.lblResponse.Caption = CC.response
                            break;
                        }
                        //Variables.Sleep(100); 
                        //System.Windows.Forms.Application.DoEvents();
                        if (DateAndTime.Timer < processTimer)
                        {
                            processTimer = (float)DateAndTime.Timer;
                        }
                    }
                    Variables.STFDNumber = "";
                    //  - over floor limit SAF transaction not setting the tender code. So tender name is empty and creating problem at Till_Renamed close
                    if (_policyManager.EMVVersion && cc.Response == "APPROVED") //EMVVERSION
                    {
                        //SetEMVTender(); // double make sure it is posting to correct tender
                    }
                    // 

                    if (cc.Response.ToUpper() == "APPROVED")
                    {
                        var tender = GetSelectedTender(tenders, tenderCode);
                        tender.Credit_Card = cc;
                        //Shiny Nov9, 2009 -EMVVERSION
                        if (Strings.UCase(Convert.ToString(_policyManager.BankSystem)) != "TD")
                        {
                            if (_policyManager.EMVVersion)
                            {
                                if (cc.Report.Length > 0)
                                {
                                    if (_creditCardManager.Language(ref cc) == "French")
                                    {
                                        merchantFileStream = _receiptManager.PrintTransRecordFrench(cc, Mask_Card, true, false, true); // To print Customer copy
                                        customerFileStream = _receiptManager.PrintTransRecordFrench(cc, Mask_Card, true, false, false); // To print Customer copy
                                    }
                                    else
                                    {
                                        merchantFileStream = _receiptManager.PrintTransRecordEnglish(cc, Mask_Card, true, false, true); // To print Customer copy
                                        customerFileStream = _receiptManager.PrintTransRecordEnglish(cc, Mask_Card, true, false, false); // To print Customer copy
                                    }
                                }
                            }
                            else
                            {
                                //End -EMV VERSION

                                customerFileStream = _creditCardManager.Language(ref cc) == "French" ? _receiptManager.PrintTransRecordFrench(cc, Mask_Card, true, false, false) : _receiptManager.PrintTransRecordEnglish(cc, Mask_Card, true, false, false);
                            }
                        }
                        //lblResponse.Text = modTPS.cc.Response;
                        //                    tmrResponse.Enabled = True
                        DisplayBankMessage(cc, out errorMessage);
                    }
                    else
                    {
                        //lblResponse.Text = modTPS.cc.Receipt_Display;
                        //                    tmrResponse.Enabled = True
                        DisplayBankMessage(cc, out errorMessage);
                        NotApproved(ref tenders, tenderCode, sale, ref cc, ref merchantFileStream, ref customerFileStream);
                    }
                    break;


                case "TIMEOUT":
                    //Behrooz Nov-14
                    if (modTPS.cc.Crd_Type == "C")
                    {

                        cc.StoreAndForward = true;
                        Variables.STFDNumber = "";
                        if (_policyManager.Support_SAF == false && cc.Trans_Type.ToUpper() == "SALEINSIDE") // Not at all allow SAF
                        {
                            cc.Response = _resourceManager.GetResString(offSet, (short)1444); //"Transaction Timed Out."
                                                                                              //Chaps_Main.DisplayMsgForm(cc.Response, (short)99, null, (byte)0, (byte)0, "", "", "", "");
                            errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(offSet, cc.Response, (short)99, null);
                            //fmeCCNum.Visible = false;
                            //cmdCancelMag_Click(cmdCancelMag, null); // ControlsEnabled True
                            return;

                        }
                        if (_policyManager.Support_SAF && cc.Trans_Type.ToUpper() == "SALEINSIDE")
                        {
                            if (_policyManager.CallBankOnly) // only allow callthebank
                            {
                                /*  //                                Ans = DisplayMsgForm("You have to call the bank for approval No.", 11)
                                  ans = 
                                  .DisplayMsgForm(Chaps_Main.GetResString((short)1443), (short)11, null, (byte)0, (byte)0, "", "", "", "");

                                  if (ans == (int)VB.MsgBoxResult.Cancel)
                                  {
                                      if (Chaps_Main.EMVProcess)
                                      {
                                          //MakeSafCancel();
                                      }
                                      //EMVVERSION- end
                                      //cmdCancelMag_Click(cmdCancelMag, null);
                                      return;
                                  }
                                  */
                            }
                            else if (!_policyManager.CallBankOnly) // Allow SAF if < floor limit, if above floor limit do only call bank authorization
                            {
                                /*
                                if (modTPS.cc.Trans_Amount > modTPS.cc.FloorLimit && modTPS.cc.Trans_Type.ToUpper() == "SALEINSIDE")
                                {
                                    // 
                                    // 
                                    //                                   Ans = DisplayMsgForm("Purchase amount is above the floor limit. You have to call the bank for approval No.", 11)
                                    ans = Chaps_Main.DisplayMsgForm(Chaps_Main.Resource.DisplayCaption((short)63, System.Convert.ToInt16(this.Tag), null, (short)0), (short)11, null, (byte)0, (byte)0, "", "", "", "");
                                    if (ans == (int)VB.MsgBoxResult.Cancel)
                                    {
                                        //EMVVERSION-   send word Cancel if we didn't get callbank auth number if florlimit > 0
                                        if (_policyManager.EMVVersion)
                                        {
                                            //MakeSafCancel();
                                        }
                                        //EMVVERSION- end

                                        return;
                                    }
                                }
                                */
                            }
                            //  as part of Datawire integration('Added by dmitry)
                            //#3:POS doesn’t have to approve a manual credit transaction with amount under floor limit in case of lack of connection.
                            //Discussed with PM, for sales transaction if manual entry track2 will be empty. It will come here if floor <0 and SALEINSIDE- Void need a swipe, so it won't come here
                        }
                        else if (cc.Track2 == "") //added by Dmitry to prevent manual SAF for less than floor limit
                        {
                            cc.Response = "CANNOT BE PROCESSED. THE CARD SHOULD BE SWIPED.";
                            //Chaps_Main.DisplayMsgForm(cc.Response, (short)99, null, (byte)0, (byte)0, "", "", "", "");
                            errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(offSet, cc.Response, (short)99, null);
                            return;
                        }
                        // 
                        switch (cc.Trans_Type.ToUpper())
                        {
                            case "SALEINSIDE":
                                cc.Trans_Type = "SAFSaleInside";
                                break;
                            case "REFUNDINSIDE":
                                cc.Trans_Type = "SAFRefundInside";
                                break;
                            case "VOIDINSIDE":
                                cc.Trans_Type = "SAFVoidInside";
                                break;
                        }
                        //EMVVERSION-   send word Cancel if we didn't get callbank auth number if florlimit > 0
                        if (_policyManager.EMVVersion && string.IsNullOrEmpty(Variables.STFDNumber))
                        {
                            Variables.STFDNumber = "CANCEL";
                        }
                        //EMVVERSION- end

                        SendToTPS(_cardManager.GetRequestString(ref cc, sale, cc.Trans_Type, "Credit", cc.Trans_Amount, Variables.STFDNumber), ref socket, ref cc);
                        //lblResponse.Text = Chaps_Main.Resource.DisplayCaption(System.Convert.ToInt16(lblResponse.Tag), System.Convert.ToInt16(this.Tag), null, (short)3); //"Processing. . . . . ."
                        //lblResponse.Refresh();
                        processTimer = (float)DateAndTime.Timer;
                        cc.Response = "";
                        while ((DateAndTime.Timer - processTimer) < _policyManager.BankTimeOutSec) // 2013 10 17 - Reji Added polity for bank transaction timeout in seconds
                        {
                            if (cc.Response.Length > 0)
                            {
                                //Me.lblResponse.Caption = CC.response
                                break;
                            }
                            //Variables.Sleep(100); 
                            //System.Windows.Forms.Application.DoEvents();
                            if (DateAndTime.Timer < processTimer)
                            {
                                processTimer = (float)DateAndTime.Timer;
                            }
                        }
                        Variables.STFDNumber = "";
                        //  - over floor limit SAF transaction not setting the tender code. So tender name is empty and creating problem at Till_Renamed close
                        if (_policyManager.EMVVersion && cc.Response == "APPROVED") //EMVVERSION
                        {
                            //SetEMVTender(); // double make sure it is posting to correct tender
                        }
                        // 

                        if (cc.Response.ToUpper() == "APPROVED")
                        {
                            var td = GetSelectedTender(tenders, tenderCode);
                            td.Credit_Card = cc;
                            //Shiny Nov9, 2009 -EMVVERSION
                            if (Strings.UCase(Convert.ToString(_policyManager.BankSystem)) != "TD")
                            {
                                if (_policyManager.EMVVersion)
                                {
                                    if (cc.Report.Length > 0)
                                    {
                                        if (_creditCardManager.Language(ref cc) == "French")
                                        {
                                            merchantFileStream = _receiptManager.PrintTransRecordFrench(cc, Mask_Card, true, false, true); // To print Customer copy
                                            customerFileStream = _receiptManager.PrintTransRecordFrench(cc, Mask_Card, true, false, false); // To print Customer copy
                                        }
                                        else
                                        {
                                            merchantFileStream = _receiptManager.PrintTransRecordEnglish(cc, Mask_Card, true, false, true); // To print Customer copy
                                            customerFileStream = _receiptManager.PrintTransRecordEnglish(cc, Mask_Card, true, false, false); // To print Customer copy
                                        }
                                    }
                                }
                                else
                                {
                                    //End -EMV VERSION

                                    if (_creditCardManager.Language(ref cc) == "French")
                                    {
                                        customerFileStream = _receiptManager.PrintTransRecordFrench(cc, Mask_Card, true, false, false); // To print Customer copy
                                    }
                                    else
                                    {
                                        customerFileStream = _receiptManager.PrintTransRecordEnglish(cc, Mask_Card, true, false, false); // To print Customer copy
                                    }
                                }
                            }
                            //lblResponse.Text = modTPS.cc.Response;
                            //                       tmrResponse.Enabled = True
                            DisplayBankMessage(cc, out errorMessage);
                        }
                        else
                        {
                            //lblResponse.Text = modTPS.cc.Receipt_Display;
                            //                       tmrResponse.Enabled = True
                            DisplayBankMessage(cc, out errorMessage);
                            NotApproved(ref tenders, tenderCode, sale, ref cc, ref merchantFileStream, ref customerFileStream);
                        }
                    }
                    else
                    {
                        DisplayBankMessage(cc, out errorMessage);
                        NotApproved(ref tenders, tenderCode, sale, ref cc, ref merchantFileStream, ref customerFileStream);
                    }
                    break;

                case "SWIPED":
                    //i1 = (short)(cc.Track2.IndexOf("|") + 1);
                    //if (i1 != 0)
                    //{
                    //    CustomerName = Strings.Right(cc.Track2, cc.Track2.Length - i1);
                    //}
                    break;





                case "NOTCOMPLETED":

                    //VB.MsgBoxStyle temp_VbStyle = (int)VB.MsgBoxStyle.Information + VB.MsgBoxStyle.OkOnly;
                    //Chaps_Main.DisplayMessage(this, (short)49, temp_VbStyle, null, (byte)0);

                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)49, null);
                    //  EMVVERSION-LAST - Screen freezing issue at POS, which luba reported- reason is crd_type is empty and we are not handling that situation
                    if (_policyManager.EMVVersion)
                    {
                        switch (cc.Crd_Type)
                        {
                            case "":
                                //lblResponse.Text = Chaps_Main.Resource.DisplayCaption(System.Convert.ToInt16(lblResponse.Tag), System.Convert.ToInt16(this.Tag), null, (short)4); //"Time out"
                                break;
                            case "D":
                            case "C":
                                cc.Trans_Date = DateTime.Parse(DateAndTime.Today.ToString("MM/dd/yy"));
                                cc.Trans_Time = DateTime.Parse(DateAndTime.TimeOfDay.ToString("hh:mm:ss"));
                                if (_creditCardManager.Language(ref cc) == "French")
                                {
                                    cc.Receipt_Display = "OPERATION NON COMPLETEE";
                                }
                                else
                                {
                                    cc.Receipt_Display = "TRANSACTION NOT COMPLETED";
                                }
                                NotApproved(ref tenders, tenderCode, sale, ref cc, ref merchantFileStream, ref customerFileStream);
                                break;
                        }
                    }
                    else
                    {
                        //shiny end- EMVVERSION
                        if (cc.Crd_Type == "C")
                        {
                            //lblResponse.Text = Chaps_Main.Resource.DisplayCaption(System.Convert.ToInt16(lblResponse.Tag), System.Convert.ToInt16(this.Tag), null, (short)4); //"Time out"
                        }
                        else
                        {
                            cc.Trans_Date = DateTime.Parse(DateAndTime.Today.ToString("mm/dd/yy"));
                            cc.Trans_Time = DateTime.Parse(DateAndTime.TimeOfDay.ToString("hh:mm:ss"));
                            cc.Receipt_Display = _creditCardManager.Language(ref cc) == "French" ? "OPERATION NON COMPLETEE" : "TRANSACTION NOT COMPLETED";
                            NotApproved(ref tenders, tenderCode, sale, ref cc, ref merchantFileStream, ref customerFileStream);
                        }
                    }
                    break;
                default:
                    DisplayBankMessage(cc, out errorMessage);
                    NotApproved(ref tenders, tenderCode, sale, ref cc, ref merchantFileStream, ref customerFileStream);
                    break;
            }
        }

        /// <summary>
        /// Method to create receipt in case of not approved 
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="sale">Sale</param>
        /// <param name="cc">Credit card</param>
        /// <param name="merchanFileStream">Merchant file</param>
        /// <param name="customerFileStream">Customer file</param>
        private void NotApproved(ref Tenders tenders, string tenderCode, Sale sale, ref Credit_Card cc,
            ref Report merchanFileStream, ref Report customerFileStream)
        {
            bool Mask_Card = _policyManager.MASK_CARDNO;
            tenders.Tend_Totals.Change = sale.Sale_Totals.Gross - Math.Abs(tenders.Tend_Totals.Tend_Used);

            if (cc.Crd_Type == "D" || (cc.Crd_Type == "C" && _policyManager.EMVVersion)) //EMVVERSION -Shiny added the 2nd"Credit card condition for  Luba on jan14 , 2009 - Need to print hip credit card decliend  receipt
            {
                var td = GetSelectedTender(tenders, tenderCode);
                td.Credit_Card = cc;
                //Shiny Nov9, 2009 -EMVVERSION
                if (Strings.UCase(Convert.ToString(_policyManager.BankSystem)) == "TD") return;
                if (_policyManager.EMVVersion)
                {
                    if (string.IsNullOrEmpty(cc.Report)) return;
                    if (_creditCardManager.Language(ref cc) == "French")
                    {
                        merchanFileStream = _receiptManager.PrintTransRecordFrench(cc, Mask_Card, true, false, true); //To Print Merchant copy
                        customerFileStream = _receiptManager.PrintTransRecordFrench(cc, Mask_Card, true, false, false); // To print Customer copy
                    }
                    else
                    {
                        merchanFileStream = _receiptManager.PrintTransRecordEnglish(cc, Mask_Card, true, false, true); //To Print Merchant copy
                        customerFileStream = _receiptManager.PrintTransRecordEnglish(cc, Mask_Card, true, false, false); // To print Customer copy
                    }
                }
                else
                {
                    //End -EMV VERSION

                    customerFileStream = _creditCardManager.Language(ref cc) == "French" ? _receiptManager.PrintTransRecordFrench(cc, Mask_Card, true, false, false) : _receiptManager.PrintTransRecordEnglish(cc, Mask_Card, true, false, false);
                }
            }
        }

        /// <summary>
        /// Method to display bank message
        /// </summary>
        /// <param name="cc">Credit card</param>
        /// <param name="errorMessage">Error</param>
        private void DisplayBankMessage(Credit_Card cc, out ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (_policyManager.EMVVersion) //EMVVERSION
            {
                errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(offSet, !string.IsNullOrEmpty(cc.BankMessage) ? cc.BankMessage : cc.Response, 99, null);
            }
            else
            {
                errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(offSet, cc.Report.Length > 0 ? cc.Report : cc.Response, 99, null);
            }
        }

        /// <summary>
        /// Method to get response
        /// </summary>
        /// <param name="strResponse">Response</param>
        /// <param name="cc">Credit card</param>
        private void GetResponse(string strResponse, ref Credit_Card cc)
        {
            string strDate = "";
            string strTime = "";
            string strSeq = "";

            WriteToLogFile("GetResponse procedure response is " + cc.Response);

            cc.Response = GetStrPosition(strResponse, 15).Trim().ToUpper();
            if (_policyManager.EMVVersion) //EMVVERSION 'Added May4,2010
            {
                cc.Card_Swiped = cc.ManualCardProcess == false;
            }
            cc.Result = GetStrPosition(strResponse, 16).Trim();
            cc.Authorization_Number = GetStrPosition(strResponse, 17).Trim().ToUpper();
            cc.ResponseCode = GetStrPosition(strResponse, 29).Trim().ToUpper();
            //  EMVVERSION
            if (_policyManager.EMVVersion) //EMVVERSION
            {
                cc.Crd_Type = GetStrPosition(strResponse, 2).Trim().Substring(0, 1);
                // _creditCardManager.SetTrack2(ref cc, GetStrPosition(strResponse, (short)12).Trim().ToUpper());
                // cc.Swipe_String = cc.Track2;
            }
            //shiny end-EMVVERSION



            strSeq = GetStrPosition(strResponse, (short)5).Trim();
            if (_policyManager.BankSystem != "Moneris")
            {
                cc.Sequence_Number = string.IsNullOrEmpty(strSeq) ? "" : strSeq.Substring(0, strSeq.Length - 1);
            }
            else //Moneris
            {
                cc.Sequence_Number = strSeq;
            }


            cc.TerminalID = GetStrPosition(strResponse, (short)8).Trim();
            cc.DebitAccount = GetStrPosition(strResponse, (short)11).Trim();
            strDate = GetStrPosition(strResponse, (short)21).Trim();
            //Nancy changed,10/21/02
            if (string.IsNullOrEmpty(strDate))
            {
                cc.Trans_Date = DateTime.Today;
            }
            else
            {
                DateTime date;
                cc.Trans_Date = DateTime.TryParseExact(strDate, "MM/dd/yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out date) ? date : DateTime.Today;
            }
            strTime = GetStrPosition(strResponse, (short)22).Trim();

            if (string.IsNullOrEmpty(strTime))
            {
                cc.Trans_Time = DateTime.Now;

            }
            else
            {
                DateTime time;
                cc.Trans_Time = DateTime.TryParseExact(strTime, "hh:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out time) ? new DateTime(1899, 12, 30, time.Hour, time.Minute, time.Second) : DateTime.Now;
            }
            //    cc.Trans_Date = Trim(GetStrPosition(strResponse, 21))
            //    cc.Trans_Time = Trim(GetStrPosition(strResponse, 22))
            cc.ApprovalCode = GetStrPosition(strResponse, (short)18).Trim();
            cc.Receipt_Display = GetStrPosition(strResponse, (short)23).Trim();

            //    cc.Report = Trim(GetStrPosition(strResponse, 30))
            if (_policyManager.EMVVersion) //EMVVERSION
            {
                cc.Report = GetStrPosition(strResponse, (short)31).Trim();
                cc.BankMessage = GetStrPosition(strResponse, (short)30).Trim();
            }
            else
            {
                cc.Report = GetStrPosition(strResponse, (short)30).Trim();
            }

            // Nicolette added next lines
            if (cc.AskVechicle)
            {
                _creditCardManager.SetVehicleNumber(ref cc, GetStrPosition(strResponse, (short)33).Trim());
            }
            if (cc.AskIdentificationNo)
            {
                _creditCardManager.SetIdNumber(ref cc, GetStrPosition(strResponse, (short)34).Trim());
            }
            if (cc.AskDriverNo)
            {
                _creditCardManager.SetDriverNumber(ref cc, GetStrPosition(strResponse, (short)34).Trim());
            }
            if (cc.AskOdometer)
            {
                _creditCardManager.SetOdoMeter(ref cc, GetStrPosition(strResponse, (short)35).Trim());
            }
            // Nicolette end
            //Nancy add for PinPad






            //if (!_policyManager.EMVVersion) //  this is for pinpad swipe
            //{
            //    if (cc.Track2 == "" && GetStrPosition(strResponse, (short)12) != "")
            //    {
            //        //        12/20/06 end
            //        _creditCardManager.SetTrack2(ref cc, GetStrPosition(strResponse, (short)12).Trim());
            //    }
            //}
            //
            //to set Language again, Nov.20th,2002, Nancy
            //    cc.language = Trim(GetStrPosition(strResponse, 10))

            //    cc.Swipe_String = cc.Track2
            _creditCardManager.SetIdNumber(ref cc, GetStrPosition(strResponse, (short)3).Trim());
            //  -even if manual entry , it was changing that to swiped, actually stps is returning whether swiped or not
            //    cc.Ca  rd_Swiped = True
            if (!_policyManager.EMVVersion) //EMVVERSION ' 
            {
                if (GetStrPosition(strResponse, (short)1).Trim().ToUpper() == "SWIPEINSIDE")
                {
                    cc.Card_Swiped = (GetStrPosition(strResponse, (short)15).Trim().ToUpper() == "SWIPED") ? true : false;
                }
            }
            //SHINY END
            // EMVVERSION
            if (_policyManager.EMVVersion) // 31 position is card name
            {
                //  because luba changed to 33 and response is giving cardname at 33 position - Shecan't remember
                cc.Name = GetStrPosition(strResponse, (short)33).Trim(); // Trim(GetStrPosition(strResponse, 32))
            }
        }

        /// <summary>
        /// Method to get string position
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="lo"></param>
        /// <returns>Position</returns>
        private string GetStrPosition(string str, short lo)
        {
            string returnValue = "";
            short i = 0;
            byte C;
            short j;
            string strT = "";
            short intTemp = 0;
            string strTemp = "";

            strT = str;
            returnValue = "";
            i = (short)0;
            if (!string.IsNullOrEmpty(str))
            {
                while (i < lo)
                {
                    intTemp = (short)(strT.IndexOf(",", StringComparison.Ordinal) + 1);
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
        /// Method to verify account
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="userCode">User code</param>
        /// <param name="amountEntered">Amount entered</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>VErify payment by account</returns>
        private VerifyPaymentByAccount VerifyAccount(Sale sale, string userCode,
            string amountEntered, out ErrorMessage errorMessage)
        {
            ErrorMessage error = new ErrorMessage();
            Credit_Card cc = new Credit_Card();
            CardSummary cardSummary = null;
            if (!string.IsNullOrEmpty(sale.Customer.LoyaltyCard))
            {
                var data = System.Text.Encoding.UTF8.GetBytes(sale.Customer.LoyaltyCard);
                cardSummary = _tenderManager.FindCardTender(Convert.ToBase64String(data), sale.Sale_Num,
                   sale.TillNumber, "Sale", userCode, out error, ref cc, true, true);
            }

            var verifyAccount = new VerifyPaymentByAccount
            {
                IsPurchaseOrderRequired = sale.Customer.UsePO,
                UseMultiPO = sale.Customer.MultiUse_PO,
                CardSummary = cardSummary
            };
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            errorMessage = new ErrorMessage();
            object[] capValue = new object[3];
            var newBalance = (decimal)(sale.Customer.Current_Balance + Conversion.Val(amountEntered));
            var strAction = _resourceManager.GetResString(offSet, (short)351);
            if (newBalance > Convert.ToDecimal(sale.Customer.Credit_Limit)) //And SA.Customer.Credit_Limit > 0 Then
            {
                //PopupCreditmessage ' 
                if (_policyManager.CreditMsg)
                {
                    if (!string.IsNullOrEmpty(sale.Customer.CL_Note))
                    {
                        //errorMessage.StatusCode = HttpStatusCode.BadRequest;
                        //errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet,0, (short)8406, sale.Customer.CL_Note, MessageType.OkOnly);
                        verifyAccount.CreditMessage = _resourceManager.CreateMessage(offSet, 0, (short)8406, sale.Customer.CL_Note, MessageType.OkOnly).Message;
                    }
                }
                //shiny end

                // Nicolette replaced next line, moving here the code from inside AuthorizedUser class
                // Refering a form inside a class that can be a public object
                // resulted in compile error when we tried to make the dll for POS classes.
                // After this issue was discussed with PM, we decided to change the code
                // and use User class in the project, June 17,2005
                // If AuthUser.GetAuthorizedUser("U_OR_LIMIT", strAction) Then

                var currentUser = _loginManager.GetExistingUser(userCode);
                if (!_policyManager.GetPol("U_OR_LIMIT", currentUser))
                {
                    // Ans = "That user is not authorized to " & Action & vbCrLf & _'                                      "Select an authorized user?", vbQuestion + vbYesNo, _

                    MessageType temp_VbStyle17 = (int)MessageType.Question + MessageType.YesNo;
                    verifyAccount.UnauthorizedMessage = _resourceManager.CreateMessage(offSet, 0, (short)8193, strAction, temp_VbStyle17).Message;
                    //errorMessage.StatusCode = HttpStatusCode.Forbidden;
                    //return null;
                }
                // end changes on June 17, 2005

                if (!_policyManager.GetPol("U_OR_LIMIT", currentUser)) return verifyAccount;
                capValue[1] = sale.Customer.Credit_Limit.ToString("#,###.00");
                capValue[2] = sale.Customer.Current_Balance.ToString("#,###.00");
                MessageType temp_VbStyle18 = (int)MessageType.Critical + MessageType.YesNo;
                //errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet,14, (short)79, CapValue, temp_VbStyle18);
                verifyAccount.OverrideARLimitMessage = _resourceManager.CreateMessage(offSet, 14, (short)79, capValue, temp_VbStyle18).Message;
            }
            return verifyAccount;
        }

        /// <summary>
        /// Method to get selcted tender
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="tenderCode">Tender code</param>
        /// <returns>Tender</returns>
        private Tender GetSelectedTender(Tenders tenders, string tenderCode)
        {
            var td = tenders.FirstOrDefault(t => t.Tender_Code == tenderCode);
            if (td != null) return td;
            {
                td = tenders.FirstOrDefault(t => t.Tender_Class == tenderCode && (t.Tender_Class == "CRCARD" || t.Tender_Class == "FLEET"));
                if (_policyManager.COMBINECR || _policyManager.COMBINEFLEET)
                    return td;
                return null;
            }
        }

        /// <summary>
        /// Method to create exact change without receipt
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="user">User</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="errorMessage">Error</param>
        /// <param name="signature">Signature</param>
        /// <param name="fileName">File name</param>
        /// <returns>Report content</returns>
        private Report ExactChange_NoReceipt(Sale sale, User user, int tillNumber,
            out ErrorMessage errorMessage, out Stream signature, ref string fileName)
        {
            Report receipt;
            errorMessage = new ErrorMessage();
            if (sale.Sale_Type == "MARKDOWN")
            {
                Tenders nullTenders = null;
                //var fileName = string.Empty;
                var rePrint = false;
                receipt = _receiptManager.Print_Receipt(tillNumber, null, ref sale, ref nullTenders, false, ref fileName, ref rePrint, out signature, user.Code);
                _saleManager.SaveSale(sale, user.Code, ref nullTenders, null);
                _saleManager.Clear_Sale(sale, sale.Sale_Num, tillNumber, user.Code, "Initial", null, false, true, false, out errorMessage);
            }
            else
            {
                var tendersRenamed = Exact_Change(sale, tillNumber, out errorMessage);
                //var fileName = string.Empty;
                var rePrint = false;
                receipt = _receiptManager.Print_Receipt(tillNumber, null, ref sale, ref tendersRenamed, false, ref fileName, ref rePrint, out signature, user.Code);
                if (sale.Sale_Totals.Gross < 0)
                {
                    _saleManager.Clear_Sale(sale, sale.Sale_Num, tillNumber, user.Code, "REFUND", tendersRenamed, false, true, false, out errorMessage);
                }
                else
                {
                    _saleManager.Clear_Sale(sale, sale.Sale_Num, tillNumber, user.Code, "SALE", tendersRenamed, false, true, false, out errorMessage);
                }
            }
            return receipt;
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
        #region SaleVendor Coupon



        /// <summary>
        /// Method to remove sale vendor coupon
        /// </summary>
        /// <param name="allTenders">Tenders</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="saleNumber">Sale vendor</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="grossTotal">Gross total</param>
        /// <param name="tendDesc">Tender description</param>
        /// <param name="couponId">Coupon ID</param>
        /// <param name="itemNum">Item number</param>
        /// <returns>Sale vendor coupons</returns>
        private List<VCoupon> RemoveVendorCouponForTender(ref Tenders allTenders, string tenderCode, SaleVendorCoupon svc,
          int saleNumber, int tillNumber, decimal grossTotal, string tendDesc, string couponId, short itemNum)
        {
            SaleVendorCouponLine svcLine = default(SaleVendorCouponLine);
            float reduceAmount = 0;
            List<VCoupon> vCoupons = new List<VCoupon>();
            reduceAmount = 0;
            var selectedTender = GetSelectedTender(allTenders, tenderCode);

            if (string.IsNullOrEmpty(couponId))
            {

                foreach (SaleVendorCouponLine tempLoopVarSvcLine in svc.SVC_Lines)
                {
                    svcLine = tempLoopVarSvcLine;
                    if (svcLine.TendDesc.Trim() == tendDesc.Trim())
                    {
                        reduceAmount = reduceAmount + (float)svcLine.TotalValue;
                        _svcManager.Remove_a_Line(ref svc, tillNumber, svcLine.SeqNum);
                    }
                }
                _tenderService.DeleteSaleVendorCoupon(saleNumber, tillNumber, tendDesc);
            }
            else
            {
                if (itemNum == 0)
                {

                    foreach (SaleVendorCouponLine tempLoopVarSvcLine in svc.SVC_Lines)
                    {
                        svcLine = tempLoopVarSvcLine;
                        if (svcLine.TendDesc.Trim() == tendDesc.Trim() && svcLine.CouponCode.Trim() == couponId.Trim())
                        {

                            reduceAmount = reduceAmount + (float)svcLine.TotalValue;
                            _svcManager.Remove_a_Line(ref svc, tillNumber, svcLine.SeqNum);
                        }
                    }
                    _tenderService.DeleteSaleVendorCouponByCouponId(saleNumber, tillNumber, tendDesc, couponId.Trim());
                }
                else
                {

                    foreach (SaleVendorCouponLine tempLoopVarSvcLine in svc.SVC_Lines)
                    {
                        svcLine = tempLoopVarSvcLine;
                        if (svcLine.TendDesc.Trim() == tendDesc.Trim() && svcLine.ItemNum == itemNum)
                        {
                            _tenderService.RemoveOneCouponLine(saleNumber, tillNumber, tendDesc, svcLine.CouponCode.Trim(), svcLine.Line_Num, svcLine.SeqNum);

                            reduceAmount = reduceAmount + (float)svcLine.TotalValue;
                            _svcManager.Remove_a_Line(ref svc, tillNumber, svcLine.SeqNum);


                            vCoupons = RefreshSerialNumbers(svc, tendDesc);




                            break;
                        }
                    }
                }

            }

            //TendAmount = Convert.ToSingle(Convert.ToSingle(SVC.Amount) - ReduceAmount);

            //if (TendAmount == 0)
            //{
            //    selectedTender.Amount_Entered = 0;
            //}
            //else
            //{
            //    selectedTender.Amount_Entered = 0;
            //}
            selectedTender.Amount_Used = Convert.ToDecimal(svc.Amount);
            selectedTender.Amount_Entered = Convert.ToDecimal(svc.Amount);
            allTenders.Tend_Totals.Tend_Used = allTenders.Tend_Totals.Tend_Used - Convert.ToDecimal(reduceAmount);
            allTenders.Tend_Totals.Change = grossTotal - allTenders.Tend_Totals.Tend_Used;
            CacheManager.AddSaleVendorCoupon(svc.Sale_Num, selectedTender.Tender_Code, svc);
            CacheManager.AddTendersForSale(svc.Sale_Num, svc.Till_Num, allTenders);
            return vCoupons;
        }

        /// <summary>
        /// Method to refresh serial numbers
        /// </summary>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="tendDesc">Tender description</param>
        /// <returns>List of sale vendor coupons</returns>
        private List<VCoupon> RefreshSerialNumbers(SaleVendorCoupon svc,
            string tendDesc)
        {
            List<VCoupon> lstSerialNum = new List<VCoupon>();
            foreach (SaleVendorCouponLine tempLoopVarSvcLine in svc.SVC_Lines)
            {
                var svcLine = tempLoopVarSvcLine;
                if (svcLine.TendDesc == tendDesc)
                {
                    var newVCoupon = new VCoupon
                    {
                        Coupon = Convert.ToInt32(svcLine.SeqNum)

                    };
                    if (!string.IsNullOrEmpty(svcLine.SerialNumber))
                    {
                        newVCoupon.SerialNumber = svcLine.ItemNum + " - " + svcLine.SerialNumber;
                    }
                    else
                    {
                        newVCoupon.SerialNumber = svcLine.ItemNum.ToString();
                    }
                    lstSerialNum.Add(newVCoupon);
                }
            }
            return lstSerialNum;
        }

        /// <summary>
        /// Method to check if valid to add vendor coupon
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="till">Till</param>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="couponId">Coupon Id</param>
        /// <param name="tendDesc">Tender description</param>
        /// <param name="seqNum">Sequence number</param>
        /// <param name="lineNum">Line number</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        private bool ValidToAddVendorCoupon(Sale sale, Till till, SaleVendorCoupon svc,
            string couponId, string tendDesc, ref short seqNum, ref short lineNum,
            out ErrorMessage error)
        {
            error = new ErrorMessage();
            bool isValid = false;
            VendorCoupon vc = default(VendorCoupon);
            bool hasThisCoupon = false;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            isValid = false;
            hasThisCoupon = false;
            var vendorCoupons = _tenderService.GetAllVendorCoupon();

            foreach (VendorCoupon tempLoopVarVc in vendorCoupons)
            {
                vc = tempLoopVarVc;

                if (vc.Code == couponId && vc.TendDesc == tendDesc)
                {
                    hasThisCoupon = true;
                    break;
                }
            }

            if (hasThisCoupon)
            {

                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    var sl = tempLoopVarSl;

                    if (!string.IsNullOrEmpty(vc.StockCode) && vc.StockCode != _resourceManager.GetResString(offSet, (short)347)) // "<NONE>" Then
                    {
                        if (vc.StockCode == sl.Stock_Code)
                        {

                            if (ValidCouponForTheLine(till, svc, sale.Sale_Num, sl.Line_Num, sl.Quantity, ref seqNum))
                            {
                                lineNum = sl.Line_Num;
                                isValid = true;
                                break;
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(vc.SubDetail) && vc.SubDetail != _resourceManager.GetResString(offSet, (short)347)) // "<NONE>" Then
                    {
                        if (vc.SubDetail == sl.Sub_Detail && vc.SubDept == sl.Sub_Dept && vc.Dept == sl.Dept)
                        {

                            if (ValidCouponForTheLine(till, svc, sale.Sale_Num, sl.Line_Num, sl.Quantity, ref seqNum))
                            {
                                lineNum = sl.Line_Num;
                                isValid = true;
                                break;
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(vc.SubDept) && vc.SubDept != _resourceManager.GetResString(offSet, (short)347)) // "<NONE>" Then
                    {
                        if (vc.SubDept == sl.Sub_Dept && vc.Dept == sl.Dept)
                        {

                            if (ValidCouponForTheLine(till, svc, sale.Sale_Num, sl.Line_Num, sl.Quantity, ref seqNum))
                            {
                                lineNum = sl.Line_Num;
                                isValid = true;
                                break;
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(vc.Dept) && vc.Dept != _resourceManager.GetResString(offSet, (short)347)) // "<NONE>" Then
                    {
                        if (vc.Dept == sl.Dept)
                        {

                            if (ValidCouponForTheLine(till, svc, sale.Sale_Num, sl.Line_Num, sl.Quantity, ref seqNum))
                            {
                                lineNum = sl.Line_Num;
                                isValid = true;
                                break;
                            }
                        }



                    }
                    else if (vc.Dept == vc.SubDept && vc.Dept == vc.SubDetail && vc.Dept == vc.StockCode && (vc.Dept == _resourceManager.GetResString(offSet, (short)347) || vc.Dept == "")) // "<NONE>" Then
                    {




                        seqNum = GetCouponSeqNum(svc, (short)1);
                        lineNum = (short)1;
                        isValid = true;
                        break;
                    }
                }
            }
            else
            {

                MessageType temp_VbStyle = (int)MessageType.OkOnly + MessageType.Critical;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)59, couponId, temp_VbStyle);

                return false;
            }

            if (!isValid)
            {

                MessageType temp_VbStyle2 = (int)MessageType.OkOnly + MessageType.Critical;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)60, couponId, temp_VbStyle2);

                return false;
            }

            return true;
        }

        // If this coupon is valid for this sale, then change the current tender's UsedAmount, also refresh the SaleTend screen
        /// <summary>
        /// Method to add vendor coupon
        /// </summary>
        /// <param name="tendersRenamed">Tenders</param>
        /// <param name="tillRenamed">Till</param>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="grossTotal">Gross total</param>
        /// <param name="seqNum">Sequence number</param>
        /// <param name="lineNum">Line number</param>
        /// <param name="couponId">Coupon Id</param>
        /// <param name="tendDesc">Tender description</param>
        /// <param name="strSerialNumber">Serial number</param>
        /// <param name="allowChange">Allow change</param>
        /// <param name="itemNum">Item number</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        private bool AddVendorCoupon(ref Tenders tendersRenamed, Till tillRenamed,
            SaleVendorCoupon svc, int saleNumber, string tenderCode, decimal grossTotal,
            short seqNum, short lineNum, string couponId, string tendDesc,
            string strSerialNumber, bool allowChange, ref short itemNum, out ErrorMessage error)
        {
            error = new ErrorMessage();
            bool returnValue = false;
            SaleVendorCouponLine svcLine = default(SaleVendorCouponLine);
            float tendAmount = 0;
            VendorCoupon VC = default(VendorCoupon);
            bool hasThisCoupon = false;

            returnValue = false;
            if (svc.Sale_Num == 0)
            {
                svc.Sale_Num = saleNumber;
                svc.Till_Num = Convert.ToByte(tillRenamed.Number);
            }
            var vendorCoupons = _tenderService.GetAllVendorCoupon();

            hasThisCoupon = false;
            foreach (VendorCoupon tempLoopVarVc in vendorCoupons)
            {
                VC = tempLoopVarVc;

                if (VC.Code == couponId && VC.TendDesc == tendDesc)
                {
                    hasThisCoupon = true;
                    break;
                }
            }

            if (!hasThisCoupon)
            {
                return returnValue;
            }


            tendAmount = 0;
            foreach (SaleVendorCouponLine tempLoopVarSvcLine in svc.SVC_Lines)
            {
                svcLine = tempLoopVarSvcLine;
                if (svcLine.TendDesc == tendDesc)
                {
                    tendAmount = tendAmount + (float)svcLine.TotalValue;
                }
            }

            tendAmount = tendAmount + VC.Value;


            if (!allowChange && Math.Abs(tendAmount) > Math.Abs(Convert.ToDouble(grossTotal.ToString("0.00"))))
            {
                // If we don't give change on this tender then we can't accept more than
                // the amount of the sale.
                //Show_Message  "Amount Cannot Exceed value of sale."
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)88, null, MessageType.OkOnly);
                return returnValue;
            }


            svcLine = new SaleVendorCouponLine
            {
                Line_Num = lineNum,
                CouponCode = VC.Code,
                CouponName = VC.Name,
                Quantity = 1,
                UnitValue = VC.Value,
                TotalValue = (decimal)VC.Value,
                TendDesc = VC.TendDesc,
                SeqNum = seqNum,
                SerialNumber = strSerialNumber
            };


            _svcManager.Add_a_Line(ref svc, tillRenamed.Number, svcLine, true);
            itemNum = svcLine.ItemNum;
            var selectedTender = GetSelectedTender(tendersRenamed, tenderCode); ;
            if (tendAmount > 0)
            {
                selectedTender.Amount_Entered = Convert.ToDecimal(tendAmount);
                //lblUsed[Active_Line].Text = TendAmount.ToString("##0.00");
                // strCouponID = CouponID.Trim();
            }

            selectedTender.Amount_Used = Convert.ToDecimal(tendAmount);
            selectedTender.Amount_Entered = Convert.ToDecimal(tendAmount);
            tendersRenamed.Tend_Totals.Tend_Used = tendersRenamed.Tend_Totals.Tend_Used + Convert.ToDecimal(VC.Value);
            tendersRenamed.Tend_Totals.Change = grossTotal - tendersRenamed.Tend_Totals.Tend_Used;
            CacheManager.AddSaleVendorCoupon(svc.Sale_Num, selectedTender.Tender_Code, svc);
            CacheManager.AddTendersForSale(svc.Sale_Num, svc.Till_Num, tendersRenamed);
            returnValue = true;
            return returnValue;
        }

        /// <summary>
        /// Method to check if valid coupon for line
        /// </summary>
        /// <param name="tillRenamed">Till</param>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="saleNo">Sale number</param>
        /// <param name="lineNum">Line number</param>
        /// <param name="saleQuantity">Sale quantity</param>
        /// <param name="seqNum">Sequence number</param>
        /// <returns>True or false</returns>
        private bool ValidCouponForTheLine(Till tillRenamed, SaleVendorCoupon svc,
            int saleNo, short lineNum, float saleQuantity,
            ref short seqNum)
        {
            bool returnValue = false;





            bool isValidCoupon = false;
            bool alreadyUsedThisCoupon = false;
            short maxSeqNum = 0;
            float lineQuantity = 0;

            returnValue = false;
            if (saleQuantity == 0)
            {
                return returnValue;
            }

            isValidCoupon = false;
            alreadyUsedThisCoupon = false;


            if (svc.Sale_Num == 0)
            {
                isValidCoupon = true;
                svc.Sale_Num = saleNo;
                svc.Till_Num = Convert.ToByte(tillRenamed.Number);
            }
            else
            {

                maxSeqNum = (short)1;
                foreach (SaleVendorCouponLine tempLoopVarSvcLine in svc.SVC_Lines)
                {
                    var svcLine = tempLoopVarSvcLine;
                    if (svcLine.Line_Num == lineNum)
                    {
                        alreadyUsedThisCoupon = true;

                        lineQuantity = lineQuantity + svcLine.Quantity;


                        if (lineQuantity + 1 > saleQuantity)
                        {
                            isValidCoupon = false;
                        }
                        else
                        {
                            isValidCoupon = true;
                        }


                        if (svcLine.SeqNum > maxSeqNum)
                        {
                            maxSeqNum = svcLine.SeqNum;
                        }
                    }
                }
                if (!alreadyUsedThisCoupon)
                {

                    isValidCoupon = true;
                }
            }

            if (alreadyUsedThisCoupon)
            {
                seqNum = (short)(maxSeqNum + 1);
            }
            else
            {
                seqNum = 1;
            }
            returnValue = isValidCoupon;

            return returnValue;
        }






        /// <summary>
        /// Method to get coupon sequence number
        /// </summary>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="lineNum">Line number</param>
        /// <returns>Coupon sequence</returns>
        private short GetCouponSeqNum(SaleVendorCoupon svc, short lineNum)
        {
            short returnValue = 0;

            bool alreadyUsedThisCoupon = false;
            short maxSeqNum = 0;
            short seqNum = 0;

            maxSeqNum = (short)1;
            foreach (SaleVendorCouponLine tempLoopVarSvcLine in svc.SVC_Lines)
            {
                var svcLine = tempLoopVarSvcLine;
                if (svcLine.Line_Num == lineNum)
                {
                    alreadyUsedThisCoupon = true;


                    if (svcLine.SeqNum > maxSeqNum)
                    {
                        maxSeqNum = svcLine.SeqNum;
                    }
                }
            }

            if (alreadyUsedThisCoupon)
            {
                seqNum = (short)(maxSeqNum + 1);
            }
            else
            {
                seqNum = (short)1;
            }

            returnValue = seqNum;

            return returnValue;
        }

        /// <summary>
        /// Method to add serial number
        /// </summary>
        /// <param name="tendersRenamed">Tenders</param>
        /// <param name="strCoupon">Coupon</param>
        /// <param name="strSerial">Serial</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="svc">Sale vendor coupon</param>
        /// <param name="sale">Sale</param>
        /// <param name="till">Till</param>
        /// <param name="grossTotal">Gross total</param>
        /// <param name="error">Error</param>
        /// <returns>List of sale vendor coupon</returns>
        private List<VCoupon> cmdSerAdd_Click(ref Tenders tendersRenamed, string strCoupon, string strSerial
            , string tenderCode, SaleVendorCoupon svc, Sale sale, Till till, decimal grossTotal,
            out ErrorMessage error)
        {
            error = new ErrorMessage();
            short seqNum = 0;
            short lineNum = 0;
            string tendDesc = "";
            short itemNum = 0;
            short serLen = 0;
            var selectedTender = GetSelectedTender(tendersRenamed, tenderCode);
            tendDesc = selectedTender.Tender_Name;
            var lstSerialNum = RefreshSerialNumbers(svc, tendDesc); ;

            var vendorCoupons = _tenderService.GetAllVendorCoupon();

            serLen = (short)0;
            foreach (VendorCoupon tempLoopVarVc in vendorCoupons)
            {
                var vc = tempLoopVarVc;
                if (vc.Code == strCoupon && vc.TendDesc == tendDesc)
                {
                    serLen = vc.SerNumLen;
                    break;
                }
            }
            strSerial = string.IsNullOrEmpty(strSerial) ? string.Empty : strSerial;
            if (serLen > 0 && strSerial.Length != serLen)
            {

                var offSet = _policyManager.LoadStoreInfo().OffSet;
                MessageType temp_VbStyle = (int)MessageType.Critical + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)8885, serLen, temp_VbStyle);
                return null;
                //return;
            }

            if (ValidToAddVendorCoupon(sale, till, svc, strCoupon, tendDesc, ref seqNum,
                ref lineNum, out error))
            {
                if (AddVendorCoupon(ref tendersRenamed, till, svc, sale.Sale_Num,
                    tenderCode, grossTotal, seqNum, lineNum, strCoupon, tendDesc,
                    strSerial, selectedTender.Give_Change, ref itemNum, out error))
                {
                    if (!string.IsNullOrEmpty(strSerial))
                    {
                        lstSerialNum.Add(new VCoupon
                        {
                            Coupon = Convert.ToInt32(seqNum),
                            SerialNumber = itemNum + " - " + strSerial
                        });
                    }
                    else
                    {
                        lstSerialNum.Add(new VCoupon
                        {
                            Coupon = Convert.ToInt32(seqNum),
                            SerialNumber = itemNum.ToString()
                        });
                    }


                }
            }
            return lstSerialNum;
        }

        /// <summary>
        /// Method to get default coupon for tender code
        /// </summary>
        /// <param name="tenderCode">Tender code</param>
        /// <returns>Coupon</returns>
        private string GetDefaultCoupon(string tenderCode)
        {
            string returnValue = "";
            var vendorCoupons = _tenderService.GetAllVendorCoupon();
            returnValue = "";
            foreach (VendorCoupon tempLoopVarVc in vendorCoupons)
            {
                var vc = tempLoopVarVc;
                if (vc.TendDesc != tenderCode.Trim() || !vc.DefaultCoupon) continue;
                returnValue = vc.Code;
                break;
            }

            return returnValue;
        }

        #endregion
        #endregion

    }//end class
}//end namespace

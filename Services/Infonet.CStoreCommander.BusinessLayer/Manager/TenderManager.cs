using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class TenderManager : ManagerBase, ITenderManager
    {
        private readonly IPolicyManager _policyManager;
        private readonly ITenderService _tenderService;
        private readonly ISaleManager _saleManager;
        private readonly ILoginManager _loginManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly IReceiptManager _receiptManager;
        private readonly ISaleService _saleService;
        private readonly ITillService _tillService;
        private readonly ICardManager _cardManager;
        private readonly ICreditCardManager _creditCardManager;
        private readonly IGivexClientManager _givexClientManager;
        private readonly ICustomerManager _customerManager;
        private readonly ISaleLineManager _saleLineManager;
        private readonly ICustomerService _customerService;
        private readonly ISaleVendorCouponManager _svcManager;
        private readonly IUtilityService _utilityService;
        private readonly ITaxExemptSaleManager _taxExemptSaleManager;
        private readonly IPurchaseListManager _purchaseListManager;
        private readonly IEncryptDecryptUtilityManager _encryptDecryptUtilityManager;
        private readonly IMainManager _mainManager;
        private readonly ICarwashManager _carwashManager;
        private readonly IWexService _wexService;
        private readonly ICardPromptManager _cardPromptManager;
        private readonly ICardService _cardService;
        private readonly IWexManager _wexManager;
         
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="policyManager"></param>
        /// <param name="tenderService"></param>
        /// <param name="saleManager"></param>
        /// <param name="loginManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="receiptManager"></param>
        /// <param name="saleService"></param>
        /// <param name="tillService"></param>
        /// <param name="cardManager"></param>
        /// <param name="creditCardManager"></param>
        /// <param name="givexClientManager"></param>
        /// <param name="customerManager"></param>
        /// <param name="saleLineManager"></param>
        /// <param name="customerService"></param>
        /// <param name="svcManager"></param>
        /// <param name="utilityService"></param>
        /// <param name="taxExemptSaleManager"></param>
        /// <param name="purchaseListManager"></param>
        /// <param name="encryptDecryptUtilityManager"></param>
        /// <param name="mainManager"></param>
        public TenderManager(
            IPolicyManager policyManager,
            ITenderService tenderService,
            ISaleManager saleManager,
            ILoginManager loginManager,
            IApiResourceManager resourceManager,
            IReceiptManager receiptManager,
            ISaleService saleService,
            ITillService tillService,
            ICardManager cardManager,
            ICreditCardManager creditCardManager,
            IGivexClientManager givexClientManager,
            ICustomerManager customerManager,
            ISaleLineManager saleLineManager,
            ICustomerService customerService,
            ISaleVendorCouponManager svcManager,
            IUtilityService utilityService,
            ITaxExemptSaleManager taxExemptSaleManager,
            IPurchaseListManager purchaseListManager,
            IEncryptDecryptUtilityManager encryptDecryptUtilityManager,
            IMainManager mainManager,
            ICarwashManager carwashManager,
            IWexManager wexManager,
            IWexService wexService,
            ICardService cardService,
            ICardPromptManager cardPromptManager)
        {
            _policyManager = policyManager;
            _tenderService = tenderService;
            _saleManager = saleManager;
            _loginManager = loginManager;
            _resourceManager = resourceManager;
            _saleService = saleService;
            _receiptManager = receiptManager;
            _tillService = tillService;
            _cardManager = cardManager;
            _creditCardManager = creditCardManager;
            _givexClientManager = givexClientManager;
            _customerManager = customerManager;
            _saleLineManager = saleLineManager;
            _customerService = customerService;
            _svcManager = svcManager;
            _utilityService = utilityService;
            _taxExemptSaleManager = taxExemptSaleManager;
            _purchaseListManager = purchaseListManager;
            _encryptDecryptUtilityManager = encryptDecryptUtilityManager;
            _mainManager = mainManager;
            _carwashManager = carwashManager;
            _wexService = wexService;
            _cardPromptManager = cardPromptManager;
            _cardService = cardService;
            _wexManager = wexManager;
        }

        /// <summary>
        /// Set Amount Entered
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="sale">Sale</param>
        /// <param name="td">Tenders</param>
        /// <param name="vData">Amount</param>
        /// <param name="limit">Limit</param>
        public void Set_Amount_Entered(ref Tenders tenders, ref Sale sale, Tender td, decimal vData,
            decimal limit = 0)
        {
            if (vData != 0)
            {
                WriteToLogFile("Set_Amount_Entered method, Tenders class. Tender name is " + td.Tender_Name + ", amount is " + Convert.ToString(vData, CultureInfo.InvariantCulture));
            }
            //  , determine if penny adjustment is necessary
            if (_policyManager.PENNY_ADJ && vData != 0)
            {
                //   condition changed based on new requirement to have penny adj. for all tenders that can get change
                //        If UCase$(td.Tender_Name) = UCase$(Policy.BASECURR) And Not mvarBaseCurr_Used Then
                if (td.Give_Change && !tenders.BaseCurr_Used)
                {
                    tenders.BaseCurr_Used = true;
                }
                else
                {
                    if (!tenders.BaseCurr_Used)
                    {
                        tenders.BaseCurr_Used = false;
                    }
                }
                if (tenders.BaseCurr_Used && sale != null && Math.Round(vData, 2) != Math.Round(sale.Sale_Totals.Gross, 2))
                {

                    if (vData != 0 && (Math.Round(tenders.Tend_Totals.Tend_Used, 2) + vData + (decimal)0.03 > Math.Round(sale.Sale_Totals.Gross, 2)) &&
                        (Math.Round(tenders.Tend_Totals.Tend_Used, 2) + vData - Math.Round(sale.Sale_Totals.Gross, 2) !=
                         Conversion.Int(tenders.Tend_Totals.Tend_Used + vData - Math.Round(sale.Sale_Totals.Gross, 2))) &&
                        Math.Round(sale.Sale_Totals.Gross, 2) > 0)
                    {
                        tenders.Tend_Totals.PennyAdj_Required = true;
                    }
                    else if (sale.Sale_Type == "RETURN" || (sale.Sale_Totals.Gross < 0 & sale.Sale_Totals.Penny_Adj != 0))
                    {
                        tenders.Tend_Totals.PennyAdj_Required = false;
                    }
                    else if ((sale.Sale_Totals.Gross < 0 & sale.Sale_Totals.Penny_Adj == 0) && (Math.Abs(vData) != Math.Abs(sale.Sale_Totals.Gross)))
                    {
                        tenders.Tend_Totals.PennyAdj_Required = true;
                    }
                    else if (sale.Sale_Type == "ARPay" && CacheManager.GetArPayment(sale.Sale_Num) != null)
                    {
                        //   added (Me.Tend_Totals.Tend_Used + vData <> ARPay.Amount) to the next line to allow completion of ARPay with exact amount as per clients request
                        if ((tenders.Tend_Totals.Tend_Used + vData + (decimal)0.03 > CacheManager.GetArPayment(sale.Sale_Num).Amount) &&
                            (vData != CacheManager.GetArPayment(sale.Sale_Num).Amount))
                        {
                            tenders.Tend_Totals.PennyAdj_Required = true;
                        }
                        else
                        {
                            tenders.Tend_Totals.PennyAdj_Required = false;
                        }
                    }
                    else
                    {
                        tenders.Tend_Totals.PennyAdj_Required = false;
                    }
                }
                else
                {
                    tenders.Tend_Totals.PennyAdj_Required = false;
                }
            }
            else
            {
                tenders.Tend_Totals.PennyAdj_Required = false;
            }
            if (vData != 0 && sale != null)
            {
                WriteToLogFile("Penny Adj required set to" + Convert.ToString(tenders.Tend_Totals.PennyAdj_Required) + " " + Convert.ToString(sale.Sale_Num) + " tender " + td.Tender_Name);
            }

            //Added by Tony on 02/08/2019
            tenders.Tend_Totals.Tend_Amount = tenders.Tend_Totals.Tend_Amount - td.Amount_Entered;
            tenders.Tend_Totals.Tend_Used = tenders.Tend_Totals.Tend_Used - td.Amount_Used;

            // the code block below was commented out by Tony on 02/08/2019

            ////Added the following condition for Ackroo only
            //if (td.Tender_Code != "ACK")
            //{
            //    // Remove the old amount
            //    tenders.Tend_Totals.Tend_Amount = tenders.Tend_Totals.Tend_Amount - td.Amount_Entered;
            //    tenders.Tend_Totals.Tend_Used = tenders.Tend_Totals.Tend_Used - td.Amount_Used;
            //}

            if (!td.Give_Change)
            {
                tenders.Tend_Totals.No_Change_Total = tenders.Tend_Totals.No_Change_Total - td.Amount_Used;
            }
            //   to use sale totals object to save and reload the penny adj. values
            if (sale != null)
            {
                if (tenders.Tend_Totals.PennyAdj_Required)
                {

                    sale.Sale_Totals.Penny_Adj = tenders.Tend_Totals.Penny_Adj;
                }
                else
                {
                    sale.Sale_Totals.Penny_Adj = 0;

                }
            }
            // If specified, restrict the amount entered to the limit.
            if (limit >= 0 & vData > limit)
            {
                vData = limit;
            }

            if (td.System_Can_Adjust)
            {
                var x = (float)((decimal)td.Exchange_Rate * vData);





                //  ' Nancy's change was screwing up the Cash deop
                if (x > 0)
                {
                    if (sale != null && sale.Sale_Type == "SALE")
                    {
                        if (x > (float)(sale.Sale_Totals.Gross - tenders.Tend_Totals.Tend_Used))
                        {
                            x = (float)(sale.Sale_Totals.Gross - tenders.Tend_Totals.Tend_Used);
                        }
                    }
                }

                td.Amount_Used = (decimal)x;
            }

            td.Amount_Entered = vData;

            //Added by Tony on 02/08/2019
            tenders.Tend_Totals.Tend_Amount = tenders.Tend_Totals.Tend_Amount + td.Amount_Entered;
            tenders.Tend_Totals.Tend_Used = tenders.Tend_Totals.Tend_Used + td.Amount_Used;

            //The code block below was commented out by Tony on 02/08/2019

            ////only
            //if (td.Tender_Code != "ACK")
            //{
            //    // and add the new one.
            //    tenders.Tend_Totals.Tend_Amount = tenders.Tend_Totals.Tend_Amount + td.Amount_Entered;
            //    tenders.Tend_Totals.Tend_Used = tenders.Tend_Totals.Tend_Used + td.Amount_Used;
            //}
            ////end

            if (!td.Give_Change)
            {
                tenders.Tend_Totals.No_Change_Total = tenders.Tend_Totals.No_Change_Total + td.Amount_Used;
            }

            // the
            // tender is KickBack card points; use 2 If statements so second condition is not
            // evaluated unless KickBack policy is on
            if (_policyManager.Use_KickBack)
            {
                if (sale != null && td.Tender_Class == "LOYALTY" && td.Credit_Card.Crd_Type == "K")
                {
                    if (sale.Customer != null)
                    {
                        if (sale.Customer.PointCard_Registered)
                        {
                            sale.Customer.Points_Redeemed = (double)vData;
                        }
                    }
                }
                //   to set PCATSGroup property

                td.PCATSGroup = _tenderService.GetPcatsGroup(td.Tender_Class);

                //   end
            }
            //   end

        }

        /// <summary>
        /// Get Alll Tenders
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="userCode">User code</param>
        /// <param name="blTillClose">Till close</param>
        /// <param name="dropReason">Drop reason</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Tenders</returns>
        public Tenders GetAllTender(int saleNumber, int tillNumber, string transactionType, string userCode,
            bool blTillClose, string dropReason, out ErrorMessage errorMessage)
        {

            List<Report> transactionReports = null;
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 1, userCode,
                out errorMessage);

           // var purchaseList = CacheManager.GetPurchaseListSaleForTill(sale.TillNumber, sale.Sale_Num);

            if (transactionType == "Sale" && errorMessage?.MessageStyle?.Message != null)
            {
                return null;
            }
            else
            {
                errorMessage = new ErrorMessage();
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
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
            if (!IsValidTransactionType(transactionType))
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = "Invalid transaction type"
                };
                errorMessage.StatusCode = HttpStatusCode.NotFound;
                return null;
            }
            if (sale != null && Math.Round(sale.Sale_Totals.Gross, 2) == 0 && transactionType == "Sale")
            {
                //"Complete the Sale?", vbQuestion + vbYesNo, "Zero Sale Total", Me)
                var temp_VbStyle4 = MessageType.YesNo;
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 95, null, temp_VbStyle4);
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
            }
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return null;
            }
            var tenders = Load(sale, transactionType, blTillClose, dropReason, out errorMessage);
            //tenders = Load(sale, transactionType, blTillClose, dropReason, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }
            if (transactionType == "Sale")
            {
                if (tenders.EnableCompletePayment)
                {
                    return tenders;
                }
                if (sale != null)
                {
                    sale.Payment = _saleService.CheckPaymentsFromDbTemp(saleNumber, tillNumber);

                    if (sale.Payment)
                    {
                        //   - for crash recovery
                        Load_Temp_Tenders(ref tenders, till, ref sale);
                    }
                    else
                    {
                        Zero_Tenders(ref tenders, ref sale);

                    }
                    bool displayNoReceiptButton;
                    var arPay = CacheManager.GetArPayment(sale.Sale_Num);
                    var payment = CacheManager.GetFleetPayment(sale.Sale_Num);
                    var grossTotal = GetGrossTotal(transactionType, sale, payment, arPay, 0, out displayNoReceiptButton);
                    tenders.Tend_Totals.Gross = grossTotal;
                    tenders.Tend_Totals.Change = Math.Abs(grossTotal) - Math.Abs(tenders.Tend_Totals.Tend_Used);
                    SetSummary2Informatiion(ref tenders, grossTotal);

                    if ((sale.Payment == false) && !(_policyManager.Use_KickBack && !string.IsNullOrEmpty(sale.Customer.PointCardNum) && sale.Customer.Points_Redeemed != 0))
                    {
                        tenders.Summary1 = _resourceManager.GetResString(offSet, 163) + 0.ToString("###,##0.00"); //"Tendered: $"
                        tenders.Summary2 = _resourceManager.GetResString(offSet, 164) + grossTotal.ToString("###,##0.00"); //"Outstanding $"
                    }
                    else
                    {
                        tenders.Summary1 = _resourceManager.GetResString(offSet, 163) + tenders.Tend_Totals.Tend_Used.ToString("###,##0.00"); //"Tendered: $"
                    }
                    //   for penny adjustment
                    if (sale.Sale_Totals.Penny_Adj != 0)
                    {
                        tenders.Summary1 = tenders.Summary1 + "\r\n" + _resourceManager.GetResString(offSet, 485) + sale.Sale_Totals.Penny_Adj.ToString("###,##0.00");
                    }
                    //   end
                    decimal sumQa = 0;
                    if (!string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && _policyManager.USE_LOYALTY && _policyManager.LOYAL_TYPE == "Points")
                    {
                        if (_policyManager.ALLOW_CUR_PT)
                        {
                            sumQa = _saleManager.ComputePoints(sale);
                            //Sum_QA = sale.ComputePoints();
                        }
                    }
                }


                //###PTC -End

                WriteToLogFile("Initialize finished SaleTend form");
                //TODO: Ackroo_Removed
                //  added to support Auto Refund for Ackroo cards
                //AutoRefundAckrootenders();
            }
            else if (transactionType == "ARPay")
            {
                bool displayNoReceipt = true;
                var arPay = GetARPayer(saleNumber, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                {
                    return null;
                }
                var gross = GetGrossTotal(transactionType, null, null, arPay, -1, out displayNoReceipt);
                //tenders.DisplayNoReceiptButton = displayNoReceipt;
                tenders.Tend_Totals.Gross = gross;
                tenders.Tend_Totals.Change = Math.Abs(gross) - Math.Abs(tenders.Tend_Totals.Tend_Used);
                tenders.Summary1 = _resourceManager.GetResString(offSet, (short)163) + tenders.Tend_Totals.Tend_Used.ToString("###,##0.00"); //"Tendered: $"
                SetSummary2Informatiion(ref tenders, gross);
            }
            else if (transactionType == "Payment")
            {
                bool displayNoReceipt = true;
                var payment = GetFleetPayer(saleNumber, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                {
                    return null;
                }
                var gross = GetGrossTotal(transactionType, null, payment, null, -1, out displayNoReceipt);
                //tenders.DisplayNoReceiptButton = displayNoReceipt;
                tenders.Tend_Totals.Gross = gross;
                tenders.Tend_Totals.Change = Math.Abs(gross) - Math.Abs(tenders.Tend_Totals.Tend_Used);
                tenders.Summary1 = _resourceManager.GetResString(offSet, 163) + tenders.Tend_Totals.Tend_Used.ToString("###,##0.00"); //"Tendered: $"
                SetSummary2Informatiion(ref tenders, gross);
            }
            Register register = new Register();
            if (sale != null)
            {
                _mainManager.SetRegister(ref register, sale.Register);
                if (register.Customer_Display)
                {
                    if (transactionType != "CashDrop")
                    {
                        tenders.CustomerDisplay = _mainManager.DisplayMsgLcd(register,
                            _mainManager.FormatLcdString(register, "", tenders.Summary1), _mainManager.FormatLcdString(register, "", tenders.Summary2));
                    }
                }
                var user = _loginManager.GetUser(userCode);
                tenders.EnableRunAway = EnableRunaway(user, sale);
                tenders.EnablePumpTest = EnablePumpTest(user, sale);
            }



            return tenders;
        }

        /// <summary>
        /// Load all tenders
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="blTillClose">Till close</param>
        /// <param name="reasonCode">Reason code</param>
        /// <param name="error">Error</param>
        /// <returns>Tenders</returns>
        public Tenders Load(Sale sale, string transactionType, bool blTillClose, string reasonCode,
            out ErrorMessage error)
        {
            Chaps_Main.SA = sale;
            error = new ErrorMessage();
            bool useTend;
            byte firstFleet = 0;
            byte firstCc = 0;
            byte firstThirdParty = 0;
            Tenders allTenders = null;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (sale != null && transactionType == "Sale")
            {
                allTenders = CacheManager.GetTenderForSale(sale.Sale_Num, sale.TillNumber);
            }
            else if (transactionType == "CashDrop")
            {
                allTenders = CacheManager.GetTenderForTransactionTypeAndReason(transactionType, reasonCode);

            }
            else if (transactionType == "ARPay")
            {
                allTenders = CacheManager.GetTenderForArpAy(sale.Sale_Num);
            }
            else if (transactionType == "Payment")
            {
                allTenders = CacheManager.GetTenderForPayment(sale.Sale_Num);
            }
            if (allTenders != null)
                return allTenders;


            allTenders = new Tenders();
            // For a till close, we need all the tenders as they are in TendMast table
            // with proper description, but displaying the tenders on the screen is based on policy
            var blCombineFleet = Convert.ToBoolean(_policyManager.COMBINEFLEET && !blTillClose);
            var blCombineCredit = Convert.ToBoolean(_policyManager.COMBINECR && !blTillClose);

            var blCombineThirdParty = _policyManager.ThirdParty && Convert.ToBoolean(!blTillClose);

            if (blCombineThirdParty)
            {
                firstThirdParty = _tenderService.GetMinDisplaySeq("THIRDPARTY");
            }


            if (blCombineFleet)
            {
                firstFleet = _tenderService.GetMinDisplaySeq("FLEET");
            }
            if (blCombineCredit)
            {
                firstCc = _tenderService.GetMinDisplaySeq("CRCARD");
            }

            var tempTenders = _tenderService.GetAlltenders();
            var tenders = new List<Tender>();


            if (!_policyManager.IsWexEnable)   // to disable the wex tender in case the policy is off
            {
                foreach (var tender in tempTenders)
                {
                    var cardCode = _tenderService.GetCardCode(tender.Tender_Code);
                    if (!(_cardService.GetCradGiftType(cardCode) == "W"))
                    {
                        tenders.Add(tender);
                    }
                }
            }
            else
            {
                foreach (var tender in tempTenders)
                {
                    tenders.Add(tender);
                }
            }

            if (transactionType == "Sale")
            {
                foreach (var tender in tenders)
                {
                    useTend = false;

                    // Select Refund Tenders
                    if (sale.Sale_Totals.Gross < 0)
                    {
                        if (tender.Give_As_Refund)
                        {
                            if (tender.Tender_Class == "ACCOUNT")
                            {
                                if (_policyManager.CREDTERM) // Only if company allows TERM CREDIT
                                {
                                    if (sale.Customer.AR_Customer && _policyManager.U_ARSALES)
                                    {
                                        useTend = true;
                                    }
                                    else if (_policyManager.Charge_Acct && _policyManager.CREDTERM && _policyManager.U_ARSALES)
                                    {
                                        useTend = true;
                                    }
                                    else
                                    {
                                        useTend = false;
                                    }
                                }
                                else
                                {
                                    useTend = false;
                                }
                            }
                            else if (tender.Tender_Class == "POINTS")
                            {
                                //  - we are allowing to use points in Refunds
                                //Use_Tend = False
                                // Use points if the loyalty system is active and it is a
                                // Points-based system and the current customer is a
                                // loyalty customer.
                                if (!string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && _policyManager.USE_LOYALTY && _policyManager.LOYAL_TYPE == "Points")
                                {
                                    useTend = true;
                                }
                            }
                            else if (tender.Tender_Class == "GIFTCERT")
                            {


                                if (_policyManager.GIFTCERT)
                                {
                                    useTend = !_tenderService.IsEkoGiftCert(tender.Tender_Code) || Convert.ToBoolean(_policyManager.ThirdParty);
                                }
                            }
                            else if (tender.Tender_Class == "CREDIT")
                            {
                                if (_policyManager.STORE_CREDIT)
                                {
                                    useTend = true;
                                }
                            }
                            else if (tender.Tender_Class == "FLEET")
                            {
                                useTend = true;
                            }
                            else if (tender.Tender_Class == "CRCARD")
                            {
                                useTend = true;

                                //   changed to separate loyalty and thirdparty classes
                            }
                            else if (tender.Tender_Class == "THIRDPARTY")
                            {
                                useTend = Convert.ToBoolean(_policyManager.ThirdParty);

                            }
                            else if (tender.Tender_Class == "LOYALTY")
                            {
                                //Ackroo
                                useTend = Convert.ToBoolean(_policyManager.ThirdParty || _policyManager.Use_KickBack || _policyManager.REWARDS_Enabled);
                                //   end
                            }
                            else
                            {
                                useTend = true;
                            }
                        }

                    }
                    else
                    {
                        // Select sale tenders
                        if (tender.Tender_Class == "ACCOUNT")
                        {
                            // Use account tenders if this business has customer A/R and
                            // the current customer is an A/R customer.
                            if (_policyManager.CREDTERM && sale.Customer.AR_Customer && _policyManager.U_ARSALES)
                            {
                                useTend = true;
                                //  Gasking Charges - If Charge_acct policy is anabled load Account even if the customer is not selected
                            }
                            else if (_policyManager.Charge_Acct && _policyManager.CREDTERM && _policyManager.U_ARSALES)
                            {
                                useTend = true;
                            }
                            else
                            {
                                useTend = false;
                            }
                        }
                        else if (tender.Tender_Class == "POINTS")
                        {
                            // Use points if the loyalty system is active and it is a
                            // Points-based system and the current customer is a
                            // loyalty customer.
                            if (!string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && _policyManager.USE_LOYALTY && _policyManager.LOYAL_TYPE == "Points")
                            {
                                useTend = true;
                            }
                        }
                        else if (tender.Tender_Class == "GIFTCERT")
                        {
                            
                            if (_policyManager.GIFTCERT)
                            {
                                useTend = !_tenderService.IsEkoGiftCert(tender.Tender_Code) || Convert.ToBoolean(_policyManager.ThirdParty);
                            }
                        }
                        else if (tender.Tender_Class == "CREDIT")
                        {
                            if (_policyManager.STORE_CREDIT)
                            {
                                useTend = true;
                            }
                        }
                        else if (tender.Tender_Class == "FLEET")
                        {
                            useTend = true;
                        }
                        else if (tender.Tender_Class == "CRCARD")
                        {
                            useTend = true;
                            //   changed to separate loyalty and thirdparty classes
                        }
                        else if (tender.Tender_Class == "THIRDPARTY")
                        {
                            useTend = Convert.ToBoolean(_policyManager.ThirdParty);

                        }
                        else if (tender.Tender_Class == "LOYALTY")
                        {
                            useTend = Convert.ToBoolean(_policyManager.ThirdParty || _policyManager.Use_KickBack || _policyManager.REWARDS_Enabled);
                        }
                        else
                        {
                            useTend = true;
                        }
                    }

                    if (useTend)
                    {
                        if ((blCombineFleet && tender.Tender_Class == "FLEET") || (blCombineCredit && tender.Tender_Class == "CRCARD") || (blCombineThirdParty && tender.Tender_Class == "THIRDPARTY"))
                        {
                            if ((blCombineFleet && tender.Sequence_Number == firstFleet) || (blCombineCredit && tender.Sequence_Number == firstCc) || (blCombineThirdParty && tender.Sequence_Number == firstThirdParty))
                            {
                                allTenders.Add(tender.TendClassDescription, tender.Tender_Class,
                                    tender.Exchange_Rate, tender.Give_Change, tender.Give_As_Refund,
                                    tender.System_Can_Adjust, tender.Sequence_Number, tender.Tender_Code,
                                    tender.Exact_Change, tender.MaxAmount, tender.MinAmount,
                                    tender.Smallest_Unit, tender.Open_Drawer, 0, tender.PrintCopies,
                                    tender.AcceptAspayment, tender.SignatureLine, tender.Image, tender.TendDescription);
                                Set_Amount_Entered(ref allTenders, ref sale, tender, tender.Amount_Entered, -1);
                            }
                        }
                        else
                        {
                            allTenders.Add(tender.TendDescription, tender.Tender_Class, tender.Exchange_Rate,
                                tender.Give_Change, tender.Give_As_Refund, tender.System_Can_Adjust,
                                tender.Sequence_Number, tender.Tender_Code, tender.Exact_Change,
                                tender.MaxAmount, tender.MinAmount, tender.Smallest_Unit, tender.Open_Drawer,
                                0, tender.PrintCopies, tender.AcceptAspayment, tender.SignatureLine,
                                tender.Image, tender.TendDescription);
                            Set_Amount_Entered(ref allTenders, ref sale, tender, tender.Amount_Entered, -1);
                        }
                    }
                }
            }
            else if (transactionType == "Payment" || transactionType == "ARPay")
            {
                foreach (var tender in tenders)
                {
                    useTend = false;


                    if ((tender.Tender_Class == "ACCOUNT") || (tender.Tender_Class == "POINTS") || (tender.Tender_Class == "COUPON"))
                    {
                        useTend = false;
                    }
                    else if (tender.Tender_Class == "GIFTCERT")
                    {


                        if (_policyManager.GIFTCERT)
                        {
                            useTend = !_tenderService.IsEkoGiftCert(Strings.Trim(tender.Tender_Code)) || Convert.ToBoolean(_policyManager.ThirdParty);
                        }
                    }
                    else if (tender.Tender_Class == "CREDIT")
                    {
                        if (_policyManager.STORE_CREDIT)
                        {
                            useTend = true;
                        }
                    }
                    else if (tender.Tender_Class == "FLEET")
                    {
                        useTend = true;
                    }
                    else if (tender.Tender_Class == "CRCARD")
                    {
                        useTend = true;
                        //   changed to separate loyalty and thirdparty classes
                    }
                    else if (tender.Tender_Class == "THIRDPARTY")
                    {
                        useTend = Convert.ToBoolean(_policyManager.ThirdParty);

                    }
                    else if (tender.Tender_Class == "LOYALTY")
                    {
                        useTend = Convert.ToBoolean(_policyManager.ThirdParty || _policyManager.Use_KickBack || _policyManager.REWARDS_Enabled);

                    }
                    else
                    {
                        useTend = true;
                    }


                    if (Strings.UCase(tender.TendDescription) == Strings.UCase(Convert.ToString(_policyManager.CouponTend)))
                    {
                        useTend = false;
                    }


                    if (useTend)
                    {
                        if (tender.AcceptAspayment)
                        {
                            if ((blCombineFleet && tender.Tender_Class == "FLEET") || (blCombineCredit && tender.Tender_Class == "CRCARD") || (blCombineThirdParty && tender.Tender_Class == "THIRDPARTY"))
                            {
                                if ((blCombineFleet && tender.Sequence_Number == firstFleet) || (blCombineCredit && tender.Sequence_Number == firstCc) || (blCombineThirdParty && tender.Sequence_Number == firstThirdParty))
                                {
                                    allTenders.Add(tender.TendClassDescription, tender.Tender_Class,
                                        tender.Exchange_Rate, tender.Give_Change, tender.Give_As_Refund,
                                        tender.System_Can_Adjust, tender.Sequence_Number, tender.Tender_Code,
                                        tender.Exact_Change, tender.MaxAmount,
                                        tender.MinAmount, tender.Smallest_Unit, tender.Open_Drawer, 0,
                                        tender.PrintCopies, tender.AcceptAspayment, tender.SignatureLine,
                                        tender.Image, tender.TendDescription);
                                    Set_Amount_Entered(ref allTenders, ref sale, tender, tender.Amount_Entered, -1);
                                }
                            }
                            else
                            {
                                allTenders.Add(tender.TendDescription, tender.Tender_Class,
                                    tender.Exchange_Rate, tender.Give_Change, tender.Give_As_Refund,
                                    tender.System_Can_Adjust, tender.Sequence_Number, tender.Tender_Code,
                                    tender.Exact_Change, tender.MaxAmount, tender.MinAmount,
                                    tender.Smallest_Unit, tender.Open_Drawer, 0, tender.PrintCopies,
                                    tender.AcceptAspayment, tender.SignatureLine, tender.Image, tender.TendDescription);
                                Set_Amount_Entered(ref allTenders, ref sale, tender, tender.Amount_Entered, -1);
                            }
                        }
                    }
                }
                if (transactionType == "ARPay")
                    CacheManager.AddTendersForArPay(sale.Sale_Num, allTenders);
                else
                    CacheManager.AddTendersForPayment(sale.Sale_Num, allTenders);
            }
            else if (transactionType == "CashDrop")
            {
                var flag = true;

                if (_policyManager.SAFEATMDROP && reasonCode != "ATM" && reasonCode != "SAFE")
                {
                    flag = false;
                }
                else if (!_policyManager.SAFEATMDROP && reasonCode != "SAFE")
                {
                    flag = false;

                }

                if (!flag)
                {
                    error.MessageStyle = new MessageStyle
                    {
                        Message = "Invalid reason"
                    };
                    error.StatusCode = HttpStatusCode.NotFound;
                    return null;
                }

                // currency
                if (reasonCode == "ATM")
                {
                    foreach (var tender in tenders)
                    {

                        if (Strings.UCase(tender.TendDescription) == _policyManager.BASECURR && Strings.UCase(tender.TendDescription) != Strings.UCase(Convert.ToString(_policyManager.CouponTend)))
                        {
                            allTenders.Add(tender.TendDescription, tender.Tender_Class, tender.Exchange_Rate,
                                tender.Give_Change, tender.Give_As_Refund, tender.System_Can_Adjust,
                                tender.Sequence_Number, tender.Tender_Code, tender.Exact_Change,
                                tender.MaxAmount, tender.MinAmount, tender.Smallest_Unit, tender.Open_Drawer,
                                0, tender.PrintCopies, tender.AcceptAspayment, tender.SignatureLine,
                                tender.Image, tender.TendDescription);
                            Set_Amount_Entered(ref allTenders, ref sale, tender, tender.Amount_Entered, -1);
                        }
                    }
                }
                else
                {
                    foreach (var tender in tenders)
                    {
                        if (tender.Open_Drawer && (tender.Tender_Class == "CASH" || tender.Tender_Class == "CHEQUE")) //   - Option to include cheque in the cash drop
                        {
                            if (Strings.UCase(tender.TendDescription) !=
                                Strings.UCase(Convert.ToString(_policyManager.CouponTend)))
                            {
                                allTenders.Add(tender.TendDescription, tender.Tender_Class, tender.Exchange_Rate,
                                    tender.Give_Change, tender.Give_As_Refund, tender.System_Can_Adjust,
                                    tender.Sequence_Number, tender.Tender_Code, tender.Exact_Change, tender.MaxAmount,
                                    tender.MinAmount, tender.Smallest_Unit, tender.Open_Drawer, 0, tender.PrintCopies,
                                    tender.AcceptAspayment, tender.SignatureLine, tender.Image, tender.TendDescription);
                                Set_Amount_Entered(ref allTenders, ref sale, tender, tender.Amount_Entered, -1);
                            }
                        }
                    }
                }
                CacheManager.AddTenders(transactionType, reasonCode, allTenders);
            }
            else if (transactionType == "Prepay" || transactionType == "Delete Prepay")
            {


                foreach (var tender in tenders)
                {
                    useTend = false;
                    if (tender.Tender_Class == "ACCOUNT")
                    {
                        // Use account tenders if this business has customer A/R and
                        // the current customer is an A/R customer.
                        if (_policyManager.CREDTERM && sale.Customer.AR_Customer && _policyManager.U_ARSALES)
                        {
                            useTend = true;
                        }
                        else if (_policyManager.Charge_Acct && _policyManager.CREDTERM && _policyManager.U_ARSALES)
                        {
                            useTend = true;
                        }
                        else
                        {
                            useTend = false;
                        }
                    }
                    else if (tender.Tender_Class == "POINTS")
                    {
                        // Use points if the loyalty system is active and it is a
                        // Points-based system and the current customer is a
                        // loyalty customer.
                        if (!string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && _policyManager.USE_LOYALTY && _policyManager.LOYAL_TYPE == "Points")
                        {
                            useTend = true;
                        }
                    }
                    else if (tender.Tender_Class == "GIFTCERT")
                    {


                        if (_policyManager.GIFTCERT)
                        {
                            useTend = !_tenderService.IsEkoGiftCert(Strings.Trim(tender.Tender_Code)) || Convert.ToBoolean(_policyManager.ThirdParty);
                        }

                    }
                    else if (tender.Tender_Class == "CREDIT")
                    {
                        if (_policyManager.STORE_CREDIT)
                        {
                            useTend = true;
                        }
                    }
                    else if (tender.Tender_Class == "FLEET")
                    {
                        useTend = true;
                    }
                    else if (tender.Tender_Class == "CRCARD")
                    {
                        useTend = true;

                        //   changed to separate loyalty and thirdparty classes
                    }
                    else if (tender.Tender_Class == "THIRDPARTY")
                    {
                        useTend = Convert.ToBoolean(_policyManager.ThirdParty);

                    }
                    else if (tender.Tender_Class == "LOYALTY")
                    {
                        //Ackroo
                        useTend = Convert.ToBoolean(_policyManager.ThirdParty || _policyManager.Use_KickBack || _policyManager.REWARDS_Enabled);
                        //   end

                    }
                    else
                    {
                        useTend = true;
                    }




                    if (useTend && (transactionType == "Prepay" || (transactionType == "Delete Prepay" && tender.Give_As_Refund)))
                    {
                        if ((blCombineFleet && tender.Tender_Class == "FLEET") || (blCombineCredit && tender.Tender_Class == "CRCARD") || (blCombineThirdParty && tender.Tender_Class == "THIRDPARTY"))
                        {
                            if ((blCombineFleet && tender.Sequence_Number == firstFleet) || (blCombineCredit && tender.Sequence_Number == firstCc) || (blCombineThirdParty && tender.Sequence_Number == firstThirdParty))
                            {
                                allTenders.Add(tender.TendClassDescription, tender.Tender_Class,
                                    tender.Exchange_Rate, tender.Give_Change, tender.Give_As_Refund,
                                    tender.System_Can_Adjust, tender.Sequence_Number, tender.Tender_Code,
                                    tender.Exact_Change, tender.MaxAmount, tender.MinAmount,
                                    tender.Smallest_Unit, tender.Open_Drawer, 0, tender.PrintCopies,
                                    tender.AcceptAspayment, tender.SignatureLine, tender.Image,
                                    tender.TendDescription);
                                Set_Amount_Entered(ref allTenders, ref sale, tender, tender.Amount_Entered, -1);
                            }
                        }
                        else
                        {
                            allTenders.Add(tender.TendDescription, tender.Tender_Class, tender.Exchange_Rate,
                                tender.Give_Change, tender.Give_As_Refund, tender.System_Can_Adjust,
                                tender.Sequence_Number, tender.Tender_Code, tender.Exact_Change,
                                tender.MaxAmount, tender.MinAmount, tender.Smallest_Unit, tender.Open_Drawer,
                                0, tender.PrintCopies, tender.AcceptAspayment, tender.SignatureLine,
                                tender.Image, tender.TendDescription);
                            Set_Amount_Entered(ref allTenders, ref sale, tender, tender.Amount_Entered, -1);
                        }
                    }
                }
            }
            else if (transactionType == "CloseCurrentTill")
            {
                foreach (var tender in tenders)
                {
                    if ((blCombineFleet && tender.Tender_Class == "FLEET") || (blCombineCredit && tender.Tender_Class == "CRCARD") || (blCombineThirdParty && tender.Tender_Class == "THIRDPARTY"))
                    {
                        if ((blCombineFleet && tender.Sequence_Number == firstFleet) || (blCombineCredit && tender.Sequence_Number == firstCc) || (blCombineThirdParty && tender.Sequence_Number == firstThirdParty))
                        {
                            allTenders.Add(tender.TendClassDescription, tender.Tender_Class,
                                tender.Exchange_Rate, tender.Give_Change, tender.Give_As_Refund,
                                tender.System_Can_Adjust, tender.Sequence_Number, tender.Tender_Code,
                                tender.Exact_Change, tender.MaxAmount, tender.MinAmount, tender.Smallest_Unit,
                                tender.Open_Drawer, 0, tender.PrintCopies, tender.AcceptAspayment,
                                tender.SignatureLine, tender.Image, tender.TendDescription);
                            Set_Amount_Entered(ref allTenders, ref sale, tender, tender.Amount_Entered, -1);
                        }
                    }
                    else
                    {
                        allTenders.Add(tender.TendDescription, tender.Tender_Class, tender.Exchange_Rate,
                            tender.Give_Change, tender.Give_As_Refund, tender.System_Can_Adjust,
                            tender.Sequence_Number, tender.Tender_Code, tender.Exact_Change,
                            tender.MaxAmount, tender.MinAmount, tender.Smallest_Unit, tender.Open_Drawer, 0,
                            tender.PrintCopies, tender.AcceptAspayment, tender.SignatureLine,
                            tender.Image, tender.TendDescription);
                        Set_Amount_Entered(ref allTenders, ref sale, tender, tender.Amount_Entered, -1);
                    }
                }
            }
            else
            {
                error.MessageStyle = new MessageStyle
                {
                    Message = "Invalid transaction type"
                };
                error.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }

            // Writing this for the issue "Not enough paid" 
            if(sale != null)   // this line is added by Tony on 08/16/2019 
            {
                var saleTenders = _saleService.GetSaleTendersFromDbTemp(sale.Sale_Num, sale.TillNumber);

                if (saleTenders.Count() != 0)
                {
                    for (int i = 1; i <= allTenders.Count(); i++)
                    {
                        foreach (var saleTender in saleTenders)
                        {
                            if (allTenders[i].Tender_Class == saleTender.TenderClass && allTenders[i].Tender_Name == saleTender.TenderName)
                            {
                                allTenders[i].Amount_Used = (decimal)saleTender.AmountUsed;
                                allTenders[i].Amount_Entered = (decimal)saleTender.AmountTend;
                                allTenders.Tend_Totals.Tend_Used = allTenders.Tend_Totals.Tend_Used + Convert.ToDecimal(saleTender.AmountUsed);
                                allTenders.Tend_Totals.Tend_Amount = allTenders.Tend_Totals.Tend_Amount + Convert.ToDecimal(saleTender.AmountTend);

                            }
                        }
                    }
                }
            }
            

            return allTenders;
        }

        /// <summary>
        /// Zero Tender
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="sale">Sale</param>
        public void Zero_Tenders(ref Tenders tenders, ref Sale sale)
        {
            foreach (Tender tempLoopVarTend in tenders)
            {
                var tend = tempLoopVarTend;

                Set_Amount_Entered(ref tenders, ref sale, tend, 0, -1);
            }
        }

        /// <summary>
        /// Method to update tenders
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="userCode">User code</param>
        /// <param name="blTillClose">Till close</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="amountEntered">Amount entered</param>
        /// <param name="transactionReports">Transaction reports</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Tenders</returns>
        public Tenders UpdateTenders(int saleNumber, int tillNumber, string transactionType, string userCode,
           bool blTillClose, string tenderCode, string amountEntered, out List<Report> transactionReports, 
           out ErrorMessage errorMessage, bool isUpdateTender = false, bool isAmountEnteredManually = false)
        {
            errorMessage = new ErrorMessage();
            transactionReports = null;
            var till = _tillService.GetTill(tillNumber);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (till == null)
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = "Till does not exists"
                };
                errorMessage.StatusCode = HttpStatusCode.NotFound;
                return null;
            }
            var tenders = GetAllTender(saleNumber, till.Number, transactionType,
                userCode, blTillClose,
                string.Empty, out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return null;
            }
            /*made register number as 1 from 0 */
            var sale = _saleManager.GetCurrentSale(saleNumber, till.Number, 1, userCode, out errorMessage);
            Load_Temp_Tenders(ref tenders, till, ref sale);
            bool isInvalidTender = false;
            var tender = GetSelectedTender(tenders, tenderCode);
            if (tender == null)
            {
                isInvalidTender = true;
            }
            else
            {
                var arPay = GetARPayer(saleNumber, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message) && transactionType == "ARPay")
                {
                    return null;
                }
                var payment = GetFleetPayer(saleNumber, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message) && transactionType == "Payment")
                {
                    return null;
                }
                //if this tender already has amount entered, remove that
                if (string.IsNullOrEmpty(amountEntered))
                {
                    Set_Amount_Entered(ref tenders, ref sale, tender, 0, -1);
                }
                bool displayNoReceiptButton;
                var gross = GetGrossTotal(transactionType, sale, payment, arPay, -1, out displayNoReceiptButton);

                SaleTend_Keydown(ref tenders, sale, userCode, tenderCode, ref amountEntered, transactionType, gross,
                    null, out transactionReports, out errorMessage, true, null, false, true, isAmountEnteredManually);
                
               

                if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message) && errorMessage.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                var message = errorMessage;
                UpdateTender(ref tenders, sale, till, transactionType, userCode, blTillClose, tenderCode,
                    amountEntered, out errorMessage);
                
                //if (transactionType == "Sale" || transactionType == "Delete Prepay") //saving only for sale type
                //{
                //    _saleManager.Save_Tender_Temp(ref tenders, sale);
                //    // sale.Save_Tender_Temp();
                //}

                var tenderstest = GetAllTender(saleNumber, tillNumber,
                 transactionType, userCode, false, string.Empty, out errorMessage);

                if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                {
                    errorMessage = message;
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
            WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside update tenders 1120 cardno value" + sale.Customer.PointCardNum);
            if (transactionType == "Sale")
            {
                WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside update tenders inside function 1122 cardno value" + sale.Customer.PointCardNum);
                CacheManager.AddTendersForSale(saleNumber, till.Number, tenders);
                _saleManager.Save_Tender_Temp(ref tenders, sale);
            }
            else if (transactionType == "ARPay")
                CacheManager.AddTendersForArPay(saleNumber, tenders);
            else if (transactionType == "Payment")
                CacheManager.AddTendersForPayment(saleNumber, tenders);
            return tenders;
        }

        /// <summary>
        /// Method to update a tender
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="sale">Sale</param>
        /// <param name="till">Till</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="userCode">User code</param>
        /// <param name="blTillClose">Till close</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="tenderedAmount">Tendered amount</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Tenders</returns>
        public Tenders UpdateTender(ref Tenders tenders, Sale sale, Till till, string transactionType, string userCode,
            bool blTillClose, string tenderCode, string tenderedAmount, out ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();
            double entered = 0;
            double used = 0;
            Tender td = default(Tender);
            double tenderMin = 0;
            double tenderMax = 0;
            string tenderName = "";
            string[] capValue = new string[3];
            bool boolInactive = false;
            var offSet = _policyManager.LoadStoreInfo().OffSet;

            if (tenderedAmount.IndexOf(";") + 1 > 0)
            {
                tenderedAmount = "";
                return null;
            }
            td = GetSelectedTender(tenders, tenderCode);// TD is the tender that is on the line we are leaving

            //   to fix the issue of items scanned in SaleTend screen resulting in crash
            //   added next If for ARPay to allow bigger amounts for ARPay (required by Langbank)
            if (transactionType.ToUpper() == "ARPay".ToUpper())
            {
                if (Conversion.Val(tenderedAmount) > 200000)
                {
                    tenderedAmount = "";
                }
            }
            else
            {
                if (Conversion.Val(tenderedAmount) > 99999.99)
                {
                    tenderedAmount = "";
                }
            }


            //    Entered = val(Format(txtAmount(Index).Text, "##0.00"))   ' The amount that is is being changed to.
            entered = Conversion.Val(string.Format(tenderedAmount, "##0.00")); // The amount that is is being changed to.

            //  - Tender  should use the minimum and maximum amount to control the amount used if the setting is <> 0 , zero means we are not putting any restriction- EKO requirement
            // One issue identified during this - If combine _policyManager is enabled we are loading only 1 st tender for that type( e.g. Fleet, Thirdparty and Creditcard)- so for those tenders we cand get the information from tender collection because other tenders setting( except fist fleet is not loaded) - reason why it is not loaded is Tender items in the collection and tender line in the pos screen is linked. If we skip the display after loading them in the collection there will be issues. so need to get the data directly fom database for combined _policyManager effective Fleet, Thirdparty and credit
            Tender tempTend = new Tender();
            if ((_policyManager.COMBINECR && td.Tender_Class == "CRCARD") || (_policyManager.COMBINEFLEET && td.Tender_Class == "FLEET"))
            {
                tenderMin = tempTend.MinAmount;
                tenderMax = tempTend.MaxAmount;
                tenderName = tempTend.Tender_Name;
                boolInactive = tempTend.Inactive;
            }
            else //
            {
                tenderMin = td.MinAmount;
                tenderMax = td.MaxAmount;
                tenderName = td.Tender_Name;
            }

            // Nov 02, 2009 Nicolette: if the tender is set as inactive, don't continue
            // boolInactive can be True only if processed tenders is a combined card
            // above If is the only way boolInactive can be True, otherwise is False
            // need to set it here if the card is combined, for regular tenders inactive
            // tenders are not loaded into tenders collection so they are not accessible
            if (boolInactive)
            {
                MessageType temp_VbStyle = (int)MessageType.Exclamation + MessageType.OkOnly;
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)77, tenderName, temp_VbStyle);
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                entered = 0;
                tenderedAmount = "";
                return null;
            }
            if (tenderMax != 0) // need to check only if it is not zero- otherwise no restriction
            {
                //Handled on UI
                if (entered != 0 & entered > tenderMax)
                {
                    tenderedAmount = tenderMax.ToString();
                    entered = tenderMax;
                }
            }
            decimal sumQa = 0;
            if (!string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && _policyManager.USE_LOYALTY
                && _policyManager.LOYAL_TYPE == "Points")
            {
                if (_policyManager.ALLOW_CUR_PT)
                {
                    sumQa = _saleManager.ComputePoints(sale);
                }
            }
            if (tenderMin != 0 & sale.Sale_Totals.Gross > 0) // shouldn't < minamount if there is a setting for tender
            {
                if (entered != 0 & entered < tenderMin)
                {
                    //MsgBox " Minimum amount required for the tender" & TenderName & " is " & TenderMin
                    capValue[1] = tenderName;
                    capValue[2] = tenderMin.ToString();
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)450, capValue, MessageType.OkOnly);
                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
            }
            // 
            bool displayNoReceiptButton;
            var arPay = CacheManager.GetArPayment(sale.Sale_Num);
            var payment = CacheManager.GetFleetPayment(sale.Sale_Num);
            var grossTotal = GetGrossTotal(transactionType, sale, payment, arPay, 0, out displayNoReceiptButton);
            tenders.Tend_Totals.Gross = grossTotal;
            tenders.Tend_Totals.Change = System.Math.Abs(grossTotal) - System.Math.Abs(tenders.Tend_Totals.Tend_Used);

            if (grossTotal > 0 & entered < 0)
            {
                // MsgBox "Negative Amounts are NOT Allowed for a Sale"
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)75, null, MessageType.OkOnly);
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }

            if (grossTotal < 0 & entered > 0)
            {
                // MsgBox "Positive Amounts are NOT Allowed for a Refund"
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)74, null, MessageType.OkOnly);
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }

            if (_policyManager.EMVVersion && (td.Tender_Class == "DBCARD" || td.Tender_Class == "CRCARD"))
            {
                sale.EMVVersion = true;
            }

            // If it is a points tender then we must limit the amount to the number
            // of points that the customer has in his account.
            if (td.Tender_Class == "POINTS")
            {
                var loyaltyLimit = Math.Round(_policyManager.LOYAL_LIMIT > 0 ?
                    modStringPad.MinVal(Convert.ToDouble(_policyManager.LOYAL_LIMIT),
                        modStringPad.MinVal(sale.Customer.Loyalty_Points + Convert.ToDouble(sumQa),
                            entered - Convert.ToDouble(_saleManager.SubPoints(sale)) / td.Exchange_Rate))
                    : modStringPad.MinVal(sale.Customer.Loyalty_Points + Convert.ToDouble(sumQa),
                        entered - Convert.ToDouble(_saleManager.SubPoints(sale)) / td.Exchange_Rate), 2);


                if (sale.Customer.Loyalty_Points <= 0 & sumQa == 0)
                {
                    MessageType temp_VbStyle2 = (int)MessageType.OkOnly + MessageType.Information;
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)70, null, temp_VbStyle2);
                    errorMessage.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    Set_Amount_Entered(ref tenders, ref sale, td, 0, 0);
                    return null;
                }
                if (loyaltyLimit < entered)
                {
                    MessageType temp_VbStyle3 = (int)MessageType.OkOnly + MessageType.Information;
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)85, loyaltyLimit, temp_VbStyle3);
                    errorMessage.StatusCode = HttpStatusCode.OK;
                    Set_Amount_Entered(ref tenders, ref sale, td, (decimal)entered, (decimal)loyaltyLimit);
                    tenderedAmount = td.Amount_Entered.ToString("0.00");
                    entered = (double)td.Amount_Entered;

                }
                else if (_policyManager.GIVE_POINTS)
                {
                    // Give points on products purchased with points
                    Set_Amount_Entered(ref tenders, ref sale, td, (decimal)entered, (decimal)(sale.Customer.Loyalty_Points + Convert.ToDouble(sumQa)));
                    tenderedAmount = td.Amount_Entered.ToString("0.00");
                    entered = (double)td.Amount_Entered;
                }
                else if (!_policyManager.GIVE_POINTS)
                {
                    // Don't give points on products purchased with points
                    Set_Amount_Entered(ref tenders, ref sale, td, (decimal)entered, (decimal)sale.Customer.Loyalty_Points);
                    tenderedAmount = td.Amount_Entered.ToString("0.00");
                    entered = (double)td.Amount_Entered;
                }
            }
            else
            {
                Set_Amount_Entered(ref tenders, ref sale, td, (decimal)entered, -1);
            }

            var tndr = GetSelectedTender(tenders, tenderCode);

            if (!_policyManager.EMVVersion && !tndr.Give_Change && Conversion.Val((entered * tndr.Exchange_Rate).ToString("0.00")) > System.Math.Abs(Conversion.Val(grossTotal.ToString("0.00"))))
            {
                // If we don't give change on this tender then we can't accept more than
                // the amount of the sale. Show_Message  "Amount Cannot Exceed value of sale."
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)88, null, MessageType.OkOnly);
                errorMessage.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return null;


            }
            if (!_policyManager.EMVVersion && Conversion.Val(tenders.Tend_Totals.No_Change_Total.ToString("0.00")) > System.Math.Abs(Conversion.Val(grossTotal.ToString("0.00"))))
            {
                // Similarly, the sum of all tenders on which no change is given cannot
                // exceed the amount of the sale.
                //Show_Message "'No Change' tenders Cannot Exceed value of sale."
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)87, null, MessageType.OkOnly);
                errorMessage.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return null;


            }
            //   for EMV version there is an issue when the customer asks for cash back and the cashier
            // selected debit but customer inserts a credit card instead. Card is approved, so it is to late for POS to do
            // the validation. Sale has to be completed to avoid the cashier shutting down the POS.
            // The issue was initial reported by GasKing
            if (_policyManager.EMVVersion)
            {
                if (!tndr.Give_Change && Conversion.Val((entered * tndr.Exchange_Rate).ToString("0.00")) > System.Math.Abs(Conversion.Val(grossTotal.ToString("0.00"))))
                {
                    // If we don't give change on this tender then we show_Message  "Amount Cannot Exceed value of sale."
                    // for EMV sale has to be completed as is because the cards are controlled by pinpad
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)88, null, MessageType.OkOnly);
                    errorMessage.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    WriteToLogFile("Cashier accepted a sale to be completed with cash back for tender that is set not to give cash back. Msg 88");
                    //in case of coupon , complete payment
                    if (tndr.Tender_Name != "COUPON")
                        return null;
                }
                if (!_policyManager.EMVVersion && Conversion.Val(tenders.Tend_Totals.No_Change_Total.ToString("0.00")) > System.Math.Abs(Conversion.Val(grossTotal.ToString("0.00"))))
                {
                    // Similarly, the sum of all tenders on which no change is given cannot
                    // exceed the amount of the sale show_Message "'No Change' tenders Cannot Exceed value of sale."
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)87, null, MessageType.OkOnly);
                    errorMessage.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    WriteToLogFile("Cashier accepted a sale to be completed with cash back for tender that is set not to give cash back. Msg 87");
                    //in case of coupon , complete payment
                    if (tndr.Tender_Name != "COUPON")
                        return null;
                }
            }

            // Passed the tests ... Accept the new tender amount.
            tenderedAmount = entered != 0 ? entered.ToString("###,##0.00") : "";
            used = Convert.ToDouble(tndr.Amount_Used);
            var usedAmount = used != 0 ? used.ToString("###,##0.00") : "";
            tenders.Summary1 = _resourceManager.GetResString(offSet, (short)163) + tenders.Tend_Totals.Tend_Used.ToString("###,##0.00"); //"Tendered: $"
            //   for penny adjustment
            if (sale.Sale_Totals.Penny_Adj != 0)
            {
                tenders.Summary1 = tenders.Summary1 + "\r\n" + _resourceManager.GetResString(offSet, (short)485) + sale.Sale_Totals.Penny_Adj.ToString("###,##0.00");
            }


            if (Conversion.Val(tenderedAmount) == 0)
            {
                if (Strings.UCase(Convert.ToString(tndr.Tender_Name)) == Strings.UCase(System.Convert.ToString(_policyManager.CouponTend)))
                {
                    tenders.Coupon = "";
                }
            }

            SetSummary2Informatiion(ref tenders, grossTotal);
            Register register = new Register();
            _mainManager.SetRegister(ref register, sale.Register);
            if (register.Customer_Display)
            {
                if (transactionType != "CashDrop")
                {
                    tenders.CustomerDisplay = _mainManager.DisplayMsgLcd(register,
                        _mainManager.FormatLcdString(register, "", tenders.Summary1), _mainManager.FormatLcdString(register, "", tenders.Summary2));
                }
            }


            if (transactionType == "Sale" || transactionType == "Delete Prepay") //saving only for sale type
            {
                _saleManager.Save_Tender_Temp(ref tenders, sale);
                // sale.Save_Tender_Temp();
            }
            // Activate the "Print Receipt" buttons if the sale is complete.
            if (grossTotal > 0) //And tenders.Tend_Totals.Tend_Used > 0 Then   ' 
            {

                if (Math.Round(Convert.ToDouble(tenders.Tend_Totals.Change), 2) > 0.001)
                {
                    tenders.DisplayNoReceiptButton = transactionType == "CashDrop";
                    tenders.EnableCompleteReceipt = transactionType == "CashDrop";
                    tenders.EnableCompletePayment = transactionType == "CashDrop";
                }
                else
                {
                    tenders.DisplayNoReceiptButton = Convert.ToBoolean(!_policyManager.PRINT_REC);
                    tenders.EnableCompleteReceipt = true;
                    tenders.EnableCompletePayment = true;
                }
            }
            //     End If ' up
            CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
            _saleService.SaveSale(sale.TillNumber, sale.Sale_Num, sale);
            return tenders;
        }

        /// <summary>
        /// Method to get gross total
        /// </summary>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="sale">Sale</param>
        /// <param name="payment">Fleet payment</param>
        /// <param name="arPayment">AR payment</param>
        /// <param name="prepayItem">Prepay item</param>
        /// <param name="displayNoReceiptButton">Display no receipt</param>
        /// <returns>Gros  total</returns>
        public decimal GetGrossTotal(string transactionType, Sale sale, Payment payment,
            AR_Payment arPayment, short prepayItem, out bool displayNoReceiptButton)
        {
            displayNoReceiptButton = true;
            decimal grossTotal = 0;

            if (transactionType == "Sale")
            {
                // Payment_Renamed is from a Sale
                grossTotal = sale.Sale_Totals.Gross;
                //   if any penny adjustment was done for the original sale POS has to return the money
                // Sale type at this point is "Sale" instead of "Return" so check the total gross to find a return
                if (grossTotal < 0)
                {
                    grossTotal = grossTotal + sale.Sale_Totals.Penny_Adj;
                    displayNoReceiptButton = false;
                    //tenders.Summary1 = tenders.Summary1 + "\r\n" + _resourceManager.GetResString(offSet,(short)485) + sale.Sale_Totals.Penny_Adj.ToString("###,##0.00");
                }
                //   end
            }
            else if (transactionType == "Payment")
            {
                // Payment_Renamed is from an ROA Payment_Renamed
                //Payment_Renamed.TopLeft = lblTopL;
                // Payment_Renamed.TopRight = lblTopR;
                grossTotal = System.Convert.ToDecimal(payment.Amount);
            }
            else if (transactionType == "ARPay")
            {
                // Payment is an AR Payment_Renamed
                grossTotal = arPayment.Amount;
                var saleType = new Sale_Type { SaleType = "ARPAY" };
                var arpayReceiptCopies = Convert.ToInt16(_policyManager.GetPol("PRINT_COPIES", saleType));
                //  ' Disable no receipt button if they defined  the number of copies _policyManager value for ARpay
                if (arpayReceiptCopies > 1)
                {
                    displayNoReceiptButton = false;
                }
            }
            else if (prepayItem != 0 && transactionType != "Delete Prepay")
            {
                displayNoReceiptButton = false;
            }
            else if (transactionType == "Delete Prepay")
            {
                displayNoReceiptButton = false;
            }
            return Math.Round(grossTotal, 2);
        }

        // ==================================================================================
        // Complete the sale and clean up your mess.
        // ==================================================================================
        /// <summary>
        /// Method to finish a sale
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="till">Till</param>
        /// <param name="issueSc">Issue store credit</param>
        /// <param name="error">Error</param>
        /// <param name="requireOpen">Require open</param>
        /// <param name="changeDue">Change due</param>
        /// <param name="isRefund">Refund sale</param>
        /// <returns>Report content</returns>
        public List<Report> Finishing_Sale(Tenders tenders, string transactionType, int saleNumber, string userCode,
            Till till, bool issueSc, out ErrorMessage error, out bool requireOpen, out string changeDue, out bool isRefund)
        {
            Store_Credit storeCredit = null;
            requireOpen = false;
            changeDue = "0.00";
            isRefund = false;
            var reports = new List<Report>();
            Tender tenderRenamed;
            var sale = _saleManager.GetCurrentSale(saleNumber, till.Number, 0, userCode, out error);
            var user = _loginManager.GetUser(userCode);
            _saleManager.ReCompute_CashBonus(ref sale);
            bool displayNoReceiptButton;
            AR_Payment arPay = null;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (transactionType == "ARPay")
            {
                arPay = GetARPayer(saleNumber, out error);
            }
            if (!string.IsNullOrEmpty(error.MessageStyle.Message) && transactionType == "ARPay")
            {
                return null;
            }
            var payment = GetFleetPayer(saleNumber, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message) && transactionType == "Payment")
            {
                return null;
            }
            error = new ErrorMessage();

            var grossTotal = GetGrossTotal(transactionType, sale, payment, arPay, 0, out displayNoReceiptButton);
            //tenders.DisplayNoReceiptButton = displayNoReceiptButton;
            isRefund = grossTotal < 0;
            //   if any penny adjustment was done for the original sale POS has to return the money
            // Sale type at this point is "Sale" instead of "Return" so check the total gross to find a return
            if (grossTotal < 0)
            {
                grossTotal = grossTotal + sale.Sale_Totals.Penny_Adj;
            }// - only we need cash bonus calculation only at the end to print or to save in the table
            if (grossTotal > 0)
            {
                // This should never activate because the "Complete Sale" buttons are not
                // enabled until enough has been paid ... but, since I'm paranoid ...
                if (Math.Round(Convert.ToDouble(tenders.Tend_Totals.Change), 2) > 0.0D)
                {
                    //Show_Message "Not Enough Paid"
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)98, null, MessageType.OkOnly);
                    error.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }

            }
            else
            {
                // Same comment here
                if (Math.Round(Convert.ToDouble(tenders.Tend_Totals.Change), 2) < 0.0D)
                {
                    //Show_Message  "Too Much Refunded "
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 99, null, MessageType.OkOnly);
                    error.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                if (Math.Round(Convert.ToDouble(tenders.Tend_Totals.Change), 2) > 0.0D)
                {
                    //Show_Message "Not Enough Refunded"
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 90, null, MessageType.OkOnly);
                    error.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
            }

            // Tell the user control to delete any Gift Certificates and/or Store_Renamed Credits that
            // were used in this sale.
            var scCredits = _tenderService.GetScCredit(sale.Sale_Num, till.Number);
            if (scCredits != null)
            {
                _tenderService.RemoveSc(saleNumber, scCredits);
            }
            var gcCredits = _tenderService.GetGcCredit(sale.Sale_Num, till.Number);
            if (gcCredits != null)
            {
                _tenderService.RemoveGc(saleNumber, gcCredits);
            }

            if (_policyManager.Store_Credit)
            {
                if (grossTotal < 0)
                {
                    // Issue a Store_Renamed credit on a refund.
                    foreach (Tender tempLoopVarTenderRenamed in tenders)
                    {
                        tenderRenamed = tempLoopVarTenderRenamed;
                        if (tenderRenamed.Amount_Used != 0 && tenderRenamed.Tender_Code == "SC")
                        {
                            var scReceipt = _receiptManager.Issue_Store_Credit(saleNumber, sale.Customer.Code, user,
                                 tenderRenamed, (float)Math.Abs(tenderRenamed.Amount_Used), out storeCredit);
                            reports.Add(scReceipt);
                            break;
                        }
                    }

                }
                else
                {
                    foreach (Tender tempLoopVarTenderRenamed in tenders)
                    {
                        tenderRenamed = tempLoopVarTenderRenamed;
                        if (tenderRenamed.Amount_Used != 0 && tenderRenamed.Tender_Code == "SC" && tenders.Tend_Totals.Change != 0)
                        {
                            // Change Handling for Store_Renamed Credits
                            var scPolicyManager = Convert.ToString(_policyManager.CRED_CHANGE);
                            if (scPolicyManager == "Always")
                            {
                                var scReceipt = _receiptManager.Issue_Store_Credit(saleNumber, sale.Customer.Code, user,
                                     tenderRenamed, (float)System.Math.Abs(tenders.Tend_Totals.Change)
                                     , out storeCredit);
                                tenders.Tend_Totals.Change = 0;
                                reports.Add(scReceipt);
                                break;
                            }
                            if (scPolicyManager == "Choice")
                            {
                                if (issueSc)
                                {
                                    var scReceipt = _receiptManager.Issue_Store_Credit(saleNumber, sale.Customer.Code,
                                        user, tenderRenamed, (float)System.Math.Abs(tenders.Tend_Totals.Change)
                                        , out storeCredit);
                                    reports.Add(scReceipt);
                                    tenders.Tend_Totals.Change = 0;
                                    break;
                                }
                            }
                        }
                        else if (tenderRenamed.Amount_Used != 0 && tenderRenamed.Tender_Class == "GIFTCERT" && tenders.Tend_Totals.Change != 0)
                        {

                            // Change Handling for Gift Certificates
                            if (_policyManager.GIFTCERT)
                            {
                                var gcPolicyManager = Convert.ToString(_policyManager.GC_CHANGE);

                                if (gcPolicyManager == "Always")
                                {
                                    var scReceipt = _receiptManager.Issue_Store_Credit(saleNumber, sale.Customer.Code, user, tenderRenamed,
                                          (float)System.Math.Abs(tenders.Tend_Totals.Change), out storeCredit);
                                    tenders.Tend_Totals.Change = 0;
                                    reports.Add(scReceipt);
                                    break;
                                }
                                if (gcPolicyManager == "Choice")
                                {
                                    if (issueSc)
                                    {
                                        var scReceipt = _receiptManager.Issue_Store_Credit(saleNumber, sale.Customer.Code,
                                            user, tenderRenamed, (float)Math.Abs(tenders.Tend_Totals.Change), out storeCredit);
                                        tenders.Tend_Totals.Change = 0;
                                        reports.Add(scReceipt);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (Tender tempLoopVarTender in tenders)
            {
                tenderRenamed = tempLoopVarTender;
                if (tenderRenamed.Amount_Used != 0)
                {
                    switch (tenderRenamed.Tender_Class)
                    {
                        case "ACCOUNT":
                            sale.Customer.Current_Balance = sale.Customer.Current_Balance + Convert.ToDouble(tenderRenamed.Amount_Used);
                            break;

                        case "COUPON":
                            var SVC = CacheManager.GetSaleVendorCoupon(saleNumber, tenderRenamed.Tender_Code);
                            _saleService.SaveSaleForSaleVendorCoupon(SVC, sale.Sale_Num, sale.TillNumber);
                            break;


                        case "GIFTCERT":
                            string temp_Policy_Name = "GiftTender";
                            if ((string)_policyManager.GetPol(temp_Policy_Name, tenderRenamed) == "EKO")
                            {
                                _tenderService.SaveToTill(sale.Sale_Num, sale.TillNumber);
                            }
                            break;

                    }
                }

            }

            // Reset the cash amount in the Till_Renamed drawer.
            var baseCurr = Convert.ToString(_policyManager.BASECURR);
            if (!string.IsNullOrEmpty(baseCurr) && grossTotal != 0)
            {
                _tillService.UpdateCash(till.Number, tenders[baseCurr].Amount_Used + tenders.Tend_Totals.Change);
            }

            // Determine if we should open the cash drawer.
            if (tenders.Tend_Totals.Change != 0 || _policyManager.OPEN_DRAWER == "Every Sale")
            {
                requireOpen = true;
            }
            else
            {
                foreach (Tender tempLoopVarTenderRenamed in tenders)
                {
                    tenderRenamed = tempLoopVarTenderRenamed;
                    if (tenderRenamed.Amount_Used != 0 && tenderRenamed.Open_Drawer)
                    {
                        requireOpen = true;
                        break;
                    }

                }
            }

            if (_policyManager.TAX_EXEMPT)
            {



                if (_policyManager.TE_Type == "SITE")
                {
                    var oPurchaseList = CacheManager.GetPurchaseListSaleForTill(till.Number, saleNumber);

                    if (oPurchaseList == null)
                    {
                        oPurchaseList = _purchaseListManager.GetPurchaseList(saleNumber, till.Number,userCode, sale.TreatyNumber,sale.TreatyName, out error);
                    }

               
                    //shiny Jan2010 - Electronic signature capture for squamish
                    if (oPurchaseList?.Count() > 0)
                    {
                        if (_policyManager.TE_SIGNATURE && Strings.UCase(Convert.ToString(_policyManager.TE_SIGNMODE)) == "READER")
                        {

                            WriteToLogFile("Control is back to SaleTend form, Finishing_Sale procedure.");
                        }
                    }
                    //  -Electronic signature capture                    
                    if (oPurchaseList?.Count() > 0)
                    {
                        if (!_purchaseListManager.SaveAndAssignToQuotas(ref oPurchaseList, user, till))
                        {
                            MessageType temp_VbStyle3 = (int)MessageType.Critical + MessageType.OkOnly;
                            _resourceManager.CreateMessage(offSet, 14, (short)53, oPurchaseList.GetLastError(), temp_VbStyle3);
                        }
                    }
                }
                else
                {
                    var oTeSale = CacheManager.GetTaxExemptSaleForTill(till.Number, saleNumber);
                    if (oTeSale != null)
                    {
                        if (oTeSale.Te_Sale_Lines.Count > 0)
                        {
                            oTeSale.Sale_Time = DateTime.Now;

                            var taxExemptReceipt = _receiptManager.PrintTaxExemptVoucher(oTeSale);
                            reports.Add(taxExemptReceipt);
                            sale.TotalTaxSaved = oTeSale.TotalExemptedTax;
                            _taxExemptSaleManager.SaveSale(oTeSale);
                        }
                        else if (oTeSale.teCardholder.GstExempt || (sale.TreatyNumber != "" && oTeSale.TaxCreditLines.Count > 0))
                        {
                            _taxExemptSaleManager.SaveSale(oTeSale);
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(tenders.Coupon))
            {
                _tenderService.UpdateCoupon(tenders.Coupon);
            }

            foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
            {
                var Sl = tempLoopVar_SL;
                if (Sl.IsCarwashProduct)
                {
                    if (Sl.Quantity > 0)
                    {
                        if (_policyManager.IsCarwashSupported && _policyManager.IsCarwashIntegrated)
                        {
                            _carwashManager.GetCarwashCode();
                        }
                    }
                    else
                    {
                         Chaps_Main.SA = sale;
                        _carwashManager.RefundCarwash();
                    }
                }
            }


            var tempFileName = "";
            var reprint = false;
            Stream signature;
            var receipt = _receiptManager.Print_Receipt(sale.TillNumber, storeCredit, ref sale, ref tenders, false,
                ref tempFileName, ref reprint, out signature, userCode);
            if (!(receipt.Copies > 1))
            {
                receipt.Copies = sale.Sale_Totals.Gross < 0 ? _policyManager.RefundReceiptCopies : 1;
            }
            reports.Add(receipt);

            //todo add print kickback method here
            //check if required to print, then return a receipt

            if (_policyManager.Use_KickBack && sale.Customer.PointCardNum != "" && !reprint)
            {
                var kickbackReceipt = _receiptManager.Print_Kickback(sale);



                reports.Add(kickbackReceipt);
            }


            WriteToLogFile("After Print_Receipt in Finishing_Sale");
            sale.Sale_Type = sale.Sale_Totals.Gross > 0 ? "SALE" : "REFUND";
            _saleManager.SaveSale(sale, userCode, ref tenders, storeCredit);
            changeDue = tenders.Tend_Totals.Change < 0 ? (-1 * tenders.Tend_Totals.Change).ToString("#0.00") : "0.00";
            return reports;
        }


        /// <summary>
        /// Method to complete AR payment
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="till">Till</param>
        /// <param name="issueSc">Issue store credit</param>
        /// <param name="requireOpen">Require open</param>
        /// <param name="changeDue">Change due</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Report content</returns>
        public List<Report> Finishing_ARPay(Tenders tenders, int saleNumber, string userCode,
            Till till, bool issueSc, out bool requireOpen, out string changeDue,
            out ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();
            Tender T;
            Tender tenderRenamed = default(Tender);
            requireOpen = false;
            var reports = new List<Report>();
            changeDue = "0.00";
            var arPay = GetARPayer(saleNumber, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (Math.Round(Convert.ToDouble(tenders.Tend_Totals.Change), 2) > 0.0D)
            {
                //        Show_Message "Not Enough Paid"
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)98, null, MessageType.OkOnly);
                return null;
            }
            var scCredits = _tenderService.GetScCredit(saleNumber, till.Number);
            if (scCredits != null)
            {
                _tenderService.RemoveSc(saleNumber, scCredits);
            }
            var gcCredits = _tenderService.GetGcCredit(saleNumber, till.Number);
            if (gcCredits != null)
            {
                _tenderService.RemoveGc(saleNumber, gcCredits);
            }

            Store_Credit sc = null;
            var user = _loginManager.GetUser(userCode);
            if (_policyManager.Store_Credit)
            {
                foreach (Tender tempLoopVarTender in tenders)
                {
                    tenderRenamed = tempLoopVarTender;
                    if (tenderRenamed.Amount_Used != 0 && tenderRenamed.Tender_Code == "SC" && tenders.Tend_Totals.Change != 0)
                    {

                        var scPolicyManager = Convert.ToString(_policyManager.CRED_CHANGE);
                        if (scPolicyManager == "Always")
                        {
                            var scReceipt = _receiptManager.Issue_Store_Credit(saleNumber, arPay.Customer.Code, user, tenderRenamed,
                                 (float)Math.Abs(tenders.Tend_Totals.Change), out sc);
                            tenders.Tend_Totals.Change = 0;
                            reports.Add(scReceipt);
                        }
                        else if (scPolicyManager == "Choice")
                        {
                            if (issueSc)
                            {
                                var scReceipt = _receiptManager.Issue_Store_Credit(saleNumber, arPay.Customer.Code, user, tenderRenamed,
                                      (float)Math.Abs(tenders.Tend_Totals.Change), out sc);
                                tenders.Tend_Totals.Change = 0;
                                reports.Add(scReceipt);
                            }
                        }
                        break;
                    }
                    if (tenderRenamed.Amount_Used != 0 && tenderRenamed.Tender_Class == "GIFTCERT" && tenders.Tend_Totals.Change != 0)
                    {

                        if (_policyManager.GIFTCERT)
                        {
                            var gcPolicyManager = Convert.ToString(_policyManager.GC_CHANGE);

                            if (gcPolicyManager == "Always")
                            {
                                var scReceipt = _receiptManager.Issue_Store_Credit(saleNumber, arPay.Customer.Code, user, tenderRenamed,
                                    (float)Math.Abs(tenders.Tend_Totals.Change), out sc);
                                tenders.Tend_Totals.Change = 0;
                                reports.Add(scReceipt);
                            }
                            else if (gcPolicyManager == "Choice")
                            {
                                if (issueSc)
                                {
                                    var scReceipt = _receiptManager.Issue_Store_Credit(saleNumber, arPay.Customer.Code, user, tenderRenamed,
                                        (float)Math.Abs(tenders.Tend_Totals.Change), out sc);
                                    tenders.Tend_Totals.Change = 0;
                                    reports.Add(scReceipt);
                                }
                            }
                            break;
                        }
                    }
                }
            }
            SaveArPayment(saleNumber, till.Number, userCode, tenders, sc);
            var baseCurr = Convert.ToString(_policyManager.BASECURR);
            if (!string.IsNullOrEmpty(baseCurr))
            {
                _tillService.UpdateCash(till.Number, (tenders[baseCurr].Amount_Used + tenders.Tend_Totals.Change));
            }

            if (tenders.Tend_Totals.Change != 0 || _policyManager.OPEN_DRAWER == "Every Sale")
            {
                requireOpen = true;
            }
            else
            {
                foreach (Tender tempLoopVarTenderRenamed in tenders)
                {
                    tenderRenamed = tempLoopVarTenderRenamed;
                    if (tenderRenamed.Amount_Used != 0 && tenderRenamed.Open_Drawer)
                    {
                        requireOpen = true;
                        break;
                    }
                }
            }

            CacheManager.DeleteArPayment(saleNumber);
            CacheManager.DeleteTendersForArPay(saleNumber);
            var arReceipt = _receiptManager.Print_ARPay(arPay, userCode, till, tenders);
            arReceipt.Copies = _policyManager.ArpayReceiptCopies;
            reports.Add(arReceipt);
            return reports;
        }

        /// <summary>
        /// Method to load temporary tenders
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="till">Till</param>
        /// <param name="sale">Sale</param>
        public void Load_Temp_Tenders(ref Tenders tenders, Till till, ref Sale sale)
        {
            var tempTenders = CacheManager.GetTenderForSale(sale.Sale_Num, till.Number);
            if (tempTenders != null)
            {
                tenders = tempTenders;
                return;
            }
            double used;
            double entered = 0;
            Tender td = default(Tender);
            used = System.Convert.ToDouble(entered == 0);
            var tend = new Tenders();
            var saleTenders = _saleService.GetSaleTendersFromDbTemp(sale.Sale_Num, till.Number);
            foreach (var saleTend in saleTenders)
            {
                if (td != null)
                {

                    td.Amount_Entered = Convert.ToDecimal(saleTend.AmountTend);
                    td.Amount_Used = Convert.ToDecimal(saleTend.AmountUsed);
                    Set_Amount_Entered(ref tenders, ref sale, td, td.Amount_Entered, -1);
                    td.AuthUser = saleTend.AuthUser;
                    tenders.Tend_Totals.Tend_Used = tenders.Tend_Totals.Tend_Used + Convert.ToDecimal(saleTend.AmountUsed);

                    if (td.Tender_Class == "COUPON")
                    {
                        SaleVendorCoupon svc = new SaleVendorCoupon();
                        var saleVendorCouponLines = _saleService.GetSaleVendorCoupons(sale.Sale_Num, till.Number);
                        svc.Sale_Num = sale.Sale_Num;
                        foreach (var line in saleVendorCouponLines)
                        {
                            _svcManager.Add_a_Line(ref svc, till.Number, line, false);
                        }
                        CacheManager.AddSaleVendorCoupon(sale.Sale_Num, td.Tender_Code, svc);
                    }

                    string temp_Policy_Name = "GiftTender";
                    if (td.Tender_Class == "GIFTCERT" && (string)_policyManager.GetPol(temp_Policy_Name, td) == "EKO" && _policyManager.ThirdParty)
                    {
                        var gcPayment = _tenderService.Load_GCTenders(till.Number, sale.Sale_Num, DataSource.CSCCurSale);
                        if (gcPayment.GC_Lines.Count > 0)
                        {
                            tenders.Card_Authorized = true;
                        }
                    }

                    if (td.Tender_Class == "CRCARD" || td.Tender_Class == "FLEET"
                        || td.Tender_Class == "DBCARD" || td.Tender_Class == "GIFTCARD"
                        || td.Tender_Class == "THIRDPARTY" || td.Tender_Class == "LOYALTY"
                        || td.Tender_Class == "ACCOUNT") // 
                    {

                        var cardtd = _saleService.GetCardTenderFromDbTemp(sale.Sale_Num, till.Number, saleTend.TenderName);
                        //TODO: encrypt card number                                                                                                                                                                                                                                   //
                        // td.Credit_Card.Cardnumber = EncryptDecryptUtility.DecryptText()
                        if (cardtd != null && cardtd.Credit_Card.Crd_Type == "L")
                        {
                            Variables.MillExchangeAmount = td.Credit_Card.Trans_Amount;
                            Variables.AccumulateCard = td.Credit_Card.Cardnumber;
                            Variables.MillExchangeCard = Variables.AccumulateCard;
                        }
                        //else if (cardtd != null && cardtd.Credit_Card.Crd_Type == "T")
                        //{
                        //}
                        tenders.Card_Authorized = true;

                        td.Credit_Card = cardtd?.Credit_Card ?? new Credit_Card();
                    }
                    CacheManager.AddTendersForSale(sale.Sale_Num, till.Number, tenders);
                }
            }
        }

        /// <summary>
        /// Method to cancel tenders
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="transactionType">Trsanction type</param>
        /// <param name="errorMessage">Error</param>
        public Sale CancelTender(int saleNumber, int tillNumber, string userCode, string transactionType,
            out ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();
            WriteToLogFile("Cancel Tenders event");
            if (transactionType == "DeletePrepay") transactionType = "Delete Prepay";
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            // 
            CustomerDisplay lcdMsg = new CustomerDisplay();
            var register = new Register();
            _mainManager.SetRegister(ref register, sale.Register);
            if (register.Customer_Display)
            {
                lcdMsg = _mainManager.DisplayMsgLcd(register, _mainManager.FormatLcdString(register,
                    _resourceManager.GetResString(offSet, (short)453), ""), "");
            }
            if (transactionType == "CashDrop")
            {
                CacheManager.DeleteTendersForCashDrop(transactionType, "SAFE");
                CacheManager.DeleteTendersForCashDrop(transactionType, "ATM");
                sale.CustomerDisplay = lcdMsg;
                return sale;
            }
            var tenders = GetAllTender(saleNumber, tillNumber, transactionType,
                userCode, false, "", out errorMessage);
            if (errorMessage?.MessageStyle?.Message != null)
            {
                return null;
            }

            if (tenders.Card_Authorized)
            {
                //TIMsgbox "You have received Authorization on a Credit or Debit Card" & vbCrLf & vbCrLf & _
                //"You cannot cancel allTenders after a card is authorized.", vbCritical + vbOKOnly, _
                //"Cannot Cancel allTenders", Me
                //  - if card_authorized flag is true and amount is not used then switch the flag to false
                if (tenders.Tend_Totals.Tend_Used == 0)
                {
                    tenders.Card_Authorized = false;
                }
                else
                {
                    MessageType temp_VbStyle = (int)MessageType.Critical + MessageType.OkOnly;
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)96, null, temp_VbStyle);
                    return null;
                }
            }

            if (_policyManager.CheckUpsell)
            {
                sale.Upsell = true;
            }

            if (transactionType == "Delete Prepay")
            {


                var deletePrepayPumpID = sale.Sale_Lines[1].pumpID;
                if (deletePrepayPumpID != 0)
                {

                    CancelHoldingDeletingPrepayFromFC(deletePrepayPumpID, out errorMessage);
                    if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                    {
                        return null;
                    }
                }
                CacheManager.DeleteCurrentSaleForTill(tillNumber, saleNumber);
                _saleService.RemoveTempDataInDbTill(tillNumber, saleNumber);
            }

            if (!string.IsNullOrEmpty(sale.CouponID))
            {
                _tenderService.SetCouponToVoid(sale.CouponID);
            }

            if (transactionType == "Sale")
            {
                Clear_TaxExempt(saleNumber, tillNumber, userCode, out errorMessage);
            }
            //  
            if (!_policyManager.USE_FUEL)
            {
                Chaps_Main.strFunctionTend = "";
                Chaps_Main.cashIndex = 0;
            }
            var couponTenders = tenders.Where(t => t.Tender_Class == "COUPON").ToList();
            foreach (var couponTender in couponTenders)
            {
                CacheManager.DeleteSaleVendorCoupon(saleNumber, couponTender.Tender_Code);
            }
            // 
            _saleService.ClearTenderRecordsFromDbTemp(tillNumber, saleNumber);//   crash recovery

            _saleService.CheckPaymentsFromDbTemp(saleNumber, tillNumber);
            CacheManager.DeleteTendersForSale(saleNumber, tillNumber);
            if (transactionType == "ARPay")
            {
                CacheManager.DeleteArPayment(saleNumber);
                CacheManager.DeleteTendersForArPay(saleNumber);
            }
            if (transactionType == "Payment")
            {
                CacheManager.DeleteFleetPayment(saleNumber);
                CacheManager.DeleteTendersForPayment(saleNumber);
            }
            _tenderService.RemoveUsedGc(saleNumber);
            _tenderService.RemoveUsedSc(saleNumber);
            bool isProcessing = true;
            WriteToLogFile("cmdCancel click event, after unload me line. IsProcessing is " + Convert.ToString(isProcessing));
            isProcessing = false;
            WriteToLogFile("cmdCancel click event. IsProcessing should be false. The value is " + Convert.ToString(isProcessing));
            //shi ny adding may21,2010 - if canceling tender- need to reverse customer selected  - since  we are not keeping previous customer info - reverse to cash sale
            if (string.IsNullOrEmpty(sale.TreatyNumber))
            {
                if (sale.TECustomerChange)
                {
                    sale.Customer = _customerManager.LoadCustomer(Utilities.Constants.CashSaleClient);
                    //  Tookout refreshlines from customer_change( because it is causing screen flip & freezing in SITE screen)
                }
            }
            //   to reset the tax exempt customer for QITE
            if (_policyManager.TE_Type == "QITE" && sale.Customer.TaxExempt)
            {
                if (sale.Customer.DiscountType == "D")
                {
                    sale.Customer = _customerManager.LoadCustomer(Utilities.Constants.CashSaleClient);
                    Sale_Line sl;
                    foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                    {
                        sl = tempLoopVarSl;
                        if (sl.Discount_Type == "$" && sl.ProductIsFuel)
                        {
                            // SL.Discount_Rate = 0;
                            _saleLineManager.SetDiscountRate(ref sl, 0);
                        }
                    }
                }
                else
                {
                    sale.Customer = _customerManager.LoadCustomer(Utilities.Constants.CashSaleClient);
                }
                _saleManager.ReCompute_Totals(ref sale);
            }
            if (transactionType == "Delete Prepay")
            {
                sale = _saleManager.InitializeSale(tillNumber, sale.Register, userCode, out errorMessage);
            }
            CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
            sale.CustomerDisplay = lcdMsg;
            return sale;
        }


        /// <summary>
        /// Method to get gift certificates
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="amountEntered">Amount entered</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>List of gift certificates</returns>
        public List<GiftCert> GetGiftCertificates(int saleNumber, int tillNumber, string userCode, string tenderCode,
            string amountEntered, string transactionType, out ErrorMessage errorMessage)
        {
            var allTenders = GetAllTender(saleNumber, tillNumber, transactionType, userCode, false, string.Empty,
                out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            bool displayNoReceiptButton;
            var arPay = GetARPayer(saleNumber, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message) && transactionType == "ARPay")
            {
                return null;
            }
            var payment = GetFleetPayer(saleNumber, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message) && transactionType == "Payment")
            {
                return null;
            }
            var grossTotal = GetGrossTotal(transactionType, sale, payment, arPay, 0, out displayNoReceiptButton);
            List<Report> transactReports;
            var cardSummary = SaleTend_Keydown(ref allTenders, sale, userCode, tenderCode,
              ref amountEntered, transactionType, grossTotal, null, out transactReports, out errorMessage);
            return cardSummary?.GiftCerts;
        }


        /// <summary>
        /// Method to get store credits
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="amountEntered">Amount entered</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>List of store credits</returns>
        public List<Store_Credit> GetStoreCredits(int saleNumber, int tillNumber, string userCode, string tenderCode,
            string amountEntered, string transactionType, out ErrorMessage errorMessage)
        {
            var allTenders = GetAllTender(saleNumber, tillNumber, transactionType, userCode, false, string.Empty,
                 out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            bool displayNoReceiptButton;
            var arPay = GetARPayer(saleNumber, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message) && transactionType == "ARPay")
            {
                return null;
            }
            var payment = GetFleetPayer(saleNumber, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message) && transactionType == "Payment")
            {
                return null;
            }
            var grossTotal = GetGrossTotal(transactionType, sale, payment, arPay, 0, out displayNoReceiptButton);
            List<Report> transactReports;
            var cardSummary = SaleTend_Keydown(ref allTenders, sale, userCode, tenderCode,
              ref amountEntered, transactionType, grossTotal, null, out transactReports, out errorMessage);
            return cardSummary?.StoreCredits;
        }

        /// <summary>
        /// Method to update tenders with keypad event
        /// </summary>
        /// <param name="allTenders">Tenders</param>
        /// <param name="sale">Sale</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="amountEntered">Amount entered</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="grossTotal">Gross total</param>
        /// <param name="cc">Credit card</param>
        /// <param name="transactionReports"></param>
        /// <param name="errorMessage">Error</param>
        /// <param name="makePayment">Payment</param>
        /// <param name="purchaseOrder">Purchase order</param>
        /// <param name="overrideArLimit">Override AR limit</param>
        /// <returns>Card summary</returns>
        public CardSummary SaleTend_Keydown(ref Tenders allTenders, Sale sale, string userCode, string tenderCode, ref string amountEntered,
            string transactionType, decimal grossTotal, Credit_Card cc, out List<Report> transactionReports, out ErrorMessage errorMessage, bool makePayment = false,
            string purchaseOrder = null, bool overrideArLimit = false, bool isUpdateTender = false, bool isAmountEnteredManually = false)
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var cardSummary = new CardSummary();
            var promptMessages = new List<string>();
            errorMessage = new ErrorMessage();
            transactionReports = null;
            short KeyCode = (short)13;
            short n;
            string cardType;
            object[] capValue = new object[3];
            string strGiftTender;
            string strTend = "";
            string strCardNumber = "";
            string strCardType = "";
            string strCardName = "";
            string strCardLanguage = "";
            bool blCardSwiped = false;
            string strExpiryDate = "";
            string strTendClass = "";
            bool TenderAutorecognition = false;
            double TenderMin = 0;
            double TenderMax = 0;
            string TenderName = "";
            bool boolInactive = false;

            strGiftTender = "";

            bool Card_Cancelled = false;
            string txtCardNum;
            string txtExpiryDate;
            var till = _tillService.GetTill(sale.TillNumber);

            object[] messages = new object[3];
            var selectedTender = GetSelectedTender(allTenders, tenderCode);

            if (selectedTender == null)
            {
                errorMessage.MessageStyle = new MessageStyle { Message = "Invalid tender code" };
                return null;
            }
            selectedTender.Credit_Card = cc;
            string cBuf = "";
            Tender TempTend = new Tender();
            string TrType = "";
            decimal TAmt = new decimal();
            int TMod = 0;
            string[] arrMsg = new string[3];
            bool IsEnter = false;
            switch (KeyCode)
            {

                case (short)System.Windows.Forms.Keys.Delete: // Delete the amount on a line
                    amountEntered = "";
                    if (Strings.UCase(System.Convert.ToString(selectedTender.Tender_Class)) == "COUPON")
                    {
                        //TODO:
                        // RemoveVendorCouponForTender(System.Convert.ToString(selectedTender.Tender_Name), "", (short)0);
                    }

                    if (Strings.UCase(System.Convert.ToString(selectedTender.Tender_Class)) == "ACCOUNT")
                    {
                        selectedTender.Credit_Card = null;
                    }
                    List<Report> transactReports = null;
                    UpdateTenders(sale.Sale_Num, sale.TillNumber, transactionType, userCode, false, tenderCode, amountEntered, out transactReports, out errorMessage);
                    break;

                case (short)System.Windows.Forms.Keys.Back:

                    amountEntered = "";


                    if (Strings.UCase(System.Convert.ToString(selectedTender.Tender_Class)) == "COUPON")
                    {
                        //TODO:
                        //RemoveVendorCouponForTender(System.Convert.ToString(selectedTender.Tender_Name), "", (short)0);
                    }
                    if (Strings.UCase(System.Convert.ToString(selectedTender.Tender_Class)) == "ACCOUNT")
                    {
                        selectedTender.Credit_Card = null;
                    }
                    UpdateTenders(sale.Sale_Num, sale.TillNumber, transactionType, userCode, false, tenderCode, amountEntered, out transactReports, out errorMessage);
                    break;

                case (short)System.Windows.Forms.Keys.Return:
                    IsEnter = true;
                    if (cc == null)
                        cc = new Credit_Card();
                    TenderAutorecognition = false;
                    TenderAutorecognition = System.Convert.ToBoolean(cc.AutoRecognition);
                    if (sale.ForCorrection && transactionType.ToUpper() == "SALE") //  - In between correction , if there is CashDrop_Renamed or Payment_Renamed happend system showing this message
                    {
                        strTend = OriginalPayment_Tender(sale, ref strCardNumber, ref strTendClass).Trim();
                        if ((selectedTender.Tender_Name != strTend && !_policyManager.COMBINECR) || (selectedTender.Tender_Class != strTendClass && _policyManager.COMBINECR))
                        {
                            MessageType temp_VbStyle = (int)MessageType.Critical + MessageType.OkOnly;
                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)54, strTend, temp_VbStyle);
                            IsEnter = false;
                            return null; //  -- Datawire Integration ( added by Dmitry)
                        }
                        else
                        {
                            object temp_ExpiryDate = strExpiryDate;
                            if (GetOriginalCardTend(sale, ref strCardName, ref strCardType, ref strCardLanguage, ref blCardSwiped, ref temp_ExpiryDate))
                            {

                            }
                            else
                            {
                                MessageType temp_VbStyle2 = (int)MessageType.Critical + MessageType.OkOnly;
                                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)55, null, temp_VbStyle2);
                                errorMessage.StatusCode = HttpStatusCode.OK;
                            }
                        }
                    }

                    if (selectedTender.Tender_Class == "GIFTCERT")
                    {
                        string temp_Policy_Name = "GiftTender";
                        strGiftTender = System.Convert.ToString(_policyManager.GetPol(temp_Policy_Name, selectedTender));
                    }

                    if (Strings.UCase(System.Convert.ToString(selectedTender.Tender_Name)) == Strings.UCase(System.Convert.ToString(_policyManager.CouponTend)))
                    {
                        if (grossTotal > 0)
                        {
                        }
                        IsEnter = false;
                        //return null;
                    }

                    if (Strings.UCase(Convert.ToString(selectedTender.Tender_Class)) == "COUPON")
                    {
                        if (grossTotal > 0)
                        {

                        }
                        IsEnter = false;
                    }

                    if (selectedTender.Tender_Class == "CRCARD")
                    {
                        selectedTender.Credit_Card.Crd_Type = "C";
                        cardSummary.SelectedCard = CardForm.Credit;
                        cardSummary.TenderClass = selectedTender.Tender_Class;
                    }
                    if (selectedTender.Tender_Class == "DBCARD")
                    {
                        selectedTender.Credit_Card.Crd_Type = "D";
                        cardSummary.SelectedCard = CardForm.Debit;
                        cardSummary.TenderClass = selectedTender.Tender_Class;
                    }
                    if (selectedTender.Tender_Class == "FLEET")
                    {
                        selectedTender.Credit_Card.Crd_Type = "F";
                        cardSummary.SelectedCard = CardForm.Fleet;
                        cardSummary.TenderClass = selectedTender.Tender_Class;
                    }
                    if (selectedTender.Tender_Class == "ACCOUNT")
                    {
                        cardSummary.SelectedCard = CardForm.Account;
                        cardSummary.TenderClass = selectedTender.Tender_Class;
                    }
                    if (selectedTender.Tender_Class == "THIRDPARTY")
                    {
                        selectedTender.Credit_Card.Crd_Type = "T";
                        cardSummary.TenderClass = selectedTender.Tender_Class;
                    }
                    string s = selectedTender.Tender_Class.Trim().ToString();
                    if ((s == "LOYALTY" || selectedTender.Tender_Class == "GIFTCARD") && (selectedTender.Tender_Code == "ACK" || selectedTender.Tender_Code == "ACKG"))
                    {
                        selectedTender.Credit_Card.Trans_Amount = (float)(Conversion.Val(amountEntered));

                        if (selectedTender.Tender_Class != "LOYALTY")
                        {
                            selectedTender.Tender_Class = "LOYALTY";
                        }
                        selectedTender.Credit_Card.Trans_Date = DateAndTime.Today;
                        selectedTender.Credit_Card.Trans_Time = DateAndTime.TimeOfDay;

                    }
                    else if (selectedTender.Tender_Class == "LOYALTY" && selectedTender.Credit_Card.GiftType != "A")
                    {
                        if (_policyManager.Use_KickBack)
                        {
                            selectedTender.Credit_Card.Crd_Type = "K";
                            // question
                            //  if (sale.Customer.Points_Redeemed == 0)

                            if (sale.Customer.Points_Redeemed == 0 && !makePayment)
                            {
                                MessageType temp_VbStyle3 = (int)MessageType.Exclamation + MessageType.OkOnly;
                                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)35, null, temp_VbStyle3);
                                amountEntered = "";
                                IsEnter = false;
                                return null;
                            }

                            double amountEnteredByUser = 0D;
                            double.TryParse(amountEntered, out amountEnteredByUser);


                            if (sale.Customer.Points_Redeemed != 0 &&
                                KickBackManager.ExchangeRate != 0 &&
                                sale.Customer.Balance_Points < ((int)(amountEnteredByUser / KickBackManager.ExchangeRate)))
                            {
                                errorMessage.MessageStyle.Message =
                                    $"You cannot use more than {sale.Customer.Balance_Points} points.";
                                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                            }
                            else if (selectedTender.Tender_Code == "KICKBACK" && selectedTender.Amount_Used == 0 
                                && isUpdateTender && sale.Customer.Points_Redeemed == 0 && isAmountEnteredManually)
                            {
                                errorMessage.MessageStyle.Message =
                                     "Cannot use this tender";
                                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                            }
                            else
                            {
                                sale.Customer.Points_Redeemed = amountEnteredByUser / KickBackManager.ExchangeRate;
                             //   sale.Customer.Balance_Points -= sale.Customer.Points_Redeemed;
                            }

                        }
                        else
                        {
                            selectedTender.Credit_Card.Crd_Type = "L";
                        }
                    }

                    if (selectedTender.Tender_Class == "GIFTCARD")
                    {
                        selectedTender.Credit_Card.Crd_Type = "G";

                        if (grossTotal > 0 && Conversion.Val(amountEntered) < 0)
                        {
                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)75, null, MessageType.OkOnly);

                            IsEnter = false;
                            return null;
                        }

                        if (grossTotal < 0 && Conversion.Val(amountEntered) > 0)
                        {
                            _resourceManager.CreateMessage(offSet, 14, (short)74, null, MessageType.OkOnly);

                            IsEnter = false;
                            return null;
                        }

                        if (!selectedTender.Give_Change && Conversion.Val(allTenders.Tend_Totals.Tend_Used) == 0 && System.Math.Abs(Convert.ToDecimal(selectedTender.Amount_Entered)) > System.Math.Abs(grossTotal) - System.Math.Abs(allTenders.Tend_Totals.Tend_Used))
                        {
                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)88, null, MessageType.OkOnly);

                            IsEnter = false;
                            return null;
                        }

                        if (System.Math.Abs(grossTotal) - System.Math.Abs(allTenders.Tend_Totals.Tend_Used) <= 0)
                        {
                            amountEntered = "";

                            IsEnter = false;
                            return null;
                        }

                        if (Math.Abs(grossTotal) - Math.Abs(allTenders.Tend_Totals.Tend_Used) > 0)
                        {
                            if (!TenderAutorecognition)
                            {
                                selectedTender.Credit_Card = new Credit_Card();
                            }
                            cardType = System.Convert.ToString(selectedTender.Tender_Class);
                            if (Conversion.Val(amountEntered) == 0)
                            {
                                if (grossTotal > 0)
                                {
                                    amountEntered = (grossTotal - allTenders.Tend_Totals.Tend_Used).ToString("##0.00");
                                }
                                else
                                {
                                    amountEntered = ((System.Math.Abs(grossTotal) - System.Math.Abs(allTenders.Tend_Totals.Tend_Used)) * -1).ToString("##0.00");
                                }
                            }
                            else if (grossTotal < 0 && Conversion.Val(amountEntered) > 0)
                            {
                                amountEntered = (Convert.ToDecimal(amountEntered) * -1).ToString("##0.00");
                            }
                            if (selectedTender.Credit_Card == null)
                            {
                                TenderAutorecognition = false;
                            }
                            if (TenderAutorecognition)
                            {
                                cardSummary.CardNumber = cc.Cardnumber;
                            }
                            else
                            {
                                cardSummary.CardNumber = "";
                            }

                            if (_policyManager.ScanGiftCard)
                            {
                                cardSummary.Caption = _resourceManager.CreateCaption(offSet, (short)17, (short)14, null, (short)2);
                            }
                            else
                            {
                                cardSummary.Caption = TenderAutorecognition ? "" : _resourceManager.CreateCaption(offSet, (short)17, (short)14, null, (short)2);
                            }

                            cardSummary.SelectedCard = CardForm.Givex;

                            if (Card_Cancelled)
                            {
                                amountEntered = "";
                                selectedTender.Amount_Used = 0;
                            }
                        }
                    }
                    if (_policyManager.Charge_Acct)
                    {
                        if (sale.Customer.AR_Customer == false && selectedTender.Tender_Class == "ACCOUNT")
                        {
                            if (Conversion.Val(amountEntered) != 0)
                            {
                                MessageType temp_VbStyle4 = (int)MessageType.OkOnly + MessageType.Information;
                                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)4199, sale.Customer.Name, temp_VbStyle4);
                                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                                amountEntered = "";
                            }
                            IsEnter = false;
                            return null;
                        }
                    }
                    if (selectedTender.Tender_Class == "ACCOUNT" && makePayment)
                    {
                        if (!string.IsNullOrEmpty(sale.Customer.LoyaltyCard))
                        {
                            if (Math.Abs(grossTotal) - Math.Abs(allTenders.Tend_Totals.Tend_Used) > 0)
                            {
                                var creditCard = selectedTender.Credit_Card;
                                selectedTender.Credit_Card.Crd_Type = "F";
                                selectedTender.Credit_Card.CardProfileID = sale.Customer.CardProfileID; // 
                                _creditCardManager.SetCardnumber(ref creditCard, sale.Customer.LoyaltyCard);
                                if (!_creditCardManager.CardIsValid(ref creditCard))
                                {
                                    MessageType temp_VbStyle5 = (int)MessageType.OkOnly + MessageType.Critical;
                                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 58, null, temp_VbStyle5);
                                    selectedTender.Credit_Card.Crd_Type = "";
                                    selectedTender.Credit_Card.CardProfileID = "";
                                    _creditCardManager.SetCardnumber(ref creditCard, "");
                                    selectedTender.Credit_Card.Expiry_Date = "";
                                    IsEnter = false;
                                    return null;
                                }
                                if (selectedTender.Credit_Card.CheckExpiryDate) // 
                                {
                                    selectedTender.Credit_Card.Expiry_Date = sale.Customer.LoyaltyExpDate;
                                    if (_creditCardManager.Card_Is_Expired(ref creditCard))
                                    {
                                        cBuf = selectedTender.Credit_Card.Expiry_Month + "/" + selectedTender.Credit_Card.Expiry_Year;
                                        MessageType temp_VbStyle6 = (int)MessageType.Critical + MessageType.OkOnly;
                                        errorMessage.StatusCode = HttpStatusCode.BadRequest;
                                        errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)92, cBuf, temp_VbStyle6);
                                        selectedTender.Credit_Card.Crd_Type = "";
                                        selectedTender.Credit_Card.CardProfileID = "";
                                        _creditCardManager.SetCardnumber(ref creditCard, "");
                                        selectedTender.Credit_Card.Expiry_Date = "";
                                        IsEnter = false;
                                        return null;
                                    }
                                }

                                selectedTender.Credit_Card.Trans_Date = DateAndTime.Today;
                                selectedTender.Credit_Card.Trans_Time = DateAndTime.TimeOfDay;

                                selectedTender.Credit_Card.Result = "0";
                                selectedTender.Credit_Card.Card_Swiped = sale.Customer.LoyaltyCardSwiped;
                                CardRestrictionValidation(selectedTender, allTenders, sale, creditCard,
                                    ref promptMessages, ref amountEntered, grossTotal, makePayment, false, out errorMessage);
                                if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                                {
                                    return null;
                                }
                                allTenders.Card_Authorized = true; // 
                            }
                            else
                            {

                                IsEnter = false;
                                return null;
                            }
                        }
                    }

                    if (transactionType != "CashDrop")
                    {

                        if (selectedTender.Tender_Class == "GIFTCERT")
                        {
                            if (strGiftTender == "LocalGift" && _policyManager.GC_FORCE && _policyManager.GC_NUMBERS)
                            {
                                if (string.IsNullOrEmpty(amountEntered))
                                {
                                    _tenderService.RemoveUsedGc(sale.Sale_Num);
                                }
                                if (string.IsNullOrEmpty(amountEntered))
                                {

                                    cardSummary.GiftCerts = _tenderService.GetAllGiftCert(DataSource.CSCMaster);
                                    if (cardSummary.GiftCerts.Count == 0)
                                    {
                                        MessageType temp_VbStyle = (int)MessageType.Information + MessageType.OkOnly;
                                        errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)8115, null, temp_VbStyle);
                                        errorMessage.StatusCode = HttpStatusCode.NotFound;
                                        return null;
                                    }
                                    else
                                    {
                                        cardSummary.GiftCerts.AddRange(_tenderService.GetExpiredGiftCert(DataSource.CSCMaster));
                                    }

                                }
                                else //if changing the gift certificate amount used, consider it as a different gift certificate and donot delete the selected one
                                {
                                    if (Conversion.Val(amountEntered) != 0)
                                    {

                                    }
                                }

                            }
                            else if (strGiftTender == "EKO" && _policyManager.ThirdParty)
                            {
                                return null;
                            }
                        }

                        if (selectedTender.Tender_Class == "CREDIT" && _policyManager.SC_CHECK && grossTotal > 0)
                        {
                            if (string.IsNullOrEmpty(amountEntered))
                            {
                                _tenderService.RemoveUsedSc(sale.Sale_Num);
                            }

                            if (string.IsNullOrEmpty(amountEntered))
                            {
                                cardSummary.StoreCredits = _tenderService.GetAllStoreCredits(DataSource.CSCMaster);
                                if (cardSummary.StoreCredits.Count == 0 && tenderCode != _policyManager.ARTender)
                                {
                                    MessageType temp_VbStyle = (int)MessageType.Information + MessageType.OkOnly;
                                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)8114, null, temp_VbStyle);
                                    errorMessage.StatusCode = HttpStatusCode.NotFound;
                                    return null;
                                }
                                cardSummary.StoreCredits.AddRange(_tenderService.GetExpiredStoreCredits(DataSource.CSCMaster));
                            }
                            else //if changing the Store_Renamed credit amount used, consider it as a different Store_Renamed credit and donot delete the selected one
                            {

                                if (Conversion.Val(amountEntered) != 0 && (Conversion.Val(amountEntered) != Conversion.Val(selectedTender.Amount_Entered)))
                                {
                                    _tenderService.RemoveUsedSc(sale.Sale_Num);
                                    var ans = (short)1;
                                    if (ans == (int)MsgBoxResult.No)
                                    {
                                        allTenders.Tend_Totals.Tend_Used = allTenders.Tend_Totals.Tend_Used - Convert.ToDecimal(amountEntered);
                                        amountEntered = "";
                                        selectedTender.Amount_Used = 0;
                                        amountEntered = "";
                                        allTenders.Tend_Totals.Change = grossTotal - allTenders.Tend_Totals.Tend_Used;

                                    }
                                }
                            }

                        }

                        //For refund ,when using Store_Renamed credit
                        if (selectedTender.Tender_Class == "CREDIT" && grossTotal < 0)
                        {
                            if (Conversion.Val(amountEntered) == 0)
                            {
                                if (grossTotal > 0)
                                {
                                    amountEntered = (grossTotal - allTenders.Tend_Totals.Tend_Used).ToString();
                                }
                                else
                                {
                                    amountEntered = ((System.Math.Abs(grossTotal) - System.Math.Abs(allTenders.Tend_Totals.Tend_Used)) * -1).ToString();
                                }
                            }
                            else if (grossTotal < 0 && Conversion.Val(amountEntered) > 0)
                            {
                                amountEntered = (Conversion.Val(amountEntered) * -1).ToString();
                            }
                        }

                        if ((selectedTender.Tender_Class == "CRCARD" || selectedTender.Tender_Class == "DBCARD" || selectedTender.Tender_Class == "FLEET" || selectedTender.Tender_Class == "THIRDPARTY" || selectedTender.Tender_Class == "LOYALTY") && _policyManager.CC_MODE == "Validate")
                        {
                            if (grossTotal > 0 && Conversion.Val(amountEntered) < 0)
                            {
                                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)75, null, MessageType.OkOnly);
                                amountEntered = "";

                                IsEnter = false;
                                return null;
                            }

                            if (grossTotal < 0 && Conversion.Val(amountEntered) > 0)
                            {
                                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)74, null, MessageType.OkOnly);
                                amountEntered = "";
                                IsEnter = false;
                                return null;
                            }

                            if (!selectedTender.Give_Change && Conversion.Val(selectedTender.Amount_Used) == 0
                                && Convert.ToDecimal(Math.Abs(Conversion.Val(amountEntered) * selectedTender.Exchange_Rate)) > System.Math.Abs(grossTotal) - System.Math.Abs(allTenders.Tend_Totals.Tend_Used))
                            {
                                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)88, null, MessageType.OkOnly);
                                amountEntered = "";

                                IsEnter = false;
                                return null;
                            }

                            if (selectedTender.Credit_Card.Crd_Type != "K" && System.Math.Abs(grossTotal) - System.Math.Abs(allTenders.Tend_Totals.Tend_Used) > 0)
                            {
                                cardType = System.Convert.ToString(selectedTender.Tender_Class);

                                if (selectedTender.Tender_Code != "ACK")
                                {
                                    if (Conversion.Val(amountEntered) == 0 && _policyManager.CC_MODE == "Validate")
                                    {
                                        if (grossTotal > 0)
                                        {
                                            amountEntered = (grossTotal - allTenders.Tend_Totals.Tend_Used).ToString();
                                        }
                                        else
                                        {
                                            amountEntered = ((System.Math.Abs(grossTotal) - System.Math.Abs(allTenders.Tend_Totals.Tend_Used)) * -1).ToString();
                                        }
                                    }
                                    else if (grossTotal < 0 && Conversion.Val(amountEntered) > 0)
                                    {
                                        amountEntered = (Conversion.Val(string.Format(amountEntered, "##0.00")) * -1).ToString();
                                    }
                                    else if (!makePayment)
                                    {
                                        return cardSummary;
                                    }
                                }

                                if (selectedTender.Credit_Card == null)
                                {
                                    TenderAutorecognition = false;
                                }
                                if (TenderAutorecognition)
                                {
                                    txtCardNum = selectedTender.Credit_Card.Cardnumber;
                                    txtExpiryDate = selectedTender.Credit_Card.Expiry_Date;
                                    TenderMin = selectedTender.MinAmount;
                                    TenderMax = selectedTender.MaxAmount;
                                    TenderName = selectedTender.Tender_Name;
                                    boolInactive = selectedTender.Inactive;

                                    if (boolInactive)
                                    {
                                        MessageType temp_VbStyle13 = (int)MessageType.Exclamation + MessageType.OkOnly;
                                        errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)77, TenderName, temp_VbStyle13);
                                        amountEntered = "";
                                        return null;
                                    }

                                    if (TenderMax != 0) // need to check only if it is not zero- otherwise no restriction
                                    {
                                        if (!string.IsNullOrEmpty(amountEntered) && Convert.ToDecimal(amountEntered) > Convert.ToDecimal(TenderMax))
                                        {
                                            amountEntered = TenderMax.ToString();
                                        }
                                    }
                                    if (TenderMin != 0 & sale.Sale_Totals.Gross > 0) //Then ' shouldn't < minamount if there is a setting for tender
                                    {
                                        if (!string.IsNullOrEmpty(amountEntered) && Convert.ToDecimal(amountEntered) < Convert.ToDecimal(TenderMin))
                                        {
                                            capValue[1] = TenderName;
                                            capValue[2] = TenderMin;
                                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)450, capValue, MessageType.OkOnly);
                                            amountEntered = "";
                                            IsEnter = false;
                                            return null;
                                        }
                                    }
                                }
                                else
                                {
                                    if ((!_policyManager.COMBINECR && selectedTender.Tender_Class == "CRCARD") || (!_policyManager.COMBINEFLEET && selectedTender.Tender_Class == "FLEET"))
                                    {
                                        TenderMin = System.Convert.ToDouble(selectedTender.MinAmount);
                                        TenderMax = System.Convert.ToDouble(selectedTender.MaxAmount);
                                        TenderName = System.Convert.ToString(selectedTender.Tender_Name);
                                        if (TenderMax != 0) // need to check only if it is not zero- otherwise no restriction
                                        {
                                            if (!string.IsNullOrEmpty(amountEntered) && Conversion.Val(amountEntered) > TenderMax)
                                            {
                                                amountEntered = TenderMax.ToString();
                                            }
                                        }
                                        if (TenderMin != 0 & sale.Sale_Totals.Gross > 0) // Then ' shouldn't < minamount if there is a setting for tender
                                        {
                                            if (!string.IsNullOrEmpty(amountEntered) && Conversion.Val(amountEntered) < TenderMin)
                                            {
                                                capValue[1] = TenderName;
                                                capValue[2] = TenderMin;
                                                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)450, capValue, MessageType.OkOnly);
                                                amountEntered = "";
                                                IsEnter = false;
                                                return null;
                                            }
                                        }
                                    }
                                    txtCardNum = "";
                                    txtExpiryDate = "";
                                }
                                Card_Cancelled = false;
                                var emvProcess = false;

                                if (_policyManager.EMVVersion == true && (selectedTender.Tender_Class == "DBCARD" || selectedTender.Tender_Class == "CRCARD"))
                                {
                                    emvProcess = true;
                                }
                                else
                                {
                                    emvProcess = false;
                                }

                                if (emvProcess == true)
                                {
                                    if (selectedTender.Tender_Class == "DBCARD" || selectedTender.Tender_Class == "CRCARD")
                                    {
                                        if (makePayment)
                                        {
                                            if (selectedTender.Tender_Class == "DBCARD")
                                            {
                                                TrType = "Debit";
                                            }
                                            else
                                            {
                                                TrType = "Credit";
                                            }

                                            selectedTender.Credit_Card.Trans_Amount = (float)(Conversion.Val(amountEntered));
                                            selectedTender.Credit_Card.Trans_Number = (sale.Sale_Num).ToString();
                                            selectedTender.Credit_Card.Void_Num = sale.Void_Num;

                                            if (selectedTender.Credit_Card.Trans_Amount < 0)
                                            {
                                                if (sale.ForCorrection)
                                                {
                                                    selectedTender.Credit_Card.Trans_Type = "VoidInside";
                                                }
                                                else
                                                {
                                                    selectedTender.Credit_Card.Trans_Type = "RefundInside";
                                                }
                                            }
                                            else
                                            {
                                                if (sale.ForCorrection)
                                                {
                                                    selectedTender.Credit_Card.Trans_Type = "VoidInside";
                                                }
                                                else
                                                {
                                                    selectedTender.Credit_Card.Trans_Type = "SaleInside";
                                                }

                                            }
                                            bool blManualCardProcess = false;
                                            selectedTender.Credit_Card.ManualCardProcess = blManualCardProcess == true;
                                            WriteToLogFile("processEMV" + TrType + Convert.ToString(sale.Sale_Num) + "ManualProcess: " + System.Convert.ToString(cc.ManualCardProcess));
                                            cc = selectedTender.Credit_Card;
                                            cc.TendCard = _creditCardManager.GetTendCard(selectedTender.Tender_Code);
                                            var user = _loginManager.GetExistingUser(UserCode);
                                            if (_policyManager.CC_MODE == "Validate" && user.User_Group.Code != "Trainer") //Card_Authorized = True
                                            {
                                                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                                var ip = _utilityService.GetPosAddress((byte)PosId);
                                                if (ip != null)
                                                {
                                                    var ipAddress = IPAddress.Parse(ip);
                                                    var remoteEndPoint = new IPEndPoint(ipAddress, 8888);
                                                    try
                                                    {
                                                        socket.Connect(remoteEndPoint);

                                                        if (socket.Connected)
                                                        {
                                                            //Nancy add the following line to clean up the response at first
                                                            cc.Response = "";
                                                            if (cc.Crd_Type == "D")
                                                            {
                                                                SendToTPS(_cardManager.GetRequestString(ref cc, sale, cc.Trans_Type, "Debit", cc.Trans_Amount, ""), ref socket, ref cc);
                                                            }
                                                            else if (cc.Crd_Type == "C")
                                                            {
                                                                SendToTPS(_cardManager.GetRequestString(ref cc, sale, cc.Trans_Type, "Credit", cc.Trans_Amount, ""), ref socket, ref cc);
                                                            }
                                                            double processTimer = (float)DateAndTime.Timer;
                                                            short timeout = _policyManager.BankTimeOutSec;
                                                            if (timeout == 0)
                                                            {
                                                                timeout = 10;
                                                            }
                                                            while ((DateAndTime.Timer - processTimer) < timeout) // 2013 10 17 - Reji Added polity for bank transaction timeout in seconds
                                                            {
                                                                // Data buffer for incoming data.
                                                                var bytes = new byte[2048];
                                                                var bytesRec = socket.Receive(bytes);
                                                                var strBuffer = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                                                                WriteToLogFile("Received from STPS: " + strBuffer);
                                                                GetResponse(strBuffer, ref cc);

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
                                                            Report merchantFileStream = null;
                                                            Report customerFileStream = null;
                                                            GetTransactionResult(ref allTenders, ref cc, selectedTender.Tender_Code, ref sale, ref socket,
                                                                ref merchantFileStream, ref customerFileStream, out errorMessage);
                                                            transactionReports = new List<Report> { merchantFileStream, customerFileStream };
                                                            if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                                                            {
                                                                var updatedCreditCard = GetSelectedTender(allTenders, tenderCode);
                                                                selectedTender.Credit_Card = cc;
                                                                allTenders.Card_Authorized = true;

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
                                                    catch (Exception ex)
                                                    {
                                                        errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)84, null,
                                                                                                              CriticalOkMessageType);
                                                        errorMessage.StatusCode = HttpStatusCode.BadRequest;
                                                        return null;
                                                    }
                                                    selectedTender.Credit_Card = cc;
                                                }
                                                else
                                                {


                                                    if (!cc.AutoRecognition && amountEntered.Equals("0"))
                                                    {
                                                        allTenders.Card_Authorized = false;
                                                    }
                                                    else
                                                    {
                                                        _creditCardManager.Authorize_Card(ref cc);
                                                        cc.Result = "0";

                                                        selectedTender.Credit_Card = cc;

                                                        allTenders.Card_Authorized = true;
                                                    }
                                                }
                                            }
                                        }
                                        IsEnter = false;
                                        break;
                                    }
                                    cardSummary.SelectedCard = CardForm.Credit;
                                }


                                if (selectedTender.Tender_Class == "DBCARD" || _policyManager.SWIPE_CARD)
                                {
                                    if (_policyManager.USE_PINPAD && selectedTender.Tender_Class != "THIRDPARTY" && selectedTender.Tender_Class != "LOYALTY")
                                    {
                                        //cmdSwipe.Enabled = true;
                                    }
                                }
                                switch (cardType)
                                {
                                    case "DBCARD":
                                        break;
                                    case "THIRDPARTY":
                                        if (sale.Sale_Totals.Gross < 0)
                                        {
                                        }
                                        break;
                                }

                                if (TenderAutorecognition)
                                {

                                }
                                if (selectedTender.Tender_Class == "LOYALTY")
                                {
                                    IsEnter = false;
                                    return null;
                                }
                            }

                            else
                            {
                                IsEnter = false;
                                return null;
                            }
                            if (Card_Cancelled)
                            {
                                amountEntered = "";
                                selectedTender.Amount_Used = 0;
                            }
                        }


                        if (Conversion.Val(amountEntered) == 0 && System.Math.Abs(grossTotal) > System.Math.Abs(allTenders.Tend_Totals.Tend_Used) && ((selectedTender.Tender_Class == "CREDIT" && _policyManager.SC_CHECK) || (selectedTender.Tender_Class == "GIFTCERT" && _policyManager.GC_FORCE && _policyManager.GC_NUMBERS && strGiftTender == "LocalGift")))
                        {

                            amountEntered = "";

                        }
                        else if (Conversion.Val(amountEntered) == 0 && System.Math.Abs(grossTotal) > System.Math.Abs(allTenders.Tend_Totals.Tend_Used) && selectedTender.Tender_Class == "ACCOUNT" && _policyManager.Use_KickBack && Variables.blChargeAcct)
                        {
                            amountEntered = "";
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(amountEntered) && System.Math.Abs(grossTotal) > System.Math.Abs(allTenders.Tend_Totals.Tend_Used))
                            {

                                // Compute the number of Smallest Units that are needed to complete the sale.
                                TAmt = System.Convert.ToDecimal((System.Math.Abs(Math.Round(grossTotal, 2)) - Math.Abs(allTenders.Tend_Totals.Tend_Used)) / (decimal)selectedTender.Exchange_Rate);
                                TMod = (int)(TAmt / (decimal)selectedTender.Smallest_Unit);

                                if (TMod * selectedTender.Smallest_Unit < Math.Round((double)TAmt, 2))
                                {
                                    TMod++;
                                }
                                TAmt = TMod * (decimal)selectedTender.Smallest_Unit;
                                if (grossTotal < 0) //
                                {
                                    TAmt = TAmt * -1;
                                }
                                amountEntered = TAmt.ToString();
                            }
                            if (Card_Cancelled)
                            {
                                amountEntered = "";
                            }
                            else
                            {
                                selectedTender.Credit_Card.Trans_Amount = (float)Conversion.Val(amountEntered);
                            }
                        }

                        // Check that the customer has room in his account.
                        if (selectedTender.Tender_Class == "ACCOUNT" && makePayment)
                        {
                            if (sale.Customer.UsePO)
                            {

                                if (!string.IsNullOrEmpty(purchaseOrder))
                                {
                                    if (sale.Customer.MultiUse_PO == false && grossTotal > 0)
                                    {
                                        if (_customerService.UsedCustomerPo(sale.Customer.Code, purchaseOrder))
                                        {
                                            arrMsg[1] = sale.Customer.Name;
                                            arrMsg[2] = purchaseOrder;
                                            errorMessage.StatusCode = HttpStatusCode.NotFound;
                                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)476, arrMsg, MessageType.OkOnly);
                                            return null;
                                        }
                                    }
                                }
                                else
                                {
                                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                                    errorMessage.MessageStyle = new MessageStyle
                                    {
                                        Message = "Invalid Request",
                                        MessageType = 0
                                    };
                                    return null;
                                }

                                sale.AR_PO = Strings.Left(purchaseOrder, 15);
                            }

                            var newBalance = (decimal)(sale.Customer.Current_Balance + Conversion.Val(amountEntered));
                            var strAction = _resourceManager.GetResString(offSet, (short)351);
                            if (newBalance > Convert.ToDecimal(sale.Customer.Credit_Limit)) //And SA.Customer.Credit_Limit > 0 Then
                            {
                                var currentUser = _loginManager.GetExistingUser(userCode);
                                if (!_policyManager.GetPol("U_OR_LIMIT", currentUser) && !makePayment)
                                {
                                    MessageType temp_VbStyle17 = (int)MessageType.Question + MessageType.YesNo;
                                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)8193, strAction, temp_VbStyle17);
                                    errorMessage.StatusCode = HttpStatusCode.Forbidden;
                                    return null;
                                }

                                if (_policyManager.GetPol("U_OR_LIMIT", currentUser))
                                {
                                    if (!overrideArLimit)
                                    {
                                        if (sale.Customer.Credit_Limit - sale.Customer.Current_Balance > 0)
                                        {
                                            amountEntered = Convert.ToDecimal(sale.Customer.Credit_Limit - sale.Customer.Current_Balance).ToString();
                                        }
                                        else
                                        {
                                            amountEntered = "";
                                        }
                                    }
                                    else
                                    {
                                        selectedTender.AuthUser = currentUser.Code;
                                    }
                                }
                                else //not authorized user
                                {
                                    amountEntered = sale.Customer.Credit_Limit - sale.Customer.Current_Balance > 0 ? Convert.ToDecimal(sale.Customer.Credit_Limit - sale.Customer.Current_Balance).ToString() : "";
                                }
                            }
                        }

                        // Activate the completion buttons if we have enough money to complete the sale.
                        if (selectedTender.Exchange_Rate.ToString("###,###,##0.000000") != "1.000000")
                        {
                            var text = _resourceManager.GetResString(offSet, (short)270) + ": " + selectedTender.Exchange_Rate.ToString("###,###,##0.000000") + "     " + _resourceManager.GetResString(offSet, (short)161) + ": " + selectedTender.Smallest_Unit.ToString("0.00"); //"Exchange Rate : ","     Small Unit : "
                        }
                        else
                        {
                            var text = _resourceManager.GetResString(offSet, (short)161) + selectedTender.Smallest_Unit.ToString("0.00"); //"Small Unit : "
                        }
                        break;
                    }
                    Card_Cancelled = false;

                    if (selectedTender.Tender_Class == "GIFTCERT" && strGiftTender == "LocalGift" && Conversion.Val(amountEntered) == 0)
                    {
                        _tenderService.RemoveUsedGc(sale.Sale_Num);
                    }
                    if (selectedTender.Tender_Class == "CREDIT" && Conversion.Val(amountEntered) == 0)
                    {
                        _tenderService.RemoveUsedSc(sale.Sale_Num);
                    }
                    if (!_policyManager.USE_FUEL)
                    {
                        if (Card_Cancelled && Chaps_Main.strFunctionTend.Length > 0)
                        {
                        }
                    }
                    IsEnter = false;
                    break;
            }
            amountEntered = Conversion.Val(amountEntered).ToString("0.00");

            switch (transactionType)
            {
                case "Sale":
                    CacheManager.AddTendersForSale(sale.Sale_Num, till.Number, allTenders);
                    break;
                case "ARPay":
                    CacheManager.AddTendersForArPay(sale.Sale_Num, allTenders);
                    break;
                case "Payment":
                    CacheManager.AddTendersForPayment(sale.Sale_Num, allTenders);
                    break;
            }

            if (cardSummary.SelectedCard != CardForm.None && cc != null)
            {
                _creditCardManager.CardInPcf(ref cc);
                cardSummary.TenderCode = selectedTender.Tender_Code;
                cardSummary.AskPin = cc.AskPCFPin;
                cardSummary.Pin = cc.PCFPIN;
                cardSummary.AskDriverNo = cc.AskDriverNo;
                cardSummary.AskIdentiifcationNumber = cc.AskIdentificationNo;
                cardSummary.AskOdometer = cc.AskOdometer;
                cardSummary.AskProductRestrictionCode = cc.AskProdRestrictCode;
                cardSummary.AskVehicle = cc.AskVechicle;
                cardSummary.CardNumber = cc.Cardnumber;
                cardSummary.Amount = amountEntered;
                cardSummary.PromptMessages = promptMessages;
                cardSummary.ProfileId = cc.CardProfileID;
            }
            return cardSummary;
        }

        /// <summary>
        /// Method to save gift ecrtificates
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="giftCerts">Gift certificates</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Tenders</returns>
        public Tenders SaveGiftCertificate(int saleNumber, int tillNumber, List<GiftCert> giftCerts,
            string transactionType, string userCode, string tenderCode, out ErrorMessage errorMessage)
        {
            var allGiftCerts = GetGiftCertificates(saleNumber, tillNumber, userCode, tenderCode, "", transactionType,
                out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var isInvalidGiftCert = false;
            if (allGiftCerts == null || allGiftCerts.Count == 0)
            {
                isInvalidGiftCert = true;
                allGiftCerts = new List<GiftCert>();
            }
            foreach (var giftCert in giftCerts)
            {
                if (!allGiftCerts.Any(t => t.GcNumber == giftCert.GcNumber
                && t.GcExpiresOn == DateTime.MinValue))
                {
                    isInvalidGiftCert = true;
                    break;
                }
            }
            if (isInvalidGiftCert)
            {
                errorMessage.MessageStyle = new MessageStyle { Message = "Request is invalid" };
                return null;
            }

            _tenderService.RemoveUsedSc(saleNumber);
            _tenderService.AddGcToDbTemp(saleNumber, tillNumber, giftCerts);
            var giftCertAmount = giftCerts.Sum(g => g.GcAmount);
            var tenders = GetAllTender(saleNumber, tillNumber, transactionType, userCode, false, string.Empty,
               out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            var till = _tillService.GetTill(tillNumber);

            var newTenders = UpdateTender(ref tenders, sale, till, transactionType, userCode, false, tenderCode,
                    giftCertAmount.ToString(), out errorMessage);
            if (transactionType == "Sale")
                CacheManager.AddTendersForSale(saleNumber, till.Number, newTenders);
            else if (transactionType == "ARPay")
                CacheManager.AddTendersForArPay(saleNumber, newTenders);
            else if (transactionType == "Payment")
                CacheManager.AddTendersForPayment(saleNumber, newTenders);
            return newTenders;
        }

        /// <summary>
        /// Method to save givex sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="cardNumber">Card number</param>
        /// <param name="transactionType">Transction type</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="userCode">User code</param>
        /// <param name="amountEntered">Amount entered</param>
        /// <param name="errorMessage">Error</param>
        /// <param name="givexReport">Signature</param>
        /// <param name="copies">Copies</param>
        /// <returns>Tenders</returns>
        public Tenders SaveGivexSale(int saleNumber, int tillNumber, string cardNumber,
            string transactionType, string tenderCode, string userCode, string amountEntered,
            out ErrorMessage errorMessage, out Report givexReport, out int copies)
        {
            copies = 1;
            givexReport = null;
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
            var allTenders = GetAllTender(saleNumber, tillNumber, transactionType, userCode, false,
                string.Empty, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            Tenders updateTenders = null;
            string strSecurity = "";
            decimal newBalance = 0;
            string refNum = "";
            string expDate = "";
            string Result = "";
            string TenderCode = "";
            var cc = new Credit_Card();
            FindCardTender(cardNumber, saleNumber, tillNumber, transactionType, userCode, out errorMessage, ref cc);
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            double result;
            if (!cc.Card_Swiped)
            {
                if (!double.TryParse(cardNumber, out result))
                {
                    MessageType temp_VbStyle5 = (int)MessageType.Critical + MessageType.OkOnly;
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)77,
                        null, temp_VbStyle5);
                    return null;
                }

                cc.Cardnumber = cardNumber;
                TenderCode = _creditCardManager.Find_TenderCode(ref sale, ref cc);
                if (!_creditCardManager.CardIsValid(ref cc))
                {
                    MessageType temp_VbStyle5 = (int)MessageType.Critical + MessageType.OkOnly;
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)94,
                        _creditCardManager.Invalid_Reason(ref cc), temp_VbStyle5);
                    return null;
                }
                _creditCardManager.SetCardnumber(ref cc, cardNumber.Trim());
                var displayNoReceiptButton = true;
                var arPay = GetARPayer(saleNumber, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message) && transactionType == "ARPay")
                {
                    return null;
                }
                var payment = GetFleetPayer(saleNumber, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message) && transactionType == "Payment")
                {
                    return null;
                }
                var grossTotal = GetGrossTotal(transactionType, sale, payment, arPay, -1, out displayNoReceiptButton);
                if (string.IsNullOrEmpty(amountEntered))
                {
                    List<Report> transactReports = null;
                    SaleTend_Keydown(ref allTenders, sale, userCode, TenderCode, ref amountEntered, transactionType,
                      grossTotal, null, out transactReports, out errorMessage);
                    if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                    {
                        return null;
                    }
                }
            }
            else
            {
                TenderCode = cc.TendCode;
                amountEntered = cc.Trans_Amount.ToString();
            }
            // For Both Negative and Positive card checking - Dec27,2001
            if (_creditCardManager.CardInNcf(ref cc)) // If Negative Card give the message from table Cardmeesage and donot process
            {

                errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(offSet, cc.Decline_Message, (short)99, null);
                return null;
            }
            if (!_creditCardManager.CardInPcf(ref cc))
            {
                //       Invalid Card. Contact the Card Issuer
                MessageType temp_VbStyle = (int)MessageType.Critical + MessageType.OkOnly;
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)78, null, temp_VbStyle);
                return null;
            }
            var selectedTender = GetSelectedTender(allTenders, tenderCode);
            if (selectedTender == null)
            {
                errorMessage.MessageStyle = new MessageStyle { Message = "Invalid Tender code" };
                return null;
            }
            selectedTender.Credit_Card.Crd_Type = _tenderService.GetCardType(tenderCode);
            if (_creditCardManager.CardIsValid(ref cc))
            {
                // If the user swiped another type of card force him to select the right card
                if (!string.IsNullOrEmpty(selectedTender.Credit_Card.Crd_Type) && selectedTender.Credit_Card.Crd_Type != cc.Crd_Type)
                {
                    // You selected a different card.
                    MessageType temp_VbStyle2 = (int)MessageType.Exclamation + MessageType.OkOnly;
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)68, null, temp_VbStyle2);
                    return null;
                }

                // check the purchase limit for used card
                if ((cc.PurchaseLimit > 0) && (Conversion.Val(amountEntered) > cc.PurchaseLimit))
                {

                    MessageType temp_VbStyle3 = (int)MessageType.Information + MessageType.OkOnly;
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)66, cc.PurchaseLimit, temp_VbStyle3);
                    amountEntered = (cc.PurchaseLimit).ToString();
                }

                cc.Trans_Amount = (float)(Conversion.Val(string.Format(amountEntered, "##0.00")));
                cc.Trans_Number = (sale.Sale_Num).ToString();
                cc.Void_Num = sale.Void_Num;

                if (sale.Sale_Totals.Gross < 0)
                {
                    if (!cc.RefundAllowed)
                    {
                        //       Refund is not allowed with this card.
                        MessageType temp_VbStyle4 = (int)MessageType.Critical + MessageType.OkOnly;
                        errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)82, null, temp_VbStyle4);
                        return null;
                    }
                    cc.Trans_Type = "RefundInside";
                }
                else
                {
                    if (sale.Void_Num > 0)
                    {
                        cc.Trans_Type = "RefundInside";
                    }
                    else
                    {
                        cc.Trans_Type = "SaleInside";
                    }
                }

                if (cc.GiftType == "G")
                {
                    GiveXReceiptType givexReceipt;

                    if (cc.Trans_Amount > 0)
                    {

                        if (_givexClientManager.RedeemGiveX(userCode, cc.Cardnumber, cc.Trans_Amount, strSecurity,
                            sale.Sale_Num, ref newBalance, ref refNum, ref expDate, ref Result, out errorMessage, out givexReceipt))
                        {
                            cc.Trans_Date = DateAndTime.Today;
                            cc.Trans_Time = DateAndTime.TimeOfDay;
                            cc.Balance = (decimal)newBalance;
                            cc.Sequence_Number = refNum;
                            cc.Expiry_Date = expDate;
                            cc.Receipt_Display = Result;
                            cc.Result = (6).ToString();

                            selectedTender.Credit_Card = cc;
                            copies = cc.PrintCopies == 0 ? 1 : cc.PrintCopies;
                            givexReport = cc.PrintCopies > 1 ? _receiptManager.Print_GiveX_Receipt(givexReceipt, copies: cc.PrintCopies, sameSale: false) : _receiptManager.Print_GiveX_Receipt(givexReceipt);

                            allTenders.Card_Authorized = true;
                            List<Report> transactReports = null;
                            updateTenders = UpdateTenders(sale.Sale_Num, sale.TillNumber, transactionType, userCode, false, tenderCode, amountEntered, out transactReports, out errorMessage);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        if (_givexClientManager.AdjustGiveX(cc.Cardnumber, System.Math.Abs((short)cc.Trans_Amount),
                            sale.Sale_Num, ref newBalance, ref refNum, ref expDate, ref Result, userCode, out errorMessage, out givexReceipt))
                        {
                            cc.Trans_Date = DateAndTime.Today;
                            cc.Trans_Time = DateAndTime.TimeOfDay;
                            cc.Balance = (decimal)newBalance;
                            cc.Sequence_Number = refNum;
                            cc.Expiry_Date = expDate;
                            cc.Receipt_Display = Result;
                            cc.Result = (3).ToString();
                            copies = cc.PrintCopies;
                            selectedTender.Credit_Card = cc;
                            givexReport = cc.PrintCopies > 1 ? _receiptManager.Print_GiveX_Receipt(givexReceipt, copies: cc.PrintCopies, sameSale: false) : _receiptManager.Print_GiveX_Receipt(givexReceipt);

                            List<Report> transactReports = null;
                            updateTenders = UpdateTenders(sale.Sale_Num, sale.TillNumber, transactionType, userCode, false, tenderCode, amountEntered, out transactReports, out errorMessage);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else if (cc.GiftType == "A")
                {
                }

            }
            else
            {
                MessageType temp_VbStyle5 = (int)MessageType.Critical + MessageType.OkOnly;
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)94,
                    _creditCardManager.Invalid_Reason(ref cc), temp_VbStyle5);
                return null;
            }
            //ControlsEnabled(true);
            return updateTenders;
        }

        /// <summary>
        /// Method to find card tender
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <param name="cc">Credit card</param>
        /// <param name="verifyRestriction">Restriction</param>
        /// <returns>Card Information</returns>
        public CardSummary FindCardTender(string cardNumber, int saleNumber, int tillNumber,
            string transactionType, string userCode, out ErrorMessage errorMessage, ref Credit_Card cc,
            bool verifyRestriction = true, bool verifyAccount = false)
        {
            var cardForm = new CardSummary();
            var promptMessages = new List<string>();
            bool displayNoReceiptButton;
            var allTenders = GetAllTender(saleNumber, tillNumber, transactionType, userCode,
                false, "", out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            //cardNumber = DecryptText(cardNumber);
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            var arPay = CacheManager.GetArPayment(sale.Sale_Num);
            var payment = CacheManager.GetFleetPayment(sale.Sale_Num);
            var grossTotal = GetGrossTotal(transactionType, sale, payment,
                arPay, -1, out displayNoReceiptButton);
            string amountEntered = null;
            string tenderCode = "";
            errorMessage = new ErrorMessage();
            if (string.IsNullOrEmpty(cardNumber))
            {
                MessageType temp_VbStyle5 = (int)MessageType.Critical + MessageType.OkOnly;
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)77,
                   null, temp_VbStyle5);
                return null;
            }

            object[] messages = new object[3];
            decimal Trans_Amount = 0;

            int StPos = 0, EndPos = 0;
            try
            {
                byte[] data = Convert.FromBase64String(cardNumber);
                string decodedString = Encoding.UTF8.GetString(data);

                cardNumber = decodedString;
            }
            catch
            {
                MessageType tempVbStyle = (int)MessageType.Critical + MessageType.OkOnly;
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)77, null, tempVbStyle);
                return null;
            }
            // If there is a "?" in the txtAmount text box we assume that
            // it was a card swiped and not an amount entered
            StPos = (short)(cardNumber.IndexOf(";") + 1);

            if (StPos > 0)
            {
                EndPos = (short)(cardNumber.Substring(StPos).IndexOf("?") + 1 + StPos);
            }

            if (EndPos > 0 & StPos > 0 & EndPos > StPos)
            {
                WriteToLogFile("Got the correct format for the track2 continue processing...");
                // If the sale has been paid already or is partially paid, don't allow to swipe any card again
                // If they scroll down, don't allow swipe as well because Scroll_lines procedure doesn't set to Active_line and Top_line properly
                if (allTenders.Tend_Totals.Change <= 0)
                {
                    if (allTenders.Tend_Totals.Change <= 0)
                    {
                        cardNumber = "";
                    }
                    return new CardSummary();
                }

                // vscLines.Enabled = false; // Jan 05, 2009
                var txtSwipeData = cardNumber;
                cardNumber = "";
                // set Swipe_String for CC object and get the tender for the card

                cc = new Credit_Card();

                if (StPos > 1)
                {
                    _creditCardManager.SetSwipeString(ref cc, Strings.Right(txtSwipeData, txtSwipeData.Length - (StPos - 1)));
                }
                else
                {
                    _creditCardManager.SetSwipeString(ref cc, txtSwipeData);
                }
            }
            else
            {
                double result;
                if (!double.TryParse(cardNumber, out result))
                {
                    MessageType temp_VbStyle5 = (int)MessageType.Critical + MessageType.OkOnly;
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)77,
                        null, temp_VbStyle5);
                    return null;
                }
                _creditCardManager.SetCardnumber(ref cc, cardNumber);
            }

            tenderCode = _creditCardManager.Find_TenderCode(ref sale, ref cc);

            WriteToLogFile("Found the tender code from tender autorecognition " + tenderCode);

            if (!string.IsNullOrEmpty(tenderCode))
            {
                cardForm.TenderClass = _tenderService.GetTendClass(tenderCode);
                cardForm.TenderCode = tenderCode;
                //  - need to check whether we used the same tender before
                if (allTenders.Tend_Totals.Tend_Used != 0) //   this loop is unnecessary if this is the first tender used
                {
                    for (int i = 0; i <= allTenders.Count - 1; i++)
                    {
                        //if ((policyManager.COMBINECR && allTenders[i+1].Credit_Card.Crd_Type == "C") || (policyManager.COMBINEFLEET && allTenders[i+1].Credit_Card.Crd_Type == "F" && TenderCode != policyManager.ARTender) || (policyManager.ThirdParty && Variables.Milliplein_Renamed.CombineThirdParty && allTenders[i+1].Credit_Card.Crd_Type == "T"))
                        if ((_policyManager.COMBINECR
                            && allTenders[i + 1].Credit_Card.Crd_Type == "C")
                            || (_policyManager.COMBINEFLEET
                            && allTenders[i + 1].Credit_Card.Crd_Type == "F"
                            && tenderCode != _policyManager.ARTender))
                        {
                            if (Strings.UCase(Convert.ToString(allTenders[i + 1].Tender_Name))
                                != _policyManager.ARTender
                                && Strings.UCase(Convert.ToString(allTenders[i + 1].Credit_Card.Crd_Type)) == cc.Crd_Type.ToUpper()
                                && allTenders[i + 1].Amount_Used != 0)
                            {
                                MessageType tempVbStyle = (int)MessageType.Information + MessageType.OkOnly;
                                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)42, " - " + _resourceManager.GetResString(offSet, (short)1440), tempVbStyle); //" This tender is already used. "
                                return null;
                            }
                        }
                        else if ((allTenders[i + 1].Tender_Code != "ACK")
                                 && (Strings.UCase(Convert.ToString(allTenders[i + 1].Tender_Name)) == tenderCode.ToUpper())
                            && allTenders[i + 1].Amount_Used != 0)
                        {
                            MessageType temp_VbStyle2 = (int)MessageType.Information + MessageType.OkOnly;
                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)42, " - " + _resourceManager.GetResString(offSet, (short)1440), temp_VbStyle2); //" This tender is already used. "
                            Variables.blChargeAcct = false;
                            return null;
                        }
                    }
                }

                //  Gasking Charges
                if (tenderCode == _policyManager.ARTender) // "ACCOUNT" Then
                {
                    sale.Customer.AR_Customer = true;
                    sale.Customer.LoyaltyCard = cc.Cardnumber;
                    sale.Customer.LoyaltyCardSwiped = true; // 

                    if (_policyManager.RSTR_PROFILE)
                    {
                        sale.Customer.CardProfileID = _customerService.GetCustomerCardProfile(sale.Customer.LoyaltyCard);
                    }

                    if (!string.IsNullOrEmpty(cc.CustomerCode))
                    {
                        sale.Customer.Code = cc.CustomerCode;
                        var oldCustomer = new Customer
                        {
                            PointCardNum = sale.Customer.PointCardNum,
                            Balance_Points = sale.Customer.Balance_Points,
                            PointCardSwipe = sale.Customer.PointCardSwipe,
                            Points_Redeemed = sale.Customer.Points_Redeemed
                        };
                        Variables.blChargeAcct = true; //-   - not to clear point card info , scanned at kick back hole screen.
                        sale.Customer = _customerManager.LoadCustomer(cc.CustomerCode);
                        sale.Customer.PointCardNum = oldCustomer.PointCardNum;
                        sale.Customer.Balance_Points = oldCustomer.Balance_Points;

                        sale.Customer.PointCardSwipe = oldCustomer.PointCardSwipe;
                        sale.Customer.Points_Redeemed = oldCustomer.Points_Redeemed;
                        sale.Customer.AR_Customer = true;
                        sale.Customer.LoyaltyCard = cc.Cardnumber;
                        sale.Customer.LoyaltyCardSwiped = true; // 


                        if (_policyManager.RSTR_PROFILE)
                        {
                            sale.Customer.CardProfileID = _customerService.GetCustomerCardProfile(sale.Customer.LoyaltyCard);
                        }
                        _saleManager.SaveTemp(ref sale, sale.TillNumber); // November 15, 2013 Nicolette added to save customer code in SaleHead; crash recovery missing the customer code resulted in issues with AR Payment_Renameds
                                                                          //  - To process the linked Kickbackcard when using Gasking Card at tender screen
                                                                          // Based on GK - If scanned a ickback card at the Kickbackscreen and then swiping the Gasking card- if a kickback linked to Gasking card that should be used. If there is no
                        CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);                                         //linked card use the swiped card.

                    }
                    short ans = 1;
                    if (_policyManager.RSTR_PROFILE && verifyRestriction)
                    {
                        if (cc.Crd_Type == "F" && cc.GiftType != "W")
                        {

                            if (!string.IsNullOrEmpty(cc.CardProfileID))
                            {
                                //  If using profile, I need to bring back the screen here . Otherwise once the message is asked it will be stuck under the  salemain ( We were already doing this at the end- with profile, it is too late...)
                                var Prfl = new CardProfile();
                                if (CardProfileValidation(ref Prfl, allTenders, sale, ref promptMessages, cc.CardProfileID, cc.Cardnumber))
                                {
                                    //Scott's comment: Can there be a prompt if the customer buys some snacks and fuel,
                                    //however only fuel is authorized for purchase.
                                    // if the customer does not want those products, the sale would have to be completed, then those items would have to be returned.
                                    //I see cashiers forgetting to return these items or not notice and tender the rest to cash to finish the sale.

                                    if (Prfl.RestrictedUse)
                                    {
                                        messages[1] = "\'" + Prfl.ProfileID + "\'";
                                        MessageType temp_VbStyle3 = MessageType.YesNo;
                                        var messageStyle = _resourceManager.CreateMessage(offSet, 0, (short)482, messages, temp_VbStyle3);
                                        cardForm.ValidationMessages.Add(messageStyle);
                                        if (ans == (int)MsgBoxResult.No)
                                        {
                                            SaveSaleProfileID(sale, false, cc.CardProfileID);
                                            Prfl.PurchaseAmount = 0;
                                            Trans_Amount = (decimal)Prfl.PurchaseAmount;
                                            Variables.blChargeAcct = false;
                                        }

                                    }
                                    //  - to allow partial use if outstanding balance is partial- get confirmation and proceed
                                    if (Prfl.PartialUse)
                                    {
                                        if (Convert.ToDouble(grossTotal - allTenders.Tend_Totals.Tend_Used) > Prfl.PurchaseAmount) //  - if there is partial use but the remaining is paid by another tender no need to ask the question
                                        {
                                            messages[1] = "\'" + Prfl.ProfileID + "\'";
                                            messages[2] = Prfl.Reason;

                                            MessageType temp_VbStyle4 = MessageType.YesNo;
                                            var messageStyle = _resourceManager.CreateMessage(offSet, 0, (short)477, messages, temp_VbStyle4);
                                            cardForm.ValidationMessages.Add(messageStyle);
                                            if (ans == (int)MsgBoxResult.No)
                                            {
                                                SaveSaleProfileID(sale, false, cc.CardProfileID);
                                                Prfl.PurchaseAmount = 0;
                                                Trans_Amount = (decimal)Prfl.PurchaseAmount;
                                            }
                                        }
                                    }

                                    if (ValidCardProfilePrompts(ref Prfl, sale, ref promptMessages, cc.Cardnumber))
                                    {
                                        cc.PONumber = Prfl.PONumber;
                                        SaveSaleProfileID(sale, true, cc.CardProfileID);
                                        //  - consider the amount paid by other allTenders if using profile
                                        //                                    Trans_Amount = Prfl.PurchaseAmount 'txtAmount(Active_Line).Text = Prfl.PurchaseAmount
                                        Trans_Amount = Convert.ToDecimal(Prfl.PurchaseAmount) < grossTotal - allTenders.Tend_Totals.Tend_Used
                                            ? Convert.ToDecimal(Prfl.PurchaseAmount) : grossTotal - allTenders.Tend_Totals.Tend_Used; //  '= Prfl.PurchaseAmount
                                                                                                                                      // 
                                    }
                                    else
                                    {
                                        SaveSaleProfileID(sale, false, cc.CardProfileID);
                                        Prfl.PurchaseAmount = 0;
                                        Trans_Amount = (decimal)Prfl.PurchaseAmount;
                                        messages[1] = "\'" + Prfl.ProfileID + "\'";
                                        messages[2] = Prfl.Reason;
                                        MessageType temp_VbStyle5 = (int)MessageType.Information + MessageType.OkOnly;
                                        var messageStyle = _resourceManager.CreateMessage(offSet, 0, (short)483, messages, temp_VbStyle5);
                                        cardForm.ValidationMessages.Add(messageStyle);

                                    }

                                }
                                else
                                {
                                    if (Prfl.PurchaseAmount == 0)
                                    {
                                        SaveSaleProfileID(sale, false, cc.CardProfileID);
                                        Trans_Amount = (decimal)Prfl.PurchaseAmount;
                                        messages[1] = "\'" + Prfl.ProfileID + "\'";
                                        messages[2] = Prfl.Reason;
                                        MessageType temp_VbStyle6 = (int)MessageType.Information + MessageType.OkOnly;
                                        var messageStyle = _resourceManager.CreateMessage(offSet, 0, (short)483, messages, temp_VbStyle6);
                                        cardForm.ValidationMessages.Add(messageStyle);
                                    }
                                }
                                if (Prfl.AskForPO)
                                    cardForm.PONumber = _resourceManager.GetResString(offSet, (short)470);
                            }
                            else
                            {
                                Trans_Amount = grossTotal - allTenders.Tend_Totals.Tend_Used;
                            }
                        }
                    }
              
                    else
                    {
                        Trans_Amount = grossTotal - allTenders.Tend_Totals.Tend_Used;
                    }

                 

                    if (Trans_Amount != 0)
                    {
                        allTenders.Card_Authorized = true;
                        cc.Result = "0";
                    }
                    // find the tender linked to swiped card    
                    cardForm.Amount = Trans_Amount.ToString();
                    Variables.blChargeAcct = false;
                    cardForm.CardNumber = cc.Cardnumber;
                }


                string cBuf;
                if (!cc.Card_Swiped)
                {
                    if (cc.CheckExpiryDate)
                    {
                        if (!cc.Expiry_Date_Valid)
                        {
                            // Invalid Expiry Date
                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 91, null,
                                CriticalOkMessageType);
                            errorMessage.StatusCode = HttpStatusCode.BadRequest;
                            return null;
                        }
                        if (_creditCardManager.Card_Is_Expired(ref cc))
                        {
                            cBuf = cc.Expiry_Month + "/" + cc.Expiry_Year;
                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 92, cBuf,
                                CriticalOkMessageType);
                            errorMessage.StatusCode = HttpStatusCode.BadRequest;
                            return null;
                        }
                        if (cc.Crd_Type == "C" &&
                            DateAndTime.DateDiff(DateInterval.Year,
                                DateAndTime.DateSerial(int.Parse("20" + Strings.Left(cc.Expiry_Date, 2)),
                                    int.Parse(Strings.Right(cc.Expiry_Date, 2)), 1), DateTime.Now) <= -20)
                        //Added by Dmitry to check if a card has exp date less then 20 years or more
                        {
                            cBuf = cc.Expiry_Month + "/" + cc.Expiry_Year;
                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 91, cBuf,
                                CriticalOkMessageType);
                            errorMessage.StatusCode = HttpStatusCode.BadRequest;
                            return null;
                        }
                    }
                }
                else
                {
                    if (cc.CheckExpiryDate)
                    {
                        if (!cc.Expiry_Date_Valid)
                        {
                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 91, null,
                                CriticalOkMessageType);
                            errorMessage.StatusCode = HttpStatusCode.BadRequest;
                            return null;
                        }
                        if (_creditCardManager.Card_Is_Expired(ref cc))
                        {
                            cBuf = cc.Expiry_Month + "/" + cc.Expiry_Year;
                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 92, cBuf,
                                CriticalOkMessageType);
                            errorMessage.StatusCode = HttpStatusCode.BadRequest;
                            return null;
                        }
                        if (cc.Crd_Type == "C"
                            && DateAndTime.DateDiff(DateInterval.Year,
                                DateAndTime.DateSerial(int.Parse("20" + Strings.Left(cc.Expiry_Date, 2)),
                                    int.Parse(Strings.Right(cc.Expiry_Date, 2)), 1), DateTime.Now) <= -20)
                        {
                            cBuf = cc.Expiry_Month + "/" + cc.Expiry_Year;
                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 91, cBuf,
                                CriticalOkMessageType);
                            errorMessage.StatusCode = HttpStatusCode.BadRequest;
                            return null;
                        }
                    }

                    //tenderCode = cc.TendCode;
                    float transAmount = 0;
                    float.TryParse(amountEntered, out transAmount);
                    cc.Trans_Amount = transAmount;
                }

                if (_creditCardManager.CardInNcf(ref cc)) // If Negative Card give the message from table Cardmeesage and donot process
                {
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 77, cc.Decline_Message, CriticalOkMessageType); ;
                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                if (!_creditCardManager.CardInPcf(ref cc))
                {
                    //       Invalid Card. Contact the Card Issuer
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 78, null, CriticalOkMessageType);
                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                var selectedTender = GetSelectedTender(allTenders, tenderCode);
                if (selectedTender == null)
                {
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 77, "", CriticalOkMessageType); ;
                    errorMessage.StatusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                selectedTender.Credit_Card.Crd_Type = _tenderService.GetCardType(tenderCode);
                if (_creditCardManager.CardIsValid(ref cc))
                {
                    // If the user swiped another type of card force him to select the right card
                    if (!string.IsNullOrEmpty(selectedTender.Credit_Card.Crd_Type) && selectedTender.Credit_Card.Crd_Type != cc.Crd_Type &&
                        tenderCode != _policyManager.ARTender)
                    {
                        // You selected a different card.
                        errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 68, null, ExclamationOkMessageType);
                        errorMessage.StatusCode = HttpStatusCode.BadRequest;
                        return null;
                    }

                    // check the purchase limit for used card
                    if ((cc.PurchaseLimit > 0) && (Conversion.Val(amountEntered) > cc.PurchaseLimit))
                    {
                        errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, 66, cc.PurchaseLimit);
                        amountEntered = (cc.PurchaseLimit).ToString();
                    }
                    // PM)- if customerverify card is checked don't process this card as fleet - it should show invalid card ( this is to eliminate gasking cards going as fleet instead of account- verifycustomercard - means process as private account card)
                    //  - For fleet card linked to AR customer and also it is a customer card and also limked to profile
                    //, if we are not using the AR account, need to validate profile and related fleet card settings, but shouldn't charge the account- since they selected fleet card
                    if (cc.Crd_Type == "F")
                    {
                        var tender = GetSelectedTender(allTenders, tenderCode);
                        if (cc.VerifyCardNumber && (tender.Tender_Class == "CRCARD" ||
                            tender.Tender_Class == "DBCARD" || tender.Tender_Class == "FLEET"))
                        {
                            MessageType temp_VbStyle11 = (int)MessageType.Information + MessageType.OkOnly;
                            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)65, null, temp_VbStyle11);
                            return null;
                        }
                        if (_creditCardManager.ValidCustomerCard(ref cc)) // this is for getting profile info
                        {
                        }
                    }


                    if (cc.Crd_Type == "F" && cc.GiftType != "W" )
                    {
                        cc.Response = "";
                        cc.Balance = 0;

                        // For Fleets Cards we need aditional input from the user
                        //end - Svetlana
                        if (cc.AskDriverNo || cc.AskIdentificationNo || cc.AskOdometer || cc.AskProdRestrictCode || cc.AskVechicle) // if POS are not using a PinPad, then ask questions through screen
                        {
                            //  Need to check CardProdLink table even when no ?'s are asked
                            // Load the card product code from CardProductLink table
                            _creditCardManager.GetCardProductCodes(ref sale, cc);
                            var curRetAmt = _creditCardManager.InsertData(sale, cc, cc.FuelServiceType);
                            amountEntered = (curRetAmt).ToString();
                            if (curRetAmt == 0)
                            {
                                // "Purchase not allowed using this card.", vbInformation + vbOKOnly, "Tenders_Renamed"
                                MessageType temp_VbStyle12 = (int)MessageType.Information + MessageType.OkOnly;
                                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)65, null, temp_VbStyle12);
                                return null;
                            }
                        }
                        cc.TerminalType = "P"; // for POS pay inside only
                    }
                    float ValidAmount = 0;
                    string strLineNum = string.Empty;


                    //  ( For Gasking)
                    if ((cc.Crd_Type == "T" || cc.Crd_Type == "L" || cc.Crd_Type == "F") && verifyRestriction)
                    {
                        ValidAmount = _creditCardManager.GetValidProductForCard(sale, cc, ref strLineNum);

                       
                        
                        // 2014 01 15 - Reji - for WEX Fleet card - to invoke the prompt for manual card entry // fleet selected Swipe
                        //if (cc.Crd_Type == "F" && cc.GiftType.ToUpper() == "W" && cc.CardPrompts.Count <= 0)
                        //{
                        //        _creditCardManager.SetCardnumber(ref cc, cardNumber);
                        //}
                        CardPrompt CardPrompt_Renamed;
                        //   to display prompts for optional data
                        if (!string.IsNullOrEmpty(cc.OptDataProfileID) & ValidAmount != 0)
                        {
                            foreach (CardPrompt tempLoopVar_CardPrompt_Renamed in cc.CardPrompts)
                            {
                                CardPrompt_Renamed = tempLoopVar_CardPrompt_Renamed;
                                Variables.STFDNumber = "";
                                errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(offSet, CardPrompt_Renamed.PromptMessage, (short)20, CardPrompt_Renamed.PromptMessage, MessageType.OkCancel);
                                {
                                    CardPrompt_Renamed.PromptAnswer = Variables.STFDNumber.Trim();
                                }
                            }
                        }
                        //   end

                        if (cc.GiftType != "W")
                        {
                            // Aug 05, 2011, Nicolette added (cc.OptDataProfileID <> "" And ValidAmount = 0) condition to the next line
                            if ((_policyManager.RSTR_PROFILE == false && string.IsNullOrEmpty(cc.CardProfileID))
                                || (!string.IsNullOrEmpty(cc.OptDataProfileID) && ValidAmount == 0 && !cc.VerifyCardNumber)) //   added  only extra criteria - if need to check profile , shouldn't exit from here
                            {

                                if (ValidAmount == 0)
                                {
                                    if (sale.Sale_Totals.Gross > 0)
                                    {
                                        //                    MsgBox "Purchase not allowed using this card.", vbInformation + vbOKOnly, "Tenders_Renamed"
                                        MessageType temp_VbStyle13 = (int)MessageType.Information + MessageType.OkOnly;
                                        errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)65, null, temp_VbStyle13);

                                    }
                                    else if (sale.Sale_Totals.Gross == 0)
                                    {


                                        MessageType temp_VbStyle14 = (int)MessageType.OkOnly + MessageType.Critical;
                                        errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)1435, null, temp_VbStyle14);

                                    }
                                    else
                                    {
                                        //You cannot refund by using this Card~Refund Not Allowed
                                        MessageType temp_VbStyle15 = (int)MessageType.OkOnly + MessageType.Critical;
                                        errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)1482, null, temp_VbStyle15);

                                    }
                                    return null;
                                }

                                //If ValidAmount < 0 And cc.Crd_Type = "L" Then
                                if (ValidAmount < 0 && cc.Crd_Type == "L" && cc.GiftType != "A")
                                {
                                    //You cannot refund by using this Card~Refund Not Allowed
                                    MessageType temp_VbStyle16 = (int)MessageType.OkOnly + MessageType.Critical;
                                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)1482, null, temp_VbStyle16);
                                    return null;
                                }
                            }
                        }
                    }

                    if (cc.Crd_Type == "T" || cc.Crd_Type == "F")
                    {
                        ValidAmount = ValidAmount - Variables.MillExchangeAmount;
                        if (selectedTender.Amount_Entered > Convert.ToDecimal(ValidAmount) || ValidAmount > 0)
                        {
                            amountEntered = Convert.ToDecimal(ValidAmount) > Trans_Amount ? Trans_Amount.ToString("#0.00") : ValidAmount.ToString("#0.00");
                        }
                    }
                    //  ( For Gasking) TODO : Sonali
                    //if (_policyManager.Use_KickBack && sale.Customer.PointCardNum != "" && sale.Customer.PointCard_Registered && sale.Customer.Points_Redeemed != 0)
                    //{
                    //    Set_Tender("K", (decimal)sale.Customer.Points_Redeemed, "", sale.Customer.Points_ExchangeRate, false,grossTotal,ref sale); //'' SA.Customer.DollarRedeemed


                    //    //shiny added on sept14, 2009 - crash recovery
                    //    // modStringPad.WriteToLogFile("Set_tender for kickback" + System.Convert.ToString(Chaps_Main.SA.Sale_Num) + "salemain enable status " + System.Convert.ToString(SaleMain.Default.Enabled));
                    //    modStringPad.WriteToLogFile("Saving temp tender for kickback tender" + System.Convert.ToString(Chaps_Main.SA.Sale_Num));

                    //    _saleManager.SaveTemp( ref sale,tillNumber);
                    //    //   modStringPad.WriteToLogFile("Finished Saving temp tender for kickback" + System.Convert.ToString(Chaps_Main.SA.Sale_Num) + "Gross" + System.Convert.ToString(Gross_Total) + "used " + Tenders_Renamed.Tend_Totals.Tend_Used);
                    //    //shiny end -sept14, 2009
                    //}

                    if (verifyRestriction)
                    {
                        CardRestrictionValidation(selectedTender, allTenders, sale, cc,
                                         ref promptMessages, ref amountEntered, grossTotal, false, verifyAccount, out errorMessage);
                        cardForm.PONumber = cc.PONumber;
                        if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                        {
                            cardForm.ValidationMessages.Add(errorMessage.MessageStyle);

                            errorMessage = new ErrorMessage();
                        }
                    }
                    // Nov 02, 2009 Nicolette: if the tender is set as inactive, don't continue
                    // boolInactive can be True only if processed Tenders_Renamed is a combined card
                    // above If is the only way boolInactive can be True, otherwise is False
                    // need to set it here if the card is combined, for regular Tenders_Renamed inactive
                    // Tenders_Renamed are not loaded into Tenders_Renamed collection so they are not accessible
                    if (selectedTender.Inactive)
                    {
                        MessageType temp_VbStyle29 = (int)MessageType.Exclamation + MessageType.OkOnly;
                        errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)77, selectedTender.Tender_Name, temp_VbStyle29);
                        return null;
                    }
                    // 
                    // length tendercode checking end
                    cardForm.TenderCode = GetSelectedTender(allTenders, tenderCode).Tender_Code;
                    var askPin = cc.AskPCFPin;
                    var profileMessages = cardForm.ValidationMessages;
                    if (string.IsNullOrEmpty(amountEntered))
                    {
                        List<Report> transactReports = null;
                        cardForm = SaleTend_Keydown(ref allTenders, sale, userCode, tenderCode, ref amountEntered, transactionType,
                            grossTotal, cc, out transactReports, out errorMessage);
                    }
                    else
                    {
                        cardForm.Amount = amountEntered;
                        if (selectedTender.Tender_Class == "CRCARD")
                        {
                            selectedTender.Credit_Card.Crd_Type = "C";
                            cardForm.SelectedCard = CardForm.Credit;
                        }
                        //  - to get the debit card type also
                        if (selectedTender.Tender_Class == "DBCARD")
                        {
                            selectedTender.Credit_Card.Crd_Type = "D";
                            cardForm.SelectedCard = CardForm.Debit;
                        }
                        // 
                        if (selectedTender.Tender_Class == "FLEET")
                        {
                            selectedTender.Credit_Card.Crd_Type = "F";
                            cardForm.SelectedCard = CardForm.Fleet;
                        }
                    }

                    if (cc.Crd_Type == "F" && cc.GiftType == "W")
                    {
                        var profileId = _wexService.GetWexProfileId();
                        cc.CardProfileID = profileId;
                        var cardPrompts = new CardPrompts();
                        _wexManager.GetProfilePrompts(ref cardPrompts,  cc, profileId);
                        cc.CardPrompts = cardPrompts;
                        for (var i = 1; i <= cardPrompts.Count; i++)
                        {
                            var prm = cardPrompts[i];
                            promptMessages.Add(prm.PromptMessage);
                        }
                    }
                    cardForm.AskPin = askPin;
                    cardForm.Pin = cc.PCFPIN;
                    cardForm.CardNumber = cc.Cardnumber;
                    cardForm.PromptMessages = promptMessages;
                    cardForm.ValidationMessages = profileMessages;
                    cardForm.ProfileId = cc.CardProfileID;
                    cardForm.PONumber = cc.PONumber;
                    

                    if (tenderCode == _policyManager.ARTender)
                    {
                        cardForm.SelectedCard = CardForm.Fleet;
                        cardForm.IsArCustomer = true;
                    }
                    var sale1 = _saleManager.GetCurrentSale(saleNumber, tillNumber, 1, userCode, out errorMessage);
                    CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
                    return cardForm;
                }
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)94,
                    _creditCardManager.Invalid_Reason(ref cc), CriticalOkMessageType);
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }
            MessageType temp_VbStyle = (int)MessageType.Critical + MessageType.OkOnly;
            errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 38, (short)84, null, temp_VbStyle);
            return null;

        }

        /// <summary>
        /// Get SaleSummary for AR Account Payment
        /// </summary>
        /// <param name="request">AR payment</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>Sale summary reponse</returns>
        public SaleSummaryResponse SaleSummaryForArPayment(ArPaymentRequest request, string userCode,
           out ErrorMessage error)
        {
            SaleSummaryResponse result = new SaleSummaryResponse();
            error = new ErrorMessage();
            string transaction_Type = "ARPay";
            var till = _tillService.GetTill(request.TillNumber);
            if (till == null)
            {
                error.MessageStyle = new MessageStyle
                {
                    Message = "Till does not exists"
                };
                error.StatusCode = HttpStatusCode.NotFound;
                return null;
            }
            var sale = _saleManager.GetCurrentSale(request.SaleNumber, request.TillNumber,
              request.RegisterNumber, userCode, out error);
            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return null;
            }
            AR_Payment arPay = new AR_Payment();

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return null;
            }
            bool canPerformARPay = _policyManager.CREDTERM
                                   && _policyManager.USE_CUST
                                   && (sale.Sale_Lines.Count == 0)
                                   && (!request.IsReturnMode);
            var security = _policyManager.LoadSecurityInfo();
            if (security.BackOfficeVersion == "Lite")
            {
                canPerformARPay = false;
            }
            if (!canPerformARPay)
            {
                error.MessageStyle = new MessageStyle { Message = "AR Payment cannot be done" };
                return null;
            }
            arPay.Amount = request.Amount;
            arPay.Penny_Adj = Convert.ToDecimal(_policyManager.PENNY_ADJ ? modGlobalFunctions.Calculate_Penny_Adj(arPay.Amount) : 0);
            arPay.Sale_Num = sale.Sale_Num;
            var customers = _customerManager.SearchArCustomer(request.CustomerCode, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return null;
            }
            arPay.Customer = _customerManager.LoadCustomer(request.CustomerCode);
            sale.Sale_Totals.Penny_Adj = arPay.Penny_Adj;
            if (arPay.Amount == 0)
            {
                // Amount paid must be greater than zero
                //Chaps_Main.DisplayMessage(0, (short)8192, temp_VbStyle, null, (byte)0);
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8192, null, CriticalOkMessageType)
                };
                return null;
            }
            Top_Box(ref arPay);
            CacheManager.AddArPayment(request.SaleNumber, arPay);

            result.SaleSummary.Add(arPay.TopLeft, arPay.TopRight);
            result.Tenders = GetAllTender(request.SaleNumber, request.TillNumber, transaction_Type, userCode, false,
               "", out error);
            return result;
        }

        /// <summary>
        /// Method to save store credits
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="storeCredits">Store credits</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Tenders</returns>
        public Tenders SaveStoreCredits(int saleNumber, int tillNumber, List<Store_Credit> storeCredits,
            string transactionType, string userCode, string tenderCode, out ErrorMessage errorMessage)
        {
            var allStoreCredits = GetStoreCredits(saleNumber, tillNumber, userCode, tenderCode, "", transactionType,
                out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }

            var isInvalidStoreCredit = false;
            if (allStoreCredits == null || allStoreCredits.Count == 0)
            {
                isInvalidStoreCredit = true;
                allStoreCredits = new List<Store_Credit>();
            }
            foreach (var storeCredit in storeCredits)
            {
                if (!allStoreCredits.Any(t => t.Number == storeCredit.Number))
                {
                    isInvalidStoreCredit = true;
                    break;
                }
            }
            if (isInvalidStoreCredit)
            {
                errorMessage.MessageStyle = new MessageStyle { Message = "Request is invalid" };
                return null;
            }

            _tenderService.RemoveUsedSc(saleNumber);
            _tenderService.AddScToDbTemp(saleNumber, tillNumber, storeCredits);
            var storeCreditsAmount = storeCredits.Sum(g => g.Amount);
            var tenders = GetAllTender(saleNumber, tillNumber, transactionType, userCode, false, string.Empty,
                 out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            var till = _tillService.GetTill(tillNumber);
            List<Report> transactReports = null;
            return UpdateTenders(sale.Sale_Num, sale.TillNumber, transactionType, userCode, false, tenderCode, storeCreditsAmount.ToString(), out transactReports, out errorMessage);
        }

        /// <summary>
        /// Method to test if cards are defined
        /// </summary>
        /// <returns>True or false</returns>
        public bool AreCardsDefined()
        {
            return _tenderService.IsCardAvailable();
        }

        /// <summary>
        /// Method to get sale summary for fleet payment
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="amount">Amount</param>
        /// <param name="isSwiped">Swiped or not</param>
        /// <param name="userCode">User code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="error">Error</param>
        /// <returns>Sale summary repsonse</returns>
        public SaleSummaryResponse SaleSummaryForFleetPayment(string cardNumber, decimal amount,
            bool isSwiped, string userCode, int tillNumber, int saleNumber, out ErrorMessage error)
        {
            SaleSummaryResponse result = new SaleSummaryResponse();
            error = new ErrorMessage();
            string transaction_Type = "Payment";
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!_policyManager.U_AllowFlPay)
            {

                MessageType temp_VbStyle = (int)MessageType.Exclamation + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 12, (short)83, null, temp_VbStyle);
                error.StatusCode = System.Net.HttpStatusCode.Forbidden;
                return null;
            }
            var creditCard = Validate_Entry(cardNumber, isSwiped, out error);
            if (creditCard == null)
            {
                return null;
            }
            var till = _tillService.GetTill(tillNumber);
            if (till == null)
            {
                error.MessageStyle = new MessageStyle
                {
                    Message = "Till does not exists"
                };
                error.StatusCode = HttpStatusCode.NotFound;
                return null;
            }
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber,
              1, userCode, out error);
            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return null;
            }
            Payment paymnet = new Payment
            {
                Card = creditCard
            };

            if (sale.Sale_Lines.Count != 0)
            {
                error.MessageStyle = new MessageStyle { Message = "Fleet Payment cannot be done" };
                return null;
            }
            paymnet.Amount = amount;
            paymnet.Penny_Adj = Convert.ToDecimal(_policyManager.PENNY_ADJ ? modGlobalFunctions.Calculate_Penny_Adj(paymnet.Amount) : 0);
            sale.Sale_Totals.Penny_Adj = paymnet.Penny_Adj;
            if (paymnet.Amount <= 0)
            {
                // Amount paid must be greater than zero
                //Chaps_Main.DisplayMessage(0, (short)8192, temp_VbStyle, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8192, null, CriticalOkMessageType)
                };
                return null;
            }
            Top_Box(ref paymnet);
            CacheManager.AddFleetPayment(saleNumber, paymnet);

            result.SaleSummary.Add(paymnet.TopLeft, paymnet.TopRight);
            result.Tenders = GetAllTender(saleNumber, tillNumber, transaction_Type,
                userCode, false, "", out error);
            return result;
        }

        // ==================================================================================
        // Complete an ROA Payment_Renamed
        // ==================================================================================
        /// <summary>
        /// Method to finish payment using fleet
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="till">Till</param>
        /// <param name="issueSc">Issue store credit</param>
        /// <param name="requireOpen">Require open</param>
        /// <param name="changeDue">Change due</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Report content</returns>
        public List<Report> Finishing_Payment(Tenders tenders, int saleNumber, string userCode,
            Till till, bool issueSC, out bool Require_Open, out string changeDue, out ErrorMessage errorMessage)
        {
            changeDue = "0.00";
            errorMessage = new ErrorMessage();
            var reports = new List<Report>();
            string SC__policyManager;
            string GC__policyManager;
            Require_Open = false;
            Tender tender_Renamed = default(Tender);
            if (Math.Round(Convert.ToDouble(tenders.Tend_Totals.Change), 2) > 0.0D)
            {
                //Show_Message   "Not Enough Paid"
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)98, null, MessageType.OkOnly);
                errorMessage.StatusCode = HttpStatusCode.NotFound;
            }
            else
            {
                Store_Credit SC = null;
                var scCredits = _tenderService.GetScCredit(saleNumber, till.Number);
                if (scCredits != null)
                {
                    _tenderService.RemoveSc(saleNumber, scCredits);
                }
                var gcCredits = _tenderService.GetGcCredit(saleNumber, till.Number);
                if (gcCredits != null)
                {
                    _tenderService.RemoveGc(saleNumber, gcCredits);
                }
                var payment = CacheManager.GetFleetPayment(saleNumber);
                var user = _loginManager.GetUser(userCode);


                if (_policyManager.Store_Credit)
                {


                    foreach (Tender tempLoopVar_tender_Renamed in tenders)
                    {
                        tender_Renamed = tempLoopVar_tender_Renamed;
                        if (tender_Renamed.Amount_Used != 0 && tender_Renamed.Tender_Code == "SC" && tenders.Tend_Totals.Change != 0)
                        {



                            SC__policyManager = Convert.ToString(_policyManager.CRED_CHANGE);

                            if (SC__policyManager == "Always")
                            {
                                var scReceipt = _receiptManager.Issue_Store_Credit(saleNumber, payment.Customer.Code, user,
                                     tender_Renamed, (float)Math.Abs(tenders.Tend_Totals.Change), out SC);
                                tenders.Tend_Totals.Change = 0;
                                reports.Add(scReceipt);
                            }
                            else if (SC__policyManager == "Choice")
                            {
                                if (issueSC)
                                {

                                    var scReceipt = _receiptManager.Issue_Store_Credit(saleNumber, payment.Customer.Code,
                                         user, tender_Renamed, (float)Math.Abs(tenders.Tend_Totals.Change)
                                         , out SC);
                                    reports.Add(scReceipt);
                                    tenders.Tend_Totals.Change = 0;
                                }
                            }
                            break;

                            // Change Handling for Gift Certificates


                            //                       Tender.Tender_Code = "GC" And _
                            //                       Tenders_Renamed.Tend_Totals.Change <> 0 Then
                        }
                        else if (tender_Renamed.Amount_Used != 0 && tender_Renamed.Tender_Class == "GIFTCERT" && tenders.Tend_Totals.Change != 0)
                        {



                            if (_policyManager.GIFTCERT)
                            {




                                GC__policyManager = System.Convert.ToString(_policyManager.GC_CHANGE);


                                if (GC__policyManager == "Always")
                                {
                                    var scReceipt = _receiptManager.Issue_Store_Credit(saleNumber, payment.Customer.Code, user,
                                     tender_Renamed, (float)Math.Abs(tenders.Tend_Totals.Change), out SC);
                                    tenders.Tend_Totals.Change = 0;
                                    reports.Add(scReceipt);
                                }
                                else if (GC__policyManager == "Choice")
                                {
                                    if (issueSC)
                                    {
                                        var scReceipt = _receiptManager.Issue_Store_Credit(saleNumber, payment.Customer.Code, user,
                                     tender_Renamed, (float)Math.Abs(tenders.Tend_Totals.Change), out SC);
                                        tenders.Tend_Totals.Change = 0;
                                        reports.Add(scReceipt);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                var Base_Curr = _policyManager.BASECURR;
                if (!string.IsNullOrEmpty(Base_Curr))
                {
                    var updatedCash = tenders[Base_Curr].Amount_Used + tenders.Tend_Totals.Change;
                    _tillService.UpdateCash(till.Number, updatedCash);
                }



                if (tenders.Tend_Totals.Change != 0 || _policyManager.OPEN_DRAWER == "Every Sale")
                {

                    Require_Open = true;
                }
                else
                {
                    foreach (Tender tempLoopVar_tender_Renamed in tenders)
                    {
                        tender_Renamed = tempLoopVar_tender_Renamed;
                        if (tender_Renamed.Amount_Used != 0 && tender_Renamed.Open_Drawer)
                        {
                            Require_Open = true;
                            break;
                        }
                    }
                }
                //Out of Scope

                //foreach (Tender tempLoopVar_tender_Renamed in tenders)
                //{
                //    tender_Renamed = tempLoopVar_tender_Renamed;
                //    if (tender_Renamed.Amount_Used != 0)
                //    {
                //        switch (tender_Renamed.Tender_Class)
                //        {
                //            case "GIFTCERT":
                //                string temp_Policy_Name = "GiftTender";
                //                if ((string)modPolicy.GetPol(temp_Policy_Name, tender_Renamed) == "EKO")
                //                {
                //                    _tenderService.SaveToTill(GC_Payment_Renamed.Sale_Num, GC_Payment_Renamed.Till_Num);
                //                    
                //                    
                //                    
                //                }
                //                break;
                //        }
                //    }



                var paymentReceipt = _receiptManager.Print_Payment(till, payment, tenders, user, DateTime.Today, DateAndTime.TimeOfDay);
                SavePayment(payment, saleNumber, (byte)till.Number, userCode, ref tenders, ref SC);
                paymentReceipt.Copies = _policyManager.PaymentReceiptCopies;
                reports.Add(paymentReceipt);
            }
            return reports;
        }

        /// <summary>
        /// Method to save profile prompt
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="cardNumber">Card number</param>
        /// <param name="profileId">Profile ID</param>
        /// <param name="cardPrompts">Card prompts</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>PO number</returns>
        public string SaveProfilePrompt(int saleNumber, int tillNumber, string cardNumber,
            string profileId, List<CardPrompt> cardPrompts, string userCode, out ErrorMessage error)
        {
            error = new ErrorMessage();
            //Condition to check weather the prompts belong to the wex or the fleet card
            if (_wexService.GetWexProfileId() == profileId)
            {
                modTPS.cc = new Credit_Card();
                modTPS.cc.CardPrompts = cardPrompts;
                Chaps_Main.SA.IsWexTenderUsed = true;
                return "";
            }
            Chaps_Main.SA.IsWexTenderUsed = false;
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 1, userCode, out error);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return null;
            }
            try
            {
                byte[] data = Convert.FromBase64String(cardNumber);
                string decodedString = Encoding.UTF8.GetString(data);
                cardNumber = decodedString;
            }
            catch
            {
                MessageType temp_VbStyle = (int)MessageType.Critical + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)77, null, temp_VbStyle);
                return null;
            }
            int StPos = 0, EndPos = 0;
            // If there is a "?" in the txtAmount text box we assume that
            // it was a card swiped and not an amount entered
            StPos = (short)(cardNumber.IndexOf(";") + 1);
            var cc = new Credit_Card();

            if (StPos > 0)
            {
                EndPos = (short)(cardNumber.Substring(StPos).IndexOf("?") + 1 + StPos);
            }

            if (EndPos > 0 & StPos > 0 & EndPos > StPos)
            {
                // vscLines.Enabled = false; // Jan 05, 2009
                var txtSwipeData = cardNumber;
                cardNumber = "";
                // set Swipe_String for CC object and get the tender for the card


                if (StPos > 1)
                {
                    _creditCardManager.SetSwipeString(ref cc, Strings.Right(txtSwipeData, txtSwipeData.Length - (StPos - 1)));
                }
                else
                {
                    _creditCardManager.SetSwipeString(ref cc, txtSwipeData);
                }
            }
            else
            {

                _creditCardManager.SetCardnumber(ref cc, cardNumber);
            }
            var Prfl = _cardManager.Loadprofile(profileId);
            if (Prfl == null)
            {
                error.MessageStyle.Message = "Invalid profile";
                return null;
            }
            Prfl.ProfileID = profileId;
            var poNumberMessage = _resourceManager.GetResString(offSet, (short)470);
            if (Prfl.AskForPO)
            {
                var poNumber = cardPrompts.FirstOrDefault(c => c.PromptMessage == poNumberMessage);
                if (poNumber == null)
                {
                    error.MessageStyle.Message = _resourceManager.GetResString(offSet, (short)471);
                    return null;
                }
                else
                {
                    if (string.IsNullOrEmpty(poNumber.PromptAnswer))
                    {
                        error.MessageStyle.Message = _resourceManager.GetResString(offSet, (short)1438) + poNumber.PromptMessage;
                        return null;
                    }
                    Prfl.PONumber = poNumber.PromptAnswer;
                }
            }
            var promptMessages = new List<string>();
            if (_cardManager.ValidProductsForProfile(ref Prfl, sale))
            {
                ValidCardProfilePrompts(ref Prfl, sale, ref promptMessages, cardNumber);
                foreach (var promptMessage in promptMessages)
                {

                    var prompt = cardPrompts.FirstOrDefault(c => c.PromptMessage == promptMessage);
                    if (prompt == null)
                    {
                        continue;
                        //error.MessageStyle.Message = _resourceManager.GetResString(offSet,(short)1438) + promptMessage;
                        //return null;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(prompt.PromptAnswer))
                        {
                            continue;
                            //error.MessageStyle.Message = _resourceManager.GetResString(offSet,(short)1438) + promptMessage;
                            //return null;
                        }
                        Prfl.CardPrompts.FirstOrDefault(p => p.PromptMessage == promptMessage).PromptAnswer = prompt.PromptAnswer;
                    }
                }
                if (Prfl.CardPrompts.Count != 0)
                {
                    _saleManager.Save_ProfilePrompt_Temp(Prfl.CardPrompts, cc.Cardnumber, Prfl.ProfileID, saleNumber, tillNumber);
                    // sale.Save_ProfilePrompt_Temp(Prfl.CardPrompts, CardNumber, Prfl.ProfileID);
                }
            }
            var card = CacheManager.GetCreditCard(sale.TillNumber, sale.Sale_Num);
            if (card != null)
            {
                card.PONumber = Prfl.PONumber;
                CacheManager.AddCreditCard(sale.TillNumber, sale.Sale_Num, card);
            }

            CacheManager.AddPoNumber(Prfl.PONumber, sale.Sale_Num, sale.TillNumber);
            return Prfl.PONumber;
        }

        /// <summary>
        /// Method to get issue store credit message
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <returns>Store credit message</returns>
        public string IssueStoreCredit(Tenders tenders)
        {
            string issueCreditMsg = string.Empty;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (_policyManager.Store_Credit)
            {
                Tender tender_Renamed;
                foreach (Tender tempLoopVar_tender_Renamed in tenders)
                {
                    tender_Renamed = tempLoopVar_tender_Renamed;
                    if (tender_Renamed.Amount_Used != 0 && tender_Renamed.Tender_Code == "SC" && tenders.Tend_Totals.Change != 0)
                    {

                        var SC__policyManager = Convert.ToString(_policyManager.CRED_CHANGE);
                        if (SC__policyManager == "Always")
                        {
                            issueCreditMsg = string.Empty;
                        }
                        else if (SC__policyManager == "Choice")
                        {
                            issueCreditMsg = _resourceManager.CreateMessage(offSet, 14, (short)97, Math.Abs(tenders.Tend_Totals.Change).ToString("0.00"), MessageType.OkOnly).Message;
                        }
                        break;
                    }
                    else if (tender_Renamed.Amount_Used != 0 && tender_Renamed.Tender_Class == "GIFTCERT" && tenders.Tend_Totals.Change != 0)
                    {
                        if (_policyManager.GIFTCERT)
                        {

                            var GC__policyManager = Convert.ToString(_policyManager.GC_CHANGE);

                            if (GC__policyManager == "Always")
                            {
                                issueCreditMsg = string.Empty;

                            }
                            else if (GC__policyManager == "Choice")
                            {
                                issueCreditMsg = _resourceManager.CreateMessage(offSet, 14, (short)97, Math.Abs(tenders.Tend_Totals.Change).ToString("0.00"), MessageType.OkOnly).Message;
                            }
                        }
                        break;
                    }
                }
            }
            return issueCreditMsg;
        }

        public bool ValidateMuliPo(int saleNumber, int tillNumber, string purchaseOrder, out ErrorMessage
            errorMessage)
        {
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, UserCode, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return false;
            }
            object[] arrMsg = new object[3];
            if (!string.IsNullOrEmpty(purchaseOrder))
            {
                var grossTotal = Math.Round(sale.Sale_Totals.Gross, 2);
                if (sale.Customer.MultiUse_PO == false && grossTotal > 0) //  - multiple chcking need to be done only for sales
                {
                    if (_customerService.UsedCustomerPo(sale.Customer.Code, purchaseOrder))
                    {
                        //                                MsgBox " Customer " & SA.Customer.Name & " is not allowing multiple use of same PO. This PO " & STFDNumber & " is already used for this customer."
                        arrMsg[1] = sale.Customer.Name;
                        arrMsg[2] = purchaseOrder;
                        var offSet = _policyManager.LoadStoreInfo().OffSet;
                        errorMessage.StatusCode = HttpStatusCode.NotFound;
                        errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)476, arrMsg, MessageType.OkOnly);
                        return false;
                    }
                }
                return true;
            }
            else
            {
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = "Invalid Request",
                    MessageType = 0
                };
                return false;
            }
        }


        #region Private methods


        private bool CancelHoldingDeletingPrepayFromFC(short pumpId, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!TCPAgent.Instance.IsConnected)
            {

                error.MessageStyle = _resourceManager.CreateMessage(offSet, 38, 20, null, MessageType.OkOnly);
                return false;
            }

            var timeIn = (float)DateAndTime.Timer;

            var response = "";
            var strRemain = "";
            string tempCommandRenamed = "CHR" + Strings.Right("0" + Convert.ToString(pumpId), 2);
            TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);
            while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
            {
                var strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting CHR" + Strings.Right("0" + System.Convert.ToString(pumpId), 2));
                if (!string.IsNullOrEmpty(strBuffer))
                {
                    modStringPad.SplitResponse(strBuffer, "CHR" + Strings.Right("0" + System.Convert.ToString(pumpId), 2), ref response, ref strRemain); //strBuffer<>""
                    if (!string.IsNullOrEmpty(response)) //got what we are waiting
                    {
                        TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                        WriteToLogFile("modify TCPAgent.PortReading from Cancel Holding Deleting Prepay: " + strRemain);
                        break;
                    }
                }
                Variables.Sleep(100);
                //System.Windows.Forms.Application.DoEvents();
                if (DateAndTime.Timer < timeIn)
                {
                    timeIn = (float)DateAndTime.Timer;
                }
            }

            if (Strings.Left(response, 7) != "CHR" + Strings.Right("0" + System.Convert.ToString(pumpId), 2) + "OK")
            {

                error.MessageStyle = _resourceManager.CreateMessage(offSet, 38, 20, null, MessageType.OkOnly);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Method to get selected tender using tender code
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="tenderCode">Tender code</param>
        /// <returns>Tender</returns>
        private Tender GetSelectedTender(Tenders tenders, string tenderCode)
        {
            var td = tenders.FirstOrDefault(t => t.Tender_Code == tenderCode);
            if (td == null)
            {
                td = tenders.FirstOrDefault(t => t.Tender_Class == tenderCode && (t.Tender_Class == "CRCARD" || t.Tender_Class == "FLEET"));
                if (_policyManager.COMBINECR || _policyManager.COMBINEFLEET)
                    return td;

                #region Added on 28 April 2017 - Ipsit - Not part of the converted code
                if (tenderCode == "DBCARD")
                { //' a debit card was detected and the class is returned instead of tender name

                    td = tenders.FirstOrDefault(x => x.Tender_Class == "DBCARD" && x.Amount_Entered == 0);
                    if (td != null)
                    {
                        return td;
                    }
                }
                #endregion

                return null;
            }
            return td;

        }

        /// <summary>
        /// Method to save payment in data base
        /// </summary>
        /// <param name="payment">Fleet payment</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="Tenders">Tenders</param>
        /// <param name="SC">Store credit</param>
        private void SavePayment(Payment payment, int saleNumber, byte tillNumber, string userCode,
            ref Tenders Tenders, ref Store_Credit SC)
        {
            Sale Sale = new Sale();

            //  int Pay_Num = 0;
            // Pay_Num = _tenderService.GetMaxROAPayment();


            Sale.Sale_Num = saleNumber;
            Sale.TillNumber = tillNumber;

            Sale.Sale_Totals.Net = 0;
            Sale.Sale_Totals.Gross = 0;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            Sale.Sale_Totals.Payment = Convert.ToDecimal(payment.Amount);
            Sale.Sale_Type = _resourceManager.GetResString(offSet, (short)160).ToUpper(); // "PAYMENT"
            if (SC == null)
            {
                _saleManager.SaveSale(Sale, userCode, ref Tenders, null);
            }
            else
            {
                _saleManager.SaveSale(Sale, userCode, ref Tenders, SC);
            }

            Sale = null;
            var user = _loginManager.GetUser(userCode);
            if (user.User_Group.Code == Entities.Constants.Trainer) //Behrooz Jan-12-06
            {
                return;
            }

            if (payment.Card != null)
            {
                payment.Account = "";


                //  - Encrypt Card numbers based on Policy + card Setup for Encryption. But for debit and credit cards only look the policy- not the card settings( always consider card setting as true)-(polcy checking is inside the Encryptdecrypt)
                //        rs![Card_Num] = EncryptDecrypt.Encrypt(Payment.Card.CardNumber)

                if (payment.Card.Crd_Type == "C" || payment.Card.Crd_Type == "D")
                {
                    payment.Card.Cardnumber = _encryptDecryptUtilityManager.Encrypt(payment.Card.Cardnumber, payment.Card.Crd_Type);
                    // payment.Card.Cardnumber = payment.Card.Cardnumber;
                }
                else //Not credit or debit card
                {
                    if (payment.Card.EncryptCard) // if card setup is to encrypt, send to encrypt- based on policy encrypt it or not
                    {
                        payment.Card.Cardnumber = _encryptDecryptUtilityManager.Encrypt(payment.Card.Cardnumber, payment.Card.Crd_Type);
                    }
                    else // if card setup is not to encrypt, no need to check the policy. keep it as it is
                    {

                        payment.Card.Cardnumber = payment.Card.Cardnumber;
                    }
                }
                // 



            }
            _tenderService.AddRoaPayment(payment, saleNumber);
            CacheManager.DeleteFleetPayment(saleNumber);
            CacheManager.DeleteTendersForPayment(saleNumber);
        }

        /// <summary>
        /// Method to check card restriction validation
        /// </summary>
        /// <param name="selectedTender">Selected tender</param>
        /// <param name="allTenders">Tenders</param>
        /// <param name="sale">Sale</param>
        /// <param name="creditCard">Credit card</param>
        /// <param name="promptMessages">prompt messages</param>
        /// <param name="amountEntered">Amount entered</param>
        /// <param name="Gross_Total">Gross total</param>
        /// <param name="errorMessage">Error</param>
        private void CardRestrictionValidation(Tender selectedTender, Tenders allTenders, Sale sale,
            Credit_Card creditCard, ref List<string> promptMessages, ref string amountEntered, decimal Gross_Total, bool makePayment,
            bool verifyAccount, out ErrorMessage errorMessage)
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            object[] messages = new object[3];
            short ans;
            errorMessage = new ErrorMessage();
            //  - Cardprofilevalidation checking here
            // adding the private card profile validation and related steps
            if (_policyManager.RSTR_PROFILE)
            {
                if (creditCard.Crd_Type == "F" && creditCard.GiftType.ToUpper() != "W")
                {
                    if (!string.IsNullOrEmpty(creditCard.CardProfileID))
                    {
                        var Prfl = new CardProfile();
                        if (CardProfileValidation(ref Prfl, allTenders, sale, ref promptMessages, System.Convert.ToString(creditCard.CardProfileID), System.Convert.ToString(creditCard.Cardnumber)))
                        {
                            //Scott's comment: Can there be a prompt if the customer buys some snacks and fuel,
                            //however only fuel is authorized for purchase.
                            // if the customer does not want those products, the sale would have to be completed, then those items would have to be returned.
                            //I see cashiers forgetting to return these items or not notice and tender the rest to cash to finish the sale.
                            if (Prfl.RestrictedUse)
                            {
                                messages[1] = "\'" + Prfl.ProfileID + "\'";
                                //TODO: Move to UI
                                MessageType temp_VbStyle7 = MessageType.YesNo;
                                if (!makePayment)
                                {
                                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)482, messages, temp_VbStyle7);
                                }
                                //ans = (short)1;
                                // S(sale, false, System.Convert.ToString(selectedTender.Credit_Card.CardProfileID)); //Erase the profileid

                                if (string.IsNullOrEmpty(amountEntered))
                                {
                                    amountEntered = Prfl.PurchaseAmount.ToString();
                                }

                                //lblUsed[Active_Line].Text = "";
                                if (!makePayment && !verifyAccount)
                                {
                                    selectedTender.Credit_Card.Crd_Type = "";
                                    selectedTender.Credit_Card.CardProfileID = "";
                                    _creditCardManager.SetCardnumber(ref creditCard, "");
                                    selectedTender.Credit_Card.Expiry_Date = "";
                                }
                                //if (txtAmount[Active_Line].Enabled)
                                //{
                                //    txtAmount[Active_Line].Focus(); //   to fix the crash because the txt was not enabled
                                //}
                                //cmdCancelMag_Click(cmdCancelMag, null);

                                //return;
                            }
                            if (Prfl.PartialUse)
                            {
                                if (Gross_Total - allTenders.Tend_Totals.Tend_Used > Convert.ToDecimal(Prfl.PurchaseAmount)) //  - if there is partial use but the remaining is paid by another tender no need to ask the question
                                {
                                    messages[1] = "\'" + Prfl.ProfileID + "\'";
                                    messages[2] = Prfl.Reason;
                                    MessageType temp_VbStyle8 = MessageType.YesNo;
                                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)477, messages, temp_VbStyle8);
                                    ans = (short)1;
                                    if (ans == (int)MsgBoxResult.No)
                                    {
                                        SaveSaleProfileID(sale, false, System.Convert.ToString(selectedTender.Credit_Card.CardProfileID)); //Erase the profileid
                                        if (string.IsNullOrEmpty(amountEntered))
                                        {
                                            amountEntered = Prfl.PurchaseAmount.ToString();
                                        }

                                        if (!makePayment && !verifyAccount)
                                        {
                                            selectedTender.Amount_Used = 0;
                                            selectedTender.Credit_Card.Crd_Type = "";
                                            selectedTender.Credit_Card.CardProfileID = "";
                                            _creditCardManager.SetCardnumber(ref creditCard, "");
                                            selectedTender.Credit_Card.Expiry_Date = "";
                                        }
                                        //if (txtAmount[Active_Line].Enabled)
                                        //{
                                        //    txtAmount[Active_Line].Focus(); //   to fix the crash because the txt was not enabled
                                        //}
                                        // cmdCancelMag_Click(cmdCancelMag, null);
                                        //return;
                                    }
                                    // return;
                                }
                            }

                            if (ValidCardProfilePrompts(ref Prfl, sale, ref promptMessages, Convert.ToString(selectedTender.Credit_Card.Cardnumber)))
                            {
                                selectedTender.Credit_Card.PONumber = Prfl.PONumber;
                                SaveSaleProfileID(sale, true, System.Convert.ToString(selectedTender.Credit_Card.CardProfileID));
                                //  not chargable 3.99 should be taken from the cash tender, then use the remaining cash tender for the chargeable product; rest should be charged from the gasking card
                                if (!string.IsNullOrEmpty(amountEntered))
                                {
                                    var amount = Math.Min(CommonUtility.GetDecimalValue(Prfl.PurchaseAmount), CommonUtility.GetDecimalValue(amountEntered));
                                    amountEntered = (Convert.ToDecimal(Prfl.PurchaseAmount) < Gross_Total - allTenders.Tend_Totals.Tend_Used ? amount : Gross_Total - allTenders.Tend_Totals.Tend_Used).ToString(); //  '= Prfl.PurchaseAmount
                                }
                                else
                                {
                                    amountEntered = (Convert.ToDecimal(Prfl.PurchaseAmount) < Gross_Total - allTenders.Tend_Totals.Tend_Used ? Convert.ToDecimal(Prfl.PurchaseAmount) : Gross_Total - allTenders.Tend_Totals.Tend_Used).ToString(); //  '= Prfl.PurchaseAmount
                                }
                                //  - to fix the zero amount in cardallTenders when using customer card at customer screen
                                selectedTender.Credit_Card.Trans_Amount = (float)Conversion.Val(amountEntered);
                                // 
                            }
                            else
                            {
                                SaveSaleProfileID(sale, false, System.Convert.ToString(selectedTender.Credit_Card.CardProfileID)); //Erase the profileid
                                if (string.IsNullOrEmpty(amountEntered))
                                {
                                    amountEntered = Prfl.PurchaseAmount.ToString();
                                }

                                selectedTender.Amount_Used = 0;
                                // "Purchase not allowed using this card.", vbInformation + vbOKOnly, "allTenders"
                                // 
                                //                            DisplayMessage Me, 42, vbInformation + vbOKOnly, " - " & Prfl.Reason
                                messages[1] = "\'" + creditCard.CardProfileID + "\'";
                                messages[2] = Prfl.Reason;
                                MessageType temp_VbStyle9 = (int)MessageType.Information + MessageType.OkOnly;
                                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)483, messages, temp_VbStyle9);
                                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                                //shiny end
                                if (!makePayment && !verifyAccount)
                                {
                                    selectedTender.Credit_Card.Crd_Type = "";
                                    selectedTender.Credit_Card.CardProfileID = "";
                                    _creditCardManager.SetCardnumber(ref creditCard, "");
                                    selectedTender.Credit_Card.Expiry_Date = "";
                                }
                                //if (txtAmount[Active_Line].Enabled)
                                //{
                                //    txtAmount[Active_Line].Focus(); //   to fix the crash because the txt was not enabled
                                //}
                                //cmdCancelMag_Click(cmdCancelMag, null);
                                //return;
                            }
                        }
                        else
                        {
                            if (Prfl.PurchaseAmount == 0)
                            {
                                SaveSaleProfileID(sale, false, System.Convert.ToString(selectedTender.Credit_Card.CardProfileID)); //erase
                                amountEntered = "";
                                selectedTender.Amount_Used = 0;
                                // "Purchase not allowed using this card.", vbInformation + vbOKOnly, "allTenders"
                                // 
                                //                            DisplayMessage Me, 42, vbInformation + vbOKOnly, " - " & Prfl.Reason
                                messages[1] = "\'" + Prfl.ProfileID + "\'";
                                messages[2] = Prfl.Reason;
                                MessageType temp_VbStyle10 = (int)MessageType.Information + MessageType.OkOnly;
                                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)483, messages, temp_VbStyle10);
                                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                                //shiny end
                                if (!makePayment && !verifyAccount)
                                {
                                    selectedTender.Credit_Card.Crd_Type = "";
                                    selectedTender.Credit_Card.CardProfileID = "";
                                    _creditCardManager.SetCardnumber(ref creditCard, "");
                                    selectedTender.Credit_Card.Expiry_Date = "";
                                }
                            }
                        }
                        if (Prfl.AskForPO)
                        {
                            if (!makePayment)
                            {
                                creditCard.PONumber = _resourceManager.GetResString(offSet, (short)470);
                            }
                            else
                            {
                                var ponumber = CacheManager.GetPoNumber(sale.Sale_Num, sale.TillNumber);
                                if (ponumber != null)
                                {
                                    creditCard.PONumber = ponumber;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method to create sale summary for fleet payment
        /// </summary>
        /// <param name="payment"></param>
        private void Top_Box(ref Payment payment)
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            payment.TopLeft = _resourceManager.GetResString(offSet, (short)160) + " : "; //Payment
            payment.TopRight = payment.Amount.ToString("###,##0.00");
        }

        /// <summary>
        /// Method to validate card entry
        /// </summary>
        /// <param name="cardNumber">card number</param>
        /// <param name="isSwiped">card swiped</param>
        /// <param name="error">Error</param>
        /// <returns>Credit card</returns>
        private Credit_Card Validate_Entry(string cardNumber, bool isSwiped, out ErrorMessage error)
        {
            var card = new Credit_Card();
            error = new ErrorMessage();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (string.IsNullOrEmpty(cardNumber))
            {
                MessageType temp_VbStyle5 = (int)MessageType.Critical + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)77,
                   _resourceManager.GetResString(offSet, 8145), temp_VbStyle5);
                return null;
            }
            try
            {
                byte[] data = Convert.FromBase64String(cardNumber);
                string decodedString = Encoding.UTF8.GetString(data);
                cardNumber = decodedString;
            }
            catch
            {
                MessageType temp_VbStyle = (int)MessageType.Critical + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)77, null, temp_VbStyle);
                return null;
            }
            string temp_UserInputString = cardNumber;
            string temp_AvoidedValuesString = "";
            if (!isSwiped)
            {
                cardNumber = Convert.ToString(Helper.SqlQueryCheck(ref temp_UserInputString, ref temp_AvoidedValuesString));
                _creditCardManager.SetCardnumber(ref card, cardNumber);
                card.Expiry_Date = DateAndTime.Month(DateTime.Now) + System.Convert.ToString(DateAndTime.Year(DateTime.Now));
            }
            else
            {
                card.Card_Swiped = true;

                short StartPos = 0;
                StartPos = (short)(cardNumber.IndexOf(";") + 1);
                if (StartPos == 0 || Strings.Right(cardNumber.Trim(), 1) != "?")
                {
                    StartPos = (short)(cardNumber.IndexOf(";") + 1);
                    if (StartPos == 0 || Strings.Right(cardNumber.Trim(), 1) != "?")
                    {
                        error.MessageStyle.Message = _resourceManager.CreateCaption(offSet, Convert.ToInt16(5), Convert.ToInt16(5), null, (short)2); // "Please Swipe Again "
                        error.StatusCode = HttpStatusCode.NotFound;

                        //if we can't get the proper data after the TimeOut,we have to Enabel the timer to get the data again
                        return null;
                    }
                    else
                    {
                        card = new Credit_Card();
                        if (StartPos > 1)
                        {
                            _creditCardManager.SetSwipeString(ref card, Strings.Right(cardNumber, cardNumber.Length - (StartPos - 1)));
                        }
                        else
                        {
                            _creditCardManager.SetSwipeString(ref card, cardNumber);
                        }
                    }
                    //Nancy end
                }
                else
                {
                    card = new Credit_Card();
                    if (StartPos > 1)
                    {
                        _creditCardManager.SetSwipeString(ref card, Strings.Right(cardNumber, cardNumber.Length - (StartPos - 1)));
                    }
                    else
                    {
                        _creditCardManager.SetSwipeString(ref card, cardNumber);
                    }

                }
            }

            if (_creditCardManager.CardIsValid(ref card))
            {
                if (!card.AllowPayment)
                {
                    // Payment is not allowed on " & Card.Name
                    MessageType temp_VbStyle = (int)MessageType.Critical + MessageType.OkOnly;
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 22, (short)94, card.Name, temp_VbStyle);
                    error.StatusCode = HttpStatusCode.NotFound;
                    return null;
                }
            }
            else
            {
                // Invalid Card - " & Card.Invalid_Reason
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 22, (short)92, _creditCardManager.Invalid_Reason(ref card), MessageType.OkOnly);
                error.StatusCode = HttpStatusCode.NotFound;
                return null;
            }
            return card;
        }

        /// <summary>
        /// Method to get summary label
        /// </summary>
        /// <param name="arPay">AR payment</param>
        private void Top_Box(ref AR_Payment arPay)
        {

            //    mvarTopLeft.Caption = "AR Payment : " & vbCrLf
            arPay.TopLeft = "AR Payment :";
            //Chaps_Main.GetResString((short)345) + " : " + "\r\n";
            arPay.TopRight = arPay.Amount.ToString("###,##0.00") + "\r\n";
            arPay.SummaryLabel = arPay.TopLeft + arPay.TopRight;
        }

        /// <summary>
        /// Method to get ar payment model
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="error">Error</param>
        /// <returns>AR payment</returns>
        private AR_Payment GetARPayer(int saleNumber, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var ar = CacheManager.GetArPayment(saleNumber);

            if (ar == null)
            {
                error.MessageStyle = new MessageStyle { Message = "Request is invalid" };
                return null;
            }
            return ar;
        }

        /// <summary>
        /// Method to get fleet payment model
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="error">Error</param>
        /// <returns>Fleet payment</returns>
        private Payment GetFleetPayer(int saleNumber, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var payment = CacheManager.GetFleetPayment(saleNumber);
            if (payment == null)
            {
                error.MessageStyle = new MessageStyle { Message = "Request is invalid" };
                return null;
            }
            return payment;
        }

        /// <summary>
        /// Method to check if enable run away or not
        /// </summary>
        /// <param name="user"></param>
        /// <param name="sale"></param>
        /// <returns></returns>
        private bool EnableRunaway(User user, Sale sale)
        {
            if (user.User_Group.Code == Utilities.Constants.Trainer) //Behrooz Jan-12-06
            {
                return false;
            }
            if (sale != null && sale.Sale_Totals.Gross > 0) //  - Enable runaway and pump button only if amount > 0
            {

                return (sale.Sale_Lines.Count != 0)
                    && CountUnPrepayFuelLines(sale) == sale.Sale_Lines.Count
                    && _policyManager.GetPol("U_RUNAWAY", user);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Method to check if enable run away or not
        /// </summary>
        /// <param name="user"></param>
        /// <param name="sale"></param>
        /// <returns></returns>
        private bool EnablePumpTest(User user, Sale sale)
        {
            if (sale != null && sale.Sale_Totals.Gross > 0) //  - Enable runaway and pump button only if amount > 0
            {

                return (sale.Sale_Lines.Count != 0)
                    && CountUnPrepayFuelLines(sale) == sale.Sale_Lines.Count
                    && _policyManager.GetPol("U_PUMPTEST", user);
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Method to count un- prepay fuel lines
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <returns>Lines</returns>
        private byte CountUnPrepayFuelLines(Sale sale)
        {
            byte returnValue = 0;

            Sale_Line SL_Count = default(Sale_Line);

            foreach (Sale_Line tempLoopVar_SL_Count in sale.Sale_Lines)
            {
                SL_Count = tempLoopVar_SL_Count;
                if ((SL_Count.ProductIsFuel && (!SL_Count.IsPropane)) && (!SL_Count.Prepay))
                {
                    returnValue++;
                }
            }


            SL_Count = null;

            return returnValue;
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

                        var msg = Encoding.ASCII.GetBytes(strRequest + System.Convert.ToString(sendStringSuf));
                        socket.Send(msg);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    retry++;
                    Thread.Sleep(200);
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
            short i1 = 0;
            switch (cc.Response.ToUpper())
            {
                case "APPROVED":
                    GetSelectedTender(tenders, tenderCode).Credit_Card = cc;
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
                        GetSelectedTender(tenders, tenderCode).Credit_Card = cc;
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
                        else if (_policyManager.Support_SAF && cc.Trans_Type.ToUpper() == "SALEINSIDE")
                        {
                            if (_policyManager.CallBankOnly) // only allow callthebank
                            {
                                /*  //                                Ans = DisplayMsgForm("You have to call the bank for approval No.", 11)
                                  ans = Chaps_Main.DisplayMsgForm(Chaps_Main.GetResString((short)1443), (short)11, null, (byte)0, (byte)0, "", "", "", "");

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
                            GetSelectedTender(tenders, tenderCode).Credit_Card = cc;
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
                    //    CustomerName = VB.Strings.Right(cc.Track2, cc.Track2.Length - i1);
                    //}
                    break;





                case "NOTCOMPLETED":

                    //VB.MsgBoxStyle temp_VbStyle = (int)VB.MsgBoxStyle.Information + VB.MsgBoxStyle.OkOnly;
                    //Chaps_Main.DisplayMessage(this, (short)49, temp_VbStyle, null, (byte)0);
                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 14, (short)49, null);
                    //  EMVVERSION-LAST - Screen freezing issue at POS, which luba reported- reason is crd_type is empty and we are not handling that situation
                    if (_policyManager.EMVVersion)
                    {
                        if (cc.Crd_Type == "")
                        {
                            //lblResponse.Text = Chaps_Main.Resource.DisplayCaption(System.Convert.ToInt16(lblResponse.Tag), System.Convert.ToInt16(this.Tag), null, (short)4); //"Time out"

                        }
                        else if (cc.Crd_Type == "D" || cc.Crd_Type == "C")
                        {
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
                GetSelectedTender(tenders, tenderCode).Credit_Card = cc;
                //Shiny Nov9, 2009 -EMVVERSION
                if (Strings.UCase(Convert.ToString(_policyManager.BankSystem)) != "TD")
                {
                    if (_policyManager.EMVVersion)
                    {
                        if (!string.IsNullOrEmpty(cc.Report))
                        {

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
                if (!string.IsNullOrEmpty(cc.BankMessage))
                {
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(offSet, cc.BankMessage, 99, null);
                }
                else
                {
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(offSet, cc.Response, 99, null);
                }
            }
            else
            {
                if (cc.Report.Length > 0)
                {
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(offSet, cc.Report, 99, null);
                }
                else
                {
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(offSet, cc.Response, 99, null);
                }
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
            cc.Response = GetStrPosition(strResponse, 15).Trim().ToUpper();
            WriteToLogFile("GetResponse procedure response is " + cc.Response);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (_policyManager.EMVVersion) //EMVVERSION 'Added May4,2010
            {
                if (cc.ManualCardProcess == false)
                {
                    cc.Card_Swiped = true;
                }
                else
                {
                    cc.Card_Swiped = false;
                }
            }
            cc.Result = GetStrPosition(strResponse, (short)16).Trim();
            cc.Authorization_Number = GetStrPosition(strResponse, (short)17).Trim().ToUpper();
            cc.ResponseCode = GetStrPosition(strResponse, (short)29).Trim().ToUpper();
            //  EMVVERSION
            if (_policyManager.EMVVersion) //EMVVERSION
            {
                cc.Crd_Type = GetStrPosition(strResponse, (short)2).Trim().Substring(0, 1);
                // _creditCardManager.SetTrack2(ref cc, GetStrPosition(strResponse, (short)12).Trim().ToUpper());
                // cc.Swipe_String = cc.Track2;
            }
            //shiny end-EMVVERSION



            strSeq = GetStrPosition(strResponse, (short)5).Trim();
            if (_policyManager.BankSystem != "Moneris")
            {
                if (string.IsNullOrEmpty(strSeq))
                {
                    cc.Sequence_Number = "";
                }
                else
                {
                    cc.Sequence_Number = strSeq.Substring(0, strSeq.Length - 1);
                }
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
                if (DateTime.TryParseExact(strDate, "MM/dd/yyyy", CultureInfo.InvariantCulture,
                       DateTimeStyles.None, out date))
                    cc.Trans_Date = date;
                else
                    cc.Trans_Date = DateTime.Today;

            }
            strTime = GetStrPosition(strResponse, (short)22).Trim();

            if (string.IsNullOrEmpty(strTime))
            {
                cc.Trans_Time = DateTime.Now;

            }
            else
            {
                DateTime time;
                if (DateTime.TryParseExact(strTime, "hh:mm:ss", CultureInfo.InvariantCulture,
                   DateTimeStyles.None, out time))
                {
                    cc.Trans_Time = new DateTime(1899, 12, 30, time.Hour, time.Minute, time.Second);
                }
                else
                    cc.Trans_Time = DateTime.Now;


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
        /// Method to get original payment tender
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="cardNumber">Card number</param>
        /// <param name="tenderClass">Tender class</param>
        /// <returns>Tender</returns>
        private string OriginalPayment_Tender(Sale sale, ref string cardNumber, ref string
            tenderClass)
        {
            string returnValue = "";
            returnValue = "";

            if (!sale.ForCorrection)
            {
                return returnValue;
            }
            if (sale.Void_Num == 0)
            {
                return returnValue;
            }



            var saleTend = _saleService.GetRefundSaleTender(sale.Void_Num, DataSource.CSCTills);

            if (saleTend == null)
            {
                saleTend = _saleService.GetRefundSaleTender(sale.Void_Num, DataSource.CSCTrans);
                if (saleTend == null)
                {
                    return returnValue;
                }
            }



            if (Strings.Trim(saleTend.TenderClass).ToUpper() != "CRCARD" && Strings.Trim(saleTend.TenderClass).ToUpper() != "DBCARD")
            {
                return returnValue;
            }

            cardNumber = !string.IsNullOrEmpty(saleTend.CCardNumber) ? _encryptDecryptUtilityManager.Decrypt(saleTend.CCardNumber) : string.Empty;
            tenderClass = Strings.Trim(saleTend.TenderClass);
            returnValue = saleTend.TenderName;

            return returnValue;
        }




        /// <summary>
        /// Method to get orginal card information
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="CardName">Card name</param>
        /// <param name="CardType">Card type</param>
        /// <param name="CardLanguage">Card language</param>
        /// <param name="CardSwiped">Card swiped</param>
        /// <param name="expiryDate">Expiry date</param>
        /// <returns>True or false</returns>
        private bool GetOriginalCardTend(Sale sale, ref string CardName, ref string CardType, ref
            string CardLanguage, ref bool CardSwiped, ref object expiryDate)
        {
            bool returnValue = false;
            if (!sale.ForCorrection)
            {
                return returnValue;
            }
            if (sale.Void_Num == 0)
            {
                return returnValue;
            }


            var cardTender = _saleService.GetCardTender(sale.Void_Num, DataSource.CSCTills);
            if (cardTender == null)
            {
                cardTender = _saleService.GetCardTender(sale.Void_Num, DataSource.CSCTrans);
                if (cardTender == null)
                {
                    return returnValue;
                }
            }

            CardType = cardTender.CardType;
            CardName = cardTender.CardName;
            CardLanguage = cardTender.Language;
            CardSwiped = cardTender.Swiped;

            if (!string.IsNullOrEmpty(CardType) && !string.IsNullOrEmpty(CardName))
            {
                returnValue = true;
            }
            return returnValue;
        }

        //shiny adding for private card profile implementation - april,2010
        /// <summary>
        /// Method to get card profile validations
        /// </summary>
        /// <param name="prfl">Profile</param>
        /// <param name="allTenders">Tenders</param>
        /// <param name="sale">Sale</param>
        /// <param name="promptMessages">Prompt messages</param>
        /// <param name="profileId">Profile Id</param>
        /// <param name="cardNumber">Card number</param>
        /// <returns>True or false</returns>
        private bool CardProfileValidation(ref CardProfile prfl, Tenders allTenders,
            Sale sale, ref List<string> promptMessages, string profileId, string cardNumber)
        {
            bool returnValue = false;

            returnValue = true;
            prfl = _cardManager.Loadprofile(profileId);
            if (prfl == null)
                return true;
            prfl.ProfileID = profileId;

            //Checking this profile is already used with another card for this transaction if yes , don't allow to use it again
            for (int i = 0; i <= allTenders.Count - 1; i++)
            {
                if (Strings.UCase(Convert.ToString(allTenders[i + 1].Credit_Card.CardProfileID)) == profileId.ToUpper() && allTenders[i + 1].Amount_Used != 0)
                {
                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    prfl.Reason = _resourceManager.GetResString(offSet, (short)1439); // " This card profile is already used."
                    returnValue = false;
                    break;
                }
            }
            //Asking for PO if need and and cancel the trans if po required and didn't give any
            short ans;
            if (prfl.AskForPO)
            {
                // promptMessages.Add(_resourceManager.GetResString(offSet,(short)470));
                //Variables.STFDNumber = "";
                //do
                //{
                //    Prfl.Reason = _resourceManager.GetResString(offSet,(short)470); //Enter PO Number
                //    ans = 1;
                //    if (ans == (int)MsgBoxResult.Ok)
                //    {
                //        temp = Strings.Left(Variables.STFDNumber.Trim(), 10);
                //        if (!string.IsNullOrEmpty(temp))
                //        {
                //            if (Strings.Len(temp) == 0) // if zero no restriction
                //            {
                //                temp = "";
                //            }
                //            else
                //            {
                //                Prfl.PONumber = Variables.STFDNumber;
                //                Prfl.Reason = "";
                //                Variables.STFDNumber = ""; //Reset STFDNumber
                //                break;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        //Cancel
                //        Prfl.PONumber = "";
                //    }
                //} while (string.IsNullOrEmpty(temp) && ans == 1);
                //if (string.IsNullOrEmpty(Prfl.PONumber))
                //{
                //    Prfl.Reason = _resourceManager.GetResString(offSet,(short)471); //"Missing Required PO Number"
                //    returnValue = false; // failed to getPO Number - cancel card transaction
                //}

            }

            if (returnValue)
            {
                if (prfl.LimitTimeofPurchase)
                {
                    if (!_cardManager.ValidProfileTimeLimit(ref prfl))
                    {
                        prfl.PurchaseAmount = 0;
                        returnValue = false;
                    }
                }
            }

            if (returnValue) //pass the timelimit, now need to check the product restriction
            {
                if (!_cardManager.ValidProductsForProfile(ref prfl, sale)) // to get the amount based on restrict\allow products setting
                {
                    prfl.PurchaseAmount = 0;
                    returnValue = false;
                }
            }

            if (returnValue) // pass the product restriction, now need to check the daily and monthly transactionlimit
            {
                if (!_cardManager.ValidTransactionLimits(ref prfl, cardNumber)) // to check it is satifying daily weekly and monthly limit and limit of 3 of trans per day
                {
                    prfl.PurchaseAmount = 0;
                    returnValue = false;
                }
            }
            return returnValue;
        }

        // This is the function to validate the prompts. If prompt is required minlength will be >0; if not getting prompt for minlength system will show the screen again, they can use cancel, it will cancel card trans
        /// <summary>
        /// Method to verify if valid card profile prompts
        /// </summary>
        /// <param name="Prfl">Card profile</param>
        /// <param name="sale"></param>
        /// <param name="promptMessages">Prompt messages</param>
        /// <param name="cardNumber">Card number</param>
        /// <returns>True or false</returns>
        private bool ValidCardProfilePrompts(ref CardProfile Prfl, Sale sale, ref List<string>
            promptMessages, string cardNumber)
        {
            bool returnValue = false;
            CardPrompt prm = default(CardPrompt);
            short i = 0;
            returnValue = true;
            if (Prfl.PromptForFuel == false)
            {
                return returnValue; //   - ask only if there is fuel products and allowed for this card
            }

            if (Prfl.CardPrompts.Count != 0)
            {
                for (i = 1; i <= Prfl.CardPrompts.Count; i++)
                {
                    prm = Prfl.CardPrompts[i];
                    promptMessages.Add(prm.PromptMessage);
                    //Variables.STFDNumber = "";
                    //do
                    //{
                    //    ans = Chaps_Main.DisplayMsgForm(prm.PromptMessage, (short)20, null, (byte)0, (byte)0, prm.PromptMessage, "", "", "");
                    //    if (ans == (int)MsgBoxResult.Ok)
                    //    {
                    //        temp = Strings.Left(Variables.STFDNumber.Trim(), prm.MaxLength);
                    //        if (!string.IsNullOrEmpty(temp))
                    //        {
                    //            if (prm.MinLength != 0 && temp.Length < prm.MinLength) // if zero no restriction
                    //            {
                    //                temp = "";
                    //            }
                    //            else
                    //            {
                    //                prm.PromptAnswer = temp;
                    //                Variables.STFDNumber = ""; //Reset STFDNumber
                    //                break;
                    //            }
                    //        }
                    //        else // didn't enter anything and pressed OK
                    //        {
                    //            if (prm.MinLength != 0 && temp.Length < prm.MinLength) // if zero no restriction
                    //            {
                    //                temp = "";
                    //            }
                    //            else
                    //            {
                    //                prm.PromptAnswer = "";
                    //                Variables.STFDNumber = ""; //Reset STFDNumber
                    //                break;
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        //Cancel
                    //        prm.PromptAnswer = "";
                    //    }
                    //} while (string.IsNullOrEmpty(temp) && ans == 1);
                    //if (prm.MinLength != 0 && string.IsNullOrEmpty(prm.PromptAnswer))
                    //{
                    //    
                    //    Prfl.Reason = _resourceManager.GetResString(offSet,(short)1438) + prm.PromptMessage;
                    //    returnValue = false; // failed to get a required prompt - cancel card transaction
                    //    break;
                    //}
                }
            }
            //if (returnValue && Prfl.CardPrompts.Count != 0)
            //{
            //    _saleManager.Save_ProfilePrompt_Temp(Prfl.CardPrompts, CardNumber, Prfl.ProfileID, sale.Sale_Num, sale.TillNumber);
            //    // sale.Save_ProfilePrompt_Temp(Prfl.CardPrompts, CardNumber, Prfl.ProfileID);
            //}
            return returnValue;
        }

        /// <summary>
        /// Method to save profile Id
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="save">Save</param>
        /// <param name="profileId">Profile Id</param>
        private void SaveSaleProfileID(Sale sale, bool save, string profileId) // this is to save sl.cardprofileid for crash recovery in saleline table
        {

            Sale_Line sl;
            foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
            {
                sl = tempLoopVarSl;
                if (save)
                {
                    if (!string.IsNullOrEmpty(sl.CardProfileID) && sl.CardProfileID == profileId)
                    {
                        sl.CardProfileID = profileId;
                        // _saleService.UpdateCardProfileId(sl.CardProfileID, sale.Sale_Num, sl.Line_Num);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(sl.CardProfileID) && sl.CardProfileID == profileId)
                    {
                        sl.CardProfileID = ""; // reset
                    }
                }
            }
            sl = null;
        }
        //shiny End private card profile implementation - april,2010

        /// <summary>
        /// Method to save AR payment
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenders">Tenders</param>
        /// <param name="SC">Store credit</param>
        private void SaveArPayment(int saleNumber, int tillNumber, string userCode, Tenders tenders, Store_Credit
            SC)
        {
            ErrorMessage error;
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out error);
            int payNum = _tenderService.GetMaxPaymentNumber();



            var arPayment = CacheManager.GetArPayment(saleNumber);
            if (arPayment == null)
            {
                return;
            }


            sale.Sale_Totals.Net = 0;
            sale.Sale_Totals.Gross = 0;
            sale.Sale_Totals.Payment = arPayment.Amount;
            sale.Sale_Type = "ARPAY";
            sale.Customer = arPayment.Customer;
            sale.Customer.Current_Balance = sale.Customer.Current_Balance - (double)arPayment.Amount;
            if (SC == null)
            {
                _saleManager.SaveSale(sale, userCode, ref tenders, null);
            }
            else
            {
                _saleManager.SaveSale(sale, userCode, ref tenders, SC);
            }
            _tenderService.SaveArPayment(arPayment, payNum);
            // Nicolette moved next lines into SaveSale method
            // (we go there anyway, so save all once time)
            //    Set rs = dbmaster.OpenRecordset( _



            //    rs.Edit
            //    If Not IsNull(rs![Cl_CurBal]) Then
            //         rs![Cl_CurBal] = rs![Cl_CurBal] - Me.Amount
            //    Else
            //         rs![Cl_CurBal] = 0 - Me.Amount
            //    End If
            //
            //    rs.Update
            //
        }

        /// <summary>
        /// Method to set summary 2 for tenders
        /// </summary>
        /// <param name="tenders"></param>
        /// <param name="grossTotal"></param>
        private void SetSummary2Informatiion(ref Tenders tenders, decimal grossTotal)
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (grossTotal > 0)
            {
                if (tenders.Tend_Totals.Tend_Used > 0) // 
                {
                    //SALE
                    if (Math.Round(Convert.ToDouble(tenders.Tend_Totals.Change), 2) < 0.0D)
                    {
                        tenders.Summary2 = _resourceManager.GetResString(offSet, (short)166) + " $" + Math.Abs(tenders.Tend_Totals.Change).ToString("###,##0.00"); //"Change Due $"
                        tenders.EnableCompletePayment = true;
                    }
                    else if (Math.Round(System.Convert.ToDouble(tenders.Tend_Totals.Change), 2) == 0.0D)
                    {
                        tenders.Summary2 = _resourceManager.GetResString(offSet, (short)165); // "Payment Complete"
                        tenders.EnableCompletePayment = true;
                        tenders.EnableCompleteReceipt = true;
                    }
                    else
                    {
                        tenders.Summary2 = _resourceManager.GetResString(offSet, (short)164) + tenders.Tend_Totals.Change.ToString("###,##0.00"); //"Outstanding $"
                        tenders.EnableCompletePayment = false;
                        tenders.EnableCompleteReceipt = false;
                    }
                    //shiny end
                }
                else
                {
                    tenders.Summary2 = _resourceManager.GetResString(offSet, (short)164) + tenders.Tend_Totals.Change.ToString("###,##0.00"); //"Outstanding $" ' 
                    tenders.EnableCompletePayment = false;
                    tenders.EnableCompleteReceipt = false;
                }
                tenders.DisplayNoReceiptButton = Convert.ToBoolean(!_policyManager.PRINT_REC);

            }
            else
            {
                if (tenders.Tend_Totals.Tend_Used < 0) // 
                {

                    if (Math.Round(Convert.ToDouble(tenders.Tend_Totals.Change), 2) < 0.0D)
                    {
                        // Can't refund more than he is owed.
                        tenders.Summary2 = _resourceManager.GetResString(offSet, (short)167) + System.Math.Abs(tenders.Tend_Totals.Change).ToString("###,##0.00"); //"Too Much Refunded $"
                        tenders.EnableCompletePayment = false;
                        tenders.EnableCompleteReceipt = false;

                    }
                    else if (Math.Round(Convert.ToDouble(tenders.Tend_Totals.Change), 2) == 0.0D)
                    {
                        // Refunded exactly the right amount.
                        tenders.Summary2 = _resourceManager.GetResString(offSet, (short)168); //"Refund Complete"
                        tenders.EnableCompletePayment = true;
                        tenders.EnableCompleteReceipt = true;
                    }
                    else
                    {
                        // STill_Renamed need to refund some more to complete.
                        tenders.Summary2 = _resourceManager.GetResString(offSet, (short)169) + tenders.Tend_Totals.Change.ToString("###,##0.00"); //"Too Be Refunded $"
                        tenders.EnableCompletePayment = false;
                        tenders.EnableCompleteReceipt = false;
                    }
                }
                tenders.DisplayNoReceiptButton = !_policyManager.PRINT_VOID;

            }

        }

        /// <summary>
        /// Method to check if invalid transaction type
        /// </summary>
        /// <param name="transactionType">Transaction type</param>
        /// <returns>True or false</returns>
        private bool IsValidTransactionType(string transactionType)
        {
            var transactionTypes = new List<string>
            {
                "Sale",
                "Payment",
                "ARPay",
                "Prepay",
                "Delete Prepay",
                "CloseCurrentTill",
                "CashDrop"
            };
            return transactionTypes.Contains(transactionType);
        }

        /// <summary>
        /// Method to clear tax exempt data
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        private void Clear_TaxExempt(int saleNumber, int tillNumber, string userCode, out ErrorMessage error)
        {
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out error);
            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return;
            }

            var oPurchaseList = CacheManager.GetPurchaseListSaleForTill(tillNumber, saleNumber);
            var oTeSale = CacheManager.GetTaxExemptSaleForTill(tillNumber, saleNumber);
            if (sale != null)
            {
                sale.TreatyNumber = "";
                sale.TreatyName = ""; // 

                short rowNumber = 0;
                if (_policyManager.TE_Type == "SITE")
                {
                    if (oPurchaseList?.Count() > 0)
                    {



                        short i;
                        for (i = 1; i <= oPurchaseList.Count(); i++)
                        {



                            if (!oPurchaseList.Item(i).WasTaxExemptReturn)
                            {
                                rowNumber = oPurchaseList.Item(i).GetRowInSalesMain();
                                var saleLine = sale.Sale_Lines[rowNumber];
                                saleLine.Vendor = sale.Sale_Lines[rowNumber].OrigVendor;
                                saleLine.IsTaxExemptItem = false;
                                //saleLine.price = oPurchaseList.Item(i).GetOriginalPrice();

                                _saleLineManager.SetPrice(ref saleLine, oPurchaseList.Item(i).GetOriginalPrice());
                                _saleLineManager.SetAmount(ref saleLine, (decimal)(oPurchaseList.Item(i).GetQuantity() * oPurchaseList.Item(i).GetOriginalPrice()));

                                saleLine.overrideCode = (short)0;
                                // Cancel and undo basket was giving discount using TE price and it was going down
                                if (saleLine.ProductIsFuel)
                                {
                                    //saleLine.Amount = (decimal)(oPurchaseList.Item(i).GetTaxIncludeAmount());
                                    _saleLineManager.SetAmount(ref saleLine, (decimal)(oPurchaseList.Item(i).GetTaxIncludeAmount()));
                                    if (saleLine.Prepay)
                                    {
                                        //saleLine.Quantity = (float)(sale.Sale_Lines[rowNumber].OrigQty);
                                        _saleLineManager.SetQuantity(ref saleLine, (float)(sale.Sale_Lines[rowNumber].OrigQty));
                                        _saleLineManager.SetAmount(ref saleLine, (decimal)(sale.Sale_Lines[rowNumber].OrigQty * sale.Sale_Lines[rowNumber].price));
                                        //saleLine.Amount = (decimal)(sale.Sale_Lines[rowNumber].OrigQty * sale.Sale_Lines[rowNumber].price);
                                    }
                                }
                                saleLine.Cost = sale.Sale_Lines[rowNumber].OrigCost; // 
                            }
                        }
                    }
                }
                else
                {
                    if (oTeSale != null)
                    {
                        if (oTeSale.Te_Sale_Lines.Count > 0)
                        {
                            foreach (TaxExemptSaleLine tempLoopVarTesl in oTeSale.Te_Sale_Lines)
                            {
                                var tesl = tempLoopVarTesl;


                                if (!tesl.WasTaxExemptReturn)
                                {
                                    rowNumber = tesl.Line_Num;
                                    var saleLine = sale.Sale_Lines[rowNumber];
                                    saleLine.IsTaxExemptItem = false;
                                    saleLine.Vendor = saleLine.OrigVendor;

                                    if (saleLine.Amount < 0)
                                    {
                                        saleLine.No_Loading = true; //  for refund of TE sale and using cancel tender  - when re-setting the amount we don't need to go through the discount_rate ( it is already set)- going through it agian screwing up the tender amount( crevir issue with discount)
                                    }

                                    //saleLine.price = tesl.OriginalPrice;
                                    _saleLineManager.SetPrice(ref saleLine, tesl.OriginalPrice);
                                    _saleLineManager.SetAmount(ref saleLine, (decimal)tesl.OriginalPrice);







                                    if (tesl.IsFuelItem)
                                    {
                                        _saleLineManager.SetPrice(ref saleLine, tesl.TaxInclPrice);
                                        _saleLineManager.SetAmount(ref saleLine, (decimal)tesl.TaxInclPrice);
                                    }
                                    saleLine.Total_Amount = 0;
                                    saleLine.overrideCode = (short)0;
                                    saleLine.Cost = saleLine.OrigCost; // 

                                }
                            }
                        }
                        else if (oTeSale.teCardholder.GstExempt)
                        {
                            foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                            {
                                var sl = tempLoopVarSl;
                                _saleLineManager.SetAmount(ref sl, sl.Net_Amount);
                                sl.Total_Amount = 0;
                                //  -to reverse the cost vendor to original
                                sl.Vendor = sl.OrigVendor;
                                sl.Cost = sl.OrigCost;
                            }
                        }
                    }
                }
                _saleManager.ApplyTaxes(ref sale, true);
                _saleManager.ReCompute_Totals(ref sale);
                CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);
            }
            CacheManager.RemovePurchaseListSaleForTill(tillNumber, saleNumber);
            CacheManager.RemoveTaxExemptSaleForTill(tillNumber, saleNumber);

        }


        private short Set_Tender(string CardType, decimal TenderAmount, string TenderName, double Exchange_Rate, bool EMV, decimal grosstotal, ref Sale sale)
        {
            short returnValue = 0;
            short i = 0;
            dynamic Tenders_Renamed = default(dynamic);

            Tenders Tend;
            Tender td = default(Tender);
            double Amount_Tendered;

            Tend = new Tenders();

            modStringPad.WriteToLogFile("Set_Tender function in SaleTend form, tender name is " + TenderName + " amount is " + System.Convert.ToString(TenderAmount) + " card type is " + CardType);
            if (TenderName.Length == 0 && CardType.Length == 0)
            {
                returnValue = (short)9999;
                return returnValue;
            }

            if (TenderName.Length == 0)
            {
                for (i = 1; i <= Tenders_Renamed.Count; i++)
                {
                    if (_tenderService.GetCardType(System.Convert.ToString(Tenders_Renamed.Item(i).Tender_Code)).ToUpper() == CardType)
                    {
                        td = new Tender();
                        td = Tenders_Renamed.Item(i);
                        break;
                    }
                }
            }

            if (CardType.Length == 0)
            {
                if (TenderName.ToUpper() == "DBCARD") // a debit card was detected and the class is returned instead of tender name
                {
                    for (i = 1; i <= Tenders_Renamed.Count; i++)
                    {
                        if (EMV)
                        {
                            Amount_Tendered = System.Convert.ToDouble(Tenders_Renamed.Item(i).Amount_Entered);
                        }
                        else
                        {
                            Amount_Tendered = 0;
                        }
                        if (Strings.UCase(System.Convert.ToString(Tenders_Renamed.Item(i).Tender_Class)) == TenderName && Tenders_Renamed.Item(i).Amount_Entered == 0)
                        {
                            td = new Tender();
                            td = Tenders_Renamed.Item(i);
                            break;
                        }
                    }
                }
                else
                {
                    for (i = 1; i <= Tenders_Renamed.Count; i++)
                    {
                        if (_policyManager.COMBINECR && Strings.UCase(System.Convert.ToString(Tenders_Renamed.Item(i).Tender_Class)) == "CRCARD" && Strings.UCase(System.Convert.ToString(Tenders_Renamed.Item(i).Tender_Class)) == TenderName && Tenders_Renamed.Item(i).Amount_Entered == 0)
                        {
                            td = new Tender();
                            td = Tenders_Renamed.Item(i);
                            break;
                        }

                        if (_policyManager.COMBINEFLEET && Strings.UCase(System.Convert.ToString(Tenders_Renamed.Item(i).Tender_Class)) == "FLEET" && Strings.UCase(System.Convert.ToString(Tenders_Renamed.Item(i).Tender_Class)) == TenderName && Tenders_Renamed.Item(i).Amount_Entered == 0)
                        {
                            td = new Tender();
                            td = Tenders_Renamed.Item(i);
                            break;
                        }




                        if (Strings.UCase(System.Convert.ToString(Tenders_Renamed.Item(i).Tender_Code)) == TenderName)
                        {
                            //Shiny sept14, 2009 - To fix the Kickback crash with small date time issue, when cashier swiping the 

                            //  card at tender screen aftersaying 'NO' to redeem points
                            if (Tenders_Renamed.Item(i).Tender_Class == "LOYALTY" && Chaps_Main.SA.Customer.PointCardNum != "" && Chaps_Main.SA.Customer.Points_Redeemed == 0)
                            {
                                break;
                            }
                            //shiny end - sept14, 2009
                            td = new Tender();
                            td = Tenders_Renamed.Item(i);
                            break;
                        }
                        else if (Strings.UCase(System.Convert.ToString(Tenders_Renamed.Item(i).Tender_Code)) == TenderName)
                        {
                            td = new Tender();
                            td = Tenders_Renamed.Item(i);
                            break;
                        }
                    }
                }
            }
            if (td == null)
            {
                returnValue = (short)9999;

                return returnValue; // Tender was not found
            }

            if (TenderAmount != 0)
            {
                if (Exchange_Rate == 0)
                {
                    if (TenderAmount * Convert.ToDecimal(td.Exchange_Rate) > grosstotal)
                    {
                        TenderAmount = (grosstotal / Convert.ToDecimal(td.Exchange_Rate));
                    }
                }
                else
                {
                    // Exchange rate parameter has been added for KickBack processing or any other tender that doesn't use the exchange rate from the database in the Tenders_Renamed defintion
                    if (TenderAmount * Convert.ToDecimal(Exchange_Rate) > grosstotal)
                    {
                        TenderAmount = (grosstotal / Convert.ToDecimal(Exchange_Rate));
                    }
                    td.Exchange_Rate = Exchange_Rate;
                }

                td.Credit_Card.Crd_Type = CardType;
                Set_Amount_Entered(ref Tend, ref sale, td, TenderAmount, -1);


            }
            else
            {
                if (CardType == "K")
                {
                    td.Exchange_Rate = Exchange_Rate;
                    Set_Amount_Entered(ref Tend, ref sale, td, TenderAmount, -1);
                }
            }

            returnValue = i;
            td = null;

            return returnValue;
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
        #endregion

    }

}

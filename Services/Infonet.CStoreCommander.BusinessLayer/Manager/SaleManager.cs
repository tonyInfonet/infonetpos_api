using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using SaleTend = Infonet.CStoreCommander.Entities.SaleTend;
using User = Infonet.CStoreCommander.Entities.User;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    /// <summary>
    /// Sale Manager 
    /// </summary>
    public class SaleManager : ManagerBase, ISaleManager
    {
        private readonly IPolicyManager _policyManager;
        private readonly ISaleService _saleService;
        private readonly IApiResourceManager _resourceManager;
        private readonly ILoginManager _loginManager;
        private readonly ILoginService _loginService;
        private readonly IStockService _stockService;
        private readonly IUtilityService _utilityService;
        private readonly ITillService _tillService;
        private readonly ICustomerService _customerService;
        private readonly ICardService _cardService;
        private readonly ITaxService _taxService;
        private readonly ISaleLineManager _saleLineManager;
        private readonly ISaleHeadManager _saleHeadManager;
        private readonly ICustomerManager _customerManager;
        private readonly IGivexClientManager _givexClientManager;
        private readonly IReasonService _reasonService;
        private readonly ICreditCardManager _creditCardManager;
        private readonly ITreatyManager _treatyManager;
        private readonly IEncryptDecryptUtilityManager _encryptManager;
        private readonly IMainManager _mainManager;
        private readonly IPrepayManager _prepayManager;
        private readonly ICarwashManager _carwashManager;
       // private readonly ICashBonusManager _cashBonusManager;


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="policyManager"></param>
        /// <param name="saleService"></param>
        /// <param name="resourceManager"></param>
        /// <param name="loginManager"></param>
        /// <param name="loginService"></param>
        /// <param name="stockService"></param>
        /// <param name="utilityService"></param>
        /// <param name="tillService"></param>
        /// <param name="customerService"></param>
        /// <param name="cardService"></param>
        /// <param name="taxService"></param>
        /// <param name="saleLineManager"></param>
        /// <param name="saleHeadManager"></param>
        /// <param name="customerManager"></param>
        /// <param name="reasonService"></param>
        /// <param name="givexClientManager"></param>
        /// <param name="creditCardManager"></param>
        /// <param name="treatyManager"></param>
        /// <param name="encryptManager"></param>
        /// <param name="mainManager"></param>
        /// <param name="carwashManager"></param>>
        public SaleManager(IPolicyManager policyManager,
            ISaleService saleService,
            IApiResourceManager resourceManager,
            ILoginManager loginManager,
            ILoginService loginService,
            IStockService stockService,
            IUtilityService utilityService,
            ITillService tillService,
            ICustomerService customerService,
            ICardService cardService,
            ITaxService taxService,
            ISaleLineManager saleLineManager,
            ISaleHeadManager saleHeadManager,
            ICustomerManager customerManager,
            IReasonService reasonService,
            IGivexClientManager givexClientManager,
            ICreditCardManager creditCardManager,
            ITreatyManager treatyManager,
            IEncryptDecryptUtilityManager encryptManager,
            IMainManager mainManager,
            IPrepayManager prepayManager,
            ICarwashManager carwashManager
            )
        {
            _policyManager = policyManager;
            _saleService = saleService;
            _resourceManager = resourceManager;
            _loginManager = loginManager;
            _loginService = loginService;
            _stockService = stockService;
            _utilityService = utilityService;
            _tillService = tillService;
            _customerService = customerService;
            _cardService = cardService;
            _taxService = taxService;
            _saleLineManager = saleLineManager;
            _saleHeadManager = saleHeadManager;
            _customerManager = customerManager;
            _reasonService = reasonService;
            _givexClientManager = givexClientManager;
            _creditCardManager = creditCardManager;
            _treatyManager = treatyManager;
            _encryptManager = encryptManager;
            _mainManager = mainManager;
            _prepayManager = prepayManager;
            _carwashManager = carwashManager;
        }

        /// <summary>
        /// Initilize a sale 
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="message">Error</param>
        /// <returns>Sale</returns>
        public Sale InitializeSale(int tillNumber, int registerNumber, string userCode,
            out ErrorMessage message)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,InitializeSale,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            //set register
            var register = new Register();
            _mainManager.SetRegister(ref register, (short)registerNumber);
            message = new ErrorMessage();
            var lcdMsg = new CustomerDisplay();
            if (register == null)
            {
                message.MessageStyle = new MessageStyle
                {
                    Message = Utilities.Constants.InvalidRegister
                };
                message.StatusCode = HttpStatusCode.NotFound;
                return null;
            }
            var sale = new Sale();

            var currentSale = _saleService.GetSaleByTillNumber(tillNumber);

            sale.TillNumber = CommonUtility.GetByteValue(tillNumber);
            sale.Register = (byte)registerNumber;
            if (currentSale == null)
            {
                sale.Sale_Num = Clear_Sale(null, 0, tillNumber, userCode, "Initial", null, true, true, false, out message);
                if (!string.IsNullOrEmpty(message.MessageStyle.Message))
                {
                    return null;
                }
                sale.Customer = _customerManager.LoadCustomer(string.Empty);
                sale.SaleHead = new SaleHead();
                sale.Payment = false;
                _saleHeadManager.SetSalePolicies(ref sale);
                SaveTemp(ref sale, sale.TillNumber);
            }
            else
            {
                Clear_Sale(currentSale, currentSale.Sale_Num, currentSale.TillNumber, userCode, "Initial", null, false, true, false, out message);

                if (!string.IsNullOrEmpty(message.MessageStyle.Message))
                {
                    return null;
                }
                //  - crash recovery
                sale = Load_Temp(currentSale.Sale_Num, tillNumber, userCode, out message);

                foreach (Sale_Line sl in sale.Sale_Lines)
                {
                    var sline = sl;
                    _saleLineManager.SetLevelPolicies(ref sline);
                }

                // moved here on June 10, 2003, outside form Load_Temp procedure
                sale.Payment = _saleService.CheckPaymentsFromDbTemp(currentSale.Sale_Num, tillNumber);
                sale.Register = (byte)registerNumber;
                SaveTemp(ref sale, sale.TillNumber);
            }
            //
            if (register.Customer_Display)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                sale.CustomerDisplay = _mainManager.DisplayMsgLcd(register, _mainManager.FormatLcdString(register,
                      _resourceManager.GetResString(offSet, (short)414) + "#:",
                      Conversion.Str(sale.Sale_Num).Trim()), "");

            }

            if (_policyManager.TAX_EXEMPT) // if tax exempt is true
            {
                if (_policyManager.TE_Type == "AITE") // if it's AITE  tax exempt
                {
                    var aiteMessage = GetAiteStoreMessage();
                }
            }

            //Update Sale object in Cache
            CacheManager.AddCurrentSaleForTill(tillNumber, sale.Sale_Num, sale);

            Performancelog.Debug(
                $"End,SaleManager,InitializeSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds}," +
                $"{DateTime.Now:hh.mm.ss.ffffff}");

            return sale;
        }

        /// <summary>
        /// Clear Sale 
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="saleType">Sale type</param>
        /// <param name="tenders">Tenders</param>
        /// <param name="newSale">New sale</param>
        /// <param name="setCtrl">Set control or not</param>
        /// <param name="useSameSaleNo">Same sale number or not</param>
        /// <param name="message">Error</param>
        /// <returns>Sale number</returns>
        public int Clear_Sale(Sale sale, int saleNumber, int tillNumber, string userCode, string saleType, Tenders tenders, bool newSale, bool setCtrl, bool useSameSaleNo, out ErrorMessage message)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Clear_Sale,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            message = new ErrorMessage();
            int saleNo = 0;
            int oldSaleNo = saleNumber;
            Tenders tdrs = null;
            if (tenders != null && tenders.Count != 0)
            {
                tdrs = tenders;
            }
            if (!(string.IsNullOrEmpty(saleType) || saleType == "Initial"))
            {
                if (saleType != "SUSPEND" && saleType != "PREPAY")
                {
                    sale.Sale_Type = saleType;
                    SaveSaveToDbTemp(sale, saleNumber, tillNumber, userCode, ref tdrs);
                }
            }

            if (newSale)
            {
                saleNo = GetSaleNo(tillNumber, userCode, out message);

            }

            //   to ensure that no valid customer status is kept for next sale
            // ValidTreatyNo property is used only if policy use real time validation is set to "Yes"
            if (_policyManager.SITE_RTVAL)
            {
                if (!(Chaps_Main.oTreatyNo == null))
                {
                    Chaps_Main.oTreatyNo.ValidTreatyNo = false;
                    Chaps_Main.oTreatyNo.SendOverride = false;
                    Chaps_Main.oTreatyNo.OverrideNumber = (0).ToString();
                    Chaps_Main.oTreatyNo.OverrideReason = (short)0;
                }
            }
            //  end


            if (useSameSaleNo)
            {
                saleNo = oldSaleNo;
            }


            // Set the cash sale customer, if the policy to use a specific customer is set, this is done in Customer class
            Performancelog.Debug($"End,SaleManager,Clear_Sale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            //Variables.KickBack = null; // SV Clear the previous KickBack instance
            return saleNo;
        }

        /// <summary>
        /// Get Sale number
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="message">Error</param>
        /// <returns>Sale number</returns>
        public int GetSaleNo(int tillNumber, string userCode, out ErrorMessage message)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,GetSaleNo,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            int returnValue = 0;
            message = new ErrorMessage();
            //var user = _loginManager.GetUser(userCode);
            var user = _loginManager.GetExistingUser(userCode);
            try
            {
                if (user.User_Group.Code == "Trainer") //Behrooz Jan-12-06
                {
                    returnValue = _saleService.GetMaxSaleNoFromSaleHeadFromDbTill(tillNumber);

                    int maxSusSale = _saleService.GetMaxSaleNoFromSusHeadFromDbTill(tillNumber);

                    if (maxSusSale >= returnValue)
                    {
                        returnValue = maxSusSale + 1;
                    }
                    return returnValue;
                }
                var messageNumber = 0;
                returnValue = _saleService.GetMaxSaleNoFromSaleNumbFromDbAdmin(tillNumber, out messageNumber);

                if (returnValue == 0)
                {
                    // Cannot get new invoice number, end the POS
                    //TODO: Udham Logger
                    //modStringPad.WriteToLogFile("Invoice number is 0. POS ended for till " + System.Convert.ToString(tillNumber));
                    //MsgBoxStyle temp_VbStyle3 = (int)MsgBoxStyle.OkOnly + MsgBoxStyle.Critical;
                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    message = new ErrorMessage
                    {
                        MessageStyle = new MessageStyle
                        {
                            Message = _resourceManager.GetResString(offSet, 8341),
                            MessageType = MessageType.OkOnly
                        },
                        ShutDownPos = true,
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    return returnValue;
                }
            }
            catch (Exception ex)
            {

                // In GetSaleNo we should not use Solution Error form and allow the user
                // to continue, but end the POS if there is any error in this function
                // Log the error in the POSLog.txt file
                WriteToLogFile("Error in GetSaleNo. POS ended for till " + System.Convert.ToString(tillNumber) + " error is " + System.Convert.ToString(ex.HResult) + " " + ex.Message);
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                message = new ErrorMessage
                {

                    MessageStyle = new MessageStyle
                    {
                        Message = _resourceManager.GetResString(offSet, 8341),
                        MessageType = MessageType.OkOnly
                    },
                    ShutDownPos = true,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
            Performancelog.Debug($"End,SaleManager,GetSaleNo,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Get current Sale number
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="message">Error</param>
        /// <returns>Sale number</returns>
        public int GetCurrentSaleNo(int tillNumber, string userCode, out ErrorMessage message)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,GetCurrentSaleNo,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            int returnValue = 0;
            message = new ErrorMessage();
            //var user = _loginManager.GetUser(userCode);
            var user = _loginManager.GetExistingUser(userCode);
            try
            {
                if (user.User_Group.Code == "Trainer") //Behrooz Jan-12-06
                {
                    returnValue = _saleService.GetMaxSaleNoFromSaleHeadFromDbTill(tillNumber);

                    int maxSusSale = _saleService.GetMaxSaleNoFromSusHeadFromDbTill(tillNumber);

                    if (maxSusSale >= returnValue)
                    {
                        returnValue = maxSusSale + 1;
                    }
                    return returnValue;
                }
                var messageNumber = 0;
                returnValue = _saleService.GetMaxSaleNo(tillNumber, out messageNumber);

                if (returnValue == 0)
                {
                    // Cannot get new invoice number, end the POS
                    //TODO: Udham Logger
                    //modStringPad.WriteToLogFile("Invoice number is 0. POS ended for till " + System.Convert.ToString(tillNumber));
                    //MsgBoxStyle temp_VbStyle3 = (int)MsgBoxStyle.OkOnly + MsgBoxStyle.Critical;
                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    message = new ErrorMessage
                    {
                        MessageStyle = new MessageStyle
                        {
                            Message = _resourceManager.GetResString(offSet, 8341),
                            MessageType = MessageType.OkOnly
                        },
                        ShutDownPos = true,
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                    return returnValue;
                }
            }
            catch (Exception ex)
            {

                // In GetSaleNo we should not use Solution Error form and allow the user
                // to continue, but end the POS if there is any error in this function
                // Log the error in the POSLog.txt file
                WriteToLogFile("Error in GetSaleNo. POS ended for till " + System.Convert.ToString(tillNumber) + " error is " + System.Convert.ToString(ex.HResult) + " " + ex.Message);
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                message = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = _resourceManager.GetResString(offSet, 8341),
                        MessageType = MessageType.OkOnly
                    },
                    ShutDownPos = true,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
            Performancelog.Debug($"End,SaleManager,GetCurrentSaleNo,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Method to apply taxes
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="value">Value</param>
        public void ApplyTaxes(ref Sale sale, bool value)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,ApplyTaxes,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var SL = new Sale_Line();
            if (value)
            {
                if (sale.ApplyTaxes)
                {
                    return;
                }
                sale.ApplyTaxes = true;
                // Changing from FALSE to TRUE ... Add the taxes back in.
                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    SL = tempLoopVarSl;
                    Compute_Taxes(ref sale, ref SL, 1);
                }
            }
            else
            {
                if (!sale.ApplyTaxes)
                {
                    return;
                }
                sale.ApplyTaxes = false;
                // Changing from TRUE to FALSE - Remove the taxes
                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    SL = tempLoopVarSl;
                    Compute_Taxes(ref sale, ref SL, 1);
                }
            }
            sale.TaxForTaxExempt = false; //   tax for tax exempt customers
                                          // ReCompute_Totals();
            if (!sale.LoadingTemp)
            {
                SaveTemp(ref sale, sale.TillNumber);
            }
            Performancelog.Debug($"End,SaleManager,ApplyTaxes,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Unsuspend Sale 
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <param name="userCode"></param>
        /// <param name="error"></param>
        /// <returns>Sale </returns>
        public Sale UnSuspend_Temp(int saleNumber, int tillNumber, string userCode, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,UnSuspend_Temp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            error = new ErrorMessage();
            //var sale = _saleService.GetSaleBySaleNoFromDbTemp(tillNumber, saleNumber);
            var sale = _saleService.GetSale(tillNumber, saleNumber);

            if (sale == null)
            {
                return null;
            }
            _saleHeadManager.SetSalePolicies(ref sale);

            //Add customer information
            sale.Customer = _customerManager.LoadCustomer(sale.Customer.Code);
            if (!string.IsNullOrEmpty(sale.CouponID))
            {
                ReCompute_Coupon(ref sale);
            }

            //  -ading for clearing the partial saved data -During crashing if data is saved in taxexempt tables when coming back claen up
            if (_policyManager.TAX_EXEMPT)
            {
                CleanupTeCrash(saleNumber, Convert.ToByte(tillNumber));
            }

            _saleService.DeleteSignatureFromDbTill(saleNumber, tillNumber);
            //need to cleanup signature saving if anything saved
            if (
                File.Exists(
                    new WindowsFormsApplicationBase().Info.DirectoryPath +
                    "\\Sign.bmp"))
            {
                File.Delete(
                    new WindowsFormsApplicationBase().Info.DirectoryPath +
                    "\\Sign.bmp");
            }

            var saleLines = _saleService.GetSaleLinesFromDbTemp(saleNumber, tillNumber, userCode);
            foreach (var saleLine in saleLines)
            {
                var sl = saleLine;
                _saleLineManager.SetSaleLinePolicy(ref sl);
                var quantity = saleLine.Quantity;
                var price = saleLine.price;
                var amount = saleLine.Amount;
                var discountRate = saleLine.Discount_Rate;
                var discountType = saleLine.Discount_Type;
                _saleLineManager.SetPluCode(ref sale, ref sl, saleLine.PLU_Code, out error);
                if (sl.Prepay && sl.pumpID > 0)
                {
                    Variables.Pump[sl.pumpID].PrepayAmount = Convert.ToSingle(sl.Amount);
                    Variables.Pump[sl.pumpID].PrepayInvoiceID = sale.Sale_Num;
                    Variables.Pump[sl.pumpID].PrepayPosition = sl.PositionID;

                    if (Variables.Pump[sl.pumpID].PrepayAmount < 0)
                    {
                        sale.DeletePrepay = true;
                    }

                }
                else if (sl.pumpID == 0)
                {

                    sl.Prepay = false;
                }
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                if (sl.Gift_Certificate && saleLine.GiftType == "LocalGift")
                {
                    sl.Description = _resourceManager.GetResString(offSet, 8124) + saleLine.Gift_Num;
                }

                if (sale.Void_Num != 0)
                {
                    Add_a_Line(ref sale, sl, userCode, tillNumber, out error, false, false, true);
                }
                else
                {
                    Add_a_Line(ref sale, sl, userCode, tillNumber, out error, false, false);
                }
                _saleLineManager.SetPrice(ref sl, price);
                _saleLineManager.SetQuantity(ref sl, quantity);
                _saleLineManager.SetAmount(ref sl, amount);
                saleLine.Discount_Type = discountType;
                _saleLineManager.SetDiscountRate(ref sl, discountRate);
                //TODO: Load givex temp data
            }

            //var saleTotals = _saleService.GetSaleTotals(saleNumber);
            //sale.Sale_Totals.Charge = saleTotals.Charge;
            //sale.Sale_Totals.SaleNumber = saleTotals.SaleNumber;
            //sale.Sale_Totals.Total = saleTotals.Total;
            sale.LoadingTemp = false;
            //update return reason 
            if (!string.IsNullOrEmpty(sale.Return_Reason.Reason) && !string.IsNullOrEmpty(sale.Return_Reason.RType))
            {
                sale.Return_Reason = _reasonService.GetReturnReason(sale.Return_Reason.Reason,
                    Convert.ToChar(sale.Return_Reason.RType));
            }

            Performancelog.Debug($"End,SaleManager,UnSuspend_Temp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            Chaps_Main.SA = sale; 
            return sale;
        }

        /// <summary>
        /// Method to add a saleline item
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="registerNumber">Registernumber</param>
        /// <param name="stockCode">Stock code</param>
        /// <param name="quantity">Quanity</param>
        /// <param name="isReturnMode">Return mode</param>
        /// <param name="giftCard">Gift card</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>True or false</returns>
        public Sale AddSaleLineItem(string userCode, int tillNumber, int saleNumber, byte registerNumber,
            string stockCode, decimal quantity, bool isReturnMode, GiftCard giftCard,
            out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,AddSaleLineItem,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            errorMessage = new ErrorMessage();
            if (string.IsNullOrEmpty(stockCode))
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = "Invalid stock Code",
                    MessageType = MessageType.OkOnly
                };
                errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                return null;
            }
            if (!ValidateTillAndSale(tillNumber, saleNumber, out errorMessage))
            {
                return null;
            }

            var sale = GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }


            var lineNumber = sale.Sale_Lines.Count + 1;
            Look_Up_Line(ref sale, lineNumber, stockCode, quantity, userCode, isReturnMode, giftCard, out errorMessage);

            //Update Sale object in Cache
            CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);

            Performancelog.Debug($"End,SaleManager,AddSaleLineItem,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            Chaps_Main.SA = sale;
            return sale;
        }



        /// <summary>
        /// Method to add a saleline item
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="registerNumber">Registernumber</param>
        /// <param name="stockCode">Stock code</param>
        /// <param name="quantity">Quanity</param>
        /// <param name="isReturnMode">Return mode</param>
        /// <param name="giftCard">Gift card</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>True or false</returns>
        public Sale VerifyAddSaleLineItem(string userCode, int tillNumber, int saleNumber, byte registerNumber,
            string stockCode, decimal quantity, bool isReturnMode, GiftCard giftCard,
            out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,AddSaleLineItem,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            errorMessage = new ErrorMessage();

            if (!ValidateTillAndSale(tillNumber, saleNumber, out errorMessage))
            {
                return null;
            }

            var sale = GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }


            var lineNumber = sale.Sale_Lines.Count + 1;
            Look_Up_Line(ref sale, lineNumber, stockCode, quantity, userCode, isReturnMode, giftCard, out errorMessage);

            //Update Sale object in Cache
            CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);

            Performancelog.Debug($"End,SaleManager,AddSaleLineItem,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            Chaps_Main.SA = sale;
            return sale;
        }




        /// <summary>
        /// Method to get sale summary
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Sale</returns>
        public object GetSaleSummary(int saleNumber, int tillNumber, string userCode, out ErrorMessage errorMessage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method to create a sale object from current sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="registerNumber"></param>
        /// <param name="userCode">User code</param>
        /// <returns></returns>
        public Sale GetCurrentSale(int saleNumber, int tillNumber, byte registerNumber, string userCode, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,GetCurrentSale,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
         
                errorMessage = new ErrorMessage();
            
            var currentSaleFromCache = CacheManager.GetCurrentSaleForTill(tillNumber, saleNumber);
            if (currentSaleFromCache != null && currentSaleFromCache.Sale_Num > 0)
            {
                if (registerNumber != 0)
                {
                    currentSaleFromCache.Register = registerNumber;
                }
                Performancelog.Debug($"End,SaleManager,GetCurrentSaleFromCache,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                Chaps_Main.SA = currentSaleFromCache;
                return currentSaleFromCache;
            }

            //get sale from db
            var sale = new Sale();
            //var user = _loginManager.GetUser(userCode);
            //var currentSale = _saleService.GetSaleBySaleNoFromDbTemp(tillNumber, saleNumber);
            var currentSale = _saleService.GetSale(tillNumber, saleNumber);

            if (currentSale == null)
            {
                //ValidateSale(saleNumber, tillNumber, userCode, out errorMessage);
                //if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                //{
                //    sale = _saleHeadManager.CreateNewSale();
                //    sale.TillNumber = (byte)tillNumber;
                //    if (registerNumber != 0)
                //    {
                //        sale.Register = registerNumber;
                //    }
                //    sale.Sale_Num = saleNumber;
                //    sale.Payment = false;
                //}
                errorMessage = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Request is invalid ",
                        MessageType = MessageType.OkOnly
                    },
                    StatusCode = HttpStatusCode.NotAcceptable
                };
                return null;
            }
            else
            {

                // _saleHeadManager.CreateSaleObject(ref currentSale, userCode, tillNumber, currentSale.Sale_Num);
                // sale = Clear_Sale(currentSale, userCode, "Initial", null, false, true, false, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                {
                    return null;
                }
                // moved here on June 10, 2003, outside form Load_Temp procedure
                sale.Payment = _saleService.CheckPaymentsFromDbTemp(currentSale.Sale_Num, tillNumber);
                //  - crash recovery
                sale = Load_Temp(currentSale.Sale_Num, tillNumber, userCode, out errorMessage);
                if (registerNumber != 0)
                {
                    sale.Register = registerNumber;
                }
            }

            CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);
            Performancelog.Debug($"End,SaleManager,GetCurrentSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            Chaps_Main.SA = sale; 
            return sale;
        }

        /// <summary>
        /// Method to look up in line for adding a sale line item
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="stockCode">Stock code</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="userCode">User code</param>
        /// <param name="isReturnMode">Return mode or not</param>
        /// <param name="giftCard">Gift card</param>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        public void Look_Up_Line(ref Sale sale, int lineNumber, string stockCode, decimal quantity, string userCode,
          bool isReturnMode, GiftCard giftCard, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Look_Up_Line,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            Register register = new Register();
            _mainManager.SetRegister(ref register, sale.Register);
            error = new ErrorMessage();
            string cStock = "";
            bool scaleItem = false; //08/21/07 Nancy added to check if it's scalable item
            float vQty = 0;
            float vAmount = 0;
            if (isReturnMode)
            {
                quantity = -quantity;
            }

            scaleItem = false;
            //var user = _loginManager.GetUser(userCode);
            //04/11/13 Reji Updated for validating user inputs from Sql Injections

            string tempUserInputString = stockCode;
            string tempAvoidedValuesString = "";
            cStock = Strings.UCase(Convert.ToString(Helper.SqlQueryCheck(ref tempUserInputString, ref tempAvoidedValuesString))); ////avoided string values Sample string = """,',--"


            if (!string.IsNullOrEmpty(cStock) && sale.Sale_Lines.Count == (lineNumber - 1))
            {



                //  -to support UPC-A10 - In this case we don't need to look for starting 2 for the scalables

                if (_policyManager.Scale_Item &&
                    ((cStock.Length == 12 && cStock.Substring(0, 1) == "2" && _policyManager.UPC_Format == "UPC-A")
                    || (cStock.Length == 13 && cStock.Substring(0, 2) == "02" && _policyManager.UPC_Format == "EAN-13")
                    || (cStock.Length == 13 && cStock.Substring(0, 2) == "22" && _policyManager.UPC_Format == "EAN-13")
                    || (cStock.Length == 10 && cStock.Substring(0, 2) == "00" && _policyManager.UPC_Format == "UPC-A10")))
                {

                    // 


                    var sStock = "";
                    if (_policyManager.UPC_Format == "UPC-A")
                    {
                        sStock = cStock.Substring(1, _policyManager.ItemCodeDgt);

                        if (_policyManager.UseWeight)
                        {
                            vQty = float.Parse((Conversion.Val(cStock.Substring(_policyManager.ItemCodeDgt + 3 - 1, _policyManager.AmountDigit)) / 1000).ToString("#0.000"));
                        }
                        else
                        {
                            vAmount = float.Parse((Conversion.Val(cStock.Substring(_policyManager.ItemCodeDgt + 3 - 1, _policyManager.AmountDigit)) / 100).ToString("#0.00"));
                        }
                        //  to support - UPC-A10 for scalable

                    }
                    else if (_policyManager.UPC_Format == "UPC-A10") //10 digit UPC
                    {
                        sStock = cStock.Substring(0, _policyManager.ItemCodeDgt);

                        if (_policyManager.UseWeight)
                        {
                            vQty = float.Parse((Conversion.Val(cStock.Substring(_policyManager.ItemCodeDgt + 2 - 1, _policyManager.AmountDigit)) / 1000).ToString("#0.000"));
                        }
                        else
                        {
                            vAmount = float.Parse((Conversion.Val(cStock.Substring(_policyManager.ItemCodeDgt + 2 - 1, _policyManager.AmountDigit)) / 100).ToString("#0.00"));
                        }
                        // 
                    }
                    else
                    {
                        sStock = cStock.Substring(2, _policyManager.ItemCodeDgt);

                        if (_policyManager.UseWeight)
                        {

                            vQty = float.Parse((Conversion.Val(cStock.Substring(_policyManager.ItemCodeDgt + 4 - 1, _policyManager.AmountDigit)) / 1000).ToString("#0.000"));
                        }
                        else
                        {
                            vAmount = float.Parse((Conversion.Val(cStock.Substring(_policyManager.ItemCodeDgt + 4 - 1, _policyManager.AmountDigit)) / 100).ToString("#0.00"));
                        }
                    }
                    cStock = sStock;
                    //  and length 5 digits need to consider it as regular products
                    if (vQty == 0 & vAmount == 0)
                    {
                        scaleItem = false;
                    }
                    else
                    {
                        // 
                        scaleItem = true;
                    }
                }

                var offSet = _policyManager.LoadStoreInfo().OffSet;
                var thisLine = _saleLineManager.CreateNewSaleLine();
                thisLine.User = userCode;
                thisLine.Sale_Num = sale.Sale_Num.ToString();
                thisLine.Till_Num = sale.TillNumber;
                _saleLineManager.SetPluCode(ref sale, ref thisLine, stockCode, out error, (quantity < 0));
                thisLine.Line_Num = (short)(sale.Sale_Lines.Count + 1);
                //validate price
                if (!IsValidPrice(thisLine.PRICE_DEC, thisLine.price, out error))
                {
                    return;
                }

                //validate quantity
                if (!IsValidQuantity(thisLine.QUANT_DEC, quantity, out error))
                {
                    return;
                }

                // 
                if (string.IsNullOrEmpty(error.MessageStyle.Message))
                {
                    //var pluMast = _stockService.GetPluMast(thisLine.PLU_Code);
                    //var stockItem = _stockService.GetStockItemByCode(cStock, _policyManager.Sell_Inactive);
                    //if ((thisLine.PluType == 'S' || thisLine.PluType == 'K') && thisLine.Stock_Type != 'G')
                    //Tony 03/20/2019
                    if ((thisLine.PluType == 'S' || thisLine.PluType == 'K')
                        && !(thisLine.Stock_Type == 'G' && thisLine.GiftType == "LocalGift")
                        && !(thisLine.Stock_Type == 'G' && thisLine.GiftType == "GiveX")
                        )
                        //end
                    {
                        //cStock = thisLine.Stock_Code;



                        //string temp_Policy_Name = "ALLOW_ENT";
                        //if (_policyManager.GetPol(temp_Policy_Name, thisLine))
                        //{
                        if (sale.Customer.Price_Code != 1 & sale.Customer.Price_Code <= _policyManager.NUM_PRICE)
                        {

                            _saleLineManager.SetPriceNumber(ref thisLine, sale.Customer.Price_Code);
                        }
                        else
                        {
                            _saleLineManager.SetPriceNumber(ref thisLine, 1);
                        }

                        //Shiny Dec5, 2008 -added for the products stored by weight
                        var qtyFormat = "";
                        //if (Chaps_Main.TEC_OScale != null || Chaps_Main.OSCALE != null) // 
                        //{
                        //    if (thisLine.Stock_BY_Weight)
                        //    {
                        //        var lngweight = 0;
                        //        var retryscale = true;
                        //        qtyFormat = Convert.ToString(thisLine.QUANT_DEC == 0 ? "#0" : ("#0." + new string('0', thisLine.QUANT_DEC)));
                        //        while (retryscale != false)
                        //        {
                        //            if (Chaps_Main.Register_Renamed.OPOS_Scale == false)
                        //            {
                        //                if (Chaps_Main.TEC_OScale.GetWeightSync(lngweight))
                        //                {
                        //                    vQty = (float)((double)lngweight / 1000);
                        //                    retryscale = false;
                        //                }
                        //                else
                        //                {
                        //                    vQty = 0;
                        //                }
                        //                // 
                        //            }
                        //            else if (Chaps_Main.Register_Renamed.OPOS_Scale) //OPOS
                        //            {
                        //                //  Mageelan scale is also using Scale.ocx - not OPOSSCALE.OCX- So commenting out this section  until we get a real opos scale
                        //                
                        //                
                        //                
                        //                
                        //                
                        //                
                        //                // 
                        //            }
                        //            //Shiny end
                        //            if (vQty == 0)
                        //            {
                        //                error.StatusCode = HttpStatusCode.Gone;
                        //                error.MessageStyle = new MessageStyle
                        //                {
                        //                    Message = "Scale couldn\'t read the weight for the product.Do you want to get the weight again?",
                        //                    MessageType = (int)MessageType.Question + MessageType.YesNo
                        //                };
                        //                return;
                        //            }
                        //        }
                        //        _saleLineManager.SetQuantity(ref thisLine, float.Parse(vQty.ToString(qtyFormat)));
                        //        _saleLineManager.SetAmount(ref thisLine, decimal.Parse(vAmount.ToString("#0.00")));
                        //    }
                        //}
                        // 

                        if (scaleItem)
                        {
                            qtyFormat = Convert.ToString(thisLine.QUANT_DEC == 0 ? "#0" : ("#0." + new string('0', thisLine.QUANT_DEC)));
                            var priceFormat = Convert.ToString(thisLine.PRICE_DEC == 0 ? "#0" : ("#0." + new string('0', thisLine.PRICE_DEC)));
                            thisLine.ScalableItem = true;

                            if (_policyManager.UseWeight)
                            {
                                _saleLineManager.SetQuantity(ref thisLine, float.Parse((vQty).ToString(qtyFormat)));
                                _saleLineManager.SetPrice(ref thisLine, double.Parse((thisLine.Regular_Price).ToString(priceFormat)));
                            }
                            else
                            {
                                float vPrice = 0;
                                if (thisLine.Regular_Price > 0)
                                {
                                    vQty = float.Parse((vAmount / thisLine.Regular_Price).ToString("#0.000"));
                                    vPrice = (float)thisLine.Regular_Price;
                                }
                                else
                                {
                                    vQty = 1;
                                    vPrice = vAmount;
                                }
                                _saleLineManager.SetQuantity(ref thisLine, float.Parse(vQty.ToString(qtyFormat)));
                                thisLine.Regular_Price = double.Parse(vPrice.ToString(priceFormat));
                                _saleLineManager.SetPrice(ref thisLine, double.Parse(vPrice.ToString(priceFormat)));
                                _saleLineManager.SetAmount(ref thisLine, decimal.Parse(vAmount.ToString("#0.00")));
                            }


                        }
                        
                        //Added this code to add stock from hot products
                        else
                        {
                            _saleLineManager.SetQuantity(ref thisLine, (float)quantity);
                            _saleLineManager.SetPrice(ref thisLine, thisLine.price);
                        }

                        if (!Add_a_Line(ref sale, thisLine, thisLine.User, sale.TillNumber, out error, true, forRefund: sale.Void_Num == 0 ? true : false))
                        {
                            error.MessageStyle = new MessageStyle
                            {
                                MessageType = MessageType.OkOnly,
                                Message = "Unable to add stock"
                            };
                            error.StatusCode = HttpStatusCode.BadRequest;
                            if (register.Customer_Display && thisLine.SendToLCD)
                            {
                                sale.CustomerDisplay = _mainManager.DisplayMsgLcd(register, Convert.ToString(_mainManager.FormatLcdString(register, thisLine.Description.Trim(),
                                     thisLine.Quantity >= 0 ? thisLine.price.ToString() : "-" + thisLine.price.ToString())),
                                     _mainManager.FormatLcdString(register, "" + _resourceManager.GetResString(offSet, (short)210), sale.Sale_Totals.Gross.ToString("0.00")));
                                thisLine.SendToLCD = false; // July 22, 2009
                            }
                            return;
                        }
                        else
                        {
                            if (register.Customer_Display && thisLine.SendToLCD)
                            {
                                sale.CustomerDisplay = _mainManager.DisplayMsgLcd(register, Convert.ToString(_mainManager.FormatLcdString(register, thisLine.Description.Trim(),
                                     thisLine.Quantity >= 0 ? thisLine.price.ToString() : "-" + thisLine.price.ToString())),
                                     _mainManager.FormatLcdString(register, "" + _resourceManager.GetResString(offSet, (short)210), sale.Sale_Totals.Gross.ToString("0.00")));
                                thisLine.SendToLCD = false; // July 22, 2009
                            }
                        }

                    }
                    else
                    {
                        // Gift Certificate



                        //Giftcard
                        //                If This_Line.GiftType = "LocalGift" Then
                        if (thisLine.GiftType == "LocalGift")
                        {



                            if (isReturnMode)
                            {

                                error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 55, null, MessageType.OkOnly);
                                error.StatusCode = HttpStatusCode.Conflict;
                                return;
                            }

                            if (_policyManager.GIFTCERT)
                            {

                                if (_policyManager.GC_NUMBERS)
                                {
                                    if (giftCard.GiftNumber != 0)
                                    {
                                        for (int i = 1; i <= (int)quantity; i++)
                                        {


                                            var newSaleLine = _saleLineManager.CreateNewSaleLine();
                                            newSaleLine.Sale_Num = sale.Sale_Num.ToString();
                                            newSaleLine.Till_Num = sale.TillNumber;
                                            _saleLineManager.SetPluCode(ref sale, ref newSaleLine, cStock, out error, isReturnMode);
                                            //validate price
                                            if (!IsValidPrice(newSaleLine.PRICE_DEC, giftCard.Price, out error))
                                            {
                                                return;
                                            }

                                            newSaleLine.User = userCode;
                                            newSaleLine.Line_Num = (short)(sale.Sale_Lines.Count + 1);
                                            newSaleLine.Description = _resourceManager.GetResString(offSet, 8124) + Convert.ToString(giftCard.GiftNumber); //"Gift Cert "
                                            newSaleLine.Regular_Price = Conversion.Val(giftCard.Price);
                                            newSaleLine.Gift_Num = giftCard.GiftNumber.ToString();
                                            // This_Line.price = Conversion.Val(txtGCPrice.Text);
                                            _saleLineManager.SetPrice(ref newSaleLine, Conversion.Val(giftCard.Price));
                                            newSaleLine.Gift_Num = (giftCard.GiftNumber).ToString();

                                            if (!Add_a_Line(ref sale, newSaleLine, newSaleLine.User, sale.TillNumber, out error, true))
                                            {
                                                error.MessageStyle = new MessageStyle
                                                {
                                                    MessageType = MessageType.OkOnly,
                                                    Message = "Unable to add stock"
                                                };
                                                error.StatusCode = HttpStatusCode.BadRequest;
                                                return;
                                            }
                                            // Nov 17, 2008 Nicolette to send to customer display, see begining of the function
                                            if (register.Customer_Display && thisLine.SendToLCD)
                                            {
                                                sale.CustomerDisplay = _mainManager.DisplayMsgLcd(register, Convert.ToString(_mainManager.FormatLcdString(register, thisLine.Description.Trim(),
                                                     newSaleLine.Quantity >= 0 ? newSaleLine.Regular_Price.ToString() : "-" + newSaleLine.Regular_Price.ToString())),
                                                     _mainManager.FormatLcdString(register, "" + _resourceManager.GetResString(offSet, (short)210), sale.Sale_Totals.Gross.ToString("0.00")));
                                                thisLine.SendToLCD = false; // July 22, 2009
                                            }
                                            giftCard.GiftNumber++;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!Add_a_Line(ref sale, thisLine, thisLine.User, sale.TillNumber, out error, adjust: true, forRefund: sale.Void_Num == 0 ? true : false))
                                    {
                                        error.MessageStyle = new MessageStyle
                                        {
                                            MessageType = MessageType.OkOnly,
                                            Message = "Unable to add stock"
                                        };
                                        error.StatusCode = HttpStatusCode.BadRequest;
                                        return;
                                    }

                                    // Nov 17, 2008 Nicolette to send to customer display, see begining of the function
                                    if (register.Customer_Display && thisLine.SendToLCD)
                                    {
                                        sale.CustomerDisplay = _mainManager.DisplayMsgLcd(register, Convert.ToString(_mainManager.FormatLcdString(register, thisLine.Description.Trim(),
                                             thisLine.Quantity >= 0 ? thisLine.price.ToString() : "-" + thisLine.price.ToString())),
                                             _mainManager.FormatLcdString(register, "" + _resourceManager.GetResString(offSet, (short)210), sale.Sale_Totals.Gross.ToString("0.00")));
                                        thisLine.SendToLCD = false; // July 22, 2009
                                    }
                                    // Nov 17, 2008 Nicolette end

                                }
                            }
                            else
                            {
                                // "You do NOT sell Gift Certificates"
                                error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8125, null, MessageType.OkOnly);
                                error.StatusCode = HttpStatusCode.Unauthorized;
                                return; //   

                            }


                        }
                        else if (thisLine.GiftType == "GiveX")
                        {


                            if (isReturnMode)
                            {

                                error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 55, null, MessageType.OkOnly);
                                error.StatusCode = HttpStatusCode.Unauthorized;
                                return;
                            }

                            if (_policyManager.GIFTCERT)
                            {
                                var refNum = string.Empty;
                                if (!string.IsNullOrEmpty(giftCard.CardNumber))
                                {
                                    GiveXReceiptType receipt;
                                    if (_givexClientManager.ActivateGiveX(giftCard.CardNumber.Trim(), (float)(Conversion.Val(giftCard.Price)),
                                        sale.Sale_Num, ref refNum, userCode, out error, out receipt))
                                    {
                                        //validate price
                                        if (!IsValidPrice(thisLine.PRICE_DEC, giftCard.Price, out error))
                                        {
                                            return;
                                        }
                                        thisLine.Line_Num = (short)(sale.Sale_Lines.Count + 1);
                                        thisLine.Regular_Price = Conversion.Val(giftCard.Price);
                                        _saleLineManager.SetPrice(ref thisLine, Conversion.Val(giftCard.Price));
                                        thisLine.Gift_Num = giftCard.CardNumber.Trim();
                                        thisLine.Serial_No = refNum;
                                        if (!Add_a_Line(ref sale, thisLine, thisLine.User, sale.TillNumber, out error, true))
                                        {
                                            error.MessageStyle = new MessageStyle
                                            {
                                                MessageType = MessageType.OkOnly,
                                                Message = "Unable to add stock"
                                            };
                                            error.StatusCode = HttpStatusCode.BadRequest;
                                            return;
                                        }
                                        // Nov 17, 2008 Nicolette to send to customer display, see begining of the function
                                        if (register.Customer_Display && thisLine.SendToLCD)
                                        {
                                            sale.CustomerDisplay = _mainManager.DisplayMsgLcd(register, Convert.ToString(_mainManager.FormatLcdString(register, thisLine.Description.Trim(),
                                                 thisLine.Quantity >= 0 ? thisLine.price.ToString() : "-" + thisLine.price.ToString())),
                                                 _mainManager.FormatLcdString(register, "" + _resourceManager.GetResString(offSet, (short)210), sale.Sale_Totals.Gross.ToString("0.00")));
                                            thisLine.SendToLCD = false; // July 22, 2009
                                            Chaps_Main.SA = sale;
                                        }
                                    }
                                    else
                                    {
                                        return;
                                    }

                                }
                                else
                                {
                                    error.MessageStyle = new MessageStyle
                                    {
                                        MessageType = MessageType.OkOnly,
                                        Message = "Empty givex card number"
                                    };
                                    error.StatusCode = HttpStatusCode.BadRequest;
                                    return;
                                }


                                //Gift
                            }
                            else if (cStock == "NS") // Non-Stock Item
                            {
                                Delete_Line((short)lineNumber, ref sale, out error);
                                return; //  
                            }
                        }
                        else
                        {
                            error.MessageStyle = new MessageStyle
                            {
                                Message = "Invalid Stock Item",
                                MessageType = MessageType.OkOnly
                            };
                            error.StatusCode = HttpStatusCode.NotAcceptable;
                            return;
                        }
                    }
                }
            }
            Performancelog.Debug($"End,SaleManager,Look_Up_Line,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }




        //   end


        //####PTC - Feb6, 2008 -End

        // Add a new line to the sale.


        //  
        //Shiny added the Getweight parameter to get the weight from the scale for the scalable product
        /// <summary>
        /// Method to add a line
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="userCode">User code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="error">Error</param>
        /// <param name="adjust">Adjust</param>
        /// <param name="tableAdjust">Table adjust</param>
        /// <param name="tempFile">Temporary file</param>
        /// <param name="forRefund">For refund or not</param>
        /// <param name="forReprint">For reprint or not</param>
        /// <param name="makePromo">Make promo</param>
        /// <param name="getWeight">Get weight or not</param>
        /// <returns>True or false</returns>
        public bool Add_a_Line(ref Sale sale, Sale_Line oLine, string userCode, int tillNumber, out ErrorMessage error, bool adjust = false,
            bool tableAdjust = true, bool tempFile = false, bool forRefund = false, bool forReprint = false,
            bool makePromo = false, bool getWeight = false)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Add_a_Line,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            Chaps_Main.SA = sale;
            bool returnValue = false;
            var securityRenamed = _policyManager.LoadSecurityInfo();
            var user = _loginManager.GetExistingUser(userCode);
            error = new ErrorMessage(); ;

            sale.ForCorrection = false;

            if (oLine.Dept == _policyManager.CarwashDepartment)
            {
              oLine.IsCarwashProduct = true;
            }
            
            if (oLine.Quantity == 0)
            {
                return false;
            }

            // Nicolette added for restricted products
            if (oLine.Confirmed == false)
            {
                return false;
            }
            // Nicolette end


            if (!forRefund)
            {
                sale.Void_Num = 0;
            }



            //  - For fule and gift certificate we shouldn't change the price_number
            //    If oLine.Stock_Type <> "G" Then
            if (oLine.Gift_Certificate == false && oLine.ProductIsFuel == false)
            {
                // 




                if (sale.Customer != null && !oLine.ScalableItem)
                {


                    //  For loyalty
                    if (sale.USE_LOYALTY && sale.LOYAL_TYPE.ToUpper() == "PRICES" &&
                        !string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "A")
                    {


                        if (!oLine.LOY_EXCLUDE)
                        {

                            if (sale.Loyal_pricecode > 0)
                            {
                                _saleLineManager.SetPriceNumber(ref oLine, sale.Loyal_pricecode);
                            }
                            else
                            {
                                _saleLineManager.SetPriceNumber(ref oLine, sale.Customer.Price_Code);
                            }
                        }
                        else
                        {
                            _saleLineManager.SetPriceNumber(ref oLine, sale.Customer.Price_Code);
                        }
                    }
                    else
                    {
                        if (sale.Customer.Price_Code > 0)
                        {
                            _saleLineManager.SetPriceNumber(ref oLine, sale.Customer.Price_Code);
                        }
                    }
                }
                //Shiny end


            }
            else if (oLine.GiftType == "GiveX")
            {
                var givexReceipt = new GiveXReceiptType
                {
                    Date = Variables.GX_Receipt.Date,
                    Time = Variables.GX_Receipt.Date,
                    UserID = Variables.GX_Receipt.Date,
                    TranType = Variables.GX_Receipt.TranType,
                    SaleNum = Variables.GX_Receipt.SaleNum,
                    SeqNum = Variables.GX_Receipt.SeqNum,
                    CardNum = Variables.GX_Receipt.CardNum,
                    ExpDate = Variables.GX_Receipt.ExpDate,
                    Balance = Variables.GX_Receipt.Balance,
                    PointBalance = Variables.GX_Receipt.PointBalance,
                    SaleAmount = Variables.GX_Receipt.SaleAmount,
                    ResponseCode = Variables.GX_Receipt.ResponseCode

                };
                givexReceipt.CardNum = _encryptManager.Encrypt(givexReceipt.CardNum, "");
                _saleService.SaveCardSales(sale, oLine, givexReceipt, DataSource.CSCCurSale);



            }

            
            returnValue = true;
            Compute_Charges(ref sale, ref oLine, 1);
            Compute_Taxes(ref sale, ref oLine, 1);
            oLine.User = Convert.ToString(user.Code);

            if (adjust)
            {
                if (Adjust_Lines(ref oLine, sale, true))
                {
                    //  For using Loyalty discount
                    if (sale.USE_LOYALTY && sale.LOYAL_TYPE.ToUpper() == "DISCOUNTS" &&
                        !string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "A")
                    {


                        if (!oLine.LOY_EXCLUDE)
                        {

                            if (sale.CUST_DISC && sale.PROD_DISC)
                            {
                                _saleLineManager.Apply_Table_Discount(ref oLine, oLine.Prod_Discount_Code, sale.Loydiscode, out error);
                            }
                            else if (sale.CUST_DISC)
                            {
                                _saleLineManager.Apply_Table_Discount(ref oLine, 0, sale.Loydiscode, out error);
                            }
                            else if (sale.PROD_DISC)
                            {
                                _saleLineManager.Apply_Table_Discount(ref oLine, oLine.Prod_Discount_Code, 0, out error);
                            }

                        }
                        else
                        {
                            if (sale.CUST_DISC && sale.PROD_DISC)
                            {
                                _saleLineManager.Apply_Table_Discount(ref oLine, oLine.Prod_Discount_Code, sale.Customer.Discount_Code, out error);
                            }
                            else if (sale.CUST_DISC)
                            {
                                _saleLineManager.Apply_Table_Discount(ref oLine, 0, sale.Customer.Discount_Code, out error);
                            }
                            else if (sale.PROD_DISC)
                            {
                                _saleLineManager.Apply_Table_Discount(ref oLine, oLine.Prod_Discount_Code, 0, out error);
                            }

                        }
                        Line_Discount_Type(ref oLine, oLine.Discount_Type);
                        Line_Discount_Rate(ref sale, ref oLine, oLine.Discount_Rate);
                    }
                    else
                    {
                        //Shiny end
                        if (sale.CUST_DISC && sale.PROD_DISC)
                        {
                            _saleLineManager.Apply_Table_Discount(ref oLine, oLine.Prod_Discount_Code, sale.Customer.Discount_Code, out error);
                        }
                        else if (sale.CUST_DISC)
                        {
                            _saleLineManager.Apply_Table_Discount(ref oLine, 0, sale.Customer.Discount_Code, out error);
                        }
                        else if (sale.PROD_DISC)
                        {
                            _saleLineManager.Apply_Table_Discount(ref oLine, oLine.Prod_Discount_Code, 0, out error);
                        }
                        Line_Discount_Type(ref oLine, oLine.Discount_Type);
                        Line_Discount_Rate(ref sale, ref oLine, oLine.Discount_Rate);
                    } //only this end if Shiny
                      //







                    if (!forReprint)
                    {
                        if (oLine.FuelRebateEligible && oLine.FuelRebate > 0 && sale.Customer.UseFuelRebate &&
                            sale.Customer.UseFuelRebateDiscount)
                        {
                            _saleLineManager.ApplyFuelRebate(ref oLine);
                        }
                        else
                        {
                            if (oLine.ProductIsFuel && !string.IsNullOrEmpty(sale.Customer.GroupID) && _policyManager.FuelLoyalty)
                            {

                                if (sale.Customer.DiscountType == "%" || sale.Customer.DiscountType == "$" ||
                                    sale.Customer.DiscountType == "D")
                                // 
                                {
                                    //  Discountchart loyalty
                                    //same as $discount by litre- only difference is discount rate should be based on grade
                                    if (sale.Customer.DiscountType == "D")
                                    {
                                        //   added next If Not ForRefund to not apply the discount for QITE that uses type "D" discount
                                        if (!forRefund)
                                        {
                                            _saleLineManager.ApplyFuelLoyalty(ref oLine, sale.Customer.DiscountType,
                                                 _saleLineManager.GetFuelDiscountChartRate(ref oLine, sale.Customer.GroupID, oLine.GradeID),
                                                 sale.Customer.DiscountName);

                                            // this will bring the discount rate based on customer group id and fuel grade
                                        }
                                    }
                                    else
                                    {
                                        // 
                                        _saleLineManager.ApplyFuelLoyalty(ref oLine, sale.Customer.DiscountType, sale.Customer.DiscountRate,
                                             sale.Customer.DiscountName);
                                    }
                                }
                            }
                        }
                    }



                    if (sale.Upsell)
                    {
                        oLine.Upsell = true;
                    }

                    sale.Sale_Lines.AddLine((short)(sale.Sale_Lines.Count + 1), oLine, (sale.Sale_Lines.Count + 1).ToString());

                }
                else
                {
                    //
                    //We might be adding an item that already exists in the sales screen as well as the coupon for it
                    //We need to increase the available qty and reset Processed
                    if (sale.Sale_Lines.blCoupon && oLine.Stock_Type != 'P')
                    {
                        //qty has been increased
                        foreach (Sale_Line tempLoopVarSLine in sale.Sale_Lines)
                        {
                            var sLine = tempLoopVarSLine;
                            if (oLine.Stock_Code == sLine.Stock_Code)
                            {
                                sLine.Processed = false;
                                sLine.AvailableQty = (short)sLine.Quantity;
                                break;
                            }
                        }
                    }
                    //End - SV
                    //returnValue = false;
                    //            MakePromo = True ' July 14, 2009 '  
                    // MakePromo should not be required to be set to True here once the promo is done in Sale_Line.Stock_Code Let property
                    // All the items added to the sale should already have the promo made at this point
                }
            }
            else
            {
                if (tableAdjust)
                {
                    //  For using Loyalty discount
                    if (sale.USE_LOYALTY && sale.LOYAL_TYPE.ToUpper() == "DISCOUNTS" &&
                        !string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "A")
                    {
                        if (!oLine.LOY_EXCLUDE)
                        {
                            if (sale.CUST_DISC && sale.PROD_DISC)
                            {
                                _saleLineManager.Apply_Table_Discount(ref oLine, oLine.Prod_Discount_Code, sale.Loydiscode, out error);
                            }
                            else if (sale.CUST_DISC)
                            {
                                _saleLineManager.Apply_Table_Discount(ref oLine, 0, sale.Loydiscode, out error);
                            }
                            else if (sale.PROD_DISC)
                            {
                                _saleLineManager.Apply_Table_Discount(ref oLine, oLine.Prod_Discount_Code, 0, out error);
                            }
                        }
                        else
                        {
                            if (sale.CUST_DISC && sale.PROD_DISC)
                            {
                                _saleLineManager.Apply_Table_Discount(ref oLine, oLine.Prod_Discount_Code, sale.Customer.Discount_Code, out error);
                            }
                            else if (sale.CUST_DISC)
                            {
                                _saleLineManager.Apply_Table_Discount(ref oLine, 0, sale.Customer.Discount_Code, out error);
                            }
                            else if (sale.PROD_DISC)
                            {
                                _saleLineManager.Apply_Table_Discount(ref oLine, oLine.Prod_Discount_Code, 0, out error);
                            }
                        }
                        Line_Discount_Type(ref oLine, oLine.Discount_Type);
                        Line_Discount_Rate(ref sale, ref oLine, oLine.Discount_Rate);

                    }
                    else
                    {
                        if (sale.CUST_DISC && sale.PROD_DISC)
                        {
                            _saleLineManager.Apply_Table_Discount(ref oLine, oLine.Prod_Discount_Code, sale.Customer.Discount_Code, out error);
                        }
                        else if (sale.CUST_DISC)
                        {
                            _saleLineManager.Apply_Table_Discount(ref oLine, 0, sale.Customer.Discount_Code, out error);
                        }
                        else if (sale.PROD_DISC)
                        {
                            _saleLineManager.Apply_Table_Discount(ref oLine, oLine.Prod_Discount_Code, 0, out error);
                        }
                        Line_Discount_Type(ref oLine, oLine.Discount_Type);
                        Line_Discount_Rate(ref sale, ref oLine, oLine.Discount_Rate);
                    }
                }








                if (!forReprint)
                {
                    if (oLine.FuelRebateEligible && oLine.FuelRebate > 0 && sale.Customer.UseFuelRebate &&
                        sale.Customer.UseFuelRebateDiscount)
                    {
                        _saleLineManager.ApplyFuelRebate(ref oLine);
                    }
                    else
                    {
                        if (oLine.ProductIsFuel && !string.IsNullOrEmpty(sale.Customer.GroupID) && _policyManager.FuelLoyalty)
                        {
                            if (sale.Customer.DiscountType == "%" || sale.Customer.DiscountType == "$" ||
                                sale.Customer.DiscountType == "D")
                            {
                                //  Discountchart loyalty
                                //same as $discount by litre- only difference is discount rate should be based on grade
                                if (sale.Customer.DiscountType == "D")
                                {
                                    //   added next If Not ForRefund to not apply the discount for QITE that uses type "D" discount
                                    if (!forRefund)
                                    {
                                        _saleLineManager.ApplyFuelLoyalty(ref oLine, sale.Customer.DiscountType,
                                            _saleLineManager.GetFuelDiscountChartRate(ref oLine, sale.Customer.GroupID, oLine.GradeID));
                                        // this will bring the discount rate based on customer group id and fuel grade
                                    }
                                }
                                else
                                {
                                    // 
                                    _saleLineManager.ApplyFuelLoyalty(ref oLine, sale.Customer.DiscountType, sale.Customer.DiscountRate,
                                         sale.Customer.DiscountName);
                                }
                            }
                        }
                    }
                }

                //Nancy added to keep the new line as Upsell if needed
                if (sale.Upsell)
                {
                    oLine.Upsell = true;
                }
                sale.Sale_Lines.AddLine((short)(sale.Sale_Lines.Count + 1), oLine, (sale.Sale_Lines.Count + 1).ToString());
            }

            // form
            if (!_policyManager.FUELONLY && adjust &&
                (Strings.UCase(Convert.ToString(securityRenamed.BackOfficeVersion)) == "FULL" ||
                 _policyManager.PROMO_SALE) &&
                (!string.IsNullOrEmpty(oLine.PromoID) || oLine.ProductIsFuel || (string.IsNullOrEmpty(oLine.PromoID) && makePromo) ||
                 oLine.RefreshPrice))
            {

                if ((oLine.ProductIsFuel && string.IsNullOrEmpty(oLine.PromoID))
                    || (string.IsNullOrEmpty(oLine.PromoID) && makePromo))
                {
                    _saleLineManager.Make_Promo(ref sale, ref oLine);
                }

                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    var sl = tempLoopVarSl;
                    sale.MakingPromo = true;
                    if ((sl.PromoID == oLine.PromoID && (sl.Line_Num != oLine.Line_Num)) || makePromo ||
                        oLine.RefreshPrice)
                    {
                        if (Adjust_Lines(ref sl, sale, false)) // September 22, 2008
                        {
                        }
                        //SaleMain.Default.Refresh_Lines(); // Apr 28, 2009 to fix subscript out of range issue
                    }
                    sale.MakingPromo = false;
                }
                if (makePromo)
                {
                    if (Adjust_Lines(ref oLine, sale, false))
                    {
                    }
                }
                oLine.RefreshPrice = false;
            }
            // End  

            Sale_Discount(ref sale, sale.Sale_Totals.Invoice_Discount, sale.Sale_Totals.Invoice_Discount_Type, tempFile,
                true);

            //Nancy added for Fuel Loyalty of Coupon type
            if (!forReprint)
            {
                ReCompute_Coupon(ref sale);
            }

            ReCompute_Totals(ref sale);

            if (!sale.LoadingTemp)
            {
                SaveTemp(ref sale, tillNumber);
            }
            //
            //We need to validate to see if we can use this coupon
            if (sale.Sale_Lines.Count > 0)
            {
                //Validate only if the item is the coupon, is not for reprint, is not for return and added in sales screen
                if (sale.Sale_Lines[sale.Sale_Lines.Count].Stock_Type == 'P' && !forReprint &&
                    (sale.Sale_Totals.Gross > 0 | sale.Sale_Lines.Count == 1))
                {
                    //Change price to negative since it is a coupon
                    var saleLine = sale.Sale_Lines[sale.Sale_Lines.Count];
                    _saleLineManager.SetPrice(ref saleLine, -1 * sale.Sale_Lines[sale.Sale_Lines.Count].price);
                    //Smriti update price on index
                    if (ValidateCoupon(ref sale, (short)sale.Sale_Lines.Count, out error) == false)
                    {
                        returnValue = false;
                        return returnValue;
                    }
                    sale.Sale_Lines.blCoupon = true;
                }
            }
            Performancelog.Debug($"End,SaleManager,Add_a_Line,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            Chaps_Main.SA = sale;
            return returnValue;
        }


        /// <summary>
        /// Method to recompute coupon
        /// </summary>
        /// <param name="sale">Sale</param>
        public void ReCompute_Coupon(ref Sale sale)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,ReCompute_Coupon,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (string.IsNullOrEmpty(sale.Customer.GroupID) || sale.Customer.DiscountType != "C" || !_policyManager.FuelLoyalty)
            {
                return;
            }

            var oldCouponTotal = sale.CouponTotal;

            var overPayPrepay = false;
            sale.CouponTotal = 0;
            foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
            {
                var sl = tempLoopVarSl;

                if (sl.Prepay && sale.OverPayment > 0)
                {
                    overPayPrepay = true;
                    break;
                }

                if (sl.ProductIsFuel)
                {
                    sale.CouponTotal = sale.CouponTotal + (decimal)(sl.Quantity * sale.Customer.DiscountRate);
                }
            }


            if (overPayPrepay)
            {
                sale.CouponTotal = oldCouponTotal;
                return;
            }

            sale.CouponTotal = decimal.Parse(sale.CouponTotal.ToString("##0.00"));

            if (sale.CouponTotal > 0 && string.IsNullOrEmpty(sale.CouponID))
            {
                sale.CouponID = GetCouponId(sale.TillNumber);
            }
            Performancelog.Debug($"End,SaleManager,ReCompute_Coupon,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to recompute sale totals
        /// </summary>
        /// <param name="sale">Sale </param>
        public void ReCompute_Totals(ref Sale sale)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,ReCompute_Totals,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            Sale_Line SL = default(Sale_Line);
            Sale_Tax STX = default(Sale_Tax);
            bool vHasRebate = false;
            vHasRebate = false;
            var saleTotal = sale.Sale_Totals;
            foreach (Sale_Tax tempLoopVarStx in sale.Sale_Totals.Sale_Taxes)
            {
                STX = tempLoopVarStx;
                STX.Tax_Included_Total = 0;
                STX.Tax_Included_Amount = 0;
                STX.Taxable_Amount = 0;
                STX.Tax_Added_Amount = 0;
                STX.Tax_Rebate = 0; //  
                STX.Rebatable_Amount = 0; //  
                STX.Tax_Exemption_GA_Added = 0; //  
                STX.Tax_Exemption_GA_Incl = 0; //  
            }

            sale.Sale_Line_Disc = 0;
            sale.PointsAmount = 0;
            sale.Sale_Totals.Net = 0;
            sale.Sale_Totals.Charge = 0;
            SetGross(ref saleTotal, 0);
            sale.Sale_Totals.Gross = saleTotal.Gross;
            sale.Sale_Totals.TotalLabel = saleTotal.TotalLabel;
            sale.Sale_Totals.SummaryLabel = saleTotal.SummaryLabel;
            foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
            {
                SL = tempLoopVar_SL;

                if (SL.FuelRebateEligible)
                {
                    vHasRebate = true;
                }

                if (!SL.Prepay)
                // 
                {
                    if (sale.Sale_Totals.Invoice_Discount == 0)
                    {
                        sale.Sale_Totals.Invoice_Discount_Type = "";
                        // SetDiscountAdjust(ref SL, 0);
                    }
                }
                Compute_Charges(ref sale, ref SL, 1);




                //   added Or (SL.IsTaxExemptItem And SL.TaxForTaxExempt) to calculate taxes and correct totals for tax exempt customers with taxes set by the policy
                if (!SL.IsTaxExemptItem || (SL.IsTaxExemptItem && SL.TaxForTaxExempt))
                {

                    Compute_Taxes(ref sale, ref SL, 1);
                }

                // added next 2 lines on Dec 19
                sale.Sale_Line_Disc = sale.Sale_Line_Disc + (decimal)SL.Line_Discount;
                sale.Sale_Totals.Net = sale.Sale_Totals.Net + SL.Net_Amount;
                var saleTotals = sale.Sale_Totals;
                SetGross(ref saleTotals, sale.Sale_Totals.Net);
                sale.Sale_Totals.Gross = saleTotals.Gross;
                sale.Sale_Totals.TotalLabel = saleTotals.TotalLabel;
                sale.Sale_Totals.SummaryLabel = saleTotals.SummaryLabel;
            }

            sale.HasRebateLine = vHasRebate;
            Performancelog.Debug($"End,SaleManager,ReCompute_Totals,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            Chaps_Main.SA = sale;

        }

        /// <summary>
        /// Method to save sale in Db cursale
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="tillNumber"></param>
        public void SaveTemp(ref Sale sale, int tillNumber)
        {
            var dateStart = DateTime.Now;
            
            Performancelog.Debug($"Start,SaleManager,SaveTemp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (sale.LoadingTemp)
            {
                Performancelog.Debug($"End,SaleManager,SaveTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return;
            }

            if (!string.IsNullOrEmpty(sale.TreatyNumber))
            {
                Performancelog.Debug($"End,SaleManager,SaveTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return;
            }
            //TODO: UD Commented for Testing                                                                                                                                                                         
            //_saleService.RemovePreviousTransactionsFromDbTemp(sale.TillNumber, sale.Sale_Num);


            if (sale.Sale_Num == 0)
            {
                Performancelog.Debug($"End,SaleManager,SaveTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return;
            }
            //var till = _tillService.GetTill(tillNumber);
            //_saleService.SaveSaleToDbTemp(tillNumber, till.Shift, sale, _policyManager.TE_Type);
            _saleService.SaveSale(tillNumber, sale.Sale_Num, sale);
            Performancelog.Debug($"End,SaleManager,SaveTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to compute point
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <returns>Points</returns>
        public decimal ComputePoints(Sale sale)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,ComputePoints,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            decimal returnValue = 0;

            decimal Sum_QA = new decimal();
            Sum_QA = 0;
            var SL = new Sale_Line();

            if (sale.USE_LOYALTY && sale.LOYAL_TYPE == "Points" && !string.IsNullOrEmpty(sale.Customer.Loyalty_Code))
            {

                foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                {
                    SL = tempLoopVar_SL;
                    // No points on items excluded from the loyalty points program.
                    if (SL.IncludeInLoyalty)
                    {
                        if (SL.PointsOnVolume)
                        {
                            Sum_QA = Sum_QA + (decimal)SL.Quantity * SL.PointsPerUnit;
                        }
                        else
                        {
                            Sum_QA = Sum_QA + (SL.Amount - (decimal)SL.Line_Discount) * SL.PointsPerDollar;
                        }
                    }
                }
            }
            Sum_QA = Sum_QA - sale.Sale_Totals.Invoice_Discount;
            returnValue = (decimal)modGlobalFunctions.Round((double)Sum_QA, 2);
            Performancelog.Debug($"End,SaleManager,ComputePoints,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            Chaps_Main.SA = sale;
            return returnValue;
        }

        /// <summary>
        /// Method to compute points that cannot be used to buy items in current sale
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <returns>Sub points</returns>
        public decimal SubPoints(Sale sale)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,SubPoints,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            decimal returnValue = 0;
            //  var SL = new Sale_Line();
            var SL = _saleLineManager.CreateNewSaleLine();
            //Smriti move this code to manager
            decimal Sum_NoPoints = new decimal();
            Sum_NoPoints = 0;

            if (sale.USE_LOYALTY && sale.LOYAL_TYPE == "Points" && !string.IsNullOrEmpty(sale.Customer.Loyalty_Code))
            {

                foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                {
                    SL = tempLoopVar_SL;

                    string temp_Policy_Name = "LOY_NOREDPO";
                    if (_policyManager.GetPol(temp_Policy_Name, SL))
                    {
                        Sum_NoPoints = Sum_NoPoints + (SL.Amount - (decimal)SL.Line_Discount);
                    }
                }

                returnValue = (decimal)modGlobalFunctions.Round((double)Sum_NoPoints, 2);

            }
            Performancelog.Debug($"End,SaleManager,SubPoints,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return returnValue;
        }

        /// <summary>
        /// Method to get sub redeemable
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <returns>Sub redeemable</returns>
        public decimal SubRedeemable(Sale sale)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,SubRedeemable,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            decimal returnValue = 0;
            decimal Sum_NoRedeemable;
            decimal Sum_LineAmount = new decimal();
            decimal Sum_Rebate = new decimal();
            Sum_NoRedeemable = 0;
            Sum_LineAmount = 0;
            Sum_Rebate = 0;
            //var SL = new Sale_Line();

            var SL = _saleLineManager.CreateNewSaleLine();
            //    If Policy.REWARDS_Enabled And Len(cc.LoyaltyBalance) > 0 Then
            if (_policyManager.GetPol("REWARDS_Enabled", null))
            {
                if (_policyManager.GetPol("Tax_Rebate", null))
                {
                    foreach (Sale_Tax tempLoopVar_STX in sale.Sale_Totals.Sale_Taxes)
                    {
                        var STX = tempLoopVar_STX;
                        if (STX.Tax_Rebate != 0)
                        {
                            Sum_Rebate = Sum_Rebate + STX.Tax_Rebate;
                        }
                    }
                }

                foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                {
                    SL = tempLoopVar_SL;
                    string temp_Policy_Name = "REWARDS_NoRedmpt";
                    if (!_policyManager.GetPol(temp_Policy_Name, SL))
                    {
                        //                ''  replaced the following line to include tax for eligible amount
                        //                Sum_NoRedeemable = Sum_NoRedeemable + (SL.Amount + SL.AddedTax - SL.Line_Discount)
                        //            Else
                        //                Sum_LineAmount = Sum_LineAmount + SL.Amount + SL.AddedTax
                        Sum_LineAmount = Sum_LineAmount + SL.Amount + SL.AddedTax - (decimal)SL.Line_Discount;
                    }

                }
                //        Sum_LineAmount = Sum_LineAmount - Sum_NoRedeemable - Sum_Rebate
                Sum_LineAmount = Sum_LineAmount - Sum_Rebate;
                returnValue = (decimal)modGlobalFunctions.Round((double)Sum_LineAmount, 2);

                if (returnValue < 0)
                {
                    returnValue = 0;
                }

            }
            Performancelog.Debug($"End,SaleManager,SubRedeemable,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            Chaps_Main.SA = sale;
            return returnValue;
        }

        /// <summary>
        /// Method to save temporary tender
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="sale">Sale</param>
        public void Save_Tender_Temp(ref Tenders tenders, Sale sale)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Save_Tender_Temp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            short nTend = 0;
            short nSeq = 0;
            double dAmount = 0;
            string sCardNumber = "";
            Tender tender_Renamed = default(Tender);
            double T_Used;
            double T_Change;
            string Sale_Type;
            string T_Name = "";
            int GC_Expiry_Days;



            GC_Expiry_Days = Convert.ToInt32(_policyManager.GetPol("GC_EXPIRE", null));

            // Remove any previous references to the transaction.
            _saleService.ClearTenderRecordsFromDbTemp(sale.TillNumber, sale.TillNumber);
            // Open Sales Transactions Recordsets

            //  end

            Sale_Type = sale.Sale_Type;
            nTend = 0;
            if (!(tenders == null))
            {
                var saleTend = new SaleTend();
                foreach (Tender tempLoopVar_tender_Renamed in (IEnumerable)tenders)
                {
                    tender_Renamed = tempLoopVar_tender_Renamed;
                    var creditCard = tender_Renamed.Credit_Card;
                    if (tender_Renamed.Amount_Used != 0 || (tender_Renamed.Credit_Card.Crd_Type == "D" && !string.IsNullOrEmpty(tender_Renamed.Credit_Card.Cardnumber))) //  - When swiping the credit card at debit tender, it is putting a zero record with debit type without any card info'EMVVERSION)
                    {
                        nTend++;
                        // Save the tender in the SALETEND Table
                        saleTend.TillNumber = sale.TillNumber;
                        saleTend.SaleNumber = sale.Sale_Num;

                        saleTend.SequenceNumber = nTend;
                        saleTend.Exchange = (decimal)tender_Renamed.Exchange_Rate;



                        //                        (Policy.COMBINEFLEET And Tender.Tender_Class = "FLEET") then
                        if ((_policyManager.COMBINECR && tender_Renamed.Tender_Class == "CRCARD") || (_policyManager.COMBINEFLEET && tender_Renamed.Tender_Class == "FLEET") || (_policyManager.ThirdParty && tender_Renamed.Tender_Class == "THIRDPARTY"))
                        {

                            //  EMVVERSION-LAST

                            //TODO:
                            if (_policyManager.EMVVersion)
                            {
                                T_Name = tender_Renamed.Tender_Name;
                            }
                            else
                            {
                                //EMVVERSION End
                                if (tender_Renamed.Credit_Card == null)
                                {
                                    T_Name = tender_Renamed.Tender_Name;
                                }
                                else
                                {
                                    T_Name = _creditCardManager.Return_TendDesc(tender_Renamed.Credit_Card);
                                }
                            }
                        }
                        else
                        {
                            // Lines commented below were used for multiple instances, which we don't use anymore
                            //                        If InStr(1, Tender.Tender_Name, " - ") > 0 Then
                            //                            T_Name = Left$(Tender.Tender_Name, InStr(1, Tender.Tender_Name, " - ") - 1)
                            //                        Else
                            //  EMVVERSION-LAST

                            T_Name = tender_Renamed.Tender_Name;
                            //                        End If
                        }
                        saleTend.TenderName = T_Name;
                        saleTend.TenderClass = tender_Renamed.Tender_Class;
                        if (sale.Sale_Totals.Gross < 0)
                        {
                            saleTend.AmountTend = Math.Abs(tender_Renamed.Amount_Entered) * -1;
                            saleTend.AmountUsed = Math.Abs(tender_Renamed.Amount_Used) * -1;
                        }
                        else
                        {
                            saleTend.AmountTend = tender_Renamed.Amount_Entered;
                            saleTend.AmountUsed = tender_Renamed.Amount_Used;
                        }



                        saleTend.SerialNumber = sale.AR_PO;


                        saleTend.Exchange = (decimal)tender_Renamed.Exchange_Rate;



                        //  - Encrypt Card numbers based on Policy + card Setup for Encryption. But for debit and credit cards only look the policy- not the card settings( always consider card setting as true)-(polcy checking is inside the Encryptdecrypt)
                        //                    ![CCard_Num] = EncryptDecrypt.Encrypt(Tender.Credit_Card.CardNumber)
                        if ((tender_Renamed.Credit_Card.Crd_Type == "C" || tender_Renamed.Credit_Card.Crd_Type == "D") && !string.IsNullOrEmpty(tender_Renamed.Credit_Card.Cardnumber))
                        {
                            saleTend.CCardNumber = _encryptManager.Encrypt(tender_Renamed.Credit_Card.Cardnumber, tender_Renamed.Credit_Card.Crd_Type);
                            //  - kickback card
                        }
                        else if (tender_Renamed.Credit_Card.Crd_Type == "K" && tender_Renamed.Tender_Class == "LOYALTY" && !string.IsNullOrEmpty(sale.Customer.PointCardNum))
                        {
                            saleTend.CCardNumber = sale.Customer.PointCardNum;
                            // 
                        }
                        else //Not credit or debit card
                        {
                            if (tender_Renamed.Credit_Card.EncryptCard && !string.IsNullOrEmpty(tender_Renamed.Credit_Card.Cardnumber)) // if card setup is to encrypt, send to encrypt- based on policy encrypt it or not
                            {
                                saleTend.CCardNumber = _encryptManager.Encrypt(tender_Renamed.Credit_Card.Cardnumber, "");
                            }
                            else // if card setup is not to encrypt, no need to check the policy. keep it as it is
                            {
                                saleTend.CCardNumber = tender_Renamed.Credit_Card.Cardnumber;
                            }
                        }
                        // 


                        saleTend.CCardAPRV = tender_Renamed.Credit_Card.Authorization_Number;

                        saleTend.AuthUser = tender_Renamed.AuthUser;
                        var dataSource = DataSource.CSCCurSale;
                        _saleService.AddSaleTend(saleTend, dataSource);
                        // S_Tend.Update();

                        // Save credit card information
                        if (!string.IsNullOrEmpty(tender_Renamed.Credit_Card.Cardnumber))
                        {
                            var cardTender = new CardTender();
                            //S_Card.AddNew();
                            cardTender.TillNumber = sale.TillNumber;
                            cardTender.SaleNumber = sale.Sale_Num;

                            //  added following condition to correct the tender info.
                            if (tender_Renamed.Tender_Code == "ACKG" || tender_Renamed.Tender_Code == "ACK")
                            {
                                cardTender.TenderName = tender_Renamed.Tender_Name;
                                cardTender.CardName = tender_Renamed.Tender_Name;
                                if (tender_Renamed.Tender_Code == "ACKG")
                                {
                                    cardTender.CardType = "G";
                                    cardTender.AllowMulticard = 2;
                                }
                                else
                                {
                                    cardTender.CardType = "L";
                                    cardTender.AllowMulticard = 1;
                                }
                            }
                            else
                            {
                                cardTender.TenderName = T_Name;
                                cardTender.CardName = tender_Renamed.Credit_Card.Name;
                                cardTender.CardType = tender_Renamed.Credit_Card.Crd_Type;
                            }
                            //  end




                            //  - Encrypt Card numbers based on Policy + card Setup for Encryption. But for debit and credit cards only look the policy- not the card settings( always consider card setting as true)-(polcy checking is inside the Encryptdecrypt)
                            //               ![Card_Number] = EncryptDecrypt.Encrypt(Tender.Credit_Card.CardNumber)
                            if (tender_Renamed.Credit_Card.Crd_Type == "C" || tender_Renamed.Credit_Card.Crd_Type == "D")
                            {
                                cardTender.CardNum = _encryptManager.Encrypt(tender_Renamed.Credit_Card.Cardnumber, tender_Renamed.Credit_Card.Crd_Type);
                            }
                            else //Not credit or debit card
                            {
                                if (tender_Renamed.Credit_Card.EncryptCard) // if card setup is to encrypt, send to encrypt- based on policy encrypt it or not
                                {
                                    cardTender.CardNum = _encryptManager.Encrypt(tender_Renamed.Credit_Card.Cardnumber, "");
                                }
                                else // if card setup is not to encrypt, no need to check the policy. keep it as it is
                                {
                                    //Ackroo
                                    if (tender_Renamed.Tender_Code == "ACK")
                                    {
                                        cardTender.CardNum = sCardNumber;
                                    }
                                    else
                                    {
                                        cardTender.CardNum = tender_Renamed.Credit_Card.Cardnumber;
                                    }
                                }
                            }
                            // 

                            cardTender.ExpiryDate = tender_Renamed.Credit_Card.Expiry_Date;
                            cardTender.Swiped = tender_Renamed.Credit_Card.Card_Swiped;
                            cardTender.StoreForward = tender_Renamed.Credit_Card.StoreAndForward;
                            if (tender_Renamed.Tender_Code == "ACKG" || tender_Renamed.Tender_Code == "ACK")
                            {
                                if (tender_Renamed.Tender_Code == "ACKG")
                                {
                                    cardTender.Amount = (decimal)(tender_Renamed.Credit_Card.Trans_Amount - dAmount);
                                }
                                else
                                {
                                    cardTender.Amount = tender_Renamed.Amount_Entered;
                                }
                            }
                            else
                            {
                                cardTender.Amount = (decimal)tender_Renamed.Credit_Card.Trans_Amount; //Tender.Amount_Used
                            }
                            cardTender.ApprovalCode = tender_Renamed.Credit_Card.Authorization_Number;
                            cardTender.SequenceNumber = tender_Renamed.Credit_Card.Sequence_Number;
                            cardTender.DeclineReason = tender_Renamed.Credit_Card.Decline_Message;
                            cardTender.Result = tender_Renamed.Credit_Card.Result;
                            cardTender.TerminalID = tender_Renamed.Credit_Card.TerminalID;
                            cardTender.DebitAccount = !string.IsNullOrEmpty(tender_Renamed.Credit_Card.DebitAccount) ? tender_Renamed.Credit_Card.DebitAccount.Substring(0, 1) : null;
                            cardTender.ResponseCode = tender_Renamed.Credit_Card.ResponseCode;
                            cardTender.ISOCode = tender_Renamed.Credit_Card.ApprovalCode;
                            cardTender.TransactionDate = tender_Renamed.Credit_Card.Trans_Date;
                            cardTender.TransactionTime = tender_Renamed.Credit_Card.Trans_Time;
                            cardTender.ReceiptDisplay = tender_Renamed.Credit_Card.Receipt_Display;
                            cardTender.Language = _creditCardManager.Language(ref creditCard);
                            cardTender.CustomerName = tender_Renamed.Credit_Card.Customer_Name;
                            cardTender.CallTheBank = _creditCardManager.Call_The_Bank(ref creditCard);
                            // 
                            cardTender.VechicleNo = tender_Renamed.Credit_Card.Vechicle_Number;
                            cardTender.DriverNo = tender_Renamed.Credit_Card.Driver_Number;
                            cardTender.IdentificationNo = tender_Renamed.Credit_Card.ID_Number;
                            cardTender.Odometer = tender_Renamed.Credit_Card.Odometer_Number;
                            cardTender.CardUsage = tender_Renamed.Credit_Card.usageType;


                            if (!_policyManager.USE_PINPAD) // donot print the information entered through a pin pad/pump- print only the one entered throug pos
                            {

                                cardTender.PrintVechicleNo = tender_Renamed.Credit_Card.Print_VechicleNo;
                                cardTender.PrintDriverNo = tender_Renamed.Credit_Card.Print_DriverNo;
                                cardTender.PrintIdentificationNo = tender_Renamed.Credit_Card.Print_IdentificationNo;
                            }
                            cardTender.PrintUsage = tender_Renamed.Credit_Card.Print_Usage;
                            cardTender.PrintOdometer = tender_Renamed.Credit_Card.Print_Odometer;
                            //shiny end
                            cardTender.Balance = tender_Renamed.Credit_Card.Balance;
                            cardTender.Quantity = tender_Renamed.Credit_Card.Quantity;


                            //cardTender.Message = !string.IsNullOrEmpty(tender_Renamed.Credit_Card.Report) ? tender_Renamed.Credit_Card.Report.Substring(0, 2500) : "";

                            cardTender.CardProfileID = tender_Renamed.Credit_Card.CardProfileID; //  
                            cardTender.PONumber = tender_Renamed.Credit_Card.PONumber; //  - it is for each cardnumber- that whay keeping as part of card
                            cardTender.Sequence = nSeq + 1;
                            cardTender.Message = tender_Renamed.Credit_Card.Report;
                            _saleService.AddCardTender(cardTender, dataSource);
                            //S_Card.Update();
                        }
                    }
                }

                //TODO :
                tender_Renamed = null;
                // T_Used = System.Convert.ToDouble(Tenders_Renamed.Tend_Totals.Tend_Used);

                CacheManager.AddTendersForSale(sale.Sale_Num, sale.TillNumber, tenders);
                UpdatePaidByCardForTempLines(sale);


            }
            else
            {
                nTend = 0;
                T_Used = 0;
                T_Change = 0;
            }

            Performancelog.Debug($"End,SaleManager,Save_Tender_Temp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to recompute cash bonus
        /// </summary>
        /// <param name="sale">Sale</param>
        public void ReCompute_CashBonus(ref Sale sale)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,ReCompute_CashBonus,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var till = _tillService.GetTill(sale.TillNumber);

            Sale_Line SL = default(Sale_Line);
            decimal OldCBonusTotal;
            bool OverPayPrepay = false;
            decimal slCashBonus = new decimal();

            if (string.IsNullOrEmpty(sale.Customer.GroupID) || sale.Customer.DiscountType != "B" || !_policyManager.GetPol("CashBonus", null))
            {
                sale.CBonusTotal = 0;
            }
            else
            {
                OldCBonusTotal = sale.CBonusTotal;

                OverPayPrepay = false;
                sale.CBonusTotal = 0;
                foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                {
                    SL = tempLoopVar_SL;

                    if (SL.Prepay && sale.OverPayment > 0)
                    {
                        OverPayPrepay = true;
                        break;
                    }

                    if (SL.ProductIsFuel && SL.Stock_Code != _policyManager.GetPol("EXC_CASHBONUS", null)) // based)
                    {
                        ErrorMessage er;
                        //  Since Cashbonus Range is entered in +ve qty when caculating we need to use the abs(sl.quantity).
                      //  slCashBonus = System.Convert.ToDecimal(_cashBonusManager.CalculateCashBonus(sale.Customer.GroupID, System.Math.Abs((short)SL.Quantity),out er));

                        //Uncomment this line to activate cashbonus
                        //sale.CBonusTotal = Convert.ToDecimal(sale.CBonusTotal + (SL.Quantity > 0 ? slCashBonus : -1 * slCashBonus)); // After calculation making it +ve, or -ve based on rela salequantity
                        slCashBonus = 0;
                    }
                }


                if (OverPayPrepay)
                {
                    sale.CBonusTotal = OldCBonusTotal;
                    return;
                }
            }
            //    mvarCBonusTotal = Format(mvarCBonusTotal, "##0.00")
            sale.CBonusTotal = decimal.Parse(sale.CBonusTotal.ToString("##0.00"));
            decimal vData = -1 * sale.CBonusTotal;

            till.CashBonus = till.CashBonus + vData;
            _tillService.UpdateTill(till);
            Chaps_Main.SA = sale;
            Performancelog.Debug($"End,SaleManager,ReCompute_CashBonus,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        //End - SV

        // This function is called only for type "F" cards, so no Type field is required here
        /// <summary>
        /// Method to save profile promt
        /// </summary>
        /// <param name="Cprompts">card prompts</param>
        /// <param name="CardNum">Card number</param>
        /// <param name="ProfileID">Profile Id</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        public void Save_ProfilePrompt_Temp(CardPrompts Cprompts, string CardNum, string ProfileID, int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Save_ProfilePrompt_Temp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            CardPrompt prm = default(CardPrompt);
            short i = 0;

            var prompt = new CardProfilePrompt();
            for (i = 1; i <= Cprompts.Count; i++)
            {
                if (!string.IsNullOrEmpty(Cprompts[i].PromptAnswer))
                {
                    prm = Cprompts[i];
                    prompt = new CardProfilePrompt();
                    //sPrompt.AddNew();
                    prompt.SaleNumber = saleNumber;
                    prompt.TillNumber = tillNumber;
                    prompt.CardNumber = CardNum;
                    prompt.ProfileID = ProfileID;
                    prompt.PromptID = prm.PromptID;
                    prompt.PromptAnswer = prm.PromptAnswer;
                    _saleService.AddCardProfilePromptToDbTemp(prompt);
                    //sPrompt.Update();
                }
            }

            Performancelog.Debug($"End,SaleManager,Save_ProfilePrompt_Temp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to remmove a sale line item
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="tillNumber">TIll number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="error">Error message</param>
        /// <param name="adjust">Adjustment required or not</param>
        /// <param name="makePromo">make promotional items or not</param>
        /// <returns>Sale</returns>
        public Sale RemoveSaleLineItem(string userCode, int tillNumber, int saleNumber, int lineNumber,
            out ErrorMessage error, bool adjust, bool makePromo)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,RemoveSaleLineItem,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!CanUserRemoveLineFromSale(userCode, out error))
            {
                return null;
            }

            if (!ValidateTillAndSale(tillNumber, saleNumber, out error))
            {
                return null;
            }

            var sale = GetCurrentSale(saleNumber, tillNumber, 1, userCode, out error);//.GetSaleBySaleNoFromDbTemp(tillNumber, saleNumber);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return null;
            }


            // var saleLine = _saleService.GetSaleLineFromDbTemp(saleNumber, tillNumber, lineNumber, userCode);
            Sale_Line saleLine;
            if (lineNumber <= sale.Sale_Lines.Count && lineNumber != 0)
            {
                saleLine = sale.Sale_Lines[lineNumber];
            }
            else
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Please select a valid SaleLine to remove from Sale",
                        MessageType = (int)MessageType.Information + MessageType.OkOnly
                    },
                    StatusCode = HttpStatusCode.NotFound
                };
                return null;
            }

            RemoveFuelItem(saleLine, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return null;
            }

            if (adjust)
            {
                saleLine.Sale_Num = sale.Sale_Num.ToString();
                Save_Deleted_Line(ref saleLine, "D");
                _saleLineManager.SetQuantity(ref saleLine, Convert.ToSingle(-saleLine.Quantity));
                Adjust_Lines(ref saleLine, sale, false, true);

                // Remove the line if it is still in the sale.
                if (sale.Sale_Lines.Count >= lineNumber)
                {
                    // Price type "X" or "I" items have the number of lines required managed by
                    // the Adjust_Lines routine so we don't need to remove the line for those price types.
                    if (saleLine.Price_Number > 1 || (saleLine.Price_Type != 'X' && saleLine.Price_Type != 'I'))
                    {
                        if ((saleLine.Price_Type == 'R' && string.IsNullOrEmpty(saleLine.PromoID)) || saleLine.ProductIsFuel || (saleLine.Price_Type == 'S') || (saleLine.Price_Type == 'F'))
                        {
                            sale.Sale_Lines.Remove(lineNumber);
                        }
                    }
                }
            }
            else
            {
                sale.Sale_Lines.Remove(lineNumber);
            }


            Security securityRenamed = _policyManager.LoadSecurityInfo();
            if (adjust && (Strings.UCase(Convert.ToString(securityRenamed.BackOfficeVersion)) == "FULL" || _policyManager.PROMO_SALE))
            {
                //  
                if (saleLine.Price_Type == 'R' && !string.IsNullOrEmpty(saleLine.PromoID))
                {
                    if (!sale.LoadingTemp)
                    {
                        SaveTemp(ref sale, tillNumber);
                    }
                    var processedPromoId = saleLine.PromoID;
                    foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                    {
                        var SL = tempLoopVar_SL;
                        if (!string.IsNullOrEmpty(processedPromoId) && makePromo && !sale.MakingPromo && SL.PromoID == processedPromoId)
                        {
                            SL.NoPromo = false; // Added on September 24, 2008
                            _saleLineManager.Make_Promo(ref sale, ref SL, true);
                            if (Adjust_Lines(ref SL, sale, false, false))
                            {
                            }
                            break;
                        }
                    }

                    // Added on September 24, 2008 to refresh the prices for all the lines
                    foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                    {
                        var SL = tempLoopVar_SL;
                        if (!string.IsNullOrEmpty(processedPromoId) && makePromo && !sale.MakingPromo && SL.PromoID == processedPromoId)
                        {
                            if (Adjust_Lines(ref SL, sale, false, false))
                            {
                            }
                        }
                        if (!string.IsNullOrEmpty(processedPromoId) && SL.Stock_Code == saleLine.Stock_Code && SL.Price_Type == 'R' && SL.SP_Prices.Count == 0)
                        {
                            _saleLineManager.SetPrice(ref SL, SL.Regular_Price);
                        }
                    }
                    // End September 24, 2008
                }

            }

            ReCompute_Totals(ref sale);
            if (sale.Sale_Totals.Invoice_Discount != 0)
            {
                Sale_Discount(ref sale, sale.Sale_Totals.Invoice_Discount, sale.Sale_Totals.Invoice_Discount_Type);
            }
            Sale_Line temp;
            if (!sale.LoadingTemp)
            {
                var saleLines = sale.Sale_Lines;
                short no = 1;
                sale.Sale_Lines = null;
                foreach (Sale_Line line in saleLines)
                {
                    temp = line;
                    sale.Sale_Lines.AddLine(no, temp, no.ToString());
                    no++;
                }
                SaveTemp(ref sale, tillNumber);
            }

            ////TODO:Check for error in this case 
            //error = null;
            //var updatedSaleLine = _saleService.GetSaleLinesFromDbTemp(saleNumber, tillNumber, userCode);
            //if (updatedSaleLine != null)
            //{
            //    sale.Sale_Lines = null;
            //    foreach (var tempSaleLine in updatedSaleLine)
            //    {
            //        sale.Sale_Lines.AddLine(tempSaleLine.Line_Num, tempSaleLine, tempSaleLine.Line_Num.ToString());
            //    }
            //}
            Performancelog.Debug($"End,SaleManager,RemoveSaleLineItem,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            //Update Sale object in Cache
            CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);
            Register register = new Register();
            _mainManager.SetRegister(ref register, sale.Register);
            // Nov 18, 2008 Nicolette added to send to customer display
            if (register.Customer_Display)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                sale.CustomerDisplay = _mainManager.DisplayMsgLcd(register, Convert.ToString(
                    _mainManager.FormatLcdString(register, saleLine.Description.Trim(),
                    saleLine.Quantity >= 0 ? saleLine.price.ToString() : "-" + saleLine.price.ToString()
                    )), _mainManager.FormatLcdString(register, "" + _resourceManager.GetResString(offSet, 8326), sale.Sale_Totals.Gross.ToString("0.00")));
            }
            Chaps_Main.SA = sale;
            // Nov 18, 2008 Nicolette end
            return sale;
        }

        /// <summary>
        /// Method to adjust lines
        /// </summary>
        /// <param name="This_Line">Sale line</param>
        /// <param name="sale">Sale</param>
        /// <param name="NewLine">New line required or not</param>
        /// <param name="Remove">Remove items or not</param>
        /// <returns>True or false</returns>
        public bool Adjust_Lines(ref Sale_Line This_Line, Sale sale, bool NewLine,
            bool Remove = false)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Adjust_Lines,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            bool returnValue = false;
            //// Get the pricing for that many products
            float[,] Spr = null;
            float Sale_Qty = 0;
            short Sale_Lin = 0;
            Sale_Line SS = default(Sale_Line);
            Sale_Line SLine = default(Sale_Line);
            Sale_Line New_Line = default(Sale_Line);
            short n = 0;
            short m;
            short nDel = 0;
            bool Fd;
            bool Can_Combine = false;
            bool Combined = false;
            short Lines_Needed = 0;
            short ns = 0;
            short nL;
            bool Line_Found;

            returnValue = true;

            // If the incoming line is New then include it's quantity in the count. If it isn't
            // then the quantity will be included in the following loop.
            Sale_Qty = Convert.ToSingle(NewLine ? This_Line.Quantity : 0);
            if (Remove)
            {
                Sale_Qty = Convert.ToSingle(-This_Line.Quantity);
            }
            Sale_Lin = Convert.ToInt16(NewLine ? 1 : 0);

            // Compute the total quantity of the item in the sale and the number of lines
            // on which it appears.
            // September 23, 2008 Nicolette added the PromoID conditions to handle same item in multiple promos
            if (string.IsNullOrEmpty(This_Line.PromoID))
            {
                foreach (Sale_Line tempLoopVar_SLine in sale.Sale_Lines)
                {
                    SLine = tempLoopVar_SLine;
                    if (This_Line.Stock_Code == SLine.Stock_Code)
                    {
                        Sale_Qty = Sale_Qty + SLine.Quantity;
                        Sale_Lin++;
                    }
                }
            }
            else // for items in a promo, summarize only the items with no promo or the same PromoID
            {
                foreach (Sale_Line tempLoopVar_SLine in sale.Sale_Lines)
                {
                    SLine = tempLoopVar_SLine;
                    if (This_Line.Stock_Code == SLine.Stock_Code &&
                        (string.IsNullOrEmpty(SLine.PromoID) || SLine.PromoID == This_Line.PromoID))
                    {
                        Sale_Qty = Sale_Qty + SLine.Quantity;
                        Sale_Lin++;
                    }
                }
            }
            // End September 23, 2008 Nicolette

            ns = Sale_Qty < 0 ? (short)-1 : (short)1;

            // Call 'PQuantity' to build the array of prices and quantities to be used for this item.
            Spr = PQuantity(This_Line, sale, Sale_Qty);
            Sale_Qty = Math.Abs((short)Sale_Qty);

            if (This_Line.Price_Number > 1)
            {
                // Price_Number > 1 is NOT regular price.
                if (NewLine)
                {
                    foreach (Sale_Line tempLoopVar_SS in sale.Sale_Lines)
                    {
                        SS = tempLoopVar_SS;
                        if (SS.Stock_Code == This_Line.Stock_Code)
                        {



                            //(Not This_Line.Gift_Certificate) And _
                            //SS.price = This_Line.price And _
                            //SS.Quantity > 0 And This_Line.Quantity > 0 And _
                            //SS.Discount_Rate = This_Line.Discount_Rate And _
                            //SS.Serial_No = "" And This_Line.Serial_No = "" And _
                            //SS.Discount_Type = This_Line.Discount_Type And _
                            //Not This_Line.ProductIsFuel And Not This_Line.Stock_BY_Weight '  


                            // Added ezipin condition
                            // TODO: Ezipin_Removed
                            string temp_Policy_Name = "COMBINE_LINE";
                            Can_Combine = _policyManager.GetPol(temp_Policy_Name, This_Line) &&
                                          !This_Line.Gift_Certificate &&
                                          SS.price == This_Line.price & SS.Quantity > 0 & This_Line.Quantity > 0 &
                                          SS.Discount_Rate == This_Line.Discount_Rate && string.IsNullOrEmpty(SS.Serial_No) &&
                                          string.IsNullOrEmpty(This_Line.Serial_No) && SS.Discount_Type == This_Line.Discount_Type &&
                                          !This_Line.ProductIsFuel && !This_Line.Stock_BY_Weight;
                            //  

                            if (Can_Combine)
                            {
                                _saleLineManager.SetQuantity(ref SS, (SS.Quantity + This_Line.Quantity));
                                _saleLineManager.SetAmount(ref SS, ((decimal)(SS.Quantity * SS.price)));
                                SS.SendToLCD = true;
                                //   If combine policy - adding same product is not sending info to customer display

                                sale.Sale_Lines.LastItemID = SS.Line_Num;

                                returnValue = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    _saleLineManager.SetAmount(ref This_Line, (decimal)(This_Line.Quantity * This_Line.price));
                }
            }
            else
            {
                // Price_Code = 1 means that this is based on regular price.
                if (This_Line.Price_Type == 'R' && string.IsNullOrEmpty(This_Line.PromoID)) //  
                {
                    if (NewLine)
                    {
                        foreach (Sale_Line tempLoopVar_SS in sale.Sale_Lines)
                        {
                            SS = tempLoopVar_SS;
                            if (SS.Stock_Code == This_Line.Stock_Code)
                            {

                                string temp_Policy_Name2 = "COMBINE_LINE";
                                Can_Combine = _policyManager.GetPol(temp_Policy_Name2, This_Line) &&
                                              !This_Line.Gift_Certificate &&
                                              SS.price == This_Line.price & SS.Quantity > 0 & This_Line.Quantity > 0 &
                                              SS.Discount_Rate == This_Line.Discount_Rate && string.IsNullOrEmpty(SS.Serial_No) &&
                                              string.IsNullOrEmpty(This_Line.Serial_No) && SS.Discount_Type == This_Line.Discount_Type &&
                                              !This_Line.ProductIsFuel && !This_Line.Stock_BY_Weight;
                                //

                                if (Can_Combine)
                                {
                                    //SS.Quantity = SS.Quantity + This_Line.Quantity;
                                    _saleLineManager.SetQuantity(ref SS, SS.Quantity + This_Line.Quantity);
                                    // SS.Amount = (decimal)(SS.Quantity * SS.price);
                                    _saleLineManager.SetAmount(ref SS, (decimal)(SS.Quantity * SS.price));
                                    SS.SendToLCD = true;
                                    //   If combine policy - adding same product is not sending info to customer display

                                    sale.Sale_Lines.LastItemID = SS.Line_Num;

                                    returnValue = false;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        //TODO: Ezipin_Removed
                        // combined
                        // this code handles the quantity changes and item added from HotButtons with qty>1; for other cases the
                        // items are added one by one, so the above code handle this situation
                        // the code assumes that quantity change is always done from 1 to a higher quantity
                        // it does not handle negative quantity
                        //  Need to split carwash products as well

                        //TODO :Remove EZI pin and Car Wash Removed 
                        var error = new ErrorMessage();
                        //if (This_Line.Quantity > 1 && This_Line.Quantity > 1)
                        //{
                        //    Lines_Needed = (short)This_Line.Quantity;
                        //    if (Lines_Needed > 1)
                        //    {
                        //        returnValue = false;
                        //        for (n = 1; n <= Lines_Needed - 1; n++)
                        //        {
                        //            nDel++;
                        //            // New_Line = new Sale_Line();
                        //            New_Line = _saleLineManager.CreateNewSaleLine();
                        //            New_Line.User = This_Line.User;
                        //            New_Line.NoPromo = true;
                        //            _saleLineManager.SetPluCode(ref New_Line, This_Line.PLU_Code, out error); ;
                        //            New_Line.Discount_Type = This_Line.Discount_Type;
                        //            _saleLineManager.SetDiscountRate(ref New_Line, This_Line.Discount_Rate);
                        //            New_Line.Line_Num = (short)(sale.Sale_Lines.Count + 1);
                        //            _saleLineManager.SetPrice(ref New_Line, This_Line.price);
                        //            _saleLineManager.SetQuantity(ref New_Line, 1);
                        //            New_Line.SendToLCD = true;
                        //            sale.GoLastLine = true;
                        //            Add_a_Line(ref sale, New_Line, New_Line.User, sale.TillNumber, false);
                        //            _saleLineManager.SetQuantity(ref This_Line, 1);
                        //            _saleLineManager.SetAmount(ref This_Line, ((decimal)(This_Line.Quantity * This_Line.price)));
                        //        }
                        //    }
                        //    New_Line = null;
                        //}
                        //else
                        //{
                        //   end
                        _saleLineManager.SetAmount(ref This_Line, (decimal)(This_Line.Quantity * This_Line.price));
                        //}
                    }

                }
                else if (This_Line.Price_Type == 'F')
                {
                    // "F" - First Unit Pricing
                    if (NewLine)
                    {
                        Combined = false;
                        foreach (Sale_Line tempLoopVar_SS in sale.Sale_Lines)
                        {
                            SS = tempLoopVar_SS;
                            // Assign the computed prices to each item in the sale.
                            if (SS.Stock_Code == This_Line.Stock_Code)
                            {
                                Line_Price(ref sale, ref SS, (double)Convert.ToDecimal(Spr[1, 2]));
                                _saleLineManager.SetAmount(ref SS, (decimal)(SS.Quantity * SS.price));
                            }
                        }

                        foreach (Sale_Line tempLoopVar_SS in sale.Sale_Lines)
                        {
                            SS = tempLoopVar_SS;

                            if (SS.Stock_Code == This_Line.Stock_Code)
                            {



                                string temp_Policy_Name3 = "COMBINE_LINE";
                                Can_Combine = _policyManager.GetPol(temp_Policy_Name3, This_Line) &&
                                              !This_Line.Gift_Certificate &&
                                              (decimal)SS.price == Convert.ToDecimal(Spr[1, 2]) &&
                                              SS.Quantity > 0 & This_Line.Quantity > 0 &
                                              SS.Discount_Rate == This_Line.Discount_Rate && string.IsNullOrEmpty(SS.Serial_No) &&
                                              string.IsNullOrEmpty(This_Line.Serial_No) && SS.Discount_Type == This_Line.Discount_Type &&
                                              !This_Line.ProductIsFuel && !This_Line.Stock_BY_Weight;

                                //

                                Line_Price(ref sale, ref SS, (double)Convert.ToDecimal(Spr[1, 2]));
                                if (Can_Combine && !Combined)
                                {
                                    Line_Quantity(ref sale, ref SS, SS.Quantity + This_Line.Quantity, false);
                                    Combined = true;
                                }

                                if (Can_Combine)
                                {
                                    sale.Sale_Lines.LastItemID = SS.Line_Num;
                                }

                                _saleLineManager.SetAmount(ref SS, (decimal)(SS.Quantity * SS.price));
                            }
                        }

                        if (Combined)
                        {
                            returnValue = false;
                        }
                        else
                        {
                            _saleLineManager.SetPrice(ref This_Line, (double)Convert.ToDecimal(Spr[1, 2]));
                            _saleLineManager.SetAmount(ref This_Line, (decimal)(This_Line.Quantity * This_Line.price));
                        }
                    }
                    else
                    {
                        if (This_Line.Price_Number == 1)
                        {
                            _saleLineManager.SetPrice(ref This_Line, (double)Convert.ToDecimal(Spr[1, 2]));
                        }
                        _saleLineManager.SetAmount(ref This_Line, (decimal)(This_Line.Quantity * This_Line.price));
                    }


                }
                else if (This_Line.Price_Type == 'S')
                {
                    // "S" - Sale Pricing
                    if (NewLine)
                    {
                        Combined = false;
                        foreach (Sale_Line tempLoopVar_SS in sale.Sale_Lines)
                        {
                            SS = tempLoopVar_SS;

                            if (SS.Stock_Code == This_Line.Stock_Code)
                            {



                                string temp_Policy_Name4 = "COMBINE_LINE";
                                Can_Combine = _policyManager.GetPol(temp_Policy_Name4, This_Line) &&
                                              !This_Line.Gift_Certificate &&
                                              (decimal)SS.price == Convert.ToDecimal(Spr[1, 2]) &&
                                              SS.Quantity > 0 & This_Line.Quantity > 0 &
                                              SS.Discount_Rate == This_Line.Discount_Rate && string.IsNullOrEmpty(SS.Serial_No) &&
                                              string.IsNullOrEmpty(This_Line.Serial_No) && SS.Discount_Type == This_Line.Discount_Type &&
                                              !This_Line.ProductIsFuel && !This_Line.Stock_BY_Weight;
                                //


                                if (Can_Combine)
                                {
                                    sale.Sale_Lines.LastItemID = SS.Line_Num;
                                }

                                _saleLineManager.SetAmount(ref SS, (decimal)(SS.Quantity * SS.price));
                            }
                        }

                        if (Combined)
                        {
                            returnValue = false;
                        }
                        else
                        {
                            _saleLineManager.SetPrice(ref This_Line, (double)Convert.ToDecimal(Spr[1, 2]));
                            _saleLineManager.SetAmount(ref This_Line, (decimal)(This_Line.Quantity * This_Line.price));
                        }
                    }
                    else
                    {
                        if (This_Line.Price_Number == 1)
                        {
                            _saleLineManager.SetPrice(ref This_Line, (double)Convert.ToDecimal(Spr[1, 2]));
                        }
                        _saleLineManager.SetAmount(ref This_Line, (decimal)(This_Line.Quantity * This_Line.price));
                    }

                    ////        ElseIf This_Line.Price_Type = "X" Or This_Line.Price_Type = "I" Then
                }
                else if (This_Line.Price_Type == 'X' || This_Line.Price_Type == 'I' ||
                         (This_Line.Price_Type == 'R' && !string.IsNullOrEmpty(This_Line.PromoID) && !This_Line.ProductIsFuel))
                //  
                {
                    // "X" - X for Pricing;      "I" - Incremental Pricing

                    // Compute how many lines are needed by counting the number of prices
                    // that the 'PQuantity' routine set.
                    Lines_Needed = 0;
                    for (n = 1; n <= Spr.GetLength(0) - 1; n++)
                    {
                        if (Spr[n, 1] != 0)
                        {
                            Lines_Needed++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    nDel = 0;

                    // Remove Lines if there are more than we need ...
                    if (Lines_Needed < Sale_Lin)
                    {
                        returnValue = false;
                        if (NewLine)
                        {
                            // Don't add the incoming line. That reduces the number we need to
                            // delete by 1.
                            nDel = 1;
                        }
                        if (nDel < Sale_Lin - Lines_Needed)
                        {
                            foreach (Sale_Line tempLoopVar_SS in sale.Sale_Lines)
                            {
                                SS = tempLoopVar_SS;
                                if (SS.Stock_Code == This_Line.Stock_Code)
                                {
                                    SS.ForPromo = true;
                                    var error = new ErrorMessage();
                                    //sale.Sale_Lines.Remove(lineNumber);
                                    RemoveSaleLineItem(SS.User, sale.TillNumber, sale.Sale_Num, SS.Line_Num, out error, false, false); // added False on July 25, 2008
                                    nDel++;
                                    if (nDel == Sale_Lin - Lines_Needed)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        // Add lines if there are not enough.
                    }
                    else if (Lines_Needed > Sale_Lin)
                    {
                        returnValue = false;
                        for (n = Sale_Lin; n <= Lines_Needed - 1; n++)
                        {
                            nDel++;
                            //New_Line = new Sale_Line();
                            ErrorMessage error = new ErrorMessage();
                            New_Line = _saleLineManager.CreateNewSaleLine();
                            New_Line.User = This_Line.User;
                            New_Line.NoPromo = true; //  
                            _saleLineManager.SetPluCode(ref sale, ref New_Line, This_Line.PLU_Code, out error);
                            New_Line.Discount_Type = This_Line.Discount_Type;
                            _saleLineManager.SetDiscountRate(ref New_Line, This_Line.Discount_Rate);
                            New_Line.Line_Num = (short)(sale.Sale_Lines.Count + 1);
                            New_Line.SendToLCD = true; //  
                                                       //  
                            if (This_Line.Price_Type == 'R' && !string.IsNullOrEmpty(This_Line.PromoID) && !This_Line.ProductIsFuel)
                            {
                                New_Line.ForPromo = true;
                                New_Line.NoPriceFormat = This_Line.NoPriceFormat;
                                sale.GoLastLine = true; //  
                            }
                            // End  
                            Add_a_Line(ref sale, New_Line, New_Line.User, sale.TillNumber, out error, false);
                        }
                    }
                    New_Line = null;

                    // Set the pricing on each line
                    n = 0;
                    foreach (Sale_Line tempLoopVar_SS in sale.Sale_Lines)
                    {
                        SS = tempLoopVar_SS;
                        if (SS.Stock_Code == This_Line.Stock_Code &&
                            (string.IsNullOrEmpty(SS.PromoID) || SS.PromoID == This_Line.PromoID))
                        {
                            n++;
                            Line_Price(ref sale, ref SS, (double)Convert.ToDecimal(Spr[n, 2]));
                            Line_Quantity(ref sale, ref SS,
                                Convert.ToInt16(Spr[n, 1]) *
                                (This_Line.Price_Type == 'R' && string.IsNullOrEmpty(This_Line.PromoID) ? ns : 1), false);
                            _saleLineManager.SetAmount(ref SS, Convert.ToDecimal(Spr[n, 3]));
                            Line_Discount_Type(ref SS, SS.Discount_Type);
                            Line_Discount_Rate(ref sale, ref SS, SS.Discount_Rate);
                            Sale_Qty = Sale_Qty - Math.Abs(Convert.ToInt16(Spr[n, 1]));
                            SS.SendToLCD = true; //  
                            if (This_Line.NoPriceFormat)
                            {
                                SS.NoPriceFormat = true;
                            }
                        }
                    }

                    // Set the quantity on the new line. These should be extra lines with regular price
                    if (Sale_Qty != 0)
                    {
                        n++;
                        _saleLineManager.SetQuantity(ref This_Line, Sale_Qty);
                        if (n <= Spr.Length - 1)
                        {
                            _saleLineManager.SetPrice(ref This_Line, (double)Convert.ToDecimal(Spr[n, 2]));
                            _saleLineManager.SetAmount(ref This_Line, Convert.ToDecimal(Spr[n, 3]));
                        }
                        else
                        {
                            _saleLineManager.SetPrice(ref This_Line, This_Line.Regular_Price);
                            _saleLineManager.SetAmount(ref This_Line, (decimal)(This_Line.price * This_Line.Quantity));
                        }
                        //                This_Line.SendToLCD = False    ' initialize
                        returnValue = true;
                    }
                    else
                    {
                        returnValue = false;
                    }


                    //        // HOLD
                    //        ///            If (This_Line.isCarwashProduct And This_Line.Quantity > 1 And This_Line.PromoID <> "") Then
                    //        ///                Lines_Needed = This_Line.Quantity
                    //        ///                Adjust_Lines = False
                    //        ///                For n = 1 To Lines_Needed - 1
                    //        ///                    nDel = nDel + 1
                    //        ///                    Set New_Line = New Sale_Line
                    //        ///                    New_Line.NoPromo = True
                    //        ///                    New_Line.isEziProduct = True
                    //        ///                    New_Line.PLU_Code = This_Line.PLU_Code
                    //        ///                    New_Line.Discount_Type = This_Line.Discount_Type
                    //        ///                    New_Line.Discount_Rate = This_Line.Discount_Rate
                    //        ///                    New_Line.Line_Num = Me.Sale_Lines.Count + 1
                    //        ///                    New_Line.Quantity = 1
                    //        ///                    New_Line.SendToLCD = True
                    //        ///                    Me.GoLastLine = True
                    //        ///                    Me.Add_a_Line New_Line, False
                    //        ///                    This_Line.Quantity = 1
                    //        ///                    New_Line.price = This_Line.price
                    //        ///                    This_Line.Amount = This_Line.Quantity * This_Line.price
                    //        ///                Next
                    //        ///                Set New_Line = Nothing
                    //        ///            End If
                    //        //   end
                    //    }
                    //}

                    return returnValue;
                }


            }
            Performancelog.Debug($"End,SaleManager,Adjust_Lines,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Method to set Line price
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="price">Price</param>
        public void Line_Price(ref Sale sale, ref Sale_Line oLine, double price)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Line_Price,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            _saleLineManager.SetPrice(ref oLine, price);
            //oLine.price = price;
            Sale_Discount(ref sale, sale.Sale_Totals.Invoice_Discount, sale.Sale_Totals.Invoice_Discount_Type);
            Performancelog.Debug($"End,SaleManager,Line_Price,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            Chaps_Main.SA = sale;
        }

        /// <summary>
        /// Method to set sale discount
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="Discount">Discount</param>
        /// <param name="Disc_Type">Discount type</param>
        /// <param name="Temp_File">Temp file or not</param>
        /// <param name="DontRecaculateTotal">If recalculation is required or not</param>
        public void Sale_Discount(ref Sale sale, decimal Discount, string Disc_Type, bool Temp_File = true,
            bool DontRecaculateTotal = false)
        {

            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Sale_Discount,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            Sale_Line SL = default(Sale_Line);
            double Disc_Per = 0;
            double DApplied = 0;
            double NetTotal = 0;

            if (sale.LoadingTemp)
            {
                return;
            }

            if (Discount != 0)
            {

                NetTotal = 0;
                foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                {
                    SL = tempLoopVar_SL;
                    NetTotal = NetTotal + (double)SL.Amount - SL.Line_Discount;
                }

                if (Disc_Type == "$")
                {
                    if (NetTotal != 0)
                    {
                        Disc_Per = (double)Discount / NetTotal;
                    }
                    else
                    {
                        Disc_Per = 0;
                    }
                    DApplied = (double)Discount;
                }
                else
                {
                    Disc_Per = sale.Sale_Totals.Discount_Percent / 100;
                    DApplied = Disc_Per * NetTotal;
                    sale.Sale_Totals.Invoice_Discount = (decimal)DApplied;
                }

                sale.Sale_Totals.Invoice_Discount_Type = Disc_Type;

                foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                {
                    SL = tempLoopVar_SL;
                    if (Disc_Type == "$")
                    {
                        if (NetTotal != 0)
                        {
                            SL.Discount_Adjust = ((double)SL.Amount - SL.Line_Discount) / NetTotal * DApplied;
                        }
                        else
                        {
                            SL.Discount_Adjust = 0;
                        }
                    }
                    else
                    {
                        SL.Discount_Adjust = Disc_Per * (double)SL.Amount;
                    }
                }

            }
            else
            {
                foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                {
                    SL = tempLoopVar_SL;
                    SL.Discount_Adjust = 0;
                }
                sale.Sale_Totals.Invoice_Discount_Type = "";
                sale.Sale_Totals.Invoice_Discount = 0;
            }


            if (!DontRecaculateTotal)
            {

                ReCompute_Totals(ref sale);
            }

            Chaps_Main.SA = sale;

            //TODO: UDHAM Commented for Performance
            //if (!Temp_File)
            //{
            //    SaveTemp(ref sale, sale.TillNumber);
            //}
            Performancelog.Debug($"End,SaleManager,Sale_Discount,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to set the line quantity
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="Quantity">Quantity</param>
        /// <param name="Adjust">Adjudt line or not</param>
        /// <returns>True or false</returns>
        public bool Line_Quantity(ref Sale sale, ref Sale_Line oLine, float Quantity, bool Adjust = true)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Line_Quantity,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            bool returnValue = false;
            var security = _policyManager.LoadSecurityInfo();
            Sale_Line SL = default(Sale_Line);
            string ProcessedPromoID = "";
            string PrevPromoID = "";

            returnValue = true;
            Compute_Charges(ref sale, ref oLine, -1);
            Compute_Taxes(ref sale, ref oLine, -1);
            _saleLineManager.SetQuantity(ref oLine, Quantity);

            //  
            if (Adjust && (Strings.UCase(Convert.ToString(security.BackOfficeVersion)) == "FULL" || _policyManager.PROMO_SALE))
            {
                if (!oLine.HotButton)
                {
                    oLine.NoPromo = false; // September 24, 2008 the promo was not redone in change quantity if the item was scaned
                }
                PrevPromoID = oLine.PromoID;
                _saleLineManager.Make_Promo(ref sale, ref oLine, true);
                ProcessedPromoID = oLine.PromoID;
                foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                {
                    SL = tempLoopVar_SL;
                    if (!sale.MakingPromo && SL.Line_Num != oLine.Line_Num && ((SL.PromoID == ProcessedPromoID && !string.IsNullOrEmpty(ProcessedPromoID)) || (SL.PromoID == PrevPromoID && !string.IsNullOrEmpty(PrevPromoID))))
                    {
                        if (Adjust_Lines(ref SL, sale, false))
                        {
                        }
                    }
                }
            }
            // End July 09, 2008

            if (Adjust)
            {
                if (!Adjust_Lines(ref oLine, sale, false))
                {
                    ReCompute_Totals(ref sale);
                    returnValue = false;
                }
                else
                {
                    Compute_Charges(ref sale, ref oLine, 1);
                    Compute_Taxes(ref sale, ref oLine, 1);
                    ReCompute_Totals(ref sale); //binal Jan23,2001
                }
            }

            if (sale.Sale_Totals.Invoice_Discount != 0)
            {
                Sale_Discount(ref sale, sale.Sale_Totals.Invoice_Discount, sale.Sale_Totals.Invoice_Discount_Type);
            }

            //TODO: UDHAM Commented for Performance
            //if (!sale.LoadingTemp)
            //{
            //    SaveTemp(ref sale, sale.TillNumber);
            //}
            Performancelog.Debug($"End,SaleManager,Line_Quantity,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            Chaps_Main.SA = sale;
            return returnValue;
        }

        /// <summary>
        /// Method to set the discount rate on a line
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="DiscRate">Discount rate</param>
        public void Line_Discount_Rate(ref Sale sale, ref Sale_Line oLine, double DiscRate)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Line_Discount_Rate,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            //if (DiscRate == oLine.Discount_Rate)
            //{
            //    return;
            //}
            _saleLineManager.SetDiscountRate(ref oLine, (float)DiscRate);
            ReCompute_Totals(ref sale);

            //TODO: Udham Commented for Performance
            //if (!sale.LoadingTemp)
            //{
            //    SaveTemp(ref sale, sale.TillNumber);
            //}
            Performancelog.Debug($"End,SaleManager,Line_Discount_Rate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            Chaps_Main.SA = sale;
            
        }

        /// <summary>
        /// Method to set the discount type on a line.
        /// </summary>
        /// <param name="oLine"></param>
        /// <param name="DiscType"></param>
        public void Line_Discount_Type(ref Sale_Line oLine, string DiscType)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Line_Discount_Type,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            oLine.Discount_Type = DiscType;
            Performancelog.Debug($"End,SaleManager,Line_Discount_Type,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to get line reason
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="saleLine">Sale line</param>
        /// <param name="ReturnReason">Return reason</param>
        public void Line_Reason(ref Sale sale, ref Sale_Line saleLine, Return_Reason ReturnReason)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Line_Reason,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            if (ReturnReason != null)
                saleLine.Return_Reasons.Add(ReturnReason.Reason, ReturnReason.RType, ReturnReason.RType);

            //TODO: Udham Commented for performance
            //if (!sale.LoadingTemp)
            //{
            //    SaveTemp(ref sale, sale.TillNumber);
            //}
            Chaps_Main.SA = sale;
            Performancelog.Debug($"End,SaleManager,Line_Reason,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to set the Price Number on a line
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="saleLine"></param>
        /// <param name="Price_Number"></param>
        public void Line_Price_Number(ref Sale sale, ref Sale_Line saleLine, short Price_Number)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Line_Price_Number,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _saleLineManager.SetPriceNumber(ref saleLine, Price_Number);
            Adjust_Lines(ref saleLine, sale, false);
            Line_Price(ref sale, ref saleLine, saleLine.price);
            Sale_Discount(ref sale, sale.Sale_Totals.Invoice_Discount, sale.Sale_Totals.Invoice_Discount_Type);
            Performancelog.Debug($"End,SaleManager,Line_Price_Number,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            Chaps_Main.SA = sale;
        }

        /// <summary>
        /// Load the tax definitions
        /// </summary>
        /// <param name="sale"></param>
        //private void Load_Taxes(ref Sale sale)
        //{
        //    var dateStart = DateTime.Now;
        //    Performancelog.Debug($"Start,SaleManager,Load_Taxes,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

        //    var taxMast = _saleService.GetTaxMast();
        //    // 

        //    foreach (var tax in taxMast)
        //    {
        //        var taxRates = _taxService.GetTaxRatesByName(tax.TaxName);
        //        foreach (var taxRate in taxRates)
        //        {
        //            sale.Sale_Totals.Sale_Taxes.Add(Convert.ToString(tax.TaxName), Convert.ToString(taxRate.TaxCode), Convert.ToSingle(Information.IsDBNull(taxRate.Rate) ? 0 : taxRate.Rate), 0, 0, 0, 0, Convert.ToSingle(Information.IsDBNull(taxRate.Rebate) ? 0 : taxRate.Rebate), 0, tax.TaxName + Convert.ToString(taxRate.TaxCode));
        //        }
        //    }
        //    Performancelog.Debug($"End,SaleManager,Load_Taxes,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        //}

        /// <summary>
        /// Method to set the gross amount
        /// </summary>
        /// <param name="saleTotals">Sale </param>
        /// <param name="netAmount">Net amount</param>
        public void SetGross(ref Sale_Totals saleTotals, decimal netAmount)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,SetGross,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var gross = netAmount;
            decimal taxRebate = 0;
            decimal taxExemption = 0;
            Sale_Tax stx;
            foreach (Sale_Tax tempLoopVarStx in saleTotals.Sale_Taxes)
            {
                stx = tempLoopVarStx;
                // agencies
                if (_policyManager.TAX_EXEMPT_GA)
                {
                    gross = gross + stx.Tax_Added_Amount - stx.Tax_Rebate - stx.Tax_Exemption_GA_Incl;
                    taxExemption = taxExemption + stx.Tax_Exemption_GA_Incl + stx.Tax_Exemption_GA_Added;
                }
                else
                {
                    //   end
                    gross = gross + stx.Tax_Added_Amount - stx.Tax_Rebate; //   added Tax_Rebate deduction
                }
                taxRebate = taxRebate + stx.Tax_Rebate; //  
            }
            gross = gross + (decimal)saleTotals.Charge;
            saleTotals.Gross = gross;
            stx = null;

            //if (OldGross != saleTotals.Gross || string.IsNullOrEmpty(saleTotals.TotalLabel))
            {
                saleTotals.TotalLabel = saleTotals.Gross.ToString("###,##0.00");
            }

            var offSet = _policyManager.LoadStoreInfo().OffSet;
            //Summarize Taxes, Discounts &Associated charges.
            var summaryLabel = "";
            foreach (Sale_Tax tempLoopVarStx in saleTotals.Sale_Taxes)
            {
                stx = tempLoopVarStx;
                if (stx.Tax_Added_Amount != 0)
                {
                    summaryLabel = summaryLabel + stx.Tax_Name + "     $" + stx.Tax_Added_Amount.ToString("###,##0.00") + "    ";
                }
                if (stx.Tax_Included_Amount != 0)
                {
                    summaryLabel = summaryLabel + stx.Tax_Name + "(I)  $" + stx.Tax_Included_Amount.ToString("###,##0.00") + "    ";
                }
            }
            if (saleTotals.Charge != 0)
            {
                summaryLabel = summaryLabel + _resourceManager.GetResString(offSet, 264) + saleTotals.Charge.ToString("###,##0.00") + "    "; //"Chg $"
            }
            if (saleTotals.Invoice_Discount != 0)
            {
                summaryLabel = summaryLabel + _resourceManager.GetResString(offSet, 265) + saleTotals.Invoice_Discount.ToString("###,##0.00") + "    "; //"Discount $"
            }
            //   don't check the policy, if is false, Tax_Rebate amount should be 0
            if (taxRebate != 0)
            {
                summaryLabel = summaryLabel + _resourceManager.GetResString(offSet, 474) + "  " + taxRebate.ToString("$###,##0.00") + "    ";
            }
            //   end
            //  
            if (taxExemption != 0)
            {
                summaryLabel = summaryLabel + _resourceManager.GetResString(offSet, 1712) + "  " + taxExemption.ToString("$###,##0.00") + "    ";
            }
            //   end
            saleTotals.SummaryLabel = summaryLabel;

            // END: Setting the Summary text including Taxes, Discounts & Associated charges - Ipsit_5
            Performancelog.Debug($"End,SaleManager,SetGross,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to save sale in Dbtill
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="userCode">UserCode</param>
        /// <param name="Tenders">Tenders</param>
        /// <param name="SC">Store credit</param>
        public void SaveSale(Sale sale, string userCode, ref Tenders Tenders, Store_Credit SC)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,SaveSale,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            

            bool addNew = false;
            var user = _loginManager.GetExistingUser(userCode);
            short nTend = 0;
            Tender tender = default(Tender);
            double T_Used = 0;
            double T_Change = 0;
            short LN = 0;
            Sale_Line SL = default(Sale_Line);
            Line_Kit SK = default(Line_Kit);
            Charge CG = default(Charge);
            Line_Tax TX = default(Line_Tax);
            Sale_Tax SX = default(Sale_Tax);
            K_Charge KC = default(K_Charge);
            Line_Kit LK;
            Charge_Tax KT = default(Charge_Tax);
            Return_Reason RN = default(Return_Reason);
            string Sale_Type = "";
            string T_Name = "";

            // Files for Sale Transactions
            Charge_Tax CT = default(Charge_Tax);
            string strSql = "";
            int GC_Expiry_Days = 0;
            decimal curTotalTaxes = new decimal();

            bool HasFuelSale = false;
            bool hasCarwashSale = false;

            string Return_Policy;
            short ret = 0;

            GC_Expiry_Days = Convert.ToInt32(_policyManager.GetPol("GC_EXPIRE", null));

            // Remove any previous references to the transaction.
            _saleService.ClearRecordsFromDbTill(sale.Sale_Num, sale.TillNumber);

            var S_DisTend = new DiscountTender();
            var S_CardPrompt = new CardProfilePrompt();


            var cardTenders = _cardService.GetCardSalesFromDbTemp(sale.TillNumber, sale.Sale_Num);
            foreach (var cardTender in cardTenders)
            {
                _cardService.AddCardSaleToDbTills(cardTender);
            }

            Sale_Type = sale.Sale_Type;
            nTend = 0;

            //  var saleTends = new List<Infonet.CStoreCommander.Entities.SaleTend>();
            if (Tenders != null && Tenders.Count != 0)
            {
                foreach (Tender tempLoopVar_tender_Renamed in Tenders)
                {
                    tender = tempLoopVar_tender_Renamed;
                    var creditCard = tender.Credit_Card;
                    if (tender.Amount_Used != 0 || (tender.Credit_Card.Crd_Type == "D" && !string.IsNullOrEmpty(tender.Credit_Card.Cardnumber))) //  - When swiping the credit card at debit tender, it is putting a zero record with debit type without any card info'EMVVERSION)
                    {
                        nTend++;
                        // Save the tender in the SALETEND Table
                        var saleTend = new SaleTend();
                        saleTend.TillNumber = sale.TillNumber;
                        saleTend.SaleNumber = sale.Sale_Num;


                        ///                        (Policy.COMBINEFLEET And Tender.Tender_Class = "FLEET") then
                        if (_policyManager.GetPol("FUELONLY", null))
                        {
                            T_Name = tender.Tender_Name;
                        }
                        else
                        {
                            if ((_policyManager.GetPol("COMBINECR", null) && tender.Tender_Class == "CRCARD") || (_policyManager.GetPol("COMBINEFLEET", null) && tender.Tender_Class == "FLEET") || (_policyManager.GetPol("ThirdParty", null) && tender.Tender_Class == "THIRDPARTY"))
                            {
                                if ((tender.Credit_Card == null) || Strings.UCase(Convert.ToString(_policyManager.GetPol("CC_MODE", null))) != "VALIDATE")
                                {

                                    T_Name = tender.Tender_Name;
                                }
                                else
                                {
                                    T_Name = _creditCardManager.Return_TendDesc(tender.Credit_Card);
                                    // version
                                    // this is not the correct fix, but is a quick fix for this issue
                                    if (_policyManager.GetPol("EMVVersion", null) && string.IsNullOrEmpty(T_Name))
                                    {
                                        T_Name = tender.Credit_Card.Name;
                                    }
                                }
                            }
                            else
                            {
                                // Lines commented below were used for multiple instances, which we don't use anymore
                                ///                        If InStr(1, Tender.Tender_Name, " - ") > 0 Then
                                ///                            T_Name = Left$(Tender.Tender_Name, InStr(1, Tender.Tender_Name, " - ") - 1)
                                ///                        Else
                                //   added the If condition to fix the issue in EMV processing of multiple cards of the same tender
                                if (_policyManager.GetPol("EMVVersion", null) && !string.IsNullOrEmpty(tender.Credit_Card.Name) && tender.Tender_Class.ToUpper() != "ACCOUNT")
                                {
                                    T_Name = tender.Credit_Card.Name;
                                }
                                else
                                {
                                    T_Name = tender.Tender_Name;
                                }
                                ///                        End If
                            }
                        }
                        saleTend.SequenceNumber = nTend;
                        saleTend.Exchange = (decimal)tender.Exchange_Rate;

                        //  added following condition to correct the tender info.
                        if (tender.Tender_Code == "ACKG" || tender.Tender_Code == "ACK")
                        {
                            saleTend.TenderName = tender.Tender_Name;
                        }
                        else
                        {
                            saleTend.TenderName = string.IsNullOrEmpty(T_Name) ? tender.Tender_Name : T_Name;
                        }

                        //   added the extra condition, UCase$(Tender.Tender_Class) <> "GIFTCARD") to fix Ackroo
                        //   added the If condition to fix the issue in EMV processing of multiple cards of the same tender
                        if (_policyManager.GetPol("EMVVersion", null) && !string.IsNullOrEmpty(_creditCardManager.Return_TendClass(tender.Credit_Card)) && (tender.Tender_Class.ToUpper() != "ACCOUNT" && tender.Tender_Class.ToUpper() != "GIFTCARD"))
                        {
                            saleTend.TenderClass = _creditCardManager.Return_TendClass(tender.Credit_Card);
                        }
                        else
                        {
                            saleTend.TenderClass = tender.Tender_Class;
                        }
                        if (sale.Sale_Totals.Gross < 0)
                        {
                            //
                            //Negative payouts (reversal of the payout) was not calculating Cash Tender properly
                            //I am not sure why we need to *-1 and do abs(), we can just assign ![AmtTend] = Tender.Amount_Entered
                            //but I left it anyway maybe other types of Sale_Types use it (I test Refund, it doesn't use it)?
                            if (sale.Sale_Type.ToUpper() == "PAYOUT")
                            {
                                saleTend.AmountTend = Math.Abs(tender.Amount_Entered);
                                saleTend.AmountUsed = Math.Abs(tender.Amount_Used);
                                //End -SV
                            }
                            else
                            {
                                saleTend.AmountTend = Math.Abs(tender.Amount_Entered) * -1;
                                saleTend.AmountUsed = Math.Abs(tender.Amount_Used) * -1;
                            }
                        }
                        else
                        {
                            saleTend.AmountTend = tender.Amount_Entered;
                            saleTend.AmountUsed = tender.Amount_Used;
                        }



                        saleTend.SerialNumber = sale.AR_PO;


                        saleTend.Exchange = (decimal)tender.Exchange_Rate;



                        //  - Encrypt Card numbers based on Policy + card Setup for Encryption. But for debit and credit cards only look the policy- not the card settings( always consider card setting as true)-(polcy checking is inside the Encryptdecrypt)
                        //               ![CCard_Num] = EncryptDecrypt.Encrypt(Tender.Credit_Card.CardNumber)

                        if ((tender.Credit_Card.Crd_Type == "C" || tender.Credit_Card.Crd_Type == "D") && !string.IsNullOrEmpty(tender.Credit_Card.Cardnumber))
                        {
                            saleTend.CCardNumber = _encryptManager.Encrypt(tender.Credit_Card.Cardnumber, tender.Credit_Card.Crd_Type);
                            //  - kickback card
                        }
                        else if (tender.Credit_Card.Crd_Type == "K" && tender.Tender_Class == "LOYALTY" && !string.IsNullOrEmpty(sale.Customer.PointCardNum))
                        {
                            saleTend.CCardNumber = sale.Customer.PointCardNum;
                            // 
                        }
                        else //Not credit or debit card
                        {
                            if (tender.Credit_Card.EncryptCard && !string.IsNullOrEmpty(tender.Credit_Card.Cardnumber)) // if card setup is to encrypt, send to encrypt- based on policy encrypt it or not
                            {
                                saleTend.CCardNumber = _encryptManager.Encrypt(tender.Credit_Card.Cardnumber, "");
                            }
                            else // if card setup is not to encrypt, no need to check the policy. keep it as it is
                            {
                                saleTend.CCardNumber = tender.Credit_Card.Cardnumber;
                            }
                        }
                        // 

                        saleTend.CCardAPRV = tender.Credit_Card.Authorization_Number;

                        saleTend.AuthUser = tender.AuthUser;

                        _saleService.AddSaleTend(saleTend, DataSource.CSCTills);

                        // Save credit card information
                        //  - EMVVERSION-LAST- Trying to eliminate track2 for EMvVersion

                        if ((sale.EMVVersion == false && !string.IsNullOrEmpty(tender.Credit_Card.Cardnumber)) || sale.EMVVersion && tender.Credit_Card.Crd_Type == "C" || tender.Credit_Card.Crd_Type == "D"
                            || sale.IsArTenderUsed)
                        {
                            if (sale.IsArTenderUsed && tender.Credit_Card?.Crd_Type == null && tender.Tender_Name == _policyManager.ARTender)
                            {
                                var card = CacheManager.GetCreditCard(sale.TillNumber, sale.Sale_Num);
                                card.Trans_Amount = tender.Credit_Card.Trans_Amount;
                                tender.Credit_Card = card;
                                T_Name = card.Name;
                                Tenders.tender = tender;
                                CacheManager.AddTendersForSale(sale.Sale_Num, sale.TillNumber, Tenders);
                            }

                            //EMVVERSION End
                            //  added if condition not to create any ACKROO records as they are copied from CurSale
                            if (tender.Tender_Code != "ACKG" && !string.IsNullOrEmpty(tender.Credit_Card?.Cardnumber))
                            {
                                var S_Card = new CardTender();
                                S_Card.TillNumber = sale.TillNumber;
                                S_Card.SaleNumber = sale.Sale_Num;
                                S_Card.TenderName = T_Name;
                                S_Card.CardName = tender.Credit_Card.Name;
                                S_Card.CardType = tender.Credit_Card.Crd_Type;


                                //  - Encrypt Card numbers based on Policy + card Setup for Encryption. But for debit and credit cards only look the policy- not the card settings( always consider card setting as true)-(polcy checking is inside the Encryptdecrypt)
                                //                    ![Card_Number] = EncryptDecrypt.Encrypt(Tender.Credit_Card.CardNumber)
                                if ((tender.Credit_Card.Crd_Type == "C" || tender.Credit_Card.Crd_Type == "D") && !string.IsNullOrEmpty(tender.Credit_Card.Cardnumber))
                                {
                                    S_Card.CardNum = _encryptManager.Encrypt(tender.Credit_Card.Cardnumber, tender.Credit_Card.Crd_Type);
                                }
                                else //Not credit or debit card
                                {
                                    if (tender.Credit_Card.EncryptCard && !string.IsNullOrEmpty(tender.Credit_Card.Cardnumber)) // if card setup is to encrypt, send to encrypt- based on policy encrypt it or not
                                    {
                                        S_Card.CardNum = _encryptManager.Encrypt(tender.Credit_Card.Cardnumber, "");
                                    }
                                    else // if card setup is not to encrypt, no need to check the policy. keep it as it is
                                    {
                                        S_Card.CardNum = tender.Credit_Card.Cardnumber;
                                    }
                                }
                                // 


                                S_Card.ExpiryDate = tender.Credit_Card.Expiry_Date;
                                S_Card.Swiped = tender.Credit_Card.Card_Swiped;
                                S_Card.StoreForward = tender.Credit_Card.StoreAndForward;
                                if (tender.Tender_Code == "ACK")
                                {
                                    S_Card.Amount = tender.Amount_Used;
                                }
                                else
                                {
                                    S_Card.Amount = (decimal)tender.Credit_Card.Trans_Amount; //Tender.Amount_Used
                                }
                                S_Card.ApprovalCode = tender.Credit_Card.Authorization_Number;
                                S_Card.SequenceNumber = tender.Credit_Card.Sequence_Number;
                                S_Card.DeclineReason = tender.Credit_Card.Decline_Message;
                                S_Card.Result = tender.Credit_Card.Result;
                                S_Card.TerminalID = tender.Credit_Card.TerminalID;
                                S_Card.DebitAccount = !string.IsNullOrEmpty(tender.Credit_Card.DebitAccount) ? tender.Credit_Card.DebitAccount.Substring(0, 1) : "";
                                S_Card.ResponseCode = tender.Credit_Card.ResponseCode;
                                S_Card.ISOCode = tender.Credit_Card.ApprovalCode;
                                S_Card.TransactionDate = tender.Credit_Card.Trans_Date == DateTime.MinValue ? DateAndTime.Today : tender.Credit_Card.Trans_Date;
                                S_Card.TransactionTime = tender.Credit_Card.Trans_Time == DateTime.MinValue ? DateAndTime.TimeOfDay : tender.Credit_Card.Trans_Time;
                                S_Card.ReceiptDisplay = tender.Credit_Card.Receipt_Display;
                                S_Card.Language = _creditCardManager.Language(ref creditCard);
                                S_Card.CustomerName = tender.Credit_Card.Customer_Name;
                                S_Card.CallTheBank = _creditCardManager.Call_The_Bank(ref creditCard);
                                // 
                                S_Card.VechicleNo = tender.Credit_Card.Vechicle_Number;
                                S_Card.DriverNo = tender.Credit_Card.Driver_Number;
                                S_Card.IdentificationNo = tender.Credit_Card.ID_Number;
                                S_Card.Odometer = tender.Credit_Card.Odometer_Number;
                                S_Card.CardUsage = tender.Credit_Card.usageType;


                                if (!_policyManager.USE_PINPAD) // donot print the information entered through a pin pad/pump- print only the one entered throug pos
                                {

                                    S_Card.PrintVechicleNo = tender.Credit_Card.Print_VechicleNo;
                                    S_Card.PrintDriverNo = tender.Credit_Card.Print_DriverNo;
                                    S_Card.PrintIdentificationNo = tender.Credit_Card.Print_IdentificationNo;
                                }
                                S_Card.PrintUsage = tender.Credit_Card.Print_Usage;
                                S_Card.PrintOdometer = tender.Credit_Card.Print_Odometer;
                                //shiny end
                                S_Card.Balance = tender.Credit_Card.Balance;
                                S_Card.Quantity = tender.Credit_Card.Quantity;


                                if (!string.IsNullOrEmpty(tender.Credit_Card.Report))
                                {
                                    var maxlength = tender.Credit_Card.Report.Length > 2500 ? 2500 : tender.Credit_Card.Report.Length;
                                    S_Card.Message = tender.Credit_Card.Report.Substring(0, maxlength);
                                }
                                else
                                {
                                    S_Card.Message = "";
                                }

                                S_Card.CardProfileID = tender.Credit_Card.CardProfileID; // 
                                S_Card.PONumber = tender.Credit_Card.PONumber;
                                S_Card.Sequence = nTend;
                                _saleService.AddCardTender(S_Card, DataSource.CSCTills);
                            }
                            //  end
                        }

                        // Update the customer's points on a points-based loyalty system.
                        if (tender.Tender_Class == "POINTS")
                        {
                            // sale.Customer.PointsAwarded = (decimal)(modGlobalFunctions.Round((double)Sum_QA, 2));
                            // sale.Customer.Loyalty_Points = sale.Customer.Loyalty_Points + (double)sale.Customer.PointsAwarded - (double)PointsUsed;
                            // _customerService.UpdateCustomer(sale.Customer);
                        }
                        else if ((tender.Tender_Class == "ACCOUNT") || (sale.Sale_Type == "ARPAY"))
                        {
                            if (user.User_Group.Code != Entities.Constants.Trainer) //Behrooz Jan-12-06
                            {
                                // Update the customer's account balance.
                                var customer = _customerService.GetClientByClientCode(sale.Customer.Code);
                                if (customer != null)
                                {
                                    customer.Current_Balance = sale.Customer.Current_Balance;
                                    _customerService.UpdateCustomer(customer);
                                }
                            }
                        }
                    }
                }
                tender = null;
                T_Used = (double)Tenders.Tend_Totals.Tend_Used;


                if (SC == null && sale.Sale_Type != "PAYOUT" && sale.Sale_Type != "BTL RTN")
                {

                    T_Change = (double)(-1 * Math.Abs(Tenders.Tend_Totals.Change));
                }
                else
                {
                    T_Change = 0;
                }
            }
            else
            {
                nTend = 0;
                T_Used = 0;
                T_Change = 0;
            }

            double dPoint = 0;
            double dBalance = 0;
            string sCardNumber = "";
            //Balance
            //dPoint = _saleService.GetPoints(sale.Sale_Num, sale.TillNumber);
            //dBalance = _saleService.GetBalance(sale.Sale_Num, sale.TillNumber);
            dPoint = Convert.ToDouble(sale?.SaleHead?.LoyalPoint);
            dBalance = Convert.ToDouble(sale?.SaleHead?.LoyaltyBalance);

            //sCardNumber = _saleService.GetCardNumber(sale.Sale_Num, sale.TillNumber);
            sCardNumber = sale?.SaleHead?.LoyaltyCard;

            //end
            var salehead = sale.SaleHead;// _saleService.GetSaleHeadFromDbTill(sale.TillNumber, sale.Sale_Num);
            addNew = true;

            if (salehead == null)
            {
                salehead = new SaleHead();
                salehead.SaleNumber = sale.Sale_Num;
                salehead.TillNumber = sale.TillNumber;
            }

            bool HasUpSell = false;
            salehead.SaleNumber = sale.Sale_Num;
            if (sale.TillNumber == 0) //binal Mar06,2002
            {
                salehead.TillNumber = sale.TillNumber;
            }
            else
            {
                salehead.TillNumber = sale.TillNumber;
            }
            var till = _tillService.GetTill(sale.TillNumber);
            salehead.Shift = till.Shift;
            salehead.ShiftDate = till.ShiftDate;
            salehead.Register = sale.Register;
            salehead.User = user.Code;
            salehead.Client = sale.Customer.Code;
            salehead.SaleDate = DateAndTime.Today;
            salehead.SaleTime = DateAndTime.TimeOfDay;
            salehead.SD = DateTime.Now;

            if (sale.Sale_Type == "VOID")
            {
                salehead.AssociatedAmount = 0;
            }
            else
            {

                salehead.AssociatedAmount = (decimal)sale.Sale_Totals.Charge;
            }
            salehead.LineDiscount = (decimal)modGlobalFunctions.Round((double)sale.Sale_Line_Disc, 2);
            salehead.INVCDiscount = (decimal)modGlobalFunctions.Round((double)sale.Sale_Totals.Invoice_Discount, 2);
            salehead.DiscountType = "%";
            salehead.TenderAmount = (decimal)T_Used * (sale.Sale_Totals.Gross < 0 ? -1 : 1);


            if (sale.Sale_Type == "VOID")
            {
                salehead.SaleAmount = sale.Sale_Totals.Gross - (decimal)sale.Sale_Totals.Charge;
            }
            else
            {

                salehead.SaleAmount = sale.Sale_Totals.Gross;
            } //12/19/06

            salehead.Deposit = 0;
            salehead.Payment = sale.Sale_Totals.Payment;
            salehead.PayOut = sale.Sale_Totals.PayOut;
            salehead.Change = (decimal)T_Change;
            salehead.Reason = sale.Return_Reason.Reason; // Nicolette
            salehead.ReasonType = sale.Return_Reason.RType; // Nicolette
            salehead.OverPayment = sale.OverPayment; // Nicolette
            salehead.PennyAdjust = sale.Sale_Totals.Penny_Adj; //  
            salehead.RefernceNum = sale.ReferenceNumber; //  

            //used
            if (Math.Round((double)sale.Customer.PointsAwarded, 2) == 0)
            {
                salehead.LoyalPoint = (decimal)dPoint;
            }
            else
            {
                salehead.LoyalPoint = (decimal)Math.Round((double)sale.Customer.PointsAwarded, 2);
            }
            salehead.LoyaltyBalance = (decimal)dBalance;
            //end

            if (SC == null)
            {
                salehead.Credits = 0;
            }
            else
            {
                salehead.Credits = -SC.Amount;
            }
            salehead.TType = sale.Sale_Type;
            salehead.SaleLine = sale.Sale_Lines.Count;
            salehead.TendLine = nTend;


            if (sale.Upsell)
            {
                HasUpSell = false;


                foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                {
                    SL = tempLoopVar_SL;
                    if (SL.Upsell)
                    {
                        HasUpSell = true;
                    }
                }
                salehead.Upsell = HasUpSell;
            }
            else
            {
                salehead.Upsell = false;
            }


            salehead.TreatyNumber = sale.TreatyNumber;

            //used

            if (string.IsNullOrEmpty(sale.Customer.LoyaltyCard))
            {
                salehead.LoyaltyCard = sCardNumber;
            }
            else
            {
                salehead.LoyaltyCard = sale.Customer.LoyaltyCard;
            }

            //end
            //TODO: Commented by UD for Performance
            if (addNew)
            {
                _saleService.AddSaleHeadToDbTill(salehead);
            }
            else
            {
                _saleService.UpdateSaleHeadToDbTill(salehead);
            }
            sale.SaleHead = salehead;

            //_saleService.UpdateSaleHeadToDBTill(salehead);
            addNew = false;
            if (sale.Void_Num > 0 && sale.ForCorrection)
            {
                var voidSale = new VoidSale();
                voidSale.TillNumber = sale.TillNumber;
                voidSale.SaleNumber = sale.Sale_Num;
                voidSale.VoidNumber = sale.Void_Num;
                _saleService.AddVoidSaleToDbTill(voidSale);
            }

            //  
            if (!string.IsNullOrEmpty(sale.Sale_Type) && sale.Sale_Type.ToUpper() == "PAYOUT" && !string.IsNullOrEmpty(sale.Vendor))
            {
                var saleVendors = new SaleVendors();
                saleVendors.TillNumber = sale.TillNumber;
                saleVendors.SaleNumber = sale.Sale_Num;
                saleVendors.Vendor = sale.Vendor;
                _saleService.AddSaleVendorsToDbTill(saleVendors);
            }

            float DisAmount = 0;
            if (!string.IsNullOrEmpty(sale.Sale_Type) && sale.Sale_Type != "VOID") //  
            {

                curTotalTaxes = 0;
                // Record all the taxes charged in the sale.


                foreach (Sale_Tax tempLoopVar_SX in sale.Sale_Totals.Sale_Taxes)
                {
                    SX = tempLoopVar_SX;
                    if (SX.Taxable_Amount != 0 | SX.Tax_Included_Amount != 0)
                    {
                        var saleTax = new Sale_Tax();
                        saleTax.TillNumber = sale.TillNumber;
                        saleTax.SaleNumber = sale.Sale_Num;
                        saleTax.Tax_Name = SX.Tax_Name;
                        saleTax.Tax_Code = SX.Tax_Code;
                        saleTax.Tax_Rate = SX.Tax_Rate;
                        saleTax.Taxable_Amount = SX.Taxable_Amount;
                        saleTax.Tax_Added_Amount = SX.Tax_Added_Amount;
                        saleTax.Tax_Included_Amount = SX.Tax_Included_Amount;
                        saleTax.Tax_Included_Total = SX.Tax_Included_Total;
                        saleTax.Tax_Rebate = SX.Tax_Rebate;
                        saleTax.Tax_Rebate_Rate = SX.Tax_Rebate_Rate;
                        curTotalTaxes = curTotalTaxes + SX.Tax_Added_Amount; //Behrooz
                        _saleService.AddSaleTaxToDbTill(saleTax);
                    }
                }
                SX = null;


                if (!string.IsNullOrEmpty(sale.Sale_Type) && sale.Sale_Type == "SALE")
                {
                    var shiftDate = _tillService.GetTill(sale.TillNumber).ShiftDate;
                    _saleService.Update_Sale_NoneResettableGrantTotal(Convert.ToInt16(sale.TillNumber), Convert.ToDateTime(shiftDate), sale.Sale_Totals.Gross - ((decimal)sale.Sale_Totals.Charge + curTotalTaxes));
                }

                if (user.User_Group.Code != Entities.Constants.Trainer) //Behrooz Jan-12-06
                {
                    // If it is a Loyalty Points program then award points based on the sum
                    // of Amount based and Quantity based items.
                    if (sale.Sale_Type == "SALE" || sale.Sale_Type == "REFUND")
                    {
                        if (sale.USE_LOYALTY && sale.LOYAL_TYPE == "Points" && !string.IsNullOrEmpty(sale.Customer.Loyalty_Code))
                        {
                            var customer = _customerService.GetClientByClientCode(sale.Customer.Code);
                            //S_Cust.Find(Criteria: "CL_CODE=\'" + sale.Customer.Code + "\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
                            if (customer != null)
                            {
                                if (sale.Customer.Loyalty_Points >= 0) //  don't allow negative points
                                {
                                    customer.Loyalty_Points = Math.Round(sale.Customer.Loyalty_Points, 2);
                                }
                                else
                                {
                                    customer.Loyalty_Points = 0;
                                }
                                _customerService.UpdateCustomer(customer);
                            }
                        }
                    }
                }

                HasFuelSale = false;
                LN = 0;
                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    SL = tempLoopVarSl;

                    if (SL.ProductIsFuel)
                    {
                        HasFuelSale = true;
                    }

                    if (SL.Dept == _policyManager.CarwashDepartment)
                    {
                        SL.IsCarwashProduct = true;
                        hasCarwashSale = true;                    }

                    LN++;
                    var saleLine = new Sale_Line();
                    saleLine = SL;
                    //S_Line.AddNew();
                    saleLine.Till_Num = sale.TillNumber;
                    saleLine.Sale_Num = sale.Sale_Num.ToString();
                    saleLine.Line_Num = LN;
                    foreach (Sale_Line s in Chaps_Main.SA.Sale_Lines)
                    {
                        if (s.CarwashCode != "")
                        {
                            saleLine.CarwashCode = s.CarwashCode;
                            saleLine.IsCarwashProduct = true;
                            sale.HasCarwashProducts = true;
                        }
                    }

                    if (user.User_Group.Code != Entities.Constants.Trainer) //Behrooz Jan-12-06
                    {

                        // If it isn't a Void or Cancel and it is a stock type that we
                        // keep track of (i.e. 'V' or 'O') then update the In-Stock and
                        // Available for sale quantities.
                        if (!string.IsNullOrEmpty(sale.Sale_Type) && sale.Sale_Type != "VOID" && sale.Sale_Type != "CANCEL")
                        {
                            if (SL.Stock_Type == 'V' || SL.Stock_Type == 'O')
                            {



                                // 
                                //                             If sl.IsTaxExemptItem And Policy.TE_Type = "AITE" Then  '  - Only for AITE we need to track inventory separately for TE products 'If sl.IsTaxExemptItem Then
                                if (SL.IsTaxExemptItem && (_policyManager.TE_Type == "AITE" || (_policyManager.TE_Type != "AITE" && _policyManager.TRACK_TEINV))) //  - Only for AITE we need to track inventory separately for TE products 'If TaxExempt Then
                                {
                                    // 
                                    //  - To avoid update error - when acessing same stock_code from 2 tills                               
                                    if (SL.Quantity < 0)
                                    {
                                        string temp_Policy_Name = "ADD_RET_TO";
                                        Return_Policy = System.Convert.ToString(this._policyManager.GetPol(temp_Policy_Name, SL)); //SL.ADD_RET_TO

                                        if (Return_Policy == "IN STOCK")
                                        {
                                            strSql = "Update ProductTaxExempt " + " SET InStock = InStock - " + SL.Quantity + ", Available =  Available - " + SL.Quantity + ", LastSaleTime = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where ProductKey = \'" + SL.Stock_Code + "\' ";
                                        }
                                        else if (Return_Policy == "HOLD")
                                        {
                                            strSql = "Update ProductTaxExempt " + " SET hold = hold - " + SL.Quantity + ", LastSaleTime = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where ProductKey = \'" + SL.Stock_Code + "\' ";
                                        }
                                        else
                                        {
                                            strSql = "Update ProductTaxExempt " + " SET Waste = Waste - " + SL.Quantity + ", LastSaleTime = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where ProductKey = \'" + SL.Stock_Code + "\' ";
                                        }
                                    }
                                    else
                                    {
                                        strSql = "Update ProductTaxExempt " + " SET InStock = InStock - " + SL.Quantity + ", Available =  Available - " + SL.Quantity + ", LastSaleTime = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where ProductKey = \'" + SL.Stock_Code + "\' ";
                                    }
                                    _saleService.UpdateTaxExempt(strSql, DataSource.CSCMaster);
                                }
                                else
                                {
                                    // Giving the update eror with global or local recordset, so changed to update directly
                                    if (SL.Quantity < 0)
                                    {
                                        string temp_Policy_Name2 = "ADD_RET_TO";
                                        Return_Policy = System.Convert.ToString(this._policyManager.GetPol(temp_Policy_Name2, SL)); //SL.ADD_RET_TO
                                        if (Return_Policy == "IN STOCK")
                                        {
                                            strSql = "Update Stock_Br " + " SET IN_Stock = In_stock - " + SL.Quantity + ", Avail =  Avail - " + SL.Quantity + ", Last_Sale = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where stock_code = \'" + SL.Stock_Code + "\' ";
                                        }
                                        else if (Return_Policy == "HOLD")
                                        {
                                            strSql = "Update Stock_Br " + " SET hold = hold - " + SL.Quantity + ", Last_Sale = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where stock_code = \'" + SL.Stock_Code + "\' ";
                                        }
                                        else
                                        {
                                            strSql = "Update Stock_Br " + " SET Waste = Waste - " + SL.Quantity + ", Last_Sale = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where stock_code = \'" + SL.Stock_Code + "\' ";
                                        }
                                    }
                                    else
                                    {
                                        strSql = "Update Stock_Br " + " SET IN_Stock = In_stock - " + SL.Quantity + ", Avail =  Avail - " + SL.Quantity + ", Last_Sale = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where stock_code = \'" + SL.Stock_Code + "\' ";
                                    }
                                    _saleService.UpdateStockBr(strSql, DataSource.CSCMaster);
                                }
                            }
                        }
                    }
                    ErrorMessage error = new ErrorMessage();
                    _saleService.AddSaleLineToDbTill(saleLine, sale.Sale_Type);

                    if (user.User_Group.Code != Entities.Constants.Trainer) //Behrooz Jan-12-06
                    {
                        if (SL.Gift_Certificate && !string.IsNullOrEmpty(sale.Sale_Type) && sale.Sale_Type != "VOID" && sale.Sale_Type != "CANCEL" && SL.GiftType == "LocalGift")
                        {
                            // If it doesn't expire then set expiry to a couple of
                            // centuries from now.
                            if (GC_Expiry_Days == 0)
                            {
                                GC_Expiry_Days = 99999;
                            }
                            var giftCert = new GiftCert();
                            giftCert.GcExpiryDays = GC_Expiry_Days;
                            giftCert.SaleNumber = sale.Sale_Num;
                            giftCert.LineNumber = LN;
                            giftCert.GcNumber = SL.Gift_Num;
                            giftCert.GcAmount = SL.Amount;
                            giftCert.GcCust = sale.Customer.Code;
                            giftCert.GcDate = DateAndTime.Today;
                            giftCert.GcUser = user.Code;
                            giftCert.GcExpiresOn = DateAndTime.DateAdd(DateInterval.Day, GC_Expiry_Days, DateAndTime.Today);
                            giftCert.GcRegister = sale.Register;
                            _saleService.AddGiftCertificate(giftCert);
                        }
                    }

                    var saleLineReason = new SaleLineReason();
                    // Nicolette to record the reasons for each line in a sale
                    foreach (Return_Reason tempLoopVar_RN in SL.Return_Reasons)
                    {
                        RN = tempLoopVar_RN;
                        saleLineReason.TillNumber = till.Number;
                        saleLineReason.SaleNumber = sale.Sale_Num;
                        saleLineReason.LineNumber = LN;
                        saleLineReason.Reason = RN.Reason;
                        saleLineReason.ReasonType = RN.RType;
                        _saleService.AddSaleLineReason(saleLineReason);
                    }
                    RN = null;
                    // Nicolette end

                    //   added Or (Len(Trim$(SL.TE_COLLECTTAX)) <> 0 And mvarTaxForTaxExempt) to the next line
                    // and If TX.Tax_Added_Amount <> 0 Or TX.Tax_Incl_Amount <> 0 Then condition inside the for loop to save taxes collected from tax exempt customers
                    if (sale.ApplyTaxes || (!string.IsNullOrEmpty(SL.TE_COLLECTTAX) && sale.TaxForTaxExempt))
                    {
                        var lineTax = new Line_Tax();
                        foreach (Line_Tax tempLoopVar_TX in SL.Line_Taxes)
                        {
                            TX = tempLoopVar_TX;
                            if (TX.Tax_Added_Amount != 0 | TX.Tax_Incl_Amount != 0)
                            {
                                // S_LTax.AddNew();
                                lineTax.TillNumber = sale.TillNumber;
                                lineTax.SaleNumber = sale.Sale_Num;
                                lineTax.LineNumber = LN;
                                lineTax.Tax_Name = TX.Tax_Name;
                                lineTax.Tax_Code = TX.Tax_Code;
                                lineTax.Tax_Rate = TX.Tax_Rate;
                                lineTax.Tax_Included = TX.Tax_Included;
                                // Nicolette added next two lines
                                lineTax.Tax_Added_Amount = TX.Tax_Added_Amount;
                                lineTax.Tax_Incl_Amount = TX.Tax_Incl_Amount;
                                //   for tax rebate Ontario, if policy is False record 0 to avoid null checking in return, reprint, suspend and crash recovery
                                lineTax.Tax_Rebate = TX.Tax_Rebate;
                                lineTax.Tax_Rebate_Rate = TX.Tax_Rebate_Rate;
                                _saleService.AddSaleLineTax(lineTax);
                            }
                        }
                        TX = null;
                    }

                    var saleKit = new Line_Kit();
                    foreach (Line_Kit tempLoopVar_SK in SL.Line_Kits)
                    {
                        SK = tempLoopVar_SK;
                        saleKit.TillNumber = till.Number;
                        saleKit.SaleNumber = sale.Sale_Num;
                        saleKit.LineNumber = LN;
                        saleKit.Kit_Item = SK.Kit_Item;
                        saleKit.Kit_Item_Desc = SK.Kit_Item_Desc;
                        saleKit.Kit_Item_Qty = SK.Kit_Item_Qty;
                        saleKit.Kit_Item_Base = SK.Kit_Item_Base;
                        saleKit.Kit_Item_Fraction = SK.Kit_Item_Fraction;
                        saleKit.Kit_Item_Allocate = SK.Kit_Item_Allocate;
                        saleKit.Kit_Item_Serial = SK.Kit_Item_Serial;
                        _saleService.AddSaleLineKit(saleKit);

                        UpdateStockLevels(SK.Kit_Item, (decimal)(SL.Quantity * SK.Kit_Item_Qty), sale.Sale_Type, ref SL, SL.IsTaxExemptItem);

                        if (sale.ApplyCharges)
                        {
                            foreach (K_Charge tempLoopVar_KC in SK.K_Charges)
                            {
                                KC = tempLoopVar_KC;
                                var chargeTax = new Charge_Tax();



                                if (sale.ApplyTaxes)
                                {
                                    foreach (Charge_Tax tempLoopVar_KT in KC.Charge_Taxes)
                                    {
                                        KT = tempLoopVar_KT;
                                        chargeTax.TillNumber = sale.TillNumber;
                                        chargeTax.SaleNumber = sale.Sale_Num;
                                        chargeTax.LineNumber = LN;
                                        chargeTax.KitItem = saleKit.Kit_Item;
                                        chargeTax.ChargeCode = KC.Charge_Code;
                                        chargeTax.Tax_Name = KT.Tax_Name;
                                        chargeTax.Tax_Code = KT.Tax_Code;
                                        chargeTax.Tax_Rate = KT.Tax_Rate;
                                        chargeTax.Tax_Included = KT.Tax_Included;
                                        chargeTax.Tax_Added_Amount = KT.Tax_Added_Amount; // Round(KT.Taxable_Amount * (KT.Tax_Rate / 100#), 2) Nicolette changed
                                        chargeTax.Tax_Incl_Amount = KT.Tax_Incl_Amount;
                                        _saleService.AddChargeTax(chargeTax);
                                    }

                                }
                                var charge = new Charge();
                                charge.TillNumber = sale.TillNumber;
                                charge.SaleNumber = sale.Sale_Num;
                                charge.LineNumber = LN;
                                charge.KitItem = SK.Kit_Item;
                                charge.AsCode = KC.Charge_Code;
                                charge.Description = KC.Charge_Desc;
                                charge.Quantity = SK.Kit_Item_Qty;
                                charge.Price = KC.Charge_Price;
                                charge.Amount = (float)(KC.Charge_Price * SL.Quantity * SK.Kit_Item_Qty);
                                _saleService.AddKitCharge(charge);
                            }
                        }
                    }
                    SK = null;
                    // Behrooz Move to the Begin of Function
                    //            Dim CT As Charge_Tax
                    if (sale.ApplyCharges)
                    {
                        var charge = new Charge();
                        foreach (Charge tempLoopVar_CG in SL.Charges)
                        {
                            CG = tempLoopVar_CG;
                            //S_Assoc.AddNew();
                            charge.TillNumber = sale.TillNumber;
                            charge.SaleNumber = sale.Sale_Num;
                            charge.LineNumber = LN;
                            charge.AsCode = CG.Charge_Code;
                            charge.Charge_Desc = CG.Charge_Desc;
                            charge.Quantity = SL.Quantity;
                            charge.Charge_Price = CG.Charge_Price;
                            charge.Amount = CG.Charge_Price * SL.Quantity;







                            if (sale.ApplyTaxes)
                            {

                                foreach (Charge_Tax tempLoopVar_CT in CG.Charge_Taxes)
                                {
                                    var chargeTax = new Charge_Tax();

                                    chargeTax.TillNumber = sale.TillNumber;
                                    chargeTax.SaleNumber = sale.Sale_Num;
                                    chargeTax.LineNumber = LN;
                                    chargeTax.KitItem = saleKit.Kit_Item;
                                    chargeTax.ChargeCode = KC.Charge_Code;
                                    chargeTax.Tax_Name = CT.Tax_Name;
                                    chargeTax.Tax_Code = CT.Tax_Code;
                                    chargeTax.Tax_Rate = CT.Tax_Rate;
                                    chargeTax.Tax_Included = CT.Tax_Included;
                                    chargeTax.Tax_Added_Amount = CT.Tax_Added_Amount; // Round(KT.Taxable_Amount * (KT.Tax_Rate / 100#), 2) Nicolette changed
                                    chargeTax.Tax_Incl_Amount = CT.Tax_Incl_Amount;
                                    _saleService.AddChargeTax(chargeTax);
                                }
                            }
                            _saleService.AddCharge(charge);
                        }
                    }
                    CG = null;
                    LK = null;
                    KC = null;

                }


                if (HasFuelSale && !string.IsNullOrEmpty(sale.Customer.GroupID) && !string.IsNullOrEmpty(sale.Customer.DiscountType) && (_policyManager.FuelLoyalty || _policyManager.CashBonus))
                {

                    DisAmount = 0;
                    if (_policyManager.FuelLoyalty)
                    {
                        foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                        {
                            SL = tempLoopVar_SL;
                            if (SL.ProductIsFuel)
                            {
                                if (sale.Customer.DiscountType == "%")
                                {
                                    DisAmount = DisAmount + (float)SL.Amount * sale.Customer.DiscountRate / 100;
                                }
                                else
                                {
                                    DisAmount = DisAmount + SL.Quantity * sale.Customer.DiscountRate;
                                }
                            }
                        }
                        DisAmount = float.Parse(DisAmount.ToString("##0.00"));
                    }
                    var discount = _saleService.GetDiscountTender(sale.Sale_Num, sale.TillNumber);
                    if (discount == null)
                    {
                        addNew = true;
                        discount = new DiscountTender();
                        discount.SaleNumber = sale.Sale_Num;
                        discount.TillNumber = till.Number;
                    }
                    discount.ClCode = sale.Customer.Code;
                    discount.CardNumber = sale.Customer.LoyaltyCard;
                    discount.SaleAmount = sale.Sale_Totals.Gross;
                    discount.DiscountType = sale.Customer.DiscountType;
                    discount.DiscountRate = sale.Customer.DiscountRate;
                    //  Cash Bonus
                    if (_policyManager.CashBonus && sale.Customer.DiscountType == "B" && sale.Customer.GroupID == "8")
                    {
                        //uncomment the following code to activate the cashbonus
                       
                    
                    //foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                        //{
                        //    SL = tempLoopVar_SL;
                        //    if (SL.ProductIsFuel)
                        //    {
                        //        ReCompute_CashBonus(ref sale);
                        //    }
                        //}
                      //  discount.DiscountAmount = sale.CBonusTotal;
                    }
                    else //shiny end
                    {
                        discount.DiscountAmount = (decimal)DisAmount;
                    }
                    if (sale.Customer.DiscountType == "C")
                    {
                        discount.CouponId = sale.CouponID;
                    }
                    else
                    {
                        discount.CouponId = "";
                    }
                    if (addNew)
                        _saleService.AddDiscountTender(discount);
                    else
                        _saleService.UpdateDiscountTender(discount);


                    if (sale.Customer.DiscountType == "C" && !string.IsNullOrEmpty(sale.CouponID))
                    {
                        var coupon = _saleService.GetCoupon(sale.CouponID);

                        if (DisAmount > 0)
                        {
                            if (coupon == null)
                            {
                                coupon = new Coupon();
                                coupon.CouponId = sale.CouponID.Trim();
                                coupon.Amount = (decimal)DisAmount;
                                coupon.ExpiryDate = DateTime.FromOADate(DateAndTime.Today.ToOADate() + 180);
                                coupon.Used = false;
                                coupon.Void = false;
                                _saleService.AddCoupon(coupon);
                            }

                        }
                        else if (DisAmount < 0)
                        {
                            if (coupon != null)
                            {
                                coupon.Void = true;
                                _saleService.UpdateCoupon(coupon);
                            }
                        }
                    }
                }

              


            }
            else
            {
                // VOID Sale - Save the lines that were deleted.

                HasFuelSale = false;

                foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                {
                    SL = tempLoopVar_SL;

                    if (SL.ProductIsFuel)
                    {
                        HasFuelSale = true;
                    }
                    SL.Sale_Num = sale.Sale_Num.ToString();
                    Save_Deleted_Line(ref SL, "V");
                }

            }

            //  - cardprofileprompt
            var cardProfilePrompts = _cardService.GetCardProfilePromptFromDbTemp(till.Number, sale.Sale_Num);
            foreach (var cardProfilePrompt in cardProfilePrompts)
            {
                _cardService.AddCardProfilePromptToDbTills(cardProfilePrompt);
            }
            //shiny end

            //   for Real time validation for SITE
            if (_policyManager.SITE_RTVAL)
            {
                //Treaty number
                if (!(Chaps_Main.oTreatyNo == null))
                {
                    if (Chaps_Main.oTreatyNo.ValidTreatyNo && (sale.Sale_Type == "SALE" || sale.Sale_Type == "REFUND"))
                    {
                        ret = Convert.ToInt16(Variables.RTVPService.PurchaseTransaction());
                        WriteToLogFile("Response is " + Convert.ToString(ret) + " from PurchaseTransaction sent with no parameters");
                        Chaps_Main.oTreatyNo.ValidTreatyNo = false;
                        Variables.RTVPService = null;
                    }
                }
            }
            Chaps_Main.SA = sale;
            _saleService.RemoveTempDataInDbTill(till.Number, sale.Sale_Num);
            //remove data of this sale from cache
            CacheManager.DeleteCurrentSaleForTill(sale.TillNumber, sale.Sale_Num);
            //   end

            //  added to keep the AllowMulticard which is Ackroo GiftCard and Ackroo Loyalty

            // Chaps_Main.dbTill.Execute("Insert INTO [CSCTills].[dbo].[CardTenders] " + "([TILL_NUM],[Sale_No],[Tender_Name],[Card_Name],[Card_Type],[CallTheBank],[Card_Number],[Expiry_Date], " + "[Swiped],[Amount],[Approval_Code],[Decline_Code],[Decline_Reason],[Store_Forward],[Result],[TerminalID], " + "[DebitAccount],[ResponseCode],[SequenceNumber],[TransactionDate],[TransactionTime],[ReceiptDisplay], " + "[ISOCode],[CustomerName],[Language],[BatchNumber],[VechicleNo],[DriverNo],[Odometer],[IdentificationNo], " + "[CardUsage],[PrintVechicleNo],[PrintDriverNo],[PrintIdentificationNo],[PrintOdometer],[PrintUsage], " + "[Balance],[Quantity],[Message],[BatchDate],[CardProfileID],[PONumber],[Sequence]) " + "SELECT [Till_Num],[Sale_No],[Tender_Name],[Card_Name],[Card_Type],[CallTheBank],[Card_Number], " + "[Expiry_Date],[Swiped],[Amount],[Approval_Code],[Decline_Code],[Decline_Reason],[Store_Forward], " + "[Result],[TerminalID],[DebitAccount],[ResponseCode],[SequenceNumber],[TransactionDate],[TransactionTime], " + "[ReceiptDisplay],[ISOCode],[CustomerName],[Language],[BatchNumber],[VechicleNo],[DriverNo],[Odometer], " + "[IdentificationNo],[CardUsage],[PrintVechicleNo],[PrintDriverNo],[PrintIdentificationNo],[PrintOdometer], " + "[PrintUsage],[Balance],[Quantity],[Message],[BatchDate],[CardProfileID],[PONumber],[Sequence]  FROM [CSCCurSale].[dbo].[CardTenders]  WHERE [AllowMulticard]>1 and [Amount]>0", out null_object3);
            //  end
            Performancelog.Debug($"End,SaleManager,SaveSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to set customer
        /// </summary>
        /// <param name="customerCode">Customer code</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Sale</returns>
        public Sale SetCustomer(string customerCode, int saleNumber, int tillNumber, string userCode,
            byte registerNumber, string card, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,SetCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            errorMessage = new ErrorMessage();
            double getPrice = 0;
            bool cd = false;
            bool pd = false;
            short loyalPricecode = 0;

            if (string.IsNullOrEmpty(customerCode))
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = "Invalid customer",
                    MessageType = 0
                };
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }

            if (!ValidateTillAndSale(tillNumber, saleNumber, out errorMessage))
            {
                return null;
            }

            var sale = GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message) && sale == null)
            {
                return null;
            }

            if (sale != null && !sale.EligibleTaxEx && !sale.Apply_CustomerChange) //   added And Not SA.EligibleTaxEx to reevaluate the sale if tax exemption button was clicked in Customer form
            {
                if (sale.Customer.Code == customerCode)
                {
                    return sale; //   if is the same code don't reevaluate the sale
                }
            }

            WriteToLogFile("Changing customer to " + customerCode);
            sale.Customer = _customerManager.LoadCustomer(customerCode);
            if (sale.Customer == null)
            {
                errorMessage = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Customer Does not Exist",
                        MessageType = MessageType.OkOnly
                    },
                    StatusCode = HttpStatusCode.NotFound
                };
                return null;
            }


            if (!string.IsNullOrEmpty(card))
            {
                MessageStyle message = new MessageStyle();
                var cardNumber = _customerManager.EvaluateCardString(card, out message);
                sale.Customer.LoyaltyCard = cardNumber;
                sale.Customer.LoyaltyExpDate = Chaps_Main.LoyExpDate;
                sale.Customer.LoyaltyCardSwiped = true;
                if (_policyManager.RSTR_PROFILE)
                {
                    sale.Customer.CardProfileID = _customerService.GetCustomerCardProfile(sale.Customer.LoyaltyCard);
                }
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


            if (_policyManager.USE_LOYALTY && Strings.UCase(Convert.ToString(_policyManager.LOYAL_TYPE)) == "PRICES" && !string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "A")
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

            pd = Convert.ToBoolean(_policyManager.PROD_DISC); // Use product discount codes

            foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
            {
                var sl = tempLoopVarSl;


                //  - For fule we shouldn't look for different price_number
                //If Not sl.Gift_Certificate then
                if (sl.Gift_Certificate == false && sl.ProductIsFuel == false)
                {
                    // 


                    if (_policyManager.USE_LOYALTY && Strings.UCase(Convert.ToString(_policyManager.LOYAL_TYPE)) == "PRICES" && !string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "A")
                    {
                        if (getPrice != sl.Price_Number)
                        {
                            if (!sl.LOY_EXCLUDE)
                            {
                                Line_Price_Number(ref sale, ref sl, loyalPricecode);
                            }
                            else
                            {
                                Line_Price_Number(ref sale, ref sl, (short)getPrice);
                            }
                        }
                    }
                    else
                    {
                        if (getPrice != sl.Price_Number)
                        {
                            Line_Price_Number(ref sale, ref sl, (short)getPrice);
                        }
                    }
                }
                //  For using Loyalty discount



                if (_policyManager.USE_LOYALTY && Strings.UCase(Convert.ToString(_policyManager.LOYAL_TYPE)) == "DISCOUNTS" && !string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "A")
                {
                    if (!sl.LOY_EXCLUDE)
                    {

                        var Loydiscode = Convert.ToInt16(_policyManager.LOYAL_DISC);
                        if (cd || pd)
                        {
                            if (cd && pd)
                            {
                                _saleLineManager.Apply_Table_Discount(ref sl, sl.Prod_Discount_Code, Loydiscode, out errorMessage);
                            }
                            else if (cd)
                            {
                                _saleLineManager.Apply_Table_Discount(ref sl, 0, Loydiscode, out errorMessage);
                            }
                            else if (pd)
                            {
                                _saleLineManager.Apply_Table_Discount(ref sl, sl.Prod_Discount_Code, 0, out errorMessage);
                            }
                        }
                        else
                        {
                            if (cd && pd)
                            {
                                _saleLineManager.Apply_Table_Discount(ref sl, sl.Prod_Discount_Code, sale.Customer.Discount_Code, out errorMessage);
                            }
                            else if (cd)
                            {
                                _saleLineManager.Apply_Table_Discount(ref sl, 0, sale.Customer.Discount_Code, out errorMessage);
                            }
                            else if (pd)
                            {
                                _saleLineManager.Apply_Table_Discount(ref sl, sl.Prod_Discount_Code, 0, out errorMessage);
                            }
                        }
                        Line_Discount_Type(ref sl, sl.Discount_Type);
                        Line_Discount_Rate(ref sale, ref sl, sl.Discount_Rate);
                    }
                }
                else
                {
                    //Shiny end

                    if (cd || pd)
                    {
                        if (cd && pd) // Use both customer & product discounts
                        {
                            _saleLineManager.Apply_Table_Discount(ref sl, sl.Prod_Discount_Code, sale.Customer.Discount_Code, out errorMessage);
                        }

                        else if (cd) // Use customer but not product
                        {
                            _saleLineManager.Apply_Table_Discount(ref sl, 0, sale.Customer.Discount_Code, out errorMessage);
                        }
                        else if (pd) // Use product but not customer
                        {
                            _saleLineManager.Apply_Table_Discount(ref sl, sl.Prod_Discount_Code, 0, out errorMessage);
                        }
                        Line_Discount_Type(ref sl, sl.Discount_Type);
                        Line_Discount_Rate(ref sale, ref sl, sl.Discount_Rate);
                    }
                    WriteToLogFile(" Applied customer discounts from Customer change");

                    if (sl.FuelRebateEligible && sl.FuelRebate > 0 && sale.Customer.UseFuelRebate && sale.Customer.UseFuelRebateDiscount)
                    {
                        _saleLineManager.ApplyFuelRebate(ref sl);
                    }
                    else
                    {



                        if (sl.ProductIsFuel && _policyManager.FuelLoyalty)
                        {
                            if (!string.IsNullOrEmpty(sale.Customer.GroupID))
                            {
                                if (!string.IsNullOrEmpty(sale.Customer.DiscountType))
                                {

                                    string temp_Policy_Name = "CL_DISCOUNTS";
                                    if (!_policyManager.GetPol(temp_Policy_Name, sl))
                                    {
                                        var offSet = _policyManager.LoadStoreInfo().OffSet;
                                        MessageType temp_VbStyle = (int)MessageType.Critical + MessageType.OkOnly;
                                        errorMessage = new ErrorMessage
                                        {
                                            MessageStyle = _resourceManager.CreateMessage(offSet, 11, 81, null, temp_VbStyle),
                                            StatusCode = HttpStatusCode.OK
                                        };
                                        return sale;
                                    }
                                    else
                                    {

                                        //  Discountchart loyalty
                                        //same as $discount by litre- only difference is discount rate should be based on grade
                                        if (sale.Customer.DiscountType == "D")
                                        {
                                            _saleLineManager.ApplyFuelLoyalty(ref sl, sale.Customer.DiscountType, _saleLineManager.GetFuelDiscountChartRate(ref sl, sale.Customer.GroupID, sl.GradeID), sale.Customer.DiscountName); // this will bring the discount rate based on customer group id and fuel grade
                                        }
                                        else
                                        {
                                            // 
                                            _saleLineManager.ApplyFuelLoyalty(ref sl, sale.Customer.DiscountType, sale.Customer.DiscountRate, sale.Customer.DiscountName);
                                            WriteToLogFile("Apply FuelLoyalty from Customer change");
                                        }
                                    }
                                }
                            }
                        }

                    }
                } //Shiny
            }

            ReCompute_Coupon(ref sale); //05/17/06 Nancy added for Fuel Loyalty of Coupon type
            ReCompute_Totals(ref sale);
            WriteToLogFile(" Finished Recompute from Customer change");
            SaveTemp(ref sale, tillNumber); //  If taking customer after adding all items and if there is no discount system was not saving the customer info.
            WriteToLogFile("SaveTemp from Customer change");
            //Update Sale object in Cache
            CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);

            Performancelog.Debug($"End,SaleManager,SetCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            Chaps_Main.SA = sale;
            return sale;
        }

        /// <summary>
        /// Method to check whether we can edit a field or not
        /// </summary>
        /// <param name="saleLines">Sale lines</param>
        /// <param name="userCode">User code</param>
        /// <returns>Sale line edit</returns>
        public List<SaleLineEdit> CheckEditOptions(Sale_Lines saleLines, string userCode)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,CheckEditOptions,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            List<SaleLineEdit> saleLineEdits = new List<SaleLineEdit>();
            if (saleLines.Count > 0)
            {
                var user = _loginManager.GetExistingUser(userCode);
                var userCanChangeQty = _policyManager.U_CHGQTY;
                //_policyManager.GetPol("U_CHGQTY", user);
                var userCanChangePrice = _policyManager.U_CHGPRICE;
                //_policyManager.GetPol("U_CHGPRICE", user);
                var userCanChangeDiscount = _policyManager.U_DISCOUNTS;
                //_policyManager.GetPol("U_DISCOUNTS", user);
                //string temp_Policy_Name = "ALLOW_QC";
                //string temp_Policy_Name2 = "ALLOW_PC";
                //string temp_Policy_Name4 = "DISC_REASON";
                //string temp_Policy_Name5 = "PR_REASON";
                //string temp_Policy_Name6 = "RET_REASON";
                var fuelProduct = false;
                foreach (Sale_Line saleLine in saleLines)
                {
                    bool qtyChange;
                    bool discountChange;
                    if ((saleLine.GiftType == "LocalGift" && _policyManager.GC_NUMBERS) || saleLine.GiftType == "GiveX")
                    {
                        qtyChange = false;
                    }
                    else
                    {
                        qtyChange = Convert.ToBoolean(userCanChangeQty && saleLine.ALLOW_QC && (!saleLine.ScalableItem));
                    }
                    if (saleLine.ProductIsFuel && saleLine.Quantity > 0 && (!saleLine.ManualFuel) && (!saleLine.IsPropane) && (_policyManager.USE_FUEL) && saleLine.pumpID != 0)
                    {
                        fuelProduct = true;
                    }
                    if ((!saleLine.Gift_Certificate && _policyManager.GC_DISCOUNT) || (_policyManager.GetPol("CL_DISCOUNTS", saleLine) && userCanChangeDiscount))
                    {
                        discountChange = true;
                    }
                    else
                    {
                        discountChange = false; // IIf((GetPol("CL_DISCOUNTS", SL) And User_Gives_Discounts), True, False)
                    }


                    saleLineEdits.Add(
                    new SaleLineEdit
                    {
                        LineNumber = saleLine.Line_Num,
                        AllowQuantityChange = qtyChange,
                        AllowPriceChange = Convert.ToBoolean(userCanChangePrice && saleLine.ALLOW_PC && (!saleLine.Gift_Certificate) && (!saleLine.Prepay)),
                        AllowDiscountChange = discountChange,
                        AllowDiscountReason = saleLine.DISC_REASON,
                        AllowPriceReason = saleLine.PR_REASON,
                        AllowReturnReason = saleLine.RET_REASON,
                        ConfirmDelete = _policyManager.CONFIRM_DEL && !fuelProduct
                    });
                }
            }
            Performancelog.Debug($"End,SaleManager,CheckEditOptions,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return saleLineEdits;
        }

        /// <summary>
        /// Update Sale Line Item
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="userCode">User code</param>
        /// <param name="discountRate">Discount rate</param>
        /// <param name="discountType">Discount type</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="price">Price</param>
        /// <param name="reasonCode">Reason code</param>
        /// <param name="reason">Reason</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="errorMessage">Error</param>
        /// <param name="lcdMsg"></param>
        /// <returns>Sale</returns>
        public Sale UpdateSaleLine(int saleNumber, int tillNumber, int lineNumber, string userCode, decimal discountRate, string discountType,
            decimal quantity, float price, string reasonCode, string reason, byte registerNumber,
            out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,UpdateSaleLine,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            if (!ValidateTillAndSale(tillNumber, saleNumber, out errorMessage))
            {
                return null;
            }

            Return_Reason returnReason = null;
            var sale = GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }
            ReasonType reasonType;
            Enum.TryParse(reason, true, out reasonType);
            if ((char)reasonType != '\0')
            {
                returnReason = _reasonService.GetReturnReason(reasonCode, (char)reasonType);
            }

            Leaving_Field(ref sale, discountRate, (decimal)price, (float)quantity, discountType, lineNumber, returnReason, out errorMessage);
            if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                SaveTemp(ref sale, tillNumber);
            }
            //Update cache
            CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);

            //Update Sale object in Cache
            // CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);

            Performancelog.Debug($"End,SaleManager,UpdateSaleLine,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            Chaps_Main.SA = sale;
            return sale;
        }

        /// <summary>
        /// Method to save sale 
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <param name="userCode"></param>
        /// <param name="tenders"></param>
        public void SaveSaveToDbTemp(Sale sale, int saleNumber, int tillNumber, string userCode, ref Tenders tenders)
        {
            if (sale == null)
            {
                sale = _saleService.GetSale(tillNumber, saleNumber);
                //sale = _saleService.GetSaleBySaleNoFromDbTemp(tillNumber, saleNumber);
            }
            if (sale != null)
            {
                ErrorMessage error = new ErrorMessage();
                //sale = Load_Temp(saleNumber, tillNumber, userCode, out error);
                SaveSale(sale, userCode, ref tenders, null);
            }
        }

        /// <summary>
        /// Method to apply charges
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="value">Value</param>
        public void ApplyCharges(ref Sale sale, bool value)
        {
            Sale_Line SL = default(Sale_Line);
            if (value)
            {
                if (sale.ApplyCharges)
                {
                    return;
                }
                sale.ApplyCharges = value;
                // Changing from FALSE to TRUE ... Add the charges back in.
                foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                {
                    SL = tempLoopVar_SL;
                    Compute_Charges(ref sale, ref SL, (short)1);
                }
            }
            else
            {
                if (!sale.ApplyCharges)
                {
                    return;
                }
                sale.ApplyCharges = value;
                // Changing from TRUE to FALSE - Remove the charges
                foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                {
                    SL = tempLoopVar_SL;
                    Compute_Charges(ref sale, ref SL, (short)1);
                }
            }

            ReCompute_Totals(ref sale);
            if (!sale.LoadingTemp)
            {
                SaveTemp(ref sale, sale.TillNumber);
            }
            Chaps_Main.SA = sale;
        }

        /// <summary>
        /// Method to enabel cash button or not
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="userCode">User code</param>
        /// <returns>Trur or false</returns>
        public bool EnableCashButton(Sale sale, string userCode)
        {
            var enableButton = true;
            if (!_policyManager.TAX_EXEMPT)
            {

                if (sale.ForCorrection)
                {
                    enableButton = false;
                }
                else
                {



                    //  Policy to control activate exact change button - User, company, user group
                    var user = _loginManager.GetExistingUser(userCode);
                    if (!_policyManager.GetPol("EXACTCHG", user))
                    {
                        enableButton = false;
                    }
                    else // 
                    {

                        enableButton = (!sale.HasRebateLine)
                             && (sale.Sale_Lines.Count != 0)
                             && (!string.IsNullOrEmpty(_policyManager.BASECURR));
                    }

                }
            }
            else
            {
                enableButton = false;
            }



            if (!string.IsNullOrEmpty(GetLoyaltyNo(sale.Void_Num)))
            {
                enableButton = false;
            }
            //   end
            return enableButton;
        }

        /// <summary>
        /// Method to enable write off or not
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <returns>True or false</returns>
        public bool EnableWriteOffButton(string userCode)
        {
            var user = _loginManager.GetExistingUser(userCode);
            return Convert.ToBoolean(_policyManager.GetPol("ALLOW_MARKDO", user));
        }

        /// <summary>
        /// Method to load vendor coupons
        /// </summary>
        /// <returns>Vendor coupons</returns>
        public VendorCoupons LoadVendorCoupons()
        {
            var vendorCoupons = CacheManager.GetVendorCoupon();
            if (vendorCoupons != null)
            {
                return vendorCoupons;
            }
            vendorCoupons = new VendorCoupons();
            var coupons = _stockService.GetVendorCoupons();
            foreach (var coupon in coupons)
            {
                vendorCoupons.AddCoupon((short)(vendorCoupons.Count + 1), coupon, "");
            }
            CacheManager.AddVendorCoupons(vendorCoupons);
            return vendorCoupons;
        }

        /// <summary>
        /// Method to add tax exemption code
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="taxExemptionCode">Tax exemption code</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        public Sale SetTaxExemptionCode(int saleNumber, int tillNumber, string userCode,
        string taxExemptionCode, out ErrorMessage error)
        {
            var sale = GetCurrentSale(saleNumber, tillNumber, 0, userCode, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return sale;
            }
            bool flag = false;
            if (_policyManager.TAX_EXEMPT_GA && !string.IsNullOrEmpty(taxExemptionCode))
            {
                sale.EligibleTaxEx = true;
                sale.ReferenceNumber = Strings.Left(taxExemptionCode.Trim(), 40);
                WriteToLogFile("Eligible for tax exemption set to true");
                flag = true;
            }
            else
            {
                sale.EligibleTaxEx = false;
                sale.ReferenceNumber = "";
                flag = false;
            }
            sale = SetCustomer(sale.Customer.Code, saleNumber, tillNumber, UserCode, 0, string.Empty, out error);
            CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);
            Chaps_Main.SA = sale;
            return sale;
        }

        #region Private methods



        /// <summary>
        /// Method to get loyalty number
        /// </summary>
        /// <param name="voidNumber">Void number</param>
        /// <returns>Loyalty number</returns>
        private string GetLoyaltyNo(int voidNumber)
        {
            int OldSaleNo = 0;
            var loyaltyCard = string.Empty;
            //OldSaleNo = _saleService.GetVoidNumberFromDBTemp();

            if (voidNumber != 0)
            {
                loyaltyCard = _saleService.GetLoyaltyCardFromDbTemp(OldSaleNo, DataSource.CSCTills);
                if (string.IsNullOrEmpty(loyaltyCard))
                {
                    loyaltyCard = _saleService.GetLoyaltyCardFromDbTemp(OldSaleNo, DataSource.CSCTrans);
                }
            }
            return loyaltyCard;
        }

        // ====================================================================
        // Validate the entry in a field.
        // ====================================================================
        private void Leaving_Field(ref Sale sale, decimal discountRate, decimal price, float quantity, string discountType, int lineNumber, Return_Reason returnReason, out ErrorMessage errorMesage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Leaving_Field,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            const double MAXPRICE = 9999; // unreadeable
            const float MAXQTY = 9999;
            const double MINPRICE = -9999;
            const float MINQTY = -9999;
            //short DiscType;
            short CL = 0;
            double DiscAmt = 0;
            string DiscStr = "";
            //  var  SL = new Sale_Line();
            var user = new User();
            string x = "";
            short n = 0;
            short LinesNo = 0;
            short Pd = 0;
            short Qd = 0;
            string fs = "";
            float Quantity = 0;
            string strCD_Message = ""; // Nov 18, 2008 Nicolette to display message to customer display when an item is deleted, a price or qty change happens, or a discount is applied
            float DPer = 0;
            float DDol = 0;
            errorMesage = new ErrorMessage();




            //if (!isEnabled)
            //{
            //    return;
            //}

            CL = (short)(lineNumber);
            if (CL <= sale.Sale_Lines.Count && CL != 0)
            {
                var SL = sale.Sale_Lines[CL];
                user = _loginManager.GetExistingUser(SL.User);

                if (discountRate != (decimal)SL.Discount_Rate || (!string.IsNullOrEmpty(discountType) && discountType != SL.Discount_Type))
                {
                    DiscStr = System.Convert.ToString(discountType);
                    //if (Strings.Left(DiscStr.Trim(), 1) == "$")
                    //{
                    //    DiscStr = DiscStr.Substring(1);
                    //}
                    //if (Strings.Right(DiscStr.Trim(), 1) == "%")
                    //{
                    //    DiscStr = Strings.Left(DiscStr, DiscStr.Length - 1);
                    //}
                    DiscAmt = (double)discountRate;
                    SL.User_Discount = false;

                    if (DiscAmt > 0)
                    {

                        string temp_Policy_Name = "CL_DISCOUNTS";
                        string temp_Policy_Name2 = "GC_DISCOUNT";
                        if (_policyManager.GetPol(temp_Policy_Name, SL) && (!SL.Gift_Certificate || (SL.Gift_Certificate && _policyManager.GetPol(temp_Policy_Name2, null)))) // you can give discounts for this product
                        {

                            if (discountType == "%" || discountType == "$")
                            {

                                // DiscType = TIMsgbox("Is this a % Discount?", vbDefaultButton1 + vbQuestion + vbYesNoCancel, "Discount Type", Me)
                                //MessageType temp_VbStyle = (MessageType)((int)MessageType.DefaultButton1 + (int)MessageType.Question + (int)MessageType.YesNoCancel);

                                //errorMesage.MessageStyle = _resourceManager.CreateMessage(offSet,11, (short)90,  null, temp_VbStyle);
                                //DiscType = (short)(Chaps_Main.DisplayMessage(this, (short)90, temp_VbStyle, null, (byte)0));

                                SL.User_Discount = true;
                                if (discountType == "%")
                                {



                                    string temp_Policy_Name3 = "MAX_DISC%";
                                    DPer = System.Convert.ToSingle(_policyManager.GetPol(temp_Policy_Name3, SL));



                                    if (DPer >= DiscAmt)
                                    {
                                        Line_Discount_Type(ref SL, "%");
                                        //sale.Line_Discount_Type(SL, "%");
                                        Line_Discount_Rate(ref sale, ref SL, DiscAmt);

                                        //sale.Line_Discount_Rate(SL, DiscAmt);
                                        //txtDiscount[Index].Text = DiscAmt.ToString("##0.00") + "%";
                                        //if (Strings.Right(SL.Discount_Type, 1) == "%")
                                        //{
                                        //   // SL.Discount_Type = Strings.Left(SL.Discount_Type, SL.Discount_Type.Length - 1); // Nicolette added, remove % char from strolddiscount
                                        //}
                                        //if (Strings.Right(discountType, 1) == "%")
                                        //{
                                        //    discountType = Strings.Left(discountType, discountType.Length - 1); //Binal added, remove % char from KeypadDis
                                        //}

                                        string temp_Policy_Name4 = "DISC_REASON";
                                        if (DiscAmt != Conversion.Val(discountRate) && _policyManager.GetPol(temp_Policy_Name4, SL) && returnReason != null)
                                        {
                                            Line_Reason(ref sale, ref SL, returnReason);
                                        }


                                        SL.SendToLCD = true;
                                        strCD_Message = _resourceManager.GetResString(offSet, (short)417); // a line discount has been given
                                    }
                                    else
                                    {
                                        //TIMsgbox "Maximum Discount is " & DPer & "%" & vbCrLf & "Discount Set to Zero", _
                                        //vbCritical + vbOKOnly, "Maximum Discount Exceeded", Me
                                        MessageType temp_VbStyle2 = (int)MessageType.Critical + MessageType.OkOnly;
                                        errorMesage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, (short)89, DPer, temp_VbStyle2);
                                        errorMesage.StatusCode = HttpStatusCode.OK;
                                        Line_Discount_Type(ref SL, " ");
                                        //sale.Line_Discount_Type(SL, " ");
                                        Line_Discount_Rate(ref sale, ref SL, 0);

                                        //sale.Line_Discount_Rate(SL, 0);
                                        //  txtDiscount[Index].Text = "";

                                    }

                                }
                                else if (discountType == "$")
                                {



                                    string temp_Policy_Name5 = "MAX_DISC$";
                                    DDol = System.Convert.ToSingle(_policyManager.GetPol(temp_Policy_Name5, SL));



                                    if (DDol < DiscAmt)
                                    {
                                        //TIMsgbox "Maximum Discount is $" & Format(DDol, "0.00") & vbCrLf & "Discount Set to Zero", _
                                        //vbCritical + vbOKOnly, "Maximum Discount Exceeded", Me

                                        MessageType temp_VbStyle3 = (int)MessageType.Critical + MessageType.OkOnly;
                                        errorMesage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, (short)88, DDol, temp_VbStyle3);
                                        errorMesage.StatusCode = HttpStatusCode.OK;
                                        Line_Discount_Type(ref SL, " ");
                                        //sale.Line_Discount_Type(SL, " ");
                                        Line_Discount_Rate(ref sale, ref SL, 0);

                                        //sale.Line_Discount_Rate(SL, 0);
                                        // txtDiscount[Index].Text = "";

                                        ///                    ElseIf DiscAmt * SL.Quantity > SL.Amount Then
                                    }
                                    else if (DiscAmt > Convert.ToDouble(SL.Amount)) //   to fix the wrong message for dollar discount
                                    {
                                        //TIMsgbox "Discount Cannot Exceed the Line Amount" & vbCrLf & "Discount Set to Zero", _
                                        //vbCritical + vbOKOnly, "Line Amount Exceeded", Me

                                        MessageType temp_VbStyle4 = (int)MessageType.Critical + MessageType.OkOnly;
                                        errorMesage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, (short)87, null, temp_VbStyle4);
                                        errorMesage.StatusCode = HttpStatusCode.OK;
                                        Line_Discount_Type(ref SL, " ");
                                        //sale.Line_Discount_Type(SL, " ");
                                        Line_Discount_Rate(ref sale, ref SL, 0);

                                        //.SA.Line_Discount_Rate(SL, 0);
                                        //txtDiscount[Index].Text = "";

                                    }
                                    else
                                    {
                                        Line_Discount_Type(ref SL, "$");
                                        // sale.Line_Discount_Type(SL, "$");
                                        Line_Discount_Rate(ref sale, ref SL, DiscAmt);

                                        // sale.Line_Discount_Rate(SL, DiscAmt);
                                        // 
                                        Pd = SL.PRICE_DEC; // Price Decimals
                                        fs = System.Convert.ToString(Pd == 0 ? " " : ("." + new string('9', Pd)));
                                        //                        txtDiscount(Index).Text = Format(DiscAmt, "##0.0")  & "%"

                                        //TODO:Check this later on 

                                        if (SL.NoPriceFormat)
                                        {
                                            //  SL.Discount_Rate = (float)Convert.ToDouble(DiscAmt.ToString("##0.00"));
                                        }
                                        else
                                        {
                                            //  SL.Discount_Rate = (float)Convert.ToDouble(DiscAmt.ToString(fs));
                                        }
                                        //shiny end
                                        if (Strings.Right(SL.Discount_Type, 1) == "%")
                                        {
                                            SL.Discount_Type = Strings.Left(SL.Discount_Type, SL.Discount_Type.Length - 1); // Nicolette added, remove % char from strolddiscount
                                        }
                                        //if (VB.Strings.Right(KeypadDis, 1) == "%")
                                        //{
                                        //    KeypadDis = VB.Strings.Left(KeypadDis, KeypadDis.Length - 1); //Binal added, remove % char from KeypadDis
                                        //}

                                        string temp_Policy_Name6 = "DISC_REASON";
                                        if (DiscAmt != Conversion.Val(discountRate) && _policyManager.GetPol(temp_Policy_Name6, SL) && returnReason != null)
                                        {
                                            Line_Reason(ref sale, ref SL, returnReason); //Nicolette added
                                        }




                                        SL.SendToLCD = true;
                                        strCD_Message = _resourceManager.GetResString(offSet, (short)417); // a line discount has been given
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageType temp_VbStyle5 = (int)MessageType.Critical + MessageType.OkOnly;
                            errorMesage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, (short)81, null, temp_VbStyle5);
                            errorMesage.StatusCode = HttpStatusCode.OK;
                            Line_Discount_Type(ref SL, " ");
                            //sale.Line_Discount_Type(SL, " ");
                            Line_Discount_Rate(ref sale, ref SL, 0);
                            return;
                            //sale.Line_Discount_Rate(SL, 0);
                            //txtDiscount[Index].Text = "";
                        }
                    }
                    else if (DiscAmt < 0)
                    {
                        // "Negative Discounts are Not Allowed", _
                        //"Negative Discount"
                        MessageType temp_VbStyle6 = (int)MessageType.Critical + MessageType.OkOnly;
                        errorMesage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, (short)86, null, temp_VbStyle6);
                        errorMesage.StatusCode = HttpStatusCode.NotAcceptable;
                        return;
                    }
                    else
                    {
                        Line_Discount_Type(ref SL, " ");
                        //sale.Line_Discount_Type(SL, " ");
                        Line_Discount_Rate(ref sale, ref SL, 0);

                        //sale.Line_Discount_Rate(SL, 0);
                        //txtDiscount[Index].Text = "";
                    }
                }
                if (Conversion.Val(price) != SL.price)
                {
                    x = price.ToString();
                    //for (n = 1; n <= Strings.Len(txtPrice[Index].Text); n++)
                    //{
                    //    if ((double.Parse(Strings.Mid(System.Convert.ToString(txtPrice[Index].Text), n, 1)) >= double.Parse("0") && double.Parse(Strings.Mid(System.Convert.ToString(txtPrice[Index].Text), n, 1)) <= double.Parse("9")) || Strings.Mid(System.Convert.ToString(txtPrice[Index].Text), n, 1) == "-" || Strings.Mid(System.Convert.ToString(txtPrice[Index].Text), n, 1) == ".")
                    //    {
                    //        x = x + Strings.Mid(System.Convert.ToString(txtPrice[Index].Text), n, 1);
                    //    }
                    //}



                    // Price Decimals

                    if (IsValidPrice(SL.PRICE_DEC, Conversion.Val(x), out errorMesage))
                    {





                        if (x.Length > 0 && Conversion.Val(x) > 0 && (!SL.IsTaxExemptItem))
                        {


                            //  - For both F type and X type we don't have to ask reason, S and I added by binal
                            //   special prices and promo prices are set by system
                            // and we don't allow user to change the price, except when the price is expired.
                            if ((SL.Price_Type == 'R' && string.IsNullOrEmpty(SL.PromoID)) || (SL.Price_Type != 'R' && SL.SP_Prices.Count == 0))
                            {
                                //Shiny end
                                //       If SL.Price_Type <> "X" Then SA.Line_Price SL, Val(X)



                                if (Conversion.Val(x) > SL.price && !_policyManager.GetPol("U_OVERPLIMIT", user))
                                {


                                    MessageType temp_VbStyle7 = (int)MessageType.Exclamation + MessageType.OkOnly;
                                    errorMesage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, (short)59, null, temp_VbStyle7);
                                    errorMesage.StatusCode = HttpStatusCode.NotAcceptable;
                                    return;
                                }
                                else
                                {
                                    //this.Enabled = false; // properly
                                    if (_policyManager.PRICE_TRACK)
                                    {
                                        _saleService.Track_PriceChange(SL.User, SL.Stock_Code, SL.price, Conversion.Val(x), "SL", (byte)0); // 
                                    }
                                    Line_Price(ref sale, ref SL, Conversion.Val(x));

                                    // sale.Line_Price(SL, Conversion.Val(x));
                                    //this.Enabled = true; //  
                                    if ((!(SL.ProductIsFuel && !SL.IsPropane)) && (string.IsNullOrEmpty(SL.PromoID)))
                                    {

                                        string temp_Policy_Name5 = "PR_REASON";
                                        if (_policyManager.GetPol(temp_Policy_Name5, SL) && returnReason != null)
                                        {
                                            Line_Reason(ref sale, ref SL, returnReason); //Nicolette
                                        }

                                    }
                                    SL.SendToLCD = true;
                                    strCD_Message = _resourceManager.GetResString(offSet, (short)416); // a price change
                                }
                            }
                            else
                            {
                                MessageType temp_VbStyle8 = (int)MessageType.Information + MessageType.OkOnly;
                                errorMesage.MessageStyle = _resourceManager.CreateMessage(offSet, 11, (short)68, SL.Stock_Code, temp_VbStyle8);
                                errorMesage.StatusCode = HttpStatusCode.NotAcceptable;
                                return;
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                if (quantity != SL.Quantity)
                {




                    // Quantity Decimals


                    Quantity = quantity;
                    if (IsValidQuantity(SL.QUANT_DEC, Convert.ToDecimal(Quantity), out errorMesage))
                    {
                        if (SL.Quantity != Quantity && quantity != 0)
                        {
                            string temp_Policy_Name8 = "ACCEPT_RET";
                            //TODO: Ezipin_Removed
                            //
                            //Ezipin products cannot be refunded, the only way is when it was not successful then eziprocessed=false
                            //and could only be refunded from refund sale screen.
                            //if (Quantity < 0 && SL.isEziProduct && SL.EziProcessed)
                            //{
                            //    MsgBoxStyle temp_VbStyle10 = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                            //    Chaps_Main.DisplayMessage(0, (short)8109, temp_VbStyle10, SL.Stock_Code + " " + SL.Description, (byte)0);
                            //    //End - SV
                            //    
                            //}
                            //else
                            //END: Ezipin_Removed
                            if (Quantity < 0 && !_policyManager.GetPol(temp_Policy_Name8, SL))
                            {


                                // Product Returns are not accepted for this Product
                                MessageType temp_VbStyle11 = (int)MessageType.Critical + MessageType.OkOnly;
                                errorMesage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)8109, SL.Stock_Code + " " + SL.Description, temp_VbStyle11);
                                errorMesage.StatusCode = HttpStatusCode.NotAcceptable;
                                return;
                            }
                            else if (Quantity < 0 && !_policyManager.GetPol("U_GIVEREF", user))
                            {
                                // You are not authorized to give refunds
                                MessageType temp_VbStyle12 = (int)MessageType.Critical + MessageType.OkOnly;
                                errorMesage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, (short)8110, null, temp_VbStyle12);
                                errorMesage.StatusCode = HttpStatusCode.Unauthorized;
                                return;
                            }
                            else
                            {
                                //   added next line to avoid Line_Quantity method
                                // to be called twice very fast, if the user click on the quantity field right after he changed the quantity in the current line. Line_Quantity was not done properly.
                                LinesNo = (short)sale.Sale_Lines.Count;
                                //this.Enabled = false;

                                if (!string.IsNullOrEmpty(SL.PromoID) && SL.Price_Type == 'R' && !SL.ProductIsFuel)
                                {
                                    //fs = System.Convert.ToString(Qd == 0 ? "0.0" : ("0." + new string('0', Qd)));
                                    Line_Quantity(ref sale, ref SL,
                                         (float)(Math.Round(quantity, Convert.ToInt32(Qd))), true);
                                    // sale.Line_Quantity(ref SL, (float)(Conversion.Val(string.Format(fs, txtQty[Index].Text))), true);
                                }
                                else
                                {
                                    Line_Quantity(ref sale, ref SL, (float)(Conversion.Val(quantity)), true);
                                }
                                ///                    WriteToLogFile "Line index " & Index & " Qty changed for stock code " & SL.Stock_Code & " to " & txtQty(Index).Text
                                //this.Enabled = true;
                                // Refresh_Lines();



                                //if (LinesNo < sale.Sale_Lines.Count)
                                //{
                                //    SetCurrentLine(MaxLine, MaxLine);
                                //}



                                string temp_Policy_Name9 = "RET_REASON";
                                if (Quantity < 0 && _policyManager.GetPol(temp_Policy_Name9, SL) && returnReason != null)
                                {
                                    Line_Reason(ref sale, ref SL, returnReason); //Nicolette added
                                }


                                SL.SendToLCD = true;
                                strCD_Message = _resourceManager.GetResString(offSet, (short)415); // quantity change
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                //if (Strings.Len(txtStockCode[Index].Text) > 0)
                //{
                //    Format_Fields(ref Index, ref SL);
                //}
                Register register = new Register();
                _mainManager.SetRegister(ref register, sale.Register);
                if (register.Customer_Display && SL.SendToLCD)
                {
                    sale.CustomerDisplay = _mainManager.DisplayMsgLcd(register,
                        Convert.ToString(_mainManager.FormatLcdString(register, SL.Description.Trim(),
                        SL.Quantity >= 0 ? SL.price.ToString() : "-" + Convert.ToString(SL.price))),
                        _mainManager.FormatLcdString(register, "" + strCD_Message,
                        sale.Sale_Totals.Gross.ToString("0.00")));
                    SL.SendToLCD = false;
                }
            }
            else
            {
                errorMesage.MessageStyle = new MessageStyle
                {
                    Message = "Line  Does not exist",
                    MessageType = MessageType.OkOnly
                };
                errorMesage.StatusCode = HttpStatusCode.NotFound;
            }

            //  EnableButtons(); // Nancy to enable only MDI buttons
            Performancelog.Debug($"End,SaleManager,Leaving_Field,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Get Aite store message
        /// </summary>
        /// <returns>Message</returns>
        private string GetAiteStoreMessage()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,GetAiteStoreMessage,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var returnValue = string.Empty;
            var globalMessages = _saleService.GetTaxExemptMessagesFromDbTrans("SM");
            foreach (var message in globalMessages)
            {
                if (message.Length > 0)
                {
                    returnValue = message;
                    _saleService.UpdateTaxExemptMessagesToDbTrans();
                }
                Variables.Sleep(50);
            }
            Performancelog.Debug($"End,SaleManager,GetAiteStoreMessage,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return returnValue;
        }

        /// <summary>
        /// Load Temp Sale 
        /// </summary>
        /// <param name="saleNo"></param>
        /// <param name="tillNumber"></param>
        /// <param name="userCode"></param>
        /// <param name="error"></param>
        private Sale Load_Temp(int saleNo, int tillNumber, string userCode, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Load_Temp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var tempSale = new Sale();
            _saleHeadManager.SetSalePolicies(ref tempSale);
            var sale = UnSuspend_Temp(saleNo, tillNumber, userCode, out error);
            tempSale.Sale_Lines = sale.Sale_Lines;
            tempSale.Sale_Totals.Sale_Taxes = sale.Sale_Totals.Sale_Taxes;
            tempSale.Sale_Num = sale.Sale_Num;
            tempSale.TillNumber = sale.TillNumber;
            tempSale.Register = sale.Register;
            tempSale.Void_Num = sale.Void_Num;
            tempSale.Customer.Code = sale.Customer.Code;
            tempSale.Customer = _customerManager.LoadCustomer(sale.Customer.Code); //Sajan

            tempSale.Customer.LoyaltyCard = sale.Customer.LoyaltyCard;
            tempSale.Customer.LoyaltyExpDate = sale.Customer.LoyaltyExpDate;
            tempSale.Customer.LoyaltyCardSwiped = sale.Customer.LoyaltyCardSwiped;
            tempSale.CouponID = sale.CouponID;
            tempSale.CouponTotal = sale.CouponTotal;
            if (_policyManager.RSTR_PROFILE)
            {
                if (!string.IsNullOrEmpty(tempSale.Customer.LoyaltyCard))
                {
                    tempSale.Customer.CardProfileID = _customerService.GetCustomerCardProfile(sale.Customer.LoyaltyCard);
                }
            }

            tempSale.DeletePrepay = sale.DeletePrepay;
            tempSale.TreatyNumber = sale.TreatyNumber;
            tempSale.HasRebateLine = sale.HasRebateLine;
            tempSale.TreatyName = sale.TreatyName;
            tempSale.Sale_Type = sale.Sale_Type;
            tempSale.Sale_Totals.Invoice_Discount_Type = sale.Sale_Totals.Invoice_Discount_Type;
            tempSale.Sale_Totals.Invoice_Discount = sale.Sale_Totals.Invoice_Discount;
            tempSale.Return_Reason = sale.Return_Reason;
            ReCompute_Totals(ref tempSale);

            Performancelog.Debug($"End,SaleManager,Load_Temp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return tempSale;
        }

        /// <summary>
        /// Clean up Crash 
        /// </summary>
        /// <param name="saleNo"></param>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        private void CleanupTeCrash(int saleNo, byte tillNumber) // shiny added this function to clean up the SITE data and reverse quota if there was a crash recovery.  Then they need to reprocess the same again
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,CleanupTeCrash,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            teTreatyNo oTreatyNo = new teTreatyNo();
            if (_policyManager.TE_Type == "SITE")
            {
                //reverse quota
                var purchaseItems = _taxService.GetPurchaseItems(saleNo, tillNumber);
                foreach (var purchaseItem in purchaseItems)
                {
                    _treatyManager.Init(ref oTreatyNo, purchaseItem.TreatyNo, true);
                    var productType = (mPrivateGlobals.teProductEnum)Enum.Parse(typeof(mPrivateGlobals.teProductEnum), CommonUtility.GetStringValue(purchaseItem.ProductType), true);
                    if (!_treatyManager.AddToQuota(oTreatyNo, ref productType, Convert.ToDouble(Convert.ToDouble(-1 * purchaseItem.Quantity * purchaseItem.UnitsPerPkg))))
                    {
                        WriteToLogFile(" Couldn\'t reverse the quantity for saleno " + System.Convert.ToString(saleNo) + " Error: " + oTreatyNo.GetLastError());
                    }
                }
                //cleanup taxexempt data
                _taxService.DeletePurchaseItem(saleNo, tillNumber);
                // During crashing if data is saved in taxexempt tables when coming back claen up  and reverse quota
            }
            else if (_policyManager.TE_Type == "AITE" || _policyManager.TE_Type == "QITE")
            {
                //reverse quota
                var taxExemptSaleLines = _taxService.GetTaxExemptSaleLines(saleNo, tillNumber);

                foreach (var taxExemptSaleLine in taxExemptSaleLines)
                {
                    var cardHolder = _taxService.GetTaxExemptCardHolder(taxExemptSaleLine.CardHolderID);
                    if (cardHolder != null)
                    {
                        if (taxExemptSaleLine.ProductType == mPrivateGlobals.teProductEnum.eCigar | taxExemptSaleLine.ProductType == mPrivateGlobals.teProductEnum.eCigarette | taxExemptSaleLine.ProductType == mPrivateGlobals.teProductEnum.eLooseTobacco)
                        {
                            cardHolder.TobaccoQuota = cardHolder.TobaccoQuota + Convert.ToSingle(-1 * taxExemptSaleLine.ExemptedTax);
                        }
                        else if (taxExemptSaleLine.ProductType == mPrivateGlobals.teProductEnum.eGasoline | taxExemptSaleLine.ProductType == mPrivateGlobals.teProductEnum.eDiesel | taxExemptSaleLine.ProductType == mPrivateGlobals.teProductEnum.emarkedGas | taxExemptSaleLine.ProductType == mPrivateGlobals.teProductEnum.emarkedDiesel) // hen
                        {
                            cardHolder.GasQuota = cardHolder.GasQuota + Convert.ToSingle(-1 * taxExemptSaleLine.ExemptedTax);
                        }
                        else if (taxExemptSaleLine.ProductType == mPrivateGlobals.teProductEnum.ePropane)
                        {
                            cardHolder.PropaneQuota = cardHolder.PropaneQuota + Convert.ToSingle(-1 * taxExemptSaleLine.ExemptedTax);
                        }
                    }
                }
                // cleanup te data
                _taxService.CleanTaxExemptData(saleNo, tillNumber);
            }

            Performancelog.Debug($"End,SaleManager,CleanupTeCrash,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }

        /// <summary>
        /// Method to validate till and sale
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>True or false</returns>
        private bool ValidateTillAndSale(int tillNumber, int saleNumber, out ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();
            if (!_tillService.IsTillExists(tillNumber))
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = "Invalid Till Number",
                    MessageType = 0
                };
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                return false;
            }

            if (saleNumber <= 0)
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = "Invalid Sale Number",
                    MessageType = 0
                };
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method to validate a sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode"></param>
        /// <param name="errorMessage">Error message</param>
        private void ValidateSale(int saleNumber, int tillNumber, string userCode, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,ValidateSale,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var saleNo = GetCurrentSaleNo(tillNumber, userCode, out errorMessage);
            if (saleNo < saleNumber)
            {
                errorMessage = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Request is invalid ",
                        MessageType = MessageType.OkOnly
                    },
                    StatusCode = HttpStatusCode.NotAcceptable
                };
            }
            Performancelog.Debug($"End,SaleManager,ValidateSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to check if loyalty customer
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <returns>True or false</returns>
        private bool Loyalty_Customer(ref Sale sale)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Loyalty_Customer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var returnValue = false;
            returnValue = sale.USE_LOYALTY && !string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && !string.IsNullOrEmpty(sale.Customer.Loyalty_Code);
            Performancelog.Debug($"End,SaleManager,Loyalty_Customer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// User Can delete Line Items
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private bool CanUserRemoveLineFromSale(string userCode, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,CanUserRemoveLineFromSale,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            errorMessage = new ErrorMessage();
            var user = _loginManager.GetExistingUser(userCode);

            if (!Convert.ToBoolean(_policyManager.GetPol("U_CAN_VOID", user)))
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                MessageType messageType = (int)MessageType.Question + MessageType.YesNo;
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 93, null, messageType),
                    StatusCode = HttpStatusCode.Forbidden
                };
                return false;
            }
            Performancelog.Debug($"End,SaleManager,Save_ProfilePrompt_Temp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return true;

        }

        /// <summary>
        /// Method to remove fuel item
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="error">Error message</param>
        private void RemoveFuelItem(Sale_Line saleLine, out ErrorMessage error)
        {
            error = new ErrorMessage();
            if (saleLine.Prepay)
            {
                if (!_prepayManager.DeletePrepayFromFc(saleLine.pumpID, true, out error))
                {
                    return;
                }
            }
            else
            {
                if (saleLine.Gift_Certificate && saleLine.GiftType == "GiveX")
                {

                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    MessageType temp_VbStyle = (int)MessageType.OkOnly + MessageType.Critical;
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 22, null, temp_VbStyle);
                    error.StatusCode = HttpStatusCode.NotAcceptable;
                    return;
                }
                else
                {
                    if (saleLine.ProductIsFuel && saleLine.Quantity > 0 && (!saleLine.ManualFuel) && (!saleLine.IsPropane) && (!Variables.POSOnly) && saleLine.pumpID != 0)
                    {
                        if (!Variables.gBasket[saleLine.pumpID].CurrentFilled)
                        {
                            if (!_prepayManager.BasketUndo(saleLine.pumpID, saleLine.PositionID, saleLine.GradeID, (float)saleLine.Amount, (float)saleLine.price, saleLine.Quantity, saleLine.Stock_Code, saleLine.MOP, out error))
                            {
                                return;
                            }
                        }
                        else if ((Variables.gBasket[saleLine.pumpID].CurrentFilled && (!saleLine.Prepay)) || !TCPAgent.Instance.IsConnected)
                        {
                            //Chaps_Main.DisplayMessage(this, (short)83, MsgBoxStyle.OkOnly, null, (byte)0);
                            var offSet = _policyManager.LoadStoreInfo().OffSet;
                            error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 83, null, CriticalOkMessageType);
                            error.StatusCode = HttpStatusCode.NotAcceptable;
                            return;
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Method to compute taxes
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="nSign">Signature</param>
        // Add or Subtract taxes from a line in the Sale Tax totals.
        private void Compute_Taxes(ref Sale sale, ref Sale_Line oLine, short nSign)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Compute_Taxes,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var CTX = new Charge_Tax();
            var K_Chg = new K_Charge();
            var Kit = new Line_Kit();
            //var SL = new Sale_Line();
            var SL = _saleLineManager.CreateNewSaleLine();
            var STX = new Sale_Tax();
            var LTX = new Line_Tax();
            var CHG = new Charge();

            double f = 0;
            double f_Included;
            double Incl_Taxes = 0;
            double Net_Taxable = 0;
            string Key = "";
            bool boolComputeTaxes = false;
            double TotalInclRates = 0;
            string strKeyLast = "";
            double PrevTax = 0;

            decimal TotalAddedTax = new decimal();
            double Total_Net_Taxable = 0; //   for HST Rebate
            bool boolTaxRebate = false; //  
            bool boolQualifiedTRFound = false; //  
            TotalAddedTax = 0;


            boolComputeTaxes = Convert.ToBoolean(_policyManager.TAX_COMP);
            boolTaxRebate = Convert.ToBoolean(_policyManager.Tax_Rebate);
            Net_Taxable = (double)oLine.Net_Amount;
            oLine.TE_Amount_Incl = 0;
            oLine.TaxForTaxExempt = false; //  

            //   for HST Rebate
            Total_Net_Taxable = 0;
            boolQualifiedTRFound = false;
            if (boolTaxRebate)
            {
                if (oLine.EligibleTaxRebate || oLine.QualifiedTaxRebate)
                {
                    Total_Net_Taxable = Net_Taxable;
                    // oLine object is not in sale lines collection when taxes are calculated
                }
                boolQualifiedTRFound = oLine.QualifiedTaxRebate;
                foreach (Sale_Line tempLoopVar_SL in sale.Sale_Lines)
                {
                    SL = tempLoopVar_SL;
                    if ((SL.EligibleTaxRebate || SL.QualifiedTaxRebate) && (oLine.Line_Num != SL.Line_Num))
                    {
                        Total_Net_Taxable = Total_Net_Taxable + (double)SL.Net_Amount;
                        if (SL.QualifiedTaxRebate)
                        {
                            boolQualifiedTRFound = true;
                        }
                    }
                }
                //   added Abs to fix issue reported by Mr. Gas in returns (email Dec 17, 2013)
                boolTaxRebate = boolTaxRebate &&
                                (Math.Abs((decimal)Total_Net_Taxable) <= _policyManager.TX_RB_AMT) &&
                                (oLine.EligibleTaxRebate || oLine.QualifiedTaxRebate) && boolQualifiedTRFound;
            }
            //   end

            foreach (Line_Tax tempLoopVar_LTX in oLine.Line_Taxes)
            {
                LTX = tempLoopVar_LTX;
                if (LTX.Tax_Included)
                {
                    Key = LTX.Tax_Name + LTX.Tax_Code;
                    if (sale.Sale_Totals.Sale_Taxes.Count > 0)
                    {
                        STX = sale.Sale_Totals.Sale_Taxes[Key];
                        if (boolComputeTaxes)
                        {
                            if (TotalInclRates == 0)
                            {
                                TotalInclRates = TotalInclRates + STX.Tax_Rate;
                            }
                            else
                            {
                                TotalInclRates = TotalInclRates + STX.Tax_Rate + TotalInclRates * STX.Tax_Rate / 100;
                            }
                        }
                        else
                        {
                            TotalInclRates = TotalInclRates + STX.Tax_Rate;
                        }
                        strKeyLast = Key;
                    }
                }
            }

            if (TotalInclRates != 0)
            {
                Incl_Taxes = modGlobalFunctions.Round(Net_Taxable * (TotalInclRates / 100) / (1 + TotalInclRates / 100), 2);
            }

            // Apply Taxes to the Sale Items
            foreach (Line_Tax tempLoopVar_LTX in oLine.Line_Taxes)
            {
                LTX = tempLoopVar_LTX;
                Key = LTX.Tax_Name + LTX.Tax_Code;
                if (sale.Sale_Totals.Sale_Taxes.Count > 0)
                {

                    STX = sale.Sale_Totals.Sale_Taxes[Key];
                    if (sale.ApplyTaxes ||
                        (!sale.ApplyTaxes && LTX.Tax_Name.ToUpper() == oLine.TE_COLLECTTAX.Trim().ToUpper()))
                    {
                        if (!sale.ApplyTaxes && LTX.Tax_Name.ToUpper() == oLine.TE_COLLECTTAX.Trim().ToUpper() &&
                            !oLine.TaxForTaxExempt)
                        {
                            oLine.TaxForTaxExempt = true; //  
                        }
                        if (!sale.ApplyTaxes && LTX.Tax_Name.ToUpper() == oLine.TE_COLLECTTAX.Trim().ToUpper() &&
                            !sale.TaxForTaxExempt)
                        {
                            sale.TaxForTaxExempt = true; //  
                        }
                        if (LTX.Tax_Included)
                        {
                            LTX.Tax_Incl_Total = (float)(oLine.Net_Amount * nSign);
                            if (Key == strKeyLast)
                            {
                                if (boolComputeTaxes)
                                {
                                    LTX.Tax_Incl_Amount =
                                        (float)
                                        modGlobalFunctions.Round(
                                            (Net_Taxable - Incl_Taxes + PrevTax) * STX.Tax_Rate / 100 * nSign, 2);
                                }
                                else
                                {
                                    LTX.Tax_Incl_Amount = (float)(Incl_Taxes - PrevTax);
                                }
                            }
                            else
                            {
                                if (boolComputeTaxes)
                                {
                                    LTX.Tax_Incl_Amount =
                                        (float)
                                        modGlobalFunctions.Round((Net_Taxable - Incl_Taxes) * STX.Tax_Rate / 100 * nSign, 2);
                                }
                                else
                                {
                                    LTX.Tax_Incl_Amount =
                                        (float)
                                        modGlobalFunctions.Round(Incl_Taxes * STX.Tax_Rate / TotalInclRates * nSign, 2);
                                }
                                PrevTax = PrevTax + LTX.Tax_Incl_Amount;
                            }
                            //  
                            if (boolTaxRebate)
                            {
                                LTX.Tax_Rebate =
                                    (decimal)modGlobalFunctions.Round(Net_Taxable * STX.Tax_Rebate_Rate / 100 * nSign, 2);
                                STX.Tax_Rebate = STX.Tax_Rebate + LTX.Tax_Rebate;
                            }
                            else
                            {
                                LTX.Tax_Rebate = 0;
                                ///                    STX.Tax_Rebate = 0   ' items
                            }
                            //   end
                            STX.Tax_Included_Total = STX.Tax_Included_Total + (decimal)LTX.Tax_Incl_Total;
                            STX.Tax_Included_Amount = STX.Tax_Included_Amount + (decimal)LTX.Tax_Incl_Amount;
                            STX.Rebatable_Amount = STX.Rebatable_Amount + (decimal)Net_Taxable; //  
                            if (boolComputeTaxes)
                            {
                                if (Key == strKeyLast)
                                {
                                    STX.Taxable_Amt_ForIncluded = (decimal)(Net_Taxable - Incl_Taxes + PrevTax);
                                }
                                else
                                {
                                    STX.Taxable_Amt_ForIncluded = (decimal)(Net_Taxable - Incl_Taxes);
                                }
                            }
                            //  
                            if (_policyManager.TAX_EXEMPT_GA && sale.EligibleTaxEx && oLine.EligibleTaxEx)
                            {
                                STX.Tax_Exemption_GA_Incl = STX.Tax_Exemption_GA_Incl + (decimal)LTX.Tax_Incl_Amount;
                                STX.Tax_Included_Total = STX.Tax_Included_Total - (decimal)LTX.Tax_Incl_Total;
                                STX.Tax_Included_Amount = STX.Tax_Included_Amount - (decimal)LTX.Tax_Incl_Amount;
                                oLine.TE_Amount_Incl = oLine.TE_Amount_Incl + (decimal)LTX.Tax_Incl_Amount;
                                LTX.Tax_Incl_Amount = 0;
                                LTX.Tax_Incl_Total = 0;
                            }
                            else
                            {
                                STX.Tax_Exemption_GA_Incl = 0;
                                oLine.TE_Amount_Incl = 0;
                            }
                            //   end
                        }
                        else
                        {
                            LTX.Taxable_Amount = (float)(Net_Taxable * nSign);
                            //  from Adam)
                            // . since it depend on some product price and tax and policy it is not easy test and say it is working in all combination
                            LTX.Tax_Added_Amount = LTX.Taxable_Amount * LTX.Tax_Rate / 100;
                            //round(LTX.Tax_Added_Amount = LTX.Taxable_Amount * LTX.Tax_Rate / 100# ,2)' LTX.Taxable_Amount * LTX.Tax_Rate / 100# ' , 2)  ' Nicolette added to record taxes for each line in a sale
                            STX.Taxable_Amount =
                                (decimal)modGlobalFunctions.Round((double)STX.Taxable_Amount + Net_Taxable * nSign, 2);
                            //  to fix EKO issue of 1 peeny difference in recipt ( compute on Tax) 'stx.Taxable_Amount = stx.Taxable_Amount + Net_Taxable * nSign
                            STX.Tax_Added_Amount =
                                (decimal)
                                modGlobalFunctions.Round((float)STX.Taxable_Amount * STX.Tax_Rate / 100, 2);
                            //  
                            if (boolTaxRebate)
                            {
                                LTX.Tax_Rebate =
                                    (decimal)modGlobalFunctions.Round(Net_Taxable * STX.Tax_Rebate_Rate / 100 * nSign, 2);
                                STX.Tax_Rebate = STX.Tax_Rebate + LTX.Tax_Rebate;
                            }
                            else
                            {
                                LTX.Tax_Rebate = 0;
                                ///                    STX.Tax_Rebate =  0  ' items
                            }
                            STX.Rebatable_Amount = STX.Rebatable_Amount + (decimal)Net_Taxable; //  
                                                                                                //   end
                                                                                                //   to implement policy "Compute tax on taxes?"
                            if (boolComputeTaxes)
                            {
                                Net_Taxable = Net_Taxable + LTX.Tax_Added_Amount;
                            }

                            TotalAddedTax = TotalAddedTax + (decimal)LTX.Tax_Added_Amount;

                            //  
                            if (_policyManager.TAX_EXEMPT_GA && sale.EligibleTaxEx && oLine.EligibleTaxEx)
                            {
                                STX.Tax_Exemption_GA_Added = STX.Tax_Exemption_GA_Added + (decimal)LTX.Tax_Added_Amount;
                                LTX.Taxable_Amount = 0;
                                LTX.Tax_Added_Amount = 0;
                                STX.Taxable_Amount =
                                    (decimal)modGlobalFunctions.Round((double)STX.Taxable_Amount - Net_Taxable * nSign, 2);
                                STX.Tax_Added_Amount =
                                    (decimal)
                                    modGlobalFunctions.Round((float)STX.Taxable_Amount * STX.Tax_Rate / 100.0D, 2);
                            }
                            else
                            {
                                STX.Tax_Exemption_GA_Added = 0;
                            }
                            //   end
                        }
                    }
                    else
                    {
                        LTX.Tax_Incl_Total = 0;
                        LTX.Tax_Incl_Amount = 0;
                        LTX.Taxable_Amount = 0;
                        LTX.Tax_Added_Amount = 0;
                        //   added If Not mvarTaxForTaxExempt for tax on tax exempt customers
                        if (!sale.TaxForTaxExempt)
                        {
                            STX.Tax_Included_Total = 0;
                            STX.Tax_Included_Amount = 0;
                            STX.Taxable_Amount = 0;
                            STX.Tax_Added_Amount = 0;
                        }
                        //   end
                    }
                }
            }
            // Apply Taxes on Associated Charges
            foreach (Charge tempLoopVar_CHG in oLine.Charges)
            {
                CHG = tempLoopVar_CHG;

                Net_Taxable = CHG.Charge_Price * oLine.Quantity;

                foreach (Charge_Tax tempLoopVar_CTX in CHG.Charge_Taxes)
                {
                    CTX = tempLoopVar_CTX;
                    Key = CTX.Tax_Name + CTX.Tax_Code;
                    if (sale.Sale_Totals.Sale_Taxes.Count > 0)
                    {

                        STX = sale.Sale_Totals.Sale_Taxes[Key];
                        if (sale.ApplyTaxes)
                        {
                            if (CTX.Tax_Included)
                            {
                                f = (100.0D + CTX.Tax_Rate) / 100.0D;
                                CTX.Tax_Incl_Total = CHG.Charge_Price * oLine.Quantity * nSign;
                                CTX.Tax_Incl_Amount = (float)(Net_Taxable * (1.0D - 1.0D / f) * nSign);
                                STX.Tax_Included_Total = STX.Tax_Included_Total + (decimal)CTX.Tax_Incl_Total;
                                STX.Tax_Included_Amount = STX.Tax_Included_Amount + (decimal)CTX.Tax_Incl_Amount;
                                //   to implement policy "Compute tax on taxes?"
                                if (boolComputeTaxes)
                                {
                                    Net_Taxable = Net_Taxable + CTX.Tax_Incl_Amount;
                                }
                            }
                            else
                            {
                                CTX.Taxable_Amount = (float)(Net_Taxable * nSign);
                                CTX.Tax_Added_Amount = (float)(CTX.Taxable_Amount * CTX.Tax_Rate / 100.0D);
                                //  , 2) ' Nicolette added
                                STX.Taxable_Amount =
                                    (decimal)modGlobalFunctions.Round((double)STX.Taxable_Amount + Net_Taxable * nSign, 2);
                                //  
                                STX.Tax_Added_Amount =
                                    (decimal)
                                    modGlobalFunctions.Round((float)STX.Taxable_Amount * STX.Tax_Rate / 100.0D, 2);
                                //   to implement policy "Compute tax on taxes?"
                                if (boolComputeTaxes)
                                {
                                    Net_Taxable = Net_Taxable + CTX.Tax_Added_Amount;
                                }

                                TotalAddedTax = TotalAddedTax + (decimal)CTX.Tax_Added_Amount;

                            }
                        }
                        else
                        {
                            CTX.Tax_Incl_Total = 0;
                            CTX.Tax_Incl_Amount = 0;
                            STX.Tax_Included_Total = 0;
                            STX.Tax_Included_Amount = 0;
                            CTX.Taxable_Amount = 0;
                            CTX.Tax_Added_Amount = 0;
                            STX.Taxable_Amount = 0;
                            STX.Tax_Added_Amount = 0;
                        }
                    }
                }
            }

            // Apply Taxes on Associated Charges in Kit Items
            foreach (Line_Kit tempLoopVar_Kit in oLine.Line_Kits)
            {
                Kit = tempLoopVar_Kit;
                foreach (K_Charge tempLoopVar_K_Chg in Kit.K_Charges)
                {
                    K_Chg = tempLoopVar_K_Chg;

                    Net_Taxable = K_Chg.Charge_Price * oLine.Quantity * Kit.Kit_Item_Qty;

                    foreach (Charge_Tax tempLoopVar_CTX in K_Chg.Charge_Taxes)
                    {
                        CTX = tempLoopVar_CTX;
                        Key = CTX.Tax_Name + CTX.Tax_Code;
                        if (sale.Sale_Totals.Sale_Taxes.Count > 0)
                        {

                            STX = sale.Sale_Totals.Sale_Taxes[Key];
                            // exemption
                            if (sale.ApplyTaxes && !(sale.EligibleTaxEx && oLine.EligibleTaxEx))
                            {
                                if (CTX.Tax_Included)
                                {
                                    f = (100.0D + CTX.Tax_Rate) / 100.0D;
                                    CTX.Tax_Incl_Total =
                                        (float)(K_Chg.Charge_Price * oLine.Quantity * Kit.Kit_Item_Qty * nSign);
                                    CTX.Tax_Incl_Amount = (float)(Net_Taxable * (1.0D - 1.0D / f) * nSign);
                                    STX.Tax_Included_Total = STX.Tax_Included_Total + (decimal)CTX.Tax_Incl_Total;
                                    STX.Tax_Included_Amount = STX.Tax_Included_Amount + (decimal)CTX.Tax_Incl_Amount;
                                    //   to implement policy "Compute tax on taxes?"
                                    if (boolComputeTaxes)
                                    {
                                        Net_Taxable = Net_Taxable + CTX.Tax_Incl_Amount;
                                    }
                                }
                                else
                                {
                                    CTX.Taxable_Amount = (float)(Net_Taxable * nSign);
                                    CTX.Tax_Added_Amount = (float)(CTX.Taxable_Amount * CTX.Tax_Rate / 100.0D);
                                    // , 2) ' Nicolette added
                                    STX.Taxable_Amount =
                                                                        (decimal)
                                                                        modGlobalFunctions.Round((double)STX.Taxable_Amount + Net_Taxable * nSign, 2);
                                    // 
                                    STX.Tax_Added_Amount =
                                        (decimal)
                                        modGlobalFunctions.Round(
                                            (float)STX.Taxable_Amount * STX.Tax_Rate / 100.0D, 2);
                                    //   to implement policy "Compute tax on taxes?"
                                    if (boolComputeTaxes)
                                    {
                                        Net_Taxable = Net_Taxable + CTX.Tax_Added_Amount;
                                    }


                                    TotalAddedTax = TotalAddedTax + (decimal)CTX.Tax_Added_Amount;

                                }
                            }
                            else
                            {
                                CTX.Tax_Incl_Total = 0;
                                CTX.Tax_Incl_Amount = 0;
                                STX.Tax_Included_Total = 0;
                                STX.Tax_Included_Amount = 0;
                                CTX.Taxable_Amount = 0;
                                CTX.Tax_Added_Amount = 0;
                                STX.Taxable_Amount = 0;
                                STX.Tax_Added_Amount = 0;
                            }
                        }
                    }
                }
            }


            oLine.AddedTax = TotalAddedTax;

            STX = null;
            Performancelog.Debug($"End,SaleManager,Compute_Taxes,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to update paid by card for temporary lines
        /// </summary>
        /// <param name="sale">Sale</param>
        private void UpdatePaidByCardForTempLines(Sale sale)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,UpdatePaidByCardForTempLines,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            //foreach (Sale_Line tempLoopVarMSl in sale.Sale_Lines)
            //{
            //    var mSl = tempLoopVarMSl;
            //    var saleLine = _saleService.GetSaleLineFromDbTemp(sale.Sale_Num, sale.TillNumber, mSl.Line_Num, mSl.User);
            //    if (saleLine != null)
            //    {
            //        saleLine.PaidByCard = mSl.PaidByCard;
            //        // rsSaleLine.Fields["PaidByCard"].Value = mSL.PaidByCard;
           /////_saleService.UpdateSaleLineToDbTemp(saleLine, sale.TillNumber, sale.Sale_Num);
            //        //rsSaleLine.Update();
            //    }
            //}
            _saleService.SaveSale(sale.TillNumber, sale.Sale_Num, sale);
            Performancelog.Debug($"End,SaleManager,UpdatePaidByCardForTempLines,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        //Jim, Dec 1 2014 end




        //                             ByVal Quantity As Currency)
        /// <summary>
        /// Method to update stock levels
        /// </summary>
        /// <param name="kitItem">Kit item</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="saleTType">Sale type</param>
        /// <param name="saleLine">Sale line</param>
        /// <param name="TaxExempt">Tax exempt</param>
        private void UpdateStockLevels(string kitItem, decimal quantity, string saleTType, ref Sale_Line saleLine, bool TaxExempt = false)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,UpdateStockLevels,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            //
            string strSql = "";
            // If it isn't a Void or Cancel and it is a stock type that we
            // keep track of (i.e. 'V' or 'O') then update the In-Stock and
            // Available for sale quantities.
            // Sale_Line SL = default(Sale_Line);
            string Return_Policy;
            string StockCode = saleLine.Stock_Code;
            // decimal Quantity = (decimal)saleLine.Quantity;
            var dataSource = DataSource.CSCMaster;
            if (saleTType != "VOID" && saleTType != "CANCEL")
            {
                // SL = new Sale_Line();
                var error = new ErrorMessage();
                saleLine.Stock_Type = _stockService.GetStockItem(saleLine.Stock_Code).StockType;
                if (saleLine.Stock_Type == 'V' || saleLine.Stock_Type == 'O')
                {


                    //  For AITE - need to track inventory separately for TE product sales
                    //But other TE type there is no requirement for tax agency. it is upto the customer.  so we are going to add a policy under taxexempt - Track_TEINV- default will be no for existing customers

                    if (TaxExempt && (_policyManager.GetPol("TE_Type", null) == "AITE" || (_policyManager.GetPol("TE_Type", null) != "AITE" && _policyManager.GetPol("TRACK_TEINV", null)))) //  - Only for AITE we need to track inventory separately for TE products 'If TaxExempt Then
                    {
                        // 
                        //  - to use direct update without recordset



                        ///                            " where ProductKey='" & StockCode & "'", _
                        ///                            dbMaster, adOpenForwardOnly)





















                        //  - To avoid update error - when acessing same stock_code from 2 tills
                        if (quantity < 0)
                        {
                            string temp_Policy_Name = "ADD_RET_TO";
                            Return_Policy = Convert.ToString(_policyManager.GetPol(temp_Policy_Name, saleLine)); //SL.ADD_RET_TO
                            if (Return_Policy == "IN STOCK")
                            {
                                strSql = "Update ProductTaxExempt " + " SET InStock = InStock - " + Convert.ToString(quantity) + ", Available =  Available - " + Convert.ToString(quantity) + ", LastSaleTime = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where ProductKey = \'" + StockCode + "\' ";
                            }
                            else if (Return_Policy == "HOLD")
                            {
                                strSql = "Update ProductTaxExempt " + " SET hold = hold - " + Convert.ToString(quantity) + ", LastSaleTime = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where ProductKey = \'" + StockCode + "\' ";
                            }
                            else
                            {
                                strSql = "Update ProductTaxExempt " + " SET Waste = Waste - " + Convert.ToString(quantity) + ", LastSaleTime = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where ProductKey = \'" + StockCode + "\' ";
                            }
                        }
                        else
                        {
                            strSql = "Update ProductTaxExempt " + " SET InStock = InStock - " + Convert.ToString(quantity) + ", Available =  Available - " + Convert.ToString(quantity) + ", LastSaleTime = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where ProductKey = \'" + StockCode + "\' ";
                        }

                        _saleService.UpdateTaxExempt(strSql, dataSource);
                        // 



                    }
                    else
                    {

                        //  when updating with recordset - giving error "Error Row cannot be located for updating. Some values may have been changed since it was last read"- reason for this eror is 2 tills update the same stock_code at same time
























                        // Giving the update eror with global or local recordset, so changed to update directly
                        if (quantity < 0)
                        {
                            string temp_Policy_Name2 = "ADD_RET_TO";
                            Return_Policy = Convert.ToString(_policyManager.GetPol(temp_Policy_Name2, saleLine)); //SL.ADD_RET_TO
                            if (Return_Policy == "IN STOCK")
                            {
                                strSql = "Update Stock_Br " + " SET IN_Stock = In_stock - " + Convert.ToString(quantity) + ", Avail =  Avail - " + Convert.ToString(quantity) + ", Last_Sale = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where stock_code = \'" + StockCode + "\' ";
                            }
                            else if (Return_Policy == "HOLD")
                            {
                                strSql = "Update Stock_Br " + " SET hold = hold - " + Convert.ToString(quantity) + ", Last_Sale = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where stock_code = \'" + StockCode + "\' ";
                            }
                            else
                            {
                                strSql = "Update Stock_Br " + " SET Waste = Waste - " + Convert.ToString(quantity) + ", Last_Sale = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where stock_code = \'" + StockCode + "\' ";
                            }
                        }
                        else
                        {
                            strSql = "Update Stock_Br " + " SET IN_Stock = In_stock - " + Convert.ToString(quantity) + ", Avail =  Avail - " + Convert.ToString(quantity) + ", Last_Sale = \'" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\' " + " where stock_code = \'" + StockCode + "\' ";
                        }
                        // object null_object2 = null;
                        _saleService.UpdateStockBr(strSql, dataSource);
                        //End - dec11, 2009

                    }
                }
            }
            Performancelog.Debug($"End,SaleManager,UpdateStockLevels,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to save deleted line
        /// </summary>
        /// <param name="oLine">Sale line</param>
        /// <param name="Del_Type">Deletion type</param>
        private void Save_Deleted_Line(ref Sale_Line oLine, string Del_Type)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Save_Deleted_Line,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            _saleService.SaveDeletedLineToDbTill(ref oLine, Del_Type);
            Performancelog.Debug($"End,SaleManager,Save_Deleted_Line,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to validate coupon
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="i">index</param>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        private bool ValidateCoupon(ref Sale sale, short i, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,ValidateCoupon,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            error = new ErrorMessage();
            bool returnValue = false;
            short j = 0;
            bool blFind; //Find required item for the coupon
            string tmpStock = "";
            short cnt = 0;
            var saleTotal = sale.Sale_Totals;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            //Validate this coupon
            cnt = 0;
            //Coupon's qty could be more than 1; so we need to validate all of them
            while (cnt < Math.Abs((short)sale.Sale_Lines[i].Quantity))
            {
                //Go through all the items in sales screen
                blFind = false;
                for (j = 1; j <= sale.Sale_Lines.Count; j++)
                {
                    //Check if there is an item for this coupon
                    if (sale.Sale_Lines[j].Dept == sale.Sale_Lines[i].Dept &&
                        sale.Sale_Lines[j].Sub_Dept == sale.Sale_Lines[i].Sub_Dept &&
                        sale.Sale_Lines[j].Sub_Detail == sale.Sale_Lines[i].Sub_Detail &&
                        sale.Sale_Lines[j].Processed == false && sale.Sale_Lines[j].Stock_Type != 'P')
                    {

                        if (sale.Sale_Lines[j].Quantity == 1)
                        {
                            sale.Sale_Lines[i].Processed = true;
                            sale.Sale_Lines[j].Processed = true;
                        }
                        else if (sale.Sale_Lines[j].Quantity > 1)
                        {
                            if (sale.Sale_Lines[j].AvailableQty == 0)
                            {
                                //Processed=False that means this item is in process for the first time
                                //We need to set availableqty to quantity
                                sale.Sale_Lines[j].AvailableQty = (short)Math.Abs(sale.Sale_Lines[j].Quantity);
                            }
                            sale.Sale_Lines[j].AvailableQty = (short)(sale.Sale_Lines[j].AvailableQty - 1);
                            if (sale.Sale_Lines[j].AvailableQty == 0)
                            {
                                //processed all quantities for this item
                                sale.Sale_Lines[i].Processed = true; //coupon processed
                                sale.Sale_Lines[j].Processed = true; //all qty for this item completely processed
                            }
                            else
                            {
                                sale.Sale_Lines[i].Processed = true; //coupon processed
                                sale.Sale_Lines[j].Processed = false; //item - still more qty
                            }
                        }
                        blFind = true; //found one item for this coupon
                        break;
                    }
                }
                if (blFind == false)
                {
                    if (cnt == 0)
                    {
                        //coupon qty is 1 and required item is not found
                        tmpStock = sale.Sale_Lines[i].Stock_Code;
                        SetGross(ref saleTotal, sale.Sale_Totals.Net - sale.Sale_Lines[i].Amount);
                        sale.Sale_Lines.Remove(i); //remove this coupon from sales screen
                        if (sale.Sale_Lines.Count == 0)
                        {
                            SetGross(ref saleTotal, 0);
                        }

                        error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 1154, tmpStock);
                        returnValue = false;
                        return returnValue;
                    }
                    //coupon qty is more than 1, all previous qty were validated but failed at this quantity
                    tmpStock = sale.Sale_Lines[i].Stock_Code;
                    var saleLineSetter = sale.Sale_Lines[i];
                    if (sale.Sale_Lines[i].Quantity < 0)
                    {
                        //qty is less then zero, so price is positive
                        _saleLineManager.SetQuantity(ref saleLineSetter, -1 * cnt);
                        sale.Sale_Lines[i].Quantity = saleLineSetter.Quantity;
                        _saleLineManager.SetAmount(ref saleLineSetter, (decimal)(sale.Sale_Lines[i].price * -1 * cnt));
                        sale.Sale_Lines[i].Amount = saleLineSetter.Amount;
                        sale.Sale_Lines[i].Net_Amount = saleLineSetter.Net_Amount;
                        sale.Sale_Lines[i].Discount_Rate = saleLineSetter.Discount_Rate;
                    }
                    else
                    {
                        //qty is more than zero, so price is negative
                        _saleLineManager.SetQuantity(ref saleLineSetter, cnt);
                        sale.Sale_Lines[i].Quantity = saleLineSetter.Quantity;
                        _saleLineManager.SetAmount(ref saleLineSetter, (decimal)(sale.Sale_Lines[i].price * cnt));
                        sale.Sale_Lines[i].Amount = saleLineSetter.Amount;
                        sale.Sale_Lines[i].Net_Amount = saleLineSetter.Net_Amount;
                        sale.Sale_Lines[i].Discount_Rate = saleLineSetter.Discount_Rate;

                    }
                    //Msgbox "No all coupons can be processed"
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 1152, tmpStock);
                    returnValue = true;
                    return returnValue;
                }
                cnt++;
            }
            sale.Sale_Totals.Gross = saleTotal.Gross;
            sale.Sale_Totals.TotalLabel = saleTotal.TotalLabel;
            sale.Sale_Totals.SummaryLabel = saleTotal.SummaryLabel;
            returnValue = true;
            Performancelog.Debug($"End,SaleManager,ValidateCoupon,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        // Add or Subtract associated charges for the current line from sale totals.
        /// <summary>
        /// Method to compute charges
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="nSign">Signature</param>
        private void Compute_Charges(ref Sale sale, ref Sale_Line oLine, short nSign)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Compute_Charges,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            Charge CHG = default(Charge);
            Line_Kit Kit = default(Line_Kit);
            K_Charge KCG = default(K_Charge);
            decimal TotalLineCharge = new decimal();

            TotalLineCharge = 0;

            foreach (Charge tempLoopVar_CHG in oLine.Charges)
            {
                CHG = tempLoopVar_CHG;
                if (sale.ApplyCharges)
                {
                    sale.Sale_Totals.Charge = sale.Sale_Totals.Charge + CHG.Charge_Price * oLine.Quantity * nSign;
                    TotalLineCharge = TotalLineCharge + (decimal)(CHG.Charge_Price * oLine.Quantity * nSign);

                    oLine.Associate_Amount = TotalLineCharge;
                    // items
                }
                else
                {
                    sale.Sale_Totals.Charge = 0;
                }
            }

            foreach (Line_Kit tempLoopVar_Kit in oLine.Line_Kits)
            {
                Kit = tempLoopVar_Kit;
                foreach (K_Charge tempLoopVar_KCG in Kit.K_Charges)
                {
                    KCG = tempLoopVar_KCG;
                    if (sale.ApplyCharges)
                    {
                        sale.Sale_Totals.Charge = sale.Sale_Totals.Charge +
                                                  KCG.Charge_Price * Kit.Kit_Item_Qty * oLine.Quantity * nSign;
                        TotalLineCharge = TotalLineCharge +
                                          (decimal)(KCG.Charge_Price * Kit.Kit_Item_Qty * oLine.Quantity * nSign);

                    }
                    else
                    {
                        sale.Sale_Totals.Charge = 0;
                    }
                }
            }

            oLine.TotalCharge = TotalLineCharge;
            Performancelog.Debug($"End,SaleManager,Compute_Charges,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }

        /// <summary>
        /// Method to find minimum value between two numbers
        /// </summary>
        /// <param name="a">First value</param>
        /// <param name="b">Second value</param>
        /// <returns>Minimum value</returns>
        private float MinVal(float a, float b)

        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,MinVal,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            float returnValue = 0;
            returnValue = a < b ? a : b;
            Performancelog.Debug($"End,SaleManager,MinVal,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return returnValue;
        }

        /// <summary>
        /// Method to get array of quantity
        /// </summary>
        /// <param name="SL">Sale line</param>
        /// <param name="sale">Sale</param>
        /// <param name="Quantity">Quantity></param>
        /// <returns>Quantity</returns>
        private float[,] PQuantity(Sale_Line SL, Sale sale, float Quantity)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,PQuantity,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            float[,] Prices = new float[1, 1];
            const short Qty = 1;
            const short PRI = 2;
            const short AMT = 3;
            Sale_Line SLine;
            short ns = 0;
            short n = 0;
            short i = 0;
            decimal Reg_Price = new decimal();
            string Price_Type;
            string Price_Units;
            short m = 0;
            float RP_Qty = 0;
            decimal Qty_Remaining = new decimal();
            decimal Max_Quantity = new decimal();
            float Segments = 0;
            short j = 0;
            short k = 0;
            decimal Qty_Used = new decimal();
            decimal Qty_Reg = new decimal();
            float DiffQty;

            ns = Quantity < 0 ? (short)-1 : (short)1;
            Quantity = Math.Abs((short)Quantity);
            // Nancy changed following line: for fuel sale we can have 0<quantity<1
            if (Quantity < 1)
            {
                Prices = new float[2, 4];
            }
            else
            {
                Prices = new float[(int)Quantity + 1, 4];
            }

            Reg_Price = (decimal)SL.Regular_Price;
            Price_Type = SL.Price_Type.ToString();
            Price_Units = SL.Price_Units.ToString();

            // Initialize everything to zero.
            for (n = 1; n <= Information.UBound(Prices, Qty); n++)
            {
                Prices[n, Qty] = 0;
                Prices[n, PRI] = 0;
                Prices[n, AMT] = 0;
            }

            // If the price number isn't 1 (i.e. Regular Price) then
            // none of the special pricing applies.
            float Remaining_Qty = 0;
            float Best_Price = 0;
            string Q = "";
            if (SL.Price_Number != 1)
            {
                Prices[1, Qty] = Quantity * ns;
                Prices[1, PRI] = (float)SL.price;
                Prices[1, AMT] = (float)(Quantity * SL.price * ns);
                m = 1;

            }
            else
            {
                if (SL.SP_Prices == null)
                {

                    // No Special Pricing defined for this item.
                    Prices[1, Qty] = Quantity * ns;
                    Prices[1, PRI] = (float)Reg_Price;
                    Prices[1, AMT] = Quantity * (float)Reg_Price * ns;
                    m = 1;

                }
                else
                {

                    switch (SL.Price_Type)
                    {

                        case 'R':

                            if (string.IsNullOrEmpty(SL.PromoID) || SL.ProductIsFuel) //  
                            {
                                // Regular Price
                                Prices[1, Qty] = Quantity * ns;
                                Prices[1, PRI] = (float)Reg_Price;
                                Prices[1, AMT] = Quantity * (float)Reg_Price * ns;
                                m = 1;

                                // REGULAR PRICING WITH PROMO   '  
                            }
                            else
                            {
                                k = (short)SL.SP_Prices.Count;
                                if (k > 0)
                                {
                                    Qty_Reg = (decimal)(SL.SP_Prices[1].From_Quantity - 1);
                                    Max_Quantity = (decimal)SL.SP_Prices[SL.SP_Prices.Count].To_Quantity;
                                    m = 1;
                                    Qty_Used = 0;
                                    Qty_Remaining = (decimal)Quantity;

                                    // Sell everything before the first quantity at regular price
                                    if (Qty_Reg > 0)
                                    {
                                        Prices[m, Qty] = MinVal((float)Qty_Remaining, (float)Qty_Reg);
                                        Prices[m, PRI] = (float)Reg_Price;
                                        Prices[m, AMT] = Prices[m, Qty] * Prices[m, PRI] * ns;
                                        Qty_Used = Qty_Used + (decimal)Prices[m, Qty];
                                        m++;
                                    }
                                    Qty_Remaining = Qty_Remaining - Qty_Used;

                                    // Apply the price structure once.
                                    if (Qty_Remaining > 0)
                                    {
                                        for (j = 1; j <= k; j++)
                                        {
                                            SP_Price with_1 = SL.SP_Prices[j];

                                            Prices[m, Qty] = MinVal((float)Qty_Remaining,
                                                with_1.To_Quantity - with_1.From_Quantity + 1);
                                            Prices[m, PRI] = Price_Units == "%"
                                                ? Convert.ToSingle(((float)Reg_Price * (1 - with_1.Price / 100)).ToString("#0.0000"))
                                                : Convert.ToSingle(with_1.Price.ToString("#0.0000"));
                                            Prices[m, AMT] = Prices[m, Qty] * Prices[m, PRI] * ns;

                                            Qty_Remaining = Qty_Remaining - (decimal)Prices[m, Qty];
                                            m++;
                                            if (Qty_Remaining <= 0)
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    // Sell everything above the maximum quantity at regular price.
                                    if (Qty_Remaining > 0)
                                    {
                                        Prices[m, Qty] = (float)Qty_Remaining;
                                        Prices[m, PRI] = (float)Reg_Price;
                                        Prices[m, AMT] = Prices[m, Qty] * Prices[m, PRI] * ns;
                                    }
                                    else
                                    {
                                        m--;
                                    }
                                }
                                else // from k > 0 condition
                                {
                                    // Apply regular price
                                    m = 1;
                                    Prices[m, Qty] = Quantity * ns;
                                    Prices[m, PRI] = (float)Reg_Price;
                                    Prices[m, AMT] = Prices[m, Qty] * (float)Reg_Price;
                                }
                            }
                            break;
                        //  

                        case 'F':
                            // First Unit Pricing
                            n = (short)SL.SP_Prices.Count;
                            // Find the largest quantity break that he has reached and set the price
                            // specified by that quantity break.
                            while (!(n == 0))
                            {
                                SP_Price with_2 = SL.SP_Prices[n];
                                if (with_2.From_Quantity <= Quantity)
                                {
                                    Prices[1, Qty] = Quantity * ns;
                                    Prices[1, PRI] = Price_Units == "%"
                                        ? (float)Reg_Price * (1 - with_2.Price / 100)
                                        : with_2.Price;
                                    Prices[1, AMT] = Quantity * Prices[1, PRI] * ns;
                                    break;
                                }
                                n--;
                            }

                            // If he hasn't reached any quantity break then use regular price.
                            if (Prices[1, Qty] == 0)
                            {
                                Prices[1, Qty] = Quantity * ns;
                                Prices[1, PRI] = (float)Reg_Price;
                                Prices[1, AMT] = Quantity * (float)Reg_Price * ns;
                            }
                            m = 1;
                            break;

                        case 'X':

                            if (sale.XRigor)
                            {
                                // "X" for Pricing - Recursive
                                // RIGOROUS means that, as block prices are applied to a quantity of
                                // product then the X-for algorithm is then applied again to the
                                // remaining quantity. For example ... if the X-for price was
                                // "3 for $10.00" and the regular price was $4.00 and 4 units were
                                // purchased then:
                                //
                                // The first 3 would be sold for $ 10.00
                                // One unit (4-3=1) would remain to be priced.
                                // That one unit does not qualify for X-For pricing and would be
                                // sold at the regular ($4.00) price.
                                n = (short)SL.SP_Prices.Count;
                                m = 0;
                                RP_Qty = 0;
                                if (n > 0) //  - Only if the price is not expired
                                {
                                    RP_Qty = SL.SP_Prices[1].From_Quantity;

                                    // Does the quantity exceed the first X-for or Incremental quantity
                                    if (Quantity >= RP_Qty)
                                    {

                                        while (!(n == 0 | Quantity < RP_Qty))
                                        {
                                            SP_Price with_3 = SL.SP_Prices[n];
                                            if (with_3.From_Quantity <= Quantity)
                                            {
                                                m++;
                                                Prices[m, Qty] = with_3.From_Quantity * ns;
                                                Prices[m, PRI] = Price_Units == "%"
                                                    ? (float)Reg_Price * (1 - with_3.Price / 100)
                                                    : with_3.Price;
                                                Prices[m, AMT] = Prices[m, PRI] * ns;
                                                Quantity = Quantity - with_3.From_Quantity;
                                                if (with_3.From_Quantity > Quantity)
                                                {
                                                    n--;
                                                }
                                            }
                                            else
                                            {
                                                n--;
                                            }
                                        }
                                    }

                                    if (RP_Qty > 0 & Quantity > 0)
                                    {
                                        m++;
                                        Prices[m, Qty] = (Quantity >= RP_Qty ? RP_Qty : Quantity) * ns;
                                        Prices[m, PRI] = (float)Reg_Price;
                                        Prices[m, AMT] = Prices[m, Qty] * (float)Reg_Price;
                                    }
                                }
                                else //expired
                                {
                                    m++;
                                    Prices[m, Qty] = Quantity * ns;
                                    Prices[m, PRI] = (float)Reg_Price;
                                    Prices[m, AMT] = Prices[m, Qty] * (float)Reg_Price;
                                }
                            }
                            else
                            {

                                // X-for Pricing - Best Price
                                // NON-RIGOROUS pricing takes the best unit price found, rounded to
                                // nearest cent and applies that price to all units purchased beyond
                                // the X-For quantity. For example ... if the X-for price was
                                // "3 for $10.00" and the regular price was $4.00 and 4 units were
                                // purchased then:
                                //
                                // The first 3 would be sold for $10.00
                                // The "best price" is computed as $10.00 / 3 = $3.34 (rounding up)
                                // The fourth unit is sold for $3.34
                                m = 0;
                                n = (short)SL.SP_Prices.Count;
                                if (n > 0) //  To exclude expired price
                                {
                                    RP_Qty = SL.SP_Prices[SL.SP_Prices.Count].From_Quantity;
                                }
                                Prices[1, Qty] = 1;
                                Prices[1, PRI] = (float)Reg_Price;
                                Prices[1, AMT] = (float)Reg_Price;

                                // Find the best pricing available
                                while (!(n == 0))
                                {
                                    SP_Price with_4 = SL.SP_Prices[n];
                                    if (with_4.From_Quantity <= Quantity)
                                    {
                                        m++;
                                        Prices[m, Qty] = with_4.From_Quantity * ns;
                                        Prices[m, PRI] = Price_Units == "%"
                                            ? (float)Reg_Price * (1 - with_4.Price / 100)
                                            : with_4.Price;
                                        Prices[m, AMT] = Prices[m, PRI] * ns;
                                        break;
                                    }
                                    n--;
                                }

                                Remaining_Qty = Quantity;
                                if (m > 0)
                                {
                                    Remaining_Qty = Quantity - Prices[1, Qty];
                                    while (Remaining_Qty >= Prices[1, Qty])
                                    {
                                        m++;
                                        Prices[m, Qty] = Prices[1, Qty];
                                        Prices[m, PRI] = Prices[1, PRI];
                                        Prices[m, AMT] = Prices[1, AMT];
                                        Remaining_Qty = Remaining_Qty - Prices[1, Qty];
                                    }
                                }

                                if (Remaining_Qty > 0)
                                {
                                    m++;
                                    // Compute the best price for the remaining units.
                                    Q = (Prices[1, PRI] / Prices[1, Qty]).ToString("0.000");
                                    if (Q.Substring(Q.Length - 1, 1) == "0")
                                    {
                                        Best_Price =
                                            (float)modGlobalFunctions.Round(Prices[1, PRI] / Prices[1, Qty], 2);
                                    }
                                    else
                                    {
                                        Q = Q.Substring(0, Q.Length - 1);
                                        Best_Price = (float)(Conversion.Val(Q) + 0.01);
                                    }

                                    Prices[m, Qty] = Remaining_Qty * ns;
                                    Prices[m, PRI] = Best_Price;
                                    Prices[m, AMT] = Prices[m, Qty] * Prices[m, PRI];
                                }
                            }
                            break;


                        case 'S':
                            //  To consider expiry also
                            n = (short)SL.SP_Prices.Count;
                            if (n > 0)
                            {

                                // Sale Pricing
                                Prices[1, Qty] = Quantity * ns;
                                Prices[1, PRI] = Price_Units == "%"
                                    ? (float)Reg_Price * (1 - SL.SP_Prices[1].Price / 100)
                                    : SL.SP_Prices[1].Price;
                                Prices[1, AMT] = Prices[1, Qty] * Prices[1, PRI] * ns;
                                m = 1;
                            }
                            else //if expired, take regular price
                            {
                                Prices[1, Qty] = Quantity * ns;
                                Prices[1, PRI] = (float)Reg_Price;
                                Prices[1, AMT] = Prices[1, Qty] * Prices[1, PRI] * ns;
                                m = 1;
                            }
                            break;

                        case 'I': // INCREMENTAL PRICING

                            if (SL.IRigor)
                            {
                                // Recursive application of Incremental Pricing
                                n = (short)SL.SP_Prices.Count;
                                if (n > 0) //  added the expiry condition
                                {
                                    Qty_Reg = (decimal)(SL.SP_Prices[1].From_Quantity - 1);
                                    k = (short)SL.SP_Prices.Count;
                                    Max_Quantity = (decimal)SL.SP_Prices[SL.SP_Prices.Count].To_Quantity;

                                    // Divide the quantity into segments. A segment is a quantity that
                                    // is equal to the maximum quantity specified in the incremental
                                    // price structure. For example, if the maximum quantity is 10 and 35
                                    // units are being sold then there are Ceil(35 / 10) = 4 segments.
                                    if (Quantity > (float)Max_Quantity)
                                    {
                                        Segments = Quantity / (float)Max_Quantity;
                                        if (Segments - Conversion.Int((short)Segments) != 0)
                                        {
                                            Segments++;
                                        }
                                        Segments = Conversion.Int((short)Segments);
                                    }
                                    else
                                    {
                                        Segments = 1;
                                    }

                                    m = 1;

                                    // Recursively apply the structure until you consume the full
                                    // specified quantity.
                                    Qty_Remaining = (decimal)Quantity;
                                    for (n = 1; n <= Segments; n++)
                                    {
                                        if (Qty_Remaining > 0)
                                        {
                                            // Handle the quantity sold at regular price.
                                            if (Qty_Reg > 0)
                                            {
                                                Prices[m, Qty] = MinVal((float)Qty_Remaining, (float)Qty_Reg);
                                                Prices[m, PRI] = (float)Reg_Price;
                                                Prices[m, AMT] = Prices[m, Qty] * Prices[m, PRI] * ns;
                                                Qty_Remaining = Qty_Remaining - (decimal)Prices[m, Qty];
                                                if (Qty_Remaining > 0)
                                                {
                                                    m++;
                                                }
                                            }

                                            // Apply the price structure until the quantity
                                            // remaining is zero or you complete one pass through
                                            // the structure.
                                            if (Qty_Remaining > 0)
                                            {
                                                for (j = 1; j <= k; j++)
                                                {
                                                    SP_Price with_5 = SL.SP_Prices[j];
                                                    if (j > 1)
                                                    {
                                                        if (SL.SP_Prices[j - 1].To_Quantity + 1 < with_5.From_Quantity)
                                                        {
                                                            Prices[m, Qty] =
                                                                MinVal((float)Qty_Remaining,
                                                                    with_5.From_Quantity -
                                                                    SL.SP_Prices[j - 1].To_Quantity - 1) * ns;
                                                            Prices[m, PRI] = (float)Reg_Price;
                                                            Prices[m, AMT] = Prices[m, Qty] * Prices[m, PRI];
                                                            Qty_Remaining = Qty_Remaining -
                                                                            Math.Abs((short)Prices[m, Qty]);
                                                            if (Qty_Remaining > 0)
                                                            {
                                                                m++;
                                                            }
                                                        }
                                                    }

                                                    if (Qty_Remaining > 0)
                                                    {
                                                        Prices[m, Qty] =
                                                            MinVal((float)Qty_Remaining,
                                                                with_5.To_Quantity - with_5.From_Quantity + 1) * ns;
                                                        Prices[m, PRI] = Price_Units == "%"
                                                            ? (float)Reg_Price * (1 - with_5.Price / 100)
                                                            : with_5.Price;
                                                        Prices[m, AMT] = Prices[m, Qty] * Prices[m, PRI];
                                                        Qty_Remaining = Qty_Remaining -
                                                                        Math.Abs((short)Prices[m, Qty]);
                                                    }
                                                    if (Qty_Remaining > 0)
                                                    {
                                                        m++;
                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                } // Next Quantity Range
                                            }
                                        }
                                    }


                                }
                                else //expired, then regular pricing for all items
                                {


                                    Prices[1, Qty] = Quantity * ns;
                                    Prices[1, PRI] = (float)Reg_Price;
                                    Prices[1, AMT] = Prices[1, Qty] * Prices[1, PRI] * ns;
                                    m = 1;
                                }
                            }
                            else // Not strict
                            {

                                // Incremental Pricing - Single Application (E.g. Limit Pricing)

                                k = (short)SL.SP_Prices.Count;
                                if (k > 0) //not expired
                                {
                                    Qty_Reg = (decimal)(SL.SP_Prices[1].From_Quantity - 1);
                                    Max_Quantity = (decimal)SL.SP_Prices[SL.SP_Prices.Count].To_Quantity;
                                    m = 1;
                                    Qty_Used = 0;
                                    Qty_Remaining = (decimal)Quantity;

                                    // Sell everything before the first quantity at regular price
                                    if (Qty_Reg > 0)
                                    {
                                        Prices[m, Qty] = MinVal((float)Qty_Remaining, (float)Qty_Reg);
                                        Prices[m, PRI] = (float)Reg_Price;
                                        Prices[m, AMT] = Prices[m, Qty] * Prices[m, PRI] * ns;
                                        Qty_Used = Qty_Used + (decimal)Prices[m, Qty];
                                        m++;
                                    }
                                    Qty_Remaining = Qty_Remaining - Qty_Used;

                                    // Apply the price structure once.
                                    if (Qty_Remaining > 0)
                                    {
                                        for (j = 1; j <= k; j++)
                                        {
                                            SP_Price with_6 = SL.SP_Prices[j];
                                            Prices[m, Qty] = MinVal((float)Qty_Remaining,
                                                with_6.To_Quantity - with_6.From_Quantity + 1);
                                            Prices[m, PRI] = Price_Units == "%"
                                                ? (float)Reg_Price * (1 - with_6.Price / 100)
                                                : with_6.Price;
                                            Prices[m, AMT] = Prices[m, Qty] * Prices[m, PRI] * ns;
                                            Qty_Remaining = Qty_Remaining - (decimal)Prices[m, Qty];
                                            m++;
                                            if (Qty_Remaining <= 0)
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    // Sell everything above the maximum quantity at regular price.
                                    if (Qty_Remaining > 0)
                                    {
                                        Prices[m, Qty] = (float)Qty_Remaining;
                                        Prices[m, PRI] = (float)Reg_Price;
                                        Prices[m, AMT] = Prices[m, Qty] * Prices[m, PRI] * ns;
                                    }
                                    else
                                    {
                                        m--;
                                    }
                                }
                                else //expired, then regular pricing for all items
                                {
                                    Prices[1, Qty] = Quantity * ns;
                                    Prices[1, PRI] = (float)Reg_Price;
                                    Prices[1, AMT] = Prices[1, Qty] * Prices[1, PRI] * ns;
                                    m = 1;
                                }
                            }
                            break;


                    }
                }
            }

            // Combine lines with the same price
            bool Combined = false;
            float[,] P_Hold = new float[1, 1];
            if (m == 0)
            {
                P_Hold = new float[2, 4];
            }
            else
            {
                P_Hold = new float[m + 1, 4];
            }
            for (n = 1; n <= m; n++)
            {
                Combined = false;
                for (i = 1; i <= m; i++)
                {
                    if ((Prices[n, PRI] == P_Hold[i, PRI]) && Prices[n, Qty] != 0)
                    {
                        P_Hold[i, Qty] = P_Hold[i, Qty] + Prices[n, Qty];
                        P_Hold[i, AMT] = P_Hold[i, AMT] + Prices[n, AMT]; //  Prices(i, AMT)
                        Combined = true;
                        break;
                    }
                    if (P_Hold[i, Qty] == 0)
                    {
                        break;
                    }
                }
                if (!Combined)
                {
                    for (i = 1; i <= P_Hold.Length - 1; i++)
                    {
                        if (P_Hold[i, Qty] == 0)
                        {
                            P_Hold[i, Qty] = Prices[n, Qty];
                            P_Hold[i, PRI] = Prices[n, PRI];
                            P_Hold[i, AMT] = Prices[n, AMT];
                            break;
                        }
                    }
                }
            }

            Array.Copy(P_Hold, Prices, P_Hold.Length);
            Performancelog.Debug($"End,SaleManager,PQuantity,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Prices;
        }

        /// <summary>
        /// Method to check a valid quantity
        /// </summary>
        /// <param name="qUANT_DEC"></param>
        /// <param name="quantity"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private bool IsValidQuantity(short qUANT_DEC, decimal quantity, out ErrorMessage error)
        {
            error = new ErrorMessage();
            const float MAXQTY = 9999;
            const float MINQTY = -9999;
            var Qd = qUANT_DEC; // Quantity Decimals

            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var fs = System.Convert.ToString(Qd == 0 ? " " : ("." + new string('9', Qd)));
            if (Convert.ToDouble(quantity) <= Conversion.Val((MINQTY).ToString() + fs))
            {
                MessageType temp_VbStyle13 = (int)MessageType.Exclamation + MessageType.OkOnly;
                error.StatusCode = HttpStatusCode.NotAcceptable;
                error.MessageStyle = new MessageStyle
                {
                    Message = "Minimum quantity is -9999~Quantity error",
                    MessageType = temp_VbStyle13
                };
                return false;
            }

            if (Convert.ToDouble(quantity) > Conversion.Val((MAXQTY).ToString() + fs))
            {
                MessageType temp_VbStyle13 = (int)MessageType.Exclamation + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, (short)75, (MAXQTY).ToString() + fs, temp_VbStyle13);
                error.StatusCode = HttpStatusCode.NotAcceptable;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method to check a valid price
        /// </summary>
        /// <param name="pRICE_DEC"></param>
        /// <param name="price"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private bool IsValidPrice(short pRICE_DEC, double price, out ErrorMessage error)
        {
            //validate price
            error = new ErrorMessage();
            const double MAXPRICE = 9999; // 
            const double MINPRICE = -9999;

            var Pd = pRICE_DEC; // Price Decimals

            var fs = System.Convert.ToString(Pd == 0 ? " " : ("." + new string('9', Pd)));
            if (Conversion.Val(price) <= Conversion.Val((MINPRICE).ToString() + fs))
            {
                MessageType temp_VbStyle9 = (int)MessageType.Exclamation + MessageType.OkOnly;
                error.MessageStyle = new MessageStyle
                {
                    Message = "Minimum price is 9999~Price error",
                    MessageType = temp_VbStyle9
                };
                error.StatusCode = HttpStatusCode.NotAcceptable;
                return false;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (Conversion.Val(price) > Conversion.Val((MAXPRICE).ToString() + fs))
            {
                MessageType temp_VbStyle9 = (int)MessageType.Exclamation + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, (short)76, (MAXPRICE).ToString() + fs, temp_VbStyle9);
                error.StatusCode = HttpStatusCode.NotAcceptable;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method to delete a line
        /// </summary>
        /// <param name="n"></param>
        /// <param name="sale"></param>
        /// <param name="error"></param>
        private void Delete_Line(short n, ref Sale sale, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,Delete_Line,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            error = new ErrorMessage();


            if (n <= sale.Sale_Lines.Count)
            {

                // Current line is in the sale. Remove it and refresh the display.
                // this.Enabled = false; // basket




                // if (CurrentLine < 1)
                {
                    //    SetCurrentLine((short)1, MaxLine);
                }
                var sLine = sale.Sale_Lines[n];



                if (sLine.Prepay)
                {
                    if (!_prepayManager.DeletePrepayFromFc(sLine.pumpID, true, out error))
                    {
                        return;
                    }
                }
                else
                {



                    if (sLine.Gift_Certificate && sLine.GiftType == "GiveX")
                    {
                        var offSet = _policyManager.LoadStoreInfo().OffSet;

                        MessageType temp_VbStyle = (int)MessageType.OkOnly + MessageType.Critical;
                        error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 22, null, temp_VbStyle);
                        error.StatusCode = HttpStatusCode.NotAcceptable;
                        return;

                        //Card
                    }


                    if (sLine.ProductIsFuel && sLine.Quantity > 0 && (!sLine.ManualFuel) && (!sLine.IsPropane) && (_policyManager.USE_FUEL) && sLine.pumpID != 0)
                    {


                        if (!Variables.gBasket[sLine.pumpID].CurrentFilled)
                        {
                            // put sale line back to basket
                            if (!_prepayManager.BasketUndo(sLine.pumpID, sLine.PositionID, sLine.GradeID, (float)sLine.Amount, (float)sLine.price, sLine.Quantity, sLine.Stock_Code, sLine.MOP, out error))
                            {
                                // this.Enabled = true; //  
                                return;
                            }

                        }
                        else
                        {
                            // this.Enabled = true;
                            return;

                        }
                    }
                    // Nicolette end
                }




                ErrorMessage errorMessage;
                RemoveSaleLineItem("", sale.TillNumber, sale.Sale_Num, n,
                      out errorMessage, true, true);
                //sale.Remove_a_Line(CurrentLine, true, true);

                //
                //We need to go through all the items if the coupons is used and see if it is still valid
                //Because the required item may be just deleted
                if (sale.Sale_Lines.blCoupon && sale.Sale_Lines.Count > 0)
                {
                    var blFind = false;
                    short i = 0;
                    for (i = 1; i <= sale.Sale_Lines.Count; i++)
                    {
                        //Mark all items as Not Processed and there is a coupon check for validity
                        sale.Sale_Lines[i].Processed = false;
                        if (sale.Sale_Lines[i].Quantity > 1)
                        {
                            sale.Sale_Lines[i].AvailableQty = 0;
                        }
                        if (sale.Sale_Lines[i].Stock_Type == 'P')
                        {
                            blFind = true;
                        }
                    }
                    if (blFind)
                    {
                        //ValidCoupon will go through all the coupon items and make sure there are still valid
                        //It will delete the coupon(s) if required item has just been deleted.
                        ValidateCoupon(ref sale, out error);
                    }
                    else
                    {
                        //no more coupons, so mark the sale as no coupons
                        sale.Sale_Lines.blCoupon = false;
                    }
                }
                else
                {
                    //no more items, mark the sale as no coupons
                    sale.Sale_Lines.blCoupon = false;
                }
                //End - SV

                //this.Enabled = true; //  

                if (sale.Sale_Lines.LastItemID - 1 < sale.Sale_Lines.Count)
                {
                    sale.Sale_Lines.LastItemID = sale.Sale_Lines.Count;
                }
                else
                {
                    sale.Sale_Lines.LastItemID--;
                }
                if (_policyManager.SOUND_SYS)
                {
                    //TODO Smriti for system sounds
                    //for (i = 0; i <= Application.OpenForms.Count - 1; i++)
                    //{
                    //    if (Application.OpenForms[i].Name == "MDIfrmPump")
                    //    {
                    //        MDIfrmPump.Default.PlaySound((byte)3, (byte)1);
                    //        break;
                    //    }
                    //}
                }
            }
            else
            {
                // Delete pressed on a new line. Just blank the fields on that line.
                return;

            }
            //Smriti UI code not required


            // Refresh_Lines();


            // Disable lines that are now blank
            //for (m = n; m <= Line_Count; m++)
            //{
            //    if (string.IsNullOrEmpty(txtStockCode.ToString()))
            //    {
            //        txtQty[m].Enabled = false;
            //        txtPrice[m].Enabled = false;
            //        txtDiscount[m].Enabled = false;
            //    }
            //}

            //Show_Line_Num(ref n);
            //
            //
            //SetCurrentLine(CurrentLine, (short)(sale.Sale_Lines.Count + 1));
            //
            //Color_Lines();
            Performancelog.Debug($"End,SaleManager,Delete_Line,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to validate coupon
        /// </summary>
        private void ValidateCoupon(ref Sale sale, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,ValidateCoupon,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            error = new ErrorMessage();
            short i = 1;
            var saleTotal = sale.Sale_Totals;
            //Go through all the items and if the item is the coupon then validate it
            while (i <= sale.Sale_Lines.Count)
            {
                //Check if there are any coupons and if so that corresponding items for them
                if (sale.Sale_Lines[i].Stock_Type == 'P' && sale.Sale_Lines[i].Processed == false)
                {
                    short cnt = 0;
                    var total = Math.Abs((short)(sale.Sale_Lines[i].Quantity));
                    do
                    {
                        //at least there is one qty for the coupon
                        var blFind = false;
                        short j;
                        for (j = 1; j <= sale.Sale_Lines.Count; j++)
                        {
                            //Check if there is an item for this coupon
                            if (sale.Sale_Lines[j].Dept == sale.Sale_Lines[i].Dept && sale.Sale_Lines[j].Sub_Dept == sale.Sale_Lines[i].Sub_Dept && sale.Sale_Lines[j].Sub_Detail == sale.Sale_Lines[i].Sub_Detail && sale.Sale_Lines[j].Processed == false && sale.Sale_Lines[j].Stock_Type != 'P')
                            {

                                if (sale.Sale_Lines[j].Quantity == 1)
                                {
                                    sale.Sale_Lines[i].Processed = true;
                                    sale.Sale_Lines[j].Processed = true;
                                }
                                else if (sale.Sale_Lines[j].Quantity > 1)
                                {
                                    if (sale.Sale_Lines[j].AvailableQty == 0)
                                    {
                                        //set availableqty for the first time
                                        sale.Sale_Lines[j].AvailableQty = (short)(sale.Sale_Lines[j].Quantity);
                                    }
                                    sale.Sale_Lines[j].AvailableQty = (short)(sale.Sale_Lines[j].AvailableQty - 1);
                                    if (sale.Sale_Lines[j].AvailableQty == 0)
                                    {
                                        //processed all quantities for this item
                                        sale.Sale_Lines[i].Processed = true; //coupon processed
                                        sale.Sale_Lines[j].Processed = true; //item completely processed
                                    }
                                    else
                                    {
                                        sale.Sale_Lines[i].Processed = true; //coupon processed
                                        sale.Sale_Lines[j].Processed = false; //item - still more qty
                                    }
                                }
                                blFind = true; //found one item for this coupon
                                break;
                            }
                        }
                        if (blFind == false)
                        {
                            var tmpStock = "";
                            if (cnt == 0)
                            {
                                //coupon has only one qty so we need to delete the coupon
                                tmpStock = sale.Sale_Lines[i].Stock_Code;
                                SetGross(ref saleTotal, sale.Sale_Totals.Net - sale.Sale_Lines[i].Amount);
                                sale.Sale_Lines.Remove(i); //remove coupon
                                if (sale.Sale_Lines.Count == 0)
                                {
                                    SetGross(ref saleTotal, 0);
                                }
                                //MsgBox "Coupon cannot be used because required item has been deleted."
                                error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 1153, tmpStock);
                                //Since coupon item is deleted, we don't need to increment i since total number of items decreased
                                cnt++;
                                break; //exit qty do loop since item is deleted
                            }
                            //coupon has more than one qty and not all qty's are validated
                            tmpStock = sale.Sale_Lines[i].Stock_Code;
                            //Msgbox "No all coupons can be processed"
                            error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 1152, tmpStock);
                            var tmpSaleLine = sale.Sale_Lines[i];
                            if (sale.Sale_Lines[i].Quantity < 0)
                            {
                                //qty is less then zero, so price is positive
                                _saleLineManager.SetQuantity(ref tmpSaleLine, (-1) * cnt);
                                _saleLineManager.SetAmount(ref tmpSaleLine, (decimal)(sale.Sale_Lines[i].price * (-1) * cnt));
                                sale.Sale_Lines[i].Quantity = tmpSaleLine.Quantity;
                                sale.Sale_Lines[i].Amount = tmpSaleLine.Amount;
                                sale.Sale_Lines[i].Net_Amount = tmpSaleLine.Net_Amount;
                                sale.Sale_Lines[i].Discount_Rate = tmpSaleLine.Discount_Rate;
                                //not impleneted completely
                            }
                            else
                            {
                                //qty is more than zero, so price is negative
                                _saleLineManager.SetAmount(ref tmpSaleLine, (decimal)(sale.Sale_Lines[i].price * cnt));
                                _saleLineManager.SetQuantity(ref tmpSaleLine, cnt);
                                sale.Sale_Lines[i].Amount = tmpSaleLine.Amount;
                                sale.Sale_Lines[i].Net_Amount = tmpSaleLine.Net_Amount;
                                sale.Sale_Lines[i].Discount_Rate = tmpSaleLine.Discount_Rate;
                                SetGross(ref saleTotal, sale.Sale_Totals.Net - Convert.ToDecimal((sale.Sale_Lines[i].Quantity - cnt) * sale.Sale_Lines[i].price));
                                sale.Sale_Lines[i].Quantity = tmpSaleLine.Quantity;
                            }
                            cnt++;
                            i++; //coupon was not deleted, so need to increment i
                            break; //no more qty's can be processed
                        }
                        cnt++;
                    } while (cnt < total); //loop through all the quantities of the coupon item
                    i++; //processed all coupon qty, move to the next item
                }
                else
                {
                    i++; //item was not a coupon so need to increment to go to the next product
                }
            }
            sale.Sale_Totals.Gross = saleTotal.Gross;
            sale.Sale_Totals.TotalLabel = saleTotal.TotalLabel;
            sale.Sale_Totals.SummaryLabel = saleTotal.SummaryLabel;
            Performancelog.Debug($"End,SaleManager,ValidateCoupon,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to get coupon id
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Coupon Id</returns>
        private string GetCouponId(int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleManager,GetCouponID,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            string strCoupon = "";


            var strPrefix = DateAndTime.Today.ToString("MMddyy") + tillNumber.ToString("000");

            while (true)
            {

                var strNum = Conversion.Int((int)(29999 * VBMath.Rnd())).ToString("00000");

                strCoupon = strPrefix + strNum.Trim();
                if (_utilityService.IsCouponAvailable(strCoupon))
                {
                    break;
                }
            }
            var returnValue = strCoupon;
            Performancelog.Debug($"End,SaleManager,GetCouponID,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }


        #endregion
    }
}



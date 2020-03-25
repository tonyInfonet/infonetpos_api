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

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class SuspendedSaleManager : ManagerBase, ISuspendedSaleManger
    {

        private readonly IPolicyManager _policyManager;
        private readonly ISuspendedSaleService _suspendedSaleService;
        private readonly IApiResourceManager _resourceManager;
        private readonly ISaleHeadManager _saleHeadManager;
        private readonly ISaleManager _saleManager;
        private readonly ILoginManager _loginManager;
        private readonly ICustomerManager _customerManager;
        private readonly ISaleLineManager _saleLineManager;
        private readonly IReasonService _reasonService;
        private readonly IPaymentManager _paymentManager;
        private readonly IReceiptManager _receiptManager;
        private readonly IPrepayManager _prepayManager;
        private readonly IMainManager _mainManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="policyManager"></param>
        /// <param name="suspendedSaleService"></param>
        /// <param name="resourceManager"></param>
        /// <param name="saleHeadManager"></param>
        /// <param name="saleManager"></param>
        /// <param name="saleLineManager"></param>
        /// <param name="loginManager"></param>
        /// <param name="customerManager"></param>
        /// <param name="reasonService"></param>
        /// <param name="paymentManager"></param>
        /// <param name="receiptManager"></param>
        /// <param name="prepayManager"></param>
        /// <param name="mainManager"></param>
        public SuspendedSaleManager(IPolicyManager policyManager,
            ISuspendedSaleService suspendedSaleService,
            IApiResourceManager resourceManager,
            ISaleHeadManager saleHeadManager,
            ISaleManager saleManager,
            ISaleLineManager saleLineManager,
            ILoginManager loginManager,
            ICustomerManager customerManager,
            IReasonService reasonService,
            IPaymentManager paymentManager,
            IReceiptManager receiptManager, IPrepayManager prepayManager,
            IMainManager mainManager)
        {
            _policyManager = policyManager;
            _suspendedSaleService = suspendedSaleService;
            _resourceManager = resourceManager;
            _saleHeadManager = saleHeadManager;
            _saleManager = saleManager;
            _saleLineManager = saleLineManager;
            _loginManager = loginManager;
            _customerManager = customerManager;
            _reasonService = reasonService;
            _paymentManager = paymentManager;
            _receiptManager = receiptManager;
            _prepayManager = prepayManager;
            _mainManager = mainManager;
        }

        /// <summary>
        /// Get Suspended Sale List
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>List of suspended sales</returns>
        public List<SusHead> GetSuspendedSale(int tillNumber, out ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SuspendedSaleManager,GetSuspendedSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            string sqlQuery;
            if (_policyManager.SHARE_SUSP)
            {

                sqlQuery = "SELECT   SusHead.Sale_No ," +
                           "         SusHead.Client ," + "SusHead.TILL " +
                           "FROM     SusHead " + "ORDER BY SusHead.Sale_No DESC";
            }
            else
            {

                sqlQuery = "SELECT   SusHead.Sale_No ," +
                           "         SusHead.Client ," + "SusHead.TILL  " +
                           "FROM     SusHead  where SusHead.TILL=" + tillNumber +
                           " ORDER BY SusHead.Sale_No DESC";
            }

            var susSale = _suspendedSaleService.GetAllSuspendedSale(sqlQuery);

            if (susSale == null || susSale.Count == 0)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 92, null, MessageType.OkOnly),
                    StatusCode = HttpStatusCode.NotFound
                };
                return null;
            }

            Performancelog.Debug($"End,SuspendedSaleManager,GetSuspendedSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return susSale;
        }

        /// <summary>
        /// Suspend Sale 
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Sale</returns>
        public Sale SuspendSale(int tillNumber, int saleNumber, string userCode, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SuspendedSaleManager,SuspendSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            if (!CanUserSuspendSale(userCode, out errorMessage))
            {
                return new Sale();
            }
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }

            //Added for checking delete prepay crash recovery
            if (sale.DeletePrepay)
            {
                //Please complete delete prepay first!~Comlete current transaction.
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 50, null),
                    StatusCode = HttpStatusCode.PreconditionFailed
                };
                return null;
            }

            if (sale.Sale_Lines.Count == 0 && !_policyManager.SUSPEND_MT)
            {
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 80, null, MessageType.OkOnly),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }
            //Added checking not allowing suspend sale with prepay
            if (_prepayManager.PrepayItemId(ref sale) > 0)
            {
                //MsgBox ("Cannot suspend sale with prepay, Please remove the prepay sale before suspend!~Suspend a Sale.")
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 57, null, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }
            var sharePolicy = _policyManager.SHARE_SUSP;
            _suspendedSaleService.SuspendSale("SUSPEND", sharePolicy, sale, userCode);
            CacheManager.DeleteCurrentSaleForTill(sale.TillNumber, sale.Sale_Num);
            sale = _saleManager.InitializeSale(tillNumber, sale.Register, userCode, out errorMessage);
            //Update Sale object in Cache
            CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
            Performancelog.Debug($"End,SuspendedSaleManager,SuspendSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return sale;
        }

        /// <summary>
        /// Unsuspend Sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Sale</returns>
        public Sale UnsuspendSale(int saleNumber, int tillNumber, string userCode, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SuspendedSaleManager,UnsuspendSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            if (!CanUserSuspendSale(userCode, out errorMessage))
            {
                return new Sale();
            }

            string sqlQuery;
            var shareSusp = _policyManager.SHARE_SUSP;
            if (shareSusp)
            {
                sqlQuery = "SELECT * FROM   SusHead  WHERE  SusHead.Sale_No = " + Convert.ToString(saleNumber);
            }
            else
            {
                sqlQuery = "SELECT * FROM   SusHead  WHERE  SusHead.Sale_No = " + Convert.ToString(saleNumber) + " AND SusHead.TILL=" + tillNumber;
            }

            var susHead = _suspendedSaleService.GetAllSuspendedSale(sqlQuery);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (susHead == null || susHead.Count == 0)
            {
                //"Not a Suspended Sale"
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8107, saleNumber),
                    StatusCode = HttpStatusCode.NotFound

                };
                return null;
            }

            _suspendedSaleService.UpdateCardSaleForUnSuspend(shareSusp, tillNumber, saleNumber);
            var sale = _suspendedSaleService.GetSuspendedSale(tillNumber, saleNumber, shareSusp);
            sale.Customer = _customerManager.LoadCustomer(sale.Customer.Code);
            if (string.IsNullOrEmpty(sale.CouponID))
            {
                _saleManager.ReCompute_Coupon(ref sale);
            }
            var saleLines = sale.Sale_Lines;
            if (sale.Sale_Totals.Sale_Taxes != null)
            {
                _saleHeadManager.Load_Taxes(ref sale);
            }
            sale.Sale_Lines = new Sale_Lines();
            _suspendedSaleService.RemovePreviousTransactionFromDbTemp(tillNumber);
            foreach (Sale_Line saleLine in saleLines)
            {
                var sl = saleLine;
                var quantity = saleLine.Quantity;
                var price = saleLine.price;
                var amount = saleLine.Amount;
                var discountRate = saleLine.Discount_Rate;
                var discountType = saleLine.Discount_Type;
                ErrorMessage error;
                _saleLineManager.SetPluCode(ref sale, ref sl, sl.PLU_Code, out error);

                if (sl.ProductIsFuel && !sl.IsPropane)
                {
                    sl.Regular_Price = Convert.ToDouble(sl.price);
                }

                if (sl.Gift_Certificate && sl.GiftType == "LocalGift")
                {
                    sl.Description = _resourceManager.GetResString(offSet, 8124) + sl.Gift_Num;
                }
                if (sale.Void_Num != 0)
                {
                    _saleManager.Add_a_Line(ref sale, sl, userCode, tillNumber, out errorMessage, adjust: false, tableAdjust: false, forRefund: true);
                }
                else
                {
                    _saleManager.Add_a_Line(ref sale, sl, userCode, tillNumber, out errorMessage, false, false);
                }

                sl.No_Loading = false;
                if (sl.ScalableItem)
                {
                    sl.Regular_Price = sl.price;
                }

                _saleLineManager.SetPrice(ref sl, price);
                _saleLineManager.SetQuantity(ref sl, quantity);
                _saleLineManager.SetAmount(ref sl, amount);
                saleLine.Discount_Type = discountType;
                _saleLineManager.SetDiscountRate(ref sl, discountRate);
                sl.No_Loading = false; // It has to be set back to false, otherwise if the user changes the qty or price in the POS screen after unsuspend, the amount will not be right

                if (shareSusp)
                {
                    // modGlobalFunctions.Load_CardSales(Chaps_Main.dbTill, (short)0, saleNumber, SL.Line_Num);
                }
            }

            _saleManager.Sale_Discount(ref sale, Convert.ToDecimal(sale.Sale_Totals.Invoice_Discount),
                Convert.ToString(sale.Sale_Totals.Invoice_Discount_Type), false);
            _suspendedSaleService.DeleteUnsuspend(shareSusp, tillNumber, saleNumber);
            _suspendedSaleService.DeleteCardSaleFromDbTemp(saleNumber);
            sale.Sale_Type = "CANCEL";
            //Update Sale object in Cache
            CacheManager.AddCurrentSaleForTill(tillNumber, sale.Sale_Num, sale);
            Performancelog.Debug($"End,SuspendedSaleManager,UnsuspendSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            Register register = new Register();
            _mainManager.SetRegister(ref register, sale.Register);
            //Added to display invoice# to customer display
            if (register.Customer_Display)
            {
                sale.CustomerDisplay = _mainManager.DisplayMsgLcd(register,
                    _mainManager.FormatLcdString(register, _resourceManager.GetResString(offSet, 414) + "#:",
                    Conversion.Str(saleNumber).Trim()), "");
            }
            return sale;
        }

        /// <summary>
        /// Verify Void sale
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>True or false</returns>
        public bool VerifyVoidSale(string userCode, int saleNumber, int tillNumber, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SuspendedSaleManager,VerifyVoidSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            if (!CanUserVoidSale(userCode, out errorMessage))
            {
                return false;
            }

            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return false;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            //Added for checking delete prepay crash recovery
            if (sale.DeletePrepay)
            {
                //Please complete delete prepay first!~Comlete current transaction.
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 50, null),
                    StatusCode = HttpStatusCode.PreconditionFailed
                };
                return false;
            }

            if (sale.Sale_Lines.Count == 0)
            {
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 77, null, MessageType.OkOnly),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return false;
            }


            if (CountFuelLines(sale) > 1) // if there are more then 1 line with fuel do not allow to void this sale
            {
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 82, null, MessageType.OkOnly),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return false;
            }

            //Added, not allow to void sale with prepay line
            //user needs to delete the unpaid prepay line first before void this sale
            if (_prepayManager.PrepayItemId(ref sale) > 0)
            {
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 82, null, MessageType.OkOnly),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return false;
            }

            //crash recovery' if there is partial/ full payment donot allow void
            if (sale.Payment)
            {
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 14, null, MessageType.OkOnly),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return false;
            }

            string lineNumber = string.Empty;
            if (modGlobalFunctions.HasGivexSale(sale, ref lineNumber))
            {
                
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 22, null, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return false;
            }
            Performancelog.Debug($"End,SuspendedSaleManager,VerifyVoidSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return true;
        }

        /// <summary>
        /// Void sale
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="voidReason">Void reason</param>
        /// <param name="errorMessage">Error</param>
        /// <param name="fs">Report content</param>
        /// <returns>Sale</returns>
        public Sale VoidSale(string userCode, int saleNumber, int tillNumber, string voidReason,
            out ErrorMessage errorMessage, out Report fs)
        {
            fs = null;
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SuspendedSaleManager,VoidSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            //Added for checking delete prepay crash recovery
            if (sale.DeletePrepay)
            {
                //Please complete delete prepay first!~Comlete current transaction.
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 50, null),
                    StatusCode = HttpStatusCode.PreconditionFailed
                };
                return null;
            }

            if (sale.Sale_Lines.Count == 0)
            {
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 77, null, MessageType.OkOnly),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }


            if (CountFuelLines(sale) > 1) // if there are more then 1 line with fuel do not allow to void this sale
            {
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 82, null, MessageType.OkOnly),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }

            //Added, not allow to void sale with prepay line
            
            if (_prepayManager.PrepayItemId(ref sale) > 0)
            {
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 82, null, MessageType.OkOnly),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }

            //crash recovery' if there is partial/ full payment donot allow void
            if (sale.Payment)
            {
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 14, null, MessageType.OkOnly),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }

            //Added to control void GiveX items
            string lineNumber = string.Empty;
            if (modGlobalFunctions.HasGivexSale(sale, ref lineNumber))
            {
                
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 22, null, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }
            if (!CanUserVoidSale(userCode, out errorMessage))
            {
                return null;
            }


            //added to print receipt for Void sales
            if (_policyManager.PRINT_VOID)
            {
                sale.Sale_Type = "VOID";
                Tenders nullTenders = null;
                var fileName = string.Empty;
                var rePrint = false;
                Stream signature;
                fs = _receiptManager.Print_Receipt(sale.TillNumber, null, ref sale, ref nullTenders,
                    true, ref fileName, ref rePrint, out signature, userCode);
            }

            var newSale = new Sale
            {
                Sale_Num =
                    _saleManager.Clear_Sale(sale, sale.Sale_Num, sale.TillNumber, userCode, "VOID", null, true, true,
                        false, out errorMessage)
            };

            CacheManager.DeleteCurrentSaleForTill(sale.TillNumber, sale.Sale_Num);
            newSale.TillNumber = sale.TillNumber;
            newSale.Customer = _customerManager.LoadCustomer(string.Empty);
            newSale.Register = sale.Register;
            newSale.SaleHead = new SaleHead();
            newSale.Payment = false;
            _saleHeadManager.SetSalePolicies(ref newSale);
            _saleManager.SaveTemp(ref newSale, newSale.TillNumber);
            Register register = null;
            _mainManager.SetRegister(ref register, newSale.Register);
            if (register.Customer_Display)
            {
                newSale.CustomerDisplay = _mainManager.DisplayMsgLcd(register,
                    _mainManager.FormatLcdString(register, _resourceManager.GetResString(offSet, 414) + "#:",
                    Conversion.Str(newSale.Sale_Num).Trim()), "");
            }
            CacheManager.AddCurrentSaleForTill(newSale.TillNumber, newSale.Sale_Num, newSale);
            Performancelog.Debug($"End,SuspendedSaleManager,VoidSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return newSale;
        }

        /// <summary>
        /// Method to writeoff a sale
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">TillNumber</param>
        /// <param name="writeOffReason"></param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="registerNumber">Register number</param>
        /// <returns>New sale</returns>
        public Report WriteOff(string userCode, int saleNumber, int tillNumber, string writeOffReason,
            out ErrorMessage errorMessage, out int registerNumber)
        {
            errorMessage = new ErrorMessage();
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            registerNumber = 0;
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            //crash recovery- if there is partial/ full payment donot allow write off
            if (sale.Payment)
            {
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 12, 86, null, MessageType.OkOnly);
                return null;
            }
            registerNumber = sale.Register;
            sale.Sale_Type = Convert.ToString(sale.Sale_Type == "MARKDOWN" ? "SALE" : "MARKDOWN");
            if (sale.Sale_Type == "MARKDOWN")
            {
                var reasonType = 'M';
                sale.Return_Reason = _reasonService.GetReturnReason(writeOffReason, reasonType);
                if (sale.Return_Reason == null)
                {
                    errorMessage.MessageStyle = new MessageStyle
                    {
                        Message = "Invalid reason"
                    };
                    errorMessage.StatusCode = HttpStatusCode.NotFound;
                    return null;
                }
                sale.Customer.Name = _resourceManager.GetResString(offSet, 159);
                sale.Customer.Code = _resourceManager.GetResString(offSet, 1252);

                _saleManager.ApplyTaxes(ref sale, false);  // Added, don't calculate taxes for MARKDOWN sales
                _saleManager.ApplyCharges(ref sale, false); // Added, don't calculate taxes for MARKDOWN sales
                var user = CacheManager.GetUser(userCode);
                Stream signature;
                var fileName = string.Empty;
                if (_policyManager.PRINT_REC)
                {
                    return _paymentManager.ExactChange_Receipt(sale, user, tillNumber, out errorMessage, out signature, ref fileName);
                }
                return _paymentManager.ExactChange_Receipt(sale, user, tillNumber, out errorMessage, out signature, ref fileName);
            }
            errorMessage.MessageStyle = new MessageStyle
            {
                Message = "Could not write off"
            };
            errorMessage.StatusCode = HttpStatusCode.NoContent;
            return null;
        }

        #region Private methods

        /// <summary>
        /// Count Fuel Lines
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        private byte CountFuelLines(Sale sale)
        {
            byte returnValue = 0;

            foreach (Sale_Line tempLoopVarSlCount in sale.Sale_Lines)
            {
                var slCount = tempLoopVarSlCount;
                if ((slCount.ProductIsFuel && (!slCount.IsPropane)) || (slCount.Gift_Certificate && slCount.GiftType == "GiveX"))
                {
                    returnValue++;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Checks Whether User can Suspend or Unsuspend Transaction
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private bool CanUserSuspendSale(string userCode, out ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();
            var user = _loginManager.GetUser(userCode);

            if (!Convert.ToBoolean(_policyManager.GetPol("U_SUSP", user)))
            {
                errorMessage = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "You are not authorised to Suspend or Unsuspend the Sale",
                        MessageType = (int)MessageType.Information + MessageType.OkOnly
                    },
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return false;
            }
            return true;
        }


        /// <summary>
        /// Checks whether user can void sale
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private bool CanUserVoidSale(string userCode, out ErrorMessage errorMessage)
        {
            errorMessage = new ErrorMessage();
            var user = _loginManager.GetUser(userCode);

            if (!Convert.ToBoolean(_policyManager.GetPol("U_CAN_VOID", user)))
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 96, null, YesNoQuestionMessageType),
                    StatusCode = HttpStatusCode.Forbidden
                };
                return false;
            }
            return true;
        }

        #endregion

    }
}
using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class ReturnSaleManager : ManagerBase, IReturnSaleManager
    {
        private readonly IReturnSaleService _returnSaleService;
        private readonly IPolicyManager _policyManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ISaleManager _saleManager;
        private readonly IReasonService _reasonService;
        private readonly ISaleService _saleService;
        private readonly ISaleLineManager _saleLineManager;
        private readonly ISaleHeadManager _saleHeadManager;
        private readonly IReceiptManager _receiptManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="returnSaleService"></param>
        /// <param name="policyManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="saleManager"></param>
        /// <param name="reasonService"></param>
        /// <param name="saleService"></param>
        /// <param name="saleLineManager"></param>
        /// <param name="saleHeadManager"></param>
        /// <param name="receiptManager"></param>
        public ReturnSaleManager(IReturnSaleService returnSaleService,
            IPolicyManager policyManager,
            IApiResourceManager resourceManager,
            ISaleManager saleManager,
            IReasonService reasonService,
            ISaleService saleService,
            ISaleLineManager saleLineManager,
            ISaleHeadManager saleHeadManager,
            IReceiptManager receiptManager)
        {
            _returnSaleService = returnSaleService;
            _policyManager = policyManager;
            _resourceManager = resourceManager;
            _saleManager = saleManager;
            _reasonService = reasonService;
            _saleService = saleService;
            _saleLineManager = saleLineManager;
            _saleHeadManager = saleHeadManager;
            _receiptManager = receiptManager;
        }

        /// <summary>
        /// Get all Sales
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public List<SaleHead> GetAllSales(int pageIndex, int pageSize)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReturnSaleManager,GetAllSales,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            DateTime saleDate = DateTime.Now.AddDays(-Convert.ToInt16(_policyManager.GetPol("RES_DAYS", null)));
            Performancelog.Debug($"End,ReturnSaleManager,GetAllSales,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return _returnSaleService.GetAllSales(saleDate, _policyManager.TIMEFORMAT, pageIndex, pageSize);
        }

        /// <summary>
        /// Get Sale By sale No
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="message">Message</param>
        /// <returns>Sale</returns>
        public Sale GetSale(int saleNumber, int tillNumber, out ErrorMessage message)
        {
            bool isSaleFound;
            bool isReturnable;
            var sale = _returnSaleService.GetSaleBySaleNumber(saleNumber, tillNumber, new DateTime(), _policyManager.TE_Type, _policyManager.TE_GETNAME, _policyManager.TAX_EXEMPT_GA, _policyManager.DefaultCust, _policyManager.DEF_CUST_CODE, out isSaleFound, out isReturnable);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!isSaleFound)
            {
                message = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet,40, 98, null)
                };
                return sale;
            }
            if (sale.Sale_Type == "SALE" || sale.Sale_Type == "REFUND")
            {
                var saleLines = _returnSaleService.GetSaleLineBySaleNumber(saleNumber, tillNumber, new DateTime(), sale.Customer.DiscountType, _policyManager.TE_Type, _policyManager.TAX_EXEMPT);

                foreach (Sale_Line sline in saleLines)
                {
                    sline.PRICE_DEC = CommonUtility.GetShortValue(_policyManager.GetPol("PRICE_DEC", sline));
                    sline.QUANT_DEC = CommonUtility.GetShortValue(_policyManager.GetPol("QUANT_DEC", sline));
                    sline.RET_REASON = _policyManager.GetPol("RET_REASON", sline);
                    sale.Sale_Lines.AddLine(sline.Line_Num, sline, "");
                }
            }
            message = new ErrorMessage();
            return sale;
        }

        /// <summary>
        /// Search Sales
        /// </summary>
        /// <param name="saleDate">Sale date</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="error">Error</param>
        /// <returns>List of sale head</returns>
        public List<SaleHead> SearchSale(DateTime? saleDate, int? saleNumber, int pageIndex, int pageSize, out ErrorMessage error)
        {
            error = new ErrorMessage();
            DateTime slDate = DateTime.Now.AddDays(-Convert.ToInt16(_policyManager.GetPol("RES_DAYS", null)));
            var list = _returnSaleService.SearchSale(saleNumber, saleDate, slDate, _policyManager.TIMEFORMAT, pageIndex, pageSize);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (list.Count == 0 && saleNumber.HasValue)
            {
                bool isReturnable;
                bool isSaleFound;
                _returnSaleService.IsSaleExist(saleNumber.Value, slDate, out isSaleFound, out isReturnable);

                if (!isSaleFound)
                {
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet,40, 96, saleNumber)
                    };
                    return null;
                }

                if (!isReturnable)
                {
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet,40, 95, slDate)
                    };
                    return null;
                }
            }
            return list;
        }

        /// <summary>
        /// Return Sale
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleTillNumber"></param>
        /// <param name="correction">Correction sale or not</param>
        /// <param name="reasonCode">Reason code</param>
        /// <param name="message">Error</param>
        /// <param name="reasonType">Reason type</param>
        /// <param name="saleLineMessages">Sale line messages</param>
        /// <param name="report"></param>
        /// <param name="fileName"></param>
        /// <returns>Sale</returns>
        public Sale ReturnSale(User user, int saleNumber, int tillNumber, int saleTillNumber, bool correction,
            string reasonType, string reasonCode, out ErrorMessage message, out List<ErrorMessage>
                saleLineMessages, out Report report, out string fileName)
        {
            Sale sale = new Sale();
            DateTime saleDate = DateTime.Now.AddDays(-Convert.ToInt16(_policyManager.GetPol("RES_DAYS", null)));
            saleLineMessages = new List<ErrorMessage>();
            report = null;
            fileName = string.Empty;
            if (!IsUserAbleToReturn(user, out message))
            {
                return sale;
            }

            Sale_Line sl;
            var blnRecReason = false;
            bool isReturnable;
            bool isSaleFound;
            sale = _returnSaleService.GetSaleBySaleNumber(saleNumber, saleTillNumber, saleDate, _policyManager.TE_Type, _policyManager.TE_GETNAME, _policyManager.TAX_EXEMPT_GA, _policyManager.DefaultCust, _policyManager.DEF_CUST_CODE, out isSaleFound, out isReturnable);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!isSaleFound)
            {
                message = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet,40, 98, null)
                };
                return sale;
            }

            if (!isReturnable)
            {
                message = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet,40, 95, saleDate)
                };
                return sale;
            }
            var sal = _saleService.GetSaleByTillNumber(tillNumber);// _saleService.GetSalesFromDbTemp(tillNumber);
            sale.Sale_Num = sal?.Sale_Num ?? _saleManager.GetCurrentSaleNo(tillNumber, user.Code, out message);
            sale.TillNumber = (byte)tillNumber;
            _saleHeadManager.SetSalePolicies(ref sale);
            var saleLines = _returnSaleService.GetSaleLineBySaleNumber(saleNumber, saleTillNumber, saleDate, sale.Customer.DiscountType, _policyManager.TE_Type, _policyManager.TAX_EXEMPT);
            //_suspendedSaleService.RemovePreviousTransactionFromDbTemp(tillNumber);

            foreach (var saleLine in saleLines)
            {
                saleLine.PRICE_DEC = CommonUtility.GetShortValue(_policyManager.GetPol("PRICE_DEC", saleLine));
                saleLine.QUANT_DEC = CommonUtility.GetShortValue(_policyManager.GetPol("QUANT_DEC", saleLine));
                if (_policyManager.GetPol("ACCEPT_RET", saleLine) && saleLine.GiftType != "GiveX")
                {
                    var newSaleLine = saleLine;
                    newSaleLine.Till_Num = sale.TillNumber;
                    var quantity = saleLine.Quantity;
                    var price = saleLine.price;
                    var amount = saleLine.Amount;
                    var discountRate = saleLine.Discount_Rate;
                    var discountType = saleLine.Discount_Type;
                    ErrorMessage error;
                    _saleLineManager.SetPluCode(ref sale, ref newSaleLine, newSaleLine.PLU_Code, out error);

                    if (newSaleLine.ProductIsFuel && !newSaleLine.IsPropane)
                    {
                        newSaleLine.Regular_Price = Convert.ToDouble(newSaleLine.price);
                    }

                    if (newSaleLine.Gift_Certificate && newSaleLine.GiftType == "LocalGift")
                    {
                        newSaleLine.Description = _resourceManager.GetResString(offSet,8124) + newSaleLine.Gift_Num;
                    }
                    _saleManager.Add_a_Line(ref sale, newSaleLine, user.Code, sale.TillNumber, out message, adjust: false, tableAdjust: false,
                        forRefund: true);

                    newSaleLine.No_Loading = false;
                    if (newSaleLine.ScalableItem)
                    {
                        newSaleLine.Regular_Price = newSaleLine.price;
                    }

                    _saleLineManager.SetPrice(ref newSaleLine, price);
                    _saleLineManager.SetQuantity(ref newSaleLine, quantity);
                    _saleLineManager.SetAmount(ref newSaleLine, amount);
                    saleLine.Discount_Type = discountType;
                    _saleLineManager.SetDiscountRate(ref newSaleLine, discountRate);
                    newSaleLine.No_Loading = false; // It has to be set back to false, otherwise if the user changes the qty or price in the POS screen after unsuspend, the amount will not be right

                }
                else
                {
                    saleLineMessages.Add(new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet,0, 8109, saleLine.Stock_Code + " " + saleLine.Description,
                                    CriticalOkMessageType)
                    });
                }
            }

            _saleManager.Sale_Discount(ref sale, Convert.ToDecimal(sale.Sale_Totals.Invoice_Discount),
                Convert.ToString(sale.Sale_Totals.Invoice_Discount_Type), false);
            sale.ForCorrection = correction; // 06/24/05 Nancy added ForCorrection
                                             //    CL = "" ' code
                                             //SaleMain.Default.lblCustName.Text = sale.Customer.Name;
            foreach (var tempLoopVarSl in saleLines)
            {
                sl = tempLoopVarSl;
                sl.No_Loading = false;
            }
            sale.LoadingTemp = false;

            foreach (var tempLoopVarSl in saleLines)
            {
                sl = tempLoopVarSl;
                if (!_policyManager.GetPol("RET_REASON", sl)) continue;
                blnRecReason = true;
                break;
            }

            if (blnRecReason)
            {
                ReasonType rType;
                Enum.TryParse(reasonType, true, out rType);
                var returnReason = (char)rType != '\0' ? _reasonService.GetReturnReason(reasonCode, (char)rType) : new Return_Reason { RType = "R" };
                foreach (var tempLoopVarSl in saleLines)
                {
                    sl = tempLoopVarSl;
                    _saleManager.Line_Reason(ref sale, ref sl, returnReason);
                }
            }
            _saleManager.SaveTemp(ref sale, tillNumber);
            //Update Sale object in Cache
            sale.ReverseRunaway = sale.Sale_Type == "RUNAWAY" && sale.Sale_Amount > 0; // 
            sale.ReversePumpTest = sale.Sale_Type == "PUMPTEST" && sale.Sale_Amount > 0; // 
            if (sale.ReverseRunaway)
            {
                var newLineMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet,11, 25, null)
                };
                saleLineMessages.Add(newLineMessage);
                Tenders nullTenders = null;
                fileName = Constants.RunAwayFile;
                var file = "Runaway";
                var rePrint = false;
                Stream signature;
                sale.Sale_Type = "RUNAWAY";
                report = _receiptManager.Print_Receipt(tillNumber, null, ref sale, ref nullTenders, true, ref file,
                    ref rePrint, out signature, user.Code);
                _saleManager.Clear_Sale(sale, sale.Sale_Num, sale.TillNumber, user.Code, "RUNAWAY", null, false, true, false, out message);

                ErrorMessage error;
                sale = _saleManager.InitializeSale(tillNumber, sale.Register, user.Code, out error);
            }
            if (sale.ReversePumpTest)
            {
                var newLineMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet,11, 27, null)
                };
                saleLineMessages.Add(newLineMessage);
                Tenders nullTenders = null;
                fileName = Constants.PumpTestFile;
                var file = "PumpTest";
                var rePrint = false;
                Stream signature;
                sale.Sale_Type = "PUMPTEST";
                report = _receiptManager.Print_Receipt(tillNumber, null, ref sale, ref nullTenders, true, ref file, ref rePrint, out signature, user.Code);
                _saleManager.Clear_Sale(sale, sale.Sale_Num, sale.TillNumber, user.Code, "PUMPTEST", null, false, true, false, out message);
                ErrorMessage error;
                sale = _saleManager.InitializeSale(tillNumber, sale.Register, user.Code, out error);
            }
            CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
            return sale;
        }

        /// <summary>
        /// Return Sale Items
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="loggedInTillNumber">Logged till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="saleLines">Sale lines</param>
        /// <param name="isCorrection">Correction sale or not</param>
        /// <param name="user">User</param>
        /// <param name="reasonCode">Reason code</param>
        /// <param name="message">Error</param>
        /// <param name="saleLineMessages">Sale line messages</param>
        /// <param name="reasonType">Reason type</param>
        /// <returns>Sale</returns>
        public Sale ReturnSaleItems(User user, int tillNumber, int loggedInTillNumber, int saleNumber, int[] saleLines, bool isCorrection, string reasonType, string reasonCode, out ErrorMessage message, out List<ErrorMessage> saleLineMessages)
        {
            var sale = new Sale();
            DateTime saleDate = DateTime.Now.AddDays(-Convert.ToInt16(_policyManager.GetPol("RES_DAYS", null)));
            saleLineMessages = new List<ErrorMessage>();

            if (!IsUserAbleToReturn(user, out message))
            {
                return sale;
            }
            Sale_Line sl;
            bool blnRecReason = false;
            bool isReturnable;
            bool isSaleFound;
            sale = _returnSaleService.GetSaleBySaleNumber(saleNumber, tillNumber, saleDate, _policyManager.TE_Type, _policyManager.TE_GETNAME, _policyManager.TAX_EXEMPT_GA, _policyManager.DefaultCust, _policyManager.DEF_CUST_CODE, out isSaleFound, out isReturnable);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!isSaleFound)
            {
                message = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet,40, 98, null)
                };
                return sale;
            }

            if (!isReturnable)
            {
                message = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet,40, 98, saleDate)
                };
                return sale;
            }

            var sal = _saleService.GetSaleByTillNumber(tillNumber);// _saleService.GetSalesFromDbTemp(tillNumber);
            if (sal == null)
            {
                sale.Sale_Num = _saleManager.GetCurrentSaleNo(tillNumber, user.Code, out message);
            }
            else
            {
                sale.Sale_Num = sal.Sale_Num;
            }
            sale.TillNumber = (byte)loggedInTillNumber;
            _saleHeadManager.SetSalePolicies(ref sale);
            var sLines = _returnSaleService.GetSaleLineBySaleNumber(saleNumber, tillNumber, saleDate, sale.Customer.DiscountType, _policyManager.TE_Type, _policyManager.TAX_EXEMPT);
            //_suspendedSaleService.RemovePreviousTransactionFromDbTemp(tillNumber);
            foreach (var saleLine in sLines)
            {
                saleLine.PRICE_DEC = CommonUtility.GetShortValue(_policyManager.GetPol("PRICE_DEC", saleLine));
                saleLine.QUANT_DEC = CommonUtility.GetShortValue(_policyManager.GetPol("QUANT_DEC", saleLine));
                saleLine.RET_REASON = _policyManager.GetPol("RET_REASON", saleLine);
                if (saleLines.ToList().Exists(x => x == CommonUtility.GetIntergerValue(saleLine.Line_Num)))
                {
                    if (_policyManager.GetPol("ACCEPT_RET", saleLine) && saleLine.GiftType != "GiveX")
                    {
                        var newSaleLine = saleLine;
                        var quantity = saleLine.Quantity;
                        var price = saleLine.price;
                        var amount = saleLine.Amount;
                        var discountRate = saleLine.Discount_Rate;
                        var discountType = saleLine.Discount_Type;
                        ErrorMessage error;
                        _saleLineManager.SetPluCode(ref sale, ref newSaleLine, newSaleLine.PLU_Code, out error);

                        if (newSaleLine.ProductIsFuel && !newSaleLine.IsPropane)
                        {
                            newSaleLine.Regular_Price = Convert.ToDouble(newSaleLine.price);
                        }

                        if (newSaleLine.Gift_Certificate && newSaleLine.GiftType == "LocalGift")
                        {
                            newSaleLine.Description = _resourceManager.GetResString(offSet,8124) + newSaleLine.Gift_Num;
                        }
                        _saleManager.Add_a_Line(ref sale, saleLine, user.Code, sale.TillNumber, out message, adjust: false, tableAdjust: false,
                                 forRefund: true);

                        newSaleLine.No_Loading = false;
                        if (newSaleLine.ScalableItem)
                        {
                            newSaleLine.Regular_Price = newSaleLine.price;
                        }

                        _saleLineManager.SetPrice(ref newSaleLine, price);
                        _saleLineManager.SetQuantity(ref newSaleLine, quantity);
                        _saleLineManager.SetAmount(ref newSaleLine, amount);
                        saleLine.Discount_Type = discountType;
                        _saleLineManager.SetDiscountRate(ref newSaleLine, discountRate);
                        newSaleLine.No_Loading = false; // It has to be set back to false, otherwise if the user changes the qty or price in the POS screen after unsuspend, the amount will not be right
                    }
                    else
                    {
                        saleLineMessages.Add(new ErrorMessage
                        {
                            MessageStyle = _resourceManager.CreateMessage(offSet,0, 8109, saleLine.Stock_Code + " " + saleLine.Description,
                                            CriticalOkMessageType)
                        });
                    }
                }
            }
            sale.ForCorrection = isCorrection; // 06/24/05 Nancy added ForCorrection
                                               //    CL = "" ' code
                                               //SaleMain.Default.lblCustName.Text = sale.Customer.Name;
            foreach (var tempLoopVarSl in sLines)
            {
                sl = tempLoopVarSl;
                sl.No_Loading = false;
            }
            sale.LoadingTemp = false;

            foreach (var tempLoopVarSl in sLines)
            {
                sl = tempLoopVarSl;
                if (_policyManager.GetPol("RET_REASON", sl))
                {
                    blnRecReason = true;
                    break;
                }
            }

            if (blnRecReason)
            {
                ReasonType rType;
                Enum.TryParse(reasonType, true, out rType);
                var returnReason = (char)rType != '\0' ? _reasonService.GetReturnReason(reasonCode, (char)rType)
                    : new Return_Reason { RType = "R" };

                foreach (var tempLoopVarSl in sLines)
                {
                    sl = tempLoopVarSl;
                    _saleManager.Line_Reason(ref sale, ref sl, returnReason);
                }
            }
            CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
            return sale;
        }

        /// <summary>
        /// Checks Whether Correction is allowed
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>True or false</returns>
        public bool IsAllowCorrection(int saleNumber)
        {
            return _returnSaleService.IsAllowCorrection(saleNumber);
        }

        /// <summary>
        /// Checks whether Reason allowed
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>True or false</returns>
        public bool IsReasonAllowed(int saleNumber)
        {
            bool blnRecReason = false;
            var sLines = _returnSaleService.IsReasonAllowed(saleNumber);
            foreach (var sLine in sLines)
            {
                if (_policyManager.GetPol("RET_REASON", sLine))
                {
                    blnRecReason = true;
                    break;
                }
            }
            return blnRecReason;
        }

        #region Private methods

        /// <summary>
        /// Checks whether User is able to return
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="message">Error</param>
        /// <returns>True or false</returns>
        private bool IsUserAbleToReturn(User user, out ErrorMessage message)
        {
            message = new ErrorMessage();

            if (!Convert.ToBoolean(_policyManager.GetPol("U_GIVEREF", user)))
            {
                
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                message = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet,38, 53, null, ExclamationOkMessageType)
                };
                return false;
            }
            return true;
        }

        #endregion
    }
}


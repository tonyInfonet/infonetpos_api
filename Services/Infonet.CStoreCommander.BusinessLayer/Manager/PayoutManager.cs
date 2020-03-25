using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.ADOData;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class PayoutManager : ManagerBase, IPayoutManager
    {
        private readonly ITaxManager _taxManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly IPolicyManager _policyManager;
        private readonly ISaleManager _saleManager;
        private readonly IReceiptManager _receiptManager;
        private readonly IStockService _stockService;
        private readonly IReasonService _reasonService;
        private readonly ITillService _tillService;
        private readonly IUserService _userService;
        private readonly ITenderManager _tenderManager;
        private readonly ITaxService _taxService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="taxManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="policyManager"></param>
        /// <param name="saleManager"></param>
        /// <param name="recieptManager"></param>
        /// <param name="stockService"></param>
        /// <param name="reasonService"></param>
        /// <param name="tillService"></param>
        /// <param name="userService"></param>
        /// <param name="tenderManager"></param>
        /// <param name="taxService"></param>
        public PayoutManager(ITaxManager taxManager, IApiResourceManager resourceManager,
            IPolicyManager policyManager, ISaleManager saleManager, IReceiptManager recieptManager,
            IStockService stockService, IReasonService reasonService, ITillService tillService,
            IUserService userService, ITenderManager tenderManager, ITaxService taxService)
        {
            _taxManager = taxManager;
            _resourceManager = resourceManager;
            _policyManager = policyManager;
            _saleManager = saleManager;
            _receiptManager = recieptManager;
            _stockService = stockService;
            _reasonService = reasonService;
            _tillService = tillService;
            _userService = userService;
            _tenderManager = tenderManager;
            _taxService = taxService;
        }

        /// <summary>
        /// Method to get payout vendor
        /// </summary>
        /// <param name="error">Error</param>
        /// <returns>Vendor payout</returns>
        public VendorPayout GetPayoutVendor(out ErrorMessage error)
        {
            error = new ErrorMessage();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!_policyManager.DO_PAYOUTS)
            {
                MessageType temp_VbStyle8 = (int)MessageType.Exclamation + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet,38, 54, null, temp_VbStyle8);
                return null;
            }
            var vendorPayout = new VendorPayout();
            var po = new Payout();
            var taxes = po.Payout_Taxes;
            _taxManager.Load_Taxes(ref taxes);
            int totals = 0;
            foreach (Payout_Tax tax in taxes)
            {
                if (totals >= 4)
                {
                    MessageType temp_VbStyle = (int)MessageType.Information + MessageType.OkOnly;
                    vendorPayout.Message = _resourceManager.CreateMessage(offSet,23, 90, null, temp_VbStyle);
                    break;
                }

                if (tax.Tax_Active)
                {
                    vendorPayout.Taxes.Add(new Tax
                    {
                        Description = _resourceManager.GetResString(offSet,194) + tax.Tax_Name,
                        Code = tax.Tax_Name,
                        Amount = 0
                    });
                    totals++;
                }
            }
            var vendors = _stockService.GetAllVendors();
            foreach (var vendor in vendors)
            {
                vendorPayout.Vendors.Add(new PayoutVendor
                {
                    Code = vendor.Code,
                    Name = vendor.Name
                });
            }
            var reasons = _reasonService.GetReasons((char)ReasonType.Payouts);
            foreach (Return_Reason reason in reasons)
            {
                vendorPayout.Reasons.Add(new VendorReason
                {
                    Code = reason.Reason,
                    Description = reason.Description
                });
            }
            return vendorPayout;
        }

        /// <summary>
        /// Method to save vendor payout
        /// </summary>
        /// <param name="po">Payout</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="taxes">Taxes</param>
        /// <param name="openDrawer">Open cash drawer</param>
        /// <param name="error">Error message</param>
        /// <returns>Report</returns>
        public Report SaveVendorPayout(Payout po, int tillNumber, string userCode, byte registerNumber,
          List<Tax> taxes, out bool openDrawer, out ErrorMessage error)
        {
            openDrawer = false;
            var sale = _saleManager.GetCurrentSale(po.Sale_Num, tillNumber, 0, userCode, out error);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return null;
            }
            if (!_policyManager.DO_PAYOUTS || (sale != null && sale.Sale_Lines.Count > 0))
            {
                MessageType temp_VbStyle8 = (int)MessageType.Exclamation + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet,38, 54, null, temp_VbStyle8);
                return null;
            }
            if (po.Gross > (decimal)214748.3647)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Maximum payout amount is 214748.3647"
                    }

                };
                return null;
            }
            var salePo = new Sale();
            var tendPo = new Tenders();

            
            

            var curr = Convert.ToString(_policyManager.BASECURR);
            
            if (string.IsNullOrEmpty(curr))
            {
                MessageType temp_VbStyle = (int)MessageType.OkOnly + MessageType.Critical;
                error.MessageStyle = _resourceManager.CreateMessage(offSet,23, 94, null, temp_VbStyle);
                return null;
            }
            var poTaxes = _taxService.GetAllTaxes();
            float sumTaxes = 0;
            foreach (var tax in taxes)
            {
                if (poTaxes.Any(p => p.TaxName == tax.Code))
                {
                    po.Payout_Taxes.Add(tax.Code, "", tax.Amount, true, tax.Code);
                    salePo.Sale_Totals.Sale_Taxes.Add(tax.Code, "I", 0, 0, 0, tax.Amount, po.Gross, 0, 0, "");
                    sumTaxes = (float)(sumTaxes + Conversion.Val(tax.Amount));
                }
                else
                {
                    MessageType temp_VbStyle8 = (int)MessageType.Exclamation + MessageType.OkOnly;
                    error.MessageStyle = new MessageStyle
                    {
                        Message = "Invalid tax entered",
                        MessageType = temp_VbStyle8
                    };
                    return null;
                }
            }
            if (!string.IsNullOrEmpty(po.Vendor.Code))
            {
                po.Vendor = _stockService.GetVendorByCode(po.Vendor.Code);
                if (po.Vendor.Code == null)
                {
                    MessageType temp_VbStyle2 = (int)MessageType.Critical + MessageType.OkOnly;
                    error.MessageStyle = new MessageStyle
                    {
                        Message = "Invalid vendor code",
                        MessageType = temp_VbStyle2
                    };
                    return null;
                }
            }
            // Nicoolette changed next line on Nov 13,2007 to allow negative payouts, based on Mr. Gas requirement
            if (po.Gross == 0)
            {
                //    If PO.Gross <= 0 Then
                //You must specify the Payout Amount, vbCritical + vbOKOnly
                MessageType temp_VbStyle2 = (int)MessageType.Critical + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet,23, 91, null, temp_VbStyle2);
                return null;
            }

            if (_policyManager.PO_REASON)
            {
                po.Return_Reason = _reasonService.GetReturnReason(po.Return_Reason.Reason, (char)ReasonType.Payouts);
                if (po.Return_Reason == null)
                {
                    MessageType temp_VbStyle2 = (int)MessageType.Critical + MessageType.OkOnly;
                    error.MessageStyle = _resourceManager.CreateMessage(offSet,23, 92, null, temp_VbStyle2);
                    return null;
                }
                salePo.Return_Reason = po.Return_Reason;
            }


            var till = _tillService.GetTill(tillNumber);
            till.Cash = till.Cash - po.Gross;
            _tillService.UpdateTill(till);
            if (_policyManager.OPEN_DRAWER == "Every Sale")
            {
                openDrawer = true;
            }
            


            tendPo.Add(curr, "Cash", 1, true, true, false, 1, curr, false, 0, 0, 0.01, true, Convert.ToDouble((object)-po.Gross), 1, true, false, "", "");
            _tenderManager.Set_Amount_Entered(ref tendPo, ref salePo, tendPo[1], -po.Gross, -1);
            // payout

            if (_policyManager.PENNY_ADJ && po.Gross != 0)
            {
                salePo.Sale_Totals.Penny_Adj = modGlobalFunctions.Calculate_Penny_Adj(po.Gross);
            }
            else
            {
                salePo.Sale_Totals.Penny_Adj = 0;
            }
            po.Penny_Adj = salePo.Sale_Totals.Penny_Adj;
            //   end

            salePo.Sale_Totals.Net = po.Gross; // - Sum_Taxes   Nicolette commented out,
                                               // once for payout all taxes are included, don't subtract the taxes,
                                               // this will afect Sale_Amt in SaleHead, March 07, 2003

            var saleTotals = salePo.Sale_Totals;
            _saleManager.SetGross(ref saleTotals, salePo.Sale_Totals.Net);
            salePo.Sale_Totals.Gross = saleTotals.Gross;
            salePo.Sale_Totals.TotalLabel = saleTotals.TotalLabel;
            salePo.Sale_Totals.SummaryLabel = saleTotals.SummaryLabel;

            salePo.Register = Convert.ToByte(registerNumber);
            salePo.Sale_Change = 0;

            salePo.TillNumber = Convert.ToByte(tillNumber);
            salePo.Sale_Date = DateTime.Now;
            salePo.Sale_Tender = 0;
            salePo.Sale_Totals.PayOut = po.Gross;
            salePo.Sale_Change = 0;
            salePo.Sale_Amount = 0;
            if (po.Sale_Num == 0)
            {
                salePo.Sale_Num = _saleManager.GetSaleNo(tillNumber, userCode, out error);
            }
            else
            {
                salePo.Sale_Num = po.Sale_Num;
            }
            
            po.Penny_Adj = salePo.Sale_Totals.Penny_Adj; //  

            salePo.Sale_Type = "PAYOUT";
            salePo.Vendor = po.Vendor.Code;
            var user = _userService.GetUser(userCode);
            var stream = _receiptManager.Print_Payout(po, userCode, user.Name, DateTime.Today,
                salePo.Sale_Date, registerNumber,
                 till);
            stream.Copies = _policyManager.PayoutReceiptCopies;
            _saleManager.SaveSale(salePo, userCode, ref tendPo, null);

            
            _saleManager.Clear_Sale(salePo, po.Sale_Num, salePo.TillNumber, userCode, "", null,
                false, false, false, out error);
            

            return stream;
        }

        /// <summary>
        /// Method to validate payout by fleet card
        /// </summary>
        /// <param name="allowSwipe">Allow Swipe</param>
        /// <param name="error">Error message</param>
        /// <returns>Caption</returns>
        public string ValidateFleetPayout(out bool allowSwipe, out ErrorMessage error)
        {
            allowSwipe = _policyManager.SWIPE_CARD;
            error = new ErrorMessage();
            string caption;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!_policyManager.U_AllowFlPay)
            {
                
                MessageType temp_VbStyle = (int)MessageType.Exclamation + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet,12, 83, null, temp_VbStyle);
                error.StatusCode = System.Net.HttpStatusCode.Forbidden;
                return null;
            }
            // There are no Cards Defined on which payments can be accepted
            if (!_tenderManager.AreCardsDefined())
            {
                MessageType temp_VbStyle = (int)MessageType.Critical + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet,22, 91, null, temp_VbStyle);
                error.StatusCode = System.Net.HttpStatusCode.OK;
            }
            if (allowSwipe && (!_policyManager.ScanLoyCard))
            {
                caption = _resourceManager.CreateCaption(offSet,63, Convert.ToInt16(22), null, 3);//"Manual Entry or Swipe a Card"
            }
            else if (_policyManager.ScanLoyCard)
            {
                caption = _resourceManager.CreateCaption(offSet,63, Convert.ToInt16(22), null, 4); // "Manual Entry or Scan a Card"
                                                                                            
            }
            else
            {
                caption = _resourceManager.CreateCaption(offSet,63, Convert.ToInt16(22), null, 2); // "Manual Entry Only"                                                                                                  //        optAccount.Value = True 'binal
            }
            return caption;
        }
    }
}

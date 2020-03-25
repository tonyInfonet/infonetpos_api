using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using RTVP.POSService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using Constants = Infonet.CStoreCommander.BusinessLayer.Utilities.Constants;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class TaxManager : ManagerBase, ITaxManager
    {
        private readonly ITaxService _taxService;
        private readonly ITreatyService _treatyService;
        private readonly ITreatyManager _treatyManager;
        private readonly ILoginManager _loginManager;
        private readonly ISiteMessageService _siteMessageService;
        private readonly IAiteCardHolderService _aiteCardHolderService;
        private readonly IPurchaseListManager _purchaseListManager;
        private readonly ITeSystemManager _teSystemManager;
        private readonly ISaleLineManager _saleLineManager;
        private readonly ITeCardholderManager _cardholderManager;
        private readonly ITaxExemptSaleLineManager _exemptSaleLineManager;
        private readonly IPolicyManager _policyManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ISaleManager _saleManager;
        private readonly ITenderManager _tenderManager;
        private readonly ICustomerManager _customerManager;
        private readonly IUtilityService _utilityService;
        private readonly ISaleService _saleService;
        private readonly IFuelPumpService _fuelPumpService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="taxService"></param>
        /// <param name="treatyService"></param>
        /// <param name="treatyManager"></param>
        /// <param name="siteMessageService"></param>
        /// <param name="aiteCardHolderService"></param>
        /// <param name="purchaseListManager"></param>
        /// <param name="teSystemManager"></param>
        /// <param name="saleLineManager"></param>
        /// <param name="cardholderManager"></param>
        /// <param name="exemptSaleLineManager"></param>
        /// <param name="policyManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="saleManager"></param>
        /// <param name="loginManager"> </param>
        /// <param name="tenderManager"></param>
        /// <param name="customerManager"></param>
        public TaxManager(
            ITaxService taxService,
            ITreatyService treatyService,
            ITreatyManager treatyManager,
            ILoginManager loginManager,
            ISiteMessageService siteMessageService,
            IAiteCardHolderService aiteCardHolderService,
            IPurchaseListManager purchaseListManager,
            ITeSystemManager teSystemManager,
            ISaleLineManager saleLineManager,
            ITeCardholderManager cardholderManager,
            ITaxExemptSaleLineManager exemptSaleLineManager,
            IPolicyManager policyManager,
            IApiResourceManager resourceManager,
            ISaleManager saleManager,
            ITenderManager tenderManager,
            ICustomerManager customerManager,
            IUtilityService utilityService,
            ISaleService saleService,
            IFuelPumpService fuelPumpService)
        {
            _taxService = taxService;
            _treatyService = treatyService;
            _treatyManager = treatyManager;
            _siteMessageService = siteMessageService;
            _aiteCardHolderService = aiteCardHolderService;
            _purchaseListManager = purchaseListManager;
            _teSystemManager = teSystemManager;
            _saleLineManager = saleLineManager;
            _cardholderManager = cardholderManager;
            _exemptSaleLineManager = exemptSaleLineManager;
            _policyManager = policyManager;
            _resourceManager = resourceManager;
            _saleManager = saleManager;
            _loginManager = loginManager;
            _tenderManager = tenderManager;
            _customerManager = customerManager;
            _utilityService = utilityService;
            _saleService = saleService;
            _fuelPumpService = fuelPumpService;
        }

        private enum TeProductType
        {
            Tobacco = 1,
            Gas = 2,
            Propane = 3
        }

        /// <summary>
        /// Get Taxes
        /// </summary>
        /// <returns></returns>
        public List<string> GetTaxes()
        {
            var taxes = _taxService.GetAllActiveTaxes();
            return taxes.Select(tax => $"{tax.Name} - {tax.Code}").ToList();
        }

        /// <summary>
        /// Method to load taxes for payout
        /// </summary>
        /// <param name="payoutTaxes"></param>
        public void Load_Taxes(ref Payout_Taxes payoutTaxes)
        {
            var taxes = _taxService.GetAllTaxes();

            foreach (var tax in taxes)
            {
                payoutTaxes.Add(tax.TaxName, tax.TaxDescription, 0, tax.Active != null && tax.Active.Value, tax.TaxName); //  - added taxactive for HST change -Me.Add rs!Tax_Name, rs!Tax_Desc, 0, rs!Tax_Name
            }
        }

        /// <summary>
        /// Get Sale Summary
        /// </summary>
        /// <param name="input">Sale summary</param>
        /// <param name="error">Error</param>
        /// <returns>Sale summary response</returns>
        public SaleSummaryResponse GetSaleSummary(SaleSummaryInput input, out ErrorMessage error)
        {
            var result = new SaleSummaryResponse();
            //var user = CacheManager.GetUser(input.UserCode) ?? _userService.GetUser(input.UserCode);
            var sale = _saleManager.GetCurrentSale(input.SaleNumber, input.TillNumber, input.RegisterNumber, input.UserCode, out error);
            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return null;
            }

            if (input.IsSiteValidated)
            {
                CacheManager.RemovePurchaseListSaleForTill(input.TillNumber, input.SaleNumber);
                if (_policyManager.SITE_RTVAL)
                {
                    _saleManager.ApplyTaxes(ref sale, false);
                    _saleManager.ReCompute_Totals(ref sale);
                }
            }
            else if (input.IsAiteValidated)
            {
                var oTeSale = CacheManager.GetTaxExemptSaleForTill(input.TillNumber, input.SaleNumber);
                mPrivateGlobals.theSystem = CacheManager.GetTeSystemForTill(input.TillNumber, input.SaleNumber);
                if (mPrivateGlobals.theSystem == null || oTeSale == null)
                {
                    error = new ErrorMessage
                    {
                        MessageStyle = new MessageStyle { Message = "Request is Invalid", MessageType = 0 },
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    return null;
                }
                if (oTeSale.TobaccoOverLimit)
                {
                    ClearTaxExempt(ref sale, oTeSale, (byte)TeProductType.Tobacco);
                }

                if (oTeSale.GasOverLimit)
                {
                    ClearTaxExempt(ref sale, oTeSale, (byte)TeProductType.Gas);

                }

                if (oTeSale.PropaneOverLimit)
                {
                    ClearTaxExempt(ref sale, oTeSale, (byte)TeProductType.Propane);
                }

                if (oTeSale.Te_Sale_Lines.Count > 0)
                {
                    foreach (TaxExemptSaleLine tempLoopVarTesl in oTeSale.Te_Sale_Lines)
                    {
                        var tesl = tempLoopVarTesl;
                        var rowNumber = tesl.Line_Num;
                        var saleLine = sale.Sale_Lines[rowNumber];


                        if (!(tesl.Amount < 0 && saleLine.IsTaxExemptItem))
                        {
                            saleLine.IsTaxExemptItem = true;
                            saleLine.price = tesl.TaxFreePrice;
                            saleLine.Amount = (decimal)tesl.Amount;
                        }
                    }
                    _saleManager.ReCompute_Totals(ref sale);
                }
            }
            else
            {
                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    var sl = tempLoopVarSl;
                    double taxIncldAmount = 0;
                    foreach (Line_Tax tempLoopVarLtx in sl.Line_Taxes)
                    {
                        var ltx = tempLoopVarLtx;
                        //   added the next If condition for tax collected from tax exempt customers
                        if (!string.IsNullOrEmpty(sl.TE_COLLECTTAX))
                        {
                            taxIncldAmount = 0;
                        }
                        else
                        {
                            //   end
                            taxIncldAmount = taxIncldAmount + ltx.Tax_Added_Amount;
                        }
                    }
                    taxIncldAmount = taxIncldAmount + sl.Quantity * sl.Regular_Price;
                    sl.OrigTaxIncldAmount = (decimal)taxIncldAmount;
                }
                _saleManager.ApplyTaxes(ref sale, true);
            }

            _saleManager.ReCompute_Totals(ref sale);
            var tenders = _tenderManager.GetAllTender(input.SaleNumber, input.TillNumber, "Sale", input.UserCode, false, "", out error);

            result.SaleSummary = SaleSummary(sale);
            result.Tenders = tenders;
            var user = _loginManager.GetUser(input.UserCode);
            result.Tenders.EnableRunAway = EnableRunaway(user, sale);
            return result;
        }

        /// <summary>
        /// Verify Tax Exempt
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>Verify tax exempt</returns>
        public VerifyTaxExempt VerifyTaxExampt(int saleNumber, int tillNumber, int registerNumber, string userCode,
            out ErrorMessage error)
        {

            var verifyTaxExmpt = new VerifyTaxExempt { ConfirmMessage = new ErrorMessage() };
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, (byte)registerNumber, userCode, out error);
            if (error?.MessageStyle?.Message != null)
            {
                return null;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var user = _loginManager.GetUser(userCode);
            var securityInfo = _policyManager.LoadSecurityInfo();
            _loginManager.GetInstallDate(ref securityInfo);
            securityInfo.ExpireDate = DateTime.FromOADate(securityInfo.Install_Date.ToOADate() + 50 * 365); // 50 Year
            if (securityInfo.ExpireDate < DateAndTime.Today)
            {
                //_resourceManager.CreateMessage(offSet,0, (short)8194, MessageType.OkOnly, null, (byte)0);
                //Transactions are not allowed beyond expiry date. Please check the date and restart POS", vbOKOnly, "Software License Expired"
                error = new ErrorMessage
                {
                    //"All defined tills are currently in use. Define more tills or wait until one becomes available.",
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8194, null),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return verifyTaxExmpt;
            }

            if (user.User_Group.Code == Entities.Constants.Trainer) //Behrooz Jan-12-06
            {
                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    var sl = tempLoopVarSl;
                    if (!sl.ProductIsFuel || sl.IsPropane) continue;
                    //_resourceManager.CreateMessage(offSet,this, (short)56, temp_VbStyle, null, (byte)0);
                    error = new ErrorMessage
                    {
                        //"All defined tills are currently in use. Define more tills or wait until one becomes available.",
                        MessageStyle = _resourceManager.CreateMessage(offSet, 11, 56, null, CriticalOkMessageType),
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                    return verifyTaxExmpt;
                }
            }


            if (sale.HasRebateLine && !sale.Customer.UseFuelRebate)
            {

                //_resourceManager.CreateMessage(offSet,this, (short)48, temp_VbStyle2, null, (byte)0); 
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 48, null, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return verifyTaxExmpt;
            }

            var hasFuelSale = false;


            if (Math.Round(sale.Sale_Totals.Gross, 2) != 0)
            {


                if (_policyManager.CouponMSG)
                {
                    if (sale.Sale_Totals.Gross >= _policyManager.CupnThrehld)
                    {
                        float fuelSaleAmount = 0;
                        foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                        {
                            var sl = tempLoopVarSl;
                            if (sl.ProductIsFuel && !sl.IsPropane)
                            {
                                hasFuelSale = true;
                                fuelSaleAmount = fuelSaleAmount + (float)sl.Amount;
                            }
                        }

                        if (hasFuelSale && (fuelSaleAmount >= _policyManager.CupnThrehld))
                        {

                            error = new ErrorMessage
                            {
                                //"All defined tills are currently in use. Define more tills or wait until one becomes available.",
                                MessageStyle = _resourceManager.CreateMessage(offSet, 11, 58, _policyManager.CouponType),
                                StatusCode = HttpStatusCode.Unauthorized
                            };
                        }
                    }
                }



                if (!_policyManager.TAX_EXEMPT) return verifyTaxExmpt;

                if (sale.DeletePrepay) return verifyTaxExmpt;


                switch (_policyManager.TE_Type)
                {
                    case "SITE":

                        if (sale.Sale_Totals.Gross < 0)
                        {
                            verifyTaxExmpt.ProcessSiteReturn = true;
                        }
                        else
                        {
                            WriteToLogFile("Before calling ProcessSITE_Sale");
                            verifyTaxExmpt.ProcessSiteSale = true;
                            if (sale.Customer.TaxExempt && _policyManager.ENABLE_BANDACCT)
                            {
                                verifyTaxExmpt.TreatyNumber = Convert.ToString(!string.IsNullOrEmpty(sale.Customer.TECardNumber) ? sale.Customer.TECardNumber : ""); //  ' band account
                                verifyTaxExmpt.TreatyName = _treatyService.GetTreatyName(verifyTaxExmpt.TreatyNumber);

                            }
                            if (_policyManager.SITE_RTVAL)
                            {
                                verifyTaxExmpt.ProcessSiteSaleReturnTax = true;
                            }
                            WriteToLogFile("After calling ProcessSITE_Sale");
                        }
                        break;
                    case "AITE":
                        verifyTaxExmpt.ProcessAite = true;
                        break;
                    case "QITE":
                        verifyTaxExmpt.ProcessQite = true;
                        break;
                }
            }
            else
            {
                if (sale.Sale_Lines.Count == 0)
                {
                    // No Items in the Sale
                    //_resourceManager.CreateMessage(offSet,0, (short)8121, MessageType.OkOnly, null, (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8121, null, MessageType.OkOnly),
                        StatusCode = HttpStatusCode.Conflict
                    };
                }
                else
                {
                    //Ans = TIMsgbox("Sale total is zero." & vbCrLf & "No Tenders Required" & vbCrLf & vbCrLf & _
                    //"Complete the Sale?", vbQuestion + vbYesNo, "Zero Sale Total", Me)
                    //var ans = (short)(_resourceManager.CreateMessage(offSet,this, (short)95, temp_VbStyle4, null, (byte)0));
                    verifyTaxExmpt.ConfirmMessage = new ErrorMessage
                    {
                        //"All defined tills are currently in use. Define more tills or wait until one becomes available.",
                        MessageStyle = _resourceManager.CreateMessage(offSet, 11, 95, null, YesNoQuestionMessageType),
                        StatusCode = HttpStatusCode.Conflict
                    };
                }
            }
            return verifyTaxExmpt;
        }

        /// <summary>
        /// Validate AITE Card
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <param name="cardNumber">Card number</param>
        /// <param name="barCode">Bar code</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="checkMode">Check mode</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>AITE response</returns>
        public AiteCardResponse ValidateAiteCard(int saleNumber, int tillNumber, int shiftNumber, string cardNumber,
            string barCode, byte registerNumber, byte checkMode,
            string userCode, out ErrorMessage error)
        {
            var isBarCode = false;
            string strNumber;
            short matchCount = 0;
            var oTeCardHolder = new teCardholder();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (_policyManager.TE_Type != "AITE")
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Please select AITE Tax Exempt policy in BackOffice",
                        MessageType = 0
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }
            if (string.IsNullOrEmpty(barCode.Trim()) && string.IsNullOrEmpty(cardNumber.Trim()))
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        MessageType = OkMessageType,
                        Message = "Please provide BarCode of AITE card number"
                    },
                    StatusCode = HttpStatusCode.Conflict
                };
                return null;
            }

            if (checkMode == 1)
            {
                isBarCode = true;
                strNumber = barCode.Trim();
            }
            else
            {
                string tempUserInputString = cardNumber;
                string tempAvoidedValuesString = "";
                strNumber = Strings.Trim(Convert.ToString(Helper.SqlQueryCheck(ref tempUserInputString, ref tempAvoidedValuesString)));
            }

            string[] msgArr = new string[3];

            if (!_cardholderManager.ValidCardHolder(ref oTeCardHolder, isBarCode, strNumber, ref matchCount))
            {

                if (matchCount > 1)
                {
                    msgArr[1] = matchCount.ToString();
                    msgArr[2] = strNumber;
                    if (isBarCode)
                    {
                        //MsgBox ("There're " & MatchCount & " cardholders found for bar code " & strNumber & ".~Invalid bar code!")
                        //_resourceManager.CreateMessage(offSet,this, (short)64, MessageType.Information, msgArr, (byte)0);
                        error = new ErrorMessage
                        {
                            MessageStyle = _resourceManager.CreateMessage(offSet, 51, 64, msgArr),
                            StatusCode = HttpStatusCode.Conflict
                        };
                    }
                    else
                    {
                        //MsgBox ("There're " & MatchCount & " cardholders found for Card Number " & strNumber & ".~Invalid Card Number!")
                        //_resourceManager.CreateMessage(offSet,this, (short)65, MessageType.Information, msgArr, (byte)0);
                        error = new ErrorMessage
                        {
                            MessageStyle = _resourceManager.CreateMessage(offSet, 51, 65, msgArr),
                            StatusCode = HttpStatusCode.Conflict
                        };
                    }
                    return null;
                }


                if (isBarCode)
                {
                    //MsgBox ("Bar code " & strNumber & " doesn't match with any cardholder!")
                    //_resourceManager.CreateMessage(offSet,this, (short)61, MessageType.Information, strNumber, (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 51, 61, strNumber),
                        StatusCode = HttpStatusCode.Conflict
                    };
                }
                else
                {
                    //MsgBox ("Card Number " & strNumber & " doesn't match with any cardholder!")
                    //_resourceManager.CreateMessage(offSet,this, (short)62, MessageType.Information, strNumber, (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 51, 62, strNumber),
                        StatusCode = HttpStatusCode.Conflict
                    };
                }
                return null;
            }

            //if (!(_policyManager.TE_Type == "AITE" && oTeCardHolder.Barcode.Trim().Length != 0))
            //{
            //    lblMessage.Text = Chaps_Main.Resource.DisplayCaption(System.Convert.ToInt16(lblMessage.Tag), System.Convert.ToInt16(this.Tag), strNumber, (short)2);
            //}
            //else
            //{
            //    lblMessage.Text = "";
            //}

            Sale sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out error);

            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return null;
            }
            var result = ProcessAite(sale, oTeCardHolder, userCode, shiftNumber, out error);

            return result;
        }

        /// <summary>
        /// Affix Bar Code
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="barCode">Bar code</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        public bool AffixBarCode(string cardNumber, string barCode, out ErrorMessage error)
        {
            var oTeCardHolder = new teCardholder();
            error = new ErrorMessage();
            var arrMsg = new string[3];
            short matchCount = 0;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (string.IsNullOrEmpty(barCode.Trim()) && string.IsNullOrEmpty(cardNumber.Trim()))
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        MessageType = OkMessageType,
                        Message = "Please provide BarCode of AITE card number"
                    },
                    StatusCode = HttpStatusCode.Conflict
                };
                return false;
            }


            var tempUserInputString = cardNumber;
            var tempAvoidedValuesString = "";
            var cardNum = Strings.Trim(Convert.ToString(Helper.SqlQueryCheck(ref tempUserInputString, ref tempAvoidedValuesString)));

            if (!_cardholderManager.ValidCardHolder(ref oTeCardHolder, false, cardNum, ref matchCount))
            {
                var msgArr = new string[3];


                if (matchCount > 1)
                {
                    msgArr[1] = matchCount.ToString();
                    msgArr[2] = cardNum;
                    //MsgBox ("There're " & MatchCount & " cardholders found for Card Number " & strNumber & ".~Invalid Card Number!")
                    //_resourceManager.CreateMessage(offSet,this, (short)65, MessageType.Information, msgArr, (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 51, 65, msgArr),
                        StatusCode = HttpStatusCode.Conflict
                    };
                    return false;
                }
                //MsgBox ("Card Number " & strNumber & " doesn't match with any cardholder!")
                //_resourceManager.CreateMessage(offSet,this, (short)62, MessageType.Information, strNumber, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 51, 62, cardNum),
                    StatusCode = HttpStatusCode.Conflict
                };
                return false;
            }

            if (string.IsNullOrEmpty(barCode.Trim()) && string.IsNullOrEmpty(cardNumber.Trim()))
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        MessageType = OkMessageType,
                        Message = "Please provide BarCode of AITE card number"
                    },
                    StatusCode = HttpStatusCode.Conflict
                };
                return false;
            }
            if (!oTeCardHolder.IsValidCardHolder)
            {
                return false;
            }
            if (oTeCardHolder.CardNumber == "")
            {
                return false;
            }
            string strNumber = barCode.Trim();

            //
            if (strNumber == oTeCardHolder.Barcode)
            {
                return false;
            }
            //

            if (_policyManager.TE_Type == "AITE" && (strNumber.Length != 12 || Strings.Left(strNumber, 1) != "2"))
            {

                //Chaps_Main.DisplayMessage(this, (short)95, MessageType.OkOnly, null, (byte)0);

                //_resourceManager.CreateMessage(offSet,this, (short)95, MessageType.OkOnly, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 51, 95, null),
                    StatusCode = HttpStatusCode.Conflict
                };
                return false;
            }


            arrMsg[1] = strNumber;
            arrMsg[2] = oTeCardHolder.CardNumber;
            //MessageType temp_VbStyle = (int)MessageType.Question + MessageType.YesNo;
            //var Ans = (short)(_resourceManager.CreateMessage(offSet,this, (short)63, temp_VbStyle, arrMsg, (byte)0));
            //if (Ans != (short)MsgBoxResult.Yes)
            //{
            //    
            //    txtBarCode.Text = oTeCardHolder.Barcode;
            //    txtBarCode.SelectionStart = 0;
            //    txtBarCode.SelectionLength = txtBarCode.Text.Length;
            //    
            //    return;
            //}

            if (!_aiteCardHolderService.AffixBarcode(barCode, cardNumber, oTeCardHolder.ValidateMode))
            {
                //MsgBox ("Failed to affix bar code to card number!")
                //_resourceManager.CreateMessage(offSet,this, (short)60, MessageType.Information, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 51, 60, null),
                    StatusCode = HttpStatusCode.Conflict
                };
                return false;
            }
            return true;
        }

        /// <summary>
        /// Exempt AITE GST/PST
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <param name="treatyNumber">Treaty number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>AITE card response</returns>
        public AiteCardResponse AiteGstPstExempt(int saleNumber, int tillNumber, int shiftNumber, string
            treatyNumber, byte registerNumber, string userCode, out ErrorMessage error)
        {
            var oTeCardHolder = new teCardholder();

            if (treatyNumber.Length < 3 || treatyNumber.Length > 20)
            {

                //_resourceManager.CreateMessage(offSet,0, (short)5191, temp_VbStyle, null, (byte)0);
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5191, null, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.Conflict
                };
                return null;
            }
            oTeCardHolder.GstExempt = true;
            oTeCardHolder.CardholderID = treatyNumber.Trim();
            oTeCardHolder.IsValidCardHolder = true;

            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out error);
            if (error?.MessageStyle?.Message != null)
            {
                return null;
            }
            var result = ProcessAite(sale, oTeCardHolder, userCode, shiftNumber, out error);
            return result;
        }


        /// <summary>
        /// Validate QITE Band Member
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="bandMember">Band member</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>QITE response</returns>
        public QiteCardResponse ValidateQiteBandMember(int saleNumber, int tillNumber,
            int shiftNumber, byte registerNumber, string bandMember, string userCode, out ErrorMessage error)
        {
            var oTeCardHolder = new teCardholder();

            if (_policyManager.TE_Type != "QITE")
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Please select QITE Tax Exempt policy in BackOffice",
                        MessageType = 0
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }

            if (string.IsNullOrEmpty(bandMember.Trim()))
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        MessageType = OkMessageType,
                        Message = "Please provide Band Member Number"
                    },
                    StatusCode = HttpStatusCode.Conflict
                };
                return null;
            }


            string tempUserInputString = bandMember;
            string tempAvoidedValuesString = "";
            string strNumber = Strings.Trim(Convert.ToString(Helper.SqlQueryCheck(ref tempUserInputString, ref tempAvoidedValuesString)));


            if (!_cardholderManager.ValidTaxExemptCustomer(ref oTeCardHolder, strNumber))
            {
                //MsgBox ("Card Number " & strNumber & " doesn't match with any cardholder!")
                //_resourceManager.CreateMessage(offSet,this, (short)66, MessageType.Information, strNumber, (byte)0);
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 51, 66, strNumber),
                    StatusCode = HttpStatusCode.Conflict
                };
                return null;
            }

            Sale sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out error);
            if (error?.MessageStyle?.Message != null)
            {
                return null;
            }
            var result = ProcessQite(sale, oTeCardHolder, userCode, shiftNumber, out error);
            result.BandMemberName = _customerManager.GetCustomerByCode(strNumber).Name;
            return result;
        }

        /// <summary>
        /// Remove Tax
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="treatyNumber">Treaty number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="intcaptureMethod">Capture method</param>
        /// <param name="documentNumber">Document number</param>
        /// <param name="error">Error</param>
        /// <returns>Treaty number response</returns>
        public TreatyNumberResponse RemoveTax(int saleNumber, int tillNumber, string treatyNumber, byte registerNumber, string userCode, short intcaptureMethod, string documentNumber, out ErrorMessage error)
        {
            string sPrompt = "";
            var tetreatyNo = new teTreatyNo();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!_policyManager.SITE_RTVAL || _policyManager.TE_Type != "SITE")
            {
                error = new ErrorMessage
                {
                    MessageStyle =
                        new MessageStyle
                        {
                            Message = "Please select SITE Tax Exempt and enable Real time validation policy in BackOffice",
                            MessageType = MessageType.OkOnly
                        },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }

            var user = CacheManager.GetUser(userCode) ?? _loginManager.GetUser(userCode);
            if (string.IsNullOrEmpty(treatyNumber))
            {
                //MsgBox "Treaty Number cannot be blank", vbOKOnly
                //_resourceManager.CreateMessage(offSet,0, (short)5192, MessageType.Critical, sPrompt, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5192, sPrompt, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }

            var isNumeric = treatyNumber.All(char.IsDigit);

            if (!isNumeric || treatyNumber.IndexOf(".", StringComparison.Ordinal) + 1 > 0 || treatyNumber.IndexOf(",", StringComparison.Ordinal) + 1 > 0)
            {
                sPrompt = "Invalid Treaty Number \"" + treatyNumber + "\".  Please re-enter 10 digit treaty number";
                //_resourceManager.CreateMessage(offSet,0, (short)5191, MessageType.Critical, sPrompt, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5191, sPrompt, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }
            string tempSTreatyNo = treatyNumber;
            if (!_treatyManager.IsValidTreatyNo(ref tempSTreatyNo, ref intcaptureMethod, user, out error))
            {
                sPrompt = "Invalid Treaty Number \"" + treatyNumber + "\".  Please re-enter 10 digit treaty number";
                //_resourceManager.CreateMessage(offSet,0, (short)5191, MessageType.Critical, sPrompt, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5191, sPrompt, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }

            if (!string.IsNullOrEmpty(documentNumber))
            {
                // max doc number for real time validation has to be 32767 because parameter in the dll function is an integer
                // Ensure that document number is entered to support the override
                if (documentNumber == "")
                {
                    //MessageType temp_VbStyle2 = (int)MessageType.Critical + MessageType.OkOnly;
                    //_resourceManager.CreateMessage(offSet,0, (short)5291, temp_VbStyle2, "doc req\'d", (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5291, "doc req\'d", CriticalOkMessageType),
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                    return null;
                }
                if (!Information.IsNumeric(documentNumber))
                {
                    //_resourceManager.CreateMessage(offSet,0, (short)5293, temp_VbStyle3, "doc req\'d", (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5293, "doc req\'d", CriticalOkMessageType),
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                    return null;
                }

                tetreatyNo.SendOverride = true;
                tetreatyNo.OverrideNumber = documentNumber.Trim(); //   changed from Integer to String
            }

            tetreatyNo.TreatyNumber = treatyNumber.Trim();
            tetreatyNo.RemoveTax = true;
            if (!string.IsNullOrEmpty(treatyNumber))
            {
                tetreatyNo.Name = _treatyService.AddorUpdateTreatyName(treatyNumber, "");
            }
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out error);
            if (error?.MessageStyle?.Message != null)
            {
                return null;
            }
            var result = new TreatyNumberResponse
            {
                TreatyNumber = tetreatyNo.TreatyNumber,
                TreatyCustomerName = tetreatyNo.Name,
                PermitNumber = tetreatyNo.OverrideNumber
            };

            if (_policyManager.SITE_RTVAL && tetreatyNo.RemoveTax)
            {
                _saleManager.ApplyTaxes(ref sale, false);
                _saleManager.ReCompute_Totals(ref sale);
            }

            foreach (Sale_Line sl in sale.Sale_Lines)
            {
                if (sl.Total_Amount == 0)
                {
                    sl.Total_Amount = sl.Amount;
                }
            }
            
            var tenders = _tenderManager.GetAllTender(sale.Sale_Num, sale.TillNumber, "Sale", user.Code, false, "", out error);

            result.SaleSummary = SaleSummary(sale);
            result.Tenders = tenders;
            result.Tenders.EnableRunAway = EnableRunaway(user, sale);
            for (int i = 1; i <= sale.Sale_Lines.Count(); i++)
            {
                if (sale.Sale_Lines[i].IsTaxExemptItem == false && sale.Sale_Lines[i].Line_Taxes.Count != 0 )
                {
                   // sale.Sale_Lines[i].Line_Taxes = null;
                    sale.Sale_Lines[i].IsTaxExemptItem = true;
                }
            }
            _saleManager.ReCompute_Totals( ref sale);

            _saleService.SaveSale(sale.TillNumber, sale.Sale_Num, sale);
            return result;
        }

        /// <summary>
        /// Method to get treaty name
        /// </summary>
        /// <param name="treatyNumber">Treaty number</param>
        /// <param name="intcaptureMethod">Capture method</param>
        /// <param name="userCode">Userb code</param>
        /// <param name="error">Error</param>
        /// <returns>Treaty name</returns>
        public string GetTreatyName(string treatyNumber, short intcaptureMethod, string userCode, out ErrorMessage
            error)
        {
            var sPrompt = string.Empty;
            var treatyName = string.Empty;
            var user = _loginManager.GetUser(userCode);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (string.IsNullOrEmpty(treatyNumber))
            {
                //MsgBox "Treaty Number cannot be blank", vbOKOnly
                //_resourceManager.CreateMessage(offSet,0, (short)5192, MessageType.Critical, sPrompt, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5192, sPrompt, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }

            bool isNumeric = treatyNumber.All(char.IsDigit);

            if (!isNumeric || treatyNumber.IndexOf(".", StringComparison.Ordinal) + 1 > 0 || treatyNumber.IndexOf(",", StringComparison.Ordinal) + 1 > 0)
            {
                sPrompt = "Invalid Treaty Number \"" + treatyNumber + "\".  Please re-enter 10 digit treaty number";
                //_resourceManager.CreateMessage(offSet,0, (short)5191, MessageType.Critical, sPrompt, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5191, sPrompt, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }
            var tempSTreatyNo = treatyNumber;

            if (!_treatyManager.IsValidTreatyNo(ref tempSTreatyNo, ref intcaptureMethod, user, out error))
            {
                sPrompt = "Invalid Treaty Number \"" + treatyNumber + "\".  Please re-enter 10 digit treaty number";
                //_resourceManager.CreateMessage(offSet,0, (short)5191, MessageType.Critical, sPrompt, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5191, sPrompt, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }
            if (!string.IsNullOrEmpty(treatyNumber))
            {
                treatyName = _treatyService.GetTreatyName(treatyNumber);
            }
            return treatyName;
        }


        /// <summary>
        /// Validate treaty number response
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="treatyNumber">Treaty number</param>
        /// <param name="treatyName">Treaty name</param>
        /// <param name="documentNumber">Document number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="intcaptureMethod">Capture method</param>
        /// <param name="isEnterPressed">Enter pressed or not</param>
        /// <param name="error">Error</param>
        /// <returns>Treaty number response</returns>
        public TreatyNumberResponse Validate(int saleNumber, int tillNumber, string treatyNumber, string treatyName,
            string documentNumber, byte registerNumber, string userCode, short intcaptureMethod, bool isEnterPressed, out ErrorMessage
            error)
        {
            var sPrompt = "";
            var user = _loginManager.GetExistingUser(userCode);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var oTreatyNo = new teTreatyNo();
            oTreatyNo.Name = treatyName;
            oTreatyNo.TreatyNumber = treatyNumber.Trim();

            if (_policyManager.TE_Type != "SITE")
            {
                error = new ErrorMessage
                {
                    MessageStyle =
                        new MessageStyle
                        {
                            Message = "Please select SITE Tax Exempt policy in BackOffice",
                            MessageType = MessageType.OkOnly
                        },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }

            if (string.IsNullOrEmpty(treatyNumber))
            {
                //_resourceManager.CreateMessage(offSet,0, (short)5192, MessageType.Critical, sPrompt, (byte)0);
                error = new ErrorMessage
                {
                    //MsgBox "Treaty Number cannot be blank", vbOKOnly
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5192, sPrompt, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return null;
            }

            bool isNumeric = treatyNumber.All(char.IsDigit);
            if (!isNumeric || treatyNumber.IndexOf(".", StringComparison.Ordinal) + 1 > 0 || treatyNumber.IndexOf(",", StringComparison.Ordinal) + 1 > 0)
            {
                sPrompt = "Invalid Treaty Number \"" + treatyNumber + "\".  Please re-enter 10 digit treaty number";
                //_resourceManager.CreateMessage(offSet,0, (short)5191, MessageType.Critical, sPrompt, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5191, sPrompt, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }
            string tempSTreatyNo = treatyNumber;

            var retailerId = _utilityService.GetAdminValue("RetailerAccountNo");

            Variables.RTVPService = new Transaction();

            var ret = Convert.ToInt16(Variables.RTVPService.SetRetailer(Convert.ToInt32(retailerId), tillNumber, saleNumber));

            WriteToLogFile("Response is " + Convert.ToString(ret) + " from SetRetailer sent with parameters " + retailerId + "," + Convert.ToString(tillNumber) + "," + Convert.ToString(saleNumber));
            
            if (!_treatyManager.IsValidTreatyNo(ref tempSTreatyNo, ref intcaptureMethod, user, out error))
            {
                if (isEnterPressed)
                {
                    return null;
                }
                if (!_policyManager.TAX_EXEMPT_FNGTR)
                {
                    sPrompt = "Invalid Treaty Number \"" + treatyNumber + "\".  Please re-enter 10 digit treaty number";
                    //_resourceManager.CreateMessage(offSet,0, (short)5191, MessageType.Critical, sPrompt, (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5191, sPrompt, CriticalOkMessageType),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
                return null;
            }

            if (_policyManager.TE_GETNAME && string.IsNullOrEmpty(treatyName))
            {
                //_resourceManager.CreateMessage(offSet,0, (short)5193, temp_VbStyle, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5193, null, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return null;
            }

            //  - For customer Name
            //if customer already exist bring customer name, otherwise ask for customername
            //   next if only, for real time validation this checking is not necessary
            // no need to check tax exempt type (SITE, AITE...) because this policy should be true only for SITE
            if (!_policyManager.SITE_RTVAL)
            {
                if (!string.IsNullOrEmpty(treatyName))
                {
                    treatyName = _treatyService.AddorUpdateTreatyName(treatyNumber, treatyName);
                }
            }
            else //   Else part only
            {
                if (!string.IsNullOrEmpty(documentNumber))
                {
                    // max doc number for real time validation has to be 32767 because parameter in the dll function is an integer
                    // Ensure that document number is entered to support the override
                    if (documentNumber == "")
                    {
                        //MessageType temp_VbStyle2 = (int)MessageType.Critical + MessageType.OkOnly;
                        //_resourceManager.CreateMessage(offSet,0, (short)5291, temp_VbStyle2, "doc req\'d", (byte)0);
                        error = new ErrorMessage
                        {
                            MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5291, "doc req\'d", CriticalOkMessageType),
                            StatusCode = HttpStatusCode.Unauthorized
                        };
                        return null;
                    }
                    if (!Information.IsNumeric(documentNumber))
                    {
                        //_resourceManager.CreateMessage(offSet,0, (short)5293, temp_VbStyle3, "doc req\'d", (byte)0);
                        error = new ErrorMessage
                        {
                            MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5293, "doc req\'d", CriticalOkMessageType),
                            StatusCode = HttpStatusCode.Unauthorized
                        };
                        return null;
                    }

                    oTreatyNo.SendOverride = true;
                    oTreatyNo.OverrideNumber = documentNumber.Trim(); //   changed from Integer to String
                }
            }

            oTreatyNo.Name = treatyName;
            oTreatyNo.TreatyNumber = treatyNumber.Trim();
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, userCode, out error);
            if (error?.MessageStyle?.Message != null)
            {
                return null;
            }

            var trestyNumResp = sale.Sale_Totals.Gross >= 0 ? ProcessSiteSale(sale, oTreatyNo, user, out error) : ProcessSiteReturn(sale, oTreatyNo, user, out error);
            return trestyNumResp;
        }

        public TreatyNumberResponse ProcessFngtrSale(int tillNumber, int saleNumber, byte registerNumber,
            string stfdNumber, out ErrorMessage error)
        {
            short i;
            short nOverRide = 0;

            var result = new TreatyNumberResponse();
            error = new ErrorMessage();

            var oPurchaseList = CacheManager.GetPurchaseListSaleForTill(tillNumber, saleNumber);
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, registerNumber, UserCode, out error);
            if (error?.MessageStyle?.Message != null)
            {
                return null;
            }

            Variables.STFDNumber = stfdNumber;
            var oTreatyNo = oPurchaseList.GetTreatyNo();

            if (!string.IsNullOrEmpty(Variables.STFDNumber))
            {
                if (Information.IsNumeric(Variables.STFDNumber))
                {
                    oTreatyNo.PhoneNumber = Variables.STFDNumber;
                    Variables.STFDNumber = ""; //reset STFDNumber
                }
                else
                {
                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    Variables.STFDNumber = "0";
                    result.IsFngtr = true;
                    result.FngtrMessage = _resourceManager.GetResString(offSet, 487);
                    _saleManager.ReCompute_Totals(ref sale);

                    CacheManager.AddPurchaseListSaleForTill(sale.TillNumber, sale.Sale_Num, oPurchaseList);
                    if (mPrivateGlobals.theSystem != null)
                    {
                        CacheManager.AddTeSystemForTill(sale.TillNumber, sale.Sale_Num, mPrivateGlobals.theSystem);
                    }
                    return result;
                }
            }

            if (oPurchaseList.Count() > 0)
            {
                for (i = 1; i <= oPurchaseList.Count(); i++)
                {
                    var rowNumber = oPurchaseList.Item(i).GetRowInSalesMain();

                    var saleLine = sale.Sale_Lines[rowNumber];





                    if (saleLine.Amount < 0 && saleLine.IsTaxExemptItem)
                    {
                        oPurchaseList.Item(i).SetTaxFreePrice((float)saleLine.price);
                        oPurchaseList.Item(i).WasTaxExemptReturn = true;
                    }
                    else
                    {
                        oPurchaseList.Item(i).WasTaxExemptReturn = false;


                        saleLine.IsTaxExemptItem = true;

                        //  added this to take the cost and price for Tax exempt vandor ( if there is a special setup for TE Vendor- otherwise use active vendor values
                        if (string.IsNullOrEmpty(saleLine.TEVendor)) // If TE vendor is not defined consider TE vendor  same as activevendor
                        {
                            saleLine.TEVendor = saleLine.Vendor;
                            saleLine.TECost = saleLine.Cost;
                        }
                        else
                        {
                            if (saleLine.Vendor == saleLine.TEVendor) // IF TAX EXEMPT VENDOR
                            {
                                saleLine.TEVendor = saleLine.Vendor;
                                saleLine.TECost = saleLine.Cost;
                            }
                            else
                            {
                                // Set the cost for the product based on the TE vendor,
                                var cost = _siteMessageService.GetStockCost(saleLine.Stock_Code, saleLine.TEVendor);
                                saleLine.TECost = cost.HasValue ? cost.Value : saleLine.Cost;
                            }
                            if (saleLine.TEVendor != saleLine.Vendor)
                            {
                                var getVendorPrice = true;

                                saleLine.Vendor = saleLine.TEVendor;
                                saleLine.Cost = saleLine.TECost;

                                if (_policyManager.TE_ByRate && getVendorPrice && saleLine.ProductIsFuel == false) //shiny - need to do the price change only for nonfuel products
                                {
                                    saleLine.Price_Number = sale.Customer.Price_Code != 0 ? sale.Customer.Price_Code : (short)1;
                                    oPurchaseList.Item(i).SetTaxFreePrice((float)saleLine.price);
                                    oPurchaseList.Item(i).SetOriginalPrice((float)saleLine.Regular_Price);
                                }
                            }
                        }
                        // settings
                        saleLine.OrigVendor = saleLine.Vendor;
                        saleLine.OrigCost = saleLine.Cost;
                        //   end

                        _saleLineManager.SetPrice(ref saleLine, oPurchaseList.Item(i).GetTaxFreePrice());
                        //saleLine.price = oPurchaseList.Item(i).GetTaxFreePrice();
                        // 

                        if (oPurchaseList.Item(i).GetOverrideCode(ref nOverRide))
                        {
                            saleLine.overrideCode = nOverRide;
                        }



                        //saleLine.Amount = decimal.Parse((saleLine.Quantity * saleLine.price).ToString("#0.00"));
                        _saleLineManager.SetAmount(ref saleLine, decimal.Parse((saleLine.Quantity * saleLine.price).ToString("#0.00")));
                        if (saleLine.Prepay)
                        {
                            saleLine.No_Loading = false; // 
                        }
                    }
                }
            }


            _saleManager.ReCompute_Totals(ref sale);

            CacheManager.AddPurchaseListSaleForTill(sale.TillNumber, sale.Sale_Num, oPurchaseList);
            if (mPrivateGlobals.theSystem != null)
            {
                CacheManager.AddTeSystemForTill(sale.TillNumber, sale.Sale_Num, mPrivateGlobals.theSystem);
            }

            var tenders = _tenderManager.GetAllTender(sale.Sale_Num, sale.TillNumber, "Sale", UserCode, false, "", out error);
            var user = CacheManager.GetUser(UserCode);
            result.SaleSummary = SaleSummary(sale);
            result.Tenders = tenders;
            result.Tenders.EnableRunAway = EnableRunaway(user, sale);
            return result;
        }


        #region Private methods

        /// <summary>
        /// Sale Summary
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <returns>Sale summarys</returns>
        private Dictionary<string, string> SaleSummary(Sale sale)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Sale_Tax saleTaxRenamed;
            decimal curTotalTaxes = 0;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (sale.Sale_Totals.Gross < 0)
            {
                result.Add(_resourceManager.GetResString(offSet, 266), sale.Sale_Totals.Net.ToString("###,##0.00"));
                //topLeftText = _resourceManager.GetResString(offSet,(short)266) + "\r\n"; //"Net Refund "
                //topRightText = sale.Sale_Totals.Net.ToString("###,##0.00") + "\r\n";

                foreach (Sale_Tax tempLoopVarSaleTaxRenamed in sale.Sale_Totals.Sale_Taxes)
                {
                    saleTaxRenamed = tempLoopVarSaleTaxRenamed;
                    if ((double)Math.Abs(saleTaxRenamed.Tax_Added_Amount) > 0.005)
                    {
                        //                mvarTopLeft.Caption = mvarTopLeft.Caption & Sale_Tax.Tax_Name & vbCrLf
                        //                mvarTopRight.Caption = mvarTopRight.Caption & Format(Sale_Tax.Tax_Added_Amount, "###,##0.00") & vbCrLf
                        curTotalTaxes = curTotalTaxes + saleTaxRenamed.Tax_Added_Amount;
                    }
                }

                result.Add(_resourceManager.GetResString(offSet, 137), curTotalTaxes.ToString("###,##0.00"));

                //topLeftText = topLeftText + _resourceManager.GetResString(offSet,(short)137) + "\r\n";
                //topRightText = topRightText + curTotalTaxes.ToString("###,##0.00") + "\r\n";

                if (sale.Sale_Totals.Charge != 0)
                {
                    result.Add(_resourceManager.GetResString(offSet, 138), sale.Sale_Totals.Charge.ToString("###,##0.00"));
                    //topLeftText = topLeftText + _resourceManager.GetResString(offSet,(short)138) + "\r\n"; //"Charges "
                    //topRightText = topRightText + sale.Sale_Totals.Charge.ToString("###,##0.00") + "\r\n";
                }
                //topLeftText = topLeftText + "\r\n";
                //topRightText = topRightText + "________" + "\r\n";

                result.Add(_resourceManager.GetResString(offSet, 210), sale.Sale_Totals.Gross.ToString("###,##0.00"));
                //topLeftText = topLeftText + _resourceManager.GetResString(offSet,(short)210); //"Total"
                //topRightText = topRightText + sale.Sale_Totals.Gross.ToString("###,##0.00");
            }
            else
            {
                result.Add(_resourceManager.GetResString(offSet, 267), sale.Sale_Totals.Net.ToString("###,##0.00"));
                //topLeftText = _resourceManager.GetResString(offSet,(short)267) + " : " + "\r\n"; //Net Sale
                //topRightText = sale.Sale_Totals.Net.ToString("###,##0.00") + "\r\n";

                foreach (Sale_Tax tempLoopVarSaleTaxRenamed in sale.Sale_Totals.Sale_Taxes)
                {
                    saleTaxRenamed = tempLoopVarSaleTaxRenamed;
                    if ((double)Math.Abs(saleTaxRenamed.Tax_Added_Amount) > 0.005)
                    {
                        //                mvarTopLeft.Caption = mvarTopLeft.Caption & Sale_Tax.Tax_Name & vbCrLf
                        //                mvarTopRight.Caption = mvarTopRight.Caption & Format(Sale_Tax.Tax_Added_Amount, "###,##0.00") & vbCrLf
                        curTotalTaxes = curTotalTaxes + saleTaxRenamed.Tax_Added_Amount;
                    }
                }
                result.Add(_resourceManager.GetResString(offSet, 137), curTotalTaxes.ToString("###,##0.00"));
                //topLeftText = topLeftText + _resourceManager.GetResString(offSet,(short)137) + "\r\n";
                //topRightText = topRightText + curTotalTaxes.ToString("###,##0.00") + "\r\n";

                if (sale.Sale_Totals.Charge != 0)
                {
                    result.Add(_resourceManager.GetResString(offSet, 138), sale.Sale_Totals.Charge.ToString("###,##0.00"));

                    //topLeftText = topLeftText + _resourceManager.GetResString(offSet,(short)138) + "\r\n"; //"Charges "
                    //topRightText = topRightText + sale.Sale_Totals.Charge.ToString("###,##0.00") + "\r\n";
                }
                //topLeftText = topLeftText + "\r\n";
                //topRightText = topRightText + "________" + "\r\n";
                result.Add(_resourceManager.GetResString(offSet, 210), sale.Sale_Totals.Gross.ToString("###,##0.00"));

                //topLeftText = topLeftText + _resourceManager.GetResString(offSet,(short)210); // "Total"
                //topRightText = topRightText + sale.Sale_Totals.Gross.ToString("###,##0.00");
            }

            return result;
        }

        /// <summary>
        /// Process AITE 
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oTeCardHolder">Tax exempt card holder</param>
        /// <param name="userCode">User code</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <param name="error">Error</param>
        /// <returns>Aite card response</returns>
        private AiteCardResponse ProcessAite(Sale sale, teCardholder oTeCardHolder, string userCode, int shiftNumber, out ErrorMessage error)
        {
            error = new ErrorMessage();
            AiteCardResponse result = new AiteCardResponse();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            //mPrivateGlobals.theSystem = CacheManager.GetTeSystemForTill(sale.TillNumber, sale.Sale_Num);
            if (oTeCardHolder.IsValidCardHolder)
            {
                var oTeSale = new TaxExemptSale
                {
                    teCardholder = oTeCardHolder,
                    Sale_Num = sale.Sale_Num,
                    TillNumber = sale.TillNumber,
                    UserCode = Convert.ToString(userCode),
                    Shift = Convert.ToInt16(shiftNumber)
                };

                sale.TreatyNumber = oTeCardHolder.CardholderID;


                foreach (Sale_Tax tempLoopVarSt in sale.Sale_Totals.Sale_Taxes)
                {
                    var st = tempLoopVarSt;
                    if (st.Taxable_Amount != 0 | st.Tax_Included_Amount != 0)
                    {
                        oTeSale.TaxCredit.Add(st.Tax_Name, st.Tax_Code, st.Tax_Rate, st.Taxable_Amount, st.Tax_Added_Amount, st.Tax_Included_Amount, st.Tax_Included_Total, st.Tax_Rebate_Rate, st.Tax_Rebate, st.Tax_Name + st.Tax_Code); //   - gave mismatch type error for AITE
                    }
                }

                if (oTeSale.teCardholder.GstExempt)
                {
                    ProcessGstExempt(sale, oTeSale);
                    var tendrs = _tenderManager.GetAllTender(sale.Sale_Num, sale.TillNumber, "Sale", userCode, false, "", out error);
                    var selecteduser = _loginManager.GetUser(userCode);
                    result.AiteCardHolderName = oTeCardHolder.Name;
                    result.AiteCardNumber = oTeCardHolder.CardNumber;
                    result.BarCode = oTeCardHolder.Barcode;
                    result.SaleSummary = SaleSummary(sale);
                    result.Tenders = tendrs;
                    result.Tenders.EnableRunAway = EnableRunaway(selecteduser, sale);
                    return result;
                }


                TaxExemptSaleLine tesl;
                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    var sl = tempLoopVarSl;

                    var tcl = new TaxCreditLine { Line_Num = sl.Line_Num };


                    float taxIncldAmount = 0;
                    float taxCreditAmount = 0;
                    float includedTax = 0;
                    foreach (Line_Tax tempLoopVarLtx in sl.Line_Taxes)
                    {
                        var ltx = tempLoopVarLtx;
                        taxIncldAmount = taxIncldAmount + ltx.Tax_Added_Amount;



                        taxCreditAmount = taxCreditAmount + ltx.Tax_Added_Amount + ltx.Tax_Incl_Amount;




                        includedTax = includedTax + ltx.Tax_Incl_Amount;



                        var lt = new Line_Tax
                        {
                            Tax_Added_Amount = ltx.Tax_Added_Amount,
                            Tax_Code = ltx.Tax_Code,
                            Tax_Hidden = ltx.Tax_Hidden,
                            Tax_Hidden_Total = ltx.Tax_Hidden_Total,
                            Tax_Incl_Amount = ltx.Tax_Incl_Amount,
                            Tax_Incl_Total = ltx.Tax_Incl_Total,
                            Tax_Included = ltx.Tax_Included,
                            Tax_Name = ltx.Tax_Name,
                            Tax_Rate = ltx.Tax_Rate,
                            Taxable_Amount = ltx.Taxable_Amount
                        };
                        tcl.Line_Taxes.AddTaxLine(lt, "");

                    }


                    if (tcl.Line_Taxes.Count > 0)
                    {


                        oTeSale.TaxCreditLines.AddLine(tcl.Line_Num, tcl, "");

                    }


                    taxIncldAmount = taxIncldAmount + (float)sl.Amount;

                    tesl = new TaxExemptSaleLine
                    {
                        Quantity = sl.Quantity,
                        OriginalPrice = (float)sl.price,
                        Line_Num = sl.Line_Num,
                        StockCode = sl.Stock_Code,
                        TaxInclPrice = taxIncldAmount,
                        TaxCreditAmount = taxCreditAmount,
                        Description = sl.Description,
                        IsFuelItem = sl.ProductIsFuel && !sl.IsPropane,
                        IncludedTax = (decimal)includedTax
                    };

                    //Shiny Dec3, 2008
                    if (tesl.IsFuelItem)
                    {

                        tesl.GradeID = sl.GradeID;

                        tesl.TierID = Convert.ToInt16(Variables.gPumps.get_Pump(sl.pumpID).TierID);

                        tesl.LevelID = Convert.ToInt16(Variables.gPumps.get_Pump(sl.pumpID).LevelID);
                        if (_policyManager.USE_FUEL)
                        {
                            tesl.ProductKey = _teSystemManager.TeMakeFuelKey(sl.GradeID, tesl.TierID, tesl.LevelID);
                        }
                        else
                        {
                            tesl.ProductKey = sl.Stock_Code;
                        }
                    }
                    else
                    {
                        tesl.ProductKey = sl.Stock_Code;
                    }
                    if (!_exemptSaleLineManager.MakeTaxExemptLine(ref tesl, ref mPrivateGlobals.theSystem))
                    {
                        var strError = tesl.GetLastError();
                        if (strError == "2")
                        {
                            //"Cannot load Tax Exempt price, Please set Tax Exempt Category for Grade-" & SL.GradeID & " first in the BackOffice system! ")
                            //_resourceManager.CreateMessage(offSet,this, (short)17, temp_VbStyle, sl.GradeID, (byte)0);
                            error = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 11, 17, sl.GradeID, CriticalOkMessageType),
                                StatusCode = HttpStatusCode.Conflict
                            };
                        }
                        else if (!string.IsNullOrEmpty(strError) && strError.ToUpper() != "NO ERROR")
                        {
                            //"Error(" & strError & ") for getting Tax Exempt price, will use original price for this sale!")
                            //_resourceManager.CreateMessage(offSet,this, (short)18, temp_VbStyle2, strError, (byte)0);
                            error = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 11, 17, strError, CriticalOkMessageType),
                                StatusCode = HttpStatusCode.Conflict
                            };
                        }
                    }
                    else
                    {




                        if (sl.Amount < 0 && sl.IsTaxExemptItem)
                        {
                            tesl.TaxFreePrice = (float)sl.price;
                            tesl.Amount = float.Parse((tesl.TaxFreePrice * tesl.Quantity).ToString("#0.00"));
                            tesl.TaxInclPrice = -1 * sl.TaxInclPrice;
                            tesl.OriginalPrice = sl.OriginalPrice;
                            tesl.ExemptedTax = float.Parse((tesl.TaxInclPrice - tesl.Amount).ToString("#0.00"));
                            tesl.WasTaxExemptReturn = true;



                        }
                        else
                        {
                            tesl.WasTaxExemptReturn = false;
                        }

                        var checkOverLimit = true;
                        var checkQuota = true;
                        oTeSale.Add_a_Line(tesl, ref checkOverLimit, ref checkQuota);
                    }
                    sl.Total_Amount = sl.Amount;

                    sl.Amount = sl.Amount - Convert.ToDecimal(includedTax);

                    //  added this to take the cost  for Tax exempt vandor ( if there is a special setup for TE Vendor- otherwise use active vendor values
                    if (string.IsNullOrEmpty(sl.TEVendor)) // If TE vendor is not defined consider TE vendor  same as activevendor
                    {
                        sl.TEVendor = sl.Vendor;
                        sl.TECost = sl.Cost;
                    }
                    else
                    {
                        if (sl.Vendor == sl.TEVendor) // IF TAX EXEMPT VENDOR
                        {
                            sl.TEVendor = sl.Vendor;
                            sl.TECost = sl.Cost;
                        }
                        else
                        {
                            // Set the cost for the product based on the TE vendor,
                            var cost = _siteMessageService.GetStockCost(sl.Stock_Code, sl.TEVendor);
                            if (cost.HasValue)
                            {
                                sl.TECost = cost.Value;
                            }
                            else
                            {
                                sl.TECost = sl.Cost;
                            }
                        }
                        if (sl.TEVendor != sl.Vendor)
                        {
                            sl.OrigVendor = sl.Vendor;
                            sl.OrigCost = sl.Cost;
                            sl.Vendor = sl.TEVendor;
                            sl.Cost = sl.TECost;
                        }
                    }
                    // 
                }

                _saleManager.ApplyTaxes(ref sale, false);
                // sale.ApplyTaxes = false; 


                if (oTeSale.Te_Sale_Lines.Count > 0)
                {

                    if (oTeSale.HasTobacco)
                    {
                        if (DateAndTime.DateAdd(DateInterval.Year, Convert.ToDouble(_policyManager.AgeRestrict), oTeSale.teCardholder.Birthdate) > DateAndTime.Today)
                        {
                            //MsgBox ("The cardholder is not allowed to purchase tobacco!~Cannot complete the sale!")
                            //_resourceManager.CreateMessage(offSet,this, (short)51, MessageType.OkOnly, null, (byte)0);
                            error = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 11, 51, null, OkMessageType),
                                StatusCode = HttpStatusCode.Conflict
                            };
                            var tndrs = _tenderManager.GetAllTender(sale.Sale_Num, sale.TillNumber, "Sale", userCode, false, "", out error);

                            result = ClearTaxExempt(sale, oTeCardHolder, new tePurchaseList(), oTeSale, tndrs);
                            return result;
                        }
                    }

                    if (oTeSale.GasOverLimit || oTeSale.PropaneOverLimit || oTeSale.TobaccoOverLimit)
                    {
                        if (_policyManager.USE_OVERRIDE)
                        {
                            //Chaps_Main.FormLoadChild(frmTeOverLimit.Default);
                            result.IsFrmOverLimit = true;
                            result.AiteCardHolderName = oTeCardHolder.Name;
                            result.AiteCardNumber = oTeCardHolder.CardNumber;
                            result.BarCode = oTeCardHolder.Barcode;
                            CacheManager.AddTaxExemptSaleForTill(sale.TillNumber, sale.Sale_Num, oTeSale);
                            CacheManager.AddTeSystemForTill(sale.TillNumber, sale.Sale_Num, mPrivateGlobals.theSystem);
                            return result;
                        }
                    }

                    if (oTeSale.Te_Sale_Lines.Count > 0)
                    {
                        foreach (TaxExemptSaleLine tempLoopVarTesl in oTeSale.Te_Sale_Lines)
                        {
                            tesl = tempLoopVarTesl;
                            var rowNumber = tesl.Line_Num;
                            var saleLine = sale.Sale_Lines[rowNumber];


                            if (!(tesl.Amount < 0 && saleLine.IsTaxExemptItem))
                            {
                                saleLine.IsTaxExemptItem = true;
                                _saleLineManager.SetPrice(ref saleLine, tesl.TaxFreePrice);
                                _saleLineManager.SetAmount(ref saleLine, (decimal)tesl.Amount);
                                //saleLine.price = tesl.TaxFreePrice;
                                //saleLine.Amount = (decimal)tesl.Amount;
                            }
                        }
                        //_saleManager.ReCompute_Totals(ref sale);
                    }
                }

                CacheManager.AddTaxExemptSaleForTill(sale.TillNumber, sale.Sale_Num, oTeSale);
            }

            _saleManager.ReCompute_Totals(ref sale);
            CacheManager.AddTeSystemForTill(sale.TillNumber, sale.Sale_Num, mPrivateGlobals.theSystem);
            //TODO:UDHAM
            // Chaps_Main.FormLoadChild(SaleTend.Default);
            var tenders = _tenderManager.GetAllTender(sale.Sale_Num, sale.TillNumber, "Sale", userCode, false, "", out error);

            result.AiteCardHolderName = oTeCardHolder.Name;
            result.AiteCardNumber = oTeCardHolder.CardNumber;
            result.BarCode = oTeCardHolder.Barcode;
            var user = _loginManager.GetUser(userCode);
            result.SaleSummary = SaleSummary(sale);
            result.Tenders = tenders;
            result.Tenders.EnableRunAway = EnableRunaway(user, sale);
            CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
            return result;
        }

        /// <summary>
        /// Clear Tax Exempt
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="cardHolder">Tax exempt card holder</param>
        /// <param name="oPurchaseList">Purchase list</param>
        /// <param name="oTeSale">Tax exempt sale</param>
        /// <param name="tenders">Tenders</param>
        /// <returns>Aite card response</returns>
        private AiteCardResponse ClearTaxExempt(Sale sale, teCardholder cardHolder, tePurchaseList oPurchaseList, TaxExemptSale oTeSale, Tenders tenders)
        {
            AiteCardResponse result = new AiteCardResponse();

            sale.TreatyNumber = "";
            sale.TreatyName = ""; // 
            result.AiteCardHolderName = cardHolder.Name;
            result.AiteCardNumber = cardHolder.CardNumber;
            result.BarCode = cardHolder.Barcode;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            short rowNumber;
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
                            saleLine.IsTaxExemptItem = false;
                            //  -to reverse the cost vendor to original
                            saleLine.Vendor = saleLine.OrigVendor;
                            //shiny end

                            saleLine.price = oPurchaseList.Item(i).GetOriginalPrice();
                            saleLine.overrideCode = 0;
                            //  - need to put back amount and qty for prepay as original
                            if (saleLine.Prepay)
                            {
                                saleLine.Quantity = (float)saleLine.OrigQty;
                                saleLine.Amount = (decimal)(saleLine.OrigQty * saleLine.price);
                            }
                            //shiny end
                            saleLine.Cost = saleLine.OrigCost; // 

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
                                //  -to reverse the cost vendor to original
                                saleLine.Vendor = saleLine.OrigVendor;
                                //shiny end

                                saleLine.price = tesl.OriginalPrice;





                                if (tesl.IsFuelItem)
                                {
                                    saleLine.Amount = (decimal)tesl.TaxInclPrice;
                                }
                                saleLine.Total_Amount = 0;
                                saleLine.overrideCode = 0;
                                saleLine.Cost = saleLine.OrigCost; // 

                            }
                        }
                    }
                    else if (oTeSale.teCardholder.GstExempt)
                    {
                        foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                        {
                            var sl = tempLoopVarSl;
                            sl.Amount = sl.Total_Amount;
                            sl.Total_Amount = 0;
                        }
                    }
                }
            }
            _saleManager.ApplyTaxes(ref sale, true);
            //sale.ApplyTaxes = true;
            result.SaleSummary = SaleSummary(sale);
            CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
            return result;
        }

        /// <summary>
        /// Process QITE
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oTeCardHolder">Tax exempt card holder</param>
        /// <param name="userCode">User code</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <param name="error">Error</param>
        /// <returns>Qite response</returns>
        private QiteCardResponse ProcessQite(Sale sale, teCardholder oTeCardHolder, string userCode, int shiftNumber, out ErrorMessage error)
        {
            Tenders tenders;
            QiteCardResponse result = new QiteCardResponse();
            var user = _loginManager.GetUser(userCode);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            //frmTaxExemptCardholder.Default.LoadMode = 1; 
            //Chaps_Main.FormLoadChild(frmTaxExemptCardholder.Default);
            if (oTeCardHolder.IsValidCardHolder)
            {
                var oTeSale = new TaxExemptSale
                {
                    teCardholder = oTeCardHolder,
                    Sale_Num = sale.Sale_Num,
                    TillNumber = sale.TillNumber,
                    UserCode = Convert.ToString(userCode),
                    Shift = Convert.ToInt16(shiftNumber)
                };

                sale.TreatyNumber = oTeCardHolder.CardholderID;

                foreach (Sale_Tax tempLoopVarSt in sale.Sale_Totals.Sale_Taxes)
                {
                    var st = tempLoopVarSt;
                    if (st.Taxable_Amount != 0 | st.Tax_Included_Amount != 0)
                    {
                        oTeSale.TaxCredit.Add(st.Tax_Name, st.Tax_Code, st.Tax_Rate, st.Taxable_Amount, st.Tax_Added_Amount, st.Tax_Included_Amount, st.Tax_Included_Total, st.Tax_Rebate_Rate, st.Tax_Rebate, st.Tax_Name + st.Tax_Code); //   - gave mismatch type error for AITE
                    }
                }

                if (oTeSale.teCardholder.GstExempt)
                {
                    ProcessGstExempt(sale, oTeSale);
                    tenders = _tenderManager.GetAllTender(sale.Sale_Num, sale.TillNumber, "Sale", userCode, false, "", out error);
                    result.SaleSummary = SaleSummary(sale);
                    result.Tenders = tenders;
                    return result;
                }

                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    var sl = tempLoopVarSl;

                    var tcl = new TaxCreditLine { Line_Num = sl.Line_Num };

                    float taxIncldAmount = 0;
                    float taxCreditAmount = 0;
                    float includedTax = 0;
                    foreach (Line_Tax tempLoopVarLtx in sl.Line_Taxes)
                    {
                        var ltx = tempLoopVarLtx;
                        taxIncldAmount = taxIncldAmount + ltx.Tax_Added_Amount;

                        taxCreditAmount = taxCreditAmount + ltx.Tax_Added_Amount + ltx.Tax_Incl_Amount;

                        includedTax = includedTax + ltx.Tax_Incl_Amount;

                        var lt = new Line_Tax
                        {
                            Tax_Added_Amount = ltx.Tax_Added_Amount,
                            Tax_Code = ltx.Tax_Code,
                            Tax_Hidden = ltx.Tax_Hidden,
                            Tax_Hidden_Total = ltx.Tax_Hidden_Total,
                            Tax_Incl_Amount = ltx.Tax_Incl_Amount,
                            Tax_Incl_Total = ltx.Tax_Incl_Total,
                            Tax_Included = ltx.Tax_Included,
                            Tax_Name = ltx.Tax_Name,
                            Tax_Rate = ltx.Tax_Rate,
                            Taxable_Amount = ltx.Taxable_Amount
                        };
                        tcl.Line_Taxes.AddTaxLine(lt, "");
                    }

                    if (tcl.Line_Taxes.Count > 0)
                    {
                        oTeSale.TaxCreditLines.AddLine(tcl.Line_Num, tcl, "");
                    }

                    taxIncldAmount = taxIncldAmount + (float)sl.Amount;

                    var tesl = new TaxExemptSaleLine
                    {
                        Quantity = sl.Quantity,
                        OriginalPrice = (float)sl.price,
                        Line_Num = sl.Line_Num,
                        StockCode = sl.Stock_Code,
                        TaxInclPrice = taxIncldAmount,
                        TaxCreditAmount = taxCreditAmount,
                        Description = sl.Description,
                        IsFuelItem = sl.ProductIsFuel && !sl.IsPropane,
                        IncludedTax = (decimal)includedTax
                    };

                    if (tesl.IsFuelItem)
                    {

                        tesl.GradeID = sl.GradeID;

                        tesl.TierID = Convert.ToInt16(Variables.gPumps.get_Pump(sl.pumpID).TierID);

                        tesl.LevelID = Convert.ToInt16(Variables.gPumps.get_Pump(sl.pumpID).LevelID);
                        tesl.ProductKey = _policyManager.USE_FUEL ? _teSystemManager.TeMakeFuelKey(sl.GradeID, tesl.TierID, tesl.LevelID) : sl.Stock_Code;
                    }
                    else
                    {
                        tesl.ProductKey = sl.Stock_Code;
                    }
                    if (!_exemptSaleLineManager.MakeTaxExemptLine(ref tesl, ref mPrivateGlobals.theSystem))// tesl.MakeTaxExemptLine())
                    {
                        var strError = tesl.GetLastError();
                        if (strError == "2")
                        {
                            //"Cannot load Tax Exempt price, Please set Tax Exempt Category for Grade-" & SL.GradeID & " first in the BackOffice system! ")
                            //_resourceManager.CreateMessage(offSet,this, (short)17, temp_VbStyle, sl.GradeID, (byte)0);
                            error = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 11, 17, sl.GradeID, CriticalOkMessageType),
                                StatusCode = HttpStatusCode.Conflict
                            };
                            return null;
                        }
                        if (!string.IsNullOrEmpty(strError) && strError.ToUpper() != "NO ERROR")
                        {
                            //"Error(" & strError & ") for getting Tax Exempt price, will use original price for this sale!")
                            //_resourceManager.CreateMessage(offSet,this, (short)18, temp_VbStyle2, strError, (byte)0);
                            error = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 11, 18, strError, CriticalOkMessageType),
                                StatusCode = HttpStatusCode.Conflict
                            };
                            return null;
                        }
                    }
                    else
                    {
                        if (sl.Amount < 0 && sl.IsTaxExemptItem)
                        {
                            tesl.TaxFreePrice = (float)sl.price;
                            tesl.Amount = float.Parse((tesl.TaxFreePrice * tesl.Quantity).ToString("#0.00"));
                            tesl.TaxInclPrice = -1 * sl.TaxInclPrice;
                            tesl.OriginalPrice = sl.OriginalPrice;
                            tesl.ExemptedTax = float.Parse((tesl.TaxInclPrice - tesl.Amount).ToString("#0.00"));
                            tesl.WasTaxExemptReturn = true;
                        }
                        else
                        {
                            tesl.WasTaxExemptReturn = false;
                        }
                        bool checkOverLimit = true;
                        bool checkQuota = true;
                        oTeSale.Add_a_Line(tesl, ref checkOverLimit, ref checkQuota);
                    }
                    if (sl.Amount < 0)
                    {
                        sl.No_Loading = true; //  for refund of TE sale  - when re-setting the amount we don't need to go through the discount_rate ( it is already set)- going through it agian screwing up the tender amount( crevir issue with discount)
                    }
                    sl.Total_Amount = sl.Amount;
                    _saleLineManager.SetAmount(ref sl, sl.Amount - Convert.ToDecimal(includedTax));
                    //sl.Amount = sl.Amount - Convert.ToDecimal(includedTax);
                    sl.No_Loading = false; //   -It has to be set back to false
                }
                _saleManager.ApplyTaxes(ref sale, false);
                //sale.ApplyTaxes = false; 
                CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
                //   to apply the rebate after tax calculation based on treaty number entered in the screen
                string changedCustomerCode = oTeCardHolder.CardholderID;
                sale = _saleManager.SetCustomer(changedCustomerCode, sale.Sale_Num, sale.TillNumber, UserCode, 0, string.Empty, out error);
                CacheManager.AddTaxExemptSaleForTill(sale.TillNumber, sale.Sale_Num, oTeSale);
                //  
            }
            _saleManager.ReCompute_Totals(ref sale);
            tenders = _tenderManager.GetAllTender(sale.Sale_Num, sale.TillNumber, "Sale", userCode, false, "", out error);
            result.QiteBandMember = oTeCardHolder.CardNumber;

            result.SaleSummary = SaleSummary(sale);
            result.Tenders = tenders;
            result.Tenders.EnableRunAway = EnableRunaway(user, sale);
            CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
            return result;
        }

        /// <summary>
        /// Process GST Exempt
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oTeSale">Tax exemption sale</param>
        private void ProcessGstExempt(Sale sale, TaxExemptSale oTeSale)
        {
            foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
            {
                var sl = tempLoopVarSl;

                float inclTax = 0;
                var tcl = new TaxCreditLine { Line_Num = sl.Line_Num };

                foreach (Line_Tax tempLoopVarLtx in sl.Line_Taxes)
                {
                    var ltx = tempLoopVarLtx;

                    var lt = new Line_Tax
                    {
                        Tax_Added_Amount = ltx.Tax_Added_Amount,
                        Tax_Code = ltx.Tax_Code,
                        Tax_Hidden = ltx.Tax_Hidden,
                        Tax_Hidden_Total = ltx.Tax_Hidden_Total,
                        Tax_Incl_Amount = ltx.Tax_Incl_Amount,
                        Tax_Incl_Total = ltx.Tax_Incl_Total,
                        Tax_Included = ltx.Tax_Included,
                        Tax_Name = ltx.Tax_Name,
                        Tax_Rate = ltx.Tax_Rate,
                        Taxable_Amount = ltx.Taxable_Amount
                    };
                    tcl.Line_Taxes.AddTaxLine(lt, "");
                    inclTax = inclTax + ltx.Tax_Incl_Amount;
                }


                if (tcl.Line_Taxes.Count > 0)
                {


                    oTeSale.TaxCreditLines.AddLine(tcl.Line_Num, tcl, "");

                }



                sl.Total_Amount = sl.Amount;


                sl.Amount = sl.Amount - Convert.ToDecimal(inclTax);
            }
            _saleManager.ApplyTaxes(ref sale, false);
            _saleManager.ReCompute_Totals(ref sale);
            CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
            //TODO:UDHAM
            //Chaps_Main.FormLoadChild(SaleTend.Default);
        }

        /// <summary>
        /// Process SITE Return
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oTreatyNo">Treaty number</param>
        /// <param name="user">User</param>
        /// <param name="error">Error</param>
        /// <returns>Treaty number response</returns>
        private TreatyNumberResponse ProcessSiteReturn(Sale sale, teTreatyNo oTreatyNo, User user,
            out ErrorMessage error)
        {
            var storeRenamed = _policyManager.LoadStoreInfo();
            var offSet = storeRenamed.OffSet;
            error = new ErrorMessage();
            TreatyNumberResponse result = new TreatyNumberResponse
            {
                TreatyNumber = oTreatyNo.TreatyNumber,
                TreatyCustomerName = oTreatyNo.Name,
                PermitNumber = oTreatyNo.OverrideNumber
            };
            double originalPrice = 0;
            short ret;
            mPrivateGlobals.teProductEnum productType = default(mPrivateGlobals.teProductEnum);

            if (sale.Sale_Totals.Gross >= 0)
            {
                return null;
            }

            // Initialize the dll communication and create the object RTVPService
            // SetRetailer always returns 0 (success)
            if (_policyManager.SITE_RTVAL)
            {
                oTreatyNo.ValidTreatyNo = false;
                if (Variables.RTVPService == null)
                {
                    Variables.RTVPService = new RTVP.POSService.Transaction();
                }
               // ret = Convert.ToInt16(Variables.RTVPService.SetRetailer(Convert.ToInt32(storeRenamed.RetailerID), sale.TillNumber, sale.Sale_Num));

               // WriteToLogFile("Response is " + Convert.ToString(ret) + " from SetRetailer sent with parameters " + storeRenamed.RetailerID + "," + Convert.ToString(sale.TillNumber) + "," + Convert.ToString(sale.Sale_Num));
            }


            if (!string.IsNullOrEmpty(oTreatyNo.TreatyNumber))
            {

                sale.TreatyNumber = oTreatyNo.TreatyNumber;
                sale.TreatyName = oTreatyNo.Name;
                if (sale.Customer.TaxExempt == false)
                {
                    if (_policyManager.TE_ByRate && _policyManager.IDENTIFY_MEMBER)
                    {

                        if (Strings.Len(_policyManager.MEMBER_IDENTITY) > 0)
                        {
                            WriteToLogFile(" Going to chnage customer for Squamish");

                            var changedCustomerCode = Convert.ToString(Strings.Left(sale.TreatyNumber, Strings.Trim(Convert.ToString(_policyManager.MEMBER_IDENTITY)).Length) == Strings.Trim(Convert.ToString(_policyManager.MEMBER_IDENTITY)) ? _policyManager.BANDMEMBER : _policyManager.NONBANDMEMBER);
                            if (!string.IsNullOrEmpty(changedCustomerCode))
                            {
                                CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
                                sale.TECustomerChange = sale.Customer.Code != changedCustomerCode;
                                sale = _saleManager.SetCustomer(changedCustomerCode, sale.Sale_Num, sale.TillNumber, UserCode, 0, string.Empty, out error); // here we don't to call refresh_lines
                            }
                        }
                    }
                }

                var oPurchaseList = new tePurchaseList();
                oPurchaseList.Init(oTreatyNo, sale.Sale_Num, sale.TillNumber);
                foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
                {
                    var sl = tempLoopVarSl;

                    double taxIncldAmount = 0;
                    //  - to match with SITE_Sale
                    if (sl.Prepay) // moved this section from prepay screen to here
                    {
                    }
                    else if (sl.ProductIsFuel)
                    {
                        if (sl.Total_Amount == 0)
                        {
                            sl.Total_Amount = sl.Amount;
                        }
                    }

                    foreach (Line_Tax tempLoopVarLtx in sl.Line_Taxes)
                    {
                        var ltx = tempLoopVarLtx;
                        taxIncldAmount = taxIncldAmount + ltx.Tax_Added_Amount;
                    }
                    taxIncldAmount = taxIncldAmount + Convert.ToDouble(sl.Amount);

                    if (sl.Amount < 0 && sl.IsTaxExemptItem)
                    {
                        taxIncldAmount = -1 * sl.TaxInclPrice;
                        if (_policyManager.TE_ByRate == false) // Regualr Site customers SITE , TE_By rate = no
                        {
                            originalPrice = sl.Regular_Price; //editing the price for TE is keeping different price in purchaseitem and saleline'SL.price
                        }
                        else if (_policyManager.TE_ByRate) //SITE , TE_By rate = yes
                        {
                            originalPrice = sl.price;
                        }
                    }
                    else
                    {
                        if (_policyManager.TE_ByRate == false) // Regualr Site customers SITE , TE_By rate = no
                        {
                            originalPrice = sl.Regular_Price; //  - editing the price for TE is keeping different price in purchaseitem and saleline'SL.price

                        }
                        else if (_policyManager.TE_ByRate) //squamish SITE , TE_By rate = yes
                        {
                            originalPrice = sl.price;
                        }
                    }

                    string strError;
                    if (sl.ProductIsFuel && !sl.IsPropane)
                    {
                        if (_policyManager.USE_FUEL)
                        {
                            string tempSProductKey = _teSystemManager.TeMakeFuelKey(sl.GradeID, Convert.ToInt16(Variables.gPumps.get_Pump(sl.pumpID).TierID), Convert.ToInt16(Variables.gPumps.get_Pump(sl.pumpID).LevelID));
                            double tempDQuantity = sl.Quantity;
                            short tempIRowNumberInSalesMainForm = sl.Line_Num;
                            bool tempIsFuelItem = true;
                            string tempStoclCode = sl.Stock_Code;
                            if (!_purchaseListManager.AddItem(ref oPurchaseList, ref sale, ref oTreatyNo, ref tempSProductKey, ref tempDQuantity, ref originalPrice, ref tempIRowNumberInSalesMainForm, ref tempStoclCode, ref taxIncldAmount, ref tempIsFuelItem))
                            {
                                strError = oPurchaseList.GetLastError();
                                if (strError == "2")
                                {
                                    error = new ErrorMessage
                                    {
                                        MessageStyle = _resourceManager.CreateMessage(offSet, 11, 17, sl.GradeID, CriticalOkMessageType),
                                        StatusCode = HttpStatusCode.Conflict
                                    };
                                    return null;
                                }
                                if (!string.IsNullOrEmpty(strError) && strError.ToUpper() != "NO ERROR")
                                {
                                    error = new ErrorMessage
                                    {
                                        MessageStyle = _resourceManager.CreateMessage(offSet, 11, 18, strError, CriticalOkMessageType),
                                        StatusCode = HttpStatusCode.Conflict
                                    };
                                    return null;
                                }
                                break;
                            }
                        }
                        else
                        {
                            string tempSProductKey2 = sl.Stock_Code;
                            double tempDQuantity2 = sl.Quantity;
                            short tempIRowNumberInSalesMainForm2 = sl.Line_Num;
                            bool tempIsFuelItem2 = false;
                            string tempStockCode = sl.Stock_Code;
                            if (!_purchaseListManager.AddItem(ref oPurchaseList, ref sale, ref oTreatyNo, ref tempSProductKey2, ref tempDQuantity2, ref originalPrice, ref tempIRowNumberInSalesMainForm2, ref tempStockCode, ref taxIncldAmount, ref tempIsFuelItem2))
                            {
                                strError = oPurchaseList.GetLastError();
                                if (!string.IsNullOrEmpty(strError) && strError.ToUpper() != "NO ERROR")
                                {
                                    error = new ErrorMessage
                                    {
                                        MessageStyle = _resourceManager.CreateMessage(offSet, 11, 18, strError, CriticalOkMessageType),
                                        StatusCode = HttpStatusCode.Conflict
                                    };
                                }
                            }
                        }
                    }
                    else
                    {
                        string tempSProductKey3 = sl.Stock_Code;
                        double tempDQuantity3 = sl.Quantity;
                        short tempIRowNumberInSalesMainForm3 = sl.Line_Num;
                        bool tempIsFuelItem3 = false;
                        string tempStockCode = sl.Stock_Code;
                        if (!_purchaseListManager.AddItem(ref oPurchaseList, ref sale, ref oTreatyNo, ref tempSProductKey3, ref tempDQuantity3, ref originalPrice, ref tempIRowNumberInSalesMainForm3, ref tempStockCode, ref taxIncldAmount, ref tempIsFuelItem3))
                        {
                            strError = oPurchaseList.GetLastError();
                            if (!string.IsNullOrEmpty(strError) && strError.ToUpper() != "NO ERROR")
                            {
                                //_resourceManager.CreateMessage(offSet,this, (short)18, temp_VbStyle4, strError, (byte)0);
                                error = new ErrorMessage
                                {
                                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 18, strError, CriticalOkMessageType),
                                    StatusCode = HttpStatusCode.Conflict
                                };
                            }
                        }
                    }
                    //   to check for errors returned by SetItemUPC, SetItemTotal or SetItemEquivalence

                    if (_policyManager.SITE_RTVAL)
                    {
                        if (oPurchaseList.RTVPError)
                        {
                            string msg = "RTVP error " + Convert.ToString(oPurchaseList.RTVPResponse) + " from " + oPurchaseList.RTVPCommand;
                            error = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 11, 18, msg, CriticalOkMessageType),
                                StatusCode = HttpStatusCode.Conflict
                            };
                        }
                    }
                }

                //   real time validation SITE
                short i;
                if (_policyManager.SITE_RTVAL)
                {
                    if (oPurchaseList.Count() > 0)
                    {
                        ret = Convert.ToInt16(Variables.RTVPService.LimitRequest());
                        var timeOut = mPrivateGlobals.theSystem.RTVP_TimeOut + 100;
                        var timeIn = (int)DateAndTime.Timer;
                        while (!(DateAndTime.Timer - timeIn > timeOut))
                        {
                            System.Windows.Forms.Application.DoEvents();
                            if (ret != 999)
                            {
                                break;
                            }
                            if (DateAndTime.Timer < timeIn)
                            {
                                timeIn = (int)DateAndTime.Timer;
                            }
                        }

                        WriteToLogFile("Response is " + Convert.ToString(ret) + " from LimitRequest sent with no parameters");
                        if (ret == 0 | ret == 4 | ret == 12)
                        {
                            // valid treaty number, no message to display, complete the transaction
                            oTreatyNo.ValidTreatyNo = true;
                        }
                        else
                        {

                            var strMessage = _siteMessageService.GetSiteMessage(ret);
                            if (!string.IsNullOrEmpty(strMessage))
                            {
                                //Chaps_Main.DisplayMsgForm(strMessage, 99, null, 0, 0, "", "", "", "");
                                error = new ErrorMessage
                                {
                                    MessageStyle = new MessageStyle
                                    {
                                        Message = strMessage,
                                        MessageType = CriticalOkMessageType
                                    },
                                    StatusCode = HttpStatusCode.Conflict
                                };
                            }
                            else
                            {
                                //_resourceManager.CreateMessage(offSet,this, (short)18, temp_VbStyle6, null, (byte)0);
                                error = new ErrorMessage
                                {
                                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 18, null, CriticalOkMessageType),
                                    StatusCode = HttpStatusCode.Conflict
                                };
                                return null;
                            }

                            if ((ret >= 5 && ret <= 18) || (ret >= 22 && ret <= 25))
                            {
                                oTreatyNo.ValidTreatyNo = false;
                                //TODO:UDHAM
                                //Chaps_Main.FormLoadChild(SaleTend.Default);
                                return null;
                            } // over the limit
                            if ((ret >= 1 && ret <= 3) || (ret >= 19 && ret <= 21))
                            {
                                oTreatyNo.ValidTreatyNo = true;
                                // dll
                                // matches items in the sale (sometimes dll returns 1 (tobacco overlimit) for a sale with fuel items only
                                var boolHasTobaccoItems = false;
                                var boolHasFuelProducts = false;
                                for (i = 1; i <= oPurchaseList.Count(); i++)
                                {
                                    oPurchaseList.Item(i).GetProductType(ref productType);
                                    if (productType == mPrivateGlobals.teProductEnum.eCigar | productType == mPrivateGlobals.teProductEnum.eCigarette | productType == mPrivateGlobals.teProductEnum.eLooseTobacco)
                                    {
                                        boolHasTobaccoItems = true;
                                    }
                                    else if (productType == mPrivateGlobals.teProductEnum.eGasoline | productType == mPrivateGlobals.teProductEnum.eDiesel | productType == mPrivateGlobals.teProductEnum.ePropane | productType == mPrivateGlobals.teProductEnum.emarkedGas | productType == mPrivateGlobals.teProductEnum.emarkedDiesel) // hen
                                    {
                                        boolHasFuelProducts = true;
                                    }
                                }
                                //   end
                                if (ret == 1) // 1 - Success. Tobacco overlimit
                                {
                                    if (boolHasTobaccoItems)
                                    {
                                        oPurchaseList.IsTobaccoOverLimit = true;
                                    }
                                }
                                else if (ret == 2) // 2 - Success. Fuel overlimit
                                {
                                    if (boolHasFuelProducts)
                                    {
                                        oPurchaseList.IsFuelOverLimit = true;
                                    }
                                }
                                else if (ret == 3) // 3 - Success. Tobacco & Fuel Overlimit
                                {
                                    if (boolHasFuelProducts)
                                    {
                                        oPurchaseList.IsFuelOverLimit = true;
                                    }
                                    if (boolHasTobaccoItems)
                                    {
                                        oPurchaseList.IsTobaccoOverLimit = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        oTreatyNo.ValidTreatyNo = false;
                    }
                }
                //   end
                _saleManager.ApplyTaxes(ref sale, false);
                //sale.ApplyTaxes = false; 

  
                if (oPurchaseList.Count() > 0)
                {



                    for (i = 1; i <= oPurchaseList.Count(); i++)
                    {
                        var rowNumber = oPurchaseList.Item(i).GetRowInSalesMain();
                        var saleLine = sale.Sale_Lines[rowNumber];




                        if (saleLine.Amount < 0 && saleLine.IsTaxExemptItem)
                        {
                            oPurchaseList.Item(i).SetTaxFreePrice((float)saleLine.price);
                            oPurchaseList.Item(i).WasTaxExemptReturn = true;
                        }
                        else
                        {
                            oPurchaseList.Item(i).WasTaxExemptReturn = false;


                            saleLine.IsTaxExemptItem = true;
                            //saleLine.price = oPurchaseList.Item(i).GetTaxFreePrice();
                            _saleLineManager.SetPrice(ref saleLine, oPurchaseList.Item(i).GetTaxFreePrice());
                        }
                    }
                    //sale.ReCompute_Totals();
                }
            }
            _saleManager.ReCompute_Totals(ref sale);

            //TODO:UDHAM
            //Chaps_Main.FormLoadChild(SaleTend.Default);
            var tenders = _tenderManager.GetAllTender(sale.Sale_Num, sale.TillNumber, "Sale", user.Code, false, "", out error);

            var retx = Convert.ToInt16(Variables.RTVPService.PurchaseTransaction());
            WriteToLogFile("Response is " + Convert.ToString(retx) + " from PurchaseTransaction sent with no parameters");
            result.SaleSummary = SaleSummary(sale);

            result.Tenders = tenders;
            result.Tenders.EnableRunAway = EnableRunaway(user, sale);
            CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
            if (sale.LoadingTemp)
            {
                _saleService.SaveSale(sale.TillNumber, sale.Sale_Num, sale);
            }
            return result;
        }

        ///// <summary>
        ///// Customer Change
        ///// </summary>
        ///// <param name="sale">Sale</param>
        ///// <param name="changedCustomerCode">Changed customer code</param>
        ///// <param name="loadCustomer">Load customer</param>
        ///// <param name="refund">Refund</param>
        ///// <param name="error">Error</param>
        //private void CustomerChange(Sale sale, string changedCustomerCode, bool loadCustomer, bool refund, out ErrorMessage error)
        //{
        //    double getPrice;
        //    short loyalPricecode = 0;
        //    error = new ErrorMessage();
        //    if (sale != null && !refund && !sale.EligibleTaxEx && !sale.Apply_CustomerChange) //   added And Not SA.EligibleTaxEx to reevaluate the sale if tax exemption button was clicked in Customer form
        //    {
        //        if (sale.Customer.Code == changedCustomerCode)
        //        {
        //            return; //   if is the same code don't reevaluate the sale
        //        }
        //    }

        //    //   Don't check if CL is "" or "*", if it is a cash sale, load the cash customer for the sale
        //    // This works also if the sale has been reset to "Cash Sale" using Customer form "Cash Sale" button
        //    // Handles the case when the customer is set by policy to a default customer
        //    //    If CL <> "" And CL <> "*" Then
        //   WriteToLogFile("Changing customer to " + changedCustomerCode);
        //    if (sale != null)
        //    {
        //        sale.Customer.Code = changedCustomerCode;
        //        if (loadCustomer)
        //        {
        //            sale.Customer = _customerManager.LoadCustomer(changedCustomerCode);
        //        }

        //        
        //        if (!string.IsNullOrEmpty(Chaps_Main.LoyCard))
        //        {
        //            sale.Customer.LoyaltyCard = Chaps_Main.LoyCard;
        //            sale.Customer.LoyaltyExpDate = Chaps_Main.LoyExpDate;
        //            sale.Customer.LoyaltyCardSwiped = Chaps_Main.LoyCrdSwiped;

        //            //Udham Commented as KickBack removed from Scope
        //            ////
        //            //if (_policyManager.Use_KickBack)
        //            //{
        //            //    var rsTemp = Chaps_Main.Get_Records("SELECT * FROM Kickback " + " WHERE CustomerCardNum=\'" + Chaps_Main.LoyCard + "\'", Chaps_Main.dbMaster, (Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (Int32)ADODB.LockTypeEnum.adLockReadOnly);
        //            //    if (!rsTemp.EOF)
        //            //    {
        //            //        sale.Customer.PointCardNum = Convert.ToString(rsTemp.Fields["PointCardNum"].Value);
        //            //        sale.Customer.PointCardPhone = Convert.ToString(rsTemp.Fields["phonenum"].Value);
        //            //        //                If SA.Customer.PointCardSwipe = "" Then SA.Customer.PointCardSwipe = "0" '0-from database, 1-from phone number, 2-swiped
        //            //        sale.Customer.PointCardSwipe = "0"; // 0-from database based on GK card swiped, 1-from phone number, 2-swiped
        //            //    }
        //            //    else
        //            //    {
        //            //        sale.Customer.PointCardNum = "";
        //            //        sale.Customer.PointCardPhone = "";
        //            //        sale.Customer.PointCardSwipe = "";
        //            //    }
        //            //}
        //        }
        //        else
        //        {
        //            sale.Customer.LoyaltyCard = "";
        //            sale.Customer.LoyaltyExpDate = "";
        //            sale.Customer.LoyaltyCardSwiped = false;
        //            sale.Customer.PointCardNum = "";
        //            sale.Customer.PointCardPhone = "";
        //            sale.Customer.PointCardSwipe = "";
        //        }
        //        

        //        //    lblCustName = SA.Customer.Name '  - screen flip for SITE
        //        sale.Sale_Client = sale.Customer.Name;

        //        if (_policyManager.USE_LOYALTY && Strings.UCase(Convert.ToString(_policyManager.LOYAL_TYPE)) == "PRICES" && !string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && sale.Customer.CL_Status == "A")
        //        {

        //            loyalPricecode = Convert.ToInt16(_policyManager.LOYAL_PRICE);
        //            getPrice = loyalPricecode; //  if selecting the customer after entering the product not giving loyalty
        //        }
        //        else
        //        {
        //            if (sale.Customer.Price_Code >= 1 & sale.Customer.Price_Code <= _policyManager.NUM_PRICE)
        //            {
        //                getPrice = sale.Customer.Price_Code;
        //            }
        //            else
        //            {
        //                getPrice = 1;
        //            }
        //        }


        //        var cd = Convert.ToBoolean(_policyManager.CUST_DISC);

        //        var pd = Convert.ToBoolean(_policyManager.PROD_DISC);

        //        foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
        //        {
        //            var sl = tempLoopVarSl;
        //            
        //            
        //            //  - For fule we shouldn't look for different price_number
        //            //If Not sl.Gift_Certificate then
        //            if (sl.Gift_Certificate == false && sl.ProductIsFuel == false)
        //            {
        //                if (_policyManager.USE_LOYALTY && Strings.UCase(Convert.ToString(_policyManager.LOYAL_TYPE)) == "PRICES" && !string.IsNullOrEmpty(sale.Customer.Loyalty_Code) && !string.IsNullOrEmpty(sale.Customer.CL_Status) && sale.Customer.CL_Status == "A")
        //                {
        //                    if (Math.Abs(getPrice - sl.Price_Number) > 0)
        //                    {
        //                        if (!sl.LOY_EXCLUDE)
        //                        {
        //                            _saleManager.Line_Price_Number(ref sale, ref sl, loyalPricecode);
        //                            // sale.Line_Price_Number(ref SL, Loyal_pricecode);
        //                        }
        //                        else
        //                        {
        //                            _saleManager.Line_Price_Number(ref sale, ref sl, (short)getPrice);
        //                            // sale.Line_Price_Number(ref SL, (short)Get_Price);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (Math.Abs(getPrice - sl.Price_Number) > 0)
        //                    {
        //                        _saleManager.Line_Price_Number(ref sale, ref sl, (short)getPrice);
        //                        //sale.Line_Price_Number(ref SL, (short)Get_Price);
        //                    }
        //                }
        //            }
        //            //  For using Loyalty discount



        //            if (_policyManager.USE_LOYALTY && Strings.UCase(Convert.ToString(_policyManager.LOYAL_TYPE)) == "DISCOUNTS" && sale.Customer.Loyalty_Code.Length > 0 && sale.Customer.CL_Status == "A")
        //            {
        //                if (!sl.LOY_EXCLUDE)
        //                {

        //                    var loydiscode = Convert.ToInt16(_policyManager.LOYAL_DISC);
        //                    if (cd || pd)
        //                    {
        //                        if (cd && pd)
        //                        {
        //                            _saleLineManager.Apply_Table_Discount(ref sl, sl.Prod_Discount_Code, loydiscode, out error);
        //                        }
        //                        else if (cd)
        //                        {
        //                            _saleLineManager.Apply_Table_Discount(ref sl, 0, loydiscode, out error);
        //                        }
        //                        else if (pd)
        //                        {
        //                            _saleLineManager.Apply_Table_Discount(ref sl, sl.Prod_Discount_Code, 0, out error);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (cd && pd)
        //                        {
        //                            _saleLineManager.Apply_Table_Discount(ref sl, sl.Prod_Discount_Code, sale.Customer.Discount_Code, out error);
        //                        }
        //                        else if (cd)
        //                        {
        //                            _saleLineManager.Apply_Table_Discount(ref sl, 0, sale.Customer.Discount_Code, out error);
        //                        }
        //                        else if (pd)
        //                        {
        //                            _saleLineManager.Apply_Table_Discount(ref sl, sl.Prod_Discount_Code, 0, out error);
        //                        }
        //                    }
        //                    _saleManager.Line_Discount_Type(ref sl, sl.Discount_Type);
        //                    _saleManager.Line_Discount_Rate(ref sale, ref sl, sl.Discount_Rate);
        //                    //sale.Line_Discount_Rate(SL, SL.Discount_Rate);
        //                }
        //            }
        //            else
        //            {
        //                //Shiny end

        //                if (cd || pd)
        //                {
        //                    if (cd && pd) // Use both customer & product discounts
        //                    {
        //                        _saleLineManager.Apply_Table_Discount(ref sl, sl.Prod_Discount_Code, sale.Customer.Discount_Code, out error);
        //                    }
        //                    else if (cd) // Use customer but not product
        //                    {
        //                        _saleLineManager.Apply_Table_Discount(ref sl, 0, sale.Customer.Discount_Code, out error);
        //                    }
        //                    else if (pd) // Use product but not customer
        //                    {
        //                        _saleLineManager.Apply_Table_Discount(ref sl, sl.Prod_Discount_Code, 0, out error);
        //                    }
        //                    _saleManager.Line_Discount_Type(ref sl, sl.Discount_Type);
        //                    // sale.Line_Discount_Type(SL, SL.Discount_Type);
        //                    _saleManager.Line_Discount_Rate(ref sale, ref sl, sl.Discount_Rate);

        //                    //sale.Line_Discount_Rate(SL, SL.Discount_Rate);
        //                }
        //               WriteToLogFile(" Applied customer discounts from Customer change");
        //                
        //                if (sl.FuelRebateEligible && sl.FuelRebate > 0 && sale.Customer.UseFuelRebate && sale.Customer.UseFuelRebateDiscount) 
        //                {
        //                    _saleLineManager.ApplyFuelRebate(ref sl);
        //                }
        //                else
        //                {
        //                    
        //                    

        //                    if (sl.ProductIsFuel && _policyManager.FuelLoyalty)
        //                    {
        //                        if (sale.Customer.GroupID != "")
        //                        {
        //                            if (sale.Customer.DiscountType != "")
        //                            {
        //                                
        //                                if (!_policyManager.GetPol("CL_DISCOUNTS", sl))
        //                                {
        //                                    //_resourceManager.CreateMessage(offSet,this, 81, temp_VbStyle, null, 0);
        //                                    error = new ErrorMessage
        //                                    {
        //                                        MessageStyle = _resourceManager.CreateMessage(offSet,11, 81, null, CriticalOkMessageType),
        //                                        StatusCode = HttpStatusCode.Conflict
        //                                    };
        //                                }
        //                                else
        //                                {
        //                                    
        //                                    //  Discountchart loyalty
        //                                    //same as $discount by litre- only difference is discount rate should be based on grade
        //                                    if (sale.Customer.DiscountType == "D")
        //                                    {
        //                                        //sl.ApplyFuelLoyalty(sale.Customer.DiscountType, sl.GetFuelDiscountChartRate(sale.Customer.GroupID, sl.GradeID), sale.Customer.DiscountName); // this will bring the discount rate based on customer group id and fuel grade
        //                                        _saleLineManager.ApplyFuelLoyalty(ref sl, sale.Customer.DiscountType, _saleLineManager.GetFuelDiscountChartRate(ref sl, sale.Customer.GroupID, sl.GradeID), sale.Customer.DiscountName);
        //                                    }
        //                                    else
        //                                    {
        //                                        // 
        //                                        _saleLineManager.ApplyFuelLoyalty(ref sl, sale.Customer.DiscountType, sale.Customer.DiscountRate, sale.Customer.DiscountName);
        //                                       WriteToLogFile("Apply FuelLoyalty from Customer change");
        //                                    }
        //                                } 
        //                            }
        //                        }
        //                    }
        //                    
        //                } 
        //            } //Shiny
        //        }
        //        _saleManager.ReCompute_Coupon(ref sale);
        //        //sale.ReCompute_Coupon(); //05/17/06 Nancy added for Fuel Loyalty of Coupon type
        //        _saleManager.ReCompute_Totals(ref sale);

        //        //sale.ReCompute_Totals();
        //       WriteToLogFile(" Finished Recompute from Customer change");
        //        _saleManager.SaveTemp(ref sale, sale.TillNumber);
        //    }
        //    // sale.SaveTemp(); //  If taking customer after adding all items and if there is no discount system was not saving the customer info.
        //   WriteToLogFile("SaveTemp from Customer change");
        //    if (refund)
        //    {
        //        //Refresh_Lines(); //  Tookout refreshlines from customer_change( because it is causing screen flip & freezing in SITE screen)
        //    }
        //}

        /// <summary>
        /// Process SITE Sale
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oTreatyNo">Treaty number</param>
        /// <param name="user">User</param>
        /// <param name="error">Error</param>
        /// <returns>Treaty number response</returns>
        private TreatyNumberResponse ProcessSiteSale(Sale sale, teTreatyNo oTreatyNo, User user, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var result = new TreatyNumberResponse
            {
                TreatyNumber = oTreatyNo.TreatyNumber,
                TreatyCustomerName = oTreatyNo.Name,
                PermitNumber = oTreatyNo.OverrideNumber
            };

            Sale_Line sl;
           // float[] InitialSaleLinesTaxes;
            short i;
            short nOverRide = 0;
            double taxIncldAmount=0;
            double originalPrice = 0;
            var productType = default(mPrivateGlobals.teProductEnum);
            var offSet = _policyManager.LoadStoreInfo().OffSet;




            //  moved this to here to keep the original tax information (squamish tax is getting recalculated when doing customer price change)
            foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
            {
                sl = tempLoopVarSl;
                taxIncldAmount = 0;
                foreach (Line_Tax tempLoopVarLtx in sl.Line_Taxes)
                {
                    var ltx = tempLoopVarLtx;
                    //   added the next If condition for tax collected from tax exempt customers
                    if (!string.IsNullOrEmpty(sl.TE_COLLECTTAX))
                    {
                        taxIncldAmount = 0;
                    }
                    else
                    {
                        //   end
                        taxIncldAmount = taxIncldAmount + ltx.Tax_Added_Amount;
                    }
                }
                taxIncldAmount = taxIncldAmount + sl.Quantity * sl.Regular_Price;
                WriteToLogFile("taxincluded amount initially : " + taxIncldAmount.ToString());
                sl.OrigTaxIncldAmount = (decimal)taxIncldAmount;
            }

            sale.TreatyNumber = oTreatyNo.TreatyNumber;
            sale.TreatyName = oTreatyNo.Name; // 
            CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
            if (sale.Customer.TaxExempt == false) //moved this line after setting on sa.treatyno- aug4,2010- Jd customer not saving treatynumber in salehead'  If it is a tax exempt linked customer - don't do the switch over
            {

                //  - Squmaish - automatically identify band customer and apply discounts or price accordingly


                if (_policyManager.TE_ByRate && _policyManager.IDENTIFY_MEMBER)
                {

                    if (!string.IsNullOrEmpty(_policyManager.MEMBER_IDENTITY))
                    {
                        WriteToLogFile(" Going to change customer for Squamish");

                        var changedCustomerCode = Convert.ToString(Strings.Left(sale.TreatyNumber, Strings.Trim(Convert.ToString(_policyManager.MEMBER_IDENTITY)).Length) == Strings.Trim(Convert.ToString(_policyManager.MEMBER_IDENTITY)) ? _policyManager.BANDMEMBER : _policyManager.NONBANDMEMBER);
                        if (!string.IsNullOrEmpty(changedCustomerCode))
                        {
                            sale.TECustomerChange = sale.Customer.Code != changedCustomerCode;
                            sale = _saleManager.SetCustomer(changedCustomerCode, sale.Sale_Num, sale.TillNumber, UserCode, 0, string.Empty, out error);  // here we don't to call refresh_lines

                        }
                    }
                }
            }
            // 

            var oPurchaseList = new tePurchaseList();
            oPurchaseList.Init(oTreatyNo, sale.Sale_Num, sale.TillNumber);

            var z = oPurchaseList.GetTreatyNo();

            foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
            {
                sl = tempLoopVarSl;

                //  - as part of eliminating prepay question. If Tax exempt customer we are changing the quantity - but keeping amount same
                if (sl.Prepay) // moved this section from prepay screen to here
                {
                    if (Math.Abs(sl.TEPrice) > 0)
                    {
                        sl.OrigQty = sl.Quantity;
                        sl.Quantity = float.Parse(((sl.Amount + Convert.ToDecimal(sl.Line_Discount)) / Convert.ToDecimal(sl.TEPrice)).ToString("#0.000")); // increasing the quantity based on TE price- same amount to keep the
                        sl.No_Loading = true; // not to calculate extra discounts  on increased volume  in taxexempt prepay ( since we increase amount and give more volume of gas using the loyalty discount & Tax exempt

                        sl.Amount = Convert.ToDecimal(((sl.Amount + Convert.ToDecimal(sl.Line_Discount)) / Convert.ToDecimal(sl.TEPrice.ToString("#0.000")) * Convert.ToDecimal(sl.Regular_Price)).ToString("#0.00"));
                        Variables.Pump[sl.pumpID].PrepayAmount = (float)sl.Amount;
                        sl.Total_Amount = sl.Amount; //  - to keep the real amount showing in pump

                        //I am guessing the following  5 lines of code is for pump display - need to show the amount for extra volume too
                        var prepayAmount = (double)sl.Amount;
                        prepayAmount = double.Parse(prepayAmount.ToString("####.00"));
                        prepayAmount = double.Parse(Strings.Replace(prepayAmount.ToString(CultureInfo.InvariantCulture), ".", "", 1, 1, CompareMethod.Text));
                        prepayAmount = double.Parse(Strings.Right("000000" + Convert.ToString(prepayAmount, CultureInfo.InvariantCulture), 6));

                        Variables.PrepAmount = prepayAmount;
                    }
                    //  - for changing the receipt and amount in the receipt for fuel sales also ( similar to prepay)
                }
                else if (sl.ProductIsFuel)
                {
                    if (sl.Total_Amount == 0)
                    {
                        sl.Total_Amount = sl.Amount;
                    }
                }
                taxIncldAmount = (double)sl.OrigTaxIncldAmount;



                if (sl.Amount < 0 && sl.IsTaxExemptItem)
                {
                    taxIncldAmount = -1 * sl.TaxInclPrice;
                    //shiny - Dec9, 2009 - Squamish nation
                    //                OriginalPrice = sl.Regular_Price

                    if (_policyManager.TE_ByRate == false) // Regualr Site customers SITE , TE_By rate = no
                    {
                        originalPrice = sl.Regular_Price; //  - editing the price for TE is keeping different price in purchaseitem and saleline'SL.price

                    }
                    else if (_policyManager.TE_ByRate) //squamish SITE , TE_By rate = yes
                    {
                        originalPrice = sl.price;
                    }
                }
                else
                {
                    //shiny - Dec9, 2009 - Squamish nation
                    //                OriginalPrice = sl.Regular_Price

                    if (_policyManager.TE_ByRate == false) // Regualr Site customers SITE , TE_By rate = no
                    {
                        originalPrice = sl.Regular_Price; //  - editing the price for TE is keeping different price in purchaseitem and saleline'SL.price

                    }
                    else if (_policyManager.TE_ByRate) //squamish SITE , TE_By rate = yes
                    {
                        originalPrice = sl.price;
                    }
                }


                string strError;
                if (sl.ProductIsFuel && !sl.IsPropane)
                {
                    if (_policyManager.USE_FUEL)
                    {

                        if (Variables.gPumps == null)
                        {
                            Variables.gPumps = _fuelPumpService.InitializeGetProperty(PosId, _policyManager.TAX_EXEMPT, _policyManager.TE_ByRate, _policyManager.TE_Type, _policyManager.AuthPumpPOS);
                        }
                        string tempSProductKey = _teSystemManager.TeMakeFuelKey(sl.GradeID, Convert.ToInt16(Variables.gPumps.get_Pump(sl.pumpID).TierID), Convert.ToInt16(Variables.gPumps.get_Pump(sl.pumpID).LevelID));
                        double tempDQuantity = sl.Quantity;
                        var tempIRowNumberInSalesMainForm = sl.Line_Num;
                        bool tempIsFuelItem = true;
                        string tempStockCode = sl.Stock_Code;
                        if (!_purchaseListManager.AddItem(ref oPurchaseList, ref sale, ref oTreatyNo, ref tempSProductKey, ref tempDQuantity, ref originalPrice, ref tempIRowNumberInSalesMainForm, ref tempStockCode, ref taxIncldAmount, ref tempIsFuelItem))
                        {
                            strError = oPurchaseList.GetLastError();
                            if (strError == "2")
                            {
                                //MsgBox ("Cannot load Tax Exempt price, Please set Tax Exempt Category for Grade-" & SL.GradeID & " first in the BackOffice system! ")
                                //_resourceManager.CreateMessage(offSet,this, 17, temp_VbStyle, sl.GradeID, 0);
                                error = new ErrorMessage
                                {
                                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 17, sl.GradeID, CriticalOkMessageType),
                                    StatusCode = HttpStatusCode.Conflict
                                };
                                return null;
                            }
                            if (!string.IsNullOrEmpty(strError) && strError.ToUpper() != "NO ERROR")
                            {
                                //MsgBox ("Error(" & strError & ") for getting Tax Exempt price, will use original price for this sale!")
                                //_resourceManager.CreateMessage(offSet,this, 18, temp_VbStyle2, strError, 0);
                                error = new ErrorMessage
                                {
                                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 18, strError, CriticalOkMessageType),
                                    StatusCode = HttpStatusCode.Conflict
                                };
                                return null;
                            }
                            break;
                        }
                    }
                    else
                    {
                        var tempSProductKey2 = sl.Stock_Code;
                        double tempDQuantity2 = sl.Quantity;
                        var tempIRowNumberInSalesMainForm2 = sl.Line_Num;
                        bool tempIsFuelItem2 = false;
                        var tempStockCode = sl.Stock_Code;
                        if (!_purchaseListManager.AddItem(ref oPurchaseList, ref sale, ref oTreatyNo, ref tempSProductKey2, ref tempDQuantity2, ref originalPrice, ref tempIRowNumberInSalesMainForm2, ref tempStockCode, ref taxIncldAmount, ref tempIsFuelItem2))
                        {
                            strError = oPurchaseList.GetLastError();
                            if (!string.IsNullOrEmpty(strError) && strError.ToUpper() != "NO ERROR")
                            {
                                //_resourceManager.CreateMessage(offSet,this, 18, temp_VbStyle3, strError, 0);
                                error = new ErrorMessage
                                {
                                    MessageStyle = _resourceManager.CreateMessage(offSet, 11, 18, strError, CriticalOkMessageType),
                                    StatusCode = HttpStatusCode.Conflict
                                };
                                return null;
                            }
                        }
                    }
                }
                else
                {
                    var tempSProductKey3 = sl.Stock_Code;
                    double tempDQuantity3 = sl.Quantity;
                    short tempIRowNumberInSalesMainForm3 = sl.Line_Num;
                    bool tempIsFuelItem3 = false;
                    var tempStockCode = sl.Stock_Code;
                    if (!_purchaseListManager.AddItem(ref oPurchaseList, ref sale, ref oTreatyNo, ref tempSProductKey3, ref tempDQuantity3, ref originalPrice, ref tempIRowNumberInSalesMainForm3, ref tempStockCode, ref taxIncldAmount, ref tempIsFuelItem3))
                    {
                        strError = oPurchaseList.GetLastError();
                        if (!string.IsNullOrEmpty(strError) && strError.ToUpper() != "NO ERROR")
                        {
                            //_resourceManager.CreateMessage(offSet,this, 18, temp_VbStyle4, strError, 0);
                            error = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 11, 18, strError, CriticalOkMessageType),
                                StatusCode = HttpStatusCode.Conflict
                            };
                            return null;
                        }
                    }
                }

                //   to check for errors returned by SetItemUPC, SetItemTotal or SetItemEquivalence

                if (!_policyManager.SITE_RTVAL) continue;
                if (oPurchaseList.RTVPError)
                {
                    //_resourceManager.CreateMessage(offSet,this, 18, temp_VbStyle5, "RTVP error " + Convert.ToString(oPurchaseList.RTVPResponse) + " from " + oPurchaseList.RTVPCommand, 0);
                    var msg = "RTVP error " + Convert.ToString(oPurchaseList.RTVPResponse) + " from" + oPurchaseList.RTVPCommand;
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 11, 18, msg, CriticalOkMessageType),
                        StatusCode = HttpStatusCode.Conflict
                    };
                    return null;
                }
            }

            if (_policyManager.SITE_RTVAL)
            {
                if (oPurchaseList.Count() > 0)
                {
                    // May 21, 2010 if the cashier clicked on Override in the treaty form, send override command here and don't go to override form anymore
                    short ret;
                    if (oTreatyNo.SendOverride)
                    {
                        ret = Convert.ToInt16(Variables.RTVPService.SetPermitNo(oTreatyNo.OverrideNumber));
                        WriteToLogFile("Response is " + Convert.ToString(ret) + " from SetPermitNo sent with parameter " + oTreatyNo.OverrideNumber);
                        for (i = 1; i <= oPurchaseList.Count(); i++)
                        {
                            oPurchaseList.Item(i).SetOverride(oTreatyNo.OverrideReason, oTreatyNo.OverrideNumber, "");
                        }
                    }

                    // GetCustomerStatus call is not necessary (based on email from SITE, it is not mandatory)
                    // LimitRequest call returns the status of the customer
                    WriteToLogFile("Before call to LimitRequest");
                    ret = Convert.ToInt16(Variables.RTVPService.LimitRequest());
                    var timeOut = mPrivateGlobals.theSystem.RTVP_TimeOut + 100;
                    var timeIn = (int)DateAndTime.Timer;
                    while (!(DateAndTime.Timer - timeIn > timeOut))
                    {
                        if (ret != 999)
                        {
                            break;
                        }
                        if (DateAndTime.Timer < timeIn)
                        {
                            timeIn = (int)DateAndTime.Timer;
                        }
                    }
                    WriteToLogFile("Response is " + Convert.ToString(ret) + " from LimitRequest sent with no parameters");

                    if (ret == 0 | ret == 4 | ret == 12)
                    {
                        // valid treaty number, no message to display, complete the transaction
                        oTreatyNo.ValidTreatyNo = true;
                    }
                    else
                    {
                        var strMessage = _siteMessageService.GetSiteMessage(ret);
                        if (string.IsNullOrEmpty(strMessage))
                        {
                            strMessage = _resourceManager.GetResString(offSet, 1118);
                        }

                        if (ret >= 5 && ret <= 11)
                        {
                            error = new ErrorMessage
                            {
                                MessageStyle = new MessageStyle
                                {
                                    Message = strMessage,
                                    MessageType = CriticalOkMessageType
                                },
                                StatusCode = HttpStatusCode.Conflict
                            };

                            oTreatyNo.ValidTreatyNo = false;
                            // return null;
                        }

                        else if ( ret >= 13 && ret <= 18 || ret >= 22 && ret <= 26)
                        {

                            //Chaps_Main.DisplayMsgForm(strMessage, 99, null, 0, 0, "", "", "", "");
                            error = new ErrorMessage
                            {
                                MessageStyle = new MessageStyle
                                {
                                    Message = strMessage,
                                    MessageType = CriticalOkMessageType
                                },
                                StatusCode = HttpStatusCode.Conflict
                            };

                            oTreatyNo.ValidTreatyNo = false;
                           // return null;
                        }
                        if ((ret >= 1 && ret <= 3) || (ret >= 19 && ret <= 21))
                        {
                            var boolHasTobaccoItems = false;
                            var boolHasFuelProducts = false;
                            for (i = 1; i <= oPurchaseList.Count(); i++)
                            {
                                oPurchaseList.Item(i).GetProductType(ref productType);
                                if (productType == mPrivateGlobals.teProductEnum.eCigar | productType == mPrivateGlobals.teProductEnum.eCigarette | productType == mPrivateGlobals.teProductEnum.eLooseTobacco)
                                {
                                    boolHasTobaccoItems = true;
                                }
                                else if (productType == mPrivateGlobals.teProductEnum.eGasoline | productType == mPrivateGlobals.teProductEnum.eDiesel | productType == mPrivateGlobals.teProductEnum.ePropane | productType == mPrivateGlobals.teProductEnum.emarkedGas | productType == mPrivateGlobals.teProductEnum.emarkedDiesel) // hen
                                {
                                    boolHasFuelProducts = true;
                                }
                            }
                            //   end
                            if (ret == 1 | ret == 19) // 1 - Success. Tobacco overlimit
                            {
                                if (boolHasTobaccoItems)
                                {
                                    oPurchaseList.IsTobaccoOverLimit = true;
                                }
                            }
                            else if (ret == 2 | ret == 20) // 2 - Success. Fuel overlimit
                            {
                                if (boolHasFuelProducts)
                                {
                                    oPurchaseList.IsFuelOverLimit = true;
                                }
                            }
                            else if (ret == 3 | ret == 21) // 3 - Success. Tobacco & Fuel Overlimit
                            {
                                if (boolHasFuelProducts)
                                {
                                    oPurchaseList.IsFuelOverLimit = true;
                                }
                                if (boolHasTobaccoItems)
                                {
                                    oPurchaseList.IsTobaccoOverLimit = true;
                                }
                            }
                            //   moved here to display the quantity in message
                            if (oPurchaseList.IsTobaccoOverLimit)
                            {
                                ret = Convert.ToInt16(Variables.RTVPService.GetRemainingTobaccoQuantity());
                                WriteToLogFile("Response is " + Convert.ToString(ret) + " from GetRemainingTobaccoQuantity sent with no parameters");
                                _treatyManager.SetRemainingTobaccoQuantity(ref oTreatyNo, ref oPurchaseList, ref sale, ret);
                            }

                            if (oPurchaseList.IsFuelOverLimit)
                            {
                                ret = Convert.ToInt16(Variables.RTVPService.GetRemainingFuelQuantity());
                                WriteToLogFile("Response is " + Convert.ToString(ret) + " from GetRemainingFuelQuantity sent with no parameters");
                                _treatyManager.SetRemainingFuelQuantity(ref oTreatyNo, ref oPurchaseList, ref sale, ret);
                            }

                            if (Convert.ToBoolean(strMessage.IndexOf("TTT", StringComparison.Ordinal) + 1))
                            {
                                strMessage = strMessage.Replace("TTT", oTreatyNo.RemainingTobaccoQuantity + " ");
                            }
                            if (Convert.ToBoolean(strMessage.IndexOf("FFF", StringComparison.Ordinal) + 1))
                            {
                                strMessage = strMessage.Replace("FFF", oTreatyNo.RemainingFuelQuantity + " ");
                            }

                            //Chaps_Main.DisplayMsgForm(strMessage, 99, null, 0, 0, "", "", "", "");
                            error = new ErrorMessage
                            {
                                MessageStyle = new MessageStyle
                                {
                                    Message = strMessage,
                                    MessageType = CriticalOkMessageType
                                },
                                StatusCode = HttpStatusCode.Conflict
                            };
                           // return null;

                        }
                        return null;
                    }
                }
                else
                {
                    oTreatyNo.ValidTreatyNo = false;
                }
            }
            //   end
            _saleManager.ApplyTaxes(ref sale, false);
            //sale.ApplyTaxes = false; 


            if (oPurchaseList.Count() > 0)
            {
                if (oPurchaseList.IsFuelOverLimit || oPurchaseList.IsTobaccoOverLimit)
                {
                    oPurchaseList.Show();

                    if (_policyManager.USE_OVERRIDE && !_policyManager.SITE_RTVAL)
                    {
                        if (_policyManager.TAX_EXEMPT_FNGTR && oPurchaseList.IsFuelOverLimit)
                        {
                            Variables.STFDNumber = "0";
                            result.IsFngtr = true;
                            result.FngtrMessage = _resourceManager.GetResString(offSet, 487);
                            _saleManager.ReCompute_Totals(ref sale);

                            CacheManager.AddPurchaseListSaleForTill(sale.TillNumber, sale.Sale_Num, oPurchaseList);
                            if (mPrivateGlobals.theSystem != null)
                            {
                                CacheManager.AddTeSystemForTill(sale.TillNumber, sale.Sale_Num, mPrivateGlobals.theSystem);
                            }
                            return result;

                            //while (!string.IsNullOrEmpty(Variables.STFDNumber))
                            //{
                            //    //Chaps_Main.DisplayMsgForm(_resourceManager.GetResString(offSet,487), 22, null, 0, 0, "", "", "", "");
                            //    error = new ErrorMessage
                            //    {
                            //        MessageStyle = new MessageStyle
                            //        {
                            //            Message = _resourceManager.GetResString(offSet,487),
                            //            MessageType = CriticalOkMessageType
                            //        },
                            //        StatusCode = HttpStatusCode.Conflict
                            //    };
                            //    if (!string.IsNullOrEmpty(Variables.STFDNumber))
                            //    {
                            //        if (Information.IsNumeric(Variables.STFDNumber))
                            //        {
                            //            oTreatyNo.PhoneNumber = Variables.STFDNumber;
                            //            Variables.STFDNumber = ""; //reset STFDNumber
                            //        }
                            //        else
                            //        {
                            //            Variables.STFDNumber = "0";
                            //        }
                            //    }
                            //    else
                            //    {
                            //        Variables.STFDNumber = "0";
                            //    }
                            //}
                            //   end
                        }
                        else
                        {
                            //Chaps_Main.FormLoadChild(frmOverrideLimit.Default);
                            result.IsFrmOverrideLimit = true;
                            for (i = 1; i <= sale.Sale_Lines.Count(); i++)
                            {
                                if (sale.Sale_Lines[i].IsTaxExemptItem == false && sale.Sale_Lines[i].Line_Taxes.Count != 0)
                                {
                                    sale.Sale_Lines[i].Line_Taxes = null;
                                    sale.Sale_Lines[i].IsTaxExemptItem = true;
                                }
                            }
                            _saleManager.ReCompute_Totals(ref sale);

                            if (!sale.LoadingTemp)
                            {
                                _saleService.SaveSale(sale.TillNumber, sale.Sale_Num, sale);
                            }

                                CacheManager.AddPurchaseListSaleForTill(sale.TillNumber, sale.Sale_Num, oPurchaseList);
                           // Chaps_Main.Transaction_Type = "storing the temprory data";
                            if (mPrivateGlobals.theSystem != null)
                            {
                                CacheManager.AddTeSystemForTill(sale.TillNumber, sale.Sale_Num, mPrivateGlobals.theSystem);
                            }
                            return result;
                        }
                    }
                }

                if (oPurchaseList.Count() > 0)
                {
                    for (i = 1; i <= oPurchaseList.Count(); i++)
                    {
                        var rowNumber = oPurchaseList.Item(i).GetRowInSalesMain();

                        var saleLine = sale.Sale_Lines[rowNumber];





                        if (saleLine.Amount < 0 && saleLine.IsTaxExemptItem)
                        {
                            oPurchaseList.Item(i).SetTaxFreePrice((float)saleLine.price);
                            oPurchaseList.Item(i).WasTaxExemptReturn = true;
                        }
                        else
                        {
                            oPurchaseList.Item(i).WasTaxExemptReturn = false;

                                
                            saleLine.IsTaxExemptItem = true;

                            //  added this to take the cost and price for Tax exempt vandor ( if there is a special setup for TE Vendor- otherwise use active vendor values
                            if (string.IsNullOrEmpty(saleLine.TEVendor)) // If TE vendor is not defined consider TE vendor  same as activevendor
                            {
                                saleLine.TEVendor = saleLine.Vendor;
                                saleLine.TECost = saleLine.Cost;
                            }
                            else
                            {
                                if (saleLine.Vendor == saleLine.TEVendor) // IF TAX EXEMPT VENDOR
                                {
                                    saleLine.TEVendor = saleLine.Vendor;
                                    saleLine.TECost = saleLine.Cost;
                                }
                                else
                                {
                                    // Set the cost for the product based on the TE vendor,
                                    var cost = _siteMessageService.GetStockCost(saleLine.Stock_Code, saleLine.TEVendor);
                                    saleLine.TECost = cost.HasValue ? cost.Value : saleLine.Cost;
                                }
                                if (saleLine.TEVendor != saleLine.Vendor)
                                {
                                    var getVendorPrice = true;

                                    saleLine.Vendor = saleLine.TEVendor;
                                    saleLine.Cost = saleLine.TECost;

                                    if (_policyManager.TE_ByRate && getVendorPrice && saleLine.ProductIsFuel == false) //shiny - need to do the price change only for nonfuel products
                                    {
                                        saleLine.Price_Number = sale.Customer.Price_Code != 0 ? sale.Customer.Price_Code : (short)1;
                                        oPurchaseList.Item(i).SetTaxFreePrice((float)saleLine.price);
                                        oPurchaseList.Item(i).SetOriginalPrice((float)saleLine.Regular_Price);
                                    }
                                }
                            }
                            // settings
                            saleLine.OrigVendor = saleLine.Vendor;
                            saleLine.OrigCost = saleLine.Cost;
                            //   end

                            _saleLineManager.SetPrice(ref saleLine, oPurchaseList.Item(i).GetTaxFreePrice());
                            //saleLine.price = oPurchaseList.Item(i).GetTaxFreePrice();
                            // 

                            if (oPurchaseList.Item(i).GetOverrideCode(ref nOverRide))
                            {
                                saleLine.overrideCode = nOverRide;
                            }



                            //saleLine.Amount = decimal.Parse((saleLine.Quantity * saleLine.price).ToString("#0.00"));
                            _saleLineManager.SetAmount(ref saleLine, decimal.Parse((saleLine.Quantity * saleLine.price).ToString("#0.00")));
                            if (saleLine.Prepay)
                            {
                                saleLine.No_Loading = false; // 
                            }
                        }
                    }
                }
            }
            _saleManager.ReCompute_Totals(ref sale);

            CacheManager.AddPurchaseListSaleForTill(sale.TillNumber, sale.Sale_Num, oPurchaseList);
            if (mPrivateGlobals.theSystem != null)
            {
                CacheManager.AddTeSystemForTill(sale.TillNumber, sale.Sale_Num, mPrivateGlobals.theSystem);
            }

            var tenders = _tenderManager.GetAllTender(sale.Sale_Num, sale.TillNumber, "Sale", user.Code, false, "", out error);
            result.SaleSummary = SaleSummary(sale);
            result.Tenders = tenders;
            result.Tenders.EnableRunAway = EnableRunaway(user, sale);

            var retx = Convert.ToInt16(Variables.RTVPService.PurchaseTransaction());
            WriteToLogFile("Response is " + Convert.ToString(retx) + " from PurchaseTransaction sent with no parameters");

            for (i = 1; i <= sale.Sale_Lines.Count(); i++)
            {
                if (sale.Sale_Lines[i].IsTaxExemptItem == false && sale.Sale_Lines[i].Line_Taxes.Count !=0)
                {
                    sale.Sale_Lines[i].Line_Taxes = null;
                    sale.Sale_Lines[i].IsTaxExemptItem = true;
                }
            }
            
            CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
            
            _saleManager.ReCompute_Totals(ref sale);

            if (!sale.LoadingTemp)
            {
                _saleService.SaveSale(sale.TillNumber, sale.Sale_Num, sale);
                WriteToLogFile("Sale is saved for SITE");
            }
            return result;
        }

        /// <summary>
        /// Method to clear tax exempt
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oTeSale">tax exempt sale</param>
        /// <param name="productType">Product type</param>
        private void ClearTaxExempt(ref Sale sale, TaxExemptSale oTeSale, byte productType)
        {
            foreach (TaxExemptSaleLine tempLoopVarTesl in oTeSale.Te_Sale_Lines)
            {
                var tesl = tempLoopVarTesl;

                if (tesl.OverLimit)
                {
                    var needRemove = false;
                    switch (productType)
                    {
                        case (byte)TeProductType.Tobacco:
                            if (tesl.ProductType == mPrivateGlobals.teProductEnum.eCigar | tesl.ProductType == mPrivateGlobals.teProductEnum.eCigarette | tesl.ProductType == mPrivateGlobals.teProductEnum.eLooseTobacco)
                            {
                                needRemove = true;
                            }
                            break;
                        case (byte)TeProductType.Gas:
                            if (tesl.ProductType == mPrivateGlobals.teProductEnum.eGasoline | tesl.ProductType == mPrivateGlobals.teProductEnum.eDiesel | tesl.ProductType == mPrivateGlobals.teProductEnum.emarkedGas | tesl.ProductType == mPrivateGlobals.teProductEnum.emarkedDiesel) // hen
                            {
                                needRemove = true;
                            }
                            break;
                        case (byte)TeProductType.Propane:
                            if (tesl.ProductType == mPrivateGlobals.teProductEnum.ePropane)
                            {
                                needRemove = true;
                            }
                            break;
                    }
                    if (needRemove)
                    {
                        //  - To clear the TE sale amount if not entering the Override reason. Then if we cancel it was keepi
                        if (!tesl.WasTaxExemptReturn)
                        {
                            object rowNumber = tesl.Line_Num;
                            sale.Sale_Lines[rowNumber].IsTaxExemptItem = false;
                            //sale.Sale_Lines[RowNumber].price = TESL.OriginalPrice;
                            var saleLine = sale.Sale_Lines[rowNumber];
                            _saleLineManager.SetPrice(ref saleLine, tesl.OriginalPrice);
                            sale.Sale_Lines[rowNumber].price = saleLine.price;
                            sale.Sale_Lines[rowNumber].Amount = saleLine.Amount;




                            if (tesl.IsFuelItem)
                            {
                                sale.Sale_Lines[rowNumber].Amount = (decimal)(tesl.TaxInclPrice - (float)tesl.IncludedTax);
                            }
                            sale.Sale_Lines[rowNumber].Total_Amount = (decimal)tesl.TaxInclPrice; // if we are using the TE customer, but not using the TE price, this should be set same as the amount, otherwise receipt is using. but if we cancel the TE customer it can be reversed back to zero
                            oTeSale.teCardholder.GstExempt = true;
                            sale.Sale_Lines[rowNumber].overrideCode = 0;
                            //  - to reverse vendor and cost also from TE vendor\cost to orig vendor\cost
                            sale.Sale_Lines[rowNumber].Cost = sale.Sale_Lines[rowNumber].OrigCost;
                            sale.Sale_Lines[rowNumber].Vendor = sale.Sale_Lines[rowNumber].OrigVendor;
                            // 
                            oTeSale.Remove_TE_Line(tesl.ItemID);
                        }
                        //                Call SA.ReCompute_Totals
                        // 
                    }
                }
            }
            _saleManager.ReCompute_Totals(ref sale);
        }

        /// <summary>
        /// Method to enable run away or not
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="sale">Sale</param>
        /// <returns>True or false</returns>
        private bool EnableRunaway(User user, Sale sale)
        {
            if (user.User_Group.Code == Constants.Trainer) //Behrooz Jan-12-06
            {
                return false;
            }
            if (sale.Sale_Totals.Gross > 0) //  - Enable runaway and pump button only if amount > 0
            {

                return (sale.Sale_Lines.Count != 0)
                    && CountUnPrepayFuelLines(sale) == sale.Sale_Lines.Count
                    && _policyManager.GetPol("U_RUNAWAY", user);
            }
            return false;
        }


        /// <summary>
        /// Method to count prepay fuel lines
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <returns>Prepay lines</returns>
        private byte CountUnPrepayFuelLines(Sale sale)
        {
            byte returnValue = 0;

            foreach (Sale_Line tempLoopVarSlCount in sale.Sale_Lines)
            {
                var slCount = tempLoopVarSlCount;
                if (slCount.ProductIsFuel && (!slCount.IsPropane) && (!slCount.Prepay))
                {
                    returnValue++;
                }
            }
            return returnValue;
        }

        #endregion
    }
}

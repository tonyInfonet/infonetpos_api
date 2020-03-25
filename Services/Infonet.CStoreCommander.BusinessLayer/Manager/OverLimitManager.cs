using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using System;
using System.Collections.Generic;
using System.Net;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class OverLimitManager : ManagerBase, IOverLimitManager
    {
        private readonly IApiResourceManager _resourceManager;
        private readonly ITeSystemManager _teSystemManager;
        private readonly ISaleManager _saleManager;
        private readonly ISaleLineManager _saleLineManager;
        private readonly ITenderManager _tenderManager;
        private readonly IPolicyManager _policyManager;

        private enum TeProductType
        {
            Tobacco = 1,
            Gas = 2,
            Propane = 3
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="teSystemManager"></param>
        /// <param name="saleManager"></param>
        /// <param name="saleLineManager"></param>
        /// <param name="tenderManager"></param>
        /// <param name="policyManager"></param>
        public OverLimitManager(
            IApiResourceManager resourceManager,
            ITeSystemManager teSystemManager,
            ISaleManager saleManager,
            ISaleLineManager saleLineManager,
            ITenderManager tenderManager,
            IPolicyManager policyManager)
        {
            _resourceManager = resourceManager;
            _teSystemManager = teSystemManager;
            _saleManager = saleManager;
            _saleLineManager = saleLineManager;
            _tenderManager = tenderManager;
            _policyManager = policyManager;
        }

        /// <summary>
        /// Method to get overlimit details
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="error">Error message</param>
        /// <returns>Over limit response</returns>
        public OverLimitResponse GetOverLimitDetails(int tillNumber, int saleNumber, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var result = new OverLimitResponse
            {
                GasReasons = new List<TaxExemptReasonResponse>(),
                TobaccoReasons = new List<TaxExemptReasonResponse>(),
                PropaneReasons = new List<TaxExemptReasonResponse>()
            };

            if (!_policyManager.USE_OVERRIDE || _policyManager.TE_Type != "AITE")
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Please select AITE Tax Exempt and enable Use override policy in BackOffice",
                        MessageType = 0
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return result;
            }

            mPrivateGlobals.theSystem = CacheManager.GetTeSystemForTill(tillNumber, saleNumber);
            if (mPrivateGlobals.theSystem == null)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle { Message = "Request is Invalid", MessageType = 0 },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }

            var oTeSale = CacheManager.GetTaxExemptSaleForTill(tillNumber, saleNumber);
            if (oTeSale == null)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle { Message = "Request is Invalid", MessageType = 0 },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }

            _teSystemManager.GetAllReasons(ref mPrivateGlobals.theSystem);
            _teSystemManager.GetAllLimits(ref mPrivateGlobals.theSystem);
            TaxExemptReason ter;
            if (oTeSale.GasOverLimit)
            {
                if (mPrivateGlobals.theSystem.GasReasons.Count > 0)
                {
                    foreach (TaxExemptReason tempLoopVarTer in mPrivateGlobals.theSystem.GasReasons)
                    {
                        ter = tempLoopVarTer;
                        result.GasReasons.Add(
                            new TaxExemptReasonResponse
                            {
                                Reason = ter.Code + "-" + ter.Description,
                                ExplanationCode = ter.ExplanationCode
                            });
                    }
                }
                result.IsGasReasons = true;
            }


            if (oTeSale.TobaccoOverLimit)
            {
                foreach (TaxExemptReason tempLoopVarTer in mPrivateGlobals.theSystem.TobaccoReasons)
                {
                    ter = tempLoopVarTer;
                    result.TobaccoReasons.Add(
                            new TaxExemptReasonResponse
                            {
                                Reason = ter.Code + "-" + ter.Description,
                                ExplanationCode = ter.ExplanationCode
                            });
                }
                result.IsTobaccoReasons = true;
            }


            if (oTeSale.PropaneOverLimit)
            {
                foreach (TaxExemptReason tempLoopVarTer in mPrivateGlobals.theSystem.PropaneReasons)
                {
                    ter = tempLoopVarTer;
                    result.PropaneReasons.Add(
                            new TaxExemptReasonResponse
                            {
                                Reason = ter.Code + "-" + ter.Description,
                                ExplanationCode = ter.ExplanationCode
                            });
                }
                result.IsPropaneReasons = true;
            }
            CacheManager.AddTaxExemptSaleForTill(tillNumber, saleNumber, oTeSale);
            CacheManager.AddTeSystemForTill(tillNumber, saleNumber, mPrivateGlobals.theSystem);
            result.TaxExemptSale = GetTaxExemptSaleModel(oTeSale);

            return result;
        }

        /// <summary>
        /// Method to save overrlimit
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="cobReason">Reason</param>
        /// <param name="txtExplanation">Explaination</param>
        /// <param name="txtLocation">Loaction</param>
        /// <param name="dtpDate">Date</param>
        /// <param name="error">Error</param>
        /// <returns>Aite response</returns>
        public AiteCardResponse DoneOverLimit(int tillNumber, int saleNumber, string userCode, string cobReason, string txtExplanation, string txtLocation, DateTime dtpDate, out ErrorMessage error)
        {
            AiteCardResponse result = new AiteCardResponse();
            var oTeSale = CacheManager.GetTaxExemptSaleForTill(tillNumber, saleNumber);
            mPrivateGlobals.theSystem = CacheManager.GetTeSystemForTill(tillNumber, saleNumber);

            var sale = CacheManager.GetCurrentSaleForTill(tillNumber, saleNumber);
            if (!_policyManager.USE_OVERRIDE || _policyManager.TE_Type != "AITE")
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "Please select AITE Tax Exempt and enable Use override policy in BackOffice",
                        MessageType = 0
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return result;
            }

            if (sale == null)
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = "The request is invalid.",
                        MessageType = 0
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return null;
            }
            if (oTeSale.TobaccoOverLimit) 
            {
                if (!ValidExplanation(ref oTeSale, (byte)TeProductType.Tobacco, cobReason, txtExplanation, txtLocation, dtpDate))
                {
                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 54, 61, null, YesNoQuestionMessageType),
                        StatusCode = HttpStatusCode.Forbidden
                    };
                    return null;
                }
            }

            if (oTeSale.GasOverLimit) 
            {
                if (!ValidExplanation(ref oTeSale, (byte)TeProductType.Gas, cobReason, txtExplanation, txtLocation, dtpDate))
                {
                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 54, 62, null, YesNoQuestionMessageType),
                        StatusCode = HttpStatusCode.Forbidden
                    };
                    return null;
                }
            }

            if (oTeSale.PropaneOverLimit) 
            {
                if (!ValidExplanation(ref oTeSale, (byte)TeProductType.Propane, cobReason, txtExplanation, txtLocation, dtpDate))
                {
                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 54, 63, null, YesNoQuestionMessageType),
                        StatusCode = HttpStatusCode.Forbidden
                    };
                    return null;
                }
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
                        _saleLineManager.SetPrice(ref saleLine, tesl.TaxFreePrice);
                        _saleLineManager.SetAmount(ref saleLine, (decimal)tesl.Amount);
                        //saleLine.price = tesl.TaxFreePrice;
                        //saleLine.Amount = (decimal)tesl.Amount;
                    }
                }
                //_saleManager.ReCompute_Totals(ref sale);
            }
            _saleManager.ReCompute_Totals(ref sale);
            var tenders = _tenderManager.GetAllTender(sale.Sale_Num, sale.TillNumber, "Sale", userCode, false, "", out error);

            result.SaleSummary = SaleSummary(sale);

            result.Tenders = tenders;
            return result;
        }

        #region Private methods

        /// <summary>
        /// Methd to verify if valid explaination
        /// </summary>
        /// <param name="oTeSale">Tax exempt sale</param>
        /// <param name="productType">Product type</param>
        /// <param name="cobReason">Reason</param>
        /// <param name="txtExplanation">Explaination</param>
        /// <param name="txtLocation">Location</param>
        /// <param name="dtpDate">Date</param>
        /// <returns>True or false</returns>
        private bool ValidExplanation(ref TaxExemptSale oTeSale, byte productType, string cobReason, string txtExplanation, string txtLocation, DateTime dtpDate)
        {
            bool returnValue = false;

            
            TaxExemptReason ter = default(TaxExemptReason);
            TaxExemptReasons ters = default(TaxExemptReasons);

            bool fndReason = false;
            string strReason = "";
            string strReasonDesp = "";
            string strReasonDetail = "";

            string cobBox = string.Empty;
            string expBox = string.Empty;
            string locBox = string.Empty;
            DateTime dateBox = DateTime.Now;

            if (string.IsNullOrEmpty(txtExplanation))
            {
                txtExplanation = String.Empty;
            }
            if (string.IsNullOrEmpty(locBox))
            {
                locBox = String.Empty;
            }

            if (productType != (byte)TeProductType.Tobacco & productType != (byte)TeProductType.Gas & productType != (byte)TeProductType.Propane)
            {
                return false;
            }

            
            if (productType == (byte)TeProductType.Tobacco)
            {
                ters = mPrivateGlobals.theSystem.TobaccoReasons;
                cobBox = cobReason;
                expBox = txtExplanation;
                locBox = txtLocation;
                dateBox = dtpDate;
            }
            else if (productType == (byte)TeProductType.Gas)
            {
                ters = mPrivateGlobals.theSystem.GasReasons;
                cobBox = cobReason;
                expBox = txtExplanation;
                locBox = txtLocation;
                dateBox = dtpDate;
            }
            else if (productType == (byte)TeProductType.Propane)
            {
                ters = mPrivateGlobals.theSystem.PropaneReasons;
                cobBox = cobReason;
                expBox = txtExplanation;
                locBox = txtLocation;
                dateBox = dtpDate;
            }

            
            if (ters != null)
                foreach (TaxExemptReason tempLoopVarTer in ters)
                {
                    ter = tempLoopVarTer;
                    if (cobBox == ter.Code + "-" + ter.Description)
                    {
                        fndReason = true;
                        break;
                    }
                }

            if (fndReason)
            {
                
                switch (ter.ExplanationCode)
                {
                    case 0: 
                        returnValue = true;
                        strReason = ter.Code;
                        strReasonDesp = ter.Description;
                        strReasonDetail = expBox.Trim();
                        break;
                    case 1: 
                    case 2:
                        if (expBox.Trim() == "")
                        {
                            returnValue = false;
                        }
                        else
                        {
                            returnValue = true;
                            strReason = ter.Code;
                            strReasonDesp = ter.Description;
                            strReasonDetail = expBox.Trim();
                        }
                        break;
                    case 3: 
                        if (string.IsNullOrEmpty(locBox))
                        {
                            returnValue = false;
                        }
                        else
                        {
                            returnValue = true;
                            strReason = ter.Code;
                            strReasonDesp = ter.Description;
                            
                            strReasonDetail = dateBox.ToString("MM/dd/yyyy") + "####" + locBox.Trim();
                        }
                        break;
                }
            }

            if (productType == (byte)TeProductType.Tobacco)
            {
                oTeSale.TobaccoReason = strReason;
                oTeSale.TobaccoReasonDesp = strReasonDesp;
                oTeSale.TobaccoReasonDetail = strReasonDetail;
            }
            else if (productType == (byte)TeProductType.Gas)
            {
                oTeSale.GasReason = strReason;
                oTeSale.GasReasonDesp = strReasonDesp;
                oTeSale.GasReasonDetail = strReasonDetail;
            }
            else if (productType == (byte)TeProductType.Propane)
            {
                oTeSale.PropaneReason = strReason;
                oTeSale.PropaneReasonDesp = strReasonDesp;
                oTeSale.PropaneReasonDetail = strReasonDetail;
            }
            return returnValue;
        }


        /// <summary>
        /// Sale Summary
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        private Dictionary<string, string> SaleSummary(Sale sale)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Sale_Tax saleTaxRenamed;
            decimal curTotalTaxes = 0;
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (sale.Sale_Totals.Gross < 0)
            {
                result.Add(_resourceManager.GetResString(offSet, 266), sale.Sale_Totals.Net.ToString("###,##0.00"));

                foreach (Sale_Tax tempLoopVarSaleTaxRenamed in sale.Sale_Totals.Sale_Taxes)
                {
                    saleTaxRenamed = tempLoopVarSaleTaxRenamed;
                    if ((double)Math.Abs(saleTaxRenamed.Tax_Added_Amount) > 0.005)
                    {
                        curTotalTaxes = curTotalTaxes + saleTaxRenamed.Tax_Added_Amount;
                    }
                }

                result.Add(_resourceManager.GetResString(offSet, 137), curTotalTaxes.ToString("###,##0.00"));

                if (sale.Sale_Totals.Charge != 0)
                {
                    result.Add(_resourceManager.GetResString(offSet, 138), sale.Sale_Totals.Charge.ToString("###,##0.00"));
                }

                result.Add(_resourceManager.GetResString(offSet, 210), sale.Sale_Totals.Gross.ToString("###,##0.00"));
            }
            else
            {
                result.Add(_resourceManager.GetResString(offSet, 267), sale.Sale_Totals.Net.ToString("###,##0.00"));

                foreach (Sale_Tax tempLoopVarSaleTaxRenamed in sale.Sale_Totals.Sale_Taxes)
                {
                    saleTaxRenamed = tempLoopVarSaleTaxRenamed;
                    if ((double)Math.Abs(saleTaxRenamed.Tax_Added_Amount) > 0.005)
                    {
                        curTotalTaxes = curTotalTaxes + saleTaxRenamed.Tax_Added_Amount;
                    }
                }
                result.Add(_resourceManager.GetResString(offSet, 137), curTotalTaxes.ToString("###,##0.00"));

                if (sale.Sale_Totals.Charge != 0)
                {
                    result.Add(_resourceManager.GetResString(offSet, 138), sale.Sale_Totals.Charge.ToString("###,##0.00"));
                }
                result.Add(_resourceManager.GetResString(offSet, 210), sale.Sale_Totals.Gross.ToString("###,##0.00"));
            }

            return result;
        }

        /// <summary>
        /// Method to get tax exempt sale model
        /// </summary>
        /// <param name="teSale">Tax exempt sale</param>
        /// <returns>List of tax exempt response</returns>
        private List<TaxExemptSaleResponse> GetTaxExemptSaleModel(TaxExemptSale teSale)
        {
            List<TaxExemptSaleResponse> teSalesModel = new List<TaxExemptSaleResponse>();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            foreach (TaxExemptSaleLine tempLoopVarTesl in teSale.Te_Sale_Lines)
            {
                TaxExemptSaleResponse model = new TaxExemptSaleResponse();
                var tesl = tempLoopVarTesl;
                if (tesl.OverLimit)
                {
                    if ((tesl.ProductType == mPrivateGlobals.teProductEnum.eCigarette) || (tesl.ProductType == mPrivateGlobals.teProductEnum.eCigar) || (tesl.ProductType == mPrivateGlobals.teProductEnum.eLooseTobacco))
                    {

                        model.Type = _resourceManager.GetResString(offSet, 5418); //"Tobacco"
                        model.Product = tesl.StockCode; // ProductKey
                        model.Quantity = tesl.Quantity.ToString("#0.00");
                        model.RegularPrice = tesl.OriginalPrice.ToString("#0.00");
                        model.TaxFreePrice = tesl.TaxFreePrice.ToString("#0.00");
                        model.ExemptedTax = tesl.ExemptedTax.ToString("$#0.00");
                        model.QuotaUsed = tesl.RunningQuota.ToString("$#0.00");
                        model.QuotaLimit = mPrivateGlobals.theSystem.TobaccoLimit.ToString("$#0.00");
                    }
                    else if ((tesl.ProductType == mPrivateGlobals.teProductEnum.eGasoline) || (tesl.ProductType == mPrivateGlobals.teProductEnum.eDiesel) || (tesl.ProductType == mPrivateGlobals.teProductEnum.emarkedGas) || (tesl.ProductType == mPrivateGlobals.teProductEnum.emarkedDiesel))
                    {

                        model.Type = _resourceManager.GetResString(offSet, 5419); // "Gasoline"
                        model.Product = tesl.StockCode; // ProductKey
                        model.Quantity = tesl.Quantity.ToString("#0.00");
                        model.RegularPrice = tesl.OriginalPrice.ToString("#0.00");
                        model.TaxFreePrice = tesl.TaxFreePrice.ToString("#0.00");
                        model.ExemptedTax = tesl.ExemptedTax.ToString("$#0.00");
                        model.QuotaUsed = tesl.RunningQuota.ToString("$#0.00");
                        model.QuotaLimit = mPrivateGlobals.theSystem.GasLimit.ToString("$#0.00");
                    }
                    else if (tesl.ProductType == mPrivateGlobals.teProductEnum.ePropane)
                    {
                        model.Type = _resourceManager.GetResString(offSet, 5421); //"Propane"
                        model.Product = tesl.StockCode; // ProductKey
                        model.Quantity = tesl.Quantity.ToString("#0.00");
                        model.RegularPrice = tesl.OriginalPrice.ToString("#0.00");
                        model.TaxFreePrice = tesl.TaxFreePrice.ToString("#0.00");
                        model.ExemptedTax = tesl.ExemptedTax.ToString("$#0.00");
                        model.QuotaUsed = tesl.RunningQuota.ToString("$#0.00");
                        model.QuotaLimit = mPrivateGlobals.theSystem.PropaneLimit.ToString("$#0.00");
                    }
                    teSalesModel.Add(model);
                }
            }
            return teSalesModel;
        }

        #endregion
    }
}

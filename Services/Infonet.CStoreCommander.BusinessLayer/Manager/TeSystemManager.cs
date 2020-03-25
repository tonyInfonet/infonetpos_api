using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class TeSystemManager : ITeSystemManager
    {
        private readonly IPolicyManager _policyManager;
        private readonly ITreatyService _treatyService;
        private readonly ITaxService _taxService;
        private readonly ITeSystemService _teSystemService;
        private readonly IFuelPumpService _fuelPumpService;
        private readonly IReasonService _reasonService;
        private readonly IStockService _stockService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="policyManager"></param>
        /// <param name="treatyService"></param>
        /// <param name="taxService"></param>
        /// <param name="teSystemService"></param>
        /// <param name="fuelPumpService"></param>
        /// <param name="reasonService"></param>
        /// <param name="stockService"></param>
        public TeSystemManager(IPolicyManager policyManager,
            ITreatyService treatyService,
            ITaxService taxService,
            ITeSystemService teSystemService,
            IFuelPumpService fuelPumpService,
            IReasonService reasonService,
            IStockService stockService)
        {
            _policyManager = policyManager;
            _treatyService = treatyService;
            _taxService = taxService;
            _teSystemService = teSystemService;
            _fuelPumpService = fuelPumpService;
            _reasonService = reasonService;
            _stockService = stockService;
        }

        /// <summary>
        /// Method to get tax free price
        /// </summary>
        /// <param name="oTeSystem">Tax exempt system</param>
        /// <param name="purchaseItem">Purchse item</param>
        /// <param name="sProductKey">Product key</param>
        /// <param name="originalPrice">Original price</param>
        /// <param name="bFound">Value found</param>
        /// <param name="stockcode">Stock code</param>
        /// <param name="productCode">Product code</param>
        /// <param name="outTaxExemptRate">tax exempt rate</param>
        /// <returns>True or false</returns>
        public bool TeGetTaxFreePrice(ref teSystem oTeSystem, ref tePurchaseItem purchaseItem, string sProductKey, float originalPrice, bool bFound, string stockcode, string productCode, float outTaxExemptRate)
        {
            
            var returnValue = false;
            var dOutPrice = purchaseItem.TaxFreePrice;
            var dOutUnitsPerPkg = purchaseItem.UnitsPerPkg;
            var eOutCategory = purchaseItem.ProdType;
            var sOutUpcCode = purchaseItem.UpcCode;
            if (_policyManager.TE_ByRate && _policyManager.TE_Type != "SITE") //sghiny dec15, 2009
            {
                if (TeGetTaxFreePriceByRate(ref sProductKey, originalPrice, ref dOutPrice, ref dOutUnitsPerPkg, ref eOutCategory, ref sOutUpcCode, ref bFound, ref outTaxExemptRate, stockcode, ref productCode))
                {
                    returnValue = true;
                }
            }
            else
            {
                if (TeGetTaxFreePriceByPrice(ref oTeSystem, ref sProductKey, originalPrice, ref dOutPrice, ref dOutUnitsPerPkg, ref eOutCategory, ref sOutUpcCode, ref bFound, stockcode))
                {
                    returnValue = true;
                }
            }
            purchaseItem.TaxFreePrice = dOutPrice;
            purchaseItem.UnitsPerPkg = dOutUnitsPerPkg;
            purchaseItem.ProdType = eOutCategory;
            purchaseItem.UpcCode = sOutUpcCode;
            return returnValue;
        }

        /// <summary>
        /// Method to get tax free price
        /// </summary>
        /// <param name="oTeSystem">Tax exempt system</param>
        /// <param name="saleLine">Sale line</param>
        /// <param name="bFound">Value found</param>
        /// <returns>True or falses</returns>
        public bool TeGetTaxFreePrice(ref teSystem oTeSystem, ref TaxExemptSaleLine saleLine, ref bool bFound)
        {
            
            var returnValue = false;

            string sProductKey = saleLine.ProductKey;
            float originalPrice = saleLine.OriginalPrice;
            var dOutPrice = saleLine.TaxFreePrice;
            var dOutUnitsPerPkg = saleLine.UnitsPerPkg;
            var eOutCategory = saleLine.ProductType;
            var sOutUpcCode = saleLine.UpcCode;
            var stockcode = saleLine.StockCode;
            var productCode = saleLine.ProductCode;
            var outTaxExemptRate = saleLine.TaxExemptRate;

            if (_policyManager.TE_ByRate && _policyManager.TE_Type != "SITE") //sghiny dec15, 2009
            {
                if (TeGetTaxFreePriceByRate(ref sProductKey, originalPrice, ref dOutPrice, ref dOutUnitsPerPkg, ref eOutCategory, ref sOutUpcCode, ref bFound, ref outTaxExemptRate, stockcode, ref productCode))
                {
                    returnValue = true;
                }
            }
            else
            {
                if (TeGetTaxFreePriceByPrice(ref oTeSystem, ref sProductKey, originalPrice, ref dOutPrice, ref dOutUnitsPerPkg, ref eOutCategory, ref sOutUpcCode, ref bFound, stockcode))
                {
                    returnValue = true;
                }
            }
            saleLine.ProductKey = sProductKey;
            saleLine.OriginalPrice = originalPrice;
            saleLine.TaxFreePrice = dOutPrice;
            saleLine.UnitsPerPkg = dOutUnitsPerPkg;
            saleLine.ProductType = eOutCategory;
            saleLine.UpcCode = sOutUpcCode;
            saleLine.StockCode = stockcode;
            saleLine.ProductCode = productCode;
            saleLine.TaxExemptRate = outTaxExemptRate;
            return returnValue;
        }

        /// <summary>
        /// Method to set tax exempt exact fuel key
        /// </summary>
        /// <param name="oTeSystem">Tax exempt system</param>
        /// <param name="sCompoundKey">Compound key</param>
        /// <param name="iGradeId">Grade id</param>
        /// <param name="iTierId">Tier id</param>
        /// <param name="iLevelId">Level id</param>
        /// <returns>True or false</returns>
        public bool TeExtractFuelKey(ref teSystem oTeSystem, ref string sCompoundKey, ref short iGradeId, ref short
            iTierId, ref short iLevelId)
        {
            var returnValue = false;
            try
            {
                var sTempError = oTeSystem.SLastError;
                oTeSystem.SLastError = "Failed to extract fuel key from \"" + sCompoundKey + "\"";

                if (!TeIsFuelKey(sCompoundKey))
                {
                    return false;
                }

                var iOldPos = (short)(sCompoundKey.IndexOf(":G:", StringComparison.Ordinal) + 1);
                if (iOldPos == 0)
                {
                    return false;
                }
                iOldPos = (short)(iOldPos + ":G:".Length); //iOldPos now points at 1 past ":G:"

                var iPos = (short)(sCompoundKey.IndexOf(",T:", StringComparison.Ordinal) + 1);
                if (iPos == 0)
                {
                    return false;
                }

                iGradeId = short.Parse(sCompoundKey.Substring(iOldPos - 1, iPos - iOldPos));

                iOldPos = (short)(iPos + ",T:".Length);
                iPos = (short)(sCompoundKey.IndexOf(",L:", iPos + 1 - 1, StringComparison.Ordinal) + 1);
                if (iPos == 0)
                {
                    return false;
                }

                iTierId = short.Parse(sCompoundKey.Substring(iOldPos - 1, iPos - iOldPos));

                iPos = (short)(iPos + ",T:".Length);
                iLevelId = short.Parse(sCompoundKey.Substring(iPos - 1, sCompoundKey.Length + 1 - iPos));

                oTeSystem.SLastError = sTempError;
                returnValue = true;
            }
            catch (Exception ex)
            {
                //ExcludeSE
                oTeSystem.SLastError = oTeSystem.SLastError + " " + ex.HResult + ":" + ex.Message;
            }
            return returnValue;
        }

        /// <summary>
        /// Method to round to high cent
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Rounded value</returns>
        public float RoundToHighCent(float value)
        {
            
            
            var returnValue = (float)(double.Parse((Conversion.Int(value * 100) / 100).ToString("#0.00")) + (Conversion.Int(value * 1000) % 10 != 0 ? 0.01 : 0));
            
            return returnValue;
        }

        /// <summary>
        /// Method to test if limit is required
        /// </summary>
        /// <param name="oTeSystem">Tax exempt system</param>
        /// <param name="productType">Product type</param>
        /// <returns>True or false</returns>
        public bool IsLimitRequired(ref teSystem oTeSystem, mPrivateGlobals.teProductEnum productType)
        {
            bool isError;

            bool returnValue = _teSystemService.IsLimitRequired((int)productType, out isError);
            if (!isError) return returnValue;
            oTeSystem.SLastError = "Error: No Category records found.";
            return false;
        }

        /// <summary>
        /// Method to get cigratte equivalent units
        /// </summary>
        /// <param name="eTobaccoProduct">Tobacco product</param>
        /// <returns>Cigratte equivalent</returns>
        public double GetCigaretteEquivalentUnits(mPrivateGlobals.teTobaccoEnum eTobaccoProduct)
        {
            double returnValue;
            //Smriti move this code to manager
            try
            {
                var product = "";
                if (eTobaccoProduct == mPrivateGlobals.teTobaccoEnum.eTobCigarette)
                {
                    product = "CigaretteEquiv";
                }
                else if (eTobaccoProduct == mPrivateGlobals.teTobaccoEnum.eTobLooseTobacco)
                {
                    product = "TobaccoEquiv";
                }
                else if (eTobaccoProduct == mPrivateGlobals.teTobaccoEnum.eTobCigar)
                {
                    product = "CigarEquiv";
                    //Case Else   ' Other values.
                }
                returnValue = _treatyService.GetCigaretteEquivalentUnits(product);
            }
            catch (Exception ex)
            {
                //ExcludeSE
                returnValue = -1;
            }
            return returnValue;
        }

        /// <summary>
        /// Method to make tax exempt fuel key
        /// </summary>
        /// <param name="iGradeId">Grade id</param>
        /// <param name="iTierId">Tier id</param>
        /// <param name="iLevelId">Level id</param>
        /// <returns>Fuel key</returns>
        public string TeMakeFuelKey(short iGradeId, short iTierId, short iLevelId)
        {
            var returnValue = "Fuel:G:" + iGradeId + ",T:" + iTierId + ",L:" + iLevelId;

            return returnValue;
        }

        /// <summary>
        /// Method to get tax exempt limit
        /// </summary>
        /// <param name="eLimit">tax exempt limit</param>
        /// <param name="dOutLimit">Limit</param>
        /// <returns>True or false</returns>
        public bool TeGetLimit(mPrivateGlobals.teLimitEnum eLimit, ref double dOutLimit)
        {
            //On Error Goto ErrHandler VBConversions Warning: could not be converted to try/catch - logic too complex
            string adminName = string.Empty;
            switch (eLimit)
            {
                case mPrivateGlobals.teLimitEnum.eCigLimit:
                case mPrivateGlobals.teLimitEnum.eTobaccoLimit:
                case mPrivateGlobals.teLimitEnum.eCigarLimit:
                    adminName = "CigLimit";
                    break;
                case mPrivateGlobals.teLimitEnum.eCigMaxThreshhold:
                case mPrivateGlobals.teLimitEnum.eTobaccoMaxThreshhold:
                case mPrivateGlobals.teLimitEnum.eCigarMaxThreshhold:
                    adminName = "CigMaxThreshold";
                    break;
                case mPrivateGlobals.teLimitEnum.eGasLimit:
                case mPrivateGlobals.teLimitEnum.eDieselLimit:
                    adminName = "GasLimit";
                    break;
                case mPrivateGlobals.teLimitEnum.ePropaneLimit:
                    adminName = _policyManager.TE_Type == "SITE" ? "GasLimit" : "PropaneLimit";
                    break;
                case mPrivateGlobals.teLimitEnum.eGasTransactionLimit:
                case mPrivateGlobals.teLimitEnum.eDieselTransactionLimit:
                    adminName = "GasTransactionLimit";
                    break;
                case mPrivateGlobals.teLimitEnum.ePropaneTransactionLimit:
                    adminName = _policyManager.TE_Type == "SITE" ? "GasTransactionLimit" : "PropaneTransactionLimit";
                    break;
            }

            dOutLimit = _teSystemService.GetAdminValue(adminName) == null ? 0 : Convert.ToDouble(_teSystemService.GetAdminValue(adminName));

            
            if (_policyManager.TE_Type != "SITE" || _policyManager.TAX_EXEMPT_FNGTR) return true;
            switch (eLimit)
            {
                case mPrivateGlobals.teLimitEnum.eTobaccoLimit:
                case mPrivateGlobals.teLimitEnum.eTobaccoMaxThreshhold:
                    dOutLimit = dOutLimit * GetCigaretteEquivalentUnits(mPrivateGlobals.teTobaccoEnum.eTobLooseTobacco);
                    break;
                case mPrivateGlobals.teLimitEnum.eCigarLimit:
                case mPrivateGlobals.teLimitEnum.eCigarMaxThreshhold:
                    dOutLimit = dOutLimit * GetCigaretteEquivalentUnits(mPrivateGlobals.teTobaccoEnum.eTobCigar);
                    break;
            }
            return true;
        }

        /// <summary>
        /// Method to get all tax exempt system reasons
        /// </summary>
        /// <param name="oTeSystem">Tax exempt system</param>
        public void GetAllReasons(ref teSystem oTeSystem)
        {
            var gasReasons = _reasonService.GetTaxExemptReasons("FUEL");
            oTeSystem.GasReasons = new TaxExemptReasons();
            oTeSystem.PropaneReasons = new TaxExemptReasons();
            oTeSystem.TobaccoReasons = new TaxExemptReasons();

            foreach (var taxExemptReason in gasReasons)
            {
                oTeSystem.GasReasons.AddReason(taxExemptReason, "");
            }


            var tobaccoReasons = _reasonService.GetTaxExemptReasons("TOBACCO");
            foreach (var taxExemptReason in tobaccoReasons)
            {
                oTeSystem.TobaccoReasons.AddReason(taxExemptReason, "");
            }


            var propaneReasons = _reasonService.GetTaxExemptReasons("PROPANE");
            foreach (var taxExemptReason in propaneReasons)
            {
                oTeSystem.PropaneReasons.AddReason(taxExemptReason, "");
            }
        }

        /// <summary>
        /// Method to get all limits
        /// </summary>
        /// <param name="oTeSystem">Tax exempt system</param>
        public void GetAllLimits(ref teSystem oTeSystem)
        {
            oTeSystem.GasLimit = _teSystemService.GetAdminValue("GasLimit") == null ? 0 : Convert.ToSingle(_teSystemService.GetAdminValue("GasLimit"));

            oTeSystem.PropaneLimit = _teSystemService.GetAdminValue("PropaneLimit") == null ? 0 : Convert.ToSingle(_teSystemService.GetAdminValue("PropaneLimit"));

            oTeSystem.TobaccoLimit = _teSystemService.GetAdminValue("CigLimit") == null ? 0 : Convert.ToSingle(_teSystemService.GetAdminValue("CigLimit"));

            oTeSystem.Retailer = _teSystemService.GetAdminValue("TaxExemptRetailer") == null ? "" : Convert.ToString(_teSystemService.GetAdminValue("TaxExemptRetailer"));

            oTeSystem.TaxCertifyCode = _teSystemService.GetAdminValue("TaxExemptCertifyCode") == null ? "" : Convert.ToString(_teSystemService.GetAdminValue("TaxExemptCertifyCode"));

            oTeSystem.TaxProgram = _teSystemService.GetAdminValue("TaxProgram") == null ? "" : Convert.ToString(_teSystemService.GetAdminValue("TaxProgram"));

            oTeSystem.VoucherFooter = _teSystemService.GetReportValues("TAXEXEMPT") == null ? "" : Convert.ToString(_teSystemService.GetReportValues("TAXEXEMPT"));

        }

        /// <summary>
        /// Method to get all override codes
        /// </summary>
        /// <param name="teSystem">Tax exempt system</param>
        /// <param name="arOverrideCodes">List of override codes</param>
        /// <returns>True or false</returns>
        public bool TeGetAllOverrideCodes(ref teSystem teSystem, ref List<OverrideCode> arOverrideCodes)
        {
            bool isError;

            arOverrideCodes = _teSystemService.GetOverrideCodes(out isError);
            if (!isError) return true;
            teSystem.SLastError = "Error: No override records found.";
            return false;
        }

        /// <summary>
        /// Method to get tax free tier level price difference
        /// </summary>
        /// <param name="tier">Tier id</param>
        /// <param name="level">Level id</param>
        /// <param name="cashPriceIncre">Cash price increment</param>
        /// <param name="creditPriceIncre">Credit price increment</param>
        /// <param name="bFound">Value found</param>
        /// <returns>True or false</returns>
        public bool TeGetTaxFreeTierLevelPriceDiff(short tier, short level, ref double cashPriceIncre, ref double creditPriceIncre, ref bool bFound)
        {
            bFound = false;

            var oRecs = _fuelPumpService.get_TierLevelPriceDiff((byte)tier, (byte)level);

            //eCategory and bFound are already set.
            if (oRecs != null)
            {
                cashPriceIncre = CommonUtility.GetDoubleValue((oRecs.TaxExemptCashDiff));
                creditPriceIncre = CommonUtility.GetDoubleValue(oRecs.TaxExemptCreditDiff);
                bFound = true;
            }
            else
            {
                return false;
            }

            //There was no error, so set to true whether a taxfree price was found or not
            return true;
        }

        /// <summary>
        /// Method to get tax free fuel price
        /// </summary>
        /// <param name="sProductKey">Product key</param>
        /// <param name="cashPrice">Cash price</param>
        /// <param name="creditPrice">Credit price</param>
        /// <param name="bFound">Value found or not</param>
        /// <returns>True or false</returns>
        public bool TeGetTaxFreeFuelPrice(ref string sProductKey, ref double cashPrice,
            ref double creditPrice, ref bool bFound)
        {
            
            if (_policyManager.TE_ByRate && _policyManager.TE_Type != "SITE") //sghiny dec15, 2009
            {
                if (TeGetTaxFreeFuelPriceByRate(ref sProductKey, ref cashPrice, ref
                    creditPrice, ref bFound))
                {
                    return true;
                }
            }
            else
            {
                if (TeGetTaxFreeFuelPriceByPrice(ref sProductKey, ref cashPrice, ref
                    creditPrice, ref bFound))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Method to get tax free grade price increment
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <param name="cashPriceIncre">Cash price increment</param>
        /// <param name="creditPriceIncre">Credit price increment</param>
        /// <param name="bFound">Value found</param>
        /// <returns>True or false</returns>
        public bool TeGetTaxFreeGradePriceIncrement(short gradeId, ref double cashPriceIncre, ref double creditPriceIncre, ref bool bFound)
        {
            bFound = false;

            var gradePriceIncrement = _fuelPumpService.get_GradePriceIncrement(CommonUtility.GetByteValue(gradeId));

            //eCategory and bFound are already set.
            if (gradePriceIncrement != null)
            {
                cashPriceIncre = CommonUtility.GetDoubleValue(gradePriceIncrement.TaxExemptCashPriceIncre);
                creditPriceIncre = CommonUtility.GetDoubleValue(gradePriceIncrement.TaxExemptCreditPriceIncre);
                bFound = true;
            }
            else
            {
                return false;
            }

            //There was no error, so set to true whether a taxfree price was found or not

            return true;
        }

        /// <summary>
        /// Method to get category
        /// </summary>
        /// <param name="sProductKey">Product key</param>
        /// <param name="eOutCategory">Tax exempt category</param>
        /// <returns>True or false</returns>
        public bool TeGetCategory(string sProductKey, ref mPrivateGlobals.teProductEnum eOutCategory)
        {
            eOutCategory = mPrivateGlobals.teProductEnum.eNone;

            var oRecs = _stockService.GetProductTaxExempt(sProductKey);

            //eCategory is already set.
            if (oRecs != null)
            {
                if (Information.IsDBNull(oRecs.CategoryFK))
                {
                    
                    return false;
                }
                if (oRecs.CategoryFK != null) eOutCategory = (mPrivateGlobals.teProductEnum)oRecs.CategoryFK;
            }
            else
            {
                return false;
            }

            //There was no error, so set to true whether a taxfree price was found or not
            return true;
        }

        /// <summary>
        /// Method to set tax free grade price increment
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <param name="cashPriceIncre">Cash price increment</param>
        /// <param name="creditPriceIncre">Credit price increment</param>
        /// <returns>True or false</returns>
        public bool TeSetTaxFreeGradePriceIncrement(short gradeId, double cashPriceIncre, double creditPriceIncre)
        {
            var oRecs = _fuelPumpService.get_GradePriceIncrement((byte)gradeId);

            if (oRecs == null)
            {
                //Insert a record into the table for this fuel product.
                _fuelPumpService.set_GradePriceIncrement((byte)gradeId, new CGradePriceIncrement
                {
                    TaxExemptCashPriceIncre = cashPriceIncre,
                    TaxExemptCreditPriceIncre = creditPriceIncre
                });
            }
            else
            {
                //Update the existing record with the new price.
                _fuelPumpService.set_GradePriceIncrement((byte)gradeId, new CGradePriceIncrement
                {
                    TaxExemptCashPriceIncre = cashPriceIncre,
                    TaxExemptCreditPriceIncre = creditPriceIncre,
                    CashPriceIncre = oRecs.CashPriceIncre,
                    CreditPriceIncre = oRecs.CreditPriceIncre
                });
            }


            

            //There was no error, so set to true whether a taxfree price was found or not

            return true;
        }

        /// <summary>
        /// Method to set tax free fuel price
        /// </summary>
        /// <param name="sProductKey">Product key</param>
        /// <param name="eProductCategory">Product category</param>
        /// <param name="dNewCashPrice">New cash price</param>
        /// <param name="dNewCreditPrice">New credit price</param>
        /// <param name="sEmpId">Emp id</param>
        /// <returns>True or false</returns>
        public bool SetTaxFreeFuelPrice(ref string sProductKey, mPrivateGlobals.teProductEnum eProductCategory, double dNewCashPrice, double dNewCreditPrice, string sEmpId)
        {
            //Check if a record exists....
            //if it does, then update the record
            //if it does not then insert a new record.
            short iTier = 0;
            short iGrade = 0;
            short iLevel = 0;

            if (!TeExtractFuelKey(ref sProductKey, ref iGrade, ref iTier, ref iLevel))
            {
                return false;
            }

            var oRecs = _fuelPumpService.GetFuelPrice(iGrade, iTier, iLevel);

            if (oRecs == null)
            {
                //Insert a record into the table for this fuel product.
                _fuelPumpService.set_FuelPrice(ref Variables.gPumps, (byte)iGrade, (byte)iTier, (byte)iLevel, new CFuelPrice
                {
                    teCashPrice = CommonUtility.GetFloatValue(dNewCashPrice),
                    teCreditPrice = CommonUtility.GetFloatValue(dNewCreditPrice),
                    EmplID = "\'" + sEmpId + "\'",
                    Date_Time = (int)(CommonUtility.GetDateTimeValue(IsoFormat(DateAndTime.Today, DateAndTime.TimeOfDay)).ToOADate())
                });
            }
            else
            {
                //Update the existing record with the new price.
                var sSql = "UPDATE FuelPrice " + "SET TaxFreePrice=" + System.Convert.ToString(dNewCashPrice) + ", TaxFreeCreditPrice=" + System.Convert.ToString(dNewCreditPrice) + ", EmplID=\'" + sEmpId + "\', Date_Time=\'" + IsoFormat(DateAndTime.Today, DateAndTime.TimeOfDay) + "\'   WHERE GradeID=" + (iGrade).ToString() + " AND TierID=" + (iTier).ToString() + " AND LevelID=" + (iLevel).ToString();
                _fuelPumpService.set_FuelPrice(ref Variables.gPumps, (byte)iGrade, (byte)iTier, (byte)iLevel, new CFuelPrice
                {
                    teCashPrice = CommonUtility.GetFloatValue(dNewCashPrice),
                    teCreditPrice = CommonUtility.GetFloatValue(dNewCreditPrice),
                    EmplID = "\'" + sEmpId + "\'",
                    Date_Time = (int)(CommonUtility.GetDateTimeValue(IsoFormat(DateAndTime.Today, DateAndTime.TimeOfDay)).ToOADate()),
                    CashPrice = CommonUtility.GetFloatValue(oRecs.CashPrice),
                    CreditPrice = CommonUtility.GetFloatValue(oRecs.CreditPrice)
                });
            }

            return true;
        }

        /// <summary>
        /// Method to set tax free tier level price difference
        /// </summary>
        /// <param name="tier">Tier id</param>
        /// <param name="level">Level id</param>
        /// <param name="cashPriceIncre">Cash price increment</param>
        /// <param name="creditPriceIncre">Credit price increment</param>
        /// <returns>True or false</returns>
        public bool TeSetTaxFreeTierLevelPriceDiff(short tier, short level, double cashPriceIncre, double
            creditPriceIncre)
        {
            var oRecs = _fuelPumpService.get_TierLevelPriceDiff((byte)tier, (byte)level);

            if (oRecs == null)
            {
                //Insert a record into the table for this fuel product.
                _fuelPumpService.set_TierLevelPriceDiff((byte)tier, (byte)level, new CTierLevelPriceDiff
                {
                    TaxExemptCashDiff = cashPriceIncre,
                    TaxExemptCreditDiff = creditPriceIncre
                });
            }
            else
            {
                //Update the existing record with the new price.
                _fuelPumpService.set_TierLevelPriceDiff((byte)tier, (byte)level, new CTierLevelPriceDiff
                {
                    TaxExemptCashDiff = cashPriceIncre,
                    TaxExemptCreditDiff = creditPriceIncre,
                    CashDiff = oRecs.CashDiff,
                    CreditDiff = oRecs.CreditDiff
                });
            }
            //There was no error, so set to true whether a taxfree price was found or not
            return true;
        }

        #region Private methods

        /// <summary>
        /// Method to get ISO format
        /// </summary>
        /// <param name="vDate">Date</param>
        /// <param name="vTime">Time</param>
        /// <returns>Iso format</returns>
        private string IsoFormat(DateTime vDate, DateTime vTime)
        {
            var sResult = vDate.ToString("yyyy-MM-dd");


            sResult = sResult + " " + vTime.ToString("hh:mm:ss");

            var returnValue = sResult;
            return returnValue;
        }

        /// <summary>
        /// Method to get tax free fuel price by rate
        /// </summary>
        /// <param name="sProductKey">Product key</param>
        /// <param name="cashPrice">Cash price</param>
        /// <param name="creditPrice">Credit price</param>
        /// <param name="bFound">Value found</param>
        /// <returns>True or false</returns>
        private bool TeGetTaxFreeFuelPriceByRate(ref string sProductKey, ref double
            cashPrice, ref double creditPrice, ref bool bFound)
        {
            short taxExemptTaxCode = 0;
            float regCashPrice = 0;
            float regCreditPrice = 0;

            bFound = false;

            if (!TeIsFuelKey(sProductKey))
            {
                return false;
            }

            short iTier = 0;
            short iGrade = 0;
            short iLevel = 0;

            if (!TeExtractFuelKey(ref sProductKey, ref iGrade, ref iTier, ref iLevel))
            {
                return false;
            }

            string strStockCode = Convert.ToString(Variables.gPumps.get_Grade((byte)iGrade).Stock_Code);

            var flag = _fuelPumpService.GetFuelPriceByRate(strStockCode, iGrade, iTier, iLevel, ref regCashPrice, ref regCreditPrice, ref taxExemptTaxCode);
            
            //             "FROM FuelPrice " & _
            //             "WHERE GradeID=" & CStr(iGrade) & _
            //             " AND TierID=" & CStr(iTier) & _
            //             " AND LevelID=" & CStr(iLevel)
            

            //eCategory and bFound are already set.
            if (flag)
            {
                float taxRate;
                string rateType;
                var rs = _taxService.GetTaxExemptRate(taxExemptTaxCode.ToString(), out taxRate, out rateType);
                if (!rs)
                {
                    cashPrice = regCashPrice;
                    creditPrice = regCreditPrice;
                }
                else
                {
                    var newTaxRate = taxRate;
                    var newRateType = rateType;
                    if (newRateType == "$")
                    {
                        cashPrice = double.Parse((regCashPrice - newTaxRate).ToString("#0.000"));
                        creditPrice = double.Parse((regCreditPrice - newTaxRate).ToString("#0.000"));
                    }
                    else if (newRateType == "%")
                    {
                        cashPrice = double.Parse((regCashPrice * (1 - newTaxRate / 100)).ToString("#0.000"));
                        creditPrice = double.Parse((regCreditPrice * (1 - newTaxRate / 100)).ToString("#0.000"));
                    }
                    else
                    {
                        cashPrice = regCashPrice;
                        creditPrice = regCreditPrice;
                    }
                }

                bFound = true;
            }
            else
            {
                return false;
            }

            //There was no error, so set to true whether a taxfree price was found or not

            return true;
        }

        /// <summary>
        /// Method to get tax free fuel price by price
        /// </summary>
        /// <param name="sProductKey">Product key</param>
        /// <param name="cashPrice">Cash price</param>
        /// <param name="creditPrice">Credit price</param>
        /// <param name="bFound">Value found</param>
        /// <returns>True or false</returns>
        private bool TeGetTaxFreeFuelPriceByPrice(ref string sProductKey, ref double cashPrice, ref double
            creditPrice, ref bool bFound)
        {
            
            bFound = false;

            short iTier = 0;
            short iGrade = 0;
            short iLevel = 0;
            if (TeIsFuelKey(sProductKey))
            {

                if (!TeExtractFuelKey(ref sProductKey, ref iGrade, ref iTier, ref iLevel))
                {
                    return false;
                }
            }

            var oRecs = _fuelPumpService.GetFuelPrice(iGrade, iTier, iLevel);

            //eCategory and bFound are already set.
            if (oRecs != null)
            {
                cashPrice = Convert.ToDouble(oRecs.TaxExemptedCashPrice);
                creditPrice = Convert.ToDouble(oRecs.TaxExemptedCreditPrice);
                bFound = true;
            }
            else
            {
                return false;
            }

            //There was no error, so set to true whether a taxfree price was found or not

            return true;
        }

        /// <summary>
        /// Method to validate if key id tax exempt fuel key
        /// </summary>
        /// <param name="sProductKey">Product key</param>
        /// <returns>Product key</returns>
        private bool TeIsFuelKey(string sProductKey)
        {
            short I;

            var iPos = (short)(sProductKey.IndexOf("Fuel:G:", StringComparison.Ordinal) + 1);
            if (iPos != 1)
            {
                return false;
            }

            for (I = 1; I <= 4; I++) //Look for at least 4 colons
            {
                iPos = (short)(sProductKey.IndexOf(":", iPos + 1 - 1, StringComparison.Ordinal) + 1);
                if (iPos == 0)
                {
                    return false; //if colon not found then return false
                }
            }

            return true;
        }

        
        /// <summary>
        /// Method to get tax free price by rate
        /// </summary>
        /// <param name="sProductKey">Product key</param>
        /// <param name="originalPrice">Original price</param>
        /// <param name="dOutPrice">Price</param>
        /// <param name="dOutUnitsPerPkg">Units per kg</param>
        /// <param name="eOutCategory">Category</param>
        /// <param name="sOutUpcCode">UPC code</param>
        /// <param name="bFound">Value found</param>
        /// <param name="outTaxExemptRate">Tax exempt rate</param>
        /// <param name="stockcode">Stock code</param>
        /// <param name="productCode">Product code</param>
        /// <returns>True or false</returns>
        private bool TeGetTaxFreePriceByRate(ref string sProductKey, float originalPrice, ref float dOutPrice,
            ref float dOutUnitsPerPkg, ref mPrivateGlobals.teProductEnum eOutCategory, ref string sOutUpcCode, ref bool bFound, ref float outTaxExemptRate, string stockcode, ref string productCode) 
        {
            //Nancy added for calculating tax for cigar

            bFound = false;
            eOutCategory = mPrivateGlobals.teProductEnum.eNone;
            sOutUpcCode = "";
            dOutUnitsPerPkg = 0;

            var regPrice = originalPrice;


            var blForFuel = TeIsFuelKey(sProductKey);
            short quantityPerPkg = 1;
            short baseUnitQty = 1;
            bool isError;
            short taxExemptTaxCode = 0;
            _taxService.TeGetTaxFreePriceByRate(blForFuel, ref sProductKey, stockcode, ref quantityPerPkg, ref baseUnitQty,
                ref taxExemptTaxCode, ref productCode, ref eOutCategory, ref dOutUnitsPerPkg, ref sOutUpcCode, out isError);

            if (isError)
            {
                return false;
            }

            if (quantityPerPkg == 0)
            {
                quantityPerPkg = 1;
            }
            if (baseUnitQty == 0)
            {
                baseUnitQty = 1;
            }

            float taxRate;
            string rateType;

            var issuccess = _taxService.GetTaxExemptRate(Convert.ToString(taxExemptTaxCode), out taxRate, out rateType);

            if (!issuccess)
            {
                dOutPrice = regPrice;
            }
            else
            {
                if (rateType == "$")
                {

                    outTaxExemptRate = taxRate;

                    if (eOutCategory == mPrivateGlobals.teProductEnum.eCigar)
                    {
                        taxRate = RoundToHighCent(quantityPerPkg * taxRate * baseUnitQty);
                        dOutPrice = regPrice - taxRate;
                        outTaxExemptRate = taxRate;
                    }
                    else
                    {
                        dOutPrice = regPrice - taxRate;
                    }
                }
                else if (rateType == "%")
                {
                    if (eOutCategory == mPrivateGlobals.teProductEnum.eCigar)
                    {
                        taxRate = RoundToHighCent(regPrice / quantityPerPkg / baseUnitQty * (taxRate / 100));
                        outTaxExemptRate = float.Parse((taxRate * quantityPerPkg * baseUnitQty).ToString("#0.00"));
                        dOutPrice = regPrice - outTaxExemptRate;
                    }
                    else
                    {
                        outTaxExemptRate = RoundToHighCent(regPrice * (taxRate / 100));
                        dOutPrice = regPrice - outTaxExemptRate;
                    }
                }
                else
                {
                    dOutPrice = regPrice;
                    outTaxExemptRate = 0; 
                }
                if (blForFuel)
                {
                    dOutPrice = float.Parse(dOutPrice.ToString("#0.000"));
                }
                else
                {
                    var noDecimals = (short)((dOutPrice - Conversion.Int((short)dOutPrice)).ToString(CultureInfo.InvariantCulture).Length - 2);
                    if (noDecimals > 4)
                    {
                        noDecimals = 4;
                    }
                    if (noDecimals < 0)
                    {
                        noDecimals = 0;
                    }
                    dOutPrice = float.Parse(dOutPrice.ToString("#0." + new string('0', noDecimals)));
                }
            }

            bFound = true;
            return true;
        }


        /// <summary>
        /// Method to get tax free price by price
        /// </summary>
        /// <param name="oTeSystem">Tax exempt system</param>
        /// <param name="sProductKey">Product key</param>
        /// <param name="originalPrice">Original price</param>
        /// <param name="dOutPrice">Price</param>
        /// <param name="dOutUnitsPerPkg">Units per kg</param>
        /// <param name="eOutCategory">Category</param>
        /// <param name="sOutUpcCode">UPC code</param>
        /// <param name="bFound">Value found</param>
        /// <param name="stockcode">Stock code</param>
        /// <returns>True or false</returns>
        private bool TeGetTaxFreePriceByPrice(ref teSystem oTeSystem, ref string sProductKey, double originalPrice
            , ref float dOutPrice, ref float dOutUnitsPerPkg, ref mPrivateGlobals.teProductEnum eOutCategory,
            ref string sOutUpcCode, ref bool bFound, string stockcode)
        {
            bFound = false;
            eOutCategory = mPrivateGlobals.teProductEnum.eNone;
            sOutUpcCode = "";
            dOutUnitsPerPkg = 0;

            string sSql;

            short iTier = 0;
            short iGrade = 0;
            short iLevel = 0;
            if (TeIsFuelKey(sProductKey))
            {

                if (!TeExtractFuelKey(ref oTeSystem, ref sProductKey, ref iGrade, ref iTier, ref iLevel))
                {
                    return false;
                }

                sSql = "SELECT TaxFreePrice, " + "(SELECT CategoryFK From CSCMaster.dbo.ProductTaxExempt Where" + " ProductKey =\'" + stockcode + "\') As CategoryFK," + "1 As UnitsPerPkg, " + "\'\' As UpcCode  FROM FuelPrice  WHERE GradeID=" + iGrade + " AND TierID=" + iTier + " AND LevelID=" + iLevel;
            }
            else
            {
                sSql = "SELECT TaxFreePrice, CategoryFK, UnitsPerPkg, UpcCode FROM CSCMaster.dbo.ProductTaxExempt  WHERE ProductKey=\'" + sProductKey + "\'";
            }

            bool isError;
            var success = _taxService.TeGetTaxFreePriceByPrice(sSql, ref dOutPrice, originalPrice, ref eOutCategory,
                 ref dOutUnitsPerPkg, ref sOutUpcCode, out bFound, out isError);

            if (!isError) return success;
            oTeSystem.SLastError = "2";
            return false;
        }

        /// <summary>
        /// Method to get exact fuel key
        /// </summary>
        /// <param name="sCompoundKey">Compound key</param>
        /// <param name="iGradeId">Grade id</param>
        /// <param name="iTierId">Tier id</param>
        /// <param name="iLevelId">Level id</param>
        /// <returns>True or false</returns>
        private bool TeExtractFuelKey(ref string sCompoundKey, ref short iGradeId, ref short iTierId, ref short
            iLevelId)
        {
            var returnValue = false;
            var sLastError = string.Empty;
            try
            {
                var sTempError = sLastError;
                sLastError = "Failed to extract fuel key from \"" + sCompoundKey + "\"";

                if (!TeIsFuelKey(sCompoundKey))
                {
                    return false;
                }

                var iOldPos = (short)(sCompoundKey.IndexOf(":G:") + 1);
                if (iOldPos == 0)
                {
                    return false;
                }
                iOldPos = (short)(iOldPos + ":G:".Length); //iOldPos now points at 1 past ":G:"

                var iPos = (short)(sCompoundKey.IndexOf(",T:") + 1);
                if (iPos == 0)
                {
                    return false;
                }

                iGradeId = short.Parse(sCompoundKey.Substring(iOldPos - 1, iPos - iOldPos));

                iOldPos = (short)(iPos + ",T:".Length);
                iPos = (short)(sCompoundKey.IndexOf(",L:", iPos + 1 - 1) + 1);
                if (iPos == 0)
                {
                    return false;
                }

                iTierId = short.Parse(sCompoundKey.Substring(iOldPos - 1, iPos - iOldPos));

                iPos = (short)(iPos + ",T:".Length);
                iLevelId = short.Parse(sCompoundKey.Substring(iPos - 1, (sCompoundKey.Length + 1) - iPos));

                sLastError = sTempError;
                returnValue = true;
            }
            catch (Exception ex)
            {
                //ExcludeSE
                sLastError = sLastError + " " + ex.HResult.ToString() + ":" + ex.Message;
            }
            return returnValue;
        }


        #endregion
    }
}

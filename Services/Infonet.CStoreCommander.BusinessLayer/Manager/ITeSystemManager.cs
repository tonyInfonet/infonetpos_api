using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ITeSystemManager
    {
        /// <summary>
        /// Method to round to high cent
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Rounded value</returns>
        float RoundToHighCent(float value);


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
        bool TeGetTaxFreePrice(ref teSystem oTeSystem, ref tePurchaseItem purchaseItem, string sProductKey,
            float originalPrice, bool bFound, string stockcode, string productCode, float outTaxExemptRate);

        /// <summary>
        /// Method to get tax free price
        /// </summary>
        /// <param name="oTeSystem">Tax exempt system</param>
        /// <param name="saleLine">Sale line</param>
        /// <param name="bFound">Value found</param>
        /// <returns>True or falses</returns>
        bool TeGetTaxFreePrice(ref teSystem oTeSystem, ref TaxExemptSaleLine saleLine, ref bool bFound);


        /// <summary>
        /// Method to test if limit is required
        /// </summary>
        /// <param name="oTeSystem">Tax exempt system</param>
        /// <param name="productType">Product type</param>
        /// <returns>True or false</returns>
        bool IsLimitRequired(ref teSystem oTeSystem, mPrivateGlobals.teProductEnum productType);

        /// <summary>
        /// Method to get cigratte equivalent units
        /// </summary>
        /// <param name="eTobaccoProduct">Tobacco product</param>
        /// <returns>Cigratte equivalent</returns>
        double GetCigaretteEquivalentUnits(mPrivateGlobals.teTobaccoEnum eTobaccoProduct);

        /// <summary>
        /// Method to make tax exempt fuel key
        /// </summary>
        /// <param name="iGradeId">Grade id</param>
        /// <param name="iTierId">Tier id</param>
        /// <param name="iLevelId">Level id</param>
        /// <returns>Fuel key</returns>
        string TeMakeFuelKey(short iGradeId, short iTierId, short iLevelId);

        /// <summary>
        /// Method to get tax exempt limit
        /// </summary>
        /// <param name="eLimit">tax exempt limit</param>
        /// <param name="dOutLimit">Limit</param>
        /// <returns>True or false</returns>
        bool TeGetLimit(mPrivateGlobals.teLimitEnum eLimit, ref double dOutLimit);

        /// <summary>
        /// Method to get all tax exempt system reasons
        /// </summary>
        /// <param name="oTeSystem">Tax exempt system</param>
        void GetAllReasons(ref teSystem oTeSystem);

        /// <summary>
        /// Method to get all limits
        /// </summary>
        /// <param name="oTeSystem">Tax exempt system</param>
        void GetAllLimits(ref teSystem oTeSystem);

        /// <summary>
        /// Method to get all override codes
        /// </summary>
        /// <param name="teSystem">Tax exempt system</param>
        /// <param name="arOverrideCodes">List of override codes</param>
        /// <returns>True or false</returns>
        bool TeGetAllOverrideCodes(ref teSystem teSystem, ref List<OverrideCode> arOverrideCodes);

        /// <summary>
        /// Method to get tax free tier level price difference
        /// </summary>
        /// <param name="tier">Tier id</param>
        /// <param name="level">Level id</param>
        /// <param name="cashPriceIncre">Cash price increment</param>
        /// <param name="creditPriceIncre">Credit price increment</param>
        /// <param name="bFound">Value found</param>
        /// <returns>True or false</returns>
        bool TeGetTaxFreeTierLevelPriceDiff(short tier, short level, ref double cashPriceIncre, ref double
            creditPriceIncre, ref bool bFound);

        /// <summary>
        /// Method to get tax free fuel price
        /// </summary>
        /// <param name="sProductKey">Product key</param>
        /// <param name="cashPrice">Cash price</param>
        /// <param name="creditPrice">Credit price</param>
        /// <param name="bFound">Value found or not</param>
        /// <returns>True or false</returns>
        bool TeGetTaxFreeFuelPrice(ref string sProductKey, ref double cashPrice, ref
            double creditPrice, ref bool bFound);

        /// <summary>
        /// Method to get tax free grade price increment
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <param name="cashPriceIncre">Cash price increment</param>
        /// <param name="creditPriceIncre">Credit price increment</param>
        /// <param name="bFound">Value found</param>
        /// <returns>True or false</returns>
        bool TeGetTaxFreeGradePriceIncrement(short gradeId, ref double cashPriceIncre,
            ref double creditPriceIncre, ref bool bFound);

        /// <summary>
        /// Method to get category
        /// </summary>
        /// <param name="sProductKey">Product key</param>
        /// <param name="eOutCategory">Tax exempt category</param>
        /// <returns>True or false</returns>
        bool TeGetCategory(string sProductKey, ref mPrivateGlobals.teProductEnum
            eOutCategory);

        /// <summary>
        /// Method to set tax free grade price increment
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <param name="cashPriceIncre">Cash price increment</param>
        /// <param name="creditPriceIncre">Credit price increment</param>
        /// <returns>True or false</returns>
        bool TeSetTaxFreeGradePriceIncrement(short gradeId, double cashPriceIncre, double
            creditPriceIncre);

        /// <summary>
        /// Method to set tax free fuel price
        /// </summary>
        /// <param name="sProductKey">Product key</param>
        /// <param name="eProductCategory">Product category</param>
        /// <param name="dNewCashPrice">New cash price</param>
        /// <param name="dNewCreditPrice">New credit price</param>
        /// <param name="sEmpId">Emp id</param>
        /// <returns>True or false</returns>
        bool SetTaxFreeFuelPrice(ref string sProductKey, mPrivateGlobals.teProductEnum eProductCategory, double
            dNewCashPrice, double dNewCreditPrice, string sEmpId);

        /// <summary>
        /// Method to set tax free tier level price difference
        /// </summary>
        /// <param name="tier">Tier id</param>
        /// <param name="level">Level id</param>
        /// <param name="cashPriceIncre">Cash price increment</param>
        /// <param name="creditPriceIncre">Credit price increment</param>
        /// <returns>True or false</returns>
         bool TeSetTaxFreeTierLevelPriceDiff(short tier, short level, double cashPriceIncre, double
            creditPriceIncre);

        /// <summary>
        /// Method to set tax exempt exact fuel key
        /// </summary>
        /// <param name="oTeSystem">Tax exempt system</param>
        /// <param name="sCompoundKey">Compound key</param>
        /// <param name="iGradeId">Grade id</param>
        /// <param name="iTierId">Tier id</param>
        /// <param name="iLevelId">Level id</param>
        /// <returns>True or false</returns>
        bool TeExtractFuelKey(ref teSystem oTeSystem, ref string sCompoundKey, ref short iGradeId, ref short
            iTierId, ref short iLevelId);

    }
}
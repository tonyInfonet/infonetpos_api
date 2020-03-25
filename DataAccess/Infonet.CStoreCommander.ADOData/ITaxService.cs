using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public interface ITaxService
    {

        /// <summary>
        /// Set up taxes
        /// </summary>
        /// <param name="stockCode"></param>
        /// <param name="taxNames"></param>
        /// <param name="selectedTaxes"></param>
        /// <returns></returns>
        bool SetupTaxes(string stockCode, List<string> taxNames, List<string> selectedTaxes);

        /// <summary>
        /// Delete stock taxes
        /// </summary>
        /// <param name="stockCode"></param>
        /// <returns></returns>
        bool DeleteStockTax(string stockCode);

        /// <summary>
        /// Get all active taxes
        /// </summary>
        /// <returns></returns>
        List<StockTax> GetAllActiveTaxes();

        /// <summary>
        /// Method to get all tax mast
        /// </summary>
        /// <returns>List of tax mast</returns>
        List<TaxMast> GetTaxMast();

        /// <summary>
        /// Method to get test rate by tax name
        /// </summary>
        /// <param name="taxName">Tax name</param>
        /// <returns>List of tax rate</returns>
        List<TaxRate> GetTaxRatesByName(string taxName);

        /// <summary>
        /// Method to get purchase items
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Purchase items</returns>
        List<tePurchaseItem> GetPurchaseItems(int saleNumber, int tillNumber);

        /// <summary>
        /// Method to delete purchase item
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">TIll number</param>
        void DeletePurchaseItem(int saleNumber, int tillNumber);

        /// <summary>
        /// Method to get tax exempt sale line
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Tax exempt sale lines</returns>
        List<TaxExemptSaleLine> GetTaxExemptSaleLines(int saleNumber, int tillNumber);

        /// <summary>
        /// Update tax exempt card holder
        /// </summary>
        /// <param name="taxExemptCardHolder">tax exempt card holder</param>
        void UpdateTaxExemptCardHolder(teCardholder taxExemptCardHolder);

        /// <summary>
        /// Method to get tax exempt card holder
        /// </summary>
        /// <param name="cardHolderId">card holder id</param>
        /// <returns>Tax exempt card holder</returns>
        teCardholder GetTaxExemptCardHolder(string cardHolderId);

        /// <summary>
        /// Method to clean tax exempt trash
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">TIll number</param>
        void CleanTaxExemptData(int saleNumber, int tillNumber);

        /// <summary>
        /// Method to get tax exempt rate
        /// </summary>
        /// <param name="taxExemptTaxCode">Tax exempt tax code</param>
        /// <param name="taxRate">Tax rate</param>
        /// <param name="rateType">Rate type</param>
        /// <returns>True or false</returns>
        bool GetTaxExemptRate(string taxExemptTaxCode, out float taxRate, out string rateType);

        /// <summary>
        /// Method to get tax free price
        /// </summary>
        /// <param name="blForFuel">For fule price or not</param>
        /// <param name="sProductKey">Product key</param>
        /// <param name="stockcode">Stock code</param>
        /// <param name="quantityPerPkg">Quantity per kg</param>
        /// <param name="baseUnitQty">Base unit quantity</param>
        /// <param name="taxExemptTaxCode">tax code</param>
        /// <param name="productCode">Product code</param>
        /// <param name="eOutCategory">Category</param>
        /// <param name="dOutUnitsPerPkg">Units per kg</param>
        /// <param name="sOutUpcCode">Out up code</param>
        /// <param name="isError">Is error or not</param>
        /// <returns>True or false</returns>
        bool TeGetTaxFreePriceByRate(bool blForFuel, ref string sProductKey, string stockcode,
            ref short quantityPerPkg, ref short baseUnitQty,
            ref short taxExemptTaxCode, ref string productCode,
            ref mPrivateGlobals.teProductEnum eOutCategory,
            ref float dOutUnitsPerPkg, ref string sOutUpcCode,
            out bool isError);

        /// <summary>
        /// Method to get tax exempt tax free price by price
        /// </summary>
        /// <param name="sSql">Query</param>
        /// <param name="dOutPrice">Price</param>
        /// <param name="originalPrice">Original Price</param>
        /// <param name="eOutCategory">Category</param>
        /// <param name="dOutUnitsPerPkg">Units per kg</param>
        /// <param name="sOutUpcCode">UPC code</param>
        /// <param name="bFound">Found or not</param>
        /// <param name="isError">Error or not</param>
        /// <returns>True or false</returns>
        bool TeGetTaxFreePriceByPrice(string sSql, ref float dOutPrice, double originalPrice,
            ref mPrivateGlobals.teProductEnum eOutCategory, ref float dOutUnitsPerPkg,
            ref string sOutUpcCode, out bool bFound,
            out bool isError);

        /// <summary>
        /// Method to load all taxes
        /// </summary>
        /// <returns></returns>
        List<TaxMast> GetAllTaxes();

        /// <summary>
        /// Method to get tax info by stock code
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>List of stock tax info</returns>
        List<StockTaxInfo> GetTaxInfoByStockCode(string stockCode);


    }
}

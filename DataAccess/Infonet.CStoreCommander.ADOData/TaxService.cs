using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infonet.CStoreCommander.ADOData
{
    public class TaxService : SqlDbService, ITaxService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Method to clean tax exempt trash
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">TIll number</param>
        public void CleanTaxExemptData(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,CleanTaxExemptData,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            Execute("delete  from TaxExemptSaleHead where sale_no =" + Convert.ToString(saleNumber) + " AND Till_Num=" + Convert.ToString(tillNumber), DataSource.CSCTills);
            Execute("delete  from TaxExemptSaleLine where sale_no =" + Convert.ToString(saleNumber) + " AND Till_Num=" + Convert.ToString(tillNumber), DataSource.CSCTills);
            Execute("delete  from TaxCredit where sale_no =" + Convert.ToString(saleNumber) + " AND Till_Num=" + Convert.ToString(tillNumber), DataSource.CSCTills);
            Execute("delete  from TaxCreditLine where sale_no =" + Convert.ToString(saleNumber) + " AND Till_Num=" + Convert.ToString(tillNumber), DataSource.CSCTills);
            _performancelog.Debug($"End,TaxService,CleanTaxExemptData,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to delete purchase item
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">TIll number</param>
        public void DeletePurchaseItem(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,DeletePurchaseItem,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            Execute(" delete from purchaseitem where sale_no =" + Convert.ToString(saleNumber) + " and Till_no =" + Convert.ToString(tillNumber), DataSource.CSCTills);
            _performancelog.Debug($"End,TaxService,DeletePurchaseItem,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Deletes Stock Tax
        /// </summary>
        /// <param name="stockCode"></param>
        /// <returns></returns>
        public bool DeleteStockTax(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,DeleteStockTax,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            Execute("DELETE FROM StockTax  WHERE Stock_Code = \'" + stockCode + "\' ", DataSource.CSCMaster);
            _performancelog.Debug($"End,TaxService,DeleteStockTax,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return true;
        }

        /// <summary>
        /// Get All active taxes
        /// </summary>
        /// <returns>List of stock tax</returns>
        public List<StockTax> GetAllActiveTaxes()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,GetAllActiveTaxes,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            List<StockTax> taxes = new List<StockTax>();
            var rsTax = GetRecords("Select DISTINCT TaxMast.Tax_Name, TaxRate.Tax_Code From TaxMast INNER JOIN TaxRate ON  TaxMast.Tax_Name = TaxRate.Tax_Name Where TaxMast.Tax_Active = 1", DataSource.CSCMaster);
            foreach (DataRow row in rsTax.Rows)
            {
                taxes.Add(new StockTax
                {
                    Name = CommonUtility.GetStringValue(row["Tax_Name"]),
                    Code = CommonUtility.GetStringValue(row["Tax_Code"]),
                });
            }
            _performancelog.Debug($"End,TaxService,GetAllActiveTaxes,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return taxes;
        }

        /// <summary>
        /// Method to get purchase items
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Purchase items</returns>
        public List<tePurchaseItem> GetPurchaseItems(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,GetPurchaseItems,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var purchaseItems = new List<tePurchaseItem>();
            var rsSite = GetRecords("select * from purchaseitem inner join cscmaster.dbo.ProductTaxExempt b " + " on purchaseitem.CscPurchaseItemKey = b.UpcCode Where sale_no =" + Convert.ToString(saleNumber) + " and Till_no =" + Convert.ToString(tillNumber), DataSource.CSCTills);


            foreach (DataRow row in rsSite.Rows)
            {
                purchaseItems.Add(new tePurchaseItem
                {
                    TreatyNo = CommonUtility.GetStringValue(row["TreatyNo"]),
                    ProductType = CommonUtility.GetStringValue(row["CategoryCodeFK"]),
                    Quantity = CommonUtility.GetShortValue(row["Quantity"]),
                    UnitsPerPkg = CommonUtility.GetFloatValue(row["UnitsPerPkg"])
                });
            }

            _performancelog.Debug($"End,TaxService,GetPurchaseItems,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return purchaseItems;
        }

        /// <summary>
        /// Method to get tax exempt card holder
        /// </summary>
        /// <param name="cardHolderId">card holder id</param>
        /// <returns>Tax exempt card holder</returns>
        public teCardholder GetTaxExemptCardHolder(string cardHolderId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,GetTaxExemptCardHolder,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var rsCardHolder = GetRecords("select * from TaxExemptCardRegistry Where CardholderID= \'" + cardHolderId + "\'", DataSource.CSCMaster);
            if (rsCardHolder.Rows.Count > 0)
            {
                return new teCardholder
                {
                    CardholderID = cardHolderId,
                    TobaccoQuota = CommonUtility.GetFloatValue(rsCardHolder.Rows[0]["TobaccoQuota"]),
                    GasQuota = CommonUtility.GetFloatValue(rsCardHolder.Rows[0]["GasQuota"]),
                    PropaneQuota = CommonUtility.GetFloatValue(rsCardHolder.Rows[0]["PropaneQuota"])
                };
            }
            _performancelog.Debug($"End,TaxService,GetTaxExemptCardHolder,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return null;
        }

        /// <summary>
        /// Method to get tax exempt sale line
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Tax exempt sale lines</returns>
        public List<TaxExemptSaleLine> GetTaxExemptSaleLines(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,GetTaxExemptSaleLines,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var taxExemptSalelines = new List<TaxExemptSaleLine>();
            var rsSite =
                GetRecords(
                    "select * from TaxExemptSaleLine a inner join TaxExemptSaleHead b " +
                    " On  a.sale_no = b.sale_No and a.till_num = b.till_num Where a.sale_no =" +
                    Convert.ToString(saleNumber) + " and a.Till_num =" + Convert.ToString(tillNumber),
                    DataSource.CSCTills);
            foreach (DataRow dataRow in rsSite.Rows)
            {
                taxExemptSalelines.Add(new TaxExemptSaleLine
                {
                    ExemptedTax = CommonUtility.GetIntergerValue(dataRow["ExemptedTax"]),
                    ProductType = (mPrivateGlobals.teProductEnum)Enum.Parse(typeof(mPrivateGlobals.teProductEnum), CommonUtility.GetStringValue(dataRow["ProductType"]), true),
                    CardHolderID = CommonUtility.GetStringValue(dataRow["CardholderID"])
                });
            }

            _performancelog.Debug($"End,TaxService,GetTaxExemptSaleLines,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return taxExemptSalelines;
        }


        /// <summary>
        /// Method to get all tax mast
        /// </summary>
        /// <returns>List of tax mast</returns>
        public List<TaxMast> GetTaxMast()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,GetTaxMast,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var taxMastRecordSet = GetRecords("SELECT * FROM TaxMast ORDER BY TaxMast.Tax_Ord ", DataSource.CSCMaster);
            var taxes = new List<TaxMast>();
            foreach (DataRow dataRow in taxMastRecordSet.Rows)
            {
                var taxMast = new TaxMast
                {
                    TaxOrd = CommonUtility.GetShortValue(dataRow["TAX_ORD"]),
                    Active = CommonUtility.GetBooleanValue(dataRow["TAX_ACTIVE"]),
                    TaxApply = CommonUtility.GetStringValue(dataRow["TAX_APPLY"]),
                    TaxDefination = CommonUtility.GetStringValue(dataRow["TAX_DEF"]),
                    TaxDescription = CommonUtility.GetStringValue(dataRow["TAX_DESC"]),
                    TaxName = CommonUtility.GetStringValue(dataRow["TAX_NAME"])
                };
                taxes.Add(taxMast);
            }
            _performancelog.Debug($"End,TaxService,GetTaxMast,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return taxes;
        }

        /// <summary>
        /// Method to get test rate by tax name
        /// </summary>
        /// <param name="taxName">Tax name</param>
        /// <returns>List of tax rate</returns>
        public List<TaxRate> GetTaxRatesByName(string taxName)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,GetTaxRatesByName,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var taxRateRecordSet = GetRecords("SELECT * FROM TaxRate  WHERE Tax_Name = \'" + taxName + "\' ", DataSource.CSCMaster);
            var taxRates = new List<TaxRate>();
            foreach (DataRow dataRow in taxRateRecordSet.Rows)
            {
                var taxRate = new TaxRate
                {
                    TaxName = CommonUtility.GetStringValue(dataRow["TAX_NAME"]),
                    TaxCode = CommonUtility.GetStringValue(dataRow["TAX_CODE"]),
                    TaxDescription = CommonUtility.GetStringValue(dataRow["TAX_DESC"]),
                    Rebate = CommonUtility.GetFloatValue(dataRow["TAX_REBATE"]),
                    Rate = CommonUtility.GetFloatValue(dataRow["TAX_RATE"]),
                    Included = CommonUtility.GetBooleanValue(dataRow["TAX_INCL"])
                };
                taxRates.Add(taxRate);
            }
            _performancelog.Debug($"End,TaxService,GetTaxRatesByName,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return taxRates;
        }

        /// <summary>
        /// Set up Taxes
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="taxNames">Tax name</param>
        /// <param name="selectedTaxes">Selected taxes</param>
        /// <returns>True or false</returns>
        public bool SetupTaxes(string stockCode, List<string> taxNames, List<string> selectedTaxes)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,SetupTaxes,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            short i;
            var returnValue = true;
            var taxes = GetAllActiveTaxes();
            for (i = 0; i < taxNames.Count; i++)
            {
                if (selectedTaxes.Contains(taxNames[i]))
                {
                    var taxName = taxes[i].Name;
                    var taxCode = taxes[i].Code;
                    _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
                    if (_connection.State == ConnectionState.Closed)
                    {
                        _connection.Open();
                    }
                    _dataTable = new DataTable();
                    _adapter = new SqlDataAdapter("SELECT * FROM StockTax where Stock_Code = \'" + stockCode + "\'and Tax_name = \'" + taxName + "\'", _connection);
                    _adapter.Fill(_dataTable);

                    if (_dataTable.Rows.Count > 0)
                    {
                        returnValue = false;
                    }
                    else
                    {
                        DataRow row = _dataTable.NewRow();
                        row["Stock_Code"] = stockCode;
                        row["Tax_Name"] = taxName;
                        row["Tax_Code"] = taxCode;
                        _dataTable.Rows.Add(row);
                        SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                        _adapter.InsertCommand = builder.GetInsertCommand();
                        _adapter.Update(_dataTable);
                        _connection.Close();
                        _adapter?.Dispose();
                    }
                }
            }
            _performancelog.Debug($"End,TaxService,SetupTaxes,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Update tax exempt card holder
        /// </summary>
        /// <param name="taxExemptCardHolder">tax exempt card holder</param>
        public void UpdateTaxExemptCardHolder(teCardholder taxExemptCardHolder)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,UpdateTaxExemptCardHolder,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var rsCardHolder = new DataTable();
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _adapter = new SqlDataAdapter("select * from TaxExemptCardRegistry Where CardholderID= \'" + taxExemptCardHolder.CardholderID + "\'", _connection);
            _adapter.Fill(rsCardHolder);
            if (rsCardHolder.Rows.Count > 0)
            {
                rsCardHolder.Rows[0]["TobaccoQuota"] = taxExemptCardHolder.TobaccoQuota;
                rsCardHolder.Rows[0]["GasQuota"] = taxExemptCardHolder.GasQuota;
                rsCardHolder.Rows[0]["PropaneQuota"] = taxExemptCardHolder.PropaneQuota;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(rsCardHolder);
                _connection.Close();
                _adapter?.Dispose();
            }
            _performancelog.Debug($"End,TaxService,UpdateTaxExemptCardHolder,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to get tax exempt rate
        /// </summary>
        /// <param name="taxExemptTaxCode">Tax exempt tax code</param>
        /// <param name="taxRate">Tax rate</param>
        /// <param name="rateType">Rate type</param>
        /// <returns>True or false</returns>
        public bool GetTaxExemptRate(string taxExemptTaxCode, out float taxRate, out string rateType)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,GetTaxExemptRate,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var rs = GetRecords("select * from TaxExemptRate where TAX_CODE=" + taxExemptTaxCode, DataSource.CSCMaster);
            if (rs.Rows.Count == 0)
            {
                taxRate = 0;
                rateType = string.Empty;
                return false;
            }

            taxRate = CommonUtility.GetFloatValue(rs.Rows[0]["Tax_Rate"]);
            rateType = CommonUtility.GetStringValue(rs.Rows[0]["RateType"]);
            _performancelog.Debug($"End,TaxService,GetTaxExemptRate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return true;
        }

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
        public bool TeGetTaxFreePriceByRate(bool blForFuel, ref string sProductKey, string stockcode,
            ref short quantityPerPkg, ref short baseUnitQty,
            ref short taxExemptTaxCode, ref string productCode,
            ref mPrivateGlobals.teProductEnum eOutCategory,
            ref float dOutUnitsPerPkg, ref string sOutUpcCode,
            out bool isError)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,TeGetTaxFreePriceByRate,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            DataTable oRecs;

            if (blForFuel)
            {
                oRecs = GetRecords("SELECT CategoryFK,ProductCode,TAX_CODE,  QuantityPerPkg, BaseUnitQty,  1 As UnitsPerPkg, \'\' As UpcCode  from ProductTaxExempt Where ProductKey =\'" + stockcode + "\' ", DataSource.CSCMaster);
            }
            else
            {
                oRecs = GetRecords("SELECT CategoryFK, ProductCode,UnitsPerPkg, TAX_CODE, QuantityPerPkg, BaseUnitQty, UpcCode FROM ProductTaxExempt Where ProductKey=\'" + sProductKey + "\'", DataSource.CSCMaster);
            }
            if (oRecs.Rows.Count == 0)
            {
                isError = false;
                return false;
            }

            taxExemptTaxCode = CommonUtility.GetShortValue(oRecs.Rows[0]["Tax_Code"]);
            productCode = CommonUtility.GetStringValue(oRecs.Rows[0]["ProductCode"]);
            if (CommonUtility.GetBooleanValue(oRecs.Rows[0]["CategoryFK"]))
            {
                eOutCategory = (mPrivateGlobals.teProductEnum)CommonUtility.GetIntergerValue(oRecs.Rows[0]["CategoryFK"]);
            }
            else
            {
                isError = true;
                return true;
            }
            isError = false;
            dOutUnitsPerPkg = CommonUtility.GetFloatValue(oRecs.Rows[0]["UnitsPerPkg"]);
            sOutUpcCode = CommonUtility.GetStringValue(oRecs.Rows[0]["UpcCode"]);
            quantityPerPkg = CommonUtility.GetShortValue(oRecs.Rows[0]["QuantityPerPkg"]);
            baseUnitQty = CommonUtility.GetShortValue(oRecs.Rows[0]["BaseUnitQty"]);
            _performancelog.Debug($"End,TaxService,TeGetTaxFreePriceByRate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return true;
        }

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
        public bool TeGetTaxFreePriceByPrice(string sSql, ref float dOutPrice, double originalPrice,
            ref mPrivateGlobals.teProductEnum eOutCategory, ref float dOutUnitsPerPkg,
            ref string sOutUpcCode, out bool bFound,
            out bool isError)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,TeGetTaxFreePriceByPrice,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var oRecs = GetRecords(sSql, DataSource.CSCPump);
            //eCategory and bFound are already set.
            if (oRecs.Rows.Count > 0)
            {
                if (CommonUtility.GetIntergerValue(oRecs.Rows[0]["TaxFreePrice"]) != 0)
                {
                    dOutPrice = CommonUtility.GetFloatValue(oRecs.Rows[0]["TaxFreePrice"]);
                }
                else
                {
                    dOutPrice = (float)originalPrice;
                }
                if (CommonUtility.GetBooleanValue(oRecs.Rows[0]["CategoryFK"]))
                {
                    eOutCategory = (mPrivateGlobals.teProductEnum)CommonUtility.GetIntergerValue(oRecs.Rows[0]["CategoryFK"]);
                }
                else
                {
                    isError = true;
                    bFound = false;
                    return false;
                }

                dOutUnitsPerPkg = CommonUtility.GetFloatValue(oRecs.Rows[0]["UnitsPerPkg"]);
                sOutUpcCode = CommonUtility.GetStringValue(oRecs.Rows[0]["UpcCode"]);
                bFound = true;
                isError = false;
                return true;
            }
            bFound = false;
            isError = false;
            _performancelog.Debug($"End,TaxService,TeGetTaxFreePriceByPrice,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return false;

        }

        /// <summary>
        /// Method to load all taxes
        /// </summary>
        /// <returns>List of tax mast</returns>
        public List<TaxMast> GetAllTaxes()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,GetAllTaxes,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var taxes = new List<TaxMast>();
            var rs = GetRecords("SELECT TaxMast.Tax_Name,  TaxMast.Tax_Desc,taxmast.tax_active  FROM   TaxMast ORDER BY TaxMast.Tax_Ord ", DataSource.CSCMaster);
            foreach (DataRow dataRow in rs.Rows)
            {
                taxes.Add(new TaxMast
                {
                    TaxName = CommonUtility.GetStringValue(dataRow["Tax_Name"]),
                    TaxDescription = CommonUtility.GetStringValue(dataRow["Tax_Desc"]),
                    Active = CommonUtility.GetBooleanValue(dataRow["Tax_Active"]),
                });
            }
            _performancelog.Debug($"End,TaxService,GetAllTaxes,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return taxes;
        }

        /// <summary>
        /// Method to get tax info by stock code
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>List of stock tax info</returns>
        public List<StockTaxInfo> GetTaxInfoByStockCode(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TaxService,GetTaxInfoByStockCode,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            List<StockTaxInfo> taxes = new List<StockTaxInfo>();
            var rsTax =
                GetRecords(
                    @"SELECT A.Tax_Name, A.Tax_Code, D.Tax_Rate, D.Tax_Incl, D.Tax_Rebate  FROM StockTax as A left join (select B.TAX_NAME,B.TAX_ACTIVE, c.Tax_Rate, c.Tax_Incl, c.Tax_Rebate, c.TAX_CODE from taxMast as B join TAXRATE as c
                    on B.TAX_NAME = c.TAX_NAME) as D on A.Tax_Name = D.TAX_NAME and A.Tax_Code = D.TAX_CODE where A.Stock_Code = '" + stockCode + "' and D.Tax_Active = 1", DataSource.CSCMaster);
            foreach (DataRow dataRow in rsTax.Rows)
            {
                taxes.Add(new StockTaxInfo
                {
                    TaxName = CommonUtility.GetStringValue(dataRow["Tax_Name"]),
                    TaxCode = CommonUtility.GetStringValue(dataRow["Tax_Code"]),
                    Rate = CommonUtility.GetFloatValue(dataRow["Tax_Rate"]),
                    Included = CommonUtility.GetBooleanValue(dataRow["Tax_Incl"]),
                    Rebate = CommonUtility.GetFloatValue(dataRow["Tax_Rebate"])
                });
            }
            _performancelog.Debug($"End,TaxService,GetTaxInfoByStockCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return taxes;
        }


    }
}

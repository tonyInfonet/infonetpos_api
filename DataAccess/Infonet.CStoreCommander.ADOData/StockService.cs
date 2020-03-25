using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Infonet.CStoreCommander.ADOData
{
    public class StockService : SqlDbService, IStockService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Method to check whether stock is available for a day
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>True or false</returns>
        public List<string> IsStockByDayAvailable(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,IsStockByDayAvailable,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            List<string> days = new List<string>();
            if (string.IsNullOrEmpty(stockCode))
            {
                _performancelog.Debug($"End,StockService,IsStockByDayAvailable,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return days;
            }
            string query = $"SELECT DayOfWeek FROM StockByDay WHERE Stock_Code= '{stockCode}'";
            var dt = GetRecords(query, DataSource.CSCMaster);
            if (dt == null || dt.Rows.Count == 0)
            {
                return days;
            }

            foreach (DataRow dr in dt.Rows)
            {
                days.Add(dr["DayOfWeek"].ToString());
            }

            _performancelog.Debug($"End,StockService,IsStockByDayAvailable,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return days;
        }

        /// <summary>
        /// Method to get plu mast by plu code
        /// </summary>
        /// <param name="pluCode">Plu code</param>
        /// <returns>Plu mast</returns>
        public PLUMast GetPluMast(string pluCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetPluMast,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            PLUMast pluMast = null;
            if (string.IsNullOrEmpty(pluCode))
            {
                return null;
            }
            string query = $"select * from Plumast where plu_code ='{pluCode}'";
            var dt = GetRecords(query, DataSource.CSCMaster);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            foreach (DataRow dr in dt.Rows)
            {
                pluMast = new PLUMast
                {
                    PLUCode = CommonUtility.GetStringValue(dr["PLU_CODE"]),
                    PLUPrim = CommonUtility.GetStringValue(dr["PLU_PRIM"]),
                    PLUType = CommonUtility.GetCharValue(dr["PLU_TYPE"])
                };
            }

            _performancelog.Debug($"End,StockService,GetPluMast,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return pluMast;
        }

        /// <summary>
        /// Method to get stock item by code
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Stock Item</returns>
        public StockItem GetStockItem(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetStockItem,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            StockItem stockItem = null;
            if (string.IsNullOrEmpty(stockCode))
            {
                return null;
            }
            string query = $"Select * from StockMst where Stock_code ='{stockCode}'";
            var dt = GetRecords(query, DataSource.CSCMaster);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            foreach (DataRow dr in dt.Rows)
            {
                stockItem = new StockItem
                {
                    StockCode = CommonUtility.GetStringValue(dr["STOCK_CODE"]),
                    Description = CommonUtility.GetStringValue(dr["DESCRIPT"]),
                    StockType = CommonUtility.GetCharValue(dr["STOCK_TYPE"]),
                    Department = CommonUtility.GetStringValue(dr["DEPT"]),
                    SubDepartment = CommonUtility.GetStringValue(dr["SUB_DEPT"]),
                    SubDetail = CommonUtility.GetStringValue(dr["Sub_Detail"]),
                    Vendor = CommonUtility.GetStringValue(dr["Vendor"]),
                };
            }

            _performancelog.Debug($"End,StockService,GetStockItem,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return stockItem;
        }

        /// <summary>
        /// Method to get stock rebate
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Stock rebate amount</returns>
        public decimal GetStockRebate(string vendor, string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetStockRebate,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            decimal rebate = 0;
            if (!string.IsNullOrEmpty(vendor))
            {
                var query =
                    $"SELECT SUM(Rebate) AS TRebate FROM StockRebates WHERE VendorID='{vendor}' AND Stock_Code= \'{stockCode}\' AND OrderNo=0";
                var dt = GetRecords(query, DataSource.CSCMaster);
                if (dt != null && dt.Rows.Count > 0)
                {
                    rebate = CommonUtility.GetDecimalValue(dt.Rows[0]["TRebate"]);
                }
            }
            _performancelog.Debug($"End,StockService,GetStockRebate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return rebate;
        }


        /// <summary>
        /// Method to get stock price
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Stock price</returns>
        public List<StockItem> GetStockPricesByCode(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetStockPricesByCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var prices = new List<StockItem>();
            var query = "SELECT price,VendorID  FROM Stock_Prices  WHERE Stock_Prices.Stock_Code = \'" + stockCode + "\'  AND OrderNo=0 AND Stock_Prices.Price_Number = 1 ORDER BY Stock_Prices.Price_Number ";
            var dt = GetRecords(query, DataSource.CSCMaster);

            if (dt.Rows.Count == 0)
            {
                return null;
            }
            foreach (DataRow dr in dt.Rows)
            {
                prices.Add(new StockItem
                {
                    Price = CommonUtility.GetDecimalValue(dr["price"]),
                    Vendor = CommonUtility.GetStringValue(dr["VendorID"]),
                });
            }
            _performancelog.Debug($"End,StockService,GetStockPricesByCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return prices;
        }


        /// <summary>
        /// Method to get priceL
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        public List<PriceL> GetPriceLByCode(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetPriceL,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");


            var dt = GetRecords("SELECT Pr_Type, Pr_Units,VendorID FROM PriceL WHERE Stock_Code=\'" + stockCode + "\' AND OrderNo=0", DataSource.CSCMaster);
            var pricels = new List<PriceL>();
            if (dt == null || dt.Rows.Count == 0)
            {
                _performancelog.Debug($"End,StockService,GetPriceLByCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return new List<PriceL>();
            }
            foreach (DataRow dr in dt.Rows)
            {
                pricels.Add(new PriceL
                {
                    Vendor = CommonUtility.GetStringValue(dr["VendorID"]),
                    PriceType = DBNull.Value.Equals(dr["pr_type"]) ? 'R' : Convert.ToChar(dr["pr_type"]),
                    PriceUnit = DBNull.Value.Equals(dr["Pr_Units"]) ? 'R' : Convert.ToChar(dr["Pr_Units"])
                });
            }
            _performancelog.Debug($"End,StockService,GetPriceLByCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return pricels;
        }

        /// <summary>
        /// Method find if there is check group button
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Truie or false</returns>
        public bool CheckGroupButton(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,CheckGroupButton,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var flag = false;
            var ds = GetRecords("select * from GroupButtons where Product = \'" + stockCode + "\' and buttontype = 1", DataSource.CSCMaster);
            if (ds != null)
            {
                flag = true;
            }
            _performancelog.Debug($"End,StockService,CheckGroupButton,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return flag;

        }

        /// <summary>
        /// Method to get product tax exempt
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>product tax exempt</returns>
        public ProductTaxExempt GetProductTaxExempt(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetProductTaxExempt,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var dt = GetRecords("SELECT CategoryFK,TEVendor FROM ProductTaxExempt WHERE UpcCode=\'" + stockCode + "\'", DataSource.CSCMaster);
            ProductTaxExempt productTaxExempt = null;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    productTaxExempt = new ProductTaxExempt
                    {
                        CategoryFK = CommonUtility.GetShortValue(dr["CategoryFK"]),
                        TEVendor = CommonUtility.GetStringValue(dr["TEVendor"])
                    };
                }
            }
            _performancelog.Debug($"End,StockService,GetProductTaxExempt,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return productTaxExempt;
        }


        /// <summary>
        /// Method to get product tax exempt
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>product tax exempt</returns>
        public ProductTaxExempt GetProductTaxExemptByProductCode(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetProductTaxExemptByProductCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var dt = GetRecords("SELECT TaxFreePrice,CategoryFK,TEVendor,Tax_Code,Available from ProductTaxExempt Where ProductKey =\'" + stockCode + "\'", DataSource.CSCMaster);
            ProductTaxExempt productTaxExempt = null;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    productTaxExempt = new ProductTaxExempt
                    {
                        CategoryFK = CommonUtility.GetShortValue(dr["CategoryFK"]),
                        TEVendor = CommonUtility.GetStringValue(dr["TEVendor"]),
                        TaxFreePrice = CommonUtility.GetFloatValue(dr["TaxFreePrice"]),
                        TaxCode = CommonUtility.GetShortValue(dr["Tax_Code"]),
                        Available = CommonUtility.GetIntergerValue(dr["Available"])
                    };
                }
            }
            _performancelog.Debug($"End,StockService,GetProductTaxExemptByProductCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return productTaxExempt;
        }

        /// <summary>
        /// Method to get stock branch
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Stock Br</returns>
        public StockBr GetStockBr(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetStockBr,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            StockBr stock = null;
            var query = $"Select * from Stock_Br where STOCK_CODE ='{stockCode}'";

            var dt = GetRecords(query, DataSource.CSCMaster);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    stock = new StockBr
                    {
                        ActiveStock = DBNull.Value.Equals(dr["Activestock"]) || Convert.ToBoolean(dr["Activestock"]),
                        AvalItems = CommonUtility.GetDoubleValue(dr["AVAIL"]),
                    };
                }
            }
            _performancelog.Debug($"End,StockService,GetStockBr,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return stock;
        }

        /// <summary>
        /// Method to get list of stock taxes
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>List of stock taxes</returns>
        public List<StockTax> GetStockTaxes(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetStockTaxes,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var stockTaxes = new List<StockTax>();
            var dt = GetRecords("SELECT StockTax.Tax_Name, StockTax.Tax_Code  FROM   StockTax  WHERE  StockTax.Stock_Code = \'" + stockCode + "\'", DataSource.CSCMaster);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    stockTaxes.Add(new StockTax
                    {
                        Code = CommonUtility.GetStringValue(dr["Tax_Code"]),
                        Name = CommonUtility.GetStringValue(dr["Tax_Name"])

                    });
                }
            }
            _performancelog.Debug($"End,StockService,GetStockTaxes,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return stockTaxes;
        }

        /// <summary>
        /// Method to get tax rate
        /// </summary>
        /// <param name="taxName">Tax name</param>
        /// <param name="taxCode">Tax code</param>
        /// <returns>Tax rate</returns>
        public TaxRate GetTaxRate(string taxName, string taxCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetTaxRate,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var dt = GetRecords("SELECT Tax_Rate, Tax_Incl, Tax_Rebate FROM TaxRate  WHERE Tax_Name = \'" + taxName + "\' AND  Tax_Code = \'" + taxCode + "\'", DataSource.CSCMaster);
            TaxRate taxRate = null;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    taxRate = new TaxRate
                    {
                        Rate = CommonUtility.GetFloatValue(dr["Tax_Rate"]),
                        Included = CommonUtility.GetBooleanValue(dr["Tax_Incl"]),
                        Rebate = CommonUtility.GetFloatValue(dr["Tax_Rebate"])
                    };
                }
            }
            _performancelog.Debug($"End,StockService,GetTaxRate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return taxRate;
        }

        /// <summary>
        /// Method to get tax mast
        /// </summary>
        /// <param name="taxName">Tax name</param>
        /// <returns>Tax mast</returns>
        public TaxMast GetTaxMast(string taxName)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetTaxMast,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            TaxMast taxMast = null;
            var dt = GetRecords("SELECT TaxMast.Tax_Active, TaxMast.Tax_Ord FROM TaxMast  WHERE Tax_Name = \'" + taxName + "\'", DataSource.CSCMaster);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    taxMast = new TaxMast
                    {
                        Active = CommonUtility.GetBooleanValue(dr["Tax_Active"]),
                        TaxOrd = CommonUtility.GetShortValue(dr["Tax_Ord"])
                    };
                }
            }
            _performancelog.Debug($"End,StockService,GetTaxMast,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return taxMast;
        }

        /// <summary>
        /// Method to get pricel for any range
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>PriceL</returns>
        public List<PriceL> GetPriceLForRange(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetPriceLForRange,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var dt = GetRecords("SELECT Pr_F_Qty, Pr_T_Qty, Price, StartDate, EndDate,VendorID   FROM     PriceL  WHERE    PriceL.Stock_Code = \'" + stockCode + "\' AND StartDate<=\'" + DateAndTime.Today.ToString("yyyyMMdd") + "\' AND EndDate>=\'" + DateAndTime.Today.ToString("yyyyMMdd") + "\' AND OrderNo=0 ORDER BY PriceL.Pr_F_Qty", DataSource.CSCMaster);
            if (dt == null)
            {
                return new List<PriceL>();
            }
            var specialPrices = new List<PriceL>();
            foreach (DataRow dr in dt.Rows)
            {
                specialPrices.Add(new PriceL
                {
                    Vendor = CommonUtility.GetStringValue(dr["VendorID"]),
                    FQuantity = CommonUtility.GetFloatValue(dr["Pr_F_Qty"]),
                    TQuantity = CommonUtility.GetFloatValue(dr["Pr_T_Qty"]),
                    Price = CommonUtility.GetFloatValue(dr["Price"]),
                    StartDate = CommonUtility.GetDateTimeValue(dr["StartDate"]),
                    EndDate = CommonUtility.GetDateTimeValue(dr["EndDate"])
                });
            }
            _performancelog.Debug($"End,StockService,GetPriceLForRange,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return specialPrices;
        }

        /// <summary>
        /// Method to check whether kit is present
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>True or false</returns>
        public bool IsKitPresent(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,IsKitPresent,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var flag = false;
            // Load Kit Components
            var ds = GetRecords("select * from Kit_Mast where KIT_CODE =\'" + stockCode + "\'", DataSource.CSCMaster);
            if (ds != null)
            {
                flag = true;
            }
            _performancelog.Debug($"End,StockService,IsKitPresent,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return flag;
        }

        /// <summary>
        /// Method to get kit items
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>List of kit items</returns>
        public List<KitItem> GetKitIems(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetKitIems,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var dt = GetRecords("SELECT Kit_Item.Stock_Code, Kit_Item.Quantity  FROM   Kit_Item  WHERE  Kit_Item.Kit_Code = \'" + stockCode + "\'", DataSource.CSCMaster);

            var kitItems = new List<KitItem>();
            foreach (DataRow dr in dt.Rows)
            {
                kitItems.Add(new KitItem
                {
                    Quantity = CommonUtility.GetDoubleValue(dr["Quantity"]),
                    StockCode = CommonUtility.GetStringValue(dr["Stock_Code"])
                });
            }
            _performancelog.Debug($"End,StockService,GetKitIems,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return kitItems;
        }

        /// <summary>
        /// Method to get associate charges
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>List of associate charges</returns>
        public List<AssociateCharge> GetAssociateCharges(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetAssociateCharges,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var charges = new List<AssociateCharge>();
            var dt = GetRecords("SELECT STOCK_AC.STOCK_CODE, ASSOC.AS_CODE, ASSOC.AS_DESC, ASSOC.AS_PRICE  FROM   ASSOC INNER JOIN STOCK_AC ON  ASSOC.AS_CODE = STOCK_AC.AS_CODE  WHERE  STOCK_AC.STOCK_CODE = \'" + stockCode + "\'", DataSource.CSCMaster);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    charges.Add(new AssociateCharge
                    {
                        StockCode = CommonUtility.GetStringValue(dr["STOCK_CODE"]),
                        AsCode = CommonUtility.GetStringValue(dr["AS_CODE"]),
                        Description = CommonUtility.GetStringValue(dr["AS_DESC"]),
                        Price = CommonUtility.GetFloatValue(dr["AS_PRICE"])
                    });
                }
            }
            _performancelog.Debug($"End,StockService,GetAssociateCharges,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return charges;
        }

        /// <summary>
        /// Method to get sale tax
        /// </summary>
        /// <param name="asCode">As code</param>
        /// <returns>List of taxes</returns>
        public List<Sale_Tax> GetTax(string asCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetTax,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var taxes = new List<Sale_Tax>();
            var dt = GetRecords("SELECT Assoc_Tax.Tax_Name, Assoc_Tax.Tax_Code,TaxRate.Tax_Incl, TaxRate.Tax_Rate  FROM   Assoc_Tax INNER JOIN TaxRate ON (Assoc_Tax.Tax_Name = TaxRate.Tax_Name AND Assoc_Tax.Tax_Code = TaxRate.Tax_Code)  WHERE  Assoc_Tax.As_Code = \'" + asCode + "\' ", DataSource.CSCMaster);

            if (dt != null)
            {
                taxes.AddRange(from DataRow dr in dt.Rows
                               select new Sale_Tax
                               {
                                   Tax_Name = CommonUtility.GetStringValue(dr["Assoc_Tax.Tax_Name"]),
                                   Tax_Code = CommonUtility.GetStringValue(dr["Assoc_Tax.Tax_Code"]),
                                   Tax_Rate = CommonUtility.GetFloatValue(dr["TaxRate.Tax_Rate"]),
                                   Tax_Included = CommonUtility.GetBooleanValue(dr["TaxRate.Tax_Incl"])
                               });
            }
            _performancelog.Debug($"End,StockService,GetTax,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return taxes;
        }


        /// <summary>
        /// Method to get promos
        /// </summary>
        /// <returns>List of promos</returns>
        public List<Promo> GetPromosForToday()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetPromosForToday,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var promos = new List<Promo>();
            var strSql = "SELECT DISTINCT [PromoHeader].[PromoID] AS PromoID, [PromoHeader].[Day] AS Day,[PromoDetail].[Qty] AS Qty, [PromoDetail].[Link] AS Link, [PromoDetail].[Amount] AS Amount,[PromoDetail].[Stock_Code],[PromoDetail].[Dept],[PromoDetail].[Sub_Dept],[PromoDetail].[SubDetail] FROM PromoHeader LEFT JOIN PromoDetail ON [PromoHeader].[PromoID] =[PromoDetail].[PromoID]  WHERE (Day IS NULL OR Day=0 OR Day=" + Convert.ToString(DateAndTime.Weekday(DateAndTime.Today)) + ") AND [PromoHeader].[EndDate]>= \'" + DateTime.Now.ToString("yyyyMMdd") + "\' AND [PromoHeader].[StartDate]<= \'" + DateTime.Now.ToString("yyyyMMdd") + "\'";
            var dt = GetRecords(strSql, DataSource.CSCMaster);
            foreach (DataRow dr in dt.Rows)
            {
                promos.Add(new Promo
                {
                    StockCode = CommonUtility.GetStringValue(dr["Stock_Code"]),
                    Dept = CommonUtility.GetStringValue(dr["Dept"]),
                    SubDept = CommonUtility.GetStringValue(dr["Sub_Dept"]),
                    SubDetail = CommonUtility.GetStringValue(dr["SubDetail"]),
                    PromoID = CommonUtility.GetStringValue(dr["PromoID"]),
                    Day = CommonUtility.GetByteValue(dr["Day"]),
                    TotalQty = CommonUtility.GetShortValue(dr["Qty"]),
                    MaxLink = CommonUtility.GetShortValue(dr["Link"]),
                    Amount = CommonUtility.GetDoubleValue(dr["Amount"])
                });
            }
            _performancelog.Debug($"End,StockService,GetPromosForToday,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return promos;
        }



        /// <summary>
        /// Method to get stock items
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of stock item</returns>
        public List<StockItem> GetStockItems(int pageIndex, int pageSize)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetStockItems,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var stockItems = new List<StockItem>();

            var sqlStmt = @"SELECT A.PLU_Code, A.PLU_Prim, B.Descript FROM PLUMast AS A INNER JOIN StockMst AS B ON A.PLU_Prim=B.Stock_Code " +
                           "ORDER BY A.PLU_Code";

            var dt = GetPagedRecords(sqlStmt, DataSource.CSCMaster, pageIndex, pageSize);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow fields in dt.Rows)
                {
                    stockItems.Add(new StockItem
                    {
                        StockCode = CommonUtility.GetStringValue(fields["PLU_Prim"]),
                        Description = CommonUtility.GetStringValue(fields["Descript"]),
                        AlternateCode = CommonUtility.GetStringValue(fields["PLU_Code"])
                    });
                }
            }
            _performancelog.Debug($"End,StockService,GetStockItems,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return stockItems;
        }

        /// <summary>
        /// Method to get only active stock items
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of stock items</returns>
        public List<StockItem> GetActiveStockItems(int pageIndex, int pageSize)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetActiveStockItems,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var stockItems = new List<StockItem>();
            const string sqlStmt = @"SELECT A.PLU_Code, A.PLU_Prim, B.Descript FROM PLUMast AS A INNER JOIN StockMst AS B ON A.PLU_Prim=B.Stock_Code 
                            INNER JOIN STOCK_BR ON B.Stock_Code = STock_BR.Stock_code
                            WHERE(Stock_BR.ActiveStock is null or Stock_Br.ActiveStock = 1)" +
                            "ORDER BY A.PLU_Code";

            var dt = GetPagedRecords(sqlStmt, DataSource.CSCMaster, pageIndex, pageSize);
            if (dt.Rows.Count != 0)
            {
                stockItems.AddRange(from DataRow fields in dt.Rows
                                    select new StockItem
                                    {
                                        StockCode = CommonUtility.GetStringValue(fields["PLU_Prim"]),
                                        Description = CommonUtility.GetStringValue(fields["Descript"]),
                                        AlternateCode = CommonUtility.GetStringValue(fields["PLU_Code"])
                                    });
            }
            _performancelog.Debug($"End,StockService,GetActiveStockItems,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return stockItems;
        }

        /// <summary>
        /// Method to search stock
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="sellInactive">If sell inactive or not</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of stock item</returns>
        public List<StockItem> SearchStock(string searchTerm, bool sellInactive, int pageIndex, int pageSize)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,SearchStock,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var stockItems = new List<StockItem>();

            string sqlStmt;

            if (sellInactive)
            {
                sqlStmt = @"SELECT A.PLU_Code, A.PLU_Prim, B.Descript FROM PLUMast AS A INNER JOIN StockMst AS B ON A.PLU_Prim=B.Stock_Code 
                           where a.PLU_CODE like '%" + searchTerm + "%' or a.PLU_PRIM like '%" + searchTerm + "%' or b.DESCRIPT like '%" + searchTerm + "%' " +
                           "ORDER BY A.PLU_Prim, B.Descript";
            }
            else
            {
                sqlStmt = @"SELECT A.PLU_Code, A.PLU_Prim, B.Descript FROM PLUMast AS A INNER JOIN StockMst AS B ON A.PLU_Prim=B.Stock_Code 
                            INNER JOIN STOCK_BR ON B.Stock_Code = STock_BR.Stock_code
                            WHERE(Stock_BR.ActiveStock is null or Stock_Br.ActiveStock = 1)
                            AND a.PLU_CODE like '%" + searchTerm + "%' or a.PLU_PRIM like '%" + searchTerm + "%' or b.DESCRIPT like '%" + searchTerm + "%'" +
                            "ORDER BY A.PLU_Prim, B.Descript";
            }
            var dt = GetPagedRecords(sqlStmt + "", DataSource.CSCMaster, pageIndex, pageSize);
            if (dt != null)
            {
                foreach (DataRow fields in dt.Rows)
                {

                    stockItems.Add(new StockItem
                    {
                        StockCode = CommonUtility.GetStringValue(fields["PLU_Prim"]),
                        Description = CommonUtility.GetStringValue(fields["Descript"]),
                        AlternateCode = CommonUtility.GetStringValue(fields["PLU_Code"])

                    });
                }
            }
            _performancelog.Debug($"End,StockService,SearchStock,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return stockItems;
        }

        /// <summary>
        /// Method to get stock item by code
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="sellInactive">Sell inactive or not</param>
        /// <returns>List of stock items</returns>
        public StockItem GetStockItemByCode(string stockCode, bool sellInactive)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetStockItemByCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            string sqlStmt;

            if (sellInactive)
            {
                sqlStmt = "SELECT StockMst.Stock_Code, StockMst.Descript  FROM   StockMst where StockMst.Stock_Code = '" + stockCode + "' ORDER BY ";
            }
            else
            {
                sqlStmt = "SELECT StockMst.Stock_Code,StockMst.Descript  FROM StockMst INNER JOIN STOCK_BR ON STOCKMST.Stock_Code = STock_BR.Stock_code WHERE (Stock_BR.ActiveStock is null or Stock_Br.ActiveStock = 1)"

                    + "AND StockMst.Stock_Code = '" + stockCode + "' ORDER BY ";
            }

            var dt = GetRecords(sqlStmt + "StockMst.Stock_Code", DataSource.CSCMaster);
            StockItem item = null;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    item = new StockItem
                    {
                        StockCode = CommonUtility.GetStringValue(dr["Stock_Code"]),
                        Description = CommonUtility.GetStringValue(dr["Descript"]),
                    };
                }
            }
            _performancelog.Debug($"End,StockService,GetStockItemByCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return item;
        }

        /// <summary>
        /// Method to add stock item
        /// </summary>
        /// <param name="stockItem">Stock item</param>
        /// <param name="loyalty">Loyalty or not</param>
        public void AddStockItem(StockItem stockItem, bool loyalty)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,AddStockItem,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var stockCode = stockItem.StockCode;
            var query = "select * from StockMst where STOCK_CODE=\'" + stockCode + "\'";
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            var fields = _dataTable.NewRow();
            fields["Stock_Code"] = stockCode;
            fields["Std_Cost"] = 0;
            fields["Avg_Cost"] = 0;
            fields["pr_type"] = "R";
            fields["Descript"] = stockItem.Description.Trim();
            fields["Dept"] = "";
            fields["Sub_Dept"] = "";
            fields["Sub_Detail"] = "";
            fields["Stock_Type"] = "V";
            fields["Brand"] = "";
            fields["Generic"] = "";
            fields["UM"] = "";
            fields["Format"] = "";
            fields["Characteristic"] = "";
            fields["Availability"] = 1;
            fields["SaleCode"] = 0;
            fields["Packing"] = 1;
            fields["Label"] = 0;
            fields["ShelfLabel"] = 0;
            fields["UpdateDate"] = DateAndTime.Today;
            fields["ElgLoyalty"] = loyalty;
            fields["CreationDate"] = DateAndTime.Today;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
           
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            _performancelog.Debug($"End,StockService,AddStockItem,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }


        /// <summary>
        /// Method to add plu mast
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        public void AddPluMast(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,AddPluMast,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var query = "select * from PluMast where plu_code=\'" + stockCode + "\'";
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            DataRow fields = _dataTable.NewRow();
            fields["PLU_Prim"] = stockCode;
            fields["PLU_Code"] = stockCode;
            fields["Plu_Type"] = "S";
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            _performancelog.Debug($"End,StockService,AddPluMast,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to add stock price
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="price">Stock price</param>
        public void AddStockPrice(string stockCode, decimal price)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,AddStockPrice,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var query = "select * from Stock_Prices where stock_code=\'" + stockCode + "\'";
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);


            if (_dataTable.Rows.Count == 0)
            {
                DataRow fields = _dataTable.NewRow();
                fields["Stock_Code"] = stockCode;
                fields["Price_Number"] = 1;
                fields["price"] = Conversion.Val(price);
                fields["VendorID"] = "ALL";
                fields["OrderNo"] = 0;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
            }
            else
            {
                _dataTable.Rows[0]["Stock_Code"] = stockCode;
                _dataTable.Rows[0]["Price_Number"] = 1;
                _dataTable.Rows[0]["price"] = Conversion.Val(price);
                _dataTable.Rows[0]["VendorID"] = "ALL";
                _dataTable.Rows[0]["OrderNo"] = 0;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
            }

            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            _performancelog.Debug($"End,StockService,AddStockPrice,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }


        /// <summary>
        /// Method to add stock branch
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        public void AddStockBranch(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,AddStockBranch,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var query = "select * from stock_BR where stock_code=\'" + stockCode + "\'";

            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);

            if (_dataTable.Rows.Count == 0)
            {
                DataRow fields = _dataTable.NewRow();
                fields["Stock_Code"] = stockCode;
                fields["REORD_QTY"] = 0;
                fields["AVAIL"] = 0;
                fields["branch"] = "01";
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
            }
            else
            {
                _dataTable.Rows[0]["Stock_Code"] = stockCode;
                _dataTable.Rows[0]["REORD_QTY"] = 0;
                _dataTable.Rows[0]["AVAIL"] = 0;
                _dataTable.Rows[0]["branch"] = "01";
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
            }
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            _performancelog.Debug($"End,StockService,AddStockBranch,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Gets the List of Hot Button Pages
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetHotButonPages()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetHotButonPages,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var pages = new Dictionary<int, string>();
            var dt = GetRecords("Select * from HotButtonpages", DataSource.CSCMaster);
            if (dt != null)
            {
                foreach (DataRow fields in dt.Rows)
                {
                    pages.Add(CommonUtility.GetIntergerValue(fields["PageID"]), CommonUtility.GetStringValue(fields["Pagename"]));
                }
            }
            _performancelog.Debug($"End,StockService,GetHotButonPages,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return pages;
        }

        /// <summary>
        /// Gets List of Hot Buttons
        /// </summary>
        /// <param name="firstIndex"></param>
        /// <param name="lastIndex"></param>
        /// <returns></returns>
        public List<HotButton> GetHotButtons(int firstIndex, int lastIndex)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetHotButtons,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            List<HotButton> buttons = new List<HotButton>();
            var dt = GetRecords("SELECT * FROM   HotButtons WHERE  HotButtons.Button >= " + firstIndex + " AND HotButtons.Button <= " + lastIndex + " ORDER BY HotButtons.Button ", DataSource.CSCMaster);

            if (dt != null)
            {
                buttons.AddRange(from DataRow dr in dt.Rows
                                 select new HotButton
                                 {
                                     Button_Number = CommonUtility.GetShortValue(dr["Button"]),
                                     Button_Product = CommonUtility.GetStringValue(dr["Description_1"]) + " " + CommonUtility.GetStringValue(dr["Description_2"]),
                                     StockCode = CommonUtility.GetStringValue(dr["Product"]),
                                     ImageUrl = CommonUtility.GetStringValue(dr["Image_Url"]),
                                     DefaultQuantity = CommonUtility.GetIntergerValue(dr["DefaultQty"]) == 0 ? 1 : CommonUtility.GetIntergerValue(dr["DefaultQty"])
                                 });
            }
            _performancelog.Debug($"End,StockService,GetHotButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return buttons;
        }

        /// <summary>
        /// Method to get stock price
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="priceNumber">Price number</param>
        /// <param name="whereClause"></param>
        /// <returns>Stock price</returns>
        public double? GetStockPriceForPriceNumber(string stockCode, short priceNumber, string whereClause)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetStockPriceForPriceNumber,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var dt = GetRecords("SELECT price FROM Stock_Prices  WHERE Stock_Prices.Stock_Code = \'" + stockCode + "\' AND Stock_Prices.Price_Number = " + Convert.ToString(priceNumber) + " " + whereClause, DataSource.CSCMaster);

            if (dt.Rows.Count != 0)
            {
                _performancelog.Debug($"End,StockService,GetStockPriceForPriceNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return CommonUtility.GetDoubleValue(dt.Rows[0]["price"]);
            }
            _performancelog.Debug($"End,StockService,GetStockPriceForPriceNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return null;
        }

        /// <summary>
        /// Method to get group price head
        /// </summary>
        /// <param name="department">Department</param>
        /// <param name="subDepartment">Sub department</param>
        /// <param name="subDetail">Sub detail</param>
        /// <returns>Group price head</returns>
        public GroupPriceHead GetGroupPriceHead(string department, string subDepartment, string subDetail)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetGroupPriceHead,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var dt = GetRecords("Select * From GP_Head  WHERE Dept = \'" + department + "\' AND  SubDept = \'" + subDepartment + "\' AND  SubDetail = \'" + subDetail + "\' AND  Pr_Type <> \'R\' ", DataSource.CSCMaster);
            GroupPriceHead groupPriceHead = null;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    groupPriceHead = new GroupPriceHead
                    {
                        Department = CommonUtility.GetStringValue(dr["Dept"]),
                        SubDepartment = CommonUtility.GetStringValue(dr["SubDept"]),
                        SubDetail = CommonUtility.GetStringValue(dr["SubDetail"]),
                        PrFrom = CommonUtility.GetDateTimeValue(dr["Pr_From"]),
                        PrTo = CommonUtility.GetDateTimeValue(dr["Pr_To"]),
                        PrType = CommonUtility.GetCharValue(dr["Pr_Type"]),
                        PrUnit = CommonUtility.GetCharValue(dr["Pr_Units"])
                    };
                }
            }
            _performancelog.Debug($"End,StockService,GetGroupPriceHead,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return groupPriceHead;
        }

        /// <summary>
        /// Method to get group price lines 
        /// </summary>
        /// <param name="department">Department</param>
        /// <param name="subDepartment">Sub department</param>
        /// <param name="subDetail">Sub detail</param>
        /// <returns>Group price lines</returns>
        public List<GroupPriceLine> GetGroupPriceLines(string department, string subDepartment, string subDetail)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetGroupPriceLines,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var prices = new List<GroupPriceLine>();
            var dt = GetRecords("Select *  FROM   GP_Lines  WHERE Dept = \'" + department + "\' AND  SubDept = \'" + subDepartment + "\' AND SubDetail = \'" + subDetail + "\' Order By GP_Lines.Pr_F_Qty ", DataSource.CSCMaster);
            if (dt != null)
            {
                prices.AddRange(from DataRow fields in dt.Rows
                                select new GroupPriceLine
                                {
                                    Department = CommonUtility.GetStringValue(fields["Dept"]),
                                    SubDepartment = CommonUtility.GetStringValue(fields["SubDept"]),
                                    SubDetail = CommonUtility.GetStringValue(fields["SubDetail"]),
                                    PrFQty = CommonUtility.GetShortValue(fields["Pr_F_Qty"]),
                                    PrTQty = CommonUtility.GetShortValue(fields["Pr_T_Qty"]),
                                    Price = CommonUtility.GetFloatValue(fields["Price"]),
                                    PrType = CommonUtility.GetCharValue(fields["Pr_Type"])
                                });
            }
            _performancelog.Debug($"End,StockService,GetGroupPriceLines,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return prices;
        }

        /// <summary>
        /// Method to get the maximum gift certificate number
        /// </summary>
        /// <returns>Gift number</returns>
        public int GetMaximumGiftNumber()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetMaximumGiftNumber,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var giftNumber = 1;
            var query = "SELECT MAX(GiftCert.GC_Num) AS [GCN] FROM GiftCert ";
            var dt = GetRecords(query, DataSource.CSCMaster);
            if (dt == null)
            {
                return giftNumber;
            }

            giftNumber = CommonUtility.GetIntergerValue(dt.Rows[0]["GCN"]);
            _performancelog.Debug($"End,StockService,GetMaximumGiftNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return giftNumber;
        }

        /// <summary>
        /// Method to get kit description
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns></returns>
        public string GetKitDescription(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetKitDescription,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var description = string.Empty;
            var dt = GetRecords("SELECT  Descript FROM StockMst WHERE Stock_Code=\'" + stockCode + "\'", DataSource.CSCMaster);
            if (dt != null)
            {
                description = CommonUtility.GetStringValue(dt.Rows[0]["Descript"]);
            }
            _performancelog.Debug($"End,StockService,GetKitDescription,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return description;
        }

        /// <summary>
        /// Method to check if active vendor price is set
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="vendorId">Vendor Id</param>
        /// <returns>True or false</returns>
        public bool IsActiveVendorPrice(string stockCode, string vendorId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,IsActiveVendorPrice,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            bool result;
            string whereVendor;
            if (string.IsNullOrEmpty(vendorId))
            {
                whereVendor = " AND (VendorID=\'ALL\' AND OrderNo=0) ";
                result = true;
            }
            else
            {
                whereVendor = " AND VendorID=\'" + vendorId + "\' AND OrderNo=0 ";
                result = false;
            }
            var ds = GetRecords("SELECT   * FROM     PriceL  WHERE    PriceL.Stock_Code = \'" + stockCode + "\' AND StartDate<=\'" + DateAndTime.Today.ToString("yyyyMMdd") + "\' AND EndDate>=\'" + DateAndTime.Today.ToString("yyyyMMdd") + "\' " + whereVendor + "ORDER BY PriceL.Pr_F_Qty", DataSource.CSCMaster);

            // if no specific special price was set for the active vendor, look for the price applicable to all vendors
            if (ds == null)
            {
                ds = GetRecords("SELECT   * FROM     PriceL  WHERE    PriceL.Stock_Code = \'" + stockCode + "\' AND StartDate<=\'" + DateAndTime.Today.ToString("yyyyMMdd") + "\' AND EndDate>=\'" + DateAndTime.Today.ToString("yyyyMMdd") + "\' AND (VendorID=\'ALL\' AND OrderNo=0) ORDER BY PriceL.Pr_F_Qty", DataSource.CSCMaster);

                if (ds != null)
                {
                    result = true;
                }
            }
            _performancelog.Debug($"End,StockService,IsActiveVendorPrice,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return result;
        }


        /// <summary>
        /// Method to add or update regular price in stock 
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="vendorId"></param>
        /// <param name="price"></param>
        /// <param name="regularPrice"></param>
        public void AddUpdateRegularPrice(string stockCode, ref string vendorId, ref double price, double regularPrice)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,AddUpdateRegularPrice,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            string whereVendorR;
            if (string.IsNullOrEmpty(vendorId))
            {
                whereVendorR = " AND (VendorID=\'ALL\' AND OrderNo=0) ";
            }
            else
            {
                whereVendorR = " AND VendorID=\'" + vendorId + "\' AND OrderNo=0 ";
            }

            var query = "SELECT   * FROM     Stock_Prices WHERE    Stock_Prices.Stock_Code = \'" + stockCode.Trim() + "\' AND  Stock_Prices.Price_Number = 1 " + whereVendorR + "ORDER BY Stock_Prices.Price_Number ";

            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);

            if (_dataTable.Rows.Count == 0)
            {
                query = "SELECT   * FROM     Stock_Prices WHERE    Stock_Prices.Stock_Code = \'" + stockCode.Trim() + "\' AND Stock_Prices.Price_Number = 1 and (VendorID=\'ALL\' AND OrderNo=0) ORDER BY Stock_Prices.Price_Number ";

                _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _dataTable = new DataTable();

                _adapter = new SqlDataAdapter(query, _connection);
                _adapter.Fill(_dataTable);
            }
            if (_dataTable != null && _dataTable.Rows.Count != 0)
            {
                var rsFields = _dataTable.Rows[0];
                price = CommonUtility.GetDoubleValue(rsFields["price"]);
                vendorId = CommonUtility.GetStringValue(rsFields["VendorID"]);
                rsFields["price"] = regularPrice;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
            }
            else
            {
                price = 0;
                var rsFields = _dataTable.NewRow();
                rsFields["Stock_Code"] = stockCode;
                rsFields["Price_Number"] = 1;
                rsFields["price"] = regularPrice;
                if (string.IsNullOrEmpty(vendorId))
                {
                    rsFields["VendorID"] = vendorId;
                }
                rsFields["OrderNo"] = 0;
                _dataTable.Rows.Add(rsFields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
            }

            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            _performancelog.Debug($"End,StockService,AddUpdateRegularPrice,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }


        /// <summary>
        /// Add or Update Special Price
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="activeVendorPrice">Active vendor price</param>
        /// <param name="vendorId">Vendor Id</param>
        /// <param name="priceType">Price type</param>
        /// <param name="gridPrices">Grid prices</param>
        /// <param name="fromdate">Start date</param>
        /// <param name="todate">End dollar</param>
        /// <param name="perDollarChecked">Dollar or percent</param>
        /// <param name="isEndDate">End date exists or not</param>
        public void AddUpdateSpecialPrice(string stockCode, bool activeVendorPrice, ref string vendorId, string priceType, List<PriceGrid> gridPrices, DateTime fromdate, DateTime todate, bool perDollarChecked, bool isEndDate)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,AddUpdateSpecialPrice,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            string sql;
            string whereVendor;
            if (activeVendorPrice)
            {
                whereVendor = " AND (VendorID=\'ALL\' AND OrderNo=0) ";
                vendorId = "ALL";
            }
            else
            {
                whereVendor = " AND VendorID=\'" + vendorId + "\' AND OrderNo=0 ";
            }

            switch (priceType)
            {
                case "S":
                    sql = "Select * From   PriceL Where  PriceL.Stock_Code = \'" + stockCode.Trim() + "\' " + whereVendor;
                    break;
                case "F":
                    sql = "Select * From   PriceL Where  PriceL.Stock_Code = \'" + stockCode.Trim() + "\' " + whereVendor + "Order by PriceL.Pr_F_Qty ";
                    break;
                case "I":
                    sql = "Select * From   PriceL Where  PriceL.Stock_Code = \'" + stockCode.Trim() + "\' " + whereVendor + "Order by PriceL.Pr_F_Qty ";
                    break;
                case "X":
                    sql = "Select * From   PriceL Where  PriceL.Stock_Code = \'" + stockCode.Trim() + "\' " + whereVendor + "Order by PriceL.Pr_F_Qty ";
                    break;
                default:
                    sql = "Select * From   PriceL Where  PriceL.Stock_Code = \'/?/?/?/?/?/?/?/?/?/?/\' " + whereVendor;
                    break;
            }

            if (priceType != "R")
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _dataTable = new DataTable();

                _adapter = new SqlDataAdapter(sql, _connection);
                _adapter.Fill(_dataTable);
                if (_dataTable.Rows.Count == 0)
                {

                    switch (priceType)
                    {
                        case "S":
                            sql = "Select * From PriceL Where PriceL.Stock_Code = \'" + stockCode.Trim() + "\' " + whereVendor;
                            break;
                        case "F":
                            sql = "Select * From PriceL Where PriceL.Stock_Code = \'" + stockCode.Trim() + "\' " + whereVendor + "Order by PriceL.Pr_F_Qty ";
                            break;
                        case "I":
                            sql = "Select * From PriceL Where PriceL.Stock_Code = \'" + stockCode.Trim() + "\' " + whereVendor + "Order by PriceL.Pr_F_Qty ";
                            break;
                        case "X":
                            sql = "Select *  From PriceL Where PriceL.Stock_Code = \'" + stockCode.Trim() + "\' " + whereVendor + "Order by PriceL.Pr_F_Qty ";
                            break;
                        default:
                            sql = "Select * From PriceL Where PriceL.Stock_Code = \'/?/?/?/?/?/?/?/?/?/?/\' " + whereVendor;
                            break;
                    }
                    _adapter = new SqlDataAdapter(sql, _connection);
                    _adapter.Fill(_dataTable);
                }

                foreach (PriceGrid priceGrid in gridPrices)
                {
                    bool addNew = false;
                    short prFQty;
                    if (priceType == "S")
                    {
                        prFQty = 0;
                    }
                    else
                    {
                        prFQty = (short)Conversion.Val(priceGrid.Column1);
                    }
                    var rows = _dataTable.Select("PR_F_Qty=" + Convert.ToString(prFQty));
                    DataRow rsDataSpFields;
                    if (rows.Length == 0)
                    {
                        rsDataSpFields = _dataTable.NewRow();
                        addNew = true;
                    }
                    else
                    {
                        rsDataSpFields = _dataTable.Rows[0];
                    }
                    rsDataSpFields["Stock_Code"] = stockCode.Trim();
                    rsDataSpFields["OrderNo"] = 0;
                    rsDataSpFields["Pr_F_Qty"] = 0;
                    rsDataSpFields["pr_T_Qty"] = 0;
                    if (!string.IsNullOrEmpty(vendorId))
                    {
                        rsDataSpFields["VendorID"] = vendorId;
                    }

                    switch (priceType)
                    {
                        case "S":
                            rsDataSpFields["price"] = priceGrid.Column1;
                            break;
                        case "F":
                            rsDataSpFields["Pr_F_Qty"] = priceGrid.Column1;
                            rsDataSpFields["price"] = priceGrid.Column2;
                            break;
                        case "I":
                            rsDataSpFields["Pr_F_Qty"] = priceGrid.Column1;
                            rsDataSpFields["pr_T_Qty"] = priceGrid.Column2;
                            rsDataSpFields["price"] = priceGrid.Column3;
                            break;
                        case "X":
                            rsDataSpFields["Pr_F_Qty"] = priceGrid.Column1;
                            rsDataSpFields["price"] = priceGrid.Column2;
                            break;
                        default:
                            rsDataSpFields["price"] = priceGrid.Column1;
                            break;
                    }

                    rsDataSpFields["pr_type"] = priceType;
                    rsDataSpFields["StartDate"] = fromdate;
                    rsDataSpFields["Pr_Units"] = perDollarChecked ? "$" : "%";

                    rsDataSpFields["EndDate"] = todate;
                    if (addNew)
                    {
                        _dataTable.Rows.Add(rsDataSpFields);
                        SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                        _adapter.InsertCommand = builder.GetInsertCommand();
                    }
                    else
                    {
                        SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                        _adapter.UpdateCommand = builder.GetUpdateCommand();
                    }
                    _adapter.Update(_dataTable);
                }

            }
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter("select * from Stockmst where stock_code = \'" + stockCode.Trim() + "\'", _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                var rsData1Fields = _dataTable.Rows[0];

                rsData1Fields["pr_type"] = priceType;

                rsData1Fields["Pr_From"] = fromdate;
                rsData1Fields["Pr_Units"] = perDollarChecked ? "$" : "%";
                if (isEndDate == false)
                {
                    rsData1Fields["Pr_To"] = todate;
                }
                else
                {
                    rsData1Fields["Pr_To"] = DBNull.Value;
                }
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            _performancelog.Debug($"End,StockService,AddUpdateSpecialPrice,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }


        /// <summary>
        /// Track Price Change
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="origPrice">Oprignal price</param>
        /// <param name="newPrice">New price</param>
        /// <param name="processType">Process type</param>
        /// <param name="pricenum">Price number</param>
        /// <param name="userCode">User code</param>
        /// <param name="vendorId">Vendor Id</param>
        public void TrackPriceChange(string stockCode, double origPrice, double newPrice,
            string processType, byte pricenum, string userCode, string vendorId = "")
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,TrackPriceChange,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (origPrice != newPrice)
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _dataTable = new DataTable();

                _adapter = new SqlDataAdapter("select * from PriceChange", _connection);
                _adapter.Fill(_dataTable);


                var rsPChangeFields = _dataTable.NewRow();
                rsPChangeFields["TILL_NUM"] = 0;
                rsPChangeFields["Stock_Code"] = stockCode;
                rsPChangeFields["Employeeid"] = userCode;
                rsPChangeFields["OriginalPrice"] = Math.Round(origPrice, 2);
                rsPChangeFields["NewPrice"] = Math.Round(newPrice, 2);
                rsPChangeFields["Date"] = DateAndTime.Today;
                rsPChangeFields["Time"] = DateTime.Now; //Time
                rsPChangeFields["Pricenum"] = pricenum;
                rsPChangeFields["Process"] = processType;
                if (vendorId.Trim().Length > 0)
                {
                    rsPChangeFields["VendorID"] = vendorId;
                }
                _dataTable.Rows.Add(rsPChangeFields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
            }
            _performancelog.Debug($"End,StockService,TrackPriceChange,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }


        /// <summary>
        /// Delete Existing Price Type
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="priceType">Price type</param>
        /// <param name="activeVendorPrice">Active vendor price</param>
        /// <param name="vendorId">Vendor Id</param>
        public void DeletePreviousPrices(string stockCode, string priceType, bool activeVendorPrice,
            string vendorId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,DeletePreviousPrices,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            const char prType = '\0';

            if (prType != priceType[0])
            {
                // once the special price type has been changed, delete the previous special price settings
                string whereVendor;
                if (activeVendorPrice)
                {
                    whereVendor = " AND (VendorID=\'ALL\' AND OrderNo=0) ";
                }
                else
                {
                    whereVendor = " AND VendorID=\'" + vendorId + "\' AND OrderNo=0 ";
                }
                Execute("DELETE FROM Pricel WHERE Stock_Code = \'" + stockCode.Trim() + "\' " + whereVendor, DataSource.CSCMaster);
            }
            _performancelog.Debug($"End,StockService,DeletePreviousPrices,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to get list of vendor coupon
        /// </summary>
        /// <returns>List of vendor coupons</returns>
        public List<VendorCoupon> GetVendorCoupons()
        {
            var coupons = new List<VendorCoupon>();
            var dt = GetRecords("select * from VendorCoupons", DataSource.CSCMaster);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var vc = new VendorCoupon
                    {
                        Code = CommonUtility.GetStringValue(dr["Code"]),
                        Name = CommonUtility.GetStringValue(dr["Name"]),
                        VendorCode = CommonUtility.GetStringValue(dr["VendorCode"]),
                        Value = CommonUtility.GetFloatValue(dr["Value"]),
                        StockCode = CommonUtility.GetStringValue(dr["StockCode"]),
                        Dept = CommonUtility.GetStringValue(dr["Dept"]),
                        SubDept = CommonUtility.GetStringValue(dr["SubDept"]),
                        SubDetail = CommonUtility.GetStringValue(dr["SubDetail"]),
                        TendDesc = CommonUtility.GetStringValue(dr["TendDesc"]),
                        DefaultCoupon = CommonUtility.GetBooleanValue(dr["DefaultCoupon"]),
                        SerNumLen = CommonUtility.GetShortValue(dr["SerialNumLen"])
                    };
                    coupons.Add(vc);
                }
            }
            return coupons;
        }

        /// <summary>
        /// Method to get vendor by code
        /// </summary>
        /// <param name="code">Vendor code</param>
        /// <returns>Vendor</returns>
        public Vendor GetVendorByCode(string code)
        {
            Vendor vendor = null;
            var dt = GetRecords("SELECT * FROM Vendors  WHERE Code=\'" + code + "\'", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                vendor = new Vendor();
                vendor.Code = code;
                vendor.Name = CommonUtility.GetStringValue(dt.Rows[0]["Name"]);
                vendor.Address.Street1 = CommonUtility.GetStringValue(dt.Rows[0]["Address"]);
                vendor.Address.City = CommonUtility.GetStringValue(dt.Rows[0]["City"]);
                vendor.Address.Phones.Add("Phone", "Phone", "", CommonUtility.GetStringValue(dt.Rows[0]["Phone"]), ""); //"Phone","Phone"
                vendor.Address.Country = CommonUtility.GetStringValue(dt.Rows[0]["Country"]);
                vendor.Address.ProvState = CommonUtility.GetStringValue(dt.Rows[0]["Province"]);
                vendor.Address.PostalCode = CommonUtility.GetStringValue(dt.Rows[0]["Postal"]);

            }
            return vendor;
        }

        /// <summary>
        /// Method to get all stock list
        /// </summary>
        /// <returns>List of stock</returns>
        public List<StockBr> GetAllStockBr()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetAllStockBr,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var stocks = new List<StockBr>();
            var query = $"Select * from Stock_Br";
            var dt = GetRecords(query, DataSource.CSCMaster);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    stocks.Add(new StockBr
                    {
                        StockCode = CommonUtility.GetStringValue(dr["Stock_Code"]),
                        ActiveStock = DBNull.Value.Equals(dr["Activestock"]) || CommonUtility.GetBooleanValue(dr["Activestock"]),
                        AvalItems = CommonUtility.GetDoubleValue(dr["AVAIL"])
                    });
                }
            }
            _performancelog.Debug($"End,StockService,GetStockBr,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return stocks;
        }

        /// <summary>
        /// Method to get stock by stock code
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Stock information</returns>
        public StockInfo GetStockByStockCode(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetPluMast,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            StockInfo stockInfo = null;
            if (string.IsNullOrEmpty(stockCode))
            {
                return null;
            }
            string query = $"select * from Plumast as p left join STOCKMST as S on p.PLU_PRIM = s.STOCK_CODE where PLU_CODE = '{stockCode}'";
            var dt = GetRecords(query, DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                stockInfo = new StockInfo
                {
                    PluCode = CommonUtility.GetStringValue(dt.Rows[0]["plu_code"]),
                    StockCode = CommonUtility.GetStringValue(dt.Rows[0]["Stock_Code"]),
                    PLUPrim = CommonUtility.GetStringValue(dt.Rows[0]["PLU_PRIM"]),
                    PLUType = CommonUtility.GetCharValue(dt.Rows[0]["PLU_TYPE"]),
                    Description = CommonUtility.GetStringValue(dt.Rows[0]["DESCRIPT"]),
                    StockType = CommonUtility.GetCharValue(dt.Rows[0]["STOCK_TYPE"]),
                    ProductDescription = CommonUtility.GetShortValue(dt.Rows[0]["PROD_DISC"]),
                    StandardCost = CommonUtility.GetDoubleValue(dt.Rows[0]["STD_COST"]),
                    AverageCost = CommonUtility.GetDoubleValue(dt.Rows[0]["AVG_COST"]),
                    Department = CommonUtility.GetStringValue(dt.Rows[0]["DEPT"]),
                    SubDepartment = CommonUtility.GetStringValue(dt.Rows[0]["SUB_DEPT"]),
                    SubDetail = CommonUtility.GetStringValue(dt.Rows[0]["Sub_Detail"]),
                    SByWeight = CommonUtility.GetBooleanValue(dt.Rows[0]["S_BY_WGHT"]),
                    Vendor = CommonUtility.GetStringValue(dt.Rows[0]["Vendor"]),
                    PRType = CommonUtility.GetCharValue(dt.Rows[0]["PR_TYPE"]),
                    PRUnit = CommonUtility.GetCharValue(dt.Rows[0]["PR_UNITS"]),
                    PRFrom = CommonUtility.GetDateTimeValue(dt.Rows[0]["PR_FROM"]),
                    PRTo = CommonUtility.GetDateTimeValue(dt.Rows[0]["PR_TO"]),
                    VendorNumber = CommonUtility.GetStringValue(dt.Rows[0]["VendorNumber"]),
                    Availability = CommonUtility.GetBooleanValue(dt.Rows[0]["Availability"]),
                    UM = CommonUtility.GetStringValue(dt.Rows[0]["UM"]),
                    EligibleLoyalty = CommonUtility.GetBooleanValue(dt.Rows[0]["ElgLoyalty"]),
                    EligibleFuelRebate = CommonUtility.GetBooleanValue(dt.Rows[0]["ElgFuelRebate"]),
                    FuelRebate = CommonUtility.GetDecimalValue(dt.Rows[0]["FuelRebate"]),
                    QualtaxRebate = CommonUtility.GetBooleanValue(dt.Rows[0]["QualTaxRebate"]),
                    EligibletaxRebate = CommonUtility.GetBooleanValue(dt.Rows[0]["ElgTaxRebate"]),
                    EligibleTaxExemption = CommonUtility.GetBooleanValue(dt.Rows[0]["ElgTaxExemption"])
                };
            }
            _performancelog.Debug($"End,StockService,GetPluMast,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return stockInfo;
        }

        /// <summary>
        /// Method to get saleline by stock code
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Sale line</returns>
        public Sale_Line GetStockInfoByStockCode(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetStockInfoByStockCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            string query = $"select p.plu_code,p.plu_prim,p.plu_type,s.stock_type,s.Dept,s.SUB_DEPT,s.Sub_Detail,s.Vendor from Plumast as p left join STOCKMST as S on p.PLU_PRIM = s.STOCK_CODE where PLU_CODE = '{stockCode}'";

            var dt = GetRecords(query, DataSource.CSCMaster);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            var stockInfo = new Sale_Line
            {
                PLU_Code = CommonUtility.GetStringValue(dt.Rows[0]["plu_code"]),
                Stock_Code = CommonUtility.GetStringValue(dt.Rows[0]["PLU_PRIM"]),
                PluType = CommonUtility.GetCharValue(dt.Rows[0]["PLU_TYPE"]),
                Stock_Type = CommonUtility.GetCharValue(dt.Rows[0]["STOCK_TYPE"]),
                Dept = CommonUtility.GetStringValue(dt.Rows[0]["DEPT"]),
                Sub_Dept = CommonUtility.GetStringValue(dt.Rows[0]["SUB_DEPT"]),
                Sub_Detail = CommonUtility.GetStringValue(dt.Rows[0]["Sub_Detail"]),
                Vendor = CommonUtility.GetStringValue(dt.Rows[0]["Vendor"])
            };
            _performancelog.Debug($"End,StockService,GetStockInfoByStockCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return stockInfo;
        }

        /// <summary>
        /// Method to get sale line info
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns></returns>
        public Sale_Line GetSaleLineInfo(string stockCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,StockService,GetSaleLineInfo,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");


            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            SqlCommand cmd = new SqlCommand("VerifyStockConstraints", _connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@StockCode", Value = stockCode });
            var rdr = cmd.ExecuteReader();
            
            if (!rdr.HasRows)
            {
                return null;
            }
            var stockInfo = new Sale_Line();
            if (rdr.Read())
            {

                stockInfo.PLU_Code = CommonUtility.GetStringValue(rdr["plu_code"]);
                stockInfo.Stock_Code = CommonUtility.GetStringValue(rdr["PLU_PRIM"]);
                stockInfo.PluType = CommonUtility.GetCharValue(rdr["PLU_TYPE"]);
                stockInfo.Stock_Type = CommonUtility.GetCharValue(rdr["STOCK_TYPE"]);
                stockInfo.Dept = CommonUtility.GetStringValue(rdr["DEPT"]);
                stockInfo.Sub_Dept = CommonUtility.GetStringValue(rdr["SUB_DEPT"]);
                stockInfo.Sub_Detail = CommonUtility.GetStringValue(rdr["Sub_Detail"]);
                stockInfo.Vendor = CommonUtility.GetStringValue(rdr["Vendor"]);
                var price = CommonUtility.GetStringValue(rdr["Price"]);

                if (string.IsNullOrEmpty(price))
                {
                    stockInfo.IsPriceSet = false;
                }
                else
                {
                    stockInfo.Regular_Price = CommonUtility.GetDoubleValue(price);
                    stockInfo.IsPriceSet = true;
                }

                var fuelType = CommonUtility.GetStringValue(rdr["FuelType"]);
                if (!string.IsNullOrEmpty(fuelType))
                {
                    stockInfo.ProductIsFuel = true;
                    stockInfo.IsPropane = fuelType == "O";
                }
                else
                {
                    stockInfo.ProductIsFuel = false;
                    stockInfo.IsPropane = false;
                }
                if (!DBNull.Value.Equals(rdr["ActiveDayCount"]))
                {
                    if (!CommonUtility.GetBooleanValue(rdr["ActiveToday"]))
                    {
                        stockInfo.Active_StockCode = true;
                        stockInfo.Active_DayOfWeek = false;
                    }
                    else
                    {
                        stockInfo.Active_DayOfWeek = true;
                    }
                }
                else
                {
                    stockInfo.Active_DayOfWeek = true;
                }
                if (!DBNull.Value.Equals(rdr["Activestock"]))
                {
                    stockInfo.Active_StockCode = CommonUtility.GetBooleanValue(rdr["Activestock"]);
                }
                else
                {
                    stockInfo.Active_StockCode = true;
                }

                if (stockInfo.Stock_Type == 'V' || stockInfo.Stock_Type == 'O')
                {
                    stockInfo.AvailItems = CommonUtility.GetDoubleValue(rdr["AVAIL"]);
                }
            }
            _performancelog.Debug($"End,StockService,GetSaleLineInfo,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return stockInfo;
        }

        /// <summary>
        /// Method to get list of vendors
        /// </summary>
        /// <returns>List of vendors</returns>
        public List<Vendor> GetAllVendors()
        {
            var vendors = new List<Vendor>();
            var rsVendors = GetRecords("SELECT Code, Name FROM Vendors", DataSource.CSCMaster);
            foreach (DataRow vendor in rsVendors.Rows)
            {
                vendors.Add(new Vendor
                {
                    Code = CommonUtility.GetStringValue(vendor["Code"]),
                    Name = CommonUtility.GetStringValue(vendor["Name"])
                });
            }
            return vendors;
        }
    }
}

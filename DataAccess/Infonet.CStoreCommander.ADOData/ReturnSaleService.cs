using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;

namespace Infonet.CStoreCommander.ADOData
{
    public class ReturnSaleService : SqlDbService, IReturnSaleService
    {
        private readonly ICustomerService _customerService;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;


        public ReturnSaleService(
            ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Get All Sales
        /// </summary>
        /// <param name="saleDate">Sale date</param>
        /// <param name="timeFormat">Time format</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of sale head</returns>
        public List<SaleHead> GetAllSales(DateTime saleDate, string timeFormat, int pageIndex, int pageSize)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleService,GetAllSales,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var salesHeads = new List<SaleHead>();
            string strSql;
            if (timeFormat == "24 HOURS") //to format the time according to the policy
            {
                strSql = "(SELECT Till, Sale_No AS [Sale No], Sale_Date AS [Date], convert(varchar,Sale_time, 108) AS [Time] , Sale_Amt AS [Amount] FROM SaleHead A WHERE A.T_Type IN ( \'SALE\' ,\'REFUND\', \'RUNAWAY\', \'PUMPTEST\' ) and A.SALE_AMT <> 0 and A.Sale_Date >= \'" + saleDate.ToString("yyyyMMdd") + "\' UNION ALL SELECT Till, Sale_No AS [Sale No], Sale_Date AS [Date], convert(varchar,Sale_time, 108) AS [Time] , Sale_Amt AS [Amount] FROM CSCTRANS.dbo.SaleHead  A WHERE A.T_Type IN ( \'SALE\' ,\'REFUND\', \'RUNAWAY\' , \'PUMPTEST\') and A.SALE_AMT <> 0 and A.Sale_Date >= \'" + saleDate.ToString("yyyyMMdd") + "\') ORDER BY Sale_No DESC";
            }
            else
            {
                strSql = "(SELECT Till, Sale_No AS [Sale No], Sale_Date AS [Date], RIGHT(CONVERT(CHAR(19),sale_time,100),7) AS [Time] , Sale_Amt AS [Amount] FROM SaleHead A WHERE A.T_Type IN ( \'SALE\' ,\'REFUND\' , \'RUNAWAY\', \'PUMPTEST\') and A.SALE_AMT <> 0 and A.Sale_Date >= \'" + saleDate.ToString("yyyyMMdd") + "\' UNION ALL SELECT Till, Sale_No AS [Sale No], Sale_Date AS [Date], RIGHT(CONVERT(CHAR(19),sale_time,100),7) AS [Time] , Sale_Amt AS [Amount] FROM CSCTRANS.dbo.SaleHead  A WHERE A.T_Type IN ( \'SALE\' ,\'REFUND\', \'RUNAWAY\' , \'PUMPTEST\') and A.SALE_AMT <> 0 and A.Sale_Date >= \'" + saleDate.ToString("yyyyMMdd") + "\') ORDER BY Sale_No DESC";
            }
            var rsSales = GetPagedRecords(strSql, DataSource.CSCTills, pageIndex, pageSize);
            foreach (DataRow fields in rsSales.Rows)
            {
                salesHeads.Add(new SaleHead
                {
                    TillNumber = CommonUtility.GetIntergerValue(fields["Till"]),
                    SaleNumber = CommonUtility.GetIntergerValue(fields["Sale No"]),
                    SaleDate = CommonUtility.GetDateTimeValue(fields["Date"]),
                    SaleTime = CommonUtility.GetDateTimeValue(fields["Time"]),
                    SaleAmount = CommonUtility.GetDecimalValue(fields["Amount"])
                });
            }

            _performancelog.Debug($"End,ReturnSaleService,GetAllSales,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return salesHeads;
        }


        /// <summary>
        /// Search Sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="slDate">Sale time</param>
        /// <param name="timeFormat">Time format</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of sale head</returns>
        public List<SaleHead> SearchSale(int? saleNumber, DateTime? saleDate, DateTime slDate, string timeFormat, int pageIndex, int pageSize)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleService,SearchSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var salesHeads = new List<SaleHead>();
            string strSql;
            string dateCheck = string.Empty;
            string saleCheck = string.Empty;
            if (saleDate.HasValue)
            {
                dateCheck = "AND A.Sale_Date = \'" + saleDate.Value.ToString("yyyyMMdd") + "\'";
            }
            if (saleNumber.HasValue)
            {
                saleCheck = " AND Sale_No LIKE \'%" + saleNumber + "%\'";
            }
            if (timeFormat == "24 HOURS") //to format the time according to the policy
            {
                strSql = "SELECT Till, Sale_No AS [Sale No], Sale_Date AS [Date], convert(varchar,Sale_time, 108) AS [Time] , Sale_Amt AS [Amount] FROM SaleHead A WHERE A.T_Type IN ( \'SALE\' ,\'REFUND\', \'RUNAWAY\', \'PUMPTEST\' ) and A.SALE_AMT <> 0 AND A.Sale_Date >= \'" + slDate.ToString("yyyyMMdd") + "\' " + dateCheck + saleCheck + " UNION ALL SELECT Till, Sale_No AS [Sale No], Sale_Date AS [Date], convert(varchar,Sale_time, 108) AS [Time] , Sale_Amt AS [Amount] FROM CSCTRANS.dbo.SaleHead  A WHERE A.T_Type IN ( \'SALE\' ,\'REFUND\', \'RUNAWAY\' , \'PUMPTEST\') and A.SALE_AMT <> 0 AND A.Sale_Date >= \'" + slDate.ToString("yyyyMMdd") + "\' " + dateCheck + saleCheck;
            }
            else
            {
                strSql = "SELECT Till, Sale_No AS [Sale No], Sale_Date AS [Date], RIGHT(CONVERT(CHAR(19),sale_time,100),7) AS [Time] , Sale_Amt AS [Amount] FROM SaleHead A WHERE A.T_Type IN ( \'SALE\' ,\'REFUND\' , \'RUNAWAY\', \'PUMPTEST\') and A.SALE_AMT <> 0 AND A.Sale_Date >= \'" + slDate.ToString("yyyyMMdd") + "\' " + dateCheck + saleCheck + " UNION ALL SELECT Till, Sale_No AS [Sale No], Sale_Date AS [Date], RIGHT(CONVERT(CHAR(19),sale_time,100),7) AS [Time] , Sale_Amt AS [Amount] FROM CSCTRANS.dbo.SaleHead  A WHERE A.T_Type IN ( \'SALE\' ,\'REFUND\', \'RUNAWAY\' , \'PUMPTEST\') and A.SALE_AMT <> 0 AND A.Sale_Date >= \'" + slDate.ToString("yyyyMMdd") + "\' " + dateCheck + saleCheck;
            }

            var rsSales = GetPagedRecords(strSql, DataSource.CSCTills, pageIndex, pageSize);
            foreach (DataRow fields in rsSales.Rows)
            {
                salesHeads.Add(new SaleHead
                {
                    TillNumber = CommonUtility.GetIntergerValue(fields["Till"]),
                    SaleNumber = CommonUtility.GetIntergerValue(fields["Sale No"]),
                    SaleDate = CommonUtility.GetDateTimeValue(fields["Date"]),
                    SaleTime = CommonUtility.GetDateTimeValue(fields["Time"]),
                    SaleAmount = CommonUtility.GetDecimalValue(fields["Amount"])
                });
            }
            _performancelog.Debug($"End,ReturnSaleService,SearchSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return salesHeads;
        }

        /// <summary>
        /// Gets the List of Sale Lines
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="discountType">Discount type</param>
        /// <param name="teType">Tax exempt type</param>
        /// <param name="taxExempt">Tax exempt name</param>
        /// <returns>List of sale lines</returns>
        public List<Sale_Line> GetSaleLineBySaleNumber(int saleNumber, int tillNumber, DateTime saleDate, string discountType, string teType, bool taxExempt)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleService,GetSaleLineBySaleNumber,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            List<Sale_Line> saleLines = new List<Sale_Line>();
            DataSource db = DataSource.CSCTrans;

            var strSqlHead = " SELECT * FROM   SaleHead Where  SaleHead.Sale_No = " + CommonUtility.GetStringValue(saleNumber) + " AND SaleHead.TILL = " + CommonUtility.GetStringValue(tillNumber) + " AND SaleHead.T_Type IN ( \'SALE\' ,\'REFUND\', \'RUNAWAY\',\'PUMPTEST\' ) and SaleHead.SALE_AMT <> 0 ";
            var strSqlLine = " SELECT * FROM   SaleLine Where SaleLine.Sale_No = " + CommonUtility.GetStringValue(saleNumber) + " AND SaleLine.TILL_NUM = " + CommonUtility.GetStringValue(tillNumber) + "ORDER BY SaleLine.Line_Num ";

            var rsHead = GetRecords(strSqlHead, DataSource.CSCTrans);
            var rsLine = GetRecords(strSqlLine, DataSource.CSCTrans);

            // If SaleNo is not found in Trans.mdb database, then
            // look for this SaleNo in all active Tills
            if (rsHead == null || rsHead.Rows.Count == 0)
            {
                rsHead = GetRecords(strSqlHead, DataSource.CSCTills);
                rsLine = GetRecords(strSqlLine, DataSource.CSCTills);
                // If sale number is found exit for do and use this recordset
                if (rsHead != null && rsHead.Rows.Count != 0)
                {
                    db = DataSource.CSCTills;
                }
            }
            else
            {
                db = DataSource.CSCTrans;
            }
            foreach (DataRow rsLineFields in rsLine.Rows)
            {

                var saleLine = new Sale_Line
                {
                    No_Loading = true,
                    Stock_Code = CommonUtility.GetStringValue(rsLineFields["Stock_Code"]),
                    PLU_Code = CommonUtility.GetStringValue(rsLineFields["PLU_Code"]),
                    Line_Num = CommonUtility.GetShortValue(rsLineFields["Line_Num"]),
                    Price_Type = CommonUtility.GetCharValue(rsLineFields["Price_Type"]),
                    Dept = CommonUtility.GetStringValue(rsLineFields["Dept"]),
                    Sub_Detail = CommonUtility.GetStringValue(rsLineFields["Sub_Detail"]),
                    Sub_Dept = CommonUtility.GetStringValue(rsLineFields["Sub_Dept"]),
                    Quantity = CommonUtility.GetFloatValue(rsLineFields["Quantity"]) * -1,
                    price = CommonUtility.GetDoubleValue(rsLineFields["price"]),
                    Regular_Price = CommonUtility.GetDoubleValue(rsLineFields["Reg_Price"]),
                    Cost = CommonUtility.GetDoubleValue(rsLineFields["Cost"]),
                    Amount = CommonUtility.GetDecimalValue(rsLineFields["Amount"]) * -1,
                    Total_Amount = CommonUtility.GetDecimalValue(rsLineFields["Total_Amt"]) * -1
                };
                
                if (!(teType == "QITE" && discountType == "D"))
                {
                    saleLine.Line_Discount = CommonUtility.GetDoubleValue(rsLineFields["Discount"]) * -1;
                }
                saleLine.Discount_Type = CommonUtility.GetStringValue(rsLineFields["Disc_Type"]);
                saleLine.Discount_Code = CommonUtility.GetStringValue(rsLineFields["Disc_Code"]);
                if (!(teType == "QITE" && discountType == "D"))
                {
                    SetDiscountRate(ref saleLine, CommonUtility.GetFloatValue(rsLineFields["Disc_Rate"]));
                }
                saleLine.Associate_Amount = CommonUtility.GetDecimalValue(rsLineFields["Assoc_Amt"]);
                saleLine.Description = CommonUtility.GetStringValue(rsLineFields["Descript"]);
                saleLine.Loyalty_Save = CommonUtility.GetFloatValue(rsLineFields["Loyl_Save"]);
                saleLine.Units = CommonUtility.GetStringValue(rsLineFields["Units"]);
                saleLine.Serial_No = CommonUtility.GetStringValue(rsLineFields["Serial_No"]);

                saleLine.Stock_Code = CommonUtility.GetStringValue(rsLineFields["Stock_Code"]);
                saleLine.pumpID = CommonUtility.GetByteValue(rsLineFields["pumpID"]);
                saleLine.PositionID = CommonUtility.GetByteValue(rsLineFields["PositionID"]);

                saleLine.GradeID = CommonUtility.GetByteValue(rsLineFields["GradeID"]);

                saleLine.Prepay = CommonUtility.GetBooleanValue(rsLineFields["Prepay"]);

                saleLine.ManualFuel = CommonUtility.GetBooleanValue(rsLineFields["ManualFuel"]);

                saleLine.IsTaxExemptItem = CommonUtility.GetBooleanValue(rsLineFields["TaxExempt"]);

                saleLine.Gift_Num = CommonUtility.GetStringValue(rsLineFields["GC_Num"]);

                saleLine.FuelRebateEligible = CommonUtility.GetBooleanValue(rsLineFields["FuelRebateUsed"]);

                saleLine.FuelRebate = CommonUtility.GetDecimalValue(rsLineFields["RebateDiscount"]);

                saleLine.EligibleTaxEx = CommonUtility.GetBooleanValue(rsLineFields["ElgTaxExemption"]);

                saleLine.CarwashCode = CommonUtility.GetStringValue(rsLineFields["CarwashCode"]);

                if (saleLine.CarwashCode != "") 
                {
                    saleLine.IsCarwashProduct = true;
                }

                if (saleLine.IsTaxExemptItem)
                {
                    if (teType == "SITE")
                    {
                        var rsPurchaseItem = GetRecords(
                            "select * from PurchaseItem Where Sale_No=" + CommonUtility.GetStringValue(saleNumber) +
                            " AND Line_No=" + CommonUtility.GetStringValue(saleLine.Line_Num), db);
                        if (rsPurchaseItem != null && rsPurchaseItem.Rows.Count != 0)
                        {
                            var rsPurchaseItemFields = rsPurchaseItem.Rows[0];

                            saleLine.OriginalPrice =
                                CommonUtility.GetFloatValue(rsPurchaseItemFields["OriginalPrice"]);
                            saleLine.TaxInclPrice = CommonUtility.GetFloatValue(rsPurchaseItemFields["Amount"]);
                        }
                        else
                        {
                            saleLine.OriginalPrice = (float)saleLine.price;
                            saleLine.TaxInclPrice = (float)(-1 * saleLine.Amount);
                        }
                    }
                    else 
                    {
                        var rsTeSaleLine = GetRecords(
                            "select * from TaxExemptSaleLine Where SALE_NO=" +
                            CommonUtility.GetStringValue(saleNumber) + " AND LINE_NUM=" +
                            CommonUtility.GetStringValue(saleLine.Line_Num), db);

                        if (rsTeSaleLine != null && rsTeSaleLine.Rows.Count != 0)
                        {
                            
                            var rsTeSaleLineFields = rsTeSaleLine.Rows[0];

                            saleLine.OriginalPrice =
                                CommonUtility.GetFloatValue(rsTeSaleLineFields["OriginalPrice"]);

                            saleLine.TaxInclPrice =
                                CommonUtility.GetFloatValue(-1 *
                                                            CommonUtility.GetIntergerValue(
                                                                rsTeSaleLineFields["TaxIncludedAmount"]));
                        }
                        else
                        {
                            saleLine.OriginalPrice = (float)saleLine.price;
                            saleLine.TaxInclPrice = (float)(-1 * saleLine.Amount);
                        }

                        saleLine.IsTaxExemptItem = false;

                    }
                }


                string strPromo = CommonUtility.GetStringValue(rsLineFields["PromoID"]);
                if (strPromo.Length != 0)
                {
                    saleLine.PromoID = strPromo;
                }


                saleLine.Line_Taxes = null;
                var rsLineTax =
                    GetRecords(
                        "Select * From   S_LineTax  WHERE  S_LineTax.Sale_No = " +
                        CommonUtility.GetStringValue(saleNumber) + " AND  S_LineTax.Line_No = " +
                        CommonUtility.GetStringValue(rsLineFields["Line_Num"]) + " " +
                        "Order By S_LineTax.Tax_Name ", db);
                if (rsLineTax == null || rsLineTax.Rows.Count == 0)
                {
                    if (taxExempt && (teType == "AITE" || teType == "QITE"))
                    {
                        rsLineTax =
                            GetRecords(
                                "Select * From   TaxCreditLine  WHERE  TaxCreditLine.Sale_No = " +
                                CommonUtility.GetStringValue(saleNumber) + " AND  TaxCreditLine.Line_No = " +
                                CommonUtility.GetStringValue(rsLineFields["Line_Num"]) + " " +
                                "Order By TaxCreditLine.Tax_Name ", db);
                        if (!saleLine.IsTaxExemptItem && rsLineTax.Rows.Count != 0)
                        {

                        }
                    }
                }

                foreach (DataRow rsLineTaxFields in rsLineTax.Rows)
                {

                    saleLine.Line_Taxes.Add(CommonUtility.GetStringValue(rsLineTaxFields["Tax_Name"]),
                        CommonUtility.GetStringValue(rsLineTaxFields["Tax_Code"]),
                        CommonUtility.GetFloatValue(rsLineTaxFields["Tax_Rate"]),
                        CommonUtility.GetBooleanValue(rsLineTaxFields["Tax_Included"]),
                        CommonUtility.GetFloatValue(rsLineTaxFields["Tax_Rebate_Rate"]),
                        CommonUtility.GetDecimalValue(rsLineTaxFields["Tax_Rebate"]), "");
                }

                // Similarly, pick up the charges associated with the line.
                saleLine.Charges = null;
                var rsLineChg =
                    GetRecords(
                        "Select *  FROM   SaleChg  WHERE  SaleChg.Sale_No = " +
                        CommonUtility.GetStringValue(saleNumber) + " AND SaleChg.Line_No = " +
                        CommonUtility.GetStringValue(rsLineFields["Line_Num"]) + " Order By SaleChg.As_Code ",
                        db);
                foreach (DataRow linChrField in rsLineChg.Rows)
                {

                    var rsLcTax =
                        GetRecords(
                            "Select *  FROM   ChargeTax  WHERE  ChargeTax.Sale_No = " +
                            CommonUtility.GetStringValue(saleNumber) + " AND ChargeTax.Line_No = " +
                            CommonUtility.GetStringValue(rsLineFields["Line_Num"]) + " AND ChargeTax.As_Code = \'" +
                            CommonUtility.GetStringValue(linChrField["As_Code"]) + "\' ", db);
                    // Find any taxes that applied to those charges.
                    var lct = new Charge_Taxes();
                    foreach (DataRow rsLcTaxFields in rsLcTax.Rows)
                    {
                        lct.Add(CommonUtility.GetStringValue(rsLcTaxFields["Tax_Name"]),
                            CommonUtility.GetStringValue(rsLcTaxFields["Tax_Code"]),
                            CommonUtility.GetFloatValue(rsLcTaxFields["Tax_Rate"]),
                            CommonUtility.GetBooleanValue(rsLcTaxFields["Tax_Included"]), "");
                    }

                    saleLine.Charges.Add(CommonUtility.GetStringValue(linChrField["As_Code"]),
                        CommonUtility.GetStringValue(linChrField["Description"]),
                        CommonUtility.GetFloatValue(linChrField["price"]), lct, "");
                }


                saleLine.Line_Kits = GetLineKits(saleNumber,
                    CommonUtility.GetIntergerValue(rsLineFields["Line_Num"]), db);
                saleLines.Add(saleLine);
            }
            _performancelog.Debug($"End,ReturnSaleService,GetSaleLineBySaleNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return saleLines;
        }

        /// <summary>
        /// Checks whether correction is allowed
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>True or false</returns>
        public bool IsAllowCorrection(int saleNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleService,IsAllowCorrection,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            //#: we have a transaction processed. We want to do void of it. After voiding the original transaction shouldn’t be available for another void.
            var strSql = "Select SALE_NO From VoidSale Where Sale_No = " + Convert.ToString(saleNumber) + " OR Void_No = " + Convert.ToString(saleNumber);
            var rsTmp = GetRecords(strSql, DataSource.CSCTrans);
            if (rsTmp == null || rsTmp.Rows.Count == 0)
            {
                rsTmp = GetRecords(strSql, DataSource.CSCTills);
            }

            
            
            if (rsTmp != null && rsTmp.Rows.Count != 0)
            {
                return false;
            }

            
            strSql = "Select TENDCLAS From SaleTend Where Sale_No = " + Convert.ToString(saleNumber) + " AND AMTTEND<>0";
            rsTmp = GetRecords(strSql, DataSource.CSCTrans);
            if (rsTmp == null || rsTmp.Rows.Count == 0)
            {
                rsTmp = GetRecords(strSql, DataSource.CSCTills);
            }

            if (rsTmp == null || rsTmp.Rows.Count == 0)
            {
                return false;
            }

            
            if (rsTmp.Rows.Count != 1)
            {
                return false;
            }
            var fields = rsTmp.Rows[0];
            
            if (CommonUtility.GetStringValue(fields["TENDCLAS"]).ToUpper() != "CRCARD" && CommonUtility.GetStringValue(fields["TENDCLAS"]).ToUpper() != "DBCARD")
            {
                _performancelog.Debug($"End,ReturnSaleService,IsAllowCorrection,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return false;
            }
            _performancelog.Debug($"End,ReturnSaleService,IsAllowCorrection,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return true;
        }


        /// <summary>
        /// Checks whether reason allowed
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>List of sale lines</returns>
        public List<Sale_Line> IsReasonAllowed(int saleNumber)
        {
            List<Sale_Line> result = new List<Sale_Line>();
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleService,IsReasonAllowed,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var strSql = "SELECT * FROM SaleLine WHERE SALE_NO =\'" + saleNumber + "\' UNION ALL SELECT * FROM CSCTRANS.dbo.SaleLine WHERE SALE_NO =\'" + saleNumber + "\'";

            var rsSaleLines = GetRecords(strSql, DataSource.CSCTills);
            foreach (DataRow fields in rsSaleLines.Rows)
            {
                Sale_Line saleLine = new Sale_Line();
                {
                    saleLine.Stock_Code = CommonUtility.GetStringValue(fields["Stock_Code"]);
                    saleLine.Dept = CommonUtility.GetStringValue(fields["Dept"]);
                    saleLine.Sub_Detail = CommonUtility.GetStringValue(fields["Sub_Detail"]);
                    saleLine.Sub_Dept = CommonUtility.GetStringValue(fields["Sub_Dept"]);
                }
                result.Add(saleLine);
            }
            _performancelog.Debug($"End,ReturnSaleService,IsReasonAllowed,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return result;
        }

        /// <summary>
        /// Get SaleBy SaleNumber
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="defaultCustCode">Default customer code</param>
        /// <param name="isSaleFound">Sale found or not</param>
        /// <param name="isReturnable">Sale returned or not</param>
        /// <param name="teType">Taxe exempt type</param>
        /// <param name="teGetName">Tax exempt name</param>
        /// <param name="taxExemptGa">Tax exempt ga</param>
        /// <param name="pDefaultCustomer">Deafult customer</param>
        /// <returns>Sale</returns>
        public Sale GetSaleBySaleNumber(int saleNumber, int tillNumber, DateTime saleDate, string teType, bool teGetName, bool taxExemptGa, bool pDefaultCustomer, string defaultCustCode, out bool isSaleFound, out bool isReturnable)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleService,GetSaleBySaleNumber,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            DataSource db = DataSource.CSCTrans;
            bool saleFound = false;
            Sale sale = new Sale();
            isSaleFound = true;
            isReturnable = true;

            var strSqlHead = " SELECT * FROM   SaleHead Where  SaleHead.Sale_No = " + CommonUtility.GetStringValue(saleNumber) + " AND SaleHead.TILL = " + CommonUtility.GetStringValue(tillNumber) + " AND SaleHead.T_Type IN ( \'SALE\' ,\'REFUND\', \'RUNAWAY\',\'PUMPTEST\' ) and SaleHead.SALE_AMT <> 0 ";
            var rsHead = GetRecords(strSqlHead, DataSource.CSCTrans);

            // If SaleNo is not found in Trans.mdb database, then
            // look for this SaleNo in all active Tills
            if (rsHead == null || rsHead.Rows.Count == 0)
            {
                rsHead = GetRecords(strSqlHead, DataSource.CSCTills);
                // If sale number is found exit for do and use this recordset
                if (rsHead != null && rsHead.Rows.Count != 0)
                {
                    saleFound = true;
                    db = DataSource.CSCTills;
                }
            }
            else
            {
                saleFound = true;
                db = DataSource.CSCTrans;
            }

            if (!saleFound)
            {
                isSaleFound = false;
                isReturnable = false;
                return sale;
            }
            var rsHeadFields = rsHead.Rows[0];

            if (saleDate != new DateTime() && CommonUtility.GetDateTimeValue(rsHeadFields["Sale_Date"]) < saleDate)
            {
                isReturnable = false;
                return sale;
            }
            sale.LoadingTemp = true;
            sale.TillNumber = CommonUtility.GetByteValue(tillNumber);
            sale.Sale_Num = saleNumber;
            sale.Sale_Amount = CommonUtility.GetDecimalValue(rsHeadFields["SALE_AMT"]);
            sale.Sale_Invc_Disc = CommonUtility.GetDecimalValue(rsHeadFields["INVC_DISC"]);
            sale.TotalTaxSaved = CommonUtility.GetFloatValue(CommonUtility.GetDecimalValue(rsHeadFields["SALE_AMT"]) - CommonUtility.GetDecimalValue(rsHeadFields["TEND_AMT"]));
            sale.Customer = LoadCustomer(CommonUtility.GetStringValue(rsHeadFields["Client"]), pDefaultCustomer, defaultCustCode);

            if ((CommonUtility.GetStringValue(rsHeadFields["T_type"]) == "RUNAWAY" || CommonUtility.GetStringValue(rsHeadFields["T_type"]) == "PUMPTEST") && CommonUtility.GetDecimalValue(rsHeadFields["Sale_amt"]) < 0)
            {
                sale.Sale_Type = "SALE";
            }
            else
            {
                sale.Sale_Type = CommonUtility.GetStringValue(rsHeadFields["T_type"]);
            }
            sale.Void_Num = saleNumber;
            sale.Sale_Deposit = CommonUtility.GetDecimalValue(rsHeadFields["Deposit"]);

            sale.TreatyNumber = CommonUtility.GetStringValue(rsHeadFields["TreatyNumber"]);
            if (teType != "AITE" && teType != "QITE" && teGetName)
            {
                sale.TreatyName = GetTreatyName(CommonUtility.GetStringValue(rsHeadFields["TreatyNumber"]));
            }

            sale.ReferenceNumber = CommonUtility.GetStringValue(rsHeadFields["ReferenceNum"]);
            if (sale.ReferenceNumber != "" && taxExemptGa)
            {
                sale.EligibleTaxEx = true;
            }

            //Added to load the original Loyalty Card
            var rsDiscountTender = GetRecords("select * from DiscountTender where SALE_NO=" + CommonUtility.GetStringValue(saleNumber) + " AND TILL_NUM=" + tillNumber, db);
            if (rsDiscountTender != null && rsDiscountTender.Rows.Count != 0)
            {
                var tenderFields = rsDiscountTender.Rows[0];
                sale.Customer.LoyaltyCard = CommonUtility.GetStringValue(tenderFields["CardNum"]);
                sale.CouponID = CommonUtility.GetStringValue(tenderFields["CouponID"]);
            }

            sale.ForCorrection = false; 

            sale.Sale_Totals.Invoice_Discount_Type = Convert.ToString(rsHeadFields["Disc_Type"]);
            sale.Sale_Totals.Invoice_Discount = Convert.ToDecimal(Convert.ToInt32(rsHeadFields["Invc_Disc"]) * -1);

            sale.Sale_Totals.Penny_Adj = Convert.ToDecimal(-1 * Convert.ToInt32(DBNull.Value.Equals(rsHeadFields["PENNY_ADJ"]) ? 0 : rsHeadFields["PENNY_ADJ"])); //  
            sale.ReverseRunaway = (string)rsHeadFields["T_type"] == "RUNAWAY" && Convert.ToInt32(rsHeadFields["Sale_amt"]) > 0; // 
            sale.ReversePumpTest = (string)rsHeadFields["T_type"] == "PUMPTEST" && Convert.ToInt32(rsHeadFields["Sale_amt"]) > 0; // 

            _performancelog.Debug($"End,ReturnSaleService,GetSaleBySaleNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return sale;
        }

        /// <summary>
        /// Checks If Sale Exists
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="isSaleFound">Sale found</param>
        /// <param name="isReturnable">Sale returnable or not</param>
        public void IsSaleExist(int saleNumber, DateTime saleDate, out bool isSaleFound, out bool isReturnable)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleService,IsSaleExist,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var saleFound = false;
            isSaleFound = true;
            isReturnable = true;

            var strSqlHead = " SELECT * FROM   SaleHead Where  SaleHead.Sale_No = " + CommonUtility.GetStringValue(saleNumber) + " AND SaleHead.T_Type IN ( \'SALE\' ,\'REFUND\', \'RUNAWAY\',\'PUMPTEST\' ) and SaleHead.SALE_AMT <> 0 ";

            var rsHead = GetRecords(strSqlHead, DataSource.CSCTrans);

            // If SaleNo is not found in Trans.mdb database, then
            // look for this SaleNo in all active Tills
            if (rsHead == null || rsHead.Rows.Count == 0)
            {
                rsHead = GetRecords(strSqlHead, DataSource.CSCTills);
                // If sale number is found exit for do and use this recordset
                if (rsHead != null && rsHead.Rows.Count != 0)
                {
                    saleFound = true;
                }
            }
            else
            {
                saleFound = true;
            }

            if (!saleFound)
            {
                isSaleFound = false;
                isReturnable = false;
                return;
            }
            var rsHeadFields = rsHead.Rows[0];

            if (saleDate != new DateTime() && CommonUtility.GetDateTimeValue(rsHeadFields["Sale_Date"]) < saleDate)
            {
                isReturnable = false;
            }
            _performancelog.Debug($"End,ReturnSaleService,IsSaleExist,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Load Customer
        /// </summary>
        /// <param name="customerCode"></param>
        /// <param name="pDefaultCustomer"></param>
        /// <param name="defaultCustCode"></param>
        /// <returns></returns>
        private Customer LoadCustomer(string customerCode, bool pDefaultCustomer, string defaultCustCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleService,LoadCustomer,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var customer = new Customer();
            const string cashSaleClient = "Cash Sale"; 
            if (pDefaultCustomer && defaultCustCode != "")
            {
                if (customerCode == defaultCustCode || customerCode == cashSaleClient || customerCode == "")
                {
                    customerCode = Convert.ToString(defaultCustCode);
                }
            }
            if (!string.IsNullOrEmpty(customerCode))
            {
                customer = _customerService.GetClientByClientCode(customerCode) ?? new Customer
                {
                    Code = cashSaleClient,
                    Name = "Cash Sale",
                    Price_Code = 1,
                    AR_Customer = false,
                    PointCardNum = "",
                    PointCardPhone = "",
                    PointCardSwipe = ""
                };
            }
            _performancelog.Debug($"End,ReturnSaleService,LoadCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return customer;
        }


        /// <summary>
        /// Get Treaty Name
        /// </summary>
        /// <param name="treatyNumber"></param>
        /// <returns></returns>
        private string GetTreatyName(string treatyNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleService,GetTreatyName,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var returnValue = "";
            var rs = GetRecords("SELECT * FROM TreatyNo WHERE TreatyNo=\'" + treatyNumber.Trim() + "\'", DataSource.CSCMaster);

            if (rs != null && rs.Rows.Count != 0)
            {
                var rsFields = rs.Rows[0];
                returnValue = CommonUtility.GetStringValue(rsFields["TreatyName"]);
            }
            _performancelog.Debug($"End,ReturnSaleService,GetAllSales,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Method to adjust the discount rate
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="discountRate">Discount rate</param>
        private void SetDiscountRate(ref Sale_Line saleLine, float discountRate)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleService,SetDiscountRate,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            saleLine.Discount_Rate = discountRate;
            if (!saleLine.No_Loading)
            {
                if (saleLine.Discount_Type == "%")
                {
                    saleLine.Line_Discount = (float)saleLine.Amount * (saleLine.Discount_Rate / 100);

                }
                else if (saleLine.Discount_Type == "$")
                {
                    if (saleLine.LINE_TYPE.ToUpper() == "LINE TOTAL")
                    {
                        saleLine.Line_Discount = saleLine.Discount_Rate;
                    }
                    else
                    {
                        saleLine.Line_Discount = saleLine.Quantity * saleLine.Discount_Rate;
                    }

                }
                else
                {
                    saleLine.Line_Discount = 0;
                }
            }

            saleLine.Net_Amount = saleLine.Amount - (decimal)Round(saleLine.Line_Discount + saleLine.Discount_Adjust, 2);
            _performancelog.Debug($"End,ReturnSaleService,SetDiscountRate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }


        /// <summary>
        /// Round
        /// </summary>
        /// <param name="number"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        private double Round(double number, int digits = 0)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleService,Round,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            short i;
            var strFormat = "0";
            if (digits > 0)
            {
                strFormat = strFormat + ".";
            }
            for (i = 1; i <= digits; i++)
            {
                strFormat = strFormat + "0";
            }
            var returnValue = Math.Round(Conversion.Val(number.ToString(strFormat)), digits);
            _performancelog.Debug($"End,ReturnSaleService,Round,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Get Line Kits
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="lineNumber"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        private Line_Kits GetLineKits(int saleNumber, int lineNumber, DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReturnSaleService,GetLineKits,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            Line_Kits lineKits = new Line_Kits();

            // Get the kit items in the line
            var rsLineKit = GetRecords("Select *  FROM   SaleKit  WHERE  SaleKit.Sale_No = " + CommonUtility.GetStringValue(saleNumber) + " AND  SaleKit.Line_No = " + CommonUtility.GetStringValue(lineNumber) + " ", dataSource);
            foreach (DataRow rsLineKitFields in rsLineKit.Rows)
            {
                // Charges on Kit items
                var rsLineKitChg = GetRecords("Select * From   SaleKitChg Where  SaleKitChg.Sale_No = " + CommonUtility.GetStringValue(saleNumber) + " AND  SaleKitChg.Line_No = " + CommonUtility.GetStringValue(lineNumber) + " AND  SaleKitChg.Kit_Item = \'" + CommonUtility.GetStringValue(rsLineKitFields["Kit_Item"]) + "\' ", dataSource);
                var lkc = new K_Charges();
                foreach (DataRow rsLineKitChgFields in rsLineKitChg.Rows)
                {
                    // Taxes on Charges on Kit items
                    var rsCgt = GetRecords("Select * From   SaleKitChgTax Where  SaleKitChgTax.Sale_No  = " + CommonUtility.GetStringValue(saleNumber) + " AND  SaleKitChgTax.Line_No  = " + CommonUtility.GetStringValue(lineNumber) + " AND SaleKitChgTax.Kit_Item = \'" + CommonUtility.GetStringValue(rsLineKitFields["Kit_Item"]) + "\' AND  SaleKitChgTax.As_Code  = \'" + CommonUtility.GetStringValue(rsLineKitChgFields["As_Code"]) + "\' ", dataSource);
                    var cgt = new Charge_Taxes();
                    foreach (DataRow rsCgtFields in rsCgt.Rows)
                    {
                        cgt.Add(CommonUtility.GetStringValue(rsCgtFields["Tax_Name"]), CommonUtility.GetStringValue(rsCgtFields["Tax_Code"]), CommonUtility.GetFloatValue(rsCgtFields["Tax_Rate"]), CommonUtility.GetBooleanValue(rsCgtFields["Tax_Included"]), "");
                    }
                    lkc.Add(CommonUtility.GetDoubleValue(rsLineKitChgFields["price"]), CommonUtility.GetStringValue(rsLineKitChgFields["Description"]), CommonUtility.GetStringValue(rsLineKitChgFields["As_Code"]), cgt, "");
                }
                lineKits.Add(CommonUtility.GetStringValue(rsLineKitFields["Kit_Item"]), CommonUtility.GetStringValue(rsLineKitFields["Descript"]), CommonUtility.GetFloatValue(rsLineKitFields["Quantity"]), CommonUtility.GetFloatValue(rsLineKitFields["Base"]), CommonUtility.GetFloatValue(rsLineKitFields["Fraction"]), CommonUtility.GetFloatValue(rsLineKitFields["Alloc"]), CommonUtility.GetStringValue(rsLineKitFields["Serial"]), lkc, "");
            }
            _performancelog.Debug($"End,ReturnSaleService,GetLineKits,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return lineKits;
        }
    }
}

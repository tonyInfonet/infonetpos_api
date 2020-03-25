using System;
using System.Collections.Generic;
using System.IO;
using Infonet.CStoreCommander.Entities;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace Infonet.CStoreCommander.ADOData
{
    public class ReportService : SqlDbService, IReportService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        /// <summary>
        /// Method to delete previous count report
        /// </summary>
        public void DeletePreviousCountReport()
        {
            Execute("DELETE FROM CountReport", DataSource.CSCTrans);
        }

        /// <summary>
        /// Method to add count report
        /// </summary>
        /// <param name="countReport">Count report</param>
        public void AddCountReport(CountReport countReport)
        {
            if (countReport != null)
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter("SELECT * FROM CountReport", _connection);
                _adapter.Fill(_dataTable);
                var fields = _dataTable.NewRow();
                fields["Dept"] = countReport.Department;
                fields["Amount"] = countReport.Amount;
                fields["Sub_Dept"] = countReport.SubDepartment;
                fields["Sub_Detail"] = countReport.SubDetail;
                fields["Stock_Code"] = countReport.StockCode;
                fields["ItemCount"] = countReport.ItemCount;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
            }
        }

        /// <summary>
        /// Method to get sales count detail
        /// </summary>
        /// <param name="selectClause">Select clause</param>
        /// <param name="department">Department</param>
        /// <param name="shiftDate">ShiftDate</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="groupByClause">Group by clause</param>
        /// <param name="detailLevel">Detail level</param>
        /// <returns>Sale count detail</returns>
        public List<CountReport> GetSalesCountDetail(string selectClause, string department, DateTime shiftDate,
              string whereClause, string groupByClause, byte detailLevel)
        {
            var saleCounts = new List<CountReport>();
            var strSql = "SELECT " + selectClause + "FROM SaleHead AS A INNER JOIN SaleLine AS B ON A.Sale_No=B.Sale_No "
                            + "LEFT OUTER JOIN CSCMaster.dbo.KIT_ITEM AS C ON B.STOCK_CODE=C.Kit_Code  WHERE B.Dept=\'"
                            + department + "\' AND A.ShiftDate=\'" + shiftDate.ToString("yyyyMMdd") + "\' " + whereClause
                            + "GROUP BY " + groupByClause + "UNION ALL  SELECT " + selectClause
                            + "FROM CSCTrans.dbo.SaleHead AS A INNER JOIN CSCTrans.dbo.SaleLine AS B ON A.Sale_No=B.Sale_No "
                            + "LEFT OUTER JOIN CSCMaster.dbo.KIT_ITEM AS C ON B.STOCK_CODE=C.Kit_Code  WHERE B.Dept=\'"
                            + department + "\' AND A.ShiftDate=\'" + shiftDate.ToString("yyyyMMdd") + "\' " + whereClause
                            + "GROUP BY " + groupByClause + "ORDER BY " + groupByClause;
            var rsTemp = GetRecords(strSql, DataSource.CSCTills);
            foreach (DataRow fields in rsTemp.Rows)
            {
                var saleCount = new CountReport();
                saleCount.Department = CommonUtility.GetStringValue(fields["Dept"]);
                saleCount.Amount = CommonUtility.GetDoubleValue(fields["Amount"]);
                if (detailLevel >= 2)
                {
                    saleCount.SubDepartment = CommonUtility.GetStringValue(fields["Sub_Dept"]);
                }
                if (detailLevel >= 3)
                {
                    saleCount.SubDetail = CommonUtility.GetStringValue(fields["Sub_Detail"]);
                }

                if (string.IsNullOrEmpty(CommonUtility.GetStringValue(fields["KStock_Code"])))
                {
                    saleCount.StockCode = CommonUtility.GetStringValue(fields["Stock_Code"]);
                    saleCount.ItemCount = CommonUtility.GetIntergerValue(fields["ItemCount"]);
                }
                else // kit code
                {
                    saleCount.StockCode = CommonUtility.GetStringValue(fields["KStock_Code"]);
                    saleCount.ItemCount = CommonUtility.GetIntergerValue(fields["ItemCount"]) * CommonUtility.GetIntergerValue(fields["Quantity"]);
                }
                saleCounts.Add(saleCount);
            }
            return saleCounts;
        }

        /// <summary>
        /// Method to get existing count reports
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="detailLevel">Detail level</param>
        /// <returns>Count reports</returns>
        public List<CountReport> GetCountReports(string query, byte detailLevel)
        {
            var countReports = new List<CountReport>();
            var rsReportTemp = GetRecords(query, DataSource.CSCTrans);
            foreach (DataRow fields in rsReportTemp.Rows)
            {
                var countReport = new CountReport();
                if (detailLevel == 1)
                {
                    countReport.Amount = CommonUtility.GetDoubleValue(fields["Amount"]);
                    countReport.ItemCount = CommonUtility.GetIntergerValue(fields["ItemCount"]);
                }
                else if (detailLevel == 2)
                {
                    countReport.Amount = CommonUtility.GetDoubleValue(fields["Amount"]);
                    countReport.ItemCount = CommonUtility.GetIntergerValue(fields["ItemCount"]);
                    countReport.SubDepartment = CommonUtility.GetStringValue(fields["Sub_Dept"]);
                }
                else if (detailLevel == 3)
                {
                    countReport.Amount = CommonUtility.GetDoubleValue(fields["Amount"]);
                    countReport.ItemCount = CommonUtility.GetIntergerValue(fields["ItemCount"]);
                    countReport.SubDepartment = CommonUtility.GetStringValue(fields["Sub_Dept"]);
                    countReport.SubDetail = CommonUtility.GetStringValue(fields["Sub_Detail"]);
                }
                else if (detailLevel == 4)
                {

                    countReport.Amount = CommonUtility.GetDoubleValue(fields["Amount"]);
                    countReport.ItemCount = CommonUtility.GetIntergerValue(fields["ItemCount"]);
                    countReport.SubDepartment = CommonUtility.GetStringValue(fields["Sub_Dept"]);
                    countReport.SubDetail = CommonUtility.GetStringValue(fields["Sub_Detail"]);
                    countReport.StockCode = CommonUtility.GetStringValue(fields["Stock_Code"]);

                }
                countReports.Add(countReport);
            }
            return countReports;
        }

        /// <summary>
        /// Method to get flash report details for till number
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns></returns>
        public FlashReportTotals GetFlashReportDetails(int tillNumber)
        {
            var rsTotals = GetRecords("SELECT Sum(SaleHead.Sale_Amt ) as [TotTend],  Sum(SaleHead.Line_Disc) as [LDisc],  Sum(SaleHead.Invc_Disc) as [IDisc],  Sum(SaleHead.OverPayment) as [OverP]  FROM SaleHead  WHERE (SaleHead.T_Type = \'SALE\') AND SaleHead.TILL = " + tillNumber, DataSource.CSCTills);

            var totalsFields = rsTotals.Rows[0];
            var rsRefund = GetRecords("SELECT Sum(SaleHead.Sale_Amt ) as [TotTend],  Sum(SaleHead.Line_Disc) as [LDisc],  Sum(SaleHead.Invc_Disc) as [IDisc],  Sum(SaleHead.OverPayment) as [OverP]  FROM SaleHead   WHERE (SaleHead.T_Type = \'REFUND\') AND SaleHead.TILL = " + tillNumber, DataSource.CSCTills);
            var refundFields = rsRefund.Rows[0];
            // Get Total Taxes

            var rsTaxes = GetRecords("SELECT Sum(S_SaleTax.Tax_Added_Amount) As [Tax] FROM   S_SaleTax INNER JOIN SaleHead ON  S_SaleTax.Sale_No = SaleHead.Sale_No WHERE (SaleHead.T_Type = \'SALE\' OR  SaleHead.T_Type = \'REFUND\')   AND SaleHead.TILL = " + tillNumber, DataSource.CSCTills);
            var taxesFields = rsTaxes.Rows[0];
            // ... and total associated charges

            var rsCharge = GetRecords("SELECT Sum(SaleChg.Amount) As [Charges]  FROM   SaleChg INNER JOIN SaleHead ON SaleChg.Sale_No = SaleHead.Sale_No  WHERE (SaleHead.T_Type = \'SALE\' OR SaleHead.T_Type = \'REFUND\')  AND SaleHead.TILL = " + tillNumber, DataSource.CSCTills);
            var chargesFields = rsCharge.Rows[0];
            var totalOverPayment = CommonUtility.GetFloatValue(totalsFields["OverP"]);

            var totals = CommonUtility.GetFloatValue(totalsFields["TotTend"]);
            totals = totals + totalOverPayment; // add the overpayments value

            var lDisc = CommonUtility.GetFloatValue(totalsFields["LDisc"]);

            var iDisc = CommonUtility.GetFloatValue(totalsFields["IDisc"]);

            var taxes = CommonUtility.GetFloatValue(taxesFields["Tax"]);

            var charges = CommonUtility.GetFloatValue(chargesFields["Charges"]);

            var refund = CommonUtility.GetFloatValue(refundFields["TotTend"]);
            var reportTotals = new FlashReportTotals
            {
                Totals = totals,
                LineDiscount = lDisc,
                InvoiceDiscount = iDisc,
                Charge = charges,
                Tax = taxes,
                Refund = refund
            };
            return reportTotals;
        }

        /// <summary>
        /// Method to get total sales without discount
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Line total</returns>
        public decimal GetLineTotal(int tillNumber)
        {
            var rsDept = GetRecords("SELECT Sum(SaleLine.Amount-SaleLine.Discount-SaleLine.Disc_Adj) as [Sales]  FROM   SaleLine INNER JOIN SaleHead ON SaleLine.Sale_No = SaleHead.Sale_No  WHERE  (SaleHead.T_Type = \'SALE\' OR SaleHead.T_Type = \'REFUND\') AND SaleHead.TILL = " + tillNumber, DataSource.CSCTills);
            if (rsDept.Rows.Count == 0) return 0;
            var fields = rsDept.Rows[0];
            return CommonUtility.GetDecimalValue(fields["Sales"]);
        }

        /// <summary>
        /// Method to get sales by department for flash report
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Departments</returns>
        public List<Department> GetDepartmentsForFlashReport(int tillNumber)
        {
            var departments = new List<Department>();
            var rsDept = GetRecords("SELECT DISTINCT SaleLine.Dept As [Department], D.Dept_Name As [Description] , Sum(SaleLine.Amount-SaleLine.Discount-SaleLine.Disc_Adj) AS [Sales] FROM SaleHead INNER JOIN (CSCMaster.dbo.Dept as [D] RIGHT JOIN SaleLine ON D.Dept = SaleLine.Dept) ON SaleHead.Sale_No = SaleLine.Sale_No  WHERE ((SaleHead.T_Type = \'SALE\') Or (SaleHead.T_Type = \'REFUND\'))  AND SaleHead.TILL = " + tillNumber + " GROUP BY D.Dept, D.Dept_Name, SaleLine.Dept ORDER BY SaleLine.Dept", DataSource.CSCTills);
            if (rsDept == null || rsDept.Rows.Count == 0) return departments;
            foreach (DataRow fields in rsDept.Rows)
            {
                departments.Add(new Department
                {
                    Dept = CommonUtility.GetStringValue(fields["Department"]),
                    DeptName = CommonUtility.GetStringValue(fields["Description"]),
                    Sales = CommonUtility.GetDecimalValue(fields["Sales"])
                });
            }
            return departments;
        }

        /// <summary>
        /// Method to get fuel sales report
        /// </summary>
        /// <param name="fuelDepartment">Fuel department name</param>
        /// <param name="tillNumber">tillNumber</param>
        /// <returns>Fuel sales</returns>
        public List<FuelSale> GetFuelSalesReport(string fuelDepartment, int tillNumber)
        {
            var fuelSales = new List<FuelSale>();
            var strSql = "SELECT SaleLine.Stock_Code, SaleLine.Descript, SaleLine.Dept, D.Dept_Name, Sum(SaleLine.Quantity) as [Volume], Sum(SaleLine.Amount-SaleLine.Discount-SaleLine.Disc_Adj) as [Sales] FROM   (SaleLine INNER JOIN SaleHead ON SaleLine.Sale_No = SaleHead.Sale_No) INNER JOIN CSCMaster.dbo.Dept as [D] ON  D.Dept = SaleLine.Dept WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND  SaleLine.Dept = \'" + fuelDepartment + "\' and   SaleHead.TILL = " + tillNumber + " Group By SaleLine.Stock_Code, SaleLine.Descript, SaleLine.Dept, D.Dept_Name Order by SaleLine.Stock_Code ";
            var rsSales = GetRecords(strSql, DataSource.CSCTills);
            if (rsSales == null || rsSales.Rows.Count == 0) return fuelSales;
            fuelSales.AddRange(from DataRow fields in rsSales.Rows
                               where !DBNull.Value.Equals(fields["Sales"])
                               select new FuelSale
                               {
                                   Department = CommonUtility.GetStringValue(fields["Dept"]),
                                   DepartmentName = CommonUtility.GetStringValue(fields["Dept_Name"]),
                                   StockCode = CommonUtility.GetStringValue(fields["Stock_Code"]),
                                   Description = CommonUtility.GetStringValue(fields["Descript"]),
                                   Volume = CommonUtility.GetDecimalValue(fields["Volume"]),
                                   Sales = CommonUtility.GetDecimalValue(fields["Sales"])
                               });
            return fuelSales;
        }

        /// <summary>
        /// Method to get change value
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Change</returns>
        public decimal GetChangeValue(int tillNumber)
        {
            var rs = GetRecords("SELECT SUM(Change) AS [sumChange] FROM SaleHead WHERE T_Type IN (\'SALE\',\'REFUND\',\'PAYMENT\',\'ARPAY\') AND TILL=" + tillNumber, DataSource.CSCTills);
            var fields = rs.Rows[0];
            return CommonUtility.GetDecimalValue(fields["sumChange"]);
        }

        /// <summary>
        /// Method to get cash draws
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns></returns>
        public Till GetCashDraws(int tillNumber)
        {
            var rs = GetRecords("SELECT SUM(Amount) AS [sumAmount], sum(CashBonus) as [BonusDraw] FROM CashDraw Where TILL= '" + tillNumber + "'", DataSource.CSCTills);
            var fields = rs.Rows[0];
            return new Till
            {
                BonusDraw = CommonUtility.GetDecimalValue(fields["BonusDraw"]),
                Draws = CommonUtility.GetDecimalValue(fields["sumAmount"])
            };
        }

        /// <summary>
        /// Method to get payouts
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        public decimal GetPayouts(int tillNumber)
        {
            var rs = GetRecords("SELECT   SUM(SaleHead.PayOut) AS [sumAmount]  FROM     SaleHead  WHERE    SaleHead.T_Type = \'PAYOUT\' AND    SaleHead.TILL=" + tillNumber, DataSource.CSCTills);

            var fields = rs.Rows[0];
            return CommonUtility.GetDecimalValue(fields["sumAmount"]);
        }

        /// <summary>
        /// Method to get bonus
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns></returns>
        public decimal GetBonusGiveAway(int tillNumber)
        {
            var rsbonus = GetRecords("Select   sum(DiscountAmount) as [Bonus]  FROM     DiscountTender  WHERE   DiscountType = \'B\' and  TILL_NUM=" + tillNumber, DataSource.CSCTills);

            if (rsbonus != null && rsbonus.Rows.Count != 0)
            {
                var fields = rsbonus.Rows[0];
                CommonUtility.GetDecimalValue(fields["Bonus"]);
            }
            return 0;
        }


        /// <summary>
        /// Method to get drop values
        /// </summary>
        /// <param name="tillNumber">Tillnumber</param>
        /// <returns>List of tenders</returns>
        public List<Tender> GetDropValues(int tillNumber)
        {
            var rsDrops = GetRecords("SELECT DropLines.Tender_Name AS [Tender] , SUM(DropLines.Amount) AS [SumAmount]  FROM DropLines Where DropLines.TILL_NUM=" + tillNumber + " GROUP BY DropLines.Tender_Name", DataSource.CSCTills);

            return (from DataRow fields in rsDrops.Rows
                    select new Tender
                    {
                        Tender_Name = CommonUtility.GetStringValue(fields["Tender"]),
                        Amount_Used = CommonUtility.GetDecimalValue(fields["SumAmount"])
                    }).ToList();
        }

        /// <summary>
        /// Method to get cash sale values
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of tenders</returns>
        public List<Tender> GetCashSaleValues(int tillNumber)
        {
            var rsSales = GetRecords("SELECT   SaleTend.TendName as [Tender], SUM(SaleTend.AmtTend) AS[sumAmount] FROM SaleTend INNER JOIN SaleHead ON SaleTend.Sale_No = SaleHead.Sale_No  WHERE    SaleHead.T_Type IN(\'SALE\',\'REFUND\')  AND    SaleHead.TILL=" + tillNumber + " GROUP BY SaleTend.TendName ", DataSource.CSCTills);

            return (from DataRow fields in rsSales.Rows
                    select new Tender
                    {
                        Tender_Name = CommonUtility.GetStringValue(fields["Tender"]),
                        Amount_Used = CommonUtility.GetDecimalValue(fields["sumAmount"])
                    }).ToList();
        }


        /// <summary>
        /// Method to get payment values
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of tenders</returns>
        public List<Tender> GetPaymentValues(int tillNumber)
        {
            var rsPayments = GetRecords("SELECT   SaleTend.TendName as [Tender], SUM(SaleTend.AmtTend) AS [sumAmount] FROM SaleTend INNER JOIN SaleHead ON SaleTend.Sale_No = SaleHead.Sale_No  WHERE    SaleHead.T_Type = \'PAYMENT\' AND    SaleHead.TILL=" + tillNumber + " GROUP BY SaleTend.TendName ", DataSource.CSCTills);
            return (from DataRow fields in rsPayments.Rows
                    select new Tender
                    {
                        Tender_Name = CommonUtility.GetStringValue(fields["Tender"]),
                        Amount_Used = CommonUtility.GetDecimalValue(fields["sumAmount"])
                    }).ToList();
        }

        /// <summary>
        /// Method to get AR payment values
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of tenders</returns>
        public List<Tender> GetArPaymentValues(int tillNumber)
        {
            var rsARpayments = GetRecords(" SELECT   SaleTend.TendName as [Tender], SUM(SaleTend.AmtTend) AS [sumAmount] FROM     SaleTend INNER JOIN SaleHead ON SaleTend.Sale_No = SaleHead.Sale_No  WHERE    SaleHead.T_Type = \'ARPAY\'  AND    SaleHead.TILL=" + tillNumber + "  GROUP BY SaleTend.TendName", DataSource.CSCTills);

            return (from DataRow fields in rsARpayments.Rows
                    select new Tender
                    {
                        Tender_Name = CommonUtility.GetStringValue(fields["Tender"]),
                        Amount_Used = CommonUtility.GetDecimalValue(fields["sumAmount"])
                    }).ToList();
        }

        /// <summary>
        /// Method to get bottle return values
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of tenders</returns>
        public List<Tender> GetBottleReturnValues(int tillNumber)
        {
            var rsBottleReturn = GetRecords("SELECT   SaleTend.TendName as [Tender], SUM(SaleTend.AmtTend) AS [sumAmount] FROM     SaleTend INNER JOIN SaleHead ON SaleTend.Sale_No = SaleHead.Sale_No  WHERE    SaleHead.T_Type = \'BTL RTN\'  AND    SaleHead.TILL=" + tillNumber + "  GROUP BY SaleTend.TendName", DataSource.CSCTills);

            return (from DataRow fields in rsBottleReturn.Rows
                    select new Tender
                    {
                        Tender_Name = CommonUtility.GetStringValue(fields["Tender"]),
                        Amount_Used = CommonUtility.GetDecimalValue(fields["sumAmount"])
                    }).ToList();
        }

        /// <summary>
        /// Method to get non cash currency tender by tender name
        /// </summary>
        /// <param name="currency">Currency</param>
        /// <param name="tenderDescription">Tender description</param>
        /// <returns>Non cash currency names</returns>
        public List<string> GetNonCashCurrencyTenders(string currency, string tenderDescription)
        {
            var rstender = GetRecords("select TENDDESC as TendName from TENDMAST Where TENDCLASS=\'CASH\' and  TENDDESC<>\'" + currency + "\'and upper(TENDDESC) <> \'" + tenderDescription + "\'", DataSource.CSCMaster);
            var tenderNames = new List<string>();
            foreach (DataRow fields in rstender.Rows)
            {
                tenderNames.Add(CommonUtility.GetStringValue(fields["TendName"]));
            }
            return tenderNames;
        }

        /// <summary>
        /// Method to get cash drop by tender name
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderName">Tender name</param>
        /// <returns>Cash drop</returns>
        public float? GetCashDropByTenderName(int tillNumber, string tenderName)
        {
            var rsDrops = GetRecords("SELECT DropLines.Tender_Name AS [Tender] , SUM(DropLines.Amount) AS [SumAmount]  FROM DropLines Where DropLines.TILL_NUM=" + tillNumber + " AND DropLines.Tender_Name=\'" + tenderName + "\' GROUP BY DropLines.Tender_Name ", DataSource.CSCTills);

            if (rsDrops == null || rsDrops.Rows.Count == 0)
            {
                return 0;
            }
            var fields = rsDrops.Rows[0];
            return CommonUtility.GetFloatValue(fields["sumAmount"]);
        }

        /// <summary>
        /// Method to get sale tender by tender name
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderName">Tender name</param>
        /// <returns>Sale tender</returns>
        public float? GetSaleTenderByTenderName(int tillNumber, string tenderName)
        {
            var rsSales = GetRecords("SELECT   SaleTend.TendName as [Tender], SUM(SaleTend.AmtTend) AS [sumAmount]  FROM     SaleTend INNER JOIN SaleHead ON SaleTend.Sale_No = SaleHead.Sale_No  WHERE   SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'PAYMENT\',\'ARPAY\')  AND    SaleHead.TILL=" + tillNumber + "  AND    SaleTend.TendName=\'" + tenderName + "\' GROUP BY SaleTend.TendName ", DataSource.CSCTills);

            if (rsSales.Rows.Count == 0)
            {
                return 0;
            }
            var fields = rsSales.Rows[0];
            return CommonUtility.GetFloatValue(fields["sumAmount"]);
        }

        /// <summary>
        /// Method to get sale totals
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Sale totals</returns>
        public SaleTend GetSaleTotals(int tillNumber)
        {
            var rsTotals = GetRecords("SELECT Sum(SaleHead.Sale_Amt ) as [TotTend], Sum(SaleHead.Line_Disc) as [LDisc], Sum(SaleHead.Invc_Disc) as [IDisc], Sum(SaleHead.OverPayment) as [OverP]  FROM SaleHead   WHERE (SaleHead.T_Type = \'SALE\' OR SaleHead.T_Type = \'REFUND\')  AND SaleHead.TILL = " + tillNumber, DataSource.CSCTills);
            var fields = rsTotals.Rows[0];
            return new SaleTend
            {
                AmountTend = CommonUtility.GetDecimalValue(fields["TotTend"]),
                AmountUsed = CommonUtility.GetDecimalValue(fields["OverP"])
            };
        }

        /// <summary>
        /// Method to get non cash tenders
        /// </summary>
        /// <returns>Non cash tenders</returns>
        public List<string> GetNonCashTenders()
        {
            var rstender = GetRecords("select TENDDESC as TendName from TENDMAST Where TENDCLASS<>\'CASH\'", DataSource.CSCMaster);
            var tenderNames = new List<string>();
            foreach (DataRow fields in rstender.Rows)
            {
                tenderNames.Add(CommonUtility.GetStringValue(fields["TendName"]));
            }
            return tenderNames;
        }

        /// <summary>
        /// Method to get list of gift cards
        /// </summary>
        /// <param name="saleNo">Sale number</param>
        /// <param name="reReprint">Reprint or not</param>
        /// <returns></returns>
        public List<GiftCardTender> GetListOfGiftcards(int saleNo, bool reReprint)
        {
            DataTable returnValue;
            if (reReprint)
            {
                returnValue = GetRecords("SELECT Tender_Name, Card_Number, Amount FROM   [CardTenders] WHERE Sale_No = " + Convert.ToString(saleNo) + " AND Amount > 0 ", DataSource.CSCTills);
                if (returnValue.Rows.Count == 0)
                {
                    returnValue = GetRecords("SELECT Tender_Name, Card_Number, Amount FROM   [CardTenders] WHERE Sale_No = " + Convert.ToString(saleNo) + " AND Amount > 0 ", DataSource.CSCTrans);
                }
            }
            else
            {
                returnValue = GetRecords("SELECT Tender_Name, Card_Number, Amount FROM   [CSCCurSale].[dbo].[CardTenders] WHERE Sale_No = " + Convert.ToString(saleNo) + " AND AllowMulticard > 0 AND Amount > 0 ", DataSource.CSCCurSale);
            }
            return (from DataRow fields in returnValue.Rows
                    select new GiftCardTender
                    {
                        TenderName = CommonUtility.GetStringValue(fields["Tender_Name"]),
                        CardNumber = CommonUtility.GetStringValue(fields["Card_Number"]),
                        Amount = CommonUtility.GetDecimalValue(fields["Amount"])
                    }).ToList();
        }

        /// <summary>
        /// Method to get card profile prompts
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="cardNumber">Card number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>CArd profile prompts</returns>
        public List<CardProfilePrompt> GetCardProfilePrompts(int saleNumber, string cardNumber, byte tillNumber)
        {
            var cardProfilePrompts = new List<CardProfilePrompt>();
            var rsSalePrompt = GetRecords("SELECT A.*, B.DisplayText FROM CardProfilePrompts A inner join CSCMASTER.dbo.CardFuelprompts B on A.Promptid = B.promptid Where B.Type=\'F\' AND sale_no = " + Convert.ToString(saleNumber) + "   and A.cardnum = \'" + cardNumber + "\' and Till_num = " + Convert.ToString(tillNumber) + " order by a.promptid ", DataSource.CSCCurSale);
            if (rsSalePrompt.Rows.Count == 0)
            {

                rsSalePrompt = GetRecords("SELECT A.*, B.DisplayText FROM CardProfilePrompts A inner join CSCMASTER.dbo.CardFuelprompts B on A.Promptid = B.promptid  Where B.Type=\'F\' AND sale_no = " + Convert.ToString(saleNumber) + "   and A.cardnum = \'" + cardNumber + "\' and Till_num = " + Convert.ToString(tillNumber) + " order by a.promptid ", DataSource.CSCTills);
                if (rsSalePrompt.Rows.Count == 0)
                {
                    rsSalePrompt = GetRecords("SELECT A.*, B.DisplayText FROM CardProfilePrompts A inner join CSCMASTER.dbo.CardFuelprompts B on A.Promptid = B.promptid  Where B.Type=\'F\' AND sale_no = " + Convert.ToString(saleNumber) + " and cardnum = \'" + cardNumber + "\' and Till_num = " + Convert.ToString(tillNumber) + " order by a.promptid ", DataSource.CSCTrans);
                }
            }
            foreach (DataRow fields in rsSalePrompt.Rows)
            {
                if (!DBNull.Value.Equals(fields["DisplayText"]) && !string.IsNullOrEmpty(CommonUtility.GetStringValue(fields["DisplayText"]))) // not print if display text is missing
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(fields["PromptAnswer"])))
                    {
                        cardProfilePrompts.Add(new CardProfilePrompt
                        {
                            PromptAnswer = CommonUtility.GetStringValue(fields["PromptAnswer"]),
                            DisplayText = CommonUtility.GetStringValue(fields["DisplayText"])

                        });
                    }
                }
            }
            return cardProfilePrompts;
        }

        /// <summary>
        /// Method to get signature
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Image</returns>
        public Stream GetSignature(int saleNumber, int tillNumber)
        {
            Stream mstream = new MemoryStream();
            var rsSign = GetRecords("Select * from Signature where sale_no = " + Convert.ToString(saleNumber) + "  and Till_num = " + Convert.ToString(tillNumber), DataSource.CSCTills);
            if (rsSign.Rows.Count == 0)
            {
                rsSign = GetRecords("Select * from Signature where sale_no = " + Convert.ToString(saleNumber) + "  and Till_num = " + Convert.ToString(tillNumber), DataSource.CSCTrans);
            }
            if (rsSign.Rows.Count != 0)
            {
                var fields = rsSign.Rows[0];
                mstream.Write(DBNull.Value.Equals(fields["signature"]) ? new byte[0] : (byte[])fields["signature"], 0, ((byte[])fields["signature"]).Length);
            }
            return mstream;
        }

        /// <summary>
        /// Method to verify if sale is available for till
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>True or false</returns>
        public bool IsSaleAvailableForTill(int tillNumber)
        {
            var rs = GetRecords("SELECT T_Type FROM SaleHead WHERE (SaleHead.T_Type = \'SALE\' OR SaleHead.T_Type = \'REFUND\') and  SaleHead.TILL = " + tillNumber, DataSource.CSCTills);
            if (rs.Rows.Count == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method to find if reader usage is available
        /// </summary>
        /// <returns>True or false</returns>
        public bool IsReaderUsageAvailable()
        {
            var rsReader = GetRecords("SELECT count(*) FROM ReaderUsage  WHERE ReaderUsage = 1 ",
                DataSource.CSCReader);
            return rsReader.Rows.Count > 0;
        }

        /// <summary>
        /// Method to get pay at pump credits
        /// </summary>
        /// <returns>List of pump credits</returns>
        public List<PayAtPumpCredit> GetPayAtPumpCredits()
        {
            var strSql = "SELECT PayPumpCredit AS [Cr], PayPumpCreditDebit AS [CrDb]  FROM ReaderUsage " + "GROUP BY PayPumpCredit, PayPumpCreditDebit";
            var rsReader = GetRecords(strSql, DataSource.CSCReader);
            return (from DataRow row in rsReader.Rows
                    select new PayAtPumpCredit
                    {
                        PayPumpCredit = CommonUtility.GetBooleanValue(row["cr"]),
                        PayPumpCreditDebit = CommonUtility.GetBooleanValue(row["crdb"])
                    }).ToList();
        }

        /// <summary>
        /// Method to find if pay at pump sales is available or not
        /// </summary>
        /// <returns>True or false</returns>
        public bool IsPayAtPumpSalesAvailable()
        {
            var strSql = "SELECT SaleHead.Sale_No  FROM SaleHead , SaleLine WHERE SaleHead.Sale_No=SaleLine.Sale_No AND " + "(SaleHead.T_Type) = \'" + "PATP_APP" + "\'" + " AND SaleHead.Till = " + 100;
            var rs = GetRecords(strSql, DataSource.CSCTills);
            return rs.Rows.Count > 0;
        }

        /// <summary>
        /// Method to get payment sales
        /// </summary>
        /// <param name="selectQuery">selct query</param>
        /// <param name="saleType">Sale type</param>
        /// <param name="date">Date</param>
        /// <returns>List of payment type sales</returns>
        public List<Sale> GetPaymentSales(string selectQuery, string saleType, DateTime date)
        {
            var str = " Where  T_Type=\'" + saleType + "\' AND " + "       Till <> " + 100 + " " + " AND   Sale_Date=\'" + (date.ToString("yyyyMMdd")) + "\' ";
            var strSql = selectQuery + " From   SaleHead " + str + " AND   Sale_Date=\'" + (date.ToString("yyyyMMdd")) + "\' ";
            strSql = strSql + " UNION ALL " + selectQuery + " From   CSCTrans.dbo.SaleHead " + str + " ORDER BY Sale_No DESC ";
            var dt = GetRecords(strSql, DataSource.CSCTills);
            var sales = new List<Sale>();
            foreach (DataRow row in dt.Rows)
            {
                sales.Add(new Sale
                {
                    Sale_Num = CommonUtility.GetIntergerValue(row["Sale"]),
                    Sale_Date = CommonUtility.GetDateTimeValue(row["Sold On"]),
                    Sale_Time = CommonUtility.GetDateTimeValue(row["Time"]),
                    Sale_Amount = CommonUtility.GetDecimalValue(row["Amount"])
                });
            }
            return sales;
        }

        /// <summary>
        /// Method to get pay inside sales
        /// </summary>
        /// <param name="selectQuery">Select query</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of pay inside sales</returns>
        public List<Sale> GetPayInsideSales(string selectQuery, DataSource dataSource)
        {
            var dt = GetRecords(selectQuery, dataSource);
            return (from DataRow row in dt.Rows
                    select new Sale
                    {
                        Sale_Num = CommonUtility.GetIntergerValue(row["Sale"]),
                        Sale_Date = CommonUtility.GetDateTimeValue(row["Sold On"]),
                        Sale_Time = CommonUtility.GetDateTimeValue(row["Time"]),
                        Sale_Amount = CommonUtility.GetDecimalValue(row["Amount"]),
                        Sale_Client = CommonUtility.GetStringValue(row["Customer"])
                    }).ToList();
        }

        /// <summary>
        /// Method to get pay at pump sales
        /// </summary>
        /// <param name="selectQuery">Select query</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of pay at pump sales</returns>
        public List<PayAtPumpSale> GetPayAtPumpSales(string selectQuery, DataSource dataSource)
        {
            var dt = GetRecords(selectQuery, dataSource);
            var sales = new List<PayAtPumpSale>();
            foreach (DataRow row in dt.Rows)
            {
                sales.Add(new PayAtPumpSale
                {
                    SaleNumber = CommonUtility.GetIntergerValue(row["Sale Number"]),
                    SaleDate = CommonUtility.GetDateTimeValue(row["Date"]),
                    SaleTime = CommonUtility.GetDateTimeValue(row["Time"]),
                    SaleAmount = CommonUtility.GetDecimalValue(row["Amount"]),
                    Volume = CommonUtility.GetFloatValue(row["Volume"]),
                    GradeId = CommonUtility.GetByteValue(row["Pump"]),
                    PumpId = CommonUtility.GetByteValue(row["Grade"])

                });
            }
            return sales;
        }

        /// <summary>
        /// Method to get terminal ids
        /// </summary>
        /// <param name="posId">Pos id</param>
        /// <returns>List of terminal ids</returns>
        public List<string> GetTerminalIds(int posId)
        {
            var terminalIds = new List<string>();
            var query = "SELECT * FROM TerminalIds where ID=" + Convert.ToString(posId) + " and TerminalType <> \'Ezipin\'";
            var rs = GetRecords(query, DataSource.CSCAdmin);
            foreach (DataRow row in rs.Rows)
            {
                terminalIds.Add(CommonUtility.GetStringValue(row["TerminalID"]));
            }
            return terminalIds;
        }

        /// <summary>
        /// Method to get close batch reports
        /// </summary>
        /// <param name="terminalIds">Terminal ids</param>
        /// <param name="dt">Close batch date</param>
        /// <returns>List of close batch</returns>
        public List<CloseBatch> GetCloseBatchReports(List<string> terminalIds,
            DateTime dt)
        {
            var closeBatches = new List<CloseBatch>();
            if (terminalIds.Count < 2) return closeBatches;
            var rs = GetRecords("SELECT CloseBatch.BatchNumber, " +
                                "CloseBatch.TerminalID," + "CloseBatch.BatchDate," +
                                "CloseBatch.BatchTime," + "CloseBatch.Report  FROM CloseBatch  WHERE (CloseBatch.TerminalID =\'"
                                + terminalIds[0] + "\' " + "OR CloseBatch.TerminalID =\'"
                                + terminalIds[1] + "\' ) " + "AND CloseBatch.BatchNumber <> \'\' "
                                + "AND CloseBatch.BatchDate >= \'" + dt.ToString("yyyyMMdd") + "\' " + "ORDER BY CloseBatch.BatchDate DESC,CloseBatch.BatchTime DESC",
                DataSource.CSCTrans);
            foreach (DataRow row in rs.Rows)
            {
                closeBatches.Add(new CloseBatch
                {
                    BatchNumber = CommonUtility.GetStringValue(row["BatchNumber"]),
                    TerminalId = CommonUtility.GetStringValue(row["TerminalID"]),
                    Date = CommonUtility.GetDateTimeValue(row["BatchDate"]),
                    Time = CommonUtility.GetDateTimeValue(row["BatchTime"]),
                    Report = CommonUtility.GetStringValue(row["Report"])
                });
            }
            return closeBatches;
        }

        /// <summary>
        /// Method to get data source of sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="sale">Sale</param>
        /// <returns>Data source</returns>
        public DataSource GetSale(int saleNumber, out Sale sale)
        {
            sale = new Sale();
            DataSource db;
            var rsHead = GetRecords("Select *  FROM   SaleHead  WHERE  SaleHead.Sale_No = " + Convert.ToString(saleNumber) + " ", DataSource.CSCTrans);
            if (rsHead.Rows.Count == 0)
            {
                rsHead = GetRecords("Select *  FROM   SaleHead  WHERE  SaleHead.Sale_No = " + Convert.ToString(saleNumber) + " ", DataSource.CSCTills);
                db = rsHead.Rows.Count == 0 ? DataSource.CSCMaster : DataSource.CSCTills;
            }
            else
            {
                db = DataSource.CSCTrans;
            }
            var fields = rsHead.Rows[0];
            sale.Sale_Payment = CommonUtility.GetDecimalValue(fields["Payment"]);
            sale.Customer.Code = CommonUtility.GetStringValue(fields["Client"]);
            sale.Sale_Num = CommonUtility.GetIntergerValue(fields["sale_no"]);
            sale.PointsAmount = CommonUtility.GetDecimalValue(fields["PayOut"]);
            sale.Return_Reason.Reason = CommonUtility.GetStringValue(fields["Reason"]);
            sale.Return_Reason.RType = CommonUtility.GetStringValue(fields["Reason_Type"]);
            sale.Sale_Client = CommonUtility.GetStringValue(fields["User"]);
            sale.Sale_Date = CommonUtility.GetDateTimeValue(fields["Sale_Date"]);
            sale.Sale_Time = CommonUtility.GetDateTimeValue(fields["Sale_Time"]);
            sale.Register = CommonUtility.GetByteValue(fields["Regist"]);
            sale.TillNumber = CommonUtility.GetByteValue(fields["Till"]);
            sale.Shift = CommonUtility.GetByteValue(fields["Shift"]);
            sale.Sale_Change = CommonUtility.GetDecimalValue(fields["Change"]);
            return db;
        }

        /// <summary>
        /// Method to get list of tenders for a sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of Tenders</returns>
        public List<Tender> GetTenders(int saleNumber, DataSource dataSource)
        {
            var tenders = new List<Tender>();
            var rsTend = GetRecords("Select *  FROM   SaleTend  WHERE  SaleTend.Sale_No = " + Convert.ToString(saleNumber) + " ", dataSource);
            foreach (DataRow field in rsTend.Rows)
            {
                tenders.Add(new Tender
                {
                    Tender_Name = CommonUtility.GetStringValue(field["TendName"]),
                    Tender_Class = CommonUtility.GetStringValue(field["TENDCLAS"]),
                    Exchange_Rate = CommonUtility.GetDoubleValue(field["Exchange_Rate"]),
                    Amount_Entered = CommonUtility.GetDecimalValue(field["AmtTend"]),
                    Image = "",
                    Sequence_Number = CommonUtility.GetShortValue(field["Sequence"]),
                    Amount_Used = CommonUtility.GetDecimalValue(field["AmtUsed"]),
                    PrintCopies = CommonUtility.GetShortValue(field["AmtTend"])
                });
            }
            return tenders;
        }

        /// <summary>
        /// Method to get card number
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>Card number</returns>
        public string GetCardNumber(int saleNumber)
        {
            var rsRoaPayments = GetRecords("select Card_Num from ROAPayments where Sale_Num=" + saleNumber, DataSource.CSCTrans);
            return rsRoaPayments.Rows.Count != 0 ? CommonUtility.GetStringValue(rsRoaPayments.Rows[0]["Card_Num"]) : null;
        }

        /// <summary>
        /// Method to get vendor code
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Vendor code</returns>
        public string GetVendorCode(int saleNumber, DataSource dataSource)
        {
            var rsSaleVendors = GetRecords("select Vendor from SaleVendors where Sale_No=" + saleNumber, dataSource);
            return rsSaleVendors.Rows.Count != 0 ? CommonUtility.GetStringValue(rsSaleVendors.Rows[0]["Vendor"]) : null;
        }

        /// <summary>
        /// Method to get sale taxes
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of sale taxes</returns>
        public List<Sale_Tax> GetSaleTaxes(int saleNumber, DataSource dataSource)
        {
            var rsSSaleTax = GetRecords("select * from S_SaleTax where Sale_No=" + saleNumber, dataSource);
            return (from DataRow row in rsSSaleTax.Rows
                    select new Sale_Tax
                    {
                        Tax_Name = CommonUtility.GetStringValue(row["Tax_Name"]),
                        Tax_Included_Amount = CommonUtility.GetDecimalValue(row["Tax_Included_Amount"])
                    }).ToList();
        }

        /// <summary>
        /// Method to get bottle ereturn
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>Bottle return payment</returns>
        public BR_Payment GetBottleReturn(int saleNumber)
        {
            var brRep = new BR_Payment { Sale_Num = saleNumber };
            var rsBottleReturn = GetRecords("select * from BottleReturn where SALE_NO=" + saleNumber, DataSource.CSCTrans);
            foreach (DataRow row in rsBottleReturn.Rows)
            {
                var br = new BottleReturn
                {
                    Product = CommonUtility.GetStringValue(row["Product"]),
                    Quantity = CommonUtility.GetFloatValue(row["Quantity"]),
                    Price = CommonUtility.GetFloatValue(row["price"]),
                    Amount = CommonUtility.GetDecimalValue(row["Amount"])
                };
                brRep.Add_a_Line(br);
            }
            return brRep;
        }

        /// <summary>
        /// Method to get list of card tenders
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data ource</param>
        /// <returns>List of card tenders</returns>
        public List<CardTender> GetCardTenders(int saleNumber, DataSource dataSource)
        {
            var cardTenders = new List<CardTender>();
            var rsCard = GetRecords("Select *  FROM   CardTenders  WHERE  CardTenders.Sale_No = " + Convert.ToString(saleNumber) + " ", dataSource);
            foreach (DataRow row in rsCard.Rows)
            {
                cardTenders.Add(new CardTender
                {
                    CardType = CommonUtility.GetStringValue(row["Card_Type"]),
                    CardNum = CommonUtility.GetStringValue(row["Card_Number"]),
                    CardName = CommonUtility.GetStringValue(row["Card_Name"]),
                    ExpiryDate = CommonUtility.GetStringValue(row["Expiry_Date"]),
                    Swiped = CommonUtility.GetBooleanValue(row["Swiped"]),
                    ApprovalCode = CommonUtility.GetStringValue(row["Approval_Code"]),
                    Language = CommonUtility.GetStringValue(row["Language"]),
                    CustomerName = CommonUtility.GetStringValue(row["CustomerName"]),
                    TerminalID = CommonUtility.GetStringValue(row["TerminalID"]),
                    TransactionDate = CommonUtility.GetDateTimeValue(row["TransactionDate"]),
                    TransactionTime = CommonUtility.GetDateTimeValue(row["TransactionTime"]),
                    Amount = CommonUtility.GetDecimalValue(row["Amount"]),
                    SaleNumber = CommonUtility.GetIntergerValue(row["sale_no"]),
                    ResponseCode = CommonUtility.GetStringValue(row["ResponseCode"]),
                    ISOCode = CommonUtility.GetStringValue(row["ISOCode"]),
                    SequenceNumber = CommonUtility.GetStringValue(row["SequenceNumber"]),
                    DebitAccount = CommonUtility.GetStringValue(row["DebitAccount"]),
                    ReceiptDisplay = CommonUtility.GetStringValue(row["ReceiptDisplay"]),
                    VechicleNo = CommonUtility.GetStringValue(row["VechicleNo"]),
                    DriverNo = CommonUtility.GetStringValue(row["DriverNo"]),
                    IdentificationNo = CommonUtility.GetStringValue(row["IdentificationNo"]),
                    Odometer = CommonUtility.GetStringValue(row["Odometer"]),
                    CardUsage = CommonUtility.GetStringValue(row["CardUsage"]),
                    PrintDriverNo = CommonUtility.GetBooleanValue(row["PrintDriverNo"]),
                    PrintIdentificationNo = CommonUtility.GetBooleanValue(row["PrintIdentificationNo"]),
                    PrintOdometer = CommonUtility.GetBooleanValue(row["PrintOdometer"]),
                    PrintUsage = CommonUtility.GetBooleanValue(row["PrintUsage"]),
                    PrintVechicleNo = CommonUtility.GetBooleanValue(row["PrintVechicleNo"]),
                    StoreForward = CommonUtility.GetBooleanValue(row["Store_Forward"]),
                    Balance = CommonUtility.GetDecimalValue(row["Balance"]),
                    Result = CommonUtility.GetStringValue(row["Result"]),
                    Message = CommonUtility.GetStringValue(row["Message"]),
                    TillNumber = CommonUtility.GetIntergerValue(row["TILL_NUM"]),
                    PONumber = CommonUtility.GetStringValue(row["PONumber"])
                });
            }
            return cardTenders;
        }

        /// <summary>
        /// Method to get history message
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>History message</returns>
        public string GetHistoryMessage(int saleNumber, DataSource dataSource)
        {
            var dt = GetRecords("SELECT Message As Receipt FROM CardTenders WHERE Sale_No=" + Convert.ToString(saleNumber), dataSource);
            return dt.Rows.Count > 0 ? CommonUtility.GetStringValue(dt.Rows[0]["receipt"]) : string.Empty;
        }

        /// <summary>
        /// Method to get receipt of pay at pump
        /// </summary>
        /// <param name="invoiceNumber">Invoice number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Receipt</returns>
        public string GetReceipt(int invoiceNumber, DataSource dataSource)
        {
            var dt = GetRecords("select * from ReceiptHist where InvoiceNum="
                + Convert.ToString(invoiceNumber), dataSource);
            return dt.Rows.Count > 0 ? CommonUtility.GetStringValue(dt.Rows[0]["receipt"]) : null;
        }

        /// <summary>
        /// Method to get sale for pay at pump
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="payAtPumpTill">Pay at pump till</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>pay at pump</returns>
        public PayAtPump GetSaleForPayAtPump(int saleNumber, int payAtPumpTill,
            DataSource dataSource)
        {
            var rsHead = GetRecords("SELECT * FROM   SaleHead  WHERE  SaleHead.Sale_No = " + Convert.ToString(saleNumber) + " " + " AND   SaleHead.TILL = " + payAtPumpTill, dataSource);
            PayAtPump papRep = new PayAtPump { Sale_Num = saleNumber };
            var fields = rsHead.Rows[0];
            papRep.Sale_Amount = CommonUtility.GetDecimalValue(fields["Sale_amt"]);
            papRep.Customer.Code = CommonUtility.GetStringValue(fields["Client"]);
            var rsDiscountTender = GetRecords("select * from DiscountTender where SALE_NO="
                + Convert.ToString(saleNumber) + " AND TILL_NUM=" + payAtPumpTill, dataSource);

            if (rsDiscountTender.Rows.Count == 0) return papRep;
            var discountTender = rsDiscountTender.Rows[0];
            papRep.Customer.LoyaltyCard = CommonUtility.GetStringValue(discountTender["CardNum"]);

            papRep.CouponID = CommonUtility.GetStringValue(discountTender["CouponID"]);
            if (!string.IsNullOrEmpty(papRep.CouponID))
            {
                papRep.CouponTotal = CommonUtility.GetDecimalValue(discountTender["Discountamount"]);
            }
            return papRep;
        }

        /// <summary>
        /// Method to get sale lines for pay at pump sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="payAtPumpTill">Pay at pump sale</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale lines</returns>
        public List<Sale_Line> GetSaleLinesForPayAtPump(int saleNumber,
            int payAtPumpTill, DataSource dataSource)
        {
            // Load each line
            var rsLines = GetRecords("SELECT * FROM   SaleLine  WHERE  SaleLine.Sale_No = "
                + Convert.ToString(saleNumber) + "  " + " AND SaleLine.TILL_NUM = "
                + payAtPumpTill + " ORDER BY SaleLine.Line_Num ", dataSource);
            return (from DataRow saleLine in rsLines.Rows
                    select new Sale_Line
                    {
                        No_Loading = true,
                        Dept = CommonUtility.GetStringValue(saleLine["Dept"]),
                        Sub_Dept = CommonUtility.GetStringValue(saleLine["Sub_Dept"]),
                        Sub_Detail = CommonUtility.GetStringValue(saleLine["Sub_Detail"]),
                        Stock_Code = CommonUtility.GetStringValue(saleLine["Stock_Code"]),
                        PLU_Code = CommonUtility.GetStringValue(saleLine["PLU_Code"]),
                        pumpID = CommonUtility.GetByteValue(saleLine["pumpID"]),
                        PositionID = CommonUtility.GetByteValue(saleLine["PositionID"]),
                        GradeID = CommonUtility.GetByteValue(saleLine["GradeID"]),
                        Quantity = CommonUtility.GetFloatValue(saleLine["Quantity"]),
                        price = CommonUtility.GetDoubleValue(saleLine["price"]),
                        Amount = CommonUtility.GetDecimalValue(saleLine["Amount"]),
                        Discount_Adjust = CommonUtility.GetDoubleValue(saleLine["Disc_adj"]),
                        Line_Discount = CommonUtility.GetDoubleValue(saleLine["Discount"]),
                        Discount_Type = CommonUtility.GetStringValue(saleLine["Disc_Type"]),
                        Discount_Code = CommonUtility.GetStringValue(saleLine["Disc_Code"]),
                        Discount_Rate = CommonUtility.GetFloatValue(saleLine["Disc_Rate"]),
                        DiscountName = CommonUtility.GetStringValue(saleLine["DiscountName"])
                    }).ToList();
        }

        /// <summary>
        /// Method to get card tenders
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="payAtPumpTill">Pay at pump till</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Card reprint</returns>
        public Card_Reprint GetCardTender(int saleNumber, int payAtPumpTill, DataSource dataSource)
        {
            var papRepCard = new Card_Reprint();

            var rsCard = GetRecords("SELECT * FROM   CardTenders  WHERE  CardTenders.Sale_No = "
                + Convert.ToString(saleNumber) + " " + " AND   CardTenders.TILL_NUM = " + payAtPumpTill + " ", dataSource);

            foreach (DataRow card in rsCard.Rows)
            {
                papRepCard.Card_Type = CommonUtility.GetStringValue(card["Card_Type"]);
                papRepCard.CardNumber = CommonUtility.GetStringValue(card["Card_Number"]);
                papRepCard.Name = CommonUtility.GetStringValue(card["Card_name"]);
                papRepCard.Language = CommonUtility.GetStringValue(card["Language"]);
                papRepCard.DebitAccount = CommonUtility.GetStringValue(card["DebitAccount"]);
                papRepCard.Expiry_Date = CommonUtility.GetStringValue(card["Expiry_Date"]);
                papRepCard.ApprovalCode = CommonUtility.GetStringValue(card["Approval_Code"]);
                papRepCard.Sequence_Number = CommonUtility.GetStringValue(card["SequenceNumber"]) == "" ? "" : CommonUtility.GetStringValue(card["SequenceNumber"]);
                papRepCard.TerminalID = CommonUtility.GetStringValue(card["TerminalID"]);
                if (!string.IsNullOrEmpty(CommonUtility.GetStringValue(card["ISOCode"])))
                {
                    papRepCard.ResponseCode = CommonUtility.GetStringValue(card["ResponseCode"]) + " - " + CommonUtility.GetStringValue(card["ISOCode"]);
                }
                else
                {
                    papRepCard.ResponseCode = "";
                }
                papRepCard.Receipt_Display = CommonUtility.GetStringValue(card["ReceiptDisplay"]);
                papRepCard.Trans_Date = CommonUtility.GetDateTimeValue(card["TransactionDate"]);
                papRepCard.Trans_Time = CommonUtility.GetDateTimeValue(card["TransactionTime"]);
                papRepCard.Card_Type = CommonUtility.GetStringValue(card["Card_Type"]);
            }

            return papRepCard;
        }

        /// <summary>
        /// Method to get grade description
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <returns>Grade description</returns>
        public string GetGradeDescription(int gradeId)
        {
            DataTable dt = GetRecords("select shortname from grade where ID =" + gradeId, DataSource.CSCPump);
            return dt.Rows.Count != 0 ? CommonUtility.GetStringValue(dt.Rows[0]["shortname"]) : string.Empty;
        }

        /// <summary>
        /// Method to get sale tenders
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of sale tenders</returns>
        public List<SaleTend> GetSaleTenders(int saleNumber, DataSource dataSource)
        {
            var saleTenders = new List<SaleTend>();
            var rsTend = GetRecords(@"SELECT SaleTend.TILL_NUM, SaleTend.SALE_NO,
                         SaleTend.Sequence, SaleTend.TENDNAME, SaleTend.TENDCLAS, SaleTend.AMTTEND, 
                         SaleTend.AMTUSED, SaleTend.Exchange_Rate, SaleTend.SERNUM,SaleTend.CCARD_NUM, 
                         SaleTend.CCARD_APRV, D.CLASSDESC FROM SaleTend LEFT JOIN CSCMaster.dbo.TENDCLAS
                         as [D]  " + " ON SaleTend.TENDCLAS = D.TENDCLASS " + " WHERE SaleTend.Sale_No = "
                         + Convert.ToString(saleNumber) + " ", dataSource);
            foreach (DataRow row in rsTend.Rows)
            {
                saleTenders.Add(new SaleTend
                {
                    TenderClass = CommonUtility.GetStringValue(row["TENDCLAS"]),
                    TenderName = CommonUtility.GetStringValue(row["TendName"]),
                    Exchange = CommonUtility.GetDecimalValue(row["Exchange_Rate"]),
                    AmountTend = CommonUtility.GetDecimalValue(row["AmtTend"]),
                    AmountUsed = CommonUtility.GetDecimalValue(row["AmtUsed"]),
                    SequenceNumber = CommonUtility.GetIntergerValue(row["Sequence"]),
                    SerialNumber = CommonUtility.GetStringValue(row["SerNum"]),
                    ClassDescription = CommonUtility.GetStringValue(row["CLASSDESC"])
                });
            }

            return saleTenders;
        }

        /// <summary>
        /// Method to find whether card tender is available
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tenderName">Tender name</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>True or false</returns>
        public bool IsCardTenderAvailable(int saleNumber, string tenderName, DataSource dataSource)
        {
            var rsTendmast = GetRecords("select * from CardTenders where Sale_No=" + Convert.ToString(saleNumber)
                + " AND Tender_Name=\'" + tenderName + "\'" + " AND Card_Type=\'F\'", dataSource);
            return rsTendmast.Rows.Count > 0;
        }

        /// <summary>
        /// Method to find if tender is available
        /// </summary>
        /// <param name="tenderName">Tender name</param>
        /// <returns>True or false</returns>
        public bool IsTenderAvailable(string tenderName)
        {
            var rsTendmast = GetRecords("select * from TendMast " + " where TENDDESC=\'"
                + tenderName + "\'", DataSource.CSCMaster);
            return rsTendmast.Rows.Count > 0;
        }

        /// <summary>
        /// Method to get sale head
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale</returns>
        public Sale GetSaleHead(int saleNumber, DataSource dataSource)
        {
            var rsHead = GetRecords("Select *  FROM   SaleHead  WHERE  SaleHead.Sale_No = " + Convert.ToString(saleNumber) + " ", dataSource);
            var fields = rsHead.Rows[0];
            var salRep = new Sale
            {
                Sale_Date = CommonUtility.GetDateTimeValue(fields["Sale_Date"]),
                Sale_Time = CommonUtility.GetDateTimeValue(fields["Sale_Time"]),
                Register = CommonUtility.GetByteValue(fields["Regist"]),
                TillNumber = CommonUtility.GetByteValue(fields["Till"]),
                Shift = CommonUtility.GetByteValue(fields["Shift"]),
                Sale_Num = saleNumber,
                Sale_Deposit = CommonUtility.GetDecimalValue(fields["Deposit"]),
                Sale_Tender = CommonUtility.GetDecimalValue(fields["Tend_Amt"]),
                Sale_Type = CommonUtility.GetStringValue(fields["T_type"]),
                Customer = { Code = CommonUtility.GetStringValue(fields["Client"]) },
                Return_Reason =
                {
                    RType = CommonUtility.GetStringValue(fields["Reason_Type"]),
                    Reason = CommonUtility.GetStringValue(fields["Reason"])
                },
                OverPayment = CommonUtility.GetDecimalValue(fields["OverPayment"]),
                LoyaltyPoints = CommonUtility.GetDecimalValue(fields["LOYL_POINT"]),
                ReferenceNumber = CommonUtility.GetStringValue(fields["ReferenceNum"]),
                Sale_Totals = { Penny_Adj = CommonUtility.GetDecimalValue(fields["PENNY_ADJ"]) },
                TreatyNumber = CommonUtility.GetStringValue(fields["TreatyNumber"])
            };

            var rsDiscountTender = GetRecords("select * from DiscountTender where SALE_NO="
                + Convert.ToString(saleNumber) + " AND TILL_NUM=" +
                Convert.ToString(salRep.TillNumber), dataSource);
            if (rsDiscountTender.Rows.Count > 0)
            {
                salRep.Customer.LoyaltyCard = CommonUtility.GetStringValue(rsDiscountTender.Rows[0]["CardNum"]);
                salRep.CouponID = CommonUtility.GetStringValue(rsDiscountTender.Rows[0]["CouponID"]);
                if (salRep.CouponID.Trim().Length > 0)
                {
                    salRep.CouponTotal = CommonUtility.GetDecimalValue(rsDiscountTender.Rows[0]["Discountamount"]);
                }
                salRep.CBonusTotal = CommonUtility.GetDecimalValue(rsDiscountTender.Rows[0]["Discountamount"]);
            }
            salRep.Sale_Change = CommonUtility.GetDecimalValue(fields["Change"]);
            salRep.Sale_Totals.Invoice_Discount_Type = CommonUtility.GetStringValue(fields["Disc_Type"]);
            salRep.Sale_Totals.Invoice_Discount = CommonUtility.GetDecimalValue(fields["Invc_Disc"]);
            salRep.Vendor = CommonUtility.GetStringValue(fields["User"]);
            return salRep;
        }

        /// <summary>
        /// Method to get sale lines
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of sale lines</returns>
        public List<Sale_Line> GetSaleLines(int saleNumber, DataSource dataSource)
        {
            var saleLines = new List<Sale_Line>();
            var rsLines = GetRecords("Select *  FROM   SaleLine  WHERE  SaleLine.Sale_No = " + Convert.ToString(saleNumber) + "  " + "Order By SaleLine.Line_Num ", dataSource);
            foreach (DataRow saleLine in rsLines.Rows)
            {
                // The 'No_Loading' property just stops the Sale_Line object from
                // Loading Prices / Taxes / Charges from the database. In this instance
                // we want those attributes as they existed in the sale and not the
                // current ones from the database.
                var sl = new Sale_Line
                {
                    No_Loading = true,
                    Dept = CommonUtility.GetStringValue(saleLine["Dept"]),
                    Sub_Dept = CommonUtility.GetStringValue(saleLine["Sub_Dept"]),
                    Sub_Detail = CommonUtility.GetStringValue(saleLine["Sub_Detail"]),
                    Stock_Code = CommonUtility.GetStringValue(saleLine["Stock_Code"]),
                    PLU_Code = CommonUtility.GetStringValue(saleLine["PLU_Code"]),
                    Line_Num = CommonUtility.GetShortValue(saleLine["Line_Num"]),
                    price = CommonUtility.GetDoubleValue(saleLine["price"]),
                    Regular_Price = CommonUtility.GetDoubleValue(saleLine["Reg_Price"]),
                    Quantity = CommonUtility.GetFloatValue(saleLine["Quantity"]),
                    Amount = CommonUtility.GetDecimalValue(saleLine["Amount"]),
                    Total_Amount = CommonUtility.GetDecimalValue(saleLine["Total_Amt"]),
                    Discount_Adjust = CommonUtility.GetDoubleValue(saleLine["Disc_adj"]),
                    Line_Discount = CommonUtility.GetDoubleValue(saleLine["Discount"]),
                    Discount_Type = CommonUtility.GetStringValue(saleLine["Disc_Type"]),
                    Discount_Code = CommonUtility.GetStringValue(saleLine["Disc_Code"]),
                    Discount_Rate = CommonUtility.GetFloatValue(saleLine["Disc_Rate"]),
                    DiscountName = CommonUtility.GetStringValue(saleLine["DiscountName"]),
                    Associate_Amount = CommonUtility.GetDecimalValue(saleLine["Assoc_Amt"]),
                    User = CommonUtility.GetStringValue(saleLine["User"])
                };

                sl.Dept = CommonUtility.GetStringValue(saleLine["Dept"]);
                sl.Sub_Dept = CommonUtility.GetStringValue(saleLine["Sub_Dept"]);
                sl.Sub_Detail = CommonUtility.GetStringValue(saleLine["Sub_Detail"]);
                sl.Description = CommonUtility.GetStringValue(saleLine["Descript"]);
                sl.pumpID = CommonUtility.GetByteValue(saleLine["pumpID"]);
                sl.Stock_Code = CommonUtility.GetStringValue(saleLine["Stock_Code"]);
                sl.Prepay = CommonUtility.GetBooleanValue(saleLine["Prepay"]);
                sl.PaidByCard = CommonUtility.GetByteValue(saleLine["PaidByCard"]);
                sl.FuelRebate = CommonUtility.GetDecimalValue(saleLine["RebateDiscount"]);
                sl.FuelRebateEligible = CommonUtility.GetBooleanValue(saleLine["FuelRebateUsed"]);
                var strPromo = CommonUtility.GetStringValue(saleLine["PromoID"]);
                if (!string.IsNullOrEmpty(strPromo))
                {
                    sl.PromoID = strPromo;
                }
                sl.EligibleTaxEx = CommonUtility.GetBooleanValue(saleLine["ElgTaxExemption"]); //  
                sl.IsTaxExemptItem = Convert.ToBoolean(saleLine["TaxExempt"]);
                saleLines.Add(sl);
            }
            return saleLines;
        }

        /// <summary>
        /// Method to get line taxes
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Line taxes</returns>
        public Line_Taxes GetLineTaxes(int saleNumber, int lineNumber, DataSource dataSource)
        {
            var lineTaxes = new Line_Taxes();
            var rsLineTax = GetRecords("SELECT * FROM S_LineTax  WHERE  S_LineTax.Sale_No = "
                + Convert.ToString(saleNumber) + " AND " + "S_LineTax.Line_No = "
                + Convert.ToString(lineNumber) + " " + "ORDER BY S_LineTax.Tax_Name ",
                dataSource);
            foreach (DataRow lineTax in rsLineTax.Rows)
            {
                lineTaxes.Add(CommonUtility.GetStringValue(lineTax["Tax_Name"]),
                    CommonUtility.GetStringValue(lineTax["Tax_Code"]),
                    CommonUtility.GetFloatValue(lineTax["Tax_Rate"]),
                    CommonUtility.GetBooleanValue(lineTax["Tax_Included"]),
                    CommonUtility.GetFloatValue(lineTax["Tax_Rebate_Rate"]),
                   CommonUtility.GetDecimalValue(lineTax["Tax_Rebate"]), "");
            }

            return lineTaxes;
        }

        /// <summary>
        /// Method to get charges
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Charges</returns>
        public Charges GetCharges(int saleNumber, int lineNumber, DataSource dataSource)
        {
            var charges = new Charges();
            // Similarly, pick up the charges associated with the line.
            var rsLineChg = GetRecords("Select *  FROM   SaleChg  WHERE  SaleChg.Sale_No = "
                + Convert.ToString(saleNumber) + " AND " + " SaleChg.Line_No = "
                + Convert.ToString(lineNumber) + " " + "Order By SaleChg.As_Code ",
                dataSource);

            foreach (DataRow lineCharge in rsLineChg.Rows)
            {
                var rsLcTax = GetRecords("Select *  FROM   ChargeTax  WHERE  ChargeTax.Sale_No = "
                    + Convert.ToString(saleNumber) + " AND " + "ChargeTax.Line_No = "
                    + Convert.ToString(lineNumber) + " AND " + "ChargeTax.As_Code = \'"
                    + Convert.ToString(lineCharge["As_Code"]) + "\' ", dataSource);

                // Find any taxes that applied to those charges.
                var lct = new Charge_Taxes();
                foreach (DataRow lineTax in rsLcTax.Rows)
                {
                    lct.Add(CommonUtility.GetStringValue(lineTax["Tax_Name"]),
                        CommonUtility.GetStringValue(lineTax["Tax_Code"]),
                        CommonUtility.GetFloatValue(lineTax["Tax_Rate"]),
                       CommonUtility.GetBooleanValue(lineTax["Tax_Included"]), "");
                }

                charges.Add(Convert.ToString(lineCharge["As_Code"]),
                      Convert.ToString(lineCharge["Description"]),
                      Convert.ToSingle(lineCharge["price"]), lct, "");
            }

            return charges;
        }

        /// <summary>
        /// Method get tax saved
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Tax saved</returns>
        public float GetTaxSaved(int saleNumber, int lineNumber, DataSource dataSource)
        {
            var rsTaxExempt = GetRecords("Select *  FROM   PurchaseItem  WHERE  Sale_No = "
                           + Convert.ToString(saleNumber) + "  " + "AND Line_No = "
                           + Convert.ToString(lineNumber), dataSource);
            return rsTaxExempt.Rows.Count > 0 ? CommonUtility.GetFloatValue(rsTaxExempt.Rows[0]["TotalTaxSaved"]) : 0;
        }

        /// <summary>
        /// Method to get total exempted tax
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Exempted tax</returns>
        public float GetTotalExemptedTax(int saleNumber, int tillNumber,
            DataSource dataSource)
        {
            var rsTaxExempt = GetRecords("Select *  FROM TaxExemptSaleHead  WHERE  SALE_NO = " + Convert.ToString(saleNumber) + "  " + "AND TILL_NUM = " + Convert.ToString(tillNumber), dataSource);
            return rsTaxExempt.Rows.Count > 0 ? CommonUtility.GetFloatValue(rsTaxExempt.Rows[0]["TotalExemptedTax"]) : 0;
        }

        /// <summary>
        /// Method to get void number
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Void number</returns>
        public int GetVoidNo(int saleNumber, DataSource dataSource)
        {
            var rsVoid = GetRecords("SELECT Void_No FROM VoidSale  WHERE Sale_No= " + Convert.ToString(saleNumber) + "", dataSource);
            return rsVoid.Rows.Count != 0 ? CommonUtility.GetIntergerValue(rsVoid.Rows[0]["Void_No"]) : 0;
        }

        /// <summary>
        /// Method to load card sales
        /// </summary>
        /// <param name="db">Data source</param>
        /// <param name="tillId">Till number</param>
        /// <param name="saleNo">Sale number</param>
        /// <param name="lineNo">Line number</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <returns>True or false</returns>
        public bool Load_CardSales(DataSource db, short tillId, int saleNo,
            short lineNo, out GiveXReceiptType givexReceipt)
        {
            givexReceipt = new GiveXReceiptType();
            DataTable rs;
            if (tillId == 0)
            {
                if (lineNo == 0)
                {
                    rs = GetRecords("select * from CardSales where SALE_NO="
                        + Convert.ToString(saleNo), db);
                }
                else
                {
                    rs = GetRecords("select * from CardSales where SALE_NO="
                        + Convert.ToString(saleNo) + " and LINE_NUM=" + Convert.ToString(lineNo),
                        db);
                }
            }
            else
            {
                if (lineNo == 0)
                {
                    rs = GetRecords("select * from CardSales where TILL_NUM=" +
                        Convert.ToString(tillId) + " and SALE_NO=" +
                        Convert.ToString(saleNo), db);
                }
                else
                {
                    rs = GetRecords("select * from CardSales where TILL_NUM="
                        + Convert.ToString(tillId) + " and SALE_NO=" + Convert.ToString(saleNo) +
                        " and LINE_NUM=" + Convert.ToString(lineNo), db);
                }
            }
            if (rs.Rows.Count == 0) return false;
            var fields = rs.Rows[0];
            if (CommonUtility.GetStringValue(fields["CardType"]) != "G") return true;
            givexReceipt.Date = DateTime.Today.ToString("mm/dd/yyyy");
            givexReceipt.Time = DateTime.Now.TimeOfDay.ToString("hh:mm:ss");
            //givexReceipt.UserID = System.Convert.ToString(GiveX_Renamed.UserID);
            givexReceipt.CardNum = CommonUtility.GetStringValue(fields["CardNum"]);
            givexReceipt.SaleNum = saleNo;
            givexReceipt.Balance = CommonUtility.GetFloatValue(fields["CardBalance"]);
            givexReceipt.SaleAmount = CommonUtility.GetFloatValue(fields["SaleAmount"]);
            givexReceipt.PointBalance = CommonUtility.GetFloatValue(fields["PointBalance"]);
            givexReceipt.TranType = CommonUtility.GetShortValue(fields["SaleType"]);
            givexReceipt.ExpDate = CommonUtility.GetStringValue(fields["ExpDate"]);
            givexReceipt.SeqNum = CommonUtility.GetStringValue(fields["RefNum"]);
            givexReceipt.ResponseCode = CommonUtility.GetStringValue(fields["Response"]);
            return true;
        }

        /// <summary>
        /// Method to get line kits
        /// </summary>
        /// <param name="sn">Serial number</param>
        /// <param name="ln">Line number</param>
        /// <param name="db">Data source</param>
        /// <returns>Line kits</returns>
        public Line_Kits Get_Line_Kit(int sn, int ln, DataSource db)
        {
            var lk = new Line_Kits();

            // Get the kit items in the line
            var rsLineKit = GetRecords("Select *  FROM   SaleKit  WHERE  SaleKit.Sale_No = " + Convert.ToString(sn) + " AND " + "       SaleKit.Line_No = " + Convert.ToString(ln) + " ", db);

            foreach (DataRow lineKit in rsLineKit.Rows)
            {
                // Charges on Kit items
                var rsLineKitChg = GetRecords("Select *  FROM   SaleKitChg  WHERE  SaleKitChg.Sale_No = " + Convert.ToString(sn) + " AND "
                    + " SaleKitChg.Line_No = " + Convert.ToString(ln) + " AND " + "       SaleKitChg.Kit_Item = \'"
                    + Convert.ToString(lineKit["Kit_Item"]) + "\' ", db);

                var lkc = new K_Charges();
                foreach (DataRow lineKitChg in rsLineKitChg.Rows)
                {
                    // Taxes on Charges on Kit items
                    var rsCgt = GetRecords("Select *  FROM   SaleKitChgTax  WHERE  SaleKitChgTax.Sale_No  = "
                        + Convert.ToString(sn) + " AND " + " SaleKitChgTax.Line_No  = "
                        + Convert.ToString(ln) + " AND " + " SaleKitChgTax.Kit_Item = \'"
                        + CommonUtility.GetStringValue(lineKit["Kit_Item"]) + "\' AND "
                        + "       SaleKitChgTax.As_Code  = \'" + Convert.ToString(lineKitChg["As_Code"])
                        + "\' ", db);

                    var cgt = new Charge_Taxes();
                    foreach (DataRow taxCharge in rsCgt.Rows)
                    {
                        cgt.Add(CommonUtility.GetStringValue(taxCharge["Tax_Name"]),
                            CommonUtility.GetStringValue(taxCharge["Tax_Code"]),
                           CommonUtility.GetFloatValue(taxCharge["Tax_Rate"]),
                           CommonUtility.GetBooleanValue(taxCharge["Tax_Included"]), "");
                    }
                    lkc.Add(CommonUtility.GetDoubleValue(lineKitChg["price"]),
                        CommonUtility.GetStringValue(lineKitChg["Description"]),
                        CommonUtility.GetStringValue(lineKitChg["As_Code"]),
                        cgt, "");
                }

                lk.Add(CommonUtility.GetStringValue(lineKit["Kit_Item"]),
                   CommonUtility.GetStringValue(lineKit["Descript"]),
                  CommonUtility.GetFloatValue(lineKit["Quantity"]),
                 CommonUtility.GetFloatValue(lineKit["Base"]),
                 CommonUtility.GetFloatValue(lineKit["Fraction"]),
                 CommonUtility.GetFloatValue(lineKit["Alloc"]),
                 CommonUtility.GetStringValue(lineKit["Serial"]), lkc, "");
            }

            var returnValue = lk;

            return returnValue;
        }

        /// <summary>
        /// Method to get tax exempt sale
        /// </summary>
        /// <param name="sn">Sale number</param>
        /// <param name="tillId">TIll number</param>
        /// <param name="db">Data source</param>
        /// <param name="teType">Tax exempt type</param>
        /// <param name="checkQuota">Check for quota</param>
        /// <returns>Tax exempt sale</returns>
        public TaxExemptSale LoadTaxExempt(int sn, byte tillId, DataSource db,
            string teType, bool checkQuota = true)
        {
            var oTeSale = new TaxExemptSale();


            if (LoadGstExempt(sn, tillId, db, ref oTeSale))
            {
                oTeSale.Sale_Num = sn;
                oTeSale.TillNumber = tillId;
            }

            var rsHead = GetRecords("select * from TaxExemptSaleHead " + " where SALE_NO=" + Convert.ToString(sn) + " AND TILL_NUM=" + Convert.ToString(tillId), db);
            var rsLine = GetRecords("select * from TaxExemptSaleLine " + " where SALE_NO=" + Convert.ToString(sn) + " AND TILL_NUM=" + Convert.ToString(tillId), db);
            if (rsHead.Rows.Count == 0)
                return null;
            oTeSale.Sale_Num = sn;
            oTeSale.TillNumber = tillId;
            var fields = rsHead.Rows[0];
            foreach (DataRow line in rsLine.Rows)
            {
                var mTeLine = new TaxExemptSaleLine
                {
                    Quantity = CommonUtility.GetFloatValue(line["Quantity"]),
                    UnitsPerPkg = CommonUtility.GetFloatValue(line["UnitQuantity"]),
                    EquvQuantity = CommonUtility.GetFloatValue(line["EquvQuantity"]),
                    OriginalPrice = CommonUtility.GetFloatValue(line["OriginalPrice"]),
                    TaxFreePrice = CommonUtility.GetFloatValue(line["price"]),
                    Line_Num = CommonUtility.GetShortValue(line["Line_Num"]),
                    StockCode = CommonUtility.GetStringValue(line["Stock_Code"]),
                    TaxInclPrice = CommonUtility.GetFloatValue(line["TaxIncludedAmount"]),
                    Amount = CommonUtility.GetFloatValue(line["Amount"]),
                    ExemptedTax = CommonUtility.GetFloatValue(line["ExemptedTax"]),
                    Description = CommonUtility.GetStringValue(line["Description"]),
                    ProductCode = CommonUtility.GetStringValue(line["ProductCode"]),
                    ProductType =
                        DBNull.Value.Equals(line["ProductType"])
                            ? 0
                            : (mPrivateGlobals.teProductEnum)(line["ProductType"]),
                    RunningQuota = CommonUtility.GetFloatValue(line["RunningQuota"]),
                    OverLimit = CommonUtility.GetBooleanValue(line["OverLimit"]),
                    TaxExemptRate = CommonUtility.GetFloatValue(line["TaxExemptRate"])
                };


                bool tempCheckOverLimit = false;
                oTeSale.Add_a_Line(mTeLine, ref tempCheckOverLimit, ref checkQuota);
                if (!mTeLine.OverLimit) continue;
                switch (mTeLine.ProductType)
                {
                    case mPrivateGlobals.teProductEnum.eCigarette:
                    case mPrivateGlobals.teProductEnum.eCigar:
                    case mPrivateGlobals.teProductEnum.eLooseTobacco:
                        oTeSale.TobaccoOverLimit = true;
                        break;
                    case mPrivateGlobals.teProductEnum.eGasoline:
                    case mPrivateGlobals.teProductEnum.eDiesel:
                    case mPrivateGlobals.teProductEnum.emarkedGas:
                    case mPrivateGlobals.teProductEnum.emarkedDiesel:

                        oTeSale.GasOverLimit = true;
                        break;
                    case mPrivateGlobals.teProductEnum.ePropane:
                        oTeSale.PropaneOverLimit = true;
                        break;
                    case mPrivateGlobals.teProductEnum.eNone:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            oTeSale.Amount = CommonUtility.GetFloatValue(fields["SaleAmount"]);
            oTeSale.ShiftDate = CommonUtility.GetDateTimeValue(fields["ShiftDate"]);
            oTeSale.Sale_Time = DBNull.Value.Equals(fields["SaleTime"]) ? DateTime.Now : CommonUtility.GetDateTimeValue(fields["SaleTime"]);
            oTeSale.GasReason = CommonUtility.GetStringValue(fields["GasReason"]);
            oTeSale.GasReasonDesp = CommonUtility.GetStringValue(fields["GasReasonDesp"]);
            oTeSale.GasReasonDetail = CommonUtility.GetStringValue(fields["GasReasonDetail"]);
            oTeSale.PropaneReason = CommonUtility.GetStringValue(fields["PropaneReason"]);
            oTeSale.PropaneReasonDesp = CommonUtility.GetStringValue(fields["PropaneReasonDesp"]);
            oTeSale.PropaneReasonDetail = CommonUtility.GetStringValue(fields["PropaneReasonDetail"]);
            oTeSale.TobaccoReason = CommonUtility.GetStringValue(fields["TobaccoReason"]);
            oTeSale.TobaccoReasonDesp = CommonUtility.GetStringValue(fields["TobaccoReasonDesp"]);
            oTeSale.TobaccoReasonDetail = CommonUtility.GetStringValue(fields["TobaccoReasonDetail"]);
            oTeSale.TotalExemptedTax = CommonUtility.GetFloatValue(fields["TotalExemptedTax"]);
            oTeSale.Shift = CommonUtility.GetShortValue(fields["Shift"]);
            oTeSale.UserCode = CommonUtility.GetStringValue(fields["User"]);
            oTeSale.teCardholder.CardholderID = CommonUtility.GetStringValue(fields["CardholderID"]);
            oTeSale.teCardholder.Barcode = CommonUtility.GetStringValue(fields["Barcode"]);
            oTeSale.teCardholder.CardNumber = CommonUtility.GetStringValue(fields["cardnumber"]);
            if (teType == "QITE")
            {
                var rsCardHolder = GetRecords("select * from CLIENT " + " where CL_CODE=\'" + oTeSale.teCardholder.CardholderID + "\'", DataSource.CSCMaster);
                if (rsCardHolder.Rows.Count == 0) return oTeSale;
                oTeSale.teCardholder.Name = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["Cl_Name"]);
                oTeSale.teCardholder.Address = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["CL_Add1"]);
                oTeSale.teCardholder.City = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["CL_City"]);
                oTeSale.teCardholder.PlateNumber = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["PlateNumber"]);
                oTeSale.teCardholder.PostalCode = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["CL_Postal"]);
            }
            else // For AITE
            {
                var rsCardHolder = GetRecords("select * from TaxExemptCardRegistry " + " where CardholderID=\'" + oTeSale.teCardholder.CardholderID + "\'", DataSource.CSCMaster);
                if (rsCardHolder.Rows.Count != 0)
                {
                    oTeSale.teCardholder.Name = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["Name"]);
                }
            }
            return oTeSale;
        }

        /// <summary>
        /// Method to load gst exempt
        /// </summary>
        /// <param name="sn">Serial number</param>
        /// <param name="tillId">Till number</param>
        /// <param name="db">Data source</param>
        /// <param name="oteTax">Tax exempt sale</param>
        /// <returns>True or false</returns>
        public bool LoadGstExempt(int sn, byte tillId, DataSource db, ref TaxExemptSale oteTax)
        {

            var rsLine = GetRecords("select * from SALELINE " + " where SALE_NO=" +
                Convert.ToString(sn) + " AND TILL_NUM="
                + Convert.ToString(tillId), db);
            if (rsLine.Rows.Count == 0)
            {
                return false;
            }

            foreach (DataRow line in rsLine.Rows)
            {

                var rsTaxCreditLine = GetRecords("SELECT * from TaxCreditLine where TILL_NUM=" + Convert.ToString(tillId) + " AND SALE_NO=" + Convert.ToString(sn) + " AND Line_No=" + Convert.ToString(line["Line_Num"]), db);
                if (rsTaxCreditLine.Rows.Count == 0) continue;
                var tx = new TaxCreditLine { Line_Num = Convert.ToInt16(line["Line_Num"]) };
                foreach (DataRow creditLine in rsTaxCreditLine.Rows)
                {
                    var lt = new Line_Tax
                    {
                        Tax_Name = CommonUtility.GetStringValue(creditLine["Tax_Name"]),
                        Tax_Code = CommonUtility.GetStringValue(creditLine["Tax_Code"]),
                        Tax_Rate = CommonUtility.GetFloatValue(creditLine["Tax_Rate"]),
                        Tax_Included = CommonUtility.GetBooleanValue(creditLine["Tax_Included"]),
                        Tax_Added_Amount = CommonUtility.GetFloatValue(creditLine["Tax_Added_Amount"]),
                        Tax_Incl_Amount = CommonUtility.GetFloatValue(creditLine["Tax_Included_Amount"])
                    };
                    tx.Line_Taxes.AddTaxLine(lt, "");
                }
                oteTax.TaxCreditLines.AddLine(tx.Line_Num, tx, "");
            }


            var rsTaxCredit = GetRecords("SELECT * from TaxCredit where TILL_NUM=" + Convert.ToString(tillId)
                + " AND SALE_NO=" + Convert.ToString(sn), db);
            foreach (DataRow row in rsTaxCredit.Rows)
            {
                var sx = new Sale_Tax
                {
                    Tax_Name = CommonUtility.GetStringValue(row["Tax_Name"]),
                    Tax_Code = CommonUtility.GetStringValue(row["Tax_Code"]),
                    Tax_Rate = CommonUtility.GetFloatValue(row["Tax_Rate"]),
                    Taxable_Amount = CommonUtility.GetDecimalValue(row["Taxable_Amount"]),
                    Tax_Added_Amount = CommonUtility.GetDecimalValue(row["Tax_Added_Amount"]),
                    Tax_Included_Amount = CommonUtility.GetDecimalValue(row["Tax_Included_Amount"]),
                    Tax_Included_Total = CommonUtility.GetDecimalValue(row["Tax_Included_Total"])
                };
                oteTax.TaxCredit.Add(sx.Tax_Name, sx.Tax_Code, sx.Tax_Rate, sx.Taxable_Amount,
                    sx.Tax_Added_Amount, sx.Tax_Included_Amount, sx.Tax_Included_Amount,
                    sx.Tax_Rebate_Rate, sx.Tax_Rebate, sx.Tax_Name + sx.Tax_Code);
            }
            return false;
        }

        /// <summary>
        /// Method to check whether close batch data is available
        /// </summary>
        /// <param name="cashoutId">Cash out id</param>
        /// <returns></returns>
        public CloseBatch GetGivexCloseAvailableForCashOutId(string cashoutId)
        {
            var strSql = "select * from GiveXClose where CashOutID=\'" + cashoutId.Trim() + "\'";
            var rsBatch = GetRecords(strSql, DataSource.CSCTrans);
            if (rsBatch.Rows.Count == 0) return null;
            return new CloseBatch
            {
                BatchNumber = CommonUtility.GetStringValue(rsBatch.Rows[0]["ID"]),
                Date = CommonUtility.GetDateTimeValue(rsBatch.Rows[0]["BatchDate"]),
                Time = CommonUtility.GetDateTimeValue(rsBatch.Rows[0]["BatchTime"]),
                Report = CommonUtility.GetStringValue(rsBatch.Rows[0]["Report"])
            };
        }

        /// <summary>
        /// Method to get temp close batch for givex
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CloseBatch GetTempCloseBatchById(string id)
        {
            var rsTmp = GetRecords("select * from GiveXClose where ID=" + Convert.ToString(Convert.ToInt32(id) - 1), DataSource.CSCTrans);
            if (rsTmp.Rows.Count == 0) return null;
            return new CloseBatch
            {
                Date = CommonUtility.GetDateTimeValue(rsTmp.Rows[0]["BatchDate"]),
                Time = CommonUtility.GetDateTimeValue(rsTmp.Rows[0]["BatchTime"]),
            };
        }

        /// <summary>
        /// method to get the reciept corresponding to a card sale 
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <returns></returns>
        public string GetWexStringBySaleNumber(int saleNumber)
        {
            var rsTmp = GetRecords("select Message from CardTenders where Sale_No =" + saleNumber.ToString(),DataSource.CSCTills);
            if (rsTmp.Rows.Count == 0) return null;
            return CommonUtility.GetStringValue(rsTmp.Rows[0]["Message"]);
        }
    }
}

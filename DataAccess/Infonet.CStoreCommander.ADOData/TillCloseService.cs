using Infonet.CStoreCommander.Entities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Infonet.CStoreCommander.ADOData
{
    public class TillCloseService : SqlDbService, ITillCloseService
    {
        /// <summary>
        /// Method to check if suspended sales are available
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>True or false</returns>
        public bool AreSuspendedSales(int tillNumber)
        {
            var rsSuspend = GetRecords("select * from SusHead where SusHead.TILL=" + tillNumber, DataSource.CSCTills);
            return rsSuspend.Rows.Count > 0;
        }

        /// <summary>
        /// Method to check if last till
        /// </summary>
        /// <param name="till">Till</param>
        /// <returns>True or false</returns>
        public bool IsLastTill(Till till)
        {
            var returnValue = true;

            var rsTill = GetRecords("select * from TILLS where Active=1 and ShiftNumber=" + till.Shift + " and TILL_NUM<>100", DataSource.CSCMaster);
            foreach (DataRow existingTill in rsTill.Rows)
            {
                if (CommonUtility.GetShortValue(existingTill["Till_Num"]) != till.Number)
                {
                    returnValue = false;
                    break;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Method to delete records from current sale
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        public void DeleteCurrentSale(int tillNumber)
        {
            Execute("Delete  From SaleHead where Till=" + tillNumber, DataSource.CSCCurSale);

            Execute("Delete  From SaleLine where Till_Num=" + tillNumber, DataSource.CSCCurSale);
            Execute("Delete  From SLineReason where Till_Num=" + tillNumber, DataSource.CSCCurSale);
            Execute("Delete FROM CardSales WHERE Till_Num=" + tillNumber, DataSource.CSCCurSale);
            Execute("Delete FROM SaleTotals", DataSource.CSCCurSale);
        }

        /// <summary>
        /// Method to get system type
        /// </summary>
        /// <returns>System type</returns>
        public string GetSystemType()
        {
            var rs = GetRecords("Select SystemType From TankGaugeSetup", DataSource.CSCPump);
            if (rs.Rows.Count > 0)
            {
                return CommonUtility.GetStringValue(rs.Rows[0]["SystemType"]);
            }
            return string.Empty;
        }

        /// <summary>
        ///Method to get maximum dip number 
        /// </summary>
        /// <returns></returns>
        public int GetMaximumDipNumber()
        {

            var rs = GetRecords("Select Max(Dip_Number) As [MG]  FROM   TankDips ", DataSource.CSCPump);
            if (rs.Rows.Count == 0)
                return 1;
            var dip = CommonUtility.GetIntergerValue(rs.Rows[0]["MG"]);
            if (dip != 0)
            {
                return dip + 1;
            }
            return 1;
        }

        /// <summary>
        /// Method to add dip event
        /// </summary>
        /// <param name="dipNumber">Dip number</param>
        /// <param name="shiftDate">Shift date</param>
        public void AddDipEvent(short dipNumber, DateTime shiftDate)
        {
            var connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            var dataTable = new DataTable();
            var query = "select * from DipEvent " + " where Dip_Number=" + Convert.ToString(dipNumber);
            var adapter = new SqlDataAdapter(query, connection);
            adapter.Fill(dataTable);
            DataRow fields = dataTable.NewRow();
            fields["Dip_Number"] = dipNumber;
            fields["DIP_Date"] = shiftDate;
            dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            adapter.InsertCommand = builder.GetInsertCommand();
            adapter.Update(dataTable);
            connection.Close();
            adapter.Dispose();
        }

        /// <summary>
        /// Method to save tank dip
        /// </summary>
        /// <param name="tankDip">Tank dip</param>
        public void SaveTankDip(TankDip tankDip)
        {
            var connection = new SqlConnection(GetConnectionString(DataSource.CSCPump));
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            var dataTable = new DataTable();
            var query = "select * from TankDips " + " where Dip_Number=" + Convert.ToString(tankDip.DipNumber) + " AND TankID=" + Convert.ToString(tankDip.TankId);
            var adapter = new SqlDataAdapter(query, connection);
            adapter.Fill(dataTable);
            bool addNew = false;
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            DataRow fields;
            if (dataTable.Rows.Count == 0)
            {
                addNew = true;
                fields = dataTable.NewRow();
                fields["Dip_Number"] = tankDip.DipNumber;
                fields["TankID"] = tankDip.TankId;
                adapter.InsertCommand = builder.GetInsertCommand();
            }
            else
            {
                fields = dataTable.Rows[0];
                adapter.UpdateCommand = builder.GetUpdateCommand();
            }
            fields["Dip_Fuel"] = tankDip.FuelDip;
            fields["Dip_Water"] = tankDip.WaterDip;
            fields["TemperatureC"] = tankDip.Temperature;
            fields["DT"] = tankDip.Date;

            fields["ShiftDate"] = tankDip.ShiftDate;
            fields["ActualDateTime"] = tankDip.ReadTime;
            fields["GradeID"] = tankDip.GradeId;
            fields["Volume"] = tankDip.Volume;
            fields["Vllage"] = tankDip.Vllage;
            if (addNew)
                dataTable.Rows.Add(fields);
            adapter.Update(dataTable);
            connection.Close();
            adapter.Dispose();
        }

        /// <summary>
        /// Method to check if prepay exists for prepay globals
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns></returns>
        public bool IsPrepayGlobalsPresent(int tillNumber)
        {
            var count = GetRecordCount("select Count(*) from PrepayGlobal where TillID = " + tillNumber, DataSource.CSCTrans);
            return count > 0;
        }

        /// <summary>
        /// Method to get all bill coins
        /// </summary>
        /// <returns></returns>
        public List<BillCoin> GetBillCoins()
        {
            var billCoins = new List<BillCoin>();
            var rsBillCoin = GetRecords("Select *  FROM BillCoin " + "Order By BillCoin.BillCoin ", DataSource.CSCMaster);
            foreach (DataRow billCoin in rsBillCoin.Rows)
            {
                billCoins.Add(new BillCoin
                {
                    Description = CommonUtility.GetStringValue(billCoin["BC_Desc"]),
                    Value = CommonUtility.GetFloatValue(billCoin["BC_Amount"]).ToString(CultureInfo.InvariantCulture)
                });
            }
            return billCoins;
        }

        /// <summary>
        /// Method to get total overpayment
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Over payment</returns>
        public decimal GetOverPayment(int tillNumber)
        {
            var rsOverPay = GetRecords("Select   Sum(SaleHead.OverPayment) as [Used]  FROM     SaleHead  WHERE    SaleHead.T_Type IN (\'SALE\',\'REFUND\') " + "         and SaleHead.TILL=" + tillNumber, DataSource.CSCTills);
            if (rsOverPay.Rows.Count > 0)
                return CommonUtility.GetDecimalValue(rsOverPay.Rows[0]["USED"]);
            return 0;
        }

        /// <summary>
        /// Method to get total payments
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Payment</returns>
        public decimal GetPayments(int tillNumber)
        {
            var rsPayments = GetRecords("Select   Sum(SaleHead.Payment) as [Used]  FROM     SaleHead  WHERE    SaleHead.T_Type = \'PAYMENT\' " + "         and SaleHead.TILL=" + tillNumber, DataSource.CSCTills);

            if (rsPayments.Rows.Count > 0)
                return CommonUtility.GetDecimalValue(rsPayments.Rows[0]["USED"]);

            return 0;
        }

        /// <summary>
        /// Method to get total AR payment 
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>AR Pay</returns>
        public decimal GetArPayment(int tillNumber)
        {
            var rsArPay = GetRecords("Select   Sum(SaleHead.Payment) as [Used]  FROM     SaleHead  WHERE    SaleHead.T_Type = \'ARPAY\' " + "         and SaleHead.TILL=" + tillNumber, DataSource.CSCTills);
            if (rsArPay.Rows.Count > 0)

                return CommonUtility.GetDecimalValue(rsArPay.Rows[0]["USED"]);
            //    Debug.Print ARPay

            return 0;
        }

        /// <summary>
        /// Method to get total payouts
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Payout</returns>
        public decimal GetPayouts(int tillNumber)
        {
            var rsPayOuts = GetRecords("Select   Sum(SaleHead.PayOut) as [Used]  FROM     SaleHead  WHERE    SaleHead.T_Type = \'PAYOUT\' " + "         and SaleHead.TILL=" + tillNumber, DataSource.CSCTills);

            if (rsPayOuts.Rows.Count > 0)

                return CommonUtility.GetDecimalValue(rsPayOuts.Rows[0]["USED"]);
            //    Debug.Print ARPay

            return 0;
        }

        /// <summary>
        /// Method to get total penny adjustment
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Penny adjustment</returns>
        public decimal GetPennyAdjustment(int tillNumber)
        {
            var rsPennyAdj = GetRecords("Select   Sum(SaleHead.Penny_Adj) as [PennyAdj]  FROM     SaleHead  WHERE    SaleHead.TILL=" + tillNumber, DataSource.CSCTills);

            if (rsPennyAdj.Rows.Count > 0)

                return CommonUtility.GetDecimalValue(rsPennyAdj.Rows[0]["PennyAdj"]);
            //    Debug.Print ARPay

            return 0;

        }

        /// <summary>
        /// Method to get total change
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Change</returns>
        public double GetChangeSum(int tillNumber)
        {
            var rsChange = GetRecords("Select Sum(SaleHead.Change) as [SumChg]  FROM   SaleHead  WHERE  SaleHead.T_Type NOT IN (\'VOID\',\'CANCEL\',\'SUSPEND\') " + "       and SaleHead.TILL=" + tillNumber, DataSource.CSCTills);

            if (rsChange.Rows.Count > 0)

                return CommonUtility.GetDoubleValue(rsChange.Rows[0]["SumChg"]);
            //    Debug.Print ARPay

            return 0;
        }

        /// <summary>
        /// Method to get draw and bonus
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Draw and bonus</returns>
        public List<decimal> GetDrawAndBonus(int tillNumber)
        {
            var result = new List<decimal>();
            var rsDraw = GetRecords("Select Count(*) as [nDraw], " + "       Sum(CashDraw.Amount) as [Draw_Total] ,sum(CashDraw.CashBonus) as [CashBonus] FROM   CashDraw where CashDraw.TILL=" + tillNumber, DataSource.CSCTills);
            if (rsDraw.Rows.Count == 0)
            {
                result.Add(0);
                result.Add(0);
            }
            else
            {
                result.Add(CommonUtility.GetDecimalValue(rsDraw.Rows[0]["Draw_Total"]));
                result.Add(CommonUtility.GetDecimalValue(rsDraw.Rows[0]["CashBonus"]));
            }

            return result;
        }

        /// <summary>
        /// Method to clear previous till close
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        public void ClearPreviousTillClose(int tillNumber)
        {
            Execute("Delete  From TillClose where TILL_Num=" + tillNumber, DataSource.CSCTills);
        }

        /// <summary>
        /// Method to get included tenders
        /// </summary>
        /// <returns>List of tender</returns>
        public List<Tender> GetIncludedTenders()
        {
            var tenders = new List<Tender>();
            var records = GetRecords("Select   *  FROM     TendMast  WHERE    TendMast.IncludeInClose = 1 ", DataSource.CSCMaster);
            foreach (DataRow record in records.Rows)
            {
                tenders.Add(new Tender
                {
                    Tender_Code = CommonUtility.GetStringValue(record["TENDCODE"]),
                    TendDescription = CommonUtility.GetStringValue(record["TENDDESC"])
                });
            }

            return tenders;
        }

        /// <summary>
        /// Method to get sale tenders
        /// </summary>
        /// <param name="tillNumber">till number</param>
        /// <returns>List of sale tender</returns>
        public List<SaleTendAmount> GetSaleTendAmountForTill(int tillNumber)
        {
            var saleTenders = new List<SaleTendAmount>();
            var rsTenders = GetRecords("Select   SaleTend.TendName     as [Tender], " + "         Count(*)              as [Count], " + "         Sum(SaleTend.AmtTend) as [Amount], " + "         Sum(SaleTend.AmtUsed) as [Used]   FROM     SaleTend INNER JOIN SaleHead ON " + "         SaleTend.Sale_No = SaleHead.Sale_No  WHERE    (SaleTend.TendName <> \'Store Credit\' OR SaleTend.AmtTend > 0) AND " + "         SaleHead.T_Type NOT IN (\'VOID\',\'CANCEL\',\'SUSPEND\') " + "         and SaleHead.TILL=" + tillNumber + "Group By SaleTend.TendName ", DataSource.CSCTills);
            foreach (DataRow tender in rsTenders.Rows)
            {
                saleTenders.Add(new SaleTendAmount
                {
                    Tender = CommonUtility.GetStringValue(tender["Tender"]),
                    Count = CommonUtility.GetIntergerValue(tender["Count"]),
                    Amount = CommonUtility.GetDecimalValue(tender["Amount"]),
                    Used = CommonUtility.GetDecimalValue(tender["Used"])
                });
            }
            return saleTenders;
        }

        /// <summary>
        /// Method to get bonus give away
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Bonus giveaway</returns>
        public decimal GetBonusGiveAway(int tillNumber)
        {
            var rsbonus = GetRecords("Select   sum(DiscountAmount) as [Bonus]  FROM     DiscountTender  WHERE   DiscountType = \'B\' and  TILL_NUM=" + tillNumber, DataSource.CSCTills);
            if (rsbonus.Rows.Count != 0)
            {

                return CommonUtility.GetDecimalValue(rsbonus.Rows[0]["Bonus"]);
            }
            return 0;

        }

        /// <summary>
        /// Method to get bonus drop
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="cbt">Cash bonus</param>
        /// <returns>Bonus drop</returns>
        public decimal GetBonusDrop(int tillNumber, string cbt)
        {
            var rsbonus = GetRecords("Select   DropLines.Tender_Name, " + "         Sum(Amount) as [Drop_Amt], " + "         Sum(Conv_Amount) as [Drop_Conv]  FROM     DropLines  WHERE    Tender_name = \'" + cbt + "\' and TILL_NUM=" + tillNumber + "Group By DropLines.Tender_Name ", DataSource.CSCTills);
            if (rsbonus.Rows.Count != 0)
            {

                return CommonUtility.GetDecimalValue(rsbonus.Rows[0]["Drop_Amt"]);
            }
            return 0;
        }

        /// <summary>
        /// Method to get drop lines
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of drop line</returns>
        public List<DropLine> GetDropLinesForTill(int tillNumber)
        {
            var dropLines = new List<DropLine>();
            var rsDrop = GetRecords("Select   DropLines.Tender_Name, " + "         Sum(Amount) as [Drop_Amt], "
                + "         Sum(Conv_Amount) as [Drop_Conv]  FROM     DropLines  WHERE    TILL_NUM=" + tillNumber
                + "Group By DropLines.Tender_Name ", DataSource.CSCTills);

            foreach (DataRow dropLine in rsDrop.Rows)
            {
                dropLines.Add(new DropLine
                {
                    TenderName = CommonUtility.GetStringValue(dropLine["Tender_Name"]),
                    Amount = CommonUtility.GetDecimalValue(dropLine["Drop_Amt"]),
                    ConvAmount = CommonUtility.GetDecimalValue(dropLine["Drop_Conv"])
                });
            }
            return dropLines;
        }

        /// <summary>
        /// Method to get till close 
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns></returns>
        public List<TillClose> GetTillCloseByTillNumber(int tillNumber)
        {
            var tillCloses = new List<TillClose>();
            var records = GetRecords("SELECT Till_Num, Tender, Count, Entered, " + "System, Difference FROM TillClose  WHERE TILL_Num=" + tillNumber, DataSource.CSCTills);
            foreach (DataRow row in records.Rows)
            {
                tillCloses.Add(new TillClose
                {
                    TillNumber = CommonUtility.GetShortValue(row["Till_Num"]),
                    Tender = CommonUtility.GetStringValue(row["Tender"]),
                    Count = CommonUtility.GetIntergerValue(row["Count"]),
                    Entered = CommonUtility.GetDoubleValue(row["Entered"]),
                    System = CommonUtility.GetDoubleValue(row["System"]),
                    Difference = CommonUtility.GetDoubleValue(row["Difference"])
                });
            }
            return tillCloses;
        }

        /// <summary>
        /// Method to get till close 
        /// </summary>
        /// <returns></returns>
        public List<TillClose> GetTillCloseForTrans()
        {
            var tillCloses = new List<TillClose>();
            var records = GetRecords("SELECT* FROM TillClose ", DataSource.CSCTrans);
            foreach (DataRow row in records.Rows)
            {
                tillCloses.Add(new TillClose
                {
                    Tender = CommonUtility.GetStringValue(row["Tender"]),
                    Count = CommonUtility.GetIntergerValue(row["Count"]),
                    Entered = CommonUtility.GetDoubleValue(row["Entered"]),
                    System = CommonUtility.GetDoubleValue(row["System"]),
                    Difference = CommonUtility.GetDoubleValue(row["Difference"])
                });
            }
            return tillCloses;
        }

        /// <summary>
        /// Method to add tll close
        /// </summary>
        /// <param name="tillClose"></param>
        public void AddTillClose(TillClose tillClose)
        {
            var connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            var dataTable = new DataTable();
            var query = "select * from TillClose where Till_Num =" + tillClose.TillNumber + " and Tender = '"
                + tillClose.Tender + "'";
            var adapter = new SqlDataAdapter(query, connection);
            adapter.Fill(dataTable);
            if (dataTable.Rows.Count == 0)
            {
                DataRow fields = dataTable.NewRow();
                fields["Till_Num"] = tillClose.TillNumber;
                fields["Tender"] = tillClose.Tender;
                fields["Count"] = tillClose.Count;
                fields["Entered"] = tillClose.Entered;
                fields["System"] = tillClose.System;
                fields["Difference"] = tillClose.Difference;
                dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                adapter.InsertCommand = builder.GetInsertCommand();
                adapter.Update(dataTable);
                connection.Close();
                adapter.Dispose();
            }
            else
            {
                UpdateTillClose(tillClose);
            }
        }

        /// <summary>
        /// Method to add tll close
        /// </summary>
        /// <param name="tillClose"></param>
        public void AddTillCloseForTrans(TillClose tillClose)
        {
            var query =
                $"Insert into TillClose (Tender,Count,Entered,System,Difference) values('{tillClose.Tender}', {tillClose.Count},{tillClose.Entered},{tillClose.System},{tillClose.Difference})";
            Execute(query, DataSource.CSCTrans);
        }

        /// <summary>
        /// Method to update till close
        /// </summary>
        /// <param name="selectedTillClose">Selected till close</param>
        public void UpdateTillClose(TillClose selectedTillClose)
        {
            var connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            var dataTable = new DataTable();
            var query = "select * from TillClose where Till_Num =" + selectedTillClose.TillNumber + " and Tender = '"
                + selectedTillClose.Tender + "'";
            var adapter = new SqlDataAdapter(query, connection);
            adapter.Fill(dataTable);
            DataRow fields = dataTable.Rows[0];
            fields["Count"] = selectedTillClose.Count;
            fields["Entered"] = selectedTillClose.Entered;
            fields["System"] = selectedTillClose.System;
            fields["Difference"] = selectedTillClose.Difference;
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            adapter.UpdateCommand = builder.GetUpdateCommand();
            adapter.Update(dataTable);
            connection.Close();
            adapter.Dispose();
        }

        /// <summary>
        /// Method to update till close
        /// </summary>
        /// <param name="tillClose">Till close</param>
        public void UpdateTillCloseForTrans(TillClose tillClose)
        {
            var query =
                $"Update TillClose set Count = {tillClose.Count}, Entered ={tillClose.Entered}, System = {tillClose.System}, Difference = {tillClose.Difference} where  Tender = '{tillClose.Tender}'";
            Execute(query, DataSource.CSCTrans);
        }

        /// <summary>
        /// Method to maximum close head
        /// </summary>
        /// <returns></returns>
        public int GetMaxCloseHead()
        {
            int cn;

            var connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            var dataTable = new DataTable();
            var adapter = new SqlDataAdapter("Select *  FROM   Close_Head Order by Close_Head.Close_Num DESC ", connection);
            adapter.Fill(dataTable);

            if (dataTable.Rows.Count == 0)
            {
                cn = 1;
            }
            else
            {
                cn = CommonUtility.GetIntergerValue(dataTable.Rows[0]["Close_Num"]) + 1;
            }
            connection.Close();
            adapter.Dispose();
            return cn;
        }

        /// <summary>
        /// Method to get total credits
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where condition</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        public decimal GetTotalCredits(int tillNumber, string whereClause, DataSource dataSource)
        {
            DataTable rs;

            // Pick up sale credits issued.
            rs = !string.IsNullOrEmpty(whereClause) ? GetRecords("Select Sum(SaleHead.Credits) as [Credits]  FROM   SaleHead  WHERE  SaleHead.T_Type NOT IN (\'VOID\',\'SUSPEND\') AND " + " Till=" + tillNumber + " AND " + whereClause, dataSource) : GetRecords("Select Sum(SaleHead.Credits) as [Credits]  FROM   SaleHead  WHERE  SaleHead.T_Type NOT IN (\'VOID\',\'SUSPEND\') " + " and SaleHead.Till=" + tillNumber, dataSource);
            if (rs.Rows.Count == 0)
                return 0;
            return CommonUtility.GetDecimalValue(rs.Rows[0]["CREDITS"]);
        }

        /// <summary>
        /// Method to get ezipin terminal Id
        /// </summary>
        /// <returns>Terminal id</returns>
        public string GetEzipinTerminalId()
        {
            var rsEzi = GetRecords("SELECT TerminalID FROM TerminalIds where TerminalType=\'Ezipin\'", DataSource.CSCAdmin);
            if (rsEzi.Rows.Count == 0)
                return string.Empty;
            return CommonUtility.GetStringValue(rsEzi.Rows[0]["TerminalID"]);
        }

        /// <summary>
        /// Method to get maximum and minimum sale number
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale numbers</returns>
        public List<int> GetMaxMinSaleNumber(int tillNumber, DataSource dataSource)
        {
            var saleNumbers = new List<int>();
            var rs = GetRecords("select MAX(SALE_NO) as MaxSL, MIN(SALE_NO) as MinSL from SALEHEAD WHERE TILL = " + tillNumber, dataSource);
            if (rs.Rows.Count == 0)
                return saleNumbers;
            saleNumbers.Add(CommonUtility.GetIntergerValue(rs.Rows[0]["MaxSL"]));
            saleNumbers.Add(CommonUtility.GetIntergerValue(rs.Rows[0]["MinSL"]));
            return saleNumbers;
        }

        /// <summary>
        /// Method to get sale heads
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where condition</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale heads</returns>
        public List<SaleHead> GetSaleHeads(int tillNumber, string whereClause, DataSource dataSource)
        {
            var saleHeads = new List<SaleHead>();
            DataTable rs;
            rs = string.IsNullOrEmpty(whereClause) ? GetRecords("Select SaleHead.T_Type as [TType], " + "       Count(*) as [Transactions], " + "       Sum(SALE_AMT) As [Amount], " + "       Sum(PAYMENT) as [Pays]  FROM   SaleHead  WHERE  SaleHead.T_Type <> \'CANCEL\' " + " AND   SaleHead.Till=" + tillNumber + " Group By SaleHead.T_Type ", dataSource) : GetRecords("Select SaleHead.T_Type as [TType], " + "       Count(*) as [Transactions], " + "       Sum(SALE_AMT) As [Amount], " + "       Sum(PAYMENT) as [Pays]  FROM   SaleHead  WHERE  SaleHead.T_Type <> \'CANCEL\' AND " + "    SaleHead.Till=" + tillNumber + " AND " + whereClause + " " + "Group By SaleHead.T_Type ", dataSource);

            foreach (DataRow saleHead in rs.Rows)
            {
                saleHeads.Add(new SaleHead
                {
                    TType = CommonUtility.GetStringValue(saleHead["TType"]),
                    CloseNum = CommonUtility.GetIntergerValue(saleHead["Transactions"]),
                    SaleAmount = CommonUtility.GetDecimalValue(saleHead["Amount"]),
                    Payment = CommonUtility.GetDecimalValue(saleHead["Pays"])
                });
            }
            return saleHeads;
        }

        /// <summary>
        /// Method to get total transactions
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where condition</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Total transactions</returns>
        public int GetTotalTransactions(int tillNumber, string whereClause, DataSource dataSource)
        {
            DataTable rs;
            rs = string.IsNullOrEmpty(whereClause) ? GetRecords("Select Count(*) as [Transactions]  FROM   SaleHead  WHERE  SaleHead.T_Type <> \'CANCEL\' " + "   AND SaleHead.Till=" + tillNumber, dataSource) : GetRecords("Select Count(*) as [Transactions]  FROM   SaleHead  WHERE  SaleHead.T_Type <> \'CANCEL\' AND " + " SaleHead.Till=" + tillNumber + " AND " + whereClause, dataSource);

            if (rs.Rows.Count == 0)
            {
                return 0;
            }
            return CommonUtility.GetIntergerValue(rs.Rows[0]["Transactions"]);
        }

        /// <summary>
        /// Method to check whether card sales are available
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>True or false</returns>
        public bool AreCardSalesAvailable(int tillNumber, string whereClause, DataSource dataSource)
        {
            DataTable rs;
            rs = string.IsNullOrEmpty(whereClause) ? GetRecords("select * from CardSales where TILL_NUM=" + tillNumber + " and CardType=\'G\'", dataSource) : GetRecords("select * from CardSales INNER JOIN SaleHead ON SaleHead.Till=CardSales.Till_Num AND SaleHead.Sale_No=CardSales.Sale_No where " + " CardType=\'G\' and " + whereClause, dataSource);
            return rs.Rows.Count > 0;

        }

        /// <summary>
        /// Method to get card sales
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Card sales</returns>
        public List<CardSale> GetCardSales(int tillNumber, string whereClause, DataSource dataSource)
        {
            var cardSales = new List<CardSale>();
            DataTable rs;
            rs = string.IsNullOrEmpty(whereClause) ? GetRecords("select COUNT(*) as SaleCNT, SaleType, " + " SUM(SaleAmount) as AmountSum from CardSales " + " where TILL_NUM=" + tillNumber + " and CardType=\'G\'" + " GROUP BY SaleType ORDER BY SaleType", dataSource) : GetRecords("select COUNT(*) as SaleCNT, SaleType, " + " SUM(SaleAmount) as AmountSum from CardSales " + " where TILL_NUM=" + tillNumber + " and CardType=\'G\' " + " AND " + whereClause + " GROUP BY SaleType ORDER BY SaleType", dataSource);
            foreach (DataRow cardSale in rs.Rows)
            {
                cardSales.Add(new CardSale
                {
                    SaleType = CommonUtility.GetStringValue(cardSale["SaleType"]),
                    SaleAmount = CommonUtility.GetDecimalValue(cardSale["AmountSum"]),
                    LineNumber = CommonUtility.GetIntergerValue(cardSale["SaleCNT"])
                });
            }
            return cardSales;
        }

        /// <summary>
        /// Method to get total cash
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="bt">Base tender</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Total cash</returns>
        public decimal GetTotalCash(int tillNumber, string whereClause, string bt, DataSource dataSource)
        {
            DataTable rs;
            rs = string.IsNullOrEmpty(whereClause) ? GetRecords("SELECT sum(amttend) as [SumCash] " + " FROM saletend INNER JOIN SaleHead ON " + "  SaleTend.Sale_No = SaleHead.Sale_No  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') " + "and tendname = \'" + bt + "\' and Till=" + tillNumber, dataSource) : GetRecords("SELECT sum(amttend) as [SumCash] " + " FROM saletend INNER JOIN SaleHead ON " + "  SaleTend.Sale_No = SaleHead.Sale_No  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') " + "and tendname = \'" + bt + "\' and " + whereClause, dataSource);

            if (rs.Rows.Count != 0)
            {
                return CommonUtility.GetDecimalValue(rs.Rows[0]["sumcash"]);
            }

            return 0;
        }

        /// <summary>
        /// Method to get total change
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Total change</returns>
        public decimal GetTotalChange(int tillNumber, string whereClause, DataSource dataSource)
        {
            DataTable rs;
            rs = string.IsNullOrEmpty(whereClause) ? GetRecords("SELECT sum(CHANGE) as [SumChange] " + " FROM SaleHead  WHERE SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\')" + " and TILL=" + tillNumber, dataSource) : GetRecords("SELECT sum(CHANGE) as [SumChange] " + " FROM SaleHead  WHERE SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\')" + " and " + whereClause, dataSource);

            if (rs.Rows.Count != 0)
            {
                return CommonUtility.GetDecimalValue(rs.Rows[0]["sumChange"]);
            }
            return 0;
        }

        /// <summary>
        /// Method to get total payment
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="bt">Base tender</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>toral payment</returns>
        public decimal GetTotalPayment(int tillNumber, string whereClause, string bt, DataSource dataSource)
        {
            DataTable rs;
            rs = string.IsNullOrEmpty(whereClause) ? GetRecords("SELECT sum(amttend) as [sumAmount]," + " sum(Change) as [sumChange] " + " FROM saletend INNER JOIN SaleHead ON " + "  SaleTend.Sale_No = SaleHead.Sale_No  WHERE  SaleHead.T_Type = \'PAYMENT\'" + " and tendname = \'" + bt + "\' and TILL_NUM=" + tillNumber, dataSource) : GetRecords("SELECT sum(amttend) as [sumAmount]," + " sum(Change) as [sumChange] " + " FROM saletend INNER JOIN SaleHead ON " + "  SaleTend.Sale_No = SaleHead.Sale_No  WHERE  SaleHead.T_Type = \'PAYMENT\'" + " and tendname = \'" + bt + "\' and " + whereClause, dataSource);

            if (rs.Rows.Count != 0)
            {
                return CommonUtility.GetDecimalValue(rs.Rows[0]["sumAmount"]) + CommonUtility.GetDecimalValue(rs.Rows[0]["sumChange"]);

            }
            return 0;
        }

        /// <summary>
        /// Method to get total ar payment
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="bt">Base tender</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Total ar payment</returns>
        public decimal GetTotalArPayment(int tillNumber, string whereClause, string bt, DataSource dataSource)
        {
            DataTable rs;
            rs = string.IsNullOrEmpty(whereClause) ? GetRecords("SELECT sum(amttend) as [sumAmount] ," + " sum(Change) as [sumChange] " + " FROM saletend INNER JOIN SaleHead ON " + "  SaleTend.Sale_No = SaleHead.Sale_No  WHERE  SaleHead.T_Type = \'ARPAY\'" + " and tendname = \'" + bt + "\' and TILL_NUM=" + tillNumber, dataSource) : GetRecords("SELECT sum(amttend) as [sumAmount]," + " sum(Change) as [sumChange] " + " FROM saletend INNER JOIN SaleHead ON " + "  SaleTend.Sale_No = SaleHead.Sale_No  WHERE  SaleHead.T_Type = \'PAYMENT\'" + " and tendname = \'" + bt + "\' and " + whereClause, dataSource);
            if (rs.Rows.Count != 0)
            {
                return CommonUtility.GetDecimalValue(rs.Rows[0]["sumAmount"]) + CommonUtility.GetDecimalValue(rs.Rows[0]["sumChange"]);
            }
            return 0;
        }

        /// <summary>
        /// Method to get total draw
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftDate">Shift date</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Total draw</returns>
        public decimal GetTotalDraw(int tillNumber, DateTime shiftDate, string whereClause, DataSource dataSource)
        {
            DataTable rs;
            if (string.IsNullOrEmpty(whereClause))
            {
                rs = GetRecords("SELECT   SUM(amount) AS [sumAmount]  FROM     Cashdraw  where TILL=" + tillNumber, dataSource);
            }
            else
            {
                rs = GetRecords("SELECT   SUM(amount) AS [sumAmount]  FROM     Cashdraw  WHERE Draw_Date=\'" +
                    shiftDate.ToString("yyyyMMdd") + "\'", dataSource);
            }

            if (rs.Rows.Count != 0)
            {
                return CommonUtility.GetDecimalValue(rs.Rows[0]["sumAmount"]);
            }
            return 0;
        }

        /// <summary>
        /// Method to get total payout
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Total payout</returns>
        public decimal GetTotalPayout(int tillNumber, string whereClause, DataSource dataSource)
        {
            DataTable rs;
            rs = string.IsNullOrEmpty(whereClause) ? GetRecords("SELECT   SUM(SaleHead.PayOut) AS [sumAmount]  FROM     SaleHead  WHERE    SaleHead.T_Type = \'PAYOUT\' " + " AND     SaleHead.TILL=" + tillNumber, dataSource) : GetRecords("SELECT   SUM(SaleHead.PayOut) AS [sumAmount]  FROM     SaleHead  WHERE    SaleHead.T_Type = \'PAYOUT\' " + " AND " + whereClause, dataSource);

            if (rs.Rows.Count != 0)
            {
                return CommonUtility.GetDecimalValue(rs.Rows[0]["sumAmount"]);
            }
            return 0;
        }

        /// <summary>
        /// Method to get total drop
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="shiftDate">Shift date</param>
        /// <param name="bt">Base tender</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Total drop</returns>
        public decimal GetTotalDrop(int tillNumber, string whereClause, DateTime shiftDate, string bt, DataSource dataSource)
        {
            DataTable rs;
            if (string.IsNullOrEmpty(whereClause))
            {
                rs = GetRecords("SELECT   SUM(amount) AS [sumAmount]  FROM     DropLines  where tender_name = \'" + bt + "\'" + "  AND    TILL_NUM=" + tillNumber, dataSource);
            }
            else
            {
                rs = GetRecords("SELECT   SUM(amount) AS [sumAmount]  FROM     DropLines  where tender_name = \'" + bt + "\'" + "  AND DropDate=\'" +
                    shiftDate.ToString("yyyyMMdd") + "\'", dataSource);
            }

            if (rs.Rows.Count != 0)
            {
                return CommonUtility.GetDecimalValue(rs.Rows[0]["sumAmount"]);
            }
            return 0;
        }

        /// <summary>
        /// Method to get total bottle return
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Total bottle return</returns>
        public decimal GetTotalBottleReturn(int tillNumber, string whereClause, DataSource dataSource)
        {
            DataTable rs;
            rs = string.IsNullOrEmpty(whereClause) ? GetRecords("SELECT   SUM(SaleHead.SALE_AMT) AS [sumAmount]  FROM     SaleHead  WHERE    SaleHead.T_Type = \'BTL RTN\' " + " AND     SaleHead.TILL=" + tillNumber, dataSource) : GetRecords("SELECT   SUM(SaleHead.SALE_AMT) AS [sumAmount]  FROM     SaleHead  WHERE    SaleHead.T_Type = \'BTL RTN\' " + " AND   " + whereClause, dataSource);

            if (rs.Rows.Count != 0)
            {
                return CommonUtility.GetDecimalValue(rs.Rows[0]["sumAmount"]);
            }
            return 0;
        }

        /// <summary>
        /// Method to get bonus cash
        /// </summary>
        /// <param name="cbt">Cash bonus tender</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        public decimal GetBonusCash(string cbt, int tillNumber, DataSource dataSource)
        {
            var rs = GetRecords("SELECT sum(amttend) as [SumCash] " + " FROM saletend INNER JOIN SaleHead ON "
                + "  SaleTend.Sale_No = SaleHead.Sale_No  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') " + "and tendname = \'" + cbt + "\' and Till=" + tillNumber, dataSource);

            if (rs.Rows.Count != 0)
            {
                return CommonUtility.GetDecimalValue(rs.Rows[0]["sumcash"]);
            }
            return 0;
        }

        /// <summary>
        /// Method to get all payout reasons
        /// </summary>
        /// <returns></returns>
        public List<Return_Reason> GetPayoutReasons()
        {
            var reasons = new List<Return_Reason>();
            var rspayout = GetRecords("select * from reasons where type = \'P\' order by code", DataSource.CSCMaster);
            foreach (DataRow payout in rspayout.Rows)
            {
                reasons.Add(new Return_Reason
                {
                    Description = CommonUtility.GetStringValue(payout["reason"]),
                    Reason = CommonUtility.GetStringValue(payout["code"])
                });
            }
            return reasons;
        }

        /// <summary>
        /// Method to get payout sale head
        /// </summary>
        /// <param name="code">Reason code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale head</returns>
        public SaleHead GetPayoutSaleHead(string code, int tillNumber, string whereClause, DataSource dataSource)
        {
            var saleHead = new SaleHead();
            DataTable rs;
            rs = string.IsNullOrEmpty(whereClause) ? GetRecords("SELECT   count(salehead.payout) as [Cnt] ,SUM(SaleHead.PayOut) " + " AS [sumAmount]  FROM     SaleHead  WHERE    SaleHead.T_Type = \'PAYOUT\' and reason_type = \'P\' " + " and reason = \'" + code + "\'" + " and Till=" + tillNumber, dataSource) : GetRecords("SELECT   count(salehead.payout) as [Cnt] ,SUM(SaleHead.PayOut) " + " AS [sumAmount]  FROM     SaleHead  WHERE    SaleHead.T_Type = \'PAYOUT\' and reason_type = \'P\' " + " and reason = \'" + code + "\'" + " and " + whereClause, dataSource);

            if (rs.Rows.Count != 0)
            {
                saleHead.SaleAmount = CommonUtility.GetDecimalValue(rs.Rows[0]["sumAmount"]);
                saleHead.CloseNum = CommonUtility.GetIntergerValue(rs.Rows[0]["cnt"]);

            }
            else
            {
                saleHead.SaleAmount = 0;
                saleHead.CloseNum = 0;
            }

            return saleHead;
        }

        /// <summary>
        /// Method to get tender description by tender code
        /// </summary>
        /// <param name="code">payout code</param>
        /// <returns></returns>
        public List<string> GetTenderDescriptions(string code)
        {
            var tenders = new List<string>();
            var rslink = GetRecords("select tenddesc from tendmast where " + " payoutcode = \'" + code + "\'", DataSource.CSCMaster);
            foreach (DataRow link in rslink.Rows)
            {
                tenders.Add(CommonUtility.GetStringValue(link["tenddesc"]));
            }
            return tenders;
        }

        /// <summary>
        /// Method to get payout sale tend
        /// </summary>
        /// <param name="tenderName">Tender name</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale tend</returns>
        public SaleTend GetPayoutSaleTend(string tenderName, int tillNumber, string whereClause, DataSource dataSource)
        {
            var saleTend = new SaleTend();
            DataTable rs;
            rs = string.IsNullOrEmpty(whereClause) ? GetRecords("select count(amttend) as [CntPayout], sum(amttend) AS [sumAmount]  FROM     Saletend  where  tendname = \'" + tenderName + "\'" + "  AND   TILL_NUM=" + tillNumber, dataSource) : GetRecords("select count(amttend) as [CntPayout], sum(amttend) AS [sumAmount]  FROM     Saletend  where  tendname = \'" + tenderName + "\'" + "  AND " + whereClause, dataSource);
            if (rs.Rows.Count != 0)
            {
                saleTend.AmountTend = CommonUtility.GetDecimalValue(rs.Rows[0]["CntPayout"]);
                saleTend.AmountUsed = CommonUtility.GetIntergerValue(rs.Rows[0]["sumAmount"]);
            }
            else
            {
                saleTend.AmountTend = 0;
                saleTend.AmountUsed = 0;
            }
            return saleTend;
        }

        /// <summary>
        /// Method to get deleted lines
        /// </summary>
        /// <param name="shiftDate">Shift date</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of sale lines</returns>
        public List<Sale_Line> GetDeletedLines(DateTime shiftDate, DataSource dataSource)
        {
            var saleLines = new List<Sale_Line>();
            var rs = GetRecords("Select DelLines.Del_Action as [Action], " + "       Count(*) as [Transactions]  FROM   DelLines  WHERE  Del_Date =\'" + shiftDate.ToString("yyyyMMdd") + "\' " + "Group By DelLines.Del_Action ", dataSource);

            foreach (DataRow row in rs.Rows)
            {
                saleLines.Add(new Sale_Line
                {
                    LINE_TYPE = CommonUtility.GetStringValue(row["Action"]),
                    AvailItems = CommonUtility.GetDoubleValue(row["Transactions"])
                });
            }

            return saleLines;
        }

        /// <summary>
        /// Method to get deleted lines
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of sale lines</returns>
        public List<Sale_Line> GetDeletedLinesForTill(int tillNumber, DataSource dataSource)
        {
            var saleLines = new List<Sale_Line>();
            var rs = GetRecords("Select DelLines.Del_Action as [Action], Count(*) as [Transactions]  FROM   DelLines where TILL_NUM=" + tillNumber + " " + "Group By DelLines.Del_Action ", dataSource);
            foreach (DataRow row in rs.Rows)
            {
                saleLines.Add(new Sale_Line
                {
                    LINE_TYPE = CommonUtility.GetStringValue(row["Action"]),
                    AvailItems = CommonUtility.GetDoubleValue(row["Transactions"])
                });
            }

            return saleLines;
        }

        /// <summary>
        /// Method to save till close data
        /// </summary>
        /// <param name="tillClose">Till close</param>
        /// <param name="till">Till</param>
        /// <param name="storeCode">Store code</param>
        /// <param name="boolShiftRec">All shift or not</param>
        /// <param name="taxExempt">Tax exempt or not</param>
        /// <param name="teType">Tax exempt type</param>
        /// <param name="dbTransRec">Trans transcation records</param>
        /// <param name="dbTillRec">Till transaction records</param>
        /// <param name="errorNumber">Error number</param>
        /// <param name="errorDescription">Error description</param>
        /// <param name="lastTable">Last table</param>
        /// <param name="lastSaleNo">Last sale number</param>
        public void Save(ref Till_Close tillClose, Till till, string storeCode, bool boolShiftRec,
            bool taxExempt, string teType, out int dbTransRec, out int dbTillRec, out int errorNumber,
            out string errorDescription, out string lastTable, out double lastSaleNo)
        {
            short n;
            lastSaleNo = 0;
            dbTransRec = 0;
            dbTillRec = 0;
            errorNumber = 0;
            errorDescription = "";
            lastTable = "";

            SqlConnection dbTill = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            SqlConnection dbTrans = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
            dbTill.Open();
            dbTrans.Open();
            var tillTransaction = dbTill.BeginTransaction();
            var transTransaction = dbTrans.BeginTransaction();


            n = 0;
            try
            {
                lastTable = "Close_Tenders";
                var dataTable = new DataTable();
                var query = "select * from Close_Tenders";
                SqlCommand cmd = new SqlCommand(query, dbTrans) { Transaction = transTransaction };
                var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dataTable);
                DataRow fields = null;
                SqlCommandBuilder builder;
                foreach (Close_Line tempLoopVarCl in tillClose)
                {
                    var cl = tempLoopVarCl;
                    fields = dataTable.NewRow();
                    n++;
                    fields["Close_Num"] = tillClose.Close_Num;
                    fields["Sequence"] = n;
                    fields["TendName"] = cl.Tender_Name;
                    fields["TENDCLASS"] = cl.Tender_Class;
                    fields["TendCount"] = cl.Tender_Count;
                    fields["Amount"] = cl.Entered;
                    fields["System"] = cl.System;
                    fields["Balance"] = cl.Balance;
                    fields["Conv_Sys"] = cl.System * (decimal)cl.Exchange_Rate;
                    fields["Conv_Amt"] = cl.Entered * (decimal)cl.Exchange_Rate;
                    fields["Conv_Bal"] = cl.Balance * (decimal)cl.Exchange_Rate;
                    dataTable.Rows.Add(fields);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.InsertCommand = builder.GetInsertCommand();
                    adapter.Update(dataTable);
                }

                lastTable = "Close_Head";
                dataTable = new DataTable();
                query = "select* from Close_Head where close_num = " + Convert.ToString(tillClose.Close_Num);
                cmd = new SqlCommand(query, dbTrans) { Transaction = transTransaction };
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dataTable);
                builder = new SqlCommandBuilder(adapter);
                var isNew = false;
                if (dataTable.Rows.Count == 0)
                {
                    adapter.InsertCommand = builder.GetInsertCommand();
                    fields = dataTable.NewRow();
                    fields["Close_Num"] = tillClose.Close_Num;
                    isNew = true;
                }
                else
                {
                    adapter.UpdateCommand = builder.GetUpdateCommand();
                    fields = dataTable.Rows[0];
                }

                fields["Close_Date"] = tillClose.Close_Date;
                fields["Close_Time"] = tillClose.Close_Time;
                fields["Open_Date"] = tillClose.Open_Date;
                fields["ShiftDate"] = tillClose.ShiftDate;
                fields["Open_Time"] = tillClose.Open_Time;
                fields["close_num"] = tillClose.Close_Num;
                fields["Till_Date"] = tillClose.Close_Date;
                fields["Till_Num"] = tillClose.Till_Number;
                fields["ShiftNumber"] = tillClose.ShiftNumber;
                fields["User"] = tillClose.User;
                fields["Float"] = tillClose.Float;
                fields["Drop"] = tillClose.Drop;
                fields["Draw"] = tillClose.Draw;
                fields["SC_Issued"] = tillClose.Credits_Issued;
                fields["BonusFloat"] = tillClose.BonusFloat;
                fields["BonusDraw"] = tillClose.BonusDraw;
                fields["BonusDrop"] = tillClose.BonusDrop;
                fields["BonusGiveAway"] = tillClose.BonusGiveAway;
                fields["GroupNumber"] = tillClose.GroupNumber;
                fields["GroupType"] = tillClose.GroupType;
                fields["Dip_Number"] = tillClose.Dip_Number;
                if (isNew)
                {
                    dataTable.Rows.Add(fields);
                }
                adapter.Update(dataTable);

                // Copy the till tables to production.
                string[] table = new string[32];

                table[1] = "SaleHead";
                table[2] = "SaleLine";
                table[3] = "SaleTend";
                table[4] = "S_LineTax";
                table[5] = "S_SaleTax";
                table[6] = "SaleKit";
                table[7] = "SaleChg";
                table[8] = "DelLines";
                table[9] = "ChargeTax";
                table[10] = "CardTenders";
                table[11] = "SaleKitChg";
                table[12] = "SaleKitChgTax";
                table[13] = "CashDraw";
                table[14] = "DropHeader";
                table[15] = "DropLines";
                table[16] = "SLineReason";
                table[17] = "VoidSale";
                table[18] = "SaleVendors";
                table[19] = "DebitSwipe";
                table[20] = "DiscountTender";
                table[21] = "CardSales";
                table[22] = "SaleVendorCoupon";
                table[23] = "GCTenders";
                table[24] = "Accumulation";
                table[25] = "TaxExemptSaleHead";
                table[26] = "TaxExemptSaleLine";
                table[27] = "PurchaseItem";
                table[28] = "TaxCredit";
                table[29] = "TaxCreditLine";
                table[30] = "Signature";
                table[31] = "CardProfilePrompts";

                Till_Close.FieldAndTable[] autoIncrementFields;
                var hasAutoIncreFieldInTill = GetAutoIncrementFields(out autoIncrementFields);

                for (n = 1; n <= (table.Length - 1); n++)
                {
                    if ((table[n] != "PurchaseItem" && table[n] != "TaxExemptSaleHead"
                        && table[n] != "TaxExemptSaleLine") || (taxExempt && teType == "SITE"
                        && table[n] == "PurchaseItem") || (taxExempt && teType != "SITE" && (table[n] == "TaxExemptSaleHead" || table[n] == "TaxExemptSaleLine")))
                    {
                        lastTable = table[n];
                        DataTable rsTill;
                        if (table[n] == "SaleHead" || table[n] == "CashDraw")
                        {
                            rsTill = GetRecords("select * from " + table[n] + " where TILL=" + till.Number, DataSource.CSCTills);
                        }
                        else if (table[n] == "PurchaseItem")
                        {
                            rsTill = GetRecords("select * from " + table[n] + " where Till_No=" + till.Number, DataSource.CSCTills);
                        }
                        else
                        {
                            rsTill = GetRecords("select * from " + table[n] + " where TILL_NUM=" + till.Number, DataSource.CSCTills);
                        }
                        dataTable = new DataTable();
                        query = "select * from " + table[n];
                        cmd = new SqlCommand(query, dbTrans) { Transaction = transTransaction };
                        adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dataTable);

                        foreach (DataRow row in rsTill.Rows)
                        {
                            var field = dataTable.NewRow();

                            foreach (DataColumn tempLoopVarFld in rsTill.Columns)
                            {
                                var fld = tempLoopVarFld;
                                bool autoIncrField;
                                if (hasAutoIncreFieldInTill)
                                {
                                    autoIncrField = IsAutoIncrementField(autoIncrementFields, table[n].Trim(), fld.ColumnName.Trim());
                                }
                                else
                                {
                                    autoIncrField = false;
                                }


                                if (!autoIncrField)
                                {
                                    if (Strings.UCase(fld.ColumnName) == "CLOSE_NUM")
                                    {
                                        field[fld.ColumnName] = tillClose.Close_Num;
                                    }
                                    else
                                    {
                                        field[fld.ColumnName] = row[fld.ColumnName];
                                    }
                                    if (fld.ColumnName.ToUpper() == "SALE_NO")
                                    {
                                        lastSaleNo = Convert.ToDouble(row[fld.ColumnName]);
                                    }
                                }

                                if (fld.ColumnName.ToUpper() == "STORE")
                                {
                                    field[fld.ColumnName] = storeCode;
                                }
                            }
                            dataTable.Rows.Add(field);
                        }
                        builder = new SqlCommandBuilder(adapter);
                        adapter.InsertCommand = builder.GetInsertCommand();
                        adapter.Update(dataTable);
                        // Empty the till-level table after you've copied it to the permanent database.
                        if (table[n] == "SaleHead" || table[n] == "CashDraw")
                        {
                            var sqlCmd = new SqlCommand("Delete  From " + table[n] + " where TILL=" + till.Number, dbTill, tillTransaction);
                            var recordsAffected = sqlCmd.ExecuteNonQuery();
                        }
                        else if (table[n] == "PurchaseItem")
                        {
                            var sqlCmd = new SqlCommand("Delete  From " + table[n] + " where Till_No=" + till.Number, dbTill, tillTransaction);
                            var recordsAffected = sqlCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            var sqlCmd = new SqlCommand("Delete  From " + table[n] + " where TILL_NUM=" + till.Number, dbTill, tillTransaction);
                            var recordsAffected = sqlCmd.ExecuteNonQuery();
                        }

                    }
                }

                new SqlCommand("Delete from EziReceipt Where till=" + till.Number, dbTill, tillTransaction).ExecuteNonQuery();
                if (boolShiftRec && tillClose.Till_Number == 1)
                {

                    var sqlCmd = new SqlCommand("UPDATE SaleHead SET MainTill_CloseNum=" + Convert.ToString(tillClose.Close_Num) + " WHERE MainTill_CloseNum IS NULL OR MainTill_CloseNum=0", dbTrans, transTransaction);
                    dbTransRec = sqlCmd.ExecuteNonQuery();
                    sqlCmd = new SqlCommand("UPDATE SaleHead SET MainTill_CloseNum=" + Convert.ToString(tillClose.Close_Num) + " WHERE MainTill_CloseNum IS NULL OR MainTill_CloseNum=0", dbTill, tillTransaction);
                    dbTillRec = sqlCmd.ExecuteNonQuery();
                    lastTable = "Tills";

                }
                tillTransaction.Commit();
                transTransaction.Commit();
                tillClose.Complete = true;
                adapter?.Dispose();
                dbTill.Close();
                dbTrans.Close();
            }

            catch (Exception ex)
            {
                errorNumber = ex.HResult;
                errorDescription = ex.Message;
                tillTransaction.Rollback();
                transTransaction.Rollback();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="autoIncrementTables"></param>
        /// <returns></returns>
        private bool GetAutoIncrementFields(out Till_Close.FieldAndTable[] autoIncrementTables)
        {
            short i = 0;
            autoIncrementTables = null;
            var rs = GetRecords(" SELECT " + " OBJECT_NAME(id) as TableName, " + " Name as FieldName FROM syscolumns " + " WHERE (status & 128) = 128", DataSource.CSCTills);

            if (rs.Rows.Count == 0)
            {
                return false;
            }

            autoIncrementTables = new Till_Close.FieldAndTable[rs.Rows.Count + 1];

            i = 1;
            foreach (DataRow row in rs.Rows)
            {
                autoIncrementTables[i].FieldName = CommonUtility.GetStringValue(row["FieldName"]);
                autoIncrementTables[i].TableName = CommonUtility.GetStringValue(row["TableName"]);
                i++;
            }

            return true;
        }

        /// <summary>
        /// Method to check whether auto increment field or not
        /// </summary>
        /// <param name="autoIncrementTables">Auto increment tables</param>
        /// <param name="tableName">Table name</param>
        /// <param name="fieldName">Field name</param>
        /// <returns>True or false</returns>
        private bool IsAutoIncrementField(Till_Close.FieldAndTable[] autoIncrementTables, string tableName, string fieldName)
        {
            bool returnValue = false;
            short i = 0;

            if (autoIncrementTables.Length - 1 < 1)
            {
                return false;
            }

            for (i = 1; i <= autoIncrementTables.Length - 1; i++)
            {
                if (tableName.ToUpper() == Strings.UCase(autoIncrementTables[i].TableName))
                {
                    if (fieldName.ToUpper() == Strings.UCase(autoIncrementTables[i].FieldName))
                    {
                        returnValue = true;
                    }
                }
            }

            return returnValue;
        }

        //Behrooz Jan-12-06
        /// <summary>
        /// Method to save trainer till
        /// </summary>
        /// <param name="tillClose">Till close</param>
        /// <param name="till">Till</param>
        public void SaveTrainerTill(ref Till_Close tillClose, Till till)
        {

            short n;
            Close_Line cl;
            string sSql = "";
            SqlConnection dbTill = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            dbTill.Open();
            var tillTransaction = dbTill.BeginTransaction();

            string[] table = new string[33];
            table[1] = "SaleHead";
            table[2] = "SaleLine";
            table[3] = "SaleTend";
            table[4] = "S_LineTax";
            table[5] = "S_SaleTax";
            table[6] = "SaleKit";
            table[7] = "SaleChg";
            table[8] = "DelLines";
            table[9] = "ChargeTax";
            table[10] = "CardTenders";
            table[11] = "SaleKitChg";
            table[12] = "SaleKitChgTax";
            table[13] = "CashDraw";
            table[14] = "DropHeader";
            table[15] = "DropLines";
            table[16] = "SLineReason";
            table[17] = "VoidSale";
            table[18] = "SaleVendors";
            table[19] = "PurchaseItem";
            table[20] = "DiscountTender";
            table[21] = "CardSales";
            table[22] = "SaleVendorCoupon";
            table[23] = "GCTenders";
            table[24] = "Accumulation";
            table[25] = "TaxExemptSaleHead";
            table[26] = "TaxExemptSaleLine";

            table[27] = "DebitSwipe";
            table[28] = "TaxCredit";
            table[29] = "TaxCreditLine";
            table[30] = "TidelTrans";
            table[31] = "Signature";
            table[32] = "CardProfilePrompts";
            try
            {
                Till_Close.FieldAndTable[] autoIncrementFields;
                bool hasAutoIncreFieldInTill;
                hasAutoIncreFieldInTill = GetAutoIncrementFields(out autoIncrementFields);
                for (n = 0; n <= (table.Length - 1); n++)
                {
                    if (table[n] == "SaleHead" || table[n] == "CashDraw")
                    {
                        new SqlCommand("Delete  From " + table[n] + " where TILL=" + Convert.ToString(till.Number), dbTill, tillTransaction).ExecuteNonQuery();
                    }
                    else if (table[n] == "PurchaseItem")
                    {
                        new SqlCommand("Delete  From " + table[n] + " where Till_No=" + Convert.ToString(till.Number), dbTill, tillTransaction).ExecuteNonQuery();
                    }
                    else
                    {
                        new SqlCommand("Delete  From " + table[n] + " where TILL_NUM=" + Convert.ToString(till.Number), dbTill, tillTransaction).ExecuteNonQuery();
                    }
                }


                sSql = "UPDATE Tills " + "SET Active=0, Process=0, posID=0" + " WHERE Till_Num=" + Convert.ToString(tillClose.Till_Number) + "";

                Execute(sSql, DataSource.CSCMaster);

                tillClose.Complete = true;
                tillTransaction.Commit();
                tillClose.Complete = true;
                dbTill.Close();
            }
            catch
            {
                tillTransaction.Rollback();
            }
        }

        /// <summary>
        /// Method to get departments
        /// </summary>
        /// <returns>List of departments</returns>
        public List<Dept> GetDepartment()
        {
            var dept = new List<Dept>();
            var rsDept = GetRecords("SELECT * FROM Dept", DataSource.CSCMaster);
            foreach (DataRow row in rsDept.Rows)
            {
                dept.Add(new Dept
                {
                    DeptCode = CommonUtility.GetStringValue(row["DEPT"]),
                    DeptName = CommonUtility.GetStringValue(row["DEPT_NAME"]),
                    EODDetail = CommonUtility.GetIntergerValue(row["EOD_Detail"]),
                    EODGroup = CommonUtility.GetIntergerValue(row["EodGroup"]),
                    CountDetail = CommonUtility.GetIntergerValue(row["Cnt_Detail"])
                });
            }
            return dept;
        }

        /// <summary>
        /// Method to get fuel department id
        /// </summary>
        /// <returns>Fuel department</returns>
        public string GetFuelDepartmentId()
        {
            string fuelDept = null;
            var recFuelDept = GetRecords("select * from FuelDept", DataSource.CSCMaster);
            if (recFuelDept.Rows.Count > 0)
            {
                fuelDept = CommonUtility.GetStringValue(recFuelDept.Rows[0]["Dept"]);
            }

            return fuelDept;
        }

        /// <summary>
        /// Method to get sale line by dept
        /// </summary>
        /// <param name="dept">Dept</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale line</returns>
        public Sale_Line GetSaleLineByDept(string dept, int tillNumber, string whereClause, DataSource dataSource)
        {
            var rsSales = string.IsNullOrEmpty(whereClause) ? GetRecords("SELECT SaleLine.Dept, D.Dept_Name, "
                + "      Sum(SaleLine.Quantity) as [Volume], Sum(SaleLine.Amount-SaleLine.Discount-SaleLine.Disc_Adj) as [Sales]  FROM   (SaleLine INNER JOIN SaleHead ON  " + "        SaleLine.Sale_No = SaleHead.Sale_No) INNER JOIN CSCMaster.dbo.Dept as [D] ON " + "        D.Dept = SaleLine.Dept  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + "       SaleLine.Dept = \'" + dept + "\' AND " + "       SaleLine.Till_Num=" + tillNumber + " Group By SaleLine.Dept, D.Dept_Name " + "Order by SaleLine.Dept ", dataSource) : GetRecords("SELECT SaleLine.Dept, D.Dept_Name, " + "       Sum(SaleLine.Quantity) as [Volume],Sum(SaleLine.Amount-SaleLine.Discount-SaleLine.Disc_Adj) as [Sales]  FROM   (SaleLine INNER JOIN SaleHead ON  " + "        SaleLine.Sale_No = SaleHead.Sale_No) INNER JOIN CSCMaster.dbo.Dept as [D] ON " + "        D.Dept = SaleLine.Dept  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + whereClause + " AND " + "       SaleLine.Dept = \'" + dept + "\' " + "Group By SaleLine.Dept, D.Dept_Name " + "Order by SaleLine.Dept ", dataSource);
            if (rsSales.Rows.Count == 0)
                return null;
            return new Sale_Line
            {
                Dept = CommonUtility.GetStringValue(rsSales.Rows[0]["Dept"]),
                Description = CommonUtility.GetStringValue(rsSales.Rows[0]["Dept_Name"]),
                Quantity = CommonUtility.GetFloatValue(rsSales.Rows[0]["Volume"]),
                Amount = CommonUtility.GetDecimalValue(rsSales.Rows[0]["Sales"])
            };
        }

        /// <summary>
        /// Method to get sale lines by sub dept
        /// </summary>
        /// <param name="dept">Dept</param>
        /// <param name="tillNumber"></param>
        /// <param name="whereClause"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        public List<Sale_Line> GetSaleLinesBySubDept(string dept, int tillNumber, string whereClause, DataSource dataSource)
        {
            var saleLines = new List<Sale_Line>();
            var rsSales = string.IsNullOrEmpty(whereClause) ? GetRecords("SELECT SaleLine.Sub_Dept, D.Sub_Name, " + "       Sum(SaleLine.Quantity) as [Volume],Sum(SaleLine.Amount-SaleLine.Discount-SaleLine.Disc_Adj) as [Sales]  FROM   (SaleLine INNER JOIN SaleHead ON  " + "        SaleLine.Sale_No = SaleHead.Sale_No) LEFT JOIN CSCMaster.dbo.SubDept as [D] ON " + "        D.Dept = SaleLine.Dept AND " + "        D.Sub_Dept = SaleLine.Sub_Dept  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + "       SaleLine.Dept = \'" + dept + "\' AND " + "       SaleLine.Till_Num=" + tillNumber + " Group By SaleLine.Sub_Dept, D.Sub_Name " + "Order by D.Sub_Name ", dataSource) : GetRecords("SELECT SaleLine.Sub_Dept, D.Sub_Name, " + "       Sum(SaleLine.Quantity) as [Volume],Sum(SaleLine.Amount-SaleLine.Discount-SaleLine.Disc_Adj) as [Sales]  FROM   (SaleLine INNER JOIN SaleHead ON  " + "        SaleLine.Sale_No = SaleHead.Sale_No) LEFT JOIN CSCMaster.dbo.SubDept as [D] ON " + "        D.Dept = SaleLine.Dept AND " + "        D.Sub_Dept = SaleLine.Sub_Dept  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + whereClause + " AND " + "       SaleLine.Dept = \'" + dept + "\' " + " Group By SaleLine.Sub_Dept, D.Sub_Name " + "Order by D.Sub_Name ", dataSource);
            foreach (DataRow saleLine in rsSales.Rows)
            {
                saleLines.Add(new Sale_Line
                {
                    Sub_Dept = CommonUtility.GetStringValue(saleLine["Sub_Dept"]),
                    Description = CommonUtility.GetStringValue(saleLine["Sub_Name"]),
                    Quantity = CommonUtility.GetFloatValue(saleLine["Volume"]),
                    Amount = CommonUtility.GetDecimalValue(saleLine["Sales"])
                });
            }
            return saleLines;
        }

        /// <summary>
        /// Method to get sale lines by sub detail
        /// </summary>
        /// <param name="dept">Dept</param>
        /// <param name="subDept">Sub dept</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale lines</returns>
        public List<Sale_Line> GetSaleLinesBySubDetail(string dept, string subDept, int tillNumber,
            string whereClause, DataSource dataSource)
        {
            var saleLines = new List<Sale_Line>();
            var rsSales = string.IsNullOrEmpty(whereClause) ? GetRecords("SELECT SaleLine.Sub_Detail, "
                + "       D.SubDetail_Name, " + "       Sum(SaleLine.Quantity) as [Volume],Sum(SaleLine.Amount-SaleLine.Discount-SaleLine.Disc_Adj) as [Sales]  FROM   (SaleLine INNER JOIN SaleHead ON  " + "        SaleLine.Sale_No = SaleHead.Sale_No) LEFT JOIN CSCMaster.dbo.SubDetail as [D] ON " + "        D.Dept = SaleLine.Dept AND " + "        D.SubDept = SaleLine.Sub_Dept AND " + "        D.SubDetail = SaleLine.Sub_Detail  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + "       SaleLine.Dept = \'" + dept + "\' AND " + "       SaleLine.Sub_Dept = \'" + subDept + "\' AND " + "       SaleLine.Till_Num=" + tillNumber + " Group By SaleLine.Sub_Detail, D.SubDetail_Name " + "Order by D.SubDetail_Name ", dataSource) : GetRecords("SELECT SaleLine.Sub_Detail, " + "       D.SubDetail_Name, " + "      Sum(SaleLine.Quantity) as [Volume], Sum(SaleLine.Amount-SaleLine.Discount-SaleLine.Disc_Adj) as [Sales]  FROM   (SaleLine INNER JOIN SaleHead ON  " + "        SaleLine.Sale_No = SaleHead.Sale_No) LEFT JOIN CSCMaster.dbo.SubDetail as [D] ON " + "        D.Dept = SaleLine.Dept AND " + "        D.SubDept = SaleLine.Sub_Dept AND " + "        D.SubDetail = SaleLine.Sub_Detail  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + whereClause + " AND " + "       SaleLine.Dept = \'" + dept + "\' AND " + "       SaleLine.Sub_Dept = \'" + subDept + "\' " + " Group By SaleLine.Sub_Detail, D.SubDetail_Name " + "Order by D.SubDetail_Name ", dataSource);
            foreach (DataRow saleLine in rsSales.Rows)
            {
                saleLines.Add(new Sale_Line
                {
                    Sub_Detail = CommonUtility.GetStringValue(saleLine["Sub_Detail"]),
                    Description = CommonUtility.GetStringValue(saleLine["SubDetail_Name"]),
                    Quantity = CommonUtility.GetFloatValue(saleLine["Volume"]),
                    Amount = CommonUtility.GetDecimalValue(saleLine["Sales"])
                });
            }
            return saleLines;
        }

        /// <summary>
        /// Method to get sale lines by stock code
        /// </summary>
        /// <param name="dept">Dept</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale lines</returns>
        public List<Sale_Line> GetSaleLinesByStockCode(string dept, int tillNumber, string whereClause,
            DataSource dataSource)
        {
            var saleLines = new List<Sale_Line>();
            var rsSales = string.IsNullOrEmpty(whereClause) ? GetRecords("SELECT SaleLine.Stock_Code, SaleLine.Descript, " + "       SaleLine.Dept, D.Dept_Name, " + "       Sum(SaleLine.Quantity) as [Volume],Sum(SaleLine.Amount-SaleLine.Discount-SaleLine.Disc_Adj) as [Sales]  FROM   (SaleLine INNER JOIN SaleHead ON  " + "        SaleLine.Sale_No = SaleHead.Sale_No) INNER JOIN CSCMaster.dbo.Dept as [D] ON " + "        D.Dept = SaleLine.Dept  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + "       SaleLine.Dept = \'" + dept + "\' AND " + "       SaleLine.Till_Num=" + tillNumber + " Group By SaleLine.Stock_Code, SaleLine.Descript, SaleLine.Dept, D.Dept_Name " + "Order by SaleLine.Stock_Code ", dataSource) : GetRecords("SELECT SaleLine.Stock_Code, SaleLine.Descript, " + "       SaleLine.Dept, D.Dept_Name, " + "       Sum(SaleLine.Quantity) as [Volume],Sum(SaleLine.Amount-SaleLine.Discount-SaleLine.Disc_Adj) as [Sales]  FROM   (SaleLine INNER JOIN SaleHead ON  " + "        SaleLine.Sale_No = SaleHead.Sale_No) INNER JOIN CSCMaster.dbo.Dept as [D] ON " + "        D.Dept = SaleLine.Dept  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + whereClause + " AND " + "       SaleLine.Dept = \'" + dept + "\' " + " Group By SaleLine.Stock_Code, SaleLine.Descript, SaleLine.Dept, D.Dept_Name " + "Order by SaleLine.Stock_Code ", dataSource);
            foreach (DataRow saleLine in rsSales.Rows)
            {
                saleLines.Add(new Sale_Line
                {
                    Stock_Code = CommonUtility.GetStringValue(saleLine["Stock_Code"]),
                    Description = CommonUtility.GetStringValue(saleLine["Descript"]),
                    Dept = CommonUtility.GetStringValue(saleLine["Dept"]),
                    Sub_Dept = CommonUtility.GetStringValue(saleLine["Dept_Name"]),
                    Quantity = CommonUtility.GetFloatValue(saleLine["Volume"]),
                    Amount = CommonUtility.GetDecimalValue(saleLine["Sales"])
                });
            }
            return saleLines;
        }

        /// <summary>
        /// Method to get sale lines
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale lines</returns>
        public List<Sale_Line> GetSaleLines(int tillNumber, string whereClause, DataSource dataSource)
        {
            var saleLines = new List<Sale_Line>();
            var rsSales = string.IsNullOrEmpty(whereClause) ? GetRecords("SELECT SaleLine.Dept, "
                + "       Sum(SaleLine.Quantity) as [Volume],Sum(SaleLine.Amount-SaleLine.Discount-SaleLine.Disc_Adj-SaleLine.TE_Amount_Incl) as [Sales]  FROM   (SaleLine INNER JOIN SaleHead ON  " + "        SaleLine.Sale_No = SaleHead.Sale_No) LEFT JOIN CSCMaster.dbo.Dept as [D] ON " + "        D.Dept = SaleLine.Dept  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + "       D.Dept IS NULL AND " + "       SaleLine.Till_Num=" + tillNumber + " Group by SaleLine.Dept ", dataSource) : GetRecords("SELECT SaleLine.Dept, " + "       Sum(SaleLine.Quantity) as [Volume],Sum(SaleLine.Amount-SaleLine.Discount-SaleLine.Disc_Adj-SaleLine.TE_Amount_Incl) as [Sales]  FROM   (SaleLine INNER JOIN SaleHead ON  " + "        SaleLine.Sale_No = SaleHead.Sale_No) LEFT JOIN CSCMaster.dbo.Dept as [D] ON " + "        D.Dept = SaleLine.Dept  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + whereClause + " AND " + "       D.Dept IS NULL " + " Group by SaleLine.Dept ", dataSource);

            foreach (DataRow saleLine in rsSales.Rows)
            {
                saleLines.Add(new Sale_Line
                {
                    Dept = CommonUtility.GetStringValue(saleLine["Dept"]),
                    Quantity = CommonUtility.GetFloatValue(saleLine["Volume"]),
                    Amount = CommonUtility.GetDecimalValue(saleLine["Sales"])
                });
            }

            return saleLines;
        }

        /// <summary>
        /// Method to get sub dept by dept
        /// </summary>
        /// <param name="dept">Dept</param>
        /// <returns>List of sub dept</returns>
        public List<SubDept> GetSubDeptByDept(string dept)
        {
            var subDepts = new List<SubDept>();
            var rsSubDept = GetRecords("Select SubDept.Sub_Dept, SubDept.Sub_Name  FROM   SubDept  WHERE  SubDept.Dept = \'" + dept + "\' ", DataSource.CSCMaster);
            foreach (DataRow subDept in rsSubDept.Rows)
            {
                subDepts.Add(new SubDept
                {
                    Sub_Dept = CommonUtility.GetStringValue(subDept["Sub_Dept"]),
                    Sub_Name = CommonUtility.GetStringValue(subDept["Sub_Name"])
                });
            }
            return subDepts;
        }

        /// <summary>
        /// Method to get categories
        /// </summary>
        /// <returns>Categories</returns>
        public Dictionary<int, string> GetCategories()
        {
            var categories = new Dictionary<int, string>();
            var rsTeCat = GetRecords("SELECT * FROM Category ORDER BY ID", DataSource.CSCMaster);
            foreach (DataRow row in rsTeCat.Rows)
            {
                categories.Add(CommonUtility.GetIntergerValue(row["ID"]), CommonUtility.GetStringValue(row["Name"]));
            }
            return categories;
        }

        /// <summary>
        /// Method to get tax exempt sale line
        /// </summary>
        /// <param name="whereClause">Wher clause</param>
        /// <param name="categoryId">Category Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Tax exempt sale line</returns>
        public TaxExemptSaleLine GetTaxExemptSaleLine(string whereClause, int categoryId, int tillNumber)
        {
            var rsTeLine = !string.IsNullOrEmpty(whereClause) ? GetRecords("Select Sum(Quantity) as [sQuantity], " + "Sum(Amount) as [sAmount], " + "Sum(ExemptedTax) as [sTotalTaxSaved]  FROM  TaxExemptSaleLine INNER JOIN SaleHead ON " + "       TaxExemptSaleLine.Sale_No = SaleHead.Sale_No  WHERE ProductType= " + Convert.ToString(categoryId) + " AND " + whereClause + " AND TILL_NUM=" + tillNumber + " Group By ProductType", DataSource.CSCTrans) : GetRecords("Select Sum(Quantity) as [sQuantity], " + "Sum(Amount) as [sAmount], " + "Sum(ExemptedTax) as [sTotalTaxSaved]  FROM  TaxExemptSaleLine  WHERE ProductType= " + Convert.ToString(categoryId) + " AND TILL_NUM=" + tillNumber + " Group By ProductType", DataSource.CSCTills);

            if (rsTeLine.Rows.Count == 0)
            {
                return null;
            }

            return new TaxExemptSaleLine
            {
                Quantity = CommonUtility.GetFloatValue(rsTeLine.Rows[0]["sQuantity"]),
                Amount = CommonUtility.GetFloatValue(rsTeLine.Rows[0]["sAmount"]),
                ExemptedTax = CommonUtility.GetFloatValue(rsTeLine.Rows[0]["sTotalTaxSaved"])
            };
        }

        /// <summary>
        /// Method to get tax credits
        /// </summary>
        /// <param name="whereClause">Where clause</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of sale tax</returns>
        public List<Sale_Tax> GetTaxCredits(string whereClause, int tillNumber)
        {
            var taxCredits = new List<Sale_Tax>();
            var rsTeCredit = !string.IsNullOrEmpty(whereClause) ? GetRecords(" SELECT DISTINCT TaxCredit.Tax_Name, " + " SUM(TaxCredit.Tax_Added_Amount) AS [Added Tax], " + " SUM(TaxCredit.Tax_Included_Amount) AS [Included Tax] " + " FROM TaxCredit INNER JOIN SaleHead " + " ON SaleHead.TILL = TaxCredit.TILL_NUM AND SaleHead.SALE_NO = TaxCredit.Sale_No " + " AND " + whereClause + " AND SaleHead.TILL =" + Convert.ToString(tillNumber) + " GROUP BY TaxCredit.Tax_Name ", DataSource.CSCTrans) : GetRecords(" SELECT DISTINCT TaxCredit.Tax_Name, " + " SUM(TaxCredit.Tax_Added_Amount) AS [Added Tax], " + " SUM(TaxCredit.Tax_Included_Amount) AS [Included Tax] " + " FROM TaxCredit INNER JOIN SaleHead  " + " ON SaleHead.TILL = TaxCredit.TILL_NUM AND SaleHead.SALE_NO = TaxCredit.Sale_No " + " AND SaleHead.TILL=" + Convert.ToString(tillNumber) + " GROUP BY TaxCredit.Tax_Name ", DataSource.CSCTills);

            foreach (DataRow row in rsTeCredit.Rows)
            {
                taxCredits.Add(new Sale_Tax
                {
                    Tax_Name = CommonUtility.GetStringValue(row["Tax_Name"]),
                    Tax_Added_Amount = CommonUtility.GetDecimalValue(row["Added Tax"]),
                    Tax_Included_Amount = CommonUtility.GetDecimalValue(row["Included Tax"])
                });
            }
            return taxCredits;
        }

        /// <summary>
        /// Method to get maximum dip number
        /// </summary>
        /// <returns></returns>
        public short GetMaxDipNumber()
        {
            var rs = GetRecords("Select Max(Dip_Number) As [DN]  FROM   TankDips ", DataSource.CSCPump);
            if (rs.Rows.Count == 0)
                return -1;
            return CommonUtility.GetShortValue(rs.Rows[0]["DN"]);
        }

        /// <summary>
        /// Method to get tank dip by dip number
        /// </summary>
        /// <param name="dn">Dip number</param>
        /// <returns>List of tank dips</returns>
        public List<TankDip> GetTankDipByDipNumber(short dn)
        {
            var tankDips = new List<TankDip>();
            var rs = GetRecords("Select *  FROM   TankDips  WHERE  Dip_Number = " + Convert.ToString(dn) + " ORDER BY TankID ", DataSource.CSCPump);
            foreach (DataRow row in rs.Rows)
            {
                tankDips.Add(new TankDip
                {
                    Date = CommonUtility.GetDateTimeValue(row["ActualDateTime"]),
                    TankId = CommonUtility.GetShortValue(row["TankID"]),
                    FuelDip = CommonUtility.GetFloatValue(row["Dip_Fuel"]),
                    WaterDip = CommonUtility.GetFloatValue(row["Dip_Water"]),
                    Temperature = CommonUtility.GetFloatValue(row["TemperatureC"]),
                    Volume = CommonUtility.GetFloatValue(row["Volume"])
                });
            }

            return tankDips;
        }

        /// <summary>
        /// Method to get tank gauge set up
        /// </summary>
        /// <returns>Tank gauge set up</returns>
        public TankGaugeSetup GetTankGaugeSetUp()
        {
            var rsSetup = GetRecords("select * from TankGaugeSetup", DataSource.CSCPump);
            if (rsSetup.Rows.Count != 0)
            {
                return new TankGaugeSetup
                {
                    DipUM = CommonUtility.GetStringValue(rsSetup.Rows[0]["DipUM"]),
                    TempUM = CommonUtility.GetStringValue(rsSetup.Rows[0]["TempUM"])
                };
            }
            return null;
        }

        /// <summary>
        /// Method to get tax exempt sale
        /// </summary>
        /// <param name="firstCloseNumber">First close number</param>
        /// <param name="lastCloseNumber">Last close number</param>
        /// <returns>Maximum tax exempt sale</returns>
        public short GetMaxTaxExemptSale(int firstCloseNumber, int lastCloseNumber)
        {
            var rsCount = GetRecords("SELECT COUNT(*) as [MaxCnt] FROM TaxExemptSaleHead "
                + " WHERE TotalExemptedTax <> 0 AND " + "  Close_Num >" + Convert.ToString(firstCloseNumber)
                + " And Close_Num <= " + Convert.ToString(lastCloseNumber), DataSource.CSCTrans);
            if (rsCount.Rows.Count == 0)
            {
                return 0;
            }
            return CommonUtility.GetShortValue(rsCount.Rows[0]["MaxCnt"]);
        }

        /// <summary>
        /// Method to get maximum tax exempt sale line
        /// </summary>
        /// <param name="firstCloseNumber">First close number</param>
        /// <param name="lastCloseNumber">Last close number</param>
        /// <returns>Max tax exmept sale line</returns>
        public short GetMaxTaxExemptSaleLine(int firstCloseNumber, int lastCloseNumber)
        {
            var rsCount = GetRecords("SELECT COUNT(* ) as [MaxCnt] FROM TaxExemptSaleLine"
                + " INNER JOIN TaxExemptSaleHead ON " + " TaxExemptSaleLine.Sale_NO = TaxExemptSaleHead.Sale_No "
                + " WHERE ExemptedTax <> 0 AND " + " Close_Num >" + Convert.ToString(firstCloseNumber)
                + " And Close_Num <= " + Convert.ToString(lastCloseNumber), DataSource.CSCTrans);
            if (rsCount.Rows.Count == 0)
            {
                return 0;
            }
            return CommonUtility.GetShortValue(rsCount.Rows[0]["MaxCnt"]);
        }

        /// <summary>
        /// Method to get maximum purchase header
        /// </summary>
        /// <param name="transTime">Transaction time</param>
        /// <returns>Maximum purchase header</returns>
        public short GetMaxPurchaseHeader(DateTime transTime)
        {
            var rsCount = GetRecords("SELECT COUNT(*) as [MaxCnt] FROM TaxExemptPurchaseHeader WHERE Transdate > \'" + Convert.ToString(transTime, CultureInfo.InvariantCulture) + "\'", DataSource.CSCTrans);
            if (rsCount.Rows.Count == 0)
            {
                return 0;
            }
            return CommonUtility.GetShortValue(rsCount.Rows[0]["MaxCnt"]);
        }

        /// <summary>
        /// Method to get maximum purchase line
        /// </summary>
        /// <param name="transTime">Transaction time</param>
        /// <returns>Maximum purchase line</returns>
        public short GetMaxPurchaseLine(DateTime transTime)
        {
            var rsCount = GetRecords("SELECT COUNT(* ) as [MaxCnt] FROM TaxExemptPurchaseDetail"
                + " INNER JOIN TaxExemptPurchaseHeader ON " + " TaxExemptPurchaseDetail.PurchaseNumber = TaxExemptPurchaseHeader.PurchaseNumber "
                + " WHERE TaxExemptPurchaseHeader.Transdate > \'" + Convert.ToString(transTime, CultureInfo.InvariantCulture) + "\'", DataSource.CSCTrans);

            if (rsCount.Rows.Count == 0)
            {
                return 0;
            }
            return CommonUtility.GetShortValue(rsCount.Rows[0]["MaxCnt"]);
        }

        /// <summary>
        /// Method to get maximum tax exempt card registry
        /// </summary>
        /// <returns></returns>
        public short GetMaxTaxExemptCardRegistry()
        {
            var rsCount = GetRecords("SELECT COUNT(* )  as [MaxCnt] FROM TaxExemptCardRegistry" + " WHERE Updated = 1 ", DataSource.CSCMaster);
            if (rsCount.Rows.Count == 0)
            {
                return 0;
            }
            return CommonUtility.GetShortValue(rsCount.Rows[0]["MaxCnt"]);
        }

        /// <summary>
        /// Method to update tables of card tenders
        /// </summary>
        /// <param name="cc">Credit card</param>
        /// <param name="dataSource">Data source</param>
        private void UpdateTables(Credit_Card cc, DataSource dataSource)
        {
            if (cc.Trans_Date == DateTime.MinValue)
            {
                cc.Trans_Date = DateAndTime.Today;
            }
            var strSql = "UPDATE  CardTenders " + " SET BatchNumber=\'" + cc.Sequence_Number + "\' ,"
                            + " BatchDate  = \'" + cc.Trans_Date.ToString("yyyyMMdd") + "\' "
                            + " Where CardTenders.CallTheBank = 1 AND " + " CardTenders.TerminalID = \'"
                            + cc.TerminalID + "\'  AND " + " CardTenders.BatchNumber IS NULL AND CardTenders.Result = \'0\'";
            Execute(strSql, dataSource);
        }

        /// <summary>
        /// Method to set close batch number
        /// </summary>
        /// <param name="cc">Credit card</param>
        public void SetCloseBatchNumber(Credit_Card cc)
        {

            var connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            var dataTable = new DataTable();
            var query = "select * from CloseBatch";
            var adapter = new SqlDataAdapter(query, connection);
            adapter.Fill(dataTable);
            DataRow fields = dataTable.NewRow();
            //Save to the CloseBatch table
            fields["BatchNumber"] = cc.Sequence_Number;
            fields["TerminalID"] = cc.TerminalID;
            fields["BatchDate"] = cc.Trans_Date;
            fields["BatchTime"] = cc.Trans_Time;
            fields["Report"] = cc.Report;
            dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            adapter.InsertCommand = builder.GetInsertCommand();
            adapter.Update(dataTable);
            connection.Close();
            adapter.Dispose();

            //Update all the Tills and Trans for this TerminalID
            UpdateTables(cc, DataSource.CSCTills);
            UpdateTables(cc, DataSource.CSCTrans);
        }

        /// <summary>
        ///Method to get maximum group number 
        /// </summary>
        /// <returns></returns>
        public int? GetMaximumGroupNumber()
        {
            var rs = GetRecords("SELECT MAX(GroupNumber) AS GN FROM TotalizerHist", DataSource.CSCPump);
            if (rs.Rows.Count == 0)
                return null;
            return CommonUtility.GetIntergerValue(rs.Rows[0]["GN"]);
        }

        /// <summary>
        /// Method to get download info
        /// </summary>
        /// <returns>Download info</returns>
        public DownloadInfo GetDownloadInfo()
        {
            var rsClose = GetRecords("SELECT Tra_Close_Num ,Tra_Trans_Time, " + " Tra_AHR_TransNo, Tra_Registry_TransNo FROM DownLoadInfo", DataSource.CSCTrans);
            if (rsClose.Rows.Count == 0)
                return null;
            return new DownloadInfo
            {
                Tra_Close_Num = CommonUtility.GetShortValue(rsClose.Rows[0]["Tra_Close_Num"]),
                Tra_Trans_Time = CommonUtility.GetDateTimeValue(rsClose.Rows[0]["Tra_Trans_Time"]),
                Tra_AHR_TransNo = CommonUtility.GetIntergerValue(rsClose.Rows[0]["Tra_AHR_TransNo"]),
                Tra_Registry_TransNo = CommonUtility.GetIntergerValue(rsClose.Rows[0]["Tra_Registry_TransNo"])
            };

        }

        /// <summary>
        /// Method to update download info
        /// </summary>
        /// <param name="query">Query</param>
        public void UpdateDownloadInfo(string query)
        {
            Execute(query, DataSource.CSCTrans);
        }

        /// <summary>
        /// Method to get terminals for a POS Id
        /// </summary>
        /// <param name="posId"></param>
        /// <returns></returns>
        public List<Terminal> GetTerminals(int posId)
        {
            var terminals = new List<Terminal>();
            var rsTerm = GetRecords("SELECT * FROM TerminalIDs where ID=" + Convert.ToString(posId) + " and TerminalType <> \'Ezipin\'", DataSource.CSCAdmin);
            foreach (DataRow terminal in rsTerm.Rows)
            {
                terminals.Add(new Terminal
                {
                    TerminalId = CommonUtility.GetStringValue(terminal["TerminalID"]),
                    TerminalType = CommonUtility.GetStringValue(terminal["TerminalType"])
                });
            }
            return terminals;
        }

        /// <summary>
        /// Method to get total card transactions for a terminal Id
        /// </summary>
        /// <param name="terminalId">Terminal Id</param>
        /// <returns></returns>
        public int GetTotalCardTransactions(string terminalId)
        {
            var rsCntTrans = GetRecords("SELECT Count(*) AS Tot FROM CardTenders WHERE " + "TerminalID=\'" + terminalId + "\'", DataSource.CSCTills);
            if (rsCntTrans.Rows.Count == 0)
                return 0;
            return CommonUtility.GetIntergerValue(rsCntTrans.Rows[0]["Tot"]);
        }

        /// <summary>
        /// Method to get maximum close number
        /// </summary>
        /// <returns></returns>
        public int GetMaxCloseNumber()
        {
            var rsTemp = GetRecords("SELECT Max(Close_Num)AS [CloseNumber] FROM TaxExemptSaleHead", DataSource.CSCTrans);
            return rsTemp.Rows.Count == 0 ? 0 : CommonUtility.GetIntergerValue(rsTemp.Rows[0]["CloseNumber"]);
        }

        /// <summary>
        /// Method to get last registry file name
        /// </summary>
        /// <returns></returns>
        public string GetLastRegistryFileName()
        {
            var rs = GetRecords("SELECT * FROM Admin WHERE upper(Name) = \'LASTREGISTRY\' ", DataSource.CSCAdmin);
            return rs.Rows.Count == 0 ? string.Empty : CommonUtility.GetStringValue(rs.Rows[0]["Value"]);
        }

        /// <summary>
        /// Method to get tax exempt sale heads
        /// </summary>
        /// <param name="firstCloseNumber">First close number</param>
        /// <param name="lastCloseNumber">Last close number</param>
        /// <returns>List of tax exempt salehead</returns>
        public List<TaxExemptSale> GetTaxExemptSaleHeads(int firstCloseNumber, int lastCloseNumber)
        {
            var teSale = new List<TaxExemptSale>();
            var rsHeader = GetRecords("SELECT * FROM TaxExemptSaleHead  WHERE TotalExemptedTax <> 0 and Close_Num >" + Convert.ToString(firstCloseNumber) + " And Close_Num <= " + Convert.ToString(lastCloseNumber), DataSource.CSCTrans);
            foreach (DataRow row in rsHeader.Rows)
            {
                teSale.Add(new TaxExemptSale
                {
                    Sale_Num = CommonUtility.GetIntergerValue(row["sale_no"]),
                    Sale_Time = CommonUtility.GetDateTimeValue(row["SaleTime"]),
                    teCardholder = new teCardholder
                    {
                        CardNumber = CommonUtility.GetStringValue(row["CardNumber"]),
                        Barcode = CommonUtility.GetStringValue(row["Barcode"])
                    },
                    GasReason = CommonUtility.GetStringValue(row["GasReason"]),
                    PropaneReason = CommonUtility.GetStringValue((row["PropaneReason"])),
                    TobaccoReason = CommonUtility.GetStringValue(row["TobaccoReason"])
                });
            }
            return teSale;
        }

        /// <summary>
        /// Method to get tax exempt sale lines
        /// </summary>
        /// <param name="saleNunber">Sale number</param>
        /// <returns>Tax exempt sale lines</returns>
        public List<TaxExemptSaleLine> GetTaxExemptSaleLines(int saleNunber)
        {
            var teSaleLines = new List<TaxExemptSaleLine>();
            var rsDetail = GetRecords("SELECT * FROM TaxExemptSaleLine" + " WHERE ExemptedTax <> 0 and TaxExemptSaleLine.SALE_NO = " + Convert.ToString(saleNunber), DataSource.CSCTrans);
            foreach (DataRow row in rsDetail.Rows)
            {
                mPrivateGlobals.teProductEnum productType;
                Enum.TryParse(CommonUtility.GetStringValue(row["ProductType"]), out productType);
                teSaleLines.Add(new TaxExemptSaleLine
                {
                    ProductType = productType,
                    StockCode = CommonUtility.GetStringValue(row["Stock_Code"]),
                    OriginalPrice = CommonUtility.GetFloatValue(row["OriginalPrice"]),
                    TaxExemptRate = CommonUtility.GetFloatValue(row["TaxExemptRate"]),
                    ProductCode = CommonUtility.GetStringValue(row["ProductCode"]),
                    Quantity = CommonUtility.GetFloatValue(row["Quantity"])
                });
            }
            return teSaleLines;
        }

        /// <summary>
        /// Method to get tax exempt purchase heads
        /// </summary>
        /// <param name="transactionTime">Transaction time</param>
        /// <returns></returns>
        public List<TePurchaseHeader> GetTaxExemptPurchaseHeads(DateTime transactionTime)
        {
            var purchaseHeaders = new List<TePurchaseHeader>();
            var rsHeader = GetRecords("SELECT * FROM TaxExemptPurchaseHeader WHERE Transdate > \'" + transactionTime + "\'", DataSource.CSCTrans);
            foreach (DataRow row in rsHeader.Rows)
            {
                purchaseHeaders.Add(new TePurchaseHeader
                {
                    PurchaseNumber = CommonUtility.GetIntergerValue(row["PurchaseNumber"]),
                    PurchaseDate = CommonUtility.GetDateTimeValue(row["PurchaseDate"]),
                    Finalised = CommonUtility.GetBooleanValue(row["Finalised"]),
                    InvoiceNumber = CommonUtility.GetStringValue(row["InvoiceNumber"]),
                    TransDate = CommonUtility.GetDateTimeValue(row["TransDate"]),
                    WholeSaleNumber = CommonUtility.GetStringValue(row["WholeSaleNumber"])
                });
            }
            return purchaseHeaders;
        }

        /// <summary>
        /// Method to get tax exempt purchase details
        /// </summary>
        /// <param name="purchaseNumber">Purchase number</param>
        /// <returns>Tax exemptpurchase detail</returns>
        public List<TePurchaseDetail> GetTaxExemptPurchaseDetails(int purchaseNumber)
        {
            var purchaseLines = new List<TePurchaseDetail>();
            var rsDetail = GetRecords("SELECT * FROM TaxExemptPurchaseDetail" + " WHERE TaxExemptPurchaseDetail.PurchaseNumber = \'" + Convert.ToString(purchaseNumber) + "\'", DataSource.CSCTrans);
            foreach (DataRow row in rsDetail.Rows)
            {
                purchaseLines.Add(new TePurchaseDetail
                {
                    PurchaseNumber = CommonUtility.GetIntergerValue(row["PurchaseNumber"]),
                    PurchaseItem = CommonUtility.GetStringValue(row["PurchaseItem"]),
                    ProductCode = CommonUtility.GetStringValue(row["ProductCode"]),
                    Description = CommonUtility.GetStringValue(row["Description"]),
                    PurchaseQuantity = CommonUtility.GetDecimalValue(row["PurchaseQuantity"]),
                    ProductType = CommonUtility.GetShortValue(row["ProductType"]),
                    CurrentStock = CommonUtility.GetStringValue(row["CurrentStock"])
                });
            }
            return purchaseLines;
        }

        /// <summary>
        /// Method to get totals
        /// </summary>
        /// <param name="strSql">Query</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        public void GetTotals(string strSql, int tillNumber, DataSource dataSource)
        {
            var rsTrans = GetRecords(strSql, dataSource);
            if (rsTrans.Rows.Count != 0)
            {
                foreach (DataRow row in rsTrans.Rows)
                {
                    var query = "Select * From BatchTotal where TILL_NUM=" + tillNumber + " and CardName = \'"
                        + CommonUtility.GetStringValue(row["Card_Name"]) + "\'";

                    var dataTable = GetRecords(query, DataSource.CSCTills);
                    if (dataTable.Rows.Count == 0)
                    {
                        var insertCommand = "Insert into BatchTotal (Till_Num, CardName , CardType";
                        if (CommonUtility.GetIntergerValue(row["Sale"]) > 0)
                        {
                            insertCommand = insertCommand + ",SaleCount, SaleAmount) values({0},'{1}','{2}',{3},{4})";
                        }
                        else
                        {
                            insertCommand = insertCommand + ",ReturnCount, ReturnAmount) values({0},'{1}','{2}',{3},{4})";
                        }
                        string command = string.Format(insertCommand, tillNumber, row["Card_Name"], row["Card_Type"],
                           row["TransNos"], row["Sale"]);
                        Execute(command, DataSource.CSCTills);
                    }
                    else
                    {
                        var updateCommand = "Update BatchTotal set ";
                        var fields = dataTable.Rows[0];
                        updateCommand = CommonUtility.GetIntergerValue(row["Sale"]) > 0 ? string.Format(updateCommand + "SaleCount = {0}, SaleAmount = {1}", Convert.ToDouble(fields["SaleCount"]) + Convert.ToDouble(row["TransNos"]), Convert.ToDouble(fields["SaleAmount"]) + Convert.ToDouble(row["Sale"])) : string.Format(updateCommand + "ReturnCount = {0}, ReturnAmount = {1}", Convert.ToDouble(fields["ReturnCount"]) + Convert.ToDouble(row["TransNos"]), Convert.ToDouble(fields["ReturnAmount"]) + Convert.ToDouble(row["Sale"]));
                        updateCommand += " where TILL_NUM=" + tillNumber + " and CardName = \'" + CommonUtility.GetStringValue(row["Card_Name"]) + "\'";
                        Execute(updateCommand, DataSource.CSCTills);
                    }

                }
            }
        }

        /// <summary>
        /// Method to get batch totals
        /// </summary>
        /// <returns></returns>
        public List<BatchTotal> GetBatchTotals()
        {
            var batchTotals = new List<BatchTotal>();
            var rsBatch = GetRecords("Select * From BatchTotal " + "Order BY  BatchTotal.CardType ", DataSource.CSCTills);
            foreach (DataRow row in rsBatch.Rows)
            {
                batchTotals.Add(new BatchTotal
                {
                    CardType = CommonUtility.GetStringValue(row["CardType"]),
                    CardName = CommonUtility.GetStringValue(row["CardName"]),
                    SaleAmount = CommonUtility.GetDoubleValue(row["SaleAmount"]),
                    ReturnAmount = CommonUtility.GetDoubleValue(row["ReturnAmount"]),
                    SaleCount = CommonUtility.GetIntergerValue(row["SaleCount"]),
                    ReturnCount = CommonUtility.GetIntergerValue(row["ReturnCount"])
                });
            }
            return batchTotals;
        }

        /// <summary>
        /// Method to delete batch total
        /// </summary>
        public void DeleteBatchTotal()
        {
            Execute("Delete From BatchTotal", DataSource.CSCTills);
        }

        /// <summary>
        /// Method to get sale lines for fuel product
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="fuelDept">Fuel department</param>
        /// <param name="whereClause">Where condition</param>
        /// <param name="dataSource">Data base</param>
        /// <returns>Sale lines</returns>
        public List<Sale_Line> GetFuelLines(int tillNumber, string fuelDept, string whereClause, DataSource dataSource)
        {
            var saleLines = new List<Sale_Line>();
            var rsFuel = string.IsNullOrEmpty(whereClause) ? GetRecords("Select   SaleLine.Stock_Code,SaleLine.Descript,S_LineTax.Tax_Name, S_LineTax.Tax_Code, Sum(Tax_Added_Amount) as [Taxes],  Sum(Tax_Included_Amount) as [Included]  FROM     SaleLine  " + "INNER JOIN S_LineTax ON SaleLine.Sale_No=S_LineTax.Sale_NO  AND SaleLine.Line_Num=S_LineTax.Line_No  WHERE    SaleLine.SALE_TYPE IN (\'SALE\',\'REFUND\',\'PATP_APP\')  AND    SaleLine.Dept=\'" + fuelDept + "\'" + "  AND    SaleLine.TILL_NUM=" + tillNumber + " " + "Group By SaleLine.Stock_Code,SaleLine.Descript, S_LineTax.Tax_Name,S_LineTax.Tax_Code ", dataSource) : GetRecords("Select   SaleLine.Stock_Code,SaleLine.Descript, S_LineTax.Tax_Name, S_LineTax.Tax_Code, Sum(Tax_Added_Amount) as [Taxes],  Sum(Tax_Included_Amount) as [Included]  FROM     (SaleLine  " + "INNER JOIN S_LineTax ON SaleLine.Sale_No=S_LineTax.Sale_NO  AND SaleLine.Line_Num=S_LineTax.Line_No) INNER JOIN SaleHead  ON SaleLine.Sale_No=SaleHead.Sale_No  WHERE    SaleLine.SALE_TYPE IN (\'SALE\',\'REFUND\',\'PATP_APP\') " + "AND      SaleLine.Dept=\'" + fuelDept + "\'AND " + whereClause + "Group By SaleLine.Stock_Code, SaleLine.Descript,S_LineTax.Tax_Name,S_LineTax.Tax_Code ", dataSource);
            foreach (DataRow row in rsFuel.Rows)
            {
                saleLines.Add(new Sale_Line
                {
                    Stock_Code = CommonUtility.GetStringValue(row["Stock_Code"]),
                    Description = CommonUtility.GetStringValue(row["Descript"]),
                    AddedTax = CommonUtility.GetDecimalValue(row["Taxes"]),
                    Amount = CommonUtility.GetDecimalValue(row["Included"]),
                    DiscountName = CommonUtility.GetStringValue(row["Tax_Name"]),
                    Discount_Code = CommonUtility.GetStringValue(row["Tax_Code"])
                });
            }
            return saleLines;
        }

        /// <summary>
        /// Method to get all EOD groups
        /// </summary>
        /// <returns></returns>
        public List<EodGroup> GetEodGroups()
        {
            var eodGroups = new List<EodGroup>();
            var rsEodGroup = GetRecords("select * from eodgroups  order by eodgroupid", DataSource.CSCMaster);
            foreach (DataRow row in rsEodGroup.Rows)
            {
                eodGroups.Add(new EodGroup
                {
                    GroupId = CommonUtility.GetStringValue(row["Eodgroupid"]),
                    GroupName = CommonUtility.GetStringValue(row["eodgroupname"])
                });
            }
            return eodGroups;
        }

        /// <summary>
        /// Method to get EOD saleline
        /// </summary>
        /// <param name="groupId">EOD group Id</param>
        /// <param name="whereClause">Where condition</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>SaleLine</returns>
        public Sale_Line GetEodSaleLine(string groupId, string whereClause, int tillNumber, DataSource dataSource)
        {
            var rsSales = whereClause.Length == 0 ? GetRecords("SELECT   max(saleline.dept) as [Dept],Sum(SaleLine.Quantity) as [Volume],   Sum(SaleLine.Amount-SaleLine.Discount-SaleLine.Disc_Adj-SaleLine.TE_Amount_Incl) as [Sales]  FROM   (SaleLine INNER JOIN SaleHead ON  " + "        SaleLine.Sale_No = SaleHead.Sale_No) INNER JOIN CSCMaster.dbo.Dept as [D] ON " + "        D.Dept = SaleLine.Dept  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + "       D.eodgroup = " + groupId + "  AND  SaleLine.TILL_NUM=" + tillNumber, dataSource) : GetRecords("SELECT   max(saleline.dept) as [Dept] ,Sum(SaleLine.Quantity) as [Volume],   Sum(SaleLine.Amount-SaleLine.Discount-SaleLine.Disc_Adj-SaleLine.TE_Amount_Incl) as [Sales]  FROM   (SaleLine INNER JOIN SaleHead ON  " + "        SaleLine.Sale_No = SaleHead.Sale_No) INNER JOIN CSCMaster.dbo.Dept as [D] ON " + "        D.Dept = SaleLine.Dept  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + whereClause + " AND " + "       D.eodgroup = " + groupId, dataSource);
            if (rsSales.Rows.Count == 0)
                return null;

            return new Sale_Line
            {
                Dept = CommonUtility.GetStringValue(rsSales.Rows[0]["Dept"]),
                Quantity = CommonUtility.GetFloatValue(rsSales.Rows[0]["Volume"]),
                Amount = CommonUtility.GetDecimalValue(rsSales.Rows[0]["Sales"])
            };
        }

        /// <summary>
        /// Method to get non taxable items
        /// </summary>
        /// <param name="whereClause">Where condition</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        public Sale_Line GetNonTaxableItems(string whereClause, int tillNumber, DataSource dataSource)
        {
            var rsNoTax = whereClause.Length == 0 ? GetRecords("SELECT Count(*) AS CNT, sum(SALELINE.QUANTITY) AS QTY, " + "SUM(SALELINE.AMOUNT-SaleLine.Discount-SaleLine.Disc_Adj) AS AMT FROM SALELINE LEFT JOIN S_LineTax ON " + "(SALELINE.LINE_NUM=S_LineTax.Line_No) AND (SALELINE.SALE_NO=S_LineTax.Sale_No) WHERE S_LineTax.Tax_Name IS NULL AND " + "SaleLine.SALE_TYPE IN (\'SALE\',\'REFUND\',\'EXCHANGE\',\'PATP_APP\') AND " + "SALELINE.TILL_NUM=" + tillNumber, dataSource) : GetRecords("SELECT Count(*) AS CNT, sum(SALELINE.QUANTITY) AS QTY, " + "SUM(SALELINE.AMOUNT-SaleLine.Discount-SaleLine.Disc_Adj) AS AMT FROM (SALELINE LEFT JOIN S_LineTax ON " + "(SALELINE.LINE_NUM=S_LineTax.Line_No) AND (SALELINE.SALE_NO=S_LineTax.Sale_No)) " + "INNER JOIN SaleHead ON SaleLine.Sale_No=SaleHead.Sale_No  WHERE S_LineTax.Tax_Name Is Null AND " + "SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\',\'PATP_APP\') AND " + whereClause, dataSource);
            if (rsNoTax.Rows.Count == 0)
                return null;
            return new Sale_Line
            {
                Line_Num = CommonUtility.GetShortValue(rsNoTax.Rows[0]["CNT"]),
                Quantity = CommonUtility.GetFloatValue(rsNoTax.Rows[0]["QTY"]),
                Amount = CommonUtility.GetDecimalValue(rsNoTax.Rows[0]["AMT"])
            };
        }

        /// <summary>
        /// Method to get non taxable sale lines
        /// </summary>
        /// <param name="whereClause">Where condition</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale lines</returns>
        public List<Sale_Line> GetNonTaxableSaleLines(string whereClause, int tillNumber, DataSource dataSource)
        {
            var saleLines = new List<Sale_Line>();
            var rsNoTaxDept = whereClause.Length == 0 ? GetRecords("SELECT SaleLine.Dept, " + "sum(SALELINE.QUANTITY) AS QTY, " + "SUM(SALELINE.AMOUNT-SaleLine.Discount-SaleLine.Disc_Adj) AS AMT FROM (SALELINE LEFT JOIN S_LineTax ON " + "(SALELINE.LINE_NUM=S_LineTax.Line_No) AND (SALELINE.SALE_NO=S_LineTax.Sale_No))  WHERE S_LineTax.Tax_Name IS NULL AND " + "SaleLine.SALE_TYPE IN (\'SALE\',\'REFUND\',\'EXCHANGE\',\'PATP_APP\')" + " AND SaleLine.TILL_NUM=" + tillNumber + " " + " Group By SaleLine.Dept " + "Order by SaleLine.Dept ", dataSource) : GetRecords("SELECT SaleLine.Dept, " + "sum(SALELINE.QUANTITY) AS QTY, " + "SUM(SALELINE.AMOUNT-SaleLine.Discount-SaleLine.Disc_Adj) AS AMT FROM ((SALELINE LEFT JOIN S_LineTax ON " + "(SALELINE.LINE_NUM=S_LineTax.Line_No) AND (SALELINE.SALE_NO=S_LineTax.Sale_No)) " + "INNER JOIN SaleHead ON SaleLine.Sale_No=SaleHead.Sale_No)  WHERE S_LineTax.Tax_Name IS NULL AND " + "SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\',\'PATP_APP\') AND " + whereClause + " Group By SaleLine.Dept " + "Order by SaleLine.Dept ", dataSource);
            foreach (DataRow row in rsNoTaxDept.Rows)
            {
                saleLines.Add(new Sale_Line
                {
                    Dept = CommonUtility.GetStringValue(row["Dept"]),
                    Quantity = CommonUtility.GetFloatValue(row["QTY"]),
                    Amount = CommonUtility.GetDecimalValue(row["AMT"])
                });
            }
            return saleLines;
        }

        /// <summary>
        /// Method to get department by department Id
        /// </summary>
        /// <param name="deptId">Department Id</param>
        /// <returns></returns>
        public string GetDepartmentById(string deptId)
        {
            var rsTemp = GetRecords("select * from dept where dept = \'" + deptId + "\'", DataSource.CSCMaster);
            if (rsTemp.Rows.Count == 0) return null;
            return CommonUtility.GetStringValue(rsTemp.Rows[0]["Dept_Name"]);
        }

        /// <summary>
        /// Method to get purchase item
        /// </summary>
        /// <param name="categoryId">Category Id</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        public Sale_Line GetPurchaseItem(string categoryId, string whereClause, int tillNumber, DataSource dataSource)
        {
            var rsTaxExempt = !string.IsNullOrEmpty(whereClause) ? GetRecords("Select Sum(Quantity) as [sQuantity], " + "Sum(Amount) as [sAmount], " + "Sum(TotalTaxSaved) as [sTotalTaxSaved]  FROM  PurchaseItem INNER JOIN SaleHead ON " + "    PurchaseItem.Sale_No = SaleHead.Sale_No " + "    and Purchaseitem.till_no = salehead.till  WHERE CategoryCodeFK= " + Convert.ToString(categoryId) + " AND " + whereClause + " Group By CategoryCodeFK", DataSource.CSCTrans) : GetRecords("Select Sum(Quantity) as [sQuantity], " + "Sum(Amount) as [sAmount], " + "Sum(TotalTaxSaved) as [sTotalTaxSaved]  FROM  PurchaseItem  WHERE CategoryCodeFK= " + categoryId + " AND Till_No=" + Convert.ToString(tillNumber) + " Group By CategoryCodeFK", dataSource);
            if (rsTaxExempt.Rows.Count == 0) return null;
            return new Sale_Line
            {
                Quantity = CommonUtility.GetFloatValue(rsTaxExempt.Rows[0]["sQuantity"]),
                Amount = CommonUtility.GetDecimalValue(rsTaxExempt.Rows[0]["sAmount"]),
                AddedTax = CommonUtility.GetDecimalValue(rsTaxExempt.Rows[0]["sTotalTaxSaved"])
            };
        }

        /// <summary>
        /// Method to get fuel sale lines
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="fuelDept">Fuel dept</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        public List<Sale_Line> GetFuelSaleLines(int tillNumber, string whereClause, string fuelDept, DataSource dataSource)
        {
            var saleLines = new List<Sale_Line>();
            var rsSales = whereClause.Length == 0 ? GetRecords("SELECT SaleLine.Stock_Code, SaleLine.Descript, " + "       SaleLine.Dept, D.Dept_Name, " + "       Sum(SaleLine.Quantity) as [Volume], " + "       Sum(SaleLine.Amount) as [Sales], " + "       Sum(SaleLine.REG_PRICE * SaleLine.QUANTITY) as [OrigSales], " + "       Sum(-SaleLine.Discount-SaleLine.Disc_Adj) as [Discount], Sum(SaleLine.TE_Amount_Incl) AS TaxEx  FROM   (SaleLine INNER JOIN SaleHead ON  " + "        SaleLine.Sale_No = SaleHead.Sale_No) INNER JOIN CSCMaster.dbo.Dept as [D] ON " + "        D.Dept = SaleLine.Dept  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\',\'PATP_APP\') AND " + "       SaleLine.Dept = \'" + fuelDept + "\' AND " + "       SaleLine.Till_Num=" + tillNumber + " Group By SaleLine.Stock_Code, SaleLine.Descript, SaleLine.Dept, D.Dept_Name " + "Order by SaleLine.Stock_Code ", dataSource) : GetRecords("SELECT SaleLine.Stock_Code, SaleLine.Descript, " + "       SaleLine.Dept, D.Dept_Name, " + "       Sum(SaleLine.Quantity) as [Volume], " + "       Sum(SaleLine.Amount) as [Sales], " + "       Sum(SaleLine.REG_PRICE * SaleLine.QUANTITY) as [OrigSales], " + "       Sum(-SaleLine.Discount-SaleLine.Disc_Adj) as [Discount], Sum(SaleLine.TE_Amount_Incl) AS TaxEx  FROM   (SaleLine INNER JOIN SaleHead ON  " + "        SaleLine.Sale_No = SaleHead.Sale_No) INNER JOIN CSCMaster.dbo.Dept as [D] ON " + "        D.Dept = SaleLine.Dept  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\',\'PATP_APP\') AND " + whereClause + " AND " + "       SaleLine.Dept = \'" + fuelDept + "\' " + " Group By SaleLine.Stock_Code, SaleLine.Descript, SaleLine.Dept, D.Dept_Name " + "Order by SaleLine.Stock_Code ", dataSource);

            foreach (DataRow row in rsSales.Rows)
            {
                saleLines.Add(new Sale_Line
                {
                    Stock_Code = CommonUtility.GetStringValue(row["Stock_Code"]),
                    Description = CommonUtility.GetStringValue(row["Descript"]),
                    Dept = CommonUtility.GetStringValue(row["Dept"]),
                    Sub_Detail = CommonUtility.GetStringValue(row["Dept_Name"]),
                    Quantity = CommonUtility.GetFloatValue(row["Volume"]),
                    Amount = CommonUtility.GetDecimalValue(row["Sales"]),
                    price = CommonUtility.GetDoubleValue(row["OrigSales"]),
                    Discount_Adjust = CommonUtility.GetDoubleValue(row["Discount"]),
                    TE_Amount_Incl = CommonUtility.GetDecimalValue(row["TaxEx"])
                });
            }
            return saleLines;
        }

        /// <summary>
        /// Method to get taxable saleLines
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="fuelDept">Fuel dept</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        public List<Sale_Line> GetTaxableSaleLines(int tillNumber, string whereClause, string fuelDept, DataSource dataSource)
        {
            var saleLines = new List<Sale_Line>();
            var rsFInclTax = whereClause.Length == 0 ? GetRecords("SELECT SaleLine.Stock_Code, SaleLine.Descript, " + "       SaleLine.Dept, D.Dept_Name, " + " Sum(S_linetax.Tax_Included_Amount) as [Includedtax]  FROM   (SaleLine INNER JOIN SaleHead ON  " + "  SaleLine.Sale_No = SaleHead.Sale_No and SaleLine.Till_num = SaleHead.Till ) " + " INNER JOIN S_LINETAX ON " + "saleline.Sale_No = s_linetax.Sale_No " + " And saleline.till_num = s_linetax.till_num " + " and saleline.line_num=s_linetax.line_no " + " INNER JOIN CSCMaster.dbo.Dept as [D] ON " + "        D.Dept = SaleLine.Dept  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\',\'PATP_APP\') AND " + "       SaleLine.Dept = \'" + fuelDept + "\' AND " + "       SaleLine.Till_Num=" + tillNumber + " Group By SaleLine.Stock_Code, SaleLine.Descript, SaleLine.Dept, D.Dept_Name " + " Order by SaleLine.Stock_Code ", dataSource) : GetRecords("SELECT SaleLine.Stock_Code, SaleLine.Descript, " + "       SaleLine.Dept, D.Dept_Name, " + " Sum(S_linetax.Tax_Included_Amount) as [Includedtax]  FROM   (SaleLine INNER JOIN SaleHead ON  " + "        SaleLine.Sale_No = SaleHead.Sale_No) " + " INNER JOIN S_LINETAX ON " + "saleline.Sale_No = s_linetax.Sale_No " + " And saleline.till_num = s_linetax.till_num " + " and saleline.line_num=s_linetax.line_no " + " INNER JOIN CSCMaster.dbo.Dept as [D] ON " + "        D.Dept = SaleLine.Dept  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\',\'PATP_APP\') AND " + whereClause + " AND " + " SaleLine.Dept = \'" + fuelDept + "\' " + " Group By SaleLine.Stock_Code, SaleLine.Descript, SaleLine.Dept, D.Dept_Name " + " Order by SaleLine.Stock_Code ", dataSource);
            foreach (DataRow row in rsFInclTax.Rows)
            {
                saleLines.Add(new Sale_Line
                {
                    Stock_Code = CommonUtility.GetStringValue(row["Stock_Code"]),
                    Description = CommonUtility.GetStringValue(row["Descript"]),
                    Dept = CommonUtility.GetStringValue(row["Dept"]),
                    Sub_Detail = CommonUtility.GetStringValue(row["Dept_Name"]),
                    AddedTax = CommonUtility.GetDecimalValue(row["Includedtax"])
                });
            }
            return saleLines;
        }

        /// <summary>
        /// Method to get total deposits
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where condition</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Deposits</returns>
        public decimal GetChargeDeposit(int tillNumber, string whereClause, DataSource dataSource)
        {
            DataTable rsDep, rsDepK;

            if (whereClause.Length == 0)
            {

                rsDep = GetRecords("Select Sum(Amount) as [Deposits]  FROM   SaleChg INNER JOIN SaleHead ON " + "       SaleChg.Sale_No = SaleHead.Sale_No  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + "       SaleChg.TILL_NUM=" + tillNumber, dataSource);
                rsDepK = GetRecords("Select Sum(Amount) as [Deposits]  FROM   SaleKitChg INNER JOIN SaleHead ON " + "       SaleKitChg.Sale_No = SaleHead.Sale_No  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\')  AND " + "       SaleKitChg.TILL_NUM=" + tillNumber, dataSource);
            }
            else
            {
                rsDep = GetRecords("Select Sum(Amount) as [Deposits]  FROM   SaleChg INNER JOIN SaleHead ON " + "       SaleChg.Sale_No = SaleHead.Sale_No  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + whereClause, dataSource);
                rsDepK = GetRecords("Select Sum(Amount) as [Deposits]  FROM   SaleKitChg INNER JOIN SaleHead ON " + "       SaleKitChg.Sale_No = SaleHead.Sale_No  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + whereClause, dataSource);
            }
            var chargeSum = (rsDep.Rows.Count > 0) ? CommonUtility.GetDecimalValue(rsDep.Rows[0]["Deposits"]) : 0;
            var kitSum = (rsDepK.Rows.Count > 0) ? CommonUtility.GetDecimalValue(rsDepK.Rows[0]["Deposits"]) : 0;

            return chargeSum + kitSum;
        }

        /// <summary>
        /// Method to get sale charges
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of charges</returns>
        public List<Charge> GetSaleCharges(int tillNumber, DataSource dataSource)
        {
            var saleCharges = new List<Charge>();

            var rsTypeDep = GetRecords("Select  distinct AS_CODE, Description  FROM   SaleChg  WHERE  SaleChg.TILL_NUM=" + tillNumber, dataSource);
            foreach (DataRow row in rsTypeDep.Rows)
            {
                saleCharges.Add(new Charge
                {
                    AsCode = CommonUtility.GetStringValue(row["AS_CODE"]),
                    Description = CommonUtility.GetStringValue(row["Description"])
                });
            }
            return saleCharges;
        }

        /// <summary>
        /// Method to get sale kit charges
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of kit charges</returns>
        public List<Charge> GetSaleKitCharge(int tillNumber, DataSource dataSource)
        {
            var saleKitCharges = new List<Charge>();

            var rsTypeDepk = GetRecords("Select  distinct AS_CODE, Description  FROM   SaleKitChg  WHERE  SaleKitChg.TILL_NUM=" + tillNumber, dataSource);
            foreach (DataRow row in rsTypeDepk.Rows)
            {
                saleKitCharges.Add(new Charge
                {
                    AsCode = CommonUtility.GetStringValue(row["AS_CODE"]),
                    Description = CommonUtility.GetStringValue(row["Description"])
                });
            }

            return saleKitCharges;
        }

        /// <summary>
        /// Method to get total charge
        /// </summary>
        /// <param name="asCode">As code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Charge</returns>
        public Charge GetTotalCharge(string asCode, int tillNumber, string whereClause, DataSource dataSource)
        {
            var charge = new Charge();
            var rsDep = whereClause.Length == 0 ? GetRecords("Select  " + "Sum(QUANTITY) as [Qty], Sum(Amount) as [Amt]  FROM   SaleChg INNER JOIN SaleHead ON " + "       SaleChg.Sale_No = SaleHead.Sale_No  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\')  " + " And SaleChg.AS_CODE =\'" + asCode + "\' AND " + " SaleChg.TILL_NUM=" + tillNumber, dataSource) : GetRecords("Select  " + "Sum(QUANTITY) as [Qty], Sum(Amount) as [Amt]  FROM   SaleChg INNER JOIN SaleHead ON " + "       SaleChg.Sale_No = SaleHead.Sale_No  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\') AND " + whereClause + " " + " And SaleChg.AS_CODE =\'" + asCode, dataSource);
            if (rsDep.Rows.Count != 0)
            {
                charge.Quantity = CommonUtility.GetFloatValue(rsDep.Rows[0]["Qty"]);
                charge.Amount = CommonUtility.GetFloatValue(rsDep.Rows[0]["Amt"]);
            }
            return charge;
        }

        /// <summary>
        /// Method to get total kit charge
        /// </summary>
        /// <param name="asCode">As code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Charge</returns>
        public Charge GetTotalKitCharge(string asCode, int tillNumber, string whereClause, DataSource dataSource)
        {
            var charge = new Charge();
            var rsDep = whereClause.Length == 0 ? GetRecords("Select  " + "Sum(QUANTITY) as [Qty], Sum(Amount) as [Amt]  FROM   SaleKitChg INNER JOIN SaleHead ON " + "       SaleKitChg.Sale_No = SaleHead.Sale_No  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\')  " + " And SaleKitChg.AS_CODE =\'" + asCode + "\' AND " + " SaleKitChg.TILL_NUM=" + tillNumber, dataSource) : GetRecords("Select  " + "Sum(QUANTITY) as [Qty], Sum(Amount) as [Amt]  FROM   SaleKitChg INNER JOIN SaleHead ON " + "       SaleKitChg.Sale_No = SaleHead.Sale_No  WHERE  SaleHead.T_Type IN (\'SALE\',\'REFUND\') AND " + whereClause + " " + " And SaleKitChg.AS_CODE =\'" + asCode + "\'", dataSource);
            if (rsDep.Rows.Count != 0)
            {
                charge.Quantity = CommonUtility.GetFloatValue(rsDep.Rows[0]["Qty"]);
                charge.Amount = CommonUtility.GetFloatValue(rsDep.Rows[0]["Amt"]);
            }
            return charge;
        }

        /// <summary>
        /// Method to get tax exempt sale lines
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="fuelDept">Fuel dept</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="teType">tax exempt type</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale lines</returns>
        public List<Sale_Line> GetTaxExemptSaleLines(int tillNumber, string fuelDept, string whereClause, string teType, DataSource dataSource)
        {
            var saleLines = new List<Sale_Line>();
            DataTable rsTeSales = new DataTable();
            if (teType == "AITE" || teType == "QITE")
            {
                rsTeSales = whereClause.Length == 0 ? GetRecords("SELECT SaleLine.Stock_Code, SaleLine.Descript, " + "       SaleLine.Dept, D.Dept_Name, " + "       Sum(TaxExemptSaleLine.Quantity) as [Volume], " + "       Sum(TaxExemptSaleLine.Amount) as [Sales], " + "       Sum(-SaleLine.Discount-SaleLine.Disc_Adj) as [Discount]  FROM   (SaleLine INNER JOIN TaxExemptSaleHead ON  " + "        SaleLine.Sale_No = TaxExemptSaleHead.Sale_No and " + "        Saleline.Till_num  = TaxExemptsalehead.Till_num )" + " INNER JOIN TaxExemptSaleLine ON " + "       TaxExemptSaleLine.Sale_No =Saleline.Sale_no" + "       and TaxExemptSaleLine.Till_NUm =Saleline.Till_Num" + "       and TaxExemptSaleline.Line_num = saleline.line_num " + "       INNER JOIN CSCMaster.dbo.Dept as [D] ON " + "       D.Dept = SaleLine.Dept  WHERE    SaleLine.SALE_TYPE IN (\'SALE\',\'REFUND\',\'PATP_APP\') " + "AND      SaleLine.Dept=\'" + fuelDept + "\' " + "  AND    SaleLine.TILL_NUM=" + tillNumber + " " + " Group By SaleLine.Stock_Code, SaleLine.Descript, SaleLine.Dept, D.Dept_Name " + " Order by SaleLine.Stock_Code ", dataSource) : GetRecords("SELECT SaleLine.Stock_Code, SaleLine.Descript, " + "       SaleLine.Dept, D.Dept_Name, " + "       Sum(TaxExemptSaleLine.Quantity) as [Volume], " + "       Sum(TaxExemptSaleLine.Amount) as [Sales], " + "       Sum(-SaleLine.Discount-SaleLine.Disc_Adj) as [Discount]  FROM   (SaleLine INNER JOIN TaxExemptSaleHead ON  " + "        SaleLine.Sale_No = TaxExemptSaleHead.Sale_No and " + "        Saleline.Till_num  = TaxExemptsalehead.Till_num )" + " INNER JOIN TaxExemptSaleLine ON " + "       TaxExemptSaleLine.Sale_No =Saleline.Sale_no" + "       and TaxExemptSaleLine.Till_NUm =Saleline.Till_Num" + "       and TaxExemptSaleline.Line_num = saleline.line_num " + "       INNER JOIN CSCMaster.dbo.Dept as [D] ON " + "       D.Dept = SaleLine.Dept  WHERE    SaleLine.SALE_TYPE IN (\'SALE\',\'REFUND\',\'PATP_APP\') " + "AND      SaleLine.Dept=\'" + fuelDept + "\'AND " + whereClause + " Group By SaleLine.Stock_Code, SaleLine.Descript, SaleLine.Dept, D.Dept_Name " + " Order by SaleLine.Stock_Code ", dataSource);


            }
            else if (teType == "SITE")
            {
                if (whereClause.Length == 0)
                {

                    rsTeSales = GetRecords("SELECT SaleLine.Stock_Code, SaleLine.Descript, " + "       SaleLine.Dept, D.Dept_Name, " + "       Sum(purchaseitem.Quantity) as [Volume], " + "       Sum(purchaseitem.Amount) as [Sales], " + "       Sum(-SaleLine.Discount-SaleLine.Disc_Adj) as [Discount]  FROM   ( SaleLine INNER JOIN SaleHead ON  " + "       SaleLine.Sale_No = SaleHead.Sale_No and " + "       SaleLine.Till_num = SaleHead.Till ) " + " INNER JOIN purchaseitem ON " + "       purchaseitem.Sale_No =Saleline.Sale_no" + "       and purchaseitem.Till_No =Saleline.Till_Num" + "       and purchaseitem.Line_no = saleline.line_num " + "       INNER JOIN CSCMaster.dbo.Dept as [D] ON " + "       D.Dept = SaleLine.Dept  WHERE    SaleLine.SALE_TYPE IN (\'SALE\',\'REFUND\',\'PATP_APP\') " + " AND      SaleLine.Dept=\'" + fuelDept + "\'" + "  AND    SaleLine.TILL_NUM=" + tillNumber + " Group By SaleLine.Stock_Code, SaleLine.Descript, SaleLine.Dept, D.Dept_Name " + " Order by SaleLine.Stock_Code ", dataSource);
                }
                else
                {
                    rsTeSales = GetRecords("SELECT SaleLine.Stock_Code, SaleLine.Descript, " + "       SaleLine.Dept, D.Dept_Name, " + "       Sum(purchaseitem.Quantity) as [Volume], " + "       Sum(purchaseitem.Amount) as [Sales], " + "       Sum(-SaleLine.Discount-SaleLine.Disc_Adj) as [Discount]  FROM   ( SaleLine INNER JOIN SaleHead ON  " + "       SaleLine.Sale_No = SaleHead.Sale_No and " + "       SaleLine.Till_num = SaleHead.Till ) " + " INNER JOIN purchaseitem ON " + "       purchaseitem.Sale_No =Saleline.Sale_no" + "       and purchaseitem.Till_No =Saleline.Till_Num" + "       and purchaseitem.Line_no = saleline.line_num " + "       INNER JOIN CSCMaster.dbo.Dept as [D] ON " + "       D.Dept = SaleLine.Dept  WHERE    SaleLine.SALE_TYPE IN (\'SALE\',\'REFUND\',\'PATP_APP\') " + "AND      SaleLine.Dept=\'" + fuelDept + "\'AND " + whereClause + " Group By SaleLine.Stock_Code, SaleLine.Descript, SaleLine.Dept, D.Dept_Name " + " Order by SaleLine.Stock_Code ", dataSource);
                }
            }
            foreach (DataRow row in rsTeSales.Rows)
            {
                saleLines.Add(new Sale_Line
                {
                    Stock_Code = CommonUtility.GetStringValue(row["Stock_Code"]),
                    Description = CommonUtility.GetStringValue(row["Descript"]),
                    Dept = CommonUtility.GetStringValue(row["Dept"]),
                    Sub_Detail = CommonUtility.GetStringValue(row["Dept_Name"]),
                    Quantity = CommonUtility.GetFloatValue(row["Volume"]),
                    Amount = CommonUtility.GetDecimalValue(row["Sales"]),
                    Discount_Adjust = CommonUtility.GetDoubleValue(row["Discount"])
                });
            }
            return saleLines;
        }

        /// <summary>
        /// Method to get sale taxes
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        public List<Sale_Tax> GetSaleTaxes(int tillNumber, string whereClause, DataSource dataSource)
        {
            var saleTaxes = new List<Sale_Tax>();
            var rsTax = whereClause.Length == 0 ? GetRecords("Select   S_SaleTax.Tax_Name,   Sum(Tax_Added_Amount) as [Taxes], Sum(Tax_Included_Amount) as [Included], Sum(Tax_Rebate) as [Rebate]  FROM     S_SaleTax INNER JOIN SaleHead ON  S_SaleTax.Sale_No = SaleHead.Sale_No  WHERE    SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') " + "    AND  S_SaleTax.TILL_NUM=" + tillNumber + " Group By S_SaleTax.Tax_Name ", dataSource) : GetRecords("Select   S_SaleTax.Tax_Name,  Sum(Tax_Added_Amount) as [Taxes],  Sum(Tax_Included_Amount) as [Included],  Sum(Tax_Rebate) as [Rebate]  FROM     S_SaleTax INNER JOIN SaleHead ON  S_SaleTax.Sale_No = SaleHead.Sale_No  WHERE    SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + whereClause + " Group By S_SaleTax.Tax_Name ", dataSource);
            foreach (DataRow tax in rsTax.Rows)
            {
                saleTaxes.Add(new Sale_Tax
                {
                    Tax_Name = CommonUtility.GetStringValue(tax["Tax_Name"]),
                    Tax_Added_Amount = CommonUtility.GetDecimalValue(tax["Taxes"]),
                    Tax_Included_Amount = CommonUtility.GetDecimalValue(tax["Included"]),
                    Tax_Rebate = CommonUtility.GetDecimalValue(tax["Rebate"])
                });
            }
            return saleTaxes;
        }

        /// <summary>
        /// Method to get line taxes for fuel dept
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="fuelDept">Fuel dept</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        public List<Line_Tax> GetLineTaxesForFuel(int tillNumber, string whereClause, string fuelDept, DataSource dataSource)
        {
            var lineTaxes = new List<Line_Tax>();
            DataTable rsTaxF;
            rsTaxF = whereClause.Length == 0 ? GetRecords("Select   S_LineTax.Tax_Name,  Sum(Tax_Added_Amount) as [Taxes], Sum(Tax_Included_Amount) as [Included]  FROM     S_LineTax  " + "INNER JOIN SaleLine ON S_LineTax.Sale_No=SaleLine.Sale_NO " + "         AND S_LineTax.Line_No=SaleLine.Line_Num  WHERE    SaleLine.SALE_TYPE IN (\'SALE\',\'REFUND\') " + "    AND  SaleLine.Dept=\'" + fuelDept + "\' " + "    AND  S_LineTax.TILL_NUM=" + tillNumber + " Group By S_LineTax.Tax_Name ", dataSource) : GetRecords("Select   S_LineTax.Tax_Name,  Sum(Tax_Added_Amount) as [Taxes],  Sum(Tax_Included_Amount) as [Included]  FROM     (S_LineTax  " + "INNER JOIN SaleLine ON S_LineTax.Sale_No=SaleLine.Sale_NO " + "         AND S_LineTax.Line_No=SaleLine.Line_Num) INNER JOIN SaleHead " + "         ON SaleLine.Sale_No=SaleHead.Sale_No  WHERE    SaleLine.SALE_TYPE IN (\'SALE\',\'REFUND\') " + "    AND  SaleLine.Dept=\'" + fuelDept + "\'AND " + whereClause + " Group By S_LineTax.Tax_Name ", dataSource);
            foreach (DataRow tax in rsTaxF.Rows)
            {
                lineTaxes.Add(new Line_Tax
                {
                    Tax_Name = CommonUtility.GetStringValue(tax["Tax_Name"]),
                    Tax_Added_Amount = CommonUtility.GetFloatValue(tax["Taxes"]),
                    Tax_Incl_Amount = CommonUtility.GetFloatValue(tax["Included"]),
                });
            }
            return lineTaxes;
        }

        /// <summary>
        /// Method to get kit charges
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Charges</returns>
        public List<Charge_Tax> GetKitCharges(int tillNumber, string whereClause, DataSource dataSource)
        {
            var kitCharges = new List<Charge_Tax>();
            var rsTaxKc = whereClause.Length == 0 ? GetRecords("Select   SaleKitChgTax.Tax_Name, Sum(Tax_Added_Amount) as [Taxes] ,Sum(Tax_Included_Amount) as [Included] FROM     SaleKitChgTax INNER JOIN SaleHead ON  SaleKitChgTax.Sale_No = SaleHead.Sale_No  WHERE    SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + "         SaleKitChgTax.Tax_Included <> 1 " + "Group By SaleKitChgTax.Tax_Name ", dataSource) : GetRecords("Select   SaleKitChgTax.Tax_Name, Sum(Tax_Added_Amount) as [Taxes],Sum(Tax_Included_Amount) as [Included]  FROM     SaleKitChgTax INNER JOIN SaleHead ON SaleKitChgTax.Sale_No = SaleHead.Sale_No  WHERE    SaleHead.T_Type IN (\'SALE\',\'REFUND\',\'EXCHANGE\') AND " + whereClause + " AND " + "         SaleKitChgTax.Tax_Included <> 1 AND " + "         SaleKitChgTax.Tax_Included <> 1 " + "Group By SaleKitChgTax.Tax_Name ", dataSource);
            foreach (DataRow tax in rsTaxKc.Rows)
            {
                kitCharges.Add(new Charge_Tax
                {
                    Tax_Name = CommonUtility.GetStringValue(tax["Tax_Name"]),
                    Tax_Added_Amount = CommonUtility.GetFloatValue(tax["Taxes"]),
                    Tax_Incl_Amount = CommonUtility.GetFloatValue(tax["Included"])
                });
            }
            return kitCharges;
        }

        /// <summary>
        /// Method to get totalizer history by group number
        /// </summary>
        /// <param name="groupNumber">Group number</param>
        /// <returns></returns>
        public List<TotalizerHist> GetTotalizerHistByGroupNumber(int groupNumber)
        {
            var totalizers = new List<TotalizerHist>();
            var rs = GetRecords("Select *  FROM   TotalizerHist  WHERE  TotalizerHist.GroupNumber = " + Convert.ToString(groupNumber) + " ", DataSource.CSCPump);
            foreach (DataRow row in rs.Rows)
            {
                totalizers.Add(new TotalizerHist
                {
                    Date = CommonUtility.GetDateTimeValue(row["Date_Time"]),
                    PumpId = CommonUtility.GetIntergerValue(row["pumpID"]),
                    PositionId = CommonUtility.GetShortValue(row["PositionID"]),
                    Volume = CommonUtility.GetDecimalValue(row["Volume"]),
                    Dollars = CommonUtility.GetDecimalValue(row["dollars"])
                });
            }
            return totalizers;
        }

        /// <summary>
        /// Method to get total totalizer reading
        /// </summary>
        /// <param name="groupNumber">Group number</param>
        /// <returns></returns>
        public List<TotalizerHist> GetTotalReading(int groupNumber)
        {
            var totalizers = new List<TotalizerHist>();
            var rs = GetRecords("Select Grade, SUM(Volume) as TotVolume, SUM(dollars) as TotDollar  " + " From   TotalizerHist " + " Where  TotalizerHist.GroupNumber = " + Convert.ToString(groupNumber) + " " + " Group by Grade", DataSource.CSCPump);

            foreach (DataRow row in rs.Rows)
            {
                totalizers.Add(new TotalizerHist
                {
                    Grade = CommonUtility.GetIntergerValue(row["Grade"]),
                    Volume = CommonUtility.GetDecimalValue(row["TotVolume"]),
                    Dollars = CommonUtility.GetDecimalValue(row["TotDollar"])
                });
            }
            return totalizers;
        }

        /// <summary>
        /// Method to get maximum high low
        /// </summary>
        /// <returns></returns>
        public short? GetMaximumHighLow()
        {
            var rsHl = GetRecords("Select Max(GroupNumber) As [MG]  FROM   TotalHighLow ", DataSource.CSCPump);
            if (rsHl.Rows.Count == 0) return null;
            return CommonUtility.GetShortValue(rsHl.Rows[0]["MG"]);
        }

        /// <summary>
        /// Method to get total high lows
        /// </summary>
        /// <param name="groupNumber">Group number</param>
        /// <returns>High lows</returns>
        public List<TotalHighLow> GetTotalHighLows(int groupNumber)
        {
            var totalHighLows = new List<TotalHighLow>();
            var rsHl = GetRecords("Select *  FROM   TotalHighLow  WHERE  TotalHighLow.GroupNumber = " + Convert.ToString(groupNumber) + " ", DataSource.CSCPump);
            foreach (DataRow row in rsHl.Rows)
            {
                totalHighLows.Add(new TotalHighLow
                {
                    PumpId = CommonUtility.GetIntergerValue(row["pumpID"]),
                    HighVolume = CommonUtility.GetDoubleValue(row["HighVolume"]),
                    LowVolume = CommonUtility.GetDoubleValue(row["LowVolume"]),
                    Date = CommonUtility.GetDateTimeValue(row["Date_Time"])
                });
            }

            return totalHighLows;
        }

        /// <summary>
        /// Method to get total over payment
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>Over payment</returns>
        public decimal GetOverPaymentForTrans(string whereClause)
        {
            var rsOverPay = GetRecords("Select   Sum(SaleHead.OverPayment) as [Used]  FROM     SaleHead  WHERE    SaleHead.T_Type IN (\'SALE\',\'REFUND\') " + "         and " + whereClause, DataSource.CSCTrans);
            if (rsOverPay.Rows.Count > 0)
                return CommonUtility.GetDecimalValue(rsOverPay.Rows[0]["USED"]);
            return 0;
        }

        /// <summary>
        /// Method to get total payments
        /// </summary>
        /// <returns>Payments</returns>
        public decimal GetPaymentsForTrans(string whereClause)
        {
            var rsPayments = GetRecords("Select   Sum(SaleHead.Payment) as [Used]  FROM     SaleHead  WHERE    SaleHead.T_Type = \'PAYMENT\' " + "         and " + whereClause, DataSource.CSCTrans);

            if (rsPayments.Rows.Count > 0)
                return CommonUtility.GetDecimalValue(rsPayments.Rows[0]["USED"]);

            return 0;
        }

        /// <summary>
        /// Method to get total AR payments
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>AR pament</returns>
        public decimal GetArPaymentForTrans(string whereClause)
        {
            var rsArPay = GetRecords("Select   Sum(SaleHead.Payment) as [Used]  FROM     SaleHead  WHERE    SaleHead.T_Type = \'ARPAY\' " + "         and " + whereClause, DataSource.CSCTrans);
            if (rsArPay.Rows.Count > 0)

                return CommonUtility.GetDecimalValue(rsArPay.Rows[0]["USED"]);
            //    Debug.Print ARPay

            return 0;
        }

        /// <summary>
        /// Method to get total payouts
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>Payouts</returns>
        public decimal GetPayoutsForTrans(string whereClause)
        {
            var rsPayOuts = GetRecords("Select   Sum(SaleHead.PayOut) as [Used]  FROM     SaleHead  WHERE    SaleHead.T_Type = \'PAYOUT\' " + "         and " + whereClause, DataSource.CSCTrans);

            if (rsPayOuts.Rows.Count > 0)

                return CommonUtility.GetDecimalValue(rsPayOuts.Rows[0]["USED"]);
            return 0;
        }

        /// <summary>
        /// Method to total penny adjustments
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>Penny adjustment</returns>
        public decimal GetPennyAdjustmentForTrans(string whereClause)
        {
            var rsPennyAdj = GetRecords("Select   Sum(SaleHead.Penny_Adj) as [PennyAdj]  FROM     SaleHead  WHERE " + whereClause, DataSource.CSCTrans);

            if (rsPennyAdj.Rows.Count > 0)

                return CommonUtility.GetDecimalValue(rsPennyAdj.Rows[0]["PennyAdj"]);

            return 0;
        }

        /// <summary>
        /// Method to get total change
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>Change</returns>
        public double GetChangeSumForTrans(string whereClause)
        {
            var rsChange = GetRecords("Select Sum(SaleHead.Change) as [SumChg]  FROM   SaleHead WHERE SaleHead.T_Type <> \'VOID\' AND " + whereClause, DataSource.CSCTrans);

            if (rsChange.Rows.Count > 0)

                return CommonUtility.GetDoubleValue(rsChange.Rows[0]["SumChg"]);
            return 0;
        }

        /// <summary>
        /// Method to get draw and bonus
        /// </summary>
        /// <param name="shiftDate"></param>
        /// <returns>Draw and bonus</returns>
        public List<decimal> GetDrawAndBonusForTrans(DateTime shiftDate)
        {
            var result = new List<decimal>();
            var rsDraw = GetRecords("Select Count(*)             as [nDraw], " + "       Sum(CashDraw.Amount) as [Draw_Total] " + " ,Sum(CashDraw.CashBonus) as [CashBonus]  FROM   CashDraw  WHERE  Draw_Date=\'" + shiftDate.ToString("yyyyMMdd") + "\'", DataSource.CSCTrans);
            if (rsDraw.Rows.Count == 0)
            {
                result.Add(0);
                result.Add(0);
            }
            else
            {
                result.Add(CommonUtility.GetDecimalValue(rsDraw.Rows[0]["Draw_Total"]));
                result.Add(CommonUtility.GetDecimalValue(rsDraw.Rows[0]["CashBonus"]));
            }

            return result;
        }

        /// <summary>
        /// Method to clear previous till close
        /// </summary>
        public void ClearPreviousTillCloseForTrans()
        {
            Execute("Delete From TillClose ", DataSource.CSCTrans);
        }

        /// <summary>
        /// Method to get sale tend amount for till
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>Sale tenders</returns>
        public List<SaleTendAmount> GetSaleTendAmountForTrans(string whereClause)
        {
            var saleTenders = new List<SaleTendAmount>();
            var rsTenders = GetRecords("Select   SaleTend.TendName as [Tender], " + "         Count(*) as [Count], " + "         Sum(SaleTend.AmtTend) as [Amount], " + "         Sum(SaleTend.AmtUsed) as [Used]  FROM     SaleTend  INNER JOIN SaleHead  ON " + "         SaleTend.Sale_No = SaleHead.Sale_No     WHERE(SaleTend.TendName <> \'Store Credit\' OR SaleTend.AmtTend > 0) AND " + "         SaleHead.T_Type NOT IN (\'VOID\',\'CANCEL\') AND " + whereClause + "Group By SaleTend.TendName ", DataSource.CSCTrans);
            foreach (DataRow tender in rsTenders.Rows)
            {
                saleTenders.Add(new SaleTendAmount
                {
                    Tender = CommonUtility.GetStringValue(tender["Tender"]),
                    Count = CommonUtility.GetIntergerValue(tender["Count"]),
                    Amount = CommonUtility.GetDecimalValue(tender["Amount"]),
                    Used = CommonUtility.GetDecimalValue(tender["Used"])
                });
            }
            return saleTenders;
        }

        /// <summary>
        /// Method to get bonus give away
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>Bonus give away</returns>
        public decimal GetBonusGiveAwayForTrans(string whereClause)
        {
            var rsbonus = GetRecords("Select   sum(DiscountAmount) as [Bonus]  FROM     DiscountTender  WHERE   DiscountType = \'B\' and " + whereClause, DataSource.CSCTrans);
            if (rsbonus.Rows.Count != 0)
            {

                return CommonUtility.GetDecimalValue(rsbonus.Rows[0]["Bonus"]);
            }
            return 0;
        }

        /// <summary>
        /// Method to get bonus drop
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="cbt">Cash bonus tender</param>
        /// <returns>Bonus drop</returns>
        public decimal GetBonusDropForTrans(string whereClause, string cbt)
        {
            var rsbonus = GetRecords("Select   DropLines.Tender_Name, " + "         Sum(Amount) as [Drop_Amt], " + "         Sum(Conv_Amount) as [Drop_Conv]  FROM     DropLines  WHERE    Tender_name = \'" + cbt + "\' and " + whereClause + "Group By DropLines.Tender_Name ", DataSource.CSCTrans);
            if (rsbonus.Rows.Count != 0)
            {

                return CommonUtility.GetDecimalValue(rsbonus.Rows[0]["Drop_Amt"]);
            }
            return 0;
        }

        /// <summary>
        /// Method to get drop lines for till
        /// </summary>
        /// <param name="shiftDate">Shift date</param>
        /// <returns>List of drop lines</returns>
        public List<DropLine> GetDropLinesForTrans(DateTime shiftDate)
        {
            var dropLines = new List<DropLine>();
            var rsDrop = GetRecords("Select   DropLines.Tender_Name, Sum(Amount) as [Drop_Amt], "
                + "         Sum(Conv_Amount) as [Drop_Conv]  FROM     DropLines  WHERE  DropDate=\'" + shiftDate.ToString("yyyyMMdd") + "\' "
                + "Group By DropLines.Tender_Name ", DataSource.CSCTrans);

            foreach (DataRow dropLine in rsDrop.Rows)
            {
                dropLines.Add(new DropLine
                {
                    TenderName = CommonUtility.GetStringValue(dropLine["Tender_Name"]),
                    Amount = CommonUtility.GetDecimalValue(dropLine["Drop_Amt"]),
                    ConvAmount = CommonUtility.GetDecimalValue(dropLine["Drop_Conv"])
                });
            }
            return dropLines;
        }

        /// <summary>
        /// method to get the card number by by its tender name 
        /// </summary>
        /// <param name="tenderName"></param>
        /// <returns></returns>
        public List<string> GetCardNumbersByTenderName(string tenderName)
        {
            var rsCardNumber = GetRecords("Select CCARD_NUM from SaleTend where TENDNAME = '" + tenderName + "'", DataSource.CSCTills);
            var cardNumbers = new List<string>();

            foreach (DataRow row in rsCardNumber.Rows)
            {
                cardNumbers.Add(CommonUtility.GetStringValue(row["CCARD_NUM"]));
            }

            return cardNumbers;
        }


    }
}

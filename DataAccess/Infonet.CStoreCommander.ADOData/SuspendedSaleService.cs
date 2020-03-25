using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Infonet.CStoreCommander.ADOData
{
    public class SuspendedSaleService : SqlDbService, ISuspendedSaleService
    {
        private SqlConnection _connection;
        private SqlDataAdapter _adapter;

        private readonly ISaleService _saleService;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        public SuspendedSaleService(ISaleService saleService)
        {
            _saleService = saleService;
        }


        /// <summary>
        /// Get All Suspended Sales
        /// </summary>
        /// <param name="sqlQuery">Query</param>
        /// <returns>List of suspended sales</returns>
        public List<SusHead> GetAllSuspendedSale(string sqlQuery)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SuspendedSaleService,GetAllSuspendedSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var susHeadRs = GetRecords(sqlQuery, DataSource.CSCTills);
            var susHead = new List<SusHead>();
            foreach (DataRow row in susHeadRs.Rows)
            {
                susHead.Add(new SusHead
                {
                    SaleNumber = CommonUtility.GetIntergerValue(row["SALE_NO"]),
                    Client = CommonUtility.GetStringValue(row["CLIENT"]),
                    TillNumber = CommonUtility.GetShortValue(row["TILL"])
                });
            }
            _performancelog.Debug($"End,SuspendedSaleService,GetAllSuspendedSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return susHead;
        }


        /// <summary>
        /// Method to suspend a sale
        /// </summary>
        /// <param name="saleType">Sale type</param>
        /// <param name="shareSusp">Share suspended sale or not</param>
        /// <param name="sale">Sale</param>
        /// <param name="userCode">User code</param>
        public void SuspendSale(string saleType, bool shareSusp, Sale sale, string userCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SuspendedSaleService,SuspendSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var uHead = new DataTable();
            var uLine = new DataTable();
            var uReason = new DataTable();
            SqlDataAdapter adapHead;
            SqlDataAdapter adapLine;
            SqlDataAdapter adapReason;
            var tillConnection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (tillConnection.State == ConnectionState.Closed)
            {
                tillConnection.Open();
            }

            if (shareSusp)
            {
                adapHead = new SqlDataAdapter("select * from SusHead", tillConnection);
                adapHead.Fill(uHead);
                adapLine = new SqlDataAdapter("select * from SusLine", tillConnection);
                adapLine.Fill(uLine);
                adapReason = new SqlDataAdapter("select * from SusReas", tillConnection);
                adapReason.Fill(uReason);
            }
            else
            {
                adapHead = new SqlDataAdapter("select * from SusHead where SusHead.TILL=" + sale.TillNumber, tillConnection);
                adapHead.Fill(uHead);
                adapLine = new SqlDataAdapter("select * from SusLine where SusLine.TILL_NUM=" + sale.TillNumber, tillConnection);
                adapLine.Fill(uLine);
                adapReason = new SqlDataAdapter("select * from SusReas where SusReas.TILL_NUM=" + sale.TillNumber, tillConnection);
                adapReason.Fill(uReason);
            }

            var sTmpCardSales = GetRecords("select * from CardSales where TILL_NUM=" + sale.TillNumber + " AND SALE_NO = " + sale.Sale_Num, DataSource.CSCCurSale);
            var sCardSales = new DataTable();
            _adapter = new SqlDataAdapter("select * from CardSales where TILL_NUM=" + sale.TillNumber, tillConnection);
            _adapter.Fill(sCardSales);
            foreach (DataRow dataRow in sTmpCardSales.Rows)
            {
                var newRow = sCardSales.NewRow();
                newRow["TILL_NUM"] = sale.TillNumber;
                newRow["SALE_NO"] = dataRow["SALE_NO"];
                newRow["LINE_NUM"] = dataRow["LINE_NUM"];
                newRow["CardType"] = dataRow["CardType"];
                newRow["CardNum"] = dataRow["CardNum"];
                newRow["CardBalance"] = dataRow["CardBalance"];
                newRow["PointBalance"] = dataRow["PointBalance"];
                newRow["SaleAmount"] = dataRow["SaleAmount"];
                newRow["SaleType"] = dataRow["SaleType"];
                newRow["ExpDate"] = dataRow["ExpDate"];
                newRow["RefNum"] = dataRow["RefNum"];
                newRow["Response"] = dataRow["Response"];
                sCardSales.Rows.Add(newRow);

            }
            var builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(sCardSales);
            _adapter?.Dispose();

            DataRow uRow = uHead.NewRow();
            uRow["SALE_NO"] = sale.Sale_Num;
            uRow["Till"] = sale.TillNumber;
            uRow["Regist"] = sale.Register;
            uRow["User"] = userCode;
            uRow["Client"] = sale.Customer.Code;
            uRow["T_type"] = sale.Sale_Type; // Nicolette added
            uRow["Reason"] = sale.Return_Reason.Reason;
            uRow["Reason_Type"] = sale.Return_Reason.RType;
            uRow["Disc_Type"] = sale.Sale_Totals.Invoice_Discount_Type;
            uRow["Invc_Disc"] = sale.Sale_Totals.Invoice_Discount;
            uRow["Discount_Percent"] = sale.Sale_Totals.Discount_Percent;
            uRow["Void_No"] = sale.Void_Num;

            uRow["LoyaltyCard"] = sale.Customer.LoyaltyCard;
            uRow["LoyaltyExpDate"] = sale.Customer.LoyaltyExpDate;
            if (!string.IsNullOrEmpty(sale.CouponID))
            {
                if (sale.CouponTotal < 0)
                {
                    uRow["CouponID"] = sale.CouponID;
                }
            }

            uRow["Upsell"] = sale.Upsell;
            uHead.Rows.Add(uRow);
            builder = new SqlCommandBuilder(adapHead);
            adapHead.InsertCommand = builder.GetInsertCommand();
            adapHead.Update(uHead);
            adapHead.Dispose();

            var ln = (short)0;
            foreach (Sale_Line tempLoopVarSl in sale.Sale_Lines)
            {
                var sl = tempLoopVarSl;
                ln++;
                var uLineRow = uLine.NewRow();
                uLineRow["sale_no"] = sale.Sale_Num;
                uLineRow["Line_Num"] = ln;
                uLineRow["PLU_Code"] = sl.PLU_Code;
                uLineRow["Quantity"] = sl.Quantity;
                uLineRow["price"] = sl.price;
                uLineRow["Discount"] = sl.Line_Discount;
                uLineRow["Disc_Type"] = sl.Discount_Type;
                uLineRow["Disc_Adj"] = sl.Discount_Adjust;
                uLineRow["Disc_Code"] = sl.Discount_Code;
                uLineRow["Disc_Rate"] = sl.Discount_Rate;
                uLineRow["DiscountName"] = sl.DiscountName; // 
                uLineRow["Loyl_Save"] = sl.Loyalty_Save;
                uLineRow["Units"] = sl.Units;
                uLineRow["Serial_No"] = sl.Serial_No;
                uLineRow["User"] = sl.User;
                uLineRow["pumpID"] = sl.pumpID;
                uLineRow["PositionID"] = sl.PositionID;
                uLineRow["GradeID"] = sl.GradeID;
                uLineRow["Gift_Cert"] = sl.Gift_Certificate;
                uLineRow["GC_Num"] = sl.Gift_Num;
                uLineRow["Till_Num"] = sale.TillNumber;
                uLineRow["ManualFuel"] = sl.ManualFuel;
                uLineRow["TaxExempt"] = sl.IsTaxExemptItem;
                uLineRow["Amount"] = sl.Amount;

                if (sl.PromoID != "")
                {
                    uLineRow["PromoID"] = sl.PromoID;
                }

                uLineRow["Upsell"] = sl.Upsell;

                uLineRow["ScalableItem"] = sl.ScalableItem;
                uLine.Rows.Add(uLineRow);

                foreach (Return_Reason tempLoopVarRr in sl.Return_Reasons)
                {
                    var reasonRow = uReason.NewRow();
                    reasonRow["Till_Num"] = sale.TillNumber;
                    reasonRow["sale_no"] = sale.Sale_Num;
                    reasonRow["Line_Num"] = ln;
                    reasonRow["Reason"] = tempLoopVarRr.Reason;
                    reasonRow["Reason_Type"] = tempLoopVarRr.RType;
                    uReason.Rows.Add(reasonRow);
                    builder = new SqlCommandBuilder(adapReason);
                    adapReason.InsertCommand = builder.GetInsertCommand();
                    adapReason.Update(uReason);
                }
                adapReason.Dispose();
                builder = new SqlCommandBuilder(adapLine);
                adapLine.InsertCommand = builder.GetInsertCommand();
                adapLine.Update(uLine);
            }

            adapLine.Dispose();
            tillConnection.Close();
            Execute("Delete from CurrentSale Where SaleNumber = " + Convert.ToString(sale.Sale_Num), DataSource.CSCCurSale);
            Execute("Delete FROM CardSales WHERE CardSales.SALE_NO = " + Convert.ToString(sale.Sale_Num), DataSource.CSCCurSale);
            _performancelog.Debug($"End,SuspendedSaleService,SuspendSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }

        /// <summary>
        /// Method to update cash for unsuspend sale
        /// </summary>
        /// <param name="shareSusp">Share suspended sale</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        public void UpdateCardSaleForUnSuspend(bool shareSusp, int tillNumber, int saleNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SuspendedSaleService,UpdateCardSaleForUnSuspend,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            DataTable sCardSales = new DataTable();
            string query;

            if (shareSusp)
            {
                query = "select * from CardSales where SALE_NO=" + Convert.ToString(saleNumber);
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _adapter = new SqlDataAdapter(query, _connection);
                _adapter.Fill(sCardSales);
            }
            else
            {
                query = "select * from CardSales where TILL_NUM=" + tillNumber + " AND SALE_NO=" + Convert.ToString(saleNumber);
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _adapter = new SqlDataAdapter(query, _connection);
                _adapter.Fill(sCardSales);
            }

            var sTmpCardSales = GetRecords("select * from CardSales", DataSource.CSCCurSale);
            foreach (DataRow row in sCardSales.Rows)
            {
                DataRow newRow = sTmpCardSales.NewRow();
                var colNames = sCardSales.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                foreach (string tempLoopVarFld in colNames)
                {
                    var fld = tempLoopVarFld;
                    newRow[fld] = row[fld];
                }
                sTmpCardSales.Rows.Add(newRow);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(sTmpCardSales);
                _connection.Close();
                _adapter?.Dispose();
            }

            if (shareSusp)
            {
                Execute("Delete  FROM CardSales   WHERE CardSales.SALE_NO = " + Convert.ToString(saleNumber), DataSource.CSCTills);
            }
            else
            {
                Execute("Delete  FROM CardSales   WHERE TILL_NUM=" + tillNumber + " AND SALE_NO = " + Convert.ToString(saleNumber), DataSource.CSCTills);
            }
            _performancelog.Debug($"End,SuspendedSaleService,UpdateCardSaleForUnSuspend,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to get suspended sale
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="shareSusp">Share suspended sale</param>
        /// <returns>Sale</returns>
        public Sale GetSuspendedSale(int tillNumber, int saleNumber, bool shareSusp)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SuspendedSaleService,GetSuspendedSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var sale = new Sale();
            DataTable uHead;
            DataTable uLine;

            if (shareSusp)
            {
                uHead = GetRecords("select * from SusHead  WHERE SALE_NO=" + Convert.ToString(saleNumber), DataSource.CSCTills);
                uLine = GetRecords("select * from SusLine WHERE SALE_NO=" + Convert.ToString(saleNumber) + " ORDER BY SusLine.Line_Num ", DataSource.CSCTills);
            }
            else
            {
                uHead = GetRecords("select * from SusHead where TILL=" + tillNumber + " AND SALE_NO=" + Convert.ToString(saleNumber), DataSource.CSCTills);
                uLine = GetRecords("select * from SusLine where TILL_NUM=" + tillNumber + " AND SALE_NO=" + Convert.ToString(saleNumber) + " ORDER BY SusLine.Line_Num ", DataSource.CSCTills);
            }

            if (uHead.Rows.Count > 0)
            {
                sale.Sale_Num = CommonUtility.GetIntergerValue(uHead.Rows[0]["SALE_NO"]);
                sale.TillNumber = (byte)tillNumber;
                sale.Register = CommonUtility.GetByteValue(uHead.Rows[0]["Regist"]);
                sale.Customer.Code = CommonUtility.GetStringValue(uHead.Rows[0]["Client"]);
                sale.Sale_Type = CommonUtility.GetStringValue(uHead.Rows[0]["T_type"]);
                sale.Return_Reason.Reason = CommonUtility.GetStringValue(uHead.Rows[0]["Reason"]);
                sale.Return_Reason.RType = CommonUtility.GetStringValue(uHead.Rows[0]["Reason_Type"]);
                sale.Sale_Totals.Invoice_Discount_Type = CommonUtility.GetStringValue(uHead.Rows[0]["Disc_Type"]);
                sale.Sale_Totals.Invoice_Discount = CommonUtility.GetDecimalValue(uHead.Rows[0]["Invc_Disc"]);
                sale.Sale_Totals.Discount_Percent = CommonUtility.GetFloatValue(uHead.Rows[0]["Discount_Percent"]);
                sale.Void_Num = CommonUtility.GetIntergerValue(uHead.Rows[0]["Void_No"]);

                sale.Customer.LoyaltyCard = CommonUtility.GetStringValue(uHead.Rows[0]["LoyaltyCard"]);
                sale.Customer.LoyaltyExpDate = CommonUtility.GetStringValue(uHead.Rows[0]["LoyaltyExpDate"]);
                sale.CouponID = CommonUtility.GetStringValue(uHead.Rows[0]["CouponID"]);
                sale.Upsell = CommonUtility.GetBooleanValue(uHead.Rows[0]["Upsell"]);
                var ln = (short)0;

                foreach (DataRow row in uLine.Rows)
                {
                    ln++;
                    var sl = new Sale_Line
                    {
                        Sale_Num = CommonUtility.GetStringValue(row["sale_no"]),
                        Till_Num = sale.TillNumber
                    };
                    sale.Sale_Num = CommonUtility.GetIntergerValue(row["sale_no"]);
                    sl.Line_Num = ln;
                    sl.PLU_Code = CommonUtility.GetStringValue(row["PLU_Code"]);
                    sl.Quantity = CommonUtility.GetFloatValue(row["Quantity"]);
                    sl.price = CommonUtility.GetDoubleValue(row["price"]);
                    sl.Amount = CommonUtility.GetDecimalValue(row["Amount"]);
                    sl.Line_Discount = CommonUtility.GetDoubleValue(row["Discount"]);
                    sl.Discount_Type = CommonUtility.GetStringValue(row["Disc_Type"]);
                    sl.Discount_Adjust = CommonUtility.GetDoubleValue(row["Disc_Adj"]);
                    sl.Discount_Code = CommonUtility.GetStringValue(row["Disc_Code"]);
                    sl.Discount_Rate = CommonUtility.GetFloatValue(row["Disc_Rate"]);
                    sl.DiscountName = CommonUtility.GetStringValue(row["DiscountName"]);
                    sl.Price_Type = CommonUtility.GetCharValue(row["Price_Type"]);
                    sl.Loyalty_Save = CommonUtility.GetFloatValue(row["Loyl_Save"]);
                    sl.Units = CommonUtility.GetStringValue(row["Units"]);
                    sl.Serial_No = CommonUtility.GetStringValue(row["Serial_No"]);
                    sl.User = CommonUtility.GetStringValue(row["User"]);
                    sl.pumpID = CommonUtility.GetByteValue(row["pumpID"]);
                    sl.PositionID = CommonUtility.GetByteValue(row["PositionID"]);
                    sl.GradeID = CommonUtility.GetByteValue(row["GradeID"]);
                    sl.Gift_Certificate = CommonUtility.GetBooleanValue(row["Gift_Cert"]);
                    sl.Gift_Num = CommonUtility.GetStringValue(row["GC_Num"]);
                    sale.TillNumber = (byte)tillNumber;
                    sl.ManualFuel = CommonUtility.GetBooleanValue(row["ManualFuel"]);

                    sl.IsTaxExemptItem = CommonUtility.GetBooleanValue(row["TaxExempt"]);

                    var strPromo = CommonUtility.GetStringValue(row["PromoID"]);
                    if (!string.IsNullOrEmpty(strPromo))
                    {
                        sl.PromoID = strPromo;
                    }
                    sl.Upsell = CommonUtility.GetBooleanValue(row["Upsell"]);
                    sl.ScalableItem = CommonUtility.GetBooleanValue(row["ScalableItem"]);
                    DataTable uReason;
                    if (shareSusp)
                    {
                        uReason =
                            GetRecords(
                                "select * from SusReas  WHERE SALE_NO=" + Convert.ToString(saleNumber) +
                                " AND LINE_Num = " + ln + " ORDER BY SusReas.Line_Num", DataSource.CSCTills);
                    }
                    else
                    {
                        uReason =
                            GetRecords(
                                "select * from SusReas where TILL_NUM=" + tillNumber + " AND SALE_NO=" +
                                Convert.ToString(saleNumber) + " AND LINE_Num = " + ln +
                                " ORDER BY SusReas.Line_Num", DataSource.CSCTills);
                    }
                    var returnReason = new Return_Reason();

                    foreach (DataRow dataRow in uReason.Rows)
                    {
                        if (CommonUtility.GetStringValue(dataRow["Reason"]) != "" &
                            CommonUtility.GetStringValue(dataRow["Reason_Type"]) != "")
                        {
                            returnReason.Reason = CommonUtility.GetStringValue(dataRow["Reason"]);
                            returnReason.RType = CommonUtility.GetStringValue(dataRow["Reason_Type"]);
                        }
                        sl.Return_Reasons.Add(returnReason.Reason, returnReason.RType, ln.ToString());
                    }
                    sale.Sale_Lines.AddLine(ln, sl, "");
                }
                _performancelog.Debug($"End,SuspendedSaleService,GetSuspendedSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return sale;
            }
            _performancelog.Debug($"End,SuspendedSaleService,GetSuspendedSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return new Sale();
        }

        /// <summary>
        /// Delete Un Suspend
        /// </summary>
        /// <param name="shareSusp"></param>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        public void DeleteUnsuspend(bool shareSusp, int tillNumber, int saleNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SuspendedSaleService,DeleteUnsuspend,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            if (shareSusp)
            {
                Execute("Delete  FROM SusLine   WHERE SusLine.Sale_No   = " + Convert.ToString(saleNumber), DataSource.CSCTills);
                Execute("Delete  FROM SusHead   WHERE SusHead.Sale_No   = " + Convert.ToString(saleNumber), DataSource.CSCTills);
                Execute("Delete  FROM SusReas   WHERE SusReas.Sale_No = " + Convert.ToString(saleNumber), DataSource.CSCTills);
                Execute("Delete  FROM CardSales   WHERE CardSales.SALE_NO = " + Convert.ToString(saleNumber), DataSource.CSCTills);
            }
            else
            {
                Execute("Delete  FROM SusLine   WHERE SusLine.Sale_No   = " + Convert.ToString(saleNumber) + " AND SusLine.TILL_NUM=" + tillNumber, DataSource.CSCTills);
                Execute("Delete  FROM SusHead   WHERE SusHead.Sale_No   = " + Convert.ToString(saleNumber) + " AND SusHead.TILL=" + tillNumber, DataSource.CSCTills);
                Execute("Delete  FROM SusReas   WHERE SusReas.Sale_No = " + Convert.ToString(saleNumber) + " AND SusReas.TILL_NUM=" + tillNumber, DataSource.CSCTills);
                Execute("Delete  FROM CardSales   WHERE CardSales.SALE_NO = " + Convert.ToString(saleNumber) + " AND CardSales.TILL_NUM=" + tillNumber, DataSource.CSCTills);
            }
            _performancelog.Debug($"End,SuspendedSaleService,DeleteUnsuspend,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }


        /// <summary>
        /// Delete Card Sale From DB Temp
        /// </summary>
        /// <param name="saleNumber"></param>
        public void DeleteCardSaleFromDbTemp(int saleNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SuspendedSaleService,DeleteCardSaleFromDbTemp,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            Execute("Delete  FROM CardSales   WHERE CardSales.SALE_NO = " + Convert.ToString(saleNumber), DataSource.CSCCurSale);
            _performancelog.Debug($"End,SuspendedSaleService,DeleteCardSaleFromDbTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to remove previous transaction from CSCCurSale
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>True or false</returns>
        public bool RemovePreviousTransactionFromDbTemp(int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SuspendedSaleService,RemovePreviousTransactionFromDbTemp,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var sale = _saleService.GetSaleByTillNumber(tillNumber);
            if (sale != null)
            {
                // Remove the sale from the temporary files.
                Execute("Delete from CurrentSale  Where TillNumber = " + Convert.ToString(tillNumber),
                    DataSource.CSCCurSale);
                Execute("Delete FROM CardSales WHERE CardSales.TILL_NUM = " + Convert.ToString(tillNumber),
                    DataSource.CSCCurSale);
                _performancelog.Debug($"End,SuspendedSaleService,RemovePreviousTransactionFromDbTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            }
            return true;
        }
    }
}

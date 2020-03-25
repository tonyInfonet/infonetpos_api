using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// Sale Service 
    /// </summary>
    public class SaleService : SqlDbService, ISaleService
    {
        protected readonly ILog Performancelog = LoggerManager.PerformanceLogger;
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;


        /// <summary>
        /// Checks Existing Sale in DB
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>True or false</returns>
        public bool ExistingSaleInDbTemp(int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,ExistingSaleInDbTemp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var flag = false;
            var saleLineRecordSet = GetRecords("SELECT Till_Num  FROM   SaleLine where Till_Num =" + tillNumber, DataSource.CSCCurSale);
            if (saleLineRecordSet.Rows.Count != 0)
            {
                flag = true;
            }
            Performancelog.Debug($"End,SaleService,ExistingSaleInDbTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return flag;
        }

        /// <summary>
        /// Remove Temp Data
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        public void RemoveTempDataInDbTill(int tillNumber, int saleNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,RemoveTempDataInDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            // Remove the sale from the temporary files.
            Execute("Delete  from CurrentSale  Where SaleNumber = " + saleNumber + " and TillNumber = " + tillNumber, DataSource.CSCCurSale);
            Execute("Delete  from CardSales Where CardSales.SALE_NO = " + saleNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCCurSale);
            Execute("delete  from saletend where sale_no =" + saleNumber + " AND Till_Num=" + tillNumber, DataSource.CSCCurSale);
            Execute("delete  from cardtenders where sale_no =" + saleNumber + " AND Till_Num=" + tillNumber, DataSource.CSCCurSale);
            Execute("delete  from USEDSC where sale_no =" + saleNumber + " AND Till_Num=" + tillNumber, DataSource.CSCCurSale);
            Execute("delete  from USEDGC where sale_no =" + saleNumber + " AND Till_Num=" + tillNumber, DataSource.CSCCurSale);
            Execute("DELETE  From SaleVendorCoupon WHERE Sale_No = " + saleNumber + " AND TILL_NUM=" + tillNumber, DataSource.CSCCurSale);
            Performancelog.Debug($"End,SaleService,RemoveTempDataInDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }


        /// <summary>
        /// Get tax exempt message
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public List<string> GetTaxExemptMessagesFromDbTrans(string messageType)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetTaxExemptMessagesFromDbTrans,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var globalMessages = new List<string>();
            var rsTeMessages = GetRecords("SELECT GlobalMessage FROM TaxExemptMessages where messagetype = '" + messageType + "' and ReviewStatus = 0", DataSource.CSCTrans);
            foreach (DataRow fields in rsTeMessages.Rows)
            {
                globalMessages.Add(CommonUtility.GetStringValue(fields["GlobalMessage"]));
            }
            Performancelog.Debug($"End,SaleService,GetTaxExemptMessagesFromDbTrans,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return globalMessages;
        }

        /// <summary>
        /// Update tax exempt message
        /// </summary>
        public void UpdateTaxExemptMessagesToDbTrans()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,UpdateTaxExemptMessagesToDbTrans,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            Execute("UPDATE TaxExemptMessages SET ReviewStatus= 1 WHERE Messagetype = \'SM\' and reviewstatus = 0", DataSource.CSCTrans);
            Performancelog.Debug($"End,SaleService,UpdateTaxExemptMessagesToDbTrans,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Check payment by saleno and shift number
        /// </summary>
        /// <param name="saleNo"></param>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        public bool CheckPaymentsFromDbTemp(int saleNo, int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,CheckPaymentsFromDbTemp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var rs = GetRecords("select sale_no from saletend where sale_no =" + Convert.ToString(saleNo) + " and Till_Num=" + Convert.ToString(tillNumber), DataSource.CSCCurSale);
            var result = rs.Rows.Count != 0;
            Performancelog.Debug($"End,SaleService,CheckPaymentsFromDbTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return result;
        }

        /// <summary>
        /// Get the max Sale number for sale head
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <returns>Sale number</returns>
        public int GetMaxSaleNoFromSaleHeadFromDbTill(int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetMaxSaleNoFromSaleHeadFromDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var saleNumber = GetRecords("select MAX(SALE_NO) as SL from SALEHEAD WHERE TILL = " + Convert.ToString(tillNumber), DataSource.CSCTills);
            var fields = saleNumber.Rows[0];
            int returnValue = DBNull.Value.Equals(fields["SL"]) ? 1 : Convert.ToInt32(CommonUtility.GetIntergerValue(fields["SL"]) + 1);
            Performancelog.Debug($"End,SaleService,GetMaxSaleNoFromSaleHeadFromDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Get the maximum sale no from Suspended head
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <returns>Sale number</returns>
        public int GetMaxSaleNoFromSusHeadFromDbTill(int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetMaxSaleNoFromSusHeadFromDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var returnValue = 0;
            var saleNumber = GetRecords("select MAX(SALE_NO) as SL from SusHead WHERE TILL = " + Convert.ToString(tillNumber), DataSource.CSCTills);
            var fields = saleNumber.Rows[0];
            if (DBNull.Value.Equals(fields["SL"]))
            {
            }
            else
            {
                returnValue = CommonUtility.GetIntergerValue(fields["SL"]);
            }
            Performancelog.Debug($"End,SaleService,GetMaxSaleNoFromSusHeadFromDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Get the max sale number from Sale number
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="messageNumber"></param>
        /// <returns>Sale number</returns>
        public int GetMaxSaleNoFromSaleNumbFromDbAdmin(int tillNumber, out int messageNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetMaxSaleNoFromSaleNumbFromDbAdmin,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            int returnValue;
            var invLeft = 0;
            messageNumber = 0;
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCAdmin));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT Sale_No FROM SALENUMB", _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                var fields = _dataTable.NewRow();
                returnValue = 1;
                fields["sale_no"] = 2;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
            }
            else
            {

                returnValue = Convert.ToInt32(_dataTable.Rows[0]["sale_no"]);
                if (Convert.ToDouble(Convert.ToInt32(_dataTable.Rows[0]["sale_no"]) + 1) >= Math.Pow(2, 31) - 1 - invLeft)
                {
                    messageNumber = 8340;
                }

                Execute("Update SALENUMB SET Sale_No=" + (returnValue + 1), DataSource.CSCAdmin);

            }

            Performancelog.Debug($"End,SaleService,GetMaxSaleNoFromSaleNumbFromDbAdmin,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        ///Method to update sale non resettable grant total
        /// </summary>
        /// <param name="tillNum">TIll number</param>
        /// <param name="sDate">Sale date</param>
        /// <param name="value">total</param>
        public void Update_Sale_NoneResettableGrantTotal(short tillNum, DateTime sDate, decimal value)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,Update_Sale_NoneResettableGrantTotal,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var sSql = "UPDATE NRGT SET ShiftDate=\'" + sDate.ToString("yyyyMMdd") + "\', Value=VAlue + " + Convert.ToString(value, CultureInfo.InvariantCulture) + " WHERE Till_Num=" + Convert.ToString(tillNum);
            Execute(sSql, DataSource.CSCTills);

            if (sDate < DateAndTime.Today)
            {
                Update_TillClose_NoneResettableGrantTotal(tillNum, sDate);
            }
            Performancelog.Debug($"End,SaleService,Update_Sale_NoneResettableGrantTotal,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to update till close non resettable grant total
        /// </summary>
        /// <param name="tillNum">Till number</param>
        /// <param name="sDate">Sale date</param>
        public void Update_TillClose_NoneResettableGrantTotal(short tillNum, DateTime sDate)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,Update_TillClose_NoneResettableGrantTotal,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            string trainFirstTill = "91";
            string trainLastTill = "99";
            string sSql;
            decimal value;
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));

            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from NRGT where TILL_NUM=" + Convert.ToString(tillNum), _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                var fields = _dataTable.Rows[0];
                value = Convert.ToDecimal(fields["Value"]);
                fields["Value"] = 0;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
            }
            else
            {
                return;
            }

            var sNrgt = GetRecords("SELECT * from NRGT where ShiftDate= \'" + sDate.ToString("yyyyMMdd") + "\'", DataSource.CSCTrans);
            if (sNrgt.Rows.Count > 0)
            {
                if (tillNum >= double.Parse(trainFirstTill) && tillNum <= double.Parse(trainLastTill))
                {
                    sSql = "UPDATE NRGT SET TrainValue=TrainValue + " + Convert.ToString(value, CultureInfo.InvariantCulture) + " WHERE ShiftDate=\'" + sDate.ToString("yyyyMMdd") + "\'";

                }
                else
                {
                    sSql = "UPDATE NRGT SET Value=VAlue + " + Convert.ToString(value, CultureInfo.InvariantCulture) + " WHERE ShiftDate=\'" + sDate.ToString("yyyyMMdd") + "\'";
                }
            }
            else
            {
                if (tillNum >= double.Parse(trainFirstTill) && tillNum <= double.Parse(trainLastTill))
                {
                    sSql = "INSERT INTO NRGT (ShiftDate, Value, TrainValue ) VALUES ( \'" + sDate.ToString("yyyyMMdd") + "\'," + Convert.ToString(0) + "," + Convert.ToString(value, CultureInfo.InvariantCulture) + ")";
                }
                else
                {
                    sSql = "INSERT INTO NRGT (ShiftDate, Value, TrainValue ) VALUES  ( \'" + sDate.ToString("yyyyMMdd") + "\'," + Convert.ToString(value, CultureInfo.InvariantCulture) + "," + Convert.ToString(0) + ")";
                }
            }

            Execute(sSql, DataSource.CSCTrans);
            Performancelog.Debug($"End,SaleService,Update_TillClose_NoneResettableGrantTotal,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Get the max sale number from Sale number
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="messageNumber"></param>
        /// <returns>Sale number</returns>
        public int GetMaxSaleNo(int tillNumber, out int messageNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetMaxSaleNo,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            int returnValue;
            var invLeft = 0;
            messageNumber = 0;
            var saleNumber = GetRecords("SELECT Sale_No FROM SALENUMB", DataSource.CSCAdmin);
            if (saleNumber == null || saleNumber.Rows.Count == 0)
            {
                returnValue = 1;
            }
            else
            {

                var fields = saleNumber.Rows[0];

                returnValue = Convert.ToInt32(fields["sale_no"]);
                if (Convert.ToDouble(fields["sale_no"]) >= Math.Pow(2, 31) - 1 - invLeft)
                {
                    messageNumber = 8340;
                }

            }


            Performancelog.Debug($"End,SaleService,GetMaxSaleNo,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }


        /// <summary>
        /// Method to get sale number
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="user">User</param>
        /// <param name="messageNumber">Message number</param>
        /// <returns>Sale number</returns>
        public int GetSaleNo(int tillNumber, User user, out int messageNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetSaleNo,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            int returnValue = 0;
            messageNumber = 0;
            DataTable sn;
            int invLeft = 0;

            try
            {
                if (user.User_Group.Code == "Trainer")
                {
                    sn = GetRecords("select MAX(SALE_NO) as SL from SALEHEAD WHERE TILL = " + Convert.ToString(tillNumber),
                        DataSource.CSCTills);
                    var fields = sn.Rows[0];
                    if (DBNull.Value.Equals(fields["SL"]))
                    {
                        returnValue = 1;
                    }
                    else
                    {
                        returnValue = CommonUtility.GetIntergerValue(fields["SL"]) + 1;
                    }
                    sn = GetRecords("select MAX(SALE_NO) as SL from SusHead WHERE TILL = " + Convert.ToString(tillNumber),
                        DataSource.CSCTills);
                    fields = sn.Rows[0];
                    if (DBNull.Value.Equals(fields["SL"]))
                    {
                    }
                    else if (CommonUtility.GetIntergerValue(fields["SL"]) >= returnValue)
                    {
                        returnValue = CommonUtility.GetIntergerValue(fields["SL"]) + 1;
                    }
                    return returnValue;
                }
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCAdmin));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter("SELECT Sale_No FROM SALENUMB", _connection);
                _adapter.Fill(_dataTable);
                if (_dataTable.Rows.Count == 0)
                {
                    returnValue = 1;
                    var snFields = _dataTable.NewRow();
                    snFields["sale_no"] = 2;
                    _dataTable.Rows.Add(snFields);
                }
                else
                {

                    returnValue = CommonUtility.GetIntergerValue(_dataTable.Rows[0]["sale_no"]);
                    _dataTable.Rows[0]["sale_no"] = CommonUtility.GetDoubleValue(_dataTable.Rows[0]["sale_no"]) + 1;
                    if (CommonUtility.GetDoubleValue(_dataTable.Rows[0]["sale_no"]) >= Math.Pow(2, 31) - 1 - invLeft)
                    {
                        messageNumber = 8340;
                    }

                }
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();

                if (returnValue == 0)
                {
                    messageNumber = 8341;
                    return returnValue;
                }
            }
            catch
            {
                messageNumber = 8341;
            }
            Performancelog.Debug($"End,SaleService,GetSaleNo,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Method to update discount tender
        /// </summary>
        /// <param name="sale">Sale</param>
        public void UpdateDiscountTender(ref Sale sale)
        {
            var rsDiscountTender = GetRecords("select * from DiscountTender where SALE_NO=" + Convert.ToString(sale.Sale_Num) + " AND TILL_NUM=" + sale.TillNumber, DataSource.CSCTills);
            if (rsDiscountTender.Rows.Count > 0)
            {

                sale.Customer.LoyaltyCard = CommonUtility.GetStringValue(rsDiscountTender.Rows[0]["CardNum"]);

                sale.CouponID = CommonUtility.GetStringValue(rsDiscountTender.Rows[0]["CouponID"]);
            }
        }

        /// <summary>
        /// Get Sale by Sale number
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        /// <returns>Sale</returns>
        public Sale GetSaleBySaleNoFromDbTill(ref Sale sale, int tillNumber, int saleNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetSaleBySaleNoFromDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var rsHead = GetRecords("SELECT * FROM SaleHead WHERE SaleHead.Sale_No = " + Convert.ToString(saleNumber) + " AND   SaleHead.Till = " + tillNumber, DataSource.CSCTills);

            if (rsHead == null || rsHead.Rows.Count == 0)
            {
                return null;
            }
            var fields = rsHead.Rows[0];

            sale.TillNumber = CommonUtility.GetByteValue(fields["Till"]);
            sale.Customer.Code = CommonUtility.GetStringValue(fields["Client"]);

            sale.Sale_Type = CommonUtility.GetStringValue(fields["T_type"]); // Nicolette
            sale.Sale_Deposit = CommonUtility.GetDecimalValue(fields["Deposit"]); // Nicolette
            sale.Return_Reason.RType = CommonUtility.GetStringValue(fields["Reason_Type"]);
            sale.Return_Reason.Reason = CommonUtility.GetStringValue(fields["Reason"]);

            if (sale.CouponID == "")
            {
                //TODO:Check the reCompute-coupon method
            }

            sale.Upsell = Convert.ToBoolean(!DBNull.Value.Equals(fields["Upsell"]) ? false : fields["Upsell"]);

            sale.TreatyNumber = Convert.ToString(!DBNull.Value.Equals(fields["TreatyNumber"]) ? "" : fields["TreatyNumber"]);
            sale.Sale_Totals.Invoice_Discount_Type = CommonUtility.GetStringValue(fields["Disc_Type"]);
            Performancelog.Debug($"End,SaleService,GetSaleBySaleNoFromDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return sale;
        }

        /// <summary>
        /// Delete the signature
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        public void DeleteSignatureFromDbTill(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,DeleteSignatureFromDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            Execute("delete  from Signature where sale_no =" + Convert.ToString(saleNumber) + " AND Till_Num=" + tillNumber, DataSource.CSCTills);
            Performancelog.Debug($"End,SaleService,DeleteSignatureFromDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }



        /// <summary>
        /// Get the Sale lines
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <param name="userCode"></param>
        /// <returns>Sale Line</returns>
        public List<Sale_Line> GetSaleLinesFromDbTemp(int saleNumber, int tillNumber, string userCode)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetSaleLinesFromDbTemp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var saleLines = new List<Sale_Line>();

            var rsLine = GetRecords("SELECT *  FROM   SaleLine  WHERE  SaleLine.Sale_No = " + Convert.ToString(saleNumber) + " AND   SaleLine.Till_Num = " + tillNumber + " ORDER BY SaleLine.Line_Num ", DataSource.CSCCurSale);
            var rsReason = GetRecords("SELECT * FROM SLineReason  WHERE SLineReason.Sale_No = " + Convert.ToString(saleNumber) + " AND   SLineReason.Till_Num = " + tillNumber + " ORDER BY SLineReason.Line_Num", DataSource.CSCCurSale);
            short ln = 0;
            foreach (DataRow lineFields in rsLine.Rows)
            {
                ln++;

                var saleLine = new Sale_Line { PLU_Code = Convert.ToString(lineFields["PLU_Code"]) };
                if (saleLine.ProductIsFuel && !saleLine.IsPropane)
                {
                    saleLine.Regular_Price = Convert.ToDouble(lineFields["price"]);
                }
                saleLine.Line_Num = ln;
                saleLine.User = userCode.ToUpper();
                saleLine.Till_Num = (byte)tillNumber;
                saleLine.Sale_Num = saleNumber.ToString();
                saleLine.Quantity = Convert.ToSingle(lineFields["Quantity"]);
                saleLine.pumpID = Convert.ToByte(lineFields["pumpID"]); // Nicolette
                saleLine.PositionID = Convert.ToByte(lineFields["PositionID"]); // Nicolette
                saleLine.GradeID = Convert.ToByte(lineFields["GradeID"]); // Nicolette
                saleLine.Gift_Certificate = Convert.ToBoolean(lineFields["Gift_Cert"]);
                saleLine.Gift_Num = Convert.ToString(lineFields["GC_Num"]);
                saleLine.Serial_No = Convert.ToString(lineFields["Serial_No"]);
                saleLine.Stock_Code = CommonUtility.GetStringValue(lineFields["Stock_Code"]);
                saleLine.Prepay = Convert.ToBoolean(lineFields["Prepay"]);
                saleLine.Amount = Convert.ToDecimal(lineFields["Amount"]);
                saleLine.PositionID = Convert.ToByte(lineFields["PositionID"]);

                saleLine.ManualFuel = Convert.ToBoolean(lineFields["ManualFuel"]);

                saleLine.IsTaxExemptItem = Convert.ToBoolean(lineFields["TaxExempt"]);

                saleLine.PaidByCard =
                    Convert.ToByte(DBNull.Value.Equals(lineFields["PaidByCard"])
                        ? 0
                        : lineFields["PaidByCard"]);

                saleLine.Upsell =
                    Convert.ToBoolean(DBNull.Value.Equals(lineFields["Upsell"])
                        ? false
                        : lineFields["Upsell"]);

                saleLine.ScalableItem =
                    Convert.ToBoolean(DBNull.Value.Equals(lineFields["ScalableItem"])
                        ? false
                        : lineFields["ScalableItem"]);


                if (saleLine.Amount < 0)
                {
                    saleLine.No_Loading = true;
                }


                saleLine.No_Loading = true;
                saleLine.price =
                    Convert.ToDouble(DBNull.Value.Equals(lineFields["price"])
                        ? 0
                        : lineFields["price"]);
                saleLine.TEPrice =
                    Convert.ToDouble(DBNull.Value.Equals(lineFields["TEPrice"])
                        ? 0
                        : lineFields["TEPrice"]);

                if (saleLine.ScalableItem)
                {
                    saleLine.Regular_Price = saleLine.price;
                }

                saleLine.Quantity =
                    Convert.ToSingle(DBNull.Value.Equals(lineFields["Quantity"])
                        ? 0
                        : lineFields["Quantity"]);
                saleLine.Amount =
                    Convert.ToDecimal(DBNull.Value.Equals(lineFields["Amount"])
                        ? 0
                        : lineFields["Amount"]);
                saleLine.Total_Amount =
                    Convert.ToDecimal(-1 *
                                             Convert.ToInt32(
                                                 DBNull.Value.Equals(lineFields["TOTAL_AMT"])
                                                     ? 0
                                                     : lineFields["TOTAL_AMT"]));
                saleLine.Discount_Adjust = Convert.ToDouble(lineFields["Disc_Adj"]);
                saleLine.Line_Discount = Convert.ToDouble(lineFields["Discount"]);
                saleLine.Discount_Type = Convert.ToString(lineFields["Disc_Type"]);
                saleLine.Discount_Code =
                    Convert.ToString(DBNull.Value.Equals(lineFields["Disc_Code"])
                        ? ""
                        : lineFields["Disc_Code"]);
                saleLine.Discount_Rate = Convert.ToSingle(lineFields["Disc_Rate"]);
                saleLine.DiscountName =
                    Convert.ToString(DBNull.Value.Equals(lineFields["DiscountName"])
                        ? ""
                        : lineFields["DiscountName"]); // 
                saleLine.CardProfileID =
                    Convert.ToString(DBNull.Value.Equals(lineFields["CardProfileID"])
                        ? 0
                        : lineFields["CardProfileID"]); // 
                saleLine.No_Loading = false;
                // It has to be set back to false, otherwise if the user changes the qty or price in the POS screen after unsuspend, the amount will not be right

                var strPromo =
                    Convert.ToString(DBNull.Value.Equals(lineFields["PromoID"])
                        ? ""
                        : lineFields["PromoID"]);
                if (strPromo.Length != 0)
                {
                    saleLine.PromoID = strPromo; //  
                }

                // Nicolette added to load reasons from temporary files
                if (rsReason != null && rsReason.Rows.Count != 0)
                {
                    foreach (DataRow reasonFields in rsReason.Rows)
                    {
                        if ((int)reasonFields["Line_Num"] == ln)
                        {
                            saleLine.Return_Reasons.Add(Convert.ToString(reasonFields["Reason"]),
                                Convert.ToString(reasonFields["Reason_Type"]),
                                Convert.ToString(reasonFields["Reason_Type"]));
                        }
                    }
                }
                else
                {
                    saleLine.Return_Reasons = new Return_Reasons();
                }
                saleLines.Add(saleLine);
            }
            Performancelog.Debug($"End,SaleService,GetSaleLinesFromDbTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return saleLines;
        }


        /// <summary>
        /// Method to save card sales
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="givexRecieptType">Givex reciept type</param>
        /// <param name="dataSource">Data source</param>
        public void SaveCardSales(Sale sale, Sale_Line oLine, GiveXReceiptType givexRecieptType, DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,SaveCardSales,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var tillNumber = sale.TillNumber;

            if (sale.Sale_Num == 0 | givexRecieptType.TranType == 0)
            {
                return;
            }
            _connection = new SqlConnection(GetConnectionString(dataSource));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            DataRow fields;
            bool addNew = false;
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from CardSales where TILL_NUM=" + tillNumber + " and SALE_NO=" + Convert.ToString(sale.Sale_Num) + " and LINE_NUM=" + Convert.ToString(oLine.Line_Num), _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                addNew = true;
                fields = _dataTable.NewRow();
                fields["Till_Num"] = tillNumber;
                fields["sale_no"] = sale.Sale_Num;
                fields["Line_Num"] = oLine.Line_Num;
            }
            else
            {
                fields = _dataTable.Rows[0];
            }
            fields["CardType"] = "G";
            fields["CardNum"] = givexRecieptType.CardNum;
            fields["CardBalance"] = givexRecieptType.Balance;
            fields["PointBalance"] = givexRecieptType.PointBalance;
            fields["SaleAmount"] = givexRecieptType.SaleAmount;
            fields["SaleType"] = givexRecieptType.TranType;
            fields["ExpDate"] = givexRecieptType.ExpDate;
            fields["RefNum"] = givexRecieptType.SeqNum;
            fields["Response"] = givexRecieptType.ResponseCode;
            if (addNew)
            {
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
            }
            else
            {
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,SaveCardSales,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to remove previous transactions from temp data base
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        public void RemovePreviousTransactionsFromDbTemp(int tillNumber, int saleNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,RemovePreviousTransactionsFromDbTemp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            // Remove any previous references to the transaction.
            Execute("Delete  from CurrentSale  Where SaleNumber = " + Convert.ToString(saleNumber) + " and TillNumber =" + tillNumber, DataSource.CSCCurSale);
            Performancelog.Debug($"End,SaleService,RemovePreviousTransactionsFromDbTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to clear sale from temp data base
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        public void ClearSaleFromDbTemp(int tillNumber, int saleNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,ClearSaleFromDbTemp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            Execute("Delete  from CurrentSale  Where SaleNumber = " + Convert.ToString(saleNumber) + " and TillNumber = " + tillNumber, DataSource.CSCCurSale);
            Execute("Delete  from CardSales Where CardSales.SALE_NO = " + Convert.ToString(saleNumber) + " and TILL_NUM=" + tillNumber, DataSource.CSCCurSale);
            Performancelog.Debug($"End,SaleService,ClearSaleFromDbTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }


        /// <summary>
        /// Method to get a sale line
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">TIll number</param>
        /// <returns>Sale line</returns>
        public Sale_Line GetPrepaySaleLine(int saleNumber, int tillNumber)
        {
            var sLine = GetRecords("select * from SALELINE where SALE_NO=" + Convert.ToString(saleNumber) + " and Prepay=1 and TILL_NUM=" + tillNumber, DataSource.CSCTills);

            var saleLine = new Sale_Line();

            if (sLine.Rows.Count > 0)
            {
                saleLine.Dept = CommonUtility.GetStringValue(sLine.Rows[0]["Dept"]);
                saleLine.Sub_Dept = CommonUtility.GetStringValue(sLine.Rows[0]["Sub_Dept"]);
                saleLine.Sub_Detail = CommonUtility.GetStringValue(sLine.Rows[0]["Sub_Detail"]);
                saleLine.Stock_Code = CommonUtility.GetStringValue(sLine.Rows[0]["Stock_Code"]);
                saleLine.PLU_Code = CommonUtility.GetStringValue(sLine.Rows[0]["PLU_Code"]);
                saleLine.price = CommonUtility.GetDoubleValue(sLine.Rows[0]["Price"]);
                saleLine.Quantity = CommonUtility.GetFloatValue(sLine.Rows[0]["Quantity"]);
                saleLine.Regular_Price = CommonUtility.GetDoubleValue(sLine.Rows[0]["Reg_Price"]);
                saleLine.PositionID = CommonUtility.GetByteValue(sLine.Rows[0]["PositionID"]);
                saleLine.GradeID = CommonUtility.GetByteValue(sLine.Rows[0]["GradeID"]);
                saleLine.Description = CommonUtility.GetStringValue(sLine.Rows[0]["Descript"]);
                saleLine.Line_Num = CommonUtility.GetShortValue(sLine.Rows[0]["Line_Num"]);
                saleLine.Line_Discount = CommonUtility.GetDoubleValue(sLine.Rows[0]["Discount"]);
                saleLine.FuelRebateEligible = CommonUtility.GetBooleanValue(sLine.Rows[0]["FuelRebateUsed"]);
                saleLine.FuelRebate = CommonUtility.GetDecimalValue(sLine.Rows[0]["RebateDiscount"]);
                saleLine.Amount = CommonUtility.GetDecimalValue(sLine.Rows[0]["Amount"]);
                saleLine.Total_Amount = Convert.ToDecimal(sLine.Rows[0]["TOTAL_AMT"]);
                saleLine.TaxForTaxExempt = CommonUtility.GetBooleanValue(sLine.Rows[0]["TaxExempt"]);
            }
            return saleLine;
        }



        /// <summary>
        /// Method to add card profile prompt to temp data base
        /// </summary>
        /// <param name="cardProfilePrompt">Card profile prompt</param>
        public void AddCardProfilePromptToDbTemp(CardProfilePrompt cardProfilePrompt)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddCardProfilePromptToDbTemp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            Execute("Delete  from CardProfilePrompts where sale_no ="
                + Convert.ToString(cardProfilePrompt.SaleNumber)
                + " and till_num = " + Convert.ToString(cardProfilePrompt.TillNumber)
                + " and cardnum = \'" + cardProfilePrompt.CardNumber
                + "\'and ProfileID = \'" + Convert.ToString(cardProfilePrompt.ProfileID)
                + "\'and PromptID = " + cardProfilePrompt.PromptID, DataSource.CSCCurSale);
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCCurSale));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("Select * from CardProfilePrompts where sale_no ="
                + Convert.ToString(cardProfilePrompt.SaleNumber)
                + " and till_num = " + Convert.ToString(cardProfilePrompt.TillNumber)
                + " and cardnum = \'" + cardProfilePrompt.CardNumber
                + "\'and ProfileID = \'" + Convert.ToString(cardProfilePrompt.ProfileID)
                + "\'and PromptID = " + cardProfilePrompt.PromptID, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                var fields = _dataTable.NewRow();
                fields["sale_no"] = cardProfilePrompt.SaleNumber;
                fields["Till_Num"] = cardProfilePrompt.TillNumber;
                fields["CardNum"] = cardProfilePrompt.CardNumber;
                fields["ProfileID"] = cardProfilePrompt.ProfileID;
                fields["PromptID"] = cardProfilePrompt.PromptID;
                fields["PromptAnswer"] = cardProfilePrompt.PromptAnswer;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,AddCardProfilePromptToDbTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to get list of tax master
        /// </summary>
        /// <returns>List of tax mast</returns>
        public List<TaxMast> GetTaxMast()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetTaxMast,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var taxMastRecordSet = GetRecords("SELECT * FROM TaxMast ORDER BY TaxMast.Tax_Ord ", DataSource.CSCMaster);
            var taxes = new List<TaxMast>();
            foreach (DataRow fields in taxMastRecordSet.Rows)
            {
                var taxMast = new TaxMast
                {
                    TaxOrd = CommonUtility.GetShortValue(fields["TAX_ORD"]),
                    Active = CommonUtility.GetBooleanValue(fields["TAX_ACTIVE"]),
                    TaxApply = CommonUtility.GetStringValue(fields["TAX_APPLY"]),
                    TaxDefination = CommonUtility.GetStringValue(fields["TAX_DEF"]),
                    TaxDescription = CommonUtility.GetStringValue(fields["TAX_DESC"]),
                    TaxName = CommonUtility.GetStringValue(fields["TAX_NAME"])
                };
                taxes.Add(taxMast);
            }
            Performancelog.Debug($"End,SaleService,GetTaxMast,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return taxes;
        }


        /// <summary>
        /// Method to get list of tax rates
        /// </summary>
        /// <returns>List of tax rates</returns>
        public List<TaxRate> GetTaxRates()
        {

            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetTaxRates,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var taxRateRecordSet = GetRecords("SELECT * FROM TaxRate", DataSource.CSCMaster);

            var taxRates = new List<TaxRate>();
            foreach (DataRow fields in taxRateRecordSet.Rows)
            {
                var taxRate = new TaxRate
                {
                    TaxName = CommonUtility.GetStringValue(fields["TAX_NAME"]),
                    TaxCode = CommonUtility.GetStringValue(fields["TAX_CODE"]),
                    TaxDescription = CommonUtility.GetStringValue(fields["TAX_DESC"]),
                    Rebate = CommonUtility.GetFloatValue(fields["TAX_REBATE"]),
                    Rate = CommonUtility.GetFloatValue(fields["TAX_RATE"]),
                    Included = CommonUtility.GetBooleanValue(fields["TAX_INCL"])
                };
                taxRates.Add(taxRate);
            }
            Performancelog.Debug($"End,SaleService,GetTaxRates,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return taxRates;
        }

        /// <summary>
        /// Method to save deleted line to CSCTills
        /// </summary>
        /// <param name="oLine">Sale line</param>
        /// <param name="delType">Deletion type</param>
        public void SaveDeletedLineToDbTill(ref Sale_Line oLine, string delType)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,SaveDeletedLineToDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from DelLines where till_num= " + oLine.Till_Num, _connection);
            _adapter.Fill(_dataTable);

            var fields = _dataTable.NewRow();
            fields["Till_Num"] = oLine.Till_Num;
            fields["sale_no"] = oLine.Sale_Num;
            fields["Del_Date"] = DateTime.Now;
            fields["Stock_Code"] = oLine.Stock_Code;
            fields["PLU_Code"] = oLine.PLU_Code ?? oLine.Stock_Code;
            fields["Descript"] = oLine.Description;
            fields["Quantity"] = oLine.Quantity;
            fields["price"] = oLine.price;
            fields["Reg_Price"] = oLine.Regular_Price;
            fields["Discount"] = oLine.Line_Discount;
            fields["Disc_Type"] = oLine.Discount_Type;
            fields["Disc_adj"] = oLine.Discount_Adjust;
            fields["Disc_Code"] = oLine.Discount_Code;
            fields["Amount"] = oLine.Amount;
            fields["Assoc_Amt"] = oLine.Associate_Amount;
            fields["Total_Amt"] = oLine.Total_Amount;
            fields["Sale_Type"] = delType;
            fields["Disc_Rate"] = oLine.Discount_Rate;
            fields["Price_Type"] = oLine.Price_Type.ToString();
            fields["User_Price"] = 0; //False
            fields["Loyl_Save"] = oLine.Loyalty_Save;
            fields["Units"] = oLine.Units;
            fields["Cost"] = oLine.Cost;
            fields["Group"] = oLine.Group_Price;
            fields["Serial_No"] = oLine.Serial_No;
            fields["Gift_Cert"] = oLine.Gift_Certificate;
            fields["Dept"] = oLine.Dept;
            fields["Sub_Dept"] = oLine.Sub_Dept;
            fields["Sub_Detail"] = oLine.Sub_Detail;
            fields["User"] = oLine.User;
            fields["Del_Action"] = delType;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,SaveDeletedLineToDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to clear tender record from Current sale database
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        public void ClearTenderRecordsFromDbTemp(int tillNumber, int saleNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,ClearTenderRecordsFromDbTemp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            Execute("Delete  From SaleTend    Where SaleTend.Sale_No = " + Convert.ToString(saleNumber) + " and TILL_NUM=" + tillNumber, DataSource.CSCCurSale);
            Execute("Delete  From CardTenders     Where CardTenders.Sale_No = " + Convert.ToString(saleNumber) + " and TILL_NUM=" + tillNumber + " and AllowMulticard<2", DataSource.CSCCurSale);
            Execute("Delete  From CardTenders     Where CardTenders.Sale_No = " + Convert.ToString(saleNumber) + " and TILL_NUM=" + tillNumber + " and (AllowMulticard>1 and Amount=0)", DataSource.CSCCurSale);
            Execute("DELETE  From SaleVendorCoupon WHERE Sale_No = " + saleNumber + " AND TILL_NUM=" + tillNumber, DataSource.CSCCurSale);
            Performancelog.Debug($"End,SaleService,ClearTenderRecordsFromDbTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        #region Save Sale

        /// <summary>
        /// Method to clear records from csctills
        /// </summary>
        /// <param name="saleLineNumber">Line number</param>
        /// <param name="tillNumber">Till number</param>
        public void ClearRecordsFromDbTill(int saleLineNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,ClearRecordsFromDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            //Execute("Delete  From SaleHead    Where SaleHead.Sale_No = " + saleLineNumber + " and Till=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  From SaleLine    Where SaleLine.Sale_No = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  From SaleTend    Where SaleTend.Sale_No = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  From SaleKit     Where SaleKit.Sale_No = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  From SaleChg     Where SaleChg.Sale_No = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  From S_LineTax   Where S_LineTax.Sale_No = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  From S_SaleTax   Where S_SaleTax.Sale_No = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  From SLineReason Where SLineReason.Sale_No = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  From SaleVendors   Where SaleVendors.Sale_No = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  From DiscountTender Where DiscountTender.SALE_NO = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  From SaleKitChg where Sale_No = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  From SaleKitChgTax where Sale_No  = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  From ChargeTax where Sale_No = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  From CardTenders where Sale_No = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  From VoidSale where Sale_No = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  From CardProfilePrompts where Sale_No = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  from CardSales Where CardSales.SALE_NO = " + saleLineNumber + " and TILL_NUM=" + tillNumber, DataSource.CSCTills);
            //Execute("Delete  from GiftCert where Sale_No = " + saleLineNumber, DataSource.CSCMaster);

            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            //_dataTable = new DataTable();
            SqlCommand cmd = new SqlCommand("ClearRecordsFromDbTill", _connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@SaleNumber", Value = saleLineNumber });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@TillNumber", Value = tillNumber });
            cmd.ExecuteNonQuery();

            Performancelog.Debug($"End,SaleService,ClearRecordsFromDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }

        /// <summary>
        /// Method to get list of sale tenders from CSCTills
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of sale tenders</returns>
        public List<SaleTend> GetSaleTendsFromDbTill(int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetSaleTendsFromDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var saleTendRs = GetRecords("SELECT * from SaleTend where TILL_NUM=" + tillNumber, DataSource.CSCTills);
            var saleTends = new List<SaleTend>();
            foreach (DataRow fields in saleTendRs.Rows)
            {
                var saleTend = new SaleTend
                {
                    SaleNumber = CommonUtility.GetIntergerValue(fields["SALE_NO"]),
                    TillNumber = tillNumber,
                    TenderClass = CommonUtility.GetStringValue(fields["TENDCLAS"]),
                    AmountTend = CommonUtility.GetDecimalValue(fields["AMTTEND"]),
                    AmountUsed = CommonUtility.GetDecimalValue(fields["AMTUSED"]),
                    AuthUser = CommonUtility.GetStringValue(fields["AuthUser"]),
                    CCardAPRV = CommonUtility.GetStringValue(fields["CCARD_APRV"]),
                    CCardNumber = CommonUtility.GetStringValue(fields["CCARD_NUM"]),
                    SequenceNumber = CommonUtility.GetIntergerValue(fields["Sequence"]),
                    SerialNumber = CommonUtility.GetStringValue(fields["SERNUM"]),
                    TenderName = CommonUtility.GetStringValue(fields["TENDNAME"]),
                    Exchange = CommonUtility.GetDecimalValue(fields["Exchange_Rate"])
                };
                saleTends.Add(saleTend);
            }
            Performancelog.Debug($"End,SaleService,GetSaleTendsFromDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return saleTends;
        }

        /// <summary>
        /// Method to get list of sale tenders from CSCCursale
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>List of sale tenders</returns>
        public List<SaleTend> GetSaleTendsFromDbTemp(int tillNumber, int saleNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetSaleTendsFromDbTemp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var saleTendRs = GetRecords("SELECT * FROM SaleTend WHERE Sale_No = " + Convert.ToString(saleNumber) + " AND Till_Num=" + tillNumber, DataSource.CSCCurSale);
            var saleTends = new List<SaleTend>();
            foreach (DataRow fields in saleTendRs.Rows)
            {
                var saleTend = new SaleTend
                {
                    SaleNumber = CommonUtility.GetIntergerValue(fields["SALE_NO"]),
                    TillNumber = tillNumber,
                    TenderClass = CommonUtility.GetStringValue(fields["TENDCLAS"]),
                    AmountTend = CommonUtility.GetDecimalValue(fields["AMTTEND"]),
                    AmountUsed = CommonUtility.GetDecimalValue(fields["AMTUSED"]),
                    AuthUser = CommonUtility.GetStringValue(fields["AuthUser"]),
                    CCardAPRV = CommonUtility.GetStringValue(fields["CCARD_APRV"]),
                    CCardNumber = CommonUtility.GetStringValue(fields["CCARD_NUM"]),
                    SequenceNumber = CommonUtility.GetIntergerValue(fields["Sequence"]),
                    SerialNumber = CommonUtility.GetStringValue(fields["SERNUM"]),
                    TenderName = CommonUtility.GetStringValue(fields["TENDNAME"]),
                    Exchange = CommonUtility.GetDecimalValue(fields["Exchange_Rate"])
                };
                saleTends.Add(saleTend);
            }
            Performancelog.Debug($"End,SaleService,GetSaleTendsFromDbTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return saleTends;
        }

        /// <summary>
        /// Method to add sale head in CSCTills
        /// </summary>
        /// <param name="saleHead">Sale head</param>
        public void UpdateSaleHeadToDbTill(SaleHead saleHead)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,UpdateSaleHeadToDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from SALEHEAD where TILL=" + saleHead.TillNumber + " AND SALE_NO =" + saleHead.SaleNumber, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                var fields = _dataTable.Rows[0];
                fields["SALE_NO"] = saleHead.SaleNumber;
                fields["TILL"] = saleHead.TillNumber;
                fields["T_TYPE"] = saleHead.TType;
                fields["TEND_LINES"] = saleHead.TendLine;
                fields["TEND_AMT"] = saleHead.TenderAmount;
                fields["TreatyNumber"] = saleHead.TreatyNumber;
                fields["DISC_TYPE"] = saleHead.DiscountType;
                fields["MainTill_CloseNum"] = saleHead.MainTillCloseNum;
                fields["Reason_Type"] = saleHead.ReasonType;
                fields["SALE_NO"] = saleHead.SaleNumber;
                fields["Reason"] = saleHead.Reason;
                fields["ReferenceNum"] = saleHead.RefernceNum;
                fields["REGIST"] = saleHead.Register;
                fields["USER"] = saleHead.User;
                fields["Upsell"] = saleHead.Upsell;
                fields["INVC_DISC"] = saleHead.INVCDiscount;
                fields["OverPayment"] = saleHead.OverPayment;
                fields["PayOut"] = saleHead.PayOut;
                fields["PO_CODE"] = saleHead.POCode;
                fields["PAYMENT"] = saleHead.Payment;
                fields["Penny_Adj"] = saleHead.PennyAdjust;
                fields["LOYL_POINT"] = saleHead.LoyalPoint;
                fields["ASSOC_AMT"] = saleHead.AssociatedAmount;
                fields["SD"] = saleHead.SD;
                fields["SALE_DATE"] = saleHead.SaleDate;
                fields["SALE_LINES"] = saleHead.SaleLine;
                fields["Shift"] = saleHead.Shift;
                fields["ShiftDate"] = saleHead.ShiftDate;
                fields["DEPOSIT"] = saleHead.Deposit;
                fields["LINE_DISC"] = saleHead.LineDiscount;
                fields["LOYL_BALANCE"] = saleHead.LoyaltyBalance;
                fields["LoyaltyCard"] = saleHead.LoyaltyCard;
                fields["CHANGE"] = saleHead.Change;
                fields["CLIENT"] = saleHead.Client;
                fields["Close_Num"] = saleHead.CloseNum;
                fields["CREDITS"] = saleHead.Credits;
                fields["STORE"] = saleHead.Store;
                fields["SALE_TIME"] = saleHead.SaleTime;
                fields["SALE_AMT"] = saleHead.SaleAmount;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,UpdateSaleHeadToDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to add sale tender
        /// </summary>
        /// <param name="saleTend">Sale tender</param>
        /// <param name="dataSource">Data source</param>
        public void AddSaleTend(SaleTend saleTend, DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddSaleTend,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(dataSource));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            bool addNew = false;
            DataRow fields;
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from SaleTend where TILL_NUM=" + saleTend.TillNumber
                + " and SALE_NO = " + saleTend.SaleNumber + " and Sequence= " + saleTend.SequenceNumber, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                fields = _dataTable.NewRow();
                addNew = true;
                fields["SALE_NO"] = saleTend.SaleNumber;
                fields["TILL_NUM"] = saleTend.TillNumber;
                fields["Sequence"] = saleTend.SequenceNumber;
            }
            else
            {
                fields = _dataTable.Rows[0];
            }
            fields["TENDNAME"] = saleTend.TenderName;
            fields["TENDCLAS"] = saleTend.TenderClass;
            fields["AMTTEND"] = saleTend.AmountTend ?? 0;
            fields["AMTUSED"] = saleTend.AmountUsed ?? 0;
            if (saleTend.Exchange != null) fields["Exchange_Rate"] = saleTend.Exchange.Value;
            fields["SERNUM"] = saleTend.SerialNumber;
            fields["CCARD_NUM"] = saleTend.CCardNumber;
            fields["CCARD_APRV"] = saleTend.CCardAPRV;
            fields["AuthUser"] = saleTend.AuthUser;
            if (addNew)
            {
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
            }
            else
            {
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,AddSaleTend,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to void sale from CSCTills
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Void sale</returns>
        public VoidSale GetVoidSaleFromDbTill(int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetVoidSaleFromDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var voidSaleRs = GetRecords("SELECT * from VoidSale where TILL_NUM=" + tillNumber, DataSource.CSCTills);
            var fields = voidSaleRs.Rows[0];

            var voidSale = new VoidSale
            {
                VoidNumber = CommonUtility.GetIntergerValue(fields["Void_No"]),
                SaleNumber = CommonUtility.GetIntergerValue(fields["Sale_No"]),
                TillNumber = tillNumber
            };
            Performancelog.Debug($"End,SaleService,GetVoidSaleFromDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return voidSale;

        }

        /// <summary>
        /// Method get sale vendor from CSCTills
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Sale vendors</returns>
        public SaleVendors GetSaleVendorsFromDbTill(int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetSaleVendorsFromDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var saleVendorsRs = GetRecords("SELECT * from SaleVendors where TILL_NUM=" + tillNumber, DataSource.CSCTills);
            var fields = saleVendorsRs.Rows[0];

            var saleVendors = new SaleVendors
            {
                Vendor = CommonUtility.GetStringValue(fields["Vendor"]),
                SaleNumber = CommonUtility.GetIntergerValue(fields["Sale_No"]),
                TillNumber = tillNumber
            };
            Performancelog.Debug($"End,SaleService,GetSaleVendorsFromDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return saleVendors;
        }

        /// <summary>
        /// Method to add void sale to CSCTills
        /// </summary>
        /// <param name="voidSale">Void sale</param>
        public void AddVoidSaleToDbTill(VoidSale voidSale)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddVoidSaleToDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from VoidSale where TILL_NUM=" + voidSale.TillNumber +
                " and Sale_No = " + voidSale.SaleNumber, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                var fields = _dataTable.NewRow();
                fields["Void_No"] = voidSale.VoidNumber;
                fields["Sale_No"] = voidSale.SaleNumber;
                fields["TILL_NUM"] = voidSale.TillNumber;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,AddVoidSaleToDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to add sale vendors to CSCTills
        /// </summary>
        /// <param name="saleVendors">Sale vendor</param>
        public void AddSaleVendorsToDbTill(SaleVendors saleVendors)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddSaleVendorsToDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from SaleVendors where TILL_NUM=" + saleVendors.TillNumber +
                " and Sale_No = " + saleVendors.SaleNumber, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                var fields = _dataTable.NewRow();
                fields["Vendor"] = saleVendors.Vendor;
                fields["Sale_No"] = saleVendors.SaleNumber;
                fields["TILL_NUM"] = saleVendors.TillNumber;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,AddSaleVendorsToDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to add sale tax to CSCTills
        /// </summary>
        /// <param name="saleTax">Sale tax</param>
        public void AddSaleTaxToDbTill(Sale_Tax saleTax)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddSaleTaxToDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from S_SaleTax where TILL_NUM=" + saleTax.TillNumber +
                 " and Sale_No = " + saleTax.SaleNumber +
                  " and Tax_Name = '" + saleTax.Tax_Name +
                   "' and Tax_Code = '" + saleTax.Tax_Code + "'", _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                var fields = _dataTable.NewRow();
                fields["TILL_NUM"] = saleTax.TillNumber;
                fields["Sale_No"] = saleTax.SaleNumber;
                fields["Tax_Name"] = saleTax.Tax_Name;
                fields["Tax_Code"] = saleTax.Tax_Code;
                fields["Tax_Rate"] = saleTax.Tax_Rate;
                fields["Taxable_Amount"] = saleTax.Taxable_Amount;
                fields["Tax_Added_Amount"] = saleTax.Tax_Added_Amount;
                fields["Tax_Included_Amount"] = saleTax.Tax_Included_Amount;
                fields["Tax_Included_Total"] = saleTax.Tax_Included_Total;
                fields["Tax_Rebate_Rate"] = saleTax.Tax_Rebate_Rate;
                fields["Tax_Rebate"] = saleTax.Tax_Rebate;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,AddSaleTaxToDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to add sale line to CSCTills
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="saleType">Sale type</param>
        public void AddSaleLineToDbTill(Sale_Line saleLine, string saleType)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddSaleLineToDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from SALELINE where TILL_NUM=" + saleLine.Till_Num + "and SALE_NO= " + saleLine.Sale_Num + " and  Line_Num = " + saleLine.Line_Num, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                var fields = _dataTable.NewRow();
                fields["PLU_Code"] = saleLine.PLU_Code;
                fields["price"] = saleLine.price;
                fields["SALE_NO"] = saleLine.Sale_Num;
                fields["TILL_NUM"] = saleLine.Till_Num;
                fields["Quantity"] = saleLine.Quantity;
                fields["Line_Num"] = saleLine.Line_Num;
                fields["Stock_Code"] = saleLine.Stock_Code;
                fields["DiscountName"] = saleLine.DiscountName;
                fields["Descript"] = saleLine.Description;
                fields["REG_PRICE"] = saleLine.Regular_Price;
                fields["Discount"] = saleLine.Line_Discount;
                fields["Disc_Type"] = saleLine.Discount_Type;
                fields["Disc_Adj"] = saleLine.Discount_Adjust;
                fields["Disc_Code"] = saleLine.Discount_Code;
                fields["Disc_Rate"] = saleLine.Discount_Rate;
                fields["ASSOC_AMT"] = saleLine.Associate_Amount;
                fields["TOTAL_AMT"] = saleLine.Total_Amount;
                fields["SALE_TYPE"] = saleType;
                fields["PRICE_TYPE"] = saleLine.Price_Type.ToString();
                fields["Amount"] = saleLine.Amount;
                fields["PumpID"] = saleLine.pumpID;
                fields["PositionID"] = saleLine.PositionID;
                fields["GradeID"] = saleLine.GradeID;
                fields["Gift_Cert"] = saleLine.Gift_Certificate;
                fields["PromoID"] = saleLine.PromoID;
                fields["Prepay"] = saleLine.Prepay;
                fields["SERIAL_NO"] = saleLine.Serial_No;
                fields["PaidByCard"] = saleLine.PaidByCard;
                fields["Upsell"] = saleLine.Upsell;
                fields["TaxExempt"] = saleLine.IsTaxExemptItem;
                fields["TOTAL_AMT"] = saleLine.Total_Amount;
                fields["Rebate"] = saleLine.Rebate;
                fields["ScalableItem"] = saleLine.ScalableItem;
                fields["CardProfileID"] = saleLine.CardProfileID;
                fields["LOYL_SAVE"] = saleLine.Loyalty_Save;
                fields["UNITS"] = saleLine.Units;
                fields["COST"] = saleLine.Cost;
                fields["GROUP"] = saleLine.Group_Price;
                fields["SERIAL_NO"] = saleLine.Serial_No;
                fields["GIFT_CERT"] = saleLine.Gift_Certificate;
                fields["DEPT"] = saleLine.Dept;
                fields["SUB_DEPT"] = saleLine.Sub_Dept;
                fields["Sub_Detail"] = saleLine.Sub_Detail;
                fields["USER"] = saleLine.User;
                fields["ManualFuel"] = saleLine.ManualFuel;
                fields["GC_Num"] = saleLine.Gift_Num;
                fields["Rebate"] = saleLine.Rebate;
                fields["PaidByCard"] = saleLine.PaidByCard;
                fields["Upsell"] = saleLine.Upsell;
                fields["FuelRebateUsed"] = saleLine.FuelRebate;
                fields["RebateDiscount"] = saleLine.Rebate;
                fields["ScalableItem"] = saleLine.ScalableItem;
                fields["CardProfileID"] = saleLine.CardProfileID;
                fields["Vendor"] = saleLine.Vendor;
                fields["TE_Amount_Incl"] = saleLine.TE_Amount_Incl;
                fields["ElgTaxExemption"] = saleLine.EligibleTaxEx;
                fields["CarwashCode"] = saleLine.CarwashCode;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,AddSaleLineToDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to update tax exempt
        /// </summary>
        /// <param name="strSql">Query</param>
        /// <param name="dataSource">Data source</param>
        public void UpdateTaxExempt(string strSql, DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,UpdateTaxExempt,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            Execute(strSql, dataSource);
            Performancelog.Debug($"End,SaleService,UpdateTaxExempt,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to update stock branch
        /// </summary>
        /// <param name="strSql">Query</param>
        /// <param name="dataSource">Data source</param>
        public void UpdateStockBr(string strSql, DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,UpdateStockBr,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            Execute(strSql, dataSource);
            Performancelog.Debug($"End,SaleService,UpdateStockBr,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to add gift certificates
        /// </summary>
        /// <param name="giftCert">Gift certificate</param>
        public void AddGiftCertificate(GiftCert giftCert)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddGiftCertificate,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from GiftCert where gc_num = '" + giftCert.GcNumber + "' and gc_amount = " + giftCert.GcAmount, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                var fields = _dataTable.NewRow();
                fields["sale_no"] = giftCert.SaleNumber;
                fields["Line_No"] = giftCert.LineNumber;
                fields["GC_Num"] = giftCert.GcNumber;
                fields["GC_Amount"] = Math.Round(giftCert.GcAmount, 2);
                fields["GC_Cust"] = giftCert.GcCust;
                fields["GC_Date"] = DateTime.Today;
                fields["GC_User"] = giftCert.GcUser;
                fields["GC_Expires_On"] = DateTime.Today.AddDays(giftCert.GcExpiryDays);
                fields["GC_Regist"] = giftCert.GcRegister;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,AddGiftCertificate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to add sale line reason
        /// </summary>
        /// <param name="reason">Reason</param>
        public void AddSaleLineReason(SaleLineReason reason)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddSaleLineReason,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from SLineReason where TILL_NUM=" + reason.TillNumber, _connection);
            _adapter.Fill(_dataTable);
            var fields = _dataTable.NewRow();
            fields["Till_Num"] = reason.TillNumber;
            fields["sale_no"] = reason.SaleNumber;
            fields["Line_Num"] = reason.LineNumber;
            fields["Reason"] = reason.Reason;
            fields["Reason_Type"] = reason.ReasonType;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,AddSaleLineReason,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to add sale line tax
        /// </summary>
        /// <param name="lineTax">Line tax</param>
        public void AddSaleLineTax(Line_Tax lineTax)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddSaleLineTax,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from S_LineTax where TILL_NUM=" + lineTax.TillNumber, _connection);
            _adapter.Fill(_dataTable);

            var fields = _dataTable.NewRow();
            fields["TILL_NUM"] = lineTax.TillNumber;
            fields["Sale_No"] = lineTax.SaleNumber;
            fields["Line_No"] = lineTax.LineNumber;
            fields["Tax_Name"] = lineTax.Tax_Name;
            fields["Tax_Code"] = lineTax.Tax_Code;
            fields["Tax_Rate"] = lineTax.Tax_Rate;
            fields["Tax_Included"] = lineTax.Tax_Included;
            fields["Tax_Added_Amount"] = lineTax.Tax_Added_Amount;
            fields["Tax_Included_Amount"] = lineTax.Tax_Incl_Amount;
            fields["Tax_Rebate"] = lineTax.Tax_Rebate;
            fields["Tax_Rebate_Rate"] = lineTax.Tax_Rebate_Rate;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,AddSaleLineTax,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to add sale line kit
        /// </summary>
        /// <param name="lineKit">Line kit</param>
        public void AddSaleLineKit(Line_Kit lineKit)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddSaleLineKit,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from SaleKit where TILL_NUM=" + lineKit.TillNumber, _connection);
            _adapter.Fill(_dataTable);

            var fields = _dataTable.NewRow();
            fields["Till_Num"] = lineKit.TillNumber;
            fields["sale_no"] = lineKit.SaleNumber;
            fields["Line_No"] = lineKit.LineNumber;
            fields["Kit_Item"] = lineKit.Kit_Item;
            fields["Descript"] = lineKit.Kit_Item_Desc;
            fields["Quantity"] = lineKit.Kit_Item_Qty;
            fields["Base"] = lineKit.Kit_Item_Base;
            fields["Fraction"] = lineKit.Kit_Item_Fraction;
            fields["Alloc"] = lineKit.Kit_Item_Allocate;
            fields["Serial"] = lineKit.Kit_Item_Serial;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,AddSaleLineKit,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to add charge tax
        /// </summary>
        /// <param name="chargeTax"></param>
        public void AddChargeTax(Charge_Tax chargeTax)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddChargeTax,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from SaleKitChgTax where TILL_NUM=" + chargeTax.TillNumber, _connection);
            _adapter.Fill(_dataTable);

            var fields = _dataTable.NewRow();
            fields["Till_Num"] = chargeTax.TillNumber;
            fields["sale_no"] = chargeTax.SaleNumber;
            fields["Line_No"] = chargeTax.LineNumber;
            fields["Kit_Item"] = chargeTax.KitItem;
            fields["As_Code"] = chargeTax.ChargeCode;
            fields["Tax_Name"] = chargeTax.Tax_Name;
            fields["Tax_Code"] = chargeTax.Tax_Code;
            fields["Tax_Rate"] = chargeTax.Tax_Rate;
            fields["Tax_Included"] = chargeTax.Tax_Included;
            fields["Tax_Added_Amount"] = chargeTax.Tax_Added_Amount;
            fields["Tax_Included_Amount"] = chargeTax.Tax_Incl_Amount;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,AddChargeTax,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to add card tender
        /// </summary>
        /// <param name="cardSale">Card sale</param>
        /// <param name="dataSource">Data source</param>
        public void AddCardTender(CardTender cardSale, DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddCardTender,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var query =
                $"Select * from CardTenders where TILL_NUM = {cardSale.TillNumber} and Sale_no = {cardSale.SaleNumber} and tender_name = '{cardSale.TenderName}'";
            _connection = new SqlConnection(GetConnectionString(dataSource));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            bool addNew = false;
            DataRow fields;
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                addNew = true;
                fields = _dataTable.NewRow();
                fields["Till_Num"] = cardSale.TillNumber;
                fields["sale_no"] = cardSale.SaleNumber;
                fields["Tender_Name"] = cardSale.TenderName;
            }
            else
            {
                fields = _dataTable.Rows[0];
            }
            fields["Card_Name"] = cardSale.CardName;
            fields["Card_Type"] = cardSale.CardType;
            fields["Store_Forward"] = cardSale.StoreForward;
            fields["Decline_Code"] = cardSale.Decline_Code;
            fields["Card_Number"] = cardSale.CardNum;
            fields["Expiry_Date"] = cardSale.ExpiryDate;
            fields["Swiped"] = cardSale.Swiped;
            fields["Amount"] = cardSale.Amount;
            fields["Approval_Code"] = cardSale.ApprovalCode;
            fields["SequenceNumber"] = cardSale.SequenceNumber;
            fields["Decline_Reason"] = cardSale.DeclineReason;
            fields["Result"] = cardSale.Result;
            fields["TerminalID"] = cardSale.TerminalID;
            fields["DebitAccount"] = cardSale.DebitAccount;
            fields["ResponseCode"] = cardSale.ResponseCode;
            fields["ISOCode"] = cardSale.ISOCode;
            fields["TransactionDate"] = cardSale.TransactionDate;
            var time = Convert.ToDateTime(cardSale.TransactionTime.ToShortTimeString());
            fields["TransactionTime"] = time.Date == DateTime.MinValue.Date ?
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, time.Hour, time.Minute, time.Second, time.Millisecond) : time;
            fields["ReceiptDisplay"] = cardSale.ReceiptDisplay;
            fields["Language"] = cardSale.Language;
            fields["CustomerName"] = cardSale.CustomerName;
            fields["CallTheBank"] = cardSale.CallTheBank;
            // 
            fields["BatchNumber"] = cardSale.BatchNumber;
            fields["VechicleNo"] = cardSale.VechicleNo;
            fields["DriverNo"] = cardSale.DriverNo;
            fields["IdentificationNo"] = cardSale.IdentificationNo;
            fields["Odometer"] = cardSale.Odometer;
            fields["CardUsage"] = cardSale.CardUsage;
            fields["Message"] = cardSale.Message;
            fields["PrintVechicleNo"] = cardSale.PrintVechicleNo;
            fields["PrintDriverNo"] = cardSale.PrintDriverNo;
            fields["PrintIdentificationNo"] = cardSale.PrintIdentificationNo;
            fields["PrintUsage"] = cardSale.PrintUsage;
            fields["PrintOdometer"] = cardSale.PrintOdometer;
            fields["Balance"] = cardSale.Balance;
            fields["Quantity"] = cardSale.Quantity;
            fields["CardProfileID"] = cardSale.CardProfileID;
            fields["PONumber"] = cardSale.PONumber;
            fields["Sequence"] = cardSale.Sequence;
            if (addNew)
            {
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
            }
            else
            {
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
            }
            try
            {
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
                Performancelog.Debug($"End,SaleService,AddCardTender,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// Method to add charge
        /// </summary>
        /// <param name="charge">Charge</param>
        public void AddCharge(Charge charge)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddCharge,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from SaleChg where TILL_NUM=" + charge.TillNumber, _connection);
            _adapter.Fill(_dataTable);
            var fields = _dataTable.NewRow();
            fields["Till_Num"] = charge.TillNumber;
            fields["sale_no"] = charge.SaleNumber;
            fields["Line_No"] = charge.LineNumber;
            //fields["Kit_Item"] = charge.KitItem;
            fields["As_Code"] = charge.AsCode;
            fields["Description"] = charge.Charge_Desc;
            fields["Quantity"] = charge.Quantity;
            fields["price"] = charge.Charge_Price;
            fields["Amount"] = charge.Charge_Price * charge.Quantity;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _adapter?.Dispose();
            _connection.Close();
            Performancelog.Debug($"End,SaleService,AddCharge,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to get discount tender
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        public DiscountTender GetDiscountTender(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetDiscountTender,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            DiscountTender discountTender = null;
            var sDisTend = GetRecords("select * from DiscountTender where SALE_NO=" + Convert.ToString(saleNumber) + " AND TILL_NUM=" + tillNumber, DataSource.CSCTills);
            if (sDisTend != null && sDisTend.Rows.Count != 0)
            {
                var fields = sDisTend.Rows[0];
                discountTender = new DiscountTender
                {
                    TillNumber = CommonUtility.GetIntergerValue(fields["Till_Num"]),
                    SaleNumber = CommonUtility.GetIntergerValue(fields["sale_no"]),
                    ClCode = CommonUtility.GetStringValue(fields["cl_code"]),
                    CardNumber = CommonUtility.GetStringValue(fields["CardNum"]),
                    SaleAmount = CommonUtility.GetDecimalValue(fields["SaleAmount"]),
                    DiscountType = CommonUtility.GetStringValue(fields["DiscountType"]),
                    DiscountRate = CommonUtility.GetFloatValue(fields["DiscountRate"]),
                    DiscountAmount = CommonUtility.GetDecimalValue(fields["Discountamount"]),
                    CouponId = CommonUtility.GetStringValue(fields["CouponID"]),
                };
            }
            Performancelog.Debug($"End,SaleService,GetDiscountTender,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return discountTender;
        }

        /// <summary>
        /// Method to add discount tender
        /// </summary>
        /// <param name="tender">Discount tender</param>
        public void AddDiscountTender(DiscountTender tender)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddDiscountTender,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from DiscountTender where SALE_NO=" + Convert.ToString(tender.SaleNumber) + " AND TILL_NUM=" + tender.TillNumber, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                var fields = _dataTable.NewRow();
                fields["Till_Num"] = tender.TillNumber;
                fields["sale_no"] = tender.SaleNumber;
                fields["cl_code"] = tender.ClCode;
                fields["CardNum"] = tender.CardNumber;
                fields["SaleAmount"] = tender.SaleAmount;
                fields["DiscountType"] = tender.DiscountType;
                fields["DiscountRate"] = tender.DiscountRate;
                fields["Discountamount"] = tender.DiscountAmount;
                fields["CouponID"] = tender.CouponId;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,AddDiscountTender,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to update discount tender
        /// </summary>
        /// <param name="tender">Discount tender</param>
        public void UpdateDiscountTender(DiscountTender tender)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,UpdateDiscountTender,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from DiscountTender where SALE_NO=" + Convert.ToString(tender.SaleNumber) + " AND TILL_NUM=" + tender.TillNumber, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                var fields = _dataTable.Rows[0];
                fields["Till_Num"] = tender.TillNumber;
                fields["sale_no"] = tender.SaleNumber;
                fields["cl_code"] = tender.ClCode;
                fields["CardNum"] = tender.CardNumber;
                fields["SaleAmount"] = tender.SaleAmount;
                fields["DiscountType"] = tender.DiscountType;
                fields["DiscountRate"] = tender.DiscountRate;
                fields["Discountamount"] = tender.DiscountAmount;
                fields["CouponID"] = tender.CouponId;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
            }
            Performancelog.Debug($"End,SaleService,UpdateDiscountTender,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to get coupon by coupon id
        /// </summary>
        /// <param name="couponId">Coupon Id</param>
        /// <returns>Coupon</returns>
        public Coupon GetCoupon(string couponId)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetCoupon,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var sCoupon = GetRecords("select * from Coupon where CouponID=\'" + couponId + "\'", DataSource.CSCMaster);
            if (sCoupon != null && sCoupon.Rows.Count != 0)
            {
                var fields = sCoupon.Rows[0];
                return new Coupon
                {
                    Amount = CommonUtility.GetDecimalValue(fields["Amount"]),
                    CouponId = CommonUtility.GetStringValue(fields["CouponId"]),
                    ExpiryDate = CommonUtility.GetDateTimeValue(fields["ExpDate"]),
                    Used = CommonUtility.GetBooleanValue(fields["Used"]),
                    Void = CommonUtility.GetBooleanValue(fields["Void"])
                };
            }
            Performancelog.Debug($"End,SaleService,GetCoupon,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return null;
        }

        /// <summary>
        /// Method to add a coupon
        /// </summary>
        /// <param name="coupon">Coupon</param>
        public void AddCoupon(Coupon coupon)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddCoupon,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from Coupon where CouponID=\'" + coupon.CouponId.Trim() + "\'", _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                var fields = _dataTable.NewRow();
                fields["CouponID"] = coupon.CouponId.Trim();
                fields["Amount"] = coupon.Amount;
                fields["ExpDate"] = DateTime.FromOADate(DateAndTime.Today.ToOADate() + 180);
                fields["USED"] = false;
                fields["Void"] = false;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,AddCoupon,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to update a coupon
        /// </summary>
        /// <param name="coupon"></param>
        public void UpdateCoupon(Coupon coupon)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,UpdateCoupon,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from Coupon where CouponID=\'" + coupon.CouponId.Trim() + "\'", _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                var fields = _dataTable.Rows[0];
                fields["CouponID"] = coupon.CouponId.Trim();
                fields["Amount"] = coupon.Amount;
                fields["ExpDate"] = DateTime.FromOADate(DateAndTime.Today.ToOADate() + 180);
                fields["USED"] = coupon.Used;
                fields["Void"] = coupon.Void;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,UpdateCoupon,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to add kit charge
        /// </summary>
        /// <param name="charge">Charge</param>
        public void AddKitCharge(Charge charge)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddKitCharge,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (charge != null)
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter("SELECT * from SaleKitChg where TILL_NUM=" + charge.TillNumber, _connection);
                _adapter.Fill(_dataTable);
                var sKitChgFields = _dataTable.NewRow();
                sKitChgFields["Till_Num"] = charge.TillNumber;
                sKitChgFields["sale_no"] = charge.SaleNumber;
                sKitChgFields["Line_No"] = charge.LineNumber;
                sKitChgFields["Kit_Item"] = charge.KitItem;
                sKitChgFields["As_Code"] = charge.AsCode;
                sKitChgFields["Description"] = charge.Description;
                sKitChgFields["Quantity"] = charge.Quantity;
                sKitChgFields["price"] = charge.Charge_Price;
                sKitChgFields["Amount"] = charge.Amount;
                _dataTable.Rows.Add(sKitChgFields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
                Performancelog.Debug($"End,SaleService,AddKitCharge,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            }
        }

        #endregion



        /// <summary>
        /// Method to add sale head to CSCTills
        /// </summary>
        /// <param name="saleHead">Sale head</param>
        public void AddSaleHeadToDbTill(SaleHead saleHead)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,AddSaleHeadToDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from SALEHEAD where TILL=" + saleHead.TillNumber + " AND SALE_NO =" + saleHead.SaleNumber, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                var fields = _dataTable.NewRow();
                fields["SALE_NO"] = saleHead.SaleNumber;
                fields["TILL"] = saleHead.TillNumber;
                fields["T_TYPE"] = saleHead.TType;
                fields["TEND_LINES"] = saleHead.TendLine;
                fields["TEND_AMT"] = saleHead.TenderAmount;
                fields["TreatyNumber"] = saleHead.TreatyNumber;
                fields["DISC_TYPE"] = saleHead.DiscountType;
                fields["MainTill_CloseNum"] = saleHead.MainTillCloseNum ?? "1";
                fields["Reason_Type"] = saleHead.ReasonType;
                fields["SALE_NO"] = saleHead.SaleNumber;
                fields["Reason"] = saleHead.Reason;
                fields["ReferenceNum"] = saleHead.RefernceNum;
                fields["REGIST"] = saleHead.Register;
                fields["USER"] = saleHead.User;
                fields["Upsell"] = saleHead.Upsell;
                fields["INVC_DISC"] = saleHead.INVCDiscount;
                fields["OverPayment"] = saleHead.OverPayment;
                fields["PayOut"] = saleHead.PayOut;
                fields["PO_CODE"] = saleHead.POCode;
                fields["PAYMENT"] = saleHead.Payment;
                fields["Penny_Adj"] = saleHead.PennyAdjust;
                fields["LOYL_POINT"] = saleHead.LoyalPoint;
                fields["ASSOC_AMT"] = saleHead.AssociatedAmount;
                fields["SD"] = saleHead.SD;
                fields["SALE_DATE"] = saleHead.SaleDate;
                fields["SALE_LINES"] = saleHead.SaleLine;
                fields["Shift"] = saleHead.Shift;
                fields["ShiftDate"] = saleHead.ShiftDate;
                fields["DEPOSIT"] = saleHead.Deposit;
                fields["LINE_DISC"] = saleHead.LineDiscount;
                fields["LOYL_BALANCE"] = saleHead.LoyaltyBalance;
                fields["LoyaltyCard"] = saleHead.LoyaltyCard;
                fields["CHANGE"] = saleHead.Change;
                fields["CLIENT"] = saleHead.Client;
                fields["Close_Num"] = saleHead.CloseNum;
                fields["CREDITS"] = saleHead.Credits;
                fields["STORE"] = saleHead.Store;
                fields["SALE_TIME"] = DateTime.Now;
                fields["SALE_AMT"] = saleHead.SaleAmount;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,AddSaleHeadToDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to set prepay from POS
        /// </summary>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="pumpId">Pump ID</param>
        /// <param name="amount">Amount</param>
        /// <param name="mop">MOP</param>
        /// <param name="positionId">Position Id</param>
        /// <returns>True or false</returns>
        public bool SetPrepaymentFromPos(int invoiceId, int tillNumber, short pumpId, float amount,
            byte mop, byte positionId)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,SetPrepaymentFromPOS,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var sSql = "SELECT * FROM PrepayGlobal WHERE pumpID = " + Convert.ToString(pumpId);
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter(sSql, _connection);
            _adapter.Fill(_dataTable);
            bool addNew = false;
            DataRow dRecsetFields;
            if (_dataTable.Rows.Count == 0)
            {
                addNew = true;
                dRecsetFields = _dataTable.NewRow();
                dRecsetFields["pumpID"] = pumpId;
            }
            else
            {
                dRecsetFields = _dataTable.Rows[0];
            }
            dRecsetFields["InvoiceID"] = invoiceId;
            dRecsetFields["MOP"] = mop;

            dRecsetFields["TillID"] = tillNumber;
            dRecsetFields["Amount"] = amount.ToString("###0.00");
            dRecsetFields["locked"] = false;
            dRecsetFields["PositionID"] = positionId;

            if (addNew)
            {
                _dataTable.Rows.Add(dRecsetFields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
            }
            else
            {
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
            }
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            Performancelog.Debug($"End,SaleService,SetPrepaymentFromPOS,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return true;
        }

        /// <summary>
        /// Method to track price change
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="stockCode">Stock code</param>
        /// <param name="origPrice">Original price</param>
        /// <param name="newPrice">New price</param>
        /// <param name="processType">Process type</param>
        /// <param name="pricenum">Price number</param>
        /// <param name="vendorId">Vendor Id</param>
        //Processtype PC--> Price check; SL--> Saleline; SC--> Stock Screen Cost Change(Price will change) SP-Stock Screen Price Change; VC- Stock Screen Switch Active Vendor; PR--> Product Category-Change Price;  FP- FuturePrice rollover
        public void Track_PriceChange(string userCode, string stockCode, double origPrice, double newPrice,
            string processType, byte pricenum, string vendorId = "")
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,Track_PriceChange,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (origPrice != newPrice)
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter("select * from PriceChange where Stock_Code ='" + stockCode + "' and Pricenum = " + pricenum, _connection);
                _adapter.Fill(_dataTable);
                var fields = _dataTable.NewRow();
                fields["TILL_NUM"] = 0;
                fields["Stock_Code"] = stockCode;
                fields["Employeeid"] = userCode;
                fields["OriginalPrice"] = Math.Round(origPrice, 2);
                fields["NewPrice"] = Math.Round(newPrice, 2);
                fields["Date"] = DateAndTime.Today;
                fields["Time"] = DateTime.Now; //Time
                fields["Pricenum"] = pricenum;
                fields["Process"] = processType;
                if (!string.IsNullOrEmpty(vendorId))
                {
                    fields["VendorID"] = vendorId;
                }
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
            }
            Performancelog.Debug($"End,SaleService,Track_PriceChange,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to get void number from current sale
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Method to get loyalty card
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        public string GetLoyaltyCardFromDbTemp(int saleNumber, DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetLoyaltyCardFromDbTemp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var strSql = "Select [LoyaltyCard] From SALEHEAD Where Sale_No = " + Convert.ToString(saleNumber);
            var rsTmp = GetRecords(strSql, dataSource);
            if (rsTmp != null && rsTmp.Rows.Count != 0)
            {
                var fields = rsTmp.Rows[0];
                return CommonUtility.GetStringValue(fields["LoyaltyCard"]);
            }
            Performancelog.Debug($"End,SaleService,GetLoyaltyCardFromDbTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return string.Empty;
        }

        /// <summary>
        /// Method to get sale tenders from CSCCurSale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of sale tender</returns>
        public List<SaleTend> GetSaleTendersFromDbTemp(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetSaleTendersFromDBTemp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var saleTends = new List<SaleTend>();
            var rs = GetRecords("SELECT * FROM SaleTend WHERE Sale_No = " + Convert.ToString(saleNumber)
                + " AND Till_Num=" + tillNumber, DataSource.CSCCurSale);
            foreach (DataRow fields in rs.Rows)
            {
                saleTends.Add(new SaleTend
                {
                    TillNumber = CommonUtility.GetIntergerValue(fields["Till_Num"]),
                    SaleNumber = CommonUtility.GetIntergerValue(fields["SALE_NO"]),
                    SequenceNumber = CommonUtility.GetIntergerValue(fields["Sequence"]),
                    TenderName = CommonUtility.GetStringValue(fields["TENDNAME"]),
                    TenderClass = CommonUtility.GetStringValue(fields["TENDCLAS"]),
                    AmountTend = CommonUtility.GetDecimalValue(fields["AMTTEND"]),
                    AmountUsed = CommonUtility.GetDecimalValue(fields["AMTUSED"]),
                    Exchange = CommonUtility.GetDecimalValue(fields["Exchange_Rate"]),
                    SerialNumber = CommonUtility.GetStringValue(fields["SERNUM"]),
                    CCardNumber = CommonUtility.GetStringValue(fields["CCARD_NUM"]),
                    CCardAPRV = CommonUtility.GetStringValue(fields["CCARD_APRV"]),
                    AuthUser = CommonUtility.GetStringValue(fields["AuthUser"]),

                });
            }
            Performancelog.Debug($"End,SaleService,GetSaleTendersFromDBTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return saleTends;
        }

        /// <summary>
        /// Method to get list of sale vendor coupon line
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Sale vendor coupon line</returns>
        public List<SaleVendorCouponLine> GetSaleVendorCoupons(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetSaleVendorCoupons,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var svc = new List<SaleVendorCouponLine>();
            var rsSaleVendorCoupon = GetRecords("select * from SaleVendorCoupon Where Till_NUM=" + tillNumber + " and SALE_NO=" + Convert.ToString(saleNumber),
                DataSource.CSCCurSale);

            foreach (DataRow fields in rsSaleVendorCoupon.Rows)
            {
                var svcLine = new SaleVendorCouponLine
                {
                    Line_Num = CommonUtility.GetShortValue(fields["Line_Num"]),
                    SeqNum = CommonUtility.GetShortValue(fields["SeqNumber"]),
                    CouponCode = CommonUtility.GetStringValue(fields["CouponCode"]),
                    CouponName = CommonUtility.GetStringValue(fields["CouponName"]),
                    UnitValue = CommonUtility.GetFloatValue(fields["UnitValue"]),
                    Quantity = CommonUtility.GetFloatValue(fields["Quantity"]),
                    TotalValue = CommonUtility.GetDecimalValue(fields["TotalValue"]),
                    SerialNumber = CommonUtility.GetStringValue(fields["SerialNumber"]),
                    TendDesc = CommonUtility.GetStringValue(fields["TendDesc"])
                };

                svc.Add(svcLine);
            }

            Performancelog.Debug($"End,SaleService,GetSaleVendorCoupons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return svc;
        }

        /// <summary>
        /// Method to get list of sale vendor coupon line from Data source
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Sale vendor coupon line</returns>
        public List<SaleVendorCouponLine> GetSaleVendorCouponsForReprint(int saleNumber,
            int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetSaleVendorCoupons,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var svc = new List<SaleVendorCouponLine>();
            var rsSaleVendorCoupon = GetRecords("select * from SaleVendorCoupon Where Till_NUM=" + tillNumber + " and SALE_NO=" + Convert.ToString(saleNumber),
                DataSource.CSCTills);
            if (rsSaleVendorCoupon.Rows.Count == 0)
            {
                rsSaleVendorCoupon = GetRecords("select * from SaleVendorCoupon Where Till_NUM=" + tillNumber + " and SALE_NO=" + Convert.ToString(saleNumber),
                DataSource.CSCTrans);
            }
            if (rsSaleVendorCoupon.Rows.Count == 0)
            {
                return null;
            }
            foreach (DataRow fields in rsSaleVendorCoupon.Rows)
            {
                var svcLine = new SaleVendorCouponLine
                {
                    Line_Num = CommonUtility.GetShortValue(fields["Line_Num"]),
                    SeqNum = CommonUtility.GetShortValue(fields["SeqNumber"]),
                    CouponCode = CommonUtility.GetStringValue(fields["CouponCode"]),
                    CouponName = CommonUtility.GetStringValue(fields["CouponName"]),
                    UnitValue = CommonUtility.GetFloatValue(fields["UnitValue"]),
                    Quantity = CommonUtility.GetFloatValue(fields["Quantity"]),
                    TotalValue = CommonUtility.GetDecimalValue(fields["TotalValue"]),
                    SerialNumber = CommonUtility.GetStringValue(fields["SerialNumber"]),
                    TendDesc = CommonUtility.GetStringValue(fields["TendDesc"])
                };

                svc.Add(svcLine);
            }

            Performancelog.Debug($"End,SaleService,GetSaleVendorCoupons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return svc;
        }


        /// <summary>
        /// Method to get card tender
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderName">Tender name</param>
        /// <returns>Tender</returns>
        public Tender GetCardTenderFromDbTemp(int saleNumber, int tillNumber, string tenderName)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetCardTenderFromDBTemp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var rcc = GetRecords("select * from cardTenders where sale_no = "
                + Convert.ToString(saleNumber) + " and tender_name = \'"
                + tenderName + "\'and Till_Num=" + tillNumber, DataSource.CSCCurSale);
            var td = new Tender();
            if (rcc.Rows.Count != 0)
            {
                var fields = rcc.Rows[0];
                td.Credit_Card.Crd_Type = CommonUtility.GetStringValue(fields["Card_Type"]);
                td.Credit_Card.Name = CommonUtility.GetStringValue(fields["Card_Name"]);
                td.Credit_Card.Cardnumber = CommonUtility.GetStringValue(fields["Card_Number"]);
                td.Credit_Card.Expiry_Date = CommonUtility.GetStringValue(fields["Expiry_Date"]);
                td.Credit_Card.Card_Swiped = CommonUtility.GetBooleanValue(fields["Swiped"]);
                td.Credit_Card.StoreAndForward = CommonUtility.GetBooleanValue(fields["Store_Forward"]);
                td.Credit_Card.Trans_Amount = Convert.ToSingle(fields["Amount"]);
                td.Credit_Card.Authorization_Number = CommonUtility.GetStringValue(fields["Approval_Code"]);
                td.Credit_Card.Sequence_Number = CommonUtility.GetStringValue(fields["SequenceNumber"]);
                td.Credit_Card.Result = CommonUtility.GetStringValue(fields["Result"]);
                td.Credit_Card.TerminalID = CommonUtility.GetStringValue(fields["TerminalID"]);
                td.Credit_Card.DebitAccount = CommonUtility.GetStringValue(fields["DebitAccount"]);
                td.Credit_Card.ResponseCode = CommonUtility.GetStringValue(fields["ResponseCode"]);
                td.Credit_Card.ApprovalCode = CommonUtility.GetStringValue(fields["ISOCode"]);
                td.Credit_Card.Trans_Date = CommonUtility.GetDateTimeValue(fields["TransactionDate"]);
                td.Credit_Card.Trans_Time = CommonUtility.GetDateTimeValue(fields["TransactionTime"]);
                td.Credit_Card.Receipt_Display = CommonUtility.GetStringValue(fields["ReceiptDisplay"]);
                td.Credit_Card.Language = CommonUtility.GetStringValue(fields["Language"]);
                td.Credit_Card.Customer_Name = CommonUtility.GetStringValue(fields["CustomerName"]);
                td.Credit_Card.Vechicle_Number = CommonUtility.GetStringValue(fields["VechicleNo"]);
                td.Credit_Card.Driver_Number = CommonUtility.GetStringValue(fields["DriverNo"]);
                td.Credit_Card.ID_Number = CommonUtility.GetStringValue(fields["IdentificationNo"]);
                td.Credit_Card.Odometer_Number = CommonUtility.GetStringValue(fields["Odometer"]);
                td.Credit_Card.usageType = CommonUtility.GetStringValue(fields["CardUsage"]);
                td.Credit_Card.Print_VechicleNo = CommonUtility.GetBooleanValue(fields["PrintVechicleNo"]);
                td.Credit_Card.Print_DriverNo = CommonUtility.GetBooleanValue(fields["PrintDriverNo"]);
                td.Credit_Card.Print_IdentificationNo = CommonUtility.GetBooleanValue(fields["PrintIdentificationNo"]);
                td.Credit_Card.Print_Usage = CommonUtility.GetBooleanValue(fields["PrintUsage"]);
                td.Credit_Card.Print_Odometer = CommonUtility.GetBooleanValue(fields["PrintOdometer"]);
                td.Credit_Card.Balance = CommonUtility.GetDecimalValue(fields["Balance"]);
                td.Credit_Card.Quantity = CommonUtility.GetDecimalValue(fields["Quantity"]);
                td.Credit_Card.CardProfileID = CommonUtility.GetStringValue(fields["CardProfileID"]);
                td.Credit_Card.PONumber = CommonUtility.GetStringValue(fields["PONumber"]); td.Credit_Card.Report = CommonUtility.GetStringValue(fields["Message"]);
            }
            Performancelog.Debug($"End,SaleService,GetCardTenderFromDBTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return td;
        }

        /// <summary>
        /// Method to get refund sale tender
        /// </summary>
        /// <param name="saleNo">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale tender</returns>
        public SaleTend GetRefundSaleTender(int saleNo, DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetRefundSaleTender,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var strSqlTend = "Select * From SaleTend Where  Sale_No = " + Convert.ToString(saleNo) + " AND AMTTEND<>0";

            var rsTend = GetRecords(strSqlTend, dataSource);
            if (rsTend == null || rsTend.Rows.Count == 0)
            {
                Performancelog.Debug($"End,SaleService,GetRefundSaleTender,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return null;
            }

            if (rsTend.Rows.Count != 1)
            {
                Performancelog.Debug($"End,SaleService,GetRefundSaleTender,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return null;
            }

            var fields = rsTend.Rows[0];
            Performancelog.Debug($"End,SaleService,GetRefundSaleTender,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return new SaleTend
            {
                CCardNumber = CommonUtility.GetStringValue(fields["CCARD_NUM"]),
                TenderClass = CommonUtility.GetStringValue(fields["TENDCLAS"]),
                TenderName = CommonUtility.GetStringValue(fields["TendName"])
            };

        }

        /// <summary>
        /// Method to get card tender
        /// </summary>
        /// <param name="saleNo">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Card tender</returns>
        public CardTender GetCardTender(int saleNo, DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetCardTender,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            CardTender cardTender = null;
            var strSqlTend = "Select * From CardTenders Where  Sale_No = " + Convert.ToString(saleNo) + " AND Amount<>0";

            var rsTend = GetRecords(strSqlTend, dataSource);
            if (rsTend != null && rsTend.Rows.Count != 0)
            {
                var fields = rsTend.Rows[0];
                cardTender = new CardTender
                {
                    CardType = CommonUtility.GetStringValue(fields["Card_Type"]),
                    CardName = CommonUtility.GetStringValue(fields["Card_Name"]),
                    Language = CommonUtility.GetStringValue(fields["Language"]),
                    Swiped = CommonUtility.GetBooleanValue(fields["Swiped"]),
                    ExpiryDate = CommonUtility.GetStringValue(fields["Expiry_Date"])
                };
            }
            Performancelog.Debug($"End,SaleService,GetCardTender,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return cardTender;
        }


        /// <summary>
        /// Get Sale From Current Sale
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <returns></returns>
        public Sale GetSale(int tillNumber, int saleNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetSale,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var rsHead = GetRecords("SELECT * FROM CurrentSale  WHERE  SaleNumber = " + Convert.ToString(saleNumber) + " " + " AND TillNumber = " + tillNumber, DataSource.CSCCurSale);

            if (rsHead == null || rsHead.Rows.Count == 0)
            {
                return null;
            }
            var fields = rsHead.Rows[0];

            byte[] mData = Convert.FromBase64String(CommonUtility.GetStringValue(fields["SaleObject"]));
            MemoryStream memorystreamd = new MemoryStream(mData);
            BinaryFormatter bfd = new BinaryFormatter();
            Sale sale = bfd.Deserialize(memorystreamd) as Sale;
            Performancelog.Debug($"End,SaleService,GetSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return sale;
        }


        /// <summary>
        /// Sale Sale 
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        /// <param name="sale"></param>
        public void SaveSale(int tillNumber, int saleNumber, Sale sale)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,SaveSale,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            // Serialize to a base 64 string
            var ws = new MemoryStream();
            var sf = new BinaryFormatter();
            sf.Serialize(ws, sale);
            var bytes = ws.GetBuffer();
            string encodedData = Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None);
            // Open Sales Transactions Recordsets
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCCurSale));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            DataRow fields;
            bool addNew = false;
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("Select * from CurrentSale where TillNumber=" + tillNumber + " and SaleNumber = " + saleNumber, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                addNew = true;
                fields = _dataTable.NewRow();
                fields["SaleNumber"] = CommonUtility.GetIntergerValue(sale.Sale_Num);
                fields["TillNumber"] = CommonUtility.GetIntergerValue(sale.TillNumber);
            }
            else
            {
                fields = _dataTable.Rows[0];
            }
            fields["SaleObject"] = encodedData;
            try
            {
                if (addNew)
                {
                    _dataTable.Rows.Add(fields);
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
            finally
            {
                _connection.Close();
                _adapter?.Dispose();
            }
            Performancelog.Debug($"End,SaleService,SaveSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }



        /// <summary>
        /// Get Sale From Current Sale By Till Number
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        public Sale GetSaleByTillNumber(int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetSaleByTillNumber,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var rsHead = GetRecords("SELECT * FROM CurrentSale  WHERE TillNumber = " + tillNumber, DataSource.CSCCurSale);

            if (rsHead == null || rsHead.Rows.Count == 0)
            {
                Performancelog.Debug($"End,SaleService,GetSaleByTillNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return null;
            }
            var fields = rsHead.Rows[0];
            var strSaleObj = CommonUtility.GetStringValue(fields["SaleObject"]).Trim('\0');
            var mData = Convert.FromBase64String(strSaleObj);
            var memorystreamd = new MemoryStream(mData);
            var bfd = new BinaryFormatter();
            var sale = bfd.Deserialize(memorystreamd) as Sale;
            Performancelog.Debug($"End,SaleService,GetSaleByTillNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return sale;
        }

        /// <summary>
        /// Method to save sale for sale vendor coupon
        /// </summary>
        /// <param name="saleVendorCoupon">Sale vendor coupon</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        public void SaveSaleForSaleVendorCoupon(SaleVendorCoupon saleVendorCoupon,
            int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,SaveSaleForSaleVendorCoupon,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            if (saleVendorCoupon == null)
            {
                Performancelog.Debug($"End,SaleService,SaveSaleForSaleVendorCoupon,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return;
            }
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));

            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            foreach (SaleVendorCouponLine tempLoopVarSvc in saleVendorCoupon.SVC_Lines)
            {
                var addNew = false;
                var svc = tempLoopVarSvc;
                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter("select * from SaleVendorCoupon where TILL_NUM="
                    + tillNumber + " AND SALE_NO=" + Convert.ToString(saleVendorCoupon.Sale_Num)
                    + " AND LINE_NUM=" + Convert.ToString(svc.Line_Num) + " AND SeqNumber="
                    + Convert.ToString(svc.SeqNum), _connection);
                _adapter.Fill(_dataTable);
                DataRow fields;
                if (_dataTable.Rows.Count == 0)
                {
                    addNew = true;
                    fields = _dataTable.NewRow();
                    fields["Till_Num"] = tillNumber;
                    fields["Sale_No"] = saleVendorCoupon.Sale_Num;
                    fields["Line_Num"] = svc.Line_Num;
                    fields["SeqNumber"] = svc.SeqNum;
                }
                else
                {
                    fields = _dataTable.Rows[0];
                }
                fields["CouponCode"] = svc.CouponCode;
                fields["CouponName"] = svc.CouponName;
                fields["UnitValue"] = svc.UnitValue;
                fields["Quantity"] = svc.Quantity;
                fields["TotalValue"] = svc.TotalValue;
                fields["SerialNumber"] = svc.SerialNumber;
                fields["TendDesc"] = svc.TendDesc;
                if (addNew)
                {
                    _dataTable.Rows.Add(fields);
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.InsertCommand = builder.GetInsertCommand();
                }
                else
                {
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                }
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
            }
            Performancelog.Debug($"End,SaleService,SaveSaleForSaleVendorCoupon,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to load tenders used in sale
        /// </summary>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Tenders list</returns>
        public List<SaleTend> LoadTendersInSale(int invoiceId, DataSource dataSource)
        {
            var tenders = new List<SaleTend>();
            var rsTend = GetRecords("SELECT SaleTend.TILL_NUM, SaleTend.SALE_NO,SaleTend.Sequence, "
                + " SaleTend.TENDNAME, SaleTend.TENDCLAS, SaleTend.AMTTEND, " + " SaleTend.AMTUSED, SaleTend.Exchange_Rate, SaleTend.SERNUM, "
                + " SaleTend.CCARD_NUM, SaleTend.CCARD_APRV, D.CLASSDESC " + " FROM SaleTend LEFT JOIN CSCMaster.dbo.TENDCLAS as [D]  "
                + " ON SaleTend.TENDCLAS = D.TENDCLASS " + " Where  SaleTend.Sale_No = " + System.Convert.ToString(invoiceId) + " ", dataSource);
            foreach (DataRow row in rsTend.Rows)
            {
                tenders.Add(new SaleTend
                {
                    SaleNumber = CommonUtility.GetIntergerValue(row["SALE_NO"]),
                    TillNumber = CommonUtility.GetIntergerValue(row["TILL_NUM"]),
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

            return tenders;
        }


        /// <summary>
        /// Method to get card tender
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderName">Tender name</param>
        /// <param name="dataSource">DataSource</param>
        /// <returns>Tender</returns>
        public Credit_Card GetCardTender(int saleNumber, int tillNumber, string tenderName, DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,SaleService,GetCardTender,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var rcc = GetRecords("select * from cardTenders where sale_no = "
                + Convert.ToString(saleNumber) + " and tender_name = \'"
                + tenderName + "\'and Till_Num=" + tillNumber, dataSource);
            Credit_Card td = null;
            if (rcc.Rows.Count != 0)
            {
                var fields = rcc.Rows[0];
                td.Crd_Type = CommonUtility.GetStringValue(fields["Card_Type"]);
                td.Name = CommonUtility.GetStringValue(fields["Card_Name"]);
                td.Cardnumber = CommonUtility.GetStringValue(fields["Card_Number"]);
                td.Expiry_Date = CommonUtility.GetStringValue(fields["Expiry_Date"]);
                td.Card_Swiped = CommonUtility.GetBooleanValue(fields["Swiped"]);
                td.Authorization_Number = CommonUtility.GetStringValue(fields["Approval_Code"]);
                td.Result = CommonUtility.GetStringValue(fields["Result"]);
                td.Language = CommonUtility.GetStringValue(fields["Language"]);
                td.Customer_Name = CommonUtility.GetStringValue(fields["CustomerName"]);
            }
            Performancelog.Debug($"End,SaleService,GetCardTender,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return td;
        }
    }
}

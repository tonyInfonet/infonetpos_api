using System;
using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;
using log4net;
using Infonet.CStoreCommander.Logging;
using System.Data.SqlClient;
using System.Data;

namespace Infonet.CStoreCommander.ADOData
{
    public class SaleLineService : SqlDbService, ISaleLineService
    {
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        /// <summary>
        /// Method to add sale line kit
        /// </summary>
        /// <param name="lineKit"></param>
        public void AddSaleLineKit(Line_Kit lineKit)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleLineService,AddSaleLineKit,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from SaleKit where TILL_NUM= " + lineKit.TillNumber + " and sale_no = " + lineKit.SaleNumber + " and Line_No = " + lineKit.LineNumber, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
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
                _performancelog.Debug($"End,SaleLineService,AddSaleLineKit,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            }
        }

        /// <summary>
        /// Method to add a sale line reason
        /// </summary>
        /// <param name="reason">Sale line reason</param>
        public void AddSaleLineReason(SaleLineReason reason)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleLineService,AddSaleLineReason,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from SLineReason where TILL_NUM= " + reason.TillNumber +
                " and sale_no = " + reason.SaleNumber +
                " and Line_Num = " + reason.LineNumber +
                " and Reason_Type = " + reason.ReasonType, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
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
                _performancelog.Debug($"End,SaleLineService,AddSaleLineReason,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            }
        }

        /// <summary>
        /// Method to add sale line tax
        /// </summary>
        /// <param name="lineTax">Line tax</param>
        public void AddSaleLineTax(Line_Tax lineTax)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleLineService,AddSaleLineTax,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from S_LineTax where TILL_NUM=" + lineTax.TillNumber +
                " and Sale_No = " + lineTax.SaleNumber +
                 " and Line_No = " + lineTax.LineNumber +
                  " and Tax_Name = " + lineTax.Tax_Name, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
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
            }
            _performancelog.Debug($"End,SaleLineService,AddSaleLineTax,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to add a sale line to current sale
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        public void AddSaleLineToDbTill(Sale_Line saleLine)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleLineService,AddSaleLineToDbTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * from SALELINE where TILL_NUM=" + saleLine.Till_Num +
                " and SALE_NO = " + saleLine.Sale_Num +
                 " and Line_Num = " + saleLine.Line_Num, _connection);
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
                fields["Disc_Type"] = saleLine.Discount_Type;
                fields["Disc_Adj"] = saleLine.Discount_Adjust;
                fields["Disc_Code"] = saleLine.Discount_Code;
                fields["Disc_Rate"] = saleLine.Discount_Rate;
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
                fields["TaxExempt"] = saleLine.TaxForTaxExempt;
                fields["TOTAL_AMT"] = saleLine.Total_Amount;
                fields["Rebate"] = saleLine.Rebate;
                fields["ScalableItem"] = saleLine.ScalableItem;
                fields["CardProfileID"] = saleLine.CardProfileID;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
            }
            _performancelog.Debug($"End,SaleLineService,AddSaleLineToDbTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }



        /// <summary>
        /// Get Sale lines 
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <param name="userCode"></param>
        /// <returns>List of sale lines</returns>
        public List<Sale_Line> GetSaleLinesFromDbTemp(int saleNumber, int tillNumber, string userCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,SaleLineService,GetSaleLinesFromDbTemp,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var saleLines = new List<Sale_Line>();

            var rsLine = GetRecords("SELECT * FROM   SaleLine WHERE  SaleLine.Sale_No = " + Convert.ToString(saleNumber) + " and   SaleLine.Till_Num = " + tillNumber + " ORDER BY SaleLine.Line_Num ", DataSource.CSCCurSale);
            var rsReason = GetRecords("SELECT * FROM SLineReason WHERE SLineReason.Sale_No = " + Convert.ToString(saleNumber) + " and   SLineReason.Till_Num = " + tillNumber + " ORDER BY SLineReason.Line_Num", DataSource.CSCCurSale);
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
                // Set this property here, otherwise the stock item is a mess in POS screen and no others properties like description, dept, sub_dept, etc are set
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
                        : lineFields["DiscountName"]);
                saleLine.CardProfileID =
                    Convert.ToString(DBNull.Value.Equals(lineFields["CardProfileID"])
                        ? 0
                        : lineFields["CardProfileID"]);
                saleLine.No_Loading = false;
                // It has to be set back to false, otherwise if the user changes the qty or price in the POS screen after unsuspend, the amount will not be right
                // Nicolette end

                var strPromo =
                    Convert.ToString(DBNull.Value.Equals(lineFields["PromoID"])
                        ? ""
                        : lineFields["PromoID"]);
                if (strPromo.Length != 0)
                {
                    saleLine.PromoID = strPromo; //  
                }

                if (rsReason != null)
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
            _performancelog.Debug($"End,SaleLineService,GetSaleLinesFromDbTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return saleLines;
        }


    }
}

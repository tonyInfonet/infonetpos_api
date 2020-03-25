using Infonet.CStoreCommander.Entities;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Infonet.CStoreCommander.ADOData
{
    public class TaxExemptService : SqlDbService, ITaxExemptService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        /// <summary>
        /// Method to save tax exempt sale head
        /// </summary>
        /// <param name="oteSale">Tax exempt sale</param>
        /// <param name="shiftDate">Shift date</param>
        public void SaveTaxExemptSaleHead(TaxExemptSale oteSale, DateTime shiftDate)
        {
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            bool addNew = false;

            
            _dataTable = new DataTable();
            var query = "select * from TaxExemptSaleHead " + " where SALE_NO=" + Convert.ToString(oteSale.Sale_Num) +
                        " AND TILL_NUM=" + Convert.ToString(oteSale.TillNumber);
            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            DataRow rsTeSaleHead;
            if (_dataTable.Rows.Count == 0)
            {
                rsTeSaleHead = _dataTable.NewRow();
                rsTeSaleHead["sale_no"] = oteSale.Sale_Num;
                rsTeSaleHead["Till_Num"] = oteSale.TillNumber;
                addNew = true;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
            }
            else
            {
                rsTeSaleHead = _dataTable.Rows[0];
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
            }
            rsTeSaleHead["Shift"] = oteSale.Shift;
            rsTeSaleHead["User"] = oteSale.UserCode;
            rsTeSaleHead["CardholderID"] = oteSale.teCardholder.CardholderID;
            rsTeSaleHead["Barcode"] = oteSale.teCardholder.Barcode;
            rsTeSaleHead["cardnumber"] = oteSale.teCardholder.CardNumber;
            rsTeSaleHead["ShiftDate"] = shiftDate;
            rsTeSaleHead["SaleTime"] = oteSale.Sale_Time; // Now
            rsTeSaleHead["SaleAmount"] = oteSale.Amount;
            rsTeSaleHead["SaleLines"] = oteSale.Te_Sale_Lines.Count;
            rsTeSaleHead["GasReason"] = oteSale.GasReason;
            rsTeSaleHead["GasReasonDesp"] = oteSale.GasReasonDesp;
            rsTeSaleHead["GasReasonDetail"] = oteSale.GasReasonDetail;
            rsTeSaleHead["PropaneReason"] = oteSale.PropaneReason;
            rsTeSaleHead["PropaneReasonDesp"] = oteSale.PropaneReasonDesp;
            rsTeSaleHead["PropaneReasonDetail"] = oteSale.PropaneReasonDetail;
            rsTeSaleHead["TobaccoReason"] = oteSale.TobaccoReason;
            rsTeSaleHead["TobaccoReasonDesp"] = oteSale.TobaccoReasonDesp;
            rsTeSaleHead["TobaccoReasonDetail"] = oteSale.TobaccoReasonDetail;
            rsTeSaleHead["TotalExemptedTax"] = oteSale.TotalExemptedTax;
            if (addNew)
            {
                _dataTable.Rows.Add(rsTeSaleHead);
            }
            _adapter.Update(_dataTable);
            
            foreach (TaxExemptSaleLine tempLoopVarTesl in oteSale.Te_Sale_Lines)
            {
                addNew = false;
                var tesl = tempLoopVarTesl;
                _dataTable = new DataTable();
                query = "select * from TaxExemptSaleLine where SALE_NO=" + Convert.ToString(oteSale.Sale_Num)
                        + " AND Till_NUM=" + Convert.ToString(oteSale.TillNumber) + " AND LINE_NUM="
                        + Convert.ToString(tesl.Line_Num);
                _adapter = new SqlDataAdapter(query, _connection);
                _adapter.Fill(_dataTable);
                DataRow rsTeSaleLine;
                if (_dataTable.Rows.Count == 0)
                {
                    rsTeSaleLine = _dataTable.NewRow();
                    rsTeSaleLine["Till_Num"] = oteSale.TillNumber;
                    rsTeSaleLine["sale_no"] = oteSale.Sale_Num;
                    rsTeSaleLine["Line_Num"] = tesl.Line_Num;
                    addNew = true;
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.InsertCommand = builder.GetInsertCommand();
                }
                else
                {
                    rsTeSaleLine = _dataTable.Rows[0];
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                }
                rsTeSaleLine["Stock_Code"] = tesl.StockCode;
                rsTeSaleLine["ProductType"] = tesl.ProductType;
                rsTeSaleLine["ProductCode"] = tesl.ProductCode;
                rsTeSaleLine["Description"] = tesl.Description;
                rsTeSaleLine["Quantity"] = tesl.Quantity;
                rsTeSaleLine["UnitQuantity"] = tesl.UnitsPerPkg;
                rsTeSaleLine["EquvQuantity"] = tesl.EquvQuantity;
                rsTeSaleLine["price"] = tesl.TaxFreePrice;
                rsTeSaleLine["OriginalPrice"] = tesl.OriginalPrice;
                rsTeSaleLine["TaxIncludedAmount"] = tesl.TaxInclPrice;
                rsTeSaleLine["Amount"] = tesl.Amount;
                rsTeSaleLine["ExemptedTax"] = tesl.ExemptedTax;
                rsTeSaleLine["RunningQuota"] = tesl.RunningQuota;
                rsTeSaleLine["OverLimit"] = tesl.OverLimit;
                rsTeSaleLine["TaxExemptRate"] = tesl.TaxExemptRate;
                if (addNew)
                {
                    _dataTable.Rows.Add(rsTeSaleLine);
                }
                _adapter.Update(_dataTable);

            }
            _connection.Close();
            _adapter?.Dispose();
            
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            query = "select * from TaxExemptCardRegistry " + " where CardholderID=\'" +
                    oteSale.teCardholder.CardholderID + "\'";

            var taxExemptRegistry = GetRecords(query, DataSource.CSCMaster);
            if (taxExemptRegistry.Rows.Count != 0)
            {
                var rsCardHolder = taxExemptRegistry.Rows[0];
                var tobaccoQuota =
                    CommonUtility.GetFloatValue(CommonUtility.GetFloatValue(rsCardHolder["TobaccoQuota"]) +
                                     oteSale.TotalExemptTobacco).ToString("#0.00");
                var gasQuota =
                    CommonUtility.GetFloatValue(CommonUtility.GetFloatValue(rsCardHolder["GasQuota"]) + oteSale.TotalExemptGas)
                        .ToString("#0.00");
                var propaneQuota =
                    CommonUtility.GetFloatValue(CommonUtility.GetFloatValue(rsCardHolder["PropaneQuota"]) +
                                     oteSale.TotalExemptPropane)
                        .ToString("#0.00");
                var updateCommand =
                    $"update TaxExemptCardRegistry set TobaccoQuota= {tobaccoQuota}, GasQuota = {gasQuota}, PropaneQuota = {propaneQuota} where CardholderID='{oteSale.teCardholder.CardholderID}'";
                Execute(updateCommand, DataSource.CSCMaster);
            }

        }

        /// <summary>
        /// Method to save tax credit
        /// </summary>
        /// <param name="oteSale">Tax exempt sale</param>
        public void SaveTaxCredit(TaxExemptSale oteSale)
        {
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            
            bool addNew;
            foreach (Sale_Tax tempLoopVarSx in oteSale.TaxCredit)
            {
                var sx = tempLoopVarSx;
                if (!(sx.Taxable_Amount != 0 | sx.Tax_Included_Amount != 0)) continue;
                addNew = false;
                _dataTable = new DataTable();
                var query = "SELECT * from TaxCredit where TILL_NUM=" + oteSale.TillNumber + " AND SALE_NO=" + Convert.ToString(oteSale.Sale_Num) + " AND Tax_Name=\'" + sx.Tax_Name + "\' AND " + " Tax_Code=\'" + sx.Tax_Code + "\' ";
                _adapter = new SqlDataAdapter(query, _connection);
                _adapter.Fill(_dataTable);
                DataRow rsTaxCredit;
                if (_dataTable.Rows.Count == 0)
                {
                    rsTaxCredit = _dataTable.NewRow();
                    rsTaxCredit["Till_Num"] = oteSale.TillNumber;
                    rsTaxCredit["sale_no"] = oteSale.Sale_Num;
                    rsTaxCredit["Tax_Name"] = sx.Tax_Name;
                    rsTaxCredit["Tax_Code"] = sx.Tax_Code;
                    addNew = true;
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.InsertCommand = builder.GetInsertCommand();
                }
                else
                {
                    rsTaxCredit = _dataTable.Rows[0];
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                }
                rsTaxCredit["Tax_Rate"] = sx.Tax_Rate;
                rsTaxCredit["Taxable_Amount"] = sx.Taxable_Amount;
                rsTaxCredit["Tax_Added_Amount"] = sx.Tax_Added_Amount;
                rsTaxCredit["Tax_Included_Amount"] = sx.Tax_Included_Amount;
                rsTaxCredit["Tax_Included_Total"] = sx.Tax_Included_Total;
                if (addNew)
                {
                    _dataTable.Rows.Add(rsTaxCredit);
                }
                _adapter.Update(_dataTable);
            }
            foreach (TaxCreditLine tempLoopVarTx in oteSale.TaxCreditLines)
            {
                var tx = tempLoopVarTx;
                foreach (Line_Tax tempLoopVarLt in tx.Line_Taxes)
                {
                    addNew = false;
                    _dataTable = new DataTable();
                    var lt = tempLoopVarLt;
                    var query = "SELECT * from TaxCreditLine where TILL_NUM=" + oteSale.TillNumber + " AND SALE_NO=" + Convert.ToString(oteSale.Sale_Num) + " AND Tax_Name=\'" + lt.Tax_Name + "\' AND " + " Line_No=" + Convert.ToString(tx.Line_Num);
                    _adapter = new SqlDataAdapter(query, _connection);
                    _adapter.Fill(_dataTable);
                    DataRow rsTaxCreditLine;
                    if (_dataTable.Rows.Count == 0)
                    {
                        rsTaxCreditLine = _dataTable.NewRow();
                        rsTaxCreditLine["Till_Num"] = oteSale.TillNumber;
                        rsTaxCreditLine["sale_no"] = oteSale.Sale_Num;
                        rsTaxCreditLine["Line_No"] = tx.Line_Num;
                        rsTaxCreditLine["Tax_Name"] = lt.Tax_Name;
                        addNew = true;
                        SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                        _adapter.InsertCommand = builder.GetInsertCommand();
                    }
                    else
                    {
                        rsTaxCreditLine = _dataTable.Rows[0];
                        SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                        _adapter.UpdateCommand = builder.GetUpdateCommand();
                    }
                    rsTaxCreditLine["Tax_Code"] = lt.Tax_Code;
                    rsTaxCreditLine["Tax_Rate"] = lt.Tax_Rate;
                    rsTaxCreditLine["Tax_Included"] = lt.Tax_Included;
                    rsTaxCreditLine["Tax_Added_Amount"] = lt.Tax_Added_Amount;
                    rsTaxCreditLine["Tax_Included_Amount"] = lt.Tax_Incl_Amount;
                    if (addNew)
                    {
                        _dataTable.Rows.Add(rsTaxCreditLine);
                    }
                    _adapter.Update(_dataTable);
                }
            }
            _adapter?.Dispose();
            _connection.Close();
        }

        /// <summary>
        /// Method to get item UPC by stock code
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Item upc</returns>
        public string GetItemUpc(string stockCode)
        {
            var rs = GetRecords("SELECT ItemUPC FROM SITELookUp WHERE StockCode=\'" + stockCode + "\'", DataSource.CSCMaster);

            if (rs.Rows.Count == 0)
            {
                return stockCode;
            }
            return DBNull.Value.Equals(rs.Rows[0]["ItemUPC"]) ? stockCode : CommonUtility.GetStringValue(rs.Rows[0]["ItemUPC"]);
        }

        /// <summary>
        /// Method to update purchase item
        /// </summary>
        /// <param name="query">Query</param>
        public void UpdatePurchaseItem(string query)
        {
            Execute(query, DataSource.CSCTills);
        }

        /// <summary>
        /// Method to update treaty number
        /// </summary>
        /// <param name="query">Query</param>
        public void UpdateTreatyNo(string query)
        {
            Execute(query, DataSource.CSCMaster);
        }


        /// <summary>
        /// Method to load tax exempt
        /// </summary>
        /// <param name="teType">Tax exemption type</param>
        /// <param name="sn">Sale number</param>
        /// <param name="tillId">Till Number</param>
        /// <param name="db">Data base</param>
        /// <param name="checkQuota">Check Quota or not</param>
        /// <returns>Tax exempt sale</returns>
        public TaxExemptSale LoadTaxExempt(string teType, int sn, byte tillId, DataSource db,
            bool checkQuota = true)
        {
            var taxExemptSale = new TaxExemptSale();

            if (LoadGstExempt(ref taxExemptSale, sn, tillId, db))
            {
                taxExemptSale.Sale_Num = sn;
                taxExemptSale.TillNumber = tillId;
            }

            var rsHead = GetRecords("select * from TaxExemptSaleHead " + " where SALE_NO=" + Convert.ToString(sn) + " AND TILL_NUM=" + Convert.ToString(tillId), db);
            var rsLine = GetRecords("select * from TaxExemptSaleLine " + " where SALE_NO=" + Convert.ToString(sn) + " AND TILL_NUM=" + Convert.ToString(tillId), db);
            if (teType == "QITE")
            {
                if (rsHead.Rows.Count == 0)
                {
                    return null;
                }
            }
            else
            {
                if (rsHead.Rows.Count == 0 || rsLine.Rows.Count == 0)
                {
                    return null;
                }
            }
            var rsHeadField = rsHead.Rows[0];
            taxExemptSale.Sale_Num = sn;
            taxExemptSale.TillNumber = tillId;

            foreach (DataRow fields in rsLine.Rows)
            {
                var mTeLine = new TaxExemptSaleLine
                {
                    Quantity = CommonUtility.GetFloatValue(fields["Quantity"]),
                    UnitsPerPkg = CommonUtility.GetFloatValue(fields["UnitQuantity"]),
                    EquvQuantity = CommonUtility.GetFloatValue(fields["EquvQuantity"]),
                    OriginalPrice = CommonUtility.GetFloatValue(fields["OriginalPrice"]),
                    TaxFreePrice = CommonUtility.GetFloatValue(fields["price"]),
                    Line_Num = CommonUtility.GetShortValue(fields["Line_Num"]),
                    StockCode = CommonUtility.GetStringValue(fields["Stock_Code"]),
                    TaxInclPrice = CommonUtility.GetFloatValue(fields["TaxIncludedAmount"]),
                    Amount = CommonUtility.GetFloatValue(fields["Amount"]),
                    ExemptedTax = CommonUtility.GetFloatValue(fields["ExemptedTax"]),
                    Description = CommonUtility.GetStringValue(fields["Description"]),
                    ProductCode = CommonUtility.GetStringValue(fields["ProductCode"])
                };
                mPrivateGlobals.teProductEnum productType;
                Enum.TryParse(CommonUtility.GetStringValue(fields["ProductType"]), out productType);
                mTeLine.ProductType = productType;
                mTeLine.RunningQuota = CommonUtility.GetFloatValue(fields["RunningQuota"]);

                mTeLine.OverLimit = CommonUtility.GetBooleanValue(fields["OverLimit"]);

                mTeLine.TaxExemptRate = CommonUtility.GetFloatValue(fields["TaxExemptRate"]);
                var tempCheckOverLimit = false;
                taxExemptSale.Add_a_Line(mTeLine, ref tempCheckOverLimit, ref checkQuota);
                if (!mTeLine.OverLimit) continue;
                switch (mTeLine.ProductType)
                {
                    case mPrivateGlobals.teProductEnum.eCigarette:
                    case mPrivateGlobals.teProductEnum.eCigar:
                    case mPrivateGlobals.teProductEnum.eLooseTobacco:
                        taxExemptSale.TobaccoOverLimit = true;
                        break;
                    case mPrivateGlobals.teProductEnum.eGasoline:
                    case mPrivateGlobals.teProductEnum.eDiesel:
                    case mPrivateGlobals.teProductEnum.emarkedGas:
                    case mPrivateGlobals.teProductEnum.emarkedDiesel:

                        taxExemptSale.GasOverLimit = true;
                        break;
                    case mPrivateGlobals.teProductEnum.ePropane:
                        taxExemptSale.PropaneOverLimit = true;
                        break;
                    case mPrivateGlobals.teProductEnum.eNone:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            taxExemptSale.Amount = CommonUtility.GetFloatValue(rsHeadField["SaleAmount"]);
            taxExemptSale.ShiftDate = CommonUtility.GetDateTimeValue(rsHeadField["ShiftDate"]);
            taxExemptSale.Sale_Time = CommonUtility.GetDateTimeValue(rsHeadField["SaleTime"]);
            taxExemptSale.GasReason = CommonUtility.GetStringValue(rsHeadField["GasReason"]);
            taxExemptSale.GasReasonDesp = CommonUtility.GetStringValue(rsHeadField["GasReasonDesp"]);
            taxExemptSale.GasReasonDetail = CommonUtility.GetStringValue(rsHeadField["GasReasonDetail"]);
            taxExemptSale.PropaneReason = CommonUtility.GetStringValue(rsHeadField["PropaneReason"]);
            taxExemptSale.PropaneReasonDesp = CommonUtility.GetStringValue(rsHeadField["PropaneReasonDesp"]);
            taxExemptSale.PropaneReasonDetail = CommonUtility.GetStringValue(rsHeadField["PropaneReasonDetail"]);
            taxExemptSale.TobaccoReason = CommonUtility.GetStringValue(rsHeadField["TobaccoReason"]);
            taxExemptSale.TobaccoReasonDesp = CommonUtility.GetStringValue(rsHeadField["TobaccoReasonDesp"]);
            taxExemptSale.TobaccoReasonDetail = CommonUtility.GetStringValue(rsHeadField["TobaccoReasonDetail"]);
            taxExemptSale.Shift = CommonUtility.GetShortValue(rsHeadField["Shift"]);
            taxExemptSale.UserCode = CommonUtility.GetStringValue(rsHeadField["User"]);
            taxExemptSale.teCardholder.CardholderID = CommonUtility.GetStringValue(rsHeadField["CardholderID"]);
            taxExemptSale.teCardholder.Barcode = CommonUtility.GetStringValue(rsHeadField["Barcode"]);
            taxExemptSale.teCardholder.CardNumber = CommonUtility.GetStringValue(rsHeadField["cardnumber"]);
            if (teType == "QITE")
            {
                var rsCardHolder = GetRecords("select * from CLIENT " + " where CL_CODE=\'" + taxExemptSale.teCardholder.CardholderID + "\'", DataSource.CSCMaster);
                if (rsCardHolder.Rows.Count > 0)
                {
                    taxExemptSale.teCardholder.Name = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["Cl_Name"]);
                    taxExemptSale.teCardholder.Address = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["CL_Add1"]);
                    taxExemptSale.teCardholder.City = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["CL_City"]);
                    taxExemptSale.teCardholder.PlateNumber = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["PlateNumber"]);
                    taxExemptSale.teCardholder.PostalCode = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["CL_Postal"]);
                }
            }
            else // For AITE
            {
                var rsCardHolder = GetRecords("select * from TaxExemptCardRegistry " + " where CardholderID=\'" + taxExemptSale.teCardholder.CardholderID + "\'", DataSource.CSCMaster);
                if (rsCardHolder.Rows.Count > 0)
                {
                    taxExemptSale.teCardholder.Name = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["Name"]);
                }
            }
            return taxExemptSale;
        }



        /// <summary>
        /// Method to load GST exempt
        /// </summary>
        /// <param name="taxExemptSale">Tax exempt sale</param>
        /// <param name="sn">Sale number</param>
        /// <param name="tillId">Till number</param>
        /// <param name="db">Data source</param>
        /// <returns>True or false</returns>
        public bool LoadGstExempt(ref TaxExemptSale taxExemptSale, int sn, byte tillId,
            DataSource db)
        {
            var rsLine = GetRecords("select * from SALELINE " + " where SALE_NO=" + Convert.ToString(sn) + " AND TILL_NUM=" + Convert.ToString(tillId), db);
            if (rsLine.Rows.Count == 0)
            {
                return false;
            }

            foreach (DataRow line in rsLine.Rows)
            {
                
                var rsTaxCreditLine = GetRecords("SELECT * from TaxCreditLine where TILL_NUM=" + Convert.ToString(tillId) + " AND SALE_NO=" + Convert.ToString(sn) + " AND Line_No=" + Convert.ToString(line["Line_Num"]), db);
                if (rsTaxCreditLine.Rows.Count == 0) continue;
                var tx = new TaxCreditLine { Line_Num = CommonUtility.GetShortValue(line["Line_Num"]) };
                foreach (DataRow taxCreditLine in rsTaxCreditLine.Rows)
                {
                    var lt = new Line_Tax
                    {
                        Tax_Name = CommonUtility.GetStringValue(taxCreditLine["Tax_Name"]),
                        Tax_Code = CommonUtility.GetStringValue(taxCreditLine["Tax_Code"]),
                        Tax_Rate = CommonUtility.GetFloatValue(taxCreditLine["Tax_Rate"]),
                        Tax_Included = CommonUtility.GetBooleanValue(taxCreditLine["Tax_Included"]),
                        Tax_Added_Amount = CommonUtility.GetFloatValue(taxCreditLine["Tax_Added_Amount"]),
                        Tax_Incl_Amount = CommonUtility.GetFloatValue(taxCreditLine["Tax_Included_Amount"])
                    };
                    tx.Line_Taxes.AddTaxLine(lt, "");
                }
                taxExemptSale.TaxCreditLines.AddLine(tx.Line_Num, tx, "");
            }

            
            var rsTaxCredit = GetRecords("SELECT * from TaxCredit where TILL_NUM=" + Convert.ToString(tillId)
                + " AND SALE_NO=" + Convert.ToString(sn), db);
            foreach (DataRow taxCredit in rsTaxCredit.Rows)
            {
                var sx = new Sale_Tax
                {
                    Tax_Name = CommonUtility.GetStringValue(taxCredit["Tax_Name"]),
                    Tax_Code = CommonUtility.GetStringValue(taxCredit["Tax_Code"]),
                    Tax_Rate = CommonUtility.GetFloatValue(taxCredit["Tax_Rate"]),
                    Taxable_Amount = CommonUtility.GetDecimalValue(taxCredit["Taxable_Amount"]),
                    Tax_Added_Amount = CommonUtility.GetDecimalValue(taxCredit["Tax_Added_Amount"]),
                    Tax_Included_Amount = CommonUtility.GetDecimalValue(taxCredit["Tax_Included_Amount"]),
                    Tax_Included_Total = CommonUtility.GetDecimalValue(taxCredit["Tax_Included_Total"])
                };
                taxExemptSale.TaxCredit.Add(sx.Tax_Name, sx.Tax_Code, sx.Tax_Rate, sx.Taxable_Amount, sx.Tax_Added_Amount, sx.Tax_Included_Amount, sx.Tax_Included_Amount, sx.Tax_Rebate_Rate, sx.Tax_Rebate, sx.Tax_Name + sx.Tax_Code); //   - gave mismatch type error for AITE
            }
            return true;
        }


        /// <summary>
        /// Method to loadGST exempt for delete prepay
        /// </summary>
        /// <param name="taxExemptSale">Tax exempt sale</param>
        /// <param name="sn">Sale number</param>
        /// <param name="tillId">Till number</param>
        /// <param name="db">Data source</param>
        /// <returns>True or false</returns>
        public bool LoadGstExemptForDeletePrepay(ref TaxExemptSale taxExemptSale, int sn,
            byte tillId, DataSource db)
        {
            var returnValue = false;

            var rsLine = GetRecords("select * from SALELINE " + " where SALE_NO=" + Convert.ToString(sn) + " AND TILL_NUM=" + Convert.ToString(tillId), db);
            if (rsLine.Rows.Count == 0)
            {
                return false;
            }

            foreach (DataRow line in rsLine.Rows)
            {
                
                var rsTaxCreditLine = GetRecords("SELECT * from TaxCreditLine where TILL_NUM=" + Convert.ToString(tillId) + " AND SALE_NO=" + Convert.ToString(sn) + " AND Line_No=" + Convert.ToString(line["Line_Num"]), db);
                if (rsTaxCreditLine.Rows.Count != 0)
                {
                    var tx = new TaxCreditLine { Line_Num = CommonUtility.GetShortValue(line["Line_Num"]) };
                    foreach (DataRow taxCreditLine in rsTaxCreditLine.Rows)
                    {
                        var lt = new Line_Tax
                        {
                            Tax_Name = CommonUtility.GetStringValue(taxCreditLine["Tax_Name"]),
                            Tax_Code = CommonUtility.GetStringValue(taxCreditLine["Tax_Code"]),
                            Tax_Rate = CommonUtility.GetFloatValue(taxCreditLine["Tax_Rate"]),
                            Tax_Included =
                                Convert.ToBoolean(-1 * CommonUtility.GetIntergerValue(taxCreditLine["Tax_Included"])),
                            Tax_Added_Amount =
                                CommonUtility.GetFloatValue(-1 *
                                                 CommonUtility.GetIntergerValue(taxCreditLine["Tax_Added_Amount"])),
                            Tax_Incl_Amount =
                                CommonUtility.GetFloatValue(-1 *
                                                 CommonUtility.GetIntergerValue(taxCreditLine["Tax_Included_Amount"]))
                        };
                        tx.Line_Taxes.AddTaxLine(lt, "");
                    }
                    taxExemptSale.TaxCreditLines.AddLine(tx.Line_Num, tx, "");
                    returnValue = true;
                }
            }

            
            var rsTaxCredit = GetRecords("SELECT * from TaxCredit where TILL_NUM=" + Convert.ToString(tillId) + " AND SALE_NO=" + Convert.ToString(sn), db);
            foreach (DataRow taxCredit in rsTaxCredit.Rows)
            {
                var sx = new Sale_Tax
                {
                    Tax_Name = CommonUtility.GetStringValue(taxCredit["Tax_Name"]),
                    Tax_Code = CommonUtility.GetStringValue(taxCredit["Tax_Code"]),
                    Tax_Rate = CommonUtility.GetFloatValue(taxCredit["Tax_Rate"]),
                    Taxable_Amount =
                        Convert.ToDecimal(-1 * CommonUtility.GetIntergerValue(taxCredit["Taxable_Amount"])),
                    Tax_Added_Amount =
                        Convert.ToDecimal(-1 * CommonUtility.GetIntergerValue(taxCredit["Tax_Added_Amount"])),
                    Tax_Included_Amount =
                        Convert.ToDecimal(-1 * CommonUtility.GetIntergerValue(taxCredit["Tax_Included_Amount"])),
                    Tax_Included_Total =
                        Convert.ToDecimal(-1 * CommonUtility.GetIntergerValue(taxCredit["Tax_Included_Total"]))
                };
                taxExemptSale.TaxCredit.Add(sx.Tax_Name, sx.Tax_Code, sx.Tax_Rate, sx.Taxable_Amount, sx.Tax_Added_Amount, sx.Tax_Included_Amount, sx.Tax_Included_Amount, sx.Tax_Rebate_Rate, sx.Tax_Rebate, sx.Tax_Name + sx.Tax_Code); //   - gave mismatch type error for AITE
            }

            return returnValue;
        }


        /// <summary>
        /// Method to load tax exempt for delete preapay
        /// </summary>
        /// <param name="taxExemptSale">Tax exempt sale</param>
        /// <param name="teType">Tax exempt type</param>
        /// <param name="till">Till</param>
        /// <param name="user">User</param>
        /// <param name="sn">Sale number</param>
        /// <param name="tillId">Till number</param>
        /// <param name="lineNum">Line number</param>
        /// <param name="newSaleNo">New sale number</param>
        /// <returns>True or false</returns>
        public bool LoadTaxExemptForDeletePrepay(ref TaxExemptSale taxExemptSale,
            string teType, Till till, User user, int sn, byte tillId, short lineNum,
            int newSaleNo)
        {
            var rsHead = GetRecords("select * from TaxExemptSaleHead " + " where SALE_NO=" + Convert.ToString(sn) + " AND TILL_NUM=" + Convert.ToString(tillId), DataSource.CSCTills);
            var rsLine = GetRecords("select * from TaxExemptSaleLine " + " where SALE_NO=" + Convert.ToString(sn) + " AND LINE_NUM=" + Convert.ToString(lineNum) + " AND TILL_NUM=" + Convert.ToString(tillId), DataSource.CSCTills);
            if (teType == "QITE")
            {
                if (rsHead.Rows.Count == 0)
                {
                    return false;
                }
            }
            else // 
            {
                if (rsHead.Rows.Count == 0 || rsLine.Rows.Count == 0)
                {
                    return false;
                }
            }


            taxExemptSale = new TaxExemptSale
            {
                Sale_Num = newSaleNo,
                TillNumber = tillId,
                Shift = Convert.ToInt16(till.Shift),
                UserCode = Convert.ToString(user.Code),
                teCardholder =
                {
                    CardholderID = CommonUtility.GetStringValue(rsHead.Rows[0]["CardholderID"]),
                    Barcode = CommonUtility.GetStringValue(rsHead.Rows[0]["Barcode"]),
                    CardNumber = CommonUtility.GetStringValue(rsHead.Rows[0]["cardnumber"])
                }
            };


            //ShinyFeb26,2009- QITE development
            if (teType == "QITE")
            {
                var rsCardHolder = GetRecords("select * from CLIENT " + " where CL_CODE=\'" + taxExemptSale.teCardholder.CardholderID + "\'", DataSource.CSCMaster);
                if (rsCardHolder.Rows.Count != 0)
                {
                    taxExemptSale.teCardholder.Name = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["Cl_Name"]);
                    taxExemptSale.teCardholder.Address = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["CL_Add1"]);
                    taxExemptSale.teCardholder.City = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["CL_City"]);
                    taxExemptSale.teCardholder.PlateNumber = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["PlateNumber"]);
                    taxExemptSale.teCardholder.PostalCode = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["CL_Postal"]);
                }
                // 
            }
            else // For AITE
            {
                var rsCardHolder = GetRecords("select * from TaxExemptCardRegistry " + " where CardholderID=\'" + taxExemptSale.teCardholder.CardholderID + "\'", DataSource.CSCMaster);
                if (rsCardHolder.Rows.Count != 0)
                {
                    taxExemptSale.teCardholder.Name = CommonUtility.GetStringValue(rsCardHolder.Rows[0]["Name"]);
                    taxExemptSale.teCardholder.GasQuota = CommonUtility.GetFloatValue(rsCardHolder.Rows[0]["GasQuota"]);
                    taxExemptSale.teCardholder.PropaneQuota = CommonUtility.GetFloatValue(rsCardHolder.Rows[0]["PropaneQuota"]);
                    taxExemptSale.teCardholder.TobaccoQuota = CommonUtility.GetFloatValue(rsCardHolder.Rows[0]["TobaccoQuota"]);
                }
            }
            if (rsLine.Rows.Count != 0)
            {
                foreach (DataRow line in rsLine.Rows)
                {
                    var mTeLine = new TaxExemptSaleLine
                    {
                        Quantity = CommonUtility.GetFloatValue(-1 * CommonUtility.GetIntergerValue(line["Quantity"])),
                        UnitsPerPkg = CommonUtility.GetFloatValue(-1 * CommonUtility.GetIntergerValue(line["UnitQuantity"])),
                        EquvQuantity = CommonUtility.GetFloatValue(-1 * CommonUtility.GetIntergerValue(line["EquvQuantity"])),
                        OriginalPrice = CommonUtility.GetFloatValue(line["OriginalPrice"]),
                        TaxFreePrice = CommonUtility.GetFloatValue(line["price"]),
                        Line_Num = 1,
                        StockCode = CommonUtility.GetStringValue(line["Stock_Code"]),
                        TaxInclPrice = CommonUtility.GetFloatValue(-1 * CommonUtility.GetIntergerValue(line["TaxIncludedAmount"])),
                        Amount = CommonUtility.GetFloatValue(-1 * CommonUtility.GetIntergerValue(line["Amount"])),
                        ExemptedTax = CommonUtility.GetFloatValue(-1 * CommonUtility.GetIntergerValue(line["ExemptedTax"])),
                        Description = CommonUtility.GetStringValue(line["Description"]),
                        ProductCode = CommonUtility.GetStringValue(line["ProductCode"])
                    };
                    
                    mPrivateGlobals.teProductEnum productType;
                    Enum.TryParse(CommonUtility.GetStringValue(line["ProductType"]), out productType);

                    mTeLine.ProductType = productType;

                    mTeLine.TaxExemptRate = CommonUtility.GetFloatValue(line["TaxExemptRate"]);

                    bool tempCheckOverLimit = true;
                    var tempCheckQuota = true;
                    taxExemptSale.Add_a_Line(mTeLine, ref tempCheckOverLimit, ref tempCheckQuota);
                }
            }

            taxExemptSale.Amount = CommonUtility.GetFloatValue(-1 * CommonUtility.GetIntergerValue(rsHead.Rows[0]["SaleAmount"]));

            LoadGstExemptForDeletePrepay(ref taxExemptSale, sn, tillId, DataSource.CSCTills); 
            return true;
        }
        

        

        /// <summary>
        /// Method to update tax exempt sale
        /// </summary>
        /// <param name="taxExemptSale">Tax exempt sale</param>
        /// <param name="gasLessQuota">Gas less quota</param>
        /// <param name="propaneLessQuota">Propane less quota</param>
        /// <param name="tobaccoLessQuota">Tobacco less quota</param>
        public void UpdateSale(TaxExemptSale taxExemptSale, float gasLessQuota, float propaneLessQuota,
            float tobaccoLessQuota)
        {
            Line_Tax lt = default(Line_Tax);
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            var query = "select * from TaxExemptSaleHead " + " where SALE_NO=" + Convert.ToString(taxExemptSale.Sale_Num) + " AND TILL_NUM=" + Convert.ToString(taxExemptSale.TillNumber);
            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            
            if (_dataTable.Rows.Count != 0)
            {
                var rsTeSaleHead = _dataTable.Rows[0];
                rsTeSaleHead["SaleAmount"] = taxExemptSale.Amount;
                if (!taxExemptSale.GasOverLimit)
                {
                    rsTeSaleHead["GasReason"] = "";
                    rsTeSaleHead["GasReasonDesp"] = "";
                    rsTeSaleHead["GasReasonDetail"] = "";
                }
                if (!taxExemptSale.PropaneOverLimit)
                {
                    rsTeSaleHead["PropaneReason"] = "";
                    rsTeSaleHead["PropaneReasonDesp"] = "";
                    rsTeSaleHead["PropaneReasonDetail"] = "";
                }
                if (!taxExemptSale.TobaccoOverLimit)
                {
                    rsTeSaleHead["TobaccoReason"] = "";
                    rsTeSaleHead["TobaccoReasonDesp"] = "";
                    rsTeSaleHead["TobaccoReasonDetail"] = "";
                }

                rsTeSaleHead["TotalExemptedTax"] = taxExemptSale.TotalExemptedTax;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
            }

            
            foreach (TaxExemptSaleLine tempLoopVarTesl in taxExemptSale.Te_Sale_Lines)
            {
                var tesl = tempLoopVarTesl;
                _dataTable = new DataTable();
                query = "select * from TaxExemptSaleLine " + " where SALE_NO="
                    + Convert.ToString(taxExemptSale.Sale_Num) + " AND LINE_NUM="
                    + Convert.ToString(taxExemptSale.TillNumber) + " AND LINE_NUM="
                    + Convert.ToString(tesl.Line_Num);
                _adapter = new SqlDataAdapter(query, _connection);
                _adapter.Fill(_dataTable);
                if (_dataTable.Rows.Count != 0)
                {
                    var rsTeSaleLine = _dataTable.Rows[0];
                    rsTeSaleLine["Quantity"] = tesl.Quantity;
                    rsTeSaleLine["EquvQuantity"] = tesl.EquvQuantity;
                    rsTeSaleLine["TaxIncludedAmount"] = tesl.TaxInclPrice;
                    rsTeSaleLine["Amount"] = tesl.Amount;
                    rsTeSaleLine["ExemptedTax"] = tesl.ExemptedTax;
                    rsTeSaleLine["OverLimit"] = tesl.OverLimit;
                    rsTeSaleLine["RunningQuota"] = tesl.RunningQuota.ToString("#0.00");

                    if (tesl.StockIsChanged)
                    {
                        rsTeSaleLine["Stock_Code"] = tesl.StockCode;
                        rsTeSaleLine["ProductType"] = tesl.ProductType;
                        rsTeSaleLine["ProductCode"] = tesl.ProductCode;
                        rsTeSaleLine["Description"] = tesl.Description;
                        rsTeSaleLine["price"] = tesl.TaxFreePrice;
                        rsTeSaleLine["OriginalPrice"] = tesl.OriginalPrice;
                    }
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                    _adapter.Update(_dataTable);
                }
            }
            bool addNew;
            foreach (Sale_Tax tempLoopVarSx in taxExemptSale.TaxCredit)
            {
                var sx = tempLoopVarSx;
                if (sx.Taxable_Amount != 0 | sx.Tax_Included_Amount != 0)
                {
                    addNew = false;
                    _dataTable = new DataTable();
                    query = "SELECT * from TaxCredit where TILL_NUM=" + taxExemptSale.TillNumber
                        + " AND SALE_NO=" + Convert.ToString(taxExemptSale.Sale_Num)
                        + " AND Tax_Name=\'" + sx.Tax_Name + "\' AND " + " Tax_Code=\'"
                        + sx.Tax_Code + "\' ";
                    _adapter = new SqlDataAdapter(query, _connection);
                    _adapter.Fill(_dataTable);
                    DataRow taxCredit;
                    if (_dataTable.Rows.Count == 0)
                    {
                        addNew = true;
                        taxCredit = _dataTable.NewRow();
                        taxCredit["Till_Num"] = taxExemptSale.TillNumber;
                        taxCredit["sale_no"] = taxExemptSale.Sale_Num;
                        taxCredit["Tax_Name"] = sx.Tax_Name;
                        taxCredit["Tax_Code"] = sx.Tax_Code;
                        SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                        _adapter.InsertCommand = builder.GetInsertCommand();
                    }
                    else
                    {
                        taxCredit = _dataTable.Rows[0];
                        SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                        _adapter.UpdateCommand = builder.GetUpdateCommand();
                    }
                    taxCredit["Tax_Rate"] = sx.Tax_Rate;
                    taxCredit["Taxable_Amount"] = sx.Taxable_Amount;
                    taxCredit["Tax_Added_Amount"] = sx.Tax_Added_Amount;
                    taxCredit["Tax_Included_Amount"] = sx.Tax_Included_Amount;
                    taxCredit["Tax_Included_Total"] = sx.Tax_Included_Total;
                    if (addNew)
                    {
                        _dataTable.Rows.Add(taxCredit);
                    }
                    _adapter.Update(_dataTable);
                }
            }

            foreach (TaxCreditLine tempLoopVarTx in taxExemptSale.TaxCreditLines)
            {
                var tx = tempLoopVarTx;
                foreach (Line_Tax tempLoopVarLt in tx.Line_Taxes)
                {
                    addNew = false;
                    _dataTable = new DataTable();
                    query = "SELECT * from TaxCreditLine where TILL_NUM=" + taxExemptSale.TillNumber
                        + " AND SALE_NO=" + Convert.ToString(taxExemptSale.Sale_Num) + " AND Tax_Name=\'"
                        + lt.Tax_Name + "\' AND " + " Line_No=" + Convert.ToString(tx.Line_Num);
                    _adapter = new SqlDataAdapter(query, _connection);
                    _adapter.Fill(_dataTable);
                    DataRow taxCreditLine;
                    lt = tempLoopVarLt;
                    if (_dataTable.Rows.Count == 0)
                    {
                        addNew = true;
                        taxCreditLine = _dataTable.NewRow();
                        taxCreditLine["Till_Num"] = taxExemptSale.TillNumber;
                        taxCreditLine["sale_no"] = taxExemptSale.Sale_Num;
                        taxCreditLine["Line_No"] = tx.Line_Num;
                        taxCreditLine["Tax_Name"] = lt.Tax_Name;
                        SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                        _adapter.InsertCommand = builder.GetInsertCommand();
                    }
                    else
                    {
                        taxCreditLine = _dataTable.Rows[0];
                        SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                        _adapter.UpdateCommand = builder.GetUpdateCommand();
                    }
                    taxCreditLine["Tax_Code"] = lt.Tax_Code;
                    taxCreditLine["Tax_Rate"] = lt.Tax_Rate;
                    taxCreditLine["Tax_Included"] = lt.Tax_Included;
                    taxCreditLine["Tax_Added_Amount"] = lt.Tax_Added_Amount;
                    taxCreditLine["Tax_Included_Amount"] = lt.Tax_Incl_Amount;
                    if (addNew)
                    {
                        _dataTable.Rows.Add(taxCreditLine);
                    }
                    _adapter.Update(_dataTable);
                }
            }
            _connection.Close();
            
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            var rsCardHolder = new DataTable();
            query = "select * from TaxExemptCardRegistry " + " where CardholderID=\'" + taxExemptSale.teCardholder.CardholderID + "\'";
            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            if (rsCardHolder.Rows.Count != 0)
            {
                rsCardHolder.Rows[0]["TobaccoQuota"] = (CommonUtility.GetFloatValue(rsCardHolder.Rows[0]["TobaccoQuota"]) - tobaccoLessQuota).ToString("#0.00");
                rsCardHolder.Rows[0]["GasQuota"] = (CommonUtility.GetFloatValue(rsCardHolder.Rows[0]["GasQuota"]) - gasLessQuota).ToString("#0.00");
                rsCardHolder.Rows[0]["PropaneQuota"] = (CommonUtility.GetFloatValue(rsCardHolder.Rows[0]["PropaneQuota"]) - propaneLessQuota).ToString("#0.00");
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(rsCardHolder);
            }
            _connection.Close();
            _adapter?.Dispose();
        }

        /// <summary>
        /// Method to get tax purchase item 
        /// </summary>
        /// <param name="saleNo">Sale numvber</param>
        /// <param name="lineNo">Line number</param>
        /// <returns>Tax exempt purchase item</returns>
        public tePurchaseItem GetPurchaseItem(int saleNo, int lineNo)
        {
            tePurchaseItem teItem = null;
            var rs = GetRecords("select * from PurchaseItem where Sale_No=" + Convert.ToString(saleNo) + " AND Line_No=" + Convert.ToString(lineNo), DataSource.CSCTills);

            if (rs.Rows.Count > 0)
            {
                teItem = new tePurchaseItem();
                teItem.TreatyNo = CommonUtility.GetStringValue(rs.Rows[0]["TreatyNo"]);
                teItem.PsTierID = CommonUtility.GetShortValue(rs.Rows[0]["TierID"]);
                teItem.PsLevelID = CommonUtility.GetShortValue(rs.Rows[0]["LevelID"]);
                teItem.PdOriginalPrice = CommonUtility.GetFloatValue(rs.Rows[0]["OriginalPrice"]);
                teItem.TaxFreePrice = CommonUtility.GetFloatValue(rs.Rows[0]["Amount"]);
                teItem.petaxInclPrice = -1 * CommonUtility.GetFloatValue(teItem.TaxFreePrice + CommonUtility.GetFloatValue(rs.Rows[0]["TotalTaxSaved"]));
            }
            return teItem;
        }
    }
}

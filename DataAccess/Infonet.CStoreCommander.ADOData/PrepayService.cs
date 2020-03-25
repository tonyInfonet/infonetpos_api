using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infonet.CStoreCommander.ADOData
{
    public class PrepayService : SqlDbService, IPrepayService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        /// <summary>
        /// Method to get prepay global
        /// </summary>
        /// <param name="pumpId">Pump id</param>
        /// <returns>Prepay</returns>
        public recPump GetPrepayGlobal(int pumpId)
        {
            var rec = new recPump();
            var dRecset = GetRecords("SELECT * FROM PrepayGlobal  WHERE pumpID = " + Convert.ToString(pumpId), DataSource.CSCTrans);
            if (dRecset.Rows.Count == 0)
            {
                rec.IsPrepay = false;
                rec.IsPrepayLocked = false;

                
                
                if (!(rec.IsHoldPrepay && rec.PrepayAmount > 0))
                {
                    rec.PrepayAmount = 0;
                    rec.PrepayInvoiceID = 0;
                    rec.PrepayPosition = 0;
                }

            }
            else
            {
                rec.PrepayAmount = CommonUtility.GetFloatValue(dRecset.Rows[0]["Amount"]);
                rec.IsPrepayLocked = CommonUtility.GetBooleanValue(dRecset.Rows[0]["Locked"]);
                rec.PrepayInvoiceID = CommonUtility.GetIntergerValue(dRecset.Rows[0]["InvoiceID"]);
                rec.PrepayPosition = CommonUtility.GetShortValue(dRecset.Rows[0]["PositionID"]);
                rec.IsPrepay = true;
            }
            return rec;
        }

        /// <summary>
        /// Method to set prepay from POS
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="amount">Amount</param>
        /// <param name="mop">MOP</param>
        /// <param name="positionId">Position Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>True or false</returns>
        public bool SetPrepaymentFromPos(int invoiceId, short pumpId, float amount,
            byte mop, byte positionId, int tillNumber)
        {

            var sSql = "SELECT * FROM PrepayGlobal WHERE pumpID = " + Convert.ToString(pumpId);

            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter(sSql, _connection);
            _adapter.Fill(_dataTable);

            if (_dataTable.Rows.Count == 0)
            {
                DataRow dataRow = _dataTable.NewRow();
                dataRow["pumpID"] = pumpId;
                dataRow["InvoiceID"] = invoiceId;
                dataRow["MOP"] = mop;
                dataRow["TillID"] = tillNumber;
                dataRow["Amount"] = amount.ToString("###0.00");
                dataRow["locked"] = false;
                dataRow["PositionID"] = positionId;

                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
            }
            else
            {
                _dataTable.Rows[0]["InvoiceID"] = invoiceId;
                _dataTable.Rows[0]["MOP"] = mop;
                _dataTable.Rows[0]["TillID"] = tillNumber;
                _dataTable.Rows[0]["Amount"] = amount.ToString("###0.00");
                _dataTable.Rows[0]["locked"] = false;
                _dataTable.Rows[0]["PositionID"] = positionId;

                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
            }
            _adapter.Update(_dataTable);
            _adapter?.Dispose();
            _connection.Close();
            return true;
        }

        /// <summary>
        /// Method to delete prepay from POS
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <returns>True or false</returns>
        public bool DeletePrepaymentFromPos(short pumpId)
        {
            var sSql = "Delete FROM PrepayGlobal WHERE pumpID = " + Convert.ToString(pumpId);
            Execute(sSql, DataSource.CSCTrans);

            return true;
        }

        /// <summary>
        /// Method to load prepay
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <returns>Prepay</returns>
        public recPump LoadPrepay(int pumpId)
        {
            recPump rec;
            var dRecset = GetRecords("SELECT * FROM PrepayGlobal  WHERE pumpID = " + Convert.ToString(pumpId), DataSource.CSCTrans);
            if (dRecset.Rows.Count == 0)
            {
                rec = new recPump
                {
                    IsHoldPrepay = false,
                    IsPrepay = false,
                    PrepayAmount = 0,
                    IsPrepayLocked = false,
                    PrepayInvoiceID = 0,
                    PrepayPosition = 0
                };
            }
            else
            {
                rec = new recPump
                {
                    IsHoldPrepay = true,
                    IsPrepay = true,
                    PrepayAmount = CommonUtility.GetFloatValue(dRecset.Rows[0]["Amount"]),
                    IsPrepayLocked = CommonUtility.GetBooleanValue(dRecset.Rows[0]["Locked"]),
                    PrepayInvoiceID = CommonUtility.GetIntergerValue(dRecset.Rows[0]["InvoiceID"]),
                    PrepayPosition = CommonUtility.GetShortValue(dRecset.Rows[0]["PositionID"])

                };
            }
            return rec;
        }

        /// <summary>
        /// Method to lock prepay
        /// </summary>
        /// <param name="pumpId">Pump id</param>
        /// <returns>True or false</returns>
        public bool LockPrepay(short pumpId)
        {
            var returnValue = false;
            var sSql = "SELECT * FROM PrepayGlobal WHERE pumpID = " + Convert.ToString(pumpId);
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter(sSql, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count > 0)
            {
                _dataTable.Rows[0]["locked"] = true;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
                returnValue = true;
            }
            _adapter?.Dispose();
            _connection.Close();
            return returnValue;
        }

        /// <summary>
        /// Method to update prepay pump id for sale
        /// </summary>
        /// <param name="saleNo">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="newPumpId">New pump Id</param>
        public void UpdatePrepayPumpIdForSale(int saleNo, int tillNumber, short newPumpId)
        {
            var sSql = "SELECT * FROM SaleLine WHERE SaleLine.Sale_No = " + Convert.ToString(saleNo) + " and Prepay=1 and TILL_NUM=" + tillNumber;

            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter(sSql, _connection);
            _adapter.Fill(_dataTable);

            if (_dataTable.Rows.Count == 0)
            {
                sSql = "SELECT * FROM SaleLine WHERE SaleLine.Sale_No = " + Convert.ToString(saleNo) + " and Prepay=1 ";
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter(sSql, _connection);
                _adapter.Fill(_dataTable);
            }

            if (_dataTable.Rows.Count == 0)
            {
                return;
            }

            _dataTable.Rows[0]["pumpID"] = newPumpId;
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.UpdateCommand = builder.GetUpdateCommand();
            _adapter.Update(_dataTable);
            _adapter?.Dispose();
            _connection.Close();
        }

        /// <summary>
        /// Method to get line kit
        /// </summary>
        /// <param name="db">Data source</param>
        /// <param name="sn">Sale number</param>
        /// <param name="ln">Line number</param>
        /// <returns>Line kits</returns>
        public Line_Kits Get_Line_Kit(DataSource db, int sn, int ln)
        {
            Line_Kits lk = new Line_Kits();

            // Get the kit items in the line
            var rsLineKit = GetRecords("Select *  FROM   SaleKit  WHERE  SaleKit.Sale_No = " + Convert.ToString(sn)
                + " AND " + "       SaleKit.Line_No = " + Convert.ToString(ln) + " ", db);

            foreach (DataRow lineKit in rsLineKit.Rows)
            {
                // Charges on Kit items
                var rsLineKitChg = GetRecords("Select *  FROM   SaleKitChg  WHERE  SaleKitChg.Sale_No = "
                    + Convert.ToString(sn) + " AND " + "       SaleKitChg.Line_No = " + Convert.ToString(ln)
                    + " AND " + "       SaleKitChg.Kit_Item = \'" + CommonUtility.GetStringValue(lineKit["Kit_Item"]) + "\' ", db);

                var lkc = new K_Charges();
                foreach (DataRow kitCharge in rsLineKitChg.Rows)
                {

                    // Taxes on Charges on Kit items
                    var rsCgt = GetRecords("Select *  FROM   SaleKitChgTax  WHERE  SaleKitChgTax.Sale_No  = "
                        + Convert.ToString(sn) + " AND " + "       SaleKitChgTax.Line_No  = "
                        + Convert.ToString(ln) + " AND " + "       SaleKitChgTax.Kit_Item = \'"
                        + CommonUtility.GetStringValue(lineKit["Kit_Item"]) + "\' AND " + "       SaleKitChgTax.As_Code  = \'"
                        + CommonUtility.GetStringValue(kitCharge["As_Code"]) + "\' ", db);

                    var cgt = new Charge_Taxes();
                    foreach (DataRow tax in rsCgt.Rows)
                    {
                        cgt.Add(CommonUtility.GetStringValue(tax["Tax_Name"]),
                            CommonUtility.GetStringValue(tax["Tax_Code"]),
                           CommonUtility.GetFloatValue(tax["Tax_Rate"]),
                           CommonUtility.GetBooleanValue(tax["Tax_Included"]), "");
                    }
                    lkc.Add(CommonUtility.GetDoubleValue(kitCharge["price"]),
                      CommonUtility.GetStringValue(kitCharge["Description"]),
                       CommonUtility.GetStringValue(kitCharge["As_Code"]), cgt, "");
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
        /// Get purchase items
        /// </summary>
        /// <param name="db">Data source</param>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>Purchase items</returns>
        public List<tePurchaseItem> GetPurchaseItems(DataSource db, int saleNumber)
        {
            var purchaseItems = new List<tePurchaseItem>();
            var rs = GetRecords("select saleline.gradeid, purchaseitem.* from PurchaseItem  " + " INNER JOIN Saleline on Saleline.Sale_no = Purchaseitem.sale_No and Saleline.line_num = purchaseitem.line_no " + " where purchaseitem.Sale_No=" + Convert.ToString(saleNumber), db);
            foreach (DataRow row in rs.Rows)
            {
                purchaseItems.Add(new tePurchaseItem
                {
                    TreatyNo = CommonUtility.GetStringValue(row["TreatyNo"]),
                    PsTierID = CommonUtility.GetShortValue(row["TierID"]),
                    PsLevelID = CommonUtility.GetShortValue(row["LevelID"]),
                    PsGradeIDpsTreatyNo = CommonUtility.GetShortValue(row["GradeID"]),
                    PdOriginalPrice = CommonUtility.GetFloatValue(row["OriginalPrice"]),
                    TaxFreePrice = CommonUtility.GetFloatValue(row["Amount"]),
                    pdTaxFreePrice = CommonUtility.GetFloatValue(row["TotalTaxSaved"]),
                    Quantity = CommonUtility.GetFloatValue(row["Quantity"]),
                    LineItem = CommonUtility.GetShortValue(row["Line_No"]),
                    stockcode = CommonUtility.GetStringValue(row["cscpurchaseitemkey"]),

                });
            }
            return purchaseItems;
        }

        /// <summary>
        /// Method to get original sale head
        /// </summary>
        /// <param name="invoiceId">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="db"></param>
        /// <returns>Sale head</returns>
        public SaleHead GetOriginalSaleHead(int invoiceId, int tillNumber, out DataSource db)
        {
            db = DataSource.CSCTills;
            var rsHead = GetRecords(" SELECT * FROM SaleHead " + " WHERE  SaleHead.Sale_No = " + Convert.ToString(invoiceId) + " and TILL=" + tillNumber, db); //, adLockReadOnly)
            if (rsHead.Rows.Count == 0)
            {
                db = DataSource.CSCTrans;
                rsHead = GetRecords(" SELECT * FROM SaleHead " + " WHERE  SaleHead.Sale_No = " + Convert.ToString(invoiceId) + " and TILL=" + tillNumber, db); //, adLockReadOnly)
            }
            if (rsHead.Rows.Count == 0)
            {
                return null;
            }
            return new SaleHead
            {
                SaleNumber = CommonUtility.GetIntergerValue(rsHead.Rows[0]["sale_no"]),
                TillNumber = CommonUtility.GetIntergerValue(rsHead.Rows[0]["Till"]),
                Client = CommonUtility.GetStringValue(rsHead.Rows[0]["Client"]),
                TType = CommonUtility.GetStringValue(rsHead.Rows[0]["T_type"]),
                Deposit = CommonUtility.GetDecimalValue(rsHead.Rows[0]["Deposit"]),
                TreatyNumber = CommonUtility.GetStringValue(rsHead.Rows[0]["TreatyNumber"]),
                Change = CommonUtility.GetDecimalValue(rsHead.Rows[0]["Change"]),
                PennyAdjust = CommonUtility.GetDecimalValue(rsHead.Rows[0]["Penny_Adj"]),
                DiscountType = CommonUtility.GetStringValue(rsHead.Rows[0]["Disc_Type"]),
                INVCDiscount = CommonUtility.GetDecimalValue(rsHead.Rows[0]["Invc_Disc"])
            };
        }

        /// <summary>
        /// Method to get orginal saleLines
        /// </summary>
        /// <param name="invoiceId">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of sale lines</returns>
        public List<Sale_Line> GetOrginalSaleLines(int invoiceId, int tillNumber, DataSource dataSource)
        {
            var saleLines = new List<Sale_Line>();

            var rsDetail = GetRecords(" SELECT * FROM SaleLine " + " WHERE SaleLine.Sale_No = " + Convert.ToString(invoiceId) + " " + " and TILL_NUM=" + tillNumber + " ORDER BY SaleLine.Line_Num ", dataSource);
            foreach (DataRow row in rsDetail.Rows)
            {
                saleLines.Add(new Sale_Line
                {
                    Dept = CommonUtility.GetStringValue(row["Dept"]),
                    Sub_Dept = CommonUtility.GetStringValue(row["Sub_Dept"]),
                    Sub_Detail = CommonUtility.GetStringValue(row["Sub_Detail"]),
                    Stock_Code = CommonUtility.GetStringValue(row["Stock_Code"]),
                    PLU_Code = CommonUtility.GetStringValue(row["PLU_Code"]),
                    Line_Num = CommonUtility.GetShortValue(row["Line_Num"]),
                    Price_Type = CommonUtility.GetCharValue(row["Price_Type"]),
                    Quantity = CommonUtility.GetFloatValue(row["Quantity"]),
                    Amount = CommonUtility.GetDecimalValue(row["Amount"]),
                    Discount_Adjust = CommonUtility.GetDoubleValue(row["Disc_adj"]),
                    Line_Discount = CommonUtility.GetDoubleValue(row["Discount"]),
                    Discount_Type = CommonUtility.GetStringValue(row["Disc_Type"]),
                    Discount_Code = CommonUtility.GetStringValue(row["Disc_Code"]),
                    Discount_Rate = CommonUtility.GetFloatValue(row["Disc_Rate"]),
                    DiscountName = CommonUtility.GetStringValue(row["DiscountName"]),
                    Associate_Amount = CommonUtility.GetDecimalValue(row["Assoc_Amt"]),
                    User = CommonUtility.GetStringValue(row["User"]),
                    Description = CommonUtility.GetStringValue(row["Descript"]),
                    Loyalty_Save = CommonUtility.GetFloatValue(row["Loyl_Save"]),
                    Units = CommonUtility.GetStringValue(row["Units"]),
                    Serial_No = CommonUtility.GetStringValue(row["Serial_No"]),
                    Prepay = Convert.ToBoolean(row["Prepay"]),
                    pumpID = CommonUtility.GetByteValue(row["pumpID"]),
                    GradeID = CommonUtility.GetByteValue(row["GradeID"]),
                    PositionID = CommonUtility.GetByteValue(row["PositionID"]),
                    IsTaxExemptItem = CommonUtility.GetBooleanValue(row["TaxExempt"]), 
                    Total_Amount = CommonUtility.GetDecimalValue(row["Total_Amt"]),
                    Regular_Price = CommonUtility.GetDoubleValue(row["Reg_Price"]),
                    price = CommonUtility.GetDoubleValue(row["price"]),
                    FuelRebateEligible = CommonUtility.GetBooleanValue(row["FuelRebateUsed"]),
                    FuelRebate = CommonUtility.GetDecimalValue(row["RebateDiscount"]),
                    PromoID = CommonUtility.GetStringValue(row["PromoID"])
                });
            }
            return saleLines;
        }

        /// <summary>
        /// Method to get original position Id
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        public short? GetOrginalPositionId(int saleNumber, int tillNumber, DataSource dataSource)
        {
            var rsTemp = GetRecords(" SELECT * FROM SaleLine " + " WHERE SaleLine.Sale_No = " + Convert.ToString(saleNumber)
                + " " + " and TILL_NUM=" + tillNumber + " and Prepay=1" + " ORDER BY SaleLine.Line_Num ", dataSource);

            if (rsTemp.Rows.Count == 0)
            {
                return null;
            }
            return CommonUtility.GetShortValue(rsTemp.Rows[0]["PositionID"]); //
        }

        /// <summary>
        /// Method to get discount tender
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        public DiscountTender GetDiscountTender(int saleNumber, int tillNumber, DataSource dataSource)
        {
            var rsDiscountTender = GetRecords("select * from DiscountTender where SALE_NO=" + Convert.ToString(saleNumber) + " AND TILL_NUM=" + Convert.ToString(tillNumber), dataSource);
            
            if (rsDiscountTender.Rows.Count != 0)
            {
                return new DiscountTender
                {
                    CardNumber = CommonUtility.GetStringValue(rsDiscountTender.Rows[0]["CardNum"]),
                    CouponId = CommonUtility.GetStringValue(rsDiscountTender.Rows[0]["CouponID"])
                };
            }
            return new DiscountTender
            {
                CardNumber = "",
                CouponId = ""
            };
        }


        /// <summary>
        /// Method to get stock description
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Description</returns>
        public string GetStockDescription(string stockCode)
        {
            string query = $"Select DESCRIPT from StockMst where Stock_code ='{stockCode}'";
            var dt = GetRecords(query, DataSource.CSCMaster);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            return CommonUtility.GetStringValue(dt.Rows[0]["DESCRIPT"]);

        }

        /// <summary>
        /// Method to get line taxes
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        public List<Line_Tax> GetLineTaxes(int saleNumber, int lineNumber, DataSource dataSource)
        {
            var lineTaxes = new List<Line_Tax>();
            var rsLineTax = GetRecords("Select * From   S_LineTax  WHERE  S_LineTax.Sale_No = "
                + Convert.ToString(saleNumber) + " AND " + "       S_LineTax.Line_No = " + Convert.ToString(lineNumber)
                + " " + "Order By S_LineTax.Tax_Name ", dataSource);

            foreach (DataRow row in rsLineTax.Rows)
            {
                lineTaxes.Add(new Line_Tax
                {
                    Tax_Name = CommonUtility.GetStringValue(row["Tax_Name"]),
                    Tax_Code = CommonUtility.GetStringValue(row["Tax_Code"]),
                    Tax_Rate = CommonUtility.GetFloatValue(row["Tax_Rate"]),
                    Tax_Included = CommonUtility.GetBooleanValue(row["Tax_Included"]),
                    Tax_Rebate_Rate = CommonUtility.GetFloatValue(row["Tax_Rebate_Rate"]),
                    Tax_Rebate = CommonUtility.GetDecimalValue(row["Tax_Rebate"])
                });
            }
            return lineTaxes;
        }

        /// <summary>
        /// Method to get product tax exempt
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns></returns>
        public ProductTaxExempt GetProductTaxExemptForStock(string stockCode)
        {
            var rsPte = GetRecords("SELECT * FROM ProductTaxExempt " + " WHERE ProductKey=\'" + stockCode + "\'", DataSource.CSCMaster);
            if (rsPte.Rows.Count != 0)
            {
                return new ProductTaxExempt
                {
                    TEVendor = CommonUtility.GetStringValue(rsPte.Rows[0]["ProductCode"]),
                    CategoryFK = CommonUtility.GetShortValue(rsPte.Rows[0]["CategoryFK"])
                };
            }
            return null;
        }

        /// <summary>
        /// Method to update discount tender
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <param name="discounTender">Discount tender</param>
        public void UpdateDiscountTender(int saleNumber, int tillNumber, DataSource dataSource, DiscountTender discounTender)
        {
            _connection = new SqlConnection(GetConnectionString(dataSource));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from DiscountTender where SALE_NO=" + Convert.ToString(saleNumber)
                + " AND TILL_NUM=" + tillNumber, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                var fields = _dataTable.Rows[0];
                fields["SaleAmount"] = discounTender.SaleAmount;
                fields["Discountamount"] = discounTender.DiscountAmount;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
                _adapter?.Dispose();
                _connection.Close();
            }
        }

        /// <summary>
        /// Method to update coupon
        /// </summary>
        /// <param name="couponId">Coupon Id</param>
        /// <param name="amount">Amount</param>
        public void UpdateCoupon(string couponId, string amount)
        {
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from Coupon where CouponID=\'" + couponId + "\'", _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                var fields = _dataTable.Rows[0];
                fields["Amount"] = amount;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
                _adapter?.Dispose();
                _connection.Close();
            }
        }

        /// <summary>
        /// Method to get sale line charges
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Charges</returns>
        public Charges GetSaleLineCharges(int saleNumber, int lineNumber, DataSource dataSource)
        {
            var rsLineChg = GetRecords("Select *  FROM   SaleChg  WHERE  SaleChg.Sale_No = " + Convert.ToString(saleNumber) + " AND " + "       SaleChg.Line_No = " + Convert.ToString(lineNumber) + " " + "Order By SaleChg.As_Code ", dataSource);
            var charges = new Charges();
            foreach (DataRow charge in rsLineChg.Rows)
            {

                var rsLcTax = GetRecords("Select *  FROM   ChargeTax  WHERE  ChargeTax.Sale_No = " + Convert.ToString(saleNumber) + " AND " + "       ChargeTax.Line_No = " + Convert.ToString(lineNumber) + " AND " + "       ChargeTax.As_Code = \'" + CommonUtility.GetStringValue(charge["As_Code"]) + "\' ", dataSource);
                // Find any taxes that applied to those charges.
                var lct = new Charge_Taxes();
                foreach (DataRow tax in rsLcTax.Rows)
                {
                    lct.Add(Convert.ToString(tax["Tax_Name"]), Convert.ToString(tax["Tax_Code"]),
                        Convert.ToSingle(tax["Tax_Rate"]), Convert.ToBoolean(tax["Tax_Included"]), "");
                }

                charges.Add(Convert.ToString(charge["As_Code"]), Convert.ToString(charge["Description"]),
                    Convert.ToSingle(charge["price"]), lct, "");
            }

            return charges;
        }

        /// <summary>
        /// Method to update sale data
        /// </summary>
        /// <param name="sx">Sale</param>
        /// <param name="sp">Sale line</param>
        /// <param name="pennyAdj">Penny adjustment</param>
        /// <param name="change">Change</param>
        /// <param name="saleQuantity">Sale quantity</param>
        /// <param name="taxExempt">Tax exempt</param>
        /// <param name="saleAmount">Sale amount</param>
        /// <param name="unitPrice">Unit price</param>
        /// <param name="iPositionId">Position Id</param>
        /// <param name="orgPosition">Original position</param>
        /// <param name="iGradeId">Grade Id</param>
        /// <param name="fuelLoyalty">Fuel loyalty</param>
        /// <param name="newTotalAmount">Total amount</param>
        /// <param name="fullSwitch">Full switch</param>
        /// <param name="dataSource">Data source</param>
        public void UpdateSaleData(Sale sx, Sale_Line sp, decimal pennyAdj, double change, float saleQuantity,
            bool taxExempt, float saleAmount, float unitPrice, short iPositionId, short orgPosition, short iGradeId,
           bool fuelLoyalty, float newTotalAmount, bool fullSwitch, DataSource dataSource)
        {
            _connection = new SqlConnection(GetConnectionString(dataSource));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            
            //  -to use the object SP instead of SL( sometimes SL is changing and resetiing info in other section of code- screwing up prepy stock_code, dept etc in saleline for TE sales) to reset additional info in
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter(" SELECT * FROM SaleHead " + " WHERE  SaleHead.Sale_No = "
                + Convert.ToString(sx.Sale_Num) + " and TILL=" + sx.TillNumber, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                var field = _dataTable.Rows[0];
                field["Penny_Adj"] = pennyAdj;
                field["Sale_amt"] = sx.Sale_Totals.Gross.ToString("##0.00");
                field["Change"] = (Convert.ToDouble(field["Change"]) + change).ToString("##0.00");
                field["OverPayment"] = sx.OverPayment.ToString("##0.00");
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
            }

            
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter(" SELECT * FROM SaleLine " + " WHERE SaleLine.Sale_No = "
                + Convert.ToString(sx.Sale_Num) + " " + " and Prepay=1", _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                var field = _dataTable.Rows[0];
                field["Quantity"] = saleQuantity.ToString("##0.000");
                
                if (taxExempt && !string.IsNullOrEmpty(sx.TreatyNumber))
                {
                    field["Amount"] = saleAmount.ToString("##0.00");
                }
                else
                {
                    field["Amount"] = saleAmount.ToString("##0.00");
                    field["price"] = unitPrice.ToString("##0.000");
                } 


                if (iPositionId != orgPosition)
                {
                    field["PositionID"] = iPositionId;
                    field["GradeID"] = iGradeId;
                    if (!string.IsNullOrEmpty(sp.Dept)) 
                    {
                        field["Dept"] = sp.Dept; 
                    }
                    if (!string.IsNullOrEmpty(sp.Sub_Dept)) 
                    {
                        field["Sub_Dept"] = sp.Sub_Dept; 
                    }
                    if (!string.IsNullOrEmpty(sp.Sub_Detail)) 
                    {
                        field["Sub_Detail"] = sp.Sub_Detail; 
                    }
                    field["Stock_Code"] = sp.Stock_Code; 
                    field["PLU_Code"] = sp.PLU_Code; 
                    field["Descript"] = sp.Description; 
                    field["price"] = sp.price; 
                    field["Reg_Price"] = sp.Regular_Price; 
                }

                field["Discount"] = sx.Sale_Lines[field["Line_Num"]].Line_Discount;
                if (fuelLoyalty && !string.IsNullOrEmpty(sx.Customer.GroupID) && sx.Customer.DiscountType == "$")
                {
                    field["Disc_Rate"] = field["Discount"];
                }

                field["Total_Amt"] = newTotalAmount; 
                if (fullSwitch)
                {
                    field["Disc_adj"] = sp.Discount_Adjust; 
                }
                else
                {
                    field["Disc_adj"] = 0;
                }
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);

                

                Execute("Delete  From S_LineTax Where S_LineTax.Sale_No = " + Convert.ToString(sx.Sale_Num)
                    + " and Line_No=" + Convert.ToString(field["Line_Num"]), dataSource);

                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter("Select * From   S_LineTax  WHERE  S_LineTax.Sale_No = " + Convert.ToString(sx.Sale_Num), _connection);
                _adapter.Fill(_dataTable);

                if (sx.ApplyTaxes)
                {
                    if (sx.Sale_Lines.Count > 0)
                    {
                        foreach (Line_Tax tempLoopVarTx in sx.Sale_Lines[field["Line_Num"]].Line_Taxes)
                        {
                            var tx = tempLoopVarTx;
                            if (tx.Tax_Added_Amount != 0 | tx.Tax_Incl_Amount != 0)
                            {
                                var newTax = _dataTable.NewRow();

                                newTax["Till_Num"] = sx.TillNumber;
                                newTax["sale_no"] = sx.Sale_Num;
                                newTax["Line_No"] = field["Line_Num"];
                                newTax["Tax_Name"] = tx.Tax_Name;
                                newTax["Tax_Code"] = tx.Tax_Code;
                                newTax["Tax_Rate"] = tx.Tax_Rate;
                                newTax["Tax_Included"] = string.Format("##0.00", tx.Tax_Included);
                                newTax["Tax_Added_Amount"] = tx.Tax_Added_Amount.ToString("##0.00");
                                newTax["Tax_Included_Amount"] = tx.Tax_Incl_Amount.ToString("##0.00");
                                _dataTable.Rows.Add(newTax);
                                builder = new SqlCommandBuilder(_adapter);
                                _adapter.InsertCommand = builder.GetInsertCommand();
                                _adapter.Update(_dataTable);

                            }
                        }
                    }
                }
            }

            
            Execute("Delete  From S_SaleTax Where S_SaleTax.Sale_No = " + Convert.ToString(sx.Sale_Num) + " ", dataSource);
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * From S_SaleTax Where S_SaleTax.Sale_No = " + Convert.ToString(sx.Sale_Num), _connection);
            _adapter.Fill(_dataTable);

            foreach (Sale_Tax tempLoopVarSt in sx.Sale_Totals.Sale_Taxes)
            {
                var st = tempLoopVarSt;
                if (st.Taxable_Amount != 0 | st.Tax_Included_Amount != 0)
                {
                    var newTax = _dataTable.NewRow();
                    newTax["Till_Num"] = sx.TillNumber;
                    newTax["sale_no"] = sx.Sale_Num;
                    newTax["Tax_Name"] = st.Tax_Name;
                    newTax["Tax_Code"] = st.Tax_Code;
                    newTax["Tax_Rate"] = st.Tax_Rate;
                    newTax["Taxable_Amount"] = st.Taxable_Amount.ToString("##0.00");
                    newTax["Tax_Added_Amount"] = st.Tax_Added_Amount.ToString("##0.00");
                    newTax["Tax_Included_Amount"] = st.Tax_Included_Amount.ToString("##0.00");
                    newTax["Tax_Included_Total"] = st.Tax_Included_Total.ToString("##0.00");
                    _dataTable.Rows.Add(newTax);
                    var builder = new SqlCommandBuilder(_adapter);
                    _adapter.InsertCommand = builder.GetInsertCommand();
                    _adapter.Update(_dataTable);

                }
            }
            _adapter?.Dispose();
            _connection.Close();
        }
    }
}

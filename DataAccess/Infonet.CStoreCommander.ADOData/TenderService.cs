using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Infonet.CStoreCommander.ADOData
{
    public class TenderService : SqlDbService, ITenderService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Gets PCATS group
        /// </summary>
        /// <param name="tendClass">Tender class</param>
        /// <returns>PCAT group</returns>
        public string GetPcatsGroup(string tendClass)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetPcatsGroup,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            var pcatGroup = string.Empty;
            var rsPcatsGroup = GetRecords("SELECT PCATSGroup FROM TendClas WHERE TENDCLASS=\'" + tendClass + "\'", DataSource.CSCMaster);
            if (rsPcatsGroup.Rows.Count > 0)
            {
                var fields = rsPcatsGroup.Rows[0];
                pcatGroup = CommonUtility.GetStringValue(fields["PCATSGroup"]);
            }
            _performancelog.Debug($"End,TenderService,GetPcatsGroup,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return pcatGroup;
        }

        /// <summary>
        /// Method to get all tender
        /// </summary>
        /// <returns>List of tenders</returns>
        public List<Tender> GetAlltenders()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetAlltenders,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var tenders = new List<Tender>();
            var tend =
                 GetRecords(
                     @"SELECT TendMast.TENDCODE, TendMast.TENDDESC,TendMast.TENDCLASS, TendMast.DISPLAYSEQ, TendMast.EXACTCHG,
                      TendMast.GIVECHG, TendMast.AcceptAsPayment, TendMast.GIVEASREF, TendMast.CANADJUST, 
                     TendMast.MAXAMOUNT, TendMast.MINAMOUNT, TendMast.EXCHANGE, 
                     TendMast.SMALLUNIT, TendMast.Put_In_Drawer, 
                     TendMast.PrintCopies, TendMast.Signature, TENDCLAS.CLASSDESC, TendMast.Inactive, 
                     TendMast.Image 
                     FROM TendMast LEFT JOIN TENDCLAS ON TendMast.TENDCLASS =TENDCLAS.TENDCLASS 
                     WHERE (TendMast.Inactive IS NULL OR TendMast.Inactive=0) AND TendMast.TENDCODE IS NOT Null 
                     AND TendMast.TENDDESC IS NOT NULL AND TendMast.TENDCLASS IS NOT NULL 
                     ORDER BY TendMast.DisplaySeq",
                     DataSource.CSCMaster);

            foreach (DataRow row in tend.Rows)
            {
                var tender = new Tender
                {
                    Tender_Code = CommonUtility.GetStringValue(row["TENDCODE"]),
                    TendDescription = CommonUtility.GetStringValue(row["TENDDESC"]),
                    Tender_Class = CommonUtility.GetStringValue(row["TENDCLASS"]),
                    Sequence_Number = CommonUtility.GetShortValue(row["DISPLAYSEQ"]),
                    Exact_Change = CommonUtility.GetBooleanValue(row["EXACTCHG"]),
                    Give_Change = CommonUtility.GetBooleanValue(row["GIVECHG"]),
                    AcceptAspayment = CommonUtility.GetBooleanValue(row["AcceptAsPayment"]),
                    Give_As_Refund = CommonUtility.GetBooleanValue(row["GIVEASREF"]),
                    System_Can_Adjust = CommonUtility.GetBooleanValue(row["CANADJUST"]),
                    MaxAmount = CommonUtility.GetDoubleValue(row["MAXAMOUNT"]),
                    MinAmount = CommonUtility.GetDoubleValue(row["MINAMOUNT"]),
                    Exchange_Rate = CommonUtility.GetDoubleValue(row["EXCHANGE"]),
                    Smallest_Unit = CommonUtility.GetDoubleValue(row["SMALLUNIT"]),
                    PrintCopies = CommonUtility.GetShortValue(row["PrintCopies"]),
                    SignatureLine = CommonUtility.GetBooleanValue(row["Signature"]),
                    Inactive = CommonUtility.GetBooleanValue(row["Inactive"]),
                    Open_Drawer = CommonUtility.GetBooleanValue(row["Put_In_Drawer"]),
                    TendClassDescription = CommonUtility.GetStringValue(row["CLASSDESC"])
                };
                tender.PrintCopies = CommonUtility.GetShortValue(row["PrintCopies"]);
                tender.Image = CommonUtility.GetStringValue(row["Image"]);
                tenders.Add(tender);
            }
            _performancelog.Debug($"End,TenderService,GetAlltenders,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return tenders;
        }

        /// <summary>
        /// Method to check if eko gift cert
        /// </summary>
        /// <param name="strTender">Tender name</param>
        /// <returns>True or false</returns>
        public bool IsEkoGiftCert(string strTender)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,IsEkoGiftCert,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var rsPComp = GetRecords("select * from P_Comp where P_NAME=\'GiftTender\'", DataSource.CSCAdmin);

            if (rsPComp.Rows.Count > 0)
            {
                if ((string)rsPComp.Rows[0]["P_Set"] == "EKO")
                {
                    _performancelog.Debug($"End,TenderService,DeletePurchaseItem,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                    return true;
                }
            }

            var rsSet = GetRecords("select * from P_SET where P_NAME=\'GiftTender\' and P_VALUE=\'" + strTender + "\' AND P_SET=\'EKO\'", DataSource.CSCAdmin);

            if (rsSet.Rows.Count > 0)
            {
                _performancelog.Debug($"End,TenderService,IsEkoGiftCert,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return true;
            }
            _performancelog.Debug($"End,TenderService,IsEkoGiftCert,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return false;
        }

        /// <summary>
        /// Method to get first third party
        /// </summary>
        /// <param name="blCombineThirdParty">Combine third party or not</param>
        /// <returns>Value</returns>
        public byte GetFirstThirdParty(bool blCombineThirdParty)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetFirstThirdParty,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var tend = GetRecords("SELECT MIN(DisplaySeq) AS FT FROM TendMast  WHERE TendClass=\'THIRDPARTY\' AND (Inactive IS NULL OR Inactive=0)", DataSource.CSCMaster);

            byte firsThirdparty = 1;

            if (tend.Rows.Count > 0)
            {
                firsThirdparty = CommonUtility.GetByteValue(tend.Rows[0]["FT"]);
            }
            _performancelog.Debug($"End,TenderService,GetFirstThirdParty,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return firsThirdparty;
        }

        /// <summary>
        /// Method to get first fleet
        /// </summary>
        /// <param name="blCombineFleet">Combine fleet or not</param>
        /// <returns>Fleet value</returns>
        public byte GetFirstFleet(bool blCombineFleet)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetFirstFleet,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var tend = GetRecords("SELECT MIN(DisplaySeq) AS FF FROM TendMast  WHERE TendClass=\'FLEET\' AND (Inactive IS NULL OR Inactive=0)", DataSource.CSCMaster);

            byte firstFleet = 1;
            if (tend.Rows.Count > 0)
            {
                firstFleet = CommonUtility.GetByteValue(tend.Rows[0]["FF"]);
            }
            _performancelog.Debug($"End,TenderService,GetFirstFleet,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return firstFleet;
        }

        /// <summary>
        /// Method to get tenders while closing current till
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Tenders</returns>
        public Tenders GetTenderForCloseCurrentTill(int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetTenderForCloseCurrentTill,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var strSql = "Select SaleTend.TendCLAS, SaleTend.TendName " +
                        "From  SaleTend INNER JOIN SaleHead ON SaleTend.Sale_No = SaleHead.Sale_No " +
                        "Where (SaleTend.TendName <> \'Store Credit\' OR SaleTend.AmtTend > 0) " +
                        "AND SaleHead.T_Type NOT IN (\'VOID\',\'CANCEL\',\'SUSPEND\') and SaleHead.TILL= " +
                       tillNumber + " Group By SaleTend.TendCLAS, SaleTend.TendName " +
                        "having SaleTend.TENDNAME not in (Select TendDesc  FROM CSCMaster.dbo.TENDMAST " +
                        "WHERE TendDesc in (Select SaleTend.TendName " +
                        "From  SaleTend INNER JOIN SaleHead ON SaleTend.Sale_No = SaleHead.Sale_No " +
                        "Where (SaleTend.TendName <> \'Store Credit\' OR SaleTend.AmtTend > 0) " +
                        "AND SaleHead.T_Type NOT IN (\'VOID\',\'CANCEL\',\'SUSPEND\') and SaleHead.TILL= " +
                       tillNumber + " Group By SaleTend.TendName))";
            var rsTmp = GetRecords(strSql, DataSource.CSCTills);
            var tenders = new Tenders();

            foreach (DataRow row in rsTmp.Rows)
            {
                var tender = new Tender
                {
                    Tender_Code = CommonUtility.GetStringValue(row["TENDCODE"]),
                    TendClassDescription = CommonUtility.GetStringValue(row["TENDDESC"]),
                    Tender_Class = CommonUtility.GetStringValue(row["TENDCLASS"]),
                    Sequence_Number = CommonUtility.GetShortValue(row["DISPLAYSEQ"]),
                    Exact_Change = CommonUtility.GetBooleanValue(row["EXACTCHG"]),
                    Give_Change = CommonUtility.GetBooleanValue(row["GIVECHG"]),
                    AcceptAspayment = CommonUtility.GetBooleanValue(row["AcceptAsPayment"]),
                    Give_As_Refund = CommonUtility.GetBooleanValue(row["GIVEASREF"]),
                    System_Can_Adjust = CommonUtility.GetBooleanValue(row["CANADJUST"]),
                    MaxAmount = CommonUtility.GetDoubleValue(row["MAXAMOUNT"]),
                    MinAmount = CommonUtility.GetDoubleValue(row["MINAMOUNT"]),
                    Exchange_Rate = CommonUtility.GetDoubleValue(row["EXCHANGE"]),
                    Smallest_Unit = CommonUtility.GetDoubleValue(row["SMALLUNIT"]),
                    PrintCopies = CommonUtility.GetShortValue(row["PrintCopies"]),
                    SignatureLine = CommonUtility.GetBooleanValue(row["Signature"]),
                    Inactive = CommonUtility.GetBooleanValue(row["Inactive"]),
                    Open_Drawer = CommonUtility.GetBooleanValue(row["Put_In_Drawer"])
                };
                tender.TendClassDescription = CommonUtility.GetStringValue(row["CLASSDESC"]);
                tenders.Add(tender.Tender_Name, tender.Tender_Class, tender.Exchange_Rate, tender.Give_Change,
                    tender.Give_As_Refund,
                    tender.System_Can_Adjust, tender.Sequence_Number, tender.Tender_Code, tender.Exact_Change,
                    tender.MaxAmount,
                    tender.MinAmount, tender.Smallest_Unit, tender.Open_Drawer, 0, tender.PrintCopies,
                    tender.AcceptAspayment, tender.SignatureLine, tender.Image, ""
                    );
            }
            _performancelog.Debug($"End,TenderService,GetTenderForCloseCurrentTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return tenders;
        }

        /// <summary>
        /// Method to get first credit card
        /// </summary>
        /// <param name="blCombineCredit">Combine credit or not</param>
        /// <returns>First Credit card</returns>
        public byte GetFirstCc(bool blCombineCredit)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetFirstCC,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var tend = GetRecords("SELECT MIN(DisplaySeq) AS FC FROM TendMast  WHERE TendClass=\'CRCARD\' AND (Inactive IS NULL OR Inactive=0)", DataSource.CSCMaster);

            byte firstCc = 1;
            if (tend.Rows.Count > 0)
            {
                firstCc = CommonUtility.GetByteValue(tend.Rows[0]["FC"]);
            }
            _performancelog.Debug($"End,TenderService,GetFirstCC,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return firstCc;
        }

        /// <summary>
        /// Method to get vendor coupons
        /// </summary>
        /// <returns>Vendor coupons</returns>
        public VendorCoupons GetAllVendorCoupon()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetAllVendorCoupon,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var tend = GetRecords("SELECT * FROM TendMast  WHERE TendClass=\'COUPON\'", DataSource.CSCMaster);
            var vendorCoupons = new VendorCoupons();

            if (tend.Rows.Count > 0)
            {
                var rsCoupon = GetRecords("select * from VendorCoupons", DataSource.CSCMaster);

                foreach (DataRow dataRow in rsCoupon.Rows)
                {
                    var vc = new VendorCoupon
                    {
                        Code = CommonUtility.GetStringValue(dataRow["Code"]),
                        Name = CommonUtility.GetStringValue(dataRow["Name"]),
                        VendorCode = CommonUtility.GetStringValue(dataRow["VendorCode"]),
                        Value = CommonUtility.GetFloatValue(dataRow["Value"]),
                        StockCode = CommonUtility.GetStringValue(dataRow["StockCode"]),
                        Dept = CommonUtility.GetStringValue(dataRow["Dept"]),
                        SubDept = CommonUtility.GetStringValue(dataRow["SubDept"]),
                        SubDetail = CommonUtility.GetStringValue(dataRow["SubDetail"]),
                        TendDesc = CommonUtility.GetStringValue(dataRow["TendDesc"]),
                        DefaultCoupon = CommonUtility.GetBooleanValue(dataRow["DefaultCoupon"]),
                        SerNumLen = CommonUtility.GetShortValue(dataRow["SerialNumLen"])
                    };
                    vendorCoupons.AddCoupon((short)(vendorCoupons.Count + 1), vc, "");
                }
            }
            _performancelog.Debug($"End,TenderService,GetAllVendorCoupon,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return vendorCoupons;
        }

        /// <summary>
        /// Method to load GC tenders
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Gift cert payment</returns>
        public GCPayment Load_GCTenders(int tillNumber, int saleNumber, DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,Load_GCTenders,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            DataTable rs;
            var gcPayment = new GCPayment
            {
                Till_Num = (byte)tillNumber,
                Sale_Num = saleNumber
            };
            if (tillNumber == 0)
            {
                if (saleNumber == 0)
                {
                    return null;
                }
                rs = GetRecords("select * from GCTenders where SALE_NO=" + Convert.ToString(saleNumber), dataSource);
            }
            else
            {
                rs = GetRecords("select * from GCTenders where TILL_NUM=" + Convert.ToString(tillNumber) + " and SALE_NO=" + Convert.ToString(saleNumber), dataSource);
            }

            foreach (DataRow dataRow in rs.Rows)
            {
                var gc = new GCTender();
                gc.Line_No = Convert.ToInt16(dataRow["Line_No"]);
                gc.CertificateNum = Convert.ToString(dataRow["CertificateNum"]);
                gc.SaleAmount = Convert.ToDecimal(dataRow["SaleAmount"]);
                gc.CertType = Convert.ToString(dataRow["CertType"]);
                gc.Balance = Convert.ToDecimal(dataRow["Balance"]);
                gc.RefNum = Convert.ToString(dataRow["RefNum"]);
                gc.ExpDate = Convert.ToString(dataRow["ExpDate"]);
                gc.TransactionTime = Convert.ToDateTime(dataRow["TransactionTime"]);
                gc.TermID = Convert.ToString(dataRow["TerminalID"]);
                gc.Sequence = Convert.ToString(dataRow["SequenceNumber"]);
                gc.Message = Convert.ToString(dataRow["Message"]);
                gcPayment.Add_a_Line(gc);
            }
            _performancelog.Debug($"End,TenderService,Load_GCTenders,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return gcPayment;
        }

        /// <summary>
        /// Method to save new GC tender
        /// </summary>
        /// <param name="oGcLine">Gift cert line</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        public void Save_New_GCTender(GCTender oGcLine, int saleNumber, int tillNumber, DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,Save_New_GCTender,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            DataTable sGcTender = new DataTable();
            short mLine;
            _connection = new SqlConnection(GetConnectionString(dataSource));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from GCTenders where TILL_NUM=" + tillNumber + " AND SALE_NO=" + Convert.ToString(saleNumber), _connection);
            _adapter.Fill(sGcTender);
            var dataRow = sGcTender.NewRow();
            dataRow["Till_Num"] = tillNumber;
            dataRow["Sale_No"] = saleNumber;
            if (sGcTender.Rows.Count == 0)
            {
                mLine = 1;
            }
            else
            {
                //rsTmp = GetRecords("select max(Line_No) as MaxLine from GCTenders Where TILL_NUM=" + tillNumber + " AND SALE_NO=" + Convert.ToString(saleNumber), dataSource);
                var line = CommonUtility.GetShortValue(sGcTender.Rows[0]["MaxLine"]) == 0 ? 0 : CommonUtility.GetShortValue(sGcTender.Rows[0]["MaxLine"]) + 1;
                mLine = (short)line;
            }
            oGcLine.Line_No = mLine;
            dataRow["Line_No"] = mLine;
            dataRow["CertificateNum"] = oGcLine.CertificateNum;
            dataRow["SaleAmount"] = oGcLine.SaleAmount;
            dataRow["CertType"] = oGcLine.CertType;
            dataRow["Balance"] = oGcLine.Balance;
            dataRow["RefNum"] = oGcLine.RefNum;
            dataRow["ExpDate"] = oGcLine.ExpDate;
            dataRow["TransactionTime"] = oGcLine.TransactionTime;
            dataRow["TerminalID"] = oGcLine.TermID;
            dataRow["SequenceNumber"] = oGcLine.Sequence;
            dataRow["Message"] = oGcLine.Message.Substring(0, 2500);
            sGcTender.Rows.Add(dataRow);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            _performancelog.Debug($"End,TenderService,Save_New_GCTender,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to save to till
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        public void SaveToTill(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,SaveToTill,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            Execute("DELETE  From GCTenders WHERE Sale_No = " + Convert.ToString(saleNumber) + " AND TILL_NUM=" + Convert.ToString(tillNumber), DataSource.CSCTills);
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter("select * from GCTenders where TILL_NUM=" + Convert.ToString(tillNumber) + " AND SALE_NO=" + Convert.ToString(saleNumber), _connection);
            _adapter.Fill(_dataTable);
            var rsTmp = GetRecords("select * from GCTenders where TILL_NUM=" + Convert.ToString(tillNumber) + " AND SALE_NO=" + Convert.ToString(saleNumber), DataSource.CSCCurSale);
            foreach (DataRow tempFields in rsTmp.Rows)
            {
                var tillFields = _dataTable.NewRow();
                tillFields["Till_Num"] = tempFields["Till_Num"];
                tillFields["Sale_No"] = tempFields["Sale_Nok"];
                tillFields["Line_No"] = tempFields["Line_No"];
                tillFields["CertificateNum"] = tempFields["CertificateNum"];
                tillFields["SaleAmount"] = tempFields["SaleAmount"];
                tillFields["CertType"] = tempFields["CertType"];
                tillFields["Balance"] = tempFields["Balance"];
                tillFields["RefNum"] = tempFields["RefNum"];
                tillFields["ExpDate"] = tempFields["ExpDate"];
                tillFields["TransactionTime"] = tempFields["TransactionTime"];
                tillFields["SequenceNumber"] = tempFields["SequenceNumber"];
                tillFields["BatchNumber"] = tempFields["BatchNumber"];
                tillFields["TerminalID"] = tempFields["TerminalID"];
                tillFields["Message"] = tempFields["Message"];
                _dataTable.Rows.Add(tillFields);
            }
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            Execute("DELETE  From GCTenders WHERE Sale_No = " + Convert.ToString(saleNumber) + " AND TILL_NUM=" + Convert.ToString(tillNumber), DataSource.CSCCurSale);
            _performancelog.Debug($"End,TenderService,SaveToTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to get credit card tender
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderName">Tender name</param>
        /// <returns>Credit card</returns>
        public Credit_Card GetCreditCardTender(int saleNumber, int tillNumber, string tenderName)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetCreditCardTender,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var rcc = GetRecords("select * from cardTenders where sale_no = " + Convert.ToString(saleNumber) + " and tender_name = \'" + Convert.ToString(tenderName) + "\'and Till_Num=" + tillNumber, DataSource.CSCCurSale);
            var creditCard = new Credit_Card();
            if (rcc.Rows.Count > 0)
            {
                creditCard.Crd_Type = Convert.ToString(rcc.Rows[0]["Card_Type"]);
                creditCard.Name = Convert.ToString(rcc.Rows[0]["Card_Name"]);
                creditCard.Cardnumber = Convert.ToString(rcc.Rows[0]["Card_Number"]);
                creditCard.Expiry_Date = Convert.ToString(rcc.Rows[0]["Expiry_Date"]);
                creditCard.Card_Swiped = Convert.ToBoolean(rcc.Rows[0]["Swiped"]);
                creditCard.StoreAndForward = Convert.ToBoolean(rcc.Rows[0]["Store_Forward"]);
                creditCard.Trans_Amount = Convert.ToSingle(rcc.Rows[0]["Amount"]);
                creditCard.Authorization_Number = Convert.ToString(rcc.Rows[0]["Approval_Code"]);
                creditCard.Sequence_Number = Convert.ToString(rcc.Rows[0]["SequenceNumber"]);
                creditCard.Result = Convert.ToString(rcc.Rows[0]["Result"]);
                creditCard.TerminalID = Convert.ToString(rcc.Rows[0]["TerminalID"]);
                creditCard.DebitAccount = Convert.ToString(rcc.Rows[0]["DebitAccount"]);
                creditCard.ResponseCode = Convert.ToString(rcc.Rows[0]["ResponseCode"]);
                creditCard.ApprovalCode = Convert.ToString(rcc.Rows[0]["ISOCode"]);
                creditCard.Trans_Date = Convert.ToDateTime(rcc.Rows[0]["TransactionDate"]);
                creditCard.Trans_Time = Convert.ToDateTime(rcc.Rows[0]["TransactionTime"]);
                creditCard.Receipt_Display = Convert.ToString(rcc.Rows[0]["ReceiptDisplay"]);
                creditCard.Language = Convert.ToString(rcc.Rows[0]["Language"]);
                creditCard.Customer_Name = Convert.ToString(rcc.Rows[0]["CustomerName"]);
                creditCard.Vechicle_Number = Convert.ToString(rcc.Rows[0]["VechicleNo"]);
                creditCard.Driver_Number = Convert.ToString(rcc.Rows[0]["DriverNo"]);
                creditCard.ID_Number = Convert.ToString(rcc.Rows[0]["IdentificationNo"]);
                creditCard.Odometer_Number = Convert.ToString(rcc.Rows[0]["Odometer"]);
                creditCard.usageType = Convert.ToString(rcc.Rows[0]["CardUsage"]);
                creditCard.Print_VechicleNo = Convert.ToBoolean(rcc.Rows[0]["PrintVechicleNo"]);
                creditCard.Print_DriverNo = Convert.ToBoolean(rcc.Rows[0]["PrintDriverNo"]);
                creditCard.Print_IdentificationNo = Convert.ToBoolean(rcc.Rows[0]["PrintIdentificationNo"]);
                creditCard.Print_Usage = Convert.ToBoolean(rcc.Rows[0]["PrintUsage"]);
                creditCard.Print_Odometer = Convert.ToBoolean(rcc.Rows[0]["PrintOdometer"]);
                creditCard.Balance = Convert.ToDecimal(rcc.Rows[0]["Balance"]);
                creditCard.Quantity = Convert.ToDecimal(rcc.Rows[0]["Quantity"]);
                creditCard.CardProfileID = Convert.ToString(rcc.Rows[0]["CardProfileID"]);
                creditCard.PONumber = Convert.ToString(rcc.Rows[0]["PONumber"]);
                creditCard.Report = Convert.ToString(rcc.Rows[0]["Message"]);
            }
            _performancelog.Debug($"End,TenderService,GetCreditCardTender,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return creditCard;
        }

        /// <summary>
        /// Method to get list of cash buttons
        /// </summary>
        /// <returns>List of cash buttons</returns>
        public List<CashButton> GetCashButton()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetCashButton,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var cashButtonRecordset = GetRecords("SELECT * FROM   CashButtons Order by CashButtons.Cash_Button ", DataSource.CSCMaster);

            var cashbuttons = (from DataRow dataRow in cashButtonRecordset.Rows
                               select new CashButton
                               {
                                   Button = CommonUtility.GetStringValue(dataRow["Cash_Button"]),
                                   Value = CommonUtility.GetIntergerValue(dataRow["Cash_Value"])
                               }).ToList();
            _performancelog.Debug($"End,TenderService,GetCashButton,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return cashbuttons;
        }


        /// <summary>
        /// Method to delete sale vendor coupon for sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tendDesc">Tender description</param>
        public void DeleteSaleVendorCoupon(int saleNumber, int tillNumber, string tendDesc)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,DeleteSaleVendorCoupon,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            Execute("DELETE  From SaleVendorCoupon WHERE Sale_No = " + Convert.ToString(saleNumber) + " AND Till_NUM=" + tillNumber + " AND TendDesc=\'" + tendDesc.Trim() + "\' ", DataSource.CSCCurSale);
            _performancelog.Debug($"End,TenderService,DeleteSaleVendorCoupon,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to delete sale vendor coupon by coupon id
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tendDesc">Tender description</param>
        /// <param name="couponId">Coupon id</param>
        public void DeleteSaleVendorCouponByCouponId(int saleNumber, int tillNumber, string tendDesc, string couponId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,DeleteSaleVendorCouponByCouponId,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            Execute("DELETE  From SaleVendorCoupon WHERE Sale_No = " + Convert.ToString(saleNumber) + " AND TillNUM=" + tillNumber + " AND TendDesc=\'" + tendDesc.Trim() + "\' and CouponCode=\'" + couponId.Trim() + "\' ", DataSource.CSCCurSale);
            _performancelog.Debug($"End,TenderService,DeleteSaleVendorCouponByCouponId,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to remove one coupon line
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tendDesc">Tender description</param>
        /// <param name="couponId">Coupon id</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="sequenceNumber">Sequence number</param>
        public void RemoveOneCouponLine(int saleNumber, int tillNumber, string tendDesc, string couponId, short lineNumber, short sequenceNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,RemoveOneCouponLine,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            Execute("DELETE  From SaleVendorCoupon WHERE Sale_No = " + Convert.ToString(saleNumber) + " AND Till_NUM=" + tillNumber + " AND TendDesc=\'" + tendDesc.Trim() + "\' " +
                " AND LINE_NUM=" + Convert.ToString(lineNumber) + " AND SeqNumber=" + Convert.ToString(sequenceNumber) + " AND CouponCode=\'" + couponId.Trim() + "\' ", DataSource.CSCCurSale);
            _performancelog.Debug($"End,TenderService,RemoveOneCouponLine,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to get minimum display sequence
        /// </summary>
        /// <param name="cardClass">Card class</param>
        /// <returns>Minimum display sequence</returns>
        public byte GetMinDisplaySeq(string cardClass)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetMinDisplaySeq,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            byte minSeq = 0;
            var records = GetRecords("SELECT MIN(DisplaySeq) AS FT FROM TendMast  WHERE TendClass=\'" + cardClass + "\' AND (Inactive IS NULL OR Inactive=0)", DataSource.CSCMaster);

            if (records.Rows.Count > 0)
            {
                minSeq = CommonUtility.GetByteValue(records.Rows[0]["FT"]);
            }
            _performancelog.Debug($"End,TenderService,GetMinDisplaySeq,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return minSeq;
        }

        /// <summary>
        /// Method to get tender name
        /// </summary>
        /// <param name="tender">Tender</param>
        /// <returns>Tender name</returns>
        public string GetTenderName(string tender)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetTenderName,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var returnValue = tender;
            var rstender = GetRecords("select tenddesc from tendmast where  TendCode = \'" + tender + "\'", DataSource.CSCMaster);
            if (rstender.Rows.Count > 0)
            {
                returnValue = CommonUtility.GetStringValue(rstender.Rows[0]["TendDesc"]);
            }
            _performancelog.Debug($"End,TenderService,GetTenderName,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Method to get list of all sale vendor coupons
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderName">Tender name</param>
        /// <returns></returns>
        public List<SaleVendorCouponLine> GetSaleVendorCouponForTender(int saleNumber, int tillNumber, string tenderName)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetSaleVendorCouponForTender,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var saleVendorCoupons = new List<SaleVendorCouponLine>();
            var strSql = "Select * from SaleVendorCoupon Where TILL_NUM=" + Convert.ToString(tillNumber) + " AND SALE_NO=" + Convert.ToString(saleNumber) + " AND TendDesc=\'" + tenderName + "\'";
            var rsSvc = GetRecords(strSql, DataSource.CSCTills);
            if (rsSvc.Rows.Count > 0)
            {
                rsSvc = GetRecords(strSql, DataSource.CSCTrans);
            }
            if (rsSvc.Rows.Count > 0)
            {
                saleVendorCoupons.Add(new SaleVendorCouponLine
                {
                    CouponCode = CommonUtility.GetStringValue(rsSvc.Rows[0]["CouponCode"]),
                    CouponName = CommonUtility.GetStringValue(rsSvc.Rows[0]["CouponName"]),
                    SerialNumber = CommonUtility.GetStringValue(rsSvc.Rows[0]["SerialNumber"]),
                    TotalValue = CommonUtility.GetDecimalValue(rsSvc.Rows[0]["TotalValue"])
                });
            }
            _performancelog.Debug($"End,TenderService,GetSaleVendorCouponForTender,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return saleVendorCoupons;
        }

        /// <summary>
        /// Method to save store credit
        /// </summary>
        /// <param name="storeCredit">Store credit</param>
        public void SaveCredit(Store_Credit storeCredit)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,SaveCredit,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            int scNum;
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter("Select   *  FROM     StoreCrd Order By StoreCrd.Sc_Num ", _connection);
            _adapter.Fill(_dataTable);

            if (_dataTable.Rows.Count == 0)
            {
                scNum = 1;
            }
            else
            {
                scNum = CommonUtility.GetIntergerValue(_dataTable.Rows[_dataTable.Rows.Count - 1]["Sc_Num"]) + 1;
            }

            storeCredit.Number = scNum;
            var dataRow = _dataTable.NewRow();
            dataRow["Sc_Num"] = storeCredit.Number;
            dataRow["Sc_Amount"] = storeCredit.Amount;
            dataRow["SC_Date"] = storeCredit.SC_Date;
            dataRow["Sc_sale"] = storeCredit.Sale_Number;
            dataRow["Sc_Cust"] = storeCredit.Customer;
            dataRow["SC_Expires_On"] = storeCredit.Expires_On;
            _dataTable.Rows.Add(dataRow);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            _performancelog.Debug($"End,TenderService,SaveCredit,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }

        /// <summary>
        /// Method to get list of gift cert credit
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Gift cert list</returns>
        public string[,] GetGcCredit(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetGCCredit,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var rs = GetRecords("select * from USEDGC where sale_no = " + Convert.ToString(saleNumber) + " AND Till_Num=" + tillNumber, DataSource.CSCCurSale);
            var i = (short)1;
            if (rs.Rows.Count > 0)
            {
                var gcRemove = new string[51, 3];
                foreach (DataRow dataRow in rs.Rows)
                {
                    gcRemove[i, 1] = Convert.ToString(dataRow["GC_NO"]);
                    gcRemove[i, 2] = Convert.ToString(dataRow["GC_Amount"]);
                    i++;
                }
                return gcRemove;
            }
            _performancelog.Debug($"End,TenderService,GetGCCredit,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return null;
        }

        /// <summary>
        /// Method to get store credit list
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Store credit list</returns>
        public string[,] GetScCredit(int saleNumber, int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetSCCredit,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var rs = GetRecords("select * from USEDSC where sale_no = " + Convert.ToString(saleNumber) + " AND Till_Num=" + tillNumber, DataSource.CSCCurSale);
            var i = (short)1;
            if (rs.Rows.Count > 0)
            {
                string[,] scRemove = new string[51, 3];
                foreach (DataRow dataRow in rs.Rows)
                {
                    scRemove[i, 1] = CommonUtility.GetStringValue(dataRow["SC_NO"]);
                    scRemove[i, 2] = CommonUtility.GetStringValue(dataRow["SC_AMOUNT"]);
                    i++;
                }
                return scRemove;
            }
            _performancelog.Debug($"End,TenderService,GetSCCredit,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return null;
        }

        /// <summary>
        /// Method to add gift cert to CSCCurSale 
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="giftCerts">List of gift certs</param>
        public void AddGcToDbTemp(int saleNumber, int tillNumber, List<GiftCert> giftCerts)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,AddGCToDbTemp,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCCurSale));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter("select * from USEDGC", _connection);
            _adapter.Fill(_dataTable);
            foreach (var giftCert in giftCerts)
            {
                if (string.IsNullOrEmpty(giftCert.GcNumber))
                {
                    break;
                }
                var fields = _dataTable.NewRow();
                fields["Till_Num"] = tillNumber;
                fields["Sale_No"] = saleNumber;
                fields["GC_NO"] = giftCert.GcNumber;
                fields["GC_Amount"] = giftCert.GcAmount;
                _dataTable.Rows.Add(fields);
            }
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            _performancelog.Debug($"End,TenderService,AddGCToDbTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to remove gift certificate
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="gcCredits">Gift certificate credits</param>
        public void RemoveGc(int saleNumber, string[,] gcCredits)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,RemoveGC,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            for (var n = 1; n <= (gcCredits.Length - 1); n++)
            {
                if (gcCredits[n, 1] == null)
                {
                    break;
                }
                var query = "SELECT *  FROM   GiftCert  WHERE  GiftCert.GC_Num    = " + Convert.ToString(Conversion.Val(gcCredits[n, 1]), CultureInfo.InvariantCulture) + " AND " + "       GiftCert.GC_Amount = " + Convert.ToString(Conversion.Val(gcCredits[n, 2]), CultureInfo.InvariantCulture) + " ";
                var rs = GetRecords(query, DataSource.CSCMaster);

                // added to save gift certificates used in
                // a sale before being deleted from GiftCert table
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter("select * from RemovedGC", _connection);
                _adapter.Fill(_dataTable);
                var dataRow = _dataTable.NewRow();
                dataRow["DeletedDate"] = DateTime.Now;
                dataRow["DeletedinSale_No"] = saleNumber;
                dataRow["GC_NUM"] = rs.Rows[0]["GC_NUM"];
                dataRow["GC_AMOUNT"] = rs.Rows[0]["GC_AMOUNT"];
                dataRow["GC_CUST"] = rs.Rows[0]["GC_CUST"];
                dataRow["GC_DATE"] = rs.Rows[0]["GC_DATE"];
                dataRow["GC_USER"] = rs.Rows[0]["GC_USER"];
                dataRow["GC_Expires_On"] = rs.Rows[0]["GC_Expires_On"];
                dataRow["Sale_No"] = rs.Rows[0]["Sale_No"];
                dataRow["Line_No"] = rs.Rows[0]["Line_No"];
                dataRow["GC_REGIST"] = rs.Rows[0]["GC_REGIST"];
                dataRow["GC_STORE"] = rs.Rows[0]["GC_STORE"];

                _dataTable.Rows.Add(dataRow);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
                Execute("delete  FROM   GiftCert  WHERE  GiftCert.GC_Num    = " + Convert.ToString(Conversion.Val(gcCredits[n, 1]), CultureInfo.InvariantCulture) + " AND " + "       GiftCert.GC_Amount = " + Convert.ToString(Conversion.Val(gcCredits[n, 2]), CultureInfo.InvariantCulture) + " ", DataSource.CSCMaster);
            }
            _connection.Close();
            _adapter?.Dispose();

            Execute("delete from USEDGC where sale_no = " + Convert.ToString(saleNumber), DataSource.CSCCurSale); //crash recovery - 
            _performancelog.Debug($"End,TenderService,RemoveGC,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to remove used gift certificate
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        public void RemoveUsedGc(int saleNumber)
        {
            Execute("delete from USEDGC where sale_no = " + Convert.ToString(saleNumber), DataSource.CSCCurSale); //crash recovery - 

        }

        /// <summary>
        /// Method to remove used store credit
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        public void RemoveUsedSc(int saleNumber)
        {
            Execute("delete from USEDSC where sale_no = " + Convert.ToString(saleNumber), DataSource.CSCCurSale); //crash recovery - 

        }

        /// <summary>
        /// Method to get list of gift certificates
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of gift certificates</returns>
        public List<GiftCert> GetAllGiftCert(DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetAllGiftCert,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var giftCerts = new List<GiftCert>();
            var query = "SELECT Round(GiftCert.GC_Num,0) as [Number],  GiftCert.GC_Date as [Sold On], Round(GiftCert.GC_Amount,2) as [Amount]  FROM   GiftCert  WHERE  GiftCert.GC_Expires_On >= \'" + DateAndTime.Today.ToString("yyyyMMdd") + "\' ORDER BY GiftCert.GC_Num ";
            var rs = GetRecords(query, dataSource);
            foreach (DataRow row in rs.Rows)
            {
                giftCerts.Add(new GiftCert
                {
                    GcNumber = CommonUtility.GetStringValue(row["Number"]),
                    GcDate = CommonUtility.GetDateTimeValue(row["Sold On"]),
                    GcAmount = CommonUtility.GetDecimalValue(row["Amount"])
                });
            }
            _performancelog.Debug($"End,TenderService,GetAllGiftCert,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return giftCerts;
        }

        /// <summary>
        /// Method to get list of expiredd gift certiifcates
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of gift certificates</returns>
        public List<GiftCert> GetExpiredGiftCert(DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetExpiredGiftCert,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var giftCerts = new List<GiftCert>();
            var query = "SELECT Round(GiftCert.GC_Num,0) as [Number],  GiftCert.GC_Date as [Sold On],  Round(GiftCert.GC_Amount,2) as [Amount], GiftCert.GC_Expires_On as [Expires_On]  FROM   GiftCert  WHERE  GiftCert.GC_Expires_On < \'" + DateAndTime.Today.ToString("yyyyMMdd") + "\' ORDER BY GiftCert.GC_Num ";

            var rs = GetRecords(query, dataSource);
            foreach (DataRow fields in rs.Rows)
            {
                giftCerts.Add(new GiftCert
                {
                    GcNumber = CommonUtility.GetStringValue(fields["Number"]),
                    GcDate = CommonUtility.GetDateTimeValue(fields["Sold On"]),
                    GcAmount = CommonUtility.GetDecimalValue(fields["Amount"]),
                    GcExpiresOn = CommonUtility.GetDateTimeValue(fields["Expires_On"])
                });
            }
            _performancelog.Debug($"End,TenderService,GetExpiredGiftCert,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return giftCerts;
        }

        /// <summary>
        /// Method to get card type by tender code
        /// </summary>
        /// <param name="tenderCode">Tender code</param>
        /// <returns>Card type</returns>
        public string GetCardTypeByTenderCode(string tenderCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetCardTypeByTenderCode,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            string returnValue = "";
            var rsTendCard = GetRecords("Select * from TendCard where TENDCODE=\'" + tenderCode + "\'", DataSource.CSCMaster);
            if (rsTendCard.Rows.Count > 0)
            {
                var cardCode = CommonUtility.GetStringValue(rsTendCard.Rows[0]["CardCode"]);

                var rsCardCode = GetRecords("select CardType from Cards where CardCode=\'" + cardCode + "\'", DataSource.CSCMaster);
                if (rsCardCode.Rows.Count > 0)
                {
                    returnValue = CommonUtility.GetStringValue(rsCardCode.Rows[0]["CardType"]);
                }
            }
            _performancelog.Debug($"End,TenderService,GetCardTypeByTenderCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// method to get the card type associated with the tender by tender code
        /// </summary>
        /// <param name="tenderCode"></param>
        /// <returns></returns>
        public int GetCardCode(string tenderCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetCardTypeByTenderCode,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            int returnValue = 0 ;
            var rsTendCard = GetRecords("Select * from TendCard where TENDCODE=\'" + tenderCode + "\'", DataSource.CSCMaster);
            if (rsTendCard.Rows.Count > 0)
            {
                var cardCode = CommonUtility.GetIntergerValue(rsTendCard.Rows[0]["CardCode"]);
                returnValue = cardCode;
            }
            return returnValue;
        }


        /// <summary>
        /// Method to load tender card
        /// </summary>
        /// <param name="cardId">Card id</param>
        /// <returns>Tender card</returns>
        public TenderCard LoadTenderCard(int cardId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,LoadTenderCard,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            TenderCard tenderCard = new TenderCard();

            var rs = GetRecords("Select *  FROM   TendCard  WHERE  TendCard.cardcode =" + Convert.ToString(cardId), DataSource.CSCMaster);


            if (rs.Rows.Count > 0)
            {
                var fields = rs.Rows[0];
                tenderCard.CardCode = cardId.ToString();
                tenderCard.TenderCode = CommonUtility.GetStringValue(fields["TendCode"]);
                tenderCard.ReportGroup = CommonUtility.GetShortValue(fields["ReportGroup"]);
                tenderCard.BankCardID = CommonUtility.GetStringValue(fields["BankCardID"]);
                tenderCard.CallTheBank = CommonUtility.GetBooleanValue(fields["Call"]);
                tenderCard.SignatureLine = CommonUtility.GetBooleanValue(fields["Signature"]);
                tenderCard.DiscountRate = CommonUtility.GetFloatValue(fields["DiscountRate"]);
                tenderCard.DiscountType = CommonUtility.GetStringValue(fields["DiscountType"]);
                tenderCard.RefundAllowed = CommonUtility.GetBooleanValue(fields["RefundAllowed"]);
                tenderCard.PurchaseLimit = CommonUtility.GetFloatValue(fields["PurchaseLimit"]);
                tenderCard.FloorLimit = CommonUtility.GetFloatValue(fields["FloorLimit"]);
                tenderCard.PrintCopies = CommonUtility.GetShortValue(fields["PrintCopies"]);
                tenderCard.ReceiptTotalText = CommonUtility.GetStringValue(fields["ReceiptTotalText"]);
                tenderCard.LimitToSale = CommonUtility.GetBooleanValue(fields["LimitToSale"]);
                tenderCard.MaxCashBack = CommonUtility.GetFloatValue(fields["MaxCashBack"]);
                tenderCard.AllowPayPump = CommonUtility.GetBooleanValue(fields["AllowPayPump"]);
                tenderCard.CardProductRestrict = CommonUtility.GetBooleanValue(fields["CardProductRestrict"]);
                tenderCard.PrintDriver = CommonUtility.GetBooleanValue(fields["PrintDriverNo"]);
                tenderCard.PrintIdentification = CommonUtility.GetBooleanValue(fields["PrintIdentificationNo"]);
                tenderCard.PrintOdometer = CommonUtility.GetBooleanValue(fields["PrintOdometer"]);
                tenderCard.PrintVechicle = CommonUtility.GetBooleanValue(fields["PrintVechicleNo"]);
                tenderCard.PrintUsage = CommonUtility.GetBooleanValue(fields["PrintUsage"]);

                tenderCard.OptDataProfileID = CommonUtility.GetStringValue(fields["OptDataProfileID"]);

            }
            _performancelog.Debug($"End,TenderService,LoadTenderCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return tenderCard;
        }


        //  
        /// <summary>
        /// Method to get card type by tender code
        /// </summary>
        /// <param name="tenderCode">Tender code</param>
        /// <returns>Card type</returns>
        public string GetCardType(string tenderCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetCardType,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var returnValue = "";
            var rsTendCard = GetRecords("select * from TendCard where TENDCODE=\'" + tenderCode + "\'", DataSource.CSCMaster);
            if (rsTendCard.Rows.Count == 0)
            {
                _performancelog.Debug($"End,TenderService,GetCardType,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return returnValue;
            }
            var rsCards = GetRecords("select CardType from Cards where CardCode=" + Convert.ToString(rsTendCard.Rows[0]["CardCode"]), DataSource.CSCMaster);
            if (rsCards.Rows.Count == 0)
            {
                _performancelog.Debug($"End,TenderService,GetCardType,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return returnValue;
            }
            returnValue = Strings.Trim(Convert.ToString(rsCards.Rows[0]["CardType"]));
            _performancelog.Debug($"End,TenderService,GetCardType,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }


        /// <summary>
        /// Method to save ar payment
        /// </summary>
        /// <param name="arPayment">AR payment</param>
        /// <param name="paymentNumber">Payment number</param>
        public void SaveArPayment(AR_Payment arPayment, int paymentNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,SaveArPayment,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from Payment", _connection);
            _adapter.Fill(_dataTable);
            var fields = _dataTable.NewRow();
            fields["Py_Num"] = paymentNumber;
            fields["Py_Date"] = DateTime.Now;
            fields["Py_Client"] = arPayment.Customer.Code;
            fields["Py_Amt"] = arPayment.Amount;
            fields["Sale_Num"] = arPayment.Sale_Num;
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _dataTable.Rows.Add(fields);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _performancelog.Debug($"End,TenderService,SaveArPayment,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection.Close();
            _adapter?.Dispose();
        }

        /// <summary>
        /// Method to get maximum payment number
        /// </summary>
        /// <returns>Maximum payment number</returns>
        public int GetMaxPaymentNumber()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetMaxPaymentNumber,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var rs = GetRecords("Select Max(Py_Num) as [PNum]  FROM Payment ", DataSource.CSCTrans);
            int paymentNumber;
            if (rs.Rows.Count == 0)
            {
                paymentNumber = 1;
            }
            else
            {
                paymentNumber = CommonUtility.GetIntergerValue(rs.Rows[0]["PNum"]) + 1;
            }
            _performancelog.Debug($"End,TenderService,GetMaxPaymentNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return paymentNumber;

        }

        /// <summary>
        /// Method to update coupon
        /// </summary>
        /// <param name="couponId">Coupon id</param>
        public void UpdateCoupon(string couponId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,UpdateCoupon,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from Coupon where CouponID=\'" + couponId + "\'", _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count > 0)
            {
                var fields = _dataTable.Rows[0];
                fields["USED"] = true;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            _performancelog.Debug($"End,TenderService,UpdateCoupon,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to remove store credit
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="storeCredits">List of store credits</param>
        public void RemoveSc(int saleNumber, string[,] storeCredits)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,RemoveSC,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            for (var n = 1; n <= (storeCredits.Length - 1); n++)
            {
                if (storeCredits[n, 1] == null)
                {
                    break;
                }
                var query = "SELECT *  FROM   StoreCrd  WHERE  StoreCrd.SC_NUM    = " + Convert.ToString(Conversion.Val(storeCredits[n, 1]), CultureInfo.InvariantCulture) + " AND " + "       StoreCrd.SC_AMOUNT = " + Convert.ToString(Conversion.Val(storeCredits[n, 2]), CultureInfo.InvariantCulture) + " ";
                var rs = GetRecords(query, DataSource.CSCMaster);

                // Nicolette added to save gift certificates used in
                // a sale before being deleted from GiftCert table
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter("select * from RemovedSC", _connection);
                _adapter.Fill(_dataTable);
                DataRow dataRow = _dataTable.NewRow();
                dataRow["DeletedDate"] = DateTime.Now;
                dataRow["DeletedinSale_No"] = saleNumber;
                dataRow["SC_NUM"] = rs.Rows[0]["SC_NUM"];
                dataRow["SC_AMOUNT"] = rs.Rows[0]["SC_AMOUNT"];
                dataRow["SC_DATE"] = rs.Rows[0]["SC_DATE"];
                dataRow["SC_SALE"] = rs.Rows[0]["SC_SALE"];
                dataRow["SC_CUST"] = rs.Rows[0]["SC_CUST"];
                dataRow["SC_Expires_ON"] = rs.Rows[0]["SC_Expires_ON"];
                _dataTable.Rows.Add(dataRow);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
                Execute("Delete  FROM   StoreCrd  WHERE  StoreCrd.SC_NUM    = " + Convert.ToString(Conversion.Val(storeCredits[n, 1]), CultureInfo.InvariantCulture) + " AND " + "       StoreCrd.SC_AMOUNT = " + Convert.ToString(Conversion.Val(storeCredits[n, 2]), CultureInfo.InvariantCulture) + " ", DataSource.CSCMaster);
            }
            _connection.Close();
            _adapter?.Dispose();

            Execute("delete from USEDSC where sale_no = " + Convert.ToString(saleNumber), DataSource.CSCCurSale); //crash recovery - 
            _performancelog.Debug($"End,TenderService,RemoveSC,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }

        /// <summary>
        /// Method to get maximum ROA payment
        /// </summary>
        /// <returns>Maximum value</returns>
        public int GetMaxRoaPayment()
        {
            int paymentNum;
            var rs = GetRecords("Select *  FROM ROAPayments Order By ROAPayments.Payment_Num ", DataSource.CSCTrans);
            if (rs.Rows.Count == 0)
            {
                paymentNum = 1;
            }
            else
            {
                paymentNum = CommonUtility.GetIntergerValue(rs.Rows[rs.Rows.Count - 1]["Payment_Num"]) + 1;
            }
            return paymentNum;
        }

        /// <summary>
        /// Method to add ROA payment
        /// </summary>
        /// <param name="payment">Payment</param>
        /// <param name="saleNumber">Sale number</param>
        public void AddRoaPayment(Payment payment, int saleNumber)
        {
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("Select *  FROM ROAPayments ", _connection);
            _adapter.Fill(_dataTable);

            var fields = _dataTable.NewRow();
            fields["Payment_Num"] = GetMaxRoaPayment();
            fields["Payment_Date"] = DateTime.Now;

            if (payment.Card == null)
            {

                fields["Account_num"] = payment.Account;
                fields["Card_Num"] = "";
                fields["Swiped"] = false;
            }
            else
            {
                fields["Account_num"] = "";


                //  - Encrypt Card numbers based on Policy + card Setup for Encryption. But for debit and credit cards only look the policy- not the card settings( always consider card setting as true)-(polcy checking is inside the Encryptdecrypt)
                //        rs![Card_Num] = EncryptDecrypt.Encrypt(Payment.Card.CardNumber)

                fields["Card_Num"] = payment.Card.Cardnumber;
                // 




                fields["Swiped"] = payment.Card.Card_Swiped;
            }

            fields["Amount"] = payment.Amount;
            fields["Sale_Num"] = saleNumber;
            _dataTable.Rows.Add(fields);
            var builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
        }


        /// <summary>
        /// Method to get list of store credits
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of store credits</returns>
        public List<Store_Credit> GetAllStoreCredits(DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetAllStoreCredits,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var storeCredits = new List<Store_Credit>();
            var query = "SELECT Round(StoreCrd.SC_Num,0) as [Number], " + "       StoreCrd.SC_Date as [Sold On], " + "       Round(StoreCrd.SC_Amount,2) as [Amount]  FROM   StoreCrd  WHERE  StoreCrd.SC_Expires_On >= \'" + DateAndTime.Today.ToString("yyyyMMdd") + "\' " + "ORDER BY StoreCrd.SC_Num ";
            var rs = GetRecords(query, dataSource);
            foreach (DataRow fields in rs.Rows)
            {
                storeCredits.Add(new Store_Credit
                {
                    Number = CommonUtility.GetIntergerValue(fields["Number"]),
                    SC_Date = CommonUtility.GetDateTimeValue(fields["Sold On"]),
                    Amount = CommonUtility.GetDecimalValue(fields["Amount"])
                });
            }
            _performancelog.Debug($"End,TenderService,GetAllStoreCredits,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return storeCredits;
        }

        /// <summary>
        /// Method to get list of expired store credits
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of store credits</returns>
        public List<Store_Credit> GetExpiredStoreCredits(DataSource dataSource)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,GetExpiredStoreCredits,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var storeCredits = new List<Store_Credit>();
            var query = "SELECT Round(StoreCrd.SC_Num,0) as [Number], " + "       StoreCrd.SC_Date as [Sold On], " + "       Round(StoreCrd.SC_Amount,2) as [Amount], " + "       StoreCrd.SC_Expires_On as [Expires_On]  FROM   StoreCrd  WHERE  StoreCrd.SC_Expires_On < \'" + DateAndTime.Today.ToString("yyyyMMdd") + "\' " + "ORDER BY StoreCrd.SC_Num ";

            var rs = GetRecords(query, dataSource);
            foreach (DataRow fields in rs.Rows)
            {
                storeCredits.Add(new Store_Credit
                {
                    Number = CommonUtility.GetIntergerValue(fields["Number"]),
                    SC_Date = CommonUtility.GetDateTimeValue(fields["Sold On"]),
                    Amount = CommonUtility.GetDecimalValue(fields["Amount"]),
                    Expires_On = CommonUtility.GetDateTimeValue(fields["Expires_On"])
                });
            }
            _performancelog.Debug($"End,TenderService,GetExpiredStoreCredits,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return storeCredits;
        }

        /// <summary>
        /// Method to add store credit to CSCCurSale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="storeCredits">List of store credits</param>
        public void AddScToDbTemp(int saleNumber, int tillNumber, List<Store_Credit> storeCredits)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TenderService,AddSCToDbTemp,{string.Empty},{dateStart:hh.mm.ss.ffffff}");


            _connection = new SqlConnection(GetConnectionString(DataSource.CSCCurSale));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from USEDSC", _connection);
            _adapter.Fill(_dataTable);
            foreach (var storeCredit in storeCredits)
            {
                if (string.IsNullOrEmpty(storeCredit.Number.ToString()))
                {
                    break;
                }
                DataRow fields = _dataTable.NewRow();
                fields["Till_Num"] = tillNumber;
                fields["Sale_No"] = saleNumber;
                fields["SC_NO"] = storeCredit.Number;
                fields["SC_Amount"] = storeCredit.Amount;
                _dataTable.Rows.Add(fields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
            }
            _adapter?.Dispose();
            _connection.Close();
            _performancelog.Debug($"End,TenderService,AddSCToDbTemp,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }

        /// <summary>
        /// Method to check if card is available
        /// </summary>
        /// <returns>True or false</returns>
        public bool IsCardAvailable()
        {
            var rs = GetRecords("SELECT TendMast.*  FROM TendMast INNER JOIN (TendCard INNER JOIN Cards " + "     ON TendCard.CardCode = Cards.CardCode) " + "     ON TendMast.TendCode = TendCard.TendCode  WHERE Cards.AllowPayment = 1 " + "ORDER BY TendMast.TendCode ", DataSource.CSCMaster);
            if (rs.Rows.Count == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method to set coupon to void
        /// </summary>
        /// <param name="couponId">Coupon Id</param>
        public void SetCouponToVoid(string couponId)
        {
            var query = "select * from Coupon where CouponID=\'" + couponId + "\'";
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                var field = _dataTable.Rows[0];
                field["Void"] = false;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
        }


        public string GetTendClass(string tenderCode)
        {
            var result = string.Empty;
            var query = "SELECT TENDCLASS from TENDMAST where TENDCODE=\'" + tenderCode + "\'";

            var rs = GetRecords(query, DataSource.CSCMaster);
            if (rs.Rows.Count > 0)
            {
                result = CommonUtility.GetStringValue(rs.Rows[0]["TENDCLASS"]);
            }
            return result;
        }

    }
}

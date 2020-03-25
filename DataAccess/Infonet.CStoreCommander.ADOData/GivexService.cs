using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infonet.CStoreCommander.ADOData
{
    public class GivexService : SqlDbService, IGivexService
    {
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        /// <summary>
        /// Save Close Batch
        /// </summary>
        /// <param name="cashoutId">CashoutId</param>
        /// <param name="reports">Reports</param>
        public void SaveCloseBatch(string cashoutId, string reports)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,GivexService,SaveCloseBatch,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            int closeId;

            var dt = GetRecords("select MAX(ID) as MaxID from GiveXClose", DataSource.CSCTrans);
            if (dt == null || dt.Rows.Count == 0)
            {
                closeId = 1;

            }
            else if (DBNull.Value.Equals(dt.Rows[0]["MAXID"]))
            {
                closeId = 1;
            }
            else
            {
                closeId = CommonUtility.GetIntergerValue(dt.Rows[0]["MAXID"]) + 1;
            }
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from GiveXClose where ID = " + closeId, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                DataRow rsfields = _dataTable.NewRow();
                rsfields["ID"] = closeId;
                rsfields["CashoutId"] = cashoutId.Trim();
                rsfields["BatchDate"] = DateTime.Now.ToShortDateString();
                rsfields["BatchTime"] = DateTime.Now.ToShortTimeString();
                rsfields["Report"] = reports.Trim();
                _dataTable.Rows.Add(rsfields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            _performancelog.Debug($"End,GivexService,SaveCloseBatch,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }



        /// <summary>
        /// Get a Valid GiveX Stock
        /// </summary>
        /// <returns>stockCode</returns>
        public string GetValidGiveXStock()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,GivexService,GetValidGiveXStock,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            string strResult = "";

            var returnValue = "";
            var strSql = "select * from P_SET where P_NAME=\'GiftType\' and P_SET=\'GiveX\'";
            var rsPSet = GetRecords(strSql, DataSource.CSCAdmin);
            if (rsPSet == null || rsPSet.Rows.Count == 0)
            {
                return returnValue;
            }

            var strNewSql = strSql + " and P_LEVEL=\'STOCK\'";
            rsPSet = GetRecords(strNewSql, DataSource.CSCAdmin);
            if (rsPSet != null && rsPSet.Rows.Count != 0)
            {
                returnValue = Strings.Trim(Convert.ToString(rsPSet.Rows[0]["P_VALUE"]));
            }
            else
            {
                strNewSql = strSql + " and P_LEVEL=\'SUBDETAIL\'";
                rsPSet = GetRecords(strNewSql, DataSource.CSCAdmin);
                if (rsPSet != null && rsPSet.Rows.Count != 0)
                {
                    strResult = Strings.Trim(Convert.ToString(rsPSet.Rows[0]["P_VALUE"]));
                }
                else
                {
                    strNewSql = strSql + " and P_LEVEL=\'SUBDEPT\'";
                    rsPSet = GetRecords(strNewSql, DataSource.CSCAdmin);
                    if (rsPSet != null && rsPSet.Rows.Count != 0)
                    {
                        strResult = Strings.Trim(Convert.ToString(rsPSet.Rows[0]["P_VALUE"]));
                    }
                    else
                    {
                        strNewSql = strSql + " and P_LEVEL=\'DEPT\'";
                        rsPSet = GetRecords(strNewSql, DataSource.CSCAdmin);
                        if (rsPSet != null && rsPSet.Rows.Count != 0)
                        {
                            strResult = Strings.Trim(Convert.ToString(rsPSet.Rows[0]["P_VALUE"]));
                        }
                    }
                }
                if (strResult.Trim() != "")
                {
                    var arr = Strings.Split(Expression: strResult, Delimiter: Convert.ToString(Strings.Chr(255)), Compare: CompareMethod.Text);
                    strSql = "";
                    var n = (short)(arr.Length - 1);
                    short i;
                    for (i = 0; i <= n; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                var strDept = arr[0];
                                strSql = "select * from STOCKMST where DEPT=\'" + strDept + "\'";
                                break;
                            case 1:
                                var strSubDept = arr[1];
                                strSql = strSql + " and SUB_DEPT=\'" + strSubDept + "\'";
                                break;
                            case 2:
                                var strSubDetail = arr[2];
                                strSql = strSql + " and Sub_Detail=\'" + strSubDetail + "\'";
                                break;
                        }
                    }
                    if (strSql.Trim() != "")
                    {
                        var rsStock = GetRecords(strSql, DataSource.CSCMaster);
                        if (rsStock != null && rsStock.Rows.Count > 0)
                        {
                            returnValue = Strings.Trim(Convert.ToString(rsStock.Rows[0]["Stock_Code"]));
                        }
                    }
                }
            }

            _performancelog.Debug($"End,GivexService,GetValidGiveXStock,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }


        /// <summary>
        /// Get Givex Report details
        /// </summary>
        /// <param name="reportDate"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        public List<GivexDetails> GetGivexReportDetails(DateTime reportDate, string timeFormat)
        {
            var result = new List<GivexDetails>();
            var rs = GetRecords("Select ID, CashOutID,BatchDate,BatchTime,Report From   GiveXClose Where  BatchDate=\'" + reportDate.ToString("yyyyMMdd") + "\' ORDER BY ID DESC ", DataSource.CSCTrans);

            if (rs != null && rs.Rows.Count > 0)
            {
                foreach (DataRow dr in rs.Rows)
                {
                    GivexDetails record = new GivexDetails();

                    record.Id = CommonUtility.GetIntergerValue(dr[0]);
                    record.CashOut = CommonUtility.GetStringValue(dr[1]);
                    record.BatchDate = CommonUtility.GetDateTimeValue(dr[2]).ToString("MM/dd/yyyy");
                    record.BatchTime = CommonUtility.GetDateTimeValue(dr[3]).ToString(timeFormat);
                    record.Report = CommonUtility.GetStringValue(dr[4]);
                    result.Add(record);
                }
            }
            return result;
        }

    }//end class
}//end namespace

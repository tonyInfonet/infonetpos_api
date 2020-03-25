using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// Till Service
    /// </summary>
    public class TillService : SqlDbService, ITillService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Get the list of tills 
        /// </summary>
        /// <param name="posId">Pos id</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftDate">Shift date</param>
        /// <param name="userCode">User code</param>
        /// <param name="active">Is active or not</param>
        /// <param name="process">Is processing or not</param>
        /// <returns>Tills</returns>
        public List<Till> GetTills(int? posId, int? tillNumber, DateTime? shiftDate,
            string userCode, int? active, int? process)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillService,GetTills,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var whereClause = new StringBuilder();
            string query;

            if (posId.HasValue)
            {
                whereClause.Append($" AND POSID={posId.Value}");

            }

            if (tillNumber.HasValue)
            {
                whereClause.Append($" AND TILL_NUM={tillNumber.Value} ");

            }

            if (shiftDate.HasValue)
            {
                whereClause.Append($" AND ShiftDate={shiftDate.Value}");

            }

            if (!string.IsNullOrEmpty(userCode))
            {
                whereClause.Append($" AND UserLoggedOn='{userCode}'");

            }
            if (active.HasValue)
            {
                whereClause.Append($" AND ACTIVE={active.Value}");

            }
            if (process.HasValue)
            {
                whereClause.Append($" AND PROCESS={process.Value}");

            }
            if (whereClause.Length != 0)
            {
                var condition = whereClause.ToString().Substring(5, whereClause.Length - 5);
                query = $"Select * from Tills where {condition}";
            }
            else
            {
                query = "Select * from Tills";
            }
            var sTills = GetRecords(query, DataSource.CSCMaster);

            var tills = new List<Till>();
            foreach (DataRow row in sTills.Rows)
            {
                var till = new Till
                {
                    Number = CommonUtility.GetShortValue(row["TILL_NUM"]),
                    Active = CommonUtility.GetBooleanValue(row["ACTIVE"]),
                    Processing = CommonUtility.GetBooleanValue(row["PROCESS"]),
                    Float = CommonUtility.GetDecimalValue(row["FLOAT"]),
                    BonusFloat = CommonUtility.GetDecimalValue(row["CashBonusFloat"]),
                    Cash = CommonUtility.GetDecimalValue(row["CASH"]),
                    Date_Open = CommonUtility.GetDateTimeValue(row["DATE_OPEN"]),
                    Time_Open = CommonUtility.GetDateTimeValue(row["TIME_OPEN"]),
                    ShiftDate = CommonUtility.GetDateTimeValue(row["ShiftDate"]),
                    Shift = CommonUtility.GetShortValue(row["ShiftNumber"]),
                    UserLoggedOn = CommonUtility.GetStringValue(row["UserLoggedOn"]),
                    POSId = CommonUtility.GetIntergerValue(row["POSID"]),
                    CashBonus = CommonUtility.GetDecimalValue(row["CashBonus"])
                };
                tills.Add(till);
            }

            _performancelog.Debug($"End,TillService,GetTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return tills;
        }

        /// <summary>
        /// Get the Till by Till Number
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Till</returns>
        public Till GetTill(int tillNumber)
        {
            var sTill = GetRecords("Select * From Tills where till_num=" + tillNumber, DataSource.CSCMaster);

            if (sTill.Rows.Count > 0)
            {
                var row = sTill.Rows[0];
                var till = new Till
                {
                    Number = CommonUtility.GetShortValue(row["TILL_NUM"]),
                    Active = CommonUtility.GetBooleanValue(row["ACTIVE"]),
                    Processing = CommonUtility.GetBooleanValue(row["PROCESS"]),
                    Float = CommonUtility.GetDecimalValue(row["FLOAT"]),
                    BonusFloat = CommonUtility.GetDecimalValue(row["CashBonusFloat"]),
                    Cash = CommonUtility.GetDecimalValue(row["CASH"]),
                    Date_Open = CommonUtility.GetDateTimeValue(row["DATE_OPEN"]),
                    Time_Open = CommonUtility.GetDateTimeValue(row["TIME_OPEN"]),
                    ShiftDate = CommonUtility.GetDateTimeValue(row["ShiftDate"]),
                    Shift = CommonUtility.GetShortValue(row["ShiftNumber"]),
                    UserLoggedOn = CommonUtility.GetStringValue(row["UserLoggedOn"]),
                    POSId = CommonUtility.GetIntergerValue(row["POSID"]),
                    CashBonus = CommonUtility.GetDecimalValue(row["CashBonus"])
                   
                };
                return till;
            }
            return null;

        }


        /// <summary>
        /// Get the Till by Till Number
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Till</returns>
        public bool IsTillExists(int tillNumber)
        {
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            var query = "Select COUNT(*) From Tills where till_num=" + tillNumber;
            SqlCommand cmd = new SqlCommand(query, _connection);

            var count = CommonUtility.GetIntergerValue(cmd.ExecuteScalar());

            if (count > 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Get the tills which are not pay at pump
        /// </summary>
        /// <param name="payAtPumpTill">aPay at pump till</param>
        /// <returns>Tills</returns>
        public List<Till> GetnotPayAtPumpTill(int payAtPumpTill)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillService,GetnotPayAtPumpTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var strSql = "Select *  FROM   Tills  WHERE Tills.Till_Num <> " + payAtPumpTill + "";

            var sTills = GetRecords(strSql, DataSource.CSCMaster);
            var tills = new List<Till>();
            foreach (DataRow row in sTills.Rows)
            {
                var till = new Till
                {
                    Number = CommonUtility.GetShortValue(row["TILL_NUM"]),
                    Active = CommonUtility.GetBooleanValue(row["ACTIVE"]),
                    Processing = CommonUtility.GetBooleanValue(row["PROCESS"]),
                    Float = CommonUtility.GetDecimalValue(row["FLOAT"]),
                    BonusFloat = CommonUtility.GetDecimalValue(row["CashBonusFloat"]),
                    Cash = CommonUtility.GetDecimalValue(row["CASH"]),
                    Date_Open = CommonUtility.GetDateTimeValue(row["DATE_OPEN"]),
                    Time_Open = CommonUtility.GetDateTimeValue(row["TIME_OPEN"]),
                    ShiftDate = CommonUtility.GetDateTimeValue(row["ShiftDate"]),
                    Shift = CommonUtility.GetShortValue(row["ShiftNumber"]),
                    UserLoggedOn = CommonUtility.GetStringValue(row["UserLoggedOn"]),
                    POSId = CommonUtility.GetIntergerValue(row["POSID"]),
                    CashBonus = CommonUtility.GetDecimalValue(row["CashBonus"])
                };
                tills.Add(till);
            }
            _performancelog.Debug($"End,TillService,GetnotPayAtPumpTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return tills;
        }

        /// <summary>
        /// Get the active tills for a given user
        /// </summary>
        /// <param name="active">Active</param>
        /// <param name="payAtPumpTill">Pay at pump till</param>
        /// <param name="userLoggedOn">User logged on</param>
        /// <returns>Tills</returns>
        public List<Till> GetTillForUser(int active, int payAtPumpTill, string userLoggedOn)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillService,GetTillForUser,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var strSql = "Select * From Tills Where  Tills.Active = " + active + " AND Tills.Till_Num <> " + payAtPumpTill + " and Tills.UserLoggedON=\'" + userLoggedOn + "\'";
            var sTills = GetRecords(strSql, DataSource.CSCMaster);

            var tills = (from DataRow row in sTills.Rows
                         select new Till
                         {
                             Number = CommonUtility.GetShortValue(row["TILL_NUM"]),
                             Active = CommonUtility.GetBooleanValue(row["ACTIVE"]),
                             Processing = CommonUtility.GetBooleanValue(row["PROCESS"]),
                             Float = CommonUtility.GetDecimalValue(row["FLOAT"]),
                             BonusFloat = CommonUtility.GetDecimalValue(row["CashBonusFloat"]),
                             Cash = CommonUtility.GetDecimalValue(row["CASH"]),
                             Date_Open = CommonUtility.GetDateTimeValue(row["DATE_OPEN"]),
                             Time_Open = CommonUtility.GetDateTimeValue(row["TIME_OPEN"]),
                             ShiftDate = CommonUtility.GetDateTimeValue(row["ShiftDate"]),
                             Shift = CommonUtility.GetShortValue(row["ShiftNumber"]),
                             UserLoggedOn = CommonUtility.GetStringValue(row["UserLoggedOn"]),
                             POSId = CommonUtility.GetIntergerValue(row["POSID"]),
                             CashBonus = CommonUtility.GetDecimalValue(row["CashBonus"])
                         }).ToList();
            _performancelog.Debug($"End,TillService,GetTillForUser,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return tills;
        }

        /// <summary>
        /// Clear the non active Tills
        /// </summary>
        /// <param name="active">Active</param>
        /// <param name="tillNumber">Till number</param>
        public void ClearNonActiveTill(int active, string tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillService,ClearNonActiveTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var query = "select * from tills where active = " + active + "  and Till_Num <> " + tillNumber + " ";
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);

            foreach (DataRow row in _dataTable.Rows)
            {
                row["Float"] = 0;
                row["CashBonusFloat"] = 0;
                row["Cash"] = 0;

                row["Date_Open"] = DBNull.Value;

                row["Time_Open"] = DBNull.Value;

                row["ShiftDate"] = DBNull.Value;

                row["ShiftNumber"] = DBNull.Value;

                row["posID"] = DBNull.Value;

                row["UserLoggedOn"] = DBNull.Value;
            }
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.UpdateCommand = builder.GetUpdateCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            _performancelog.Debug($"End,TillService,ClearNonActiveTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Update the till
        /// </summary>
        /// <param name="till">Till</param>
        /// <returns>Till</returns>
        public Till UpdateTill(Till till)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillService,UpdateTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var query = $"select * from tills where TILL_NUM = {till.Number}";
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count > 0)
            {
                _dataTable.Rows[0]["Float"] = till.Float;
                _dataTable.Rows[0]["CashBonusFloat"] = till.CashBonus;
                _dataTable.Rows[0]["Cash"] = till.Cash;
                _dataTable.Rows[0]["Date_Open"] = till.Date_Open == DateTime.MinValue ? DateTime.Now : till.Date_Open;
                _dataTable.Rows[0]["Time_Open"] = till.Time_Open.Date == DateTime.MinValue.Date ? DateTime.Now : till.Time_Open;
                _dataTable.Rows[0]["ShiftDate"] = till.ShiftDate == DateTime.MinValue ? DateTime.Now : till.ShiftDate;
                _dataTable.Rows[0]["ShiftNumber"] = till.Shift;
                _dataTable.Rows[0]["posID"] = till.POSId;
                _dataTable.Rows[0]["UserLoggedOn"] = till.UserLoggedOn;
                _dataTable.Rows[0]["Active"] = till.Active;
                _dataTable.Rows[0]["PROCESS"] = till.Processing;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            _performancelog.Debug($"End,TillService,UpdateTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return till;
        }

        /// <summary>
        /// Get the Maximum shiftNumber from tills
        /// </summary>
        /// <param name="shiftDate">Shift date</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Maximum shift number</returns>
        public int GetMaximumShiftNumber(DateTime shiftDate, int? tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillService,GetMaximumShiftNumber,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            int maxShiftNumber = 0;
            StringBuilder whereClause = new StringBuilder();
            string query;
            if (shiftDate != DateTime.MinValue)
            {
                whereClause.Append($"AND shiftdate=\'{shiftDate:yyyy-MM-dd}' ");
            }

            if (tillNumber.HasValue)
            {
                whereClause.Append($"AND Till_Num = {tillNumber.Value} ");
            }

            if (whereClause.Length == 0)
                query = "Select max(shiftnumber)as NowShift from tills ";
            else
            {
                var condition = whereClause.ToString().Substring(4, whereClause.Length - 4);
                query = $"Select max(shiftnumber) as NowShift from tills where {condition}";
            }
            var sShift = GetRecords(query, DataSource.CSCMaster);

            if (!DBNull.Value.Equals(sShift.Rows[0]["NowShift"]))
            {
                return CommonUtility.GetIntergerValue(sShift.Rows[0]["NowShift"]);
            }
            _performancelog.Debug($"End,TillService,GetMaximumShiftNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return maxShiftNumber;
        }

        /// <summary>
        /// Method to get all tills
        /// </summary>
        /// <returns>List of till numbers</returns>
        public List<int> GetAllTills()
        {
            var tills = new List<int>();
            var payAtPumpTill = "100";
            var rs = GetRecords("SELECT Till_Num FROM Tills WHERE Till_Num<>" + payAtPumpTill, DataSource.CSCMaster);
            foreach (DataRow row in rs.Rows)
            {
                tills.Add(CommonUtility.GetIntergerValue(row["Till_Num"]));
            }
            return tills;
        }

        /// <summary>
        /// Method to update cash in till
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="amount">Amount</param>
        /// <returns>Updated cash</returns>
        public decimal UpdateCash(int tillNumber, decimal amount)
        {
            decimal updatedCash = 0;
            var query = "Select * From Tills Where Tills.Till_Num = " + tillNumber;
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count > 0)
            {
                updatedCash = CommonUtility.GetDecimalValue(_dataTable.Rows[0]["Cash"]) + amount;
                _dataTable.Rows[0]["Cash"] = updatedCash;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
            return updatedCash;
        }

        /// <summary>
        /// Method to inactivate process
        /// </summary>
        /// <param name="till">Till</param>
        public void No_Process(Till till)
        {
            var query = "Select * From Tills Where Tills.Till_Num = " + Convert.ToString(till.Number);
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count > 0)
            {
                _dataTable.Rows[0]["Process"] = 0;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
        }
    }
}

using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infonet.CStoreCommander.ADOData
{
    public class CashService : SqlDbService, ICashService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;


        /// <summary>
        /// Method to add cash draw
        /// </summary>
        /// <param name="cashDraw">Cash draw</param>
        public void AddCashDraw(CashDraw cashDraw)
        {
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from CashDraw where TILL=" + cashDraw.TillNumber, _connection);
            _adapter.Fill(_dataTable);
            var fields = _dataTable.NewRow();
            fields["Draw_Date"] = cashDraw.DrawDate;
            fields["User"] = cashDraw.User;
            fields["Till"] = cashDraw.TillNumber;
            fields["Amount"] = cashDraw.TotalValue;
            fields["Reason"] = cashDraw.Reason;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
        }

        /// <summary>
        /// Method to get all bills
        /// </summary>
        /// <returns></returns>
        public List<Cash> GetBills()
        {
            var dt = GetRecords("Select *  FROM   DrawTypes  WHERE  DrawTypes.Currency_Type = \'B\' Order By DrawTypes.Button_Number ", DataSource.CSCMaster);
            var bills = new List<Cash>();
            foreach (DataRow dr in dt.Rows)
            {
                bills.Add(new Cash
                {
                    CurrencyName = CommonUtility.GetStringValue(dr["Currency_Name"]),
                    Value = CommonUtility.GetDecimalValue(CommonUtility.GetDecimalValue(dr["Value"]).ToString("0.00")),
                    Image = CommonUtility.GetStringValue(dr["Image"])
                });

            }
            return bills;
        }

        /// <summary>
        /// Method to get list of cash buttons
        /// </summary>
        /// <returns>List of cash buttons</returns>
        public List<CashButton> GetCashButtons()
        {
            var cashButtons = new List<CashButton>();
            var dt = GetRecords("SELECT * FROM   CashButtons Order by CashButtons.Cash_Button ", DataSource.CSCMaster);
            foreach (DataRow dr in dt.Rows)
            {
                cashButtons.Add(new CashButton
                {
                    Value = CommonUtility.GetByteValue(dr["Cash_Value"])
                });
            }
            return cashButtons;
        }

        /// <summary>
        /// Method to get all coins
        /// </summary>
        /// <returns></returns>
        public List<Cash> GetCoins()
        {
            var dt = GetRecords("Select *  FROM   DrawTypes  WHERE  DrawTypes.Currency_Type = \'C\' Order By DrawTypes.Button_Number ", DataSource.CSCMaster);
            var coins = new List<Cash>();
            foreach (DataRow dr in dt.Rows)
            {
                coins.Add(new Cash
                {
                    CurrencyName = CommonUtility.GetStringValue(dr["Currency_Name"]),
                    Value = CommonUtility.GetDecimalValue(CommonUtility.GetDecimalValue(dr["Value"]).ToString("0.00")),
                    Image = CommonUtility.GetStringValue(dr["Image"])
                });
            }
            return coins;
        }

        
        /// <summary>
        /// Method to add  drop header
        /// </summary>
        /// <param name="dropHeader">Drop header</param>
        public void AddDropHeader(DropHeader dropHeader)
        {
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from DropHeader", _connection);
            _adapter.Fill(_dataTable);
            var fields = _dataTable.NewRow();

            fields["DropDate"] = dropHeader.DropDate;
            fields["User"] = dropHeader.UserCode;
            fields["Till_Num"] = dropHeader.TillNumber;
            fields["DropCount"] = dropHeader.DropCount;
            fields["shiftid"] = dropHeader.ShiftId;
            fields["ShiftDate"] = dropHeader.ShiftDate;
            if (!string.IsNullOrEmpty(dropHeader.EnvelopeNo))
            {
                fields["EnvelopeNo"] = dropHeader.EnvelopeNo;
            }
            fields["ReasonCode"] = dropHeader.ReasonCode;
            fields["DropID"] = dropHeader.DropId;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
        }

        /// <summary>
        /// Method to add drop line
        /// </summary>
        /// <param name="dropLine">Drop line</param>
        public void AddDropLine(DropLine dropLine)
        {
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from DropLines", _connection);
            _adapter.Fill(_dataTable);
            var fields = _dataTable.NewRow();
            fields["Till_Num"] = dropLine.TillNumber;
            fields["DropDate"] = dropLine.DropDate;
            fields["Tender_Name"] = dropLine.TenderName;
            fields["Exchange_Rate"] = dropLine.ExchangeRate;
            fields["Amount"] = dropLine.Amount;
            fields["Conv_Amount"] = dropLine.ConvAmount;
            fields["DropID"] = dropLine.DropID;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
        }

        /// <summary>
        /// Method to get maximum cash drop from drop header
        /// </summary>
        /// <returns>Cash drop</returns>
        public short GetMaxCashDrop(int tillNumber, DateTime shiftDate, int shiftNumber)
        {
            short cntDrop = 0;
            var rsdrop = GetRecords("select max(dropcount) as [maxCnt] from DropHeader where TILL_NUM = " + tillNumber + " and Shiftdate = \'" + shiftDate.ToString("yyyyMMdd") + "\'and shiftid = " + shiftNumber, DataSource.CSCTills);
            if (rsdrop.Rows.Count == 0)
            {
                cntDrop++;
            }
            else
            {
                var fields = rsdrop.Rows[0];
                cntDrop = Convert.ToInt16(DBNull.Value.Equals(fields["maxcnt"]) ? 1 : CommonUtility.GetIntergerValue(fields["maxcnt"]) + 1);
            }
            return cntDrop;
        }

        /// <summary>
        /// Method to get max drop id
        /// <param name="dataSource">Data Source</param>
        /// </summary>
        /// <returns></returns>
        public int GetMaxDropId(DataSource dataSource)
        {
            var dt = GetRecords("Select max(DropID) as DropID from DropHeader", dataSource);
            if (dt == null || dt.Rows.Count == 0)
            {
                return 0;
            }
            return DBNull.Value.Equals(dt.Rows[0]["DropID"]) ? 0 : CommonUtility.GetIntergerValue(dt.Rows[0]["DropID"]) + 1;
        }

        /// <summary>
        /// Method to save cash draw
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="reasonCode">Reason Code</param>
        public void SaveCashDraw(int tillNumber, string userCode, string reasonCode)
        {
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from OpenDrawer", _connection);
            _adapter.Fill(_dataTable);
            var fields = _dataTable.NewRow();
            fields["Till"] = tillNumber;
            fields["DateOpen"] = DateTime.Now;
            fields["UserCode"] = userCode;
            fields["Reason"] = reasonCode;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
        }
    }
}

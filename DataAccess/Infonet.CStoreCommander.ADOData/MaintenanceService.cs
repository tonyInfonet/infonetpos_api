using Infonet.CStoreCommander.Entities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infonet.CStoreCommander.ADOData
{
    public class MaintenanceService : SqlDbService, IMaintenanceService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        /// <summary>
        /// Method to get terminal Ids
        /// </summary>
        /// <param name="posId">POS Id</param>
        /// <returns></returns>
        public List<Terminal> GetTerminalIds(byte posId)
        {
            var terminals = new List<Terminal>();
            var rsTerm = GetRecords("SELECT * FROM TerminalIDs where ID=" + Convert.ToString(posId) + " and terminaltype <> \'Ezipin\'", DataSource.CSCAdmin);

            if (rsTerm.Rows.Count == 0)
            {
                return null;
            }
            foreach (DataRow row in rsTerm.Rows)
            {
                Terminal terminal = new Terminal();
                terminal.Id = CommonUtility.GetIntergerValue(row["ID"]);
                terminal.TerminalType = CommonUtility.GetStringValue(row["TerminalType"]);
                terminal.TerminalId = CommonUtility.GetStringValue(row["TerminalID"]);

                terminals.Add(terminal);
            }
            return terminals;
        }

        /// <summary>
        /// Method to set close batch number
        /// </summary>
        /// <param name="cc">Credit card</param>
        public void SetCloseBatchNumber(Credit_Card cc)
        {
            //Save to the CloseBatch table
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from CloseBatch", _connection);
            _adapter.Fill(_dataTable);

            var fields = _dataTable.NewRow();
            fields["BatchNumber"] = cc.Sequence_Number;
            fields["TerminalID"] = cc.TerminalID;
            fields["BatchDate"] = cc.Trans_Date;
            fields["BatchTime"] = cc.Trans_Time;
            fields["Report"] = cc.Report;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();

            //Update all the Tills and Trans for this TerminalID
            UpdateTables(cc, DataSource.CSCTills);
            UpdateTables(cc, DataSource.CSCTrans);
        }

        /// <summary>
        /// Method to update tables
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="dataSource">Data source</param>
        private void UpdateTables(Credit_Card creditCard, DataSource dataSource)
        {
            string strSql = "";
            if (Information.IsDBNull(creditCard.Trans_Date))
            {
                creditCard.Trans_Date = DateAndTime.Today;
            }
            strSql = "UPDATE  CardTenders " + " SET BatchNumber=\'" + creditCard.Sequence_Number + "\' ," + " BatchDate = \'" + creditCard.Trans_Date.ToString("yyyyMMdd") + "\' " + " Where CardTenders.CallTheBank = 1 AND " + " CardTenders.TerminalID = \'" + creditCard.TerminalID + "\'  AND " + " CardTenders.BatchNumber IS NULL AND CardTenders.Result = \'0\'";
            Execute(strSql, dataSource);
        }

    }
}

using System;
using System.Data.SqlClient;
using System.Data;

namespace Infonet.CStoreCommander.ADOData
{
    public class EncryptDecryptUtilityService : SqlDbService, IEncryptDecryptUtilityService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        /// <summary>
        /// Method to get password saved in database
        /// </summary>
        /// <returns></returns>
        public string GetPassword()
        {
            string strPassword;

            var dt = GetRecords("select * from Variables where Name=\'EncryptPassword\'", DataSource.CSCMaster);
            if (dt.Rows.Count > 0)
            {
                strPassword = !DBNull.Value.Equals(dt.Rows[0]["Value"]) ?
                    CommonUtility.GetStringValue(dt.Rows[0]["Value"]) : "hA5dphy7eKpd79IQWT1qeQ==";
            }
            else
            {
                var query = "select Max(VarIndex) as MaxIndex from Variables";
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter(query, _connection);
                _adapter.Fill(_dataTable);
                if (_dataTable.Rows.Count == 0)
                {
                    DataRow fields = _dataTable.NewRow();
                    fields["VarIndex"] = Convert.ToInt32(DBNull.Value.Equals(dt.Rows[0]["MaxIndex"])
                        ? 0 : dt.Rows[0]["MaxIndex"]) + 1;
                    fields["Name"] = "EncryptPassword";
                    
                    fields["Description"] = "To keep Encrypted Password.";
                    _dataTable.Rows.Add(fields);
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.InsertCommand = builder.GetInsertCommand();
                    _adapter.Update(_dataTable);
                }
                _connection.Close();
                _adapter?.Dispose();
                strPassword = "hA5dphy7eKpd79IQWT1qeQ=="; 
            }

            return strPassword;

        }

        /// <summary>
        /// Method to save password in database
        /// </summary>
        /// <param name="strPassword"></param>
        public void SavePassword(string strPassword)
        {
            string strEncryptPassword = "";

            var query = "select * from Variables where Name=\'EncryptPassword\'";
            bool addNew = false;
            DataRow fields;
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTills));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count == 0)
            {
                fields = _dataTable.NewRow();
                addNew = true;
                var rsTmp = GetRecords("select Max(VarIndex) as MaxIndex from Variables", DataSource.CSCMaster);
                fields["VarIndex"] = Convert.ToInt32(DBNull.Value.Equals(rsTmp.Rows[0]["MaxIndex"]) ? 0 : rsTmp.Rows[0]["MaxIndex"]) + 1;
                fields["Name"] = "EncryptPassword";
                fields["Description"] = "To keep Encrypted Password.";
            }
            else
            {
                fields = _dataTable.Rows[0];
            }
            fields["Value"] = strEncryptPassword;
            if (addNew)
            {
                _dataTable.Rows.Add(fields);
                var builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
            }
            else {
                var builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
            }
            _connection.Close();
            _adapter?.Dispose();
        }

        /// <summary>
        /// Method to initialise data
        /// </summary>
        /// <returns></returns>
        public bool ClassInitialize()
        {
            bool myEncryptEnabled;
            string query = "select * from Variables where Name=\'EncryptEnabled\'";
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
                if (!DBNull.Value.Equals(_dataTable.Rows[0]["Value"]))
                {
                    myEncryptEnabled = CommonUtility.GetIntergerValue(_dataTable.Rows[0]["Value"]) == 1;
                }
                else
                {
                    myEncryptEnabled = true;
                    _dataTable.Rows[0]["Value"] = "1";
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                    _adapter.Update(_dataTable);
                }
            }
            else
            {
                query = "select Max(VarIndex) as MaxIndex from Variables";
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter(query, _connection);
                _adapter.Fill(_dataTable);
                DataRow rsTmpfields = _dataTable.NewRow();
                rsTmpfields["VarIndex"] = Convert.ToInt32(DBNull.Value.Equals(rsTmpfields["MaxIndex"]) ? 0 : rsTmpfields["MaxIndex"]) + 1;
                rsTmpfields["Name"] = "EncryptEnabled";
                rsTmpfields["Value"] = "1";
                rsTmpfields["Description"] = "To Encrypt Card Number or not, 1-Yes/0-No.";
                _dataTable.Rows.Add(rsTmpfields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
                _adapter.Update(_dataTable);
                myEncryptEnabled = true;
            }
            _connection.Close();
            _adapter?.Dispose();
            return myEncryptEnabled;
        }
    }
}

using System;
using System.Data;
using System.Data.SqlClient;

namespace Infonet.CStoreCommander.ADOData
{
    public class AiteCardHolderService : SqlDbService, IAiteCardHolderService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        /// <summary>
        /// Method to affix bar code
        /// </summary>
        /// <param name="strBarcode">Bar code</param>
        /// <param name="cardNumber">Card number</param>
        /// <param name="validateMode">Validation mode</param>
        /// <returns>True or false</returns>
        public bool AffixBarcode(string strBarcode, string cardNumber, byte validateMode)
        {
            if (string.IsNullOrEmpty(cardNumber))
            {
                return false;
            }

            
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("Select * from TaxExemptCardRegistry where BarCode=\'" + strBarcode + "\' " + " OR AltBarCode=\'" + strBarcode + "\'", _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                return false;
            }

            //since allow affix bar code based on bar code
            if (validateMode == 3 | validateMode == 1)
            {
                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter("select * from TaxExemptCardRegistry where CardNumber=\'" + cardNumber + "\' ", _connection);
                _adapter.Fill(_dataTable);
                if (_dataTable.Rows.Count == 0)
                {
                    return false;
                }
                var rsFields = _dataTable.Rows[0];
                if (CommonUtility.GetStringValue(rsFields["Barcode"]) != strBarcode || DBNull.Value.Equals(rsFields["Barcode"]))
                {
                    rsFields["Barcode"] = strBarcode;
                    rsFields["Updated"] = 1; 
                    rsFields["UpdateTime"] = DateTime.Now;
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                    _adapter.Update(_dataTable);
                }
            }
            else
            {
                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter("select * from TaxExemptCardRegistry where AltCardNumber=\'" + cardNumber + "\' ", _connection);
                _adapter.Fill(_dataTable);
                if (_dataTable.Rows.Count == 0)
                {
                    return false;
                }
                var rsFields = _dataTable.Rows[0];
                if (CommonUtility.GetStringValue(rsFields["AltBarCode"]) != strBarcode)
                {
                    rsFields["AltBarCode"] = strBarcode;
                    rsFields["Updated"] = 1; 
                    rsFields["UpdateTime"] = DateTime.Now;
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                    _adapter.Update(_dataTable);
                }
            }
            _connection.Close();
            _adapter?.Dispose();
            return true;
        }
    }
}

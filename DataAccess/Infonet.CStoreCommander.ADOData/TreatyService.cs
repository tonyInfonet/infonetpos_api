using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.ADOData
{
    public class TreatyService : SqlDbService, ITreatyService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;


        /// <summary>
        /// Add or Update Treaty Name
        /// </summary>
        /// <param name="treatyNumber">Treaty number</param>
        /// <param name="treatyName">Treaty name</param>
        /// <returns>Treaty name</returns>
        public string AddorUpdateTreatyName(string treatyNumber, string treatyName)
        {
            if (!string.IsNullOrEmpty(treatyNumber))
            {
                return null;
            }
            var returnValue = treatyName;
            var query = "SELECT * FROM TreatyNo WHERE TreatyNo=\'" + treatyNumber + "\'";
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
                if (!DBNull.Value.Equals(_dataTable.Rows[0]["TreatyName"]))
                {
                    if (CommonUtility.GetStringValue(_dataTable.Rows[0]["TreatyName"]).Length <= 0) return returnValue;
                    var trtyName = CommonUtility.GetStringValue(_dataTable.Rows[0]["TreatyName"]);
                    if (treatyName.Trim().Length == 0)
                    {
                        returnValue = Convert.ToString(_dataTable.Rows[0]["TreatyName"]);
                    }

                    else if (!treatyName.Equals(trtyName))
                    {
                        _dataTable.Rows[0]["TreatyName"] = treatyName.Trim();
                        SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                        _adapter.UpdateCommand = builder.GetUpdateCommand();
                        _adapter.Update(_dataTable);
                        _connection.Close();
                        _adapter?.Dispose();
                    }
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Get Treaty Name
        /// </summary>
        /// <param name="treatyNumber">Treaty number</param>
        /// <returns></returns>
        public string GetTreatyName(string treatyNumber)
        {
            if (string.IsNullOrEmpty(treatyNumber)) return "";
            var returnValue = "";
            var rs = GetRecords("SELECT * FROM TreatyNo WHERE TreatyNo=\'" + treatyNumber.Trim() + "\'", DataSource.CSCMaster);
            if (rs.Rows.Count > 0)
            {
                returnValue = CommonUtility.GetStringValue(rs.Rows[0]["TreatyName"]);
            }
            return returnValue;
        }

        /// <summary>
        /// Get Cigarette Equivalent Units
        /// </summary>
        /// <param name="productType">Product type</param>
        /// <returns>Cigratte equivalent units</returns>
        public double GetCigaretteEquivalentUnits(string productType)
        {
            var sSql = "SELECT Value FROM Admin WHERE Name=" + "'" + productType + "'";

            var oRecs = GetRecords(sSql, DataSource.CSCAdmin);

            var returnValue = CommonUtility.GetDoubleValue(oRecs.Rows[0]["Value"]);
            return returnValue;
        }

        /// <summary>
        /// Get Quota
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="treatyNumber">Treaty number</param>
        /// <returns>Quota</returns>
        public double GetQuota(string fieldName, string treatyNumber)
        {
            var sSql = "SELECT " + fieldName + " As Quota " + " FROM TreatyNo WHERE TreatyNo=\'" + treatyNumber + "\'";

            var oRecs = GetRecords(sSql, DataSource.CSCMaster);

            var dOutQuota = oRecs.Rows.Count == 0 ? 0 : CommonUtility.GetDoubleValue(oRecs.Rows[0]["Quota"]);
            return dOutQuota;
        }

        /// <summary>
        /// Is Invalid Certificate
        /// </summary>
        /// <param name="treatyNumber">Treaty number</param>
        /// <returns>True or false</returns>
        public bool IsInvalidCertificate(string treatyNumber)
        {
            var rsInvalidCert = GetRecords("SELECT * FROM InvalidCertificates WHERE CertificateNumber=\'" + treatyNumber + "\'", DataSource.CSCMaster);
            return rsInvalidCert.Rows.Count > 0;
        }

        /// <summary>
        /// Method to update treaty number
        /// </summary>
        /// <param name="query">Query</param>
        public void UpdateTreatyNumber(string query)
        {
            Execute(query, DataSource.CSCMaster);
        }

        /// <summary>
        /// Method to get list of treaty numbers
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>List of treaty numbers</returns>
        public List<TreatyNo> GetTreatyNumbers(string query)
        {
            var oRecs = GetRecords(query, DataSource.CSCMaster);

            return (from DataRow treaty in oRecs.Rows
                    select new TreatyNo
                    {
                        TreatyNumber = CommonUtility.GetStringValue(treaty["TreatyNo"]),
                        AltTreatyNo = CommonUtility.GetShortValue(treaty["AltTreatyNo"]),
                        TobaccoQuota = CommonUtility.GetIntergerValue(treaty["TobaccoQuota"]),
                        GasQuota = CommonUtility.GetFloatValue(treaty["GasQuota"]),
                        TreatyName = CommonUtility.GetStringValue(treaty["TreatyName"])
                    }).ToList();
        }
    }
}

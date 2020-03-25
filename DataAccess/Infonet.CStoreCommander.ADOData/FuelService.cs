using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Infonet.CStoreCommander.ADOData
{
    public class FuelService : SqlDbService, IFuelService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        #region CSCMaster services

        /// <summary>
        /// Method to get discount rate of fuel
        /// </summary>
        /// <param name="groupId">Group id</param>
        /// <param name="gradeId">Grade id</param>
        /// <returns>DIscount rate</returns>
        public float? GetDiscountRate(string groupId, string gradeId)
        {
            var dt = GetRecords("SELECT DiscountRate FROM FuelDiscountChart  WHERE GroupID = \'" + groupId + "\' and Grade= " + Convert.ToString(gradeId), DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                return CommonUtility.GetFloatValue(dt.Rows[0]["DiscountRate"]);
            }
            return null;
        }

        /// <summary>
        /// Method to save post pay enabled
        /// </summary>
        /// <param name="vData">Data</param>
        public void SavePostPayEnabled(bool vData)
        {
            var query = "select * from Variables where Name=\'PostPayEnabled\'";
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
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _dataTable.Rows[0]["Value"] = vData ? "1" : "0";
            }
            else
            {
                var rsTmp = GetRecords("select Max(VarIndex) as MaxIndex from Variables", DataSource.CSCMaster);
                DataRow rsfields = _dataTable.NewRow();
                rsfields["VarIndex"] = DBNull.Value.Equals(rsTmp.Rows[0]["MaxIndex"]) ? 0 : CommonUtility.GetIntergerValue(rsTmp.Rows[0]["MaxIndex"]) + 1;
                rsfields["Name"] = "PostPayEnabled";
                rsfields["Value"] = vData ? "1" : "0";
                rsfields["Description"] = "To keep the current PostPay status, 1-On/0-Off.";
                _dataTable.Rows.Add(rsfields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
            }

            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
        }

        /// <summary>
        /// Method to save pay pump enabled
        /// </summary>
        /// <param name="vData">Data</param>
        public void SavePayPumpEnabled(bool vData)
        {
            string query = "select * from Variables where Name=\'PayPumpEnabled\'";
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
                _dataTable.Rows[0]["Value"] = vData ? "1" : "0";
            }
            else
            {
                query = "select Max(VarIndex) as MaxIndex from Variables";
                var rsTemp = GetRecords(query, DataSource.CSCMaster);
                DataRow rsfields = _dataTable.NewRow();
                rsfields["VarIndex"] = DBNull.Value.Equals(rsTemp.Rows[0]["MaxIndex"]) ? 0 : CommonUtility.GetIntergerValue(rsTemp.Rows[0]["MaxIndex"]) + 1;
                rsfields["Name"] = "PayPumpEnabled";
                rsfields["Value"] = vData ? "1" : "0";
                rsfields["Description"] = "To keep the current PayPump status, 1-On/0-Off.";
                _dataTable.Rows.Add(rsfields);
            }
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.UpdateCommand = builder.GetUpdateCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
        }

        /// <summary>
        /// Method to save prepay enabled
        /// </summary>
        /// <param name="vData">Data</param>
        public void SavePrepayEnabled(bool vData)
        {
            string query = "select * from Variables where Name=\'PrepayEnabled\'";
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
                _dataTable.Rows[0]["Value"] = vData ? "1" : "0";
            }
            else
            {
                var rsTmp = GetRecords("select Max(VarIndex) as MaxIndex from Variables", DataSource.CSCMaster);
                DataRow rsfields = _dataTable.NewRow();
                rsfields["VarIndex"] = DBNull.Value.Equals(rsTmp.Rows[0]["MaxIndex"]) ? 0 : CommonUtility.GetIntergerValue(rsTmp.Rows[0]["MaxIndex"]) + 1;
                rsfields["Name"] = "PrepayEnabled";
                rsfields["Value"] = vData ? "1" : "0";
                rsfields["Description"] = "To keep the current Prepay status, 1-On/0-Off.";
                _dataTable.Rows.Add(rsfields);
            }
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.UpdateCommand = builder.GetUpdateCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
        }

        /// <summary>
        /// Method to load service
        /// </summary>
        /// <returns></returns>
        public Service LoadService()
        {
            var service = new Service();
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from Variables where Name=\'PostPayEnabled\'", _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                if (!DBNull.Value.Equals(_dataTable.Rows[0]["Value"]))
                {
                    service.PostPayEnabled = CommonUtility.GetDoubleValue(_dataTable.Rows[0]["Value"]) == 1;
                }
                else
                {
                    service.PostPayEnabled = true;
                    _dataTable.Rows[0]["Value"] = "1";
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                }
            }
            else
            {
                var rsTmp = GetRecords("select Max(VarIndex) as MaxIndex from Variables", DataSource.CSCMaster);
                DataRow rsfields = _dataTable.NewRow();
                rsfields["VarIndex"] = DBNull.Value.Equals(rsTmp.Rows[0]["MaxIndex"]) ? 0 : CommonUtility.GetIntergerValue(rsTmp.Rows[0]["MaxIndex"]) + 1;
                rsfields["Name"] = "PostPayEnabled";
                rsfields["Value"] = "1";
                rsfields["Description"] = "To keep the current PostPay status, 1-On/0-Off.";
                _dataTable.Rows.Add(rsfields);
                service.PostPayEnabled = true;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
            }

            _adapter.Update(_dataTable);

            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from Variables where Name=\'PayPumpEnabled\'", _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                if (!DBNull.Value.Equals(_dataTable.Rows[0]["Value"]))
                {
                    service.PayPumpEnabled = CommonUtility.GetDoubleValue(_dataTable.Rows[0]["Value"]) == 1;
                }
                else
                {
                    service.PayPumpEnabled = true;
                    _dataTable.Rows[0]["Value"] = "1";
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                }

            }
            else
            {
                var rsTmp = GetRecords("select Max(VarIndex) as MaxIndex from Variables", DataSource.CSCMaster);
                DataRow rsfields = _dataTable.NewRow();
                rsfields["VarIndex"] = DBNull.Value.Equals(rsTmp.Rows[0]["MaxIndex"]) ? 0 : CommonUtility.GetIntergerValue(rsTmp.Rows[0]["MaxIndex"]) + 1;
                rsfields["Name"] = "PayPumpEnabled";
                rsfields["Value"] = "1";
                rsfields["Description"] = "To keep the current PayPump status, 1-On/0-Off.";
                _dataTable.Rows.Add(rsfields);
                service.PayPumpEnabled = true;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
            }
            _adapter.Update(_dataTable);
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("select * from Variables where Name=\'PrepayEnabled\'", _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                if (!DBNull.Value.Equals(_dataTable.Rows[0]["Value"]))
                {
                    service.PrepayEnabled = CommonUtility.GetDoubleValue(_dataTable.Rows[0]["Value"]) == 1;
                }
                else
                {
                    service.PrepayEnabled = true;
                    _dataTable.Rows[0]["Value"] = "1";
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                }
            }
            else
            {
                var rsTmp = GetRecords("select Max(VarIndex) as MaxIndex from Variables", DataSource.CSCMaster);
                DataRow rsfields = _dataTable.NewRow();
                rsfields["VarIndex"] = DBNull.Value.Equals(rsTmp.Rows[0]["MaxIndex"]) ? 0 : CommonUtility.GetIntergerValue(rsTmp.Rows[0]["MaxIndex"]) + 1;
                rsfields["Name"] = "PrepayEnabled";
                rsfields["Value"] = "1";
                rsfields["Description"] = "To keep the current Prepay status, 1-On/0-Off.";
                _dataTable.Rows.Add(rsfields);
                service.PrepayEnabled = true;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
            }
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            return service;
        }

        /// <summary>
        /// Method to save post pay set manually
        /// </summary>
        /// <param name="vData">Data</param>
        public void Save_PostPaySetManually(bool vData)
        {
            var query = "SELECT * FROM Variables WHERE Name=\'PostPayManually\'";
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
                _dataTable.Rows[0]["Value"] = vData ? "1" : "0";
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
            }
            else
            {
                var rsTmp = GetRecords("SELECT Max(VarIndex) AS MaxIndex FROM Variables", DataSource.CSCMaster);
                DataRow rsfields = _dataTable.NewRow();
                rsfields["VarIndex"] = DBNull.Value.Equals(rsTmp.Rows[0]["MaxIndex"]) ? 0 : CommonUtility.GetIntergerValue(rsTmp.Rows[0]["MaxIndex"]) + 1;
                rsfields["Name"] = "PostPayManually";
                rsfields["Value"] = vData ? "1" : "0";
                rsfields["Description"] = "PostPay status manually set from POS, 1-Yes/0-No";
                _dataTable.Rows.Add(rsfields);
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.InsertCommand = builder.GetInsertCommand();
            }

            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
        }

        #endregion

        #region CSCPump services

        /// <summary>
        /// Method to get fuelType
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Fuel type</returns>
        public string GetFuelTypeFromDbPump(string stockCode)
        {
            if (string.IsNullOrEmpty(stockCode))
            {
                return null;
            }
            string fuelType = null;
            var query = "select FuelType from Grade where UPPER(Stock_Code)= \'" + stockCode + "\'";
            var rdr = GetRecords(query, DataSource.CSCPump);
            if (rdr != null && rdr.Rows.Count > 0)
            {
                fuelType = CommonUtility.GetStringValue(rdr.Rows[0]["FuelType"]);
            }
            return fuelType;
        }
        #endregion

        #region CSCAdmin

        /// <summary>
        /// Method to load fuel department
        /// </summary>
        /// <returns></returns>
        public string LoadFuelDept()
        {
            //Modified by Tony 05/21/2019
            string fuelDept;
            var rsFuel = GetRecords("Select top 1 Dept From FuelDept ", DataSource.CSCMaster);
            if (rsFuel != null && rsFuel.Rows.Count > 0)
            {
                fuelDept = CommonUtility.GetStringValue(rsFuel.Rows[0]["Dept"]);
                if (string.IsNullOrEmpty(fuelDept))
                    fuelDept = "1";
            }
            else
            {
                fuelDept = "1";
            }
            return fuelDept;
            //string fuelDept;
            //var rsFuel = GetRecords("Select * From FuelDept ", DataSource.CSCAdmin);
            //if (rsFuel != null && rsFuel.Rows.Count > 0)
            //{
            //    fuelDept = CommonUtility.GetStringValue(rsFuel.Rows[0]["Dept"]);
            //    if (!string.IsNullOrEmpty(fuelDept))
            //    {
            //        var query = "Select  Dept as [Code], Dept_Name as [Name], EOD_Detail as [Detail]  FROM  Dept WHERE Dept = \'" + fuelDept + "\'";
            //        _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            //        if (_connection.State == ConnectionState.Closed)
            //        {
            //            _connection.Open();
            //        }
            //        _dataTable = new DataTable();
            //        _adapter = new SqlDataAdapter(query, _connection);
            //        _adapter.Fill(_dataTable);
            //        if (_dataTable.Rows.Count == 0)
            //        {
            //            DataRow mainFields = _dataTable.NewRow();
            //            mainFields["Code"] = fuelDept;
            //            mainFields["Name"] = fuelDept;
            //            mainFields["Detail"] = 1;
            //            _dataTable.Rows.Add(mainFields);
            //        }
            //        SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            //        _adapter.InsertCommand = builder.GetInsertCommand();
            //        _adapter.Update(_dataTable);
            //    }
            //}
            //else
            //{
            //    fuelDept = "";
            //}
            //return fuelDept;
        }

        /// <summary>
        /// Method to load fuel sale grocery coupon
        /// </summary>
        /// <returns>FSGD coupon</returns>
        public string LoadFsgdCoupon()
        {
            string fsgdCouponStr;
            var dt = GetRecords("SELECT ID, Sale_Foot, Ref_Foot FROM Receipt WHERE ID in (\'3\')", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                fsgdCouponStr = CommonUtility.GetStringValue(dt.Rows[0]["Sale_Foot"]);
            }
            else
            {
                fsgdCouponStr = "";
            }

            return fsgdCouponStr;
        }

        #endregion

        #region CSCReader

        /// <summary>
        /// Method to get header line
        /// </summary>
        /// <param name="num">Line number</param>
        /// <returns>Header line</returns>
        public string GetHeaderLine(short num)
        {
            StringBuilder header = new StringBuilder();
            const short headerNumber = 51;
            var rs = GetRecords("SELECT * FROM Message WHERE ReferenceNumber=" + Convert.ToString(headerNumber + num), DataSource.CSCReader);
            foreach (DataRow dr in rs.Rows)
            {
                header.Append(CommonUtility.GetStringValue(dr["line"]));
                header.Append("\r\n");
            }
            return header.ToString();
        }

        /// <summary>
        /// Method to get footer line
        /// </summary>
        /// <param name="num">Line num</param>
        /// <returns>Footer line</returns>
        public string GetFooterLine(short num)
        {
            StringBuilder footer = new StringBuilder();
            const short footerNumber = 55;
            var rs = GetRecords("SELECT * FROM Message WHERE ReferenceNumber=" + Convert.ToString(footerNumber + num), DataSource.CSCReader);
            foreach (DataRow dr in rs.Rows)
            {
                footer.Append(CommonUtility.GetStringValue(dr["line"]));
                footer.Append("\r\n");
            }
            return footer.ToString();
        }

        /// <summary>
        /// Method to check if this is existing coupon
        /// </summary>
        /// <param name="couponId">Coupon</param>
        /// <returns>True or false</returns>
        public bool IsExistingCoupon(string couponId)
        {
            var dt = GetRecords("select * from Coupon where CouponID=\'" + couponId + "\'", DataSource.CSCMaster);
            return dt == null || dt.Rows.Count == 0;
        }

        /// <summary>
        /// Method to get list of prices
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>List of prices</returns>
        public List<PriceL> GetPriceL(string stockCode)
        {
            var rs = GetRecords("SELECT   *  FROM     Pricel  WHERE    Pricel.Stock_code = \'" + stockCode + "\' ORDER BY Pricel.Pr_F_Qty ", DataSource.CSCMaster);

            return (from DataRow dr in rs.Rows
                    select new PriceL
                    {
                        FQuantity = CommonUtility.GetFloatValue(dr["Pr_F_Qty"]),
                        TQuantity = CommonUtility.GetFloatValue(dr["Pr_T_Qty"]),
                        Price = CommonUtility.GetFloatValue(dr["Price"]),
                        StartDate = CommonUtility.GetDateTimeValue(dr["StartDate"]),
                        EndDate = CommonUtility.GetDateTimeValue(dr["EndDate"])
                    }).ToList();
        }

        #endregion

        /// <summary>
        /// Get PumpSpace Value
        /// </summary>
        /// <returns></returns>
        public byte GetPumpSpace()
        {
            byte result = 1;
            var cmdtxt = "SELECT * FROM Setup";
            var pumpSetup = GetRecords(cmdtxt, DataSource.CSCPump);

            if (pumpSetup.Rows.Count > 0)
            {
                result = CommonUtility.GetByteValue(pumpSetup.Rows[0]["Pump_Space"]);
            }
            return result;
        }

        /// <summary>
        /// Gets the delay in seconds needed between 2 pumps operations
        /// </summary>
        /// <returns>Delay in seconds</returns>
        public int GetClickDelayForPumps()
        {
            int result = 0;
            var cmdtxt = "SELECT * FROM Setup";
            var pumpSetup = GetRecords(cmdtxt, DataSource.CSCPump);

            if (pumpSetup.Rows.Count > 0)
            {
                result = CommonUtility.GetByteValue(pumpSetup.Rows[0]["ClickDelay"]);
            }
            return result;
        }
    }
}






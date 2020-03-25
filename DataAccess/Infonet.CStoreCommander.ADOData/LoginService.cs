using System;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using System.Data.SqlClient;
using System.Data;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// Login Service
    /// </summary>
    public class LoginService : SqlDbService, ILoginService
    {

        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        #region CSCAdmin services

        /// <summary>
        /// Method to load Security information 
        /// </summary>
        /// <returns>Security</returns>
        public Security LoadSecurityInfo()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,LoginService,LoadSecurityInfo,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var security = new Security();
            var rs = GetRecords("Select Install_Date,Security_Key,POS_BO_Features,Pump_Features,NIC_Number,Num_OF_POS,MaxConcurrentPOS from Setup_C", DataSource.CSCAdmin);
            if (rs == null || rs.Rows.Count == 0) return security;
            var fields = rs.Rows[0];
            security.Install_Date_Encrypt = CommonUtility.GetStringValue(fields["Install_Date"]);
            security.Security_Key = CommonUtility.GetStringValue(fields["Security_Key"]);
            security.POS_BO_Features = CommonUtility.GetStringValue(fields["POS_BO_Features"]);
            security.Pump_Features = Convert.ToString(DBNull.Value.Equals(fields["Pump_Features"]) ? new string('0', 32) : fields["Pump_Features"]);
            security.NIC_Number = CommonUtility.GetStringValue(fields["NIC_Number"]);
            security.Number_OF_POS = CommonUtility.GetByteValue(DBNull.Value.Equals(fields["Num_OF_POS"]) ? 0 : fields["Num_OF_POS"]);
            security.MaxConcurrentPOS = CommonUtility.GetByteValue(DBNull.Value.Equals(fields["MaxConcurrentPOS"]) ? 0 : fields["MaxConcurrentPOS"]);
            security.ExpireDate = DateTime.FromOADate(security.Install_Date.ToOADate() + 5);

            _performancelog.Debug($"End,LoginService,LoadSecurityInfo,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return security;
        }

        /// <summary>
        /// Load Store information 
        /// </summary>
        /// <returns></returns>
        public Store LoadStoreInfo()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,LoginService,LoadStoreInfo,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var store = new Store();
            var rs = GetRecords("Select * from Setup_C", DataSource.CSCAdmin);
            if (rs != null && rs.Rows.Count > 0)
            {
                var fields = rs.Rows[0];
                store.Name = CommonUtility.GetStringValue(fields["Company_Name"]);
                store.RegName = CommonUtility.GetStringValue(fields["Reg_Name"]);
                store.RegNum = CommonUtility.GetStringValue(fields["Reg_Num"]);
                store.SecRegName = CommonUtility.GetStringValue(fields["Sec_Reg_Name"]);
                store.SecRegNum = CommonUtility.GetStringValue(fields["Sec_Reg_Num"]);
                store.Language = CommonUtility.GetStringValue(fields["Language"]);
                // Store.Code is used in till close to set Store field in SaleHead table
                // With a null value in Store field in SaleHead after the till was closed, DTS package that transfers data from Bo to Ho will fail
                // BackOffice has this property set, so for tills closed from BO there is no problem
                store.Code = CommonUtility.GetStringValue(fields["Store"]);
                store.Address.Street1 = CommonUtility.GetStringValue(fields["Address_1"]);
                store.Address.Street2 = CommonUtility.GetStringValue(fields["Address_2"]);
                store.Address.City = CommonUtility.GetStringValue(fields["City"]);
                store.Address.ProvState = CommonUtility.GetStringValue(fields["Province"]);
                store.Address.Country = CommonUtility.GetStringValue(fields["Country"]);
                store.Address.PostalCode = CommonUtility.GetStringValue(fields["Postal_Code"]);
                store.Address.Phones.Add("Tel", "Phone", "", CommonUtility.GetStringValue(fields["Tel_Num"]), "");
                store.Address.Phones.Add("Fax", "Fax", "", CommonUtility.GetStringValue(fields["Fax_Num"]), ""); //"Fax","Fax"              
            }

            if (!string.IsNullOrEmpty(store.Language))
            {
                rs = GetRecords("Select * From Languages  WHERE Languages.Language = \'" + store.Language + "\'", DataSource.CSCMaster);
                if (rs.Rows.Count != 0)
                {
                    store.OffSet = CommonUtility.GetShortValue(rs.Rows[0]["OffSet"]);
                }
                else
                {
                    store.OffSet = 0;
                }
            }
            else
            {
                store.OffSet = 0;
            }

            rs = GetRecords("SELECT * FROM Receipt where ID=\'1\'", DataSource.CSCMaster);
            if (rs != null && rs.Rows.Count > 0)
            {
                var fields = rs.Rows[0];

                store.Sale_Footer = CommonUtility.GetStringValue(fields["Sale_Foot"]);
                store.Refund_Footer = CommonUtility.GetStringValue(fields["Ref_Foot"]);
            }
            rs = GetRecords("SELECT * FROM Receipt where ID=\'2\'", DataSource.CSCMaster);
            if (rs == null || rs.Rows.Count == 0) return store;
            {
                var fields = rs.Rows[0];

                store.TaxExempt_Footer = CommonUtility.GetStringValue(fields["Sale_Foot"]);
            }
            _performancelog.Debug($"End,LoginService,LoadStoreInfo,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return store;
        }


        /// <summary>
        /// Get the IP address of the POS
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public short GetPosId(string ipAddress)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,LoginService,GetPosId,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            short posId = 0;
            var rsChapsMain = GetRecords("Select ID From POS_IP_Address where IP_Address= \'" + ipAddress + "\'", DataSource.CSCAdmin);
            if (rsChapsMain != null && rsChapsMain.Rows.Count > 0)
            {
                var fields = rsChapsMain.Rows[0];
                posId = CommonUtility.GetShortValue(fields["ID"]);
            }
            _performancelog.Debug(
                $"End,LoginService,GetPosId,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return posId;
        }


        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="modelUserName">User name</param>
        /// <param name="modelPassword">Password</param>
        /// <returns></returns>
        public bool ChangePassword(string modelUserName, string modelPassword)
        {
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCAdmin));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * FROM [User]  WHERE [User].U_Code = \'" + modelUserName.Trim() + "\'", _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                _dataTable.Rows[0]["epw"] = modelPassword;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
                return true;
            }
            return false;
        }


        /// <summary>
        /// Check Logged in user of POS
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="posId">Pos id</param>
        /// <returns>True or false</returns>
        public bool CheckLoggedinUserPos(string userName, int posId)
        {
            var rsUser = GetRecords("select Till_num, POSID from tills where userloggedon=\'" + userName + "\' and Process=1", DataSource.CSCMaster);
            if (rsUser != null && rsUser.Rows.Count > 0)
            {
                if (CommonUtility.GetIntergerValue(rsUser.Rows[0]["posID"]) != posId)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Updates the Logged in User
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>True or false</returns>
        public bool UpdateLoggedInUser(string userName, int tillNumber)
        {
            Execute("update tills set userloggedon=\'" + userName + "\' where till_num=" + tillNumber, DataSource.CSCMaster);
            return true;
        }

        #endregion
    }
}

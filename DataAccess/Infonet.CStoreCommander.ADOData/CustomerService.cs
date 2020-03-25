                     using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// Customer Service
    /// </summary>
    public class CustomerService : SqlDbService, ICustomerService
    {
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        /// <summary>
        /// Get Customers
        /// </summary>
        /// <param name="showCardCustomers"></param>
        /// <param name="additionalCriteria"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Customer> GetCustomers(bool showCardCustomers, string additionalCriteria, int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,GetCustomers,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            string sqlStmt;

            if (showCardCustomers)
            {
                sqlStmt = "SELECT CLIENT.CL_CODE AS [Code], " +
                            "       CLIENT.CL_NAME AS [Name], " +
                            "       CLIENT.CL_PHONE AS [Phone] " +
                            "FROM   CLIENT " +
                            " WHERE (CL_STATUS  is null  or cl_status = \'A\' or CL_STATUS = \'F\') " +
                            additionalCriteria +
                            " ORDER BY CLIENT.CL_CODE";
            }
            else
            {
                sqlStmt = "SELECT CLIENT.CL_CODE AS [Code]," +
                            " CLIENT.CL_NAME AS [Name]," +
                            " CLIENT.CL_PHONE AS [Phone]" +
                            " FROM   CLIENT  left join clientcard on client.cl_code = clientcard.cl_code" +
                            " WHERE (CL_STATUS  is null  or cl_status = \'A\' or CL_STATUS = \'F\')  " +
                            additionalCriteria +
                            "  AND CLIENTCARD.CL_CODE IS NULL ORDER BY CLIENT.CL_CODE";
            }

            var dt = GetPagedRecords(sqlStmt, DataSource.CSCMaster, pageIndex, pageSize);
            var customers = new List<Customer>();
            if (dt != null)
            {
                customers.AddRange(from DataRow fields in dt.Rows
                                   select new Customer
                                   {
                                       Name = CommonUtility.GetStringValue(fields["Name"]),
                                       Code = CommonUtility.GetStringValue(fields["Code"]),
                                       Phone = CommonUtility.GetStringValue(fields["Phone"])
                                   });
            }
            _performancelog.Debug($"End,CustomerService,GetCustomers,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return customers;
        }

        /// <summary>
        /// Get AR Customers
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Customer> GetArCustomers(int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,GetArCustomers,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            string sqlStmt = "SELECT CL_Code AS Code, CL_Name AS Name, CL_Phone AS Phone,CL_CURBAL,CL_LIMIT FROM Client WHERE CL_ARCust = 1 ORDER BY Cl_Code ";


            var dt = GetPagedRecords(sqlStmt, DataSource.CSCMaster, pageIndex, pageSize);
            var customers = new List<Customer>();
            if (dt != null)
            {
                customers.AddRange(from DataRow fields in dt.Rows
                                   select new Customer
                                   {
                                       Name = CommonUtility.GetStringValue(fields["Name"]),
                                       Code = CommonUtility.GetStringValue(fields["Code"]),
                                       Phone = CommonUtility.GetStringValue(fields["Phone"]),
                                       Current_Balance = CommonUtility.GetDoubleValue(fields["CL_CURBAL"]),
                                       Credit_Limit = CommonUtility.GetDoubleValue(fields["CL_LIMIT"])
                                   });
            }
            _performancelog.Debug($"End,CustomerService,GetArCustomers,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return customers;
        }



        /// <summary>
        /// Get Customer By Client Code
        /// </summary>
        /// <param name="clientCode">code</param>
        /// <returns>Customer</returns>
        public Customer GetClientByClientCode(string clientCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,GetClientByClientCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            Customer customer = null;

            var dt = GetRecords(" SELECT * FROM   Client WHERE Client.CL_Code = \'" + clientCode + "\'", DataSource.CSCMaster);

            if (dt != null && dt.Rows.Count > 0)
            {
                _performancelog.Debug($"End,CustomerService,GetClientByClientCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                customer = new Customer
                {
                    Code = CommonUtility.GetStringValue(dt.Rows[0]["CL_CODE"]),
                    Name = CommonUtility.GetStringValue(dt.Rows[0]["CL_NAME"]),
                    Category = CommonUtility.GetStringValue(dt.Rows[0]["CL_CATEG"]),
                    Address_1 = CommonUtility.GetStringValue(dt.Rows[0]["CL_ADD1"]),
                    Address_2 = CommonUtility.GetStringValue(dt.Rows[0]["CL_ADD2"]),
                    City = CommonUtility.GetStringValue(dt.Rows[0]["CL_CITY"]),
                    Province = CommonUtility.GetStringValue(dt.Rows[0]["CL_PROV"]),
                    Country = CommonUtility.GetStringValue(dt.Rows[0]["CL_COUNTRY"]),
                    Postal_Code = CommonUtility.GetStringValue(dt.Rows[0]["CL_POSTAL"]),
                    Area_Code = CommonUtility.GetStringValue(dt.Rows[0]["CL_ACODE"]),
                    Phone = CommonUtility.GetStringValue(dt.Rows[0]["CL_PHONE"]),
                    Cell_Phone = CommonUtility.GetStringValue(dt.Rows[0]["CL_CELL"]),
                    Fax = CommonUtility.GetStringValue(dt.Rows[0]["CL_FAX"]),
                    Toll_Free = CommonUtility.GetStringValue(dt.Rows[0]["CL_TOLL"]),
                    E_Mail = CommonUtility.GetStringValue(dt.Rows[0]["CL_EMAIL"]),
                    Contact_1 = CommonUtility.GetStringValue(dt.Rows[0]["CL_CONT1"]),
                    Contact_2 = CommonUtility.GetStringValue(dt.Rows[0]["CL_CONT2"]),
                    Loyalty_Code = CommonUtility.GetStringValue(dt.Rows[0]["LO_NUM"]),
                    Loyalty_Points = CommonUtility.GetDoubleValue(dt.Rows[0]["LO_POINTS"]),
                    AR_Customer = CommonUtility.GetBooleanValue(dt.Rows[0]["CL_ARCUST"]),
                    Customer_Type = CommonUtility.GetStringValue(dt.Rows[0]["CL_TYPE"]),
                    Terms = CommonUtility.GetShortValue(dt.Rows[0]["TERMS"]),
                    Credit_Limit = CommonUtility.GetDoubleValue(dt.Rows[0]["CL_LIMIT"]),
                    Current_Balance = CommonUtility.GetDoubleValue(dt.Rows[0]["CL_CURBAL"]),
                    CL_Status = CommonUtility.GetStringValue(dt.Rows[0]["CL_STATUS"]),
                    UsePO = CommonUtility.GetBooleanValue(dt.Rows[0]["UsePO"]),
                    GroupID = CommonUtility.GetStringValue(dt.Rows[0]["GroupID"]),
                    UseFuelRebate = CommonUtility.GetBooleanValue(dt.Rows[0]["UseFuelRebate"]),
                    UseFuelRebateDiscount = CommonUtility.GetBooleanValue(dt.Rows[0]["UseFuelRebateDiscount"]),
                    CL_Note = CommonUtility.GetStringValue(dt.Rows[0]["CL_Note"]),
                    TaxExempt = CommonUtility.GetBooleanValue(dt.Rows[0]["TaxExempt"]),
                    PlateNumber = CommonUtility.GetStringValue(dt.Rows[0]["PlateNumber"]),
                    TECardNumber = CommonUtility.GetStringValue(dt.Rows[0]["TECardNumber"]),
                    Discount_Code = CommonUtility.GetByteValue(dt.Rows[0]["CUST_DISC"]),
                    Price_Code = CommonUtility.GetByteValue(dt.Rows[0]["Price_Code"]),
                    MultiUse_PO = CommonUtility.GetBooleanValue(dt.Rows[0]["UsePO"]) && CommonUtility.GetBooleanValue(dt.Rows[0]["MultiUse_PO"]),
                };

                if (!string.IsNullOrEmpty(customer.GroupID))
                {
                    var clientDt = GetRecords("select * from ClientGroup  where GroupID=\'" + customer.GroupID + "\'", DataSource.CSCMaster);
                    if (clientDt == null || clientDt.Rows.Count == 0)
                    {
                        
                        
                        customer.GroupID = "";
                    }
                    else
                    {
                        customer.GroupName = CommonUtility.GetStringValue(clientDt.Rows[0]["GroupName"]);
                        customer.DiscountType = CommonUtility.GetStringValue(clientDt.Rows[0]["DiscountType"]);
                        customer.DiscountRate = CommonUtility.GetFloatValue(clientDt.Rows[0]["DiscountRate"]);
                        customer.Footer = CommonUtility.GetStringValue(clientDt.Rows[0]["LoyaltyFooter"]);
                        customer.DiscountName = CommonUtility.GetStringValue(clientDt.Rows[0]["DiscountName"]);
                    }
                }
               // var clientCard = GetClientCardForCustomer()
            }
            _performancelog.Debug($"End,CustomerService,GetClientByClientCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return customer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        public ClientCard GetClientCardByCardNumber(string cardNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,GetClientCardByCardNumber,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var dt = GetRecords("select * from ClientCard where CardNum=\'" + cardNumber.Trim() + "\'", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                _performancelog.Debug($"End,CustomerService,GetCustomers,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return new ClientCard
                {
                    CardNumber = CommonUtility.GetStringValue(dt.Rows[0]["CardNum"]),
                    ClientCode = CommonUtility.GetStringValue(dt.Rows[0]["CL_Code"]),
                    CardName = CommonUtility.GetStringValue(dt.Rows[0]["CardName"]),
                    ExpirationDate = CommonUtility.GetDateTimeValue(dt.Rows[0]["ExpDate"]),
                    Pin = CommonUtility.GetStringValue(dt.Rows[0]["PIN"]),
                    CardStatus = Convert.ToChar(dt.Rows[0]["CardStatus"]),
                    CreditLimiit = Convert.ToDecimal(dt.Rows[0]["CreditLimiit"]),
                    Balance = Convert.ToDecimal(dt.Rows[0]["Balance"]),
                    AllowRedemption = CommonUtility.GetBooleanValue(dt.Rows[0]["AllowRedemption"]),
                    TaxExemptedCardNumber = CommonUtility.GetStringValue(dt.Rows[0]["TECardNumber"]),
                    ProfileID = CommonUtility.GetStringValue(dt.Rows[0]["ProfileID"])
                };
            }
            _performancelog.Debug($"End,CustomerService,GetClientCardByCardNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return null;
        }


        /// <summary>
        /// Checks Whether AR customer Exists
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsArCustomer(string code)
        {
            var dt = GetRecords("SELECT CL_Code FROM Client WHERE CL_ArCust = 1 AND CL_Code=\'" + code.Trim() + "\'", DataSource.CSCMaster);

            return dt != null && dt.Rows.Count != 0;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        public ClientCard GetClientCardForCustomer(string cardNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,GetClientCardByCardNumber,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var dt = GetRecords("SELECT * FROM ClientCard inner join client on clientcard.cl_code = client.cl_code  WHERE CardNum = \'" + cardNumber.Trim() + "\'", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                _performancelog.Debug($"End,CustomerService,GetCustomers,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return new ClientCard
                {
                    ClientCode = CommonUtility.GetStringValue(dt.Rows[0]["CL_Code"]),
                    CardStatus = Convert.ToChar(dt.Rows[0]["CardStatus"]),
                    ProfileID = CommonUtility.GetStringValue(dt.Rows[0]["ProfileID"]),
                    ClientArCustomer = CommonUtility.GetBooleanValue(dt.Rows[0]["CL_arcust"]),
                    CardNumber = cardNumber
                };
            }
            _performancelog.Debug($"End,CustomerService,GetClientCardByCardNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ClientCard GetClientCardForGasKingCustomer(string code)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,GetClientCardForGasKingCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (code != null)
            {
                //var dt = GetRecords("SELECT * FROM ClientCard inner join client on clientcard.cl_code = client.cl_code  WHERE CardNum = \'" + cardNumber.Trim() + "\'", DataSource.CSCMaster);
                var dt = GetRecords("SELECT * FROM ClientCard  WHERE CardNum = \'" + code.Trim() + "\'", DataSource.CSCMaster);
                if (dt != null && dt.Rows.Count > 0)
                {
                    _performancelog.Debug($"End,CustomerService,GetCustomers,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                    return new ClientCard
                    {
                        CardNumber = CommonUtility.GetStringValue(dt.Rows[0]["CardNum"]),
                        //CardStatus = Convert.ToChar(dt.Rows[0]["CardStatus"]),
                        //ProfileID = CommonUtility.GetStringValue(dt.Rows[0]["ProfileID"]),
                        //ClientArCustomer = CommonUtility.GetBooleanValue(dt.Rows[0]["CL_arcust"]),
                        AllowRedemption = CommonUtility.GetBooleanValue(dt.Rows[0]["AllowRedemption"])
                        //AllowRedemption   Comm

                    };
                }
            }
            _performancelog.Debug($"End,CustomerService,GetClientCardByCardNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return null;
        }

        /// <summary>
        /// Search Customers
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="totalrecords"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Customer> Search(string searchCriteria, out int totalrecords, int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,Search,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            totalrecords = 0;
            var customers = new List<Customer>();
            var query = "Select * from Client " +
                                              " WHERE  (Client.CL_Status IS NULL " +
                                              " OR Client.CL_Status IN (\'A\',\'F\',\'\')) " +
                                              " AND (CL_NAME like \'%" + searchCriteria + "%\' OR " +
                                              " CL_CODE like \'%" + searchCriteria + "%\' OR " +
                                              " CL_PHONE like \'%" + searchCriteria + "%\' )" +
                                              "ORDER BY CLIENT.CL_CODE, CLIENT.CL_NAME, CLIENT.CL_PHONE";
            var dt = GetPagedRecords(query, DataSource.CSCMaster, pageIndex, pageSize);
            if (dt != null)
            {
                totalrecords = dt.Rows.Count;

                customers.AddRange(from DataRow fields in dt.Rows
                                   select new Customer
                                   {
                                       Code = CommonUtility.GetStringValue(fields["CL_CODE"]),
                                       Name = CommonUtility.GetStringValue(fields["CL_NAME"]),
                                       Category = CommonUtility.GetStringValue(fields["CL_CATEG"]),
                                       Address_1 = CommonUtility.GetStringValue(fields["CL_ADD1"]),
                                       Address_2 = CommonUtility.GetStringValue(fields["CL_ADD2"]),
                                       City = CommonUtility.GetStringValue(fields["CL_CITY"]),
                                       Province = CommonUtility.GetStringValue(fields["CL_PROV"]),
                                       Country = CommonUtility.GetStringValue(fields["CL_COUNTRY"]),
                                       Postal_Code = CommonUtility.GetStringValue(fields["CL_POSTAL"]),
                                       Area_Code = CommonUtility.GetStringValue(fields["CL_ACODE"]),
                                       Phone = CommonUtility.GetStringValue(fields["CL_PHONE"]),
                                       Cell_Phone = CommonUtility.GetStringValue(fields["CL_CELL"]),
                                       Fax = CommonUtility.GetStringValue(fields["CL_FAX"]),
                                       Toll_Free = CommonUtility.GetStringValue(fields["CL_TOLL"]),
                                       E_Mail = CommonUtility.GetStringValue(fields["CL_EMAIL"]),
                                       Contact_1 = CommonUtility.GetStringValue(fields["CL_CONT1"]),
                                       Contact_2 = CommonUtility.GetStringValue(fields["CL_CONT2"]),
                                       Loyalty_Code = CommonUtility.GetStringValue(fields["LO_NUM"]),
                                       Loyalty_Points = CommonUtility.GetDoubleValue(fields["LO_POINTS"]),
                                       AR_Customer = CommonUtility.GetBooleanValue(fields["CL_ARCUST"]),
                                       Customer_Type = CommonUtility.GetStringValue(fields["CL_TYPE"]),
                                       Terms = CommonUtility.GetShortValue(fields["TERMS"]),
                                       Credit_Limit = CommonUtility.GetDoubleValue(fields["CL_LIMIT"]),
                                       Current_Balance = CommonUtility.GetDoubleValue(fields["CL_CURBAL"]),
                                       CL_Status = CommonUtility.GetStringValue(fields["CL_STATUS"]),
                                       UsePO = CommonUtility.GetBooleanValue(fields["UsePO"]),
                                       GroupID = CommonUtility.GetStringValue(fields["GroupID"]),
                                       UseFuelRebate = CommonUtility.GetBooleanValue(fields["UseFuelRebate"]),
                                       UseFuelRebateDiscount = CommonUtility.GetBooleanValue(fields["UseFuelRebateDiscount"]),
                                       CL_Note = CommonUtility.GetStringValue(fields["CL_Note"]),
                                       TaxExempt = CommonUtility.GetBooleanValue(fields["TaxExempt"]),
                                       PlateNumber = CommonUtility.GetStringValue(fields["PlateNumber"]),
                                       MultiUse_PO = CommonUtility.GetBooleanValue(fields["MultiUse_PO"]),
                                       TECardNumber = CommonUtility.GetStringValue(fields["TECardNumber"]),
                                       Discount_Code = CommonUtility.GetByteValue(fields["CUST_DISC"])
                                   });
            }

            _performancelog.Debug($"End,CustomerService,Search,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return customers;
        }


        /// <summary>
        /// Search Customers
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="totalrecords"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Customer> SearchArCustomer(string searchCriteria, out int totalrecords, int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,SearchArCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var customers = new List<Customer>();
            var query = "Select * from Client " +
                                              " WHERE  CL_ArCust = 1" +
                                              " AND (CL_NAME like \'%" + searchCriteria + "%\' OR " +
                                              " CL_CODE like \'%" + searchCriteria + "%\' OR " +
                                              " CL_PHONE like \'%" + searchCriteria + "%\' )" +
                                              "ORDER BY CLIENT.CL_CODE, CLIENT.CL_NAME, CLIENT.CL_PHONE";
            var dt = GetPagedRecords(query, DataSource.CSCMaster, pageIndex, pageSize);
            totalrecords = 0;
            if (dt != null)
            {
                totalrecords = dt.Rows.Count;
                customers.AddRange(from DataRow fields in dt.Rows
                                   select new Customer
                                   {
                                       Code = CommonUtility.GetStringValue(fields["CL_CODE"]),
                                       Name = CommonUtility.GetStringValue(fields["CL_NAME"]),
                                       Phone = CommonUtility.GetStringValue(fields["CL_PHONE"]),
                                       Current_Balance = CommonUtility.GetDoubleValue(fields["CL_CURBAL"]),
                                       Credit_Limit = CommonUtility.GetDoubleValue(fields["CL_LIMIT"])
                                   });
            }
            _performancelog.Debug($"End,CustomerService,SearchArCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return customers;
        }


        /// <summary>
        /// Update customer
        /// </summary>
        /// <param name="customer"></param>
        public void UpdateCustomer(Customer customer)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,UpdateCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();
            _adapter = new SqlDataAdapter("SELECT * FROM   Client " +
                                                  " WHERE  Client.CL_Code = \'" + customer.Code +
                                                  "\'", _connection);
            _adapter.Fill(_dataTable);
            if (_dataTable.Rows.Count != 0)
            {
                _dataTable.Rows[0]["CL_CURBAL"] = customer.Current_Balance;
                _dataTable.Rows[0]["LO_POINTS"] = customer.Loyalty_Points;
                SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                _adapter.UpdateCommand = builder.GetUpdateCommand();
                _adapter.Update(_dataTable);
                _connection.Close();
                _adapter?.Dispose();
            }
            _performancelog.Debug($"End,CustomerService,UpdateCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }


        /// <summary>
        /// get all Loyalty customers
        /// </summary>
        /// <param name="additionalCriteria"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Customer> GetLoyaltyCustomers(string additionalCriteria, int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,GetLoyaltyCustomers,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var sqlStmt = "SELECT CLIENT.CL_CODE AS [Code]," +
                             "Client.LO_Num as [Loyalty]," +
                             " CLIENT.CL_NAME AS [Name]," +
                             " CLIENT.CL_PHONE AS [Phone]," +
                             "client.Lo_Points as [Points]" +
                             " FROM   CLIENT" +
                             " WHERE (CL_STATUS  is null  OR CL_Status IN (\'A\',\'F\',\'\'))  " +
                             additionalCriteria +
                             " ORDER BY CLIENT.CL_CODE";

            var dt = GetPagedRecords(sqlStmt, DataSource.CSCMaster, pageIndex, pageSize);
            var customers = new List<Customer>();
            if (dt != null)
            {
                customers.AddRange(from DataRow fields in dt.Rows
                                   select new Customer
                                   {
                                       Name = CommonUtility.GetStringValue(fields["Name"]),
                                       Code = CommonUtility.GetStringValue(fields["Code"]),
                                       Phone = CommonUtility.GetStringValue(fields["Phone"]),
                                       Loyalty_Points = CommonUtility.GetDoubleValue(fields["Points"]),
                                       Loyalty_Code = CommonUtility.GetStringValue(fields["Loyalty"])
                                   });
            }
            _performancelog.Debug($"End,CustomerService,GetLoyaltyCustomers,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return customers;
        }

        /// <summary>
        /// Checks customer by Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsCustomerExist(string code)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,IsCustomerExist,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var dt = GetRecords("SELECT CL_Code FROM Client WHERE Cl_Code=\'" + code.Trim(' ') + "\'", DataSource.CSCMaster);

            _performancelog.Debug($"End,CustomerService,IsCustomerExist,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return dt != null && dt.Rows.Count > 0;
        }

        /// <summary>
        /// Check Customer by LoyaltyNumber
        /// </summary>
        /// <param name="customerModelLoyaltyNumber"></param>
        /// <returns></returns>
        public bool CheckCustomerByLoyaltyNumber(string customerModelLoyaltyNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,CheckCustomerByLoyaltyNumber,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var dt = GetRecords("SELECT CL_Code FROM Client WHERE Lo_Num=\'" + customerModelLoyaltyNumber.Trim(' ') + "\'", DataSource.CSCMaster);
            _performancelog.Debug($"End,CustomerService,CheckCustomerByLoyaltyNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return dt != null && dt.Rows.Count > 0;
        }

        /// <summary>
        /// Save Customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool SaveCustomer(Customer customer)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,SaveCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!string.IsNullOrEmpty(customer.Code))
            {
                _connection = new SqlConnection(GetConnectionString(DataSource.CSCMaster));
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                var addNew = false;
                _dataTable = new DataTable();
                _adapter = new SqlDataAdapter("SELECT * FROM   Client " +
                                                      " WHERE  Client.CL_Code = \'" + customer.Code +
                                                      "\'", _connection);
                _adapter.Fill(_dataTable);
                DataRow fields;
                if (_dataTable.Rows.Count == 0)
                {
                    fields = _dataTable.NewRow();
                    fields["cl_code"] = customer.Code; // Nicolette added for new customers
                    addNew = true;
                }
                else
                {
                    fields = _dataTable.Rows[0];
                }
                fields["Cl_Name"] = customer.Name;
                fields["CL_Add1"] = customer.Address_1;
                fields["CL_Add2"] = customer.Address_2;
                fields["CL_City"] = customer.City;
                fields["CL_Prov"] = customer.Province;
                fields["CL_Country"] = customer.Country;
                fields["CL_Postal"] = customer.Postal_Code;
                fields["CL_Phone"] = customer.Phone;
                fields["CL_Fax"] = customer.Fax;
                fields["CL_Off_Ph"] = customer.Work_Phone;
                fields["CL_Cell"] = customer.Cell_Phone;
                fields["CL_Acode"] = customer.Area_Code;
                fields["CL_EMail"] = customer.E_Mail;
                fields["CL_Toll"] = customer.Toll_Free;
                fields["CL_Categ"] = customer.Category;
                fields["CL_Cont1"] = customer.Contact_1;
                fields["CL_Cont2"] = customer.Contact_2;
                fields["Lo_Num"] = customer.Loyalty_Code;
                fields["Lo_Points"] = customer.Loyalty_Points;
                fields["CL_ARCust"] = customer.AR_Customer;
                fields["CL_Type"] = customer.Customer_Type;
                fields["Terms"] = customer.Terms;
                fields["Discount_Days"] = customer.Discount;
                fields["Early_Pay_Percent"] = customer.Percent;
                fields["CUST_DISC"] = customer.Discount_Code;
                fields["Price_Code"] = customer.Price_Code;
                fields["CL_Status"] = "A";
                fields["CL_HistBal"] = customer.Opening_Balance;
                fields["CL_HPaid"] = customer.Amount_Paid;
                fields["CL_Limit"] = customer.Credit_Limit;
                fields["CL_CurBal"] = customer.Current_Balance;
                fields["UsePO"] = customer.UsePO; 
                fields["MultiUse_PO"] = customer.MultiUse_PO; 
                fields["UseFuelRebate"] = customer.UseFuelRebate; 
                fields["UseFuelRebateDiscount"] = customer.UseFuelRebateDiscount; 
                fields["CL_Note"] = customer.CL_Note; 
                fields["PlateNumber"] = customer.PlateNumber; 
                fields["TaxExempt"] = customer.TaxExempt; 
                fields["TECardNumber"] = customer.TECardNumber; 
                if (addNew)
                {
                    _dataTable.Rows.Add(fields);
                    var builder = new SqlCommandBuilder(_adapter);
                    _adapter.InsertCommand = builder.GetInsertCommand();
                    _adapter.Update(_dataTable);
                }
                else
                {
                    var builder = new SqlCommandBuilder(_adapter);
                    _adapter.UpdateCommand = builder.GetUpdateCommand();
                    _adapter.Update(_dataTable);
                }
                _connection.Close();
                _adapter?.Dispose();
            }
            _performancelog.Debug($"End,CustomerService,SaveCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return true;
        }

        /// <summary>
        /// Search Loyalty customers
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="totalrecords"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<Customer> SearchLoyaltyCustomer(string searchTerm, out int totalrecords, int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,SearchLoyaltyCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var customers = new List<Customer>();
            var query = "SELECT CLIENT.CL_CODE AS[Code]," +
                             "CLIENT.LO_Num as [Loyalty]," +
                             " CLIENT.CL_NAME AS [Name]," +
                             " CLIENT.CL_PHONE AS [Phone]," +
                             "client.Lo_Points as [Points]" +
                             " FROM   CLIENT  left join clientcard on CLIENT.CL_CODE = clientcard.CL_CODE" +
                             " WHERE (CLIENT.CL_STATUS  is null  OR CLIENT.CL_Status IN (\'A\',\'F\',\'\'))  " +
                             " AND (CLIENT.CL_CODE like \'%" + searchTerm + "%\' OR " +
                             " CLIENT.LO_Num like \'%" + searchTerm + "%\' OR " +
                             " CLIENT.CL_NAME like \'%" + searchTerm + "%\' OR " +
                             " CLIENT.CL_PHONE like \'%" + searchTerm + "%\' OR " +
                             " CLIENT.Lo_Points like \'%" + searchTerm + "%\' )" +
                             " ORDER BY CLIENT.CL_CODE, CLIENT.LO_Num, CLIENT.CL_NAME, CLIENT.CL_PHONE";

            var dt = GetPagedRecords(query, DataSource.CSCMaster, pageIndex, pageSize);
            totalrecords = 0;
            if (dt != null)
            {
                totalrecords = dt.Rows.Count;
                foreach (DataRow fields in dt.Rows)
                {
                    var customer = new Customer
                    {
                        Name = CommonUtility.GetStringValue(fields["Name"]),
                        Code = CommonUtility.GetStringValue(fields["Code"]),
                        Phone = CommonUtility.GetStringValue(fields["Phone"]),
                        Loyalty_Points = CommonUtility.GetDoubleValue(fields["Points"]),
                        Loyalty_Code = CommonUtility.GetStringValue(fields["Loyalty"])
                    };
                    customers.Add(customer);
                }
            }
            _performancelog.Debug($"End,CustomerService,SearchLoyaltyCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return customers;
        }

        /// <summary>
        /// Method to get kick back record by card number
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>Kick back</returns>
        public KickBack GetKickBackRecordByCardNumber(string cardNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,GetKickBackRecordByCardNumber,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var kickBack = new KickBack();
            var dt = GetRecords("SELECT * FROM Kickback Where CustomerCardNum=\'" + cardNumber + "\'", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                kickBack.PointCardNumber = Convert.ToString(dt.Rows[0]["PointCardNum"]);
                kickBack.PhoneNumber = Convert.ToString(dt.Rows[0]["phonenum"]);
            }
            _performancelog.Debug($"End,CustomerService,GetKickBackRecordByCardNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return kickBack;
        }

        /// <summary>
        /// Method to get customer card profile
        /// </summary>
        /// <param name="customerCard">Customer card</param>
        /// <returns></returns>
        public string GetCustomerCardProfile(string customerCard)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,GetCustomerCardProfile,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string profileId = string.Empty;
            var dt = GetRecords("select * from ClientCard where CardNum=\'" + customerCard.Trim() + "\'", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                profileId = CommonUtility.GetStringValue(dt.Rows[0]["ProfileID"]);
            }
            _performancelog.Debug($"End,CustomerService,GetCustomerCardProfile,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return profileId;
        }

        /// <summary>
        /// Method to get customer group
        /// </summary>
        /// <param name="groupId">Group Id</param>
        public ClientGroup GetCustomerGroup(string groupId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,GetCustomerGroup,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var clientGroup = new ClientGroup();
            if (!string.IsNullOrEmpty(groupId))
            {
                var dt = GetRecords("SELECT * FROM ClientGroup Where GroupID=\'" + groupId + "\'", DataSource.CSCMaster);
                if (dt == null || dt.Rows.Count == 0)
                {
                    _performancelog.Debug($"End,CustomerService,GetCustomerGroup,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                    return null;
                }
                clientGroup.GroupName = CommonUtility.GetStringValue(dt.Rows[0]["GroupName"]);
                clientGroup.DiscountType = CommonUtility.GetStringValue(dt.Rows[0]["DiscountType"]);
                clientGroup.DiscountRate = CommonUtility.GetFloatValue(dt.Rows[0]["DiscountRate"]);
                clientGroup.Footer = CommonUtility.GetStringValue(dt.Rows[0]["LoyaltyFooter"]);
                clientGroup.DiscountName = CommonUtility.GetStringValue(dt.Rows[0]["DiscountName"]);
            }
            _performancelog.Debug($"End,CustomerService,GetCustomerGroup,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return clientGroup;
        }

        /// <summary>
        /// Get Tax Exempt Card Holder
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public teCardholder GetTaxExemptCustomer(string customerId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,GetTaxExemptCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var cardHolder = new teCardholder();
            bool returnValue = false;

            var dt = GetRecords("Select * From Client where CL_Code = \'" + customerId + "\'", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                if (CommonUtility.GetBooleanValue(dt.Rows[0]["TaxExempt"]))
                {
                    returnValue = true;
                    cardHolder.Name = CommonUtility.GetStringValue(dt.Rows[0]["Cl_Name"]);
                    cardHolder.Address = CommonUtility.GetStringValue(dt.Rows[0]["CL_Add1"]);
                    cardHolder.City = CommonUtility.GetStringValue(dt.Rows[0]["CL_City"]);
                    cardHolder.PlateNumber = CommonUtility.GetStringValue(dt.Rows[0]["PlateNumber"]);
                    cardHolder.PostalCode = CommonUtility.GetStringValue(dt.Rows[0]["CL_Postal"]);
                    cardHolder.Note = CommonUtility.GetStringValue(dt.Rows[0]["CL_Note"]);
                    cardHolder.CardholderID = customerId;
                    cardHolder.CardNumber = customerId;
                }
                cardHolder.IsValidCardHolder = Convert.ToBoolean(returnValue);
            }
            _performancelog.Debug($"End,CustomerService,GetTaxExemptCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return cardHolder;
        }

        /// <summary>
        /// Method to get card holder
        /// </summary>
        /// <param name="isBarCode">Is bar code or not</param>
        /// <param name="strNumber">Number</param>
        /// <param name="matchCount">Match count</param>
        /// <param name="ageRestrict">Age restriction</param>
        /// <returns>Tax exempt card holder</returns>
        public teCardholder GetCardHolder(bool isBarCode, string strNumber, out short matchCount, int ageRestrict)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerService,GetCardHolder,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            string strSql;
            teCardholder cardHolder = new teCardholder { IsValidCardHolder = false };
            if (isBarCode)
            {
                strSql = "Select * from TaxExemptCardRegistry where BarCode=\'" + strNumber + "\'  OR AltBarCode=\'" + strNumber + "\'";
            }
            else
            {
                strSql = "Select * from TaxExemptCardRegistry where CardNumber=\'" + strNumber + "\'  OR AltCardNumber=\'" + strNumber + "\'";
            }
            var dt = GetRecords(strSql, DataSource.CSCMaster);
            matchCount = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                matchCount = Convert.ToInt16(dt.Rows.Count);
                if (matchCount > 1)
                {
                    cardHolder.IsValidCardHolder = false;
                    return cardHolder;
                }
                
                DateTime mBirthDate;
                if (CommonUtility.GetBooleanValue(dt.Rows[0]["Birthdate"]))
                {
                    mBirthDate = DateAndTime.DateAdd(DateInterval.Year, Convert.ToDouble(-1 * (ageRestrict + 1)), DateAndTime.Today);
                }
                else
                {
                    mBirthDate = CommonUtility.GetDateTimeValue(dt.Rows[0]["Birthdate"]);
                }
                cardHolder.Birthdate = mBirthDate;
                

                cardHolder.Name = CommonUtility.GetStringValue(dt.Rows[0]["Name"]);
                cardHolder.CardholderID = CommonUtility.GetStringValue(dt.Rows[0]["CardholderID"]);
                cardHolder.IsValidCardHolder = false;
                if (isBarCode)
                {
                    if (CommonUtility.GetStringValue(dt.Rows[0]["Barcode"]) == strNumber)
                    {
                        cardHolder.ValidateMode = 1;
                        cardHolder.CardNumber = CommonUtility.GetStringValue(dt.Rows[0]["CardNumber"]);
                    }
                    else
                    {
                        cardHolder.ValidateMode = 2;
                        cardHolder.CardNumber = CommonUtility.GetStringValue(dt.Rows[0]["AltCardNumber"]);
                    }
                    cardHolder.Barcode = strNumber;
                }
                else
                {
                    if (CommonUtility.GetStringValue(dt.Rows[0]["CardNumber"]) == strNumber)
                    {
                        cardHolder.ValidateMode = 3;
                        cardHolder.Barcode = CommonUtility.GetStringValue(dt.Rows[0]["Barcode"]);
                    }
                    else
                    {
                        cardHolder.ValidateMode = 4;
                        cardHolder.Barcode = CommonUtility.GetStringValue(dt.Rows[0]["AltBarCode"]);
                    }
                    cardHolder.CardNumber = strNumber;
                }
                cardHolder.GasQuota = CommonUtility.GetFloatValue(dt.Rows[0]["GasQuota"]);
                cardHolder.PropaneQuota = CommonUtility.GetFloatValue(dt.Rows[0]["PropaneQuota"]);
                cardHolder.TobaccoQuota = CommonUtility.GetFloatValue(dt.Rows[0]["TobaccoQuota"]);
                cardHolder.IsValidCardHolder = true;
            }
            _performancelog.Debug($"End,CustomerService,GetCardHolder,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return cardHolder;
        }

        //   end
        /// <summary>
        /// Method to find whether to check allow redemption
        /// </summary>
        /// <param name="customerCard">Customer card</param>
        /// <returns>True or false</returns>
        public bool Check_Allowredemption(string customerCard) 
        {
            if (string.IsNullOrEmpty(customerCard))
            {
                return false;
            }

            var dt = GetRecords("select * from ClientCard where CardNum=\'" + customerCard.Trim() + "\'", DataSource.CSCMaster);
            if (dt != null && dt.Rows.Count > 0)
            {
                return CommonUtility.GetBooleanValue(dt.Rows[0]["AllowRedemption"]);
            }
            else
            {
                return false;
            }
        }
        //shiny end

        /// <summary>
        /// Method to finf given po is used or not
        /// </summary>
        /// <param name="customerCode">Cutomer code</param>
        /// <param name="poNumber">PO number</param>
        /// <returns>True or false</returns>
        public bool UsedCustomerPo(string customerCode, string poNumber)
        {
            var returnValue = false;
            var dt = GetRecords("Select Sernum from Saletend inner join salehead  on SALEHEAD.SALE_NO = SALETEND.SALE_NO AND  SALEHEAD.TILL = SALETEND.till_num  WHERE SALETEND.tendclas = \'ACCOUNT\' and  SALEHEAD.client = \'" + customerCode + "\' and SaleHead.T_Type = \'SALE\' and UPPER(SALETEND.SerNum) = \'" + poNumber.ToUpper() + "\'", DataSource.CSCTills);
            if (dt == null || dt.Rows.Count == 0)
            {
                dt = GetRecords("Select Sernum from Saletend inner join salehead  on SALEHEAD.SALE_NO = SALETEND.SALE_NO AND  SALEHEAD.TILL = SALETEND.till_num  WHERE SALETEND.tendclas = \'ACCOUNT\' and  SALEHEAD.client = \'" + customerCode + "\' and SaleHead.T_Type = \'SALE\' and UPPER(SALETEND.SerNum) = \'" + poNumber.ToUpper() + "\'", DataSource.CSCTrans);
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                returnValue = true;
            }
            return returnValue;
        }


    }//end class
}//end namespace

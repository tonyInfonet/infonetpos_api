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
    public class UtilityService : SqlDbService, IUtilityService
    {
        private SqlConnection _connection;
        private DataTable _dataTable;
        private SqlDataAdapter _adapter;

        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Get the Admin by name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Admin Value</returns>
        public string GetAdminValue(string name)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,GetAdminValue,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var query = $"select * from Admin where Name= '{name}'";
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCAdmin));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);

            if (_dataTable.Rows.Count == 0)
            {
                if (name.Equals("ExemptCode"))
                {
                    var fields = _dataTable.NewRow();
                    fields["Name"] = "ExemptCode";
                    fields["DisplayName"] = "Exemption Code";
                    fields["DataTypeFK"] = 2;
                    fields["Value"] = "";
                    _dataTable.Rows.Add(fields);
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.InsertCommand = builder.GetInsertCommand();
                    _adapter.Update(_dataTable);
                    _connection.Close();
                    _adapter?.Dispose();
                    return string.Empty;
                }
            }
            string strTmp = CommonUtility.GetStringValue(_dataTable.Rows[0]["Value"]);
            _performancelog.Debug($"End,UtilityService,GetAdminValue,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return strTmp;
        }

        /// <summary>
        /// Get the IpAddressess
        /// </summary>
        /// <returns>All Ip address</returns>
        public string GetIpAddresses()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,GetIpAddresses,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var returnValue = "";
            var query = "Select * From POS_IP_Address";
            var rs = GetRecords(query, DataSource.CSCAdmin);
            foreach (DataRow row in rs.Rows)
            {
                returnValue = returnValue + CommonUtility.GetStringValue(row["IP_Address"]);
            }
            _performancelog.Debug($"End,UtilityService,GetIpAddresses,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return returnValue;

        }

        /// <summary>
        /// Get the Pos Id by Ip Address
        /// </summary>
        /// <param name="ipAddress">Ip address</param>
        /// <returns>Pos id</returns>
        public int GetPosId(string ipAddress)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,GetPosId,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var posId = 0;
            var query = $"Select * From POS_IP_Address where IP_Address='{ipAddress}'";
            var sPosId = GetRecords(query, DataSource.CSCAdmin);
            if (sPosId.Rows.Count > 0)
            {
                posId = CommonUtility.GetIntergerValue(sPosId.Rows[0]["ID"]);
            }
            _performancelog.Debug($"End,UtilityService,GetPosId,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return posId;
        }

        /// <summary>
        /// Get Distinct Ip Address
        /// </summary>
        /// <param name="posId">Pos id</param>
        /// <returns>Ip addresses</returns>
        public int GetDistinctIpAddress(byte posId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,GetDistinctIpAddress,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var rs = GetRecords("select DISTINCT POSID from TILLS where POSID<>" + Convert.ToString(posId) + " AND POSID<>0 ", DataSource.CSCMaster);
            var count = rs.Rows.Count;
            _performancelog.Debug($"End,UtilityService,GetDistinctIpAddress,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return count;
        }

        /// <summary>
        /// Get whether write to log or not
        /// </summary>
        /// <param name="posId">Pos id</param>
        /// <returns>Ip addresses</returns>
        public bool CanWritePosLog(int posId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,CanWritePosLog,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var rs = GetRecords("select WritePOSLog from POS_IP_Address where ID=" + posId, DataSource.CSCAdmin);
            if (rs.Rows.Count == 0) return false;
            _performancelog.Debug($"End,UtilityService,CanWritePosLog,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return CommonUtility.GetBooleanValue(rs.Rows[0]["WritePOSLog"]);
        }

        /// <summary>
        /// Save Admin 
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        public void SaveAdminValue(string name, string value)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,SaveAdminValue,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var query = $"select * from Admin where Name= '{name}'";
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
                if (name.Equals("ExemptCode"))
                {
                    DataRow fields = _dataTable.NewRow();
                    fields["Name"] = "ExemptCode";
                    fields["DisplayName"] = "Exemption Code";
                    fields["DataTypeFK"] = 2;
                    fields["Value"] = string.IsNullOrEmpty(value) ? "" : value;
                    _dataTable.Rows.Add(fields);
                    SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
                    _adapter.InsertCommand = builder.GetInsertCommand();
                    _adapter.Update(_dataTable);
                    _connection.Close();
                    _adapter?.Dispose();
                }
            }
            _performancelog.Debug($"End,UtilityService,SaveAdminValue,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Get The pos Address by Pos Id
        /// </summary>
        /// <param name="posId">Pos id</param>
        /// <returns>Ip address</returns>
        public string GetPosAddress(byte posId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,GetPosAddress,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var ipAddress = string.Empty;
            var query = $"Select * From POS_IP_Address where ID={posId}";
            var sPosId = GetRecords(query, DataSource.CSCAdmin);
            if (sPosId.Rows.Count > 0)
            {
                ipAddress = CommonUtility.GetStringValue(sPosId.Rows[0]["IP_Address"]);
            }
            _performancelog.Debug($"End,UtilityService,GetPosAddress,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return ipAddress;
        }

        /// <summary>
        /// Checks If Coupon is Available
        /// </summary>
        /// <param name="couponId">Coupon Id</param>
        /// <returns>True or false</returns>
        public bool IsCouponAvailable(string couponId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,IsCouponAvailable,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var rsCoupon = GetRecords("select * from Coupon where CouponID=\'" + couponId + "\'", DataSource.CSCMaster);
            if (rsCoupon.Rows.Count == 0)
            {
                _performancelog.Debug($"End,UtilityService,GetCustomers,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return true;
            }
            _performancelog.Debug($"End,UtilityService,IsCouponAvailable,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return false;
        }

        /// <summary>
        /// Method to get discount percent 
        /// </summary>
        /// <param name = "productDiscount" > Product discount</param>
        /// <param name = "customerDiscount" > Customer discount</param>
        /// <returns>Discount percent</returns>
        public float? GetDiscountPercent(short productDiscount, short customerDiscount)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,GetDiscountPercent,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            float? discountpercentage = null;
            var rs = GetRecords("Select * From   DiscTab  WHERE  DiscTab.Prod_Disc = "
                                      + Convert.ToString(productDiscount) + " AND " + "       DiscTab.Cust_Disc = "
                                      + Convert.ToString(customerDiscount) + " ", DataSource.CSCMaster);
            if (rs.Rows.Count > 0)
            {
                discountpercentage = CommonUtility.GetFloatValue(rs.Rows[0]["Disc_Perc"]);
            }
            _performancelog.Debug($"End,UtilityService,GetDiscountPercent,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return discountpercentage;
        }

        /// <summary>
        /// Method to check whether an age restriction exists
        /// </summary>
        /// <param name="intRestr">Restriction code</param>
        /// <returns>Restriction</returns>
        public Restriction ExistsRestriction(short intRestr)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,ExistsRestriction,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var restriction = new Restriction { Code = intRestr };
            var query = "SELECT * FROM [Restrictions]  WHERE Code=" + Convert.ToString(intRestr);

            var rs = GetRecords(query, DataSource.CSCMaster);

            if (rs.Rows.Count == 0)
            {
                restriction.Exist_Restriction = false;
            }
            else
            {
                restriction.Exist_Restriction = true;
                short ageLimit = CommonUtility.GetShortValue(rs.Rows[0]["AgeLimit"]);
                if (ageLimit == 0)
                {
                    restriction.Description = CommonUtility.GetStringValue(rs.Rows[0]["Restriction"]);
                }
                else
                {
                    restriction.Description = CommonUtility.GetStringValue(rs.Rows[0]["Restriction"]) + " " + DateAndTime.DateAdd(DateInterval.Year, -1 * ageLimit, DateAndTime.Today).ToString("MM/dd/yyyy");
                }
            }
            _performancelog.Debug($"End,UtilityService,ExistsRestriction,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return restriction;
        }

        /// <summary>
        /// Method to get all departments
        /// </summary>
        /// <returns>List of departments</returns>
        public List<Department> GetAllDepartments()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,GetAllDepartments,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var rs = GetRecords("SELECT Dept, Dept_Name, Cnt_Detail FROM Dept Where Cnt_Detail>0 AND Cnt_Detail<5 ORDER BY Dept", DataSource.CSCMaster);
            var departments = new List<Department>();
            foreach (DataRow row in rs.Rows)
            {
                departments.Add(new Department
                {
                    Dept = CommonUtility.GetStringValue(row["Dept"]),
                    DeptName = CommonUtility.GetStringValue(row["Dept_Name"]),
                    CountDetail = CommonUtility.GetIntergerValue(row["Cnt_Detail"])
                });
            }
            _performancelog.Debug($"End,UtilityService,GetAllDepartments,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return departments;
        }

        /// <summary>
        /// Method to get department by id
        /// </summary>
        /// <param name="department">Department Id</param>
        /// <returns>List of departments</returns>
        public List<Department> GetDepartmentById(string department)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,GetDepartmentById,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var rs = GetRecords("SELECT Dept,Dept_Name, Cnt_Detail FROM Dept  WHERE (Cnt_Detail > 0 AND Cnt_Detail < 5) AND Dept=\'" + department + "\'", DataSource.CSCMaster);
            var departments = new List<Department>();
            foreach (DataRow row in rs.Rows)
            {
                departments.Add(new Department
                {
                    Dept = CommonUtility.GetStringValue(row["Dept"]),
                    DeptName = CommonUtility.GetStringValue(row["Dept_Name"]),
                    CountDetail = CommonUtility.GetIntergerValue(row["Cnt_Detail"])
                });
            }
            _performancelog.Debug($"End,UtilityService,GetDepartmentById,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return departments;
        }

        /// <summary>
        /// Method to get sub department name
        /// </summary>
        /// <param name="department">Department</param>
        /// <param name="subDepartment">Sub department</param>
        /// <returns>Sub department name</returns>
        public string GetSubDepartmentName(string department, string subDepartment)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,GetSubDepartmentName,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var rsName = GetRecords("SELECT Sub_Name FROM SubDept WHERE Dept=\'" + department + "\' AND Sub_Dept=\'" + subDepartment + "\'", DataSource.CSCMaster);

            if (rsName.Rows.Count > 0)
            {
                _performancelog.Debug($"End,UtilityService,GetSubDepartmentName,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return CommonUtility.GetStringValue(rsName.Rows[0]["Sub_Name"]);
            }
            _performancelog.Debug($"End,UtilityService,GetSubDepartmentName,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return null;
        }

        /// <summary>
        /// Method to get sub detail
        /// </summary>
        /// <param name="department">Department</param>
        /// <param name="subDepartment">Sub department</param>
        /// <param name="subDetail">Sub detail</param>
        /// <returns>Sub detail name</returns>
        public string GetSubDetailName(string department, string subDepartment, string subDetail)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,GetSubDetailName,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var subDetailName = string.Empty;
            var rsName = GetRecords("SELECT SubDetail_Name FROM SubDetail WHERE Dept=\'"
                                + department + "\' AND SubDept=\'"
                                + subDepartment
                                + "\' AND SubDetail=\'"
                                + subDetail
                                + "\' ", DataSource.CSCMaster);

            if (rsName.Rows.Count > 0)
            {
                subDetailName = CommonUtility.GetStringValue(rsName.Rows[0]["SubDetail_Name"]);
            }
            _performancelog.Debug($"End,UtilityService,GetSubDetailName,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return subDetailName;
        }


        /// <summary>
        /// Method to get fuel department id
        /// </summary>
        /// <returns>Fuel department</returns>
        public string GetFuelDepartmentId()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,GetFuelDepartmentId,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string fuelDept = null;
            var recFuelDept = GetRecords("select * from FuelDept", DataSource.CSCMaster);
            if (recFuelDept.Rows.Count > 0)
            {
                fuelDept = CommonUtility.GetStringValue(recFuelDept.Rows[0]["Dept"]);
            }
            _performancelog.Debug($"End,UtilityService,GetFuelDepartmentId,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return fuelDept;
        }

        /// <summary>
        /// Method to get total departments
        /// </summary>
        /// <returns>Total departments</returns>
        public int GetTotalDepartments()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,GetTotalDepartments,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var rs = GetRecords("SELECT COUNT(*) AS Tot FROM Dept  WHERE Cnt_Detail>0", DataSource.CSCMaster);
            _performancelog.Debug($"End,UtilityService,GetTotalDepartments,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return CommonUtility.GetIntergerValue(rs.Rows[0]["Tot"]);
        }

        /// <summary>
        /// Method to load device
        /// </summary>
        /// <returns>Device</returns>
        public Device LoadDevice()
        {
            Device device = null;
            var query = "select max(DeviceID) as MaxID from Device";

            var records = GetRecords(query, DataSource.CSCAdmin);
            if (records.Rows.Count > 0)
            {
                var maxId = CommonUtility.GetIntergerValue(records.Rows[0]["MAXID"]);
                if (maxId > 0)
                {
                    device = new Device { DeviceName = new string[maxId + 1] };
                    var rsDevice = GetRecords("select * from Device", DataSource.CSCAdmin);
                    foreach (DataRow row in rsDevice.Rows)
                    {
                        device.DeviceName[CommonUtility.GetIntergerValue(row["DeviceId"])] = CommonUtility.GetStringValue(row["DeviceName"]);
                        string deviceName = CommonUtility.GetStringValue(row["DeviceName"]);
                        switch (Strings.UCase(deviceName))
                        {
                            case "SCANNER":
                                device.ScannerID = CommonUtility.GetByteValue(row["DeviceID"]);
                                break;
                            case "RECEIPTPRINTER":
                                device.ReceiptPrinterID = CommonUtility.GetByteValue(row["DeviceID"]);
                                break;
                            case "MSR":
                                device.MSRID = CommonUtility.GetByteValue(row["DeviceID"]);
                                break;
                            case "CASHDRAWER":
                                device.CashDrawerID = CommonUtility.GetByteValue(row["DeviceID"]);
                                break;
                            case "CUSTOMERDISPLAY":
                                device.CustomerDisplayID = CommonUtility.GetByteValue(row["DeviceID"]);
                                break;
                            case "REPORTPRINTER":
                                device.ReportPrinterID = CommonUtility.GetByteValue(row["DeviceID"]);
                                break;
                            case "SCALE":
                                device.ScaleID = CommonUtility.GetByteValue(row["DeviceID"]);
                                break;

                        }
                    }
                }
            }
            return device;
        }

        /// <summary>
        /// Method to set register info
        /// </summary>
        /// <param name="registerNumber">Register number</param>
        /// <param name="device">Device</param>
        /// <returns>Register</returns>
        public Register SetRegisterInfo(short registerNumber, Device device)
        {
            var rs = GetRecords("Select *  FROM Register  WHERE Register.Reg_No = " + Convert.ToString(registerNumber), DataSource.CSCAdmin);
            if (rs.Rows.Count == 0) return null;
            var register = new Register { Register_Num = registerNumber };
            foreach (DataRow row in rs.Rows)
            {
                if (CommonUtility.GetByteValue(row["DeviceID"]) == device.ScannerID)
                {
                    register.Scanner = CommonUtility.GetBooleanValue(row["Active"]);
                    register.Opos_Scanner = CommonUtility.GetBooleanValue(row["UseOPOS"]);
                    register.Scanner_Name = CommonUtility.GetStringValue(row["DeviceName"]);
                    register.Scanner_Port = CommonUtility.GetStringValue(row["PortNum"]);
                    register.Scanner_Setting = CommonUtility.GetStringValue(row["PortSetting"]);
                }
                else if (CommonUtility.GetByteValue(row["DeviceID"]) == device.CashDrawerID)
                {
                    register.Cash_Drawer = CommonUtility.GetBooleanValue(row["Active"]);
                    register.Opos_Cash_Drawer = CommonUtility.GetBooleanValue(row["UseOPOS"]);
                    register.Cash_Drawer_Name = CommonUtility.GetStringValue(row["DeviceName"]);
                    register.Cash_Drawer_Open_Code = CommonUtility.GetShortValue(row["PortNum"]);
                }
                else if (CommonUtility.GetByteValue(row["DeviceID"]) == device.CustomerDisplayID)
                {
                    register.Customer_Display = CommonUtility.GetBooleanValue(row["Active"]);
                    register.Opos_Customer_Display = CommonUtility.GetBooleanValue(row["UseOPOS"]);
                    register.Customer_Display_Name = CommonUtility.GetStringValue(row["DeviceName"]);
                    if (CommonUtility.GetStringValue(row["DriverName"]).Length == 0)
                    {
                        register.Customer_Display_Code = 0;
                    }
                    else
                    {
                        register.Customer_Display_Code = (byte)(Conversion.Val(Strings.Left(CommonUtility.GetStringValue(row["DriverName"]), 1)));
                    }
                    register.Customer_Display_Port = CommonUtility.GetByteValue(row["PortNum"]);
                    if (CommonUtility.GetBooleanValue(row["UseOPOS"]))
                    {
                        register.Customer_Display_Len = 20;
                    }
                }
                else if (CommonUtility.GetByteValue(row["DeviceID"]) == device.ReceiptPrinterID)
                {
                    register.Receipt_Printer = CommonUtility.GetBooleanValue(row["Active"]);
                    register.Opos_Receipt_Printer = CommonUtility.GetBooleanValue(row["UseOPOS"]);
                    register.ReceiptPrinterName = CommonUtility.GetStringValue(row["DeviceName"]);
                    register.ReceiptDriver = CommonUtility.GetStringValue(row["DriverName"]);
                }
                else if (CommonUtility.GetByteValue(row["DeviceID"]) == device.ReportPrinterID)
                {
                    register.Report_Printer = CommonUtility.GetBooleanValue(row["Active"]);
                    register.Opos_Report_Printer = CommonUtility.GetBooleanValue(row["UseOPOS"]);
                    register.Report_Printer_Name = CommonUtility.GetStringValue(row["DeviceName"]);
                    register.Report_Printer_Driver = CommonUtility.GetStringValue(row["DriverName"]);
                    register.Report_Printer_font = CommonUtility.GetStringValue(row["FontName"]);
                    register.Report_Printer_font_size = CommonUtility.GetIntergerValue(row["FontSize"]);
                }
                else if (CommonUtility.GetByteValue(row["DeviceID"]) == device.MSRID)
                {
                    register.MSR = CommonUtility.GetBooleanValue(row["Active"]);
                    register.Opos_MSR = CommonUtility.GetBooleanValue(row["UseOPOS"]);
                    register.MSR_Name = CommonUtility.GetStringValue(row["DeviceName"]);
                }
                else if (CommonUtility.GetByteValue(row["DeviceID"]) == device.ScaleID)
                {
                    register.UseScale = CommonUtility.GetBooleanValue(row["Active"]);
                    register.OPOS_Scale = CommonUtility.GetBooleanValue(row["UseOPOS"]);
                    register.SCALE_Name = CommonUtility.GetStringValue(row["DeviceName"]);
                }
            }
            return register;
        }

        /// <summary>
        /// Method to get lsit of sounds
        /// </summary>
        /// <returns>List of sounds</returns>
        public List<Sound> GetAllSounds()
        {
            var sounds = new List<Sound>();
            var rs = GetRecords("SELECT * FROM Sounds", DataSource.CSCMaster);
            foreach (DataRow row in rs.Rows)
            {
                sounds.Add(new Sound
                {
                    ID = CommonUtility.GetIntergerValue(row["ID"]),
                    Active = CommonUtility.GetBooleanValue(row["Active"]),
                    SoundName = CommonUtility.GetStringValue(row["soundname"]),
                    SoundType = CommonUtility.GetStringValue(row["soundType"])
                });
            }
            return sounds;
        }

        /// <summary>
        /// Method to get all message buttons
        /// </summary>
        /// <returns>Message buttons list</returns>
        public List<MessageButton> GetAllMessageButtons()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,GetAllMessageButtons,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var messages = new List<MessageButton>();
            //var dt = GetRecords("SELECT *  FROM   MessageButtons  WHERE  Button >= "
            //    + Convert.ToString(pageIndex) + " AND " + " Button <= "
            //    + Convert.ToString(pageSize) + " " + "ORDER BY Button ", DataSource.CSCMaster);
            var dt = GetRecords("SELECT *  FROM   MessageButtons ORDER BY Button ", DataSource.CSCMaster);
            foreach (DataRow row in dt.Rows)
            {
                messages.Add(new MessageButton
                {
                    Index = CommonUtility.GetIntergerValue(row["Button"]),
                    Caption = CommonUtility.GetStringValue(row["Caption"]),
                    Message = CommonUtility.GetStringValue(row["Message"]),
                });
            }
            _performancelog.Debug($"End,UtilityService,GetAllMessageButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return messages;
        }

        /// <summary>
        /// Method to save a message button information
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="till">Till</param>
        /// <param name="userCode">User code</param>
        public void SaveMessageButton(MessageButton message, Till till, string userCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,SaveMessageButton,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var query = "select * from Messages";
            _connection = new SqlConnection(GetConnectionString(DataSource.CSCTrans));
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            _dataTable = new DataTable();

            _adapter = new SqlDataAdapter(query, _connection);
            _adapter.Fill(_dataTable);
            var fields = _dataTable.NewRow();
            fields["TillID"] = till.Number;
            fields["Shift"] = till.Shift;
            fields["ShiftDate"] = till.ShiftDate;
            fields["NoteTime"] = DateTime.Now;
            fields["User"] = userCode;
            fields["MessageID"] = message.Index;
            fields["Description"] = message.Caption;
            fields["Message"] = message.Message;
            _dataTable.Rows.Add(fields);
            SqlCommandBuilder builder = new SqlCommandBuilder(_adapter);
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.Update(_dataTable);
            _connection.Close();
            _adapter?.Dispose();
            _performancelog.Debug($"End,UtilityService,SaveMessageButton,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Method to get message by button id
        /// </summary>
        /// <param name="buttonId">Button id</param>
        /// <returns>Message button</returns>
        public MessageButton GetMessageByButtonId(int buttonId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UtilityService,GetMessageByButtonId,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var dt = GetRecords("SELECT *  FROM   MessageButtons  WHERE  Button = "
                + Convert.ToString(buttonId), DataSource.CSCMaster);
            _performancelog.Debug($"End,UtilityService,GetMessageByButtonId,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else return new MessageButton
            {
                Index = CommonUtility.GetIntergerValue(dt.Rows[0]["Button"]),
                Caption = CommonUtility.GetStringValue(dt.Rows[0]["Caption"]),
                Message = CommonUtility.GetStringValue(dt.Rows[0]["Message"]),
            };
        }
    }
}

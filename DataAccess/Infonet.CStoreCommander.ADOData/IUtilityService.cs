using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IUtilityService
    {
        /// <summary>
        /// Get the PosId By IpAddress
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns>PosId</returns>
        int GetPosId(string ipAddress);

        /// <summary>
        /// Get the Admin Value by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Admin Value </returns>
        string GetAdminValue(string name);

        /// <summary>
        /// Save admin Value 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void SaveAdminValue(string name, string value);

        /// <summary>
        /// Get The IP Address
        /// </summary>
        /// <returns>IP Address</returns>
        string GetIpAddresses();

        /// <summary>
        /// Get the IP Address
        /// </summary>
        /// <param name="posId"></param>
        /// <returns></returns>
        int GetDistinctIpAddress(byte posId);

        /// <summary>
        /// Get the Pos Address by PosId
        /// </summary>
        /// <param name="posId"></param>
        /// <returns></returns>
        string GetPosAddress(byte posId);

        /// <summary>
        /// Checks whether coupon is available
        /// </summary>
        /// <param name="couponId"></param>
        /// <returns></returns>
        bool IsCouponAvailable(string couponId);

        /// <summary>
        /// Method to get discount percent 
        /// </summary>
        /// <param name="productDiscount">Product discount</param>
        /// <param name="customerDiscount">Customer discount</param>
        /// <returns>Discount percent</returns>
        float? GetDiscountPercent(short productDiscount, short customerDiscount);

        /// <summary>
        /// Method to check whether an age restriction exists
        /// </summary>
        /// <param name="intRestr"></param>
        /// <returns>True or false</returns>
        Restriction ExistsRestriction(short intRestr);


        /// <summary>
        /// Method to get all departments
        /// </summary>
        /// <returns>List of departments</returns>
        List<Department> GetAllDepartments();

        /// <summary>
        /// Method to get department by id
        /// </summary>
        /// <returns>List of departments</returns>
        List<Department> GetDepartmentById(string department);

        /// <summary>
        /// Method to get sub department name
        /// </summary>
        /// <param name="department">Department</param>
        /// <param name="subDepartment">Sub department</param>
        /// <returns></returns>
        string GetSubDepartmentName(string department, string subDepartment);

        /// <summary>
        /// Method to get sub detail
        /// </summary>
        /// <param name="department">Department</param>
        /// <param name="subDepartment">Sub department</param>
        /// <param name="subDetail">Sub detail</param>
        /// <returns></returns>
        string GetSubDetailName(string department, string subDepartment, string subDetail);

        /// <summary>
        /// Method to get fuel department id
        /// </summary>
        /// <returns>Fuel department</returns>
        string GetFuelDepartmentId();

        /// <summary>
        /// Method to get all departments count
        /// </summary>
        /// <returns></returns>
        int GetTotalDepartments();


        /// <summary>
        /// Method to load device
        /// </summary>
        /// <returns>Device</returns>
        Device LoadDevice();

        /// <summary>
        /// Method to set register info
        /// </summary>
        /// <param name="registerNumber">Register number</param>
        /// <param name="device">Device</param>
        /// <returns>Register</returns>
        Register SetRegisterInfo(short registerNumber, Device device);

        /// <summary>
        /// Method to get lsit of sounds
        /// </summary>
        /// <returns>List of sounds</returns>
        List<Sound> GetAllSounds();

        /// <summary>
        /// Method to get all message buttons
        /// </summary>
        /// <returns>Message buttons list</returns>
        List<MessageButton> GetAllMessageButtons();

        /// <summary>
        /// Method to save a message button information
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="till">Till</param>
        /// <param name="userCode">User code</param>
        void SaveMessageButton(MessageButton message, Till till, string userCode);

        /// <summary>
        /// Method to get message by button id
        /// </summary>
        /// <param name="buttonId">Button id</param>
        /// <returns>Message button</returns>
        MessageButton GetMessageByButtonId(int buttonId);

        /// <summary>
        /// Get whether write to log or not
        /// </summary>
        /// <param name="posId">Pos id</param>
        /// <returns>Ip addresses</returns>
        bool CanWritePosLog(int posId);
    }
}

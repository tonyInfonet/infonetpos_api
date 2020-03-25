using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    /// <summary>
    /// Login Manager interface 
    /// </summary>
    public interface ILoginManager
    {
        /// <summary>
        /// Authenitcate the POS 
        /// </summary>
        /// <param name="ipAddress">Ip address</param>
        /// <param name="message">Message</param>
        /// <param name="error">Error message</param>
        /// <returns>Pos id</returns>
        int Authenticate(string ipAddress, out string message, out ErrorMessage error);

        /// <summary>
        /// Check for valid user
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        bool IsValidUser(string userName, string password, out ErrorMessage error);

        /// <summary>
        /// Get the user by user code
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <returns>User</returns>
        User GetUser(string userCode);

        /// <summary>
        /// Get the user by user code in cache
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <returns>User</returns>
        User GetExistingUser(string userCode);

        /// <summary>
        /// Get the pos id by ip address
        /// </summary>
        /// <param name="ipAddress">Ip address</param>
        /// <returns>Pos Id</returns>
        int GetPosId(string ipAddress);

        /// <summary>
        /// Get the ip address by pos id 
        /// </summary>
        /// <param name="posId">Pos id</param>
        /// <returns>Ip address</returns>
        string GetIpAddress(int posId);


        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="modelUserName">User name</param>
        /// <param name="modelPassword">Password</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>True or false</returns>
        bool ChangePassword(string modelUserName, string modelPassword, out ErrorMessage errorMessage);


        /// <summary>
        /// Change User
        /// </summary>
        /// <param name="currentUserCode">Current user code</param>
        /// <param name="userName">New user name</param>
        /// <param name="password">New user password</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <param name="shiftDate">Shift date</param>
        /// <param name="posId">POS id</param>
        /// <param name="unAuthorizedAccess">Unauthorizes switch user or not</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="newUser">New user name</param>
        /// <returns>True or false</returns>
        bool ChangeUser(string currentUserCode, string userName, string password,
            int tillNumber, int? shiftNumber, string shiftDate, int posId,
            bool unAuthorizedAccess,out ErrorMessage errorMessage, out string newUser);


        /// <summary>
        /// Method to get Installed Date 
        /// </summary>
        /// <param name="security">Security</param>
        void GetInstallDate(ref Security security);

        /// <summary>
        /// Get password by user code
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>User</returns>
        string GetPassword(string userCode, out ErrorMessage error);
    }
}

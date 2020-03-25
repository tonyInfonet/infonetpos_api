
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// Login service Interface
    /// </summary>
    public interface ILoginService
    {
        #region CSCAdmin services

        /// <summary>
        /// Method to load Security information 
        /// </summary>
        /// <returns>Security</returns>
        Store LoadStoreInfo();

        /// <summary>
        /// Load the security information 
        /// </summary>
        /// <returns></returns>
        Security LoadSecurityInfo();

        /// <summary>
        /// Get pos id 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        short GetPosId(string ipAddress);

        #endregion


        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="modelUserName">User name</param>
        /// <param name="modelPassword">Password</param>
        /// <returns>True or false</returns>
        bool ChangePassword(string modelUserName, string modelPassword);


        /// <summary>
        /// Check POS ID of the logged in User
        /// </summary>
        /// <param name="userName">User names</param>
        /// <param name="posId">Pos Id</param>
        /// <returns>Trur or false</returns>
        bool CheckLoggedinUserPos(string userName, int posId);

        /// <summary>
        /// Updates the Logged in User
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>True or false</returns>
        bool UpdateLoggedInUser(string userName, int tillNumber);
    }
}
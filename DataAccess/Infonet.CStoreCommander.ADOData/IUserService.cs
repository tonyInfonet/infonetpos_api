using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// User Service interface
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Get the user by User Code
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        User GetUser(string userCode);
        
        /// <summary>
        /// Get the User Group by User Code
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns>User_Group</returns>
        User_Group GetUserGroup(string userCode);
    }
}

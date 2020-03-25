using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using System;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// User Service
    /// </summary>
    public class UserService : SqlDbService, IUserService
    {
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Get User by User Code
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <returns>User</returns>
        public User GetUser(string userCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UserService,GetUser,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var query = $"Select U_CODE,U_NAME,EPW from [USER] where U_CODE='{userCode}'";
            var dt = GetRecords(query, DataSource.CSCAdmin);
            if (dt != null && dt.Rows.Count > 0)
            {
                var user = new User
                {
                    Code = CommonUtility.GetStringValue(dt.Rows[0]["U_CODE"]),
                    Name = CommonUtility.GetStringValue(dt.Rows[0]["U_NAME"]),
                    epw = CommonUtility.GetStringValue(dt.Rows[0]["EPW"]),
                    User_Group = GetUserGroup(CommonUtility.GetStringValue(dt.Rows[0]["U_CODE"]))
                };
                _performancelog.Debug($"End,TillService,GetUser,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return user;
            }
            _performancelog.Debug($"End,TillService,GetUser,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return null;
        }

        /// <summary>
        /// Get User Group  by user Code
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <returns>User_Group</returns>
        public User_Group GetUserGroup(string userCode)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,UserService,GetUserGroup,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var userGroup = new User_Group();
            var query =
                $"select Uig.UGroup, UGroup.UG_Name, UGroup.SecurityLevel From Uig INNER JOIN UGroup ON Uig.UGroup = UGroup.UGroup Where  Uig.[User] = '{userCode}'";
            var dt = GetRecords(query, DataSource.CSCAdmin);
            if (dt != null && dt.Rows.Count > 0)
            {
                userGroup.Code = CommonUtility.GetStringValue(dt.Rows[0]["UGroup"]);
                userGroup.Name = CommonUtility.GetStringValue(dt.Rows[0]["UG_Name"]);
                userGroup.SecurityLevel = CommonUtility.GetByteValue(dt.Rows[0]["SecurityLevel"]);
            }
            _performancelog.Debug($"End,TillService,GetUserGroup,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return userGroup;
        }
    }
}

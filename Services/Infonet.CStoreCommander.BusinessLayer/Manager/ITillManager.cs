using System.Collections.Generic;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    /// <summary>
    /// Till Manger interface
    /// </summary>
    public interface ITillManager
    {
        /// <summary>
        /// GetShifts
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="tillNumber"></param>
        /// <param name="posId"></param>
        /// <param name="errorMessage"></param>
        /// <param name="shiftsUsedForDay"></param>
        /// <param name="floatAmount"></param>
        /// <returns>Shifts</returns>
        List<Shift> GetShifts(string userName, int tillNumber, int posId, out ErrorMessage errorMessage,
            out bool shiftsUsedForDay, out decimal floatAmount);

        /// <summary>
        /// Get Till numbers
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="posId"></param>
        /// <param name="preDefinedTill"></param>
        /// <param name="errorMessage"></param>
        /// <returns>Till Number</returns>
        List<int> GetTills(string userName, int posId, int preDefinedTill, out ErrorMessage errorMessage);
        /// <summary>
        /// Check if there is any available till
        /// </summary>
        /// <returns></returns>
        bool IsTillAvailable();

        /// <summary>
        /// check for active till
        /// </summary>
        /// <param name="posId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool IsActiveTillAvailable(int posId, out ErrorMessage errorMessage);


        /// <summary>
        /// Update till number
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="shiftNumber"></param>
        /// <param name="shiftDate"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="posId"></param>
        /// <param name="floatAmount"></param>
        /// <param name="errorMessage"></param>
        string UpdateTillInformation(int tillNumber, int? shiftNumber, string shiftDate, string userName, string password, int posId, decimal floatAmount, out ErrorMessage errorMessage);

        /// <summary>
        /// Logout the Till
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="error">Error message</param>
        /// <returns></returns>
        bool Logout(int tillNumber, out ErrorMessage error);

        /// <summary>
        /// Method to get all tills
        /// </summary>
        /// <returns>List of tills</returns>
        Dictionary<int, string> GetAllTills();


        /// <summary>
        /// Method to get all shifts
        /// </summary>
        /// <returns>List of shifts</returns>
        Dictionary<int, string> GetAllShifts();
    }
}
using System;
using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// Till Service interface
    /// </summary>
    public interface ITillService
    {
        /// <summary>
        /// Get the Tills
        /// </summary>
        /// <param name="posId">Pos Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftDate">Shift date</param>
        /// <param name="userCode">User code</param>
        /// <param name="active">Active</param>
        /// <param name="process">Process</param>
        /// <returns>Tills</returns>
        List<Till> GetTills(int? posId, int? tillNumber, DateTime? shiftDate, string userCode, int? active, int? process);

        /// <summary>
        /// Get the till by till number
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <returns>Till</returns>
        Till GetTill(int tillNumber);

        /// <summary>
        /// Get the tills which are not pay at pump 
        /// </summary>
        /// <param name="payAtPumpTill"></param>
        /// <returns>Tills</returns>
        List<Till> GetnotPayAtPumpTill(int payAtPumpTill);

        /// <summary>
        /// Get the tills for a given user
        /// </summary>
        /// <param name="active"></param>
        /// <param name="payAtPumpTill"></param>
        /// <param name="userLoggedOn"></param>
        /// <returns>Tills</returns>
        List<Till> GetTillForUser(int active, int payAtPumpTill, string userLoggedOn);

        /// <summary>
        /// Clear the on active till
        /// </summary>
        /// <param name="active"></param>
        /// <param name="tillNumber"></param>
        void ClearNonActiveTill(int active, string tillNumber);

        /// <summary>
        /// Update the till
        /// </summary>
        /// <param name="till"></param>
        /// <returns>Till</returns>
        Till UpdateTill(Till till);

        /// <summary>
        /// Get the maximum till number
        /// </summary>
        /// <param name="shiftDate"></param>
        /// <param name="tillNumber"></param>
        /// <returns>Maximum till number</returns>
        int GetMaximumShiftNumber(DateTime shiftDate, int? tillNumber);

        /// <summary>
        /// Method to get all tills
        /// </summary>
        /// <returns>List of till numbers</returns>
        List<int> GetAllTills();


        /// <summary>
        /// Method to inactivate process
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="amount">Amount</param>
        decimal UpdateCash(int tillNumber, decimal amount);

        /// <summary>
        /// Is Till Exists
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        bool IsTillExists(int tillNumber);
    }
}

using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// Get Shift Service interface
    /// </summary>
    public interface IShiftService
    {
        /// <summary>
        /// Get maximum Shift number
        /// </summary>
        /// <returns></returns>
        int GetMaximumShiftNumber();

        /// <summary>
        /// Get the next active  shift 
        /// </summary>
        /// <param name="active"></param>
        /// <param name="tillNumber"></param>
        /// <returns>ShiftNumber</returns>
        List<ShiftStore> GetNextActiveShift(int active, int tillNumber);

        /// <summary>
        /// Get the next Shift 
        /// </summary>
        /// <param name="shiftNumber"></param>
        /// <param name="active"></param>
        /// <param name="tillNumber"></param>
        /// <returns>Shifts</returns>
        List<ShiftStore> GetNextShift(int shiftNumber, int active, int tillNumber);

        /// <summary>
        /// Get Shift 
        /// </summary>
        /// <param name="shiftNumber"></param>
        /// <param name="active"></param>
        /// <returns>Shift</returns>
        ShiftStore GetShift(int shiftNumber, int active);

        /// <summary>
        /// Get Shift by shift number 
        /// </summary>
        /// <param name="shiftNumber"></param>
        /// <returns>Shift</returns>
        ShiftStore GetShiftByShiftNumber(int shiftNumber);

        /// <summary>
        /// Get Shifts
        /// </summary>
        /// <param name="active"></param>
        /// <returns>Shifts</returns>
        List<ShiftStore> GetShifts(byte? active);

        /// <summary>
        /// Update Shift
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        ShiftStore UpdateShift(ShiftStore shift);
    }
}
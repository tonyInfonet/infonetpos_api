using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// Bottle Return interface
    /// </summary>
    public interface IBottleReturnService
    {
        #region CSCMater services

        /// <summary>
        /// Get Bottle Returns to display on Main Grid
        /// </summary>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <returns>List of bottles</returns>    
        List<BottleReturn> GetBottlesFromDbMaster(int startIndex, int endIndex);

        #endregion

        #region CSCTrans services

        /// <summary>
        /// Method to save bottle return
        /// </summary>
        /// <param name="brPayment">Bottle return payment</param>
        void SaveBottleReturnsToDbTrans(BR_Payment brPayment);

        #endregion
    }
}

using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IMaintenanceService
    {

        /// <summary>
        /// Method to get terminal Ids
        /// </summary>
        /// <param name="posId">POS Id</param>
        /// <returns></returns>
        List<Terminal> GetTerminalIds(byte posId);

        /// <summary>
        /// Method to set close batch number
        /// </summary>
        /// <param name="cc">Credit card</param>
        void SetCloseBatchNumber(Credit_Card cc);
    }
}

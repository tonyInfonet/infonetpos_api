using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IBottleManager
    {
        /// <summary>
        /// Get Bottles
        /// </summary>
        /// <param name="pageId">Page ID</param>
        /// <returns>List of bottles</returns>
        List<BottleReturn> GetBottles(int pageId = 1);


        /// <summary>
        /// Save Bottle return
        /// </summary>
        /// <param name="brPayment">Br payment</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <param name="report"></param>
        /// <param name="openDrawer">Open drawer or not</param>
        Sale SaveBottleReturn(BR_Payment brPayment, out ErrorMessage error, out Report report, out bool openDrawer);
    }
}

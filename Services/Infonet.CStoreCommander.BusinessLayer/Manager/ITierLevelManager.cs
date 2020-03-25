using System.Collections.Generic;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ITierLevelManager
    {
        /// <summary>
        /// Add or Update Tier Level for pump
        /// </summary>
        /// <param name="pumpIds"></param>
        /// <param name="tierId"></param>
        /// <param name="levelId"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        TierLevelResponse AddUpdateTierLevel(List<int> pumpIds, int tierId, int levelId, out ErrorMessage error);

        /// <summary>
        /// Get Tier Level for all pumps
        /// </summary>
        /// <returns></returns>
        TierLevelResponse GetAllPumps();
    }
}
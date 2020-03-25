using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IReasonManager
    {
        /// <summary>
        /// Method to get reasons
        /// </summary>
        /// <param name="reasonType">Reason type</param>
        /// <returns>List of reasons</returns>
        List<Return_Reason> GetReasons(ReasonType reasonType);

        /// <summary>
        /// Method to get reason type name
        /// </summary>
        /// <param name="reasonType">Reason name</param>
        /// <returns>Reason description</returns>
        string GetReasonType(ReasonType reasonType);
    }
}

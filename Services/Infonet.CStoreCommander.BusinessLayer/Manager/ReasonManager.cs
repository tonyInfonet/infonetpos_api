using System;
using System.Collections.Generic;
using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class ReasonManager : ManagerBase, IReasonManager
    {
        private readonly IReasonService _reasonService;
        private readonly IApiResourceManager _resourceManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="reasonService"></param>
        /// <param name="resourceManager"></param>
        public ReasonManager(IReasonService reasonService, IApiResourceManager resourceManager)
        {
            _reasonService = reasonService;
            _resourceManager = resourceManager;
        }

        /// <summary>
        /// Method to get reasons
        /// </summary>
        /// <param name="reasonType">Reason type</param>
        /// <returns>List of reasons</returns>
        public List<Return_Reason> GetReasons(ReasonType reasonType)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,ReasonManager,GetReasons,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var type = (char)reasonType;
            var reasons = new List<Return_Reason>();
            var store = CacheManager.GetStoreInfo();
            var offSet = store?.OffSet ?? 0;
            if (type != '\0')
            {
               reasons = _reasonService.GetReasons(type);
            }
            if (reasons == null || reasons.Count == 0)
            {
                reasons = new List<Return_Reason>
                {
                    new Return_Reason
                    {
                        Description = _resourceManager.GetResString(offSet,207), //"<No reasons defined>"
                        Reason = "0"
                    }
                };
            }
            Performancelog.Debug($"End,ReasonManager,GetReasons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return reasons;
        }

        /// <summary>
        /// Method to get reason type name
        /// </summary>
        /// <param name="reasonType">Reason name</param>
        /// <returns>Reason description</returns>
        public string GetReasonType(ReasonType reasonType)
        {
            var type = (char)reasonType;
            return type != '\0' ? _reasonService.GetReasonType(type) : string.Empty;
        }
    }
}

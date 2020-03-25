using System;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.BusinessLayer.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IOverLimitManager
    {
        /// <summary>
        /// Method to save overrlimit
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="cobReason">Reason</param>
        /// <param name="txtExplanation">Explaination</param>
        /// <param name="txtLocation">Loaction</param>
        /// <param name="dtpDate">Date</param>
        /// <param name="error">Error</param>
        /// <returns>Aite response</returns>
        AiteCardResponse DoneOverLimit(int tillNumber, int saleNumber, string userCode, 
            string cobReason, string txtExplanation, string txtLocation, DateTime dtpDate,
            out ErrorMessage error);

        /// <summary>
        /// Method to get overlimit details
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="error">Error message</param>
        /// <returns>Over limit response</returns>
        OverLimitResponse GetOverLimitDetails(int tillNumber, int saleNumber,
            out ErrorMessage error);
    }
}
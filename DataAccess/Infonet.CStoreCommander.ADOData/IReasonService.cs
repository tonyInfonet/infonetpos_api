using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IReasonService
    {
        #region CSCMaster

        /// <summary>
        /// Method to get list of reasons
        /// </summary>
        /// <param name="type">Reason type</param>
        /// <returns>List of reasons</returns>
        List<Return_Reason> GetReasons(char type);

        /// <summary>
        /// Method to get reason type name
        /// </summary>
        /// <param name="type">Reason type</param>
        /// <returns>List of reasons</returns>
        string GetReasonType(char type);

        /// <summary>
        /// Method to get description according to code and reason type
        /// </summary>
        /// <param name="code">Reason code</param>
        /// <param name="type">Reason type</param>
        /// <returns>Description</returns>
        string GetDescription(string code, char type);

        /// <summary>
        /// Get return reason
        /// </summary>
        /// <param name="code">Reason code</param>
        /// <param name="type">Reason type</param>
        /// <returns>Return reason</returns>
        Return_Reason GetReturnReason(string code, char type);


        /// <summary>
        /// Get Tax Reasons
        /// </summary>
        /// <param name="reasonType">Reason type</param>
        /// <returns>Tax exemption reasons</returns>
        List<TaxExemptReason> GetTaxExemptReasons(string reasonType);

        #endregion
    }
}
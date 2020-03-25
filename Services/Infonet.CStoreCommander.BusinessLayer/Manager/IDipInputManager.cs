using System.Collections.Generic;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IDipInputManager
    {
        /// <summary>
        /// Get Dip Input Values
        /// </summary>
        /// <returns>List of dip inputs</returns>
        List<DipInput> GetDipInputValues();


        /// <summary>
        /// Save Dip Inputs
        /// </summary>
        /// <param name="dipInputs">Dip inputs</param>
        /// <param name="error">Error</param>
        /// <returns>Lis of dip imputs</returns>
        List<DipInput> SaveDipInputs(List<DipInput> dipInputs, out ErrorMessage error);

        /// <summary>
        /// Get Report of the Dip input values for print
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <returns>Dip input report</returns>
        Report PrintDipReport(int tillNumber, int shiftNumber, int registerNumber, string userCode,
            out ErrorMessage error);
    }
}
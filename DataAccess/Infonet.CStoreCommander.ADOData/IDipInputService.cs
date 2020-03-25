using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IDipInputService
    {
        /// <summary>
        /// Get DipInput Values
        /// </summary>
        /// <returns></returns>
        List<DipInput> GetDipInputValues();

        /// <summary>
        /// Get Maximum Dip value for tank
        /// </summary>
        /// <param name="tankno">Tank number</param>
        /// <returns>Maximum dip input</returns>
        float MaximumDip(byte tankno);

        /// <summary>
        /// Save Dip Inputs
        /// </summary>
        /// <param name="dipInputs">Dip inputs</param>
        void SaveDipInputs(List<DipInput> dipInputs);


        /// <summary>
        /// Gets Dip Inputs for Reports
        /// </summary>
        /// <returns>List of dip inputs</returns>
        List<DipInput> GetDipInputsForReport(string strDate);

        /// <summary>
        /// Checks whether Tank and Grade Exists
        /// </summary>
        /// <param name="tankno">Tank Number</param>
        /// <param name="gradeId">Grade Id</param>
        /// <returns>True or false</returns>
        bool IsTankExists(byte tankno, int gradeId);
    }
}
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IPropaneManager
    {
        /// <summary>
        /// Method to add propane sale
        /// </summary>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="isAmount">Is amount</param>
        /// <param name="propaneValue">Propane value</param>
        /// <param name="error">Error message</param>
        /// <returns>Sale</returns>
        Sale AddPropaneSale(int gradeId, int pumpId, int saleNumber, int tillNumber, byte registerNumber, bool isAmount, decimal propaneValue, out ErrorMessage error);

        /// <summary>
        /// Method to get propane grades
        /// </summary>
        /// <param name="error">Error message</param>
        /// <returns></returns>
        List<PropaneGrade> GetPropaneGrades(out ErrorMessage error);

        /// <summary>
        /// Method to get propane pumps by grade Id
        /// </summary>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="error">Error message</param>
        /// <returns>Propane pumps</returns>
        List<PropanePump> GetPropanePumpsByGradeId(int gradeId, out ErrorMessage error);

        /// <summary>
        /// Method to get volume value
        /// </summary>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="propaneValue">Propane value</param>
        /// <param name="error">Error message</param>
        /// <returns>Volume value</returns>
        string GetVolumeValue(int gradeId, int pumpId, int saleNumber, int tillNumber, byte registerNumber, decimal propaneValue, out ErrorMessage error);

    }
}
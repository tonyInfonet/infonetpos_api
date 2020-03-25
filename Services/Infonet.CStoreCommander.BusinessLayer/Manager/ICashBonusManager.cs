using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;
using System.IO;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
  public interface ICashBonusManager
    {

        /// <summary>
        /// Method to get cash bonus draw buttons
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// </summary>
        /// <returns>Cash Bonus draw buttons</returns>
        CashBonusDrawButton GetCashBonusDrawButtons(string userCode, out ErrorMessage error);

        /// <summary>
        /// method to calculate the cashbonus
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="SaleLitre"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        double CalculateCashBonus(string GroupID, float SaleLitre, out ErrorMessage error);

        /// <summary>
        /// Method to complete cash bonus draw
        /// </summary>
        /// <param name="cashBonusDraw"></param>
        /// <param name="userCode"></param>
        /// <param name="copies"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
    //    FileStream CompleteCashDraw(CashBonusDrawButton cashBonusDraw, string userCode, out int copies,
            //out ErrorMessage errorMessage)


    }
}

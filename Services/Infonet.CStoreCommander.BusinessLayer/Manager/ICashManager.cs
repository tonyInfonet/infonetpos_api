using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;
using System.IO;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ICashManager
    {
        /// <summary>
        /// Method to get cash draw buttons
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// </summary>
        /// <returns>Cash draw buttons</returns>
        CashDrawButton GetCashDrawButtons(string userCode, out ErrorMessage error);


        /// <summary>
        /// Method to get cash bonus draw buttons
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// </summary>
        /// <returns>Cash Bonus draw buttons</returns>
   //     CashBonusDrawButton GetCashBonusDrawButtons(string userCode, out ErrorMessage error);



        /// <summary>
        /// Print cash draw 
        /// </summary>
        /// <param name="cashDraw">Cash draw</param>
        /// <param name="userCode"> User</param>
        /// <param name="copies">Copies</param>
        /// <param name="errorMessage"> Error</param>
        /// <returns></returns>
        FileStream CompleteCashDraw(CashDrawButton cashDraw, string userCode, out int copies,
            out ErrorMessage errorMessage);

        /// <summary>
        /// Method to get cash buttons
        /// </summary>
        /// <returns>List of cash buttons</returns>
        List<CashButton> GetCashButtons();

        /// <summary>
        /// Method to complete cash drop
        /// </summary>
        /// <param name="selectedTenders">Selected tenders</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="reason">Reason code</param>
        /// <param name="envelopeNumber">Envelope number</param>
        /// <param name="copies">Copies</param>
        /// <param name="error">Error</param>
        /// <returns>Stream</returns>
        FileStream CompleteCashDrop(List<Tender> selectedTenders, int tillNumber, string userCode,
         byte registerNumber, string reason, string envelopeNumber,out int copies, out ErrorMessage error);

        /// <summary>
        /// Method to update the tenders selected
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="reason">Reason code</param>
        /// <param name="errorMessage">Error Message</param>
        /// <returns></returns>
        Tenders UpdateCashDropTendered(List<Tender> tenders, string reason, int saleNumber,
           int tillNumber, string userCode,out ErrorMessage errorMessage);

        /// <summary>
        /// Method to save the cash drawer reason
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="reasonCode">Reason code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="error">Error message</param>
        void OpenCashDrawer(string userCode, string reasonCode, int tillNumber, out ErrorMessage error);

    }
}

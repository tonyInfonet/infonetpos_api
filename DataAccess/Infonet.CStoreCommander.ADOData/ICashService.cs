using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public interface ICashService
    {
        /// <summary>
        /// Method to get all coins
        /// </summary>
        /// <returns></returns>
        List<Cash> GetCoins();


        ///<summary>
        /// Method to get all cash bonus coins
        ///</summary>
        ///<returns></returns>
   //    List<Cash> GetCashBonusCoins();

        /// <summary>
        /// Method to get all bills
        /// </summary>
        /// <returns></returns>
        List<Cash> GetBills();

        /// <summary>
        /// Method to add cash draw
        /// </summary>
        /// <param name="cashDraw">Cash draw</param>
        void AddCashDraw(CashDraw cashDraw);

        /// <summary>
        /// Method to get list of cash buttons
        /// </summary>
        /// <returns>List of cash buttons</returns>
        List<CashButton> GetCashButtons();

        ///<summary>
        /// Method to add  drop header
        /// </summary>
        /// <param name="dropHeader">Drop header</param>
        void AddDropHeader(DropHeader dropHeader);

        /// <summary>
        /// Method to add drop line
        /// </summary>
        /// <param name="dropLines">Drop line</param>
        void AddDropLine(DropLine dropLines);

        /// <summary>
        /// Method to get maximum cash drop from drop header
        /// </summary>
        /// <returns>Cash drop</returns>
        short GetMaxCashDrop(int tillNumber, DateTime shiftDate, int shiftNumber);

        /// <summary>
        /// Method to get max drop id
        /// <param name="dataSource">Data source</param>
        /// </summary>
        /// <returns></returns>
        int GetMaxDropId(DataSource dataSource);


        /// <summary>
        /// Method to save cash draw
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="reasonCode">Reason Code</param>
        void SaveCashDraw(int tillNumber, string userCode, string reasonCode);
    }
}

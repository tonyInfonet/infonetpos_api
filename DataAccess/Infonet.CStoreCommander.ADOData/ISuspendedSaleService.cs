using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public  interface ISuspendedSaleService
    {
        /// <summary>
        /// Get All Suspended Sales
        /// </summary>
        /// <param name="sqlQuery">Query</param>
        /// <returns>List of suspended sale head</returns>
        List<SusHead> GetAllSuspendedSale(string sqlQuery);

        /// <summary>
        /// Suspend Sale
        /// </summary>
        /// <param name="saleType">Sale type</param>
        /// <param name="shareSusp">Share suspended sale</param>
        /// <param name="sale">Sale</param>
        /// <param name="userCode">User code</param>
        void SuspendSale(string saleType, bool shareSusp, Sale sale, string userCode);

        /// <summary>
        /// Update Card Sale
        /// </summary>
        /// <param name="shareSusp">Share suspended sale</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        void UpdateCardSaleForUnSuspend(bool shareSusp, int tillNumber, int saleNumber);

        /// <summary>
        /// Get Suspended Sales
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="shareSusp">Share suspended sale</param>
        /// <returns>Sale</returns>
        Sale GetSuspendedSale(int tillNumber, int saleNumber, bool shareSusp);

        /// <summary>
        /// Delete unsuspend
        /// </summary>
        /// <param name="shareSusp">Share supended sale</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        void DeleteUnsuspend(bool shareSusp, int tillNumber, int saleNumber);

        /// <summary>
        /// Delete Card Sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        void DeleteCardSaleFromDbTemp(int saleNumber);

        /// <summary>
        /// Remove Previous Transactions
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>True or false</returns>
        bool RemovePreviousTransactionFromDbTemp(int tillNumber);
    }
}

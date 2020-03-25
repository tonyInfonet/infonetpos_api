using System.Collections.Generic;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ISuspendedSaleManger
    {
        /// <summary>
        /// Get Suspended Sale List
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>List of suspended sales</returns>
        List<SusHead> GetSuspendedSale(int tillNumber, out ErrorMessage errorMessage);

        /// <summary>
        /// Suspend Sale 
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Sale</returns>
        Sale SuspendSale(int tillNumber, int saleNumber,string userCode, out ErrorMessage 
            errorMessage);


        /// <summary>
        /// Unsuspend Sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Sale</returns>
        Sale UnsuspendSale(int saleNumber, int tillNumber,string userCode, out ErrorMessage errorMessage);

        /// <summary>
        /// Void Sale
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="voidReason">Void reason</param>
        /// <param name="errorMessage">Error</param>
        /// <param name="fs">File stream</param>
        /// <returns>Sale</returns>
        Sale VoidSale(string userCode, int saleNumber, int tillNumber, string voidReason,
            out ErrorMessage errorMessage, out Report fs);


        /// <summary>
        /// Method to writeoff a sale
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">TillNumber</param>
        /// <param name="writeOffReason"></param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="registerNumber">Register number</param>
        /// <returns>New sale</returns>
        Report WriteOff(string userCode, int saleNumber, int tillNumber, string writeOffReason,
            out ErrorMessage errorMessage, out int registerNumber);

        /// <summary>
        /// Verify Void sale
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>True or false</returns>
        bool VerifyVoidSale(string userCode, int saleNumber, int tillNumber, out ErrorMessage errorMessage);

    }
}

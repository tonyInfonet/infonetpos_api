using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IMaintenanceManager
    {
        /// <summary>
        /// Method to close batch
        /// </summary>
        /// <param name="posId">Pos Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="error">Error message</param>
        /// <returns>List of report</returns>
       List<Report> CloseBatch(byte posId, int tillNumber, int saleNumber, byte registerNumber,
           out ErrorMessage error);

        /// <summary>
        /// Method to initiaise close batch
        /// </summary>
        /// <param name="error">Error message</param>
        /// <returns></returns>
        bool Initialize(out ErrorMessage error);

        /// <summary>
        /// Method to update postpay
        /// </summary>
        /// <param name="newStatus">New state</param>
        /// <param name="error"></param>
        void UpdatePostPay(bool newStatus,out ErrorMessage error);

        /// <summary>
        /// Method to update prepay
        /// </summary>
        /// <param name="newStatus">New status</param>
        /// <param name="error"></param>
        void UpdatePrepay(bool newStatus, out ErrorMessage error);
    }
}

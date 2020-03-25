using System;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ITillCloseManager
    {
        /// <summary>
        /// Method to close till
        /// </summary>
        /// <param name="tillClose">till close</param>
        /// <param name="till">Till</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="shiftDate">Shift date</param>
        /// <param name="allShifts">All shifts or not</param>
        void Close_Till(ref Till_Close tillClose, Till till, string whereClause, DateTime shiftDate, 
            bool allShifts);

        /// <summary>
        /// Method to validate till close
        /// </summary>
        /// <param name="tillNumber">Till close</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <returns></returns>
        TillCloseResponse ValidateTillClose(int tillNumber, int saleNumber,string userCode, out ErrorMessage error);

        /// <summary>
        /// Method to save sale and close till
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <returns>Close current till response</returns>
        CloseCurrentTillResponseModel TillClose(int tillNumber,int saleNumber, string userCode, out ErrorMessage error);

        /// <summary>
        /// Method to end a sale session
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        bool EndSale(int tillNumber, int saleNumber,string userCode, out ErrorMessage error);

        /// <summary>
        /// Method to read tank dip
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        bool ReadTankDip(int tillNumber, out ErrorMessage error);

        /// <summary>
        /// Method to update till close
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderName">Tender name</param>
        /// <param name="entered">Entered amount</param>
        /// <param name="description">Bill coin</param>
        /// <param name="amount">Amount</param>
        /// <param name="error">Error message</param>
        /// <returns>Close current till response</returns>
        CloseCurrentTillResponseModel UpdateTillClose(int tillNumber, string tenderName, decimal entered, string description,
            decimal amount, out ErrorMessage error);

        /// <summary>
        /// Method to finish till close and print till close report
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="readTankDipSuccess">Read tank dip response</param>
        /// <param name="readTotalizerSuccess">Read totaliser response</param>
        /// <param name="customerDisplay"></param>
        /// <param name="error">Error message</param>
        /// <returns>Till close report</returns>
        List<Report> FinishTillClose(int tillNumber, string userCode, int registerNumber, bool? readTankDipSuccess,
            bool? readTotalizerSuccess,out CustomerDisplay customerDisplay, out ErrorMessage error);

        /// <summary>
        /// Method to print eod details 
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="cbDate">Close batch date</param>
        /// <param name="cbTime">Close batch time</param>
        /// <param name="crTermId">Terminal Id</param>
        /// <param name="crBatchNum">Batch number</param>
        /// <param name="dbTermId">Terminal Id</param>
        /// <param name="dbBatchNum">Batch number</param>
        /// <param name="reprint">Reprint</param>
        /// <param name="batchDate">Batch date</param>
        /// <returns>Report</returns>
        Report PrintEodDetails(int tillNumber, ref DateTime cbDate, ref DateTime cbTime, string crTermId,
           string crBatchNum, string dbTermId, string dbBatchNum, bool reprint,
           DateTime batchDate);
    }
}
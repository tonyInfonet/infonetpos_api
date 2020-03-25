using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IReturnSaleManager
    {
        /// <summary>
        /// Get all Sales
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        List<SaleHead> GetAllSales(int pageIndex, int pageSize);

        /// <summary>
        /// Get Sale By sale No
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="message">Message</param>
        /// <returns>Sale</returns>
        Sale GetSale(int saleNumber, int tillNumber, out ErrorMessage message);

        /// <summary>
        /// Search Sales
        /// </summary>
        /// <param name="saleDate">Sale date</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="error">Error</param>
        /// <returns>List of sale head</returns>
        List<SaleHead> SearchSale(DateTime? saleDate, int? saleNumber, int pageIndex, int pageSize,
            out ErrorMessage error);

        /// <summary>
        /// Return Sale
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="loggedInTillNumber">Logged till number</param>
        /// <param name="correction">Correction sale or not</param>
        /// <param name="reasonCode">Reason code</param>
        /// <param name="message">Error</param>
        /// <param name="reasonType">Reason type</param>
        /// <param name="saleLineMessages">Sale line messages</param>
        /// <param name="report">Report</param>
        /// <param name="fileName">Filename</param>
        /// <returns>Sale</returns>
        Sale ReturnSale(User user, int saleNumber, int tillNumber, int loggedInTillNumber,
            bool correction, string reasonType, string reasonCode, out ErrorMessage message, 
            out List<ErrorMessage> saleLineMessages, out Report report, out string fileName);

        /// <summary>
        /// Return Sale Items
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="loggedInTillNumber">Logged till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="saleLines">Sale lines</param>
        /// <param name="isCorrection">Correction sale or not</param>
        /// <param name="user">User</param>
        /// <param name="reasonCode">Reason code</param>
        /// <param name="message">Error</param>
        /// <param name="saleLineMessages">Sale line messages</param>
        /// <param name="reasonType">Reason type</param>
        /// <returns>Sale</returns>
        Sale ReturnSaleItems(User user, int tillNumber, int loggedInTillNumber, int saleNumber, 
            int[] saleLines, bool isCorrection, string reasonType, string reasonCode,
            out ErrorMessage message, out List<ErrorMessage> saleLineMessages);


        /// <summary>
        /// Checks Whether Correction is allowed
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>True or false</returns>
        bool IsAllowCorrection(int saleNumber);


        /// <summary>
        /// Checks whether reason allowed
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>True or false</returns>
        bool IsReasonAllowed(int saleNumber);
    }
}

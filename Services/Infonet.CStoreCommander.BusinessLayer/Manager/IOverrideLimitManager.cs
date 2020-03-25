using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IOverrideLimitManager
    {
        bool IsSuccess { get; }

        /// <summary>
        /// Method to check override
        /// </summary>
        /// <returns>True or false</returns>
        bool CheckOverride();

        /// <summary>
        /// Method to complete override limit
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <returns>Sale summary response</returns>
        SaleSummaryResponse CompleteOverrideLimit(int tillNumber, int saleNumber,
            byte registerNumber, string userCode, out ErrorMessage error);

        /// <summary>
        /// Method to validate override limit
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="purchaseItemNo">Purchase item info</param>
        /// <param name="documentNo">Document number</param>
        /// <param name="overRideCode">Override code</param>
        /// <param name="documentDetail">Document detail</param>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        bool DoneOverRideLimit(int saleNumber, int tillNumber, string userCode, short purchaseItemNo, string documentNo, string overRideCode, string documentDetail, out ErrorMessage error);

        /// <summary>
        /// Method to load override limit details
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>Override limit repsonse</returns>
        OverrideLimitResponse LoadOverrideLimitDetails(int saleNumber, int tillNumber, string userCode,string treatyNumber, string treatyName,
            out ErrorMessage error);
        
        /// <summary>
        /// Method to load purchase items
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="pepurchaseList">Purchase list</param>
        /// <param name="limitThreshold">Threshold limit</param>
        /// <param name="limitMax">Maximum limit</param>
        /// <returns>List of purchase item response</returns>
        List<PurchaseItemResponse> LoadPurchaseItems(int tillNumber, int saleNumber, ref tePurchaseList pepurchaseList, out double limitThreshold, out double limitMax);
    }
}
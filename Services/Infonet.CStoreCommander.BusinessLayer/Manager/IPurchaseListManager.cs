using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IPurchaseListManager
    {
        /// <summary>
        /// Add Item to Purchase List
        /// </summary>
        /// <param name="purchaseList">Purchase list</param>
        /// <param name="sale">Sale</param>
        /// <param name="peTreatyNo">Treaty number</param>
        /// <param name="sProductKey">Product key</param>
        /// <param name="dQuantity">Quantity</param>
        /// <param name="dOriginalPrice">Original price</param>
        /// <param name="iRowNumberInSalesMainForm">Row number</param>
        /// <param name="stockcode">Stock code</param>
        /// <param name="taxInclPrice">Tax included price</param>
        /// <param name="isFuelItem">Is fuel item or not</param>
        /// <returns>True or false</returns>
        bool AddItem(ref tePurchaseList purchaseList, ref Sale sale, ref teTreatyNo peTreatyNo, 
            ref string sProductKey, ref double dQuantity, ref double dOriginalPrice, 
            ref short iRowNumberInSalesMainForm, ref string stockcode, ref double taxInclPrice,
            ref bool isFuelItem);

        /// <summary>
        /// Method to save and assign to quotas
        /// </summary>
        /// <param name="purchaseList">Purchase list</param>
        /// <param name="user">User</param>
        /// <param name="till">Till</param>
        /// <param name="saveToDb">Save to db</param>
        /// <returns>True or false</returns>
        bool SaveAndAssignToQuotas(ref tePurchaseList purchaseList, User user, Till till, bool saveToDb = true);

        /// <summary>
        /// method to generate the purchase list
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="oTreatyNo"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        tePurchaseList GetPurchaseList(int saleNumber,int tillNumber, string userCode, string treatyNumber, string treatyName, out ErrorMessage error);
    }
}
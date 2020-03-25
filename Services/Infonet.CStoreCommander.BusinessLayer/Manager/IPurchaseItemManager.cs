using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IPurchaseItemManager
    {
        /// <summary>
        /// Method to initiliase purchase item
        /// </summary>
        /// <param name="purchaseItem">Purchase item</param>
        /// <param name="oTreatyNo">Treaty number</param>
        /// <param name="sProductKey">Product key</param>
        /// <param name="dOriginalPrice">Original price</param>
        /// <param name="dQuantity">Orginal quantity</param>
        /// <param name="iRowNumberInSalesMainForm">Row number is sale line</param>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="tillId">Till Id</param>
        /// <param name="stockcode">Stock code</param>
        /// <param name="taxInclPrice">Tax included price</param>
        /// <param name="isFuelItem">Is fuel item or not</param>
        /// <param name="noRtvp">RTVP or not</param>
        /// <returns>True or false</returns>
        bool Init(ref tePurchaseItem purchaseItem, ref teTreatyNo oTreatyNo, ref string sProductKey,
            double dOriginalPrice, double dQuantity, short iRowNumberInSalesMainForm,
            int invoiceId, short tillId, ref string stockcode, double taxInclPrice, 
            bool isFuelItem, bool noRtvp);

        /// <summary>
        /// Method to update quantity in db
        /// </summary>
        /// <param name="purchaseItem">Purchase item</param>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="lineNum">Line number</param>
        /// <param name="reducedQuantity">Reduced quantity</param>
        /// <param name="treatyNo">Treaty number</param>
        /// <param name="stock">Stock code</param>
        /// <param name="tePrice">Tax exemot price</param>
        /// <param name="orgPrice">Original price</param>
        void UpdateQuantityInDb(tePurchaseItem purchaseItem, int invoiceId,
           short lineNum, float reducedQuantity, string treatyNo, string stock = "",
           float tePrice = 0, float orgPrice = 0);

        /// <summary>
        /// Method to save purchase item
        /// </summary>
        /// <param name="purchaseItem">Purchase item</param>
        /// <param name="user">User</param>
        /// <param name="till">Till number</param>
        /// <returns>True or false</returns>
        bool Save(tePurchaseItem purchaseItem, User user, Till till);
    }
}
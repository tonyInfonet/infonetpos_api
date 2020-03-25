using System.Collections.Generic;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.BusinessLayer.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    /// <summary>
    /// Interface to perform stock related operations 
    /// </summary>

    public interface IStockManager
    {
        /// <summary>
        /// Method to get list of stock items using pagination
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of stock items</returns>
        List<StockItem> GetStockItems(int pageIndex = 1, int pageSize = 100);

        /// <summary>
        /// Method to get stock item using a stock term
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of stock items</returns>
        List<StockItem> SearchStockItems(string searchTerm, int pageIndex = 1, int pageSize = 100);

        /// <summary>
        /// Method to get stock item using stock code
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="error">Error message</param>
        /// <returns>Stock item</returns>
        StockItem GetStockByCode(string stockCode, out ErrorMessage error);

        /// <summary>
        /// Method to add a stock Item
        /// </summary>
        /// <param name="userName">UserCode</param>
        /// <param name="stockItem">Stock Item</param>
        /// <param name="taxCodes">List of tax codes</param>
        /// <param name="error">Error message</param>
        void AddStockItem(string userName, StockItem stockItem, List<string> taxCodes, out ErrorMessage error);

        /// <summary>
        /// Gets the List of Hot button Pages
        /// </summary>
        /// <returns></returns>
        Dictionary<int, string> GetHotButonPages();

        /// <summary>
        /// Get HotButtons
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        List<HotButton> GetHotButons(int pageId);

        /// <summary>
        /// Method to set vendor coupon
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="code">Vendor code</param>
        void SetVendorCoupon(ref Vendor vendor, string code);

        /// <summary>
        /// Method to get stock details by stock code
        /// </summary>
        /// <param name="code">Stock code</param>
        /// <returns>Stock</returns>
        Stock GetStockDetails(string code);
               

    }
}

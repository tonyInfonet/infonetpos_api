using System;
using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// Interface for stock related database operations
    /// </summary>
    public interface IStockService
    {
        /// <summary>
        /// Method to check whether stock is available for a day
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>True or false</returns>
        List<string> IsStockByDayAvailable(string stockCode);

        /// <summary>
        /// Method to get plu mast by plu code
        /// </summary>
        /// <param name="pluCode">Plu code</param>
        /// <returns>Plu mast</returns>
        PLUMast GetPluMast(string pluCode);

        /// <summary>
        /// Method to get stock item by code
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Stock Item</returns>
        StockItem GetStockItem(string stockCode);

        /// <summary>
        /// Method to get stock rebate
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="code">Stock code</param>
        /// <returns>Stock rebate amount</returns>
        decimal GetStockRebate(string vendor, string code);

        /// <summary>
        /// Method to get stock price
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Stock price</returns>
        //double? GetStockPrice(string vendor, string stockCode);
        List<StockItem> GetStockPricesByCode(string stockCode);

        /// <summary>
        /// Method to get priceL
        /// <param name="stockCode">Stock code</param>
        /// </summary>
        List<PriceL> GetPriceLByCode(string stockCode);

        /// <summary>
        /// Method to get pricel for any range
        /// </summary>
        /// <param name="code">Stock code</param>
        /// <returns>PriceL</returns>
        List<PriceL> GetPriceLForRange(string code);

        /// <summary>
        /// Method find if there is check group button
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Truie or false</returns>
        bool CheckGroupButton(string stockCode);

        /// <summary>
        /// Method to get product tax exempt
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>product tax exempt</returns>
        ProductTaxExempt GetProductTaxExempt(string stockCode);

        /// <summary>
        /// Method to get stock branch
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Stock Br</returns>
        StockBr GetStockBr(string stockCode);

        /// <summary>
        /// Method to get list of stock taxes
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>List of stock taxes</returns>
        List<StockTax> GetStockTaxes(string stockCode);

        /// <summary>
        /// Method to get tax rate
        /// </summary>
        /// <param name="taxName">Tax name</param>
        /// <param name="taxCode">Tax code</param>
        /// <returns>Tax rate</returns>
        TaxRate GetTaxRate(string taxName, string taxCode);

        /// <summary>
        /// Method to get tax mast
        /// </summary>
        /// <param name="taxName">Tax name</param>
        /// <returns>Tax mast</returns>
        TaxMast GetTaxMast(string taxName);

        /// <summary>
        /// Method to check whether kit is present
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>True or false</returns>
        bool IsKitPresent(string stockCode);

        /// <summary>
        /// Method to get kit items
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>List of kit items</returns>
        List<KitItem> GetKitIems(string stockCode);

        /// <summary>
        /// Method to get associate charges
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>List of associate charges</returns>
        List<AssociateCharge> GetAssociateCharges(string stockCode);

        /// <summary>
        /// Method to get sale tax
        /// </summary>
        /// <param name="asCode">As code</param>
        /// <returns>List of taxes</returns>
        List<Sale_Tax> GetTax(string asCode);

     
        /// <summary>
        /// Method to get stock items
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of stock item</returns>
        List<StockItem> GetStockItems(int pageIndex, int pageSize);

        /// <summary>
        /// Method to get only active stock items
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of stock items</returns>
        List<StockItem> GetActiveStockItems(int pageIndex, int pageSize);

        /// <summary>
        /// Method to search stock
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="sellInactive">If sell inactive or not</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of stock item</returns>
        List<StockItem> SearchStock(string searchTerm, bool sellInactive, int pageIndex, int pageSize);

        /// <summary>
        /// Method to get stock item by code
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="sellInactive">Sell inactive or not</param>
        /// <returns>List of stock items</returns>
        StockItem GetStockItemByCode(string stockCode, bool sellInactive);

        /// <summary>
        /// Method to add stock item
        /// </summary>
        /// <param name="stockItem">Stock item</param>
        /// <param name="loyalty">Loyalty or not</param>
        void AddStockItem(StockItem stockItem, bool loyalty);

        /// <summary>
        /// Method to add plu mast
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        void AddPluMast(string stockCode);

        /// <summary>
        /// Method to add stock price
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="price">Stock price</param>
        void AddStockPrice(string stockCode, decimal price);

        /// <summary>
        /// Method to add stock branch
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        void AddStockBranch(string stockCode);

        /// <summary>
        /// Gets the HotButton Pages list
        /// </summary>
        /// <returns></returns>
        Dictionary<int, string> GetHotButonPages();

        /// <summary>
        /// Get Hot Buttons
        /// </summary>
        /// <param name="firstIndex"></param>
        /// <param name="lastIndex"></param>
        /// <returns></returns>
        List<HotButton> GetHotButtons(int firstIndex, int lastIndex);

        /// <summary>
        /// Method to get stock price
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="priceNumber">Price number</param>
        /// <param name="whereClause"></param>
        /// <returns>Stock price</returns>
        double? GetStockPriceForPriceNumber(string stockCode, short priceNumber, string whereClause);

        /// <summary>
        /// Method to get group price head
        /// </summary>
        /// <param name="department">Department</param>
        /// <param name="subDepartment">Sub department</param>
        /// <param name="subDetail">Sub detail</param>
        /// <returns>Group price head</returns>
        GroupPriceHead GetGroupPriceHead(string department, string subDepartment, string subDetail);

        /// <summary>
        /// Method to get group price lines
        /// </summary>
        /// <param name="department">Department</param>
        /// <param name="subDepartment">Sub department</param>
        /// <param name="subDetail">Sub detail</param>
        /// <returns>Group price lines</returns>
        List<GroupPriceLine> GetGroupPriceLines(string department, string subDepartment, string subDetail);


        /// <summary>
        /// Method to get the maximum gift certificate number
        /// </summary>
        /// <returns>Gift number</returns>
        int GetMaximumGiftNumber();

        /// <summary>
        /// Method to get kit description
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns></returns>
        string GetKitDescription(string stockCode);

        /// <summary>
        /// Get Product Tax Exempt by Code
        /// </summary>
        /// <param name="stockCode"></param>
        /// <returns></returns>
        ProductTaxExempt GetProductTaxExemptByProductCode(string stockCode);

        /// <summary>
        /// Is Active vendor
        /// </summary>
        /// <param name="stockCode"></param>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        bool IsActiveVendorPrice(string stockCode, string vendorId);

        /// <summary>
        /// Add Update Regular Price
        /// </summary>
        /// <param name="stockCode"></param>
        /// <param name="vendorId"></param>
        /// <param name="price"></param>
        /// <param name="regularPrice"></param>
        void AddUpdateRegularPrice(string stockCode, ref string vendorId, ref double price, double regularPrice);

        /// <summary>
        /// Add or Update Special Price
        /// </summary>
        /// <param name="stockCode"></param>
        /// <param name="activeVendorPrice"></param>
        /// <param name="vendorId"></param>
        /// <param name="priceType"></param>
        /// <param name="gridPrices"></param>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <param name="perDollarChecked"></param>
        /// <param name="isEndDate"></param>
        void AddUpdateSpecialPrice(string stockCode, bool activeVendorPrice, ref string vendorId, string priceType,
            List<PriceGrid> gridPrices, DateTime fromdate, DateTime todate, bool perDollarChecked, bool isEndDate);

        /// <summary>
        /// Track Price Change
        /// </summary>
        /// <param name="stockCode"></param>
        /// <param name="origPrice"></param>
        /// <param name="newPrice"></param>
        /// <param name="processType"></param>
        /// <param name="pricenum"></param>
        /// <param name="userCode"></param>
        /// <param name="vendorId"></param>
        void TrackPriceChange(string stockCode, double origPrice, double newPrice, string processType, byte pricenum,
            string userCode, string vendorId = "");



        /// <summary>
        /// delete previous prices
        /// </summary>
        /// <param name="stockCode"></param>
        /// <param name="priceType"></param>
        /// <param name="activeVendorPrice"></param>
        /// <param name="vendorId"></param>
        void DeletePreviousPrices(string stockCode, string priceType, bool activeVendorPrice, string vendorId);

        /// <summary>
        /// Method to get list of vendor coupon
        /// </summary>
        /// <returns>List of vendor coupons</returns>
        List<VendorCoupon> GetVendorCoupons();

        /// <summary>
        /// Method to get vendor by code
        /// </summary>
        /// <param name="code">Vendor code</param>
        /// <returns>Vendor</returns>
        Vendor GetVendorByCode(string code);


        /// <summary>
        /// Method to get all stock list
        /// </summary>
        /// <returns>List of stock</returns>
        List<StockBr> GetAllStockBr();

        /// <summary>
        /// Method to get stock by stock code
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Stock information</returns>
        StockInfo GetStockByStockCode(string stockCode);

        /// <summary>
        /// Method to get saleline by stock code
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Sale line</returns>
        Sale_Line GetStockInfoByStockCode(string stockCode);

        /// <summary>
        /// Method to get list of vendors
        /// </summary>
        /// <returns>List of vendors</returns>
        List<Vendor> GetAllVendors();

        /// <summary>
        /// Method to get sale line info
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns></returns>
        Sale_Line GetSaleLineInfo(string stockCode);
    }
}

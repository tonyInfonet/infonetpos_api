using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IPriceCheckManager
    {
        /// <summary>
        /// Get Stock Price details
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <returns>Stock price</returns>
        StockPriceCheck GetStockPriceDetails(string stockCode, int tillNumber,int saleNumber, 
            byte registerNumber, string userCode, out ErrorMessage error);

        /// <summary>
        /// Apply Regular Price
        /// </summary>
        /// <param name="regularPrice">Regular price</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>Stock price check</returns>
        StockPriceCheck ApplyRegularPrice(RegularPriceCheck regularPrice, string userCode,
            out ErrorMessage error);

        /// <summary>
        /// Apply Special Price
        /// </summary>
        /// <param name="priceCheck">Price check</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>Stock price check</returns>
        StockPriceCheck ApplySpecialPrice(SpecialPriceCheck priceCheck, string userCode,
            out ErrorMessage error);
    }
}
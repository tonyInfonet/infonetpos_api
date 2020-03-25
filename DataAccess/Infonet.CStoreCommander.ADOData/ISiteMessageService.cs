namespace Infonet.CStoreCommander.ADOData
{
    public interface ISiteMessageService
    {
        /// <summary>
        /// Get SITE MESSAGE
        /// </summary>
        /// <param name="id">Message Id</param>
        /// <returns>Site message</returns>
        string GetSiteMessage(int id);

        /// <summary>
        /// Get Stock Cost
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="vendor">Vendor</param>
        /// <returns>Cost</returns>
        double? GetStockCost(string stockCode, string vendor);
    }
}
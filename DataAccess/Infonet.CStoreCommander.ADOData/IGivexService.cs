using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IGivexService
    {
        /// <summary>
        /// Save Close Batch
        /// </summary>
        /// <param name="cashoutId">CashoutId</param>
        /// <param name="reports">Reports</param>
        void SaveCloseBatch(string cashoutId, string reports);

        /// <summary>
        /// Get a Valid GiveX Stock
        /// </summary>
        /// <returns>stockCode</returns>
        string GetValidGiveXStock();

        /// <summary>
        /// Get Givex Report Details
        /// </summary>
        /// <param name="reportDate"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        List<GivexDetails> GetGivexReportDetails(DateTime reportDate, string timeFormat);
    }
}

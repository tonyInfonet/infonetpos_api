using Microsoft.VisualBasic;
using System;

namespace Infonet.CStoreCommander.ADOData
{
    public class SiteMessageService : SqlDbService, ISiteMessageService
    {

        /// <summary>
        /// Get SITE MESSAGE
        /// </summary>
        /// <param name="id">Message Id</param>
        /// <returns>Site message</returns>
        public string GetSiteMessage(int id)
        {
            var rsMsgs = GetRecords("SELECT Message FROM SITEMessages WHERE ID=" + Convert.ToString(id), DataSource.CSCMaster);
            string strMessage = string.Empty;
            if (rsMsgs != null && rsMsgs.Rows.Count != 0)
            {
                var rsFields = rsMsgs.Rows[0];
                strMessage = CommonUtility.GetStringValue(rsFields["Message"]);
                strMessage = strMessage.Replace(Strings.Chr(150), '\u002D');
            }
            return strMessage;
        }


        /// <summary>
        /// Get Stock Cost
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="vendor">Vendor</param>
        /// <returns>Cost</returns>
        public double? GetStockCost(string stockCode, string vendor)
        {
            var rs = GetRecords("SELECT Cost FROM StockCosts WHERE Stock_code = \'" + stockCode + "\' AND VendorID = \'" + vendor + "\'", DataSource.CSCMaster);
            if (rs != null && rs.Rows.Count != 0)
            {
                var rsFields = rs.Rows[0];
                double cost = Convert.ToDouble(rsFields["Cost"]);
                return cost;
            }
            return null;
        }
    }
}

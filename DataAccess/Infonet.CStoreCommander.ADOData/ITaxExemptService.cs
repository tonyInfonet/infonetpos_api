using Infonet.CStoreCommander.Entities;
using System;

namespace Infonet.CStoreCommander.ADOData
{
    public interface ITaxExemptService
    {
        /// <summary>
        /// Save Tax Exempt Sale Head
        /// </summary>
        /// <param name="oteSale"></param>
        /// <param name="shiftDate"></param>
        void SaveTaxExemptSaleHead(TaxExemptSale oteSale, DateTime shiftDate);

        /// <summary>
        /// Save Tax credit
        /// </summary>
        /// <param name="oteSale"></param>
        void SaveTaxCredit(TaxExemptSale oteSale);

        /// <summary>
        /// Get Item UPC
        /// </summary>
        /// <param name="stockCode"></param>
        /// <returns></returns>
        string GetItemUpc(string stockCode);

        /// <summary>
        /// UPdate purchase item
        /// </summary>
        /// <param name="query"></param>
        void UpdatePurchaseItem(string query);

        /// <summary>
        /// Update Treaty number
        /// </summary>
        /// <param name="query"></param>
        void UpdateTreatyNo(string query);

        /// <summary>
        /// Load Tax exempt
        /// </summary>
        /// <param name="teType"></param>
        /// <param name="sn"></param>
        /// <param name="tillId"></param>
        /// <param name="db"></param>
        /// <param name="checkQuota"></param>
        /// <returns></returns>
        TaxExemptSale LoadTaxExempt(string teType, int sn, byte tillId,
            DataSource db, bool checkQuota = true);

        /// <summary>
        /// Load GST exempt
        /// </summary>
        /// <param name="taxExemptSale"></param>
        /// <param name="sn"></param>
        /// <param name="tillId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        bool LoadGstExempt(ref TaxExemptSale taxExemptSale,
            int sn, byte tillId, DataSource db);

        /// <summary>
        /// Load GST for delete prepay
        /// </summary>
        /// <param name="taxExemptSale"></param>
        /// <param name="sn"></param>
        /// <param name="tillId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        bool LoadGstExemptForDeletePrepay(ref TaxExemptSale
            taxExemptSale, int sn, byte tillId, DataSource db);

        /// <summary>
        /// Load Tax for delete
        /// </summary>
        /// <param name="taxExemptSale"></param>
        /// <param name="teType"></param>
        /// <param name="till"></param>
        /// <param name="user"></param>
        /// <param name="sn"></param>
        /// <param name="tillId"></param>
        /// <param name="lineNum"></param>
        /// <param name="newSaleNo"></param>
        /// <returns></returns>
        bool LoadTaxExemptForDeletePrepay(ref TaxExemptSale taxExemptSale,
           string teType, Till till, User user, int sn, byte tillId, short lineNum,
           int newSaleNo);

        /// <summary>
        /// Update Sale
        /// </summary>
        /// <param name="taxExemptSale"></param>
        /// <param name="gasLessQuota"></param>
        /// <param name="propaneLessQuota"></param>
        /// <param name="tobaccoLessQuota"></param>
        void UpdateSale(TaxExemptSale taxExemptSale, float gasLessQuota, float propaneLessQuota,
            float tobaccoLessQuota);

        /// <summary>
        /// Get Purchase Item
        /// </summary>
        /// <param name="saleNo"></param>
        /// <param name="lineNo"></param>
        /// <returns></returns>
        tePurchaseItem GetPurchaseItem(int saleNo, int lineNo);
    }
}


using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ITaxExemptSaleManager
    {
        /// <summary>
        /// Method to save tax exempt sale
        /// </summary>
        /// <param name="oteSale">Tax exempt sale</param>
        void SaveSale(TaxExemptSale oteSale);

        /// <summary>
        /// Method to update tax exempt sale
        /// </summary>
        /// <param name="taxExemptSale">Tax exempt sale</param>
        void UpdateSale(ref TaxExemptSale taxExemptSale);

        /// <summary>
        /// Method to load tax exempt prepay
        /// </summary>
        /// <param name="saleNo">Sale number</param>
        /// <param name="sl">Sale line</param>
        /// <param name="lineNo">Line number</param>
        /// <param name="sale">Sale</param>
        /// <param name="oPurchaseList">Purchase list</param>
        /// <param name="oTreatyNo">Treaty number</param>
        /// <returns>True or false</returns>
        bool LoadTaxExemptPrepay(int saleNo, Sale_Line sl, short lineNo, ref Sale sale, 
            ref tePurchaseList oPurchaseList, ref teTreatyNo oTreatyNo);
    }
}

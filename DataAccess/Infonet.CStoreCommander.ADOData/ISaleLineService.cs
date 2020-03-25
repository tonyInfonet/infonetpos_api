using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public interface ISaleLineService
    {
        /// <summary>
        /// Get Sale lines 
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <returns>List of sale lines</returns>
        List<Sale_Line> GetSaleLinesFromDbTemp(int saleNumber, int tillNumber, string userCode);

        /// <summary>
        /// Method to add a sale line to current sale
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        void AddSaleLineToDbTill(Sale_Line saleLine);

        /// <summary>
        /// Method to add a sale line reason
        /// </summary>
        /// <param name="reason">Sale line reason</param>
        void AddSaleLineReason(SaleLineReason reason);

        /// <summary>
        /// Method to add sale line tax
        /// </summary>
        /// <param name="lineTax">Line tax</param>
        void AddSaleLineTax(Line_Tax lineTax);

        /// <summary>
        /// Method to add sale line kit
        /// </summary>
        /// <param name="lineKit">Line kit</param>
        void AddSaleLineKit(Line_Kit lineKit);

    }
}

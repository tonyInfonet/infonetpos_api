using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IPrepayService
    {

        /// <summary>
        /// Method to delete prepay from POS
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <returns>True or false</returns>
        bool DeletePrepaymentFromPos(short pumpId);

        /// <summary>
        /// Method to get prepay global
        /// </summary>
        /// <param name="pumpId">Pump id</param>
        /// <returns>Prepay</returns>
        recPump GetPrepayGlobal(int pumpId);

        /// <summary>
        /// Method to load prepay
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <returns>Prepay</returns>
        recPump LoadPrepay(int pumpId);

        /// <summary>
        /// Method to lock prepay
        /// </summary>
        /// <param name="pumpId">Pump id</param>
        /// <returns>True or false</returns>
        bool LockPrepay(short pumpId);

        /// <summary>
        /// Method to set prepay from POS
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="amount">Amount</param>
        /// <param name="mop">MOP</param>
        /// <param name="positionId">Position Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>True or false</returns>
        bool SetPrepaymentFromPos(int invoiceId, short pumpId, float amount, byte mop, 
            byte positionId, int tillNumber);


        /// <summary>
        /// Method to update prepay pump id for sale
        /// </summary>
        /// <param name="saleNo">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="newPumpId">New pump Id</param>
        void UpdatePrepayPumpIdForSale(int saleNo, int tillNumber, short newPumpId);

        /// <summary>
        /// Method to get line kit
        /// </summary>
        /// <param name="db">Data source</param>
        /// <param name="sn">Sale number</param>
        /// <param name="ln">Line number</param>
        /// <returns>Line kits</returns>
        Line_Kits Get_Line_Kit(DataSource db, int sn, int ln);

        /// <summary>
        /// Get purchase items
        /// </summary>
        /// <param name="db">Data source</param>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>Purchase items</returns>
        List<tePurchaseItem> GetPurchaseItems(DataSource db, int saleNumber);

        /// <summary>
        /// Method to get original sale head
        /// </summary>
        /// <param name="invoiceId">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data base</param>
        /// <returns>Sale head</returns>
        SaleHead GetOriginalSaleHead(int invoiceId, int tillNumber, out DataSource dataSource);

        /// <summary>
        /// Method to get orginal saleLines
        /// </summary>
        /// <param name="invoiceId">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of sale lines</returns>
        List<Sale_Line> GetOrginalSaleLines(int invoiceId, int tillNumber, DataSource dataSource);

        /// <summary>
        /// Method to get original position Id
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        short? GetOrginalPositionId(int saleNumber, int tillNumber, DataSource dataSource);

        /// <summary>
        /// Method to get discount tender
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        DiscountTender GetDiscountTender(int saleNumber, int tillNumber, DataSource dataSource);

        /// <summary>
        /// Method to get stock description
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Description</returns>
        string GetStockDescription(string stockCode);

        /// <summary>
        /// Method to get line taxes
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns></returns>
        List<Line_Tax> GetLineTaxes(int saleNumber, int lineNumber, DataSource dataSource);

        /// <summary>
        /// Method to get product tax exempt
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns></returns>
        ProductTaxExempt GetProductTaxExemptForStock(string stockCode);

        /// <summary>
        /// Method to update discount tender
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        /// <param name="discounTender">Discount tender</param>
        void UpdateDiscountTender(int saleNumber, int tillNumber, DataSource dataSource, DiscountTender discounTender);

        /// <summary>
        /// Method to update coupon
        /// </summary>
        /// <param name="couponId">Coupon Id</param>
        /// <param name="amount">Amount</param>
        void UpdateCoupon(string couponId, string amount);
        
        /// <summary>
        /// Method to get sale line charges
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Charges</returns>
        Charges GetSaleLineCharges(int saleNumber, int lineNumber, DataSource dataSource);

        /// <summary>
        /// Method to update sale data
        /// </summary>
        /// <param name="sx">Sale</param>
        /// <param name="sp">Sale line</param>
        /// <param name="pennyAdj">Penny adjustment</param>
        /// <param name="change">Change</param>
        /// <param name="saleQuantity">Sale quantity</param>
        /// <param name="taxExempt">Tax exempt</param>
        /// <param name="saleAmount">Sale amount</param>
        /// <param name="unitPrice">Unit price</param>
        /// <param name="iPositionId">Position Id</param>
        /// <param name="orgPosition">Original position</param>
        /// <param name="iGradeId">Grade Id</param>
        /// <param name="fuelLoyalty">Fuel loyalty</param>
        /// <param name="newTotalAmount">Total amount</param>
        /// <param name="fullSwitch">Full switch</param>
        /// <param name="dataSource">Data source</param>
        void UpdateSaleData(Sale sx, Sale_Line sp, decimal pennyAdj, double change, float saleQuantity,
            bool taxExempt, float saleAmount, float unitPrice, short iPositionId, short orgPosition, short iGradeId,
           bool fuelLoyalty, float newTotalAmount, bool fullSwitch, DataSource dataSource);
    }
}
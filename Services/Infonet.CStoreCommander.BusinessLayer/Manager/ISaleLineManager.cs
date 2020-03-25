using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ISaleLineManager
    {
        /// <summary>
        /// Method to set sale line policies
        /// </summary>
        /// <param name="saleLine"></param>
        void SetSaleLinePolicy(ref Sale_Line saleLine);


        /// <summary>
        /// Method to create a new saleLine
        /// </summary>
        /// <returns>Sale line</returns>
        Sale_Line CreateNewSaleLine();

        /// <summary>
        /// Method to set plu code
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="saleLine">Sale line</param>
        /// <param name="stockCode">Stock code</param>
        /// <param name="error"></param>
        /// <param name="isReturnMode"></param>
        void SetPluCode(ref Sale sale, ref Sale_Line saleLine, string stockCode, out ErrorMessage
            error, bool isReturnMode = false);

        /// <summary>
        /// Method to set sub detail
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="subDetail">Sub detail</param>
        void SetSubDetail(ref Sale_Line saleLine, string subDetail);

        /// <summary>
        /// Method to adjust the discount rate
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="discountRate">Discount rate</param>
        void SetDiscountRate(ref Sale_Line saleLine, float discountRate);

        /// <summary>
        /// Method to set the amount
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="amount">Amount</param>
        void SetAmount(ref Sale_Line saleLine, decimal amount);

        /// <summary>
        /// Method to set price
        /// </summary>
        /// <param name="saleLine">Saleline</param>
        /// <param name="price">Price</param>
        void SetPrice(ref Sale_Line saleLine, double price);

        /// <summary>
        /// Method to set quantity
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="quantity">Quantity</param>
        void SetQuantity(ref Sale_Line saleLine, float quantity);

        /// <summary>
        /// Method to set price number
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="priceNumber">Price number</param>
        void SetPriceNumber(ref Sale_Line saleLine, short priceNumber);

        /// <summary>
        /// Method to set stock code
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="saleLine">Sale line</param>
        /// <param name="stockCode">Stock code</param>
        /// <param name="userCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="isReturnMode"></param>
        void SetStockCode(ref Sale sale, ref Sale_Line saleLine, string stockCode, string userCode,
            out ErrorMessage errorMessage, bool isReturnMode = false);


        /// <summary>
        /// Method to apply table discount
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="pc">PC</param>
        /// <param name="cc">cc</param>
        /// <param name="errorMessage">Error</param>
        void Apply_Table_Discount(ref Sale_Line saleLine, short pc, short cc, out ErrorMessage
            errorMessage);

        /// <summary>
        /// Method to apply fuel loyalty 
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="discountType">Discount type</param>
        /// <param name="discountRate">Discount rate</param>
        /// <param name="discountName">Discount name</param>
        void ApplyFuelLoyalty(ref Sale_Line saleLine, string discountType, float discountRate,
            string discountName = "");

        /// <summary>
        /// Method to apply fuel rebate
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        void ApplyFuelRebate(ref Sale_Line saleLine);

        /// <summary>
        /// Method to make group prices
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="dept">Department</param>
        /// <param name="subDept">Sub department</param>
        /// <param name="subDetail">Sub detail</param>
        /// <returns></returns>
        SP_Prices MakeGroupPrice(ref Sale_Line saleLine, string dept, string subDept, string
            subDetail);

        /// <summary>
        /// Method to load sale line policies
        /// </summary>
        /// <param name="saleLine"></param>
        void LoadSaleLinePolicies(ref Sale_Line saleLine);

        /// <summary>
        /// Method to get fuel discount chart rate
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="groupId">Group Id</param>
        /// <param name="gradeId">Grade ID</param>
        /// <returns>Fuel discount</returns>
        float GetFuelDiscountChartRate(ref Sale_Line saleLine, string groupId, byte gradeId);


        /// <summary>
        /// Method to  check restrictions on stock 
        /// </summary>
        /// <param name="isReturnMode">Return mode or not</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">TillNumber</param>
        /// <param name="userCode"> User code</param>
        /// <param name="error">Error</param>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Stock message</returns>
        StockMessage CheckStockConditions(int saleNumber, int tillNumber, string stockCode, string userCode, bool isReturnMode,
            float quantity, out ErrorMessage error);


        /// <summary>
        /// Make Promo
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="saleLine">Sale line</param>
        /// <param name="changeQuantity">Change quantity</param>
        void Make_Promo(ref Sale sale, ref Sale_Line saleLine, bool changeQuantity = false);

        /// <summary>
        /// Method to make taxes
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <returns>Line taxes</returns>
        Line_Taxes Make_Taxes(Sale_Line saleLine);


  
        /// <summary>
        /// Set policies at each sale line level
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        void SetLevelPolicies(ref Sale_Line saleLine);

    }
}

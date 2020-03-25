using System.Collections.Generic;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    /// <summary>
    /// Sale Manager interface 
    /// </summary>
    public interface ISaleManager
    {
        /// <summary>
        /// Clear Sale 
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="saleType">Sale type</param>
        /// <param name="tenders">Tenders</param>
        /// <param name="newSale">New sale</param>
        /// <param name="setCtrl">Set control or not</param>
        /// <param name="useSameSaleNo">Same sale number or not</param>
        /// <param name="message">Error</param>
        /// <returns>Sale number</returns>
        int Clear_Sale(Sale sale, int saleNumber, int tillNumber, string userCode, string saleType,
            Tenders tenders, bool newSale, bool setCtrl, bool useSameSaleNo,
            out ErrorMessage message);

        /// <summary>
        /// Initilize a sale 
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="registerNumber">registerNumber</param>
        /// <param name="userCode">User code</param>
        /// <param name="message">Error</param>
        /// <returns>Sale</returns>
        Sale InitializeSale(int tillNumber, int registerNumber, string userCode, out ErrorMessage message);


        /// <summary>
        /// Method to apply taxes
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="value">Value</param>
        void ApplyTaxes(ref Sale sale, bool value);

        /// <summary>
        /// Method to apply charges
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="value">Value</param>
        void ApplyCharges(ref Sale sale, bool value);

        /// <summary>
        /// Method to save sale in Db cursale
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="tillNumber"></param>
        void SaveTemp(ref Sale sale, int tillNumber);

        /// <summary>
        /// Method to compute point
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <returns>Points</returns>
        decimal ComputePoints(Sale sale);

        /// <summary>
        /// Method to compute points that cannot be used to buy items in current sale
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <returns>Sub points</returns>
        decimal SubPoints(Sale sale);

        /// <summary>
        /// Method to save profile promt
        /// </summary>
        /// <param name="cprompts">card prompts</param>
        /// <param name="cardNum">Card number</param>
        /// <param name="profileId">Profile Id</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        void Save_ProfilePrompt_Temp(CardPrompts cprompts, string cardNum, string profileId, int saleNumber,
            int tillNumber);

        /// <summary>
        /// Method to get sub redeemable
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <returns>Sub redeemable</returns>
        decimal SubRedeemable(Sale sale);

        /// <summary>
        /// Method to save temporary tender
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="sale">Sale</param>
        void Save_Tender_Temp(ref Tenders tenders, Sale sale);


        /// <summary>
        /// Method to recompute cash bonus
        /// </summary>
        /// <param name="sale">Sale</param>
        void ReCompute_CashBonus(ref Sale sale);

        /// <summary>
        /// Method to set the line quantity
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="adjust">Adjudt line or not</param>
        /// <returns>True or false</returns>
        bool Line_Quantity(ref Sale sale, ref Sale_Line oLine, float quantity, bool adjust = true);

        /// <summary>
        /// Method to adjust lines
        /// </summary>
        /// <param name="thisLine">Sale line</param>
        /// <param name="sale">Sale</param>
        /// <param name="newLine">New line required or not</param>
        /// <param name="remove">Remove items or not</param>
        /// <returns>True or false</returns>
        bool Adjust_Lines(ref Sale_Line thisLine, Sale sale, bool newLine, bool remove = false);

        /// <summary>
        /// Method to add a saleline item
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="registerNumber">Registernumber</param>
        /// <param name="stockCode">Stock code</param>
        /// <param name="quantity">Quanity</param>
        /// <param name="isReturnMode">Return mode</param>
        /// <param name="giftCard">Gift card</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>True or false</returns>
        Sale AddSaleLineItem(string userCode, int tillNumber, int saleNumber, byte registerNumber,
            string stockCode, decimal quantity, bool isReturnMode, GiftCard giftCard, out ErrorMessage errorMessage);



        /// <summary>
        /// Verify Add Sale Line item
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        /// <param name="registerNumber"></param>
        /// <param name="stockCode"></param>
        /// <param name="quantity"></param>
        /// <param name="isReturnMode"></param>
        /// <param name="giftCard"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        Sale VerifyAddSaleLineItem(string userCode, int tillNumber, int saleNumber, byte registerNumber,
            string stockCode, decimal quantity, bool isReturnMode, GiftCard giftCard,
            out ErrorMessage errorMessage);

        /// <summary>
        /// Method to remmove a sale line item
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="tillNumber">TIll number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="error">Error message</param>
        /// <param name="adjust">Adjustment required or not</param>
        /// <param name="makePromo">make promotional items or not</param>
        /// <returns>Sale</returns>
        Sale RemoveSaleLineItem(string userCode, int tillNumber, int saleNumber, int lineNumber,
            out ErrorMessage error, bool adjust, bool makePromo);

        /// <summary>
        /// Method to save sale in Dbtill
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="userCode">UserCode</param>
        /// <param name="tenders">Tenders</param>
        /// <param name="sc">Store credit</param>
        void SaveSale(Sale sale, string userCode, ref Tenders tenders, Store_Credit sc);

        /// <summary>
        /// Method to set the discount type on a line.
        /// </summary>
        /// <param name="oLine"></param>
        /// <param name="discType"></param>
        void Line_Discount_Type(ref Sale_Line oLine, string discType);

        /// <summary>
        /// Method to recompute sale totals
        /// </summary>
        /// <param name="sale">Sale </param>
        void ReCompute_Totals(ref Sale sale);

        /// <summary>
        /// Method to recompute coupon
        /// </summary>
        /// <param name="sale">Sale</param>
        void ReCompute_Coupon(ref Sale sale);

        /// <summary>
        /// Method to set the Price Number on a line
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="saleLine"></param>
        /// <param name="priceNumber"></param>
        void Line_Price_Number(ref Sale sale, ref Sale_Line saleLine, short priceNumber);

        /// <summary>
        /// Method to get line reason
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="saleLine">Sale line</param>
        /// <param name="returnReason">Return reason</param>
        void Line_Reason(ref Sale sale, ref Sale_Line saleLine, Return_Reason returnReason);

        /// <summary>
        /// Method to set Line price
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="price">Price</param>
        void Line_Price(ref Sale sale, ref Sale_Line oLine, double price);

        /// <summary>
        /// Method to set sale discount
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="discount">Discount</param>
        /// <param name="discType">Discount type</param>
        /// <param name="tempFile">Temp file or not</param>
        /// <param name="dontRecaculateTotal">If recalculation is required or not</param>
        void Sale_Discount(ref Sale sale, decimal discount, string discType, bool tempFile = true,
            bool dontRecaculateTotal = false);

        /// <summary>
        /// Method to set the discount rate on a line
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="discRate">Discount rate</param>
        void Line_Discount_Rate(ref Sale sale, ref Sale_Line oLine, double discRate);

        /// <summary>
        /// Method to add a line
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="userCode">User code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="error">Error</param>
        /// <param name="adjust">Adjust</param>
        /// <param name="tableAdjust">Table adjust</param>
        /// <param name="tempFile">Temporary file</param>
        /// <param name="forRefund">For refund or not</param>
        /// <param name="forReprint">For reprint or not</param>
        /// <param name="makePromo">Make promo</param>
        /// <param name="getWeight">Get weight or not</param>
        /// <returns>True or false</returns>
        bool Add_a_Line(ref Sale sale, Sale_Line oLine, string userCode, int tillNumber, out ErrorMessage error, bool adjust = false,
             bool tableAdjust = true, bool tempFile = false, bool forRefund = false, bool forReprint = false,
             bool makePromo = false, bool getWeight = false);

        /// <summary>
        /// Method to set customer
        /// </summary>
        /// <param name="customerCode">Customer code</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Sale</returns>
        Sale SetCustomer(string customerCode, int saleNumber, int tillNumber, string userCode,
            byte registerNumber, string card, out ErrorMessage errorMessage);

        /// <summary>
        /// Method to set gross amount
        /// </summary>
        /// <param name="saleTotals">Sale totals</param>
        /// <param name="netAmount">Net amount</param>
        void SetGross(ref Sale_Totals saleTotals, decimal netAmount);

        /// <summary>
        /// Get Sale number
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="message">Error</param>
        /// <returns>Sale number</returns>
        int GetSaleNo(int tillNumber, string userCode, out ErrorMessage message);


        /// <summary>
        /// Method to check whether we can edit a field or not
        /// </summary>
        /// <param name="saleline">Sale lines</param>
        /// <param name="userCode">User code</param>
        /// <returns>Sale line edit</returns>
        List<SaleLineEdit> CheckEditOptions(Sale_Lines saleline, string userCode);

        /// <summary>
        /// Update Sale Line Item
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="userCode">User code</param>
        /// <param name="discountRate">Discount rate</param>
        /// <param name="discountType">Discount type</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="price">Price</param>
        /// <param name="reasonCode">Reason code</param>
        /// <param name="reason">Reason</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Sale</returns>
        Sale UpdateSaleLine(int saleNumber, int tillNumber, int lineNumber, string userCode, decimal discountRate, string discountType,
            decimal quantity, float price, string reasonCode, string reason, byte registerNumber,
            out ErrorMessage errorMessage);


        /// <summary>
        /// Method to get sale summary
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Sale</returns>
        object GetSaleSummary(int saleNumber, int tillNumber, string userCode, out ErrorMessage
            errorMessage);

        /// <summary>
        /// Method to create a sale object from current sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="registerNumber"></param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        Sale GetCurrentSale(int saleNumber, int tillNumber, byte registerNumber, string userCode, out ErrorMessage errorMessage);

        /// <summary>
        /// Method to look up in line for adding a sale line item
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="stockCode">Stock code</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="userCode">User code</param>
        /// <param name="isReturnMode">Return mode or not</param>
        /// <param name="giftCard">Gift card</param>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        void Look_Up_Line(ref Sale sale, int lineNumber, string stockCode, decimal quantity, string userCode,
          bool isReturnMode, GiftCard giftCard, out ErrorMessage error);

        /// <summary>
        /// Get current Sale number
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="message">Error</param>
        /// <returns>Sale number</returns>
        int GetCurrentSaleNo(int tillNumber, string userCode, out ErrorMessage message);

        /// <summary>
        /// Method to enable cash button
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="userCode">User code</param>
        /// <returns>True or false</returns>
        bool EnableCashButton(Sale sale, string userCode);

        /// <summary>
        /// Method to enable write off button
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <returns>True or false</returns>
        bool EnableWriteOffButton(string userCode);

        /// <summary>
        /// Method to load vendor coupons
        /// </summary>
        /// <returns>Vendor coupons</returns>
        VendorCoupons LoadVendorCoupons();

        /// <summary>
        /// Method to set tax exemption code
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="taxExemptionCode">Tax exemption code</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        Sale SetTaxExemptionCode(int saleNumber, int tillNumber, string userCode,
        string taxExemptionCode, out ErrorMessage error);
    }
}
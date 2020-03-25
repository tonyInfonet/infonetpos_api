using System;
using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    /// <summary>
    /// Sale service interface
    /// </summary>
    public interface ISaleService
    {
        /// <summary>
        /// Check payments
        /// </summary>
        /// <param name="saleNo"></param>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        bool CheckPaymentsFromDbTemp(int saleNo, int tillNumber);

        /// <summary>
        /// Get the maximum sale no from Sale head
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <returns>Sale number</returns>
        int GetMaxSaleNoFromSaleHeadFromDbTill(int tillNumber);

        /// <summary>
        /// Get the max Sale no from sale number
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="messageNumber"></param>
        /// <returns>Sale number </returns>
        int GetMaxSaleNoFromSaleNumbFromDbAdmin(int tillNumber, out int messageNumber);

        /// <summary>
        /// Get the max  Sale number from suspended head 
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <returns>Sale number</returns>
        int GetMaxSaleNoFromSusHeadFromDbTill(int tillNumber);

        ///// <summary>
        ///// Get sale
        ///// </summary>
        ///// <param name="tillNumber"></param>
        ///// <returns>Sale</returns>
        //Sale GetSalesFromDbTemp(int tillNumber);

        /// <summary>
        /// Get tax exepmt message
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns>messages</returns>
        List<string> GetTaxExemptMessagesFromDbTrans(string messageType);

        /// <summary>
        /// Update tax exepmt messages
        /// </summary>
        void UpdateTaxExemptMessagesToDbTrans();


        /// <summary>
        /// Method to get list of sale tenders from CSCCursale
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>List of sale tenders</returns>
        List<SaleTend> GetSaleTendsFromDbTemp(int tillNumber, int saleNumber);


        /// <summary>
        /// Get Sale by Sale number
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        /// <param name="sale"></param>
        /// <returns>Sale</returns>
        Sale GetSaleBySaleNoFromDbTill(ref Sale sale, int tillNumber, int saleNumber);

        /// <summary>
        /// Delete signature 
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        void DeleteSignatureFromDbTill(int saleNumber, int tillNumber);

        /// <summary>
        /// Get Sale lines 
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <param name="userCode"></param>
        /// <returns>Sale_Line</returns>
        List<Sale_Line> GetSaleLinesFromDbTemp(int saleNumber, int tillNumber, string userCode);

        /// <summary>
        /// Save Card Sales
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="oLine"></param>
        /// <param name="givexReceipt"></param>
        /// <param name="dataSource"></param>
        void SaveCardSales(Sale sale, Sale_Line oLine, GiveXReceiptType givexReceipt, DataSource dataSource);


        /// <summary>
        /// Remove Previous Transactions
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        void RemovePreviousTransactionsFromDbTemp(int tillNumber, int saleNumber);


        /// <summary>
        /// Save Deleted Lines
        /// </summary>
        /// <param name="oLine"></param>
        /// <param name="delType"></param>
        void SaveDeletedLineToDbTill(ref Sale_Line oLine, string delType);

        /// <summary>
        /// Get Tax Master 
        /// </summary>
        /// <returns></returns>
        List<TaxMast> GetTaxMast();

        /// <summary>
        /// Get tax rates
        /// </summary>
        /// <returns></returns>
        List<TaxRate> GetTaxRates();

        /// <summary>
        /// Delete records from Till
        /// </summary>
        /// <param name="saleLineNumber"></param>
        /// <param name="tillNumber"></param>
        void ClearRecordsFromDbTill(int saleLineNumber, int tillNumber);

        /// <summary>
        /// Get Sale Tends from DB Till
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        List<SaleTend> GetSaleTendsFromDbTill(int tillNumber);

        /// <summary>
        /// Add Sale Tend
        /// </summary>
        /// <param name="saleTend"></param>
        /// <param name="dataSource"></param>
        void AddSaleTend(SaleTend saleTend, DataSource dataSource);

        /// <summary>
        /// Add Card Tender
        /// </summary>
        /// <param name="cardSale"></param>
        /// <param name="dataSource"></param>
        void AddCardTender(CardTender cardSale, DataSource dataSource);


        /// <summary>
        /// Add Card profile
        /// </summary>
        /// <param name="cardProfilePrompt"></param>
        void AddCardProfilePromptToDbTemp(CardProfilePrompt cardProfilePrompt);


        /// <summary>
        /// Update SaleHead 
        /// </summary>
        /// <param name="saleHead"></param>
        void UpdateSaleHeadToDbTill(SaleHead saleHead);

        /// <summary>
        /// Add Void Sale
        /// </summary>
        /// <param name="voidSale"></param>
        void AddVoidSaleToDbTill(VoidSale voidSale);

        /// <summary>
        /// Get Void Sale
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        VoidSale GetVoidSaleFromDbTill(int tillNumber);

        /// <summary>
        /// Add Sale Vendors
        /// </summary>
        /// <param name="saleVendors"></param>
        void AddSaleVendorsToDbTill(SaleVendors saleVendors);

        /// <summary>
        /// Get Sale Tendors
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        SaleVendors GetSaleVendorsFromDbTill(int tillNumber);

        /// <summary>
        /// Add Sale Tax
        /// </summary>
        /// <param name="saleTax"></param>
        void AddSaleTaxToDbTill(Sale_Tax saleTax);

        /// <summary>
        /// Add SaleLine
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="saleType">Sale type</param>
        void AddSaleLineToDbTill(Sale_Line saleLine, string saleType);

        /// <summary>
        /// Update Tax Exempt
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="dataSource"></param>
        void UpdateTaxExempt(string strSql, DataSource dataSource);

        /// <summary>
        /// Update Stock BR
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="dataSource"></param>
        void UpdateStockBr(string strSql, DataSource dataSource);

        /// <summary>
        /// Clear Tender Records
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        void ClearTenderRecordsFromDbTemp(int tillNumber, int saleNumber);

        /// <summary>
        /// Add Gift Certificate
        /// </summary>
        /// <param name="giftCert"></param>
        void AddGiftCertificate(GiftCert giftCert);

        /// <summary>
        /// Add Sale Line reason
        /// </summary>
        /// <param name="reason"></param>
        void AddSaleLineReason(SaleLineReason reason);

        /// <summary>
        /// Add Sale Line Tax
        /// </summary>
        /// <param name="lineTax"></param>
        void AddSaleLineTax(Line_Tax lineTax);


        /// <summary>
        /// Add Sale Line Kit
        /// </summary>
        /// <param name="lineKit"></param>
        void AddSaleLineKit(Line_Kit lineKit);

        /// <summary>
        /// Add Charge Tax
        /// </summary>
        /// <param name="chargeTax"></param>
        void AddChargeTax(Charge_Tax chargeTax);

        /// <summary>
        /// Add Charge
        /// </summary>
        /// <param name="charge"></param>
        void AddCharge(Charge charge);

        /// <summary>
        /// Get Discount Tender
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        DiscountTender GetDiscountTender(int saleNumber, int tillNumber);

        /// <summary>
        /// Add Discount Tender
        /// </summary>
        /// <param name="tender"></param>
        void AddDiscountTender(DiscountTender tender);


        /// <summary>
        /// Update Discount Tender
        /// </summary>
        /// <param name="tender"></param>
        void UpdateDiscountTender(DiscountTender tender);

        /// <summary>
        /// Get Coupon
        /// </summary>
        /// <param name="couponId"></param>
        /// <returns></returns>
        Coupon GetCoupon(string couponId);

        /// <summary>
        /// Add Coupon
        /// </summary>
        /// <param name="coupon"></param>
        void AddCoupon(Coupon coupon);

        /// <summary>
        /// Update Coupon
        /// </summary>
        /// <param name="coupon"></param>
        void UpdateCoupon(Coupon coupon);

        /// <summary>
        /// Add Kit Charge
        /// </summary>
        /// <param name="charge"></param>
        void AddKitCharge(Charge charge);

        /// <summary>
        /// Add Sale Head
        /// </summary>
        /// <param name="saleHead"></param>
        void AddSaleHeadToDbTill(SaleHead saleHead);

        /// <summary>
        /// Check Existing sale in DB
        /// </summary>
        /// <returns></returns>
        bool ExistingSaleInDbTemp(int tillNumber);

        /// <summary>
        /// Remove Temp data
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        void RemoveTempDataInDbTill(int tillNumber, int saleNumber);

        /// <summary>
        /// Get the max sale number from Sale number
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="messageNumber"></param>
        /// <returns>Sale number</returns>
        int GetMaxSaleNo(int tillNumber, out int messageNumber);

        /// <summary>
        /// Method to get sale number
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="user">User</param>
        /// <param name="messageNumber">Message number</param>
        /// <returns>Sale number</returns>
        int GetSaleNo(int tillNumber, User user, out int messageNumber);

        /// <summary>
        /// Method to set prepay from POS
        /// </summary>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="pumpId">Pump ID</param>
        /// <param name="amount">Amount</param>
        /// <param name="mop">MOP</param>
        /// <param name="positionId">Position Id</param>
        /// <returns>True or false</returns>
        bool SetPrepaymentFromPos(int invoiceId, int tillNumber, short pumpId, float amount,
            byte mop, byte positionId);

        /// <summary>
        /// Method to track price change
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="stockCode"></param>
        /// <param name="origPrice"></param>
        /// <param name="newPrice"></param>
        /// <param name="processType"></param>
        /// <param name="pricenum"></param>
        /// <param name="vendorId"></param>
        //Processtype PC--> Price check; SL--> Saleline; SC--> Stock Screen Cost Change(Price will change) SP-Stock Screen Price Change; VC- Stock Screen Switch Active Vendor; PR--> Product Category-Change Price;  FP- FuturePrice rollover
        void Track_PriceChange(string userCode, string stockCode, double origPrice, double newPrice,
            string processType, byte pricenum, string vendorId = "");


        /// <summary>
        ///Method to update sale non resettable grant total
        /// </summary>
        /// <param name="tillNum">TIll number</param>
        /// <param name="sDate">Sale date</param>
        /// <param name="value">total</param>
        void Update_Sale_NoneResettableGrantTotal(short tillNum, DateTime sDate, decimal value);

        /// <summary>
        /// Method to update till close non resettable grant total
        /// </summary>
        /// <param name="tillNum">Till number</param>
        /// <param name="sDate">Sale date</param>
        void Update_TillClose_NoneResettableGrantTotal(short tillNum, DateTime sDate);

        /// <summary>
        /// Method to get loyalty card
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        string GetLoyaltyCardFromDbTemp(int saleNumber, DataSource dataSource);

        /// <summary>
        /// Method to get sale tenders from CSCCurSale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>List of sale tender</returns>
        List<SaleTend> GetSaleTendersFromDbTemp(int saleNumber, int tillNumber);

        /// <summary>
        /// Method to get list of sale vendor coupon line
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Sale vendor coupon line</returns>
        List<SaleVendorCouponLine> GetSaleVendorCoupons(int saleNumber, int tillNumber);

        /// <summary>
        /// Method to get card tender
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderName">Tender name</param>
        /// <returns>Tender</returns>
        Tender GetCardTenderFromDbTemp(int saleNumber, int tillNumber, string tenderName);

        /// <summary>
        /// Method to get refund sale tender
        /// </summary>
        /// <param name="saleNo">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Sale tender</returns>
        SaleTend GetRefundSaleTender(int saleNo, DataSource dataSource);

        /// <summary>
        /// Method to get card tender
        /// </summary>
        /// <param name="saleNo">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Card tender</returns>
        CardTender GetCardTender(int saleNo, DataSource dataSource);

        /// <summary>
        /// Get Sale
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        /// <returns></returns>
        Sale GetSale(int tillNumber, int saleNumber);

        /// <summary>
        /// Save Sale
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        /// <param name="sale"></param>
        void SaveSale(int tillNumber, int saleNumber, Sale sale);


        /// <summary>
        /// Get Sale By Till Number
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <returns></returns>
        Sale GetSaleByTillNumber(int tillNumber);

        /// <summary>
        /// Method to save sale for sale vendor coupon
        /// </summary>
        /// <param name="saleVendorCoupon">Sale vendor coupon</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        void SaveSaleForSaleVendorCoupon(SaleVendorCoupon saleVendorCoupon, int saleNumber, int tillNumber);

        /// <summary>
        /// Method to update discount tender
        /// </summary>
        /// <param name="sale">Sale</param>
        void UpdateDiscountTender(ref Sale sale);

        Sale_Line GetPrepaySaleLine(int saleNumber, int tillNumber);

        /// <summary>
        /// Method to get list of sale vendor coupon line from Data source
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Sale vendor coupon line</returns>
        List<SaleVendorCouponLine> GetSaleVendorCouponsForReprint(int saleNumber,
            int tillNumber);

        /// <summary>
        /// Method to load tenders used in sale
        /// </summary>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Tenders list</returns>
        List<SaleTend> LoadTendersInSale(int invoiceId, DataSource dataSource);

        /// <summary>
        /// Get Card Tender
        /// </summary>
        /// <param name="saleNumber"></param>
        /// <param name="tillNumber"></param>
        /// <param name="tenderName"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        Credit_Card GetCardTender(int saleNumber, int tillNumber, string tenderName, DataSource dataSource);

    }
}
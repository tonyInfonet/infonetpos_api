using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.ADOData
{
    public interface ITenderService
    {
        /// <summary>
        /// Gets PCAT group
        /// </summary>
        /// <param name="tendClass"></param>
        /// <returns></returns>
        string GetPcatsGroup(string tendClass);

        /// <summary>
        /// Method to get vendor coupons
        /// </summary>
        /// <returns>Vendor coupons</returns>
        VendorCoupons GetAllVendorCoupon();

        /// <summary>
        /// Method to get list of cash buttons
        /// </summary>
        /// <returns>List of cash buttons</returns>
        List<CashButton> GetCashButton();

        /// <summary>
        /// Method to get first third party
        /// </summary>
        /// <param name="blCombineThirdParty">Combine third party or not</param>
        /// <returns>Value</returns>
        byte GetFirstThirdParty(bool blCombineThirdParty);

        /// <summary>
        /// Method to get first fleet
        /// </summary>
        /// <param name="blCombineFleet">Combine fleet or not</param>
        /// <returns>Fleet value</returns>
        byte GetFirstFleet(bool blCombineFleet);

        /// <summary>
        /// Method to get first credit card
        /// </summary>
        /// <param name="blCombineCredit">Combine credit or not</param>
        /// <returns>First Credit card</returns>
        byte GetFirstCc(bool blCombineCredit);

        /// <summary>
        /// Method to get all tender
        /// </summary>
        /// <returns>List of tenders</returns>
        List<Tender> GetAlltenders();

        /// <summary>
        /// Method to check if eko gift cert
        /// </summary>
        /// <param name="strTender">Tender name</param>
        /// <returns>True or false</returns>
        bool IsEkoGiftCert(string strTender);

        /// <summary>
        /// Method to get tenders while closing current till
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Tenders</returns>
        Tenders GetTenderForCloseCurrentTill(int tillNumber);


        /// <summary>
        /// Method to load GC tenders
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="dataSource">Data source</param>
        /// <returns>Gift cert payment</returns>
        GCPayment Load_GCTenders(int tillNumber, int saleNumber, DataSource dataSource);

        /// <summary>
        /// Method to get credit card tender
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderName">Tender name</param>
        /// <returns>Credit card</returns>
        Credit_Card GetCreditCardTender(int saleNumber, int tillNumber, string tenderName);

        /// <summary>
        /// Method to delete sale vendor coupon for sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tendDesc">Tender description</param>
        void DeleteSaleVendorCoupon(int saleNumber, int tillNumber, string tendDesc);

        /// <summary>
        /// Method to delete sale vendor coupon by coupon id
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tendDesc">Tender description</param>
        /// <param name="couponId">Coupon id</param>
        void DeleteSaleVendorCouponByCouponId(int saleNumber, int tillNumber, string tendDesc, string couponId);

        /// <summary>
        /// Method to remove one coupon line
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tendDesc">Tender description</param>
        /// <param name="couponId">Coupon id</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="sequenceNumber">Sequence number</param>
        void RemoveOneCouponLine(int saleNumber, int tillNumber, string tendDesc, string couponId, short lineNumber,
            short sequenceNumber);

        /// <summary>
        /// Method to get minimum display sequence
        /// </summary>
        /// <param name="cardClass">Card class</param>
        /// <returns>Minimum display sequence</returns>
        byte GetMinDisplaySeq(string cardClass);

        /// <summary>
        /// Method to get tender name
        /// </summary>
        /// <param name="tender">Tender</param>
        /// <returns>Tender name</returns>
        string GetTenderName(string tender);

        /// <summary>
        /// Method to get list of all sale vendor coupons
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderName">Tender name</param>
        /// <returns></returns>
        List<SaleVendorCouponLine> GetSaleVendorCouponForTender(int saleNumber, int tillNumber, string tenderName);

        /// <summary>
        /// Method to save new GC tender
        /// </summary>
        /// <param name="oGcLine">Gift cert line</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="dataSource">Data source</param>
        void Save_New_GCTender(GCTender oGcLine, int saleNumber, int tillNumber, DataSource dataSource);

        /// <summary>
        /// Method to save to till
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        void SaveToTill(int saleNumber, int tillNumber);

        /// <summary>
        /// Method to save store credit
        /// </summary>
        /// <param name="storeCredit"></param>
        void SaveCredit(Store_Credit storeCredit);

        /// <summary>
        /// Method to get list of gift cert credit
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Gift cert list</returns>
        string[,] GetGcCredit(int saleNumber, int tillNumber);

        /// <summary>
        /// Method to get store credit list
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Store credit list</returns>
        string[,] GetScCredit(int saleNumber, int tillNumber);

        /// <summary>
        /// Method to remove gift certificate
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="gcCredits">Gift certificate credits</param>
        void RemoveGc(int saleNumber, string[,] gcCredits);

        /// <summary>
        /// Method to remove store credit
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="storeCredits">List of store credits</param>
        void RemoveSc(int saleNumber, string[,] storeCredits);

        /// <summary>
        /// Method to add gift cert to CSCCurSale 
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="giftCerts">List of gift certs</param>
        void AddGcToDbTemp(int saleNumber, int tillNumber, List<GiftCert> giftCerts);


        /// <summary>
        /// Method to add store credit to CSCCurSale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="storeCredits">List of store credits</param>
        void AddScToDbTemp(int saleNumber, int tillNumber, List<Store_Credit> storeCredits);

        /// <summary>
        /// Method to get list of gift certificates
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of gift certificates</returns>
        List<GiftCert> GetAllGiftCert(DataSource dataSource);

        /// <summary>
        /// Method to get list of expiredd gift certiifcates
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of gift certificates</returns>
        List<GiftCert> GetExpiredGiftCert(DataSource dataSource);

        /// <summary>
        /// Method to get list of store credits
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of store credits</returns>
        List<Store_Credit> GetAllStoreCredits(DataSource dataSource);

        /// <summary>
        /// Method to get list of expired store credits
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <returns>List of store credits</returns>
        List<Store_Credit> GetExpiredStoreCredits(DataSource dataSource);

        /// <summary>
        /// Method to get card type by tender code
        /// </summary>
        /// <param name="tenderCode">Tender code</param>
        /// <returns>Card type</returns>
        string GetCardTypeByTenderCode(string tenderCode);

        /// <summary>
        /// Method to load tender card
        /// </summary>
        /// <param name="cardId">Card id</param>
        /// <returns>Tender card</returns>
        TenderCard LoadTenderCard(int cardId);

        /// <summary>
        /// Method to get card type by tender code
        /// </summary>
        /// <param name="tenderCode">Tender code</param>
        /// <returns>Card type</returns>
        string GetCardType(string tenderCode);

        /// <summary>
        /// Method to save ar payment
        /// </summary>
        /// <param name="arPayment">AR payment</param>
        /// <param name="paymentNumber">Payment number</param>
        void SaveArPayment(AR_Payment arPayment, int paymentNumber);

        /// <summary>
        /// Method to get maximum payment number
        /// </summary>
        /// <returns>Maximum payment number</returns>
        int GetMaxPaymentNumber();

        /// <summary>
        /// Method to update coupon
        /// </summary>
        /// <param name="couponId">Coupon id</param>
        void UpdateCoupon(string couponId);

        /// <summary>
        /// Method to get maximum ROA payment
        /// </summary>
        /// <returns>Maximum value</returns>
        int GetMaxRoaPayment();

        /// <summary>
        /// Method to add ROA payment
        /// </summary>
        /// <param name="payment">Payment</param>
        /// <param name="saleNumber">Sale number</param>
        void AddRoaPayment(Payment payment, int saleNumber);

        /// <summary>
        /// Method to check if card is available
        /// </summary>
        /// <returns>True or false</returns>
        bool IsCardAvailable();

        /// <summary>
        /// Method to remove used gift certificate
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        void RemoveUsedGc(int saleNumber);

        /// <summary>
        /// Method to remove used store credit
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        void RemoveUsedSc(int saleNumber);


        /// <summary>
        /// Method to set coupon to void
        /// </summary>
        /// <param name="couponId">Coupon Id</param>
        void SetCouponToVoid(string couponId);

        /// <summary>
        /// method to get the card code associated with the tender using tendercode
        /// </summary>
        /// <param name="tenderCode"></param>
        /// <returns></returns>
        int GetCardCode(string tenderCode);

        string GetTendClass(string tenderCode);
    }
}

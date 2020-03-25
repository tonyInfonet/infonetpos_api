using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ITenderManager
    {
        /// <summary>
        /// Get Alll Tenders
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="userCode">User code</param>
        /// <param name="blTillClose">Till close</param>
        /// <param name="dropReason">Drop reason</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Tenders</returns>
        Tenders GetAllTender(int saleNumber, int tillNumber, string transactionType, string userCode,
            bool blTillClose, string dropReason, out ErrorMessage errorMessage);


        /// <summary>
        /// Load all tenders
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="blTillClose">Till close</param>
        /// <param name="reasonCode">Reason code</param>
        /// <param name="error">Error</param>
        /// <returns>Tenders</returns>
        Tenders Load(Sale sale, string transactionType, bool blTillClose,
            string reasonCode, out ErrorMessage error);


        /// <summary>
        /// Set Amount Entered
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="sale">Sale</param>
        /// <param name="td">Tenders</param>
        /// <param name="vData">Amount</param>
        /// <param name="limit">Limit</param>
        void Set_Amount_Entered(ref Tenders tenders, ref Sale sale, Tender td,
            decimal vData, decimal limit = 0);



        /// <summary>
        /// Zero Tender
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="sale">Sale</param>
        void Zero_Tenders(ref Tenders tenders, ref Sale sale);

        /// <summary>
        /// Method to update tenders
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="userCode">User code</param>
        /// <param name="blTillClose">Till close</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="amountEntered">Amount entered</param>
        /// <param name="transactionReports"></param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Tenders</returns>
        Tenders UpdateTenders(int saleNumber, int tillNumber, string transactionType, string userCode,
            bool blTillClose, string tenderCode, string amountEntered, out List<Report> transactionReports, 
            out ErrorMessage errorMessage, bool isUpdateTender = false, bool isAmountEnteredManually = false);

        /// <summary>
        /// Method to load temporary tenders
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="till">Till</param>
        /// <param name="sale">Sale</param>
        void Load_Temp_Tenders(ref Tenders tenders, Till till, ref Sale sale);

        /// <summary>
        /// Method to finish a sale
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="till">Till</param>
        /// <param name="issueSc">Issue store credit</param>
        /// <param name="error">Error</param>
        /// <param name="requireOpen">Require open</param>
        /// <param name="changeDue">Change due</param>
        /// <param name="isRefund">Refund sale</param>
        /// <returns>Report content</returns>
        List<Report> Finishing_Sale(Tenders tenders, string transactionType, int saleNumber, string userCode,
             Till till, bool issueSc, out ErrorMessage error, out bool requireOpen,
             out string changeDue, out bool isRefund);

        /// <summary>
        /// Method to complete AR payment
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="till">Till</param>
        /// <param name="issueSc">Issue store credit</param>
        /// <param name="requireOpen">Require open</param>
        /// <param name="changeDue">Change due</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Report content</returns>
        List<Report> Finishing_ARPay(Tenders tenders, int saleNumber, string userCode,
            Till till, bool issueSc, out bool requireOpen, out string changeDue,
            out ErrorMessage errorMessage);

        /// <summary>
        /// Method to cancel tenders
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="transactionType">Trsanction type</param>
        /// <param name="errorMessage">Error</param>
        Sale CancelTender(int saleNumber, int tillNumber, string userCode, string transactionType,
           out ErrorMessage errorMessage);

        /// <summary>
        /// Method to get gift certificates
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="amountEntered">Amount entered</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>List of gift certificates</returns>
        List<GiftCert> GetGiftCertificates(int saleNumber, int tillNumber, string userCode, string tenderCode,
           string amountEntered, string transactionType, out ErrorMessage errorMessage);

        /// <summary>
        /// Method to save gift ecrtificates
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="giftCerts">Gift certificates</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Tenders</returns>
        Tenders SaveGiftCertificate(int saleNumber, int tillNumber, List<GiftCert> giftCerts,
            string transactionType, string userCode, string tenderCode, out ErrorMessage errorMessage);

        /// <summary>
        /// Method to get store credits
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="amountEntered">Amount entered</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>List of store credits</returns>
        List<Store_Credit> GetStoreCredits(int saleNumber, int tillNumber, string userCode, string tenderCode,
           string amountEntered, string transactionType, out ErrorMessage errorMessage);

        /// <summary>
        /// Method to save store credits
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="storeCredits">Store credits</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Tenders</returns>
        Tenders SaveStoreCredits(int saleNumber, int tillNumber, List<Store_Credit> storeCredits,
            string transactionType, string userCode, string tenderCode, out ErrorMessage errorMessage);

        /// <summary>
        /// Method to update tenders with keypad event
        /// </summary>
        /// <param name="allTenders">Tenders</param>
        /// <param name="sale">Sale</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="amountEntered">Amount entered</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="grossTotal">Gross total</param>
        /// <param name="cc">Credit card</param>
        /// <param name="transactionReports">Transaction reports</param>
        /// <param name="errorMessage">Error</param>
        /// <param name="makePayment">Payment</param>
        /// <param name="purchaseOrder">Purchase order</param>
        /// <param name="overrideArLimit">Override AR limit</param>
        /// <returns>Card summary</returns>
        CardSummary SaleTend_Keydown(ref Tenders allTenders, Sale sale, string userCode, string tenderCode, ref string amountEntered,
            string transactionType, decimal grossTotal, Credit_Card cc, out List<Report> transactionReports, out ErrorMessage errorMessage, bool makePayment = false,
            string purchaseOrder = null, bool overrideArLimit = false, bool isUpdateTender = false, bool isAmountEnteredManually = false);

        /// <summary>
        /// Method to update a tender
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="sale">Sale</param>
        /// <param name="till">Till</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="userCode">User code</param>
        /// <param name="blTillClose">Till close</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="tenderedAmount">Tendered amount</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Tenders</returns>
        Tenders UpdateTender(ref Tenders tenders, Sale sale, Till till, string transactionType, string userCode,
            bool blTillClose, string tenderCode, string tenderedAmount, out ErrorMessage errorMessage);


        /// <summary>
        /// Method to save givex sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="cardNumber">Card number</param>
        /// <param name="transactionType">Transction type</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="userCode">User code</param>
        /// <param name="amountEntered">Amount entered</param>
        /// <param name="errorMessage">Error</param>
        /// <param name="stream">Signature</param>
        /// <param name="copies">Copies</param>
        /// <returns>Tenders</returns>
        Tenders SaveGivexSale(int saleNumber, int tillNumber, string cardNumber,
            string transactionType, string tenderCode, string userCode, string amountEntered,
            out ErrorMessage errorMessage, out Report stream, out int copies);

        /// <summary>
        /// Method to find card tender
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <param name="cc">Credit card</param>
        /// <param name="verifyRestriction">Restriction</param>
        /// <returns>Card Information</returns>
        CardSummary FindCardTender(string cardNumber, int saleNumber, int tillNumber,
            string transactionType, string userCode, out ErrorMessage error, ref Credit_Card cc,
            bool verifyRestriction = true, bool verifyAccount = false);

        /// <summary>
        /// Get SaleSummary for AR Account Payment
        /// </summary>
        /// <param name="request">AR payment</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>Sale summary reponse</returns>
        SaleSummaryResponse SaleSummaryForArPayment(ArPaymentRequest request, string userCode,
           out ErrorMessage error);

        /// <summary>
        /// Method to get gross total
        /// </summary>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="sale">Sale</param>
        /// <param name="payment">Fleet payment</param>
        /// <param name="arPayment">AR payment</param>
        /// <param name="prepayItem">Prepay item</param>
        /// <param name="displayNoReceiptButton">Display no receipt</param>
        /// <returns>Gros  total</returns>
        decimal GetGrossTotal(string transactionType, Sale sale, Payment payment,
            AR_Payment arPayment, short prepayItem, out bool displayNoReceiptButton);

        /// <summary>
        /// Method to test if cards are defined
        /// </summary>
        /// <returns>True or false</returns>
        bool AreCardsDefined();

        /// <summary>
        /// Method to get sale summary for fleet payment
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="amount">Amount</param>
        /// <param name="isSwiped">Swiped or not</param>
        /// <param name="userCode">User code</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="error">Error</param>
        /// <returns>Sale summary repsonse</returns>
        SaleSummaryResponse SaleSummaryForFleetPayment(string cardNumber, decimal amount,
            bool isSwiped, string userCode, int tillNumber, int saleNumber, out ErrorMessage error);

        /// <summary>
        /// Method to finish payment using fleet
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="till">Till</param>
        /// <param name="issueSc">Issue store credit</param>
        /// <param name="requireOpen">Require open</param>
        /// <param name="changeDue">Change due</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Report content</returns>
        List<Report> Finishing_Payment(Tenders tenders, int saleNumber, string userCode,
            Till till, bool issueSc, out bool requireOpen, out string changeDue, out ErrorMessage errorMessage);

        /// <summary>
        /// Method to save profile prompt
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="cardNumber">Card number</param>
        /// <param name="profileId">Profile ID</param>
        /// <param name="cardPrompts">Card prompts</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>PO number</returns>
        string SaveProfilePrompt(int saleNumber, int tillNumber, string cardNumber,
            string profileId, List<CardPrompt> cardPrompts, string userCode, out ErrorMessage error);

        /// <summary>
        /// Method to get issue store credit message
        /// </summary>
        /// <param name="tenders">Tenders</param>
        /// <returns>Store credit message</returns>
        string IssueStoreCredit(Tenders tenders);

        /// <summary>
        /// Method to validate multi PO error message
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="purchaseOrder">Purchase order</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>True or false</returns>
        bool ValidateMuliPo(int saleNumber, int tillNumber, string purchaseOrder, out ErrorMessage
            errorMessage);
    }
}
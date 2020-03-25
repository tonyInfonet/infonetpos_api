using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;
using System.IO;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IPaymentManager
    {
        /// <summary>
        /// Method to pay by exact cash
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="userCode">User code</param>
        /// <param name="fileName">File name</param>
        /// <param name="errorMessage">Error</param>
        /// <param name="receipt">Receipt</param>
        /// <param name="lcdMsg">Customer display message</param>
        /// <returns>New sale</returns>
        Sale ByCashExact(int tillNumber, int saleNumber, string userCode, ref Report 
            receipt, ref string fileName,
            out ErrorMessage errorMessage, out CustomerDisplay lcdMsg);

        /// <summary>
        /// Method to complete payment as run away
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error</param>
        /// <param name="receipt">Receipt content</param>
        /// <returns>Sale</returns>
        Sale RunAway(int saleNumber, int tillNumber, string userCode, ref Report 
            receipt, out ErrorMessage errorMessage);
        
        /// <summary>
        /// Method to get exact change receipt
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="user">User</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="errorMessage">Error</param>
        /// <param name="signature">Signature</param>
        /// <param name="fileName">File name</param>
        /// <returns>Report content</returns>
        Report ExactChange_Receipt(Sale sale, User user, int tillNumber, out ErrorMessage errorMessage,
            out Stream signature, ref string fileName);

        /// <summary>
        /// Method to perform payment by credit/debit/fleet
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="cardNumber">Card number</param>
        /// <param name="userCode">User code</param>
        /// <param name="amountUsed">Amount used</param>
        /// <param name="merchantFileStream">Merchant file</param>
        /// <param name="customerFileStream">Customer file</param>
        /// <param name="errorMessage">Error</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="poNumber">PO number</param>
        /// <returns>Tenders</returns>
        Tenders ByCard(int saleNumber, int tillNumber, string cardNumber, string userCode,string transactionType,
           string poNumber,string amountUsed, ref Report merchantFileStream, ref Report customerFileStream, out ErrorMessage errorMessage);

        /// <summary>
        /// Method to perform payment by coupon tender
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="couponId">Coupon Id</param>
        /// <param name="blTillClose">Till close</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Tenders</returns>
        Tenders ByCoupon(int saleNumber, int tillNumber, string transactionType, string couponId,
            bool blTillClose, string tenderCode, string userCode,
            out ErrorMessage errorMessage);

        /// <summary>
        /// Method to perform a transaction by account
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="amountTend">Amount tendered</param>
        /// <param name="tillClose">Close till</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="userCode">User code</param>
        /// <param name="purchaseOrder">Purchase order</param>
        /// <param name="overrideArLimit">Override ar limit</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Tenders</returns>
        Tenders ByAccount(int saleNumber, int tillNumber, string transactionType, string amountTend,
            bool tillClose, string tenderCode, string userCode, string purchaseOrder, bool overrideArLimit,
            out ErrorMessage errorMessage);

        /// <summary>
        /// Method to verify payment by account
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="amountEntered">Amount entered</param>
        /// <param name="tillClose">Close till</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>Payment by account messages</returns>
        VerifyPaymentByAccount VerifyPaymentByAccount(int saleNumber, int tillNumber, string transactionType,
            string amountEntered, bool tillClose, string tenderCode, string userCode,
            out ErrorMessage errorMessage);

        /// <summary>
        /// Method to get sale vendor coupon 
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="coupon">Coupon</param>
        /// <param name="error">Error</param>
        /// <returns>List of sale vendor coupon</returns>
        List<VCoupon> GetSaleVendorCoupon(int saleNumber, int tillNumber, string userCode,
           string tenderCode,ref string coupon,out ErrorMessage error);

        /// <summary>
        /// Method to add sale vendor coupon
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="coupon">Coupon</param>
        /// <param name="serialNumber">Serial number</param>
        /// <param name="error">Error</param>
        /// <returns>List of sale vendor coupon</returns>
        List<VCoupon> AddSaleVendorCoupon(int saleNumber, int tillNumber, string userCode,
          string tenderCode,string coupon, string serialNumber, out ErrorMessage error);

        /// <summary>
        /// Method to remove sale vendor coupon
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="coupon">Coupon</param>
        /// <param name="error">Error</param>
        /// <returns>List of sale vendor coupon</returns>
        List<VCoupon> RemoveSaleVendorCoupon(int saleNumber, int tillNumber, string userCode,
          string tenderCode, string coupon, out ErrorMessage error);

        /// <summary>
        /// Method to perform transaction by sale vendor coupon
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="tenderCode">Tender code</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>Tenders</returns>
        Tenders PaymentByVCoupon(int saleNumber, int tillNumber, string tenderCode,
            string userCode, out ErrorMessage error);

        /// <summary>
        /// Method to check till limit and verify exceed limit message
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Till limit message</returns>
        string CheckTillLimit(int tillNumber);

        /// <summary>
        /// Method to complete payment
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="transactionType">Transaction type</param>
        /// <param name="userCode">User code</param>
        /// <param name="issueSc">Issue store credit</param>
        /// <param name="error">Error</param>
        /// <param name="openCashDrawer">Open cash drawer</param>
        /// <param name="changeDue">Change due</param>
        /// <param name="isRefund">Refund sale</param>
        /// <param name="lcdDisplay">LCD display</param>
        /// <returns>Report content</returns>
        List<Report> CompletePayment(int saleNumber, int tillNumber, string transactionType,
            string userCode, bool issueSc, out ErrorMessage error, out bool openCashDrawer,
            out string changeDue, out bool isRefund, out CustomerDisplay lcdDisplay);

        /// <summary>
        /// Method to complete a transaction as pump test
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="userCode">User code</param>
        /// <param name="newSale">New sale</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Report content</returns>
        Report CompletePumpTest(int saleNumber, int tillNumber, string userCode,out Sale newSale,
            out ErrorMessage errorMessage);


        string KickbackCommunicationError();
    }
}

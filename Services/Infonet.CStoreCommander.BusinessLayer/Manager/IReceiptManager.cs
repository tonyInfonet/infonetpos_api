using System;
using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;
using System.IO;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IReceiptManager
    {
        //Tony 03/19/2019
        List<string> GetReceiptHeader();
        //End
        /// <summary>
        /// Method to print bottle return
        /// </summary>
        /// <param name="brPay">Bottle return payment</param>
        /// <param name="userName">User name</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="saleTime">Sale time</param>
        /// <param name="registerNum">Register number</param>
        /// <param name="tillNum">Till number</param>
        /// <param name="tillShift">Shift number</param>
        /// <param name="reprint">Reprint or not</param>
        /// <returns>Report content</returns>
        Report Print_BottleReturn(BR_Payment brPay, string userName, DateTime saleDate,
             DateTime saleTime, short registerNum, short tillNum, short tillShift,
             bool reprint = false);

        /// <summary>
        /// Method to get all departments
        /// </summary>
        /// <returns>List of departments</returns>
        Dictionary<string, string> GetAllDepartmets();

        /// <summary>
        /// Method to print sales count summary report
        /// </summary>
        /// <param name="department">Department id</param>
        /// <param name="tillNumber">TIll number</param>
        /// <param name="shiftNumber">Shiftnumber</param>
        /// <param name = "loggedTill">Logged till</param>
        FileStream PrintSaleCountReport(string department, int tillNumber, int shiftNumber, Till loggedTill);

        /// <summary>
        /// Method to get totals for flash report
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Byte</returns>
        FlashReportTotals GetTotalsForFlashReport(int tillNumber);

        /// <summary>
        /// Get list of departments and the net sales
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Bytes</returns>
        List<Department> GetDepartmentDetailsForFlashReport(int tillNumber);

        /// <summary>
        /// Method to print flash report
        /// </summary>
        /// <param name="till">Till</param>
        /// <param name="departments">All departments</param>
        /// <param name="totals">Flash report totals</param>
        /// <returns>File stream</returns>
        FileStream PrintFlashReport(Till till, FlashReportTotals totals, List<Department> departments);

        /// <summary>
        /// Method to print till audit report
        /// </summary>
        /// <param name="till">Till</param>
        /// <returns>File stream</returns>
        FileStream PrintTillAuditReport(Till till);

        /// <summary>
        /// Method to validate critera to get a report
        /// </summary>
        /// <param name="departmentId">Department id</param>
        /// <param name="tillNumber">TIll number</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <returns>True or false</returns>
        MessageStyle IsValidateReportCriteria(string departmentId, int tillNumber, int shiftNumber);

        /// <summary>
        /// Method to print receipt
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <param name="tenders">Tenders</param>
        /// <param name="printIt">Print or not</param>
        /// <param name="fileName">File name</param>
        /// <param name="reprint">Reprint</param>
        /// <param name="signature">Signature</param>
        /// <param name="userName">username</param>
        /// <param name="completePrepay">Compelte prepay</param>
        /// <param name="reprintCards">Reprint cards</param>
        /// <param name="loggedTill">Logged till number</param>
        /// <param name="storeCredit">Store credit</param>
        Report Print_Receipt(int loggedTill, Store_Credit storeCredit, ref Sale sale, 
            ref Tenders tenders, bool printIt, ref string fileName, ref bool reprint,
            out Stream signature, string userName, bool completePrepay = false, 
            Reprint_Cards reprintCards = null);

        /// <summary>
        /// Method to find if any flas report is available or not
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>True or false</returns>
        bool IsFlashReportAvailable(int tillNumber);


        /// <summary>
        /// Method to find if a user can audit till or not
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <returns>True or false</returns>
        bool UserCanAuditTill(string userCode);

        /// <summary>
        /// Method to print cash draw
        /// </summary>
        /// <param name="till">Till</param>
        /// <param name="registerNumber">Register Number</param>
        /// <param name="userCode">User</param>
        /// <param name="coins">Coins</param>
        /// <param name="bills">Bills</param>
        /// <param name="returnReason">Return Reason</param>
        /// <param name="totalAmount">Total amount</param>
        /// <returns></returns>
        FileStream Print_Draw(Till till, short registerNumber, string userCode, List<Cash> coins,
            List<Cash> bills, Return_Reason returnReason, decimal totalAmount);

        /// <summary>
        /// Method to print cash drop
        /// </summary>
        /// <param name="tenders">List of tenders</param>
        /// <param name="till">Till</param>
        /// <param name="user">User</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="cashDrop">Cash drop</param>
        /// <param name="cntDrop"> Count drop</param>
        /// <returns>Stream</returns>
        FileStream PrintDrop(Tenders tenders, Till till, User user, byte registerNumber,
            CashDrop cashDrop, short cntDrop);

        /// <summary>
        /// Method to print gift cert payment
        /// </summary>
        /// <param name="gcPayment">Gift cert</param>
        /// <param name="printIt">Print </param>
        /// <param name="reprint">Reprint</param>
        /// <param name="copies">Copies</param>
        /// <param name="sameSale">Same sale</param>
        FileStream PrintGiftCard(GCPayment gcPayment,bool printIt = true, bool reprint = false, 
            short copies = 1,bool sameSale = false);

        /// <summary>
        /// Method to issue store credit receipt
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="customerCode">Customer code</param>
        /// <param name="user">User</param>
        /// <param name="tender">Tender</param>
        /// <param name="amount">Amount</param>
        /// <param name="storeCredit">Store credit</param>
        /// <returns>Report content</returns>
        Report Issue_Store_Credit(int saleNumber, string customerCode,User user, Tender tender, 
            float amount, out Store_Credit storeCredit);

        /// <summary>
        /// Method to print givex receipt
        /// </summary>
        /// <param name="gxReceipt">Givex receipt</param>
        /// <param name="printIt">Print it or not</param>
        /// <param name="reprint">Reprint it or not</param>
        /// <param name="copies">copies</param>
        /// <param name="sameSale">Same sale or not</param>
        /// <returns>Report content</returns>
        Report Print_GiveX_Receipt(GiveXReceiptType gxReceipt, bool printIt = true, bool reprint = false, short copies = 1,
            bool sameSale = false);

        /// <summary>
        /// Method to print AR pay
        /// </summary>
        /// <param name="arPay">AR payment</param>
        /// <param name="userCode">User code</param>
        /// <param name="till">Till</param>
        /// <param name="tenders">Tenders</param>
        /// <param name="printIt">Print it or not</param>
        /// <param name="reprint">Reprint it or not</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="saleTime">Sale time</param>
        /// <param name="reprintCards">Reprint cards</param>
        /// <returns>Report content</returns>
        Report Print_ARPay(AR_Payment arPay, string userCode, Till till, Tenders tenders = null,
            bool printIt = false, bool reprint = false, DateTime saleDate = default(DateTime),
            DateTime saleTime = default(DateTime), Reprint_Cards reprintCards = null);

        /// <summary>
        /// Method to print transaction record in french
        /// </summary>
        /// <param name="tc">Credit card</param>
        /// <param name="maskCard">Mask card number</param>
        /// <param name="printIt">Print it or not</param>
        /// <param name="reprint">Reprint it or not</param>
        /// <param name="merchantCopy">Merchant copy or not</param>
        /// <returns>Report content</returns>
        Report PrintTransRecordFrench(Credit_Card tc, bool maskCard, bool printIt, 
            bool reprint, bool merchantCopy);

        /// <summary>
        /// Method to print transaction record in english
        /// </summary>
        /// <param name="tc">Credit card</param>
        /// <param name="maskCard">Mask card number</param>
        /// <param name="printIt">Print it or not</param>
        /// <param name="reprint">Reprint it or not</param>
        /// <param name="merchantCopy">Merchant copy or not</param>
        /// <returns>Report content</returns>
        Report PrintTransRecordEnglish(Credit_Card tc, bool maskCard, bool printIt,
            bool reprint, bool merchantCopy);

        /// <summary>
        /// Method to print prepay
        /// </summary>
        /// <param name="prepay">Prepay</param>
        /// <param name="user">User</param>
        /// <param name="storeCredit">Store credit</param>
        /// <param name="prepayTenders">Prepay tenders</param>
        /// <returns>Report content</returns>
        FileStream PrintPrepay(Prepay prepay, User user, Store_Credit storeCredit, 
            Tenders prepayTenders);

        /// <summary>
        /// Method to reprint pay at pump
        /// </summary>
        /// <param name="payPump">Pay at pump</param>
        /// <param name="cCard">Credit card</param>
        /// <param name="fileName">File name</param>
        /// <returns>Report content</returns>
        FileStream RePrintPayAtPump(PayAtPump payPump, Card_Reprint cCard, string fileName);


        /// <summary>
        /// Method to print payout
        /// </summary>
        /// <param name="po">payout</param>
        /// <param name="userCode">User code</param>
        /// <param name="userName">User name</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="saleTime">Sale time</param>
        /// <param name="registerNum">Register number</param>
        /// <param name="till">Till</param>
        /// <param name="rePrint">Reprint or not</param>
        /// <returns>Report content</returns>
        Report Print_Payout(Payout po, string userCode, string userName, DateTime saleDate,
            DateTime saleTime, short registerNum, Till till, bool rePrint = false);

        /// <summary>
        /// Method to print payment
        /// </summary>
        /// <param name="till">Till</param>
        /// <param name="payment">Payment</param>
        /// <param name="tenders">Tenders</param>
        /// <param name="user">User</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="saleTime">Sale time</param>
        /// <param name="reprint">Reprint</param>
        /// <param name="reprintCards">Reprint cards</param>
        /// <returns>Report content</returns>
        Report Print_Payment(Till till, Payment payment, Tenders tenders, User user,
            DateTime saleDate, DateTime saleTime, bool reprint = false, 
            Reprint_Cards reprintCards = null);


        /// <summary>
        /// Method to get reprint reports
        /// </summary>
        /// <returns>List of reprint reports</returns>
        List<ReprintReport> GetReprintReports();

        /// <summary>
        /// Method to get reprint sales
        /// </summary>
        /// <param name="reportName">Report name</param>
        /// <param name="date">Date</param>
        /// <param name="error">Error</param>
        /// <returns>Reprint sale</returns>
        ReprintSale GetReprintSales(string reportName, DateTime? date,
            out ErrorMessage error);

        /// <summary>
        /// Method to get report
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="date">Sale date</param>
        /// <param name="reportName">Report name</param>
        /// <param name="fileName">File name</param>
        /// <param name="error">Error</param>
        /// <returns>Report content</returns>
        List<Report> GetReport(int saleNumber, DateTime? date,
            string reportName, out string fileName, out ErrorMessage error);

        /// <summary>
        /// Method to print tax exempt voucher
        /// </summary>
        /// <param name="oteSale">Tax exempt sale</param>
        /// <param name="reprint">Reprint</param>
        /// <param name="printIt">Print or not</param>
        /// <returns>Report content</returns>
        Report PrintTaxExemptVoucher(TaxExemptSale oteSale, bool reprint = false, 
            bool printIt = true);

        /// <summary>
        /// Method to print givex close report
        /// </summary>
        /// <param name="cashoutId">Cashout Id</param>
        /// <param name="printIt">Print or not</param>
        /// <param name="reprint">Reprint</param>
        /// <returns></returns>
        Report PrintGiveXClose(string cashoutId, bool printIt, ref bool reprint);


        /// <summary>
        /// Method to print Kickback Points
        /// </summary>
        /// <param name="points">Points</param>
        /// <returns>File stream</returns>
        FileStream PrintKickbackPoints(int points);



        /// <summary>
        /// Method to make Kickback Points Report
        /// </summary>
        /// <param name="strFileName">Filename</param>
        /// <param name="points">Kickback Points</param>
        /// <returns>File stream</returns>
        FileStream Kickback_Report(string strFileName, int points);


        Report Print_Kickback(Sale sale);
        FileStream Print_KickbackReceipt(string strFileName, Sale sale);

    }
}

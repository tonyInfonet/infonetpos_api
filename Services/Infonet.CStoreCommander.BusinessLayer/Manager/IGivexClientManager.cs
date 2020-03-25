using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IGivexClientManager
    {
        /// <summary>
        /// Activate GiveX
        /// </summary>
        /// <param name="strCardNum">Card number</param>
        /// <param name="amount">Amount</param>
        /// <param name="saleNum">Sale number</param>
        /// <param name="refNum">Reference number</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="givexReceipt"></param>
        /// <returns>true/false</returns>
        bool ActivateGiveX(string strCardNum, float amount, int saleNum, ref string refNum,
            string userCode, out ErrorMessage errorMessage, out GiveXReceiptType givexReceipt);

        /// <summary>
        /// Adjust givex
        /// </summary>
        /// <param name="strCardNum">Card number</param>
        /// <param name="amount">Amount</param>
        /// <param name="saleNum">Sale number</param>
        /// <param name="newBalance">New balance</param>
        /// <param name="refNum">Reference number</param>
        /// <param name="expDate">Expiry date</param>
        /// <param name="result">Result</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <returns>True or false</returns>
        bool AdjustGiveX(string strCardNum, decimal amount, int saleNum, ref decimal newBalance,
            ref string refNum, ref string expDate,ref string result, string userCode, 
            out ErrorMessage errorMessage, out GiveXReceiptType givexReceipt);

        /// <summary>
        /// Deactivate Givex card
        /// </summary>
        /// <param name="strCardNum">Card number</param>
        /// <param name="amount">Amount</param>
        /// <param name="saleNum">Sale number</param>
        /// <param name="refNum">Reference number</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <returns>True or false</returns>
        bool DeactivateGiveX(string strCardNum, decimal amount, int saleNum, string refNum,
            string userCode, out ErrorMessage errorMessage ,out GiveXReceiptType givexReceipt);

        /// <summary>
        /// Check givex balance
        /// </summary>
        /// <param name="strCardNum">Car number</param>
        /// <param name="saleNum">Sale number</param>
        /// <param name="balance">Balance</param>
        /// <param name="cardStatus">Card status</param>
        /// <param name="expDate">Expiry date</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <returns>True or false</returns>
        bool GiveX_Balance(string strCardNum, int saleNum, ref decimal balance, ref string cardStatus, ref string expDate,
            string userCode, out ErrorMessage errorMessage, out GiveXReceiptType givexReceipt);

        /// <summary>
        /// Increase givex amount
        /// </summary>
        /// <param name="strCardNum">Card number</param>
        /// <param name="amount">Amount</param>
        /// <param name="saleNum">Sale number</param>
        /// <param name="newBalance">New balance</param>
        /// <param name="refNum">Reference number</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <returns>True or false</returns>
        bool IncreaseGiveX(string strCardNum, float amount, int saleNum, ref decimal newBalance, ref string refNum,
            string userCode, out ErrorMessage errorMessage, out GiveXReceiptType givexReceipt);

        /// <summary>
        /// GiveX batch Close
        /// </summary>
        /// <param name="saleNum">Sale number</param>
        /// <param name="cashoutId">Cash out id</param>
        /// <param name="report">Report</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>True or false</returns>
        bool GiveX_Close(int saleNum, ref string cashoutId, ref string report,
            string userCode, out ErrorMessage errorMessage);

        /// <summary>
        /// Method to redeem givex
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="strCardNum">Card number</param>
        /// <param name="amount">Amount</param>
        /// <param name="securityCode">Security code</param>
        /// <param name="saleNum">Sale number</param>
        /// <param name="newBalance">New balance</param>
        /// <param name="refNum">Reference number</param>
        /// <param name="expDate">Expiry date</param>
        /// <param name="result">Result</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <returns>Trur or false</returns>
        bool RedeemGiveX(string userCode, string strCardNum, float amount, string securityCode,
                    int saleNum, ref decimal newBalance, ref string refNum, ref string expDate,
                    ref string result, out ErrorMessage errorMessage, out GiveXReceiptType givexReceipt);
    }
}

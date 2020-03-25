using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class GivexClientManager : ManagerBase, IGivexClientManager
    {
        private readonly IApiResourceManager _resourceManager;
        private readonly Dictionary<string, object> _givexPolicies;
        private readonly short _offSet;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="policyManager"></param>
        public GivexClientManager(IApiResourceManager resourceManager, IPolicyManager
            policyManager)
        {
            _resourceManager = resourceManager;
            _offSet = policyManager.LoadStoreInfo().OffSet;
            _givexPolicies = new Dictionary<string, object>
            {
                {"GIVEX_PASS", policyManager.GIVEX_PASS},
                {"GIVEX_USER", policyManager.GIVEX_USER},
                {"GIVEX_IP", policyManager.GIVEX_IP},
                {"GIVEX_PORT", policyManager.GIVEX_PORT},
                {"GIVETIMEOUT", policyManager.GIVETIMEOUT},
                {"GiveXMerchID", policyManager.GiveXMerchID},
                {"AlwAdjGiveX", policyManager.AlwAdjGiveX}
            };
        }

        /// <summary>
        /// Activate GiveX
        /// </summary>
        /// <param name="strCardNum">Card number</param>
        /// <param name="amount">Amount</param>
        /// <param name="saleNum">Sale number</param>
        /// <param name="refNum">Reference number</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <returns>true/false</returns>
        public bool ActivateGiveX(string strCardNum, float amount, int saleNum, ref string refNum,
            string userCode, out ErrorMessage errorMessage, out GiveXReceiptType givexReceipt)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,GivexClientManager,ActivateGiveX,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var returnValue = false;
            errorMessage = new ErrorMessage();
            var response = new Variables.GiveXResponseType();
            givexReceipt = new GiveXReceiptType();
            refNum = "";
            if (string.IsNullOrEmpty(strCardNum))
            {
                return false;
            }

            modStringPad.InitGiveXRequest();
            Variables.GX_Request.ServiceType = "Activation";

            Variables.GX_Request.UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]);

            Variables.GX_Request.UserPassword = Convert.ToString(_givexPolicies["GIVEX_PASS"]);

            Variables.GX_Request.OperatorID = Convert.ToString(userCode); // "ALL"
            Variables.GX_Request.GivexNumber = strCardNum.Trim();
            Variables.GX_Request.Language = "0";
            Variables.GX_Request.Amount = (amount).ToString(CultureInfo.InvariantCulture);
            Variables.GX_Request.TransactionCode = (saleNum).ToString();
            Variables.GX_Request.TransmissionDate = DateAndTime.Today.ToString("MM/dd/yyyy");
            Variables.GX_Request.TransmissionTime = DateAndTime.TimeOfDay.ToString("hh:mm:ss");
            Variables.GX_Request.TerminalID = "ALL";
            var strSend = modStringPad.GetGiveXRequest();

            if (SendGiveXRequest(strSend, "Activation," + (saleNum), "Activation", ref response, out errorMessage))
            {
                if (response.Result != "Approved")
                {
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, response.TransactionReference.Trim() != "" ? 3265 : 3264, 99, response.TransactionReference, CriticalOkMessageType);
                    errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                    return false;
                }
                if (response.GivexNumber != strCardNum.Trim())
                {
                    //Call DisplayMsgForm("Invalid response from GiveX TPS, Gift Certificate is canceled!", 99) 
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3264, 99,
                        response.TransactionReference, CriticalOkMessageType);
                }
                else
                {
                    returnValue = true;
                    refNum = response.TransactionReference;



                    modStringPad.InitGiveXReceipt();
                    Variables.GX_Receipt.Date = DateAndTime.Today.ToString("MM/dd/yyyy");
                    Variables.GX_Receipt.Time = DateAndTime.TimeOfDay.ToString("hh:mm:ss");

                    Variables.GX_Receipt.UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]);
                    Variables.GX_Receipt.TranType = 1; //Activation
                    Variables.GX_Receipt.SaleNum = saleNum;
                    Variables.GX_Receipt.SeqNum = response.TransactionReference;
                    Variables.GX_Receipt.CardNum = response.GivexNumber;
                    Variables.GX_Receipt.SaleAmount = float.Parse(response.Amount);
                    Variables.GX_Receipt.ExpDate = string.Format(response.ExpiryDate, "yyMM");
                    Variables.GX_Receipt.Balance = (float)(Conversion.Val(response.CertificateBalance));
                    Variables.GX_Receipt.PointBalance = (float)(Conversion.Val(response.PointsBalance));
                    Variables.GX_Receipt.ResponseCode = response.Result;

                    givexReceipt = new GiveXReceiptType
                    {
                        Date = DateAndTime.Today.ToString("MM/dd/yyyy"),
                        Time = DateAndTime.TimeOfDay.ToString("hh:mm:ss"),
                        UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]),
                        TranType = 1, //Activation
                        SaleNum = saleNum,
                        SeqNum = response.TransactionReference,
                        CardNum = response.GivexNumber,
                        SaleAmount = float.Parse(response.Amount),
                        ExpDate = string.Format(response.ExpiryDate, "yyMM"),
                        Balance = (float)(Conversion.Val(response.CertificateBalance)),
                        PointBalance = (float)(Conversion.Val(response.PointsBalance)),
                        ResponseCode = response.Result,
                    };
                }
            }
            Performancelog.Debug($"End,GivexClientManager,ActivateGiveX,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

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
        public bool AdjustGiveX(string strCardNum, decimal amount, int saleNum, ref decimal newBalance,
            ref string refNum, ref string expDate, ref string result, string userCode,
            out ErrorMessage errorMessage, out GiveXReceiptType givexReceipt)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,GivexClientManager,AdjustGiveX,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var returnValue = false;

            errorMessage = new ErrorMessage();

            var response = new Variables.GiveXResponseType();

            var capValue = new object[3];
            givexReceipt = new GiveXReceiptType();
            newBalance = 0;
            if (strCardNum.Trim() == "")
            {
                return false;
            }

            modStringPad.InitGiveXRequest();
            Variables.GX_Request.ServiceType = "Adjustment";

            Variables.GX_Request.UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]);

            Variables.GX_Request.UserPassword = Convert.ToString(_givexPolicies["GIVEX_PASS"]);

            Variables.GX_Request.OperatorID = userCode; // "ALL"
            Variables.GX_Request.GivexNumber = strCardNum.Trim();
            Variables.GX_Request.Language = "0";
            Variables.GX_Request.Amount = (amount).ToString(CultureInfo.InvariantCulture);
            Variables.GX_Request.TransactionCode = (saleNum).ToString();
            Variables.GX_Request.TransmissionDate = DateAndTime.Today.ToString("MM/dd/yyyy");
            Variables.GX_Request.TransmissionTime = DateAndTime.TimeOfDay.ToString("hh:mm:ss");
            Variables.GX_Request.TerminalID = "ALL";
            var strSend = modStringPad.GetGiveXRequest();

            if (SendGiveXRequest(strSend, "Adjustment," + saleNum, "Adjustment", ref response, out errorMessage))
            {
                if (response.Result != "Approved")
                {
                    if (!string.IsNullOrEmpty(response.TransactionReference))
                    {
                        if (response.TransactionReference.Trim() == "")
                        {
                            capValue[1] = "";
                        }
                        else
                        {
                            capValue[1] = "(" + response.TransactionReference.Trim() + ") ";
                        }
                    }
                    else
                    {
                        capValue[1] = "";
                    }

                    capValue[2] = strCardNum.Trim();
                    //Call DisplayMsgForm("Invalid response {}from GiveX TPS, CANNOT adjust the amount for GiveX card {}.", 99)
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3267, 99, capValue, CriticalOkMessageType);
                    errorMessage.StatusCode = HttpStatusCode.NotAcceptable;

                    return false;
                }
                if (response.GivexNumber != strCardNum.Trim())
                {
                    capValue[1] = "";
                    capValue[2] = strCardNum.Trim();

                    // Call DisplayMsgForm("Invalid response {}from GiveX TPS, CANNOT adjust the amount for GiveX card {}.", 99)
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3267, 99, null, CriticalOkMessageType);
                    errorMessage.StatusCode = HttpStatusCode.NotAcceptable;

                    return false;
                }
                newBalance = (decimal)(Conversion.Val(response.CertificateBalance));
                refNum = response.TransactionReference;
                expDate = String.Format(response.ExpiryDate, "yymm");
                result = response.Result;
                returnValue = true;

                modStringPad.InitGiveXReceipt();
                Variables.GX_Receipt.Date = DateAndTime.Today.ToString("MM/dd/yyyy");
                Variables.GX_Receipt.Time = DateAndTime.TimeOfDay.ToString("hh:mm:ss");

                Variables.GX_Receipt.UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]);
                Variables.GX_Receipt.TranType = 3; //Adjustment
                Variables.GX_Receipt.SeqNum = (saleNum).ToString();
                Variables.GX_Receipt.CardNum = response.GivexNumber;
                Variables.GX_Receipt.ExpDate = string.Format(response.ExpiryDate, "yymm");
                Variables.GX_Receipt.Balance = (float)(Conversion.Val(response.CertificateBalance));
                Variables.GX_Receipt.SaleAmount = (float)amount;
                Variables.GX_Receipt.PointBalance = (float)(Conversion.Val(response.PointsBalance));
                Variables.GX_Receipt.ResponseCode = response.Result;
            }

            givexReceipt = new GiveXReceiptType
            {
                Date = DateAndTime.Today.ToString("MM/dd/yyyy"),
                Time = DateAndTime.TimeOfDay.ToString("hh:mm:ss"),
                UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]),
                TranType = 3,//Adjustment
                SeqNum = (saleNum).ToString(),
                CardNum = response.GivexNumber,
                ExpDate = string.Format("yyMM", response.ExpiryDate),
                Balance = (float)(Conversion.Val(response.CertificateBalance)),
                SaleAmount = (float)amount,
                PointBalance = (float)(Conversion.Val(response.PointsBalance)),
                ResponseCode = response.Result
            };
            Performancelog.Debug($"End,GivexClientManager,AdjustGiveX,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

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
        public bool DeactivateGiveX(string strCardNum, decimal amount, int saleNum, string refNum,
            string userCode, out ErrorMessage errorMessage, out GiveXReceiptType givexReceipt)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,GivexClientManager,DeactivateGiveX,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            givexReceipt = new GiveXReceiptType();
            var returnValue = false;
            errorMessage = new ErrorMessage();

            var response = new Variables.GiveXResponseType();

            var capValue = new object[3];

            if (strCardNum?.Trim() == "" || refNum?.Trim() == "")
            {
                return false;
            }

            modStringPad.InitGiveXRequest();
            Variables.GX_Request.ServiceType = "Cancel";

            Variables.GX_Request.UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]);

            Variables.GX_Request.UserPassword = Convert.ToString(_givexPolicies["GIVEX_PASS"]);

            Variables.GX_Request.OperatorID = Convert.ToString(userCode); // "ALL"
            Variables.GX_Request.GivexNumber = strCardNum.Trim();
            Variables.GX_Request.Language = "0";
            Variables.GX_Request.Amount = (amount).ToString(CultureInfo.InvariantCulture);
            Variables.GX_Request.TransactionCode = (saleNum).ToString();
            Variables.GX_Request.TransmissionDate = DateAndTime.Today.ToString("MM/dd/yyyy");
            Variables.GX_Request.TransmissionTime = DateAndTime.TimeOfDay.ToString("hh:mm:ss");
            Variables.GX_Request.TerminalID = "ALL";
            Variables.GX_Request.TransactionReference = refNum.Trim();
            var strSend = modStringPad.GetGiveXRequest();

            if (SendGiveXRequest(strSend, "Cancel," + (saleNum), "Cancel", ref response, out errorMessage))
            {
                if (response.Result != "Approved")
                {
                    if (!string.IsNullOrEmpty(response.TransactionReference))
                    {
                        if (response.TransactionReference.Trim() == "")
                        {
                            capValue[1] = "";
                        }
                        else
                        {
                            capValue[1] = "(" + response.TransactionReference.Trim() + ") ";
                        }
                    }
                    else
                    {
                        capValue[1] = "";
                    }

                    capValue[2] = strCardNum.Trim();

                    //Call DisplayMsgForm("Invalid response {}from GiveX TPS, CANNOT deactivate GiveX card {}.", 99)
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3266, 99, capValue, CriticalOkMessageType);

                    return false;
                }
                if (response.GivexNumber != strCardNum.Trim())
                {
                    capValue[1] = "";

                    capValue[2] = strCardNum.Trim();
                    //Call DisplayMsgForm("Invalid response {}from GiveX TPS, CANNOT deactivate GiveX card {}.", 99)
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3266, 99, capValue, CriticalOkMessageType);
                    return false;
                }
                returnValue = true;


                modStringPad.InitGiveXReceipt();
                Variables.GX_Receipt.Date = DateAndTime.Today.ToString("MM/dd/yyyy");
                Variables.GX_Receipt.Time = DateAndTime.TimeOfDay.ToString("hh:mm:ss");

                Variables.GX_Receipt.UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]);
                Variables.GX_Receipt.TranType = 2; //Cancel
                Variables.GX_Receipt.SaleNum = saleNum;
                Variables.GX_Receipt.SeqNum = response.TransactionReference;
                Variables.GX_Receipt.CardNum = response.GivexNumber;
                Variables.GX_Receipt.SaleAmount = float.Parse(response.Amount);
                Variables.GX_Receipt.ExpDate = string.Format(response.ExpiryDate, "yymm");
                Variables.GX_Receipt.Balance = (float)(Conversion.Val(response.CertificateBalance));
                Variables.GX_Receipt.PointBalance = (float)(Conversion.Val(response.PointsBalance));
                Variables.GX_Receipt.ResponseCode = response.Result;
            }

            givexReceipt = new GiveXReceiptType
            {
                Date = DateAndTime.Today.ToString("MM/dd/yyyy"),
                Time = DateAndTime.TimeOfDay.ToString("hh:mm:ss"),
                UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]),
                TranType = 2,//Adjustment
                SaleNum = saleNum,
                SeqNum = response.TransactionReference,
                CardNum = response.GivexNumber,
                SaleAmount = float.Parse(response.Amount),
                ExpDate = string.Format(response.ExpiryDate, "yyMM"),
                Balance = (float)(Conversion.Val(response.CertificateBalance)),
                PointBalance = (float)(Conversion.Val(response.PointsBalance)),
                ResponseCode = response.Result
            };
            Performancelog.Debug($"End,GivexClientManager,DeactivateGiveX,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

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
        public bool GiveX_Balance(string strCardNum, int saleNum, ref decimal balance, ref string cardStatus, ref string expDate,
            string userCode, out ErrorMessage errorMessage, out GiveXReceiptType givexReceipt)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,GivexClientManager,GiveX_Balance,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            givexReceipt = new GiveXReceiptType();
            var returnValue = false;
            errorMessage = new ErrorMessage();

            var response = new Variables.GiveXResponseType();

            var capValue = new object[3];

            balance = 0;
            cardStatus = "";
            expDate = "";
            if (strCardNum.Trim() == "")
            {
                return false;
            }

            modStringPad.InitGiveXRequest();
            Variables.GX_Request.ServiceType = "BalanceInquiry";

            Variables.GX_Request.UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]);

            Variables.GX_Request.UserPassword = Convert.ToString(_givexPolicies["GIVEX_PASS"]);

            Variables.GX_Request.OperatorID = Convert.ToString(userCode); // "ALL"
            Variables.GX_Request.GivexNumber = strCardNum.Trim();
            Variables.GX_Request.Language = "0";
            Variables.GX_Request.TransactionCode = (saleNum).ToString();
            Variables.GX_Request.TransmissionDate = DateAndTime.Today.ToString("MM/dd/yyyy");
            Variables.GX_Request.TransmissionTime = DateAndTime.TimeOfDay.ToString("hh:mm:ss");
            Variables.GX_Request.TerminalID = "ALL";
            var strSend = modStringPad.GetGiveXRequest();

            if (SendGiveXRequest(strSend, "BalanceInquiry," + (saleNum), "BalanceInquiry", ref response, out errorMessage))
            {
                if (response.Result != "Approved")
                {
                    if (response.CertificateBalance.Trim() == "")
                    {
                        capValue[1] = "";
                    }
                    else
                    {
                        capValue[1] = "(" + response.CertificateBalance.Trim() + ") ";
                    }

                    capValue[2] = strCardNum.Trim();

                    cardStatus = response.CertificateBalance.Trim();
                    //Call DisplayMsgForm("Invalid response {}from GiveX TPS, CANNOT check the balance for GiveX card {}.", 99)                    
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3268, 99, capValue, CriticalOkMessageType);
                    return false;
                }
                if (response.GivexNumber != strCardNum.Trim())
                {

                    capValue[1] = "";

                    capValue[2] = strCardNum.Trim();
                    //Call DisplayMsgForm("Invalid response {}from GiveX TPS, CANNOT check the balance for GiveX card {}.", 99)
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3268, 99, null, CriticalOkMessageType);
                    return false;
                }
                balance = decimal.Parse(response.CertificateBalance);
                expDate = string.Format(response.ExpiryDate, "MM/dd/yyyy");
                returnValue = true;


                modStringPad.InitGiveXReceipt();
                Variables.GX_Receipt.Date = DateAndTime.Today.ToString("MM/dd/yyyy");
                Variables.GX_Receipt.Time = DateAndTime.TimeOfDay.ToString("hh:mm:ss");

                Variables.GX_Receipt.UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]);
                Variables.GX_Receipt.TranType = 4; //Balance Inquiry
                Variables.GX_Receipt.SeqNum = (saleNum).ToString();
                Variables.GX_Receipt.CardNum = response.GivexNumber;
                Variables.GX_Receipt.ExpDate = string.Format(response.ExpiryDate, "yyMM");
                Variables.GX_Receipt.Balance = (float)(Conversion.Val(response.CertificateBalance));
                Variables.GX_Receipt.PointBalance = (float)(Conversion.Val(response.PointsBalance));
                Variables.GX_Receipt.ResponseCode = response.Result;
                givexReceipt = new GiveXReceiptType
                {
                    Date = DateAndTime.Today.ToString("MM/dd/yyyy"),
                    Time = DateAndTime.TimeOfDay.ToString("hh:mm:ss"),

                    UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]),
                    TranType = 4,//Balance Inquiry
                    SeqNum = (saleNum).ToString(),
                    CardNum = response.GivexNumber,
                    ExpDate = string.Format(response.ExpiryDate, "yyMM"),
                    Balance = (float)(Conversion.Val(response.CertificateBalance)),
                    PointBalance = (float)(Conversion.Val(response.PointsBalance)),
                    ResponseCode = response.Result
                };
            }
            Performancelog.Debug($"End,GivexClientManager,GiveX_Balance,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

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
        public bool IncreaseGiveX(string strCardNum, float amount, int saleNum, ref decimal newBalance, ref string refNum,
            string userCode, out ErrorMessage errorMessage, out GiveXReceiptType givexReceipt)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,GivexClientManager,IncreaseGiveX,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            givexReceipt = new GiveXReceiptType();
            var returnValue = false;
            errorMessage = new ErrorMessage();
            var response = new Variables.GiveXResponseType();

            var capValue = new object[3];

            newBalance = 0;
            refNum = "";
            if (strCardNum.Trim() == "")
            {
                return false;
            }

            modStringPad.InitGiveXRequest();
            Variables.GX_Request.ServiceType = "Increment";

            Variables.GX_Request.UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]);

            Variables.GX_Request.UserPassword = Convert.ToString(_givexPolicies["GIVEX_PASS"]);

            Variables.GX_Request.OperatorID = Convert.ToString(userCode); // "ALL"
            Variables.GX_Request.GivexNumber = strCardNum.Trim();
            Variables.GX_Request.Language = "0";
            Variables.GX_Request.Amount = (amount).ToString(CultureInfo.InvariantCulture);
            Variables.GX_Request.TransactionCode = (saleNum).ToString();
            Variables.GX_Request.TransmissionDate = DateAndTime.Today.ToString("MM/dd/yyyy");
            Variables.GX_Request.TransmissionTime = DateAndTime.TimeOfDay.ToString("hh:mm:ss");
            Variables.GX_Request.TerminalID = "ALL";
            var strSend = modStringPad.GetGiveXRequest();

            if (SendGiveXRequest(strSend, "Increment," + (saleNum), "Increment", ref response, out errorMessage))
            {
                if (response.Result != "Approved")
                {
                    if (!string.IsNullOrEmpty(response.TransactionReference))
                    {
                        if (response.TransactionReference.Trim() == "")
                        {

                            capValue[1] = "";
                        }
                        else
                        {

                            capValue[1] = "(" + response.TransactionReference.Trim() + ") ";
                        }
                    }
                    else
                    {
                        capValue[1] = "";
                    }

                    capValue[2] = strCardNum.Trim();

                    // Call DisplayMsgForm("Invalid response {}from GiveX TPS, CANNOT increase the amount for GiveX card {}.", 99)

                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3270, 99, capValue, CriticalOkMessageType);
                    return false;
                }
                if (response.GivexNumber != strCardNum.Trim())
                {

                    capValue[1] = "";

                    capValue[2] = strCardNum.Trim();
                    //Call DisplayMsgForm("Invalid response {}from GiveX TPS, CANNOT increase the amount for GiveX card {}.", 99)                    
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3270, 99, null, CriticalOkMessageType);
                    return false;
                }
                newBalance = decimal.Parse(response.CertificateBalance);
                refNum = response.TransactionReference;
                returnValue = true;

                modStringPad.InitGiveXReceipt();
                Variables.GX_Receipt.Date = DateAndTime.Today.ToString("MM/dd/yyyy");
                Variables.GX_Receipt.Time = DateAndTime.TimeOfDay.ToString("hh:mm:ss");

                Variables.GX_Receipt.UserID = Convert.ToString(_givexPolicies["GIVEX_PASS"]);
                Variables.GX_Receipt.TranType = 5; //Increment
                Variables.GX_Receipt.SeqNum = (saleNum).ToString();
                Variables.GX_Receipt.CardNum = response.GivexNumber;
                Variables.GX_Receipt.ExpDate = string.Format(response.ExpiryDate, "yyMM");
                Variables.GX_Receipt.Balance = (float)(Conversion.Val(response.CertificateBalance));
                Variables.GX_Receipt.SaleAmount = amount;
                Variables.GX_Receipt.PointBalance = (float)(Conversion.Val(response.PointsBalance));
                Variables.GX_Receipt.ResponseCode = response.Result;

                givexReceipt = new GiveXReceiptType
                {
                    Date = DateAndTime.Today.ToString("MM/dd/yyyy"),
                    Time = DateAndTime.TimeOfDay.ToString("hh:mm:ss"),
                    UserID = Convert.ToString(_givexPolicies["GIVEX_PASS"]),
                    TranType = 5,//Increment
                    SeqNum = (saleNum).ToString(),
                    CardNum = response.GivexNumber,
                    ExpDate = string.Format(response.ExpiryDate, "yyMM"),
                    Balance = (float)(Conversion.Val(response.CertificateBalance)),
                    SaleAmount = amount,
                    PointBalance = (float)(Conversion.Val(response.PointsBalance)),
                    ResponseCode = response.Result

                };
            }
            Performancelog.Debug($"End,GivexClientManager,IncreaseGiveX,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// GiveX batch Close
        /// </summary>
        /// <param name="saleNum">Sale number</param>
        /// <param name="cashoutId">Cash out id</param>
        /// <param name="report">Report</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>True or false</returns>
        public bool GiveX_Close(int saleNum, ref string cashoutId, ref string report,
            string userCode, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,GivexClientManager,GiveX_Close,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var returnValue = false;
            var response = new Variables.GiveXResponseType();
            var strReport = "";

            cashoutId = "";
            report = "";

            modStringPad.InitGiveXRequest();
            Variables.GX_Request.ServiceType = "CashOut";

            Variables.GX_Request.UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]);

            Variables.GX_Request.UserPassword = Convert.ToString(_givexPolicies["GIVEX_PASS"]);
            Variables.GX_Request.OperatorID = "ALL";
            Variables.GX_Request.Language = "0";
            Variables.GX_Request.TransactionCode = (saleNum).ToString();
            Variables.GX_Request.TransmissionDate = DateAndTime.Today.ToString("MM/dd/yyyy");
            Variables.GX_Request.TransmissionTime = DateAndTime.TimeOfDay.ToString("hh:mm:ss");
            Variables.GX_Request.TerminalID = "ALL";
            var strSend = modStringPad.GetGiveXRequest();

            if (SendGiveXRequest(strSend, "CashOut," + (saleNum), "CashOut", ref response, out errorMessage))
            {
                if (response.Result != "Approved")
                {
                    //Call DisplayMsgForm("CashOut is declined from GiveX TPS, CANNOT close batch for GiveX.", 99)                                       
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3273, 99, null, CriticalOkMessageType);
                    errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                    return false;

                }
                cashoutId = response.CashoutId.Trim();
                if (cashoutId == "0")
                {

                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3274, 99, null, CriticalOkMessageType);
                    errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                    return false;
                }




                returnValue = true;

                if (GetGiveXReport(saleNum, cashoutId, ref strReport, userCode, out errorMessage))
                {
                    report = strReport;
                }
            }
            Performancelog.Debug($"End,GivexClientManager,GiveX_Close,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        // Feb 25, 2009: Nicolette moved here this function from modGlobalFunctions module
        // It is called only in this form, so there is no reason to have it public
        // Also, in order to fix the GiveX issue (click or scroll when GiveX is processed
        // messes up other Tenders_Renamed) this function has to access boolCardProcessing variable
        // that is a private variable to this form, so I would have had to make boolCardProcessing
        // public if the function has not been moved
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
        public bool RedeemGiveX(string userCode, string strCardNum, float amount, string securityCode,
            int saleNum, ref decimal newBalance, ref string refNum, ref string expDate,
            ref string result, out ErrorMessage errorMessage, out GiveXReceiptType givexReceipt)
        {
            errorMessage = new ErrorMessage();
            givexReceipt = new GiveXReceiptType();
            string strSend = "";
            var response = new Variables.GiveXResponseType();
            var capValue = new object[3];

            var returnValue = false;
            newBalance = 0;
            refNum = "";
            if (string.IsNullOrEmpty(strCardNum))
            {
                return false;
            }

            modStringPad.InitGiveXRequest();
            Variables.GX_Request.ServiceType = "Redemption";
            Variables.GX_Request.UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]);
            Variables.GX_Request.UserPassword = Convert.ToString(_givexPolicies["GIVEX_PASS"]);
            Variables.GX_Request.OperatorID = Convert.ToString(userCode); // "ALL"
            Variables.GX_Request.GivexNumber = strCardNum.Trim();
            Variables.GX_Request.Language = "0";
            Variables.GX_Request.Amount = (amount).ToString();
            Variables.GX_Request.TransactionCode = (saleNum).ToString();
            Variables.GX_Request.SecurityCode = securityCode.Trim();
            Variables.GX_Request.TransmissionDate = DateAndTime.Today.ToString("MM/dd/yyyy");
            Variables.GX_Request.TransmissionTime = DateAndTime.TimeOfDay.ToString("hh:mm:ss");
            Variables.GX_Request.TerminalID = "ALL";
            strSend = modStringPad.GetGiveXRequest();

            if (SendGiveXRequest(strSend, "Redemption," + saleNum, "Redemption", ref response,
                out errorMessage))
            {
                if (response.Result != "Approved")
                {
                    if (response.TransactionReference.Trim() == "")
                    {
                        capValue[1] = "";
                    }
                    else
                    {
                        capValue[1] = "(" + response.TransactionReference.Trim() + ") ";
                    }
                    capValue[2] = strCardNum.Trim();

                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3271, 99, capValue, 0);
                    return false;
                }
                if (response.GivexNumber != strCardNum.Trim())
                {
                    capValue[1] = "";
                    capValue[2] = strCardNum.Trim();

                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3271, 99, capValue, 0);
                }
                else
                {
                    newBalance = (decimal)(Conversion.Val(response.CertificateBalance));
                    refNum = response.TransactionReference;
                    expDate = response.ExpiryDate != "****" ? string.Format("yyMM", response.ExpiryDate) : response.ExpiryDate;
                    result = response.Result;
                    returnValue = true;

                    modStringPad.InitGiveXReceipt();
                    Variables.GX_Receipt.Date = DateAndTime.Today.ToString("MM/dd/yyyy");
                    Variables.GX_Receipt.Time = DateAndTime.TimeOfDay.ToString("hh:mm:ss");
                    Variables.GX_Receipt.UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]);
                    Variables.GX_Receipt.TranType = 6; //Redemption
                    Variables.GX_Receipt.SeqNum = (saleNum).ToString();
                    Variables.GX_Receipt.CardNum = response.GivexNumber;
                    Variables.GX_Receipt.ExpDate = response.ExpiryDate != "****" ? string.Format("yyMM", response.ExpiryDate) : response.ExpiryDate;
                    Variables.GX_Receipt.Balance = (float)(Conversion.Val(response.CertificateBalance));
                    Variables.GX_Receipt.SaleAmount = amount;
                    Variables.GX_Receipt.PointBalance = (float)(Conversion.Val(response.PointsBalance));
                    Variables.GX_Receipt.ResponseCode = response.Result;
                }
            }
            givexReceipt = new GiveXReceiptType
            {
                Date = DateAndTime.Today.ToString("MM/dd/yyyy"),
                Time = DateAndTime.TimeOfDay.ToString("hh:mm:ss"),
                UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]),
                TranType = 6, //Redemption
                SeqNum = (saleNum).ToString(),
                CardNum = response.GivexNumber,
                ExpDate = response.ExpiryDate != "****" ? string.Format("yyMM", response.ExpiryDate) : response.ExpiryDate,
                Balance = (float)(Conversion.Val(response.CertificateBalance)),
                SaleAmount = amount,
                PointBalance = (float)(Conversion.Val(response.PointsBalance)),
                ResponseCode = response.Result
            };
            return returnValue;
        }

        #region Private methods

        /// <summary>
        /// Get GiveX Report
        /// </summary>
        /// <param name="saleNum">Sale number</param>
        /// <param name="cashoutId">Cash out id</param>
        /// <param name="report">Report</param>
        /// <param name="userCode">User code</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>True or false</returns>
        private bool GetGiveXReport(int saleNum, string cashoutId, ref string report,
           string userCode, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,GivexClientManager,GetGiveXReport,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var returnValue = false;

            var response = new Variables.GiveXResponseType();
            report = "";

            modStringPad.InitGiveXRequest();
            Variables.GX_Request.ServiceType = "Report";

            Variables.GX_Request.UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]);

            Variables.GX_Request.UserPassword = Convert.ToString(_givexPolicies["GIVEX_PASS"]);
            Variables.GX_Request.OperatorID = "ALL";
            Variables.GX_Request.CashoutId = cashoutId;
            Variables.GX_Request.ReportType = "1";
            Variables.GX_Request.Language = "0";
            Variables.GX_Request.TransactionCode = (saleNum).ToString();
            Variables.GX_Request.TransmissionDate = DateAndTime.Today.ToString("MM/dd/yyyy");
            Variables.GX_Request.TransmissionTime = DateAndTime.TimeOfDay.ToString("hh:mm:ss");
            Variables.GX_Request.TerminalID = "ALL";
            var strSend = modStringPad.GetGiveXRequest();
            if (SendGiveXRequest(strSend, "Report," + (saleNum), "Report", ref response, out errorMessage))
            {
                if (response.Result != "Approved")
                {
                    // Call DisplayMsgForm("Report is declined from GiveX TPS, CANNOT close batch for GiveX.", 99)                    
                    var messageType = (int)MessageType.OkOnly + MessageType.Critical;
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3275, 99, null, messageType);
                    errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                    return false;
                }
                var strReport = response.ReportLines;
                if (response.Continuation == "1")
                {
                    modStringPad.InitGiveXRequest();
                    Variables.GX_Request.ServiceType = "ReportContinuation";

                    Variables.GX_Request.UserID = Convert.ToString(_givexPolicies["GIVEX_USER"]);

                    Variables.GX_Request.UserPassword = Convert.ToString(_givexPolicies["GIVEX_PASS"]);

                    Variables.GX_Request.OperatorID = Convert.ToString(userCode); // "ALL"
                    Variables.GX_Request.Language = "0";
                    Variables.GX_Request.TransactionCode = (saleNum).ToString();
                    Variables.GX_Request.TransmissionDate = DateAndTime.Today.ToString("MM/dd/yyyy");
                    Variables.GX_Request.TransmissionTime = DateAndTime.TimeOfDay.ToString("hh:mm:ss");
                    Variables.GX_Request.TerminalID = "ALL";
                    strSend = modStringPad.GetGiveXRequest();
                    if (SendGiveXRequest(strSend, "ReportContinuation," + (saleNum), "ReportContinuation",
                        ref response, out errorMessage))
                    {
                        if (response.Result == "Approved")
                        {
                            strReport = strReport + response.ReportLines;
                        }
                    }
                }
                report = strReport;
                returnValue = true;
            }
            else
            {
                errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3276, 99, null, CriticalOkMessageType);
                errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
            }
            Performancelog.Debug($"End,GivexClientManager,GetGiveXReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Send GiveX Request
        /// </summary>
        /// <param name="strSend">Send string</param>
        /// <param name="strToCheck">Check</param>
        /// <param name="strDisplay">Display</param>
        /// <param name="gxResponse">Response</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>True or false</returns>
        private bool SendGiveXRequest(string strSend, string strToCheck, string strDisplay,
            ref Variables.GiveXResponseType gxResponse, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,GivexClientManager,SendGiveXRequest,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var strRemain = "";
            var gotResponse = false;
            errorMessage = new ErrorMessage();

            Variables.GiveX_TCP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var ipAddress = IPAddress.Parse(_givexPolicies["GIVEX_IP"].ToString());

            var remoteEndPoint = new IPEndPoint(ipAddress, Convert.ToInt16(_givexPolicies["GIVEX_PORT"]));
            try
            {
                Variables.GiveX_TCP.Connect(remoteEndPoint);

                var timeIn = (float)DateAndTime.Timer;

                while (!(Variables.GiveX_TCP.Connected || DateAndTime.Timer - timeIn > Convert.ToInt16(_givexPolicies["GIVETIMEOUT"])))
                {
                    if (DateAndTime.Timer < timeIn)
                    {
                        timeIn = (float)DateAndTime.Timer;
                    }
                }

                if (!Variables.GiveX_TCP.Connected)
                {
                    //Call DisplayMsgForm("Cannot communicate to GiveX TPS, {} Not Successful!", vbCritical + vbOKOnly, , , True)                
                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 8368, 99, strDisplay, CriticalOkMessageType);
                    errorMessage.StatusCode = HttpStatusCode.NotAcceptable;

                    Variables.GiveX_TCP = null;
                    return false;
                }

                // Encode the data string into a byte array.
                var msg = Encoding.ASCII.GetBytes(strSend);
                Variables.GiveX_TCP.Send(msg);


                // Data buffer for incoming data.
                var bytes = new byte[1024];

                while (!(DateAndTime.Timer - timeIn > Convert.ToInt16(_givexPolicies["GIVETIMEOUT"])))
                {
                    var bytesRec = Variables.GiveX_TCP.Receive(bytes);
                    //int bytesRec = 0;
                    var strBuffer = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    WriteToLogFile("GiveX_TCP.PortReading: " + strBuffer + " from waiting " + strToCheck);
                    if (!string.IsNullOrEmpty(strBuffer))
                    {
                        if (modStringPad.SplitGiveXResponse(strBuffer, strToCheck, ref gxResponse, ref strRemain))
                        {
                            //Variables.GiveX_TCP.PortReading = strRemain;
                            WriteToLogFile("modify GiveX_TCP.PortReading to: " + strRemain);
                            gotResponse = true;
                            break;
                        }
                    }
                    // Variables.Sleep(100);               
                    if (DateAndTime.Timer < timeIn)
                    {
                        timeIn = (float)DateAndTime.Timer;
                    }
                }

                if (!gotResponse)
                {

                    errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 3272, 99, strDisplay, CriticalOkMessageType);
                    errorMessage.StatusCode = HttpStatusCode.NotAcceptable;

                    WriteToLogFile("TimeOut for waiting " + strToCheck);
                }

                Variables.GiveX_TCP.Shutdown(SocketShutdown.Both);
                Variables.GiveX_TCP.Close();

                Variables.GiveX_TCP = null;

                var returnValue = gotResponse;
                Performancelog.Debug($"End,GivexClientManager,SendGiveXRequest,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return returnValue;
            }
            catch (Exception)
            {
                //Call DisplayMsgForm("Cannot communicate to GiveX TPS, {} Not Successful!", vbCritical + vbOKOnly, , , True)                
                errorMessage.MessageStyle = _resourceManager.DisplayMsgForm(_offSet, 8368, 99, strDisplay, CriticalOkMessageType);
                errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                return false;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class GivexManager : ManagerBase, IGivexManager
    {
        private readonly IGivexService _givexService;
        private readonly IApiResourceManager _resourceManager;
        private readonly ISaleManager _saleManager;
        private readonly ISaleLineManager _saleLineManager;
        private readonly IGivexClientManager _givexClientManager;
        private readonly IReceiptManager _receiptManager;
        private readonly IPolicyManager _policyManager;

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="givexService"></param>
        /// <param name="givexClientManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="saleManager"></param>
        /// <param name="saleLineManager"></param>
        /// <param name="receiptManager"></param>
        /// <param name="policyManager"></param>
        public GivexManager(IGivexService givexService,
            IApiResourceManager resourceManager,
            ISaleManager saleManager,
            ISaleLineManager saleLineManager,
            IGivexClientManager givexClientManager,
            IReceiptManager receiptManager,
            IPolicyManager policyManager
           )
        {
            _givexService = givexService;
            _resourceManager = resourceManager;
            _saleManager = saleManager;
            _saleLineManager = saleLineManager;
            _givexClientManager = givexClientManager;
            _receiptManager = receiptManager;
            _policyManager = policyManager;
        }

        #endregion

        /// <summary>
        /// Get a Valid GiveX Stockcode
        /// </summary>
        /// <param name="errorMessage">errorMessage</param>        
        /// <returns>stock code</returns>
        public string GetValidGiveXStock(out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,GivexManager,GetValidGiveXStock,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            errorMessage = new ErrorMessage();
            var stockCode = _givexService.GetValidGiveXStock();
            if (string.IsNullOrEmpty(stockCode))
            {
                var store = CacheManager.GetStoreInfo();
                short offSet = store?.OffSet ?? (short)0;
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 12, 64, null, CriticalOkMessageType);
                errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                return null;
            }
            Performancelog.Debug($"End,GivexManager,GetValidGiveXStock,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return stockCode;
        }


        /// <summary>
        /// Adjust Givex Card with the new amount
        /// </summary>
        /// <param name="givexCardNumber">givexCardNumber</param>
        /// <param name="amount">amount</param>
        /// <param name="userCode">userCode</param>
        /// <param name="tillNumber">tillNumber</param>
        /// <param name="saleNumber">saleNumber</param>
        /// <param name="stockCode">stockCode</param>
        /// <param name="givexReceipt"></param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>Sale</returns>
        public Sale AdjustGivexCard(string givexCardNumber, decimal amount, string userCode,
                                        int tillNumber, int saleNumber, string stockCode, out Report givexReceipt,
                                        out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,GivexManager,AdjustGivexCard,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            decimal newBalance = 0;
            var refNum = "";
            var expDate = "";
            string result = "";
            givexReceipt = null;
            var newLine = _saleLineManager.CreateNewSaleLine();
            var sale = new Sale();
            GiveXReceiptType givexReceiptType;

            if (!IsValidPrice(2, amount, out errorMessage))
            {
                return null;
            }
            if (_givexClientManager.AdjustGiveX(givexCardNumber, amount, saleNumber, ref newBalance, ref refNum,
                ref expDate, ref result, userCode, out errorMessage, out givexReceiptType))
            {


                givexReceipt = _receiptManager.Print_GiveX_Receipt(givexReceiptType);

                sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
                if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                {
                    _saleLineManager.SetPluCode(ref sale, ref newLine, stockCode, out errorMessage);
                    if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                    {
                        newLine.Line_Num = (short)(sale.Sale_Lines.Count + 1);
                        newLine.Regular_Price = Conversion.Val(amount);

                        _saleLineManager.SetPrice(ref newLine, Conversion.Val(amount));

                        newLine.Gift_Num = givexCardNumber.Trim();
                        _saleManager.Add_a_Line(ref sale, newLine, userCode, tillNumber, out errorMessage, true);

                        //Update Sale object in Cache
                        CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);
                    }
                }
            }
            Performancelog.Debug($"End,GivexManager,AdjustGivexCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return sale;
        }

        /// <summary>
        /// Activate Givex Card
        /// </summary>
        /// <param name="givexCardNumber">givexCardNumber</param>
        /// <param name="givexPrice">givexPrice</param>
        /// <param name="userCode">userCode</param>
        /// <param name="tillNumber">tillNumber</param>
        /// <param name="saleNumber">saleNumber</param>
        /// <param name="stockCode">stockCode</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>Sale</returns>
        public Sale ActivateGivexCard(string givexCardNumber, decimal givexPrice, string userCode,
            int tillNumber, int saleNumber, string stockCode, out Report givexReceipt, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,GivexManager,ActivateGivexCard,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            givexReceipt = null;
            var refNum = "";
            var newLine = _saleLineManager.CreateNewSaleLine();
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle?.Message))
            {
                return null;
            }
            if (!IsValidPrice(2, givexPrice, out errorMessage))
            {
                return null;
            }
            GiveXReceiptType givex;
            if (_givexClientManager.ActivateGiveX(givexCardNumber.Trim(), (float)Conversion.Val(givexPrice),
                saleNumber, ref refNum, userCode, out errorMessage, out givex))
            {

                givexReceipt = _receiptManager.Print_GiveX_Receipt(givex);

                if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                {
                    _saleLineManager.SetPluCode(ref sale, ref newLine, stockCode, out errorMessage);

                    if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                    {
                        newLine.Line_Num = (short)(sale.Sale_Lines.Count + 1);
                        newLine.Regular_Price = Conversion.Val(givexPrice);

                        _saleLineManager.SetPrice(ref newLine, Conversion.Val(givexPrice));

                        newLine.Gift_Num = givexCardNumber.Trim();
                        _saleManager.Add_a_Line(ref sale, newLine, userCode, tillNumber, out errorMessage, true);

                        //Update Sale object in Cache
                        CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);
                    }
                }
            }
            Performancelog.Debug($"End,GivexManager,ActivateGivexCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return sale;
        }

        /// <summary>
        /// Deactivate Givex Card
        /// </summary>
        /// <param name="givexCardNumber">givexCardNumber</param>
        /// <param name="givexPrice">givexPrice</param>
        /// <param name="userCode">userCode</param>
        /// <param name="tillNumber">tillNumber</param>
        /// <param name="saleNumber">saleNumber</param>
        /// <param name="stockCode">stockCode</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>Sale</returns>
        public Sale DeactivateGivexCard(string givexCardNumber, decimal givexPrice, string userCode,
            int tillNumber, int saleNumber, string stockCode, out Report givexReceipt, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,GivexManager,DeactivateGivexCard,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            givexReceipt = null;
            var sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                return null;
            }
            var firstOrDefault = sale.Sale_Lines.FirstOrDefault(x => x.Gift_Num == givexCardNumber);
            if (firstOrDefault == null)
            {

                var store = CacheManager.GetStoreInfo();
                short offSet = store?.OffSet ?? (short)0;
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 32, 93, null, CriticalOkMessageType);
                errorMessage.StatusCode = HttpStatusCode.NotAcceptable;
                return null;
            }

            short lineNumber = firstOrDefault.Line_Num;// _saleService.GetSaleLineFromDbTemp(givexCardNumber);

            var saleLine = sale.Sale_Lines[lineNumber];

            var newLine = _saleLineManager.CreateNewSaleLine();


            var amount = saleLine.Amount;
            var refNum = saleLine.Serial_No;
            if (!IsValidPrice(2, givexPrice, out errorMessage))
            {
                return null;
            }
            GiveXReceiptType givex;
            if (_givexClientManager.DeactivateGiveX(givexCardNumber.Trim(), amount, saleNumber, refNum, userCode, out errorMessage, out givex))
            {

                givexReceipt = _receiptManager.Print_GiveX_Receipt(givex);

                sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
                if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                {
                    _saleLineManager.SetPluCode(ref sale, ref newLine, stockCode, out errorMessage);
                    if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                    {
                        newLine.Line_Num = (short)(sale.Sale_Lines.Count + 1);
                        newLine.Regular_Price = Conversion.Val(0 - amount);

                        _saleLineManager.SetPrice(ref newLine, Conversion.Val(0 - amount));

                        newLine.Gift_Num = givexCardNumber.Trim();
                        _saleManager.Add_a_Line(ref sale, newLine, userCode, tillNumber, out errorMessage, true);

                        //Update Sale object in Cache
                        CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);
                    }
                }
            }


            Performancelog.Debug($"End,GivexManager,DeactivateGivexCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return sale;
        }


        /// <summary>
        /// Get Givex Card Balance
        /// </summary>
        /// <param name="givexCardNumber">givexCardNumber</param>
        /// <param name="saleNumber">saleNumber</param>
        /// <param name="tillNumber"></param>
        /// <param name="userCode">userCode</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>balance</returns>
        public decimal GetCardBalance(string givexCardNumber, int saleNumber, int
            tillNumber, string userCode, out Report givexReceipt, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,GivexManager,GetCardBalance,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            decimal amount = 0;
            var cardStatus = "";
            var expDate = "";
            givexReceipt = null;
            _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
            if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                GiveXReceiptType givex;
                if (_givexClientManager.GiveX_Balance(givexCardNumber.Trim(), saleNumber, ref amount, ref cardStatus,
                    ref expDate, userCode, out errorMessage, out givex))
                {
                    givexReceipt = _receiptManager.Print_GiveX_Receipt(givex);
                }
            }
            Performancelog.Debug($"End,GivexManager,GetCardBalance,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return amount;
        }

        /// <summary>
        /// Increase Givex Card value
        /// </summary>
        /// <param name="givexCardNumber">givexCardNumber</param>
        /// <param name="givexPrice">givexPrice</param>
        /// <param name="userCode">userCode</param>
        /// <param name="tillNumber">tillNumber</param>
        /// <param name="saleNumber">saleNumber</param>
        /// <param name="stockCode">stockCode</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>Sale</returns>
        public Sale IncreaseGivexCard(string givexCardNumber, decimal givexPrice, string userCode,
           int tillNumber, int saleNumber, string stockCode, out Report givexReceipt, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,GivexManager,IncreaseGivexCard,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            decimal newBalance = 0;
            var refNum = "";
            givexReceipt = null;
            var newLine = _saleLineManager.CreateNewSaleLine();
            var sale = new Sale();
            GiveXReceiptType givex;
            if (!IsValidPrice(2, givexPrice, out errorMessage))
            {
                return null;
            }
            if (_givexClientManager.IncreaseGiveX(givexCardNumber.Trim(), (float)Conversion.Val(givexPrice),
                saleNumber, ref newBalance, ref refNum, userCode, out errorMessage, out givex))
            {


                givexReceipt = _receiptManager.Print_GiveX_Receipt(givex);


                sale = _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);
                if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                {
                    _saleLineManager.SetPluCode(ref sale, ref newLine, stockCode, out errorMessage);
                    if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                    {
                        newLine.Line_Num = (short)(sale.Sale_Lines.Count + 1);
                        newLine.Regular_Price = Conversion.Val(givexPrice);

                        _saleLineManager.SetPrice(ref newLine, Conversion.Val(givexPrice));

                        newLine.Gift_Num = givexCardNumber.Trim();
                        _saleManager.Add_a_Line(ref sale, newLine, userCode, tillNumber, out errorMessage, true);

                        //Update Sale object in Cache
                        CacheManager.AddCurrentSaleForTill(tillNumber, saleNumber, sale);
                    }
                }
            }

            Performancelog.Debug($"End,GivexManager,IncreaseGivexCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return sale;
        }

        /// <summary>
        /// Close Batch
        /// </summary>
        /// <param name="saleNumber">saleNumber</param>
        /// <param name="tillNumber"></param>
        /// <param name="userCode">userCode</param>
        /// <param name="givexReport">Givex receipt</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>true/false</returns>
        public bool CloseBatch(int saleNumber, int tillNumber, string userCode, out Report givexReport, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,GivexManager,CloseBatch,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var returnVal = false;
            var cashoutId = "";
            var reports = "";
            givexReport = null;
            //check sale exist for the particular sale number or not
            _saleManager.GetCurrentSale(saleNumber, tillNumber, 0, userCode, out errorMessage);

            if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                if (_givexClientManager.GiveX_Close(saleNumber, ref cashoutId, ref reports, userCode, out errorMessage))
                {
                    if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                    {
                        _givexService.SaveCloseBatch(cashoutId, reports);
                        if (!string.IsNullOrEmpty(cashoutId) && !string.IsNullOrEmpty(reports))
                        {

                            var tempReprint = false;
                            givexReport = _receiptManager.PrintGiveXClose(cashoutId, true, ref tempReprint);
                        }
                        returnVal = true;
                    }
                }
            }
            Performancelog.Debug($"End,GivexManager,CloseBatch,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return returnVal;
        }


        /// <summary>
        /// Gets the Givex Report
        /// </summary>
        /// <param name="reportDate"></param>
        /// <returns></returns>
        public GivexReport GetGivexReport(DateTime reportDate)
        {
            var result = new GivexReport
            {
                ReportDetails = new List<GivexDetails>()
            };

            string timeFormat;
            if (_policyManager.TIMEFORMAT == "24 HOURS")
            {
                timeFormat = "hh:mm:ss";
            }
            else
            {
                timeFormat = "hh:mm:ss tt";
            }
            result.ReportDetails = _givexService.GetGivexReportDetails(reportDate, timeFormat);



            if (result.ReportDetails.Count > 0)
            {
                bool refPrint = true;
                result.CloseBatchReport = _receiptManager.PrintGiveXClose(result.ReportDetails[0].CashOut, true, ref refPrint);
                //rtbReport.LoadFile((new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\" + "GiveXClose.txt");
            }

            return result;
        }



        #region Private methods
        /// <summary>
        /// Method to check a valid price
        /// </summary>
        /// <param name="pRiceDec"></param>
        /// <param name="price"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private bool IsValidPrice(short pRiceDec, decimal price, out ErrorMessage error)
        {
            //validate price
            error = new ErrorMessage();
            const double maxprice = 9999; // 
            const double minprice = -9999;

            var pd = pRiceDec; // Price Decimals

            var fs = Convert.ToString(pd == 0 ? " " : "." + new string('9', pd));
            if (Conversion.Val(price) <= Conversion.Val(minprice + fs))
            {
                MessageType temp_VbStyle9 = (int)MessageType.Exclamation + MessageType.OkOnly;
                error.MessageStyle = new MessageStyle
                {
                    Message = "Minimum price is 9999~Price error",
                    MessageType = temp_VbStyle9
                };
                error.StatusCode = HttpStatusCode.NotAcceptable;
                return false;
            }
            if (Conversion.Val(price) > Conversion.Val(maxprice + fs))
            {
                var store = CacheManager.GetStoreInfo();
                short offSet = store?.OffSet ?? (short)0;
                MessageType temp_VbStyle9 = (int)MessageType.Exclamation + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 76, maxprice + fs, temp_VbStyle9);
                error.StatusCode = HttpStatusCode.NotAcceptable;
                return false;
            }
            return true;
        }

        #endregion

    }//end class
}//end namespace

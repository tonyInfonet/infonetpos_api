using System;
using System.IO;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.Entities;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using log4net;
using Infonet.CStoreCommander.Logging;
using System.Net;
using System.Web.Http;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
  public  class CashBonusManager:ICashBonusManager
    {
        

        private readonly ICashBonusService _cashBonusService;
        private readonly IReceiptManager _receiptManager;
        private readonly ITillService _tillService;
        private readonly IReasonService _reasonService;
        private readonly IPolicyManager _policyManager;
        private readonly ILoginManager _loginManager;
        private readonly IApiResourceManager _resourceManager;
        //private readonly ITenderManager _tenderManager;
        //private readonly ISaleManager _saleManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cashService"></param>
        /// <param name="receiptManager"></param>
        /// <param name="tillService"></param>
        /// <param name="reasonService"></param>
        /// <param name="policyManager"></param>
        /// <param name="loginManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="tenderManager"></param>
        /// <param name="saleManager"></param>
        public CashBonusManager(ICashBonusService cashBonusService, IPolicyManager policyManager, ITillService tillService, ILoginManager loginManager, IApiResourceManager resourceManager, IReceiptManager receiptManager, IReasonService reasonService /*ITillService tillService,
           IReasonService reasonService
          , ITenderManager tenderManager,
           ISaleManager saleManager*/)
        {
            _cashBonusService = cashBonusService;
            _receiptManager = receiptManager;
            _reasonService = reasonService;
            _policyManager = policyManager;
            _loginManager = loginManager;
            _resourceManager = resourceManager;
            _tillService = tillService;
            //_tenderManager = tenderManager;
            //_saleManager = saleManager;
        }




        /// <summary>
        /// Method to get cash draw buttons
        /// </summary>
        /// <returns>Cash draw buttons</returns>
        public CashBonusDrawButton GetCashBonusDrawButtons(string userCode, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,GetCashBonusDrawButtons,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            error = new ErrorMessage();
            var user = _loginManager.GetExistingUser(userCode);
            if (!Convert.ToBoolean(_policyManager.GetPol("U_TILLDRAW", user)))
            {
                MessageType temp_VbStyle3 = (int)MessageType.Exclamation + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 38, 56, null, temp_VbStyle3);
                error.StatusCode = System.Net.HttpStatusCode.Forbidden;
                _performancelog.Debug($"End,CashManager,GetCashBonusDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return null;
            }

            var cashBonusDrawButton = new CashBonusDrawButton
            {
                Coins = _cashBonusService.GetCashBonusCoins()

            };
            _performancelog.Debug($"End,CashManager,GetCashBonusDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return cashBonusDrawButton;
        }

        /// <summary>
        /// Method to calculate cash bonus
        /// </summary>
        /// <returns>Calculate cash bonus</returns>
        public double CalculateCashBonus(string GroupID, float SaleLitre ,out ErrorMessage error)
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            error = new ErrorMessage();
            double _cashbonus = _cashBonusService.CalculateCashBonus(GroupID,SaleLitre);
            return _cashbonus;
        }


        /// <summary>
        /// Method to complete the cash bonus draw
        /// </summary>
        /// <param name="cashDraw"></param>
        /// <param name="userCode"></param>
        /// <param name="copies"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public FileStream CompleteCashBonusDraw(CashBonusDrawButton cashBonusDraw, string userCode, out int copies,
            out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,GetCashDrawButtons,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            copies = _policyManager.CashDrawReceiptCopies;
            errorMessage = new ErrorMessage();
            var message = string.Empty;
            var till = _tillService.GetTill(cashBonusDraw.TillNumber);
            if (till == null)
            {
                message = "Till does not exists";
            }

            if (cashBonusDraw.Amount <= 0)
            {
                message = "Invalid Cash Bonus Draw Amount";
            }

            if (!string.IsNullOrEmpty(message))
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = message
                };
                errorMessage.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return null;
            }
            var isInvalidCash = false;
            var isInvalidQuantity = false;

            var cashBonusDrawButtons = GetCashBonusDrawButtons(userCode, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return null;
            }

            ValidateCoins(cashBonusDraw, ref isInvalidCash, ref isInvalidQuantity, cashBonusDrawButtons);
            //ValidateBills(cashDraw, ref isInvalidCash, ref isInvalidQuantity, cashDrawButtons);

            if (isInvalidCash)
            {
                message = "Invalid CashBonus";
            }

            if (isInvalidQuantity)
            {
                message = "Invalid CashBonus Quantity";
            }

            if (!string.IsNullOrEmpty(message))
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = message
                };
                errorMessage.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return null;
            }

            //print receipt
            var reasonType = ReasonType.CashDraw;
            var reason = _reasonService.GetReturnReason(cashBonusDraw.DrawReason, (char)reasonType);
            if (_policyManager.DRAW_REASON && reason == null)
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    Message = "Invalid Reason"
                };
                errorMessage.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return null;
            }

            var draw = new CashBonusDraw
            {
                TillNumber = cashBonusDraw.TillNumber,
                DrawDate = DateTime.Now,
                //TotalValue = (float)cashDraw.Amount,
                User = userCode.ToUpper(),
                Reason = cashBonusDraw.DrawReason,
                CashBonus = cashBonusDraw.Amount
            };

            // add values to cash bonus draw
            _cashBonusService.AddCashBonusDraw(draw);

            //update till
            if (till != null)
            {
                 
                 till.CashBonus = till.CashBonus + cashBonusDraw.Amount;
                _tillService.UpdateTill(till);
                _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
               // return _receiptManager.Print_Draw(till, cashBonusDraw.RegisterNumber, userCode, cashBonusDraw.Coins, reason, cashBonusDraw.Amount);
            }
            return null;
        }


        /// <summary>
        /// method to validate coins
        /// </summary>
        /// <param name="cashDraw"></param>
        /// <param name="isInvalidCash"></param>
        /// <param name="isInvalidQuantity"></param>
        /// <param name="cashDrawButtons"></param>
        private void ValidateCoins(CashBonusDrawButton cashDraw, ref bool isInvalidCash, ref bool
                  isInvalidQuantity, CashBonusDrawButton cashDrawButtons)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CashManager,GetCashDrawButtons,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            foreach (var coin in cashDraw.Coins)
            {
                if (!cashDrawButtons.Coins.Any(c => c.CurrencyName == coin.CurrencyName
                && c.Value == coin.Value))
                {
                    isInvalidCash = true;
                    break;
                }
                if (coin.Quantity < 1 && coin.Quantity > 99)
                {
                    isInvalidQuantity = true;
                    break;
                }
            }
            _performancelog.Debug($"End,CashManager,GetCashDrawButtons,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }
    }
}

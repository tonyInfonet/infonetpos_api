using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using System;
using System.Collections.Generic;
using System.Net;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class BottleManager : ManagerBase, IBottleManager
    {
        private readonly IBottleReturnService _bottleReturnService;
        private readonly IPolicyManager _policyManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ISaleService _saleService;
        private readonly ISaleManager _saleManager;
        private readonly ILoginManager _loginManager;
        private readonly ITenderManager _tenderManager;
        private readonly IReceiptManager _receiptManager;
        private readonly ITillService _tillService;


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bottleReturnService"></param>
        /// <param name="policyManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="saleService"></param>
        /// <param name="saleManager"></param>
        /// <param name="loginManager"></param>
        /// <param name="tenderManager"></param>
        /// <param name="receiptManager"></param>
        /// <param name="tillService"></param>
        public BottleManager(IBottleReturnService bottleReturnService,
            IPolicyManager policyManager,
            IApiResourceManager resourceManager,
            ISaleService saleService,
            ISaleManager saleManager,
            ILoginManager loginManager,
            ITenderManager tenderManager,
            IReceiptManager receiptManager,
            ITillService tillService)
        {
            _bottleReturnService = bottleReturnService;
            _policyManager = policyManager;
            _resourceManager = resourceManager;
            _saleService = saleService;
            _saleManager = saleManager;
            _loginManager = loginManager;
            _tenderManager = tenderManager;
            _receiptManager = receiptManager;
            _tillService = tillService;
        }

        /// <summary>
        /// Get Bottles
        /// </summary>
        /// <param name="pageId">Page ID</param>
        /// <returns>List of bottles</returns>
        public List<BottleReturn> GetBottles(int pageId = 1)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,BottleManager,GetBottles,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            const int startRangeforBottle = 1000;
            var startIndex = (pageId - 1) * 36 + startRangeforBottle + 1;
            var endIndex = pageId * 36 + startRangeforBottle;
            var bottles = _bottleReturnService.GetBottlesFromDbMaster(startIndex, endIndex);

            Performancelog.Debug($"End,BottleManager,GetBottles,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return bottles;
        }


        /// <summary>
        /// Save Bottle return
        /// </summary>
        /// <param name="brPayment">Br payment</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <param name="bottleReport">Report</param>
        /// <param name="openDrawer">Open drawer or not</param>
        public Sale SaveBottleReturn(BR_Payment brPayment,
            out ErrorMessage error, out Report bottleReport, out bool openDrawer)
        {
            bottleReport = null;
            openDrawer = false;
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,BottleManager,SaveBottleReturn,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            error = new ErrorMessage();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var userCode = UserCode;
            var user = _loginManager.GetExistingUser(userCode);
            var existingSale = _saleService.GetSaleByTillNumber(brPayment.TillNumber);
            if (existingSale != null && existingSale.Sale_Lines.Count > 0)
            {

                error.MessageStyle = new MessageStyle
                {
                    Message = "Please finish current sale before bottle return.~Bottle Return",
                    MessageType = ExclamationOkMessageType
                };
                error.StatusCode = HttpStatusCode.Conflict;
                return null;
            }
            if (!_policyManager.GetPol("U_BOTTLERTN", user))
            {

                error.MessageStyle = _resourceManager.CreateMessage(offSet, 38, 57, null, ExclamationOkMessageType);
                error.StatusCode = HttpStatusCode.Forbidden;
                return null;
            }

            if (brPayment.Amount != 0)
            {
                if (string.IsNullOrEmpty(Convert.ToString(_policyManager.GetPol("BASECURR", null))))
                {
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 18, 61, null, CriticalOkMessageType);
                    error.StatusCode = HttpStatusCode.NotFound;
                    return null;
                }

                if (brPayment.Amount > (decimal)Math.Abs(_policyManager.GetPol("U_BR_LIMIT", user)))
                {
                    // Exceed the bottle return limit, Please get an authorized user!
                    error.MessageStyle = _resourceManager.CreateMessage(offSet, 18, 62, null, CriticalOkMessageType);
                    error.StatusCode = HttpStatusCode.Forbidden;
                    return null;
                }

                if (_policyManager.OPEN_DRAWER == "Every Sale")
                {
                    openDrawer = true;
                }
                var sale = new Sale
                {
                    Sale_Totals = { Net = Convert.ToDecimal(brPayment.Amount) }
                };
                var saleTotals = sale.Sale_Totals;
                _saleManager.SetGross(ref saleTotals, sale.Sale_Totals.Net);
                sale.Sale_Totals.Gross = saleTotals.Gross;
                sale.Sale_Totals.TotalLabel = saleTotals.TotalLabel;
                sale.Sale_Totals.SummaryLabel = saleTotals.SummaryLabel;
                sale.Register = brPayment.RegisterNumber;
                sale.Sale_Change = 0;

                sale.TillNumber = Convert.ToByte(brPayment.TillNumber);
                sale.Sale_Date = DateTime.Now;
                sale.Sale_Tender = 0;
                sale.Sale_Change = 0;
                sale.Sale_Amount = 0;
                if (_policyManager.GetPol("Penny_Adj", null) && brPayment.Amount != 0)
                {

                    sale.Sale_Totals.Penny_Adj = Helper.Calculate_Penny_Adj(Convert.ToDecimal(brPayment.Amount));
                }
                else
                {
                    sale.Sale_Totals.Penny_Adj = 0;
                }

                brPayment.Penny_Adj = sale.Sale_Totals.Penny_Adj;
                if (existingSale == null)
                {
                    brPayment.Sale_Num = _saleManager.GetCurrentSaleNo(brPayment.TillNumber, userCode, out error);
                }
                var tendBr = new Tenders();
                var tender = tendBr.Add(Convert.ToString(_policyManager.GetPol("BASECURR", null)),
                     "Cash", 1, true, true, false, (short)1,
                     Convert.ToString(_policyManager.GetPol("BASECURR", null)),
                     false, 0, 0, 0.01, true, Convert.ToDouble(-brPayment.Amount),
                     (short)1, true, false, "", "");
                _tenderManager.Set_Amount_Entered(ref tendBr, ref sale, tender, -brPayment.Amount);

                sale.Sale_Num = Convert.ToInt32(brPayment.Sale_Num);
                sale.Sale_Type = "BTL RTN";
                var shiftNumber = _tillService.GetTill(brPayment.TillNumber).Shift;
                bottleReport = _receiptManager.Print_BottleReturn(brPayment, user.Name, DateTime.Today,
                    DateTime.Now, sale.Register, (short)brPayment.TillNumber, shiftNumber);
                bottleReport.Copies = _policyManager.BottleReturnReceiptCopies;
                _bottleReturnService.SaveBottleReturnsToDbTrans(brPayment);
                _saleManager.SaveSale(sale, userCode, ref tendBr, null);
                CacheManager.DeleteCurrentSaleForTill(sale.TillNumber, sale.Sale_Num);
                //_saleManager.Clear_Sale(sale.Sale_Num,sale.TillNumber, userCode, "", null, true, false, false, out msg);
                sale = _saleManager.InitializeSale(brPayment.TillNumber, brPayment.RegisterNumber, userCode, out error);
                //Update Sale object in Cache
                CacheManager.AddCurrentSaleForTill(sale.TillNumber, sale.Sale_Num, sale);
                Performancelog.Debug($"End,BottleManager,SaveBottleReturn,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return sale;
            }
            return null;
        }

    }
}

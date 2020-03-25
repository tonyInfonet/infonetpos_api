using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infonet.CStoreCommander.BusinessLayer.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class PaymentSourceManager : ManagerBase, IPaymentSourceManager
    {
        private readonly IPaymentSourceService _PaymentSourceService;
        public PaymentSourceManager(IPaymentSourceService PaymentSourceService)
        {
            _PaymentSourceService = PaymentSourceService;
        }

        public bool DownloadFile()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,PaymentSourceManager,DownloadFile,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            bool bDownload = _PaymentSourceService.DownloadFile();
            Performancelog.Debug($"End,PaymentSourceManager,DownloadFile,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return bDownload;
        }

        public List<PSLogo> GetPSLogos()
        {
            return _PaymentSourceService.GetPSLogos();
        }

        public List<PSProduct> GetPSProducts()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,PaymentSourceManager,GetPSProducts,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            List<PSProduct> olist = _PaymentSourceService.GetPSProducts();
            Performancelog.Debug($"End,PaymentSourceManager,GetPSProducts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return olist;
        }

        public PSProfile GetPSProfile()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,PaymentSourceManager,GetPSProfile,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            PSProfile pspf = _PaymentSourceService.GetPSProfile();
            Performancelog.Debug($"End,PaymentSourceManager,GetPSProfile,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return pspf;
        }

        public PSRefund GetPSRefund(int TILL_NUM, int SALE_NO, string TransactionID)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,PaymentSourceManager,GetPSRefund,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            PSRefund psr;
            Sale sale = CacheManager.GetCurrentSaleForTill(TILL_NUM, SALE_NO);
            if(sale!=null)
            foreach(Sale_Line sl in sale.Sale_Lines)
            {
                if(sl.Serial_No== TransactionID)
                {
                    psr = new PSRefund();
                    psr.UpcNumber = sl.Stock_Code;
                    psr.Amount = string.Format("{0:0.00}", sl.Amount);
                    psr.Name = sl.Description;
                    return psr;
                }
            }
            psr = _PaymentSourceService.GetPSRefund(TransactionID);
            Performancelog.Debug($"End,PaymentSourceManager,GetPSRefund,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return psr;
        }

        public string GetPSTransactionID()
        {
            
            return _PaymentSourceService.GetPSTransactionID();
        }

        public PSVoucherInfo GetPSVoucherInfo(string ProdName)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,PaymentSourceManager,GetPSVoucherInfo,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            PSVoucher psvc = _PaymentSourceService.GetPSVoucherInfo(ProdName);
            if (psvc == null)
                return null;
            PSVoucherInfo psvcinfo = new PSVoucherInfo();
            psvcinfo.Voucher = psvc;
            psvcinfo.Logos = _PaymentSourceService.GetPSLogos(ProdName);
            Performancelog.Debug($"End,PaymentSourceManager,GetPSVoucherInfo,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return psvcinfo;
        }

        public bool SavePSTransactionID(int TILL_NUM, int SALE_NO, int LINE_NUM, string TransID)
        {
            bool bSaved = false;
            try
            {
                Sale sale = CacheManager.GetCurrentSaleForTill(TILL_NUM, SALE_NO);
                sale.Sale_Lines[LINE_NUM].Serial_No = TransID;
                //Update current sale
                CacheManager.AddCurrentSaleForTill(TILL_NUM, SALE_NO, sale);
                //Get crrent sale after updating
                //sale = CacheManager.GetCurrentSaleForTill(TILL_NUM, SALE_NO);
                bSaved = true;
            }
            catch (Exception ex)
            {
                Performancelog.Error("PaymentSourceManager.SavePSTransactionID(): " + ex.Message);
            }
            
            return bSaved;
        }
        public List<PSTransaction> GetPSTransactions(int TILL_NUM, int SALE_NO, int PastDays)
        {
            List<PSTransaction> olist = new List<PSTransaction>();
            try
            {
                var sale = CacheManager.GetCurrentSaleForTill(TILL_NUM, SALE_NO);
                if (sale != null)
                    olist = (from c in sale.Sale_Lines
                             where !string.IsNullOrEmpty(c.Serial_No)
                             select new PSTransaction()
                             {
                                 TransactionID = c.Serial_No,
                                 SALE_DATE = string.Format("{0:MM-dd-yyyy}", DateTime.Now),
                                 STOCK_CODE = c.Stock_Code,
                                 DESCRIPT = c.Description,
                                 Amount = string.Format("{0:0.00}", c.Amount)
                             }).ToList();
                List<PSTransaction> olist1 = _PaymentSourceService.GetPSTransactions(PastDays);
                olist = olist.Union(olist1).ToList();
            }
            catch (Exception ex)
            {
                Performancelog.Error("PaymentSourceManager.GetCurTransactions(): " + ex.Message);
            }
            return olist;
        }
    }
}

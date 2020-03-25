using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IPaymentSourceManager
    {
        string GetPSTransactionID();
        bool DownloadFile();
        List<PSProduct> GetPSProducts();
        PSProfile GetPSProfile();
        PSVoucherInfo GetPSVoucherInfo(string ProdName);
        bool SavePSTransactionID(int TILL_NUM, int SALE_NO, int LINE_NUM, string TransID);
        List<PSLogo> GetPSLogos();
        PSRefund GetPSRefund(int TILL_NUM, int SALE_NO,string TransactionID);
        List<PSTransaction> GetPSTransactions(int TILL_NUM, int SALE_NO, int PastDays);
    }
}

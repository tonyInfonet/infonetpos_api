using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IPaymentSourceService
    {
        bool DownloadFile();
        List<PSProduct> GetPSProducts();
        PSProfile GetPSProfile();
        PSVoucher GetPSVoucherInfo(string ProdName);
        List<PSLogo> GetPSLogos(string ProdName);
        string GetPSTransactionID();
        List<PSLogo> GetPSLogos();
        PSRefund GetPSRefund(string TransactionID);
        List<PSTransaction> GetPSTransactions(int PastDays);
    }
}

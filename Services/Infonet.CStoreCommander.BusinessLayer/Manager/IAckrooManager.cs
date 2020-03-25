using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IAckrooManager
    {
        string GetValidAckrooStock();
        List<Carwash> GetCarwashCategories();
        string GetAckrooCarwashStockCode(string sDesc);
        string GetLoyaltyNo(int Sale_No);

    }
}

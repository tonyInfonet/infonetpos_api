using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class AckrooManager : ManagerBase, IAckrooManager
    {
        private readonly IAckrooService _AckrooService;
        public AckrooManager(IAckrooService AckrooService)
        {
            _AckrooService = AckrooService;
        }
        public string GetLoyaltyNo(int Sale_No)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,AckrooManager,GetLoyaltyNo,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string sVal = _AckrooService.GetLoyaltyNo(Sale_No);
            Performancelog.Debug($"End,AckrooManager,GetLoyaltyNo,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return sVal;
        }
        public string GetValidAckrooStock()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,AckrooManager,GetValidAckrooStock,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string sVal = _AckrooService.GetValidAckrooStock();

            Performancelog.Debug($"End,AckrooManager,GetValidAckrooStock,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return sVal;
        }
       public List<Carwash> GetCarwashCategories()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,AckrooManager,GetCarwashCategories,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var olist= _AckrooService.GetCarwashCategories();
            Performancelog.Debug($"End,AckrooManager,GetCarwashCategories,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return olist;
        }
        public string GetAckrooCarwashStockCode(string sDesc)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,AckrooManager,GetAckrooCarwashStockCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string sVal = _AckrooService.GetAckrooCarwashStockCode(sDesc);

            Performancelog.Debug($"End,AckrooManager,GetAckrooCarwashStockCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return sVal;
        }
    }
}

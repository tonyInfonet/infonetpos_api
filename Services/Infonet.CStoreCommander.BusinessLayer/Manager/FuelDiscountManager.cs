using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.ADOData;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class FuelDiscountManager : ManagerBase, IFuelDiscountManager
    {
        private readonly IFuelDiscountService _fuelDiscountService;
        public FuelDiscountManager(IFuelDiscountService fuelDiscountService)
        {
            _fuelDiscountService = fuelDiscountService;
        }
        public List<ClientGroup> GetClientGroups()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,FuelDiscountManager,GetClientGroups,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            List<ClientGroup> olist = _fuelDiscountService.GetClientGroups();
            Performancelog.Debug($"End,FuelDiscountManager,GetClientGroups,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return olist;
        }

        public string GetFuelCodes()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,FuelDiscountManager,GetFuelCodes,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string sCodes = _fuelDiscountService.GetFuelCodes();
            Performancelog.Debug($"End,FuelDiscountManager,GetFuelCodes,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return sCodes;

        }
    }
}

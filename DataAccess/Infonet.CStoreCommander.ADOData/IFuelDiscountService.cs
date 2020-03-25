using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IFuelDiscountService
    {
        List<ClientGroup> GetClientGroups();
        string GetFuelCodes();
    }
}

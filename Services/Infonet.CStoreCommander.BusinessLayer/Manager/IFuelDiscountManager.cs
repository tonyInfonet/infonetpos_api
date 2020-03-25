using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IFuelDiscountManager
    {
        List<ClientGroup> GetClientGroups();
        string GetFuelCodes();
    }
}

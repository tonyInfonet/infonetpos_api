using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.ADOData
{
    public interface ICashBonusService
    {
        List<CashBonus> GetCashBonusCoins();

        double CalculateCashBonus(string GroupID, float SaleLitre);

        void AddCashBonusDraw(CashBonusDraw cashBonusDraw);

    }
}

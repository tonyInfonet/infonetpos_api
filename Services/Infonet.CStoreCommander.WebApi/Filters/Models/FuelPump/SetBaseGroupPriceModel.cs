using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    public class SetBaseGroupPriceModel
    {
        public List<GroupFuelPriceModel> Prices { get; set; }
        public int Row { get; set; }
    }
}
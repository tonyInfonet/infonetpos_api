using Infonet.CStoreCommander.BusinessLayer.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    public class PriceIncrementDecrementPayloadModel
    {
        public List<PriceIncrement> PriceIncrements { get; set; } = new List<PriceIncrement>();
        public List<PriceDecrement> PriceDecrements { get; set; } = new List<PriceDecrement>();
        public bool TaxExempt { get; set; }
    }
}
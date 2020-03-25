using Infonet.CStoreCommander.BusinessLayer.Entities;

namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    public class PriceDecrementPayloadModel
    {
        public PriceDecrement Price { get; set; }
        public bool TaxExempt { get; set; }
    }
}
﻿using Infonet.CStoreCommander.BusinessLayer.Entities;

namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    public class PriceIncrementPayloadModel
    {
        public PriceIncrement Price { get; set; }
        public bool TaxExempt { get; set; }
    }
}
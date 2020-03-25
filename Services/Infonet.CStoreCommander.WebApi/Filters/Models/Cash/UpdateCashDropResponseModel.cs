using System;
using System.Collections.Generic;
using Infonet.CStoreCommander.WebApi.Models.Tender;

namespace Infonet.CStoreCommander.WebApi.Models.Cash
{
    /// <summary>
    /// Updated tenders in caash drop 
    /// </summary>
    public class UpdateCashDropResponseModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public UpdateCashDropResponseModel()
        {
                Tenders = new List<TenderResponseModel>();
        }

        /// <summary>
        /// Tenders
        /// </summary>
        public  List<TenderResponseModel> Tenders { get; set; }

        /// <summary>
        /// Tendered amount
        /// </summary>
        public string TenderedAmount { get; set; }
    }
}
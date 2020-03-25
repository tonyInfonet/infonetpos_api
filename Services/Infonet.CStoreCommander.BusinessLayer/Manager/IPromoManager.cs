using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public  interface IPromoManager
    {
        /// <summary>
        /// Method to clear all promos
        /// </summary>
        /// <returns></returns>
        bool Clear_AllPromos(ref Promos promo);

        /// <summary>
        /// Method to load all promos
        /// </summary>
        /// <param name="optPromoId">PromoID</param>
        /// <returns>Promos</returns>
        Promos Load_Promos(string optPromoId);

        /// <summary>
        /// Method to get all promos for today
        /// </summary>
        /// <returns>List of promos</returns>
        List<Promo> GetPromosForToday();
    }
}

using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IPromoService
    {
        /// <summary>
        /// Method to get all promos for today
        /// </summary>
        /// <param name="optPromoId">Promo id</param>
        /// <returns>Promo headers</returns>
        List<Promo> GetPromoHeadersForToday(string optPromoId);
               

        /// <summary>
        /// Method to get promo lines
        /// </summary>
        /// <param name="promoId">Promo id</param>
        /// <param name="none"></param>
        /// <returns>List of promo lines</returns>
        List<Promo_Line> GetPromoLines(string promoId ,string none);

        /// <summary>
        /// Method to get distinct promoId for today
        /// </summary>
        /// <returns>Promo IDs</returns>
        List<string> GetDistinctPromoIdsForToday();

        /// <summary>
        /// Method to get promos for today
        /// </summary>
        /// <returns>List of promos</returns>
        List<Promo> GetPromosForToday();
    }
}

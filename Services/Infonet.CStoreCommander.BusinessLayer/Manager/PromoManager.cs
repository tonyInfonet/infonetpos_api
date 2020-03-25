using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class PromoManager : ManagerBase, IPromoManager
    {
        private readonly IApiResourceManager _resourceManager;
        private readonly IPromoService _promoService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="promoService"></param>
        public PromoManager(IApiResourceManager resourceManager, IPromoService promoService)
        {
            _resourceManager = resourceManager;
            _promoService = promoService;
        }

        // Load all the promotions in the Promos collection. Can load a specified promo or all of them
        /// <summary>
        /// Method to load all promos
        /// </summary>
        /// <param name="optPromoId">PromoID</param>
        /// <returns>Promos</returns>
        public Promos Load_Promos(string optPromoId)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,PromoManager,Load_Promos,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var promos = CacheManager.GetPromosForPromoId(optPromoId);
            if (promos != null && promos.Count != 0)
            {
                return promos;
            }
            promos = new Promos();
            var store = CacheManager.GetStoreInfo();
            var offSet = store?.OffSet ?? 0;
            var none = _resourceManager.GetResString(offSet, 347);
            var promoHeaders = _promoService.GetPromoHeadersForToday(optPromoId);
            foreach (var promoHeader in promoHeaders)
            {
                var promo = promoHeader;
                var promoLines = new Promo_Lines();

                //promo.MaxLink = _promoService.GetMaxLink(promo.PromoID);
                //var noOfLinks = _promoService.GetNumberOfLinks(promo.PromoID);
                //foreach (var noOfLink in noOfLinks)
                //{
                //    if (noOfLink > 1)
                //    {
                //        promo.MultiLink = true;
                //        break;
                //    }
                //}

                var promoDetails = _promoService.GetPromoLines(promo.PromoID, none);
                foreach (var promoDetail in promoDetails)
                {
                    promoLines.AddLine(promoDetail, "");
                }
                promo.Promo_Lines = promoLines;
                promos.Add(promo, promo.PromoID);
            }
            Performancelog.Debug($"End,PromoManager,Load_Promos,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            CacheManager.AddPromos(optPromoId, promos);
            return promos;
        }

        //   to clear all promotions before reloading them
        /// <summary>
        /// Method to clear all promos
        /// </summary>
        /// <returns></returns>
        public bool Clear_AllPromos(ref Promos promos)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,PromoManager,Clear_AllPromos,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var promoIDs = GetPromosForToday().Select(p => p.PromoID).ToList();
            if (promoIDs.Count != promos.Count)
            {
                Performancelog.Debug($"End,PromoManager,Clear_AllPromos,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return false;
            }

            foreach (var promoId in promoIDs)
            {
                var strKey = promoId;
                if (!string.IsNullOrEmpty(strKey))
                {
                    promos.Remove(strKey);
                }
            }
            Performancelog.Debug($"End,PromoManager,Clear_AllPromos,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return true;
        }

        /// <summary>
        /// Method to get all promos for today
        /// </summary>
        /// <returns>List of promos</returns>
        public List<Promo> GetPromosForToday()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,PromoManager,GetPromosForToday,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var promosForToday = CacheManager.GetPromosForToday();
            if (promosForToday != null && promosForToday.Count != 0)
            {
                return promosForToday;
            }
            promosForToday = _promoService.GetPromosForToday();
            Performancelog.Debug($"End,PromoManager,GetPromosForToday,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            CacheManager.AddPromosForToday(promosForToday);
            return promosForToday;
        }
    }
}

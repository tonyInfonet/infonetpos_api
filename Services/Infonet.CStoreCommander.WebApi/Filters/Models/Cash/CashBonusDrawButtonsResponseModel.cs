using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infonet.CStoreCommander.WebApi.Models.Cash
{
    public class CashBonusDrawButtonsResponseModel
    {
        public CashBonusDrawButtonsResponseModel()
        {
            Coins = new List<CashBonusModel>();

        }

        /// <summary>
        /// Coins
        /// </summary>
        public List<CashBonusModel> Coins { get; set; }


    }

        /// <summary>
        /// Cash model
        /// </summary>
        //public class CashModel
        //{
        //    /// <summary>
        //    /// Currency name
        //    /// </summary>
        //    public string CurrencyName { get; set; }

        //    /// <summary>
        //    /// Value
        //    /// </summary>
        //    public decimal Value { get; set; }

        //    /// <summary>
        //    /// Image
        //    /// </summary>
        //    public string Image { get; set; }

        //}
    }

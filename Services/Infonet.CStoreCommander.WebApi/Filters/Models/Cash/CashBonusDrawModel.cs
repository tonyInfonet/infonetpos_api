using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infonet.CStoreCommander.WebApi.Models.Cash
{
    public class CashBonusDrawModel
    {
        
            public CashBonusDrawModel()
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
        public class CashBonusModel
        {
            /// <summary>
            /// Currency name
            /// </summary>
            public string CurrencyName { get; set; }

            /// <summary>
            /// Value
            /// </summary>
            public decimal Value { get; set; }

            /// <summary>
            /// Image
            /// </summary>
            public int ButtonNumber { get; set; }
            
        }
    }


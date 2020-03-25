using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.Cash
{
    /// <summary>
    /// Cash draw button model
    /// </summary>
    public class CashDrawButtonsResponseModel
    {
        public CashDrawButtonsResponseModel()
        {
            Coins = new List<CashModel>();
            Bills = new List<CashModel>();
        }

        /// <summary>
        /// Coins
        /// </summary>
        public List<CashModel> Coins { get; set; }

        /// <summary>
        /// Bills
        /// </summary>
        public List<CashModel> Bills { get; set; }
    }

    /// <summary>
    /// Cash model
    /// </summary>
    public class CashModel
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
        public string Image { get; set; }

    }
}
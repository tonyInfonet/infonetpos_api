namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    /// <summary>
    /// Pump Model
    /// </summary>
    public class PumpModel
    {
        /// <summary>
        /// Pump Id
        /// </summary>
        public int PumpId { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Pump Button caption
        /// </summary>
        public string PumpButtonCaption { get; set; }


        /// <summary>
        /// Basket button caption
        /// </summary>
        public string BasketButtonCaption { get; set; }

        /// <summary>
        /// Basket button visible
        /// </summary>
        public int BasketButtonVisible { get; set; }

        /// <summary>
        /// Basket label caption
        /// </summary>
        public string BasketLabelCaption { get; set; }

        /// <summary>
        /// Pay pump or prepay
        /// </summary>
        public dynamic PayPumporPrepay { get; set; }

        /// <summary>
        /// Prepay text
        /// </summary>
        public string PrepayText { get; set; }

        /// <summary>
        /// Enable basket button
        /// </summary>
        public dynamic EnableBasketButton { get; set; }

        /// <summary>
        /// Enable stack basket button
        /// </summary>
        public dynamic EnableStackBasketButton { get; set; }
    }
}
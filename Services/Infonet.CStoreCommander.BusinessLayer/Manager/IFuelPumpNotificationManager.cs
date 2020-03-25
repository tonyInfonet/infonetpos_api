using Infonet.CStoreCommander.Resources;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IFuelPumpNotificationManager
    {
        /// <summary>
        /// Method to read price change notification
        /// </summary>
        /// <returns>Message</returns>
        MessageStyle ReadPricheChangeNotificationHo();
    }
}
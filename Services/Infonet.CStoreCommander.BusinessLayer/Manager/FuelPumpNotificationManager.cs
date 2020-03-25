using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.Resources;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class FuelPumpNotificationManager : ManagerBase, IFuelPumpNotificationManager
    {
        private readonly IPolicyManager _policyManager;
        private readonly IFuelPumpService _fuelPumpService;
        private readonly IApiResourceManager _resourceManager;

        public FuelPumpNotificationManager(
            IPolicyManager policyManager,
            IFuelPumpService fuelPumpService,
            IApiResourceManager resourceManager)
        {
            _policyManager = policyManager;
            _fuelPumpService = fuelPumpService;
            _resourceManager = resourceManager;
        }

        /// <summary>
        /// Method to read price change notification
        /// </summary>
        /// <returns>Message</returns>
        public MessageStyle ReadPricheChangeNotificationHo()
        {

            MessageStyle msg = null;
            if (_policyManager.FUELPR_HO)
            {
                if (modGlobalFunctions.BoolFuelPriceApplied)
                {
                    WriteToLogFile("Fuel price change from HeadOffice: boolFuelPriceApplied is " + System.Convert.ToString(modGlobalFunctions.BoolFuelPriceApplied) + " counter was reset to 0.");
                }
                modGlobalFunctions.BoolFuelPriceApplied = false;
                if (_fuelPumpService.IsPriceChangeFromHo())
                {
                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    msg = _resourceManager.CreateMessage(offSet,38, 71, null, _policyManager.FPR_USER ? MessageType.YesNoCancel : MessageType.YesNo);
                }
            }
            return msg;
        }

    }
}

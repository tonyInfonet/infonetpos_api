using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class GetPropertyManager : ManagerBase, IGetPropertyManager
    {
        private readonly IFuelPumpService _fuelPumpService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="fuelPumpService"></param>
        public GetPropertyManager(IFuelPumpService fuelPumpService)
        {
            _fuelPumpService = fuelPumpService;
        }

        /// <summary>
        /// Method to get prices to display
        /// </summary>
        /// <param name="row">Row</param>
        /// <returns>Prices to display</returns>
        public CPricesToDisplay get_PricesToDisplay(byte row)
        {
            return _fuelPumpService.get_PricesToDisplay(row);
        }

        /// <summary>
        /// Method to set prices to display
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="value">Value</param>
        public void set_PricesToDisplay(byte row, CPricesToDisplay value)
        {
            _fuelPumpService.set_PricesToDisplay(row, value);
        }

        /// <summary>
        /// Method to get tier level price difference
        /// </summary>
        /// <param name="tier">Tier</param>
        /// <param name="level">Level</param>
        /// <returns>Tier level price difference</returns>
        public CTierLevelPriceDiff get_TierLevelPriceDiff(byte tier, byte level)
        {
            return _fuelPumpService.get_TierLevelPriceDiff(tier, level);
        }

        /// <summary>
        /// Method to set tier level price difference
        /// </summary>
        /// <param name="tier">Tier</param>
        /// <param name="level">Level</param>
        /// <param name="value">Tier level price difference</param>
        public void set_TierLevelPriceDiff(byte tier, byte level, CTierLevelPriceDiff
            value)
        {
            _fuelPumpService.set_TierLevelPriceDiff(tier, level, value);
        }

        /// <summary>
        /// Method to get grade price increment
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <returns>Grade price increment</returns>
        public CGradePriceIncrement get_GradePriceIncrement(byte gradeId)
        {
            return _fuelPumpService.get_GradePriceIncrement(gradeId);
        }

        /// <summary>
        /// Method to set grade price increment
        /// </summary>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="value">Grade price increment</param>
        public void set_GradePriceIncrement(byte gradeId, CGradePriceIncrement value)
        {
            _fuelPumpService.set_GradePriceIncrement(gradeId, value);
        }

        /// <summary>
        /// Method to set fuel price
        /// </summary>
        /// <param name="fuelProperty">Get property</param>
        /// <param name="gradeId">Grade id</param>
        /// <param name="tierId">Tier id</param>
        /// <param name="levelId">Level id</param>
        /// <param name="value">Fuel price</param>
        public void set_FuelPrice(ref GetProperty fuelProperty, byte gradeId, byte tierId,
            byte levelId, CFuelPrice value)
        {

            _fuelPumpService.set_FuelPrice(ref fuelProperty, gradeId, tierId, levelId, value);
        }

        /// <summary>
        /// Method to get tank info
        /// </summary>
        /// <param name="id">Grade id</param>
        /// <returns>Tank info</returns>
        public CTankInfo get_TankInfo(byte id)
        {
            return _fuelPumpService.get_TankInfo(id);
        }

        /// <summary>
        /// Method to get tank type
        /// </summary>
        /// <param name="tankCode">Tank code</param>
        /// <returns>Tank type</returns>
        public CTankType get_TankType(string tankCode)
        {
            return _fuelPumpService.get_TankType(tankCode);
        }

        /// <summary>
        /// Method to get tank chart
        /// </summary>
        /// <param name="dipChart">Dip chart</param>
        /// <param name="depth">Depth</param>
        /// <returns>Tank chart</returns>
        public dynamic get_TankChart(string dipChart, short depth)
        {
            return _fuelPumpService.get_TankChart(dipChart, depth);
        }

        /// <summary>
        /// Method to get sale option
        /// </summary>
        /// <param name="optionId">Option id</param>
        /// <returns>Sale option</returns>
        public dynamic get_SaleOption(byte optionId)
        {
            return _fuelPumpService.get_SaleOption(optionId);
        }

        /// <summary>
        /// Method to set fuel price in history
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <param name="tierId">Tier id</param>
        /// <param name="levelId">Level id</param>
        /// <param name="value">Fuel price</param>
        public void set_PutPriceinHist(byte gradeId, byte tierId, byte levelId,
            CFuelPrice value)
        {
            _fuelPumpService.set_PutPriceinHist(gradeId, tierId, levelId, value);
        }

        /// <summary>
        /// Method to write totalizer
        /// </summary>
        /// <param name="till">Till</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="posId">Pos Id</param>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="volume">Volume</param>
        /// <param name="amount">Amount</param>
        /// <param name="mg">Group number</param>
        /// <returns>True or false</returns>
        public bool WriteTotalizer(Till till, short pumpId, short posId, byte gradeId, 
            string volume, string amount, short mg)
        {
            return _fuelPumpService.WriteTotalizer(till, pumpId, posId, gradeId, volume, amount, mg);
        }

        /// <summary>
        /// Method to write total high low
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="highVolume">High volume</param>
        /// <param name="lowVolume">Low volume</param>
        /// <param name="mg">Group number</param>
        /// <returns>True or false</returns>
        public bool WriteTotalHighLow(short pumpId, string highVolume, string lowVolume,
            short mg)
        {
            return _fuelPumpService.WriteTotalHighLow(pumpId, highVolume, lowVolume, mg);
        }

        // Returns maximum of ReportID from FuelPrice and FuelPriceHist
        /// <summary>
        /// Method to get the maximum report id
        /// </summary>
        /// <returns>Report id</returns>
        public int Get_ReportID()
        {
            return _fuelPumpService.Get_ReportID();
        }

        /// <summary>
        /// Method to read fuel price
        /// </summary>
        /// <param name="fuelProperties"></param>
        /// <param name="isTaxExempt">Is tax exempt policy</param>
        /// <param name="isTeByRate">Is tax exempt by rate policy</param>
        /// <param name="teType">Tax exempt type</param>
        public void Read_FuelPrice(ref GetProperty fuelProperties, bool isTaxExempt, 
            bool isTeByRate, string teType)
        {
            _fuelPumpService.Read_FuelPrice(ref fuelProperties, isTaxExempt, isTeByRate, teType);
        }

        /// <summary>
        /// Method to read pump
        /// </summary>
        /// <param name="fuelProperties">Fuel properties</param>
        /// <param name="isAuthPumpPos">Is pump authenticated</param>
        public void Read_pump(ref GetProperty fuelProperties, bool isAuthPumpPos)
        {
            _fuelPumpService.Read_pump(ref fuelProperties, PosId, isAuthPumpPos);
        }
    }
}

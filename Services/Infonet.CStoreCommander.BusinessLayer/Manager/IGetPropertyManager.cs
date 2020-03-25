using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IGetPropertyManager
    {
        /// <summary>
        /// Method to get grade price increment
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <returns>Grade price increment</returns>
        CGradePriceIncrement get_GradePriceIncrement(byte gradeId);

        /// <summary>
        /// Method to get prices to display
        /// </summary>
        /// <param name="row">Row</param>
        /// <returns>Prices to display</returns>
        CPricesToDisplay get_PricesToDisplay(byte row);

        // Returns maximum of ReportID from FuelPrice and FuelPriceHist
        /// <summary>
        /// Method to get the maximum report id
        /// </summary>
        /// <returns>Report id</returns>
        int Get_ReportID();

        /// <summary>
        /// Method to get sale option
        /// </summary>
        /// <param name="optionId">Option id</param>
        /// <returns>Sale option</returns>
        dynamic get_SaleOption(byte optionId);


        /// <summary>
        /// Method to get tank chart
        /// </summary>
        /// <param name="dipChart">Dip chart</param>
        /// <param name="depth">Depth</param>
        /// <returns>Tank chart</returns>
        dynamic get_TankChart(string dipChart, short depth);

        /// <summary>
        /// Method to get tank info
        /// </summary>
        /// <param name="id">Grade id</param>
        /// <returns>Tank info</returns>
        CTankInfo get_TankInfo(byte id);

        /// <summary>
        /// Method to get tank type
        /// </summary>
        /// <param name="tankCode">Tank code</param>
        /// <returns>Tank type</returns>
        CTankType get_TankType(string tankCode);

        /// <summary>
        /// Method to get tier level price difference
        /// </summary>
        /// <param name="tier">Tier</param>
        /// <param name="level">Level</param>
        /// <returns>Tier level price difference</returns>
        CTierLevelPriceDiff get_TierLevelPriceDiff(byte tier, byte level);

        /// <summary>
        /// Method to set fuel price
        /// </summary>
        /// <param name="fuelProperty">Get property</param>
        /// <param name="gradeId">Grade id</param>
        /// <param name="tierId">Tier id</param>
        /// <param name="levelId">Level id</param>
        /// <param name="value">Fuel price</param>
        void set_FuelPrice(ref GetProperty fuelProperty, byte gradeId, byte tierId, byte levelId,
            CFuelPrice value);

        /// <summary>
        /// Method to set grade price increment
        /// </summary>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="value">Grade price increment</param>
        void set_GradePriceIncrement(byte gradeId, CGradePriceIncrement value);

        /// <summary>
        /// Method to set prices to display
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="value">Value</param>
        void set_PricesToDisplay(byte row, CPricesToDisplay value);


        /// <summary>
        /// Method to set fuel price in history
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <param name="tierId">Tier id</param>
        /// <param name="levelId">Level id</param>
        /// <param name="value">Fuel price</param>
        void set_PutPriceinHist(byte gradeId, byte tierId, byte levelId, CFuelPrice value);


        /// <summary>
        /// Method to set tier level price difference
        /// </summary>
        /// <param name="tier">Tier</param>
        /// <param name="level">Level</param>
        /// <param name="value">Tier level price difference</param>
        void set_TierLevelPriceDiff(byte tier, byte level, CTierLevelPriceDiff value);

        /// <summary>
        /// Method to write total high low
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="highVolume">High volume</param>
        /// <param name="lowVolume">Low volume</param>
        /// <param name="mg">Group number</param>
        /// <returns>True or false</returns>
        bool WriteTotalHighLow(short pumpId, string highVolume, string lowVolume, short mg);

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
        bool WriteTotalizer(Till till, short pumpId, short posId, byte gradeId, string volume,
            string amount, short mg);

        /// <summary>
        /// Method to read fuel price
        /// </summary>
        /// <param name="fuelProperties"></param>
        /// <param name="isTaxExempt">Is tax exempt policy</param>
        /// <param name="isTeByRate">Is tax exempt by rate policy</param>
        /// <param name="teType">Tax exempt type</param>
        void Read_FuelPrice(ref GetProperty fuelProperties, bool isTaxExempt, bool isTeByRate,
            string teType);

        /// <summary>
        /// Method to read pump
        /// </summary>
        /// <param name="fuelProperties">Fuel properties</param>
        /// <param name="isAuthPumpPos">Is pump authenticated</param>
        void Read_pump(ref GetProperty fuelProperties, bool isAuthPumpPos);
    }
}
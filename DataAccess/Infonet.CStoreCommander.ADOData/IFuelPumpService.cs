using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IFuelPumpService
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
        /// Method to initilaize get property
        /// </summary>
        /// <param name="posId">Pos Id</param>
        /// <param name="isTaxExempt">Is tax exempt policy</param>
        /// <param name="isTeByRate">Is tax exempt by rate policy</param>
        /// <param name="teType">Tax exempt type</param>
        /// <param name="isAuthPumpPos">Is authenticated POS</param>
        /// <returns>Property</returns>
        GetProperty InitializeGetProperty(int posId, bool isTaxExempt, bool isTeByRate, string teType, bool isAuthPumpPos);

        /// <summary>
        /// Method to read all data
        /// </summary>
        /// <param name="fuelProperties">Fuel properties</param>
        /// <param name="posId">POS id</param>
        /// <param name="isTaxExempt">Is tax exempt policy</param>
        /// <param name="isTeByRate">Is tax exempt by rate policy</param>
        /// <param name="teType">Tax exempt type</param>
        /// <param name="isAuthPumpPos">Is authenticated POS</param>
        void Read_AllData(ref GetProperty fuelProperties, int posId, bool isTaxExempt, bool isTeByRate, string teType, bool isAuthPumpPos);

        /// <summary>
        /// Method to read assignment
        /// </summary>
        /// <param name="fuelProperties">Fuel properties</param>
        void Read_Assignment(ref GetProperty fuelProperties);

        /// <summary>
        /// Method to read FCIP
        /// </summary>
        /// <param name="fuelProperties">Fuel properties</param>
        void Read_FCIP(ref GetProperty fuelProperties);

        /// <summary>
        /// Method to read fuel price
        /// </summary>
        /// <param name="fuelProperties">fuelProperties</param>
        /// <param name="isTaxExempt">Is tax exempt policy</param>
        /// <param name="isTeByRate">Is tax exempt by rate policy</param>
        /// <param name="teType">Tax exempt type</param>
        void Read_FuelPrice(ref GetProperty fuelProperties, bool isTaxExempt, bool isTeByRate, string teType);

        /// <summary>
        /// Method to read grade
        /// </summary>
        /// <param name="fuelProperties">Fuel properties</param>
        void Read_Grade(ref GetProperty fuelProperties);

        /// <summary>
        /// Method to read pump
        /// </summary>
        /// <param name="fuelProperties">Fuel properties</param>
        /// <param name="posId">POS id</param>
        /// <param name="isAuthPumpPos">Is pump authenticated</param>
        void Read_pump(ref GetProperty fuelProperties, int posId, bool isAuthPumpPos);

        /// <summary>
        /// Method to set fuel price
        /// </summary>
        /// <param name="fuelProperty">Get property</param>
        /// <param name="gradeId">Grade id</param>
        /// <param name="tierId">Tier id</param>
        /// <param name="levelId">Level id</param>
        /// <param name="value">Fuel price</param>
        void set_FuelPrice(ref GetProperty fuelProperty, byte gradeId, byte tierId, byte levelId, CFuelPrice value);

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
        /// <param name="tillRenamed">Till</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="posId">Pos Id</param>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="volume">Volume</param>
        /// <param name="amount">Amount</param>
        /// <param name="mg">Group number</param>
        /// <returns>True or false</returns>
        bool WriteTotalizer(Till tillRenamed, short pumpId, short posId, byte gradeId, string volume, string amount, short mg);


        /// <summary>
        /// Method to find if price is chaged from head office
        /// </summary>
        /// <returns>True or false</returns>
        bool IsPriceChangeFromHo();

        /// <summary>
        /// Method to get the maximum group number of totalizer history
        /// </summary>
        /// <returns>Group number</returns>
        short GetMaxGroupNumberofTotalizerHistory();

        /// <summary>
        /// Method to get maximum group number of total high low
        /// </summary>
        /// <returns>Group number</returns>
        short GetMaxGroupNumberofTotalHighLow();

        /// <summary>
        /// Method to delete fuel price
        /// </summary>
        void DeleteFuelPrice();

        /// <summary>
        /// Method to get list of head office fuel prices
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <param name="tierId">Tier id</param>
        /// <param name="levelId">Level id</param>
        /// <returns>List of fuel prices</returns>
        List<FuelPrice> GetHeadOfficeFuelPrices(short gradeId, short tierId, short levelId);

        /// <summary>
        /// Method to get fuel price by grade id, tier id and level id
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <param name="tierId">Tier Id</param>
        /// <param name="levelId">Level Id</param>
        /// <returns>Fuel price</returns>
        FuelPrice GetFuelPrice(short gradeId, short tierId, short levelId);


        /// <summary>
        /// Method to fuel price by rate
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <param name="gradeId">Grade id</param>
        /// <param name="tierId">Tier id</param>
        /// <param name="levelId">Level id</param>
        /// <param name="regularCashPrice">Regular cash price</param>
        /// <param name="regularCreditPrice">Regular credit price</param>
        /// <param name="taxExemptTaxCode">Tax exempt tax code</param>
        /// <returns>True or false</returns>
        bool GetFuelPriceByRate(string stockCode, int gradeId, int tierId, int levelId,
            ref float regularCashPrice, ref float regularCreditPrice, ref short taxExemptTaxCode);

        /// <summary>
        /// Method to find if prepay is set
        /// </summary>
        /// <param name="tillNumber">Till Number</param>
        /// <returns>True or false</returns>
        bool IsPrepaySet(int tillNumber);

        /// <summary>
        /// Method to get all totalizer history
        /// </summary>
        /// <param name="groupNumber">Group number</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="gradeId">Grade id</param>
        /// <returns>List of totaliser history</returns>
        List<TotalizerHist> GetTotalizerHist(int groupNumber, short pumpId, short gradeId);

        /// <summary>
        /// Method to get tax exempt by rate
        /// </summary>
        /// <param name="gradeId">Grade id</param>
        /// <param name="regPrice">Regular price</param>
        /// <param name="stockCode">Stock code</param>
        /// <returns>Tax exempt price</returns>
        string GetTePriceByRate(short gradeId, float regPrice, string stockCode);


        /// <summary>
        /// Get Propane Grades
        /// </summary>
        /// <returns></returns>
        List<PropaneGrade> GetPropaneGrades();


        /// <summary>
        /// Get Propane pumps by Propane Grade Id
        /// </summary>
        /// <param name="gradeId"></param>
        /// <returns></returns>
        List<PropanePump> GetPumpsByPropaneGradeId(int gradeId);

        /// <summary>
        /// Get Position Id by Pump ID and Grade ID
        /// </summary>
        /// <param name="pumpId"></param>
        /// <param name="gradeId"></param>
        /// <returns></returns>
        int GetPositionId(int pumpId, int gradeId);


        /// <summary>
        /// Method to unlock prepay
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns></returns>
        List<UnCompleteSale> LoadMyUnlockedPrepay(int tillNumber);


        /// <summary>
        /// Update Tier and Level for pump
        /// </summary>
        /// <param name="pumpId"></param>
        /// <param name="tierId"></param>
        /// <param name="levelId"></param>
        /// <returns></returns>
        bool UpdateTierLevelForPump(int pumpId, int tierId, int levelId);

        /// <summary>
        /// Method to get grades for price to display
        /// </summary>
        /// <returns></returns>
        List<string> GetGradesForPriceToDisplay();

        /// <summary>
        /// Method to get tiers for prices to display
        /// </summary>
        /// <returns></returns>
        List<string> GetTiersForPriceToDisplay();

        /// <summary>
        /// Method to get levels for prices to display
        /// </summary>
        /// <returns></returns>
        List<string> GetLevelsForPriceToDisplay();

        /// <summary>
        /// Method to save price to display
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="grade">Grade</param>
        /// <param name="level">Level</param>
        /// <param name="tier">Tier</param>
        /// <returns>True or false</returns>
        bool SavePriceToDisplay(byte row, string grade, string level, string tier);

        /// <summary>
        /// Method to get grade for price increment ids
        /// </summary>
        /// <returns>Grade price increments</returns>
        List<short> GetGradePriceIncrementIds();
    }
}
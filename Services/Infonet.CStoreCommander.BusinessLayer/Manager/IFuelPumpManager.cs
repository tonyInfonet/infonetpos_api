//using AxMSWinsockLib;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IFuelPumpManager
    {

        /// <summary>
        /// Method to add fuel manually
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <param name="amountOn">Amount</param>
        /// <param name="cobGrade">Grade</param>
        /// <param name="activePump">Active pump id</param>
        /// <param name="isCashSelected">Cash selected or not</param>
        /// <returns>Sale</returns>
        Sale AddFuelManually(int saleNumber, int tillNumber, byte registerNumber, string userCode, out ErrorMessage error, float amountOn, string cobGrade, short activePump, bool isCashSelected);

        /// <summary>
        /// Method to set fuel price manually
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <param name="amountOn">Amount</param>
        /// <param name="cobGrade">Grade</param>
        /// <param name="activePump">Active pump or not</param>
        /// <param name="isCashSelected">Cash selcted or not</param>
        /// <returns>Sale</returns>
        Sale cmdSet_ClickEvent(int saleNumber, int tillNumber, byte registerNumber,
            string userCode, out ErrorMessage error, float amountOn, string cobGrade, short activePump, bool isCashSelected);

        /// <summary>
        /// Method to show big pump action
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="stopPressed">Stop pressed</param>
        /// <param name="resumePressed">Resume pressed</param>
        /// <param name="error">Error message</param>
        /// <returns>Big pump</returns>
        BigPump PumpAction(short pumpId, bool stopPressed, bool resumePressed, out ErrorMessage error);

        /// <summary>
        /// Method to read price change notification
        /// </summary>
        /// <returns>Messge</returns>
        MessageStyle ReadPricheChangeNotificationHo();

        /// <summary>
        /// Method to read UDP
        /// </summary>
        /// <param name="udpReading">UDP reading</param>
        /// <returns>Pump status</returns>
        PumpStatus ReadUdp(string udpReading);

        /// <summary>
        /// Method to load change increment
        /// </summary>
        /// <param name="taxExempt"></param>
        /// <returns></returns>
        ChangeIncrement LoadChangeIncrement(bool taxExempt);

        /// <summary>
        /// Method to read UDP data
        /// </summary>
        /// <returns>Pump status</returns>
        PumpStatus ReadUdpData(int tillNumber);

        /// <summary>
        /// Method to update price chage
        /// </summary>
        /// <param name="ans">Selected option</param>
        /// <param name="counter">Counter</param>
        /// <param name="error">Error message</param>
        /// <returns>Page name </returns>
        string UpdatePriceChange(int ans, int counter, out ErrorMessage error);

        /// <summary>
        /// Method to set price increment
        /// </summary>
        /// <param name="price"></param>
        /// <param name="taxExempt"></param>
        /// <param name="error"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        PriceIncrement SetPriceIncrement(PriceIncrement price, bool taxExempt, ref ErrorMessage error, ref string report);

        /// <summary>
        /// Method to set price decrement
        /// </summary>
        /// <param name="price"></param>
        /// <param name="taxExempt"></param>
        /// <param name="error"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        PriceDecrement SetPriceDecrement(PriceDecrement price, bool taxExempt, ref ErrorMessage error, ref string report);

        /// <summary>
        /// Method to read totalizer
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="errorMessage">Error message</param>
        void ReadTotalizer(int tillNumber, ref ErrorMessage errorMessage);

        /// <summary>
        /// Method to load list of fuel prices
        /// </summary>
        /// <param name="report">Report</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>List of fuel price</returns>
        BasePrices LoadGroupedBasePrices(ref string report, out ErrorMessage errorMessage);

        /// <summary>
        /// Method to verify base prices
        /// </summary>
        /// <param name="updatedPrices">Updated prices</param>
        /// <param name="errorMessage">Error message</param>
        void VerifyGroupedBasePrices(List<FuelPrice> updatedPrices, ref ErrorMessage errorMessage);

        /// <summary>
        /// Method to save base prices
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="prices">Prices</param>
        /// <param name="totalizer">Read totalizer or not</param>
        /// <param name="tankDip">Tank dip or not</param>
        /// <param name="priceSign">Price sign or not</param>
        /// <param name="blTot">Totalizer or not</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="messages">List of messages</param>
        string SaveGroupedBasePrices(int tillNumber, List<FuelPrice> prices, bool totalizer, bool tankDip, bool priceSign, bool blTot,
            out string priceReport,
            out string fuelPriceReport, out ErrorMessage errorMessage, ref List<MessageStyle> messages);

        /// <summary>
        /// Method to load pumps
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Pump status</returns>
        PumpStatus LoadPumps(int tillNumber);

        /// <summary>
        /// Method to stop all pumps
        /// </summary>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        bool StopAllPumps(out ErrorMessage error);

        /// <summary>
        /// Method to resume all pumps
        /// </summary>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        bool ResumeAllPumps(out ErrorMessage error);

        /// <summary>
        /// Method to get list of fuel prices
        /// </summary>
        /// <param name="fuelPrices">Fuel prices</param>
        /// <param name="row">Row number</param>
        /// <param name="report">Report</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>List of fuel prices</returns>
        List<FuelPrice> SetGroupedBasePrice(List<FuelPrice> fuelPrices, int row, ref string report, out ErrorMessage errorMessage);

        /// <summary>
        /// Method to add fuel sale from basket
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="pumpId">Pump id</param>
        /// <param name="basketValue">Basket value</param>
        /// <param name="error">Error</param>
        /// <returns>Sale</returns>
        Sale AddFuelSaleFromBasket(int saleNumber, int tillNumber, byte registerNumber, short pumpId, float basketValue, out ErrorMessage error);

        /// <summary>
        /// Method get list of pump grades
        /// </summary>
        /// <param name="pumpId">Pump id</param>
        /// <returns>List of pump grades</returns>
        List<string> LoadPumpGrades(short pumpId);

        /// <summary>
        /// Method to load list of fuel prices
        /// </summary>
        /// <param name="report"></param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>List of fuel price</returns>
        BasePrices LoadBasePrices(ref string report, out ErrorMessage errorMessage);

        /// <summary>
        /// Method to set base prices
        /// </summary>
        /// <param name="updatedPrice"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        FuelPrice SetBasePrice(ref FuelPrice updatedPrice, out ErrorMessage message);


        /// <summary>
        /// Method to verify base price
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="updatedPrices"></param>
        /// <param name="isPricesToDisplayChecked"></param>
        /// <param name="isTankDipChecked"></param>
        /// <param name="isTotalizerChecked"></param>
        /// <param name="error"></param>
        /// <param name="caption2"></param>
        /// <returns></returns>
        string VerifyBasePrices(int tillNumber, List<FuelPrice> updatedPrices, bool isPricesToDisplayChecked,
            bool isTankDipChecked, bool isTotalizerChecked, out ErrorMessage error, ref string caption2);

        /// <summary>
        /// Method to save base price
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="updatedPrices"></param>
        /// <param name="isPricesToDisplayChecked"></param>
        /// <param name="isTankDipChecked"></param>
        /// <param name="isTotalizerChecked"></param>
        /// <param name="error"></param>
        /// <param name="caption2"></param>
        /// <returns></returns>
        string SaveBasePrices(int tillNumber, List<FuelPrice> updatedPrices, bool isPricesToDisplayChecked,
            bool isTankDipChecked, bool isTotalizerChecked, out ErrorMessage error, ref string caption2);


        /// <summary>
        /// Method to read tank dip
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        bool ReadTankDip(int tillNumber, out ErrorMessage error);

        /// <summary>
        /// Method to get tank dip report
        /// </summary>
        /// <param name="readTankDipSuccess"></param>
        /// <returns></returns>
        string GetTankDipReport(bool? readTankDipSuccess);

        /// <summary>
        /// Method to load prices to display
        /// </summary>
        /// <returns></returns>
        PriceToDisplay LoadPricesToDisplay();

        /// <summary>
        /// Method to save prices to display
        /// </summary>
        /// <param name="selectedGrades"></param>
        /// <param name="selectedTiers"></param>
        /// <param name="selectedLevels"></param>
        /// <returns></returns>
        bool SavePricesToDisplay(List<string> selectedGrades, List<string> selectedTiers,
            List<string> selectedLevels);

        /// <summary>
        /// Stop Pump Broadcasting
        /// </summary>
        /// <returns></returns>
        bool DisableFramePump();
    }
}
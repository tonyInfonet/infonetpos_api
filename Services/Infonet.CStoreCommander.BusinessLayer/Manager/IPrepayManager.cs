using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IPrepayManager
    {
        /// <summary>
        /// Method to refresh prepay
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="prepayStatus">Prepay status</param>
        /// <returns>True or false</returns>
        bool RefreshPrepay(short pumpId, short prepayStatus);

        /// <summary>
        /// Method to get prepay status string
        /// </summary>
        /// <returns></returns>
       string PrepayStatusString();


        /// <summary>
        /// Method to delete prepay from POS
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <returns>True ro false</returns>
        void DeletePrepaymentFromPos(short pumpId);

        /// <summary>
        /// Method to get prepay basket
        /// </summary>
        /// <param name="strBasket">Basket</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Prepay basket</returns>
        int IsMyPrepayBasket(string strBasket, int tillNumber);

        /// <summary>
        /// Method to get prepay item Id
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <returns>Prepay item Id</returns>
        short PrepayItemId(ref Sale sale);

        /// <summary>
        /// Method to set prepayment
        /// </summary>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="amount">Amount</param>
        /// <param name="position">Postion Id</param>
        /// <returns>True or false</returns>
        void SetPrepayment(int invoiceId, short pumpId, float amount, byte position);

        /// <summary>
        /// Method to set prepayment from POS
        /// </summary>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="amount">Amount</param>
        /// <param name="mop">MOP</param>
        /// <param name="positionId">Postion Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>True or false</returns>
        bool SetPrepaymentFromPos(int invoiceId, short pumpId, float amount, byte mop, byte positionId, int tillNumber);

        /// <summary>
        /// Method to swirch prepay
        /// </summary>
        /// <param name="oldPumpId">Old pump Id</param>
        /// <param name="newPumpId">New pump Id</param>
        /// <returns>True or false</returns>
        void SwitchPrepayment(short oldPumpId, short newPumpId);

        /// <summary>
        /// Method to hold prepayment
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <returns>True or false</returns>
        void HoldPrepayment(short pumpId);

        /// <summary>
        /// Method to set prepay from Fuel control
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="prepayPriceType">Prpeay price type</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        bool SetPrepayFromFc(short pumpId,int tillNumber, int saleNumber, string prepayPriceType, out ErrorMessage 
            error);

        /// <summary>
        /// Method to delete prepay from POS
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="showMessage">Show message or not</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        bool DeletePrepayFromFc(short pumpId, bool showMessage, out ErrorMessage error);

        /// <summary>
        /// Undo Basket
        /// </summary>
        /// <param name="pumpId"></param>
        /// <param name="posId"></param>
        /// <param name="gradeId"></param>
        /// <param name="amount"></param>
        /// <param name="up"></param>
        /// <param name="volume"></param>
        /// <param name="stockCode"></param>
        /// <param name="mop"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        bool BasketUndo(short pumpId, short posId, short gradeId, float amount, float up, float volume,
            string stockCode, byte mop, out ErrorMessage error);
    }
}
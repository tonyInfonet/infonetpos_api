using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IFuelPrepayManager
    {
        /// <summary>
        /// Method to add prepay
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="activePump">Active pump</param>
        /// <param name="amountOn">Amount</param>
        /// <param name="cobGradeText">Grade text</param>
        /// <param name="isAmountTypeCash">Cash type</param>
        /// <param name="error">Error message</param>
        /// <returns>Sale</returns>
        Sale AddPrepay(int saleNumber, int tillNumber, short activePump, float amountOn,
            string cobGradeText, bool isAmountTypeCash, out ErrorMessage error);

        /// <summary>
        /// Method to delete prepay
        /// </summary>
        /// <param name="activePump">Active pump</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="error">Error</param>
        /// <returns>Sale summary response</returns>
        SaleSummaryResponse DeletePrepay(short activePump, int saleNumber, int tillNumber,
            int shiftNumber, byte registerNumber, out ErrorMessage error);

        /// <summary>
        /// Method to switch prepay
        /// </summary>
        /// <param name="activePump">Active pump Id</param>
        /// <param name="newPumpId">New pump Id</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="error">Error message</param>
        /// <returns>True ro false</returns>
        bool SwitchPrepay(short activePump, short newPumpId, int saleNumber, int tillNumber,
            out ErrorMessage error);


        /// <summary>
        /// Method to finish switch prepay basket
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="changeDue">Change due message</param>
        /// <param name="openDrawer">Open drawer</param>
        /// <param name="fs">File stream</param>
        void Finish_SwitchPrepayBaskets(int tillNumber, out string changeDue,
            out bool openDrawer, out Report fs);

        /// <summary>
        /// Method to remove prepay basket
        /// </summary>
        /// <param name="baskId">Basket Id</param>
        /// <param name="prepayInvoice">Prepay invoice</param>
        /// <returns>True or false</returns>
        bool RemovePrepayBasket(string baskId, string prepayInvoice);

        /// <summary>
        /// Method to update prepay sale
        /// </summary>
        /// <param name="invoiceId">Sale number</param>
        /// <param name="saleAmount">Sale amount</param>
        /// <param name="saleQuantity">Sale quantity</param>
        /// <param name="unitPrice">Unit price</param>
        /// <param name="lessAmount">Less amount</param>
        /// <param name="overPayment">Over payment</param>
        /// <param name="goToTender">Go to tender</param>
        /// <param name="iPositionId">Position Id</param>
        /// <param name="iGradeId">Grade Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="changeDue">Change due</param>
        /// <param name="openDrawer">Open drawer</param>
        /// <param name="fs">File stream</param>
        /// <returns>True or false</returns>
        bool UpdatePrepaySale(int invoiceId, float saleAmount, float saleQuantity, float unitPrice,
            float lessAmount, bool overPayment, bool goToTender, short iPositionId, short iGradeId, int tillNumber
            , out string changeDue, out bool openDrawer, out Report fs);

    }
}
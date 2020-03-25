using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using System;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IGivexManager
    {

        /// <summary>
        /// Get a Valid GiveX Stockcode
        /// </summary>
        /// <param name="errorMessage">errorMessage</param>        
        /// <returns>stock code</returns>
        string GetValidGiveXStock(out ErrorMessage errorMessage);

        /// <summary>
        /// Adjust Givex Card with the new amount
        /// </summary>
        /// <param name="givexCardNumber">givexCardNumber</param>
        /// <param name="amount">amount</param>
        /// <param name="userCode">userCode</param>
        /// <param name="tillNumber">tillNumber</param>
        /// <param name="saleNumber">saleNumber</param>
        /// <param name="stockCode">stockCode</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>Sale</returns>
        Sale AdjustGivexCard(string givexCardNumber, decimal amount, string userCode,
                                        int tillNumber, int saleNumber,string stockCode,
                                        out Report givexReceipt, out ErrorMessage errorMessage);


        /// <summary>
        /// Activate Givex Card
        /// </summary>
        /// <param name="givexCardNumber">givexCardNumber</param>
        /// <param name="givexPrice">givexPrice</param>
        /// <param name="userCode">userCode</param>
        /// <param name="tillNumber">tillNumber</param>
        /// <param name="saleNumber">saleNumber</param>
        /// <param name="stockCode">stockCode</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>Sale</returns>
        Sale ActivateGivexCard(string givexCardNumber, decimal givexPrice, string userCode,
                                        int tillNumber, int saleNumber, string stockCode, 
                                        out Report givexReceipt, out ErrorMessage errorMessage);

        /// <summary>
        /// Deactivate Givex Card
        /// </summary>
        /// <param name="givexCardNumber">givexCardNumber</param>
        /// <param name="givexPrice">givexPrice</param>
        /// <param name="userCode">userCode</param>
        /// <param name="tillNumber">tillNumber</param>
        /// <param name="saleNumber">saleNumber</param>
        /// <param name="stockCode">stockCode</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>Sale</returns>
        Sale DeactivateGivexCard(string givexCardNumber, decimal givexPrice, string userCode,
                                        int tillNumber, int saleNumber, string stockCode, 
                                        out Report givexReceipt, out ErrorMessage errorMessage);

        /// <summary>
        /// Get Givex Card Balance
        /// </summary>
        /// <param name="givexCardNumber">givexCardNumber</param>
        /// <param name="saleNumber">saleNumber</param>
        /// <param name="tillNumber"></param>
        /// <param name="userCode">userCode</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>balance</returns>
        decimal GetCardBalance(string givexCardNumber, int saleNumber, int tillNumber, string userCode, 
            out Report givexReceipt, out ErrorMessage errorMessage);


        /// <summary>
        /// Increase Givex Card value
        /// </summary>
        /// <param name="givexCardNumber">givexCardNumber</param>
        /// <param name="givexPrice">givexPrice</param>
        /// <param name="userCode">userCode</param>
        /// <param name="tillNumber">tillNumber</param>
        /// <param name="saleNumber">saleNumber</param>
        /// <param name="stockCode">stockCode</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>Sale</returns>
        Sale IncreaseGivexCard(string givexCardNumber, decimal givexPrice, string userCode,
                                        int tillNumber, int saleNumber, string stockCode,
                                        out Report givexReceipt, out ErrorMessage errorMessage);

        /// <summary>
        /// Close Batch
        /// </summary>
        /// <param name="saleNumber">saleNumber</param>
        /// <param name="tillNumber">tillNumber</param>
        /// <param name="userCode">userCode</param>
        /// <param name="givexReceipt">Givex receipt</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>true/false</returns>
        bool CloseBatch(int saleNumber, int tillNumber, string userCode, out Report givexReceipt,
            out ErrorMessage errorMessage);

        /// <summary>
        /// Get Givex Report
        /// </summary>
        /// <param name="reportDate"></param>
        /// <returns></returns>
        GivexReport GetGivexReport(DateTime reportDate);
    }
}

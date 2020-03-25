using System;
using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IReturnSaleService
    {
        
        /// <summary>
        /// Get All Sales
        /// </summary>
        /// <param name="saleDate">Sale date</param>
        /// <param name="timeFormat">Time format</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of sale head</returns>
        List<SaleHead> GetAllSales(DateTime saleDate, string timeFormat, int pageIndex, int pageSize);

        /// <summary>
        /// Search Sale
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="slDate">Sale time</param>
        /// <param name="timeFormat">Time format</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of sale head</returns>
        List<SaleHead> SearchSale(int? saleNumber, DateTime? saleDate, DateTime slDate,
            string timeFormat, int pageIndex, int pageSize);


        /// <summary>
        /// Get SaleBy SaleNumber
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="defaultCustCode">Default customer code</param>
        /// <param name="isSaleFound">Sale found or not</param>
        /// <param name="isReturnable">Sale returned or not</param>
        /// <param name="teType">Taxe exempt type</param>
        /// <param name="teGetName">Tax exempt name</param>
        /// <param name="taxExemptGa">Tax exempt ga</param>
        /// <param name="pDefaultCustomer">Deafult customer</param>
        /// <returns>Sale</returns>
        Sale GetSaleBySaleNumber(int saleNumber, int tillNumber, DateTime saleDate, string teType,
            bool teGetName, bool taxExemptGa, bool pDefaultCustomer, string defaultCustCode, 
            out bool isSaleFound, out bool isReturnable);


        /// <summary>
        /// Gets the List of Sale Lines
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="discountType">Discount type</param>
        /// <param name="teType">Tax exempt type</param>
        /// <param name="taxExempt">Tax exempt name</param>
        /// <returns>List of sale lines</returns>
        List<Sale_Line> GetSaleLineBySaleNumber(int saleNumber, int tillNumber, DateTime saleDate, 
            string discountType, string teType, bool taxExempt);


        /// <summary>
        /// Checks whether correction is allowed
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>True or false</returns>
        bool IsAllowCorrection(int saleNumber);


        /// <summary>
        /// Checks whether reason allowed
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <returns>List of sale lines</returns>
        List<Sale_Line> IsReasonAllowed(int saleNumber);

        /// <summary>
        /// Checks If Sale Exists
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="saleDate">Sale date</param>
        /// <param name="isSaleFound">Sale found</param>
        /// <param name="isReturnable">Sale returnable or not</param>
        void IsSaleExist(int saleNumber, DateTime saleDate, out bool isSaleFound, out bool
            isReturnable);
    }
}
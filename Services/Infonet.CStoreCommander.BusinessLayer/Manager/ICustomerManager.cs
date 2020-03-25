using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.BusinessLayer.Utilities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    /// <summary>
    /// Customer Manager iterface 
    /// </summary>
    public interface ICustomerManager
    {
        /// <summary>
        /// Get all Customers
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>list of customer</returns>
        List<Customer> GetCustomers(int pageIndex = 1, int pageSize = 100);


        /// <summary>
        /// Get ALL AR Customers
        /// </summary>
        /// <param name="error"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<Customer> GetArCustomers(out ErrorMessage error, int pageIndex = 1, int pageSize = 100);

        /// <summary>
        /// Get customer by code 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Customer GetCustomerByCode(string code);

        /// <summary>
        /// Get client card by card number
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        ClientCard GetClientCardByCardNumber(string cardNumber);

        /// <summary>
        /// search customer by search term
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="totalResults"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<Customer> Search(string searchCriteria, out int totalResults, int pageIndex = 1, int pageSize = 100);

        /// <summary>
        /// Search AR Customer
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="error"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<Customer> SearchArCustomer(string searchCriteria, out ErrorMessage error, int pageIndex = 1, int pageSize = 100);

        /// <summary>
        /// Get AR Customer By card Number
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        Customer GetArCustomerByCardNumber(string cardNumber, out ErrorMessage error);


        /// <summary>
        /// Search Ar card
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="isLoyaltycard"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Customer SearchCustomerCard(string cardNumber, bool isLoyaltycard, out MessageStyle message);

        /// <summary>
        /// Get all Loyalty Customers
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>list of loyalty customer</returns>
        List<Customer> GetLoyaltyCustomers(int pageIndex = 1, int pageSize = 100);

        /// <summary>
        /// Checks Customer by Customer code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        bool IsCustomerExist(string code);

        /// <summary>
        /// Check customer by Loyalty Number
        /// </summary>
        /// <param name="customerModelLoyaltyNumber"></param>
        /// <returns></returns>
        bool CheckCustomerByLoyaltyNumber(string customerModelLoyaltyNumber);

        /// <summary>
        /// Save Customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        bool SaveCustomer(Customer customer);

        /// <summary>
        /// Search loyalty Customers
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalResults"></param>
        /// <returns></returns>
        IList<Customer> SearchLoyaltyCustomer(string searchTerm, out int totalResults, int pageIndex = 1, int pageSize = 100);

        /// <summary>
        /// Method to load a customer
        /// </summary>
        /// <param name="customerCode">Customer code</param>
        /// <returns>Customer</returns>
        Customer LoadCustomer(string customerCode);

        /// <summary>
        /// Method to get client group for customer
        /// </summary>
        /// <param name="customer">Customer</param>
        void GetClientGroup(ref Customer customer);

        /// <summary>
        /// Method to set a customer as loyalty customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        bool SetLoyaltyCustomer(Customer customer, out ErrorMessage error);


        string EvaluateCardString(string txtSwipeData, out MessageStyle errorMessage);
    }
}

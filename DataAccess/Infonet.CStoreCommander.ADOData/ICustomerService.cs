using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.ADOData
{
    public interface ICustomerService
    {
        /// <summary>
        /// Get Customer
        /// </summary>
        /// <param name="showCardCustomers"></param>
        /// <param name="additionalCriteria"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<Customer> GetCustomers(bool showCardCustomers, string additionalCriteria, int pageIndex = 1, int pageSize = 100);

        /// <summary>
        /// Get AR Customers
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<Customer> GetArCustomers(int pageIndex = 1, int pageSize = 100);

        /// <summary>
        /// Get Client Card by cardNumber
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        ClientCard GetClientCardByCardNumber(string cardNumber);

        /// <summary>
        /// Get Client by clientCode
        /// </summary>
        /// <param name="clientCode"></param>
        /// <returns></returns>
        Customer GetClientByClientCode(string clientCode);

        /// <summary>
        /// Search Customer 
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="totalrecords"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<Customer> Search(string searchCriteria, out int totalrecords, int pageIndex = 1, int pageSize = 100);

        /// <summary>
        /// Search AR customers
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="totalrecords"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<Customer> SearchArCustomer(string searchCriteria, out int totalrecords, int pageIndex = 1, int pageSize = 100);

        /// <summary>
        /// Checks Whether AR Customer or Not
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        bool IsArCustomer(string code);

        /// <summary>
        /// Updated Customer
        /// </summary>
        /// <param name="customer"></param>
        void UpdateCustomer(Customer customer);

        /// <summary>
        /// Get loyalty customers
        /// </summary>
        /// <param name="additionalCriteria"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<Customer> GetLoyaltyCustomers(string additionalCriteria, int pageIndex = 1, int pageSize = 100);

        /// <summary>
        /// Checks customer by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        bool IsCustomerExist(string code);

        /// <summary>
        /// checks customer by Loyalty number
        /// </summary>
        /// <param name="customerModelLoyaltyNumber"></param>
        /// <returns></returns>
        bool CheckCustomerByLoyaltyNumber(string customerModelLoyaltyNumber);

        /// <summary>
        /// Save customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        bool SaveCustomer(Customer customer);

        /// <summary>
        /// Search Loyalty Number
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="totalrecords"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList<Customer> SearchLoyaltyCustomer(string searchTerm, out int totalrecords, int pageIndex = 1, int pageSize = 100);


        /// <summary>
        /// Gets KickBack record by Card Number
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        KickBack GetKickBackRecordByCardNumber(string cardNumber);

        /// <summary>
        /// Method to get customer card profile
        /// </summary>
        /// <param name="customerCard">Customer card</param>
        /// <returns>Customer card profile id</returns>
        string GetCustomerCardProfile(string customerCard);


        /// <summary>
        /// Method to get customer group
        /// </summary>
        /// <param name="groupId">Group Id</param>
        ClientGroup GetCustomerGroup(string groupId);

        /// <summary>
        /// Get Tax Exempt Customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        teCardholder GetTaxExemptCustomer(string customerId);

        /// <summary>
        /// Get Card Holder
        /// </summary>
        /// <param name="isBarCode"></param>
        /// <param name="strNumber"></param>
        /// <param name="matchCount">Match count</param>
        /// <param name="ageRestrict">Agr restriction</param>
        /// <returns></returns>
        teCardholder GetCardHolder(bool isBarCode, string strNumber, out short matchCount, int ageRestrict);

        /// <summary>
        /// Method to get client card for customer
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>Client card</returns>
        ClientCard GetClientCardForCustomer(string cardNumber);

        /// <summary>
        /// Method to find if entered PO is used by a customer
        /// </summary>
        /// <param name="customerCode">Customer code</param>
        /// <param name="poNumber">PO number</param>
        /// <returns>True or false</returns>
        bool UsedCustomerPo(string customerCode, string poNumber);



        /// <summary>
        /// Get Gasking customer by clientCode
        /// </summary>
        /// <param name="clientCode"></param>
        /// <returns></returns>
        ClientCard GetClientCardForGasKingCustomer(string code);


        bool Check_Allowredemption(string customerCard);
    }
}

using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class CustomerManager : ManagerBase, ICustomerManager
    {
        private readonly ICustomerService _customerService;
        private readonly IPolicyManager _policyManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ICreditCardManager _creditCardManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerService"></param>
        /// <param name="policyManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="creditCardManager"></param>
        public CustomerManager(ICustomerService customerService,
            IPolicyManager policyManager,
            IApiResourceManager resourceManager,
            ICreditCardManager creditCardManager
        )
        {
            _customerService = customerService;
            _policyManager = policyManager;
            _resourceManager = resourceManager;
            _creditCardManager = creditCardManager;
        }

        /// <summary>
        /// Get all Customers
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>list of customer</returns>
        public List<Customer> GetCustomers(int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,CustomerManager,GetCustomers,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var additionalCriteria = string.Empty;

            if (_policyManager.TAX_EXEMPT && _policyManager.TE_Type == "SITE" && _policyManager.TE_ByRate &&
                _policyManager.IDENTIFY_MEMBER)
            {
                additionalCriteria = " AND CLIENT.CL_CODE <> \'" + _policyManager.BANDMEMBER +
                                     "\' and CLIENT.CL_CODE <> \'" + _policyManager.NONBANDMEMBER + "\'";
            }

            var customers = _customerService.GetCustomers(_policyManager.ShowCardCustomers, additionalCriteria,
                pageIndex, pageSize);
            Performancelog.Debug(
                $"End,CustomerManager,GetCustomers,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return customers;
        }

        /// <summary>
        /// Get all AR Customers
        /// </summary>
        /// <param name="error"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>list of customer</returns>
        public List<Customer> GetArCustomers(out ErrorMessage error, int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            error = new ErrorMessage();
            Performancelog.Debug($"Start,CustomerManager,GetArCustomers,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var customers = _customerService.GetArCustomers(pageIndex, pageSize);
            if (customers.Count == 0 && (pageIndex == 1 || pageIndex == 0))
            {
                //Chaps_Main.DisplayMessage(this, (short)91, temp_VbStyle, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 41, 91, null, CriticalOkMessageType)
                };
                return null;
            }

            Performancelog.Debug(
                $"End,CustomerManager,GetArCustomers,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return customers;
        }

        /// <summary>
        /// Get Customer By Code
        /// </summary>
        /// <param name="code">client code</param>
        /// <returns>customer object</returns>
        public Customer GetCustomerByCode(string code)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug(
                $"Start,CustomerManager,GetCustomerByCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var customer = _customerService.GetClientByClientCode(code);
            Performancelog.Debug(
                $"End,CustomerManager,GetCustomerByCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return customer;
        }

        /// <summary>
        /// Get AR Customer By Card Number
        /// </summary>
        /// <param name="cardNumberString"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public Customer GetArCustomerByCardNumber(string cardNumberString, out ErrorMessage error)
        {
            error = new ErrorMessage();
            string strCustomer;
            string strStatus = "";
            bool boolCardClient;
            DateTime dExpDate = default(DateTime);
            Customer customer;
            string tempAvoidedValuesString = "";
            MessageStyle message;
            var cardNumber = EvaluateCardString(cardNumberString, out message);
            if (!string.IsNullOrEmpty(message.Message))
            {
                error.MessageStyle = message;
                return null;
            }
            SqlQueryCheck(ref cardNumber, ref tempAvoidedValuesString);

            ClientCard clientCard = _customerService.GetClientCardByCardNumber(cardNumber);

            if (clientCard != null)
            {
                strCustomer = clientCard.ClientCode;
                strStatus = clientCard.CardStatus.ToString();
                dExpDate = clientCard.ExpirationDate;
                boolCardClient = true;
            }
            else
            {
                boolCardClient = false;
                strCustomer = cardNumber.Trim();
            }

            var blnFoundCst = _customerService.IsArCustomer(strCustomer);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (!blnFoundCst)
            {
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 41, 99, strCustomer, OkMessageType)
                };
                return null;
            }
            if (strStatus == "V" && boolCardClient && _policyManager.CUST_EXPDATE &&
                dExpDate != DateTime.Parse("12:00:00 AM") && dExpDate < DateAndTime.Today)
            {
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 1492, dExpDate, OkMessageType)
                };
                return null;
            }


            // If the customer is found and has a card, verify that the card is valid
            if (strStatus != "V" && boolCardClient)
            {
                switch (strStatus)
                {
                    case "C":
                        error = new ErrorMessage
                        {
                            MessageStyle =
                                _resourceManager.CreateMessage(offSet, 0, 1708, _resourceManager.GetResString(offSet, 1710),
                                    OkMessageType)
                        };
                        break;
                    case "D":
                        error = new ErrorMessage
                        {
                            MessageStyle =
                                _resourceManager.CreateMessage(offSet, 0, 1708, _resourceManager.GetResString(offSet, 8158),
                                    OkMessageType)
                        };
                        break;
                    case "E":
                        error = new ErrorMessage
                        {
                            MessageStyle =
                                _resourceManager.CreateMessage(offSet, 0, 1708, _resourceManager.GetResString(offSet, 1709),
                                    OkMessageType)
                        };
                        break;
                    default:
                        error = new ErrorMessage
                        {
                            MessageStyle = _resourceManager.CreateMessage(offSet, 0, 3884, null, OkMessageType)
                        };
                        break;
                }
                blnFoundCst = false;
            }

            if (blnFoundCst)
            {
                customer = new Customer { Code = strCustomer };
                customer = LoadCustomer(customer.Code);
            }
            else
            {
                return null;
            }
            return customer;
        }

        /// <summary>
        ///  Get Client Card By CardNumber
        /// </summary>
        /// <param name="cardNumber">card number</param>
        /// <returns>ClientCard</returns>
        public ClientCard GetClientCardByCardNumber(string cardNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug(
                $"Start,CustomerManager,GetCustomerByCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var clientCard = _customerService.GetClientCardByCardNumber(cardNumber);
            Performancelog.Debug(
                $"End,CustomerManager,GetClientCardByCardNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return clientCard;
        }

        /// <summary> 
        /// 
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="totalResults"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Customer> Search(string searchCriteria, out int totalResults, int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,CustomerManager,Search,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var cardNumber = searchCriteria;

            var customer = _customerService.Search(searchCriteria, out totalResults, pageIndex, pageSize);
            if (customer.Count > 0)
            {
                return customer;
            }
            string tempAvoidedValuesString = "";
            SqlQueryCheck(ref searchCriteria, ref tempAvoidedValuesString);
            var customerByCard = _customerService.GetClientCardByCardNumber(searchCriteria);
            if (customerByCard != null && customer.All(c => c.Code != customerByCard.ClientCode))
            {
                //check if get by card
                totalResults = -999;
                var cardCustomer = _customerService.GetClientByClientCode(customerByCard.ClientCode);
                customer.Add(cardCustomer);
            }
            else
            {
                string strExp;
                string strStatus;
                DateTime dExpiryDate;
                var boolExpiredCard = false;
                var blnFoundCst = false;

                tempAvoidedValuesString = "";
                MessageStyle message = null;
                var crdNumber = EvaluateCardString(cardNumber, out message);
                if (!string.IsNullOrEmpty(message.Message))
                {
                    return null;
                }
                SqlQueryCheck(ref crdNumber, ref tempAvoidedValuesString);
                var strCustomerCode = GetCustomerBasedOnClientCard(crdNumber, out strExp, out strStatus, out dExpiryDate);
                var crd = _customerService.GetClientByClientCode(strCustomerCode);
                if (crd != null)
                {
                    totalResults = -999;
                    customer.Add(crd);
                }
            }
            Performancelog.Debug(
                $"End,CustomerManager,Search,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return customer;
        }

        /// <summary> 
        /// Search AR Customer
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="error"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Customer> SearchArCustomer(string searchCriteria, out ErrorMessage error, int pageIndex = 1,
            int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            error = new ErrorMessage();
            var message = new MessageStyle();
            Performancelog.Debug($"Start,CustomerManager,SearchArCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            int totalResults;

            string strCustomer;
            string strStatus = "";
            bool boolCardClient;
            DateTime dExpDate = default(DateTime);
            Customer customerByCard;
            string tempAvoidedValuesString = "";
            var cardNumber = EvaluateCardString(searchCriteria, out message);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (string.IsNullOrEmpty(cardNumber))
            {
                var customer = _customerService.SearchArCustomer(searchCriteria, out totalResults, pageIndex, pageSize);

                if (totalResults == 0)
                {
                    offSet = _policyManager.LoadStoreInfo().OffSet;
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 41, 99, searchCriteria, OkMessageType)
                    };
                    return null;
                }

                Performancelog.Debug(
                    $"End,CustomerManager,SearchArCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return customer;
            }

            SqlQueryCheck(ref cardNumber, ref tempAvoidedValuesString);

            ClientCard clientCard = _customerService.GetClientCardByCardNumber(cardNumber);

            if (clientCard != null)
            {
                strCustomer = clientCard.ClientCode;
                strStatus = clientCard.CardStatus.ToString();
                dExpDate = clientCard.ExpirationDate;
                boolCardClient = true;
            }
            else
            {
                boolCardClient = false;
                strCustomer = cardNumber.Trim();
            }

            var blnFoundCst = _customerService.IsArCustomer(strCustomer);

            if (blnFoundCst)
            {
                customerByCard = new Customer { Code = strCustomer };
                customerByCard = LoadCustomer(customerByCard.Code);
                return new List<Customer> { customerByCard };
            }
            else
            {
                var customer = _customerService.SearchArCustomer(searchCriteria, out totalResults, pageIndex, pageSize);

                if (totalResults == 0)
                {
                    offSet = _policyManager.LoadStoreInfo().OffSet;
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 41, 99, searchCriteria, OkMessageType)
                    };
                    return null;
                }

                Performancelog.Debug(
                    $"End,CustomerManager,SearchArCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return customer;
            }
        }

        /// <summary>
        /// Search card by card number 
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="isLoyaltycard"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Customer SearchCustomerCard(string cardNumber, bool isLoyaltycard, out MessageStyle message)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug(
                $"Start,CustomerManager,SearchCustomerCard,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            Customer customer = null;
            string strExp;
            string strStatus;
            DateTime dExpiryDate;
            var boolExpiredCard = false;
            var blnFoundCst = false;

            string tempAvoidedValuesString = "";
            var crdNumber = EvaluateCardString(cardNumber, out message);
            if (!string.IsNullOrEmpty(message.Message))
            {
                return null;
            }
            SqlQueryCheck(ref crdNumber, ref tempAvoidedValuesString);
            var strCustomerCode = GetCustomerBasedOnClientCard(crdNumber, out strExp, out strStatus, out dExpiryDate);

            if (!string.IsNullOrEmpty(strCustomerCode))
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                blnFoundCst = true;
                //To check card status
                if (strStatus != "V")
                {
                    switch (strStatus)
                    {
                        case "C":
                            message = _resourceManager.CreateMessage(offSet, 0, 1708, _resourceManager.GetResString(offSet, 1710),
                                MessageType.OkOnly);
                            break;
                        case "D":
                            message = _resourceManager.CreateMessage(offSet, 0, 1708, _resourceManager.GetResString(offSet, 8158),
                                MessageType.OkOnly);
                            break;
                        case "E":
                            message = _resourceManager.CreateMessage(offSet, 0, 1708, _resourceManager.GetResString(offSet, 1709),
                                MessageType.OkOnly);
                            break;
                        default:
                            message = _resourceManager.CreateMessage(offSet, 0, 3884, null, MessageType.OkOnly);
                            break;
                    }
                    Performancelog.Debug(
                        $"End,CustomerManager,SearchCustomerCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                    return null;
                }
                customer = _customerService.GetClientByClientCode(strCustomerCode);
                dExpiryDate = DateAndTime.DateSerial(int.Parse(Strings.Left(strExp, 2)),
                    int.Parse(Strings.Right(strExp, 2)), int.Parse("20"));
                WriteToLogFile("dExpiryDate set to " + Convert.ToString(dExpiryDate, CultureInfo.InvariantCulture));
                // enabled
                if (_policyManager.CUST_EXPDATE && dExpiryDate < DateAndTime.Today)
                {
                    message = _resourceManager.CreateMessage(offSet, 0, 1708, dExpiryDate.ToString("yyMM"));
                    boolExpiredCard = true;
                    blnFoundCst = false;
                }
            }

            if (!blnFoundCst && !boolExpiredCard)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                message = isLoyaltycard
                    ? _resourceManager.CreateMessage(offSet, 16, 91, null, ExclamationOkMessageType)
                    : _resourceManager.CreateMessage(offSet, 0, 8119, cardNumber, OkMessageType);
            }
            Performancelog.Debug(
                $"End,CustomerManager,SearchCustomerCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return customer;
        }

        /// <summary>
        /// Get all Loyalty Customers
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>list of loyalty customer</returns>
        public List<Customer> GetLoyaltyCustomers(int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug(
                $"Start,CustomerManager,GetLoyaltyCustomers,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var additionalCriteria = string.Empty;

            if (_policyManager.TAX_EXEMPT && _policyManager.TE_Type == "SITE" && _policyManager.TE_ByRate &&
                _policyManager.IDENTIFY_MEMBER)
            {
                additionalCriteria = " AND CLIENT.CL_CODE <> \'" + _policyManager.BANDMEMBER +
                                     "\' and CLIENT.CL_CODE <> \'" + _policyManager.NONBANDMEMBER + "\'";
            }

            var customers = _customerService.GetLoyaltyCustomers(additionalCriteria, pageIndex, pageSize);
            Performancelog.Debug(
                $"End,CustomerManager,GetLoyaltyCustomers,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return customers;
        }

        /// <summary>
        /// Checks customer by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsCustomerExist(string code)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,CustomerManager,IsCustomerExist,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var status = _customerService.IsCustomerExist(code);
            Performancelog.Debug(
                $"End,CustomerManager,IsCustomerExist,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return status;
        }

        /// <summary>
        /// Checks Customer by Loyalty Number
        /// </summary>
        /// <param name="customerModelLoyaltyNumber"></param>
        /// <returns></returns>
        public bool CheckCustomerByLoyaltyNumber(string customerModelLoyaltyNumber)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug(
                $"Start,CustomerManager,CheckCustomerByLoyaltyNumber,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var status = _customerService.CheckCustomerByLoyaltyNumber(customerModelLoyaltyNumber);
            Performancelog.Debug(
                $"End,CustomerManager,CheckCustomerByLoyaltyNumber,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return status;
        }

        /// <summary>
        /// Save Customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool SaveCustomer(Customer customer)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,CustomerManager,SaveCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            bool status = _customerService.SaveCustomer(customer);
            Performancelog.Debug(
                $"End,CustomerManager,SaveCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return status;
        }

        /// <summary>
        /// Search Loyalty Customers
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="totalResults"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<Customer> SearchLoyaltyCustomer(string searchTerm, out int totalResults, int pageIndex = 1,
            int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug(
                $"Start,CustomerManager,SearchLoyaltyCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var customers = _customerService.SearchLoyaltyCustomer(searchTerm, out totalResults, pageIndex, pageSize);

            Performancelog.Debug(
                $"End,CustomerManager,SearchLoyaltyCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            if (customers?.Count > 0)
            {
                return customers;
            }

            string tempAvoidedValuesString = "";

            var error = new MessageStyle();

            var customerByCard = SearchCustomerCard(searchTerm, true, out error);
            if (customerByCard != null)
            {
                //check if get by card
                totalResults = -999;
                customers.Add(customerByCard);
            }

            return customers;
        }

        /// <summary>
        /// Method to load a customer
        /// </summary>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        public Customer LoadCustomer(string customerCode)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,CustomerManager,LoadCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            Customer customer;
            var CASH_SALE_CLIENT = "Cash Sale";
            //   to use this const to save as client for all the transaction that don't have a sepcific customer salected. It has to be same for English, French or any language supported. It was saving Cash, Cash Sale or empty strings.
            //    If Me.Code <> ""  Then ' used
            // and default customer code is set, then load it and use it

            if (_policyManager.DefaultCust && !string.IsNullOrEmpty(_policyManager.DEF_CUST_CODE))
            {
                if (customerCode == _policyManager.DEF_CUST_CODE || customerCode == CASH_SALE_CLIENT ||
                    string.IsNullOrEmpty(customerCode))
                {
                    customerCode = Convert.ToString(_policyManager.DEF_CUST_CODE);
                }
            }
            if (!string.IsNullOrEmpty(customerCode))
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                customer = _customerService.GetClientByClientCode(customerCode) ?? new Customer
                {
                    Code = customerCode,
                    Name = _resourceManager.GetResString(offSet, 400),
                    Price_Code = 1,
                    AR_Customer = false,
                    PointCardNum = "",
                    PointCardPhone = "",
                    PointCardSwipe = ""
                };
            }
            else
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                customer = new Customer
                {
                    Code = CASH_SALE_CLIENT,
                    Name = _resourceManager.GetResString(offSet, 400),
                    Price_Code = 1,
                    AR_Customer = false,
                    PointCardNum = "",
                    PointCardPhone = "",
                    PointCardSwipe = ""
                };
            }
            Performancelog.Debug(
                $"End,CustomerManager,LoadCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return customer;
        }

        /// <summary>
        /// Method to get customer card profile
        /// </summary>
        /// <param name="customerCard">Customer card</param>
        public string GetCustomerCardProfile(string customerCard)
        //  - use after crash recovery to load the profile again if I use account & swiped card at customer screen
        {
            return !string.IsNullOrEmpty(customerCard)
                ? _customerService.GetCustomerCardProfile(customerCard)
                : string.Empty;
        }

        /// <summary>
        /// Method to get client group for customer
        /// </summary>
        /// <param name="customer">Customer</param>
        public void GetClientGroup(ref Customer customer)
        {
            var clientGroup = _customerService.GetCustomerGroup(customer.GroupID);
            if (clientGroup == null)
            {
                customer.GroupID = string.Empty;
            }
            else
            {
                clientGroup.GroupName = customer.GroupName;
                clientGroup.DiscountType = customer.DiscountType;
                clientGroup.DiscountRate = customer.DiscountRate;
                clientGroup.Footer = customer.Footer;
                clientGroup.DiscountName = customer.DiscountName;
            }
        }
        //   end

        /// <summary>
        /// Method to set a customer as loyalty customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="error"></param>
        /// <returns>True or false</returns>
        public bool SetLoyaltyCustomer(Customer customer, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug(
                $"Start,CustomerManager,SetLoyaltyCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            error = new ErrorMessage();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (string.IsNullOrEmpty(customer.Code))
            {
                MessageType messageType = (int)MessageType.Critical + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8108, null, messageType);
            }
            var existingCustomer = _customerService.GetClientByClientCode(customer.Code);
            if (existingCustomer == null)
            {
                MessageType messageType = (int)MessageType.Critical + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 6581, null, messageType);
                return false;
            }
            if (!string.IsNullOrEmpty(existingCustomer.Loyalty_Code))
            {
                MessageType messageType = (int)MessageType.Critical + MessageType.OkOnly;
                error.MessageStyle = new MessageStyle
                {
                    Message = "This customer is already a loyalty customer",
                    MessageType = messageType
                };
                return false;
            }
            if (customer.Code.Length > 10)
            {
                MessageType messageType = (int)MessageType.Critical + MessageType.OkOnly;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 16, 90, null, messageType);
                return false;
            }

            if (string.IsNullOrEmpty(customer.Loyalty_Code) || customer.Loyalty_Code == "0")
            {
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 16, 94, null, MessageType.OkOnly);
                return false;
            }

            if (CheckCustomerByLoyaltyNumber(customer.Loyalty_Code))
            {
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 16, 93, null, MessageType.OkOnly);
                return false;
            }
            existingCustomer.Loyalty_Code = customer.Loyalty_Code;
            bool status = _customerService.SaveCustomer(existingCustomer);
            Performancelog.Debug(
                $"End,CustomerManager,SetLoyaltyCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return status;
        }

        #region Private methods

        /// <summary>
        /// Method to evaluate card string entered
        /// </summary>
        /// <param name="txtSwipeData">Swipeds string</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Card string</returns>
        public string EvaluateCardString(string txtSwipeData, out MessageStyle errorMessage)
        {
            errorMessage = new MessageStyle();
            Credit_Card cardRenamed = new Credit_Card();
            var startPos = (short)(txtSwipeData.IndexOf(";", StringComparison.Ordinal) + 1);
            if (startPos == 0 || Strings.Right(txtSwipeData.Trim(), 1) != "?")
            {
                //because we have loop to wait for the full data, we have to disable the timer at first
                //var timeIn = (float)DateAndTime.Timer;
                ////wait for 5 seconds TimeOut Or if we already got the proper track2
                //while ((DateAndTime.Timer - timeIn < 5) &&
                //       (txtSwipeData.IndexOf(";", StringComparison.Ordinal) + 1 == 0 ||
                //        Strings.Right(txtSwipeData.Trim(), 1) != "?"))
                //{
                //    if (DateAndTime.Timer < timeIn)
                //    {
                //        timeIn = (float)DateAndTime.Timer;
                //    }
                //}
                startPos = (short)(txtSwipeData.IndexOf(";", StringComparison.Ordinal) + 1);
                if (startPos == 0 || Strings.Right(txtSwipeData.Trim(), 1) != "?")
                {
                    var offSet = _policyManager.LoadStoreInfo().OffSet;
                    double result;
                    if (!double.TryParse(txtSwipeData, out result))
                    {
                        MessageType temp_VbStyle5 = (int)MessageType.Critical + MessageType.OkOnly;
                        errorMessage = _resourceManager.CreateMessage(offSet, 14, 77,
                            null, temp_VbStyle5);
                        return null;
                    }
                    _creditCardManager.SetCardnumber(ref cardRenamed, txtSwipeData);
                    return txtSwipeData;
                }
                cardRenamed = new Credit_Card();
                if (startPos > 1)
                {
                    _creditCardManager.SetSwipeString(ref cardRenamed, Strings.Right(txtSwipeData, txtSwipeData.Length - (startPos - 1)));
                    //cardRenamed.Swipe_String = ;
                }
                else
                {
                    _creditCardManager.SetSwipeString(ref cardRenamed, txtSwipeData);
                }
                return cardRenamed.Cardnumber;
            }
            cardRenamed = new Credit_Card();
            if (startPos > 1)
            {
                _creditCardManager.SetSwipeString(ref cardRenamed, Strings.Right(txtSwipeData, txtSwipeData.Length - (startPos - 1)));
            }
            else
            {
                _creditCardManager.SetSwipeString(ref cardRenamed, txtSwipeData);
            }

            var mvarSwipeString = cardRenamed.Swipe_String;
            cardRenamed.Card_Swiped = true;
            var n = (short)(mvarSwipeString.IndexOf("?", StringComparison.Ordinal) + 1);
            var m = (short)(mvarSwipeString.IndexOf(";", StringComparison.Ordinal) + 1);

            if (m == 1)
            {
                _creditCardManager.SetTrack1(ref cardRenamed, "");
                _creditCardManager.SetTrack2(ref cardRenamed, mvarSwipeString);
            }
            else if (n <= m - 1)
            {
                _creditCardManager.SetTrack1(ref cardRenamed, mvarSwipeString.Substring(0, n));
                _creditCardManager.SetTrack2(ref cardRenamed, mvarSwipeString.Substring(m - 1));
            }
            return cardRenamed.Cardnumber;
        }

        /// <summary>
        /// Get customer based on client card 
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="expDate"></param>
        /// <param name="cardStatus"></param>
        /// <param name="dExpiryDate"></param>
        /// <returns></returns>
        private string GetCustomerBasedOnClientCard(string cardNumber, out string expDate, out string cardStatus,
            out DateTime dExpiryDate)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug(
                $"Start,CustomerManager,GetCustomerBasedOnClientCard,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var clientCard = _customerService.GetClientCardByCardNumber(cardNumber);

            if (clientCard == null)
            {
                dExpiryDate = new DateTime();
                expDate = string.Empty;
                cardStatus = string.Empty;
                Performancelog.Debug(
                    $"End,CustomerManager,GetCustomerBasedOnClientCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return string.Empty;
            }

            dExpiryDate = clientCard.ExpirationDate;
            expDate = clientCard.ExpirationDate.ToString("yyMM");

            cardStatus = clientCard.CardStatus.ToString();

            var returnValue = clientCard.ClientCode;

            Performancelog.Debug(
                $"End,CustomerManager,GetCustomerBasedOnClientCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return returnValue;
        }

        /// <summary>
        /// Check card Number
        /// </summary>
        /// <param name="userInputString"></param>
        /// <param name="avoidedValuesString"></param>
        /// <returns></returns>
        private void SqlQueryCheck(ref string userInputString, ref string avoidedValuesString)
        {
            avoidedValuesString = avoidedValuesString.ToUpper();
            avoidedValuesString = avoidedValuesString + "##$$@@";

            if (avoidedValuesString.IndexOf("\'", StringComparison.Ordinal) + 1 == 0)
            {
                userInputString = userInputString.Replace("\'", "\"");
            }
            if (avoidedValuesString.IndexOf("--", StringComparison.Ordinal) + 1 == 0)
            {
                userInputString = userInputString.Replace("--", "");
            }
            if (avoidedValuesString.IndexOf("/*", StringComparison.Ordinal) + 1 == 0)
            {
                userInputString = userInputString.Replace("/*", "");
            }
            if (avoidedValuesString.IndexOf("*/", StringComparison.Ordinal) + 1 == 0)
            {
                userInputString = userInputString.Replace("*/", "");
            }
            if (avoidedValuesString.IndexOf(";", StringComparison.Ordinal) + 1 == 0)
            {
                userInputString = userInputString.Replace(";", "");
            }
        }


        #endregion

    }
}

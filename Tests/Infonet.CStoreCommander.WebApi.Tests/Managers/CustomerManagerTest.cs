using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.Resources;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.WebApi.Tests.Managers
{/// <summary>
/// Test Method for Customer Manger Class
/// </summary>
    [TestFixture]
    public class CustomerManagerTest
    {

        private Mock<ICustomerService> _customerService = new Mock<ICustomerService>();
        private Mock<IPolicyManager> _policyManager = new Mock<IPolicyManager>();
        private ICustomerManager _customerManager;
        private IApiResourceManager _resourceManager = new ApiResourceManager();
        private readonly Mock<ICreditCardManager> _creditCardCardManager = new Mock<ICreditCardManager>();

        [SetUp]
        public void Setup()
        {
            // Set up some testing data
            var customers = GetCustomerTestData();

            _policyManager.Setup(p => p.LoadStoreInfo()).Returns(new Store { Language = "English" });
            _customerService.Setup(a => a.GetCustomers(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(customers);

            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
                _resourceManager, _creditCardCardManager.Object);
        }


        #region Customer Tests

        /// <summary>
        /// Set the Customer Test Data
        /// </summary>
        /// <returns></returns>
        private List<Entities.Customer> GetCustomerTestData()
        {
            var firstCustomer = new Entities.Customer
            {

                Code = "1",
                Name = "TestCustomer1",
                Phone = "12345",
                CL_Status = "A"
            };

            var secondCustomer = new Customer
            {
                Code = "2",
                Name = "TestCustomer2",
                Phone = "543221",
                CL_Status = "A",
                Loyalty_Code = "777"
            };
            var customers = new List<Customer>()
            {
             firstCustomer,
             secondCustomer
            };

            return customers;

        }

        /// <summary>
        /// Set the Client Card test Data 
        /// </summary>
        /// <returns></returns>
        private List<Entities.ClientCard> GetClientCardTestData()
        {
            var firstCard = new Entities.ClientCard
            {

                CardName = "1",
                ClientCode = "1",
                CardNumber = "12345",
                ExpirationDate = System.DateTime.Now,
                Pin = "123",
                CardStatus = 'A',
                CreditLimiit = 1000.00M,
                Balance = 100.00M,
                AllowRedemption = false,
                TaxExemptedCardNumber = "345",
                ProfileID = "A"
            };

            var secondCard = new Entities.ClientCard
            {
                CardName = "2",
                ClientCode = "TestCustomer2",
                CardNumber = "45678",
                ExpirationDate = System.DateTime.Now,
                Pin = "345",
                CardStatus = 'B',
                CreditLimiit = 1000.00M,
                Balance = 100.00M,
                AllowRedemption = true,
                TaxExemptedCardNumber = "789",
                ProfileID = "B"
            };
            var customers = new List<Entities.ClientCard>()
            {
             firstCard,
             secondCard
            };

            return customers;
        }
        /// <summary>
        /// Setup
        /// </summary>

        /// <summary>
        /// Test to get all customers according to policies
        /// </summary>
        [Test]
        public void GetCustomersTest()
        {
            var expected = 2;
            _policyManager.Setup(p => p.TAX_EXEMPT).Returns(true);
            _policyManager.Setup(p => p.TE_Type).Returns("SITE");
            _policyManager.Setup(p => p.TE_ByRate).Returns(true);
            _policyManager.Setup(p => p.IDENTIFY_MEMBER).Returns(true);
            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
              _resourceManager, _creditCardCardManager.Object);
            var actual = _customerManager.GetCustomers().Count();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test Customer if customer is not present
        /// </summary>
        [Test]
        public void GetCustomersIfNoCustomerIsPresentTest()
        {
            var customer = new List<Entities.Customer>();
            var expected = 0;
            _policyManager.Setup(p => p.TAX_EXEMPT).Returns(true);
            _policyManager.Setup(p => p.TE_Type).Returns("SITE");
            _policyManager.Setup(p => p.TE_ByRate).Returns(true);
            _policyManager.Setup(p => p.IDENTIFY_MEMBER).Returns(true);
            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
              _resourceManager, _creditCardCardManager.Object);
            _customerService.Setup(c => c.GetCustomers(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(customer = new List<Entities.Customer>());
            var actual = _customerManager.GetCustomers().Count();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test Get customer by code 
        /// </summary>
        [Test]
        public void GetCustomerByCodeTest()
        {
            var expected = "2";
            var getCustomer = GetCustomerTestData();
            var customerCode = "2";
            var getCustomerByCode = getCustomer.FirstOrDefault(x => x.Code == customerCode);
            _customerService.Setup(x => x.GetClientByClientCode(It.IsAny<string>()))
                       .Returns(getCustomerByCode);
            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
               _resourceManager, _creditCardCardManager.Object);
            var actual = _customerManager.GetCustomerByCode(customerCode).Code;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test the  Get client card by card number 
        /// </summary>
        [Test]
        public void GetClientCardByCardNumberTest()
        {
            var expected = "12345";
            var getClient = GetClientCardTestData();
            var clientCard = "12345";
            var getClientByCardNumber = getClient.FirstOrDefault(x => x.CardNumber == clientCard);
            _customerService.Setup(x => x.GetClientCardByCardNumber(It.IsAny<string>()))
                       .Returns(getClientByCardNumber);
            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
                        _resourceManager, _creditCardCardManager.Object);
            var actual = _customerManager.GetClientCardByCardNumber(clientCard).CardNumber;
            Assert.AreEqual(expected, actual);
        }

        // Mark as ignored to successfully build
        /// <summary>
        /// Test the search method if search term is phone number
        /// </summary>
        [Ignore("Mark as ignored to successfully build")]
        public void SearchIfSearchTermIsPhoneNumberTest()
        {
            int totalResults;
            int pageIndex = 1;
            int pageSize = 2;
            var expected = 1;
            var getCustomer = GetCustomerTestData();
            var searchTerm = "12345";
            var getCustomerbySearchhTerm = getCustomer.Where(x => x.Name == searchTerm || x.Phone == searchTerm || x.Code == searchTerm).ToList();
            _customerService.Setup(x => x.Search(It.IsAny<string>(), out totalResults, pageIndex, pageSize))
                       .Returns(getCustomerbySearchhTerm);
            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
              _resourceManager, _creditCardCardManager.Object);
            var actual = _customerManager.Search(searchTerm, out totalResults, pageIndex, pageSize).Count;
            Assert.AreEqual(expected, actual);
        }

        private List<Customer> GetCustomerData(string searchTerm)
        {
            var getCustomer = GetCustomerTestData();
            var getCustomerbySerachTerm = getCustomer.Where(x => x.Name == searchTerm || x.Phone == searchTerm || x.Code == searchTerm).ToList();
            return getCustomerbySerachTerm;
        }

        // Mark as ignored to successfully build
        /// <summary>
        ///  Test the search method if search term is Code number
        /// </summary>
        [Ignore("Mark as ignored to successfully build")]
        public void SearchIfSearchTermIsCodeTest()
        {
            int totalRecords = 0;
            int pageIndex = 1;
            int pageSize = 2;
            var expected = 2;
            var searchTerm = "1";
            _customerService.Setup(x => x.Search(searchTerm, out totalRecords, pageIndex, pageSize))
            .Returns(GetCustomerTestData());
            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
             _resourceManager, _creditCardCardManager.Object);
            var actual = _customerManager.Search(searchTerm, out totalRecords, pageIndex, pageSize).Count;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///  Test the search method if search term is name
        /// </summary>
        //[Test]
        //public void SearchIfSearchTermIsNameTest()
        //{
        //    int totalRecords = 0;
        //    var expected = 2;
        //    var searchTerm = "TestCustomer2";
        //    _customerService.Setup(x => x.Search(It.IsAny<string>(), out totalRecords,
        //        It.IsAny<int>(), It.IsAny<int>())).Returns(GetCustomerTestData());
        //    _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
        //      _resourceManager, _creditCardCardManager.Object);
        //    var actual = _customerManager.Search(searchTerm, out totalRecords).Count;
        //    Assert.AreEqual(expected, actual);
        //}

        /// <summary>
        /// Test search Ar customer if card status is not V,C,D,E
        /// </summary>
        [Test]
        public void SearchArCardIfCardStatusIsNotVCDETest()
        {
            var customer = new Entities.Customer();
            var strExp = string.Empty;
            DateTime dExpiryDate;
            var expected = "Invalid card !";
            MessageStyle message;
            var cardNumber = "12345";
            var getClient = GetClientCardTestData();
            var getClientByCardNumber = getClient.FirstOrDefault(x => x.CardNumber == cardNumber);
            _customerService.Setup(x => x.GetClientCardByCardNumber(It.IsAny<string>()))
                       .Returns(getClientByCardNumber);
            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
              _resourceManager, _creditCardCardManager.Object);
            var actual = _customerManager.SearchCustomerCard(cardNumber, false, out message);
            Assert.AreEqual(expected, message.Message);
        }

        /// <summary>
        /// Test search Ar customer if card status is V
        /// </summary>
        [Test]
        public void SearchArCardIfCardStatusIsVTest()
        {
            var customer = new Entities.Customer();
            var strExp = string.Empty;
            MessageStyle message;
            var cardNumber = "12345";
            var expected = "1";
            var customerCode = "1";
            var getClient = GetClientCardTestData();
            var getCustomer = GetCustomerTestData();
            var getClientByCardNumber = getClient.FirstOrDefault(x => x.CardNumber == cardNumber);
            _customerService.Setup(x => x.GetClientCardByCardNumber(It.IsAny<string>()))
                       .Returns(getClientByCardNumber);
            getClientByCardNumber.CardStatus = 'V';
            var getCustomerByCode = getCustomer.FirstOrDefault(x => x.Code == customerCode);
            _customerService.Setup(x => x.GetClientByClientCode(It.IsAny<string>()))
                       .Returns(getCustomerByCode);
            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
              _resourceManager, _creditCardCardManager.Object);
            var actual = _customerManager.SearchCustomerCard(cardNumber, false, out message);
            Assert.AreEqual(expected, actual.Code);
        }

        /// <summary>
        /// Test search Ar customer if card status is C not V 
        /// </summary>
        [Test]
        public void SearchArCardIfCardStatusIsCNotVTest()
        {
            var customer = new Entities.Customer();
            var strExp = string.Empty;
            MessageStyle message;
            var cardNumber = "12345";
            var expected = "Card is Canceled.";
            var customerCode = "1";
            var getClient = GetClientCardTestData();
            var getCustomer = GetCustomerTestData();
            var getClientByCardNumber = getClient.FirstOrDefault(x => x.CardNumber == cardNumber);
            _customerService.Setup(x => x.GetClientCardByCardNumber(It.IsAny<string>()))
                       .Returns(getClientByCardNumber);
            getClientByCardNumber.CardStatus = 'C';
            var getCustomerByCode = getCustomer.FirstOrDefault(x => x.Code == customerCode);
            _customerService.Setup(x => x.GetClientByClientCode(It.IsAny<string>()))
                       .Returns(getCustomerByCode);
            //_resourceManager.Setup(x => x.CreateErrorMessage(It.IsAny<short>(), It.IsAny<object>()))
            //          .Returns("Card Status type is C");
            //_customerManager = new CustomerManager(_customerService.Object, _policyManager.Object, _resourceManager);
            var actual = _customerManager.SearchCustomerCard(cardNumber, false, out message);
            Assert.AreEqual(expected, message.Message);
        }

        /// <summary>
        /// Test search Ar customer if card status is D not V 
        /// </summary>
        [Test]
        public void SearchArCardIfCardStatusIsDNotVTest()
        {
            var customer = new Entities.Customer();
            var strExp = string.Empty;
            MessageStyle message;
            var cardNumber = "12345";
            var expected = "Card is Disabled.";
            var customerCode = "1";
            var getClient = GetClientCardTestData();
            var getCustomer = GetCustomerTestData();
            var getClientByCardNumber = getClient.FirstOrDefault(x => x.CardNumber == cardNumber);
            _customerService.Setup(x => x.GetClientCardByCardNumber(It.IsAny<string>()))
                       .Returns(getClientByCardNumber);
            getClientByCardNumber.CardStatus = 'D';
            var getCustomerByCode = getCustomer.FirstOrDefault(x => x.Code == customerCode);
            _customerService.Setup(x => x.GetClientByClientCode(It.IsAny<string>()))
                       .Returns(getCustomerByCode);
            //_resourceManager.Setup(x => x.CreateErrorMessage(It.IsAny<short>(), It.IsAny<object>()))
            //          .Returns("Card Status type is D");
            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
                        _resourceManager, _creditCardCardManager.Object);
            var actual = _customerManager.SearchCustomerCard(cardNumber, false, out message);
            Assert.AreEqual(expected, message.Message);
        }

        /// <summary>
        /// Test search Ar customer if card status is E not V 
        /// </summary>
        [Test]
        public void SearchArCardIfCardStatusIsENotVTest()
        {
            var customer = new Entities.Customer();
            var strExp = string.Empty;
            MessageStyle message;
            var cardNumber = "12345";
            var expected = "Card is Expired.";
            var customerCode = "1";
            var getClient = GetClientCardTestData();
            var getCustomer = GetCustomerTestData();
            var getClientByCardNumber = getClient.FirstOrDefault(x => x.CardNumber == cardNumber);
            _customerService.Setup(x => x.GetClientCardByCardNumber(It.IsAny<string>()))
                       .Returns(getClientByCardNumber);
            getClientByCardNumber.CardStatus = 'E';
            var getCustomerByCode = getCustomer.FirstOrDefault(x => x.Code == customerCode);
            _customerService.Setup(x => x.GetClientByClientCode(It.IsAny<string>()))
                       .Returns(getCustomerByCode);
            //_resourceManager.Setup(x => x.CreateErrorMessage(It.IsAny<short>(), It.IsAny<object>()))
            //          .Returns("Card Status type is E");
            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
              _resourceManager, _creditCardCardManager.Object);
            var actual = _customerManager.SearchCustomerCard(cardNumber, false, out message);
            Assert.AreEqual(expected, message.Message);
        }
        #endregion

        #region Loyalty Customer Tests


        /// <summary>
        /// Test to get all loyalty customers according to policies
        /// </summary>
        [Test]
        public void GetLoyaltyCustomersTest()
        {
            var expected = 2;
            _policyManager.Setup(p => p.TAX_EXEMPT).Returns(true);
            _policyManager.Setup(p => p.TE_Type).Returns("SITE");
            _policyManager.Setup(p => p.TE_ByRate).Returns(true);
            _policyManager.Setup(p => p.IDENTIFY_MEMBER).Returns(true);
            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
              _resourceManager, _creditCardCardManager.Object);
            var actual = _customerManager.GetCustomers().Count();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test loyalty Customer if customer is not present
        /// </summary>
        [Test]
        public void GetLoyaltyCustomersIfNoCustomerIsPresentTest()
        {
            var customer = new List<Entities.Customer>();
            var expected = 0;
            _policyManager.Setup(p => p.TAX_EXEMPT).Returns(true);
            _policyManager.Setup(p => p.TE_Type).Returns("SITE");
            _policyManager.Setup(p => p.TE_ByRate).Returns(true);
            _policyManager.Setup(p => p.IDENTIFY_MEMBER).Returns(true);
            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
             _resourceManager, _creditCardCardManager.Object);
            _customerService.Setup(c => c.GetLoyaltyCustomers(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(customer = new List<Entities.Customer>());
            var actual = _customerManager.GetLoyaltyCustomers().Count();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test the loyalty search method if search term is phone number
        /// </summary>
        [Test]
        public void SearchLoyaltyCustomerIfSearchTermIsPhoneNumberTest()
        {
            int totalResults;
            int pageIndex = 1;
            int pageSize = 2;
            var expected = 1;
            var getCustomer = GetCustomerTestData();
            var searchTerm = "12345";
            var getCustomerbySerachTerm = getCustomer.Where(x => x.Name == searchTerm || x.Phone == searchTerm || x.Code == searchTerm).ToList();
            _customerService.Setup(x => x.SearchLoyaltyCustomer(It.IsAny<string>(), out totalResults, pageIndex, pageSize))
                       .Returns(getCustomerbySerachTerm);
            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
              _resourceManager, _creditCardCardManager.Object);
            var actual = _customerManager.SearchLoyaltyCustomer(searchTerm, out totalResults, pageIndex, pageSize).Count;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///  Test the loyalty search method if search term is Code number
        /// </summary>
        [Test]
        public void SearchLoyaltyCustomerIfSearchTermIsCodeTest()
        {
            int totalRecords = 0;
            var expected = 2;
            var searchTerm = "1";
            _customerService.Setup(x => x.SearchLoyaltyCustomer(It.IsAny<string>(), out totalRecords,
                It.IsAny<int>(), It.IsAny<int>())).Returns(GetCustomerTestData());
            var actual = _customerManager.SearchLoyaltyCustomer(searchTerm, out totalRecords).Count;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///  Test the loyalty search method if search term is name
        /// </summary>
        [Test]
        public void SearchLoyaltyCustomerIfSearchTermIsNameTest()
        {
            int totalRecords = 0;
            var expected = 2;
            var getCustomer = GetCustomerTestData();
            var searchTerm = "TestCustomer2";
            _customerService.Setup(x => x.Search(It.IsAny<string>(), out totalRecords,
                 It.IsAny<int>(), It.IsAny<int>())).Returns(GetCustomerTestData());
            var actual = _customerManager.SearchLoyaltyCustomer(searchTerm, out totalRecords).Count;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test method to Save customer with valid user
        /// </summary>
        [Test]
        public void SaveCustomerTest()
        {
            var cust = new Customer
            {
                Code = "12345",
                Name = "Test12345",
                Phone = "12345",
                Loyalty_Code = "12345",
                Loyalty_Points = 12.5
            };
            _customerService.Setup(x => x.SaveCustomer(It.IsAny<Customer>())).Returns(true);
            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
              _resourceManager, _creditCardCardManager.Object);
            var actual = _customerManager.SaveCustomer(cust);
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Test method to Save customer with Invalid user
        /// </summary>
        [Test]
        public void SaveCustomerNegativeTest()
        {
            var cust = new Customer
            {
                Code = "",
                Name = "Test12345",
                Phone = "12345",
                Loyalty_Code = "12345",
                Loyalty_Points = 12.5
            };
            _customerService.Setup(x => x.SaveCustomer(It.IsAny<Customer>())).Returns(false);
            _customerManager = new CustomerManager(_customerService.Object, _policyManager.Object,
               _resourceManager, _creditCardCardManager.Object);
            var actual = _customerManager.SaveCustomer(cust);
            Assert.IsFalse(actual);
        }


        #endregion
    }
}

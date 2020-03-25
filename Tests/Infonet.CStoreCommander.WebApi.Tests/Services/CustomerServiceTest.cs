//using NUnit.Framework;
//using Infonet.CStoreCommander.Data.ServiceClasses;
//using Moq;
//using System.Linq;
//using Infonet.CStoreCommander.Data.CSCMaster;
//using System.Collections.Generic;
//using Infonet.CStoreCommander.Data;
//using Infonet.CStoreCommander.Data.Repositories;

//namespace Infonet.CStoreCommander.WebApi.Tests
//{
//    /// <summary>
//    /// Test class for customer service
//    /// </summary>
//    [TestFixture]
//    public class CustomerServiceTest
//    {

//        private Mock<IRepository<CLIENT>> _customerRepository =
//                                                new Mock<IRepository<CLIENT>>();
//        private Mock<IRepository<ClientCard>> _cardRepository =
//                                                new Mock<IRepository<ClientCard>>();
//        private ICustomerService _customerService;


//        /// <summary>
//        /// Setup
//        /// </summary>
//        [SetUp]
//        public void Setup()
//        {
//            // Set up some testing data
//            var firstCustomer = new CLIENT
//            {
//                CL_CODE = "1",
//                CL_NAME = "TestCustomer1",
//                CL_PHONE = "12345",
//                CL_STATUS = "A"
//            };

//            var secondCustomer = new CLIENT
//            {
//                CL_CODE = "2",
//                CL_NAME = "TestCustomer2",
//                CL_PHONE = "543221",
//                CL_STATUS = "A",
//                LO_NUM = "777"
//            };

//            var firstClientCard = new ClientCard
//            {
//                CardNum = "123",
//                CL_Code = "1"
//            };

//            var secondClientCard = new ClientCard
//            {
//                CardNum = "999",
//                CL_Code = "2"
//            };

//            //create customers
//            var customers = new List<CLIENT>();
//            customers.Add(firstCustomer);
//            customers.Add(secondCustomer);

//            //create clients
//            var clientCards = new List<ClientCard>();
//            clientCards.Add(firstClientCard);
//            clientCards.Add(secondClientCard);

//            //get data
//            _customerRepository.Setup(c => c.GetAll()).Returns(customers.AsQueryable());
//            _cardRepository.Setup(c => c.GetAll()).Returns(clientCards.AsQueryable());
//            _customerService = new CustomerService(_customerRepository.Object,
//                                                   _cardRepository.Object);
//        }

//        /// <summary>
//        /// Test to get all customers
//        /// </summary>
//        [Test]
//        public void GetAllCustomersTest()
//        {
//            var expected = 2;
//            var actual = _customerService.GetAllCustomers().Count();
//            Assert.AreEqual(actual, expected);
//        }

//        /// <summary>
//        /// Test to add loyalty customers
//        /// </summary>
//        [Test]
//        public void AddLoyaltyCustomerTest()
//        {
//            var expected = true;
//            var newLoyaltyCustomer = new CLIENT
//            {
//                CL_CODE = "3",
//                CL_NAME = "New Customer",
//                CL_PHONE = "xyz",
//                LO_NUM = "123",
//                LO_POINTS = 0
//            };
//            var actual = _customerService.AddLoyaltyCustomer(newLoyaltyCustomer);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to add loyalty customer as null
//        /// </summary>
//        [Test]
//        public void AddLoyaltyCustomerWithNullTest()
//        {
//            var expected = false;
//            var actual = _customerService.AddLoyaltyCustomer(null);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get all active customers
//        /// </summary>
//        [Test]
//        public void GetActiveCustomersTest()
//        {
//            var expected = 2;
//            var actual = _customerService.GetActiveCustomers().Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get customer by code
//        /// </summary>
//        [Test]
//        public void GetCustomerByCodeTest()
//        {
//            var expected = "TestCustomer1";
//            var code = "1";
//            var actual = _customerService.GetCustomerByCode(code);
//            if (actual != null)
//                Assert.AreEqual(expected, actual.CL_NAME);

//        }


//        /// <summary>
//        /// Test to get customer by code if customer doe not exists
//        /// </summary>
//        [Test]
//        public void GetCustomerByCodeIfNotExistsTest()
//        {
//            CLIENT expected = null;
//            var code = "0";
//            var actual = _customerService.GetCustomerByCode(code);
//            Assert.AreEqual(expected, actual);

//        }

//        /// <summary>
//        /// Test to get customer by null code
//        /// </summary>
//        [Test]
//        public void GetCustomerByNullCodeTest()
//        {
//            CLIENT expected = null;
//            var actual = _customerService.GetCustomerByCode(null);
//            Assert.AreEqual(expected, actual);

//        }

//        /// <summary>
//        /// Test to get customer by empty code
//        /// </summary>
//        [Test]
//        public void GetCustomerByEmptyCodeTest()
//        {
//            CLIENT expected = null;
//            var code = string.Empty;
//            var actual = _customerService.GetCustomerByCode(code);
//            Assert.AreEqual(expected, actual);

//        }

//        /// <summary>
//        /// Test to search a customer if exists
//        /// </summary>
//        [Test]
//        public void SearchCustomerTestIfExists()
//        {
//            var expected = 1;
//            var searchTerm = "345";
//            var actual = _customerService.SearchCustomer(searchTerm, false).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to search a customer if not exists
//        /// </summary>
//        [Test]
//        public void SearchCustomerTestIfNotExists()
//        {
//            var expected = 0;
//            var searchTerm = "dump";
//            var actual = _customerService.SearchCustomer(searchTerm, false).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to search a customer by null searchTerm
//        /// </summary>
//        [Test]
//        public void SearchCustomerByNullTermTest()
//        {
//            var expected = 2;
//            var actual = _customerService.SearchCustomer(null, false).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to search a customer by empty searchTerm
//        /// </summary>
//        [Test]
//        public void SearchCustomerByEmptyTermTest()
//        {
//            var expected = 2;
//            var searchTerm = string.Empty;
//            var actual = _customerService.SearchCustomer(searchTerm, false).Count();
//            Assert.AreEqual(expected, actual);
//        }


//        /// <summary>
//        /// Test to search a loyalty customer if exists
//        /// </summary>
//        [Test]
//        public void SearchLoyaltyCustomerTestIfExists()
//        {
//            var expected = 1;
//            var searchTerm = "77";
//            var actual = _customerService.SearchCustomer(searchTerm, true).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to search a loyalty customer if not exists
//        /// </summary>
//        [Test]
//        public void SearchLoyaltyCustomerTestIfNotExists()
//        {
//            var expected = 0;
//            var searchTerm = "dump";
//            var actual = _customerService.SearchCustomer(searchTerm, true).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to search a loyalty customer by null searchTerm
//        /// </summary>
//        [Test]
//        public void SearchLoyaltyCustomerByNullTermTest()
//        {
//            var expected = 2;
//            var actual = _customerService.SearchCustomer(null, true).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to search a loyalty customer by empty searchTerm
//        /// </summary>
//        [Test]
//        public void SearchLoyaltyCustomerByEmptyTermTest()
//        {
//            var expected = 2;
//            var searchTerm = string.Empty;
//            var actual = _customerService.SearchCustomer(searchTerm, true).Count();
//            Assert.AreEqual(expected, actual);
//        }


//        /// <summary>
//        /// Test to get all client cards
//        /// </summary>
//        [Test]
//        public void GetClientCardsTest()
//        {
//            var expected = 2;
//            var actual = _customerService.GetClientCards(null, null).Count();
//            Assert.AreEqual(actual, expected);
//        }

//        /// <summary>
//        /// Test to get customer by loyalty number if exists
//        /// </summary>
//        [Test]
//        public void GetCustomerByLoyaltyNumberIfExistsTest()
//        {
//            var expected = "TestCustomer2";
//            var searchTerm = "777";
//            var actual = _customerService.GetCustomerByLoyaltyNumber(searchTerm);
//            if (actual != null)
//                Assert.AreEqual(expected, actual.CL_NAME);
//        }


//        /// <summary>
//        /// Test to get customer by loyalty number if not exists
//        /// </summary>
//        [Test]
//        public void GetCustomerByLoyaltyNumberIfNotExistsTest()
//        {
//            CLIENT expected = null;
//            var searchTerm = "000";
//            var actual = _customerService.GetCustomerByLoyaltyNumber(searchTerm);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get customer by null loyalty number
//        /// </summary>
//        [Test]
//        public void GetCustomerByNullLoyaltyNumberTest()
//        {
//            CLIENT expected = null;
//            var actual = _customerService.GetCustomerByLoyaltyNumber(null);
//            Assert.AreEqual(expected, actual);
//        }


//        /// <summary>
//        /// Test to get customer by empty loyalty number
//        /// </summary>
//        [Test]
//        public void GetCustomerByEmptyLoyaltyNumberTest()
//        {
//            CLIENT expected = null;
//            var searchTerm = string.Empty;
//            var actual = _customerService.GetCustomerByLoyaltyNumber(searchTerm);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to update a customer
//        /// </summary>
//        [Test]
//        public void UpdateCustomerTest()
//        {
//            var expected = true;
//            var updateCustomer = new CLIENT
//            {
//                CL_CODE = "1",
//                CL_NAME = "Updated Customer",
//                CL_PHONE = "xyz",
//                LO_NUM = "aaa",
//                LO_POINTS = 10
//            };
//            var actual = _customerService.UpdateCustomer(updateCustomer);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to update a null customer
//        /// </summary>
//        [Test]
//        public void UpdateNullCustomerTest()
//        {
//            var expected = false;
//            var actual = _customerService.UpdateCustomer(null);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to update a non existing customer
//        /// </summary>
//        [Test]
//        public void UpdateNonExistingCustomerTest()
//        {
//            var expected = false;
//            var updateCustomer = new CLIENT
//            {
//                CL_CODE = "99",
//                CL_NAME = "Non Existing Customer",
//                CL_PHONE = "xyz",
//                LO_NUM = "aaa",
//                LO_POINTS = 10
//            };
//            var actual = _customerService.UpdateCustomer(updateCustomer);
//            Assert.AreEqual(expected, actual);
//        }

//    }
//}

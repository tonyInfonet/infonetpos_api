//using Infonet.CStoreCommander.Data;
//using Infonet.CStoreCommander.Data.CSCAdmin;
//using Infonet.CStoreCommander.Data.CSCMaster;
//using Infonet.CStoreCommander.Data.Repositories;
//using Infonet.CStoreCommander.Data.ServiceClasses;
//using Infonet.CStoreCommander.WebApi.Controllers.V1;
//using Infonet.CStoreCommander.WebApi.Models;
//using Infonet.CStoreCommander.WebApi.Tests.Helpers;
//using Moq;
//using NUnit.Framework;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Script.Serialization;

//namespace Infonet.CStoreCommander.WebApi.Tests.Controllers
//{
//    [TestFixture]
//    public class CustomerV1ControllerTest
//    {
//        private Mock<IRepository<CLIENT>> _customerRepository =
//                                               new Mock<IRepository<CLIENT>>();
//        private Mock<IRepository<ClientCard>> _cardRepository =
//                                                new Mock<IRepository<ClientCard>>();
//        private ICustomerService _customerService;

//        private Mock<IRepository<P_COMP>> _companyPolicyRepository = new Mock<IRepository<P_COMP>>();
//        private Mock<IRepository<P_SET>> _setPolicyRepository = new Mock<IRepository<P_SET>>();
//        private Mock<IRepository<P_CANBE>> _levelPolicyRepository = new Mock<IRepository<P_CANBE>>();
//        private IPolicyService _policyService;
//        private ICustomerManager _customerManager;
//        private CustomerV1Controller _customerController;

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
//                CL_Code = "1",
//                CardStatus = "V"
//            };

//            var secondClientCard = new ClientCard
//            {
//                CardNum = "80008",
//                CL_Code = "2",
//                CardStatus = "D"
//            };

//            var firstCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "TAXEXEMPT",
//                P_SEQ = 1761,
//                Implemented = true,
//                P_NAME = "BANDMEMBER",
//                P_DESC = "Customer settings linked to Band Member:",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "STAB(Client, CL_code)",
//                P_SET = string.Empty,
//                P_VARTYPE = "C",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var secondCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "TAXEXEMPT",
//                P_SEQ = 1841,
//                Implemented = true,
//                P_NAME = "IDENTIFY_MEMBER",
//                P_DESC = "Automatically identify the Band Member using Band Account?",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "No",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var thirdCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "TAXEXEMPT",
//                P_SEQ = 1771,
//                Implemented = true,
//                P_NAME = "NONBANDMEMBER",
//                P_DESC = "Customer settings linked to Non Band Member:",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "STAB(Client, CL_code)",
//                P_SET = string.Empty,
//                P_VARTYPE = "C",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var fourthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "CUST",
//                P_SEQ = 1400,
//                Implemented = true,
//                P_NAME = "ShowCardCust",
//                P_DESC = "Display Parent Account if customer card is linked(Y/N)?",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "Yes",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var fifthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "FEATURES",
//                P_SEQ = 410,
//                Implemented = true,
//                P_NAME = "TAX_EXEMPT",
//                P_DESC = "Supports Tax Exempt",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "Yes",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };
//            var sixthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "TAXEXEMPT",
//                P_SEQ = 1050,
//                Implemented = true,
//                P_NAME = "TE_ByRate",
//                P_DESC = "Calculate price based on tax rate(AITE/QITE-Yes,SITE-No)?",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "No",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var seventhCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "TAXEXEMPT",
//                P_SEQ = 1060,
//                Implemented = true,
//                P_NAME = "TE_Type",
//                P_DESC = "Tax Exempt Type",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{SITE,AITE,QITE}",
//                P_SET = "SITE",
//                P_VARTYPE = "C",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            //create customers
//            var customers = new List<CLIENT>();
//            customers.Add(firstCustomer);
//            customers.Add(secondCustomer);

//            //create clients
//            var clientCards = new List<ClientCard>();
//            clientCards.Add(firstClientCard);
//            clientCards.Add(secondClientCard);

//            //create company policy
//            var companyPolicies = new List<P_COMP>();
//            companyPolicies.Add(firstCompanyPolicy);
//            companyPolicies.Add(secondCompanyPolicy);
//            companyPolicies.Add(thirdCompanyPolicy);
//            companyPolicies.Add(fourthCompanyPolicy);
//            companyPolicies.Add(fifthCompanyPolicy);
//            companyPolicies.Add(sixthCompanyPolicy);
//            companyPolicies.Add(seventhCompanyPolicy);

//            //create set policy
//            var setPolicies = new List<P_SET>();

//            //create level policy
//            var levelPolicies = new List<P_CANBE>();

//            //get data
//            _customerRepository.Setup(c => c.GetAll()).Returns(customers.AsQueryable());
//            _cardRepository.Setup(c => c.GetAll()).Returns(clientCards.AsQueryable());
//            _customerService = new CustomerService(_customerRepository.Object,
//                                                   _cardRepository.Object);
//            _companyPolicyRepository.Setup(u => u.GetAll()).Returns(companyPolicies.AsQueryable());
//            _setPolicyRepository.Setup(u => u.GetAll()).Returns(setPolicies.AsQueryable());
//            _levelPolicyRepository.Setup(l => l.GetAll()).Returns(levelPolicies.AsQueryable());
//            _policyService = new PolicyService(_companyPolicyRepository.Object, _setPolicyRepository.Object,
//                                                _levelPolicyRepository.Object);

//            _customerManager = new CustomerManager(_customerService, _policyService);
//            _customerController = new CustomerV1Controller(_customerManager);


//        }

//        /// <summary>
//        /// Test to get customers
//        /// </summary>
//        [Test]
//        public void GetCustomersTest()
//        {
//            _customerController.Request = new HttpRequestMessage();
//            _customerController.Configuration = new System.Web.Http.HttpConfiguration();
//            var expected = 2;
//            // Act
//            var response = _customerController.Index();

//            // Assert          
//            var json = response.Content.ReadAsStringAsync().Result;
//            var customers = JsonHelper.FromJson<List<Customer>>(json);
//            Assert.AreEqual(expected, customers.Count());
//            Assert.IsTrue(response.IsSuccessStatusCode);

//        }

//        /// <summary>
//        /// Test to get customers using code
//        /// </summary>
//        [Test]
//        public void GetCustomerTest()
//        {
//            _customerController.Request = new HttpRequestMessage();
//            _customerController.Configuration = new System.Web.Http.HttpConfiguration();
//            var expected = "TestCustomer1";
//            var code = "1";
//            var response = _customerController.GetCustomer(code);

//            // Assert
//            var json = response.Content.ReadAsStringAsync().Result;
//            var customer = JsonHelper.FromJson<Customer>(json);
//            Assert.AreEqual(expected, customer.Name);
//            Assert.IsTrue(response.IsSuccessStatusCode);
//        }


//        /// <summary>
//        /// Test to get customers using invalid code
//        /// </summary>
//        [Test]
//        public void GetCustomerForInvalidCodeTest()
//        {
//            _customerController.Request = new HttpRequestMessage();
//            _customerController.Configuration = new System.Web.Http.HttpConfiguration();
//            var expected = "Customer does not exists";
//            var code = "99";
//            var response = _customerController.GetCustomer(code);

//            // Assert
//            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
//            var errorContent = response.Content.ReadAsStringAsync().Result;
//            var serializer = new JavaScriptSerializer();
//            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

//            dynamic glossaryEntry = serializer.Deserialize(errorContent, typeof(object)) as dynamic;
//            var actualError = glossaryEntry.error;
//            Assert.AreEqual(actualError, expected);
//        }

//        /// <summary>
//        /// Test to search customers using search term
//        /// </summary>
//        [Test]
//        public void SearchCustomerTest()
//        {
//            _customerController.Request = new HttpRequestMessage();
//            _customerController.Configuration = new System.Web.Http.HttpConfiguration();
//            var expected = 1;
//            var searchTerm = "345";
//            var response = _customerController.SearchCustomer(searchTerm);
//            var json = response.Content.ReadAsStringAsync().Result;
//            var customers = JsonHelper.FromJson<List<Customer>>(json);
//            Assert.AreEqual(expected, customers.Count());
//            Assert.IsTrue(response.IsSuccessStatusCode);
//        }


//        /// <summary>
//        /// Test to search customers if not exists
//        /// </summary>
//        [Test]
//        public void SearchCustomerForNonExistingCustomerTest()
//        {
//            _customerController.Request = new HttpRequestMessage();
//            _customerController.Configuration = new System.Web.Http.HttpConfiguration();
//            var expected = 0;
//            var searchTerm = "999";
//            var response = _customerController.SearchCustomer(searchTerm);
//            var json = response.Content.ReadAsStringAsync().Result;
//            var customers = JsonHelper.FromJson<List<Customer>>(json);
//            Assert.AreEqual(expected, customers.Count());
//            Assert.IsTrue(response.IsSuccessStatusCode);
//        }


//        /// <summary>
//        /// Test to search customers if search term is in invalid client card
//        /// </summary>
//        [Test]
//        public void SearchCustomerForInvalidClientCardsTest()
//        {
//            _customerController.Request = new HttpRequestMessage();
//            _customerController.Configuration = new System.Web.Http.HttpConfiguration();
//            var expected = "Card is disabled";
//            var searchTerm = "80";
//            var response = _customerController.SearchCustomer(searchTerm);
//            var errorContent = response.Content.ReadAsStringAsync().Result;
//            Assert.AreEqual(errorContent.Trim('"'), expected);
//            Assert.IsTrue(response.IsSuccessStatusCode);
//        }

//        /// <summary>
//        /// Test to get loyalty customer by code test
//        /// </summary>
//        [Test]
//        public void GetLoyaltyCustomersTest()
//        {
//            var expected = 2;
//            // Act
//            var actual = _customerController.GetLoyaltyCustomers().Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to search loyalty customers by search term
//        /// </summary>
//        [Test]
//        public void LoyaltySearchTest()
//        {
//            _customerController.Request = new HttpRequestMessage();
//            _customerController.Configuration = new System.Web.Http.HttpConfiguration();
//            var expected = 1;
//            var searchTerm = "345";
//            var response = _customerController.LoyaltySearch(searchTerm);
//            var json = response.Content.ReadAsStringAsync().Result;
//            var customers = JsonHelper.FromJson<List<Customer>>(json);
//            Assert.AreEqual(expected, customers.Count());
//            Assert.IsTrue(response.IsSuccessStatusCode);
//        }


//        /// <summary>
//        /// Test to search loyalty customers if not exists
//        /// </summary>
//        [Test]
//        public void LoyaltySearchForNonExistingCustomerTest()
//        {
//            _customerController.Request = new HttpRequestMessage();
//            _customerController.Configuration = new System.Web.Http.HttpConfiguration();
//            var expected = 0;
//            var searchTerm = "999";
//            var response = _customerController.LoyaltySearch(searchTerm);
//            var json = response.Content.ReadAsStringAsync().Result;
//            var customers = JsonHelper.FromJson<List<Customer>>(json);
//            Assert.AreEqual(expected, customers.Count());
//            Assert.IsTrue(response.IsSuccessStatusCode);
//        }


//        /// <summary>
//        /// Test to search loyalty customers if search term is in invalid client card
//        /// </summary>
//        [Test]
//        public void LoyaltySearchForInvalidClientCardsTest()
//        {
//            _customerController.Request = new HttpRequestMessage();
//            _customerController.Configuration = new System.Web.Http.HttpConfiguration();
//            var expected = "Card is disabled";
//            var searchTerm = "80";
//            var response = _customerController.LoyaltySearch(searchTerm);
//            var errorContent = response.Content.ReadAsStringAsync().Result;
//            Assert.AreEqual(errorContent.Trim('"'), expected);
//            Assert.IsTrue(response.IsSuccessStatusCode);
//        }

//        /// <summary>
//        /// Test to add loyalty customers
//        /// </summary>
//        [Test]
//        public void AddLoyaltyCustomerTest()
//        {
//            _customerController.Request = new HttpRequestMessage();
//            _customerController.Configuration = new System.Web.Http.HttpConfiguration();
//            var newLoyaltyCustomer = new Customer
//            {
//                Code = "3",
//                Name = "New Customer",
//                Phone = "xyz",
//                LoyaltyNumber = "123",
//                LoyaltyPoints = 0
//            };
//            var response = _customerController.AddLoyaltyCustomer(newLoyaltyCustomer);
//            Assert.IsTrue(response.IsSuccessStatusCode);
//        }

//        /// <summary>
//        /// Test to add a null loyalty customer
//        /// </summary>
//        [Test]
//        public void AddNullLoyaltyCustomerTest()
//        {
//            _customerController.Request = new HttpRequestMessage();
//            _customerController.Configuration = new System.Web.Http.HttpConfiguration();
//            var expected = "No customer to add";
//            var response = _customerController.AddLoyaltyCustomer(null);
//            var errorContent = response.Content.ReadAsStringAsync().Result;
//            var serializer = new JavaScriptSerializer();
//            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

//            dynamic glossaryEntry = serializer.Deserialize(errorContent, typeof(object)) as dynamic;
//            var actualError = glossaryEntry.error;
//            Assert.AreEqual(actualError, expected);
//            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
//        }

//        /// <summary>
//        /// Test to set a customer as loyalty customer
//        /// </summary>
//        [Test]
//        public void SetLoyaltyCustomerTest()
//        {
//            _customerController.Request = new HttpRequestMessage();
//            _customerController.Configuration = new System.Web.Http.HttpConfiguration();
//            var code = "1";
//            var loyaltyNumber = "123";
//            var response = _customerController.SetLoyaltyCustomer(code, loyaltyNumber);
//            Assert.IsTrue(response.IsSuccessStatusCode);
//        }


//        /// <summary>
//        /// Test to set the loyalty number for non existing customer
//        /// </summary>
//        [Test]
//        public void SetLoyaltyCustomerForNonExistingCustomerTest()
//        {
//            _customerController.Request = new HttpRequestMessage();
//            _customerController.Configuration = new System.Web.Http.HttpConfiguration();
//            var expected = "Customer does not exists";
//            var code = "0";
//            var loyaltyNumber = "123";
//            var response = _customerController.SetLoyaltyCustomer(code, loyaltyNumber);
//            var errorContent = response.Content.ReadAsStringAsync().Result;
//            var serializer = new JavaScriptSerializer();
//            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

//            dynamic glossaryEntry = serializer.Deserialize(errorContent, typeof(object)) as dynamic;
//            var actualError = glossaryEntry.error;
//            Assert.AreEqual(actualError, expected);
//            Assert.IsTrue(response.IsSuccessStatusCode);
//        }
//    }
//}

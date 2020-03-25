//using Infonet.CStoreCommander.Data;
//using Infonet.CStoreCommander.Data.CSCAdmin;
//using Infonet.CStoreCommander.Data.Repositories;
//using Infonet.CStoreCommander.Data.ServiceClasses;
//using Infonet.CStoreCommander.WebApi.Controllers.V1;
//using Infonet.CStoreCommander.WebApi.Manager;
//using Infonet.CStoreCommander.WebApi.Models;
//using Infonet.CStoreCommander.WebApi.Tests.Helpers;
//using Moq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Script.Serialization;

//namespace Infonet.CStoreCommander.WebApi.Tests.Controllers
//{
//    [TestFixture]
//   public class PolicyV1ControllerTest
//    {
//        private Mock<IRepository<P_COMP>> _companyPolicyRepository = new Mock<IRepository<P_COMP>>();
//        private Mock<IRepository<P_SET>> _setPolicyRepository = new Mock<IRepository<P_SET>>();
//        private Mock<IRepository<P_CANBE>> _levelPolicyRepository = new Mock<IRepository<P_CANBE>>();
//        private Mock<IRepository<POS_IP_Address>> _posIpAddressRepository =
//                                              new Mock<IRepository<POS_IP_Address>>();
//        private IPolicyService _policyService;
//        private IUtilityService _utilityService;
//        private IStorePolicyManager _storePolicyManager;
//        private ICustomerPolicyManager _customerPolicyManager;
//        private PolicyV1Controller _policyController;

//        /// <summary>
//        /// Setup
//        /// </summary>
//        [SetUp]
//        public void SetUp()
//        {

//            // Set up some testing data
//            var firstCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "LOYALTY",
//                P_SEQ = 780,
//                Implemented = true,
//                P_NAME = "CBONUSNAME",
//                P_DESC = "Cash Bonus Name",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{?}",
//                P_SET = "test",
//                P_VARTYPE = "C",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var secondCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "LOYALTY",
//                P_SEQ = 790,
//                Implemented = true,
//                P_NAME = "CBONUSFLOAT",
//                P_DESC = "Do you provide till Cash Bonus float??",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "Yes",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var thirdCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "TILL_SAL",
//                P_SEQ = 30,
//                Implemented = true,
//                P_NAME = "TILL_FLOAT",
//                P_DESC = "Do you provide a till float?",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "No",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var fourthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "LOYALTY",
//                P_SEQ = 770,
//                Implemented = true,
//                P_NAME = "CASHBONUS",
//                P_DESC = "Support Cash Bonus Program?",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "Yes",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var fifthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "TILL_SAL",
//                P_SEQ = 1921,
//                Implemented = true,
//                P_NAME = "TILL_NUM",
//                P_DESC = "Use predefined till number",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "No",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var sixthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "TILL_EOD",
//                P_SEQ = 80,
//                Implemented = true,
//                P_NAME = "USE_SHIFTS",
//                P_DESC = "Use Store Shifts",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "Yes",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var seventhCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "PRAC",
//                P_SEQ = 2051,
//                Implemented = true,
//                P_NAME = "WINDOWS_LOGIN",
//                P_DESC = "Login using Windows User",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "No",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var eigthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "CUST",
//                P_SEQ = 1420,
//                Implemented = true,
//                P_NAME = "DEFCUST_CODE",
//                P_DESC = "Default customer is",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "STAB(Client, CL_Code)",
//                P_SET = "Cash Sale",
//                P_VARTYPE = "C",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var ninthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "CUST",
//                P_SEQ = 1410,
//                Implemented = true,
//                P_NAME = "DEFAULTCUST",
//                P_DESC = "Use default customer",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "Yes",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var tenthCompanyPolicy = new P_COMP
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

//            var eleventhCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "CUST",
//                P_SEQ = 1951,
//                Implemented = true,
//                P_NAME = "CUST_SCAN",
//                P_DESC = "Scan customer cards in POS to identify the customer?",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "No",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var twelfthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "CUST",
//                P_SEQ = 1961,
//                Implemented = true,
//                P_NAME = "CUST_SWP",
//                P_DESC = "Swipe customer cards in POS to identify the customer?",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "No",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var thirteenthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "CUST",
//                P_SEQ = 1981,
//                Implemented = true,
//                P_NAME = "MEMBER_CODE",
//                P_DESC = "Allow Member Code Entry at the Pump?",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "No",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var fourteenthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "CUST",
//                P_SEQ = 20,
//                Implemented = true,
//                P_NAME = "CREDTERM",
//                P_DESC = "Do you offer credit terms?",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "Yes",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var fifteenthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "CUST",
//                P_SEQ = 30,
//                Implemented = true,
//                P_NAME = "USE_ARCUST",
//                P_DESC = "Use AR Customers",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "Yes",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var sixteenthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "CUST",
//                P_SEQ = 10,
//                Implemented = true,
//                P_NAME = "USE_CUST",
//                P_DESC = "Use Customers",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "Yes",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var seventeenthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "CUST",
//                P_SEQ = 1390,
//                Implemented = true,
//                P_NAME = "CreditMsg",
//                P_DESC = "Show customer note if the customer is over limit(Y/N)?",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "{Yes,No}",
//                P_SET = "Yes",
//                P_VARTYPE = "L",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var eighteenthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "CUST",
//                P_SEQ = 1601,
//                Implemented = true,
//                P_NAME = "ARTENDER",
//                P_DESC = "Tender Name for Customer AR Account:",
//                P_LEVELS = "{COMPANY}",
//                P_CHOICES = "STAB(TendMast, TendDesc)",
//                P_SET = "ARACC",
//                P_VARTYPE = "C",
//                P_ACTIVE = true,
//                P_USED = true
//            };

//            var firstIpAddress = new POS_IP_Address
//            {
//                ID = 1,
//                IP_Address = "172.16.16.1",
//                WritePosLog = true
//            };

//            var secondIpAddress = new POS_IP_Address
//            {
//                ID = 2,
//                IP_Address = "172.16.16.2",
//                WritePosLog = true
//            };

//            //create company policy
//            var companyPolicies = new List<P_COMP>();
//            companyPolicies.Add(firstCompanyPolicy);
//            companyPolicies.Add(secondCompanyPolicy);
//            companyPolicies.Add(thirdCompanyPolicy);
//            companyPolicies.Add(fourthCompanyPolicy);
//            companyPolicies.Add(fifthCompanyPolicy);
//            companyPolicies.Add(sixthCompanyPolicy);
//            companyPolicies.Add(seventhCompanyPolicy);
//            companyPolicies.Add(eigthCompanyPolicy);
//            companyPolicies.Add(ninthCompanyPolicy);
//            companyPolicies.Add(tenthCompanyPolicy);
//            companyPolicies.Add(eleventhCompanyPolicy);
//            companyPolicies.Add(twelfthCompanyPolicy);
//            companyPolicies.Add(thirteenthCompanyPolicy);
//            companyPolicies.Add(fourteenthCompanyPolicy);
//            companyPolicies.Add(fifteenthCompanyPolicy);
//            companyPolicies.Add(sixteenthCompanyPolicy);
//            companyPolicies.Add(seventeenthCompanyPolicy);
//            companyPolicies.Add(eighteenthCompanyPolicy);

//            //create set policy
//            var setPolicies = new List<P_SET>();

//            //create level policy
//            var levelPolicies = new List<P_CANBE>();


//            //create pos ip addresses
//            var posIpAddresses = new List<POS_IP_Address>();
//            posIpAddresses.Add(firstIpAddress);
//            posIpAddresses.Add(secondIpAddress);

//            //get data
//            _posIpAddressRepository.Setup(c => c.GetAll()).Returns(posIpAddresses.AsQueryable());
//            _utilityService = new UtilityService(_posIpAddressRepository.Object);

//            _companyPolicyRepository.Setup(u => u.GetAll()).Returns(companyPolicies.AsQueryable());
//            _setPolicyRepository.Setup(u => u.GetAll()).Returns(setPolicies.AsQueryable());
//            _levelPolicyRepository.Setup(l => l.GetAll()).Returns(levelPolicies.AsQueryable());
//            _policyService = new PolicyService(_companyPolicyRepository.Object, _setPolicyRepository.Object,
//                                                _levelPolicyRepository.Object);
//            _storePolicyManager = new StorePolicyManager(_policyService, _utilityService);
//            _customerPolicyManager = new CustomerPolicyManager(_policyService);
//            _policyController = new PolicyV1Controller(_storePolicyManager, _customerPolicyManager);
//            _policyController.Request = new HttpRequestMessage();
//            _policyController.Configuration = new System.Web.Http.HttpConfiguration();
//        }


//        /// <summary>
//        /// Test to get store policies
//        /// </summary>
//        [Test]
//        public void GetStorePolicyTest()
//        {
//            var expected = 1;
//            var ipAddress = "172.16.16.1";
//            var response = _policyController.GetStorePolicy(ipAddress);
//            var json = response.Content.ReadAsStringAsync().Result;
//            var storePolicy = JsonHelper.FromJson<StorePolicy>(json);
//            Assert.AreEqual(expected, storePolicy.POSId);
//            Assert.IsTrue(response.IsSuccessStatusCode);
//        }

//        /// <summary>
//        /// Test to get store policies
//        /// </summary>
//        [Test]
//        public void GetStorePolicyForInvalidIpTest()
//        {
//            var expected = "IP Address is Not Registered.";
//            var ipAddress = "0.0.0.0";
//            var response = _policyController.GetStorePolicy(ipAddress);
//            var errorContent = response.Content.ReadAsStringAsync().Result;
//            var serializer = new JavaScriptSerializer();
//            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

//            dynamic glossaryEntry = serializer.Deserialize(errorContent, typeof(object)) as dynamic;
//            var actualError = glossaryEntry.error;
//            Assert.AreEqual(actualError, expected);
//            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
//        }

//        /// <summary>
//        /// Test to get customer policies
//        /// </summary>
//        [Test]
//        public void GetCustomerPolicyTest()
//        {
//            var expected = "Cash Sale";
//            var response = _policyController.GetCustomerPolicy();
//            var json = response.Content.ReadAsStringAsync().Result;
//            var customerPolicy = JsonHelper.FromJson<CustomerPolicy>(json);
//            Assert.AreEqual(expected, customerPolicy.DefaultCustomerCode);
//            Assert.IsTrue(response.IsSuccessStatusCode);
//        }
//    }
//}

//using Infonet.CStoreCommander.Data;
//using Infonet.CStoreCommander.Data.CSCAdmin;
//using Infonet.CStoreCommander.Data.CSCMaster;
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
//using System.Net.Http;
//using System.Web.Script.Serialization;

//namespace Infonet.CStoreCommander.WebApi.Tests.Controllers
//{
//    [TestFixture]
//    public class TillV1ControllerTest
//    {
//        private Mock<IRepository<ShiftStore>> _shiftStoreRepository =
//                                                  new Mock<IRepository<ShiftStore>>();
//        private Mock<IRepository<TILL>> _tillRepository =
//                                                    new Mock<IRepository<TILL>>();

//        private ITillService _tillService;
//        private Mock<IRepository<P_COMP>> _companyPolicyRepository = new Mock<IRepository<P_COMP>>();
//        private Mock<IRepository<P_SET>> _setPolicyRepository = new Mock<IRepository<P_SET>>();
//        private Mock<IRepository<P_CANBE>> _levelPolicyRepository = new Mock<IRepository<P_CANBE>>();
//        private IPolicyService _policyService;
//        private Mock<IRepository<POS_IP_Address>> _posIpAddressRepository =
//                                                new Mock<IRepository<POS_IP_Address>>();
//        private IUtilityService _utilityService;
//        private ITillManager _tillManager;
//        private TillV1Controller _tillController;

//        /// <summary>
//        /// Setup
//        /// </summary>
//        [SetUp]
//        public void SetUp()
//        {
//            // Set up some testing data
//            var firstShiftStore = new ShiftStore
//            {
//                Active = true,
//                CurrentDay = false,
//                ShiftNumber = 1,
//                StartTime = DateTime.Today
//            };

//            var secondShiftStore = new ShiftStore
//            {
//                Active = false,
//                CurrentDay = true,
//                ShiftNumber = 2,
//                StartTime = DateTime.Today.AddDays(2)
//            };


//            var firstTill = new TILL
//            {
//                ACTIVE = true,
//                CASH = 0,
//                CashBonus = null,
//                CashBonusFloat = null,
//                DATE_OPEN = DateTime.Today,
//                FLOAT = null,
//                POSID = 1,
//                PROCESS = false,
//                ShiftDate = DateTime.Today,
//                ShiftNumber = 1,
//                TILL_NUM = 1,
//                TIME_OPEN = DateTime.Today,
//                UserLoggedOn = "Test User 1"
//            };

//            var secondTill = new TILL
//            {
//                ACTIVE = true,
//                CASH = 0,
//                CashBonus = null,
//                CashBonusFloat = null,
//                DATE_OPEN = DateTime.Today.AddDays(2),
//                FLOAT = null,
//                POSID = 2,
//                PROCESS = false,
//                ShiftDate = DateTime.Today.AddDays(2),
//                ShiftNumber = 2,
//                TILL_NUM = 2,
//                TIME_OPEN = DateTime.Today.AddDays(2),
//                UserLoggedOn = "Test User 2"
//            };

//            var firstCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "Loyalty",
//                P_SEQ = 1,
//                P_DESC = "Use loyalty customer",
//                P_LEVELS = "{COMPANY}",
//                P_SET = "Yes",
//                P_NAME = "Use Loyalty",
//                P_USED = true,
//                P_VARTYPE = "L",
//                Implemented = true,
//                P_ACTIVE = true,
//                P_CHOICES = "{Yes,No}"
//            };

//            var secondCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "Prac",
//                P_SEQ = 2,
//                P_DESC = "Time is printed in format",
//                P_LEVELS = "{USER,COMPANY}",
//                P_SET = "12 HOURS",
//                P_NAME = "TIMEFORMAT",
//                P_USED = true,
//                P_VARTYPE = "C",
//                Implemented = true,
//                P_ACTIVE = true,
//                P_CHOICES = "{12 HOURS,24 HOURS}"
//            };


//            var firstSetPolicy = new P_SET
//            {
//                P_LEVEL = "USER",
//                P_NAME = "VOID_AUTH",
//                P_SET1 = "Yes",
//                P_VALUE = "MANAGER"
//            };

//            var secondSetPolicy = new P_SET
//            {
//                P_LEVEL = "DEPT",
//                P_NAME = "ACCEPT_RETURN",
//                P_SET1 = "No",
//                P_VALUE = "FUEL"
//            };

//            var thirdSetPolicy = new P_SET
//            {
//                P_LEVEL = "SUB_DEPT",
//                P_NAME = "ACCEPT_RETURN",
//                P_SET1 = "Yes",
//                P_VALUE = "FUEL"
//            };


//            var firstlevelPolicy = new P_CANBE
//            {
//                P_Canbe1 = "DEPT",
//                P_NAME = "ADD_STOCK",
//                P_Seq = 3
//            };

//            var secondlevelPolicy = new P_CANBE
//            {
//                P_Canbe1 = "COMAPNY",
//                P_NAME = "ADD_CUSTOMER",
//                P_Seq = 4
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

//            //create shifts
//            var shifts = new List<ShiftStore>();
//            shifts.Add(firstShiftStore);
//            shifts.Add(secondShiftStore);

//            //create tills
//            var tills = new List<TILL>();
//            tills.Add(firstTill);
//            tills.Add(secondTill);

//            //create company policy
//            var companyPolicies = new List<P_COMP>();
//            companyPolicies.Add(firstCompanyPolicy);
//            companyPolicies.Add(secondCompanyPolicy);

//            //create set policy
//            var setPolicies = new List<P_SET>();
//            setPolicies.Add(firstSetPolicy);
//            setPolicies.Add(secondSetPolicy);
//            setPolicies.Add(thirdSetPolicy);

//            //create level policy
//            var levelPolicies = new List<P_CANBE>();
//            levelPolicies.Add(firstlevelPolicy);
//            levelPolicies.Add(secondlevelPolicy);

//            //create pos ip addresses
//            var posIpAddresses = new List<POS_IP_Address>();
//            posIpAddresses.Add(firstIpAddress);
//            posIpAddresses.Add(secondIpAddress);

//            _shiftStoreRepository.Setup(u => u.GetAll()).Returns(shifts.AsQueryable());
//            _tillRepository.Setup(u => u.GetAll()).Returns(tills.AsQueryable());
//            _tillService = new TillService(_shiftStoreRepository.Object,
//                                                _tillRepository.Object);
//            _companyPolicyRepository.Setup(u => u.GetAll()).Returns(companyPolicies.AsQueryable());
//            _setPolicyRepository.Setup(u => u.GetAll()).Returns(setPolicies.AsQueryable());
//            _levelPolicyRepository.Setup(l => l.GetAll()).Returns(levelPolicies.AsQueryable());
//            _policyService = new PolicyService(_companyPolicyRepository.Object, _setPolicyRepository.Object,
//                                                _levelPolicyRepository.Object);
//            _posIpAddressRepository.Setup(c => c.GetAll()).Returns(posIpAddresses.AsQueryable());
//            _utilityService = new UtilityService(_posIpAddressRepository.Object);

//            _tillManager = new TillManager(_tillService, _policyService, _utilityService);
//            _tillController = new TillV1Controller(_tillManager);
//            _tillController.Request = new HttpRequestMessage();
//            _tillController.Configuration = new System.Web.Http.HttpConfiguration();

//        }

//        /// <summary>
//        /// Test to get the active tills
//        /// </summary>
//        [Test]
//        public void ActiveTillsTest()
//        {
//            //var expected = 2;
//            var posId = 1;
//            var response = _tillController.ActiveTills(posId);
//            Assert.IsTrue(response.IsSuccessStatusCode);
//        }

//        /// <summary>
//        /// Test to get the active shifts
//        /// </summary>
//        [Test]
//        public void ActiveShiftsTest()
//        {
//            var expected = 1;
//            var tillNumber = 1;
//            var response = _tillController.ActiveShifts(tillNumber);
//            // Assert
//            var json = response.Content.ReadAsStringAsync().Result;
//            var serializer = new JavaScriptSerializer();
//            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

//            dynamic glossaryEntry = serializer.Deserialize(json, typeof(object)) as dynamic;
//            var shifts = glossaryEntry.shifts;
//            Assert.AreEqual(expected, shifts.Count);
//            Assert.IsTrue(response.IsSuccessStatusCode);
//        }
//    }
//}

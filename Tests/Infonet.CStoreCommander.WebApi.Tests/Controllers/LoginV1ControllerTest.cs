//using Infonet.CStoreCommander.Data.CSCAdmin;
//using Infonet.CStoreCommander.Data.CSCMaster;
//using Infonet.CStoreCommander.Data.Repositories;
//using Infonet.CStoreCommander.Data.ServiceClasses;
//using Infonet.CStoreCommander.WebApi.Controllers.V1;
//using Infonet.CStoreCommander.WebApi.Exceptions;
//using Infonet.CStoreCommander.WebApi.Manager;
//using Infonet.CStoreCommander.WebApi.Models;
//using Infonet.CStoreCommander.WebApi.Tests.Helpers;
//using Infonet.CStoreCommander.WebApi.Utilities;
//using Moq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Script.Serialization;

//namespace Infonet.CStoreCommander.WebApi.Tests.Controllers
//{
//    [TestFixture]
//    public class LoginV1ControllerTest
//    {
//        private Mock<IRepository<USER>> _userRepository = new Mock<IRepository<USER>>();
//        private Mock<IRepository<UIG>> _userGroupRepository = new Mock<IRepository<UIG>>();
//        private IUserService _userService;
//        private IUserManager _userManager;
//        private Mock<IRepository<ShiftStore>> _shiftStoreRepository =
//                                                   new Mock<IRepository<ShiftStore>>();
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
//        private IEncryption _encryption;
//        private ITillManager _tillManager;
//        private LoginV1Controller _loginController;


//        /// <summary>
//        /// Setup
//        /// </summary>
//        [SetUp]
//        public void Setup()
//        {
//            _encryption = new Encryption();
//            var pswd = _encryption.EncryptText("54321");
//            // Set up some testing data
//            var firstUser = new USER
//            {
//                DOB = DateTime.Today,
//                EPW = "12345",
//                U_CODE = "1",
//                U_ID = 1,
//                U_NAME = "Test User 1"
//            };

//            var secondUser = new USER
//            {
//                DOB = DateTime.Today,
//                EPW = pswd,
//                U_CODE = "Test User 2",
//                U_ID = 1,
//                U_NAME = "Test User 2"
//            };


//            var thirdUser = new USER
//            {
//                DOB = DateTime.Today,
//                EPW = pswd,
//                U_CODE = "Test User 3",
//                U_ID = 1,
//                U_NAME = "Test User 3"
//            };

//            var firstUserGroup = new UIG
//            {
//                USER = "Test User 1",
//                UGROUP = "Manager"
//            };

//            var secondUserGroup = new UIG
//            {
//                USER = "Test User 2",
//                UGROUP = "MANAGER"
//            };

//            var thirdUserGroup = new UIG
//            {
//                USER = "Test User 3",
//                UGROUP = "TRAINER"
//            };

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
//                PROCESS = true,
//                ShiftDate = DateTime.Today,
//                ShiftNumber = 1,
//                TILL_NUM = 1,
//                TIME_OPEN = DateTime.Today,
//                UserLoggedOn = "Test User 1"
//            };

//            var secondTill = new TILL
//            {
//                ACTIVE = false,
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

//            var thirdTill = new TILL
//            {
//                ACTIVE = false,
//                CASH = 0,
//                CashBonus = null,
//                CashBonusFloat = null,
//                DATE_OPEN = DateTime.Today.AddDays(2),
//                FLOAT = null,
//                POSID = 2,
//                PROCESS = false,
//                ShiftDate = DateTime.Today.AddDays(2),
//                ShiftNumber = 2,
//                TILL_NUM = 92,
//                TIME_OPEN = DateTime.Today.AddDays(2),
//                UserLoggedOn = "Test User 3"
//            };

//            var firstCompanyPolicy = new P_COMP
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

//            var thirdCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "TILL_EOD",
//                P_SEQ = 2,
//                P_DESC = "Automatically pick the next shift (it's not based on time)?",
//                P_LEVELS = "{USER,COMPANY}",
//                P_SET = "Yes",
//                P_NAME = "AutoShftPick",
//                P_USED = true,
//                P_VARTYPE = "C",
//                Implemented = true,
//                P_ACTIVE = true,
//            };

//            var fourthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "TILL_EOD",
//                P_SEQ = 2,
//                P_DESC = "Is it 24 Hour Store?",
//                P_LEVELS = "{USER,COMPANY}",
//                P_SET = "No",
//                P_NAME = "Hour24Store",
//                P_USED = true,
//                P_VARTYPE = "C",
//                Implemented = true,
//                P_ACTIVE = true,
//            };

//            var fifthCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "TILL_EOD",
//                P_SEQ = 2,
//                P_DESC = "Shift 1 Starts in the --------- Day",
//                P_LEVELS = "{USER,COMPANY}",
//                P_SET = "Next",
//                P_NAME = "SHIFT_DAY",
//                P_USED = true,
//                P_VARTYPE = "C",
//                Implemented = true,
//                P_ACTIVE = true,
//            };


//            var sixthCompanyPolicy = new P_COMP
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


//            var seventhCompanyPolicy = new P_COMP
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

//            var thirdPolicy = new P_COMP
//            {
//                P_CLASS = "USER_SYS",
//                P_DESC = "User can log on more than one till",
//                P_LEVELS = "{COMPANY}",
//                P_SET = "No",
//                P_NAME = "LOG_UNLIMIT",
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


//            //create users
//            var users = new List<USER>();
//            users.Add(firstUser);
//            users.Add(secondUser);
//            users.Add(thirdUser);

//            //create user groups
//            var userGroups = new List<UIG>();
//            userGroups.Add(firstUserGroup);
//            userGroups.Add(secondUserGroup);
//            userGroups.Add(thirdUserGroup);

//            //create shifts
//            var shifts = new List<ShiftStore>();
//            shifts.Add(firstShiftStore);
//            shifts.Add(secondShiftStore);

//            //create tills
//            var tills = new List<TILL>();
//            tills.Add(firstTill);
//            tills.Add(secondTill);
//            tills.Add(thirdTill);

//            //create company policy
//            var companyPolicies = new List<P_COMP>();
//            companyPolicies.Add(firstCompanyPolicy);
//            companyPolicies.Add(secondCompanyPolicy);
//            companyPolicies.Add(thirdPolicy);
//            companyPolicies.Add(thirdCompanyPolicy);
//            companyPolicies.Add(fourthCompanyPolicy);
//            companyPolicies.Add(fifthCompanyPolicy);
//            companyPolicies.Add(sixthCompanyPolicy);
//            companyPolicies.Add(seventhCompanyPolicy);

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

//            _userRepository.Setup(u => u.GetAll()).Returns(users.AsQueryable());
//            _userGroupRepository.Setup(u => u.GetAll()).Returns(userGroups.AsQueryable());
//            _userService = new UserService(_userRepository.Object, _userGroupRepository.Object);
//            _userManager = new UserManager(_userService, _encryption);
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
//            _loginController = new LoginV1Controller(_userManager, _tillManager);
//            _loginController.Request = new HttpRequestMessage();
//            _loginController.Configuration = new System.Web.Http.HttpConfiguration();
//        }

//        /// <summary>
//        /// Test for login in case of invalid password
//        /// </summary>
//        [Test]
//        public void LoginTestForInvalidPassword()
//        {
//            var expected = "Incorrect username or password.";
//            var user = new User
//            {
//                UserName = "Test User 1",
//                Password = "12345"
//            };
//            var response = _loginController.Login(user);

//            // Assert
//            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
//            var errorContent = response.Content.ReadAsStringAsync().Result;
//            var serializer = new JavaScriptSerializer();
//            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

//            dynamic glossaryEntry = serializer.Deserialize(errorContent, typeof(object)) as dynamic;
//            var actualError = glossaryEntry.Error;
//            Assert.AreEqual(actualError, expected);
//        }

//        /// <summary>
//        /// Test for login in case of invalid tillnumber
//        /// </summary>
//        [Test]
//        public void LoginTestForInvalidTillNumber()
//        {
//            var user = new User
//            {
//                UserName = "Test User 2",
//                Password = "54321"
//            };
//            Assert.Throws<ApiBusinessException>(() => _loginController.Login(user));
//        }


//        /// <summary>
//        /// Test for login in case of non existing shift number
//        /// </summary>
//        [Test]
//        public void LoginTestForNonExistingShiftNumber()
//        {
//            var expected = "Please provide shift number.";
//            var user = new User
//            {
//                UserName = "Test User 2",
//                Password = "54321",
//                UserGroupCode = "MANAGER",
//                TillNumber = 2,
//            };
//            var response = _loginController.Login(user);
//            var errorContent = response.Content.ReadAsStringAsync().Result;
//            var serializer = new JavaScriptSerializer();
//            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

//            dynamic glossaryEntry = serializer.Deserialize(errorContent, typeof(object)) as dynamic;
//            var actualError = glossaryEntry.Error;
//            Assert.AreEqual(actualError, expected);
//            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
//        }

//        /// <summary>
//        /// Test for login in case of invalid shift number
//        /// </summary>
//        [Test]
//        public void LoginTestForInvalidShiftNumber()
//        {
//            var expected = "Please provide shift from the available shifts.";
//            var user = new User
//            {
//                UserName = "Test User 2",
//                Password = "54321",
//                UserGroupCode = "MANAGER",
//                TillNumber = 2,
//                ShiftNumber = 4
//            };
//            var response = _loginController.Login(user);
//            var errorContent = response.Content.ReadAsStringAsync().Result;
//            var serializer = new JavaScriptSerializer();
//            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

//            dynamic glossaryEntry = serializer.Deserialize(errorContent, typeof(object)) as dynamic;
//            var actualError = glossaryEntry.Error;
//            Assert.AreEqual(actualError, expected);
//            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
//        }


//        /// <summary>
//        /// Test for login in case of invalid pos id
//        /// </summary>
//        [Test]
//        public void LoginTestForInvalidPosId()
//        {
//            var expected = "Please provide POS Id.";
//            var user = new User
//            {
//                UserName = "Test User 2",
//                Password = "54321",
//                UserGroupCode = "MANAGER",
//                TillNumber = 2,
//                ShiftNumber = 1
//            };
//            var response = _loginController.Login(user);
//            var errorContent = response.Content.ReadAsStringAsync().Result;
//            var serializer = new JavaScriptSerializer();
//            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

//            dynamic glossaryEntry = serializer.Deserialize(errorContent, typeof(object)) as dynamic;
//            var actualError = glossaryEntry.Error;
//            Assert.AreEqual(actualError, expected);
//            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
//        }


//        /// <summary>
//        /// Test for login in case of invalid pos id
//        /// </summary>
//        [Test]
//        public void LoginTestForTrainer()
//        {
//            var expected = "This User/Test User 3 is a Trainer User You are going to run POS in Trainer Mode.";
//            var user = new User
//            {
//                UserName = "Test User 3",
//                Password = "54321",
//                UserGroupCode = "TRAINER",
//                TillNumber = 92,
//                ShiftNumber = 1,
//                PosId = 2
//            };
//            var response = _loginController.Login(user);
//            var errorContent = response.Content.ReadAsStringAsync().Result;
//            var serializer = new JavaScriptSerializer();
//            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

//            dynamic glossaryEntry = serializer.Deserialize(errorContent, typeof(object)) as dynamic;
//            var message = glossaryEntry.message;
//            Assert.AreEqual(message, expected);
//            Assert.IsTrue(response.IsSuccessStatusCode);
//        }
//    }

//}

using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System;
using Infonet.CStoreCommander.Resources;

namespace Infonet.CStoreCommander.WebApi.Tests.Managers
{
    /// <summary>
    /// Test method for Login Manager 
    /// </summary>
    [TestFixture]
    public class LoginManagerTest
    {
        private readonly Mock<IUserService> _userService = new Mock<IUserService>();
        private readonly Mock<IUtilityService> _utilityService = new Mock<IUtilityService>();
        private readonly IApiResourceManager _resourceManager = new ApiResourceManager();
        private readonly Mock<ILoginService> _loginService = new Mock<ILoginService>();
        private readonly Mock<ITillService> _tillService = new Mock<ITillService>();
        private readonly Mock<IShiftService> _shiftService = new Mock<IShiftService>();
        private readonly Mock<IPolicyManager> _policyManager = new Mock<IPolicyManager>();
        private ILoginManager _loginManager;
        List<User> _users;
        Dictionary<int, string> _ipAddresses;

        /// <summary>
        /// Set the user Test Data 
        /// </summary>
        /// <returns></returns>
        private List<User> GetUsers()
        {
            EncryptionManager encryption = new EncryptionManager();
            var text = encryption.EncryptText("abc");
            var firstUser = new User
            {
                Name = "X",
                Code = "X",
                epw = text,
                Password = "abc",
                User_Group = new User_Group
                {
                    Code = "Manager",
                    Name = "Manager",
                    SecurityLevel = 1
                }
            };

            var secondUser = new User
            {
                Name = "A",
                Code = "A",
                epw = text,
                Password = "B",
                User_Group = new User_Group
                {
                    Code = "Developer",
                    Name = "Manager",
                    SecurityLevel = 1
                }
            };
            var users = new List<User>
            {
                firstUser,
                secondUser
            };
            return users;
        }

        /// <summary>
        /// Set the Get IpAddress Test Data
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, string> GetIpAddresses()
        {
            _ipAddresses = new Dictionary<int, string>();
            _ipAddresses.Add(1, "172.0.0.0");
            _ipAddresses.Add(2, "173.0.0.0");
            return _ipAddresses;
        }

        /// <summary>
        /// Set the store Test Data
        /// </summary>
        /// <returns></returns>

        private Store LoadStoreInfo()
        {
            return new Store
            {
                Name = "Wilsons Gas Stop - Sackville",
                RegName = "HST",
                RegNum = "R105702096",
                SecRegName = "",
                SecRegNum = "",
                Language = "English",
                Code = "118",
                Address = new Address
                {
                    Street1 = "655 Sackville Drive",
                    Street2 = "",
                    ProvState = "NS",
                    City = "Lower Sackville",
                    Country = "Canada",
                    EMail = "",
                    PostalCode = "B4C 2S4",
                },
                Sale_Footer = "",
                Refund_Footer = ""
            };
        }

        /// <summary>
        /// Set the Security Test Data for load security information
        /// </summary>
        /// <returns></returns>
        private Security LoadSecurityInfo()
        {
            return new Security
            {
                Install_Date_Encrypt = "281309070103000501005561545657545755565702000210160600194300",
                Security_Key = "0110331",
                POS_BO_Features = "11101110000000000000000000000000",
                Pump_Features = "1010000000000000",
                NIC_Number = "160 179 204 253 55 16",
                Number_OF_POS = 2,
                MaxConcurrentPOS = 2
            };
        }

        /// <summary>
        /// Set the security test data for load empty security information 
        /// </summary>
        /// <returns></returns>
        private Security LoadEmptySecurityInfo()
        {
            return new Security
            {
                Install_Date_Encrypt = "281309070103000501005561545657545755565702000210160600194300",
                Security_Key = "",
                POS_BO_Features = "11101110000000000000000000000000",
                Pump_Features = "1010000000000000",
                NIC_Number = "160 179 204 253 55 16",
                Number_OF_POS = 2,
                MaxConcurrentPOS = 2
            };
        }

        /// <summary>
        /// Set the security test data for load empty security code advanced install data information  
        /// </summary>
        /// <returns></returns>
        private Security LoadEmptySecurityCodeAdvancedInstallDateInfo()
        {
            return new Security
            {
                Install_Date_Encrypt = "281309070103000501005561545657545755565702000210160600194300",
                Security_Key = "",
                POS_BO_Features = "11101110000000000000000000000000",
                Pump_Features = "1010000000000000",
                NIC_Number = "160 179 204 253 55 16",
                Number_OF_POS = 2,
                MaxConcurrentPOS = 2
            };
        }

        /// <summary>
        /// set the security test data for invalid security information
        /// </summary>
        /// <returns></returns>
        private Security LoadInvalidSecurityInfo()
        {
            return new Security
            {
                Install_Date_Encrypt = "281309070",
                Security_Key = "011",
                POS_BO_Features = "11101110000000000000000000000000",
                Pump_Features = "1010000000000000",
                NIC_Number = "160 179 204 253 55 16",
                Number_OF_POS = 2,
                MaxConcurrentPOS = 2,
                Install_Date = DateTime.Now.AddDays(1)
            };
        }

        /// <summary>
        /// Set the security test data for load invalid date information 
        /// </summary>
        /// <returns></returns>
        private Security LoadInvalidDateInfo()
        {
            return new Security
            {
                Install_Date_Encrypt = "281309070",
                Security_Key = "",
                POS_BO_Features = "11101110000000000000000000000000",
                Pump_Features = "1010000000000000",
                NIC_Number = "160 179 204 253 55 16",
                Number_OF_POS = 2,
                MaxConcurrentPOS = 2,
                Install_Date = DateTime.Now.AddDays(1)
            };
        }

        private int GetDistinctIpAddress(int posId)
        {
            return _ipAddresses.Count(i => i.Key != posId && i.Key != 0);
        }

        /// <summary>
        /// Set the pos address test data for get pos ip address
        /// </summary>
        /// <param name="posId"></param>
        /// <returns></returns>
        private string GetPosIpAddress(byte posId)
        {
            if (_ipAddresses.ContainsKey(posId))
            {
                return _ipAddresses.FirstOrDefault(x => x.Key == posId).Value;
            }
            return string.Empty;
        }

        /// <summary>
        /// set the get admin value for get admin value 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetAdminValue(string value)
        {
            if (value == "RetailerAccountNo")
                return "1";
            if (value == "ExemptCode")
                return DateTime.Today.ToString("mm/dd/yyyy");
            return string.Empty;
        }

        /// <summary>
        /// Get the pos id test data
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private int GetPosIdData(string ipAddress)
        {
            if (_ipAddresses.ContainsValue(ipAddress))
            {
                return _ipAddresses.FirstOrDefault(x => x.Value == ipAddress).Key;
            }
            return 0;
        }

        /// <summary>
        /// set the get user test data 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private User GetUserData(string code)
        {
            return _users.FirstOrDefault(x => x.Code == code);
        }

        //Setup

        [SetUp]
        public void SetUp()
        {
            _users = GetUsers();
            _ipAddresses = GetIpAddresses();

            _userService.Setup(u => u.GetUser(It.IsAny<string>()))
                        .Returns((string code) => GetUserData(code));

            _utilityService.Setup(u => u.GetPosId(It.IsAny<string>()))
                .Returns((string ipAddress) => GetPosIdData(ipAddress));

            _utilityService.Setup(u => u.GetAdminValue(It.IsAny<string>()))
                .Returns((string value) => GetAdminValue(value));

            _utilityService.Setup(u => u.GetPosAddress(It.IsAny<byte>()))
                .Returns((byte posId) => GetPosIpAddress(posId));

            _policyManager.Setup(l => l.LoadSecurityInfo()).Returns(LoadSecurityInfo());

            _policyManager.Setup(l => l.LoadStoreInfo()).Returns(LoadStoreInfo());
            _loginManager = new LoginManager(_utilityService.Object, _userService.Object,
                                         _loginService.Object, _resourceManager, _tillService.Object,
                                          _shiftService.Object, _policyManager.Object);
        }

        /// <summary>
        /// Test the getUser method for valid user
        /// </summary>
        [Test]
        public void GetUserTest()
        {
            var expected = "X";
            var userCode = "X";
            var actual = _loginManager.GetUser(userCode);
            Assert.AreEqual(expected, actual.Name);
        }

        /// <summary>
        /// Test the valid getuser for invalid user
        /// </summary>
        [Test]
        public void GetUserForInvalidUserTest()
        {
            var userCode = "ABC";
            var actual = _loginManager.GetUser(userCode);
            Assert.IsNull(actual);
        }

        /// <summary>
        /// test the get user for empty code 
        /// </summary>
        [Test]
        public void GetUserForEmptyCodeTest()
        {
            var userCode = string.Empty;
            var actual = _loginManager.GetUser(userCode);
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Test the get user for Null code 
        /// </summary>
        [Test]
        public void GetUserForNullCodeTest()
        {
            var userCode = string.Empty;
            var actual = _loginManager.GetUser(userCode);
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Test the GetPosID for valid ip address
        /// </summary>
        [Test]
        public void GetPosIdTest()
        {
            var expected = 1;
            var ipAddress = "172.0.0.0";
            var actual = _loginManager.GetPosId(ipAddress);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test the GetPosId for invalid ip address
        /// </summary>
        [Test]
        public void GetPosIdForInvalidIpTest()
        {
            var expected = 0;
            var ipAddress = "27.0.0.0";
            var actual = _loginManager.GetPosId(ipAddress);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test the GetPosId for null ip address
        /// </summary>
        [Test]
        public void GetPosIdForNullIpTest()
        {
            var expected = 0;
            var actual = _loginManager.GetPosId(null);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test the getPosId for empty Ip Address
        /// </summary>
        [Test]
        public void GetPosIdForEmptyIpTest()
        {
            var expected = 0;
            var ipAddress = string.Empty;
            var actual = _loginManager.GetPosId(ipAddress);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test the valid user for valid user
        /// </summary>
        [Test]
        public void IsValidUserTest()
        {
            var userName = "X";
            var password = "abc";
            ErrorMessage errorMessage;
            var actual = _loginManager.IsValidUser(userName, password, out errorMessage);
            Assert.IsTrue(actual);
            Assert.IsNull(errorMessage.MessageStyle.Message);
        }

        /// <summary>
        /// Test the isValidUser for invalid user
        /// </summary>
        [Test]
        public void IsValidUserForInvalidUserNameTest()
        {
            var error = "UserID Y does not exist. ~No Such User";
            var userName = "Y";
            var password = "abc";
            ErrorMessage errorMessage;
            var actual = _loginManager.IsValidUser(userName, password, out errorMessage);
            Assert.IsFalse(actual);
            Assert.AreEqual(error, errorMessage.MessageStyle.Message);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, errorMessage.StatusCode);
            Assert.IsFalse(errorMessage.ShutDownPos);
        }


        /// <summary>
        /// Test isValiduser for invalid password
        /// </summary>
        [Test]
        public void IsValidUserForInvalidPasswordTest()
        {
            var error = "Invalid  Password~Invalid Password";
            var userName = "A";
            var password = "xyz";
            ErrorMessage errorMessage;
            var actual = _loginManager.IsValidUser(userName, password, out errorMessage);
            Assert.IsFalse(actual);
            Assert.AreEqual(error, errorMessage.MessageStyle.Message);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, errorMessage.StatusCode);
            Assert.IsFalse(errorMessage.ShutDownPos);
        }


        /// <summary>
        /// Test IsValidUser for empty username
        /// </summary>
        [Test]
        public void IsValidUserForEmptyUserNameTest()
        {
            var error = "Please provide username";
            var userName = string.Empty;
            var password = "abc";
            ErrorMessage errorMessage;
            var actual = _loginManager.IsValidUser(userName, password, out errorMessage);
            Assert.IsFalse(actual);
            Assert.AreEqual(error, errorMessage.MessageStyle.Message);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, errorMessage.StatusCode);
            Assert.IsFalse(errorMessage.ShutDownPos);
        }

        /// <summary>
        /// Test IsvalidUser for Empty Password
        /// </summary>
        [Test]
        public void IsValidUserForEmptyPasswordTest()
        {
            var error = "Please provide password";
            var userName = "X";
            var password = string.Empty;
            ErrorMessage errorMessage;
            var actual = _loginManager.IsValidUser(userName, password, out errorMessage);
            Assert.IsFalse(actual);
            Assert.AreEqual(error, errorMessage.MessageStyle.Message);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, errorMessage.StatusCode);
            Assert.IsFalse(errorMessage.ShutDownPos);
        }

        /// <summary>
        /// Test isValid user for null user Name
        /// </summary>
        [Test]
        public void IsValidUserForNullUserNameTest()
        {
            var error = "Please provide username";
            var password = "abc";
            ErrorMessage errorMessage;
            var actual = _loginManager.IsValidUser(null, password, out errorMessage);
            Assert.IsFalse(actual);
            Assert.AreEqual(error, errorMessage.MessageStyle.Message);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, errorMessage.StatusCode);
            Assert.IsFalse(errorMessage.ShutDownPos);
        }


        /// <summary>
        /// Test isValiduser for null password
        /// </summary>
        [Test]
        public void IsValidUserForNullPasswordTest()
        {
            var error = "Please provide password";
            var userName = "X";
            ErrorMessage errorMessage;
            var actual = _loginManager.IsValidUser(userName, null, out errorMessage);
            Assert.IsFalse(actual);
            Assert.AreEqual(error, errorMessage.MessageStyle.Message);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, errorMessage.StatusCode);
            Assert.IsFalse(errorMessage.ShutDownPos);
        }

        /// <summary>
        /// Test the Authenticate method for valid Ip Address
        /// </summary>
        [Test]
        public void AuthenticateTest()
        {
            var expected = 1;
            var ipAddress = "172.16.0.0";
            string message;
            ErrorMessage error;
            _policyManager.Setup(l => l.LoadSecurityInfo()).Returns(LoadSecurityInfo);
            _loginManager = new LoginManager(_utilityService.Object, _userService.Object,
                                         _loginService.Object, _resourceManager, _tillService.Object,
                                          _shiftService.Object, _policyManager.Object);
            var actual = _loginManager.Authenticate(ipAddress, out message, out error);
            Assert.IsNotNull(error.MessageStyle.Message);

        }
        /// <summary>
        /// /Test the Authenticate method for invalid Ip Address
        /// </summary>
        [Test]
        public void AuthenticateForInvalidIpTest()
        {
            _policyManager.Setup(l => l.LoadSecurityInfo()).Returns(LoadSecurityInfo);
            _loginManager = new LoginManager(_utilityService.Object, _userService.Object,
                                         _loginService.Object, _resourceManager, _tillService.Object,
                                          _shiftService.Object, _policyManager.Object);
            var expected = "You don't have permission to login to POS. Please contact your Dealer.";
            var ipAddress = "127.0.0.0";
            string message;
            ErrorMessage error;
            _loginManager.Authenticate(ipAddress, out message, out error);
            Assert.AreEqual(expected, error.MessageStyle.Message);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, error.StatusCode);
            Assert.IsTrue(error.ShutDownPos);
        }

        /// <summary>
        /// Test Authenticate for empty security code 
        /// </summary>
        [Test]
        public void AuthenticateForEmptySecutiyCodeTest()
        {
            _policyManager.Setup(l => l.LoadSecurityInfo()).Returns(LoadEmptySecurityInfo());
            _loginManager = new LoginManager(_utilityService.Object, _userService.Object,
                                         _loginService.Object, _resourceManager, _tillService.Object,
                                          _shiftService.Object, _policyManager.Object);
            var expected = "License Expired. Please get a Valid Security Code from your Dealer.";
            var ipAddress = "172.0.0.0";
            string message;
            ErrorMessage error;
            _loginManager.Authenticate(ipAddress, out message, out error);
            Assert.AreEqual(expected, error.MessageStyle.Message);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, error.StatusCode);
            Assert.IsTrue(error.ShutDownPos);
        }

        /// <summary>
        /// Test Authenticate for invalid Security Code test
        /// </summary>
        [Test]
        public void AuthenticateForInvalidSecutiyCodeTest()
        {
            _policyManager.Setup(l => l.LoadSecurityInfo()).Returns(LoadInvalidSecurityInfo());
            _loginManager = new LoginManager(_utilityService.Object, _userService.Object,
                                          _loginService.Object, _resourceManager, _tillService.Object,
                                           _shiftService.Object, _policyManager.Object);
            var expected = "You don't have permission to login to POS. Please contact your Dealer.";
            var ipAddress = "172.0.0.0";
            string message;
            ErrorMessage error;
            _loginManager.Authenticate(ipAddress, out message, out error);
            Assert.AreEqual(expected, error.MessageStyle.Message);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, error.StatusCode);
            Assert.IsTrue(error.ShutDownPos);
        }

        /// <summary>
        /// Test Authenticate for invalid Date
        /// </summary>
        [Test]
        public void AuthenticateForInvalidDateTest()
        {
            _policyManager.Setup(l => l.LoadSecurityInfo()).Returns(LoadInvalidDateInfo());
            _loginManager = new LoginManager(_utilityService.Object, _userService.Object,
                                         _loginService.Object, _resourceManager, _tillService.Object,
                                          _shiftService.Object, _policyManager.Object);
            var expected = "You don't have permission to login to POS. Please contact your Dealer.";
            var ipAddress = "172.0.0.0";
            string message;
            ErrorMessage error;
            _loginManager.Authenticate(ipAddress, out message, out error);
            Assert.AreEqual(expected, error.MessageStyle.Message);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, error.StatusCode);
            Assert.IsTrue(error.ShutDownPos);
        }

        /// <summary>
        /// Test Authenticate for expiring Date 
        /// </summary>
        [Test]
        public void AuthenticateForExpiringDateTest()
        {
            _policyManager.Setup(l => l.LoadSecurityInfo()).Returns(LoadEmptySecurityCodeAdvancedInstallDateInfo());
            _loginManager = new LoginManager(_utilityService.Object, _userService.Object,
                                        _loginService.Object, _resourceManager, _tillService.Object,
                                         _shiftService.Object, _policyManager.Object);
            var expected = "License Expired. Please get a Valid Security Code from your Dealer.";
            var ipAddress = "172.0.0.0";
            string message;
            ErrorMessage error;
            _loginManager.Authenticate(ipAddress, out message, out error);
            Assert.AreEqual(expected, error.MessageStyle.Message);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, error.StatusCode);
            Assert.IsTrue(error.ShutDownPos);
        }

        /// <summary>
        /// Test the GetIpAddress for valid PosId
        /// </summary>
        [Test]
        public void GetIpAddressTest()
        {
            var expected = "172.0.0.0";
            var posId = (byte)1;
            var actual = _loginManager.GetIpAddress(posId);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test the GetIpAddress for invalid PosId
        /// 
        /// </summary>
        [Test]
        public void GetIpAddressForInvalidPosIdTest()
        {
            var expected = string.Empty;
            var posId = (byte)0;
            var actual = _loginManager.GetIpAddress(posId);
            Assert.AreEqual(expected, actual);
        }

    }
}

//using Infonet.CStoreCommander.Data;
//using Infonet.CStoreCommander.Data.CSCAdmin;
//using Infonet.CStoreCommander.Data.Repositories;
//using Infonet.CStoreCommander.Data.ServiceClasses;
//using Moq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Infonet.CStoreCommander.WebApi.Tests.Services
//{
//    [TestFixture]
//    public class UserServiceTest
//    {
//        private Mock<IRepository<USER>> _userRepository = new Mock<IRepository<USER>>();
//        private Mock<IRepository<UIG>> _userGroupRepository = new Mock<IRepository<UIG>>();
//        private IUserService _userService;

//        /// <summary>
//        /// Setup
//        /// </summary>
//        [SetUp]
//        public void Setup()
//        {
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
//                EPW = "54321",
//                U_CODE = "2",
//                U_ID = 1,
//                U_NAME = "Test User 2"
//            };


//            var firstUserGroup = new UIG
//            {
//                USER = "Test User 1",
//                UGROUP = "Manager"
//            };

//            var secondUserGroup = new UIG
//            {
//                USER = "Test User 2",
//                UGROUP = "Trainer"
//            };

//            //create users
//            var users = new List<USER>();
//            users.Add(firstUser);
//            users.Add(secondUser);

//            //create user groups
//            var userGroups = new List<UIG>();
//            userGroups.Add(firstUserGroup);
//            userGroups.Add(secondUserGroup);

//            _userRepository.Setup(u => u.GetAll()).Returns(users.AsQueryable());
//            _userGroupRepository.Setup(u => u.GetAll()).Returns(userGroups.AsQueryable());
//            _userService = new UserService(_userRepository.Object, _userGroupRepository.Object);
//        }

//        /// <summary>
//        /// Test to get uig for valid username
//        /// </summary>
//        [Test]
//        public void GetUigByUserNameForExistingUserNameTest()
//        {
//            var expected = "Manager";
//            var userName = "Test User 1";
//            var actual = _userService.GetUigByUserName(userName);
//            if (actual != null)
//                Assert.AreEqual(expected, actual.UGROUP);
//        }

//        /// <summary>
//        /// Test to get uig in case of non existing user name
//        /// </summary>
//        [Test]
//        public void GetUigByUserNameForNonExistingUserNameTest()
//        {
//            UIG expected = null;
//            var userName = "Test";
//            var actual = _userService.GetUigByUserName(userName);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get uig in case of null user name
//        /// </summary>
//        [Test]
//        public void GetUigByUserNameForNullUserNameTest()
//        {
//            UIG expected = null;
//            var actual = _userService.GetUigByUserName(null);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get uig in case of empty user name
//        /// </summary>
//        [Test]
//        public void GetUigByUserNameForEmptyUserNameTest()
//        {
//            UIG expected = null;
//            var userName = string.Empty;
//            var actual = _userService.GetUigByUserName(userName);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get user for existing user name
//        /// </summary>
//        [Test]
//        public void GeUserForExistingUserNameTest()
//        {
//            var expected = "1";
//            var userName = "Test User 1";
//            var actual = _userService.GetUser(userName);
//            if (actual != null)
//                Assert.AreEqual(expected, actual.U_CODE);
//        }

//        /// <summary>
//        /// Test to get user for non existing user name
//        /// </summary>
//        [Test]
//        public void GeUserForNonExistingUserNameTest()
//        {
//            USER expected = null;
//            var userName = "Test";
//            var actual = _userService.GetUser(userName);
//            Assert.AreEqual(expected, actual);

//        }

//        /// <summary>
//        /// Test to get user for null user name
//        /// </summary>
//        [Test]
//        public void GeUserForNullUserNameTest()
//        {
//            USER expected = null;
//            var actual = _userService.GetUser(null);
//            Assert.AreEqual(expected, actual);

//        }

//        /// <summary>
//        /// Test to get user for empty user name
//        /// </summary>
//        [Test]
//        public void GeUserForEmptyUserNameTest()
//        {
//            USER expected = null;
//            var userName = string.Empty;
//            var actual = _userService.GetUser(userName);
//            Assert.AreEqual(expected, actual);

//        }
//    }
//}

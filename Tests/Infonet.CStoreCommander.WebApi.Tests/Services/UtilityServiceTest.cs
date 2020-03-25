//using Infonet.CStoreCommander.Data;
//using Infonet.CStoreCommander.Data.CSCAdmin;
//using Infonet.CStoreCommander.Data.Repositories;
//using Infonet.CStoreCommander.Data.ServiceClasses;
//using Moq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Infonet.CStoreCommander.WebApi.Tests.Services
//{
//    /// <summary>
//    /// Test calss for utility service
//    /// </summary>
//    [TestFixture]
//   public class UtilityServiceTest
//    {
//        private Mock<IRepository<POS_IP_Address>> _posIpAddressRepository =
//                                                new Mock<IRepository<POS_IP_Address>>();
//        private IUtilityService _utilityService;

//        /// <summary>
//        /// Setup
//        /// </summary>
//        [SetUp]
//        public void SetUp()
//        {
//            // Set up some testing data
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

//            //create pos ip addresses
//            var posIpAddresses = new List<POS_IP_Address>();
//            posIpAddresses.Add(firstIpAddress);
//            posIpAddresses.Add(secondIpAddress);

//            //get data
//            _posIpAddressRepository.Setup(c => c.GetAll()).Returns(posIpAddresses.AsQueryable());
//            _utilityService = new UtilityService(_posIpAddressRepository.Object);

//        }

//        /// <summary>
//        /// Test to validate ip address for a valid pos ip
//        /// </summary>
//        [Test]
//        public void ValidateIpAddressTestForValidIp()
//        {
//            var expected = 1;
//            var ipAddress = "172.16.16.1";
//            var actual = _utilityService.ValidateIpAddress(ipAddress);
//            Assert.AreEqual(expected, actual);

//        }

//        /// <summary>
//        /// Test to validate ip address for an invalid pos ip
//        /// </summary>
//        [Test]
//        public void ValidateIpAddressTestForInvalidIp()
//        {
//            var expected = -1;
//            var ipAddress = "0.0.0.0";
//            var actual = _utilityService.ValidateIpAddress(ipAddress);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to validate ip address for null pos ip
//        /// </summary>
//        [Test]
//        public void ValidateIpAddressTestFoNullIp()
//        {
//            var expected = -1;
//            var actual = _utilityService.ValidateIpAddress(null);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to validate ip address for empty pos ip
//        /// </summary>
//        [Test]
//        public void ValidateIpAddressTestForEmptyIp()
//        {
//            var expected = -1;
//            var ipAddress = string.Empty;
//            var actual = _utilityService.ValidateIpAddress(ipAddress);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get the ip address for an existing pos id
//        /// </summary>
//        [Test]
//        public void GetIpByForExistingPosId()
//        {
//            var expected = "172.16.16.1";
//            var posId = 1;
//            var actual = _utilityService.GetIpByPosId(posId);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get the ip address for a non existing pos id
//        /// </summary>
//        [Test]
//        public void GetIpByForNonExistingPosId()
//        {
//            string expected = null;
//            var posId = 0;
//            var actual = _utilityService.GetIpByPosId(posId);
//            Assert.AreEqual(expected, actual);
//        }
//    }
//}

//using Infonet.CStoreCommander.Data;
//using Infonet.CStoreCommander.Data.CSCAdmin;
//using Infonet.CStoreCommander.Data.CSCMaster;
//using Infonet.CStoreCommander.Data.Repositories;
//using Infonet.CStoreCommander.Data.ServiceClasses;
//using Moq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Infonet.CStoreCommander.WebApi.Tests.Services
//{
//    /// <summary>
//    /// Test for till service
//    /// </summary>
//    [TestFixture]
//   public class TillServiceTest
//    {
//        private Mock<IRepository<ShiftStore>> _shiftStoreRepository = 
//                                                    new Mock<IRepository<ShiftStore>>();
//        private Mock<IRepository<TILL>> _tillRepository =
//                                                    new Mock<IRepository<TILL>>();

//        private ITillService _tillService;

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
//                PROCESS = true,
//                ShiftDate = DateTime.Today.AddDays(2),
//                ShiftNumber = 2,
//                TILL_NUM = 2,
//                TIME_OPEN = DateTime.Today.AddDays(2),
//                UserLoggedOn = "Test User 2"
//            };

//            //create shifts
//            var shifts = new List<ShiftStore>();
//            shifts.Add(firstShiftStore);
//            shifts.Add(secondShiftStore);

//            //create tills
//            var tills = new List<TILL>();
//            tills.Add(firstTill);
//            tills.Add(secondTill);

//            _shiftStoreRepository.Setup(u => u.GetAll()).Returns(shifts.AsQueryable());
//            _tillRepository.Setup(u => u.GetAll()).Returns(tills.AsQueryable());
//            _tillService = new TillService(_shiftStoreRepository.Object, 
//                                                _tillRepository.Object);

//        }

//        /// <summary>
//        /// Test to get shift for an existing shift number
//        /// </summary>
//        [Test]
//        public void GetShiftForExistingShiftNumberTest()
//        {
//            var expected = DateTime.Today;
//            var shiftNumber = 1;
//            var actual = _tillService.GetShift(shiftNumber);
//            if (actual != null)
//                Assert.AreEqual(expected, actual.StartTime);

//        }

//        /// <summary>
//        /// Test to get shift for a non existing shift number
//        /// </summary>
//        [Test]
//        public void GetShiftForNOnExistingShiftNumberTest()
//        {
//            ShiftStore expected = null;
//            var shiftNumber = 0;
//            var actual = _tillService.GetShift(shiftNumber);
//                Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get all shifts
//        /// </summary>
//        [Test]
//        public void GetAllShiftsTest()
//        {
//            var expected = 2;
//            var actual = _tillService.GetShifts(null,null).Count();
//                Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get all active shifts
//        /// </summary>
//        [Test]
//        public void GetActiveShiftsTest()
//        {
//            var expected = 1;
//            var isActive = true;
//            var actual = _tillService.GetShifts(isActive, null).Count();
//            Assert.AreEqual(expected, actual);
//        }


//        /// <summary>
//        /// Test to get shifts greater than a shift number
//        /// </summary>
//        [Test]
//        public void GetHigherShiftsTest()
//        {
//            var expected = 1;
//            var minimumShiftNumber = 1;
//            var actual = _tillService.GetShifts(null, minimumShiftNumber).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get active shifts greater than a shift number
//        /// </summary>
//        [Test]
//        public void GetActiveAndHigherShiftsTest()
//        {
//            var expected = 0;
//            var isActive = true;
//            var minimumShiftNumber = 1;
//            var actual = _tillService.GetShifts(isActive, minimumShiftNumber).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get till for an existing till number
//        /// </summary>
//        [Test]
//        public void GetTillForExistingTillNumberTest()
//        {
//            var expected = true;
//            var tillNumber = 1;
//            var actual = _tillService.GetTill(tillNumber);
//            if (actual != null)
//                Assert.AreEqual(expected, actual.ACTIVE);
//        }

//        /// <summary>
//        /// Test to get till for a non existing till number
//        /// </summary>
//        [Test]
//        public void GetTillForNonExistingTillNumberTest()
//        {
//            TILL expected = null;
//            var tillNumber = 0;
//            var actual = _tillService.GetTill(tillNumber);
//                Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get all tills
//        /// </summary>
//        [Test]
//        public void GetAllTillsTest()
//        {
//            var expected = 2;
//            var actual = _tillService.GetTills(null, null, null, null, null).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get all active tills
//        /// </summary>
//        [Test]
//        public void GetActiveTillsTest()
//        {
//            var expected = 1;
//            var isActive = true;
//            var actual = _tillService.GetTills(isActive, null, null, null, null).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get all processing tills
//        /// </summary>
//        [Test]
//        public void GetProcessingTillsTest()
//        {
//            var expected = 2;
//            var isProcessing = true;
//            var actual = _tillService.GetTills(null, isProcessing, null, null, null).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get till for a user
//        /// </summary>
//        [Test]
//        public void GetTillsForAUser()
//        {
//            var expected = 1;
//            var user = "Test User 1";
//            var actual = _tillService.GetTills(null, null, user, null, null).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get till for a shift date
//        /// </summary>
//        [Test]
//        public void GetTillForAShiftDate()
//        {
//            var expected = 1;
//            var shiftDate = DateTime.Today;
//            var actual = _tillService.GetTills(null, null, null, shiftDate, null).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get till for a pos id
//        /// </summary>
//        [Test]
//        public void GetTillForPosID()
//        {
//            var expected = 1;
//            var posId = 2;
//            var actual = _tillService.GetTills(null, null, null, null, posId).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get tills which are active, processing and being user by a particular
//        /// user, at a given date and pos Id
//        /// </summary>
//        [Test]
//        public void GetTillActiveAndProcessingTillForAUserAtADateAndPosID()
//        {
//            var expected = 1;
//            var isActive = true;
//            var isProcessing = true;
//            var user = "Test User 1";
//            var shiftDate = DateTime.Today;
//            var posId = 1;
//            var actual = _tillService.GetTills(isActive, isProcessing, user, shiftDate, 
//                                               posId).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to update an existing shift
//        /// </summary>
//        [Test]
//        public void UpdateShiftTest()
//        {
//            var expected = true;
//            var updatedShift = new ShiftStore
//            {
//                Active = true,
//                CurrentDay = true,
//                ShiftNumber = 2,
//                StartTime = DateTime.Today.AddDays(5)
//            };
//            var actual = _tillService.UpdateShift(updatedShift);
//            Assert.AreEqual(expected, actual);

//        }

//        /// <summary>
//        /// Test to update a null shift
//        /// </summary>
//        [Test]
//        public void UpdateNullShiftTest()
//        {
//            var expected = false;
//            var actual = _tillService.UpdateShift(null);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to update a non existing shift
//        /// </summary>
//        [Test]
//        public void UpdateNonExistingShiftTest()
//        {
//            var expected = false;
//            var updatedShift = new ShiftStore
//            {
//                Active = true,
//                CurrentDay = true,
//                ShiftNumber = 99,
//                StartTime = DateTime.Today.AddDays(2)
//            };
//            var actual = _tillService.UpdateShift(updatedShift);
//            Assert.AreEqual(expected, actual);
//        }
        
//        /// <summary>
//        /// Test to update an existing till
//        /// </summary>
//        [Test]
//        public void UpdateTillTest()
//        {
//            var expected = true;
//            var updatedTill = new TILL
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
//                UserLoggedOn = "Updated User"
//            };
//            var actual = _tillService.UpdateTill(updatedTill);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to update a null till
//        /// </summary>
//        [Test]
//        public void UpdateNullTillTest()
//        {
//            var expected = false;
//            var actual = _tillService.UpdateTill(null);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to update a non existing till
//        /// </summary>
//        [Test]
//        public void UpdateNonExistingTillTest()
//        {
//            var expected = false;
//            var updatedTill = new TILL
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
//                TILL_NUM = 99,
//                TIME_OPEN = DateTime.Today,
//                UserLoggedOn = "Updated User"
//            };
//            var actual = _tillService.UpdateTill(updatedTill);
//            Assert.AreEqual(expected, actual);
//        }
//    }
//}

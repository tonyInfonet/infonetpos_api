//using System;
//using Infonet.CStoreCommander.ADOData;
//using Infonet.CStoreCommander.BusinessLayer.Manager;
//using Infonet.CStoreCommander.BusinessLayer.Utilities;
//using Infonet.CStoreCommander.Entities;
//using Infonet.CStoreCommander.Resources;
//using Moq;
//using NUnit.Framework;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//namespace Infonet.CStoreCommander.WebApi.Tests.Managers
//{
//    /// <summary>
//    /// Test methods for bottle manager
//    /// </summary>
//    [TestFixture]
//    public class BottleManagerTest
//    {
//        private readonly Mock<ISaleService> _saleService = new Mock<ISaleService>();
//        private readonly Mock<IPolicyManager> _policyManagerMock = new Mock<IPolicyManager>();
//        private readonly Mock<IBottleReturnService> _bottleReturnService = new Mock<IBottleReturnService>();
//        private readonly IApiResourceManager _resourceManager = new ApiResourceManager();
//        private readonly Mock<ISaleManager> _saleManager = new Mock<ISaleManager>();
//        private readonly Mock<ILoginManager> _loginManager = new Mock<ILoginManager>();
//        private readonly Mock<ITenderManager> _tenderManager = new Mock<ITenderManager>(MockBehavior.Loose);
//        private readonly Mock<IReceiptManager> _receiptManager = new Mock<IReceiptManager>();
//        private readonly Mock<ITillService> _tillService = new Mock<ITillService>();
//        private IBottleManager _bottleManager;
//        List<Till> _tills;

//        private List<BottleReturn> GetBottlesData()
//        {
//            var firstBottle = new BottleReturn
//            {
//                Description = "First Bottle",
//                Image_Url = "FirstBottle.jpg",
//                Product = "Can",
//                Price = 1.1f,
//                Quantity = 1,
//                Amount = 0
//            };
//            var secondBottle = new BottleReturn
//            {
//                Description = "Second Bottle",
//                Image_Url = "SecondBottle.jpg",
//                Product = "Coke",
//                Price = 2.1f,
//                Quantity = 1,
//                Amount = 0
//            };
//            return new List<BottleReturn>
//            {
//                firstBottle,
//                secondBottle
//            };
//        }

//        private static Dictionary<string, object> GetPolicyForInvalidBaseCurrencyData()
//        {
//            var policies = new Dictionary<string, object>
//            {
//                {"BASECURR", null},
//                {"U_BR_LIMIT", 100},
//                {"Open_Drawer", "Every Sale"},
//                {"Penny_Adj", true}
//            };
//            return policies;
//        }

//        private static Dictionary<string, object> GetPolicyForEverySaleData()
//        {
//            var policies = new Dictionary<string, object>
//            {
//                {"BASECURR", "CASH"},
//                {"U_BR_LIMIT", 100},
//                {"Open_Drawer", "Every Sale"},
//                {"Penny_Adj", true}
//            };
//            return policies;
//        }

//        private static Dictionary<string, object> GetPolicyForNoPennyAdjustData()
//        {
//            var policies = new Dictionary<string, object>
//            {
//                {"BASECURR", "CASH"},
//                {"U_BR_LIMIT", 100},
//                {"Open_Drawer", "Last Sale"},
//                {"Penny_Adj", false}
//            };
//            return policies;
//        }

//        private static Dictionary<string, object> GetPolicyForPennyAdjustData()
//        {
//            var policies = new Dictionary<string, object>
//            {
//                {"BASECURR", "CASH"},
//                {"U_BR_LIMIT", 100},
//                {"Open_Drawer", "Last Sale"},
//                {"Penny_Adj", true}
//            };
//            return policies;
//        }

//        private static object GetPolicy(string policyName, int condition)
//        {
//            var policies = new Dictionary<string, object>();
//            switch (condition)
//            {
//                case 1: policies = GetPolicyForInvalidBaseCurrencyData(); break;
//                case 2: policies = GetPolicyForNoPennyAdjustData(); break;
//                case 3: policies = GetPolicyForPennyAdjustData(); break;
//                case 4: policies = GetPolicyForEverySaleData(); break;
//            }
//            object value;
//            return policies.TryGetValue(policyName, out value) ? value : null;
//        }

//        /// <summary>
//        /// Set the test Data for GetTill
//        /// </summary>
//        /// <param name="tillNumber"></param>
//        /// <returns></returns>
//        private Till GetTillData(int tillNumber)
//        {
//            return _tills.FirstOrDefault(x => x.Number == tillNumber);
//        }

//        /// <summary>
//        /// Set the test Data for tills 
//        /// </summary>
//        /// <returns></returns>
//        private List<Till> GetTills()
//        {
//            var firstTill = new Till
//            {
//                Active = true,
//                Processing = true,
//                POSId = 1,
//                Date_Open = DateTime.Now,
//                Time_Open = DateTime.Now,
//                UserLoggedOn = "X",
//                Shift = 1,
//                ShiftDate = DateTime.Now,
//                Number = 1
//            };

//            var secondTill = new Till
//            {
//                Active = true,
//                Processing = false,
//                POSId = 2,
//                Date_Open = DateTime.Now,
//                Time_Open = DateTime.Now,
//                UserLoggedOn = "Y",
//                Shift = 2,
//                ShiftDate = DateTime.Now,
//                Number = 7
//            };
//            var thirdTill = new Till
//            {
//                Active = false,
//                Processing = false,
//                POSId = 2,
//                Date_Open = DateTime.Now,
//                Time_Open = DateTime.Now,
//                UserLoggedOn = "Y",
//                Shift = 2,
//                ShiftDate = DateTime.Now,
//                Number = 7
//            };

//            _tills = new List<Till>()
//                {
//                    firstTill,
//                    secondTill,
//                    thirdTill
//                };

//            return _tills;
//        }

//        [SetUp]
//        public void Setup()
//        {
//            _tills = GetTills();
//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                        .Returns((int tillNumber) => GetTillData(tillNumber));
//            _bottleReturnService.Setup(b => b.GetBottlesFromDbMaster(It.IsAny<int>(), It.IsAny<int>()))
//                .Returns(GetBottlesData());
//            _loginManager.Setup(u => u.GetUser(It.IsAny<string>())).Returns(new User {Code = "X",Name = "123"});
//            _bottleManager = new BottleManager(_bottleReturnService.Object, _policyManagerMock.Object,
//                _resourceManager, _saleService.Object, _saleManager.Object, _loginManager.Object,
//                _tenderManager.Object, _receiptManager.Object, _tillService.Object);
//        }

//        [Test]
//        public void GetBottlesTest()
//        {
//            var expected = 2;
//            var actual = _bottleManager.GetBottles();
//            Assert.AreEqual(expected, actual.Count);
//        }

//        [Test]
//        public void SaveBottleReturnForNoDefinedBaseCurrencyTest()
//        {
//            _policyManagerMock.Setup(p => p.GetPol(It.IsAny<string>(), It.IsAny<object>()))
//                .Returns((string policyName, object value) => GetPolicy(policyName, 1));
//            _bottleManager = new BottleManager(_bottleReturnService.Object, _policyManagerMock.Object,
//                _resourceManager, _saleService.Object, _saleManager.Object, _loginManager.Object,
//                _tenderManager.Object, _receiptManager.Object, _tillService.Object);
//            var expected = "Base currency is not defined. Bottle Return cannot be saved.~No base currency!";
//            ErrorMessage error;
//            var bottleReturn = new BR_Payment
//            {
//                Sale_Num = 1,
//                TillNumber = 1,
//                Penny_Adj = 0,
//                Amount = 30,
//                Br_Lines = new BottleReturns()
//            };
//            var userCode = "X";
//            FileStream fs;
//            bool openDrawer;
//            _bottleManager.SaveBottleReturn(bottleReturn, userCode, out error, out fs,out openDrawer);
//            Assert.AreEqual(expected, error.MessageStyle.Message);
//        }

//        [Test]
//        public void SaveBottleReturnForExceedingReturnLimitTest()
//        {
//            _policyManagerMock.Setup(p => p.GetPol(It.IsAny<string>(), It.IsAny<object>()))
//                .Returns((string policyName, object value) => GetPolicy(policyName, 3));
//            _bottleManager = new BottleManager(_bottleReturnService.Object, _policyManagerMock.Object,
//              _resourceManager, _saleService.Object, _saleManager.Object, _loginManager.Object,
//              _tenderManager.Object, _receiptManager.Object, _tillService.Object);
//            var expected = "Exceed the bottle return limit, Please get an authorized user!~Get an authorized user!";
//            ErrorMessage error;
//            var bottleReturn = new BR_Payment
//            {
//                Sale_Num = 1,
//                TillNumber = 1,
//                Penny_Adj = 0,
//                Amount = 999,
//                Br_Lines = new BottleReturns()
//            };
//            var userCode = "X";
//            FileStream fs;
//            bool openDrawer;
//            _bottleManager.SaveBottleReturn(bottleReturn, userCode, out error, out fs, out openDrawer);
//            Assert.AreEqual(expected, error.MessageStyle.Message);
//        }

//        [Test]
//        public void SaveBottleReturnForEverySaleDrawerTest()
//        {
//            _policyManagerMock.Setup(p => p.GetPol(It.IsAny<string>(), It.IsAny<object>()))
//                .Returns((string policyName, object value) => GetPolicy(policyName, 4));
//            _saleService.Setup(s => s.GetSale(It.IsAny<int>(), It.IsAny<int>()))
//                .Returns(new Sale());
//            _bottleManager = new BottleManager(_bottleReturnService.Object, _policyManagerMock.Object,
//               _resourceManager, _saleService.Object, _saleManager.Object, _loginManager.Object,
//               _tenderManager.Object, _receiptManager.Object, _tillService.Object);
//            ErrorMessage error;
//            var bottleReturn = new BR_Payment
//            {
//                Sale_Num = 1,
//                TillNumber = 1,
//                Penny_Adj = 0,
//                Amount = 10,
//                Br_Lines = new BottleReturns()
//            };
//            var userCode = "X";
//            FileStream fs;
//            bool openDrawer;
//            _bottleManager.SaveBottleReturn(bottleReturn, userCode, out error, out fs, out openDrawer);
//            Assert.IsNull(error.MessageStyle.Message);
//        }


//        [Test]
//        public void SaveBottleReturnForNoPennyAdjustTest()
//        {
//            _policyManagerMock.Setup(p => p.GetPol(It.IsAny<string>(), It.IsAny<object>()))
//                .Returns((string policyName, object value) => GetPolicy(policyName, 2));
//            _saleService.Setup(s => s.GetSale(It.IsAny<int>(), It.IsAny<int>()))
//                .Returns(new Sale());
//            _bottleManager = new BottleManager(_bottleReturnService.Object, _policyManagerMock.Object,
//               _resourceManager, _saleService.Object, _saleManager.Object, _loginManager.Object,
//               _tenderManager.Object, _receiptManager.Object, _tillService.Object);
//            ErrorMessage error;
//            var bottleReturn = new BR_Payment
//            {
//                Sale_Num = 1,
//                TillNumber = 1,
//                Penny_Adj = 0,
//                Amount = 10,
//                Br_Lines = new BottleReturns()
//            };
//            var userCode = "X";
//            FileStream fs;
//            bool openDrawer;
//            _bottleManager.SaveBottleReturn(bottleReturn, userCode, out error, out fs, out openDrawer);
//            Assert.IsNull(error.MessageStyle.Message);
//        }
//    }
//}

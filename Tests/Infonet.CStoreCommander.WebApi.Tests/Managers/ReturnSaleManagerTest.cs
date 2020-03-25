//using Infonet.CStoreCommander.ADOData;
//using Infonet.CStoreCommander.BusinessLayer.Manager;
//using Infonet.CStoreCommander.BusinessLayer.Utilities;
//using Infonet.CStoreCommander.Entities;
//using Infonet.CStoreCommander.Resources;
//using Moq;
//using NUnit.Framework;
//using System.Collections.Generic;

//using System.Linq;

//namespace Infonet.CStoreCommander.WebApi.Tests.Managers
//{
//    [TestFixture]
//    public class ReturnSaleManagerTest
//    {
//        private Mock<IReturnSaleService> _returnSaleService = new Mock<IReturnSaleService>();
//        private Mock<IPolicyService> _policyService = new Mock<IPolicyService>();
//        private Mock<ISaleManager> _saleManager = new Mock<ISaleManager>();
//        private Mock<IReasonService> _reasonService = new Mock<IReasonService>();
//        private Mock<ISaleService> _saleService = new Mock<ISaleService>();
//        private Mock<ISaleLineManager> _saleLineManager = new Mock<ISaleLineManager>();
//        private Mock<ISaleHeadManager> _saleHeadManager = new Mock<ISaleHeadManager>();



//        private IApiResourceManager _resourceManager = new ApiResourceManager();
//        private ReturnSaleManager _returnSaleManager;

//        [SetUp]
//        public void Setup()
//        {
//            _returnSaleManager = new ReturnSaleManager(_returnSaleService.Object, _policyService.Object,
//                   _resourceManager, _saleManager.Object, _reasonService.Object, _saleService.Object, _saleLineManager.Object, _saleHeadManager.Object);
//        }

//        /// <summary>
//        /// Get Sale Head Test Data
//        /// </summary>
//        /// <returns></returns>
//        private List<SaleHead> GetAllSaleHeadTestData()
//        {
//            var firstSaleHead = new SaleHead
//            {
//                SaleNumber = 1,
//                TillNumber = 1,
//                SaleAmount = 100,
//                SaleDate = System.DateTime.Now

//            };
//            var secondSaleHead = new SaleHead
//            {
//                SaleNumber = 2,
//                TillNumber = 1,
//                SaleAmount = 200,
//                SaleDate = System.DateTime.Now

//            };

//            var saleHead = new List<SaleHead>
//            {
//               firstSaleHead,
//               secondSaleHead
//            };

//            return saleHead;
//        }

//        /// <summary>
//        /// Set the user Test Data 
//        /// </summary>
//        /// <returns></returns>
//        private List<User> GetUsers()
//        {
//            EncryptionManager encryption = new EncryptionManager();
//            var text = encryption.EncryptText("abc");
//            var firstUser = new User
//            {
//                Name = "X",
//                Code = "X",
//                epw = text,
//                Password = "abc",
//                User_Group = new User_Group
//                {
//                    Code = "Manager",
//                    Name = "Manager",
//                    SecurityLevel = 1
//                }
//            };

//            var secondUser = new User
//            {
//                Name = "A",
//                Code = "A",
//                epw = text,
//                Password = "B",
//                User_Group = new User_Group
//                {
//                    Code = "Developer",
//                    Name = "Manager",
//                    SecurityLevel = 1
//                }
//            };
//            var users = new List<User>
//            {
//                firstUser,
//                secondUser
//            };
//            return users;
//        }

//        /// <summary>
//        /// set the get user test data 
//        /// </summary>
//        /// <param name="code"></param>
//        /// <returns></returns>
//        private User GetUserData(string code)
//        {
//            return GetUsers().FirstOrDefault(x => x.Code == code);
//        }

//        /// <summary>
//        /// GetSaleLines 
//        /// </summary>
//        /// <returns></returns>
//        private List<Sale_Line> GetSaleLineBySaleNumberTestData()
//        {
//            var FirstsaleLines = new Sale_Line
//            {
//                PLU_Code = "Test",
//                Line_Num = 1,
//                User = "X",
//                Till_Num = 1,
//                Sale_Num = 2.ToString(),
//                Quantity = 1,
//                pumpID = (byte)1,
//                PositionID = (byte)1,
//                GradeID = (byte)1,
//                Gift_Certificate = false,
//                Gift_Num = "12345",
//                Serial_No = "123456",
//                // Nicolette end
//                
//                Stock_Code = "099999000443",
//                Prepay = false,
//                Amount = 100.93M,
//                ManualFuel = false,
//                IsTaxExemptItem = false,
//                PaidByCard = (byte)1,
//                Upsell = false,
//                ScalableItem = false,
//                No_Loading = true,
//                Discount_Adjust = 1,
//                Discount_Type = "R",
//                Discount_Code = "",
//                Discount_Rate = 10,
//                Return_Reasons = new Return_Reasons
//                {

//                },
//            };
//            var secondSaleLines = new Sale_Line
//            {
//                PLU_Code = "Test",
//                Line_Num = 2,
//                User = "X",
//                Till_Num = (byte)1,
//                Sale_Num = 2.ToString(),
//                Quantity = 1,
//                pumpID = (byte)1,
//                PositionID = (byte)1,
//                GradeID = (byte)1,
//                Gift_Certificate = false,
//                Gift_Num = "12345",
//                Serial_No = "123456",
//                // Nicolette end
//                
//                Stock_Code = "099999000443",
//                Prepay = false,
//                Amount = 100.93M,
//                ManualFuel = false,
//                IsTaxExemptItem = false,
//                PaidByCard = (byte)1,
//                Upsell = false,
//                ScalableItem = false,
//                No_Loading = true,
//                Discount_Adjust = 1,
//                Discount_Type = "R",
//                Discount_Code = "",
//                Discount_Rate = 10,
//                Return_Reasons = new Return_Reasons
//                {

//                },
//            };

//            return new List<Sale_Line>
//            {
//                FirstsaleLines,
//                secondSaleLines
//            };
//        }
//        /// <summary>
//        /// Get sale Test Data 
//        /// </summary>
//        /// <returns></returns>
//        private Sale GetSaleTestData()
//        {
//            var sale = new Sale
//            {
//                Sale_Num = 1,
//                TillNumber = 1,
//                Shift = 2
//            };
//            return sale;
//        }

//        [Test]
//        public void GetAllSalesTest()
//        {
//            var expected = 2;
//            _returnSaleService.Setup(a => a.GetAllSales(It.IsAny<System.DateTime>(), It.IsAny<int>()
//                , It.IsAny<int>()))
//                .Returns(GetAllSaleHeadTestData());
//            var actual = _returnSaleManager.GetAllSales(1, 20);

//            Assert.AreEqual(expected, actual.Count);
//        }

//        [Test]
//        public void GetSaleWhenISSaleFoundFalseTest()
//        {
//            ErrorMessage error;
//            var expected = "Selected sale number not found.~Sale number not found.";
//            bool isSaleFound = false;
//            bool isReturnAble = false;
//            _returnSaleService.Setup(a => a.GetSaleBySaleNumber(It.IsAny<int>(), It.IsAny<int>(),
//                It.IsAny<System.DateTime>(), out isSaleFound, out isReturnAble))
//                .Returns(GetSaleTestData());

//            _returnSaleService.Setup(a => a.GetSaleLineBySaleNumber(It.IsAny<int>(), It.IsAny<int>(),
//                It.IsAny<System.DateTime>(), It.IsAny<string>()))
//                .Returns(GetSaleLineBySaleNumberTestData());

//            _returnSaleManager = new ReturnSaleManager(_returnSaleService.Object, _policyService.Object,
//                   _resourceManager, _saleManager.Object, _reasonService.Object, _saleService.Object, _saleLineManager.Object, _saleHeadManager.Object);
//            var actual = _returnSaleManager.GetSale(1, 1, out error);
//            Assert.AreEqual(expected, error.MessageStyle.Message);
//        }

//        [Test]
//        public void GetSaleISSaleFoundISTrueTest()
//        {
//            ErrorMessage error;
//            var expected = 1;
//            bool isSaleFound = true;
//            bool isReturnAble = false;
//            _returnSaleService.Setup(a => a.GetSaleBySaleNumber(It.IsAny<int>(), It.IsAny<int>(),
//                It.IsAny<System.DateTime>(), out isSaleFound, out isReturnAble))
//                .Returns(GetSaleTestData());

//            _returnSaleService.Setup(a => a.GetSaleLineBySaleNumber(It.IsAny<int>(), It.IsAny<int>(),
//                It.IsAny<System.DateTime>(), It.IsAny<string>()))
//                .Returns(GetSaleLineBySaleNumberTestData());

//            _returnSaleManager = new ReturnSaleManager(_returnSaleService.Object, _policyService.Object,
//                  _resourceManager, _saleManager.Object, _reasonService.Object, _saleService.Object, _saleLineManager.Object, _saleHeadManager.Object);

//            var actual = _returnSaleManager.GetSale(1, 1, out error);
//            Assert.AreEqual(expected, actual.Sale_Num);
//        }

//        [Test]
//        public void ReturnSaleTestISUserIsNotAbleToReturnSale()
//        {
//            string userCode = "X";
//            var user = GetUserData(userCode);
//            ErrorMessage error;
//            List<ErrorMessage> saleLineMessage;
//            var expected = "This user can't perform a return._Select an authorized user.~Return";
//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(false);
//            bool isSaleFound = true;
//            bool isReturnAble = false;
//            var actual = _returnSaleManager.ReturnSale(user, 1, 1, 2, false, "PriceChange", "",
//                out error, out saleLineMessage);
//            Assert.AreEqual(expected, error.MessageStyle.Message);
//        }


//        [Test]
//        public void ReturnSaleISSaleFoundISTrueAndIsReturnableFalseTest()
//        {
//            string userCode = "X";
//            var user = GetUserData(userCode);
//            ErrorMessage error;
//            List<ErrorMessage> saleLineMessage;
//            var expected = "Selected sale number not found.~Sale number not found.";
//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            bool isSaleFound = true;
//            bool isReturnAble = false;
//            _returnSaleService.Setup(a => a.GetSaleBySaleNumber(It.IsAny<int>(), It.IsAny<int>(),
//                It.IsAny<System.DateTime>(), out isSaleFound, out isReturnAble))
//                .Returns(GetSaleTestData());

//            _returnSaleService.Setup(a => a.GetSaleLineBySaleNumber(It.IsAny<int>(), It.IsAny<int>(),
//                It.IsAny<System.DateTime>(), It.IsAny<string>()))
//                .Returns(GetSaleLineBySaleNumberTestData());

//            _returnSaleManager = new ReturnSaleManager(_returnSaleService.Object, _policyService.Object,
//                  _resourceManager, _saleManager.Object, _reasonService.Object, _saleService.Object, _saleLineManager.Object, _saleHeadManager.Object);

//            var actual = _returnSaleManager.ReturnSale(user, 1, 1, 2, false, "PriceChange", "",
//                out error, out saleLineMessage);
//            Assert.AreEqual(expected, error.MessageStyle.Message);
//        }


//        [Test]
//        public void ReturnSaleISSaleFoundISTrueAndIsReturnableTrueTest()
//        {
//            string userCode = "X";
//            var user = GetUserData(userCode);
//            ErrorMessage error;
//            List<ErrorMessage> saleLineMessage;
//            var expected = 0;
//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            bool isSaleFound = true;
//            bool isReturnAble = true;
//            _returnSaleService.Setup(a => a.GetSaleBySaleNumber(It.IsAny<int>(), It.IsAny<int>(),
//                It.IsAny<System.DateTime>(), out isSaleFound, out isReturnAble))
//                .Returns(GetSaleTestData());

//            _returnSaleService.Setup(a => a.GetSaleLineBySaleNumber(It.IsAny<int>(), It.IsAny<int>(),
//                It.IsAny<System.DateTime>(), It.IsAny<string>()))
//                .Returns(GetSaleLineBySaleNumberTestData());

//            _returnSaleManager = new ReturnSaleManager(_returnSaleService.Object, _policyService.Object,
//                  _resourceManager, _saleManager.Object, _reasonService.Object, _saleService.Object, _saleLineManager.Object, _saleHeadManager.Object);

//            var actual = _returnSaleManager.ReturnSale(user, 1, 1, 2, false, "PriceChange", "",
//                out error, out saleLineMessage);
//            Assert.AreEqual(expected, actual.Sale_Lines.Count);
//        }

//        [Test]
//        public void ReturnSaleItemsWhenIsSaleFoundFalseTest()
//        {
//            int[] saleLine = { 1, 2 };
//            string userCode = "X";
//            var user = GetUserData(userCode);
//            ErrorMessage error;
//            List<ErrorMessage> saleLineMessage;
//            var expected = "Selected sale number not found.~Sale number not found.";
//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            bool isSaleFound = false;
//            bool isReturnAble = true;
//            _returnSaleService.Setup(a => a.GetSaleBySaleNumber(It.IsAny<int>(), It.IsAny<int>(),
//                It.IsAny<System.DateTime>(), out isSaleFound, out isReturnAble))
//                .Returns(GetSaleTestData());

//            _returnSaleManager = new ReturnSaleManager(_returnSaleService.Object, _policyService.Object,
//                  _resourceManager, _saleManager.Object, _reasonService.Object, _saleService.Object, _saleLineManager.Object, _saleHeadManager.Object);

//            var actual = _returnSaleManager.ReturnSaleItems(user, 1, 1, 2, saleLine, false, "PriceChange"
//                , "", out error, out saleLineMessage);

//            Assert.AreEqual(expected, error.MessageStyle.Message);
//        }

//        [Test]
//        public void ReturnSaleItemsWhenisReturnAbleFalseTest()
//        {
//            int[] saleLine = { 1, 2 };
//            string userCode = "X";
//            var user = GetUserData(userCode);
//            ErrorMessage error;
//            List<ErrorMessage> saleLineMessage;
//            var expected = "Selected sale number not found.~Sale number not found.";
//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            bool isSaleFound = true;
//            bool isReturnAble = false;
//            _returnSaleService.Setup(a => a.GetSaleBySaleNumber(It.IsAny<int>(), It.IsAny<int>(),
//                It.IsAny<System.DateTime>(), out isSaleFound, out isReturnAble))
//                .Returns(GetSaleTestData());


//            _returnSaleManager = new ReturnSaleManager(_returnSaleService.Object, _policyService.Object,
//                  _resourceManager, _saleManager.Object, _reasonService.Object, _saleService.Object, _saleLineManager.Object, _saleHeadManager.Object);

//            var actual = _returnSaleManager.ReturnSaleItems(user, 1, 1, 2, saleLine, false, "PriceChange"
//                , "", out error, out saleLineMessage);

//            Assert.AreEqual(expected, error.MessageStyle.Message);
//        }


//        [Test]
//        public void ReturnSaleItemsIsSaleFoundTrueAndisReturnAbleISAlsoTrueTest()
//        {
//            int[] saleLine = { 1, 2 };
//            string userCode = "X";
//            var user = GetUserData(userCode);
//            ErrorMessage error;
//            List<ErrorMessage> saleLineMessage;
//            var expected = 0;
//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            bool isSaleFound = true;
//            bool isReturnAble = true;
//            _returnSaleService.Setup(a => a.GetSaleBySaleNumber(It.IsAny<int>(), It.IsAny<int>(),
//                It.IsAny<System.DateTime>(), out isSaleFound, out isReturnAble))
//                .Returns(GetSaleTestData());

//            _returnSaleService.Setup(a => a.GetSaleLineBySaleNumber(It.IsAny<int>(), It.IsAny<int>(),
//                It.IsAny<System.DateTime>(), It.IsAny<string>()))
//                .Returns(GetSaleLineBySaleNumberTestData());

//            _returnSaleManager = new ReturnSaleManager(_returnSaleService.Object, _policyService.Object,
//                  _resourceManager, _saleManager.Object, _reasonService.Object, _saleService.Object, _saleLineManager.Object, _saleHeadManager.Object);

//            var actual = _returnSaleManager.ReturnSaleItems(user, 1, 1, 2, saleLine, false, "PriceChange"
//                , "", out error, out saleLineMessage);

//            Assert.AreEqual(expected, actual.Sale_Lines.Count);
//        }
//    }
//}

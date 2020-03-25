using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

using System.Linq;

namespace Infonet.CStoreCommander.WebApi.Tests.Managers
{
    [TestFixture]
    public class StockManagerTest
    {
        private Mock<ITaxService> _taxService = new Mock<ITaxService>();
        private Mock<IStockService> _stockService = new Mock<IStockService>();
        private Mock<IPolicyManager> _policyManager = new Mock<IPolicyManager>();
        private IApiResourceManager _resourceManager = new ApiResourceManager();
        private Mock<ILoginManager> _loginManager = new Mock<ILoginManager>();
        private IStockManager _stockManager;

        [SetUp]
        public void SetUp()
        {
            _stockService.Setup(s => s.AddStockItem(It.IsAny<StockItem>(), It.IsAny<bool>()));
            _stockService.Setup(s => s.AddPluMast(It.IsAny<string>()));
            _stockService.Setup(s => s.AddStockBranch(It.IsAny<string>()));
            _stockService.Setup(s => s.AddStockPrice(It.IsAny<string>(), It.IsAny<decimal>()));
            _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
              _taxService.Object, _resourceManager, _loginManager.Object);
        }

        /// <summary>
        /// Set the taxes data
        /// </summary>
        /// <returns></returns>
        private List<StockTax> GetTaxesData()
        {
            var firstTax = new StockTax
            {
                Name = "Tax",
                Code = "1"
            };

            var secondTax = new StockTax
            {
                Name = "Tax",
                Code = "2"
            };

            return new List<StockTax>
            {
                firstTax,
                secondTax
            };
        }

        /// <summary>
        /// Gets the Pages data
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, string> GetHotButtonPagesData()
        {
            return new Dictionary<int, string>
            {
                {1,"Page 1"},
                {2,"Page 2"}
            };
        }


        /// <summary>
        /// Gets Hot Buttons Data
        /// </summary>
        /// <returns></returns>
        private List<HotButton> GetHotButtonsData()
        {
            return new List<HotButton>
            {
                new HotButton
                {
                    Button_Number = 1,
                    ImageUrl = "",
                    DefaultQuantity = 1,
                    StockCode = "123",
                    Button_Product = "Product1"
                },
                new HotButton
                {
                    Button_Number = 2,
                    ImageUrl = "",
                    DefaultQuantity = 1,
                    StockCode = "1234",
                    Button_Product = "Product2"
                }
            };
        }

        /// <summary>
        /// Get the stock items data
        /// </summary>
        /// <returns></returns>
        private List<StockItem> GetStockItemsData()
        {
            var firstStockItem = new StockItem
            {
                StockCode = "101",
                Description = "First Product"
            };
            var secondStockItem = new StockItem
            {
                StockCode = "102",
                Description = "Second Product"
            };
            return new List<StockItem>
            {
                firstStockItem,
                secondStockItem
            };
        }

        /// <summary>
        /// Get the active stock item data
        /// </summary>
        /// <returns></returns>
        private List<StockItem> GetActiveStockItemsData()
        {
            var firstStockItem = new StockItem
            {
                StockCode = "1",
                Description = "First Active Product"
            };
            return new List<StockItem>
            {
                firstStockItem
            };
        }

        /// <summary>
        /// Get stock item by code serice setup
        /// </summary>
        /// <param name="stockCode">Stock code</param>
        /// <returns></returns>
        private StockItem GetStockItem(string stockCode)
        {
            return GetStockItemsData().FirstOrDefault(s => s.StockCode == stockCode);
        }

        /// <summary>
        /// Get no stock item
        /// </summary>
        /// <returns></returns>
        private StockItem NoStockData()
        {
            return null;
        }

        /// <summary>
        /// Get no plu mast
        /// </summary>
        /// <returns></returns>
        private PLUMast NoPLUData()
        {
            return null;
        }

        /// <summary>
        /// Get stock items by search term setup
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        private List<StockItem> SearchStockItems(string searchTerm)
        {
            var items = GetStockItemsData();
            return items.Where(i => i.Description.Contains(searchTerm)
                              || i.StockCode.Contains(searchTerm)).ToList();
        }

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
        /// set the get user test data 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private User GetUserData(string code)
        {
            return GetUsers().FirstOrDefault(x => x.Code == code);
        }

        ///// <summary>
        ///// Test method to add stock item with  invalid user
        ///// </summary>
        //[Test]
        //public void AddStockItemWithInvalidUserTest()
        //{
        //    _loginManager.Setup(l => l.GetUser(It.IsAny<string>()))
        //        .Returns((string code) => GetUserData(code));
        //    _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
        //        _taxService.Object,_resourceManager,_loginManager.Object);
        //    var userCode = "Test user";
        //    var stockItem = new StockItem
        //    {
        //        StockCode = "TestStock",
        //        Description = "Test Stock Item",
        //        Price = (decimal)1.00,
        //    };
        //    var taxes = new List<string>
        //    {
        //        "Tax-1",
        //        "Tax-2"
        //    };
        //    ErrorMessage error;
        //    var expected = "UserID Test user does not exist. ~No Such User";
        //    _stockManager.AddStockItem(userCode, stockItem, taxes, out error);
        //    Assert.AreEqual(expected, error.MessageStyle.Message);
        //    Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, error.StatusCode);
        //    Assert.IsFalse(error.ShutDownPos);
        //}

        ///// <summary>
        ///// Test method to add stock item if user cannot add stock
        ///// </summary>
        //[Test]
        //public void AddStockItemIfUserCannotAddStockTest()
        //{
        //    _loginManager.Setup(l => l.GetUser(It.IsAny<string>()))
        //        .Returns((string code) => GetUserData(code));
        //    _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
        //        _taxService.Object, _resourceManager, _loginManager.Object);
        //    var userCode = "X";
        //    var stockItem = new StockItem
        //    {
        //        StockCode = "TestStock",
        //        Description = "Test Stock Item",
        //        Price = (decimal)1.00,
        //    };
        //    var taxes = new List<string>
        //    {
        //        "Tax-1",
        //        "Tax-2"
        //    };
        //    ErrorMessage error;
        //    _stockManager.AddStockItem(userCode, stockItem, taxes, out error);
        //    Assert.AreEqual(System.Net.HttpStatusCode.Conflict, error.StatusCode);
        //    Assert.IsFalse(error.ShutDownPos);
        //}

        ///// <summary>
        ///// Test method to add stock with empty stock code
        ///// </summary>
        //[Test]
        //public void AddStockItemForEmptyStockCodeTest()
        //{
        //    _loginManager.Setup(l => l.GetUser(It.IsAny<string>()))
        //        .Returns((string code) => GetUserData(code));
        //    _policyManager.Setup(p => p.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
        //    _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
        //          _taxService.Object, _resourceManager, _loginManager.Object);
        //    var userCode = "X";
        //    var stockItem = new StockItem
        //    {
        //        StockCode = string.Empty,
        //        Description = "Test Stock Item",
        //        Price = (decimal)1.00,
        //    };
        //    var taxes = new List<string>
        //    {
        //        "Tax-1",
        //        "Tax-2"
        //    };
        //    ErrorMessage error;
        //    var expected = "Stock code is required";
        //    _stockManager.AddStockItem(userCode, stockItem, taxes, out error);
        //    Assert.AreEqual(expected, error.MessageStyle.Message);
        //    Assert.AreEqual(System.Net.HttpStatusCode.NotAcceptable, error.StatusCode);
        //    Assert.IsFalse(error.ShutDownPos);
        //}

        ///// <summary>
        ///// Test method to add stock without any stock code
        ///// </summary>
        //[Test]
        //public void AddStockItemForNullStockCodeTest()
        //{
        //    _loginManager.Setup(l => l.GetUser(It.IsAny<string>()))
        //        .Returns((string code) => GetUserData(code));
        //    _policyManager.Setup(p => p.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
        //    _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
        //      _taxService.Object, _resourceManager, _loginManager.Object);
        //    var userCode = "X";
        //    var stockItem = new StockItem
        //    {
        //        Description = "Test Stock Item",
        //        Price = (decimal)1.00,
        //    };
        //    var taxes = new List<string>
        //    {
        //        "Tax-1",
        //        "Tax-2"
        //    };
        //    ErrorMessage error;
        //    var expected = "Stock code is required";
        //    _stockManager.AddStockItem(userCode, stockItem, taxes, out error);
        //    Assert.AreEqual(expected, error.MessageStyle.Message);
        //    Assert.AreEqual(System.Net.HttpStatusCode.NotAcceptable, error.StatusCode);
        //    Assert.IsFalse(error.ShutDownPos);
        //}

        ///// <summary>
        ///// Test method to add stock with empty description
        ///// </summary>
        //[Test]
        //public void AddStockItemForEmptyDescriptionTest()
        //{
        //    _loginManager.Setup(l => l.GetUser(It.IsAny<string>()))
        //        .Returns((string code) => GetUserData(code));
        //    _policyManager.Setup(p => p.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
        //    _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
        //        _taxService.Object, _resourceManager, _loginManager.Object);
        //    var userCode = "X";
        //    var stockItem = new StockItem
        //    {
        //        StockCode = "TestStock",
        //        Description = string.Empty,
        //        Price = (decimal)1.00,
        //    };
        //    var taxes = new List<string>
        //    {
        //        "Tax-1",
        //        "Tax-2"
        //    };
        //    ErrorMessage error;
        //    var expected = "Description is required";
        //    _stockManager.AddStockItem(userCode, stockItem, taxes, out error);
        //    Assert.AreEqual(expected, error.MessageStyle.Message);
        //    Assert.AreEqual(System.Net.HttpStatusCode.NotAcceptable, error.StatusCode);
        //    Assert.IsFalse(error.ShutDownPos);
        //}

        ///// <summary>
        ///// Test method to add stock without description
        ///// </summary>
        //[Test]
        //public void AddStockItemForNullDescriptionTest()
        //{
        //    _loginManager.Setup(l => l.GetUser(It.IsAny<string>()))
        //       .Returns((string code) => GetUserData(code));
        //    _policyManager.Setup(p => p.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
        //    _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
        //      _taxService.Object, _resourceManager, _loginManager.Object);
        //    var userCode = "X";
        //    var stockItem = new StockItem
        //    {
        //        StockCode = "TestStock",
        //        Price = (decimal)1.00,
        //    };
        //    var taxes = new List<string>
        //    {
        //        "Tax-1",
        //        "Tax-2"
        //    };
        //    ErrorMessage error;
        //    var expected = "Description is required";
        //    _stockManager.AddStockItem(userCode, stockItem, taxes, out error);
        //    Assert.AreEqual(expected, error.MessageStyle.Message);
        //    Assert.AreEqual(System.Net.HttpStatusCode.NotAcceptable, error.StatusCode);
        //    Assert.IsFalse(error.ShutDownPos);
        //}

        ///// <summary>
        ///// Test method to add stock with price less than 0
        ///// </summary>
        //[Test]
        //public void AddStockItemInvalidPriceTest()
        //{
        //    _loginManager.Setup(l => l.GetUser(It.IsAny<string>()))
        //       .Returns((string code) => GetUserData(code));
        //    _policyManager.Setup(p => p.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
        //    _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
        //        _taxService.Object, _resourceManager, _loginManager.Object);
        //    var userCode = "X";
        //    var stockItem = new StockItem
        //    {
        //        StockCode = "TestStock",
        //        Description = "Test Stock Item",
        //        Price = (decimal)-1.00,
        //    };
        //    var taxes = new List<string>
        //    {
        //        "Tax-1",
        //        "Tax-2"
        //    };
        //    ErrorMessage error;
        //    var expected = "Price should be more than 0";
        //    _stockManager.AddStockItem(userCode, stockItem, taxes, out error);
        //    Assert.AreEqual(expected, error.MessageStyle.Message);
        //    Assert.AreEqual(System.Net.HttpStatusCode.NotAcceptable, error.StatusCode);
        //    Assert.IsFalse(error.ShutDownPos);
        //}

        ///// <summary>
        ///// Test method to add stock when taxes are available for selected taxes
        ///// </summary>
        //[Test]
        //public void AddStockItemWithExistingTaxForStockCodeTest()
        //{
        //    _loginManager.Setup(l => l.GetUser(It.IsAny<string>()))
        //        .Returns((string code) => GetUserData(code));
        //    _policyManager.Setup(p => p.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
        //    _taxService.Setup(t => t.GetAllActiveTaxes()).Returns(GetTaxesData());
        //    _taxService.Setup(t => t.SetupTaxes(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>()))
        //        .Returns(false);
        //    _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
        //       _taxService.Object, _resourceManager, _loginManager.Object);
        //    var userCode = "X";
        //    var stockItem = new StockItem
        //    {
        //        StockCode = "TestStock",
        //        Description = "Test Stock Item",
        //        Price = (decimal)1.00,
        //    };
        //    var taxes = new List<string>
        //    {
        //        "Tax-1",
        //        "Tax-2"
        //    };
        //    ErrorMessage error;
        //    var expected = "Stock Code TestStock can't have duplicate entry for the same tax name";
        //    _stockManager.AddStockItem(userCode, stockItem, taxes, out error);
        //    Assert.AreEqual(expected, error.MessageStyle.Message);
        //    Assert.AreEqual(System.Net.HttpStatusCode.Conflict, error.StatusCode);
        //    Assert.IsFalse(error.ShutDownPos);
        //}

        ///// <summary>
        ///// Test method to add stock item if stock code already exists
        ///// </summary>
        //[Test]
        //public void AddStockItemIfStockExistsTest()
        //{
        //    _loginManager.Setup(l => l.GetUser(It.IsAny<string>()))
        //      .Returns((string code) => GetUserData(code));
        //    _policyManager.Setup(p => p.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
        //    _taxService.Setup(t => t.GetAllActiveTaxes()).Returns(GetTaxesData());
        //    _taxService.Setup(t => t.SetupTaxes(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>()))
        //        .Returns(true);
        //    _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
        //       _taxService.Object, _resourceManager, _loginManager.Object);
        //    var userCode = "X";
        //    var stockItem = new StockItem
        //    {
        //        StockCode = "TestStock",
        //        Description = "Test Stock Item",
        //        Price = (decimal)1.00,
        //    };
        //    var taxes = new List<string>
        //    {
        //        "Tax-1",
        //        "Tax-2"
        //    };
        //    ErrorMessage error;
        //    var expected = "Stock or Alternate Code TestStock already exists.~Input error.";
        //    _stockManager.AddStockItem(userCode, stockItem, taxes, out error);
        //    Assert.AreEqual(expected, error.MessageStyle.Message);
        //    Assert.AreEqual(System.Net.HttpStatusCode.Conflict, error.StatusCode);
        //    Assert.IsFalse(error.ShutDownPos);
        //}

        ///// <summary>
        ///// Test method to add stock if plu code already exists
        ///// </summary>
        //[Test]
        //public void AddStockItemIfPLUExistsTest()
        //{
        //    _loginManager.Setup(l => l.GetUser(It.IsAny<string>()))
        //       .Returns((string code) => GetUserData(code));
        //    _policyManager.Setup(p => p.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
        //    _taxService.Setup(t => t.GetAllActiveTaxes()).Returns(GetTaxesData());
        //    _taxService.Setup(t => t.SetupTaxes(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>()))
        //        .Returns(true);
        //    _stockService.Setup(t => t.GetPluMast(It.IsAny<string>()))
        //       .Returns(new PLUMast());
        //    _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
        //        _taxService.Object, _resourceManager, _loginManager.Object);
        //    var userCode = "X";
        //    var stockItem = new StockItem
        //    {
        //        StockCode = "TestStock",
        //        Description = "Test Stock Item",
        //        Price = (decimal)1.00,
        //    };
        //    var taxes = new List<string>
        //    {
        //        "Tax-1",
        //        "Tax-2"
        //    };
        //    ErrorMessage error;
        //    var expected = "Stock or Alternate Code TestStock already exists.~Input error.";
        //    _stockManager.AddStockItem(userCode, stockItem, taxes, out error);
        //    Assert.AreEqual(expected, error.MessageStyle.Message);
        //    Assert.AreEqual(System.Net.HttpStatusCode.Conflict, error.StatusCode);
        //    Assert.IsFalse(error.ShutDownPos);
        //}

        ///// <summary>
        ///// Test method to add stock item
        ///// </summary>
        //[Test]
        //public void AddStockItemTest()
        //{
        //    _loginManager.Setup(l => l.GetUser(It.IsAny<string>()))
        //       .Returns((string code) => GetUserData(code));
        //    _policyManager.Setup(p => p.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
        //    _taxService.Setup(t => t.GetAllActiveTaxes()).Returns(GetTaxesData());
        //    _taxService.Setup(t => t.SetupTaxes(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>()))
        //        .Returns(true);
        //     _stockService.Setup(s => s.GetStockItemByCode(It.IsAny<string>(), It.IsAny<bool>())).Returns(NoStockData());
        //    _stockService.Setup(s => s.GetPluMast(It.IsAny<string>())).Returns(NoPLUData());
        //    _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
        //         _taxService.Object, _resourceManager, _loginManager.Object);
        //    var userCode = "X";
        //    var stockItem = new StockItem
        //    {
        //        StockCode = "TestStock",
        //        Description = "Test Stock Item",
        //        Price = (decimal)1.00,
        //    };
        //    var taxes = new List<string>
        //    {
        //        "Tax-1",
        //        "Tax-2"
        //    };
        //    ErrorMessage error;
        //    _stockManager.AddStockItem(userCode, stockItem, taxes, out error);
        //    Assert.IsNull(error.MessageStyle.Message);
        //}

        /// <summary>
        /// Test method to get stock item by code
        /// </summary>
        [Test]
        public void GetStockByCodeTest()
        {
            _policyManager.Setup(p => p.Sell_Inactive).Returns(true);
            _stockService.Setup(s => s.GetStockItemByCode(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((string code, bool sellInactive) => { return GetStockItem(code); });
            _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
               _taxService.Object, _resourceManager, _loginManager.Object);
            var expected = "First Product";
            var stockCode = "101";
            ErrorMessage error;
            var actual = _stockManager.GetStockByCode(stockCode, out error);
            Assert.AreEqual(expected, actual.Description);
            Assert.IsNull(error.MessageStyle.Message);
        }

        ///// <summary>
        ///// Test method to get stock item by code if stock does not exists
        ///// </summary>
        //[Test]
        //public void GetStockByCodeIfNotExistsTest()
        //{
        //    _policyManager.Setup(p => p.Sell_Inactive).Returns(true);
        //    _stockService.Setup(s => s.GetStockItemByCode(It.IsAny<string>(), It.IsAny<bool>()))
        //        .Returns((string code, bool sellInactive) => { return GetStockItem(code); });
        //    _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
        //       _taxService.Object, _resourceManager, _loginManager.Object);
        //    var expected = "Stock Code 99  does not exist.~Stock Item Not Found";
        //    var stockCode = "99";
        //    ErrorMessage error;
        //    var actual = _stockManager.GetStockByCode(stockCode, out error);
        //    Assert.AreEqual(expected, error.MessageStyle.Message);
        //    Assert.AreEqual(System.Net.HttpStatusCode.NotFound, error.StatusCode);
        //    Assert.IsFalse(error.ShutDownPos);
        //}

        /// <summary>
        /// Test method to get stock items
        /// </summary>
        [Test]
        public void GetStockItemsTest()
        {
            _policyManager.Setup(p => p.Sell_Inactive).Returns(true);
            _stockService.Setup(s => s.GetStockItems(It.IsAny<int>(), It.IsAny<int>()))
              .Returns(GetStockItemsData());
            _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
               _taxService.Object, _resourceManager, _loginManager.Object);
            var expected = 2;
            var actual = _stockManager.GetStockItems();
            Assert.AreEqual(expected, actual.Count);
        }

        /// <summary>
        /// Test method to get active stock items
        /// </summary>
        [Test]
        public void GetActiveStockItemsTest()
        {
            _policyManager.Setup(p => p.Sell_Inactive).Returns(false);
            _stockService.Setup(s => s.GetActiveStockItems(It.IsAny<int>(), It.IsAny<int>()))
              .Returns(GetActiveStockItemsData());
            _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
              _taxService.Object, _resourceManager, _loginManager.Object);
            var expected = 1;
            var actual = _stockManager.GetStockItems();
            Assert.AreEqual(expected, actual.Count);
        }

        /// <summary>
        /// Test method to search stock item
        /// </summary>
        [Test]
        public void SearchStockItemsTest()
        {
            _policyManager.Setup(p => p.Sell_Inactive).Returns(false);
            _stockService.Setup(s => s.SearchStock(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
              .Returns((string searchTerm, bool sellInactive, int pageIndex, int pageSize) => { return SearchStockItems(searchTerm); });
            _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
               _taxService.Object, _resourceManager, _loginManager.Object);
            var expected = 1;
            var search = "First";
            var actual = _stockManager.SearchStockItems(search);
            Assert.AreEqual(expected, actual.Count);
        }

        /// <summary>
        /// Test method to search stock if no item found
        /// </summary>
        [Test]
        public void SearchStockItemsIfNoItemFoundTest()
        {
            _policyManager.Setup(p => p.Sell_Inactive).Returns(true);
            _stockService.Setup(s => s.SearchStock(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
              .Returns((string searchTerm, bool sellInactive, int pageIndex, int pageSize) => { return SearchStockItems(searchTerm); });
            _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
                _taxService.Object, _resourceManager, _loginManager.Object);
            var expected = 0;
            var search = "Test";
            var actual = _stockManager.SearchStockItems(search);
            Assert.AreEqual(expected, actual.Count);
        }


        /// <summary>
        /// Test method to get hot button pages
        /// </summary>
        [Test]
        public void GetHotButtonPagesTest()
        {
            _stockService.Setup(s => s.GetHotButonPages()).Returns(GetHotButtonPagesData());
            _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
               _taxService.Object, _resourceManager, _loginManager.Object);
            var expected = 2;
            var actual = _stockManager.GetHotButonPages();
            Assert.AreEqual(expected, actual.Count);
        }

        /// <summary>
        /// Test method to get stock items
        /// </summary>
        [Test]
        public void GetHotButtonsTest()
        {
            _stockService.Setup(s => s.GetHotButtons(It.IsAny<int>(), It.IsAny<int>())).Returns(GetHotButtonsData());
            _stockManager = new StockManager(_stockService.Object, _policyManager.Object,
               _taxService.Object, _resourceManager, _loginManager.Object);
            var expected = 2;
            int pageId = 1;
            var actual = _stockManager.GetHotButons(pageId);
            Assert.AreEqual(expected, actual.Count);
        }
    }
}

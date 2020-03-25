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
//    public class SuspendSaleManagerTest
//    {
//        private Mock<IPolicyManager> _policyManager = new Mock<IPolicyManager>();
//        private Mock<ISuspendedSaleService> _suspendedSaleService = new Mock<ISuspendedSaleService>();
//        private IApiResourceManager _resourceManager = new ApiResourceManager();
//        private Mock<ISaleHeadManager> _mocksaleHeadManager = new Mock<ISaleHeadManager>();
//        private Mock<ISaleService> _saleService = new Mock<ISaleService>();
//        private Mock<ISaleManager> _mocksaleManager = new Mock<ISaleManager>();
//        private Mock<IUserService> _userService = new Mock<IUserService>();
//        private Mock<IPolicyService> _policyService = new Mock<IPolicyService>();
//        private Mock<ISaleLineManager> _saleLineManager = new Mock<ISaleLineManager>();
//        private Mock<IStockService> _stockService = new Mock<IStockService>();
//        private Mock<ICustomerManager> _customerManager = new Mock<ICustomerManager>();
//        private Mock<IStockManager> _stockManager = new Mock<IStockManager>();
//        private Mock<IReasonService> _reasonService = new Mock<IReasonService>();
//        private Mock<ILoginManager> _mockloginManager = new Mock<ILoginManager>();
//        private Mock<ILoginService> _loginService = new Mock<ILoginService>();
//        private Mock<ICustomerService> _customerService = new Mock<ICustomerService>();
//        private Mock<IUtilityService> _utilityService = new Mock<IUtilityService>();
//        private Mock<ITillService> _tillService = new Mock<ITillService>();
//        private Mock<IShiftService> _shiftService = new Mock<IShiftService>();
//        private Mock<IPromoManager> _promoManager = new Mock<IPromoManager>();
//        private Mock<IFuelService> _fuelService = new Mock<IFuelService>();
//        private Mock<ICardService> _cardService = new Mock<ICardService>();
//        private Mock<ITaxService> _taxService = new Mock<ITaxService>();
//        private Mock<IGivexClientManager> _givexManager = new Mock<IGivexClientManager>();
//        private Mock<IPaymentManager> _paymentManager = new Mock<IPaymentManager>();
//        private Mock<ICreditCardManager> _creditCardManager = new Mock<ICreditCardManager>();

//        private ISuspendedSaleManger _suspendSaleManager;
//        private SaleHeadManager _saleHeadManager;
//        private SaleManager _saleManager;
//        private ILoginManager _loginManager;

//        /// <summary>
//        /// Set Up the Data For Get All Suspended Sale 
//        /// </summary>
//        /// <returns></returns>
//        private List<SusHead> GetAllSuspendedSale(string query)
//        {
//            var firstSuspendedSale = new SusHead()
//            {
//                SaleNumber = 1,
//                Client = "X",
//                TillNumber = 1
//            };

//            var secondSuspendedSale = new SusHead()
//            {
//                SaleNumber = 2,
//                Client = "Y",
//                TillNumber = 2
//            };

//            var suspendedSale = new List<SusHead>()
//            {
//                firstSuspendedSale,
//                secondSuspendedSale
//            };

//            return suspendedSale;
//        }
//        /// <summary>
//        /// Get Sale From DB temp By SaleNumber and tillNumber
//        /// </summary>
//        /// <param name="salenumber"></param>
//        /// <param name="tillNumber"></param>
//        /// <returns></returns>
//        private Sale GetSaleBySaleNumberFromDbTemp(int salenumber, int tillNumber)
//        {
//            var sale = new Sale
//            {
//                Sale_Num = salenumber,
//                TillNumber = (byte)tillNumber,
//                Customer = new Customer
//                {
//                    Code = "X"
//                },
//                Sale_Type = "Suspend Sale ",
//                Void_Num = 0,
//                Return_Reason = new Return_Reason
//                {
//                    Reason = "Damaged Product"
//                },
//                Upsell = false,
//                TreatyName = "X",
//                TreatyNumber = "12345",
//                Sale_Totals = new Sale_Totals
//                {
//                    Invoice_Discount_Type = "R",
//                    Discount_Percent = 3,
//                    Invoice_Discount = 2
//                }
//            };
//            return sale;
//        }

//        /// <summary>
//        /// Get Sale From DB temp By SaleNumber and tillNumber
//        /// </summary>
//        /// <param name="salenumber"></param>
//        /// <param name="tillNumber"></param>
//        /// <returns></returns>
//        private Sale GetSaleFromDbTemp(int tillNumber)
//        {
//            var sale = new Sale
//            {
//                Sale_Num = 1,
//                TillNumber = (byte)tillNumber,
//                Customer = new Customer
//                {
//                    Code = "Test user"
//                },
//                Sale_Type = "Suspend Sale ",
//                Void_Num = 0,

//                Return_Reason = new Return_Reason
//                {
//                    Reason = "Damaged Product"
//                },
//                Upsell = false,
//                TreatyName = "X",
//                TreatyNumber = "12345",


//                Sale_Totals = new Sale_Totals
//                {
//                    Invoice_Discount_Type = "R",
//                    Discount_Percent = 3,
//                    Invoice_Discount = 2
//                }
//            };
//            return sale;
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
//                Name = "Test User",
//                Code = "Test User",
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
//        /// Get  plu mast
//        /// </summary>
//        /// <returns></returns>
//        private PLUMast GetPLUCode()
//        {
//            var pluMast = new PLUMast
//            {
//                PLUCode = "Test",
//                PLUPrim = "",
//                PLUType = 'R'
//            };
//            return pluMast;
//        }

//        private List<Customer> GetCustomers()
//        {
//            var firstcustomer = new Customer
//            {
//                Code = "Test User",
//                Name = "Cash Sale"
//            };
//            var secondcustomer = new Customer
//            {
//                Code = "X",
//                Name = "B"
//            };

//            var customers = new List<Customer>()
//            {
//                firstcustomer,
//                secondcustomer
//            };
//            return customers;
//        }

//        /// <summary>
//        /// Get Sale Totals 
//        /// </summary>
//        private Sale_Totals GetSaleTotals(int saleNumber)
//        {
//            var firsttotals = new Sale_Totals
//            {
//                Total = 100,
//                TotalLabel = "100" + "HST" + "12 $ ",
//                SaleNumber = saleNumber
//            };

//            //var secondtotals = new Sale_Totals
//            //{
//            //    Total = 200,
//            //    TotalLabel = "200" + "HST" + "12 $ ",
//            //    SaleNumber = saleNumber
//            //};

//            return firsttotals;
//        }

//        /// <summary>
//        /// Load All Taxes 
//        /// </summary>
//        private List<TaxMast> GetAllTaxMast()
//        {
//            var FirsttaxMast = new TaxMast()
//            {
//                TaxOrd = 2,
//                Active = true,
//                TaxApply = null,
//                TaxDefination = "1",
//                TaxDescription = "Goods and Services Tax",
//                TaxName = "GST"
//            };
//            var secondtaxMast = new TaxMast()
//            {
//                TaxOrd = 3,
//                Active = true,
//                TaxApply = null,
//                TaxDefination = "1",
//                TaxDescription = "Provincial Sales Tax",
//                TaxName = "PST"
//            };

//            var ListtaxMast = new List<TaxMast>
//            {
//                FirsttaxMast,
//                secondtaxMast
//            };
//            return ListtaxMast;
//        }

//        private List<Sale_Line> GetSaleLinesFromDbtemp(int saleNumber, int tillNumber, string userCode)
//        {
//            var FirstsaleLines = new Sale_Line
//            {
//                PLU_Code = "Test",
//                Line_Num = 1,
//                User = userCode,
//                Till_Num = (byte)tillNumber,
//                Sale_Num = saleNumber.ToString(),
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
//                User = userCode,
//                Till_Num = (byte)tillNumber,
//                Sale_Num = saleNumber.ToString(),
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
//        /// Get Tax Rate by Tax Name  
//        /// </summary>
//        /// 
//        private List<TaxRate> GetTaxRateByTaxName(string taxName)
//        {
//            var taxRate1 = new TaxRate
//            {

//                TaxName = taxName,
//                TaxCode = "I",
//                TaxDescription = "Included GST",
//                Rebate = 0,
//                Rate = 5,
//                Included = true
//            };

//            var taxRate2 = new TaxRate
//            {

//                TaxName = taxName,
//                TaxCode = "A",
//                TaxDescription = "Added PST",
//                Rebate = 0,
//                Rate = 10,
//                Included = false
//            };

//            var taxRateList = new List<TaxRate>
//            {
//                taxRate1,
//                taxRate2
//            };

//            var taxRate = taxRateList.FirstOrDefault(x => x.TaxName == taxName);
//            return taxRateList;
//        }

//        /// <summary>
//        /// Sale Setup 
//        /// </summary>
//        /// 

//        /// <summary>
//        ///Set Suspended Sale 
//        /// </summary>
//        /// <param name="salenumber"></param>
//        /// <param name="tillNumber"></param>
//        /// <returns></returns>
//        private Sale GetSuspendedSale()
//        {
//            var sale = new Sale
//            {
//                Sale_Num = 2,
//                TillNumber = (byte)1,
//                Customer = new Customer
//                {
//                    Code = "X"
//                },
//                Sale_Type = "Suspend Sale ",
//                Void_Num = 0,
//                Return_Reason = new Return_Reason
//                {
//                    Reason = "Damaged Product"
//                },
//                Upsell = false,
//                TreatyName = "X",
//                TreatyNumber = "12345",
//                Sale_Totals = new Sale_Totals
//                {
//                    Invoice_Discount_Type = "R",
//                    Discount_Percent = 3,
//                    Invoice_Discount = 2
//                }
//            };
//            return sale;
//        }

//        [Test]
//        public void GetAllSuspendedSaleWhenSuspendedSalePresentANdShareSuspendedSalePolicy()
//        {
//            var expected = 2;
//            int tillNumber = 1;
//            ErrorMessage error;
//            _policyManager.Setup(u => u.SHARE_SUSP).Returns(true);
//            _suspendedSaleService.Setup(u => u.GetAllSuspendedSale(It.IsAny<string>()))
//               .Returns((string sqlQuery) => { return GetAllSuspendedSale(sqlQuery); });
//            _suspendSaleManager = new SuspendedSaleManager(_policyManager.Object, _suspendedSaleService.Object, _resourceManager,
//                 _mocksaleHeadManager.Object, _mocksaleManager.Object, _saleLineManager.Object,
//                 _userService.Object, _policyService.Object, _customerManager.Object, _reasonService.Object, _paymentManager.Object);
//            var actual = _suspendSaleManager.GetSuspendedSale(tillNumber, out error).Count;
//            Assert.AreEqual(expected, actual);

//        }

//        [Test]
//        public void GetAllSuspendedSaleWhenSuspendedSalePresentANdShareSuspendedSalePolicyFalse()
//        {
//            var expected = 1;
//            int tillNumber = 1;
//            ErrorMessage error;
//            _policyManager.Setup(u => u.SHARE_SUSP).Returns(false);
//            _suspendedSaleService.Setup(u => u.GetAllSuspendedSale(It.IsAny<string>()))
//                .Returns((string sqlQuery) => { return GetAllSuspendedSale(sqlQuery); });
//            _suspendSaleManager = new SuspendedSaleManager(_policyManager.Object, _suspendedSaleService.Object, _resourceManager,
//                  _mocksaleHeadManager.Object, _mocksaleManager.Object, _saleLineManager.Object, _userService.Object, _policyService.Object, _customerManager.Object, _reasonService.Object, _paymentManager.Object);
//            var actual = _suspendSaleManager.GetSuspendedSale(tillNumber, out error).Where(x => x.TillNumber == 1).ToList();
//            Assert.AreEqual(expected, actual.Count);
//        }

//        [Test]
//        public void GetAllSuspendedSaleWhenNoSuspendedSalePresent()
//        {
//            var expected = "There are no suspended transactions.~Unsuspend a Sale.";
//            int tillNumber = 1;
//            ErrorMessage error;
//            _policyManager.Setup(u => u.SHARE_SUSP).Returns(false);
//            _suspendedSaleService.Setup(u => u.GetAllSuspendedSale(It.IsAny<string>()))
//                .Returns((string sqlQuery) => { return null; });
//            _suspendSaleManager = new SuspendedSaleManager(_policyManager.Object, _suspendedSaleService.Object, _resourceManager,
//                 _mocksaleHeadManager.Object, _mocksaleManager.Object, _saleLineManager.Object, _userService.Object, _policyService.Object, _customerManager.Object, _reasonService.Object, _paymentManager.Object);

//            var actual = _suspendSaleManager.GetSuspendedSale(tillNumber, out error);
//            Assert.AreEqual(expected, error.MessageStyle.Message);
//        }

//        [Test]
//        public void SuspendSaleWhenThereISNoSuspendedSalePresent()
//        {
//            var expected = "Please select a valid Sale to Suspend";
//            int tillNumber = 1;
//            int saleNumber = 1;
//            var userCode = "Test user";
//            ErrorMessage error = new ErrorMessage();
//            error.MessageStyle.Message = "Please select a valid Sale to Suspend";
//            // _policyManager.Setup(u => u.SHARE_SUSP).Returns(false);
//            _policyService.Setup(u => u.GetPol("U_SUSP", null)).Returns(true);
//            _userService.Setup(u => u.GetUser(It.IsAny<string>()))
//                .Returns((string code) => { return GetUserData(code); });
//            _mocksaleManager.Setup(u => u.GetCurrentSale(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<string>(), out error))
//                .Returns(() => { return null; });

//            //  .Returns((int till, int saleNumber) => { return GetSaleBySaleNumberFromDbTemp(saleNumber, till); });
//            _suspendSaleManager = new SuspendedSaleManager(_policyManager.Object, _suspendedSaleService.Object, _resourceManager,
//                 _mocksaleHeadManager.Object, _mocksaleManager.Object, _saleLineManager.Object, _userService.Object, _policyService.Object, _customerManager.Object, _reasonService.Object, _paymentManager.Object);
//            var actual = _suspendSaleManager.SuspendSale(tillNumber, saleNumber, userCode, out error);
//            Assert.AreEqual(expected, error.MessageStyle.Message);

//        }

//        [Test]
//        public void SuspendSaleWhenUserIsNotAuthorizeToSale()
//        {
//            var expected = "You are not authorised to Suspend or Unsuspend the Sale";
//            int tillNumber = 1;
//            int saleNumber = 1;
//            var userCode = "Test user";
//            ErrorMessage error;
//            // _policyManager.Setup(u => u.SHARE_SUSP).Returns(false);
//            _policyService.Setup(u => u.GetPol("U_SUSP", null)).Returns(false);
//            _userService.Setup(u => u.GetUser(It.IsAny<string>()))
//                .Returns((string code) => { return GetUserData(code); });
//            //_saleService.Setup(u => u.GetSaleBySaleNoFromDbTemp(It.IsAny<int>(), It.IsAny<int>()))
//            //    .Returns(() => { return null; });
//            _saleService.Setup(u => u.GetSale(It.IsAny<int>(), It.IsAny<int>()))
//              .Returns((int till, int sale_number) => { return GetSaleBySaleNumberFromDbTemp(sale_number, till); });
//            _suspendSaleManager = new SuspendedSaleManager(_policyManager.Object, _suspendedSaleService.Object, _resourceManager,
//                 _mocksaleHeadManager.Object, _mocksaleManager.Object, _saleLineManager.Object, _userService.Object, _policyService.Object, _customerManager.Object, _reasonService.Object, _paymentManager.Object);
//            var actual = _suspendSaleManager.SuspendSale(tillNumber, saleNumber, userCode, out error);
//            Assert.AreEqual(expected, error.MessageStyle.Message);

//        }

//        [Test]
//        public void SuspendSaleAndCreateSaleWhenSaleNumberCantReadFromDataBase()
//        {
//            int tillNumber = 1;
//            var expected = "Sale number can not be read from database ~ Error";
//            int saleNumber = 1;
//            var userCode = "Test User";
//            var taxName = "PST";
//            ErrorMessage error;
//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//           .Returns((int till, int sale_number, string code) => { return GetSaleLinesFromDbtemp(sale_number, till, code); });

//            _mockloginManager.Setup(u => u.GetUser(It.IsAny<string>()))
//                .Returns((string code) => { return GetUserData(code); });

//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) => { return GetCustomers().FirstOrDefault(a => a.Code == code); });
//            _stockService.Setup(u => u.GetPluMast(It.IsAny<string>()))
//                .Returns((string stockCode) => { return GetPLUCode(); });
//            //_saleService.Setup(u => u.(It.IsAny<int>()))
//            //  .Returns((int sale) => { return GetSaleTotals(sale); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });

//            // _saleService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });

//            // SaleSetUp();
//            _saleService.Setup(u => u.GetSale(It.IsAny<int>(), It.IsAny<int>()))
//            .Returns((int till, int sale_number) => { return GetSaleBySaleNumberFromDbTemp(sale_number, till); });
//            _policyService.Setup(u => u.GetPol("U_SUSP", null)).Returns(true);
//            _saleHeadManager = new SaleHeadManager(_saleService.Object, _customerManager.Object, _policyManager.Object);

//            _saleManager = new SaleManager(_policyManager.Object, _saleService.Object, _resourceManager, _mockloginManager.Object, _loginService.Object,
//                _stockService.Object, _policyService.Object, _utilityService.Object, _tillService.Object, _customerService.Object,
//                _cardService.Object, _taxService.Object, _saleLineManager.Object, _saleHeadManager, _customerManager.Object,
//                _reasonService.Object, _givexManager.Object, _creditCardManager.Object);


//            _suspendSaleManager = new SuspendedSaleManager(_policyManager.Object, _suspendedSaleService.Object, _resourceManager,
//                  _mocksaleHeadManager.Object, _mocksaleManager.Object, _saleLineManager.Object, _userService.Object, _policyService.Object, _customerManager.Object, _reasonService.Object, _paymentManager.Object);
//            var actual = _suspendSaleManager.SuspendSale(tillNumber, saleNumber, userCode, out error);
//            Assert.AreEqual(expected, error.MessageStyle.Message);
//        }

//        [Test]
//        public void UnsuspendSale()
//        {

//            var expected = 2;
//            int tillNumber = 1;
//            int saleNumber = 1;
//            var userCode = "Test user";
//            ErrorMessage error;
//            _policyService.Setup(u => u.GetPol("U_SUSP", null)).Returns(true);
//            _userService.Setup(u => u.GetUser(It.IsAny<string>()))
//                .Returns((string code) => { return GetUserData(code); });
//            _suspendedSaleService.Setup(u => u.GetAllSuspendedSale(It.IsAny<string>()))
//              .Returns((string sqlQuery) => { return GetAllSuspendedSale(sqlQuery); });

//            _suspendedSaleService.Setup(u => u.GetSuspendedSale(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
//              .Returns(() => { return GetSuspendedSale(); });
//            _suspendSaleManager = new SuspendedSaleManager(_policyManager.Object, _suspendedSaleService.Object, _resourceManager,
//                 _mocksaleHeadManager.Object, _mocksaleManager.Object, _saleLineManager.Object, _userService.Object, _policyService.Object, _customerManager.Object, _reasonService.Object, _paymentManager.Object);
//            var actual = _suspendSaleManager.UnsuspendSale(tillNumber, saleNumber, userCode, out error);
//            Assert.AreEqual(expected, actual.Sale_Num);
//        }


//        [Test]
//        public void UnsuspendSaleWhenGivenSaleIsNotSuspended()
//        {

//            var expected = "Sale Number 1 is not suspended. ~Not a Suspended Sale";
//            int tillNumber = 1;
//            int saleNumber = 1;
//            var userCode = "Test user";
//            ErrorMessage error;
//            _policyService.Setup(u => u.GetPol("U_SUSP", null)).Returns(true);
//            _userService.Setup(u => u.GetUser(It.IsAny<string>()))
//                .Returns((string code) => { return GetUserData(code); });
//            _suspendedSaleService.Setup(u => u.GetAllSuspendedSale(It.IsAny<string>()))
//              .Returns((string sqlQuery) => { return null; });

//            _suspendedSaleService.Setup(u => u.GetSuspendedSale(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
//              .Returns(() => { return GetSuspendedSale(); });
//            _suspendSaleManager = new SuspendedSaleManager(_policyManager.Object, _suspendedSaleService.Object, _resourceManager,
//                  _mocksaleHeadManager.Object, _mocksaleManager.Object, _saleLineManager.Object, _userService.Object, _policyService.Object, _customerManager.Object, _reasonService.Object, _paymentManager.Object);
//            var actual = _suspendSaleManager.UnsuspendSale(tillNumber, saleNumber, userCode, out error);
//            Assert.AreEqual(expected, error.MessageStyle.Message);
//        }
//    }
//}

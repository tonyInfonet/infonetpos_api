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
//    public class SaleManagerTest
//    {
//        private Mock<IPolicyManager> _policyManager = new Mock<IPolicyManager>();
//        private Mock<ISaleService> _saleService = new Mock<ISaleService>();
//        private Mock<ILoginManager> _loginManager = new Mock<ILoginManager>();
//        private Mock<ILoginService> _loginService = new Mock<ILoginService>();
//        private Mock<IStockService> _stockService = new Mock<IStockService>();
//        private Mock<IPolicyService> _policyService = new Mock<IPolicyService>();
//        private Mock<IUtilityService> _utilityService = new Mock<IUtilityService>();
//        private Mock<ITillService> _tillService = new Mock<ITillService>();
//        private Mock<IFuelService> _fuelService = new Mock<IFuelService>();
//        private Mock<ICustomerService> _customerService = new Mock<ICustomerService>();
//        private Mock<ICardService> _cardService = new Mock<ICardService>();
//        private Mock<ITaxService> _taxService = new Mock<ITaxService>();
//        private Mock<ISaleLineManager> _saleLineManager = new Mock<ISaleLineManager>();
//        private Mock<ISaleHeadManager> _saleHeadManager = new Mock<ISaleHeadManager>();
//        private Mock<ICustomerManager> _customerManager = new Mock<ICustomerManager>();
//        private Mock<IReasonService> _reasonService = new Mock<IReasonService>();
//        private Mock<IPromoManager> _promoManger = new Mock<IPromoManager>();
//        private IApiResourceManager _resourceManager = new ApiResourceManager();
//        private Mock<IGivexClientManager> _givexManager = new Mock<IGivexClientManager>();
//        private Mock<ICreditCardManager> _creditCardManager = new Mock<ICreditCardManager>();
//        private SaleManager _saleManager;

//        private Sale CreateSaleObject()
//        {
//            //dynamic flag=null;

//            var sale = new Sale
//            {
//                Sale_Num = 2,
//                TillNumber = 1,
//                TotalTaxSaved = 10,
//                ApplyTaxes = false

//            };
//            var saleLineFirst = new Sale_Line
//            {
//                Line_Num = 1,
//                Till_Num = 1,
//                Sale_Num = "2",
//                price = 100,
//                Quantity = 2,
//                Stock_Code = "12345",
//                PLU_Code = "12345",
//                Net_Amount = 90,
//                ProductIsFuel = true,
//                TE_COLLECTTAX = "GST",
//                IncludeInLoyalty = true,
//                PointsPerDollar = 12,
//                PointsPerUnit = 5,
//                Amount = 3,
//                Line_Discount = 2


//                //GC_REPT= SetPolicyforSaleLine(flag)
//            };
//            saleLineFirst.Line_Taxes.Add("GST", "A", 10, true, 2, 4, "");
//            sale.Sale_Lines.AddLine(1, saleLineFirst, "");
//            var saleLineSecond = new Sale_Line
//            {
//                Line_Num = 2,
//                Till_Num = 1,
//                Sale_Num = "2",
//                price = 50,
//                Quantity = 2,
//                Stock_Code = "123",
//                PLU_Code = "123",
//                Net_Amount = 48,
//                TE_COLLECTTAX = "PST",


//            };
//            saleLineSecond.Line_Taxes.Add("PST", "B", 2, true, 1, 1, "");

//            sale.Sale_Lines.AddLine(2, saleLineFirst, "");
//            sale.Sale_Totals = new Sale_Totals
//            {
//                SaleNumber = 2,
//                Total = 150,
//                Penny_Adj = 10,
//                Payment = 140,
//                Gross = 160,
//                Net = 160,
//                Invoice_Discount = 10
//            };
//            sale.Sale_Totals.Sale_Taxes.Add("GST", "A", 2, 160, 10, 20, 20, 1, 1, "GSTA");
//            sale.Customer = new Customer
//            {
//                Code = "A",
//                Name = "B",
//                Price_Code = 1,
//                GroupID = "1",
//                DiscountName = "A",
//                DiscountType = "C"



//            };

//            sale.Return_Reason = new Return_Reason
//            {
//                RType = "D",
//                Reason = "Damaged",
//                Description = "Damaged Product"
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
//        /// Get Trainer User
//        /// </summary>
//        /// <returns></returns>
//        private User GetTrainerUser()
//        {
//            EncryptionManager encryption = new EncryptionManager();
//            var text = encryption.EncryptText("abc");
//            var trainerUser = new User
//            {
//                Name = "X",
//                Code = "X",
//                epw = text,
//                Password = "abc",
//                User_Group = new User_Group
//                {
//                    Code = "Trainer",
//                    Name = "XYZ",
//                    SecurityLevel = 1
//                }
//            };

//            return trainerUser;
//        }

//        /// <summary>
//        /// Load Security 
//        /// </summary>
//        /// <returns></returns>
//        private Security LoadSecurityInfo()
//        {
//            var security = new Security();
//            security.Install_Date_Encrypt = System.DateTime.Now.ToString();
//            security.Security_Key = "";
//            security.POS_BO_Features = "";
//            security.Pump_Features = "";
//            security.NIC_Number = "";
//            security.Number_OF_POS = 1;
//            security.MaxConcurrentPOS = 1;
//            return security;
//        }

//        /// <summary>
//        /// Get Customers
//        /// </summary>
//        /// <returns></returns>
//        private List<Customer> GetCustomers()
//        {
//            var firstcustomer = new Customer
//            {
//                Code = "Test User",
//                Name = "Cash Sale",
//                LoyaltyCard = "123",
//                LoyaltyCardSwiped = false,
//                LoyaltyExpDate = "System.DateTime.Now",
//                GroupID = "1"
//            };
//            var secondcustomer = new Customer
//            {
//                Code = "A",
//                Name = "A",
//                LoyaltyCard = "123",
//                LoyaltyCardSwiped = false,
//                LoyaltyExpDate = "System.DateTime.Now",
//                GroupID = "1"
//            };

//            var customers = new List<Customer>()
//            {
//                firstcustomer,
//                secondcustomer
//            };
//            return customers;
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
//        /// Get  plu mast
//        /// </summary>
//        /// <returns></returns>
//        private PLUMast GetPLUCode()
//        {
//            var pluMast = new PLUMast
//            {
//                PLUCode = "Test",
//                PLUPrim = "",
//                PLUType = 'S'
//            };
//            return pluMast;
//        }

//        /// <summary>
//        ///Get Curent Sale From DbTemp
//        /// </summary>
//        /// <param name="salenumber"></param>
//        /// <param name="tillNumber"></param>
//        /// <returns></returns>
//        private Sale GetCurentSaleFromDbTemp()
//        {
//            var sale = new Sale
//            {
//                Sale_Num = 1,
//                TillNumber = (byte)1,
//                Customer = new Customer
//                {
//                    Code = "A",
//                    LoyaltyCard = "123",
//                    LoyaltyCardSwiped = false,
//                    LoyaltyExpDate = "System.DateTime.Now",
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
//        private Return_Reason GetReturnReason()
//        {
//            var returnReason = new Return_Reason
//            {
//                Description = "testDesc",
//                Reason = "D",
//                RType = "R"
//            };
//            return returnReason;
//        }

//        /// <summary>
//        /// Set SaleLinePolicy 
//        /// </summary>
//        /// <returns></returns>

//        private Sale_Line SetSaleLinePolicy()
//        {
//            var saleLine = new Sale_Line()
//            {
//                GC_REPT = "12",
//                LOYAL_PPD = 10M,
//                NUM_PRICE = 1,
//                GROUP_PRTY = false,
//                GC_DISCOUNT = false,
//                LOYAL_DISC = 1
//            };
//            return saleLine;
//        }

//        /// <summary>
//        /// GetSaleLines 
//        /// </summary>
//        /// <returns></returns>
//        private List<Sale_Line> GetSaleLinesFromDbtemp()
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
//        /// Get Till 
//        /// </summary>
//        /// 
//        private Till GetTill()
//        {
//            var till = new Till
//            {
//                Number = 1,
//                Active = true,
//                Processing = false,
//                Float = 10,
//                BonusFloat = 1,
//                Cash = 100,
//                Date_Open = System.DateTime.Now,
//                Time_Open = System.DateTime.Now,
//                ShiftDate = System.DateTime.Now,
//                Shift = 2,
//                UserLoggedOn = "X",
//                POSId = 1,
//                CashBonus = 50
//            };

//            return till;
//        }

//        /// <summary>
//        /// Create New Sale
//        /// </summary>
//        private Sale CreateNewSale()
//        {
//            var sale = new Sale();
//            sale.Sale_Totals.Sale_Taxes.Add("GST", "A", 2, 160, 10, 20, 20, 1, 1, "GSTA");
//            sale.USE_LOYALTY = true;
//            sale.LOYAL_TYPE = "";
//            sale.Loyal_pricecode = 1;
//            sale.CUST_DISC = false;
//            sale.Loydiscode = 1;
//            sale.PROD_DISC = false;
//            sale.Combine_Policy = false;
//            sale.XRigor = false;
//            sale.Customer.Code = "Test User";
//            sale.Customer.Name = "Cash Sale";
//            sale.Customer.LoyaltyCard = "123";
//            sale.Customer.LoyaltyCardSwiped = false;
//            sale.Customer.LoyaltyExpDate = System.DateTime.Now.ToString();

//            return sale;
//        }

//        /// <summary>
//        /// Intialize SaleManager SetUp 
//        /// </summary>

//        private SaleManager SaleManagerSetup()
//        {
//            _saleManager = new SaleManager(_policyManager.Object, _saleService.Object, _resourceManager, _loginManager.Object,
//                _loginService.Object, _stockService.Object, _policyService.Object, _utilityService.Object, _tillService.Object,
//                _customerService.Object, _cardService.Object, _taxService.Object, _saleLineManager.Object, _saleHeadManager.Object,
//                _customerManager.Object, _reasonService.Object, _givexManager.Object, _creditCardManager.Object);
//            return _saleManager;
//        }
//        [Test]
//        public void InitializeSaleTest()
//        {
//            int tillNumber = 1;
//            string userCode = "A";
//            string taxName = "PST";
//            var expected = 1;
//            ErrorMessage error;
//            var sale = new Sale();

//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });
//            _saleService.Setup(a => a.GetSaleByTillNumber(It.IsAny<int>())).Returns((int till) => { return GetCurentSaleFromDbTemp(); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//              { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });

//            // _saleHeadManager.Setup(a => a.CreateSaleObject(ref sale, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
//            //  .Returns(() => { return CreateSaleObject(); });
//            _saleManager = new SaleManager(_policyManager.Object, _saleService.Object, _resourceManager, _loginManager.Object, _loginService.Object,
//                _stockService.Object, _policyService.Object, _utilityService.Object, _tillService.Object, _customerService.Object,
//                _cardService.Object, _taxService.Object, _saleLineManager.Object, _saleHeadManager.Object, _customerManager.Object,
//                _reasonService.Object, _givexManager.Object, _creditCardManager.Object);
//            var actual = _saleManager.InitializeSale(tillNumber, userCode, out error);

//            Assert.AreEqual(expected, actual.Sale_Num);

//        }

//        [Test]
//        public void ClearSaleWhenNewSale()
//        {
//            int tillNumber = 1;
//            string userCode = "A";
//            string taxName = "PST";
//            string saleType = "";
//            var expected = 10;
//            int messageNumber = 1;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });
//            _saleService.Setup(a => a.GetSaleByTillNumber(It.IsAny<int>())).Returns((int till) => { return GetCurentSaleFromDbTemp(); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _saleService.Setup(u => u.GetMaxSaleNoFromSaleNumbFromDbAdmin(It.IsAny<int>(), out messageNumber)).Returns(() => { return 10; });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });
//            _saleManager = SaleManagerSetup();

//            //  var actual = _saleManager.Clear_Sale(sale.Sale_Num,sale.TillNumber, userCode, saleType, null, true, false, false, out error);
//            //Assert.AreEqual(expected, actual);
//            //_saleHeadManager.Setup(a => a.CreateSaleObject(ref sale, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
//            //    .Returns(() => { return CreateSaleObject(); });
//            _saleManager = new SaleManager(_policyManager.Object, _saleService.Object, _resourceManager, _loginManager.Object, _loginService.Object,
//                 _stockService.Object, _policyService.Object, _utilityService.Object, _tillService.Object, _customerService.Object,
//                 _cardService.Object, _taxService.Object, _saleLineManager.Object, _saleHeadManager.Object, _customerManager.Object,
//                 _reasonService.Object, _givexManager.Object, _creditCardManager.Object);
//            var actual = _saleManager.Clear_Sale(sale, sale.Sale_Num, sale.TillNumber, userCode, saleType, null, true, false, false, out error);
//            Assert.AreEqual(expected, actual);
//        }


//        [Test]
//        public void GetSaleNumberWhenSuspenededSaleNumberISGreaterThanSaleHeadTest()
//        {
//            int tillNumber = 1;
//            string userCode = "Trainer";

//            var expected = 12;
//            //  int messageNumber = 1;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns(() => { return GetTrainerUser(); });

//            _saleService.Setup(a => a.GetMaxSaleNoFromSaleHeadFromDbTill(It.IsAny<int>())).Returns((int till) => { return 10; });

//            _saleService.Setup(a => a.GetMaxSaleNoFromSusHeadFromDbTill(It.IsAny<int>())).Returns(() => { return 11; });


//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.GetSaleNo(tillNumber, userCode, out error);
//            Assert.AreEqual(expected, actual);

//        }

//        [Test]
//        public void GetSaleNumberWhenSuspenededSaleNumberISLessThankSaleheadTestNotFound()
//        {
//            int tillNumber = 1;
//            string userCode = "Trainer";

//            var expected = 10;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns(() => { return GetTrainerUser(); });

//            _saleService.Setup(a => a.GetMaxSaleNoFromSaleHeadFromDbTill(It.IsAny<int>())).Returns((int till) => { return 10; });

//            _saleService.Setup(a => a.GetMaxSaleNoFromSusHeadFromDbTill(It.IsAny<int>())).Returns(() => { return 0; });

//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.GetSaleNo(tillNumber, userCode, out error);
//            Assert.AreEqual(expected, actual);

//        }

//        [Test]
//        public void GetSaleNumberWhenUserISNotTrainer()
//        {
//            int tillNumber = 1;
//            string userCode = "X";

//            var expected = 15;
//            int messageNumber = 1;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetMaxSaleNoFromSaleHeadFromDbTill(It.IsAny<int>())).Returns((int till) => { return 10; });

//            _saleService.Setup(a => a.GetMaxSaleNoFromSusHeadFromDbTill(It.IsAny<int>())).Returns(() => { return 0; });

//            _saleService.Setup(a => a.GetMaxSaleNoFromSaleNumbFromDbAdmin(It.IsAny<int>(), out messageNumber)).Returns(() => { return 15; });


//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.GetSaleNo(tillNumber, userCode, out error);
//            Assert.AreEqual(expected, actual);
//        }

//        [Test]
//        public void ApplyTaxTestWhenTax_CompANDTax_RebateIsTrue()
//        {
//            int tillNumber = 1;
//            string userCode = "A";
//            string taxName = "PST";
//            string saleType = "";
//            var expected = 23.52M;
//            int messageNumber = 1;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSaleByTillNumber(It.IsAny<int>())).Returns((int till) => { return GetCurentSaleFromDbTemp(); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });

//            _policyManager.Setup(a => a.TAX_COMP).Returns(true);

//            _policyManager.Setup(a => a.Tax_Rebate).Returns(true);
//            _policyManager.Setup(a => a.TE_Type).Returns("");

//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _saleService.Setup(u => u.GetMaxSaleNoFromSaleNumbFromDbAdmin(It.IsAny<int>(), out messageNumber)).Returns(() => { return 10; });

//            _saleService.Setup(u => u.SaveSale(It.IsAny<int>(), It.IsAny<int>(), sale));


//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });


//            _saleManager = SaleManagerSetup();
//            _saleManager.ApplyTaxes(ref sale, true);
//            Assert.AreEqual(expected, sale.Sale_Totals.Sale_Taxes[1].Tax_Included_Amount);

//        }


//        [Test]
//        public void ApplyTaxTestWhenApplyTaxInSaleISFalse()
//        {
//            int tillNumber = 1;
//            string userCode = "A";
//            string taxName = "PST";
//            string saleType = "";
//            var expected = 20M;
//            int messageNumber = 1;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSaleByTillNumber(It.IsAny<int>())).Returns((int till) => { return GetCurentSaleFromDbTemp(); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });

//            _policyManager.Setup(a => a.TAX_COMP).Returns(true);

//            _policyManager.Setup(a => a.Tax_Rebate).Returns(true);
//            _policyManager.Setup(a => a.TE_Type).Returns("");

//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _saleService.Setup(u => u.GetMaxSaleNoFromSaleNumbFromDbAdmin(It.IsAny<int>(), out messageNumber)).Returns(() => { return 10; });

//            _saleService.Setup(u => u.SaveSale(It.IsAny<int>(), It.IsAny<int>(), sale));


//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });


//            _saleManager = SaleManagerSetup();
//            sale.ApplyTaxes = true;
//            _saleManager.ApplyTaxes(ref sale, true);
//            Assert.AreEqual(expected, sale.Sale_Totals.Sale_Taxes[1].Tax_Included_Amount);

//        }


//        [Test]
//        public void GetCurrentSaleWhenThereISCurrentSale()
//        {
//            int tillNumber = 1;
//            string userCode = "A";
//            string taxName = "PST";
//            int saleNumber = 2;
//            var expected = 1;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });


//            _saleHeadManager.Setup(a => a.CreateNewSale()).Returns(CreateNewSale());


//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });


//            _saleManager = SaleManagerSetup();
//            sale.ApplyTaxes = true;
//            var actual = _saleManager.GetCurrentSale(saleNumber, tillNumber, 98, userCode, out error);
//            Assert.AreEqual(expected, actual.Sale_Num);

//        }

//        [Test]
//        public void GetCurrentSaleWhenThereISNoCurrentSale()
//        {
//            int tillNumber = 1;
//            string userCode = "A";
//            string taxName = "PST";
//            int saleNumber = 2;
//            var expected = "Request is invalid ";
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return null; });


//            _saleHeadManager.Setup(a => a.CreateNewSale()).Returns(CreateNewSale());


//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });

//            _saleManager = SaleManagerSetup();

//            sale.ApplyTaxes = true;
//            var actual = _saleManager.GetCurrentSale(saleNumber, tillNumber, 98, userCode, out error);
//            Assert.AreEqual(expected, error.MessageStyle.Message);

//        }

//        [Test]
//        public void AddSaleLineItemWhenInvalidStockCodeTest()
//        {
//            int tillNumber = 1;
//            string userCode = "A";
//            string taxName = "PST";
//            int saleNumber = 2;
//            var expected = "Invalid stock Code";
//            ErrorMessage error;
//            _saleManager = SaleManagerSetup();

//            var actual = _saleManager.AddSaleLineItem(userCode, tillNumber, saleNumber, 98, "", 2, false, null, out error);
//            Assert.AreEqual(expected, error.MessageStyle.Message);

//        }


//        [Test]
//        public void AddSaleLineItemWhenValidStockCodeButThereIsNoCurrentSaleTest()
//        {
//            int tillNumber = 1;
//            string userCode = "A";
//            int saleNumber = 2;
//            var expected = "Request is invalid ";
//            ErrorMessage error;
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return null; });

//            _saleManager = SaleManagerSetup();

//            var actual = _saleManager.AddSaleLineItem(userCode, tillNumber, saleNumber, 98, userCode, 2, false, null, out error);
//            Assert.AreEqual(expected, error.MessageStyle.Message);

//        }


//        [Test]
//        public void AddSaleLineItemWhenValidStockCodeAndThereISCurrentSaler()
//        {
//            int tillNumber = 1;
//            string userCode = "A";
//            string taxName = "PST";
//            int saleNumber = 2;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            object saleLine;
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(false);
//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });

//            _saleHeadManager.Setup(a => a.CreateNewSale()).Returns(CreateNewSale());

//            _stockService.Setup(u => u.GetPluMast(It.IsAny<string>()))
//               .Returns((string stockCode) => { return GetPLUCode(); });

//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });


//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.AddSaleLineItem(userCode, tillNumber, saleNumber, 98, userCode, 2, false, null, out error);
//            Assert.AreEqual(1, 1);

//        }


//        [Test]
//        public void RecomputeCouponWhenThereISNOCouponInSaleOrSaleTotalCouponISZerorTest()
//        {
//            var expected = 0.00M;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(false);

//            _policyManager.Setup(a => a.FuelLoyalty).Returns(true);


//            _stockService.Setup(u => u.GetPluMast(It.IsAny<string>()))
//               .Returns((string stockCode) => { return GetPLUCode(); });

//            _saleManager = SaleManagerSetup();
//            _saleManager.ReCompute_Coupon(ref sale);
//            Assert.AreEqual(expected, sale.CouponTotal);

//        }

//        [Test]
//        public void RecomputeCouponWhenThereWhenSaleCouponTotalISNotZero()
//        {
//            var expected = 8.00M;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            sale.CouponTotal = 10;
//            sale.Customer.DiscountRate = 2;
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(false);

//            _policyManager.Setup(a => a.FuelLoyalty).Returns(true);

//            _utilityService.Setup(a => a.IsCouponAvailable(It.IsAny<string>())).Returns(true);
//            _stockService.Setup(u => u.GetPluMast(It.IsAny<string>()))
//               .Returns((string stockCode) => { return GetPLUCode(); });

//            _saleManager = SaleManagerSetup();
//            _saleManager.ReCompute_Coupon(ref sale);
//            Assert.AreEqual(expected, sale.CouponTotal);

//        }

//        [Test]
//        public void Recomputetotal()
//        {
//            var expected = 180;
//            string taxName = "PST";
//            var sale = CreateSaleObject();
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(false);
//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });

//            _saleHeadManager.Setup(a => a.CreateNewSale()).Returns(CreateNewSale());

//            _stockService.Setup(u => u.GetPluMast(It.IsAny<string>()))
//               .Returns((string stockCode) => { return GetPLUCode(); });

//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });

//            _saleManager = SaleManagerSetup();
//            _saleManager.ReCompute_Totals(ref sale);
//            Assert.AreEqual(expected, sale.Sale_Totals.Gross);

//        }


//        [Test]
//        public void SaveTempTest()
//        {
//            int tillNumber = 1;
//            var expected = 1;
//            var sale = CreateSaleObject();

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleManager = SaleManagerSetup();
//            _saleManager.SaveTemp(ref sale, tillNumber);
//            Assert.AreEqual(expected, sale.TillNumber);

//        }

//        [Test]
//        public void ComputePointsWithOutLoyalityTest()
//        {
//            int tillNumber = 1;
//            var expected = -10;
//            var sale = CreateSaleObject();

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.ComputePoints(sale);
//            Assert.AreEqual(expected, actual);

//        }

//        [Test]
//        public void ComputePointsWithLoyalityTest()
//        {
//            int tillNumber = 1;
//            var expected = 14;
//            var sale = CreateSaleObject();
//            sale.USE_LOYALTY = true;
//            sale.LOYAL_TYPE = "Points";
//            sale.Customer.Loyalty_Code = "L";
//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.ComputePoints(sale);
//            Assert.AreEqual(expected, actual);

//        }

//        [Test]
//        public void SubPointWhenUseLoyalityTest()
//        {
//            int tillNumber = 1;
//            var expected = 2;
//            var sale = CreateSaleObject();
//            sale.USE_LOYALTY = true;
//            sale.LOYAL_TYPE = "Points";
//            sale.Customer.Loyalty_Code = "L";
//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });
//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.SubPoints(sale);
//            Assert.AreEqual(expected, actual);

//        }

//        [Test]
//        public void SubPointWhenUseLoyalityISFalseTest()
//        {
//            int tillNumber = 1;
//            var expected = 0;
//            var sale = CreateSaleObject();
//            sale.USE_LOYALTY = false;
//            sale.LOYAL_TYPE = "Points";
//            sale.Customer.Loyalty_Code = "L";
//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });
//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.SubPoints(sale);
//            Assert.AreEqual(expected, actual);

//        }

//        [Test]
//        public void SubRedeemableTest()
//        {
//            int tillNumber = 1;
//            var expected = 0;
//            var sale = CreateSaleObject();
//            sale.USE_LOYALTY = false;
//            sale.LOYAL_TYPE = "Points";
//            sale.Customer.Loyalty_Code = "L";
//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });
//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.SubRedeemable(sale);
//            Assert.AreEqual(expected, actual);

//        }

//        [Test]
//        public void ReCompute_CashBonusWhenDiscountTypeNotBTest()
//        {
//            int tillNumber = 1;
//            var expected = 0;
//            var sale = CreateSaleObject();
//            sale.USE_LOYALTY = false;
//            sale.CBonusTotal = 50;
//            sale.LOYAL_TYPE = "Points";
//            sale.Customer.DiscountType = "L";
//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });
//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleManager = SaleManagerSetup();
//            _saleManager.ReCompute_CashBonus(ref sale);
//            Assert.AreEqual(expected, sale.CBonusTotal);

//        }

//        [Test]
//        public void ReCompute_CashBonusWhenDiscountTypeBTest()
//        {
//            int tillNumber = 1;
//            var expected = 0;
//            var sale = CreateSaleObject();
//            sale.USE_LOYALTY = false;
//            sale.LOYAL_TYPE = "Points";
//            sale.Customer.DiscountType = "B";
//            sale.CBonusTotal = 50;
//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            _policyService.Setup(a => a.GetPol("EXC_CASHBONUS", null)).Returns("A");
//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });
//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleManager = SaleManagerSetup();
//            _saleManager.ReCompute_CashBonus(ref sale);
//            Assert.AreEqual(expected, sale.CBonusTotal);

//        }

//        [Test]
//        public void RemoveSaleLineItemWhenUserISNotAuthorizedToDeleteLineItemTest()
//        {
//            int tillNumber = 1;
//            int saleNumber = 1;
//            string userCode = "A";
//            string taxName = "PST";
//            string saleType = "";
//            var expected = "You are not authorized to delete lines_Select an Authorized User?~Authorization Required";
//            int messageNumber = 1;

//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(false);
//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });

//            _saleHeadManager.Setup(a => a.CreateNewSale()).Returns(CreateNewSale());

//            _stockService.Setup(u => u.GetPluMast(It.IsAny<string>()))
//               .Returns((string stockCode) => { return GetPLUCode(); });

//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });

//            _saleManager = SaleManagerSetup();
//            _saleManager.RemoveSaleLineItem(userCode, tillNumber, saleNumber, 1, out error, false, false);
//            Assert.AreEqual(expected, error.MessageStyle.Message);

//        }

//        [Test]
//        public void RemoveSaleLineItemWhenUserISAllowtoDeleteLineItemTest()
//        {
//            int tillNumber = 1;
//            int saleNumber = 1;
//            string userCode = "A";
//            string taxName = "PST";
//            string saleType = "";
//            var expected = 1;
//            int messageNumber = 1;

//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });

//            _saleHeadManager.Setup(a => a.CreateNewSale()).Returns(CreateNewSale());

//            _stockService.Setup(u => u.GetPluMast(It.IsAny<string>()))
//               .Returns((string stockCode) => { return GetPLUCode(); });

//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });

//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.RemoveSaleLineItem(userCode, tillNumber, saleNumber, 1, out error, false, false);
//            Assert.AreEqual(expected, actual.Sale_Lines.Count);

//        }

//        [Test]
//        public void Adjust_LinesTest()
//        {

//            string taxName = "PST";
//            var expected = true;
//            ErrorMessage error;
//            var sale = CreateSaleObject();

//            var SL = sale.Sale_Lines.FirstOrDefault();
//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });

//            _saleHeadManager.Setup(a => a.CreateNewSale()).Returns(CreateNewSale());

//            _stockService.Setup(u => u.GetPluMast(It.IsAny<string>()))
//               .Returns((string stockCode) => { return GetPLUCode(); });

//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });

//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.Adjust_Lines(ref SL, sale, false, false);
//            Assert.AreEqual(expected, actual);

//        }

//        [Test]
//        public void Sale_DiscountTest()
//        {

//            string taxName = "PST";
//            var expected = 4;
//            ErrorMessage error;
//            var sale = CreateSaleObject();

//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });

//            _saleHeadManager.Setup(a => a.CreateNewSale()).Returns(CreateNewSale());

//            _stockService.Setup(u => u.GetPluMast(It.IsAny<string>()))
//               .Returns((string stockCode) => { return GetPLUCode(); });

//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });

//            _saleManager = SaleManagerSetup();
//            _saleManager.Sale_Discount(ref sale, 10, "C");
//            Assert.AreEqual(expected, sale.Sale_Line_Disc);

//        }

//        [Test]
//        public void Line_QuantityTest()
//        {

//            string taxName = "PST";
//            var expected = true;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            var SL = sale.Sale_Lines.FirstOrDefault();

//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _loginService.Setup(a => a.LoadSecurityInfo()).Returns(LoadSecurityInfo());
//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });

//            _saleHeadManager.Setup(a => a.CreateNewSale()).Returns(CreateNewSale());

//            _stockService.Setup(u => u.GetPluMast(It.IsAny<string>()))
//               .Returns((string stockCode) => { return GetPLUCode(); });

//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });

//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.Line_Quantity(ref sale, ref SL, 10, false);
//            Assert.AreEqual(expected, actual);

//        }

//        [Test]
//        public void Line_Discount_RateTest()
//        {

//            string taxName = "PST";
//            var expected = 180;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            var SL = sale.Sale_Lines.FirstOrDefault();

//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _loginService.Setup(a => a.LoadSecurityInfo()).Returns(LoadSecurityInfo());
//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });

//            _saleHeadManager.Setup(a => a.CreateNewSale()).Returns(CreateNewSale());

//            _stockService.Setup(u => u.GetPluMast(It.IsAny<string>()))
//               .Returns((string stockCode) => { return GetPLUCode(); });

//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });

//            _saleManager = SaleManagerSetup();
//            _saleManager.Line_Discount_Rate(ref sale, ref SL, 10);
//            Assert.AreEqual(expected, sale.Sale_Totals.Gross);

//        }


//        [Test]
//        public void Line_Discount_TypeTest()
//        {

//            var expected = "Test";
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            var SL = sale.Sale_Lines.FirstOrDefault();
//            _saleManager = SaleManagerSetup();
//            _saleManager.Line_Discount_Type(ref SL, "Test");
//            Assert.AreEqual(expected, SL.Discount_Type);

//        }

//        [Test]
//        public void Line_ReasonTest()
//        {
//            var returnReason = GetReturnReason();

//            var expected = "R";
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            var SL = sale.Sale_Lines.FirstOrDefault();
//            _policyManager.Setup(a => a.TE_Type).Returns("");
//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleManager = SaleManagerSetup();
//            _saleManager.Line_Reason(ref sale, ref SL, returnReason);
//            Assert.AreEqual(expected, SL.Return_Reasons[1].RType);

//        }

//        [Test]
//        public void Line_Price_NumberTest()
//        {

//            var expected = "R";
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            var SL = sale.Sale_Lines.FirstOrDefault();
//            _policyManager.Setup(a => a.TE_Type).Returns("");
//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleManager = SaleManagerSetup();
//            _saleManager.Line_Price_Number(ref sale, ref SL, 2);

//        }


//        [Test]
//        public void SetGrossWhenTAX_EXEMPT_GAIsFalseTest()
//        {

//            var expected = 19;
//            var sale = CreateSaleObject();
//            var saletotals = sale.Sale_Totals;
//            _saleManager = SaleManagerSetup();
//            _saleManager.SetGross(ref saletotals, 10);
//            Assert.AreEqual(expected, saletotals.Gross);

//        }

//        [Test]
//        public void SetGrossWhenTAX_EXEMPT_GAIsTrueTest()
//        {

//            var expected = 19;
//            _policyManager.Setup(a => a.TAX_EXEMPT_GA).Returns(true);
//            var sale = CreateSaleObject();
//            var saletotals = sale.Sale_Totals;
//            _saleManager = SaleManagerSetup();
//            _saleManager.SetGross(ref saletotals, 10);
//            Assert.AreEqual(expected, saletotals.Gross);

//        }

//        [Test]
//        public void SetCustomerWhenCustomerDoesNotExistTest()
//        {

//            string taxName = "PST";
//            var expected = "Customer Does not Exist";
//            ErrorMessage error;
//            var sale = CreateSaleObject();

//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });


//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });

//            _saleManager = SaleManagerSetup();
//            _saleManager.SetCustomer("1", 1, 1, "X", 123, out error);
//            Assert.AreEqual(expected, error.MessageStyle.Message);

//        }

//        [Test]
//        public void SetCustomerWhenCustomerIsPreviousCustomerTest()
//        {

//            string taxName = "PST";
//            var expected = "A";
//            ErrorMessage error;
//            var sale = CreateSaleObject();

//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });

//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.SetCustomer("A", 1, 1, "X", 123, out error);
//            Assert.AreEqual(expected, actual.Customer.Code);

//        }

//        [Test]
//        public void SetCustomerWhenCustomerIsTest()
//        {

//            string taxName = "PST";
//            var expected = "Test User";
//            ErrorMessage error;
//            var sale = CreateSaleObject();

//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });


//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });

//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.SetCustomer("Test User", 1, 1, "X", 123, out error);
//            Assert.AreEqual(expected, actual.Customer.Code);

//        }

//        [Test]
//        public void CheckEditOptionsWhenUserCanChangeQuantityTest()
//        {

//            string taxName = "PST";
//            var expected = true;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            var SL = sale.Sale_Lines;

//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _policyService.Setup(a => a.GetPol("U_CHGQTY", It.IsAny<object>())).Returns(true);
//            _policyService.Setup(a => a.GetPol("U_DISCOUNTS", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("U_CHGPRICE", It.IsAny<object>())).Returns(false);

//            _policyService.Setup(a => a.GetPol("ALLOW_QC", It.IsAny<object>())).Returns(true);
//            _policyService.Setup(a => a.GetPol("ALLOW_PC", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("CL_DISCOUNTS", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("DISC_REASON", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("PR_REASON", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("RET_REASON", It.IsAny<object>())).Returns(false);


//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.CheckEditOptions(SL, "");
//            Assert.AreEqual(expected, actual.FirstOrDefault().AllowQuantityChange);

//        }

//        [Test]
//        public void CheckEditOptionsWhenUserCanChangePriceTest()
//        {

//            string taxName = "PST";
//            var expected = true;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            var SL = sale.Sale_Lines;

//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _policyService.Setup(a => a.GetPol("U_CHGQTY", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("U_DISCOUNTS", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("U_CHGPRICE", It.IsAny<object>())).Returns(true);

//            _policyService.Setup(a => a.GetPol("ALLOW_QC", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("ALLOW_PC", It.IsAny<object>())).Returns(true);
//            _policyService.Setup(a => a.GetPol("CL_DISCOUNTS", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("DISC_REASON", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("PR_REASON", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("RET_REASON", It.IsAny<object>())).Returns(false);


//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.CheckEditOptions(SL, "");
//            Assert.AreEqual(expected, actual.FirstOrDefault().AllowPriceChange);

//        }

//        [Test]
//        public void CheckEditOptionsWhenAllowDiscountChangeTest()
//        {

//            string taxName = "PST";
//            var expected = true;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            var SL = sale.Sale_Lines;

//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _policyService.Setup(a => a.GetPol("U_CHGQTY", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("U_DISCOUNTS", It.IsAny<object>())).Returns(true);
//            _policyService.Setup(a => a.GetPol("U_CHGPRICE", It.IsAny<object>())).Returns(false);

//            _policyService.Setup(a => a.GetPol("ALLOW_QC", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("ALLOW_PC", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("CL_DISCOUNTS", It.IsAny<object>())).Returns(true);
//            _policyService.Setup(a => a.GetPol("DISC_REASON", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("PR_REASON", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("RET_REASON", It.IsAny<object>())).Returns(false);


//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.CheckEditOptions(SL, "");
//            Assert.AreEqual(expected, actual.FirstOrDefault().AllowDiscountChange);

//        }

//        [Test]
//        public void CheckEditOptionsWhenAllowDiscountReasonTest()
//        {

//            string taxName = "PST";
//            var expected = true;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            var SL = sale.Sale_Lines;

//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });


//            _policyService.Setup(a => a.GetPol("ALLOW_QC", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("ALLOW_PC", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("CL_DISCOUNTS", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("DISC_REASON", It.IsAny<object>())).Returns(true);
//            _policyService.Setup(a => a.GetPol("PR_REASON", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("RET_REASON", It.IsAny<object>())).Returns(false);


//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.CheckEditOptions(SL, "");
//            Assert.AreEqual(expected, actual.FirstOrDefault().AllowDiscountReason);

//        }

//        [Test]
//        public void CheckEditOptionsWhenAllowPriceReasonTest()
//        {

//            string taxName = "PST";
//            var expected = true;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            var SL = sale.Sale_Lines;

//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _policyService.Setup(a => a.GetPol("ALLOW_QC", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("ALLOW_PC", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("CL_DISCOUNTS", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("DISC_REASON", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("PR_REASON", It.IsAny<object>())).Returns(true);
//            _policyService.Setup(a => a.GetPol("RET_REASON", It.IsAny<object>())).Returns(false);


//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.CheckEditOptions(SL, "");
//            Assert.AreEqual(expected, actual.FirstOrDefault().AllowPriceReason);

//        }

//        [Test]
//        public void CheckEditOptionsWhenAllowReturnReasonTest()
//        {

//            string taxName = "PST";
//            var expected = true;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            var SL = sale.Sale_Lines;

//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _policyService.Setup(a => a.GetPol("ALLOW_QC", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("ALLOW_PC", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("CL_DISCOUNTS", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("DISC_REASON", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("PR_REASON", It.IsAny<object>())).Returns(false);
//            _policyService.Setup(a => a.GetPol("RET_REASON", It.IsAny<object>())).Returns(true);


//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.CheckEditOptions(SL, "");
//            Assert.AreEqual(expected, actual.FirstOrDefault().AllowReturnReason);

//        }


//        [Test]
//        public void UpdateSaleLineTest()
//        {

//            string taxName = "PST";
//            var expected = 201.86;
//            ErrorMessage error;
//            var sale = CreateSaleObject();
//            var SL = sale.Sale_Lines.FirstOrDefault();

//            _loginManager.Setup(a => a.GetUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });

//            _policyManager.Setup(a => a.PROMO_SALE).Returns(true);

//            _loginService.Setup(a => a.LoadSecurityInfo()).Returns(LoadSecurityInfo());
//            _saleService.Setup(a => a.GetSale(It.IsAny<int>(), It.IsAny<int>())).Returns((int till, int sale_num) => { return GetCurentSaleFromDbTemp(); });

//            _policyService.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
//            _saleLineManager.Setup(a => a.CreateNewSaleLine()).Returns(() => { return SetSaleLinePolicy(); });

//            _saleHeadManager.Setup(a => a.CreateNewSale()).Returns(CreateNewSale());

//            _stockService.Setup(u => u.GetPluMast(It.IsAny<string>()))
//               .Returns((string stockCode) => { return GetPLUCode(); });

//            _reasonService.Setup(a => a.GetReturnReason(It.IsAny<string>(), It.IsAny<char>()))
//                .Returns(() => { return GetReturnReason(); });

//            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
//            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

//            _saleService.Setup(u => u.GetSaleLinesFromDbTemp(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//          .Returns(() => { return GetSaleLinesFromDbtemp(); });

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>())).Returns(() => { return GetTill(); });
//            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

//            _taxService.Setup(u => u.GetTaxRatesByName(taxName)).Returns((string name) => { return GetTaxRateByTaxName(name); });

//            _saleManager = SaleManagerSetup();
//            var actual = _saleManager.UpdateSaleLine(1, 1, 1, "A", 2, "R", 4, 10, "R", "PriceChanges", 123, out error);
//            Assert.AreEqual(expected, actual.Sale_Totals.Gross);

//        }

//    }
//}

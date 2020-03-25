using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Infonet.CStoreCommander.BusinessLayer.Entities;

namespace Infonet.CStoreCommander.WebApi.Tests.Managers
{
    [TestFixture]
    public class SaleLineManagerTest
    {
        private readonly Mock<IPolicyManager> _policyManager = new Mock<IPolicyManager>();
        private Mock<ISaleLineService> _saleLineService = new Mock<ISaleLineService>();
        private Mock<ILoginService> _loginService = new Mock<ILoginService>();
        private Mock<IStockService> _stockService = new Mock<IStockService>();
        private Mock<IStockManager> _stockManager = new Mock<IStockManager>();
        private Mock<IFuelService> _fuelService = new Mock<IFuelService>();
        private Mock<IUtilityService> _utilityService = new Mock<IUtilityService>();
        private Mock<ILoginManager> _loginManager = new Mock<ILoginManager>();
        private Mock<IPromoManager> _promoManager = new Mock<IPromoManager>();

        private IApiResourceManager _resourceManager = new ApiResourceManager();
        private SaleLineManager _saleLineManager;

        /// <summary>
        /// Create Sale object
        /// </summary>
        /// <returns></returns>
        private Sale CreateSaleObjectTestData()
        {
            //dynamic flag=null;

            var sale = new Sale
            {
                Sale_Num = 2,
                TillNumber = 1,
                TotalTaxSaved = 10,
                ApplyTaxes = false

            };
            var saleLineFirst = new Sale_Line
            {
                Line_Num = 1,
                Till_Num = 1,
                Sale_Num = "2",
                price = 100,
                Quantity = 2,
                Stock_Code = "12345",
                PLU_Code = "12345",
                Net_Amount = 90,
                ProductIsFuel = true,
                TE_COLLECTTAX = "GST",
                IncludeInLoyalty = true,
                PointsPerDollar = 12,
                PointsPerUnit = 5,
                Amount = 3,
                Line_Discount = 2,
                Discount_Type = "%"


                //GC_REPT= SetPolicyforSaleLine(flag)
            };
            saleLineFirst.Line_Taxes.Add("GST", "A", 10, true, 2, 4, "");
            sale.Sale_Lines.AddLine(1, saleLineFirst, "");
            var saleLineSecond = new Sale_Line
            {
                Line_Num = 2,
                Till_Num = 1,
                Sale_Num = "2",
                price = 50,
                Quantity = 2,
                Stock_Code = "123",
                PLU_Code = "123",
                Net_Amount = 48,
                TE_COLLECTTAX = "PST",
                Discount_Type = "$"


            };
            saleLineSecond.Line_Taxes.Add("PST", "B", 2, true, 1, 1, "");

            sale.Sale_Lines.AddLine(2, saleLineFirst, "");
            sale.Sale_Totals = new Sale_Totals
            {
                SaleNumber = 2,
                Total = 150,
                Penny_Adj = 10,
                Payment = 140,
                Gross = 160,
                Net = 160,
                Invoice_Discount = 10
            };
            sale.Sale_Totals.Sale_Taxes.Add("GST", "A", 2, 160, 10, 20, 20, 1, 1, "GSTA");
            sale.Customer = new Customer
            {
                Code = "A",
                Name = "B",
                Price_Code = 1,
                GroupID = "1",
                DiscountName = "A",
                DiscountType = "C"



            };

            sale.Return_Reason = new Return_Reason
            {
                RType = "D",
                Reason = "Damaged",
                Description = "Damaged Product"
            };

            return sale;
        }

        /// <summary>
        /// Load Security 
        /// </summary>
        /// <returns></returns>
        private Security LoadSecurityInfo()
        {
            var security = new Security
            {
                Install_Date_Encrypt = System.DateTime.Now.ToString(CultureInfo.InvariantCulture),
                Security_Key = "",
                POS_BO_Features = "",
                Pump_Features = "",
                NIC_Number = "",
                Number_OF_POS = 1,
                MaxConcurrentPOS = 1
            };
            return security;
        }

        /// <summary>
        /// Get  plu mast
        /// </summary>
        /// <returns></returns>
        private Stock GetStockDetails()
        {
            var stock = new Stock
            {
                StockCode = "1",
                Description = "Test description",
                Department = "2",
                SubDepartment = "",
                SubDetail = "V",
                Vendor = "",
                PRType = 'R',
                PRUnit = '$'
            };
            stock.Vendor = "";
            stock.LoyaltySave = 0;
            stock.ProductDescription = 1;
            stock.SByWeight = false;
            stock.UM = "";
            stock.StandardCost = 5;

            stock.AverageCost = 5;
            return stock;
        }

        /// <summary>
        /// Get  Stock item test Data
        /// </summary>
        /// <returns></returns>
        private Sale_Line GetStockItemTestData()
        {
            var stockItem = new Sale_Line
            {
                Stock_Code = "1",
                Description = "TestDescription",
                Stock_Type = 'I',
                User = string.Empty
            };
            return stockItem;
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

        /// <summary>
        /// Load Promos 
        /// </summary>
        /// <returns></returns>
        private Promos LoadPromoTestData()
        {
            var firstPromo = new Promo
            {
                PromoID = "1",
                Day = 1,
                TotalQty = 1,
                MaxLink = 2,
                Amount = 10

            };

            var secondPromo = new Promo
            {
                PromoID = "2",
                Day = 1,
                TotalQty = 1,
                MaxLink = 2,
                Amount = 20
            };

            var promos = new Promos { { firstPromo, "" }, { secondPromo, "" } };
            return promos;
        }

        private List<Promo> GetPromoTestData()
        {
            var firstPromo = new Promo
            {
                PromoID = "1",
                Day = 1,
                TotalQty = 1,
                MaxLink = 2,
                Amount = 10

            };

            var secondPromo = new Promo
            {
                PromoID = "2",
                Day = 1,
                TotalQty = 1,
                MaxLink = 2,
                Amount = 20
            };

            var promos = new List<Promo>
            {
                firstPromo,
                secondPromo
            };

            return promos;
        }
        /// <summary>
        /// Get Sale_tax Test Data
        /// </summary>
        /// <returns></returns>
        private List<Sale_Tax> GetSaleTaxTestData()
        {
            var taxes = new List<Sale_Tax>();
            var tax1 = new Sale_Tax
            {
                Tax_Name = "PST",
                Tax_Code = "1",
                Tax_Rate = 10,
                Tax_Included = true,
            };

            var tax2 = new Sale_Tax
            {
                Tax_Name = "HST",
                Tax_Code = "2",
                Tax_Rate = 20,
                Tax_Included = false,
            };

            taxes.Add(tax1);
            taxes.Add(tax2);
            return taxes;
        }

        /// <summary>
        /// Get Group Price Head
        /// </summary>
        /// <returns></returns>
        private GroupPriceHead GetGroupPriceHeadTestData()
        {
            var priceHead = new GroupPriceHead
            {
                Department = "test",
                SubDepartment = "test",
                SubDetail = "desc",
                PrFrom = new System.DateTime(2008, 12, 25),
                PrTo = new System.DateTime(2017, 12, 25),
                PrType = 'I',
                PrUnit = 'I'
            };
            return priceHead;
        }

        /// <summary>
        /// Get Group Price line test Data 
        /// </summary>
        /// <returns></returns>
        private List<GroupPriceLine> GetGroupPriceLineTestData()
        {
            var priceLineFirst = new GroupPriceLine
            {
                Department = "test",
                SubDepartment = "test",
                SubDetail = "desc",
                PrFQty = 1,
                PrTQty = 1,
                Price = 12,
                PrType = 'I'
            };

            var priceLineSecond = new GroupPriceLine
            {
                Department = "test1",
                SubDepartment = "test2",
                SubDetail = "desc1",
                PrFQty = 1,
                PrTQty = 1,
                Price = 12,
                PrType = 'I'
            };
            var priceLine = new List<GroupPriceLine>
            {
                priceLineFirst,
                priceLineSecond
            };
            return priceLine;
        }
        /// <summary>
        /// Get Associated Charge Test Data 
        /// </summary>
        /// <returns></returns>
        private List<AssociateCharge> GetAssociatedChargeTestData()
        {
            var associatedFirstCharge = new AssociateCharge()
            {
                StockCode = "Test",
                AsCode = "1",
                Description = "AssociatedCharge",
                Price = 10

            };
            var associatedSecondCharge = new AssociateCharge()
            {
                StockCode = "Test1",
                AsCode = "2",
                Description = "AssociatedCharge",
                Price = 11

            };
            var associatedCharge = new List<AssociateCharge>()
            {
                associatedFirstCharge,
                associatedSecondCharge

            };
            return associatedCharge;
        }

        /// <summary>
        /// Get Restriction Test Data 
        /// </summary>
        /// <returns></returns>
        private Restriction GetRestrictionTestData()
        {
            var res = new Restriction
            {
                Code = 1,
                Description = "Test"
            };
            return res;
        }

        /// <summary>
        /// Get Product Tax exempt 
        /// </summary>
        /// <returns></returns>
        private ProductTaxExempt GetProductTaxExemptTestData()
        {
            var productTaxExempt = new ProductTaxExempt
            {
                CategoryFK = 1,
                TEVendor = "Test"
            };
            return productTaxExempt;
        }

        [SetUp]
        public void SetUp()
        {
            _policyManager.Setup(a => a.GC_REPT).Returns("1");
            _policyManager.Setup(a => a.GROUP_PRTY).Returns(true);
            _policyManager.Setup(a => a.GC_DISCOUNT).Returns(true);
            _policyManager.Setup(a => a.NUM_PRICE).Returns(1);
            _policyManager.Setup(a => a.LOYAL_PPD).Returns(1);
            _policyManager.Setup(a => a.LOYAL_DISC).Returns(1);
            _policyManager.Setup(p => p.LoadStoreInfo()).Returns(new Store {Language = "English"});
            _saleLineManager = new SaleLineManager(_resourceManager, _policyManager.Object,
                _stockService.Object, _fuelService.Object, _utilityService.Object,
                _loginManager.Object, _promoManager.Object, _stockManager.Object);
        }

        [Test]
        public void SetSaleLinePolicyTest()
        {
            var saleLine = new Sale_Line();
            var expected = 1;
            _saleLineManager.SetSaleLinePolicy(ref saleLine);
            Assert.IsTrue(saleLine.GROUP_PRTY);
            Assert.IsTrue(saleLine.GC_DISCOUNT);
            Assert.AreEqual(expected, saleLine.LOYAL_PPD);
            Assert.AreEqual(expected, saleLine.NUM_PRICE);
            Assert.AreEqual(expected, saleLine.LOYAL_DISC);
            Assert.AreEqual("1", saleLine.GC_REPT);
        }

        [Test]
        public void CreateSaleLineTest()
        {
            var expected = 1;
            var saleLine = _saleLineManager.CreateNewSaleLine();
            Assert.IsTrue(saleLine.GROUP_PRTY);
            Assert.IsTrue(saleLine.GC_DISCOUNT);
            Assert.AreEqual(expected, saleLine.LOYAL_PPD);
            Assert.AreEqual(expected, saleLine.NUM_PRICE);
            Assert.AreEqual(expected, saleLine.LOYAL_DISC);
            Assert.AreEqual("1", saleLine.GC_REPT);
        }

        //[Test]
        //public void SetPluCodeWhenPluCodeIsNotNullTest()
        //{
        //    var stockCode = "1";
        //    var expected = "1";
        //    ErrorMessage error;
        //    var sale = CreateSaleObjectTestData();
        //    var saleLine = sale.Sale_Lines.FirstOrDefault();
        //    _stockManager.Setup(a => a.GetStockDetails(It.IsAny<string>())).Returns(GetStockDetails());
        //    _promoManager.Setup(a => a.Load_Promos(It.IsAny<string>())).Returns(LoadPromoTestData());
        //    _stockManager.Setup(a => a.GetSaleLineInfo(It.IsAny<string>())).Returns(GetStockItemTestData());
        //    _policyManager.Setup(a => a.PROMO_SALE).Returns(false);
        //    _loginService.Setup(a => a.LoadSecurityInfo()).Returns(LoadSecurityInfo());
        //    _stockService.Setup(a => a.GetTax(It.IsAny<string>())).Returns(GetSaleTaxTestData());
        //    _promoManager.Setup(a => a.GetPromosForToday()).Returns(GetPromoTestData());
        //    _stockService.Setup(a => a.GetAssociateCharges(It.IsAny<string>())).Returns(GetAssociatedChargeTestData());
        //    _saleLineManager.SetPluCode(ref sale, ref saleLine, stockCode, out error, false);
        //    Assert.AreEqual(expected, saleLine.Stock_Code);

        //}

        [Test]
        public void SetPluCodeWhenPluCodeIsNullTest()
        {
            var stockCode = "1";
            var expected = "Stock Code  1  does not exist.~Stock Item Not Found";
            ErrorMessage error;
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _stockManager.Setup(a => a.GetStockDetails(It.IsAny<string>())).Returns(() => null);
            _saleLineManager.SetPluCode(ref sale, ref saleLine, stockCode, out error, false);
            Assert.AreEqual(expected, error.MessageStyle.Message);

        }


        [Test]
        public void SetSetSubDetailWhenSubDetailIsNotFuelTypeTest()
        {
            var expected = false;
            var subDetail = "test";
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _fuelService.Setup(a => a.GetFuelTypeFromDbPump(It.IsAny<string>())).Returns(() => { return null; });
            _saleLineManager.SetSubDetail(ref saleLine, subDetail);
            Assert.AreEqual(expected, saleLine.IsPropane);

        }

        [Test]
        public void SetSetSubDetaillWhenSubDetailIsFuelTypeTest()
        {
            var expected = true;
            var subDetail = "test";
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _fuelService.Setup(a => a.GetFuelTypeFromDbPump(It.IsAny<string>())).Returns(() => { return "O"; });
            _saleLineManager.SetSubDetail(ref saleLine, subDetail);
            Assert.AreEqual(expected, saleLine.IsPropane);

        }


        [Test]
        public void SetDiscountRateTest()
        {
            var expected = 0.3;
            var discountRate = 10;
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _fuelService.Setup(a => a.GetFuelTypeFromDbPump(It.IsAny<string>())).Returns(() => { return "O"; });
            _saleLineManager.SetDiscountRate(ref saleLine, discountRate);
            Assert.AreEqual(expected, saleLine.Line_Discount);

        }

        [Test]
        public void SetSetPriceTest()
        {
            var expected = 20;
            var setPrice = 10;
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _fuelService.Setup(a => a.GetFuelTypeFromDbPump(It.IsAny<string>())).Returns(() => { return "O"; });
            _saleLineManager.SetPrice(ref saleLine, setPrice);
            Assert.AreEqual(expected, saleLine.Amount);

        }


        [Test]
        public void SetQuantityTest()
        {
            var expected = 3;
            var quantity = 3;
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _fuelService.Setup(a => a.GetFuelTypeFromDbPump(It.IsAny<string>())).Returns(() => { return "O"; });
            _saleLineManager.SetQuantity(ref saleLine, quantity);
            Assert.AreEqual(expected, saleLine.Amount);

        }


        [Test]
        public void SetPriceNumberTest()
        {
            var expected = 1;
            var priceNumber = 2;
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _fuelService.Setup(a => a.GetFuelTypeFromDbPump(It.IsAny<string>())).Returns(() => { return "O"; });
            _saleLineManager.SetPriceNumber(ref saleLine, (short)priceNumber);
            Assert.AreEqual(expected, saleLine.Price_Number);
        }

        //[Test]
        //public void SetStockCode()
        //{
        //    var stockCode = "1";
        //    var expected = 'I';
        //    ErrorMessage error;
        //    var sale = CreateSaleObjectTestData();
        //    var saleLine = sale.Sale_Lines.FirstOrDefault();

        //    _stockService.Setup(a => a.GetProductTaxExempt(It.IsAny<string>())).Returns(GetProductTaxExemptTestData());
        //    _stockService.Setup(a => a.GetStockRebate(It.IsAny<string>(), It.IsAny<string>())).Returns(10);
        //    _stockManager.Setup(a => a.GetStockDetails(It.IsAny<string>())).Returns(GetStockDetails());
        //    _promoManager.Setup(a => a.Load_Promos(It.IsAny<string>())).Returns(LoadPromoTestData());
        //    _stockManager.Setup(a => a.GetSaleLineInfo(It.IsAny<string>())).Returns(GetStockItemTestData());
        //    _policyManager.Setup(a => a.TAX_EXEMPT).Returns(false);
        //    _loginService.Setup(a => a.LoadSecurityInfo()).Returns(LoadSecurityInfo());
        //    _stockService.Setup(a => a.GetTax(It.IsAny<string>())).Returns(GetSaleTaxTestData());
        //    _promoManager.Setup(a => a.GetPromosForToday()).Returns(GetPromoTestData());
        //    _stockService.Setup(a => a.GetAssociateCharges(It.IsAny<string>())).Returns(GetAssociatedChargeTestData());
        //    _saleLineManager.SetStockCode(ref sale, ref saleLine, stockCode, "X", out error, false);
        //    Assert.AreEqual(expected, saleLine.Stock_Type);

        //}

        [Test]
        public void TestApply_Table_DiscountWhenGivenDiscountISMoreThanTheMaxDiscount()
        {
            var stockCode = "1";
            var expected = "Maximum Discount is 1 % _ Discount Set to Zero~Maximum Discount Exceeded";
            ErrorMessage error;
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _policyManager.Setup(a => a.OR_USER_DISC).Returns(true);
            _policyManager.Setup(a => a.AUTO_SALE).Returns(true);
            _policyManager.Setup(a => a.GC_DISCOUNT).Returns(true);
            _policyManager.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
            _utilityService.Setup(a => a.GetDiscountPercent(It.IsAny<short>(), It.IsAny<short>())).Returns(10);
            _saleLineManager.Apply_Table_Discount(ref saleLine, 2, 2, out error);
            Assert.AreEqual(expected, error.MessageStyle.Message);

        }

        [Test]
        public void TestApply_Table_DiscountWhenGivenDiscountISLessThanTheMaxDiscount()
        {
            var expected = 10;
            ErrorMessage error;
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _policyManager.Setup(a => a.OR_USER_DISC).Returns(true);
            _policyManager.Setup(a => a.AUTO_SALE).Returns(true);
            _policyManager.Setup(a => a.GC_DISCOUNT).Returns(true);
            _policyManager.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
            _policyManager.Setup(a => a.GetPol("MAX_DISC%", It.IsAny<object>())).Returns(15);
            _utilityService.Setup(a => a.GetDiscountPercent(It.IsAny<short>(), It.IsAny<short>())).Returns(10);
            _saleLineManager.Apply_Table_Discount(ref saleLine, 2, 2, out error);
            Assert.AreEqual(expected, saleLine.Discount_Rate);

        }

        [Test]
        public void TestApplyFuelLoyaltyWhenThereIsDiscountType()
        {
            var expected = "test";
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _saleLineManager.ApplyFuelLoyalty(ref saleLine, "%", 10, "test");
            Assert.AreEqual(expected, saleLine.DiscountName);

        }

        [Test]
        public void TestApplyFuelLoyaltyWhenThereIsNoDiscountType()
        {
            //var expected = "";
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            //_policyManager.Setup(a => a.OR_USER_DISC).Returns(true);
            //_policyManager.Setup(a => a.AUTO_SALE).Returns(true);
            //_policyManager.Setup(a => a.GC_DISCOUNT).Returns(true);
            //_policyManager.Setup(a => a.GetPol(It.IsAny<string>(), It.IsAny<object>())).Returns(true);
            //_policyManager.Setup(a => a.GetPol("MAX_DISC%", It.IsAny<object>())).Returns(15);
            //_utilityService.Setup(a => a.GetDiscountPercent(It.IsAny<short>(), It.IsAny<short>())).Returns(10);
            _saleLineManager.ApplyFuelLoyalty(ref saleLine, "", 10, "test");
            Assert.IsNull(saleLine.DiscountName);

        }


        [Test]
        public void TestApplyFuelLoyaltyWhenDiscountTypeISInDollar()
        {
            var expected = "test";
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _saleLineManager.ApplyFuelLoyalty(ref saleLine, "$", 10, "test");
            Assert.AreEqual(expected, saleLine.DiscountName);

        }



        [Test]
        public void TestApplyFuelRebateWhenFuelRebateIsZero()
        {
            var expected = 0;
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _saleLineManager.ApplyFuelRebate(ref saleLine);
            Assert.AreEqual(expected, saleLine.FuelRebate);

        }

        [Test]
        public void TestApplyFuelRebateWhenFuelRebateIsGreaterThanZero()
        {
            var expected = 1;
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            saleLine.FuelRebate = 1;
            _saleLineManager.ApplyFuelRebate(ref saleLine);
            Assert.AreEqual(expected, saleLine.FuelRebate);

        }

        [Test]
        public void TestMakeGroupPrice()
        {
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _stockService.Setup(a => a.GetGroupPriceHead(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(GetGroupPriceHeadTestData());
            _stockService.Setup(a => a.GetGroupPriceLines(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
              .Returns(GetGroupPriceLineTestData());
            var actual = _saleLineManager.MakeGroupPrice(ref saleLine, "test", "test", "des");
            Assert.IsNotNull(actual);

        }

        [Test]
        public void TestGetFuelDiscountChartRateWhenChargeRateIsNotPresentInDataBase()
        {
            var expected = 0;
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _fuelService.Setup(a => a.GetDiscountRate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => null);
            var actual = _saleLineManager.GetFuelDiscountChartRate(ref saleLine, "test", 1);
            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void TestGetFuelDiscountChartRateWhenChargeRateIsPresentInDataBase()
        {
            var expected = 10;
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _fuelService.Setup(a => a.GetDiscountRate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => 10);
            var actual = _saleLineManager.GetFuelDiscountChartRate(ref saleLine, "test", 1);
            Assert.AreEqual(expected, actual);

        }

        //[Test]
        //public void TestCheckStockCondition()
        //{
        //    //var sale = CreateSaleObjectTestData();
        //    //var saleLine = sale.Sale_Lines.FirstOrDefault();
        //    ErrorMessage error;
        //    var expected1 = "No Regular Price Set for Product  1 _Defaulting to $0.01!~No Regular Price";
        //    var expected2 = "Product 1 cannot be manually entered.~Entry Not Allowed";
        //    _policyManager.Setup(a => a.Sell_Inactive).Returns(true);
        //    _policyManager.Setup(a => a.GetPol("ALLOW_ENT", It.IsAny<object>())).Returns(true);
        //    _policyManager.Setup(a => a.GetPol("U_AddStock", It.IsAny<object>())).Returns(true);
        //    _utilityService.Setup(a => a.ExistsRestriction(It.IsAny<short>())).Returns(GetRestrictionTestData());
        //    _stockManager.Setup(a => a.GetStockDetails(It.IsAny<string>())).Returns(GetStockDetails());
        //    _stockManager.Setup(a => a.GetSaleLineInfo(It.IsAny<string>())).Returns(GetStockItemTestData());
        //    _loginManager.Setup(a => a.GetExistingUser(It.IsAny<string>())).Returns((string code) => { return GetUserData(code); });
        //    var actual = _saleLineManager.CheckStockConditions(1, 1, "1", "X", false, 1, out error);
        //    Assert.AreEqual(expected1, actual.RegularPriceMessage.Message);
        //    Assert.AreEqual(expected2, actual.ManuallyEnterMessage);
        //}

        [Test]
        public void TestLoadSaleLinePolicy()
        {
            var sale = CreateSaleObjectTestData();
            var saleLine = sale.Sale_Lines.FirstOrDefault();
            _policyManager.Setup(a => a.ThirdParty).Returns(true);
            _policyManager.Setup(a => a.TAX_EXEMPT).Returns(true);
            _policyManager.Setup(a => a.GetPol("QUANT_DEC", It.IsAny<object>())).Returns(1);
            _policyManager.Setup(a => a.GetPol("PRICE_DEC", It.IsAny<object>())).Returns(1);
            _policyManager.Setup(a => a.GetPol("LOY-EXCLUDE", It.IsAny<object>())).Returns(true);
            _policyManager.Setup(a => a.GetPol("I_RIGOR", It.IsAny<object>())).Returns(true);
            _policyManager.Setup(a => a.GetPol("VOL_POINTS", It.IsAny<object>())).Returns(true);
            _policyManager.Setup(a => a.GetPol("LOYAL_PPU", It.IsAny<object>())).Returns(1);
            _policyManager.Setup(a => a.GetPol("TE_COLLECTTAX", It.IsAny<object>())).Returns("1");
            _policyManager.Setup(a => a.GetPol("GiftType", It.IsAny<object>())).Returns("1");
            _policyManager.Setup(a => a.GetPol("TrdPtyExt", It.IsAny<object>())).Returns("1");
            _policyManager.Setup(a => a.GetPol("TE_AgeRstr", It.IsAny<object>())).Returns("False");
            _saleLineManager.LoadSaleLinePolicies(ref saleLine);
            Assert.AreEqual(1, saleLine.QUANT_DEC);
            Assert.AreEqual(1, saleLine.PRICE_DEC);
            Assert.IsTrue(saleLine.LOY_EXCLUDE);
            Assert.IsTrue(saleLine.I_RIGOR);
            Assert.IsTrue(saleLine.VOL_POINTS);
            Assert.AreEqual(1, saleLine.LOYAL_PPU);
            Assert.AreEqual("1", saleLine.TE_COLLECTTAX);
            Assert.IsNull(saleLine.GiftType);
            Assert.AreEqual("1", saleLine.ThirdPartyExtractCode);
            Assert.AreEqual("False", saleLine.TE_AgeRstr);

        }
    }
}
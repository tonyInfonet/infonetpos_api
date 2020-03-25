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
    public class SaleHeadManagerTest
    {
        private Mock<ISaleService> _saleService = new Mock<ISaleService>();
        private Mock<ICustomerManager> _customerManager = new Mock<ICustomerManager>();
        private Mock<IPolicyManager> _policyManager = new Mock<IPolicyManager>();
        private SaleHeadManager _saleHeadManager;


        [SetUp]
        public void Setup()
        {
            _policyManager.Setup(a => a.USE_LOYALTY).Returns(true);
            _policyManager.Setup(a => a.LOYAL_TYPE).Returns("A");
            _policyManager.Setup(a => a.LOYAL_PRICE).Returns(2);
            _policyManager.Setup(a => a.CUST_DISC).Returns(true);
            _policyManager.Setup(a => a.LOYAL_DISC).Returns(1);
            _policyManager.Setup(a => a.PROD_DISC).Returns(true);
            _policyManager.Setup(a => a.COMBINE_LINE).Returns(true);
            _policyManager.Setup(a => a.X_RIGOR).Returns(true);
            _saleHeadManager = new SaleHeadManager(_saleService.Object, _customerManager.Object, _policyManager.Object);
        }

        /// <summary>
        /// Create New Sale
        /// </summary>
        private Sale CreateNewSaleTestData()
        {
            var sale = new Sale();
            sale.Sale_Totals.Sale_Taxes.Add("GST", "A", 2, 160, 10, 20, 20, 1, 1, "GSTA");
            sale.USE_LOYALTY = true;
            sale.LOYAL_TYPE = "";
            sale.Loyal_pricecode = 1;
            sale.CUST_DISC = false;
            sale.Loydiscode = 1;
            sale.PROD_DISC = false;
            sale.Combine_Policy = false;
            sale.XRigor = false;
            sale.Customer.Code = "Test User";
            sale.Customer.Name = "Cash Sale";
            sale.Customer.LoyaltyCard = "123";
            sale.Customer.LoyaltyCardSwiped = false;
            sale.Customer.LoyaltyExpDate = System.DateTime.Now.ToString();

            return sale;
        }

        /// <summary>
        /// Get Customers
        /// </summary>
        /// <returns></returns>
        private List<Customer> GetCustomers()
        {
            var firstcustomer = new Customer
            {
                Code = "Test User",
                Name = "Cash Sale",
                LoyaltyCard = "123",
                LoyaltyCardSwiped = false,
                LoyaltyExpDate = "System.DateTime.Now",
                GroupID = "1"
            };
            var secondcustomer = new Customer
            {
                Code = "A",
                Name = "A",
                LoyaltyCard = "123",
                LoyaltyCardSwiped = false,
                LoyaltyExpDate = "System.DateTime.Now",
                GroupID = "1"
            };

            var customers = new List<Customer>()
            {
                firstcustomer,
                secondcustomer
            };
            return customers;
        }

        /// <summary>
        /// Load All Taxes 
        /// </summary>
        private List<TaxMast> GetAllTaxMast()
        {
            var FirsttaxMast = new TaxMast()
            {
                TaxOrd = 2,
                Active = true,
                TaxApply = null,
                TaxDefination = "1",
                TaxDescription = "Goods and Services Tax",
                TaxName = "GST"
            };
            var secondtaxMast = new TaxMast()
            {
                TaxOrd = 3,
                Active = true,
                TaxApply = null,
                TaxDefination = "1",
                TaxDescription = "Provincial Sales Tax",
                TaxName = "PST"
            };

            var ListtaxMast = new List<TaxMast>
            {
                FirsttaxMast,
                secondtaxMast
            };
            return ListtaxMast;
        }

        /// <summary>
        /// Get Tax Rate by Tax Name  
        /// </summary>
        /// 
        private List<TaxRate> GetTaxRateByTaxName()
        {
            var taxRate1 = new TaxRate
            {

                TaxName = "GST",
                TaxCode = "I",
                TaxDescription = "Included GST",
                Rebate = 0,
                Rate = 5,
                Included = true
            };

            var taxRate2 = new TaxRate
            {

                TaxName = "PST",
                TaxCode = "A",
                TaxDescription = "Added PST",
                Rebate = 0,
                Rate = 10,
                Included = false
            };

            var taxRateList = new List<TaxRate>
            {
                taxRate1,
                taxRate2
            };
            return taxRateList;
        }

        [Test]
        public void SetSalePolicyTest()
        {
            var sale = CreateNewSaleTestData();
            _saleHeadManager.SetSalePolicies(ref sale);
            Assert.IsTrue(sale.USE_LOYALTY);
            Assert.AreEqual("A", sale.LOYAL_TYPE);
            Assert.AreEqual(2, sale.Loyal_pricecode);
            Assert.IsTrue(sale.CUST_DISC);
            Assert.AreEqual(1, sale.Loydiscode);
            Assert.IsTrue(sale.PROD_DISC);
            Assert.IsTrue(sale.Combine_Policy);
            Assert.IsTrue(sale.XRigor);
        }


        [Test]
        public void CreateNewSaleTest()
        {

            var taxName = "PST";
            var sale = CreateNewSaleTestData();

            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

            _saleService.Setup(u => u.GetTaxRates()).Returns(GetTaxRateByTaxName());

            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

            var actual = _saleHeadManager.CreateNewSale();

            Assert.IsNotNull(actual);
        }

        [Test]
        public void LoadTaxTest()
        {
            var taxName = "PST";
            var sale = CreateNewSaleTestData();

            _saleService.Setup(u => u.GetTaxMast()).Returns(() => { return GetAllTaxMast().Where(x => x.TaxName == taxName).ToList(); });

            _saleService.Setup(u => u.GetTaxRates()).Returns(GetTaxRateByTaxName());

            _customerManager.Setup(a => a.LoadCustomer(It.IsAny<string>())).Returns((string code) =>
            { return GetCustomers().FirstOrDefault(a => a.Code == code); });

            var actual = _saleHeadManager.CreateNewSale();

            Assert.IsNotNull(actual.Sale_Totals.Sale_Taxes);
        }
    }
}

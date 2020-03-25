//using System;
//using Infonet.CStoreCommander.ADOData;
//using Infonet.CStoreCommander.BusinessLayer.Manager;
//using Infonet.CStoreCommander.Entities;
//using Moq;
//using NUnit.Framework;
//using System.Collections.Generic;
//using Infonet.CStoreCommander.Resources;

//namespace Infonet.CStoreCommander.WebApi.Tests.Managers
//{
//    /// <summary>
//    /// Test methods for tax manager
//    /// </summary>
//    [TestFixture]
//    public class TaxManagerTest
//    {
//        private readonly Mock<ITaxService> _taxService = new Mock<ITaxService>();
//        private readonly Mock<ITreatyService> _treatyService = new Mock<ITreatyService>();
//        private readonly Mock<ITreatyManager> _treatyManager = new Mock<ITreatyManager>();
//        private readonly Mock<ILoginManager> _loginManager = new Mock<ILoginManager>();
//        private readonly Mock<ISiteMessageService> _siteMessageService = new Mock<ISiteMessageService>();
//        private readonly Mock<IAiteCardHolderService> _aiteCardHolderService = new Mock<IAiteCardHolderService>();
//        private readonly Mock<IPurchaseListManager> _purchaseListManager = new Mock<IPurchaseListManager>();
//        private readonly Mock<ITeSystemManager> _teSystemManager = new Mock<ITeSystemManager>();
//        private readonly Mock<ISaleLineManager> _saleLineManager = new Mock<ISaleLineManager>();
//        private readonly Mock<ITeCardholderManager> _cardholderManager = new Mock<ITeCardholderManager>();
//        private readonly Mock<ITaxExemptSaleLineManager> _exemptSaleLineManager = new Mock<ITaxExemptSaleLineManager>();
//        private readonly Mock<IPolicyManager> _policyManager = new Mock<IPolicyManager>();
//        private readonly IApiResourceManager _resourceManager = new ApiResourceManager();
//        private readonly Mock<ISaleManager> _saleManager = new Mock<ISaleManager>();
//        private readonly Mock<ITenderManager> _tenderManager = new Mock<ITenderManager>();
//        private readonly Mock<ICustomerManager> _customerManager = new Mock<ICustomerManager>();
//        private ITaxManager _taxManager;

//        /// <summary>
//        /// Mock data to get taxes data
//        /// </summary>
//        /// <returns>List of stock data</returns>
//        private List<StockTax> GetTaxesData()
//        {
//            var firstTax = new StockTax
//            {
//                Name = "Tax",
//                Code = "1"
//            };

//            var secondTax = new StockTax
//            {
//                Name = "Tax",
//                Code = "2"
//            };

//            return new List<StockTax>
//            {
//                firstTax,
//                secondTax
//            };
//        }

//        /// <summary>
//        /// Test method to get all taxes
//        /// </summary>
//        [Test]
//        public void GetTaxesTest()
//        {
//            var taxes = GetTaxesData();
//            _taxService.Setup(t => t.GetAllActiveTaxes()).Returns(taxes);
//            _taxManager = new TaxManager(_taxService.Object, _treatyService.Object, _treatyManager.Object,
//                _loginManager.Object, _siteMessageService.Object, _aiteCardHolderService.Object,
//                _purchaseListManager.Object, _teSystemManager.Object, _saleLineManager.Object,
//                _cardholderManager.Object, _exemptSaleLineManager.Object, _policyManager.Object,
//                _resourceManager, _saleManager.Object, _tenderManager.Object, _customerManager.Object);
//            var expected = 2;
//            var taxCode = "Tax - 1";
//            var actual = _taxManager.GetTaxes();
//            Assert.AreEqual(expected, actual.Count);
//            Assert.AreEqual(taxCode, actual[0]);
//        }

//        /// <summary>
//        /// Test method to get taxes when no taxes are present
//        /// </summary>
//        [Test]
//        public void GetTaxesForNoTaxTest()
//        {
//            _taxService.Setup(t => t.GetAllActiveTaxes()).Returns(new List<StockTax>());
//            _taxManager = new TaxManager(_taxService.Object, _treatyService.Object, _treatyManager.Object,
//                _loginManager.Object, _siteMessageService.Object, _aiteCardHolderService.Object,
//                _purchaseListManager.Object, _teSystemManager.Object, _saleLineManager.Object,
//                _cardholderManager.Object, _exemptSaleLineManager.Object, _policyManager.Object,
//                _resourceManager, _saleManager.Object, _tenderManager.Object, _customerManager.Object);
//            var expected = 0;
//            var actual = _taxManager.GetTaxes();
//            Assert.AreEqual(expected, actual.Count);
//        }
//    }
//}

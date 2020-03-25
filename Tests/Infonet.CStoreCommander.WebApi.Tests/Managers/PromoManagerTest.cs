using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

using System.Linq;

namespace Infonet.CStoreCommander.WebApi.Tests.Managers
{
    [TestFixture]
    public class PromoManagerTest
    {
        private readonly Mock<IPromoService> _promoService = new Mock<IPromoService>();
        private PromoManager _promoManger;
        private readonly IApiResourceManager _resourceManager = new ApiResourceManager();

        /// <summary>
        /// Load Promo Test Data
        /// </summary>
        /// <returns></returns>
        private List<Promo> LoadPromoTestData()
        {
            var firstPromoHead = new Promo
            {
                PromoID = "12",
                Description = "GS LAYS 180G 2 FOR 6",
                DiscType = "P",
                Amount = 6,
                MultiLink = false

            };

            var secondPromoHead = new Promo
            {
                PromoID = "13",
                Description = "GS PEPSI 2L 2 FOR 5",
                DiscType = "P",
                Amount = 10,
                MultiLink = true

            };

            var promoHead = new List<Promo>
            {
                firstPromoHead,
                secondPromoHead
            };

            return promoHead;
        }

        /// <summary>
        /// Load Promo Lines Test Data 
        /// </summary>
        /// <returns></returns>
        private List<Promo_Line> LoadPromoLineTestData()
        {
            var firstPromoline = new Promo_Line
            {
                Amount = 12,
                Stock_Code = "090939021252",
                Dept = "A",
                Sub_Dept = "B",
                Sub_Detail = "X",
                Link = 1,
                Quantity = 1
            };

            var secondPromoline = new Promo_Line
            {
                Amount = 10,
                Stock_Code = "099999000610",
                Dept = "C",
                Sub_Dept = "D",
                Sub_Detail = "Y",
                Link = 2,
                Quantity = 1
            };

            var promoLine = new List<Promo_Line>
            {
                firstPromoline,
                secondPromoline
            };
            return promoLine;
        }

        [SetUp]
        public void SetUp()
        {
            _promoManger = new PromoManager(_resourceManager, _promoService.Object);
        }

        [Test]
        public void LoadPromoTest()
        {
            var expected = 2;
            var promoId = "12";
            var links = new List<int>
            {
                1,
                2
            };
            _promoService.Setup(a => a.GetPromoHeadersForToday(promoId))
                .Returns((string promo) => LoadPromoTestData());

            _promoService.Setup(a => a.GetPromoLines(It.IsAny<string>(), It.IsAny<string>()))
               .Returns(LoadPromoLineTestData);

            //_promoService.Setup(a => a.GetNumberOfLinks(It.IsAny<string>()))
            //  .Returns(() => links);

            var actual = _promoManger.Load_Promos(promoId);
            Assert.AreEqual(expected, actual.Count);
            var firstOrDefault = actual.FirstOrDefault();
            if (firstOrDefault != null) Assert.AreEqual(expected, firstOrDefault.Promo_Lines.Count());
        }

        //[Test]
        //public void RemoveWhenPRomoCountISEqualToDataBaseTest()
        //{
        //    var expected = true;
        //    var promoId = "12";
        //    var links = new List<int>
        //    {
        //        1,
        //        2
        //    };

        //    var promoIds = new List<string>
        //    {
        //        "12",
        //        "13"
        //    };
        //    _promoService.Setup(a => a.GetPromoHeadersForToday(promoId))
        //        .Returns((string promo) => { return LoadPromoTestData(); });

        //    _promoService.Setup(a => a.GetPromoLines(It.IsAny<string>(), It.IsAny<string>()))
        //       .Returns(() => { return LoadPromoLineTestData(); });

        //    _promoService.Setup(a => a.GetDistinctPromoIdsForToday())
        //    .Returns(() => { return promoIds; });

        //    _promoService.Setup(a => a.GetNumberOfLinks(It.IsAny<string>()))
        //      .Returns(() => { return links; });

        //    var loadPromo = _promoManger.Load_Promos(promoId);
        //    var actual = _promoManger.Clear_AllPromos(ref loadPromo);
        //    Assert.AreEqual(expected,actual);
        //}

        //[Test]
        //public void RemoveWhenPRomoCountISNotEqualToDataBaseTest()
        //{
        //    var expected = false;
        //    var promoId = "12";
        //    var links = new List<int>
        //    {
        //        1,
        //        2
        //    };

        //    var promoIds = new List<string>
        //    {
        //        "12",
        //        "13",
        //        "14"
        //    };
        //    _promoService.Setup(a => a.GetPromoHeadersForToday(promoId))
        //        .Returns((string promo) => { return LoadPromoTestData(); });

        //    _promoService.Setup(a => a.GetPromoLines(It.IsAny<string>(), It.IsAny<string>()))
        //       .Returns(() => { return LoadPromoLineTestData(); });

        //    _promoService.Setup(a => a.GetDistinctPromoIdsForToday())
        //    .Returns(() => { return promoIds; });

        //    _promoService.Setup(a => a.GetNumberOfLinks(It.IsAny<string>()))
        //      .Returns(() => { return links; });

        //    var loadPromo = _promoManger.Load_Promos(promoId);
        //    var actual = _promoManger.Clear_AllPromos(ref loadPromo);
        //    Assert.AreEqual(expected, actual);
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using NUnit.Framework;
using Moq;

namespace Infonet.CStoreCommander.WebApi.Tests.Managers
{
    [TestFixture]
    class CarwashManagerTest 
    {

        private  Mock<IPolicyManager> _policyManager = new Mock<IPolicyManager>();
        private ICarwashManager _carwashManager;

        [SetUp]
        public void Setup()
        {
            _policyManager.Setup(p => p.IsCarwashIntegrated).Returns(true);
            _policyManager.Setup(p => p.IsCarwashSupported).Returns(true);
            _carwashManager = new CarwashStub(_policyManager.Object);
        }

        [Test]
        public  void GetCarwashCode()
        {
          
          var expectedOutput = true;
          var result =  _carwashManager.GetCarwashCode();
          Assert.AreEqual(expectedOutput,result);
        }

        [Test]
        public void ValidateCarwashCode()
        {
            var expectedOutput = false;
            var result = _carwashManager.ValidateCarwash("12345");
            Assert.AreEqual(result,expectedOutput);
        }

        [Test]
        public void RefundCarwash()
        {
            _carwashManager.RefundCarwash();
        }

    }
}

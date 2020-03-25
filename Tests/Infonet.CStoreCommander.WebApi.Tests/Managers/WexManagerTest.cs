using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.Resources;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.WebApi.Tests.Managers
{
    [TestFixture]
    public class WexManagerTest
    {
        private Mock<IPolicyManager> _policyManager = new Mock<IPolicyManager>();
        private Mock<IWexManager> _wexManager = new Mock<IWexManager>();
        private Mock<IApiResourceManager> _resourceManager = new  Mock<IApiResourceManager>();
        private Mock<IEncryptDecryptUtilityManager> _encryptDecryptManager = new Mock<IEncryptDecryptUtilityManager>();

        [SetUp]
        public void Setup()
        {
            _policyManager.Setup(p => p.IsCarwashIntegrated).Returns(true);
            _policyManager.Setup(p => p.IsCarwashSupported).Returns(true);
        }
    }
}

using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infonet.CStoreCommander.WebApi.Tests.Managers
{
    /// <summary>
    /// Test methods for reason manager
    /// </summary>
    [TestFixture]
    public class ReasonManagerTest
    {
        private readonly Mock<IReasonService> _reasonService = new Mock<IReasonService>();
        private readonly IApiResourceManager _resourceManager = new ApiResourceManager();
        private IReasonManager _reasonManager;

        /// <summary>
        /// Create mock data for reason
        /// </summary>
        /// <returns>List of reasons</returns>
        private List<Return_Reason> GetReasonsData()
        {
            var firstReason = new Return_Reason
            {
                Description = "Discounted",
                Reason = "1",
                RType = "D"
            };

            var secondReason = new Return_Reason
            {
                Description = "OTHERS",
                Reason = "1",
                RType = "C"
            };

            var thirdReason = new Return_Reason
            {
                Description = "LOTTERY Prize",
                Reason = "2",
                RType = "D"
            };

            return new List<Return_Reason>
            {
                firstReason,
                secondReason,
                thirdReason
            };
        }

        /// <summary>
        /// Mock data for reason types
        /// </summary>
        /// <returns>List of reason type</returns>
        private Dictionary<char, string> GetReasonTypeData()
        {
            var reasonTypes = new Dictionary<char, string> {{'D', "Discount Product"},
                { 'C', "Price Changes"}};
            return reasonTypes;
        }

        /// <summary>
        /// Mock service method to get reasons by type
        /// </summary>
        /// <param name="type">Reason Type</param>
        /// <returns>List of reasons</returns>
        private List<Return_Reason>  GetReasonsByType(char type)
        {
            var reasons = GetReasonsData();
            return reasons.Where(r => Convert.ToChar(r.RType) == type).ToList();
        }

        /// <summary>
        /// Mock method to get  reason name by type
        /// </summary>
        /// <param name="type">Reason type</param>
        /// <returns>Name of reason</returns>
        private string GetReasonTypeNameByType(char type)
        {
            var reasonTypes = GetReasonTypeData();
            var name = string.Empty;
            if (reasonTypes.ContainsKey(type))
                reasonTypes.TryGetValue(type, out name);
            return name;
        }

        /// <summary>
        /// Test method to get list of reasons
        /// </summary>
        [Test]
        public void GetReasonsTest()
        {           
            _reasonService.Setup(t => t.GetReasons(It.IsAny<char>()))
                .Returns((char type) => GetReasonsByType(type));
            _reasonManager = new ReasonManager(_reasonService.Object, _resourceManager);
            var expected = 2;
            var actual = _reasonManager.GetReasons(ReasonType.Discounts);
            Assert.AreEqual(expected, actual.Count);
        }

        /// <summary>
        /// Test method to get reasons for when no deason is defined for reason type
        /// </summary>
        [Test]
        public void GetReasonsWhenNoReasonsTest()
        {
            _reasonService.Setup(t => t.GetReasons(It.IsAny<char>()))
                .Returns((char type) => GetReasonsByType(type));
            _reasonManager = new ReasonManager(_reasonService.Object, _resourceManager);
            var expected = 1;
            var actual = _reasonManager.GetReasons(ReasonType.WriteOff);
            Assert.AreEqual(expected, actual.Count);
            var firstOrDefault = actual.FirstOrDefault();
            if (firstOrDefault != null) Assert.AreEqual("0", firstOrDefault.Reason);
        }

        /// <summary>
        /// Test method to get reason type name
        /// </summary>
        [Test]
        public void GetReasonTypeTest()
        {
            _reasonService.Setup(t => t.GetReasonType(It.IsAny<char>()))
                .Returns((char type) => GetReasonTypeNameByType(type));
            _reasonManager = new ReasonManager(_reasonService.Object, _resourceManager);
            var expected = "Price Changes";
            var actual = _reasonManager.GetReasonType(ReasonType.PriceChanges);
            Assert.AreEqual(expected,actual);
        }

        /// <summary>
        /// Test method to get reason type name if not available
        /// </summary>
        [Test]
        public void GetReasonTypeWhenTypeNotFoundTest()
        {
            _reasonService.Setup(t => t.GetReasonType(It.IsAny<char>()))
                .Returns((char type) => GetReasonTypeNameByType(type));
            _reasonManager = new ReasonManager(_reasonService.Object, _resourceManager);
            var actual = _reasonManager.GetReasonType(ReasonType.WriteOff);
            Assert.IsEmpty(actual);
        }
    }
}

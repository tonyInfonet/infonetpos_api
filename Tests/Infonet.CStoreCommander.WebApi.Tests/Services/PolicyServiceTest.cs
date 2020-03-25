//using Infonet.CStoreCommander.Data;
//using Infonet.CStoreCommander.Data.CSCAdmin;
//using Infonet.CStoreCommander.Data.Repositories;
//using Infonet.CStoreCommander.Data.ServiceClasses;
//using Moq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Infonet.CStoreCommander.WebApi.Tests.Services
//{
//    [TestFixture]
//    public class PolicyServiceTest
//    {
//        private Mock<IRepository<P_COMP>> _companyPolicyRepository = new Mock<IRepository<P_COMP>>();
//        private Mock<IRepository<P_SET>> _setPolicyRepository = new Mock<IRepository<P_SET>>();
//        private Mock<IRepository<P_CANBE>> _levelPolicyRepository = new Mock<IRepository<P_CANBE>>();
//        private IPolicyService _policyService;

//        /// <summary>
//        /// Setup
//        /// </summary>
//        [SetUp]
//        public void SetUp()
//        {
//            // Set up some testing data
//            var firstCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "Loyalty",
//                P_SEQ = 1,
//                P_DESC = "Use loyalty customer",
//                P_LEVELS = "{COMPANY}",
//                P_SET = "Yes",
//                P_NAME = "Use Loyalty",
//                P_USED = true,
//                P_VARTYPE = "L",
//                Implemented = true,
//                P_ACTIVE = true,
//                P_CHOICES = "{Yes,No}"
//            };

//            var secondCompanyPolicy = new P_COMP
//            {
//                P_CLASS = "Prac",
//                P_SEQ = 2,
//                P_DESC = "Time is printed in format",
//                P_LEVELS = "{USER,COMPANY}",
//                P_SET = "12 HOURS",
//                P_NAME = "TIMEFORMAT",
//                P_USED = true,
//                P_VARTYPE = "C",
//                Implemented = true,
//                P_ACTIVE = true,
//                P_CHOICES = "{12 HOURS,24 HOURS}"
//            };


//            var firstSetPolicy = new P_SET
//            {
//                P_LEVEL = "USER",
//                P_NAME = "VOID_AUTH",
//                P_SET1 = "Yes",
//                P_VALUE = "MANAGER"
//            };

//            var secondSetPolicy = new P_SET
//            {
//                P_LEVEL = "DEPT",
//                P_NAME = "ACCEPT_RETURN",
//                P_SET1 = "No",
//                P_VALUE = "FUEL"
//            };

//            var thirdSetPolicy = new P_SET
//            {
//                P_LEVEL = "SUB_DEPT",
//                P_NAME = "ACCEPT_RETURN",
//                P_SET1 = "Yes",
//                P_VALUE = "FUEL"
//            };


//            var firstlevelPolicy = new P_CANBE
//            {
//                P_Canbe1 = "DEPT",
//                P_NAME = "ADD_STOCK",
//                P_Seq = 3
//            };

//            var secondlevelPolicy = new P_CANBE
//            {
//                P_Canbe1 = "COMAPNY",
//                P_NAME = "ADD_CUSTOMER",
//                P_Seq = 4
//            };

//            //create company policy
//            var companyPolicies = new List<P_COMP>();
//            companyPolicies.Add(firstCompanyPolicy);
//            companyPolicies.Add(secondCompanyPolicy);

//            //create set policy
//            var setPolicies = new List<P_SET>();
//            setPolicies.Add(firstSetPolicy);
//            setPolicies.Add(secondSetPolicy);
//            setPolicies.Add(thirdSetPolicy);

//            //create level policy
//            var levelPolicies = new List<P_CANBE>();
//            levelPolicies.Add(firstlevelPolicy);
//            levelPolicies.Add(secondlevelPolicy);

//            _companyPolicyRepository.Setup(u => u.GetAll()).Returns(companyPolicies.AsQueryable());
//            _setPolicyRepository.Setup(u => u.GetAll()).Returns(setPolicies.AsQueryable());
//            _levelPolicyRepository.Setup(l => l.GetAll()).Returns(levelPolicies.AsQueryable());
//            _policyService = new PolicyService(_companyPolicyRepository.Object, _setPolicyRepository.Object,
//                                                _levelPolicyRepository.Object);

//        }

//        /// <summary>
//        /// Test to get company policy by existing policy name
//        /// </summary>
//        [Test]
//        public void GetCompanyPolicyByExistingPolicyNameTest()
//        {
//            var expected = 1;
//            var policyName = "Use Loyalty";
//            var actual = _policyService.GetCompanyPolicyByName(policyName);
//            if (actual != null)
//                Assert.AreEqual(expected, actual.P_SEQ);

//        }

//        /// <summary>
//        /// Test to get company policy for non existing policy name
//        /// </summary>
//        [Test]
//        public void GetCompanyPolicyByNonExistingPolicyNameTest()
//        {
//            P_COMP expected = null;
//            var policyName = "Fake Policy";
//            var actual = _policyService.GetCompanyPolicyByName(policyName);
//            Assert.AreEqual(expected, actual);
//        }


//        /// <summary>
//        /// Test to get company policy for null policy name
//        /// </summary>
//        [Test]
//        public void GetCompanyPolicyByNullPolicyNameTest()
//        {
//            P_COMP expected = null;
//            var actual = _policyService.GetCompanyPolicyByName(null);
//            Assert.AreEqual(expected, actual);
//        }


//        /// <summary>
//        /// Test to get company policy for empty policy name
//        /// </summary>
//        [Test]
//        public void GetCompanyPolicyByEmptyPolicyNameTest()
//        {
//            P_COMP expected = null;
//            var policyName = string.Empty;
//            var actual = _policyService.GetCompanyPolicyByName(policyName);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get policy level policy for existing policy name
//        /// </summary>
//        [Test]
//        public void GetPolicyLevelByExistingPolicyNameTest()
//        {
//            var expected = 1;
//            var policyName = "ADD_CUSTOMER";
//            var actual = _policyService.GetPolicyLevelByName(policyName).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get policy level policy for non existing policy name
//        /// </summary>
//        [Test]
//        public void GetPolicyLevelByNonExistingPolicyNameTest()
//        {
//            var expected = 0;
//            var policyName = "Fake Policy";
//            var actual = _policyService.GetPolicyLevelByName(policyName).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get policy level policy for empty policy name
//        /// </summary>
//        [Test]
//        public void GetPolicyLevelForEmptyPolicyNameTest()
//        {
//            var expected = 0;
//            var policyName = string.Empty;
//            var actual = _policyService.GetPolicyLevelByName(policyName).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get policy level policy for null policy name
//        /// </summary>
//        [Test]
//        public void GetPolicyLevelByNullPolicyNameTest()
//        {
//            var expected = 0;
//            var actual = _policyService.GetPolicyLevelByName(null).Count();
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get policy by existing policy name set at a level and value
//        /// </summary>
//        [Test]
//        public void GetSetPolicyByExistingPolicyNameTest()
//        {
//            var expected = "Yes";
//            var policyName = "VOID_AUTH";
//            var level = "USER";
//            var value = "MANAGER";
//            var actual = _policyService.GetSetPolicyByName(policyName, level, value);
//            if(actual != null)
//            Assert.AreEqual(expected, actual.P_SET1);
//        }

//        /// <summary>
//        /// Test to get policy by existing policy name set at a level and value
//        /// </summary>
//        [Test]
//        public void GetSetPolicyByNonExistingPolicyNameTest()
//        {
//            P_SET expected = null;
//            var policyName = "Fake Policy";
//            var level = "USER";
//            var value = "MANAGER";
//            var actual = _policyService.GetSetPolicyByName(policyName, level, value);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get policy by existing policy name but non existing level
//        /// </summary>
//        [Test]
//        public void GetSetPolicyByExistingPolicyNameInvalidLevelTest()
//        {
//            P_SET expected = null;
//            var policyName = "ACCEPT_RETURN";
//            var level = "Fake level";
//            var value = "FUEL";
//            var actual = _policyService.GetSetPolicyByName(policyName, level, value);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get policy by existing policy name but non existing value
//        /// </summary>
//        [Test]
//        public void GetSetPolicyByExistingPolicyNameInvalidValueTest()
//        {
//            P_SET expected = null;
//            var policyName = "ACCEPT_RETURN";
//            var level = "DEPT";
//            var value = "Fake Value";
//            var actual = _policyService.GetSetPolicyByName(policyName, level, value);
//            Assert.AreEqual(expected, actual);
//        }

//        /// <summary>
//        /// Test to get policy by a null policy name 
//        /// </summary>
//        [Test]
//        public void GetSetPolicyByNullPolicyNameTest()
//        {
//            P_SET expected = null;
//            var level = "DEPT";
//            var value = "Fake Value";
//            var actual = _policyService.GetSetPolicyByName(null, level, value);
//            Assert.AreEqual(expected, actual);
//        }


//        /// <summary>
//        /// Test to get policy by a empty policy name 
//        /// </summary>
//        [Test]
//        public void GetSetPolicyByEmptyPolicyNameTest()
//        {
//            P_SET expected = null;
//            var policyName = string.Empty;
//            var level = "DEPT";
//            var value = "Fake Value";
//            var actual = _policyService.GetSetPolicyByName(policyName, level, value);
//            Assert.AreEqual(expected, actual);

//        }
//    }
//}

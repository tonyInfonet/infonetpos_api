//using Infonet.CStoreCommander.ADOData;
//using Infonet.CStoreCommander.BusinessLayer.Manager;
//using Infonet.CStoreCommander.BusinessLayer.Utilities;
//using Infonet.CStoreCommander.Entities;
//using Moq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Infonet.CStoreCommander.Resources;

//namespace Infonet.CStoreCommander.WebApi.Tests.Managers
//{
//    /// <summary>
//    /// Test the method for Till and Shift 
//    /// </summary>
//    [TestFixture]
//    public class TillManagerTest
//    {
//        private readonly Mock<ITillService> _tillService = new Mock<ITillService>();
//        private readonly Mock<IShiftService> _shiftService = new Mock<IShiftService>();
//        private readonly Mock<ILoginManager> _loginManager = new Mock<ILoginManager>();
//        private readonly Mock<IUserService> _userService = new Mock<IUserService>();
//        private readonly Mock<IPolicyManager> _policyManager = new Mock<IPolicyManager>();
//        private readonly Mock<ISaleService> _saleService = new Mock<ISaleService>();
//        private readonly IApiResourceManager _resourceManager = new ApiResourceManager();
//        private ITillManager _tillManager;
//        List<Till> _tills;
//        List<User> _users;
//        List<ShiftStore> _shifts;

//        /// <summary>
//        /// Set the Test data for Users
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
//                Password = "Y",
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
//        /// Set the Test Data for Trainer User
//        /// </summary>
//        /// <returns></returns>
//        private User GetTrainerUser()
//        {
//            EncryptionManager encryption = new EncryptionManager();
//            var text = encryption.EncryptText("abc");
//            var firstUser = new User
//            {
//                Name = "T",
//                Code = "X",
//                epw = text,
//                Password = "T",
//                User_Group = new User_Group
//                {
//                    Code = "Trainer",
//                    Name = "Trainer",
//                    SecurityLevel = 1
//                }
//            };
//            return firstUser;
//        }

//        /// <summary>
//        /// Set the test data non trainer user
//        /// </summary>
//        /// <returns></returns>
//        private User GetNonTrainerUser()
//        {
//            EncryptionManager encryption = new EncryptionManager();
//            var text = encryption.EncryptText("abc");
//            var firstUser = new User
//            {
//                Name = "X",
//                Code = "X",
//                epw = text,
//                Password = "Y",
//                User_Group = new User_Group
//                {
//                    Code = "Manager",
//                    Name = "Manager",
//                    SecurityLevel = 1
//                }
//            };
//            return firstUser;
//        }

//        /// <summary>
//        /// Set the test Data for tills 
//        /// </summary>
//        /// <returns></returns>
//        private List<Till> GetTills()
//        {
//            var firstTill = new Till
//            {
//                Active = true,
//                Processing = true,
//                POSId = 1,
//                Date_Open = DateTime.Now,
//                Time_Open = DateTime.Now,
//                UserLoggedOn = "X",
//                Shift = 1,
//                ShiftDate = DateTime.Now,
//                Number = 1
//            };

//            var secondTill = new Till
//            {
//                Active = true,
//                Processing = false,
//                POSId = 2,
//                Date_Open = DateTime.Now,
//                Time_Open = DateTime.Now,
//                UserLoggedOn = "Y",
//                Shift = 2,
//                ShiftDate = DateTime.Now,
//                Number = 7
//            };
//            var thirdTill = new Till
//            {
//                Active = false,
//                Processing = false,
//                POSId = 2,
//                Date_Open = DateTime.Now,
//                Time_Open = DateTime.Now,
//                UserLoggedOn = "Y",
//                Shift = 2,
//                ShiftDate = DateTime.Now,
//                Number = 7
//            };

//            var tills = new List<Till>()
//        {
//            firstTill,
//            secondTill,
//            thirdTill
//        };

//            return tills;
//        }

//        /// <summary>
//        /// Set the test data non processsing tills 
//        /// </summary>
//        /// <returns></returns>
//        private Till GetNonProcessingTill()
//        {
//            return new Till
//            {
//                Active = true,
//                Processing = false,
//                Number = 1,
//                Shift = 1,
//                ShiftDate = DateTime.Today
//            };
//        }

//        /// <summary>
//        /// set the test Data for processsing tills 
//        /// </summary>
//        /// <returns></returns>
//        private Till GetProcessingTill()
//        {
//            return new Till
//            {
//                Active = true,
//                Processing = true,
//                Number = 1,
//                Shift = 1,
//                ShiftDate = DateTime.Today
//            };
//        }

//        /// <summary>
//        /// Set the test Data for Active tills
//        /// </summary>
//        /// <returns></returns>
//        private Till GetActiveTill()
//        {
//            return new Till
//            {
//                Active = false,
//                Processing = false,
//                Number = 1,
//                Shift = 1,
//                ShiftDate = DateTime.Today
//            };
//        }

//        /// <summary>
//        /// Set the test data for checkUser ID in tills
//        /// </summary>
//        /// <returns></returns>
//        private List<Till> GetCheckUserIdTillsData()
//        {
//            var firstTill = new Till
//            {
//                Active = true,
//                Processing = false,
//                POSId = 1,
//                Date_Open = DateTime.Now,
//                Time_Open = DateTime.Now,
//                UserLoggedOn = "X",
//                Shift = 1,
//                ShiftDate = DateTime.Now,
//                Number = 1
//            };

//            var secondTill = new Till
//            {
//                Active = true,
//                Processing = false,
//                POSId = 2,
//                Date_Open = DateTime.Now,
//                Time_Open = DateTime.Now,
//                UserLoggedOn = "T",
//                Shift = 2,
//                ShiftDate = DateTime.Now,
//                Number = 92
//            };
//            var thirdTill = new Till
//            {
//                Active = false,
//                Processing = false,
//                POSId = 2,
//                Date_Open = DateTime.Now,
//                Time_Open = DateTime.Now,
//                UserLoggedOn = "Y",
//                Shift = 2,
//                ShiftDate = DateTime.Now,
//                Number = 100
//            };

//            var tills = new List<Till>()
//        {
//            firstTill,
//            secondTill,
//            thirdTill
//        };

//            return tills;
//        }

//        /// <summary>
//        /// Set the test data for logged user
//        /// </summary>
//        /// <returns></returns>
//        private List<Till> GetTillsForLoggedUser()
//        {
//            var till = new Till
//            {
//                Active = true,
//                Processing = true,
//                Number = 1,
//                UserLoggedOn = "X",
//                POSId = 2
//            };
//            return new List<Till> { till };
//        }

//        /// <summary>
//        /// Set the test data for shift
//        /// </summary>
//        /// <param name="shiftNumber"></param>
//        /// <returns></returns>
//        private ShiftStore GetShiftData(int shiftNumber)

//        {
//            var shifts = GetShifts();
//            return shifts.FirstOrDefault(s => s.ShiftNumber == shiftNumber);
//        }

//        /// <summary>
//        /// Set the test data for shifts
//        /// </summary>
//        /// <returns></returns>
//        private List<ShiftStore> GetShifts()
//        {
//            var firstShift = new ShiftStore
//            {
//                Active = 1,
//                CurrentDay = 0,
//                ShiftNumber = 1,
//                StartTime = Convert.ToDateTime("1899-12-30 23:00:00.000")
//            };

//            var secondShift = new ShiftStore
//            {
//                Active = 1,
//                CurrentDay = 0,
//                ShiftNumber = 2,
//                StartTime = Convert.ToDateTime("1899-12-30 07:30:00.000")
//            };

//            var thirdShift = new ShiftStore
//            {
//                Active = 1,
//                CurrentDay = 0,
//                ShiftNumber = 3,
//                StartTime = Convert.ToDateTime("1899-12-30 15:15:00.000")
//            };

//            _shifts = new List<ShiftStore>
//            {
//                firstShift,
//                secondShift,
//                thirdShift
//            };

//            return _shifts;
//        }

//        /// <summary>
//        /// Set the Test Data for non active shifts
//        /// </summary>
//        /// <returns></returns>
//        private List<ShiftStore> GetNonActiveShifts()
//        {
//            var firstShift = new ShiftStore
//            {
//                Active = 0,
//                CurrentDay = 0,
//                ShiftNumber = 1,
//                StartTime = Convert.ToDateTime("1899-12-30 23:00:00.000")
//            };

//            return new List<ShiftStore>
//            {
//                firstShift
//            };
//        }

//        /// <summary>
//        /// Set the test data for non active and non processsing tills
//        /// </summary>
//        /// <returns></returns>
//        private List<Till> GetNonActiveAndNonProcessingTills()
//        {
//            var firstTill = new Till
//            {
//                Active = true,
//                Processing = false,
//                POSId = 1,
//                Date_Open = DateTime.Now,
//                Time_Open = DateTime.Now,
//                UserLoggedOn = "X",
//                Shift = 1,
//                ShiftDate = DateTime.Now,
//                Number = 1
//            };

//            var secondTill = new Till
//            {
//                Active = true,
//                Processing = false,
//                POSId = 1,
//                Date_Open = DateTime.Now,
//                Time_Open = DateTime.Now,
//                UserLoggedOn = "Y",
//                Shift = 2,
//                ShiftDate = DateTime.Now,
//                Number = 7
//            };

//            var tills = new List<Till>()
//        {
//            firstTill,
//            secondTill
//        };

//            return tills;
//        }

//        /// <summary>
//        /// Set the test Data for next active shift
//        /// </summary>
//        /// <returns></returns>
//        private List<ShiftStore> GetNextActiveShifts()
//        {
//            var firstShift = new ShiftStore
//            {
//                Active = 1,
//                CurrentDay = 0,
//                ShiftNumber = 1,
//                StartTime = Convert.ToDateTime("1899-12-30 23:00:00.000")
//            };

//            _shifts = new List<ShiftStore>
//            {
//                firstShift,
//            };

//            return _shifts;
//        }

//        /// <summary>
//        /// Set the test data for get tills
//        /// </summary>
//        /// <param name="posId"></param>
//        /// <param name="tillNumber"></param>
//        /// <param name="shiftDate"></param>
//        /// <param name="userCode"></param>
//        /// <param name="active"></param>
//        /// <param name="process"></param>
//        /// <returns></returns>

//        private List<Till> GetTillsData(int? posId, int? tillNumber, DateTime? shiftDate, string userCode, int? active, int? process)
//        {
//            if (posId.HasValue)
//            {
//                _tills = _tills.Where(x => x.POSId == posId).ToList();
//            }
//            if (tillNumber.HasValue)
//            {
//                _tills = _tills.Where(x => x.Number == tillNumber).ToList();
//            }
//            if (shiftDate.HasValue)
//            {
//                _tills = _tills.Where(x => x.ShiftDate == shiftDate).ToList();
//            }
//            if (!string.IsNullOrEmpty(userCode))
//            {
//                _tills = _tills.Where(x => x.UserLoggedOn == userCode).ToList();
//            }
//            if (active.HasValue)
//            {
//                _tills = _tills.Where(x => x.Active == (active == 1)).ToList();
//            }
//            if (process.HasValue)
//            {
//                _tills = _tills.Where(x => x.Processing == (process == 1)).ToList();
//            }
//            return _tills;
//        }

//        /// <summary>
//        /// Set the test data for not pay at pump till
//        /// </summary>
//        /// <param name="tillNumber"></param>
//        /// <returns></returns>
//        private List<Till> GetNotPayAtPumpTillData(int tillNumber)
//        {
//            return _tills = _tills.Where(x => (x.Number != tillNumber && x.Processing != true)).ToList();
//        }

//        /// <summary>
//        /// Set the test Data for user
//        /// </summary>
//        /// <param name="code"></param>
//        /// <returns></returns>
//        private User GetUserData(string code)
//        {
//            return _users.FirstOrDefault(x => x.Code == code);
//        }

//        /// <summary>
//        /// Set the test Data for GetTill
//        /// </summary>
//        /// <param name="tillNumber"></param>
//        /// <returns></returns>
//        private Till GetTillData(int tillNumber)
//        {
//            return _tills.FirstOrDefault(x => x.Number == tillNumber);
//        }

//        /// <summary>
//        /// Set the test Data for GetTillForUser
//        /// </summary>
//        /// <param name="active"></param>
//        /// <param name="payAtPumpTill"></param>
//        /// <param name="userCode"></param>
//        /// <returns></returns>
//        private List<Till> GetTillForUserData(int active, int payAtPumpTill, string userCode)
//        {
//            return _tills = _tills.Where((a => a.Active == ((active == 1)) && a.Number != payAtPumpTill && a.UserLoggedOn == userCode)).ToList();
//        }

//        [SetUp]
//        public void SetUp()
//        {
//            _users = GetUsers();
//            _tills = GetTills();
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                            It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                        .Returns((int? posId, int? tillNumber, DateTime? shiftDate, string userCode, int? active,
//                        int? process) => GetTillsData(posId, tillNumber, shiftDate, userCode, active, process));

//            _tillService.Setup(a => a.GetnotPayAtPumpTill(It.IsAny<int>()))
//                         .Returns((int tillNumber) => GetNotPayAtPumpTillData(tillNumber));

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                         .Returns((int tillNumber) => GetTillData(tillNumber));

//            _tillService.Setup(u => u.GetTillForUser(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//                        .Returns((int active, int payAtPumpTill, string userCode) => GetTillForUserData(active, payAtPumpTill, userCode));

//            _userService.Setup(u => u.GetUser(It.IsAny<string>()))
//                        .Returns((string code) => GetUserData(code));
//            _loginManager.Setup(l => l.GetUser(It.IsAny<string>())).Returns(GetNonTrainerUser());

//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//        }

//        /// <summary>
//        /// Test the GetTills method for positive scenerio 
//        /// </summary>
//        [Test]
//        public void GetTillsTest()
//        {
//            var expected = 0;
//            var username = "X";
//            var posId = 1;
//            ErrorMessage errorMessage;
//            int predDefinedTill = 0;
//            _loginManager.Setup(l => l.GetUser(It.IsAny<string>())).Returns(GetNonTrainerUser());
//            _loginManager.Setup(l => l.GetIpAddress(It.IsAny<int>())).Returns("100.10.00");
//            _policyManager.Setup(p => p.U_SELL).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(true);
//            _tillService.Setup(p => p.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                           It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                       .Returns(GetCheckUserIdTillsData());
//            var actual = _tillManager.GetTills(username, posId, predDefinedTill, out errorMessage);
//            Assert.AreEqual(actual.Count, expected);
//        }

//        /// <summary>
//        /// Test the is Till Available method for positive scenerio 
//        /// </summary>
//        [Test]
//        public void IsTillAvailableTest()
//        {
//            var actual = _tillManager.IsTillAvailable();
//            Assert.IsTrue(actual);

//        }

//        /// <summary>
//        /// Test the is Till Available method when no till is available
//        /// </summary>
//        [Test]
//        public void IsTillNotAvailableTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                           It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                       .Returns(_tills = new List<Till>()
//                       );
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var actual = _tillManager.IsTillAvailable();

//            Assert.IsFalse(actual);
//        }

//        /// <summary>
//        /// Test the GetTills for invalid user
//        /// </summary>
//        [Test]
//        public void GetTillsForInvalidUserTest()
//        {
//            var expected = "UserID ABC does not exist. ~No Such User";
//            var username = "ABC";
//            var posId = 1;
//            ErrorMessage errorMessage;
//            int predDefinedTill = 0;
//            User user = null;
//            _loginManager.Setup(l => l.GetUser(It.IsAny<string>())).Returns(user);
//            _loginManager.Setup(l => l.GetIpAddress(It.IsAny<int>())).Returns("100.10.00");
//            _policyManager.Setup(p => p.U_SELL).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(true);
//            _tillManager.GetTills(username, posId, predDefinedTill, out errorMessage);

//            Assert.AreEqual(errorMessage.MessageStyle.Message, expected);
//        }

//        /// <summary>
//        /// test the Get tills for Invalid pos id
//        /// </summary>
//        [Test]
//        public void GetTillsForInvalidPosIdTest()
//        {
//            var expected = "Security Alert. Check your POS IP Address!";
//            var username = "X";
//            var posId = 10;
//            ErrorMessage errorMessage;
//            int predDefinedTill = 0;
//            string ipAddress = null;
//            _loginManager.Setup(l => l.GetIpAddress(It.IsAny<int>())).Returns(ipAddress);
//            _loginManager.Setup(l => l.GetUser(It.IsAny<string>())).Returns(GetNonTrainerUser());
//            _policyManager.Setup(p => p.U_SELL).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(true);
//            _tillManager.GetTills(username, posId, predDefinedTill, out errorMessage);

//            Assert.AreEqual(errorMessage.MessageStyle.Message, expected);
//        }

//        /// <summary>
//        /// test the check force till when AutoShiftPick policy is set
//        /// </summary>
//        [Test]
//        public void CheckForceTillIfAutoShiftPickTest()
//        {
//            var expected = 0;
//            var posId = 1;
//            var user = GetNonTrainerUser();
//            var predDefinedTill = 0;
//            ErrorMessage errorMessage;
//            _loginManager.Setup(l => l.GetUser(It.IsAny<string>())).Returns(GetNonTrainerUser());
//            _loginManager.Setup(l => l.GetIpAddress(It.IsAny<int>())).Returns("100.10.00");
//            _policyManager.Setup(p => p.U_SELL).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(true);
//            _tillService.Setup(a => a.GetnotPayAtPumpTill(It.IsAny<int>()))
//                     .Returns((int tillNumber) => GetNotPayAtPumpTillData(tillNumber));
//            _tillService.Setup(p => p.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                          It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                      .Returns(_tills = GetNonActiveAndNonProcessingTills());
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var actual = _tillManager.GetTills(user.Code, posId, predDefinedTill, out errorMessage);
//            Assert.AreEqual(actual.Count, expected);
//            Assert.AreEqual(errorMessage.TillNumber, 1);
//        }

//        /// <summary>
//        /// test the check force till when AutoShiftPick policy is not set and active and process till is present
//        /// </summary>
//        [Test]
//        public void CheckForceTillIfNotAutoShiftPickAndActiveAndProcessTest()
//        {
//            var expected = 1;
//            var posId = 1;
//            ErrorMessage errorMessage;
//            var user = GetNonTrainerUser();
//            var predDefinedTill = 0;
//            _loginManager.Setup(l => l.GetUser(It.IsAny<string>())).Returns(GetNonTrainerUser());
//            _loginManager.Setup(l => l.GetIpAddress(It.IsAny<int>())).Returns("100.10.00");
//            _policyManager.Setup(p => p.U_SELL).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(false);
//            _tillService.Setup(p => p.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                           It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                       .Returns(_tills = GetNonActiveAndNonProcessingTills());
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            _tillManager.GetTills(user.Code, posId, predDefinedTill, out errorMessage);
//            Assert.AreEqual(errorMessage.TillNumber, expected);
//        }

//        /// <summary>
//        /// test the check force till when AutoShiftPick policy is not set and active and  process is not there  for given Pos ID
//        /// </summary>
//        [Test]
//        public void CheckForceTillIfNotAutoShiftPickAndIfNotActiveAndNotProcessForGivenPosIdOrPayAtPumpTest()
//        {
//            var expected = "All defined tills are currently in use.";
//            var posId = 1;
//            var user = GetNonTrainerUser();
//            var predDefinedTill = 0;
//            ErrorMessage errorMessage;
//            _policyManager.Setup(p => p.AutoShftPick).Returns(false);
//            _tillService.Setup(p => p.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                            It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                        .Returns(_tills = new List<Till>());
//            _tillService.Setup(p => p.GetnotPayAtPumpTill(It.IsAny<int>())).Returns(_tills = new List<Till>());
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            _tillManager.GetTills(user.Code, posId, predDefinedTill, out errorMessage);
//            Assert.IsTrue(errorMessage.MessageStyle.Message.Contains(expected));
//        }

//        /// <summary>
//        /// Test if IS actilve till Available positive scenario
//        /// </summary>
//        [Test]
//        public void IsActiveTillAvailableTest()
//        {
//            var posId = 1;
//            ErrorMessage errorMessage;
//            _policyManager.Setup(p => p.AutoShftPick).Returns(false);
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var actual = _tillManager.IsActiveTillAvailable(posId, out errorMessage);
//            Assert.IsTrue(actual);
//        }

//        /// <summary>
//        /// Test if  no ISactilve till Available
//        /// </summary>
//        [Test]
//        public void IfNotIsActiveAvailableTest()
//        {
//            var posId = 1;
//            ErrorMessage errorMessage;
//            _policyManager.Setup(p => p.AutoShftPick).Returns(false);
//            _tillService.Setup(p => p.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                            It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                        .Returns(_tills = new List<Till>());
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var actual = _tillManager.IsActiveTillAvailable(posId, out errorMessage);
//            Assert.IsFalse(actual);
//        }

//        /// <summary>
//        /// Test the check user id if user code is trainer and autoshfitPick is set and for user till is null
//        /// </summary>
//        [Test]
//        public void Check_UserIDIfUserCodeTrainerAndAutoShiftPickTillForUserisNullTest()
//        {
//            var user = GetTrainerUser();
//            var tills = new List<Till>();
//            var predDefinedTill = (short)0;
//            var posId = 1;
//            var errorMessage = new ErrorMessage();
//            var expected = "All defined tills are currently in use. _Define more tills or wait until one becomes available.~No Tills Available";

//            _loginManager.Setup(l => l.GetUser(It.IsAny<string>())).Returns(GetNonTrainerUser());
//            _loginManager.Setup(l => l.GetIpAddress(It.IsAny<int>())).Returns("100.10.00");
//            _policyManager.Setup(p => p.U_SELL).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(true);

//            _tillService.Setup(u => u.GetTillForUser(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//           .Returns(tills = new List<Till>()
//           );

//            _tillService.Setup(u => u.GetnotPayAtPumpTill(It.IsAny<int>()))
//          .Returns(tills = new List<Till>()
//          );

//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);

//            _tillManager.GetTills(user.Code, posId, predDefinedTill, out errorMessage);
//            Assert.AreEqual(errorMessage.MessageStyle.Message, expected);

//        }

//        /// <summary>
//        /// Test the check user id if user code is not trainer and not unlimited login policy autoshfitPick is not set 
//        /// </summary>
//        [Test]
//        public void Check_UserIDIfUserCodeNotTrainerAndIfNotUnlimitAndAutoShiftPickTillFalseTest()
//        {
//            var user = GetNonTrainerUser();
//            var posId = 1;
//            var expected = "This user is already logged on another till.";
//            _loginManager.Setup(l => l.GetUser(It.IsAny<string>())).Returns(GetNonTrainerUser());
//            _loginManager.Setup(l => l.GetIpAddress(It.IsAny<int>())).Returns("100.10.00");
//            _policyManager.Setup(p => p.U_SELL).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(false);
//            _policyManager.Setup(p => p.LogUnlimit).Returns(false);
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser()
//                         );

//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            ErrorMessage errorMessage;
//            _tillManager.GetTills(user.Code, posId, 0, out errorMessage);
//            Assert.AreEqual(errorMessage.MessageStyle.Message, expected);

//        }

//        /// <summary>
//        /// Test the check user id if user code is trainer and autoshfitPick is set and for user till is  not null
//        /// 
//        /// </summary>
//        [Test]
//        public void Check_UserIDIfUserCodeTrainerAndAutoShiftPickTillForUserisNotNullTest()
//        {
//            var user = GetTrainerUser();
//            var tills = new List<Till>();
//            var errorMessage = new ErrorMessage();
//            var expected = 0;
//            _policyManager.Setup(p => p.AutoShftPick).Returns(true);

//            _tillService.Setup(u => u.GetTillForUser(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
//           .Returns(GetTillsForLoggedUser()
//           );
//            _tillService.Setup(u => u.GetTill(It.IsAny<int>()))
//          .Returns(GetTillsForLoggedUser().FirstOrDefault()
//          );

//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);

//            // _tillManager.Check_UserID(user, Force_Till, posId, ref tills, out errorMessage);
//            var actual = tills.Count;
//            Assert.AreEqual(actual, expected);

//        }

//        /// <summary>
//        /// Test compute till when user can't sell policy is set 
//        /// </summary>
//        [Test]
//        public void ComputeTillsIfUserCannotSellTest()
//        {
//            var expected = "You are not authorized to sell products~No Authorization";
//            var posId = 1;
//            var errorMessage = new ErrorMessage();
//            var user = new User();
//            var tills = GetTills();
//            int predDefinedTill = 0;
//            _policyManager.Setup(p => p.U_SELL).Returns(false);
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            _tillManager.GetTills(user.Code, posId, predDefinedTill, out errorMessage);
//            Assert.AreEqual(errorMessage.MessageStyle.Message, expected);
//        }

//        /// <summary>
//        /// Test compute till when user can't sell policy is not set and Force till is there  
//        /// </summary>
//        [Test]
//        public void ComputeTillsIfUserCanSellAndNotForceTillTest()
//        {
//            var expected = "You are not authorized to sell products~No Authorization";
//            var posId = 1;
//            var errorMessage = new ErrorMessage();
//            var user = new User();
//            var tills = GetTills();
//            int predDefinedTill = 0;
//            _policyManager.Setup(p => p.U_SELL).Returns(false);
//            _tillService.Setup(a => a.GetnotPayAtPumpTill(It.IsAny<int>()))
//                        .Returns(GetNonActiveAndNonProcessingTills());
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            _tillManager.GetTills(user.Code, posId, predDefinedTill, out errorMessage);
//            Assert.AreEqual(errorMessage.MessageStyle.Message, expected);
//        }

//        /// <summary>
//        /// Test GetShift for Processing till positive scenerio
//        /// </summary>
//        [Test]
//        public void GetShiftsForProcesssingTillTest()
//        {
//            _tillService.Setup(u => u.GetTill(It.IsAny<int>()))
//                      .Returns(GetProcessingTill());

//            _loginManager.Setup(l => l.GetUser(It.IsAny<string>())).Returns(GetNonTrainerUser());
//            _loginManager.Setup(l => l.GetIpAddress(It.IsAny<int>())).Returns("100.10.00");
//            _policyManager.Setup(p => p.U_SELL).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(true);
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object, _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);

//            var expected = "Till - 1 is currently used by another POS!.\r\nPlease select another Till! ~Cannot select Till-1";
//            var tillNumber = 1;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay;
//            decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay, out tilllFloat);
//            Assert.AreEqual(expected, error.MessageStyle.Message);
//            Assert.IsNull(actual);
//            Assert.AreEqual(System.Net.HttpStatusCode.Conflict, error.StatusCode);
//            Assert.IsFalse(error.ShutDownPos);
//        }

//        /// <summary>
//        /// Test GetShifts for already logged in user
//        /// </summary>
//        [Test]
//        public void GetShiftsForAlreadyLoggedUserTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser());

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                       .Returns(GetNonProcessingTill());
//            _loginManager.Setup(l => l.GetUser(It.IsAny<string>())).Returns(GetNonTrainerUser());
//            _policyManager.Setup(p => p.LogUnlimit).Returns(false);

//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);

//            var expected = "This user is already logged on another till.";
//            var tillNumber = 2;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay;
//            decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay, out tilllFloat);
//            Assert.AreEqual(expected, error.MessageStyle.Message);
//            Assert.IsNull(actual);
//            Assert.AreEqual(System.Net.HttpStatusCode.Conflict, error.StatusCode);
//            Assert.IsFalse(error.ShutDownPos);
//        }

//        /// <summary>
//        /// Test shift if till is invalid 
//        /// </summary>
//        [Test]
//        public void GetShiftsForInvalidTillTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser());

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                          .Returns((int tillNo) => { return GetTillData(tillNo); });
//            _policyManager.Setup(p => p.LogUnlimit).Returns(true);

//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var expected = "Till does not exists";
//            var tillNumber = 100;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay;
//            decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay, out tilllFloat);
//            Assert.AreEqual(expected, error.MessageStyle.Message);
//            Assert.IsNull(actual);
//            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, error.StatusCode);
//            Assert.IsTrue(error.ShutDownPos);
//        }

//        /// <summary>
//        /// Test GetShift for active till 
//        /// </summary>
//        [Test]
//        public void GetShiftsForActiveTillTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser());

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                         .Returns(GetNonProcessingTill());

//            _policyManager.Setup(p => p.LogUnlimit).Returns(true);

//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var expected = 1;
//            var tillNumber = 1;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay;
//            decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay, out tilllFloat);
//            Assert.AreEqual(expected, actual.Count);
//        }

//        /// <summary>
//        /// Test GetShiftfor UseShiftPolicy is set
//        /// </summary>
//        [Test]
//        public void GetShiftsForUseShiftPolicyTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser());

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                         .Returns(GetActiveTill());

//            _policyManager.Setup(p => p.LogUnlimit).Returns(true);
//            _policyManager.Setup(p => p.USE_SHIFTS).Returns(false);

//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var expected = 1;
//            var tillNumber = 1;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay;
//            decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay,
//                out tilllFloat);
//            Assert.AreEqual(expected, actual.Count);
//        }

//        /// <summary>
//        /// Test GetShift for no active shift
//        /// </summary>
//        [Test]
//        public void GetShiftsForNoActiveShiftsTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser());

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                         .Returns(GetActiveTill());

//            _policyManager.Setup(p => p.LogUnlimit).Returns(true);
//            _policyManager.Setup(p => p.USE_SHIFTS).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(false);
//            _shiftService.Setup(s => s.GetShifts(It.IsAny<byte>())).Returns(new List<ShiftStore>());
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var expected = "You must have at least one active shift defined. Can not continue!~Terminate Application.";
//            var tillNumber = 1;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay;
//            decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay,
//                out tilllFloat);
//            Assert.AreEqual(expected, error.MessageStyle.Message);
//            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, error.StatusCode);
//            Assert.IsTrue(error.ShutDownPos);
//        }

//        /// <summary>
//        /// Test GetShift for active shift and current policy is set
//        /// </summary>
//        [Test]
//        public void GetShiftsForActiveShiftsAndCurrentPolicyTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser());

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                         .Returns(GetActiveTill());

//            _policyManager.Setup(p => p.LogUnlimit).Returns(true);
//            _policyManager.Setup(p => p.USE_SHIFTS).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(false);
//            _policyManager.Setup(p => p.SHIFT_DAY).Returns("Current");

//            _shiftService.Setup(s => s.GetShifts(It.IsAny<byte>())).Returns(GetShifts());
//            _shiftService.Setup(s => s.GetShifts(null)).Returns(GetShifts());
//            _shiftService.Setup(s => s.GetMaximumShiftNumber()).Returns(3);
//            _shiftService.Setup(s => s.UpdateShift(It.IsAny<ShiftStore>()));
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var expected = 2;
//            var tillNumber = 1;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay;
//            decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay,
//                out tilllFloat);
//            Assert.AreEqual(expected, actual.Count);
//        }

//        /// <summary>
//        /// Test GetShift for active shifts
//        /// </summary>
//        [Test]
//        public void GetShiftsForActiveShiftsTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser());

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                         .Returns(GetActiveTill());

//            _policyManager.Setup(p => p.LogUnlimit).Returns(true);
//            _policyManager.Setup(p => p.USE_SHIFTS).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(false);
//            _policyManager.Setup(p => p.SHIFT_DAY).Returns("Next");

//            _shiftService.Setup(s => s.GetShifts(It.IsAny<byte>())).Returns(GetShifts());
//            _shiftService.Setup(s => s.GetShifts(null)).Returns(GetShifts());
//            _shiftService.Setup(s => s.GetMaximumShiftNumber()).Returns(3);
//            _shiftService.Setup(s => s.UpdateShift(It.IsAny<ShiftStore>()));
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object,
//                _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var expected = 2;
//            var tillNumber = 1;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay;
//            decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay,
//                out tilllFloat);
//            Assert.AreEqual(expected, actual.Count);
//        }

//        /// <summary>
//        /// Test getNextShift for no active shift
//        /// </summary>
//        [Test]
//        public void GetShiftsForNextShiftTWithNoActiveShiftsTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser());

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                         .Returns(GetActiveTill());

//            _policyManager.Setup(p => p.LogUnlimit).Returns(true);
//            _policyManager.Setup(p => p.USE_SHIFTS).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(true);

//            _shiftService.Setup(s => s.GetShifts(It.IsAny<byte>())).Returns(new List<ShiftStore>());
//            _shiftService.Setup(s => s.UpdateShift(It.IsAny<ShiftStore>()));
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object, _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var expected = "You must have at least one active shift defined. Can not continue!~Terminate Application.";
//            var tillNumber = 1;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay;
//            decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay,
//                out tilllFloat);
//            Assert.AreEqual(expected, error.MessageStyle.Message);
//            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, error.StatusCode);
//            Assert.IsTrue(error.ShutDownPos);
//        }

//        /// <summary>
//        /// Test GetShift for hours store
//        /// </summary>
//        [Test]
//        public void GetShiftsForHourStoreTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser());

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                         .Returns(GetActiveTill());

//            _policyManager.Setup(p => p.LogUnlimit).Returns(true);
//            _policyManager.Setup(p => p.USE_SHIFTS).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(true);
//            _policyManager.Setup(p => p.Hour24Store).Returns(true);
//            _policyManager.Setup(p => p.SHIFT_DAY).Returns("Current");
//            _shiftService.Setup(s => s.GetShifts(It.IsAny<byte>())).Returns(GetShifts());
//            _tillService.Setup(s => s.GetMaximumShiftNumber(It.IsAny<DateTime>(), It.IsAny<int?>())).Returns(3);
//            _shiftService.Setup(s => s.UpdateShift(It.IsAny<ShiftStore>()));
//            _shiftService.Setup(s => s.GetNextShift(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<ShiftStore>());
//            _shiftService.Setup(s => s.GetNextActiveShift(It.IsAny<int>(), It.IsAny<int>())).Returns(GetNextActiveShifts());
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object, _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var expected = 1;
//            var tillNumber = 1;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay;
//            decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay,
//                out tilllFloat);
//            Assert.AreEqual(expected, actual.Count);
//        }

//        /// <summary>
//        /// Test GetShift for not hours with no next shift store
//        /// </summary>
//        [Test]
//        public void GetShiftsForNotHourWithNoNextShiftStoreTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser());

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                         .Returns(GetActiveTill());

//            _policyManager.Setup(p => p.LogUnlimit).Returns(true);
//            _policyManager.Setup(p => p.USE_SHIFTS).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(true);
//            _policyManager.Setup(p => p.Hour24Store).Returns(false);
//            _policyManager.Setup(p => p.SHIFT_DAY).Returns("Current");
//            _shiftService.Setup(s => s.GetShifts(It.IsAny<byte>())).Returns(GetShifts());
//            _tillService.Setup(s => s.GetMaximumShiftNumber(It.IsAny<DateTime>(), It.IsAny<int?>())).Returns(3);
//            _shiftService.Setup(s => s.UpdateShift(It.IsAny<ShiftStore>()));
//            _shiftService.Setup(s => s.GetNextShift(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<ShiftStore>());
//            _shiftService.Setup(s => s.GetNextActiveShift(It.IsAny<int>(), It.IsAny<int>())).Returns(GetNextActiveShifts());
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object, _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var expected = "All defined shifts are in use or are not active. Define more shifts or wait till one becomes available.~No Shift Available";
//            var tillNumber = 1;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay;
//            decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay,
//                out tilllFloat);
//            Assert.AreEqual(expected, error.MessageStyle.Message);
//            Assert.AreEqual(System.Net.HttpStatusCode.Conflict, error.StatusCode);
//            Assert.IsTrue(error.ShutDownPos);
//        }

//        /// <summary>
//        /// Test GetShift for not hour Shift store
//        /// </summary>
//        [Test]
//        public void GetShiftsForNotHourShiftStoreTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser());

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                         .Returns(GetActiveTill());

//            _policyManager.Setup(p => p.LogUnlimit).Returns(true);
//            _policyManager.Setup(p => p.USE_SHIFTS).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(true);
//            _policyManager.Setup(p => p.Hour24Store).Returns(false);
//            _policyManager.Setup(p => p.SHIFT_DAY).Returns("Current");
//            _shiftService.Setup(s => s.GetShifts(It.IsAny<byte>())).Returns(GetShifts());
//            _tillService.Setup(s => s.GetMaximumShiftNumber(It.IsAny<DateTime>(), It.IsAny<int?>())).Returns(3);
//            _shiftService.Setup(s => s.UpdateShift(It.IsAny<ShiftStore>()));
//            _shiftService.Setup(s => s.GetNextShift(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(GetNextActiveShifts());
//            _shiftService.Setup(s => s.GetNextActiveShift(It.IsAny<int>(), It.IsAny<int>())).Returns(GetNextActiveShifts());
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object, _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var expected = 1;
//            var tillNumber = 1;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay;
//            decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay,
//                out tilllFloat);
//            Assert.AreEqual(expected, actual.Count);
//        }

//        /// <summary>
//        /// Test GetShift for no now shfit
//        /// </summary>
//        [Test]
//        public void GetShiftsWithNoNowShiftTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser());

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                         .Returns(GetActiveTill());

//            _policyManager.Setup(p => p.LogUnlimit).Returns(true);
//            _policyManager.Setup(p => p.USE_SHIFTS).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(true);
//            _policyManager.Setup(p => p.Hour24Store).Returns(true);
//            _policyManager.Setup(p => p.SHIFT_DAY).Returns("Current");
//            _shiftService.Setup(s => s.GetShifts(It.IsAny<byte>())).Returns(GetShifts());
//            _tillService.Setup(s => s.GetMaximumShiftNumber(It.IsAny<DateTime>(), It.IsAny<int?>())).Returns(0);
//            _shiftService.Setup(s => s.UpdateShift(It.IsAny<ShiftStore>()));
//            _shiftService.Setup(s => s.GetNextShift(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(GetNextActiveShifts());
//            _shiftService.Setup(s => s.GetNextActiveShift(It.IsAny<int>(), It.IsAny<int>())).Returns(GetNextActiveShifts());
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object, _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var expected = 1;
//            var tillNumber = 1;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay;
//            decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay,
//                out tilllFloat);
//            Assert.AreEqual(expected, actual.Count);
//            Assert.IsTrue(shiftsUsedForDay);
//        }

//        /// <summary>
//        /// Test GetShifts for not curent test
//        /// </summary>
//        [Test]
//        public void GetShiftsForNotCurrentTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser());

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                         .Returns(GetActiveTill());

//            _policyManager.Setup(p => p.LogUnlimit).Returns(true);
//            _policyManager.Setup(p => p.USE_SHIFTS).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(true);
//            _policyManager.Setup(p => p.Hour24Store).Returns(true);
//            _policyManager.Setup(p => p.SHIFT_DAY).Returns("Next");
//            _shiftService.Setup(s => s.GetShifts(It.IsAny<byte>())).Returns(GetShifts());
//            _tillService.Setup(s => s.GetMaximumShiftNumber(It.IsAny<DateTime>(), It.IsAny<int?>())).Returns(3);
//            _shiftService.Setup(s => s.UpdateShift(It.IsAny<ShiftStore>()));
//            _shiftService.Setup(s => s.GetNextShift(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<ShiftStore>());
//            _shiftService.Setup(s => s.GetNextActiveShift(It.IsAny<int>(), It.IsAny<int>())).Returns(GetNextActiveShifts());
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object, _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var expected = 1;
//            var tillNumber = 1;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay;
//            decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay,
//                out tilllFloat);
//            Assert.AreEqual(expected, actual.Count);
//        }

//        /// <summary>
//        /// Test GetShift for not current and no hour store policy is not set
//        /// </summary>
//        [Test]
//        public void GetShiftsForNotCurrentAndNoHourStoreTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser());

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                         .Returns(GetActiveTill());

//            _policyManager.Setup(p => p.LogUnlimit).Returns(true);
//            _policyManager.Setup(p => p.USE_SHIFTS).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(true);
//            _policyManager.Setup(p => p.Hour24Store).Returns(false);
//            _policyManager.Setup(p => p.SHIFT_DAY).Returns("Next");
//            _shiftService.Setup(s => s.GetShifts(It.IsAny<byte>())).Returns(GetShifts());
//            _tillService.Setup(s => s.GetMaximumShiftNumber(It.IsAny<DateTime>(), It.IsAny<int?>())).Returns(3);
//            _shiftService.Setup(s => s.UpdateShift(It.IsAny<ShiftStore>()));
//            _shiftService.Setup(s => s.GetNextShift(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(GetNextActiveShifts());
//            _shiftService.Setup(s => s.GetNextActiveShift(It.IsAny<int>(), It.IsAny<int>())).Returns(GetNextActiveShifts());
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object, _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var expected = 1;
//            var tillNumber = 1;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay; decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay,
//                out tilllFloat);
//            Assert.AreEqual(expected, actual.Count);
//        }

//        /// <summary>
//        /// Test GetShift for not current and no hour store policy is not set with no active shift
//        /// </summary>
//        [Test]
//        public void GetShiftsForNotCurrentAndNoHourStoreWithNoActiveShiftTest()
//        {
//            _tillService.Setup(u => u.GetTills(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(),
//                             It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
//                         .Returns(GetTillsForLoggedUser());

//            _tillService.Setup(a => a.GetTill(It.IsAny<int>()))
//                         .Returns(GetActiveTill());

//            _policyManager.Setup(p => p.LogUnlimit).Returns(true);
//            _policyManager.Setup(p => p.USE_SHIFTS).Returns(true);
//            _policyManager.Setup(p => p.AutoShftPick).Returns(true);
//            _policyManager.Setup(p => p.Hour24Store).Returns(false);
//            _policyManager.Setup(p => p.SHIFT_DAY).Returns("Next");
//            _shiftService.Setup(s => s.GetShifts(It.IsAny<byte>())).Returns(GetShifts());
//            _tillService.Setup(s => s.GetMaximumShiftNumber(It.IsAny<DateTime>(), It.IsAny<int?>())).Returns(3);
//            _shiftService.Setup(s => s.UpdateShift(It.IsAny<ShiftStore>()));
//            _shiftService.Setup(s => s.GetNextShift(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<ShiftStore>());
//            _shiftService.Setup(s => s.GetNextActiveShift(It.IsAny<int>(), It.IsAny<int>())).Returns(GetNextActiveShifts());
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object, _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var expected = "All defined shifts are in use or are not active. Define more shifts or wait till one becomes available.~No Shift Available";
//            var tillNumber = 1;
//            var userName = "X";
//            int posId = 1;
//            ErrorMessage error;
//            bool shiftsUsedForDay;
//            decimal tilllFloat;
//            var actual = _tillManager.GetShifts(userName, tillNumber, posId, out error, out shiftsUsedForDay,
//                out tilllFloat);
//            Assert.AreEqual(expected, error.MessageStyle.Message);
//            Assert.AreEqual(System.Net.HttpStatusCode.Conflict, error.StatusCode);
//            Assert.IsTrue(error.ShutDownPos);
//        }

//        /// <summary>
//        /// Test for update till with positive scenerio
//        /// </summary>
//        [Test]
//        public void UpdateTillInformationTest()
//        {
//            var error = new ErrorMessage();
//            _loginManager.Setup(l => l.IsValidUser(It.IsAny<string>(), It.IsAny<string>(), out error)).Returns(true);
//            _loginManager.Setup(l => l.GetIpAddress(It.IsAny<int>())).Returns("172.0.0.16");
//            _tillService.Setup(t => t.GetTill(It.IsAny<int>())).Returns((int tillNo) => { return GetTillData(tillNo); });
//            _policyManager.Setup(p => p.TILL_FLOAT).Returns(true);
//            _tillService.Setup(t => t.UpdateTill(It.IsAny<Till>()));
//            _shiftService.Setup(s => s.UpdateShift(It.IsAny<ShiftStore>()));
//            _shiftService.Setup(s => s.GetShiftByShiftNumber(It.IsAny<int>())).Returns((int shiftNo) => { return GetShiftData(shiftNo); });
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object, _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);

//            var tillNumber = 1;
//            var userName = "X";
//            var password = "X";
//            var posId = 1;
//            var floatAmount = 100;
//            var shiftNumber = 1;
//            string shiftDate = DateTime.Now.ToString("MM/dd/yyyy");
//            var errorMessage = new ErrorMessage();
//            _tillManager.UpdateTillInformation(tillNumber,
//                shiftNumber, shiftDate, userName, password, posId, floatAmount, out errorMessage);
//            Assert.IsNull(errorMessage.MessageStyle.Message);
//        }

//        /// <summary>
//        /// Test for update till when invalid ip address
//        /// </summary>
//        [Test]
//        public void UpdateTillInformationForInvalidIpTest()
//        {
//            var error = new ErrorMessage();
//            _loginManager.Setup(l => l.IsValidUser(It.IsAny<string>(), It.IsAny<string>(), out error)).Returns(true);
//            _loginManager.Setup(l => l.GetIpAddress(It.IsAny<int>())).Returns(string.Empty);
//            _tillService.Setup(t => t.GetTill(It.IsAny<int>())).Returns((int tillNo) => { return GetTillData(tillNo); });
//            _policyManager.Setup(p => p.TILL_FLOAT).Returns(true);
//            _tillService.Setup(t => t.UpdateTill(It.IsAny<Till>()));
//            _shiftService.Setup(s => s.UpdateShift(It.IsAny<ShiftStore>()));
//            _shiftService.Setup(s => s.GetShiftByShiftNumber(It.IsAny<int>())).Returns((int shiftNo) => { return GetShiftData(shiftNo); });
//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object, _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var tillNumber = 1;
//            var userName = "X";
//            var password = "X";
//            var posId = 1;
//            var floatAmount = 100;
//            var shiftNumber = 1;
//            string shiftDate = DateTime.Now.ToString("MM/dd/yyyy");
//            var expected = "Security Alert. Check your POS IP Address!";
//            var errorMessage = new ErrorMessage();
//            _tillManager.UpdateTillInformation(tillNumber,
//                shiftNumber, shiftDate, userName, password, posId, floatAmount, out errorMessage);
//            Assert.AreEqual(expected, errorMessage.MessageStyle.Message);
//            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, errorMessage.StatusCode);
//            Assert.IsTrue(error.ShutDownPos);
//        }

//        /// <summary>
//        /// Test for update till when invalid till number
//        /// </summary>
//        [Test]
//        public void UpdateTillInformationForInvalidTillNumberTest()
//        {
//            var error = new ErrorMessage();
//            _loginManager.Setup(l => l.IsValidUser(It.IsAny<string>(), It.IsAny<string>(), out error)).Returns(true);
//            _loginManager.Setup(l => l.GetIpAddress(It.IsAny<int>())).Returns("172.0.0.0");
//            _tillService.Setup(t => t.GetTill(It.IsAny<int>())).Returns((int tillNo) => { return GetTillData(tillNo); });
//            _policyManager.Setup(p => p.TILL_FLOAT).Returns(true);
//            _tillService.Setup(t => t.UpdateTill(It.IsAny<Till>()));
//            _shiftService.Setup(s => s.UpdateShift(It.IsAny<ShiftStore>()));
//            _shiftService.Setup(s => s.GetShiftByShiftNumber(It.IsAny<int>())).Returns((int shiftNo) => { return GetShiftData(shiftNo); });

//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object, _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var tillNumber = 100;
//            var userName = "X";
//            var password = "X";
//            var posId = 1;
//            var floatAmount = 100;
//            var shiftNumber = 1;
//            var expected = "Till does not exists";
//            var errorMessage = new ErrorMessage();
//            string shiftDate = DateTime.Now.ToString("MM/dd/yyyy");
//            _tillManager.UpdateTillInformation(tillNumber,
//                shiftNumber, shiftDate, userName, password, posId, floatAmount, out errorMessage);
//            Assert.AreEqual(expected, errorMessage.MessageStyle.Message);
//            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, errorMessage.StatusCode);
//            Assert.IsFalse(error.ShutDownPos);
//        }

//        /// <summary>
//        /// Update till information for null shift number
//        /// </summary>
//        [Test]
//        public void UpdateTillInformationForNullShiftNumberTest()
//        {
//            var error = new ErrorMessage();
//            _loginManager.Setup(l => l.IsValidUser(It.IsAny<string>(), It.IsAny<string>(), out error)).Returns(true);
//            _loginManager.Setup(l => l.GetIpAddress(It.IsAny<int>())).Returns("172.0.0.0");
//            _tillService.Setup(t => t.GetTill(It.IsAny<int>())).Returns((int tillNo) => { return GetTillData(tillNo); });
//            _policyManager.Setup(p => p.TILL_FLOAT).Returns(true);
//            _tillService.Setup(t => t.UpdateTill(It.IsAny<Till>()));
//            _shiftService.Setup(s => s.UpdateShift(It.IsAny<ShiftStore>()));
//            _shiftService.Setup(s => s.GetShiftByShiftNumber(It.IsAny<int>())).Returns((int shiftNo) => { return GetShiftData(shiftNo); });

//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object, _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var tillNumber = 1;
//            var userName = "X";
//            var password = "X";
//            var posId = 1;
//            var floatAmount = 100;
//            var errorMessage = new ErrorMessage();
//            string shiftDate = DateTime.Now.ToString("MM/dd/yyyy");
//            var expected = "Please select a shift";
//            _tillManager.UpdateTillInformation(tillNumber,
//                null, shiftDate, userName, password, posId, floatAmount, out errorMessage);
//            Assert.AreEqual(errorMessage.MessageStyle.Message, expected);
//        }

//        /// <summary>
//        ///Test  Update till for no float amount 
//        /// </summary>
//        [Test]
//        public void UpdateTillInformationForNoFloatAmountTest()
//        {
//            var error = new ErrorMessage();
//            _loginManager.Setup(l => l.IsValidUser(It.IsAny<string>(), It.IsAny<string>(), out error)).Returns(true);
//            _loginManager.Setup(l => l.GetIpAddress(It.IsAny<int>())).Returns("172.0.0.0");
//            _tillService.Setup(t => t.GetTill(It.IsAny<int>())).Returns((int tillNo) => { return GetTillData(tillNo); });
//            _policyManager.Setup(p => p.TILL_FLOAT).Returns(false);
//            _tillService.Setup(t => t.UpdateTill(It.IsAny<Till>()));
//            _shiftService.Setup(s => s.UpdateShift(It.IsAny<ShiftStore>()));
//            _shiftService.Setup(s => s.GetShiftByShiftNumber(It.IsAny<int>())).Returns((int shiftNo) => { return GetShiftData(shiftNo); });

//            _tillManager = new TillManager(_tillService.Object, _shiftService.Object, _loginManager.Object, _userService.Object, _policyManager.Object, _resourceManager, _saleService.Object);
//            var tillNumber = 1;
//            var userName = "X";
//            var password = "X";
//            var posId = 1;
//            var floatAmount = 0;
//            var shiftNumber = 1;
//            string shiftDate = DateTime.Now.ToString("MM/dd/yyyy");
//            var errorMessage = new ErrorMessage();
//            _tillManager.UpdateTillInformation(tillNumber,
//                shiftNumber, shiftDate, userName, password, posId, floatAmount, out errorMessage);
//            Assert.IsNull(errorMessage.MessageStyle.Message);

//        }
//    }
//}
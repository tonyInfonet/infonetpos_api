using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Net;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    /// <summary>
    /// Login Manager 
    /// </summary>
    public class LoginManager : ManagerBase, ILoginManager
    {
        private readonly IApiResourceManager _resourceManager;
        private readonly IPolicyManager _policyManager;
        private readonly IUtilityService _utilityService;
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;
        private readonly ITillService _tillService;
        private readonly IShiftService _shiftService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="utilityService"></param>
        /// <param name="userService"></param>
        /// <param name="loginService"></param>
        /// <param name="resourceManager"></param>
        /// <param name="tillService"></param>
        /// <param name="shiftService"></param>
        /// <param name="policyManager"></param>
        public LoginManager(IUtilityService utilityService,
            IUserService userService,
            ILoginService loginService,
            IApiResourceManager resourceManager,
            ITillService tillService,
            IShiftService shiftService,
            IPolicyManager policyManager)
        {
            _userService = userService;
            _utilityService = utilityService;
            _loginService = loginService;
            _resourceManager = resourceManager;
            _tillService = tillService;
            _shiftService = shiftService;
            _policyManager = policyManager;
        }

        /// <summary>
        /// Authenitcate the POS
        /// </summary>
        /// <param name="ipAddress">Ip address</param>
        /// <param name="message">Message</param>
        /// <param name="error">Error message</param>
        /// <returns>Pos id</returns>
        public int Authenticate(string ipAddress, out string message, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,LoginManager,Authenticate,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            message = string.Empty;
            var posId = 0;
            if (IsValidSecurityCode(out error))
            {
                if (!string.IsNullOrEmpty(error.MessageStyle.Message))
                {
                    message = error.MessageStyle.Message;
                    error.MessageStyle.Message = string.Empty;
                }
                posId = _utilityService.GetPosId(ipAddress);
                if (posId != 0 && IsPosAllowed((byte)posId))
                {
                    Performancelog.Debug($"End,LoginManager,Authenticate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                    return posId;
                }
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8198, null, MessageType.OkOnly);
                error.StatusCode = HttpStatusCode.Unauthorized;
                error.ShutDownPos = true;
            }
            else
            {
                error.StatusCode = HttpStatusCode.Unauthorized;
                error.ShutDownPos = true;
            }
            Performancelog.Debug($"End,LoginManager,Authenticate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return posId;
        }

        /// <summary>
        /// Get Installed Date 
        /// </summary>
        /// <param name="security">Security</param>
        public void GetInstallDate(ref Security security)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,LoginManager,GetInstallDate,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var dl = "";

            var strRenamed = security.Install_Date_Encrypt;
            if (strRenamed.Length >= 60)
            {
                short i;
                for (i = 21; i <= 40; i += 2)
                {
                    dl = dl + Convert.ToString(Strings.Chr((int)(Conversion.Val(strRenamed.Substring(i - 1, 2)) - 7)));
                }

                strRenamed = DateAndTime.Month(DateTime.Parse(dl)).ToString("00") + "/" + DateAndTime.Day(DateTime.Parse(dl)).ToString("00") + "/" + DateAndTime.Year(DateTime.Parse(dl)).ToString("0000");
                security.Install_Date = DateTime.Parse(strRenamed);
                //        Me.Install_Date = Format(DL, "mm/dd/yyyy")
            }
            else
            {
                security.Install_Date = Convert.ToDateTime(DateTime.FromOADate(0));
            }
            Performancelog.Debug($"End,LoginManager,GetInstallDate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Check for valid user
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <param name="errorMessage">Error message</param>
        public bool IsValidUser(string userName, string password, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,LoginManager,IsValidUser,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            errorMessage = new ErrorMessage();
            if (string.IsNullOrEmpty(userName))
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    MessageType = MessageType.OkOnly,
                    Message = "Please provide username"
                };
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                errorMessage.ShutDownPos = false;
                Performancelog.Debug(
                    $"End,LoginManager,IsValidUser,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                errorMessage.MessageStyle = new MessageStyle
                {
                    MessageType = MessageType.OkOnly,
                    Message = "Please provide password"
                };
                errorMessage.StatusCode = HttpStatusCode.BadRequest;
                errorMessage.ShutDownPos = false;
                Performancelog.Debug(
                    $"End,LoginManager,IsValidUser,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return false;
            }
            if (!_policyManager.WINDOWS_LOGIN)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                var user = GetUser(userName.ToUpper());
                if (user == null)
                {

                    errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 10, 95, userName,
                        (MessageType)((int)MessageType.Critical + (int)MessageType.OkOnly));
                    errorMessage.StatusCode = HttpStatusCode.Unauthorized;
                    errorMessage.ShutDownPos = false;
                    Performancelog.Debug(
                        $"End,LoginManager,IsValidUser,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                    return false;
                }
                if (string.IsNullOrEmpty(user.Password) || user.Password.ToUpper().Equals(password.ToUpper())) return true;

                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 10, 92, null,
                    (MessageType)((int)MessageType.Critical + (int)MessageType.OkOnly));
                errorMessage.StatusCode = HttpStatusCode.Unauthorized;
                errorMessage.ShutDownPos = false;
                Performancelog.Debug(
                    $"End,LoginManager,IsValidUser,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the user by user code
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <returns>User</returns>
        public User GetUser(string userCode)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,LoginManager,GetUser,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            if (string.IsNullOrEmpty(userCode)) return null;
            var user = _userService.GetUser(userCode.ToUpper());
            if (user == null)
            {
                return null;
            }

            var pswd = new EncryptionManager();
            var decryptedText = pswd.DecryptText(user.epw);
            user.Password = decryptedText;

            Performancelog.Debug($"End,LoginManager,GetUser,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return user;
        }

        /// <summary>
        /// Get the user by user code in cache
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <returns>User</returns>
        public User GetExistingUser(string userCode)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,LoginManager,GetExistingUser,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            if (string.IsNullOrEmpty(userCode)) return null;
            var user = CacheManager.GetUser(userCode);
            if (user != null) return user;
            user = GetUser(userCode);
            Performancelog.Debug($"End,LoginManager,GetExistingUser,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            CacheManager.AddUser(userCode, user);
            return user;
        }


        /// <summary>
        /// Get the pos id by ip address
        /// </summary>
        /// <param name="ipAddress">Ip address</param>
        /// <returns>Pos Id</returns>
        public int GetPosId(string ipAddress)
        {
            return !string.IsNullOrEmpty(ipAddress) ? _utilityService.GetPosId(ipAddress) : 0;
        }

        /// <summary>
        /// Get the ip address by pos id 
        /// </summary>
        /// <param name="posId">Pos id</param>
        /// <returns>Ip address</returns>
        public string GetIpAddress(int posId)
        {
            return posId != 0 ? _utilityService.GetPosAddress((byte)posId) : string.Empty;
        }

        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="modelUserName">User name</param>
        /// <param name="modelPassword">Password</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>True or false</returns>
        public bool ChangePassword(string modelUserName, string modelPassword, out ErrorMessage errorMessage)
        {
            var encrypt = new EncryptionManager();
            var epw = encrypt.EncryptText(modelPassword);
            var result = _loginService.ChangePassword(modelUserName, epw);

            //MsgBox "Successfully Changed Password For the User " & Trim(Me.cbouser.Text), vbOKOnly, "Password Change"
            //Chaps_Main.DisplayMessage(user, (short)66, MsgBoxStyle.OkOnly, cobUser.Text.Trim(), (byte)0); //shiny feb3, 2010 'Trim(cobUser.List(cobUser.ListIndex))
            if (result)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 39, 66, modelUserName, MessageType.OkOnly)
                };
            }
            else
            {
                errorMessage = new ErrorMessage();
            }
            return result;
        }

        /// <summary>
        /// Change User
        /// </summary>
        /// <param name="currentUserCode">Current user code</param>
        /// <param name="userName">New user name</param>
        /// <param name="password">New user password</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <param name="shiftDate">Shift date</param>
        /// <param name="posId">POS id</param>
        /// <param name="unauthorizedAccess">Unauthorizes switch user or not</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="newUser">New user</param>
        /// <returns>True or false</returns>
        public bool ChangeUser(string currentUserCode, string userName, string password,
            int tillNumber, int? shiftNumber, string shiftDate, int posId,
            bool unauthorizedAccess, out ErrorMessage errorMessage, out string newUser)
        {
            newUser = string.Empty;
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,LoginManager,ChangeUser,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var returnValue = true;
            var isValidUser = CheckUserId(userName, password, posId, unauthorizedAccess, out errorMessage);
            if (isValidUser && !unauthorizedAccess)
            {
                shiftDate = _tillService.GetTill(tillNumber).ShiftDate.ToString("MM/dd/yyyy");
                UpdateTillInformation(tillNumber, shiftNumber, shiftDate, userName, posId, out errorMessage);
                returnValue = _loginService.UpdateLoggedInUser(userName, tillNumber);
            }
            else if (!isValidUser)
            {
                return false;
            }
            Performancelog.Debug($"End,LoginManager,ChangeUser,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            var user = _userService.GetUser(userName);
            if (user.User_Group.Code == Utilities.Constants.Trainer)
            {
                newUser = user.User_Group.Code + "-" + user.Code;
            }
            return returnValue;
        }

        /// <summary>
        /// Get password by user code
        /// </summary>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>User</returns>
        public string GetPassword(string userCode, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,LoginManager,GetPassword,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            error = new ErrorMessage();
            if (_policyManager.WINDOWS_LOGIN)
            {
                if (string.IsNullOrEmpty(userCode))
                {
                    error = new ErrorMessage
                    {
                        MessageStyle = new MessageStyle { Message = "Please provide userName" },
                        StatusCode = HttpStatusCode.NotFound
                    };
                    return null;
                }
                var user = _userService.GetUser(userCode.ToUpper());
                if (user == null)
                {
                    error = new ErrorMessage
                    {
                        MessageStyle = new MessageStyle
                        {
                            Message = "You are Not Authorized to Login",
                            MessageType = MessageType.OkOnly
                        },
                        StatusCode = HttpStatusCode.Unauthorized,
                        ShutDownPos = true

                    };
                    return null;
                }
                //Add user in cache

                var pswd = new EncryptionManager();
                var decryptedText = pswd.DecryptText(user.epw);
                user.Password = decryptedText;
                CacheManager.AddUser(userCode, user);
                Performancelog.Debug(
                    $"End,LoginManager,GetPassword,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return user.Password;
            }
            error = new ErrorMessage
            {
                MessageStyle = new MessageStyle
                {
                    Message = Utilities.Constants.InvalidRequest,
                    MessageType = MessageType.OkOnly
                },
                StatusCode = HttpStatusCode.BadRequest,
                ShutDownPos = true

            };
            return null;
        }

        #region Private methods

        /// <summary>
        /// Method to update till information
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <param name="shiftDate">Shift date</param>
        /// <param name="userName">User name</param>
        /// <param name="posId">Pos Id</param>
        /// <param name="errorMessage">Error</param>
        private void UpdateTillInformation(int tillNumber, int? shiftNumber, string shiftDate, string userName, int posId, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,UpdateTillInformation,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            errorMessage = new ErrorMessage();
            if (!string.IsNullOrEmpty(GetIpAddress(posId)))
            {
                var useShift = _policyManager.USE_SHIFTS;
                var till = _tillService.GetTill(tillNumber);
                var shift = new ShiftStore();
                if (!useShift)
                {
                    shiftNumber = null;
                }
                else
                {
                    if (shiftNumber.HasValue)
                    {
                        shift = _shiftService.GetShiftByShiftNumber(shiftNumber.Value);
                        if (shift == null && shiftNumber.Value != 0)
                        {
                            errorMessage = new ErrorMessage
                            {
                                MessageStyle = new MessageStyle
                                {
                                    Message = "Shift does not exists"
                                },
                                StatusCode = HttpStatusCode.NotFound,
                                ShutDownPos = true

                            };
                            return;
                        }
                    }
                    else
                    {
                        errorMessage = new ErrorMessage
                        {
                            MessageStyle = new MessageStyle
                            {
                                Message = "Please select a shift"
                            },
                            StatusCode = HttpStatusCode.NotFound,
                            ShutDownPos = true

                        };
                        return;
                    }
                }
                // Nicolette added to record date, time, active=true in
                // Tills table if USE_SHIFTS=No and TILL_FLOAT=No
                if (till != null)
                {
                    DateTime date;
                    DateTime.TryParse(shiftDate, out date);

                    if (!till.Active)
                    {
                        till.Date_Open = DateAndTime.Today;
                        till.Time_Open = DateAndTime.TimeOfDay;
                    }
                    till.Active = true;
                    till.Processing = true;
                    till.POSId = posId;
                    till.UserLoggedOn = userName;
                    if (useShift)
                    {
                        till.Shift = (short)shiftNumber.Value;
                        till.ShiftDate = date;
                    }
                    else
                    {
                        till.Shift = 0;
                    }
                    _tillService.UpdateTill(till);
                }
                else
                {
                    errorMessage = new ErrorMessage
                    {
                        MessageStyle = new MessageStyle
                        {
                            Message = "Till does not exists"
                        },
                        StatusCode = HttpStatusCode.NotFound,
                        ShutDownPos = true

                    };
                    return;
                }

                if (useShift && shift != null)
                {
                    if (shift.ShiftNumber == 0) return;
                    shift.Active = 1;
                    _shiftService.UpdateShift(shift);
                }
            }
            else
            {
                //"Security Alert. Check your POS IP Address!";
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8198, null, MessageType.OkOnly);
                errorMessage.StatusCode = HttpStatusCode.Unauthorized;
                errorMessage.ShutDownPos = true;
            }
            Performancelog.Debug($"End,TillManager,UpdateTillInformation,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Checks User ID
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <param name="posId">POS Id</param>
        /// <param name="unauthorizedAccess">Unauthorized access</param>
        /// <param name="message">Error</param>
        /// <returns>True or false</returns>
        private bool CheckUserId(string userName, string password, int posId,
            bool unauthorizedAccess, out ErrorMessage message)
        {
            var returnValue = true;
            message = new ErrorMessage();
            //var user = _userService.GetUser(userName);
            var user = CacheManager.GetUser(userName) ?? _userService.GetUser(userName);

            var encryptionManager = new EncryptionManager();
            if (user != null)
            {
                //Add user in Cache
                CacheManager.AddUser(userName, user);
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                if (_policyManager.LogUnlimit == false)
                {
                    //User cannot log on to more than one register unless it is a Trainer
                    if (user.User_Group.Code != "Trainer" && !unauthorizedAccess)
                    {
                        var result = _loginService.CheckLoggedinUserPos(userName, posId);

                        if (result)
                        {
                            message = new ErrorMessage
                            {
                                MessageStyle = _resourceManager.CreateMessage(offSet, 30, 95, null, CriticalOkMessageType)
                            };
                            return false;
                        }
                    }
                }
                if (!Convert.ToBoolean(_policyManager.GetPol("U_SELL", user)))
                {
                    //TIMsgbox "You are not authorized to sell products", _
                    //vbCritical + vbOKOnly, "No Authorization"
                    message = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 30, 92, null, CriticalOkMessageType)
                    };
                }
                //End - SV

                if (!Convert.ToBoolean(_policyManager.GetPol("U_REQ_PW", user)))
                {
                    return true;
                }
                if (password != encryptionManager.DecryptText(user.epw) || string.IsNullOrEmpty(password))
                {
                    message = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 30, 91, null, CriticalOkMessageType)
                    };
                    returnValue = false;
                }
            }
            else
            {
                //TIMsgbox "UserID " & txtUserid.Text & " does not exist.", vbCritical + vbOKOnly, "No Such User", Me
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                message = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 30, 93, userName, CriticalOkMessageType)
                };
                returnValue = false;
            }
            return returnValue;
        }

        /// <summary>
        /// is Pos Allowed
        /// </summary>
        /// <param name="posId">POS id</param>
        /// <returns></returns>
        private bool IsPosAllowed(byte posId)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,LoginManager,IsPosAllowed,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var returnValue = false;
            var security = _policyManager.LoadSecurityInfo();
            if (_utilityService.GetDistinctIpAddress(posId) < security.MaxConcurrentPOS)
            {
                returnValue = true;
            }
            Performancelog.Debug($"End,LoginManager,IsPosAllowed,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return returnValue;
        }

        /// <summary>
        /// Validate the security code
        /// </summary>
        /// <param name="errorMessage">Error</param>
        /// <returns>True or false</returns>
        private bool IsValidSecurityCode(out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,LoginManager,IsValidSecurityCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            errorMessage = new ErrorMessage();

            var store = _policyManager.LoadStoreInfo();

            //   to load retailerID for SITE real time validation
            // used GetPol function because Policy object is not yet created
            if (_policyManager.SITE_RTVAL)
            {
                store.RetailerID = _utilityService.GetAdminValue("RetailerAccountNo");
            }
            //   end
            var security = _policyManager.LoadSecurityInfo();
            GetInstallDate(ref security);

            var isValid = CheckSecurity(ref security, store);

            errorMessage.MessageStyle = new MessageStyle
            {
                Message = security.Message,
                MessageType = MessageType.OkOnly
            };
            Performancelog.Debug($"End,LoginManager,IsValidSecurityCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return isValid;
        }

        /// <summary>
        /// Check Security 
        /// </summary>
        /// <param name="security">Security</param>
        /// <param name="store">Store</param>
        /// <returns>True or false</returns>
        private bool CheckSecurity(ref Security security, Store store)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,LoginManager,CheckSecurity,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");


            SetPolicyFeatures(security);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (security.Security_Key.Length == 0 && security.Install_Date != DateTime.FromOADate(0)) // first time
            {
                // We dont give them permission to work without security key
                if (DateAndTime.DateDiff(DateInterval.Day, security.Install_Date, DateTime.Now) < 0 && security.Install_Date <= DateAndTime.Today) // installation time
                {
                    security.Message = $"System Is Going To Shut Down in {5 - DateAndTime.DateDiff("d", security.Install_Date, DateTime.Now)}  days";
                    security.ExpireDate = DateTime.FromOADate(security.Install_Date.ToOADate() + 5);
                    return true;
                }
                //"License Expired. Please get a Valid Security Code from your Dealer."
                security.Message = _resourceManager.CreateCaption(offSet, 5, 83, null, 0);
                return false; // installation time is over
            }
            if (security.Install_Date == DateTime.FromOADate(0) || security.Install_Date > DateAndTime.Today)
            {
                //"You don't have permission to login to POS. Please contact your Dealer.";
                security.Message = _resourceManager.CreateCaption(offSet, 6, 83, null, 0);
                return false;

            }
            if (security.Security_Key.Length > 0)
            {

                //"You don't have permission to login to POS. Please contact your Dealer.";
                security.Message = _resourceManager.CreateCaption(offSet, 6, 83, null, 0);

                var secStr = Convert.ToString(store.Name);
                secStr = secStr + store.Address.Street1;
                secStr = secStr + store.Address.City;
                secStr = secStr + store.Address.PostalCode;
                secStr = secStr + store.RegNum;
                secStr = secStr + DateAndTime.Month(security.Install_Date).ToString("00") + "/" +
                         DateAndTime.Day(security.Install_Date).ToString("00") + "/" +
                         DateAndTime.Year(security.Install_Date).ToString("0000");
                secStr = secStr + security.POS_BO_Features;
                secStr = secStr + security.Pump_Features;







                var secStrNoNic = secStr;
                secStr = secStr + security.NIC_Number;
                var strTmp = security.Number_OF_POS.ToString();
                strTmp = strTmp + Convert.ToString(security.MaxConcurrentPOS);
                strTmp = strTmp + _utilityService.GetIpAddresses();
                secStr = secStr + strTmp;
                secStrNoNic = secStrNoNic + strTmp;


                short i;
                for (i = 1; i <= security.Limit.Length - 1; i++)
                {
                    if (Helper.GetKey(secStr + security.Limit[i].Trim()) == security.Security_Key)
                    {
                        switch (security.Limit[i].Trim())
                        {
                            case "30":
                            case "45":
                            case "60":
                            case "75":
                            case "90":
                            case "120":
                            case "180":
                                if (DateAndTime.DateDiff(DateInterval.Day, security.Install_Date, DateTime.Now) <= Conversion.Val(security.Limit[i]))
                                {
                                    // $"System Is Going To Shut Down in {Convert.ToInt64(security.Limit[i]) - DateAndTime.DateDiff("d", security.Install_Date, DateTime.Now)}  days";

                                    security.Message = _resourceManager.CreateCaption(offSet, 4, 83, Conversion.Val(security.Limit[i]) - DateAndTime.DateDiff(DateInterval.Day, security.Install_Date, DateTime.Now), 0);
                                    security.ExpireDate = DateTime.FromOADate(security.Install_Date.ToOADate() + Conversion.Val(security.Limit[i]));
                                    return true;
                                }
                                //"License Expired. Please Contact your Dealer.";
                                security.Message = _resourceManager.CreateCaption(offSet, 7, 83, null, 0);
                                return false;
                            case "PAID":
                                security.Message = string.Empty;
                                security.ExpireDate = DateTime.FromOADate(security.Install_Date.ToOADate() + 50 * 365); // 50 Year
                                return true;
                        }
                    }
                }



                if (Helper.GetKey(secStrNoNic + "3") == security.Security_Key)
                {
                    if (DateAndTime.DateDiff(DateInterval.Day, security.Install_Date, DateTime.Now) <= 3)
                    {

                        var strInsDate = LoadTempInstallDate();
                        if (string.IsNullOrEmpty(strInsDate))
                        {
                            SaveTempInstallDate(security.Install_Date);


                        }
                        else if (strInsDate != security.Install_Date.ToString("MM/dd/yyyy"))
                        {
                            //"You don't have permission to login to POS. Please contact your Dealer.";
                            security.Message = _resourceManager.CreateCaption(offSet, 6, 83, null, 0);
                            return false;
                        }


                        //$"System Is Going To Shut Down in {3 - DateAndTime.DateDiff("d", security.Install_Date, DateTime.Now)}  days";
                        security.Message = _resourceManager.CreateCaption(offSet, 4, 83, 3 - DateAndTime.DateDiff(DateInterval.Day, security.Install_Date, DateTime.Now), 0);
                        security.ExpireDate = DateTime.FromOADate(security.Install_Date.ToOADate() + 3);
                        return true;
                    }
                    //"License Expired. Please Contact your Dealer.";
                    security.Message = _resourceManager.CreateCaption(offSet, 7, 83, null, 0);
                    return false;
                }

            }
            Performancelog.Debug($"End,LoginManager,CheckSecurity,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return false;
        }

        /// <summary>
        /// Set Policy features
        /// </summary>
        /// <param name="security">Security</param>
        private void SetPolicyFeatures(Security security)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,LoginManager,SetPolicyFeatures,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            _policyManager.SetUpPolicy(security);
            Performancelog.Debug($"End,LoginManager,SetPolicyFeatures,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Load temp install date
        /// </summary>
        /// <returns>Instal date</returns>
        private string LoadTempInstallDate()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,LoginManager,LoadTempInstallDate,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var returnValue = _utilityService.GetAdminValue("ExemptCode");
            if (!string.IsNullOrEmpty(returnValue))
            {
                returnValue = Encrypt(returnValue);
            }
            else
            {
                Performancelog.Debug($"End,LoginManager,LoadTempInstallDate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return string.Empty;
            }
            Performancelog.Debug($"End,LoginManager,LoadTempInstallDate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }


        /// <summary>
        /// save temp Install Date
        /// </summary>
        /// <param name="insDate">Install date</param>
        private void SaveTempInstallDate(DateTime insDate)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,LoginManager,SaveTempInstallDate,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var strTmp = insDate.ToString("MM/dd/yyyy");
            var value = Encrypt(strTmp);
            _utilityService.SaveAdminValue("ExemptCode", value);
            Performancelog.Debug($"End,LoginManager,SaveTempInstallDate,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        // changed from private function to public so encrypt() can be used
        /// <summary>
        /// Encrypt the string 
        /// </summary>
        /// <param name="strInput">Input</param>
        /// <returns>Encrypted string</returns>
        private string Encrypt(string strInput)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,LoginManager,Encrypt,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            int iCount;
            int lngPtr = 0;

            var strKey = "gb$171a?9v@4tFeD<p";
            for (iCount = 1; iCount <= strInput.Length; iCount++)
            {
                StringType.MidStmtStr(ref strInput, iCount, 1, Strings.Chr(Convert.ToInt32(Strings.Asc(strInput.Substring(iCount - 1, 1)) ^ Strings.Asc(strKey.Substring(lngPtr + 1 - 1, 1)))).ToString());
                lngPtr = Convert.ToInt32((lngPtr + 1) % strKey.Length);
            }
            var returnValue = strInput;
            Performancelog.Debug($"End,LoginManager,Encrypt,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        #endregion
    }
}

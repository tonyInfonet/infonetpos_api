using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using User = Infonet.CStoreCommander.Entities.User;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    /// <summary>
    /// Till manager
    /// </summary>
    public class TillManager : ManagerBase, ITillManager
    {
        private readonly ITillService _tillService;
        private readonly IShiftService _shiftService;
        private readonly ILoginManager _loginManager;
        private readonly IUserService _userService;
        private readonly IPolicyManager _policyManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ISaleService _saleService;


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="tillService"></param>
        /// <param name="shiftService"></param>
        /// <param name="loginManager"></param>
        /// <param name="userService"></param>
        /// <param name="policyManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="saleService"></param>
        public TillManager(ITillService tillService,
            IShiftService shiftService,
            ILoginManager loginManager,
            IUserService userService,
            IPolicyManager policyManager,
            IApiResourceManager resourceManager,
            ISaleService saleService)
        {
            _tillService = tillService;
            _shiftService = shiftService;
            _loginManager = loginManager;
            _userService = userService;
            _policyManager = policyManager;
            _resourceManager = resourceManager;
            _saleService = saleService;
        }

        /// <summary>
        /// Check for Available Tills
        /// </summary>
        /// <returns></returns>
        public bool IsTillAvailable()
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,IsTillAvailable,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var tills = _tillService.GetTills(null, null, null, null, null, null);
            if (tills == null || tills.Count == 0)
            {
                Performancelog.Debug($"End,TillManager,IsTillAvailable,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return false;
            }
            Performancelog.Debug($"End,TillManager,IsTillAvailable,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return true;
        }

        /// <summary>
        /// Get Till numbers
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="posId"></param>
        /// <param name="predDefinedTill"></param>
        /// <param name="errorMessage"></param>
        /// <returns>Till numbers</returns>
        public List<int> GetTills(string userName, int posId, int predDefinedTill, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,GetTills,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var user = _loginManager.GetUser(userName);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var tills = new List<int>();
            if (user != null)
            {
                if (!string.IsNullOrEmpty(_loginManager.GetIpAddress(posId)))
                {
                    if (!_policyManager.GetPol("U_SELL", user))
                    {
                        errorMessage = new ErrorMessage
                        {
                            //"You are not authorised to sell products",
                            MessageStyle = _resourceManager.CreateMessage(offSet, 10, 93, null, (MessageType)((int)MsgBoxStyle.Critical + (int)MsgBoxStyle.OkOnly)),
                            StatusCode = HttpStatusCode.Unauthorized,
                            ShutDownPos = true
                        };
                        Performancelog.Debug($"End,TillManager,ComputeTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                        return null;
                    }
                    tills = GetAllTills(user, posId, predDefinedTill, out errorMessage);
                }
                else
                {
                    errorMessage = new ErrorMessage
                    {
                        //"Security Alert. Check your POS IP Address!",
                        MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8198, null, MessageType.OkOnly),
                        StatusCode = HttpStatusCode.Unauthorized,
                        ShutDownPos = true
                    };
                }
            }
            else
            {
                if (_policyManager.WINDOWS_LOGIN)
                {
                    errorMessage = new ErrorMessage
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
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 10, 95, userName, MessageType.OkOnly),
                    StatusCode = HttpStatusCode.Unauthorized,
                    ShutDownPos = false
                };
            }
            Performancelog.Debug($"End,TillManager,GetTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            if (user != null && user.User_Group.Code != Utilities.Constants.Trainer && tills != null)
                tills = tills.Where(t => t < Utilities.Constants.TrainFirstTill).ToList();
            return tills;
        }

        /// <summary>
        /// Check for Active Till Available 
        /// </summary>
        /// <param name="posId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool IsActiveTillAvailable(int posId, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,IsActiveTillAvailable,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            errorMessage = new ErrorMessage();
            const int payAtPumpTill = 100;

            var tills = _tillService.GetTills(posId, null, null, null, 1, 1);

            var storeTills = _tillService.GetnotPayAtPumpTill(payAtPumpTill);

            storeTills = storeTills.Where(x => !x.Processing).ToList();

            if (tills != null && tills.Count != 0)
            {
                Performancelog.Debug($"End,TillManager,IsActiveTillAvailable,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return true;
            }
            if (storeTills.Count != 0)
            {
                Performancelog.Debug($"End,TillManager,IsActiveTillAvailable,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return true;
            }
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            errorMessage = new ErrorMessage
            {

                //"All defined tills are currently in use. Define more tills or wait until one becomes available.",
                MessageStyle = _resourceManager.CreateMessage(offSet, 10, 91, null),
                StatusCode = HttpStatusCode.NotFound,
                ShutDownPos = true
            };

            Performancelog.Debug($"End,TillManager,IsActiveTillAvailable,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return false;
        }

        /// <summary>
        /// Check for Force Till
        /// </summary>
        /// <param name="posId"></param>
        /// <param name="user"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private int CheckForceTill(int posId, User user, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,CheckForceTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var forceTill = 0;
            errorMessage = new ErrorMessage();
            if (_policyManager.AutoShftPick)
            {
                forceTill = 0;
            }

            else
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                //regular way
                const int payAtPumpTill = 100;
                var tills = _tillService.GetTills(posId, null, null, null, 1, 1);
                var storeTills = _tillService.GetnotPayAtPumpTill(payAtPumpTill);
                storeTills = storeTills.Where(x => !x.Processing).ToList();
                if (tills == null || tills.Count == 0)
                {
                    if (storeTills.Count != 0) return forceTill;
                    errorMessage = new ErrorMessage
                    {
                        //"All defined tills are currently in use. Define more tills or wait until one becomes available.",
                        MessageStyle = _resourceManager.CreateMessage(offSet, 10, 91, null),
                        StatusCode = HttpStatusCode.NotFound,
                        ShutDownPos = true
                    };

                    return forceTill;
                }
                tills = string.Equals(user.User_Group.Code, Utilities.Constants.Trainer, StringComparison.CurrentCultureIgnoreCase)
                    ? tills.Where(t => t.Number >= double.Parse(Entities.Constants.TrainFirstTill)).ToList() :
                      tills.Where(t => t.Number < double.Parse(Entities.Constants.TrainFirstTill)).ToList();
                var firstOrDefault = tills.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    forceTill = Convert.ToInt32(firstOrDefault.Number);
                    errorMessage = new ErrorMessage
                    {

                        //$"Forcing Till Number {forceTill} because _ an incomplete sale was found for that till",
                        MessageStyle = _resourceManager.CreateMessage(offSet, 10, 94, forceTill, MessageType.OkOnly),
                        StatusCode = HttpStatusCode.OK,
                        ShutDownPos = false,
                        TillNumber = forceTill,
                        ShiftNumber = firstOrDefault.Shift,
                        FloatAmount = firstOrDefault.Float,
                        ShiftDate = firstOrDefault.ShiftDate.ToString("MM/dd/yyyy")
                    };
                }
            }
            Performancelog.Debug($"End,TillManager,CheckForceTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return forceTill;
        }

        /// <summary>
        /// Get All till number 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="posId"></param>
        /// <param name="preDefinedTill"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private List<int> GetAllTills(User user, int posId, int preDefinedTill, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,GetAllTills,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var availableTills = new List<int>();



            const int payAtPumpTill = 100;
            var forceTill = (short)CheckForceTill(posId, user, out errorMessage);
            var storeTills = _tillService.GetnotPayAtPumpTill(payAtPumpTill);
            storeTills = storeTills.Where(x => !x.Processing).ToList();
            if (!IsActiveTillAvailable(posId, out errorMessage))
            {
                return null;
            }

            //if (forceTill != 0
            //    && (forceTill < Constants.TrainFirstTill && user.User_Group.Code != Constants.Trainer)
            //   && (forceTill >= Constants.TrainFirstTill && user.User_Group.Code == Constants.Trainer))
            //{

            //    
            //    var till = _tillService.GetTill(forceTill);
            //    availableTills.Add(forceTill);
            //    errorMessage = new ErrorMessage
            //    {
            //        //$"Forcing Till Number {forceTill} because _ an incomplete sale was found for that till",
            //        MessageStyle = _resourceManager.CreateMessage(offSet,10, 94, forceTill, MessageType.OkOnly),
            //        StatusCode = HttpStatusCode.OK,
            //        ShutDownPos = false,
            //        TillNumber = forceTill,
            //        ShiftNumber = till.Shift,
            //        FloatAmount = till.Float,
            //        ShiftDate = till.ShiftDate.ToString("MM/dd/yyyy")
            //    };
            //    Performancelog.Debug(
            //        $"End,TillManager,GetAllTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            //    return availableTills;
            //}
            //forceTill = 0;

            //var tills = storeTills.Where(t => t.Number < Convert.ToInt32(modNRGT.TrainFirstTill)).ToList();
            var tills = new List<Till>(storeTills);
            //ComputeTills(user, posId, forceTill, preDefinedTill, ref tills, out errorMessage);
            Check_UserID(user, ref forceTill, posId, ref tills, out errorMessage);
            if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                ComputeTills(user, posId, forceTill, preDefinedTill, ref tills, out errorMessage);
            }

            if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message) || errorMessage.StatusCode == HttpStatusCode.Ambiguous)
            {
                availableTills.AddRange(tills.Select(t => Convert.ToInt32(t.Number)).ToList());
            }
            Performancelog.Debug(
                $"End,TillManager,GetAllTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return availableTills;

        }

        /// <summary>
        /// Compute Tills 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="posId"></param>
        /// <param name="forceTill"></param>
        /// <param name="predDefinedTill"></param>
        /// <param name="tills"></param>
        /// <param name="errorMessage"></param>
        private void ComputeTills(User user, int posId, int forceTill, int predDefinedTill, ref List<Till> tills, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,ComputeTills,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            short iTill = 0;
            var capValue = new object[4];
            var capValueNoSh = new object[3];
            errorMessage = new ErrorMessage();
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (forceTill == 0)
            {
                // more than one till is defined, active and not processing
                if (tills.Count > 1)
                {

                    if (string.Equals(user.User_Group.Code, Utilities.Constants.Trainer, StringComparison.CurrentCultureIgnoreCase)) //Behrooz Jan-12-06
                    {
                        errorMessage = new ErrorMessage
                        {
                            MessageStyle = new MessageStyle
                            {
                                Message = "This User (" + user.Name + ") is a Trainer User" + "\r\n" + "You are going to run POS in Trainer Mode. ~Trainer Mode",
                                MessageType = MessageType.OkOnly
                            },
                            ShutDownPos = false,
                            StatusCode = HttpStatusCode.Ambiguous

                        };
                        var userTills = new List<Till>(tills);
                        tills.Clear();

                        foreach (var till in userTills)
                        {
                            if (till.Number >= Convert.ToInt32(Utilities.Constants.TrainFirstTill) && till.Number <= Convert.ToInt32(Entities.Constants.TrainLastTill))
                            {
                                tills.Add(till);
                            }

                        }

                    }
                    // system picks predefined till in DataPath.ini
                    if (_policyManager.TILL_NUM)
                    {
                        iTill = (short)predDefinedTill;
                        if (_tillService.GetTill(iTill) == null)
                        {
                            errorMessage = new ErrorMessage
                            {
                                MessageStyle = new MessageStyle
                                {
                                    Message = "Predefined tillnumber does not exists"
                                },
                                StatusCode = HttpStatusCode.NotFound,
                                ShutDownPos = true

                            };
                            Performancelog.Debug($"End,TillManager,ComputeTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                            return;
                        }
                        WriteToLogFile("Till number set based on DataPath.ini is " + Convert.ToString(iTill));
                        if (!_policyManager.AutoShftPick)
                        {
                            if (IsProcessing(iTill, posId))
                            {
                                errorMessage = new ErrorMessage
                                {
                                    MessageStyle = new MessageStyle
                                    {
                                        Message = string.Format("Till - " + Convert.ToString(iTill) + " is currently used by another POS!." + "\r\n" + "Please select another Till! ~Cannot select Till-" + Convert.ToString(iTill)),
                                        MessageType = (MessageType)((int)MsgBoxStyle.Critical + (int)MsgBoxStyle.OkOnly)
                                    },
                                    StatusCode = HttpStatusCode.Conflict,
                                    ShutDownPos = false

                                };
                            }
                        }
                        tills.Clear();
                        tills.Add(new Till
                        {
                            Number = iTill
                        });
                    }

                    //tills.AddRange(userTills);
                }

                else
                {
                    if (_policyManager.TILL_NUM)
                    {
                        iTill = (short)predDefinedTill;
                        if (_tillService.GetTill(iTill) == null)
                        {
                            errorMessage = new ErrorMessage
                            {
                                MessageStyle = new MessageStyle
                                {
                                    Message = "Predefined tillnumber does not exists"
                                },
                                StatusCode = HttpStatusCode.NotFound,
                                ShutDownPos = true

                            };
                            Performancelog.Debug($"End,TillManager,ComputeTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                            return;
                        }
                        WriteToLogFile("Till number set based on DataPath.ini is " + Convert.ToString(iTill));
                    }
                    else
                    {
                        if (tills.Count != 0)
                        {
                            var firstOrDefault = tills.FirstOrDefault();
                            if (firstOrDefault != null) iTill = Convert.ToInt16(firstOrDefault.Number);
                        }
                    }
                    if (!_policyManager.AutoShftPick)
                    {
                        if (IsProcessing(iTill, posId))
                        {
                            errorMessage = new ErrorMessage
                            {
                                MessageStyle = new MessageStyle
                                {
                                    Message = string.Format("Till - " + Convert.ToString(iTill) + " is currently used by another POS!." + "\r\n" + "Please select another Till! ~Cannot select Till-" + Convert.ToString(iTill)),
                                    MessageType = CriticalOkMessageType
                                },
                                StatusCode = HttpStatusCode.Conflict,
                                ShutDownPos = false
                            };
                        }
                    }
                    // Check if the till was not closed since yesterday    
                    var rsTemp = _tillService.GetTill(iTill);

                    if (rsTemp != null && rsTemp.Active)  // if(till.Count>)){
                    {
                        if (rsTemp.ShiftDate != DateAndTime.Today)
                        {
                            // Aug 04, 2010 Nicolette added policy checking to fix the message displaying shift 0
                            if (_policyManager.USE_SHIFTS)
                            {
                                capValue[1] = rsTemp.Number;
                                capValue[2] = rsTemp.Shift;
                                capValue[3] = rsTemp.ShiftDate.ToString("MM/dd/yyyy");
                                //MsgBox "You are logging to Till #:" & rsTemp!Till_Num & ", Shift:" & rsTemp!ShiftNumber & ", ShiftDate:" & rsTemp!ShiftDate & ". Please close the till or continue using this shift."

                                errorMessage = new ErrorMessage
                                {
                                    //Message = "You are logging to Till " + iTill + ":, Shift: " + rsTemp.Shift + ", ShiftDate: " + rsTemp.ShiftDate + ". Please close the till or continue using this shift.",
                                    MessageStyle = _resourceManager.CreateMessage(offSet, 10, 88, capValue),
                                    ShutDownPos = false,
                                    StatusCode = HttpStatusCode.OK,
                                    TillNumber = iTill,
                                    ShiftNumber = rsTemp.Shift,
                                    FloatAmount = rsTemp.Float,
                                    ShiftDate = rsTemp.ShiftDate.ToString("MM/dd/yyyy")
                                };
                            }
                            else
                            {
                                capValueNoSh[1] = rsTemp.Number;
                                capValueNoSh[2] = rsTemp.ShiftDate.ToString("MM/dd/yyyy");
                                errorMessage = new ErrorMessage
                                {
                                    //"You are logging to Till " + iTill + ":, ShiftDate:" + rsTemp.ShiftDate + ". Please close the till or continue using this shift.",
                                    MessageStyle = _resourceManager.CreateMessage(offSet, 10, 83, capValueNoSh),
                                    ShutDownPos = false,
                                    StatusCode = HttpStatusCode.OK,
                                    TillNumber = iTill,
                                    ShiftNumber = rsTemp.Shift,
                                    FloatAmount = rsTemp.Float,
                                    ShiftDate = rsTemp.ShiftDate.ToString("MM/dd/yyyy")
                                };
                            }
                        }
                    }

                    //CheckProcesss will get the sShifts
                }
            }
            else
            {
                // - Forcing Till
                if (_policyManager.LogUnlimit == false)
                {
                    //User cannot log on to more than one register
                    var rsUser = _tillService.GetTills(null, null, null, user.Code, null, 1);
                    if (rsUser != null && rsUser.Count != 0)
                    {
                        if (forceTill != 0 && (rsUser.Count(r => r.POSId != posId) != 0))
                        {
                            //Check if it is forcing till and if so let the user log on BUT
                            //if there is already user logged on Don't allow to log in to Forced Till

                            if (!string.Equals(user.User_Group.Code, Utilities.Constants.Trainer, StringComparison.CurrentCultureIgnoreCase))
                            {
                                errorMessage = new ErrorMessage
                                {
                                    //"There is already user logged on Don't allow to log in to Forced Till",
                                    MessageStyle = _resourceManager.CreateMessage(offSet, 10, 99, null, (MessageType)((int)MessageType.Critical + (int)MessageType.OkOnly)),
                                    StatusCode = HttpStatusCode.Conflict,
                                    ShutDownPos = false
                                };
                                Performancelog.Debug($"End,TillManager,ComputeTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                                return;
                            }
                        }
                    }
                }
                //End - SV

                var tillNumber = forceTill;
                var rt = _tillService.GetTill(tillNumber);
                rt.Active = true;
                rt.Processing = true;
                rt.POSId = posId; //to identify which pos is using this till
                rt.UserLoggedOn = user.Code; //
                rt.ShiftDate = rt.ShiftDate == DateTime.MinValue ? DateAndTime.Today.Date : rt.ShiftDate.Date;
                _tillService.UpdateTill(rt);

                //TIMsgbox "Forcing Till Number " & Force_Till & " because " & vbCrLf & _
                //"an incomplete sale was found for that till.", _
                //vbOKOnly, "Forcing Till"
                errorMessage = new ErrorMessage
                {
                    //"Forcing Till Number " + forceTill + " because an incomplete sale was found for that till.",
                    MessageStyle = _resourceManager.CreateMessage(offSet, 10, 94, forceTill, MessageType.OkOnly),
                    StatusCode = HttpStatusCode.OK,
                    ShutDownPos = false,
                    TillNumber = forceTill,
                    ShiftNumber = rt.Shift,
                    FloatAmount = rt.Float,
                    ShiftDate = rt.ShiftDate.ToString("MM/dd/yyyy")
                };
            }
            Performancelog.Debug($"End,TillManager,ComputeTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }

        /// <summary>
        /// Check User ID 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="forceTill"></param>
        /// <param name="posId"></param>
        /// <param name="tills"></param>
        /// <param name="errorMessage"></param>
        private void Check_UserID(User user, ref short forceTill, int posId, ref List<Till> tills, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,Check_UserID,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            errorMessage = new ErrorMessage();
            if (string.Equals(user.User_Group.Code, Utilities.Constants.Trainer, StringComparison.CurrentCultureIgnoreCase)) //Behrooz Jan-12-06
            {
                if (forceTill > 0 && forceTill < double.Parse(Entities.Constants.TrainFirstTill))
                {
                    forceTill = 0;
                }
            }
            else
            {
                //
                if (_policyManager.LogUnlimit == false && !_policyManager.AutoShftPick)
                {
                    //User cannot log on to more than one register
                    var rsUser = _tillService.GetTills(null, null, null, user.Code, null, 1);
                    if (rsUser != null && rsUser.Count != 0)
                    {
                        var firstOrDefault = rsUser.FirstOrDefault();
                        if (firstOrDefault != null && (forceTill == 0 || (forceTill != 0 && firstOrDefault.POSId != posId)))
                        {
                            //Check if it is forcing till and if so let the user log on BUT
                            //if there is already user logged on Don't allow to log in to Forced Till
                            var offSet = _policyManager.LoadStoreInfo().OffSet;
                            errorMessage = new ErrorMessage
                            {
                                // "This user is already logged in another till",
                                MessageStyle = _resourceManager.CreateMessage(offSet, 10, 99, null, CriticalOkMessageType),
                                ShutDownPos = false,
                                StatusCode = HttpStatusCode.Conflict
                            };
                            Performancelog.Debug($"End,TillManager,Check_UserID,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                            return;
                        }
                    }
                }
                //End - SV
                if (forceTill > 0)
                {
                    if (forceTill >= Utilities.Constants.TrainFirstTill)
                    {
                        forceTill = 0;
                    }
                }
            }
            if (!_policyManager.AutoShftPick) return;
            //Check if there is a till for this user
            var rsTill = _tillService.GetTillForUser(1, 100, user.Code);
            if (rsTill == null || rsTill.Count == 0)
            {
                //tills.Clear();
                //tills.AddRange(rsTill);
                GetAllAvailTill_FillUplstTills(ref tills, out errorMessage);
            }
            else
            {
                //Check if the till is for this POS computer (POS_ID)
                var tillsForPos = new List<Till>();
                foreach (var till in rsTill)
                {
                    if (till.POSId != posId) continue;
                    tills.Clear();
                    tills.Add(till);
                    tillsForPos.Add(_tillService.GetTill(till.Number));
                }
                rsTill.Clear();
                rsTill.AddRange(tillsForPos);
                tillsForPos.Clear();
                if (tills.Count != 0) return;
                {
                    //there is a till but not for this computer,check if it is processing right now
                    //if there is more than one pos not processing, get them to choose which one to log in to
                    //if so, then pick another till, if it is not processing, you can use that till
                    if (rsTill.Count > 1)
                    {
                        //there is 2 pos for that user, need to check if they are both processing
                        foreach (var till in rsTill)
                        {
                            //tills.Clear();
                            if (till.Processing) continue;
                            tills.Add(till);
                            tillsForPos.Add(_tillService.GetTill(till.Number));
                        }
                        rsTill.Clear();
                        rsTill.AddRange(tillsForPos);
                        tillsForPos.Clear();

                        if (tills.Count == 0)
                        {
                            //all the tills for this user are currently processing
                            GetAllAvailTill_FillUplstTills(ref tills, out errorMessage);
                        }
                    }
                    else if (rsTill.Count == 1)
                    {
                        //there is 1 pos for that user
                        var firstOrDefault = rsTill.FirstOrDefault();
                        if (firstOrDefault != null && firstOrDefault.Processing == false)
                        {
                            tills.Clear();
                            tills.Add(rsTill.FirstOrDefault());
                        }
                        else
                        {
                            //there is a till for this user but he/she is logged on it in other computer
                            //so pick another till #
                            GetAllAvailTill_FillUplstTills(ref tills, out errorMessage);
                        }
                    }
                }
            }
            Performancelog.Debug($"End,TillManager,Check_UserID,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

        }


        /// <summary>
        /// Get All Available Tills and fill the list of tills
        /// </summary>
        /// <param name="tills"></param>
        /// <param name="errorMessage"></param>
        private void GetAllAvailTill_FillUplstTills(ref List<Till> tills, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,GetAllAvailTill_FillUplstTills,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            //rsTill = Chaps_Main.Get_Records(Source: "Select Tills.Till_Num as [Till] From Tills  WHERE  Tills.Active <> 1 AND Tills.Till_Num <> " + System.Convert.ToString(Conversion.Val(Chaps_Main.PayAtPumpTill)), DB: Chaps_Main.dbMaster, LockType: (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
            errorMessage = new ErrorMessage();
            var rsTill = _tillService.GetnotPayAtPumpTill(100);
            rsTill = rsTill.Where(x => !x.Active).ToList();
            if (rsTill.Count == 0)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                errorMessage = new ErrorMessage
                {
                    //"All defined tills are currently in use. Define more tills or wait until one becomes available.",
                    MessageStyle = _resourceManager.CreateMessage(offSet, 10, 91, null, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.NotFound,
                    ShutDownPos = true

                };
            }
            //Fill up the lstTills list
            tills.Clear();
            if (rsTill.Count == 0)
            {
                Performancelog.Debug($"End,TillManager,GetAllAvailTill_FillUplstTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return;
            }
            tills.AddRange(rsTill);
            //tills.AddRange(rsTill.Where(till => till.Number < Convert.ToInt32(Utilities.Constants.TrainFirstTill)));

            Performancelog.Debug($"End,TillManager,GetAllAvailTill_FillUplstTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }

        /// <summary>
        /// Get shifts
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="tillNumber"></param>
        /// <param name="posId"></param>
        /// <param name="errorMessage"></param>
        /// <param name="shiftsUsedForDay"></param>
        /// <param name="floatAmount"></param>
        /// <returns>Shifts</returns>
        public List<Shift> GetShifts(string userName, int tillNumber, int posId, out ErrorMessage errorMessage,
                                     out bool shiftsUsedForDay, out decimal floatAmount)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,GetShifts,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            //var user = _userService.GetUser(userName);
            var user = CacheManager.GetUser(userName) ?? _loginManager.GetUser(userName);

            shiftsUsedForDay = false;
            var tillNo = (short)tillNumber;
            floatAmount = _tillService.GetTill(tillNo) != null ? _tillService.GetTill(tillNo).Float : 0;
            if (IsProcessing(tillNo, posId))
            {
                errorMessage = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = string.Format("Till - " + Convert.ToString(tillNo) + " is currently used by another POS!." + "\r\n" + "Please select another Till! ~Cannot select Till-" + Convert.ToString(tillNo)),
                        MessageType = CriticalOkMessageType
                    },
                    StatusCode = HttpStatusCode.Conflict,
                    ShutDownPos = false

                };
                Performancelog.Debug($"End,TillManager,GetShifts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return null;
            }
            if (_policyManager.LogUnlimit == false)
            {
                //We need to check one more time if this user is already logged in. It's possible for the cashier to enter
                //user and password in both Tills and then select till number in each till consequently
                //User cannot log on to more than one register
                var tills = _tillService.GetTills(null, null, null, userName, null, 1);
                if (tills != null && tills.Count != 0 && !tills.Any(t => t.Number == tillNo))
                {
                    if (user.User_Group.Name != Utilities.Constants.Trainer)
                    {
                        var offSet = _policyManager.LoadStoreInfo().OffSet;
                        errorMessage = new ErrorMessage
                        {
                            //"User is already logged on->cannot log in again but he / she can log if the till is forced",
                            MessageStyle = _resourceManager.CreateMessage(offSet, 10, 99, null, CriticalOkMessageType),
                            StatusCode = HttpStatusCode.Conflict,
                            ShutDownPos = false
                        };
                        Performancelog.Debug($"End,TillManager,GetShifts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                        return null;
                    }
                }
            }
            //End - SV

            var shifts = FindShifts(tillNo, userName, posId, out errorMessage, out shiftsUsedForDay);

            if (shifts == null || shifts.Count == 0)
            {
                if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                {
                    errorMessage = new ErrorMessage
                    {
                        MessageStyle = new MessageStyle
                        {
                            Message = "No shifts found.",
                            MessageType = MessageType.OkOnly
                        },
                        StatusCode = HttpStatusCode.Conflict,
                        ShutDownPos = true
                    };
                }
                Performancelog.Debug($"End,TillManager,GetShifts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return null;

            }
            if (user.User_Group.Name != Entities.Constants.Trainer) return shifts;
            _policyManager.CC_MODE = "Cross-Ring";
            //TODO register
            //if (registerRenamed != null)
            //{
            //    registerRenamed.Cash_Drawer = false;
            //}

            _policyManager.AllowPrepay = false;
            Performancelog.Debug($"End,TillManager,GetShifts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return shifts;
        }

        /// <summary>
        /// Check for processsing tills
        /// </summary>
        /// <param name="vTill"></param>
        /// <param name="posId"></param>
        /// <returns></returns>
        public bool IsProcessing(short vTill, int posId)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,IsProcessing,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var returnValue = false;
            var till = _tillService.GetTill(vTill);
            if (till != null && till.Processing && till.POSId != posId)
            {
                returnValue = true;
            }
            Performancelog.Debug($"End,TillManager,IsProcessing,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return returnValue;
        }

        /// <summary>
        /// Find shifts
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="userName"></param>
        /// <param name="posId"></param>
        /// <param name="errorMessage"></param>
        /// <param name="shiftsUsedForDay"></param>
        /// <returns>Shifts</returns>
        private List<Shift> FindShifts(int tillNumber, string userName, int posId, out ErrorMessage errorMessage, out bool shiftsUsedForDay)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,FindShifts,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            errorMessage = new ErrorMessage();
            shiftsUsedForDay = false;
            var till = _tillService.GetTill(tillNumber);
            if (till != null)
            {
                var shifts = new List<Shift>();
                till.ShiftDate = till.ShiftDate == DateTime.MinValue ? DateAndTime.Today.Date : till.ShiftDate.Date;
                if (!till.Active)
                {
                    if (_policyManager.USE_SHIFTS)
                    {
                        shifts = !_policyManager.AutoShftPick ? Compute_Shifts(ref till, out errorMessage) : Get_NextShift(ref till, out errorMessage, out shiftsUsedForDay);
                    }
                    else
                    {
                        till.Shift = 0;
                        errorMessage = new ErrorMessage
                        {
                            MessageStyle = new MessageStyle
                            {
                                Message = "Cannot use shift"
                            },
                            StatusCode = HttpStatusCode.OK,
                            TillNumber = 0,
                            ShiftNumber = 0,
                            FloatAmount = till.Float

                        };
                        shifts.Add(new Shift
                        {
                            ShiftNumber = 0,
                            DisplayFormat = "0",
                            ShiftDate = till.ShiftDate.ToString("MM/dd/yyyy")
                        });
                    }
                }
                else // Till is Active
                {
                    till.Processing = true;
                    till.POSId = posId; //to identify which pos is using this till
                    till.UserLoggedOn = userName; //
                    errorMessage = new ErrorMessage
                    {
                        MessageStyle = new MessageStyle
                        {
                            Message = "Till is active"
                        },
                        StatusCode = HttpStatusCode.OK,
                        TillNumber = till.Number,
                        ShiftNumber = till.Shift,
                        FloatAmount = till.Float

                    };
                    shifts.Add(new Shift
                    {
                        ShiftNumber = till.Shift,
                        DisplayFormat = till.Shift.ToString(),
                        ShiftDate = till.ShiftDate.ToString("MM/dd/yyyy")
                    });
                    _tillService.UpdateTill(till);
                }
                Performancelog.Debug($"End,TillManager,FindShifts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return shifts;
            }
            errorMessage = new ErrorMessage
            {
                MessageStyle = new MessageStyle
                {
                    Message = "Till does not exists"
                },
                StatusCode = HttpStatusCode.NotFound,
                ShutDownPos = true

            };
            Performancelog.Debug($"End,TillManager,FindShifts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return null;
        }

        /// <summary>
        /// Method to compute shift
        /// </summary>
        /// <param name="till">Till</param>
        /// <param name="errorMessage">Error</param>
        /// <returns>List of shifts</returns>
        private List<Shift> Compute_Shifts(ref Till till, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,Compute_Shifts,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var offset = _policyManager.LoadStoreInfo().OffSet;
            errorMessage = new ErrorMessage();
            short n = 0;
            short nextShift;
            DateTime et = default(DateTime);
            short nAddDaysCurrent = 0;
            short nAddDaysNext = 0;
            DateTime shift1Date;
            var activeShifts = _shiftService.GetShifts(1);
            var shiftDates = new List<Shift>();
            if (activeShifts == null || activeShifts.Count == 0)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 10, 97, null),
                    StatusCode = HttpStatusCode.NotFound,
                    ShutDownPos = true

                };
                Performancelog.Debug($"End,TillManager,Compute_Shifts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return null;
            }

            var shifts = _shiftService.GetShifts(null);
            var totalShifts = shifts.Count - 1;
            short currentShift = 0;
            var maxShift = _shiftService.GetMaximumShiftNumber();
            var index = 0;
            var st = shifts[index].StartTime;
            if (maxShift > 1)
            {
                if (index <= totalShifts)
                {
                    index++;
                }
                if (index <= totalShifts)
                {
                    et = DateAndTime.DateAdd(DateInterval.Second, -1, shifts[index].StartTime);
                }
                if (index != 0)
                {
                    index--;
                }
            }
            else if (index <= totalShifts)
            {
                et = DateAndTime.DateAdd(DateInterval.Second, -1, shifts[index].StartTime);
            }

            // Does this shift span midnight?
            var spansMidnight = st.TimeOfDay > et.TimeOfDay;
            //Binal july 08
            //    for updating the shift_day starts at the current/next day
            if (_policyManager.SHIFT_DAY == "Current")
            {

                index = 0;
                shifts[index].CurrentDay = 1;
            }
            else
            {
                index = 0;
                shifts[index].CurrentDay = 0;
            }
            _shiftService.UpdateShift(shifts[index]);

            //Binal end
            if (spansMidnight)
            {
                if (DateAndTime.TimeOfDay.TimeOfDay >= st.TimeOfDay)
                {
                    shift1Date = shifts[index].CurrentDay == 1 ? DateAndTime.Today : DateAndTime.DateAdd(DateInterval.Day, 1, DateAndTime.Today);
                }
                else
                {
                    shift1Date = shifts[index].CurrentDay == 1 ? DateAndTime.DateAdd(DateInterval.Day, -1, DateAndTime.Today) : DateAndTime.Today;
                }
            }
            else
            {
                shift1Date = DateAndTime.TimeOfDay.TimeOfDay < st.TimeOfDay ? DateAndTime.DateAdd(DateInterval.Day, -1, DateAndTime.Today) : DateAndTime.Today;
            }

            // Find the current shift. That's the one where the start time
            // is before the current time and the end time is later than
            // the current time.
            while (index <= totalShifts)
            {
                st = shifts[index].StartTime;
                if (shifts[index].ShiftNumber == maxShift)
                {
                    index = 0;
                    et = DateAndTime.DateAdd(DateInterval.Second, -1, shifts[index].StartTime);
                    index = totalShifts;
                }
                else
                {
                    // Other shifts end one second before the next one starts.
                    if (index <= totalShifts)
                    {
                        index++;
                    }
                    if (index <= totalShifts)
                    {
                        et = DateAndTime.DateAdd(DateInterval.Second, -1, shifts[index].StartTime);
                    }
                    if (index != 0)
                    {
                        index--;
                    }
                }
                if (st.TimeOfDay < et.TimeOfDay)
                {
                    // Start and stop in the same day
                    if ((st.TimeOfDay <= DateAndTime.TimeOfDay.TimeOfDay) && (et.TimeOfDay >= DateAndTime.TimeOfDay.TimeOfDay))
                    {
                        // Save the shift that includes the current time (Active or not)
                        n = (short)shifts[index].ShiftNumber;
                        if (shifts[index].Active == 1)
                        {
                            currentShift = n; // Set the current shift.
                        }
                    }
                }
                else
                {
                    // Start in one day, stop in the next
                    if ((st.TimeOfDay <= DateAndTime.TimeOfDay.TimeOfDay) || (et.TimeOfDay >= DateAndTime.TimeOfDay.TimeOfDay))
                    {
                        // Save the shift that includes the current time (Active or not)
                        n = (short)shifts[index].ShiftNumber;
                        if (shifts[index].Active == 1)
                        {
                            currentShift = n; // Set the current shift.
                        }
                    }
                }

                if (currentShift > 0)
                {
                    break;
                }
                if (index <= totalShifts)
                {
                    index++;
                }
            }

            // If the Current_Shift is zero, that means that the shift that includes
            // the current time is not active. Look for the next active shift after
            // the one that includes the current time.
            if (currentShift == 0)
            {
                if (n == maxShift)
                {
                    index = 0;
                    nAddDaysCurrent = 1;
                }
                else
                {
                    index = shifts.FindIndex(s => s.ShiftNumber == n + 1);
                }

                while (index <= totalShifts)
                {
                    if (shifts[index].Active == 1)
                    {
                        st = shifts[index].StartTime;
                        if (shifts[index].ShiftNumber == maxShift)
                        {
                            index = 0;
                            et = DateAndTime.DateAdd(DateInterval.Second, -1, shifts[index].StartTime);
                            index = totalShifts;
                        }
                        else
                        {
                            // Other shifts end one second before the next one starts.
                            if (index <= totalShifts)
                            {
                                index++;
                            }
                            if (index <= totalShifts)
                            {
                                et = DateAndTime.DateAdd(DateInterval.Second, -1, shifts[index].StartTime);
                            }
                            if (index != 0)
                            {
                                index--;
                            }
                        }
                        currentShift = Convert.ToInt16(shifts[index].ShiftNumber);
                        break;
                    }
                    if (index <= totalShifts)
                    {
                        index++;
                    }
                }
            }

            // If Current_Shift is still zero, that means that the shift that includes
            // the current time is not active and neither were any after the the
            // current one. Start at shift 1 and look for an active shift.
            if (currentShift == 0)
            {
                index = 0;
                nAddDaysCurrent = 1;
                while (index <= totalShifts)
                {
                    if (shifts[index].Active == 1)
                    {
                        currentShift = Convert.ToInt16(shifts[index].ShiftNumber);
                        st = Convert.ToDateTime(shifts[index].StartTime);
                        if (shifts[index].ShiftNumber == maxShift)
                        {
                            // The last defined shift ends one second before the first one.
                            index = 0;
                            et = DateAndTime.DateAdd(DateInterval.Second, -1, shifts[index].StartTime);
                            index = totalShifts;
                        }
                        else
                        {
                            // Other shifts end one second before the next one starts.
                            if (index <= totalShifts)
                            {
                                index++;
                            }
                            if (index <= totalShifts)
                            {
                                et = DateAndTime.DateAdd(DateInterval.Second, -1, shifts[index].StartTime);
                            }
                            if (index != 0)
                            {
                                index--;
                            }
                        }
                        break;
                    }
                    if (index <= totalShifts)
                    {
                        index++;
                    }
                }
            }

            var dateCurrent = DateAndTime.DateAdd(DateInterval.Day, nAddDaysCurrent, shift1Date);

            shiftDates.Add(item: new Shift
            {
                ShiftNumber = currentShift,
                DisplayFormat =
                    $"{_resourceManager.GetResString(offset, 889)} ({currentShift}) - {st:hh:mm tt} {_resourceManager.GetResString(offset, 888)} {et:hh:mm tt} {_resourceManager.GetResString(offset, 887)} {dateCurrent:MMM-dd-yyyy}",
                ShiftDate = dateCurrent.ToString("MM/dd/yyyy")
            });

            // Now find the next ACTIVE shift after the current one.
            if (currentShift == maxShift)
            {
                nextShift = 1;
                nAddDaysNext = 1;
            }
            else
            {
                nextShift = (short)(currentShift + 1);
            }

            index = shifts.FindIndex(s => s.ShiftNumber == nextShift);

            while (shifts[index].Active == 0)
            {
                if (nextShift == maxShift)
                {
                    nextShift = 1;
                    nAddDaysNext = 1;
                }
                else
                {
                    nextShift++;
                }
                index = shifts.FindIndex(s => s.ShiftNumber == nextShift);
            }

            st = shifts[index].StartTime;
            if (shifts[index].ShiftNumber > currentShift)
            {
                if (shifts[index].ShiftNumber == maxShift)
                {
                    // The last defined shift ends one second before the first one.
                    index = 0;
                    et = DateAndTime.DateAdd(DateInterval.Second, -1, shifts[index].StartTime);
                    index = totalShifts;
                }
                else
                {
                    // Other shifts end one second before the next one starts.
                    if (index <= totalShifts)
                    {
                        index++;
                    }
                    if (index <= totalShifts)
                    {
                        et = DateAndTime.DateAdd(DateInterval.Second, -1, shifts[index].StartTime);
                    }
                    if (index != 0)
                    {
                        index--;
                    }
                }
            }
            else
            {
                if (index <= totalShifts)
                {
                    index++;
                }
                if (index <= totalShifts)
                {
                    et = DateAndTime.DateAdd(DateInterval.Second, -1, shifts[index].StartTime);
                }
                if (index != 0)
                {
                    index--;
                }
            }

            if (nAddDaysNext > 1)
            {
                nAddDaysNext = 1;
            }
            var dateNext = DateAndTime.DateAdd(DateInterval.Day, nAddDaysNext, dateCurrent);
            till.Shift = currentShift;

            shiftDates.Add(new Shift
            {
                ShiftNumber = nextShift,

                DisplayFormat =
                    $"{_resourceManager.GetResString(offset, 886)} ({nextShift}) - {st:hh:mm tt} {_resourceManager.GetResString(offset, 888)} {et:hh:mm tt} {_resourceManager.GetResString(offset, 887)} {dateNext:MMM-dd-yyyy}",
                ShiftDate = dateNext.ToString("MM/dd/yyyy")
            });
            Performancelog.Debug($"End,TillManager,Compute_Shifts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return shiftDates;
        }


        /// <summary>
        /// Get next Shifts 
        /// </summary>
        /// <param name="till"></param>
        /// <param name="errorMessage"></param>
        /// <param name="shiftsUsedforDay"></param>
        /// <returns></returns>
        private List<Shift> Get_NextShift(ref Till till, out ErrorMessage errorMessage, out bool shiftsUsedforDay)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,Get_NextShift,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            bool firstShift = false;
            var nextActiveShifts = new List<ShiftStore>();
            var activeShifts = _shiftService.GetShifts(1);
            shiftsUsedforDay = false;
            errorMessage = new ErrorMessage();
            var shiftDates = new List<Shift>();
            if (activeShifts == null || activeShifts.Count == 0)
            {
                errorMessage = new ErrorMessage
                {
                    //"You must have at least one active shift defined. Cannot Continue. ",
                    MessageStyle = _resourceManager.CreateMessage(offSet, 10, 97, null, CriticalOkMessageType),
                    ShutDownPos = true,
                    StatusCode = HttpStatusCode.NotFound

                };
                return null;
            }

            //Since it is not 24 hour operation, there is no problem with last and first shift (overlapping)
            int nowShift;
            if (!_policyManager.Hour24Store)
            {
                // Added formatting for french conversion
                nowShift = _tillService.GetMaximumShiftNumber(DateAndTime.Today, till.Number);
            }
            else
            {
                nowShift = _tillService.GetMaximumShiftNumber(DateAndTime.Today, till.Number);

                if (nowShift == 0)
                {
                    //If there are no records with today's shift then the first shift must have started yesterday
                    nowShift = _tillService.GetMaximumShiftNumber(DateAndTime.DateAdd(DateInterval.Day, -1, DateAndTime.Today), till.Number);
                }
            }

            if (nowShift != 0)
            {
                //this is not the first shift of the day; need to select the next shift
                var nextShifts = _shiftService.GetNextShift(nowShift, 1, till.Number);
                if (nextShifts == null || nextShifts.Count == 0)
                {
                    if (_policyManager.Hour24Store)
                    {
                        shiftsUsedforDay = true;

                        errorMessage = new ErrorMessage
                        {
                            //"Do you want to start the 1st shift of the next day? If your answer is No, then you need to create a new active shift in BackOffice.",
                            MessageStyle = _resourceManager.CreateMessage(offSet, 10, 87, CriticalOkMessageType),
                            StatusCode = HttpStatusCode.Conflict,
                            ShutDownPos = false
                        };
                        //We have used up all the shifts for this day, start the next day shift?? or add a new shift
                        //MsgBoxStyle temp_VbStyle2 = (int)MsgBoxStyle.Question + MsgBoxStyle.YesNo;
                        //ans = (int)(Chaps_Main.DisplayMessage(this, (short)87, temp_VbStyle2, null, (byte)0));
                        ////ans = MsgBox("Do you want to start the 1st shift of the next day? If your answer is No, then you need to create a new active shift in BackOffice.", vbYesNo + vbCritical)
                        //if (ans == (int)MsgBoxResult.No)
                        //{

                        //    //rs = null;
                        //}
                        firstShift = true;
                        nextActiveShifts = _shiftService.GetNextActiveShift(1, till.Number);

                    }
                    else
                    {
                        //Policy is not 24 hours. This is not the first shift and we don't have any more shifts for today
                        //there is no more shift left
                        // MsgBox "All defined shifts are in used or are not active. Define more shifts or wait till one becomes available. ", vbCritical + vbOKOnly, _
                        //"Terminate Application"

                        errorMessage = new ErrorMessage
                        {
                            //"All defined shifts are in used or are not active. Define more shifts or wait till one becomes available. ",
                            MessageStyle = _resourceManager.CreateMessage(offSet, 10, 89, null, CriticalOkMessageType),
                            StatusCode = HttpStatusCode.Conflict,
                            ShutDownPos = true
                        };

                        return null;
                    }
                }
                else
                {
                    nextActiveShifts.AddRange(nextShifts);
                }
            }
            else
            {
                //first shift of the day
                firstShift = true;
                if (_policyManager.Hour24Store)
                {
                    shiftsUsedforDay = true;
                    //ans = MsgBox("Do you want to start the 1st shift of the next day? If your answer is No, then you need to create a new active shift in BackOffice.", vbYesNo + vbCritical)
                    //MsgBoxStyle temp_VbStyle4 = (int)MsgBoxStyle.Question + MsgBoxStyle.YesNo;
                    //ans = (int)(Chaps_Main.DisplayMessage(this, (short)87, temp_VbStyle4, null, (byte)0));
                    //if (ans == (int)MsgBoxResult.No)
                    //{

                    //}

                    errorMessage = new ErrorMessage
                    {

                        //"Do you want to start the 1st shift of the next day? If your answer is No, then you need to create a new active shift in BackOffice.",
                        MessageStyle = _resourceManager.CreateMessage(offSet, 10, 87, null, QuestionOkMessageType),
                        StatusCode = HttpStatusCode.Conflict,
                        ShutDownPos = false
                    };
                    nextActiveShifts = _shiftService.GetNextActiveShift(1, till.Number);
                }
                else
                {
                    nextActiveShifts = _shiftService.GetNextActiveShift(1, till.Number);
                }
            }

            if (nextActiveShifts == null || nextActiveShifts.Count == 0)
            {
                errorMessage = new ErrorMessage
                {
                    //"All defined shifts are in used or are not active. Define more shifts or wait till one becomes available. ",
                    MessageStyle = _resourceManager.CreateMessage(offSet, 10, 89, null, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.NotFound,
                    ShutDownPos = true
                };
                return null;
            }

            var firstOrDefault = nextActiveShifts.FirstOrDefault();
            if (firstOrDefault != null)
                till.Shift = (short)firstOrDefault.ShiftNumber;
            if (firstShift)
            {
                //If Time & Date >= #12:00:00 AM# & Date Then 'cut-off is 10 p.m.
                //Cut-off is 6:00 p.m. After 6:00 p.m. we'r checking for the current and next policy to set the shiftdate

                // if (DateAndTime.DateDiff(Microsoft.VisualBasic.DateInterval.Hour, DateTime.Now.TimeOfDay, DateTime.Parse(DateAndTime.Today + " " + System.Convert.ToString(DateTime.Parse("12:00:00 AM")))) > -18)
                if (DateTime.Now.TimeOfDay.TotalMinutes - DateTime.Today.Date.TimeOfDay.TotalMinutes > 18 * 60)
                {

                    till.ShiftDate = DateAndTime.Today.Date;
                }
                else
                {
                    if (_policyManager.SHIFT_DAY == "Current")
                    {

                        till.ShiftDate = DateAndTime.Today.Date;
                    }
                    else
                    {

                        till.ShiftDate = DateAndTime.DateAdd(DateInterval.Day, 1, DateAndTime.Today).Date;
                    }
                }
                //Also we need to clear all info for tills that are not active
                _tillService.ClearNonActiveTill(0, Chaps_Main.PayAtPumpTill);
            }
            else
            {
                //This is not the first shift, so no need to check for Current or Next Shift Date
                till.ShiftDate = DateAndTime.Today.Date;
            }
            shiftDates.Add(new Shift
            {
                ShiftNumber = till.Shift,
                DisplayFormat = till.Shift.ToString(),
                ShiftDate = till.ShiftDate.ToString("MM/dd/yyyy")
            });
            Performancelog.Debug($"End,TillManager,Get_NextShift,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return shiftDates;
        }


        /// <summary>
        /// Update till
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="shiftNumber"></param>
        /// <param name="shiftDate"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="posId"></param>
        /// <param name="floatAmount"></param>
        /// <param name="errorMessage"></param>
        public string UpdateTillInformation(int tillNumber, int? shiftNumber, string shiftDate, string userName,
            string password, int posId, decimal floatAmount, out ErrorMessage errorMessage)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,UpdateTillInformation,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!_loginManager.IsValidUser(userName, password, out errorMessage)) return null;
            if (!string.IsNullOrEmpty(_loginManager.GetIpAddress(posId)))
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
                            return null;
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
                        return null;
                    }
                }
                // Nicolette added to record date, time, active=true in
                // Tills table if USE_SHIFTS=No and TILL_FLOAT=No
                if (till != null)
                {
                    DateTime date;
                    DateTime.TryParse(shiftDate, out date);
                    if (date == DateTime.MinValue)
                    {
                        date = DateAndTime.Today;
                    }

                    // If till is logged in first time then update Open time
                    if (!till.Active)
                    {
                        till.Date_Open = DateAndTime.Today;
                        till.Time_Open = DateAndTime.TimeOfDay;
                    }
                    till.Active = true;
                    till.Processing = true;
                    till.POSId = posId;
                    till.UserLoggedOn = userName.ToUpper();
                    if (useShift)
                    {
                        till.Shift = (short)shiftNumber.Value;
                        till.ShiftDate = date;
                    }
                    else
                    {
                        till.Shift = 0;
                    }
                    if (_policyManager.TILL_FLOAT)
                    {
                        if (floatAmount <= (decimal)214748.3647)
                        {
                            till.Float = floatAmount;
                            till.Cash = floatAmount;
                        }
                        else
                        {
                            errorMessage = new ErrorMessage
                            {
                                MessageStyle = new MessageStyle
                                {
                                    Message = "Maximum float amount is 214748.3647"
                                },
                                StatusCode = HttpStatusCode.BadRequest,
                                ShutDownPos = false

                            };
                            return null;
                        }
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
                    return null;
                }

                if (useShift && shift != null)
                {
                    if (shift.ShiftNumber == 0) return null;
                    shift.Active = 1;
                    _shiftService.UpdateShift(shift);
                }
            }
            else
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                //"Security Alert. Check your POS IP Address!";
                errorMessage.MessageStyle = _resourceManager.CreateMessage(offSet, 0, 8198, null, MessageType.OkOnly);
                errorMessage.StatusCode = HttpStatusCode.Unauthorized;
                errorMessage.ShutDownPos = true;
            }
            Performancelog.Debug($"End,TillManager,UpdateTillInformation,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            var caption = string.Empty;
            var user = _userService.GetUser(userName);
            if (user.User_Group.Code == Utilities.Constants.Trainer)
            {
                caption = user.User_Group.Code + "-" + user.Code;
            }
            return caption;
        }

        /// <summary>
        /// Logout the Till
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Logout(int tillNumber, out ErrorMessage error)
        {
            var dateStart = DateTime.Now;
            Performancelog.Debug($"Start,TillManager,Logout,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            error = new ErrorMessage();
            //if (_saleService.ExistingSaleInDbTemp(tillNumber))
            if (_saleService.GetSaleByTillNumber(tillNumber).Sale_Lines.Count > 0)
            {
                var offSet = _policyManager.LoadStoreInfo().OffSet;
                MessageType temp_VbStyle3 = (int)MessageType.OkOnly + MessageType.Information;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 67, null, temp_VbStyle3);
                error.StatusCode = HttpStatusCode.Conflict;
                return false;
            }

            var rt = _tillService.GetTill(tillNumber);
            rt.Active = false;
            rt.Processing = false;
            rt.UserLoggedOn = null;
            rt.ShiftDate = rt.ShiftDate == DateTime.MinValue ? DateAndTime.Today.Date : rt.ShiftDate.Date;
            var till = _tillService.UpdateTill(rt);

            Performancelog.Debug($"End,TillManager,Logout,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            if (!till.Active && !till.Processing)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to get all tills
        /// </summary>
        /// <returns>List of tills</returns>
        public Dictionary<int, string> GetAllTills()
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var tills = new Dictionary<int, string>();
            string all = _resourceManager.GetResString(offSet, 407);
            tills.Add(0, all);
            var availableTills = _tillService.GetAllTills();
            foreach (var till in availableTills)
            {
                tills.Add(till, till.ToString());
            }
            return tills;
        }

        /// <summary>
        /// Method to get all shifts
        /// </summary>
        /// <returns>List of shifts</returns>
        public Dictionary<int, string> GetAllShifts()
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var shifts = new Dictionary<int, string>();
            string all = _resourceManager.GetResString(offSet, (short)407);
            shifts.Add(0, all);
            var availableShifts = _shiftService.GetShifts(null);
            foreach (var shift in availableShifts)
            {
                shifts.Add(shift.ShiftNumber, shift.ShiftNumber.ToString());
            }
            return shifts;
        }

    }
}
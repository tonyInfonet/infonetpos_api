using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Filters;
using log4net;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Login;
using Infonet.CStoreCommander.WebApi.Models.Till;
using Infonet.CStoreCommander.WebApi.Utilities;
using Infonet.CStoreCommander.WebApi.Resources;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.BusinessLayer.Entities;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for Tills and Shifts
    /// </summary>
    [RoutePrefix("api/v1/tills")]
    public class TillController : ApiController
    {
        private readonly ILoginManager _loginManager;
        private readonly ITillManager _tillManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ITillCloseManager _tillCloseManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="loginManager"></param>
        /// <param name="tillManager"></param>
        /// <param name="resourceManager"></param>
        /// <param name="tillCloseManager"></param>
        public TillController(ILoginManager loginManager, ITillManager tillManager, IApiResourceManager
            resourceManager, ITillCloseManager tillCloseManager)
        {
            _loginManager = loginManager;
            _tillManager = tillManager;
            _resourceManager = resourceManager;
            _tillCloseManager = tillCloseManager;
        }

        /// <summary>
        /// Authenticates the user and returns the list of tills available for Login
        /// </summary>
        /// <param name="user"></param>
        /// <returns>List of available tills</returns>
        [HttpPost]
        [Route("activeTills")]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(InvalidLoginReponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(InvalidLoginReponseModel))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActiveTillResponseModel))]
        public HttpResponseMessage GetTills([FromBody]LoginUserModel user)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillController,GetTills,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            // Authenticating the user with the username and password
            if (user == null)
            {
                return Request.CreateResponse(
                  HttpStatusCode.BadRequest,
                  new ErrorResponse
                  {
                      Error = new MessageStyle { Message = Constants.InvalidRequest }
                  });
            }

            ErrorMessage errorMessage;
            if (_loginManager.IsValidUser(user.UserName, user.Password, out errorMessage))
            {
                var tills = _tillManager.GetTills(user.UserName, user.PosId, user.TillNumber, out errorMessage);
                //trainer mode
                if (errorMessage.StatusCode == HttpStatusCode.Ambiguous)
                {
                    _performancelog.Debug($"End,TillController,GetTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                    var response = new ActiveTillResponseModel
                    {
                        Tills = tills.Select(x => new Models.Till.Till
                        {
                            TillNumber = x
                        }).ToList(),
                        ShiftNumber = errorMessage.ShiftNumber,
                        ShiftDate = string.IsNullOrEmpty(errorMessage.ShiftDate) ? DateTime.Today.ToString("MM/dd/yyyy") : errorMessage.ShiftDate,
                        CashFloat = errorMessage.FloatAmount,
                        Message = errorMessage.MessageStyle ?? new MessageStyle(),
                        ShutDownPOS = errorMessage.ShutDownPos,
                        ForceTill = false,
                        IsTrainer = true
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, response);

                }

                //check if there are tills to be passed
                if (errorMessage.StatusCode == HttpStatusCode.OK && errorMessage.TillNumber != 0)
                {
                    tills.Clear();
                    tills.Add(errorMessage.TillNumber);
                    _performancelog.Debug($"End,TillController,GetTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                    var response = new ActiveTillResponseModel
                    {
                        Tills = tills.Select(x => new Models.Till.Till
                        {
                            TillNumber = x
                        }).ToList(),
                        ShiftNumber = errorMessage.ShiftNumber,
                        ShiftDate = string.IsNullOrEmpty(errorMessage.ShiftDate) ? DateTime.Today.ToString("MM/dd/yyyy") : errorMessage.ShiftDate,
                        CashFloat = errorMessage.FloatAmount,
                        Message = errorMessage.MessageStyle ?? new MessageStyle(),
                        ShutDownPOS = errorMessage.ShutDownPos,
                        ForceTill = true
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, response);

                }
                _performancelog.Debug($"End,TillController,GetTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                {
                    var response = new ActiveTillResponseModel
                    {
                        Tills = tills.Select(x => new Models.Till.Till
                        {
                            TillNumber = x
                        }).ToList(),
                        ShiftNumber = 0,
                        CashFloat = 0,
                        Message = new MessageStyle(),
                        //  code = 0,
                        ForceTill = false
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                return Request.CreateResponse(errorMessage.StatusCode,
                    new InvalidLoginReponseModel
                    {
                        Error = errorMessage.MessageStyle,
                        ShutDownPOS = errorMessage.ShutDownPos
                    });
            }
            _performancelog.Debug($"End,TillController,GetTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(errorMessage.StatusCode,
                new InvalidLoginReponseModel
                {
                    Error = errorMessage.MessageStyle,
                    ShutDownPOS = errorMessage.ShutDownPos
                });
        }

        /// <summary>
        /// Authenticates the user and returns the shifts availabel for the Login
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>List of active shifts</returns>
        [HttpPost]
        [Route("activeShifts")]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(InvalidLoginReponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(InvalidLoginReponseModel))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActiveShiftResponseModel))]
        public HttpResponseMessage GetShifts(UserModel user)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillController,GetShifts,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            ErrorMessage errorMessage;
            if (_loginManager.IsValidUser(user.UserName, user.Password, out errorMessage))
            {
                if (!string.IsNullOrEmpty(_loginManager.GetIpAddress((byte)user.PosId)))
                {
                    bool shiftsUsedForDay;
                    decimal floatAmount;
                    var shifts = _tillManager.GetShifts(user.UserName, user.TillNumber, user.PosId, out errorMessage,
                        out shiftsUsedForDay, out floatAmount);
                    if (errorMessage.StatusCode == HttpStatusCode.OK && errorMessage.TillNumber != 0)
                    {
                        _performancelog.Debug($"End,TillController,GetShifts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                        //if shift is forced
                        return Request.CreateResponse(HttpStatusCode.OK,
                               new ActiveShiftResponseModel
                               {
                                   Shifts = shifts,
                                   ShiftsUsedForDay = shiftsUsedForDay,
                                   CashFloat = floatAmount,
                                   ForceShift = true
                               });

                    }
                    //if we get all shifts
                    if (string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
                    {
                        _performancelog.Debug($"End,TillController,GetShifts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                        return Request.CreateResponse(HttpStatusCode.OK, new ActiveShiftResponseModel
                        {
                            Shifts = shifts,
                            ShiftsUsedForDay = shiftsUsedForDay,
                            CashFloat = floatAmount,
                            ForceShift = false
                        });
                    }
                    //if there is next shift that can be defined
                    if (shifts != null && shifts.Count != 0)
                    {
                        _performancelog.Debug($"End,TillController,GetShifts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                        return Request.CreateResponse(HttpStatusCode.OK, new ActiveShiftResponseModel
                        {
                            Shifts = shifts,
                            ShiftsUsedForDay = shiftsUsedForDay,
                            CashFloat = floatAmount,
                            ForceShift = false
                        });
                    }
                    _performancelog.Debug($"End,TillController,GetShifts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                    //if there is any message
                    return Request.CreateResponse(errorMessage.StatusCode,
                        new InvalidLoginReponseModel
                        {
                            Error = errorMessage.MessageStyle,
                            ShutDownPOS = errorMessage.ShutDownPos
                        });
                }
                _performancelog.Debug($"End,TillController,GetShifts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                var store = CacheManager.GetStoreInfo();
                var offSet = store?.OffSet ?? 0;
                //invalid ip address
                return Request.CreateResponse(HttpStatusCode.Unauthorized,
                    new InvalidLoginReponseModel()
                    {

                        Error = new MessageStyle
                        {
                            Message = _resourceManager.GetResString(8198, offSet),
                            MessageType = MessageType.OkCancel
                        },
                        ShutDownPOS = true
                    });
            }
            //invalid user
            return Request.CreateResponse(errorMessage.StatusCode,
                new InvalidLoginReponseModel
                {
                    Error = errorMessage.MessageStyle,
                    ShutDownPOS = errorMessage.ShutDownPos
                });
        }


        /// <summary>
        /// Logout User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("updateTillClose")]
        [HttpPost]
        [ApiAuthorization]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CloseCurrentTillResponseModel))]
        public HttpResponseMessage UpdatetillClose([FromBody]UpdateTillCloseInputModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillController,UpdatetillClose,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            ErrorMessage errorMessage;
            if (model == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = Constants.InvalidRequest }
                    });
            }
            if (model.UpdatedTender == null && model.UpdatedBillCoin == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                  new ErrorResponse
                  {
                      Error = new MessageStyle { Message = Constants.InvalidRequest }
                  });
            }
            if (model.UpdatedTender == null)
            {
                model.UpdatedTender.Name = string.Empty;
                model.UpdatedTender.Entered = 0;
            }
            if (model.UpdatedBillCoin == null)
            {
                model.UpdatedBillCoin.Description = string.Empty;
                model.UpdatedBillCoin.Amount = 0;
            }
            var result = _tillCloseManager.UpdateTillClose(model.TillNumber, model.UpdatedTender.Name,
              model.UpdatedTender.Entered, model.UpdatedBillCoin.Description, model.UpdatedBillCoin.Amount,
              out errorMessage);

            _performancelog.Debug($"End,TillController,UpdatetillClose,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                //if there is any message
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Method to get all tills
        /// </summary>
        /// <returns>List of tills</returns>
        [HttpGet]
        [ApiAuthorization]
        [Route("getAllTills")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "", Type = typeof(MessageStyle))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(List<TillModel>))]
        public HttpResponseMessage GetAllTills()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillController,GetAllTills,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var tills = _tillManager.GetAllTills();
            var response = from till in tills
                           select new TillModel
                           {
                               Id = till.Key,
                               TillNumber = till.Value
                           };

            _performancelog.Debug($"End,TillController,GetAllTills,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }


        /// <summary>
        /// Method to get all shifts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorization]
        [Route("getAllShifts")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(MessageStyle))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "", Type = typeof(MessageStyle))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(List<ShiftModel>))]
        public HttpResponseMessage GetAllShifts()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillController,GetAllShifts,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var shifts = _tillManager.GetAllShifts();
            var response = from shift in shifts
                           select new ShiftModel
                           {
                               Id = shift.Key,
                               ShiftNumber = shift.Value
                           };

            _performancelog.Debug($"End,TillController,GetAllShifts,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// Validate till close message
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <returns></returns>
        [Route("validateTillClose")]
        [HttpGet]
        [ApiAuthorization]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(MessageStyle))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TillCloseResponse))]
        public HttpResponseMessage ValidateTillClose(int tillNumber, int saleNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillController,ValidateTillClose,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string userCode;
            ErrorMessage errorMessage;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = _tillCloseManager.ValidateTillClose(tillNumber, saleNumber, userCode, out errorMessage);

            _performancelog.Debug($"End,TillController,ValidateTillClose,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                //if there is any message
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Method to end shift
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        ///  /// <param name="saleNumber">Sale number</param>
        /// <returns></returns>
        [Route("endShift")]
        [HttpGet]
        [ApiAuthorization]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(MessageStyle))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SuccessReponse))]
        public HttpResponseMessage EndShift(int tillNumber, int saleNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillController,EndShift,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string userCode;
            ErrorMessage errorMessage;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = _tillCloseManager.EndSale(tillNumber, saleNumber, userCode, out errorMessage);

            _performancelog.Debug($"End,TillController,EndShift,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                //if there is any message
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessReponse
            {
                Success = result
            });
        }

        /// <summary>
        /// Method to read tank dip
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns></returns>
        [Route("readTankDip")]
        [HttpGet]
        [ApiAuthorization]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(MessageStyle))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SuccessReponse))]
        public HttpResponseMessage ReadTankDip(int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillController,ReadTankDip,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            ErrorMessage errorMessage;
            var result = _tillCloseManager.ReadTankDip(tillNumber, out errorMessage);

            _performancelog.Debug($"End,TillController,ReadTankDip,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                //if there is any message
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessReponse
            {
                Success = result
            });
        }

        /// <summary>
        /// Method to read tank dip
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <returns></returns>
        [Route("closeTill")]
        [HttpGet]
        [ApiAuthorization]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(InvalidLoginReponseModel))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CloseCurrentTillResponseModel))]
        public HttpResponseMessage CloseTill(int tillNumber, int saleNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillController,CloseTill,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string userCode;
            ErrorMessage errorMessage;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var result = _tillCloseManager.TillClose(tillNumber, saleNumber, userCode, out errorMessage);

            _performancelog.Debug($"End,TillController,CloseTill,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message))
            {
                //if there is any message
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new InvalidLoginReponseModel
                    {
                        Error = errorMessage.MessageStyle,
                        ShutDownPOS = errorMessage.ShutDownPos
                    });
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Method to read tank dip
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="registerNumber">Sale number</param>
        /// <param name="readTankDip"></param>
        /// <param name="readTotaliser"></param>
        /// <returns></returns>
        [Route("finishClose")]
        [HttpGet]
        [ApiAuthorization]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FinishTillCloseResponseModel))]
        public HttpResponseMessage FinishTillClose(int tillNumber, int registerNumber, bool? readTankDip, bool? readTotaliser)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,TillController,FinishTillClose,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            string userCode;
            ErrorMessage errorMessage;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            CustomerDisplay lcdMessage;
            var reports = _tillCloseManager.FinishTillClose(tillNumber, userCode, registerNumber,
                readTankDip, readTotaliser, out lcdMessage, out errorMessage);

            _performancelog.Debug($"End,TillController,FinishTillClose,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            if (!string.IsNullOrEmpty(errorMessage.MessageStyle.Message) && errorMessage.StatusCode != HttpStatusCode.OK)
            {
                //if there is any message
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle,
                    });
            }
            var response = new FinishTillCloseResponseModel
            {
                Reports = reports,
                LcdMessage = lcdMessage,
                Message = errorMessage.MessageStyle
            };
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        #region Private methods

        /// <summary>
        /// Method to get user code 
        /// </summary>
        /// <param name="userCode">Usercode</param>
        /// <param name="httpResponseMessage">HttpResponse</param>
        /// <returns>True or false</returns>
        private bool GetUserCode(out string userCode, out HttpResponseMessage httpResponseMessage)
        {
            userCode = string.Empty;
            httpResponseMessage = null;
            var accessToken = Request.GetFirstHeaderValueOrDefault<string>("authToken");
            if (accessToken == null)
            {
                {
                    httpResponseMessage = Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        new ErrorResponse
                        {
                            Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                        });
                    return true;
                }
            }

            userCode = TokenValidator.GetUserCode(accessToken);
            return false;
        }

        #endregion
    }
}

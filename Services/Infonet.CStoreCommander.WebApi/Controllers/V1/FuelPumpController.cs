using Infonet.CStoreCommander.BusinessLayer.Manager;

using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Mapper;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.FuelPump;
using Infonet.CStoreCommander.WebApi.Models.Report;
using Infonet.CStoreCommander.WebApi.Models.Sale;
using Infonet.CStoreCommander.WebApi.Models.Tender;
using Infonet.CStoreCommander.WebApi.Resources;
using Infonet.CStoreCommander.WebApi.Utilities;
using log4net;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Fuel Pump Controller
    /// </summary>
    [RoutePrefix("api/v1/fuelPump")]
    [ApiAuthorization]
    public class FuelPumpController : ApiController
    {
        private readonly IFuelPumpManager _fuelPumpManager;
        private readonly IFuelPrepayManager _fuelPrepayManager;
        private readonly ISaleManager _saleManager;
        private readonly IPropaneManager _propaneManager;
        private readonly IUnCompletePrepayManager _unCompletePrepayManager;
        private readonly ITierLevelManager _tierLevelManager;

        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="fuelPumpManager"></param>
        /// <param name="saleManager"></param>
        /// <param name="fuelPrepayManager"></param>
        /// <param name="propaneManager"></param>
        /// <param name="unCompletePrepayManager"></param>
        /// <param name="tierLevelManager"></param>
        public FuelPumpController(IFuelPumpManager fuelPumpManager,
            ISaleManager saleManager,
            IFuelPrepayManager fuelPrepayManager,
            IPropaneManager propaneManager,
            IUnCompletePrepayManager unCompletePrepayManager,
            ITierLevelManager tierLevelManager)
        {
            _fuelPumpManager = fuelPumpManager;
            _saleManager = saleManager;
            _fuelPrepayManager = fuelPrepayManager;
            _propaneManager = propaneManager;
            _unCompletePrepayManager = unCompletePrepayManager;
            _tierLevelManager = tierLevelManager;
        }


        /// <summary>
        /// Method to initialize pumps
        /// </summary>
        /// <returns>Pumps List with Status</returns>
        [Route("intialize")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(PumpStatus))]
        public HttpResponseMessage InitializePumps(int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,InitializePumps,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error = new ErrorMessage();

            var pumps = _fuelPumpManager.LoadPumps(tillNumber);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                _performancelog.Debug($"End,FuelPumpController,InitializePumps,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }

            //var pumpModel = (from pump in pumps
            //                 select new PumpModel
            //                 {
            //                     PrepayText = pump.PrepayText,
            //                     EnableStackBasketBotton = pump.EnableStackBasketBotton,
            //                     BasketButtonVisible = pump.BasketButtonVisible,
            //                     EnableBasketBotton = pump.EnableBasketBotton,
            //                     BasketLabelCaption = pump.BasketLabelCaption,
            //                     PayPumporPrepay = pump.PayPumporPrepay,
            //                     PumpId = pump.PumpId,
            //                     Status = pump.Status,
            //                     PumpButtonCaption = pump.PumpButtonCaption,
            //                     BasketButtonCaption = pump.BasketButtonCaption
            //                 }).ToList();

            _performancelog.Debug($"End,FuelPumpController,InitializePumps,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, pumps);
        }

        /// <summary>
        /// Method to updateFuelPrice
        /// </summary>
        /// <returns></returns>
        [Route("updateFuelPrice")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(string))]
        public HttpResponseMessage UpdateFuelPumpPrice(int option, int counter)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,UpdateFuelPumpPrice,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;

            var pumps = _fuelPumpManager.UpdatePriceChange(option, counter, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                _performancelog.Debug($"End,FuelPumpController,UpdateFuelPumpPrice,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }

            _performancelog.Debug($"End,FuelPumpController,UpdateFuelPumpPrice,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, pumps);
        }

        /// <summary>
        /// Method to get Price change notification from Head Office
        /// </summary>
        /// <returns></returns>
        [Route("getHeadOfficeNotification")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetFuelPumpNotification()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,GetFuelPumpNotification,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var messageStyle = _fuelPumpManager.ReadPricheChangeNotificationHo();

            _performancelog.Debug($"End,FuelPumpController,GetFuelPumpNotification,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, messageStyle);
        }

        /// <summary>
        /// Method to get pump status
        /// </summary>
        /// <returns></returns>
        [Route("getPumpsStatus")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(PumpStatus))]
        public HttpResponseMessage GetPumpsStatus(int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,GetFuelPumpNotification,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var pumpStatus = _fuelPumpManager.ReadUdpData(tillNumber);

            _performancelog.Debug($"End,FuelPumpController,GetFuelPumpNotification,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, pumpStatus);
        }

        /// <summary>
        /// Method to update pump action
        /// </summary>
        /// <returns></returns>
        [Route("getPumpAction")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(BigPump))]
        public HttpResponseMessage GetPumpAction(short pumpId, bool stopPressed, bool resumePressed)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,GetPumpAction,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            ErrorMessage error;
            var pumpStatus = _fuelPumpManager.PumpAction(pumpId, stopPressed, resumePressed, out error);

            _performancelog.Debug($"End,FuelPumpController,GetPumpAction,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            return Request.CreateResponse(HttpStatusCode.OK, pumpStatus);
        }

        /// <summary>
        /// Stop broadcast of big pump
        /// </summary>
        /// <returns></returns>
        [Route("stopBroadcast")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        public HttpResponseMessage StopBroadcast()
        {
            var result = _fuelPumpManager.DisableFramePump();
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Loads all the grades of the supplied pump
        /// </summary>
        /// <param name="pumpId"></param>
        /// <param name="switchPrepay">Switches for any existing prepays</param>
        /// <param name="tillNumber">Till number for the sale</param>
        /// <returns></returns>
        [Route("loadGrades")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(List<string>))]
        public HttpResponseMessage LoadPumpGrades(short pumpId, bool switchPrepay, short tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,LoadPumpGrades,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            //  - Including the switch Prepay baskets too - "CHANGE PREPAY GRADE"
            if (switchPrepay)
            {
                //  - "CHANGE PREPAY GRADE" - Need to process any outstanding switch Prepay baskets - PM suggested to do it here( need to do it here and before exiting out from POS)
                string changeDue;
                bool openDrawer;
                Report fs;
                _fuelPrepayManager.Finish_SwitchPrepayBaskets(tillNumber, out changeDue,
                    out openDrawer, out fs);
            }

            var pumpGrades = _fuelPumpManager.LoadPumpGrades(pumpId);

            if (pumpGrades == null || pumpGrades.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    Error = new MessageStyle
                    {
                        Message = Constants.NoPumpsDefined
                    }
                });
            }

            _performancelog.Debug($"End,FuelPumpController,LoadPumpGrades,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, pumpGrades);
        }

        /// <summary>
        /// Adds a fuel item to the selected pump manually
        /// </summary>
        /// <returns></returns>
        [Route("addManually")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, Type = typeof(ErrorResponse))]
        public HttpResponseMessage AddFuelManually([FromBody] ManualFuelModel fuelModel)
        {
            ErrorMessage error;
            var accessToken = Request.GetFirstHeaderValueOrDefault<string>("authToken");
            if (accessToken == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = Resource.Error }
                    });
            }

            var userCode = TokenValidator.GetUserCode(accessToken);

            if (fuelModel == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle
                        {
                            Message = Resource.InvalidRequest,
                            MessageType = MessageType.OkOnly
                        },
                    });
            }

            var sale = _fuelPumpManager.AddFuelManually(fuelModel.SaleNumber, fuelModel.TillNumber,
                fuelModel.RegisterNumber, userCode, out error, fuelModel.Amount, fuelModel.Grade,
                fuelModel.PumpId, fuelModel.IsCashSelected);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }

            if (sale != null)
            {
                object saleModel;
                var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                var enableButtons = _saleManager.EnableCashButton(sale, userCode);
                var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);

                if (!string.IsNullOrEmpty(error.MessageStyle.Message))
                {
                    var message = new
                    {
                        error = error.MessageStyle,
                    };
                    var messages = new List<object> { message };

                    saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff, messages);
                }
                else
                {
                    saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
                }

                return Request.CreateResponse(HttpStatusCode.OK, saleModel);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
            {
                Error = new MessageStyle
                {
                    Message = Resource.InvalidRequest,
                    MessageType = MessageType.OkOnly
                }
            });
        }

        /// <summary>
        /// Add prepay fuel
        /// </summary>
        /// <returns></returns>
        [Route("prepay/add")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage AddPrepay([FromBody] AddPrepayModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,AddPrepay,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            var accessToken = Request.GetFirstHeaderValueOrDefault<string>("authToken");
            if (accessToken == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                    });
            }

            var userCode = TokenValidator.GetUserCode(accessToken);

            var sale = _fuelPrepayManager.AddPrepay(model.SaleNumber, model.TillNumber, model.ActivePump, model.Amount, model.FuelGrade, model.IsAmountCash, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            if (sale != null)
            {
                var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                var enableButton = _saleManager.EnableCashButton(sale, userCode);
                var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
                var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButton, userCanWriteOff);
                _performancelog.Debug($"End,FuelPumpController,AddPrepay,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.OK, saleModel);
            }


            return null;

        }

        /// <summary>
        /// Switch Prepay
        /// </summary>
        /// <returns></returns>
        [Route("prepay/switch")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage SwitchPrepay([FromBody] SwitchPrepayModel model)
        {
            ErrorMessage error;

            var result = _fuelPrepayManager.SwitchPrepay(model.ActivePump, model.NewPumpId, model.SaleNumber, model.TillNumber, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);

        }

        /// <summary>
        /// Delete Prepay
        /// </summary>
        /// <returns></returns>
        [Route("prepay/delete")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(SaleSummaryResponseModel))]
        public HttpResponseMessage DeletePrepay([FromBody] DeletePrepayModel model)
        {
            ErrorMessage error;
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,DeletePrepay,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var result = _fuelPrepayManager.DeletePrepay(model.ActivePump, model.SaleNumber, model.TillNumber, model.ShiftNumber, model.RegisterNumber, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }

            var response = new SaleSummaryResponseModel
            {
                SaleSummary = result.SaleSummary != null ? (from taxSumm in result.SaleSummary
                                                            select new NameValuePair
                                                            {
                                                                Key = taxSumm.Key,
                                                                Value = taxSumm.Value
                                                            }).ToList() : null,
                TenderSummary = result.Tenders != null ?
                TenderMapper.GetTenderSummaryModel(result.Tenders, string.Empty, result.Tenders) : null
            };

            _performancelog.Debug($"End,FuelPumpController,DeletePrepay,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// Resume all pumps
        /// </summary>
        /// <returns></returns>
        [Route("resumeall")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage ResumeAllPumps()
        {
            ErrorMessage error;

            var result = _fuelPumpManager.ResumeAllPumps(out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);

        }

        /// <summary>
        /// Stop all pumps
        /// </summary>
        /// <returns></returns>
        [Route("stopall")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage StopAllPumps()
        {
            ErrorMessage error;

            var result = _fuelPumpManager.StopAllPumps(out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);

        }

        /// <summary>
        /// Add fuel sale from Basket
        /// </summary>
        /// <returns></returns>
        [Route("basket/add")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage AddFuelSaleFromBasket([FromBody] FuelBasketModel model)
        {
           // bool  flag = false;
            //SaleModel s=null;
            WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 550");
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,AddPrepay,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            var accessToken = Request.GetFirstHeaderValueOrDefault<string>("authToken");
            if (accessToken == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                    });
            }

            var userCode = TokenValidator.GetUserCode(accessToken);

                WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside 572");
                var sale = _fuelPumpManager.AddFuelSaleFromBasket(model.SaleNumber, model.TillNumber, model.RegisterNumber, model.ActivePump, model.BasketValue, out error);
                WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 574");

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict,
                        new ErrorResponse
                        {
                            Error = error.MessageStyle,
                        });
                }

                var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                var enableButton = _saleManager.EnableCashButton(sale, userCode);
                var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
                var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButton, userCanWriteOff);
                _performancelog.Debug($"End,FuelPumpController,AddPrepay,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                WriteUDPData(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + "Inside if 584");
              //  s = saleModel;
            
                return Request.CreateResponse(HttpStatusCode.OK, saleModel);
            
        }

        /// <summary>
        /// Get Propane Grades
        /// </summary>
        /// <returns></returns>
        [Route("propane/loadGrades")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(PropaneGrade))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetPropaneGrades()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,GetPropaneGrades,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;

            var propaneGrades = _propaneManager.GetPropaneGrades(out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            _performancelog.Debug($"End,FuelPumpController,GetPropaneGrades,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, propaneGrades);
        }

        /// <summary>
        /// Get Propane Pumps
        /// </summary>
        /// <returns></returns>
        [Route("propane/loadPumps")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(PropanePump))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetPropanePumps(int gradeId)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,GetPropanePumps,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;

            var propanePumps = _propaneManager.GetPropanePumpsByGradeId(gradeId, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            _performancelog.Debug($"End,FuelPumpController,GetPropanePumps,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, propanePumps);
        }


        /// <summary>
        /// Add propane fuel sale from Basket
        /// </summary>
        /// <returns></returns>
        [Route("propane/add")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage AddPropaneFuelSale([FromBody] PropaneSaleItemModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,AddPropaneFuelSale,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            var accessToken = Request.GetFirstHeaderValueOrDefault<string>("authToken");
            if (accessToken == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                    });
            }

            var userCode = TokenValidator.GetUserCode(accessToken);

            var sale = _propaneManager.AddPropaneSale(model.GradeId, model.PumpId, model.SaleNumber, model.TillNumber, model.RegisterNumber, model.IsAmount, model.PropaneValue, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }

            var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
            var enableButton = _saleManager.EnableCashButton(sale, userCode);
            var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
            var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButton, userCanWriteOff);
            _performancelog.Debug($"End,FuelPumpController,AddPropaneFuelSale,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, saleModel);

        }

        /// <summary>
        /// Get Tier level for pumps
        /// </summary>
        /// <returns></returns>
        [Route("tierLevel/load")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(TierLevelResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage LoadTierLevels()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,LoadTierLevels,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var tierLevels = _tierLevelManager.GetAllPumps();
            _performancelog.Debug($"End,FuelPumpController,LoadTierLevels,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, tierLevels);
        }

        /// <summary>
        /// Update Tier Level for pumps
        /// </summary>
        /// <returns></returns>
        [Route("tierLevel/update")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(TierLevelResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage UpdateTierLevel([FromBody] TierLevelModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,UpdateTierLevel,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;

            var tierLevels = _tierLevelManager.AddUpdateTierLevel(model.PumpIds, model.TierId, model.LevelId, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            _performancelog.Debug($"End,FuelPumpController,UpdateTierLevel,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, tierLevels);
        }

        /// <summary>
        /// Load uncomplete prepays
        /// </summary>
        /// <returns></returns>
        [Route("uncompletePrepay/load")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(UnCompletePrepayResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage LoadUnCompletePrepay(int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,LoadUnCompletePrepay,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;

            var uncompletePrepays = _unCompletePrepayManager.LoadUncompleteGrid(tillNumber, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            _performancelog.Debug($"End,FuelPumpController,LoadUnCompletePrepay,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, uncompletePrepays);
        }

        /// <summary>
        /// Load uncomplete prepays
        /// </summary>
        /// <returns></returns>
        [Route("uncompletePrepay/change")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(ChangeUnCompleteResponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage ChangeUnCompletePrepay([FromBody] UnCompletePrepayInputModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,ChangeUnCompletePrepay,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string changeDue;
            bool openDrawer;
            var fs = _unCompletePrepayManager.ChangeUncompletePrepay(model.PumpId, model.SaleNum, model.TillNumber,
                model.FinishAmount, model.FinishQty, model.FinishPrice, model.PrepayAmount, model.PositionId, model.GradeId,
                out changeDue, out openDrawer, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }
            ReportModel receiptModel = null;
            if (fs != null)
            {
                receiptModel = new ReportModel
                {
                    ReportName = fs.ReportName,
                    ReportContent = fs.ReportContent,
                    Copies = 1
                };
            }

            var response = new ChangeUnCompleteResponseModel
            {
                ChangeDue = changeDue,
                OpenDrawer = openDrawer,
                TaxExemptReceipt = receiptModel
            };
            _performancelog.Debug($"End,FuelPumpController,ChangeUnCompletePrepay,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// Load uncomplete prepays
        /// </summary>
        /// <returns></returns>
        [Route("uncompletePrepay/overpayment")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(OverPaymentResponseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage OverPaymentUnCompletePrepay([FromBody] UnCompletePrepayInputModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,OverPaymentUnCompletePrepay,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            bool openDrawer;
            var fs = _unCompletePrepayManager.OverpaymentUncompletePrepay(model.PumpId, model.SaleNum, model.TillNumber,
                 model.FinishAmount, model.FinishQty, model.FinishPrice, model.PrepayAmount, model.PositionId, model.GradeId,
                 out openDrawer, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }

            ReportModel receiptModel = null;
            if (fs != null)
            {
                receiptModel = new ReportModel
                {
                    ReportName = fs.ReportName,
                    ReportContent = fs.ReportContent,
                    Copies = 1
                };
            }

            var response = new OverPaymentResponseModel
            {
                OpenDrawer = openDrawer,
                TaxExemptReceipt = receiptModel
            };
            _performancelog.Debug($"End,FuelPumpController,OverPaymentUnCompletePrepay,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// delete uncomplete prepay
        /// </summary>
        /// <returns></returns>
        [Route("uncompletePrepay/delete")]
        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SaleModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage DeleteUnCompletePrepay([FromBody] DeleteUnCompletePrepayModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,DeleteUnCompletePrepay,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            string userCode;
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var sale = _unCompletePrepayManager.DeleteUnCompletePrepay(model.PumpId, model.SaleNum, model.TillNumber, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }


            object saleModel;
            var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
            var enableButtons = _saleManager.EnableCashButton(sale, userCode);
            var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                var message = new
                {
                    error = error.MessageStyle,
                };
                var messages = new List<object> { message };

                saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff, messages);
            }
            else
            {
                saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
            }
            _performancelog.Debug($"End,SaleController,DeleteUnCompletePrepay,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, saleModel);

        }

        /// <summary>
        /// Get Propane Volume for Amount
        /// </summary>
        /// <returns></returns>
        [Route("propane/GetFuelVolume")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetFuelVolume([FromBody] PropaneVolumeModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,FuelPumpController,GetFuelVolume,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            var accessToken = Request.GetFirstHeaderValueOrDefault<string>("authToken");
            if (accessToken == null)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                    });
            }

            var userCode = TokenValidator.GetUserCode(accessToken);

            var sale = _propaneManager.GetVolumeValue(model.GradeId, model.PumpId, model.SaleNumber, model.TillNumber, model.RegisterNumber, model.PropaneValue, out error);

            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    new ErrorResponse
                    {
                        Error = error.MessageStyle,
                    });
            }

            return Request.CreateResponse(HttpStatusCode.OK, sale);

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


        public void WriteUDPData(string msgStr)
        {
            try
            {
                var logPath = @"C:\APILog\";
                var fileName = logPath + "PosLog_" + DateTime.Today.ToString("MM/dd/yyyy") + ".txt";

                using (StreamWriter fileWriter = new StreamWriter(fileName, true))
                {
                    fileWriter.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + Microsoft.VisualBasic.Strings.Space(3) + msgStr);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}

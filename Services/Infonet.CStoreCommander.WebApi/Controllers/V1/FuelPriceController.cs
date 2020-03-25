using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.FuelPump;
using Infonet.CStoreCommander.WebApi.Models.Report;
using Infonet.CStoreCommander.WebApi.Resources;
using log4net;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Fuel Price Controller
    /// </summary>
    [RoutePrefix("api/v1/fuelPrice")]
    [ApiAuthorization]
    public class FuelPriceController : ApiController
    {
        private readonly IFuelPumpManager _fuelPumpManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="fuelPumpManager"></param>
        public FuelPriceController(IFuelPumpManager fuelPumpManager)
        {
            _fuelPumpManager = fuelPumpManager;
        }

        /// <summary>
        /// Method to read totalizer reading
        /// </summary>
        /// <returns>Response message</returns>
        [Route("readTotalizer")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SuccessReponse))]
        public HttpResponseMessage ReadTotalizer(int tillNumber)
        {
            ErrorMessage errorMessage = new ErrorMessage();

            _fuelPumpManager.ReadTotalizer(tillNumber, ref errorMessage);

            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle.Message) && errorMessage.StatusCode != HttpStatusCode.OK)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle,
                    });
            }

            return Request.CreateResponse(HttpStatusCode.OK,
                new SuccessReponse
                {
                    Success = true
                });
        }

        /// <summary>
        /// Method to load grouped base prices
        /// </summary>
        /// <returns>Http Response</returns>
        [Route("loadGroupBasePrices")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(BaseGroupPricesModel))]
        public HttpResponseMessage LoadGroupBasePrices()
        {
            ErrorMessage errorMessage;
            string report = string.Empty;

            var basePrices = _fuelPumpManager.LoadGroupedBasePrices(ref report, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle.Message) && errorMessage.StatusCode != HttpStatusCode.OK)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle,
                    });
            }

            if (basePrices != null && basePrices.FuelPrices.Count > 0)
            {
                var basePricesModel = from bp in basePrices.FuelPrices
                                      select new GroupFuelPriceModel
                                      {
                                          Row = bp.Row,
                                          CashPrice = bp.CashPrice,
                                          CreditPrice = bp.CreditPrice,
                                          Grade = bp.Grade,
                                          TaxExemptedCashPrice = bp.TaxExemptedCashPrice,
                                          TaxExemptedCreditPrice = bp.TaxExemptedCreditPrice,
                                      };

                var model = new BaseGroupPricesModel
                {
                    FuelPrices = basePricesModel.ToList(),
                    Report = new ReportModel
                    {
                        ReportName = Constants.PriceFile,
                        ReportContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(report)),

                    },
                    CanReadTotalizer = basePrices.CanReadTotalizer,
                    IsCreditPriceEnabled = basePrices.IsCreditPriceEnabled,
                    IsIncrementEnabled = basePrices.IsIncrementEnabled,
                    CanSelectPricesToDisplay = basePrices.CanSelectPricesToDisplay,
                    Caption = basePrices.Caption,
                    IsCashPriceEnabled = basePrices.IsCashPriceEnabled,
                    IsErrorEnabled = basePrices.IsErrorEnabled,
                    IsExitEnabled = basePrices.IsExitEnabled,
                    IsPricesToDisplayChecked = basePrices.IsPricesToDisplayChecked,
                    IsPricesToDisplayEnabled = basePrices.IsPricesToDisplayEnabled,
                    IsReadTankDipChecked = basePrices.IsReadTankDipChecked,
                    IsReadTankDipEnabled = basePrices.IsReadTankDipEnabled,
                    IsReadTotalizerChecked = basePrices.IsReadTotalizerChecked,
                    IsReadTotalizerEnabled = basePrices.IsReadTotalizerEnabled,
                    IsTaxExemptedCashPriceEnabled = basePrices.IsTaxExemptedCashPriceEnabled,
                    IsTaxExemptedCreditPriceEnabled = basePrices.IsTaxExemptedCreditPriceEnabled,
                    IsTaxExemptionVisible = basePrices.IsTaxExemptionVisible
                };

                return Request.CreateResponse(HttpStatusCode.OK, model);
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
        /// Method to verify grouped base prices
        /// </summary>
        /// <param name="model">List of fuel price model</param>
        /// <returns>Http response</returns>
        [HttpPost]
        [Route("verifyGroupBasePrices")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Forbidden, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SuccessReponse))]
        public HttpResponseMessage VerifyGroupBasePrices([FromBody] List<GroupFuelPriceModel> model)
        {
            ErrorMessage errorMessage = default(ErrorMessage);

            var updatedPrices = (from f in model
                                 select new FuelPrice
                                 {
                                     CashPrice = f.CashPrice,
                                     CreditPrice = f.CreditPrice,
                                     Grade = f.Grade,
                                     TaxExemptedCashPrice = f.TaxExemptedCashPrice,
                                     TaxExemptedCreditPrice = f.TaxExemptedCreditPrice
                                 }).ToList();

            _fuelPumpManager.VerifyGroupedBasePrices(updatedPrices, ref errorMessage);

            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle.Message) && errorMessage.StatusCode != HttpStatusCode.OK)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle,
                    });
            }

            return Request.CreateResponse(HttpStatusCode.OK,
                new SuccessReponse
                {
                    Success = true
                });
        }

        /// <summary>
        /// Method to set grouped base price
        /// </summary>
        /// <param name="model">Fuel price model</param>
        /// <returns>Http response</returns>
        [HttpPost]
        [Route("setGroupBasePrice")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(BaseGroupPricesModel))]
        public HttpResponseMessage SetGroupBasePrice([FromBody] SetBaseGroupPriceModel model)
        {
            ErrorMessage errorMessage;
            string report = string.Empty;

            var prices = from fp in model.Prices
                         select new FuelPrice
                         {
                             CashPrice = fp.CashPrice,
                             CreditPrice = fp.CreditPrice,
                             Grade = fp.Grade,
                             Row = fp.Row,
                             TaxExemptedCashPrice = fp.TaxExemptedCashPrice,
                             TaxExemptedCreditPrice = fp.TaxExemptedCreditPrice,
                         };

            var basePrices = _fuelPumpManager.SetGroupedBasePrice(prices.ToList(), model.Row, ref report, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle.Message) && errorMessage.StatusCode != HttpStatusCode.OK)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle,
                    });
            }

            if (basePrices != null)
            {
                var response = (from f in basePrices
                                select new GroupFuelPriceModel
                                {
                                    Row = f.Row,
                                    CashPrice = f.CashPrice,
                                    CreditPrice = f.CreditPrice,
                                    Grade = f.Grade,
                                    TaxExemptedCashPrice = f.TaxExemptedCashPrice,
                                    TaxExemptedCreditPrice = f.TaxExemptedCreditPrice,
                                }).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new BaseGroupPricesModel
                {
                    FuelPrices = response,
                    Report = new ReportModel
                    {
                        ReportName = Constants.PriceFile,
                        ReportContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(report)),

                    }
                });
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
        /// Method to save group base prices
        /// </summary>
        /// <param name="model">List of fuel price</param>
        /// <returns>Http repsonse</returns>
        [Route("saveBaseGroupPrices")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Forbidden, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(FuelPriceResponse))]
        public HttpResponseMessage SaveGroupBasePrices([FromBody] SaveGroupPriceInputModel model)
        {
            ErrorMessage errorMessage;
            List<MessageStyle> messages = default(List<MessageStyle>);
            string priceRept = string.Empty;
            string fuelPriceRept = string.Empty;
            var updatedPrices = (from f in model.GroupFuelPrices
                                 select new FuelPrice
                                 {
                                     CashPrice = f.CashPrice,
                                     CreditPrice = f.CreditPrice,
                                     Grade = f.Grade,
                                     TaxExemptedCashPrice = f.TaxExemptedCashPrice,
                                     TaxExemptedCreditPrice = f.TaxExemptedCreditPrice
                                 }).ToList();

            var response = _fuelPumpManager.SaveGroupedBasePrices(model.TillNumber, updatedPrices, model.IsReadTotalizerChecked, model.IsReadTankDipChecked, model.IsPricesToDisplayChecked,
                false, out priceRept, out fuelPriceRept, out errorMessage, ref messages);

            if (!string.IsNullOrEmpty(response))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new FuelPriceResponse
                {
                    Error = errorMessage?.MessageStyle,
                    Caption = response,
                    PriceReport = new ReportModel
                    {
                        ReportName = Constants.PriceFile,
                        ReportContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(priceRept))

                    },
                    FuelPriceReport = new ReportModel
                    {
                        ReportName = Constants.FuelPriceFile,
                        ReportContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(fuelPriceRept)),

                    }
                });
            }

            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle.Message))
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle
                    });
            }

            if (messages != null && messages.Count > 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    messages);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new FuelPriceResponse
            {
                Error = errorMessage.MessageStyle,
                Caption = response,
                PriceReport = new ReportModel
                {
                    ReportName = Constants.PriceFile,
                    ReportContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(priceRept))

                },
                FuelPriceReport = new ReportModel
                {
                    ReportName = Constants.FuelPriceFile,
                    ReportContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(fuelPriceRept)),

                }
            });
        }

        /// <summary>
        /// Method to load base prices
        /// </summary>
        /// <returns>Http Response</returns>
        [Route("loadBasePrices")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(BasePricesModel))]
        public HttpResponseMessage LoadBasePrices()
        {
            ErrorMessage errorMessage;
            var report = string.Empty;
            var basePrices = _fuelPumpManager.LoadBasePrices(ref report, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle.Message) && errorMessage.StatusCode != HttpStatusCode.OK)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle,
                    });
            }

            if (basePrices != null && basePrices.FuelPrices.Count > 0)
            {
                var basePricesModel = from bp in basePrices.FuelPrices
                                      select new FuelPriceModel
                                      {
                                          Grade = bp.Grade,
                                          GradeId = bp.GradeId,
                                          Level = bp.Level,
                                          LevelId = bp.LevelId,
                                          Tier = bp.Tier,
                                          TierId = bp.TierId,
                                          CashPrice = bp.CashPrice,
                                          CreditPrice = bp.CreditPrice,
                                          TaxExemptedCashPrice = bp.TaxExemptedCashPrice,
                                          TaxExemptedCreditPrice = bp.TaxExemptedCreditPrice,
                                      };

                var model = new BasePricesModel
                {
                    FuelPrices = basePricesModel.ToList(),
                    Report = new ReportModel
                    {
                        ReportName = Constants.PriceFile,
                        ReportContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(report)),
                    },
                    IsTaxExemptionVisible = basePrices.IsTaxExemptionVisible,
                    CanReadTotalizer = basePrices.CanReadTotalizer,
                    CanSelectPricesToDisplay = basePrices.CanSelectPricesToDisplay,
                    Caption = basePrices.Caption,
                    IsErrorEnabled = basePrices.IsErrorEnabled,
                    IsExitEnabled = basePrices.IsExitEnabled,
                    IsPricesToDisplayChecked = basePrices.IsPricesToDisplayChecked,
                    IsPricesToDisplayEnabled = basePrices.IsPricesToDisplayEnabled,
                    IsReadTankDipChecked = basePrices.IsReadTankDipChecked,
                    IsReadTankDipEnabled = basePrices.IsReadTankDipEnabled,
                    IsReadTotalizerChecked = basePrices.IsReadTotalizerChecked,
                    IsReadTotalizerEnabled = basePrices.IsReadTotalizerEnabled,
                    IsCashPriceEnabled = basePrices.IsCashPriceEnabled,
                    IsCreditPriceEnabled = basePrices.IsCreditPriceEnabled,
                    IsTaxExemptedCashPriceEnabled = basePrices.IsTaxExemptedCashPriceEnabled,
                    IsTaxExemptedCreditPriceEnabled = basePrices.IsTaxExemptedCreditPriceEnabled
                };

                return Request.CreateResponse(HttpStatusCode.OK, model);
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
        /// Method to set base price
        /// </summary>
        /// <param name="model">Fuel price model</param>
        /// <returns>Http response</returns>
        [HttpPost]
        [Route("setBasePrice")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(FuelPriceModel))]
        public HttpResponseMessage SetBasePrice([FromBody] FuelPriceModel model)
        {
            ErrorMessage errorMessage;

            var fuelPrice = new FuelPrice
            {
                CashPrice = model.CashPrice,
                CreditPrice = model.CreditPrice,
                Grade = model.Grade,
                TaxExemptedCashPrice = model.TaxExemptedCashPrice,
                TaxExemptedCreditPrice = model.TaxExemptedCreditPrice,
                GradeId = model.GradeId,
                LevelId = model.LevelId,
                TierId = model.TierId,
                Level = model.Level,
                Tier = model.Tier
            };

            var updatedPrice = _fuelPumpManager.SetBasePrice(ref fuelPrice, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle.Message) && errorMessage.StatusCode != HttpStatusCode.OK)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle,
                    });
            }

            if (updatedPrice != null)
            {
                var response = new FuelPriceModel
                {
                    CashPrice = updatedPrice.CashPrice,
                    LevelId = updatedPrice.LevelId,
                    GradeId = updatedPrice.GradeId,
                    CreditPrice = updatedPrice.CreditPrice,
                    Grade = updatedPrice.Grade,
                    Level = updatedPrice.Level,
                    TaxExemptedCashPrice = updatedPrice.TaxExemptedCashPrice,
                    TaxExemptedCreditPrice = updatedPrice.TaxExemptedCreditPrice,
                    Tier = updatedPrice.Tier,
                    TierId = updatedPrice.TierId
                };

                return Request.CreateResponse(HttpStatusCode.OK, response);
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
        /// Method to Verify base prices
        /// </summary>
        /// <param name="model">List of fuel price</param>
        /// <returns>Http repsonse</returns>
        [Route("verifyBasePrices")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(MessageStyle))]
        [SwaggerResponse(HttpStatusCode.Forbidden, Description = "", Type = typeof(MessageStyle))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "", Type = typeof(MessageStyle))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SuccessReponse))]
        public HttpResponseMessage VerifyBasePrices([FromBody] BasePricesSaveModel model)
        {
            ErrorMessage errorMessage;
            var caption2 = string.Empty;

            var updatedPrices = (from f in model.FuelPrices
                                 select new FuelPrice
                                 {
                                     CashPrice = f.CashPrice,
                                     CreditPrice = f.CreditPrice,
                                     Grade = f.Grade,
                                     TaxExemptedCashPrice = f.TaxExemptedCashPrice,
                                     TaxExemptedCreditPrice = f.TaxExemptedCreditPrice,
                                     GradeId = f.GradeId,
                                     LevelId = f.LevelId,
                                     TierId = f.TierId
                                 }).ToList();

            _fuelPumpManager.VerifyBasePrices(model.TillNumber, updatedPrices, model.IsPricesToDisplayChecked, model.IsReadTankDipChecked,
                 model.IsReadTotalizerChecked, out errorMessage, ref caption2);

            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle.Message)
                && errorMessage.StatusCode != HttpStatusCode.OK)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle,
                    });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new SuccessReponse
            {
                Success = true
            });
        }

        /// <summary>
        /// Method to save base prices
        /// </summary>
        /// <param name="model">List of fuel price</param>
        /// <returns>Http repsonse</returns>
        [Route("saveBasePrices")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(MessageStyle))]
        [SwaggerResponse(HttpStatusCode.Forbidden, Description = "", Type = typeof(MessageStyle))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "", Type = typeof(MessageStyle))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(ErrorResponseWithCaption))]
        public HttpResponseMessage SaveBasePrices([FromBody] BasePricesSaveModel model)
        {
            ErrorMessage errorMessage;
            var caption2 = string.Empty;
            var updatedPrices = (from f in model.FuelPrices
                                 select new FuelPrice
                                 {
                                     CashPrice = f.CashPrice,
                                     CreditPrice = f.CreditPrice,
                                     Grade = f.Grade,
                                     TaxExemptedCashPrice = f.TaxExemptedCashPrice,
                                     TaxExemptedCreditPrice = f.TaxExemptedCreditPrice,
                                     GradeId = f.GradeId,
                                     LevelId = f.LevelId,
                                     TierId = f.TierId
                                 }).ToList();

            var message = _fuelPumpManager.SaveBasePrices(model.TillNumber, updatedPrices, model.IsPricesToDisplayChecked, model.IsReadTankDipChecked,
                model.IsReadTotalizerChecked, out errorMessage, ref caption2);

            if (!string.IsNullOrEmpty(message))
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new ErrorResponseWithCaption
                    {
                        Error = errorMessage.MessageStyle,
                        Caption = string.Format("{0};{1}", message, caption2)
                    });
            }

            if (!string.IsNullOrEmpty(errorMessage?.MessageStyle.Message)
                && errorMessage.StatusCode != HttpStatusCode.OK)
            {
                return Request.CreateResponse(errorMessage.StatusCode,
                    new ErrorResponse
                    {
                        Error = errorMessage.MessageStyle,
                    });
            }

            return Request.CreateResponse(HttpStatusCode.OK,
                    new ErrorResponseWithCaption
                    {
                        Error = errorMessage.MessageStyle,
                        Caption = string.Format("{0};{1}", message, caption2)
                    });
        }

        /// <summary>
        /// Method to load prices to display
        /// </summary>
        /// <returns></returns>
        [Route("loadPricesToDisplay")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(PriceToDisplay))]
        public HttpResponseMessage LoadPricesToDisplay()
        {
            var pricesToDisplay = _fuelPumpManager.LoadPricesToDisplay();

            return Request.CreateResponse(HttpStatusCode.OK, pricesToDisplay);
        }

        /// <summary>
        /// Method to save prices to display
        /// </summary>
        /// <returns></returns>
        [Route("savePricesToDisplay")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SuccessReponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(MessageStyle))]
        public HttpResponseMessage SavePricesToDisplay([FromBody] SavePricesToDisplayModel model)
        {
            var result = _fuelPumpManager.SavePricesToDisplay(model.SelectedGrades,
                model.SelectedTiers, model.SelectedLevels);

            if (result)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new SuccessReponse
                {
                    Success = true
                });
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, new MessageStyle
            {
                Message = Resource.InvalidRequest
            });
        }

        /// <summary>
        /// Method to load price increment and decremenet
        /// </summary>
        /// <returns></returns>
        [Route("loadPriceIncrementDecrement")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(ChangeIncrement))]
        public HttpResponseMessage LoadPriceIncrementDecrement(bool taxExempt)
        {
            var prices = _fuelPumpManager.LoadChangeIncrement(taxExempt);

            return Request.CreateResponse(HttpStatusCode.OK, prices);
        }

        /// <summary>
        /// Method to set price increment
        /// </summary>
        /// <returns></returns>
        [Route("setPriceIncrement")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SetPriceIncrementModel))]
        public HttpResponseMessage SetPriceIncrement([FromBody] PriceIncrementPayloadModel model)
        {
            var error = new ErrorMessage();
            var report = string.Empty;
            var price = _fuelPumpManager.SetPriceIncrement(model.Price, model.TaxExempt, ref error, ref report);

            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return Request.CreateResponse(error.StatusCode,
                   new ErrorResponse
                   {
                       Error = error.MessageStyle,
                   });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new SetPriceIncrementModel
            {
                Price = price,
                Report = new ReportModel
                {
                    ReportName = Constants.PriceFile,
                    ReportContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(report)),
                }
            });
        }

        /// <summary>
        /// Method to set price decrement
        /// </summary>
        /// <returns></returns>
        [Route("setPriceDecrement")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(SetPriceDecrementModel))]
        public HttpResponseMessage SetPriceDecrement([FromBody] PriceDecrementPayloadModel model)
        {
            var error = new ErrorMessage();
            var report = string.Empty;
            var price = _fuelPumpManager.SetPriceDecrement(model.Price, model.TaxExempt, ref error, ref report);

            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return Request.CreateResponse(error.StatusCode,
                   new ErrorResponse
                   {
                       Error = error.MessageStyle,
                   });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new SetPriceDecrementModel
            {
                Price = price,
                Report = new ReportModel
                {
                    ReportName = Constants.PriceFile,
                    ReportContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(report)),
                }
            });
        }
    }
}
using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Resources;
using Infonet.CStoreCommander.WebApi.Utilities;
using log4net;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Report;
using Dept = Infonet.CStoreCommander.WebApi.Models.Report.Dept;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for Reports
    /// </summary>
    [RoutePrefix("api/v1/report")]
    [ApiAuthorization]
    public class ReportController : ApiController
    {
        private readonly IReceiptManager _receiptManager;
        private readonly ITillService _tillService;
        private readonly IApiResourceManager _resourceManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="receiptManager"></param>
        /// <param name="tillService"></param>
        /// <param name="resourceManager"></param>
        public ReportController(IReceiptManager receiptManager, ITillService tillService,
            IApiResourceManager resourceManager)
        {
            _receiptManager = receiptManager;
            _tillService = tillService;
            _resourceManager = resourceManager;
        }
        /// <summary>
        /// Method to get sales receipt header
        /// </summary>
        /// <returns>List of string</returns>
        [Route("getReceiptHeader")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(List<string>))]
        public HttpResponseMessage GetReceiptHeader()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReportController,GetReceiptHeader,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            try
            {
                List<string> olist = _receiptManager.GetReceiptHeader();
                _performancelog.Debug($"End,ReportController,GetReceiptHeader,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return Request.CreateResponse(HttpStatusCode.OK, olist);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }




        }
        /// <summary>
        /// Method to get sales count summary report
        /// </summary>
        /// <returns>List of departments</returns>
        [Route("getSaleSummaryReport")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(ReportModel))]
        public HttpResponseMessage GetSaleSummaryReport(string departmentId, int tillNumber, int shiftNumber,
            int loggedTillNumber)
        {

            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReportController,GetSaleSummaryReport,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            var till = _tillService.GetTill(loggedTillNumber);
            if (till == null)
            {
                var errorMessage = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = Resource.TillNotExist
                    },

                };
                _performancelog.Debug($"End,ReportController,GetSaleSummaryReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });

            }
            var error = _receiptManager.IsValidateReportCriteria(departmentId, tillNumber, shiftNumber);
            if (!string.IsNullOrEmpty(error.Message))
            {
                _performancelog.Debug($"End,ReportController,GetSaleSummaryReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = error
                });
            }
            var fs = _receiptManager.PrintSaleCountReport(departmentId, tillNumber, shiftNumber, till);
            try
            {
                var content = Helper.CreateBytes(fs);
                var salesCountModel = new ReportModel
                {
                    ReportName = Constants.SalesCountFile,
                    ReportContent = content,
                    Copies = 1
                };
                _performancelog.Debug($"End,ReportController,GetSaleSummaryReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return Request.CreateResponse(HttpStatusCode.OK, salesCountModel);
            }
            catch
            {
                _performancelog.Debug($"End,ReportController,GetSaleSummaryReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                });
            }
        }

        /// <summary>
        /// Method to get sales count summary report
        /// </summary>
        /// <returns>List of departments</returns>
        [Route("getFlashReport")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(FlashReportModel))]
        public HttpResponseMessage GetFlashReport(int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReportController,GetFlashReport,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var till = _tillService.GetTill(tillNumber);
            if (till == null)
            {
                var errorMessage = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = Resource.TillNotExist
                    },

                };
                _performancelog.Debug($"End,ReportController,GetFlashReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });

            }
            if (!_receiptManager.IsFlashReportAvailable(tillNumber))
            {
                var store = CacheManager.GetStoreInfo();
                var offSet = store?.OffSet ?? 0;
                MessageType temp_VbStyle11 = (int)MessageType.Exclamation + MessageType.OkOnly;
                var errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5688, null, temp_VbStyle11)

                };
                _performancelog.Debug($"End,ReportController,GetFlashReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }
            var totals = _receiptManager.GetTotalsForFlashReport(tillNumber);
            var departments = _receiptManager.GetDepartmentDetailsForFlashReport(tillNumber);
            var fs = _receiptManager.PrintFlashReport(till, totals, departments);
            try
            {
                var content = Helper.CreateBytes(fs);
                FlashReportModel flashReport = CreateFlashReport(totals, departments, content);
                _performancelog.Debug($"End,ReportController,GetFlashReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return Request.CreateResponse(HttpStatusCode.OK, flashReport);
            }
            catch
            {
                _performancelog.Debug($"End,ReportController,GetTillAuditReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                });
            }


        }

        /// <summary>
        /// Method to get sales count summary report
        /// </summary>
        /// <returns>List of departments</returns>
        [Route("getTillAuditReport")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(ReportModel))]
        public HttpResponseMessage GetTillAuditReport(int tillNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReportController,GetTillAuditReport,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var till = _tillService.GetTill(tillNumber);
            if (till == null)
            {
                var errorMessage = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = Resource.TillNotExist
                    },

                };
                _performancelog.Debug($"End,ReportController,GetTillAuditReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });

            }
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
            if (!_receiptManager.UserCanAuditTill(userCode))
            {
                var store = CacheManager.GetStoreInfo();
                var offSet = store?.OffSet ?? 0;
                MessageType temp_VbStyle11 = (int)MessageType.Exclamation + MessageType.OkOnly;
                var errorMessage = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 0, 5692, null, temp_VbStyle11)

                };
                _performancelog.Debug($"End,ReportController,GetTillAuditReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.Forbidden, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });
            }
            var fs = _receiptManager.PrintTillAuditReport(till);
            try
            {
                var content = Helper.CreateBytes(fs);
                var tillAuditReport = new ReportModel
                {
                    ReportName = Constants.TillAuditFile,
                    ReportContent = content,
                    Copies = 1
                };
                _performancelog.Debug($"End,ReportController,GetTillAuditReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.OK, tillAuditReport);
            }
            catch
            {
                _performancelog.Debug($"End,ReportController,GetTillAuditReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                });
            }
        }


        /// <summary>
        /// Get list of all report names
        /// </summary>
        /// <returns>List of reports</returns>
        [Route("getReprintReportNames")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ReprintReport>))]
        public HttpResponseMessage GetReprintReportList()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReportController,Index,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var reports = _receiptManager.GetReprintReports();
            _performancelog.Debug($"End,ReportController,Index,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, reports);
        }




        /// <summary>
        /// Get list of all report names
        /// </summary>
        /// <returns>List of reports</returns>
        [Route("sales")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ReprintSale>))]
        public HttpResponseMessage GetReprintSales(string reportType, DateTime? date)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReportController,GetReprintSales,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            ErrorMessage error;
            var reports = _receiptManager.GetReprintSales(reportType, date,
            out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                _performancelog.Debug($"End,ReportController,GetReprintSales,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return Request.CreateResponse(error.StatusCode, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }
            _performancelog.Debug($"End,ReportController,GetReprintSales,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, reports);
        }


        /// <summary>
        /// Get report
        /// </summary>
        /// <returns>List of reports</returns>
        [Route("getReprintReport")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Report>))]
        public HttpResponseMessage GetReport(int saleNumber, DateTime? saleDate, string reportType)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReportController,GetReport,{string.Empty},{dateStart:hh.mm.ss.ffffff}");
            string fileName;
            ErrorMessage error;
            var reports = _receiptManager.GetReport(saleNumber, saleDate, reportType, out fileName,
                out error);
            reports.Reverse();
            reports.ForEach(x => { x.Copies = 1; });
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                _performancelog.Debug($"End,ReportController,GetReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
                return Request.CreateResponse(error.StatusCode, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }
            _performancelog.Debug($"End,ReportController,GetReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, reports);
        }

        #region Private methods

        /// <summary>
        /// Method to create flash report
        /// </summary>
        /// <param name="totals">Totals</param>
        /// <param name="departments">Departments</param>
        /// <param name="content">Report</param>
        /// <returns>Flash report model</returns>
        private FlashReportModel CreateFlashReport(FlashReportTotals totals, List<Department> departments, string content)
        {
            var flashReport = new FlashReportModel
            {
                Totals = new Totals
                {
                    ProductSales = totals.ProductSales.ToString(Constants.CurrencyFormat),
                    LineDiscount = (-1 * totals.LineDiscount).ToString(Constants.DiscountFormat),
                    InvoiceDiscount = (-1 * totals.InvoiceDiscount).ToString(Constants.DiscountFormat),
                    SalesAfterDiscount = totals.SalesAfterDiscount.ToString(Constants.DiscountFormat),
                    Taxes = totals.Tax.ToString(Constants.CurrencyFormat),
                    Charges = totals.Charge.ToString(Constants.CurrencyFormat),
                    Refunded = totals.Refund.ToString(Constants.DiscountFormat),
                    TotalsReceipts = totals.TotalReceipt.ToString(Constants.CurrencyFormat)
                },
                Report = new ReportModel
                {
                    ReportName = Constants.FlasReportFile,
                    ReportContent = content,
                    Copies = 1
                }
            };
            foreach (var department in departments)
            {
                flashReport.Departments.Add(new Dept
                {
                    Department = department.Dept,
                    Description = department.DeptName,
                    NetSales = department.Sales.ToString(Constants.CurrencyFormat)
                });
            }

            return flashReport;
        }

        #endregion



        /// <summary>
        /// Method to get Kickback report
        /// </summary>
        /// <returns>Kickback report</returns>
        [Route("printKickbackReport")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "", Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(ReportModel))]
        public HttpResponseMessage PrintKickbackReport(int points)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ReportController,PrintKickbackReport,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

        
            if (points == 0)
            {
                var errorMessage = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        Message = Resource.PointsNull
                    },

                };
                _performancelog.Debug($"End,ReportController,PrintKickbackReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = errorMessage.MessageStyle
                });

            }
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

       
            var fs = _receiptManager.PrintKickbackPoints(points);
            try
            {
                var content = Helper.CreateBytes(fs);
                var kickBackReport = new ReportModel
                {
                    ReportName = Constants.KickbackPoints,
                    ReportContent = content,
                    Copies = 1
                };
                _performancelog.Debug($"End,ReportController,PrintKickbackReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.OK, kickBackReport);
            }
            catch
            {
                _performancelog.Debug($"End,ReportController,PrintKickbackReport,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                });
            }


        }
    }
}

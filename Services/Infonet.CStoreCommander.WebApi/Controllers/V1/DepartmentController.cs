using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.WebApi.Filters;
using log4net;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infonet.CStoreCommander.WebApi.Models.Department;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for Department
    /// </summary>
    [RoutePrefix("api/v1/department")]
    [ApiAuthorization]
    public class DepartmentController : ApiController
    {
        private readonly IReceiptManager _receiptManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="receiptManager"></param>
        public DepartmentController(IReceiptManager receiptManager)
        {
            _receiptManager = receiptManager;
        }

        /// <summary>
        /// Method to get list of all departments
        /// </summary>
        /// <returns>List of departments</returns>
        [Route("getAllDepartments")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "", Type = typeof(List<DepartmentResponseModel>))]
        public HttpResponseMessage GetAllDepartments()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,DepartmentController,GetAllDepartments,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var departments = _receiptManager.GetAllDepartmets();
            var response = departments.Select(department => new DepartmentResponseModel
            {
                DepartmentId = department.Key,
                DepartmentName = department.Value
            }).ToList();
            _performancelog.Debug($"End,DepartmentController,GetAllDepartments,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}

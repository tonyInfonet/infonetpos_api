using Infonet.CStoreCommander.WebApi.Helpers;
using System;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Tracing;
using Infonet.CStoreCommander.Logging;
using log4net;

namespace Infonet.CStoreCommander.WebApi.Filters
{
    /// <summary>
    /// Logging Filter Class
    /// </summary>
    public class LoggingFilterAttribute : ActionFilterAttribute
    {
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Override method for each action executing
        /// </summary>
        /// <param name="filterContext">Filter context</param>
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            GlobalConfiguration.Configuration.Services.Replace(typeof(ITraceWriter), new NLogger());
            var trace = GlobalConfiguration.Configuration.Services.GetTraceWriter();
            trace.Info(filterContext.Request, "Controller : " + filterContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine + "Action : " + filterContext.ActionDescriptor.ActionName, "JSON", filterContext.ActionArguments);
            filterContext.Request.Properties[filterContext.ActionDescriptor.ActionName] = Stopwatch.StartNew();
        }

        /// <summary>
        /// Override method for each Action Executed
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            Stopwatch stopwatch = (Stopwatch)actionExecutedContext.Request.Properties[actionExecutedContext.ActionContext.ActionDescriptor.ActionName];
            _performancelog.Debug($"End,{actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName+"Controller"},{"OnActionExecuted - " + actionExecutedContext.ActionContext.ActionDescriptor.ActionName},{stopwatch.Elapsed.TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
        }
    }
}
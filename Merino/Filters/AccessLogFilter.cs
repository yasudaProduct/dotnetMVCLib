using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System.Security.Claims;

namespace Merino.Filters
{
    public class AccessLogFilter : IActionFilter
    {
        private readonly ILogger<AccessLogFilter> _logger;

        public AccessLogFilter(ILogger<AccessLogFilter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// アクションメソッド実行前の処理
        /// </summary>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _logger.LogTrace(getControllerName(filterContext) + "_" + getActionName(filterContext)+ "▼▼Start▼▼");
        }

        /// <summary>
        /// アクションメソッド実行後の処理
        /// </summary>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _logger.LogTrace(getControllerName(filterContext) + "_" + getActionName(filterContext) + "▲▲End▲▲");
        }

        /// <summary>
        /// アクセスログを出力する
        /// </summary>
        private void OutputAccessLog(FilterContext filterContext, string starOrtEnd)
        {
            Logger logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            try
            {
                var controllerActionDescriptor = filterContext.ActionDescriptor as ControllerActionDescriptor;
                var name = filterContext.HttpContext.User.FindFirst(ClaimTypes.Name) == null ? "no user" : filterContext.HttpContext.User.FindFirst(ClaimTypes.Name).Value;

                logger.Info($"Controller:{controllerActionDescriptor.ControllerName} Action:{controllerActionDescriptor.ActionName} User:{(name ?? "Not User")} {starOrtEnd}");
            }
            catch (Exception ex)
            {
                logger.Error("\r\n" + "ログ出力時にエラーが発生しました。" + ex);
            }
        }

        private string getControllerName(ActionExecutingContext filterContext)
        {
            var routeData = filterContext.RouteData;
            return routeData.Values["controller"].ToString();
        }
        private string getActionName(ActionExecutingContext filterContext)
        {
            var routeData = filterContext.RouteData;
            return routeData.Values["action"].ToString();
        }
        private string getControllerName(ActionExecutedContext filterContext)
        {
            var routeData = filterContext.RouteData;
            return routeData.Values["controller"].ToString();
        }
        private string getActionName(ActionExecutedContext filterContext)
        {
            var routeData = filterContext.RouteData;
            return routeData.Values["action"].ToString();
        }
    }
}

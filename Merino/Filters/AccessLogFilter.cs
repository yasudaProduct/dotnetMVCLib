using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System.Security.Claims;

namespace Merino.Filters
{
    /// <summary>
    /// アクセスログフィルタークラス
    /// </summary>
    public class AccessLogFilter : IActionFilter
    {
        private readonly ILogger<AccessLogFilter> _logger;

        /// <summary>
        /// AccessLogFilter クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="logger">ILogger インスタンス</param>
        public AccessLogFilter(ILogger<AccessLogFilter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// アクションメソッド実行前の処理
        /// </summary>
        /// <param name="filterContext">ActionExecutingContext インスタンス</param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _logger.LogTrace(getControllerName(filterContext) + "_" + getActionName(filterContext) + "▼▼Start▼▼");
        }

        /// <summary>
        /// アクションメソッド実行後の処理
        /// </summary>
        /// <param name="filterContext">ActionExecutedContext インスタンス</param>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _logger.LogTrace(getControllerName(filterContext) + "_" + getActionName(filterContext) + "▲▲End▲▲");
        }

        /// <summary>
        /// アクセスログを出力する
        /// </summary>
        /// <param name="filterContext">FilterContext インスタンス</param>
        /// <param name="starOrtEnd">開始または終了を表す文字列</param>
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

        /// <summary>
        /// ActionExecutingContext からコントローラー名を取得します。
        /// </summary>
        /// <param name="filterContext">ActionExecutingContext インスタンス</param>
        /// <returns>コントローラー名</returns>
        private string getControllerName(ActionExecutingContext filterContext)
        {
            var routeData = filterContext.RouteData;
            return routeData.Values["controller"].ToString();
        }

        /// <summary>
        /// ActionExecutingContext からアクション名を取得します。
        /// </summary>
        /// <param name="filterContext">ActionExecutingContext インスタンス</param>
        /// <returns>アクション名</returns>
        private string getActionName(ActionExecutingContext filterContext)
        {
            var routeData = filterContext.RouteData;
            return routeData.Values["action"].ToString();
        }

        /// <summary>
        /// ActionExecutedContext からコントローラー名を取得します。
        /// </summary>
        /// <param name="filterContext">ActionExecutedContext インスタンス</param>
        /// <returns>コントローラー名</returns>
        private string getControllerName(ActionExecutedContext filterContext)
        {
            var routeData = filterContext.RouteData;
            return routeData.Values["controller"].ToString();
        }

        /// <summary>
        /// ActionExecutedContext からアクション名を取得します。
        /// </summary>
        /// <param name="filterContext">ActionExecutedContext インスタンス</param>
        /// <returns>アクション名</returns>
        private string getActionName(ActionExecutedContext filterContext)
        {
            var routeData = filterContext.RouteData;
            return routeData.Values["action"].ToString();
        }
    }
}

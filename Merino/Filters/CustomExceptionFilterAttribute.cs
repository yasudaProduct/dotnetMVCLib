using Microsoft.AspNetCore.Mvc.Filters;

namespace Merino.Filters
{
    /// <summary>
    /// カスタム例外フィルター属性クラスです。
    /// </summary>
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// <see cref="CustomExceptionFilterAttribute"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public CustomExceptionFilterAttribute()
        {

        }

        /// <summary>
        /// 例外が発生したときに実行されるメソッドです。
        /// </summary>
        /// <param name="context">例外コンテキスト</param>
        public override void OnException(ExceptionContext context)
        {
            //_logger.LogError("例外発生：" + context.Exception.Message.ToString() + "\r\n");
            //_logger.LogError(context.Exception.StackTrace);
            Console.WriteLine("例外発生：" + context.Exception.Message.ToString());
            Console.WriteLine(context.Exception.StackTrace);

            //if (!_hostingEnvironment.IsDevelopment())
            //{
            //    // do nothing
            //    return;
            //}

            //TODO メール送信

            // var result = new ViewResult { ViewName = "Error"};
            //result.ViewData.Add("Exception", context.Exception);
            // context.Result = result;
        }
    }
}

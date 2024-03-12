using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merino.Filters
{
    public class CustomExceptionFilterAttribute: ExceptionFilterAttribute
    {

        public CustomExceptionFilterAttribute()
        {

        }

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

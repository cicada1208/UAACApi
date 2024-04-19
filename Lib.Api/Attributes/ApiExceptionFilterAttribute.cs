using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models;
using NLog;
using System.Net;

namespace Lib.Api.Attributes
{
    /// <summary>
    /// action exception handle and error response
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override void OnException(ExceptionContext context)
        {
            // ExceptionFilter 可攔截所有類型 Exception，除了 HttpResponseException。
            // 可撰寫多個 ExceptionFilter class 篩選特定 Exception，使每種篩選器僅處理特定 Exception。
            // 例如: if (context.Exception is XyzException) ...

            //var requestIP = context.HttpContext.Connection.RemoteIpAddress?.ToString();
            //var url = context.HttpContext.Request.GetEncodedUrl(); // $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host.Value}{context.HttpContext.Request.Path}{context.HttpContext.Request.QueryString.ToUriComponent()}";
            //var method = context.HttpContext.Request.Method;
            //var controllerName = context.RouteData.Values["controller"];
            //var actionName = context.RouteData.Values["action"];
            //var actionArguments = context.HttpContext.Items["__ActionArguments"];
            //var exception = Regex.Replace(context.Exception.ToString(), "\r\n", "@@");

            //var error = new
            //{
            //    RequestIP = requestIP,
            //    Url = url,
            //    Method = method,
            //    ControllerName = controllerName,
            //    ActionName = actionName,
            //    ActionArguments = actionArguments,
            //    Exception = exception
            //};

            //var errorJson = JsonConvert.SerializeObject(error, Formatting.Indented);
            //errorJson = Regex.Replace(errorJson, "@@", System.Environment.NewLine);

            // 記錄 ActionException，用於 LoggerMiddleware
            context.HttpContext.Items["__ActionException"] = context.Exception.ToString();

            // Error Response:
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Result = new JsonResult(new ApiError(HttpStatusCode.InternalServerError));

            // Error Log:
            ////NLog.Logger logger = NLog.LogManager.GetLogger($"{controllerName}.{actionName}");
            //logger.Error(errorJson);

            // 手動 throw HttpResponseException，後續其他 ExceptionFilter 便不會執行
            //throw new HttpResponseException(context.Response);

            base.OnException(context);
        }
    }
}

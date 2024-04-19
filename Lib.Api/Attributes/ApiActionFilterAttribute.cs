using Microsoft.AspNetCore.Mvc.Filters;

namespace Lib.Api.Attributes
{
    public class ApiActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 記錄 ActionArguments，用於 ApiExceptionFilterAttribute or LoggerMiddleware
            context.HttpContext.Items["__ActionArguments"] = context.ActionArguments;
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }
    }
}

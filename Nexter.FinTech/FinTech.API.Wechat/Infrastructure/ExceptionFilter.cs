using FinTech.API.Wechat.Dto;
using FinTech.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace FinTech.API.Wechat.Infrastructure
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public object Exception { get; set; }
        public object ErrorCode { get; set; }
    }

    public class ExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment _env;
        readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger, IHostingEnvironment env)
        {
            _logger = logger;
            _env = env;
        }



        public void OnException(ExceptionContext context)
        {
            if (context.Exception is BusinessViolation exception)
            {
                context.Result = new JsonResult(Result.Fail(exception.StatusCode, exception.Message));
            }
            else
            {
                var requestBody = context.HttpContext.Request.GetRequestBodyString();
                _logger.LogError(new EventId(context.Exception.HResult),
                    context.Exception,
                    $"requestUri:{context.HttpContext.Request.GetDisplayUrl()} requestBody: {requestBody}");
                var errorResponse = new ErrorResponse
                {
                    Message = "系统错误， 请再尝试。"
                };
                if (_env.IsDevelopment() || _env.IsEnvironment("Testing"))
                {
                    errorResponse.Exception = new
                    {
                        context.Exception.GetBaseException().Message,
                        context.Exception.StackTrace,
                        Class = context.Exception.GetType().Name
                    };
                }
                context.Result = new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status500InternalServerError };
            }
            context.ExceptionHandled = true;
        }
    }
}

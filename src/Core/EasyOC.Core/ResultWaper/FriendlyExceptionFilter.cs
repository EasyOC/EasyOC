using EasyOC.Core.ResultWaper.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Shell.Scope;
using System.Threading.Tasks;

namespace EasyOC.Core.Filter
{
    public sealed class FriendlyExceptionFilter : IAsyncExceptionFilter
    {


        /// <summary>
        /// 异常拦截
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            // 解析异常处理服务，实现自定义异常额外操作，如记录日志等
            var globalExceptionHandler = context.HttpContext.RequestServices.GetService<IGlobalExceptionHandler>();
            if (globalExceptionHandler != null)
            {
                await globalExceptionHandler.OnExceptionAsync(context);
            }

            // 如果异常在其他地方被标记了处理，那么这里不再处理
            if (context.ExceptionHandled) return;

            // 获取控制器信息
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var serviceProvider = ShellScope.Context.ServiceProvider;
            var LoggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            ILogger _logger = LoggerFactory.CreateLogger(actionDescriptor.ControllerTypeInfo.AsType());
            var exception = context.Exception;
            _logger.LogError(exception, exception.Message);

            // 解析异常信息
            var exceptionMetadata = UnifyContext.GetExceptionMetadata(context);

            // 判断是否是验证异常
            var isValidationException = exception is AppFriendlyException friendlyException && friendlyException.ValidationException;
            // 如果是验证异常，返回 400
            if (isValidationException) context.Result = new BadRequestResult();
            else
            {
                // 返回友好异常
                context.Result = new ContentResult()
                {
                    Content = exceptionMetadata.Errors.ToString(),
                    StatusCode = exceptionMetadata.StatusCode
                };
            }

        }
    }

}

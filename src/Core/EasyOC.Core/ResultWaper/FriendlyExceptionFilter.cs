using EasyOC.Core.ResultWaper.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Shell.Scope;
using System;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.Core.Filter
{
    public sealed class FriendlyExceptionFilter : IAsyncExceptionFilter, IOrderedFilter
    {
        public int Order => 999999;

        /// <summary>
        /// 异常拦截
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            // 获取控制器信息

            var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            ILogger _logger = loggerFactory.CreateLogger(actionDescriptor.ControllerTypeInfo.AsType());
            var exception = context.Exception;
            _logger.LogError(exception, exception.Message);

            // //释放资源
            // try
            // {
            //     await using var session = context.HttpContext.RequestServices.GetRequiredService<ISession>();
            //     if (session != null)
            //     {
            //         await session.CancelAsync();
            //     }
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine(e);
            // }
            // using (var freeSql = ShellScope.Services.GetRequiredService<IFreeSql>())
            // {
            //     freeSql
            // }
            if (typeof(Controller).IsAssignableFrom(actionDescriptor.ControllerTypeInfo)) return;

            // 解析异常处理服务，实现自定义异常额外操作，如记录日志等
            var globalExceptionHandler = context.HttpContext.RequestServices.GetService<IUnifyResultProvider>();
            if (globalExceptionHandler != null)
            {
                globalExceptionHandler.OnException(context, UnifyContext.GetExceptionMetadata(context));
            }

            // 如果异常在其他地方被标记了处理，那么这里不再处理
            if (context.ExceptionHandled) return;
            // 解析异常信息
            var exceptionMetadata = UnifyContext.GetExceptionMetadata(context);

            // 判断是否是验证异常
            var isValidationException = exception is AppFriendlyException friendlyException && friendlyException.ValidationException;
            // 如果是验证异常，返回 400
            if (isValidationException) context.Result = new BadRequestResult();
            else
            {
                context.Result = new JsonResult(new
                {
                    Message = exceptionMetadata.Errors, StatusCode = exceptionMetadata.StatusCode
                })
                {
                    StatusCode = exceptionMetadata.StatusCode
                };
            }
        }
    }

}

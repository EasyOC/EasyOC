using EasyOC.Core.ResultWaper.Extensions;
using EasyOC.Core.ResultWaper.Internal;
using EasyOC.Core.ResultWaper.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell.Scope;
using System;
using System.Linq;
using System.Reflection;

namespace EasyOC.Core.Filter
{
    public static class UnifyContext
    { /// <summary>
      /// 是否启用规范化结果
      /// </summary>
        internal static bool EnabledUnifyHandler = false;

        /// <summary>
        /// 规范化结果类型
        /// </summary>
        internal static Type RESTfulResultType = typeof(RESTfulResult<>);

        /// <summary>
        /// 规范化结果额外数据键
        /// </summary>
        internal static string UnifyResultExtrasKey = "UNIFY_RESULT_EXTRAS";


        /// <summary>
        /// 检查请求成功是否进行规范化处理
        /// </summary>
        /// <param name="method"></param>
        /// <param name="unifyResult"></param>
        /// <param name="isWebRequest"></param>
        /// <returns>返回 true 跳过处理，否则进行规范化处理</returns>
        internal static bool CheckSucceededNonUnify(MethodInfo method, out IUnifyResultProvider unifyResult, bool isWebRequest = true)
        {
            // 判断是否跳过规范化处理
            var isSkip = !EnabledUnifyHandler
                  || method.GetRealReturnType().HasImplementedRawGeneric(RESTfulResultType)
                  || method.CustomAttributes.Any(x => typeof(NonUnifyAttribute).IsAssignableFrom(x.AttributeType) || typeof(ProducesResponseTypeAttribute).IsAssignableFrom(x.AttributeType) || typeof(IApiResponseMetadataProvider).IsAssignableFrom(x.AttributeType))
                  || method.ReflectedType.IsDefined(typeof(NonUnifyAttribute), true);

            if (!isWebRequest)
            {
                unifyResult = null;
                return isSkip;
            }

            unifyResult = isSkip ? null : ShellScope.Current.ServiceProvider.GetService<IUnifyResultProvider>();
            return unifyResult == null || isSkip;
        }

        /// <summary>
        /// 检查短路状态码（>=400）是否进行规范化处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="unifyResult"></param>
        /// <returns>返回 true 跳过处理，否则进行规范化处理</returns>
        internal static bool CheckStatusCodeNonUnify(HttpContext context, out IUnifyResultProvider unifyResult)
        {
            // 获取终点路由特性
            var endpointFeature = context.Features.Get<IEndpointFeature>();
            if (endpointFeature == null) return (unifyResult = null) == null;

            // 判断是否跳过规范化处理
            var isSkip = !EnabledUnifyHandler
                    || context.GetMetadata<NonUnifyAttribute>() != null
                    || endpointFeature?.Endpoint?.Metadata?.GetMetadata<NonUnifyAttribute>() != null;

            unifyResult = isSkip ? null : context.RequestServices.GetService<IUnifyResultProvider>();
            return unifyResult == null || isSkip;
        }

        /// <summary>
        /// 获取异常元数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ExceptionMetadata GetExceptionMetadata(ExceptionContext context)
        {
            object errorCode = default;
            object errors = default;
            var statusCode = StatusCodes.Status500InternalServerError;
            var isValidationException = false; // 判断是否是验证异常

            // 判断是否是友好异常
            if (context.Exception is AppFriendlyException friendlyException)
            {
                errorCode = friendlyException.ErrorCode;
                statusCode = friendlyException.StatusCode;
                isValidationException = friendlyException.ValidationException;
                errors = friendlyException.ErrorMessage;
            }

            // 处理验证失败异常
            if (!isValidationException)
            {
                errors = context.Exception?.InnerException?.Message ?? context.Exception.Message;
            }

            return new ExceptionMetadata
            {
                StatusCode = statusCode,
                ErrorCode = errorCode,
                Errors = errors
            };
        }


        /// <summary>
        /// 检查是否是有效的结果（可进行规范化的结果）
        /// </summary>
        /// <param name="result"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static bool CheckVaildResult(IActionResult result, out object data)
        {
            data = default;

            // 排除以下结果，跳过规范化处理
            var isDataResult = result switch
            {
                ViewResult => false,
                PartialViewResult => false,
                FileResult => false,
                ChallengeResult => false,
                SignInResult => false,
                SignOutResult => false,
                RedirectToPageResult => false,
                RedirectToRouteResult => false,
                RedirectResult => false,
                RedirectToActionResult => false,
                LocalRedirectResult => false,
                ForbidResult => false,
                ViewComponentResult => false,
                PageResult => false,
                _ => true,
            };

            // 目前支持返回值 ActionResult
            if (isDataResult) data = result switch
            {
                // 处理内容结果
                ContentResult content => content.Content,
                // 处理对象结果
                ObjectResult obj => obj.Value,
                // 处理 JSON 对象
                JsonResult json => json.Value,
                _ => null,
            };

            return isDataResult;
        }
        public static object TakeExtras(this HttpContext httpContext)
        {
            object extras = null;
            httpContext?.Items?.TryGetValue(UnifyResultExtrasKey, out extras);
            return extras;
        }
    }
}

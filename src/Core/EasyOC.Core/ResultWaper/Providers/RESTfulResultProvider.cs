using EasyOC.Core.Filter;
using EasyOC.Core.ResultWaper.Internal;
using EasyOC.Core.ResultWaper.UnifyResult.Attributes;
using EasyOC.Core.ResultWaper.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EasyOC;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Modules;
using System;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EasyOC.Core.ResultWaper.Providers
{
    /// <summary>
    /// RESTful 风格返回值
    /// </summary>
    [UnifyModel(typeof(RESTfulResult<>))]
    public class RESTfulResultProvider : IUnifyResultProvider
    {
        private readonly INotifier _notifier;
        private readonly HtmlEncoder _htmlEncoder;
        public RESTfulResultProvider(INotifier notifier, HtmlEncoder htmlEncoder)
        {
            _notifier = notifier;
            _htmlEncoder = htmlEncoder;
        }

        /// <summary>
        /// 异常返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public IActionResult OnException(ExceptionContext context, ExceptionMetadata metadata)
        {
            return new JsonResult(RESTfulResult(metadata.StatusCode, message: metadata.Errors, httpContext: context.HttpContext));
        }

        /// <summary>
        /// 成功返回值
        /// 同时处理 Message <see cref="NotifyEntry"/>
        /// <seealso cref="NotifierExtensions.SuccessAsync(INotifier, Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString)"/> 
        /// <seealso cref="NotifierExtensions.ErrorAsync(INotifier, Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString)"/> 
        /// <seealso cref="NotifierExtensions.InformationAsync(INotifier, Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString)"/> 
        /// <seealso cref="NotifierExtensions.WarningAsync(INotifier, Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString)"/> 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public IActionResult OnSucceeded(ActionExecutedContext context, object data)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new NotifyEntryConverter(_htmlEncoder));
            var notifyList = _notifier.List().ToArray().Select(x => new
            {
                Message = new
                {
                    //对Message解码
                    Value = WebUtility.HtmlDecode(x.GetMessageAsString(_htmlEncoder)),
                },
                x.Type,
            });
            _notifier.List().Clear();


            return new JsonResult(RESTfulResult(StatusCodes.Status200OK, true, data,
            message: notifyList,//处理OC 的代码内输出的消息
            httpContext: context.HttpContext));
        }



        /// <summary>
        /// 验证失败返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public IActionResult OnValidateFailed(ActionExecutingContext context, ValidationMetadata metadata)
        {
            return new JsonResult(RESTfulResult(StatusCodes.Status400BadRequest, message: metadata.ValidationResult, httpContext: context.HttpContext));
        }

        /// <summary>
        /// 特定状态码返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public async Task OnResponseStatusCodes(HttpContext context, int statusCode)
        {
            switch (statusCode)
            {
                // 处理 401 状态码
                case StatusCodes.Status401Unauthorized:
                    await context.Response.WriteAsJsonAsync(RESTfulResult(statusCode, message: "401 Unauthorized", httpContext: context)
                    );
                    break;
                // 处理 403 状态码
                case StatusCodes.Status403Forbidden:
                    await context.Response.WriteAsJsonAsync(RESTfulResult(statusCode, message: "403 Forbidden", httpContext: context)
                    );
                    break;

                default: break;
            }
        }

        /// <summary>
        /// 返回 RESTful 风格结果集
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="succeeded"></param>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private static RESTfulResult<object> RESTfulResult(int statusCode, bool succeeded = default, object data = default, object message = default, HttpContext httpContext = default)
        {
            return new RESTfulResult<object>
            {
                StatusCode = statusCode,
                Status = statusCode == 200 ? 0 : statusCode,
                Succeeded = succeeded,
                Data = data,
                Msg = message,
                Extras = httpContext.TakeExtras(),
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
        }

    }
}
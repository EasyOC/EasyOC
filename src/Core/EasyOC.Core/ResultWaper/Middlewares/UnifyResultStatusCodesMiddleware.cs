using EasyOC.Core.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace EasyOC.Core.ResultWaper.Middlewares
{
    /// <summary>
    /// 状态码中间件
    /// </summary>
    public class UnifyResultStatusCodesMiddleware
    {
        /// <summary>
        /// 请求委托
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="next"></param>
        public UnifyResultStatusCodesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 中间件执行方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            // 只有请求错误（短路状态码）才支持规范化处理
            if (context.Response.StatusCode < 400 || context.Response.StatusCode == 404) return;

            // 处理规范化结果
            if (!UnifyContext.CheckStatusCodeNonUnify(context, out var unifyResult))
            {
                await unifyResult.OnResponseStatusCodes(context, context.Response.StatusCode);
            }
        }
    }
}
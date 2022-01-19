
using EasyOC.Core.Filter;
using EasyOC.Core.ResultWaper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using OrchardCore.DisplayManagement.Notify;
using System.Linq;
using System.Threading.Tasks;

namespace Furion.UnifyResult
{
    /// <summary>
    /// 规范化结构（请求成功）过滤器
    /// </summary>
    public class SucceededUnifyResultFilter : IAsyncActionFilter, IOrderedFilter
    { 
        /// <summary>
        /// 过滤器排序
        /// </summary>
        internal const int FilterOrder = 8888;

        /// <summary>
        /// 排序属性
        /// </summary>
        public int Order => FilterOrder;

        /// <summary>
        /// 处理规范化结果
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 执行 Action 并获取结果
            var actionExecutedContext = await next();
            //context.Result = actionExecutedContext.Result;
            // 如果出现异常，则不会进入该过滤器
            if (actionExecutedContext.Exception != null) return;

            // 获取控制器信息
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            if (typeof(Controller).IsAssignableFrom(actionDescriptor.ControllerTypeInfo)) return;
            // 判断是否跳过规范化处理
            if (UnifyContext.CheckSucceededNonUnify(actionDescriptor.MethodInfo, out var unifyResult)) return;

            // 处理 BadRequestObjectResult 类型规范化处理
            if (actionExecutedContext.Result is BadRequestObjectResult badRequestObjectResult)
            {
                // 解析验证消息
                var validationMetadata = ValidatorContext.GetValidationMetadata(badRequestObjectResult.Value);

                var result = unifyResult.OnValidateFailed(context, validationMetadata);
                if (result != null) actionExecutedContext.Result = result;

            }
            else
            {
                IActionResult result = default;

                // 检查是否是有效的结果（可进行规范化的结果）
                if (UnifyContext.CheckVaildResult(actionExecutedContext.Result, out var data))
                {
                    result = unifyResult.OnSucceeded(actionExecutedContext, data);
                }

                // 如果是不能规范化的结果类型，则跳过
                if (result == null) return;

                actionExecutedContext.Result = result;
            }
        }
    }
}

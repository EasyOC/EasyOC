﻿using EasyOC.Core.Authorization.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using OrchardCore.Security;
using System.Linq;
using System.Threading.Tasks;

namespace EasyOC.Core.Authorization.Handlers
{
    /// <summary>
    /// 授权策略执行程序
    /// </summary>
    public class AppAuthorizeHandler : IAuthorizationHandler
    {
        private readonly IOrchardCorePermissionService _orchardCorePermissionService;
        private readonly IPermissionGrantingService _permissionGrantingService;
        public AppAuthorizeHandler(IOrchardCorePermissionService orchardCorePermissionService, IAuthorizationService authorizationService, IPermissionGrantingService permissionGrantingService)
        {
            _orchardCorePermissionService = orchardCorePermissionService;
            _permissionGrantingService = permissionGrantingService;
        }
        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (context.HasSucceeded)
            {
                return;
            }
            // 获取 HttpContext 上下文
            var httpContext = GetCurrentHttpContext(context);
            // 获取权限特性
            var allowAnonymous = httpContext.GetMetadata<AllowAnonymousAttribute>();

            if (allowAnonymous != null)
            {
                return;
            }

            if (context?.User?.Identity?.IsAuthenticated ?? false)
            {
                foreach (var requirement in context.PendingRequirements)
                {
                    // 获取权限特性
                    var authorize = httpContext.GetMetadata<AuthorizeAttribute>();
                    if (authorize != null && authorize.Policy != string.Empty)
                    {
                        var permissions = await _orchardCorePermissionService.GetPermissionsAsync();
                        var matched = permissions.FirstOrDefault(x => x.Name == authorize.Policy);
                        if (matched != null &&
                            _permissionGrantingService.IsGranted(requirement, context.User.Claims))
                        {
                            context.Succeed(requirement);
                        }
                    }
                }

            }
            //await HandleOrchardRequirement(context, requirement);
        }

        private Task HandleOrchardRequirement(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.HasSucceeded || !(context?.User?.Identity?.IsAuthenticated ?? false))
            {
                return Task.CompletedTask;
            }
            else if (_permissionGrantingService.IsGranted(requirement, context.User.Claims))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }


        /// <summary>
        /// 获取当前 HttpContext 上下文
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DefaultHttpContext GetCurrentHttpContext(AuthorizationHandlerContext context)
        {
            DefaultHttpContext httpContext;

            // 获取 httpContext 对象
            if (context.Resource is AuthorizationFilterContext filterContext) httpContext = (DefaultHttpContext)filterContext.HttpContext;
            else if (context.Resource is DefaultHttpContext defaultHttpContext) httpContext = defaultHttpContext;
            else httpContext = null;

            return httpContext;
        }
    }
}

using EasyOC.Core.Authorization.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell.Scope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC
{
    public class EOCAuthorizationAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        private List<string> _permissions = new List<string>();
        public bool Ignore { get; set; } = false;
        public EOCAuthorizationAttribute(params OrchardCoreContentTypes[] permission)
        {
            foreach (var item in permission)
            {
                _permissions.Add(item.ToString());
            }
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            //忽略权限检查
            if (Ignore) return;

            if (!(context.HttpContext.User?.Identity?.IsAuthenticated ?? false))
            {
                context.Result = new ContentResult()
                {
                    Content = "Unauthorized.",
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }
            if (_permissions.Any())
            {
                var ServiceProvider = ShellScope.Current.ServiceProvider;
                var _orchardCorePermissionService = ServiceProvider.GetRequiredService<IOrchardCorePermissionService>();
                var _authorizationService =
                    ServiceProvider.GetRequiredService<IAuthorizationService>();
                var permissions =
                    await _orchardCorePermissionService.GetPermissionsAsync();
                foreach (var item in permissions.Where(x => _permissions.Contains(x.Name)))
                {
                    if (!await _authorizationService.AuthorizeAsync(context.HttpContext.User, item))
                    {
                        context.Result = new ContentResult()
                        {
                            Content = "Permission denied.",
                            StatusCode = StatusCodes.Status403Forbidden
                        };
                        return;
                    }
                }
            }

        }
    }
}

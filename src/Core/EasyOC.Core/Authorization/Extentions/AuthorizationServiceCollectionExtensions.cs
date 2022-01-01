﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 策略授权服务拓展类
    /// </summary>
    public static class AuthorizationServiceCollectionExtensions
    {
        /// <summary>
        /// 添加策略授权服务
        /// </summary>
        /// <typeparam name="TAuthorizationHandler">策略授权处理程序</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="configure">自定义配置</param>
        /// <param name="enableGlobalAuthorize">是否启用全局授权</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddAppAuthorization<TAuthorizationHandler>(this IServiceCollection services, Action<IServiceCollection> configure = null, bool enableGlobalAuthorize = false)
            where TAuthorizationHandler : class, IAuthorizationHandler
        {
            // 注册授权策略提供器
            //services.AddScoped<IAuthorizationPolicyProvider, AppAuthorizationPolicyProvider>();

            // 注册策略授权处理程序
            services.AddScoped<IAuthorizationHandler, TAuthorizationHandler>();

            ////启用全局授权
            //if (enableGlobalAuthorize)
            //{
            //    services.Configure<MvcOptions>(options =>
            //    {
            //        options.Filters.Add(new AuthorizeFilter());
            //    });
            //}

            configure?.Invoke(services);
            return services;
        }


    }
}

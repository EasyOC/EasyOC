using EasyOC.Scripting.Liquid;
using EasyOC.Scripting.Providers;
using EasyOC.Scripting.Providers.OrchardCore.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Liquid;
using OrchardCore.Modules;
using OrchardCore.Scripting;
using System;

namespace EasyOC.Scripting
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGlobalMethodProvider, EasyOCScriptExtendsProvider>();
            services.AddSingleton<IGlobalMethodProvider, QueryGlobalMethodProvider>();
            services.AddLiquidFilter<UsersByUserNameFilter>("users_by_userName");

        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            //routes.MapAreaControllerRoute(
            //    name: "Home",
            //    areaName: "EasyOC.Scripting",
            //    pattern: "Home/Index",
            //    defaults: new { controller = "Home", action = "Index" }
            //);
        }
    }
}

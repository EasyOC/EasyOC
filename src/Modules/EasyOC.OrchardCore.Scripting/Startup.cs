using EasyOC.OrchardCore.Scripting.Liquid;
using EasyOC.OrchardCore.Scripting.Providers;
using EasyOC.OrchardCore.Scripting.Providers.OrchardCore.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Liquid;
using OrchardCore.Modules;
using OrchardCore.Scripting;
using System;

namespace EasyOC.OrchardCore.Scripting
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
            //    areaName: "EasyOC.OrchardCore.Scripting",
            //    pattern: "Home/Index",
            //    defaults: new { controller = "Home", action = "Index" }
            //);
        }
    }
}

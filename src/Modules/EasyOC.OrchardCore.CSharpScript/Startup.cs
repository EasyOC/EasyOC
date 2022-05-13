using EasyOC.OrchardCore.CSharp.Services;
using System;
using Fluid;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace EasyOC.OrchardCore.CSharp
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICSharpScriptProvider, CSharpScriptProvider>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "EasyOC.OrchardCore.CSharp",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}

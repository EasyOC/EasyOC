// using EasyOC.OrchardCore.AuditTrail.Drivers;
using EasyOC.OrchardCore.AuditTrail.Handlers;
using EasyOC.OrchardCore.AuditTrail.Services;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.AuditTrail.Models;
using OrchardCore.AuditTrail.Services.Models;
using OrchardCore.ContentTypes.Events;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Modules;

namespace EasyOC.OrchardCore.AuditTrail
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IConfigureOptions<AuditTrailOptions>, ContentTypeAuditTrailEventConfiguration>();
            services.AddScoped<IContentDefinitionEventHandler, AuditTrailContentTypeHandler>();
            // services.AddScoped<IDisplayDriver<AuditTrailEvent>, AuditTrailContentTypeEventDisplayDriver>();

        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "EasyOC.OrchardCore.AuditTrail",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}

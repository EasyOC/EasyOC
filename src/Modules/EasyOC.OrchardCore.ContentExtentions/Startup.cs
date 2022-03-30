using EasyOC.OrchardCore.ContentExtentions.AppServices;
using EasyOC.OrchardCore.ContentExtentions.GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Apis.GraphQL;
using OrchardCore.Modules;
using System;

namespace EasyOC.OrchardCore.ContentExtentions
{
    [RequireFeatures("OrchardCore.ContentFields",
        "OrchardCore.ContentPreview", "EasyOC.OrchardCore.ContentExtentions")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddScoped<IContentTypeManagementAppService, ContentTypeManagementAppService>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        { 
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "EasyOC.OrchardCore.ContentExtentions",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}




using EasyOC.OrchardCore.ContentExtentions.AppServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddScoped<IContentManagementAppService, ContentManagementAppService>();

        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            //builder.UseDynamicWebApi((serviceProvider, options) =>
            //{
            //    options.AddAssemblyOptions(this.GetType().Assembly);
            //});

            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "EasyOC.OrchardCore.ContentExtentions",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}




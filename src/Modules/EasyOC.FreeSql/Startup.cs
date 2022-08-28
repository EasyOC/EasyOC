using EasyOC.FreeSql.Drivers;
using EasyOC.FreeSql.Queries;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Queries;

namespace EasyOC.FreeSql
{
    [RequireFeatures("EasyOC.DynamicTypeIndex")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDisplayDriver<Query>, FreeSqlQueryDisplayDriver>();
            services.AddScoped<IQuerySource, FreeSqlQuerySource>();
            services.AddScoped<INavigationProvider, AdminMenu>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "EasyOC.FreeSql",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}

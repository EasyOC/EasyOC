using EasyOC.OrchardCore.OpenApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using System;

namespace EasyOC.OrchardCore.OpenApi
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IRolesAppService, RolesAppService>();
            services.AddScoped<IUsersAppService, UsersAppService>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "EasyOC.OrchardCore.OpenApi",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index", id = "" }
            );
        }
    }
}

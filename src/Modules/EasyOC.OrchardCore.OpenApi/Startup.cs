using EasyOC.OrchardCore.OpenApi.Indexs;
using EasyOC.OrchardCore.OpenApi.Migrations;
using EasyOC.OrchardCore.OpenApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using System;
using YesSql.Indexes;

namespace EasyOC.OrchardCore.OpenApi
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IRolesAppService, RolesAppService>();
            services.AddScoped<IUsersAppService, UsersAppService>();
            services.AddScoped<IDataMigration, UserProfileMigrations>();
            services.AddSingleton<IIndexProvider, UserProfileIndexProvider>();
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

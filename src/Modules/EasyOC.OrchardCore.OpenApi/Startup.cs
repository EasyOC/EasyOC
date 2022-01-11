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
    [Feature("OrchardCore.ContentFields.Indexing.SQL")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<IIndexProvider, CustomUserSettingsIndexProvider>();
            services.AddScoped<IRolesAppService, RolesAppService>();
            services.AddScoped<IUsersAppService, UsersAppService>();
            services.AddScoped<IDataMigration, UserProfileMigrations>();
            services.AddSingleton<IIndexProvider, UserProfileIndexProvider>();
            services.AddSingleton<IIndexProvider, UserTextFieldIndexProvider>();
            
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

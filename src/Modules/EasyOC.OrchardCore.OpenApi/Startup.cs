using EasyOC.OrchardCore.OpenApi.GraphQL;
using EasyOC.OrchardCore.OpenApi.Handlers;
using EasyOC.OrchardCore.OpenApi.Indexs;
using EasyOC.OrchardCore.OpenApi.Migrations;
using EasyOC.OrchardCore.OpenApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Apis.GraphQL;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.Users.Handlers;
using System;
using YesSql.Indexes;

namespace EasyOC.OrchardCore.OpenApi
{
    [Feature("EasyOC.OrchardCore.OpenApi")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<IIndexProvider, CustomUserSettingsIndexProvider>();
            services.AddScoped<IRolesAppService, RolesAppService>();
            services.AddScoped<IUsersAppService, UsersAppService>();
            services.AddScoped<IDataMigration, UserProfileMigrations>();
            services.AddSingleton<IIndexProvider, UserProfileIndexProvider>();
            //services.AddSingleton<IIndexProvider, UserTextFieldIndexProvider>();
            services.Replace(ServiceDescriptor.Singleton<ISchemaBuilder, LuceneQueryFieldTypeProvider>());
            services.AddScoped<IUserEventHandler, UserEventHandler>();

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

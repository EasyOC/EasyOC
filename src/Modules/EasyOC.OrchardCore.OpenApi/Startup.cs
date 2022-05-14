using EasyOC.Core;
using EasyOC.OrchardCore.OpenApi.GraphQL;
using EasyOC.OrchardCore.OpenApi.GraphQL.Types;
using EasyOC.OrchardCore.OpenApi.Handlers;
using EasyOC.OrchardCore.OpenApi.Migrations;
using EasyOC.OrchardCore.OpenApi.Model;
using EasyOC.OrchardCore.OpenApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Apis;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.Users.Handlers;
using OrchardCore.Users.Services;
using System;

namespace EasyOC.OrchardCore.OpenApi
{
    [Feature("EasyOC.OrchardCore.OpenApi")]
    [RequireFeatures(Constants.EasyOCCoreModuleId)]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddContentPart<UserProfile>();

            services.AddScoped<IRolesAppService, RolesAppService>();
            services.AddScoped<IUsersAppService, UsersAppService>();
            services.AddScoped<IDataMigration, VbenMenuMigrations>();
            services.AddScoped<IUserEventHandler, EOCUserEventHandler>();
            services.AddObjectGraphType<UserPickerField, UserPickerFieldQueryObjectType>();
            services.AddSingleton<ISchemaBuilder, UserInfoQueryFieldTypeProvider>();

            services.AddScoped<IUserClaimsProvider, UserTokenLifeTimeClaimsProvider>();
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

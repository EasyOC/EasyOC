using EasyOC.Core;
using EasyOC.OrchardCore.ContentExtentions.GraphQL;
using EasyOC.OrchardCore.OpenApi.GraphQL;
using EasyOC.OrchardCore.OpenApi.GraphQL.Types;
using EasyOC.OrchardCore.OpenApi.Handlers;
using EasyOC.OrchardCore.OpenApi.Indexs;
using EasyOC.OrchardCore.OpenApi.Migrations;
using EasyOC.OrchardCore.OpenApi.Model;
using EasyOC.OrchardCore.OpenApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Apis;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.Users.Handlers;
using OrchardCore.Users.Services;
using System;
using YesSql.Indexes;

namespace EasyOC.OrchardCore.OpenApi
{
    [Feature("EasyOC.OrchardCore.OpenApi")]
    [RequireFeatures(Constants.EasyOCCoreModuleId)]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(this.GetType().Assembly);
            //services.AddSingleton<IIndexProvider, CustomUserSettingsIndexProvider>();
            services.AddSingleton<IIndexProvider, VbenMenuPartIndexProvider>();
            services.AddContentPart<VbenMenu>().AddHandler<VbenMenuHandler>();

            services.AddScoped<IRolesAppService, RolesAppService>();
            services.AddScoped<IUsersAppService, UsersAppService>();
            services.AddScoped<IDataMigration, UserProfileMigrations>();
            services.AddScoped<IDataMigration, VbenMenuMigrations>();

            //services.AddSingleton<IIndexProvider, UserProfileIndexProvider>();
            //services.AddSingleton<IIndexProvider, UserTextFieldIndexProvider>();
            services.AddScoped<IUserEventHandler, UserEventHandler>();
            //services.AddObjectGraphType<TotalQueryResults, TotalQueryResultObjectType>();
            services.AddObjectGraphType<UserPickerField, UserPickerFieldQueryObjectType>();
            //services.Replace(ServiceDescriptor.Singleton<ISchemaBuilder, LuceneQueryFieldTypeProvider>());
            //services.AddSingleton<ISchemaBuilder, EOCLuceneQueryFieldTypeProvider>();
            services.AddSingleton<ISchemaBuilder, UserInfoQueryFieldTypeProvider>();
            //services.AddContentMutationGraphQL();

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

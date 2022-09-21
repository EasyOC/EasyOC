using EasyOC.RDBMS.Drivers;
using EasyOC.RDBMS.Migrations;
using EasyOC.RDBMS.Queries.ScriptQuery;
using EasyOC.RDBMS.Scripting;
using EasyOC.RDBMS.Services;
using EasyOC.RDBMS.Workflows.Activities;
using EasyOC.RDBMS.Workflows.Drivers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Queries;
using OrchardCore.Scripting;
using OrchardCore.Security.Permissions;
using OrchardCore.Workflows.Helpers;
using System;

namespace EasyOC.RDBMS
{
    [Feature("EasyOC.RDBMS")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(GetType().Assembly);
            services.AddScoped<IContentFieldsValuePathProvider, ContentFieldsValuePathProvider>();
            // services.AddScoped<IQuerySource, FreeSqlQuerySource>();
            services.AddScoped<IDisplayDriver<Query>, ScriptQueryDisplayDriver>();
            services.AddScoped<IQuerySource, ScriptQuerySource>();
            services.AddSingleton<IGlobalMethodProvider, FreeSqlWorkflowMethodsProvider>();
            services.AddActivity<SQLTask, SQLTaskDisplayDriver>();
            services.AddScoped<IRDBMSAppService, RDBMSAppService>();
            services.AddScoped<IPermissionProvider, Permissions>();
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<IDataMigration, RDBMSMappingConfigMigration>();
            services.AddScoped<IDataMigration, DbConnectionConfigMigration>();
            services.AddTransient<IScriptQueryService,ScriptQueryService>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "EasyOC.RDBMS",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );

        }
    }
}




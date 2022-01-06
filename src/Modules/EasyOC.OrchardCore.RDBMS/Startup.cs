using EasyOC.Core.Extensions;
using EasyOC.OrchardCore.RDBMS.Migrations;
using EasyOC.OrchardCore.RDBMS.Queries.Sql;
using EasyOC.OrchardCore.RDBMS.Scripting;
using EasyOC.OrchardCore.RDBMS.Services;
using EasyOC.OrchardCore.RDBMS.Workflows.Activities;
using EasyOC.OrchardCore.RDBMS.Workflows.Drivers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Queries;
using OrchardCore.Scripting;
using OrchardCore.Workflows.Helpers;
using System;

namespace EasyOC.OrchardCore.RDBMS
{
    [RequireFeatures("EasyOC.OrchardCore.VueElementUI", "EasyOC.OrchardCore.RDBMS")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddFreeSql();
            //替换 IQuerySource
            services.Replace(ServiceDescriptor.Scoped<IQuerySource, FreeSqlQuerySource>());
            services.AddSingleton<IGlobalMethodProvider, FreeSqlWorkflowMethodsProvider>();
            //services.AddAutoMapper(GetType().Assembly);
            services.AddActivity<SQLTask, SQLTaskDisplayDriver>();
            services.AddScoped<IRDBMSAppService, RDBMSAppService>();
            //services.AddScoped<IPermissionProvider, Permissions>();
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<IContentFieldsValuePathProvider, ContentFieldsValuePathProvider>();
            services.AddScoped<IDataMigration, RDBMSMappingConfigMigration>();
            services.AddScoped<IDataMigration, DbConnectionConfigMigration>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "EasyOC.OrchardCore.RDBMS",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );

        }
    }
}




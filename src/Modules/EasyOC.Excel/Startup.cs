using EasyOC.Excel.Migrations;
using EasyOC.Excel.Models;
using EasyOC.Excel.Scripting;
//using EasyOC.Excel.Scripting;
using EasyOC.Excel.Services;
using EasyOC.Excel.Workflow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.Scripting;
using OrchardCore.Workflows.Helpers;
using System;

namespace EasyOC.Excel
{
    [Feature("EasyOC.Excel")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddScoped<IDataMigration, ExcelModuleMigrations>();
            services.AddScoped<IExcelAppService, ExcelAppService>();
            services.AddActivity<ExcelTask, ExcelTaskDisplayDriver>();
            services.AddContentPart<ImportExcelSettings>();

            // services.AddHttpClient(Consts.GraphqlClient);
            services.AddSingleton<IGlobalMethodProvider, ExcelMethodsProvider>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
            name: "Home",
            areaName: "EasyOC.Excel",
            pattern: "Home/Index",
            defaults: new
            {
                controller = "Home", action = "Index"
            }
            );
        }
    }
}

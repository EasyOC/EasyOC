using EasyOC.OrchardCore.Excel.Migrations;
using EasyOC.OrchardCore.Excel.Models;
//using EasyOC.OrchardCore.Excel.Scripting;
using EasyOC.OrchardCore.Excel.Services;
using EasyOC.OrchardCore.Excel.Workflow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.Workflows.Helpers;
using System;

namespace EasyOC.OrchardCore.Excel
{
    [Feature("EasyOC.OrchardCore.Excel")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDataMigration, ExcelModuleMigrations>();
            services.AddScoped<IExcelAppService, ExcelAppService>();
            services.AddActivity<ExcelTask, ExcelTaskDisplayDriver>();
            services.AddContentPart<ImportExcelSettings>();
            //services.AddSingleton<IGlobalMethodProvider, ExcelMethodsProvider>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "EasyOC.OrchardCore.Excel",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}




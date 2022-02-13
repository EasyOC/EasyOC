using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using TransformalizeModule.Models;
using OrchardCore.ContentManagement;
using OrchardCore.Modules;
using OrchardCore.ResourceManagement;
using OrchardCore.Data.Migration;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using TransformalizeModule.Drivers;
using TransformalizeModule.Services.Contracts;
using TransformalizeModule.Services;
using TransformalizeModule.Handlers;
using OrchardCore.ContentManagement.Handlers;
using Microsoft.AspNetCore.Http;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Settings;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;
using TransformalizeModule.Navigation;
using Transformalize.Contracts;
using Transformalize.Logging;
using OrchardCore.Workflows.Helpers;
using TransformalizeModule.Activities;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell;
using System.IO;

namespace TransformalizeModule {
   public class Startup : StartupBase {

      public Startup() {
         System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
      }

      public override void ConfigureServices(IServiceCollection services) {

         // services.AddSession();
         services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

         // transformalize services
         services.AddScoped(sp => new MemoryLogger(LogLevel.Info));
         services.AddScoped(typeof(CombinedLogger<>));
         services.AddScoped<ILinkService, LinkService>();
         services.AddScoped<ISortService, SortService>();
         services.AddScoped<IArrangementService, ArrangementService>();
         services.AddScoped<IArrangementLoadService, ArrangementLoadService>();
         services.AddScoped<IArrangementRunService, ArrangementRunService>();
         services.AddScoped<IArrangementStreamService, ArrangementStreamService>();
         services.AddScoped<IArrangementSchemaService, ArrangementSchemaService>();
         services.AddScoped<IParameterService, ParameterService>();
         services.AddScoped<ICommonService, CommonService>();
         services.AddScoped<IReportService, ReportService>();
         services.AddScoped<ITaskService, TaskService>();
         services.AddScoped<IFormService, FormService>();
         services.AddScoped<ISchemaService, SchemaService>();
         services.AddScoped<ISettingsService, SettingsService>();
         services.AddScoped<ITransformalizeParametersModifier, TransformalizeParametersModifier>();
         services.AddScoped<ILoadFormModifier, LoadFormModifier>();
         services.AddScoped<IFileService, FileService>();

         services.AddTransient<IConfigurationContainer, OrchardConfigurationContainer>();
         services.AddTransient<IContainer, OrchardContainer>();

         // orchard cms services
         services.AddScoped<IDataMigration, Migrations>();
         services.AddScoped<IPermissionProvider, Permissions>();
         services.AddTransient<IConfigureOptions<ResourceManagementOptions>, ResourceManagementOptionsConfiguration>();
         services.AddScoped<IContentHandler, TransformalizeHandler>();

         // parts
         services.AddContentPart<TransformalizeReportPart>().UseDisplayDriver<TransformalizeReportPartDisplayDriver>();
         services.AddContentPart<TransformalizeTaskPart>().UseDisplayDriver<TransformalizeTaskPartDisplayDriver>();
         services.AddContentPart<TransformalizeFormPart>().UseDisplayDriver<TransformalizeFormPartDisplayDriver>();
         services.AddContentPart<TransformalizeFilePart>().UseDisplayDriver<TransformalizeFilePartDisplayDriver>();

         // settings
         services.AddScoped<IDisplayDriver<ISite>, TransformalizeSettingsDisplayDriver>();
         services.AddScoped<INavigationProvider, TransformalizeSettingsAdminMenu>();

         // activities
         services.AddActivity<TransformalizeActivity, TransformalizeActivityDisplayDriver>();

         // file system, see https://github.com/Lombiq/Orchard-Training-Demo-Module/blob/dev/Startup.cs
         services.AddSingleton<ICustomFileStore>(serviceProvider => {
            var options = serviceProvider.GetRequiredService<IOptions<ShellOptions>>().Value;
            var settings = serviceProvider.GetRequiredService<ShellSettings>();
            var sitePath = PathExtensions.Combine(options.ShellsApplicationDataPath, options.ShellsContainerName, settings.Name);
            var path = PathExtensions.Combine(sitePath, "Transformalize", "Files");
            return new CustomFileStore(path);
         });
      }

      public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {

         RouteReporting(routes);
         RouteForms(routes);
         RouteFiles(routes);
         RouteTasks(routes);
         RouteBulkActions(routes);

         routes.MapAreaControllerRoute(
             name: "Transformalize Parameters",
             areaName: Common.ModuleName,
             pattern: "t/tp/{ContentItemId}",
             defaults: new { controller = "Arrangement", action = "TransformalizeParameters" }
         );

         routes.MapAreaControllerRoute(
             name: "Schema API",
             areaName: Common.ModuleName,
             pattern: "t/schema/{format}/{ContentItemId}",
             defaults: new { controller = "Schema", action = "Index", format = "xml" }
         );

         // builder.UseSession();
      }

      public void RouteBulkActions(IEndpointRouteBuilder routes) {

         routes.MapAreaControllerRoute(
             name: null,
             areaName: Common.ModuleName,
             pattern: "t/action/create",
             defaults: new { controller = "BulkAction", action = "Create" }
         );

         routes.MapAreaControllerRoute(
             name: null,
             areaName: Common.ModuleName,
             pattern: "t/action/review",
             defaults: new { controller = "BulkAction", action = "Review" }
         );

         routes.MapAreaControllerRoute(
             name: null,
             areaName: Common.ModuleName,
             pattern: "t/action/review/form",
             defaults: new { controller = "BulkAction", action = "Form" }
         );

         routes.MapAreaControllerRoute(
             name: null,
             areaName: Common.ModuleName,
             pattern: "t/action/run",
             defaults: new { controller = "BulkAction", action = "Run" }
         );

         routes.MapAreaControllerRoute(
             name: null,
             areaName: Common.ModuleName,
             pattern: "t/action/result",
             defaults: new { controller = "BulkAction", action = "Result" }
         );

      }

      public void RouteTasks(IEndpointRouteBuilder routes) {

         routes.MapAreaControllerRoute(
            name: "Task Review",
            areaName: Common.ModuleName,
            pattern: "t/task/{ContentItemId}",
            defaults: new { controller = "Task", action = "Review" }
         );

         routes.MapAreaControllerRoute(
             name: "Task Review Form",
             areaName: Common.ModuleName,
             pattern: "t/task/form/{ContentItemId}",
             defaults: new { controller = "Task", action = "Form" }
         );

         routes.MapAreaControllerRoute(
             name: "Task Run",
             areaName: Common.ModuleName,
             pattern: "t/task/run/{ContentItemId}",
             defaults: new { controller = "Task", action = "Run" }
         );

         routes.MapAreaControllerRoute(
             name: "Task Run API",
             areaName: Common.ModuleName,
             pattern: "t/task/run/{format}/{ContentItemId}",
             defaults: new { controller = "Task", action = "Run", format = "json" }
         );

      }

      public void RouteForms(IEndpointRouteBuilder routes) {

         routes.MapAreaControllerRoute(
            name: "Form Page",
            areaName: Common.ModuleName,
            pattern: "t/form/{ContentItemId}",
            defaults: new { controller = "Form", action = "Index" }
         );

         routes.MapAreaControllerRoute(
             name: "Form Content",
             areaName: Common.ModuleName,
             pattern: "t/form/content/{ContentItemId}",
             defaults: new { controller = "Form", action = "Form" }
         );

         routes.MapAreaControllerRoute(
            name: "Run Form API",
            areaName: Common.ModuleName,
            pattern: "t/form/{format}/{ContentItemId}",
            defaults: new { controller = "Form", action = "Run", format = "json" }
         );

      }

      public void RouteFiles(IEndpointRouteBuilder routes) {

         routes.MapAreaControllerRoute(
            name: "File View",
            areaName: Common.ModuleName,
            pattern: "t/file/{ContentItemId}",
            defaults: new { controller = "File", action = "Index" }
         );

         routes.MapAreaControllerRoute(
             name: "File Upload",
             areaName: Common.ModuleName,
             pattern: "t/file/{ContentItemId}",
             defaults: new { controller = "File", action = "Upload" }
         );

      }

      public void RouteReporting(IEndpointRouteBuilder routes) {

         routes.MapAreaControllerRoute(
             name: "Report Log",
             areaName: Common.ModuleName,
             pattern: "t/report/log/{ContentItemId}",
             defaults: new { controller = "Report", action = "Log" }
         );

         routes.MapAreaControllerRoute(
            name: "Stream CSV",
            areaName: Common.ModuleName,
            pattern: "t/report/stream/csv/{ContentItemId}",
            defaults: new { controller = "Report", action = "StreamCsv" }
         );

         routes.MapAreaControllerRoute(
            name: "Stream JSON",
            areaName: Common.ModuleName,
            pattern: "t/report/stream/json/{ContentItemId}",
            defaults: new { controller = "Report", action = "StreamJson" }
         );

         routes.MapAreaControllerRoute(
            name: "Stream Geo JSON",
            areaName: Common.ModuleName,
            pattern: "t/report/stream/geojson/{ContentItemId}",
            defaults: new { controller = "Report", action = "StreamGeoJson" }
         );

         routes.MapAreaControllerRoute(
            name: "Stream Geo JSON to Map",
            areaName: Common.ModuleName,
            pattern: "t/report/stream/map/{ContentItemId}",
            defaults: new { controller = "Map", action = "Stream" }
         );

         routes.MapAreaControllerRoute(
            name: "Stream JSON to Calendar",
            areaName: Common.ModuleName,
            pattern: "t/report/stream/calendar/{ContentItemId}",
            defaults: new { controller = "Calendar", action = "Stream" }
         );

         routes.MapAreaControllerRoute(
             name: "Report Map",
             areaName: Common.ModuleName,
             pattern: "t/report/map/{ContentItemId}",
             defaults: new { controller = "Map", action = "Index" }
         );

         routes.MapAreaControllerRoute(
             name: "Report Calendar",
             areaName: Common.ModuleName,
             pattern: "t/report/calendar/{ContentItemId}",
             defaults: new { controller = "Calendar", action = "Index" }
         );

         routes.MapAreaControllerRoute(
            name: "Run Report API",
            areaName: Common.ModuleName,
            pattern: "t/report/{format}/{ContentItemId}",
            defaults: new { controller = "Report", action = "Run", format = "json" }
         );

         routes.MapAreaControllerRoute(
            name: "Run Report",
            areaName: Common.ModuleName,
            pattern: "t/report/{ContentItemId}",
            defaults: new { controller = "Report", action = "Index" }
         );

      }

   }
}
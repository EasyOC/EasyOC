using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using StartupBase = OrchardCore.Modules.StartupBase;
using Elsa;
using Elsa.Scripting.Liquid.Extensions;
using Elsa.Server.Api.Mapping;
using Elsa.Server.Api.Services;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Elsa.OrchardCore
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {

            services
           .AddElsaCore(elsa => elsa
               .UseOrchardCorePersistence()
               .AddConsoleActivities()
               .AddHttpActivities(options =>
               {
                   options.BaseUrl = new Uri("https://localhost:44389/Elsa.OrchardCore/Elsa");
               })
               .AddQuartzTemporalActivities()
               .AddJavaScriptActivities()
               .AddWorkflowsFrom<Startup>()
           ).AddJavaScriptExpressionEvaluator()
            .AddLiquidExpressionEvaluator();

            // Elsa API endpoints.
            services.AddElsaApiEndpoints();
            services.AddVersionedApiExplorer(delegate (ApiExplorerOptions o)
            {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
            });
            services.AddApiVersioning(delegate (ApiVersioningOptions options)
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = ApiVersion.Default;
                options.AssumeDefaultVersionWhenUnspecified = true;
            });
            services.AddSingleton<ConnectionConverter>();
            services.AddSingleton<ActivityBlueprintConverter>();
            services.AddScoped<IWorkflowBlueprintMapper, WorkflowBlueprintMapper>();
            services.AddSingleton<IEndpointContentSerializerSettingsProvider, EndpointContentSerializerSettingsProvider>();
            services.AddAutoMapperProfile<AutoMapperProfile>();
            // For Dashboard.
            //services.AddRazorPages();
            //services.AddElsaSwagger();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            //builder.UseHttpActivities();
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "Elsa.OrchardCore",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}

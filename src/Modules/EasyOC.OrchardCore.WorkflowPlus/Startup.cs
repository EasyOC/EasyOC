
using EasyOC.OrchardCore.WorkflowPlus.Activities;
using EasyOC.OrchardCore.WorkflowPlus.Drivers;
using EasyOC.OrchardCore.WorkflowPlus.Servcie;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Modules;
using OrchardCore.Workflows.Helpers;
using OrchardCore.Workflows.Services;
using System;

namespace EasyOC.OrchardCore.WorkflowPlus
{
    [RequireFeatures(
        "OrchardCore.Queries.Sql",
        "OrchardCore.Workflows",
        "OrchardCore.Workflows.Http",
        "EasyOC.OrchardCore.WorkflowPlus")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(this.GetType().Assembly);
            services.Replace(ServiceDescriptor.Scoped<IWorkflowManager, JZWorkflowManager>());
            services.AddActivity<PowerShellTask, PowerShellTaskDisplayDriver>();
            services.AddActivity<EmailPlus, EmailPlusDisplayDriver>();
            services.AddActivity<WorkflowFaultEvent, WorkflowFaultEventDisplayDriver>();
            services.AddActivity<CreateUserTask, CreateUserTaskDriver>();
            services.AddScoped<IWorkflowFaultHandler, WorkflowFaultHandler>();
            //services.AddScoped<IWorkflowExecutionContextHandler, WorkflowPlusExecutionContextHandler>();
            //services.AddScoped<IQuerySource, FreeSqlQuerySource>();

        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "EasyOC.OrchardCore.WorkflowPlus",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}




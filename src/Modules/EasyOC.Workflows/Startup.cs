using EasyOC.Workflows.Activities;
using EasyOC.Workflows.Drivers;
using EasyOC.Workflows.Servcie;
using EasyOC.Workflows.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Modules;
using OrchardCore.Workflows.Helpers;
using OrchardCore.Workflows.Services;
using System;

namespace EasyOC.Workflows
{
    [RequireFeatures("EasyOC.Workflows")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(this.GetType().Assembly);
            services.Replace(ServiceDescriptor.Scoped<IWorkflowManager, EocWorkflowManager>());
            // 安全风险
            // services.AddActivity<PowerShellTask, PowerShellTaskDisplayDriver>();
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
                areaName: "EasyOC.Workflow",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}




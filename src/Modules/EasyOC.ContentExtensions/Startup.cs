using EasyOC.ContentExtensions.AppServices;
using EasyOC.ContentExtensions.Drivers;
using EasyOC.ContentExtensions.Handlers;
using EasyOC.ContentExtensions.Scripting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.Modules;
using OrchardCore.Scripting;
using System;

namespace EasyOC.ContentExtensions
{
    [RequireFeatures("OrchardCore.Contents")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddScoped<IContentTypeManagementAppService, ContentTypeManagementAppService>();
            services.Replace(ServiceDescriptor.Scoped<IContentManager, EOCDefaultContentManager>());
            services.AddScoped<IBatchImportEventHandler, BatchImportEventHandlerBase>();
            services.AddSingleton<IGlobalMethodProvider, ContentMethodsProvider>();


            services.AddScoped(typeof(IHandleExecutor<>), typeof(HandleExecutorBase<>));
            
            
            services.AddScoped<IContentTypeDefinitionDisplayDriver, ScriptHandlerDisplayDriver>();

        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "EasyOC.ContentExtensions",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}




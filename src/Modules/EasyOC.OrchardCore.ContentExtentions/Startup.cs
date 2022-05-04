using EasyOC.OrchardCore.ContentExtentions.AppServices;
using EasyOC.OrchardCore.ContentExtentions.GraphQL;
using EasyOC.OrchardCore.ContentExtentions.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.Modules;
using System;

namespace EasyOC.OrchardCore.ContentExtentions
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
            //修改类型定义事件处理声明
            //services.AddScoped<IContentDefinitionHandler, DefaultContentDefinitionHandlerBase>();
            services.Replace(ServiceDescriptor.Scoped<IContentDefinitionManager, EOCContentDefinitionManager>());

        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "EasyOC.OrchardCore.ContentExtentions",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}




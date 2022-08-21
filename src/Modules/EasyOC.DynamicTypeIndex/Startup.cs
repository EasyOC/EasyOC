using EasyOC.ContentExtentions.Handlers;
using EasyOC.DynamicTypeIndex.Handlers;
using EasyOC.DynamicTypeIndex.Index;
using EasyOC.DynamicTypeIndex.Migrations;
using EasyOC.DynamicTypeIndex.Service;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using YesSql.Indexes;

namespace EasyOC.DynamicTypeIndex
{
    [RequireFeatures("EasyOC.ContentExtentions")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            // NatashaInitializer.Preheating();
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddScoped<IDataMigration, DynamicIndexDataMigration>();
            services.AddScoped<IDynamicIndexAppService, DynamicIndexAppService>();
            services.AddContentPart<DynamicIndexConfigSetting>();
            services.AddSingleton<IIndexProvider, DynamicIndexConfigDataIndexProvider>();
            services.AddScoped<IContentHandler, DynamicIndexTableHandler>();
            services.AddScoped<IBatchImportEventHandler, DynamicIndexTableHandler>();

        }
    }
}

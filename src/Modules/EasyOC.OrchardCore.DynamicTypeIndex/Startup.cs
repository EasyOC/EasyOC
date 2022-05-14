using EasyOC.OrchardCore.ContentExtentions.Handlers;
using EasyOC.OrchardCore.DynamicTypeIndex.Handlers;
using EasyOC.OrchardCore.DynamicTypeIndex.Index;
using EasyOC.OrchardCore.DynamicTypeIndex.Migrations;
using EasyOC.OrchardCore.DynamicTypeIndex.Service;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using YesSql.Indexes;

namespace EasyOC.OrchardCore.DynamicTypeIndex
{
    [RequireFeatures("EasyOC.OrchardCore.ContentExtentions")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
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

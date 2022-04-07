using EasyOC.OrchardCore.DynamicTypeIndex.Index;
using EasyOC.OrchardCore.DynamicTypeIndex.Migrations;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
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
            //.AddHandler<VbenMenuHandler>()
            services.AddSingleton<IIndexProvider, DynamicIndexConfigDataIndexProvider>();
            services.AddScoped<IDynamicIndexTableBuilder, DynamicIndexTableBuilder>();

            NatashaInitializer.InitializeAndPreheating();

        }
    }
}

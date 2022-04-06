using System;
using EasyOC.OrchardCore.DynamicTypeIndex.Index;
using EasyOC.OrchardCore.DynamicTypeIndex.Migrations;
using Fluid;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
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
            services.AddContentPart<DynamicIndexConfigSetting>();
            //.AddHandler<VbenMenuHandler>()
            services.AddSingleton<IIndexProvider, DynamicIndexConfigDataIndexProvider>();

            services.AddScoped<IDynamicIndexTableBuilder, DynamicIndexTableBuilder>();
        }
    }
}

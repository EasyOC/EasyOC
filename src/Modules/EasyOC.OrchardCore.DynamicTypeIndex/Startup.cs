using System;
using EasyOC.OrchardCore.DynamicTypeIndex.Migrations;
using Fluid;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;

namespace EasyOC.OrchardCore.DynamicTypeIndex
{
    [RequireFeatures("EasyOC.OrchardCore.ContentExtentions")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDataMigration, DynamicIndexDataMigration>();
            services.AddScoped<IDynamicIndexTableBuilder, DynamicIndexTableBuilder>();
        } 
    }
}

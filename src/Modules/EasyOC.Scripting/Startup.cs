
using EasyOC.Scripting.Drivers;
using EasyOC.Scripting.Filters;
using EasyOC.Scripting.Graphql;
using EasyOC.Scripting.Liquid;
using EasyOC.Scripting.Providers;
using EasyOC.Scripting.Providers.OrchardCore.Queries;
using EasyOC.Scripting.Queries.ScriptQuery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Apis.GraphQL;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Liquid;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Queries;
using OrchardCore.Scripting;
using System;
using System.Linq;

namespace EasyOC.Scripting
{
    [RequireFeatures("EasyOC.GraphQL")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGlobalMethodProvider, EasyOCScriptExtendsProvider>();
            services.AddScoped<INavigationProvider, AdminMenu>();
            //Fix Array
            services.Remove(services.FirstOrDefault(x => x.ImplementationType == typeof(QueryGlobalMethodProvider)));
            services.AddSingleton<IGlobalMethodProvider, QueryGlobalMethodProviderPatch>();
            
            services.AddLiquidFilter<UsersByUserNameFilter>("users_by_userName");
            services.AddLiquidFilter<AbsoluteBaseUrlFilter>("absolute_baseUrl");

            services.AddScoped<IDisplayDriver<Query>, ScriptQueryDisplayDriver>();
            services.AddScoped<IQuerySource, ScriptQuerySource>();
            services.AddTransient<IScriptQueryService, ScriptQueryService>();
            services.AddSingleton<ISchemaBuilder, ScriptQueryFiledTypeProvider>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            //routes.MapAreaControllerRoute(
            //    name: "Home",
            //    areaName: "EasyOC.Scripting",
            //    pattern: "Home/Index",
            //    defaults: new { controller = "Home", action = "Index" }
            //);
        }
    }
}

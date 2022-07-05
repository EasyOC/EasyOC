using System;
using Fluid;
using Lucene.Net.Analysis.Cn;
using Lucene.Net.Analysis.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Lucene;
using OrchardCore.Lucene.Services;
using OrchardCore.Modules;

namespace EasyOC.Lucene
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            //注册Lucene 分析器
            services.Configure<LuceneOptions>(o =>
            {
                o.Analyzers.Add(new LuceneAnalyzer("KeywordAnalyzer", new KeywordAnalyzer()));
            });
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "EasyOC.Lucene",
                pattern: "Home/Index",
                defaults: new
                {
                    controller = "Home", action = "Index"
                }
            );
        }
    }
}

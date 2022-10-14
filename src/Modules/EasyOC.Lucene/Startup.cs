using Lucene.Net.Analysis.Core;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Search.Lucene;
using OrchardCore.Search.Lucene.Services;
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
    }
}

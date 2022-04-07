using EaysOC.GraphQL.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentManagement.GraphQL;
using OrchardCore.Modules;

namespace EaysOC.GraphQL
{
    [Feature("EasyOC.OrchardCore.OpenApi")]
    [RequireFeatures(EasyOC.Core.Constants.EasyOCCoreModuleId)]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Singleton<ISchemaBuilder, LuceneQueryFieldTypeProvider>());
            services.AddSingleton<ISchemaBuilder, EOCLuceneQueryFieldTypeProvider>();
            services.AddContentGraphQL();
            //services.AddContentMutationGraphQL(); 
        }

    }
}

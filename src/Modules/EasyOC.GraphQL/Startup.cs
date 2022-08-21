using EasyOC.GraphQL.Abstractions;
using EaysOC.GraphQL.Queries;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentManagement.GraphQL;
using OrchardCore.ContentManagement.GraphQL.Mutations;
using OrchardCore.ContentManagement.GraphQL.Mutations.Types;
using OrchardCore.Modules;

namespace EaysOC.GraphQL
{
    [Feature("EasyOC.OrchardCore.OpenApi")]
    [RequireFeatures(EasyOC.Core.Constants.EasyOCCoreModuleId, "OrchardCore.Apis.GraphQL")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISchemaBuilder, EOCLuceneQueryFieldTypeProvider>();
            //services.AddSingleton<ISchemaBuilder, LuceneQueryFieldTypeProvider>();
            //services.AddContentGraphQL();
            //services.AddContentMutationGraphQL();

            services.AddTransient<CreateContentItemInputType>();

            services.AddGraphMutationType<CreateContentItemMutation>();
            services.AddGraphMutationType<DeleteContentItemMutation>();

            services.AddTransient<DeletionStatusObjectGraphType>();
            services.AddTransient<CreateContentItemInputType>();
        }

    }
}

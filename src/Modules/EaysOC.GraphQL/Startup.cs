using EasyOC;
using EasyOC.GraphQL.Abstractions;
using EaysOC.GraphQL.Queries;
using EaysOC.GraphQL.Queries.Types;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement.GraphQL;
using OrchardCore.ContentManagement.GraphQL.Mutations;
using OrchardCore.ContentManagement.GraphQL.Mutations.Types;
using OrchardCore.Modules;
using OrchardCore.Apis;
namespace EaysOC.GraphQL
{
    [Feature("EaysOC.GraphQL")]
    [RequireFeatures(EasyOC.Core.Constants.EasyOCCoreModuleId, "OrchardCore.Apis.GraphQL")]
    public class Startup : StartupBase
    {
        public override int Order { get { return 1000; } }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISchemaBuilder, EOCLuceneQueryFieldTypeProvider>();
            //services.AddSingleton<ISchemaBuilder, LuceneQueryFieldTypeProvider>();
            //services.AddContentGraphQL();
            //services.AddContentMutationGraphQL();

            services.AddSingleton<ISchemaBuilder, ContentItemByVersionQuery>();

            // services.Remove(ServiceDescriptor.Singleton<ISchemaBuilder, ContentTypeQuery>());
            services.AddSingleton<ISchemaBuilder, EOCContentTypeQuery>();

            services.ReplaceObjectGraphType<ContentPickerField, ContentPickerFieldQueryObjectType>();


            services.AddTransient<PagedContentItemsType>();
            services.AddSingleton<ISchemaBuilder, PagedContentItemsQuery>();
            // services.AddObjectGraphType<BagPart, BagPartQueryObjectType>();



            services.AddTransient<CreateContentItemInputType>();


            services.AddGraphMutationType<CreateContentItemMutation>();
            services.AddGraphMutationType<DeleteContentItemMutation>();
            services.AddTransient<DeletionStatusObjectGraphType>();
            services.AddTransient<CreateContentItemInputType>();


        }

    }
}

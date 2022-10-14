using EasyOC.GraphQL.Abstractions;
using EasyOC.GraphQL.Handlers;
using EasyOC.GraphQL.Queries;
using EasyOC.GraphQL.Queries.Types;
using EasyOC.GraphQL.Servicies;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement.GraphQL.Mutations;
using OrchardCore.ContentManagement.GraphQL.Mutations.Types;
using OrchardCore.Modules;

namespace EasyOC.GraphQL
{
    [RequireFeatures( "OrchardCore.Apis.GraphQL","OrchardCore.ContentFields","OrchardCore.Media")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            // services.AddSingleton<ISchemaBuilder, OverrideRegisteredObjectTypesProvider>();
            services.AddSingleton<ISchemaBuilder, EOCLuceneQueryFieldTypeProvider>();
            //services.AddContentGraphQL();
            //services.AddContentMutationGraphQL();

            services.AddSingleton<ISchemaBuilder, ContentItemByVersionQuery>();

            // services.Remove(ServiceDescriptor.Singleton<ISchemaBuilder, ContentTypeQuery>());
            services.AddSingleton<ISchemaBuilder, EOCContentTypeQuery>();

            services.ReplaceObjectGraphType<ContentPickerField, ContentPickerFieldQueryObjectType>();
            services.AddTransient<PagedContentItemsType>();
            services.AddSingleton<ISchemaBuilder, PagedContentItemsQuery>();
            services.AddScoped<IPagedContentItemQueryWhereFilter, DefaultPagedContentItemQueryWhereFilter>();
            services.AddScoped<IGraphqlExecuterService, GraphqlExecuterService>();



            services.AddTransient<CreateContentItemInputType>();
            services.AddGraphMutationType<CreateContentItemMutation>();
            services.AddGraphMutationType<DeleteContentItemMutation>();
            services.AddTransient<DeletionStatusObjectGraphType>();
            services.AddTransient<CreateContentItemInputType>();
        }
    }

}

using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentManagement.GraphQL.Mutations.Types;
using OrchardCore.ContentManagement.GraphQL.Options;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.Modules;
using OrchardCore.Queries;

namespace OrchardCore.ContentManagement.GraphQL.Mutations
{
    /// <summary>
    /// Registers all Content Types as queries.
    /// </summary>
    public class CreateContentItemMutationBuilder : ISchemaBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<GraphQLContentOptions> _contentOptionsAccessor;
        private readonly IClock _clock;

        public CreateContentItemMutationBuilder(IHttpContextAccessor httpContextAccessor,
            IClock clock, IOptions<GraphQLContentOptions> contentOptionsAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _clock = clock;
            _contentOptionsAccessor = contentOptionsAccessor;
        }

        public Task BuildAsync(ISchema schema)
        {
            var serviceProvider = _httpContextAccessor.HttpContext.RequestServices;

            var contentDefinitionManager = serviceProvider.GetService<IContentDefinitionManager>();
            var contentTypeBuilders = serviceProvider.GetServices<IContentTypeBuilder>();
            var contentTypeMutationBuilders = serviceProvider.GetServices<IContentTypeMutationBuilder>();

            foreach (var typeDefinition in contentDefinitionManager.ListTypeDefinitions())
            {
                var typeType = new ContentItemType(_contentOptionsAccessor)
                {
                    Name = typeDefinition.Name
                };

                var typeInputType = new CreateContentItemInputType
                {
                    Name = typeDefinition.Name
                };

                var mutation = new CreateContentItemMutation(_httpContextAccessor, _clock)
                {
                    Name = "Create" + typeDefinition.Name,
                    ResolvedType = new ListGraphType(typeType)
                };
                mutation.Arguments = new QueryArguments {
                    new QueryArgument(new ListGraphType(typeInputType)){ Name = "parameters"}
                };

                foreach (var builder in contentTypeBuilders)
                {
                    builder.Build(mutation, typeDefinition, typeType);
                }

                foreach (var builder in contentTypeMutationBuilders)
                {
                    builder.BuildAsync(mutation, typeDefinition, typeInputType);
                }
                if (!schema.Mutation.HasField(mutation.Name))
                {
                    schema.Mutation.AddField(mutation);

                }
            }
            return Task.CompletedTask;

        }
        public Task<string> GetIdentifierAsync()
        {
            var queryManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IQueryManager>();
            return queryManager.GetIdentifierAsync();
        }
    }
}

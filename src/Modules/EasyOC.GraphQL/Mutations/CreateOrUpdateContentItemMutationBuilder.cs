//using System.Linq;
//using System.Threading.Tasks;
//using GraphQL.Types;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;
//using Microsoft.Extensions.Primitives;
//using OrchardCore.Apis.GraphQL;
//using OrchardCore.ContentManagement.GraphQL.Mutations.Types;
//using OrchardCore.ContentManagement.GraphQL.Options;
//using OrchardCore.ContentManagement.GraphQL.Queries.Types;
//using OrchardCore.ContentManagement.Metadata;
//using OrchardCore.Modules;
//using OrchardCore.Queries;

//namespace OrchardCore.ContentManagement.GraphQL.Mutations
//{
//    /// <summary>
//    /// Registers all Content Types as queries.
//    /// </summary>
//    public class CreateOrUpdateContentItemMutationBuilder : ISchemaBuilder
//    {
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private readonly IOptions<GraphQLContentOptions> _contentOptionsAccessor;
//        private readonly IClock _clock;

//        public CreateOrUpdateContentItemMutationBuilder(IHttpContextAccessor httpContextAccessor,
//            IClock clock, IOptions<GraphQLContentOptions> contentOptionsAccessor)
//        {
//            _httpContextAccessor = httpContextAccessor;
//            _clock = clock;
//            _contentOptionsAccessor = contentOptionsAccessor;
//        }

//        public Task BuildAsync(ISchema schema)
//        {
//            var serviceProvider = _httpContextAccessor.HttpContext.RequestServices;

//            var contentDefinitionManager = serviceProvider.GetService<IContentDefinitionManager>();
//            var contentTypeBuilders = serviceProvider.GetServices<IContentTypeBuilder>();
//            var contentTypeMutationBuilders = serviceProvider.GetServices<IContentTypeMutationBuilder>();

            
//                if (!schema.Mutation.HasField(mutation.Name))
//                {
//                    schema.Mutation.AddField(mutation);

//                }
//            return Task.CompletedTask;

//        }
//        public Task<string> GetIdentifierAsync()
//        {
//            var queryManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IQueryManager>();
//            return queryManager.GetIdentifierAsync();
//        }
//    }
//}

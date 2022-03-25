using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using OrchardCore.ContentManagement.Records;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.OrchardCore.OpenApi.GraphQL
{
    public class ContentItemAllVersionQuery : ISchemaBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer S;

        public ContentItemAllVersionQuery(IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<ContentItemAllVersionQuery> localizer)
        {
            _httpContextAccessor = httpContextAccessor;

            S = localizer;
        }

        public Task<string> GetIdentifierAsync() => Task.FromResult(string.Empty);

        public Task BuildAsync(ISchema schema)
        {
            var field = new FieldType
            {
                Name = "ContentItemAllVersion",
                Description = S["Content items are instances of content types, just like objects are instances of classes."],
                Type = typeof(ContentItemInterface),
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "contentItemId",
                        Description = S["Content item id"]
                    }
                ),
                Resolver = new AsyncFieldResolver<ContentItem>(ResolveAsync)
            };

            schema.Query.AddField(field);

            return Task.CompletedTask;
        }

        private async Task<ContentItem> ResolveAsync(ResolveFieldContext context)
        {
            var contentItemId = context.GetArgument<string>("contentItemId");
            var session = _httpContextAccessor.HttpContext.RequestServices.GetService<YesSql.ISession>();
            var contentItem = await session
                      .Query<ContentItem, ContentItemIndex>()
                      .Where(x =>
                          x.ContentItemId == contentItemId)
                      .FirstOrDefaultAsync();
            return contentItem;
        }
    }
}

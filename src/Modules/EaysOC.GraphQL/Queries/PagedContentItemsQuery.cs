using EasyOC.OrchardCore.ContentExtentions.GraphQL.Types;
using FreeSql.Internal.Model;
using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Indexing;
using System;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.OrchardCore.ContentExtentions.GraphQL
{
    public class PagedContentItemsQuery : ISchemaBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer S;
        private readonly IFreeSql _freesql;

        public PagedContentItemsQuery(IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<ContentItemByVersionQuery> localizer, IFreeSql freesql)
        {
            _httpContextAccessor = httpContextAccessor;

            S = localizer;
            _freesql = freesql;
        }

        public Task<string> GetIdentifierAsync() => Task.FromResult(string.Empty);

        public Task BuildAsync(ISchema schema)
        {
            var totalType = new ObjectGraphType<PagedContentItems>
            {
                Name = "PagedContentItems"
            };
            var items = totalType.Field(typetype.Type, "items",
            resolve: context =>
            {
                return context.Source?.Items ?? Array.Empty<object>();
            });
            items.ResolvedType = typetype.ResolvedType;
            totalType.Field<IntGraphType>("total",
            resolve: context =>
            {
                return context.Source?.Total ?? 0;
            });
            var field = new FieldType
            {
                Name = "items",
                Description = S["Content items are instances of content types, just like objects are instances of classes."],
                Type = typeof(ContentItemInterface),
                Arguments = new QueryArguments(
                new QueryArgument<StringGraphType>
                {
                    Name = "DynamicFilter", Description = S["dynamicFilterInfo"]
                }
                ),
                Resolver = new AsyncFieldResolver<PagedContentItems>(ResolveAsync)
            };
            field.schema.Query.AddField(field);

            return Task.CompletedTask;
        }

        private async Task<PagedContentItems> ResolveAsync(ResolveFieldContext context)
        {
            var dynamicFilterInfoStr = context.GetArgument<string>("dynamicFilterInfo");
            var dynamicFilterInfo = JsonConvert.DeserializeObject<DynamicFilterInfo>(dynamicFilterInfoStr);
            var contentManager = _httpContextAccessor?.HttpContext?.RequestServices.GetService<IContentManager>();
            var prepareQuery = _freesql.Select<DocumentIndex>()
                .WhereDynamicFilter(dynamicFilterInfo);
            var totalCount = await prepareQuery.CountAsync();
            var ids = await prepareQuery.ToListAsync(x => x.ContentItemId);
            var contentItem = await contentManager?.GetAsync(ids);
            var contentItemInterface = new PagedContentItems
            {
                Items = contentItem, TotalItems = totalCount
            };
            return contentItemInterface;
        }
    }
}

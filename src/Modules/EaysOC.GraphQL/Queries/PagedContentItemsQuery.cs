using EasyOC.OrchardCore.DynamicTypeIndex.Index;
using EasyOC.OrchardCore.DynamicTypeIndex.Service;
using EaysOC.GraphQL.Queries.Types;
using FreeSql.Internal.CommonProvider;
using FreeSql.Internal.Model;
using GraphQL.Types;
using YesSql;
using MSHttp=Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.Apis.GraphQL;
using OrchardCore.Apis.GraphQL.Resolvers;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.ContentManagement.Records;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EaysOC.GraphQL.Queries
{
    public class PagedContentItemsQuery : ISchemaBuilder
    {
        private readonly MSHttp.IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer S;
        private readonly IFreeSql _freesql;

        public PagedContentItemsQuery(MSHttp.IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<ContentItemByVersionQuery> localizer, IFreeSql freesql
        )
        {
            _httpContextAccessor = httpContextAccessor;
            S = localizer;
            _freesql = freesql;
        }

        public Task<string> GetIdentifierAsync() => Task.FromResult(string.Empty);

        public Task BuildAsync(ISchema schema)
        {
            var typeType = new PagedContentItemsType();
            var field = new FieldType()
            {
                Name = "ContentItems",
                Description = S["Content items are instances of content types, just like objects are instances of classes."],
                Resolver = new LockedAsyncFieldResolver<TotalQueryResults>(ResolveAsync),
                Type = typeType.GetType(),
                ResolvedType = typeType,
                Arguments = new QueryArguments(
                new QueryArgument<IntGraphType>
                {
                    Name = "page", Description = "The page number", DefaultValue = 1
                },
                new QueryArgument<BooleanGraphType>
                {
                    Name = "published", Description = "The published status filter", DefaultValue = true
                },
                new QueryArgument<BooleanGraphType>
                {
                    Name = "latest", Description = "The latest version status filter", DefaultValue = true
                },
                new QueryArgument<IntGraphType>()
                {
                    Name = "pageSize", Description = "The page size", DefaultValue = 10
                },
                new QueryArgument<DynamicFilterInput>()
                {
                    Name = "dynamicFilter", Description = "The dynamic filter: 参考：http://www.freesql.net/guide/select.html#%E7%89%B9%E5%88%AB%E4%BB%8B%E7%BB%8D-wheredynamicfilter", DefaultValue = ""
                },
                new QueryArgument<DynamicOrderByInput>()
                {
                    Name = "orderBy", Description = "The order by info."
                }, GetContentTypePickerArgument()
                )
            };
            schema.Query.AddField(field);
            schema.RegisterType<DynamicFilterInput>();
            return Task.CompletedTask;

            // schema.RegisterType<DynamicOrderByInput>();
        }

        private async Task<TotalQueryResults> ResolveAsync(ResolveFieldContext context)
        {
            var contentType = context.GetArgument<string>("contentType");
            if (string.IsNullOrEmpty(contentType))
            {
                return null;
            }


            var published = context.GetArgument<bool?>("published") ?? true;
            var latest = context.GetArgument<bool?>("latest") ?? true;
            var graphContext = (GraphQLContext)context.UserContext;
            var serviceProvider = graphContext.ServiceProvider;
            var dynamicIndexAppService = serviceProvider.GetRequiredService<IDynamicIndexAppService>();
            var dIndexConfig = await dynamicIndexAppService.GetDynamicIndexConfigAsync(contentType);
            if (dIndexConfig == null)
            {
                return null;
            }
            var prepareQuery = _freesql.Select<ContentItemIndex, DIndexBase>()
                .LeftJoin((a, b) => a.ContentItemId == b.ContentItemId)
                .Where((a, b) =>
                    a.ContentType == contentType &&
                    a.Published == published &&
                    a.Latest == latest
                );

            var indexType = await dynamicIndexAppService.GetDynamicIndexTypeAsync(dIndexConfig.EntityInfo);

            var joinType = (prepareQuery as Select0Provider)?._tables.LastOrDefault();
            joinType.Table = _freesql.CodeFirst.GetTableByEntity(indexType);

            var filterInfo = context.GetArgument<DynamicFilterInfo>("dynamicFilter");
            if (filterInfo is not null)
            {
                prepareQuery = prepareQuery.WhereDynamicFilter(filterInfo);
            }

            //如果 排序不为空
            if (context.HasPopulatedArgument("orderBy"))
            {
                var orderByArguments = JObject.FromObject(context.Arguments["orderBy"]);
                if (orderByArguments != null)
                {
                    var orderByField = orderByArguments["field"].Value<string>();
                    var orderByDirection = orderByArguments["direction"].Value<string>();
                    if (orderByField != null && orderByDirection != null)
                    {
                        prepareQuery = prepareQuery.OrderByPropertyName(orderByField, orderByDirection != "1");
                    }
                }
            }
            var page = context.GetArgument<int?>("page") ?? 1;
            var pageSize = context.GetArgument<int?>("pageSize") ?? 10;
            var ids = await prepareQuery
                .Count(out var totalCount)
                .Page(page, pageSize)
                .ToListAsync((x, b) => x.ContentItemId);

            if (!ids.Any())
            {
                return null;
            }
            var contentManager = serviceProvider.GetService<IContentManager>();
            var session = graphContext.ServiceProvider.GetService<ISession>();
            session.Query<ContentItem>();
            var contentItem = await contentManager?.GetAsync(ids, latest)!;
            var queryResults = new TotalQueryResults
            {
                Items = contentItem, Total = Convert.ToInt32(totalCount)
            };
            return queryResults;
        }
        private QueryArgument GetContentTypePickerArgument()
        {
            var definitionManager = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IContentDefinitionManager>();
            var pickerType = new EnumerationGraphType()
            {
                Name = "ContentTypePicker",

            };
            var contentTypes = definitionManager.LoadTypeDefinitions()
                .Select(x =>
                {
                    var stereotype = x.Settings.ToObject<ContentTypeSettings>().Stereotype;
                    return new
                    {
                        x.DisplayName, x.Name, Stereotype = stereotype
                    };
                }
                )
                .OrderBy(x => x.Stereotype);
            foreach (var typeDef in contentTypes)
            {
                pickerType.AddValue(typeDef.Name, typeDef.DisplayName, typeDef.Name, deprecationReason: typeDef.Stereotype);
            }
            return new QueryArgument<NonNullGraphType<EnumerationGraphType>>()
            {
                Name = "contentType", Description = "picker content type", ResolvedType = new NonNullGraphType(pickerType)
            };

        }
        //
        // private string ToDbColumnName(,string name)
        // {
        //     if(dIndexConfig.Fields.Any(x=>x.FillDbFiledOption()))
        //     return name.ToLower();
        // }
    }
}

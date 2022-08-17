using EasyOC;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using OrchardCore.Apis.GraphQL;
using OrchardCore.Apis.GraphQL.Queries;
using OrchardCore.Apis.GraphQL.Resolvers;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.GraphQL;
using OrchardCore.ContentManagement.GraphQL.Options;
using OrchardCore.ContentManagement.GraphQL.Queries;
using OrchardCore.ContentManagement.GraphQL.Queries.Predicates;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Environment.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using YesSql;
using Expression = OrchardCore.ContentManagement.GraphQL.Queries.Predicates.Expression;
using ISession = YesSql.ISession;

namespace EasyOC.GraphQL.Queries
{
    /// <summary>
    /// Registers all Content Types as queries.
    /// </summary>
    public class EOCContentTypeQuery : ISchemaBuilder
    {
        private static readonly List<string> ContentItemProperties;
        private readonly int _defaultNumberOfItems;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<GraphQLContentOptions> _contentOptionsAccessor;
        private readonly IOptions<GraphQLSettings> _settingsAccessor;
        private readonly IStringLocalizer S;

        static EOCContentTypeQuery()
        {
            ContentItemProperties = new List<string>();

            foreach (var property in typeof(ContentItemIndex).GetProperties())
            {
                ContentItemProperties.Add(property.Name);
            }
        }

        public EOCContentTypeQuery(IHttpContextAccessor httpContextAccessor,
            IOptions<GraphQLContentOptions> contentOptionsAccessor,
            IOptions<GraphQLSettings> settingsAccessor,
            IStringLocalizer<EOCContentTypeQuery> localizer)
        {
            _httpContextAccessor = httpContextAccessor;
            _contentOptionsAccessor = contentOptionsAccessor;
            _settingsAccessor = settingsAccessor;
            S = localizer;

            _defaultNumberOfItems = settingsAccessor.Value.DefaultNumberOfResults;
        }

        public Task<string> GetIdentifierAsync()
        {
            var contentDefinitionManager =
                _httpContextAccessor.HttpContext.RequestServices.GetService<IContentDefinitionManager>();
            return contentDefinitionManager.GetIdentifierAsync();
        }

        public Task BuildAsync(ISchema schema)
        {
            var serviceProvider = _httpContextAccessor.HttpContext.RequestServices;

            var contentDefinitionManager = serviceProvider.GetService<IContentDefinitionManager>();
            var contentTypeBuilders = serviceProvider.GetServices<IContentTypeBuilder>().ToList();

            foreach (var typeDefinition in contentDefinitionManager.ListTypeDefinitions())
            {
                var query = schema.Query.GetField(typeDefinition.Name);
                if (query == null)
                {
                    continue;
                }

                //提取已有参数
                var oldArgs = query.Arguments.Where(x =>
                    new[] { "where", "orderBy", "first", "skip" }.Contains(x.Name)).ToList();
                //覆盖参数
                query.Arguments = new(oldArgs)
                {
                    new QueryArgument<PublicationStatusGraphType>
                    {
                        Name = "status",
                        Description = "publication status of the content item",
                        ResolvedType = new PublicationStatusGraphType(),
                        DefaultValue = PublicationStatusEnum.Published
                    },
                    new QueryArgument<BooleanGraphType>
                    {
                        Name = "publishedFilter",
                        Description = "published status of the content item",
                        ResolvedType = new BooleanGraphType()
                    },
                    new QueryArgument<BooleanGraphType>
                    {
                        Name = "latestFilter",
                        Description = "latest version of the content item",
                        ResolvedType = new BooleanGraphType()
                    }
                };
                //替换原有 Resolver
                query.Resolver = new LockedAsyncFieldResolver<IEnumerable<ContentItem>>(Resolve);
            }

            foreach (var builder in contentTypeBuilders)
            {
                builder.Clear();
            }

            schema.RegisterType<PublicationStatusGraphType>();
            return Task.CompletedTask;
        }

        private async Task<IEnumerable<ContentItem>> Resolve(ResolveFieldContext context)
        {
            var graphContext = (GraphQLContext)context.UserContext;

            var versionOption = VersionOptions.Published;

            if (context.HasPopulatedArgument("status"))
            {
                versionOption = GetVersionOption(context.GetArgument<PublicationStatusEnum>("status"));
            }

            JObject where = null;
            if (context.HasArgument("where"))
            {
                where = JObject.FromObject(context.Arguments["where"]);
            }

            var session = graphContext.ServiceProvider.GetService<ISession>();

            var preQuery = session.Query<ContentItem>();

            var filters = graphContext.ServiceProvider.GetServices<IGraphQLFilter<ContentItem>>();

            foreach (var filter in filters)
            {
                preQuery = await filter.PreQueryAsync(preQuery, context);
            }

            var query = preQuery.With<ContentItemIndex>();
            bool? published = null;
            if (context.HasArgument("publishedFilter"))
            {
                published = context.GetArgument<bool>("publishedFilter");
            }

            bool? latest = null;
            if (context.HasArgument("latestFilter"))
            {
                latest = context.GetArgument<bool>("latestFilter");
            }

            if (published.HasValue || latest.HasValue)
            {
                query = query.WhereIf(published.HasValue, x => x.Published == published.Value);
                query = query.WhereIf(latest.HasValue, x => x.Latest == latest.Value);
            }
            else
            {
                query = FilterVersion(query, versionOption);
            }

            query = FilterContentType(query, context);
            query = OrderBy(query, context);

            var contentItemsQuery = FilterWhereArguments(query, where, context, session, graphContext);
            contentItemsQuery = PageQuery(contentItemsQuery, context, graphContext);

            var contentItems = await contentItemsQuery.ListAsync();

            foreach (var filter in filters)
            {
                contentItems = await filter.PostQueryAsync(contentItems, context);
            }

            return contentItems;
        }

        private IQuery<ContentItem> FilterWhereArguments(
            IQuery<ContentItem, ContentItemIndex> query,
            JObject where,
            ResolveFieldContext fieldContext,
            ISession session,
            GraphQLContext context)
        {
            if (where == null)
            {
                return query;
            }

            string defaultTableAlias = query.GetTypeAlias(typeof(ContentItemIndex));

            IPredicateQuery predicateQuery = new PredicateQuery(
                dialect: session.Store.Configuration.SqlDialect,
                shellSettings: context.ServiceProvider.GetService<ShellSettings>(),
                propertyProviders: context.ServiceProvider.GetServices<IIndexPropertyProvider>());

            // Create the default table alias
            predicateQuery.CreateAlias("", nameof(ContentItemIndex));
            predicateQuery.CreateTableAlias(nameof(ContentItemIndex), defaultTableAlias);

            // Add all provided table alias to the current predicate query
            var providers = context.ServiceProvider.GetServices<IIndexAliasProvider>();
            var indexes = new Dictionary<string, IndexAlias>(StringComparer.OrdinalIgnoreCase);
            var indexAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var aliasProvider in providers)
            {
                foreach (var alias in aliasProvider.GetAliases())
                {
                    predicateQuery.CreateAlias(alias.Alias, alias.Index);
                    indexAliases.Add(alias.Alias, alias.Alias);
                    if (!indexes.ContainsKey(alias.Index))
                    {
                        indexes.Add(alias.Index, alias);
                    }
                }
            }

            var expressions = Expression.Conjunction();
            BuildWhereExpressions(where, expressions, null, fieldContext, indexAliases);
            expressions.SearchUsedAlias(predicateQuery);

            // Add all Indexes that were used in the predicate query

            IQuery<ContentItem> contentQuery = query;
            foreach (var usedAlias in predicateQuery.GetUsedAliases())
            {
                if (indexes.ContainsKey(usedAlias))
                {
                    contentQuery = contentQuery.With(indexes[usedAlias].IndexType);
                    var tableAlias = query.GetTypeAlias(indexes[usedAlias].IndexType);
                    predicateQuery.CreateTableAlias(indexes[usedAlias].Index, tableAlias);
                }
            }

            var whereSqlClause = expressions.ToSqlString(predicateQuery);


            query = query.Where(whereSqlClause);

            // Add all parameters that were used in the predicate query
            foreach (var parameter in predicateQuery.Parameters)
            {
                query = query.WithParameter(parameter.Key, parameter.Value);
            }

            return contentQuery;
        }

        private IQuery<ContentItem> PageQuery(IQuery<ContentItem> contentItemsQuery, ResolveFieldContext context,
            GraphQLContext graphQLContext)
        {
            var first = context.GetArgument<int>("first");

            if (first == 0)
            {
                first = _defaultNumberOfItems;
            }

            contentItemsQuery = contentItemsQuery.Take(first);

            if (context.HasPopulatedArgument("skip"))
            {
                var skip = context.GetArgument<int>("skip");

                contentItemsQuery = contentItemsQuery.Skip(skip);
            }

            return contentItemsQuery;
        }

        private VersionOptions GetVersionOption(PublicationStatusEnum status)
        {
            switch (status)
            {
                case PublicationStatusEnum.Published: return VersionOptions.Published;
                case PublicationStatusEnum.PublishedOrLatest: return VersionOptions.PublishedOrLatest;
                case PublicationStatusEnum.Draft: return VersionOptions.Draft;
                case PublicationStatusEnum.Latest: return VersionOptions.Latest;
                case PublicationStatusEnum.All: return VersionOptions.AllVersions;
                default: return VersionOptions.Published;
            }
        }

        private static IQuery<ContentItem, ContentItemIndex> FilterContentType(
            IQuery<ContentItem, ContentItemIndex> query, ResolveFieldContext context)
        {
            var contentType = ((ListGraphType)context.ReturnType).ResolvedType.Name;

            return query.Where(q => q.ContentType == contentType);
        }

        private static IQuery<ContentItem, ContentItemIndex> FilterVersion(IQuery<ContentItem, ContentItemIndex> query,
            VersionOptions versionOption)
        {
            if (versionOption.IsPublishedOrLatest)
            {
                query = query.Where(q => q.Published || q.Latest);
            }

            if (versionOption.IsPublished)
            {
                query = query.Where(q => q.Published == true);
            }
            else if (versionOption.IsDraft)
            {
                query = query.Where(q => q.Latest == true && q.Published == false);
            }
            else if (versionOption.IsLatest)
            {
                query = query.Where(q => q.Latest == true);
            }

            return query;
        }

        private void BuildWhereExpressions(JToken where, Junction expressions, string tableAlias,
            ResolveFieldContext fieldContext, IDictionary<string, string> indexAliases)
        {
            if (where is JArray array)
            {
                foreach (var child in array.Children())
                {
                    if (child is JObject whereObject)
                    {
                        BuildExpressionsInternal(whereObject, expressions, tableAlias, fieldContext, indexAliases);
                    }
                }
            }
            else if (where is JObject whereObject)
            {
                BuildExpressionsInternal(whereObject, expressions, tableAlias, fieldContext, indexAliases);
            }
        }

        private void BuildExpressionsInternal(JObject where, Junction expressions, string tableAlias,
            ResolveFieldContext fieldContext, IDictionary<string, string> indexAliases)
        {
            foreach (var entry in where.Properties())
            {
                IPredicate expression = null;

                var values = entry.Name.Split('_', 2);

                // Gets the full path name without the comparison e.g. aliasPart.alias, not aliasPart.alias_contains.
                var property = values[0];

                // figure out table aliases for collapsed parts and ones with the part suffix removed by the dsl
                if (tableAlias == null || !tableAlias.EndsWith("Part", StringComparison.OrdinalIgnoreCase))
                {
                    var whereArgument = fieldContext?.FieldDefinition?.Arguments.FirstOrDefault(x => x.Name == "where");

                    if (whereArgument != null)
                    {
                        var whereInput = (WhereInputObjectGraphType)whereArgument.ResolvedType;

                        foreach (var field in whereInput.Fields.Where(x => x.GetMetadata<string>("PartName") != null))
                        {
                            var partName = field.GetMetadata<string>("PartName");
                            if ((tableAlias == null && field.GetMetadata<bool>("PartCollapsed") &&
                                 field.Name.Equals(property, StringComparison.OrdinalIgnoreCase)) ||
                                (tableAlias != null && partName.ToFieldName()
                                    .Equals(tableAlias, StringComparison.OrdinalIgnoreCase)))
                            {
                                tableAlias = indexAliases.TryGetValue(partName, out var indexTableAlias)
                                    ? indexTableAlias
                                    : tableAlias;
                                break;
                            }
                        }
                    }
                }

                if (tableAlias != null)
                {
                    property = $"{tableAlias}.{property}";
                }

                if (values.Length == 1)
                {
                    if (string.Equals(values[0], "or", StringComparison.OrdinalIgnoreCase))
                    {
                        expression = Expression.Disjunction();
                        BuildWhereExpressions(entry.Value, (Junction)expression, tableAlias, fieldContext,
                            indexAliases);
                    }
                    else if (string.Equals(values[0], "and", StringComparison.OrdinalIgnoreCase))
                    {
                        expression = Expression.Conjunction();
                        BuildWhereExpressions(entry.Value, (Junction)expression, tableAlias, fieldContext,
                            indexAliases);
                    }
                    else if (string.Equals(values[0], "not", StringComparison.OrdinalIgnoreCase))
                    {
                        expression = Expression.Conjunction();
                        BuildWhereExpressions(entry.Value, (Junction)expression, tableAlias, fieldContext,
                            indexAliases);
                        expression = Expression.Not(expression);
                    }
                    else if (entry.HasValues && entry.Value.Type == JTokenType.Object)
                    {
                        // Loop through the part's properties, passing the name of the part as the table tableAlias.
                        // This tableAlias can then be used with the table alias to index mappings to join with the correct table.
                        BuildWhereExpressions(entry.Value, expressions, values[0], fieldContext, indexAliases);
                    }
                    else
                    {
                        var propertyValue = entry.Value.ToObject<object>();
                        expression = Expression.Equal(property, propertyValue);
                    }
                }
                else
                {
                    var value = entry.Value.ToObject<object>();

                    switch (values[1])
                    {
                        case "not":
                            expression = Expression.Not(Expression.Equal(property, value));
                            break;
                        case "gt":
                            expression = Expression.GreaterThan(property, value);
                            break;
                        case "gte":
                            expression = Expression.GreaterThanOrEqual(property, value);
                            break;
                        case "lt":
                            expression = Expression.LessThan(property, value);
                            break;
                        case "lte":
                            expression = Expression.LessThanOrEqual(property, value);
                            break;
                        case "contains":
                            expression = Expression.Like(property, (string)value, MatchOptions.Contains);
                            break;
                        case "not_contains":
                            expression =
                                Expression.Not(Expression.Like(property, (string)value, MatchOptions.Contains));
                            break;
                        case "starts_with":
                            expression = Expression.Like(property, (string)value, MatchOptions.StartsWith);
                            break;
                        case "not_starts_with":
                            expression =
                                Expression.Not(Expression.Like(property, (string)value, MatchOptions.StartsWith));
                            break;
                        case "ends_with":
                            expression = Expression.Like(property, (string)value, MatchOptions.EndsWith);
                            break;
                        case "not_ends_with":
                            expression =
                                Expression.Not(Expression.Like(property, (string)value, MatchOptions.EndsWith));
                            break;
                        case "in":
                            expression = Expression.In(property, entry.Value.ToObject<object[]>());
                            break;
                        case "not_in":
                            expression = Expression.Not(Expression.In(property, entry.Value.ToObject<object[]>()));
                            break;

                        default:
                            expression = Expression.Equal(property, value);
                            break;
                    }
                }

                if (expression != null)
                {
                    expressions.Add(expression);
                }
            }
        }

        private IQuery<ContentItem, ContentItemIndex> OrderBy(IQuery<ContentItem, ContentItemIndex> query,
            ResolveFieldContext context)
        {
            if (context.HasPopulatedArgument("orderBy"))
            {
                var orderByArguments = JObject.FromObject(context.Arguments["orderBy"]);

                if (orderByArguments != null)
                {
                    var thenBy = false;

                    foreach (var property in orderByArguments.Properties())
                    {
                        var direction = (OrderByDirection)property.Value.Value<int>();

                        Expression<Func<ContentItemIndex, object>> selector = null;

                        switch (property.Name)
                        {
                            case "contentItemId":
                                selector = x => x.ContentItemId;
                                break;
                            case "contentItemVersionId":
                                selector = x => x.ContentItemVersionId;
                                break;
                            case "displayText":
                                selector = x => x.DisplayText;
                                break;
                            case "published":
                                selector = x => x.Published;
                                break;
                            case "latest":
                                selector = x => x.Latest;
                                break;
                            case "createdUtc":
                                selector = x => x.CreatedUtc;
                                break;
                            case "modifiedUtc":
                                selector = x => x.ModifiedUtc;
                                break;
                            case "publishedUtc":
                                selector = x => x.PublishedUtc;
                                break;
                            case "owner":
                                selector = x => x.Owner;
                                break;
                            case "author":
                                selector = x => x.Author;
                                break;
                        }

                        if (selector != null)
                        {
                            if (!thenBy)
                            {
                                query = direction == OrderByDirection.Ascending
                                        ? query.OrderBy(selector)
                                        : query.OrderByDescending(selector)
                                    ;
                            }
                            else
                            {
                                query = direction == OrderByDirection.Ascending
                                        ? query.ThenBy(selector)
                                        : query.ThenByDescending(selector)
                                    ;
                            }

                            thenBy = true;
                        }
                    }
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.CreatedUtc);
            }

            return query;
        }
    }

    public class VersionOptions
    {
        /// <summary>
        /// Gets the latest version.
        /// </summary>
        public static VersionOptions Latest { get { return new VersionOptions { IsLatest = true }; } }

        /// <summary>
        /// Gets the latest published version.
        /// </summary>
        public static VersionOptions Published { get { return new VersionOptions { IsPublished = true }; } }

        public static VersionOptions PublishedOrLatest
        {
            get { return new VersionOptions { IsPublishedOrLatest = true }; }
        }

        /// <summary>
        /// Gets the latest draft version.
        /// </summary>
        public static VersionOptions Draft { get { return new VersionOptions { IsDraft = true }; } }

        /// <summary>
        /// Gets the latest version and creates a new version draft based on it.
        /// </summary>
        public static VersionOptions DraftRequired
        {
            get { return new VersionOptions { IsDraft = true, IsDraftRequired = true }; }
        }

        /// <summary>
        /// Gets all versions.
        /// </summary>
        public static VersionOptions AllVersions { get { return new VersionOptions { IsAllVersions = true }; } }

        public bool IsLatest { get; private set; }
        public bool IsPublished { get; private set; }
        public bool IsPublishedOrLatest { get; private set; }
        public bool IsDraft { get; private set; }
        public bool IsDraftRequired { get; private set; }
        public bool IsAllVersions { get; private set; }
    }

    public enum PublicationStatusEnum
    {
        PublishedOrLatest,
        Published,
        Draft,
        Latest,
        All
    }

    public class PublicationStatusGraphType : EnumerationGraphType
    {
        public PublicationStatusGraphType()
        {
            Name = "Status";
            Description = "publication status";
            AddValue("PUBLISHEDORLATEST", "published or latest version content item",
                PublicationStatusEnum.PublishedOrLatest);
            AddValue("PUBLISHED", "published content item version", PublicationStatusEnum.Published);
            AddValue("DRAFT", "draft content item version", PublicationStatusEnum.Draft);
            AddValue("LATEST", "the latest version, either published or draft", PublicationStatusEnum.Latest);
            AddValue("ALL", "all historical versions", PublicationStatusEnum.All);
        }
    }
}

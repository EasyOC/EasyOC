using EasyOC.GraphQL.Queries.Types;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.Apis.GraphQL;
using OrchardCore.Apis.GraphQL.Resolvers;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.GraphQL.Queries;
using OrchardCore.Search.Lucene;
using OrchardCore.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuceneQueryResults = OrchardCore.Search.Lucene.LuceneQueryResults;

namespace EasyOC.GraphQL.Queries
{
    public class EOCLuceneQueryFieldTypeProvider : ISchemaBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EOCLuceneQueryFieldTypeProvider> _logger;

        public EOCLuceneQueryFieldTypeProvider(IHttpContextAccessor httpContextAccessor,
            ILogger<EOCLuceneQueryFieldTypeProvider> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public Task<string> GetIdentifierAsync()
        {
            var queryManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IQueryManager>();
            return queryManager.GetIdentifierAsync();
        }

        public async Task BuildAsync(ISchema schema)
        {

            var queryManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IQueryManager>();

            var queries = await queryManager.ListQueriesAsync();

            foreach (var query in queries.OfType<LuceneQuery>())
            {
                if (string.IsNullOrWhiteSpace(query.Schema))
                    continue;

                var name = query.Name;

                try
                {
                    var querySchema = JObject.Parse(query.Schema);
                    if (!querySchema.ContainsKey("type"))
                    {
                        _logger.LogError("The Query '{Name}' schema is invalid, the 'type' property was not found.",
                            name);
                        continue;
                    }

                    var type = querySchema["type"].ToString();


                    if (querySchema.ContainsKey("hasTotal") && querySchema["hasTotal"].ToString()
                            .Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        var fieldTypeName = querySchema["fieldTypeName"]?.ToString() ?? query.Name;
                        FieldType fieldType = schema.Query.GetField(fieldTypeName);
                        if (fieldType == null)
                        {
                            continue;
                        }

                        if (query.ReturnContentItems &&
                            type.StartsWith("ContentItem/", StringComparison.OrdinalIgnoreCase))
                        {
                            var contentType = type.Remove(0, 12);
                            BuildTotalContentTypeFieldType(fieldType, schema, contentType, query, fieldTypeName);
                        }
                        else
                        {
                            BuildTotalSchemaBasedFieldType(fieldType, query, querySchema, fieldTypeName);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "The Query '{Name}' has an invalid schema.", name);
                }
            }
        }

        private FieldType BuildTotalSchemaBasedFieldType(FieldType fieldType, LuceneQuery query, JToken querySchema,
            string fieldTypeName)
        {
            var properties = querySchema["properties"];
            if (properties == null)
            {
                return null;
            }

            var totalType = new ObjectGraphType<TotalQueryResults>() { Name = fieldTypeName };

            var typetype = new ObjectGraphType<JObject> { Name = fieldTypeName };
            var listType = new ListGraphType(typetype);

            totalType.Field(listType.GetType(), "items",
                resolve: context =>
                {
                    return context.Source?.Items;
                });
            var total = totalType.Field<IntGraphType>("total",
                resolve: context =>
                {
                    return context.Source?.Total;
                });


            foreach (JProperty child in properties.Children())
            {
                var name = child.Name;
                var nameLower = name.Replace('.', '_');
                var type = child.Value["type"].ToString();
                var description = child.Value["description"]?.ToString();

                if (type == "string")
                {
                    var field = typetype.Field(
                        typeof(StringGraphType),
                        nameLower,
                        description: description,
                        resolve: context =>
                        {
                            var source = context.Source;
                            return source[context.FieldDefinition.Metadata["Name"].ToString()].ToObject<string>();
                        });
                    field.Metadata.Add("Name", name);
                }
                else if (type == "integer")
                {
                    var field = typetype.Field(
                        typeof(IntGraphType),
                        nameLower,
                        description: description,
                        resolve: context =>
                        {
                            var source = context.Source;
                            return source[context.FieldDefinition.Metadata["Name"].ToString()].ToObject<int>();
                        });
                    field.Metadata.Add("Name", name);
                }
            }

            fieldType.Arguments = new QueryArguments(
                new QueryArgument<StringGraphType> { Name = "parameters" }
            );
            fieldType.Name = fieldTypeName;
            fieldType.Description = "Represents the " + query.Source + " Query : " + query.Name;
            fieldType.ResolvedType = totalType;
            fieldType.Resolver = new LockedAsyncFieldResolver<object, object>(async context =>
            {
                var queryManager = context.RequestServices.GetService<IQueryManager>();
                var iquery = await queryManager.GetQueryAsync(query.Name);

                var parameters = context.GetArgument<string>("parameters");

                var queryParameters = parameters != null
                    ? JsonConvert.DeserializeObject<Dictionary<string, object>>(parameters)
                    : new Dictionary<string, object>();

                var result = await queryManager.ExecuteQueryAsync(iquery, queryParameters) as LuceneQueryResults;
                return result;
            });
            fieldType.Type = totalType.GetType();

            return fieldType;
        }


        private FieldType BuildTotalContentTypeFieldType(FieldType fieldType, ISchema schema, string contentType,
            LuceneQuery query, string fieldTypeName)
        {
            var typetype = schema.Query.Fields.OfType<ContentItemsFieldType>()
                .FirstOrDefault(x => x.Name == contentType);
            if (typetype == null)
            {
                return null;
            }

            var totalType = new ObjectGraphType<TotalQueryResults> { Name = fieldTypeName };

            var items = totalType.Field(typetype.Type, "items",
                resolve: context =>
                {
                    return context.Source?.Items ?? Array.Empty<ContentItem>();
                });
            items.ResolvedType = typetype.ResolvedType;
            totalType.Field<IntGraphType>("total",
                resolve: context =>
                {
                    return context.Source?.Total ?? 0;
                });

            fieldType.Arguments = new QueryArguments(
                new QueryArgument<StringGraphType> { Name = "parameters" });

            fieldType.Name = fieldTypeName;
            fieldType.Description = "Represents the " + query.Source + " Query : " + query.Name;
            fieldType.ResolvedType = totalType;
            fieldType.Resolver = new LockedAsyncFieldResolver<object, object>(async context =>
            {
                var queryManager = context.RequestServices.GetService<IQueryManager>();
                var iquery = await queryManager.GetQueryAsync(query.Name);

                var parameters = context.GetArgument<string>("parameters");

                var queryParameters = parameters != null
                    ? JsonConvert.DeserializeObject<Dictionary<string, object>>(parameters)
                    : new Dictionary<string, object>();
                var result = await queryManager.ExecuteQueryAsync(iquery, queryParameters);

                return new TotalQueryResults
                {
                    Total = (result as LuceneQueryResults)?.Count, Items = result?.Items ?? Array.Empty<ContentItem>()
                };
            });
            fieldType.Type = totalType.GetType();

            return fieldType;
        }
    }
}

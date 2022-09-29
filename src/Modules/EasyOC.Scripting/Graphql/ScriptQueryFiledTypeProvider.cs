using AngleSharp.Text;
using EasyOC.Scripting.Graphql.Models;
using EasyOC.Scripting.Queries.ScriptQuery;
using EasyOC.Scripting.Queries.ScriptQuery.Models;
using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.Apis.GraphQL;
using OrchardCore.Apis.GraphQL.Resolvers;
using OrchardCore.ContentManagement.GraphQL.Queries;
using OrchardCore.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyOC.Scripting.Graphql
{
    public class ScriptQueryFiledTypeProvider : ISchemaBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        public ScriptQueryFiledTypeProvider(IHttpContextAccessor httpContextAccessor, ILogger<ScriptQueryFiledTypeProvider> logger)
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

            foreach (var query in queries.OfType<ScriptQuery>())
            {
                if (query.Schema is null)
                {
                    continue;
                }
                var querySchema = JToken.Parse(query.Schema);
                var fieldTypeName = querySchema["fieldTypeName"]?.ToString() ?? query.Name;
                var filedType = BuildScriptFieldType(schema, query, querySchema, fieldTypeName);
                if (filedType != null)
                {
                    schema.Query.AddField(filedType);
                }
            }
        }
        private static readonly string[] NormalFieldTypes =
        {
            "int", "string", "float", "boolean"
        };
        public FieldType BuildScriptFieldType(ISchema schema, ScriptQuery query, JToken querySchema, string fieldTypeName)
        {
            var typetype = new ObjectGraphType<JToken>()
            {
                Name = fieldTypeName
            };
            foreach (JProperty item in querySchema)
            {
                if (item.Name == "items")
                {
                    var newToken = new JObject();
                    if (item.Value["type"]?.Type == JTokenType.Object)
                    {
                        newToken["ofType"] = JToken.Parse(item.Value["type"].ToString());
                    }
                    else if (item.Value["type"]?.Type == JTokenType.String)
                    {
                        newToken["ofType"] = item.Value["type"].ToString();
                    }
                    else
                    {
                        newToken["ofType"] = JToken.Parse(item.Value.ToString());
                    }
                    newToken["type"] = "array";
                    item.Value = newToken;
                    var filedDescription = FieldSchemaDescription.Parse(item);

                    filedDescription.ValuePath = filedDescription.ValuePath.ToPascalCase();
                    var itemType = BuildObjectField(filedDescription, schema);
                    itemType.Resolver = new FuncFieldResolver<JToken, JToken>(x => x.Source["Items"]);
                    typetype.AddField(itemType);
                }
                else
                {
                    var filedDescription = FieldSchemaDescription.Parse(item);

                    filedDescription.ValuePath = filedDescription.ValuePath.ToPascalCase();
                    var itemType = BuildObjectField(filedDescription, schema);
                    typetype.AddField(itemType);
                }
            }

            var fieldType = new FieldType
            {
                Arguments = new QueryArguments(
                new QueryArgument<StringGraphType>
                {
                    Name = "parameters"
                }),

                Name = fieldTypeName,
                Description = "Represents the " + query.Source + " Query : " + query.Name,
                ResolvedType = typetype,
                Resolver = new LockedAsyncFieldResolver<JToken, JToken>(async context => {
                    var queryManager = context.ResolveServiceProvider().GetService<IQueryManager>();
                    var iquery = await queryManager.GetQueryAsync(query.Name);

                    var parameters = context.GetArgument<string>("parameters");

                    var queryParameters = parameters != null ?
                        JsonConvert.DeserializeObject<Dictionary<string, object>>(parameters)
                        : new Dictionary<string, object>();

                    var result = await queryManager.ExecuteQueryAsync(iquery, queryParameters);

                    var scriptQueryResult = result as ScriptQueryResult;
                    return JToken.FromObject(scriptQueryResult);
                }),
                // Type = typeof(ObjectGraphType<JToken>)
            };
            return fieldType;
        }


        public FieldType BuildObjectField(FieldSchemaDescription fieldDescription, ISchema schema)
        {
            FieldType fieldType = null;
            if (fieldDescription.TypeName is "object")
            {
                //构建包裹类 的 ResolveType
                var containerType = new ObjectGraphType<JObject>()
                {
                    Name = fieldDescription.FieldName,
                };
                foreach (var f in fieldDescription.Fields)
                {
                    if (f.TypeName is "object" or "array")
                    {
                        containerType.AddField(BuildObjectField(f, schema));
                    }
                    else
                    {
                        FieldType subFieldType = null;
                        if (NormalFieldTypes.Contains(f.TypeName))
                        {
                            subFieldType = BuildNormalField(f);
                        }
                        if (subFieldType == null)
                        {
                            subFieldType = schema.Query.Fields.OfType<ContentItemsFieldType>().FirstOrDefault(x => x.Name == f.TypeName);
                        }
                        if (subFieldType != null)
                        {
                            containerType.AddField(subFieldType);
                        }
                    }
                }
                //构建包裹类
                fieldType = new FieldType()
                {
                    Name = fieldDescription.FieldName,
                    Description = fieldDescription.Description,
                    ResolvedType = containerType,
                    Resolver = new FuncFieldResolver<JToken,JToken>(
                    context => {
                        var source = context.Source;
                        return source[context.FieldDefinition.Metadata["Name"].ToString()];
                    })
                };
                //添加 值获取方式
                fieldType.Metadata.Add("Name", fieldDescription.ValuePath);

            }
            if (fieldDescription.TypeName == "array")
            {
                if (fieldDescription.OfType == "object")
                {
                    //构建包裹类 的 ResolveType
                    var containerType = new ObjectGraphType<JObject>()
                    {
                        Name = fieldDescription.FieldName,
                    };
                    foreach (var f in fieldDescription.Fields)
                    {
                        if (f.TypeName is "object" or "array")
                        {
                            containerType.AddField(BuildObjectField(f, schema));
                        }
                        else
                        {
                            FieldType subFieldType = null;
                            if (NormalFieldTypes.Contains(f.TypeName))
                            {
                                subFieldType = BuildNormalField(f);
                            }
                            if (subFieldType == null)
                            {
                                subFieldType = schema.Query.Fields.OfType<ContentItemsFieldType>().FirstOrDefault(x => x.Name == f.TypeName);
                            }
                            if (subFieldType != null)
                            {
                                containerType.AddField(subFieldType);
                            }
                        }
                    }
                    //构建包裹类
                    fieldType = new FieldType()
                    {
                        Name = fieldDescription.FieldName,
                        Description = fieldDescription.Description,
                        ResolvedType = new ListGraphType(containerType),
                        Resolver = new FuncFieldResolver<JToken, JToken>(context => {
                            var source = context.Source;
                            return source[context.FieldDefinition.Metadata["Name"].ToString()];
                        })
                    };
                    //添加 值获取方式
                    fieldType.Metadata.Add("Name", fieldDescription.ValuePath);
                }
                else
                {
                    var containerType = new ObjectGraphType<JObject>();
                    FieldType subFieldType = null;
                    if (NormalFieldTypes.Contains(fieldDescription.OfType))
                    {
                        subFieldType = BuildNormalField(fieldDescription);
                    }
                    if (subFieldType == null)
                    {
                        subFieldType = schema.Query.Fields.OfType<ContentItemsFieldType>().FirstOrDefault(x => x.Name == fieldDescription.OfType);
                    }
                    if (subFieldType != null)
                    {
                        fieldType = new FieldType()
                        {
                            Name = fieldDescription.FieldName,
                            Description = fieldDescription.Description,
                            ResolvedType = new ListGraphType(containerType),
                            Resolver = new FuncFieldResolver<JToken, JToken>(
                            context => {
                                var source = context.Source;
                                return source[context.FieldDefinition.Metadata["Name"].ToString()];
                            })
                        };
                        //添加 值获取方式
                        fieldType.Metadata.Add("Name", fieldDescription.ValuePath);
                    }
                }
            }

            if (NormalFieldTypes.Contains(fieldDescription.TypeName))
            {
                fieldType = BuildNormalField(fieldDescription);
            }
            if (fieldType == null)
            {
                fieldType = schema.Query.Fields.OfType<ContentItemsFieldType>().FirstOrDefault(x => x.Name == fieldDescription.TypeName);
            }

            return fieldType;
        }


        private FieldType BuildNormalField(FieldSchemaDescription fieldSchemaDescription)
        {

            Type dataType = null;
            Type graphqlType = null;
            switch (fieldSchemaDescription.TypeName)
            {
                case "string":
                    dataType = typeof(string);
                    graphqlType = typeof(StringGraphType);
                    break;
                case "int" or "integer":
                    dataType = typeof(int);
                    graphqlType = typeof(IntGraphType);
                    break;
                case "float":
                    dataType = typeof(float);
                    graphqlType = typeof(FloatGraphType);
                    break;
                case "boolean":
                    dataType = typeof(bool);
                    graphqlType = typeof(BooleanGraphType);
                    break;
            }
            var field = new FieldType()
            {
                Name = fieldSchemaDescription.FieldName,
                Description = fieldSchemaDescription.Description,
                Type = graphqlType,
                Resolver = new FuncFieldResolver<JToken, object>(
                context => {
                    var source = context.Source;
                    return source[context.FieldDefinition.Metadata["Name"].ToString()].ToObject(dataType);
                })
            };
            field.Metadata.Add("Name", fieldSchemaDescription.ValuePath);

            return field;
        }

    }
}
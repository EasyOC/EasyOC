using EasyOC.Core.Application;
using EasyOC.DynamicWebApi.Attributes;
using EasyOC.ContentExtentions.AppServices.Dtos;
using EasyOC.ContentExtentions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Contents;
using OrchardCore.Contents.Models;
using OrchardCore.Lucene;
using OrchardCore.Mvc.Utilities;
using OrchardCore.Queries;
using OrchardCore.Queries.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EasyOC.ContentExtentions.AppServices
{
    [EOCAuthorization(OCPermissions.ViewContentTypes)]
    public class ContentTypeManagementAppService : AppServiceBase, IContentTypeManagementAppService
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IQueryManager _queryManager;


        public ContentTypeManagementAppService(IContentDefinitionManager contentDefinitionManager,
            IQueryManager queryManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _queryManager = queryManager;
        }

        /// <summary>
        /// 列出所有类型定义
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public PagedResult<ContentTypeListItemDto> GetAllTypes(GetAllTypeFilterInput input)
        {
            var onlyStereoTypeNone = input?.Stereotype == Stereotype.OnlyEmpty;
            var result = _contentDefinitionManager.ListTypeDefinitions().ToList()
                .WhereIf(!string.IsNullOrEmpty(input.Filter), x
                    => x.Name.Contains(input.Filter, StringComparison.OrdinalIgnoreCase)
                       || x.DisplayName.Contains(input.Filter, StringComparison.OrdinalIgnoreCase))
                .Select(x =>
                {
                    var listItem = new ContentTypeListItemDto();
                    listItem.Name = x.Name;
                    listItem.DisplayName = x.DisplayName;
                    listItem.Stereotype = x.GetSettings<ContentTypeSettings>().Stereotype;
                    return listItem;
                })
                .WhereIf(input.Stereotype.HasValue && !onlyStereoTypeNone,
                x => input.Stereotype.Value.ToDescriptionOrString()
                    .Equals(x.Stereotype, StringComparison.OrdinalIgnoreCase))
                .WhereIf(onlyStereoTypeNone, x => x.Stereotype.IsNullOrWhiteSpace())
                .ToList();

            return result.ToPagedResult(input);
        }

        public PagedResult<ContentPartDefinitionDto> GetAllParts(SimpleFilterAndPageQueryInput input)
        {
            return _contentDefinitionManager.ListPartDefinitions()
                .Select(x => x.ToDto(false))
                .WhereIf(input.Filter.IsNullOrWhiteSpace(), x
                    => x.DisplayName.Contains(input.Filter) || x.Description.Contains(input.Filter))
                .ToPagedResult(input);
        }

        public ContentPartDefinitionDto GetPartDefinition(string name, bool withSettings = false)
        {
            var part = _contentDefinitionManager.LoadPartDefinition(name);
            return part.ToDto(withSettings);
        }


        public ContentTypeDefinitionDto GetTypeDefinition(string name, bool withSettings = false)
        {
            var typeDefinition = _contentDefinitionManager.LoadTypeDefinition(name);
            if (typeDefinition == null)
            {
                throw new AppFriendlyException(HttpStatusCode.BadRequest, "ContentType not found");
            }
            return typeDefinition.ToDto(withSettings);
        }

        [EOCAuthorization(OCPermissions.EditContentTypes)]
        public async Task<IEnumerable<QueryDefDto>> ListAllQueriesAsync()
        {
            var queries = await _queryManager.ListQueriesAsync();
            var result = queries.Select(x =>
            {
                var queryDef = ObjectMapper.Map<QueryDefDto>(x);
                var schema = JObject.Parse(x.Schema);
                if (schema.ContainsKey("hasTotal") && schema["hasTotal"] != null)
                {
                    queryDef.HasTotal = schema["hasTotal"].Value<bool>();
                }
                return queryDef;
            });
            return result;
        }
        [EOCAuthorization(OCPermissions.EditContentTypes)]
        public async Task<EditViewContentDefinitionDto> GetTypeDefinitionForEdit(string name)
        {
            var typeDefinition = _contentDefinitionManager.LoadTypeDefinition(name);
            var typDto = new EditViewContentDefinitionDto
            {
                Settings = typeDefinition.GetSettings<ContentTypeSettings>(),
                FullTextOption = typeDefinition.GetSettings<FullTextAspectSettings>(),
                Name = typeDefinition.Name,
                DisplayName = typeDefinition.DisplayName,
                Fields = GetFields(typeDefinition)
            };

            return typDto;
        }

        [EOCAuthorization(OCPermissions.EditContentTypes)]
        public Task<object> CreateTypeDefinition(CreateTypeDefinitionInput input)
        {
            return null;
            // return  Task.FromResult(null);
            // var typeDefinition = _contentDefinitionManager.DeletePartDefinition(name);
        }

        /// <summary>
        /// 获取指定类型的字段清单
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public List<ContentFieldsMappingDto> GetFields(string typeName)
        {
            var typeDef = _contentDefinitionManager.GetTypeDefinition(typeName);
            return GetFields(typeDef);
        }


        [IgnoreWebApiMethod]
        public List<ContentFieldsMappingDto> GetFields(ContentTypeDefinition typeDef)
        {
            var fields = new List<ContentFieldsMappingDto>();
            fields.AddRange(new[]
            {
                new ContentFieldsMappingDto
                {
                    DisplayName = S["ID"].Value, FieldName = "ContentItemId", IsSelf = true, IsBasic = true,
                },
                new ContentFieldsMappingDto
                {
                    DisplayName = S["标题"].Value, FieldName = "DisplayText", IsSelf = true, IsBasic = true
                }
            });

            var lastCols = new[]
            {
                new ContentFieldsMappingDto
                {
                    DisplayName = S["修改时间"].Value, FieldName = "ModifiedUtc", IsSelf = true, IsBasic = true
                },
                new ContentFieldsMappingDto
                {
                    DisplayName = S["创建时间"].Value, FieldName = "CreatedUtc", IsSelf = true, IsBasic = true
                },
                new ContentFieldsMappingDto
                {
                    DisplayName = S["归属人"].Value, FieldName = "Owner", IsSelf = true, IsBasic = true
                },
                new ContentFieldsMappingDto
                {
                    DisplayName = S["作者"].Value, FieldName = "Author", IsSelf = true, IsBasic = true
                },
                new ContentFieldsMappingDto
                {
                    DisplayName = S["发布时间"].Value, FieldName = "PublishedUtc", IsSelf = true, IsBasic = true
                },
                new ContentFieldsMappingDto
                {
                    DisplayName = S["版本号"].Value, FieldName = "ContentItemVersionId", IsSelf = true, IsBasic = true
                },
                new ContentFieldsMappingDto
                {
                    DisplayName = S["内容类型"].Value, FieldName = "ContentType", IsSelf = true, IsBasic = true
                },
                new ContentFieldsMappingDto
                {
                    DisplayName = S["最新版"].Value, FieldName = "Latest", IsSelf = true, IsBasic = true
                },
                new ContentFieldsMappingDto
                {
                    DisplayName = S["已发布"].Value, FieldName = "Published", IsSelf = true, IsBasic = true
                }
            };


            foreach (var item in fields)
            {
                item.LastValueKey = item.KeyPath = item.FieldName;
            }

            foreach (var part in typeDef.Parts)
            {

                foreach (var field in part.PartDefinition.Fields)
                {
                    var lastKey = field.FieldDefinition.GetFiledValuePath();
                    var fieldModel = new ContentFieldsMappingDto
                    {
                        DisplayName = S[field.DisplayName()].Value,
                        Description = field.Description(),
                        FieldName = field.Name,
                        PartName = part.Name,
                        IsSelf = part.Name == typeDef.Name,
                        PartDisplayName = part.DisplayName(),
                        KeyPath = $"{part.Name}.{field.Name}.{lastKey}",
                        LastValueKey = lastKey,
                        FieldSettings = field.Settings,
                        FieldType = field.FieldDefinition.Name,
                        IsBasic = false
                    };
                    if (fieldModel.KeyPath.EndsWith('.'))
                    {
                        fieldModel.KeyPath = fieldModel.KeyPath.TrimEnd('.');
                    }

                    string gpFieldName = field.Name;

                    if (fieldModel.IsSelf)
                    {
                        //model.filedName
                        gpFieldName = $"{fieldModel.FieldName.ToCamelCase()}";
                    }
                    else
                    {
                        gpFieldName = fieldModel.PartName.ToCamelCase().TrimEnd("Part");
                        gpFieldName = $"{gpFieldName}.{fieldModel.FieldName.ToCamelCase()}";
                        gpFieldName = gpFieldName.Replace("Part.", "");
                    }

                    var gpValuePath = field.FieldDefinition.GetGraphqlValuePath();

                    if (gpValuePath is not null)
                    {
                        //model.filedName.contentItemIds.firstValue
                        //model.partName.filedName.contentItemIds.firstValue
                        fieldModel.GraphqlValuePath = $"{gpFieldName}.{gpValuePath}";
                    }
                    else
                    {
                        fieldModel.GraphqlValuePath = gpFieldName;
                    }


                    fields.Add(fieldModel);
                }
            }
            fields.AddRange(lastCols);
            foreach (var field in fields)
            {
                if (field.GraphqlValuePath is null)
                {
                    field.GraphqlValuePath = field.FieldName.ToCamelCase();
                }
            }
            return fields;
        }
    }
}

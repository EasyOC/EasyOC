using EasyOC.Core.Application;
using EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos;
using EasyOC.OrchardCore.ContentExtentions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Contents;
using OrchardCore.Lucene;
using OrchardCore.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices
{
    [EOCAuthorization(OCPermissions.ViewContentTypes)]
    public class ContentTypeManagementAppService : AppServiceBase, IContentTypeManagementAppService
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IQueryManager _queryManager;


        public ContentTypeManagementAppService(IContentDefinitionManager contentDefinitionManager, IQueryManager queryManager)
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
                    x => input.Stereotype.Value.ToDescriptionOrString().Equals(x.Stereotype, StringComparison.OrdinalIgnoreCase))
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
            return typeDefinition.ToDto(withSettings);
        }


        [EOCAuthorization(OCPermissions.EditContentTypes)]
        public async Task<IEnumerable<QueryDefDto>> ListLuceneQueriesAsync()
        {
            var queries = (await _queryManager.ListQueriesAsync()).OfType<LuceneQuery>();
            return ObjectMapper.Map<IEnumerable<QueryDefDto>>(queries);
        }
        /// <summary>
        /// 获取指定类型的字段清单
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public List<ContentFieldsMappingDto> GetFields(string typeName)
        {
            var typeDef = _contentDefinitionManager.GetTypeDefinition(typeName);
            var fields = new List<ContentFieldsMappingDto>();
            fields.Add(new ContentFieldsMappingDto { DisplayName = S["ID"].Value, FieldName = "ContentItemId", IsContentItemProperty = true });
            fields.Add(new ContentFieldsMappingDto { DisplayName = S["版本号"].Value, FieldName = "ContentItemVersionId", IsContentItemProperty = true });
            fields.Add(new ContentFieldsMappingDto { DisplayName = S["内容类型"].Value, FieldName = "ContentType", IsContentItemProperty = true });
            fields.Add(new ContentFieldsMappingDto { DisplayName = S["标题"].Value, FieldName = "DisplayText", IsContentItemProperty = true });
            fields.Add(new ContentFieldsMappingDto { DisplayName = S["最新版"].Value, FieldName = "Latest", IsContentItemProperty = true });
            fields.Add(new ContentFieldsMappingDto { DisplayName = S["已发布"].Value, FieldName = "Published", IsContentItemProperty = true });
            fields.Add(new ContentFieldsMappingDto { DisplayName = S["修改时间"].Value, FieldName = "ModifiedUtc", IsContentItemProperty = true });
            fields.Add(new ContentFieldsMappingDto { DisplayName = S["发布时间"].Value, FieldName = "PublishedUtc", IsContentItemProperty = true });
            fields.Add(new ContentFieldsMappingDto { DisplayName = S["创建时间"].Value, FieldName = "CreatedUtc", IsContentItemProperty = true });
            fields.Add(new ContentFieldsMappingDto { DisplayName = S["归属人"].Value, FieldName = "Owner", IsContentItemProperty = true });
            fields.Add(new ContentFieldsMappingDto { DisplayName = S["作者"].Value, FieldName = "Author", IsContentItemProperty = true });
            foreach (var item in fields)
            {
                item.LastValueKey = item.KeyPath = item.FieldName;
            }
            foreach (var part in typeDef.Parts)
            {
                foreach (var field in part.PartDefinition.Fields)
                {
                    var lastKey = GetFiledValuePath(field.FieldDefinition.Name);
                    fields.Add(new ContentFieldsMappingDto
                    {
                        DisplayName = S[field.DisplayName()].Value,
                        Description = field.Description(),
                        FieldName = field.Name,
                        PartName = part.Name,
                        PartDisplayName = part.DisplayName(),
                        KeyPath = $"{part.Name}.{field.Name}.{lastKey}",
                        LastValueKey = lastKey,
                        FieldSettings = field.Settings,
                        FieldType = field.FieldDefinition.Name
                    });
                }

            }


            return fields;
        }

        public static string GetFiledValuePath(string fieldName)
        {
            string valuePath;
            switch (fieldName)
            {
                case "TextField":
                    valuePath = "Text";
                    break;
                case "BooleanField":
                case "DateField":
                case "TimeField":
                case "DateTimefield":
                case "NumericField":
                    valuePath = "Value";
                    break;
                case "ContentPickerField":
                    valuePath = "ContentItemIds";
                    break;
                case "UserPickerField":
                    valuePath = "UserIds";
                    break;
                default:
                    return null;
            }
            return valuePath;
        }



    }
}




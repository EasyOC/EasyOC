using EasyOC.Core.Application;
using EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos;
using EasyOC.OrchardCore.ContentExtentions.Models;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Lucene;
using OrchardCore.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices
{
    [EOCAuthorization(OCPermissions.ViewContentTypes)]
    public class ContentManagementAppService : AppServcieBase, IContentManagementAppService
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IQueryManager _queryManager;
        public ContentManagementAppService(IContentDefinitionManager contentDefinitionManager, IQueryManager queryManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _queryManager = queryManager;
        }

        /// <summary>
        /// 列出所有类型定义
        /// </summary>
        /// <returns></returns>
        public PagedResult<ContentTypeListItemDto> GetAllTypes(GetAllTypeFilterInput input)
        {

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
                .WhereIf(!string.IsNullOrEmpty(input.Stereotype), x
                    => input.Stereotype.Equals(x.Stereotype, StringComparison.OrdinalIgnoreCase));

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
        /// <summary>
        /// 获取指定类型的字段清单
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public List<ContentFieldsMappingDto> GetFields(string typeName) => _contentDefinitionManager.GetAllFields(typeName).ToList();

        [EOCAuthorization(OCPermissions.EditContentTypes)]
        public async Task<IEnumerable<QueryDefDto>> ListLuceneQueriesAsync()
        { 
            var queries = (await _queryManager.ListQueriesAsync()).OfType<LuceneQuery>();
            return ObjectMapper.Map<IEnumerable<QueryDefDto>>(queries);
        }

    }
}




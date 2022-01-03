using EasyOC.Core.Application;
using EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices
{
    [EOCAuthorization(OCPermissions.ViewContentTypes)]
    public class ContentManagementAppService : AppServcieBase, IContentManagementAppService
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        public ContentManagementAppService(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }

        /// <summary>
        /// 列出所有类型定义
        /// </summary>
        /// <returns></returns>
        [EOCAuthorization("ViewContentTypes")]
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
            //if (!await AuthorizationService.AuthorizeAsync(User, Permissions.ViewContentTypes))
            //{
            //    throw new AppFriendlyException("Unauthorized", StatusCodes.Status401Unauthorized);
            //}
            return _contentDefinitionManager.ListPartDefinitions()
                .Select(x => x.ToDto(false))
                .WhereIf(input.Filter.IsNullOrWhiteSpace(), x
                => x.DisplayName.Contains(input.Filter) || x.Description.Contains(input.Filter))
                .ToPagedResult(input);
        }
        public ContentPartDefinitionDto GetPartDefinition(string name, bool withSettings = false)
        {
            //if (!await AuthorizationService.AuthorizeAsync(User, Permissions.ViewContentTypes))
            //{
            //    throw new AppFriendlyException("Unauthorized", StatusCodes.Status401Unauthorized);
            //}
            var part = _contentDefinitionManager.LoadPartDefinition(name);
            return part.ToDto(true, withSettings);
        }


        public ContentTypeDefinitionDto GetTypeDefinition(string name, bool withSettings = false)
        {
            var typeDefinition = _contentDefinitionManager.LoadTypeDefinition(name);
            return typeDefinition.ToDto(withSettings);
        }




    }
}




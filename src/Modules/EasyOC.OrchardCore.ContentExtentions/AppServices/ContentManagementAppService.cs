using EasyOC.Core.Application;
using EasyOC.Core.Authorization.Permissions;
using EasyOC.Dto;
using EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using System;
using OC = OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices
{
    [EOCAuthorization(OrchardCoreContentTypes.ViewContentTypes)]
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
        [EOCAuthorization(Ignore = true)]
        public PagedResult<ContentTypeListItemDto> GetAllTypes(GetAllTypeFilterInput input)
        {
            if (!AuthorizationService.AuthorizeAsync(User, new OC.Permission("ViewContentTypes")).Result)
            {
                throw new AppFriendlyException("Unauthorized", StatusCodes.Status401Unauthorized);
            }
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




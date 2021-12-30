using EasyOC.Core.Application;
using EasyOC.DynamicWebApi.Attributes;
using EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos;
using EasyOC.OrchardCore.ContentExtentions.Models;
using Microsoft.AspNetCore.Authorization;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentManagement.Metadata.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Permissions = OrchardCore.ContentTypes.Permissions;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices
{
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
        public async Task<IEnumerable<ContentTypeListItemDto>> GetAllTypesAsync()
        {
            if (!await AuthorizationService.AuthorizeAsync(User, Permissions.ViewContentTypes))
            {
                throw new UnauthorizedAccessException();
            }
            var result = _contentDefinitionManager.ListTypeDefinitions().ToList()
               .Select(x =>
               {
                   var listItem = ObjectMapper.Map<ContentTypeListItemDto>(x);
                   listItem.Stereotype = x.GetSettings<ContentTypeSettings>().Stereotype;
                   return listItem;
               });
            return result;
        }

        public IEnumerable<string> GetAllParts()
        {

            return _contentDefinitionManager.ListPartDefinitions()
                .Select(x => x.Name);

        }

        public ContentPartApiModel GetPartDefinition(string name, bool incloudeSettings = false)
        {
            var part = _contentDefinitionManager.LoadPartDefinition(name);

            return new ContentPartApiModel
            {
                Name = part.Name,
                Settings = part.Settings,
                Fields = GetPartFields(part, incloudeSettings)
            };

        }

        [NonDynamicMethod]
        public IEnumerable<ContentFiledsApiModel> GetPartFields(ContentPartDefinition partDefinition, bool incloudeSettings = false)
        {
            return partDefinition.Fields.Select(f => new ContentFiledsApiModel
            {
                Name = f.Name,
                FieldTypeName = f.FieldDefinition.Name,
                DisplayName = f.DisplayName(),
                Settings = incloudeSettings ? f.Settings : null
            });
        }
        public ContentTypeApiModel GetTypeDefinition(string name, bool incloudeSettings = false)
        {
            var typeDefinition = _contentDefinitionManager.LoadTypeDefinition(name);
            var contentTypeDef = new ContentTypeApiModel
            {
                Name = typeDefinition.Name,
                DisplayName = typeDefinition.DisplayName,
                Settings = incloudeSettings ? typeDefinition.Settings : null,
                Parts = typeDefinition.Parts.Select(
                     p =>
                         {
                             return new ContentPartApiModel
                             {
                                 Name = p.Name,
                                 Settings = incloudeSettings ? p.Settings : null,
                                 Fields = GetPartFields(p.PartDefinition, incloudeSettings)
                             };
                         })

            };
            return contentTypeDef;
        }


    }
}




using EasyOC.OrchardCore.ContentExtentions.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using EasyOC.DynamicWebApi;
using EasyOC.DynamicWebApi.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices
{
    [DynamicWebApi]
    public class ContentManagementAppService : IDynamicWebApi, IContentManagementAppService
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
        public Dictionary<string, string> GetAllTypes()
        {
            var result = _contentDefinitionManager.ListTypeDefinitions()
               .ToDictionary(x => x.Name, v => v.DisplayName);
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




using EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos;
using EasyOC.OrchardCore.ContentExtentions.Models;
using System.Collections.Generic;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices
{
    public interface IContentManagementAppService
    {
        PagedResult<ContentPartDefinitionDto> GetAllParts(SimpleFilterAndPageQueryInput input);
        PagedResult<ContentTypeListItemDto> GetAllTypes(GetAllTypeFilterInput input);
        ContentPartDefinitionDto GetPartDefinition(string name, bool withSettings = false);
        ContentTypeDefinitionDto GetTypeDefinition(string name, bool withSettings = false);

        IEnumerable<ContentFieldsMappingDto> GetAllFieldsByType(string typeName);

    }
}

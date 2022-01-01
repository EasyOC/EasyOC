﻿using EasyOC.Dto;
using EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices
{
    public interface IContentManagementAppService
    {
        PagedResult<ContentPartDefinitionDto> GetAllParts(SimpleFilterAndPageQueryInput input);
        PagedResult<ContentTypeListItemDto> GetAllTypes(GetAllTypeFilterInput input);
        ContentPartDefinitionDto GetPartDefinition(string name, bool withSettings = false);
        ContentTypeDefinitionDto GetTypeDefinition(string name, bool withSettings = false);
    }
}

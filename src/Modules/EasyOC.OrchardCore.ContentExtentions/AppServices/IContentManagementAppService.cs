using EasyOC.Dto;
using EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices
{
    public interface IContentManagementAppService
    {
        Task<PagedResult<ContentPartDefinitionDto>> GetAllPartsAsync(SimpleFilterAndPageQueryInput input);
        Task<PagedResult<ContentTypeListItemDto>> GetAllTypesAsync(GetAllTypeFilterInput input);
        Task<ContentPartDefinitionDto> GetPartDefinitionAsync(string name, bool withSettings = false);
        Task<ContentTypeDefinitionDto> GetTypeDefinitionAsync(string name, bool withSettings = false);
    }
}
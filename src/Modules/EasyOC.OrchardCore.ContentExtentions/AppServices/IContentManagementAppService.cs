using EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos;
using EasyOC.OrchardCore.ContentExtentions.Models;
using OrchardCore.ContentManagement.Metadata.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices
{
    public interface IContentManagementAppService
    {
        IEnumerable<string> GetAllParts();
        Task<IEnumerable<ContentTypeDefinitionDto>> GetAllTypesAsync();
        ContentPartApiModel GetPartDefinition(string name, bool incloudeSettings = false);
        IEnumerable<ContentFiledsApiModel> GetPartFields(ContentPartDefinition partDefinition, bool incloudeSettings = false);
        ContentTypeApiModel GetTypeDefinition(string name, bool incloudeSettings = false);
    }
}



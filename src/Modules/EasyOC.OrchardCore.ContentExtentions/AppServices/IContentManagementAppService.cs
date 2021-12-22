using EasyOC.OrchardCore.ContentExtentions.Models;
using OrchardCore.ContentManagement.Metadata.Models;
using System.Collections.Generic;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices
{
    public interface IContentManagementAppService
    {
        IEnumerable<string> GetAllParts();
        Dictionary<string, string> GetAllTypes();
        ContentPartApiModel GetPartDefinition(string name, bool incloudeSettings = false);
        IEnumerable<ContentFiledsApiModel> GetPartFields(ContentPartDefinition partDefinition, bool incloudeSettings = false);
        ContentTypeApiModel GetTypeDefinition(string name, bool incloudeSettings = false);
    }
}



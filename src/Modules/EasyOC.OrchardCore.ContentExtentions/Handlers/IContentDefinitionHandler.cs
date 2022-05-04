using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentManagement.Metadata.Records;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.ContentExtentions.Handlers
{
    public interface IContentDefinitionHandler
    {
        Task AfterDeleteTypeDefinition(ContentTypeDefinitionRecord record);
        Task AfterStorePartDefinition(ContentPartDefinition contentPartDefinition);
        Task AfterStoreTypeDefinition(ContentTypeDefinition contentTypeDefinition);
        Task<bool> BeforeDeleteTypeDefinition(ContentTypeDefinitionRecord record);
        Task<bool> BeforeStorePartDefinition(ContentPartDefinition contentPartDefinition);
        Task<bool> BeforeStoreTypeDefinition(ContentTypeDefinition contentTypeDefinition);
        Task<bool> BeforeDeletePartDefinition(ContentPartDefinitionRecord record, IEnumerable<ContentTypeDefinition> typesWithPart);
        Task AfterDeletePartDefinition(ContentPartDefinitionRecord record, IEnumerable<ContentTypeDefinition> typesWithPart);
    }
}

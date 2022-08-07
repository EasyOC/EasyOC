using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentManagement.Metadata.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.ContentExtentions.Handlers 
{
    public class DefaultContentDefinitionHandlerBase : IContentDefinitionHandler
    {
        public virtual Task<bool> BeforeStoreTypeDefinition(ContentTypeDefinition contentTypeDefinition)
        {
            return Task.FromResult(true);
        }

        public virtual Task AfterStoreTypeDefinition(ContentTypeDefinition contentTypeDefinition)
        {
            return Task.CompletedTask;
        }

        public virtual Task<bool> BeforeStorePartDefinition(ContentPartDefinition contentPartDefinition)
        {
            return Task.FromResult(true);
        }

        public virtual Task AfterStorePartDefinition(ContentPartDefinition contentPartDefinition)
        {
            return Task.CompletedTask;
        }

        public virtual Task<bool> BeforeDeleteTypeDefinition(ContentTypeDefinitionRecord record)
        {
            return Task.FromResult(true);
        }

        public virtual Task AfterDeleteTypeDefinition(ContentTypeDefinitionRecord record)
        {
            return Task.CompletedTask;
        }

        public virtual Task<bool> BeforeDeletePartDefinition(ContentPartDefinitionRecord record, IEnumerable<ContentTypeDefinition> typesWithPart)
        {
            return Task.FromResult(true);
        }

        public virtual Task AfterDeletePartDefinition(ContentPartDefinitionRecord record, IEnumerable<ContentTypeDefinition> typesWithPart)
        {
            return Task.CompletedTask;
        }
    }
}

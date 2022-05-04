using OrchardCore.ContentTypes.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.ContentExtentions.Handlers
{
    public class ContentDefinitionEventHandlerBase : IContentDefinitionEventHandler
    {
        public virtual void ContentFieldAttached(ContentFieldAttachedContext context)
        {
            
        }

        public virtual void ContentFieldDetached(ContentFieldDetachedContext context)
        {
            
        }

        public virtual void ContentPartAttached(ContentPartAttachedContext context)
        {
            
        }

        public virtual void ContentPartCreated(ContentPartCreatedContext context)
        {
            
        }

        public virtual void ContentPartDetached(ContentPartDetachedContext context)
        {
            
        }

        public virtual void ContentPartImported(ContentPartImportedContext context)
        {
            
        }

        public virtual void ContentPartImporting(ContentPartImportingContext context)
        {
            
        }

        public virtual void ContentPartRemoved(ContentPartRemovedContext context)
        {
            
        }

        public virtual void ContentTypeCreated(ContentTypeCreatedContext context)
        {
            
        }

        public virtual void ContentTypeImported(ContentTypeImportedContext context)
        {
            
        }

        public virtual void ContentTypeImporting(ContentTypeImportingContext context)
        {
            
        }

        public virtual void ContentTypeRemoved(ContentTypeRemovedContext context)
        {
            
        }
    }
}

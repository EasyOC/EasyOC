using OrchardCore.ContentManagement.Handlers;
using System.Threading.Tasks;

namespace EasyOC.ContentExtensions.Handlers
{
    public class ScriptHandler:ContentHandlerBase
    {
        
        
        public override Task CreatingAsync(CreateContentContext context)
        {
            return base.CreatingAsync(context);
        }
        public override Task UpdatingAsync(UpdateContentContext context)
        {
            return base.UpdatingAsync(context);
        }

        public override Task RemovingAsync(RemoveContentContext context)
        {
            return base.RemovingAsync(context);
        }
        public override Task PublishingAsync(PublishContentContext context)
        {
            return base.PublishingAsync(context);
        }
        public override Task DraftSavingAsync(SaveDraftContentContext context)
        {
            return base.DraftSavingAsync(context);
        }
    }
}
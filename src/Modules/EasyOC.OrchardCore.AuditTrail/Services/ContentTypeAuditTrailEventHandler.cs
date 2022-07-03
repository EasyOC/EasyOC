using OrchardCore.ContentManagement;
using EasyOC.OrchardCore.AuditTrail.Models;
using OrchardCore.AuditTrail.Services;
using OrchardCore.AuditTrail.Services.Models;
using OrchardCore.Contents.AuditTrail.Models;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.AuditTrail.Services
{
    public class ContentTypeAuditTrailEventHandler: AuditTrailEventHandlerBase
    {
        public override Task CreateAsync(AuditTrailCreateContext context)
        {
            if (context.Category != "ContentType")
            {
                return Task.CompletedTask;
            }

            if (context is AuditTrailCreateContext<AuditTrailContentTypeEvent> contentEvent)
            {
                var auditTrailPart = contentEvent.AuditTrailEventItem.ContentItem.As<AuditTrailPart>();
                if (auditTrailPart == null)
                {
                    return Task.CompletedTask;
                }

                contentEvent.AuditTrailEventItem.Comment = auditTrailPart.Comment;
            }

            return Task.CompletedTask;
        }
    }
}

using EasyOC.OrchardCore.AuditTrail.Models;
using EasyOC.OrchardCore.AuditTrail.ViewModel;
using OrchardCore.AuditTrail.Drivers;
using OrchardCore.AuditTrail.Models;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.AuditTrail.Drivers
{
    public class AuditTrailContentTypeEventDisplayDriver :
        AuditTrailEventSectionDisplayDriver<AuditTrailContentTypeEvent>
    {
        public override Task<IDisplayResult> DisplayAsync(AuditTrailEvent auditTrailEvent,
            AuditTrailContentTypeEvent typeEvent, BuildDisplayContext context)
        {
            return Task.FromResult<IDisplayResult>(
                Initialize<AuditTrailContentTypeEventDetailViewModel>("AuditTrailContentTypeEventDetail_DetailAdmin",
                    m => BuildViewModel(m, auditTrailEvent, typeEvent))
                    .Location("DetailAdmin", "Content:10")
            );
        }

        private static void BuildViewModel(AuditTrailContentTypeEventDetailViewModel m,
            AuditTrailEvent auditTrailEvent,
            AuditTrailContentTypeEvent typeEvent)
        {
            m.AuditTrailEvent = auditTrailEvent;
            m.ContentTypeEvent = typeEvent;
        }
    }
}

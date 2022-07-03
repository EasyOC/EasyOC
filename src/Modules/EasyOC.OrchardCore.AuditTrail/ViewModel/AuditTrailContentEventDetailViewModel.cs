using OrchardCore.ContentManagement;
using OrchardCore.Contents.AuditTrail.ViewModels;

namespace EasyOC.OrchardCore.AuditTrail.ViewModel
{
    public class AuditTrailContentTypeEventDetailViewModel : AuditTrailContentTypeEventViewModel
    {
        public string Previous { get; set; }
        public string Current { get; set; }

        public ContentItem PreviousContentItem { get; set; }
    }
}

using EasyOC.OrchardCore.AuditTrail.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.AuditTrail.Models;
using OrchardCore.AuditTrail.Services.Models;
using OrchardCore.ContentManagement;

namespace EasyOC.OrchardCore.AuditTrail.ViewModel
{
    public class AuditTrailContentTypeEventViewModel
    {
        public string Name { get; set; }

        public ContentItem ContentItem { get; set; }
        [BindNever]
        public AuditTrailEvent AuditTrailEvent { get; set; }


        [BindNever]
        public AuditTrailEventDescriptor Descriptor { get; set; }

        [BindNever]
        public AuditTrailContentTypeEvent ContentTypeEvent { get; set; }
    }
}

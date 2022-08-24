using EasyOC.AuditTrail.Models;
using EasyOC.AuditTrail.Services;
using EasyOC.ContentExtensions.Handlers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using OrchardCore.AuditTrail.Services;
using OrchardCore.AuditTrail.Services.Models;
using OrchardCore.ContentTypes.Events;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EasyOC.AuditTrail.Handlers
{
    public class AuditTrailContentTypeHandler : ContentDefinitionEventHandlerBase
    {
        private readonly IAuditTrailManager _auditTrailManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditTrailContentTypeHandler(
            IAuditTrailManager auditTrailManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _auditTrailManager = auditTrailManager;
            _httpContextAccessor = httpContextAccessor;
        }


        private async Task RecordAuditTrailEventAsync(string eventType,
            AuditTrailContentTypeEvent context)
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            await _auditTrailManager.RecordEventAsync(
            new AuditTrailContext<AuditTrailContentTypeEvent>
            (
            name: eventType.ToString(),
            category: ContentTypeAuditTrailEventConfiguration.ContentType,
            correlationId: context.ToString(),
            userId: user.FindFirstValue(ClaimTypes.NameIdentifier),
            userName: user?.Identity?.Name,
            auditTrailEventItem: context
            ));
        }

        #region ContentTypes
        public override async void ContentTypeCreated(ContentTypeCreatedContext context)
        {
            await RecordAuditTrailEventAsync(ContentTypeAuditTrailEventConfiguration.ContentTypeCreated,
            new AuditTrailContentTypeEvent
            {
                TypeName = context.ContentTypeDefinition.Name, EventContext = JObject.FromObject(context)
            });
        }
        public override async void ContentTypeRemoved(ContentTypeRemovedContext context)
        {
            await RecordAuditTrailEventAsync(ContentTypeAuditTrailEventConfiguration.ContentTypeRemoved,
            new AuditTrailContentTypeEvent
            {
                TypeName = context.ContentTypeDefinition.Name, EventContext = JObject.FromObject(context)
            });
        }

        public override async void ContentTypeImported(ContentTypeImportedContext context)
        {
            await RecordAuditTrailEventAsync(ContentTypeAuditTrailEventConfiguration.ContentTypeImported,
            new AuditTrailContentTypeEvent
            {
                TypeName = context.ContentTypeDefinition.Name, EventContext = JObject.FromObject(context)
            });
        }
        #endregion

        public override async void ContentPartCreated(ContentPartCreatedContext context)
        {
            await RecordAuditTrailEventAsync(ContentTypeAuditTrailEventConfiguration.ContentPartCreated,
            new AuditTrailContentTypeEvent
            {
                PartName = context.ContentPartDefinition.Name, EventContext = JObject.FromObject(context)
            });
        }
        public override async void ContentPartRemoved(ContentPartRemovedContext context)
        {
            await RecordAuditTrailEventAsync(ContentTypeAuditTrailEventConfiguration.ContentPartRemoved,
            new AuditTrailContentTypeEvent
            {
                PartName = context.ContentPartDefinition.Name, EventContext = JObject.FromObject(context)
            });
        }
        public override async void ContentPartAttached(ContentPartAttachedContext context)
        {
            await RecordAuditTrailEventAsync(ContentTypeAuditTrailEventConfiguration.ContentPartAttached,
            new AuditTrailContentTypeEvent
            {
                PartName = context.ContentPartName, TypeName = context.ContentTypeName, EventContext = JObject.FromObject(context)
            });
        }
        public override async void ContentPartDetached(ContentPartDetachedContext context)
        {
            await RecordAuditTrailEventAsync(ContentTypeAuditTrailEventConfiguration.ContentPartDetached,
            new AuditTrailContentTypeEvent
            {
                PartName = context.ContentPartName, TypeName = context.ContentTypeName, EventContext = JObject.FromObject(context)
            });
        }
        public override async void ContentPartImported(ContentPartImportedContext context)
        {
            await RecordAuditTrailEventAsync(ContentTypeAuditTrailEventConfiguration.ContentPartImported,
            new AuditTrailContentTypeEvent
            {
                PartName = context.ContentPartDefinition.Name, EventContext = JObject.FromObject(context)
            });
        }
        public override async void ContentFieldAttached(ContentFieldAttachedContext context)
        {
            await RecordAuditTrailEventAsync(ContentTypeAuditTrailEventConfiguration.ContentFieldAttached,
            new AuditTrailContentTypeEvent
            {
                FieldName = context.ContentFieldName, PartName = context.ContentPartName, EventContext = JObject.FromObject(context)
            });
        }

        public override async void ContentFieldDetached(ContentFieldDetachedContext context)
        {
            await RecordAuditTrailEventAsync(ContentTypeAuditTrailEventConfiguration.ContentFieldDetached,
            new AuditTrailContentTypeEvent
            {
                FieldName = context.ContentFieldName, PartName = context.ContentPartName, EventContext = JObject.FromObject(context)
            });
        }
    }


}

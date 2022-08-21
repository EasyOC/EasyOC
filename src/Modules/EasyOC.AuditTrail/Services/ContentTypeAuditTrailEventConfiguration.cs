using Microsoft.Extensions.Options;
using OrchardCore.AuditTrail.Services.Models;

namespace EasyOC.AuditTrail.Services
{
    public class ContentTypeAuditTrailEventConfiguration : IConfigureOptions<AuditTrailOptions>
    {
        public const string ContentType = nameof(ContentType);
        public const string ContentTypeCreated = nameof(ContentTypeCreated);
        public const string ContentTypeRemoved = nameof(ContentTypeRemoved);
        public const string ContentTypeImported = nameof(ContentTypeImported);
        public const string ContentPartCreated = nameof(ContentPartCreated);
        public const string ContentPartImported = nameof(ContentPartImported);
        public const string ContentPartRemoved = nameof(ContentPartRemoved);
        public const string ContentPartAttached = nameof(ContentPartAttached);
        public const string ContentPartDetached = nameof(ContentPartDetached);
        public const string ContentFieldAttached = nameof(ContentFieldAttached);
        public const string ContentFieldDetached = nameof(ContentFieldDetached);

        public void Configure(AuditTrailOptions options)
        {
            options.For<ContentTypeAuditTrailEventConfiguration>(ContentType,
                S => S["ContentType"])
                .WithEvent(nameof(ContentTypeCreated), S => S["ContentTypeCreated"],
                S => S["A ContentType was created."], true)
                .WithEvent(nameof(ContentTypeRemoved), S => S["ContentTypeUpdated"],
                S => S["A ContentType was updated."], true)
                .WithEvent(nameof(ContentTypeImported), S => S["ContentTypeImported"],
                S => S["A ContentPart was Imported."], true)
                .WithEvent(nameof(ContentPartCreated), S => S["ContentPartCreated"],
                S => S["A ContentPart was created."], true)
                .WithEvent(nameof(ContentPartImported), S => S["ContentPartImported"],
                S => S["A ContentPart was  Imported."], true)
                .WithEvent(nameof(ContentPartRemoved), S => S["ContentPartRemoved"],
                S => S["A ContentPart was Removed."], true)
                .WithEvent(nameof(ContentPartAttached), S => S["ContentPartAttached"],
                S => S["A ContentPart was Attached."], true)
                .WithEvent(nameof(ContentPartDetached), S => S["ContentPartDetached"],
                S => S["A ContentPart was Detached."], true)
                .WithEvent(nameof(ContentFieldAttached), S => S["ContentFieldAttached"],
                S => S["A ContentField was Attached."], true)
                .WithEvent(nameof(ContentFieldDetached), S => S["ContentFieldDetached"],
                S => S["A ContentField was Detached."], true);
        }
    }


}

using EasyOC.Scripting.Models;
using EasyOC.Scripting.Servicies;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using Shortcodes;
using System.Linq;
using System.Threading.Tasks;

namespace EasyOC.Scripting.Handlers
{
    public class ScriptHandlerPartHandler : ContentHandlerBase
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IDbAccessableJSScopeBuilder _dbAccessableJSScopeBuilder;

        public ScriptHandlerPartHandler(IContentDefinitionManager contentDefinitionManager,
                                        IDbAccessableJSScopeBuilder dbAccessableJSScopeBuilder)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _dbAccessableJSScopeBuilder = dbAccessableJSScopeBuilder;
        }

        private async Task TrigerScriptHander(string eventName, ContentItem contentItem = default, object context = default, ContentTypeDefinition contentTypeDefinition = default)
        {
            if (eventName.EndsWith("Async"))
            {
                eventName.Replace("Async", string.Empty);
            }
            if (contentTypeDefinition is null)
            {
                contentTypeDefinition = _contentDefinitionManager.LoadTypeDefinition(contentItem.ContentType);
            }
            if (contentTypeDefinition is null)
            {
                return;
            }
            var handerSettings = contentTypeDefinition.Parts.FirstOrDefault(x => x.Name == nameof(ScriptHandlerPart));
            if (handerSettings != null)
            {
                var settings = handerSettings.GetSettings<ScriptHandlerPartSettings>();

                if (settings.Disabled || settings.ScriptHandlerSettingItems is null)
                {
                    return;
                }
                foreach (var item in settings.ScriptHandlerSettingItems.Where(x => !x.Disabled && x.EventName == eventName).OrderBy(x => x.Order))
                {
                    var scope = await _dbAccessableJSScopeBuilder.CreateScopeAsync();
                    scope.Engine.SetValue("parameters", new
                    {
                        contentItem,
                        contentTypeDefinition,
                        context
                    });
                    scope.Engine.Execute(item.Script);
                }
            }

        }

        public override async Task ActivatedAsync(ActivatedContentContext context)
        {
            await TrigerScriptHander(nameof(ActivatedAsync), contentItem: context.ContentItem, context);
        }

        public override async Task ActivatingAsync(ActivatingContentContext context)
        {
            await TrigerScriptHander(nameof(ActivatingAsync), contentItem: context.ContentItem, context, context.Definition);
        }
        public override async Task CloningAsync(CloneContentContext context)
        {
            await TrigerScriptHander(nameof(CloningAsync), contentItem: context.ContentItem, context);

        }
        public override async Task ClonedAsync(CloneContentContext context)
        {
            await TrigerScriptHander(nameof(ClonedAsync), contentItem: context.ContentItem, context);
        }

        public override async Task CreatingAsync(CreateContentContext context)
        {
            await TrigerScriptHander(nameof(CreatingAsync), contentItem: context.ContentItem, context);
        }
        public override async Task CreatedAsync(CreateContentContext context)
        {
            await TrigerScriptHander(nameof(CreatedAsync), contentItem: context.ContentItem, context);
        }


        public override async Task DraftSavedAsync(SaveDraftContentContext context)
        {
            await TrigerScriptHander(nameof(DraftSavedAsync), contentItem: context.ContentItem, context);

        }

        public override async Task DraftSavingAsync(SaveDraftContentContext context)
        {

            await TrigerScriptHander(nameof(DraftSavingAsync), contentItem: context.ContentItem, context);
        }

        public override async Task GetContentItemAspectAsync(ContentItemAspectContext context)
        {

            await TrigerScriptHander(nameof(GetContentItemAspectAsync), contentItem: context.ContentItem, context);
        }

        public override async Task ImportedAsync(ImportContentContext context)
        {
            await TrigerScriptHander(nameof(ImportedAsync), contentItem: context.ContentItem, context);
        }

        public override async Task ImportingAsync(ImportContentContext context)
        {
            await TrigerScriptHander(nameof(ImportingAsync), contentItem: context.ContentItem, context);
        }

        public override async Task InitializedAsync(InitializingContentContext context)
        {
            await TrigerScriptHander(nameof(InitializedAsync), contentItem: context.ContentItem, context);
        }

        public override async Task InitializingAsync(InitializingContentContext context)
        {
            await TrigerScriptHander(nameof(InitializingAsync), contentItem: context.ContentItem, context);
        }

        public override async Task LoadedAsync(LoadContentContext context)
        {
            await TrigerScriptHander(nameof(LoadedAsync), contentItem: context.ContentItem, context);
        }

        public override async Task LoadingAsync(LoadContentContext context)
        {
            await TrigerScriptHander(nameof(LoadingAsync), contentItem: context.ContentItem, context);
        }

        public override async Task PublishedAsync(PublishContentContext context)
        {
            await TrigerScriptHander(nameof(PublishedAsync), contentItem: context.ContentItem, context);
        }

        public override async Task PublishingAsync(PublishContentContext context)
        {
            await TrigerScriptHander(nameof(PublishingAsync), contentItem: context.ContentItem, context);
        }

        public override async Task RemovedAsync(RemoveContentContext context)
        {
            await TrigerScriptHander(nameof(RemovedAsync), contentItem: context.ContentItem, context);
        }

        public override async Task RemovingAsync(RemoveContentContext context)
        {
            await TrigerScriptHander(nameof(RemovingAsync), contentItem: context.ContentItem, context);
        }

        public override async Task RestoredAsync(RestoreContentContext context)
        {
            await TrigerScriptHander(nameof(RestoredAsync), contentItem: context.ContentItem, context);
        }

        public override async Task RestoringAsync(RestoreContentContext context)
        {
            await TrigerScriptHander(nameof(RestoringAsync), contentItem: context.ContentItem, context);
        }

        public override async Task UnpublishedAsync(PublishContentContext context)
        {
            await TrigerScriptHander(nameof(UnpublishedAsync), contentItem: context.ContentItem, context);
        }

        public override async Task UnpublishingAsync(PublishContentContext context)
        {
            await TrigerScriptHander(nameof(UnpublishingAsync), contentItem: context.ContentItem, context);
        }

        public override async Task UpdatedAsync(UpdateContentContext context)
        {
            await TrigerScriptHander(nameof(UpdatedAsync), contentItem: context.ContentItem, context);
        }

        public override async Task UpdatingAsync(UpdateContentContext context)
        {
            await TrigerScriptHander(nameof(UpdatingAsync), contentItem: context.ContentItem, context);
        }

        public override async Task ValidatedAsync(ValidateContentContext context)
        {
            await TrigerScriptHander(nameof(ValidatedAsync), contentItem: context.ContentItem, context);
        }

        public override async Task ValidatingAsync(ValidateContentContext context)
        {
            await TrigerScriptHander(nameof(ValidatingAsync), contentItem: context.ContentItem, context);
        }

        public override async Task VersionedAsync(VersionContentContext context)
        {
            await TrigerScriptHander(nameof(VersionedAsync), contentItem: context.ContentItem, context);
        }

        public override async Task VersioningAsync(VersionContentContext context)
        {
            await TrigerScriptHander(nameof(VersioningAsync), contentItem: context.ContentItem, context);
        }

    }
}
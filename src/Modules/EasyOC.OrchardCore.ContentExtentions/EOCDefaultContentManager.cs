using EasyOC.OrchardCore.ContentExtentions.Handlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YesSql;
using YesSql.Services;

namespace OrchardCore.ContentManagement
{
    public class EOCDefaultContentManager : DefaultContentManager
    {
        private const int ImportBatchSize = 500; 
  
        private readonly ISession _session;
        private readonly ILogger _logger;
 
        private readonly IClock _clock;
        private readonly IEnumerable<IBatchImportEventHandler> _batchImportEventHandlers;
        public EOCDefaultContentManager(
            IContentDefinitionManager contentDefinitionManager,
            IContentManagerSession contentManagerSession,
            IEnumerable<IContentHandler> handlers,
            ISession session,
            IContentItemIdGenerator idGenerator,
            ILogger<EOCDefaultContentManager> logger,
            IClock clock, IEnumerable<IBatchImportEventHandler> batchImportEventHandlers) :
            base(contentDefinitionManager, contentManagerSession, handlers, session, idGenerator, logger, clock)
        { 
            _session = session; 
            _logger = logger;
            _clock = clock;
            _batchImportEventHandlers = batchImportEventHandlers;
        }


        public new async Task ImportAsync(IEnumerable<ContentItem> contentItems)
        {
            var contentList = contentItems.Select(x => new ImportContentContext(x)).ToList();
            await _batchImportEventHandlers.InvokeAsync((handler, list) => handler.BeforeImportAsync(list), contentList, _logger);

            var skip = 0;

            var importedVersionIds = new HashSet<string>();

            var batchedContentItems = contentItems.Take(ImportBatchSize);

            while (batchedContentItems.Any())
            {
                // Preload all the versions for this batch from the database.
                var versionIds = batchedContentItems
                     .Where(x => !String.IsNullOrEmpty(x.ContentItemVersionId))
                     .Select(x => x.ContentItemVersionId);

                var itemIds = batchedContentItems
                    .Where(x => !String.IsNullOrEmpty(x.ContentItemId))
                    .Select(x => x.ContentItemId);

                var existingContentItems = await _session
                    .Query<ContentItem, ContentItemIndex>(x =>
                        x.ContentItemId.IsIn(itemIds) &&
                        (x.Latest || x.Published || x.ContentItemVersionId.IsIn(versionIds)))
                    .ListAsync();

                var versionsToUpdate = existingContentItems.Where(c => versionIds.Any(v => String.Equals(v, c.ContentItemVersionId, StringComparison.OrdinalIgnoreCase)));
                var versionsThatMaybeEvicted = existingContentItems.Except(versionsToUpdate);

                foreach (var version in existingContentItems)
                {
                    await LoadAsync(version);
                }

                foreach (var importingItem in batchedContentItems)
                {
                    ContentItem originalVersion = null;
                    if (!String.IsNullOrEmpty(importingItem.ContentItemVersionId))
                    {
                        if (importedVersionIds.Contains(importingItem.ContentItemVersionId))
                        {
                            _logger.LogInformation("Duplicate content item version id '{ContentItemVersionId}' skipped", importingItem.ContentItemVersionId);
                            continue;
                        }

                        importedVersionIds.Add(importingItem.ContentItemVersionId);

                        originalVersion = versionsToUpdate.FirstOrDefault(x => String.Equals(x.ContentItemVersionId, importingItem.ContentItemVersionId, StringComparison.OrdinalIgnoreCase));
                    }

                    if (originalVersion == null)
                    {
                        // The version does not exist in the current database.
                        var context = new ImportContentContext(importingItem);

                        await Handlers.InvokeAsync((handler, context) => handler.ImportingAsync(context), context, _logger);

                        var evictionVersions = versionsThatMaybeEvicted.Where(x => String.Equals(x.ContentItemId, importingItem.ContentItemId, StringComparison.OrdinalIgnoreCase));
                        var result = await base.CreateContentItemVersionAsync(importingItem);
                        if (!result.Succeeded)
                        {
                            if (_logger.IsEnabled(LogLevel.Error))
                            {
                                _logger.LogError("Error importing content item version id '{ContentItemVersionId}' : '{Errors}'", importingItem?.ContentItemVersionId, string.Join(", ", result.Errors));
                            }

                            throw new ValidationException(string.Join(", ", result.Errors));
                        }

                        // Imported handlers will only be fired if the validation has been successful.
                        // Consumers should implement validated handlers to alter the success of that operation.
                        await ReversedHandlers.InvokeAsync((handler, context) => handler.ImportedAsync(context), context, _logger);
                    }
                    else
                    {
                        // The version exists in the database.
                        // It is important to only import changed items.
                        // We compare the two versions and skip importing it if they are the same.
                        // We do this to prevent unnecessary sql updates, and because UpdateContentItemVersionAsync
                        // may remove drafts of updated items.
                        // This is necesary because an imported item maybe set to latest, and published.
                        // In this case, the draft item in the system, must be removed, or there will be two drafts.
                        // The draft item should be removed, because it would now be orphaned, as the imported published item
                        // would be further ahead, on a timeline, between the two.

                        var jImporting = JObject.FromObject(importingItem);

                        // Removed Published and Latest from consideration when evaluating.
                        // Otherwise an import of an unchanged (but published) version would overwrite a newer published version.
                        jImporting.Remove(nameof(ContentItem.Published));
                        jImporting.Remove(nameof(ContentItem.Latest));

                        var jOriginal = JObject.FromObject(originalVersion);

                        jOriginal.Remove(nameof(ContentItem.Published));
                        jOriginal.Remove(nameof(ContentItem.Latest));

                        if (JToken.DeepEquals(jImporting, jOriginal))
                        {
                            _logger.LogInformation("Importing '{ContentItemVersionId}' skipped as it is unchanged", importingItem.ContentItemVersionId);
                            continue;
                        }

                        // Handlers are only fired if the import is going ahead.
                        var context = new ImportContentContext(importingItem, originalVersion);

                        await Handlers.InvokeAsync((handler, context) => handler.ImportingAsync(context), context, _logger);

                        var evictionVersions = versionsThatMaybeEvicted.Where(x => String.Equals(x.ContentItemId, importingItem.ContentItemId, StringComparison.OrdinalIgnoreCase));
                        var result = await UpdateContentItemVersionAsync(originalVersion, importingItem);
                        if (!result.Succeeded)
                        {
                            if (_logger.IsEnabled(LogLevel.Error))
                            {
                                _logger.LogError("Error importing content item version id '{ContentItemVersionId}' : '{Errors}'", importingItem.ContentItemVersionId, string.Join(", ", result.Errors));
                            }

                            throw new ValidationException(string.Join(", ", result.Errors));
                        }

                        // Imported handlers will only be fired if the validation has been successful.
                        // Consumers should implement validated handlers to alter the success of that operation.
                        await ReversedHandlers.InvokeAsync((handler, context) => handler.ImportedAsync(context), context, _logger);
                    }
                }

                skip += ImportBatchSize;
                batchedContentItems = contentItems.Skip(skip).Take(ImportBatchSize);
            }

            await _batchImportEventHandlers.InvokeAsync((handler, list) => handler.AfterImportAsync(list), contentList, _logger);


        }

    }
}

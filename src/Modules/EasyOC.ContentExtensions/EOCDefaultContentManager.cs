using EasyOC.ContentExtensions.Handlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement.CompiledQueries;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
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
    public class EOCDefaultContentManager : IContentManager
    {
        private const int ImportBatchSize = 500;

        private readonly ISession _session;
        private readonly ILogger _logger;
        private readonly IEnumerable<IBatchImportEventHandler> _batchImportEventHandlers;
        private readonly IHandleExecutor _handleExecutor;
        private readonly DefaultContentManager _defaultContentManager;

        public EOCDefaultContentManager(
            IContentDefinitionManager contentDefinitionManager,
            IContentManagerSession contentManagerSession,
            IEnumerable<IContentHandler> handlers,
            ISession session,
            IContentItemIdGenerator idGenerator,
            ILogger<EOCDefaultContentManager> logger,
            ILogger<DefaultContentManager> defaultlogger,
            IClock clock, IEnumerable<IBatchImportEventHandler> batchImportEventHandlers,
            IHandleExecutor<EOCDefaultContentManager> handleExecutor)

        {
            _defaultContentManager = new DefaultContentManager(contentDefinitionManager,
                contentManagerSession, handlers, session,
                idGenerator, defaultlogger, clock);
            _session = session;
            _logger = logger;
            _batchImportEventHandlers = batchImportEventHandlers;
            _handleExecutor = handleExecutor;
        }

        public IEnumerable<IContentHandler> Handlers => _defaultContentManager.Handlers;
        public IEnumerable<IContentHandler> ReversedHandlers => _defaultContentManager.ReversedHandlers;

        public async Task ImportAsync(IEnumerable<ContentItem> contentItems)
        {
            var contentList = contentItems.Select(x => new ImportContentContext(x)).ToList();
            await _batchImportEventHandlers.InvokeAsync((handler, list) => handler.BeforeImportAsync(list), contentList,
                _logger);

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

                var versionsToUpdate = existingContentItems.Where(c =>
                    versionIds.Any(v => String.Equals(v, c.ContentItemVersionId, StringComparison.OrdinalIgnoreCase)));
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
                            _logger.LogInformation("Duplicate content item version id '{ContentItemVersionId}' skipped",
                                importingItem.ContentItemVersionId);
                            continue;
                        }

                        importedVersionIds.Add(importingItem.ContentItemVersionId);

                        originalVersion = versionsToUpdate.FirstOrDefault(x => String.Equals(x.ContentItemVersionId,
                            importingItem.ContentItemVersionId, StringComparison.OrdinalIgnoreCase));
                    }

                    if (originalVersion == null)
                    {
                        // The version does not exist in the current database.
                        var context = new ImportContentContext(importingItem);

                        await _handleExecutor.InvokeAsync(_defaultContentManager.Handlers,
                            (handler, context) => handler.ImportingAsync(context), context);

                        var evictionVersions = versionsThatMaybeEvicted.Where(x =>
                            String.Equals(x.ContentItemId, importingItem.ContentItemId,
                                StringComparison.OrdinalIgnoreCase));
                        var result = await _defaultContentManager.CreateContentItemVersionAsync(importingItem);
                        if (!result.Succeeded)
                        {
                            if (_logger.IsEnabled(LogLevel.Error))
                            {
                                _logger.LogError(
                                    "Error importing content item version id '{ContentItemVersionId}' : '{Errors}'",
                                    importingItem?.ContentItemVersionId, string.Join(", ", result.Errors));
                            }

                            throw new ValidationException(string.Join(", ", result.Errors));
                        }

                        // Imported handlers will only be fired if the validation has been successful.
                        // Consumers should implement validated handlers to alter the success of that operation.
                        await _handleExecutor.InvokeAsync(_defaultContentManager.ReversedHandlers,
                            (handler, context) => handler.ImportedAsync(context), context);
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
                            _logger.LogInformation("Importing '{ContentItemVersionId}' skipped as it is unchanged",
                                importingItem.ContentItemVersionId);
                            continue;
                        }

                        // Handlers are only fired if the import is going ahead.
                        var context = new ImportContentContext(importingItem, originalVersion);

                        await _handleExecutor.InvokeAsync(_defaultContentManager.Handlers,
                            (handler, context) => handler.ImportingAsync(context), context);

                        var evictionVersions = versionsThatMaybeEvicted.Where(x =>
                            String.Equals(x.ContentItemId, importingItem.ContentItemId,
                                StringComparison.OrdinalIgnoreCase));
                        var result = await UpdateContentItemVersionAsync(originalVersion, importingItem);
                        if (!result.Succeeded)
                        {
                            if (_logger.IsEnabled(LogLevel.Error))
                            {
                                _logger.LogError(
                                    "Error importing content item version id '{ContentItemVersionId}' : '{Errors}'",
                                    importingItem.ContentItemVersionId, string.Join(", ", result.Errors));
                            }

                            throw new ValidationException(string.Join(", ", result.Errors));
                        }

                        // Imported handlers will only be fired if the validation has been successful.
                        // Consumers should implement validated handlers to alter the success of that operation.

                        await _handleExecutor.InvokeAsync(_defaultContentManager.ReversedHandlers,
                            (handler, context) => handler.ImportedAsync(context), context);
                    }
                }

                skip += ImportBatchSize;
                batchedContentItems = contentItems.Skip(skip).Take(ImportBatchSize);
            }

            await _batchImportEventHandlers.InvokeAsync((handler, list) => handler.AfterImportAsync(list), contentList,
                _logger);
        }

        public Task<ContentItem> GetAsync(string contentItemId, VersionOptions options)
        {
            return _defaultContentManager.GetAsync(contentItemId, options);
        }

        public Task<ContentItem> NewAsync(string contentType)
        {
            return _defaultContentManager.NewAsync(contentType);
        }

        public Task UpdateAsync(ContentItem contentItem)
        {
            return _defaultContentManager.UpdateAsync(contentItem);
        }

        public Task CreateAsync(ContentItem contentItem, VersionOptions options)
        {
            return _defaultContentManager.CreateAsync(contentItem, options);
        }

        public Task<ContentValidateResult> CreateContentItemVersionAsync(ContentItem contentItem)
        {
            return _defaultContentManager.CreateContentItemVersionAsync(contentItem);
        }

        public Task<ContentValidateResult> UpdateContentItemVersionAsync(ContentItem updatingVersion,
            ContentItem updatedVersion)
        {
            return _defaultContentManager.UpdateContentItemVersionAsync(updatingVersion, updatedVersion);
        }

        public Task<ContentValidateResult> ValidateAsync(ContentItem contentItem)
        {
            return _defaultContentManager.ValidateAsync(contentItem);
        }

        public Task<ContentValidateResult> RestoreAsync(ContentItem contentItem)
        {
            return _defaultContentManager.RestoreAsync(contentItem);
        }

        public Task<ContentItem> GetAsync(string id)
        {
            return _defaultContentManager.GetAsync(id);
        }

        public Task<IEnumerable<ContentItem>> GetAsync(IEnumerable<string> contentItemIds, bool latest = false)
        {
            return _defaultContentManager.GetAsync(contentItemIds, latest);
        }

        public Task<ContentItem> GetVersionAsync(string contentItemVersionId)
        {
            return _defaultContentManager.GetVersionAsync(contentItemVersionId);
        }

        public Task<ContentItem> LoadAsync(ContentItem contentItem)
        {
            return _defaultContentManager.LoadAsync(contentItem);
        }

        public Task RemoveAsync(ContentItem contentItem)
        {
            return _defaultContentManager.RemoveAsync(contentItem);
        }

        public Task DiscardDraftAsync(ContentItem contentItem)
        {
            return _defaultContentManager.DiscardDraftAsync(contentItem);
        }

        public Task SaveDraftAsync(ContentItem contentItem)
        {
            return _defaultContentManager.SaveDraftAsync(contentItem);
        }

        public async Task PublishAsync(ContentItem contentItem)
        {
            if (contentItem.Published)
            {
                return;
            }

            // Create a context for the item and it's previous published record
            // Because of this query the content item will need to be re-enlisted
            // to be saved.
            var previous = await _session
                .Query<ContentItem, ContentItemIndex>(x =>
                    x.ContentItemId == contentItem.ContentItemId && x.Published)
                .FirstOrDefaultAsync();

            var context = new PublishContentContext(contentItem, previous);

            // invoke handlers to acquire state, or at least establish lazy loading callbacks
            await _handleExecutor.InvokeAsync(Handlers, (handler, context1) =>
                handler.PublishingAsync(context1), context);

            if (context.Cancel)
            {
                return;
            }

            if (previous != null)
            {
                _session.Save(previous, checkConcurrency: true);
                previous.Latest = previous.Published = false;
            }

            contentItem.Published = true;
            _session.Save(contentItem, checkConcurrency: true);

            await _handleExecutor.InvokeAsync(ReversedHandlers,
                (handler, context1) => handler.PublishedAsync(context1), context);
        }

        public Task UnpublishAsync(ContentItem contentItem)
        {
            return _defaultContentManager.UnpublishAsync(contentItem);
        }

        public Task<TAspect> PopulateAspectAsync<TAspect>(IContent content, TAspect aspect)
        {
            return _defaultContentManager.PopulateAspectAsync(content, aspect);
        }

        public Task<ContentItem> CloneAsync(ContentItem contentItem)
        {
            return _defaultContentManager.CloneAsync(contentItem);
        }
    }
}

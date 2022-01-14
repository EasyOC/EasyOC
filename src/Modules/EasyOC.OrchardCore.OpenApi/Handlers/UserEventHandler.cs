using EasyOC.OrchardCore.OpenApi.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentFields.Indexing.SQL;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Entities;
using OrchardCore.Users.Handlers;
using OrchardCore.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.OpenApi.Handlers
{
    public class UserEventHandler : UserEventHandlerBase
    {
        private static readonly JsonMergeSettings UpdateJsonMergeSettings = new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Replace };

        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentItemIdGenerator _idGenerator;
        private readonly IContentManager _contentManager;
        //private readonly IFreeSql _fsql;
        private readonly ILogger _logger;
        public UserEventHandler(
          IContentDefinitionManager contentDefinitionManager,
          ILogger<UserEventHandler> logger,
          IContentItemIdGenerator idGenerator,
          IContentManager contentManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _logger = logger;
            _idGenerator = idGenerator;
            _contentManager = contentManager;
        }



        private Task UpdateIndexAsync(UserContextBase context, Action<ContentItem> action)
        {
            var user = context.User as User;
            var contentItem = user.As<ContentItem>(nameof(UserProfile));
            contentItem.Content["User"] = JObject.FromObject(new { user.UserName, user.UserId, user.Email });
            contentItem.ContentItemId = user.UserId;
            contentItem.Owner = user.UserId;
            contentItem.Latest = true;
            contentItem.Published = true;
            action?.Invoke(contentItem);
            return Task.CompletedTask;
        }
        public override async Task CreatedAsync(UserCreateContext context)
        {
            await UpdateIndexAsync(context, async model =>
           {
               await _contentManager.CreateAsync(model);
           });

        }

        public override async Task UpdatedAsync(UserUpdateContext context)
        {
            await UpdateIndexAsync(context, async model =>
          {
              var contentItem = await _contentManager.GetAsync(model.ContentItemId);
              if (contentItem != null)
              {
                  contentItem.Merge(model, UpdateJsonMergeSettings);
                  contentItem.Latest = true;
                  contentItem.Published = true;
                  await _contentManager.UpdateAsync(contentItem);
              }
              else
              {
                  await _contentManager.CreateAsync(model);
              }
          });
        }

        private async Task CreateAsync(ContentItem model)
        {
            //var newContentItem = await _contentManager.NewAsync(model.ContentType);
            //newContentItem.Merge(model, UpdateJsonMergeSettings);
            //newContentItem.ContentItemId = model.ContentItemId;
            //newContentItem.Latest = true;
            //newContentItem.Published = true;
            await _contentManager.CreateAsync(model);
        }

        public override async Task DeletedAsync(UserDeleteContext context)
        {
            await UpdateIndexAsync(context, async contentitem =>
          {
              await _contentManager.RemoveAsync(contentitem);
          });
        }
    }
}

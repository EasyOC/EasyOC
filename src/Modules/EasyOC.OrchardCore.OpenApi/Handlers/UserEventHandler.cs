using AutoMapper;
using EasyOC.OrchardCore.OpenApi.Dto;
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
        private readonly IMapper _mapper;
        public UserEventHandler(
          IContentDefinitionManager contentDefinitionManager,
          ILogger<UserEventHandler> logger,
          IContentItemIdGenerator idGenerator,
          IContentManager contentManager, IMapper mapper)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _logger = logger;
            _idGenerator = idGenerator;
            _contentManager = contentManager;
            _mapper = mapper;
        }



        private async Task UpdateIndexAsync(UserContextBase context, Action<ContentItem> action)
        {
            var user = context.User as User;
            var contentItem = user.As<ContentItem>("UserProfileInternal");
            if (contentItem == null)
            {
                return;
            }
            var userSimpleData = _mapper.Map<UserDetailsDto>(user);
            userSimpleData.Properties = null;
            var jContent = contentItem.Content as JObject;
            try
            {

                jContent["UserProfile"] = contentItem.Content["UserProfileInternal"] as JObject;
                jContent.Remove("UserProfileInternal");
                jContent["UserProfile"]["OwnerUser"] = JObject.FromObject(new
                {
                    UserIds = new string[] { user.UserId },
                    UserNames = new string[] { user.UserName }
                });
                jContent["UserProfile"]["User"] = JObject.FromObject(userSimpleData);
                jContent["UserProfile"]["UserName"] = JObject.FromObject(new { Text = user.UserName });
                jContent["UserProfile"]["Email"] = JObject.FromObject(new { Text = user.Email });
                jContent["UserProfile"]["UserId"] = JObject.FromObject(new { Text = user.UserId });
            }
            catch (Exception e)
            {
                throw e;
            }
            contentItem.ContentItemId = user.UserId;
            contentItem.Owner = user.UserId;
            //使用 UserProfileInternal 更新 UserProfile
            contentItem.ContentType = "UserProfile";
            contentItem.Latest = true;
            contentItem.Published = true;
            action?.Invoke(contentItem);
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

using AutoMapper;
using EasyOC.OrchardCore.OpenApi.Dto;
using EasyOC.OrchardCore.OpenApi.Model;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.Indexing.SQL;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Entities;
using OrchardCore.Users.Handlers;
using OrchardCore.Users.Models;
using System;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.OrchardCore.OpenApi.Handlers
{
    public class EOCUserEventHandler : UserEventHandlerBase
    {
        private static readonly JsonMergeSettings UpdateJsonMergeSettings = new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Replace };

        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentItemIdGenerator _idGenerator;
        private readonly IContentManager _contentManager;
        private readonly INotifier notifier;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IHtmlLocalizer htmlLocalizer;
        private readonly ISession _session;
        public EOCUserEventHandler(
          IContentDefinitionManager contentDefinitionManager,
          ILogger<EOCUserEventHandler> logger,
          IContentItemIdGenerator idGenerator,
          IContentManager contentManager, IMapper mapper, IHtmlLocalizer<EOCUserEventHandler> htmlLocalizer, INotifier notifier, ISession session)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _logger = logger;
            _idGenerator = idGenerator;
            _contentManager = contentManager;
            _mapper = mapper;
            this.htmlLocalizer = htmlLocalizer;
            this.notifier = notifier;
            _session = session;
        }



        private async Task UpdateIndexAsync(UserContextBase context)
        {
            var user = context.User as User;
            var internalProfile = user.As<ContentItem>("UserProfileInternal");
            if (internalProfile == null || internalProfile.Content.UserProfilePart == null)
            {
                return;
            }
      
            var contentItem = await _contentManager.NewAsync(nameof(UserProfile));
        
            
            contentItem.Owner = user.UserId;
            var existsContent = await _contentManager.GetAsync(user.UserId, VersionOptions.DraftRequired);
            var isCreate = existsContent == null;
            if (!isCreate)
            {
                contentItem = existsContent.Merge(contentItem, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Replace });
            }
            contentItem.ContentItemId = user.UserId;

            await _contentManager.CreateOrUpdateAndPublishAsync(contentItem, isCreate, new PublishOptions
            {
                Notifier = notifier,
                HtmlLocalizer = htmlLocalizer
            });

        }
        public override async Task CreatedAsync(UserCreateContext context)
        {
            await UpdateIndexAsync(context);

        }

        public override async Task UpdatedAsync(UserUpdateContext context)
        {

            await UpdateIndexAsync(context);
        }

        public override async Task DeletedAsync(UserDeleteContext context)
        {
            var user = context.User as User;
            var existsContent = await _contentManager.GetAsync(user.UserId);
            await _contentManager.RemoveAsync(existsContent);
        }
    }
}

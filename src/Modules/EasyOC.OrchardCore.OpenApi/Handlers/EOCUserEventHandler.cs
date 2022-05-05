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



        private async Task UpdateIndexAsync(UserContextBase context, Action<ContentItem> action = null)
        {
            var user = context.User as User;
            var userProfileContent = user.As<ContentItem>("UserProfileInternal");
            if (userProfileContent == null)
            {
                return;
            }
            userProfileContent.ContentType = "UserProfile";
            var contentItem = await _contentManager.CloneAsync(userProfileContent); 
            contentItem.Remove("UserProfileInternal");
            //ContentPart must be registered manually
            contentItem.Alter<UserProfile>("UserProfile", profilePart =>
            {
                profilePart.UserName.Text = user.UserName;
                profilePart.Email.Text = user.Email;
                profilePart.UserId.Text = user.UserId;
                profilePart.OwnerUser.UserIds = new string[] { user.UserId };

                // The following code does not work
                //profilePart.GetOrCreate<TextField>("UserId").Text = user.UserId; 

            });
            
            contentItem.Owner = user.UserId;
            contentItem.ContentItemId = user.UserId;
            var existsContent = await _contentManager.GetAsync(user.UserId);
            var isCreate = existsContent == null;
            if (!isCreate)
            {
                contentItem = existsContent.Merge(contentItem, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Replace });
            }

            action?.Invoke(contentItem);
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

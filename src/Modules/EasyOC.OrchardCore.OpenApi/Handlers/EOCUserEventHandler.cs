using AutoMapper;
using EasyOC.OrchardCore.OpenApi.Dto;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
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

            var jContent = contentItem.Content as JObject;

            jContent["UserProfile"] = contentItem.Content["UserProfileInternal"] as JObject;
            jContent.Remove("UserProfileInternal");
            jContent["UserProfile"]["OwnerUser"] = JObject.FromObject(new
            {
                UserIds = new string[] { user.UserId },
                UserNames = new string[] { user.UserName }
            });
            //jContent["UserProfile"]["User"] = JObject.FromObject(userSimpleData);

            jContent["UserProfile"]["UserName"] = JObject.FromObject(new { Text = user.UserName });
            jContent["UserProfile"]["Email"] = JObject.FromObject(new { Text = user.Email });
            jContent["UserProfile"]["UserId"] = JObject.FromObject(new { Text = user.UserId });
            contentItem.Owner = user.UserId;
            //使用 UserProfileInternal 更新 UserProfile

            contentItem.ContentType = "UserProfile";


            var existsContent = await _session.Query<ContentItem, UserPickerFieldIndex>
                                                         (x => x.SelectedUserId == user.UserId && x.Published && x.Latest)
                                        .FirstOrDefaultAsync(_contentManager);
            
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
            await _session.SaveChangesAsync();

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

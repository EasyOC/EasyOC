using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.DisplayManagement.Notify;
using System.Net;
using System.Threading.Tasks;

namespace EasyOC
{
    public static class ContentManagerExtentions
    {

        public static async Task<bool> CreateOrUpdateAndPublishAsync(this IContentManager contentManager,
            ContentItem contentItem, bool isCreate,
            PublishOptions publishOptions = null)
        {
            if (isCreate)
            {
                return await contentManager.CreateAndPublishAsync(contentItem, publishOptions);
            }
            else
            {
                return await contentManager.UpdateAndPublishAsync(contentItem, publishOptions);
            }
        }

        /// <summary>
        /// 创建并发布内容
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="contentItem"></param>
        /// <param name="publishOptions">发布选项</param> 
        /// <returns></returns>
        public static async Task<bool> CreateAndPublishAsync(this IContentManager contentManager, ContentItem contentItem,
            PublishOptions publishOptions = null)
        {
            var result = await contentManager.UpdateValidateAndCreateAsync(contentItem, VersionOptions.Draft);
            return await DoPublish(contentManager, contentItem,
                  result, publishOptions);

        }
        /// <summary>
        /// 更新并发布内容，如果需要重新触发默认的 publish 事件，需要更新 ：contentitem.published = false
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="contentItem"></param>
        /// <param name="publishOptions">发布选项</param>
        /// <returns></returns>
        public static async Task<bool> UpdateAndPublishAsync(this IContentManager contentManager, ContentItem contentItem,
          PublishOptions publishOptions = null)
        {
            await contentManager.UpdateAsync(contentItem);
            var result = await contentManager.ValidateAsync(contentItem);
            return await DoPublish(contentManager, contentItem, result, publishOptions);
        }



        private static async Task<bool> DoPublish(IContentManager contentManager, ContentItem contentItem,
            ContentValidateResult result,
           PublishOptions publishOptions = null
            )
        {
            if (!result.Succeeded)
            {
                if (publishOptions != null && publishOptions.Notifier != null)
                {
                    foreach (var error in result.Errors)
                    {
                        if (publishOptions?.HtmlLocalizer != null)
                        {
                            await publishOptions.Notifier
                                .ErrorAsync(publishOptions?.HtmlLocalizer[error.ErrorMessage, error.MemberNames]);
                        }
                    }
                }
                var continueThrow = publishOptions?.OnError?.Invoke(result);
                if (continueThrow ?? true)
                {
                    throw new AppFriendlyException(HttpStatusCode.BadRequest, result.Errors);
                }
                return false;
            }
            else
            {
                if (publishOptions?.BeforePublish?.Invoke(contentItem) ?? true)
                {
                    await contentManager.PublishAsync(contentItem);
                };
                return true;
            }
        }



    }
}

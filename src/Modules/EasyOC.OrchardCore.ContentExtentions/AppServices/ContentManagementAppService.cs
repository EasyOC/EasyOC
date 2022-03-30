using EasyOC.Core.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.Contents;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos
{
    [EOCAuthorization("AccessContentApi")]
    public class ContentManagementAppService : AppServcieBase
    {

        private readonly IContentDefinitionManager _contentDefinitionManager;

        private readonly IContentManager _contentManager;

        public ContentManagementAppService(IContentManager contentManager, IContentDefinitionManager contentDefinitionManager)
        {
            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
        }

        private static readonly JsonMergeSettings UpdateJsonMergeSettings = new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Replace };

        public async Task<bool> DeleteAsync(string contentItemId)
        {
            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);

            if (contentItem == null)
            {
                throw new AppFriendlyException(HttpStatusCode.NotFound);

            }

            if (!await AuthorizationService.AuthorizeAsync(User, CommonPermissions.DeleteContent, contentItem))
            {
                throw new AppFriendlyException(HttpStatusCode.Unauthorized);
            }

            await _contentManager.RemoveAsync(contentItem);

            return true;

        }
        public async Task<string> PostContent([FromBody] ContentModel model, [FromQuery] bool draft = false)
        {
            // It is really important to keep the proper method calls order with the ContentManager
            // so that all event handlers gets triggered in the right sequence.
            if (model.ContentType.IsNullOrWhiteSpace())
            {
                throw new AppFriendlyException("invaild property: contentType", StatusCodes.Status400BadRequest);
            }
            var typeDef = _contentDefinitionManager.GetTypeDefinition(model.ContentType);
            if (typeDef == null)
            {
                throw new AppFriendlyException($"invaild property: contentType ,'{model.ContentType}' is not found "
                    , StatusCodes.Status400BadRequest);
            }
            var contentItem = await _contentManager.GetAsync(model.ContentItemId, VersionOptions.DraftRequired);
            if (contentItem == null)
            {
                if (!await AuthorizationService.AuthorizeAsync(User, CommonPermissions.PublishContent))
                {
                    throw new AppFriendlyException(HttpStatusCode.Unauthorized);
                }
                var newContentItem = await _contentManager.NewAsync(model.ContentType);
                newContentItem.Merge(MergeFromSimpleData(newContentItem, model, typeDef));

                var result = await _contentManager.UpdateValidateAndCreateAsync(newContentItem, VersionOptions.Draft);

                if (!result.Succeeded)
                {
                    throw new AppFriendlyException(HttpStatusCode.BadRequest, result.Errors);
                }
                contentItem = newContentItem;
            }
            else
            {
                if (!await AuthorizationService.AuthorizeAsync(User, CommonPermissions.EditContent, contentItem))
                {
                    throw new AppFriendlyException(HttpStatusCode.Unauthorized, "当前用户没有权限更新此内容");
                }
                contentItem.Merge(MergeFromSimpleData(contentItem, model, typeDef), UpdateJsonMergeSettings);

                await _contentManager.UpdateAsync(contentItem);
                var result = await _contentManager.ValidateAsync(contentItem);

                if (!result.Succeeded)
                {
                    throw new AppFriendlyException(HttpStatusCode.BadRequest, result.Errors);
                }
            }

            if (!draft)
            {
                await _contentManager.PublishAsync(contentItem);
            }
            else
            {
                await _contentManager.SaveDraftAsync(contentItem);
            }

            return contentItem.ContentItemId;
        }

        public static ContentItem MergeFromSimpleData(ContentItem contentItem, ContentModel model, ContentTypeDefinition typeDefinition)
        {
            JObject jObject = JObject.FromObject(model);

            #region Update BaseInfo
            if (model.ContentItemId != null)
            {
                contentItem.ContentItemId = model.ContentItemId;
            }
            //if (model.ContentItemVersionId != null)
            //{
            //    contentItem.ContentItemVersionId = model.ContentItemVersionId;
            //}

            if (model.DisplayText != null)
            {
                contentItem.DisplayText = model.DisplayText;
            }


            #endregion
            //填充自带属性
            var selfPart = typeDefinition.Parts.FirstOrDefault(x => x.Name == model.ContentType);
            if (selfPart != null)
            {
                foreach (var item in selfPart.PartDefinition.Fields)
                {
                    var fieldPath = ContentTypeManagementAppService.GetFiledValuePath(item.FieldDefinition.Name);
                    if (fieldPath != null)
                    {
                        if (jObject.ContainsKey(item.Name.ToCamelCase()))
                        {
                            contentItem.Content[contentItem.ContentType][item.Name][fieldPath] = jObject[(item.Name.ToCamelCase())];

                        }
                    }
                }
            }

            foreach (var part in typeDefinition.Parts.Where(x => x.Name != contentItem.ContentType))
            {
                JToken partData;
                if (!jObject.ContainsKey(part.Name.ToCamelCase()) || (partData = jObject[part.Name.ToCamelCase()]) == null)
                {
                    continue;
                }
                var partObj = new JObject(partData);
                foreach (var item in part.PartDefinition.Fields)
                {
                    var fieldPath = ContentTypeManagementAppService.GetFiledValuePath(item.FieldDefinition.Name);
                    if (fieldPath != null && partObj.ContainsKey(item.Name.ToCamelCase()))
                    {
                        contentItem.Content[part.Name][item.Name][fieldPath] =
                               partObj[item.Name.ToCamelCase()];

                    }

                }
            }

            return contentItem;
        }
    }
}

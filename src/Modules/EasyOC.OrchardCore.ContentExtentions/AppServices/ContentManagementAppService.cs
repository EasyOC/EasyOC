using EasyOC.Core.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        public ContentManagementAppService(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        private static readonly JsonMergeSettings UpdateJsonMergeSettings = new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Replace };


        public async Task<string> PostContent(ContentModel model)
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
                throw new AppFriendlyException("invaild property: contentType", StatusCodes.Status400BadRequest);
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
                    throw new AppFriendlyException(HttpStatusCode.Unauthorized);
                }
                contentItem.Merge(MergeFromSimpleData(contentItem, model, typeDef), UpdateJsonMergeSettings);

                await _contentManager.UpdateAsync(contentItem);
                var result = await _contentManager.ValidateAsync(contentItem);

                if (!result.Succeeded)
                {
                    throw new AppFriendlyException(HttpStatusCode.BadRequest, result.Errors);
                }

            }

            if (model.Published ?? false)
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
            //填充自带属性
            var selfPart = typeDefinition.Parts.FirstOrDefault(x => x.Name == model.ContentType);

            #region Update BaseInfo
            if (model.ContentItemId != null)
            {
                contentItem.ContentItemId = model.ContentItemId;
            }
            if (model.ContentItemVersionId != null)
            {
                contentItem.ContentItemId = model.ContentItemVersionId;
            }
            if (model.DisplayText != null)
            {
                contentItem.DisplayText = model.DisplayText;
            }
            if (model.Published.HasValue)
            {
                contentItem.Published = model.Published.Value;
            }
            if (model.Latest.HasValue)
            {
                contentItem.Latest = model.Latest.Value;
            }

            #endregion
            if (selfPart != null)
            {
                foreach (var item in selfPart.PartDefinition.Fields)
                {
                    var fieldPath = ContentTypeManagementAppService.GetFiledValuePath(item.FieldDefinition.Name);
                    if (fieldPath != null)
                    {
                        contentItem.Content[contentItem.ContentType][item.Name][fieldPath] = model[item.Name.ToCamelCase()].ToString();
                    }
                }
            }

            foreach (var part in typeDefinition.Parts.Where(x => x.Name != contentItem.ContentType))
            {
                foreach (var item in part.PartDefinition.Fields)
                {
                    var fieldPath = ContentTypeManagementAppService.GetFiledValuePath(item.FieldDefinition.Name);
                    if (fieldPath != null)
                    {
                        contentItem.Content[part.Name][item.Name][fieldPath] =
                                jObject[part.Name.ToCamelCase()][item.Name.ToCamelCase()];
                    }

                }
            }

            return contentItem;
        }
    }
}

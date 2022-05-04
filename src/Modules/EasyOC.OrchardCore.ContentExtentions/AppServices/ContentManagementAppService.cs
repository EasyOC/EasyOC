using EasyOC.Core.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.Contents;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos
{
    [EOCAuthorization("AccessContentApi")]
    public class ContentManagementAppService : AppServiceBase
    {
        private static readonly JsonMergeSettings UpdateJsonMergeSettings =
            new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Replace };
        private readonly IEnumerable<IContentHandler> _contentHandlers;
        private readonly IEnumerable<IContentDisplayHandler> _handlers;

        public ContentManagementAppService(IEnumerable<IContentHandler> contentHandlers, IEnumerable<IContentDisplayHandler> handlers)
        {
            _contentHandlers = contentHandlers;
            _handlers = handlers;
        }



        public async Task<bool> DeleteAsync(string contentItemId)
        {
            var contentItem = await ContentManager.GetAsync(contentItemId, VersionOptions.Latest);

            if (contentItem == null)
            {
                throw new AppFriendlyException(HttpStatusCode.NotFound);
            }

            if (!await AuthorizationService.AuthorizeAsync(HttpUser, CommonPermissions.DeleteContent, contentItem))
            {
                throw new AppFriendlyException(HttpStatusCode.Unauthorized);
            }

            await ContentManager.RemoveAsync(contentItem);

            return true;
        }

        public async Task<string> PostContent([FromBody] ContentModel model, [FromQuery] bool draft = false)
        {
            // It is really important to keep the proper method calls order with the ContentManager
            // so that all event handlers gets triggered in the right sequence.
            if (model.ContentType.IsNullOrWhiteSpace())
            {
                throw new AppFriendlyException("invalid property: contentType", StatusCodes.Status400BadRequest);
            }

            var typeDef = ContentDefinitionManager.GetTypeDefinition(model.ContentType);
            if (typeDef == null)
            {
                throw new AppFriendlyException($"invalid property: contentType ,'{model.ContentType}' is not found "
                    , StatusCodes.Status400BadRequest);
            }

            var contentItem = await ContentManager.GetAsync(model.ContentItemId, VersionOptions.DraftRequired);
            if (contentItem == null)
            {
                if (!await AuthorizationService.AuthorizeAsync(HttpUser, CommonPermissions.PublishContent))
                {
                    throw new AppFriendlyException(HttpStatusCode.Unauthorized);
                }

                var newContentItem = await ContentManager.NewAsync(model.ContentType);
                newContentItem.Merge(MergeFromSimpleData(newContentItem, model, typeDef));

                var result = await ContentManager.UpdateValidateAndCreateAsync(newContentItem, VersionOptions.Draft);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        await Notifier.ErrorAsync(H[error.ErrorMessage, error.MemberNames]);
                    }
                    throw new AppFriendlyException(HttpStatusCode.BadRequest, result.Errors);
                }
              
                contentItem = newContentItem;
            }
            else
            {
                if (!await AuthorizationService.AuthorizeAsync(HttpUser, CommonPermissions.EditContent, contentItem))
                {
                    throw new AppFriendlyException(HttpStatusCode.Unauthorized, "当前用户没有权限更新此内容");
                }

                contentItem.Merge(MergeFromSimpleData(contentItem, model, typeDef), UpdateJsonMergeSettings);

                await ContentManager.UpdateAsync(contentItem);
                var result = await ContentManager.ValidateAsync(contentItem);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        await Notifier.ErrorAsync(H[error.ErrorMessage, error.MemberNames]);
                    }
                    throw new AppFriendlyException(HttpStatusCode.BadRequest, result.Errors);
                }

            }

            if (!draft)
            {
                await ContentManager.PublishAsync(contentItem);
            }
            else
            {
                await ContentManager.SaveDraftAsync(contentItem);
            }

            return contentItem.ContentItemId;
        }

        public static ContentItem MergeFromSimpleData(ContentItem contentItem, ContentModel model,
            ContentTypeDefinition typeDefinition)
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
                    var valuePath = ContentTypeManagementAppService.GetFiledValuePath(item.FieldDefinition.Name);
                    if (valuePath == null || !jObject.ContainsKey(item.Name.ToCamelCase()))
                    {
                        continue;
                    }

                    if (item.FieldDefinition.Name is "ContentPickerField")
                    {
                        var valueToken = jObject.SelectToken($"{item.Name.ToCamelCase()}.firstValue");
                        #region FirstValue
                        if (valueToken != null)
                        {
                            var token = new JObject { [valuePath] = new JArray(new string[] { valueToken.Value<string>() }) };
                            contentItem.Content[contentItem.ContentType][item.Name] = token;
                        }
                        #endregion
                        else
                        {
                            valueToken = jObject.SelectToken($"{item.Name.ToCamelCase()}.{valuePath.ToCamelCase()}");
                            var token = new JObject { [valuePath] = valueToken };
                            if (token.Type == JTokenType.Array)
                            {
                                contentItem.Content[contentItem.ContentType][item.Name] = token;
                            }
                        }

                    }
                    //else if (item.FieldDefinition.Name is "UserPickerField")
                    //{
                    //    var token = new JObject { [valuePath] = jObject.SelectToken($"{item.Name.ToCamelCase()}.{valuePath.ToCamelCase()}") };
                    //    if (token.Type == JTokenType.Array)
                    //    {
                    //        contentItem.Content[contentItem.ContentType][item.Name] = token;
                    //    }
                    //}
                    else
                    {
                        var token = new JObject { [valuePath] = jObject[(item.Name.ToCamelCase())] };
                        contentItem.Content[contentItem.ContentType][item.Name] = token;
                    }
                }
            }

            foreach (var part in typeDefinition.Parts.Where(x => x.Name != contentItem.ContentType))
            {
                JToken partData;
                if (!jObject.ContainsKey(part.Name.ToCamelCase()) ||
                    (partData = jObject[part.Name.ToCamelCase()]) == null)
                {
                    continue;
                }

                var partObj = new JObject(partData);
                foreach (var item in part.PartDefinition.Fields)
                {
                    var valuePath = ContentTypeManagementAppService.GetFiledValuePath(item.FieldDefinition.Name);
                    if (valuePath == null || !partObj.ContainsKey(item.Name.ToCamelCase()))
                    {
                        continue;
                    }

                    if (item.FieldDefinition.Name is "ContentPickerField" or "UserPickerField")
                    {
                        contentItem.Content[contentItem.ContentType][item.Name][valuePath] =
                            partObj.SelectToken($"{item.Name.ToCamelCase()}.{valuePath.ToCamelCase()}");
                    }
                    else
                    {
                        var token = new JObject { [valuePath] = partObj[item.Name.ToCamelCase()] };
                        contentItem.Content[part.Name][item.Name][valuePath] = token;
                    }
                }
            }

            return contentItem;
        }

    }
}

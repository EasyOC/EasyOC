using EasyOC.Core.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.Contents;
using OrchardCore.DisplayManagement.Notify;
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
            new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Replace
            };

        private readonly IEnumerable<IContentHandler> _contentHandlers;
        private readonly IEnumerable<IContentDisplayHandler> _handlers;

        public ContentManagementAppService(IEnumerable<IContentHandler> contentHandlers,
            IEnumerable<IContentDisplayHandler> handlers)
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

        public async Task<object> PostContent([FromBody] ContentModel model, [FromQuery] bool draft = false)
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
                await Notifier.SuccessAsync(H["发布成功！"]);
            }
            else
            {
                await ContentManager.SaveDraftAsync(contentItem);
                await Notifier.SuccessAsync(H["已保存草稿"]);
            }

            return new
            {
                contentItem.Id,
                contentItem.Latest,
                contentItem.Published,
                contentItem.ContentItemId,
                contentItem.ContentItemVersionId,
            };
        }


        public async Task ImportAsync([FromBody] ImportContentInput model)
        {
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


            if (!await AuthorizationService.AuthorizeAsync(HttpUser, CommonPermissions.PublishContent))
            {
                throw new AppFriendlyException(HttpStatusCode.Unauthorized);
            }


            var ls = model.InputList.Select((m) =>
            {
                var contentItem = ContentManager.NewAsync(model.ContentType).GetAwaiter().GetResult();
                //var newContentItem = contentItem as ContentItem;
                m[nameof(ContentModel.ContentType)] = model.ContentType;
                return MergeFromSimpleData(contentItem, m, typeDef);
            });

            await ContentManager.ImportAsync(ls);
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
            // if (model.ContentItemVersionId != null)
            // {
            //     contentItem.ContentItemVersionId = model.ContentItemVersionId;
            // }

            if (model.DisplayText != null)
            {
                contentItem.DisplayText = model.DisplayText;
            }
            #endregion

            //填充自带属性
            // var selfPart = typeDefinition.Parts.FirstOrDefault(x => x.Name == model.ContentType);
            foreach (var partDefinition in typeDefinition.Parts)
            {
                FillPartData(contentItem, partDefinition, jObject);
            }

            return contentItem;
        }


        private static void FillPartData(ContentItem contentItem, ContentTypePartDefinition partDefinition, JObject inputValue)
        {
            var partName = partDefinition.PartDefinition.Name;

            foreach (var item in partDefinition.PartDefinition.Fields)
            {
                var valuePath = item.FieldDefinition.GetFiledValuePath();
                if (valuePath == null)
                {
                    continue;
                }
                var inputKeyPrefix = contentItem.ContentType == partName ? string.Empty : $"{partName.ToCamelCase()}.";
                var inputKey = $"{inputKeyPrefix}{item.Name.ToCamelCase()}";
                var filedValue = inputValue.SelectToken(inputKey);
                if (filedValue is null)
                {
                    return;
                }

                if (item.FieldDefinition.Name is nameof(ContentPickerField) or nameof(UserPickerField) or "MediaField")
                {

                    var firstValue = filedValue.SelectToken("firstValue");
                    if (firstValue?.Value<string>() != null)
                    {
                        contentItem.Content[partName][item.Name] = new JObject
                        {
                            [valuePath] = new JArray(new object[]
                            {
                                firstValue.Value<string>()
                            })
                        };
                    }
                    else
                    {
                        if (filedValue.Type == JTokenType.Array)
                        {
                            contentItem.Content[partName][item.Name] = new JObject
                            {
                                [valuePath] = new JArray(new object[]
                                {
                                    filedValue.Value<string>()
                                })
                            };
                        }
                        else
                        {
                            contentItem.Content[partName][item.Name] = new JObject
                            {
                                [valuePath] = filedValue
                            };
                        }
                    }
                }
                else
                {
                    contentItem.Content[partName][item.Name] = new JObject
                    {
                        [valuePath] = filedValue
                    };
                }
            }
        }
    }
}

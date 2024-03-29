﻿using EasyOC.Core.Application;
using EasyOC.Core.DtoModels;
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

namespace EasyOC.ContentExtensions.AppServices.Dtos
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




        public async Task<ContentItemDto> GetAsync(string contentItemId)
        {
            if (!await AuthorizationService.AuthorizeAsync(User, Permissions.AccessContentApi))
            {
                throw new AppFriendlyException(HttpStatusCode.Unauthorized);
            }

            var contentItem = await ContentManager.GetAsync(contentItemId);

            if (contentItem == null)
            {
                throw new AppFriendlyException(HttpStatusCode.NotFound);
            }

            if (!await AuthorizationService.AuthorizeAsync(User, CommonPermissions.ViewContent, contentItem))
            {
                await Notifier.ErrorAsync(H["Permission denied."]);
                throw new AppFriendlyException(HttpStatusCode.Unauthorized);
            }

            return contentItem.ToDto();
        }


        [HttpDelete]
        public async Task<JArray> BatchDeleteAsync([FromQuery] string ids)
        {
            var results = new List<JObject>();
            if (ids == null)
            {
                return JArray.FromObject(results);
            }
            var idArray = ids.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (idArray.Any())
            {
                var checkedContentItems = await ContentManager.GetAsync(idArray);
                foreach (var contentItem in checkedContentItems)
                {
                    if (!await AuthorizationService.AuthorizeAsync(User, CommonPermissions.DeleteContent, contentItem))
                    {
                        await Notifier.ErrorAsync(H["Couldn't remove the content. Permission denied."]);
                        await YesSession.CancelAsync();
                    }
                    await ContentManager.RemoveAsync(contentItem);
                    results.Add(JObject.FromObject(new
                    {
                        contentItemId = contentItem.ContentItemId,
                        displayText = contentItem.DisplayText
                    }));
                }
            }

            return JArray.FromObject(results);
        }

        public async Task<JObject> DeleteAsync(string contentItemId)
        {
            var contentItem = await ContentManager.GetAsync(contentItemId, VersionOptions.Latest);
            if (contentItem == null)
            {
                throw new AppFriendlyException(HttpStatusCode.NotFound);
            }

            if (!await AuthorizationService.AuthorizeAsync(User, CommonPermissions.DeleteContent, contentItem))
            {
                await Notifier.ErrorAsync(H["Couldn't remove the content. Permission denied."]);
            }

            var typeDefinition = ContentDefinitionManager.GetTypeDefinition(contentItem.ContentType);

            await ContentManager.RemoveAsync(contentItem);

            await Notifier.SuccessAsync(String.IsNullOrWhiteSpace(contentItem.DisplayText)
                ? H["That content has been removed."]
                : H["That {0} has been removed.", contentItem.DisplayText]);

            return JObject.FromObject(new
            {
                contentItemId = contentItem.ContentItemId,
                contentItemVersionId = contentItem.ContentItemVersionId,
                displayText = contentItem.DisplayText
            });
        }

        public async Task<object> PostContent([FromBody] ContentModel model, [FromQuery] bool draft = false, [FromQuery] bool merge = true)
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
                if (!await AuthorizationService.AuthorizeAsync(User, CommonPermissions.PublishContent))
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
                if (!await AuthorizationService.AuthorizeAsync(User, CommonPermissions.EditContent, contentItem))
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

            return JObject.FromObject(new
            {
                contentItem.Id,
                contentItem.Latest,
                contentItem.Published,
                contentItem.ContentItemId,
                contentItem.ContentItemVersionId,
            });
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


            if (!await AuthorizationService.AuthorizeAsync(User, CommonPermissions.PublishContent))
            {
                throw new AppFriendlyException(HttpStatusCode.Unauthorized);
            }


            var ls = model.InputList.Select((contentModel) =>
            {
                var contentItem = ContentManager.NewAsync(model.ContentType).GetAwaiter().GetResult();
                //var newContentItem = contentItem as ContentItem;
                contentModel[nameof(ContentModel.ContentType)] = model.ContentType;
                return MergeFromSimpleData(contentItem, contentModel, typeDef);
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

                //如果未指定布尔值，则直接指定为False ，因为 MVC 版本尚未处理此问题
                if (item.FieldDefinition.Name == nameof(BooleanField))
                {
                    contentItem.Content[partName][item.Name] = new JObject
                    {
                        [valuePath] = filedValue ?? false
                    };
                    continue;
                }
                // 避免未指定的字段被覆盖
                if (filedValue is null)
                {
                    continue;
                }

                if (item.FieldDefinition.Name is nameof(ContentPickerField) or nameof(UserPickerField) or "MediaField")
                {

                    var firstValue = filedValue.SelectToken("firstValue");

                    if (firstValue != null)
                    {
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

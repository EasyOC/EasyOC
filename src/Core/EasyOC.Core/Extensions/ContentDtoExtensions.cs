using EasyOC.Core.DtoModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyOC
{
    public static class ContentDtoExtensions
    {
        private static JsonSerializerSettings Formatter = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessExtensionDataNames = true
                }
            }
        };

        private static JsonSerializer JsonSerializer = JsonSerializer.Create(
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        ProcessExtensionDataNames = true
                    }
                }
            });

        private static readonly JsonMergeSettings ReplaceJsonMergeSettings = new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Replace,
            // TODO don't need this is we are pascal casing names first.
            //PropertyNameComparison = StringComparison.OrdinalIgnoreCase
        };

        public static TDto ToDto<TDto>(this ContentDefinition content)
               where TDto : ContentDefinitionDto
        {
            var serialized = JObject.FromObject(content);
            var deserialized = serialized.ToObject(typeof(TDto)) as TDto;
            //deserialized.AdditionalPropertiesToCamelCase();
            return deserialized;
        }

        public static ContentItemDto ToDto(this ContentItem content)
        {
            return content.ToDto<ContentItemDto>();
        }

        public static TDto ToDto<TDto>(this ContentElement content)
            where TDto : ContentElementDto
        {
            var serialized = JObject.FromObject(content);
            var deserialized = serialized.ToObject(typeof(TDto)) as TDto;
            //deserialized.AdditionalPropertiesToCamelCase();
            return deserialized;
        }

        /// <summary>
        /// Returns a new instance of a content item from a dto
        /// For use when working with items in bags.
        /// Use with care: Will create a new instance if used and saved on a primary content item.
        /// </summary>
        public static ContentItem ToContentItem(this ContentItemDto content)
        {
            var serialized = JObject.FromObject(content);
            var deserialized = serialized.ToObject<ContentItem>();
            return deserialized;
        }

        //TODO a from ?
        public static TDto ToDto<TDto>(this ContentElementDto contentDto)
            where TDto : ContentElementDto
        {
            var serialized = JObject.FromObject(contentDto);
            var deserialized = serialized.ToObject(typeof(TDto)) as TDto;
            //deserialized.AdditionalPropertiesToCamelCase();
            return deserialized;
        }

        public static IList<TDto> OfDtoType<TDto>(this IList<ContentItemDto> contentDtos)
            where TDto : ContentElementDto
        {
            return contentDtos.OfDtoType<TDto>("ItemDto");
        }

        public static IList<TDto> OfDtoType<TDto>(this IList<ContentItemDto> contentDtos, string schemaItemExtension)
            where TDto : ContentElementDto
        {
            var originalDtos = contentDtos.ToArray();
            var results = new List<TDto>();
            int i = 0;
            foreach (var contentDto in originalDtos)
            {
                if (contentDto is TDto)
                {
                    results.Add(contentDto as TDto);
                }
                else
                {
                    if (contentDto.ContentType == typeof(TDto).Name.Replace(schemaItemExtension, ""))
                    {
                        var typedContentDto = contentDto.ToDto<TDto>();
                        contentDtos[i] = typedContentDto as ContentItemDto;
                        results.Add(typedContentDto);
                    }
                }
                i++;
            }

            return results;
        }

        // TODO we don't have a merge for elements.
        public static ContentItem FromDto<TDto>(this ContentItem contentItem, TDto dto, JsonMergeSettings jsonMergeSettings = null)
            where TDto : ContentItemDto
        {
            if (dto == null)
            {
                throw new ArgumentNullException();
            }

            var jObject = JObject.FromObject(dto);
            if (jsonMergeSettings == null)
            {
                return contentItem.Merge(jObject, ReplaceJsonMergeSettings);
            }
            else
            {
                return contentItem.Merge(jObject, jsonMergeSettings);
            }
        }

        //internal static void AdditionalPropertiesToCamelCase(this ContentElementDto contentElementDto)
        //{
        //    if (contentElementDto.Parts != null && contentElementDto.Parts.Any())
        //    {
        //        var additionalProperties = new Dictionary<string, object>();
        //        foreach (var key in contentElementDto.Parts.Keys)
        //        {
        //            var value = contentElementDto.Parts[key];
        //            contentElementDto.Parts.Remove(key);
        //            var camelCaseKey = char.ToLowerInvariant(key[0]) + key.Substring(1);
        //            additionalProperties.Add(camelCaseKey, value);
        //        }
        //        contentElementDto.Parts = additionalProperties;
        //    }
        //}
    }
}




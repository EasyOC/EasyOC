using EasyOC.OrchardCore.DynamicTypeIndex.Models;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;

namespace EasyOC
{
    public static class ContentItemExtentions
    {
        public static Dictionary<string, object> ToDictModel(this ContentItem doc, DynamicIndexConfigModel config, bool useUnderline = true)
        {
            var docContent = doc.Content as JObject;
            var dictModel = new Dictionary<string, object>();
            dictModel.Add("Id", doc.Id);
            dictModel.Add("DocumentId", doc.Id);
            dictModel.Add("ContentItemVersionId", doc.ContentItemVersionId);
            dictModel.Add("ContentItemId", doc.ContentItemId);
            dictModel.Add("Published", doc.Published);
            dictModel.Add("Latest", doc.Latest);
            dictModel.Add("DisplayText", doc.DisplayText);

            foreach (var fConfig in config.Fields)
            {
                var valueKey = fConfig.Name;
                if (!useUnderline)
                {
                    valueKey = valueKey.Replace("_", String.Empty);
                }
                JToken valueToken;
                if (!fConfig.ContentFieldOption.ValuePath.IsNullOrWhiteSpace())
                {
                    valueToken = docContent.SelectToken(fConfig.ContentFieldOption.ValueFullPath);
                    if (valueToken != null)
                    {
                        try
                        {
                            dictModel.Add(valueKey,
                                valueToken.GetOCFieldValue(fConfig.ContentFieldOption));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                }
                else
                {
                    valueToken = docContent.SelectToken(fConfig.Name);
                    if (valueToken != null)
                    {
                        object value = null;
                        switch (valueToken.Type)
                        {
                            case JTokenType.Integer:
                                value = valueToken.Value<int?>();
                                break;
                            case JTokenType.Float:
                                value = valueToken.Value<float?>();
                                break;
                            case JTokenType.String:
                                value = valueToken.Value<string>();
                                break;
                            case JTokenType.Boolean:
                                value = valueToken.Value<bool?>();
                                break;
                            case JTokenType.Date:
                                value = valueToken.Value<DateTime?>();
                                break;
                            case JTokenType.TimeSpan:
                                value = valueToken.Value<TimeSpan?>();
                                break;
                            default:
                                break;
                        }
                        dictModel.Add(valueKey, value);
                    }
                }
            }
            return dictModel;
        }


        public static List<Dictionary<string, object>> ToDictModel(this IEnumerable<ContentItem> docs, DynamicIndexConfigModel config, bool useUnderline = true)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (var doc in docs)
            {
                list.Add(doc.ToDictModel(config, useUnderline));
            }
            return list;
        }
    }
}

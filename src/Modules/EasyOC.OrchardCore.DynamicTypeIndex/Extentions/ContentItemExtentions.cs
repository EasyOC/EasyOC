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
            var jdoc = doc.Content as JObject;
            var dmodel = new Dictionary<string, object>();
            dmodel.Add("ContentItemId", doc.ContentItemId);
            dmodel.Add("DocumentId", doc.Id);
            dmodel.Add("Id", null);

            foreach (var fConfig in config.Fields)
            {
                var valueKey = fConfig.Name;
                if (!useUnderline)
                {
                    valueKey = valueKey.Replace("_", String.Empty);
                }
                JToken valueToken;
                if (!fConfig.ContentFieldOption.ValueFullPath.IsNullOrWhiteSpace())
                {
                    valueToken = jdoc.SelectToken(fConfig.ContentFieldOption.ValueFullPath);
                    if (valueToken != null)
                    {
                        dmodel.Add(valueKey,
                                valueToken.GetOCFieldValue(fConfig.ContentFieldOption.FieldName));
                    }
                }
                else
                {
                    valueToken = jdoc.SelectToken(fConfig.Name);
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
                        dmodel.Add(valueKey, value);
                    }
                }
            }
            return dmodel;
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

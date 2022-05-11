
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace OrchardCore.ContentManagement
{
    public static class ContentExtensions
    {
        public const string WeldedPartSettingsName = "@WeldedPartSettings";

        //
        // 摘要:
        //     These settings instruct merge to replace current value, even for null values.
        private static readonly JsonMergeSettings JsonMergeSettings = new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Replace,
            MergeNullValueHandling = MergeNullValueHandling.Merge
        };
 

        ////
        //// 摘要:
        ////     Gets whether a content element has a specific element attached.
        ////
        //// 参数:
        ////   contentElement:
        ////     The OrchardCore.ContentManagement.ContentElement.
        ////
        //// 类型参数:
        ////   TElement:
        ////     The expected type of the content element.
        //public static bool Has(this ContentElement contentElement,string name)
        //{
        //    return contentElement.Has(name);
        //}

     

       
        //
        // 摘要:
        //     Gets a content element by its name or create a new one.
        //
        // 参数:
        //   contentElement:
        //     The OrchardCore.ContentManagement.ContentElement.
        //
        //   name:
        //     The name of the content element.
        //
        // 类型参数:
        //   TElement:
        //     The expected type of the content element.
        //
        // 返回结果:
        //     The content element instance or a new one if it doesn't exist.
        public static TElement GetOrCreate<TElement>(this ContentElement contentElement, string name) where TElement : ContentElement, new()
        {
            TElement val = contentElement.Get<TElement>(name);
            if (val == null)
            {
                TElement val2 = new TElement();
                val2.ContentItem = contentElement.ContentItem;
                contentElement.Data[name] = val2.Data;
                contentElement.Elements[name] = val2;
                return val2;
            }

            return val;
        }

        //
        // 摘要:
        //     Adds a content element by name.
        //
        // 参数:
        //   contentElement:
        //     The OrchardCore.ContentManagement.ContentElement.
        //
        //   name:
        //     The name of the content element.
        //
        //   element:
        //     The element to add to the OrchardCore.ContentManagement.ContentItem.
        //
        // 返回结果:
        //     The current OrchardCore.ContentManagement.ContentItem instance.
        public static ContentElement Weld(this ContentElement contentElement, string name, ContentElement element)
        {
            if (!contentElement.Data.ContainsKey(name))
            {
                element.Data = JObject.FromObject(element);
                element.ContentItem = contentElement.ContentItem;
                contentElement.Data[name] = element.Data;
                contentElement.Elements[name] = element;
            }

            return contentElement;
        }

        //
        // 摘要:
        //     Welds a new part to the content item. If a part of the same type is already welded
        //     nothing is done. This part can be not defined in Content Definitions.
        //
        // 类型参数:
        //   TElement:
        //     The type of the part to be welded.
        public static ContentElement Weld<TElement>(this ContentElement contentElement, object settings = null) where TElement : ContentElement, new()
        {
            string name = typeof(TElement).Name;
            if (!(contentElement.Data[name] is JObject))
            {
                TElement element = new TElement();
                contentElement.Weld(name, element);
            }

            if (!contentElement.Data.TryGetValue("@WeldedPartSettings", out JToken value))
            {
                value = (contentElement.Data["@WeldedPartSettings"] = new JObject());
            }

            ((JObject)value)[name] = ((settings == null) ? new JObject() : JObject.FromObject(settings, ContentBuilderSettings.IgnoreDefaultValuesSerializer));
            return contentElement;
        }

        //
        // 摘要:
        //     Updates the content element with the specified name.
        //
        // 参数:
        //   contentElement:
        //     The OrchardCore.ContentManagement.ContentElement.
        //
        //   name:
        //     The name of the element to update.
        //
        //   element:
        //     The content element instance to update.
        //
        // 返回结果:
        //     The current OrchardCore.ContentManagement.ContentItem instance.
        public static ContentElement Apply(this ContentElement contentElement, string name, ContentElement element)
        {
            JObject jObject = contentElement.Data[name] as JObject;
            if (jObject != null)
            {
                jObject.Merge(JObject.FromObject(element), JsonMergeSettings);
            }
            else
            {
                jObject = JObject.FromObject(element);
                contentElement.Data[name] = jObject;
            }

            element.Data = jObject;
            element.ContentItem = contentElement.ContentItem;
            contentElement.Elements[name] = element;
            if (element is ContentField)
            {
                contentElement.ContentItem?.Elements.Clear();
            }

            return contentElement;
        }

        //
        // 摘要:
        //     Updates the whole content.
        //
        // 参数:
        //   contentElement:
        //     The OrchardCore.ContentManagement.ContentElement.
        //
        //   element:
        //     The content element instance to update.
        //
        // 返回结果:
        //     The current OrchardCore.ContentManagement.ContentItem instance.
        public static ContentElement Apply(this ContentElement contentElement, ContentElement element)
        {
            if (contentElement.Data != null)
            {
                contentElement.Data.Merge(JObject.FromObject(element.Data), JsonMergeSettings);
            }
            else
            {
                contentElement.Data = JObject.FromObject(element.Data, ContentBuilderSettings.IgnoreDefaultValuesSerializer);
            }

            contentElement.Elements.Clear();
            return contentElement;
        }

        //
        // 摘要:
        //     Modifies a new or existing content element by name.
        //
        // 参数:
        //   contentElement:
        //     The OrchardCore.ContentManagement.ContentElement.
        //
        //   name:
        //     The name of the content element to update.
        //
        //   action:
        //     An action to apply on the content element.
        //
        // 类型参数:
        //   TElement:
        //     The type of the part to be altered.
        //
        // 返回结果:
        //     The current OrchardCore.ContentManagement.ContentElement instance.
        public static ContentElement Alter<TElement>(this ContentElement contentElement, string name, Action<TElement> action) where TElement : ContentElement, new()
        {
            TElement orCreate = contentElement.GetOrCreate<TElement>(name);
            action(orCreate);
            contentElement.Apply(name, orCreate);
            return contentElement;
        }

        //
        // 摘要:
        //     Updates the content item data.
        //
        // 返回结果:
        //     The current OrchardCore.ContentManagement.ContentPart instance.
        public static ContentPart Apply(this ContentPart contentPart)
        {
            contentPart.ContentItem.Apply(contentPart.GetType().Name, contentPart);
            return contentPart;
        }

        //
        // 摘要:
        //     Whether the content element is published or not.
        //
        // 参数:
        //   content:
        //     The content to check.
        //
        // 返回结果:
        //     True if the content is published, False otherwise.
        public static bool IsPublished(this IContent content)
        {
            if (content.ContentItem != null)
            {
                return content.ContentItem.Published;
            }

            return false;
        }

        //
        // 摘要:
        //     Whether the content element has a draft or not.
        //
        // 参数:
        //   content:
        //     The content to check.
        //
        // 返回结果:
        //     True if the content has a draft, False otherwise.
        public static bool HasDraft(this IContent content)
        {
            if (content.ContentItem != null)
            {
                if (content.ContentItem.Published)
                {
                    return !content.ContentItem.Latest;
                }

                return true;
            }

            return false;
        }

        //
        // 摘要:
        //     Gets all content elements of a specific type.
        //
        // 类型参数:
        //   TElement:
        //     The expected type of the content elements.
        //
        // 返回结果:
        //     The content element instances or empty sequence if no entries exist.
        public static IEnumerable<TElement> OfType<TElement>(this ContentElement contentElement) where TElement : ContentElement
        {
            foreach (KeyValuePair<string, ContentElement> element in contentElement.Elements)
            {
                TElement val = element.Value as TElement;
                if (val != null)
                {
                    yield return val;
                }
            }
        }
    }
} 

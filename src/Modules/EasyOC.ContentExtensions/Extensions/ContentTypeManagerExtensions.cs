using EasyOC.ContentExtensions.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using System.Collections.Generic;

namespace EasyOC
{
    public static class ContentTypeManagerExtensions
    {
        public static string GetFiledValuePath(string fieldName)
        {
            string valuePath;
            switch (fieldName)
            {
                case "TextField":
                    valuePath = "Text";
                    break;
                case "BooleanField":
                case "DateField":
                case "TimeField":
                case "DateTimeField":
                case "NumericField":
                    valuePath = "Value";
                    break;
                case "ContentPickerField":
                    valuePath = "ContentItemIds";
                    break;
                case "UserPickerField":
                    valuePath = "UserIds";
                    break;
                default:
                    return null;
            }
            return valuePath;
        }

        public static string GetGraphqlValuePath(string fieldTypeName)
        {
            string valuePath;
            switch (fieldTypeName)
            {
                case "ContentPickerField":
                    valuePath = "firstValue";
                    break;
                case "UserPickerField":
                    valuePath = "firstValue";
                    break;
                default:
                    return null;
            }
            return valuePath;
        }
        public static string GetGraphqlValuePath(this ContentFieldDefinition fieldDefinition)
        {
            return GetGraphqlValuePath(fieldDefinition.Name);
        }
        public static string GetFiledValuePath(this ContentFieldDefinition fieldDefinition)
        {
            return GetFiledValuePath(fieldDefinition.Name);
        }
    }
}

using EasyOC.OrchardCore.ContentExtentions.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using System.Collections.Generic;

namespace EasyOC
{
    public static class ContentTypeManagerExtentions
    {



         

        public static string GetFiledValuePath(this ContentFieldDefinition fieldDefinition)
        {
            string valuePath;
            switch (fieldDefinition.Name)
            {
                case "TextField":
                    valuePath = "Text";
                    break;
                case "BooleanField":
                case "DateField":
                case "TimeField":
                case "DateTimefield":
                case "NumericField":
                    valuePath = "Value";
                    break;
                case "ContentPickerField":
                    valuePath = "ContentItemIds[0]";
                    break;
                case "UserPickerField":
                    valuePath = "UserIds[0]";
                    break;
                default:
                    return null;
            }
            return valuePath;
        }
    }
}

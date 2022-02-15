using EasyOC.OrchardCore.ContentExtentions.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using System.Collections.Generic;

namespace EasyOC
{
    public static class ContentManagerExtentions
    {
        //public static List<ContentFieldsMappingDto> GetAllFields(this IContentDefinitionManager contentDefinitionManager, string typeName)
        //{
        //    var typeDef = contentDefinitionManager.GetTypeDefinition(typeName);
        //    var fields = new List<ContentFieldsMappingDto>();
        //    fields.Add(new ContentFieldsMappingDto { DisplayName = "ID", FieldName = "ContentItemId", IsContentItemProperty = true });
        //    fields.Add(new ContentFieldsMappingDto { DisplayName = "版本号", FieldName = "ContentItemVersionId", IsContentItemProperty = true });
        //    fields.Add(new ContentFieldsMappingDto { DisplayName = "内容类型", FieldName = "ContentType", IsContentItemProperty = true });
        //    fields.Add(new ContentFieldsMappingDto { DisplayName = "标题", FieldName = "DisplayText", IsContentItemProperty = true });
        //    fields.Add(new ContentFieldsMappingDto { DisplayName = "最新版", FieldName = "Latest", IsContentItemProperty = true });
        //    fields.Add(new ContentFieldsMappingDto { DisplayName = "已发布", FieldName = "Published", IsContentItemProperty = true });
        //    fields.Add(new ContentFieldsMappingDto { DisplayName = "修改时间", FieldName = "ModifiedUtc", IsContentItemProperty = true });
        //    fields.Add(new ContentFieldsMappingDto { DisplayName = "发布时间", FieldName = "PublishedUtc", IsContentItemProperty = true });
        //    fields.Add(new ContentFieldsMappingDto { DisplayName = "发布时间", FieldName = "CreatedUtc", IsContentItemProperty = true });
        //    fields.Add(new ContentFieldsMappingDto { DisplayName = "归属人", FieldName = "Owner", IsContentItemProperty = true });
        //    fields.Add(new ContentFieldsMappingDto { DisplayName = "作者", FieldName = "Author", IsContentItemProperty = true });
        //    foreach (var item in fields)
        //    {
        //        item.LastValueKey = item.KeyPath = item.FieldName;
        //    }
        //    foreach (var part in typeDef.Parts)
        //    {
        //        foreach (var field in part.PartDefinition.Fields)
        //        {
        //            var lastKey = GetFiledValuePath(field.FieldDefinition.Name);
        //            fields.Add(new ContentFieldsMappingDto
        //            {
        //                DisplayName = field.DisplayName(),
        //                Description = field.Description(),
        //                FieldName = field.Name,
        //                PartName = part.Name,
        //                PartDisplayName = part.DisplayName(),
        //                KeyPath = $"{part.Name}.{field.Name}.{lastKey}",
        //                LastValueKey = lastKey,
        //                FieldSettings = field.Settings,
        //                FieldType = field.FieldDefinition.Name
        //            });
        //        }

        //    }


        //    return fields;
        //}

        //public static string GetFiledValuePath(string fieldName)
        //{
        //    string valuePath;
        //    switch (fieldName)
        //    {
        //        case "TextField":
        //            valuePath = "Text";
        //            break;
        //        case "BooleanField":
        //        case "DateField":
        //        case "TimeField":
        //        case "DateTimefield":
        //        case "NumericField":
        //            valuePath = "Value";
        //            break;
        //        case "ContentPickerField":
        //            valuePath = "ContentItemIds[0]";
        //            break;
        //        case "UserPickerField":
        //            valuePath = "UserIds[0]";
        //            break;
        //        default:
        //            return null;
        //    }
        //    return valuePath;
        //}
    }
}

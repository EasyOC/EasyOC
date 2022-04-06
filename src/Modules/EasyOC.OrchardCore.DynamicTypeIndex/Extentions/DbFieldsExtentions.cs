using EasyOC.OrchardCore.DynamicTypeIndex.Service.Dto;
using TextField = OrchardCore.ContentFields.Fields.TextField;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement.Metadata.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EasyOC.OrchardCore.DynamicTypeIndex
{
    public static class DbFieldsExtentions
    {
        public static DynamicIndexFieldItem AddDbField<T>
            (this List<DynamicIndexFieldItem> list,
                string columnName, int length = 0, bool isSystem = false,
               bool isNullable = false,
               bool isIdentity = false,
               bool isPrimaryKey = false,
               bool addToTableIndex = false
            )
        {
            var item = new DynamicIndexFieldItem
            {
                DbFieldOption = new DbFiledOption
                {
                    Name = columnName,
                    IsSystem = isSystem,
                    AddToTableIndex = addToTableIndex,
                    IsIdentity = isIdentity,
                    IsPrimaryKey = isPrimaryKey,
                    IsNullable = isNullable,
                    CsTypeName = typeof(T).FullName,
                }
            };
            if (length != 0)
            {
                item.DbFieldOption.Length = length;
            }
            list.Add(item);
            return item;
        }

        public static DynamicIndexFieldItem ToDynamicIndexField
                    (this ContentPartFieldDefinition field,
                         ContentTypePartDefinition part
                    )
        {
            var isTypeSelf = part.IsTypeSelfPart();
            var fieldPath = $"{part.Name}.{field.Name}";
            var valuePath = field.FieldDefinition.GetFiledValuePath();

            var item = new DynamicIndexFieldItem
            {
                Name = field.Name,
                ContentFieldOption = new ContentFieldOption
                {
                    IsSelfField = isTypeSelf,
                    DisplayName = field.DisplayName(),
                    FieldName = isTypeSelf ? field.Name : fieldPath,
                    FieldType = field.FieldDefinition.Name,
                    ValuePath = valuePath,
                    PartName = part.Name,
                    ValueFullPath = $"{fieldPath}.{valuePath}",
                },
                DbFieldOption = field.ToDbFiledOption(isTypeSelf)
            };

            return item;
        }


        public static DbFiledOption ToDbFiledOption(this ContentPartFieldDefinition field, bool isTypeSelf = false)
        {
            var dbOption = new DbFiledOption();
            dbOption.Name = isTypeSelf ? field.Name : $"{field.PartDefinition.Name}_{field.Name}";
            dbOption.IsNullable = true;
            dbOption.IsSystem = false;


            ///TODO: 可以考虑使用必填检查将字段设置为必填，但如果通过UI将非必填改为必填会出现空值列无法设置为不可为空
            ///暂不处理
            switch (field.FieldDefinition.Name)
            {
                case nameof(TextField):
                case nameof(LinkField):
                case nameof(HtmlField):
                    dbOption.CsTypeName = typeof(string).FullName;
                    dbOption.Length = -1;//Unlimited
                    break;
                case nameof(BooleanField):
                    dbOption.CsTypeName = typeof(bool).FullName;
                    break;
                case nameof(NumericField):
                    dbOption.CsTypeName = typeof(decimal).FullName;
                    break;
                case nameof(DateTimeField):
                case nameof(DateField):
                case nameof(TimeField):
                    dbOption.CsTypeName = typeof(DateTime).FullName;
                    break;
                case nameof(ContentPickerField):
                case nameof(UserPickerField):
                    dbOption.CsTypeName = typeof(string).FullName;
                    dbOption.Length = 26;//Unlimited
                    break;
                default:
                    break;
            }
            return dbOption;
        }

        public static bool IsTypeSelfPart(this ContentTypePartDefinition part)
        {
            return part.Name == part.ContentTypeDefinition.Name;
        }
    }
}

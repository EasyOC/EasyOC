using EasyOC.DynamicTypeIndex.Models;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement.Metadata.Models;
using System;
using System.Collections.Generic;
using TextField=OrchardCore.ContentFields.Fields.TextField;

namespace EasyOC.DynamicTypeIndex
{
    public static class DbFieldsExtensions
    {
        public static DynamicIndexFieldItem AddDbField<T>
        (this List<DynamicIndexFieldItem> list,
            string columnName, int length = 0, bool isSystem = false,
            bool isNullable = false,
            bool isIdentity = false,
            bool isPrimaryKey = false,
            bool isIndex = false
        )
        {
            var item = new DynamicIndexFieldItem
            {

                Name = columnName,
                IsSystem = isSystem,
                IsDefaultIndex = isIndex,
                IsIdentity = isIdentity,
                IsPrimaryKey = isPrimaryKey,
                IsNullable = isNullable,
                CsTypeName = typeof(T).FullName,

            };
            if (length != 0)
            {
                item.Length = length;
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


            var item = new DynamicIndexFieldItem()
            {
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

            };
            item.FillDbFiledOption(field, isTypeSelf);

            return item;
        }
        public static string GetCsFieldName(this DbFiledOption dbOption)
        {
            return dbOption.Name.Replace("_", string.Empty);
        }

        public static void FillDbFiledOption(this DbFiledOption dbOption, ContentPartFieldDefinition field, bool isTypeSelf = false)
        {
            dbOption.Name = isTypeSelf ? field.Name : $"{field.PartDefinition.Name}_{field.Name}";
            dbOption.IsNullable = true;
            dbOption.IsSystem = false;


            //TODO: 可以考虑使用必填检查将字段设置为必填，但如果通过UI将非必填改为必填会出现空值列无法设置为不可为空
            //暂不处理
            switch (field.FieldDefinition.Name)
            {
                case nameof(TextField):
                case nameof(LinkField):
                case nameof(HtmlField):
                case nameof(TimeField):
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
            //return dbOption;
        }

        public static bool IsTypeSelfPart(this ContentTypePartDefinition part)
        {
            return part.Name == part.ContentTypeDefinition.Name;
        }
    }
}

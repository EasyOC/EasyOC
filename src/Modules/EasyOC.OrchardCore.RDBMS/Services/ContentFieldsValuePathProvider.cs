using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyOC.OrchardCore.RDBMS.Services
{
    public class ContentFieldsValuePathProvider : IContentFieldsValuePathProvider
    {
        public static Dictionary<string, FieldTypeValuePathDescriptor> ContentFieldValuePathMappings = new Dictionary<string, FieldTypeValuePathDescriptor>
        {
            {
                nameof(BooleanField),
                new FieldTypeValuePathDescriptor
                {
                    FieldName=nameof(BooleanField),
                    Description = "Boolean field",
                    FieldTypes = new[] { typeof(bool) },
                    UnderlyingType = typeof(BooleanField),
                    FieldAccessor = field => (bool)field.Content.Value
                }
            },
            {
                nameof(DateField),
                new FieldTypeValuePathDescriptor
                {
                    FieldName=nameof(DateField),
                    Description = "Date field",
                    FieldTypes = new[] { typeof(DateTime?),typeof(DateTime) },
                    UnderlyingType = typeof(DateField),
                    FieldAccessor = field => (DateTime?)field.Content.Value
                }
            },
            {
                nameof(DateTimeField),
                new FieldTypeValuePathDescriptor
                {
                    Description = "Date & time field",
                    FieldName=nameof(DateTimeField),
                    FieldTypes = new[] { typeof(DateTime?),typeof(DateTime) },
                    UnderlyingType = typeof(DateTimeField),
                    FieldAccessor = field => (DateTime?)field.Content.Value
                }
            },
            {
                nameof(NumericField),
                new FieldTypeValuePathDescriptor
                {
                    Description = "Numeric field",
                    FieldName=nameof(NumericField),
                    FieldTypes = new[] {
                        typeof(decimal),typeof(decimal?),
                        typeof(int),typeof(int?),typeof(short),typeof(short?),
                        typeof(long), typeof(long?),typeof(double?),typeof(double),
                    },
                    UnderlyingType = typeof(NumericField),
                    FieldAccessor = field => (decimal?)field.Content.Value
                }
            },
            {
                nameof(TextField),
                new FieldTypeValuePathDescriptor
                {
                    Description = "Text field",
                    FieldName=nameof(TextField),
                    FieldTypes = new[] { typeof(string) } ,
                    UnderlyingType = typeof(TextField),
                    ValuePath="Text",
                    FieldAccessor = field => field.Content.Text
                }
            },
            {
                nameof(TimeField),
                new FieldTypeValuePathDescriptor
                {
                    FieldName=nameof(TimeField),
                    Description = "Time field",
                    FieldTypes = new[]{ typeof(TimeSpan?) },
                    UnderlyingType = typeof(TimeField),
                    FieldAccessor = field => (TimeSpan?)field.Content.Value
                }
            },
            {
                nameof(MultiTextField),
                new FieldTypeValuePathDescriptor
                {
                    Description = "Multi text field",
                    FieldTypes =new []{  typeof(string) },
                    UnderlyingType = typeof(MultiTextField),
                    ValuePath=nameof(MultiTextField.Values),
                    FieldAccessor = field => field.Content.Values
                }
            }
        };

        public IEnumerable<OrchardCoreBaseField> GetAllOrchardCoreBaseFields()
        {
            return ContentFieldValuePathMappings.Select(x => new OrchardCoreBaseField
            {
                Name = x.Key,
                Description = x.Value.Description,
                ValuePath = x.Value.ValuePath,
                DefaultMappToCsType = x.Value.FieldTypes.Select(t => t.FullName)
            });
        }
        public FieldTypeValuePathDescriptor GetField(ContentPartFieldDefinition field)
        {
            if (!ContentFieldValuePathMappings.ContainsKey(field.FieldDefinition.Name)) return null;
            var fieldDescriptor = ContentFieldValuePathMappings[field.FieldDefinition.Name];
            return fieldDescriptor;
        }

        public FieldTypeValuePathDescriptor GetField<T>(T fieldType)
        {
            return GetField(fieldType.GetType());
        }
        public FieldTypeValuePathDescriptor GetField(Type fieldType)
        {
            return ContentFieldValuePathMappings.Values.FirstOrDefault(t => t.FieldTypes.Any(x => x == fieldType));
        }

    }

    public class OrchardCoreBaseField
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ValuePath { get; set; }
        public IEnumerable<string> DefaultMappToCsType { get; set; }
    }

    public class FieldTypeValuePathDescriptor
    {
        public string Description { get; set; }
        public string FieldName { get; set; }
        public Type[] FieldTypes { get; set; }
        public Type UnderlyingType { get; set; }
        public Func<ContentElement, object> FieldAccessor { get; set; }
        public string ValuePath { get; set; } = "Value";
    }


}




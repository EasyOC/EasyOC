using OrchardCore.ContentManagement.Metadata.Models;
using System;
using System.Collections.Generic;

namespace EasyOC.RDBMS.Services
{
    public interface IContentFieldsValuePathProvider
    {
        IEnumerable<OrchardCoreBaseField> GetAllOrchardCoreBaseFields();
        FieldTypeValuePathDescriptor GetField(ContentPartFieldDefinition field);
        FieldTypeValuePathDescriptor GetField<T>(T fieldType);
        FieldTypeValuePathDescriptor GetField(Type fieldType);
    }
}



using EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Environment.Shell.Scope;
using System.Linq;

namespace EaysOC.GraphQL.Queries.Types
{
    public class ContentTypePickerGraphType : NonNullGraphType<EnumerationGraphType>
    {
        public ContentTypePickerGraphType()
        {
            var pickerType= new EnumerationGraphType();
            var contentTypes = contentDefinitionManager.LoadTypeDefinitions()
                .Select(x =>
                {
                    var stereotype = x.Settings.ToObject<ContentTypeSettings>().Stereotype;
                    return new
                    {
                        x.DisplayName, x.Name, Stereotype = stereotype
                    };
                }
                )
                .OrderBy(x => x.Stereotype);
            foreach (var typeDef in contentTypes)
            {
                pickerType.AddValue(typeDef.Name, typeDef.DisplayName, typeDef.Name, deprecationReason: typeDef.Stereotype);
            }

            Name = "ContentType";
            Description = "Picker a contentType";
            ResolvedType = pickerType;
        }
    }
}

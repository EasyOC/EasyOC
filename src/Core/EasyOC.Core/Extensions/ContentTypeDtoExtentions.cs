using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentManagement.Metadata.Settings;
using System.Collections.Generic;
using System.Linq;

namespace EasyOC
{
    public static class ContentTypeDtoExtentions
    {
        public static ContentTypeDefinitionDto ToDto(this ContentTypeDefinition typeDefinition, bool incloudeSettings = true)
        {
            var dto = new ContentTypeDefinitionDto
            {
                Name = typeDefinition.Name,
                DisplayName = typeDefinition.DisplayName,
                Settings = incloudeSettings ? typeDefinition.Settings : null,
                Parts = typeDefinition.Parts.Select(p
                =>
                {
                    return new ContentTypePartDefinitionDto
                    {
                        Name = p.Name,
                        DisplayName = p.DisplayName(),
                        Description = p.Description(),
                        PartDefinition = p.PartDefinition.ToDto(incloudeSettings),
                    };
                })
            };
            return dto;
        }

        public static ContentPartDefinitionDto ToDto(this ContentPartDefinition contentPartDefinition, bool withSettings = true)
        {
            return new ContentPartDefinitionDto
            {
                Fields = contentPartDefinition.GetFields(),
                Name = contentPartDefinition.Name,
                Description = contentPartDefinition.Description(),
                DisplayName = contentPartDefinition.DisplayName(),
                Settings = withSettings ? contentPartDefinition.Settings : null
            };
        }

        public static IEnumerable<ContentPartFieldDefinitionDto> GetFields(this ContentPartDefinition partDefinition, bool withSettings = true)
        {
            return partDefinition.Fields.Select(f => new ContentPartFieldDefinitionDto
            {
                Name = f.Name,
                FieldDefinition = new ContentFieldDefinitionDto { Name = f.FieldDefinition.Name },
                Description = f.Description(),
                DisplayName = f.DisplayName(),
                Settings = withSettings ? f.Settings : null
            });
        }
    }
}

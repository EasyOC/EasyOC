using AutoMapper;
using OrchardCore.ContentManagement.Metadata.Models;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos
{
    [AutoMap(typeof(ContentTypeDefinition))]
    public class ContentTypeDefinitionDto : ContentDefinitionDto
    {
        public string DisplayName { get; set; }
    }
}

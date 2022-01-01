using AutoMapper;
using OrchardCore.ContentManagement.Metadata.Models;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos
{
    [AutoMap(typeof(ContentTypeDefinition))]
    public class ContentTypeListItemDto 
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Stereotype { get; internal set; }
        public string Name { get; set; }

    }
}

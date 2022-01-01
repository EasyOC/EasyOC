using AutoMapper;
using AutoMapper.Configuration.Annotations;
using EasyOC.Core;
using EasyOC.Core.Swagger.Attributes;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement.Metadata.Models;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos
{
    [AutoMap(typeof(ContentTypeDefinition))]
    public class ContentTypeListItemDto
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Stereotype { get; internal set; }

    }
}

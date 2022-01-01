using AutoMapper;
using AutoMapper.Configuration.Annotations;
using EasyOC.Core;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement.Metadata.Models;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos
{
    [AutoMap(typeof(ContentTypeDefinition))]
    public class ContentTypeListItemDto : ContentDefinitionDto
    {
        public string DisplayName { get; set; }
        public string Stereotype { get; internal set; }
        [Ignore]
        [IgnoreMap]
        public override object Settings { get => base.Settings  ; set => base.Settings = value; }

    }
}

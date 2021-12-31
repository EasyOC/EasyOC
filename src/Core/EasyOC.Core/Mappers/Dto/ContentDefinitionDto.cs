using AutoMapper;
using AutoMapper.Configuration.Annotations;
using EasyOC.Core.Mappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement.Metadata.Models;
using System.Collections.Generic;

namespace EasyOC
{
    [AutoMap(typeof(ContentTypeDefinition))]

    public class ContentTypeDefinitionDto : ContentDefinitionDto
    {
        public string DisplayName { get; set; }

        public IEnumerable<ContentTypePartDefinitionDto> Parts { get; set; }

    }


    [AutoMap(typeof(ContentTypePartDefinition))]
    public class ContentTypePartDefinitionDto : ContentDefinitionDto
    {
        public ContentPartDefinitionDto PartDefinition { get; set; }

    }
    [AutoMap(typeof(ContentPartDefinition))]
    public class ContentPartDefinitionDto : ContentDefinitionDto
    {
        public IEnumerable<ContentPartFieldDefinitionDto> Fields { get; set; }

    }

    [AutoMap(typeof(ContentPartFieldDefinition))]
    public class ContentPartFieldDefinitionDto : ContentDefinitionDto
    {
        public ContentFieldDefinitionDto FieldDefinition { get; set; }

    }

    [AutoMap(typeof(ContentFieldDefinition))]
    public class ContentFieldDefinitionDto
    {
        public string Name { get; set; }

    }

    [AutoMap(typeof(ContentDefinition))]
    public class ContentDefinitionDto
    {
        public string Name { get; set; }
        //[ValueConverter(typeof(JObjectConverter))]
        //[JsonProperty(PropertyName = "Settings")]

        public virtual object Settings { get; set; }
    }
}

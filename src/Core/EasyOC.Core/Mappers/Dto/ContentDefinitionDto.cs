using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement.Metadata.Models;
using System.Collections.Generic;

namespace EasyOC
{
    //[AutoMap(typeof(ContentTypeDefinition))]
    /// <summary>
    /// Dto of <see cref="ContentTypeDefinition"/>
    /// Converting Method <see cref="ContentTypeDtoExtentions"/>
    /// </summary>
    public class ContentTypeDefinitionDto : ContentDefinitionDto
    {
        public string DisplayName { get; set; }

        public IEnumerable<ContentTypePartDefinitionDto> Parts { get; set; }

    }


    //[AutoMap(typeof(ContentTypePartDefinition))]
    public class ContentTypePartDefinitionDto : ContentDefinitionDto
    {
        public ContentPartDefinitionDto PartDefinition { get; set; }
        public string DisplayName { get; set; }
        public string Description { get;  set; }
    }
    //[AutoMap(typeof(ContentPartDefinition))]
    /// <summary>
    /// Dto of <see cref="ContentPartDefinition"/>
    /// </summary>
    public class ContentPartDefinitionDto : ContentDefinitionDto
    {
        public IEnumerable<ContentPartFieldDefinitionDto> Fields { get; set; }
        public string DisplayName { get; set; }
        public string Description { get;  set; }
    }

    //[AutoMap(typeof(ContentPartFieldDefinition))]
    public class ContentPartFieldDefinitionDto : ContentDefinitionDto
    {
        public ContentFieldDefinitionDto FieldDefinition { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }

    //[AutoMap(typeof(ContentFieldDefinition))]
    public class ContentFieldDefinitionDto
    {
        public string Name { get; set; }

    }

    //[AutoMap(typeof(ContentDefinition))]
    public class ContentDefinitionDto
    {
        public string Name { get; set; }
        public virtual JObject Settings { get; set; }
    }
}

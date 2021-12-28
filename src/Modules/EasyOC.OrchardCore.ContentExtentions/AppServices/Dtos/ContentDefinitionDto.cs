using AutoMapper;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement.Metadata.Models;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos
{
    [AutoMap(typeof(ContentDefinition))]
    public class ContentDefinitionDto
    {
        public string Name { get; set; }
        //public JObject Settings { get; set; }
    }
}

using EasyOC.OrchardCore.ContentExtentions.Models;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Contents.Models;
using System.Collections.Generic;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos;

public class EditViewContentDefinitionDto
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public ContentTypeSettings Settings { get; set; } = new ContentTypeSettings();
    public FullTextAspectSettings FullTextOption { get; set; } = new FullTextAspectSettings();
    public List<ContentFieldsMappingDto> Fields { get; set; }
}

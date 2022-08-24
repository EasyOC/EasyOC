using EasyOC.ContentExtensions.Models;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Contents.Models;
using System.Collections.Generic;

namespace EasyOC.ContentExtensions.AppServices.Dtos;

public class EditViewContentDefinitionDto
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public ContentTypeSettings Settings { get; set; } = new ContentTypeSettings();
    public FullTextAspectSettings FullTextOption { get; set; } = new FullTextAspectSettings();
    public List<ContentFieldsMappingDto> Fields { get; set; }
}

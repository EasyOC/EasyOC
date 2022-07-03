using System.Collections.Generic;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos;

public class ImportContentInput
{
    public List<ContentModel> InputList { get; set; }
    public string ContentType { get; set; }
    public bool Draft { get; set; } = false;
}
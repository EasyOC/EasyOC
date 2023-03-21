using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata.Models;

namespace EasyOC.ContentExtensions.ViewModels
{
    public class DisplayJsonFieldViewModel
    {
        public string Value { get; set; };
        // public Fields.JsonField Field { get; set; }
        public ContentPart Part { get; set; }
        public ContentPartFieldDefinition PartFieldDefinition { get; set; }
    }
}

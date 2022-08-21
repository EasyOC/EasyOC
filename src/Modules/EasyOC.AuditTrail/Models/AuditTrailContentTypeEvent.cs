using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using System.Linq;

namespace EasyOC.AuditTrail.Models
{
    public class AuditTrailContentTypeEvent
    {
        public string Name { get; set; } = "ContentType";
        public JObject EventContext { get; set; }
        public string TypeName { get; set; }
        public string PartName { get; set; }
        public string FieldName { get; set; }
        public override string ToString()
        {
            var arr = new[]
            {
                TypeName, PartName, FieldName
            };
            return string.Join(".", arr.Where(x => x != null));
        }
    }
}

using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.DynamicTypeIndex.Models
{
    public class DynamicIndexCachePakage
    {
        public ContentItem Document { get; set; }
        public DynamicIndexConfigModel ConfigModel { get; set; }
        public Type EntityType { get; set; }
    }
}

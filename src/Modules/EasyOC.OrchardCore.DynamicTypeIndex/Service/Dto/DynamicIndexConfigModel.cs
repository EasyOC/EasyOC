using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.DynamicTypeIndex.Service.Dto
{
    public class DynamicIndexConfigModel
    {
        public string ContentItemId { get; set; }
        public string TypeName { get; set; }
        public string TableName { get; set; }
        public bool Enabled { get; set; }

        public List<DynamicIndexFieldItem> Fields { get; set; } = new List<DynamicIndexFieldItem>();
    }
 
}

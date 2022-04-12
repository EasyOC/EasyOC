using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.DynamicTypeIndex.Models
{
    public class DynamicIndexConfigModel
    {
        public string ContentItemId { get; set; }
        public string TypeName { get; set; }
        public string TableName { get; set; }
        public DynamicIndexEntityInfo EntityInfo { get; set; } = new DynamicIndexEntityInfo();
        public List<DynamicIndexFieldItem> Fields { get; set; } = new List<DynamicIndexFieldItem>();
    }

    public class DynamicIndexEntityInfo
    {
        public string EntityName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string FullName
        {
            get
            {
                return $"{NameSpace}.{EntityName}";
            }
        }
        public string NameSpace { get; set; }
        public string EntityContent { get; set; }
    }
}

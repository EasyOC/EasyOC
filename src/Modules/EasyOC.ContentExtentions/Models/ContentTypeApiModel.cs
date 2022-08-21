using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace EasyOC.OrchardCore.ContentExtentions.Models
{

    public class ContentTypeApiModel : CommonContentDefApiModel
    {
        public string DisplayName { get; set; }
        public IEnumerable<ContentPartApiModel> Parts { get; set; }
    }

    public class ContentPartApiModel : CommonContentDefApiModel
    {

        public IEnumerable<ContentFiledsApiModel> Fields { get; set; }

    }

    public class ContentFiledsApiModel : CommonContentDefApiModel
    {
        public string DisplayName { get; set; }
        public string FieldTypeName { get; set; }
    }


    public class CommonContentDefApiModel
    {
        public string Name { get; set; }
        public JObject Settings { get; set; }
    }
}




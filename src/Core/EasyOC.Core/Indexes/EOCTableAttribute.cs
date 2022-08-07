using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.Core.Indexes
{
    public class EOCTableAttribute : TableAttribute
    {
        public EOCTableAttribute(string collection = default)
        {
            Collection = collection;
        }

        public string Collection { get; set; }


    }
}

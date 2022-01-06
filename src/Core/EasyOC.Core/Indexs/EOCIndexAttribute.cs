using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.Core.Indexs
{
    public class EOCIndexAttribute : IndexAttribute
    {

        public string Collection { get; set; } = string.Empty;

        public EOCIndexAttribute(string name, string fields, string collection = default) : base(name, fields)
        {
            Rename(name, collection);
        }
  

        public EOCIndexAttribute(string name, string fields, bool isUnique, string collection = default) : base(name, fields, isUnique)
        {
            Rename(name, collection);
        }

        private void Rename(string name, string collection)
        {
            Collection = collection;
            if (!collection.IsNullOrWhiteSpace())
            {
                Name = String.Format("{0}_{1}", collection, name);
            }
        }
    }
}

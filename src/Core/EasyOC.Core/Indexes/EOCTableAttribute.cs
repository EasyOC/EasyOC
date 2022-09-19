using FreeSql.DataAnnotations;

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

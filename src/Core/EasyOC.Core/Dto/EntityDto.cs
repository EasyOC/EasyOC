using AutoMapper;
using Newtonsoft.Json.Linq;
using OrchardCore.Entities;

namespace EasyOC
{
    [AutoMap(typeof(Entity), ReverseMap = true)]
    public class EntityDto
    {
        public JObject Properties
        {
            get;
            set;
        } = new JObject();
    }
}

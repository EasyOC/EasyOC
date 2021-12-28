using AutoMapper;
using Newtonsoft.Json.Linq;
using OrchardCore.Entities;

namespace EasyOC.OrchardCore.OpenApi
{
    [AutoMap(typeof(Entity), ReverseMap = true)]
    internal class EntityDto
    {
        public JObject Properties
        {
            get;
            set;
        } = new JObject();
    }
}

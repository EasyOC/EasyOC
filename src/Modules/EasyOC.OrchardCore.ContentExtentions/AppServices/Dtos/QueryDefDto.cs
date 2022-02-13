using AutoMapper;
using OrchardCore.Lucene;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos
{
    [AutoMap(typeof(LuceneQuery),ReverseMap =true)]
    public class QueryDefDto
    {
        public string Name { get; set; }
        public string Schema { get; set; }
        
        public bool ReturnContentItems { get; set; }
    }
}

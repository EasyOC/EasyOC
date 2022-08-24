using AutoMapper;
using OrchardCore.Data;
using OrchardCore.Lucene;
using OrchardCore.Queries.Sql;

namespace EasyOC.ContentExtensions.AppServices.Dtos
{
    [AutoMap(typeof(LuceneQuery), ReverseMap = true)]
    [AutoMap(typeof(SqlQuery), ReverseMap = true)]
    public class QueryDefDto
    {
        public string Name { get; set; }
        public string Schema { get; set; }
        public bool HasTotal { get; set; }
        public bool ReturnContentItems { get; set; }
    }
}

using AutoMapper;
using OrchardCore.Queries;
using System.Collections.Generic;
using LuceneQueryResults = OrchardCore.Lucene.LuceneQueryResults;

namespace EasyOC.GraphQL.Queries.Types
{
    [AutoMap(typeof(LuceneQueryResults))]
    public class TotalQueryResults : IQueryResults
    {
        public IEnumerable<object> Items { get; set; }

        public int? Total { get; set; }
    }
}

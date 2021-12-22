using OrchardCore.Queries;
using System.Collections.Generic;

namespace StatCan.OrchardCore.Queries.GraphQL
{
    public class GraphQLQueryResults : IQueryResults
    {
        public IEnumerable<object> Items { get; set; }
    }
}




using Newtonsoft.Json.Linq;
using OrchardCore.Queries;
using StatCan.OrchardCore.Queries.GraphQL.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StatCan.OrchardCore.Queries.GraphQL
{
    public class GraphQLQuerySource : IQuerySource
    {
        private readonly IGraphQLQueryService _queryService;

        public GraphQLQuerySource(IGraphQLQueryService queryService)
        {
            _queryService = queryService;
        }

        public string Name => "GraphQL";

        public Query Create()
        {
            return new GraphQLQuery();
        }

        public async Task<IQueryResults> ExecuteQueryAsync(Query query, IDictionary<string, object> parameters)
        {
            var graphQLQuery = query as GraphQLQuery;
            var graphQLQueryResults = new GraphQLQueryResults();

            var result = await _queryService.ExecuteQuery(graphQLQuery.Template, parameters);

            graphQLQueryResults.Items = new List<JObject> { JObject.FromObject(result.Result) };

            return graphQLQueryResults;
        }
    }
}




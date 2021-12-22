using System.Collections.Generic;
using System.Threading.Tasks;

namespace StatCan.OrchardCore.Queries.GraphQL.Services
{
    public interface IGraphQLQueryService
    {
        Task<ExecuteGQLQueryResults> ExecuteQuery(string queryTemplate, IDictionary<string, object> parameters);
    }
}




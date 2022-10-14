using GraphQL;
using OrchardCore.Apis.GraphQL;
using System.Threading.Tasks;

namespace EasyOC.GraphQL.Servicies
{
    public interface IGraphqlExecuterService
    {
        Task<ExecutionResult> ExecuteQuery(GraphQLRequest request);
    }
}
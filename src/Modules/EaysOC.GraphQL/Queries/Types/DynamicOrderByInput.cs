using GraphQL.Types;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;

namespace EaysOC.GraphQL.Queries.Types
{
    public class DynamicOrderByInput : InputObjectGraphType
    {
        public DynamicOrderByInput()
        {
            Name = "DynamicOrderByInput";
            Field<NonNullGraphType<StringGraphType>>("field");
            Field<NonNullGraphType<OrderByDirectionGraphType>>("direction");
        }
    }
}

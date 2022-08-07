using GraphQL.Types;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;

namespace EaysOC.GraphQL.Queries.Types
{
    public class DynamicOrderByInput : InputObjectGraphType
    {
        public DynamicOrderByInput()
        {
            Name = "DynamicOrderByInput";
            Field<NonNullGraphType<StringGraphType>>("field",description:"partName.fieldName ，参照Graphql 执行结果返回格式");
            Field<NonNullGraphType<OrderByDirectionGraphType>>("direction");
        }
    }
}

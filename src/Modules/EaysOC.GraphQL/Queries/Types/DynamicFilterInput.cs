using FreeSql.Internal.Model;
using GraphQL.Types;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using System;

namespace EaysOC.GraphQL.Queries.Types
{
    public class DynamicFilterInput : InputObjectGraphType<DynamicFilterInfo>
    {
        public DynamicFilterInput()
        {
            Name = "DynamicFilterInput";
            Field<StringGraphType>("Field", resolve: context => context.Source.Field);
            Field<DynamicFilterOperatorGraphType>("Operator", resolve: context => context.Source.Operator);
            Field<IdGraphType>("Value", resolve: context => context.Source.Value);
            Field<DynamicFilterLogicGraphType>("Logic", resolve: context => context.Source.Logic);
            Field<ListGraphType<DynamicFilterInput>>("Filters", resolve: context => context.Source.Filters);

        }
    }
    public class DynamicFilterOperatorGraphType : EnumerationGraphType
    {
        public DynamicFilterOperatorGraphType()
        {
            this.Name = "DynamicFilterOperator";
            this.Description = "the Dynamic Filter Operator";
            foreach (var name in Enum.GetNames(typeof(DynamicFilterOperator)))
            {
                this.AddValue(name, name, (object)Enum.Parse(typeof(DynamicFilterOperator), name));
            }
        }
    }
    public class DynamicFilterLogicGraphType : EnumerationGraphType
    {
        public DynamicFilterLogicGraphType()
        {
            this.Name = "DynamicFilterLogic";
            this.Description = "Dynamic Filter Logic";
            foreach (var name in Enum.GetNames(typeof(DynamicFilterLogic)))
            {
                this.AddValue(name, name, (object)Enum.Parse(typeof(DynamicFilterLogic), name));
            }
        }
    }
}

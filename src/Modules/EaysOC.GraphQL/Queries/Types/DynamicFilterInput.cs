using FreeSql.Internal.Model;
using GraphQL.Types;
using GraphQL.Utilities;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EaysOC.GraphQL.Queries.Types
{
    public class DynamicFilterInput : InputObjectGraphType<DynamicFilterInfo>
    {
        public DynamicFilterInput()
        {
            Name = "DynamicFilterInput";
            Description = "DynamicFilterInput";
            Field<StringGraphType>("Value", resolve: context => context.Source?.Value
            // ,
            // description: "查询值,如果是数组或日期，请传入Json字符串"
            );
            Field<StringGraphType>("Field", description: "partName.fieldName ，参照Graphql 执行结果返回格式",
            resolve: context => context.Source.Field);
            Field<ListGraphType<DynamicFilterInput>>("Filters", resolve: context => context.Source.Filters);
            Field<DynamicFilterOperatorGraphType>("Operator",
            resolve: context => context.Source?.Operator);
            Field<DynamicFilterLogicGraphType>("Logic", resolve: context => context.Source?.Logic);
        }
    }
    public class DynamicFilterOperatorGraphType : EnumerationGraphType<DynamicFilterOperator>
    {
        // public DynamicFilterOperatorGraphType()
        // {
        //     Type enumType = typeof(DynamicFilterOperator);
        //     this.Name = this.Name ?? StringUtils.ToPascalCase(enumType.Name);
        //     foreach (string name in Enum.GetNames(enumType).Where(x => !"Equal,Equals".Contains(x)))
        //         this.AddValue(this.ChangeEnumCase(((IEnumerable<MemberInfo>)
        //             enumType.GetMember(name,
        //             BindingFlags.DeclaredOnly
        //             | BindingFlags.Static | BindingFlags.Public))
        //         .First<MemberInfo>().Name),
        //         (string)null, Enum.Parse(enumType, name));
        //
        // }
    }
    public class DynamicFilterLogicGraphType : EnumerationGraphType<DynamicFilterLogic>
    {
        // public DynamicFilterLogicGraphType()
        // {
        //     this.Name = "DynamicFilterLogic";
        //     this.Description = "Dynamic Filter Logic";
        //     foreach (var name in Enum.GetNames(typeof(DynamicFilterLogic)))
        //     {
        //         this.AddValue(name, name, (object)Enum.Parse(typeof(DynamicFilterLogic), name));
        //     }
        // }
    }
}

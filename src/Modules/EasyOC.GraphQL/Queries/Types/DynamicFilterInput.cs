﻿using FreeSql.Internal.Model;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyOC.GraphQL.Queries.Types
{
    public class DynamicFilterInput : InputObjectGraphType<DynamicFilterInfo>
    {
        public DynamicFilterInput()
        {
            Name = "DynamicFilterInput";
            Description = "DynamicFilterInput";
            Field<IdGraphType>("Value", resolve: context => context.Source?.Value
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
    public class DynamicFilterOperatorGraphType : EnumerationGraphType
    {
        public DynamicFilterOperatorGraphType()
        {
            string[] ExceptedValues =
            {
                "Custom"
            };

            Type enumType = typeof(DynamicFilterOperator);
            foreach (string name in Enum.GetNames(enumType).Where(x => !ExceptedValues.Contains(x)))
            {
                AddValue(
                (enumType.GetMember(name,
                    BindingFlags.DeclaredOnly
                    | BindingFlags.Static | BindingFlags.Public))
                .First<MemberInfo>().Name.ToUpper(),
                (string)null, Enum.Parse(enumType, name));
            }

        }
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

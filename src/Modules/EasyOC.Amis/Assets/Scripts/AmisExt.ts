enum DynamicFilterOperator {
    /// <summary>
    /// like
    /// </summary>
    Contains = "Contains",
    StartsWith = "StartsWith",
    EndsWith = "EndsWith",
    NotContains = "NotContains",
    NotStartsWith = "NotStartsWith",
    NotEndsWith = "NotEndsWith",

    /// <summary>
    /// =<para></para>
    /// Equal/Equals/Eq 效果相同
    /// </summary>
    Equal = "Equal",
    /// <summary>
    /// =<para></para>
    /// Equal/Equals/Eq 效果相同
    /// </summary>
    Equals = "Equals",
    /// <summary>
    /// =<para></para>
    /// Equal/Equals/Eq 效果相同
    /// </summary>
    Eq = "Eq",
    /// <summary>
    /// &lt;&gt;
    /// </summary>
    NotEqual = "NotEqual",

    /// <summary>
    /// &gt;
    /// </summary>
    GreaterThan = "GreaterThan",
    /// <summary>
    /// &gt;=
    /// </summary>
    GreaterThanOrEqual = "GreaterThanOrEqual",
    /// <summary>
    /// &lt;
    /// </summary>
    LessThan = "LessThan",
    /// <summary>
    /// &lt;=
    /// </summary>
    LessThanOrEqual = "LessThanOrEqual",

    /// <summary>
    /// &gt;= and &lt;<para></para>
    /// 此时 Value 的值格式为逗号分割：value1,value2 或者数组
    /// </summary>
    Range = "Range",

    /// <summary>
    /// &gt;= and &lt;<para></para>
    /// 此时 Value 的值格式为逗号分割：date1,date2 或者数组<para></para>
    /// 这是专门为日期范围查询定制的操作符，它会处理 date2 + 1，比如：<para></para>
    /// 当 date2 选择的是 2020-05-30，那查询的时候是 &lt; 2020-05-31<para></para>
    /// 当 date2 选择的是 2020-05，那查询的时候是 &lt; 2020-06<para></para>
    /// 当 date2 选择的是 2020，那查询的时候是 &lt; 2021<para></para>
    /// 当 date2 选择的是 2020-05-30 12，那查询的时候是 &lt; 2020-05-30 13<para></para>
    /// 当 date2 选择的是 2020-05-30 12:30，那查询的时候是 &lt; 2020-05-30 12:31<para></para>
    /// 并且 date2 只支持以上 5 种格式 (date1 没有限制)
    /// </summary>
    DateRange = "DateRange",
    /// <summary>
    /// in (1,2,3)<para></para>
    /// 此时 Value 的值格式为逗号分割：value1,value2,value3... 或者数组
    /// </summary>
    Any = "Any",
    /// <summary>
    /// not in (1,2,3)<para></para>
    /// 此时 Value 的值格式为逗号分割：value1,value2,value3... 或者数组
    /// </summary>
    NotAny = "NotAny"

}

enum DynamicFilterLogic { And = "And", Or = "Or" }

class DynamicFilterInfo {
    field?: string
    operator?: DynamicFilterOperator
    value?: any
    logic?: DynamicFilterLogic
    filters?: DynamicFilterInfo[] = []
}

function convertToJSONFilter(condition: any): DynamicFilterInfo[] {
    const arr: DynamicFilterInfo[] = condition.map((child: any) => {
        const filterItem = {
            logic: child.conjunction ?? DynamicFilterLogic.And,
            filters: []
        } as DynamicFilterInfo
        if (child.left && child.left.field && !child.children || child.children.length == 0) {
            // 如果子节点有left、op和right, 则转换为graphql形式的filter,否则返回空字符串
            if (child.left && child.op && child.right) {
                const props = {field: child.left.field, operator: child.op, value: child.right};
                switch (props.operator) {
                    case 'between':
                        filterItem.operator = DynamicFilterOperator.Range
                        break
                    case 'select_any_in':
                        filterItem.operator = DynamicFilterOperator.Any
                        break
                    case 'select_not_any_in':
                        filterItem.operator = DynamicFilterOperator.NotAny
                        break
                    default: {
                        //@ts-ignore
                        const targetKey = props.operator?.replaceAll("_", "").toLowerCase() as string
                        for (const key in DynamicFilterOperator) {
                            if (targetKey == (key.toLowerCase())) {
                                //@ts-ignore
                                filterItem.operator = DynamicFilterOperator[key]
                                break
                            }
                        }
                    }
                }
                filterItem.field = props.field;
                filterItem.value = props.value;
            }

        } else if (child.children && child.children.length > 0) {
            // 如果有子节点, 则转换为graphql形式的filter
            filterItem.filters = convertToJSONFilter(child.children);
        }
        return filterItem
    });
    return arr
}


//@ts-ignore
function dynamicJsonFilter(condition: any) {

    let dynamicFilter = {
        filters: []
    } as DynamicFilterInfo;
    // 如果有子节点, 则转换为DynamicFilter形式的filter
    if (condition?.children?.length > 0) {
        const children = condition.children;
        // 利用递归将筛选转换为graphql形式的filter字符串
        const mapResult = convertToJSONFilter(children);
        if (mapResult)
            if (mapResult?.length > 1) {
                //@ts-ignore
                dynamicFilter.logic = condition.conjunction;
                dynamicFilter.filters = mapResult;
            } else {
                dynamicFilter = mapResult[0];
            }
    }

    return JSON.stringify(dynamicFilter)

    // return "{\"field\":\"address.city\",\"operator\":\"Eq\",\"value\":[]}"
}


function genGraphqlFilter(children: any) {
    console.log('genGraphqlFilter children: ', children);
    const arr = children.map((child: any) => {
        if (child.left && child.left.field) {
            if (child.left && child.op && child.right) {
                const filter = {field: child.left.field, operator: "", value: child.right};

                switch (child.op) {
                    case 'between':
                        filter.operator = "RANGE"
                        break
                    case 'select_any_in':
                        filter.operator = "ANY"
                        break
                    case 'select_not_any_in':
                        filter.operator = "NOT_ANY"
                        break
                    default:
                        filter.operator = child.op
                        break
                }

                let filterStringJoin = '';
                for (const item in filter) {
                    if (filterStringJoin) {
                        filterStringJoin = filterStringJoin + ","
                    }
                    if (item === "field" || item === "value") {
                        filterStringJoin = filterStringJoin + item + ':"' + filter[item] + '"'
                    } else if (item === "operator") {
                        filterStringJoin = filterStringJoin + item + ':' + filter[item]
                    } else {
                        console.error("filter[item]: ", filter, item)
                        return "{}";
                    }

                }

                return "{" + filterStringJoin + "}";
            } else {
                return "{}";
            }
        } else if (child.children && child.children.length > 0) {
            const genChilds = genGraphqlFilter(child.children)
            if (genChilds === "[]") {
                return "{}"
            } else {
                return "{" + "logic:" + child.conjunction + ",filters:" + genChilds + "}";
            }
        } else {
            return "{}";
        }
    }).filter((item: any) => item !== "{}");

    console.log('arr: ', arr);
    if (arr.length > 0) {
        return "[" + arr.join(",") + "]";

    } else {
        return "[]";
    }
    //return JSON.stringify(arr);
}


//@ts-ignore
window.amisExt = {
    dynamicJsonFilter,
    convertCondition: function (condition: any) {
        console.log(condition, "convertCondition")
        let filterString = "";
        // 如果有子节点, 则转换为graphql形式的filter
        if (condition && condition.children && condition.children.length > 0) {
            const children = condition.children;
            console.log('children: ', children);
            // 利用递归将筛选转换为graphql形式的filter字符串
            filterString = "{" + "logic:" + condition.conjunction + "," + "filters:" + genGraphqlFilter(children) + "}";
        }

        if (filterString) {
            return filterString;

        } else {
            return "{}";
        }
    }
}

using System;
using EasyOC.OrchardCore.DynamicTypeIndex.Index;
using FreeSql;
using Microsoft.AspNetCore.Mvc.Localization;
using OrchardCore.ContentManagement.Records;
using OrchardCore.DisplayManagement.Notify;

namespace EaysOC.GraphQL.Handlers;

public class DefaultPagedContentItemQueryWhereFilter : IPagedContentItemQueryWhereFilter
{
    public int OrderIndex { get; set; } = 0;
    private readonly INotifier _notifier;
    private readonly IHtmlLocalizer H;

    public DefaultPagedContentItemQueryWhereFilter(INotifier notifier, IHtmlLocalizer<DefaultPagedContentItemQueryWhereFilter> h)
    {
        _notifier = notifier;
        H = h;
    }

    public ISelect<ContentItemIndex, DIndexBase> WhereFilter(ISelect<ContentItemIndex, DIndexBase> prepareQuery,
        string contentTypeName, Action<bool> resultCallBack, IFreeSql freeSql = null)
    {
        if (contentTypeName == "AmisSchema")
        {
            return prepareQuery;
        }

        _notifier.ErrorAsync(H[$"没有权限访问此数据字段：published"]);
        resultCallBack(false);
        return prepareQuery.Where((a, b) =>
            a.Published == true &&
            a.Latest == true);
    }
}